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

use bitflags::bitflags;
use strum::FromRepr;

bitflags! {
    #[derive(Clone, Copy)]
    /// Enable specific Filter capabilities.
    pub struct FilterType: u32 {
        /// File I/O control.
        const CONTROL = 0x01;
        /// Transparent file encryption.
        const ENCRYPTION = 0x02;
        /// File I/O monitoring.
        const MONITOR = 0x04;
        /// Registry I/O monitoring & control.
        const REGISTRY = 0x08;
        /// Process/thread monitor & control.
        const PROCESS = 0x10;
        /// Hierarchical storage management.
        const HSM = 0x40;
        /// Cloud storage.
        const CLOUD = 0x80;
    }
}

bitflags! {
    #[derive(Clone, Copy, Debug)]
    /// Select specific file events.
    pub struct FileEventType: u32 {
        const CREATED = 0x00000020;
        const WRITTEN = 0x00000040;
        const RENAMED = 0x00000080;
        const DELETED = 0x00000100;
        const SECURITY_CHANGED = 0x00000200;
        const INFO_CHANGED = 0x00000400;
        const READ = 0x00000800;
        /// File copy; only available on Windows 11.
        const COPIED = 0x00001000;
    }
}

/// Types of I/O filesystem events.
#[repr(u32)]
#[derive(Debug, Clone, Copy, PartialEq, Eq, Hash)]
pub enum IOName {
    PreFileCreate = 0x00020001,
    PostFileCreate = 0x00020002,
    PreFileRead = 0x00020003,
    PostFileRead = 0x00020004,
    PreFileWrite = 0x00020005,
    PostFileWrite = 0x00020006,
    PreQueryFileSize = 0x00020007,
    PostQueryFileSize = 0x00020008,
    PreQueryFileBasicInfo = 0x00020009,
    PostQueryFileBasicInfo = 0x0002000A,
    PreQueryFileStandardInfo = 0x0002000B,
    PostQueryFileStandardInfo = 0x0002000C,
    PreQueryFileNetworkInfo = 0x0002000D,
    PostQueryFileNetworkInfo = 0x0002000E,
    PreQueryFileId = 0x0002000F,
    PostQueryFileId = 0x00020010,
    PreQueryFileInfo = 0x00020011,
    PostQueryFileInfo = 0x00020012,
    PreSetFileSize = 0x00020013,
    PostSetFileSize = 0x00020014,
    PreSetFileBasicInfo = 0x00020015,
    PostSetFileBasicInfo = 0x00020016,
    PreSetFileStandardInfo = 0x00020017,
    PostSetFileStandardInfo = 0x00020018,
    PreSetFileNetworkInfo = 0x00020019,
    PostSetFileNetworkInfo = 0x0002001A,
    PreMoveOrRenameFile = 0x0002001B,
    PostMoveOrRenameFile = 0x0002001C,
    PreDeleteFile = 0x0002001D,
    PostDeleteFile = 0x0002001E,
    PreSetFileInfo = 0x0002001F,
    PostSetFileInfo = 0x00020020,
    PreQueryDirectoryFile = 0x00020021,
    PostQueryDirectoryFile = 0x00020022,
    PreQueryFileSecurity = 0x00020023,
    PostQueryFileSecurity = 0x00020024,
    PreSetFileSecurity = 0x00020025,
    PostSetFileSecurity = 0x00020026,
    PreFileHandleClose = 0x00020027,
    PostFileHandleClose = 0x00020028,
    PreFileClose = 0x00020029,
    PostFileClose = 0x0002002A,
}

