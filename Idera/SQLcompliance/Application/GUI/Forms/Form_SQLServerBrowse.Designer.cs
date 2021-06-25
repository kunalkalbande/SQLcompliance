namespace Idera.SQLcompliance.Application.GUI.Forms
{
   partial class Form_SQLServerBrowse
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
          this.label1 = new System.Windows.Forms.Label();
          this.listBoxServers = new System.Windows.Forms.ListBox();
          this.btnOK = new System.Windows.Forms.Button();
          this.btnCancel = new System.Windows.Forms.Button();
          this.pnlLoading = new System.Windows.Forms.Panel();
          this.lblLoading = new System.Windows.Forms.Label();
          this.progLoading = new System.Windows.Forms.ProgressBar();
          this.pnlLoading.SuspendLayout();
          this.SuspendLayout();
          // 
          // label1
          // 
          this.label1.Location = new System.Drawing.Point(12, 8);
          this.label1.Name = "label1";
          this.label1.Size = new System.Drawing.Size(156, 16);
          this.label1.TabIndex = 0;
          this.label1.Text = "&Select SQL Server:";
          // 
          // listBoxServers
          // 
          this.listBoxServers.BackColor = System.Drawing.Color.White;
          this.listBoxServers.Location = new System.Drawing.Point(12, 24);
          this.listBoxServers.Name = "listBoxServers";
          this.listBoxServers.Size = new System.Drawing.Size(292, 212);
          this.listBoxServers.Sorted = true;
          this.listBoxServers.TabIndex = 1;
          this.listBoxServers.SelectedIndexChanged += new System.EventHandler(this.listBoxServers_SelectedIndexChanged);
          // 
          // btnOK
          // 
          this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
          this.btnOK.Enabled = false;
          this.btnOK.Location = new System.Drawing.Point(312, 24);
          this.btnOK.Name = "btnOK";
          this.btnOK.Size = new System.Drawing.Size(75, 23);
          this.btnOK.TabIndex = 2;
          this.btnOK.Text = "&OK";
          this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
          // 
          // btnCancel
          // 
          this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
          this.btnCancel.Location = new System.Drawing.Point(312, 56);
          this.btnCancel.Name = "btnCancel";
          this.btnCancel.Size = new System.Drawing.Size(75, 23);
          this.btnCancel.TabIndex = 3;
          this.btnCancel.Text = "&Cancel";
          this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
          // 
          // pnlLoading
          // 
          this.pnlLoading.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
          this.pnlLoading.Controls.Add(this.lblLoading);
          this.pnlLoading.Controls.Add(this.progLoading);
          this.pnlLoading.Location = new System.Drawing.Point(104, 112);
          this.pnlLoading.Name = "pnlLoading";
          this.pnlLoading.Size = new System.Drawing.Size(108, 37);
          this.pnlLoading.TabIndex = 6;
          // 
          // lblLoading
          // 
          this.lblLoading.AutoSize = true;
          this.lblLoading.BackColor = System.Drawing.SystemColors.Control;
          this.lblLoading.Location = new System.Drawing.Point(3, 0);
          this.lblLoading.Name = "lblLoading";
          this.lblLoading.Size = new System.Drawing.Size(54, 13);
          this.lblLoading.TabIndex = 7;
          this.lblLoading.Text = "Loading...";
          // 
          // progLoading
          // 
          this.progLoading.Location = new System.Drawing.Point(3, 16);
          this.progLoading.Name = "progLoading";
          this.progLoading.Size = new System.Drawing.Size(100, 13);
          this.progLoading.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
          this.progLoading.TabIndex = 6;
          // 
          // Form_SQLServerBrowse
          // 
          this.AcceptButton = this.btnOK;
          this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
          this.CancelButton = this.btnCancel;
          this.ClientSize = new System.Drawing.Size(397, 252);
          this.Controls.Add(this.pnlLoading);
          this.Controls.Add(this.btnCancel);
          this.Controls.Add(this.btnOK);
          this.Controls.Add(this.label1);
          this.Controls.Add(this.listBoxServers);
          this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
          this.HelpButton = true;
          this.MaximizeBox = false;
          this.MinimizeBox = false;
          this.Name = "Form_SQLServerBrowse";
          this.ShowInTaskbar = false;
          this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
          this.Text = "Select SQL Server";
          this.Shown += new System.EventHandler(this.Form_SQLServerBrowse_Shown);
          this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.Form_SQLServerBrowse_HelpRequested);
          this.pnlLoading.ResumeLayout(false);
          this.pnlLoading.PerformLayout();
          this.ResumeLayout(false);

      }
      #endregion

      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.ListBox listBoxServers;
      private System.Windows.Forms.Button btnOK;
       private System.Windows.Forms.Button btnCancel;
       private System.Windows.Forms.Panel pnlLoading;
       private System.Windows.Forms.Label lblLoading;
       private System.Windows.Forms.ProgressBar progLoading;
   }
}