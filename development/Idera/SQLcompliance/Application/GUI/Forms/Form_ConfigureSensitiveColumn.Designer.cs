namespace Idera.SQLcompliance.Application.GUI.Forms
{
    partial class Form_ConfigureSensitiveColumn
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

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
            System.Windows.Forms.Label label5;
            System.Windows.Forms.Label label4;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_ConfigureSensitiveColumn));
            System.Windows.Forms.ColumnHeader columnHeader3;
            this._pnlSensitiveColumns = new System.Windows.Forms.Panel();
            this._btnCancel = new System.Windows.Forms.Button();
            this._btnOK = new System.Windows.Forms.Button();
            this._btnAddSCDataSet = new System.Windows.Forms.Button();
            this._lvSCTables = new System.Windows.Forms.ListView();
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this._btnEditSCTable = new System.Windows.Forms.Button();
            this._btnAddSCTable = new System.Windows.Forms.Button();
            this._btnRemoveSCTable = new System.Windows.Forms.Button();
            //SQlCM-5747 v5.6
            this._btnConfigureSC = new System.Windows.Forms.Button();
            label5 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this._pnlSensitiveColumns.SuspendLayout();
            this.SuspendLayout();
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(7, 11);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(217, 13);
            label5.TabIndex = 13;
            label5.Text = "Tables audited for Sensitive Column Access:";
            // 
            // label4
            // 
            label4.Location = new System.Drawing.Point(7, 222);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(550, 164);
            label4.TabIndex = 18;
            label4.Text = resources.GetString("label4.Text");
            // 
            // columnHeader3
            // 
            columnHeader3.Text = "Table Name";
            columnHeader3.Width = 150;
            // 
            // _pnlSensitiveColumns
            // 
            this._pnlSensitiveColumns.Controls.Add(this._btnCancel);
            this._pnlSensitiveColumns.Controls.Add(this._btnOK);
            this._pnlSensitiveColumns.Controls.Add(this._btnAddSCDataSet);
            this._pnlSensitiveColumns.Controls.Add(label5);
            this._pnlSensitiveColumns.Controls.Add(label4);
            this._pnlSensitiveColumns.Controls.Add(this._lvSCTables);
            this._pnlSensitiveColumns.Controls.Add(this._btnEditSCTable);
            this._pnlSensitiveColumns.Controls.Add(this._btnAddSCTable);
            this._pnlSensitiveColumns.Controls.Add(this._btnRemoveSCTable);
            this._pnlSensitiveColumns.Controls.Add(this._btnConfigureSC); //SQlCM-5747 v5.6
            this._pnlSensitiveColumns.Dock = System.Windows.Forms.DockStyle.Fill;
            this._pnlSensitiveColumns.Location = new System.Drawing.Point(0, 0);
            this._pnlSensitiveColumns.Name = "_pnlSensitiveColumns";
            this._pnlSensitiveColumns.Size = new System.Drawing.Size(562, 430);
            this._pnlSensitiveColumns.TabIndex = 20;
            // 
            // _btnCancel
            // 
            this._btnCancel.Location = new System.Drawing.Point(477, 395);
            this._btnCancel.Name = "_btnCancel";
            this._btnCancel.Size = new System.Drawing.Size(75, 23);
            this._btnCancel.TabIndex = 42;
            this._btnCancel.Text = "&Cancel";
            this._btnCancel.UseVisualStyleBackColor = true;
            this._btnCancel.Click += new System.EventHandler(this._btnCancel_Click);
            // 
            // _btnOK
            // 
            this._btnOK.Location = new System.Drawing.Point(396, 396);
            this._btnOK.Name = "_btnOK";
            this._btnOK.Size = new System.Drawing.Size(75, 23);
            this._btnOK.TabIndex = 41;
            this._btnOK.Text = "&OK";
            this._btnOK.UseVisualStyleBackColor = true;
            this._btnOK.Click += new System.EventHandler(this._btnOK_Click);
            // 
            // _btnAddSCDataSet
            // 
            this._btnAddSCDataSet.Location = new System.Drawing.Point(455, 72);
            this._btnAddSCDataSet.Name = "_btnAddSCDataSet";
            this._btnAddSCDataSet.Size = new System.Drawing.Size(75, 23);
            this._btnAddSCDataSet.TabIndex = 19;
            this._btnAddSCDataSet.Text = "Add Dataset";
            this._btnAddSCDataSet.UseVisualStyleBackColor = true;
            this._btnAddSCDataSet.Click += new System.EventHandler(this._btnAddSCDataSet_Click);
            // 
            // _lvSCTables
            // 
            this._lvSCTables.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            columnHeader3,
            this.columnHeader5,
            this.columnHeader6});
            this._lvSCTables.FullRowSelect = true;
            this._lvSCTables.HideSelection = false;
            this._lvSCTables.Location = new System.Drawing.Point(7, 27);
            this._lvSCTables.Name = "_lvSCTables";
            this._lvSCTables.Size = new System.Drawing.Size(430, 179);
            this._lvSCTables.TabIndex = 14;
            this._lvSCTables.UseCompatibleStateImageBehavior = false;
            this._lvSCTables.View = System.Windows.Forms.View.Details;
            this._lvSCTables.SelectedIndexChanged += new System.EventHandler(this.SelectedIndexChanged_lvSCTables);
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Columns";
            this.columnHeader5.Width = 176;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Type";
            this.columnHeader6.Width = 139;
            // 
            // _btnEditSCTable
            // 
            this._btnEditSCTable.Location = new System.Drawing.Point(455, 130);
            this._btnEditSCTable.Name = "_btnEditSCTable";
            this._btnEditSCTable.Size = new System.Drawing.Size(75, 23);
            this._btnEditSCTable.TabIndex = 17;
            this._btnEditSCTable.Text = "Edit...";
            this._btnEditSCTable.UseVisualStyleBackColor = true;
            this._btnEditSCTable.Click += new System.EventHandler(this.Click_btnEditSCTable);
            // 
            // _btnAddSCTable
            // 
            this._btnAddSCTable.Location = new System.Drawing.Point(455, 43);
            this._btnAddSCTable.Name = "_btnAddSCTable";
            this._btnAddSCTable.Size = new System.Drawing.Size(75, 23);
            this._btnAddSCTable.TabIndex = 15;
            this._btnAddSCTable.Text = "Add Column";
            this._btnAddSCTable.UseVisualStyleBackColor = true;
            this._btnAddSCTable.Click += new System.EventHandler(this._btnAddSCTable_Click);
            // 
            // _btnRemoveSCTable
            // 
            this._btnRemoveSCTable.Location = new System.Drawing.Point(455, 101);
            this._btnRemoveSCTable.Name = "_btnRemoveSCTable";
            this._btnRemoveSCTable.Size = new System.Drawing.Size(75, 23);
            this._btnRemoveSCTable.TabIndex = 16;
            this._btnRemoveSCTable.Text = "Remove";
            this._btnRemoveSCTable.UseVisualStyleBackColor = true;
            this._btnRemoveSCTable.Click += new System.EventHandler(this.Click_btnRemoveSCTable);

            //SQlCM-5747 v5.6
            // 
            // _btnConfigureSC
            // 
            this._btnConfigureSC.Location = new System.Drawing.Point(455, 160);
            this._btnConfigureSC.Name = "_btnConfigureSC";
            this._btnConfigureSC.Size = new System.Drawing.Size(75, 23);
            this._btnConfigureSC.TabIndex = 16;
            this._btnConfigureSC.Text = "Configure";
            this._btnConfigureSC.UseVisualStyleBackColor = true;
            this._btnConfigureSC.Click += new System.EventHandler(this.Click_btnConfigureSC);
            //SQlCM-5747 v5.6 - END

            // 
            // Form_ConfigureSensitiveColumn
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(562, 430);
            this.Controls.Add(this._pnlSensitiveColumns);
            this.Name = "Form_ConfigureSensitiveColumn";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configure Sensitive Column";
            this._pnlSensitiveColumns.ResumeLayout(false);
            this._pnlSensitiveColumns.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel _pnlSensitiveColumns;
        private System.Windows.Forms.Button _btnAddSCDataSet;
        private System.Windows.Forms.ListView _lvSCTables;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.Button _btnEditSCTable;
        private System.Windows.Forms.Button _btnAddSCTable;
        private System.Windows.Forms.Button _btnRemoveSCTable;
        private System.Windows.Forms.Button _btnCancel;
        private System.Windows.Forms.Button _btnOK;
        private System.Windows.Forms.Button _btnConfigureSC;//SQlCM-5747 v5.6

    }
}