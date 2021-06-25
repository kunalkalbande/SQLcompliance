namespace Idera.SQLcompliance.Application.GUI.Forms
{
   partial class Form_LoginFilterOptions
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_LoginFilterOptions));
         this.btnOK = new System.Windows.Forms.Button();
         this.btnCancel = new System.Windows.Forms.Button();
         this.label1 = new System.Windows.Forms.Label();
         this.checkFilterLogins = new System.Windows.Forms.CheckBox();
         this.comboTime = new System.Windows.Forms.ComboBox();
         this.SuspendLayout();
         // 
         // btnOK
         // 
         this.btnOK.Location = new System.Drawing.Point(232, 104);
         this.btnOK.Name = "btnOK";
         this.btnOK.Size = new System.Drawing.Size(75, 23);
         this.btnOK.TabIndex = 4;
         this.btnOK.Text = "&OK";
         this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
         // 
         // btnCancel
         // 
         this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.btnCancel.Location = new System.Drawing.Point(316, 104);
         this.btnCancel.Name = "btnCancel";
         this.btnCancel.Size = new System.Drawing.Size(75, 23);
         this.btnCancel.TabIndex = 5;
         this.btnCancel.Text = "&Cancel";
         this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
         // 
         // label1
         // 
         this.label1.Location = new System.Drawing.Point(8, 8);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(392, 40);
         this.label1.TabIndex = 13;
         this.label1.Text = resources.GetString("label1.Text");
         // 
         // checkFilterLogins
         // 
         this.checkFilterLogins.Checked = true;
         this.checkFilterLogins.CheckState = System.Windows.Forms.CheckState.Checked;
         this.checkFilterLogins.Location = new System.Drawing.Point(20, 56);
         this.checkFilterLogins.Name = "checkFilterLogins";
         this.checkFilterLogins.Size = new System.Drawing.Size(296, 16);
         this.checkFilterLogins.TabIndex = 15;
         this.checkFilterLogins.Text = "&Filter duplicate logins occuring within a time period of ";
         this.checkFilterLogins.CheckedChanged += new System.EventHandler(this.checkFilterLogins_CheckedChanged);
         // 
         // comboTime
         // 
         this.comboTime.Items.AddRange(new object[] {
            "15 minutes",
            "30 minutes",
            "1 hour",
            "2 hours",
            "4 hours",
            "8 hours",
            "12 hours",
            "1 day"});
         this.comboTime.Location = new System.Drawing.Point(308, 54);
         this.comboTime.Name = "comboTime";
         this.comboTime.Size = new System.Drawing.Size(84, 21);
         this.comboTime.TabIndex = 18;
         this.comboTime.Text = "1 hour";
         // 
         // Form_LoginFilterOptions
         // 
         this.AcceptButton = this.btnOK;
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.CancelButton = this.btnCancel;
         this.ClientSize = new System.Drawing.Size(402, 136);
         this.Controls.Add(this.comboTime);
         this.Controls.Add(this.checkFilterLogins);
         this.Controls.Add(this.label1);
         this.Controls.Add(this.btnCancel);
         this.Controls.Add(this.btnOK);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.HelpButton = true;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "Form_LoginFilterOptions";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Login Filtering Options";
         this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.Form_Defaults_HelpRequested);
         this.ResumeLayout(false);

      }
      #endregion

      private System.Windows.Forms.Button btnOK;
      private System.Windows.Forms.Button btnCancel;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.CheckBox checkFilterLogins;
      private System.Windows.Forms.ComboBox comboTime;
	}
}