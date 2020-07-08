using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using EaseFilter.CommonObjects;

namespace ProcessMon
{
    public partial class ProcessFilterSettingForm : Form
    {
        ProcessFilterRule  selectedFilterRule = null;
        bool isNewFilterRule = false;

        public ProcessFilterSettingForm()
        {
            InitializeComponent();
            InitListView();
        }

        public void InitListView()
        {
            //init ListView control
            listView_FilterRules.Clear();		//clear control
            //create column header for ListView
            listView_FilterRules.Columns.Add("#", 20, System.Windows.Forms.HorizontalAlignment.Left);
            listView_FilterRules.Columns.Add("ProcessId", 100, System.Windows.Forms.HorizontalAlignment.Left);
            listView_FilterRules.Columns.Add("ProcessNameMask", 200, System.Windows.Forms.HorizontalAlignment.Left);
            listView_FilterRules.Columns.Add("ControlFlag", 100, System.Windows.Forms.HorizontalAlignment.Left);
            listView_FilterRules.Columns.Add("FileAccessRights", 300, System.Windows.Forms.HorizontalAlignment.Left);

            foreach (ProcessFilterRule rule in GlobalConfig.ProcessFilterRules.Values)
            {
                AddItem(rule);
                selectedFilterRule = rule;
            }

            InitSelectedFilterRule();

        }

        private void InitSelectedFilterRule()
        {
            if (null == selectedFilterRule)
            {
                selectedFilterRule = new ProcessFilterRule();
                selectedFilterRule.ProcessNameFilterMask = "*";
                selectedFilterRule.ControlFlag = 16128;

                isNewFilterRule = true;
            }

            if (selectedFilterRule.ProcessId.Length > 0 && selectedFilterRule.ProcessId != "0" )
            {
                textBox_ProcessId.Text = selectedFilterRule.ProcessId; 
                radioButton_Pid_Click(null,null);
            }
            else
            {
                radioButton_Name_Click(null, null);
                textBox_ProcessName.Text = selectedFilterRule.ProcessNameFilterMask;
                textBox_ControlFlag.Text = selectedFilterRule.ControlFlag.ToString();
            }
        }

        private void AddItem(ProcessFilterRule newRule)
        {
            string[] itemStr = new string[listView_FilterRules.Columns.Count];
            itemStr[0] = listView_FilterRules.Items.Count.ToString();
            itemStr[1] = newRule.ProcessId;
            itemStr[2] = newRule.ProcessNameFilterMask;
            itemStr[3] = newRule.ControlFlag.ToString();
            itemStr[4] = newRule.FileAccessRights;
            ListViewItem item = new ListViewItem(itemStr, 0);
            item.Tag = newRule;
            listView_FilterRules.Items.Add(item);
        }


        private void button_SelectControlFlag_Click(object sender, EventArgs e)
        {
            OptionForm optionForm = new OptionForm(OptionForm.OptionType.ProcessControlFlag, textBox_ControlFlag.Text);

            if (optionForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox_ControlFlag.Text = optionForm.ProcessControlFlag.ToString();
            }
        }


