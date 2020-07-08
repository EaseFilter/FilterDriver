///////////////////////////////////////////////////////////////////////////////
//
//    (C) Copyright 2011 EaseFilter Technologies
//    All Rights Reserved
//
//    This software is part of a licensed software product and may
//    only be used or copied in accordance with the terms of that license.
//
//    NOTE:  THIS MODULE IS UNSUPPORTED SAMPLE CODE
//
//    This module contains sample code provided for convenience and
//    demonstration purposes only,this software is provided on an 
//    "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, 
//     either express or implied.  
//
///////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32.SafeHandles;
using System.Security.AccessControl;
using System.Security.Principal;

using EaseFilter.CommonObjects;

namespace FileProtector
{
    public partial class FileProtectorUnitTest : Form
    {

        bool isUnitTestCompleted = false;

        /// <summary>
        ///  To manage your files, you need to create at least one filter rule, you can have multiple filter rules. 
        ///  A filter rule only can have one unique include file mask,
        ///  A filter rule can have multiple exclude file masks, multiple include process names and exclude process names, 
        ///  multiple include process Ids and exclude process Ids, multiple include user names and exclude user names. 
        /// </summary>
        private static string unitTestFolder = "c:\\EaseFilterUnitTest";
        private static string unitTestFile = "c:\\EaseFilterUnitTest\\unitTestFile.txt";

        /// <summary>
        /// Test monitor feature with registering the IO events, get notification after the file was closed for the registered IO.
        /// </summary>
        private static string unitTestMonitorTestFolder = "c:\\EaseFilterUnitTest\\monitorFolder";
        private static string unitTestMonitorTestFile = "c:\\EaseFilterUnitTest\\monitorFolder\\unitTestMonitorTestFile.txt";
        private static bool isTestMonitorFileCreated = false;
        private static bool isTestMonitorFileReanmed = false;
        private static bool isTestMonitorFileDeleted = false;
        private static bool isTestMonitorFileWritten = false;
        private static bool isTestMonitorFileRead = false;
        private static bool isTestMonitorFileSecurityChanged = false;
        private static bool isTestMonitorFileInfoChanged = false;

        public static bool isNewProcessCreated = false;


        /// <summary>
        /// Test Control IO feature with callback function, you can allow or block the file access in the callback function based on the 
        /// user and file information, here we demo how to block the new file creation, file rename, file delete IO.
        /// </summary>
        private static string unitTestCallbackFolder = "c:\\EaseFilterUnitTest\\callbackFolder";
        private static string unitTestCallbackFile = "c:\\EaseFilterUnitTest\\callbackFolder\\unitTestFile.txt";
        private static string unitTestCallbackBlockNewFile = unitTestCallbackFolder + "\\blockNewFileCreationFile.txt";
        private static string unitTestCopyBeforeDeleteCallbackFile = "c:\\EaseFilterUnitTest\\callbackFolder\\unitTestCopyBeforeDeleteCallbackFile.txt";
        private static string unitTestCopyAfterDeleteCallbackFile = "c:\\EaseFilterUnitTest\\callbackFolder\\unitTestCopyAfterDeleteCallbackFile.txt";

      
        /// <summary>
        /// Set the exclude folders for a filter rule, exclude file mask must be the subset of the include file mask.
        /// All IO from the file names which match both include file mask and exclude file mask won't be intercepted by filter driver, 
        /// it meant it will be skipped.
        /// </summary>
        private static string filterRuleExcludeTestFolder  = "c:\\EaseFilterUnitTest\\excludeFolder";
        private static string excludeFolderTestFile = "c:\\EaseFilterUnitTest\\excludeFolder\\excludeFile.txt";


        /// <summary>
        /// Set the exclude filter rule, it meant all IO from the excludeFilterRuleTestFolder won't be intercepted by filter driver.
        /// Exclude filter rule is a global setting, exclude filter rule has the highest priority.
        /// </summary>
        private static string globalExcludeFilterRuleTestFolder = "c:\\EaseFilterUnitTest\\excludeFilterRuleFolder";
        private static string globalExcludeFilterRuleTestFile = "c:\\EaseFilterUnitTest\\excludeFilterRuleFolder\\excludeFilterRuleTestFile.txt";


        public FileProtectorUnitTest()
        {
            InitializeComponent();

        }

        private static bool CopyFileBeforeDeltion(FilterAPI.MessageSendData messageSend)
        {
            try
            {
               

                FilterAPI.MessageType messageType = (FilterAPI.MessageType)messageSend.MessageType;
                WinData.FileInfomationClass infoClass = (WinData.FileInfomationClass)messageSend.InfoClass;


                bool isFileDeleting = false;
                if (messageSend.Status == (uint)NtStatus.Status.Success)
                {
                    if (messageType == FilterAPI.MessageType.PRE_CREATE)
                    {
                        if ((messageSend.CreateOptions & (uint)WinData.CreateOptions.FILE_DELETE_ON_CLOSE) > 0)
                        {
                            isFileDeleting = true;
                        }
                    }
                    else if (messageType == FilterAPI.MessageType.PRE_SET_INFORMATION)
                    {
                        if (infoClass == WinData.FileInfomationClass.FileDispositionInformation)
                        {
                            isFileDeleting = true;
                        }
                    }

                
                    if (isFileDeleting)
                    {

                        IntPtr fileHandle = IntPtr.Zero;
                        //bool retVal = FilterAPI.GetFileHandleInFilter(messageSend.FileName, (uint)FileAccess.Read, ref fileHandle);
                        //if (retVal)
                        //{
                        //    SafeFileHandle sHandle = new SafeFileHandle(fileHandle, true);
                        //    FileStream fileStream = new FileStream(sHandle, FileAccess.Read);

                        //    FileStream copyFileStream = new FileStream(unitTestCopyAfterDeleteCallbackFile, FileMode.Create, FileAccess.Write, FileShare.None);

                        //    byte[] buffer = new byte[4096];
                        //    int readLen = 0;

                        //    do
                        //    {
                        //        readLen = fileStream.Read(buffer, 0, buffer.Length);

                        //        if (readLen > 0)
                        //        {
                        //            copyFileStream.Write(buffer, 0, readLen);
                        //        }
                        //    }
                        //    while (readLen > 0);

                        //    copyFileStream.Close();
                        //    fileStream.Close();

                         
                        //}

                    }
                }
            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(156, "CopyFileBeforeDeltion", EventLevel.Error, "CopyFileBeforeDeltion failed." + ex.Message);
            }
               
            return true;
        }