bitflags! {
    #[derive(Clone, Copy, Debug)]
    /// I/O types that can be intercepted via monitor/control filter.
    ///
    /// Monitor can only register events after they occur (post I/O),
    /// while control can register all events (pre and post).
    ///
    /// See [Wikipedia](https://en.m.wikipedia.org/wiki/I/O_request_packet)
    /// for basic information about what an IRP is.
    pub struct IOCallbackClass: u64 {
        const NONE = 0;

        /// `IRP_MJ_CREATE`: open file handle.
        const PRE_CREATE = 0x00000001;
        /// `IRP_MJ_CREATE`: open file handle.
        const POST_CREATE = 0x00000002;

        const PRE_NEW_FILE_CREATED = 0x0000000100000000;
        const POST_NEW_FILE_CREATED = 0x0000000200000000;

        /// Fast I/O read. Returns true if data is cached. If not cached, a new cache read IRP will be generated.
        const PRE_FASTIO_READ = 0x00000004;
        /// Fast I/O read. Returns true if data is cached. If not cached, a new cache read IRP will be generated.
        const POST_FASTIO_READ = 0x00000008;

        /// `IRP_MJ_READ`: read data from cache. If not cached, a paging read request is generated.
        const PRE_CACHE_READ = 0x00000010;
        /// `IRP_MJ_READ`: read data from cache. If not cached, a paging read request is generated.
        const POST_CACHE_READ = 0x00000020;

        /// `IRP_MJ_READ`: read data without cache, bypassing the cache manager.
        const PRE_NOCACHE_READ = 0x00000040;
        /// `IRP_MJ_READ`: read data without cache, bypassing the cache manager.
        const POST_NOCACHE_READ = 0x00000080;

        /// `IRP_MJ_READ`: paging read that caches on-disk data.
        const PRE_PAGING_IO_READ = 0x00000100;
        /// `IRP_MJ_READ`: paging read that caches on-disk data.
        const POST_PAGING_IO_READ = 0x00000200;

        /// Fast I/O write.
        ///
        /// Data written to cache if the request is immediately satisfied, otherwise an IRP cache write will be generated.
        const PRE_FASTIO_WRITE = 0x00000400;
        /// Fast I/O write.
        ///
        /// Data written to cache if the request is immediately satisfied, otherwise an IRP cache write will be generated.
        const POST_FASTIO_WRITE = 0x00000800;

        /// `IRP_MJ_WRITE` cache write.
        ///
        /// A paging write IRP will be generated after this.
        const PRE_CACHE_WRITE = 0x00001000;
        /// `IRP_MJ_WRITE` cache write.
        ///
        /// A paging write IRP will be generated after this.
        const POST_CACHE_WRITE = 0x00002000;

        /// `IRP_MJ_WRITE`: write directly to disk, bypassing cache manager.
        const PRE_NOCACHE_WRITE = 0x00004000;
        /// `IRP_MJ_WRITE`: write directly to disk, bypassing cache manager.
        const POST_NOCACHE_WRITE = 0x00008000;

        /// `IRP_MJ_WRITE`: paging write that moves data from cache to disk.
        const PRE_PAGING_IO_WRITE = 0x00010000;
        /// `IRP_MJ_WRITE`: paging write that moves data from cache to disk.
        const POST_PAGING_IO_WRITE = 0x00020000;

        /// `IRP_QUERY_INFORMATION`: file information query.
        ///
        /// This flag registers all queries; other `PRE_QUERY` flags can watch specific queries, e.g. only file size.
        const PRE_QUERY_INFORMATION = 0x00040000;
        /// `IRP_QUERY_INFORMATION`: file information query.
        ///
        /// This flag registers all queries; other `QUERY` flags can watch specific queries, e.g. only file size.
        const POST_QUERY_INFORMATION = 0x00080000;

        /// `IRP_QUERY_INFORMATION`: file size.
        const PRE_QUERY_FILE_SIZE = 0x0000000400000000;
        /// `IRP_QUERY_INFORMATION`: file size.
        const POST_QUERY_FILE_SIZE = 0x0000000800000000;

        /// `IRP_QUERY_INFORMATION`: basic file information.
        const PRE_QUERY_FILE_BASIC_INFO = 0x0000001000000000;
        /// `IRP_QUERY_INFORMATION`: basic file information.
        const POST_QUERY_FILE_BASIC_INFO = 0x0000002000000000;

        /// `IRP_QUERY_INFORMATION`: standard file information.
        const PRE_QUERY_FILE_STANDARD_INFO = 0x0000004000000000;
        /// `IRP_QUERY_INFORMATION`: standard file information.
        const POST_QUERY_FILE_STANDARD_INFO = 0x0000008000000000;

        /// `IRP_QUERY_INFORMATION`: file network information.
        const PRE_QUERY_FILE_NETWORK_INFO = 0x0000010000000000;
        /// `IRP_QUERY_INFORMATION`: file network information.
        const POST_QUERY_FILE_NETWORK_INFO = 0x0000020000000000;

        /// `IRP_QUERY_INFORMATION`: file ID.
        const PRE_QUERY_FILE_ID = 0x0000040000000000;
        /// `IRP_QUERY_INFORMATION`: file ID.
        const POST_QUERY_FILE_ID = 0x0000080000000000;

        /// `IRP_SET_INFORMATION`: set file information.
        ///
        /// This flag registers all requests; other `SET` flags can watch specific requests, e.g. only file size.
        const PRE_SET_INFORMATION = 0x00100000;
        /// `IRP_SET_INFORMATION`: set file information.
        ///
        /// This flag registers all requests; other `SET` flags can watch specific requests, e.g. only file size.
        const POST_SET_INFORMATION = 0x00200000;

        /// `IRP_SET_INFORMATION`: file size.
        const PRE_SET_FILE_SIZE = 0x0000400000000000;
        /// `IRP_SET_INFORMATION`: file size.
        const POST_SET_FILE_SIZE = 0x0000800000000000;

        /// `IRP_SET_INFORMATION`: file basic information.
        const PRE_SET_FILE_BASIC_INFO = 0x0001000000000000;
        /// `IRP_SET_INFORMATION`: file basic information.
        const POST_SET_FILE_BASIC_INFO = 0x0002000000000000;

        /// `IRP_SET_INFORMATION`: file standard information.
        const PRE_SET_FILE_STANDARD_INFO = 0x0004000000000000;
        /// `IRP_SET_INFORMATION`: file standard information.
        const POST_SET_FILE_STANDARD_INFO = 0x0008000000000000;

        /// `IRP_SET_INFORMATION`: file network information.
        const PRE_SET_FILE_NETWORK_INFO = 0x0010000000000000;
        /// `IRP_SET_INFORMATION`: file network information.
        const POST_SET_FILE_NETWORK_INFO = 0x0020000000000000;

        /// `IRP_SET_INFORMATION`: file rename.
        const PRE_RENAME_FILE = 0x0040000000000000;
        /// `IRP_SET_INFORMATION`: file rename.
        const POST_RENAME_FILE = 0x0080000000000000;

        /// `IRP_SET_INFORMATION`: file delete.
        const PRE_DELETE_FILE = 0x0100000000000000;
        /// `IRP_SET_INFORMATION`: file delete.
        const POST_DELETE_FILE = 0x0200000000000000;

        /// `IRP_MJ_DIRECTORY_CONTROL`: query directory information.
        const PRE_DIRECTORY = 0x00400000;
        /// `IRP_MJ_DIRECTORY_CONTROL`: query directory information.
        const POST_DIRECTORY = 0x00800000;

        /// `IRP_MJ_QUERY_SECURITY`: query file security information.
        const PRE_QUERY_SECURITY = 0x01000000;
        /// `IRP_MJ_QUERY_SECURITY`: query file security information.
        const POST_QUERY_SECURITY = 0x02000000;

        /// `IRP_MJ_SET_SECURITY`: set file security information.
        const PRE_SET_SECURITY = 0x04000000;
        /// `IRP_MJ_SET_SECURITY`: set file security information.
        const POST_SET_SECURITY = 0x08000000;

        /// `IRP_MJ_CLEANUP`: close file handle.
        const PRE_CLEANUP = 0x10000000;
        /// `IRP_MJ_CLEANUP`: close file handle.
        const POST_CLEANUP = 0x20000000;

        /// `IRP_MJ_CLOSE`: close file I/O.
        const PRE_CLOSE = 0x40000000;
        /// `IRP_MJ_CLOSE`: close file I/O.
        const POST_CLOSE = 0x80000000;
    }
}

