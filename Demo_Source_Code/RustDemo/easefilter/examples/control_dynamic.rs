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

//! Example: dynamic access control via a callback returning `DenyReply`.
//!
//! The userspace callback inspects every pre-operation I/O event and decides
//! whether to allow or deny it — **every denied operation prints
//! a `[cb-event]` first**, proving the callback was invoked.
//!
//! Run from an elevated prompt:
//!
//! ```text
//! cargo run --example control_dynamic -- "C:\path\to\protect"
//! ```

use std::env;
use std::fmt::Write;
use std::io;
use std::path::Path;

use easefilter::FilterController;
use easefilter::enums::{AccessFlag, FileEventType, FilterType, IOCallbackClass, InformationClass};
use easefilter::events::{DenyReply, Event, FileEventKind, Reply};
use easefilter::rules::{FileRule, FilterRule, FilterRuleInstalled};

fn main() {
    let dir = env::args().nth(1).unwrap_or_else(|| {
        eprintln!("Usage: control_dynamic.exe <directory>");
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

    // Callback dynamically decides which events to reject or accept
    ef.on_message(|event: &Event, _can_reply: bool| {
        let Event::File(file_event) = event else {
            return None::<Box<dyn Reply>>;
        };
        let file = file_event.file_name.display();
        let mut msg = String::new();
        let mut reply: Option<Box<dyn Reply>> = None;

        match &file_event.kind {
            FileEventKind::ChangeNotification(event_type) => {
                writeln!(&mut msg, "  [event]  {event_type:?}      {file}").unwrap();
            }
            FileEventKind::IoCallback(class) => {
                writeln!(&mut msg, "  [event]  {class:?}  {file}").unwrap();

                if class.contains(IOCallbackClass::PRE_SET_INFORMATION) {
                    if let Some(info_class) = &file_event.info_class {
                        match info_class {
                            InformationClass::FileRenameInformation
                            | InformationClass::FileRenameInformationEx => {
                                writeln!(&mut msg, "    -> rename denied").unwrap();
                                reply = Some(Box::new(DenyReply));
                            }
                            InformationClass::FileDispositionInformation
                            | InformationClass::FileDispositionInformationEx => {
                                writeln!(&mut msg, "    -> delete denied").unwrap();
                                reply = Some(Box::new(DenyReply));
                            }
                            _ => {
                                writeln!(&mut msg, "    -> allowed (not rename/delete)").unwrap();
                            }
                        }
                    }
                } else {
                    writeln!(&mut msg, "    -> allowed (not PRE_SET_INFORMATION)").unwrap();
                }
            }
        }
        println!("{msg}");
        reply
    });

    // ------------------------------------------------------------------
    // Start the driver in CONTROL mode
    // ------------------------------------------------------------------
    ef.start(&license_key, FilterType::CONTROL)
        .expect("failed to start filter driver");

    // ------------------------------------------------------------------
    // Install a rule with ALLOW_MAX_RIGHT_ACCESS so nothing is blocked
    // at driver level; the callback decides at runtime.
    // ------------------------------------------------------------------
    let pattern = format!(r"{}\*", protect.to_str().unwrap());
    let installed = FileRule {
        file_path: pattern,
        access_flag: AccessFlag::ALLOW_MAX_RIGHT_ACCESS,
        control_io_filter: IOCallbackClass::PRE_SET_INFORMATION,
        change_event_filter: FileEventType::READ,
        ..FileRule::default()
    }
    .install(&mut ef)
    .expect("failed to install rule");

    println!();
    println!("=== CONTROL (dynamic)  protecting: {dir} ===");
    println!("  rename:  will be denied by callback");
    println!("  delete:  will be denied by callback");
    println!("  read:    will be allowed");
    println!("  write:   will be allowed");
    println!();
    println!("  Try deleting or renaming files inside {dir} manually.");
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
