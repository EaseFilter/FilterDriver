// (C) Copyright 2026 EaseFilter Technologies
// All Rights Reserved
//
// This software is part of a licensed software product and may
// only be used or copied in accordance with the terms of that license.
//
// NOTE:  THIS MODULE IS UNSUPPORTED SAMPLE CODE
//
// This module contains sample code provided for convenience and
// demonstration purposes only,this software is provided on an
// "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
//  either express or implied.

//! Integration tests for file monitoring.
#![allow(irrefutable_let_patterns)]
//!
//! All tests modify system state (installing/uninstalling a kernel driver)
//! and require Administrator privileges. Run from an elevated prompt:
//!
//! ```text
//! set EASEFILTER_TEST_LICENSE_KEY=your-key-here
//! cargo test --test monitor_filter
//! ```

mod common;

use std::env;
use std::fs;
use std::path::PathBuf;
use std::process::Stdio;
use std::sync::{
    Arc, Mutex,
    atomic::{AtomicBool, Ordering},
};
use std::thread;
use std::time::Duration;

use common::{require_admin, temp_dir, wait_until_true};
use easefilter::FilterController;
use easefilter::enums::{FileEventType, FilterType};
use easefilter::events::{Event, FileEventKind};
use easefilter::rules::{FileRule, FilterRule, FilterRuleInstalled};
use serial_test::serial;

// ---------------------------------------------------------------------------
// Test helpers
// ---------------------------------------------------------------------------

#[derive(Debug, Clone)]
struct FileEventInfo {
    file_name: PathBuf,
    kind: FileEventKind,
    filter_rule_id: u32,
    #[allow(dead_code)]
    pid: u32,
}

// ---------------------------------------------------------------------------
// Tests
// ---------------------------------------------------------------------------

#[test]
#[serial]
fn monitor_change_events() {
    require_admin();

    let tmp = temp_dir("ef_test_monitor_change_events");

    let events: Arc<Mutex<Vec<FileEventInfo>>> = Arc::new(Mutex::new(Vec::new()));
    let events_cb = events.clone();
    let wrong = Arc::new(AtomicBool::new(false));
    let wrong_cb = wrong.clone();

    let mut ef = FilterController::new();
    ef.on_message(move |event: &Event, _can_reply: bool| {
        if let Event::File(file_event) = event {
            if let FileEventKind::ChangeNotification(_) = &file_event.kind {
                events_cb.lock().unwrap().push(FileEventInfo {
                    file_name: file_event.file_name.clone(),
                    kind: file_event.kind.clone(),
                    filter_rule_id: file_event.filter_rule_id,
                    pid: file_event.process.pid,
                });
            }
        } else {
            wrong_cb.store(true, Ordering::SeqCst);
        }
        None::<Box<dyn easefilter::events::Reply>>
    });
    ef.start(common::TEST_LICENSE_KEY, FilterType::MONITOR)
        .unwrap();

    let pattern = format!(r"{}\*", tmp.to_str().unwrap());
    let rule = FileRule {
        file_path: pattern,
        change_event_filter: FileEventType::CREATED
            | FileEventType::WRITTEN
            | FileEventType::RENAMED
            | FileEventType::READ
            | FileEventType::DELETED,
        ..FileRule::default()
    };
    let installed = rule.install(&mut ef).unwrap();

    thread::sleep(common::RULE_ACTIVATION_DELAY);

    let test_file = tmp.join("test.txt");
    let renamed_file = tmp.join("renamed.txt");

    // CREATED
    fs::write(&test_file, b"hello").unwrap();
    assert!(
        wait_until_true(Duration::from_secs(3), || {
            events.lock().unwrap().iter().any(|e| {
            matches!(&e.kind, FileEventKind::ChangeNotification(t) if t.contains(FileEventType::CREATED))
                && e.file_name == test_file
            })
        }),
        "CREATED event did not fire"
    );
    assert!(!wrong.load(Ordering::SeqCst), "unexpected non-change event");
    events.lock().unwrap().clear();

    // WRITTEN
    fs::write(&test_file, b"world").unwrap();
    assert!(
        wait_until_true(Duration::from_secs(3), || {
            events.lock().unwrap().iter().any(|e| {
            matches!(&e.kind, FileEventKind::ChangeNotification(t) if t.contains(FileEventType::WRITTEN))
                && e.file_name == test_file
            })
        }),
        "WRITTEN event did not fire"
    );
    events.lock().unwrap().clear();

    // RENAMED
    fs::rename(&test_file, &renamed_file).unwrap();
    assert!(
        wait_until_true(Duration::from_secs(3), || {
            events.lock().unwrap().iter().any(|e| {
            matches!(&e.kind, FileEventKind::ChangeNotification(t) if t.contains(FileEventType::RENAMED))
                && e.file_name == test_file
            })
        }),
        "RENAMED event did not fire"
    );
    events.lock().unwrap().clear();

    // READ
    let _ = fs::read(&renamed_file).unwrap();
    assert!(
        wait_until_true(Duration::from_secs(3), || {
            events.lock().unwrap().iter().any(|e| {
            matches!(&e.kind, FileEventKind::ChangeNotification(t) if t.contains(FileEventType::READ))
                && e.file_name == renamed_file
            })
        }),
        "READ event did not fire"
    );
    events.lock().unwrap().clear();

    // DELETED
    fs::remove_file(&renamed_file).unwrap();
    assert!(
        wait_until_true(Duration::from_secs(3), || {
            events.lock().unwrap().iter().any(|e| {
            matches!(&e.kind, FileEventKind::ChangeNotification(t) if t.contains(FileEventType::DELETED))
                && e.file_name == renamed_file
            })
        }),
        "DELETED event did not fire"
    );
    events.lock().unwrap().clear();

    // Uninstall
    installed.uninstall(&mut ef).unwrap();
    fs::write(&test_file, b"after uninstall").unwrap();
    std::thread::sleep(Duration::from_millis(200));
    assert!(
        events.lock().unwrap().is_empty(),
        "events fired after rule was uninstalled"
    );

    ef.stop();
    let _ = fs::remove_dir_all(&tmp);
}

