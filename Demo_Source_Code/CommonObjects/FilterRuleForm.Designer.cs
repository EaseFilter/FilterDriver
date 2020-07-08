namespace EaseFilter.CommonObjects
{
    partial class FilterRuleForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FilterRuleForm));
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.button_SaveFilterRule = new System.Windows.Forms.Button();
            this.textBox_ExcludeFilterMask = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_IncludeFilterMask = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button_ControlSettings = new System.Windows.Forms.Button();
            this.label20 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.textBox_ExcludeUserNames = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.textBox_IncludeUserNames = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.textBox_ExcludeProcessNames = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.textBox_IncludeProcessNames = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.button_SelectExcludePID = new System.Windows.Forms.Button();
            this.textBox_ExcludePID = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.button_SelectIncludePID = new System.Windows.Forms.Button();
            this.textBox_IncludePID = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox_AccessControl = new System.Windows.Forms.GroupBox();
            this.textBox_SelectedEvents = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.button_SelectedEvents = new System.Windows.Forms.Button();
            this.button_RegisterMonitorIO = new System.Windows.Forms.Button();
            this.label16 = new System.Windows.Forms.Label();
            this.textBox_MonitorIO = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.button_FilterCreateOptions = new System.Windows.Forms.Button();
            this.textBox_FilterCreateOptions = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.button_FilterDisposition = new System.Windows.Forms.Button();
            this.textBox_FilterDisposition = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.button_FilterDesiredAccess = new System.Windows.Forms.Button();
            this.textBox_FilterDesiredAccess = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox_AccessControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(211, 36);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(166, 12);
            this.label4.TabIndex = 10;
            this.label4.Text = "( you can use wild card character \'*\', \'?\')";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(211, 75);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(131, 12);
            this.label3.TabIndex = 9;
            this.label3.Text = "( split with \';\' for multiple items)";
            // 
            // button_SaveFilterRule
            // 
            this.button_SaveFilterRule.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button_SaveFilterRule.Location = new System.Drawing.Point(456, 554);
            this.button_SaveFilterRule.Name = "button_SaveFilterRule";
            this.button_SaveFilterRule.Size = new System.Drawing.Size(75, 23);
            this.button_SaveFilterRule.TabIndex = 8;
            this.button_SaveFilterRule.Text = "Save";
            this.button_SaveFilterRule.UseVisualStyleBackColor = true;
            this.button_SaveFilterRule.Click += new System.EventHandler(this.button_SaveFilter_Click);
            // 
            // textBox_ExcludeFilterMask
            // 
            this.textBox_ExcludeFilterMask.Location = new System.Drawing.Point(213, 52);
            this.textBox_ExcludeFilterMask.Name = "textBox_ExcludeFilterMask";
            this.textBox_ExcludeFilterMask.Size = new System.Drawing.Size(242, 20);
            this.textBox_ExcludeFilterMask.TabIndex = 4;
            this.textBox_ExcludeFilterMask.MouseHover += new System.EventHandler(this.textBox_ExcludeFilterMask_MouseHover);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(111, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Exclude file filter mask";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(112, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Manage file filter mask";
            // 
            // textBox_IncludeFilterMask
            // 
            this.textBox_IncludeFilterMask.Location = new System.Drawing.Point(213, 13);
            this.textBox_IncludeFilterMask.Name = "textBox_IncludeFilterMask";
            this.textBox_IncludeFilterMask.Size = new System.Drawing.Size(242, 20);
            this.textBox_IncludeFilterMask.TabIndex = 0;
            this.textBox_IncludeFilterMask.Text = "c:\\test\\*";
            this.textBox_IncludeFilterMask.MouseHover += new System.EventHandler(this.textBox_IncludeFilterMask_MouseHover);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button_ControlSettings);
            this.groupBox1.Controls.Add(this.label20);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.textBox_ExcludeUserNames);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.textBox_IncludeUserNames);
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.textBox_ExcludeProcessNames);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.textBox_IncludeProcessNames);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.button_SelectExcludePID);
            this.groupBox1.Controls.Add(this.textBox_ExcludePID);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.button_SelectIncludePID);
            this.groupBox1.Controls.Add(this.textBox_IncludePID);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.groupBox_AccessControl);
            this.groupBox1.Controls.Add(this.textBox_ExcludeFilterMask);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.textBox_IncludeFilterMask);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(572, 531);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            // 
            // button_ControlSettings
            // 
            this.button_ControlSettings.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.button_ControlSettings.Location = new System.Drawing.Point(18, 485);
            this.button_ControlSettings.Name = "button_ControlSettings";
            this.button_ControlSettings.Size = new System.Drawing.Size(517, 23);
            this.button_ControlSettings.TabIndex = 12;
            this.button_ControlSettings.Text = "File Access Control Congfiguration Settings";
            this.button_ControlSettings.UseVisualStyleBackColor = false;
            this.button_ControlSettings.Click += new System.EventHandler(this.button_ControlSettings_Click);
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.Location = new System.Drawing.Point(211, 186);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(222, 12);
            this.label20.TabIndex = 74;
            this.label20.Text = "( split with \';\' for multiple items, format \"notepad.exe\")";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(210, 253);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(245, 12);
            this.label13.TabIndex = 68;
            this.label13.Text = "(split with \';\' for multiple items, format \"domain\\username\" )";
            // 
            // textBox_ExcludeUserNames
            // 
            this.textBox_ExcludeUserNames.Location = new System.Drawing.Point(212, 268);
            this.textBox_ExcludeUserNames.Name = "textBox_ExcludeUserNames";
            this.textBox_ExcludeUserNames.Size = new System.Drawing.Size(242, 20);
            this.textBox_ExcludeUserNames.TabIndex = 67;
            this.textBox_ExcludeUserNames.MouseHover += new System.EventHandler(this.textBox_ExcludeUserNames_MouseHover);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(15, 268);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(102, 13);
            this.label14.TabIndex = 66;
            this.label14.Text = "Exclude user names";
            // 
            // textBox_IncludeUserNames
            // 
            this.textBox_IncludeUserNames.Location = new System.Drawing.Point(212, 230);
            this.textBox_IncludeUserNames.Name = "textBox_IncludeUserNames";
            this.textBox_IncludeUserNames.Size = new System.Drawing.Size(242, 20);
            this.textBox_IncludeUserNames.TabIndex = 65;
            this.textBox_IncludeUserNames.MouseHover += new System.EventHandler(this.textBox_IncludeUserNames_MouseHover);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(15, 237);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(99, 13);
            this.label15.TabIndex = 64;
            this.label15.Text = "Include user names";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(211, 115);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(222, 12);
            this.label12.TabIndex = 57;
            this.label12.Text = "( split with \';\' for multiple items, format \"notepad.exe\")";
            // 
            // textBox_ExcludeProcessNames
            // 
            this.textBox_ExcludeProcessNames.Location = new System.Drawing.Point(213, 165);
            this.textBox_ExcludeProcessNames.Name = "textBox_ExcludeProcessNames";
            this.textBox_ExcludeProcessNames.Size = new System.Drawing.Size(242, 20);
            this.textBox_ExcludeProcessNames.TabIndex = 56;
            this.textBox_ExcludeProcessNames.MouseHover += new System.EventHandler(this.textBox_ExcludeProcessNames_MouseHover);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(15, 165);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(119, 13);
            this.label11.TabIndex = 55;
            this.label11.Text = "Exclude process names";
            // 
            // textBox_IncludeProcessNames
            // 
            this.textBox_IncludeProcessNames.Location = new System.Drawing.Point(213, 92);
            this.textBox_IncludeProcessNames.Name = "textBox_IncludeProcessNames";
            this.textBox_IncludeProcessNames.Size = new System.Drawing.Size(242, 20);
            this.textBox_IncludeProcessNames.TabIndex = 54;
            this.textBox_IncludeProcessNames.MouseHover += new System.EventHandler(this.textBox_IncludeProcessNames_MouseHover);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(16, 99);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(116, 13);
            this.label10.TabIndex = 53;
            this.label10.Text = "Include process names";
            // 
            // button_SelectExcludePID
            // 
            this.button_SelectExcludePID.Location = new System.Drawing.Point(474, 204);
            this.button_SelectExcludePID.Name = "button_SelectExcludePID";
            this.button_SelectExcludePID.Size = new System.Drawing.Size(41, 20);
            this.button_SelectExcludePID.TabIndex = 44;
            this.button_SelectExcludePID.Text = "...";
            this.button_SelectExcludePID.UseVisualStyleBackColor = true;
            this.button_SelectExcludePID.Click += new System.EventHandler(this.button_SelectExcludePID_Click);
            // 
            // textBox_ExcludePID
            // 
            this.textBox_ExcludePID.Location = new System.Drawing.Point(213, 202);
            this.textBox_ExcludePID.Name = "textBox_ExcludePID";
            this.textBox_ExcludePID.ReadOnly = true;
            this.textBox_ExcludePID.Size = new System.Drawing.Size(242, 20);
            this.textBox_ExcludePID.TabIndex = 43;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 204);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(102, 13);
            this.label5.TabIndex = 42;
            this.label5.Text = "Exclude process Ids";
            // 
            // button_SelectIncludePID
            // 
            this.button_SelectIncludePID.Location = new System.Drawing.Point(475, 134);
            this.button_SelectIncludePID.Name = "button_SelectIncludePID";
            this.button_SelectIncludePID.Size = new System.Drawing.Size(41, 20);
            this.button_SelectIncludePID.TabIndex = 41;
            this.button_SelectIncludePID.Text = "...";
            this.button_SelectIncludePID.UseVisualStyleBackColor = true;
            this.button_SelectIncludePID.Click += new System.EventHandler(this.button_SelectIncludePID_Click);
            // 
            // textBox_IncludePID
            // 
            this.textBox_IncludePID.Location = new System.Drawing.Point(213, 135);
            this.textBox_IncludePID.Name = "textBox_IncludePID";
            this.textBox_IncludePID.ReadOnly = true;
            this.textBox_IncludePID.Size = new System.Drawing.Size(242, 20);
            this.textBox_IncludePID.TabIndex = 40;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(16, 134);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(99, 13);
            this.label6.TabIndex = 39;
            this.label6.Text = "Include process Ids";
            // 
            // groupBox_AccessControl
            // 
            this.groupBox_AccessControl.Controls.Add(this.button_FilterCreateOptions);
            this.groupBox_AccessControl.Controls.Add(this.textBox_FilterCreateOptions);
            this.groupBox_AccessControl.Controls.Add(this.label7);
            this.groupBox_AccessControl.Controls.Add(this.button_FilterDisposition);
            this.groupBox_AccessControl.Controls.Add(this.textBox_FilterDisposition);
            this.groupBox_AccessControl.Controls.Add(this.label8);
            this.groupBox_AccessControl.Controls.Add(this.button_FilterDesiredAccess);
            this.groupBox_AccessControl.Controls.Add(this.textBox_FilterDesiredAccess);
            this.groupBox_AccessControl.Controls.Add(this.label17);
            this.groupBox_AccessControl.Controls.Add(this.textBox_SelectedEvents);
            this.groupBox_AccessControl.Controls.Add(this.label19);
            this.groupBox_AccessControl.Controls.Add(this.label9);
            this.groupBox_AccessControl.Controls.Add(this.label18);
            this.groupBox_AccessControl.Controls.Add(this.button_SelectedEvents);
            this.groupBox_AccessControl.Controls.Add(this.button_RegisterMonitorIO);
            this.groupBox_AccessControl.Controls.Add(this.label16);
            this.groupBox_AccessControl.Controls.Add(this.textBox_MonitorIO);
            this.groupBox_AccessControl.Location = new System.Drawing.Point(17, 299);
            this.groupBox_AccessControl.Name = "groupBox_AccessControl";
            this.groupBox_AccessControl.Size = new System.Drawing.Size(517, 180);
            this.groupBox_AccessControl.TabIndex = 24;
            this.groupBox_AccessControl.TabStop = false;
            this.groupBox_AccessControl.Text = "Monitor Filter Only Settings";
            // 
            // textBox_SelectedEvents
            // 
            this.textBox_SelectedEvents.Location = new System.Drawing.Point(201, 16);
            this.textBox_SelectedEvents.Name = "textBox_SelectedEvents";
            this.textBox_SelectedEvents.ReadOnly = true;
            this.textBox_SelectedEvents.Size = new System.Drawing.Size(242, 20);
            this.textBox_SelectedEvents.TabIndex = 51;
            this.textBox_SelectedEvents.Text = "0";
            this.textBox_SelectedEvents.MouseHover += new System.EventHandler(this.textBox_SelectedEvents_MouseHover);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19.Location = new System.Drawing.Point(197, 85);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(233, 12);
            this.label19.TabIndex = 73;
            this.label19.Text = "(Receive notification when registered IO was triggered )";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(-1, 23);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(97, 13);
            this.label9.TabIndex = 50;
            this.label9.Text = "Register file events";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.Location = new System.Drawing.Point(198, 39);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(237, 12);
            this.label18.TabIndex = 72;
            this.label18.Text = "(Receive file event notification after file handle is closed )";
            // 
            // button_SelectedEvents
            // 
            this.button_SelectedEvents.Location = new System.Drawing.Point(461, 16);
            this.button_SelectedEvents.Name = "button_SelectedEvents";
            this.button_SelectedEvents.Size = new System.Drawing.Size(41, 20);
            this.button_SelectedEvents.TabIndex = 52;
            this.button_SelectedEvents.Text = "...";
            this.button_SelectedEvents.UseVisualStyleBackColor = true;
            this.button_SelectedEvents.Click += new System.EventHandler(this.button_SelectedEvents_Click);
            // 
            // button_RegisterMonitorIO
            // 
            this.button_RegisterMonitorIO.Location = new System.Drawing.Point(461, 63);
            this.button_RegisterMonitorIO.Name = "button_RegisterMonitorIO";
            this.button_RegisterMonitorIO.Size = new System.Drawing.Size(41, 20);
            this.button_RegisterMonitorIO.TabIndex = 71;
            this.button_RegisterMonitorIO.Text = "...";
            this.button_RegisterMonitorIO.UseVisualStyleBackColor = true;
            this.button_RegisterMonitorIO.Click += new System.EventHandler(this.button_RegisterMonitorIO_Click);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(-3, 67);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(97, 13);
            this.label16.TabIndex = 69;
            this.label16.Text = "Register monitor IO";
            // 
            // textBox_MonitorIO
            // 
            this.textBox_MonitorIO.Location = new System.Drawing.Point(199, 63);
            this.textBox_MonitorIO.Name = "textBox_MonitorIO";
            this.textBox_MonitorIO.ReadOnly = true;
            this.textBox_MonitorIO.Size = new System.Drawing.Size(242, 20);
            this.textBox_MonitorIO.TabIndex = 70;
            this.textBox_MonitorIO.Text = "0";
            this.textBox_MonitorIO.MouseHover += new System.EventHandler(this.textBox_MonitorIO_MouseHover);
            // 
            // button_FilterCreateOptions
            // 
            this.button_FilterCreateOptions.Location = new System.Drawing.Point(461, 152);
            this.button_FilterCreateOptions.Name = "button_FilterCreateOptions";
            this.button_FilterCreateOptions.Size = new System.Drawing.Size(41, 20);
            this.button_FilterCreateOptions.TabIndex = 111;
            this.button_FilterCreateOptions.Text = "...";
            this.button_FilterCreateOptions.UseVisualStyleBackColor = true;
            this.button_FilterCreateOptions.Click += new System.EventHandler(this.button_FilterCreateOptions_Click);
            // 
            // textBox_FilterCreateOptions
            // 
            this.textBox_FilterCreateOptions.Location = new System.Drawing.Point(199, 152);
            this.textBox_FilterCreateOptions.Name = "textBox_FilterCreateOptions";
            this.textBox_FilterCreateOptions.ReadOnly = true;
            this.textBox_FilterCreateOptions.Size = new System.Drawing.Size(242, 20);
            this.textBox_FilterCreateOptions.TabIndex = 110;
            this.textBox_FilterCreateOptions.Text = "0";
            this.textBox_FilterCreateOptions.MouseHover += new System.EventHandler(this.textBox_FilterCreateOptions_MouseHover);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(2, 157);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(138, 13);
            this.label7.TabIndex = 109;
            this.label7.Text = "Filter IO With CreateOptions";
            // 
            // button_FilterDisposition
            // 
            this.button_FilterDisposition.Location = new System.Drawing.Point(461, 126);
            this.button_FilterDisposition.Name = "button_FilterDisposition";
            this.button_FilterDisposition.Size = new System.Drawing.Size(41, 20);
            this.button_FilterDisposition.TabIndex = 108;
            this.button_FilterDisposition.Text = "...";
            this.button_FilterDisposition.UseVisualStyleBackColor = true;
            this.button_FilterDisposition.Click += new System.EventHandler(this.button_FilterDisposition_Click);
            // 
            // textBox_FilterDisposition
            // 
            this.textBox_FilterDisposition.Location = new System.Drawing.Point(199, 126);
            this.textBox_FilterDisposition.Name = "textBox_FilterDisposition";
            this.textBox_FilterDisposition.ReadOnly = true;
            this.textBox_FilterDisposition.Size = new System.Drawing.Size(242, 20);
            this.textBox_FilterDisposition.TabIndex = 107;
            this.textBox_FilterDisposition.Text = "0";
            this.textBox_FilterDisposition.MouseHover += new System.EventHandler(this.textBox_FilterDisposition_MouseHover);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(2, 131);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(122, 13);
            this.label8.TabIndex = 106;
            this.label8.Text = "Filter IO With Disposition";
            // 
            // button_FilterDesiredAccess
            // 
            this.button_FilterDesiredAccess.Location = new System.Drawing.Point(461, 100);
            this.button_FilterDesiredAccess.Name = "button_FilterDesiredAccess";
            this.button_FilterDesiredAccess.Size = new System.Drawing.Size(41, 20);
            this.button_FilterDesiredAccess.TabIndex = 105;
            this.button_FilterDesiredAccess.Text = "...";
            this.button_FilterDesiredAccess.UseVisualStyleBackColor = true;
            this.button_FilterDesiredAccess.Click += new System.EventHandler(this.button_FilterDesiredAccess_Click);
            // 
            // textBox_FilterDesiredAccess
            // 
            this.textBox_FilterDesiredAccess.Location = new System.Drawing.Point(199, 100);
            this.textBox_FilterDesiredAccess.Name = "textBox_FilterDesiredAccess";
            this.textBox_FilterDesiredAccess.ReadOnly = true;
            this.textBox_FilterDesiredAccess.Size = new System.Drawing.Size(242, 20);
            this.textBox_FilterDesiredAccess.TabIndex = 104;
            this.textBox_FilterDesiredAccess.Text = "0";
            this.textBox_FilterDesiredAccess.MouseHover += new System.EventHandler(this.textBox_FilterDesiredAccess_MouseHover);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(2, 105);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(142, 13);
            this.label17.TabIndex = 103;
            this.label17.Text = "Filter IO With DesiredAccess";
            // 
            // FilterRuleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(596, 589);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button_SaveFilterRule);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FilterRuleForm";
            this.Text = "Filter rule settings";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox_AccessControl.ResumeLayout(false);
            this.groupBox_AccessControl.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_ExcludeFilterMask;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_IncludeFilterMask;
        private System.Windows.Forms.Button button_SaveFilterRule;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox_AccessControl;
        private System.Windows.Forms.Button button_SelectExcludePID;
        private System.Windows.Forms.TextBox textBox_ExcludePID;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button_SelectIncludePID;
        private System.Windows.Forms.TextBox textBox_IncludePID;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBox_ExcludeProcessNames;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox textBox_IncludeProcessNames;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox textBox_ExcludeUserNames;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox textBox_IncludeUserNames;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Button button_SelectedEvents;
        private System.Windows.Forms.TextBox textBox_SelectedEvents;
        private System.Windows.Forms.Button button_RegisterMonitorIO;
        private System.Windows.Forms.TextBox textBox_MonitorIO;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Button button_ControlSettings;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button button_FilterCreateOptions;
        private System.Windows.Forms.TextBox textBox_FilterCreateOptions;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button button_FilterDisposition;
        private System.Windows.Forms.TextBox textBox_FilterDisposition;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button button_FilterDesiredAccess;
        private System.Windows.Forms.TextBox textBox_FilterDesiredAccess;
        private System.Windows.Forms.Label label17;
    }
}