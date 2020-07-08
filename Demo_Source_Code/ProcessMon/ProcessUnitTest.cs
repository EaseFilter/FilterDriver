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
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

using EaseFilter.CommonObjects;

namespace ProcessMon
{
    static public class ProcessUnitTest
    {
        public static RichTextBox unitTestResult = new RichTextBox();
        public static bool newProcessCreationNotification = false;
        public static bool monitorIONotification = false;
        public static bool controlIONotification = false;

        static private void AppendUnitTestResult(string text, Color color)
        {
            if (color == Color.Black)
            {
                unitTestResult.AppendText(Environment.NewLine);
                unitTestResult.SelectionFont = new Font("Arial", 12, FontStyle.Bold);
            }

            unitTestResult.SelectionStart = unitTestResult.TextLength;
            unitTestResult.SelectionLength = 0;

            unitTestResult.SelectionColor = color;
            unitTestResult.AppendText(text + Environment.NewLine);
            unitTestResult.SelectionColor = unitTestResult.ForeColor;

            if (color == Color.Black)
            {
                unitTestResult.AppendText(Environment.NewLine);
            }


        }

        private static void DenyNewProcessTest()
        {
            try
            {
                FilterAPI.ResetConfigData();

                //
                //Process control flag test,deny new process creation.
                //
                uint controlFlag = (uint)FilterAPI.ProcessControlFlag.DENY_NEW_PROCESS_CREATION;

                //start new process should be fine
                string processName = "cmd.exe";
                var proc = System.Diagnostics.Process.Start(processName);
                proc.Kill();

                if (!FilterAPI.AddProcessFilterRule((uint)processName.Length * 2, processName, controlFlag, 0))
                {
                    AppendUnitTestResult("AddProcessFilterRule failed:" + FilterAPI.GetLastErrorMessage(), Color.Red);
                    return;

                }

                try
                {
                    proc = System.Diagnostics.Process.Start(processName);
                    proc.Kill();
                    AppendUnitTestResult("DENY_NEW_PROCESS_CREATION control flag test failed, denied process launch was failed.", Color.Red);
                }
                catch (Exception ex)
                {
                    AppendUnitTestResult("DENY_NEW_PROCESS_CREATION control flag test passed.", Color.Green);
                }

            }
            catch (Exception ex)
            {
                AppendUnitTestResult("DENY_NEW_PROCESS_CREATION failed, return error:" + ex.Message, Color.Red);
            }
           
        }

        private static void NewProcessCallbackTest()
        {
            try
            {
                FilterAPI.ResetConfigData();

                //
                //Process control flag test, new process creation notification.
                //
                string processName = "cmd.exe";
                uint controlFlag = (uint)FilterAPI.ProcessControlFlag.PROCESS_CREATION_NOTIFICATION;
                if (!FilterAPI.AddProcessFilterRule((uint)processName.Length * 2, processName, controlFlag, 0))
                {
                    AppendUnitTestResult("AddProcessFilterRule failed:" + FilterAPI.GetLastErrorMessage(), Color.Red);
                    return;

                }

                try
                {
                    //start new process should be fine
                    var proc = System.Diagnostics.Process.Start(processName);
                    proc.Kill();

                    Thread.Sleep(1000);

                    if (newProcessCreationNotification)
                    {
                        AppendUnitTestResult("PROCESS_CREATION_NOTIFICATION control flag test passed.", Color.Green);
                    }
                    else
                    {
                        AppendUnitTestResult("PROCESS_CREATION_NOTIFICATION test failed, didn't receive the new process creation notification.", Color.Red);
                    }
                }
                catch (Exception ex)
                {
                    AppendUnitTestResult("PROCESS_CREATION_NOTIFICATION failed," + ex.Message, Color.Red);
                }

            }
            catch (Exception ex)
            {
                AppendUnitTestResult("PROCESS_CREATION_NOTIFICATION failed," + ex.Message, Color.Red);
            }

           
        }

