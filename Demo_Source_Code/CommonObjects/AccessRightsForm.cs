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

    public partial class Form_AccessRights : Form
    {
        public enum AccessRightType
        {
            ProcessNameRight = 0,
            ProccessIdRight,
            UserNameRight,
        }

        AccessRightType type = AccessRightType.ProccessIdRight;

        public string accessRightText = string.Empty;

        public Form_AccessRights(AccessRightType _type)
        {
            InitializeComponent();

            type = _type;
            groupBox_UserName.Location = groupBox_ProcessName.Location;

            textBox_FileAccessFlags.Text = FilterAPI.ALLOW_MAX_RIGHT_ACCESS.ToString();

            switch (type)
            {
                case AccessRightType.ProcessNameRight: groupBox_ProcessName.Visible = true; break;
                case AccessRightType.ProccessIdRight: groupBox_ProcessId.Visible = true; break;
                case AccessRightType.UserNameRight: groupBox_UserName.Visible = true; break;
            }
            
        }

        private void button_Add_Click(object sender, EventArgs e)
        {

            switch (type)
            {
                case AccessRightType.ProcessNameRight:
                    {
                        if (textBox_ProcessName.Text.Trim().Length > 0)
                        {
                            string[] processNames = textBox_ProcessName.Text.Trim().Split(new char[] { ';' });
                            if (processNames.Length > 0)
                            {
                                foreach (string processName in processNames)
                                {
                                    if (processName.Trim().Length > 0)
                                    {
                                        if (accessRightText.Length > 0)
                                        {
                                            accessRightText += ";";
                                        }

                                        accessRightText += processName.Trim() + "!" + textBox_FileAccessFlags.Text;
                                    }
                                }
                            }
                        }
                        break;
                    }

                case AccessRightType.UserNameRight:
                    {
                        if (textBox_UserName.Text.Trim().Length > 0)
                        {
                            string[] userNames = textBox_UserName.Text.Trim().Split(new char[] { ';' });
                            if (userNames.Length > 0)
                            {
                                foreach (string userName in userNames)
                                {
                                    if (userName.Trim().Length > 0)
                                    {
                                        if (accessRightText.Length > 0)
                                        {
                                            accessRightText += ";";
                                        }

                                        accessRightText += userName.Trim() + "!" + textBox_FileAccessFlags.Text;
                                    }
                                }
                            }
                        }

                        break;
                    }

                case AccessRightType.ProccessIdRight:
                    {
                        if (textBox_ProcessId.Text.Trim().Length > 0)
                        {
                            string[] processIds = textBox_ProcessId.Text.Trim().Split(new char[] { ';' });
                            if (processIds.Length > 0)
                            {
                                foreach (string processId in processIds)
                                {
                                    if (processId.Trim().Length > 0)
                                    {
                                        if (accessRightText.Length > 0)
                                        {
                                            accessRightText += ";";
                                        }

                                        accessRightText += processId.Trim() + "!" + textBox_FileAccessFlags.Text;
                                    }
                                }
                            }
                        }

                        break;
                    }
            }

        }


        private void SetCheckBoxValue()
        {

            uint accessFlags = uint.Parse(textBox_FileAccessFlags.Text);
    
            if ((accessFlags & (uint)FilterAPI.AccessFlag.ALLOW_FILE_DELETE) > 0)
            {
                checkBox_AllowDelete.Checked = true;
            }
            else
            {
                checkBox_AllowDelete.Checked = false;
            }

            if ((accessFlags & (uint)FilterAPI.AccessFlag.ALLOW_FILE_RENAME) > 0)
            {
                checkBox_AllowRename.Checked = true;
            }
            else
            {
                checkBox_AllowRename.Checked = false;
            }

            if ((accessFlags & (uint)FilterAPI.AccessFlag.ALLOW_WRITE_ACCESS) > 0 )
            {
                checkBox_Write.Checked = true;
            }
            else
            {
                checkBox_Write.Checked = false;
            }

            if ((accessFlags & (uint)FilterAPI.AccessFlag.ALLOW_READ_ACCESS) > 0)
            {
                checkBox_Read.Checked = true;
            }
            else
            {
                checkBox_Read.Checked = false;
            }

            if ((accessFlags & (uint)FilterAPI.AccessFlag.ALLOW_QUERY_INFORMATION_ACCESS) > 0)
            {
                checkBox_QueryInfo.Checked = true;
            }
            else
            {
                checkBox_QueryInfo.Checked = false;
            }

            if ( (accessFlags & (uint)FilterAPI.AccessFlag.ALLOW_SET_INFORMATION) > 0 )
            {
                checkBox_SetInfo.Checked = true;
            }
            else
            {
                checkBox_SetInfo.Checked = false;
            }

            if ((accessFlags & (uint)FilterAPI.AccessFlag.ALLOW_OPEN_WITH_CREATE_OR_OVERWRITE_ACCESS) > 0)
            {
                checkBox_Creation.Checked = true;
            }
            else
            {
                checkBox_Creation.Checked = false;
            }

            if ((accessFlags & (uint)FilterAPI.AccessFlag.ALLOW_QUERY_SECURITY_ACCESS) > 0)
            {
                checkBox_QuerySecurity.Checked = true;
            }
            else
            {
                checkBox_QuerySecurity.Checked = false;
            }

            if ((accessFlags & (uint)FilterAPI.AccessFlag.ALLOW_SET_SECURITY_ACCESS) > 0)
            {
                checkBox_SetSecurity.Checked = true;
            }
            else
            {
                checkBox_SetSecurity.Checked = false;
            }

            if ((accessFlags & (uint)FilterAPI.AccessFlag.ALLOW_ALL_SAVE_AS) > 0)
            {
                checkBox_AllowSaveAs.Checked = true;
            }
            else
            {
                checkBox_AllowSaveAs.Checked = false;
            }

            if ((accessFlags & (uint)FilterAPI.AccessFlag.ALLOW_COPY_PROTECTED_FILES_OUT) > 0)
            {
                checkBox_AllowCopyout.Checked = true;
            }
            else
            {
                checkBox_AllowCopyout.Checked = false;
            }

            if ((accessFlags & (uint)FilterAPI.AccessFlag.ALLOW_READ_ENCRYPTED_FILES) > 0)
            {
                checkBox_AllowReadEncryptedFiles.Checked = true;
            }
            else
            {
                checkBox_AllowReadEncryptedFiles.Checked = false;
            }
        }

        private void button_FileAccessFlags_Click(object sender, EventArgs e)
        {
            OptionForm optionForm = new OptionForm(OptionForm.OptionType.Access_Flag, textBox_FileAccessFlags.Text);

            if (optionForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (optionForm.AccessFlags > 0)
                {
                    textBox_FileAccessFlags.Text = optionForm.AccessFlags.ToString();
                }
                else
                {
                    //if the accessFlag is 0, it is exclude filter rule,this is not what we want, so we need to include this flag.
                    textBox_FileAccessFlags.Text = ((uint)FilterAPI.AccessFlag.LEAST_ACCESS_FLAG).ToString();
                }

                SetCheckBoxValue();
            }
        }

        private void checkBox_Read_CheckedChanged(object sender, EventArgs e)
        {
            uint accessFlags = uint.Parse(textBox_FileAccessFlags.Text.Trim());
            if (checkBox_Read.Checked)
            {
                accessFlags |= (uint)FilterAPI.AccessFlag.ALLOW_READ_ACCESS;
            }
            else
            {
                accessFlags &= ~(uint)FilterAPI.AccessFlag.ALLOW_READ_ACCESS;
            }

            textBox_FileAccessFlags.Text = accessFlags.ToString();
        }

        private void checkBox_Write_CheckedChanged(object sender, EventArgs e)
        {
            uint accessFlags = uint.Parse(textBox_FileAccessFlags.Text.Trim());
            if (checkBox_Write.Checked)
            {
                accessFlags |= (uint)FilterAPI.AccessFlag.ALLOW_WRITE_ACCESS;
            }
            else
            {
                accessFlags &= ~(uint)FilterAPI.AccessFlag.ALLOW_WRITE_ACCESS;
            }

            textBox_FileAccessFlags.Text = accessFlags.ToString();
        }

        private void checkBox_Creation_CheckedChanged(object sender, EventArgs e)
        {
            uint accessFlags = uint.Parse(textBox_FileAccessFlags.Text.Trim());
            if (checkBox_Creation.Checked)
            {
                accessFlags |= (uint)FilterAPI.AccessFlag.ALLOW_OPEN_WITH_CREATE_OR_OVERWRITE_ACCESS;
            }
            else
            {
                accessFlags &= ~(uint)FilterAPI.AccessFlag.ALLOW_OPEN_WITH_CREATE_OR_OVERWRITE_ACCESS;
            }

            textBox_FileAccessFlags.Text = accessFlags.ToString();
        }



        private void checkBox_QueryInfo_CheckedChanged(object sender, EventArgs e)
        {
            uint accessFlags = uint.Parse(textBox_FileAccessFlags.Text.Trim());
            if (checkBox_QueryInfo.Checked)
            {
                accessFlags |= (uint)FilterAPI.AccessFlag.ALLOW_QUERY_INFORMATION_ACCESS;
            }
            else
            {
                accessFlags &= ~(uint)FilterAPI.AccessFlag.ALLOW_QUERY_INFORMATION_ACCESS;
            }

            textBox_FileAccessFlags.Text = accessFlags.ToString();
        }

        private void checkBox_SetInfo_CheckedChanged(object sender, EventArgs e)
        {
            uint accessFlags = uint.Parse(textBox_FileAccessFlags.Text.Trim());
            if (checkBox_SetInfo.Checked)
            {
                accessFlags |= (uint)FilterAPI.AccessFlag.ALLOW_SET_INFORMATION;
            }
            else
            {
                accessFlags &= ~(uint)FilterAPI.AccessFlag.ALLOW_SET_INFORMATION;
            }

            textBox_FileAccessFlags.Text = accessFlags.ToString();
        }

        private void checkBox_AllowRename_CheckedChanged(object sender, EventArgs e)
        {
            uint accessFlags = uint.Parse(textBox_FileAccessFlags.Text.Trim());
            if (checkBox_AllowRename.Checked)
            {
                accessFlags |= (uint)FilterAPI.AccessFlag.ALLOW_FILE_RENAME;
            }
            else
            {
                accessFlags &= ~(uint)FilterAPI.AccessFlag.ALLOW_FILE_RENAME;
            }

            textBox_FileAccessFlags.Text = accessFlags.ToString();
        }

        private void checkBox_AllowDelete_CheckedChanged(object sender, EventArgs e)
        {
            uint accessFlags = uint.Parse(textBox_FileAccessFlags.Text.Trim());
            if (checkBox_AllowDelete.Checked)
            {
                accessFlags |= (uint)FilterAPI.AccessFlag.ALLOW_FILE_DELETE;
            }
            else
            {
                accessFlags &= ~(uint)FilterAPI.AccessFlag.ALLOW_FILE_DELETE;
            }

            textBox_FileAccessFlags.Text = accessFlags.ToString();
        }

        private void checkBox_QuerySecurity_CheckedChanged(object sender, EventArgs e)
        {
            uint accessFlags = uint.Parse(textBox_FileAccessFlags.Text.Trim());
            if (checkBox_QuerySecurity.Checked)
            {
                accessFlags |= (uint)FilterAPI.AccessFlag.ALLOW_QUERY_SECURITY_ACCESS;
            }
            else
            {
                accessFlags &= ~(uint)FilterAPI.AccessFlag.ALLOW_QUERY_SECURITY_ACCESS;
            }

            textBox_FileAccessFlags.Text = accessFlags.ToString();
        }

        private void checkBox_SetSecurity_CheckedChanged(object sender, EventArgs e)
        {
            uint accessFlags = uint.Parse(textBox_FileAccessFlags.Text.Trim());
            if (checkBox_SetSecurity.Checked)
            {
                accessFlags |= (uint)FilterAPI.AccessFlag.ALLOW_SET_SECURITY_ACCESS;
            }
            else
            {
                accessFlags &= ~(uint)FilterAPI.AccessFlag.ALLOW_SET_SECURITY_ACCESS;
            }

            textBox_FileAccessFlags.Text = accessFlags.ToString();
        }

        private void checkBox_AllowSaveAs_CheckedChanged(object sender, EventArgs e)
        {
            uint accessFlags = uint.Parse(textBox_FileAccessFlags.Text.Trim());
            if (checkBox_AllowSaveAs.Checked)
            {
                accessFlags |= (uint)FilterAPI.AccessFlag.ALLOW_ALL_SAVE_AS;
            }
            else
            {
                accessFlags &= ~(uint)FilterAPI.AccessFlag.ALLOW_ALL_SAVE_AS;
            }

            textBox_FileAccessFlags.Text = accessFlags.ToString();
        }

        private void checkBox_AllowCopyout_CheckedChanged(object sender, EventArgs e)
        {
            uint accessFlags = uint.Parse(textBox_FileAccessFlags.Text.Trim());
            if (checkBox_AllowCopyout.Checked)
            {
                accessFlags |= (uint)FilterAPI.AccessFlag.ALLOW_COPY_PROTECTED_FILES_OUT;
            }
            else
            {
                accessFlags &= ~(uint)FilterAPI.AccessFlag.ALLOW_COPY_PROTECTED_FILES_OUT;
            }

            textBox_FileAccessFlags.Text = accessFlags.ToString();
        }

        private void checkBox_AllowReadEncryptedFiles_CheckedChanged(object sender, EventArgs e)
        {
            uint accessFlags = uint.Parse(textBox_FileAccessFlags.Text.Trim());
            if (checkBox_AllowReadEncryptedFiles.Checked)
            {
                accessFlags |= (uint)FilterAPI.AccessFlag.ALLOW_READ_ENCRYPTED_FILES;
            }
            else
            {
                accessFlags &= ~(uint)FilterAPI.AccessFlag.ALLOW_READ_ENCRYPTED_FILES;
            }

            textBox_FileAccessFlags.Text = accessFlags.ToString();
        }

        private void button_ProcessId_Click(object sender, EventArgs e)
        {
            OptionForm optionForm = new OptionForm(OptionForm.OptionType.ProccessId, textBox_ProcessId.Text);

            if (optionForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (optionForm.ProcessId.Length > 0)
                {
                    textBox_ProcessId.Text = optionForm.ProcessId;
                }
            }
        }

 
    }
}
