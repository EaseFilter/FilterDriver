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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;

using EaseFilter.CommonObjects;

namespace ProcessMon
{
    public partial class ProcessMon : Form
    {
        //Purchase a license key with the link: http://www.easefilter.com/Order.htm
        //Email us to request a trial key: info@easefilter.com //free email is not accepted.
        string registerKey = GlobalConfig.registerKey;

        ProcessInfoMessage processMessage = null;

        public ProcessMon()
        {
            GlobalConfig.filterType = FilterAPI.FilterType.FILE_SYSTEM_PROCESS|FilterAPI.FilterType.FILE_SYSTEM_CONTROL|FilterAPI.FilterType.FILE_SYSTEM_MONITOR;
            InitializeComponent();

            StartPosition = FormStartPosition.CenterScreen;
            DisplayVersion();

            InitListView();

            processMessage = new ProcessInfoMessage(listView_Info);

        }

        ~ProcessMon()
        {
            FilterAPI.StopFilter();
            GlobalConfig.Stop();
        }

        private void DisplayVersion()
        {
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            try
            {
                string filterDllPath = Path.Combine(GlobalConfig.AssemblyPath, "FilterAPI.Dll");
                version = FileVersionInfo.GetVersionInfo(filterDllPath).ProductVersion;
            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(43, "LoadFilterAPI Dll", EventLevel.Error, "FilterAPI.dll can't be found." + ex.Message);
            }

            this.Text += "    Version:  " + version;
        }



        public void InitListView()
        {
            listView_Info.Clear();		//clear control
            //create column header for ListView
            listView_Info.Columns.Add("#", 40, System.Windows.Forms.HorizontalAlignment.Left);
            listView_Info.Columns.Add("MessageType", 160, System.Windows.Forms.HorizontalAlignment.Left);
            listView_Info.Columns.Add("UserName", 150, System.Windows.Forms.HorizontalAlignment.Left);
            listView_Info.Columns.Add("ImageName(PID)",200, System.Windows.Forms.HorizontalAlignment.Left);
            listView_Info.Columns.Add("ThreadId", 60, System.Windows.Forms.HorizontalAlignment.Left);            
            listView_Info.Columns.Add("Description", 600, System.Windows.Forms.HorizontalAlignment.Left);
        }

   

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
             MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
            ProcessFilterSettingForm settingForm = new ProcessFilterSettingForm();
            settingForm.ShowDialog();
        }

        private void toolStripButton_StartFilter_Click(object sender, EventArgs e)
        {
            try
            {
                string lastError = string.Empty;

                bool ret = FilterAPI.StartFilter((int)GlobalConfig.FilterConnectionThreads
                                            , registerKey
                                            , new FilterAPI.FilterDelegate(FilterCallback)
                                            , new FilterAPI.DisconnectDelegate(DisconnectCallback)
                                            , ref lastError);
                if (!ret)
                {
                    MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                    MessageBox.Show("Start filter failed." + lastError);
                    return;
                }

                if (GlobalConfig.ProcessFilterRules.Count == 0 && null != sender)
                {
                    ProcessFilterRule defaultProcessFilterRule = new ProcessFilterRule();
                    defaultProcessFilterRule.ProcessNameFilterMask = "*";
                    defaultProcessFilterRule.ControlFlag = 16128;

                    GlobalConfig.AddProcessFilterRule(defaultProcessFilterRule);

                    MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                    MessageBox.Show("You didn't setup any filtere rule, by defult it will monitor all process/thread operations.");
                }


                toolStripButton_StartFilter.Enabled = false;
                toolStripButton_Stop.Enabled = true;

                GlobalConfig.SendConfigSettingsToFilter();

                EventManager.WriteMessage(102, "StartFilter", EventLevel.Information, "Start filter service succeeded.");
            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(104, "StartFilter", EventLevel.Error, "Start filter service failed with error " + ex.Message);
            }
        }

