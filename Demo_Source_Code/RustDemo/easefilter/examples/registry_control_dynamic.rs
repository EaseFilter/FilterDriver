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

//! Example: dynamic registry access control via callback + `DenyReply`.
//!
//! The userspace callback inspects every pre-operation registry event and decides
//! whether to allow or deny it.
//!
//! Run from an elevated prompt:
//!
//! ```text
//! cargo run --example registry_control_dynamic -- "*\EasefilterRust\*"
//! ```
//!
//! The key mask may be a glob pattern.

use std::env;
use std::fmt::Write;
use std::io;

use easefilter::FilterController;
use easefilter::enums::{FilterType, RegCallbackClass, RegControlFlag};
use easefilter::events::{DenyReply, Event, Reply};
use easefilter::rules::{FilterRule, FilterRuleInstalled, RegistryRule};

fn main() {
    let key_mask = env::args().nth(1).unwrap_or_else(|| {
        eprintln!("Dynamically allows or denies registry operations based on key name.");
        eprintln!("Usage: registry_control_dynamic.exe <key_mask>");
        std::process::exit(1);
    });

    let license_key = env::var("EASEFILTER_TEST_LICENSE_KEY")
        .expect("Set EASEFILTER_TEST_LICENSE_KEY to your license key");

    let mut ef = FilterController::new();

    // Callback dynamically decides which registry operations to reject
    ef.on_message(move |event: &Event, _can_reply: bool| {
        let Event::Registry(ev) = event else {
            return None::<Box<dyn Reply>>;
        };

        let mut msg = String::new();
        writeln!(&mut msg, "  [event]  {:?}", ev.kind).unwrap();
        writeln!(&mut msg, "           key:  {}", ev.key_name).unwrap();
        if let Some(ref new_path) = ev.new_path {
            writeln!(&mut msg, "           new:  {}", new_path).unwrap();
        }
        if let Ok(ref path) = ev.process.path {
            writeln!(
                &mut msg,
                "           pid:  {}  image: {}",
                ev.process.pid, path
            )
            .unwrap();
        }

        let mut reply: Option<Box<dyn Reply>> = None;
        if ev.key_name.contains("deny") {
            writeln!(&mut msg, "    -> operation denied").unwrap();
            reply = Some(Box::new(DenyReply));
        } else {
            writeln!(&mut msg, "    -> operation allowed").unwrap();
        }
        println!("{msg}");
        reply
    });

    // Start the driver in REGISTRY mode
    ef.start(&license_key, FilterType::REGISTRY)
        .expect("failed to start filter driver");

    // Install a rule with all operations allowed at driver level;
    // the callback makes the decision at runtime.
    let installed = RegistryRule {
        key_mask: key_mask.clone(),
        proc_name: "*".into(),
        callback_class: RegCallbackClass::REG_PRE_CREATE_KEY
            | RegCallbackClass::REG_PRE_DELETE_KEY
            | RegCallbackClass::REG_PRE_SET_VALUE_KEY
            | RegCallbackClass::REG_PRE_RENAME_KEY
            | RegCallbackClass::REG_PRE_OPEN_KEY,
        access_flag: RegControlFlag::REG_MAX_ACCESS_FLAG,
        exclude_filter: false,
    }
    .install(&mut ef)
    .expect("failed to install registry rule");

    println!();
    println!("=== REGISTRY CONTROL (dynamic)  monitoring: {key_mask} ===");
    println!("  Operations on keys containing \"deny\" will be blocked by the callback.");
    println!("  All other operations will be allowed.");
    println!();
    println!("  Try creating, deleting or modifying keys under the mask from another window.");
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
