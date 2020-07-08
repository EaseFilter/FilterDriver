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
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

using EaseFilter.CommonObjects;

namespace RegMon
{
    public partial class RegMonForm : Form
    {
         //Purchase a license key with the link: http://www.easefilter.com/Order.htm
        //Email us to request a trial key: info@easefilter.com //free email is not accepted.
        string registerKey = GlobalConfig.registerKey;

        RegistryMessage filterMessage = null;

        public RegMonForm()
        {
            GlobalConfig.filterType = FilterAPI.FilterType.FILE_SYSTEM_REGISTRY;

            InitializeComponent();

            StartPosition = FormStartPosition.CenterScreen;
            filterMessage = new RegistryMessage(listView_Info);

            DisplayVersion();

            InitListView();

        }

        ~RegMonForm()
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
            listView_Info.Columns.Add("Time", 100, System.Windows.Forms.HorizontalAlignment.Left);
            listView_Info.Columns.Add("UserName", 150, System.Windows.Forms.HorizontalAlignment.Left);
            listView_Info.Columns.Add("ProcessName(PID)", 100, System.Windows.Forms.HorizontalAlignment.Left);
            listView_Info.Columns.Add("ThreadId", 60, System.Windows.Forms.HorizontalAlignment.Left);
            listView_Info.Columns.Add("RegCallbackClassName", 160, System.Windows.Forms.HorizontalAlignment.Left);
            listView_Info.Columns.Add("KeyName", 300, System.Windows.Forms.HorizontalAlignment.Left);
            listView_Info.Columns.Add("Return Status", 100, System.Windows.Forms.HorizontalAlignment.Left);
            listView_Info.Columns.Add("Description", 400, System.Windows.Forms.HorizontalAlignment.Left);
        }

        private void RegMonForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            FilterAPI.ResetConfigData();
            FilterAPI.StopFilter();
            GlobalConfig.Stop();
            Application.Exit();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
            RegistryAccessControlForm regitryAccessControlForm = new RegistryAccessControlForm();
            regitryAccessControlForm.ShowDialog();
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

                toolStripButton_StartFilter.Enabled = false;
                toolStripButton_Stop.Enabled = true;


                if (GlobalConfig.RegistryFilterRules.Count == 0 && null != sender)
                {
                    RegistryFilterRule regFilterRule = new RegistryFilterRule();
                    regFilterRule.ProcessNameFilterMask = "*";
                    regFilterRule.AccessFlag = FilterAPI.MAX_REGITRY_ACCESS_FLAG;
                    regFilterRule.RegCallbackClass = 93092006832128;

                    GlobalConfig.AddRegistryFilterRule(regFilterRule);

                    MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                    MessageBox.Show("You didn't setup any filtere rule, by defult it will monitor all registry access.");
                }



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
                FilterAPI.MessageSendData messageSend = (FilterAPI.MessageSendData)Marshal.PtrToStructure(sendDataPtr, typeof(FilterAPI.MessageSendData));

                if (FilterAPI.MESSAGE_SEND_VERIFICATION_NUMBER != messageSend.VerificationNumber)
                {
                    MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                    MessageBox.Show("Received message corrupted.Please check if the MessageSendData structure is correct.");

                    EventManager.WriteMessage(139, "FilterCallback", EventLevel.Error, "Received message corrupted.Please check if the MessageSendData structure is correct.");
                    return false;
                }

                filterMessage.AddMessage(messageSend);

                if (replyDataPtr.ToInt64() != 0)
                {
                    FilterAPI.MessageReplyData messageReply = (FilterAPI.MessageReplyData)Marshal.PtrToStructure(replyDataPtr, typeof(FilterAPI.MessageReplyData));

                    if (messageSend.MessageType == (uint)FilterAPI.FilterCommand.FILTER_SEND_REG_CALLBACK_INFO)
                    {
                        //this is registry callback request
                        RegistryHandler.AuthorizeRegistryAccess(messageSend, ref messageReply);
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

        private void toolStripButton_UnitTest_Click(object sender, EventArgs e)
        {
            toolStripButton_StartFilter_Click(null, null);
            RegUnitTest regUnitTest = new RegUnitTest();
            regUnitTest.ShowDialog();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void toolsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
