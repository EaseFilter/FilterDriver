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

//! Example: transparent encryption with a static key derived from a passphrase.
//!
//! A 32-byte AES key is derived via Argon2id from the passphrase.  The driver
//! handles encrypt/decrypt transparently: files on disk stay encrypted, but
//! reads and writes through the filter are plaintext.
//!
//! Run from an elevated prompt:
//!
//! ```text
//! cargo run --example encrypt_static -- "C:\path\to\protect"
//! cargo run --example encrypt_static -- "C:\path\to\protect" "my passphrase"
//! ```

use std::env;
use std::fmt::Write;
use std::io;
use std::path::Path;

use easefilter::FilterController;
use easefilter::enums::{AccessFlag, BooleanConfig, EncryptEventType, FilterType};
use easefilter::events::{Event, Reply};
use easefilter::rules::{EncryptRule, FilterRule, FilterRuleInstalled};

const DEFAULT_PASSPHRASE: &str = "EaseFilter-Example";

fn derive_key(passphrase: &str) -> [u8; 32] {
    use argon2::Argon2;
    let salt = b"EaseFilter-Example-Salt";
    let mut key = [0u8; 32];
    Argon2::default()
        .hash_password_into(passphrase.as_bytes(), salt, &mut key)
        .expect("Argon2 key derivation failed");
    key
}

fn main() {
    let dir = env::args().nth(1).unwrap_or_else(|| {
        eprintln!("Usage: encrypt_static.exe <directory> [passphrase]");
        std::process::exit(1);
    });

    let passphrase = env::args().nth(2).unwrap_or_else(|| {
        eprintln!("info: no passphrase given; using default \"{DEFAULT_PASSPHRASE}\"");
        DEFAULT_PASSPHRASE.to_string()
    });

    let protect = Path::new(&dir);
    if !protect.is_dir() {
        eprintln!("error: not a directory: {dir}");
        std::process::exit(1);
    }

    let license_key = env::var("EASEFILTER_TEST_LICENSE_KEY")
        .expect("Set EASEFILTER_TEST_LICENSE_KEY to your license key");

    let enc_key = derive_key(&passphrase);

    let mut ef = FilterController::new();

    ef.on_message(|event: &Event, _can_reply: bool| {
        let Event::Encrypt(enc) = event else {
            return None::<Box<dyn Reply>>;
        };
        let file = enc.file_name.display();
        let mut msg = String::new();
        match enc.kind {
            EncryptEventType::RequestKey => {
                writeln!(&mut msg, "[encrypt]  key-request       {file}").unwrap();
            }
            EncryptEventType::RequestIvAndKey => {
                writeln!(&mut msg, "[encrypt]  iv+key-request    {file}").unwrap();
            }
            EncryptEventType::RequestIvAndKeyAndTagData => {
                writeln!(&mut msg, "[encrypt]  iv+key+tag-req    {file}").unwrap();
            }
            EncryptEventType::RequestIvAndKeyAndAccessFlag => {
                writeln!(&mut msg, "[encrypt]  iv+key+flags-req  {file}").unwrap();
            }
        }
        if let Ok(sid_str) = enc.sid.to_string() {
            writeln!(&mut msg, "           user SID: {sid_str}").unwrap();
        }
        match &enc.process.path {
            Ok(path) => writeln!(
                &mut msg,
                "           process: {path} (pid={})",
                enc.process.pid
            )
            .unwrap(),
            Err(_) => writeln!(&mut msg, "           process: (pid={})", enc.process.pid).unwrap(),
        }
        if let Some(ref tag) = enc.tag_data {
            let preview = if tag.len() > 32 {
                format!("{}...", String::from_utf8_lossy(&tag[..32]))
            } else {
                String::from_utf8_lossy(tag).to_string()
            };
            writeln!(&mut msg, "           tag data: {preview}").unwrap();
        }
        writeln!(&mut msg, "           status: {:?}", enc.io_status).unwrap();
        println!("{msg}");

        None::<Box<dyn Reply>>
    });

    ef.start(&license_key, FilterType::ENCRYPTION)
        .expect("failed to start filter driver");

    let pattern = format!(r"{}\*", protect.to_str().unwrap());
    let installed = EncryptRule {
        file_path: pattern,
        encryption_key: Some(enc_key.to_vec()),
        access_flag: AccessFlag::ALLOW_MAX_RIGHT_ACCESS | AccessFlag::ENABLE_FILE_ENCRYPTION_RULE,
        boolean_config: BooleanConfig::empty(),
    }
    .install(&mut ef)
    .expect("failed to install encryption rule");

    println!();
    println!("=== TRANSPARENT ENCRYPTION (static key)  protecting: {dir} ===");
    println!("  Files written to this directory are encrypted at rest.");
    println!("  Reads through the filter return plaintext automatically.");
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
