What can you do with the EaseFilter SDK

A.File and Folder Monitoring
  Monitor Windows file I/O activities in real time, track the file access and changes, monitor file and folder permission changes, audit who is writing, deleting, moving or reading files, report the user name and process name, get the user name and the ip address when the Windows file server's file is accessed by network user.
B.File Access Control and Security Control
  Control Windows file I/O activities in real time, intercept the file system call, modify its content before or after the request goes down to the file system, allow/deny/cancel its execution based on filter rules. Protect the sensitive files, you can verify the user identity, authenticate them, authorize the file access, prevent the confidential files from being accessed, modified, renamed, deleted, or read by unauthorized users, you also hide your sensitive files to the unauthorized users, protect intellectual property from being copied.
C.File Encryption At-Rest for Enterprise
  Enterprise transparent and continuous file-level encryption protects against unauthorized access by users and processes, secures unstructured data for the enterprise. High-performance hardware accelerated encryption, encryption overhead is minimized using the AES hardware encryption capabilities available in modern CPUs.
D.Process Monitoring and Protection
 Get the callback notification for the process/thread creation or termination, prevent the untrusted executable binaries ( malwares) from being launched.
E.Registry Monitoring and Protection
 Protect Windows core registry keys and values and prevent potentially damaging system configuration changes, besides operating system files. Get the notifications of each registry operation when the registry key was accessed or modified by the applications.

How to use EaseFilter SDK

The EaseFilter control file system filter driver SDK includes two components (EaseFlt.sys and FilterAPI.dll) with 32bit and 64bit version. EaseFlt.sys is the file system filter driver which provides a complete, modular environment for building active file system filters. FilterAPI.dll is a user mode DLL which is responsible for the communication between filter driver and your use mode application ,and it is also a wrapper DLL which exports the API to the user mode applications.
Install/Uninstall the filter driver with admin privilege
InstallDriver()
UnInstallDriver()

Start the filter driver
To start the filter driver, first we need to set the registration key, then register the callback funtion with the worker thread number.
SetRegistrationKey(WCHAR* RegisterKey);

RegisterMessageCallback(ULONG ThreadCount,Proto_Message_Callback MessageCallback,Proto_Disconnect_Callback DisconnectCallback );

Setup the filter driver configuration
To setup the filter driver type with the combination of the below filter type enumeration, then you have have the associated features of the filter driver. If you register the I/O events or callback, setup the maximum time of the filter driver waits for the response from the user mode application.
Typedef  enum  FilterType 
{
	FILE_SYSTEM_CONTROL		= 1,
	FILE_SYSTEM_ENCRYPTION		= 2,
	FILE_SYSTEM_MONITOR		= 4,  
	FILE_SYSTEM_REGISTRY		= 8, 
	FILE_SYSTEM_PROCESS		= 16,
};
		
SetFilterType(ULONG FilterType);

SetConnectionTimeout(ULONG TimeOutInSeconds);
Filter the file I/O with file filter rule
To know which file we want to filter, we need to set the filter rule with the file name filter mask, the FilterMask sets the target folder or files,it can include wild character ‘*’or ‘?’. For example: c:\test\*txt, the filter only monitors I/Os of the files end with ‘txt’ in the folder c:\test. To control the file I/O for the control filter driver, we can set the access flag for the filter rule, the access flags can be the combination of the bits as following enumeration.

