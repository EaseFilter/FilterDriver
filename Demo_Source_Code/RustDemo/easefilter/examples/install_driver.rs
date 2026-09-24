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
//  either express or implied.

//! Example for installing and running the EaseFilter system service.

use easefilter::filter_api;

fn main() {
    filter_api::uninstall_driver().unwrap();
    filter_api::install_driver().unwrap();

    if filter_api::is_driver_running() {
        println!("EaseFilter driver service has been installed, and is running.")
    } else {
        panic!("Failed to install EaseFilter driver.")
    }
}
