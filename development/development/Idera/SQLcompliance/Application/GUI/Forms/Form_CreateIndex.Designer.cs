namespace Idera.SQLcompliance.Application.GUI.Forms
{
   partial class Form_CreateIndex
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
         this._lnkText = new System.Windows.Forms.LinkLabel();
         this._lblUpdateNow = new System.Windows.Forms.Label();
         this._btnNo = new System.Windows.Forms.Button();
         this._btnYes = new System.Windows.Forms.Button();
         this.SuspendLayout();
         // 
         // _lnkText
         // 
         this._lnkText.Location = new System.Drawing.Point(12, 12);
         this._lnkText.Name = "_lnkText";
         this._lnkText.Size = new System.Drawing.Size(364, 64);
         this._lnkText.TabIndex = 2;
         this._lnkText.TabStop = true;
         this._lnkText.Text = "linkLabel1";
         // 
         // _lblUpdateNow
         // 
         this._lblUpdateNow.Location = new System.Drawing.Point(12, 84);
         this._lblUpdateNow.Name = "_lblUpdateNow";
         this._lblUpdateNow.Size = new System.Drawing.Size(364, 44);
         this._lblUpdateNow.TabIndex = 4;
         this._lblUpdateNow.Text = "Do you want to update your event database now?";
         // 
         // _btnNo
         // 
         this._btnNo.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this._btnNo.Location = new System.Drawing.Point(300, 136);
         this._btnNo.Name = "_btnNo";
         this._btnNo.Size = new System.Drawing.Size(75, 23);
         this._btnNo.TabIndex = 1;
         this._btnNo.Text = "&No";
         // 
         // _btnYes
         // 
         this._btnYes.DialogResult = System.Windows.Forms.DialogResult.OK;
         this._btnYes.Location = new System.Drawing.Point(216, 136);
         this._btnYes.Name = "_btnYes";
         this._btnYes.Size = new System.Drawing.Size(75, 23);
         this._btnYes.TabIndex = 0;
         this._btnYes.Text = "&Yes";
         // 
         // Form_CreateIndex
         // 
         this.AcceptButton = this._btnYes;
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.CancelButton = this._btnNo;
         this.ClientSize = new System.Drawing.Size(382, 168);
         this.Controls.Add(this._btnYes);
         this.Controls.Add(this._btnNo);
         this.Controls.Add(this._lblUpdateNow);
         this.Controls.Add(this._lnkText);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "Form_CreateIndex";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Update Indexes";
         this.ResumeLayout(false);

      }
      #endregion

      private System.Windows.Forms.Button _btnNo;
      private System.Windows.Forms.Button _btnYes;
      private System.Windows.Forms.LinkLabel _lnkText;
      private System.Windows.Forms.Label _lblUpdateNow;

	}
}