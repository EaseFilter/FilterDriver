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
// either express or implied.

//! Low-level FFI to Windows API functions consumed by `easefilter`.
//!
//! All `unsafe` code is contained within the safe public wrappers declared here.

#![allow(non_snake_case, non_camel_case_types)]

use std::io;

use crate::{BOOL, DWORD, HANDLE, HLOCAL, LPWSTR, PSID};

// ---------------------------------------------------------------------------
// advapi32.dll (not in auto-generated bindings)
// ---------------------------------------------------------------------------

#[cfg_attr(target_os = "windows", link(name = "advapi32"))]
extern "system" {
    /// <https://learn.microsoft.com/en-us/windows/win32/api/sddl/nf-sddl-convertsidtostringsidw>
    pub fn ConvertSidToStringSidW(Sid: PSID, StringSid: *mut LPWSTR) -> BOOL;
}

// ---------------------------------------------------------------------------
// Safe wrappers
// ---------------------------------------------------------------------------

/// Wraps `ConvertSidToStringSidW` + `LocalFree`.
///
/// Converts a raw SID byte array into its string representation
/// (e.g. `"S-1-5-21-..."`).
pub fn sid_to_string(sid: &[u8; 256]) -> Result<String, io::Error> {
    let mut ptr: LPWSTR = std::ptr::null_mut();
    // SAFETY: ConvertSidToStringSidW allocates a string via LocalAlloc;
    // LocalFree frees it below.
    let ret = unsafe { ConvertSidToStringSidW(sid.as_ptr() as PSID, &mut ptr) };
    if ret == 0 {
        return Err(io::Error::last_os_error());
    }

    // SAFETY: ptr points to a valid null-terminated wide string allocated by the API.
    let s = unsafe {
        let mut len = 0;
        while *ptr.add(len) != 0 {
            len += 1;
        }
        String::from_utf16_lossy(std::slice::from_raw_parts(ptr, len))
    };

    // SAFETY: ptr was allocated by ConvertSidToStringSidW, free with LocalFree.
    unsafe {
        crate::LocalFree(ptr as HLOCAL);
    }

    Ok(s)
}

/// Wraps `LookupAccountSidW`.
///
/// Returns `(account_name, domain_name)` for the given SID.
pub fn lookup_account_sid(sid: &[u8; 256]) -> Result<(String, String), io::Error> {
    let mut name_len: DWORD = 0;
    let mut domain_len: DWORD = 0;
    let mut sid_type: i32 = 0;

    // First call: determine buffer sizes.
    // SAFETY: Passing null buffers is valid for querying required sizes.
    let ret = unsafe {
        crate::LookupAccountSidW(
            std::ptr::null(),
            sid.as_ptr() as PSID,
            std::ptr::null_mut(),
            &mut name_len,
            std::ptr::null_mut(),
            &mut domain_len,
            &mut sid_type,
        )
    };

    if ret == 0 {
        let err = unsafe { crate::GetLastError() };
        if err != crate::ERROR_INSUFFICIENT_BUFFER {
            return Err(io::Error::from_raw_os_error(err as i32));
        }
    }

    if name_len == 0 {
        name_len = 64;
    }
    if domain_len == 0 {
        domain_len = 64;
    }

    let mut name_buf = vec![0u16; name_len as usize];
    let mut domain_buf = vec![0u16; domain_len as usize];

    // SAFETY: buffers are sized as requested.
    let ret2 = unsafe {
        crate::LookupAccountSidW(
            std::ptr::null(),
            sid.as_ptr() as PSID,
            name_buf.as_mut_ptr(),
            &mut name_len,
            domain_buf.as_mut_ptr(),
            &mut domain_len,
            &mut sid_type,
        )
    };
    if ret2 == 0 {
        return Err(io::Error::last_os_error());
    }

    let name = {
        let end = name_buf
            .iter()
            .position(|&c| c == 0)
            .unwrap_or(name_buf.len());
        String::from_utf16_lossy(&name_buf[..end])
    };
    let domain = {
        let end = domain_buf
            .iter()
            .position(|&c| c == 0)
            .unwrap_or(domain_buf.len());
        String::from_utf16_lossy(&domain_buf[..end])
    };

    Ok((name, domain))
}

/// Wraps `OpenProcess` + `QueryFullProcessImageNameW` + `CloseHandle`.
///
/// Returns the full image path for a process identified by its PID.
pub fn get_process_path(pid: u32) -> Result<String, io::Error> {
    // SAFETY: OpenProcess returns a handle or null.
    let handle: HANDLE = unsafe {
        crate::OpenProcess(
            0x1000, // PROCESS_QUERY_LIMITED_INFORMATION
            0,      // FALSE
            pid,
        )
    };
    if handle.is_null() {
        return Err(io::Error::last_os_error());
    }

    let mut buf = vec![0u16; 260];
    let mut size: DWORD = buf.len() as DWORD;

    // SAFETY: buf is sized for MAX_PATH (260) wide chars.
    let ret = unsafe { crate::QueryFullProcessImageNameW(handle, 0, buf.as_mut_ptr(), &mut size) };

    // SAFETY: CloseHandle is always safe to call with an open handle.
    unsafe {
        crate::CloseHandle(handle);
    }

    if ret == 0 {
        return Err(io::Error::last_os_error());
    }

    let s = String::from_utf16_lossy(&buf[..size as usize]);
    Ok(s)
}
