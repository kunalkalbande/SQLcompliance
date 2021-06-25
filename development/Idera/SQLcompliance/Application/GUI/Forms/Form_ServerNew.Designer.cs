namespace Idera.SQLcompliance.Application.GUI.Forms
{
   partial class Form_ServerNew
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_ServerNew));
         this.panelButtons = new System.Windows.Forms.Panel();
         this.btnCancel = new System.Windows.Forms.Button();
         this.btnFinish = new System.Windows.Forms.Button();
         this.btnNext = new System.Windows.Forms.Button();
         this.btnBack = new System.Windows.Forms.Button();
         this.panelLeft = new System.Windows.Forms.Panel();
         this.pictureBox1 = new System.Windows.Forms.PictureBox();
         this.panelServer = new System.Windows.Forms.Panel();
         this.panelGeneralProperties = new System.Windows.Forms.Panel();
         this.textDescription = new System.Windows.Forms.TextBox();
         this.lblDescription = new System.Windows.Forms.Label();
         this.btnBrowse = new System.Windows.Forms.Button();
         this.textSQLServer = new System.Windows.Forms.TextBox();
         this.labelSQLServer = new System.Windows.Forms.Label();
         this.lblGeneralDescription = new System.Windows.Forms.Label();
         this.lblGeneralTitle = new System.Windows.Forms.Label();
         this.panelAgentDeploy = new System.Windows.Forms.Panel();
         this.panelAgentDeployProperties = new System.Windows.Forms.Panel();
         this.label25 = new System.Windows.Forms.Label();
         this.radioButtonAlreadyDeployed = new System.Windows.Forms.RadioButton();
         this.radioButtonDeployLater = new System.Windows.Forms.RadioButton();
         this.radioButtonDeployNow = new System.Windows.Forms.RadioButton();
         this.radioButtonDeployManually = new System.Windows.Forms.RadioButton();
         this.labelAgentDeployDescription = new System.Windows.Forms.Label();
         this.labelAgentDeployTitle = new System.Windows.Forms.Label();
         this.panelAgentService = new System.Windows.Forms.Panel();
         this.panelAgentProperties = new System.Windows.Forms.Panel();
         this.label15 = new System.Windows.Forms.Label();
         this.groupBox2 = new System.Windows.Forms.GroupBox();
         this.label1 = new System.Windows.Forms.Label();
         this.textServicePasswordConfirm = new System.Windows.Forms.TextBox();
         this.textServicePassword = new System.Windows.Forms.TextBox();
         this.label2 = new System.Windows.Forms.Label();
         this.textServiceAccount = new System.Windows.Forms.TextBox();
         this.label3 = new System.Windows.Forms.Label();
         this.labelAgentDescription = new System.Windows.Forms.Label();
         this.labelAgentTitle = new System.Windows.Forms.Label();
         this.panelAudit = new System.Windows.Forms.Panel();
         this.linkLblHelpBestPractices = new System.Windows.Forms.LinkLabel();
         this.lblAuditDescription = new System.Windows.Forms.Label();
         this.lblAuditTitle = new System.Windows.Forms.Label();
         this.panelAuditProperties = new System.Windows.Forms.Panel();
         this.grpAuditResults = new System.Windows.Forms.GroupBox();
         this._cbAccessCheckFilter = new System.Windows.Forms.CheckBox();
         this.rbAuditFailedOnly = new System.Windows.Forms.RadioButton();
         this.rbAuditSuccessfulOnly = new System.Windows.Forms.RadioButton();
         this.grpAuditActivity = new System.Windows.Forms.GroupBox();
         this.chkAuditUDE = new System.Windows.Forms.CheckBox();
         this.chkAuditAdmin = new System.Windows.Forms.CheckBox();
         this.label26 = new System.Windows.Forms.Label();
         this.label31 = new System.Windows.Forms.Label();
         this.chkAuditFailedLogins = new System.Windows.Forms.CheckBox();
         this.chkAuditLogins = new System.Windows.Forms.CheckBox();
         this.chkAuditSecurity = new System.Windows.Forms.CheckBox();
         this.chkAuditDDL = new System.Windows.Forms.CheckBox();
         this.panelAuditedUsers = new System.Windows.Forms.Panel();
         this.label4 = new System.Windows.Forms.Label();
         this.label5 = new System.Windows.Forms.Label();
         this.panel2 = new System.Windows.Forms.Panel();
         this.label14 = new System.Windows.Forms.Label();
         this.btnRemovePriv = new System.Windows.Forms.Button();
         this.btnAddPriv = new System.Windows.Forms.Button();
         this.lstPrivilegedUsers = new System.Windows.Forms.ListView();
         this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
         this.label19 = new System.Windows.Forms.Label();
         this.panelAuditedUserActivity = new System.Windows.Forms.Panel();
         this.linklblHelpBestPractices2 = new System.Windows.Forms.LinkLabel();
         this.label6 = new System.Windows.Forms.Label();
         this.label7 = new System.Windows.Forms.Label();
         this.panel3 = new System.Windows.Forms.Panel();
         this.grpAuditUserActivity = new System.Windows.Forms.GroupBox();
         this.chkAuditUserUDE = new System.Windows.Forms.CheckBox();
         this._rbUserAuditFailed = new System.Windows.Forms.RadioButton();
         this._rbUserAuditPassed = new System.Windows.Forms.RadioButton();
         this.chkAuditUserAdmin = new System.Windows.Forms.CheckBox();
         this.chkAuditUserCaptureSQL = new System.Windows.Forms.CheckBox();
         this.chkUserAccessCheckFilter = new System.Windows.Forms.CheckBox();
         this.chkAuditUserDML = new System.Windows.Forms.CheckBox();
         this.chkAuditUserSELECT = new System.Windows.Forms.CheckBox();
         this.chkAuditUserSecurity = new System.Windows.Forms.CheckBox();
         this.chkAuditUserDDL = new System.Windows.Forms.CheckBox();
         this.chkAuditUserFailedLogins = new System.Windows.Forms.CheckBox();
         this.chkAuditUserLogins = new System.Windows.Forms.CheckBox();
         this.rbAuditUserSelected = new System.Windows.Forms.RadioButton();
         this.rbAuditUserAll = new System.Windows.Forms.RadioButton();
         this.panelSummary = new System.Windows.Forms.Panel();
         this.label8 = new System.Windows.Forms.Label();
         this.label9 = new System.Windows.Forms.Label();
         this.panel4 = new System.Windows.Forms.Panel();
         this._flblVirtualServerInfo = new Infragistics.Win.FormattedLinkLabel.UltraFormattedLinkLabel();
         this.lblInstance = new System.Windows.Forms.Label();
         this.label11 = new System.Windows.Forms.Label();
         this.label10 = new System.Windows.Forms.Label();
         this.panelAgentTrace = new System.Windows.Forms.Panel();
         this.panel7 = new System.Windows.Forms.Panel();
         this.label13 = new System.Windows.Forms.Label();
         this.label12 = new System.Windows.Forms.Label();
         this.txtTraceDirectory = new System.Windows.Forms.TextBox();
         this.radioSpecifyTrace = new System.Windows.Forms.RadioButton();
         this.radioDefaultTrace = new System.Windows.Forms.RadioButton();
         this.labeAgentTraceDescription = new System.Windows.Forms.Label();
         this.labelAgentTraceTitle = new System.Windows.Forms.Label();
         this.panelLicenseLimit = new System.Windows.Forms.Panel();
         this.lblLicenseDescription = new System.Windows.Forms.Label();
         this.lblLicenseTitle = new System.Windows.Forms.Label();
         this.panelLicenseProperties = new System.Windows.Forms.Panel();
         this.pictureBox2 = new System.Windows.Forms.PictureBox();
         this.lblLicenseViolation = new System.Windows.Forms.Label();
         this.panelPermissions = new System.Windows.Forms.Panel();
         this.lblPermissionsDescription = new System.Windows.Forms.Label();
         this.labelPermissionsTitle = new System.Windows.Forms.Label();
         this.panelPermissionsProperties = new System.Windows.Forms.Panel();
         this.label16 = new System.Windows.Forms.Label();
         this.groupBox3 = new System.Windows.Forms.GroupBox();
         this.radioGrantAll = new System.Windows.Forms.RadioButton();
         this.radioGrantEventsOnly = new System.Windows.Forms.RadioButton();
         this.radioDeny = new System.Windows.Forms.RadioButton();
         this.panelExistingDatabase = new System.Windows.Forms.Panel();
         this.panel5 = new System.Windows.Forms.Panel();
         this.textExistingDatabase = new System.Windows.Forms.Label();
         this.label27 = new System.Windows.Forms.Label();
         this.label20 = new System.Windows.Forms.Label();
         this.radioDeleteDatabase = new System.Windows.Forms.RadioButton();
         this.radioMaintainDatabase = new System.Windows.Forms.RadioButton();
         this.label17 = new System.Windows.Forms.Label();
         this.label18 = new System.Windows.Forms.Label();
         this.panelIncompatibleDatabase = new System.Windows.Forms.Panel();
         this.panel6 = new System.Windows.Forms.Panel();
         this.textDatabaseName = new System.Windows.Forms.Label();
         this.label24 = new System.Windows.Forms.Label();
         this.label21 = new System.Windows.Forms.Label();
         this.radioIncompatibleOverwrite = new System.Windows.Forms.RadioButton();
         this.radioIncompatibleCancel = new System.Windows.Forms.RadioButton();
         this.label23 = new System.Windows.Forms.Label();
         this.label22 = new System.Windows.Forms.Label();
         this.panelCluster = new System.Windows.Forms.Panel();
         this.panel8 = new System.Windows.Forms.Panel();
         this.checkVirtualServer = new System.Windows.Forms.CheckBox();
         this.label29 = new System.Windows.Forms.Label();
         this.label30 = new System.Windows.Forms.Label();
         this.label32 = new System.Windows.Forms.Label();
         this.panelCantAudit2005 = new System.Windows.Forms.Panel();
         this.label28 = new System.Windows.Forms.Label();
         this.label33 = new System.Windows.Forms.Label();
         this.panel9 = new System.Windows.Forms.Panel();
         this.pictureBox3 = new System.Windows.Forms.PictureBox();
         this.label34 = new System.Windows.Forms.Label();
         this.panelCantConnect = new System.Windows.Forms.Panel();
         this.label35 = new System.Windows.Forms.Label();
         this.label36 = new System.Windows.Forms.Label();
         this.panel10 = new System.Windows.Forms.Panel();
         this.pictureBox4 = new System.Windows.Forms.PictureBox();
         this.label37 = new System.Windows.Forms.Label();
         this.chkAuditUserCaptureTrans = new System.Windows.Forms.CheckBox();
         this.panelButtons.SuspendLayout();
         this.panelLeft.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
         this.panelServer.SuspendLayout();
         this.panelGeneralProperties.SuspendLayout();
         this.panelAgentDeploy.SuspendLayout();
         this.panelAgentDeployProperties.SuspendLayout();
         this.panelAgentService.SuspendLayout();
         this.panelAgentProperties.SuspendLayout();
         this.groupBox2.SuspendLayout();
         this.panelAudit.SuspendLayout();
         this.panelAuditProperties.SuspendLayout();
         this.grpAuditResults.SuspendLayout();
         this.grpAuditActivity.SuspendLayout();
         this.panelAuditedUsers.SuspendLayout();
         this.panel2.SuspendLayout();
         this.panelAuditedUserActivity.SuspendLayout();
         this.panel3.SuspendLayout();
         this.grpAuditUserActivity.SuspendLayout();
         this.panelSummary.SuspendLayout();
         this.panel4.SuspendLayout();
         this.panelAgentTrace.SuspendLayout();
         this.panel7.SuspendLayout();
         this.panelLicenseLimit.SuspendLayout();
         this.panelLicenseProperties.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
         this.panelPermissions.SuspendLayout();
         this.panelPermissionsProperties.SuspendLayout();
         this.groupBox3.SuspendLayout();
         this.panelExistingDatabase.SuspendLayout();
         this.panel5.SuspendLayout();
         this.panelIncompatibleDatabase.SuspendLayout();
         this.panel6.SuspendLayout();
         this.panelCluster.SuspendLayout();
         this.panel8.SuspendLayout();
         this.panelCantAudit2005.SuspendLayout();
         this.panel9.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
         this.panelCantConnect.SuspendLayout();
         this.panel10.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
         this.SuspendLayout();
         // 
         // panelButtons
         // 
         this.panelButtons.Controls.Add(this.btnCancel);
         this.panelButtons.Controls.Add(this.btnFinish);
         this.panelButtons.Controls.Add(this.btnNext);
         this.panelButtons.Controls.Add(this.btnBack);
         this.panelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.panelButtons.Location = new System.Drawing.Point(0, 338);
         this.panelButtons.Name = "panelButtons";
         this.panelButtons.Size = new System.Drawing.Size(558, 38);
         this.panelButtons.TabIndex = 9;
         this.panelButtons.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.Form_ServerNew_HelpRequested);
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
         this.pictureBox1.Image = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.Wizard_Server;
         this.pictureBox1.Location = new System.Drawing.Point(0, 0);
         this.pictureBox1.Name = "pictureBox1";
         this.pictureBox1.Size = new System.Drawing.Size(112, 336);
         this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
         this.pictureBox1.TabIndex = 0;
         this.pictureBox1.TabStop = false;
         // 
         // panelServer
         // 
         this.panelServer.BackColor = System.Drawing.SystemColors.ControlLightLight;
         this.panelServer.Controls.Add(this.panelGeneralProperties);
         this.panelServer.Controls.Add(this.lblGeneralDescription);
         this.panelServer.Controls.Add(this.lblGeneralTitle);
         this.panelServer.Location = new System.Drawing.Point(111, 0);
         this.panelServer.Name = "panelServer";
         this.panelServer.Size = new System.Drawing.Size(448, 336);
         this.panelServer.TabIndex = 11;
         // 
         // panelGeneralProperties
         // 
         this.panelGeneralProperties.BackColor = System.Drawing.SystemColors.Control;
         this.panelGeneralProperties.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
         this.panelGeneralProperties.Controls.Add(this.textDescription);
         this.panelGeneralProperties.Controls.Add(this.lblDescription);
         this.panelGeneralProperties.Controls.Add(this.btnBrowse);
         this.panelGeneralProperties.Controls.Add(this.textSQLServer);
         this.panelGeneralProperties.Controls.Add(this.labelSQLServer);
         this.panelGeneralProperties.Location = new System.Drawing.Point(0, 60);
         this.panelGeneralProperties.Name = "panelGeneralProperties";
         this.panelGeneralProperties.Size = new System.Drawing.Size(448, 276);
         this.panelGeneralProperties.TabIndex = 16;
         // 
         // textDescription
         // 
         this.textDescription.Location = new System.Drawing.Point(88, 36);
         this.textDescription.MaxLength = 255;
         this.textDescription.Multiline = true;
         this.textDescription.Name = "textDescription";
         this.textDescription.Size = new System.Drawing.Size(316, 108);
         this.textDescription.TabIndex = 18;
         // 
         // lblDescription
         // 
         this.lblDescription.Location = new System.Drawing.Point(10, 38);
         this.lblDescription.Name = "lblDescription";
         this.lblDescription.Size = new System.Drawing.Size(64, 16);
         this.lblDescription.TabIndex = 17;
         this.lblDescription.Text = "&Description:";
         // 
         // btnBrowse
         // 
         this.btnBrowse.Location = new System.Drawing.Point(412, 8);
         this.btnBrowse.Name = "btnBrowse";
         this.btnBrowse.Size = new System.Drawing.Size(23, 20);
         this.btnBrowse.TabIndex = 16;
         this.btnBrowse.Text = ".&..";
         this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
         // 
         // textSQLServer
         // 
         this.textSQLServer.Location = new System.Drawing.Point(88, 8);
         this.textSQLServer.MaxLength = 255;
         this.textSQLServer.Name = "textSQLServer";
         this.textSQLServer.Size = new System.Drawing.Size(316, 20);
         this.textSQLServer.TabIndex = 15;
         this.textSQLServer.TextChanged += new System.EventHandler(this.textSQLServer_TextChanged);
         // 
         // labelSQLServer
         // 
         this.labelSQLServer.FlatStyle = System.Windows.Forms.FlatStyle.System;
         this.labelSQLServer.Location = new System.Drawing.Point(12, 12);
         this.labelSQLServer.Name = "labelSQLServer";
         this.labelSQLServer.Size = new System.Drawing.Size(70, 15);
         this.labelSQLServer.TabIndex = 14;
         this.labelSQLServer.Text = "&SQL Server:";
         this.labelSQLServer.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
         // 
         // lblGeneralDescription
         // 
         this.lblGeneralDescription.Location = new System.Drawing.Point(14, 24);
         this.lblGeneralDescription.Name = "lblGeneralDescription";
         this.lblGeneralDescription.Size = new System.Drawing.Size(420, 28);
         this.lblGeneralDescription.TabIndex = 15;
         this.lblGeneralDescription.Text = "Specify the SQL Server to register with SQL Compliance Manager. Once a SQL Server" +
             " is registered, you can begin auditing database activity on the server. ";
         // 
         // lblGeneralTitle
         // 
         this.lblGeneralTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.lblGeneralTitle.Location = new System.Drawing.Point(14, 8);
         this.lblGeneralTitle.Name = "lblGeneralTitle";
         this.lblGeneralTitle.Size = new System.Drawing.Size(384, 16);
         this.lblGeneralTitle.TabIndex = 14;
         this.lblGeneralTitle.Text = "General";
         // 
         // panelAgentDeploy
         // 
         this.panelAgentDeploy.BackColor = System.Drawing.SystemColors.ControlLightLight;
         this.panelAgentDeploy.Controls.Add(this.panelAgentDeployProperties);
         this.panelAgentDeploy.Controls.Add(this.labelAgentDeployDescription);
         this.panelAgentDeploy.Controls.Add(this.labelAgentDeployTitle);
         this.panelAgentDeploy.Location = new System.Drawing.Point(111, 0);
         this.panelAgentDeploy.Name = "panelAgentDeploy";
         this.panelAgentDeploy.Size = new System.Drawing.Size(448, 336);
         this.panelAgentDeploy.TabIndex = 11;
         // 
         // panelAgentDeployProperties
         // 
         this.panelAgentDeployProperties.BackColor = System.Drawing.SystemColors.Control;
         this.panelAgentDeployProperties.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
         this.panelAgentDeployProperties.Controls.Add(this.label25);
         this.panelAgentDeployProperties.Controls.Add(this.radioButtonAlreadyDeployed);
         this.panelAgentDeployProperties.Controls.Add(this.radioButtonDeployLater);
         this.panelAgentDeployProperties.Controls.Add(this.radioButtonDeployNow);
         this.panelAgentDeployProperties.Controls.Add(this.radioButtonDeployManually);
         this.panelAgentDeployProperties.Location = new System.Drawing.Point(0, 60);
         this.panelAgentDeployProperties.Name = "panelAgentDeployProperties";
         this.panelAgentDeployProperties.Size = new System.Drawing.Size(448, 276);
         this.panelAgentDeployProperties.TabIndex = 35;
         // 
         // label25
         // 
         this.label25.Location = new System.Drawing.Point(12, 12);
         this.label25.Name = "label25";
         this.label25.Size = new System.Drawing.Size(416, 40);
         this.label25.TabIndex = 37;
         this.label25.Text = resources.GetString("label25.Text");
         // 
         // radioButtonAlreadyDeployed
         // 
         this.radioButtonAlreadyDeployed.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
         this.radioButtonAlreadyDeployed.Location = new System.Drawing.Point(20, 204);
         this.radioButtonAlreadyDeployed.Name = "radioButtonAlreadyDeployed";
         this.radioButtonAlreadyDeployed.Size = new System.Drawing.Size(416, 32);
         this.radioButtonAlreadyDeployed.TabIndex = 36;
         this.radioButtonAlreadyDeployed.Text = "Already Deployed - The SQLcompliance Agent has already been deployed on the compu" +
             "ter hosting this SQL Server instance. ";
         this.radioButtonAlreadyDeployed.Visible = false;
         // 
         // radioButtonDeployLater
         // 
         this.radioButtonDeployLater.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
         this.radioButtonDeployLater.Location = new System.Drawing.Point(20, 108);
         this.radioButtonDeployLater.Name = "radioButtonDeployLater";
         this.radioButtonDeployLater.Size = new System.Drawing.Size(420, 32);
         this.radioButtonDeployLater.TabIndex = 34;
         this.radioButtonDeployLater.Text = "Deploy &Later - Indicates that you will install the SQLcompliance Agent using the" +
             " Management Console at a later time such as during off-hours. ";
         this.radioButtonDeployLater.TextAlign = System.Drawing.ContentAlignment.TopLeft;
         // 
         // radioButtonDeployNow
         // 
         this.radioButtonDeployNow.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
         this.radioButtonDeployNow.Location = new System.Drawing.Point(20, 60);
         this.radioButtonDeployNow.Name = "radioButtonDeployNow";
         this.radioButtonDeployNow.Size = new System.Drawing.Size(396, 48);
         this.radioButtonDeployNow.TabIndex = 33;
         this.radioButtonDeployNow.Text = "&Deploy Now - Installs the SQLcompliance agent at this time.  This option require" +
             "s that a connection be established between the SQL Server to be audited and the " +
             "Management Console.";
         this.radioButtonDeployNow.TextAlign = System.Drawing.ContentAlignment.TopLeft;
         // 
         // radioButtonDeployManually
         // 
         this.radioButtonDeployManually.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
         this.radioButtonDeployManually.Checked = true;
         this.radioButtonDeployManually.Location = new System.Drawing.Point(20, 144);
         this.radioButtonDeployManually.Name = "radioButtonDeployManually";
         this.radioButtonDeployManually.Size = new System.Drawing.Size(420, 56);
         this.radioButtonDeployManually.TabIndex = 35;
         this.radioButtonDeployManually.TabStop = true;
         this.radioButtonDeployManually.Text = resources.GetString("radioButtonDeployManually.Text");
         this.radioButtonDeployManually.TextAlign = System.Drawing.ContentAlignment.TopLeft;
         // 
         // labelAgentDeployDescription
         // 
         this.labelAgentDeployDescription.Location = new System.Drawing.Point(14, 24);
         this.labelAgentDeployDescription.Name = "labelAgentDeployDescription";
         this.labelAgentDeployDescription.Size = new System.Drawing.Size(420, 28);
         this.labelAgentDeployDescription.TabIndex = 34;
         this.labelAgentDeployDescription.Text = "Specify the deployment option for this instance\'s agent.";
         // 
         // labelAgentDeployTitle
         // 
         this.labelAgentDeployTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.labelAgentDeployTitle.Location = new System.Drawing.Point(14, 8);
         this.labelAgentDeployTitle.Name = "labelAgentDeployTitle";
         this.labelAgentDeployTitle.Size = new System.Drawing.Size(384, 16);
         this.labelAgentDeployTitle.TabIndex = 33;
         this.labelAgentDeployTitle.Text = "SQLcompliance Agent Deployment";
         // 
         // panelAgentService
         // 
         this.panelAgentService.BackColor = System.Drawing.SystemColors.ControlLightLight;
         this.panelAgentService.Controls.Add(this.panelAgentProperties);
         this.panelAgentService.Controls.Add(this.labelAgentDescription);
         this.panelAgentService.Controls.Add(this.labelAgentTitle);
         this.panelAgentService.Location = new System.Drawing.Point(111, 0);
         this.panelAgentService.Name = "panelAgentService";
         this.panelAgentService.Size = new System.Drawing.Size(448, 336);
         this.panelAgentService.TabIndex = 11;
         // 
         // panelAgentProperties
         // 
         this.panelAgentProperties.BackColor = System.Drawing.SystemColors.Control;
         this.panelAgentProperties.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
         this.panelAgentProperties.Controls.Add(this.label15);
         this.panelAgentProperties.Controls.Add(this.groupBox2);
         this.panelAgentProperties.Location = new System.Drawing.Point(0, 60);
         this.panelAgentProperties.Name = "panelAgentProperties";
         this.panelAgentProperties.Size = new System.Drawing.Size(448, 276);
         this.panelAgentProperties.TabIndex = 53;
         // 
         // label15
         // 
         this.label15.Location = new System.Drawing.Point(24, 124);
         this.label15.Name = "label15";
         this.label15.Size = new System.Drawing.Size(364, 56);
         this.label15.TabIndex = 53;
         this.label15.Text = resources.GetString("label15.Text");
         // 
         // groupBox2
         // 
         this.groupBox2.Controls.Add(this.label1);
         this.groupBox2.Controls.Add(this.textServicePasswordConfirm);
         this.groupBox2.Controls.Add(this.textServicePassword);
         this.groupBox2.Controls.Add(this.label2);
         this.groupBox2.Controls.Add(this.textServiceAccount);
         this.groupBox2.Controls.Add(this.label3);
         this.groupBox2.Location = new System.Drawing.Point(12, 12);
         this.groupBox2.Name = "groupBox2";
         this.groupBox2.Size = new System.Drawing.Size(387, 104);
         this.groupBox2.TabIndex = 51;
         this.groupBox2.TabStop = false;
         this.groupBox2.Text = "SQLcompliance Agent Service Account:";
         // 
         // label1
         // 
         this.label1.FlatStyle = System.Windows.Forms.FlatStyle.System;
         this.label1.Location = new System.Drawing.Point(10, 73);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(97, 15);
         this.label1.TabIndex = 47;
         this.label1.Text = "C&onfirm password:";
         this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
         // 
         // textServicePasswordConfirm
         // 
         this.textServicePasswordConfirm.Location = new System.Drawing.Point(156, 69);
         this.textServicePasswordConfirm.MaxLength = 64;
         this.textServicePasswordConfirm.Name = "textServicePasswordConfirm";
         this.textServicePasswordConfirm.PasswordChar = '*';
         this.textServicePasswordConfirm.Size = new System.Drawing.Size(224, 20);
         this.textServicePasswordConfirm.TabIndex = 48;
         // 
         // textServicePassword
         // 
         this.textServicePassword.Location = new System.Drawing.Point(156, 45);
         this.textServicePassword.MaxLength = 64;
         this.textServicePassword.Name = "textServicePassword";
         this.textServicePassword.PasswordChar = '*';
         this.textServicePassword.Size = new System.Drawing.Size(224, 20);
         this.textServicePassword.TabIndex = 46;
         // 
         // label2
         // 
         this.label2.FlatStyle = System.Windows.Forms.FlatStyle.System;
         this.label2.Location = new System.Drawing.Point(10, 49);
         this.label2.Name = "label2";
         this.label2.Size = new System.Drawing.Size(72, 15);
         this.label2.TabIndex = 45;
         this.label2.Text = "&Password :";
         this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
         // 
         // textServiceAccount
         // 
         this.textServiceAccount.Location = new System.Drawing.Point(156, 21);
         this.textServiceAccount.MaxLength = 255;
         this.textServiceAccount.Name = "textServiceAccount";
         this.textServiceAccount.Size = new System.Drawing.Size(223, 20);
         this.textServiceAccount.TabIndex = 44;
         this.textServiceAccount.TextChanged += new System.EventHandler(this.textServiceAccount_TextChanged);
         // 
         // label3
         // 
         this.label3.FlatStyle = System.Windows.Forms.FlatStyle.System;
         this.label3.Location = new System.Drawing.Point(10, 24);
         this.label3.Name = "label3";
         this.label3.Size = new System.Drawing.Size(142, 17);
         this.label3.TabIndex = 0;
         this.label3.Text = "&Login account (domain\\user):";
         this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
         // 
         // labelAgentDescription
         // 
         this.labelAgentDescription.Location = new System.Drawing.Point(14, 24);
         this.labelAgentDescription.Name = "labelAgentDescription";
         this.labelAgentDescription.Size = new System.Drawing.Size(420, 28);
         this.labelAgentDescription.TabIndex = 52;
         this.labelAgentDescription.Text = "Specify the service options. This account needs to be given SQL Server Administra" +
             "tor privileges on the registered SQL Server.";
         // 
         // labelAgentTitle
         // 
         this.labelAgentTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.labelAgentTitle.Location = new System.Drawing.Point(14, 8);
         this.labelAgentTitle.Name = "labelAgentTitle";
         this.labelAgentTitle.Size = new System.Drawing.Size(384, 16);
         this.labelAgentTitle.TabIndex = 51;
         this.labelAgentTitle.Text = "SQLcompliance Agent Service Account";
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
         this.panelAudit.TabIndex = 21;
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
         this.lblAuditDescription.Size = new System.Drawing.Size(420, 28);
         this.lblAuditDescription.TabIndex = 15;
         this.lblAuditDescription.Text = "Select the type of audit data you want to collect for this SQL Server instance.";
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
         this.panelAuditProperties.Controls.Add(this.grpAuditResults);
         this.panelAuditProperties.Controls.Add(this.grpAuditActivity);
         this.panelAuditProperties.Location = new System.Drawing.Point(0, 60);
         this.panelAuditProperties.Name = "panelAuditProperties";
         this.panelAuditProperties.Size = new System.Drawing.Size(448, 276);
         this.panelAuditProperties.TabIndex = 16;
         // 
         // grpAuditResults
         // 
         this.grpAuditResults.Controls.Add(this._cbAccessCheckFilter);
         this.grpAuditResults.Controls.Add(this.rbAuditFailedOnly);
         this.grpAuditResults.Controls.Add(this.rbAuditSuccessfulOnly);
         this.grpAuditResults.Location = new System.Drawing.Point(12, 172);
         this.grpAuditResults.Name = "grpAuditResults";
         this.grpAuditResults.Size = new System.Drawing.Size(424, 96);
         this.grpAuditResults.TabIndex = 1;
         this.grpAuditResults.TabStop = false;
         this.grpAuditResults.Text = "Access Check Filter";
         // 
         // _cbAccessCheckFilter
         // 
         this._cbAccessCheckFilter.Location = new System.Drawing.Point(12, 20);
         this._cbAccessCheckFilter.Name = "_cbAccessCheckFilter";
         this._cbAccessCheckFilter.Size = new System.Drawing.Size(256, 16);
         this._cbAccessCheckFilter.TabIndex = 0;
         this._cbAccessCheckFilter.Text = "&Filter events based on access check";
         this._cbAccessCheckFilter.Click += new System.EventHandler(this.Click_cbAccessCheckFilter);
         // 
         // rbAuditFailedOnly
         // 
         this.rbAuditFailedOnly.Location = new System.Drawing.Point(32, 68);
         this.rbAuditFailedOnly.Name = "rbAuditFailedOnly";
         this.rbAuditFailedOnly.Size = new System.Drawing.Size(244, 16);
         this.rbAuditFailedOnly.TabIndex = 2;
         this.rbAuditFailedOnly.Text = "Au&dit only activities that failed access check";
         // 
         // rbAuditSuccessfulOnly
         // 
         this.rbAuditSuccessfulOnly.Checked = true;
         this.rbAuditSuccessfulOnly.Location = new System.Drawing.Point(32, 44);
         this.rbAuditSuccessfulOnly.Name = "rbAuditSuccessfulOnly";
         this.rbAuditSuccessfulOnly.Size = new System.Drawing.Size(352, 16);
         this.rbAuditSuccessfulOnly.TabIndex = 1;
         this.rbAuditSuccessfulOnly.TabStop = true;
         this.rbAuditSuccessfulOnly.Text = "Audit only activities that &passed access check";
         // 
         // grpAuditActivity
         // 
         this.grpAuditActivity.Controls.Add(this.chkAuditUDE);
         this.grpAuditActivity.Controls.Add(this.chkAuditAdmin);
         this.grpAuditActivity.Controls.Add(this.label26);
         this.grpAuditActivity.Controls.Add(this.label31);
         this.grpAuditActivity.Controls.Add(this.chkAuditFailedLogins);
         this.grpAuditActivity.Controls.Add(this.chkAuditLogins);
         this.grpAuditActivity.Controls.Add(this.chkAuditSecurity);
         this.grpAuditActivity.Controls.Add(this.chkAuditDDL);
         this.grpAuditActivity.Location = new System.Drawing.Point(12, 12);
         this.grpAuditActivity.Name = "grpAuditActivity";
         this.grpAuditActivity.Size = new System.Drawing.Size(420, 156);
         this.grpAuditActivity.TabIndex = 0;
         this.grpAuditActivity.TabStop = false;
         this.grpAuditActivity.Text = "Audited Activity";
         // 
         // chkAuditUDE
         // 
         this.chkAuditUDE.Checked = true;
         this.chkAuditUDE.CheckState = System.Windows.Forms.CheckState.Checked;
         this.chkAuditUDE.FlatStyle = System.Windows.Forms.FlatStyle.System;
         this.chkAuditUDE.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
         this.chkAuditUDE.Location = new System.Drawing.Point(12, 131);
         this.chkAuditUDE.Name = "chkAuditUDE";
         this.chkAuditUDE.Size = new System.Drawing.Size(352, 16);
         this.chkAuditUDE.TabIndex = 5;
         this.chkAuditUDE.Text = "&User Defined Events (custom SQL Server event type)";
         this.chkAuditUDE.TextAlign = System.Drawing.ContentAlignment.TopLeft;
         // 
         // chkAuditAdmin
         // 
         this.chkAuditAdmin.Checked = true;
         this.chkAuditAdmin.CheckState = System.Windows.Forms.CheckState.Checked;
         this.chkAuditAdmin.FlatStyle = System.Windows.Forms.FlatStyle.System;
         this.chkAuditAdmin.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
         this.chkAuditAdmin.Location = new System.Drawing.Point(12, 108);
         this.chkAuditAdmin.Name = "chkAuditAdmin";
         this.chkAuditAdmin.Size = new System.Drawing.Size(272, 16);
         this.chkAuditAdmin.TabIndex = 4;
         this.chkAuditAdmin.Text = "&Administrative Actions (e.g. DBCC)";
         this.chkAuditAdmin.TextAlign = System.Drawing.ContentAlignment.TopLeft;
         // 
         // label26
         // 
         this.label26.Location = new System.Drawing.Point(120, 68);
         this.label26.Name = "label26";
         this.label26.Size = new System.Drawing.Size(252, 16);
         this.label26.TabIndex = 42;
         this.label26.Text = "(e.g.  GRANT, REVOKE, LOGIN CHANGE PWD)";
         // 
         // label31
         // 
         this.label31.Location = new System.Drawing.Point(160, 92);
         this.label31.Name = "label31";
         this.label31.Size = new System.Drawing.Size(200, 16);
         this.label31.TabIndex = 41;
         this.label31.Text = "(e.g.  CREATE or DROP DATABASE)";
         // 
         // chkAuditFailedLogins
         // 
         this.chkAuditFailedLogins.Checked = true;
         this.chkAuditFailedLogins.CheckState = System.Windows.Forms.CheckState.Checked;
         this.chkAuditFailedLogins.Location = new System.Drawing.Point(12, 39);
         this.chkAuditFailedLogins.Name = "chkAuditFailedLogins";
         this.chkAuditFailedLogins.Size = new System.Drawing.Size(120, 16);
         this.chkAuditFailedLogins.TabIndex = 1;
         this.chkAuditFailedLogins.Text = "Failed Lo&gins";
         // 
         // chkAuditLogins
         // 
         this.chkAuditLogins.Checked = true;
         this.chkAuditLogins.CheckState = System.Windows.Forms.CheckState.Checked;
         this.chkAuditLogins.Location = new System.Drawing.Point(12, 16);
         this.chkAuditLogins.Name = "chkAuditLogins";
         this.chkAuditLogins.Size = new System.Drawing.Size(120, 16);
         this.chkAuditLogins.TabIndex = 0;
         this.chkAuditLogins.Text = "&Logins";
         // 
         // chkAuditSecurity
         // 
         this.chkAuditSecurity.Checked = true;
         this.chkAuditSecurity.CheckState = System.Windows.Forms.CheckState.Checked;
         this.chkAuditSecurity.Location = new System.Drawing.Point(12, 62);
         this.chkAuditSecurity.Name = "chkAuditSecurity";
         this.chkAuditSecurity.Size = new System.Drawing.Size(120, 16);
         this.chkAuditSecurity.TabIndex = 2;
         this.chkAuditSecurity.Text = "&Security Changes";
         // 
         // chkAuditDDL
         // 
         this.chkAuditDDL.Checked = true;
         this.chkAuditDDL.CheckState = System.Windows.Forms.CheckState.Checked;
         this.chkAuditDDL.FlatStyle = System.Windows.Forms.FlatStyle.System;
         this.chkAuditDDL.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
         this.chkAuditDDL.Location = new System.Drawing.Point(12, 85);
         this.chkAuditDDL.Name = "chkAuditDDL";
         this.chkAuditDDL.Size = new System.Drawing.Size(148, 16);
         this.chkAuditDDL.TabIndex = 3;
         this.chkAuditDDL.Text = "Database Def&inition (DDL)";
         this.chkAuditDDL.TextAlign = System.Drawing.ContentAlignment.TopLeft;
         // 
         // panelAuditedUsers
         // 
         this.panelAuditedUsers.BackColor = System.Drawing.SystemColors.ControlLightLight;
         this.panelAuditedUsers.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
         this.panelAuditedUsers.Controls.Add(this.label4);
         this.panelAuditedUsers.Controls.Add(this.label5);
         this.panelAuditedUsers.Controls.Add(this.panel2);
         this.panelAuditedUsers.Location = new System.Drawing.Point(111, 0);
         this.panelAuditedUsers.Name = "panelAuditedUsers";
         this.panelAuditedUsers.Size = new System.Drawing.Size(448, 336);
         this.panelAuditedUsers.TabIndex = 25;
         // 
         // label4
         // 
         this.label4.Location = new System.Drawing.Point(14, 24);
         this.label4.Name = "label4";
         this.label4.Size = new System.Drawing.Size(420, 28);
         this.label4.TabIndex = 15;
         this.label4.Text = "Select users whose activity you want audited regardless of other audit settings l" +
             "ike included databases. Select server logins and roles to specify privileged use" +
             "rs.";
         // 
         // label5
         // 
         this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.label5.Location = new System.Drawing.Point(14, 8);
         this.label5.Name = "label5";
         this.label5.Size = new System.Drawing.Size(384, 16);
         this.label5.TabIndex = 14;
         this.label5.Text = "Privileged Users";
         // 
         // panel2
         // 
         this.panel2.BackColor = System.Drawing.SystemColors.Control;
         this.panel2.Controls.Add(this.label14);
         this.panel2.Controls.Add(this.btnRemovePriv);
         this.panel2.Controls.Add(this.btnAddPriv);
         this.panel2.Controls.Add(this.lstPrivilegedUsers);
         this.panel2.Controls.Add(this.label19);
         this.panel2.Location = new System.Drawing.Point(0, 60);
         this.panel2.Name = "panel2";
         this.panel2.Size = new System.Drawing.Size(448, 276);
         this.panel2.TabIndex = 16;
         // 
         // label14
         // 
         this.label14.Location = new System.Drawing.Point(12, 208);
         this.label14.Name = "label14";
         this.label14.Size = new System.Drawing.Size(420, 52);
         this.label14.TabIndex = 29;
         this.label14.Text = resources.GetString("label14.Text");
         // 
         // btnRemovePriv
         // 
         this.btnRemovePriv.Enabled = false;
         this.btnRemovePriv.Location = new System.Drawing.Point(380, 56);
         this.btnRemovePriv.Name = "btnRemovePriv";
         this.btnRemovePriv.Size = new System.Drawing.Size(60, 23);
         this.btnRemovePriv.TabIndex = 28;
         this.btnRemovePriv.Text = "&Remove";
         this.btnRemovePriv.Click += new System.EventHandler(this.btnRemovePriv_Click);
         // 
         // btnAddPriv
         // 
         this.btnAddPriv.Location = new System.Drawing.Point(380, 28);
         this.btnAddPriv.Name = "btnAddPriv";
         this.btnAddPriv.Size = new System.Drawing.Size(60, 23);
         this.btnAddPriv.TabIndex = 27;
         this.btnAddPriv.Text = "&Add...";
         this.btnAddPriv.Click += new System.EventHandler(this.btnAddPriv_Click);
         // 
         // lstPrivilegedUsers
         // 
         this.lstPrivilegedUsers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
         this.lstPrivilegedUsers.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
         this.lstPrivilegedUsers.HideSelection = false;
         this.lstPrivilegedUsers.Location = new System.Drawing.Point(12, 28);
         this.lstPrivilegedUsers.Name = "lstPrivilegedUsers";
         this.lstPrivilegedUsers.Size = new System.Drawing.Size(360, 168);
         this.lstPrivilegedUsers.TabIndex = 26;
         this.lstPrivilegedUsers.UseCompatibleStateImageBehavior = false;
         this.lstPrivilegedUsers.View = System.Windows.Forms.View.Details;
         // 
         // columnHeader1
         // 
         this.columnHeader1.Width = 335;
         // 
         // label19
         // 
         this.label19.Location = new System.Drawing.Point(12, 12);
         this.label19.Name = "label19";
         this.label19.Size = new System.Drawing.Size(176, 16);
         this.label19.TabIndex = 25;
         this.label19.Text = "&Audited Privileged Users:";
         // 
         // panelAuditedUserActivity
         // 
         this.panelAuditedUserActivity.BackColor = System.Drawing.SystemColors.ControlLightLight;
         this.panelAuditedUserActivity.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
         this.panelAuditedUserActivity.Controls.Add(this.linklblHelpBestPractices2);
         this.panelAuditedUserActivity.Controls.Add(this.label6);
         this.panelAuditedUserActivity.Controls.Add(this.label7);
         this.panelAuditedUserActivity.Controls.Add(this.panel3);
         this.panelAuditedUserActivity.Location = new System.Drawing.Point(111, 0);
         this.panelAuditedUserActivity.Name = "panelAuditedUserActivity";
         this.panelAuditedUserActivity.Size = new System.Drawing.Size(448, 336);
         this.panelAuditedUserActivity.TabIndex = 26;
         // 
         // linklblHelpBestPractices2
         // 
         this.linklblHelpBestPractices2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.linklblHelpBestPractices2.AutoSize = true;
         this.linklblHelpBestPractices2.LinkArea = new System.Windows.Forms.LinkArea(0, 99);
         this.linklblHelpBestPractices2.Location = new System.Drawing.Point(15, 37);
         this.linklblHelpBestPractices2.Name = "linklblHelpBestPractices2";
         this.linklblHelpBestPractices2.Size = new System.Drawing.Size(277, 17);
         this.linklblHelpBestPractices2.TabIndex = 39;
         this.linklblHelpBestPractices2.TabStop = true;
         this.linklblHelpBestPractices2.Text = "Learn how to optimize performance with audit settings.";
         this.linklblHelpBestPractices2.UseCompatibleTextRendering = true;
         this.linklblHelpBestPractices2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linklblHelpBestPractices2_LinkClicked);
         // 
         // label6
         // 
         this.label6.Location = new System.Drawing.Point(14, 24);
         this.label6.Name = "label6";
         this.label6.Size = new System.Drawing.Size(420, 28);
         this.label6.TabIndex = 15;
         this.label6.Text = "Select which activities to audit for privileged users.";
         // 
         // label7
         // 
         this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.label7.Location = new System.Drawing.Point(14, 8);
         this.label7.Name = "label7";
         this.label7.Size = new System.Drawing.Size(384, 16);
         this.label7.TabIndex = 14;
         this.label7.Text = "Privileged User Audited Activity";
         // 
         // panel3
         // 
         this.panel3.BackColor = System.Drawing.SystemColors.Control;
         this.panel3.Controls.Add(this.grpAuditUserActivity);
         this.panel3.Controls.Add(this.rbAuditUserSelected);
         this.panel3.Controls.Add(this.rbAuditUserAll);
         this.panel3.Location = new System.Drawing.Point(0, 60);
         this.panel3.Name = "panel3";
         this.panel3.Size = new System.Drawing.Size(448, 276);
         this.panel3.TabIndex = 16;
         // 
         // grpAuditUserActivity
         // 
         this.grpAuditUserActivity.Controls.Add(this.chkAuditUserCaptureTrans);
         this.grpAuditUserActivity.Controls.Add(this.chkAuditUserUDE);
         this.grpAuditUserActivity.Controls.Add(this._rbUserAuditFailed);
         this.grpAuditUserActivity.Controls.Add(this._rbUserAuditPassed);
         this.grpAuditUserActivity.Controls.Add(this.chkAuditUserAdmin);
         this.grpAuditUserActivity.Controls.Add(this.chkAuditUserCaptureSQL);
         this.grpAuditUserActivity.Controls.Add(this.chkUserAccessCheckFilter);
         this.grpAuditUserActivity.Controls.Add(this.chkAuditUserDML);
         this.grpAuditUserActivity.Controls.Add(this.chkAuditUserSELECT);
         this.grpAuditUserActivity.Controls.Add(this.chkAuditUserSecurity);
         this.grpAuditUserActivity.Controls.Add(this.chkAuditUserDDL);
         this.grpAuditUserActivity.Controls.Add(this.chkAuditUserFailedLogins);
         this.grpAuditUserActivity.Controls.Add(this.chkAuditUserLogins);
         this.grpAuditUserActivity.Location = new System.Drawing.Point(28, 52);
         this.grpAuditUserActivity.Name = "grpAuditUserActivity";
         this.grpAuditUserActivity.Size = new System.Drawing.Size(400, 192);
         this.grpAuditUserActivity.TabIndex = 28;
         this.grpAuditUserActivity.TabStop = false;
         // 
         // chkAuditUserUDE
         // 
         this.chkAuditUserUDE.Location = new System.Drawing.Point(176, 84);
         this.chkAuditUserUDE.Name = "chkAuditUserUDE";
         this.chkAuditUserUDE.Size = new System.Drawing.Size(188, 16);
         this.chkAuditUserUDE.TabIndex = 7;
         this.chkAuditUserUDE.Text = "&User Defined Events";
         // 
         // _rbUserAuditFailed
         // 
         this._rbUserAuditFailed.Location = new System.Drawing.Point(304, 126);
         this._rbUserAuditFailed.Name = "_rbUserAuditFailed";
         this._rbUserAuditFailed.Size = new System.Drawing.Size(60, 16);
         this._rbUserAuditFailed.TabIndex = 10;
         this._rbUserAuditFailed.Text = "Faile&d";
         // 
         // _rbUserAuditPassed
         // 
         this._rbUserAuditPassed.Location = new System.Drawing.Point(224, 126);
         this._rbUserAuditPassed.Name = "_rbUserAuditPassed";
         this._rbUserAuditPassed.Size = new System.Drawing.Size(76, 16);
         this._rbUserAuditPassed.TabIndex = 9;
         this._rbUserAuditPassed.Text = "&Passed";
         // 
         // chkAuditUserAdmin
         // 
         this.chkAuditUserAdmin.Checked = true;
         this.chkAuditUserAdmin.CheckState = System.Windows.Forms.CheckState.Checked;
         this.chkAuditUserAdmin.FlatStyle = System.Windows.Forms.FlatStyle.System;
         this.chkAuditUserAdmin.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
         this.chkAuditUserAdmin.Location = new System.Drawing.Point(8, 84);
         this.chkAuditUserAdmin.Name = "chkAuditUserAdmin";
         this.chkAuditUserAdmin.Size = new System.Drawing.Size(164, 16);
         this.chkAuditUserAdmin.TabIndex = 6;
         this.chkAuditUserAdmin.Text = "&Administrative actions";
         this.chkAuditUserAdmin.TextAlign = System.Drawing.ContentAlignment.TopLeft;
         // 
         // chkAuditUserCaptureSQL
         // 
         this.chkAuditUserCaptureSQL.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
         this.chkAuditUserCaptureSQL.Location = new System.Drawing.Point(8, 148);
         this.chkAuditUserCaptureSQL.Name = "chkAuditUserCaptureSQL";
         this.chkAuditUserCaptureSQL.Size = new System.Drawing.Size(372, 20);
         this.chkAuditUserCaptureSQL.TabIndex = 11;
         this.chkAuditUserCaptureSQL.Text = "Capture S&QL statements for DML and SELECT activities.";
         this.chkAuditUserCaptureSQL.TextAlign = System.Drawing.ContentAlignment.TopLeft;
         this.chkAuditUserCaptureSQL.CheckedChanged += new System.EventHandler(this.chkUserCaptureSQL_CheckedChanged);
         // 
         // chkUserAccessCheckFilter
         // 
         this.chkUserAccessCheckFilter.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
         this.chkUserAccessCheckFilter.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
         this.chkUserAccessCheckFilter.Location = new System.Drawing.Point(8, 124);
         this.chkUserAccessCheckFilter.Name = "chkUserAccessCheckFilter";
         this.chkUserAccessCheckFilter.Size = new System.Drawing.Size(220, 20);
         this.chkUserAccessCheckFilter.TabIndex = 8;
         this.chkUserAccessCheckFilter.Text = "&Filter events based on access check:";
         this.chkUserAccessCheckFilter.Click += new System.EventHandler(this.Click_chkUserAccessCheckFilter);
         // 
         // chkAuditUserDML
         // 
         this.chkAuditUserDML.FlatStyle = System.Windows.Forms.FlatStyle.System;
         this.chkAuditUserDML.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
         this.chkAuditUserDML.Location = new System.Drawing.Point(176, 36);
         this.chkAuditUserDML.Name = "chkAuditUserDML";
         this.chkAuditUserDML.Size = new System.Drawing.Size(188, 16);
         this.chkAuditUserDML.TabIndex = 3;
         this.chkAuditUserDML.Text = "Database &Modification (DML)";
         this.chkAuditUserDML.TextAlign = System.Drawing.ContentAlignment.TopLeft;
         this.chkAuditUserDML.CheckedChanged += new System.EventHandler(this.chkAuditUserDML_CheckedChanged);
         // 
         // chkAuditUserSELECT
         // 
         this.chkAuditUserSELECT.Location = new System.Drawing.Point(176, 60);
         this.chkAuditUserSELECT.Name = "chkAuditUserSELECT";
         this.chkAuditUserSELECT.Size = new System.Drawing.Size(188, 16);
         this.chkAuditUserSELECT.TabIndex = 5;
         this.chkAuditUserSELECT.Text = "Database S&ELECTs";
         this.chkAuditUserSELECT.CheckedChanged += new System.EventHandler(this.chkAuditUserDML_CheckedChanged);
         // 
         // chkAuditUserSecurity
         // 
         this.chkAuditUserSecurity.Checked = true;
         this.chkAuditUserSecurity.CheckState = System.Windows.Forms.CheckState.Checked;
         this.chkAuditUserSecurity.Location = new System.Drawing.Point(8, 60);
         this.chkAuditUserSecurity.Name = "chkAuditUserSecurity";
         this.chkAuditUserSecurity.Size = new System.Drawing.Size(152, 16);
         this.chkAuditUserSecurity.TabIndex = 4;
         this.chkAuditUserSecurity.Text = "&Security changes";
         // 
         // chkAuditUserDDL
         // 
         this.chkAuditUserDDL.Checked = true;
         this.chkAuditUserDDL.CheckState = System.Windows.Forms.CheckState.Checked;
         this.chkAuditUserDDL.FlatStyle = System.Windows.Forms.FlatStyle.System;
         this.chkAuditUserDDL.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
         this.chkAuditUserDDL.Location = new System.Drawing.Point(176, 12);
         this.chkAuditUserDDL.Name = "chkAuditUserDDL";
         this.chkAuditUserDDL.Size = new System.Drawing.Size(192, 16);
         this.chkAuditUserDDL.TabIndex = 1;
         this.chkAuditUserDDL.Text = "Database Def&inition (DDL)";
         this.chkAuditUserDDL.TextAlign = System.Drawing.ContentAlignment.TopLeft;
         // 
         // chkAuditUserFailedLogins
         // 
         this.chkAuditUserFailedLogins.Checked = true;
         this.chkAuditUserFailedLogins.CheckState = System.Windows.Forms.CheckState.Checked;
         this.chkAuditUserFailedLogins.Location = new System.Drawing.Point(8, 36);
         this.chkAuditUserFailedLogins.Name = "chkAuditUserFailedLogins";
         this.chkAuditUserFailedLogins.Size = new System.Drawing.Size(144, 16);
         this.chkAuditUserFailedLogins.TabIndex = 2;
         this.chkAuditUserFailedLogins.Text = "Failed lo&gins";
         // 
         // chkAuditUserLogins
         // 
         this.chkAuditUserLogins.Checked = true;
         this.chkAuditUserLogins.CheckState = System.Windows.Forms.CheckState.Checked;
         this.chkAuditUserLogins.Location = new System.Drawing.Point(8, 12);
         this.chkAuditUserLogins.Name = "chkAuditUserLogins";
         this.chkAuditUserLogins.Size = new System.Drawing.Size(156, 16);
         this.chkAuditUserLogins.TabIndex = 0;
         this.chkAuditUserLogins.Text = "&Logins";
         // 
         // rbAuditUserSelected
         // 
         this.rbAuditUserSelected.Checked = true;
         this.rbAuditUserSelected.Location = new System.Drawing.Point(12, 36);
         this.rbAuditUserSelected.Name = "rbAuditUserSelected";
         this.rbAuditUserSelected.Size = new System.Drawing.Size(292, 16);
         this.rbAuditUserSelected.TabIndex = 27;
         this.rbAuditUserSelected.TabStop = true;
         this.rbAuditUserSelected.Text = "Audit selected activities done by privileged users";
         this.rbAuditUserSelected.CheckedChanged += new System.EventHandler(this.rbAuditUserSelected_CheckedChanged);
         // 
         // rbAuditUserAll
         // 
         this.rbAuditUserAll.Location = new System.Drawing.Point(12, 12);
         this.rbAuditUserAll.Name = "rbAuditUserAll";
         this.rbAuditUserAll.Size = new System.Drawing.Size(260, 16);
         this.rbAuditUserAll.TabIndex = 26;
         this.rbAuditUserAll.Text = "Audit all activities done by privileged users";
         this.rbAuditUserAll.CheckedChanged += new System.EventHandler(this.rbAuditUserSelected_CheckedChanged);
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
         this.label8.Text = "Ready to register a SQL Server instance";
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
         this.panel4.Controls.Add(this._flblVirtualServerInfo);
         this.panel4.Controls.Add(this.lblInstance);
         this.panel4.Controls.Add(this.label11);
         this.panel4.Controls.Add(this.label10);
         this.panel4.Location = new System.Drawing.Point(0, 60);
         this.panel4.Name = "panel4";
         this.panel4.Size = new System.Drawing.Size(448, 276);
         this.panel4.TabIndex = 16;
         // 
         // _flblVirtualServerInfo
         // 
         this._flblVirtualServerInfo.Location = new System.Drawing.Point(12, 146);
         this._flblVirtualServerInfo.Name = "_flblVirtualServerInfo";
         this._flblVirtualServerInfo.Size = new System.Drawing.Size(412, 59);
         this._flblVirtualServerInfo.TabIndex = 3;
         this._flblVirtualServerInfo.TabStop = true;
         this._flblVirtualServerInfo.Value = resources.GetString("_flblVirtualServerInfo.Value");
         this._flblVirtualServerInfo.LinkClicked += new Infragistics.Win.FormattedLinkLabel.LinkClickedEventHandler(this.LinkClicked_flblVirtualServerInfo);
         // 
         // lblInstance
         // 
         this.lblInstance.Location = new System.Drawing.Point(44, 48);
         this.lblInstance.Name = "lblInstance";
         this.lblInstance.Size = new System.Drawing.Size(384, 36);
         this.lblInstance.TabIndex = 2;
         this.lblInstance.Text = "txtSummaryInstance";
         // 
         // label11
         // 
         this.label11.Location = new System.Drawing.Point(12, 12);
         this.label11.Name = "label11";
         this.label11.Size = new System.Drawing.Size(420, 28);
         this.label11.TabIndex = 1;
         this.label11.Text = "You have finished entering the data necessary to register the following SQL Serve" +
             "r with SQL Compliance Manager for auditing:";
         // 
         // label10
         // 
         this.label10.Location = new System.Drawing.Point(12, 96);
         this.label10.Name = "label10";
         this.label10.Size = new System.Drawing.Size(412, 44);
         this.label10.TabIndex = 0;
         this.label10.Text = "Click Finish to begin the registration process. This may take several minutes as " +
             "it initializes the repository databases and optionally installs the SQLcomplianc" +
             "e Agent.";
         // 
         // panelAgentTrace
         // 
         this.panelAgentTrace.BackColor = System.Drawing.SystemColors.ControlLightLight;
         this.panelAgentTrace.Controls.Add(this.panel7);
         this.panelAgentTrace.Controls.Add(this.labeAgentTraceDescription);
         this.panelAgentTrace.Controls.Add(this.labelAgentTraceTitle);
         this.panelAgentTrace.Location = new System.Drawing.Point(111, 0);
         this.panelAgentTrace.Name = "panelAgentTrace";
         this.panelAgentTrace.Size = new System.Drawing.Size(448, 336);
         this.panelAgentTrace.TabIndex = 29;
         // 
         // panel7
         // 
         this.panel7.BackColor = System.Drawing.SystemColors.Control;
         this.panel7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
         this.panel7.Controls.Add(this.label13);
         this.panel7.Controls.Add(this.label12);
         this.panel7.Controls.Add(this.txtTraceDirectory);
         this.panel7.Controls.Add(this.radioSpecifyTrace);
         this.panel7.Controls.Add(this.radioDefaultTrace);
         this.panel7.Location = new System.Drawing.Point(0, 60);
         this.panel7.Name = "panel7";
         this.panel7.Size = new System.Drawing.Size(448, 276);
         this.panel7.TabIndex = 53;
         // 
         // label13
         // 
         this.label13.Location = new System.Drawing.Point(12, 12);
         this.label13.Name = "label13";
         this.label13.Size = new System.Drawing.Size(420, 40);
         this.label13.TabIndex = 4;
         this.label13.Text = resources.GetString("label13.Text");
         // 
         // label12
         // 
         this.label12.Location = new System.Drawing.Point(48, 164);
         this.label12.Name = "label12";
         this.label12.Size = new System.Drawing.Size(368, 24);
         this.label12.TabIndex = 3;
         this.label12.Text = "Note:  This directory will be created by the agent installation.";
         // 
         // txtTraceDirectory
         // 
         this.txtTraceDirectory.Enabled = false;
         this.txtTraceDirectory.Location = new System.Drawing.Point(48, 136);
         this.txtTraceDirectory.MaxLength = 255;
         this.txtTraceDirectory.Name = "txtTraceDirectory";
         this.txtTraceDirectory.Size = new System.Drawing.Size(348, 20);
         this.txtTraceDirectory.TabIndex = 2;
         // 
         // radioSpecifyTrace
         // 
         this.radioSpecifyTrace.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
         this.radioSpecifyTrace.Location = new System.Drawing.Point(32, 116);
         this.radioSpecifyTrace.Name = "radioSpecifyTrace";
         this.radioSpecifyTrace.Size = new System.Drawing.Size(364, 20);
         this.radioSpecifyTrace.TabIndex = 1;
         this.radioSpecifyTrace.Text = "Specify alternate trace directory:";
         this.radioSpecifyTrace.CheckedChanged += new System.EventHandler(this.radioSpecifyTrace_CheckedChanged);
         // 
         // radioDefaultTrace
         // 
         this.radioDefaultTrace.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
         this.radioDefaultTrace.Checked = true;
         this.radioDefaultTrace.Location = new System.Drawing.Point(32, 64);
         this.radioDefaultTrace.Name = "radioDefaultTrace";
         this.radioDefaultTrace.Size = new System.Drawing.Size(380, 40);
         this.radioDefaultTrace.TabIndex = 0;
         this.radioDefaultTrace.TabStop = true;
         this.radioDefaultTrace.Text = "Use default trace directory - By default, the SQLcompliance Agent will store coll" +
             "ected audit data in a protected subdirectory of the agent installation directory" +
             ".";
         this.radioDefaultTrace.CheckedChanged += new System.EventHandler(this.radioSpecifyTrace_CheckedChanged);
         // 
         // labeAgentTraceDescription
         // 
         this.labeAgentTraceDescription.Location = new System.Drawing.Point(14, 24);
         this.labeAgentTraceDescription.Name = "labeAgentTraceDescription";
         this.labeAgentTraceDescription.Size = new System.Drawing.Size(420, 28);
         this.labeAgentTraceDescription.TabIndex = 52;
         this.labeAgentTraceDescription.Text = "Specify directory for temporary storage of audit data";
         // 
         // labelAgentTraceTitle
         // 
         this.labelAgentTraceTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.labelAgentTraceTitle.Location = new System.Drawing.Point(14, 8);
         this.labelAgentTraceTitle.Name = "labelAgentTraceTitle";
         this.labelAgentTraceTitle.Size = new System.Drawing.Size(384, 16);
         this.labelAgentTraceTitle.TabIndex = 51;
         this.labelAgentTraceTitle.Text = "SQLcompliance Agent Trace Directory";
         // 
         // panelLicenseLimit
         // 
         this.panelLicenseLimit.BackColor = System.Drawing.SystemColors.ControlLightLight;
         this.panelLicenseLimit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
         this.panelLicenseLimit.Controls.Add(this.lblLicenseDescription);
         this.panelLicenseLimit.Controls.Add(this.lblLicenseTitle);
         this.panelLicenseLimit.Controls.Add(this.panelLicenseProperties);
         this.panelLicenseLimit.Location = new System.Drawing.Point(111, 0);
         this.panelLicenseLimit.Name = "panelLicenseLimit";
         this.panelLicenseLimit.Size = new System.Drawing.Size(448, 336);
         this.panelLicenseLimit.TabIndex = 30;
         // 
         // lblLicenseDescription
         // 
         this.lblLicenseDescription.Location = new System.Drawing.Point(14, 24);
         this.lblLicenseDescription.Name = "lblLicenseDescription";
         this.lblLicenseDescription.Size = new System.Drawing.Size(420, 28);
         this.lblLicenseDescription.TabIndex = 15;
         this.lblLicenseDescription.Text = "Maximum number of licensed instances reached";
         // 
         // lblLicenseTitle
         // 
         this.lblLicenseTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.lblLicenseTitle.Location = new System.Drawing.Point(14, 8);
         this.lblLicenseTitle.Name = "lblLicenseTitle";
         this.lblLicenseTitle.Size = new System.Drawing.Size(384, 16);
         this.lblLicenseTitle.TabIndex = 14;
         this.lblLicenseTitle.Text = "License Limit Reached";
         // 
         // panelLicenseProperties
         // 
         this.panelLicenseProperties.BackColor = System.Drawing.SystemColors.Control;
         this.panelLicenseProperties.Controls.Add(this.pictureBox2);
         this.panelLicenseProperties.Controls.Add(this.lblLicenseViolation);
         this.panelLicenseProperties.Location = new System.Drawing.Point(0, 60);
         this.panelLicenseProperties.Name = "panelLicenseProperties";
         this.panelLicenseProperties.Size = new System.Drawing.Size(448, 276);
         this.panelLicenseProperties.TabIndex = 16;
         // 
         // pictureBox2
         // 
         this.pictureBox2.Image = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.StatusWarning_48;
         this.pictureBox2.Location = new System.Drawing.Point(52, 32);
         this.pictureBox2.Name = "pictureBox2";
         this.pictureBox2.Size = new System.Drawing.Size(48, 48);
         this.pictureBox2.TabIndex = 1;
         this.pictureBox2.TabStop = false;
         // 
         // lblLicenseViolation
         // 
         this.lblLicenseViolation.Location = new System.Drawing.Point(116, 36);
         this.lblLicenseViolation.Name = "lblLicenseViolation";
         this.lblLicenseViolation.Size = new System.Drawing.Size(284, 40);
         this.lblLicenseViolation.TabIndex = 0;
         this.lblLicenseViolation.Text = "You have already reached the maximum number of registered SQL Servers allowed by " +
             "the current license. Please contact IDERA to purchase additional licenses.";
         // 
         // panelPermissions
         // 
         this.panelPermissions.BackColor = System.Drawing.SystemColors.ControlLightLight;
         this.panelPermissions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
         this.panelPermissions.Controls.Add(this.lblPermissionsDescription);
         this.panelPermissions.Controls.Add(this.labelPermissionsTitle);
         this.panelPermissions.Controls.Add(this.panelPermissionsProperties);
         this.panelPermissions.Location = new System.Drawing.Point(111, 0);
         this.panelPermissions.Name = "panelPermissions";
         this.panelPermissions.Size = new System.Drawing.Size(448, 336);
         this.panelPermissions.TabIndex = 31;
         // 
         // lblPermissionsDescription
         // 
         this.lblPermissionsDescription.Location = new System.Drawing.Point(14, 24);
         this.lblPermissionsDescription.Name = "lblPermissionsDescription";
         this.lblPermissionsDescription.Size = new System.Drawing.Size(420, 28);
         this.lblPermissionsDescription.TabIndex = 15;
         this.lblPermissionsDescription.Text = "Select the default level of access to audit data for this SQL Server instance..";
         // 
         // labelPermissionsTitle
         // 
         this.labelPermissionsTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.labelPermissionsTitle.Location = new System.Drawing.Point(14, 8);
         this.labelPermissionsTitle.Name = "labelPermissionsTitle";
         this.labelPermissionsTitle.Size = new System.Drawing.Size(384, 16);
         this.labelPermissionsTitle.TabIndex = 14;
         this.labelPermissionsTitle.Text = "Default Permissions";
         // 
         // panelPermissionsProperties
         // 
         this.panelPermissionsProperties.BackColor = System.Drawing.SystemColors.Control;
         this.panelPermissionsProperties.Controls.Add(this.label16);
         this.panelPermissionsProperties.Controls.Add(this.groupBox3);
         this.panelPermissionsProperties.Location = new System.Drawing.Point(0, 60);
         this.panelPermissionsProperties.Name = "panelPermissionsProperties";
         this.panelPermissionsProperties.Size = new System.Drawing.Size(448, 276);
         this.panelPermissionsProperties.TabIndex = 16;
         // 
         // label16
         // 
         this.label16.Location = new System.Drawing.Point(12, 8);
         this.label16.Name = "label16";
         this.label16.Size = new System.Drawing.Size(424, 48);
         this.label16.TabIndex = 30;
         this.label16.Text = "SQL Compliance Manager creates a database for each registered SQL Server instance" +
             " to hold collected audit data.  Select the default level of access you want to g" +
             "rant users.";
         // 
         // groupBox3
         // 
         this.groupBox3.Controls.Add(this.radioGrantAll);
         this.groupBox3.Controls.Add(this.radioGrantEventsOnly);
         this.groupBox3.Controls.Add(this.radioDeny);
         this.groupBox3.Location = new System.Drawing.Point(12, 56);
         this.groupBox3.Name = "groupBox3";
         this.groupBox3.Size = new System.Drawing.Size(420, 149);
         this.groupBox3.TabIndex = 29;
         this.groupBox3.TabStop = false;
         this.groupBox3.Text = "Default Database Permissions";
         // 
         // radioGrantAll
         // 
         this.radioGrantAll.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
         this.radioGrantAll.Checked = true;
         this.radioGrantAll.Location = new System.Drawing.Point(8, 20);
         this.radioGrantAll.Name = "radioGrantAll";
         this.radioGrantAll.Size = new System.Drawing.Size(372, 16);
         this.radioGrantAll.TabIndex = 37;
         this.radioGrantAll.TabStop = true;
         this.radioGrantAll.Text = "Grant right to read events and their associated SQL statements .";
         // 
         // radioGrantEventsOnly
         // 
         this.radioGrantEventsOnly.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
         this.radioGrantEventsOnly.Location = new System.Drawing.Point(8, 44);
         this.radioGrantEventsOnly.Name = "radioGrantEventsOnly";
         this.radioGrantEventsOnly.Size = new System.Drawing.Size(404, 40);
         this.radioGrantEventsOnly.TabIndex = 36;
         this.radioGrantEventsOnly.Text = "Grant right to read events only - To allow users to view the associated SQL state" +
             "ments, you will need to explicictly grant users read access to the database.";
         // 
         // radioDeny
         // 
         this.radioDeny.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
         this.radioDeny.Location = new System.Drawing.Point(8, 88);
         this.radioDeny.Name = "radioDeny";
         this.radioDeny.Size = new System.Drawing.Size(392, 50);
         this.radioDeny.TabIndex = 35;
         this.radioDeny.Text = "Deny read access by default - To allow users to view events and the associated SQ" +
             "L, you will need to explicitly grant users read access to the database.";
         this.radioDeny.TextAlign = System.Drawing.ContentAlignment.TopLeft;
         // 
         // panelExistingDatabase
         // 
         this.panelExistingDatabase.BackColor = System.Drawing.SystemColors.ControlLightLight;
         this.panelExistingDatabase.Controls.Add(this.panel5);
         this.panelExistingDatabase.Controls.Add(this.label17);
         this.panelExistingDatabase.Controls.Add(this.label18);
         this.panelExistingDatabase.Location = new System.Drawing.Point(111, 0);
         this.panelExistingDatabase.Name = "panelExistingDatabase";
         this.panelExistingDatabase.Size = new System.Drawing.Size(448, 336);
         this.panelExistingDatabase.TabIndex = 32;
         // 
         // panel5
         // 
         this.panel5.BackColor = System.Drawing.SystemColors.Control;
         this.panel5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
         this.panel5.Controls.Add(this.textExistingDatabase);
         this.panel5.Controls.Add(this.label27);
         this.panel5.Controls.Add(this.label20);
         this.panel5.Controls.Add(this.radioDeleteDatabase);
         this.panel5.Controls.Add(this.radioMaintainDatabase);
         this.panel5.Location = new System.Drawing.Point(0, 60);
         this.panel5.Name = "panel5";
         this.panel5.Size = new System.Drawing.Size(448, 276);
         this.panel5.TabIndex = 35;
         // 
         // textExistingDatabase
         // 
         this.textExistingDatabase.Location = new System.Drawing.Point(81, 224);
         this.textExistingDatabase.Name = "textExistingDatabase";
         this.textExistingDatabase.Size = new System.Drawing.Size(340, 44);
         this.textExistingDatabase.TabIndex = 41;
         // 
         // label27
         // 
         this.label27.Location = new System.Drawing.Point(25, 224);
         this.label27.Name = "label27";
         this.label27.Size = new System.Drawing.Size(56, 16);
         this.label27.TabIndex = 40;
         this.label27.Text = "Database:";
         // 
         // label20
         // 
         this.label20.Location = new System.Drawing.Point(12, 12);
         this.label20.Name = "label20";
         this.label20.Size = new System.Drawing.Size(412, 40);
         this.label20.TabIndex = 37;
         this.label20.Text = "A database containing audit data for this SQL Server instance already exists. To " +
             "avoid losing any previously collected audit data, use the existing database and " +
             "keep the existing audit data.";
         // 
         // radioDeleteDatabase
         // 
         this.radioDeleteDatabase.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
         this.radioDeleteDatabase.Location = new System.Drawing.Point(20, 116);
         this.radioDeleteDatabase.Name = "radioDeleteDatabase";
         this.radioDeleteDatabase.Size = new System.Drawing.Size(360, 44);
         this.radioDeleteDatabase.TabIndex = 36;
         this.radioDeleteDatabase.Text = "Delete the previously collected audit data but use the existing database. This op" +
             "tion will reinitialize the existing database. All audit data in the database wil" +
             "l be permanently deleted.";
         // 
         // radioMaintainDatabase
         // 
         this.radioMaintainDatabase.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
         this.radioMaintainDatabase.Checked = true;
         this.radioMaintainDatabase.Location = new System.Drawing.Point(20, 64);
         this.radioMaintainDatabase.Name = "radioMaintainDatabase";
         this.radioMaintainDatabase.Size = new System.Drawing.Size(380, 44);
         this.radioMaintainDatabase.TabIndex = 35;
         this.radioMaintainDatabase.TabStop = true;
         this.radioMaintainDatabase.Text = "Keep the previously collected audit data and use the existing database.  If the  " +
             "events database is from a previous version of SQL Compliance Manager,  it will b" +
             "e automatically upgraded.";
         this.radioMaintainDatabase.TextAlign = System.Drawing.ContentAlignment.TopLeft;
         // 
         // label17
         // 
         this.label17.Location = new System.Drawing.Point(14, 24);
         this.label17.Name = "label17";
         this.label17.Size = new System.Drawing.Size(420, 28);
         this.label17.TabIndex = 34;
         this.label17.Text = "Audit data for this SQL Server instance already exists.";
         // 
         // label18
         // 
         this.label18.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.label18.Location = new System.Drawing.Point(14, 8);
         this.label18.Name = "label18";
         this.label18.Size = new System.Drawing.Size(384, 16);
         this.label18.TabIndex = 33;
         this.label18.Text = "Existing Audit Data";
         // 
         // panelIncompatibleDatabase
         // 
         this.panelIncompatibleDatabase.BackColor = System.Drawing.SystemColors.ControlLightLight;
         this.panelIncompatibleDatabase.Controls.Add(this.panel6);
         this.panelIncompatibleDatabase.Controls.Add(this.label23);
         this.panelIncompatibleDatabase.Controls.Add(this.label22);
         this.panelIncompatibleDatabase.Location = new System.Drawing.Point(111, 0);
         this.panelIncompatibleDatabase.Name = "panelIncompatibleDatabase";
         this.panelIncompatibleDatabase.Size = new System.Drawing.Size(448, 336);
         this.panelIncompatibleDatabase.TabIndex = 33;
         // 
         // panel6
         // 
         this.panel6.BackColor = System.Drawing.SystemColors.Control;
         this.panel6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
         this.panel6.Controls.Add(this.textDatabaseName);
         this.panel6.Controls.Add(this.label24);
         this.panel6.Controls.Add(this.label21);
         this.panel6.Controls.Add(this.radioIncompatibleOverwrite);
         this.panel6.Controls.Add(this.radioIncompatibleCancel);
         this.panel6.Location = new System.Drawing.Point(0, 60);
         this.panel6.Name = "panel6";
         this.panel6.Size = new System.Drawing.Size(448, 276);
         this.panel6.TabIndex = 35;
         // 
         // textDatabaseName
         // 
         this.textDatabaseName.Location = new System.Drawing.Point(80, 224);
         this.textDatabaseName.Name = "textDatabaseName";
         this.textDatabaseName.Size = new System.Drawing.Size(340, 44);
         this.textDatabaseName.TabIndex = 39;
         // 
         // label24
         // 
         this.label24.Location = new System.Drawing.Point(24, 224);
         this.label24.Name = "label24";
         this.label24.Size = new System.Drawing.Size(56, 16);
         this.label24.TabIndex = 38;
         this.label24.Text = "Database:";
         // 
         // label21
         // 
         this.label21.Location = new System.Drawing.Point(12, 12);
         this.label21.Name = "label21";
         this.label21.Size = new System.Drawing.Size(412, 52);
         this.label21.TabIndex = 37;
         this.label21.Text = resources.GetString("label21.Text");
         // 
         // radioIncompatibleOverwrite
         // 
         this.radioIncompatibleOverwrite.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
         this.radioIncompatibleOverwrite.Location = new System.Drawing.Point(24, 108);
         this.radioIncompatibleOverwrite.Name = "radioIncompatibleOverwrite";
         this.radioIncompatibleOverwrite.Size = new System.Drawing.Size(376, 44);
         this.radioIncompatibleOverwrite.TabIndex = 36;
         this.radioIncompatibleOverwrite.Text = "Continue registering this SQL Server instance and replace the existing database. " +
             "This option will reinitialize the existing database. All audit data in the datab" +
             "ase will be permanently deleted.";
         // 
         // radioIncompatibleCancel
         // 
         this.radioIncompatibleCancel.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
         this.radioIncompatibleCancel.Checked = true;
         this.radioIncompatibleCancel.Location = new System.Drawing.Point(24, 72);
         this.radioIncompatibleCancel.Name = "radioIncompatibleCancel";
         this.radioIncompatibleCancel.Size = new System.Drawing.Size(420, 32);
         this.radioIncompatibleCancel.TabIndex = 35;
         this.radioIncompatibleCancel.TabStop = true;
         this.radioIncompatibleCancel.Text = "Cancel the registration of this SQL Server instance to allow backing up the exist" +
             "ing database. This option will exit the New Registered SQL Server wizard.";
         this.radioIncompatibleCancel.TextAlign = System.Drawing.ContentAlignment.TopLeft;
         this.radioIncompatibleCancel.CheckedChanged += new System.EventHandler(this.radioIncompatible_CheckedChanged);
         // 
         // label23
         // 
         this.label23.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.label23.Location = new System.Drawing.Point(14, 8);
         this.label23.Name = "label23";
         this.label23.Size = new System.Drawing.Size(384, 16);
         this.label23.TabIndex = 33;
         this.label23.Text = "Existing Incompatible Database";
         // 
         // label22
         // 
         this.label22.Location = new System.Drawing.Point(14, 24);
         this.label22.Name = "label22";
         this.label22.Size = new System.Drawing.Size(420, 28);
         this.label22.TabIndex = 34;
         this.label22.Text = "A database with an unsupported schema already exists for this instance.";
         // 
         // panelCluster
         // 
         this.panelCluster.BackColor = System.Drawing.SystemColors.ControlLightLight;
         this.panelCluster.Controls.Add(this.panel8);
         this.panelCluster.Controls.Add(this.label30);
         this.panelCluster.Controls.Add(this.label32);
         this.panelCluster.Location = new System.Drawing.Point(111, 0);
         this.panelCluster.Name = "panelCluster";
         this.panelCluster.Size = new System.Drawing.Size(448, 336);
         this.panelCluster.TabIndex = 34;
         // 
         // panel8
         // 
         this.panel8.BackColor = System.Drawing.SystemColors.Control;
         this.panel8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
         this.panel8.Controls.Add(this.checkVirtualServer);
         this.panel8.Controls.Add(this.label29);
         this.panel8.Location = new System.Drawing.Point(0, 60);
         this.panel8.Name = "panel8";
         this.panel8.Size = new System.Drawing.Size(448, 276);
         this.panel8.TabIndex = 35;
         // 
         // checkVirtualServer
         // 
         this.checkVirtualServer.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
         this.checkVirtualServer.Location = new System.Drawing.Point(20, 60);
         this.checkVirtualServer.Name = "checkVirtualServer";
         this.checkVirtualServer.Size = new System.Drawing.Size(400, 40);
         this.checkVirtualServer.TabIndex = 40;
         this.checkVirtualServer.Text = "&This SQL Server instance is hosted by a Microsoft SQL Server Cluster virtual ser" +
             "ver.";
         this.checkVirtualServer.TextAlign = System.Drawing.ContentAlignment.TopLeft;
         // 
         // label29
         // 
         this.label29.Location = new System.Drawing.Point(12, 12);
         this.label29.Name = "label29";
         this.label29.Size = new System.Drawing.Size(412, 40);
         this.label29.TabIndex = 37;
         this.label29.Text = "Select whether this SQL Server instance is a virtual SQL Server hosted by Microso" +
             "ft Cluster Services. This choice affects the deployment options available for th" +
             "is SQL Server.";
         // 
         // label30
         // 
         this.label30.Location = new System.Drawing.Point(14, 24);
         this.label30.Name = "label30";
         this.label30.Size = new System.Drawing.Size(420, 28);
         this.label30.TabIndex = 34;
         this.label30.Text = "Specify whether this is a virtual SQL Server hosted on a cluster.";
         // 
         // label32
         // 
         this.label32.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.label32.Location = new System.Drawing.Point(14, 8);
         this.label32.Name = "label32";
         this.label32.Size = new System.Drawing.Size(384, 16);
         this.label32.TabIndex = 33;
         this.label32.Text = "SQL Server Cluster";
         // 
         // panelCantAudit2005
         // 
         this.panelCantAudit2005.BackColor = System.Drawing.SystemColors.ControlLightLight;
         this.panelCantAudit2005.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
         this.panelCantAudit2005.Controls.Add(this.label28);
         this.panelCantAudit2005.Controls.Add(this.label33);
         this.panelCantAudit2005.Controls.Add(this.panel9);
         this.panelCantAudit2005.Location = new System.Drawing.Point(111, 0);
         this.panelCantAudit2005.Name = "panelCantAudit2005";
         this.panelCantAudit2005.Size = new System.Drawing.Size(448, 336);
         this.panelCantAudit2005.TabIndex = 35;
         // 
         // label28
         // 
         this.label28.Location = new System.Drawing.Point(14, 24);
         this.label28.Name = "label28";
         this.label28.Size = new System.Drawing.Size(420, 28);
         this.label28.TabIndex = 15;
         this.label28.Text = "Current configuration doesn\'t allow auditing SQL Server 2005 instances";
         // 
         // label33
         // 
         this.label33.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.label33.Location = new System.Drawing.Point(14, 8);
         this.label33.Name = "label33";
         this.label33.Size = new System.Drawing.Size(384, 16);
         this.label33.TabIndex = 14;
         this.label33.Text = "Unsupported SQL Server Version";
         // 
         // panel9
         // 
         this.panel9.BackColor = System.Drawing.SystemColors.Control;
         this.panel9.Controls.Add(this.pictureBox3);
         this.panel9.Controls.Add(this.label34);
         this.panel9.Location = new System.Drawing.Point(0, 60);
         this.panel9.Name = "panel9";
         this.panel9.Size = new System.Drawing.Size(448, 276);
         this.panel9.TabIndex = 16;
         // 
         // pictureBox3
         // 
         this.pictureBox3.Image = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.StatusWarning_48;
         this.pictureBox3.Location = new System.Drawing.Point(52, 32);
         this.pictureBox3.Name = "pictureBox3";
         this.pictureBox3.Size = new System.Drawing.Size(48, 48);
         this.pictureBox3.TabIndex = 1;
         this.pictureBox3.TabStop = false;
         // 
         // label34
         // 
         this.label34.Location = new System.Drawing.Point(116, 24);
         this.label34.Name = "label34";
         this.label34.Size = new System.Drawing.Size(292, 84);
         this.label34.TabIndex = 0;
         this.label34.Text = resources.GetString("label34.Text");
         // 
         // panelCantConnect
         // 
         this.panelCantConnect.BackColor = System.Drawing.SystemColors.ControlLightLight;
         this.panelCantConnect.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
         this.panelCantConnect.Controls.Add(this.label35);
         this.panelCantConnect.Controls.Add(this.label36);
         this.panelCantConnect.Controls.Add(this.panel10);
         this.panelCantConnect.Location = new System.Drawing.Point(111, 0);
         this.panelCantConnect.Name = "panelCantConnect";
         this.panelCantConnect.Size = new System.Drawing.Size(448, 336);
         this.panelCantConnect.TabIndex = 36;
         // 
         // label35
         // 
         this.label35.Location = new System.Drawing.Point(14, 24);
         this.label35.Name = "label35";
         this.label35.Size = new System.Drawing.Size(420, 28);
         this.label35.TabIndex = 15;
         this.label35.Text = "Unable to connect to the specified SQL Server ";
         // 
         // label36
         // 
         this.label36.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.label36.Location = new System.Drawing.Point(14, 8);
         this.label36.Name = "label36";
         this.label36.Size = new System.Drawing.Size(384, 16);
         this.label36.TabIndex = 14;
         this.label36.Text = "Cannot connect to target SQL Server";
         // 
         // panel10
         // 
         this.panel10.BackColor = System.Drawing.SystemColors.Control;
         this.panel10.Controls.Add(this.pictureBox4);
         this.panel10.Controls.Add(this.label37);
         this.panel10.Location = new System.Drawing.Point(0, 60);
         this.panel10.Name = "panel10";
         this.panel10.Size = new System.Drawing.Size(448, 276);
         this.panel10.TabIndex = 16;
         // 
         // pictureBox4
         // 
         this.pictureBox4.Image = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.StatusWarning_48;
         this.pictureBox4.Location = new System.Drawing.Point(52, 32);
         this.pictureBox4.Name = "pictureBox4";
         this.pictureBox4.Size = new System.Drawing.Size(48, 48);
         this.pictureBox4.TabIndex = 1;
         this.pictureBox4.TabStop = false;
         // 
         // label37
         // 
         this.label37.Location = new System.Drawing.Point(116, 24);
         this.label37.Name = "label37";
         this.label37.Size = new System.Drawing.Size(300, 84);
         this.label37.TabIndex = 0;
         this.label37.Text = resources.GetString("label37.Text");
         // 
         // chkAuditUserCaptureTrans
         // 
         this.chkAuditUserCaptureTrans.AutoSize = true;
         this.chkAuditUserCaptureTrans.Location = new System.Drawing.Point(8, 170);
         this.chkAuditUserCaptureTrans.Name = "chkAuditUserCaptureTrans";
         this.chkAuditUserCaptureTrans.Size = new System.Drawing.Size(233, 17);
         this.chkAuditUserCaptureTrans.TabIndex = 12;
         this.chkAuditUserCaptureTrans.Text = "Capture Transaction Status for DML Activity";
         this.chkAuditUserCaptureTrans.UseVisualStyleBackColor = true;
         // 
         // Form_ServerNew
         // 
         this.AcceptButton = this.btnNext;
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.CancelButton = this.btnCancel;
         this.ClientSize = new System.Drawing.Size(558, 376);
         this.Controls.Add(this.panelAuditedUserActivity);
         this.Controls.Add(this.panelSummary);
         this.Controls.Add(this.panelCluster);
         this.Controls.Add(this.panelLeft);
         this.Controls.Add(this.panelButtons);
         this.Controls.Add(this.panelAudit);
         this.Controls.Add(this.panelAgentTrace);
         this.Controls.Add(this.panelAgentService);
         this.Controls.Add(this.panelServer);
         this.Controls.Add(this.panelCantAudit2005);
         this.Controls.Add(this.panelIncompatibleDatabase);
         this.Controls.Add(this.panelExistingDatabase);
         this.Controls.Add(this.panelCantConnect);
         this.Controls.Add(this.panelAgentDeploy);
         this.Controls.Add(this.panelAuditedUsers);
         this.Controls.Add(this.panelLicenseLimit);
         this.Controls.Add(this.panelPermissions);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.HelpButton = true;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "Form_ServerNew";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "New Registered SQL Server";
         this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.Form_ServerNew_HelpRequested);
         this.panelButtons.ResumeLayout(false);
         this.panelLeft.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
         this.panelServer.ResumeLayout(false);
         this.panelGeneralProperties.ResumeLayout(false);
         this.panelGeneralProperties.PerformLayout();
         this.panelAgentDeploy.ResumeLayout(false);
         this.panelAgentDeployProperties.ResumeLayout(false);
         this.panelAgentService.ResumeLayout(false);
         this.panelAgentProperties.ResumeLayout(false);
         this.groupBox2.ResumeLayout(false);
         this.groupBox2.PerformLayout();
         this.panelAudit.ResumeLayout(false);
         this.panelAudit.PerformLayout();
         this.panelAuditProperties.ResumeLayout(false);
         this.grpAuditResults.ResumeLayout(false);
         this.grpAuditActivity.ResumeLayout(false);
         this.panelAuditedUsers.ResumeLayout(false);
         this.panel2.ResumeLayout(false);
         this.panelAuditedUserActivity.ResumeLayout(false);
         this.panelAuditedUserActivity.PerformLayout();
         this.panel3.ResumeLayout(false);
         this.grpAuditUserActivity.ResumeLayout(false);
         this.grpAuditUserActivity.PerformLayout();
         this.panelSummary.ResumeLayout(false);
         this.panel4.ResumeLayout(false);
         this.panelAgentTrace.ResumeLayout(false);
         this.panel7.ResumeLayout(false);
         this.panel7.PerformLayout();
         this.panelLicenseLimit.ResumeLayout(false);
         this.panelLicenseProperties.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
         this.panelPermissions.ResumeLayout(false);
         this.panelPermissionsProperties.ResumeLayout(false);
         this.groupBox3.ResumeLayout(false);
         this.panelExistingDatabase.ResumeLayout(false);
         this.panel5.ResumeLayout(false);
         this.panelIncompatibleDatabase.ResumeLayout(false);
         this.panel6.ResumeLayout(false);
         this.panelCluster.ResumeLayout(false);
         this.panel8.ResumeLayout(false);
         this.panelCantAudit2005.ResumeLayout(false);
         this.panel9.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
         this.panelCantConnect.ResumeLayout(false);
         this.panel10.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
         this.ResumeLayout(false);

      }
      #endregion

      private System.Windows.Forms.Button btnCancel;
      private System.Windows.Forms.Button btnFinish;
      private System.Windows.Forms.Button btnNext;
      private System.Windows.Forms.Button btnBack;
      private System.Windows.Forms.Panel panelServer;
      private System.Windows.Forms.Panel panelAgentDeploy;
      private System.Windows.Forms.Panel panelAgentService;
      private System.Windows.Forms.Panel panelButtons;
      private System.Windows.Forms.Panel panelLeft;
      private System.Windows.Forms.Label lblGeneralDescription;
      private System.Windows.Forms.Label lblGeneralTitle;
      private System.Windows.Forms.Label labelAgentDeployDescription;
      private System.Windows.Forms.Label labelAgentDeployTitle;
      private System.Windows.Forms.Label labelAgentDescription;
      private System.Windows.Forms.Label labelAgentTitle;
      private System.Windows.Forms.Panel panelGeneralProperties;
      private System.Windows.Forms.Button btnBrowse;
      private System.Windows.Forms.TextBox textSQLServer;
      private System.Windows.Forms.Label labelSQLServer;
      private System.Windows.Forms.Panel panelAgentDeployProperties;
      private System.Windows.Forms.RadioButton radioButtonDeployLater;
      private System.Windows.Forms.RadioButton radioButtonDeployNow;
      private System.Windows.Forms.RadioButton radioButtonDeployManually;
      private System.Windows.Forms.Panel panelAgentProperties;
      private System.Windows.Forms.GroupBox groupBox2;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.TextBox textServicePasswordConfirm;
      private System.Windows.Forms.TextBox textServicePassword;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.TextBox textServiceAccount;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.PictureBox pictureBox1;
      private System.Windows.Forms.Label lblDescription;
      private System.Windows.Forms.TextBox textDescription;
      private System.Windows.Forms.Panel panelAudit;
      private System.Windows.Forms.Label lblAuditDescription;
      private System.Windows.Forms.Label lblAuditTitle;
      private System.Windows.Forms.Panel panelAuditProperties;
      private System.Windows.Forms.CheckBox chkAuditSecurity;
      private System.Windows.Forms.CheckBox chkAuditDDL;
      private System.Windows.Forms.Panel panelAuditedUsers;
      private System.Windows.Forms.Label label4;
      private System.Windows.Forms.Label label5;
      private System.Windows.Forms.Panel panel2;
      private System.Windows.Forms.Label label19;
      private System.Windows.Forms.Panel panelAuditedUserActivity;
      private System.Windows.Forms.Label label6;
      private System.Windows.Forms.Label label7;
      private System.Windows.Forms.Panel panel3;
      private System.Windows.Forms.GroupBox grpAuditActivity;
      private System.Windows.Forms.RadioButton rbAuditUserSelected;
      private System.Windows.Forms.RadioButton rbAuditUserAll;
      private System.Windows.Forms.CheckBox chkAuditUserCaptureSQL;
      private System.Windows.Forms.CheckBox chkAuditUserDML;
      private System.Windows.Forms.CheckBox chkAuditUserSELECT;
      private System.Windows.Forms.CheckBox chkAuditUserSecurity;
      private System.Windows.Forms.CheckBox chkAuditUserDDL;
      private System.Windows.Forms.CheckBox chkAuditUserFailedLogins;
      private System.Windows.Forms.CheckBox chkAuditUserLogins;
      private System.Windows.Forms.GroupBox grpAuditUserActivity;
      private System.Windows.Forms.CheckBox chkAuditFailedLogins;
      private System.Windows.Forms.CheckBox chkAuditLogins;
      private System.Windows.Forms.ListView lstPrivilegedUsers;
      private System.Windows.Forms.Button btnRemovePriv;
      private System.Windows.Forms.Button btnAddPriv;
      private System.Windows.Forms.ColumnHeader columnHeader1;
      private System.Windows.Forms.Panel panelSummary;
      private System.Windows.Forms.Label label8;
      private System.Windows.Forms.Label label9;
      private System.Windows.Forms.Panel panel4;
      private System.Windows.Forms.Label label10;
      private System.Windows.Forms.Label label11;
      private System.Windows.Forms.Label lblInstance;
      private System.Windows.Forms.Panel panelAgentTrace;
      private System.Windows.Forms.Panel panel7;
      private System.Windows.Forms.Label labeAgentTraceDescription;
      private System.Windows.Forms.Label labelAgentTraceTitle;
      private System.Windows.Forms.RadioButton radioDefaultTrace;
      private System.Windows.Forms.Label label12;
      private System.Windows.Forms.RadioButton radioSpecifyTrace;
      private System.Windows.Forms.TextBox txtTraceDirectory;
      private System.Windows.Forms.Label label13;
      private System.Windows.Forms.Panel panelLicenseLimit;
      private System.Windows.Forms.Label lblLicenseDescription;
      private System.Windows.Forms.Label lblLicenseTitle;
      private System.Windows.Forms.Panel panelLicenseProperties;
      private System.Windows.Forms.Label lblLicenseViolation;
      private System.Windows.Forms.PictureBox pictureBox2;
      private System.Windows.Forms.Label label15;
      private System.Windows.Forms.Label label26;
      private System.Windows.Forms.Label label31;
      private System.Windows.Forms.Panel panelPermissions;
      private System.Windows.Forms.Label lblPermissionsDescription;
      private System.Windows.Forms.Label labelPermissionsTitle;
      private System.Windows.Forms.Panel panelPermissionsProperties;
      private System.Windows.Forms.GroupBox groupBox3;
      private System.Windows.Forms.Label label16;
      private System.Windows.Forms.RadioButton radioDeny;
      private System.Windows.Forms.RadioButton radioGrantAll;
      private System.Windows.Forms.RadioButton radioGrantEventsOnly;
      private System.Windows.Forms.Panel panel5;
      private System.Windows.Forms.Label label17;
      private System.Windows.Forms.Label label18;
      private System.Windows.Forms.Label label20;
      private System.Windows.Forms.Panel panelExistingDatabase;
      private System.Windows.Forms.RadioButton radioDeleteDatabase;
      private System.Windows.Forms.RadioButton radioMaintainDatabase;
      private System.Windows.Forms.Panel panelIncompatibleDatabase;
      private System.Windows.Forms.Label label22;
      private System.Windows.Forms.Label label23;
      private System.Windows.Forms.Label label24;
      private System.Windows.Forms.Label label21;
      private System.Windows.Forms.RadioButton radioIncompatibleOverwrite;
      private System.Windows.Forms.RadioButton radioIncompatibleCancel;
      private System.Windows.Forms.Panel panel6;
      private System.Windows.Forms.Label textDatabaseName;
      private System.Windows.Forms.Label textExistingDatabase;
      private System.Windows.Forms.Label label27;
      private System.Windows.Forms.Label label25;
      private System.Windows.Forms.RadioButton radioButtonAlreadyDeployed;
      private System.Windows.Forms.Panel panelCluster;
      private System.Windows.Forms.Panel panel8;
      private System.Windows.Forms.Label label29;
      private System.Windows.Forms.Label label30;
      private System.Windows.Forms.Label label32;
      private System.Windows.Forms.CheckBox checkVirtualServer;
      private System.Windows.Forms.Label label14;
      private System.Windows.Forms.CheckBox chkAuditUserAdmin;
      private System.Windows.Forms.CheckBox chkUserAccessCheckFilter;
      private System.Windows.Forms.CheckBox chkAuditAdmin;
      private System.Windows.Forms.GroupBox grpAuditResults;
      private System.Windows.Forms.RadioButton rbAuditFailedOnly;
      private System.Windows.Forms.RadioButton rbAuditSuccessfulOnly;
      private System.Windows.Forms.Panel panelCantAudit2005;
      private System.Windows.Forms.Label label28;
      private System.Windows.Forms.Label label33;
      private System.Windows.Forms.Panel panel9;
      private System.Windows.Forms.PictureBox pictureBox3;
      private System.Windows.Forms.Label label34;
      private System.Windows.Forms.Panel panelCantConnect;
      private System.Windows.Forms.Label label35;
      private System.Windows.Forms.Label label36;
      private System.Windows.Forms.Panel panel10;
      private System.Windows.Forms.PictureBox pictureBox4;
      private System.Windows.Forms.Label label37;
      private System.Windows.Forms.CheckBox _cbAccessCheckFilter;
      private System.Windows.Forms.RadioButton _rbUserAuditFailed;
      private System.Windows.Forms.RadioButton _rbUserAuditPassed;
      private System.Windows.Forms.CheckBox chkAuditUDE;
      private System.Windows.Forms.CheckBox chkAuditUserUDE;
      private Infragistics.Win.FormattedLinkLabel.UltraFormattedLinkLabel _flblVirtualServerInfo;
      private System.Windows.Forms.LinkLabel linkLblHelpBestPractices;
      private System.Windows.Forms.LinkLabel linklblHelpBestPractices2;
      private System.Windows.Forms.CheckBox chkAuditUserCaptureTrans;
	}
}