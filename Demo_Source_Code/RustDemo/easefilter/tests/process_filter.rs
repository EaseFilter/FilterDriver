//! Integration tests for PROCESS mode (process/thread monitor & control).
//!
//! All tests modify system state (installing/uninstalling a kernel driver)
//! and require Administrator privileges. Run from an elevated prompt:
//!
//! ```text
//! set EASEFILTER_TEST_LICENSE_KEY=your-key-here
//! cargo test --test process_filter
//! ```

mod common;

use std::sync::{
    Arc, Mutex,
    atomic::{AtomicBool, AtomicU32, Ordering},
};
use std::time::Duration;

use common::{RULE_ACTIVATION_DELAY, require_admin, temp_dir, wait_until_true};
use easefilter::FilterController;
use easefilter::enums::{FilterType, ProcessControlFlag, ProcessEventType};
use easefilter::events::{DenyReply, Event, Reply};
use easefilter::rules::{FilterRule, FilterRuleInstalled, ProcessRule};
use serial_test::serial;

/// Thread creation/termination monitoring.
#[test]
#[serial]
fn process_thread_monitor() {
    require_admin();

    // create threads, and have each thread create a sub-thread.
    // check that the TIDs of the first-level threads are properly registered.

    let expected_tids = Arc::new(Mutex::new(Vec::new()));
    let created_tids = Arc::new(Mutex::new(Vec::new()));
    let terminated_tids = Arc::new(Mutex::new(Vec::new()));
    let wrong = Arc::new(AtomicBool::new(false));

    let exe_path = std::env::current_exe().unwrap();
    let exe_str = exe_path.to_str().unwrap().to_string();

    let mut ef = FilterController::new();
    {
        let created_tids = created_tids.clone();
        let terminated_tids = terminated_tids.clone();
        let wrong = wrong.clone();
        ef.on_message(move |event: &Event, _can_reply: bool| {
            let Event::Process(process_event) = event else {
                wrong.store(true, Ordering::SeqCst);
                return None::<Box<dyn Reply>>;
            };
            match process_event.kind {
                ProcessEventType::ThreadCreated => {
                    created_tids.lock().unwrap().push(process_event.process.tid);
                }
                ProcessEventType::ThreadTerminated => {
                    terminated_tids
                        .lock()
                        .unwrap()
                        .push(process_event.process.tid);
                }
                _ => {
                    wrong.store(true, Ordering::SeqCst);
                }
            }
            None::<Box<dyn Reply>>
        });
    }
    ef.start(common::TEST_LICENSE_KEY, FilterType::PROCESS)
        .unwrap();

    let rule = ProcessRule {
        executable_mask: exe_str,
        control_flag: ProcessControlFlag::THREAD_CREATION_NOTIFICATION
            | ProcessControlFlag::THREAD_TERMINATION_NOTIFICATION,
    };
    let installed = rule.install(&mut ef).unwrap();

    std::thread::sleep(RULE_ACTIVATION_DELAY);

    let n_threads: u32 = 5;
    let mut handles = Vec::new();
    for _ in 0..n_threads {
        let expected_tids = expected_tids.clone();
        handles.push(std::thread::spawn(move || {
            let tid = unsafe { easefilter_sys::GetCurrentThreadId() };
            expected_tids.lock().unwrap().push(tid);
            let _ = std::thread::spawn(|| {}).join();
        }));
    }
    for h in handles {
        h.join().unwrap();
    }

    let expected_tids_for_check = expected_tids.lock().unwrap().clone();

    assert!(
        wait_until_true(Duration::from_secs(3), || {
            let created = created_tids.lock().unwrap();
            expected_tids_for_check
                .iter()
                .all(|tid| created.contains(tid))
        }),
        "expected all thread TIDs in ThreadCreated events, missing some: {expected_tids_for_check:?} vs {:?}",
        created_tids.lock().unwrap()
    );
    assert!(
        wait_until_true(Duration::from_secs(3), || {
            let terminated = terminated_tids.lock().unwrap();
            expected_tids_for_check
                .iter()
                .all(|tid| terminated.contains(tid))
        }),
        "expected all thread TIDs in ThreadTerminated events, missing some: {expected_tids_for_check:?} vs {:?}",
        terminated_tids.lock().unwrap()
    );

    assert!(
        !wrong.load(Ordering::SeqCst),
        "received unexpected event type"
    );

    installed.uninstall(&mut ef).unwrap();
    ef.stop();
}

