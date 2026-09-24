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

//! Thin safe wrapper over the raw `easefilter-sys` FFI bindings.
//!
//! Functions mirror the C API (`FilterAPI.h`) but accept Rust-native types
//! and return `Result` instead of raw C status codes.
//!
//! This is the Rust equivalent of `easefilter/filter_api.py` in the Python library.

use std::ffi::CString;

use easefilter_sys as ffi;

use ffi::PWCHAR;

use crate::enums::{
    AccessFlag, BooleanConfig, FileEventType, FilterType, IOCallbackClass, ProcessControlFlag,
    RegCallbackClass, RegControlFlag,
};
use crate::util;
use crate::{EaseFilterErr, InvalidInput};

/// Install the EaseFilter driver as a system service.
///
/// Idempotent; does nothing if the driver is already running.
pub fn install_driver() -> Result<(), EaseFilterErr> {
    if is_driver_running() {
        return Ok(());
    }
    unsafe { util::handle_error(ffi::InstallDriver()) }
}

/// Uninstall the EaseFilter driver service.
pub fn uninstall_driver() -> Result<(), EaseFilterErr> {
    unsafe { util::handle_error(ffi::UnInstallDriver()) }
}

/// Check if the EaseFilter driver service is currently running.
pub fn is_driver_running() -> bool {
    unsafe { ffi::IsDriverServiceRunning() == 1 }
}

/// Set the license registration key.
///
/// Must be called before registering message callbacks.
/// The key is a C-style string (narrow, not wide).
pub fn set_registration_key(key: &str) -> Result<(), EaseFilterErr> {
    let c_key = CString::new(key)
        .map_err(|_| EaseFilterErr::InvalidInput(InvalidInput::InteriorNullByte))?;
    // The C signature is `char*` (mutable), but the function does not
    // mutate the buffer, so the cast is safe.
    unsafe { util::handle_error(ffi::SetRegistrationKey(c_key.as_ptr() as *mut i8)) }
}

/// Message callback (`Proto_Message_Callback` from `FilterAPI.h`)
///
/// Called on a driver worker thread when a filesystem event occurs.
/// `send_data` contains the event details. Write a response into `reply_data`.
/// Return 1 on success.
pub type MessageCallback = unsafe extern "C" fn(
    send_data: *mut ffi::MESSAGE_SEND_DATA,
    reply_data: *mut ffi::MESSAGE_REPLY_DATA,
) -> i32;

/// Disconnect callback (`Proto_Disconnect_Callback` from `FilterAPI.h`)
pub type DisconnectCallback = unsafe extern "C" fn();

/// Register message and disconnect callbacks with the filter driver.
///
/// `thread_count` specifies the number of worker threads waiting for callbacks.
/// Pass `None` to omit a callback.
pub fn register_message_callback(
    thread_count: u32,
    message_callback: Option<MessageCallback>,
    disconnect_callback: Option<DisconnectCallback>,
) -> Result<(), EaseFilterErr> {
    unsafe {
        util::handle_error(ffi::RegisterMessageCallback(
            thread_count,
            message_callback,
            disconnect_callback,
        ))
    }
}

/// Disconnect from the filter driver service.
pub fn disconnect() {
    unsafe { ffi::Disconnect() }
}

/// Set the filter type to enable specific filter capabilities.
pub fn set_filter_type(filter_type: FilterType) -> Result<(), EaseFilterErr> {
    unsafe { util::handle_error(ffi::SetFilterType(filter_type.bits())) }
}

/// Add a file filter rule (monitor or control) to the filter driver.
///
/// `access_flag`: access control rights for files matching `filter_mask`.
/// `filter_mask`: a unique file path pattern.
/// `is_resident`:
/// `filter_rule_id`: appears in `messageId` field of callback messages.
pub fn add_file_filter_rule(
    access_flag: AccessFlag,
    filter_mask: &str,
    is_resident: bool,
    filter_rule_id: u32,
) -> Result<(), EaseFilterErr> {
    let mut c_mask = util::to_wide_string(filter_mask)?;
    unsafe {
        util::handle_error(ffi::AddFileFilterRule(
            access_flag.bits(),
            c_mask.as_mut_ptr() as PWCHAR,
            is_resident as i32,
            filter_rule_id,
        ))
    }
}

