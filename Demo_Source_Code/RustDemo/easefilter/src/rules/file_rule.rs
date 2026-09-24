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

use super::{FilterRule, FilterRuleInstalled};
use crate::EaseFilterErr;
use crate::FilterController;
use crate::enums::{AccessFlag, BooleanConfig, FileEventType, IOCallbackClass};
use crate::filter_api;

/// File monitor/control rule.
///
/// Configure all fields, then call [`install`](FilterRule::install) to send it
/// to the driver.
///
/// # Examples
///
/// ```
/// # use easefilter::rules::FileRule;
/// # use easefilter::enums::{FileEventType, IOCallbackClass};
/// let rule = FileRule {
///     file_path: r"C:\MonitorDir".into(),
///     change_event_filter: FileEventType::CREATED | FileEventType::WRITTEN,
///     ..FileRule::default()
/// };
/// ```
pub struct FileRule {
    /// File path to monitor (may be a glob, e.g. `"*"`).
    ///
    /// This must be unique, otherwise it overwrites the older rule.
    pub file_path: String,

    /// Allow/disallow flags for file access (Control/encryption mode only).
    pub access_flag: AccessFlag,

    /// Rule-specific configuration flags.
    pub boolean_config: BooleanConfig,

    /// List of file change events to monitor.
    pub change_event_filter: FileEventType,

    pub is_resident: bool,

    /// List of granular I/O events to monitor.
    pub monitor_io_filter: IOCallbackClass,

    /// List of I/O events to send to the control filter.
    ///
    /// Set this to configure a CONTROL-mode rule. For MONITOR-only rules,
    /// leave at the default ([`IOCallbackClass::NONE`]).
    pub control_io_filter: IOCallbackClass,
}

impl Default for FileRule {
    fn default() -> Self {
        Self {
            file_path: String::new(),
            access_flag: AccessFlag::ALLOW_MAX_RIGHT_ACCESS,
            boolean_config: BooleanConfig::empty(),
            change_event_filter: FileEventType::empty(),
            is_resident: false,
            monitor_io_filter: IOCallbackClass::NONE,
            control_io_filter: IOCallbackClass::NONE,
        }
    }
}

impl FilterRule for FileRule {
    type Installed = FileRuleInstalled;

    fn install(self, ef: &mut FilterController) -> Result<Self::Installed, EaseFilterErr> {
        let rule_id = ef.next_rule_id();

        let FileRule {
            file_path,
            access_flag,
            boolean_config,
            change_event_filter,
            is_resident,
            monitor_io_filter,
            control_io_filter,
        } = self;

        filter_api::add_file_filter_rule(access_flag, &file_path, is_resident, rule_id)?;
        filter_api::add_boolean_config_to_filter_rule(&file_path, boolean_config)?;
        filter_api::register_file_changed_events(&file_path, change_event_filter)?;
        filter_api::register_monitor_io(&file_path, monitor_io_filter)?;
        filter_api::register_control_io(&file_path, control_io_filter)?;

        Ok(FileRuleInstalled {
            inner: FileRule {
                file_path,
                access_flag,
                boolean_config,
                change_event_filter,
                is_resident,
                monitor_io_filter,
                control_io_filter,
            },
            rule_id,
        })
    }
}

/// A [`FileRule`] that has been installed into the filter driver.
///
/// The rule is removed from the driver when [`uninstall`](FilterRuleInstalled::uninstall)
/// is called.
pub struct FileRuleInstalled {
    inner: FileRule,
    rule_id: u32,
}

impl FilterRuleInstalled for FileRuleInstalled {
    fn rule_id(&self) -> u32 {
        self.rule_id
    }

    fn uninstall(self, _ef: &mut FilterController) -> Result<(), EaseFilterErr> {
        filter_api::remove_filter_rule(&self.inner.file_path)
    }
}
