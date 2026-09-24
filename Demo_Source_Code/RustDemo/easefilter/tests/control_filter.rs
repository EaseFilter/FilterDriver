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

//! Integration tests for CONTROL mode (static and dynamic access deny).
//!
//! All tests modify system state (installing/uninstalling a kernel driver)
//! and require Administrator privileges. Run from an elevated prompt:
//!
//! ```text
//! set EASEFILTER_TEST_LICENSE_KEY=your-key-here
//! cargo test --test control_filter
//! ```

mod common;

use std::fs;
use std::io;

use common::{RULE_ACTIVATION_DELAY, require_admin, temp_dir};
use easefilter::FilterController;
use easefilter::enums::{AccessFlag, FilterType, IOCallbackClass, InformationClass};
use easefilter::events::{DenyReply, Event, FileEventKind, Reply};
use easefilter::rules::{FileRule, FilterRule, FilterRuleInstalled};
use serial_test::serial;

/// Format a slice of `AccessFlag` values as sorted flag names for readable assertions.
fn fmt_flags(flags: &[AccessFlag]) -> String {
    let mut names: Vec<String> = flags
        .iter()
        .flat_map(|f| f.iter_names().map(|(n, _)| n.to_string()))
        .collect();
    names.sort();
    format!("[{}]", names.join(", "))
}

fn assert_fails_match(actual: &[AccessFlag], expected: &[AccessFlag]) {
    let mut actual_bits: Vec<u32> = actual.iter().map(|f| f.bits()).collect();
    let mut exp_bits: Vec<u32> = expected.iter().map(|f| f.bits()).collect();
    actual_bits.sort();
    exp_bits.sort();
    assert_eq!(
        actual_bits,
        exp_bits,
        "\n  actual (permission denied):   {}\n  expected (permission denied): {}",
        fmt_flags(actual),
        fmt_flags(expected),
    );
}

// =========================================================================
// Static (AccessFlag) permission tests
// =========================================================================

fn run_static_case(deny: AccessFlag, expected: &[AccessFlag]) {
    require_admin();

    let tmp = temp_dir("ef_test_ctrl_static");

    let mut ef = FilterController::new();
    ef.on_message(|_: &Event, _: bool| None::<Box<dyn Reply>>);
    ef.start(common::TEST_LICENSE_KEY, FilterType::CONTROL)
        .unwrap();

    let test_file = tmp.join("MAGIC_FILE_NAME");
    fs::write(&test_file, b"hello").unwrap();

    let pattern = format!(r"{}\*", tmp.to_str().unwrap());
    let access_flag = AccessFlag::ALLOW_MAX_RIGHT_ACCESS & !deny;
    let installed = FileRule {
        file_path: pattern,
        access_flag,
        ..FileRule::default()
    }
    .install(&mut ef)
    .unwrap();

    std::thread::sleep(RULE_ACTIVATION_DELAY);

    let mut fails = Vec::new();

    // --- Try each operation ---

    // Read
    if let Err(e) = fs::read_to_string(&test_file)
        && e.kind() == io::ErrorKind::PermissionDenied
    {
        fails.push(AccessFlag::ALLOW_READ_ACCESS);
    }

    // Write
    if let Err(e) = fs::write(&test_file, b"test")
        && e.kind() == io::ErrorKind::PermissionDenied
    {
        fails.push(AccessFlag::ALLOW_WRITE_ACCESS);
    }

    // Rename
    let file2 = tmp.join("NEW_FILE_NAME");
    if let Err(e) = fs::rename(&test_file, &file2)
        && e.kind() == io::ErrorKind::PermissionDenied
    {
        fails.push(AccessFlag::ALLOW_FILE_RENAME);
    }
    if file2.exists() {
        let _ = fs::rename(&file2, &test_file);
    }

    // Delete
    if let Err(e) = fs::remove_file(&test_file)
        && e.kind() == io::ErrorKind::PermissionDenied
    {
        fails.push(AccessFlag::ALLOW_FILE_DELETE);
    }
    if !test_file.exists() {
        let _ = fs::write(&test_file, b"recreated");
    }

    // Open for read
    if let Err(e) = fs::File::open(&test_file)
        && e.kind() == io::ErrorKind::PermissionDenied
    {
        fails.push(AccessFlag::ALLOW_OPEN_WITH_READ_ACCESS);
    }

    // Open for write
    if let Err(e) = fs::OpenOptions::new().write(true).open(&test_file)
        && e.kind() == io::ErrorKind::PermissionDenied
    {
        fails.push(AccessFlag::ALLOW_OPEN_WITH_WRITE_ACCESS);
    }

    // --- Cleanup ---
    let _ = installed.uninstall(&mut ef);
    ef.stop();
    let _ = fs::remove_dir_all(&tmp);

    assert_fails_match(&fails, expected);
}

#[test]
#[serial]
fn control_static_empty() {
    run_static_case(AccessFlag::empty(), &[]);
}

