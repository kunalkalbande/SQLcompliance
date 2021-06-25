namespace Idera.SQLcompliance.Application.GUI.Forms
{
   partial class Form_EventCategorySelector
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
         this._lblSelectCategory = new System.Windows.Forms.Label();
         this._comboCategory = new System.Windows.Forms.ComboBox();
         this._btnCancel = new System.Windows.Forms.Button();
         this._btnOk = new System.Windows.Forms.Button();
         this.SuspendLayout();
         // 
         // _lblSelectCategory
         // 
         this._lblSelectCategory.Location = new System.Drawing.Point(8, 8);
         this._lblSelectCategory.Name = "_lblSelectCategory";
         this._lblSelectCategory.Size = new System.Drawing.Size(136, 21);
         this._lblSelectCategory.TabIndex = 0;
         this._lblSelectCategory.Text = "Select an event category:";
         this._lblSelectCategory.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // _comboCategory
         // 
         this._comboCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this._comboCategory.Location = new System.Drawing.Point(144, 8);
         this._comboCategory.Name = "_comboCategory";
         this._comboCategory.Size = new System.Drawing.Size(184, 21);
         this._comboCategory.Sorted = true;
         this._comboCategory.TabIndex = 2;
         this._comboCategory.SelectedIndexChanged += new System.EventHandler(this.SelectedIndexChanged_comboCategory);
         // 
         // _btnCancel
         // 
         this._btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this._btnCancel.Location = new System.Drawing.Point(252, 52);
         this._btnCancel.Name = "_btnCancel";
         this._btnCancel.Size = new System.Drawing.Size(75, 23);
         this._btnCancel.TabIndex = 4;
         this._btnCancel.Text = "&Cancel";
         // 
         // _btnOk
         // 
         this._btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
         this._btnOk.Location = new System.Drawing.Point(164, 52);
         this._btnOk.Name = "_btnOk";
         this._btnOk.Size = new System.Drawing.Size(75, 23);
         this._btnOk.TabIndex = 5;
         this._btnOk.Text = "&OK";
         // 
         // Form_EventCategorySelector
         // 
         this.AcceptButton = this._btnOk;
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.CancelButton = this._btnCancel;
         this.ClientSize = new System.Drawing.Size(334, 84);
         this.Controls.Add(this._btnOk);
         this.Controls.Add(this._btnCancel);
         this.Controls.Add(this._comboCategory);
         this.Controls.Add(this._lblSelectCategory);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "Form_EventCategorySelector";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Specify Event Category";
         this.ResumeLayout(false);

      }
		#endregion

      private System.Windows.Forms.Label _lblSelectCategory;
      private System.Windows.Forms.ComboBox _comboCategory;
      private System.Windows.Forms.Button _btnCancel;
      private System.Windows.Forms.Button _btnOk;

	}
}