bitflags! {
    #[derive(Clone, Copy)]
    /// Access control for a file.
    ///
    /// Only for Control mode.
    ///
    /// IMPORTANT: Do not set this to 0 for least access.
    /// See `AccessFlag::LEAST_ACCESS_FLAG`.
    pub struct AccessFlag: u32 {
        /// Filter driver skips all I/O on this file.
        ///
        /// (i.e, no access protection.)
        const EXCLUDE_FILTER_RULE = 0x00000000;
        /// Block file open.
        const EXCLUDE_FILE_ACCESS = 0x00000001;
        /// Allow reparsing the file to another filename through a reparse mask.
        const ENABLE_REPARSE_FILE_OPEN = 0x00000002;
        /// Allow hiding files in a directory through a hide file mask.
        const ENABLE_HIDE_FILES_IN_DIRECTORY_BROWSING = 0x00000004;
        /// Enable transparent file encryption if an encryption key is added.
        const ENABLE_FILE_ENCRYPTION_RULE = 0x00000008;
        /// Allow file open to access the file's security information.
        const ALLOW_OPEN_WITH_ACCESS_SYSTEM_SECURITY = 0x00000010;
        /// Allow file open with read access.
        const ALLOW_OPEN_WITH_READ_ACCESS = 0x00000020;
        /// Allow file open with write access.
        const ALLOW_OPEN_WITH_WRITE_ACCESS = 0x00000040;
        /// Allow file open with create/overwrite access.
        const ALLOW_OPEN_WITH_CREATE_OR_OVERWRITE_ACCESS = 0x00000080;
        /// Allow file open with delete access.
        const ALLOW_OPEN_WITH_DELETE_ACCESS = 0x00000100;
        /// Allow reading data from file.
        const ALLOW_READ_ACCESS = 0x00000200;
        /// Allow writing data from file.
        const ALLOW_WRITE_ACCESS = 0x00000400;
        /// Allow querying the file's information.
        const ALLOW_QUERY_INFORMATION_ACCESS = 0x00000800;
        /// Allow changing the file's information:
        ///
        /// - File attributes
        /// - File size
        /// - File name
        /// - Delete file
        const ALLOW_SET_INFORMATION = 0x00001000;
        /// Allow renaming the file.
        const ALLOW_FILE_RENAME = 0x00002000;
        /// Allow deleting the file.
        const ALLOW_FILE_DELETE = 0x00004000;
        /// Allow changing file size.
        const ALLOW_FILE_SIZE_CHANGE = 0x00008000;
        /// Allow querying the file's security information.
        const ALLOW_QUERY_SECURITY_ACCESS = 0x00010000;
        /// Allow changing the file's security information.
        const ALLOW_SET_SECURITY_ACCESS = 0x00020000;
        /// Allow listing the directory's contents.
        const ALLOW_DIRECTORY_LIST_ACCESS = 0x00040000;
        /// Allow remote access via share folder.
        const ALLOW_FILE_ACCESS_FROM_NETWORK = 0x00080000;
        /// Allow encrypting a new file if the encryption filter rule is enabled.
        const ALLOW_ENCRYPT_NEW_FILE = 0x00100000;
        /// Allow applications to read encrypted files; if not set, return encrypted data.
        const ALLOW_READ_ENCRYPTED_FILES = 0x00200000;
        /// Allow applications to create a new file after opening the protected file.
        const ALLOW_ALL_SAVE_AS = 0x00400000;
        /// Allow copying protected files out of the protected folder if `ALLOW_ALL_SAVE_AS` is enabled.
        const ALLOW_COPY_PROTECTED_FILES_OUT = 0x00800000;
        /// Allow file to be memory mapped.
        const ALLOW_FILE_MEMORY_MAPPED = 0x01000000;
        /// Disable encrypting data on read when the encryption filter is enabled.
        const DISABLE_ENCRYPT_DATA_ON_READ = 0x02000000;
        /// Allow copying protected files to USB storage.
        const ALLOW_COPY_PROTECTED_FILES_TO_USB = 0x04000000;
        /// Disable all access to this file.
        ///
        /// IMPORTANT: Do not set `AccessFlag` to 0.
        /// This is equivalent to disabling all protections
        /// (`EXCLUDE_FILTER_RULE`).
        const LEAST_ACCESS_FLAG = 0xF0000000;
        /// Allow all access to the file.
        const ALLOW_MAX_RIGHT_ACCESS = 0xFFFFFFF0;
    }
}

