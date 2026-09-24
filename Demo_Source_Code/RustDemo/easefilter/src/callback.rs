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

use std::sync::{Arc, RwLock};

use easefilter_sys as ffi;

use crate::enums::{
    EncryptEventType, FileEventType, FilterCommand, IOCallbackClass, ProcessEventType,
    RegCallbackClass,
};
use crate::events::{
    EncryptEvent, Event, FileEvent, FileEventKind, ProcessEvent, RegistryEvent, Reply,
};

type DisconnectFn = Arc<dyn Fn() + Send + Sync + 'static>;
type EventFn = Arc<dyn Fn(&Event, bool) -> Option<Box<dyn Reply>> + Send + Sync + 'static>;

/// Registered disconnect callback.
pub static DISCONNECT_CB: RwLock<Option<DisconnectFn>> = RwLock::new(None);

/// Registered message callback.
pub static MESSAGE_CB: RwLock<Option<EventFn>> = RwLock::new(None);

pub unsafe extern "C" fn disconnect_trampoline() {
    // clone callback arc and immediately drop lock
    let cb = DISCONNECT_CB
        .read()
        .expect("DISCONNECT_CB lock poisoned")
        .clone();
    if let Some(cb) = cb {
        cb();
    }
}

pub unsafe extern "C" fn message_trampoline(
    send_data: *mut ffi::MESSAGE_SEND_DATA,
    reply_data: *mut ffi::MESSAGE_REPLY_DATA,
) -> i32 {
    let data = unsafe { &*send_data };
    assert_eq!(
        data.VerificationNumber,
        ffi::MESSAGE_SEND_VERIFICATION_NUMBER,
        "MESSAGE_SEND_DATA verification number mismatch"
    );

    let cmd = data.FilterCommand;

    let event = if let Some(proc_type) = ProcessEventType::from_repr(cmd) {
        Event::Process(ProcessEvent::from_message(data, proc_type))
    } else if cmd == FilterCommand::FilterSendRegCallbackInfo as u32 {
        let reg_class = RegCallbackClass::from_bits_truncate(data.MessageType as u64);
        Event::Registry(RegistryEvent::from_message(data, reg_class))
    } else if let Some(encrypt_type) = EncryptEventType::from_repr(cmd) {
        Event::Encrypt(EncryptEvent::from_message(data, encrypt_type))
    } else if cmd == FilterCommand::FilterSendFileChangedEvent as u32 {
        let file_type = FileEventType::from_bits_truncate(data.InfoClass);
        Event::File(FileEvent::from_message(
            data,
            FileEventKind::ChangeNotification(file_type),
        ))
    } else {
        let io_callback_class = IOCallbackClass::from_bits_truncate(data.MessageType as u64);
        Event::File(FileEvent::from_message(
            data,
            FileEventKind::IoCallback(io_callback_class),
        ))
    };

    let can_reply = !reply_data.is_null();

    // clone callback arc and immediately drop lock
    let cb = MESSAGE_CB.read().expect("MESSAGE_CB lock poisoned").clone();

    let reply = cb.and_then(|cb| cb(&event, can_reply));

    if let Some(reply) = reply
        && let Some(raw_reply) = unsafe { reply_data.as_mut() }
    {
        reply.apply(raw_reply);
    }
    1
}
