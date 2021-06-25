namespace Idera.SQLcompliance.Application.GUI.Forms
{
   partial class Form_EmailTemplate
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
         this._lblSubject = new System.Windows.Forms.Label();
         this._lblMessage = new System.Windows.Forms.Label();
         this._tbSubject = new System.Windows.Forms.TextBox();
         this._tbMessage = new System.Windows.Forms.TextBox();
         this._btnCancel = new System.Windows.Forms.Button();
         this._btnOk = new System.Windows.Forms.Button();
         this._listBoxMacros = new System.Windows.Forms.ListBox();
         this.label1 = new System.Windows.Forms.Label();
         this.SuspendLayout();
         // 
         // _lblSubject
         // 
         this._lblSubject.Location = new System.Drawing.Point(8, 10);
         this._lblSubject.Name = "_lblSubject";
         this._lblSubject.Size = new System.Drawing.Size(32, 16);
         this._lblSubject.TabIndex = 0;
         this._lblSubject.Text = "&Title";
         this._lblSubject.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // _lblMessage
         // 
         this._lblMessage.Location = new System.Drawing.Point(8, 44);
         this._lblMessage.Name = "_lblMessage";
         this._lblMessage.Size = new System.Drawing.Size(100, 16);
         this._lblMessage.TabIndex = 2;
         this._lblMessage.Text = "&Message";
         this._lblMessage.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
         // 
         // _tbSubject
         // 
         this._tbSubject.Location = new System.Drawing.Point(40, 8);
         this._tbSubject.Name = "_tbSubject";
         this._tbSubject.Size = new System.Drawing.Size(296, 20);
         this._tbSubject.TabIndex = 1;
         this._tbSubject.Text = "ALERT $AlertId:  $AlertLevel priority alert on $Instance";
         this._tbSubject.Enter += new System.EventHandler(this.Focus_Enter);
         // 
         // _tbMessage
         // 
         this._tbMessage.Location = new System.Drawing.Point(8, 60);
         this._tbMessage.Multiline = true;
         this._tbMessage.Name = "_tbMessage";
         this._tbMessage.ScrollBars = System.Windows.Forms.ScrollBars.Both;
         this._tbMessage.Size = new System.Drawing.Size(328, 200);
         this._tbMessage.TabIndex = 3;
         this._tbMessage.Text = "Event Type:  $Type\r\nUser: $User\r\nInstance: $Instance\r\nDatabase:  $Database\r\nSQL: " +
             " $SQL\r\n\r\nTime of Notification:  $Now\r\nTime of Alert:  $AlertTime\r\nTime of Source" +
             " Event:  $EventTime";
         this._tbMessage.Enter += new System.EventHandler(this.Focus_Enter);
         // 
         // _btnCancel
         // 
         this._btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this._btnCancel.Location = new System.Drawing.Point(448, 276);
         this._btnCancel.Name = "_btnCancel";
         this._btnCancel.Size = new System.Drawing.Size(75, 23);
         this._btnCancel.TabIndex = 7;
         this._btnCancel.Text = "&Cancel";
         // 
         // _btnOk
         // 
         this._btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
         this._btnOk.Location = new System.Drawing.Point(368, 276);
         this._btnOk.Name = "_btnOk";
         this._btnOk.Size = new System.Drawing.Size(75, 23);
         this._btnOk.TabIndex = 6;
         this._btnOk.Text = "&OK";
         // 
         // _listBoxMacros
         // 
         this._listBoxMacros.Location = new System.Drawing.Point(352, 60);
         this._listBoxMacros.Name = "_listBoxMacros";
         this._listBoxMacros.Size = new System.Drawing.Size(168, 199);
         this._listBoxMacros.Sorted = true;
         this._listBoxMacros.TabIndex = 5;
         this._listBoxMacros.DoubleClick += new System.EventHandler(this.DoubleClick_listBoxMacros);
         // 
         // label1
         // 
         this.label1.Location = new System.Drawing.Point(352, 10);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(168, 49);
         this.label1.TabIndex = 4;
         this.label1.Text = "&Double-click a variable to add it to the email subject or message.";
         this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
         // 
         // Form_EmailTemplate
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.CancelButton = this._btnCancel;
         this.ClientSize = new System.Drawing.Size(530, 308);
         this.Controls.Add(this.label1);
         this.Controls.Add(this._listBoxMacros);
         this.Controls.Add(this._tbMessage);
         this.Controls.Add(this._tbSubject);
         this.Controls.Add(this._btnOk);
         this.Controls.Add(this._btnCancel);
         this.Controls.Add(this._lblMessage);
         this.Controls.Add(this._lblSubject);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.HelpButton = true;
         this.KeyPreview = true;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "Form_EmailTemplate";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Alert Message Template";
         this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.Form_EmailTemplate_HelpRequested);
         this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyDown_Form_EmailTemplate);
         this.ResumeLayout(false);
         this.PerformLayout();

      }
      #endregion

      private System.Windows.Forms.Label _lblSubject;
      private System.Windows.Forms.Label _lblMessage;
      private System.Windows.Forms.TextBox _tbSubject;
      private System.Windows.Forms.TextBox _tbMessage;
      private System.Windows.Forms.Button _btnCancel;
      private System.Windows.Forms.Button _btnOk;
      private System.Windows.Forms.ListBox _listBoxMacros;
      private System.Windows.Forms.Label label1;

	}
}