        Boolean FilterCallback(IntPtr sendDataPtr, IntPtr replyDataPtr)
        {
            Boolean ret = true;

            try
            {
                
               //for file access right unit test, to handle the monitor or control IO callback, reference the FileMonitor/FileProtector project
               FilterAPI.MessageSendData messageSend = (FilterAPI.MessageSendData)Marshal.PtrToStructure(sendDataPtr, typeof(FilterAPI.MessageSendData));
               if (messageSend.MessageType == (uint)FilterAPI.MessageType.PRE_CREATE)
               {
                   ProcessUnitTest.controlIONotification = true;
                  
                   FilterAPI.MessageReplyData messageReply = (FilterAPI.MessageReplyData)Marshal.PtrToStructure(replyDataPtr, typeof(FilterAPI.MessageReplyData));

                   //if you want to block the file access, you can return below status.
                   //messageReply.ReturnStatus = (uint)FilterAPI.NTSTATUS.STATUS_ACCESS_DENIED;
                   //messageReply.FilterStatus = (uint)FilterAPI.FilterStatus.FILTER_COMPLETE_PRE_OPERATION;

                   messageReply.ReturnStatus = (uint)FilterAPI.NTSTATUS.STATUS_SUCCESS;
                   Marshal.StructureToPtr(messageReply, replyDataPtr, true);

                   return true;
               }

               if (messageSend.MessageType == (uint)FilterAPI.MessageType.POST_CREATE)
               {
                   ProcessUnitTest.monitorIONotification = true;
                   return true;
               }

               FilterAPI.PROCESS_INFO processInfo = (FilterAPI.PROCESS_INFO)Marshal.PtrToStructure(sendDataPtr, typeof(FilterAPI.PROCESS_INFO));
               if (FilterAPI.MESSAGE_SEND_VERIFICATION_NUMBER != processInfo.VerificationNumber)
               {
                   MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                   MessageBox.Show("Received message corrupted.Please check if the PROCESS_INFO structure is correct.");

                   EventManager.WriteMessage(139, "FilterCallback", EventLevel.Error, "Received message corrupted.Please check if the PROCESS_INFO structure is correct.");
                   return false;
               }

                processMessage.AddMessage(processInfo);

                if (replyDataPtr.ToInt64() != 0)
                {
                    FilterAPI.MessageReplyData messageReply = (FilterAPI.MessageReplyData)Marshal.PtrToStructure(replyDataPtr, typeof(FilterAPI.MessageReplyData));

                    if (processInfo.MessageType == (uint)FilterAPI.FilterCommand.FILTER_SEND_PROCESS_CREATION_INFO)
                    {
                        //this is new process creation, you can block it here by returning the STATUS_ACCESS_DENIED.
                        messageReply.ReturnStatus = processInfo.Status;
                        Marshal.StructureToPtr(messageReply, replyDataPtr, true);
                    }
                }

                return ret;
            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(134, "FilterCallback", EventLevel.Error, "filter callback exception." + ex.Message);
                return false;
            }

        }

        void DisconnectCallback()
        {
            EventManager.WriteMessage(190, "DisconnectCallback", EventLevel.Information, "Filter Disconnected." + FilterAPI.GetLastErrorMessage());
        }

        private void ProcessMon_FormClosing(object sender, FormClosingEventArgs e)
        {
            FilterAPI.ResetConfigData();
            FilterAPI.StopFilter();
            GlobalConfig.Stop();
            Application.Exit();
        }

        private void toolStripButton_Stop_Click(object sender, EventArgs e)
        {
            FilterAPI.ResetConfigData();
            FilterAPI.StopFilter();

            toolStripButton_StartFilter.Enabled = true;
            toolStripButton_Stop.Enabled = false;
        }

        private void toolStripButton_ClearMessage_Click(object sender, EventArgs e)
        {
            InitListView();
        }

        private void exitToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Close();
        }

        private void uninstallDriverToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FilterAPI.StopFilter();
            FilterAPI.UnInstallDriver();
        }

        private void toolStripButton_UnitTest_Click(object sender, EventArgs e)
        {            
            toolStripButton_StartFilter_Click(null, null);
            ProcessUnitTestForm regUnitTest = new ProcessUnitTestForm();
            regUnitTest.ShowDialog();
        }
    }
}
