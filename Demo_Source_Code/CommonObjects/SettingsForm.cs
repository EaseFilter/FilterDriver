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

namespace EaseFilter.CommonObjects
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
            InitOptionForm();
        }

        private void InitOptionForm()
        {
            try
            {
                comboBox_EventLevel.Items.Clear();

                //General infomation
                foreach (EventLevel item in Enum.GetValues(typeof(EventLevel)))
                {
                    comboBox_EventLevel.Items.Add(item.ToString());

                    if ((uint)item == (uint)GlobalConfig.EventLevel)
                    {
                        comboBox_EventLevel.SelectedItem = item.ToString();
                    }
                }

                textBox_MaximumFilterMessage.Text = GlobalConfig.MaximumFilterMessages.ToString();
                textBox_TransactionLog.Text = GlobalConfig.FilterMessageLogName;
                textBox_LogSize.Text = (GlobalConfig.FilterMessageLogFileSize/1024).ToString();
                checkBox_TransactionLog.Checked = GlobalConfig.EnableLogTransaction;
                checkBox_OutputMessageToConsole.Checked = GlobalConfig.OutputMessageToConsole;
                checkBox_EnableNotification.Checked = GlobalConfig.EnableNotification;
                checkBox_DisableDir.Checked =  GlobalConfig.DisableDirIO;

                if ((GlobalConfig.BooleanConfig & (uint)FilterAPI.BooleanConfig.ENABLE_SEND_DATA_BUFFER) > 0)
                {
                    checkBox_SendBuffer.Checked = true;
                }
                else
                {
                    checkBox_SendBuffer.Checked = false;
                }

                foreach (uint pid in GlobalConfig.IncludePidList)
                {
                    if (textBox_IncludePID.Text.Length > 0)
                    {
                        textBox_IncludePID.Text += ";";
                    }

                    textBox_IncludePID.Text += pid.ToString();
                }

                foreach (uint pid in GlobalConfig.ExcludePidList)
                {
                    if (textBox_ExcludePID.Text.Length > 0)
                    {
                        textBox_ExcludePID.Text += ";";
                    }

                    textBox_ExcludePID.Text += pid.ToString();
                }

                foreach (uint pid in GlobalConfig.ProtectPidList)
                {
                    if (textBox_ProtectedPID.Text.Length > 0)
                    {
                        textBox_ProtectedPID.Text += ";";
                    }

                    textBox_ProtectedPID.Text += pid.ToString();
                }

                InitListView();

            }
            catch (Exception ex)
            {
                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                MessageBox.Show("Initialize the option form failed with error " + ex.Message, "Init options.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void InitListView()
        {
            //init ListView control
            listView_FilterRules.Clear();		//clear control
            //create column header for ListView
            listView_FilterRules.Columns.Add("#", 20, System.Windows.Forms.HorizontalAlignment.Left);
            listView_FilterRules.Columns.Add("InlcudeFilterMask", 150, System.Windows.Forms.HorizontalAlignment.Left);
            listView_FilterRules.Columns.Add("ExcludeFilterMask", 200, System.Windows.Forms.HorizontalAlignment.Left);
            listView_FilterRules.Columns.Add("AccessFlags", 100, System.Windows.Forms.HorizontalAlignment.Left);

            foreach (FilterRule rule in GlobalConfig.FilterRules.Values)
            {
                AddItem(rule);
            }

        }

        private void AddItem(FilterRule newRule)
        {
            string[] itemStr = new string[listView_FilterRules.Columns.Count];
            itemStr[0] = listView_FilterRules.Items.Count.ToString();
            itemStr[1] = newRule.IncludeFileFilterMask;
            itemStr[2] = newRule.ExcludeFileFilterMasks;
            itemStr[3] = newRule.AccessFlag.ToString();
            ListViewItem item = new ListViewItem(itemStr, 0);
            item.Tag = newRule;
            listView_FilterRules.Items.Add(item);
        }

        private void button_AddFilter_Click(object sender, EventArgs e)
        {
            string defaultAccessFlags = ((uint)FilterAPI.ALLOW_MAX_RIGHT_ACCESS ).ToString();
            FilterRule filterRule = new FilterRule();
            filterRule.Id = GlobalConfig.GetFilterRuleId();
            filterRule.IncludeFileFilterMask = "c:\\test\\*";
            filterRule.EncryptMethod = (int)FilterAPI.EncryptionMethod.ENCRYPT_FILE_WITH_SAME_KEY_AND_UNIQUE_IV;
            filterRule.EventType = (uint)(FilterAPI.EVENTTYPE.CREATED|FilterAPI.EVENTTYPE.DELETED|FilterAPI.EVENTTYPE.RENAMED|FilterAPI.EVENTTYPE.WRITTEN|FilterAPI.EVENTTYPE.READ|FilterAPI.EVENTTYPE.SECURITY_CHANGED);

            filterRule.MonitorIO = 2863311530;
            filterRule.ControlIO = 2863311530;
            filterRule.AccessFlag = (uint)FilterAPI.ALLOW_MAX_RIGHT_ACCESS ;

            FilterRuleForm filterRuleForm = new FilterRuleForm(filterRule);
            filterRuleForm.StartPosition = FormStartPosition.CenterParent;
            filterRuleForm.ShowDialog();

            InitListView();
        }

        private void button_EditFilterRule_Click(object sender, EventArgs e)
        {
            if (listView_FilterRules.SelectedItems.Count != 1)
            {
                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                MessageBox.Show("Please select one filter rule to edit.", "Edit Filter Rule", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            System.Windows.Forms.ListViewItem item = listView_FilterRules.SelectedItems[0];
            FilterRule filterRule = (FilterRule)item.Tag;

            FilterRuleForm filterRuleForm = new FilterRuleForm(filterRule);
            filterRuleForm.StartPosition = FormStartPosition.CenterParent;
            filterRuleForm.ShowDialog();

            InitListView();
        }

        private void button_DeleteFilter_Click(object sender, EventArgs e)
        {
            if (listView_FilterRules.SelectedItems.Count == 0)
            {
                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                MessageBox.Show("There are no filter rule selected.", "Delete Filter Rule", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (System.Windows.Forms.ListViewItem item in listView_FilterRules.SelectedItems)
            {
                FilterRule filterRule = (FilterRule)item.Tag;
                GlobalConfig.RemoveFilterRule(filterRule.IncludeFileFilterMask);
            }

            InitListView();
        }


        private void button_ApplyOptions_Click(object sender, EventArgs e)
        {
            try
            {
               
                GlobalConfig.MaximumFilterMessages = int.Parse(textBox_MaximumFilterMessage.Text);
                GlobalConfig.EnableLogTransaction = checkBox_TransactionLog.Checked;
                GlobalConfig.OutputMessageToConsole = checkBox_OutputMessageToConsole.Checked;
                GlobalConfig.EnableNotification = checkBox_EnableNotification.Checked;
                GlobalConfig.FilterMessageLogName = textBox_TransactionLog.Text;
                GlobalConfig.FilterMessageLogFileSize = long.Parse(textBox_LogSize.Text) * 1024;
                GlobalConfig.EventLevel = (EventLevel)comboBox_EventLevel.SelectedIndex;
                GlobalConfig.DisableDirIO = checkBox_DisableDir.Checked;

                if (checkBox_SendBuffer.Checked)
                {
                    GlobalConfig.BooleanConfig |= (uint)FilterAPI.BooleanConfig.ENABLE_SEND_DATA_BUFFER;
                }
                else
                {
                    GlobalConfig.BooleanConfig &= (uint)(~FilterAPI.BooleanConfig.ENABLE_SEND_DATA_BUFFER);
                }

                List<uint> inPids = new List<uint>();
                if (textBox_IncludePID.Text.Length > 0)
                {
                    if (textBox_IncludePID.Text.EndsWith(";"))
                    {
                        textBox_IncludePID.Text = textBox_IncludePID.Text.Remove(textBox_IncludePID.Text.Length - 1);
                    }

                    string[] pids = textBox_IncludePID.Text.Split(new char[] { ';' });
                    for (int i = 0; i < pids.Length; i++)
                    {
                        inPids.Add(uint.Parse(pids[i].Trim()));
                    }
                }
                 
                GlobalConfig.IncludePidList = inPids;

                List<uint> exPids = new List<uint>();
                if (textBox_ExcludePID.Text.Length > 0)
                {
                    if (textBox_ExcludePID.Text.EndsWith(";"))
                    {
                        textBox_ExcludePID.Text = textBox_ExcludePID.Text.Remove(textBox_ExcludePID.Text.Length - 1);
                    }

                    string[] pids = textBox_ExcludePID.Text.Split(new char[] { ';' });
                    for (int i = 0; i < pids.Length; i++)
                    {
                        exPids.Add(uint.Parse(pids[i].Trim()));
                    }
                }
                GlobalConfig.ExcludePidList = exPids;

                List<uint> protectPids = new List<uint>();
                if (textBox_ProtectedPID.Text.Trim().Length > 0)
                {
                    if (textBox_ProtectedPID.Text.EndsWith(";"))
                    {
                        textBox_ProtectedPID.Text = textBox_ProtectedPID.Text.Remove(textBox_ProtectedPID.Text.Length - 1);
                    }

                    string[] pids = textBox_ProtectedPID.Text.Split(new char[] { ';' });
                    for (int i = 0; i < pids.Length; i++)
                    {
                        protectPids.Add(uint.Parse(pids[i].Trim()));
                    }
                }

                GlobalConfig.ProtectPidList = protectPids;

                if (GlobalConfig.FilterRules.Count == 0)
                {
                    MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                    MessageBox.Show("There are no one filter rule added, the filter driver won't monitor any file.", "Filter Rule", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                GlobalConfig.SaveConfigSetting();

                this.Close();

            }
            catch (Exception ex)
            {
                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                MessageBox.Show("Save options failed with error " + ex.Message, "Save options.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button_SelectIncludePID_Click(object sender, EventArgs e)
        {

            OptionForm optionForm = new OptionForm(OptionForm.OptionType.ProccessId, textBox_IncludePID.Text);

            if (optionForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox_IncludePID.Text = optionForm.ProcessId;
            }
        }

        private void button_SelectExcludePID_Click(object sender, EventArgs e)
        {

            OptionForm optionForm = new OptionForm(OptionForm.OptionType.ProccessId, textBox_ExcludePID.Text);

            if (optionForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox_ExcludePID.Text = optionForm.ProcessId;
            }
        }


        private void button_SelectProtectPID_Click(object sender, EventArgs e)
        {
            OptionForm optionForm = new OptionForm(OptionForm.OptionType.ProccessId, textBox_ProtectedPID.Text);

            if (optionForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox_ProtectedPID.Text = optionForm.ProcessId;
            }
        }
     
       
     
    }
}