#[test]
#[serial]
fn monitor_rule_id_per_rule() {
    require_admin();

    let tmp = temp_dir("ef_test_monitor_rule_id");

    let events: Arc<Mutex<Vec<FileEventInfo>>> = Arc::new(Mutex::new(Vec::new()));
    let events_cb = events.clone();

    let mut ef = FilterController::new();
    ef.on_message(move |event: &Event, _can_reply: bool| {
        if let Event::File(file_event) = event {
            events_cb.lock().unwrap().push(FileEventInfo {
                file_name: file_event.file_name.clone(),
                kind: file_event.kind.clone(),
                filter_rule_id: file_event.filter_rule_id,
                pid: file_event.process.pid,
            });
        }
        None::<Box<dyn easefilter::events::Reply>>
    });
    ef.start(common::TEST_LICENSE_KEY, FilterType::MONITOR)
        .unwrap();

    let file1 = tmp.join("file_one.txt");
    let file2 = tmp.join("file_two.txt");

    let rule1 = FileRule {
        file_path: file1.to_str().unwrap().to_string(),
        change_event_filter: FileEventType::CREATED,
        ..FileRule::default()
    };
    let installed1 = rule1.install(&mut ef).unwrap();
    let id1 = installed1.rule_id();

    let rule2 = FileRule {
        file_path: file2.to_str().unwrap().to_string(),
        change_event_filter: FileEventType::CREATED,
        ..FileRule::default()
    };
    let installed2 = rule2.install(&mut ef).unwrap();
    let id2 = installed2.rule_id();

    assert_ne!(id1, id2, "rule IDs should be unique");

    thread::sleep(common::RULE_ACTIVATION_DELAY);

    fs::write(&file1, b"one").unwrap();
    fs::write(&file2, b"two").unwrap();
    assert!(
        wait_until_true(Duration::from_secs(3), || {
            let guard = events.lock().unwrap();
            guard.iter().any(|e| e.file_name == file1) && guard.iter().any(|e| e.file_name == file2)
        }),
        "expected events for both files"
    );

    let snapshot = events.lock().unwrap().clone();
    let mut seen_file1 = false;
    let mut seen_file2 = false;

    for e in &snapshot {
        if e.file_name == file1 {
            seen_file1 = true;
            assert_eq!(
                e.filter_rule_id, id1,
                "file_one should have rule_id {id1}, got {}",
                e.filter_rule_id
            );
        }
        if e.file_name == file2 {
            seen_file2 = true;
            assert_eq!(
                e.filter_rule_id, id2,
                "file_two should have rule_id {id2}, got {}",
                e.filter_rule_id
            );
        }
    }

    assert!(seen_file1, "no event for file_one");
    assert!(seen_file2, "no event for file_two");

    installed1.uninstall(&mut ef).unwrap();
    installed2.uninstall(&mut ef).unwrap();
    ef.stop();
    let _ = fs::remove_dir_all(&tmp);
}