#[test]
#[serial]
fn control_static_read() {
    run_static_case(
        AccessFlag::ALLOW_READ_ACCESS,
        &[AccessFlag::ALLOW_READ_ACCESS],
    );
}

#[test]
#[serial]
fn control_static_write() {
    run_static_case(
        AccessFlag::ALLOW_WRITE_ACCESS,
        &[AccessFlag::ALLOW_WRITE_ACCESS],
    );
}

#[test]
#[serial]
fn control_static_read_write() {
    run_static_case(
        AccessFlag::ALLOW_READ_ACCESS | AccessFlag::ALLOW_WRITE_ACCESS,
        &[
            AccessFlag::ALLOW_READ_ACCESS,
            AccessFlag::ALLOW_WRITE_ACCESS,
        ],
    );
}

#[test]
#[serial]
fn control_static_rename() {
    run_static_case(
        AccessFlag::ALLOW_FILE_RENAME,
        &[AccessFlag::ALLOW_FILE_RENAME],
    );
}

#[test]
#[serial]
fn control_static_delete() {
    run_static_case(
        AccessFlag::ALLOW_FILE_DELETE,
        &[AccessFlag::ALLOW_FILE_DELETE],
    );
}

// =========================================================================
// Directory listing test
// =========================================================================

#[test]
#[serial]
fn control_dir_listing() {
    require_admin();

    let tmp = temp_dir("ef_test_ctrl_dir");

    let mut ef = FilterController::new();
    ef.on_message(|_: &Event, _: bool| None::<Box<dyn Reply>>);
    ef.start(common::TEST_LICENSE_KEY, FilterType::CONTROL)
        .unwrap();

    let test_file = tmp.join("test.txt");
    let test_dir = tmp.join("subdir");
    fs::create_dir(&test_dir).unwrap();
    let sub_file = tmp.join("subdir/file.txt");
    fs::write(&test_file, b"hello").unwrap();
    fs::write(&sub_file, b"world").unwrap();

    let pattern = format!(r"{}\*", tmp.to_str().unwrap());
    let installed = FileRule {
        file_path: pattern.clone(),
        access_flag: AccessFlag::ALLOW_MAX_RIGHT_ACCESS & !AccessFlag::ALLOW_DIRECTORY_LIST_ACCESS,
        ..FileRule::default()
    }
    .install(&mut ef)
    .unwrap();

    std::thread::sleep(RULE_ACTIVATION_DELAY);

    // Directory listing should be denied
    let listing_err = fs::read_dir(&tmp).err();
    assert!(
        listing_err.is_some()
            && listing_err.as_ref().unwrap().kind() == io::ErrorKind::PermissionDenied,
        "expected PermissionDenied for directory listing, got: {listing_err:?}"
    );

    // But individual file read/write should still work
    assert_eq!(fs::read_to_string(&test_file).unwrap(), "hello");
    assert_eq!(fs::read_to_string(&sub_file).unwrap(), "world");

    fs::write(&test_file, b"updated").unwrap();
    assert_eq!(fs::read_to_string(&test_file).unwrap(), "updated");

    // Cleanup
    let _ = installed.uninstall(&mut ef);
    ef.stop();
    let _ = fs::remove_dir_all(&tmp);
}

// =========================================================================
// Dynamic (callback-based) permission tests
// =========================================================================

