namespace Idera.SQLcompliance.Application.GUI.Forms
{
   partial class Form_Boolean
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
         this._lblDirections = new System.Windows.Forms.Label();
         this._comboBool = new System.Windows.Forms.ComboBox();
         this._btnOk = new System.Windows.Forms.Button();
         this._btnCancel = new System.Windows.Forms.Button();
         this.SuspendLayout();
         // 
         // _lblDirections
         // 
         this._lblDirections.Location = new System.Drawing.Point(8, 12);
         this._lblDirections.Name = "_lblDirections";
         this._lblDirections.Size = new System.Drawing.Size(144, 16);
         this._lblDirections.TabIndex = 0;
         this._lblDirections.Text = "Directions go here";
         // 
         // _comboBool
         // 
         this._comboBool.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this._comboBool.Items.AddRange(new object[] {
            "True",
            "False"});
         this._comboBool.Location = new System.Drawing.Point(148, 8);
         this._comboBool.Name = "_comboBool";
         this._comboBool.Size = new System.Drawing.Size(112, 21);
         this._comboBool.TabIndex = 1;
         // 
         // _btnOk
         // 
         this._btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
         this._btnOk.Location = new System.Drawing.Point(108, 48);
         this._btnOk.Name = "_btnOk";
         this._btnOk.Size = new System.Drawing.Size(75, 23);
         this._btnOk.TabIndex = 2;
         this._btnOk.Text = "&OK";
         // 
         // _btnCancel
         // 
         this._btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this._btnCancel.Location = new System.Drawing.Point(185, 48);
         this._btnCancel.Name = "_btnCancel";
         this._btnCancel.Size = new System.Drawing.Size(75, 23);
         this._btnCancel.TabIndex = 3;
         this._btnCancel.Text = "&Cancel";
         // 
         // Form_Boolean
         // 
         this.AcceptButton = this._btnOk;
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.CancelButton = this._btnCancel;
         this.ClientSize = new System.Drawing.Size(266, 80);
         this.Controls.Add(this._btnCancel);
         this.Controls.Add(this._btnOk);
         this.Controls.Add(this._comboBool);
         this.Controls.Add(this._lblDirections);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "Form_Boolean";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Boolean";
         this.ResumeLayout(false);

      }
      #endregion

      private System.Windows.Forms.Label _lblDirections;
      private System.Windows.Forms.ComboBox _comboBool;
      private System.Windows.Forms.Button _btnOk;
      private System.Windows.Forms.Button _btnCancel;

	}
}