#[test]
#[serial]
fn monitor_process_info() {
    require_admin();

    let tmp = temp_dir("ef_test_monitor_process_info");
    let fired = Arc::new(AtomicBool::new(false));
    let fired_cb = fired.clone();
    let wrong = Arc::new(AtomicBool::new(false));
    let wrong_cb = wrong.clone();
    let captured_pid = Arc::new(Mutex::new(None::<u32>));
    let captured_pid_cb = captured_pid.clone();
    let captured_path = Arc::new(Mutex::new(None::<String>));
    let captured_path_cb = captured_path.clone();

    let mut ef = FilterController::new();
    ef.on_message(move |event: &Event, _can_reply: bool| {
        let Event::File(file_event) = event else {
            return None::<Box<dyn easefilter::events::Reply>>;
        };
        if file_event.file_name.ends_with("target.txt") {
            *captured_pid_cb.lock().unwrap() = Some(file_event.process.pid);
            match &file_event.process.path {
                Ok(p) => *captured_path_cb.lock().unwrap() = Some(p.clone()),
                Err(e) => {
                    eprintln!("process path error: {e}");
                    wrong_cb.store(true, Ordering::SeqCst);
                }
            }
            fired_cb.store(true, Ordering::SeqCst);
        }
        None::<Box<dyn easefilter::events::Reply>>
    });
    ef.start(common::TEST_LICENSE_KEY, FilterType::MONITOR)
        .unwrap();

    let test_file = tmp.join("target.txt");
    fs::write(&test_file, b"content").unwrap();

    let rule = FileRule {
        file_path: test_file.to_str().unwrap().to_string(),
        change_event_filter: FileEventType::READ,
        ..FileRule::default()
    };
    let installed = rule.install(&mut ef).unwrap();

    // Let the CREATED event settle
    std::thread::sleep(Duration::from_millis(200));

    let child = std::process::Command::new("cmd.exe")
        .args(["/C", "type", test_file.to_str().unwrap()])
        .stdout(Stdio::piped())
        .stderr(Stdio::piped())
        .spawn()
        .unwrap();
    let child_pid = child.id();
    let output = child.wait_with_output().unwrap();
    assert_eq!(output.stdout, b"content");

    assert!(
        wait_until_true(Duration::from_secs(3), || fired.load(Ordering::SeqCst)),
        "event did not fire for cmd.exe read"
    );
    assert!(!wrong.load(Ordering::SeqCst), "process info was wrong");

    let got_pid = captured_pid.lock().unwrap().take();
    assert_eq!(got_pid, Some(child_pid), "PID should match cmd.exe");

    let got_path = captured_path.lock().unwrap().take();
    assert!(got_path.is_some(), "process path should be resolved");
    assert!(
        got_path.as_ref().unwrap().ends_with("cmd.exe"),
        "process path should end with cmd.exe, got {:?}",
        got_path
    );

    installed.uninstall(&mut ef).unwrap();
    ef.stop();
    let _ = fs::remove_dir_all(&tmp);
}

