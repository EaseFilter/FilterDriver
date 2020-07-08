namespace EaseFilter.CommonObjects
{
    partial class SettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBox_DisableDir = new System.Windows.Forms.CheckBox();
            this.comboBox_EventLevel = new System.Windows.Forms.ComboBox();
            this.label31 = new System.Windows.Forms.Label();
            this.checkBox_EnableNotification = new System.Windows.Forms.CheckBox();
            this.checkBox_OutputMessageToConsole = new System.Windows.Forms.CheckBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.button_SelectProtectPID = new System.Windows.Forms.Button();
            this.textBox_ProtectedPID = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button_EditFilterRule = new System.Windows.Forms.Button();
            this.button_DeleteFilter = new System.Windows.Forms.Button();
            this.button_AddFilter = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.listView_FilterRules = new System.Windows.Forms.ListView();
            this.textBox_LogSize = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.checkBox_TransactionLog = new System.Windows.Forms.CheckBox();
            this.textBox_TransactionLog = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBox_MaximumFilterMessage = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.button_SelectExcludePID = new System.Windows.Forms.Button();
            this.textBox_ExcludePID = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.button_SelectIncludePID = new System.Windows.Forms.Button();
            this.textBox_IncludePID = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button_ApplyOptions = new System.Windows.Forms.Button();
            this.checkBox_SendBuffer = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBox_SendBuffer);
            this.groupBox1.Controls.Add(this.checkBox_DisableDir);
            this.groupBox1.Controls.Add(this.comboBox_EventLevel);
            this.groupBox1.Controls.Add(this.label31);
            this.groupBox1.Controls.Add(this.checkBox_EnableNotification);
            this.groupBox1.Controls.Add(this.checkBox_OutputMessageToConsole);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.button_SelectProtectPID);
            this.groupBox1.Controls.Add(this.textBox_ProtectedPID);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.button_EditFilterRule);
            this.groupBox1.Controls.Add(this.button_DeleteFilter);
            this.groupBox1.Controls.Add(this.button_AddFilter);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.textBox_LogSize);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.checkBox_TransactionLog);
            this.groupBox1.Controls.Add(this.textBox_TransactionLog);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.textBox_MaximumFilterMessage);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.button_SelectExcludePID);
            this.groupBox1.Controls.Add(this.textBox_ExcludePID);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.button_SelectIncludePID);
            this.groupBox1.Controls.Add(this.textBox_IncludePID);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(602, 552);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // checkBox_DisableDir
            // 
            this.checkBox_DisableDir.AutoSize = true;
            this.checkBox_DisableDir.Location = new System.Drawing.Point(398, 237);
            this.checkBox_DisableDir.Name = "checkBox_DisableDir";
            this.checkBox_DisableDir.Size = new System.Drawing.Size(120, 17);
            this.checkBox_DisableDir.TabIndex = 67;
            this.checkBox_DisableDir.Text = "Disable Directory IO";
            this.checkBox_DisableDir.UseVisualStyleBackColor = true;
            // 
            // comboBox_EventLevel
            // 
            this.comboBox_EventLevel.FormattingEnabled = true;
            this.comboBox_EventLevel.Location = new System.Drawing.Point(169, 283);
            this.comboBox_EventLevel.Name = "comboBox_EventLevel";
            this.comboBox_EventLevel.Size = new System.Drawing.Size(362, 21);
            this.comboBox_EventLevel.TabIndex = 66;
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(10, 283);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(77, 13);
            this.label31.TabIndex = 65;
            this.label31.Text = "Event log level";
            // 
            // checkBox_EnableNotification
            // 
            this.checkBox_EnableNotification.AutoSize = true;
            this.checkBox_EnableNotification.Location = new System.Drawing.Point(398, 257);
            this.checkBox_EnableNotification.Name = "checkBox_EnableNotification";
            this.checkBox_EnableNotification.Size = new System.Drawing.Size(113, 17);
            this.checkBox_EnableNotification.TabIndex = 64;
            this.checkBox_EnableNotification.Text = "Enable notification";
            this.checkBox_EnableNotification.UseVisualStyleBackColor = true;
            // 
            // checkBox_OutputMessageToConsole
            // 
            this.checkBox_OutputMessageToConsole.AutoSize = true;
            this.checkBox_OutputMessageToConsole.Checked = true;
            this.checkBox_OutputMessageToConsole.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_OutputMessageToConsole.Location = new System.Drawing.Point(398, 150);
            this.checkBox_OutputMessageToConsole.Name = "checkBox_OutputMessageToConsole";
            this.checkBox_OutputMessageToConsole.Size = new System.Drawing.Size(155, 17);
            this.checkBox_OutputMessageToConsole.TabIndex = 63;
            this.checkBox_OutputMessageToConsole.Text = "Output message to console";
            this.checkBox_OutputMessageToConsole.UseVisualStyleBackColor = true;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(167, 222);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(253, 12);
            this.label13.TabIndex = 62;
            this.label13.Text = "(The filter message log file name with full path if it is enabled.)";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(167, 171);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(233, 12);
            this.label12.TabIndex = 61;
            this.label12.Text = "(The maximum message items to display in the console)";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(168, 123);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(232, 12);
            this.label10.TabIndex = 59;
            this.label10.Text = "(Skip the file IOs from these processes if it is not empty)";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(168, 84);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(268, 12);
            this.label9.TabIndex = 58;
            this.label9.Text = "(Only manage the file IOs from these processes if it is not empty)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(168, 42);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(171, 12);
            this.label4.TabIndex = 57;
            this.label4.Text = "(prevent the processes being termiated.)";
            // 
            // button_SelectProtectPID
            // 
            this.button_SelectProtectPID.Location = new System.Drawing.Point(548, 20);
            this.button_SelectProtectPID.Name = "button_SelectProtectPID";
            this.button_SelectProtectPID.Size = new System.Drawing.Size(30, 20);
            this.button_SelectProtectPID.TabIndex = 56;
            this.button_SelectProtectPID.Text = "...";
            this.button_SelectProtectPID.UseVisualStyleBackColor = true;
            this.button_SelectProtectPID.Click += new System.EventHandler(this.button_SelectProtectPID_Click);
            // 
            // textBox_ProtectedPID
            // 
            this.textBox_ProtectedPID.Location = new System.Drawing.Point(170, 21);
            this.textBox_ProtectedPID.Name = "textBox_ProtectedPID";
            this.textBox_ProtectedPID.ReadOnly = true;
            this.textBox_ProtectedPID.Size = new System.Drawing.Size(361, 20);
            this.textBox_ProtectedPID.TabIndex = 55;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(121, 13);
            this.label2.TabIndex = 54;
            this.label2.Text = "Add protect process IDs";
            // 
            // button_EditFilterRule
            // 
            this.button_EditFilterRule.Location = new System.Drawing.Point(236, 513);
            this.button_EditFilterRule.Name = "button_EditFilterRule";
            this.button_EditFilterRule.Size = new System.Drawing.Size(92, 23);
            this.button_EditFilterRule.TabIndex = 53;
            this.button_EditFilterRule.Text = "Edit Filter Rule";
            this.button_EditFilterRule.UseVisualStyleBackColor = true;
            this.button_EditFilterRule.Click += new System.EventHandler(this.button_EditFilterRule_Click);
            // 
            // button_DeleteFilter
            // 
            this.button_DeleteFilter.Location = new System.Drawing.Point(427, 513);
            this.button_DeleteFilter.Name = "button_DeleteFilter";
            this.button_DeleteFilter.Size = new System.Drawing.Size(114, 23);
            this.button_DeleteFilter.TabIndex = 52;
            this.button_DeleteFilter.Text = "Delete Filter Rule";
            this.button_DeleteFilter.UseVisualStyleBackColor = true;
            this.button_DeleteFilter.Click += new System.EventHandler(this.button_DeleteFilter_Click);
            // 
            // button_AddFilter
            // 
            this.button_AddFilter.Location = new System.Drawing.Point(14, 510);
            this.button_AddFilter.Name = "button_AddFilter";
            this.button_AddFilter.Size = new System.Drawing.Size(127, 23);
            this.button_AddFilter.TabIndex = 51;
            this.button_AddFilter.Text = "Add New Filter Rule";
            this.button_AddFilter.UseVisualStyleBackColor = true;
            this.button_AddFilter.Click += new System.EventHandler(this.button_AddFilter_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.listView_FilterRules);
            this.groupBox2.Location = new System.Drawing.Point(13, 340);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(528, 167);
            this.groupBox2.TabIndex = 50;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Filter Rule Settings-- Monitor or Control File Access ";
            // 
            // listView_FilterRules
            // 
            this.listView_FilterRules.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView_FilterRules.FullRowSelect = true;
            this.listView_FilterRules.Location = new System.Drawing.Point(3, 16);
            this.listView_FilterRules.Name = "listView_FilterRules";
            this.listView_FilterRules.Size = new System.Drawing.Size(522, 148);
            this.listView_FilterRules.TabIndex = 1;
            this.listView_FilterRules.UseCompatibleStateImageBehavior = false;
            this.listView_FilterRules.View = System.Windows.Forms.View.Details;
            // 
            // textBox_LogSize
            // 
            this.textBox_LogSize.Location = new System.Drawing.Point(169, 254);
            this.textBox_LogSize.Name = "textBox_LogSize";
            this.textBox_LogSize.Size = new System.Drawing.Size(201, 20);
            this.textBox_LogSize.TabIndex = 46;
            this.textBox_LogSize.Text = "1024";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(10, 254);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(128, 13);
            this.label8.TabIndex = 45;
            this.label8.Text = "Maximum log file size (KB)";
            // 
            // checkBox_TransactionLog
            // 
            this.checkBox_TransactionLog.AutoSize = true;
            this.checkBox_TransactionLog.Location = new System.Drawing.Point(398, 202);
            this.checkBox_TransactionLog.Name = "checkBox_TransactionLog";
            this.checkBox_TransactionLog.Size = new System.Drawing.Size(143, 17);
            this.checkBox_TransactionLog.TabIndex = 44;
            this.checkBox_TransactionLog.Text = "Enable log filter message";
            this.checkBox_TransactionLog.UseVisualStyleBackColor = true;
            // 
            // textBox_TransactionLog
            // 
            this.textBox_TransactionLog.Location = new System.Drawing.Point(170, 200);
            this.textBox_TransactionLog.Name = "textBox_TransactionLog";
            this.textBox_TransactionLog.Size = new System.Drawing.Size(200, 20);
            this.textBox_TransactionLog.TabIndex = 43;
            this.textBox_TransactionLog.Text = "filterMessage.log";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 200);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(148, 13);
            this.label7.TabIndex = 42;
            this.label7.Text = "File name to log filter message";
            // 
            // textBox_MaximumFilterMessage
            // 
            this.textBox_MaximumFilterMessage.Location = new System.Drawing.Point(170, 148);
            this.textBox_MaximumFilterMessage.Name = "textBox_MaximumFilterMessage";
            this.textBox_MaximumFilterMessage.Size = new System.Drawing.Size(200, 20);
            this.textBox_MaximumFilterMessage.TabIndex = 41;
            this.textBox_MaximumFilterMessage.Text = "500";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 148);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(89, 13);
            this.label6.TabIndex = 40;
            this.label6.Text = "Maximum records";
            // 
            // button_SelectExcludePID
            // 
            this.button_SelectExcludePID.Location = new System.Drawing.Point(548, 100);
            this.button_SelectExcludePID.Name = "button_SelectExcludePID";
            this.button_SelectExcludePID.Size = new System.Drawing.Size(30, 20);
            this.button_SelectExcludePID.TabIndex = 38;
            this.button_SelectExcludePID.Text = "...";
            this.button_SelectExcludePID.UseVisualStyleBackColor = true;
            this.button_SelectExcludePID.Click += new System.EventHandler(this.button_SelectExcludePID_Click);
            // 
            // textBox_ExcludePID
            // 
            this.textBox_ExcludePID.Location = new System.Drawing.Point(170, 100);
            this.textBox_ExcludePID.Name = "textBox_ExcludePID";
            this.textBox_ExcludePID.ReadOnly = true;
            this.textBox_ExcludePID.Size = new System.Drawing.Size(361, 20);
            this.textBox_ExcludePID.TabIndex = 37;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 100);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(110, 13);
            this.label5.TabIndex = 36;
            this.label5.Text = "Excluded process IDs";
            // 
            // button_SelectIncludePID
            // 
            this.button_SelectIncludePID.Location = new System.Drawing.Point(549, 60);
            this.button_SelectIncludePID.Name = "button_SelectIncludePID";
            this.button_SelectIncludePID.Size = new System.Drawing.Size(30, 20);
            this.button_SelectIncludePID.TabIndex = 34;
            this.button_SelectIncludePID.Text = "...";
            this.button_SelectIncludePID.UseVisualStyleBackColor = true;
            this.button_SelectIncludePID.Click += new System.EventHandler(this.button_SelectIncludePID_Click);
            // 
            // textBox_IncludePID
            // 
            this.textBox_IncludePID.Location = new System.Drawing.Point(170, 61);
            this.textBox_IncludePID.Name = "textBox_IncludePID";
            this.textBox_IncludePID.ReadOnly = true;
            this.textBox_IncludePID.Size = new System.Drawing.Size(361, 20);
            this.textBox_IncludePID.TabIndex = 33;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 13);
            this.label1.TabIndex = 32;
            this.label1.Text = "Included process IDs";
            // 
            // button_ApplyOptions
            // 
            this.button_ApplyOptions.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button_ApplyOptions.Location = new System.Drawing.Point(508, 580);
            this.button_ApplyOptions.Name = "button_ApplyOptions";
            this.button_ApplyOptions.Size = new System.Drawing.Size(106, 23);
            this.button_ApplyOptions.TabIndex = 1;
            this.button_ApplyOptions.Text = "Apply Settings";
            this.button_ApplyOptions.UseVisualStyleBackColor = true;
            this.button_ApplyOptions.Click += new System.EventHandler(this.button_ApplyOptions_Click);
            // 
            // checkBox_SendBuffer
            // 
            this.checkBox_SendBuffer.AutoSize = true;
            this.checkBox_SendBuffer.Location = new System.Drawing.Point(398, 173);
            this.checkBox_SendBuffer.Name = "checkBox_SendBuffer";
            this.checkBox_SendBuffer.Size = new System.Drawing.Size(177, 17);
            this.checkBox_SendBuffer.TabIndex = 68;
            this.checkBox_SendBuffer.Text = "Enable Send Read/Write Buffer";
            this.checkBox_SendBuffer.UseVisualStyleBackColor = true;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(665, 629);
            this.Controls.Add(this.button_ApplyOptions);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SettingsForm";
            this.Text = "Filter Driver Global Settings";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button_SelectExcludePID;
        private System.Windows.Forms.TextBox textBox_ExcludePID;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button_SelectIncludePID;
        private System.Windows.Forms.TextBox textBox_IncludePID;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button_ApplyOptions;
        private System.Windows.Forms.CheckBox checkBox_TransactionLog;
        private System.Windows.Forms.TextBox textBox_TransactionLog;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBox_MaximumFilterMessage;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox_LogSize;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListView listView_FilterRules;
        private System.Windows.Forms.Button button_DeleteFilter;
        private System.Windows.Forms.Button button_AddFilter;
        private System.Windows.Forms.Button button_EditFilterRule;
        private System.Windows.Forms.Button button_SelectProtectPID;
        private System.Windows.Forms.TextBox textBox_ProtectedPID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox checkBox_OutputMessageToConsole;
        private System.Windows.Forms.CheckBox checkBox_EnableNotification;
        private System.Windows.Forms.ComboBox comboBox_EventLevel;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.CheckBox checkBox_DisableDir;
        private System.Windows.Forms.CheckBox checkBox_SendBuffer;
    }
}