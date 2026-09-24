//! Integration tests for REGISTRY mode.
//!
//! All tests modify system state (installing/uninstalling a kernel driver)
//! and require Administrator privileges. Run from an elevated prompt:
//!
//! ```text
//! set EASEFILTER_TEST_LICENSE_KEY=your-key-here
//! cargo test --test registry_filter
//! ```

mod common;

use std::sync::{
    Arc, Mutex,
    atomic::{AtomicBool, Ordering},
};
use std::time::Duration;

use common::{RULE_ACTIVATION_DELAY, require_admin, temp_dir, wait_until_true};
use easefilter::FilterController;
use easefilter::enums::{FilterType, RegCallbackClass, RegControlFlag};
use easefilter::events::{DenyReply, Event, Reply};
use easefilter::rules::{FilterRule, FilterRuleInstalled, RegistryRule};
use serial_test::serial;

const REG_EXE: &str = "C:\\Windows\\System32\\reg.exe";
const APP_NAME: &str = "EasefilterRust";

fn strip_key_name(full_path: &str) -> String {
    full_path
        .split('\\')
        .next_back()
        .unwrap_or("")
        .trim_end_matches(':')
        .to_string()
}

/// Registry event monitoring: create and delete keys, verify events arrive.
#[test]
#[serial]
fn registry_monitor() {
    require_admin();

    let tmp = temp_dir("ef_test_registry_monitor");
    let dummy_exe = tmp.join("monitor_helper.exe");
    std::fs::copy(REG_EXE, &dummy_exe).unwrap();
    let dummy_str = dummy_exe.to_str().unwrap().to_string();

    // Clean up any leftover keys from previous runs
    let _ = std::process::Command::new(&dummy_str)
        .args(["delete", &format!("HKCU\\Software\\{APP_NAME}"), "/f"])
        .output();

    let test_keys = ["monitor_1", "monitor_2", "monitor_3"];
    let found_keys = Arc::new(Mutex::new(Vec::new()));
    let all_found = Arc::new(AtomicBool::new(false));
    let wrong = Arc::new(AtomicBool::new(false));

    let mut ef = FilterController::new();
    {
        let found_keys = found_keys.clone();
        let all_found = all_found.clone();
        let wrong = wrong.clone();
        ef.on_message(move |event: &Event, _can_reply: bool| {
            let Event::Registry(ev) = event else {
                wrong.store(true, Ordering::SeqCst);
                return None::<Box<dyn Reply>>;
            };
            let key_name = strip_key_name(&ev.key_name);
            found_keys.lock().unwrap().push(key_name.clone());
            if test_keys
                .iter()
                .all(|k| found_keys.lock().unwrap().iter().any(|fk| fk == k))
            {
                all_found.store(true, Ordering::SeqCst);
            }
            None::<Box<dyn Reply>>
        });
    }
    ef.start(common::TEST_LICENSE_KEY, FilterType::REGISTRY)
        .unwrap();

    let rule = RegistryRule {
        key_mask: format!("*\\{APP_NAME}\\*"),
        proc_name: dummy_str.clone(),
        callback_class: RegCallbackClass::REG_PRE_CREATE_KEY | RegCallbackClass::REG_PRE_DELETE_KEY,
        access_flag: RegControlFlag::REG_MAX_ACCESS_FLAG,
        exclude_filter: false,
    };
    let installed = rule.install(&mut ef).unwrap();

    std::thread::sleep(RULE_ACTIVATION_DELAY);

    for k in &test_keys {
        let key_path = format!("HKCU\\Software\\{APP_NAME}\\{k}");
        let add = std::process::Command::new(&dummy_str)
            .args(["add", &key_path, "/f"])
            .output()
            .expect("reg add should succeed");
        assert!(add.status.success(), "reg add failed for {key_path}");

        let del = std::process::Command::new(&dummy_str)
            .args(["delete", &key_path, "/f"])
            .output()
            .expect("reg delete should succeed");
        assert!(del.status.success(), "reg delete failed for {key_path}");
    }

    assert!(
        wait_until_true(Duration::from_secs(5), || all_found.load(Ordering::SeqCst)),
        "expected all test keys in events, found: {:?}",
        found_keys.lock().unwrap()
    );
    assert!(!wrong.load(Ordering::SeqCst), "received non-registry event");

    installed.uninstall(&mut ef).unwrap();
    ef.stop();
    let _ = std::fs::remove_dir_all(&tmp);
}

