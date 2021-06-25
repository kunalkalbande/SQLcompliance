namespace Idera.SQLcompliance.Application.GUI.Forms
{
   partial class Form_DatabaseNew
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_DatabaseNew));
         this.panelLeft = new System.Windows.Forms.Panel();
         this.pictureBox1 = new System.Windows.Forms.PictureBox();
         this.panelGeneral = new System.Windows.Forms.Panel();
         this.lblGeneralDescription = new System.Windows.Forms.Label();
         this.lblGeneralTitle = new System.Windows.Forms.Label();
         this.panelGeneralProperties = new System.Windows.Forms.Panel();
         this.comboServer = new System.Windows.Forms.ComboBox();
         this.lblServer = new System.Windows.Forms.Label();
         this.btnCancel = new System.Windows.Forms.Button();
         this.btnFinish = new System.Windows.Forms.Button();
         this.btnNext = new System.Windows.Forms.Button();
         this.btnBack = new System.Windows.Forms.Button();
         this.panelAudit = new System.Windows.Forms.Panel();
         this.linkLblHelpBestPractices = new System.Windows.Forms.LinkLabel();
         this.lblAuditDescription = new System.Windows.Forms.Label();
         this.lblAuditTitle = new System.Windows.Forms.Label();
         this.panelAuditProperties = new System.Windows.Forms.Panel();
         this.chkCaptureSQL = new System.Windows.Forms.CheckBox();
         this.grpAuditResults = new System.Windows.Forms.GroupBox();
         this._rbAccessFailed = new System.Windows.Forms.RadioButton();
         this._rbAccessPassed = new System.Windows.Forms.RadioButton();
         this.chkFilterOnAccess = new System.Windows.Forms.CheckBox();
         this.grpAuditActivity = new System.Windows.Forms.GroupBox();
         this.chkAuditAdmin = new System.Windows.Forms.CheckBox();
         this.chkAuditDML = new System.Windows.Forms.CheckBox();
         this.chkAuditSELECT = new System.Windows.Forms.CheckBox();
         this.checkBox5 = new System.Windows.Forms.CheckBox();
         this.chkAuditSecurity = new System.Windows.Forms.CheckBox();
         this.chkAuditDDL = new System.Windows.Forms.CheckBox();
         this.panelDatabases = new System.Windows.Forms.Panel();
         this.lblDatabasesDescription = new System.Windows.Forms.Label();
         this.lblDatabasesTitle = new System.Windows.Forms.Label();
         this.panelDatabasesProperties = new System.Windows.Forms.Panel();
         this.btnAddNewUserDatabase = new System.Windows.Forms.Button();
         this.textNewUserDatabase = new System.Windows.Forms.TextBox();
         this.labelNewUseratabase = new System.Windows.Forms.Label();
         this.btnUnselectAll = new System.Windows.Forms.Button();
         this.btnSelectAll = new System.Windows.Forms.Button();
         this.lblDatabases = new System.Windows.Forms.Label();
         this.listDatabases = new System.Windows.Forms.CheckedListBox();
         this.panelDmlFilters = new System.Windows.Forms.Panel();
         this.panelObjectsProperties = new System.Windows.Forms.Panel();
         this.chkAuditSystemTables = new System.Windows.Forms.CheckBox();
         this.chkAuditUserTables = new System.Windows.Forms.CheckBox();
         this.chkAuditOther = new System.Windows.Forms.CheckBox();
         this.chkAuditStoredProcedures = new System.Windows.Forms.CheckBox();
         this.radioSelectedDML = new System.Windows.Forms.RadioButton();
         this.radioAllDML = new System.Windows.Forms.RadioButton();
         this.lblObjectDescription = new System.Windows.Forms.Label();
         this.lblObjectTitle = new System.Windows.Forms.Label();
         this.panelSystemDatabases = new System.Windows.Forms.Panel();
         this.lblSystemDatabasesDescription = new System.Windows.Forms.Label();
         this.lblSystemDatabasesTitle = new System.Windows.Forms.Label();
         this.panel2 = new System.Windows.Forms.Panel();
         this.btnAddNewSystemDatabase = new System.Windows.Forms.Button();
         this.textNewSystemDatabase = new System.Windows.Forms.TextBox();
         this.labelNewSytemDatabase = new System.Windows.Forms.Label();
         this.btnSystemUnselectAll = new System.Windows.Forms.Button();
         this.btnSystemSelectAll = new System.Windows.Forms.Button();
         this.label3 = new System.Windows.Forms.Label();
         this.listSystemDatabases = new System.Windows.Forms.CheckedListBox();
         this.panelSummary = new System.Windows.Forms.Panel();
         this.panel3 = new System.Windows.Forms.Panel();
         this.label11 = new System.Windows.Forms.Label();
         this.label1 = new System.Windows.Forms.Label();
         this.label2 = new System.Windows.Forms.Label();
         this.panelAllDBsAdded = new System.Windows.Forms.Panel();
         this.panel5 = new System.Windows.Forms.Panel();
         this.pictureBox3 = new System.Windows.Forms.PictureBox();
         this.label6 = new System.Windows.Forms.Label();
         this.label7 = new System.Windows.Forms.Label();
         this.label8 = new System.Windows.Forms.Label();
         this.panelAuditMode = new System.Windows.Forms.Panel();
         this.lblModeDescription = new System.Windows.Forms.Label();
         this.lblModeTitle = new System.Windows.Forms.Label();
         this.panel4 = new System.Windows.Forms.Panel();
         this.linkTypical = new System.Windows.Forms.LinkLabel();
         this.radioCustom = new System.Windows.Forms.RadioButton();
         this.radioTypical = new System.Windows.Forms.RadioButton();
         this.lblLevelIntro = new System.Windows.Forms.Label();
         this.panelCantLoadDatabases = new System.Windows.Forms.Panel();
         this.panel6 = new System.Windows.Forms.Panel();
         this.pictureBox2 = new System.Windows.Forms.PictureBox();
         this.label4 = new System.Windows.Forms.Label();
         this.label5 = new System.Windows.Forms.Label();
         this.label9 = new System.Windows.Forms.Label();
         this._pnlTrustedUsers = new System.Windows.Forms.Panel();
         this.label10 = new System.Windows.Forms.Label();
         this.label12 = new System.Windows.Forms.Label();
         this.panel7 = new System.Windows.Forms.Panel();
         this.label14 = new System.Windows.Forms.Label();
         this.btnRemoveTrusted = new System.Windows.Forms.Button();
         this.btnAddTrusted = new System.Windows.Forms.Button();
         this.lstTrustedUsers = new System.Windows.Forms.ListView();
         this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
         this.label19 = new System.Windows.Forms.Label();
         this.chkCaptureTrans = new System.Windows.Forms.CheckBox();
         this.panelLeft.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
         this.panelGeneral.SuspendLayout();
         this.panelGeneralProperties.SuspendLayout();
         this.panelAudit.SuspendLayout();
         this.panelAuditProperties.SuspendLayout();
         this.grpAuditResults.SuspendLayout();
         this.grpAuditActivity.SuspendLayout();
         this.panelDatabases.SuspendLayout();
         this.panelDatabasesProperties.SuspendLayout();
         this.panelDmlFilters.SuspendLayout();
         this.panelObjectsProperties.SuspendLayout();
         this.panelSystemDatabases.SuspendLayout();
         this.panel2.SuspendLayout();
         this.panelSummary.SuspendLayout();
         this.panel3.SuspendLayout();
         this.panelAllDBsAdded.SuspendLayout();
         this.panel5.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
         this.panelAuditMode.SuspendLayout();
         this.panel4.SuspendLayout();
         this.panelCantLoadDatabases.SuspendLayout();
         this.panel6.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
         this._pnlTrustedUsers.SuspendLayout();
         this.panel7.SuspendLayout();
         this.SuspendLayout();
         // 
         // panelLeft
         // 
         this.panelLeft.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
         this.panelLeft.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
         this.panelLeft.Controls.Add(this.pictureBox1);
         this.panelLeft.Location = new System.Drawing.Point(0, 0);
         this.panelLeft.Name = "panelLeft";
         this.panelLeft.Size = new System.Drawing.Size(112, 336);
         this.panelLeft.TabIndex = 11;
         // 
         // pictureBox1
         // 
         this.pictureBox1.Image = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.Wizard_DB;
         this.pictureBox1.Location = new System.Drawing.Point(0, 0);
         this.pictureBox1.Name = "pictureBox1";
         this.pictureBox1.Size = new System.Drawing.Size(112, 336);
         this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
         this.pictureBox1.TabIndex = 0;
         this.pictureBox1.TabStop = false;
         // 
         // panelGeneral
         // 
         this.panelGeneral.BackColor = System.Drawing.SystemColors.ControlLightLight;
         this.panelGeneral.Controls.Add(this.lblGeneralDescription);
         this.panelGeneral.Controls.Add(this.lblGeneralTitle);
         this.panelGeneral.Controls.Add(this.panelGeneralProperties);
         this.panelGeneral.Location = new System.Drawing.Point(111, 0);
         this.panelGeneral.Name = "panelGeneral";
         this.panelGeneral.Size = new System.Drawing.Size(448, 336);
         this.panelGeneral.TabIndex = 12;
         // 
         // lblGeneralDescription
         // 
         this.lblGeneralDescription.Location = new System.Drawing.Point(14, 24);
         this.lblGeneralDescription.Name = "lblGeneralDescription";
         this.lblGeneralDescription.Size = new System.Drawing.Size(420, 28);
         this.lblGeneralDescription.TabIndex = 13;
         this.lblGeneralDescription.Text = "Select the SQL Server that hosts the databases you want to audit. This instance m" +
             "ust be registered with SQL Compliance Manager.";
         // 
         // lblGeneralTitle
         // 
         this.lblGeneralTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.lblGeneralTitle.Location = new System.Drawing.Point(14, 8);
         this.lblGeneralTitle.Name = "lblGeneralTitle";
         this.lblGeneralTitle.Size = new System.Drawing.Size(384, 16);
         this.lblGeneralTitle.TabIndex = 12;
         this.lblGeneralTitle.Text = "General";
         // 
         // panelGeneralProperties
         // 
         this.panelGeneralProperties.BackColor = System.Drawing.SystemColors.Control;
         this.panelGeneralProperties.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
         this.panelGeneralProperties.Controls.Add(this.comboServer);
         this.panelGeneralProperties.Controls.Add(this.lblServer);
         this.panelGeneralProperties.Location = new System.Drawing.Point(0, 60);
         this.panelGeneralProperties.Name = "panelGeneralProperties";
         this.panelGeneralProperties.Size = new System.Drawing.Size(448, 276);
         this.panelGeneralProperties.TabIndex = 11;
         // 
         // comboServer
         // 
         this.comboServer.ItemHeight = 13;
         this.comboServer.Location = new System.Drawing.Point(84, 8);
         this.comboServer.Name = "comboServer";
         this.comboServer.Size = new System.Drawing.Size(352, 21);
         this.comboServer.TabIndex = 7;
         this.comboServer.TextChanged += new System.EventHandler(this.comboServerAndDatabase_TextChanged);
         // 
         // lblServer
         // 
         this.lblServer.Location = new System.Drawing.Point(12, 12);
         this.lblServer.Name = "lblServer";
         this.lblServer.Size = new System.Drawing.Size(68, 20);
         this.lblServer.TabIndex = 6;
         this.lblServer.Text = "&SQL Server:";
         // 
         // btnCancel
         // 
         this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
         this.btnCancel.Location = new System.Drawing.Point(490, 346);
         this.btnCancel.Name = "btnCancel";
         this.btnCancel.Size = new System.Drawing.Size(62, 20);
         this.btnCancel.TabIndex = 117;
         this.btnCancel.Text = "&Cancel";
         this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
         // 
         // btnFinish
         // 
         this.btnFinish.Enabled = false;
         this.btnFinish.FlatStyle = System.Windows.Forms.FlatStyle.System;
         this.btnFinish.Location = new System.Drawing.Point(420, 346);
         this.btnFinish.Name = "btnFinish";
         this.btnFinish.Size = new System.Drawing.Size(62, 20);
         this.btnFinish.TabIndex = 116;
         this.btnFinish.Text = "&Finish";
         this.btnFinish.Click += new System.EventHandler(this.btnFinish_Click);
         // 
         // btnNext
         // 
         this.btnNext.FlatStyle = System.Windows.Forms.FlatStyle.System;
         this.btnNext.Location = new System.Drawing.Point(350, 346);
         this.btnNext.Name = "btnNext";
         this.btnNext.Size = new System.Drawing.Size(62, 20);
         this.btnNext.TabIndex = 115;
         this.btnNext.Text = "&Next >";
         this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
         // 
         // btnBack
         // 
         this.btnBack.Enabled = false;
         this.btnBack.FlatStyle = System.Windows.Forms.FlatStyle.System;
         this.btnBack.Location = new System.Drawing.Point(280, 346);
         this.btnBack.Name = "btnBack";
         this.btnBack.Size = new System.Drawing.Size(62, 20);
         this.btnBack.TabIndex = 114;
         this.btnBack.Text = "< &Back";
         this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
         // 
         // panelAudit
         // 
         this.panelAudit.BackColor = System.Drawing.SystemColors.ControlLightLight;
         this.panelAudit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
         this.panelAudit.Controls.Add(this.linkLblHelpBestPractices);
         this.panelAudit.Controls.Add(this.lblAuditDescription);
         this.panelAudit.Controls.Add(this.lblAuditTitle);
         this.panelAudit.Controls.Add(this.panelAuditProperties);
         this.panelAudit.Location = new System.Drawing.Point(111, 0);
         this.panelAudit.Name = "panelAudit";
         this.panelAudit.Size = new System.Drawing.Size(448, 336);
         this.panelAudit.TabIndex = 17;
         // 
         // linkLblHelpBestPractices
         // 
         this.linkLblHelpBestPractices.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.linkLblHelpBestPractices.AutoSize = true;
         this.linkLblHelpBestPractices.LinkArea = new System.Windows.Forms.LinkArea(0, 99);
         this.linkLblHelpBestPractices.Location = new System.Drawing.Point(15, 37);
         this.linkLblHelpBestPractices.Name = "linkLblHelpBestPractices";
         this.linkLblHelpBestPractices.Size = new System.Drawing.Size(277, 17);
         this.linkLblHelpBestPractices.TabIndex = 39;
         this.linkLblHelpBestPractices.TabStop = true;
         this.linkLblHelpBestPractices.Text = "Learn how to optimize performance with audit settings.";
         this.linkLblHelpBestPractices.UseCompatibleTextRendering = true;
         this.linkLblHelpBestPractices.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLblHelpBestPractices_LinkClicked);
         // 
         // lblAuditDescription
         // 
         this.lblAuditDescription.Location = new System.Drawing.Point(14, 24);
         this.lblAuditDescription.Name = "lblAuditDescription";
         this.lblAuditDescription.Size = new System.Drawing.Size(386, 28);
         this.lblAuditDescription.TabIndex = 15;
         this.lblAuditDescription.Text = "Specify the type of audit data you want to collect on the selected databases.";
         // 
         // lblAuditTitle
         // 
         this.lblAuditTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.lblAuditTitle.Location = new System.Drawing.Point(14, 8);
         this.lblAuditTitle.Name = "lblAuditTitle";
         this.lblAuditTitle.Size = new System.Drawing.Size(384, 16);
         this.lblAuditTitle.TabIndex = 14;
         this.lblAuditTitle.Text = "Audit Settings";
         // 
         // panelAuditProperties
         // 
         this.panelAuditProperties.BackColor = System.Drawing.SystemColors.Control;
         this.panelAuditProperties.Controls.Add(this.chkCaptureTrans);
         this.panelAuditProperties.Controls.Add(this.chkCaptureSQL);
         this.panelAuditProperties.Controls.Add(this.grpAuditResults);
         this.panelAuditProperties.Controls.Add(this.grpAuditActivity);
         this.panelAuditProperties.Location = new System.Drawing.Point(0, 60);
         this.panelAuditProperties.Name = "panelAuditProperties";
         this.panelAuditProperties.Size = new System.Drawing.Size(448, 276);
         this.panelAuditProperties.TabIndex = 16;
         // 
         // chkCaptureSQL
         // 
         this.chkCaptureSQL.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
         this.chkCaptureSQL.Location = new System.Drawing.Point(20, 208);
         this.chkCaptureSQL.Name = "chkCaptureSQL";
         this.chkCaptureSQL.Size = new System.Drawing.Size(356, 20);
         this.chkCaptureSQL.TabIndex = 14;
         this.chkCaptureSQL.Text = "Capture S&QL statements for DML and SELECT activity.";
         this.chkCaptureSQL.TextAlign = System.Drawing.ContentAlignment.TopLeft;
         this.chkCaptureSQL.CheckedChanged += new System.EventHandler(this.chkCaptureSQL_CheckedChanged);
         // 
         // grpAuditResults
         // 
         this.grpAuditResults.Controls.Add(this._rbAccessFailed);
         this.grpAuditResults.Controls.Add(this._rbAccessPassed);
         this.grpAuditResults.Controls.Add(this.chkFilterOnAccess);
         this.grpAuditResults.Location = new System.Drawing.Point(12, 108);
         this.grpAuditResults.Name = "grpAuditResults";
         this.grpAuditResults.Size = new System.Drawing.Size(384, 92);
         this.grpAuditResults.TabIndex = 11;
         this.grpAuditResults.TabStop = false;
         this.grpAuditResults.Text = "Access Check Filter";
         // 
         // _rbAccessFailed
         // 
         this._rbAccessFailed.Location = new System.Drawing.Point(32, 64);
         this._rbAccessFailed.Name = "_rbAccessFailed";
         this._rbAccessFailed.Size = new System.Drawing.Size(240, 16);
         this._rbAccessFailed.TabIndex = 2;
         this._rbAccessFailed.Text = "Au&dit only actions that failed access check";
         // 
         // _rbAccessPassed
         // 
         this._rbAccessPassed.Location = new System.Drawing.Point(32, 40);
         this._rbAccessPassed.Name = "_rbAccessPassed";
         this._rbAccessPassed.Size = new System.Drawing.Size(276, 16);
         this._rbAccessPassed.TabIndex = 1;
         this._rbAccessPassed.Text = "A&udit only actions that passed access check";
         // 
         // chkFilterOnAccess
         // 
         this.chkFilterOnAccess.Checked = true;
         this.chkFilterOnAccess.CheckState = System.Windows.Forms.CheckState.Checked;
         this.chkFilterOnAccess.Location = new System.Drawing.Point(12, 16);
         this.chkFilterOnAccess.Name = "chkFilterOnAccess";
         this.chkFilterOnAccess.Size = new System.Drawing.Size(276, 16);
         this.chkFilterOnAccess.TabIndex = 0;
         this.chkFilterOnAccess.Text = "&Filter events based on access check";
         this.chkFilterOnAccess.Click += new System.EventHandler(this.Click_chkFilterOnAccess);
         // 
         // grpAuditActivity
         // 
         this.grpAuditActivity.Controls.Add(this.chkAuditAdmin);
         this.grpAuditActivity.Controls.Add(this.chkAuditDML);
         this.grpAuditActivity.Controls.Add(this.chkAuditSELECT);
         this.grpAuditActivity.Controls.Add(this.checkBox5);
         this.grpAuditActivity.Controls.Add(this.chkAuditSecurity);
         this.grpAuditActivity.Controls.Add(this.chkAuditDDL);
         this.grpAuditActivity.Location = new System.Drawing.Point(12, 12);
         this.grpAuditActivity.Name = "grpAuditActivity";
         this.grpAuditActivity.Size = new System.Drawing.Size(384, 92);
         this.grpAuditActivity.TabIndex = 16;
         this.grpAuditActivity.TabStop = false;
         this.grpAuditActivity.Text = "Audited Activity";
         // 
         // chkAuditAdmin
         // 
         this.chkAuditAdmin.Checked = true;
         this.chkAuditAdmin.CheckState = System.Windows.Forms.CheckState.Checked;
         this.chkAuditAdmin.FlatStyle = System.Windows.Forms.FlatStyle.System;
         this.chkAuditAdmin.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
         this.chkAuditAdmin.Location = new System.Drawing.Point(12, 68);
         this.chkAuditAdmin.Name = "chkAuditAdmin";
         this.chkAuditAdmin.Size = new System.Drawing.Size(156, 16);
         this.chkAuditAdmin.TabIndex = 6;
         this.chkAuditAdmin.Text = "&Administrative Actions";
         this.chkAuditAdmin.TextAlign = System.Drawing.ContentAlignment.TopLeft;
         // 
         // chkAuditDML
         // 
         this.chkAuditDML.FlatStyle = System.Windows.Forms.FlatStyle.System;
         this.chkAuditDML.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
         this.chkAuditDML.Location = new System.Drawing.Point(172, 20);
         this.chkAuditDML.Name = "chkAuditDML";
         this.chkAuditDML.Size = new System.Drawing.Size(164, 16);
         this.chkAuditDML.TabIndex = 8;
         this.chkAuditDML.Text = "Database &Modification (DML)";
         this.chkAuditDML.TextAlign = System.Drawing.ContentAlignment.TopLeft;
         this.chkAuditDML.CheckedChanged += new System.EventHandler(this.chkAuditDML_CheckedChanged);
         // 
         // chkAuditSELECT
         // 
         this.chkAuditSELECT.Location = new System.Drawing.Point(172, 44);
         this.chkAuditSELECT.Name = "chkAuditSELECT";
         this.chkAuditSELECT.Size = new System.Drawing.Size(136, 16);
         this.chkAuditSELECT.TabIndex = 9;
         this.chkAuditSELECT.Text = "Database SE&LECTs";
         this.chkAuditSELECT.CheckedChanged += new System.EventHandler(this.chkAuditDML_CheckedChanged);
         // 
         // checkBox5
         // 
         this.checkBox5.Location = new System.Drawing.Point(592, 432);
         this.checkBox5.Name = "checkBox5";
         this.checkBox5.Size = new System.Drawing.Size(344, 24);
         this.checkBox5.TabIndex = 4;
         this.checkBox5.Text = "Database Modifications (DML) - Insert, Update, Delete, Execute";
         // 
         // chkAuditSecurity
         // 
         this.chkAuditSecurity.Checked = true;
         this.chkAuditSecurity.CheckState = System.Windows.Forms.CheckState.Checked;
         this.chkAuditSecurity.Location = new System.Drawing.Point(12, 20);
         this.chkAuditSecurity.Name = "chkAuditSecurity";
         this.chkAuditSecurity.Size = new System.Drawing.Size(120, 16);
         this.chkAuditSecurity.TabIndex = 34;
         this.chkAuditSecurity.Text = "&Security Changes";
         // 
         // chkAuditDDL
         // 
         this.chkAuditDDL.Checked = true;
         this.chkAuditDDL.CheckState = System.Windows.Forms.CheckState.Checked;
         this.chkAuditDDL.FlatStyle = System.Windows.Forms.FlatStyle.System;
         this.chkAuditDDL.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
         this.chkAuditDDL.Location = new System.Drawing.Point(12, 44);
         this.chkAuditDDL.Name = "chkAuditDDL";
         this.chkAuditDDL.Size = new System.Drawing.Size(156, 16);
         this.chkAuditDDL.TabIndex = 5;
         this.chkAuditDDL.Text = "Database De&finition (DDL)";
         this.chkAuditDDL.TextAlign = System.Drawing.ContentAlignment.TopLeft;
         // 
         // panelDatabases
         // 
         this.panelDatabases.BackColor = System.Drawing.SystemColors.ControlLightLight;
         this.panelDatabases.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
         this.panelDatabases.Controls.Add(this.lblDatabasesDescription);
         this.panelDatabases.Controls.Add(this.lblDatabasesTitle);
         this.panelDatabases.Controls.Add(this.panelDatabasesProperties);
         this.panelDatabases.Location = new System.Drawing.Point(111, 0);
         this.panelDatabases.Name = "panelDatabases";
         this.panelDatabases.Size = new System.Drawing.Size(448, 336);
         this.panelDatabases.TabIndex = 18;
         // 
         // lblDatabasesDescription
         // 
         this.lblDatabasesDescription.BackColor = System.Drawing.SystemColors.ControlLightLight;
         this.lblDatabasesDescription.Location = new System.Drawing.Point(14, 24);
         this.lblDatabasesDescription.Name = "lblDatabasesDescription";
         this.lblDatabasesDescription.Size = new System.Drawing.Size(422, 28);
         this.lblDatabasesDescription.TabIndex = 36;
         this.lblDatabasesDescription.Text = "Select the user databases you want to audit. SQL Compliance Manager will collect " +
             "audit data for the selected databases.";
         // 
         // lblDatabasesTitle
         // 
         this.lblDatabasesTitle.BackColor = System.Drawing.SystemColors.ControlLightLight;
         this.lblDatabasesTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.lblDatabasesTitle.Location = new System.Drawing.Point(14, 8);
         this.lblDatabasesTitle.Name = "lblDatabasesTitle";
         this.lblDatabasesTitle.Size = new System.Drawing.Size(384, 16);
         this.lblDatabasesTitle.TabIndex = 35;
         this.lblDatabasesTitle.Text = "User Databases";
         // 
         // panelDatabasesProperties
         // 
         this.panelDatabasesProperties.BackColor = System.Drawing.SystemColors.Control;
         this.panelDatabasesProperties.Controls.Add(this.btnAddNewUserDatabase);
         this.panelDatabasesProperties.Controls.Add(this.textNewUserDatabase);
         this.panelDatabasesProperties.Controls.Add(this.labelNewUseratabase);
         this.panelDatabasesProperties.Controls.Add(this.btnUnselectAll);
         this.panelDatabasesProperties.Controls.Add(this.btnSelectAll);
         this.panelDatabasesProperties.Controls.Add(this.lblDatabases);
         this.panelDatabasesProperties.Controls.Add(this.listDatabases);
         this.panelDatabasesProperties.Location = new System.Drawing.Point(0, 60);
         this.panelDatabasesProperties.Name = "panelDatabasesProperties";
         this.panelDatabasesProperties.Size = new System.Drawing.Size(448, 276);
         this.panelDatabasesProperties.TabIndex = 37;
         // 
         // btnAddNewUserDatabase
         // 
         this.btnAddNewUserDatabase.Enabled = false;
         this.btnAddNewUserDatabase.Location = new System.Drawing.Point(361, 240);
         this.btnAddNewUserDatabase.Name = "btnAddNewUserDatabase";
         this.btnAddNewUserDatabase.Size = new System.Drawing.Size(75, 20);
         this.btnAddNewUserDatabase.TabIndex = 9;
         this.btnAddNewUserDatabase.Text = "Add";
         this.btnAddNewUserDatabase.Visible = false;
         this.btnAddNewUserDatabase.Click += new System.EventHandler(this.btnAddNewUserDatabase_Click);
         // 
         // textNewUserDatabase
         // 
         this.textNewUserDatabase.Location = new System.Drawing.Point(104, 240);
         this.textNewUserDatabase.Name = "textNewUserDatabase";
         this.textNewUserDatabase.Size = new System.Drawing.Size(248, 20);
         this.textNewUserDatabase.TabIndex = 8;
         this.textNewUserDatabase.Visible = false;
         this.textNewUserDatabase.TextChanged += new System.EventHandler(this.textNewUserDatabase_TextChanged);
         // 
         // labelNewUseratabase
         // 
         this.labelNewUseratabase.Location = new System.Drawing.Point(12, 244);
         this.labelNewUseratabase.Name = "labelNewUseratabase";
         this.labelNewUseratabase.Size = new System.Drawing.Size(88, 16);
         this.labelNewUseratabase.TabIndex = 7;
         this.labelNewUseratabase.Text = "Enter Database:";
         this.labelNewUseratabase.Visible = false;
         // 
         // btnUnselectAll
         // 
         this.btnUnselectAll.Location = new System.Drawing.Point(364, 56);
         this.btnUnselectAll.Name = "btnUnselectAll";
         this.btnUnselectAll.Size = new System.Drawing.Size(75, 23);
         this.btnUnselectAll.TabIndex = 3;
         this.btnUnselectAll.Text = "&Unselect All";
         this.btnUnselectAll.Click += new System.EventHandler(this.btnUnselectAll_Click);
         // 
         // btnSelectAll
         // 
         this.btnSelectAll.Location = new System.Drawing.Point(364, 28);
         this.btnSelectAll.Name = "btnSelectAll";
         this.btnSelectAll.Size = new System.Drawing.Size(75, 23);
         this.btnSelectAll.TabIndex = 2;
         this.btnSelectAll.Text = "&Select All";
         this.btnSelectAll.Click += new System.EventHandler(this.btnSelectAll_Click);
         // 
         // lblDatabases
         // 
         this.lblDatabases.Location = new System.Drawing.Point(12, 12);
         this.lblDatabases.Name = "lblDatabases";
         this.lblDatabases.Size = new System.Drawing.Size(148, 12);
         this.lblDatabases.TabIndex = 1;
         this.lblDatabases.Text = "User Databases:";
         // 
         // listDatabases
         // 
         this.listDatabases.CheckOnClick = true;
         this.listDatabases.Location = new System.Drawing.Point(12, 28);
         this.listDatabases.Name = "listDatabases";
         this.listDatabases.Size = new System.Drawing.Size(344, 199);
         this.listDatabases.Sorted = true;
         this.listDatabases.TabIndex = 0;
         // 
         // panelDmlFilters
         // 
         this.panelDmlFilters.BackColor = System.Drawing.SystemColors.ControlLightLight;
         this.panelDmlFilters.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
         this.panelDmlFilters.Controls.Add(this.panelObjectsProperties);
         this.panelDmlFilters.Controls.Add(this.lblObjectDescription);
         this.panelDmlFilters.Controls.Add(this.lblObjectTitle);
         this.panelDmlFilters.Location = new System.Drawing.Point(111, 0);
         this.panelDmlFilters.Name = "panelDmlFilters";
         this.panelDmlFilters.Size = new System.Drawing.Size(448, 336);
         this.panelDmlFilters.TabIndex = 19;
         // 
         // panelObjectsProperties
         // 
         this.panelObjectsProperties.BackColor = System.Drawing.SystemColors.Control;
         this.panelObjectsProperties.Controls.Add(this.chkAuditSystemTables);
         this.panelObjectsProperties.Controls.Add(this.chkAuditUserTables);
         this.panelObjectsProperties.Controls.Add(this.chkAuditOther);
         this.panelObjectsProperties.Controls.Add(this.chkAuditStoredProcedures);
         this.panelObjectsProperties.Controls.Add(this.radioSelectedDML);
         this.panelObjectsProperties.Controls.Add(this.radioAllDML);
         this.panelObjectsProperties.Location = new System.Drawing.Point(0, 60);
         this.panelObjectsProperties.Name = "panelObjectsProperties";
         this.panelObjectsProperties.Size = new System.Drawing.Size(448, 276);
         this.panelObjectsProperties.TabIndex = 37;
         // 
         // chkAuditSystemTables
         // 
         this.chkAuditSystemTables.Location = new System.Drawing.Point(28, 84);
         this.chkAuditSystemTables.Name = "chkAuditSystemTables";
         this.chkAuditSystemTables.Size = new System.Drawing.Size(164, 24);
         this.chkAuditSystemTables.TabIndex = 39;
         this.chkAuditSystemTables.Text = "Audit &system tables";
         // 
         // chkAuditUserTables
         // 
         this.chkAuditUserTables.Checked = true;
         this.chkAuditUserTables.CheckState = System.Windows.Forms.CheckState.Checked;
         this.chkAuditUserTables.Location = new System.Drawing.Point(28, 60);
         this.chkAuditUserTables.Name = "chkAuditUserTables";
         this.chkAuditUserTables.Size = new System.Drawing.Size(164, 24);
         this.chkAuditUserTables.TabIndex = 38;
         this.chkAuditUserTables.Text = "Audit &user tables";
         // 
         // chkAuditOther
         // 
         this.chkAuditOther.Location = new System.Drawing.Point(28, 132);
         this.chkAuditOther.Name = "chkAuditOther";
         this.chkAuditOther.Size = new System.Drawing.Size(288, 24);
         this.chkAuditOther.TabIndex = 41;
         this.chkAuditOther.Text = "Audit &all other object types (views, indexes, etc.)";
         // 
         // chkAuditStoredProcedures
         // 
         this.chkAuditStoredProcedures.Location = new System.Drawing.Point(28, 108);
         this.chkAuditStoredProcedures.Name = "chkAuditStoredProcedures";
         this.chkAuditStoredProcedures.Size = new System.Drawing.Size(164, 24);
         this.chkAuditStoredProcedures.TabIndex = 40;
         this.chkAuditStoredProcedures.Text = "Audit stored &procedures";
         // 
         // radioSelectedDML
         // 
         this.radioSelectedDML.Location = new System.Drawing.Point(12, 36);
         this.radioSelectedDML.Name = "radioSelectedDML";
         this.radioSelectedDML.Size = new System.Drawing.Size(340, 24);
         this.radioSelectedDML.TabIndex = 37;
         this.radioSelectedDML.Text = "Audit the following database objects:";
         this.radioSelectedDML.CheckedChanged += new System.EventHandler(this.radioAllDML_CheckedChanged);
         // 
         // radioAllDML
         // 
         this.radioAllDML.Checked = true;
         this.radioAllDML.Location = new System.Drawing.Point(12, 12);
         this.radioAllDML.Name = "radioAllDML";
         this.radioAllDML.Size = new System.Drawing.Size(328, 24);
         this.radioAllDML.TabIndex = 36;
         this.radioAllDML.TabStop = true;
         this.radioAllDML.Text = "Audit all database objects";
         this.radioAllDML.CheckedChanged += new System.EventHandler(this.radioAllDML_CheckedChanged);
         // 
         // lblObjectDescription
         // 
         this.lblObjectDescription.Location = new System.Drawing.Point(14, 24);
         this.lblObjectDescription.Name = "lblObjectDescription";
         this.lblObjectDescription.Size = new System.Drawing.Size(420, 28);
         this.lblObjectDescription.TabIndex = 36;
         this.lblObjectDescription.Text = "Select the database objects you want to audit for DML and SELECT activities.";
         // 
         // lblObjectTitle
         // 
         this.lblObjectTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.lblObjectTitle.Location = new System.Drawing.Point(14, 8);
         this.lblObjectTitle.Name = "lblObjectTitle";
         this.lblObjectTitle.Size = new System.Drawing.Size(384, 16);
         this.lblObjectTitle.TabIndex = 35;
         this.lblObjectTitle.Text = "DML and SELECT Audit Filters";
         // 
         // panelSystemDatabases
         // 
         this.panelSystemDatabases.BackColor = System.Drawing.SystemColors.ControlLightLight;
         this.panelSystemDatabases.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
         this.panelSystemDatabases.Controls.Add(this.lblSystemDatabasesDescription);
         this.panelSystemDatabases.Controls.Add(this.lblSystemDatabasesTitle);
         this.panelSystemDatabases.Controls.Add(this.panel2);
         this.panelSystemDatabases.Location = new System.Drawing.Point(111, 0);
         this.panelSystemDatabases.Name = "panelSystemDatabases";
         this.panelSystemDatabases.Size = new System.Drawing.Size(448, 336);
         this.panelSystemDatabases.TabIndex = 20;
         // 
         // lblSystemDatabasesDescription
         // 
         this.lblSystemDatabasesDescription.BackColor = System.Drawing.SystemColors.ControlLightLight;
         this.lblSystemDatabasesDescription.Location = new System.Drawing.Point(14, 24);
         this.lblSystemDatabasesDescription.Name = "lblSystemDatabasesDescription";
         this.lblSystemDatabasesDescription.Size = new System.Drawing.Size(422, 28);
         this.lblSystemDatabasesDescription.TabIndex = 36;
         this.lblSystemDatabasesDescription.Text = "Select the system databases you want to audit. SQL Compliance Manager will collec" +
             "t audit data for the selected databases.";
         // 
         // lblSystemDatabasesTitle
         // 
         this.lblSystemDatabasesTitle.BackColor = System.Drawing.SystemColors.ControlLightLight;
         this.lblSystemDatabasesTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.lblSystemDatabasesTitle.Location = new System.Drawing.Point(14, 8);
         this.lblSystemDatabasesTitle.Name = "lblSystemDatabasesTitle";
         this.lblSystemDatabasesTitle.Size = new System.Drawing.Size(384, 16);
         this.lblSystemDatabasesTitle.TabIndex = 35;
         this.lblSystemDatabasesTitle.Text = "System Databases";
         // 
         // panel2
         // 
         this.panel2.BackColor = System.Drawing.SystemColors.Control;
         this.panel2.Controls.Add(this.btnAddNewSystemDatabase);
         this.panel2.Controls.Add(this.textNewSystemDatabase);
         this.panel2.Controls.Add(this.labelNewSytemDatabase);
         this.panel2.Controls.Add(this.btnSystemUnselectAll);
         this.panel2.Controls.Add(this.btnSystemSelectAll);
         this.panel2.Controls.Add(this.label3);
         this.panel2.Controls.Add(this.listSystemDatabases);
         this.panel2.Location = new System.Drawing.Point(0, 60);
         this.panel2.Name = "panel2";
         this.panel2.Size = new System.Drawing.Size(448, 276);
         this.panel2.TabIndex = 37;
         // 
         // btnAddNewSystemDatabase
         // 
         this.btnAddNewSystemDatabase.Enabled = false;
         this.btnAddNewSystemDatabase.Location = new System.Drawing.Point(364, 240);
         this.btnAddNewSystemDatabase.Name = "btnAddNewSystemDatabase";
         this.btnAddNewSystemDatabase.Size = new System.Drawing.Size(75, 20);
         this.btnAddNewSystemDatabase.TabIndex = 6;
         this.btnAddNewSystemDatabase.Text = "Add";
         this.btnAddNewSystemDatabase.Visible = false;
         this.btnAddNewSystemDatabase.Click += new System.EventHandler(this.btnAddNewSystemDatabase_Click);
         // 
         // textNewSystemDatabase
         // 
         this.textNewSystemDatabase.Location = new System.Drawing.Point(104, 240);
         this.textNewSystemDatabase.Name = "textNewSystemDatabase";
         this.textNewSystemDatabase.Size = new System.Drawing.Size(252, 20);
         this.textNewSystemDatabase.TabIndex = 5;
         this.textNewSystemDatabase.Visible = false;
         this.textNewSystemDatabase.TextChanged += new System.EventHandler(this.textNewSystemDatabase_TextChanged);
         // 
         // labelNewSytemDatabase
         // 
         this.labelNewSytemDatabase.Location = new System.Drawing.Point(12, 244);
         this.labelNewSytemDatabase.Name = "labelNewSytemDatabase";
         this.labelNewSytemDatabase.Size = new System.Drawing.Size(88, 16);
         this.labelNewSytemDatabase.TabIndex = 4;
         this.labelNewSytemDatabase.Text = "Enter Database:";
         this.labelNewSytemDatabase.Visible = false;
         // 
         // btnSystemUnselectAll
         // 
         this.btnSystemUnselectAll.Location = new System.Drawing.Point(364, 56);
         this.btnSystemUnselectAll.Name = "btnSystemUnselectAll";
         this.btnSystemUnselectAll.Size = new System.Drawing.Size(75, 23);
         this.btnSystemUnselectAll.TabIndex = 3;
         this.btnSystemUnselectAll.Text = "&Unselect All";
         this.btnSystemUnselectAll.Click += new System.EventHandler(this.btnSystemUnselectAll_Click);
         // 
         // btnSystemSelectAll
         // 
         this.btnSystemSelectAll.Location = new System.Drawing.Point(364, 28);
         this.btnSystemSelectAll.Name = "btnSystemSelectAll";
         this.btnSystemSelectAll.Size = new System.Drawing.Size(75, 23);
         this.btnSystemSelectAll.TabIndex = 2;
         this.btnSystemSelectAll.Text = "&Select All";
         this.btnSystemSelectAll.Click += new System.EventHandler(this.btnSystemSelectAll_Click);
         // 
         // label3
         // 
         this.label3.Location = new System.Drawing.Point(12, 12);
         this.label3.Name = "label3";
         this.label3.Size = new System.Drawing.Size(148, 12);
         this.label3.TabIndex = 1;
         this.label3.Text = "System Databases:";
         // 
         // listSystemDatabases
         // 
         this.listSystemDatabases.CheckOnClick = true;
         this.listSystemDatabases.Location = new System.Drawing.Point(12, 28);
         this.listSystemDatabases.Name = "listSystemDatabases";
         this.listSystemDatabases.Size = new System.Drawing.Size(344, 199);
         this.listSystemDatabases.Sorted = true;
         this.listSystemDatabases.TabIndex = 0;
         // 
         // panelSummary
         // 
         this.panelSummary.BackColor = System.Drawing.SystemColors.ControlLightLight;
         this.panelSummary.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
         this.panelSummary.Controls.Add(this.panel3);
         this.panelSummary.Controls.Add(this.label1);
         this.panelSummary.Controls.Add(this.label2);
         this.panelSummary.Location = new System.Drawing.Point(111, 0);
         this.panelSummary.Name = "panelSummary";
         this.panelSummary.Size = new System.Drawing.Size(448, 336);
         this.panelSummary.TabIndex = 21;
         // 
         // panel3
         // 
         this.panel3.BackColor = System.Drawing.SystemColors.Control;
         this.panel3.Controls.Add(this.label11);
         this.panel3.Location = new System.Drawing.Point(0, 60);
         this.panel3.Name = "panel3";
         this.panel3.Size = new System.Drawing.Size(448, 276);
         this.panel3.TabIndex = 37;
         // 
         // label11
         // 
         this.label11.Location = new System.Drawing.Point(12, 12);
         this.label11.Name = "label11";
         this.label11.Size = new System.Drawing.Size(420, 28);
         this.label11.TabIndex = 2;
         this.label11.Text = "You have finished entering the data necessary to add additional audited databases" +
             ". Click Finish to add the selected databases.";
         // 
         // label1
         // 
         this.label1.Location = new System.Drawing.Point(14, 24);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(420, 28);
         this.label1.TabIndex = 36;
         this.label1.Text = "Ready to add audited databases";
         // 
         // label2
         // 
         this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.label2.Location = new System.Drawing.Point(14, 8);
         this.label2.Name = "label2";
         this.label2.Size = new System.Drawing.Size(384, 16);
         this.label2.TabIndex = 35;
         this.label2.Text = "Summary";
         // 
         // panelAllDBsAdded
         // 
         this.panelAllDBsAdded.BackColor = System.Drawing.SystemColors.ControlLightLight;
         this.panelAllDBsAdded.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
         this.panelAllDBsAdded.Controls.Add(this.panel5);
         this.panelAllDBsAdded.Controls.Add(this.label7);
         this.panelAllDBsAdded.Controls.Add(this.label8);
         this.panelAllDBsAdded.Location = new System.Drawing.Point(111, 0);
         this.panelAllDBsAdded.Name = "panelAllDBsAdded";
         this.panelAllDBsAdded.Size = new System.Drawing.Size(448, 336);
         this.panelAllDBsAdded.TabIndex = 118;
         // 
         // panel5
         // 
         this.panel5.BackColor = System.Drawing.SystemColors.Control;
         this.panel5.Controls.Add(this.pictureBox3);
         this.panel5.Controls.Add(this.label6);
         this.panel5.Location = new System.Drawing.Point(0, 60);
         this.panel5.Name = "panel5";
         this.panel5.Size = new System.Drawing.Size(448, 276);
         this.panel5.TabIndex = 37;
         // 
         // pictureBox3
         // 
         this.pictureBox3.Image = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.GoSign_48;
         this.pictureBox3.Location = new System.Drawing.Point(52, 40);
         this.pictureBox3.Name = "pictureBox3";
         this.pictureBox3.Size = new System.Drawing.Size(48, 48);
         this.pictureBox3.TabIndex = 3;
         this.pictureBox3.TabStop = false;
         // 
         // label6
         // 
         this.label6.Location = new System.Drawing.Point(116, 52);
         this.label6.Name = "label6";
         this.label6.Size = new System.Drawing.Size(284, 40);
         this.label6.TabIndex = 2;
         this.label6.Text = "You are already auditing all available user and system databases from the selecte" +
             "d SQL Server.";
         // 
         // label7
         // 
         this.label7.Location = new System.Drawing.Point(14, 24);
         this.label7.Name = "label7";
         this.label7.Size = new System.Drawing.Size(420, 28);
         this.label7.TabIndex = 36;
         this.label7.Text = "All user and system databases have already been selected for auditing.";
         // 
         // label8
         // 
         this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.label8.Location = new System.Drawing.Point(14, 8);
         this.label8.Name = "label8";
         this.label8.Size = new System.Drawing.Size(384, 16);
         this.label8.TabIndex = 35;
         this.label8.Text = "No databases available";
         // 
         // panelAuditMode
         // 
         this.panelAuditMode.BackColor = System.Drawing.SystemColors.ControlLightLight;
         this.panelAuditMode.Controls.Add(this.lblModeDescription);
         this.panelAuditMode.Controls.Add(this.lblModeTitle);
         this.panelAuditMode.Controls.Add(this.panel4);
         this.panelAuditMode.Location = new System.Drawing.Point(111, 0);
         this.panelAuditMode.Name = "panelAuditMode";
         this.panelAuditMode.Size = new System.Drawing.Size(448, 336);
         this.panelAuditMode.TabIndex = 119;
         // 
         // lblModeDescription
         // 
         this.lblModeDescription.Location = new System.Drawing.Point(14, 24);
         this.lblModeDescription.Name = "lblModeDescription";
         this.lblModeDescription.Size = new System.Drawing.Size(420, 28);
         this.lblModeDescription.TabIndex = 13;
         this.lblModeDescription.Text = "Specify the audit collection level for the databases you want to audit.";
         // 
         // lblModeTitle
         // 
         this.lblModeTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.lblModeTitle.Location = new System.Drawing.Point(14, 8);
         this.lblModeTitle.Name = "lblModeTitle";
         this.lblModeTitle.Size = new System.Drawing.Size(384, 16);
         this.lblModeTitle.TabIndex = 12;
         this.lblModeTitle.Text = "Audit Collection Level";
         // 
         // panel4
         // 
         this.panel4.BackColor = System.Drawing.SystemColors.Control;
         this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
         this.panel4.Controls.Add(this.linkTypical);
         this.panel4.Controls.Add(this.radioCustom);
         this.panel4.Controls.Add(this.radioTypical);
         this.panel4.Controls.Add(this.lblLevelIntro);
         this.panel4.Location = new System.Drawing.Point(0, 60);
         this.panel4.Name = "panel4";
         this.panel4.Size = new System.Drawing.Size(448, 276);
         this.panel4.TabIndex = 11;
         // 
         // linkTypical
         // 
         this.linkTypical.Location = new System.Drawing.Point(280, 63);
         this.linkTypical.Name = "linkTypical";
         this.linkTypical.Size = new System.Drawing.Size(100, 12);
         this.linkTypical.TabIndex = 3;
         this.linkTypical.TabStop = true;
         this.linkTypical.Text = "Tell me more";
         this.linkTypical.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkTypical_LinkClicked);
         // 
         // radioCustom
         // 
         this.radioCustom.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
         this.radioCustom.Location = new System.Drawing.Point(24, 84);
         this.radioCustom.Name = "radioCustom";
         this.radioCustom.Size = new System.Drawing.Size(400, 68);
         this.radioCustom.TabIndex = 2;
         this.radioCustom.Text = resources.GetString("radioCustom.Text");
         this.radioCustom.TextAlign = System.Drawing.ContentAlignment.TopLeft;
         // 
         // radioTypical
         // 
         this.radioTypical.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
         this.radioTypical.Checked = true;
         this.radioTypical.Location = new System.Drawing.Point(24, 48);
         this.radioTypical.Name = "radioTypical";
         this.radioTypical.Size = new System.Drawing.Size(396, 32);
         this.radioTypical.TabIndex = 1;
         this.radioTypical.TabStop = true;
         this.radioTypical.Text = "Typical - Audits events and activities most commonly required by auditors. This c" +
             "ollection level meets most auditing needs.";
         // 
         // lblLevelIntro
         // 
         this.lblLevelIntro.Location = new System.Drawing.Point(12, 12);
         this.lblLevelIntro.Name = "lblLevelIntro";
         this.lblLevelIntro.Size = new System.Drawing.Size(428, 32);
         this.lblLevelIntro.TabIndex = 0;
         this.lblLevelIntro.Text = "Select the audit collection level you want to use for the newly audited databases" +
             ". The collection level affects the amount of event data collected for database a" +
             "ctivities.";
         // 
         // panelCantLoadDatabases
         // 
         this.panelCantLoadDatabases.BackColor = System.Drawing.SystemColors.ControlLightLight;
         this.panelCantLoadDatabases.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
         this.panelCantLoadDatabases.Controls.Add(this.panel6);
         this.panelCantLoadDatabases.Controls.Add(this.label5);
         this.panelCantLoadDatabases.Controls.Add(this.label9);
         this.panelCantLoadDatabases.Location = new System.Drawing.Point(111, 0);
         this.panelCantLoadDatabases.Name = "panelCantLoadDatabases";
         this.panelCantLoadDatabases.Size = new System.Drawing.Size(448, 336);
         this.panelCantLoadDatabases.TabIndex = 120;
         // 
         // panel6
         // 
         this.panel6.BackColor = System.Drawing.SystemColors.Control;
         this.panel6.Controls.Add(this.pictureBox2);
         this.panel6.Controls.Add(this.label4);
         this.panel6.Location = new System.Drawing.Point(0, 60);
         this.panel6.Name = "panel6";
         this.panel6.Size = new System.Drawing.Size(448, 276);
         this.panel6.TabIndex = 37;
         // 
         // pictureBox2
         // 
         this.pictureBox2.Image = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.StopSign_48;
         this.pictureBox2.Location = new System.Drawing.Point(52, 40);
         this.pictureBox2.Name = "pictureBox2";
         this.pictureBox2.Size = new System.Drawing.Size(48, 48);
         this.pictureBox2.TabIndex = 3;
         this.pictureBox2.TabStop = false;
         // 
         // label4
         // 
         this.label4.Location = new System.Drawing.Point(116, 40);
         this.label4.Name = "label4";
         this.label4.Size = new System.Drawing.Size(296, 80);
         this.label4.TabIndex = 2;
         this.label4.Text = resources.GetString("label4.Text");
         // 
         // label5
         // 
         this.label5.Location = new System.Drawing.Point(14, 24);
         this.label5.Name = "label5";
         this.label5.Size = new System.Drawing.Size(420, 28);
         this.label5.TabIndex = 36;
         this.label5.Text = "Cannot reach SQL Server instance to load database lists";
         // 
         // label9
         // 
         this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.label9.Location = new System.Drawing.Point(14, 8);
         this.label9.Name = "label9";
         this.label9.Size = new System.Drawing.Size(384, 16);
         this.label9.TabIndex = 35;
         this.label9.Text = "Cannot load databases";
         // 
         // _pnlTrustedUsers
         // 
         this._pnlTrustedUsers.BackColor = System.Drawing.SystemColors.ControlLightLight;
         this._pnlTrustedUsers.Controls.Add(this.label10);
         this._pnlTrustedUsers.Controls.Add(this.label12);
         this._pnlTrustedUsers.Controls.Add(this.panel7);
         this._pnlTrustedUsers.Location = new System.Drawing.Point(111, 0);
         this._pnlTrustedUsers.Name = "_pnlTrustedUsers";
         this._pnlTrustedUsers.Size = new System.Drawing.Size(448, 336);
         this._pnlTrustedUsers.TabIndex = 121;
         // 
         // label10
         // 
         this.label10.Location = new System.Drawing.Point(14, 24);
         this.label10.Name = "label10";
         this.label10.Size = new System.Drawing.Size(420, 28);
         this.label10.TabIndex = 13;
         this.label10.Text = "Select users whose activities you never want collected, regardless of other audit" +
             " settings.";
         // 
         // label12
         // 
         this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.label12.Location = new System.Drawing.Point(14, 8);
         this.label12.Name = "label12";
         this.label12.Size = new System.Drawing.Size(384, 16);
         this.label12.TabIndex = 12;
         this.label12.Text = "Trusted Users";
         // 
         // panel7
         // 
         this.panel7.BackColor = System.Drawing.SystemColors.Control;
         this.panel7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
         this.panel7.Controls.Add(this.label14);
         this.panel7.Controls.Add(this.btnRemoveTrusted);
         this.panel7.Controls.Add(this.btnAddTrusted);
         this.panel7.Controls.Add(this.lstTrustedUsers);
         this.panel7.Controls.Add(this.label19);
         this.panel7.Location = new System.Drawing.Point(0, 60);
         this.panel7.Name = "panel7";
         this.panel7.Size = new System.Drawing.Size(448, 276);
         this.panel7.TabIndex = 11;
         // 
         // label14
         // 
         this.label14.Location = new System.Drawing.Point(9, 207);
         this.label14.Name = "label14";
         this.label14.Size = new System.Drawing.Size(420, 52);
         this.label14.TabIndex = 34;
         this.label14.Text = resources.GetString("label14.Text");
         // 
         // btnRemoveTrusted
         // 
         this.btnRemoveTrusted.Enabled = false;
         this.btnRemoveTrusted.Location = new System.Drawing.Point(377, 55);
         this.btnRemoveTrusted.Name = "btnRemoveTrusted";
         this.btnRemoveTrusted.Size = new System.Drawing.Size(60, 23);
         this.btnRemoveTrusted.TabIndex = 33;
         this.btnRemoveTrusted.Text = "&Remove";
         this.btnRemoveTrusted.Click += new System.EventHandler(this.btnRemoveTrusted_Click);
         // 
         // btnAddTrusted
         // 
         this.btnAddTrusted.Location = new System.Drawing.Point(377, 27);
         this.btnAddTrusted.Name = "btnAddTrusted";
         this.btnAddTrusted.Size = new System.Drawing.Size(60, 23);
         this.btnAddTrusted.TabIndex = 32;
         this.btnAddTrusted.Text = "&Add...";
         this.btnAddTrusted.Click += new System.EventHandler(this.btnAddTrusted_Click);
         // 
         // lstTrustedUsers
         // 
         this.lstTrustedUsers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
         this.lstTrustedUsers.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
         this.lstTrustedUsers.HideSelection = false;
         this.lstTrustedUsers.Location = new System.Drawing.Point(9, 27);
         this.lstTrustedUsers.Name = "lstTrustedUsers";
         this.lstTrustedUsers.Size = new System.Drawing.Size(360, 168);
         this.lstTrustedUsers.TabIndex = 31;
         this.lstTrustedUsers.UseCompatibleStateImageBehavior = false;
         this.lstTrustedUsers.View = System.Windows.Forms.View.Details;
         // 
         // columnHeader1
         // 
         this.columnHeader1.Width = 335;
         // 
         // label19
         // 
         this.label19.Location = new System.Drawing.Point(9, 11);
         this.label19.Name = "label19";
         this.label19.Size = new System.Drawing.Size(176, 16);
         this.label19.TabIndex = 30;
         this.label19.Text = "&Trusted Users:";
         // 
         // chkCaptureTrans
         // 
         this.chkCaptureTrans.AutoSize = true;
         this.chkCaptureTrans.Location = new System.Drawing.Point(20, 233);
         this.chkCaptureTrans.Name = "chkCaptureTrans";
         this.chkCaptureTrans.Size = new System.Drawing.Size(255, 17);
         this.chkCaptureTrans.TabIndex = 17;
         this.chkCaptureTrans.Text = "Capture Transaction Status for DML Activity";
         this.chkCaptureTrans.UseVisualStyleBackColor = true;
         // 
         // Form_DatabaseNew
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.CancelButton = this.btnCancel;
         this.ClientSize = new System.Drawing.Size(558, 376);
         this.Controls.Add(this.btnCancel);
         this.Controls.Add(this.btnFinish);
         this.Controls.Add(this.btnNext);
         this.Controls.Add(this.btnBack);
         this.Controls.Add(this.panelLeft);
         this.Controls.Add(this.panelAudit);
         this.Controls.Add(this._pnlTrustedUsers);
         this.Controls.Add(this.panelAuditMode);
         this.Controls.Add(this.panelGeneral);
         this.Controls.Add(this.panelSummary);
         this.Controls.Add(this.panelSystemDatabases);
         this.Controls.Add(this.panelDatabases);
         this.Controls.Add(this.panelCantLoadDatabases);
         this.Controls.Add(this.panelAllDBsAdded);
         this.Controls.Add(this.panelDmlFilters);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.HelpButton = true;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "Form_DatabaseNew";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "New Audited Database";
         this.Load += new System.EventHandler(this.Form_DatabaseNew_Load);
         this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.Form_DatabaseNew_HelpRequested);
         this.panelLeft.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
         this.panelGeneral.ResumeLayout(false);
         this.panelGeneralProperties.ResumeLayout(false);
         this.panelAudit.ResumeLayout(false);
         this.panelAudit.PerformLayout();
         this.panelAuditProperties.ResumeLayout(false);
         this.panelAuditProperties.PerformLayout();
         this.grpAuditResults.ResumeLayout(false);
         this.grpAuditActivity.ResumeLayout(false);
         this.panelDatabases.ResumeLayout(false);
         this.panelDatabasesProperties.ResumeLayout(false);
         this.panelDatabasesProperties.PerformLayout();
         this.panelDmlFilters.ResumeLayout(false);
         this.panelObjectsProperties.ResumeLayout(false);
         this.panelSystemDatabases.ResumeLayout(false);
         this.panel2.ResumeLayout(false);
         this.panel2.PerformLayout();
         this.panelSummary.ResumeLayout(false);
         this.panel3.ResumeLayout(false);
         this.panelAllDBsAdded.ResumeLayout(false);
         this.panel5.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
         this.panelAuditMode.ResumeLayout(false);
         this.panel4.ResumeLayout(false);
         this.panelCantLoadDatabases.ResumeLayout(false);
         this.panel6.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
         this._pnlTrustedUsers.ResumeLayout(false);
         this.panel7.ResumeLayout(false);
         this.ResumeLayout(false);

      }
      #endregion

      private System.Windows.Forms.Panel panelLeft;
      private System.Windows.Forms.Button btnCancel;
      private System.Windows.Forms.Button btnFinish;
      private System.Windows.Forms.Button btnNext;
      private System.Windows.Forms.Button btnBack;
      private System.Windows.Forms.Panel panelAudit;
      private System.Windows.Forms.PictureBox pictureBox1;
      private System.Windows.Forms.Panel panelGeneral;
      private System.Windows.Forms.ComboBox comboServer;
      private System.Windows.Forms.Label lblServer;
      private System.Windows.Forms.Panel panelGeneralProperties;
      private System.Windows.Forms.Label lblGeneralDescription;
      private System.Windows.Forms.Label lblGeneralTitle;
      private System.Windows.Forms.Label lblObjectDescription;
      private System.Windows.Forms.Label lblObjectTitle;
      private System.Windows.Forms.Label lblAuditDescription;
      private System.Windows.Forms.Label lblAuditTitle;
      private System.Windows.Forms.Panel panelObjectsProperties;
      private System.Windows.Forms.Panel panelAuditProperties;
      private System.Windows.Forms.GroupBox grpAuditResults;
      private System.Windows.Forms.GroupBox grpAuditActivity;
      private System.Windows.Forms.CheckBox chkAuditDML;
      private System.Windows.Forms.CheckBox chkAuditSELECT;
      private System.Windows.Forms.CheckBox checkBox5;
      private System.Windows.Forms.CheckBox chkAuditSecurity;
      private System.Windows.Forms.CheckBox chkAuditDDL;
      private System.Windows.Forms.Panel panelDatabases;
      private System.Windows.Forms.Label lblDatabasesDescription;
      private System.Windows.Forms.Label lblDatabasesTitle;
      private System.Windows.Forms.Panel panelDatabasesProperties;
      private System.Windows.Forms.CheckedListBox listDatabases;
      private System.Windows.Forms.Label lblDatabases;
      private System.Windows.Forms.Button btnSelectAll;
      private System.Windows.Forms.Button btnUnselectAll;
      private System.Windows.Forms.Panel panelSystemDatabases;
      private System.Windows.Forms.Label lblSystemDatabasesDescription;
      private System.Windows.Forms.Label lblSystemDatabasesTitle;
      private System.Windows.Forms.Panel panel2;
      private System.Windows.Forms.Button btnSystemUnselectAll;
      private System.Windows.Forms.Button btnSystemSelectAll;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.CheckedListBox listSystemDatabases;
      private System.Windows.Forms.CheckBox chkAuditAdmin;
      private System.Windows.Forms.Panel panel3;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.Panel panelSummary;
      private System.Windows.Forms.Label label11;
      private System.Windows.Forms.RadioButton radioAllDML;
      private System.Windows.Forms.RadioButton radioSelectedDML;
      private System.Windows.Forms.CheckBox chkAuditSystemTables;
      private System.Windows.Forms.CheckBox chkAuditUserTables;
      private System.Windows.Forms.CheckBox chkAuditStoredProcedures;
      private System.Windows.Forms.CheckBox chkAuditOther;
      private System.Windows.Forms.Panel panelDmlFilters;
      private System.Windows.Forms.CheckBox chkCaptureSQL;
      private System.Windows.Forms.Panel panelAllDBsAdded;
      private System.Windows.Forms.Panel panel5;
      private System.Windows.Forms.PictureBox pictureBox3;
      private System.Windows.Forms.Label label6;
      private System.Windows.Forms.Label label7;
      private System.Windows.Forms.Label label8;
      private System.Windows.Forms.Panel panelAuditMode;
      private System.Windows.Forms.Label lblModeDescription;
      private System.Windows.Forms.Label lblModeTitle;
      private System.Windows.Forms.Panel panel4;
      private System.Windows.Forms.Label lblLevelIntro;
      private System.Windows.Forms.RadioButton radioTypical;
      private System.Windows.Forms.RadioButton radioCustom;
      private System.Windows.Forms.LinkLabel linkTypical;
      private System.Windows.Forms.TextBox textNewSystemDatabase;
      private System.Windows.Forms.Button btnAddNewSystemDatabase;
      private System.Windows.Forms.Button btnAddNewUserDatabase;
      private System.Windows.Forms.TextBox textNewUserDatabase;
      private System.Windows.Forms.Label labelNewUseratabase;
      private System.Windows.Forms.Label labelNewSytemDatabase;
      private System.Windows.Forms.Panel panel6;
      private System.Windows.Forms.PictureBox pictureBox2;
      private System.Windows.Forms.Label label4;
      private System.Windows.Forms.Label label5;
      private System.Windows.Forms.Label label9;
      private System.Windows.Forms.Panel panelCantLoadDatabases;
      private System.Windows.Forms.CheckBox chkFilterOnAccess;
      private System.Windows.Forms.RadioButton _rbAccessPassed;
      private System.Windows.Forms.RadioButton _rbAccessFailed;
      private System.Windows.Forms.Panel _pnlTrustedUsers;
      private System.Windows.Forms.Label label10;
      private System.Windows.Forms.Label label12;
      private System.Windows.Forms.Panel panel7;
      private System.Windows.Forms.Label label14;
      private System.Windows.Forms.Button btnRemoveTrusted;
      private System.Windows.Forms.Button btnAddTrusted;
      private System.Windows.Forms.ListView lstTrustedUsers;
      private System.Windows.Forms.ColumnHeader columnHeader1;
      private System.Windows.Forms.Label label19;
      private System.Windows.Forms.LinkLabel linkLblHelpBestPractices;
      private System.Windows.Forms.CheckBox chkCaptureTrans;
	}
}