        public static bool FileIOEventHandler(FilterAPI.MessageSendData messageSend)
        {
           
            if (messageSend.MessageType == (uint)FilterAPI.FilterCommand.FILTER_SEND_PROCESS_CREATION_INFO)
            {
                isNewProcessCreated = true;
                return true;
            }

            uint messageType = messageSend.MessageType;
            uint infoClass = messageSend.InfoClass;
            string fileName = messageSend.FileName;

            if (!fileName.ToLower().StartsWith(unitTestMonitorTestFolder.ToLower()))
            {
                return true;
            }

            if (messageType == (uint)FilterAPI.FilterCommand.FILTER_SEND_FILE_CHANGED_EVENT)
            {
                if ((infoClass & (uint)FilterAPI.EVENTTYPE.CREATED) == (uint)FilterAPI.EVENTTYPE.CREATED)
                {
                    isTestMonitorFileCreated = true;
                }
                if ((infoClass & (uint)FilterAPI.EVENTTYPE.RENAMED) == (uint)FilterAPI.EVENTTYPE.RENAMED)
                {
                    isTestMonitorFileReanmed = true;
                }
                if ((infoClass & (uint)FilterAPI.EVENTTYPE.DELETED) == (uint)FilterAPI.EVENTTYPE.DELETED)
                {
                    isTestMonitorFileDeleted = true;
                }
                if ((infoClass & (uint)FilterAPI.EVENTTYPE.WRITTEN) == (uint)FilterAPI.EVENTTYPE.WRITTEN)
                {
                    isTestMonitorFileWritten = true;
                }
                if ((infoClass & (uint)FilterAPI.EVENTTYPE.INFO_CHANGED) == (uint)FilterAPI.EVENTTYPE.INFO_CHANGED)
                {
                    isTestMonitorFileInfoChanged = true;
                }
                if ((infoClass & (uint)FilterAPI.EVENTTYPE.READ) == (uint)FilterAPI.EVENTTYPE.READ)
                {
                    isTestMonitorFileRead = true;
                }
                if ((infoClass & (uint)FilterAPI.EVENTTYPE.SECURITY_CHANGED) == (uint)FilterAPI.EVENTTYPE.SECURITY_CHANGED)
                {
                    isTestMonitorFileSecurityChanged = true;
                }
            }

            return true;
        }

        public static bool UnitTestCallbackHandler(FilterAPI.MessageSendData messageSend)
        {
            try
            {
                string fileName = messageSend.FileName.ToLower();               

                if (fileName.StartsWith(unitTestCopyBeforeDeleteCallbackFile.ToLower()))
                {
                    CopyFileBeforeDeltion(messageSend);
                    return true;
                }

                if (!fileName.StartsWith(unitTestCallbackFolder.ToLower()))
                {
                    return true;
                }

                FilterAPI.MessageType messageType = (FilterAPI.MessageType)messageSend.MessageType;
                WinData.FileInfomationClass infoClass = (WinData.FileInfomationClass)messageSend.InfoClass;

                bool isTestFileDeleteRequest = false;
                bool isTestFileRenameRequest = false;

                if (messageType == FilterAPI.MessageType.PRE_CREATE)
                {
                    if (fileName.StartsWith(unitTestCallbackBlockNewFile.ToLower()))
                    {
                        if (messageSend.Disposition != (uint)WinData.Disposition.FILE_OPEN)
                        {
                            //this is going to create a new test callback file,it will be blocked.
                            return false;
                        }
                    }


                    if ((messageSend.CreateOptions & (uint)WinData.CreateOptions.FILE_DELETE_ON_CLOSE) > 0)
                    {
                        isTestFileDeleteRequest = true;
                    }
                }
                else if (messageType == FilterAPI.MessageType.PRE_SET_INFORMATION)
                {
                    if (infoClass == WinData.FileInfomationClass.FileDispositionInformation)
                    {
                        isTestFileDeleteRequest = true;
                    }
                    else if (infoClass == WinData.FileInfomationClass.FileRenameInformation)
                    {
                        isTestFileRenameRequest = true;
                    }
                }

                if (isTestFileDeleteRequest || isTestFileRenameRequest)
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(156, "UnitTestCallbackHandler", EventLevel.Error, "UnitTestCallbackHandler failed." + ex.Message);
            }


            return true;
        }


        private void InitializeUnitTest()
        {
            if (!Directory.Exists(unitTestFolder))
            {
                Directory.CreateDirectory(unitTestFolder);
            }

            if (!File.Exists(unitTestFile))
            {
                File.AppendAllText(unitTestFile, "This is unit test file.");
            }

            if (!Directory.Exists(unitTestMonitorTestFolder))
            {
                Directory.CreateDirectory(unitTestMonitorTestFolder);
            }


            if (!Directory.Exists(globalExcludeFilterRuleTestFolder))
            {
                Directory.CreateDirectory(globalExcludeFilterRuleTestFolder);
            }

            if (!File.Exists(globalExcludeFilterRuleTestFile))
            {
                File.AppendAllText(globalExcludeFilterRuleTestFile, "This is unit test excludeFilterRuleTestFolder file.");
            }


            if (!Directory.Exists(filterRuleExcludeTestFolder))
            {
                Directory.CreateDirectory(filterRuleExcludeTestFolder);
            }

            if (!File.Exists(excludeFolderTestFile))
            {
                File.AppendAllText(excludeFolderTestFile, "This is excludeFolderTestFile test file.");
            }

            if (!Directory.Exists(unitTestCallbackFolder))
            {
                Directory.CreateDirectory(unitTestCallbackFolder);
            }

            if (!File.Exists(unitTestCallbackFile))
            {
                File.AppendAllText(unitTestCallbackFile, "This is unitTestCallbackFile test file.");
            }

            if (!File.Exists(unitTestCopyBeforeDeleteCallbackFile))
            {
                File.AppendAllText(unitTestCopyBeforeDeleteCallbackFile, "This is unitTestCopyBeforeDeleteCallbackFile test file.");
            }
        }

