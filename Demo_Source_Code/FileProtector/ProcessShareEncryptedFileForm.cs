///////////////////////////////////////////////////////////////////////////////
//
//    (C) Copyright 2012 EaseFilter Technologies Inc.
//    All Rights Reserved
//
//    This software is part of a licensed software product and may
//    only be used or copied in accordance with the terms of that license.
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

using EaseFilter.CommonObjects;

namespace FileProtector
{
    public partial class ProcessShareEncryptedFileForm : Form
    {
        public ProcessShareEncryptedFileForm()
        {
            InitializeComponent();
        }

        private void button_Start_Click(object sender, EventArgs e)
        {
            string lastError = string.Empty;

            if (!EncryptionHandler.ConvertFileToFilterDriverAwareEncryptFile(textBox_FileName.Text, textBox_PassPhrase.Text, out lastError))
            {
                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                MessageBox.Show("Process share encrypted file failed with error:" + lastError, "Process share encrypted file", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }
            else
            {
                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                MessageBox.Show("Process share encrypted file succeeded.", "Process share encrypted file", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void checkBox_DisplayPassword_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_DisplayPassword.Checked)
            {
                textBox_PassPhrase.UseSystemPasswordChar = false;
            }
            else
            {
                textBox_PassPhrase.UseSystemPasswordChar = true;
            }
        }

        private void button_OpenFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox_FileName.Text = openFileDialog.FileName;
            }
        }

    
    }
}
