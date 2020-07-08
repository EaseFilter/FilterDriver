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

namespace FileMonitor
{
    public partial class MonitorForm : Form
    {
        //Purchase a license key with the link: http://www.easefilter.com/Order.htm
        //Email us to request a trial key: info@easefilter.com //free email is not accepted.
        string registerKey = GlobalConfig.registerKey;

        FilterMessage filterMessage = null;

        public MonitorForm()
        {

            GlobalConfig.filterType = FilterAPI.FilterType.FILE_SYSTEM_MONITOR;

            InitializeComponent();

            StartPosition = FormStartPosition.CenterScreen;

            filterMessage = new FilterMessage(listView_Info);

            DisplayVersion();

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

        ~MonitorForm()
        {
            GlobalConfig.Stop();
        }

        private void MonitorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            GlobalConfig.Stop();
        }

        private void toolStripButton_StartFilter_Click(object sender, EventArgs e)
        {
            try
            {
                string lastError = string.Empty;

                bool ret = FilterAPI.StartFilter( (int)GlobalConfig.FilterConnectionThreads
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

                toolStripButton_StartFilter.Enabled = false;
                toolStripButton_Stop.Enabled = true;

                if (GlobalConfig.FilterRules.Count == 0 && null != sender)
                {
                    FilterRule filterRule = new FilterRule();
                    filterRule.IncludeFileFilterMask = "c:\\test\\*";
                    filterRule.ExcludeFileFilterMasks = "c:\\windows*";
                    filterRule.EventType = (uint)(FilterAPI.EVENTTYPE.WRITTEN | FilterAPI.EVENTTYPE.CREATED | FilterAPI.EVENTTYPE.DELETED | FilterAPI.EVENTTYPE.RENAMED);
                    filterRule.AccessFlag = (uint)FilterAPI.ALLOW_MAX_RIGHT_ACCESS;
                    GlobalConfig.FilterRules.Add(filterRule.IncludeFileFilterMask, filterRule);

                    MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                    MessageBox.Show("You don't have any monitor folder setup, add c:\\test\\* as your default test folder, I/Os from c:\\test\\* will show up in the console.");
                }

                GlobalConfig.SendConfigSettingsToFilter();

                EventManager.WriteMessage(102, "StartFilter", EventLevel.Information, "Start filter service succeeded.");
            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(104, "StartFilter", EventLevel.Error, "Start filter service failed with error " + ex.Message);
            }

        }

        private void toolStripButton_Stop_Click(object sender, EventArgs e)
        {
            FilterAPI.StopFilter();

            toolStripButton_StartFilter.Enabled = true;
            toolStripButton_Stop.Enabled = false;
        }

        private void toolStripButton_ClearMessage_Click(object sender, EventArgs e)
        {
            filterMessage.InitListView();
        }

        Boolean FilterCallback(IntPtr sendDataPtr, IntPtr replyDataPtr)
        {
            Boolean ret = true;

            try
            {
                FilterAPI.MessageSendData messageSend = (FilterAPI.MessageSendData)Marshal.PtrToStructure(sendDataPtr, typeof(FilterAPI.MessageSendData));

                if (FilterAPI.MESSAGE_SEND_VERIFICATION_NUMBER != messageSend.VerificationNumber)
                {
                    MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                    MessageBox.Show("Received message corrupted.Please check if the MessageSendData structure is correct.");

                    EventManager.WriteMessage(139, "FilterCallback", EventLevel.Error, "Received message corrupted.Please check if the MessageSendData structure is correct.");

                    return false;
                }

                filterMessage.AddMessage(messageSend);

                MonitorDemo.UnitTestCallbackHandler(messageSend);

                string info = "FileMonitor process request " + FilterMessage.FormatIOName(messageSend) + ",pid:" + messageSend.ProcessId +
                           " ,filename:" + messageSend.FileName + ",return status:" + FilterMessage.FormatStatus(messageSend.Status);

                if (messageSend.Status == (uint)NtStatus.Status.Success)
                {
                    ret = false;
                    EventManager.WriteMessage(98, "FilterCallback", EventLevel.Verbose, info);
                }
                else
                {
                    ret = true;
                    EventManager.WriteMessage(98, "FilterCallback", EventLevel.Error, info);
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
           EventManager.WriteMessage(165,"DisconnectCallback", EventLevel.Information,"Filter Disconnected." + FilterAPI.GetLastErrorMessage());
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsForm settingForm = new SettingsForm();
            settingForm.StartPosition = FormStartPosition.CenterParent;
            settingForm.ShowDialog();
        }


        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            EventForm.DisplayEventForm();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MonitorForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
            if (MessageBox.Show("Do you want to minimize to system tray?", "Close", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {

            }
            else
            {
                GlobalConfig.Stop();
                Application.Exit();
            }
        }

        private void unInstallFilterDriverToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FilterAPI.StopFilter();
            FilterAPI.UnInstallDriver();
        }

        private void toolStripButton_LoadMessage_Click(object sender, EventArgs e)
        {
            filterMessage.LoadMessageFromLogToConsole();
        }

        private void toolStripButton_UnitTest_Click(object sender, EventArgs e)
        {
            toolStripButton_StartFilter_Click(null, null);

            MonitorDemo monitorDemo = new MonitorDemo();
            monitorDemo.ShowDialog();
        }

      

        private void toolStripButton_TestTool_Click(object sender, EventArgs e)
        {
            string name = Path.Combine(GlobalConfig.AssemblyPath, "FileIOTest.exe");

            if (!File.Exists(name))
            {
                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                MessageBox.Show(name +" doesn't exist.");
                return;
            }

            Process testTool = new Process();
            testTool.StartInfo.FileName = name;
            testTool.Start();
        }

        private void toolStripButton_Help_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.easefilter.com/programming.htm");
        }

    }
}