        private void CleanupTestFolder(string folder)
        {
            try
            {
                if (Directory.Exists(folder))
                {
                    Directory.Delete(folder, true);
                }

            }
            catch (Exception ex)
            {
                AppendText("Clean up test folder failed:" + ex.Message, Color.Red);
            }
        }

        private void ProcessFilterUnitTest()
        {

            string message = "Process Filter With Callback Function Unit Test";
            AppendText(message, Color.Black);

            message = " 1. Register DENY_NEW_PROCESS_CREATION to test deny new process creation." + Environment.NewLine;
            message += " 2. The callback function will get the notification before the new process was created, and block the new process creation." + Environment.NewLine;

            AppendText(message, Color.Gray);

            ProcessFilterRule processControlFilterRule = new ProcessFilterRule();
            processControlFilterRule.ProcessNameFilterMask = "notepad.exe";
            processControlFilterRule.ControlFlag = (uint)(FilterAPI.ProcessControlFlag.DENY_NEW_PROCESS_CREATION | FilterAPI.ProcessControlFlag.PROCESS_CREATION_NOTIFICATION);

            try
            {
                GlobalConfig.ProcessFilterRules.Clear();
                GlobalConfig.AddProcessFilterRule(processControlFilterRule);
                GlobalConfig.SendConfigSettingsToFilter();

                try
                {
                    Process testTool = new Process();
                    testTool.StartInfo.FileName = "notepad.exe";
                    testTool.Start();

                    AppendText("Test process filter failed. Start new process succeeded.", Color.Red);
                }
                catch( Exception ex)
                {
                    AppendText("Launch process 'notepad.exe' failed with error:" + ex.Message, Color.Gray);
                    AppendText("Test DENY_NEW_PROCESS_CREATION passed.", Color.Green);

                    if (isNewProcessCreated)
                    {
                        AppendText("Test PROCESS_CREATION_NOTIFICATION passed.", Color.Green);
                    }
                }

            }
            catch (Exception ex)
            {
                AppendText("Test process filter failed." + ex.Message, Color.Red);
            }
            finally
            {
                GlobalConfig.RemoveProcessFilterRule(processControlFilterRule);
                GlobalConfig.SendConfigSettingsToFilter();
            }
        }

        public void StartFilterUnitTest()
        {
            try
            {
                FilterAPI.ResetConfigData();

                CleanupTestFolder(unitTestFolder);

                InitializeUnitTest();

                FileEventMonitorTest();

                FolderProtectionUnitTest();

                AddProcessRightsUnitTest();

                AddUserRightsUnitTest();

                NoRenameAndCopyUnitTest();

                FolderLockerUnitTest();

                FileAccessControlUnitTest();

                CopyFileBeforeDeletionUnitTest();

                ShowFileEventMonitorTestResult();

                GlobalConfig.Load();

              
            }
            catch (Exception ex)
            {
                richTextBox_TestResult.Text = "Filter test exception:" + ex.Message;
            }
        }

        private void AppendText(string text, Color color)
        {
            if (color == Color.Black)
            {
                richTextBox_TestResult.AppendText(Environment.NewLine);
                richTextBox_TestResult.SelectionFont = new Font("Arial", 12, FontStyle.Bold);
            }

            richTextBox_TestResult.SelectionStart = richTextBox_TestResult.TextLength;
            richTextBox_TestResult.SelectionLength = 0;

            richTextBox_TestResult.SelectionColor = color;
            richTextBox_TestResult.AppendText(text + Environment.NewLine);
            richTextBox_TestResult.SelectionColor = richTextBox_TestResult.ForeColor;

            if (color == Color.Black)
            {
                richTextBox_TestResult.AppendText(Environment.NewLine);
            }


        }

        private void FileEventMonitorTest()
        {
            //
            //Test case1: Monitor File IO ( creation,rename,delete,write,security change,file info change, file read)
            //Register file events to get the notification when the registered IO was triggered.
            //This test will notify the UnitTestCallbackHandler with the IO events which we did below.
            //


            FilterRule monitorFilterRule = new FilterRule();
            monitorFilterRule.IncludeFileFilterMask = unitTestMonitorTestFolder + "\\*";
            monitorFilterRule.AccessFlag = FilterAPI.ALLOW_MAX_RIGHT_ACCESS;
            monitorFilterRule.EventType = (uint)(FilterAPI.EVENTTYPE.CREATED | FilterAPI.EVENTTYPE.DELETED | FilterAPI.EVENTTYPE.RENAMED | FilterAPI.EVENTTYPE.WRITTEN
                                                 | FilterAPI.EVENTTYPE.READ | FilterAPI.EVENTTYPE.INFO_CHANGED | FilterAPI.EVENTTYPE.SECURITY_CHANGED);

            try
            {
                GlobalConfig.FilterRules.Clear();
                GlobalConfig.FilterRules.Add(monitorFilterRule.IncludeFileFilterMask, monitorFilterRule);
                GlobalConfig.SendConfigSettingsToFilter();

                //create and write events
                File.AppendAllText(unitTestMonitorTestFile, "This is IO events monitor test file.");

                //read event
                File.ReadAllText(unitTestMonitorTestFile);

                //file info change event
                File.SetAttributes(unitTestMonitorTestFile, FileAttributes.Normal);

                // Get a FileSecurity object that represents the current security settings.
                DirectoryInfo dInfo = new DirectoryInfo(unitTestMonitorTestFile);
                DirectorySecurity dSecurity = dInfo.GetAccessControl();
                dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                 //test security change event
                dInfo.SetAccessControl(dSecurity);

                //test rename event.
                string newMonitorFileName = unitTestMonitorTestFolder + "\\newMonitorFileName.txt";
                File.Move(unitTestMonitorTestFile, newMonitorFileName);

                //test delete event.
                File.Delete(newMonitorFileName);
            }
            catch (Exception ex)
            {
                AppendText("FileEventMonitorTest failed." + ex.Message, Color.Red);
            }
          

        }

