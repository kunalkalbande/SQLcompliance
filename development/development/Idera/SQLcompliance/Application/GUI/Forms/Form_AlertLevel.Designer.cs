namespace Idera.SQLcompliance.Application.GUI.Forms
{
   partial class Form_AlertLevel
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
         this._btnCancel = new System.Windows.Forms.Button();
         this._btnOk = new System.Windows.Forms.Button();
         this._comboAlertLevel = new System.Windows.Forms.ComboBox();
         this._lblDirections = new System.Windows.Forms.Label();
         this.SuspendLayout();
         // 
         // _btnCancel
         // 
         this._btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this._btnCancel.Location = new System.Drawing.Point(168, 48);
         this._btnCancel.Name = "_btnCancel";
         this._btnCancel.Size = new System.Drawing.Size(75, 23);
         this._btnCancel.TabIndex = 3;
         this._btnCancel.Text = "&Cancel";
         // 
         // _btnOk
         // 
         this._btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
         this._btnOk.Location = new System.Drawing.Point(88, 48);
         this._btnOk.Name = "_btnOk";
         this._btnOk.Size = new System.Drawing.Size(75, 23);
         this._btnOk.TabIndex = 2;
         this._btnOk.Text = "&OK";
         // 
         // _comboAlertLevel
         // 
         this._comboAlertLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this._comboAlertLevel.Location = new System.Drawing.Point(124, 8);
         this._comboAlertLevel.Name = "_comboAlertLevel";
         this._comboAlertLevel.Size = new System.Drawing.Size(120, 21);
         this._comboAlertLevel.TabIndex = 1;
         // 
         // _lblDirections
         // 
         this._lblDirections.Location = new System.Drawing.Point(8, 12);
         this._lblDirections.Name = "_lblDirections";
         this._lblDirections.Size = new System.Drawing.Size(112, 16);
         this._lblDirections.TabIndex = 0;
         this._lblDirections.Text = "&Select the alert level:";
         // 
         // Form_AlertLevel
         // 
         this.AcceptButton = this._btnOk;
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.CancelButton = this._btnCancel;
         this.ClientSize = new System.Drawing.Size(250, 80);
         this.Controls.Add(this._btnCancel);
         this.Controls.Add(this._btnOk);
         this.Controls.Add(this._comboAlertLevel);
         this.Controls.Add(this._lblDirections);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "Form_AlertLevel";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Alert Level";
         this.ResumeLayout(false);

      }
      #endregion

      private System.Windows.Forms.Button _btnCancel;
      private System.Windows.Forms.Button _btnOk;
      private System.Windows.Forms.ComboBox _comboAlertLevel;
      private System.Windows.Forms.Label _lblDirections;
	}
}