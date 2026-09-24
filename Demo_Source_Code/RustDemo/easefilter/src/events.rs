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

pub mod encrypt_event;
pub mod file_event;
pub mod process_event;
pub mod registry_event;
pub mod reply;
pub mod utils;

pub use encrypt_event::{AES_MAX_TAG_DATA_SIZE, EncryptEvent, EncryptReply};
pub use file_event::{FileEvent, FileEventKind, FileTimes};
pub use process_event::ProcessEvent;
pub use registry_event::RegistryEvent;
pub use reply::{DenyReply, Reply};
pub use utils::{AccountData, ProcessData, Sid};

/// An event from the filter driver parsed by this library.
#[derive(Debug)]
pub enum Event {
    File(FileEvent),
    Encrypt(EncryptEvent),
    Process(ProcessEvent),
    Registry(RegistryEvent),
}