/// Static registry denial via [RegControlFlag].
#[test]
#[serial]
fn deny_registry_static() {
    require_admin();

    let tmp = temp_dir("ef_test_deny_registry_static");
    let dummy_exe = tmp.join("deny_static.exe");
    std::fs::copy(REG_EXE, &dummy_exe).unwrap();
    let dummy_str = dummy_exe.to_str().unwrap().to_string();

    // Clean up and create the parent key before installing the rule
    let _ = std::process::Command::new(&dummy_str)
        .args(["delete", &format!("HKCU\\Software\\{APP_NAME}"), "/f"])
        .output();
    let parent = format!("HKCU\\Software\\{APP_NAME}");
    std::process::Command::new(&dummy_str)
        .args(["add", &parent, "/f"])
        .output()
        .ok();

    let mut ef = FilterController::new();
    ef.on_message(|_: &Event, _: bool| None::<Box<dyn Reply>>);
    ef.start(common::TEST_LICENSE_KEY, FilterType::REGISTRY)
        .unwrap();

    let rule = RegistryRule {
        key_mask: format!("*\\{APP_NAME}\\*"),
        proc_name: dummy_str.clone(),
        callback_class: RegCallbackClass::REG_PRE_SET_VALUE_KEY,
        access_flag: RegControlFlag::REG_MAX_ACCESS_FLAG
            & !RegControlFlag::REG_ALLOW_SET_VALUE_KEY_INFORMATION,
        exclude_filter: false,
    };
    let installed = rule.install(&mut ef).unwrap();

    std::thread::sleep(RULE_ACTIVATION_DELAY);

    let key_path = format!("HKCU\\Software\\{APP_NAME}\\static_deny_key");
    let result = std::process::Command::new(&dummy_str)
        .args(["add", &key_path, "/f"])
        .output();
    assert!(
        result.is_err() || !result.unwrap().status.success(),
        "reg add should fail when REG_ALLOW_SET_VALUE_KEY_INFORMATION is denied"
    );

    installed.uninstall(&mut ef).unwrap();

    let output = std::process::Command::new(&dummy_str)
        .args(["add", &key_path, "/f"])
        .output()
        .unwrap();
    assert!(
        output.status.success(),
        "reg add should succeed after rule is removed"
    );

    // Cleanup
    let _ = std::process::Command::new(&dummy_str)
        .args(["delete", &key_path, "/f"])
        .output();
    ef.stop();
    let _ = std::fs::remove_dir_all(&tmp);
}

/// Dynamic (callback-based) registry denial.
#[test]
#[serial]
fn deny_registry_dynamic() {
    require_admin();

    let tmp = temp_dir("ef_test_deny_registry_dynamic");
    let dummy_exe = tmp.join("deny_dynamic.exe");
    std::fs::copy(REG_EXE, &dummy_exe).unwrap();
    let dummy_str = dummy_exe.to_str().unwrap().to_string();

    // Clean up any leftover keys from previous runs
    let _ = std::process::Command::new(&dummy_str)
        .args(["delete", &format!("HKCU\\Software\\{APP_NAME}"), "/f"])
        .output();

    // Create the parent key before installing the rule
    let parent = format!("HKCU\\Software\\{APP_NAME}");
    std::process::Command::new(&dummy_str)
        .args(["add", &parent, "/f"])
        .output()
        .ok();

    let deny_list = ["deny_a", "deny_b"];
    let allow_list = ["allow_a", "allow_b"];
    let denied = Arc::new(AtomicBool::new(false));

    let mut ef = FilterController::new();
    {
        let denied = denied.clone();
        ef.on_message(move |event: &Event, _can_reply: bool| {
            let Event::Registry(ev) = event else {
                return None::<Box<dyn Reply>>;
            };
            let key_name = strip_key_name(&ev.key_name);
            if deny_list.contains(&key_name.as_str()) {
                denied.store(true, Ordering::SeqCst);
                return Some(Box::new(DenyReply));
            }
            None::<Box<dyn Reply>>
        });
    }
    ef.start(common::TEST_LICENSE_KEY, FilterType::REGISTRY)
        .unwrap();

    let rule = RegistryRule {
        key_mask: format!("*\\{APP_NAME}\\*"),
        proc_name: dummy_str.clone(),
        callback_class: RegCallbackClass::REG_PRE_CREATE_KEY
            | RegCallbackClass::REG_PRE_OPEN_KEY
            | RegCallbackClass::REG_PRE_SET_VALUE_KEY,
        access_flag: RegControlFlag::REG_MAX_ACCESS_FLAG,
        exclude_filter: false,
    };
    let installed = rule.install(&mut ef).unwrap();

    std::thread::sleep(RULE_ACTIVATION_DELAY);

    // Allowed keys should succeed
    for k in &allow_list {
        let key_path = format!("HKCU\\Software\\{APP_NAME}\\{k}");
        let output = std::process::Command::new(&dummy_str)
            .args(["add", &key_path, "/f"])
            .output()
            .unwrap();
        assert!(output.status.success(), "allowed key {k} should be created");
    }

    // Denied keys should fail
    for k in &deny_list {
        let key_path = format!("HKCU\\Software\\{APP_NAME}\\{k}");
        let output = std::process::Command::new(&dummy_str)
            .args(["add", &key_path, "/f"])
            .output()
            .unwrap();
        assert!(
            !output.status.success(),
            "denied key {k} should not be created"
        );
    }

    assert!(
        wait_until_true(Duration::from_secs(5), || denied.load(Ordering::SeqCst)),
        "DenyReply should have been returned for a denied key"
    );

    installed.uninstall(&mut ef).unwrap();

    // Cleanup all keys
    for k in allow_list.iter().chain(deny_list.iter()) {
        let key_path = format!("HKCU\\Software\\{APP_NAME}\\{k}");
        let _ = std::process::Command::new(&dummy_str)
            .args(["delete", &key_path, "/f"])
            .output();
    }

    ef.stop();
    let _ = std::fs::remove_dir_all(&tmp);
}
