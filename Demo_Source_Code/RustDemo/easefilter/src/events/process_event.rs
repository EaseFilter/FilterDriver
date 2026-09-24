use easefilter_sys as ffi;
use widestring::U16Str;

use crate::enums::ProcessEventType;
use crate::errors::SystemError;
use crate::events::utils::{ProcessData, Sid};

/// A process or thread event from the filter driver.
///
/// Received when a [`ProcessRule`](crate::rules::ProcessRule) is installed with
/// the appropriate [`ProcessControlFlag`](crate::enums::ProcessControlFlag) flags.
#[derive(Debug)]
pub struct ProcessEvent {
    /// Which process event subtype (creation, termination, etc.).
    pub kind: ProcessEventType,

    /// ID of the filter rule that triggered this event.
    pub filter_rule_id: u32,

    /// Sequential message ID.
    pub message_id: u32,

    /// Security Identifier of the user who initiated this event.
    pub sid: Sid,

    /// Information about the process or thread this event pertains to.
    pub process: ProcessData,

    /// Information about the parent process (only for creation events).
    pub parent_proc: Option<ProcessData>,

    /// Information about the creating process/thread (only for creation events).
    pub creating_proc: Option<ProcessData>,

    /// Full command line used to start the process (only for creation events).
    pub command_line: Option<String>,
}

impl ProcessEvent {
    pub(crate) fn from_message(data: &ffi::MESSAGE_SEND_DATA, kind: ProcessEventType) -> Self {
        let proc_info = unsafe { &*(data.DataBuffer.as_ptr() as *const ffi::PROCESS_INFO) };

        let (parent_proc, creating_proc, command_line) =
            if kind == ProcessEventType::ProcessCreationInfo {
                let cmd_len = proc_info.CommandLineLength as usize / 2;
                let cmd_slice = &proc_info.CommandLine[..cmd_len.min(proc_info.CommandLine.len())];
                let cmd = if cmd_slice.is_empty() {
                    None
                } else {
                    Some(U16Str::from_slice(cmd_slice).to_string_lossy().to_string())
                };

                let par = ProcessData {
                    pid: proc_info.ParentProcessId,
                    tid: 0,
                    path: ffi::win32::get_process_path(proc_info.ParentProcessId)
                        .map_err(SystemError::ProcessPath),
                };

                let create = ProcessData {
                    pid: proc_info.CreatingProcessId,
                    tid: proc_info.CreatingThreadId,
                    path: ffi::win32::get_process_path(proc_info.CreatingProcessId)
                        .map_err(SystemError::ProcessPath),
                };

                (Some(par), Some(create), cmd)
            } else {
                (None, None, None)
            };

        Self {
            kind,
            filter_rule_id: data.FilterRuleId,
            message_id: data.MessageId,
            sid: Sid::from_raw(&data.Sid),
            process: ProcessData::from_message(data),
            parent_proc,
            creating_proc,
            command_line,
        }
    }
}
