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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;

using EaseFilter.CommonObjects;

namespace AutoFileCryptTool
{
    public partial class Form_FileCrypt : Form
    {
        bool isServiceRunning = true;

        public Form_FileCrypt()
        {
            GlobalConfig.filterType = FilterAPI.FilterType.FILE_SYSTEM_ENCRYPTION;

            InitializeComponent();

            StartPosition = FormStartPosition.CenterScreen;

            string lastError = string.Empty;
            if (!FilterDriverService.StartFilterService(out lastError))
            {
                isServiceRunning = false;

                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                MessageBox.Show("Start encryption service failed with error:" + lastError + ", auto folder encryption service will stop.", "Auto FileCrypt Service", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            button_Start.Enabled = false;
         
            InitializeFileCrypt();

        }

        private void InitializeFileCrypt()
        {
            if (GlobalConfig.FilterRules.Count > 0)
            {
                //delete the predefined items for users.
                listView_Folders.Items.Clear();
            }


            textBox_BlackList.Text = GlobalConfig.ProtectFolderBlackList;
            textBox_WhiteList.Text = GlobalConfig.ProtectFolderWhiteList;

            label_VersionInfo.Text = "Current version:" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + ",© EaseFilter Technologies All rights reserved.";

            if (!isServiceRunning)
            {
                listView_Folders.Items.Clear();
                string warningMessage = "The encryption service failed to start.";
                string message2 = "Please run with administrator permission.";
                ListViewItem item = new ListViewItem(warningMessage);
                listView_Folders.Items.Add(item);
                item = new ListViewItem(message2);
                listView_Folders.Items.Add(item);

                return;
            }


            foreach (string folder in GlobalConfig.FilterRules.Keys)
            {
                string folderName = folder;
                if (folderName.EndsWith("\\*"))
                {
                    folderName = folderName.Substring(0, folderName.Length - 2);
                }

                ListViewItem item = new ListViewItem(folderName);
                item.ImageIndex = 0;
                listView_Folders.Items.Add(item);

                AddEncyrptFolder(folderName);
            }

        }

        private void ApplySettingsToFilterDriver()
        {
            foreach (FilterRule filterRule in GlobalConfig.FilterRules.Values)
            {
                filterRule.EncryptionPassPhrase = GlobalConfig.MasterPassword;
                filterRule.AccessFlag = (uint)FilterAPI.AccessFlag.ENABLE_FILE_ENCRYPTION_RULE;
                filterRule.EncryptMethod = (int)FilterAPI.EncryptionMethod.ENCRYPT_FILE_WITH_SAME_KEY_AND_UNIQUE_IV;
                filterRule.ProcessRights = "";

                if (GlobalConfig.ProtectFolderWhiteList == "*")
                {
                    // all process can read the encyrpted file except the black list processes.
                    filterRule.AccessFlag |= (uint)FilterAPI.ALLOW_MAX_RIGHT_ACCESS;
                }
                else
                {
                    // all process can't read the encyrpted file except the white list processes.
                    filterRule.AccessFlag |= (uint)FilterAPI.ALLOW_MAX_RIGHT_ACCESS & (uint)(~FilterAPI.AccessFlag.ALLOW_READ_ENCRYPTED_FILES);


                    //for whitelist process, it has maximum acess rights.
                    string[] whiteList = GlobalConfig.ProtectFolderWhiteList.Split(new char[] { ';' });
                    if (whiteList.Length > 0)
                    {
                        foreach (string authorizedUser in whiteList)
                        {
                            if (authorizedUser.Trim().Length > 0)
                            {
                                filterRule.ProcessRights += ";" + authorizedUser + "!" + FilterAPI.ALLOW_MAX_RIGHT_ACCESS.ToString();
                            }
                        }
                    }
                }

                //for blacklist process, it has maximum acess rights.
                string[] blacklist = GlobalConfig.ProtectFolderBlackList.Split(new char[] { ';' });
                if (blacklist.Length > 0)
                {
                    foreach (string unAuthorizedUser in blacklist)
                    {
                        if (unAuthorizedUser.Trim().Length > 0)
                        {
                            //can't read the encrypted files
                            uint accessFlag = FilterAPI.ALLOW_MAX_RIGHT_ACCESS & (uint)(~FilterAPI.AccessFlag.ALLOW_READ_ENCRYPTED_FILES);
                            filterRule.ProcessRights += ";" + unAuthorizedUser + "!" + accessFlag.ToString();
                        }
                    }
                }

            }

            //send the filter rule settings to the filter driver here.
            GlobalConfig.SaveConfigSetting();

        }

        private bool AddEncyrptFolder(string folderName)
        {
            if (GlobalConfig.FilterRules.Count == 0)
            {
                //delete the predefined items for users.
                listView_Folders.Items.Clear();
            }

           
            string includeFilterMask = folderName + "\\*";

            if (!GlobalConfig.IsFilterRuleExist(includeFilterMask))
            {
                FilterRule filterRule = new FilterRule();
                filterRule.IncludeFileFilterMask = includeFilterMask;
                GlobalConfig.AddFilterRule(filterRule);

                ListViewItem item = new ListViewItem(folderName);
                item.ImageIndex = 0;
                listView_Folders.Items.Add(item);

                listView_Folders.EnsureVisible(listView_Folders.Items.Count - 1);

                ApplySettingsToFilterDriver();

                return true;
            }
            else
            {
                return false;
            }
        }

        private void button_AddFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fdDiaglog = new FolderBrowserDialog();
            if (fdDiaglog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string folderName = fdDiaglog.SelectedPath;
                AddEncyrptFolder(folderName);
            }
        }

        private void RemoveEncyrptFolder(string folderName)
        {
            GlobalConfig.RemoveFilterRule(folderName + "\\*");
            GlobalConfig.SaveConfigSetting();
        }

        private void button_RemoveFolder_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView_Folders.SelectedItems)
            {
                string folderName = item.Text;
                RemoveEncyrptFolder(folderName);

                listView_Folders.Items.Remove(item);
            }
        }