/// Remove a filter rule from the filter driver by its mask.
pub fn remove_filter_rule(filter_mask: &str) -> Result<(), EaseFilterErr> {
    let mut c_mask = util::to_wide_string(filter_mask)?;
    unsafe { util::handle_error(ffi::RemoveFilterRule(c_mask.as_mut_ptr() as PWCHAR)) }
}

/// Register events for a file filter rule.
pub fn register_file_changed_events(
    filter_mask: &str,
    event_type: FileEventType,
) -> Result<(), EaseFilterErr> {
    let mut c_mask = util::to_wide_string(filter_mask)?;
    unsafe {
        util::handle_error(ffi::RegisterFileChangedEventsToFilterRule(
            c_mask.as_mut_ptr() as PWCHAR,
            event_type.bits(),
        ))
    }
}

/// Register granular events for a file filter rule.
pub fn register_monitor_io(
    filter_mask: &str,
    register_io: IOCallbackClass,
) -> Result<(), EaseFilterErr> {
    let mut c_mask = util::to_wide_string(filter_mask)?;
    unsafe {
        util::handle_error(ffi::RegisterMonitorIOToFilterRule(
            c_mask.as_mut_ptr() as PWCHAR,
            register_io.bits(),
        ))
    }
}

/// Add boolean configuration flags to a specific filter rule.
pub fn add_boolean_config_to_filter_rule(
    filter_mask: &str,
    config: BooleanConfig,
) -> Result<(), EaseFilterErr> {
    let mut c_mask = util::to_wide_string(filter_mask)?;
    unsafe {
        util::handle_error(ffi::AddBooleanConfigToFilterRule(
            c_mask.as_mut_ptr() as PWCHAR,
            config.bits(),
        ))
    }
}

/// Add a static encryption key to a filter rule.
///
/// Each file gets its own auto-generated IV. The encryption information is
/// prepended to the file as a header.
///
/// `encryption_key` must be 16, 24, or 32 bytes.
pub fn add_encryption_key_to_filter_rule(
    filter_mask: &str,
    encryption_key: &[u8],
) -> Result<(), EaseFilterErr> {
    let mut c_mask = util::to_wide_string(filter_mask)?;
    unsafe {
        util::handle_error(ffi::AddEncryptionKeyToFilterRule(
            c_mask.as_mut_ptr() as PWCHAR,
            encryption_key.len() as u32,
            encryption_key.as_ptr() as *mut _,
        ))
    }
}

/// Add a static encryption key and IV to a filter rule.
///
/// All files in this rule use the same encryption key and IV.
pub fn add_encryption_key_and_iv_to_filter_rule(
    filter_mask: &str,
    encryption_key: &[u8],
    iv: &[u8],
) -> Result<(), EaseFilterErr> {
    let mut c_mask = util::to_wide_string(filter_mask)?;
    unsafe {
        util::handle_error(ffi::AddEncryptionKeyAndIVToFilterRule(
            c_mask.as_mut_ptr() as PWCHAR,
            encryption_key.len() as u32,
            encryption_key.as_ptr() as *mut _,
            iv.len() as u32,
            iv.as_ptr() as *mut _,
        ))
    }
}

/// Register granular control I/O events for a file filter rule.
pub fn register_control_io(
    filter_mask: &str,
    register_io: IOCallbackClass,
) -> Result<(), EaseFilterErr> {
    let mut c_mask = util::to_wide_string(filter_mask)?;
    unsafe {
        util::handle_error(ffi::RegisterControlIOToFilterRule(
            c_mask.as_mut_ptr() as PWCHAR,
            register_io.bits(),
        ))
    }
}

/// Add a process filter rule.
///
/// `process_name_mask` is the executable path pattern (e.g. `"C:\\Program Files\\*"`,
/// `"notepad.exe"`).
/// `control_flag` specifies which process/thread events to monitor or deny.
/// `filter_rule_id` appears in the `filter_rule_id` field of callback messages.
pub fn add_process_filter_rule(
    process_name_mask: &str,
    control_flag: ProcessControlFlag,
    filter_rule_id: u32,
) -> Result<(), EaseFilterErr> {
    let mut c_mask = util::to_wide_string(process_name_mask)?;
    let byte_len = c_mask.as_slice().len() * 2;
    unsafe {
        util::handle_error(ffi::AddProcessFilterRule(
            byte_len as u32,
            c_mask.as_mut_ptr() as PWCHAR,
            control_flag.bits(),
            filter_rule_id,
        ))
    }
}

