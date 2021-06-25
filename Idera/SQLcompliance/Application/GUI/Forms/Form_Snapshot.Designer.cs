namespace Idera.SQLcompliance.Application.GUI.Forms
{
   partial class Form_Snapshot
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
         this.radioAll = new System.Windows.Forms.RadioButton();
         this.radioServer = new System.Windows.Forms.RadioButton();
         this.comboServer = new System.Windows.Forms.ComboBox();
         this.SuspendLayout();
         // 
         // btnOK
         // 
         this.btnOK.Location = new System.Drawing.Point(304, 84);
         this.btnOK.Name = "btnOK";
         this.btnOK.Size = new System.Drawing.Size(75, 23);
         this.btnOK.TabIndex = 0;
         this.btnOK.Text = "&OK";
         this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
         // 
         // btnCancel
         // 
         this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.btnCancel.Location = new System.Drawing.Point(388, 84);
         this.btnCancel.Name = "btnCancel";
         this.btnCancel.Size = new System.Drawing.Size(75, 23);
         this.btnCancel.TabIndex = 1;
         this.btnCancel.Text = "&Cancel";
         this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
         // 
         // radioAll
         // 
         this.radioAll.Checked = true;
         this.radioAll.Location = new System.Drawing.Point(12, 8);
         this.radioAll.Name = "radioAll";
         this.radioAll.Size = new System.Drawing.Size(444, 24);
         this.radioAll.TabIndex = 2;
         this.radioAll.TabStop = true;
         this.radioAll.Text = "Capture an audit snapshot for all registered SQL Servers.";
         this.radioAll.CheckedChanged += new System.EventHandler(this.radioServer_CheckedChanged);
         // 
         // radioServer
         // 
         this.radioServer.Location = new System.Drawing.Point(12, 40);
         this.radioServer.Name = "radioServer";
         this.radioServer.Size = new System.Drawing.Size(172, 24);
         this.radioServer.TabIndex = 3;
         this.radioServer.Text = "Capture an audit snapshot for";
         this.radioServer.CheckedChanged += new System.EventHandler(this.radioServer_CheckedChanged);
         // 
         // comboServer
         // 
         this.comboServer.Enabled = false;
         this.comboServer.Location = new System.Drawing.Point(180, 40);
         this.comboServer.Name = "comboServer";
         this.comboServer.Size = new System.Drawing.Size(284, 21);
         this.comboServer.TabIndex = 4;
         // 
         // Form_Snapshot
         // 
         this.AcceptButton = this.btnOK;
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.CancelButton = this.btnCancel;
         this.ClientSize = new System.Drawing.Size(474, 116);
         this.Controls.Add(this.comboServer);
         this.Controls.Add(this.radioServer);
         this.Controls.Add(this.radioAll);
         this.Controls.Add(this.btnCancel);
         this.Controls.Add(this.btnOK);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.HelpButton = true;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "Form_Snapshot";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Capture Audit Snapshot";
         this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.Form_Snapshot_HelpRequested);
         this.ResumeLayout(false);

      }
      #endregion

      private System.Windows.Forms.Button btnOK;
      private System.Windows.Forms.Button btnCancel;
      private System.Windows.Forms.RadioButton radioAll;
      private System.Windows.Forms.RadioButton radioServer;
      private System.Windows.Forms.ComboBox comboServer;
   }
}