typedef enum AccessFlag
{
  EXCLUDE_FILTER_RULE					= 0X00000000,
  EXCLUDE_FILE_ACCESS					= 0x00000001,
  REPARSE_FILE_OPEN					= 0x00000002,
  HIDE_FILES_IN_DIRECTORY_BROWSING			= 0x00000004,
  FILE_ENCRYPTION_RULE					= 0x00000008,
  ALLOW_OPEN_WTIH_ACCESS_SYSTEM_SECURITY		= 0x00000010,
  ALLOW_OPEN_WITH_READ_ACCESS				= 0x00000020,
  ALLOW_OPEN_WITH_WRITE_ACCESS				= 0x00000040,
  ALLOW_OPEN_WITH_CREATE_OR_OVERWRITE_ACCESS		= 0x00000080,
  ALLOW_OPEN_WITH_DELETE_ACCESS				= 0x00000100,
  ALLOW_READ_ACCESS					= 0x00000200,
  ALLOW_WRITE_ACCESS					= 0x00000400,
  ALLOW_QUERY_INFORMATION_ACCESS			= 0x00000800,
  ALLOW_SET_INFORMATION					= 0x00001000,
  ALLOW_FILE_RENAME					= 0x00002000,
  ALLOW_FILE_DELETE					= 0x00004000,
  ALLOW_FILE_SIZE_CHANGE		 		= 0x00008000,
  ALLOW_QUERY_SECURITY_ACCESS				= 0x00010000,
  ALLOW_SET_SECURITY_ACCESS				= 0x00020000,
  ALLOW_DIRECTORY_LIST_ACCESS				= 0x00040000,
  ALLOW_FILE_ACCESS_FROM_NETWORK			= 0x00080000,
  ALLOW_NEW_FILE_ENCRYPTION				= 0x00100000,
  ALLOW_READ_ENCRYPTED_FILES				= 0x00200000,
  ALLOW_ALL_SAVE_AS					= 0x00400000,
  ALLOW_COPY_PROTECTED_FILES_OUT			= 0x00800000,
  ALLOW_FILE_MEMORY_MAPPED				= 0x01000000,
  LEAST_ACCESS_FLAG					= 0xf0000000,
  ALLOW_MAX_RIGHT_ACCESS				= 0xfffffff0,
	
};


AddFileFilterRule(ULONG  AccessFlag,WCHAR* FilterMask, ULONG FilterId)
 
Eexcluded files from the filter rule
If you want to exclude the I/Os of some file for the filter rule, you can add the exclude file filter mask to the filter rule.
AddExcludeFileMaskToFilterRule(WCHAR* FilterMask,WCHAR* ExcludeFileFilterMask);


Example:
Manage the file I/Os for files in folder c:\test, but exclude all the .txt files:


AddFileFilterRule(ALLOW_MAX_RIGHT_ACCESS, L"c:\\test\\*", 1);
AddExcludeFileMaskToFilterRule(L"c:\\test\\*",L"*.txt");
Setup the filter rule only for the specific processes
If you want to setup the filter rule only for some specific processes, you can add the include process name filter mask to the filter rule.
AddIncludeProcessNameToFilterRule(WCHAR* FilterMask,WCHAR* IncludeProcessNameFilterMask);


Example:
Manage the file I/Os for files in folder c:\test only for process "notepad.exe":

AddFileFilterRule(ALLOW_MAX_RIGHT_ACCESS, L"c:\\test\\*", 1);
AddIncludeProcessNameToFilterRule(L"c:\\test\\*",L"notepad.exe");
Setup the filter rule to exclude some specific processes
If you want to setup the filter rule except for some specific processes, you can add the exclude process name filter mask to the filter rule.
AddExcludeProcessNameToFilterRule(WCHAR* FilterMask,WCHAR* ExcludeProcessNameFilterMask);


Example:
Manage the file I/Os for files in folder c:\test except for process "notepad.exe":

AddFileFilterRule(ALLOW_MAX_RIGHT_ACCESS, L"c:\\test\\*", 1);
AddExcludeProcessNameToFilterRule(L"c:\\test\\*",L"notepad.exe");
Setup the filter rule only for the specific users
If you want to setup the filter rule only for some specific users, you can add the include user name filter mask to the filter rule.
AddIncludeUserNameToFilterRule(WCHAR* FilterMask,WCHAR* IncludeUserNameFilterMask);


Example:
Manage the file I/Os for files in folder c:\test only for user "TestDoman\\TestUser":

