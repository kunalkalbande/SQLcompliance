namespace Idera.SQLcompliance.Application.GUI.Forms
{
   partial class Form_Connect
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
         this.label1 = new System.Windows.Forms.Label();
         this.txtServer = new System.Windows.Forms.TextBox();
         this.button1 = new System.Windows.Forms.Button();
         this.SuspendLayout();
         // 
         // btnOK
         // 
         this.btnOK.Enabled = false;
         this.btnOK.Location = new System.Drawing.Point(272, 52);
         this.btnOK.Name = "btnOK";
         this.btnOK.Size = new System.Drawing.Size(75, 23);
         this.btnOK.TabIndex = 3;
         this.btnOK.Text = "&OK";
         this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
         // 
         // btnCancel
         // 
         this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.btnCancel.Location = new System.Drawing.Point(356, 52);
         this.btnCancel.Name = "btnCancel";
         this.btnCancel.Size = new System.Drawing.Size(75, 23);
         this.btnCancel.TabIndex = 4;
         this.btnCancel.Text = "&Cancel";
         this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
         // 
         // label1
         // 
         this.label1.Location = new System.Drawing.Point(8, 12);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(132, 16);
         this.label1.TabIndex = 0;
         this.label1.Text = "&Repository SQL Server:";
         // 
         // txtServer
         // 
         this.txtServer.Location = new System.Drawing.Point(136, 8);
         this.txtServer.Name = "txtServer";
         this.txtServer.Size = new System.Drawing.Size(268, 20);
         this.txtServer.TabIndex = 1;
         this.txtServer.TextChanged += new System.EventHandler(this.txtServer_TextChanged);
         // 
         // button1
         // 
         this.button1.Location = new System.Drawing.Point(408, 8);
         this.button1.Name = "button1";
         this.button1.Size = new System.Drawing.Size(24, 20);
         this.button1.TabIndex = 2;
         this.button1.Text = "...";
         this.button1.Click += new System.EventHandler(this.button1_Click);
         // 
         // Form_Connect
         // 
         this.AcceptButton = this.btnOK;
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.CancelButton = this.btnCancel;
         this.ClientSize = new System.Drawing.Size(442, 84);
         this.Controls.Add(this.button1);
         this.Controls.Add(this.txtServer);
         this.Controls.Add(this.label1);
         this.Controls.Add(this.btnCancel);
         this.Controls.Add(this.btnOK);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.HelpButton = true;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "Form_Connect";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Connect to Repository";
         this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.Form_Connect_HelpRequested);
         this.ResumeLayout(false);
         this.PerformLayout();

      }
      #endregion

      private System.Windows.Forms.Button btnOK;
      private System.Windows.Forms.Button btnCancel;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.TextBox txtServer;
      private System.Windows.Forms.Button button1;

	}
}