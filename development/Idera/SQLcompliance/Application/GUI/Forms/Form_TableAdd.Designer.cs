using System;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
   partial class Form_TableAdd
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
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.listAvailable = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listSelected = new System.Windows.Forms.ListView();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this._lblHeader = new System.Windows.Forms.Label();
            this._lblDescription = new System.Windows.Forms.Label();
            this._seperator = new System.Windows.Forms.Label();
            this._lblBadDescription = new System.Windows.Forms.Label();
            this._lblInvalidCharDesc = new System.Windows.Forms.Label();
            this.btnFirst = new System.Windows.Forms.Button();
            this.btnPrevious = new System.Windows.Forms.Button();
            this.tbPageNo = new System.Windows.Forms.TextBox();
            this.btnLast = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.tbSearch = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(304, 479);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 9;
            this.btnOK.Text = "&OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(384, 479);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // listAvailable
            // 
            this.listAvailable.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listAvailable.FullRowSelect = true;
            this.listAvailable.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listAvailable.HideSelection = false;
            this.listAvailable.Location = new System.Drawing.Point(8, 118);
            this.listAvailable.Name = "listAvailable";
            this.listAvailable.ShowItemToolTips = true;
            this.listAvailable.Size = new System.Drawing.Size(180, 240);
            this.listAvailable.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listAvailable.TabIndex = 1;
            this.listAvailable.UseCompatibleStateImageBehavior = false;
            this.listAvailable.View = System.Windows.Forms.View.Details;
            this.listAvailable.SelectedIndexChanged += new System.EventHandler(this.listAvailable_SelectedIndexChanged);
            this.listAvailable.DoubleClick += new System.EventHandler(this.listAvailable_DoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 155;
            // 
            // listSelected
            // 
            this.listSelected.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
            this.listSelected.FullRowSelect = true;
            this.listSelected.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listSelected.HideSelection = false;
            this.listSelected.Location = new System.Drawing.Point(276, 118);
            this.listSelected.Name = "listSelected";
            this.listSelected.ShowItemToolTips = true;
            this.listSelected.Size = new System.Drawing.Size(180, 240);
            this.listSelected.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listSelected.TabIndex = 5;
            this.listSelected.UseCompatibleStateImageBehavior = false;
            this.listSelected.View = System.Windows.Forms.View.Details;
            this.listSelected.SelectedIndexChanged += new System.EventHandler(this.listSelected_SelectedIndexChanged);
            this.listSelected.DoubleClick += new System.EventHandler(this.listSelected_DoubleClick);
            // 
            // columnHeader2
            // 
            this.columnHeader2.Width = 155;
            // 
            // btnAdd
            // 
            this.btnAdd.Enabled = false;
            this.btnAdd.Location = new System.Drawing.Point(196, 154);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(72, 23);
            this.btnAdd.TabIndex = 2;
            this.btnAdd.Text = "&Add ->";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Enabled = false;
            this.btnRemove.Location = new System.Drawing.Point(196, 182);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(72, 23);
            this.btnRemove.TabIndex = 3;
            this.btnRemove.Text = "<- &Remove";
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 73);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "A&vailable Tables:";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(276, 73);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 16);
            this.label2.TabIndex = 4;
            this.label2.Text = "&Selected Tables:";
            // 
            // _lblHeader
            // 
            this._lblHeader.AutoSize = true;
            this._lblHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblHeader.Location = new System.Drawing.Point(12, 9);
            this._lblHeader.Name = "_lblHeader";
            this._lblHeader.Size = new System.Drawing.Size(75, 13);
            this._lblHeader.TabIndex = 11;
            this._lblHeader.Text = "Select table";
            // 
            // _lblDescription
            // 
            this._lblDescription.Location = new System.Drawing.Point(12, 29);
            this._lblDescription.Name = "_lblDescription";
            this._lblDescription.Size = new System.Drawing.Size(444, 30);
            this._lblDescription.TabIndex = 12;
            this._lblDescription.Text = "Specific user tables can be selected";
            // 
            // _seperator
            // 
            this._seperator.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this._seperator.Location = new System.Drawing.Point(0, 59);
            this._seperator.Margin = new System.Windows.Forms.Padding(0);
            this._seperator.Name = "_seperator";
            this._seperator.Size = new System.Drawing.Size(469, 2);
            this._seperator.TabIndex = 13;
            // 
            // _lblBadDescription
            // 
            this._lblBadDescription.Location = new System.Drawing.Point(6, 454);
            this._lblBadDescription.Name = "_lblBadDescription";
            this._lblBadDescription.Size = new System.Drawing.Size(454, 18);
            this._lblBadDescription.TabIndex = 14;
            this._lblBadDescription.Text = "Bold table names contain BLOB data and must use column selection for Before-After" +
    " auditing.";
            this._lblBadDescription.Visible = false;
            // 
            // _lblInvalidCharDesc
            // 
            this._lblInvalidCharDesc.Location = new System.Drawing.Point(5, 437);
            this._lblInvalidCharDesc.Name = "_lblInvalidCharDesc";
            this._lblInvalidCharDesc.Size = new System.Drawing.Size(444, 17);
            this._lblInvalidCharDesc.TabIndex = 15;
            this._lblInvalidCharDesc.Text = "Table names that contain \\ / : | \" < > or ? will not be included in the list of a" +
    "vailable tables.";
            // 
            // btnFirst
            // 
            this.btnFirst.BackgroundImage = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.PagingFirst;
            this.btnFirst.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnFirst.Location = new System.Drawing.Point(8, 365);
            this.btnFirst.Name = "btnFirst";
            this.btnFirst.Size = new System.Drawing.Size(28, 20);
            this.btnFirst.TabIndex = 16;
            this.btnFirst.UseVisualStyleBackColor = true;
            this.btnFirst.Click += new System.EventHandler(this.btnFirst_Click);
            // 
            // btnPrevious
            // 
            this.btnPrevious.BackgroundImage = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.PagingPrevious;
            this.btnPrevious.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnPrevious.Location = new System.Drawing.Point(40, 365);
            this.btnPrevious.Name = "btnPrevious";
            this.btnPrevious.Size = new System.Drawing.Size(28, 20);
            this.btnPrevious.TabIndex = 17;
            this.btnPrevious.UseVisualStyleBackColor = true;
            this.btnPrevious.Click += new System.EventHandler(this.btnPrevious_Click);
            // 
            // tbPageNo
            // 
            this.tbPageNo.BackColor = System.Drawing.Color.White;
            this.tbPageNo.Location = new System.Drawing.Point(73, 366);
            this.tbPageNo.Name = "tbPageNo";
            this.tbPageNo.ReadOnly = true;
            this.tbPageNo.Size = new System.Drawing.Size(52, 20);
            this.tbPageNo.TabIndex = 18;
            this.tbPageNo.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnLast
            // 
            this.btnLast.BackgroundImage = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.PagingLast;
            this.btnLast.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnLast.Location = new System.Drawing.Point(160, 365);
            this.btnLast.Name = "btnLast";
            this.btnLast.Size = new System.Drawing.Size(28, 20);
            this.btnLast.TabIndex = 20;
            this.btnLast.UseVisualStyleBackColor = true;
            this.btnLast.Click += new System.EventHandler(this.btnLast_Click);
            // 
            // btnNext
            // 
            this.btnNext.BackgroundImage = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.PagingNext;
            this.btnNext.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnNext.Location = new System.Drawing.Point(129, 365);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(28, 20);
            this.btnNext.TabIndex = 19;
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // tbSearch
            // 
            this.tbSearch.Location = new System.Drawing.Point(9, 92);
            this.tbSearch.Name = "tbSearch";
            this.tbSearch.Size = new System.Drawing.Size(148, 20);
            this.tbSearch.TabIndex = 21;
            // 
            // btnSearch
            // 
            this.btnSearch.BackgroundImage = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.Search;
            this.btnSearch.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnSearch.Location = new System.Drawing.Point(160, 91);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(28, 20);
            this.btnSearch.TabIndex = 22;
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 393);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(94, 13);
            this.label3.TabIndex = 23;
            this.label3.Text = "Records Per Page";
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(116, 391);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numericUpDown1.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(71, 20);
            this.numericUpDown1.TabIndex = 24;
            this.numericUpDown1.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            this.numericUpDown1.Leave += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // Form_TableAdd
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(466, 514);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.tbSearch);
            this.Controls.Add(this.btnLast);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.tbPageNo);
            this.Controls.Add(this.btnPrevious);
            this.Controls.Add(this.btnFirst);
            this.Controls.Add(this._lblInvalidCharDesc);
            this.Controls.Add(this._lblBadDescription);
            this.Controls.Add(this._seperator);
            this.Controls.Add(this._lblHeader);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.listSelected);
            this.Controls.Add(this.listAvailable);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this._lblDescription);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form_TableAdd";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add User Tables";
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.Form_TableAdd_HelpRequested);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.numericUpDown1_ValueChanged);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

      }
        
        #endregion

        private System.Windows.Forms.Button btnOK;
      private System.Windows.Forms.Button btnCancel;
      private System.Windows.Forms.ListView listAvailable;
      private System.Windows.Forms.ListView listSelected;
      private System.Windows.Forms.Button btnAdd;
      private System.Windows.Forms.Button btnRemove;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.ColumnHeader columnHeader1;
      private System.Windows.Forms.ColumnHeader columnHeader2;
      private System.Windows.Forms.Label _lblHeader;
      private System.Windows.Forms.Label _lblDescription;
      private System.Windows.Forms.Label _seperator;
      private System.Windows.Forms.Label _lblBadDescription;
      private System.Windows.Forms.Label _lblInvalidCharDesc;
      private System.Windows.Forms.Button btnFirst;
      private System.Windows.Forms.Button btnPrevious;
      private System.Windows.Forms.TextBox tbPageNo;
      private System.Windows.Forms.Button btnLast;
      private System.Windows.Forms.Button btnNext;
      private System.Windows.Forms.TextBox tbSearch;
      private System.Windows.Forms.Button btnSearch;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.NumericUpDown numericUpDown1;
    }
}