AddFileFilterRule(ALLOW_MAX_RIGHT_ACCESS, L"c:\\test\\*", 1);
AddIncludeUserNameToFilterRule(L"c:\\test\\*",L"TestDoman\\TestUser");
Setup the filter rule to exclude the specific users
If you want to setup the filter rule except for some specific users, you can add the exclude user name filter mask to the filter rule.
AddExcludeUserNameToFilterRule(WCHAR* FilterMask,WCHAR* ExcludeUserNameFilterMask);


Example:
Manage the file I/Os for files in folder c:\test except for user "TestDoman\\TestUser":

AddFileFilterRule(ALLOW_MAX_RIGHT_ACCESS, L"c:\\test\\*", 1);
AddExcludeUserNameToFilterRule(L"c:\\test\\*",L"TestDoman\\TestUser");
Setup the filter rule to filter the file I/O operation by the sepcific file create options
You can register the preoperation or postoperation I/O operations, set filter for the callback IO by the file open option DesiredAccess, Disposition and CreateOptions.

Example:
Register the PRE_CREATE, only callback when the file opens with DELETE access.

AddFileFilterRule(ALLOW_MAX_RIGHT_ACCESS, L"c:\\test\\*", 1);
RegisterControlToFilterRule(L"c:\\test\\*",PRE_CREATE);
AddRegisterIOFilterToFilterRule(L"c:\\test\\*",DELETE,0,0);

What can you do with the File Monitor Filter Driver SDK

1. Monitor the file I/O events, get the notification of the new file creation, file was written, file was renamed, file was deleted, file security was changed, to know who ( user name and process name ) made those I/Os.
To track the file I/O events, first we need to add the filter rule for the file name filter mask which we want to manage, then register the I/O events we want to track, the register I/O events can be the combination of the bits of the following enumeration. The events will be sent after the I/O was completed and the file handle was closed.

typedef enum FileEventType
{  
  	FILE_WAS_CREATED	= 0x00000020,
	FILE_WAS_WRITTEN	= 0x00000040,
	FILE_WAS_RENAMED	= 0x00000080,
	FILE_WAS_DELETED	= 0x00000100,
	FILE_SECURITY_CHANGED	= 0x00000200,
	FILE_INFO_CHANGED	= 0x00000400,
	FILE_WAS_READ		= 0x00000800,
};

		
RegisterEventTypeToFilterRule(WCHAR* FilterMask, ULONG  EventType );

Example:
Track the file change events ( written, renamed, deleted ) for files in folder c:\test:

AddFileFilterRule(ALLOW_MAX_RIGHT_ACCESS, L"c:\\test\\*", 1);
RegisterEventTypeToFilterRule(L"c:\\test\\*",FILE_WAS_WRITTEN|FILE_WAS_RENAMED|FILE_WAS_DELETED); 


2. Monitor the specific file I/O requests, get the notification to know the file open options ( DesiredAccess, ShareMode, CreationDisposition), to know the read or write offset and length, to know what file information ( file size, file creation time, change time, file attributes) was queried or set.
To track the specific file I/O request, first we need to add the filter rule for the file path filter mask which we want to manage, then register the I/O request type we want to track, the register I/O requests can be the combination of the bits of the following enumeration.

typedef enum  MessageType
{
	POST_CREATE			= 0x00000002,	
	POST_FASTIO_READ		= 0x00000008,	
	POST_CACHE_READ			= 0x00000020,
	POST_NOCACHE_READ		= 0x00000080,
	POST_PAGING_IO_READ		= 0x00000200,
	POST_FASTIO_WRITE		= 0x00000800,
	POST_CACHE_WRITE		= 0x00002000,
	POST_NOCACHE_WRITE		= 0x00008000,
	POST_PAGING_IO_WRITE 		= 0x00020000,
	POST_QUERY_INFORMATION		= 0x00080000,
	POST_SET_INFORMATION		= 0x00200000,
	POST_DIRECTORY			= 0x00800000,
	POST_QUERY_SECURITY		= 0x02000000,
	POST_SET_SECURITY		= 0x08000000,
	POST_CLEANUP			= 0x20000000,
	POST_CLOSE			= 0x80000000, 

};

		
RegisterMonitorToFilterRule(WCHAR* FilterMask,ULONG  RegisterIO);