        private void ShowFileEventMonitorTestResult()
        {

            /// print out the monitor IO events test result.
            string message = "File IO Events Unit Test.";
            AppendText(message,Color.Black);
            message = "Make sure your monitor filter is enabled. Register the monitor file IO events: create,rename,delete,written,file info changed, read, security change in the test folder." + Environment.NewLine;
            AppendText(message, Color.Gray);

            //sleep 4 seconds to wait for the callback events ready.
            System.Threading.Thread.Sleep(4000);

            if (isTestMonitorFileCreated)
            {
                AppendText("Test new file creation event passed.", Color.Green);
                AppendText("A test file was created.", Color.Gray);
            }
            else
            {
                AppendText("Test new file creation event failed.", Color.Red);
            }

            if (isTestMonitorFileInfoChanged)
            {
                AppendText("Test file info change event passed.", Color.Green);
                AppendText("The test file's attributes were changed.", Color.Gray);
            }
            else
            {
                AppendText("Test file info change event failed.", Color.Red);
            }

            if (isTestMonitorFileRead)
            {
                AppendText("Test file read event passed.", Color.Green);
                AppendText("The test file's data was read.", Color.Gray);
            }
            else
            {
                AppendText("Test file read event  failed.", Color.Red);
            }
            if (isTestMonitorFileReanmed)
            {
                AppendText("Test file rename event passed.", Color.Green);
                AppendText("The test file was renamed.", Color.Gray);
            }
            else
            {
                AppendText("Test file rename event failed.", Color.Red);
            }
            if (isTestMonitorFileSecurityChanged)
            {
                AppendText("Test file security change event passed.", Color.Green);
                AppendText("The test file's security was changed.", Color.Gray);
            }
            else
            {
                AppendText("Test file security change event failed.", Color.Red);
            }

            if (isTestMonitorFileWritten)
            {
                AppendText("Test file written event passed.", Color.Green);
                AppendText("The test file was written.", Color.Gray);
            }
            else
            {
                AppendText("Test file written event failed.", Color.Red);
            }


            if (isTestMonitorFileDeleted)
            {
                AppendText("Test file delete event passed.", Color.Green);
                AppendText("The test file was deleted.", Color.Gray);
            }
            else
            {
                AppendText("Test file delete event failed.", Color.Red);
            }
        }

