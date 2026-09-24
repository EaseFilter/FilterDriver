use std::path::PathBuf;

use easefilter_sys as ffi;
use widestring::U16Str;

use crate::enums::{AccessFlag, EncryptEventType, FileStatus};
use crate::events::Reply;
use crate::events::utils::{ProcessData, Sid};

pub const AES_MAX_TAG_DATA_SIZE: usize = 914;

#[derive(Debug)]
pub struct EncryptEvent {
    /// Which encryption event subtype (RequestKey, RequestIvAndKey, etc.).
    pub kind: EncryptEventType,

    /// ID of the filter rule that triggered this event.
    pub filter_rule_id: u32,

    /// Sequential message ID.
    pub message_id: u32,

    /// Security Identifier of the user who initiated this event.
    pub sid: Sid,

    /// Full path of the file being encrypted or decrypted.
    pub file_name: PathBuf,

    /// Information about the process that initiated this event.
    pub process: ProcessData,

    /// I/O operation status from the driver.
    pub io_status: FileStatus,

    /// Optional tag data attached to the file (present on RequestIvAndKey events).
    pub tag_data: Option<Vec<u8>>,
}

impl EncryptEvent {
    pub(crate) fn from_message(data: &ffi::MESSAGE_SEND_DATA, kind: EncryptEventType) -> Self {
        let char_count = (data.FileNameLength as usize) / 2;
        let file_name = if char_count > 0 && char_count <= data.FileName.len() {
            PathBuf::from(U16Str::from_slice(&data.FileName[..char_count]).to_string_lossy())
        } else {
            PathBuf::new()
        };

        let tag_data = match kind {
            EncryptEventType::RequestIvAndKey => {
                let len = data.DataBufferLength as usize;
                if len > 0 {
                    Some(data.DataBuffer[..len.min(data.DataBuffer.len())].to_vec())
                } else {
                    None
                }
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
            io_status: FileStatus::from_repr(data.Status).unwrap_or(FileStatus::Error),
            tag_data,
        }
    }
}

/// Reply to a request for encryption key, IV, and optional tag data.
///
/// To deny a decryption request, use [`DenyReply`] instead.
///
/// [`DenyReply`]: super::DenyReply
pub struct EncryptReply {
    /// Encryption key (must be 16, 24, or 32 bytes).
    pub encryption_key: Vec<u8>,

    /// Specific 16-byte initialization vector.
    ///
    /// If `None`, the driver auto-generates a unique IV per file.
    pub iv: Option<[u8; 16]>,

    /// Optional metadata to attach to the file header.
    ///
    /// Only meaningful for [`EncryptEventType::RequestIvAndKeyAndTagData`] events.
    pub tag_data: Option<Vec<u8>>,
}

impl EncryptReply {
    fn validate(&self) -> Result<(), &'static str> {
        let ln = self.encryption_key.len();
        if ln != 16 && ln != 24 && ln != 32 {
            return Err("encryption key must be 16, 24, or 32 bytes");
        }
        if let Some(iv) = &self.iv
            && iv.len() != 16
        {
            return Err("IV must be 16 bytes");
        }
        if let Some(tag) = &self.tag_data
            && tag.len() > AES_MAX_TAG_DATA_SIZE
        {
            return Err("tag data exceeds AES_MAX_TAG_DATA_SIZE");
        }
        Ok(())
    }
}

impl Reply for EncryptReply {
    fn apply(self: Box<Self>, reply: &mut ffi::MESSAGE_REPLY_DATA) {
        if self.validate().is_err() {
            reply.ReturnStatus = FileStatus::Error as u32;
            return;
        }

        // SAFETY: Accessing the AESData union member is safe because we are
        // writing to it, not reading uninitialized data.
        unsafe {
            let aes = &mut reply.ReplyData.AESData;
            let data = &mut aes.Data;

            let key_len = self.encryption_key.len() as u32;
            data.EncryptionKeyLength = key_len;

            std::ptr::copy_nonoverlapping(
                self.encryption_key.as_ptr(),
                data.EncryptionKey.as_mut_ptr(),
                key_len as usize,
            );

            if let Some(iv) = self.iv {
                data.IVLength = 16;
                std::ptr::copy_nonoverlapping(iv.as_ptr(), data.IV.as_mut_ptr(), 16);
            } else {
                data.IVLength = 0;
            }

            let base_size = std::mem::size_of_val(data);
            let total_size = if let Some(ref tag) = self.tag_data {
                let tag_len = tag.len() as u32;
                data.TagDataLength = tag_len;
                std::ptr::copy_nonoverlapping(
                    tag.as_ptr(),
                    data.TagData.as_mut_ptr(),
                    tag_len as usize,
                );
                base_size + tag.len()
            } else {
                data.TagDataLength = 0;
                base_size
            };

            aes.SizeOfData = total_size as u32;
            data.AccessFlag = AccessFlag::ALLOW_MAX_RIGHT_ACCESS.bits();
        }

        reply.ReturnStatus = FileStatus::Success as u32;
    }
}
