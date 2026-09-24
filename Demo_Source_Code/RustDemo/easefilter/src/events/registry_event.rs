use easefilter_sys as ffi;
use widestring::U16Str;

use crate::enums::RegCallbackClass;
use crate::events::utils::{ProcessData, Sid};

/// A registry event from the filter driver.
///
/// Received when a [`RegistryRule`](crate::rules::RegistryRule) is installed with
/// the appropriate [`RegCallbackClass`] flags.
#[derive(Debug)]
pub struct RegistryEvent {
    /// Which registry event subtype.
    pub kind: RegCallbackClass,

    /// ID of the filter rule that triggered this event.
    pub filter_rule_id: u32,

    /// Sequential message ID.
    pub message_id: u32,

    /// Security Identifier of the user who initiated this event.
    pub sid: Sid,

    /// Information about the process that triggered this event.
    pub process: ProcessData,

    /// Registry key path being operated on.
    pub key_name: String,

    /// New key name for rename operations.
    pub new_path: Option<String>,
}

impl RegistryEvent {
    pub(crate) fn from_message(data: &ffi::MESSAGE_SEND_DATA, kind: RegCallbackClass) -> Self {
        let name_len = (data.FileNameLength as usize / 2).min(data.FileName.len());
        let key_name = if name_len > 0 {
            U16Str::from_slice(&data.FileName[..name_len])
                .to_string_lossy()
                .to_string()
        } else {
            String::new()
        };

        let new_path = if kind == RegCallbackClass::REG_PRE_RENAME_KEY
            || kind == RegCallbackClass::REG_POST_RENAME_KEY
        {
            let byte_count = (data.DataBufferLength as usize).min(data.DataBuffer.len());
            let u16_vec: Vec<u16> = data.DataBuffer[..byte_count]
                .chunks_exact(2)
                .map(|c| u16::from_le_bytes([c[0], c[1]]))
                .collect();
            if u16_vec.is_empty() {
                None
            } else {
                Some(U16Str::from_slice(&u16_vec).to_string_lossy().to_string())
            }
        } else {
            None
        };

        Self {
            kind,
            filter_rule_id: data.FilterRuleId,
            message_id: data.MessageId,
            sid: Sid::from_raw(&data.Sid),
            process: ProcessData::from_message(data),
            key_name,
            new_path,
        }
    }
}
