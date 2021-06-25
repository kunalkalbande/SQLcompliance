namespace Idera.SQLcompliance.Application.GUI.Forms
{
   partial class Form_LoginNew
	{
      /// <summary>
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.Container components = null;

      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      protected override void Dispose(bool disposing)
      {
         if (disposing)
         {
            if (components != null)
            {
               components.Dispose();
            }
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_LoginNew));
            this.panelButtons = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnFinish = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.panelLeft = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panelSummary = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.textSummaryAccess = new System.Windows.Forms.TextBox();
            this.textSummaryLoginName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.lblInstance = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.panelLogin = new System.Windows.Forms.Panel();
            this.labelAgentTraceDescription = new System.Windows.Forms.Label();
            this.labelAgentTraceTitle = new System.Windows.Forms.Label();
            this.panel7 = new System.Windows.Forms.Panel();
            this.label13 = new System.Windows.Forms.Label();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.textName = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioDenyAccess = new System.Windows.Forms.RadioButton();
            this.radioGrantAccess = new System.Windows.Forms.RadioButton();
            this.panelAgentProperties = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.radioNo = new System.Windows.Forms.RadioButton();
            this.radioYes = new System.Windows.Forms.RadioButton();
            this.label7 = new System.Windows.Forms.Label();
            this.labelAgentDescription = new System.Windows.Forms.Label();
            this.labelAgentTitle = new System.Windows.Forms.Label();
            this.panelAccess = new System.Windows.Forms.Panel();
            this.panelDatabaseAccess = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.listDatabases = new System.Windows.Forms.CheckedListBox();
            this.chkWebApplicationAccess = new System.Windows.Forms.CheckBox();
            this.panelButtons.SuspendLayout();
            this.panelLeft.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panelSummary.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panelLogin.SuspendLayout();
            this.panel7.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panelAgentProperties.SuspendLayout();
            this.panelAccess.SuspendLayout();
            this.panelDatabaseAccess.SuspendLayout();
            this.panel3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelButtons
            // 
            this.panelButtons.Controls.Add(this.btnCancel);
            this.panelButtons.Controls.Add(this.btnFinish);
            this.panelButtons.Controls.Add(this.btnNext);
            this.panelButtons.Controls.Add(this.btnBack);
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelButtons.Location = new System.Drawing.Point(0, 334);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(558, 38);
            this.panelButtons.TabIndex = 9;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnCancel.Location = new System.Drawing.Point(490, 10);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(62, 20);
            this.btnCancel.TabIndex = 203;
            this.btnCancel.Text = "&Cancel";
            // 
            // btnFinish
            // 
            this.btnFinish.Enabled = false;
            this.btnFinish.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnFinish.Location = new System.Drawing.Point(420, 10);
            this.btnFinish.Name = "btnFinish";
            this.btnFinish.Size = new System.Drawing.Size(62, 20);
            this.btnFinish.TabIndex = 202;
            this.btnFinish.Text = "&Finish";
            this.btnFinish.Click += new System.EventHandler(this.btnFinish_Click);
            // 
            // btnNext
            // 
            this.btnNext.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnNext.Location = new System.Drawing.Point(350, 10);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(62, 20);
            this.btnNext.TabIndex = 201;
            this.btnNext.Text = "&Next >";
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnBack
            // 
            this.btnBack.Enabled = false;
            this.btnBack.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnBack.Location = new System.Drawing.Point(280, 10);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(62, 20);
            this.btnBack.TabIndex = 200;
            this.btnBack.Text = "< &Back";
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // panelLeft
            // 
            this.panelLeft.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.panelLeft.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelLeft.Controls.Add(this.pictureBox1);
            this.panelLeft.Location = new System.Drawing.Point(0, 0);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(112, 336);
            this.panelLeft.TabIndex = 10;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.pictureBox1.Image = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.Wizard_Login;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(112, 336);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // panelSummary
            // 
            this.panelSummary.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.panelSummary.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelSummary.Controls.Add(this.label8);
            this.panelSummary.Controls.Add(this.label9);
            this.panelSummary.Controls.Add(this.panel4);
            this.panelSummary.Location = new System.Drawing.Point(111, 0);
            this.panelSummary.Name = "panelSummary";
            this.panelSummary.Size = new System.Drawing.Size(448, 336);
            this.panelSummary.TabIndex = 27;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(14, 24);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(420, 28);
            this.label8.TabIndex = 15;
            this.label8.Text = "Ready to create the SQL Server Login";
            // 
            // label9
            // 
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(14, 8);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(384, 16);
            this.label9.TabIndex = 14;
            this.label9.Text = "Summary";
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.SystemColors.Control;
            this.panel4.Controls.Add(this.textSummaryAccess);
            this.panel4.Controls.Add(this.textSummaryLoginName);
            this.panel4.Controls.Add(this.label5);
            this.panel4.Controls.Add(this.lblInstance);
            this.panel4.Controls.Add(this.label11);
            this.panel4.Controls.Add(this.label10);
            this.panel4.Location = new System.Drawing.Point(0, 60);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(448, 276);
            this.panel4.TabIndex = 16;
            // 
            // textSummaryAccess
            // 
            this.textSummaryAccess.Location = new System.Drawing.Point(360, 80);
            this.textSummaryAccess.Name = "textSummaryAccess";
            this.textSummaryAccess.ReadOnly = true;
            this.textSummaryAccess.Size = new System.Drawing.Size(44, 20);
            this.textSummaryAccess.TabIndex = 7;
            this.textSummaryAccess.Text = "No";
            // 
            // textSummaryLoginName
            // 
            this.textSummaryLoginName.Location = new System.Drawing.Point(104, 52);
            this.textSummaryLoginName.Name = "textSummaryLoginName";
            this.textSummaryLoginName.ReadOnly = true;
            this.textSummaryLoginName.Size = new System.Drawing.Size(300, 20);
            this.textSummaryLoginName.TabIndex = 5;
            this.textSummaryLoginName.Text = "Computer";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(28, 84);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(320, 16);
            this.label5.TabIndex = 4;
            this.label5.Text = "Grant login permission to administer SQL Compliance Manager:";
            // 
            // lblInstance
            // 
            this.lblInstance.Location = new System.Drawing.Point(28, 56);
            this.lblInstance.Name = "lblInstance";
            this.lblInstance.Size = new System.Drawing.Size(92, 16);
            this.lblInstance.TabIndex = 2;
            this.lblInstance.Text = "Login Name:";
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(12, 12);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(420, 16);
            this.label11.TabIndex = 1;
            this.label11.Text = "You are ready to create the new SQL Server Login using the following options:";
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(12, 136);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(412, 44);
            this.label10.TabIndex = 0;
            this.label10.Text = "Click Finish to create the new login.";
            // 
            // panelLogin
            // 
            this.panelLogin.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.panelLogin.Controls.Add(this.labelAgentTraceDescription);
            this.panelLogin.Controls.Add(this.labelAgentTraceTitle);
            this.panelLogin.Controls.Add(this.panel7);
            this.panelLogin.Location = new System.Drawing.Point(111, 0);
            this.panelLogin.Name = "panelLogin";
            this.panelLogin.Size = new System.Drawing.Size(448, 336);
            this.panelLogin.TabIndex = 29;
            // 
            // labelAgentTraceDescription
            // 
            this.labelAgentTraceDescription.Location = new System.Drawing.Point(14, 24);
            this.labelAgentTraceDescription.Name = "labelAgentTraceDescription";
            this.labelAgentTraceDescription.Size = new System.Drawing.Size(420, 28);
            this.labelAgentTraceDescription.TabIndex = 52;
            this.labelAgentTraceDescription.Text = "Specify properties for the new SQL Server login";
            // 
            // labelAgentTraceTitle
            // 
            this.labelAgentTraceTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelAgentTraceTitle.Location = new System.Drawing.Point(14, 8);
            this.labelAgentTraceTitle.Name = "labelAgentTraceTitle";
            this.labelAgentTraceTitle.Size = new System.Drawing.Size(384, 16);
            this.labelAgentTraceTitle.TabIndex = 51;
            this.labelAgentTraceTitle.Text = "SQL Server Windows Authentication";
            // 
            // panel7
            // 
            this.panel7.BackColor = System.Drawing.SystemColors.Control;
            this.panel7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel7.Controls.Add(this.chkWebApplicationAccess);
            this.panel7.Controls.Add(this.label13);
            this.panel7.Controls.Add(this.btnBrowse);
            this.panel7.Controls.Add(this.textName);
            this.panel7.Controls.Add(this.label6);
            this.panel7.Controls.Add(this.groupBox1);
            this.panel7.Location = new System.Drawing.Point(0, 60);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(448, 276);
            this.panel7.TabIndex = 53;
            // 
            // label13
            // 
            this.label13.Location = new System.Drawing.Point(12, 12);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(420, 16);
            this.label13.TabIndex = 7;
            this.label13.Text = "Specify a Windows user account to be used to access SQL Compliance Manager:";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(404, 36);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(24, 20);
            this.btnBrowse.TabIndex = 6;
            this.btnBrowse.Text = "...";
            this.btnBrowse.Visible = false;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // textName
            // 
            this.textName.Location = new System.Drawing.Point(52, 36);
            this.textName.MaxLength = 128;
            this.textName.Name = "textName";
            this.textName.Size = new System.Drawing.Size(344, 20);
            this.textName.TabIndex = 5;
            this.textName.TextChanged += new System.EventHandler(this.textName_TextChanged);
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(12, 40);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(40, 16);
            this.label6.TabIndex = 4;
            this.label6.Text = "Name:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioDenyAccess);
            this.groupBox1.Controls.Add(this.radioGrantAccess);
            this.groupBox1.Location = new System.Drawing.Point(52, 64);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.groupBox1.Size = new System.Drawing.Size(344, 72);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Security Access";
            // 
            // radioDenyAccess
            // 
            this.radioDenyAccess.Location = new System.Drawing.Point(12, 44);
            this.radioDenyAccess.Name = "radioDenyAccess";
            this.radioDenyAccess.Size = new System.Drawing.Size(92, 16);
            this.radioDenyAccess.TabIndex = 8;
            this.radioDenyAccess.Text = "Deny access";
            // 
            // radioGrantAccess
            // 
            this.radioGrantAccess.Checked = true;
            this.radioGrantAccess.Location = new System.Drawing.Point(12, 20);
            this.radioGrantAccess.Name = "radioGrantAccess";
            this.radioGrantAccess.Size = new System.Drawing.Size(92, 16);
            this.radioGrantAccess.TabIndex = 7;
            this.radioGrantAccess.TabStop = true;
            this.radioGrantAccess.Text = "Grant access";
            // 
            // panelAgentProperties
            // 
            this.panelAgentProperties.BackColor = System.Drawing.SystemColors.Control;
            this.panelAgentProperties.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelAgentProperties.Controls.Add(this.label1);
            this.panelAgentProperties.Controls.Add(this.radioNo);
            this.panelAgentProperties.Controls.Add(this.radioYes);
            this.panelAgentProperties.Controls.Add(this.label7);
            this.panelAgentProperties.Location = new System.Drawing.Point(0, 60);
            this.panelAgentProperties.Name = "panelAgentProperties";
            this.panelAgentProperties.Size = new System.Drawing.Size(448, 276);
            this.panelAgentProperties.TabIndex = 53;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 80);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(388, 16);
            this.label1.TabIndex = 55;
            this.label1.Text = "Do you want to add this login to the System Administrators server role?";
            // 
            // radioNo
            // 
            this.radioNo.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.radioNo.Checked = true;
            this.radioNo.Location = new System.Drawing.Point(20, 128);
            this.radioNo.Name = "radioNo";
            this.radioNo.Size = new System.Drawing.Size(336, 20);
            this.radioNo.TabIndex = 54;
            this.radioNo.TabStop = true;
            this.radioNo.Text = "No, only allow this user the ability to view collected audit data.";
            this.radioNo.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            // 
            // radioYes
            // 
            this.radioYes.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.radioYes.Location = new System.Drawing.Point(20, 104);
            this.radioYes.Name = "radioYes";
            this.radioYes.Size = new System.Drawing.Size(432, 20);
            this.radioYes.TabIndex = 53;
            this.radioYes.Text = "Yes, grant this user permission to configure SQL Compliance Manager settings.";
            this.radioYes.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(12, 12);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(404, 56);
            this.label7.TabIndex = 52;
            this.label7.Text = resources.GetString("label7.Text");
            // 
            // labelAgentDescription
            // 
            this.labelAgentDescription.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.labelAgentDescription.Location = new System.Drawing.Point(14, 24);
            this.labelAgentDescription.Name = "labelAgentDescription";
            this.labelAgentDescription.Size = new System.Drawing.Size(420, 28);
            this.labelAgentDescription.TabIndex = 52;
            this.labelAgentDescription.Text = "Specify the level of permissions within SQL Compliance Manager";
            // 
            // labelAgentTitle
            // 
            this.labelAgentTitle.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.labelAgentTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelAgentTitle.Location = new System.Drawing.Point(14, 8);
            this.labelAgentTitle.Name = "labelAgentTitle";
            this.labelAgentTitle.Size = new System.Drawing.Size(384, 16);
            this.labelAgentTitle.TabIndex = 51;
            this.labelAgentTitle.Text = "SQL Compliance Manager Permissions";
            // 
            // panelAccess
            // 
            this.panelAccess.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.panelAccess.Controls.Add(this.panelAgentProperties);
            this.panelAccess.Controls.Add(this.labelAgentDescription);
            this.panelAccess.Controls.Add(this.labelAgentTitle);
            this.panelAccess.Location = new System.Drawing.Point(111, 0);
            this.panelAccess.Name = "panelAccess";
            this.panelAccess.Size = new System.Drawing.Size(448, 336);
            this.panelAccess.TabIndex = 11;
            // 
            // panelDatabaseAccess
            // 
            this.panelDatabaseAccess.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.panelDatabaseAccess.Controls.Add(this.label2);
            this.panelDatabaseAccess.Controls.Add(this.label3);
            this.panelDatabaseAccess.Controls.Add(this.panel3);
            this.panelDatabaseAccess.Location = new System.Drawing.Point(111, 0);
            this.panelDatabaseAccess.Name = "panelDatabaseAccess";
            this.panelDatabaseAccess.Size = new System.Drawing.Size(448, 336);
            this.panelDatabaseAccess.TabIndex = 30;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(14, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(420, 28);
            this.label2.TabIndex = 52;
            this.label2.Text = "Specify the event databases that can be accessed by this login";
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(14, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(384, 16);
            this.label3.TabIndex = 51;
            this.label3.Text = "Database Access";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.SystemColors.Control;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.groupBox2);
            this.panel3.Controls.Add(this.label4);
            this.panel3.Controls.Add(this.label12);
            this.panel3.Controls.Add(this.listDatabases);
            this.panel3.Location = new System.Drawing.Point(0, 60);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(448, 276);
            this.panel3.TabIndex = 53;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBox3);
            this.groupBox2.Controls.Add(this.checkBox2);
            this.groupBox2.Controls.Add(this.checkBox1);
            this.groupBox2.Location = new System.Drawing.Point(12, 140);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(416, 76);
            this.groupBox2.TabIndex = 44;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Permissions in \'database\'";
            // 
            // checkBox3
            // 
            this.checkBox3.Location = new System.Drawing.Point(8, 56);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(312, 16);
            this.checkBox3.TabIndex = 2;
            this.checkBox3.Text = "Cannot view any data in this database.";
            // 
            // checkBox2
            // 
            this.checkBox2.Location = new System.Drawing.Point(8, 36);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(316, 16);
            this.checkBox2.TabIndex = 1;
            this.checkBox2.Text = "Can view SQL statements associated with audit events";
            // 
            // checkBox1
            // 
            this.checkBox1.Location = new System.Drawing.Point(8, 16);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(144, 16);
            this.checkBox1.TabIndex = 0;
            this.checkBox1.Text = "Can view audit events";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(12, 224);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(416, 32);
            this.label4.TabIndex = 43;
            this.label4.Text = "Note: Granting the ability to view the data in an events database is only necessa" +
    "ry if the database is not set to give access to all users by default.";
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(12, 12);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(368, 16);
            this.label12.TabIndex = 42;
            this.label12.Text = "Specify which event databases can be accessed by this login:";
            // 
            // listDatabases
            // 
            this.listDatabases.Location = new System.Drawing.Point(12, 28);
            this.listDatabases.Name = "listDatabases";
            this.listDatabases.Size = new System.Drawing.Size(416, 109);
            this.listDatabases.TabIndex = 41;
            this.listDatabases.ThreeDCheckBoxes = true;
            // 
            // chkWebApplicationAccess
            // 
            this.chkWebApplicationAccess.AutoSize = true;
            this.chkWebApplicationAccess.Location = new System.Drawing.Point(52, 143);
            this.chkWebApplicationAccess.Name = "chkWebApplicationAccess";
            this.chkWebApplicationAccess.Size = new System.Drawing.Size(142, 17);
            this.chkWebApplicationAccess.TabIndex = 10;
            this.chkWebApplicationAccess.Text = "Web Application Access";
            this.chkWebApplicationAccess.UseVisualStyleBackColor = true;
            // 
            // Form_LoginNew
            // 
            this.AcceptButton = this.btnNext;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(558, 372);
            this.Controls.Add(this.panelLogin);
            this.Controls.Add(this.panelLeft);
            this.Controls.Add(this.panelButtons);
            this.Controls.Add(this.panelAccess);
            this.Controls.Add(this.panelSummary);
            this.Controls.Add(this.panelDatabaseAccess);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form_LoginNew";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "New SQL Server Login";
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.Form_LoginNew_HelpRequested);
            this.panelButtons.ResumeLayout(false);
            this.panelLeft.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panelSummary.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panelLogin.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.panelAgentProperties.ResumeLayout(false);
            this.panelAccess.ResumeLayout(false);
            this.panelDatabaseAccess.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

      }
      #endregion

      private System.Windows.Forms.Button btnCancel;
      private System.Windows.Forms.Button btnFinish;
      private System.Windows.Forms.Button btnNext;
      private System.Windows.Forms.Button btnBack;
      private System.Windows.Forms.Panel panelButtons;
      private System.Windows.Forms.Panel panelLeft;
      private System.Windows.Forms.PictureBox pictureBox1;
      private System.Windows.Forms.Panel panelSummary;
      private System.Windows.Forms.Label label8;
      private System.Windows.Forms.Label label9;
      private System.Windows.Forms.Panel panel4;
      private System.Windows.Forms.Label label10;
      private System.Windows.Forms.Label label11;
      private System.Windows.Forms.Label lblInstance;
      private System.Windows.Forms.Panel panel7;
      private System.Windows.Forms.Label labelAgentTraceTitle;
      private System.Windows.Forms.Label label5;
      private System.Windows.Forms.Label label6;
      private System.Windows.Forms.GroupBox groupBox1;
      private System.Windows.Forms.Panel panelLogin;
      private System.Windows.Forms.Label labelAgentTraceDescription;
      private System.Windows.Forms.TextBox textSummaryAccess;
      private System.Windows.Forms.TextBox textSummaryLoginName;
      private System.Windows.Forms.TextBox textName;
      private System.Windows.Forms.Panel panelAgentProperties;
      private System.Windows.Forms.RadioButton radioNo;
      private System.Windows.Forms.RadioButton radioYes;
      private System.Windows.Forms.Label label7;
      private System.Windows.Forms.Label labelAgentDescription;
      private System.Windows.Forms.Label labelAgentTitle;
      private System.Windows.Forms.Panel panelAccess;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.Panel panelDatabaseAccess;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.Panel panel3;
      private System.Windows.Forms.Label label12;
      private System.Windows.Forms.CheckedListBox listDatabases;
      private System.Windows.Forms.GroupBox groupBox2;
      private System.Windows.Forms.CheckBox checkBox2;
      private System.Windows.Forms.CheckBox checkBox1;
      private System.Windows.Forms.CheckBox checkBox3;
      private System.Windows.Forms.Label label4;
      private System.Windows.Forms.RadioButton radioDenyAccess;
      private System.Windows.Forms.RadioButton radioGrantAccess;
      private System.Windows.Forms.Button btnBrowse;
      private System.Windows.Forms.Label label13;
      private System.Windows.Forms.CheckBox chkWebApplicationAccess;
	}
}