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

//! Integration tests for file rules.
//!
//! All tests modify system state and require Administrator privileges.
//! Run from an elevated prompt:
//!
//!     set EASEFILTER_TEST_LICENSE_KEY=your-key-here
//!     cargo test --test file_rule

mod common;

use std::fs;
use std::sync::{
    Arc, Mutex,
    atomic::{AtomicBool, Ordering},
};
use std::time::Duration;

use common::{require_admin, temp_dir, wait_until_true};
use easefilter::FilterController;
use easefilter::enums::{FileEventType, FilterType};
use easefilter::rules::{FileRule, FilterRule, FilterRuleInstalled};
use serial_test::serial;

#[test]
#[serial]
fn monitor_file_creation() {
    require_admin();

    let tmp = temp_dir("ef_test_file_rule");

    let fired = Arc::new(AtomicBool::new(false));
    let fired_cb = fired.clone();
    let name = Arc::new(Mutex::new(None::<String>));
    let name_cb = name.clone();

    let mut ef = FilterController::new();
    ef.on_message(move |ev: &easefilter::events::Event, _can_reply: bool| {
        let easefilter::events::Event::File(event) = ev else {
            return None::<Box<dyn easefilter::events::Reply>>;
        };
        *name_cb.lock().unwrap() = Some(event.file_name.to_string_lossy().to_string());
        fired_cb.store(true, Ordering::SeqCst);
        None::<Box<dyn easefilter::events::Reply>>
    });
    ef.start(common::TEST_LICENSE_KEY, FilterType::MONITOR)
        .unwrap();

    let rule = FileRule {
        file_path: format!(r"{}\*", tmp.to_str().unwrap()),
        change_event_filter: FileEventType::CREATED,
        ..FileRule::default()
    };
    let installed = rule.install(&mut ef).unwrap();

    std::thread::sleep(common::RULE_ACTIVATION_DELAY);

    assert!(
        !fired.load(Ordering::SeqCst),
        "event fired before file was written"
    );

    let test_file = tmp.join("test.txt");
    fs::write(&test_file, b"hello").unwrap();

    assert!(
        wait_until_true(Duration::from_secs(2), || fired.load(Ordering::SeqCst)),
        "event did not fire after file was written"
    );

    let captured = name.lock().unwrap().take();
    assert!(captured.is_some(), "callback did not set file name");
    assert_eq!(captured.as_ref().unwrap(), test_file.to_str().unwrap(),);

    installed.uninstall(&mut ef).unwrap();
    ef.stop();
    let _ = fs::remove_dir_all(&tmp);
}
