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

use std::io;

use thiserror::Error;

use crate::enums::BooleanConfig;

/// Errors that can occur during system-level operations
/// (SID conversion, account lookup, process path lookup).
#[derive(Debug, Error)]
pub enum SystemError {
    #[error("SID conversion failed")]
    SidConversion(#[source] io::Error),
    #[error("account lookup failed")]
    AccountLookup(#[source] io::Error),
    #[error("process path lookup failed")]
    ProcessPath(#[source] io::Error),
}

/// Error returned by EaseFilter API operations.
#[derive(Debug, Error)]
pub enum EaseFilterErr {
    #[error("driver error: {0}")]
    Driver(String),
    #[error(transparent)]
    InvalidInput(#[from] InvalidInput),
    #[error(transparent)]
    System(#[from] SystemError),
}

/// Invalid input provided to an API function.
#[derive(Debug, Error)]
pub enum InvalidInput {
    #[error("string contains interior null byte")]
    InteriorNullByte,
    #[error("encryption key must be 16, 24, or 32 bytes (got {got})")]
    InvalidKeyLength { got: usize },
    #[error(
        "BooleanConfig::REQUEST_ENCRYPT_KEY_IV_AND_TAGDATA_FROM_SERVICE \
             is required when encryption_key is None (got: {got:?})"
    )]
    EncryptionKeyRequired { got: BooleanConfig },
}
