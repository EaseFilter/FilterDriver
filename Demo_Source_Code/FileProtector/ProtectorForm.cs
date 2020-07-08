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

namespace FileProtector
{
    public partial class ProtectorForm : Form
    {
        //Purchase a license key with the link: http://www.easefilter.com/Order.htm
        //Email us to request a trial key: info@easefilter.com //free email is not accepted.
        string registerKey = GlobalConfig.registerKey;

        FilterMessage filterMessage = null;

        public ProtectorForm()
        {
            GlobalConfig.filterType = FilterAPI.FilterType.FILE_SYSTEM_MONITOR | FilterAPI.FilterType.FILE_SYSTEM_CONTROL | FilterAPI.FilterType.FILE_SYSTEM_ENCRYPTION
                |FilterAPI.FilterType.FILE_SYSTEM_PROCESS|FilterAPI.FilterType.FILE_SYSTEM_REGISTRY;

            InitializeComponent();

            StartPosition = FormStartPosition.CenterScreen;
            filterMessage = new FilterMessage(listView_Info);

            DisplayVersion();

        }

        ~ProtectorForm()
        {
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


                if (GlobalConfig.FilterRules.Count == 0 && null != sender )
                {
                    FilterRule filterRule = new FilterRule();
                    filterRule.Id = GlobalConfig.GetFilterRuleId();
                    filterRule.IncludeFileFilterMask = "c:\\test\\*";
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

        static public void ToDebugger(string msg)
        {
            //System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(false);
            //string caller = st.GetFrame(1).GetMethod().Name;
            System.Diagnostics.Debug.WriteLine(" Time:" + DateTime.Now.ToLongTimeString() + ": " + msg);
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

                EventManager.WriteMessage(149, "FilterCallback", EventLevel.Verbose, "Received message Id#" + messageSend.MessageId + " type:" + messageSend.MessageType
                    + " CreateOptions:" + messageSend.CreateOptions.ToString("X") + " infoClass:" + messageSend.InfoClass + " fileName:" + messageSend.FileName );

                filterMessage.AddMessage(messageSend);

                FileProtectorUnitTest.FileIOEventHandler(messageSend);

                if (replyDataPtr.ToInt64() != 0)
                {
                    FilterAPI.MessageReplyData messageReply = (FilterAPI.MessageReplyData)Marshal.PtrToStructure(replyDataPtr, typeof(FilterAPI.MessageReplyData));

                    //if (messageSend.MessageType == (uint)FilterAPI.FilterCommand.FILTER_SEND_PROCESS_TERMINATION_INFO)
                    //{
                    //    //the process termination callback, you can get the notification if you register the callback setting of the process filter.
                    //}

                    if (messageSend.MessageType == (uint)FilterAPI.FilterCommand.FILTER_SEND_PROCESS_CREATION_INFO)
                    {
                        //this is new process creation, you can block it here by returning the STATUS_ACCESS_DENIED, below is the process information
                        FilterAPI.PROCESS_INFO processInfo = (FilterAPI.PROCESS_INFO)Marshal.PtrToStructure(sendDataPtr, typeof(FilterAPI.PROCESS_INFO));

                        messageReply.ReturnStatus = processInfo.Status;
                        Marshal.StructureToPtr(messageReply, replyDataPtr, true);
                    }                 
                    else if ( messageSend.MessageType == (uint)FilterAPI.FilterCommand.FILTER_REQUEST_ENCRYPTION_IV_AND_KEY )
                    {
                        //this is encryption filter rule with boolean config "REQUEST_ENCRYPT_KEY_AND_IV_FROM_SERVICE" enabled.                        
                        //the filter driver request the IV and key to open or create the encrypted file.                        

                        //if you want to deny the file open or creation, you set the value as below:
                        //messageReply.ReturnStatus = (uint)FilterAPI.NTSTATUS.STATUS_ACCESS_DENIED;
                        //messageReply.FilterStatus = (uint)FilterAPI.FilterStatus.FILTER_COMPLETE_PRE_OPERATION;

                        EventManager.WriteMessage(200, "filtercallback", EventLevel.Verbose, messageSend.FileName + " FILTER_REQUEST_ENCRYPTION_IV_AND_KEY" );

                        //Here we return the test iv and key to the filter driver, you need to replace with you own iv and key in your code.
                        AESDataBuffer  aesData = new AESDataBuffer();
                        aesData.AccessFlags = FilterAPI.ALLOW_MAX_RIGHT_ACCESS;
                        aesData.IV = FilterAPI.DEFAULT_IV_TAG;
                        aesData.IVLength = (uint)aesData.IV.Length;
                        aesData.EncryptionKey = Utils.GetKeyByPassPhrase(DigitalRightControl.PassPhrase,32);
                        aesData.EncryptionKeyLength = (uint)aesData.EncryptionKey.Length;

                        byte[] aesDataArray = DigitalRightControl.ConvertAESDataToByteArray(aesData);
                        messageReply.DataBufferLength = (uint)aesDataArray.Length;
                        Array.Copy(aesDataArray,messageReply.DataBuffer,aesDataArray.Length);

                        messageReply.ReturnStatus = (uint)FilterAPI.NTSTATUS.STATUS_SUCCESS;


                    }
                    else
                    {
                        //this is for control filter driver when the pre-IO was registered.

                        //here you can control the IO behaviour and modify the data.
                        if (!FileProtectorUnitTest.UnitTestCallbackHandler(messageSend) || !FilterService.AuthorizeFileAccess(messageSend, ref messageReply))
                        {
                            //to comple the PRE_IO
                            messageReply.ReturnStatus = (uint)FilterAPI.NTSTATUS.STATUS_ACCESS_DENIED;
                            messageReply.FilterStatus = (uint)FilterAPI.FilterStatus.FILTER_COMPLETE_PRE_OPERATION;

                            EventManager.WriteMessage(160, "FilterCallback", EventLevel.Error, "Return error for I/O request:" + ((FilterAPI.MessageType)messageSend.MessageType).ToString() +
                                ",fileName:" + messageSend.FileName);
                        }
                        else
                        {

                            messageReply.MessageId = messageSend.MessageId;
                            messageReply.MessageType = messageSend.MessageType;
                            messageReply.ReturnStatus = (uint)FilterAPI.NTSTATUS.STATUS_SUCCESS;

                        }

                    }

                    Marshal.StructureToPtr(messageReply, replyDataPtr, true);
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

        private void encryptFileWithToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EncryptedFileForm encryptForm = new EncryptedFileForm("Encrypt file", FilterAPI.EncryptType.Encryption);
            encryptForm.ShowDialog();
        }

        private void decryptFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EncryptedFileForm encryptForm = new EncryptedFileForm("Decrypt file", FilterAPI.EncryptType.Decryption);
            encryptForm.ShowDialog();
        }

        private void getEncryptedFileIVTagToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InputForm inputForm = new InputForm("Input file name", "Plase input file name", "");

            if (inputForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fileName = inputForm.InputText;

                //by default we set the custom tag data with iv data

                byte[] iv = new Byte[16];
                uint ivLength = (uint)iv.Length;
                bool retVal = FilterAPI.GetAESTagData(fileName, ref ivLength, iv);

                if (!retVal)
                {
                    MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                    MessageBox.Show("GetAESTagData failed with error " + FilterAPI.GetLastErrorMessage(), "GetAESTagData", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                MessageBox.Show("Get encrypted file " + fileName + " tag data succeeded.", "IV Tag", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

  
        private void ProtectorForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
            if (MessageBox.Show("Do you want to minimize to system tray?", "Close", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {

            }
            else
            {
                FilterAPI.StopFilter();
                GlobalConfig.Stop();
                Application.Exit();
            }
        }

        private void unInstallFilterDriverToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FilterAPI.StopFilter();
            FilterAPI.UnInstallDriver();
        }     

        private void protectorTutorialToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TutorialForm tutorialForm = new TutorialForm();
            tutorialForm.ShowDialog();
        }

        private void toolStripButton_LoadMessage_Click(object sender, EventArgs e)
        {
            filterMessage.LoadMessageFromLogToConsole();
        }

        private void toolStripButton_UnitTest_Click(object sender, EventArgs e)
        {
            toolStripButton_StartFilter_Click(null, null);
           // System.Threading.Tasks.Task.Factory.StartNew(() => { ProtectorUnitTest(); });
            ProtectorUnitTest();

        }

        private void ProtectorUnitTest()
        {
            FileProtectorUnitTest fileProtectorUnitTest = new FileProtectorUnitTest();
            fileProtectorUnitTest.ShowDialog();
        }

        private void toolStripButton_TestTool_Click(object sender, EventArgs e)
        {
            string name = Path.Combine(GlobalConfig.AssemblyPath, "FileIOTest.exe");

            if (!File.Exists(name))
            {
                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                MessageBox.Show(name + " doesn't exist.");
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
