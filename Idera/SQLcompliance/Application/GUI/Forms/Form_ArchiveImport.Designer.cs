namespace Idera.SQLcompliance.Application.GUI.Forms
{
   partial class Form_ArchiveImport
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
         this.btnOK = new System.Windows.Forms.Button();
         this.btnCancel = new System.Windows.Forms.Button();
         this.comboDatabase = new System.Windows.Forms.ComboBox();
         this.groupBox1 = new System.Windows.Forms.GroupBox();
         this.textDisplayName = new System.Windows.Forms.TextBox();
         this.label2 = new System.Windows.Forms.Label();
         this.textEndDate = new System.Windows.Forms.TextBox();
         this.textStartDate = new System.Windows.Forms.TextBox();
         this.lblTimeCreated = new System.Windows.Forms.Label();
         this.lblStatus = new System.Windows.Forms.Label();
         this.textServer = new System.Windows.Forms.TextBox();
         this.lblServer = new System.Windows.Forms.Label();
         this.textDescription = new System.Windows.Forms.TextBox();
         this.lblDescription = new System.Windows.Forms.Label();
         this.label1 = new System.Windows.Forms.Label();
         this.checkShowAllDatabases = new System.Windows.Forms.CheckBox();
         this.label3 = new System.Windows.Forms.Label();
         this.groupBox1.SuspendLayout();
         this.SuspendLayout();
         // 
         // btnOK
         // 
         this.btnOK.Enabled = false;
         this.btnOK.Location = new System.Drawing.Point(356, 268);
         this.btnOK.Name = "btnOK";
         this.btnOK.Size = new System.Drawing.Size(75, 23);
         this.btnOK.TabIndex = 14;
         this.btnOK.Text = "&OK";
         this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
         // 
         // btnCancel
         // 
         this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.btnCancel.Location = new System.Drawing.Point(440, 268);
         this.btnCancel.Name = "btnCancel";
         this.btnCancel.Size = new System.Drawing.Size(75, 23);
         this.btnCancel.TabIndex = 15;
         this.btnCancel.Text = "&Cancel";
         this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
         // 
         // comboDatabase
         // 
         this.comboDatabase.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.comboDatabase.Location = new System.Drawing.Point(116, 36);
         this.comboDatabase.Name = "comboDatabase";
         this.comboDatabase.Size = new System.Drawing.Size(400, 21);
         this.comboDatabase.TabIndex = 1;
         this.comboDatabase.SelectedIndexChanged += new System.EventHandler(this.comboDatabase_SelectedIndexChanged);
         this.comboDatabase.DropDown += new System.EventHandler(this.comboDatabase_DropDown);
         // 
         // groupBox1
         // 
         this.groupBox1.Controls.Add(this.textDisplayName);
         this.groupBox1.Controls.Add(this.label2);
         this.groupBox1.Controls.Add(this.textEndDate);
         this.groupBox1.Controls.Add(this.textStartDate);
         this.groupBox1.Controls.Add(this.lblTimeCreated);
         this.groupBox1.Controls.Add(this.lblStatus);
         this.groupBox1.Controls.Add(this.textServer);
         this.groupBox1.Controls.Add(this.lblServer);
         this.groupBox1.Controls.Add(this.textDescription);
         this.groupBox1.Controls.Add(this.lblDescription);
         this.groupBox1.Location = new System.Drawing.Point(12, 100);
         this.groupBox1.Name = "groupBox1";
         this.groupBox1.Size = new System.Drawing.Size(504, 160);
         this.groupBox1.TabIndex = 3;
         this.groupBox1.TabStop = false;
         this.groupBox1.Text = "Archive Information";
         // 
         // textDisplayName
         // 
         this.textDisplayName.Location = new System.Drawing.Point(108, 20);
         this.textDisplayName.Name = "textDisplayName";
         this.textDisplayName.ReadOnly = true;
         this.textDisplayName.Size = new System.Drawing.Size(384, 20);
         this.textDisplayName.TabIndex = 5;
         this.textDisplayName.TabStop = false;
         // 
         // label2
         // 
         this.label2.Location = new System.Drawing.Point(12, 24);
         this.label2.Name = "label2";
         this.label2.Size = new System.Drawing.Size(80, 16);
         this.label2.TabIndex = 4;
         this.label2.Text = "&Display Name:";
         // 
         // textEndDate
         // 
         this.textEndDate.Location = new System.Drawing.Point(260, 128);
         this.textEndDate.Name = "textEndDate";
         this.textEndDate.ReadOnly = true;
         this.textEndDate.Size = new System.Drawing.Size(120, 20);
         this.textEndDate.TabIndex = 13;
         this.textEndDate.TabStop = false;
         // 
         // textStartDate
         // 
         this.textStartDate.Location = new System.Drawing.Point(108, 128);
         this.textStartDate.Name = "textStartDate";
         this.textStartDate.ReadOnly = true;
         this.textStartDate.Size = new System.Drawing.Size(120, 20);
         this.textStartDate.TabIndex = 11;
         this.textStartDate.TabStop = false;
         // 
         // lblTimeCreated
         // 
         this.lblTimeCreated.Location = new System.Drawing.Point(236, 132);
         this.lblTimeCreated.Name = "lblTimeCreated";
         this.lblTimeCreated.Size = new System.Drawing.Size(16, 16);
         this.lblTimeCreated.TabIndex = 12;
         this.lblTimeCreated.Text = "to";
         // 
         // lblStatus
         // 
         this.lblStatus.Location = new System.Drawing.Point(12, 132);
         this.lblStatus.Name = "lblStatus";
         this.lblStatus.Size = new System.Drawing.Size(96, 20);
         this.lblStatus.TabIndex = 10;
         this.lblStatus.Text = "&Event Time Span:";
         // 
         // textServer
         // 
         this.textServer.Location = new System.Drawing.Point(108, 44);
         this.textServer.Name = "textServer";
         this.textServer.ReadOnly = true;
         this.textServer.Size = new System.Drawing.Size(384, 20);
         this.textServer.TabIndex = 7;
         this.textServer.TabStop = false;
         // 
         // lblServer
         // 
         this.lblServer.Location = new System.Drawing.Point(12, 48);
         this.lblServer.Name = "lblServer";
         this.lblServer.Size = new System.Drawing.Size(72, 16);
         this.lblServer.TabIndex = 6;
         this.lblServer.Text = "S&QL Server:";
         // 
         // textDescription
         // 
         this.textDescription.Location = new System.Drawing.Point(108, 68);
         this.textDescription.MaxLength = 255;
         this.textDescription.Multiline = true;
         this.textDescription.Name = "textDescription";
         this.textDescription.ReadOnly = true;
         this.textDescription.Size = new System.Drawing.Size(384, 48);
         this.textDescription.TabIndex = 9;
         this.textDescription.TabStop = false;
         // 
         // lblDescription
         // 
         this.lblDescription.Location = new System.Drawing.Point(12, 72);
         this.lblDescription.Name = "lblDescription";
         this.lblDescription.Size = new System.Drawing.Size(68, 16);
         this.lblDescription.TabIndex = 8;
         this.lblDescription.Text = "&Description:";
         // 
         // label1
         // 
         this.label1.Location = new System.Drawing.Point(12, 40);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(100, 16);
         this.label1.TabIndex = 0;
         this.label1.Text = "&Archive Database:";
         // 
         // checkShowAllDatabases
         // 
         this.checkShowAllDatabases.Location = new System.Drawing.Point(116, 64);
         this.checkShowAllDatabases.Name = "checkShowAllDatabases";
         this.checkShowAllDatabases.Size = new System.Drawing.Size(132, 16);
         this.checkShowAllDatabases.TabIndex = 2;
         this.checkShowAllDatabases.Text = "&Show all databases";
         this.checkShowAllDatabases.CheckedChanged += new System.EventHandler(this.checkShowAllDatabases_CheckedChanged);
         // 
         // label3
         // 
         this.label3.Location = new System.Drawing.Point(12, 12);
         this.label3.Name = "label3";
         this.label3.Size = new System.Drawing.Size(492, 20);
         this.label3.TabIndex = 16;
         this.label3.Text = "Select an existing archive database to attach to SQL Compliance Manager.";
         // 
         // Form_ArchiveImport
         // 
         this.AcceptButton = this.btnOK;
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.CancelButton = this.btnCancel;
         this.ClientSize = new System.Drawing.Size(526, 300);
         this.Controls.Add(this.label3);
         this.Controls.Add(this.checkShowAllDatabases);
         this.Controls.Add(this.label1);
         this.Controls.Add(this.groupBox1);
         this.Controls.Add(this.comboDatabase);
         this.Controls.Add(this.btnCancel);
         this.Controls.Add(this.btnOK);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.HelpButton = true;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "Form_ArchiveImport";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Attach Archive Database";
         this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.Form_Defaults_HelpRequested);
         this.groupBox1.ResumeLayout(false);
         this.groupBox1.PerformLayout();
         this.ResumeLayout(false);

      }
      #endregion

      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.Button btnOK;
      private System.Windows.Forms.Button btnCancel;
      private System.Windows.Forms.GroupBox groupBox1;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.TextBox textServer;
      private System.Windows.Forms.Label lblServer;
      private System.Windows.Forms.Label lblDescription;
      private System.Windows.Forms.TextBox textEndDate;
      private System.Windows.Forms.TextBox textStartDate;
      private System.Windows.Forms.Label lblTimeCreated;
      private System.Windows.Forms.Label lblStatus;
      private System.Windows.Forms.TextBox textDescription;
      private System.Windows.Forms.ComboBox comboDatabase;
      private System.Windows.Forms.CheckBox checkShowAllDatabases;
      private System.Windows.Forms.TextBox textDisplayName;
      private System.Windows.Forms.Label label3;
	}
}