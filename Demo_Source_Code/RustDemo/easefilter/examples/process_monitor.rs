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

//! Example: monitor process/thread creation and termination events.
//!
//! Run from an elevated prompt:
//!
//! ```text
//! cargo run --example process_monitor -- "C:\Windows\System32\notepad.exe"
//! ```
//!
//! Then in another window launch the monitored executable.
//!
//! The executable mask may be a glob pattern (e.g. `"C:\\Windows\\*.exe"`).

use std::env;
use std::fmt::Write;
use std::io;

use easefilter::FilterController;
use easefilter::enums::{FilterType, ProcessControlFlag, ProcessEventType};
use easefilter::events::{Event, Reply};
use easefilter::rules::{FilterRule, FilterRuleInstalled, ProcessRule};

fn main() {
    let exec_path = env::args().nth(1).unwrap_or_else(|| {
        eprintln!(
            "Monitors process/thread creation and termination events for a given executable."
        );
        eprintln!("Usage: process_monitor.exe <executable_path>");
        std::process::exit(1);
    });

    let license_key = env::var("EASEFILTER_TEST_LICENSE_KEY")
        .expect("Set EASEFILTER_TEST_LICENSE_KEY to your license key");

    let mut ef = FilterController::new();

    ef.on_message(|event: &Event, _can_reply: bool| {
        let Event::Process(pe) = event else {
            return None::<Box<dyn Reply>>;
        };

        let mut msg = String::new();
        match pe.kind {
            ProcessEventType::ProcessCreationInfo => {
                writeln!(
                    &mut msg,
                    "{:<20} pid={:>6}",
                    "[proc-create]", pe.process.pid
                )
                .unwrap();
                if let Ok(p) = &pe.process.path {
                    writeln!(&mut msg, "{:<20} {p}", "  image:").unwrap();
                }
                if let Some(ref parent) = pe.parent_proc {
                    writeln!(&mut msg, "{:<20} pid={:>6}", "  parent:", parent.pid).unwrap();
                }
                if let Some(ref creating) = pe.creating_proc {
                    writeln!(
                        &mut msg,
                        "{:<20} pid={:>6} tid={:>6}",
                        "  creator:", creating.pid, creating.tid
                    )
                    .unwrap();
                }
                if let Some(ref cmd) = pe.command_line {
                    writeln!(&mut msg, "{:<20} {cmd}", "  cmdline:").unwrap();
                }
            }
            ProcessEventType::ProcessTerminated => {
                writeln!(&mut msg, "{:<20} pid={:>6}", "[proc-term]", pe.process.pid).unwrap();
            }
            ProcessEventType::ThreadCreated => {
                writeln!(
                    &mut msg,
                    "{:<20} pid={:>6} tid={:>6}",
                    "[thread-create]", pe.process.pid, pe.process.tid
                )
                .unwrap();
            }
            ProcessEventType::ThreadTerminated => {
                writeln!(
                    &mut msg,
                    "{:<20} pid={:>6} tid={:>6}",
                    "[thread-term]", pe.process.pid, pe.process.tid
                )
                .unwrap();
            }
            _ => {
                writeln!(
                    &mut msg,
                    "{:<20} pid={:>6}",
                    format!("[{:?}]", pe.kind),
                    pe.process.pid
                )
                .unwrap();
            }
        }
        println!("{msg}");

        None::<Box<dyn Reply>>
    });

    // ------------------------------------------------------------------
    // Start the driver in PROCESS mode
    // ------------------------------------------------------------------
    ef.start(&license_key, FilterType::PROCESS)
        .expect("failed to start filter driver");

    // ------------------------------------------------------------------
    // Install a rule that subscribes to all process/thread notifications
    // ------------------------------------------------------------------
    let installed = ProcessRule {
        executable_mask: exec_path,
        control_flag: ProcessControlFlag::PROCESS_CREATION_NOTIFICATION
            | ProcessControlFlag::PROCESS_TERMINATION_NOTIFICATION
            | ProcessControlFlag::THREAD_CREATION_NOTIFICATION
            | ProcessControlFlag::THREAD_TERMINATION_NOTIFICATION,
    }
    .install(&mut ef)
    .expect("failed to install process rule");

    println!();
    println!("=== PROCESS MONITOR active ===");
    println!("  Start the monitored executable from another window.");
    println!("  Press ENTER to stop and uninstall the rule");
    println!();

    println!("Monitor active. Press ENTER to stop...");
    let mut input = String::new();
    io::stdin().read_line(&mut input).ok();

    installed
        .uninstall(&mut ef)
        .expect("failed to uninstall process rule");
    ef.stop();
    println!("Rule uninstalled.");
}
