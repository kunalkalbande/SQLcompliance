namespace Idera.SQLcompliance.Application.GUI.Forms
{
   partial class Form_ArchiveProperties
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
         this.groupBox1 = new System.Windows.Forms.GroupBox();
         this.textEndDate = new System.Windows.Forms.TextBox();
         this.textStartDate = new System.Windows.Forms.TextBox();
         this.lblTimeCreated = new System.Windows.Forms.Label();
         this.lblStatus = new System.Windows.Forms.Label();
         this.textLastIntegrityCheckResults = new System.Windows.Forms.TextBox();
         this.label5 = new System.Windows.Forms.Label();
         this.textLastIntegrityCheck = new System.Windows.Forms.TextBox();
         this.label4 = new System.Windows.Forms.Label();
         this.textDatabase = new System.Windows.Forms.TextBox();
         this.label3 = new System.Windows.Forms.Label();
         this.textIntegrityStatus = new System.Windows.Forms.TextBox();
         this.label2 = new System.Windows.Forms.Label();
         this.textDisplayName = new System.Windows.Forms.TextBox();
         this.label1 = new System.Windows.Forms.Label();
         this.textServer = new System.Windows.Forms.TextBox();
         this.lblServer = new System.Windows.Forms.Label();
         this.textDescription = new System.Windows.Forms.TextBox();
         this.lblDescription = new System.Windows.Forms.Label();
         this.tabPagePermissions = new System.Windows.Forms.TabPage();
         this.label16 = new System.Windows.Forms.Label();
         this.groupBox3 = new System.Windows.Forms.GroupBox();
         this.radioGrantAll = new System.Windows.Forms.RadioButton();
         this.radioGrantEventsOnly = new System.Windows.Forms.RadioButton();
         this.radioDeny = new System.Windows.Forms.RadioButton();
         this.tabControl1.SuspendLayout();
         this.tabPageGeneral.SuspendLayout();
         this.groupBox1.SuspendLayout();
         this.tabPagePermissions.SuspendLayout();
         this.groupBox3.SuspendLayout();
         this.SuspendLayout();
         // 
         // btnCancel
         // 
         this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.btnCancel.Location = new System.Drawing.Point(392, 364);
         this.btnCancel.Name = "btnCancel";
         this.btnCancel.Size = new System.Drawing.Size(75, 23);
         this.btnCancel.TabIndex = 14;
         this.btnCancel.Text = "&Cancel";
         this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
         // 
         // btnOK
         // 
         this.btnOK.Location = new System.Drawing.Point(312, 364);
         this.btnOK.Name = "btnOK";
         this.btnOK.Size = new System.Drawing.Size(75, 23);
         this.btnOK.TabIndex = 13;
         this.btnOK.Text = "&OK";
         this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
         // 
         // tabControl1
         // 
         this.tabControl1.Controls.Add(this.tabPageGeneral);
         this.tabControl1.Controls.Add(this.tabPagePermissions);
         this.tabControl1.Location = new System.Drawing.Point(0, 0);
         this.tabControl1.Name = "tabControl1";
         this.tabControl1.SelectedIndex = 0;
         this.tabControl1.Size = new System.Drawing.Size(476, 356);
         this.tabControl1.TabIndex = 36;
         // 
         // tabPageGeneral
         // 
         this.tabPageGeneral.Controls.Add(this.groupBox1);
         this.tabPageGeneral.Controls.Add(this.textDisplayName);
         this.tabPageGeneral.Controls.Add(this.label1);
         this.tabPageGeneral.Controls.Add(this.textServer);
         this.tabPageGeneral.Controls.Add(this.lblServer);
         this.tabPageGeneral.Controls.Add(this.textDescription);
         this.tabPageGeneral.Controls.Add(this.lblDescription);
         this.tabPageGeneral.Location = new System.Drawing.Point(4, 22);
         this.tabPageGeneral.Name = "tabPageGeneral";
         this.tabPageGeneral.Size = new System.Drawing.Size(468, 330);
         this.tabPageGeneral.TabIndex = 0;
         this.tabPageGeneral.Text = "General";
         this.tabPageGeneral.UseVisualStyleBackColor = true;
         // 
         // groupBox1
         // 
         this.groupBox1.Controls.Add(this.textEndDate);
         this.groupBox1.Controls.Add(this.textStartDate);
         this.groupBox1.Controls.Add(this.lblTimeCreated);
         this.groupBox1.Controls.Add(this.lblStatus);
         this.groupBox1.Controls.Add(this.textLastIntegrityCheckResults);
         this.groupBox1.Controls.Add(this.label5);
         this.groupBox1.Controls.Add(this.textLastIntegrityCheck);
         this.groupBox1.Controls.Add(this.label4);
         this.groupBox1.Controls.Add(this.textDatabase);
         this.groupBox1.Controls.Add(this.label3);
         this.groupBox1.Controls.Add(this.textIntegrityStatus);
         this.groupBox1.Controls.Add(this.label2);
         this.groupBox1.Location = new System.Drawing.Point(12, 152);
         this.groupBox1.Name = "groupBox1";
         this.groupBox1.Size = new System.Drawing.Size(444, 164);
         this.groupBox1.TabIndex = 16;
         this.groupBox1.TabStop = false;
         this.groupBox1.Text = "Archive Database Summary";
         // 
         // textEndDate
         // 
         this.textEndDate.Location = new System.Drawing.Point(284, 40);
         this.textEndDate.Name = "textEndDate";
         this.textEndDate.ReadOnly = true;
         this.textEndDate.Size = new System.Drawing.Size(112, 20);
         this.textEndDate.TabIndex = 19;
         this.textEndDate.TabStop = false;
         this.textEndDate.Text = "12/13/2004 9:15 AM";
         // 
         // textStartDate
         // 
         this.textStartDate.Location = new System.Drawing.Point(152, 40);
         this.textStartDate.Name = "textStartDate";
         this.textStartDate.ReadOnly = true;
         this.textStartDate.Size = new System.Drawing.Size(112, 20);
         this.textStartDate.TabIndex = 18;
         this.textStartDate.TabStop = false;
         // 
         // lblTimeCreated
         // 
         this.lblTimeCreated.Location = new System.Drawing.Point(268, 44);
         this.lblTimeCreated.Name = "lblTimeCreated";
         this.lblTimeCreated.Size = new System.Drawing.Size(16, 16);
         this.lblTimeCreated.TabIndex = 17;
         this.lblTimeCreated.Text = "to";
         // 
         // lblStatus
         // 
         this.lblStatus.Location = new System.Drawing.Point(8, 44);
         this.lblStatus.Name = "lblStatus";
         this.lblStatus.Size = new System.Drawing.Size(96, 16);
         this.lblStatus.TabIndex = 16;
         this.lblStatus.Text = "Event Time Span:";
         // 
         // textLastIntegrityCheckResults
         // 
         this.textLastIntegrityCheckResults.Location = new System.Drawing.Point(152, 132);
         this.textLastIntegrityCheckResults.Name = "textLastIntegrityCheckResults";
         this.textLastIntegrityCheckResults.ReadOnly = true;
         this.textLastIntegrityCheckResults.Size = new System.Drawing.Size(284, 20);
         this.textLastIntegrityCheckResults.TabIndex = 10;
         this.textLastIntegrityCheckResults.TabStop = false;
         // 
         // label5
         // 
         this.label5.Location = new System.Drawing.Point(8, 136);
         this.label5.Name = "label5";
         this.label5.Size = new System.Drawing.Size(144, 16);
         this.label5.TabIndex = 9;
         this.label5.Text = "&Last Integrity Check Result:";
         // 
         // textLastIntegrityCheck
         // 
         this.textLastIntegrityCheck.Location = new System.Drawing.Point(152, 108);
         this.textLastIntegrityCheck.Name = "textLastIntegrityCheck";
         this.textLastIntegrityCheck.ReadOnly = true;
         this.textLastIntegrityCheck.Size = new System.Drawing.Size(284, 20);
         this.textLastIntegrityCheck.TabIndex = 8;
         this.textLastIntegrityCheck.TabStop = false;
         // 
         // label4
         // 
         this.label4.Location = new System.Drawing.Point(8, 112);
         this.label4.Name = "label4";
         this.label4.Size = new System.Drawing.Size(108, 16);
         this.label4.TabIndex = 7;
         this.label4.Text = "&Last Integrity Check:";
         // 
         // textDatabase
         // 
         this.textDatabase.Location = new System.Drawing.Point(152, 16);
         this.textDatabase.Name = "textDatabase";
         this.textDatabase.ReadOnly = true;
         this.textDatabase.Size = new System.Drawing.Size(284, 20);
         this.textDatabase.TabIndex = 6;
         this.textDatabase.TabStop = false;
         // 
         // label3
         // 
         this.label3.Location = new System.Drawing.Point(8, 20);
         this.label3.Name = "label3";
         this.label3.Size = new System.Drawing.Size(92, 16);
         this.label3.TabIndex = 5;
         this.label3.Text = "Database &Name:";
         // 
         // textIntegrityStatus
         // 
         this.textIntegrityStatus.Location = new System.Drawing.Point(152, 84);
         this.textIntegrityStatus.Name = "textIntegrityStatus";
         this.textIntegrityStatus.ReadOnly = true;
         this.textIntegrityStatus.Size = new System.Drawing.Size(284, 20);
         this.textIntegrityStatus.TabIndex = 4;
         this.textIntegrityStatus.TabStop = false;
         // 
         // label2
         // 
         this.label2.Location = new System.Drawing.Point(8, 88);
         this.label2.Name = "label2";
         this.label2.Size = new System.Drawing.Size(100, 16);
         this.label2.TabIndex = 3;
         this.label2.Text = "&Database Integrity:";
         // 
         // textDisplayName
         // 
         this.textDisplayName.Location = new System.Drawing.Point(96, 36);
         this.textDisplayName.MaxLength = 128;
         this.textDisplayName.Name = "textDisplayName";
         this.textDisplayName.Size = new System.Drawing.Size(360, 20);
         this.textDisplayName.TabIndex = 11;
         this.textDisplayName.TabStop = false;
         this.textDisplayName.TextChanged += new System.EventHandler(this.textDisplayName_TextChanged);
         // 
         // label1
         // 
         this.label1.Location = new System.Drawing.Point(12, 40);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(80, 16);
         this.label1.TabIndex = 10;
         this.label1.Text = "&Display Name:";
         // 
         // textServer
         // 
         this.textServer.Location = new System.Drawing.Point(96, 8);
         this.textServer.Name = "textServer";
         this.textServer.ReadOnly = true;
         this.textServer.Size = new System.Drawing.Size(360, 20);
         this.textServer.TabIndex = 2;
         this.textServer.TabStop = false;
         // 
         // lblServer
         // 
         this.lblServer.Location = new System.Drawing.Point(12, 12);
         this.lblServer.Name = "lblServer";
         this.lblServer.Size = new System.Drawing.Size(72, 16);
         this.lblServer.TabIndex = 1;
         this.lblServer.Text = "&SQL Server:";
         // 
         // textDescription
         // 
         this.textDescription.Location = new System.Drawing.Point(96, 64);
         this.textDescription.MaxLength = 256;
         this.textDescription.Multiline = true;
         this.textDescription.Name = "textDescription";
         this.textDescription.Size = new System.Drawing.Size(360, 76);
         this.textDescription.TabIndex = 6;
         // 
         // lblDescription
         // 
         this.lblDescription.Location = new System.Drawing.Point(12, 68);
         this.lblDescription.Name = "lblDescription";
         this.lblDescription.Size = new System.Drawing.Size(68, 16);
         this.lblDescription.TabIndex = 5;
         this.lblDescription.Text = "D&escription:";
         // 
         // tabPagePermissions
         // 
         this.tabPagePermissions.Controls.Add(this.label16);
         this.tabPagePermissions.Controls.Add(this.groupBox3);
         this.tabPagePermissions.Location = new System.Drawing.Point(4, 22);
         this.tabPagePermissions.Name = "tabPagePermissions";
         this.tabPagePermissions.Size = new System.Drawing.Size(468, 330);
         this.tabPagePermissions.TabIndex = 2;
         this.tabPagePermissions.Text = "Default Permissions";
         this.tabPagePermissions.UseVisualStyleBackColor = true;
         // 
         // label16
         // 
         this.label16.Location = new System.Drawing.Point(8, 12);
         this.label16.Name = "label16";
         this.label16.Size = new System.Drawing.Size(448, 16);
         this.label16.TabIndex = 34;
         this.label16.Text = "Select the default level of access you want to grant users.";
         // 
         // groupBox3
         // 
         this.groupBox3.Controls.Add(this.radioGrantAll);
         this.groupBox3.Controls.Add(this.radioGrantEventsOnly);
         this.groupBox3.Controls.Add(this.radioDeny);
         this.groupBox3.Location = new System.Drawing.Point(8, 32);
         this.groupBox3.Name = "groupBox3";
         this.groupBox3.Size = new System.Drawing.Size(452, 120);
         this.groupBox3.TabIndex = 33;
         this.groupBox3.TabStop = false;
         this.groupBox3.Text = "Default Database Permissions";
         // 
         // radioGrantAll
         // 
         this.radioGrantAll.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
         this.radioGrantAll.Checked = true;
         this.radioGrantAll.Location = new System.Drawing.Point(8, 20);
         this.radioGrantAll.Name = "radioGrantAll";
         this.radioGrantAll.Size = new System.Drawing.Size(344, 16);
         this.radioGrantAll.TabIndex = 34;
         this.radioGrantAll.TabStop = true;
         this.radioGrantAll.Text = "Grant right to read events and their associated SQL statements .";
         // 
         // radioGrantEventsOnly
         // 
         this.radioGrantEventsOnly.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
         this.radioGrantEventsOnly.Location = new System.Drawing.Point(8, 44);
         this.radioGrantEventsOnly.Name = "radioGrantEventsOnly";
         this.radioGrantEventsOnly.Size = new System.Drawing.Size(428, 32);
         this.radioGrantEventsOnly.TabIndex = 32;
         this.radioGrantEventsOnly.Text = "Grant right to read events only - To allow users to view the associated SQL state" +
             "ments, you will need to explicictly grant users read access to the database.";
         // 
         // radioDeny
         // 
         this.radioDeny.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
         this.radioDeny.Location = new System.Drawing.Point(8, 80);
         this.radioDeny.Name = "radioDeny";
         this.radioDeny.Size = new System.Drawing.Size(424, 30);
         this.radioDeny.TabIndex = 9;
         this.radioDeny.Text = "Deny read access by default - To allow users to view events and the associated SQ" +
             "L, you will need to explicitly grant users read access to the database.";
         this.radioDeny.TextAlign = System.Drawing.ContentAlignment.TopLeft;
         // 
         // Form_ArchiveProperties
         // 
         this.AcceptButton = this.btnOK;
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.CancelButton = this.btnCancel;
         this.ClientSize = new System.Drawing.Size(474, 396);
         this.Controls.Add(this.tabControl1);
         this.Controls.Add(this.btnCancel);
         this.Controls.Add(this.btnOK);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.HelpButton = true;
         this.KeyPreview = true;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "Form_ArchiveProperties";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Archive Properties";
         this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.Form_ArchiveProperties_HelpRequested);
         this.tabControl1.ResumeLayout(false);
         this.tabPageGeneral.ResumeLayout(false);
         this.tabPageGeneral.PerformLayout();
         this.groupBox1.ResumeLayout(false);
         this.groupBox1.PerformLayout();
         this.tabPagePermissions.ResumeLayout(false);
         this.groupBox3.ResumeLayout(false);
         this.ResumeLayout(false);

      }
      #endregion

      private System.Windows.Forms.Button btnCancel;
      private System.Windows.Forms.Button btnOK;
      private System.Windows.Forms.TabControl tabControl1;
      private System.Windows.Forms.TabPage tabPageGeneral;
      private System.Windows.Forms.TabPage tabPagePermissions;
      private System.Windows.Forms.Label lblServer;
      private System.Windows.Forms.Label lblDescription;
      private System.Windows.Forms.TextBox textServer;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.TextBox textDisplayName;
      private System.Windows.Forms.TextBox textDescription;
      private System.Windows.Forms.Label label16;
      private System.Windows.Forms.GroupBox groupBox3;
      private System.Windows.Forms.RadioButton radioDeny;
      private System.Windows.Forms.RadioButton radioGrantAll;
      private System.Windows.Forms.RadioButton radioGrantEventsOnly;
      private System.Windows.Forms.GroupBox groupBox1;
      private System.Windows.Forms.TextBox textIntegrityStatus;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.TextBox textDatabase;
      private System.Windows.Forms.TextBox textLastIntegrityCheck;
      private System.Windows.Forms.Label label4;
      private System.Windows.Forms.TextBox textLastIntegrityCheckResults;
      private System.Windows.Forms.Label label5;
      private System.Windows.Forms.TextBox textEndDate;
      private System.Windows.Forms.TextBox textStartDate;
      private System.Windows.Forms.Label lblTimeCreated;
      private System.Windows.Forms.Label lblStatus;
	}
}