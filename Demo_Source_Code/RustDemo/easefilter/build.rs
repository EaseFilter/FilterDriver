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

use std::path::PathBuf;
use std::{env, fs};

fn main() {
    // copy dll and sys files next to the binary

    let manifest_dir = PathBuf::from(env::var("CARGO_MANIFEST_DIR").unwrap());
    let bin_dir = manifest_dir
        .join("..")
        .join("..")
        .join("..")
        .join("Bin")
        .join("x64");

    let out_dir = PathBuf::from(env::var("OUT_DIR").unwrap());
    // OUT_DIR = target/debug/build/easefilter-{hash}/out
    // target/ = OUT_DIR/../../../
    // deps/   = target/debug/deps/
    let target_dir = out_dir
        .parent()
        .and_then(|p| p.parent())
        .and_then(|p| p.parent())
        .expect("could not locate target directory from OUT_DIR");

    for subdir in &["", "deps"] {
        for file in &["FilterAPI.dll", "EaseFlt.sys"] {
            let src = bin_dir.join(file);
            let dest = target_dir.join(subdir).join(file);
            if !dest.exists() {
                fs::copy(src.clone(), &dest).unwrap_or_else(|e| {
                    panic!(
                        "failed to copy {} to {}: {e}",
                        src.display(),
                        dest.display()
                    )
                });
            }
        }
    }
}
