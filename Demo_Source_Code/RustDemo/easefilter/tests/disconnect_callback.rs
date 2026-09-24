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

//! Tests that the disconnect callback fires when the filter is stopped.

mod common;

use serial_test::serial;
use std::sync::mpsc;
use std::time::Duration;

use easefilter::FilterController;
use easefilter::enums::FilterType;

use common::require_admin;

#[test]
#[serial]
fn disconnect_callback() {
    require_admin();

    let (tx, rx) = mpsc::channel();

    let mut ef = FilterController::new();
    ef.on_disconnect(move || {
        let _ = tx.send(());
    });
    ef.start(common::TEST_LICENSE_KEY, FilterType::MONITOR)
        .expect("start should succeed");

    assert!(
        rx.recv_timeout(Duration::from_millis(100)).is_err(),
        "disconnect callback called before disconnection"
    );

    ef.stop();

    rx.recv_timeout(Duration::from_secs(5))
        .expect("disconnect callback was not called within 5 seconds");
}