/// Static process denial via [`ProcessControlFlag::DENY_NEW_PROCESS_CREATION`]
#[test]
#[serial]
fn deny_process_static() {
    require_admin();

    let tmp = temp_dir("ef_test_deny_process_static");
    let exec_path = tmp.join("test_exec.exe");
    std::fs::copy("C:\\Windows\\System32\\cmd.exe", &exec_path).unwrap();
    let exec_str = exec_path.to_str().unwrap().to_string();

    let mut ef = FilterController::new();
    ef.on_message(|_: &Event, _: bool| None::<Box<dyn Reply>>);
    ef.start(common::TEST_LICENSE_KEY, FilterType::PROCESS)
        .unwrap();

    let rule = ProcessRule {
        executable_mask: exec_str,
        control_flag: ProcessControlFlag::DENY_NEW_PROCESS_CREATION,
    };
    let installed = rule.install(&mut ef).unwrap();

    std::thread::sleep(RULE_ACTIVATION_DELAY);

    let result = std::process::Command::new(&exec_path)
        .arg("/C")
        .arg("echo test")
        .spawn();
    assert!(
        result.is_err(),
        "spawn should fail when DENY_NEW_PROCESS_CREATION is set"
    );

    installed.uninstall(&mut ef).unwrap();

    let output = std::process::Command::new(&exec_path)
        .arg("/C")
        .arg("echo test")
        .output()
        .unwrap();
    assert!(
        output.status.success(),
        "process should run after rule is removed"
    );

    ef.stop();
    let _ = std::fs::remove_dir_all(&tmp);
}

/// Dynamic (callback-based) process denial
#[test]
#[serial]
fn deny_process_dynamic() {
    require_admin();

    let tmp = temp_dir("ef_test_deny_process_dynamic");
    let exec_path = tmp.join("deny_dynamic.exe");
    std::fs::copy("C:\\Windows\\System32\\cmd.exe", &exec_path).unwrap();
    let exec_str = exec_path.to_str().unwrap().to_string();

    let forbidden = "FORBIDDEN_CMD";
    let denied = Arc::new(AtomicBool::new(false));
    let approved = Arc::new(AtomicU32::new(0));

    let mut ef = FilterController::new();
    {
        let forbidden = String::from(forbidden);
        let denied = denied.clone();
        let approved = approved.clone();
        ef.on_message(move |event: &Event, _can_reply: bool| {
            let Event::Process(process_event) = event else {
                return None::<Box<dyn Reply>>;
            };
            if let Some(ref cmd) = process_event.command_line
                && cmd.contains(&forbidden)
            {
                denied.store(true, Ordering::SeqCst);
                return Some(Box::new(DenyReply));
            }
            approved.fetch_add(1, Ordering::SeqCst);
            None::<Box<dyn Reply>>
        });
    }
    ef.start(common::TEST_LICENSE_KEY, FilterType::PROCESS)
        .unwrap();

    let rule = ProcessRule {
        executable_mask: exec_str,
        control_flag: ProcessControlFlag::PROCESS_CREATION_NOTIFICATION,
    };
    let installed = rule.install(&mut ef).unwrap();

    std::thread::sleep(RULE_ACTIVATION_DELAY);

    // Command without forbidden string should succeed
    let mut allowed_child = std::process::Command::new(&exec_path)
        .arg("/C")
        .arg("echo allowed")
        .spawn()
        .expect("allowed command should spawn");

    assert!(
        wait_until_true(Duration::from_secs(3), || approved.load(Ordering::SeqCst)
            >= 1),
        "callback should have been invoked for allowed command"
    );
    assert!(
        allowed_child.wait().ok().is_some_and(|s| s.success()),
        "allowed command should start"
    );

    // Command with forbidden string should be denied
    let denied_result = std::process::Command::new(&exec_path)
        .arg("/C")
        .arg(format!("echo {}", forbidden))
        .spawn();

    assert!(
        wait_until_true(Duration::from_secs(3), || denied.load(Ordering::SeqCst)),
        "DenyReply should have been returned for forbidden command"
    );
    if let Ok(mut child) = denied_result {
        assert!(
            !child.wait().ok().is_some_and(|s| s.success()),
            "forbidden command should be denied"
        );
    }

    installed.uninstall(&mut ef).unwrap();
    ef.stop();
    let _ = std::fs::remove_dir_all(&tmp);
}

