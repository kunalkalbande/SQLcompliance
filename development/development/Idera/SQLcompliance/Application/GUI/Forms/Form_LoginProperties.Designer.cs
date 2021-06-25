namespace Idera.SQLcompliance.Application.GUI.Forms
{
   partial class Form_LoginProperties
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageGeneral = new System.Windows.Forms.TabPage();
            this.chkWebApplicationAccess = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioThroughGroup = new System.Windows.Forms.RadioButton();
            this.radioDenyAccess = new System.Windows.Forms.RadioButton();
            this.radioGrantAccess = new System.Windows.Forms.RadioButton();
            this.textName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupPermissions = new System.Windows.Forms.GroupBox();
            this.radioAuditor = new System.Windows.Forms.RadioButton();
            this.radioSysadmin = new System.Windows.Forms.RadioButton();
            this.label7 = new System.Windows.Forms.Label();
            this.tabPageAccess = new System.Windows.Forms.TabPage();
            this.listDatabases = new System.Windows.Forms.CheckedListBox();
            this.groupDatabasePermissions = new System.Windows.Forms.GroupBox();
            this.radioDatabaseDeny = new System.Windows.Forms.RadioButton();
            this.radioDatabaseEventsOnly = new System.Windows.Forms.RadioButton();
            this.radioDatabaseReadAll = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.tabPageReportAccess = new System.Windows.Forms.TabPage();
            this.listReports = new System.Windows.Forms.CheckedListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPageGeneral.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupPermissions.SuspendLayout();
            this.tabPageAccess.SuspendLayout();
            this.groupDatabasePermissions.SuspendLayout();
            this.tabPageReportAccess.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(320, 332);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 14;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(240, 332);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 13;
            this.btnOK.Text = "&OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPageGeneral);
            this.tabControl1.Controls.Add(this.tabPageAccess);
            this.tabControl1.Controls.Add(this.tabPageReportAccess);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(404, 324);
            this.tabControl1.TabIndex = 36;
            // 
            // tabPageGeneral
            // 
            this.tabPageGeneral.Controls.Add(this.chkWebApplicationAccess);
            this.tabPageGeneral.Controls.Add(this.groupBox1);
            this.tabPageGeneral.Controls.Add(this.textName);
            this.tabPageGeneral.Controls.Add(this.label1);
            this.tabPageGeneral.Controls.Add(this.groupPermissions);
            this.tabPageGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabPageGeneral.Name = "tabPageGeneral";
            this.tabPageGeneral.Size = new System.Drawing.Size(396, 298);
            this.tabPageGeneral.TabIndex = 0;
            this.tabPageGeneral.Text = "General";
            this.tabPageGeneral.UseVisualStyleBackColor = true;
            // 
            // chkWebApplicationAccess
            // 
            this.chkWebApplicationAccess.AutoSize = true;
            this.chkWebApplicationAccess.Location = new System.Drawing.Point(12, 227);
            this.chkWebApplicationAccess.Name = "chkWebApplicationAccess";
            this.chkWebApplicationAccess.Size = new System.Drawing.Size(142, 17);
            this.chkWebApplicationAccess.TabIndex = 9;
            this.chkWebApplicationAccess.Text = "Web Application Access";
            this.chkWebApplicationAccess.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioThroughGroup);
            this.groupBox1.Controls.Add(this.radioDenyAccess);
            this.groupBox1.Controls.Add(this.radioGrantAccess);
            this.groupBox1.Location = new System.Drawing.Point(12, 44);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(372, 68);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Security access";
            // 
            // radioThroughGroup
            // 
            this.radioThroughGroup.Location = new System.Drawing.Point(136, 20);
            this.radioThroughGroup.Name = "radioThroughGroup";
            this.radioThroughGroup.Size = new System.Drawing.Size(180, 17);
            this.radioThroughGroup.TabIndex = 16;
            this.radioThroughGroup.Text = "Through group membership";
            this.radioThroughGroup.Visible = false;
            // 
            // radioDenyAccess
            // 
            this.radioDenyAccess.Location = new System.Drawing.Point(12, 44);
            this.radioDenyAccess.Name = "radioDenyAccess";
            this.radioDenyAccess.Size = new System.Drawing.Size(88, 17);
            this.radioDenyAccess.TabIndex = 15;
            this.radioDenyAccess.Text = "Deny access";
            // 
            // radioGrantAccess
            // 
            this.radioGrantAccess.Location = new System.Drawing.Point(12, 20);
            this.radioGrantAccess.Name = "radioGrantAccess";
            this.radioGrantAccess.Size = new System.Drawing.Size(93, 17);
            this.radioGrantAccess.TabIndex = 14;
            this.radioGrantAccess.Text = "Grant access";
            // 
            // textName
            // 
            this.textName.Location = new System.Drawing.Point(48, 12);
            this.textName.Name = "textName";
            this.textName.ReadOnly = true;
            this.textName.Size = new System.Drawing.Size(336, 20);
            this.textName.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 16);
            this.label1.TabIndex = 5;
            this.label1.Text = "Name:";
            // 
            // groupPermissions
            // 
            this.groupPermissions.Controls.Add(this.radioAuditor);
            this.groupPermissions.Controls.Add(this.radioSysadmin);
            this.groupPermissions.Controls.Add(this.label7);
            this.groupPermissions.Location = new System.Drawing.Point(12, 124);
            this.groupPermissions.Name = "groupPermissions";
            this.groupPermissions.Size = new System.Drawing.Size(372, 96);
            this.groupPermissions.TabIndex = 3;
            this.groupPermissions.TabStop = false;
            this.groupPermissions.Text = "Permissions within SQL Compliance Manager";
            // 
            // radioAuditor
            // 
            this.radioAuditor.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.radioAuditor.Checked = true;
            this.radioAuditor.Location = new System.Drawing.Point(12, 68);
            this.radioAuditor.Name = "radioAuditor";
            this.radioAuditor.Size = new System.Drawing.Size(196, 20);
            this.radioAuditor.TabIndex = 61;
            this.radioAuditor.TabStop = true;
            this.radioAuditor.Text = "Can view and report on audit data. ";
            this.radioAuditor.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            // 
            // radioSysadmin
            // 
            this.radioSysadmin.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.radioSysadmin.Location = new System.Drawing.Point(12, 20);
            this.radioSysadmin.Name = "radioSysadmin";
            this.radioSysadmin.Size = new System.Drawing.Size(344, 48);
            this.radioSysadmin.TabIndex = 60;
            this.radioSysadmin.Text = "Can configure SQL Compliance Manager settings and view audit data.  This login is" +
    " a member of the Systems Administrators role on the SQL Server instance that hos" +
    "t the Repository.";
            this.radioSysadmin.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(8, 20);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(254, 16);
            this.label7.TabIndex = 59;
            // 
            // tabPageAccess
            // 
            this.tabPageAccess.Controls.Add(this.listDatabases);
            this.tabPageAccess.Controls.Add(this.groupDatabasePermissions);
            this.tabPageAccess.Controls.Add(this.label3);
            this.tabPageAccess.Location = new System.Drawing.Point(4, 22);
            this.tabPageAccess.Name = "tabPageAccess";
            this.tabPageAccess.Size = new System.Drawing.Size(396, 298);
            this.tabPageAccess.TabIndex = 2;
            this.tabPageAccess.Text = "Database Access";
            this.tabPageAccess.UseVisualStyleBackColor = true;
            // 
            // listDatabases
            // 
            this.listDatabases.Location = new System.Drawing.Point(8, 40);
            this.listDatabases.Name = "listDatabases";
            this.listDatabases.Size = new System.Drawing.Size(380, 154);
            this.listDatabases.Sorted = true;
            this.listDatabases.TabIndex = 3;
            this.listDatabases.ThreeDCheckBoxes = true;
            this.listDatabases.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.listDatabases_ItemCheck);
            this.listDatabases.SelectedIndexChanged += new System.EventHandler(this.listDatabases_SelectedIndexChanged);
            // 
            // groupDatabasePermissions
            // 
            this.groupDatabasePermissions.Controls.Add(this.radioDatabaseDeny);
            this.groupDatabasePermissions.Controls.Add(this.radioDatabaseEventsOnly);
            this.groupDatabasePermissions.Controls.Add(this.radioDatabaseReadAll);
            this.groupDatabasePermissions.Location = new System.Drawing.Point(8, 200);
            this.groupDatabasePermissions.Name = "groupDatabasePermissions";
            this.groupDatabasePermissions.Size = new System.Drawing.Size(376, 92);
            this.groupDatabasePermissions.TabIndex = 41;
            this.groupDatabasePermissions.TabStop = false;
            this.groupDatabasePermissions.Text = "Permissions in \'database\'";
            // 
            // radioDatabaseDeny
            // 
            this.radioDatabaseDeny.Location = new System.Drawing.Point(8, 68);
            this.radioDatabaseDeny.Name = "radioDatabaseDeny";
            this.radioDatabaseDeny.Size = new System.Drawing.Size(320, 17);
            this.radioDatabaseDeny.TabIndex = 6;
            this.radioDatabaseDeny.Text = "Deny ability to read events and associated SQL statements";
            this.radioDatabaseDeny.CheckedChanged += new System.EventHandler(this.radioDatabase_CheckedChanged);
            // 
            // radioDatabaseEventsOnly
            // 
            this.radioDatabaseEventsOnly.Location = new System.Drawing.Point(8, 44);
            this.radioDatabaseEventsOnly.Name = "radioDatabaseEventsOnly";
            this.radioDatabaseEventsOnly.Size = new System.Drawing.Size(332, 17);
            this.radioDatabaseEventsOnly.TabIndex = 5;
            this.radioDatabaseEventsOnly.Text = "Can read events only";
            this.radioDatabaseEventsOnly.CheckedChanged += new System.EventHandler(this.radioDatabase_CheckedChanged);
            // 
            // radioDatabaseReadAll
            // 
            this.radioDatabaseReadAll.Location = new System.Drawing.Point(8, 20);
            this.radioDatabaseReadAll.Name = "radioDatabaseReadAll";
            this.radioDatabaseReadAll.Size = new System.Drawing.Size(344, 17);
            this.radioDatabaseReadAll.TabIndex = 4;
            this.radioDatabaseReadAll.Text = "Can read events and their associated SQL statements .";
            this.radioDatabaseReadAll.CheckedChanged += new System.EventHandler(this.radioDatabase_CheckedChanged);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(8, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(376, 28);
            this.label3.TabIndex = 4;
            this.label3.Text = "If you set Repository events databases to limit or deny access by default, select" +
    " the databases you want this login to access.";
            // 
            // tabPageReportAccess
            // 
            this.tabPageReportAccess.Controls.Add(this.listReports);
            this.tabPageReportAccess.Controls.Add(this.label2);
            this.tabPageReportAccess.Location = new System.Drawing.Point(4, 22);
            this.tabPageReportAccess.Name = "tabPageReportAccess";
            this.tabPageReportAccess.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageReportAccess.Size = new System.Drawing.Size(396, 298);
            this.tabPageReportAccess.TabIndex = 3;
            this.tabPageReportAccess.Text = "Report Access";
            this.tabPageReportAccess.UseVisualStyleBackColor = true;
            // 
            // listReports
            // 
            this.listReports.Location = new System.Drawing.Point(6, 22);
            this.listReports.Name = "listReports";
            this.listReports.Size = new System.Drawing.Size(380, 259);
            this.listReports.Sorted = true;
            this.listReports.TabIndex = 6;
            this.listReports.ThreeDCheckBoxes = true;
            this.listReports.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.listReports_ItemCheck);
            this.listReports.SelectedIndexChanged += new System.EventHandler(this.listReports_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(6, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(376, 16);
            this.label2.TabIndex = 5;
            this.label2.Text = "Select the reports available for this user.";
            // 
            // Form_LoginProperties
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(402, 364);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form_LoginProperties";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Login Properties";
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.Form_LoginProperties_HelpRequested);
            this.tabControl1.ResumeLayout(false);
            this.tabPageGeneral.ResumeLayout(false);
            this.tabPageGeneral.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupPermissions.ResumeLayout(false);
            this.tabPageAccess.ResumeLayout(false);
            this.groupDatabasePermissions.ResumeLayout(false);
            this.tabPageReportAccess.ResumeLayout(false);
            this.ResumeLayout(false);

      }
      #endregion

      private System.Windows.Forms.Button btnCancel;
      private System.Windows.Forms.Button btnOK;
      private System.Windows.Forms.TabControl tabControl1;
      private System.Windows.Forms.TabPage tabPageGeneral;
      private System.Windows.Forms.TabPage tabPageAccess;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.CheckedListBox listDatabases;
      private System.Windows.Forms.Label label7;
      private System.Windows.Forms.TextBox textName;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.RadioButton radioAuditor;
      private System.Windows.Forms.GroupBox groupBox1;
      private System.Windows.Forms.RadioButton radioThroughGroup;
      private System.Windows.Forms.RadioButton radioDenyAccess;
      private System.Windows.Forms.RadioButton radioGrantAccess;
      private System.Windows.Forms.GroupBox groupDatabasePermissions;
      private System.Windows.Forms.GroupBox groupPermissions;
      private System.Windows.Forms.RadioButton radioDatabaseReadAll;
      private System.Windows.Forms.RadioButton radioDatabaseEventsOnly;
      private System.Windows.Forms.RadioButton radioDatabaseDeny;
      private System.Windows.Forms.RadioButton radioSysadmin;
      private System.Windows.Forms.CheckBox chkWebApplicationAccess;
        private System.Windows.Forms.TabPage tabPageReportAccess;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckedListBox listReports;
    }
}