/// Remove a process filter rule by its executable path mask.
pub fn remove_process_filter_rule(process_name_mask: &str) -> Result<(), EaseFilterErr> {
    let mut c_mask = util::to_wide_string(process_name_mask)?;
    let byte_len = c_mask.as_slice().len() * 2;
    unsafe {
        util::handle_error(ffi::RemoveProcessFilterRule(
            byte_len as u32,
            c_mask.as_mut_ptr() as PWCHAR,
        ))
    }
}

// ---------------------------------------------------------------------------
// Registry filter rule API
// ---------------------------------------------------------------------------

/// Add a registry filter rule.
///
/// `key_mask` is the registry key path pattern (e.g. `"*\\Software\\MyApp\\*"`).
/// `process_name` is the executable path pattern to match.
/// `callback_class` selects which registry events to receive.
/// `access_flag` controls which registry operations are allowed.
/// `filter_rule_id` appears in the `filter_rule_id` field of callback messages.
pub fn add_registry_filter_rule(
    key_mask: &str,
    process_name: &str,
    callback_class: RegCallbackClass,
    access_flag: RegControlFlag,
    exclude_filter: bool,
    filter_rule_id: u32,
) -> Result<(), EaseFilterErr> {
    let mut c_key = util::to_wide_string(key_mask)?;
    let mut c_proc = util::to_wide_string(process_name)?;
    let mut c_user = util::to_wide_string("*")?;
    let key_byte_len = c_key.as_slice().len() * 2;
    let proc_byte_len = c_proc.as_slice().len() * 2;
    let user_byte_len = c_user.as_slice().len() * 2;
    unsafe {
        util::handle_error(ffi::AddRegistryFilterRule(
            proc_byte_len as u32,
            c_proc.as_mut_ptr() as PWCHAR,
            0, // processId
            user_byte_len as u32,
            c_user.as_mut_ptr() as PWCHAR,
            key_byte_len as u32,
            c_key.as_mut_ptr() as PWCHAR,
            access_flag.bits(),
            callback_class.bits(),
            exclude_filter as i32,
            filter_rule_id,
        ))
    }
}

/// Add a registry filter rule matching by process name only.
pub fn add_registry_filter_rule_by_process_name(
    process_name: &str,
    callback_class: RegCallbackClass,
    access_flag: RegControlFlag,
    exclude_filter: bool,
) -> Result<(), EaseFilterErr> {
    let mut c_name = util::to_wide_string(process_name)?;
    let byte_len = c_name.as_slice().len() * 2;
    unsafe {
        util::handle_error(ffi::AddRegistryFilterRuleByProcessName(
            byte_len as u32,
            c_name.as_mut_ptr() as PWCHAR,
            access_flag.bits(),
            callback_class.bits(),
            exclude_filter as i32,
        ))
    }
}

/// Add a registry filter rule matching by process ID.
pub fn add_registry_filter_rule_by_process_id(
    process_id: u32,
    callback_class: RegCallbackClass,
    access_flag: RegControlFlag,
    exclude_filter: bool,
) -> Result<(), EaseFilterErr> {
    unsafe {
        util::handle_error(ffi::AddRegistryFilterRuleByProcessId(
            process_id,
            access_flag.bits(),
            callback_class.bits(),
            exclude_filter as i32,
        ))
    }
}

/// Remove a registry filter rule by its associated process name.
pub fn remove_registry_filter_rule_by_process_name(
    process_name: &str,
) -> Result<(), EaseFilterErr> {
    let mut c_name = util::to_wide_string(process_name)?;
    let byte_len = c_name.as_slice().len() * 2;
    unsafe {
        util::handle_error(ffi::RemoveRegistryFilterRuleByProcessName(
            byte_len as u32,
            c_name.as_mut_ptr() as PWCHAR,
        ))
    }
}

/// Remove a registry filter rule by its associated process ID.
pub fn remove_registry_filter_rule_by_process_id(process_id: u32) -> Result<(), EaseFilterErr> {
    unsafe { util::handle_error(ffi::RemoveRegistryFilterRuleByProcessId(process_id)) }
}

#[cfg(test)]
mod tests {
    use super::*;
    use crate::InvalidInput;

    #[test]
    fn set_registration_key_rejects_interior_null() {
        let err = set_registration_key("key\0withnull").unwrap_err();
        assert!(matches!(
            err,
            EaseFilterErr::InvalidInput(InvalidInput::InteriorNullByte)
        ));
    }
}