/// Message types between user-mode and driver.
#[repr(u32)]
#[derive(Debug, Clone, Copy, PartialEq, Eq, Hash)]
pub enum FilterCommand {
    MessageTypeRestoreBlockOrFile = 0x00000001,
    MessageTypeRestoreFileToOriginalFolder = 0x00000002,
    MessageTypeGetFileList = 0x00000004,
    MessageTypeRestoreFileToCache = 0x00000008,
    MessageTypeSendEventNotification = 0x00000010,
    MessageTypeDeleteFile = 0x00000020,
    MessageTypeRenameFile = 0x00000040,
    MessageTypeSendMessageFilename = 0x00000080,
    FilterSendFileChangedEvent = 0x00010001,
    FilterRequestUserPermit = 0x00010002,
    FilterRequestEncryptionKey = 0x00010003,
    FilterRequestEncryptionIvAndKey = 0x00010004,
    FilterRequestEncryptionIvAndKeyAndAccessFlag = 0x00010005,
    FilterRequestEncryptionIvAndKeyAndTagData = 0x00010006,
    FilterSendRegCallbackInfo = 0x00010007,
    FilterSendProcessCreationInfo = 0x00010008,
    FilterSendProcessTerminationInfo = 0x00010009,
    FilterSendThreadCreationInfo = 0x0001000A,
    FilterSendThreadTerminationInfo = 0x0001000B,
    FilterSendProcessHandleInfo = 0x0001000C,
    FilterSendThreadHandleInfo = 0x0001000D,
    FilterSendAttachedVolumeInfo = 0x0001000E,
    FilterSendDetachedVolumeInfo = 0x0001000F,
    FilterSendDeniedFileIoEvent = 0x00010010,
    FilterSendDeniedVolumeDismountEvent = 0x00010011,
    FilterSendDeniedProcessEvent = 0x00010012,
    FilterSendDeniedRegistryAccessEvent = 0x00010013,
    FilterSendDeniedProcessTerminatedEvent = 0x00010014,
    FilterSendDeniedUsbReadEvent = 0x00010015,
    FilterSendDeniedUsbWriteEvent = 0x00010016,
    FilterSendPreTerminateProcessInfo = 0x00010017,
}

