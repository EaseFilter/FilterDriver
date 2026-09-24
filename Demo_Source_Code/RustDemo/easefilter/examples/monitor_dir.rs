// (C) Copyright 2025 EaseFilter Technologies
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
// either express or implied.

//! Example: monitor a directory for file-system events.
//!
//! Run from an elevated prompt:
//!
//! ```text
//! cargo run --example monitor_dir -- "C:\path\to\watch"
//! ```

use std::env;
use std::fmt::Write;
use std::io;
use std::path::Path;

use easefilter::FilterController;
use easefilter::enums::{FileEventType, FilterType, IOCallbackClass};
use easefilter::events::{Event, FileEventKind, Reply};
use easefilter::rules::{FileRule, FilterRule, FilterRuleInstalled};

fn main() {
    let dir = env::args().nth(1).unwrap_or_else(|| {
        eprintln!("Usage: monitor_dir.exe <directory>");
        std::process::exit(1);
    });

    let watch_path = Path::new(&dir);
    if !watch_path.is_dir() {
        eprintln!("error: not a directory: {dir}");
        std::process::exit(1);
    }

    let license_key = env::var("EASEFILTER_TEST_LICENSE_KEY")
        .expect("Set EASEFILTER_TEST_LICENSE_KEY to your license key");

    // ------------------------------------------------------------------
    // Build the filter controller and register the event callback
    // ------------------------------------------------------------------
    let mut ef = FilterController::new();

    ef.on_message(|event: &Event, _can_reply: bool| {
        if let Event::File(file_event) = event {
            let file = file_event.file_name.display();
            let mut msg = String::new();

            match &file_event.kind {
                FileEventKind::ChangeNotification(event_type) => {
                    if event_type.contains(FileEventType::CREATED) {
                        writeln!(&mut msg, "[created] {file}").unwrap();
                    }
                    if event_type.contains(FileEventType::DELETED) {
                        writeln!(&mut msg, "[deleted] {file}").unwrap();
                    }
                    if event_type.contains(FileEventType::RENAMED) {
                        let new = file_event
                            .new_path
                            .as_ref()
                            .map(|p| p.display().to_string())
                            .unwrap_or_else(|| "?".into());
                        writeln!(&mut msg, "[renamed] {file} \u{2192} {new}").unwrap();
                    }
                    if event_type.contains(FileEventType::WRITTEN) {
                        writeln!(&mut msg, "[written] {file}").unwrap();
                    }
                }
                FileEventKind::IoCallback(class) => {
                    if class.contains(IOCallbackClass::PRE_CREATE) {
                        writeln!(&mut msg, "[pre-create] {file}").unwrap();
                    }
                    if (class.contains(IOCallbackClass::PRE_SET_INFORMATION)
                        || class.contains(IOCallbackClass::POST_SET_INFORMATION))
                        && let Some(info_class) = &file_event.info_class
                    {
                        writeln!(&mut msg, "[set-info] {file}  class={info_class:?}").unwrap();
                    }
                }
            }

            if let Ok(sid_str) = file_event.sid.to_string() {
                writeln!(&mut msg, "  user SID: {sid_str}").unwrap();
            }

            if let Ok(acct) = file_event.sid.lookup_account()
                && !acct.username.is_empty()
            {
                writeln!(&mut msg, "  account:  {}\\{}", acct.domain, acct.username).unwrap();
            }

            match &file_event.process.path {
                Ok(path) => writeln!(
                    &mut msg,
                    "  process:  {path} (pid={})",
                    file_event.process.pid
                )
                .unwrap(),
                Err(_) => {
                    writeln!(&mut msg, "  process:  (pid={})", file_event.process.pid).unwrap()
                }
            }

            println!("{msg}");
        }

        None::<Box<dyn Reply>>
    });

    // ------------------------------------------------------------------
    // Start the driver and install a MONITOR rule for the directory
    // ------------------------------------------------------------------
    ef.start(&license_key, FilterType::MONITOR)
        .expect("failed to start filter driver");

    let pattern = format!(r"{}\*", watch_path.to_str().unwrap());
    let installed = FileRule {
        file_path: pattern,
        change_event_filter: FileEventType::CREATED
            | FileEventType::DELETED
            | FileEventType::RENAMED
            | FileEventType::WRITTEN,
        ..FileRule::default()
    }
    .install(&mut ef)
    .expect("failed to install file rule");

    println!("Monitoring {dir} ----- press ENTER to stop and uninstall the rule");
    println!();

    println!("Monitoring active. Press ENTER to stop...");
    let mut input = String::new();
    io::stdin().read_line(&mut input).ok();

    installed
        .uninstall(&mut ef)
        .expect("failed to uninstall rule");
    ef.stop();
    println!("Rule uninstalled.");
}
