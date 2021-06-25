namespace Idera.SQLcompliance.Application.GUI.Forms
{
    partial class Form_BeforeAfterRegulationGuidelines
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_BeforeAfterRegulationGuidelines));
            this._pnlBeforeAfter = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this._btnCancel = new System.Windows.Forms.Button();
            this._btnOK = new System.Windows.Forms.Button();
            this._gbCLR = new System.Windows.Forms.GroupBox();
            this._btnEnableCLR = new System.Windows.Forms.Button();
            this._lblClrStatus = new System.Windows.Forms.Label();
            this._pbClrStatus = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this._btnEditBATable = new System.Windows.Forms.Button();
            this._btnRemoveBATable = new System.Windows.Forms.Button();
            this._btnAddBATable = new System.Windows.Forms.Button();
            this._lvBeforeAfterTables = new System.Windows.Forms.ListView();
            this._columnTableName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this._columnMaxRows = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this._columnColumnNames = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label2 = new System.Windows.Forms.Label();
            this._lblBeforeAfterStatus = new System.Windows.Forms.Label();
            this._pnlBeforeAfter.SuspendLayout();
            this._gbCLR.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._pbClrStatus)).BeginInit();
            this.SuspendLayout();
            // 
            // _pnlBeforeAfter
            // 
            this._pnlBeforeAfter.Controls.Add(this.label1);
            this._pnlBeforeAfter.Controls.Add(this.panel1);
            this._pnlBeforeAfter.Controls.Add(this._btnCancel);
            this._pnlBeforeAfter.Controls.Add(this._btnOK);
            this._pnlBeforeAfter.Controls.Add(this._gbCLR);
            this._pnlBeforeAfter.Controls.Add(this.label3);
            this._pnlBeforeAfter.Controls.Add(this._btnEditBATable);
            this._pnlBeforeAfter.Controls.Add(this._btnRemoveBATable);
            this._pnlBeforeAfter.Controls.Add(this._btnAddBATable);
            this._pnlBeforeAfter.Controls.Add(this._lvBeforeAfterTables);
            this._pnlBeforeAfter.Controls.Add(this.label2);
            this._pnlBeforeAfter.Dock = System.Windows.Forms.DockStyle.Fill;
            this._pnlBeforeAfter.Location = new System.Drawing.Point(0, 0);
            this._pnlBeforeAfter.Name = "_pnlBeforeAfter";
            this._pnlBeforeAfter.Size = new System.Drawing.Size(536, 447);
            this._pnlBeforeAfter.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 225);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(368, 13);
            this.label1.TabIndex = 42;
            this.label1.Text = "Note: If you do not select any columns, all columns will be audited by default. Auditing before-after data can \n result in a significant amount of data being collected. You should audit before-after data for tables only when \n it is necessary to have the before and after data for DML activity within the table.";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.Gray;
            this.panel1.Location = new System.Drawing.Point(13, 405);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(511, 1);
            this.panel1.TabIndex = 41;
            // 
            // _btnCancel
            // 
            this._btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._btnCancel.Location = new System.Drawing.Point(438, 416);
            this._btnCancel.Name = "_btnCancel";
            this._btnCancel.Size = new System.Drawing.Size(75, 23);
            this._btnCancel.TabIndex = 40;
            this._btnCancel.Text = "&Cancel";
            this._btnCancel.Click += new System.EventHandler(this._btnCancel_Click);
            // 
            // _btnOK
            // 
            this._btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._btnOK.Location = new System.Drawing.Point(358, 416);
            this._btnOK.Name = "_btnOK";
            this._btnOK.Size = new System.Drawing.Size(75, 23);
            this._btnOK.TabIndex = 39;
            this._btnOK.Text = "&OK";
            this._btnOK.Click += new System.EventHandler(this._btnOK_Click);
            // 
            // _gbCLR
            // 
            this._gbCLR.Controls.Add(this._btnEnableCLR);
            this._gbCLR.Controls.Add(this._lblClrStatus);
            this._gbCLR.Controls.Add(this._pbClrStatus);
            this._gbCLR.Location = new System.Drawing.Point(8, 291);
            this._gbCLR.Name = "_gbCLR";
            this._gbCLR.Size = new System.Drawing.Size(427, 103);
            this._gbCLR.TabIndex = 13;
            this._gbCLR.TabStop = false;
            this._gbCLR.Text = "CLR Status";
            // 
            // _btnEnableCLR
            // 
            this._btnEnableCLR.Location = new System.Drawing.Point(71, 62);
            this._btnEnableCLR.Name = "_btnEnableCLR";
            this._btnEnableCLR.Size = new System.Drawing.Size(75, 23);
            this._btnEnableCLR.TabIndex = 2;
            this._btnEnableCLR.Text = "Enable Now";
            this._btnEnableCLR.UseVisualStyleBackColor = true;
            this._btnEnableCLR.Click += new System.EventHandler(this._btnEnableCLR_Click);
            // 
            // _lblClrStatus
            // 
            this._lblClrStatus.Location = new System.Drawing.Point(68, 30);
            this._lblClrStatus.Name = "_lblClrStatus";
            this._lblClrStatus.Size = new System.Drawing.Size(279, 29);
            this._lblClrStatus.TabIndex = 1;
            this._lblClrStatus.Text = "label4";
            // 
            // _pbClrStatus
            // 
            this._pbClrStatus.Location = new System.Drawing.Point(6, 19);
            this._pbClrStatus.Name = "_pbClrStatus";
            this._pbClrStatus.Size = new System.Drawing.Size(48, 48);
            this._pbClrStatus.TabIndex = 0;
            this._pbClrStatus.TabStop = false;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(8, 225);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(427, 63);
            this.label3.TabIndex = 12;
            // 
            // _btnEditBATable
            // 
            this._btnEditBATable.Location = new System.Drawing.Point(444, 106);
            this._btnEditBATable.Name = "_btnEditBATable";
            this._btnEditBATable.Size = new System.Drawing.Size(75, 23);
            this._btnEditBATable.TabIndex = 11;
            this._btnEditBATable.Text = "Edit...";
            this._btnEditBATable.UseVisualStyleBackColor = true;
            this._btnEditBATable.Click += new System.EventHandler(this._btnEditBATable_Click);
            // 
            // _btnRemoveBATable
            // 
            this._btnRemoveBATable.Location = new System.Drawing.Point(444, 77);
            this._btnRemoveBATable.Name = "_btnRemoveBATable";
            this._btnRemoveBATable.Size = new System.Drawing.Size(75, 23);
            this._btnRemoveBATable.TabIndex = 10;
            this._btnRemoveBATable.Text = "Remove";
            this._btnRemoveBATable.UseVisualStyleBackColor = true;
            this._btnRemoveBATable.Click += new System.EventHandler(this._btnRemoveBATable_Click);
            // 
            // _btnAddBATable
            // 
            this._btnAddBATable.Location = new System.Drawing.Point(444, 48);
            this._btnAddBATable.Name = "_btnAddBATable";
            this._btnAddBATable.Size = new System.Drawing.Size(75, 23);
            this._btnAddBATable.TabIndex = 9;
            this._btnAddBATable.Text = "Add...";
            this._btnAddBATable.UseVisualStyleBackColor = true;
            this._btnAddBATable.Click += new System.EventHandler(this._btnAddBATable_Click);
            // 
            // _lvBeforeAfterTables
            // 
            this._lvBeforeAfterTables.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this._columnTableName,
            this._columnMaxRows,
            this._columnColumnNames});
            this._lvBeforeAfterTables.FullRowSelect = true;
            this._lvBeforeAfterTables.HideSelection = false;
            this._lvBeforeAfterTables.Location = new System.Drawing.Point(9, 30);
            this._lvBeforeAfterTables.Name = "_lvBeforeAfterTables";
            this._lvBeforeAfterTables.Size = new System.Drawing.Size(430, 179);
            this._lvBeforeAfterTables.TabIndex = 8;
            this._lvBeforeAfterTables.UseCompatibleStateImageBehavior = false;
            this._lvBeforeAfterTables.View = System.Windows.Forms.View.Details;
            this._lvBeforeAfterTables.SelectedIndexChanged += new System.EventHandler(this._lvBeforeAfterTables_SelectedIndexChanged);
            // 
            // _columnTableName
            // 
            this._columnTableName.Text = "Table Name";
            this._columnTableName.Width = 169;
            // 
            // _columnMaxRows
            // 
            this._columnMaxRows.Text = "Maximum Rows";
            this._columnMaxRows.Width = 94;
            // 
            // _columnColumnNames
            // 
            this._columnColumnNames.Text = "Columns";
            this._columnColumnNames.Width = 156;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(204, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Tables audited for DML Before-After data:";
            // 
            // _lblBeforeAfterStatus
            // 
            this._lblBeforeAfterStatus.Location = new System.Drawing.Point(8, 14);
            this._lblBeforeAfterStatus.Name = "_lblBeforeAfterStatus";
            this._lblBeforeAfterStatus.Size = new System.Drawing.Size(447, 113);
            this._lblBeforeAfterStatus.TabIndex = 1;
            this._lblBeforeAfterStatus.Text = "label4";
            // 
            // Form_BeforeAfterRegulationGuidelines
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(536, 447);
            this.Controls.Add(this._pnlBeforeAfter);
            this.Controls.Add(this._lblBeforeAfterStatus);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form_BeforeAfterRegulationGuidelines";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configure Before After Data Change";
            this._pnlBeforeAfter.ResumeLayout(false);
            this._pnlBeforeAfter.PerformLayout();
            this._gbCLR.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._pbClrStatus)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel _pnlBeforeAfter;
        private System.Windows.Forms.GroupBox _gbCLR;
        private System.Windows.Forms.Button _btnEnableCLR;
        private System.Windows.Forms.Label _lblClrStatus;
        private System.Windows.Forms.PictureBox _pbClrStatus;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button _btnEditBATable;
        private System.Windows.Forms.Button _btnRemoveBATable;
        private System.Windows.Forms.Button _btnAddBATable;
        private System.Windows.Forms.ListView _lvBeforeAfterTables;
        private System.Windows.Forms.Label _lblBeforeAfterStatus;
        private System.Windows.Forms.ColumnHeader _columnTableName;
        private System.Windows.Forms.ColumnHeader _columnMaxRows;
        private System.Windows.Forms.ColumnHeader _columnColumnNames;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button _btnCancel;
        private System.Windows.Forms.Button _btnOK;
        private System.Windows.Forms.Label label1;
    }
}