bitflags! {
    #[derive(Clone, Copy)]
    /// Return status for callback reply data.
    pub struct FilterStatus: u32 {
        /// Message needs to be processed.
        const FILTER_MESSAGE_IS_DIRTY = 0x00000001;
        /// Pre-operation completed by your code.
        const FILTER_COMPLETE_PRE_OPERATION = 0x00000002;
        /// Callback's reply data contains an update.
        const FILTER_DATA_BUFFER_IS_UPDATED = 0x00000004;
        /// Read block databuffer is returned to filter.
        const FILTER_BLOCK_DATA_WAS_RETURNED = 0x00000008;
        /// The whole cache file was downloaded.
        const FILTER_CACHE_FILE_WAS_RETURNED = 0x00000010;
        /// Whole cache file downloaded, and stub file needs to be rehydrated.
        const FILTER_REHYDRATE_FILE_VIA_CACHE_FILE = 0x00000020;
    }
}

bitflags! {
    #[derive(Clone, Copy)]
    pub struct ProcessControlFlag: u32 {
        /// Deny the new process creation if the flag is on.
        const DENY_NEW_PROCESS_CREATION = 0x00000001;
        /// Send the callback request before the process is going to be terminated.
        ///
        /// You can block the process termination in the callback function.
        const PROCESS_PRE_TERMINATION_REQUEST = 0x00000002;
        /// Get a notification when a new process is being created.
        const PROCESS_CREATION_NOTIFICATION = 0x00000100;
        /// Get a notification when a process was terminated.
        const PROCESS_TERMINATION_NOTIFICATION = 0x00000200;
        /// Get a notification for process handle operations,
        /// when a handle for a process is being created or duplicated.
        const PROCESS_HANDLE_OP_NOTIFICATION = 0x00000400;
        /// Get a notifcation when a new thread is being created.
        const THREAD_CREATION_NOTIFICATION = 0x00000800;
        /// Get a notification when a thread was terminated.
        const THREAD_TERMINATION_NOTIFICATION = 0x00001000;
        /// Get a notification for thread handle operations, when a handle for a process
        /// is being created or duplicated.
        const THREAD_HANDLE_OP_NOTIFICATION = 0x00002000;
    }
}

bitflags! {
    #[derive(Clone, Copy, Debug)]
    /// Configuration and feature switches.
    ///
    /// Use `EaseFilter::configure()` to apply this.
    pub struct BooleanConfig: u32 {
        /// EaseTag: if true, after opening the reparse point file, won't restore data back for read/write.
        const ENABLE_NO_RECALL_FLAG = 0x00000001;
        /// If true, disables unloading the filter driver.
        const DISABLE_FILTER_UNLOAD_FLAG = 0x00000002;
        /// If true, sets the offline attribute for virtual files.
        const ENABLE_SET_OFFLINE_FLAG = 0x00000004;
        /// If true, uses a default IV to encrypt files in encryption mode.
        const ENABLE_DEFAULT_IV_TAG = 0x00000008;
        /// If true, sends message data to a persistent file, otherwise send the event to the service immediately.
        const ENABLE_ADD_MESSAGE_TO_FILE = 0x00000010;
        /// (Version 5.0) If true, encrypted file metadata is embedded in the reparse point tag.
        const ENCRYPT_FILE_WITH_REPARSE_POINT_TAG = 0x00000020;
        /// Deprecated.
        const REQUEST_ENCRYPT_KEY_AND_IV_FROM_SERVICE = 0x00000040;
        /// If true, enables control filter rules at boot time.
        const ENABLE_PROTECTION_IN_BOOT_TIME = 0x00000080;
        /// If true, encryption rules get the encryption key and IV, plus optional tag
        /// data from your user-mode callback code instead of storing it in the driver.
        const REQUEST_ENCRYPT_KEY_IV_AND_TAGDATA_FROM_SERVICE = 0x00000100;
        /// If enabled, will send read/write data to user-mode callback code.
        const ENABLE_SEND_DATA_BUFFER = 0x00000200;
        /// If true, will re-open files when rehydrating the stub.
        const ENABLE_REOPEN_FILE_ON_REHYDRATION = 0x00000400;
        /// If true, queues monitor mode events in a buffer, then processes them asynchronously.
        ///
        /// This avoids blocking I/O requests, but may result in events being dropped when the buffer is full.
        const ENABLE_MONITOR_EVENT_BUFFER = 0x00000800;
        /// If true, sends events even if they are blocked.
        const ENABLE_SEND_DENIED_EVENT = 0x00001000;
        /// If true, and write access is disabled, writes return success and write zero data to the file, and sends write data to user mode.
        const ENABLE_WRITE_WITH_ZERO_DATA_AND_SEND_DATA = 0x00002000;
        /// If true, treats portable massive storage as USB.
        ///
        /// This is for the volume control flag for `BLOCK_USB_READ`, `BLOCK_USB_WRITE`.
        const DISABLE_REMOVABLE_MEDIA_AS_USB = 0x00004000;
        /// If true, blocks moving an encrypted file to a different folder.
        ///
        /// By default, moving a file out of an encrypted folder results in the file
        /// being unable to be decrypted.
        const DISABLE_RENAME_ENCRYPTED_FILE = 0x00008000;
        /// If true, disables file synchronization for file reading in CloudTier.
        const DISABLE_FILE_SYNCHRONIZATION = 0x00010000;
        /// If true, data protection will continue after the service process is stopped.
        const ENABLE_PROTECTION_IF_SERVICE_STOPPED = 0x00020000;
        /// If true and write encrypt info to cache is enabled, signals the system thread to write cached data to disk right away.
        const ENABLE_SIGNAL_WRITE_ENCRYPT_INFO_EVENT = 0x00020000;
        /// Enable this feature for the `ALLOW_SAVE_AS` or
        /// `ALLOW_COPY_PROTECTED_FILES_OUT` accessFlag values. By default this feature
        /// is disabled, because if these two flags are disabled this will block all new
        /// file creation of the process which has read the protected files.
        const ENABLE_BLOCK_SAVE_AS_FLAG = 0x00040000;
    }
}

