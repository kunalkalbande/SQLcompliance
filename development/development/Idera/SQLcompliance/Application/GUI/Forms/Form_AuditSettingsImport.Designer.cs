namespace Idera.SQLcompliance.Application.GUI.Forms
{
   partial class Form_AuditSettingsImport
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
         System.Windows.Forms.Label @__lblSelectAuditSettings;
         System.Windows.Forms.Label @__lblSelectFile;
         System.Windows.Forms.Label @__lblSelectDbToImport;
         this._pnlLeft = new System.Windows.Forms.Panel();
         this._pictureBox = new System.Windows.Forms.PictureBox();
         this._pnlCenter = new System.Windows.Forms.Panel();
         this._pnlBrowse = new System.Windows.Forms.Panel();
         this._tbFile = new System.Windows.Forms.TextBox();
         this._btnBrowse = new System.Windows.Forms.Button();
         this._pnlSummary = new System.Windows.Forms.Panel();
         this._rbOverwriteCurrent = new System.Windows.Forms.RadioButton();
         this._rbAddToCurrent = new System.Windows.Forms.RadioButton();
         this.@__lblFinish = new System.Windows.Forms.Label();
         this._pnlTargetDatabases = new System.Windows.Forms.Panel();
         this._lstDatabaseList = new System.Windows.Forms.ListView();
         this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
         this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
         this._btnClearDatabases = new System.Windows.Forms.Button();
         this._btnSelectAllDatabases = new System.Windows.Forms.Button();
         this.@__lblTargetDatabases = new System.Windows.Forms.Label();
         this._pnlSelectSettings = new System.Windows.Forms.Panel();
         this._lstDatabaseSettings = new System.Windows.Forms.ListBox();
         this._checkMatchDbNames = new System.Windows.Forms.CheckBox();
         this._chkServerPrivUser = new System.Windows.Forms.CheckBox();
         this._chkDatabase = new System.Windows.Forms.CheckBox();
         this._checkServer = new System.Windows.Forms.CheckBox();
         this._pnlTargetServers = new System.Windows.Forms.Panel();
         this._lstTargetServers = new System.Windows.Forms.ListView();
         this._btnClearServers = new System.Windows.Forms.Button();
         this._btnSelectAllServers = new System.Windows.Forms.Button();
         this.@__lblTargetServers = new System.Windows.Forms.Label();
         this._pnlBottom = new System.Windows.Forms.Panel();
         this._btnCancel = new System.Windows.Forms.Button();
         this._btnNext = new System.Windows.Forms.Button();
         this._btnBack = new System.Windows.Forms.Button();
         this._btnFinish = new System.Windows.Forms.Button();
         this._pnlTop = new System.Windows.Forms.Panel();
         this._lblDescription = new System.Windows.Forms.Label();
         this._lblTitle = new System.Windows.Forms.Label();
         this._lblBorder1 = new System.Windows.Forms.Label();
         this._lblBorder2 = new System.Windows.Forms.Label();
            this._chkDatabasePrivUser = new System.Windows.Forms.CheckBox();
         @__lblSelectAuditSettings = new System.Windows.Forms.Label();
         @__lblSelectFile = new System.Windows.Forms.Label();
         @__lblSelectDbToImport = new System.Windows.Forms.Label();
         this._pnlLeft.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this._pictureBox)).BeginInit();
         this._pnlCenter.SuspendLayout();
         this._pnlBrowse.SuspendLayout();
         this._pnlSummary.SuspendLayout();
         this._pnlTargetDatabases.SuspendLayout();
         this._pnlSelectSettings.SuspendLayout();
         this._pnlTargetServers.SuspendLayout();
         this._pnlBottom.SuspendLayout();
         this._pnlTop.SuspendLayout();
         this.SuspendLayout();
         // 
         // __lblSelectAuditSettings
         // 
         @__lblSelectAuditSettings.Location = new System.Drawing.Point(14, 8);
         @__lblSelectAuditSettings.Name = "__lblSelectAuditSettings";
         @__lblSelectAuditSettings.Size = new System.Drawing.Size(224, 17);
         @__lblSelectAuditSettings.TabIndex = 10;
         @__lblSelectAuditSettings.Text = "Select the audit settings to import:";
         @__lblSelectAuditSettings.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // __lblSelectFile
         // 
         @__lblSelectFile.Location = new System.Drawing.Point(14, 8);
         @__lblSelectFile.Name = "__lblSelectFile";
         @__lblSelectFile.Size = new System.Drawing.Size(182, 17);
         @__lblSelectFile.TabIndex = 2;
         @__lblSelectFile.Text = "Select an Audit Settings file to import:";
         @__lblSelectFile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // __lblSelectDbToImport
         // 
            @__lblSelectDbToImport.Location = new System.Drawing.Point(31, 143);
         @__lblSelectDbToImport.Name = "__lblSelectDbToImport";
         @__lblSelectDbToImport.Size = new System.Drawing.Size(187, 17);
         @__lblSelectDbToImport.TabIndex = 20;
         @__lblSelectDbToImport.Text = "Select the database settings to import:";
         @__lblSelectDbToImport.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // _pnlLeft
         // 
         this._pnlLeft.Controls.Add(this._pictureBox);
         this._pnlLeft.Location = new System.Drawing.Point(0, 0);
         this._pnlLeft.Name = "_pnlLeft";
         this._pnlLeft.Size = new System.Drawing.Size(110, 335);
         this._pnlLeft.TabIndex = 3;
         // 
         // _pictureBox
         // 
         this._pictureBox.Image = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.Wizard_ImportAuditSettings;
         this._pictureBox.Location = new System.Drawing.Point(0, 0);
         this._pictureBox.Name = "_pictureBox";
         this._pictureBox.Size = new System.Drawing.Size(110, 335);
         this._pictureBox.TabIndex = 0;
         this._pictureBox.TabStop = false;
         // 
         // _pnlCenter
         // 
         this._pnlCenter.Controls.Add(this._pnlBrowse);
         this._pnlCenter.Controls.Add(this._pnlSummary);
         this._pnlCenter.Controls.Add(this._pnlTargetDatabases);
         this._pnlCenter.Controls.Add(this._pnlSelectSettings);
         this._pnlCenter.Controls.Add(this._pnlTargetServers);
         this._pnlCenter.Location = new System.Drawing.Point(110, 61);
         this._pnlCenter.Name = "_pnlCenter";
         this._pnlCenter.Size = new System.Drawing.Size(446, 274);
         this._pnlCenter.TabIndex = 1;
         // 
         // _pnlBrowse
         // 
         this._pnlBrowse.Controls.Add(@__lblSelectFile);
         this._pnlBrowse.Controls.Add(this._tbFile);
         this._pnlBrowse.Controls.Add(this._btnBrowse);
         this._pnlBrowse.Dock = System.Windows.Forms.DockStyle.Fill;
         this._pnlBrowse.Location = new System.Drawing.Point(0, 0);
         this._pnlBrowse.Name = "_pnlBrowse";
         this._pnlBrowse.Size = new System.Drawing.Size(446, 274);
         this._pnlBrowse.TabIndex = 21;
         // 
         // _tbFile
         // 
         this._tbFile.Location = new System.Drawing.Point(17, 31);
         this._tbFile.Name = "_tbFile";
         this._tbFile.Size = new System.Drawing.Size(417, 20);
         this._tbFile.TabIndex = 1;
         this._tbFile.TextChanged += new System.EventHandler(this.TextChanged_tbFile);
         // 
         // _btnBrowse
         // 
         this._btnBrowse.Location = new System.Drawing.Point(359, 57);
         this._btnBrowse.Name = "_btnBrowse";
         this._btnBrowse.Size = new System.Drawing.Size(75, 23);
         this._btnBrowse.TabIndex = 0;
         this._btnBrowse.Text = "Browse...";
         this._btnBrowse.UseVisualStyleBackColor = true;
         this._btnBrowse.Click += new System.EventHandler(this.Click_btnBrowse);
         // 
         // _pnlSummary
         // 
         this._pnlSummary.Controls.Add(this._rbOverwriteCurrent);
         this._pnlSummary.Controls.Add(this._rbAddToCurrent);
         this._pnlSummary.Controls.Add(this.@__lblFinish);
         this._pnlSummary.Dock = System.Windows.Forms.DockStyle.Fill;
         this._pnlSummary.Location = new System.Drawing.Point(0, 0);
         this._pnlSummary.Name = "_pnlSummary";
         this._pnlSummary.Size = new System.Drawing.Size(446, 274);
         this._pnlSummary.TabIndex = 20;
         this._pnlSummary.Visible = false;
         // 
         // _rbOverwriteCurrent
         // 
         this._rbOverwriteCurrent.AutoSize = true;
         this._rbOverwriteCurrent.Location = new System.Drawing.Point(14, 32);
         this._rbOverwriteCurrent.Name = "_rbOverwriteCurrent";
         this._rbOverwriteCurrent.Size = new System.Drawing.Size(171, 17);
         this._rbOverwriteCurrent.TabIndex = 2;
         this._rbOverwriteCurrent.TabStop = true;
         this._rbOverwriteCurrent.Text = "Overwrite current audit settings";
         this._rbOverwriteCurrent.UseVisualStyleBackColor = true;
         // 
         // _rbAddToCurrent
         // 
         this._rbAddToCurrent.AutoSize = true;
         this._rbAddToCurrent.Checked = true;
         this._rbAddToCurrent.Location = new System.Drawing.Point(14, 8);
         this._rbAddToCurrent.Name = "_rbAddToCurrent";
         this._rbAddToCurrent.Size = new System.Drawing.Size(157, 17);
         this._rbAddToCurrent.TabIndex = 1;
         this._rbAddToCurrent.TabStop = true;
         this._rbAddToCurrent.Text = "Add to current audit settings";
         this._rbAddToCurrent.UseVisualStyleBackColor = true;
         // 
         // __lblFinish
         // 
         this.@__lblFinish.AutoSize = true;
         this.@__lblFinish.Location = new System.Drawing.Point(28, 119);
         this.@__lblFinish.Name = "__lblFinish";
         this.@__lblFinish.Size = new System.Drawing.Size(235, 13);
         this.@__lblFinish.TabIndex = 0;
         this.@__lblFinish.Text = "You are done.  Click Finish to perform the import.";
         // 
         // _pnlTargetDatabases
         // 
         this._pnlTargetDatabases.Controls.Add(this._lstDatabaseList);
         this._pnlTargetDatabases.Controls.Add(this._btnClearDatabases);
         this._pnlTargetDatabases.Controls.Add(this._btnSelectAllDatabases);
         this._pnlTargetDatabases.Controls.Add(this.@__lblTargetDatabases);
         this._pnlTargetDatabases.Dock = System.Windows.Forms.DockStyle.Fill;
         this._pnlTargetDatabases.Location = new System.Drawing.Point(0, 0);
         this._pnlTargetDatabases.Name = "_pnlTargetDatabases";
         this._pnlTargetDatabases.Size = new System.Drawing.Size(446, 274);
         this._pnlTargetDatabases.TabIndex = 17;
         this._pnlTargetDatabases.Visible = false;
         // 
         // _lstDatabaseList
         // 
         this._lstDatabaseList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
         this._lstDatabaseList.FullRowSelect = true;
         this._lstDatabaseList.HideSelection = false;
         this._lstDatabaseList.Location = new System.Drawing.Point(17, 31);
         this._lstDatabaseList.Name = "_lstDatabaseList";
         this._lstDatabaseList.Size = new System.Drawing.Size(359, 194);
         this._lstDatabaseList.TabIndex = 18;
         this._lstDatabaseList.UseCompatibleStateImageBehavior = false;
         this._lstDatabaseList.View = System.Windows.Forms.View.Details;
         this._lstDatabaseList.SelectedIndexChanged += new System.EventHandler(this.SelectedIndexChanged_lstDatabaseList);
         this._lstDatabaseList.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.ColumnClick_lstDatabaseList);
         // 
         // columnHeader1
         // 
         this.columnHeader1.Text = "Database Name";
         this.columnHeader1.Width = 171;
         // 
         // columnHeader2
         // 
         this.columnHeader2.Text = "Server Name";
         this.columnHeader2.Width = 177;
         // 
         // _btnClearDatabases
         // 
         this._btnClearDatabases.Location = new System.Drawing.Point(121, 231);
         this._btnClearDatabases.Name = "_btnClearDatabases";
         this._btnClearDatabases.Size = new System.Drawing.Size(75, 23);
         this._btnClearDatabases.TabIndex = 17;
         this._btnClearDatabases.Text = "Clear All";
         this._btnClearDatabases.UseVisualStyleBackColor = true;
         this._btnClearDatabases.Click += new System.EventHandler(this.Click_btnClearDatabases);
         // 
         // _btnSelectAllDatabases
         // 
         this._btnSelectAllDatabases.Location = new System.Drawing.Point(201, 231);
         this._btnSelectAllDatabases.Name = "_btnSelectAllDatabases";
         this._btnSelectAllDatabases.Size = new System.Drawing.Size(75, 23);
         this._btnSelectAllDatabases.TabIndex = 16;
         this._btnSelectAllDatabases.Text = "Select All";
         this._btnSelectAllDatabases.UseVisualStyleBackColor = true;
         this._btnSelectAllDatabases.Click += new System.EventHandler(this.Click_btnSelectAllDatabases);
         // 
         // __lblTargetDatabases
         // 
         this.@__lblTargetDatabases.Location = new System.Drawing.Point(14, 8);
         this.@__lblTargetDatabases.Name = "__lblTargetDatabases";
         this.@__lblTargetDatabases.Size = new System.Drawing.Size(263, 17);
         this.@__lblTargetDatabases.TabIndex = 14;
         this.@__lblTargetDatabases.Text = "Select the target databases for the import:";
         this.@__lblTargetDatabases.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // _pnlSelectSettings
         // 
            this._pnlSelectSettings.Controls.Add(this._chkDatabasePrivUser);
         this._pnlSelectSettings.Controls.Add(@__lblSelectDbToImport);
         this._pnlSelectSettings.Controls.Add(this._lstDatabaseSettings);
         this._pnlSelectSettings.Controls.Add(this._checkMatchDbNames);
         this._pnlSelectSettings.Controls.Add(this._chkServerPrivUser);
         this._pnlSelectSettings.Controls.Add(this._chkDatabase);
         this._pnlSelectSettings.Controls.Add(this._checkServer);
         this._pnlSelectSettings.Controls.Add(@__lblSelectAuditSettings);
         this._pnlSelectSettings.Dock = System.Windows.Forms.DockStyle.Fill;
         this._pnlSelectSettings.Location = new System.Drawing.Point(0, 0);
         this._pnlSelectSettings.Name = "_pnlSelectSettings";
         this._pnlSelectSettings.Size = new System.Drawing.Size(446, 274);
         this._pnlSelectSettings.TabIndex = 1;
         this._pnlSelectSettings.Visible = false;
         // 
         // _lstDatabaseSettings
         // 
         this._lstDatabaseSettings.FormattingEnabled = true;
            this._lstDatabaseSettings.Location = new System.Drawing.Point(34, 164);
         this._lstDatabaseSettings.Name = "_lstDatabaseSettings";
         this._lstDatabaseSettings.Size = new System.Drawing.Size(184, 95);
         this._lstDatabaseSettings.TabIndex = 19;
         // 
         // _checkMatchDbNames
         // 
         this._checkMatchDbNames.AutoSize = true;
            this._checkMatchDbNames.Location = new System.Drawing.Point(34, 123);
         this._checkMatchDbNames.Name = "_checkMatchDbNames";
         this._checkMatchDbNames.Size = new System.Drawing.Size(220, 17);
         this._checkMatchDbNames.TabIndex = 18;
         this._checkMatchDbNames.Text = "Only import for matching database names";
         this._checkMatchDbNames.UseVisualStyleBackColor = true;
         this._checkMatchDbNames.CheckedChanged += new System.EventHandler(this.CheckChanged_checkMatchDbNames);
         // 
         // _checkPrivUser
         // 
         this._chkServerPrivUser.AutoSize = true;
         this._chkServerPrivUser.Location = new System.Drawing.Point(17, 54);
         this._chkServerPrivUser.Name = "_checkPrivUser";
            this._chkServerPrivUser.Size = new System.Drawing.Size(199, 17);
         this._chkServerPrivUser.TabIndex = 17;
            this._chkServerPrivUser.Text = "Server Privileged User Audit Settings";
         this._chkServerPrivUser.UseVisualStyleBackColor = true;
         this._chkServerPrivUser.CheckedChanged += new System.EventHandler(this.CheckChanged_all);
         // 
         // _checkDatabase
         // 
         this._chkDatabase.AutoSize = true;
         this._chkDatabase.Location = new System.Drawing.Point(17, 77);
         this._chkDatabase.Name = "_checkDatabase";
         this._chkDatabase.Size = new System.Drawing.Size(140, 17);
         this._chkDatabase.TabIndex = 16;
         this._chkDatabase.Text = "Database Audit Settings";
         this._chkDatabase.UseVisualStyleBackColor = true;
         this._chkDatabase.CheckedChanged += new System.EventHandler(this.CheckChanged_all);
         // 
         // _checkServer
         // 
         this._checkServer.AutoSize = true;
         this._checkServer.Location = new System.Drawing.Point(17, 31);
         this._checkServer.Name = "_checkServer";
         this._checkServer.Size = new System.Drawing.Size(125, 17);
         this._checkServer.TabIndex = 15;
         this._checkServer.Text = "Server Audit Settings";
         this._checkServer.UseVisualStyleBackColor = true;
         this._checkServer.CheckedChanged += new System.EventHandler(this.CheckChanged_all);
         // 
         // _pnlTargetServers
         // 
         this._pnlTargetServers.Controls.Add(this._lstTargetServers);
         this._pnlTargetServers.Controls.Add(this._btnClearServers);
         this._pnlTargetServers.Controls.Add(this._btnSelectAllServers);
         this._pnlTargetServers.Controls.Add(this.@__lblTargetServers);
         this._pnlTargetServers.Dock = System.Windows.Forms.DockStyle.Fill;
         this._pnlTargetServers.Location = new System.Drawing.Point(0, 0);
         this._pnlTargetServers.Name = "_pnlTargetServers";
         this._pnlTargetServers.Size = new System.Drawing.Size(446, 274);
         this._pnlTargetServers.TabIndex = 2;
         this._pnlTargetServers.Visible = false;
         // 
         // _lstTargetServers
         // 
         this._lstTargetServers.HideSelection = false;
         this._lstTargetServers.Location = new System.Drawing.Point(14, 31);
         this._lstTargetServers.Name = "_lstTargetServers";
         this._lstTargetServers.Size = new System.Drawing.Size(201, 199);
         this._lstTargetServers.TabIndex = 14;
         this._lstTargetServers.UseCompatibleStateImageBehavior = false;
         this._lstTargetServers.View = System.Windows.Forms.View.List;
         this._lstTargetServers.SelectedIndexChanged += new System.EventHandler(this.SelectedIndexChanged_lstTargetServers);
         // 
         // _btnClearServers
         // 
         this._btnClearServers.Location = new System.Drawing.Point(39, 236);
         this._btnClearServers.Name = "_btnClearServers";
         this._btnClearServers.Size = new System.Drawing.Size(75, 23);
         this._btnClearServers.TabIndex = 13;
         this._btnClearServers.Text = "Clear All";
         this._btnClearServers.UseVisualStyleBackColor = true;
         this._btnClearServers.Click += new System.EventHandler(this.Click_clearAllServers);
         // 
         // _btnSelectAllServers
         // 
         this._btnSelectAllServers.Location = new System.Drawing.Point(120, 236);
         this._btnSelectAllServers.Name = "_btnSelectAllServers";
         this._btnSelectAllServers.Size = new System.Drawing.Size(75, 23);
         this._btnSelectAllServers.TabIndex = 12;
         this._btnSelectAllServers.Text = "Select All";
         this._btnSelectAllServers.UseVisualStyleBackColor = true;
         this._btnSelectAllServers.Click += new System.EventHandler(this.Click_selectAllServers);
         // 
         // __lblTargetServers
         // 
         this.@__lblTargetServers.Location = new System.Drawing.Point(14, 8);
         this.@__lblTargetServers.Name = "__lblTargetServers";
         this.@__lblTargetServers.Size = new System.Drawing.Size(263, 17);
         this.@__lblTargetServers.TabIndex = 10;
         this.@__lblTargetServers.Text = "Select the target servers for import:";
         this.@__lblTargetServers.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // _pnlBottom
         // 
         this._pnlBottom.Controls.Add(this._btnCancel);
         this._pnlBottom.Controls.Add(this._btnNext);
         this._pnlBottom.Controls.Add(this._btnBack);
         this._pnlBottom.Controls.Add(this._btnFinish);
         this._pnlBottom.Location = new System.Drawing.Point(0, 336);
         this._pnlBottom.Name = "_pnlBottom";
         this._pnlBottom.Size = new System.Drawing.Size(556, 38);
         this._pnlBottom.TabIndex = 0;
         // 
         // _btnCancel
         // 
         this._btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this._btnCancel.Location = new System.Drawing.Point(490, 10);
         this._btnCancel.Name = "_btnCancel";
         this._btnCancel.Size = new System.Drawing.Size(62, 20);
         this._btnCancel.TabIndex = 3;
         this._btnCancel.Text = "Cancel";
         // 
         // _btnNext
         // 
         this._btnNext.Enabled = false;
         this._btnNext.Location = new System.Drawing.Point(350, 10);
         this._btnNext.Name = "_btnNext";
         this._btnNext.Size = new System.Drawing.Size(62, 20);
         this._btnNext.TabIndex = 0;
         this._btnNext.Text = "Next >";
         this._btnNext.Click += new System.EventHandler(this.Click_btnNext);
         // 
         // _btnBack
         // 
         this._btnBack.Enabled = false;
         this._btnBack.Location = new System.Drawing.Point(280, 10);
         this._btnBack.Name = "_btnBack";
         this._btnBack.Size = new System.Drawing.Size(62, 20);
         this._btnBack.TabIndex = 0;
         this._btnBack.Text = "< Back";
         this._btnBack.Click += new System.EventHandler(this.Click_btnBack);
         // 
         // _btnFinish
         // 
         this._btnFinish.DialogResult = System.Windows.Forms.DialogResult.OK;
         this._btnFinish.Enabled = false;
         this._btnFinish.Location = new System.Drawing.Point(420, 10);
         this._btnFinish.Name = "_btnFinish";
         this._btnFinish.Size = new System.Drawing.Size(62, 20);
         this._btnFinish.TabIndex = 2;
         this._btnFinish.Text = "Finish";
         this._btnFinish.Click += new System.EventHandler(this.Click_btnFinish);
         // 
         // _pnlTop
         // 
         this._pnlTop.BackColor = System.Drawing.Color.White;
         this._pnlTop.Controls.Add(this._lblDescription);
         this._pnlTop.Controls.Add(this._lblTitle);
         this._pnlTop.Location = new System.Drawing.Point(110, 0);
         this._pnlTop.Name = "_pnlTop";
         this._pnlTop.Size = new System.Drawing.Size(446, 60);
         this._pnlTop.TabIndex = 2;
         // 
         // _lblDescription
         // 
         this._lblDescription.Location = new System.Drawing.Point(14, 24);
         this._lblDescription.Name = "_lblDescription";
         this._lblDescription.Size = new System.Drawing.Size(416, 28);
         this._lblDescription.TabIndex = 1;
         this._lblDescription.Text = "Description";
         // 
         // _lblTitle
         // 
         this._lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this._lblTitle.Location = new System.Drawing.Point(14, 8);
         this._lblTitle.Name = "_lblTitle";
         this._lblTitle.Size = new System.Drawing.Size(281, 16);
         this._lblTitle.TabIndex = 0;
         this._lblTitle.Text = "Title";
         this._lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // _lblBorder1
         // 
         this._lblBorder1.BackColor = System.Drawing.Color.Black;
         this._lblBorder1.Location = new System.Drawing.Point(110, 60);
         this._lblBorder1.Name = "_lblBorder1";
         this._lblBorder1.Size = new System.Drawing.Size(446, 1);
         this._lblBorder1.TabIndex = 3;
         this._lblBorder1.Text = "label1";
         // 
         // _lblBorder2
         // 
         this._lblBorder2.BackColor = System.Drawing.Color.Black;
         this._lblBorder2.Location = new System.Drawing.Point(0, 335);
         this._lblBorder2.Name = "_lblBorder2";
         this._lblBorder2.Size = new System.Drawing.Size(556, 1);
         this._lblBorder2.TabIndex = 4;
         this._lblBorder2.Text = "label2";
         // 
            // checkBox1
            // 
            this._chkDatabasePrivUser.AutoSize = true;
            this._chkDatabasePrivUser.Location = new System.Drawing.Point(17, 100);
            this._chkDatabasePrivUser.Name = "checkBox1";
            this._chkDatabasePrivUser.Size = new System.Drawing.Size(214, 17);
            this._chkDatabasePrivUser.TabIndex = 21;
            this._chkDatabasePrivUser.Text = "Database Privileged User Audit Settings";
            this._chkDatabasePrivUser.UseVisualStyleBackColor = true;
            this._chkDatabasePrivUser.CheckedChanged += new System.EventHandler(this.CheckChanged_all);

            // 
         // Form_AuditSettingsImport
         // 
         this.AcceptButton = this._btnFinish;
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.CancelButton = this._btnCancel;
         this.ClientSize = new System.Drawing.Size(556, 374);
         this.Controls.Add(this._lblBorder2);
         this.Controls.Add(this._lblBorder1);
         this.Controls.Add(this._pnlLeft);
         this.Controls.Add(this._pnlBottom);
         this.Controls.Add(this._pnlCenter);
         this.Controls.Add(this._pnlTop);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.HelpButton = true;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "Form_AuditSettingsImport";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Import Audit Settings";
         this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.Form_AuditSettingsImport_HelpRequested);
         this._pnlLeft.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this._pictureBox)).EndInit();
         this._pnlCenter.ResumeLayout(false);
         this._pnlBrowse.ResumeLayout(false);
         this._pnlBrowse.PerformLayout();
         this._pnlSummary.ResumeLayout(false);
         this._pnlSummary.PerformLayout();
         this._pnlTargetDatabases.ResumeLayout(false);
         this._pnlSelectSettings.ResumeLayout(false);
         this._pnlSelectSettings.PerformLayout();
         this._pnlTargetServers.ResumeLayout(false);
         this._pnlBottom.ResumeLayout(false);
         this._pnlTop.ResumeLayout(false);
         this.ResumeLayout(false);

      }
      #endregion

      private System.Windows.Forms.Panel _pnlLeft;
      private System.Windows.Forms.Panel _pnlCenter;
      private System.Windows.Forms.Panel _pnlBottom;
      private System.Windows.Forms.Button _btnBack;
      private System.Windows.Forms.Button _btnNext;
      private System.Windows.Forms.Button _btnFinish;
      private System.Windows.Forms.Button _btnCancel;
      private System.Windows.Forms.PictureBox _pictureBox;
      private System.Windows.Forms.Panel _pnlTop;
      private System.Windows.Forms.Label _lblTitle;
      private System.Windows.Forms.Label _lblDescription;
      private System.Windows.Forms.Label _lblBorder2;
      private System.Windows.Forms.Label _lblBorder1;
      private System.Windows.Forms.Label __lblTargetServers;
      private System.Windows.Forms.Panel _pnlTargetDatabases;
      private System.Windows.Forms.Panel _pnlTargetServers;
      private System.Windows.Forms.Panel _pnlSelectSettings;
      private System.Windows.Forms.CheckBox _chkServerPrivUser;
      private System.Windows.Forms.CheckBox _chkDatabase;
      private System.Windows.Forms.CheckBox _checkServer;
      private System.Windows.Forms.CheckBox _checkMatchDbNames;
      private System.Windows.Forms.ListBox _lstDatabaseSettings;
      private System.Windows.Forms.Button _btnClearServers;
      private System.Windows.Forms.Button _btnSelectAllServers;
      private System.Windows.Forms.ListView _lstDatabaseList;
      private System.Windows.Forms.ColumnHeader columnHeader1;
      private System.Windows.Forms.ColumnHeader columnHeader2;
      private System.Windows.Forms.Button _btnClearDatabases;
      private System.Windows.Forms.Button _btnSelectAllDatabases;
      private System.Windows.Forms.Label __lblTargetDatabases;
      private System.Windows.Forms.Panel _pnlSummary;
      private System.Windows.Forms.RadioButton _rbOverwriteCurrent;
      private System.Windows.Forms.RadioButton _rbAddToCurrent;
      private System.Windows.Forms.Label __lblFinish;
      private System.Windows.Forms.Panel _pnlBrowse;
      private System.Windows.Forms.TextBox _tbFile;
      private System.Windows.Forms.Button _btnBrowse;
      private System.Windows.Forms.ListView _lstTargetServers;
      private System.Windows.Forms.CheckBox _chkDatabasePrivUser;
	}
}