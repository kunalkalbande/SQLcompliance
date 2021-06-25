namespace Idera.SQLcompliance.Application.GUI.Forms
{
   partial class Form_EventTypeList
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
         this._btnCancel = new System.Windows.Forms.Button();
         this._lblSelectionDirections = new System.Windows.Forms.Label();
         this._listBoxEventTypes = new System.Windows.Forms.CheckedListBox();
         this._btnSelectAll = new System.Windows.Forms.Button();
         this._btnClearAll = new System.Windows.Forms.Button();
         this.SuspendLayout();
         // 
         // _btnOk
         // 
         this._btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
         this._btnOk.Location = new System.Drawing.Point(204, 28);
         this._btnOk.Name = "_btnOk";
         this._btnOk.Size = new System.Drawing.Size(75, 23);
         this._btnOk.TabIndex = 4;
         this._btnOk.Text = "&OK";
         // 
         // _btnCancel
         // 
         this._btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this._btnCancel.Location = new System.Drawing.Point(204, 56);
         this._btnCancel.Name = "_btnCancel";
         this._btnCancel.Size = new System.Drawing.Size(75, 23);
         this._btnCancel.TabIndex = 5;
         this._btnCancel.Text = "&Cancel";
         // 
         // _lblSelectionDirections
         // 
         this._lblSelectionDirections.Location = new System.Drawing.Point(8, 8);
         this._lblSelectionDirections.Name = "_lblSelectionDirections";
         this._lblSelectionDirections.Size = new System.Drawing.Size(188, 16);
         this._lblSelectionDirections.TabIndex = 0;
         this._lblSelectionDirections.Text = "S&elect the event types to exclude:";
         this._lblSelectionDirections.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // _listBoxEventTypes
         // 
         this._listBoxEventTypes.CheckOnClick = true;
         this._listBoxEventTypes.Location = new System.Drawing.Point(8, 28);
         this._listBoxEventTypes.Name = "_listBoxEventTypes";
         this._listBoxEventTypes.Size = new System.Drawing.Size(188, 244);
         this._listBoxEventTypes.Sorted = true;
         this._listBoxEventTypes.TabIndex = 1;
         this._listBoxEventTypes.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.ItemCheck_listBoxEventTypes);
         // 
         // _btnSelectAll
         // 
         this._btnSelectAll.Location = new System.Drawing.Point(24, 280);
         this._btnSelectAll.Name = "_btnSelectAll";
         this._btnSelectAll.Size = new System.Drawing.Size(75, 23);
         this._btnSelectAll.TabIndex = 2;
         this._btnSelectAll.Text = "&Select All";
         this._btnSelectAll.Click += new System.EventHandler(this.Click_btnSelectAll);
         // 
         // _btnClearAll
         // 
         this._btnClearAll.Location = new System.Drawing.Point(104, 280);
         this._btnClearAll.Name = "_btnClearAll";
         this._btnClearAll.Size = new System.Drawing.Size(75, 23);
         this._btnClearAll.TabIndex = 3;
         this._btnClearAll.Text = "C&lear All";
         this._btnClearAll.Click += new System.EventHandler(this.Click_btnClearAll);
         // 
         // Form_EventTypeList
         // 
         this.AcceptButton = this._btnOk;
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.CancelButton = this._btnCancel;
         this.ClientSize = new System.Drawing.Size(286, 312);
         this.Controls.Add(this._btnClearAll);
         this.Controls.Add(this._btnSelectAll);
         this.Controls.Add(this._listBoxEventTypes);
         this.Controls.Add(this._lblSelectionDirections);
         this.Controls.Add(this._btnCancel);
         this.Controls.Add(this._btnOk);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "Form_EventTypeList";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Exclude Event Types";
         this.ResumeLayout(false);

      }
      #endregion

      private System.Windows.Forms.Button _btnOk;
      private System.Windows.Forms.Button _btnCancel;
      private System.Windows.Forms.Label _lblSelectionDirections;
      private System.Windows.Forms.CheckedListBox _listBoxEventTypes;
      private System.Windows.Forms.Button _btnSelectAll;
      private System.Windows.Forms.Button _btnClearAll;
	}
}