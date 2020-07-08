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
    public partial class FilterRuleForm : Form
    {
        FilterRule filterRule = new FilterRule();

        public FilterRuleForm(FilterRule _filterRule)
        {
            InitializeComponent();

            filterRule = _filterRule;
            textBox_IncludeFilterMask.Text = filterRule.IncludeFileFilterMask;
            textBox_ExcludeFilterMask.Text = filterRule.ExcludeFileFilterMasks;
            textBox_SelectedEvents.Text = filterRule.EventType.ToString();
            textBox_IncludePID.Text = filterRule.IncludeProcessIds;
            textBox_ExcludePID.Text = filterRule.ExcludeProcessIds;
            textBox_ExcludeProcessNames.Text = filterRule.ExcludeProcessNames;
            textBox_IncludeProcessNames.Text = filterRule.IncludeProcessNames;
            textBox_ExcludeUserNames.Text = filterRule.ExcludeUserNames;
            textBox_IncludeUserNames.Text = filterRule.IncludeUserNames;
            textBox_MonitorIO.Text = filterRule.MonitorIO.ToString();
            textBox_FilterDesiredAccess.Text = filterRule.FilterDesiredAccess.ToString();
            textBox_FilterDisposition.Text = filterRule.FilterDisposition.ToString();
            textBox_FilterCreateOptions.Text = filterRule.FilterCreateOptions.ToString();

            if (GlobalConfig.filterType == FilterAPI.FilterType.FILE_SYSTEM_MONITOR)
            {
                button_ControlSettings.Visible = false;
            }
          
        }

        private void button_SaveFilter_Click(object sender, EventArgs e)
        {
            if (textBox_IncludeFilterMask.Text.Trim().Length == 0)
            {
                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                MessageBox.Show("The include filter mask can't be empty.", "Add Filter Rule", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //include filter mask must be unique, but it can have multiple exclude filter masks associate to the same include filter mask.
            filterRule.IncludeFileFilterMask = textBox_IncludeFilterMask.Text;
            filterRule.ExcludeFileFilterMasks = textBox_ExcludeFilterMask.Text;
            filterRule.IncludeProcessNames = textBox_IncludeProcessNames.Text;
            filterRule.ExcludeProcessNames = textBox_ExcludeProcessNames.Text;
            filterRule.IncludeUserNames = textBox_IncludeUserNames.Text;
            filterRule.ExcludeUserNames = textBox_ExcludeUserNames.Text;
            filterRule.IncludeProcessIds = textBox_IncludePID.Text;
            filterRule.ExcludeProcessIds = textBox_ExcludePID.Text;
            filterRule.EventType = uint.Parse(textBox_SelectedEvents.Text);
            filterRule.MonitorIO = uint.Parse(textBox_MonitorIO.Text);
            filterRule.Id = GlobalConfig.GetFilterRuleId();

            filterRule.FilterDesiredAccess = uint.Parse(textBox_FilterDesiredAccess.Text);
            filterRule.FilterDisposition = uint.Parse(textBox_FilterDisposition.Text);
            filterRule.FilterCreateOptions = uint.Parse(textBox_FilterCreateOptions.Text);

            GlobalConfig.AddFilterRule(filterRule);

            this.Close();
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

        private void button_SelectedEvents_Click(object sender, EventArgs e)
        {
            OptionForm optionForm = new OptionForm(OptionForm.OptionType.EventNotification, textBox_SelectedEvents.Text);

            if (optionForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox_SelectedEvents.Text = optionForm.EventNotification.ToString();
            }
        }

        private void button_RegisterMonitorIO_Click(object sender, EventArgs e)
        {
            OptionForm optionForm = new OptionForm(OptionForm.OptionType.Register_Request, textBox_MonitorIO.Text,true);

            if (optionForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox_MonitorIO.Text = optionForm.RequestRegistration.ToString();
            }
        }

        private void button_FilterDesiredAccess_Click(object sender, EventArgs e)
        {
            OptionForm optionForm = new OptionForm(OptionForm.OptionType.FilterDesiredAccess, textBox_FilterDesiredAccess.Text);

            if (optionForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox_FilterDesiredAccess.Text = optionForm.FilterDesiredAccess.ToString();
            }
        }

        private void button_FilterDisposition_Click(object sender, EventArgs e)
        {
            OptionForm optionForm = new OptionForm(OptionForm.OptionType.FilterDisposition, textBox_FilterDisposition.Text);

            if (optionForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox_FilterDisposition.Text = optionForm.FilterDisposition.ToString();
            }
        }

        private void button_FilterCreateOptions_Click(object sender, EventArgs e)
        {
            OptionForm optionForm = new OptionForm(OptionForm.OptionType.FilterCreateOptions, textBox_FilterCreateOptions.Text);

            if (optionForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox_FilterCreateOptions.Text = optionForm.FilterCreateOptions.ToString();
            }
        }      


        private void button_ControlSettings_Click(object sender, EventArgs e)
        {
            ControlFilterRuleSettigs controlSettingForm = new ControlFilterRuleSettigs(filterRule);

            if (controlSettingForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                filterRule = controlSettingForm.filterRule;
            }
        }

        private void textBox_IncludeFilterMask_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Input managed file filter mask with wild card character '*', for map drive format '*\\192.168.1.1\\shareName\\foldername\\*'", textBox_IncludeFilterMask);
        }

        private void textBox_ExcludeFilterMask_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Exclude the files IO with this setting, you can add multiple exclude file filter masks seperated with ';'.", textBox_ExcludeFilterMask);
        }

        private void textBox_IncludeProcessNames_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("If the include process name is not empty,then the filter driver only manages the files were opened by the include process name.", textBox_IncludeProcessNames);
        }

        private void textBox_ExcludeProcessNames_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("If the exclude process name is not empty,then the filter driver won't manage the files were opened by the exclude process name.", textBox_ExcludeProcessNames);
        }

        private void textBox_IncludeUserNames_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("If the include user name is not empty,then the filter driver only manages the files were opened by the include user name.", textBox_IncludeUserNames);
        }

        private void textBox_ExcludeUserNames_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("If the exclude user name is not empty,then the filter driver won't manage the files were opened by the exclude user name.", textBox_ExcludeUserNames);
        }

        private void textBox_SelectedEvents_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("This is the asynchronous IO notification with the user name, process name, file name information after the file was closed.", textBox_SelectedEvents);
        }

        private void textBox_MonitorIO_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("This is the asynchronous IO notification with the user name, process name, file name and detail IO data information.", textBox_MonitorIO);
        }

        private void textBox_FilterDesiredAccess_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("If the IO was registered, and if the DesiredAccess is not 0, only the file opens with this DesiredAccess will trigger the callback.", textBox_FilterDesiredAccess);
        }

        private void textBox_FilterDisposition_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("If the IO was registered, and if the Disposition is not 0, only the file opens with this Disposition will trigger the callback.", textBox_FilterDisposition);
        }

        private void textBox_FilterCreateOptions_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("If the IO was registered, and if the CreateOptions is not 0, only the file opens with this CreateOptions will trigger the callback.", textBox_FilterCreateOptions);
        }      

    }
}
