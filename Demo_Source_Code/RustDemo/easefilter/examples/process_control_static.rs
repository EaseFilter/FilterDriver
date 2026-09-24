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

//! Example: static process denial via `DENY_NEW_PROCESS_CREATION`.
//!
//! Run from an elevated prompt:
//!
//! ```text
//! cargo run --example process_control_static -- "C:\Windows\System32\notepad.exe"
//! ```
//!
//! You will not be able to start notepad.
//!
//! The executable mask may be a glob pattern (e.g. `"C:\\Windows\\*.exe"`).

use std::env;
use std::io;

use easefilter::FilterController;
use easefilter::enums::{FilterType, ProcessControlFlag};
use easefilter::events::{Event, Reply};
use easefilter::rules::{FilterRule, FilterRuleInstalled, ProcessRule};

fn main() {
    let exec_path = env::args().nth(1).unwrap_or_else(|| {
        eprintln!("Statically denies creation of a specific executable at the driver level.");
        eprintln!("Usage: process_control_static.exe <executable_path>");
        std::process::exit(1);
    });

    let license_key = env::var("EASEFILTER_TEST_LICENSE_KEY")
        .expect("Set EASEFILTER_TEST_LICENSE_KEY to your license key");

    let mut ef = FilterController::new();

    ef.on_message(|_: &Event, _: bool| None::<Box<dyn Reply>>);

    // ------------------------------------------------------------------
    // Start the driver in PROCESS mode
    // ------------------------------------------------------------------
    ef.start(&license_key, FilterType::PROCESS)
        .expect("failed to start filter driver");

    // ------------------------------------------------------------------
    // Install a rule that blocks the executable at driver level
    // ------------------------------------------------------------------
    let installed = ProcessRule {
        executable_mask: exec_path.clone(),
        control_flag: ProcessControlFlag::DENY_NEW_PROCESS_CREATION,
    }
    .install(&mut ef)
    .expect("failed to install process rule");

    println!();
    println!("=== PROCESS CONTROL (static)  blocking: {exec_path} ===");
    println!("  The executable will be denied by the driver.");
    println!("  Press ENTER to stop and uninstall the rule");
    println!();

    println!("Control active. Press ENTER to stop...");
    let mut input = String::new();
    io::stdin().read_line(&mut input).ok();

    installed
        .uninstall(&mut ef)
        .expect("failed to uninstall process rule");
    ef.stop();
    println!("Rule uninstalled.");
}