bitflags! {
    #[derive(Clone, Copy, Debug, PartialEq)]
    /// REGISTRY mode filter driver callback class.
    ///
    /// These are registry events that can be subscribed to.
    pub struct RegCallbackClass: u64 {
        const REG_PRE_DELETE_KEY = 0x00000001;
        const REG_PRE_SET_VALUE_KEY = 0x00000002;
        const REG_PRE_DELETE_VALUE_KEY = 0x00000004;
        const REG_PRE_SET_INFORMATION_KEY = 0x00000008;
        const REG_PRE_RENAME_KEY = 0x00000010;
        const REG_PRE_ENUMERATE_KEY = 0x00000020;
        const REG_PRE_ENUMERATE_VALUE_KEY = 0x00000040;
        const REG_PRE_QUERY_KEY = 0x00000080;
        const REG_PRE_QUERY_VALUE_KEY = 0x00000100;
        const REG_PRE_QUERY_MULTIPLE_VALUE_KEY = 0x00000200;
        const REG_PRE_CREATE_KEY = 0x00000400;
        const REG_POST_CREATE_KEY = 0x00000800;
        const REG_PRE_OPEN_KEY = 0x00001000;
        const REG_POST_OPEN_KEY = 0x00002000;
        const REG_PRE_KEY_HANDLE_CLOSE = 0x00004000;

        // .NET only values
        const REG_POST_DELETE_KEY = 0x00008000;
        const REG_POST_SET_VALUE_KEY = 0x00010000;
        const REG_POST_DELETE_VALUE_KEY = 0x00020000;
        const REG_POST_SET_INFORMATION_KEY = 0x00040000;
        const REG_POST_RENAME_KEY = 0x00080000;
        const REG_POST_ENUMERATE_KEY = 0x00100000;
        const REG_POST_ENUMERATE_VALUE_KEY = 0x00200000;
        const REG_POST_QUERY_KEY = 0x00400000;
        const REG_POST_QUERY_VALUE_KEY = 0x00800000;
        const REG_POST_QUERY_MULTIPLE_VALUE_KEY = 0x01000000;
        const REG_POST_KEY_HANDLE_CLOSE = 0x02000000;
        const REG_PRE_CREATE_KEY_EX = 0x04000000;
        const REG_POST_CREATE_KEY_EX = 0x08000000;
        const REG_PRE_OPEN_KEY_EX = 0x10000000;
        const REG_POST_OPEN_KEY_EX = 0x20000000;

        // Available in Windows Vista and later
        const REG_PRE_FLUSH_KEY = 0x40000000;
        const REG_POST_FLUSH_KEY = 0x80000000;

        const REG_PRE_LOAD_KEY = 0x100000000;
        const REG_POST_LOAD_KEY = 0x200000000;
        const REG_PRE_UNLOAD_KEY = 0x400000000;
        const REG_POST_UNLOAD_KEY = 0x800000000;
        const REG_PRE_QUERY_KEY_SECURITY = 0x1000000000;
        const REG_POST_QUERY_KEY_SECURITY = 0x2000000000;
        const REG_PRE_SET_KEY_SECURITY = 0x4000000000;
        const REG_POST_SET_KEY_SECURITY = 0x8000000000;

        /// Per-object context cleanup.
        const REG_CALLBACK_OBJECT_CONTEXT_CLEANUP = 0x10000000000;

        // Available in Windows Vista SP2 and later
        const REG_PRE_RESTORE_KEY = 0x20000000000;
        const REG_POST_RESTORE_KEY = 0x40000000000;
        const REG_PRE_SAVE_KEY = 0x80000000000;
        const REG_POST_SAVE_KEY = 0x100000000000;
        const REG_PRE_REPLACE_KEY = 0x200000000000;
        const REG_POST_REPLACE_KEY = 0x400000000000;

        // Available in Windows 10 and later
        const REG_PRE_QUERY_KEY_NAME = 0x800000000000;
        const REG_POST_QUERY_KEY_NAME = 0x1000000000000;

        const MAX_REG_CALLBACK_CLASS = 0xFFFFFFFFFFFFFFFF;
    }
}

