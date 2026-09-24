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

use std::path::PathBuf;

use chrono::{DateTime, Utc};
use easefilter_sys as ffi;
use widestring::U16Str;

use crate::enums::{FileEventType, IOCallbackClass, InformationClass};
use crate::events::utils::{ProcessData, Sid};

// ---------------------------------------------------------------------------
// File timestamps
// ---------------------------------------------------------------------------

/// File timestamps associated with a filter event.
///
/// Each field is `None` when the raw FILETIME value from the driver
/// could not be converted (negative or out-of-range).
#[derive(Clone, Debug)]
pub struct FileTimes {
    pub create: Option<DateTime<Utc>>,
    pub access: Option<DateTime<Utc>>,
    pub write: Option<DateTime<Utc>>,
}

impl FileTimes {
    pub(crate) fn from_message(data: &ffi::MESSAGE_SEND_DATA) -> Self {
        Self {
            create: filetime_to_datetime(data.CreationTime),
            access: filetime_to_datetime(data.LastAccessTime),
            write: filetime_to_datetime(data.LastWriteTime),
        }
    }
}

// ---------------------------------------------------------------------------
// File event kind
// ---------------------------------------------------------------------------

/// Specifics of a [`FileEvent`].
#[derive(Clone, Debug)]
pub enum FileEventKind {
    /// Granular file I/O operations
    IoCallback(IOCallbackClass),
    /// General file operations (created, deleted, renamed, etc.).
    ChangeNotification(FileEventType),
}

// ---------------------------------------------------------------------------
// File event
// ---------------------------------------------------------------------------

/// A file-system event from the filter driver.
#[derive(Debug)]
pub struct FileEvent {
    pub kind: FileEventKind,

    /// ID of the filter rule that triggered this event.
    pub filter_rule_id: u32,

    /// Sequential message ID.
    pub message_id: u32,

    /// Security Identifier of the user who initiated this event.
    pub sid: Sid,

    /// Full path of the file the event pertains to.
    pub file_name: PathBuf,

    /// Information about the process that initiated this event.
    pub process: ProcessData,

    /// File timestamps at the time of the event.
    pub time: FileTimes,

    /// File information class, if the event queries or sets file information.
    ///
    /// Only present for `IoCallback` events that carry a
    /// `SET_INFORMATION` operation.
    pub info_class: Option<InformationClass>,

    /// New path for rename / copy operations.
    ///
    /// Only present for `ChangeNotification` events with
    /// [`FileEventType::RENAMED`] or [`FileEventType::COPIED`].
    pub new_path: Option<PathBuf>,
}

impl FileEvent {
    /// Construct a `FileEvent` from raw driver message data.
    pub(crate) fn from_message(data: &ffi::MESSAGE_SEND_DATA, kind: FileEventKind) -> Self {
        let char_count = (data.FileNameLength as usize) / 2;
        let file_name = if char_count > 0 && char_count <= data.FileName.len() {
            PathBuf::from(U16Str::from_slice(&data.FileName[..char_count]).to_string_lossy())
        } else {
            PathBuf::new()
        };

        let info_class = match &kind {
            FileEventKind::IoCallback(class)
                if class.intersects(
                    IOCallbackClass::PRE_SET_INFORMATION | IOCallbackClass::POST_SET_INFORMATION,
                ) =>
            {
                InformationClass::from_repr(data.InfoClass)
            }
            _ => None,
        };

        let new_path = match &kind {
            FileEventKind::ChangeNotification(file_type)
                if file_type.intersects(FileEventType::RENAMED | FileEventType::COPIED)
                    && data.DataBufferLength > 0 =>
            {
                let byte_count = (data.DataBufferLength as usize).min(data.DataBuffer.len());
                let u16_vec: Vec<u16> = data.DataBuffer[..byte_count]
                    .chunks_exact(2)
                    .map(|c| u16::from_le_bytes([c[0], c[1]]))
                    .collect();
                Some(PathBuf::from(
                    U16Str::from_slice(&u16_vec).to_string_lossy(),
                ))
            }
            _ => None,
        };

        Self {
            kind,
            filter_rule_id: data.FilterRuleId,
            message_id: data.MessageId,
            sid: Sid::from_raw(&data.Sid),
            file_name,
            process: ProcessData::from_message(data),
            time: FileTimes::from_message(data),
            info_class,
            new_path,
        }
    }
}

// ---------------------------------------------------------------------------
// Internal helpers
// ---------------------------------------------------------------------------

/// Convert a Windows FILETIME (100 ns intervals since 1601-01-01 UTC) to a
/// `chrono::DateTime<Utc>`.
///
/// Returns `None` if the FILETIME is negative or the resulting timestamp
/// is outside the range representable by `chrono`.
fn filetime_to_datetime(filetime: i64) -> Option<DateTime<Utc>> {
    if filetime < 0 {
        return None;
    }
    const HUNDRED_NS_PER_SEC: i64 = 10_000_000;
    // Seconds between Windows FILETIME epoch (1601-01-01) and Unix epoch (1970-01-01)
    const FILETIME_UNIX_EPOCH_DELTA: i64 = 11_644_473_600;

    let total_secs = filetime / HUNDRED_NS_PER_SEC - FILETIME_UNIX_EPOCH_DELTA;
    let subsec_nanos = ((filetime % HUNDRED_NS_PER_SEC) * 100) as u32;

    DateTime::from_timestamp(total_secs, subsec_nanos)
}

#[cfg(test)]
mod tests {
    use super::*;

    fn check(ft: i64, expected_secs: i64, expected_nsecs: u32) {
        let dt = filetime_to_datetime(ft).unwrap();
        assert_eq!(dt.timestamp(), expected_secs, "ft={} seconds mismatch", ft);
        assert_eq!(
            dt.timestamp_subsec_nanos(),
            expected_nsecs,
            "ft={} subsec_nanos mismatch",
            ft
        );
    }

    // --- Edge cases ---

    #[test]
    fn negative_filetime_is_none() {
        assert!(filetime_to_datetime(-1).is_none());
    }

    #[test]
    fn filetime_max_is_some() {
        // i64::MAX as FILETIME maps to year ~28869, still within chrono's range.
        assert!(filetime_to_datetime(i64::MAX).is_some());
    }

    // --- Boundary: FILETIME epoch (1601-01-01) ---

    #[test]
    fn ft_0() {
        check(0, -11644473600, 0);
    }

    #[test]
    fn ft_1() {
        check(1, -11644473600, 100);
    }

    #[test]
    fn ft_10() {
        check(10, -11644473600, 1000);
    }

    // --- Checkpoint: Unix epoch ---

    #[test]
    fn unix_epoch() {
        check(116444736000000000, 0, 0);
    }

    #[test]
    fn unix_epoch_plus_100ns() {
        check(116444736000000001, 0, 100);
    }

    #[test]
    fn unix_epoch_plus_1us() {
        check(116444736000000010, 0, 1000);
    }

    #[test]
    fn unix_epoch_plus_1s() {
        check(116444736010000000, 1, 0);
    }

    #[test]
    fn arbitrary_subsec() {
        check(116444736012345678, 1, 234567800);
    }
}