        private void button_AddFilter_Click(object sender, EventArgs e)
        {
            try
            {
                selectedFilterRule = null;
                InitSelectedFilterRule();


            }
            catch (Exception ex)
            {
                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                MessageBox.Show("Add registry filter rule failed." + ex.Message, "Add Filter Rule", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

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
                ProcessFilterRule filterRule = (ProcessFilterRule)item.Tag;
                GlobalConfig.RemoveProcessFilterRule(filterRule);
            }

            InitListView();
        }

        private void button_Apply_Click_1(object sender, EventArgs e)
        {
            GlobalConfig.SaveConfigSetting();
        }

        private void button_ProcessInfo_Click(object sender, EventArgs e)
        {
            string information = "Process Control: allow/deny the binaries executing, prevent the suspicious binaries(malware) from executing.\r\n\r\n";
            information += "Get notification for process/thread creation and termination.\r\n\r\n";
            information += "Set the file access rights to the process.\r\n\r\n";

            MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
            MessageBox.Show(information, "Process Filter Setting", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void listView_FilterRules_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView_FilterRules.SelectedItems.Count > 0)
            {
               selectedFilterRule = ((ProcessFilterRule)listView_FilterRules.SelectedItems[0].Tag).Copy();
               InitSelectedFilterRule();
            }
        }

        private void radioButton_Pid_Click(object sender, EventArgs e)
        {
            radioButton_Pid.Checked = true;
            textBox_ProcessName.ReadOnly = true;
            textBox_ProcessId.ReadOnly = false;
            button_SelectPid.Enabled = true;
        }

        private void radioButton_Name_Click(object sender, EventArgs e)
        {
            radioButton_Name.Checked = true;
            textBox_ProcessName.ReadOnly = false;
            textBox_ProcessId.ReadOnly = true;
            textBox_ProcessId.Text = "0";
            button_SelectPid.Enabled = false;
        }

        private void button_SelectPid_Click(object sender, EventArgs e)
        {
            OptionForm optionForm = new OptionForm(OptionForm.OptionType.ProccessId, textBox_ProcessId.Text);

            if (optionForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (optionForm.ProcessId.Length > 0)
                {
                    textBox_ProcessId.Text = optionForm.ProcessId;

                    //we choose to use process Id instead of the process name
                    textBox_ProcessName.Text = "";
                }
            }
        }

        private void button_ConfigFileAccessRights_Click(object sender, EventArgs e)
        {

            if (textBox_ProcessId.Text.Trim().Length > 0 && textBox_ProcessId.Text != "0" )
            {
                if (selectedFilterRule.ProcessId != textBox_ProcessId.Text)
                {
                    selectedFilterRule = new ProcessFilterRule();
                    isNewFilterRule = true;
                }

                selectedFilterRule.ProcessId = textBox_ProcessId.Text;
                
            }
            else if (textBox_ProcessName.Text.Trim().Length > 0)
            {
                if (selectedFilterRule.ProcessNameFilterMask != textBox_ProcessName.Text)
                {
                    isNewFilterRule = true;
                    selectedFilterRule = new ProcessFilterRule();
                }

                selectedFilterRule.ProcessNameFilterMask = textBox_ProcessName.Text;
               
            }
            else
            {
                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                MessageBox.Show("The process name mask and Pid can't be null.", "Add Filter Rule", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            uint controlFlag = 0;

            if (uint.TryParse(textBox_ControlFlag.Text, out controlFlag) && ((controlFlag & (uint)FilterAPI.ProcessControlFlag.DENY_NEW_PROCESS_CREATION )> 0))
            {
                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                MessageBox.Show("The process was blocked to executed, no need to set the file access rights for the process.", "Add Filter Rule", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (isNewFilterRule)
            {
                //default file access rights setting for new process filter rule

                //allow the windows dll or exe to be read by the process, or it can't be loaded.
                selectedFilterRule.FileAccessRights = "c:\\windows\\*!" + FilterAPI.ALLOW_FILE_READ_ACCESS + ";";

                //No access rights to all other folders by default.
                selectedFilterRule.FileAccessRights += "*!" + ((uint)FilterAPI.AccessFlag.LEAST_ACCESS_FLAG).ToString() + ";";
            }

            ProcessFileAccessRights processFileAccessRigths = new ProcessFileAccessRights(selectedFilterRule);
            processFileAccessRigths.ShowDialog();
        }

        private void button_Save_Click(object sender, EventArgs e)
        {
            if (textBox_ProcessId.Text.Trim().Length > 0 && textBox_ProcessId.Text != "0")
            {
                //please note that the process Id will be changed when the process launch every time.
                selectedFilterRule.ProcessId = textBox_ProcessId.Text;
                selectedFilterRule.ProcessNameFilterMask = "";
            }
            else if (textBox_ProcessName.Text.Trim().Length > 0)
            {
                selectedFilterRule.ProcessId = "";
                selectedFilterRule.ProcessNameFilterMask = textBox_ProcessName.Text;
            }
            else
            {
                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                MessageBox.Show("The process name mask and Pid can't be null.", "Add Filter Rule", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            uint controlFlag = 0;
            uint.TryParse(textBox_ControlFlag.Text, out controlFlag);
            selectedFilterRule.ControlFlag = controlFlag;

            GlobalConfig.AddProcessFilterRule(selectedFilterRule);

            InitListView();
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            InitListView();
        }

      
    }
}