bitflags! {
    #[derive(Clone, Copy)]
    /// Permission flags for the REGISTRY mode filter.
    pub struct RegControlFlag: u32 {
        const REG_ALLOW_OPEN_KEY = 0x00000001;
        const REG_ALLOW_CREATE_KEY = 0x00000002;
        const REG_ALLOW_QUERY_KEY = 0x00000004;
        const REG_ALLOW_RENAME_KEY = 0x00000008;
        const REG_ALLOW_DELETE_KEY = 0x00000010;
        const REG_ALLOW_SET_VALUE_KEY_INFORMATION = 0x00000020;
        const REG_ALLOW_SET_INFORMATION_KEY = 0x00000040;
        const REG_ALLOW_ENUMERATE_KEY = 0x00000080;
        const REG_ALLOW_QUERY_VALUE_KEY = 0x00000100;
        const REG_ALLOW_ENUMERATE_VALUE_KEY = 0x00000200;
        const REG_ALLOW_QUERY_MULTIPLE_VALUE_KEY = 0x00000400;
        const REG_ALLOW_DELETE_VALUE_KEY = 0x00000800;
        const REG_ALLOW_QUERY_KEY_SECURITY = 0x00001000;
        const REG_ALLOW_SET_KEY_SECRUITY = 0x00002000;
        const REG_ALLOW_RESTORE_KEY = 0x00004000;
        const REG_ALLOW_REPLACE_KEY = 0x00008000;
        const REG_ALLOW_SAVE_KEY = 0x00010000;
        const REG_ALLOW_FLUSH_KEY = 0x00020000;
        const REG_ALLOW_LOAD_KEY = 0x00040000;
        const REG_ALLOW_UNLOAD_KEY = 0x00080000;
        const REG_ALLOW_KEY_CLOSE = 0x00100000;
        const REG_ALLOW_QUERY_KEYNAME = 0x00200000;
        const REG_MAX_ACCESS_FLAG = 0xFFFFFFFF;
    }
}

#[repr(u32)]
#[derive(Debug, Clone, Copy, PartialEq, Eq, Hash, FromRepr)]
pub enum FileStatus {
    Success = 0,
    AccessDenied = 0xC0000022,
    Reparse = 0x00000104,
    NoMoreFiles = 0x80000006,
    Warning = 0x80000000,
    Error = 0xC0000000,
}

