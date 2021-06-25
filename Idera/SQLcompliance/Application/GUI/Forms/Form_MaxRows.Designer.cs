namespace Idera.SQLcompliance.Application.GUI.Forms
{
   partial class Form_MaxRows
   {
      /// <summary>
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
      protected override void Dispose(bool disposing)
      {
         if (disposing && (components != null))
         {
            components.Dispose();
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
         System.Windows.Forms.Label _lblHeader;
         this._comboMaxRows = new System.Windows.Forms.ComboBox();
         this._btnCancel = new System.Windows.Forms.Button();
         this._btnOk = new System.Windows.Forms.Button();
         _lblHeader = new System.Windows.Forms.Label();
         this.SuspendLayout();
         // 
         // _lblHeader
         // 
         _lblHeader.Location = new System.Drawing.Point(12, 9);
         _lblHeader.Name = "_lblHeader";
         _lblHeader.Size = new System.Drawing.Size(271, 42);
         _lblHeader.TabIndex = 0;
         _lblHeader.Text = "Select how many rows of change data to capture per DML transaction:";
         // 
         // _comboMaxRows
         // 
         this._comboMaxRows.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this._comboMaxRows.FormattingEnabled = true;
         this._comboMaxRows.Items.AddRange(new object[] {
            "All",
            "1000",
            "100",
            "10",
            "1",
            "0"});
         this._comboMaxRows.Location = new System.Drawing.Point(15, 54);
         this._comboMaxRows.Name = "_comboMaxRows";
         this._comboMaxRows.Size = new System.Drawing.Size(187, 21);
         this._comboMaxRows.TabIndex = 1;
         // 
         // _btnCancel
         // 
         this._btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this._btnCancel.Location = new System.Drawing.Point(211, 96);
         this._btnCancel.Name = "_btnCancel";
         this._btnCancel.Size = new System.Drawing.Size(75, 23);
         this._btnCancel.TabIndex = 2;
         this._btnCancel.Text = "Cancel";
         this._btnCancel.UseVisualStyleBackColor = true;
         // 
         // _btnOk
         // 
         this._btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
         this._btnOk.Location = new System.Drawing.Point(130, 96);
         this._btnOk.Name = "_btnOk";
         this._btnOk.Size = new System.Drawing.Size(75, 23);
         this._btnOk.TabIndex = 3;
         this._btnOk.Text = "OK";
         this._btnOk.UseVisualStyleBackColor = true;
         // 
         // Form_MaxRows
         // 
         this.AcceptButton = this._btnOk;
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.CancelButton = this._btnCancel;
         this.ClientSize = new System.Drawing.Size(298, 131);
         this.Controls.Add(this._btnOk);
         this.Controls.Add(this._btnCancel);
         this.Controls.Add(this._comboMaxRows);
         this.Controls.Add(_lblHeader);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.Name = "Form_MaxRows";
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Maximum Rows";
         this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.ComboBox _comboMaxRows;
      private System.Windows.Forms.Button _btnCancel;
      private System.Windows.Forms.Button _btnOk;
   }
}