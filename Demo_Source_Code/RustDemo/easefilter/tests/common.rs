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

use std::fs;
use std::path::PathBuf;
use std::time::Duration;

/// Delay after installing a rule so the kernel driver can activate it.
#[allow(dead_code)]
pub(crate) const RULE_ACTIVATION_DELAY: Duration = Duration::from_millis(50);

/// License key for integration tests.
///
/// Set `EASEFILTER_TEST_LICENSE_KEY` at build time:
///
/// ```text
/// $env:EASEFILTER_TEST_LICENSE_KEY="your-key-here"
/// cargo test
/// ```
#[allow(dead_code)]
pub(crate) const TEST_LICENSE_KEY: &str = env!(
    "EASEFILTER_TEST_LICENSE_KEY",
    "set EASEFILTER_TEST_LICENSE_KEY to your EaseFilter license key before `cargo test`"
);

/// Create a clean temporary directory.
///
/// Removes any pre-existing directory at the path, then creates it.
#[allow(dead_code)]
pub(crate) fn temp_dir(name: &str) -> PathBuf {
    let dir = std::env::temp_dir().join(name);
    let _ = fs::remove_dir_all(&dir);
    fs::create_dir_all(&dir).unwrap();
    dir
}

/// Poll every 10ms until `condition()` returns true or `timeout` expires.
///
/// Returns the final value of `condition()`.
#[allow(dead_code)]
pub(crate) fn wait_until_true<F>(timeout: Duration, mut condition: F) -> bool
where
    F: FnMut() -> bool,
{
    let start = std::time::Instant::now();
    while start.elapsed() < timeout {
        if condition() {
            return true;
        }
        std::thread::sleep(Duration::from_millis(10));
    }
    condition()
}

/// Fail fast if not running as Administrator.
pub fn require_admin() {
    let whoami = std::process::Command::new("C:\\Windows\\System32\\whoami")
        .args(["/groups"])
        .output()
        .expect("failed to run whoami");
    let stdout = String::from_utf8_lossy(&whoami.stdout);
    if !stdout.contains("S-1-16-12288") {
        panic!("tests require Administrator privileges - run from an elevated prompt");
    }
}
