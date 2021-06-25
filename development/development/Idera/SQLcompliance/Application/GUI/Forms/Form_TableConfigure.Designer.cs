namespace Idera.SQLcompliance.Application.GUI.Forms
{
   partial class Form_TableConfigure
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
         this._seperator = new System.Windows.Forms.Label();
         this._lblDescription = new System.Windows.Forms.Label();
         this._lblHeader = new System.Windows.Forms.Label();
         this.label2 = new System.Windows.Forms.Label();
         this.label1 = new System.Windows.Forms.Label();
         this.btnRemove = new System.Windows.Forms.Button();
         this.btnAdd = new System.Windows.Forms.Button();
         this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
         this.listAvailable = new System.Windows.Forms.ListView();
         this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
         this.btnCancel = new System.Windows.Forms.Button();
         this.listSelected = new System.Windows.Forms.ListView();
         this.btnOK = new System.Windows.Forms.Button();
         this._comboMaxRows = new System.Windows.Forms.ComboBox();
         this._radioColumnsAll = new System.Windows.Forms.RadioButton();
         this._radioColumnsSelect = new System.Windows.Forms.RadioButton();
         this.panel1 = new System.Windows.Forms.Panel();
         _lblRows = new System.Windows.Forms.Label();
         this.panel1.SuspendLayout();
         this.SuspendLayout();
         // 
         // _lblRows
         // 
         _lblRows.Location = new System.Drawing.Point(8, 71);
         _lblRows.Name = "_lblRows";
         _lblRows.Size = new System.Drawing.Size(359, 20);
         _lblRows.TabIndex = 25;
         _lblRows.Text = "Select how many rows of change data to capture per DML transaction:";
         // 
         // _seperator
         // 
         this._seperator.BackColor = System.Drawing.SystemColors.ActiveCaption;
         this._seperator.Location = new System.Drawing.Point(0, 59);
         this._seperator.Margin = new System.Windows.Forms.Padding(0);
         this._seperator.Name = "_seperator";
         this._seperator.Size = new System.Drawing.Size(507, 2);
         this._seperator.TabIndex = 24;
         // 
         // _lblDescription
         // 
         this._lblDescription.Location = new System.Drawing.Point(12, 29);
         this._lblDescription.Name = "_lblDescription";
         this._lblDescription.Size = new System.Drawing.Size(464, 30);
         this._lblDescription.TabIndex = 23;
         this._lblDescription.Text = "label4";
         // 
         // _lblHeader
         // 
         this._lblHeader.AutoSize = true;
         this._lblHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this._lblHeader.Location = new System.Drawing.Point(12, 9);
         this._lblHeader.Name = "_lblHeader";
         this._lblHeader.Size = new System.Drawing.Size(249, 13);
         this._lblHeader.TabIndex = 22;
         this._lblHeader.Text = "Select maximum rows and columns to audit";
         // 
         // label2
         // 
         this.label2.Location = new System.Drawing.Point(284, 2);
         this.label2.Name = "label2";
         this.label2.Size = new System.Drawing.Size(100, 16);
         this.label2.TabIndex = 18;
         this.label2.Text = "&Selected Columns:";
         // 
         // label1
         // 
         this.label1.Location = new System.Drawing.Point(16, 2);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(104, 16);
         this.label1.TabIndex = 14;
         this.label1.Text = "A&vailable Columns:";
         // 
         // btnRemove
         // 
         this.btnRemove.Enabled = false;
         this.btnRemove.Location = new System.Drawing.Point(204, 82);
         this.btnRemove.Name = "btnRemove";
         this.btnRemove.Size = new System.Drawing.Size(72, 23);
         this.btnRemove.TabIndex = 17;
         this.btnRemove.Text = "<- &Remove";
         this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
         // 
         // btnAdd
         // 
         this.btnAdd.Enabled = false;
         this.btnAdd.Location = new System.Drawing.Point(204, 54);
         this.btnAdd.Name = "btnAdd";
         this.btnAdd.Size = new System.Drawing.Size(72, 23);
         this.btnAdd.TabIndex = 16;
         this.btnAdd.Text = "&Add ->";
         this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
         // 
         // columnHeader2
         // 
         this.columnHeader2.Width = 155;
         // 
         // listAvailable
         // 
         this.listAvailable.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
         this.listAvailable.FullRowSelect = true;
         this.listAvailable.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
         this.listAvailable.HideSelection = false;
         this.listAvailable.Location = new System.Drawing.Point(16, 18);
         this.listAvailable.Name = "listAvailable";
         this.listAvailable.ShowItemToolTips = true;
         this.listAvailable.Size = new System.Drawing.Size(180, 240);
         this.listAvailable.Sorting = System.Windows.Forms.SortOrder.Ascending;
         this.listAvailable.TabIndex = 15;
         this.listAvailable.UseCompatibleStateImageBehavior = false;
         this.listAvailable.View = System.Windows.Forms.View.Details;
         this.listAvailable.SelectedIndexChanged += new System.EventHandler(this.listAvailable_SelectedIndexChanged);
         this.listAvailable.DoubleClick += new System.EventHandler(this.listAvailable_DoubleClick);
         // 
         // columnHeader1
         // 
         this.columnHeader1.Width = 155;
         // 
         // btnCancel
         // 
         this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.btnCancel.Location = new System.Drawing.Point(406, 429);
         this.btnCancel.Name = "btnCancel";
         this.btnCancel.Size = new System.Drawing.Size(75, 23);
         this.btnCancel.TabIndex = 21;
         this.btnCancel.Text = "&Cancel";
         this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
         // 
         // listSelected
         // 
         this.listSelected.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
         this.listSelected.FullRowSelect = true;
         this.listSelected.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
         this.listSelected.HideSelection = false;
         this.listSelected.Location = new System.Drawing.Point(284, 18);
         this.listSelected.Name = "listSelected";
         this.listSelected.ShowItemToolTips = true;
         this.listSelected.Size = new System.Drawing.Size(180, 240);
         this.listSelected.Sorting = System.Windows.Forms.SortOrder.Ascending;
         this.listSelected.TabIndex = 19;
         this.listSelected.UseCompatibleStateImageBehavior = false;
         this.listSelected.View = System.Windows.Forms.View.Details;
         this.listSelected.SelectedIndexChanged += new System.EventHandler(this.listSelected_SelectedIndexChanged);
         this.listSelected.DoubleClick += new System.EventHandler(this.listSelected_DoubleClick);
         // 
         // btnOK
         // 
         this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.btnOK.Location = new System.Drawing.Point(326, 429);
         this.btnOK.Name = "btnOK";
         this.btnOK.Size = new System.Drawing.Size(75, 23);
         this.btnOK.TabIndex = 20;
         this.btnOK.Text = "&OK";
         this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
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
         this._comboMaxRows.Location = new System.Drawing.Point(11, 87);
         this._comboMaxRows.Name = "_comboMaxRows";
         this._comboMaxRows.Size = new System.Drawing.Size(187, 21);
         this._comboMaxRows.TabIndex = 26;
         // 
         // _radioColumnsAll
         // 
         this._radioColumnsAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this._radioColumnsAll.AutoSize = true;
         this._radioColumnsAll.Checked = true;
         this._radioColumnsAll.Location = new System.Drawing.Point(11, 116);
         this._radioColumnsAll.Name = "_radioColumnsAll";
         this._radioColumnsAll.Size = new System.Drawing.Size(106, 17);
         this._radioColumnsAll.TabIndex = 27;
         this._radioColumnsAll.TabStop = true;
         this._radioColumnsAll.Text = "Audit All Columns";
         this._radioColumnsAll.UseVisualStyleBackColor = true;
         this._radioColumnsAll.CheckedChanged += new System.EventHandler(this._radioColumnsAll_CheckedChanged);
         // 
         // _radioColumnsSelect
         // 
         this._radioColumnsSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this._radioColumnsSelect.AutoSize = true;
         this._radioColumnsSelect.Location = new System.Drawing.Point(11, 139);
         this._radioColumnsSelect.Name = "_radioColumnsSelect";
         this._radioColumnsSelect.Size = new System.Drawing.Size(137, 17);
         this._radioColumnsSelect.TabIndex = 28;
         this._radioColumnsSelect.Text = "Audit Selected Columns";
         this._radioColumnsSelect.UseVisualStyleBackColor = true;
         this._radioColumnsSelect.CheckedChanged += new System.EventHandler(this._radioColumnsSelect_CheckedChanged);
         // 
         // panel1
         // 
         this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.panel1.Controls.Add(this.label1);
         this.panel1.Controls.Add(this.listSelected);
         this.panel1.Controls.Add(this.listAvailable);
         this.panel1.Controls.Add(this.btnAdd);
         this.panel1.Controls.Add(this.btnRemove);
         this.panel1.Controls.Add(this.label2);
         this.panel1.Enabled = false;
         this.panel1.Location = new System.Drawing.Point(12, 165);
         this.panel1.Name = "panel1";
         this.panel1.Size = new System.Drawing.Size(469, 261);
         this.panel1.TabIndex = 29;
         // 
         // Form_TableConfigure
         // 
         this.AcceptButton = this.btnOK;
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
         this.CancelButton = this.btnCancel;
         this.ClientSize = new System.Drawing.Size(488, 464);
         this.Controls.Add(this.panel1);
         this.Controls.Add(this._radioColumnsSelect);
         this.Controls.Add(this._radioColumnsAll);
         this.Controls.Add(this._comboMaxRows);
         this.Controls.Add(_lblRows);
         this.Controls.Add(this._seperator);
         this.Controls.Add(this._lblDescription);
         this.Controls.Add(this._lblHeader);
         this.Controls.Add(this.btnCancel);
         this.Controls.Add(this.btnOK);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.HelpButton = true;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "Form_TableConfigure";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Configure Table Auditing";
         this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.Form_TableAdd_HelpRequested);
         this.panel1.ResumeLayout(false);
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Label _seperator;
      private System.Windows.Forms.Label _lblDescription;
      private System.Windows.Forms.Label _lblHeader;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.Button btnRemove;
      private System.Windows.Forms.Button btnAdd;
      private System.Windows.Forms.ColumnHeader columnHeader2;
      private System.Windows.Forms.ListView listAvailable;
      private System.Windows.Forms.ColumnHeader columnHeader1;
      private System.Windows.Forms.Button btnCancel;
      private System.Windows.Forms.ListView listSelected;
      private System.Windows.Forms.Button btnOK;
      private System.Windows.Forms.ComboBox _comboMaxRows;
      private System.Windows.Forms.RadioButton _radioColumnsAll;
      private System.Windows.Forms.RadioButton _radioColumnsSelect;
      private System.Windows.Forms.Panel panel1;
      private System.Windows.Forms.Label _lblRows;
   }
}