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

//! Filter rule types.
//!
//! Rules (e.g. [`FileRule`]) are rule configurations, and once installed,
//! they become installed rules (e.g. [`FileRuleInstalled`]).

use crate::EaseFilterErr;
use crate::FilterController;

pub mod encrypt_rule;
pub mod file_rule;
pub mod process_rule;
pub mod registry_rule;

pub use encrypt_rule::*;
pub use file_rule::*;
pub use process_rule::*;
pub use registry_rule::*;

/// A rule that can be installed into the filter driver.
pub trait FilterRule {
    type Installed;
    fn install(self, ef: &mut FilterController) -> Result<Self::Installed, EaseFilterErr>;
}

/// A rule that can be uninstalled from the filter driver.
pub trait FilterRuleInstalled {
    /// Unique rule ID assigned by the driver.
    fn rule_id(&self) -> u32;
    fn uninstall(self, ef: &mut FilterController) -> Result<(), EaseFilterErr>;
}