        private void FolderProtectionUnitTest()
        {
            //
            //Test case2: Folder Protection Unit Test
            //All files in the test folder "c:\\EaseFilterUnitTest" will be readonly, except the current process, you can add more authorized processes here.   
            //All files in the test folder "c:\\EaseFilterUnitTest" can't be accessed by process "explorer.exe" and "cmd.exe", you can add more processes.
            //There are exception for the sub folder "excludeFolder", no any restriction to this folder.

            //pros: it can cover all files in the protected folder.

            //cons: the blocked processes don't have permission to access the files in the protected folder.
            //      it only can block the listed processes.

            string message = "Folder Protection Unit Test";
            AppendText(message, Color.Black);

            message = "1. The protected folder is read only to all processes except for the authorized processes." + Environment.NewLine;
            message +="2. Block the explorer to copy&paste the protected files." + Environment.NewLine;
            message +="3. Block the protected files file access from remote network." + Environment.NewLine;
            message +="4. Set up an exclude sub foler of the protected folder which excludes the security restrition." + Environment.NewLine;

            AppendText(message, Color.Gray);

            //Prevent copy&paste operation from explorer and dos prompt.
            FilterRule blockAccessFilterRule = new FilterRule();
            blockAccessFilterRule.IncludeFileFilterMask = unitTestFolder + "\\*";
            //blocked application list.
            blockAccessFilterRule.IncludeProcessNames = "explorer.exe;cmd.exe";
            blockAccessFilterRule.AccessFlag = (uint)((~FilterAPI.AccessFlag.ALLOW_READ_ACCESS)) & FilterAPI.ALLOW_MAX_RIGHT_ACCESS;
            blockAccessFilterRule.ExcludeFileFilterMasks = filterRuleExcludeTestFolder + "\\*";

            //set the accessFlag to readonly for all the files in the test folder except the current process.
            FilterRule readonlyFilterRule = new FilterRule();
            readonlyFilterRule.IncludeFileFilterMask = unitTestFolder + "*";
            readonlyFilterRule.ExcludeFileFilterMasks = filterRuleExcludeTestFolder + "\\*";
            readonlyFilterRule.AccessFlag = (uint)((~FilterAPI.AccessFlag.ALLOW_OPEN_WITH_WRITE_ACCESS) & (~FilterAPI.AccessFlag.ALLOW_OPEN_WITH_CREATE_OR_OVERWRITE_ACCESS)
                                                   & (~FilterAPI.AccessFlag.ALLOW_SET_INFORMATION) & (~FilterAPI.AccessFlag.ALLOW_SET_SECURITY_ACCESS) & (~FilterAPI.AccessFlag.ALLOW_FILE_ACCESS_FROM_NETWORK))
                                                   & FilterAPI.ALLOW_MAX_RIGHT_ACCESS;
            try
            {
                GlobalConfig.FilterRules.Clear();
                GlobalConfig.FilterRules.Add(blockAccessFilterRule.IncludeFileFilterMask, blockAccessFilterRule);
                GlobalConfig.FilterRules.Add(readonlyFilterRule.IncludeFileFilterMask, readonlyFilterRule);

                GlobalConfig.SendConfigSettingsToFilter();

               // The current process should have the read permission.
                try
                {
                    string textData = File.ReadAllText(unitTestFile);
                }
                catch (Exception ex)
                {
                    AppendText("Read file " + unitTestFile + " failed." + ex.Message, Color.Red);
                    return;
                }

                try
                {
                    string cmdExcludeFileName = filterRuleExcludeTestFolder + "\\cmd_copy.txt";

                    //for files in the exclude folder, anyone can read and write here.
                    File.Copy(unitTestFile, cmdExcludeFileName);

                    if (File.Exists(cmdExcludeFileName))
                    {
                        AppendText("Test with exclude folder passed.", Color.Green);
                        AppendText("Copy a protected test file to the exclude sub folder succeeded.", Color.Gray);
                    }
                    else
                    {
                        AppendText("Test with excludeFolder failed.", Color.Red);
                    }

                }
                catch (Exception ex)
                {
                    AppendText("Test with exclude folder failed." + ex.Message, Color.Red);
                    return;
                }

                try
                {
                    //The current process shouldn't have the write permission.
                    File.AppendAllText(unitTestFile, ",new test data was added.");

                    AppendText("Test with blocking write access failed.", Color.Red);
                    return;

                }
                catch
                {
                    AppendText("Test with blocking write access passed.", Color.Green);
                    AppendText("Write data to the protected file was blocked.", Color.Gray);
                }


                GlobalConfig.FilterRules.Remove(readonlyFilterRule.IncludeFileFilterMask);

                //here you can put your exclude process name which has the full permission to the test folder.
                readonlyFilterRule.ExcludeProcessIds = FilterAPI.GetCurrentProcessId().ToString();
                GlobalConfig.FilterRules.Add(readonlyFilterRule.IncludeFileFilterMask, readonlyFilterRule);

                GlobalConfig.SendConfigSettingsToFilter();

                try
                {
                    //The current process should have the write permission.
                    File.AppendAllText(unitTestFile, ",new test data was added.");

                    AppendText("Test with exclude process passed.", Color.Green);
                    AppendText("Write data to the protected file was passed with authorized process.", Color.Gray);
                }
                catch
                {
                    AppendText("Test with exclude process failed.", Color.Red);
                    return;
                }

                try
                {
                    //the cmd.exe should be blocked to access here
                    string cmdBlockFileName = unitTestFolder + "\\cmd_copy.txt";
                    Process.Start("cmd", "/C copy " + unitTestFile + "  " + cmdBlockFileName);

                    if (File.Exists(cmdBlockFileName))
                    {
                        AppendText("Test with include process cmd failed.", Color.Red);
                        return;
                    }
                    else
                    {
                        AppendText("Test with include process cmd passed.", Color.Green);
                        AppendText("Copy protected file from cmd.exe in dos prompt was blocked.", Color.Gray);
                    }
                }
                catch (Exception ex)
                {
                    AppendText("Test with include process cmd failed." + ex.Message, Color.Red);
                }


                //blockAccessFilterRule inlcude file mask is the subset of the include file mask of the readonlyFilterRule
                //the filter driver will check with the filter rule blockAccessFilterRule, if it doesn't match, then check with readonlyFilterRule.

                AppendText("Test with sub folder filter rule passed.", Color.Green);
                AppendText("Create two filter rules, the files inside the sub folder matched with sub folder filter rule.", Color.Gray);

            }
            catch (Exception ex)
            {
                AppendText("Test readonly folder failed." + ex.Message, Color.Red);
            }
          
        }

        private void AddProcessRightsUnitTest()
        {
            //
            //Test case3: Add Process Rights Unit Test
            //All files in the test folder "c:\\EaseFilterUnitTest" will be readonly, except the current process, you can add more authorized processes here.   

            //pros: You can set the specific access rights to different processes in the protected folder, all other processes will have the same access rights of the filter rule.

            string message = "Add Process Rights Unit Test";
            AppendText(message, Color.Black);

            message = "1. The protected folder is read only to all processes except for the authorized processes." + Environment.NewLine;
            AppendText(message, Color.Gray);

            //Prevent copy&paste operation from explorer and dos prompt.
            FilterRule blockAccessFilterRule = new FilterRule();
            blockAccessFilterRule.IncludeFileFilterMask = unitTestFolder + "\\*";
            blockAccessFilterRule.AccessFlag = (uint)FilterAPI.ALLOW_MAX_RIGHT_ACCESS; //default access rights to the folder.

            //Remove all access rights for the process "cmd".
            blockAccessFilterRule.ProcessRights = "cmd.exe!" + ((uint)FilterAPI.AccessFlag.LEAST_ACCESS_FLAG).ToString();

            try
            {
                GlobalConfig.FilterRules.Clear();
                GlobalConfig.FilterRules.Add(blockAccessFilterRule.IncludeFileFilterMask, blockAccessFilterRule);
                GlobalConfig.SendConfigSettingsToFilter();

                // The current process should have all permission.
                File.AppendAllText(unitTestFile, ",new test data was added.");
                string textData = File.ReadAllText(unitTestFile);

                //the cmd.exe should be blocked to access here
                string cmdBlockFileName = unitTestFolder + "\\cmd_copy.txt";
                Process.Start("cmd", "/C copy " + unitTestFile + "  " + cmdBlockFileName);

                if (File.Exists(cmdBlockFileName))
                {
                    AppendText("Test add process rights failed.", Color.Red);
                    return;
                }
                else
                {
                    AppendText("Test add process rights passed.", Color.Green);
                    AppendText("Remove process cmd.exe access rights succeeded.", Color.Gray);
                }
            }
            catch (Exception ex)
            {
                AppendText("Test add process rights failed." + ex.Message, Color.Red);
            }

        }