        private static void ProcessFileControlTest()
        {

            try
            {
                FilterAPI.ResetConfigData();

                //
                //File access control test for process, to test block the new file creation for current process
                //
                string fileName = "test.txt";
                uint currentPid = FilterAPI.GetCurrentProcessId();
                uint accessFlag = FilterAPI.ALLOW_MAX_RIGHT_ACCESS & (uint)~FilterAPI.AccessFlag.ALLOW_OPEN_WITH_CREATE_OR_OVERWRITE_ACCESS;
                if (!FilterAPI.AddFileControlToProcessById(currentPid, 2, "*", accessFlag))
                {
                    AppendUnitTestResult("AddFileControlToProcessById failed:" + FilterAPI.GetLastErrorMessage(), Color.Red);
                    return;
                }

                try
                {
                    File.AppendAllText(fileName, "This is test file content");
                    AppendUnitTestResult("File access control test for current process failed, can't block the new creation.", Color.Red);
                }
                catch (Exception ex)
                {
                    AppendUnitTestResult("File access control flag ALLOW_OPEN_WITH_CREATE_OR_OVERWRITE_ACCESS test for current process passed", Color.Green);
                }

            }
            catch (Exception ex)
            {
                AppendUnitTestResult("File access control test for current process failed," + ex.Message, Color.Red);
            }

            
        }

        private static void ProcessFileIOCallbackTest()
        {
            try
            {
                FilterAPI.ResetConfigData();

                //
                //monitor and control IO callback notification test for current process
                //
                string fileName = GlobalConfig.AssemblyPath + "\\test.txt";
                uint currentPid = FilterAPI.GetCurrentProcessId();
                uint accessFlag = FilterAPI.ALLOW_MAX_RIGHT_ACCESS;
                string currentProcessName = GlobalConfig.AssemblyName;

                if (!FilterAPI.AddFileControlToProcessByName((uint)currentProcessName.Length * 2, currentProcessName, (uint)fileName.Length * 2, fileName, accessFlag))
                {
                    AppendUnitTestResult("AddFileControlToProcessByName failed:" + FilterAPI.GetLastErrorMessage(), Color.Red);
                    return;
                }

                if (!FilterAPI.AddFileCallbackIOToProcessByName((uint)currentProcessName.Length * 2, currentProcessName, (uint)fileName.Length * 2, fileName,
                 (uint)FilterAPI.MessageType.POST_CREATE, (uint)FilterAPI.MessageType.PRE_CREATE, 0, 0, 0))
                {
                    AppendUnitTestResult("AddFileControlToProcessByName failed:" + FilterAPI.GetLastErrorMessage(), Color.Red);
                    return;
                }

                try
                {
                    File.AppendAllText(fileName, "This is test file content");
                    Thread.Sleep(2000);

                    if (monitorIONotification)
                    {
                        AppendUnitTestResult("Register monitor IO callback for current process test passed.", Color.Green);
                    }
                    else
                    {
                        AppendUnitTestResult("File monitor IO callback test failed, no monitor callback.", Color.Red);
                    }

                    if (controlIONotification)
                    {
                        AppendUnitTestResult("Register control IO callback for current process test passed.", Color.Green);
                    }
                    else
                    {
                        AppendUnitTestResult("File control IO callback test failed, no monitor callback.", Color.Red);
                    }

                }
                catch (Exception ex)
                {
                    AppendUnitTestResult("Register monitor/control IO callback for current process failed." + ex.Message, Color.Red);
                }

            }
            catch (Exception ex)
            {
                AppendUnitTestResult("File access control test for current process failed," + ex.Message, Color.Red);
           
            }

            
        }

        public static void ProcessFilterUnitTest(RichTextBox richTextBox_TestResult)
        {

            string lastError = string.Empty;
            string userName = Environment.UserDomainName + "\\" + Environment.UserName;

            unitTestResult = richTextBox_TestResult;


            string message = "Process Filter Driver Unit Test.";
            AppendUnitTestResult(message, Color.Black);

            DenyNewProcessTest();

            NewProcessCallbackTest();

            ProcessFileControlTest();

            ProcessFileIOCallbackTest();
        }
    }
}
