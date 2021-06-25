namespace Idera.SQLcompliance.Application.GUI.Forms
{
   partial class Form_StringSearch
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
         this._tbStringEntry = new System.Windows.Forms.TextBox();
         this._listBoxStringList = new System.Windows.Forms.ListBox();
         this._rbInclude = new System.Windows.Forms.RadioButton();
         this._rbExclude = new System.Windows.Forms.RadioButton();
         this._lblListDescription = new System.Windows.Forms.Label();
         this._gbIncludeExclude = new System.Windows.Forms.GroupBox();
         this._cbNulls = new System.Windows.Forms.CheckBox();
         this._cbBlanks = new System.Windows.Forms.CheckBox();
         this._gbIncludeExclude.SuspendLayout();
         this.SuspendLayout();
         // 
         // _btnAdd
         // 
         this._btnAdd.Enabled = false;
         this._btnAdd.Location = new System.Drawing.Point(304, 112);
         this._btnAdd.Name = "_btnAdd";
         this._btnAdd.Size = new System.Drawing.Size(75, 20);
         this._btnAdd.TabIndex = 2;
         this._btnAdd.Text = "&Add";
         this._btnAdd.Click += new System.EventHandler(this.Click_btnAdd);
         // 
         // _btnRemove
         // 
         this._btnRemove.Enabled = false;
         this._btnRemove.Location = new System.Drawing.Point(304, 168);
         this._btnRemove.Name = "_btnRemove";
         this._btnRemove.Size = new System.Drawing.Size(75, 20);
         this._btnRemove.TabIndex = 5;
         this._btnRemove.Text = "&Remove";
         this._btnRemove.Click += new System.EventHandler(this.Click_btnRemove);
         // 
         // _btnCancel
         // 
         this._btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this._btnCancel.Location = new System.Drawing.Point(303, 312);
         this._btnCancel.Name = "_btnCancel";
         this._btnCancel.Size = new System.Drawing.Size(75, 23);
         this._btnCancel.TabIndex = 9;
         this._btnCancel.Text = "&Cancel";
         this._btnCancel.Click += new System.EventHandler(this.Click_btnCancel);
         // 
         // _btnOk
         // 
         this._btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
         this._btnOk.Location = new System.Drawing.Point(223, 312);
         this._btnOk.Name = "_btnOk";
         this._btnOk.Size = new System.Drawing.Size(75, 23);
         this._btnOk.TabIndex = 8;
         this._btnOk.Text = "&OK";
         this._btnOk.Click += new System.EventHandler(this.Click_btnOK);
         // 
         // _lblDirections
         // 
         this._lblDirections.Location = new System.Drawing.Point(8, 92);
         this._lblDirections.Name = "_lblDirections";
         this._lblDirections.Size = new System.Drawing.Size(320, 16);
         this._lblDirections.TabIndex = 0;
         this._lblDirections.Text = "&Specify a word or phrase to search for in the sender\'s address:";
         this._lblDirections.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // _tbStringEntry
         // 
         this._tbStringEntry.Location = new System.Drawing.Point(8, 112);
         this._tbStringEntry.Name = "_tbStringEntry";
         this._tbStringEntry.Size = new System.Drawing.Size(288, 20);
         this._tbStringEntry.TabIndex = 1;
         this._tbStringEntry.TextChanged += new System.EventHandler(this.TextChanged_tbStringEntry);
         // 
         // _listBoxStringList
         // 
         this._listBoxStringList.Location = new System.Drawing.Point(8, 168);
         this._listBoxStringList.Name = "_listBoxStringList";
         this._listBoxStringList.Size = new System.Drawing.Size(288, 95);
         this._listBoxStringList.TabIndex = 4;
         this._listBoxStringList.SelectedIndexChanged += new System.EventHandler(this.SelectedIndexChanged_listBoxStringList);
         // 
         // _rbInclude
         // 
         this._rbInclude.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
         this._rbInclude.Checked = true;
         this._rbInclude.Location = new System.Drawing.Point(8, 20);
         this._rbInclude.Name = "_rbInclude";
         this._rbInclude.Size = new System.Drawing.Size(256, 20);
         this._rbInclude.TabIndex = 6;
         this._rbInclude.TabStop = true;
         this._rbInclude.Text = "&Inclusive - match any strings in the list";
         this._rbInclude.TextAlign = System.Drawing.ContentAlignment.TopLeft;
         // 
         // _rbExclude
         // 
         this._rbExclude.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
         this._rbExclude.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
         this._rbExclude.Location = new System.Drawing.Point(8, 44);
         this._rbExclude.Name = "_rbExclude";
         this._rbExclude.Size = new System.Drawing.Size(256, 20);
         this._rbExclude.TabIndex = 7;
         this._rbExclude.Text = "&Exclusive - match all strings not in the list";
         this._rbExclude.TextAlign = System.Drawing.ContentAlignment.TopLeft;
         // 
         // _lblListDescription
         // 
         this._lblListDescription.Location = new System.Drawing.Point(8, 148);
         this._lblListDescription.Name = "_lblListDescription";
         this._lblListDescription.Size = new System.Drawing.Size(288, 16);
         this._lblListDescription.TabIndex = 3;
         this._lblListDescription.Text = "&List:";
         this._lblListDescription.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // _gbIncludeExclude
         // 
         this._gbIncludeExclude.Controls.Add(this._rbExclude);
         this._gbIncludeExclude.Controls.Add(this._rbInclude);
         this._gbIncludeExclude.Location = new System.Drawing.Point(8, 8);
         this._gbIncludeExclude.Name = "_gbIncludeExclude";
         this._gbIncludeExclude.Size = new System.Drawing.Size(288, 68);
         this._gbIncludeExclude.TabIndex = 10;
         this._gbIncludeExclude.TabStop = false;
         this._gbIncludeExclude.Text = "groupBox1";
         // 
         // _cbNulls
         // 
         this._cbNulls.AutoSize = true;
         this._cbNulls.Location = new System.Drawing.Point(11, 269);
         this._cbNulls.Name = "_cbNulls";
         this._cbNulls.Size = new System.Drawing.Size(168, 17);
         this._cbNulls.TabIndex = 14;
         this._cbNulls.Text = "Match Null Application Names";
         this._cbNulls.UseVisualStyleBackColor = true;
         this._cbNulls.Visible = false;
         // 
         // _cbBlanks
         // 
         this._cbBlanks.AutoSize = true;
         this._cbBlanks.Location = new System.Drawing.Point(11, 288);
         this._cbBlanks.Name = "_cbBlanks";
         this._cbBlanks.Size = new System.Drawing.Size(221, 17);
         this._cbBlanks.TabIndex = 15;
         this._cbBlanks.Text = "Match Empty or Blank Application Names";
         this._cbBlanks.UseVisualStyleBackColor = true;
         this._cbBlanks.Visible = false;
         // 
         // Form_StringSearch
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.CancelButton = this._btnCancel;
         this.ClientSize = new System.Drawing.Size(386, 345);
         this.Controls.Add(this._cbBlanks);
         this.Controls.Add(this._cbNulls);
         this.Controls.Add(this._gbIncludeExclude);
         this.Controls.Add(this._lblListDescription);
         this.Controls.Add(this._tbStringEntry);
         this.Controls.Add(this._listBoxStringList);
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
         this.Name = "Form_StringSearch";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Search Text";
         this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.Form_StringSearch_HelpRequested);
         this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyDown_Form_StringSearch);
         this._gbIncludeExclude.ResumeLayout(false);
         this.ResumeLayout(false);
         this.PerformLayout();

      }
      #endregion

      private System.Windows.Forms.Button _btnAdd;
      private System.Windows.Forms.Button _btnRemove;
      private System.Windows.Forms.Button _btnCancel;
      private System.Windows.Forms.Button _btnOk;
      private System.Windows.Forms.Label _lblDirections;
      private System.Windows.Forms.TextBox _tbStringEntry;
      private System.Windows.Forms.ListBox _listBoxStringList;
      private System.Windows.Forms.RadioButton _rbInclude;
      private System.Windows.Forms.RadioButton _rbExclude;
      private System.Windows.Forms.Label _lblListDescription;
      private System.Windows.Forms.GroupBox _gbIncludeExclude;
      private System.Windows.Forms.CheckBox _cbNulls;
      private System.Windows.Forms.CheckBox _cbBlanks;
   }
}