/// Verify PID, parent PID, command line, and other [`ProcessEvent`] fields
/// from the process creation callback.
#[test]
#[serial]
fn process_monitor_info() {
    require_admin();

    let tmp = temp_dir("ef_test_process_monitor_info");
    let exec_path = tmp.join("test_exec.exe");
    std::fs::copy("C:\\Windows\\System32\\cmd.exe", &exec_path).unwrap();
    let exec_str = exec_path.to_str().unwrap().to_string();

    let self_pid = std::process::id();
    let self_tid = unsafe { easefilter_sys::GetCurrentThreadId() };

    let process_pid = Arc::new(Mutex::new(0u32));
    let process_path = Arc::new(Mutex::new(String::new()));
    let parent_pid = Arc::new(Mutex::new(0u32));
    let parent_path_ok = Arc::new(AtomicBool::new(false));
    let creating_pid = Arc::new(Mutex::new(0u32));
    let creating_tid = Arc::new(Mutex::new(0u32));
    let cmdline = Arc::new(Mutex::new(None::<String>));
    let sid_string = Arc::new(Mutex::new(String::new()));
    let rule_id_seen = Arc::new(Mutex::new(0u32));
    let wrong = Arc::new(AtomicBool::new(false));
    let seen = Arc::new(AtomicBool::new(false));

    let mut ef = FilterController::new();
    {
        let process_pid = process_pid.clone();
        let process_path = process_path.clone();
        let parent_pid = parent_pid.clone();
        let parent_path_ok = parent_path_ok.clone();
        let creating_pid = creating_pid.clone();
        let creating_tid = creating_tid.clone();
        let cmdline = cmdline.clone();
        let sid_string = sid_string.clone();
        let rule_id_seen = rule_id_seen.clone();
        let wrong = wrong.clone();
        let seen = seen.clone();
        ef.on_message(move |event: &Event, _can_reply: bool| {
            let Event::Process(process_event) = event else {
                wrong.store(true, Ordering::SeqCst);
                return None::<Box<dyn Reply>>;
            };
            if process_event.kind != ProcessEventType::ProcessCreationInfo {
                return None::<Box<dyn Reply>>;
            }
            *process_pid.lock().unwrap() = process_event.process.pid;
            if let Ok(ref p) = process_event.process.path {
                *process_path.lock().unwrap() = p.clone();
            }
            if let Some(ref par) = process_event.parent_proc {
                *parent_pid.lock().unwrap() = par.pid;
                parent_path_ok.store(par.path.is_ok(), Ordering::SeqCst);
            }
            if let Some(ref cr) = process_event.creating_proc {
                *creating_pid.lock().unwrap() = cr.pid;
                *creating_tid.lock().unwrap() = cr.tid;
            }
            *cmdline.lock().unwrap() = process_event.command_line.clone();
            *rule_id_seen.lock().unwrap() = process_event.filter_rule_id;
            *sid_string.lock().unwrap() = process_event.sid.to_string().unwrap_or_default();
            seen.store(true, Ordering::SeqCst);
            None::<Box<dyn Reply>>
        });
    }
    ef.start(common::TEST_LICENSE_KEY, FilterType::PROCESS)
        .unwrap();

    let rule = ProcessRule {
        executable_mask: exec_str.clone(),
        control_flag: ProcessControlFlag::PROCESS_CREATION_NOTIFICATION,
    };
    let installed = rule.install(&mut ef).unwrap();
    let installed_rule_id = installed.rule_id();
    std::thread::sleep(RULE_ACTIVATION_DELAY);

    let child = std::process::Command::new(&exec_str)
        .arg("/C")
        .arg("echo process_monitor_info")
        .spawn()
        .expect("should spawn successfully");
    let child_pid = child.id();
    let output = child.wait_with_output().unwrap();
    assert!(output.status.success());

    assert!(
        wait_until_true(Duration::from_secs(5), || seen.load(Ordering::SeqCst)),
        "callback was never invoked"
    );

    assert!(!wrong.load(Ordering::SeqCst), "received non-process event");

    assert_eq!(
        *process_pid.lock().unwrap(),
        child_pid,
        "process.pid should match spawned process PID"
    );
    assert_eq!(
        *process_path.lock().unwrap(),
        exec_str,
        "process.path should match the rule executable mask"
    );
    assert_eq!(
        *parent_pid.lock().unwrap(),
        self_pid,
        "parent_proc.pid should match test process PID"
    );
    assert!(
        parent_path_ok.load(Ordering::SeqCst),
        "parent_proc.path should be Ok"
    );
    assert_eq!(
        *creating_pid.lock().unwrap(),
        self_pid,
        "creating_proc.pid should match test process PID"
    );
    assert_eq!(
        *creating_tid.lock().unwrap(),
        self_tid,
        "creating_proc.tid should match the calling thread TID"
    );
    let cmd = cmdline.lock().unwrap();
    assert!(
        cmd.as_ref()
            .is_some_and(|c| c.contains("echo") && c.contains("process_monitor_info")),
        "command_line should contain the echo command, got: {cmd:?}"
    );
    drop(cmd);
    assert_eq!(
        *rule_id_seen.lock().unwrap(),
        installed_rule_id,
        "filter_rule_id should match installed rule ID"
    );
    let sid = sid_string.lock().unwrap();
    assert!(
        sid.starts_with("S-1-"),
        "sid should start with S-1-, got: {sid}"
    );

    installed.uninstall(&mut ef).unwrap();
    ef.stop();
    let _ = std::fs::remove_dir_all(&tmp);
}
