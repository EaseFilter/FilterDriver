namespace EaseFilter.CommonObjects
{
    partial class Form_AccessRights
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button_Add = new System.Windows.Forms.Button();
            this.groupBox_ProcessName = new System.Windows.Forms.GroupBox();
            this.textBox_ProcessName = new System.Windows.Forms.TextBox();
            this.label_AccessFlags = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBox_AllowReadEncryptedFiles = new System.Windows.Forms.CheckBox();
            this.checkBox_AllowCopyout = new System.Windows.Forms.CheckBox();
            this.checkBox_AllowSaveAs = new System.Windows.Forms.CheckBox();
            this.checkBox_SetSecurity = new System.Windows.Forms.CheckBox();
            this.checkBox_QueryInfo = new System.Windows.Forms.CheckBox();
            this.checkBox_Read = new System.Windows.Forms.CheckBox();
            this.checkBox_QuerySecurity = new System.Windows.Forms.CheckBox();
            this.checkBox_SetInfo = new System.Windows.Forms.CheckBox();
            this.checkBox_Write = new System.Windows.Forms.CheckBox();
            this.checkBox_AllowDelete = new System.Windows.Forms.CheckBox();
            this.checkBox_AllowRename = new System.Windows.Forms.CheckBox();
            this.checkBox_Creation = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_FileAccessFlags = new System.Windows.Forms.TextBox();
            this.button_FileAccessFlags = new System.Windows.Forms.Button();
            this.groupBox_UserName = new System.Windows.Forms.GroupBox();
            this.textBox_UserName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox_ProcessId = new System.Windows.Forms.GroupBox();
            this.button_ProcessId = new System.Windows.Forms.Button();
            this.textBox_ProcessId = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox_ProcessName.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox_UserName.SuspendLayout();
            this.groupBox_ProcessId.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_Add
            // 
            this.button_Add.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button_Add.Location = new System.Drawing.Point(485, 411);
            this.button_Add.Name = "button_Add";
            this.button_Add.Size = new System.Drawing.Size(75, 23);
            this.button_Add.TabIndex = 25;
            this.button_Add.Text = "Add";
            this.button_Add.UseVisualStyleBackColor = true;
            this.button_Add.Click += new System.EventHandler(this.button_Add_Click);
            // 
            // groupBox_ProcessName
            // 
            this.groupBox_ProcessName.Controls.Add(this.textBox_ProcessName);
            this.groupBox_ProcessName.Controls.Add(this.label_AccessFlags);
            this.groupBox_ProcessName.Location = new System.Drawing.Point(43, 19);
            this.groupBox_ProcessName.Name = "groupBox_ProcessName";
            this.groupBox_ProcessName.Size = new System.Drawing.Size(517, 48);
            this.groupBox_ProcessName.TabIndex = 26;
            this.groupBox_ProcessName.TabStop = false;
            this.groupBox_ProcessName.Visible = false;
            // 
            // textBox_ProcessName
            // 
            this.textBox_ProcessName.Location = new System.Drawing.Point(149, 16);
            this.textBox_ProcessName.Name = "textBox_ProcessName";
            this.textBox_ProcessName.Size = new System.Drawing.Size(298, 20);
            this.textBox_ProcessName.TabIndex = 27;
            this.textBox_ProcessName.Text = "notepad.exe;   cmd.exe;";
            // 
            // label_AccessFlags
            // 
            this.label_AccessFlags.AutoSize = true;
            this.label_AccessFlags.Location = new System.Drawing.Point(9, 19);
            this.label_AccessFlags.Name = "label_AccessFlags";
            this.label_AccessFlags.Size = new System.Drawing.Size(91, 15);
            this.label_AccessFlags.TabIndex = 28;
            this.label_AccessFlags.Text = "Process Name ";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBox_AllowReadEncryptedFiles);
            this.groupBox2.Controls.Add(this.checkBox_AllowCopyout);
            this.groupBox2.Controls.Add(this.checkBox_AllowSaveAs);
            this.groupBox2.Controls.Add(this.checkBox_SetSecurity);
            this.groupBox2.Controls.Add(this.checkBox_QueryInfo);
            this.groupBox2.Controls.Add(this.checkBox_Read);
            this.groupBox2.Controls.Add(this.checkBox_QuerySecurity);
            this.groupBox2.Controls.Add(this.checkBox_SetInfo);
            this.groupBox2.Controls.Add(this.checkBox_Write);
            this.groupBox2.Controls.Add(this.checkBox_AllowDelete);
            this.groupBox2.Controls.Add(this.checkBox_AllowRename);
            this.groupBox2.Controls.Add(this.checkBox_Creation);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.textBox_FileAccessFlags);
            this.groupBox2.Controls.Add(this.button_FileAccessFlags);
            this.groupBox2.Location = new System.Drawing.Point(43, 188);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(517, 204);
            this.groupBox2.TabIndex = 76;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Acess Rights";
            // 
            // checkBox_AllowReadEncryptedFiles
            // 
            this.checkBox_AllowReadEncryptedFiles.AutoSize = true;
            this.checkBox_AllowReadEncryptedFiles.Checked = true;
            this.checkBox_AllowReadEncryptedFiles.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_AllowReadEncryptedFiles.Location = new System.Drawing.Point(148, 146);
            this.checkBox_AllowReadEncryptedFiles.Name = "checkBox_AllowReadEncryptedFiles";
            this.checkBox_AllowReadEncryptedFiles.Size = new System.Drawing.Size(195, 19);
            this.checkBox_AllowReadEncryptedFiles.TabIndex = 49;
            this.checkBox_AllowReadEncryptedFiles.Text = "Allow encrypted file being read";
            this.checkBox_AllowReadEncryptedFiles.UseVisualStyleBackColor = true;
            this.checkBox_AllowReadEncryptedFiles.CheckedChanged += new System.EventHandler(this.checkBox_AllowReadEncryptedFiles_CheckedChanged);
            // 
            // checkBox_AllowCopyout
            // 
            this.checkBox_AllowCopyout.AutoSize = true;
            this.checkBox_AllowCopyout.Checked = true;
            this.checkBox_AllowCopyout.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_AllowCopyout.Location = new System.Drawing.Point(333, 146);
            this.checkBox_AllowCopyout.Name = "checkBox_AllowCopyout";
            this.checkBox_AllowCopyout.Size = new System.Drawing.Size(177, 19);
            this.checkBox_AllowCopyout.TabIndex = 48;
            this.checkBox_AllowCopyout.Text = "Allow files being copied out";
            this.checkBox_AllowCopyout.UseVisualStyleBackColor = true;
            this.checkBox_AllowCopyout.CheckedChanged += new System.EventHandler(this.checkBox_AllowCopyout_CheckedChanged);
            // 
            // checkBox_AllowSaveAs
            // 
            this.checkBox_AllowSaveAs.AutoSize = true;
            this.checkBox_AllowSaveAs.Checked = true;
            this.checkBox_AllowSaveAs.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_AllowSaveAs.Location = new System.Drawing.Point(333, 123);
            this.checkBox_AllowSaveAs.Name = "checkBox_AllowSaveAs";
            this.checkBox_AllowSaveAs.Size = new System.Drawing.Size(162, 19);
            this.checkBox_AllowSaveAs.TabIndex = 47;
            this.checkBox_AllowSaveAs.Text = "Allow file being saved as";
            this.checkBox_AllowSaveAs.UseVisualStyleBackColor = true;
            this.checkBox_AllowSaveAs.CheckedChanged += new System.EventHandler(this.checkBox_AllowSaveAs_CheckedChanged);
            // 
            // checkBox_SetSecurity
            // 
            this.checkBox_SetSecurity.AutoSize = true;
            this.checkBox_SetSecurity.Checked = true;
            this.checkBox_SetSecurity.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_SetSecurity.Location = new System.Drawing.Point(333, 100);
            this.checkBox_SetSecurity.Name = "checkBox_SetSecurity";
            this.checkBox_SetSecurity.Size = new System.Drawing.Size(156, 19);
            this.checkBox_SetSecurity.TabIndex = 46;
            this.checkBox_SetSecurity.Text = "Allow changing security";
            this.checkBox_SetSecurity.UseVisualStyleBackColor = true;
            this.checkBox_SetSecurity.CheckedChanged += new System.EventHandler(this.checkBox_SetSecurity_CheckedChanged);
            // 
            // checkBox_QueryInfo
            // 
            this.checkBox_QueryInfo.AutoSize = true;
            this.checkBox_QueryInfo.Checked = true;
            this.checkBox_QueryInfo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_QueryInfo.Location = new System.Drawing.Point(149, 78);
            this.checkBox_QueryInfo.Name = "checkBox_QueryInfo";
            this.checkBox_QueryInfo.Size = new System.Drawing.Size(173, 19);
            this.checkBox_QueryInfo.TabIndex = 42;
            this.checkBox_QueryInfo.Text = "Allow querying information";
            this.checkBox_QueryInfo.UseVisualStyleBackColor = true;
            this.checkBox_QueryInfo.CheckedChanged += new System.EventHandler(this.checkBox_QueryInfo_CheckedChanged);
            // 
            // checkBox_Read
            // 
            this.checkBox_Read.AutoSize = true;
            this.checkBox_Read.Checked = true;
            this.checkBox_Read.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_Read.Location = new System.Drawing.Point(9, 78);
            this.checkBox_Read.Name = "checkBox_Read";
            this.checkBox_Read.Size = new System.Drawing.Size(122, 19);
            this.checkBox_Read.TabIndex = 44;
            this.checkBox_Read.Text = "Allow reading file";
            this.checkBox_Read.UseVisualStyleBackColor = true;
            this.checkBox_Read.CheckedChanged += new System.EventHandler(this.checkBox_Read_CheckedChanged);
            // 
            // checkBox_QuerySecurity
            // 
            this.checkBox_QuerySecurity.AutoSize = true;
            this.checkBox_QuerySecurity.Checked = true;
            this.checkBox_QuerySecurity.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_QuerySecurity.Location = new System.Drawing.Point(335, 78);
            this.checkBox_QuerySecurity.Name = "checkBox_QuerySecurity";
            this.checkBox_QuerySecurity.Size = new System.Drawing.Size(152, 19);
            this.checkBox_QuerySecurity.TabIndex = 43;
            this.checkBox_QuerySecurity.Text = "Allow querying security";
            this.checkBox_QuerySecurity.UseVisualStyleBackColor = true;
            this.checkBox_QuerySecurity.CheckedChanged += new System.EventHandler(this.checkBox_QuerySecurity_CheckedChanged);
            // 
            // checkBox_SetInfo
            // 
            this.checkBox_SetInfo.AutoSize = true;
            this.checkBox_SetInfo.Checked = true;
            this.checkBox_SetInfo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_SetInfo.Location = new System.Drawing.Point(149, 100);
            this.checkBox_SetInfo.Name = "checkBox_SetInfo";
            this.checkBox_SetInfo.Size = new System.Drawing.Size(177, 19);
            this.checkBox_SetInfo.TabIndex = 45;
            this.checkBox_SetInfo.Text = "Allow changing information";
            this.checkBox_SetInfo.UseVisualStyleBackColor = true;
            this.checkBox_SetInfo.CheckedChanged += new System.EventHandler(this.checkBox_SetInfo_CheckedChanged);
            // 
            // checkBox_Write
            // 
            this.checkBox_Write.AutoSize = true;
            this.checkBox_Write.Checked = true;
            this.checkBox_Write.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_Write.Location = new System.Drawing.Point(9, 101);
            this.checkBox_Write.Name = "checkBox_Write";
            this.checkBox_Write.Size = new System.Drawing.Size(116, 19);
            this.checkBox_Write.TabIndex = 38;
            this.checkBox_Write.Text = "Allow writing file";
            this.checkBox_Write.UseVisualStyleBackColor = true;
            this.checkBox_Write.CheckedChanged += new System.EventHandler(this.checkBox_Write_CheckedChanged);
            // 
            // checkBox_AllowDelete
            // 
            this.checkBox_AllowDelete.AutoSize = true;
            this.checkBox_AllowDelete.Checked = true;
            this.checkBox_AllowDelete.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_AllowDelete.Location = new System.Drawing.Point(6, 146);
            this.checkBox_AllowDelete.Name = "checkBox_AllowDelete";
            this.checkBox_AllowDelete.Size = new System.Drawing.Size(124, 19);
            this.checkBox_AllowDelete.TabIndex = 40;
            this.checkBox_AllowDelete.Text = "Allow deleting file";
            this.checkBox_AllowDelete.UseVisualStyleBackColor = true;
            this.checkBox_AllowDelete.CheckedChanged += new System.EventHandler(this.checkBox_AllowDelete_CheckedChanged);
            // 
            // checkBox_AllowRename
            // 
            this.checkBox_AllowRename.AutoSize = true;
            this.checkBox_AllowRename.Checked = true;
            this.checkBox_AllowRename.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_AllowRename.Location = new System.Drawing.Point(149, 123);
            this.checkBox_AllowRename.Name = "checkBox_AllowRename";
            this.checkBox_AllowRename.Size = new System.Drawing.Size(133, 19);
            this.checkBox_AllowRename.TabIndex = 39;
            this.checkBox_AllowRename.Text = "Allow renaming file";
            this.checkBox_AllowRename.UseVisualStyleBackColor = true;
            this.checkBox_AllowRename.CheckedChanged += new System.EventHandler(this.checkBox_AllowRename_CheckedChanged);
            // 
            // checkBox_Creation
            // 
            this.checkBox_Creation.AutoSize = true;
            this.checkBox_Creation.Checked = true;
            this.checkBox_Creation.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_Creation.Location = new System.Drawing.Point(9, 123);
            this.checkBox_Creation.Name = "checkBox_Creation";
            this.checkBox_Creation.Size = new System.Drawing.Size(150, 19);
            this.checkBox_Creation.TabIndex = 41;
            this.checkBox_Creation.Text = "Allow creating new file";
            this.checkBox_Creation.UseVisualStyleBackColor = true;
            this.checkBox_Creation.CheckedChanged += new System.EventHandler(this.checkBox_Creation_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(120, 15);
            this.label2.TabIndex = 32;
            this.label2.Text = "Access Control Flags";
            // 
            // textBox_FileAccessFlags
            // 
            this.textBox_FileAccessFlags.Location = new System.Drawing.Point(149, 35);
            this.textBox_FileAccessFlags.Name = "textBox_FileAccessFlags";
            this.textBox_FileAccessFlags.Size = new System.Drawing.Size(242, 20);
            this.textBox_FileAccessFlags.TabIndex = 31;
            this.textBox_FileAccessFlags.Text = "0";
            // 
            // button_FileAccessFlags
            // 
            this.button_FileAccessFlags.Location = new System.Drawing.Point(421, 34);
            this.button_FileAccessFlags.Name = "button_FileAccessFlags";
            this.button_FileAccessFlags.Size = new System.Drawing.Size(60, 20);
            this.button_FileAccessFlags.TabIndex = 33;
            this.button_FileAccessFlags.Text = "...";
            this.button_FileAccessFlags.UseVisualStyleBackColor = true;
            this.button_FileAccessFlags.Click += new System.EventHandler(this.button_FileAccessFlags_Click);
            // 
            // groupBox_UserName
            // 
            this.groupBox_UserName.Controls.Add(this.textBox_UserName);
            this.groupBox_UserName.Controls.Add(this.label1);
            this.groupBox_UserName.Location = new System.Drawing.Point(43, 126);
            this.groupBox_UserName.Name = "groupBox_UserName";
            this.groupBox_UserName.Size = new System.Drawing.Size(517, 46);
            this.groupBox_UserName.TabIndex = 30;
            this.groupBox_UserName.TabStop = false;
            this.groupBox_UserName.Visible = false;
            // 
            // textBox_UserName
            // 
            this.textBox_UserName.Location = new System.Drawing.Point(149, 16);
            this.textBox_UserName.Name = "textBox_UserName";
            this.textBox_UserName.Size = new System.Drawing.Size(298, 20);
            this.textBox_UserName.TabIndex = 27;
            this.textBox_UserName.Text = "domain1\\user1;  domain2\\user2;";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 15);
            this.label1.TabIndex = 28;
            this.label1.Text = "User Name ";
            // 
            // groupBox_ProcessId
            // 
            this.groupBox_ProcessId.Controls.Add(this.button_ProcessId);
            this.groupBox_ProcessId.Controls.Add(this.textBox_ProcessId);
            this.groupBox_ProcessId.Controls.Add(this.label3);
            this.groupBox_ProcessId.Location = new System.Drawing.Point(43, 73);
            this.groupBox_ProcessId.Name = "groupBox_ProcessId";
            this.groupBox_ProcessId.Size = new System.Drawing.Size(517, 48);
            this.groupBox_ProcessId.TabIndex = 29;
            this.groupBox_ProcessId.TabStop = false;
            this.groupBox_ProcessId.Visible = false;
            // 
            // button_ProcessId
            // 
            this.button_ProcessId.Location = new System.Drawing.Point(461, 15);
            this.button_ProcessId.Name = "button_ProcessId";
            this.button_ProcessId.Size = new System.Drawing.Size(50, 20);
            this.button_ProcessId.TabIndex = 38;
            this.button_ProcessId.Text = "...";
            this.button_ProcessId.UseVisualStyleBackColor = true;
            this.button_ProcessId.Click += new System.EventHandler(this.button_ProcessId_Click);
            // 
            // textBox_ProcessId
            // 
            this.textBox_ProcessId.Location = new System.Drawing.Point(149, 16);
            this.textBox_ProcessId.Name = "textBox_ProcessId";
            this.textBox_ProcessId.Size = new System.Drawing.Size(298, 20);
            this.textBox_ProcessId.TabIndex = 27;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 19);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 15);
            this.label3.TabIndex = 28;
            this.label3.Text = "Process Id";
            // 
            // Form_AccessRights
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(590, 446);
            this.Controls.Add(this.groupBox_ProcessId);
            this.Controls.Add(this.groupBox_UserName);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.button_Add);
            this.Controls.Add(this.groupBox_ProcessName);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form_AccessRights";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add Access Rights";
            this.groupBox_ProcessName.ResumeLayout(false);
            this.groupBox_ProcessName.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox_UserName.ResumeLayout(false);
            this.groupBox_UserName.PerformLayout();
            this.groupBox_ProcessId.ResumeLayout(false);
            this.groupBox_ProcessId.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_Add;
        private System.Windows.Forms.GroupBox groupBox_ProcessName;
        private System.Windows.Forms.TextBox textBox_ProcessName;
        private System.Windows.Forms.Label label_AccessFlags;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox_UserName;
        private System.Windows.Forms.TextBox textBox_UserName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_FileAccessFlags;
        private System.Windows.Forms.Button button_FileAccessFlags;
        private System.Windows.Forms.GroupBox groupBox_ProcessId;
        private System.Windows.Forms.Button button_ProcessId;
        private System.Windows.Forms.TextBox textBox_ProcessId;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkBox_AllowReadEncryptedFiles;
        private System.Windows.Forms.CheckBox checkBox_AllowCopyout;
        private System.Windows.Forms.CheckBox checkBox_AllowSaveAs;
        private System.Windows.Forms.CheckBox checkBox_SetSecurity;
        private System.Windows.Forms.CheckBox checkBox_QueryInfo;
        private System.Windows.Forms.CheckBox checkBox_Read;
        private System.Windows.Forms.CheckBox checkBox_QuerySecurity;
        private System.Windows.Forms.CheckBox checkBox_SetInfo;
        private System.Windows.Forms.CheckBox checkBox_Write;
        private System.Windows.Forms.CheckBox checkBox_AllowDelete;
        private System.Windows.Forms.CheckBox checkBox_AllowRename;
        private System.Windows.Forms.CheckBox checkBox_Creation;
    }
}