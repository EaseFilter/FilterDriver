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

namespace  SecureShare
{
    public partial class ConsoleForm : Form
    {
        //Purchase a license key with the link: http://www.easefilter.com/Order.htm
        //Email us to request a trial key: info@easefilter.com //free email is not accepted.
        string registerKey = GlobalConfig.registerKey;

        FilterMessage filterMessage = null;
        FilterWorker filterWorker = null;

        public ConsoleForm()
        {

            GlobalConfig.filterType = FilterAPI.FilterType.FILE_SYSTEM_MONITOR | FilterAPI.FilterType.FILE_SYSTEM_CONTROL | FilterAPI.FilterType.FILE_SYSTEM_ENCRYPTION
                | FilterAPI.FilterType.FILE_SYSTEM_PROCESS | FilterAPI.FilterType.FILE_SYSTEM_REGISTRY;

            InitializeComponent();

            StartPosition = FormStartPosition.CenterScreen;

            filterMessage = new FilterMessage(listView_Info);
            filterWorker = new FilterWorker(listView_Info);

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

        ~ConsoleForm()
        {
            GlobalConfig.Stop();
        }

        private void MonitorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            GlobalConfig.Stop();
        }

        private void toolStripButton_StartFilter_Click(object sender, EventArgs e)
        {
            string lastError = string.Empty;
            if (filterWorker.StartService(ref lastError))
            {
                toolStripButton_StartFilter.Enabled = false;
                toolStripButton_Stop.Enabled = true;
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
            ShareFileSettingForm settingForm = new ShareFileSettingForm();
            settingForm.StartPosition = FormStartPosition.CenterParent;
            settingForm.ShowDialog();
        }


        private void ConsoleForm_FormClosed(object sender, FormClosedEventArgs e)
        {

            GlobalConfig.Stop();
            Application.Exit();
        }

        private void unInstallFilterDriverToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FilterAPI.StopFilter();
            FilterAPI.UnInstallDriver();
        }

        private void shareManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GlobalConfig.StoreSharedFileMetaDataInServer)
            {
                ShareFileManagerInServer shareManager = new ShareFileManagerInServer();
                shareManager.ShowDialog();
            }
            else
            {
                ShareFileManagerInLocal shareManager = new ShareFileManagerInLocal();
                shareManager.ShowDialog();
            }
        }

        private void toolStripButton_Help_Click(object sender, EventArgs e)
        {
            MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
            string helpInfo = "1.Setup the configuration by clicking the settings to test in local or in EaseFilter Server.\r\n";
            helpInfo += "2.Create encrypted files in the share file manager.\r\n";
            helpInfo += "3.Distribute the shared file to the clients.\r\n";
            helpInfo += "4. Copy the shared file to the share file drop folder in the client.\r\n";
            helpInfo += "5. if you want to copy the shared file to real time protected folder in the client, you have to stop the filter service first, or the file will be encrypted again.\r\n";
            helpInfo += "6. Start the filter service in the client.\r\n";            
            helpInfo += "7. Open the shared file in the client.\r\n";
            helpInfo += "8. You can check who accessed your files from the access log in the share file manager when you test with the EaseFilter Server.\r\n";

            MessageBox.Show(helpInfo, "How to use this application?", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

      
    
    }
}
