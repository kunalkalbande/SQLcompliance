namespace Idera.SQLcompliance.Application.GUI.Forms
{
   partial class Form_EventLogType
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
         this._comboEntryTypes = new System.Windows.Forms.ComboBox();
         this._lblDirections = new System.Windows.Forms.Label();
         this.SuspendLayout();
         // 
         // _btnCancel
         // 
         this._btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this._btnCancel.Location = new System.Drawing.Point(260, 44);
         this._btnCancel.Name = "_btnCancel";
         this._btnCancel.Size = new System.Drawing.Size(75, 23);
         this._btnCancel.TabIndex = 3;
         this._btnCancel.Text = "&Cancel";
         // 
         // _btnOk
         // 
         this._btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
         this._btnOk.Location = new System.Drawing.Point(180, 44);
         this._btnOk.Name = "_btnOk";
         this._btnOk.Size = new System.Drawing.Size(75, 23);
         this._btnOk.TabIndex = 2;
         this._btnOk.Text = "&OK";
         // 
         // _comboEntryTypes
         // 
         this._comboEntryTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this._comboEntryTypes.Items.AddRange(new object[] {
            "Information",
            "Warning",
            "Error"});
         this._comboEntryTypes.Location = new System.Drawing.Point(184, 6);
         this._comboEntryTypes.Name = "_comboEntryTypes";
         this._comboEntryTypes.Size = new System.Drawing.Size(152, 21);
         this._comboEntryTypes.TabIndex = 1;
         // 
         // _lblDirections
         // 
         this._lblDirections.Location = new System.Drawing.Point(8, 8);
         this._lblDirections.Name = "_lblDirections";
         this._lblDirections.Size = new System.Drawing.Size(176, 16);
         this._lblDirections.TabIndex = 0;
         this._lblDirections.Text = "&Select the type of event log entry:";
         // 
         // Form_EventLogType
         // 
         this.AcceptButton = this._btnOk;
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.CancelButton = this._btnCancel;
         this.ClientSize = new System.Drawing.Size(342, 76);
         this.Controls.Add(this._btnCancel);
         this.Controls.Add(this._btnOk);
         this.Controls.Add(this._comboEntryTypes);
         this.Controls.Add(this._lblDirections);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "Form_EventLogType";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Event Log Entry Type";
         this.ResumeLayout(false);

      }
      #endregion

      private System.Windows.Forms.Button _btnCancel;
      private System.Windows.Forms.Button _btnOk;
      private System.Windows.Forms.Label _lblDirections;
      private System.Windows.Forms.ComboBox _comboEntryTypes;
	}
}