        private void listView_Folders_DragDrop(object sender, DragEventArgs e)
        {
            string[] fileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            if (null != fileList)
            {
                foreach (string folder in fileList)
                {
                    if (Directory.Exists(folder))
                    {
                        AddEncyrptFolder(folder);
                    }
                }
            }

        }

        private void listView_Folders_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void button_StartToEncrypt_Click(object sender, EventArgs e)
        {
            try
            {
                string fileName = textBox_EncryptSourceName.Text;
                string targetFileName = textBox_EncryptTargetName.Text;

                byte[] key = Utils.GetKeyByPassPhrase(GlobalConfig.MasterPassword,32);
                
                bool retVal = false;
                byte[] iv = Utils.GetIVByPassPhrase(GlobalConfig.MasterPassword);

                if (fileName.Equals(targetFileName, StringComparison.CurrentCulture))
                {
                    retVal = FilterAPI.AESEncryptFileWithTag(fileName, (uint)key.Length, key, (uint)iv.Length, iv, (uint)iv.Length, iv);
                }
                else
                {
                    retVal = FilterAPI.AESEncryptFileToFileWithTag(fileName, targetFileName, (uint)key.Length, key, (uint)iv.Length, iv, (uint)iv.Length, iv);
                }

                if (!retVal)
                {
                    string lastError = "Encrypt file " + targetFileName + " failed with error:" + FilterAPI.GetLastErrorMessage();
                    ShowMessage(lastError, MessageBoxIcon.Error);
                }
                else
                {
                    string lastError = "Encrypt file " + fileName + " to " + targetFileName + " succeeded.";
                    ShowMessage(lastError, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                string lastError = "Encrypt file failed with error:" + ex.Message;
                ShowMessage(lastError, MessageBoxIcon.Error);
            }
        }

        private void button_BrowseEncryptFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDiag  = new OpenFileDialog();
            if (fileDiag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox_EncryptSourceName.Text = fileDiag.FileName;
                textBox_EncryptTargetName.Text = fileDiag.FileName;
            }
        }

        private void button_BrowseFileToDecrypt_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDiag = new OpenFileDialog();
            if (fileDiag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox_DecryptSourceName.Text = fileDiag.FileName; 
                textBox_DecryptTargetName.Text = fileDiag.FileName;
               
            }
        }

