namespace Idera.SQLcompliance.Application.GUI.Forms
{
   partial class Form_AgentProperties
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_AgentProperties));
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tabProperties = new System.Windows.Forms.TabControl();
            this.tabPageGeneral = new System.Windows.Forms.TabPage();
            this.grpAuditStatus = new System.Windows.Forms.GroupBox();
            this.textCurrentAuditLevel = new System.Windows.Forms.TextBox();
            this.textAgentAuditLevel = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.txtTimeLastUpdate = new System.Windows.Forms.TextBox();
            this.lblLastConfigUpdate = new System.Windows.Forms.Label();
            this.txtAuditSettingsStatus = new System.Windows.Forms.TextBox();
            this.lblAuditSettings = new System.Windows.Forms.Label();
            this.btnUpdateAuditSettings = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboLogLevel = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textAgentStatus = new System.Windows.Forms.TextBox();
            this.txtTimeLastHeartbeat = new Idera.SQLcompliance.Application.GUI.Controls.NumericTextBox();
            this.label25 = new System.Windows.Forms.Label();
            this.textHeartbeatFrequency = new Idera.SQLcompliance.Application.GUI.Controls.NumericTextBox();
            this.textAgentPort = new Idera.SQLcompliance.Application.GUI.Controls.NumericTextBox();
            this.textAgentVersion = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.txtComputer = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPageDeployment = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioManuallyDeployed = new System.Windows.Forms.RadioButton();
            this.radioAutoDeploy = new System.Windows.Forms.RadioButton();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.textServiceAccount = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.tabPageServers = new System.Windows.Forms.TabPage();
            this.listServers = new System.Windows.Forms.ListView();
            this.columnServer = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label2 = new System.Windows.Forms.Label();
            this.tabPageTrace = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.radioFilesUncompression = new System.Windows.Forms.RadioButton();
            this.radioFilesCompression = new System.Windows.Forms.RadioButton();
            this._gbTamper = new System.Windows.Forms.GroupBox();
            this._lblTamper = new System.Windows.Forms.Label();
            this._tbTamperInterval = new Idera.SQLcompliance.Application.GUI.Controls.NumericTextBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.textAgentTraceDirectory = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textTraceStartTimeout = new Idera.SQLcompliance.Application.GUI.Controls.NumericTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textForcedCollectionInterval = new Idera.SQLcompliance.Application.GUI.Controls.NumericTextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.textCollectionInterval = new Idera.SQLcompliance.Application.GUI.Controls.NumericTextBox();
            this.textTraceFileRolloverSize = new Idera.SQLcompliance.Application.GUI.Controls.NumericTextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupTimeLimit = new System.Windows.Forms.GroupBox();
            this.textMaxUnattendedTime = new Idera.SQLcompliance.Application.GUI.Controls.NumericTextBox();
            this.label29 = new System.Windows.Forms.Label();
            this.radioTimeLimit = new System.Windows.Forms.RadioButton();
            this.radioUnlimitedTime = new System.Windows.Forms.RadioButton();
            this.groupSizeLimit = new System.Windows.Forms.GroupBox();
            this.label28 = new System.Windows.Forms.Label();
            this.textMaxFolderSize = new Idera.SQLcompliance.Application.GUI.Controls.NumericTextBox();
            this.radioDirSizeLimit = new System.Windows.Forms.RadioButton();
            this.radioDirSizeUnlimited = new System.Windows.Forms.RadioButton();
            this.menuDatabaseAdd = new System.Windows.Forms.MenuItem();
            this.menuDatabaseRemove = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.menuDatabaseEnable = new System.Windows.Forms.MenuItem();
            this.menuDatabaseDisable = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuDatabaseProperties = new System.Windows.Forms.MenuItem();
            this.tabProperties.SuspendLayout();
            this.tabPageGeneral.SuspendLayout();
            this.grpAuditStatus.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPageDeployment.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.tabPageServers.SuspendLayout();
            this.tabPageTrace.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this._gbTamper.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupTimeLimit.SuspendLayout();
            this.groupSizeLimit.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(380, 396);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "&OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(460, 396);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // tabProperties
            // 
            this.tabProperties.Controls.Add(this.tabPageGeneral);
            this.tabProperties.Controls.Add(this.tabPageDeployment);
            this.tabProperties.Controls.Add(this.tabPageServers);
            this.tabProperties.Controls.Add(this.tabPageTrace);
            this.tabProperties.Location = new System.Drawing.Point(0, 0);
            this.tabProperties.Name = "tabProperties";
            this.tabProperties.SelectedIndex = 0;
            this.tabProperties.Size = new System.Drawing.Size(540, 388);
            this.tabProperties.TabIndex = 0;
            // 
            // tabPageGeneral
            // 
            this.tabPageGeneral.Controls.Add(this.grpAuditStatus);
            this.tabPageGeneral.Controls.Add(this.groupBox1);
            this.tabPageGeneral.Controls.Add(this.txtComputer);
            this.tabPageGeneral.Controls.Add(this.label1);
            this.tabPageGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabPageGeneral.Name = "tabPageGeneral";
            this.tabPageGeneral.Size = new System.Drawing.Size(532, 362);
            this.tabPageGeneral.TabIndex = 0;
            this.tabPageGeneral.Text = "General";
            this.tabPageGeneral.UseVisualStyleBackColor = true;
            // 
            // grpAuditStatus
            // 
            this.grpAuditStatus.Controls.Add(this.textCurrentAuditLevel);
            this.grpAuditStatus.Controls.Add(this.textAgentAuditLevel);
            this.grpAuditStatus.Controls.Add(this.label16);
            this.grpAuditStatus.Controls.Add(this.label17);
            this.grpAuditStatus.Controls.Add(this.txtTimeLastUpdate);
            this.grpAuditStatus.Controls.Add(this.lblLastConfigUpdate);
            this.grpAuditStatus.Controls.Add(this.txtAuditSettingsStatus);
            this.grpAuditStatus.Controls.Add(this.lblAuditSettings);
            this.grpAuditStatus.Controls.Add(this.btnUpdateAuditSettings);
            this.grpAuditStatus.Location = new System.Drawing.Point(12, 136);
            this.grpAuditStatus.Name = "grpAuditStatus";
            this.grpAuditStatus.Size = new System.Drawing.Size(508, 112);
            this.grpAuditStatus.TabIndex = 3;
            this.grpAuditStatus.TabStop = false;
            this.grpAuditStatus.Text = "Audit Settings";
            // 
            // textCurrentAuditLevel
            // 
            this.textCurrentAuditLevel.Location = new System.Drawing.Point(404, 44);
            this.textCurrentAuditLevel.Name = "textCurrentAuditLevel";
            this.textCurrentAuditLevel.ReadOnly = true;
            this.textCurrentAuditLevel.Size = new System.Drawing.Size(92, 20);
            this.textCurrentAuditLevel.TabIndex = 7;
            this.textCurrentAuditLevel.TabStop = false;
            this.textCurrentAuditLevel.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textAgentAuditLevel
            // 
            this.textAgentAuditLevel.Location = new System.Drawing.Point(404, 20);
            this.textAgentAuditLevel.Name = "textAgentAuditLevel";
            this.textAgentAuditLevel.ReadOnly = true;
            this.textAgentAuditLevel.Size = new System.Drawing.Size(92, 20);
            this.textAgentAuditLevel.TabIndex = 3;
            this.textAgentAuditLevel.TabStop = false;
            this.textAgentAuditLevel.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label16
            // 
            this.label16.Location = new System.Drawing.Point(256, 48);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(152, 16);
            this.label16.TabIndex = 6;
            this.label16.Text = "Current audit settings level:";
            // 
            // label17
            // 
            this.label17.Location = new System.Drawing.Point(256, 24);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(152, 16);
            this.label17.TabIndex = 2;
            this.label17.Text = "Audit settings level at agent:";
            // 
            // txtTimeLastUpdate
            // 
            this.txtTimeLastUpdate.Location = new System.Drawing.Point(120, 16);
            this.txtTimeLastUpdate.Name = "txtTimeLastUpdate";
            this.txtTimeLastUpdate.ReadOnly = true;
            this.txtTimeLastUpdate.Size = new System.Drawing.Size(120, 20);
            this.txtTimeLastUpdate.TabIndex = 1;
            this.txtTimeLastUpdate.TabStop = false;
            this.txtTimeLastUpdate.Text = "Never";
            // 
            // lblLastConfigUpdate
            // 
            this.lblLastConfigUpdate.Location = new System.Drawing.Point(8, 20);
            this.lblLastConfigUpdate.Name = "lblLastConfigUpdate";
            this.lblLastConfigUpdate.Size = new System.Drawing.Size(112, 16);
            this.lblLastConfigUpdate.TabIndex = 0;
            this.lblLastConfigUpdate.Text = "Last agent &update:";
            // 
            // txtAuditSettingsStatus
            // 
            this.txtAuditSettingsStatus.Location = new System.Drawing.Point(120, 44);
            this.txtAuditSettingsStatus.Name = "txtAuditSettingsStatus";
            this.txtAuditSettingsStatus.ReadOnly = true;
            this.txtAuditSettingsStatus.Size = new System.Drawing.Size(120, 20);
            this.txtAuditSettingsStatus.TabIndex = 5;
            this.txtAuditSettingsStatus.TabStop = false;
            this.txtAuditSettingsStatus.Text = "Update pending";
            // 
            // lblAuditSettings
            // 
            this.lblAuditSettings.Location = new System.Drawing.Point(8, 48);
            this.lblAuditSettings.Name = "lblAuditSettings";
            this.lblAuditSettings.Size = new System.Drawing.Size(112, 16);
            this.lblAuditSettings.TabIndex = 4;
            this.lblAuditSettings.Text = "Aud&it settings status:";
            // 
            // btnUpdateAuditSettings
            // 
            this.btnUpdateAuditSettings.Enabled = false;
            this.btnUpdateAuditSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUpdateAuditSettings.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnUpdateAuditSettings.Location = new System.Drawing.Point(190, 84);
            this.btnUpdateAuditSettings.Name = "btnUpdateAuditSettings";
            this.btnUpdateAuditSettings.Size = new System.Drawing.Size(120, 20);
            this.btnUpdateAuditSettings.TabIndex = 8;
            this.btnUpdateAuditSettings.Text = "Update &now";
            this.btnUpdateAuditSettings.Click += new System.EventHandler(this.btnUpdateAuditSettings_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboLogLevel);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.textAgentStatus);
            this.groupBox1.Controls.Add(this.txtTimeLastHeartbeat);
            this.groupBox1.Controls.Add(this.label25);
            this.groupBox1.Controls.Add(this.textHeartbeatFrequency);
            this.groupBox1.Controls.Add(this.textAgentPort);
            this.groupBox1.Controls.Add(this.textAgentVersion);
            this.groupBox1.Controls.Add(this.label24);
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Controls.Add(this.label27);
            this.groupBox1.Controls.Add(this.label33);
            this.groupBox1.Location = new System.Drawing.Point(12, 32);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(508, 96);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Agent Settings";
            // 
            // comboLogLevel
            // 
            this.comboLogLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboLogLevel.Items.AddRange(new object[] {
            "Silent",
            "Normal",
            "Verbose",
            "Debug"});
            this.comboLogLevel.Location = new System.Drawing.Point(388, 64);
            this.comboLogLevel.Name = "comboLogLevel";
            this.comboLogLevel.Size = new System.Drawing.Size(112, 21);
            this.comboLogLevel.TabIndex = 11;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(12, 20);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(76, 16);
            this.label5.TabIndex = 0;
            this.label5.Text = "Agent status:";
            // 
            // textAgentStatus
            // 
            this.textAgentStatus.Location = new System.Drawing.Point(100, 16);
            this.textAgentStatus.Name = "textAgentStatus";
            this.textAgentStatus.ReadOnly = true;
            this.textAgentStatus.Size = new System.Drawing.Size(124, 20);
            this.textAgentStatus.TabIndex = 1;
            this.textAgentStatus.TabStop = false;
            this.textAgentStatus.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtTimeLastHeartbeat
            // 
            this.txtTimeLastHeartbeat.Location = new System.Drawing.Point(386, 16);
            this.txtTimeLastHeartbeat.Name = "txtTimeLastHeartbeat";
            this.txtTimeLastHeartbeat.ReadOnly = true;
            this.txtTimeLastHeartbeat.Size = new System.Drawing.Size(112, 20);
            this.txtTimeLastHeartbeat.TabIndex = 3;
            this.txtTimeLastHeartbeat.TabStop = false;
            this.txtTimeLastHeartbeat.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label25
            // 
            this.label25.Location = new System.Drawing.Point(256, 20);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(88, 16);
            this.label25.TabIndex = 2;
            this.label25.Text = "Last heartbeat:";
            // 
            // textHeartbeatFrequency
            // 
            this.textHeartbeatFrequency.Location = new System.Drawing.Point(386, 40);
            this.textHeartbeatFrequency.MaxLength = 4;
            this.textHeartbeatFrequency.Name = "textHeartbeatFrequency";
            this.textHeartbeatFrequency.Size = new System.Drawing.Size(112, 20);
            this.textHeartbeatFrequency.TabIndex = 7;
            this.textHeartbeatFrequency.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textAgentPort
            // 
            this.textAgentPort.Location = new System.Drawing.Point(100, 64);
            this.textAgentPort.Name = "textAgentPort";
            this.textAgentPort.ReadOnly = true;
            this.textAgentPort.Size = new System.Drawing.Size(124, 20);
            this.textAgentPort.TabIndex = 9;
            this.textAgentPort.TabStop = false;
            this.textAgentPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textAgentVersion
            // 
            this.textAgentVersion.Location = new System.Drawing.Point(100, 40);
            this.textAgentVersion.Name = "textAgentVersion";
            this.textAgentVersion.ReadOnly = true;
            this.textAgentVersion.Size = new System.Drawing.Size(124, 20);
            this.textAgentVersion.TabIndex = 5;
            this.textAgentVersion.TabStop = false;
            this.textAgentVersion.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label24
            // 
            this.label24.Location = new System.Drawing.Point(12, 44);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(80, 16);
            this.label24.TabIndex = 4;
            this.label24.Text = "Agent version:";
            // 
            // label15
            // 
            this.label15.Location = new System.Drawing.Point(12, 68);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(84, 16);
            this.label15.TabIndex = 8;
            this.label15.Text = "Agent port:";
            // 
            // label27
            // 
            this.label27.Location = new System.Drawing.Point(256, 68);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(88, 16);
            this.label27.TabIndex = 10;
            this.label27.Text = "&Logging level:";
            // 
            // label33
            // 
            this.label33.Location = new System.Drawing.Point(256, 44);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(128, 16);
            this.label33.TabIndex = 6;
            this.label33.Text = "&Heartbeat interval (min):";
            // 
            // txtComputer
            // 
            this.txtComputer.Location = new System.Drawing.Point(184, 8);
            this.txtComputer.Name = "txtComputer";
            this.txtComputer.ReadOnly = true;
            this.txtComputer.Size = new System.Drawing.Size(336, 20);
            this.txtComputer.TabIndex = 1;
            this.txtComputer.TabStop = false;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(176, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "SQLcompliance Agent Computer:";
            // 
            // tabPageDeployment
            // 
            this.tabPageDeployment.Controls.Add(this.groupBox2);
            this.tabPageDeployment.Controls.Add(this.groupBox5);
            this.tabPageDeployment.Location = new System.Drawing.Point(4, 22);
            this.tabPageDeployment.Name = "tabPageDeployment";
            this.tabPageDeployment.Size = new System.Drawing.Size(532, 362);
            this.tabPageDeployment.TabIndex = 1;
            this.tabPageDeployment.Text = "Deployment";
            this.tabPageDeployment.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioManuallyDeployed);
            this.groupBox2.Controls.Add(this.radioAutoDeploy);
            this.groupBox2.Location = new System.Drawing.Point(12, 68);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(508, 120);
            this.groupBox2.TabIndex = 80;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "SQLcompliance Agent Deployment";
            // 
            // radioManuallyDeployed
            // 
            this.radioManuallyDeployed.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.radioManuallyDeployed.Enabled = false;
            this.radioManuallyDeployed.Location = new System.Drawing.Point(12, 60);
            this.radioManuallyDeployed.Name = "radioManuallyDeployed";
            this.radioManuallyDeployed.Size = new System.Drawing.Size(476, 56);
            this.radioManuallyDeployed.TabIndex = 1;
            this.radioManuallyDeployed.Text = resources.GetString("radioManuallyDeployed.Text");
            this.radioManuallyDeployed.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            // 
            // radioAutoDeploy
            // 
            this.radioAutoDeploy.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.radioAutoDeploy.Enabled = false;
            this.radioAutoDeploy.Location = new System.Drawing.Point(12, 20);
            this.radioAutoDeploy.Name = "radioAutoDeploy";
            this.radioAutoDeploy.Size = new System.Drawing.Size(484, 34);
            this.radioAutoDeploy.TabIndex = 0;
            this.radioAutoDeploy.Text = "Automatic Deployment - The SQLcompliance Agent for this instance is installed/uni" +
    "nstalled from the SQL Compliance Manager Console.";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.textServiceAccount);
            this.groupBox5.Controls.Add(this.label9);
            this.groupBox5.Location = new System.Drawing.Point(12, 8);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(508, 52);
            this.groupBox5.TabIndex = 79;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "SQLcompliance Agent Service:";
            // 
            // textServiceAccount
            // 
            this.textServiceAccount.Location = new System.Drawing.Point(106, 19);
            this.textServiceAccount.Name = "textServiceAccount";
            this.textServiceAccount.ReadOnly = true;
            this.textServiceAccount.Size = new System.Drawing.Size(394, 20);
            this.textServiceAccount.TabIndex = 75;
            this.textServiceAccount.TabStop = false;
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(12, 21);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(88, 17);
            this.label9.TabIndex = 74;
            this.label9.Text = "&Service account:";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabPageServers
            // 
            this.tabPageServers.Controls.Add(this.listServers);
            this.tabPageServers.Controls.Add(this.label2);
            this.tabPageServers.Location = new System.Drawing.Point(4, 22);
            this.tabPageServers.Name = "tabPageServers";
            this.tabPageServers.Size = new System.Drawing.Size(532, 362);
            this.tabPageServers.TabIndex = 9;
            this.tabPageServers.Text = "SQL Servers";
            this.tabPageServers.UseVisualStyleBackColor = true;
            // 
            // listServers
            // 
            this.listServers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnServer,
            this.columnDescription});
            this.listServers.HideSelection = false;
            this.listServers.Location = new System.Drawing.Point(16, 44);
            this.listServers.Name = "listServers";
            this.listServers.Size = new System.Drawing.Size(504, 300);
            this.listServers.TabIndex = 1;
            this.listServers.UseCompatibleStateImageBehavior = false;
            this.listServers.View = System.Windows.Forms.View.Details;
            // 
            // columnServer
            // 
            this.columnServer.Text = "SQL Server";
            this.columnServer.Width = 200;
            // 
            // columnDescription
            // 
            this.columnDescription.Text = "Description";
            this.columnDescription.Width = 275;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(12, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(500, 28);
            this.label2.TabIndex = 0;
            this.label2.Text = "This SQLcompliance Agent is responsible for gathering and sending collected audit" +
    " data for the following registered SQL Servers:";
            // 
            // tabPageTrace
            // 
            this.tabPageTrace.Controls.Add(this.groupBox4);
            this.tabPageTrace.Controls.Add(this._gbTamper);
            this.tabPageTrace.Controls.Add(this.groupBox7);
            this.tabPageTrace.Controls.Add(this.groupBox3);
            this.tabPageTrace.Controls.Add(this.groupTimeLimit);
            this.tabPageTrace.Controls.Add(this.groupSizeLimit);
            this.tabPageTrace.Location = new System.Drawing.Point(4, 22);
            this.tabPageTrace.Name = "tabPageTrace";
            this.tabPageTrace.Size = new System.Drawing.Size(532, 362);
            this.tabPageTrace.TabIndex = 8;
            this.tabPageTrace.Text = "Trace Options";
            this.tabPageTrace.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.radioFilesUncompression);
            this.groupBox4.Controls.Add(this.radioFilesCompression);
            this.groupBox4.Location = new System.Drawing.Point(280, 76);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(242, 125);
            this.groupBox4.TabIndex = 5;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Trace Compression Options:";
            // 
            // radioFilesUncompression
            // 
            this.radioFilesUncompression.Location = new System.Drawing.Point(12, 44);
            this.radioFilesUncompression.Name = "radioFilesUncompression";
            this.radioFilesUncompression.Size = new System.Drawing.Size(220, 24);
            this.radioFilesUncompression.TabIndex = 3;
            this.radioFilesUncompression.TabStop = true;
            this.radioFilesUncompression.Text = "Do not compress CM events files.";
            this.radioFilesUncompression.CheckedChanged += new System.EventHandler(this.radioFilesUncompression_CheckedChanged);
            // 
            // radioFilesCompression
            // 
            this.radioFilesCompression.Checked = true;
            this.radioFilesCompression.Location = new System.Drawing.Point(12, 20);
            this.radioFilesCompression.Name = "radioFilesCompression";
            this.radioFilesCompression.Size = new System.Drawing.Size(188, 24);
            this.radioFilesCompression.TabIndex = 2;
            this.radioFilesCompression.TabStop = true;
            this.radioFilesCompression.Text = "Compress CM events files.";
            this.radioFilesCompression.CheckedChanged += new System.EventHandler(this.radioFilesCompression_CheckedChanged);
            // 
            // _gbTamper
            // 
            this._gbTamper.Controls.Add(this._lblTamper);
            this._gbTamper.Controls.Add(this._tbTamperInterval);
            this._gbTamper.Location = new System.Drawing.Point(10, 207);
            this._gbTamper.Name = "_gbTamper";
            this._gbTamper.Size = new System.Drawing.Size(512, 44);
            this._gbTamper.TabIndex = 2;
            this._gbTamper.TabStop = false;
            this._gbTamper.Text = "Trace Tamper Detection Options";
            // 
            // _lblTamper
            // 
            this._lblTamper.Location = new System.Drawing.Point(12, 20);
            this._lblTamper.Name = "_lblTamper";
            this._lblTamper.Size = new System.Drawing.Size(164, 16);
            this._lblTamper.TabIndex = 0;
            this._lblTamper.Text = "Tamper detection interval (sec):";
            this._lblTamper.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _tbTamperInterval
            // 
            this._tbTamperInterval.Location = new System.Drawing.Point(176, 16);
            this._tbTamperInterval.MaxLength = 4;
            this._tbTamperInterval.Name = "_tbTamperInterval";
            this._tbTamperInterval.Size = new System.Drawing.Size(76, 20);
            this._tbTamperInterval.TabIndex = 1;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.textAgentTraceDirectory);
            this.groupBox7.Controls.Add(this.label14);
            this.groupBox7.Location = new System.Drawing.Point(8, 12);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(512, 56);
            this.groupBox7.TabIndex = 0;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "SQLcompliance Agent Trace Directory";
            // 
            // textAgentTraceDirectory
            // 
            this.textAgentTraceDirectory.Location = new System.Drawing.Point(104, 20);
            this.textAgentTraceDirectory.Name = "textAgentTraceDirectory";
            this.textAgentTraceDirectory.ReadOnly = true;
            this.textAgentTraceDirectory.Size = new System.Drawing.Size(400, 20);
            this.textAgentTraceDirectory.TabIndex = 1;
            this.textAgentTraceDirectory.TabStop = false;
            // 
            // label14
            // 
            this.label14.Location = new System.Drawing.Point(12, 24);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(88, 16);
            this.label14.TabIndex = 0;
            this.label14.Text = "Trace directory:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.textTraceStartTimeout);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.textForcedCollectionInterval);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.textCollectionInterval);
            this.groupBox3.Controls.Add(this.textTraceFileRolloverSize);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Location = new System.Drawing.Point(8, 76);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(260, 125);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Trace Collection Options:";
            // 
            // textTraceStartTimeout
            // 
            this.textTraceStartTimeout.Location = new System.Drawing.Point(176, 89);
            this.textTraceStartTimeout.MaxLength = 4;
            this.textTraceStartTimeout.Name = "textTraceStartTimeout";
            this.textTraceStartTimeout.Size = new System.Drawing.Size(76, 20);
            this.textTraceStartTimeout.TabIndex = 7;
            this.textTraceStartTimeout.Text = "30";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 92);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(130, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Trace Start Timeout (sec):";
            // 
            // textForcedCollectionInterval
            // 
            this.textForcedCollectionInterval.Location = new System.Drawing.Point(176, 64);
            this.textForcedCollectionInterval.MaxLength = 4;
            this.textForcedCollectionInterval.Name = "textForcedCollectionInterval";
            this.textForcedCollectionInterval.Size = new System.Drawing.Size(76, 20);
            this.textForcedCollectionInterval.TabIndex = 5;
            this.textForcedCollectionInterval.Text = "3";
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(12, 68);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(160, 12);
            this.label10.TabIndex = 4;
            this.label10.Text = "Force collection interval (min):";
            // 
            // textCollectionInterval
            // 
            this.textCollectionInterval.Location = new System.Drawing.Point(176, 40);
            this.textCollectionInterval.MaxLength = 4;
            this.textCollectionInterval.Name = "textCollectionInterval";
            this.textCollectionInterval.Size = new System.Drawing.Size(76, 20);
            this.textCollectionInterval.TabIndex = 3;
            // 
            // textTraceFileRolloverSize
            // 
            this.textTraceFileRolloverSize.Location = new System.Drawing.Point(176, 16);
            this.textTraceFileRolloverSize.MaxLength = 2;
            this.textTraceFileRolloverSize.Name = "textTraceFileRolloverSize";
            this.textTraceFileRolloverSize.Size = new System.Drawing.Size(76, 20);
            this.textTraceFileRolloverSize.TabIndex = 1;
            this.textTraceFileRolloverSize.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textTraceFileRolloverSize_KeyPress);
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(12, 20);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(156, 16);
            this.label6.TabIndex = 0;
            this.label6.Text = "Trace file rollover size (MB):";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(12, 44);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(128, 12);
            this.label4.TabIndex = 2;
            this.label4.Text = "Collection interval (min):";
            // 
            // groupTimeLimit
            // 
            this.groupTimeLimit.Controls.Add(this.textMaxUnattendedTime);
            this.groupTimeLimit.Controls.Add(this.label29);
            this.groupTimeLimit.Controls.Add(this.radioTimeLimit);
            this.groupTimeLimit.Controls.Add(this.radioUnlimitedTime);
            this.groupTimeLimit.Location = new System.Drawing.Point(280, 259);
            this.groupTimeLimit.Name = "groupTimeLimit";
            this.groupTimeLimit.Size = new System.Drawing.Size(240, 72);
            this.groupTimeLimit.TabIndex = 4;
            this.groupTimeLimit.TabStop = false;
            this.groupTimeLimit.Text = "Unattended Auditing Time Limit";
            // 
            // textMaxUnattendedTime
            // 
            this.textMaxUnattendedTime.Location = new System.Drawing.Point(176, 40);
            this.textMaxUnattendedTime.MaxLength = 3;
            this.textMaxUnattendedTime.Name = "textMaxUnattendedTime";
            this.textMaxUnattendedTime.Size = new System.Drawing.Size(24, 20);
            this.textMaxUnattendedTime.TabIndex = 2;
            this.textMaxUnattendedTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label29
            // 
            this.label29.Location = new System.Drawing.Point(204, 44);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(32, 16);
            this.label29.TabIndex = 3;
            this.label29.Text = "days";
            // 
            // radioTimeLimit
            // 
            this.radioTimeLimit.Location = new System.Drawing.Point(12, 40);
            this.radioTimeLimit.Name = "radioTimeLimit";
            this.radioTimeLimit.Size = new System.Drawing.Size(168, 24);
            this.radioTimeLimit.TabIndex = 1;
            this.radioTimeLimit.TabStop = true;
            this.radioTimeLimit.Text = "Limit unattended auditing to ";
            this.radioTimeLimit.CheckedChanged += new System.EventHandler(this.radioUnlimitedTime_CheckedChanged);
            // 
            // radioUnlimitedTime
            // 
            this.radioUnlimitedTime.Checked = true;
            this.radioUnlimitedTime.Location = new System.Drawing.Point(12, 16);
            this.radioUnlimitedTime.Name = "radioUnlimitedTime";
            this.radioUnlimitedTime.Size = new System.Drawing.Size(104, 24);
            this.radioUnlimitedTime.TabIndex = 0;
            this.radioUnlimitedTime.TabStop = true;
            this.radioUnlimitedTime.Text = "Unlimited";
            this.radioUnlimitedTime.CheckedChanged += new System.EventHandler(this.radioUnlimitedTime_CheckedChanged);
            // 
            // groupSizeLimit
            // 
            this.groupSizeLimit.Controls.Add(this.label28);
            this.groupSizeLimit.Controls.Add(this.textMaxFolderSize);
            this.groupSizeLimit.Controls.Add(this.radioDirSizeLimit);
            this.groupSizeLimit.Controls.Add(this.radioDirSizeUnlimited);
            this.groupSizeLimit.Location = new System.Drawing.Point(8, 259);
            this.groupSizeLimit.Name = "groupSizeLimit";
            this.groupSizeLimit.Size = new System.Drawing.Size(260, 72);
            this.groupSizeLimit.TabIndex = 3;
            this.groupSizeLimit.TabStop = false;
            this.groupSizeLimit.Text = "Trace Directory Size Limit";
            // 
            // label28
            // 
            this.label28.Location = new System.Drawing.Point(200, 44);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(28, 16);
            this.label28.TabIndex = 3;
            this.label28.Text = "GB";
            // 
            // textMaxFolderSize
            // 
            this.textMaxFolderSize.Location = new System.Drawing.Point(148, 40);
            this.textMaxFolderSize.MaxLength = 4;
            this.textMaxFolderSize.Name = "textMaxFolderSize";
            this.textMaxFolderSize.Size = new System.Drawing.Size(48, 20);
            this.textMaxFolderSize.TabIndex = 2;
            this.textMaxFolderSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // radioDirSizeLimit
            // 
            this.radioDirSizeLimit.Location = new System.Drawing.Point(12, 40);
            this.radioDirSizeLimit.Name = "radioDirSizeLimit";
            this.radioDirSizeLimit.Size = new System.Drawing.Size(136, 24);
            this.radioDirSizeLimit.TabIndex = 1;
            this.radioDirSizeLimit.TabStop = true;
            this.radioDirSizeLimit.Text = "Limit trace directory to";
            this.radioDirSizeLimit.CheckedChanged += new System.EventHandler(this.radioDirSizeUnlimited_CheckedChanged);
            // 
            // radioDirSizeUnlimited
            // 
            this.radioDirSizeUnlimited.Checked = true;
            this.radioDirSizeUnlimited.Location = new System.Drawing.Point(12, 16);
            this.radioDirSizeUnlimited.Name = "radioDirSizeUnlimited";
            this.radioDirSizeUnlimited.Size = new System.Drawing.Size(104, 24);
            this.radioDirSizeUnlimited.TabIndex = 0;
            this.radioDirSizeUnlimited.TabStop = true;
            this.radioDirSizeUnlimited.Text = "Unlimited";
            this.radioDirSizeUnlimited.CheckedChanged += new System.EventHandler(this.radioDirSizeUnlimited_CheckedChanged);
            // 
            // menuDatabaseAdd
            // 
            this.menuDatabaseAdd.Index = -1;
            this.menuDatabaseAdd.Text = "";
            // 
            // menuDatabaseRemove
            // 
            this.menuDatabaseRemove.Index = -1;
            this.menuDatabaseRemove.Text = "";
            // 
            // menuItem3
            // 
            this.menuItem3.Index = -1;
            this.menuItem3.Text = "";
            // 
            // menuDatabaseEnable
            // 
            this.menuDatabaseEnable.Index = -1;
            this.menuDatabaseEnable.Text = "";
            // 
            // menuDatabaseDisable
            // 
            this.menuDatabaseDisable.Index = -1;
            this.menuDatabaseDisable.Text = "";
            // 
            // menuItem1
            // 
            this.menuItem1.Index = -1;
            this.menuItem1.Text = "";
            // 
            // menuDatabaseProperties
            // 
            this.menuDatabaseProperties.Index = -1;
            this.menuDatabaseProperties.Text = "";
            // 
            // Form_AgentProperties
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(542, 428);
            this.Controls.Add(this.tabProperties);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form_AgentProperties";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SQLcompliance Agent Properties";
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.Form_AgentProperties_HelpRequested);
            this.tabProperties.ResumeLayout(false);
            this.tabPageGeneral.ResumeLayout(false);
            this.tabPageGeneral.PerformLayout();
            this.grpAuditStatus.ResumeLayout(false);
            this.grpAuditStatus.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPageDeployment.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.tabPageServers.ResumeLayout(false);
            this.tabPageTrace.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this._gbTamper.ResumeLayout(false);
            this._gbTamper.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupTimeLimit.ResumeLayout(false);
            this.groupTimeLimit.PerformLayout();
            this.groupSizeLimit.ResumeLayout(false);
            this.groupSizeLimit.PerformLayout();
            this.ResumeLayout(false);

      }
      #endregion

      private System.Windows.Forms.Button btnOK;
      private System.Windows.Forms.Button btnCancel;
      private System.Windows.Forms.TabControl tabProperties;
      private System.Windows.Forms.TabPage tabPageGeneral;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.MenuItem menuDatabaseAdd;
      private System.Windows.Forms.MenuItem menuDatabaseRemove;
      private System.Windows.Forms.MenuItem menuItem3;
      private System.Windows.Forms.MenuItem menuItem1;
      private System.Windows.Forms.MenuItem menuDatabaseEnable;
      private System.Windows.Forms.MenuItem menuDatabaseDisable;
      private System.Windows.Forms.MenuItem menuDatabaseProperties;
      private System.Windows.Forms.TabPage tabPageTrace;
      private Idera.SQLcompliance.Application.GUI.Controls.NumericTextBox textMaxUnattendedTime;
      private System.Windows.Forms.Label label29;
      private System.Windows.Forms.RadioButton radioTimeLimit;
      private System.Windows.Forms.RadioButton radioUnlimitedTime;
      private System.Windows.Forms.Label label28;
      private Idera.SQLcompliance.Application.GUI.Controls.NumericTextBox textMaxFolderSize;
      private System.Windows.Forms.RadioButton radioDirSizeLimit;
      private System.Windows.Forms.RadioButton radioDirSizeUnlimited;
      private System.Windows.Forms.GroupBox groupBox3;
      private Idera.SQLcompliance.Application.GUI.Controls.NumericTextBox textForcedCollectionInterval;
      private System.Windows.Forms.Label label10;
      private Idera.SQLcompliance.Application.GUI.Controls.NumericTextBox textCollectionInterval;
      private Idera.SQLcompliance.Application.GUI.Controls.NumericTextBox textTraceFileRolloverSize;
      private System.Windows.Forms.Label label6;
      private System.Windows.Forms.Label label4;
      private System.Windows.Forms.GroupBox groupBox7;
      private System.Windows.Forms.TextBox textAgentTraceDirectory;
      private System.Windows.Forms.Label label14;
      private System.Windows.Forms.GroupBox groupTimeLimit;
      private System.Windows.Forms.GroupBox groupSizeLimit;
      private System.Windows.Forms.GroupBox groupBox2;
      private System.Windows.Forms.RadioButton radioManuallyDeployed;
      private System.Windows.Forms.RadioButton radioAutoDeploy;
      private System.Windows.Forms.GroupBox groupBox5;
      private System.Windows.Forms.TextBox textServiceAccount;
      private System.Windows.Forms.Label label9;
      private System.Windows.Forms.GroupBox groupBox1;
      private System.Windows.Forms.Label label5;
      private System.Windows.Forms.TextBox textAgentStatus;
      private System.Windows.Forms.Label label25;
      private Idera.SQLcompliance.Application.GUI.Controls.NumericTextBox textHeartbeatFrequency;
      private Idera.SQLcompliance.Application.GUI.Controls.NumericTextBox textAgentPort;
      private System.Windows.Forms.TextBox textAgentVersion;
      private System.Windows.Forms.Label label24;
      private System.Windows.Forms.Label label15;
      private System.Windows.Forms.Label label27;
      private System.Windows.Forms.Label label33;
      private System.Windows.Forms.TabPage tabPageDeployment;
      private System.Windows.Forms.GroupBox grpAuditStatus;
      private System.Windows.Forms.TextBox txtTimeLastUpdate;
      private System.Windows.Forms.Label lblLastConfigUpdate;
      private System.Windows.Forms.TextBox txtAuditSettingsStatus;
      private System.Windows.Forms.Label lblAuditSettings;
      private System.Windows.Forms.TextBox textCurrentAuditLevel;
      private System.Windows.Forms.TextBox textAgentAuditLevel;
      private System.Windows.Forms.Label label16;
      private System.Windows.Forms.Label label17;
      private System.Windows.Forms.TabPage tabPageServers;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.ListView listServers;
      private System.Windows.Forms.ColumnHeader columnServer;
      private System.Windows.Forms.ColumnHeader columnDescription;
      private System.Windows.Forms.TextBox txtComputer;
      private Idera.SQLcompliance.Application.GUI.Controls.NumericTextBox txtTimeLastHeartbeat;
      private System.Windows.Forms.Button btnUpdateAuditSettings;
      private System.Windows.Forms.ComboBox comboLogLevel;
      private System.Windows.Forms.GroupBox _gbTamper;
      private System.Windows.Forms.Label _lblTamper;
      private Idera.SQLcompliance.Application.GUI.Controls.NumericTextBox _tbTamperInterval;
      private Idera.SQLcompliance.Application.GUI.Controls.NumericTextBox textTraceStartTimeout;
      private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton radioFilesUncompression;
        private System.Windows.Forms.RadioButton radioFilesCompression;
    }
}