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

//! Integration tests for the `driver` module.
//!
//! All tests modify system state (installing/uninstalling a kernel driver)
//! and require Administrator privileges. Run from an elevated prompt:
//!
//!     set PATH=C:\path\to\workspace;%PATH%
//!     cargo test --test driver_integration

mod common;

use common::require_admin;
use std::process::Command;

use easefilter::filter_api;
use serial_test::serial;

const SERVICE_NAME: &str = "EaseFilter";
const SC: &str = "C:\\Windows\\System32\\sc";
const FLTMC: &str = "C:\\Windows\\System32\\fltmc";

/// Ground truth check using `sc query` and `fltmc`.
fn check_installed() -> bool {
    let sc_running = Command::new(SC)
        .args(["query", SERVICE_NAME])
        .output()
        .is_ok_and(|o| {
            let out = String::from_utf8_lossy(&o.stdout);
            o.status.success() && out.contains("RUNNING")
        });

    if !sc_running {
        return false;
    }

    Command::new(FLTMC).output().is_ok_and(|o| {
        String::from_utf8_lossy(&o.stdout)
            .split_whitespace()
            .any(|w| w == SERVICE_NAME)
    })
}

/// Driver install/uninstall test
#[test]
#[serial]
fn lifecycle() {
    require_admin();

    assert_eq!(
        filter_api::is_driver_running(),
        check_installed(),
        "is_driver_running() disagrees with sc query / fltmc"
    );

    if filter_api::is_driver_running() {
        filter_api::uninstall_driver().ok();
    }

    filter_api::install_driver().expect("install should succeed");
    assert!(check_installed());
    assert!(filter_api::is_driver_running());
    assert_eq!(filter_api::is_driver_running(), check_installed());

    filter_api::install_driver().expect("second install should succeed (idempotent)");
    assert!(check_installed());
    assert_eq!(filter_api::is_driver_running(), check_installed());

    filter_api::uninstall_driver().expect("uninstall should succeed");
    assert!(!check_installed());
    assert!(!filter_api::is_driver_running());
    assert_eq!(filter_api::is_driver_running(), check_installed());
}