#[test]
#[serial]
fn monitor_sid_and_account() {
    require_admin();

    let tmp = temp_dir("ef_test_monitor_sid");
    let fired = Arc::new(AtomicBool::new(false));
    let fired_cb = fired.clone();
    let wrong = Arc::new(AtomicBool::new(false));
    let wrong_cb = wrong.clone();
    let sid_str = Arc::new(Mutex::new(None::<String>));
    let sid_str_cb = sid_str.clone();
    let acct_user = Arc::new(Mutex::new(None::<String>));
    let acct_user_cb = acct_user.clone();
    let acct_domain = Arc::new(Mutex::new(None::<String>));
    let acct_domain_cb = acct_domain.clone();

    let mut ef = FilterController::new();
    ef.on_message(move |event: &Event, _can_reply: bool| {
        let Event::File(file_event) = event else {
            return None::<Box<dyn easefilter::events::Reply>>;
        };
        if file_event.file_name.ends_with("sid_check.txt") {
            match file_event.sid.to_string() {
                Ok(s) => *sid_str_cb.lock().unwrap() = Some(s),
                Err(e) => {
                    eprintln!("SID to_string error: {e}");
                    wrong_cb.store(true, Ordering::SeqCst);
                }
            }
            match file_event.sid.lookup_account() {
                Ok(a) => {
                    *acct_user_cb.lock().unwrap() = Some(a.username);
                    *acct_domain_cb.lock().unwrap() = Some(a.domain);
                }
                Err(e) => {
                    eprintln!("SID lookup_account error: {e}");
                    wrong_cb.store(true, Ordering::SeqCst);
                }
            }
            fired_cb.store(true, Ordering::SeqCst);
        }
        None::<Box<dyn easefilter::events::Reply>>
    });
    ef.start(common::TEST_LICENSE_KEY, FilterType::MONITOR)
        .unwrap();

    let test_file = tmp.join("sid_check.txt");

    let rule = FileRule {
        file_path: test_file.to_str().unwrap().to_string(),
        change_event_filter: FileEventType::CREATED,
        ..FileRule::default()
    };
    let installed = rule.install(&mut ef).unwrap();

    thread::sleep(common::RULE_ACTIVATION_DELAY);

    fs::write(&test_file, b"sid test").unwrap();

    assert!(
        wait_until_true(Duration::from_secs(2), || fired.load(Ordering::SeqCst)),
        "no event for the created file"
    );
    assert!(!wrong.load(Ordering::SeqCst), "SID/account lookup failed");

    let s = sid_str.lock().unwrap().take();
    assert!(s.is_some(), "SID string should be captured");
    assert!(
        s.as_ref().unwrap().starts_with("S-"),
        "SID should start with 'S-', got {:?}",
        s
    );

    let user = acct_user.lock().unwrap().take();
    let domain = acct_domain.lock().unwrap().take();
    let expected_user = env::var("USERNAME").unwrap();
    let expected_domain = env::var("COMPUTERNAME").unwrap();
    assert!(
        user.as_deref()
            .is_some_and(|u| u.eq_ignore_ascii_case(&expected_user)),
        "username mismatch: got {user:?}, expected {expected_user:?}"
    );
    assert!(
        domain
            .as_deref()
            .is_some_and(|d| d.eq_ignore_ascii_case(&expected_domain)),
        "domain mismatch: got {domain:?}, expected {expected_domain:?}"
    );

    installed.uninstall(&mut ef).unwrap();
    ef.stop();
    let _ = fs::remove_dir_all(&tmp);
}

