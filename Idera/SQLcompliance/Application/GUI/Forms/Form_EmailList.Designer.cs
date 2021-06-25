namespace Idera.SQLcompliance.Application.GUI.Forms
{
   partial class Form_EmailList
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
         this._btnAdd = new System.Windows.Forms.Button();
         this._btnRemove = new System.Windows.Forms.Button();
         this._btnCancel = new System.Windows.Forms.Button();
         this._btnOk = new System.Windows.Forms.Button();
         this._lblDirections = new System.Windows.Forms.Label();
         this._lblSearchList = new System.Windows.Forms.Label();
         this._tbStringEntry = new System.Windows.Forms.TextBox();
         this._listBoxStringList = new System.Windows.Forms.ListBox();
         this.SuspendLayout();
         // 
         // _btnAdd
         // 
         this._btnAdd.Enabled = false;
         this._btnAdd.Location = new System.Drawing.Point(328, 28);
         this._btnAdd.Name = "_btnAdd";
         this._btnAdd.Size = new System.Drawing.Size(75, 20);
         this._btnAdd.TabIndex = 2;
         this._btnAdd.Text = "&Add";
         this._btnAdd.Click += new System.EventHandler(this.Click_btnAdd);
         // 
         // _btnRemove
         // 
         this._btnRemove.Enabled = false;
         this._btnRemove.Location = new System.Drawing.Point(328, 80);
         this._btnRemove.Name = "_btnRemove";
         this._btnRemove.Size = new System.Drawing.Size(75, 20);
         this._btnRemove.TabIndex = 5;
         this._btnRemove.Text = "&Remove";
         this._btnRemove.Click += new System.EventHandler(this.Click_btnRemove);
         // 
         // _btnCancel
         // 
         this._btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this._btnCancel.Location = new System.Drawing.Point(328, 188);
         this._btnCancel.Name = "_btnCancel";
         this._btnCancel.Size = new System.Drawing.Size(75, 23);
         this._btnCancel.TabIndex = 7;
         this._btnCancel.Text = "&Cancel";
         this._btnCancel.Click += new System.EventHandler(this.Click_btnCancel);
         // 
         // _btnOk
         // 
         this._btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
         this._btnOk.Location = new System.Drawing.Point(248, 188);
         this._btnOk.Name = "_btnOk";
         this._btnOk.RightToLeft = System.Windows.Forms.RightToLeft.No;
         this._btnOk.Size = new System.Drawing.Size(75, 23);
         this._btnOk.TabIndex = 6;
         this._btnOk.Text = "&OK";
         this._btnOk.Click += new System.EventHandler(this.Click_btnOK);
         // 
         // _lblDirections
         // 
         this._lblDirections.Location = new System.Drawing.Point(8, 8);
         this._lblDirections.Name = "_lblDirections";
         this._lblDirections.Size = new System.Drawing.Size(316, 16);
         this._lblDirections.TabIndex = 0;
         this._lblDirections.Text = "&Specify an email address for each person to receive this alert:";
         this._lblDirections.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // _lblSearchList
         // 
         this._lblSearchList.Location = new System.Drawing.Point(8, 60);
         this._lblSearchList.Name = "_lblSearchList";
         this._lblSearchList.Size = new System.Drawing.Size(280, 16);
         this._lblSearchList.TabIndex = 3;
         this._lblSearchList.Text = "&Use the following email addresses:";
         this._lblSearchList.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // _tbStringEntry
         // 
         this._tbStringEntry.Location = new System.Drawing.Point(8, 28);
         this._tbStringEntry.Name = "_tbStringEntry";
         this._tbStringEntry.Size = new System.Drawing.Size(308, 20);
         this._tbStringEntry.TabIndex = 1;
         this._tbStringEntry.TextChanged += new System.EventHandler(this.TextChanged_tbStringEntry);
         // 
         // _listBoxStringList
         // 
         this._listBoxStringList.Location = new System.Drawing.Point(8, 80);
         this._listBoxStringList.Name = "_listBoxStringList";
         this._listBoxStringList.Size = new System.Drawing.Size(308, 95);
         this._listBoxStringList.TabIndex = 4;
         this._listBoxStringList.SelectedIndexChanged += new System.EventHandler(this.SelectedIndexChanged_listBoxStringList);
         // 
         // Form_EmailList
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.CancelButton = this._btnCancel;
         this.ClientSize = new System.Drawing.Size(410, 220);
         this.Controls.Add(this._listBoxStringList);
         this.Controls.Add(this._tbStringEntry);
         this.Controls.Add(this._lblSearchList);
         this.Controls.Add(this._lblDirections);
         this.Controls.Add(this._btnOk);
         this.Controls.Add(this._btnCancel);
         this.Controls.Add(this._btnRemove);
         this.Controls.Add(this._btnAdd);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.HelpButton = true;
         this.KeyPreview = true;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "Form_EmailList";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Specify Addresses";
         this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.Form_EmailList_HelpRequested);
         this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyDown_Form_EmailList);
         this.ResumeLayout(false);
         this.PerformLayout();

      }
      #endregion

      private System.Windows.Forms.Button _btnAdd;
      private System.Windows.Forms.Button _btnRemove;
      private System.Windows.Forms.Button _btnCancel;
      private System.Windows.Forms.Button _btnOk;
      private System.Windows.Forms.Label _lblDirections;
      private System.Windows.Forms.Label _lblSearchList;
      private System.Windows.Forms.TextBox _tbStringEntry;
      private System.Windows.Forms.ListBox _listBoxStringList;

	}
}