fn run_dynamic_case(control_mask: IOCallbackClass, control_all: bool, expected: &[AccessFlag]) {
    require_admin();

    let tmp = temp_dir("ef_test_ctrl_dyn");

    let mut ef = FilterController::new();

    // The callback captures `control_mask` and denies any event whose
    // class matches a flag in the mask (excluding PRE_SET_INFORMATION,
    // which is handled via info_class for rename/delete).
    ef.on_message(move |event: &Event, _can_reply: bool| {
        let Event::File(file_event) = event else {
            return None::<Box<dyn Reply>>;
        };
        if let FileEventKind::IoCallback(class) = &file_event.kind {
            let has_rename = control_mask.contains(IOCallbackClass::PRE_RENAME_FILE);
            let has_delete = control_mask.contains(IOCallbackClass::PRE_DELETE_FILE);

            let deny = control_mask.iter().any(|flag| {
                flag.bits() != IOCallbackClass::PRE_SET_INFORMATION.bits() && class.contains(flag)
            }) || (class.contains(IOCallbackClass::PRE_SET_INFORMATION)
                && file_event.info_class.is_some_and(|ic| {
                    (has_rename
                        && matches!(
                            ic,
                            InformationClass::FileRenameInformation
                                | InformationClass::FileRenameInformationEx
                        ))
                        || (has_delete
                            && matches!(
                                ic,
                                InformationClass::FileDispositionInformation
                                    | InformationClass::FileDispositionInformationEx
                            ))
                }));

            if deny {
                return Some(Box::new(DenyReply));
            }
        }
        None::<Box<dyn Reply>>
    });

    ef.start(common::TEST_LICENSE_KEY, FilterType::CONTROL)
        .unwrap();

    let test_file = tmp.join("MAGIC_FILE_NAME");
    fs::write(&test_file, b"hello").unwrap();

    let pattern = format!(r"{}\*", tmp.to_str().unwrap());
    let io_filter = if control_all {
        IOCallbackClass::all()
    } else {
        control_mask
    };
    let installed = FileRule {
        file_path: pattern,
        access_flag: AccessFlag::ALLOW_MAX_RIGHT_ACCESS,
        control_io_filter: io_filter,
        ..FileRule::default()
    }
    .install(&mut ef)
    .unwrap();

    std::thread::sleep(RULE_ACTIVATION_DELAY);

    let mut fails = Vec::new();

    // Read
    if let Err(e) = fs::read_to_string(&test_file)
        && e.kind() == io::ErrorKind::PermissionDenied
    {
        fails.push(AccessFlag::ALLOW_READ_ACCESS);
    }

    // Write
    if let Err(e) = fs::write(&test_file, b"test")
        && e.kind() == io::ErrorKind::PermissionDenied
    {
        fails.push(AccessFlag::ALLOW_WRITE_ACCESS);
    }

    // Rename
    let file2 = tmp.join("NEW_FILE_NAME");
    if let Err(e) = fs::rename(&test_file, &file2)
        && e.kind() == io::ErrorKind::PermissionDenied
    {
        fails.push(AccessFlag::ALLOW_FILE_RENAME);
    }
    if file2.exists() {
        let _ = fs::rename(&file2, &test_file);
    }

    // Delete
    if let Err(e) = fs::remove_file(&test_file)
        && e.kind() == io::ErrorKind::PermissionDenied
    {
        fails.push(AccessFlag::ALLOW_FILE_DELETE);
    }
    if !test_file.exists() {
        let _ = fs::write(&test_file, b"recreated");
    }

    // Open for read
    if let Err(e) = fs::File::open(&test_file)
        && e.kind() == io::ErrorKind::PermissionDenied
    {
        fails.push(AccessFlag::ALLOW_OPEN_WITH_READ_ACCESS);
    }

    // Open for write
    if let Err(e) = fs::OpenOptions::new().write(true).open(&test_file)
        && e.kind() == io::ErrorKind::PermissionDenied
    {
        fails.push(AccessFlag::ALLOW_OPEN_WITH_WRITE_ACCESS);
    }

    // Create new file
    let new_file = tmp.join("NEW_CREATED_FILE");
    if let Err(e) = fs::OpenOptions::new()
        .create_new(true)
        .write(true)
        .open(&new_file)
        && e.kind() == io::ErrorKind::PermissionDenied
    {
        fails.push(AccessFlag::ALLOW_OPEN_WITH_CREATE_OR_OVERWRITE_ACCESS);
    }

    // Cleanup
    let _ = installed.uninstall(&mut ef);
    ef.stop();
    let _ = fs::remove_dir_all(&tmp);

    assert_fails_match(&fails, expected);
}

#[test]
#[serial]
fn control_dyn_empty() {
    run_dynamic_case(IOCallbackClass::empty(), false, &[]);
}

#[test]
#[serial]
fn control_dyn_read() {
    let mask = IOCallbackClass::PRE_CACHE_READ
        | IOCallbackClass::PRE_NOCACHE_READ
        | IOCallbackClass::PRE_FASTIO_READ
        | IOCallbackClass::PRE_PAGING_IO_READ;
    run_dynamic_case(mask, false, &[AccessFlag::ALLOW_READ_ACCESS]);
}

#[test]
#[serial]
fn control_dyn_write() {
    let mask = IOCallbackClass::PRE_CACHE_WRITE
        | IOCallbackClass::PRE_NOCACHE_WRITE
        | IOCallbackClass::PRE_FASTIO_WRITE
        | IOCallbackClass::PRE_PAGING_IO_WRITE;
    run_dynamic_case(mask, false, &[AccessFlag::ALLOW_WRITE_ACCESS]);
}

#[test]
#[serial]
fn control_dyn_delete() {
    let mask = IOCallbackClass::PRE_DELETE_FILE | IOCallbackClass::PRE_SET_INFORMATION;
    run_dynamic_case(mask, false, &[AccessFlag::ALLOW_FILE_DELETE]);
}

#[test]
#[serial]
fn control_dyn_rename() {
    let mask = IOCallbackClass::PRE_RENAME_FILE | IOCallbackClass::PRE_SET_INFORMATION;
    run_dynamic_case(mask, false, &[AccessFlag::ALLOW_FILE_RENAME]);
}

#[test]
#[serial]
fn control_dyn_read_with_all() {
    let mask = IOCallbackClass::PRE_CACHE_READ
        | IOCallbackClass::PRE_NOCACHE_READ
        | IOCallbackClass::PRE_FASTIO_READ
        | IOCallbackClass::PRE_PAGING_IO_READ;
    // control_all = true: all events flow through, but callback only denies reads
    run_dynamic_case(mask, true, &[AccessFlag::ALLOW_READ_ACCESS]);
}