#[test]
#[serial]
fn monitor_rename_new_path() {
    require_admin();

    let tmp = temp_dir("ef_test_monitor_rename");
    let fired = Arc::new(AtomicBool::new(false));
    let fired_cb = fired.clone();
    let wrong = Arc::new(AtomicBool::new(false));
    let _wrong_cb = wrong.clone();
    let new_path = Arc::new(Mutex::new(None::<PathBuf>));
    let new_path_cb = new_path.clone();

    let mut ef = FilterController::new();
    ef.on_message(move |event: &Event, _can_reply: bool| {
        if let Event::File(file_event) = event
            && matches!(&file_event.kind, FileEventKind::ChangeNotification(t) if t.contains(FileEventType::RENAMED))
        {
            *new_path_cb.lock().unwrap() = file_event.new_path.clone();
            fired_cb.store(true, Ordering::SeqCst);
        }
        None::<Box<dyn easefilter::events::Reply>>
    });
    ef.start(common::TEST_LICENSE_KEY, FilterType::MONITOR)
        .unwrap();

    let pattern = format!(r"{}\*", tmp.to_str().unwrap());
    let rule = FileRule {
        file_path: pattern,
        change_event_filter: FileEventType::RENAMED,
        ..FileRule::default()
    };
    let installed = rule.install(&mut ef).unwrap();

    thread::sleep(common::RULE_ACTIVATION_DELAY);

    let src = tmp.join("source.txt");
    let dst = tmp.join("dest.txt");
    fs::write(&src, b"rename me").unwrap();

    // Let CREATED settle, then rename
    std::thread::sleep(Duration::from_millis(300));
    fs::rename(&src, &dst).unwrap();

    assert!(
        wait_until_true(Duration::from_secs(3), || fired.load(Ordering::SeqCst)),
        "RENAMED event did not fire"
    );
    assert!(!wrong.load(Ordering::SeqCst), "unexpected error");

    let got = new_path.lock().unwrap().take();
    assert!(got.is_some(), "new_path should be present on rename");
    assert_eq!(
        got.as_ref().unwrap(),
        &dst,
        "new_path should be {dst:?}, got {:?}",
        got
    );

    installed.uninstall(&mut ef).unwrap();
    ef.stop();
    let _ = fs::remove_dir_all(&tmp);
}

#[test]
#[serial]
fn monitor_event_filtering() {
    require_admin();

    let tmp = temp_dir("ef_test_monitor_filtering");

    let events: Arc<Mutex<Vec<FileEventInfo>>> = Arc::new(Mutex::new(Vec::new()));
    let events_cb = events.clone();

    let mut ef = FilterController::new();
    ef.on_message(move |event: &Event, _can_reply: bool| {
        if let Event::File(file_event) = event {
            events_cb.lock().unwrap().push(FileEventInfo {
                file_name: file_event.file_name.clone(),
                kind: file_event.kind.clone(),
                filter_rule_id: file_event.filter_rule_id,
                pid: file_event.process.pid,
            });
        }
        None::<Box<dyn easefilter::events::Reply>>
    });
    ef.start(common::TEST_LICENSE_KEY, FilterType::MONITOR)
        .unwrap();

    let pattern = format!(r"{}\*", tmp.to_str().unwrap());
    // Only subscribe to CREATED and DELETED
    let rule = FileRule {
        file_path: pattern,
        change_event_filter: FileEventType::CREATED | FileEventType::DELETED,
        ..FileRule::default()
    };
    let installed = rule.install(&mut ef).unwrap();

    thread::sleep(common::RULE_ACTIVATION_DELAY);

    let test_file = tmp.join("filtered.txt");

    // CREATED should fire
    fs::write(&test_file, b"hello").unwrap();
    assert!(
        wait_until_true(Duration::from_secs(3), || {
            events.lock().unwrap().iter().any(|e| {
            matches!(&e.kind, FileEventKind::ChangeNotification(t) if t.contains(FileEventType::CREATED))
                && e.file_name == test_file
            })
        }),
        "CREATED event did not fire"
    );
    events.lock().unwrap().clear();

    // WRITTEN should NOT fire (not subscribed)
    fs::write(&test_file, b"world").unwrap();
    assert!(
        !wait_until_true(Duration::from_millis(200), || {
            events.lock().unwrap().iter().any(|e| {
            matches!(&e.kind, FileEventKind::ChangeNotification(t) if t.contains(FileEventType::WRITTEN))
                && e.file_name == test_file
            })
        }),
        "WRITTEN event fired but was not subscribed"
    );
    events.lock().unwrap().clear();

    // DELETED should fire
    fs::remove_file(&test_file).unwrap();
    assert!(
        wait_until_true(Duration::from_secs(3), || {
            events.lock().unwrap().iter().any(|e| {
            matches!(&e.kind, FileEventKind::ChangeNotification(t) if t.contains(FileEventType::DELETED))
                && e.file_name == test_file
            })
        }),
        "DELETED event did not fire"
    );

    installed.uninstall(&mut ef).unwrap();
    ef.stop();
    let _ = fs::remove_dir_all(&tmp);
}