        private void AddUserRightsUnitTest()
        {
            //
            //Test case4: Add User Rights Unit Test
            //Set current user with readonly right access to folder "c:\\EaseFilterUnitTest".   

            //pros: You can set the specific access rights to different users in the protected folder, all other users will have the same access rights of the filter rule.

            string message = "Add User Rights Unit Test";
            AppendText(message, Color.Black);

            string userName = Environment.UserDomainName + "\\" + Environment.UserName;

            message = "1. The protected folder can't be acccessed by the current user:" + userName  + Environment.NewLine;
            AppendText(message, Color.Gray);

            //Prevent copy&paste operation from explorer and dos prompt.
            FilterRule blockAccessFilterRule = new FilterRule();
            blockAccessFilterRule.IncludeFileFilterMask = unitTestFolder + "\\*";
            blockAccessFilterRule.AccessFlag = (uint)FilterAPI.ALLOW_MAX_RIGHT_ACCESS; //default access rights to the folder.

            //Remove all access rights to the current user.
            blockAccessFilterRule.UserRights = userName + ":" + ((uint)FilterAPI.AccessFlag.LEAST_ACCESS_FLAG).ToString();

            try
            {
                GlobalConfig.FilterRules.Clear();
                GlobalConfig.FilterRules.Add(blockAccessFilterRule.IncludeFileFilterMask, blockAccessFilterRule);
                GlobalConfig.SendConfigSettingsToFilter();

                try
                {
                    // The current process doesnt have any permission.
                    File.AppendAllText(unitTestFile, ",new test data was added.");
                    AppendText("Test add user rights failed.", Color.Red);
                }
                catch
                {
                    AppendText("Test add user rights passed.", Color.Green);
                    AppendText("Remove current user access rights succeeded.", Color.Gray);
                    return;
                }

            }
            catch (Exception ex)
            {
                AppendText("Test add user rights failed." + ex.Message, Color.Red);
            }

        }

        private void NoRenameAndCopyUnitTest()
        {


            //
            //Test case5: prevent specific files in protected folder from being copied out 
            //All files with protected extension ".prt" only can store in protected folder "c:\\EaseFilterUnitTest".   
            //
            //pros: the simplest way to prevent the protected files from being copied out.
            //cons: it only can block the specific files.     

            GlobalConfig.FilterRules.Clear();

            string message = "Protect Files With Extension '.prt'";
            AppendText(message, Color.Black);

            message =  "1. Prevent the protected files from renaming and deleting." + Environment.NewLine;
            message += "2. Prevent the protected files from copying out of the protected folder." + Environment.NewLine;
            message += "3. Set up an exclude sub foler of the protected folder which excludes the security restrition." + Environment.NewLine;

            AppendText(message, Color.Gray);

            //not allow file rename or delete and copy out of the folder
            FilterRule noRenameFilterRule = new FilterRule();
            noRenameFilterRule.IncludeFileFilterMask = unitTestFolder + "*.prt";
            noRenameFilterRule.AccessFlag = (uint)((~FilterAPI.AccessFlag.ALLOW_FILE_RENAME)
                & (~FilterAPI.AccessFlag.ALLOW_FILE_DELETE) 
                & (~FilterAPI.AccessFlag.ALLOW_COPY_PROTECTED_FILES_OUT)) 
                & FilterAPI.ALLOW_MAX_RIGHT_ACCESS;

            FilterRule globalExcludeFilterRule = new FilterRule();
            globalExcludeFilterRule.IncludeFileFilterMask = filterRuleExcludeTestFolder + "\\*";

            try
            {

                GlobalConfig.FilterRules.Add(noRenameFilterRule.IncludeFileFilterMask, noRenameFilterRule);
                GlobalConfig.FilterRules.Add(globalExcludeFilterRule.IncludeFileFilterMask, globalExcludeFilterRule);

                GlobalConfig.SendConfigSettingsToFilter();


                string protectedFileName = unitTestFolder + "\\protectFile.prt";
                File.AppendAllText(protectedFileName, "This is protected file with .prt extension.");

                //file rename is blocked 
                try
                {
                    File.Move(protectedFileName, unitTestFolder + "\\test.txt");
                    AppendText("Test with file rename prevention failed.", Color.Red);
                    return;
                }
                catch
                {
                    AppendText("Test with file rename prevention passed.", Color.Green);
                    AppendText("Block the protected file being renamed.", Color.Gray);
                }

                //file delete is blocked 
                try
                {
                    File.Delete(protectedFileName);
                    AppendText("Test with file delete prevention failed.", Color.Red);
                    return;
                }
                catch
                {
                    AppendText("Test with file delete prevention passed.", Color.Green);
                    AppendText("Block the protected file being deleted.", Color.Gray);
                }

                //exclude folder file deletion is allowed
                try
                {
                    File.Delete(globalExcludeFilterRuleTestFile);
                    AppendText("Test global exclude filter rule passed.", Color.Green);
                    AppendText("Delete the file from the exclude folder succeeded.", Color.Gray);
                }
                catch (Exception ex)
                {
                    AppendText("Test global exclude filter rule failed." + ex.Message, Color.Red);
                    return;
                }

                //protected file copy out to the protect folder is blocked 
                try
                {
                    File.Copy(protectedFileName, "c:\\test.txt");
                    AppendText("Test with file copying out prevention failed.", Color.Red);
                    return;
                }
                catch
                {
                    AppendText("Test with file copying out prevention passed.", Color.Green);
                    AppendText("Copy the protected file outside of the protected folder is blocked.", Color.Gray);
                }
            }
            catch (Exception ex)
            {
                AppendText("NoRenameAndCopyUnitTest failed." + ex.Message, Color.Red);
                return;
            }
           
        }

