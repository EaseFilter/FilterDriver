namespace AutoFileCryptTool
{
    partial class Form_FileCrypt
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_FileCrypt));
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("");
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem(new string[] {
            "You can drag&drop the empty folders to here"}, -1, System.Drawing.Color.DarkOrange, System.Drawing.Color.Empty, new System.Drawing.Font("Arial Rounded MT Bold", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))));
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem(new string[] {
            "The encryption driver will encrypt/decrypt the files automatically"}, -1, System.Drawing.Color.DarkOrange, System.Drawing.Color.Empty, new System.Drawing.Font("Arial Rounded MT Bold", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))));
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem(new string[] {
            "The files will be encrypted with 256 bit AES algorithm"}, -1, System.Drawing.Color.DarkOrange, System.Drawing.Color.Empty, new System.Drawing.Font("Arial Rounded MT Bold", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))));
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem(new string[] {
            "Go to setting to configure the while list and black list processes"}, -1, System.Drawing.Color.DarkOrange, System.Drawing.Color.Empty, new System.Drawing.Font("Arial Rounded MT Bold", 9F));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.tabPage_Settings = new System.Windows.Forms.TabPage();
            this.button_ApplySetting = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.textBox_BlackList = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.textBox_WhiteList = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label_user = new System.Windows.Forms.Label();
            this.tabPage_Help = new System.Windows.Forms.TabPage();
            this.linkLabel_Report = new System.Windows.Forms.LinkLabel();
            this.linkLabel_SDK = new System.Windows.Forms.LinkLabel();
            this.linkLabel_Updates = new System.Windows.Forms.LinkLabel();
            this.label_VersionInfo = new System.Windows.Forms.Label();
            this.tabPage_DecryptFile = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_DecryptTargetName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button_BrowseFileToDecrypt = new System.Windows.Forms.Button();
            this.textBox_DecryptSourceName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button_StartToDecrypt = new System.Windows.Forms.Button();
            this.tabPage_EncryptFile = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_EncryptTargetName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox_EncryptSourceName = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.button_BrowseEncryptFile = new System.Windows.Forms.Button();
            this.button_StartToEncrypt = new System.Windows.Forms.Button();
            this.tabPage_Folder = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button_Stop = new System.Windows.Forms.Button();
            this.button_Start = new System.Windows.Forms.Button();
            this.button_RemoveFolder = new System.Windows.Forms.Button();
            this.button_AddFolder = new System.Windows.Forms.Button();
            this.listView_Folders = new System.Windows.Forms.ListView();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage_Settings.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.tabPage_Help.SuspendLayout();
            this.tabPage_DecryptFile.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tabPage_EncryptFile.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabPage_Folder.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "folder_ok.png");
            // 
            // tabPage_Settings
            // 
            this.tabPage_Settings.Controls.Add(this.button_ApplySetting);
            this.tabPage_Settings.Controls.Add(this.groupBox5);
            this.tabPage_Settings.Location = new System.Drawing.Point(4, 22);
            this.tabPage_Settings.Name = "tabPage_Settings";
            this.tabPage_Settings.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_Settings.Size = new System.Drawing.Size(548, 255);
            this.tabPage_Settings.TabIndex = 5;
            this.tabPage_Settings.Text = "Settings";
            this.tabPage_Settings.UseVisualStyleBackColor = true;
            // 
            // button_ApplySetting
            // 
            this.button_ApplySetting.Location = new System.Drawing.Point(431, 215);
            this.button_ApplySetting.Name = "button_ApplySetting";
            this.button_ApplySetting.Size = new System.Drawing.Size(97, 20);
            this.button_ApplySetting.TabIndex = 46;
            this.button_ApplySetting.Text = "Apply settings";
            this.button_ApplySetting.UseVisualStyleBackColor = true;
            this.button_ApplySetting.Click += new System.EventHandler(this.button_ApplySetting_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.textBox_BlackList);
            this.groupBox5.Controls.Add(this.label10);
            this.groupBox5.Controls.Add(this.textBox_WhiteList);
            this.groupBox5.Controls.Add(this.label9);
            this.groupBox5.Controls.Add(this.label_user);
            this.groupBox5.Location = new System.Drawing.Point(25, 17);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(503, 153);
            this.groupBox5.TabIndex = 0;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Access control";
            // 
            // textBox_BlackList
            // 
            this.textBox_BlackList.Location = new System.Drawing.Point(151, 93);
            this.textBox_BlackList.Name = "textBox_BlackList";
            this.textBox_BlackList.Size = new System.Drawing.Size(287, 20);
            this.textBox_BlackList.TabIndex = 51;
            this.textBox_BlackList.Text = "explorer.exe";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(15, 96);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(119, 15);
            this.label10.TabIndex = 50;
            this.label10.Text = "Black List Processes";
            // 
            // textBox_WhiteList
            // 
            this.textBox_WhiteList.Location = new System.Drawing.Point(151, 38);
            this.textBox_WhiteList.Name = "textBox_WhiteList";
            this.textBox_WhiteList.Size = new System.Drawing.Size(287, 20);
            this.textBox_WhiteList.TabIndex = 49;
            this.textBox_WhiteList.Text = "*";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(15, 38);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(120, 15);
            this.label9.TabIndex = 48;
            this.label9.Text = "White List Processes";
            // 
            // label_user
            // 
            this.label_user.AutoSize = true;
            this.label_user.Location = new System.Drawing.Point(148, 68);
            this.label_user.Name = "label_user";
            this.label_user.Size = new System.Drawing.Size(225, 15);
            this.label_user.TabIndex = 47;
            this.label_user.Text = "(split with \';\' for multiple process names)";
            // 
            // tabPage_Help
            // 
            this.tabPage_Help.Controls.Add(this.linkLabel_Report);
            this.tabPage_Help.Controls.Add(this.linkLabel_SDK);
            this.tabPage_Help.Controls.Add(this.linkLabel_Updates);
            this.tabPage_Help.Controls.Add(this.label_VersionInfo);
            this.tabPage_Help.Location = new System.Drawing.Point(4, 22);
            this.tabPage_Help.Name = "tabPage_Help";
            this.tabPage_Help.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_Help.Size = new System.Drawing.Size(548, 255);
            this.tabPage_Help.TabIndex = 4;
            this.tabPage_Help.Text = "Help&Support";
            this.tabPage_Help.UseVisualStyleBackColor = true;
            // 
            // linkLabel_Report
            // 
            this.linkLabel_Report.AutoSize = true;
            this.linkLabel_Report.Location = new System.Drawing.Point(24, 84);
            this.linkLabel_Report.Name = "linkLabel_Report";
            this.linkLabel_Report.Size = new System.Drawing.Size(199, 15);
            this.linkLabel_Report.TabIndex = 11;
            this.linkLabel_Report.TabStop = true;
            this.linkLabel_Report.Text = "Report a bug or make a suggestion";
            this.linkLabel_Report.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_ReportClicked);
            // 
            // linkLabel_SDK
            // 
            this.linkLabel_SDK.AutoSize = true;
            this.linkLabel_SDK.Location = new System.Drawing.Point(24, 120);
            this.linkLabel_SDK.Name = "linkLabel_SDK";
            this.linkLabel_SDK.Size = new System.Drawing.Size(335, 15);
            this.linkLabel_SDK.TabIndex = 10;
            this.linkLabel_SDK.TabStop = true;
            this.linkLabel_SDK.Text = "Online Transparent Encryption SDK and Example Download";
            this.linkLabel_SDK.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_SDK_LinkClicked);
            // 
            // linkLabel_Updates
            // 
            this.linkLabel_Updates.AutoSize = true;
            this.linkLabel_Updates.Location = new System.Drawing.Point(24, 43);
            this.linkLabel_Updates.Name = "linkLabel_Updates";
            this.linkLabel_Updates.Size = new System.Drawing.Size(105, 15);
            this.linkLabel_Updates.TabIndex = 9;
            this.linkLabel_Updates.TabStop = true;
            this.linkLabel_Updates.Text = "Check for updates";
            this.linkLabel_Updates.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_Report_LinkClicked);
            // 
            // label_VersionInfo
            // 
            this.label_VersionInfo.AutoSize = true;
            this.label_VersionInfo.Location = new System.Drawing.Point(24, 26);
            this.label_VersionInfo.Name = "label_VersionInfo";
            this.label_VersionInfo.Size = new System.Drawing.Size(0, 15);
            this.label_VersionInfo.TabIndex = 9;
            // 
            // tabPage_DecryptFile
            // 
            this.tabPage_DecryptFile.Controls.Add(this.groupBox3);
            this.tabPage_DecryptFile.Controls.Add(this.button_StartToDecrypt);
            this.tabPage_DecryptFile.Location = new System.Drawing.Point(4, 22);
            this.tabPage_DecryptFile.Name = "tabPage_DecryptFile";
            this.tabPage_DecryptFile.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_DecryptFile.Size = new System.Drawing.Size(548, 255);
            this.tabPage_DecryptFile.TabIndex = 2;
            this.tabPage_DecryptFile.Text = "Decrypt File";
            this.tabPage_DecryptFile.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.textBox_DecryptTargetName);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.button_BrowseFileToDecrypt);
            this.groupBox3.Controls.Add(this.textBox_DecryptSourceName);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Location = new System.Drawing.Point(17, 29);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(471, 152);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(144, 73);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(297, 15);
            this.label4.TabIndex = 9;
            this.label4.Text = "target file name can be the same as the source name";
            // 
            // textBox_DecryptTargetName
            // 
            this.textBox_DecryptTargetName.Location = new System.Drawing.Point(147, 98);
            this.textBox_DecryptTargetName.Name = "textBox_DecryptTargetName";
            this.textBox_DecryptTargetName.Size = new System.Drawing.Size(258, 20);
            this.textBox_DecryptTargetName.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 101);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 15);
            this.label1.TabIndex = 7;
            this.label1.Text = "Target File name";
            // 
            // button_BrowseFileToDecrypt
            // 
            this.button_BrowseFileToDecrypt.Location = new System.Drawing.Point(411, 39);
            this.button_BrowseFileToDecrypt.Name = "button_BrowseFileToDecrypt";
            this.button_BrowseFileToDecrypt.Size = new System.Drawing.Size(44, 23);
            this.button_BrowseFileToDecrypt.TabIndex = 6;
            this.button_BrowseFileToDecrypt.Text = "...";
            this.button_BrowseFileToDecrypt.UseVisualStyleBackColor = true;
            this.button_BrowseFileToDecrypt.Click += new System.EventHandler(this.button_BrowseFileToDecrypt_Click);
            // 
            // textBox_DecryptSourceName
            // 
            this.textBox_DecryptSourceName.Location = new System.Drawing.Point(147, 41);
            this.textBox_DecryptSourceName.Name = "textBox_DecryptSourceName";
            this.textBox_DecryptSourceName.Size = new System.Drawing.Size(258, 20);
            this.textBox_DecryptSourceName.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(104, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "Source File name";
            // 
            // button_StartToDecrypt
            // 
            this.button_StartToDecrypt.Location = new System.Drawing.Point(413, 187);
            this.button_StartToDecrypt.Name = "button_StartToDecrypt";
            this.button_StartToDecrypt.Size = new System.Drawing.Size(75, 23);
            this.button_StartToDecrypt.TabIndex = 6;
            this.button_StartToDecrypt.Text = "Start";
            this.button_StartToDecrypt.UseVisualStyleBackColor = true;
            this.button_StartToDecrypt.Click += new System.EventHandler(this.button_StartToDecrypt_Click);
            // 
            // tabPage_EncryptFile
            // 
            this.tabPage_EncryptFile.Controls.Add(this.groupBox2);
            this.tabPage_EncryptFile.Controls.Add(this.button_StartToEncrypt);
            this.tabPage_EncryptFile.Location = new System.Drawing.Point(4, 22);
            this.tabPage_EncryptFile.Name = "tabPage_EncryptFile";
            this.tabPage_EncryptFile.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_EncryptFile.Size = new System.Drawing.Size(548, 255);
            this.tabPage_EncryptFile.TabIndex = 1;
            this.tabPage_EncryptFile.Text = "Encrypt File";
            this.tabPage_EncryptFile.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.textBox_EncryptTargetName);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.textBox_EncryptSourceName);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.button_BrowseEncryptFile);
            this.groupBox2.Location = new System.Drawing.Point(12, 31);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(471, 157);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(137, 70);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(297, 15);
            this.label3.TabIndex = 14;
            this.label3.Text = "target file name can be the same as the source name";
            // 
            // textBox_EncryptTargetName
            // 
            this.textBox_EncryptTargetName.Location = new System.Drawing.Point(140, 95);
            this.textBox_EncryptTargetName.Name = "textBox_EncryptTargetName";
            this.textBox_EncryptTargetName.Size = new System.Drawing.Size(258, 20);
            this.textBox_EncryptTargetName.TabIndex = 13;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 98);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(100, 15);
            this.label5.TabIndex = 12;
            this.label5.Text = "Target File name";
            // 
            // textBox_EncryptSourceName
            // 
            this.textBox_EncryptSourceName.Location = new System.Drawing.Point(140, 38);
            this.textBox_EncryptSourceName.Name = "textBox_EncryptSourceName";
            this.textBox_EncryptSourceName.Size = new System.Drawing.Size(258, 20);
            this.textBox_EncryptSourceName.TabIndex = 11;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(16, 41);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(104, 15);
            this.label6.TabIndex = 10;
            this.label6.Text = "Source File name";
            // 
            // button_BrowseEncryptFile
            // 
            this.button_BrowseEncryptFile.Location = new System.Drawing.Point(404, 36);
            this.button_BrowseEncryptFile.Name = "button_BrowseEncryptFile";
            this.button_BrowseEncryptFile.Size = new System.Drawing.Size(44, 23);
            this.button_BrowseEncryptFile.TabIndex = 6;
            this.button_BrowseEncryptFile.Text = "...";
            this.button_BrowseEncryptFile.UseVisualStyleBackColor = true;
            this.button_BrowseEncryptFile.Click += new System.EventHandler(this.button_BrowseEncryptFile_Click);
            // 
            // button_StartToEncrypt
            // 
            this.button_StartToEncrypt.Location = new System.Drawing.Point(408, 204);
            this.button_StartToEncrypt.Name = "button_StartToEncrypt";
            this.button_StartToEncrypt.Size = new System.Drawing.Size(75, 23);
            this.button_StartToEncrypt.TabIndex = 4;
            this.button_StartToEncrypt.Text = "Start";
            this.button_StartToEncrypt.UseVisualStyleBackColor = true;
            this.button_StartToEncrypt.Click += new System.EventHandler(this.button_StartToEncrypt_Click);
            // 
            // tabPage_Folder
            // 
            this.tabPage_Folder.Controls.Add(this.splitContainer1);
            this.tabPage_Folder.Location = new System.Drawing.Point(4, 22);
            this.tabPage_Folder.Name = "tabPage_Folder";
            this.tabPage_Folder.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_Folder.Size = new System.Drawing.Size(548, 255);
            this.tabPage_Folder.TabIndex = 0;
            this.tabPage_Folder.Text = "Auto Folder Encrption";
            this.tabPage_Folder.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.listView_Folders);
            this.splitContainer1.Size = new System.Drawing.Size(542, 249);
            this.splitContainer1.SplitterDistance = 114;
            this.splitContainer1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button_Stop);
            this.groupBox1.Controls.Add(this.button_Start);
            this.groupBox1.Controls.Add(this.button_RemoveFolder);
            this.groupBox1.Controls.Add(this.button_AddFolder);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(114, 249);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            // 
            // button_Stop
            // 
            this.button_Stop.Location = new System.Drawing.Point(5, 185);
            this.button_Stop.Name = "button_Stop";
            this.button_Stop.Size = new System.Drawing.Size(94, 23);
            this.button_Stop.TabIndex = 3;
            this.button_Stop.Text = "Stop Service";
            this.button_Stop.UseVisualStyleBackColor = true;
            this.button_Stop.Click += new System.EventHandler(this.button_Stop_Click);
            // 
            // button_Start
            // 
            this.button_Start.Location = new System.Drawing.Point(6, 143);
            this.button_Start.Name = "button_Start";
            this.button_Start.Size = new System.Drawing.Size(94, 23);
            this.button_Start.TabIndex = 2;
            this.button_Start.Text = "Start Service";
            this.button_Start.UseVisualStyleBackColor = true;
            this.button_Start.Click += new System.EventHandler(this.button_Start_Click);
            // 
            // button_RemoveFolder
            // 
            this.button_RemoveFolder.Location = new System.Drawing.Point(6, 72);
            this.button_RemoveFolder.Name = "button_RemoveFolder";
            this.button_RemoveFolder.Size = new System.Drawing.Size(94, 23);
            this.button_RemoveFolder.TabIndex = 1;
            this.button_RemoveFolder.Text = "Remove folder";
            this.button_RemoveFolder.UseVisualStyleBackColor = true;
            this.button_RemoveFolder.Click += new System.EventHandler(this.button_RemoveFolder_Click);
            // 
            // button_AddFolder
            // 
            this.button_AddFolder.Location = new System.Drawing.Point(6, 34);
            this.button_AddFolder.Name = "button_AddFolder";
            this.button_AddFolder.Size = new System.Drawing.Size(94, 23);
            this.button_AddFolder.TabIndex = 0;
            this.button_AddFolder.Text = "Add folder";
            this.button_AddFolder.UseVisualStyleBackColor = true;
            this.button_AddFolder.Click += new System.EventHandler(this.button_AddFolder_Click);
            // 
            // listView_Folders
            // 
            this.listView_Folders.AllowDrop = true;
            this.listView_Folders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView_Folders.FullRowSelect = true;
            this.listView_Folders.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4,
            listViewItem5});
            this.listView_Folders.Location = new System.Drawing.Point(0, 0);
            this.listView_Folders.Name = "listView_Folders";
            this.listView_Folders.Size = new System.Drawing.Size(424, 249);
            this.listView_Folders.SmallImageList = this.imageList1;
            this.listView_Folders.TabIndex = 0;
            this.listView_Folders.UseCompatibleStateImageBehavior = false;
            this.listView_Folders.View = System.Windows.Forms.View.List;
            this.listView_Folders.DragDrop += new System.Windows.Forms.DragEventHandler(this.listView_Folders_DragDrop);
            this.listView_Folders.DragEnter += new System.Windows.Forms.DragEventHandler(this.listView_Folders_DragEnter);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage_Folder);
            this.tabControl1.Controls.Add(this.tabPage_EncryptFile);
            this.tabControl1.Controls.Add(this.tabPage_DecryptFile);
            this.tabControl1.Controls.Add(this.tabPage_Settings);
            this.tabControl1.Controls.Add(this.tabPage_Help);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(556, 281);
            this.tabControl1.TabIndex = 0;
            // 
            // Form_FileCrypt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(556, 281);
            this.Controls.Add(this.tabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form_FileCrypt";
            this.Text = "Auto FileCrypt Tool";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form_FileCrypt_FormClosed);
            this.tabPage_Settings.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.tabPage_Help.ResumeLayout(false);
            this.tabPage_Help.PerformLayout();
            this.tabPage_DecryptFile.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tabPage_EncryptFile.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabPage_Folder.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.TabPage tabPage_Settings;
        private System.Windows.Forms.TabPage tabPage_Help;
        private System.Windows.Forms.LinkLabel linkLabel_SDK;
        private System.Windows.Forms.LinkLabel linkLabel_Updates;
        private System.Windows.Forms.Label label_VersionInfo;
        private System.Windows.Forms.TabPage tabPage_DecryptFile;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button button_BrowseFileToDecrypt;
        private System.Windows.Forms.TextBox textBox_DecryptSourceName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button_StartToDecrypt;
        private System.Windows.Forms.TabPage tabPage_EncryptFile;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button_BrowseEncryptFile;
        private System.Windows.Forms.Button button_StartToEncrypt;
        private System.Windows.Forms.TabPage tabPage_Folder;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button_RemoveFolder;
        private System.Windows.Forms.Button button_AddFolder;
        private System.Windows.Forms.ListView listView_Folders;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button button_ApplySetting;
        private System.Windows.Forms.Label label_user;
        private System.Windows.Forms.TextBox textBox_BlackList;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBox_WhiteList;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.LinkLabel linkLabel_Report;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox_DecryptTargetName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_EncryptTargetName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox_EncryptSourceName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button button_Stop;
        private System.Windows.Forms.Button button_Start;
    }
}

