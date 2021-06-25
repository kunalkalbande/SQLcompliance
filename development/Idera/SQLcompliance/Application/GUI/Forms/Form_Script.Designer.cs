namespace Idera.SQLcompliance.Application.GUI.Forms
{
   partial class Form_Script
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
         this._tbScript = new System.Windows.Forms.TextBox();
         this._btnClose = new System.Windows.Forms.Button();
         this._btnCopy = new System.Windows.Forms.Button();
         this._btnSave = new System.Windows.Forms.Button();
         this.SuspendLayout();
         // 
         // _tbScript
         // 
         this._tbScript.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                     | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this._tbScript.Location = new System.Drawing.Point(8, 8);
         this._tbScript.Multiline = true;
         this._tbScript.Name = "_tbScript";
         this._tbScript.ScrollBars = System.Windows.Forms.ScrollBars.Both;
         this._tbScript.Size = new System.Drawing.Size(356, 220);
         this._tbScript.TabIndex = 0;
         // 
         // _btnClose
         // 
         this._btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this._btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
         this._btnClose.Location = new System.Drawing.Point(292, 236);
         this._btnClose.Name = "_btnClose";
         this._btnClose.Size = new System.Drawing.Size(75, 23);
         this._btnClose.TabIndex = 1;
         this._btnClose.Text = "C&lose";
         // 
         // _btnCopy
         // 
         this._btnCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this._btnCopy.Location = new System.Drawing.Point(208, 236);
         this._btnCopy.Name = "_btnCopy";
         this._btnCopy.Size = new System.Drawing.Size(75, 23);
         this._btnCopy.TabIndex = 2;
         this._btnCopy.Text = "&Copy";
         this._btnCopy.Click += new System.EventHandler(this.Click_btnCopy);
         // 
         // _btnSave
         // 
         this._btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this._btnSave.Location = new System.Drawing.Point(124, 236);
         this._btnSave.Name = "_btnSave";
         this._btnSave.Size = new System.Drawing.Size(75, 23);
         this._btnSave.TabIndex = 3;
         this._btnSave.Text = "&Save";
         this._btnSave.Click += new System.EventHandler(this.Click_btnSave);
         // 
         // Form_Script
         // 
         this.AcceptButton = this._btnClose;
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(372, 266);
         this.Controls.Add(this._btnSave);
         this.Controls.Add(this._btnCopy);
         this.Controls.Add(this._btnClose);
         this.Controls.Add(this._tbScript);
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.MinimumSize = new System.Drawing.Size(170, 150);
         this.Name = "Form_Script";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "View Script";
         this.ResumeLayout(false);
         this.PerformLayout();

      }
      #endregion

      private System.Windows.Forms.TextBox _tbScript;
      private System.Windows.Forms.Button _btnClose;
      private System.Windows.Forms.Button _btnCopy;
      private System.Windows.Forms.Button _btnSave;

	}
}