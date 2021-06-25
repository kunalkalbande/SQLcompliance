namespace Idera.SQLcompliance.Application.GUI.Forms
{
   partial class Form_IntegrityCheck
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
         this.label4 = new System.Windows.Forms.Label();
         this.groupBox1 = new System.Windows.Forms.GroupBox();
         this.listDatabases = new System.Windows.Forms.ListView();
         this.chServer = new System.Windows.Forms.ColumnHeader();
         this.chType = new System.Windows.Forms.ColumnHeader();
         this.chDatabase = new System.Windows.Forms.ColumnHeader();
         this.checkShowArchives = new System.Windows.Forms.CheckBox();
         this.groupBox1.SuspendLayout();
         this.SuspendLayout();
         // 
         // btnOK
         // 
         this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.btnOK.Enabled = false;
         this.btnOK.Location = new System.Drawing.Point(336, 254);
         this.btnOK.Name = "btnOK";
         this.btnOK.Size = new System.Drawing.Size(75, 23);
         this.btnOK.TabIndex = 3;
         this.btnOK.Text = "&Check";
         this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
         // 
         // btnCancel
         // 
         this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.btnCancel.Location = new System.Drawing.Point(424, 254);
         this.btnCancel.Name = "btnCancel";
         this.btnCancel.Size = new System.Drawing.Size(75, 23);
         this.btnCancel.TabIndex = 4;
         this.btnCancel.Text = "&Close";
         this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
         // 
         // label4
         // 
         this.label4.Location = new System.Drawing.Point(8, 8);
         this.label4.Name = "label4";
         this.label4.Size = new System.Drawing.Size(488, 28);
         this.label4.TabIndex = 5;
         this.label4.Text = "This option checks for unexpected changes in your audit data, detecting when even" +
             "ts have been modified, added, or deleted by a script or an application other tha" +
             "n SQL Compliance Manager.";
         // 
         // groupBox1
         // 
         this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                     | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.groupBox1.Controls.Add(this.listDatabases);
         this.groupBox1.Location = new System.Drawing.Point(12, 44);
         this.groupBox1.Name = "groupBox1";
         this.groupBox1.Size = new System.Drawing.Size(478, 204);
         this.groupBox1.TabIndex = 1;
         this.groupBox1.TabStop = false;
         this.groupBox1.Text = "Select a database to check:";
         // 
         // listDatabases
         // 
         this.listDatabases.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                     | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.listDatabases.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chServer,
            this.chType,
            this.chDatabase});
         this.listDatabases.FullRowSelect = true;
         this.listDatabases.HideSelection = false;
         this.listDatabases.Location = new System.Drawing.Point(12, 16);
         this.listDatabases.MultiSelect = false;
         this.listDatabases.Name = "listDatabases";
         this.listDatabases.Size = new System.Drawing.Size(458, 180);
         this.listDatabases.TabIndex = 2;
         this.listDatabases.UseCompatibleStateImageBehavior = false;
         this.listDatabases.View = System.Windows.Forms.View.Details;
         this.listDatabases.SelectedIndexChanged += new System.EventHandler(this.listDatabases_SelectedIndexChanged);
         // 
         // chServer
         // 
         this.chServer.Text = "SQL Server";
         this.chServer.Width = 125;
         // 
         // chType
         // 
         this.chType.Text = "Type";
         this.chType.Width = 67;
         // 
         // chDatabase
         // 
         this.chDatabase.Text = "Database";
         this.chDatabase.Width = 242;
         // 
         // checkShowArchives
         // 
         this.checkShowArchives.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.checkShowArchives.Location = new System.Drawing.Point(20, 256);
         this.checkShowArchives.Name = "checkShowArchives";
         this.checkShowArchives.Size = new System.Drawing.Size(156, 20);
         this.checkShowArchives.TabIndex = 6;
         this.checkShowArchives.Text = "&Show archive databases";
         this.checkShowArchives.CheckedChanged += new System.EventHandler(this.checkShowArchives_CheckedChanged);
         // 
         // Form_IntegrityCheck
         // 
         this.AcceptButton = this.btnOK;
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.CancelButton = this.btnCancel;
         this.ClientSize = new System.Drawing.Size(504, 284);
         this.Controls.Add(this.checkShowArchives);
         this.Controls.Add(this.groupBox1);
         this.Controls.Add(this.label4);
         this.Controls.Add(this.btnCancel);
         this.Controls.Add(this.btnOK);
         this.HelpButton = true;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.MinimumSize = new System.Drawing.Size(512, 318);
         this.Name = "Form_IntegrityCheck";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
         this.Text = "Check Repository Integrity";
         this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.Form_Defaults_HelpRequested);
         this.groupBox1.ResumeLayout(false);
         this.ResumeLayout(false);

      }
      #endregion

      private System.Windows.Forms.Button btnOK;
      private System.Windows.Forms.Button btnCancel;
      private System.Windows.Forms.Label label4;
      private System.Windows.Forms.GroupBox groupBox1;
      private System.Windows.Forms.ListView listDatabases;
      private System.Windows.Forms.ColumnHeader chServer;
      private System.Windows.Forms.ColumnHeader chDatabase;
      private System.Windows.Forms.ColumnHeader chType;
      private System.Windows.Forms.CheckBox checkShowArchives;
   }
}