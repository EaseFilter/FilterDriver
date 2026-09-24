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

use easefilter_sys as ffi;

use crate::errors::SystemError;

// ---------------------------------------------------------------------------
// Account data
// ---------------------------------------------------------------------------

/// A user account returned by [`Sid::lookup_account`].
#[derive(Clone, Debug, Default)]
pub struct AccountData {
    pub username: String,
    pub domain: String,
}

// ---------------------------------------------------------------------------
// Security Identifier
// ---------------------------------------------------------------------------

/// Raw Windows Security Identifier (SID) bytes from the driver message.
///
/// The SID is stored as a 256-byte array to avoid eagerly calling
/// Win32 API functions. Use [`Sid::to_string`] or [`Sid::lookup_account`]
/// to convert on demand.
#[derive(Clone, Copy)]
pub struct Sid([u8; 256]);

impl Sid {
    pub(crate) fn from_raw(raw: &[u8; 256]) -> Self {
        Self(*raw)
    }

    pub fn as_raw(&self) -> &[u8; 256] {
        &self.0
    }

    /// Convert the SID to its string form (e.g. `S-1-5-...`).
    ///
    /// Calls `ConvertSidToStringSidW` under the hood.
    pub fn to_string(&self) -> Result<String, SystemError> {
        ffi::win32::sid_to_string(&self.0).map_err(SystemError::SidConversion)
    }

    /// Look up the account name and domain for this SID.
    ///
    /// Calls `LookupAccountSidW` under the hood.
    pub fn lookup_account(&self) -> Result<AccountData, SystemError> {
        let (name, domain) =
            ffi::win32::lookup_account_sid(&self.0).map_err(SystemError::AccountLookup)?;
        Ok(AccountData {
            username: name,
            domain,
        })
    }
}

impl std::fmt::Debug for Sid {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        f.debug_struct("Sid").finish_non_exhaustive()
    }
}

// ---------------------------------------------------------------------------
// Process data
// ---------------------------------------------------------------------------

/// Information about a process involved in a filter event.
#[derive(Debug)]
pub struct ProcessData {
    pub pid: u32,
    pub tid: u32,
    /// Full path to the process executable.
    pub path: Result<String, SystemError>,
}

impl ProcessData {
    pub(crate) fn from_message(data: &ffi::MESSAGE_SEND_DATA) -> Self {
        let pid = data.ProcessId;
        let tid = data.ThreadId;
        let path = ffi::win32::get_process_path(pid).map_err(SystemError::ProcessPath);
        Self { pid, tid, path }
    }
}
