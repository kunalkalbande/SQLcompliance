namespace Idera.SQLcompliance.Application.GUI.Forms
{
   partial class Form_AlertMessage
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
         this._tbBody = new System.Windows.Forms.TextBox();
         this._tbTitle = new System.Windows.Forms.TextBox();
         this.label1 = new System.Windows.Forms.Label();
         this.SuspendLayout();
         // 
         // _btnOk
         // 
         this._btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
         this._btnOk.Location = new System.Drawing.Point(206, 224);
         this._btnOk.Name = "_btnOk";
         this._btnOk.Size = new System.Drawing.Size(75, 23);
         this._btnOk.TabIndex = 3;
         this._btnOk.Text = "&OK";
         // 
         // _tbBody
         // 
         this._tbBody.Location = new System.Drawing.Point(9, 32);
         this._tbBody.Multiline = true;
         this._tbBody.Name = "_tbBody";
         this._tbBody.ReadOnly = true;
         this._tbBody.ScrollBars = System.Windows.Forms.ScrollBars.Both;
         this._tbBody.Size = new System.Drawing.Size(272, 184);
         this._tbBody.TabIndex = 26;
         this._tbBody.Text = "Message";
         // 
         // _tbTitle
         // 
         this._tbTitle.Location = new System.Drawing.Point(49, 8);
         this._tbTitle.Name = "_tbTitle";
         this._tbTitle.ReadOnly = true;
         this._tbTitle.Size = new System.Drawing.Size(232, 20);
         this._tbTitle.TabIndex = 25;
         this._tbTitle.Text = "Title";
         // 
         // label1
         // 
         this.label1.Location = new System.Drawing.Point(9, 8);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(32, 16);
         this.label1.TabIndex = 24;
         this.label1.Text = "Title:";
         this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // Form_AlertMessage
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(290, 256);
         this.Controls.Add(this._tbBody);
         this.Controls.Add(this._tbTitle);
         this.Controls.Add(this.label1);
         this.Controls.Add(this._btnOk);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "Form_AlertMessage";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Alert Message";
         this.ResumeLayout(false);
         this.PerformLayout();

      }
      #endregion

      private System.Windows.Forms.Button _btnOk;
      private System.Windows.Forms.TextBox _tbBody;
      private System.Windows.Forms.TextBox _tbTitle;
      private System.Windows.Forms.Label label1;

	}
}