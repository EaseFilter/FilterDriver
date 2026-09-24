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

//! Example: static registry access control via `RegControlFlag`.
//!
//! Run from an elevated prompt:
//!
//! ```text
//! cargo run --example registry_control_static -- "*\EasefilterRust\*"
//! ```
//!
//! Registry keys matching the mask will be denied creation at the driver level.
//! The key mask may be a glob pattern.

use std::env;
use std::io;

use easefilter::FilterController;
use easefilter::enums::{FilterType, RegCallbackClass, RegControlFlag};
use easefilter::events::{Event, Reply};
use easefilter::rules::{FilterRule, FilterRuleInstalled, RegistryRule};

fn main() {
    let key_mask = env::args().nth(1).unwrap_or_else(|| {
        eprintln!("Statically denies creation of registry keys matching a mask.");
        eprintln!("Usage: registry_control_static.exe <key_mask>");
        std::process::exit(1);
    });

    let license_key = env::var("EASEFILTER_TEST_LICENSE_KEY")
        .expect("Set EASEFILTER_TEST_LICENSE_KEY to your license key");

    let mut ef = FilterController::new();

    ef.on_message(|_: &Event, _: bool| None::<Box<dyn Reply>>);

    // Start the driver in REGISTRY mode
    ef.start(&license_key, FilterType::REGISTRY)
        .expect("failed to start filter driver");

    // Install a rule that denies CREATE_KEY at driver level
    let installed = RegistryRule {
        key_mask: key_mask.clone(),
        proc_name: "*".into(),
        callback_class: RegCallbackClass::REG_PRE_CREATE_KEY,
        access_flag: RegControlFlag::REG_MAX_ACCESS_FLAG & !RegControlFlag::REG_ALLOW_CREATE_KEY,
        exclude_filter: false,
    }
    .install(&mut ef)
    .expect("failed to install registry rule");

    println!();
    println!("=== REGISTRY CONTROL (static)  protecting: {key_mask} ===");
    println!("  create key:  will be denied by the driver");
    println!();
    println!("  Try creating keys under the mask from another window.");
    println!("  Press ENTER to stop and uninstall the rule");
    println!();

    println!("Control active. Press ENTER to stop...");
    let mut input = String::new();
    io::stdin().read_line(&mut input).ok();

    installed
        .uninstall(&mut ef)
        .expect("failed to uninstall registry rule");
    ef.stop();
    println!("Rule uninstalled.");
}
