namespace Idera.SQLcompliance.Application.GUI.Forms
{
   partial class Form_ViewName
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
         this._tbViewName = new System.Windows.Forms.TextBox();
         this._btnCancel = new System.Windows.Forms.Button();
         this._lblDirections = new System.Windows.Forms.Label();
         this._btnOk = new System.Windows.Forms.Button();
         this.SuspendLayout();
         // 
         // _tbViewName
         // 
         this._tbViewName.Location = new System.Drawing.Point(8, 32);
         this._tbViewName.MaxLength = 60;
         this._tbViewName.Name = "_tbViewName";
         this._tbViewName.Size = new System.Drawing.Size(216, 20);
         this._tbViewName.TabIndex = 1;
         // 
         // _btnCancel
         // 
         this._btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this._btnCancel.Location = new System.Drawing.Point(152, 64);
         this._btnCancel.Name = "_btnCancel";
         this._btnCancel.Size = new System.Drawing.Size(75, 23);
         this._btnCancel.TabIndex = 3;
         this._btnCancel.Text = "&Cancel";
         // 
         // _lblDirections
         // 
         this._lblDirections.Location = new System.Drawing.Point(8, 8);
         this._lblDirections.Name = "_lblDirections";
         this._lblDirections.Size = new System.Drawing.Size(216, 16);
         this._lblDirections.TabIndex = 0;
         this._lblDirections.Text = "&Enter a name for the new view:";
         this._lblDirections.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // _btnOk
         // 
         this._btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
         this._btnOk.Location = new System.Drawing.Point(64, 64);
         this._btnOk.Name = "_btnOk";
         this._btnOk.Size = new System.Drawing.Size(75, 23);
         this._btnOk.TabIndex = 2;
         this._btnOk.Text = "&OK";
         this._btnOk.Click += new System.EventHandler(this.Click_btnOk);
         // 
         // Form_ViewName
         // 
         this.AcceptButton = this._btnOk;
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.CancelButton = this._btnCancel;
         this.ClientSize = new System.Drawing.Size(232, 94);
         this.Controls.Add(this._btnOk);
         this.Controls.Add(this._lblDirections);
         this.Controls.Add(this._btnCancel);
         this.Controls.Add(this._tbViewName);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "Form_ViewName";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "View Name";
         this.ResumeLayout(false);
         this.PerformLayout();

      }
      #endregion

      private System.Windows.Forms.TextBox _tbViewName;
      private System.Windows.Forms.Button _btnCancel;
      private System.Windows.Forms.Label _lblDirections;
      private System.Windows.Forms.Button _btnOk;
   }
}