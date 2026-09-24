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

//! Example: transparent encryption with a callback providing the key dynamically.
//!
//! No static key is embedded in the rule.  Every time the driver needs to
//! encrypt or decrypt a file, it fires an event to the userspace callback,
//! which responds with the encryption key (and optionally an IV and tag data).
//!
//! Run from an elevated prompt:
//!
//! ```text
//! cargo run --example encrypt_callback -- "C:\path\to\protect"
//! ```

use std::env;
use std::fmt::Write;
use std::io;
use std::path::Path;

use easefilter::FilterController;
use easefilter::enums::{AccessFlag, BooleanConfig, EncryptEventType, FilterType};
use easefilter::events::{EncryptReply, Event, Reply};
use easefilter::rules::{EncryptRule, FilterRule, FilterRuleInstalled};

fn main() {
    let dir = env::args().nth(1).unwrap_or_else(|| {
        eprintln!("Usage: encrypt_callback.exe <directory>");
        std::process::exit(1);
    });

    let protect = Path::new(&dir);
    if !protect.is_dir() {
        eprintln!("error: not a directory: {dir}");
        std::process::exit(1);
    }

    let license_key = env::var("EASEFILTER_TEST_LICENSE_KEY")
        .expect("Set EASEFILTER_TEST_LICENSE_KEY to your license key");

    let enc_key: Vec<u8> = b"0123456789abcdef0123456789abcdef".to_vec();

    let mut ef = FilterController::new();

    ef.on_message(move |event: &Event, _can_reply: bool| {
        let Event::Encrypt(enc) = event else {
            return None::<Box<dyn Reply>>;
        };
        let file = enc.file_name.display();
        let mut msg = String::new();
        match enc.kind {
            EncryptEventType::RequestKey => {
                writeln!(&mut msg, "[encrypt]  key-request        {file}").unwrap();
            }
            EncryptEventType::RequestIvAndKey => {
                writeln!(&mut msg, "[encrypt]  iv+key-request     {file}").unwrap();
            }
            EncryptEventType::RequestIvAndKeyAndTagData => {
                writeln!(&mut msg, "[encrypt]  iv+key+tag-req     {file}").unwrap();
            }
            EncryptEventType::RequestIvAndKeyAndAccessFlag => {
                writeln!(&mut msg, "[encrypt]  iv+key+flags-req   {file}").unwrap();
            }
        }
        if let Ok(sid_str) = enc.sid.to_string() {
            writeln!(&mut msg, "            user SID: {sid_str}").unwrap();
        }
        match &enc.process.path {
            Ok(path) => writeln!(
                &mut msg,
                "            process: {path} (pid={})",
                enc.process.pid
            )
            .unwrap(),
            Err(_) => writeln!(&mut msg, "            process: (pid={})", enc.process.pid).unwrap(),
        }
        if let Some(ref tag) = enc.tag_data {
            let preview = if tag.len() > 32 {
                format!("{}...", String::from_utf8_lossy(&tag[..32]))
            } else {
                String::from_utf8_lossy(tag).to_string()
            };
            writeln!(&mut msg, "            tag data: {preview}").unwrap();
        }
        writeln!(&mut msg, "            status: {:?}", enc.io_status).unwrap();
        println!("{msg}");

        Some(Box::new(EncryptReply {
            encryption_key: enc_key.clone(),
            iv: None,
            tag_data: None,
        }))
    });

    ef.start(&license_key, FilterType::ENCRYPTION)
        .expect("failed to start filter driver");

    let pattern = format!(r"{}\*", protect.to_str().unwrap());
    let installed = EncryptRule {
        file_path: pattern,
        encryption_key: None,
        access_flag: AccessFlag::ALLOW_MAX_RIGHT_ACCESS | AccessFlag::ENABLE_FILE_ENCRYPTION_RULE,
        boolean_config: BooleanConfig::REQUEST_ENCRYPT_KEY_IV_AND_TAGDATA_FROM_SERVICE,
    }
    .install(&mut ef)
    .expect("failed to install encryption rule");

    println!();
    println!("=== TRANSPARENT ENCRYPTION (callback-based key)  protecting: {dir} ===");
    println!("  Each encrypt/decrypt request fires a callback that provides");
    println!("  the AES key.  Files are encrypted at rest.");
    println!("  Press ENTER to stop and uninstall the rule");
    println!();

    println!("Encryption active. Press ENTER to stop...");
    let mut input = String::new();
    io::stdin().read_line(&mut input).ok();

    installed
        .uninstall(&mut ef)
        .expect("failed to uninstall encryption rule");
    ef.stop();
    println!("Encryption rule uninstalled.");
}
