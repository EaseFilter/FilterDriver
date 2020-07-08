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
    public partial class ControlFilterRuleSettigs : Form
    {
        public FilterRule filterRule = new FilterRule();

        public ControlFilterRuleSettigs()
        {
        }

        public ControlFilterRuleSettigs(FilterRule _filterRule)
        {
            InitializeComponent();
            filterRule = _filterRule;

            textBox_FileAccessFlags.Text = filterRule.AccessFlag.ToString();
            textBox_PassPhrase.Text = filterRule.EncryptionPassPhrase;
            textBox_HiddenFilterMask.Text = filterRule.HiddenFileFilterMasks;
            textBox_ReparseFileFilterMask.Text = filterRule.ReparseFileFilterMasks;
            textBox_ControlIO.Text = filterRule.ControlIO.ToString();
            checkBox_EnableProtectionInBootTime.Checked = filterRule.IsResident;

            textBox_ProcessRights.Text = filterRule.ProcessRights;
            textBox_ProcessIdRights.Text = filterRule.ProcessIdRights;

            textBox_UserRights.Text = filterRule.UserRights;

            textBox_FilterDesiredAccess.Text = filterRule.FilterDesiredAccess.ToString();
            textBox_FilterDisposition.Text = filterRule.FilterDisposition.ToString();
            textBox_FilterCreateOptions.Text = filterRule.FilterCreateOptions.ToString();


            SetCheckBoxValue();

            if (filterRule.EncryptionKeySize == 16)
            {
                radioButton_128.Checked = true;
            }
            else if (filterRule.EncryptionKeySize == 24)
            {
                radioButton_196.Checked = true;
            }
            else
            {
                radioButton_256.Checked = true;
            }
        }

        private void SetCheckBoxValue()
        {

            uint accessFlags = uint.Parse(textBox_FileAccessFlags.Text);

            if ((accessFlags & (uint)FilterAPI.AccessFlag.ENABLE_FILE_ENCRYPTION_RULE) > 0 )
            {
                checkBox_Encryption.Checked = true;
                textBox_PassPhrase.ReadOnly = false;
            }
            else
            {
                checkBox_Encryption.Checked = false;
            }

            if ((accessFlags & (uint)FilterAPI.AccessFlag.ALLOW_FILE_ACCESS_FROM_NETWORK) > 0)
            {
                checkBox_AllowRemoteAccess.Checked = true;
            }
            else
            {
                checkBox_AllowRemoteAccess.Checked = false;
            }

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

            if ((accessFlags & (uint)FilterAPI.AccessFlag.ALLOW_WRITE_ACCESS) > 0
                && (accessFlags & (uint)FilterAPI.AccessFlag.ALLOW_SET_INFORMATION) > 0)
            {
                checkBox_AllowChange.Checked = true;
            }
            else
            {
                checkBox_AllowChange.Checked = false;
            }

            if ((accessFlags & (uint)FilterAPI.AccessFlag.ALLOW_OPEN_WITH_CREATE_OR_OVERWRITE_ACCESS) > 0)
            {
                checkBox_AllowNewFileCreation.Checked = true;
            }
            else
            {
                checkBox_AllowNewFileCreation.Checked = false;
            }

            if ((accessFlags & (uint)FilterAPI.AccessFlag.ALLOW_FILE_MEMORY_MAPPED) > 0)
            {
                checkBox_AllowMemoryMapped.Checked = true;
            }
            else
            {
                checkBox_AllowMemoryMapped.Checked = false;
            }

            if ((accessFlags & (uint)FilterAPI.AccessFlag.ALLOW_SET_SECURITY_ACCESS) > 0)
            {
                checkBox_AllowSetSecurity.Checked = true;
            }
            else
            {
                checkBox_AllowSetSecurity.Checked = false;
            }


            if ((accessFlags & (uint)FilterAPI.AccessFlag.ALLOW_ENCRYPT_NEW_FILE) > 0)
            {
                checkBox_AllowEncryptNewFile.Checked = true;
            }
            else
            {
                checkBox_AllowEncryptNewFile.Checked = false;
            }

            if ((accessFlags & (uint)FilterAPI.AccessFlag.ALLOW_COPY_PROTECTED_FILES_OUT) > 0)
            {
                checkBox_AllowCopyOut.Checked = true;
            }
            else
            {
                checkBox_AllowCopyOut.Checked = false;
            }

            if ((accessFlags & (uint)FilterAPI.AccessFlag.ALLOW_READ_ENCRYPTED_FILES) > 0)
            {
                checkBox_AllowReadEncryptedFiles.Checked = true;
            }
            else
            {
                checkBox_AllowReadEncryptedFiles.Checked = false;
            }


            if ((accessFlags & (uint)FilterAPI.AccessFlag.DISABLE_ENCRYPT_DATA_ON_READ) > 0)
            {
                checkBox_EncryptOnRead.Checked = false;
            }
            else
            {
                checkBox_EncryptOnRead.Checked = true;
            }
        }

        private void button_SaveControlSettings_Click(object sender, EventArgs e)
        {
            string encryptionPassPhrase = string.Empty;
            uint accessFlags = uint.Parse(textBox_FileAccessFlags.Text);

            if (checkBox_Encryption.Checked)
            {
                filterRule.EncryptMethod = (int)FilterAPI.EncryptionMethod.ENCRYPT_FILE_WITH_SAME_KEY_AND_UNIQUE_IV;
                encryptionPassPhrase = textBox_PassPhrase.Text;

                //enable encryption for this filter rule.
                accessFlags = accessFlags | (uint)FilterAPI.AccessFlag.ENABLE_FILE_ENCRYPTION_RULE;
            }

            if (textBox_HiddenFilterMask.Text.Trim().Length > 0)
            {
                //enable hidden filter mask for this filter rule.
                accessFlags = accessFlags | (uint)FilterAPI.AccessFlag.ENABLE_HIDE_FILES_IN_DIRECTORY_BROWSING;
            }

            if (radioButton_128.Checked)
            {
                filterRule.EncryptionKeySize = 16;
            }
            else if (radioButton_196.Checked)
            {
                filterRule.EncryptionKeySize = 24;
            }
            else
            {
                filterRule.EncryptionKeySize = 32;
            }
            filterRule.EncryptionPassPhrase = encryptionPassPhrase;
            filterRule.HiddenFileFilterMasks = textBox_HiddenFilterMask.Text;
            filterRule.ReparseFileFilterMasks = textBox_ReparseFileFilterMask.Text;
            filterRule.AccessFlag = accessFlags;
            filterRule.ControlIO = uint.Parse(textBox_ControlIO.Text);
            filterRule.IsResident = checkBox_EnableProtectionInBootTime.Checked;
            filterRule.UserRights = textBox_UserRights.Text;
            filterRule.ProcessRights = textBox_ProcessRights.Text;
            filterRule.ProcessIdRights = textBox_ProcessIdRights.Text;
            filterRule.Id = GlobalConfig.GetFilterRuleId();
            filterRule.FilterDesiredAccess = uint.Parse(textBox_FilterDesiredAccess.Text);
            filterRule.FilterDisposition = uint.Parse(textBox_FilterDisposition.Text);
            filterRule.FilterCreateOptions = uint.Parse(textBox_FilterCreateOptions.Text);

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


        private void button_RegisterControlIO_Click(object sender, EventArgs e)
        {
            OptionForm optionForm = new OptionForm(OptionForm.OptionType.Register_Request, textBox_ControlIO.Text);

            if (optionForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox_ControlIO.Text = optionForm.RequestRegistration.ToString();
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


        private void button_AddProcessRights_Click(object sender, EventArgs e)
        {
            Form_AccessRights accessRightsForm = new Form_AccessRights(Form_AccessRights.AccessRightType.ProcessNameRight);

            if (accessRightsForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (textBox_ProcessRights.Text.Trim().Length > 0)
                {
                    textBox_ProcessRights.Text += ";" + accessRightsForm.accessRightText;
                }
                else
                {
                    textBox_ProcessRights.Text = accessRightsForm.accessRightText;
                }
            }
        }

        private void button_AddProcessIdRights_Click(object sender, EventArgs e)
        {
            Form_AccessRights accessRightsForm = new Form_AccessRights(Form_AccessRights.AccessRightType.ProccessIdRight);

            if (accessRightsForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (textBox_ProcessIdRights.Text.Trim().Length > 0)
                {
                    textBox_ProcessIdRights.Text += ";" + accessRightsForm.accessRightText;
                }
                else
                {
                    textBox_ProcessIdRights.Text = accessRightsForm.accessRightText;
                }
            }

        }   


        private void button_AddUserRights_Click(object sender, EventArgs e)
        {
            Form_AccessRights accessRightsForm = new Form_AccessRights(Form_AccessRights.AccessRightType.UserNameRight);

            if (accessRightsForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (textBox_UserRights.Text.Trim().Length > 0)
                {
                    textBox_UserRights.Text += ";" + accessRightsForm.accessRightText;
                }
                else
                {
                    textBox_UserRights.Text = accessRightsForm.accessRightText;
                }
            }
        }


        private void checkBox_Encryption_CheckedChanged(object sender, EventArgs e)
        {
            uint accessFlags = uint.Parse(textBox_FileAccessFlags.Text);

            if (checkBox_Encryption.Checked)
            {
                textBox_PassPhrase.ReadOnly = false;
                accessFlags = accessFlags | ((uint)FilterAPI.AccessFlag.ENABLE_FILE_ENCRYPTION_RULE);
            }
            else
            {
                textBox_PassPhrase.ReadOnly = true;
                accessFlags = accessFlags & ((uint)~FilterAPI.AccessFlag.ENABLE_FILE_ENCRYPTION_RULE);
            }

            textBox_FileAccessFlags.Text = accessFlags.ToString();
        }

        private void checkBox_AllowDelete_CheckedChanged(object sender, EventArgs e)
        {

            uint accessFlags = uint.Parse(textBox_FileAccessFlags.Text);

            if (!checkBox_AllowDelete.Checked)
            {
                accessFlags = accessFlags & ((uint)~FilterAPI.AccessFlag.ALLOW_FILE_DELETE);
            }
            else
            {
                accessFlags = accessFlags | ((uint)FilterAPI.AccessFlag.ALLOW_FILE_DELETE);
            }

            textBox_FileAccessFlags.Text = accessFlags.ToString();
        }

        private void checkBox_AllowChange_CheckedChanged(object sender, EventArgs e)
        {
            uint accessFlags = uint.Parse(textBox_FileAccessFlags.Text);
            if (!checkBox_AllowChange.Checked)
            {
                accessFlags = accessFlags & ((uint)~FilterAPI.AccessFlag.ALLOW_WRITE_ACCESS) & ((uint)~FilterAPI.AccessFlag.ALLOW_SET_INFORMATION);
            }
            else
            {
                accessFlags = accessFlags | ((uint)FilterAPI.AccessFlag.ALLOW_WRITE_ACCESS) | ((uint)FilterAPI.AccessFlag.ALLOW_SET_INFORMATION);
            }

            textBox_FileAccessFlags.Text = accessFlags.ToString();
        }

        private void checkBox_AllowRename_CheckedChanged(object sender, EventArgs e)
        {
            uint accessFlags = uint.Parse(textBox_FileAccessFlags.Text);

            if (!checkBox_AllowRename.Checked)
            {
                accessFlags = accessFlags & ((uint)~FilterAPI.AccessFlag.ALLOW_FILE_RENAME);
            }
            else
            {
                accessFlags = accessFlags | ((uint)FilterAPI.AccessFlag.ALLOW_FILE_RENAME);
            }

            textBox_FileAccessFlags.Text = accessFlags.ToString();
        }

        private void checkBox_AllowRemoteAccess_CheckedChanged(object sender, EventArgs e)
        {
            uint accessFlags = uint.Parse(textBox_FileAccessFlags.Text);

            if (!checkBox_AllowRemoteAccess.Checked)
            {
                accessFlags = accessFlags & ((uint)~FilterAPI.AccessFlag.ALLOW_FILE_ACCESS_FROM_NETWORK);
            }
            else
            {
                accessFlags = accessFlags | ((uint)FilterAPI.AccessFlag.ALLOW_FILE_ACCESS_FROM_NETWORK);
            }

            textBox_FileAccessFlags.Text = accessFlags.ToString();
        }

        private void checkBox_AllowNewFileCreation_CheckedChanged(object sender, EventArgs e)
        {
            uint accessFlags = uint.Parse(textBox_FileAccessFlags.Text);

            if (!checkBox_AllowNewFileCreation.Checked)
            {
                accessFlags = accessFlags & ((uint)~FilterAPI.AccessFlag.ALLOW_OPEN_WITH_CREATE_OR_OVERWRITE_ACCESS);
            }
            else
            {
                accessFlags = accessFlags | ((uint)FilterAPI.AccessFlag.ALLOW_OPEN_WITH_CREATE_OR_OVERWRITE_ACCESS);
            }

            textBox_FileAccessFlags.Text = accessFlags.ToString();
        }

        private void checkBox_AllowMemoryMapped_CheckedChanged(object sender, EventArgs e)
        {
            uint accessFlags = uint.Parse(textBox_FileAccessFlags.Text);

            if (!checkBox_AllowMemoryMapped.Checked)
            {
                accessFlags = accessFlags & ((uint)~FilterAPI.AccessFlag.ALLOW_FILE_MEMORY_MAPPED);
            }
            else
            {
                accessFlags = accessFlags | ((uint)FilterAPI.AccessFlag.ALLOW_FILE_MEMORY_MAPPED);
            }

            textBox_FileAccessFlags.Text = accessFlags.ToString();
        }

        private void checkBox_AllowSetSecurity_CheckedChanged(object sender, EventArgs e)
        {
            uint accessFlags = uint.Parse(textBox_FileAccessFlags.Text);

            if (!checkBox_AllowSetSecurity.Checked)
            {
                accessFlags = accessFlags & ((uint)~FilterAPI.AccessFlag.ALLOW_SET_SECURITY_ACCESS);
            }
            else
            {
                accessFlags = accessFlags | ((uint)FilterAPI.AccessFlag.ALLOW_SET_SECURITY_ACCESS);
            }

            textBox_FileAccessFlags.Text = accessFlags.ToString();
        }


        private void checkBox_AllowEncryptNewFile_CheckedChanged(object sender, EventArgs e)
        {
            uint accessFlags = uint.Parse(textBox_FileAccessFlags.Text);

            if (!checkBox_AllowEncryptNewFile.Checked)
            {
                accessFlags = accessFlags & ((uint)~FilterAPI.AccessFlag.ALLOW_ENCRYPT_NEW_FILE);
            }
            else
            {
                accessFlags = accessFlags | ((uint)FilterAPI.AccessFlag.ALLOW_ENCRYPT_NEW_FILE);
            }

            textBox_FileAccessFlags.Text = accessFlags.ToString();
        }

        private void checkBox_AllowCopyOut_CheckedChanged(object sender, EventArgs e)
        {
            uint accessFlags = uint.Parse(textBox_FileAccessFlags.Text);

            if (!checkBox_AllowCopyOut.Checked)
            {
                accessFlags = accessFlags & ((uint)~FilterAPI.AccessFlag.ALLOW_COPY_PROTECTED_FILES_OUT);
            }
            else
            {
                accessFlags = accessFlags | ((uint)FilterAPI.AccessFlag.ALLOW_COPY_PROTECTED_FILES_OUT);
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


        private void checkBox_EncryptOnRead_CheckedChanged(object sender, EventArgs e)
        {
            uint accessFlags = uint.Parse(textBox_FileAccessFlags.Text.Trim());
            if (checkBox_EncryptOnRead.Checked)
            {
                if (!checkBox_Encryption.Checked)
                {
                    MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                    MessageBox.Show("The encryption is not enabled, enable it first.", "EncryptOnRead", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                accessFlags &= ~(uint)FilterAPI.AccessFlag.DISABLE_ENCRYPT_DATA_ON_READ;
            }
            else
            {
                accessFlags |= (uint)FilterAPI.AccessFlag.DISABLE_ENCRYPT_DATA_ON_READ;
            }

            textBox_FileAccessFlags.Text = accessFlags.ToString();
        }


        private void textBox_FileAccessFlags_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("This is the access control flags,uncheck the access right will deny the associated access.", textBox_FileAccessFlags);
        }

        private void textBox_ProcessRights_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("You can set the specific processes access rights,to authorize or deny the file access to these processes.", textBox_FileAccessFlags);
        }

        private void textBox_UserRights_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("You can set the specific users access rights,to authorize or deny the file access to these users.", textBox_UserRights);
        }

        private void textBox_ControlIO_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Register the callback I/O, you can modify, allow or block the IO access in the callback function.", textBox_ControlIO);
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

        private void textBox_ReparseFileFilterMask_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("You can reparse the file open to another file path with this setting.", textBox_ReparseFileFilterMask);
        }

        private void textBox_HiddenFilterMask_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("You can hide the file names when users browse the folder with this setting.", textBox_HiddenFilterMask);
        }

        private void textBox_PassPhrase_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Enable the transparent file encryption with this passphrase when the accessFlag encryption was enabled.", textBox_PassPhrase);
        }

        private void checkBox_EncryptOnRead_MouseHover(object sender, EventArgs e)
        {
            string info = "If encryption on read was enabled, the process always will get the encrypted data." + Environment.NewLine;
            info += "It doesn't matter if the file was encrypted or not on disk." + Environment.NewLine;
            info += "Your file will be automatically encrypted when it was sent out from your computer.";
            toolTip1.Show(info, checkBox_EncryptOnRead);
        }

        private void checkBox_AllowReadEncryptedFiles_MouseHover(object sender, EventArgs e)
        {
            string info = "If your file was encrypted on disk, and the process is not allowed to read it," + Environment.NewLine;
            info += "the process will get the encrypted data instead of the descrypted data." + Environment.NewLine;
            info += "If you want to copy or backup the encrypted file, uncheck this box for the process.";
            toolTip1.Show(info, checkBox_AllowReadEncryptedFiles);
        }

        private void checkBox_AllowCopyOut_MouseHover(object sender, EventArgs e)
        {
            string info = "If the process read the proected file, and the file was not allowed to copy out," + Environment.NewLine;
            info += "the process will be blocked to create new file anywhere except the current folder." + Environment.NewLine;
            toolTip1.Show(info, checkBox_AllowCopyOut);
        }

        private void checkBox_AllowEncryptNewFile_MouseHover(object sender, EventArgs e)
        {
            string info = "If the encryption for new file is not enabled, the new created file won't be encrypted automatically." + Environment.NewLine;
            info += "If you want to automatically encrypt the new file, you need to check this box." + Environment.NewLine;
            toolTip1.Show(info, checkBox_AllowEncryptNewFile);
        }

      

    
    }
}
