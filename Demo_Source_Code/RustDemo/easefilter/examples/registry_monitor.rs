//! Example: monitor registry events.
//!
//! Run from an elevated prompt:
//!
//! ```text
//! cargo run --example registry_monitor -- "*\EasefilterRust\*"
//! ```
//!
//! Then in another window use `reg.exe` to add/delete keys under the monitored path.
//!
//! The key mask may be a glob pattern.

use std::env;
use std::fmt::Write;
use std::io;

use easefilter::FilterController;
use easefilter::enums::{FilterType, RegCallbackClass, RegControlFlag};
use easefilter::events::{Event, Reply};
use easefilter::rules::{FilterRule, FilterRuleInstalled, RegistryRule};

fn main() {
    let key_mask = env::args().nth(1).unwrap_or_else(|| {
        eprintln!("Monitors registry events for a given key mask.");
        eprintln!("Usage: registry_monitor.exe <key_mask>");
        std::process::exit(1);
    });

    let license_key = env::var("EASEFILTER_TEST_LICENSE_KEY")
        .expect("Set EASEFILTER_TEST_LICENSE_KEY to your license key");

    let mut ef = FilterController::new();

    ef.on_message(|event: &Event, _can_reply: bool| {
        let Event::Registry(ev) = event else {
            return None::<Box<dyn Reply>>;
        };

        let mut msg = String::new();
        writeln!(&mut msg, "[{:?}]", ev.kind).unwrap();
        writeln!(&mut msg, "  key name:   {}", ev.key_name).unwrap();
        if let Some(ref new_path) = ev.new_path {
            writeln!(&mut msg, "  new path:   {}", new_path).unwrap();
        }
        writeln!(&mut msg, "  process:    pid={}", ev.process.pid).unwrap();
        if let Ok(ref p) = ev.process.path {
            writeln!(&mut msg, "  image:      {}", p).unwrap();
        }
        if let Ok(ref sid_str) = ev.sid.to_string() {
            writeln!(&mut msg, "  user:       {}", sid_str).unwrap();
        }
        writeln!(&mut msg, "  rule id:    {}", ev.filter_rule_id).unwrap();
        writeln!(&mut msg, "  message id: {}", ev.message_id).unwrap();
        println!("{msg}");

        None::<Box<dyn Reply>>
    });

    ef.start(&license_key, FilterType::REGISTRY)
        .expect("failed to start filter driver");

    let installed = RegistryRule {
        key_mask,
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
    println!("=== REGISTRY MONITOR active ===");
    println!("  Perform registry operations from another window.");
    println!("  Press ENTER to stop and uninstall the rule");
    println!();

    println!("Monitor active. Press ENTER to stop...");
    let mut input = String::new();
    io::stdin().read_line(&mut input).ok();

    installed
        .uninstall(&mut ef)
        .expect("failed to uninstall registry rule");
    ef.stop();
    println!("Rule uninstalled.");
}
