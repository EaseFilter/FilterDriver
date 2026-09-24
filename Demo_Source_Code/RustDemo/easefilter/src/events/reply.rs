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

use crate::enums::{FileStatus, FilterStatus};

/// A reply to a driver message.
pub trait Reply: Send + Sync {
    /// Write this reply's data into the raw C `MESSAGE_REPLY_DATA` struct.
    fn apply(self: Box<Self>, reply: &mut ffi::MESSAGE_REPLY_DATA);
}

/// Deny access to the requested operation.
pub struct DenyReply;

impl Reply for DenyReply {
    fn apply(self: Box<Self>, reply: &mut ffi::MESSAGE_REPLY_DATA) {
        reply.ReturnStatus = FileStatus::AccessDenied as u32;
        reply.FilterStatus = (FilterStatus::FILTER_MESSAGE_IS_DIRTY
            | FilterStatus::FILTER_COMPLETE_PRE_OPERATION)
            .bits();
    }
}
