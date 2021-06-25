namespace Idera.SQLcompliance.Application.GUI.Forms
{
   partial class Form_AlertingOptions
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
         this._btnOk = new System.Windows.Forms.Button();
         this._btnCancel = new System.Windows.Forms.Button();
         this._btnTest = new System.Windows.Forms.Button();
         this._lblSenderAddress = new System.Windows.Forms.Label();
         this._tbSenderAddress = new System.Windows.Forms.TextBox();
         this._cbUseSsl = new System.Windows.Forms.CheckBox();
         this._cbUseAuth = new System.Windows.Forms.CheckBox();
         this._lblPassword = new System.Windows.Forms.Label();
         this._tbPort = new System.Windows.Forms.TextBox();
         this._lblPort = new System.Windows.Forms.Label();
         this._lblUsername = new System.Windows.Forms.Label();
         this._lblServer = new System.Windows.Forms.Label();
         this._tbUsername = new System.Windows.Forms.TextBox();
         this._tbPassword = new System.Windows.Forms.TextBox();
         this._tbServer = new System.Windows.Forms.TextBox();
         this.SuspendLayout();
         // 
         // _btnOk
         // 
         this._btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
         this._btnOk.Location = new System.Drawing.Point(124, 176);
         this._btnOk.Name = "_btnOk";
         this._btnOk.Size = new System.Drawing.Size(75, 23);
         this._btnOk.TabIndex = 13;
         this._btnOk.Text = "&OK";
         // 
         // _btnCancel
         // 
         this._btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this._btnCancel.Location = new System.Drawing.Point(204, 176);
         this._btnCancel.Name = "_btnCancel";
         this._btnCancel.Size = new System.Drawing.Size(75, 23);
         this._btnCancel.TabIndex = 14;
         this._btnCancel.Text = "&Cancel";
         // 
         // _btnTest
         // 
         this._btnTest.Location = new System.Drawing.Point(32, 176);
         this._btnTest.Name = "_btnTest";
         this._btnTest.Size = new System.Drawing.Size(72, 23);
         this._btnTest.TabIndex = 12;
         this._btnTest.Text = "&Test...";
         this._btnTest.Click += new System.EventHandler(this.Click_btnTest);
         // 
         // _lblSenderAddress
         // 
         this._lblSenderAddress.Location = new System.Drawing.Point(8, 132);
         this._lblSenderAddress.Name = "_lblSenderAddress";
         this._lblSenderAddress.Size = new System.Drawing.Size(88, 20);
         this._lblSenderAddress.TabIndex = 10;
         this._lblSenderAddress.Text = "S&ender Address:";
         this._lblSenderAddress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // _tbSenderAddress
         // 
         this._tbSenderAddress.Location = new System.Drawing.Point(96, 132);
         this._tbSenderAddress.Name = "_tbSenderAddress";
         this._tbSenderAddress.Size = new System.Drawing.Size(184, 20);
         this._tbSenderAddress.TabIndex = 11;
         // 
         // _cbUseSsl
         // 
         this._cbUseSsl.Location = new System.Drawing.Point(216, 40);
         this._cbUseSsl.Name = "_cbUseSsl";
         this._cbUseSsl.Size = new System.Drawing.Size(64, 20);
         this._cbUseSsl.TabIndex = 5;
         this._cbUseSsl.Text = "SS&L";
         // 
         // _cbUseAuth
         // 
         this._cbUseAuth.Location = new System.Drawing.Point(8, 40);
         this._cbUseAuth.Name = "_cbUseAuth";
         this._cbUseAuth.Size = new System.Drawing.Size(152, 20);
         this._cbUseAuth.TabIndex = 4;
         this._cbUseAuth.Text = "&Requires Authentication";
         this._cbUseAuth.CheckedChanged += new System.EventHandler(this.CheckedChanged_cbUseAuth);
         // 
         // _lblPassword
         // 
         this._lblPassword.Enabled = false;
         this._lblPassword.Location = new System.Drawing.Point(8, 96);
         this._lblPassword.Name = "_lblPassword";
         this._lblPassword.Size = new System.Drawing.Size(64, 20);
         this._lblPassword.TabIndex = 8;
         this._lblPassword.Text = "P&assword:";
         this._lblPassword.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // _tbPort
         // 
         this._tbPort.Location = new System.Drawing.Point(248, 8);
         this._tbPort.Name = "_tbPort";
         this._tbPort.Size = new System.Drawing.Size(32, 20);
         this._tbPort.TabIndex = 3;
         this._tbPort.Text = "25";
         // 
         // _lblPort
         // 
         this._lblPort.Location = new System.Drawing.Point(216, 8);
         this._lblPort.Name = "_lblPort";
         this._lblPort.Size = new System.Drawing.Size(32, 20);
         this._lblPort.TabIndex = 2;
         this._lblPort.Text = "&Port:";
         this._lblPort.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // _lblUsername
         // 
         this._lblUsername.Enabled = false;
         this._lblUsername.Location = new System.Drawing.Point(8, 72);
         this._lblUsername.Name = "_lblUsername";
         this._lblUsername.Size = new System.Drawing.Size(64, 20);
         this._lblUsername.TabIndex = 6;
         this._lblUsername.Text = "&Username:";
         this._lblUsername.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // _lblServer
         // 
         this._lblServer.Location = new System.Drawing.Point(8, 8);
         this._lblServer.Name = "_lblServer";
         this._lblServer.Size = new System.Drawing.Size(80, 20);
         this._lblServer.TabIndex = 0;
         this._lblServer.Text = "&SMTP Server:";
         this._lblServer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // _tbUsername
         // 
         this._tbUsername.Enabled = false;
         this._tbUsername.Location = new System.Drawing.Point(72, 72);
         this._tbUsername.Name = "_tbUsername";
         this._tbUsername.Size = new System.Drawing.Size(128, 20);
         this._tbUsername.TabIndex = 7;
         // 
         // _tbPassword
         // 
         this._tbPassword.Enabled = false;
         this._tbPassword.Location = new System.Drawing.Point(72, 96);
         this._tbPassword.Name = "_tbPassword";
         this._tbPassword.PasswordChar = '*';
         this._tbPassword.Size = new System.Drawing.Size(128, 20);
         this._tbPassword.TabIndex = 9;
         // 
         // _tbServer
         // 
         this._tbServer.Location = new System.Drawing.Point(88, 8);
         this._tbServer.Name = "_tbServer";
         this._tbServer.Size = new System.Drawing.Size(112, 20);
         this._tbServer.TabIndex = 1;
         // 
         // Form_AlertingOptions
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(286, 208);
         this.Controls.Add(this._btnTest);
         this.Controls.Add(this._lblSenderAddress);
         this.Controls.Add(this._tbSenderAddress);
         this.Controls.Add(this._tbPort);
         this.Controls.Add(this._tbUsername);
         this.Controls.Add(this._tbPassword);
         this.Controls.Add(this._tbServer);
         this.Controls.Add(this._cbUseSsl);
         this.Controls.Add(this._cbUseAuth);
         this.Controls.Add(this._lblPassword);
         this.Controls.Add(this._lblPort);
         this.Controls.Add(this._lblUsername);
         this.Controls.Add(this._lblServer);
         this.Controls.Add(this._btnOk);
         this.Controls.Add(this._btnCancel);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.HelpButton = true;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "Form_AlertingOptions";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Configure Email Settings";
         this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.Form_AlertingOptions_HelpRequested);
         this.ResumeLayout(false);
         this.PerformLayout();

      }
      #endregion

      private System.Windows.Forms.Button _btnOk;
      private System.Windows.Forms.Button _btnCancel;
      private System.Windows.Forms.Button _btnTest;
      private System.Windows.Forms.Label _lblSenderAddress;
      private System.Windows.Forms.TextBox _tbSenderAddress;
      private System.Windows.Forms.CheckBox _cbUseSsl;
      private System.Windows.Forms.CheckBox _cbUseAuth;
      private System.Windows.Forms.Label _lblPassword;
      private System.Windows.Forms.TextBox _tbPort;
      private System.Windows.Forms.Label _lblPort;
      private System.Windows.Forms.Label _lblUsername;
      private System.Windows.Forms.Label _lblServer;
      private System.Windows.Forms.TextBox _tbUsername;
      private System.Windows.Forms.TextBox _tbPassword;
      private System.Windows.Forms.TextBox _tbServer;

	}
}