        private void FolderLockerUnitTest()
        {

            //
            //Test case6: Folder Locker
            //1. Transparently encrypt/decrpt files files in the test folder "c:\\EaseFilterUnitTest".   
            //2. All files are hidden in the test folder "c:\\EaseFilterUnitTest".
            //3. Add/Remove the exception application which will be excluded from the restriction.

            string message = "Folder Locker Unit Test";
            AppendText(message, Color.Black);

            message =  " 1. Encrypt files transparently in the locked folder." + Environment.NewLine;
            message += " 2. Hide protected files to unauthorized processes." + Environment.NewLine;
            message += " 3. Allow authorized processes to read encrypted files." + Environment.NewLine;

            AppendText(message, Color.Gray);

            //the authorized processes in the include process list.
            FilterRule AuthorizedProcessFilterRule = new FilterRule();
            AuthorizedProcessFilterRule.IncludeProcessIds = FilterAPI.GetCurrentProcessId().ToString();
            AuthorizedProcessFilterRule.IncludeFileFilterMask = unitTestFolder + "\\*";
            AuthorizedProcessFilterRule.AccessFlag = (uint)FilterAPI.AccessFlag.ENABLE_FILE_ENCRYPTION_RULE | FilterAPI.ALLOW_MAX_RIGHT_ACCESS;
            AuthorizedProcessFilterRule.EncryptionPassPhrase = "password";//this password must be the same as the previous one.

            //all other processes can't read the files, can't see the files.
            FilterRule folderLockerFilterRule = new FilterRule();
            folderLockerFilterRule.IncludeFileFilterMask = unitTestFolder + "*";
            folderLockerFilterRule.AccessFlag = ((uint)FilterAPI.AccessFlag.ENABLE_FILE_ENCRYPTION_RULE | (uint)FilterAPI.AccessFlag.ENABLE_HIDE_FILES_IN_DIRECTORY_BROWSING)
                                                |((uint)(~FilterAPI.AccessFlag.ALLOW_READ_ACCESS) & FilterAPI.ALLOW_MAX_RIGHT_ACCESS);
            folderLockerFilterRule.EncryptionPassPhrase = "password";
            folderLockerFilterRule.HiddenFileFilterMasks = "*";

            try
            {
                GlobalConfig.FilterRules.Clear();
                GlobalConfig.FilterRules.Add(folderLockerFilterRule.IncludeFileFilterMask, folderLockerFilterRule);
                GlobalConfig.SendConfigSettingsToFilter();

                string folderLockerTestFileName = unitTestFolder + "\\folderLockerTestFile.txt";
                string testContent = "This is folderLockerTestFileName FileName test file.";

                File.AppendAllText(folderLockerTestFileName, testContent);

                string[] txtFileList = Directory.GetFiles(unitTestFolder, "*.txt");
                if (txtFileList.Length == 0)
                {
                    //All .txt file name should be hidden, the file list should be empty.
                    AppendText("Test hidden file list passed.", Color.Green);
                    AppendText("Hide the protected files from directory browsing.", Color.Gray);
                }
                else
                {
                    AppendText("Test hidden file list failed.", Color.Red);
                    return;
                }

                try
                {
                    //read the encrypted file was blocked.
                    string readContent = File.ReadAllText(folderLockerTestFileName);
                    AppendText("Test block read access failed.", Color.Red);
                    return;

                }
                catch
                {
                    AppendText("Test block read access passed.", Color.Green);
                    AppendText("Block the protected file reading from unauthorized processes.", Color.Gray);
                }


                GlobalConfig.FilterRules.Add(AuthorizedProcessFilterRule.IncludeFileFilterMask, AuthorizedProcessFilterRule);
                GlobalConfig.SendConfigSettingsToFilter();                

                //the current process was added with exception filter rule.
                txtFileList = Directory.GetFiles(unitTestFolder, "*.txt");
                if (txtFileList.Length > 0)
                {
                    //All .txt file name should be listed now for the current process.
                    AppendText("Test authorized processes filter rule passed.", Color.Green);

                    AppendText("Add authorized processes which can get the file list.", Color.Gray);
                }
                else
                {
                    AppendText("Test  authorized processes  filter rule failed.", Color.Red);
                    return;
                }

                try
                {
                    IntPtr fileHandle = IntPtr.Zero;
                    string lastError = string.Empty;

                    if (FilterAPI.OpenRawEnCyptedFile(folderLockerTestFileName, out fileHandle, out lastError))
                    {
                        SafeFileHandle shFile = new SafeFileHandle(fileHandle, true);
                        FileStream fs = new FileStream(shFile, FileAccess.Read);
                        byte[] buffer = new byte[1024];
                        int len = fs.Read(buffer, 0, 1024);

                        string encryptedText = Encoding.Unicode.GetString(buffer);
                        encryptedText = encryptedText.TrimEnd((char)0);

                        fs.Close();

                        if (string.Compare(testContent, encryptedText, false) != 0)
                        {
                            AppendText("Test folder locker transparent file encryption passed.", Color.Green);
                            AppendText("Protected file's data was encrypted.", Color.Gray);
                        }
                        else
                        {
                            AppendText("Test folder locker transparent file encryption failed.", Color.Red);
                            return;
                        }

                    }
                    else
                    {
                        AppendText("Test folder locker transparent file encryption failed.OpenRawEnCyptedFile failed:" + lastError, Color.Red);
                        return;
                    }

                    string readContent = File.ReadAllText(folderLockerTestFileName);
                    if (string.Compare(testContent, readContent, false) == 0)
                    {
                        AppendText("Test folder locker transparent file decryption passed.", Color.Green);
                        AppendText("Protected file's data was decrypted.", Color.Gray);
                    }
                    else
                    {
                        AppendText("Test folder locker transparent file decryption failed.", Color.Red);
                        return;
                    }
                }
                catch 
                {
                    AppendText("Test  folder locker failed.", Color.Red);
                    return;
                }

            }
            catch 
            {
                AppendText("Test folder locker failed." , Color.Red);
            }
         
        }

