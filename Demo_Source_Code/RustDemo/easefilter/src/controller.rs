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

use std::sync::Arc;

use crate::EaseFilterErr;
use crate::callback;
use crate::enums;
use crate::events::{Event, Reply};
use crate::filter_api;

pub struct FilterController {
    started: bool,
    /// Next available rule ID number.
    rule_counter: u32,
}

impl FilterController {
    pub fn new() -> Self {
        Self {
            started: false,
            rule_counter: 0,
        }
    }

    /// Allocate a sequential rule ID.
    pub fn next_rule_id(&mut self) -> u32 {
        let id = self.rule_counter;
        self.rule_counter += 1;
        id
    }

    /// Start the filter driver and register callbacks.
    ///
    /// `license_key` is the registration key provided by EaseFilter.
    /// `filter_type` selects which filter capabilities to enable.
    /// Call `on_disconnect()` and `on_message()` *before* this.
    pub fn start(
        &mut self,
        license_key: &str,
        filter_type: enums::FilterType,
    ) -> Result<(), EaseFilterErr> {
        if self.started {
            return Ok(());
        }
        filter_api::install_driver()?;
        filter_api::set_registration_key(license_key)?;
        filter_api::register_message_callback(
            20,
            Some(callback::message_trampoline as filter_api::MessageCallback),
            Some(callback::disconnect_trampoline as filter_api::DisconnectCallback),
        )?;
        filter_api::set_filter_type(filter_type)?;
        self.started = true;
        Ok(())
    }

    /// Disconnect from the filter driver without dropping the handle.
    ///
    /// Use `stop` normally.
    fn disconnect(&mut self) {
        if !self.started {
            return;
        }
        filter_api::disconnect();
        *callback::MESSAGE_CB
            .write()
            .expect("MESSAGE_CB lock poisoned") = None;
        self.started = false;
    }

    /// Stop the filter driver.
    ///
    /// This consumes the driver handle and prevents further use.
    pub fn stop(mut self) {
        self.disconnect();
    }

    pub fn is_started(&self) -> bool {
        self.started
    }

    /// Set a callback for when the driver disconnects.
    ///
    /// Must be called before `start()`.
    pub fn on_disconnect<F: Fn() + Send + Sync + 'static>(&mut self, cb: F) {
        *callback::DISCONNECT_CB
            .write()
            .expect("DISCONNECT_CB lock poisoned") = Some(Arc::new(cb));
    }

    /// Set a callback for filesystem events from the driver.
    ///
    /// Must be called before `start()`. Callbacks receive events, and can sometimes reply to event
    /// (e.g. to deny them.)
    ///
    /// # Callback parameters
    ///
    /// - `event`: The parsed event from the filter driver.
    /// - `can_reply`: Whether the event can be replied to. When `false`, the driver is a
    ///   monitor-only connection and any [`Reply`] returned by the callback will be silently dropped.
    pub fn on_message<F>(&mut self, cb: F)
    where
        F: Fn(&Event, bool) -> Option<Box<dyn Reply>> + Send + Sync + 'static,
    {
        *callback::MESSAGE_CB
            .write()
            .expect("MESSAGE_CB lock poisoned") = Some(Arc::new(cb));
    }
}

impl Default for FilterController {
    fn default() -> Self {
        Self::new()
    }
}

impl Drop for FilterController {
    fn drop(&mut self) {
        self.disconnect();
    }
}