Example:
Get the notification when the file was opened/read for files in folder c:\test:

AddFileFilterRule(ALLOW_MAX_RIGHT_ACCESS, L"c:\\test\\*", 1);
RegisterMonitorToFilterRule(L"c:\\test\\*",POST_CREATE|POST_FASTIO_READ|POST_CACHE_READ|POST_NOCACHE_READ|POST_PAGING_IO_READ); 
     
What can you do with the File Control Filter Driver SDK

1. Block the new file creation via configuring the access control flag of the filter rule.
Example:
Block the new file creation in folder c:\test:
AddFileFilterRule(ALLOW_MAX_RIGHT_ACCESS&(~ALLOW_OPEN_WITH_CREATE_OR_OVERWRITE_ACCESS), L"c:\\test\\*", 1);

2. Prevent your sensitive files from being copied out of your protected folder
Example:
Prevent the files in folder c:\test from being copied out.
AddFileFilterRule(ALLOW_MAX_RIGHT_ACCESS&(~ALLOW_COPY_PROTECTED_FILES_OUT), L"c:\\test\\*", 1);
     
3. Prevent your sensitive files from being modified, renamed or deleted
Example:
Prevent the file from being modified, renamed or deleted in folder c:\test:
AddFileFilterRule(ALLOW_MAX_RIGHT_ACCESS&(~(ALLOW_WRITE_ACCESS|ALLOW_FILE_RENAME|ALLOW_FILE_DELETE), L"c:\\test\\*", 1);

4. Prevent your sensitive files from being accessed from the network computer
Example:
Protect the files in folder c:\test, block the file access from the network.
AddFileFilterRule(ALLOW_MAX_RIGHT_ACCESS&(~ALLOW_FILE_ACCESS_FROM_NETWORK), L"c:\\test\\*", 1);

5. Hide your sensitive files to the specific processes or users
  
Example:
Hide the files in folder c:\test for process "explorer.exe"
AddFileFilterRule(ALLOW_MAX_RIGHT_ACCESS|HIDE_FILES_IN_DIRECTORY_BROWSING, L"c:\\test\\*", 1);
AddIncludeProcessNameToFilterRule(L"c:\\test\\*",L"explorer.exe");
AddHiddenFileMaskToFilterRule(L"c:\\test\\*",L"*.*");

6. Reparse your file open from one location to another location.
Example:
Reparse the file open in folder c:\test to another folder c:\reparseFolder"
AddFileFilterRule(ALLOW_MAX_RIGHT_ACCESS|REPARSE_FILE_OPEN, L"c:\\test\\*", 1);
AddReparseFileMaskToFilterRule(L"c:\\test\\*",L"c:\\reparseFolder\\*");

7. Allow or deny the specific file I/O operation via registering the specific I/O callback routine based on the process name, user name or the file I/O information.
Example:
Register the PRE_CREATE, PRE_SETINFORMATION I/O for folder c:\test, you can allow or deny the file opern, creation, deletion, rename in the callback routine.
AddFileFilterRule(ALLOW_MAX_RIGHT_ACCESS, L"c:\\test\\*", 1);
RegisterControlToFilterRule(L"c:\\test\\*",PRE_CREATE|PRE_SET_INFORMATION);

8. Authorize or De-authorize the file access rights (read,write,rename,delete..) to the specific processes or users.
Example:
Set the full access rights to the process "notepad.exe", set the readonly access rights to the process "wordpad.exe", remove all the access rights to other processes.
AddFileFilterRule(LEAST_ACCESS_FLAG, L"c:\\test\\*", 1);
AddProcessRightsToFilterRule(L"c:\\test\\*",L"notepad.exe",ALLOW_MAX_RIGHT_ACCESS);
AddProcessRightsToFilterRule(L"c:\\test\\*",L"wordpad.exe",ALLOW_MAX_RIGHT_ACCESS&(~(ALLOW_OPEN_WITH_CREATE_OR_OVERWRITE_ACCESS|ALLOW_WRITE_ACCESS|ALLOW_FILE_RENAME|ALLOW_FILE_DELETE|ALLOW_SET_INFORMATION));

What can you do with the Transparent File Encryption Filter Driver SDK

1. Automatically encrypt or decrypt the file in Windows kernel memory, always keep the file encryped on disk. Per file encryption on the fly, protect your sensitive files with data encryption at rest.
Example:
Transparent encrypt or decrypt files in folder c:\test automatically with AES 256bits key.
AddFileFilterRule(ALLOW_MAX_RIGHT_ACCESS|FILE_ENCRYPTION_RULE, L"c:\\test\\*", 1);

//256 bit,32bytes encrytpion key
unsigned char key[] = {0x60,0x3d,0xeb,0x10,0x15,0xca,0x71,0xbe,0x2b,0x73,0xae,0xf0,0x85,0x7d,0x77,0x81,0x1f,0x35,0x2c,0x07,0x3b,0x61,0x08,0xd7,0x2d,0x98,0x10,0xa3,0x09,0x14,0xdf,0xf4};
AddEncryptionKeyToFilterRule(L"c:\\test\\*",sizeof(key),key);

2. Create white list and black list of the users and processes for encrypted files, the white list of the users or processes will get the decrypted data when they read the encrypted file, the black list of the users or pocesses will get the encrypted data when they read the encrypted files.
Example:
Transparent encrypt files in folder c:\test automatically with AES 256bits key, only authorized process "notepad.exe" can read the encrypted file, 
so when you copy the encrypted file in Windows explorer, the encrypted files will be copied out instead of the decrypted files.
AddFileFilterRule((ALLOW_MAX_RIGHT_ACCESS|FILE_ENCRYPTION_RULE)&(~ALLOW_READ_ENCRYPTED_FILES), L"c:\\test\\*", 1);

//256 bit,32bytes encrytpion key
unsigned char key[] = {0x60,0x3d,0xeb,0x10,0x15,0xca,0x71,0xbe,0x2b,0x73,0xae,0xf0,0x85,0x7d,0x77,0x81,0x1f,0x35,0x2c,0x07,0x3b,0x61,0x08,0xd7,0x2d,0x98,0x10,0xa3,0x09,0x14,0xdf,0xf4};
AddEncryptionKeyToFilterRule(L"c:\\test\\*",sizeof(key),key);
AddProcessRightsToFilterRule(L"c:\\test\\*",L"notepad.exe",ALLOW_MAX_RIGHT_ACCESS);

3. Only encrypt your files when it was sending out or copying out from your computer, your file is not encrypted in the local disk, the data was encrypted only when it was read.

Example:
The files will be encrypted when the process "outlook.exe"  read the files in folder c:\test, the new created files in folder c:\test won't be encrypted automatically.

AddFileFilterRule((ALLOW_MAX_RIGHT_ACCESS|FILE_ENCRYPTION_RULE)&(~ALLOW_ENCRYPT_NEW_FILES), L"c:\\test\\*", 1);

//256 bit,32bytes encrytpion key
unsigned char key[] = {0x60,0x3d,0xeb,0x10,0x15,0xca,0x71,0xbe,0x2b,0x73,0xae,0xf0,0x85,0x7d,0x77,0x81,0x1f,0x35,0x2c,0x07,0x3b,0x61,0x08,0xd7,0x2d,0x98,0x10,0xa3,0x09,0x14,0xdf,0xf4};
AddEncryptionKeyToFilterRule(L"c:\\test\\*",sizeof(key),key);
AddProcessRightsToFilterRule(L"c:\\test\\*",L"outlook.exe",(ALLOW_MAX_RIGHT_ACCESS|FILE_ENCRYPTION_RULE)&(~DISABLE_ENCRYPT_ON_READ));

After encrypted file was sent out to your customer, the customer needs to setup a folder "c:\dropFolder" which enables the process can read encrypted file, disable the automatically new file encryption feature.
AddFileFilterRule((ALLOW_MAX_RIGHT_ACCESS|FILE_ENCRYPTION_RULE)&(~ALLOW_ENCRYPT_NEW_FILES), L"c:\\dropFolder\\*", 1);

//256 bit,32bytes encrytpion key
unsigned char key[] = {0x60,0x3d,0xeb,0x10,0x15,0xca,0x71,0xbe,0x2b,0x73,0xae,0xf0,0x85,0x7d,0x77,0x81,0x1f,0x35,0x2c,0x07,0x3b,0x61,0x08,0xd7,0x2d,0x98,0x10,0xa3,0x09,0x14,0xdf,0xf4};
AddEncryptionKeyToFilterRule(L"c:\\dropFolder\\*",sizeof(key),key);

What can you do with the Process Filter Driver SDK

1. Get the notification of the process or thread creation, termination by registering the process callback control flag.
Example:
Get the notification when any process or thread was created, terminated or other handle operations.

AddProcessFilterRule(2, L"*", PROCESS_CREATION_NOTIFICATION|PROCESS_TERMINATION_NOTIFICATION|THREAD_CREATION_NOTIFICATION|THREAD_TERMINATION_NOTIFICATION,0);

2. Prevent the untrusted binaries from being launched, block the downloaded malware files from internet running, block the untrusted excutable files from email running.
Example:
Block the processes running from the folder c:\untrustFiles.

AddProcessFilterRule(wcslen(L"c:\\untrustFiles\\*")*2, L"c:\\untrustFiles\\*", DENY_NEW_PROCESS_CREATION,0);
  
3. Prevent your important process from being terminated.
Example:
Protect the process from being terminated by process Id.

AddProtectedProcessId(processId);
  
4. Restrict the file access rights of the folder for the specific processes.
Example:
Set readonly access to the folder c:\windows for the process "notepad.exe", set the full access rights to the folder c:\test for the process "notepad.exe".

AddFileControlToProcessByName(wcslen(L"notepad.exe")*2, L"notepad.exe", wcslen(L"c:\\windows\\*")*2,L"c:\\windows\\*"
,ALLOW_MAX_RIGHT_ACCESS&(~(ALLOW_OPEN_WITH_CREATE_OR_OVERWRITE_ACCESS|ALLOW_WRITE_ACCESS|ALLOW_FILE_RENAME|ALLOW_FILE_DELETE|ALLOW_SET_INFORMATION));

AddFileControlToProcessByName(wcslen(L"notepad.exe")*2, L"notepad.exe", wcslen(L"c:\\test\\*")*2,L"c:\\test\\*",ALLOW_MAX_RIGHT_ACCESS );
    
What can you do with the Registry Filter Driver SDK

1.Prevent the registries from being modified for the specific processes, restrict the registry access rights to the specific processes.
Example:
Set the registry readonly access rights to the process "notepad.exe".

AddRegistryFilterRuleByProcessName(wcslen(L"notepad.exe")*2, L"notepad.exe",ALLOW_READ_REGITRY_ACCESS_FLAG,0,0 );
  
2. Get the notification of the registry operations to the specific processes by registering the registry callback class.
Example:
Get all the notification of the registry operations for the process "notepad.exe".

AddRegistryFilterRuleByProcessName(wcslen(L"notepad.exe")*2, L"notepad.exe",REG_MAX_ACCESS_FLAG,MAX_REG_CALLBACK_CLASS,0 );

For more information, please go to here:
https://www.easefilter.com/programming.htm


## EaseFilter File System Filter Driver SDK Reference
| Product Name | Description |
| --- | --- |
| [File Monitor SDK](https://www.easefilter.com/kb/file-monitor-filter-driver-sdk.htm) | EaseFilter File Monitor Filter Driver SDK Introduction. |
| [File Control SDK](https://www.easefilter.com/kb/file-control-file-security-sdk.htm) | EaseFilter File Control Filter Driver SDK Introduction. |
| [File Encryption SDK](https://www.easefilter.com/kb/transparent-file-encryption-filter-driver-sdk.htm) | EaseFilter Transparent File Encryption Filter Driver SDK Introduction. |
| [Registry Filter SDK](https://www.easefilter.com/kb/registry-filter-drive-sdk.htm) | EaseFilter Registry Filter Driver SDK Introduction. |
| [Process Filter SDK](https://www.easefilter.com/kb/process-filter-driver-sdk.htm) | EaseFilter Process Filter Driver SDK Introduction. |
| [Storage Tiering SDK](https://www.easefilter.com/cloud/storage-tiering-sdk.htm) | EaseFilter Storage Tiering Filter Driver SDK Introduction. |
| [EaseFilter SDK Programming](https://www.easefilter.com/kb/programming.htm) | EaseFilter Filter Driver SDK Programming. |

## EaseFilter SDK Sample Projects
| Sample Project | Description |
| --- | --- |
| [Auto File DRM Encryption](https://www.easefilter.com/kb/auto-file-drm-encryption-tool.htm) | Auto file encryption with DRM data embedded. |
| [Transparent File Encrypt](https://www.easefilter.com/kb/AutoFileEncryption.htm) | Transparent on access file encryption. |
| [Secure File Sharing with DRM](https://www.easefilter.com/kb/DRM_Secure_File_Sharing.htm) | Secure encrypted file sharing with digital rights management. |
| [File Monitor Example](https://www.easefilter.com/kb/file-monitor-demo.htm) | Monitor file system I/O in real time, tracking file changes. |
| [File Protector Example](https://www.easefilter.com/kb/file-protector-demo.htm) | Prevent sensitive files from being accessed by unauthorized users or processes. |
| [FolderLocker Example](https://www.easefilter.com/kb/FolderLocker.htm) | Lock file automatically in a FolderLocker. |
| [Process Monitor](https://www.easefilter.com/kb/Process-Monitor.htm) | Monitor the process creation and termination, block unauthorized process running. |
| [Registry Monitor](https://www.easefilter.com/kb/RegMon.htm) | Monitor the Registry activities, block the modification of the Registry keys. |
| [Secure Sandbox Example](https://www.easefilter.com/kb/Secure-Sandbox.htm) |A secure sandbox example, block the processes accessing the files out of the box. |
| [FileSystemWatcher Example](https://www.easefilter.com/kb/FileSystemWatcher.htm) | File system watcher, logging the file I/O events. |

## Filter Driver Reference

* Understand MiniFilter Driver: https://www.easefilter.com/kb/understand-minifilter.htm
* Understand File I/O: https://www.easefilter.com/kb/File_IO.htm
* Understand I/O Request Packets(IRPs):https://www.easefilter.com/kb/understand-irps.htm
* Filter Driver Developer Guide: https://www.easefilter.com/kb/DeveloperGuide.htm
* MiniFilter Filter Driver Framework: https://www.easefilter.com/kb/minifilter-framework.htm
* Isolation Filter Driver: https://www.easefilter.com/kb/Isolation_Filter_Driver.htm

## Support
If you have questions or need help, please contact support@easefilter.com 

[Home](https://www.easefilter.com/) | [Solution](https://www.easefilter.com/solutions.htm) | [Download](https://www.easefilter.com/download.htm) | [Demos](https://www.easefilter.com/online-fileio-test.aspx) | [Blog](https://blog.easefilter.com/) | [Programming](https://www.easefilter.com/kb/programming.htm)