        private void FileAccessControlUnitTest()
        {

            //
            //Test case7: Control the file IO with callback function.
            // 1. You will get the notification before the file was opened,renamed,deleted. 
            // 2. In callback funtion, you can decide if proceed the IO operation based on the information
            //    (user name,process name,file information, file content). 
            // 3. For the test file, it will be blocked to create new file,rename file, delete file.

            string message = "File Access Control With Callback Function Unit Test";
            AppendText(message, Color.Black);

            message =  " 1. Register the PRE_CREATE and PRE_SET_INFORMATION." + Environment.NewLine;
            message += " 2. The callback function will get the notification before the protected file was opened or file information was changed." + Environment.NewLine;
            message += " 3. Block the new file creation, file rename and file deletion in callback function." + Environment.NewLine;

            AppendText(message, Color.Gray);

            FilterRule callbackControlFilterRule = new FilterRule();
            callbackControlFilterRule.IncludeFileFilterMask = unitTestCallbackFolder + "\\*";
            callbackControlFilterRule.AccessFlag = FilterAPI.ALLOW_MAX_RIGHT_ACCESS;
            callbackControlFilterRule.ControlIO = (uint)(FilterAPI.MessageType.PRE_CREATE | FilterAPI.MessageType.PRE_SET_INFORMATION);

            try
            {
                GlobalConfig.FilterRules.Clear();
                GlobalConfig.FilterRules.Add(callbackControlFilterRule.IncludeFileFilterMask, callbackControlFilterRule);
                GlobalConfig.SendConfigSettingsToFilter();

                try
                {
                    File.Delete(unitTestCallbackFile);
                    AppendText("Test block file deletion in callback failed.", Color.Red);
                    return;
                }
                catch
                {
                    AppendText("Test block file deletion in callback  passed.", Color.Green);
                    AppendText("Delete protected file was blocked by callback function.", Color.Gray);
                }

                try
                {
                    string blockRenameFileName = unitTestFolder + "\\blockRenameFile.txt";
                    File.Move(unitTestCallbackFile, blockRenameFileName);
                    AppendText("Test block file rename in callback failed.", Color.Red);
                    return;
                }
                catch
                {
                    AppendText("Test block file rename in callback  passed.", Color.Green);
                    AppendText("Rename protected file was blocked by callback function.", Color.Gray);
                }

                try
                {
                    FileStream fs = new FileStream(unitTestCallbackBlockNewFile, FileMode.CreateNew);
                    fs.Close();

                    AppendText("Test block new file creation in callback failed.", Color.Red);
                    return;
                }
                catch
                {
                    AppendText("Test block the new file creation in callback passed.", Color.Green);
                    AppendText("Create new file was blocked by callback function.", Color.Gray);
                }

            }
            catch (Exception ex)
            {
                AppendText("Test File Access Control failed." + ex.Message, Color.Red);
            }
            finally
            {
                GlobalConfig.FilterRules.Remove(callbackControlFilterRule.IncludeFileFilterMask);
                GlobalConfig.SendConfigSettingsToFilter();
            }
        }

        private void CopyFileBeforeDeletionUnitTest()
        {

            //
            //Test case8: copy file before it was deleted 
            //Make a copy of the file before it was deleted.   

            string message = "Copy Protected File Before File Deletion Unit Test";
            AppendText(message, Color.Black);

            message = " Make a copy of the file before it was deleted." + Environment.NewLine;
            AppendText(message, Color.Gray);

            FilterRule copyFileBeforeDeletionlFilterRule = new FilterRule();
            copyFileBeforeDeletionlFilterRule.IncludeFileFilterMask = unitTestCallbackFolder + "\\*";
            copyFileBeforeDeletionlFilterRule.AccessFlag = FilterAPI.ALLOW_MAX_RIGHT_ACCESS;
            copyFileBeforeDeletionlFilterRule.ControlIO = (uint)(FilterAPI.MessageType.PRE_CREATE | FilterAPI.MessageType.PRE_SET_INFORMATION);

            try
            {
                GlobalConfig.FilterRules.Clear();
                GlobalConfig.FilterRules.Add(copyFileBeforeDeletionlFilterRule.IncludeFileFilterMask, copyFileBeforeDeletionlFilterRule);
                GlobalConfig.SendConfigSettingsToFilter();

                try
                {
                   File.Delete(unitTestCopyBeforeDeleteCallbackFile);

                    //if (File.Exists(unitTestCopyAfterDeleteCallbackFile))
                    //{
                    //    AppendText("Test copy file befoe deletion callback passed.", Color.Green);
                    //    AppendText("The protected file was copied.", Color.Gray);
                    //    AppendText("The protected file was deleted.", Color.Gray);
                    //}
                    //else
                    //{
                    //    AppendText("Test copy file befoe deletion callback failed.", Color.Red);
                    //    return;
                    //}

                }
                catch (Exception ex)
                {
                    AppendText("Test copy file befoe deletion callback exception:" + ex.Message, Color.Red);
                    return;
                }

            }
            catch (Exception ex)
            {
                AppendText("Test copy file befoe deletion callback exception:" + ex.Message, Color.Red);
            }         
        }

      

        private void FileProtectorUnitTest_Activated(object sender, EventArgs e)
        {
            if (!isUnitTestCompleted)
            {
                StartFilterUnitTest();
                isUnitTestCompleted = true;
            }
        }

    }
}
