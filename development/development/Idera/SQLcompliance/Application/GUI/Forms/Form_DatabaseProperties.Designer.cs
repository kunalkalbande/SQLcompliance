namespace Idera.SQLcompliance.Application.GUI.Forms
{
   partial class Form_DatabaseProperties
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

            System.Windows.Forms.Label @__lblTrustedUserTxt;
            System.Windows.Forms.Label lblTrustedUserDisableNote;// SQLCM-5373
            System.Windows.Forms.Label label3;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_DatabaseProperties));
            System.Windows.Forms.ColumnHeader _columnTableName;
            System.Windows.Forms.ColumnHeader _columnMaxRows;
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label4;
            System.Windows.Forms.Label label5;
            System.Windows.Forms.ColumnHeader columnHeader3;
            this._btnCancel = new System.Windows.Forms.Button();
            this._btnOK = new System.Windows.Forms.Button();
            this._tabControl = new System.Windows.Forms.TabControl();
            this._tabGeneral = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblServer = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.txtServer = new System.Windows.Forms.TextBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.grpStatus = new System.Windows.Forms.GroupBox();
            this.txtTimeEnabledModified = new System.Windows.Forms.TextBox();
            this.lblTimeEnabledModified = new System.Windows.Forms.Label();
            this.txtTimeLastModified = new System.Windows.Forms.TextBox();
            this.lblTimeLastModified = new System.Windows.Forms.Label();
            this.txtTimeCreated = new System.Windows.Forms.TextBox();
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.lblTimeCreated = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this._tabAuditSettings = new System.Windows.Forms.TabPage();
            this.chkDBCaptureDDL = new System.Windows.Forms.CheckBox();
            this._chkCaptureTrans = new System.Windows.Forms.CheckBox();
            this._chkCaptureSQL = new System.Windows.Forms.CheckBox();
            this.grpAuditResults = new System.Windows.Forms.GroupBox();
            this._rbFailed = new System.Windows.Forms.RadioButton();
            this._rbPassed = new System.Windows.Forms.RadioButton();
            this._cbFilterOnAccess = new System.Windows.Forms.CheckBox();
            this._grpAuditActivity = new System.Windows.Forms.GroupBox();
            this._chkAuditAdmin = new System.Windows.Forms.CheckBox();
            this._chkAuditDML = new System.Windows.Forms.CheckBox();
            this._chkAuditSELECT = new System.Windows.Forms.CheckBox();
            this._chkAuditSecurity = new System.Windows.Forms.CheckBox();
            this._chkAuditDDL = new System.Windows.Forms.CheckBox();
            this._tabFilters = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.radioSelectedDML = new System.Windows.Forms.RadioButton();
            this.radioAllDML = new System.Windows.Forms.RadioButton();
            this.grpUserObjects = new System.Windows.Forms.GroupBox();
            this._chkAuditSystemTables = new System.Windows.Forms.CheckBox();
            this._chkAuditOther = new System.Windows.Forms.CheckBox();
            this._chkAuditStoredProcedures = new System.Windows.Forms.CheckBox();
            this.grpUserTables = new System.Windows.Forms.GroupBox();
            this.listTables = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.rbDontAuditUserTables = new System.Windows.Forms.RadioButton();
            this._btnRemove = new System.Windows.Forms.Button();
            this._btnAddTable = new System.Windows.Forms.Button();
            this.rbSelectedUserTables = new System.Windows.Forms.RadioButton();
            this.rbAllUserTables = new System.Windows.Forms.RadioButton();
            this._tabBeforeAfter = new System.Windows.Forms.TabPage();
            this._pnlBeforeAfter = new System.Windows.Forms.Panel();
            this._gbCLR = new System.Windows.Forms.GroupBox();
            this._btnEnableCLR = new System.Windows.Forms.Button();
            this._lblClrStatus = new System.Windows.Forms.Label();
            this._pbClrStatus = new System.Windows.Forms.PictureBox();
            this._btnEditBATable = new System.Windows.Forms.Button();
            this._btnRemoveBATable = new System.Windows.Forms.Button();
            this._btnAddBATable = new System.Windows.Forms.Button();
            this._lvBeforeAfterTables = new System.Windows.Forms.ListView();
            this._columnColumnNames = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this._lblBeforeAfterStatus = new System.Windows.Forms.Label();
            this._tabSensitiveColumns = new System.Windows.Forms.TabPage();
            this._pnlSensitiveColumns = new System.Windows.Forms.Panel();
            this._btnAddSCDataSet = new System.Windows.Forms.Button();
            this._lvSCTables = new System.Windows.Forms.ListView();
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this._btnEditSCTable = new System.Windows.Forms.Button();
            this._btnAddSCTable = new System.Windows.Forms.Button();
            this._btnRemoveSCTable = new System.Windows.Forms.Button();
            this._lblSCStatus = new System.Windows.Forms.Label();
            this._tabTrustedUsers = new System.Windows.Forms.TabPage();
            this._pnlTrustedUsers = new System.Windows.Forms.Panel();
            this.label19 = new System.Windows.Forms.Label();
            this._btnAddUser = new System.Windows.Forms.Button();
            this._lnkTrustedUserHelp = new System.Windows.Forms.LinkLabel();
            this._btnRemoveUser = new System.Windows.Forms.Button();
            this._lstTrustedUsers = new System.Windows.Forms.ListView();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this._lblTrustedUserStatus = new System.Windows.Forms.Label();
            this._tabPrivilegedUser = new System.Windows.Forms.TabPage();
            this.lstPrivilegedUsers = new System.Windows.Forms.ListView();
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnRemovePriv = new System.Windows.Forms.Button();
            this.btnAddPriv = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.labelPreSelectedPrivilegedUsers = new System.Windows.Forms.Label();
            this.lstPreSelectedPrivilegedUsers = new System.Windows.Forms.ListView();
            this.grpPrivilegedUserActivity = new System.Windows.Forms.GroupBox();
            this.grpAuditUserActivity = new System.Windows.Forms.GroupBox();
            // SQLCM-5375-6.1.4.3-Capture Logout Events at Server level	
            this.chkAuditUserLogouts = new System.Windows.Forms.CheckBox();
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
            this.rbAuditUserSelected = new System.Windows.Forms.RadioButton();
            this.rbAuditUserAll = new System.Windows.Forms.RadioButton();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.linkLblHelpBestPractices = new System.Windows.Forms.LinkLabel();
            @__lblTrustedUserTxt = new System.Windows.Forms.Label();
            lblTrustedUserDisableNote = new System.Windows.Forms.Label(); // v5.6 SQLCM-5373
            label3 = new System.Windows.Forms.Label();
            _columnTableName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            _columnMaxRows = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            label2 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            //SQlCM-5747 v5.6
            this._btnConfigureSC = new System.Windows.Forms.Button();
            this._tabControl.SuspendLayout();
            this._tabGeneral.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.grpStatus.SuspendLayout();
            this._tabAuditSettings.SuspendLayout();
            this.grpAuditResults.SuspendLayout();
            this._grpAuditActivity.SuspendLayout();
            this._tabFilters.SuspendLayout();
            this.grpUserObjects.SuspendLayout();
            this.grpUserTables.SuspendLayout();
            this._tabBeforeAfter.SuspendLayout();
            this._pnlBeforeAfter.SuspendLayout();
            this._gbCLR.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._pbClrStatus)).BeginInit();
            this._tabSensitiveColumns.SuspendLayout();
            this._pnlSensitiveColumns.SuspendLayout();
            this._tabTrustedUsers.SuspendLayout();
            this._pnlTrustedUsers.SuspendLayout();
            this._tabPrivilegedUser.SuspendLayout();
            this.grpPrivilegedUserActivity.SuspendLayout();
            this.grpAuditUserActivity.SuspendLayout();
            this.SuspendLayout();
            // 
            // __lblTrustedUserTxt
            // 
            @__lblTrustedUserTxt.AutoSize = true;
            @__lblTrustedUserTxt.Location = new System.Drawing.Point(8, 279);
            @__lblTrustedUserTxt.Name = "__lblTrustedUserTxt";
            @__lblTrustedUserTxt.Size = new System.Drawing.Size(254, 13);
            @__lblTrustedUserTxt.TabIndex = 28;
            @__lblTrustedUserTxt.Text = "Specify the logins or roles you trust on this database.";

            // v5.6 SQLCM-5373
            // 
            // __lblTrustedUserTxt
            // 
            lblTrustedUserDisableNote.AutoSize = true;
            lblTrustedUserDisableNote.Location = new System.Drawing.Point(8, 340);
            lblTrustedUserDisableNote.Name = "lblTrustedUserDisableNote";
            lblTrustedUserDisableNote.Size = new System.Drawing.Size(254, 26);
            lblTrustedUserDisableNote.MaximumSize = new System.Drawing.Size(471, 0);
            lblTrustedUserDisableNote.TabIndex = 28;
            lblTrustedUserDisableNote.Text = "Note: Trusted Users set at the Server Level are displayed in the list above as grayed out. Trusted Users set at the Server Level cannot be removed from the Database Level configuration.";
            // 
            // label3
            // 
            label3.Location = new System.Drawing.Point(8, 225);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(427, 63);
            label3.TabIndex = 12;
            label3.Text = resources.GetString("label3.Text");
            // 
            // _columnTableName
            // 
            _columnTableName.Text = "Table Name";
            _columnTableName.Width = 169;
            // 
            // _columnMaxRows
            // 
            _columnMaxRows.Text = "Maximum Rows";
            _columnMaxRows.Width = 94;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(8, 14);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(204, 13);
            label2.TabIndex = 7;
            label2.Text = "Tables audited for DML Before-After data:";
            // 
            // label4
            // 
            label4.Location = new System.Drawing.Point(7, 222);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(508, 174);
            label4.TabIndex = 18;
            label4.Text = resources.GetString("label4.Text");
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(7, 11);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(217, 13);
            label5.TabIndex = 13;
            label5.Text = "Tables audited for Sensitive Column Access:";
            // 
            // columnHeader3
            // 
            columnHeader3.Text = "Table Name";
            columnHeader3.Width = 150;
            // 
            // _btnCancel
            // 
            this._btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._btnCancel.Location = new System.Drawing.Point(444, 476);
            this._btnCancel.Name = "_btnCancel";
            this._btnCancel.Size = new System.Drawing.Size(75, 23);
            this._btnCancel.TabIndex = 14;
            this._btnCancel.Text = "&Cancel";
            this._btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // _btnOK
            // 
            this._btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._btnOK.Location = new System.Drawing.Point(364, 476);
            this._btnOK.Name = "_btnOK";
            this._btnOK.Size = new System.Drawing.Size(75, 23);
            this._btnOK.TabIndex = 13;
            this._btnOK.Text = "&OK";
            this._btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // _tabControl
            // 
            this._tabControl.Controls.Add(this._tabGeneral);
            this._tabControl.Controls.Add(this._tabAuditSettings);
            this._tabControl.Controls.Add(this._tabFilters);
            this._tabControl.Controls.Add(this._tabBeforeAfter);
            this._tabControl.Controls.Add(this._tabSensitiveColumns);
            this._tabControl.Controls.Add(this._tabTrustedUsers);
            this._tabControl.Controls.Add(this._tabPrivilegedUser);
            this._tabControl.Location = new System.Drawing.Point(0, 0);
            this._tabControl.Name = "_tabControl";
            this._tabControl.SelectedIndex = 0;
            this._tabControl.Size = new System.Drawing.Size(537, 441);
            this._tabControl.TabIndex = 36;
            // 
            // _tabGeneral
            // 
            this._tabGeneral.Controls.Add(this.groupBox1);
            this._tabGeneral.Controls.Add(this.grpStatus);
            this._tabGeneral.Location = new System.Drawing.Point(4, 22);
            this._tabGeneral.Name = "_tabGeneral";
            this._tabGeneral.Size = new System.Drawing.Size(529, 415);
            this._tabGeneral.TabIndex = 0;
            this._tabGeneral.Text = "General";
            this._tabGeneral.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblServer);
            this.groupBox1.Controls.Add(this.lblName);
            this.groupBox1.Controls.Add(this.txtServer);
            this.groupBox1.Controls.Add(this.txtName);
            this.groupBox1.Controls.Add(this.lblDescription);
            this.groupBox1.Controls.Add(this.txtDescription);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(471, 169);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Database";
            // 
            // lblServer
            // 
            this.lblServer.Location = new System.Drawing.Point(6, 19);
            this.lblServer.Name = "lblServer";
            this.lblServer.Size = new System.Drawing.Size(88, 16);
            this.lblServer.TabIndex = 1;
            this.lblServer.Text = "&Server instance:";
            // 
            // lblName
            // 
            this.lblName.Location = new System.Drawing.Point(6, 47);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(88, 16);
            this.lblName.TabIndex = 3;
            this.lblName.Text = "Database &name:";
            // 
            // txtServer
            // 
            this.txtServer.Location = new System.Drawing.Point(98, 15);
            this.txtServer.Name = "txtServer";
            this.txtServer.ReadOnly = true;
            this.txtServer.Size = new System.Drawing.Size(359, 20);
            this.txtServer.TabIndex = 2;
            this.txtServer.TabStop = false;
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(98, 43);
            this.txtName.Name = "txtName";
            this.txtName.ReadOnly = true;
            this.txtName.Size = new System.Drawing.Size(359, 20);
            this.txtName.TabIndex = 4;
            this.txtName.TabStop = false;
            // 
            // lblDescription
            // 
            this.lblDescription.Location = new System.Drawing.Point(6, 75);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(68, 16);
            this.lblDescription.TabIndex = 5;
            this.lblDescription.Text = "&Description:";
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(98, 73);
            this.txtDescription.MaxLength = 255;
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(359, 84);
            this.txtDescription.TabIndex = 6;
            this.txtDescription.Text = "Description";
            // 
            // grpStatus
            // 
            this.grpStatus.Controls.Add(this.txtTimeEnabledModified);
            this.grpStatus.Controls.Add(this.lblTimeEnabledModified);
            this.grpStatus.Controls.Add(this.txtTimeLastModified);
            this.grpStatus.Controls.Add(this.lblTimeLastModified);
            this.grpStatus.Controls.Add(this.txtTimeCreated);
            this.grpStatus.Controls.Add(this.txtStatus);
            this.grpStatus.Controls.Add(this.lblTimeCreated);
            this.grpStatus.Controls.Add(this.lblStatus);
            this.grpStatus.Location = new System.Drawing.Point(12, 187);
            this.grpStatus.Name = "grpStatus";
            this.grpStatus.Size = new System.Drawing.Size(471, 132);
            this.grpStatus.TabIndex = 9;
            this.grpStatus.TabStop = false;
            this.grpStatus.Text = "Status";
            // 
            // txtTimeEnabledModified
            // 
            this.txtTimeEnabledModified.Location = new System.Drawing.Point(172, 100);
            this.txtTimeEnabledModified.Name = "txtTimeEnabledModified";
            this.txtTimeEnabledModified.ReadOnly = true;
            this.txtTimeEnabledModified.Size = new System.Drawing.Size(116, 20);
            this.txtTimeEnabledModified.TabIndex = 7;
            this.txtTimeEnabledModified.TabStop = false;
            this.txtTimeEnabledModified.Text = "12/13/2004 9:15 AM";
            // 
            // lblTimeEnabledModified
            // 
            this.lblTimeEnabledModified.Location = new System.Drawing.Point(8, 104);
            this.lblTimeEnabledModified.Name = "lblTimeEnabledModified";
            this.lblTimeEnabledModified.Size = new System.Drawing.Size(160, 16);
            this.lblTimeEnabledModified.TabIndex = 6;
            this.lblTimeEnabledModified.Text = "Last change in auditing status:";
            // 
            // txtTimeLastModified
            // 
            this.txtTimeLastModified.Location = new System.Drawing.Point(172, 72);
            this.txtTimeLastModified.Name = "txtTimeLastModified";
            this.txtTimeLastModified.ReadOnly = true;
            this.txtTimeLastModified.Size = new System.Drawing.Size(116, 20);
            this.txtTimeLastModified.TabIndex = 5;
            this.txtTimeLastModified.TabStop = false;
            this.txtTimeLastModified.Text = "12/13/2004 9:15 AM";
            // 
            // lblTimeLastModified
            // 
            this.lblTimeLastModified.Location = new System.Drawing.Point(8, 76);
            this.lblTimeLastModified.Name = "lblTimeLastModified";
            this.lblTimeLastModified.Size = new System.Drawing.Size(76, 16);
            this.lblTimeLastModified.TabIndex = 4;
            this.lblTimeLastModified.Text = "Last Modified:";
            // 
            // txtTimeCreated
            // 
            this.txtTimeCreated.Location = new System.Drawing.Point(172, 44);
            this.txtTimeCreated.Name = "txtTimeCreated";
            this.txtTimeCreated.ReadOnly = true;
            this.txtTimeCreated.Size = new System.Drawing.Size(116, 20);
            this.txtTimeCreated.TabIndex = 3;
            this.txtTimeCreated.TabStop = false;
            this.txtTimeCreated.Text = "12/13/2004 9:15 AM";
            // 
            // txtStatus
            // 
            this.txtStatus.Location = new System.Drawing.Point(172, 16);
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.ReadOnly = true;
            this.txtStatus.Size = new System.Drawing.Size(116, 20);
            this.txtStatus.TabIndex = 2;
            this.txtStatus.TabStop = false;
            this.txtStatus.Text = "Disabled";
            // 
            // lblTimeCreated
            // 
            this.lblTimeCreated.Location = new System.Drawing.Point(8, 48);
            this.lblTimeCreated.Name = "lblTimeCreated";
            this.lblTimeCreated.Size = new System.Drawing.Size(76, 16);
            this.lblTimeCreated.TabIndex = 1;
            this.lblTimeCreated.Text = "Date created:";
            // 
            // lblStatus
            // 
            this.lblStatus.Location = new System.Drawing.Point(8, 20);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(92, 20);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "Auditing status:";
            // 
            // _tabAuditSettings
            // 
            this._tabAuditSettings.Controls.Add(this.chkDBCaptureDDL);
            this._tabAuditSettings.Controls.Add(this._chkCaptureTrans);
            this._tabAuditSettings.Controls.Add(this._chkCaptureSQL);
            this._tabAuditSettings.Controls.Add(this.grpAuditResults);
            this._tabAuditSettings.Controls.Add(this._grpAuditActivity);
            this._tabAuditSettings.Location = new System.Drawing.Point(4, 22);
            this._tabAuditSettings.Name = "_tabAuditSettings";
            this._tabAuditSettings.Size = new System.Drawing.Size(529, 415);
            this._tabAuditSettings.TabIndex = 1;
            this._tabAuditSettings.Text = "Audited Activities";
            this._tabAuditSettings.UseVisualStyleBackColor = true;
            // 
            // chkDBCaptureDDL
            // 
            this.chkDBCaptureDDL.AutoSize = true;
            this.chkDBCaptureDDL.Location = new System.Drawing.Point(24, 337);
            this.chkDBCaptureDDL.Name = "chkDBCaptureDDL";
            this.chkDBCaptureDDL.Size = new System.Drawing.Size(288, 17);
            this.chkDBCaptureDDL.TabIndex = 23;
            this.chkDBCaptureDDL.Text = "Capture SQL statements for DDL and Security Changes";
            this.chkDBCaptureDDL.UseVisualStyleBackColor = true;
            // 
            // _chkCaptureTrans
            // 
            this._chkCaptureTrans.AutoSize = true;
            this._chkCaptureTrans.Location = new System.Drawing.Point(24, 314);
            this._chkCaptureTrans.Name = "_chkCaptureTrans";
            this._chkCaptureTrans.Size = new System.Drawing.Size(233, 17);
            this._chkCaptureTrans.TabIndex = 22;
            this._chkCaptureTrans.Text = "Capture Transaction Status for DML Activity";
            this._chkCaptureTrans.UseVisualStyleBackColor = true;
            // 
            // _chkCaptureSQL
            // 
            this._chkCaptureSQL.Location = new System.Drawing.Point(24, 288);
            this._chkCaptureSQL.Name = "_chkCaptureSQL";
            this._chkCaptureSQL.Size = new System.Drawing.Size(384, 20);
            this._chkCaptureSQL.TabIndex = 21;
            this._chkCaptureSQL.Text = "Capture SQL Statements for DML and SELECT activities";
            this._chkCaptureSQL.CheckedChanged += new System.EventHandler(this.chkCaptureSQL_CheckedChanged);
            // 
            // grpAuditResults
            // 
            this.grpAuditResults.Controls.Add(this._rbFailed);
            this.grpAuditResults.Controls.Add(this._rbPassed);
            this.grpAuditResults.Controls.Add(this._cbFilterOnAccess);
            this.grpAuditResults.Location = new System.Drawing.Point(12, 186);
            this.grpAuditResults.Name = "grpAuditResults";
            this.grpAuditResults.Size = new System.Drawing.Size(471, 92);
            this.grpAuditResults.TabIndex = 18;
            this.grpAuditResults.TabStop = false;
            this.grpAuditResults.Text = "Access Check Filter";
            // 
            // _rbFailed
            // 
            this._rbFailed.Location = new System.Drawing.Point(32, 64);
            this._rbFailed.Name = "_rbFailed";
            this._rbFailed.Size = new System.Drawing.Size(280, 16);
            this._rbFailed.TabIndex = 2;
            this._rbFailed.Text = "&Failed";
            // 
            // _rbPassed
            // 
            this._rbPassed.Checked = true;
            this._rbPassed.Location = new System.Drawing.Point(32, 40);
            this._rbPassed.Name = "_rbPassed";
            this._rbPassed.Size = new System.Drawing.Size(272, 16);
            this._rbPassed.TabIndex = 1;
            this._rbPassed.TabStop = true;
            this._rbPassed.Text = "&Passed";
            // 
            // _cbFilterOnAccess
            // 
            this._cbFilterOnAccess.Checked = true;
            this._cbFilterOnAccess.CheckState = System.Windows.Forms.CheckState.Checked;
            this._cbFilterOnAccess.Location = new System.Drawing.Point(12, 16);
            this._cbFilterOnAccess.Name = "_cbFilterOnAccess";
            this._cbFilterOnAccess.Size = new System.Drawing.Size(276, 16);
            this._cbFilterOnAccess.TabIndex = 0;
            this._cbFilterOnAccess.Text = "&Filter events based on access check";
            this._cbFilterOnAccess.Click += new System.EventHandler(this.Click_ChkExcludeFailedAccess);
            // 
            // _grpAuditActivity
            // 
            this._grpAuditActivity.Controls.Add(this._chkAuditAdmin);
            this._grpAuditActivity.Controls.Add(this._chkAuditDML);
            this._grpAuditActivity.Controls.Add(this._chkAuditSELECT);
            this._grpAuditActivity.Controls.Add(this._chkAuditSecurity);
            this._grpAuditActivity.Controls.Add(this._chkAuditDDL);
            this._grpAuditActivity.Location = new System.Drawing.Point(12, 12);
            this._grpAuditActivity.Name = "_grpAuditActivity";
            this._grpAuditActivity.Size = new System.Drawing.Size(471, 168);
            this._grpAuditActivity.TabIndex = 10;
            this._grpAuditActivity.TabStop = false;
            this._grpAuditActivity.Text = "Audited Activities";
            // 
            // _chkAuditAdmin
            // 
            this._chkAuditAdmin.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this._chkAuditAdmin.Location = new System.Drawing.Point(12, 44);
            this._chkAuditAdmin.Name = "_chkAuditAdmin";
            this._chkAuditAdmin.Size = new System.Drawing.Size(212, 20);
            this._chkAuditAdmin.TabIndex = 13;
            this._chkAuditAdmin.Text = "&Administrative Actions";
            this._chkAuditAdmin.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            // 
            // _chkAuditDML
            // 
            this._chkAuditDML.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this._chkAuditDML.Location = new System.Drawing.Point(12, 92);
            this._chkAuditDML.Name = "_chkAuditDML";
            this._chkAuditDML.Size = new System.Drawing.Size(212, 20);
            this._chkAuditDML.TabIndex = 14;
            this._chkAuditDML.Text = "Database &Modification (DML)";
            this._chkAuditDML.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this._chkAuditDML.CheckedChanged += new System.EventHandler(this.chkAuditDML_CheckedChanged);
            // 
            // _chkAuditSELECT
            // 
            this._chkAuditSELECT.Location = new System.Drawing.Point(12, 116);
            this._chkAuditSELECT.Name = "_chkAuditSELECT";
            this._chkAuditSELECT.Size = new System.Drawing.Size(208, 20);
            this._chkAuditSELECT.TabIndex = 15;
            this._chkAuditSELECT.Text = "Database SE&LECT operations";
            this._chkAuditSELECT.CheckedChanged += new System.EventHandler(this.chkAuditDML_CheckedChanged);
            // 
            // _chkAuditSecurity
            // 
            this._chkAuditSecurity.Checked = true;
            this._chkAuditSecurity.CheckState = System.Windows.Forms.CheckState.Checked;
            this._chkAuditSecurity.Location = new System.Drawing.Point(12, 20);
            this._chkAuditSecurity.Name = "_chkAuditSecurity";
            this._chkAuditSecurity.Size = new System.Drawing.Size(184, 16);
            this._chkAuditSecurity.TabIndex = 12;
            this._chkAuditSecurity.Text = "&Security Changes";
            this._chkAuditSecurity.CheckedChanged += new System.EventHandler(this._chkAuditDDL_CheckedChanged);
            // 
            // _chkAuditDDL
            // 
            this._chkAuditDDL.Checked = true;
            this._chkAuditDDL.CheckState = System.Windows.Forms.CheckState.Checked;
            this._chkAuditDDL.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this._chkAuditDDL.Location = new System.Drawing.Point(12, 68);
            this._chkAuditDDL.Name = "_chkAuditDDL";
            this._chkAuditDDL.Size = new System.Drawing.Size(156, 16);
            this._chkAuditDDL.TabIndex = 11;
            this._chkAuditDDL.Text = "Data&base Definition (DDL)";
            this._chkAuditDDL.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this._chkAuditDDL.CheckedChanged += new System.EventHandler(this._chkAuditDDL_CheckedChanged);
            // 
            // _tabFilters
            // 
            this._tabFilters.Controls.Add(this.label1);
            this._tabFilters.Controls.Add(this.radioSelectedDML);
            this._tabFilters.Controls.Add(this.radioAllDML);
            this._tabFilters.Controls.Add(this.grpUserObjects);
            this._tabFilters.Controls.Add(this.grpUserTables);
            this._tabFilters.Location = new System.Drawing.Point(4, 22);
            this._tabFilters.Name = "_tabFilters";
            this._tabFilters.Size = new System.Drawing.Size(529, 415);
            this._tabFilters.TabIndex = 3;
            this._tabFilters.Text = "DML/SELECT Filters";
            this._tabFilters.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 372);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(480, 28);
            this.label1.TabIndex = 44;
            this.label1.Text = "Note: The settings on this page only apply if you have selected to audit DML or S" +
    "ELECT activites for this database on the \'Audited Activities\' tab.";
            // 
            // radioSelectedDML
            // 
            this.radioSelectedDML.Location = new System.Drawing.Point(12, 36);
            this.radioSelectedDML.Name = "radioSelectedDML";
            this.radioSelectedDML.Size = new System.Drawing.Size(288, 24);
            this.radioSelectedDML.TabIndex = 43;
            this.radioSelectedDML.Text = "Audit the following database objects:";
            this.radioSelectedDML.CheckedChanged += new System.EventHandler(this.radioAllDML_CheckedChanged);
            // 
            // radioAllDML
            // 
            this.radioAllDML.Location = new System.Drawing.Point(12, 12);
            this.radioAllDML.Name = "radioAllDML";
            this.radioAllDML.Size = new System.Drawing.Size(312, 24);
            this.radioAllDML.TabIndex = 42;
            this.radioAllDML.Text = "Audit all database objects";
            this.radioAllDML.CheckedChanged += new System.EventHandler(this.radioAllDML_CheckedChanged);
            // 
            // grpUserObjects
            // 
            this.grpUserObjects.Controls.Add(this._chkAuditSystemTables);
            this.grpUserObjects.Controls.Add(this._chkAuditOther);
            this.grpUserObjects.Controls.Add(this._chkAuditStoredProcedures);
            this.grpUserObjects.Location = new System.Drawing.Point(28, 262);
            this.grpUserObjects.Name = "grpUserObjects";
            this.grpUserObjects.Size = new System.Drawing.Size(464, 96);
            this.grpUserObjects.TabIndex = 29;
            this.grpUserObjects.TabStop = false;
            this.grpUserObjects.Text = "Other Object Types:";
            // 
            // _chkAuditSystemTables
            // 
            this._chkAuditSystemTables.Location = new System.Drawing.Point(12, 16);
            this._chkAuditSystemTables.Name = "_chkAuditSystemTables";
            this._chkAuditSystemTables.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._chkAuditSystemTables.Size = new System.Drawing.Size(164, 24);
            this._chkAuditSystemTables.TabIndex = 30;
            this._chkAuditSystemTables.Text = "Audit s&ystem tables";
            // 
            // _chkAuditOther
            // 
            this._chkAuditOther.Location = new System.Drawing.Point(12, 64);
            this._chkAuditOther.Name = "_chkAuditOther";
            this._chkAuditOther.Size = new System.Drawing.Size(304, 24);
            this._chkAuditOther.TabIndex = 32;
            this._chkAuditOther.Text = "Audit &all other object types (views, indexes, etc.)";
            // 
            // _chkAuditStoredProcedures
            // 
            this._chkAuditStoredProcedures.Location = new System.Drawing.Point(12, 40);
            this._chkAuditStoredProcedures.Name = "_chkAuditStoredProcedures";
            this._chkAuditStoredProcedures.Size = new System.Drawing.Size(164, 24);
            this._chkAuditStoredProcedures.TabIndex = 31;
            this._chkAuditStoredProcedures.Text = "Audit &stored procedures";
            // 
            // grpUserTables
            // 
            this.grpUserTables.Controls.Add(this.listTables);
            this.grpUserTables.Controls.Add(this.rbDontAuditUserTables);
            this.grpUserTables.Controls.Add(this._btnRemove);
            this.grpUserTables.Controls.Add(this._btnAddTable);
            this.grpUserTables.Controls.Add(this.rbSelectedUserTables);
            this.grpUserTables.Controls.Add(this.rbAllUserTables);
            this.grpUserTables.Location = new System.Drawing.Point(28, 64);
            this.grpUserTables.Name = "grpUserTables";
            this.grpUserTables.Size = new System.Drawing.Size(464, 192);
            this.grpUserTables.TabIndex = 22;
            this.grpUserTables.TabStop = false;
            this.grpUserTables.Text = "User Tables";
            // 
            // listTables
            // 
            this.listTables.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listTables.Enabled = false;
            this.listTables.ForeColor = System.Drawing.SystemColors.WindowText;
            this.listTables.FullRowSelect = true;
            this.listTables.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listTables.Location = new System.Drawing.Point(28, 60);
            this.listTables.Name = "listTables";
            this.listTables.Size = new System.Drawing.Size(364, 100);
            this.listTables.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listTables.TabIndex = 38;
            this.listTables.UseCompatibleStateImageBehavior = false;
            this.listTables.View = System.Windows.Forms.View.Details;
            this.listTables.SelectedIndexChanged += new System.EventHandler(this.listTables_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 300;
            // 
            // rbDontAuditUserTables
            // 
            this.rbDontAuditUserTables.Location = new System.Drawing.Point(12, 168);
            this.rbDontAuditUserTables.Name = "rbDontAuditUserTables";
            this.rbDontAuditUserTables.Size = new System.Drawing.Size(232, 16);
            this.rbDontAuditUserTables.TabIndex = 28;
            this.rbDontAuditUserTables.Text = "Don\'t audit user &tables";
            this.rbDontAuditUserTables.CheckedChanged += new System.EventHandler(this.rbDontAuditUserTables_CheckedChanged);
            // 
            // _btnRemove
            // 
            this._btnRemove.Location = new System.Drawing.Point(398, 88);
            this._btnRemove.Name = "_btnRemove";
            this._btnRemove.Size = new System.Drawing.Size(60, 23);
            this._btnRemove.TabIndex = 27;
            this._btnRemove.Text = "&Remove";
            this._btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // _btnAddTable
            // 
            this._btnAddTable.Location = new System.Drawing.Point(398, 60);
            this._btnAddTable.Name = "_btnAddTable";
            this._btnAddTable.Size = new System.Drawing.Size(60, 23);
            this._btnAddTable.TabIndex = 26;
            this._btnAddTable.Text = "&Add...";
            this._btnAddTable.Click += new System.EventHandler(this.btnAddTable_Click);
            // 
            // rbSelectedUserTables
            // 
            this.rbSelectedUserTables.Location = new System.Drawing.Point(12, 36);
            this.rbSelectedUserTables.Name = "rbSelectedUserTables";
            this.rbSelectedUserTables.Size = new System.Drawing.Size(244, 24);
            this.rbSelectedUserTables.TabIndex = 24;
            this.rbSelectedUserTables.Text = "Audit the &following user tables:";
            this.rbSelectedUserTables.CheckedChanged += new System.EventHandler(this.rbSelectedUserTables_CheckedChanged);
            // 
            // rbAllUserTables
            // 
            this.rbAllUserTables.Checked = true;
            this.rbAllUserTables.Location = new System.Drawing.Point(12, 16);
            this.rbAllUserTables.Name = "rbAllUserTables";
            this.rbAllUserTables.Size = new System.Drawing.Size(232, 24);
            this.rbAllUserTables.TabIndex = 23;
            this.rbAllUserTables.TabStop = true;
            this.rbAllUserTables.Text = "Audit all user &tables";
            // 
            // _tabBeforeAfter
            // 
            this._tabBeforeAfter.Controls.Add(this._pnlBeforeAfter);
            this._tabBeforeAfter.Controls.Add(this._lblBeforeAfterStatus);
            this._tabBeforeAfter.Location = new System.Drawing.Point(4, 22);
            this._tabBeforeAfter.Name = "_tabBeforeAfter";
            this._tabBeforeAfter.Size = new System.Drawing.Size(529, 415);
            this._tabBeforeAfter.TabIndex = 5;
            this._tabBeforeAfter.Text = "Before-After Data";
            this._tabBeforeAfter.UseVisualStyleBackColor = true;
            // 
            // _pnlBeforeAfter
            // 
            this._pnlBeforeAfter.Controls.Add(this._gbCLR);
            this._pnlBeforeAfter.Controls.Add(label3);
            this._pnlBeforeAfter.Controls.Add(this._btnEditBATable);
            this._pnlBeforeAfter.Controls.Add(this._btnRemoveBATable);
            this._pnlBeforeAfter.Controls.Add(this._btnAddBATable);
            this._pnlBeforeAfter.Controls.Add(this._lvBeforeAfterTables);
            this._pnlBeforeAfter.Controls.Add(label2);
            this._pnlBeforeAfter.Dock = System.Windows.Forms.DockStyle.Fill;
            this._pnlBeforeAfter.Location = new System.Drawing.Point(0, 0);
            this._pnlBeforeAfter.Name = "_pnlBeforeAfter";
            this._pnlBeforeAfter.Size = new System.Drawing.Size(529, 415);
            this._pnlBeforeAfter.TabIndex = 0;
            // 
            // _gbCLR
            // 
            this._gbCLR.Controls.Add(this._btnEnableCLR);
            this._gbCLR.Controls.Add(this._lblClrStatus);
            this._gbCLR.Controls.Add(this._pbClrStatus);
            this._gbCLR.Location = new System.Drawing.Point(8, 291);
            this._gbCLR.Name = "_gbCLR";
            this._gbCLR.Size = new System.Drawing.Size(427, 103);
            this._gbCLR.TabIndex = 13;
            this._gbCLR.TabStop = false;
            this._gbCLR.Text = "CLR Status";
            // 
            // _btnEnableCLR
            // 
            this._btnEnableCLR.Location = new System.Drawing.Point(71, 62);
            this._btnEnableCLR.Name = "_btnEnableCLR";
            this._btnEnableCLR.Size = new System.Drawing.Size(75, 23);
            this._btnEnableCLR.TabIndex = 2;
            this._btnEnableCLR.Text = "Enable Now";
            this._btnEnableCLR.UseVisualStyleBackColor = true;
            this._btnEnableCLR.Click += new System.EventHandler(this.Click_btnEnableCLR);
            // 
            // _lblClrStatus
            // 
            this._lblClrStatus.Location = new System.Drawing.Point(68, 30);
            this._lblClrStatus.Name = "_lblClrStatus";
            this._lblClrStatus.Size = new System.Drawing.Size(279, 29);
            this._lblClrStatus.TabIndex = 1;
            this._lblClrStatus.Text = "label4";
            // 
            // _pbClrStatus
            // 
            this._pbClrStatus.Location = new System.Drawing.Point(6, 19);
            this._pbClrStatus.Name = "_pbClrStatus";
            this._pbClrStatus.Size = new System.Drawing.Size(48, 48);
            this._pbClrStatus.TabIndex = 0;
            this._pbClrStatus.TabStop = false;
            // 
            // _btnEditBATable
            // 
            this._btnEditBATable.Location = new System.Drawing.Point(444, 106);
            this._btnEditBATable.Name = "_btnEditBATable";
            this._btnEditBATable.Size = new System.Drawing.Size(75, 23);
            this._btnEditBATable.TabIndex = 11;
            this._btnEditBATable.Text = "Edit...";
            this._btnEditBATable.UseVisualStyleBackColor = true;
            this._btnEditBATable.Click += new System.EventHandler(this.Click_btnEditBATable);
            // 
            // _btnRemoveBATable
            // 
            this._btnRemoveBATable.Location = new System.Drawing.Point(444, 77);
            this._btnRemoveBATable.Name = "_btnRemoveBATable";
            this._btnRemoveBATable.Size = new System.Drawing.Size(75, 23);
            this._btnRemoveBATable.TabIndex = 10;
            this._btnRemoveBATable.Text = "Remove";
            this._btnRemoveBATable.UseVisualStyleBackColor = true;
            this._btnRemoveBATable.Click += new System.EventHandler(this.Click_btnRemoveBATable);
            // 
            // _btnAddBATable
            // 
            this._btnAddBATable.Location = new System.Drawing.Point(444, 48);
            this._btnAddBATable.Name = "_btnAddBATable";
            this._btnAddBATable.Size = new System.Drawing.Size(75, 23);
            this._btnAddBATable.TabIndex = 9;
            this._btnAddBATable.Text = "Add...";
            this._btnAddBATable.UseVisualStyleBackColor = true;
            this._btnAddBATable.Click += new System.EventHandler(this.Click_btnAddBATable);
            // 
            // _lvBeforeAfterTables
            // 
            this._lvBeforeAfterTables.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            _columnTableName,
            _columnMaxRows,
            this._columnColumnNames});
            this._lvBeforeAfterTables.FullRowSelect = true;
            this._lvBeforeAfterTables.HideSelection = false;
            this._lvBeforeAfterTables.Location = new System.Drawing.Point(9, 30);
            this._lvBeforeAfterTables.Name = "_lvBeforeAfterTables";
            this._lvBeforeAfterTables.Size = new System.Drawing.Size(430, 179);
            this._lvBeforeAfterTables.TabIndex = 8;
            this._lvBeforeAfterTables.UseCompatibleStateImageBehavior = false;
            this._lvBeforeAfterTables.View = System.Windows.Forms.View.Details;
            this._lvBeforeAfterTables.ItemActivate += new System.EventHandler(this.ItemActivate_lvBeforeAfterTables);
            this._lvBeforeAfterTables.SelectedIndexChanged += new System.EventHandler(this.SelectedIndexChanged_lvBeforeAfterTables);
            // 
            // _columnColumnNames
            // 
            this._columnColumnNames.Text = "Columns";
            this._columnColumnNames.Width = 156;
            // 
            // _lblBeforeAfterStatus
            // 
            this._lblBeforeAfterStatus.Location = new System.Drawing.Point(8, 14);
            this._lblBeforeAfterStatus.Name = "_lblBeforeAfterStatus";
            this._lblBeforeAfterStatus.Size = new System.Drawing.Size(447, 113);
            this._lblBeforeAfterStatus.TabIndex = 1;
            this._lblBeforeAfterStatus.Text = "label4";
            // 
            // _tabSensitiveColumns
            // 
            this._tabSensitiveColumns.Controls.Add(this._pnlSensitiveColumns);
            this._tabSensitiveColumns.Controls.Add(this._lblSCStatus);
            this._tabSensitiveColumns.Location = new System.Drawing.Point(4, 22);
            this._tabSensitiveColumns.Name = "_tabSensitiveColumns";
            this._tabSensitiveColumns.Size = new System.Drawing.Size(529, 415);
            this._tabSensitiveColumns.TabIndex = 6;
            this._tabSensitiveColumns.Text = "Sensitive Columns";
            this._tabSensitiveColumns.UseVisualStyleBackColor = true;
            // 
            // _pnlSensitiveColumns
            // 
            this._pnlSensitiveColumns.Controls.Add(this._btnAddSCDataSet);
            this._pnlSensitiveColumns.Controls.Add(label5);
            this._pnlSensitiveColumns.Controls.Add(label4);
            this._pnlSensitiveColumns.Controls.Add(this._lvSCTables);
            this._pnlSensitiveColumns.Controls.Add(this._btnEditSCTable);
            this._pnlSensitiveColumns.Controls.Add(this._btnAddSCTable);
            this._pnlSensitiveColumns.Controls.Add(this._btnRemoveSCTable);
            this._pnlSensitiveColumns.Controls.Add(this._btnConfigureSC); //SQlCM-5747 v5.6
            this._pnlSensitiveColumns.Dock = System.Windows.Forms.DockStyle.Fill;
            this._pnlSensitiveColumns.Location = new System.Drawing.Point(0, 0);
            this._pnlSensitiveColumns.Name = "_pnlSensitiveColumns";
            this._pnlSensitiveColumns.Size = new System.Drawing.Size(529, 415);
            this._pnlSensitiveColumns.TabIndex = 19;
            // 
            // _btnAddSCDataSet
            // 
            this._btnAddSCDataSet.Location = new System.Drawing.Point(443, 72);
            this._btnAddSCDataSet.Name = "_btnAddSCDataSet";
            this._btnAddSCDataSet.Size = new System.Drawing.Size(75, 23);
            this._btnAddSCDataSet.TabIndex = 19;
            this._btnAddSCDataSet.Text = "Add Dataset";
            this._btnAddSCDataSet.UseVisualStyleBackColor = true;
            this._btnAddSCDataSet.Click += new System.EventHandler(this._btnAddSCDataSet_Click);
            // 
            // _lvSCTables
            // 
            this._lvSCTables.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            columnHeader3,
            this.columnHeader5,
            this.columnHeader6});
            this._lvSCTables.FullRowSelect = true;
            this._lvSCTables.HideSelection = false;
            this._lvSCTables.Location = new System.Drawing.Point(7, 27);
            this._lvSCTables.Name = "_lvSCTables";
            this._lvSCTables.Size = new System.Drawing.Size(430, 179);
            this._lvSCTables.TabIndex = 14;
            this._lvSCTables.UseCompatibleStateImageBehavior = false;
            this._lvSCTables.View = System.Windows.Forms.View.Details;
            this._lvSCTables.SelectedIndexChanged += new System.EventHandler(this.SelectedIndexChanged_lvSCTables);
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Columns";
            this.columnHeader5.Width = 176;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Type";
            this.columnHeader6.Width = 139;
            // 
            // _btnEditSCTable
            // 
            this._btnEditSCTable.Location = new System.Drawing.Point(443, 130);
            this._btnEditSCTable.Name = "_btnEditSCTable";
            this._btnEditSCTable.Size = new System.Drawing.Size(75, 23);
            this._btnEditSCTable.TabIndex = 17;
            this._btnEditSCTable.Text = "Edit...";
            this._btnEditSCTable.UseVisualStyleBackColor = true;
            this._btnEditSCTable.Click += new System.EventHandler(this.Click_btnEditSCTable);
            // 
            // _btnAddSCTable
            // 
            this._btnAddSCTable.Location = new System.Drawing.Point(443, 45);
            this._btnAddSCTable.Name = "_btnAddSCTable";
            this._btnAddSCTable.Size = new System.Drawing.Size(75, 23);
            this._btnAddSCTable.TabIndex = 15;
            this._btnAddSCTable.Text = "Add Column";
            this._btnAddSCTable.UseVisualStyleBackColor = true;
            this._btnAddSCTable.Click += new System.EventHandler(this._btnAddSCTable_Click);
            // 
            // _btnRemoveSCTable
            // 
            this._btnRemoveSCTable.Location = new System.Drawing.Point(443, 101);
            this._btnRemoveSCTable.Name = "_btnRemoveSCTable";
            this._btnRemoveSCTable.Size = new System.Drawing.Size(75, 23);
            this._btnRemoveSCTable.TabIndex = 16;
            this._btnRemoveSCTable.Text = "Remove";
            this._btnRemoveSCTable.UseVisualStyleBackColor = true;
            this._btnRemoveSCTable.Click += new System.EventHandler(this.Click_btnRemoveSCTable);

            //SQlCM-5747 v5.6
            // 
            // _btnConfigureSC
            // 
            this._btnConfigureSC.Location = new System.Drawing.Point(443, 160);
            this._btnConfigureSC.Name = "_btnConfigureSC";
            this._btnConfigureSC.Size = new System.Drawing.Size(75, 23);
            this._btnConfigureSC.TabIndex = 16;
            this._btnConfigureSC.Text = "Configure";
            this._btnConfigureSC.UseVisualStyleBackColor = true;
            this._btnConfigureSC.Click += new System.EventHandler(this.Click_btnConfigureSC);
            //SQlCM-5747 v5.6 - END
            // 
            // _lblSCStatus
            // 
            this._lblSCStatus.Location = new System.Drawing.Point(8, 14);
            this._lblSCStatus.Name = "_lblSCStatus";
            this._lblSCStatus.Size = new System.Drawing.Size(447, 113);
            this._lblSCStatus.TabIndex = 20;
            this._lblSCStatus.Text = "label4";
            // 
            // _tabTrustedUsers
            // 
            this._tabTrustedUsers.Controls.Add(this._pnlTrustedUsers);
            this._tabTrustedUsers.Controls.Add(this._lblTrustedUserStatus);
            this._tabTrustedUsers.Location = new System.Drawing.Point(4, 22);
            this._tabTrustedUsers.Name = "_tabTrustedUsers";
            this._tabTrustedUsers.Size = new System.Drawing.Size(529, 415);
            this._tabTrustedUsers.TabIndex = 4;
            this._tabTrustedUsers.Text = "Trusted Users";
            this._tabTrustedUsers.UseVisualStyleBackColor = true;
            // 
            // _pnlTrustedUsers
            // 
            this._pnlTrustedUsers.Controls.Add(this.label19);
            this._pnlTrustedUsers.Controls.Add(@__lblTrustedUserTxt);
            this._pnlTrustedUsers.Controls.Add(lblTrustedUserDisableNote); // v5.6 SQLCM-5373
            this._pnlTrustedUsers.Controls.Add(this._btnAddUser);
            this._pnlTrustedUsers.Controls.Add(this._lnkTrustedUserHelp);
            this._pnlTrustedUsers.Controls.Add(this._btnRemoveUser);
            this._pnlTrustedUsers.Controls.Add(this._lstTrustedUsers);
            this._pnlTrustedUsers.Dock = System.Windows.Forms.DockStyle.Fill;
            this._pnlTrustedUsers.Location = new System.Drawing.Point(0, 0);
            this._pnlTrustedUsers.Name = "_pnlTrustedUsers";
            this._pnlTrustedUsers.Size = new System.Drawing.Size(529, 415);
            this._pnlTrustedUsers.TabIndex = 29;
            // 
            // label19
            // 
            this.label19.Location = new System.Drawing.Point(8, 14);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(284, 16);
            this.label19.TabIndex = 23;
            this.label19.Text = "&Trusted users and roles to be filtered:";
            // 
            // _btnAddUser
            // 
            this._btnAddUser.Location = new System.Drawing.Point(444, 48);
            this._btnAddUser.Name = "_btnAddUser";
            this._btnAddUser.Size = new System.Drawing.Size(75, 23);
            this._btnAddUser.TabIndex = 24;
            this._btnAddUser.Text = "&Add...";
            this._btnAddUser.Click += new System.EventHandler(this.Click_btnAddUser);
            // 
            // _lnkTrustedUserHelp
            // 
            this._lnkTrustedUserHelp.LinkArea = new System.Windows.Forms.LinkArea(86, 16);
            this._lnkTrustedUserHelp.Location = new System.Drawing.Point(8, 231);
            this._lnkTrustedUserHelp.Name = "_lnkTrustedUserHelp";
            this._lnkTrustedUserHelp.Size = new System.Drawing.Size(427, 48);
            this._lnkTrustedUserHelp.TabIndex = 27;
            this._lnkTrustedUserHelp.TabStop = true;
            this._lnkTrustedUserHelp.Text = "A trusted user is a SQL Server login or role whose activity you do not need to au" +
    "dit. Tell me more ...";
            this._lnkTrustedUserHelp.UseCompatibleTextRendering = true;
            this._lnkTrustedUserHelp.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkClicked_lnkTrustedUserHelp);
            // 
            // _btnRemoveUser
            // 
            this._btnRemoveUser.Location = new System.Drawing.Point(444, 77);
            this._btnRemoveUser.Name = "_btnRemoveUser";
            this._btnRemoveUser.Size = new System.Drawing.Size(75, 23);
            this._btnRemoveUser.TabIndex = 25;
            this._btnRemoveUser.Text = "&Remove";
            this._btnRemoveUser.Click += new System.EventHandler(this.Click_btnRemoveUser);
            // 
            // _lstTrustedUsers
            // 
            this._lstTrustedUsers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
            this._lstTrustedUsers.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this._lstTrustedUsers.Location = new System.Drawing.Point(9, 30);
            this._lstTrustedUsers.Name = "_lstTrustedUsers";
            this._lstTrustedUsers.Size = new System.Drawing.Size(430, 179);
            this._lstTrustedUsers.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this._lstTrustedUsers.TabIndex = 26;
            this._lstTrustedUsers.UseCompatibleStateImageBehavior = false;
            this._lstTrustedUsers.View = System.Windows.Forms.View.Details;
            this._lstTrustedUsers.SelectedIndexChanged += new System.EventHandler(this.SelectedIndexChanged_lstTrustedUsers);
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Name";
            this.columnHeader2.Width = 415;
            // 
            // _lblTrustedUserStatus
            // 
            this._lblTrustedUserStatus.Location = new System.Drawing.Point(30, 30);
            this._lblTrustedUserStatus.Name = "_lblTrustedUserStatus";
            this._lblTrustedUserStatus.Size = new System.Drawing.Size(425, 56);
            this._lblTrustedUserStatus.TabIndex = 30;
            // 
            // _tabPrivilegedUser
            // 
            this._tabPrivilegedUser.Controls.Add(this.lstPrivilegedUsers);
            this._tabPrivilegedUser.Controls.Add(this.btnRemovePriv);
            this._tabPrivilegedUser.Controls.Add(this.btnAddPriv);
            this._tabPrivilegedUser.Controls.Add(this.label6);
            this._tabPrivilegedUser.Controls.Add(this.labelPreSelectedPrivilegedUsers);
            this._tabPrivilegedUser.Controls.Add(this.lstPreSelectedPrivilegedUsers);
            this._tabPrivilegedUser.Controls.Add(this.grpPrivilegedUserActivity);
            this._tabPrivilegedUser.Location = new System.Drawing.Point(4, 22);
            this._tabPrivilegedUser.Name = "_tabPrivilegedUser";
            this._tabPrivilegedUser.Padding = new System.Windows.Forms.Padding(3);
            this._tabPrivilegedUser.Size = new System.Drawing.Size(529, 415);
            this._tabPrivilegedUser.TabIndex = 7;
            this._tabPrivilegedUser.Text = "Privileged User Auditing";
            this._tabPrivilegedUser.UseVisualStyleBackColor = true;
            // 
            // lstPrivilegedUsers
            // 
            this.lstPrivilegedUsers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4});
            this.lstPrivilegedUsers.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lstPrivilegedUsers.Location = new System.Drawing.Point(12, 111);
            this.lstPrivilegedUsers.Name = "lstPrivilegedUsers";
            this.lstPrivilegedUsers.Size = new System.Drawing.Size(440, 70);
            this.lstPrivilegedUsers.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lstPrivilegedUsers.TabIndex = 27;
            this.lstPrivilegedUsers.UseCompatibleStateImageBehavior = false;
            this.lstPrivilegedUsers.View = System.Windows.Forms.View.Details;
            this.lstPrivilegedUsers.SelectedIndexChanged += new System.EventHandler(this.lstPrivilegedUsers_SelectedIndexChanged);
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Name";
            this.columnHeader4.Width = 415;

            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Name";
            this.columnHeader7.Width = 415;

            // 
            // btnRemovePriv
            // 
            this.btnRemovePriv.Location = new System.Drawing.Point(460, 140);
            this.btnRemovePriv.Name = "btnRemovePriv";
            this.btnRemovePriv.Size = new System.Drawing.Size(60, 23);
            this.btnRemovePriv.TabIndex = 25;
            this.btnRemovePriv.Text = "&Remove";
            this.btnRemovePriv.Click += new System.EventHandler(this.btnRemovePriv_Click);
            // 
            // btnAddPriv
            // 
            this.btnAddPriv.Location = new System.Drawing.Point(460, 110);
            this.btnAddPriv.Name = "btnAddPriv";
            this.btnAddPriv.Size = new System.Drawing.Size(60, 23);
            this.btnAddPriv.TabIndex = 24;
            this.btnAddPriv.Text = "&Add...";
            this.btnAddPriv.Click += new System.EventHandler(this.btnAddPriv_Click);
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(12, 95);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(284, 16);
            this.label6.TabIndex = 23;
            this.label6.Text = "Privileged users and roles to be audited:";

            // 
            // labelPreSelectedPrivilegedUsers
            // 
            this.labelPreSelectedPrivilegedUsers.Location = new System.Drawing.Point(12, 5);
            this.labelPreSelectedPrivilegedUsers.Name = "labelPreSelectedPrivilegedUsers";
            this.labelPreSelectedPrivilegedUsers.Size = new System.Drawing.Size(284, 18);
            this.labelPreSelectedPrivilegedUsers.TabIndex = 23;
            this.labelPreSelectedPrivilegedUsers.Text = "Server-Level Privileged Users";

            // 
            // lstPreSelectedPrivilegedUsers
            // 
            this.lstPreSelectedPrivilegedUsers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader7});
            this.lstPreSelectedPrivilegedUsers.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lstPreSelectedPrivilegedUsers.Location = new System.Drawing.Point(12, 25);
            this.lstPreSelectedPrivilegedUsers.Name = "lstPreSelectedPrivilegedUsers";
            this.lstPreSelectedPrivilegedUsers.Size = new System.Drawing.Size(440, 70);
            this.lstPreSelectedPrivilegedUsers.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lstPreSelectedPrivilegedUsers.TabIndex = 27;
            this.lstPreSelectedPrivilegedUsers.UseCompatibleStateImageBehavior = false;
            this.lstPreSelectedPrivilegedUsers.View = System.Windows.Forms.View.Details;
            this.lstPreSelectedPrivilegedUsers.ShowItemToolTips = true;
            //this.lstPreSelectedPrivilegedUsers.SelectedIndexChanged += new System.EventHandler(this.lstPrivilegedUsers_SelectedIndexChanged);

            // 
            // grpPrivilegedUserActivity
            // 
            this.grpPrivilegedUserActivity.Controls.Add(this.grpAuditUserActivity);
            this.grpPrivilegedUserActivity.Controls.Add(this.rbAuditUserSelected);
            this.grpPrivilegedUserActivity.Controls.Add(this.rbAuditUserAll);
            this.grpPrivilegedUserActivity.Controls.Add(this.checkBox3);
            this.grpPrivilegedUserActivity.Enabled = false;
            this.grpPrivilegedUserActivity.Location = new System.Drawing.Point(12, 185);
            this.grpPrivilegedUserActivity.Name = "grpPrivilegedUserActivity";
            this.grpPrivilegedUserActivity.Size = new System.Drawing.Size(508, 226);
            this.grpPrivilegedUserActivity.TabIndex = 26;
            this.grpPrivilegedUserActivity.TabStop = false;
            this.grpPrivilegedUserActivity.Text = "Audited Activity";
            // 
            // grpAuditUserActivity
            // 
            this.grpAuditUserActivity.Controls.Add(this.chkAuditUserLogouts);
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
            this.grpAuditUserActivity.Location = new System.Drawing.Point(28, 56);
            this.grpAuditUserActivity.Name = "grpAuditUserActivity";
            this.grpAuditUserActivity.Size = new System.Drawing.Size(468, 164);
            this.grpAuditUserActivity.TabIndex = 15;
            this.grpAuditUserActivity.TabStop = false;
            // 
            // chkAuditUserLogouts
            // SQLCM-5375-6.1.4.3-Capture Logout Events at Server level	
            this.chkAuditUserLogouts.Checked = false;
            this.chkAuditUserLogouts.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAuditUserLogouts.Location = new System.Drawing.Point(10, 32);
            this.chkAuditUserLogouts.Name = "chkAuditUserLogouts";
            this.chkAuditUserLogouts.Size = new System.Drawing.Size(104, 17);
            this.chkAuditUserLogouts.TabIndex = 6;
            this.chkAuditUserLogouts.Text = "Log&outs";
            // 
            // chkUserCaptureDDL
            // 
            this.chkUserCaptureDDL.AutoSize = true;
            this.chkUserCaptureDDL.Location = new System.Drawing.Point(10, 143);
            this.chkUserCaptureDDL.Name = "chkUserCaptureDDL";
            this.chkUserCaptureDDL.Size = new System.Drawing.Size(291, 17);
            this.chkUserCaptureDDL.TabIndex = 13;
            this.chkUserCaptureDDL.Text = "Capture S&QL statements for DDL and Security Changes";
            this.chkUserCaptureDDL.UseVisualStyleBackColor = true;
            // 
            // chkUserCaptureTrans
            // 
            this.chkUserCaptureTrans.AutoSize = true;
            this.chkUserCaptureTrans.Location = new System.Drawing.Point(10, 122);
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
            this._rbUserAuditFailed.Location = new System.Drawing.Point(304, 81);
            this._rbUserAuditFailed.Name = "_rbUserAuditFailed";
            this._rbUserAuditFailed.Size = new System.Drawing.Size(60, 16);
            this._rbUserAuditFailed.TabIndex = 10;
            this._rbUserAuditFailed.Text = "Faile&d";
            // 
            // _rbUserAuditPassed
            // 
            this._rbUserAuditPassed.Checked = true;
            this._rbUserAuditPassed.Location = new System.Drawing.Point(224, 81);
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
            this.chkUserCaptureSQL.Location = new System.Drawing.Point(10, 101);
            this.chkUserCaptureSQL.Name = "chkUserCaptureSQL";
            this.chkUserCaptureSQL.Size = new System.Drawing.Size(356, 20);
            this.chkUserCaptureSQL.TabIndex = 11;
            this.chkUserCaptureSQL.Text = "Capture S&QL Statements for DML and SELECT activities";
            this.chkUserCaptureSQL.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.chkUserCaptureSQL.CheckedChanged += new System.EventHandler(this.chkUserCaptureSQL_CheckedChanged);
            // 
            // _cbUserFilterAccessCheck
            // 
            this._cbUserFilterAccessCheck.Location = new System.Drawing.Point(10, 81);
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
            this.chkAuditUserSecurity.CheckedChanged += new System.EventHandler(this.chkAuditUserDDL_CheckedChanged);
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
            this.chkAuditUserDDL.CheckedChanged += new System.EventHandler(this.chkAuditUserDDL_CheckedChanged);
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
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.Gray;
            this.panel1.Location = new System.Drawing.Point(3, 469);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(527, 1);
            this.panel1.TabIndex = 38;
            // 
            // linkLblHelpBestPractices
            // 
            this.linkLblHelpBestPractices.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.linkLblHelpBestPractices.AutoSize = true;
            this.linkLblHelpBestPractices.LinkArea = new System.Windows.Forms.LinkArea(0, 99);
            this.linkLblHelpBestPractices.Location = new System.Drawing.Point(13, 446);
            this.linkLblHelpBestPractices.Name = "linkLblHelpBestPractices";
            this.linkLblHelpBestPractices.Size = new System.Drawing.Size(277, 17);
            this.linkLblHelpBestPractices.TabIndex = 37;
            this.linkLblHelpBestPractices.TabStop = true;
            this.linkLblHelpBestPractices.Text = "Learn how to optimize performance with audit settings.";
            this.linkLblHelpBestPractices.UseCompatibleTextRendering = true;
            this.linkLblHelpBestPractices.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLblHelpBestPractices_LinkClicked);
            // 
            // Form_DatabaseProperties
            // 
            this.AcceptButton = this._btnOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this._btnCancel;
            this.ClientSize = new System.Drawing.Size(534, 508);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.linkLblHelpBestPractices);
            this.Controls.Add(this._tabControl);
            this.Controls.Add(this._btnCancel);
            this.Controls.Add(this._btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form_DatabaseProperties";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Audited Database Properties";
            this.Load += new System.EventHandler(this.FormDatabasePropertiesLoad);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.Form_DatabaseProperties_HelpRequested);
            this._tabControl.ResumeLayout(false);
            this._tabGeneral.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grpStatus.ResumeLayout(false);
            this.grpStatus.PerformLayout();
            this._tabAuditSettings.ResumeLayout(false);
            this._tabAuditSettings.PerformLayout();
            this.grpAuditResults.ResumeLayout(false);
            this._grpAuditActivity.ResumeLayout(false);
            this._tabFilters.ResumeLayout(false);
            this.grpUserObjects.ResumeLayout(false);
            this.grpUserTables.ResumeLayout(false);
            this._tabBeforeAfter.ResumeLayout(false);
            this._pnlBeforeAfter.ResumeLayout(false);
            this._pnlBeforeAfter.PerformLayout();
            this._gbCLR.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._pbClrStatus)).EndInit();
            this._tabSensitiveColumns.ResumeLayout(false);
            this._pnlSensitiveColumns.ResumeLayout(false);
            this._pnlSensitiveColumns.PerformLayout();
            this._tabTrustedUsers.ResumeLayout(false);
            this._pnlTrustedUsers.ResumeLayout(false);
            this._pnlTrustedUsers.PerformLayout();
            this._tabPrivilegedUser.ResumeLayout(false);
            this._tabPrivilegedUser.PerformLayout();
            this.grpPrivilegedUserActivity.ResumeLayout(false);
            this.grpAuditUserActivity.ResumeLayout(false);
            this.grpAuditUserActivity.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

      }
      #endregion

      private System.Windows.Forms.Button _btnCancel;
      private System.Windows.Forms.Button _btnOK;
      private System.Windows.Forms.TabControl _tabControl;
      private System.Windows.Forms.TabPage _tabGeneral;
      private System.Windows.Forms.TabPage _tabAuditSettings;
      private System.Windows.Forms.TabPage _tabFilters;
      private System.Windows.Forms.TextBox txtDescription;
      private System.Windows.Forms.TextBox txtName;
      private System.Windows.Forms.CheckBox _chkAuditDML;
      private System.Windows.Forms.CheckBox _chkAuditDDL;
      private System.Windows.Forms.CheckBox _chkAuditSELECT;
      private System.Windows.Forms.CheckBox _chkAuditSecurity;
      private System.Windows.Forms.Button _btnRemove;
      private System.Windows.Forms.Button _btnAddTable;
      private System.Windows.Forms.RadioButton rbSelectedUserTables;
      private System.Windows.Forms.RadioButton rbAllUserTables;
      private System.Windows.Forms.TextBox txtServer;
      private System.Windows.Forms.GroupBox grpAuditResults;
      private System.Windows.Forms.GroupBox _grpAuditActivity;
      private System.Windows.Forms.CheckBox _chkAuditStoredProcedures;
      private System.Windows.Forms.RadioButton rbDontAuditUserTables;
      private System.Windows.Forms.TextBox txtTimeCreated;
      private System.Windows.Forms.TextBox txtStatus;
      private System.Windows.Forms.Label lblTimeCreated;
      private System.Windows.Forms.Label lblStatus;
      private System.Windows.Forms.TextBox txtTimeLastModified;
      private System.Windows.Forms.Label lblTimeLastModified;
      private System.Windows.Forms.TextBox txtTimeEnabledModified;
      private System.Windows.Forms.Label lblTimeEnabledModified;
      private System.Windows.Forms.ListView listTables;
      private System.Windows.Forms.ColumnHeader columnHeader1;
      private System.Windows.Forms.GroupBox grpStatus;
      private System.Windows.Forms.Label lblServer;
      private System.Windows.Forms.Label lblDescription;
      private System.Windows.Forms.Label lblName;
      private System.Windows.Forms.GroupBox grpUserObjects;
      private System.Windows.Forms.GroupBox grpUserTables;
      private System.Windows.Forms.CheckBox _chkCaptureSQL;
      private System.Windows.Forms.CheckBox _chkAuditSystemTables;
      private System.Windows.Forms.CheckBox _chkAuditAdmin;
      private System.Windows.Forms.RadioButton radioSelectedDML;
      private System.Windows.Forms.RadioButton radioAllDML;
      private System.Windows.Forms.CheckBox _chkAuditOther;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.CheckBox _cbFilterOnAccess;
      private System.Windows.Forms.RadioButton _rbFailed;
      private System.Windows.Forms.RadioButton _rbPassed;
      private System.Windows.Forms.TabPage _tabTrustedUsers;
      private System.Windows.Forms.ListView _lstTrustedUsers;
      private System.Windows.Forms.ColumnHeader columnHeader2;
      private System.Windows.Forms.Button _btnRemoveUser;
      private System.Windows.Forms.Button _btnAddUser;
      private System.Windows.Forms.Label label19;
      private System.Windows.Forms.LinkLabel _lnkTrustedUserHelp;
      private System.Windows.Forms.TabPage _tabBeforeAfter;
      private System.Windows.Forms.Panel _pnlBeforeAfter;
      private System.Windows.Forms.GroupBox _gbCLR;
      private System.Windows.Forms.Button _btnEnableCLR;
      private System.Windows.Forms.Label _lblClrStatus;
      private System.Windows.Forms.PictureBox _pbClrStatus;
      private System.Windows.Forms.Button _btnEditBATable;
      private System.Windows.Forms.Button _btnRemoveBATable;
      private System.Windows.Forms.Button _btnAddBATable;
      private System.Windows.Forms.ListView _lvBeforeAfterTables;
      private System.Windows.Forms.Label _lblBeforeAfterStatus;
      private System.Windows.Forms.Label _lblTrustedUserStatus;
      private System.Windows.Forms.Panel _pnlTrustedUsers;
      private System.Windows.Forms.ColumnHeader _columnColumnNames;
      private System.Windows.Forms.Panel panel1;
      private System.Windows.Forms.LinkLabel linkLblHelpBestPractices;
       private System.Windows.Forms.TabPage _tabSensitiveColumns;
       private System.Windows.Forms.Button _btnEditSCTable;
       private System.Windows.Forms.Button _btnRemoveSCTable;
       private System.Windows.Forms.Button _btnAddSCTable;
       private System.Windows.Forms.GroupBox groupBox1;
      private System.Windows.Forms.Label _lblSCStatus;
      private System.Windows.Forms.Panel _pnlSensitiveColumns;
      private System.Windows.Forms.CheckBox _chkCaptureTrans;
      private System.Windows.Forms.TabPage _tabPrivilegedUser;
      private System.Windows.Forms.ListView lstPrivilegedUsers;
      private System.Windows.Forms.ColumnHeader columnHeader4;
      private System.Windows.Forms.Button btnRemovePriv;
      private System.Windows.Forms.Button btnAddPriv;
      private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label labelPreSelectedPrivilegedUsers;
        private System.Windows.Forms.ListView lstPreSelectedPrivilegedUsers;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.GroupBox grpPrivilegedUserActivity;
      private System.Windows.Forms.GroupBox grpAuditUserActivity;
      private System.Windows.Forms.CheckBox chkUserCaptureTrans;
      private System.Windows.Forms.CheckBox chkAuditUserUserDefined;
      private System.Windows.Forms.RadioButton _rbUserAuditFailed;
      private System.Windows.Forms.RadioButton _rbUserAuditPassed;
      private System.Windows.Forms.CheckBox chkAuditUserAdmin;
      private System.Windows.Forms.CheckBox chkUserCaptureSQL;
      private System.Windows.Forms.CheckBox _cbUserFilterAccessCheck;
      private System.Windows.Forms.CheckBox chkAuditUserDML;
      private System.Windows.Forms.CheckBox chkAuditUserSELECT;
      private System.Windows.Forms.CheckBox chkAuditUserSecurity;
      private System.Windows.Forms.CheckBox chkAuditUserDDL;
      private System.Windows.Forms.CheckBox chkAuditUserFailedLogins;
      private System.Windows.Forms.CheckBox chkAuditUserLogins;
      private System.Windows.Forms.RadioButton rbAuditUserSelected;
      private System.Windows.Forms.RadioButton rbAuditUserAll;
      private System.Windows.Forms.CheckBox checkBox3;
      private System.Windows.Forms.CheckBox chkUserCaptureDDL;
      private System.Windows.Forms.CheckBox chkDBCaptureDDL;
      private System.Windows.Forms.Button _btnAddSCDataSet;
      private System.Windows.Forms.ListView _lvSCTables;
      private System.Windows.Forms.ColumnHeader columnHeader5;
      private System.Windows.Forms.ColumnHeader columnHeader6;
      private System.Windows.Forms.CheckBox chkAuditUserLogouts;
      private System.Windows.Forms.Button _btnConfigureSC;//SQlCM-5747 v5.6
    }
}