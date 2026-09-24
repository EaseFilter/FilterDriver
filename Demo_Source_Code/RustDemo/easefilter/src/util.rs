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

//! Utilities for dealing with FFI.

use easefilter_sys as ffi;
use ffi::{PULONG, PWCHAR, ULONG};
use widestring::{U16CString, U16Str};

use crate::{EaseFilterErr, InvalidInput};

/// Convert a `&str` to a null-terminated wide string for FFI calls.
pub fn to_wide_string(s: &str) -> Result<U16CString, EaseFilterErr> {
    U16CString::from_str(s).map_err(|_| EaseFilterErr::InvalidInput(InvalidInput::InteriorNullByte))
}

/// Turn an EaseFilter return code into a [`Result`].
///
/// If there is an error, this also retrieves an error message from the driver.
pub fn handle_error(ret_code: i32) -> Result<(), EaseFilterErr> {
    if ret_code == 1 {
        Ok(())
    } else {
        let mut buf_len: ULONG = 0;
        unsafe { ffi::GetLastErrorMessage(std::ptr::null_mut(), &mut buf_len as PULONG) };

        let mut buf: Vec<u16> =
            vec![0u16; usize::try_from(buf_len).expect("ULONG (u32) always fits in usize")];

        unsafe { ffi::GetLastErrorMessage(buf.as_mut_ptr() as PWCHAR, &mut buf_len as PULONG) };

        let s = U16Str::from_slice(&buf).to_string_lossy().to_string();

        Err(EaseFilterErr::Driver(s))
    }
}

#[cfg(test)]
mod tests {
    use super::*;
    use crate::InvalidInput;

    #[test]
    fn to_wide_string_rejects_interior_null() {
        let err = to_wide_string("hello\0world").unwrap_err();
        assert!(matches!(
            err,
            EaseFilterErr::InvalidInput(InvalidInput::InteriorNullByte)
        ));
    }
}
