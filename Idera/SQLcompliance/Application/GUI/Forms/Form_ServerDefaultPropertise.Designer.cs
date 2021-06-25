namespace Idera.SQLcompliance.Application.GUI.Forms
{
    partial class Form_ServerDefaultPropertise
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_ServerDefaultPropertise));
            this.@__lblTrustedUserTxt = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tabProperties = new System.Windows.Forms.TabControl();
            this.tabPageAuditSettings = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.radioAuditLogs = new System.Windows.Forms.RadioButton();
            this.radioXEvents = new System.Windows.Forms.RadioButton();
            this.radioTrace = new System.Windows.Forms.RadioButton();
            this.grpAuditResults = new System.Windows.Forms.GroupBox();
            this._cbFilterAccessCheck = new System.Windows.Forms.CheckBox();
            this.rbAuditFailedOnly = new System.Windows.Forms.RadioButton();
            this.rbAuditSuccessfulOnly = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.grpAuditActivity = new System.Windows.Forms.GroupBox();
            this.chkAuditUserDefined = new System.Windows.Forms.CheckBox();
            this.chkAuditAdmin = new System.Windows.Forms.CheckBox();
            this.label26 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.checkBox5 = new System.Windows.Forms.CheckBox();
            this.chkAuditSecurity = new System.Windows.Forms.CheckBox();
            this.chkAuditDDL = new System.Windows.Forms.CheckBox();
            this.chkAuditFailedLogins = new System.Windows.Forms.CheckBox();
            this.chkAuditLogins = new System.Windows.Forms.CheckBox();
            this.chkAuditLogouts = new System.Windows.Forms.CheckBox();
            this.tabTrustedUsers = new System.Windows.Forms.TabPage();
            this.pnlTrustedUsers = new System.Windows.Forms.Panel();
            this.txtBxTrustedUserName = new System.Windows.Forms.TextBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.labelTrustedUsers = new System.Windows.Forms.Label();
            this.btnAddTrustedUser = new System.Windows.Forms.Button();
            this.lnkTrustedUserHelp = new System.Windows.Forms.LinkLabel();
            this.btnRemoveTrustedUser = new System.Windows.Forms.Button();
            this.lstTrustedUsers = new System.Windows.Forms.ListView();
            this.columnTrustedUserHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblTrustedUserStatus = new System.Windows.Forms.Label();
            this.tabPageUsers = new System.Windows.Forms.TabPage();
            this.txtBxPrivilegedUser = new System.Windows.Forms.TextBox();
            this.cmbBxPrivilegedUser = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.lblGreyingNotes = new System.Windows.Forms.Label();
            this.lstPrivilegedUsers = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnRemovePriv = new System.Windows.Forms.Button();
            this.btnAddPriv = new System.Windows.Forms.Button();
            this.label19 = new System.Windows.Forms.Label();
            this.grpPrivilegedUserActivity = new System.Windows.Forms.GroupBox();
            this.grpAuditUserActivity = new System.Windows.Forms.GroupBox();
            this.chkUserCaptureDDL = new System.Windows.Forms.CheckBox();
            this.chkUserCaptureTrans = new System.Windows.Forms.CheckBox();
            this.chkAuditUserUserDefined = new System.Windows.Forms.CheckBox();
            this._rbUserAuditFailed = new System.Windows.Forms.RadioButton();
            this._rbUserAuditPassed = new System.Windows.Forms.RadioButton();
            this.chkAuditUserAdmin = new System.Windows.Forms.CheckBox();
            this.chkUserCaptureSQL = new System.Windows.Forms.CheckBox();
            this._cbUserFilterAccessCheck = new System.Windows.Forms.CheckBox();
            this.chkAuditUserDML = new System.Windows.Forms.CheckBox();
            this.chkAuditUserSELECT = new System.Windows.Forms.CheckBox();
            this.chkAuditUserSecurity = new System.Windows.Forms.CheckBox();
            this.chkAuditUserDDL = new System.Windows.Forms.CheckBox();
            this.chkAuditUserFailedLogins = new System.Windows.Forms.CheckBox();
            this.chkAuditUserLogins = new System.Windows.Forms.CheckBox();
            this.chkAuditUserLogouts = new System.Windows.Forms.CheckBox();
            this.rbAuditUserSelected = new System.Windows.Forms.RadioButton();
            this.rbAuditUserAll = new System.Windows.Forms.RadioButton();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.tabPageThresholds = new System.Windows.Forms.TabPage();
            this._checkAlerts = new System.Windows.Forms.CheckBox();
            this._checkLogins = new System.Windows.Forms.CheckBox();
            this._checkLogouts = new System.Windows.Forms.CheckBox();
            this._checkFailedLogins = new System.Windows.Forms.CheckBox();
            this._checkDDL = new System.Windows.Forms.CheckBox();
            this._checkSecurity = new System.Windows.Forms.CheckBox();
            this._checkAllActivity = new System.Windows.Forms.CheckBox();
            this._checkPrivUser = new System.Windows.Forms.CheckBox();
            this.label32 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this._cbPrivUserPeriod = new System.Windows.Forms.ComboBox();
            this._cbAlertsPeriod = new System.Windows.Forms.ComboBox();
            this._cbLoginsPeriod = new System.Windows.Forms.ComboBox();
            this._cbLogoutsPeriod = new System.Windows.Forms.ComboBox();
            this._cbSecurityPeriod = new System.Windows.Forms.ComboBox();
            this._cbAllActivityPeriod = new System.Windows.Forms.ComboBox();
            this._cbFailedLoginsPeriod = new System.Windows.Forms.ComboBox();
            this._cbDDLPeriod = new System.Windows.Forms.ComboBox();
            this.label25 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.label34 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this._txtAlertsWarning = new Idera.SQLcompliance.Application.GUI.Controls.NumericTextBox();
            this._txtLoginsWarning = new Idera.SQLcompliance.Application.GUI.Controls.NumericTextBox();
            this._txtLogoutsWarning = new Idera.SQLcompliance.Application.GUI.Controls.NumericTextBox();
            this._txtFailedLoginsWarning = new Idera.SQLcompliance.Application.GUI.Controls.NumericTextBox();
            this._txtDDLWarning = new Idera.SQLcompliance.Application.GUI.Controls.NumericTextBox();
            this._txtSecurityWarning = new Idera.SQLcompliance.Application.GUI.Controls.NumericTextBox();
            this._txtAllActivityWarning = new Idera.SQLcompliance.Application.GUI.Controls.NumericTextBox();
            this._txtPrivUserError = new Idera.SQLcompliance.Application.GUI.Controls.NumericTextBox();
            this._txtAlertsError = new Idera.SQLcompliance.Application.GUI.Controls.NumericTextBox();
            this._txtLoginsError = new Idera.SQLcompliance.Application.GUI.Controls.NumericTextBox();
            this._txtLogoutsError = new Idera.SQLcompliance.Application.GUI.Controls.NumericTextBox();
            this._txtFailedLoginsError = new Idera.SQLcompliance.Application.GUI.Controls.NumericTextBox();
            this._txtDDLError = new Idera.SQLcompliance.Application.GUI.Controls.NumericTextBox();
            this._txtSecurityError = new Idera.SQLcompliance.Application.GUI.Controls.NumericTextBox();
            this._txtAllActivityError = new Idera.SQLcompliance.Application.GUI.Controls.NumericTextBox();
            this._txtPrivUserWarning = new Idera.SQLcompliance.Application.GUI.Controls.NumericTextBox();
            this.tabPageAdvanced = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelSqlReportCharacters = new System.Windows.Forms.Label();
            this.textReportLimitSQL = new System.Windows.Forms.TextBox();
            this.labelSqlReport = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.textLimitSQL = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.radioLimitSQL = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.radioUnlimitedSQL = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label16 = new System.Windows.Forms.Label();
            this.radioGrantAll = new System.Windows.Forms.RadioButton();
            this.radioGrantEventsOnly = new System.Windows.Forms.RadioButton();
            this.radioDeny = new System.Windows.Forms.RadioButton();
            this.menuDatabaseAdd = new System.Windows.Forms.MenuItem();
            this.menuDatabaseRemove = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.menuDatabaseEnable = new System.Windows.Forms.MenuItem();
            this.menuDatabaseDisable = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuDatabaseProperties = new System.Windows.Forms.MenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnRestoreToIderaDefaultSettings = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.tabProperties.SuspendLayout();
            this.tabPageAuditSettings.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.grpAuditResults.SuspendLayout();
            this.grpAuditActivity.SuspendLayout();
            this.tabTrustedUsers.SuspendLayout();
            this.pnlTrustedUsers.SuspendLayout();
            this.tabPageUsers.SuspendLayout();
            this.grpPrivilegedUserActivity.SuspendLayout();
            this.grpAuditUserActivity.SuspendLayout();
            this.tabPageThresholds.SuspendLayout();
            this.tabPageAdvanced.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // __lblTrustedUserTxt
            // 
            this.@__lblTrustedUserTxt.AutoSize = true;
            this.@__lblTrustedUserTxt.Location = new System.Drawing.Point(7, 357);
            this.@__lblTrustedUserTxt.Name = "__lblTrustedUserTxt";
            this.@__lblTrustedUserTxt.Size = new System.Drawing.Size(239, 13);
            this.@__lblTrustedUserTxt.TabIndex = 28;
            this.@__lblTrustedUserTxt.Text = "Specify the logins or roles you trust on this server.";
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(381, 536);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "&Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(461, 536);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // tabProperties
            // 
            this.tabProperties.Controls.Add(this.tabPageAuditSettings);
            this.tabProperties.Controls.Add(this.tabTrustedUsers);
            this.tabProperties.Controls.Add(this.tabPageUsers);
            this.tabProperties.Controls.Add(this.tabPageThresholds);
            this.tabProperties.Controls.Add(this.tabPageAdvanced);
            this.tabProperties.Location = new System.Drawing.Point(0, 0);
            this.tabProperties.Name = "tabProperties";
            this.tabProperties.SelectedIndex = 0;
            this.tabProperties.Size = new System.Drawing.Size(540, 488);
            this.tabProperties.TabIndex = 0;
            // 
            // tabPageAuditSettings
            // 
            this.tabPageAuditSettings.Controls.Add(this.groupBox4);
            this.tabPageAuditSettings.Controls.Add(this.grpAuditResults);
            this.tabPageAuditSettings.Controls.Add(this.label2);
            this.tabPageAuditSettings.Controls.Add(this.grpAuditActivity);
            this.tabPageAuditSettings.Location = new System.Drawing.Point(4, 22);
            this.tabPageAuditSettings.Name = "tabPageAuditSettings";
            this.tabPageAuditSettings.Size = new System.Drawing.Size(532, 462);
            this.tabPageAuditSettings.TabIndex = 1;
            this.tabPageAuditSettings.Text = "Audited Activities";
            this.tabPageAuditSettings.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.radioAuditLogs);
            this.groupBox4.Controls.Add(this.radioXEvents);
            this.groupBox4.Controls.Add(this.radioTrace);
            this.groupBox4.Location = new System.Drawing.Point(12, 303);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(508, 89);
            this.groupBox4.TabIndex = 35;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Capture DML and Select Activities";
            // 
            // radioAuditLogs
            // 
            this.radioAuditLogs.AutoSize = true;
            this.radioAuditLogs.Location = new System.Drawing.Point(12, 65);
            this.radioAuditLogs.Name = "radioAuditLogs";
            this.radioAuditLogs.Size = new System.Drawing.Size(194, 17);
            this.radioAuditLogs.TabIndex = 2;
            this.radioAuditLogs.Text = "Via SQL Server Audit Specifications";
            this.radioAuditLogs.UseVisualStyleBackColor = true;
            this.radioAuditLogs.CheckedChanged += new System.EventHandler(this.auditOption_CheckedChanged);
            // 
            // radioXEvents
            // 
            this.radioXEvents.AutoSize = true;
            this.radioXEvents.Location = new System.Drawing.Point(12, 42);
            this.radioXEvents.Name = "radioXEvents";
            this.radioXEvents.Size = new System.Drawing.Size(124, 17);
            this.radioXEvents.TabIndex = 1;
            this.radioXEvents.Text = "Via Extended Events";
            this.radioXEvents.UseVisualStyleBackColor = true;
            this.radioXEvents.CheckedChanged += new System.EventHandler(this.auditOption_CheckedChanged);
            // 
            // radioTrace
            // 
            this.radioTrace.AutoSize = true;
            this.radioTrace.Checked = true;
            this.radioTrace.Location = new System.Drawing.Point(12, 19);
            this.radioTrace.Name = "radioTrace";
            this.radioTrace.Size = new System.Drawing.Size(107, 17);
            this.radioTrace.TabIndex = 0;
            this.radioTrace.TabStop = true;
            this.radioTrace.Text = "Via Trace Events";
            this.radioTrace.UseVisualStyleBackColor = true;
            this.radioTrace.CheckedChanged += new System.EventHandler(this.auditOption_CheckedChanged);
            // 
            // grpAuditResults
            // 
            this.grpAuditResults.Controls.Add(this._cbFilterAccessCheck);
            this.grpAuditResults.Controls.Add(this.rbAuditFailedOnly);
            this.grpAuditResults.Controls.Add(this.rbAuditSuccessfulOnly);
            this.grpAuditResults.Location = new System.Drawing.Point(12, 208);
            this.grpAuditResults.Name = "grpAuditResults";
            this.grpAuditResults.Size = new System.Drawing.Size(508, 96);
            this.grpAuditResults.TabIndex = 34;
            this.grpAuditResults.TabStop = false;
            this.grpAuditResults.Text = "Access Check Filter";
            // 
            // _cbFilterAccessCheck
            // 
            this._cbFilterAccessCheck.Location = new System.Drawing.Point(12, 24);
            this._cbFilterAccessCheck.Name = "_cbFilterAccessCheck";
            this._cbFilterAccessCheck.Size = new System.Drawing.Size(228, 16);
            this._cbFilterAccessCheck.TabIndex = 0;
            this._cbFilterAccessCheck.Text = "&Filter events based on access check";
            this._cbFilterAccessCheck.Click += new System.EventHandler(this.Click_cbFilterAccessCheck);
            this._cbFilterAccessCheck.CheckedChanged += new System.EventHandler(this.auditSettings_CheckedChanged);
            // 
            // rbAuditFailedOnly
            // 
            this.rbAuditFailedOnly.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.rbAuditFailedOnly.Location = new System.Drawing.Point(32, 72);
            this.rbAuditFailedOnly.Name = "rbAuditFailedOnly";
            this.rbAuditFailedOnly.Size = new System.Drawing.Size(292, 16);
            this.rbAuditFailedOnly.TabIndex = 2;
            this.rbAuditFailedOnly.Text = "&Failed";
            // 
            // rbAuditSuccessfulOnly
            // 
            this.rbAuditSuccessfulOnly.Checked = true;
            this.rbAuditSuccessfulOnly.Location = new System.Drawing.Point(32, 48);
            this.rbAuditSuccessfulOnly.Name = "rbAuditSuccessfulOnly";
            this.rbAuditSuccessfulOnly.Size = new System.Drawing.Size(248, 16);
            this.rbAuditSuccessfulOnly.TabIndex = 1;
            this.rbAuditSuccessfulOnly.TabStop = true;
            this.rbAuditSuccessfulOnly.Text = "&Passed";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(16, 408);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(504, 44);
            this.label2.TabIndex = 33;
            this.label2.Text = resources.GetString("label2.Text");
            // 
            // grpAuditActivity
            // 
            this.grpAuditActivity.Controls.Add(this.chkAuditUserDefined);
            this.grpAuditActivity.Controls.Add(this.chkAuditAdmin);
            this.grpAuditActivity.Controls.Add(this.label26);
            this.grpAuditActivity.Controls.Add(this.label31);
            this.grpAuditActivity.Controls.Add(this.checkBox5);
            this.grpAuditActivity.Controls.Add(this.chkAuditSecurity);
            this.grpAuditActivity.Controls.Add(this.chkAuditDDL);
            this.grpAuditActivity.Controls.Add(this.chkAuditFailedLogins);
            this.grpAuditActivity.Controls.Add(this.chkAuditLogins);
            this.grpAuditActivity.Controls.Add(this.chkAuditLogouts);
            this.grpAuditActivity.Location = new System.Drawing.Point(12, 12);
            this.grpAuditActivity.Name = "grpAuditActivity";
            this.grpAuditActivity.Size = new System.Drawing.Size(508, 190);
            this.grpAuditActivity.TabIndex = 21;
            this.grpAuditActivity.TabStop = false;
            this.grpAuditActivity.Text = "Audited Activity";
            // 
            // chkAuditUserDefined
            // 
            this.chkAuditUserDefined.Checked = true;
            this.chkAuditUserDefined.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAuditUserDefined.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.chkAuditUserDefined.Location = new System.Drawing.Point(12, 165);
            this.chkAuditUserDefined.Name = "chkAuditUserDefined";
            this.chkAuditUserDefined.Size = new System.Drawing.Size(364, 16);
            this.chkAuditUserDefined.TabIndex = 28;
            this.chkAuditUserDefined.Text = "&User Defined Events (custom SQL Server event type)";
            this.chkAuditUserDefined.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.chkAuditUserDefined.CheckedChanged += new System.EventHandler(this.auditSettings_CheckedChanged);
            // 
            // chkAuditAdmin
            // 
            this.chkAuditAdmin.Checked = true;
            this.chkAuditAdmin.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAuditAdmin.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.chkAuditAdmin.Location = new System.Drawing.Point(12, 117);
            this.chkAuditAdmin.Name = "chkAuditAdmin";
            this.chkAuditAdmin.Size = new System.Drawing.Size(272, 17);
            this.chkAuditAdmin.TabIndex = 27;
            this.chkAuditAdmin.Text = "&Administrative Actions (e.g. DBCC)";
            this.chkAuditAdmin.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.chkAuditAdmin.CheckedChanged += new System.EventHandler(this.auditSettings_CheckedChanged);
            // 
            // label26
            // 
            this.label26.Location = new System.Drawing.Point(124, 93);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(272, 16);
            this.label26.TabIndex = 18;
            this.label26.Text = "(e.g.  GRANT, REVOKE, LOGIN CHANGE PWD)";
            // 
            // label31
            // 
            this.label31.Location = new System.Drawing.Point(160, 141);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(196, 16);
            this.label31.TabIndex = 16;
            this.label31.Text = "(e.g.  CREATE or DROP DATABASE)";
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
            this.chkAuditSecurity.Location = new System.Drawing.Point(12, 93);
            this.chkAuditSecurity.Name = "chkAuditSecurity";
            this.chkAuditSecurity.Size = new System.Drawing.Size(124, 17);
            this.chkAuditSecurity.TabIndex = 24;
            this.chkAuditSecurity.Text = "&Security Changes";
            this.chkAuditSecurity.CheckedChanged += new System.EventHandler(this.auditSettings_CheckedChanged);
            // 
            // chkAuditDDL
            // 
            this.chkAuditDDL.Checked = true;
            this.chkAuditDDL.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAuditDDL.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.chkAuditDDL.Location = new System.Drawing.Point(12, 141);
            this.chkAuditDDL.Name = "chkAuditDDL";
            this.chkAuditDDL.Size = new System.Drawing.Size(148, 16);
            this.chkAuditDDL.TabIndex = 26;
            this.chkAuditDDL.Text = "&Database Def&inition(DDL) ";
            this.chkAuditDDL.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.chkAuditDDL.CheckedChanged += new System.EventHandler(this.auditSettings_CheckedChanged);
            // 
            // chkAuditFailedLogins
            // 
            this.chkAuditFailedLogins.Checked = true;
            this.chkAuditFailedLogins.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAuditFailedLogins.Location = new System.Drawing.Point(12, 69);
            this.chkAuditFailedLogins.Name = "chkAuditFailedLogins";
            this.chkAuditFailedLogins.Size = new System.Drawing.Size(104, 17);
            this.chkAuditFailedLogins.TabIndex = 23;
            this.chkAuditFailedLogins.Text = "Failed lo&gins";
            this.chkAuditFailedLogins.CheckedChanged += new System.EventHandler(this.auditSettings_CheckedChanged);
            // 
            // chkAuditLogins
            // 
            this.chkAuditLogins.Checked = true;
            this.chkAuditLogins.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAuditLogins.Location = new System.Drawing.Point(12, 20);
            this.chkAuditLogins.Name = "chkAuditLogins";
            this.chkAuditLogins.Size = new System.Drawing.Size(104, 17);
            this.chkAuditLogins.TabIndex = 22;
            this.chkAuditLogins.Text = "&Logins";
            this.chkAuditLogins.CheckedChanged += new System.EventHandler(this.auditSettings_CheckedChanged);
            // 
            // chkAuditLogouts
            // 
            this.chkAuditLogouts.Checked = true;
            this.chkAuditLogouts.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAuditLogouts.Location = new System.Drawing.Point(12, 43);
            this.chkAuditLogouts.Name = "chkAuditLogouts";
            this.chkAuditLogouts.Size = new System.Drawing.Size(104, 17);
            this.chkAuditLogouts.TabIndex = 22;
            this.chkAuditLogouts.Text = "Log&outs";
            this.chkAuditLogouts.CheckedChanged += new System.EventHandler(this.auditSettings_CheckedChanged);
            // 
            // tabTrustedUsers
            // 
            this.tabTrustedUsers.Controls.Add(this.pnlTrustedUsers);
            this.tabTrustedUsers.Controls.Add(this.lblTrustedUserStatus);
            this.tabTrustedUsers.Location = new System.Drawing.Point(4, 22);
            this.tabTrustedUsers.Name = "tabTrustedUsers";
            this.tabTrustedUsers.Size = new System.Drawing.Size(532, 462);
            this.tabTrustedUsers.TabIndex = 4;
            this.tabTrustedUsers.Text = "Trusted Users";
            this.tabTrustedUsers.UseVisualStyleBackColor = true;
            // 
            // pnlTrustedUsers
            // 
            this.pnlTrustedUsers.Controls.Add(this.txtBxTrustedUserName);
            this.pnlTrustedUsers.Controls.Add(this.comboBox1);
            this.pnlTrustedUsers.Controls.Add(this.label1);
            this.pnlTrustedUsers.Controls.Add(this.labelTrustedUsers);
            this.pnlTrustedUsers.Controls.Add(this.@__lblTrustedUserTxt);
            this.pnlTrustedUsers.Controls.Add(this.btnAddTrustedUser);
            this.pnlTrustedUsers.Controls.Add(this.lnkTrustedUserHelp);
            this.pnlTrustedUsers.Controls.Add(this.btnRemoveTrustedUser);
            this.pnlTrustedUsers.Controls.Add(this.lstTrustedUsers);
            this.pnlTrustedUsers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlTrustedUsers.Location = new System.Drawing.Point(0, 0);
            this.pnlTrustedUsers.Name = "pnlTrustedUsers";
            this.pnlTrustedUsers.Size = new System.Drawing.Size(532, 462);
            this.pnlTrustedUsers.TabIndex = 29;
            // 
            // txtBxTrustedUserName
            // 
            this.txtBxTrustedUserName.Location = new System.Drawing.Point(234, 14);
            this.txtBxTrustedUserName.Name = "txtBxTrustedUserName";
            this.txtBxTrustedUserName.Size = new System.Drawing.Size(203, 20);
            this.txtBxTrustedUserName.TabIndex = 31;
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Server Roles",
            "Server Logins"});
            this.comboBox1.Location = new System.Drawing.Point(107, 14);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(105, 21);
            this.comboBox1.TabIndex = 30;
            this.comboBox1.SelectedIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 13);
            this.label1.TabIndex = 29;
            this.label1.Text = "Add Trusted User:";
            // 
            // labelTrustedUsers
            // 
            this.labelTrustedUsers.Location = new System.Drawing.Point(8, 55);
            this.labelTrustedUsers.Name = "labelTrustedUsers";
            this.labelTrustedUsers.Size = new System.Drawing.Size(284, 16);
            this.labelTrustedUsers.TabIndex = 23;
            this.labelTrustedUsers.Text = "&Trusted users and roles to be filtered:";
            // 
            // btnAddTrustedUser
            // 
            this.btnAddTrustedUser.Location = new System.Drawing.Point(451, 11);
            this.btnAddTrustedUser.Name = "btnAddTrustedUser";
            this.btnAddTrustedUser.Size = new System.Drawing.Size(75, 23);
            this.btnAddTrustedUser.TabIndex = 24;
            this.btnAddTrustedUser.Text = "&Add...";
            this.btnAddTrustedUser.Click += new System.EventHandler(this.Click_btnAddTrustedUser);
            // 
            // lnkTrustedUserHelp
            // 
            this.lnkTrustedUserHelp.LinkArea = new System.Windows.Forms.LinkArea(86, 16);
            this.lnkTrustedUserHelp.Location = new System.Drawing.Point(10, 289);
            this.lnkTrustedUserHelp.Name = "lnkTrustedUserHelp";
            this.lnkTrustedUserHelp.Size = new System.Drawing.Size(427, 48);
            this.lnkTrustedUserHelp.TabIndex = 27;
            this.lnkTrustedUserHelp.TabStop = true;
            this.lnkTrustedUserHelp.Text = "A trusted user is a SQL Server login or role whose activity you do not need to au" +
    "dit. Tell me more ...";
            this.lnkTrustedUserHelp.UseCompatibleTextRendering = true;
            this.lnkTrustedUserHelp.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkClicked_lnkTrustedUserHelp);
            // 
            // btnRemoveTrustedUser
            // 
            this.btnRemoveTrustedUser.Location = new System.Drawing.Point(451, 89);
            this.btnRemoveTrustedUser.Name = "btnRemoveTrustedUser";
            this.btnRemoveTrustedUser.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveTrustedUser.TabIndex = 25;
            this.btnRemoveTrustedUser.Text = "&Remove";
            this.btnRemoveTrustedUser.Click += new System.EventHandler(this.Click_btnRemoveTrustedUser);
            // 
            // lstTrustedUsers
            // 
            this.lstTrustedUsers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnTrustedUserHeader2});
            this.lstTrustedUsers.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lstTrustedUsers.Location = new System.Drawing.Point(8, 89);
            this.lstTrustedUsers.Name = "lstTrustedUsers";
            this.lstTrustedUsers.Size = new System.Drawing.Size(430, 179);
            this.lstTrustedUsers.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lstTrustedUsers.TabIndex = 26;
            this.lstTrustedUsers.UseCompatibleStateImageBehavior = false;
            this.lstTrustedUsers.View = System.Windows.Forms.View.Details;
            this.lstTrustedUsers.SelectedIndexChanged += new System.EventHandler(this.SelectedIndexChanged_lstTrustedUsers);
            // 
            // columnTrustedUserHeader2
            // 
            this.columnTrustedUserHeader2.Text = "Name";
            this.columnTrustedUserHeader2.Width = 415;
            // 
            // lblTrustedUserStatus
            // 
            this.lblTrustedUserStatus.Location = new System.Drawing.Point(30, 30);
            this.lblTrustedUserStatus.Name = "lblTrustedUserStatus";
            this.lblTrustedUserStatus.Size = new System.Drawing.Size(425, 56);
            this.lblTrustedUserStatus.TabIndex = 30;
            // 
            // tabPageUsers
            // 
            this.tabPageUsers.Controls.Add(this.txtBxPrivilegedUser);
            this.tabPageUsers.Controls.Add(this.cmbBxPrivilegedUser);
            this.tabPageUsers.Controls.Add(this.label3);
            this.tabPageUsers.Controls.Add(this.lblGreyingNotes);
            this.tabPageUsers.Controls.Add(this.lstPrivilegedUsers);
            this.tabPageUsers.Controls.Add(this.btnRemovePriv);
            this.tabPageUsers.Controls.Add(this.btnAddPriv);
            this.tabPageUsers.Controls.Add(this.label19);
            this.tabPageUsers.Controls.Add(this.grpPrivilegedUserActivity);
            this.tabPageUsers.Location = new System.Drawing.Point(4, 22);
            this.tabPageUsers.Name = "tabPageUsers";
            this.tabPageUsers.Size = new System.Drawing.Size(532, 462);
            this.tabPageUsers.TabIndex = 7;
            this.tabPageUsers.Text = "Privileged User Auditing";
            this.tabPageUsers.UseVisualStyleBackColor = true;
            // 
            // txtBxPrivilegedUser
            // 
            this.txtBxPrivilegedUser.Location = new System.Drawing.Point(245, 7);
            this.txtBxPrivilegedUser.Name = "txtBxPrivilegedUser";
            this.txtBxPrivilegedUser.Size = new System.Drawing.Size(185, 20);
            this.txtBxPrivilegedUser.TabIndex = 26;
            // 
            // cmbBxPrivilegedUser
            // 
            this.cmbBxPrivilegedUser.FormattingEnabled = true;
            this.cmbBxPrivilegedUser.Items.AddRange(new object[] {
            "Server Roles",
            "Server Logins"});
            this.cmbBxPrivilegedUser.Location = new System.Drawing.Point(118, 7);
            this.cmbBxPrivilegedUser.Name = "cmbBxPrivilegedUser";
            this.cmbBxPrivilegedUser.Size = new System.Drawing.Size(99, 21);
            this.cmbBxPrivilegedUser.TabIndex = 25;
            this.cmbBxPrivilegedUser.SelectedIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(103, 13);
            this.label3.TabIndex = 24;
            this.label3.Text = "Add Privileged User:";
            // 
            // lblGreyingNotes
            // 
            this.lblGreyingNotes.AutoSize = true;
            this.lblGreyingNotes.Location = new System.Drawing.Point(20, 433);
            this.lblGreyingNotes.MaximumSize = new System.Drawing.Size(508, 0);
            this.lblGreyingNotes.Name = "lblGreyingNotes";
            this.lblGreyingNotes.Size = new System.Drawing.Size(497, 26);
            this.lblGreyingNotes.TabIndex = 23;
            this.lblGreyingNotes.Text = "Note: Selected items that are disabled have been enabled at the server level. Des" +
    "elected items that are disabled are waiting for other settings to be applied bef" +
    "ore you can use them.";
            // 
            // lstPrivilegedUsers
            // 
            this.lstPrivilegedUsers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lstPrivilegedUsers.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lstPrivilegedUsers.Location = new System.Drawing.Point(12, 54);
            this.lstPrivilegedUsers.Name = "lstPrivilegedUsers";
            this.lstPrivilegedUsers.Size = new System.Drawing.Size(440, 120);
            this.lstPrivilegedUsers.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lstPrivilegedUsers.TabIndex = 22;
            this.lstPrivilegedUsers.UseCompatibleStateImageBehavior = false;
            this.lstPrivilegedUsers.View = System.Windows.Forms.View.Details;
            this.lstPrivilegedUsers.SelectedIndexChanged += new System.EventHandler(this.lstPrivilegedUsers_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 415;
            // 
            // btnRemovePriv
            // 
            this.btnRemovePriv.Location = new System.Drawing.Point(460, 56);
            this.btnRemovePriv.Name = "btnRemovePriv";
            this.btnRemovePriv.Size = new System.Drawing.Size(60, 23);
            this.btnRemovePriv.TabIndex = 20;
            this.btnRemovePriv.Text = "&Remove";
            this.btnRemovePriv.Click += new System.EventHandler(this.btnRemovePriv_Click);
            // 
            // btnAddPriv
            // 
            this.btnAddPriv.Location = new System.Drawing.Point(460, 5);
            this.btnAddPriv.Name = "btnAddPriv";
            this.btnAddPriv.Size = new System.Drawing.Size(60, 23);
            this.btnAddPriv.TabIndex = 19;
            this.btnAddPriv.Text = "&Add...";
            this.btnAddPriv.Click += new System.EventHandler(this.btnAddPriv_Click);
            // 
            // label19
            // 
            this.label19.Location = new System.Drawing.Point(9, 33);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(284, 16);
            this.label19.TabIndex = 17;
            this.label19.Text = "&Privileged users and roles to be audited:";
            // 
            // grpPrivilegedUserActivity
            // 
            this.grpPrivilegedUserActivity.Controls.Add(this.grpAuditUserActivity);
            this.grpPrivilegedUserActivity.Controls.Add(this.rbAuditUserSelected);
            this.grpPrivilegedUserActivity.Controls.Add(this.rbAuditUserAll);
            this.grpPrivilegedUserActivity.Controls.Add(this.checkBox3);
            this.grpPrivilegedUserActivity.Enabled = false;
            this.grpPrivilegedUserActivity.Location = new System.Drawing.Point(15, 180);
            this.grpPrivilegedUserActivity.Name = "grpPrivilegedUserActivity";
            this.grpPrivilegedUserActivity.Size = new System.Drawing.Size(508, 235);
            this.grpPrivilegedUserActivity.TabIndex = 21;
            this.grpPrivilegedUserActivity.TabStop = false;
            this.grpPrivilegedUserActivity.Text = "Audited Activity";
            // 
            // grpAuditUserActivity
            // 
            this.grpAuditUserActivity.Controls.Add(this.chkUserCaptureDDL);
            this.grpAuditUserActivity.Controls.Add(this.chkUserCaptureTrans);
            this.grpAuditUserActivity.Controls.Add(this.chkAuditUserUserDefined);
            this.grpAuditUserActivity.Controls.Add(this._rbUserAuditFailed);
            this.grpAuditUserActivity.Controls.Add(this._rbUserAuditPassed);
            this.grpAuditUserActivity.Controls.Add(this.chkAuditUserAdmin);
            this.grpAuditUserActivity.Controls.Add(this.chkUserCaptureSQL);
            this.grpAuditUserActivity.Controls.Add(this._cbUserFilterAccessCheck);
            this.grpAuditUserActivity.Controls.Add(this.chkAuditUserDML);
            this.grpAuditUserActivity.Controls.Add(this.chkAuditUserSELECT);
            this.grpAuditUserActivity.Controls.Add(this.chkAuditUserSecurity);
            this.grpAuditUserActivity.Controls.Add(this.chkAuditUserDDL);
            this.grpAuditUserActivity.Controls.Add(this.chkAuditUserFailedLogins);
            this.grpAuditUserActivity.Controls.Add(this.chkAuditUserLogins);
            this.grpAuditUserActivity.Controls.Add(this.chkAuditUserLogouts);
            this.grpAuditUserActivity.Location = new System.Drawing.Point(28, 56);
            this.grpAuditUserActivity.Name = "grpAuditUserActivity";
            this.grpAuditUserActivity.Size = new System.Drawing.Size(468, 173);
            this.grpAuditUserActivity.TabIndex = 15;
            this.grpAuditUserActivity.TabStop = false;
            // 
            // chkUserCaptureDDL
            // 
            this.chkUserCaptureDDL.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.chkUserCaptureDDL.Location = new System.Drawing.Point(10, 145);
            this.chkUserCaptureDDL.Name = "chkUserCaptureDDL";
            this.chkUserCaptureDDL.Size = new System.Drawing.Size(356, 20);
            this.chkUserCaptureDDL.TabIndex = 13;
            this.chkUserCaptureDDL.Text = "Capture S&QL statements for DDL and Security Changes";
            this.chkUserCaptureDDL.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.chkUserCaptureDDL.CheckedChanged += new System.EventHandler(this.chkUserCaptureDDL_CheckedChanged);
            // 
            // chkUserCaptureTrans
            // 
            this.chkUserCaptureTrans.AutoSize = true;
            this.chkUserCaptureTrans.Location = new System.Drawing.Point(10, 125);
            this.chkUserCaptureTrans.Name = "chkUserCaptureTrans";
            this.chkUserCaptureTrans.Size = new System.Drawing.Size(233, 17);
            this.chkUserCaptureTrans.TabIndex = 12;
            this.chkUserCaptureTrans.Text = "Capture Transaction Status for DML Activity";
            this.chkUserCaptureTrans.UseVisualStyleBackColor = true;
            // 
            // chkAuditUserUserDefined
            // 
            this.chkAuditUserUserDefined.Location = new System.Drawing.Point(282, 52);
            this.chkAuditUserUserDefined.Name = "chkAuditUserUserDefined";
            this.chkAuditUserUserDefined.Size = new System.Drawing.Size(180, 16);
            this.chkAuditUserUserDefined.TabIndex = 5;
            this.chkAuditUserUserDefined.Text = "&User Defined Events";
            // 
            // _rbUserAuditFailed
            // 
            this._rbUserAuditFailed.Location = new System.Drawing.Point(304, 84);
            this._rbUserAuditFailed.Name = "_rbUserAuditFailed";
            this._rbUserAuditFailed.Size = new System.Drawing.Size(60, 16);
            this._rbUserAuditFailed.TabIndex = 10;
            this._rbUserAuditFailed.Text = "Faile&d";
            // 
            // _rbUserAuditPassed
            // 
            this._rbUserAuditPassed.Checked = true;
            this._rbUserAuditPassed.Location = new System.Drawing.Point(224, 84);
            this._rbUserAuditPassed.Name = "_rbUserAuditPassed";
            this._rbUserAuditPassed.Size = new System.Drawing.Size(76, 16);
            this._rbUserAuditPassed.TabIndex = 9;
            this._rbUserAuditPassed.TabStop = true;
            this._rbUserAuditPassed.Text = "&Passed";
            // 
            // chkAuditUserAdmin
            // 
            this.chkAuditUserAdmin.Checked = true;
            this.chkAuditUserAdmin.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAuditUserAdmin.Location = new System.Drawing.Point(128, 32);
            this.chkAuditUserAdmin.Name = "chkAuditUserAdmin";
            this.chkAuditUserAdmin.Size = new System.Drawing.Size(144, 16);
            this.chkAuditUserAdmin.TabIndex = 1;
            this.chkAuditUserAdmin.Text = "Administrative Actions";
            // 
            // chkUserCaptureSQL
            // 
            this.chkUserCaptureSQL.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.chkUserCaptureSQL.Location = new System.Drawing.Point(10, 104);
            this.chkUserCaptureSQL.Name = "chkUserCaptureSQL";
            this.chkUserCaptureSQL.Size = new System.Drawing.Size(356, 20);
            this.chkUserCaptureSQL.TabIndex = 11;
            this.chkUserCaptureSQL.Text = "Capture S&QL Statements for DML and SELECT activities";
            this.chkUserCaptureSQL.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.chkUserCaptureSQL.CheckedChanged += new System.EventHandler(this.chkUserCaptureSQL_CheckedChanged);
            // 
            // _cbUserFilterAccessCheck
            // 
            this._cbUserFilterAccessCheck.Location = new System.Drawing.Point(10, 84);
            this._cbUserFilterAccessCheck.Name = "_cbUserFilterAccessCheck";
            this._cbUserFilterAccessCheck.Size = new System.Drawing.Size(226, 16);
            this._cbUserFilterAccessCheck.TabIndex = 8;
            this._cbUserFilterAccessCheck.Text = "&Filter events based on access check:";
            this._cbUserFilterAccessCheck.Click += new System.EventHandler(this.chkExcludes_Click);
            // 
            // chkAuditUserDML
            // 
            this.chkAuditUserDML.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.chkAuditUserDML.Location = new System.Drawing.Point(282, 12);
            this.chkAuditUserDML.Name = "chkAuditUserDML";
            this.chkAuditUserDML.Size = new System.Drawing.Size(164, 16);
            this.chkAuditUserDML.TabIndex = 8;
            this.chkAuditUserDML.Text = "Database &Modification (DML)";
            this.chkAuditUserDML.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.chkAuditUserDML.CheckedChanged += new System.EventHandler(this.chkAuditUserDML_CheckedChanged);
            // 
            // chkAuditUserSELECT
            // 
            this.chkAuditUserSELECT.Location = new System.Drawing.Point(282, 32);
            this.chkAuditUserSELECT.Name = "chkAuditUserSELECT";
            this.chkAuditUserSELECT.Size = new System.Drawing.Size(180, 16);
            this.chkAuditUserSELECT.TabIndex = 2;
            this.chkAuditUserSELECT.Text = "Database S&ELECT operations";
            this.chkAuditUserSELECT.CheckedChanged += new System.EventHandler(this.chkAuditUserDML_CheckedChanged);
            // 
            // chkAuditUserSecurity
            // 
            this.chkAuditUserSecurity.Checked = true;
            this.chkAuditUserSecurity.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAuditUserSecurity.Location = new System.Drawing.Point(128, 12);
            this.chkAuditUserSecurity.Name = "chkAuditUserSecurity";
            this.chkAuditUserSecurity.Size = new System.Drawing.Size(112, 17);
            this.chkAuditUserSecurity.TabIndex = 7;
            this.chkAuditUserSecurity.Text = "&Security Changes";
            this.chkAuditUserSecurity.CheckedChanged += new System.EventHandler(this.chkUserCaptureDDL_CheckedChanged);
            // 
            // chkAuditUserDDL
            // 
            this.chkAuditUserDDL.Checked = true;
            this.chkAuditUserDDL.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAuditUserDDL.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.chkAuditUserDDL.Location = new System.Drawing.Point(128, 52);
            this.chkAuditUserDDL.Name = "chkAuditUserDDL";
            this.chkAuditUserDDL.Size = new System.Drawing.Size(156, 16);
            this.chkAuditUserDDL.TabIndex = 4;
            this.chkAuditUserDDL.Text = "Database Def&inition (DDL)";
            this.chkAuditUserDDL.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.chkAuditUserDDL.CheckedChanged += new System.EventHandler(this.chkUserCaptureDDL_CheckedChanged);
            // 
            // chkAuditUserFailedLogins
            // 
            this.chkAuditUserFailedLogins.Checked = true;
            this.chkAuditUserFailedLogins.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAuditUserFailedLogins.Location = new System.Drawing.Point(10, 52);
            this.chkAuditUserFailedLogins.Name = "chkAuditUserFailedLogins";
            this.chkAuditUserFailedLogins.Size = new System.Drawing.Size(104, 17);
            this.chkAuditUserFailedLogins.TabIndex = 3;
            this.chkAuditUserFailedLogins.Text = "Failed lo&gins";
            // 
            // chkAuditUserLogins
            // 
            this.chkAuditUserLogins.Checked = true;
            this.chkAuditUserLogins.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAuditUserLogins.Location = new System.Drawing.Point(10, 12);
            this.chkAuditUserLogins.Name = "chkAuditUserLogins";
            this.chkAuditUserLogins.Size = new System.Drawing.Size(104, 17);
            this.chkAuditUserLogins.TabIndex = 0;
            this.chkAuditUserLogins.Text = "&Logins";
            // 
            // chkAuditUserLogouts
            // 
            this.chkAuditUserLogouts.Checked = true;
            this.chkAuditUserLogouts.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAuditUserLogouts.Location = new System.Drawing.Point(10, 32);
            this.chkAuditUserLogouts.Name = "chkAuditUserLogouts";
            this.chkAuditUserLogouts.Size = new System.Drawing.Size(104, 17);
            this.chkAuditUserLogouts.TabIndex = 6;
            this.chkAuditUserLogouts.Text = "Log&outs";
            // 
            // rbAuditUserSelected
            // 
            this.rbAuditUserSelected.Location = new System.Drawing.Point(8, 40);
            this.rbAuditUserSelected.Name = "rbAuditUserSelected";
            this.rbAuditUserSelected.Size = new System.Drawing.Size(292, 17);
            this.rbAuditUserSelected.TabIndex = 24;
            this.rbAuditUserSelected.Text = "Audit selected activities d&one by privileged users";
            this.rbAuditUserSelected.CheckedChanged += new System.EventHandler(this.rbUserSelected_CheckedChanged);
            // 
            // rbAuditUserAll
            // 
            this.rbAuditUserAll.Checked = true;
            this.rbAuditUserAll.Location = new System.Drawing.Point(8, 20);
            this.rbAuditUserAll.Name = "rbAuditUserAll";
            this.rbAuditUserAll.Size = new System.Drawing.Size(260, 17);
            this.rbAuditUserAll.TabIndex = 23;
            this.rbAuditUserAll.TabStop = true;
            this.rbAuditUserAll.Text = "Au&dit all activities done by privileged users";
            this.rbAuditUserAll.CheckedChanged += new System.EventHandler(this.rbUserSelected_CheckedChanged);
            // 
            // checkBox3
            // 
            this.checkBox3.Location = new System.Drawing.Point(592, 432);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(344, 24);
            this.checkBox3.TabIndex = 4;
            this.checkBox3.Text = "Database Modifications (DML) - Insert, Update, Delete, Execute";
            // 
            // tabPageThresholds
            // 
            this.tabPageThresholds.Controls.Add(this._checkAlerts);
            this.tabPageThresholds.Controls.Add(this._checkLogins);
            this.tabPageThresholds.Controls.Add(this._checkLogouts);
            this.tabPageThresholds.Controls.Add(this._checkFailedLogins);
            this.tabPageThresholds.Controls.Add(this._checkDDL);
            this.tabPageThresholds.Controls.Add(this._checkSecurity);
            this.tabPageThresholds.Controls.Add(this._checkAllActivity);
            this.tabPageThresholds.Controls.Add(this._checkPrivUser);
            this.tabPageThresholds.Controls.Add(this.label32);
            this.tabPageThresholds.Controls.Add(this.label30);
            this.tabPageThresholds.Controls.Add(this.label29);
            this.tabPageThresholds.Controls.Add(this.label28);
            this.tabPageThresholds.Controls.Add(this.label27);
            this.tabPageThresholds.Controls.Add(this._cbPrivUserPeriod);
            this.tabPageThresholds.Controls.Add(this._cbAlertsPeriod);
            this.tabPageThresholds.Controls.Add(this._cbLoginsPeriod);
            this.tabPageThresholds.Controls.Add(this._cbLogoutsPeriod);
            this.tabPageThresholds.Controls.Add(this._cbSecurityPeriod);
            this.tabPageThresholds.Controls.Add(this._cbAllActivityPeriod);
            this.tabPageThresholds.Controls.Add(this._cbFailedLoginsPeriod);
            this.tabPageThresholds.Controls.Add(this._cbDDLPeriod);
            this.tabPageThresholds.Controls.Add(this.label25);
            this.tabPageThresholds.Controls.Add(this.label24);
            this.tabPageThresholds.Controls.Add(this.label23);
            this.tabPageThresholds.Controls.Add(this.label17);
            this.tabPageThresholds.Controls.Add(this.label33);
            this.tabPageThresholds.Controls.Add(this.label34);
            this.tabPageThresholds.Controls.Add(this.label15);
            this.tabPageThresholds.Controls.Add(this.label11);
            this.tabPageThresholds.Controls.Add(this._txtAlertsWarning);
            this.tabPageThresholds.Controls.Add(this._txtLoginsWarning);
            this.tabPageThresholds.Controls.Add(this._txtLogoutsWarning);
            this.tabPageThresholds.Controls.Add(this._txtFailedLoginsWarning);
            this.tabPageThresholds.Controls.Add(this._txtDDLWarning);
            this.tabPageThresholds.Controls.Add(this._txtSecurityWarning);
            this.tabPageThresholds.Controls.Add(this._txtAllActivityWarning);
            this.tabPageThresholds.Controls.Add(this._txtPrivUserError);
            this.tabPageThresholds.Controls.Add(this._txtAlertsError);
            this.tabPageThresholds.Controls.Add(this._txtLoginsError);
            this.tabPageThresholds.Controls.Add(this._txtLogoutsError);
            this.tabPageThresholds.Controls.Add(this._txtFailedLoginsError);
            this.tabPageThresholds.Controls.Add(this._txtDDLError);
            this.tabPageThresholds.Controls.Add(this._txtSecurityError);
            this.tabPageThresholds.Controls.Add(this._txtAllActivityError);
            this.tabPageThresholds.Controls.Add(this._txtPrivUserWarning);
            this.tabPageThresholds.Location = new System.Drawing.Point(4, 22);
            this.tabPageThresholds.Name = "tabPageThresholds";
            this.tabPageThresholds.Size = new System.Drawing.Size(532, 462);
            this.tabPageThresholds.TabIndex = 8;
            this.tabPageThresholds.Text = "Auditing Thresholds";
            this.tabPageThresholds.UseVisualStyleBackColor = true;
            // 
            // _checkAlerts
            // 
            this._checkAlerts.AutoSize = true;
            this._checkAlerts.Location = new System.Drawing.Point(437, 56);
            this._checkAlerts.Name = "_checkAlerts";
            this._checkAlerts.Size = new System.Drawing.Size(15, 14);
            this._checkAlerts.TabIndex = 3;
            this._checkAlerts.UseVisualStyleBackColor = true;
            this._checkAlerts.CheckedChanged += new System.EventHandler(this.CheckedChanged_ThresholdEnabled);
            // 
            // _checkLogins
            // 
            this._checkLogins.AutoSize = true;
            this._checkLogins.Location = new System.Drawing.Point(437, 82);
            this._checkLogins.Name = "_checkLogins";
            this._checkLogins.Size = new System.Drawing.Size(15, 14);
            this._checkLogins.TabIndex = 3;
            this._checkLogins.UseVisualStyleBackColor = true;
            this._checkLogins.CheckedChanged += new System.EventHandler(this.CheckedChanged_ThresholdEnabled);
            // 
            // _checkLogouts
            // 
            this._checkLogouts.AutoSize = true;
            this._checkLogouts.Location = new System.Drawing.Point(437, 108);
            this._checkLogouts.Name = "_checkLogouts";
            this._checkLogouts.Size = new System.Drawing.Size(15, 14);
            this._checkLogouts.TabIndex = 3;
            this._checkLogouts.UseVisualStyleBackColor = true;
            this._checkLogouts.CheckedChanged += new System.EventHandler(this.CheckedChanged_ThresholdEnabled);
            // 
            // _checkFailedLogins
            // 
            this._checkFailedLogins.AutoSize = true;
            this._checkFailedLogins.Location = new System.Drawing.Point(437, 134);
            this._checkFailedLogins.Name = "_checkFailedLogins";
            this._checkFailedLogins.Size = new System.Drawing.Size(15, 14);
            this._checkFailedLogins.TabIndex = 7;
            this._checkFailedLogins.UseVisualStyleBackColor = true;
            this._checkFailedLogins.CheckedChanged += new System.EventHandler(this.CheckedChanged_ThresholdEnabled);
            // 
            // _checkDDL
            // 
            this._checkDDL.AutoSize = true;
            this._checkDDL.Location = new System.Drawing.Point(437, 186);
            this._checkDDL.Name = "_checkDDL";
            this._checkDDL.Size = new System.Drawing.Size(15, 14);
            this._checkDDL.TabIndex = 15;
            this._checkDDL.UseVisualStyleBackColor = true;
            this._checkDDL.CheckedChanged += new System.EventHandler(this.CheckedChanged_ThresholdEnabled);
            // 
            // _checkSecurity
            // 
            this._checkSecurity.AutoSize = true;
            this._checkSecurity.Location = new System.Drawing.Point(437, 160);
            this._checkSecurity.Name = "_checkSecurity";
            this._checkSecurity.Size = new System.Drawing.Size(15, 14);
            this._checkSecurity.TabIndex = 11;
            this._checkSecurity.UseVisualStyleBackColor = true;
            this._checkSecurity.CheckedChanged += new System.EventHandler(this.CheckedChanged_ThresholdEnabled);
            // 
            // _checkAllActivity
            // 
            this._checkAllActivity.AutoSize = true;
            this._checkAllActivity.Location = new System.Drawing.Point(437, 242);
            this._checkAllActivity.Name = "_checkAllActivity";
            this._checkAllActivity.Size = new System.Drawing.Size(15, 14);
            this._checkAllActivity.TabIndex = 23;
            this._checkAllActivity.UseVisualStyleBackColor = true;
            this._checkAllActivity.CheckedChanged += new System.EventHandler(this.CheckedChanged_ThresholdEnabled);
            // 
            // _checkPrivUser
            // 
            this._checkPrivUser.AutoSize = true;
            this._checkPrivUser.Location = new System.Drawing.Point(437, 212);
            this._checkPrivUser.Name = "_checkPrivUser";
            this._checkPrivUser.Size = new System.Drawing.Size(15, 14);
            this._checkPrivUser.TabIndex = 19;
            this._checkPrivUser.UseVisualStyleBackColor = true;
            this._checkPrivUser.CheckedChanged += new System.EventHandler(this.CheckedChanged_ThresholdEnabled);
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label32.Location = new System.Drawing.Point(419, 20);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(46, 13);
            this.label32.TabIndex = 28;
            this.label32.Text = "Enabled";
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label30.Location = new System.Drawing.Point(333, 20);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(37, 13);
            this.label30.TabIndex = 2;
            this.label30.Text = "Period";
            // 
            // label29
            // 
            this.label29.Location = new System.Drawing.Point(7, 287);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(499, 75);
            this.label29.TabIndex = 27;
            this.label29.Text = "Auditing thresholds can indicate when event activity is unusually high. If a thre" +
    "shold is exceeded, its status displays on the Activity Report Card tab for the e" +
    "vent category.";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label28.Location = new System.Drawing.Point(241, 20);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(38, 13);
            this.label28.TabIndex = 1;
            this.label28.Text = "Critical";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label27.Location = new System.Drawing.Point(146, 20);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(47, 13);
            this.label27.TabIndex = 0;
            this.label27.Text = "Warning";
            // 
            // _cbPrivUserPeriod
            // 
            this._cbPrivUserPeriod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbPrivUserPeriod.Enabled = false;
            this._cbPrivUserPeriod.FormattingEnabled = true;
            this._cbPrivUserPeriod.Items.AddRange(new object[] {
            "per hour",
            "per day"});
            this._cbPrivUserPeriod.Location = new System.Drawing.Point(318, 209);
            this._cbPrivUserPeriod.MaxDropDownItems = 2;
            this._cbPrivUserPeriod.Name = "_cbPrivUserPeriod";
            this._cbPrivUserPeriod.Size = new System.Drawing.Size(67, 21);
            this._cbPrivUserPeriod.TabIndex = 18;
            // 
            // _cbAlertsPeriod
            // 
            this._cbAlertsPeriod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbAlertsPeriod.Enabled = false;
            this._cbAlertsPeriod.FormattingEnabled = true;
            this._cbAlertsPeriod.Items.AddRange(new object[] {
            "per hour",
            "per day"});
            this._cbAlertsPeriod.Location = new System.Drawing.Point(318, 53);
            this._cbAlertsPeriod.MaxDropDownItems = 2;
            this._cbAlertsPeriod.Name = "_cbAlertsPeriod";
            this._cbAlertsPeriod.Size = new System.Drawing.Size(67, 21);
            this._cbAlertsPeriod.TabIndex = 2;
            // 
            // _cbLoginsPeriod
            // 
            this._cbLoginsPeriod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbLoginsPeriod.Enabled = false;
            this._cbLoginsPeriod.FormattingEnabled = true;
            this._cbLoginsPeriod.Items.AddRange(new object[] {
            "per hour",
            "per day"});
            this._cbLoginsPeriod.Location = new System.Drawing.Point(318, 79);
            this._cbLoginsPeriod.MaxDropDownItems = 2;
            this._cbLoginsPeriod.Name = "_cbLoginsPeriod";
            this._cbLoginsPeriod.Size = new System.Drawing.Size(67, 21);
            this._cbLoginsPeriod.TabIndex = 18;
            // 
            // _cbLogoutsPeriod
            // 
            this._cbLogoutsPeriod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbLogoutsPeriod.Enabled = false;
            this._cbLogoutsPeriod.FormattingEnabled = true;
            this._cbLogoutsPeriod.Items.AddRange(new object[] {
            "per hour",
            "per day"});
            this._cbLogoutsPeriod.Location = new System.Drawing.Point(318, 105);
            this._cbLogoutsPeriod.MaxDropDownItems = 2;
            this._cbLogoutsPeriod.Name = "_cbLogoutsPeriod";
            this._cbLogoutsPeriod.Size = new System.Drawing.Size(67, 21);
            this._cbLogoutsPeriod.TabIndex = 18;
            // 
            // _cbSecurityPeriod
            // 
            this._cbSecurityPeriod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbSecurityPeriod.Enabled = false;
            this._cbSecurityPeriod.FormattingEnabled = true;
            this._cbSecurityPeriod.Items.AddRange(new object[] {
            "per hour",
            "per day"});
            this._cbSecurityPeriod.Location = new System.Drawing.Point(318, 157);
            this._cbSecurityPeriod.MaxDropDownItems = 2;
            this._cbSecurityPeriod.Name = "_cbSecurityPeriod";
            this._cbSecurityPeriod.Size = new System.Drawing.Size(67, 21);
            this._cbSecurityPeriod.TabIndex = 10;
            // 
            // _cbAllActivityPeriod
            // 
            this._cbAllActivityPeriod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbAllActivityPeriod.Enabled = false;
            this._cbAllActivityPeriod.FormattingEnabled = true;
            this._cbAllActivityPeriod.Items.AddRange(new object[] {
            "per hour",
            "per day"});
            this._cbAllActivityPeriod.Location = new System.Drawing.Point(318, 235);
            this._cbAllActivityPeriod.MaxDropDownItems = 2;
            this._cbAllActivityPeriod.Name = "_cbAllActivityPeriod";
            this._cbAllActivityPeriod.Size = new System.Drawing.Size(67, 21);
            this._cbAllActivityPeriod.TabIndex = 22;
            // 
            // _cbFailedLoginsPeriod
            // 
            this._cbFailedLoginsPeriod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbFailedLoginsPeriod.Enabled = false;
            this._cbFailedLoginsPeriod.FormattingEnabled = true;
            this._cbFailedLoginsPeriod.Items.AddRange(new object[] {
            "per hour",
            "per day"});
            this._cbFailedLoginsPeriod.Location = new System.Drawing.Point(318, 131);
            this._cbFailedLoginsPeriod.MaxDropDownItems = 2;
            this._cbFailedLoginsPeriod.Name = "_cbFailedLoginsPeriod";
            this._cbFailedLoginsPeriod.Size = new System.Drawing.Size(67, 21);
            this._cbFailedLoginsPeriod.TabIndex = 6;
            // 
            // _cbDDLPeriod
            // 
            this._cbDDLPeriod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbDDLPeriod.Enabled = false;
            this._cbDDLPeriod.FormattingEnabled = true;
            this._cbDDLPeriod.Items.AddRange(new object[] {
            "per hour",
            "per day"});
            this._cbDDLPeriod.Location = new System.Drawing.Point(318, 183);
            this._cbDDLPeriod.MaxDropDownItems = 2;
            this._cbDDLPeriod.Name = "_cbDDLPeriod";
            this._cbDDLPeriod.Size = new System.Drawing.Size(67, 21);
            this._cbDDLPeriod.TabIndex = 14;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label25.Location = new System.Drawing.Point(32, 239);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(77, 13);
            this.label25.TabIndex = 23;
            this.label25.Text = "Overall Activity";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label24.Location = new System.Drawing.Point(32, 161);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(45, 13);
            this.label24.TabIndex = 19;
            this.label24.Text = "Security";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label23.Location = new System.Drawing.Point(32, 187);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(29, 13);
            this.label23.TabIndex = 15;
            this.label23.Text = "DDL";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(32, 135);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(69, 13);
            this.label17.TabIndex = 11;
            this.label17.Text = "Failed Logins";
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label33.Location = new System.Drawing.Point(32, 83);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(38, 13);
            this.label33.TabIndex = 11;
            this.label33.Text = "Logins";
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label34.Location = new System.Drawing.Point(32, 109);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(45, 13);
            this.label34.TabIndex = 11;
            this.label34.Text = "Logouts";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(32, 57);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(64, 13);
            this.label15.TabIndex = 7;
            this.label15.Text = "Event Alerts";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(32, 213);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(78, 13);
            this.label11.TabIndex = 3;
            this.label11.Text = "Privileged User";
            // 
            // _txtAlertsWarning
            // 
            this._txtAlertsWarning.Enabled = false;
            this._txtAlertsWarning.Location = new System.Drawing.Point(136, 53);
            this._txtAlertsWarning.Name = "_txtAlertsWarning";
            this._txtAlertsWarning.Size = new System.Drawing.Size(66, 20);
            this._txtAlertsWarning.TabIndex = 0;
            // 
            // _txtLoginsWarning
            // 
            this._txtLoginsWarning.Enabled = false;
            this._txtLoginsWarning.Location = new System.Drawing.Point(136, 79);
            this._txtLoginsWarning.Name = "_txtLoginsWarning";
            this._txtLoginsWarning.Size = new System.Drawing.Size(66, 20);
            this._txtLoginsWarning.TabIndex = 0;
            // 
            // _txtLogoutsWarning
            // 
            this._txtLogoutsWarning.Enabled = false;
            this._txtLogoutsWarning.Location = new System.Drawing.Point(136, 105);
            this._txtLogoutsWarning.Name = "_txtLogoutsWarning";
            this._txtLogoutsWarning.Size = new System.Drawing.Size(66, 20);
            this._txtLogoutsWarning.TabIndex = 0;
            // 
            // _txtFailedLoginsWarning
            // 
            this._txtFailedLoginsWarning.Enabled = false;
            this._txtFailedLoginsWarning.Location = new System.Drawing.Point(136, 131);
            this._txtFailedLoginsWarning.Name = "_txtFailedLoginsWarning";
            this._txtFailedLoginsWarning.Size = new System.Drawing.Size(66, 20);
            this._txtFailedLoginsWarning.TabIndex = 4;
            // 
            // _txtDDLWarning
            // 
            this._txtDDLWarning.Enabled = false;
            this._txtDDLWarning.Location = new System.Drawing.Point(136, 183);
            this._txtDDLWarning.Name = "_txtDDLWarning";
            this._txtDDLWarning.Size = new System.Drawing.Size(66, 20);
            this._txtDDLWarning.TabIndex = 12;
            // 
            // _txtSecurityWarning
            // 
            this._txtSecurityWarning.Enabled = false;
            this._txtSecurityWarning.Location = new System.Drawing.Point(136, 157);
            this._txtSecurityWarning.Name = "_txtSecurityWarning";
            this._txtSecurityWarning.Size = new System.Drawing.Size(66, 20);
            this._txtSecurityWarning.TabIndex = 8;
            // 
            // _txtAllActivityWarning
            // 
            this._txtAllActivityWarning.Enabled = false;
            this._txtAllActivityWarning.Location = new System.Drawing.Point(136, 235);
            this._txtAllActivityWarning.Name = "_txtAllActivityWarning";
            this._txtAllActivityWarning.Size = new System.Drawing.Size(66, 20);
            this._txtAllActivityWarning.TabIndex = 20;
            // 
            // _txtPrivUserError
            // 
            this._txtPrivUserError.Enabled = false;
            this._txtPrivUserError.Location = new System.Drawing.Point(227, 209);
            this._txtPrivUserError.Name = "_txtPrivUserError";
            this._txtPrivUserError.Size = new System.Drawing.Size(66, 20);
            this._txtPrivUserError.TabIndex = 17;
            // 
            // _txtAlertsError
            // 
            this._txtAlertsError.Enabled = false;
            this._txtAlertsError.Location = new System.Drawing.Point(227, 53);
            this._txtAlertsError.Name = "_txtAlertsError";
            this._txtAlertsError.Size = new System.Drawing.Size(66, 20);
            this._txtAlertsError.TabIndex = 1;
            // 
            // _txtLoginsError
            // 
            this._txtLoginsError.Enabled = false;
            this._txtLoginsError.Location = new System.Drawing.Point(227, 79);
            this._txtLoginsError.Name = "_txtLoginsError";
            this._txtLoginsError.Size = new System.Drawing.Size(66, 20);
            this._txtLoginsError.TabIndex = 1;
            // 
            // _txtLogoutsError
            // 
            this._txtLogoutsError.Enabled = false;
            this._txtLogoutsError.Location = new System.Drawing.Point(227, 105);
            this._txtLogoutsError.Name = "_txtLogoutsError";
            this._txtLogoutsError.Size = new System.Drawing.Size(66, 20);
            this._txtLogoutsError.TabIndex = 1;
            // 
            // _txtFailedLoginsError
            // 
            this._txtFailedLoginsError.Enabled = false;
            this._txtFailedLoginsError.Location = new System.Drawing.Point(227, 131);
            this._txtFailedLoginsError.Name = "_txtFailedLoginsError";
            this._txtFailedLoginsError.Size = new System.Drawing.Size(66, 20);
            this._txtFailedLoginsError.TabIndex = 5;
            // 
            // _txtDDLError
            // 
            this._txtDDLError.Enabled = false;
            this._txtDDLError.Location = new System.Drawing.Point(227, 183);
            this._txtDDLError.Name = "_txtDDLError";
            this._txtDDLError.Size = new System.Drawing.Size(66, 20);
            this._txtDDLError.TabIndex = 13;
            // 
            // _txtSecurityError
            // 
            this._txtSecurityError.Enabled = false;
            this._txtSecurityError.Location = new System.Drawing.Point(227, 157);
            this._txtSecurityError.Name = "_txtSecurityError";
            this._txtSecurityError.Size = new System.Drawing.Size(66, 20);
            this._txtSecurityError.TabIndex = 9;
            // 
            // _txtAllActivityError
            // 
            this._txtAllActivityError.Enabled = false;
            this._txtAllActivityError.Location = new System.Drawing.Point(227, 235);
            this._txtAllActivityError.Name = "_txtAllActivityError";
            this._txtAllActivityError.Size = new System.Drawing.Size(66, 20);
            this._txtAllActivityError.TabIndex = 21;
            // 
            // _txtPrivUserWarning
            // 
            this._txtPrivUserWarning.Enabled = false;
            this._txtPrivUserWarning.Location = new System.Drawing.Point(136, 209);
            this._txtPrivUserWarning.Name = "_txtPrivUserWarning";
            this._txtPrivUserWarning.Size = new System.Drawing.Size(66, 20);
            this._txtPrivUserWarning.TabIndex = 16;
            // 
            // tabPageAdvanced
            // 
            this.tabPageAdvanced.Controls.Add(this.groupBox1);
            this.tabPageAdvanced.Controls.Add(this.groupBox3);
            this.tabPageAdvanced.Location = new System.Drawing.Point(4, 22);
            this.tabPageAdvanced.Name = "tabPageAdvanced";
            this.tabPageAdvanced.Size = new System.Drawing.Size(532, 462);
            this.tabPageAdvanced.TabIndex = 4;
            this.tabPageAdvanced.Text = "Advanced";
            this.tabPageAdvanced.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.labelSqlReportCharacters);
            this.groupBox1.Controls.Add(this.textReportLimitSQL);
            this.groupBox1.Controls.Add(this.labelSqlReport);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.textLimitSQL);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.radioLimitSQL);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.radioUnlimitedSQL);
            this.groupBox1.Location = new System.Drawing.Point(12, 163);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(508, 251);
            this.groupBox1.TabIndex = 33;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "SQL Statement Limit";
            // 
            // labelSqlReportCharacters
            // 
            this.labelSqlReportCharacters.Location = new System.Drawing.Point(280, 214);
            this.labelSqlReportCharacters.Name = "labelSqlReportCharacters";
            this.labelSqlReportCharacters.Size = new System.Drawing.Size(60, 18);
            this.labelSqlReportCharacters.TabIndex = 36;
            this.labelSqlReportCharacters.Text = "characters.";
            this.labelSqlReportCharacters.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textReportLimitSQL
            // 
            this.textReportLimitSQL.Enabled = false;
            this.textReportLimitSQL.Location = new System.Drawing.Point(238, 214);
            this.textReportLimitSQL.MaxLength = 5;
            this.textReportLimitSQL.Name = "textReportLimitSQL";
            this.textReportLimitSQL.Size = new System.Drawing.Size(40, 20);
            this.textReportLimitSQL.TabIndex = 35;
            // 
            // labelSqlReport
            // 
            this.labelSqlReport.Location = new System.Drawing.Point(25, 214);
            this.labelSqlReport.Name = "labelSqlReport";
            this.labelSqlReport.Size = new System.Drawing.Size(221, 18);
            this.labelSqlReport.TabIndex = 34;
            this.labelSqlReport.Text = "For Reports, SQL text will be truncated after";
            this.labelSqlReport.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(6, 115);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(494, 32);
            this.label7.TabIndex = 33;
            this.label7.Text = "Use the following option to specify the maximum size of stored SQL statements. St" +
    "atements exceeding this maximum are truncated.";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(252, 187);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(60, 18);
            this.label6.TabIndex = 9;
            this.label6.Text = "characters";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textLimitSQL
            // 
            this.textLimitSQL.Location = new System.Drawing.Point(210, 187);
            this.textLimitSQL.MaxLength = 5;
            this.textLimitSQL.Name = "textLimitSQL";
            this.textLimitSQL.Size = new System.Drawing.Size(40, 20);
            this.textLimitSQL.TabIndex = 8;
            this.textLimitSQL.Text = "128";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(6, 65);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(494, 40);
            this.label5.TabIndex = 31;
            this.label5.Text = resources.GetString("label5.Text");
            // 
            // radioLimitSQL
            // 
            this.radioLimitSQL.Checked = true;
            this.radioLimitSQL.Location = new System.Drawing.Point(9, 184);
            this.radioLimitSQL.Name = "radioLimitSQL";
            this.radioLimitSQL.Size = new System.Drawing.Size(212, 24);
            this.radioLimitSQL.TabIndex = 1;
            this.radioLimitSQL.TabStop = true;
            this.radioLimitSQL.Text = "Truncate stored SQL statements after ";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(6, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(494, 40);
            this.label4.TabIndex = 30;
            this.label4.Text = resources.GetString("label4.Text");
            // 
            // radioUnlimitedSQL
            // 
            this.radioUnlimitedSQL.Location = new System.Drawing.Point(9, 159);
            this.radioUnlimitedSQL.Name = "radioUnlimitedSQL";
            this.radioUnlimitedSQL.Size = new System.Drawing.Size(240, 16);
            this.radioUnlimitedSQL.TabIndex = 0;
            this.radioUnlimitedSQL.Text = "Store entire text of SQL statements";
            this.radioUnlimitedSQL.CheckedChanged += new System.EventHandler(this.radioUnlimitedSQL_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label16);
            this.groupBox3.Controls.Add(this.radioGrantAll);
            this.groupBox3.Controls.Add(this.radioGrantEventsOnly);
            this.groupBox3.Controls.Add(this.radioDeny);
            this.groupBox3.Location = new System.Drawing.Point(12, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(508, 143);
            this.groupBox3.TabIndex = 31;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Default Database Permissions";
            // 
            // label16
            // 
            this.label16.Location = new System.Drawing.Point(6, 16);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(496, 28);
            this.label16.TabIndex = 32;
            this.label16.Text = "Select the default level of access you want to grant users on the database contai" +
    "ning  audit data for this SQL Server instance.";
            // 
            // radioGrantAll
            // 
            this.radioGrantAll.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.radioGrantAll.Checked = true;
            this.radioGrantAll.Location = new System.Drawing.Point(9, 47);
            this.radioGrantAll.Name = "radioGrantAll";
            this.radioGrantAll.Size = new System.Drawing.Size(460, 17);
            this.radioGrantAll.TabIndex = 34;
            this.radioGrantAll.TabStop = true;
            this.radioGrantAll.Text = "Grant right to read events and their associated SQL statements .";
            // 
            // radioGrantEventsOnly
            // 
            this.radioGrantEventsOnly.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.radioGrantEventsOnly.Location = new System.Drawing.Point(9, 69);
            this.radioGrantEventsOnly.Name = "radioGrantEventsOnly";
            this.radioGrantEventsOnly.Size = new System.Drawing.Size(464, 32);
            this.radioGrantEventsOnly.TabIndex = 32;
            this.radioGrantEventsOnly.Text = "Grant right to read events only - To allow users to view the associated SQL state" +
    "ments, you will need to explicictly grant users read access to the database.";
            // 
            // radioDeny
            // 
            this.radioDeny.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.radioDeny.Location = new System.Drawing.Point(9, 107);
            this.radioDeny.Name = "radioDeny";
            this.radioDeny.Size = new System.Drawing.Size(476, 30);
            this.radioDeny.TabIndex = 9;
            this.radioDeny.Text = "Deny read access by default - To allow users to view events and the associated SQ" +
    "L, you will need to explicitly grant users read access to the database.";
            this.radioDeny.TextAlign = System.Drawing.ContentAlignment.TopLeft;
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
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.Gray;
            this.panel1.Location = new System.Drawing.Point(4, 526);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(532, 1);
            this.panel1.TabIndex = 7;
            // 
            // btnRestoreToIderaDefaultSettings
            // 
            this.btnRestoreToIderaDefaultSettings.Location = new System.Drawing.Point(4, 536);
            this.btnRestoreToIderaDefaultSettings.Name = "btnRestoreToIderaDefaultSettings";
            this.btnRestoreToIderaDefaultSettings.Size = new System.Drawing.Size(172, 23);
            this.btnRestoreToIderaDefaultSettings.TabIndex = 39;
            this.btnRestoreToIderaDefaultSettings.Text = "&Reset to Idera Default Settings";
            this.btnRestoreToIderaDefaultSettings.UseVisualStyleBackColor = true;
            this.btnRestoreToIderaDefaultSettings.Click += new System.EventHandler(this.btnRestoreToIderaDefaultSettings_Click);
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(12, 491);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(524, 32);
            this.label8.TabIndex = 40;
            this.label8.Text = "The settings on this page will affect all databases on this server. Please use ca" +
    "ution when selecting these settings as it could potentially create a lot of audi" +
    "t data.";
            // 
            // Form_ServerDefaultPropertise
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(542, 566);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.btnRestoreToIderaDefaultSettings);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.tabProperties);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form_ServerDefaultPropertise";
            this.ShowInTaskbar = false;
            this.Text = "SQL Server Default Audit Settings"; //SQLCM-5654
            this.Load += new System.EventHandler(this.Form_ServerDefaultProperties_Load);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.Form_ServerProperties_HelpRequested);
            this.tabProperties.ResumeLayout(false);
            this.tabPageAuditSettings.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.grpAuditResults.ResumeLayout(false);
            this.grpAuditActivity.ResumeLayout(false);
            this.tabTrustedUsers.ResumeLayout(false);
            this.pnlTrustedUsers.ResumeLayout(false);
            this.pnlTrustedUsers.PerformLayout();
            this.tabPageUsers.ResumeLayout(false);
            this.tabPageUsers.PerformLayout();
            this.grpPrivilegedUserActivity.ResumeLayout(false);
            this.grpAuditUserActivity.ResumeLayout(false);
            this.grpAuditUserActivity.PerformLayout();
            this.tabPageThresholds.ResumeLayout(false);
            this.tabPageThresholds.PerformLayout();
            this.tabPageAdvanced.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.GroupBox grpPrivilegedUserActivity;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton radioDeny;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.RadioButton radioGrantAll;
        private System.Windows.Forms.RadioButton radioGrantEventsOnly;
        private System.Windows.Forms.TabPage tabPageThresholds;
        private System.Windows.Forms.CheckBox chkAuditUserAdmin;
        private System.Windows.Forms.CheckBox chkAuditAdmin;
        private System.Windows.Forms.GroupBox grpAuditResults;
        private System.Windows.Forms.RadioButton rbAuditFailedOnly;
        private System.Windows.Forms.RadioButton rbAuditSuccessfulOnly;
        private System.Windows.Forms.CheckBox _cbFilterAccessCheck;
        private System.Windows.Forms.CheckBox _cbUserFilterAccessCheck;
        private System.Windows.Forms.RadioButton _rbUserAuditPassed;
        private System.Windows.Forms.RadioButton _rbUserAuditFailed;
        private System.Windows.Forms.CheckBox chkAuditUserDefined;
        private System.Windows.Forms.CheckBox chkAuditUserUserDefined;
        private System.Windows.Forms.Button btnAddPriv;
        private System.Windows.Forms.Button btnRemovePriv;
        private System.Windows.Forms.ListView lstPrivilegedUsers;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TabControl tabProperties;
        private System.Windows.Forms.CheckBox checkBox5;
        private System.Windows.Forms.MenuItem menuDatabaseAdd;
        private System.Windows.Forms.MenuItem menuDatabaseRemove;
        private System.Windows.Forms.MenuItem menuItem3;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem menuDatabaseEnable;
        private System.Windows.Forms.MenuItem menuDatabaseDisable;
        private System.Windows.Forms.MenuItem menuDatabaseProperties;
        private System.Windows.Forms.CheckBox chkAuditDDL;
        private System.Windows.Forms.CheckBox chkAuditFailedLogins;
        private System.Windows.Forms.CheckBox chkAuditLogins;
        private System.Windows.Forms.CheckBox chkAuditLogouts;
        private System.Windows.Forms.TabPage tabPageAdvanced;
        private System.Windows.Forms.TabPage tabPageAuditSettings;
        private System.Windows.Forms.CheckBox chkAuditSecurity;
        private System.Windows.Forms.TabPage tabPageUsers;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.GroupBox grpAuditActivity;
        private System.Windows.Forms.GroupBox grpAuditUserActivity;
        private System.Windows.Forms.CheckBox chkUserCaptureSQL;
        private System.Windows.Forms.CheckBox chkAuditUserDML;
        private System.Windows.Forms.CheckBox chkAuditUserSELECT;
        private System.Windows.Forms.CheckBox chkAuditUserSecurity;
        private System.Windows.Forms.CheckBox chkAuditUserDDL;
        private System.Windows.Forms.CheckBox chkAuditUserFailedLogins;
        private System.Windows.Forms.CheckBox chkAuditUserLogins;
        private System.Windows.Forms.CheckBox chkAuditUserLogouts;
        private System.Windows.Forms.RadioButton rbAuditUserAll;
        private System.Windows.Forms.RadioButton rbAuditUserSelected;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textLimitSQL;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RadioButton radioLimitSQL;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton radioUnlimitedSQL;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label17;
        //start sqlcm 5.6-5363
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.Label label34;
        //end sqlcm 5.6 -5363
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label11;
        private Controls.NumericTextBox _txtAlertsWarning;
        //start sqlcm 5.6-5363
        private Controls.NumericTextBox _txtLoginsWarning;
        private Controls.NumericTextBox _txtLogoutsWarning;
        //end sqlcm 5.6-5363
        private Controls.NumericTextBox _txtFailedLoginsWarning;
        private Controls.NumericTextBox _txtDDLWarning;
        private Controls.NumericTextBox _txtSecurityWarning;
        private Controls.NumericTextBox _txtAllActivityWarning;
        private Controls.NumericTextBox _txtPrivUserError;
        private Controls.NumericTextBox _txtAlertsError;
        //start sqlcm 5.6 -5363
        private Controls.NumericTextBox _txtLoginsError;
        private Controls.NumericTextBox _txtLogoutsError;
        //end sqlcm 5.6-5363
        private Controls.NumericTextBox _txtFailedLoginsError;
        private Controls.NumericTextBox _txtDDLError;
        private Controls.NumericTextBox _txtSecurityError;
        private Controls.NumericTextBox _txtAllActivityError;
        private Controls.NumericTextBox _txtPrivUserWarning;
        private System.Windows.Forms.ComboBox _cbPrivUserPeriod;
        private System.Windows.Forms.ComboBox _cbAlertsPeriod;
        //start sqlcm 5.6 -5363
        private System.Windows.Forms.ComboBox _cbLoginsPeriod;
        private System.Windows.Forms.ComboBox _cbLogoutsPeriod;
        //end sqlcm 5.6 - 5363
        private System.Windows.Forms.ComboBox _cbSecurityPeriod;
        private System.Windows.Forms.ComboBox _cbAllActivityPeriod;
        private System.Windows.Forms.ComboBox _cbFailedLoginsPeriod;
        private System.Windows.Forms.ComboBox _cbDDLPeriod;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.CheckBox _checkAlerts;
        //start sqlcm 5.6 -5363
        private System.Windows.Forms.CheckBox _checkLogins;
        private System.Windows.Forms.CheckBox _checkLogouts;
        //end sqlcm 5.6 - 5363
        private System.Windows.Forms.CheckBox _checkFailedLogins;
        private System.Windows.Forms.CheckBox _checkDDL;
        private System.Windows.Forms.CheckBox _checkSecurity;
        private System.Windows.Forms.CheckBox _checkAllActivity;
        private System.Windows.Forms.CheckBox _checkPrivUser;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox chkUserCaptureTrans;
        private System.Windows.Forms.CheckBox chkUserCaptureDDL;
        private System.Windows.Forms.Label labelSqlReportCharacters;
        private System.Windows.Forms.TextBox textReportLimitSQL;
        private System.Windows.Forms.Label labelSqlReport;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton radioAuditLogs;
        private System.Windows.Forms.RadioButton radioXEvents;
        private System.Windows.Forms.RadioButton radioTrace;
        private System.Windows.Forms.Label lblGreyingNotes;
        //Trusted users // v5.6 SQLCM-5373
        private System.Windows.Forms.TabPage tabTrustedUsers;
        private System.Windows.Forms.ListView lstTrustedUsers;
        private System.Windows.Forms.Label lblTrustedUserStatus;
        private System.Windows.Forms.Panel pnlTrustedUsers;
        private System.Windows.Forms.LinkLabel lnkTrustedUserHelp;
        private System.Windows.Forms.Label labelTrustedUsers;
        private System.Windows.Forms.ColumnHeader columnTrustedUserHeader2;
        private System.Windows.Forms.Button btnRemoveTrustedUser;
        private System.Windows.Forms.Button btnAddTrustedUser;
        private System.Windows.Forms.Label __lblTrustedUserTxt;
        private System.Windows.Forms.Button btnRestoreToIderaDefaultSettings;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtBxTrustedUserName;
        private System.Windows.Forms.TextBox txtBxPrivilegedUser;
        private System.Windows.Forms.ComboBox cmbBxPrivilegedUser;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label8;
    };
}