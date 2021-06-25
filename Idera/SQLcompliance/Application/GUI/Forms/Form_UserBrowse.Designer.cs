namespace Idera.SQLcompliance.Application.GUI.Forms
{
   partial class Form_UserBrowse
   {
      /// <summary>
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

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
         this.label1 = new System.Windows.Forms.Label();
         this.comboDomains = new System.Windows.Forms.ComboBox();
         this.groupBox1 = new System.Windows.Forms.GroupBox();
         this.listUsers = new System.Windows.Forms.ListView();
         this.chName = new System.Windows.Forms.ColumnHeader();
         this.groupBox1.SuspendLayout();
         this.SuspendLayout();
         // 
         // btnOK
         // 
         this.btnOK.Location = new System.Drawing.Point(312, 328);
         this.btnOK.Name = "btnOK";
         this.btnOK.Size = new System.Drawing.Size(75, 23);
         this.btnOK.TabIndex = 0;
         this.btnOK.Text = "&OK";
         this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
         // 
         // btnCancel
         // 
         this.btnCancel.Location = new System.Drawing.Point(392, 328);
         this.btnCancel.Name = "btnCancel";
         this.btnCancel.Size = new System.Drawing.Size(75, 23);
         this.btnCancel.TabIndex = 1;
         this.btnCancel.Text = "&Cancel";
         this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
         // 
         // label1
         // 
         this.label1.Location = new System.Drawing.Point(8, 12);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(52, 20);
         this.label1.TabIndex = 2;
         this.label1.Text = "&Domain:";
         // 
         // comboDomains
         // 
         this.comboDomains.Location = new System.Drawing.Point(60, 8);
         this.comboDomains.Name = "comboDomains";
         this.comboDomains.Size = new System.Drawing.Size(408, 21);
         this.comboDomains.TabIndex = 3;
         this.comboDomains.Text = "comboBox1";
         this.comboDomains.SelectedIndexChanged += new System.EventHandler(this.comboDomains_SelectedIndexChanged);
         // 
         // groupBox1
         // 
         this.groupBox1.Controls.Add(this.listUsers);
         this.groupBox1.Location = new System.Drawing.Point(8, 36);
         this.groupBox1.Name = "groupBox1";
         this.groupBox1.Size = new System.Drawing.Size(460, 284);
         this.groupBox1.TabIndex = 4;
         this.groupBox1.TabStop = false;
         this.groupBox1.Text = "Users";
         // 
         // listUsers
         // 
         this.listUsers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chName});
         this.listUsers.Location = new System.Drawing.Point(12, 20);
         this.listUsers.Name = "listUsers";
         this.listUsers.Size = new System.Drawing.Size(440, 256);
         this.listUsers.TabIndex = 0;
         this.listUsers.UseCompatibleStateImageBehavior = false;
         this.listUsers.View = System.Windows.Forms.View.Details;
         this.listUsers.SelectedIndexChanged += new System.EventHandler(this.listUsers_SelectedIndexChanged);
         // 
         // chName
         // 
         this.chName.Text = "Name";
         this.chName.Width = 415;
         // 
         // Form_UserBrowse
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(476, 358);
         this.Controls.Add(this.groupBox1);
         this.Controls.Add(this.comboDomains);
         this.Controls.Add(this.label1);
         this.Controls.Add(this.btnCancel);
         this.Controls.Add(this.btnOK);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
         this.HelpButton = true;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "Form_UserBrowse";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Form_UserBrowse";
         this.groupBox1.ResumeLayout(false);
         this.ResumeLayout(false);

      }
      #endregion

      private System.Windows.Forms.Button btnOK;
      private System.Windows.Forms.Button btnCancel;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.ComboBox comboDomains;
      private System.Windows.Forms.GroupBox groupBox1;
      private System.Windows.Forms.ListView listUsers;
      private System.Windows.Forms.ColumnHeader chName;
   }
}