        private void button_StartToDecrypt_Click(object sender, EventArgs e)
        {
            try
            {
                string fileName = textBox_DecryptSourceName.Text;
                string targetFileName = textBox_DecryptTargetName.Text;

                byte[] key = Utils.GetKeyByPassPhrase(GlobalConfig.MasterPassword, 32);

                bool retVal = false;
                if (fileName.Equals(targetFileName, StringComparison.CurrentCulture))
                {
                    retVal = FilterAPI.AESDecryptFile(fileName, (uint)key.Length, key, (uint)FilterAPI.DEFAULT_IV_TAG.Length, FilterAPI.DEFAULT_IV_TAG);
                }
                else
                {
                    retVal = FilterAPI.AESDecryptFileToFile(fileName, targetFileName,(uint)key.Length, key, (uint)FilterAPI.DEFAULT_IV_TAG.Length, FilterAPI.DEFAULT_IV_TAG);
                }

                if (!retVal)
                {
                    string lastError = "Decrypt file " + fileName + " failed with error:" + FilterAPI.GetLastErrorMessage();
                    ShowMessage(lastError, MessageBoxIcon.Error);
                }
                else
                {
                    string lastError = "Decrypt file " + fileName + " to " + targetFileName + " succeeded.";
                    ShowMessage(lastError, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                string lastError = "Decrypt file failed with error:" + ex.Message;
                ShowMessage(lastError, MessageBoxIcon.Error);
            }
        }

        private void ShowMessage(string message,MessageBoxIcon messageIcon)
        {
            MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
            MessageBox.Show(message, "Message", MessageBoxButtons.OK, messageIcon);
        }

     

        private void linkLabel_Report_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.easefilter.com/info/easefilter_changeset.txt");
        }

        private void linkLabel1_ReportClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.easefilter.com/ReportIssue.htm");
        }

        private void linkLabel_SDK_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://easefilter.com/Forums_Files/Transparent_Encryption_Filter_Driver.htm");
        }

        private void button_Activate_Click(object sender, EventArgs e)
        {
        }

        private void Form_FileCrypt_FormClosed(object sender, FormClosedEventArgs e)
        {
            MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
            if (MessageBox.Show("Do you want to minimize to system tray?", "Close", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {

            }
            else
            {
                GlobalConfig.Stop();
                FilterAPI.StopFilter();

                Application.Exit();
            }
        }


        private void button_ApplySetting_Click(object sender, EventArgs e)
        {

            GlobalConfig.ProtectFolderWhiteList = textBox_WhiteList.Text;
            GlobalConfig.ProtectFolderBlackList = textBox_BlackList.Text;

            GlobalConfig.SaveConfigSetting();

            ApplySettingsToFilterDriver();


            MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
            MessageBox.Show("Your setting was saved.", "Apply settings", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void button_Start_Click(object sender, EventArgs e)
        {
            button_Start.Enabled = false;
            button_Stop.Enabled = true;

            string lastError = string.Empty;
            if (!FilterDriverService.StartFilterService(out lastError))
            {
                isServiceRunning = false;

                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                MessageBox.Show("Start encryption service failed with error:" + lastError + ", auto folder encryption service will stop.", "Auto FileCrypt Service", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button_Stop_Click(object sender, EventArgs e)
        {
            button_Start.Enabled = true;
            button_Stop.Enabled = false;

            FilterDriverService.StopService();

        }      
       

    }
}