/// The specific class a generic POST_SET_INFORMATION event.
///
/// For more information, see <https://learn.microsoft.com/en-us/openspecs/windows_protocols/ms-fscc/4718fc40-e539-4014-8e33-b675af74e3e1>
#[repr(u32)]
#[derive(Debug, Clone, Copy, PartialEq, Eq, Hash, FromRepr)]
pub enum InformationClass {
    FileDirectoryInformation = 1,
    FileFullDirectoryInformation = 2,
    FileBothDirectoryInformation = 3,
    FileBasicInformation = 4,
    FileStandardInformation = 5,
    FileInternalInformation = 6,
    FileEaInformation = 7,
    FileAccessInformation = 8,
    FileNameInformation = 9,
    FileRenameInformation = 10,
    FileLinkInformation = 11,
    FileNamesInformation = 12,
    FileDispositionInformation = 13,
    FilePositionInformation = 14,
    FileFullEaInformation = 15,
    FileModeInformation = 16,
    FileAlignmentInformation = 17,
    FileAllInformation = 18,
    FileAllocationInformation = 19,
    FileEndOfFileInformation = 20,
    FileAlternateNameInformation = 21,
    FileStreamInformation = 22,
    FilePipeInformation = 23,
    FilePipeLocalInformation = 24,
    FilePipeRemoteInformation = 25,
    FileMailslotQueryInformation = 26,
    FileMailslotSetInformation = 27,
    FileCompressionInformation = 28,
    FileObjectIdInformation = 29,
    FileCompletionInformation = 30,
    FileMoveClusterInformation = 31,
    FileQuotaInformation = 32,
    FileReparsePointInformation = 33,
    FileNetworkOpenInformation = 34,
    FileAttributeTagInformation = 35,
    FileTrackingInformation = 36,
    FileIdBothDirectoryInformation = 37,
    FileIdFullDirectoryInformation = 38,
    FileValidDataLengthInformation = 39,
    FileShortNameInformation = 40,
    FileIoCompletionNotificationInformation = 41,
    FileIoStatusBlockRangeInformation = 42,
    FileIoPriorityHintInformation = 43,
    FileSfioReserveInformation = 44,
    FileSfioVolumeInformation = 45,
    FileHardLinkInformation = 46,
    FileProcessIdsUsingFileInformation = 47,
    FileNormalizedNameInformation = 48,
    FileNetworkPhysicalNameInformation = 49,
    FileIdGlobalTxDirectoryInformation = 50,
    FileIsRemoteDeviceInformation = 51,
    FileUnusedInformation = 52,
    FileNumaNodeInformation = 53,
    FileStandardLinkInformation = 54,
    FileRemoteProtocolInformation = 55,
    FileRenameInformationBypassAccessCheck = 56,
    FileLinkInformationBypassAccessCheck = 57,
    FileVolumeNameInformation = 58,
    FileIdInformation = 59,
    FileIdExtdDirectoryInformation = 60,
    FileReplaceCompletionInformation = 61,
    FileHardLinkFullIdInformation = 62,
    FileIdExtdBothDirectoryInformation = 63,
    FileDispositionInformationEx = 64,
    FileRenameInformationEx = 65,
    FileRenameInformationExBypassAccessCheck = 66,
    FileDesiredStorageClassInformation = 67,
    FileStatInformation = 68,
    FileMemoryPartitionInformation = 69,
    FileStatLxInformation = 70,
    FileCaseSensitiveInformation = 71,
    FileLinkInformationEx = 72,
    FileLinkInformationExBypassAccessCheck = 73,
    FileStorageReserveIdInformation = 74,
    FileCaseSensitiveInformationForceAccessCheck = 75,
    FileKnownFolderInformation = 76,
    FileMaximumInformation = 77,
}

/// Namespace for specific process-related [`FilterCommand`] members.
///
/// [`FilterCommand`] can be directly cast to this.
#[repr(u32)]
#[derive(Debug, Clone, Copy, PartialEq, Eq, Hash, FromRepr)]
pub enum ProcessEventType {
    /// Tried to run an executable; blocked by filter.
    ProcessCreationBlocked = 0x00010012,
    /// Process stopped.
    ProcessTerminated = 0x00010009,
    /// New thread created.
    ThreadCreated = 0x0001000A,
    /// Thread stopped.
    ThreadTerminated = 0x0001000B,
    /// Notification for opening a new Windows process handle.
    ProcessHandleInfo = 0x0001000C,
    /// Notification for opening a new Windows thread handle.
    ThreadHandleInfo = 0x0001000D,
    /// Process is being created; can be blocked by your filter code.
    ProcessCreationInfo = 0x00010008,
    /// Process is being terminated forcefully; can be blocked by your filter code.
    ProcessPreTerminateInfo = 0x00010017,
}

/// Namespace for encryption-related [`FilterCommand`] members.
///
/// [`FilterCommand`] can be directly cast to this.
#[repr(u32)]
#[derive(Debug, Clone, Copy, PartialEq, Eq, Hash, FromRepr)]
pub enum EncryptEventType {
    /// Your code has to provide an encryption key.
    RequestKey = 0x00010003,
    /// Your code has to provide an encryption key and IV.
    ///
    /// This event may also contain existing tag data for the file.
    RequestIvAndKey = 0x00010004,
    /// Your code has to provide an encryption key and IV, and optionally set tag data for the file.
    RequestIvAndKeyAndTagData = 0x00010006,
    /// Your code has to provide an encryption key, IV, and the `AccessFlag`.
    RequestIvAndKeyAndAccessFlag = 0x00010005,
}
