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

//! Example: static access control via `AccessFlag`.
//!
//! Run from an elevated prompt:
//!
//! ```text
//! cargo run --example control_static -- "C:\path\to\protect"
//! ```

use std::env;
use std::io;
use std::path::Path;

use easefilter::FilterController;
use easefilter::enums::{AccessFlag, FileEventType, FilterType};
use easefilter::events::{Event, FileEventKind, Reply};
use easefilter::rules::{FileRule, FilterRule, FilterRuleInstalled};

fn main() {
    let dir = env::args().nth(1).unwrap_or_else(|| {
        eprintln!("Usage: control_static.exe <directory>");
        std::process::exit(1);
    });

    let protect = Path::new(&dir);
    if !protect.is_dir() {
        eprintln!("error: not a directory: {dir}");
        std::process::exit(1);
    }

    let license_key = env::var("EASEFILTER_TEST_LICENSE_KEY")
        .expect("Set EASEFILTER_TEST_LICENSE_KEY to your license key");

    let mut ef = FilterController::new();

    ef.on_message(|event: &Event, _can_reply: bool| {
        let Event::File(file_event) = event else {
            return None::<Box<dyn Reply>>;
        };
        let file = file_event.file_name.display();
        match &file_event.kind {
            FileEventKind::ChangeNotification(event_type) => {
                println!("  [event]  {event_type:?}   {file}");
            }
            FileEventKind::IoCallback(class) => {
                println!("  [event]  {class:?}   {file}");
            }
        }
        None::<Box<dyn Reply>>
    });

    // ------------------------------------------------------------------
    // Start the driver in CONTROL mode
    // ------------------------------------------------------------------
    ef.start(&license_key, FilterType::CONTROL)
        .expect("failed to start filter driver");

    // ------------------------------------------------------------------
    // Install a rule that denies WRITE and DELETE at driver level
    // ------------------------------------------------------------------
    let pattern = format!(r"{}\*", protect.to_str().unwrap());
    let installed = FileRule {
        file_path: pattern,
        access_flag: AccessFlag::ALLOW_MAX_RIGHT_ACCESS
            & !AccessFlag::ALLOW_WRITE_ACCESS
            & !AccessFlag::ALLOW_FILE_DELETE,
        change_event_filter: FileEventType::READ,
        ..FileRule::default()
    }
    .install(&mut ef)
    .expect("failed to install rule");

    println!();
    println!("=== CONTROL (static)  protecting: {dir} ===");
    println!("  write:  will be denied by driver");
    println!("  delete: will be denied by driver");
    println!("  read:   will be allowed (callback fires for the READ change event)");
    println!();
    println!("  Events denied by the driver won't fire callbacks here by default.");
    println!();
    println!("  Try editing or deleting files inside {dir} manually.");
    println!("  Press ENTER to stop and uninstall the rule");
    println!();

    println!("Control active. Press ENTER to stop...");
    let mut input = String::new();
    io::stdin().read_line(&mut input).ok();

    installed
        .uninstall(&mut ef)
        .expect("failed to uninstall rule");
    ef.stop();
    println!("Rule uninstalled.");
}
