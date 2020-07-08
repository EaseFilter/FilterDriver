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

namespace FileMonitor
{
    public partial class MonitorDemo : Form
    {

        bool isUnitTestCompleted = false;

        /// <summary>
        ///  To manage your files, you need to create at least one filter rule, you can have multiple filter rules. 
        ///  A filter rule only can have one unique include file mask,
        ///  A filter rule can have multiple exclude file masks, multiple include process names and exclude process names, 
        ///  multiple include process Ids and exclude process Ids, multiple include user names and exclude user names. 
        /// </summary>


        /// <summary>
        /// Test monitor feature with registering the IO events, get notification after the file was closed.
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

        /// <summary>
        /// Test monitor IO registration,get notification for the registered IO.
        /// </summary>
        private static bool isPostCreateNotificationTriggered = false;
        private static bool isPostSetInformationNotificationTriggered = false;

        /// <summary>
        /// Unit test folder
        /// </summary>
        private static string unitTestFolder = "c:\\EaseFilterUnitTest";

        /// <summary>
        /// Test Control IO feature with callback function
        /// </summary>
        private static string unitTestCallbackFolder = "c:\\EaseFilterUnitTest\\callbackFolder";
        private static string unitTestCallbackFile = "c:\\EaseFilterUnitTest\\callbackFolder\\unitTestFile.txt";
       
      
        public MonitorDemo()
        {
            InitializeComponent();           
           
        }


        private static bool FileIOEventHandler(FilterAPI.MessageSendData messageSend)
        {
            uint messageType = messageSend.MessageType;
            uint infoClass = messageSend.InfoClass;
            string fileName = messageSend.FileName;

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

                if (fileName.ToLower().StartsWith(unitTestMonitorTestFolder.ToLower()))
                {
                    return FileIOEventHandler(messageSend);
                }

                if (!fileName.StartsWith(unitTestCallbackFile.ToLower()))
                {
                    return true;
                }

                FilterAPI.MessageType messageType = (FilterAPI.MessageType)messageSend.MessageType;
                WinData.FileInfomationClass infoClass = (WinData.FileInfomationClass)messageSend.InfoClass;

                if (messageType == FilterAPI.MessageType.POST_CREATE)
                {
                    isPostCreateNotificationTriggered = true;
                }
                else if (messageType == FilterAPI.MessageType.POST_SET_INFORMATION)
                {
                    isPostSetInformationNotificationTriggered = true;
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
           
            if (!Directory.Exists(unitTestMonitorTestFolder))
            {
                Directory.CreateDirectory(unitTestMonitorTestFolder);
            }

            if (!Directory.Exists(unitTestCallbackFolder))
            {
                Directory.CreateDirectory(unitTestCallbackFolder);
            }

            if (!File.Exists(unitTestCallbackFile))
            {
                File.AppendAllText(unitTestCallbackFile, "This is unitTestCallbackFile test file.");
            }

        }

        private void CleanupTest(string folder)
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

     

        public void StartFilterUnitTest()
        {
            try
            {
                CleanupTest(unitTestFolder);

                InitializeUnitTest();

                FileEventMonitorTest();

                MonitorIORegistrationUnitTest();

                //sleep 4 seconds to wait for the callback events ready.
                System.Threading.Thread.Sleep(4000);

                ShowFileEventMonitorTestResult();


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
            finally
            {

                GlobalConfig.FilterRules.Remove(monitorFilterRule.IncludeFileFilterMask);
                GlobalConfig.SendConfigSettingsToFilter();
            }


        }

        private void ShowFileEventMonitorTestResult()
        {

            /// print out the monitor IO events test result.
            string message = "File IO Events Unit Test.";
            AppendText(message,Color.Black);
            message = "Make sure your monitor filter is enabled. Register the monitor file IO events: create,rename,delete,written,file info changed, read, security change in the test folder." + Environment.NewLine;
            AppendText(message, Color.Gray);

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


        private void MonitorIORegistrationUnitTest()
        {

            string message = "Monitor IO Registration Unit Test";
            AppendText(message, Color.Black);

            message = " 1. Register the POST_CREATE and POST_SET_INFORMATION." + Environment.NewLine;
            message += " 2. The callback function will get the notification after the file was opened or file information was changed." + Environment.NewLine;

            AppendText(message, Color.Gray);

            FilterRule callbackControlFilterRule = new FilterRule();
            callbackControlFilterRule.IncludeFileFilterMask = unitTestCallbackFolder + "\\*";
            callbackControlFilterRule.AccessFlag = FilterAPI.ALLOW_MAX_RIGHT_ACCESS;
            callbackControlFilterRule.MonitorIO = (uint)(FilterAPI.MessageType.POST_CREATE | FilterAPI.MessageType.POST_SET_INFORMATION);

            try
            {

                GlobalConfig.FilterRules.Add(callbackControlFilterRule.IncludeFileFilterMask, callbackControlFilterRule);
                GlobalConfig.SendConfigSettingsToFilter();

                File.ReadAllText(unitTestCallbackFile);

                File.SetAttributes(unitTestCallbackFile, FileAttributes.Normal);

            }
            catch (Exception ex)
            {
                AppendText("Monitor IO Registration Unit Test failed." + ex.Message, Color.Red);
            }
            finally
            {
                GlobalConfig.FilterRules.Remove(callbackControlFilterRule.IncludeFileFilterMask);
                GlobalConfig.SendConfigSettingsToFilter();
            }

            //wait for the test result.
            System.Threading.Thread.Sleep(2000);

            if (isPostCreateNotificationTriggered)
            {
                AppendText("Test POST_CREATE IO registration passed.", Color.Green);
                AppendText("The POST_CREATE notification was triggered.", Color.Gray);
            }
            else
            {
                AppendText("Test POST_CREATE IO registration failed.", Color.Red);
            }


            if (isPostSetInformationNotificationTriggered)
            {
                AppendText("Test POST_SET_INFORMATION IO registration passed.", Color.Green);
                AppendText("The POST_SET_INFORMATION notification was triggered.", Color.Gray);
            }
            else
            {
                AppendText("Test POST_SET_INFORMATION IO registration failed.", Color.Red);
            }
        }       


        private void MonitorDemo_Activated(object sender, EventArgs e)
        {
            if (!isUnitTestCompleted)
            {
                StartFilterUnitTest();
                isUnitTestCompleted = true;
            }
        }
    }
}
