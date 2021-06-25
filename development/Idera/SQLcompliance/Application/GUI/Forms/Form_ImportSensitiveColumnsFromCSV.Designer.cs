using Idera.SQLcompliance.Application.GUI.Helper;
using Idera.SQLcompliance.Core;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
    partial class Form_ImportSensitiveColumnsFromCSV
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_ImportSensitiveColumnsFromCSV));
            this.label1 = new System.Windows.Forms.Label();
            this.txtCSVFile = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.SCTreeView = new System.Windows.Forms.Label();
            this.treeSensitiveColumns = new System.Windows.Forms.TreeView();
            this.label2 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 146);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "CSV File:";
            // 
            // txtCSVFile
            // 
            this.txtCSVFile.Location = new System.Drawing.Point(68, 143);
            this.txtCSVFile.Name = "txtCSVFile";
            this.txtCSVFile.Size = new System.Drawing.Size(371, 20);
            this.txtCSVFile.TabIndex = 1;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(445, 141);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 0;
            this.btnBrowse.Text = "&Browse...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // SCTreeView
            // 
            this.SCTreeView.Location = new System.Drawing.Point(13, 177);
            this.SCTreeView.Name = "SCTreeView";
            this.SCTreeView.Size = new System.Drawing.Size(507, 43);
            this.SCTreeView.TabIndex = 3;
            this.SCTreeView.Text = resources.GetString("SCTreeView.Text");
            // 
            // treeSensitiveColumns
            // 
            this.treeSensitiveColumns.CheckBoxes = true;
            this.treeSensitiveColumns.Location = new System.Drawing.Point(12, 223);
            this.treeSensitiveColumns.Name = "treeSensitiveColumns";
            this.treeSensitiveColumns.Size = new System.Drawing.Size(508, 245);
            this.treeSensitiveColumns.TabIndex = 2;
            this.treeSensitiveColumns.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.sscTreeViewFromCSV_AfterCheck);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(508, 129);
            this.label2.TabIndex = 5;
            this.label2.Text = resources.GetString("label2.Text");
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Enabled = false;
            this.btnOK.Location = new System.Drawing.Point(364, 474);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "&OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(445, 474);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // Form_ImportSensitiveColumnsFromCSV
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(532, 506);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.treeSensitiveColumns);
            this.Controls.Add(this.SCTreeView);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.txtCSVFile);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form_ImportSensitiveColumnsFromCSV";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Import Sensitive Columns from CSV";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtCSVFile;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Label SCTreeView;
        private System.Windows.Forms.TreeView treeSensitiveColumns;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private DatabaseObjectRecord dor;
    }
}