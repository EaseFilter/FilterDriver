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

//! Example: dynamic process control via a callback returning `DenyReply`.
//!
//! The userspace callback inspects every process creation event and decides whether to allow or
//! deny it. Every denied event will trigger the callback code.
//!
//! Run from an elevated prompt:
//!
//! ```text
//! cargo run --example process_control_dynamic -- "C:\Windows\System32\cmd.exe" "rmdir"
//! ```
//!
//! Launching `cmd.exe /c "rmdir ..."` from another window will be denied.
//!
//! The executable mask may be a glob pattern (e.g. `"C:\\Windows\\*.exe"`).

use std::env;
use std::fmt::Write;
use std::io;

use easefilter::FilterController;
use easefilter::enums::{FilterType, ProcessControlFlag, ProcessEventType};
use easefilter::events::{DenyReply, Event, Reply};
use easefilter::rules::{FilterRule, FilterRuleInstalled, ProcessRule};

fn main() {
    let mut args = env::args().skip(1);
    let (exec_path, forbidden) = match (args.next(), args.next()) {
        (Some(ep), Some(f)) => (ep, f),
        _ => {
            eprintln!(
                "Dynamically allows or denies process creation based on command-line content."
            );
            eprintln!("Usage: process_control_dynamic.exe <executable_path> <forbidden_keyword>");
            std::process::exit(1);
        }
    };

    let license_key = env::var("EASEFILTER_TEST_LICENSE_KEY")
        .expect("Set EASEFILTER_TEST_LICENSE_KEY to your license key");

    let mut ef = FilterController::new();

    {
        let forbidden = forbidden.clone();
        ef.on_message(move |event: &Event, _can_reply: bool| {
            let Event::Process(pe) = event else {
                return None::<Box<dyn Reply>>;
            };

            let path = match pe.process.path.as_ref() {
                Ok(p) => p.as_str(),
                Err(_) => "?",
            };
            let mut msg = String::new();
            writeln!(
                &mut msg,
                "[cb-event]  {:<24?}  pid={:>6}  {path}",
                pe.kind, pe.process.pid
            )
            .unwrap();

            let mut reply: Option<Box<dyn Reply>> = None;
            if pe.kind == ProcessEventType::ProcessCreationInfo {
                if let Some(ref cmd) = pe.command_line
                    && cmd.contains(&forbidden)
                {
                    writeln!(
                        &mut msg,
                        "    -> process denied (command line contains \"{forbidden}\")"
                    )
                    .unwrap();
                    reply = Some(Box::new(DenyReply));
                } else {
                    writeln!(&mut msg, "    -> process allowed").unwrap();
                }
            }
            println!("{msg}");

            reply
        });
    }

    // ------------------------------------------------------------------
    // Start the driver in PROCESS mode
    // ------------------------------------------------------------------
    ef.start(&license_key, FilterType::PROCESS)
        .expect("failed to start filter driver");

    // ------------------------------------------------------------------
    // Install a rule that sends process creation events to the callback
    // ------------------------------------------------------------------
    let installed = ProcessRule {
        executable_mask: exec_path.clone(),
        control_flag: ProcessControlFlag::PROCESS_CREATION_NOTIFICATION,
    }
    .install(&mut ef)
    .expect("failed to install process rule");

    println!();
    println!("=== PROCESS CONTROL (dynamic)  controlling: {exec_path} ===");
    println!("  Processes with \"{forbidden}\" in the command line will be denied.");
    println!("  Start the monitored executable from another window.");
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
