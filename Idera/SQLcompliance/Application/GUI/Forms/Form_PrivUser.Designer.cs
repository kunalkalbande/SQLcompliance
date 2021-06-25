namespace Idera.SQLcompliance.Application.GUI.Forms
{
   partial class Form_PrivUser
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
         this.listAvailable = new System.Windows.Forms.ListView();
         this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
         this.btnAdd = new System.Windows.Forms.Button();
         this.comboDatabases = new System.Windows.Forms.ComboBox();
         this.label1 = new System.Windows.Forms.Label();
         this.label2 = new System.Windows.Forms.Label();
         this.btnRemove = new System.Windows.Forms.Button();
         this.listSelected = new System.Windows.Forms.ListView();
         this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
         this.label3 = new System.Windows.Forms.Label();
         this.groupBox1 = new System.Windows.Forms.GroupBox();
         this.groupBox2 = new System.Windows.Forms.GroupBox();
         this.label4 = new System.Windows.Forms.Label();
         this.SuspendLayout();
         // 
         // btnOK
         // 
         this.btnOK.Location = new System.Drawing.Point(320, 368);
         this.btnOK.Name = "btnOK";
         this.btnOK.Size = new System.Drawing.Size(70, 23);
         this.btnOK.TabIndex = 9;
         this.btnOK.Text = "&OK";
         this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
         // 
         // btnCancel
         // 
         this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.btnCancel.Location = new System.Drawing.Point(396, 368);
         this.btnCancel.Name = "btnCancel";
         this.btnCancel.Size = new System.Drawing.Size(70, 23);
         this.btnCancel.TabIndex = 10;
         this.btnCancel.Text = "&Cancel";
         this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
         // 
         // listAvailable
         // 
         this.listAvailable.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
         this.listAvailable.FullRowSelect = true;
         this.listAvailable.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
         this.listAvailable.HideSelection = false;
         this.listAvailable.Location = new System.Drawing.Point(8, 64);
         this.listAvailable.Name = "listAvailable";
         this.listAvailable.Size = new System.Drawing.Size(380, 124);
         this.listAvailable.Sorting = System.Windows.Forms.SortOrder.Ascending;
         this.listAvailable.TabIndex = 1;
         this.listAvailable.UseCompatibleStateImageBehavior = false;
         this.listAvailable.View = System.Windows.Forms.View.Details;
         this.listAvailable.DoubleClick += new System.EventHandler(this.listAvailable_DoubleClick);
         this.listAvailable.SelectedIndexChanged += new System.EventHandler(this.listAvailable_SelectedIndexChanged);
         // 
         // columnHeader1
         // 
         this.columnHeader1.Width = 355;
         // 
         // btnAdd
         // 
         this.btnAdd.Enabled = false;
         this.btnAdd.Location = new System.Drawing.Point(396, 64);
         this.btnAdd.Name = "btnAdd";
         this.btnAdd.Size = new System.Drawing.Size(70, 23);
         this.btnAdd.TabIndex = 2;
         this.btnAdd.Text = "&Add";
         this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
         // 
         // comboDatabases
         // 
         this.comboDatabases.Location = new System.Drawing.Point(8, 20);
         this.comboDatabases.Name = "comboDatabases";
         this.comboDatabases.Size = new System.Drawing.Size(380, 21);
         this.comboDatabases.Sorted = true;
         this.comboDatabases.TabIndex = 11;
         this.comboDatabases.SelectedIndexChanged += new System.EventHandler(this.comboDatabases_SelectedIndexChanged);
         // 
         // label1
         // 
         this.label1.Location = new System.Drawing.Point(8, 4);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(156, 16);
         this.label1.TabIndex = 12;
         this.label1.Text = "Show Logins/Roles from:";
         // 
         // label2
         // 
         this.label2.Location = new System.Drawing.Point(8, 48);
         this.label2.Name = "label2";
         this.label2.Size = new System.Drawing.Size(124, 16);
         this.label2.TabIndex = 13;
         this.label2.Text = "Available Logins/Roles:";
         // 
         // btnRemove
         // 
         this.btnRemove.Enabled = false;
         this.btnRemove.Location = new System.Drawing.Point(396, 220);
         this.btnRemove.Name = "btnRemove";
         this.btnRemove.Size = new System.Drawing.Size(70, 23);
         this.btnRemove.TabIndex = 14;
         this.btnRemove.Text = "&Remove";
         this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
         // 
         // listSelected
         // 
         this.listSelected.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
         this.listSelected.FullRowSelect = true;
         this.listSelected.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
         this.listSelected.HideSelection = false;
         this.listSelected.Location = new System.Drawing.Point(8, 220);
         this.listSelected.Name = "listSelected";
         this.listSelected.Size = new System.Drawing.Size(380, 128);
         this.listSelected.Sorting = System.Windows.Forms.SortOrder.Ascending;
         this.listSelected.TabIndex = 15;
         this.listSelected.UseCompatibleStateImageBehavior = false;
         this.listSelected.View = System.Windows.Forms.View.Details;
         this.listSelected.SelectedIndexChanged += new System.EventHandler(this.listSelected_SelectedIndexChanged);
         // 
         // columnHeader2
         // 
         this.columnHeader2.Width = 355;
         // 
         // label3
         // 
         this.label3.Location = new System.Drawing.Point(8, 204);
         this.label3.Name = "label3";
         this.label3.Size = new System.Drawing.Size(184, 16);
         this.label3.TabIndex = 16;
         this.label3.Text = "Add to Privileged User List:";
         // 
         // groupBox1
         // 
         this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
         this.groupBox1.Location = new System.Drawing.Point(156, 208);
         this.groupBox1.Name = "groupBox1";
         this.groupBox1.Size = new System.Drawing.Size(312, 4);
         this.groupBox1.TabIndex = 17;
         this.groupBox1.TabStop = false;
         // 
         // groupBox2
         // 
         this.groupBox2.Location = new System.Drawing.Point(8, 356);
         this.groupBox2.Name = "groupBox2";
         this.groupBox2.Size = new System.Drawing.Size(460, 4);
         this.groupBox2.TabIndex = 18;
         this.groupBox2.TabStop = false;
         // 
         // label4
         // 
         this.label4.Location = new System.Drawing.Point(8, 366);
         this.label4.Name = "label4";
         this.label4.Size = new System.Drawing.Size(292, 28);
         this.label4.TabIndex = 19;
         this.label4.Text = "Note: Specifying large numbers of privileged users may have a performance impact " +
             "on the audited SQL Server.";
         // 
         // Form_PrivUser
         // 
         this.AcceptButton = this.btnOK;
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.CancelButton = this.btnCancel;
         this.ClientSize = new System.Drawing.Size(474, 400);
         this.Controls.Add(this.label4);
         this.Controls.Add(this.groupBox1);
         this.Controls.Add(this.label3);
         this.Controls.Add(this.btnRemove);
         this.Controls.Add(this.listSelected);
         this.Controls.Add(this.label2);
         this.Controls.Add(this.label1);
         this.Controls.Add(this.comboDatabases);
         this.Controls.Add(this.btnAdd);
         this.Controls.Add(this.listAvailable);
         this.Controls.Add(this.btnCancel);
         this.Controls.Add(this.btnOK);
         this.Controls.Add(this.groupBox2);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.HelpButton = true;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "Form_PrivUser";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Add Privileged Users";
         this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.Form_PrivUser_HelpRequested);
         this.Load += new System.EventHandler(this.Form_PrivUser_Load);
         this.ResumeLayout(false);

      }
      #endregion

      private System.Windows.Forms.Button btnOK;
      private System.Windows.Forms.Button btnCancel;
      private System.Windows.Forms.ListView listAvailable;
      private System.Windows.Forms.Button btnAdd;
      private System.Windows.Forms.ColumnHeader columnHeader1;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.Button btnRemove;
      public System.Windows.Forms.ListView listSelected;
      private System.Windows.Forms.ColumnHeader columnHeader2;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.GroupBox groupBox1;
      private System.Windows.Forms.GroupBox groupBox2;
      private System.Windows.Forms.ComboBox comboDatabases;
      private System.Windows.Forms.Label label4;
   }
}