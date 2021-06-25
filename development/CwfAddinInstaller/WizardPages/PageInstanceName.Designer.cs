namespace CwfAddinInstaller.WizardPages
{
    partial class PageInstanceName
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cboInstances = new System.Windows.Forms.ComboBox();
            this.chkUpgradeAll = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Enter the Display Name";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(3, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(308, 43);
            this.label2.TabIndex = 2;
            this.label2.Text = "NOTE: Display Name for Product will be displayed as identifier for SQLCM if multi" +
    "ple instances of Product are installed in Dashboard.";
            // 
            // cboInstances
            // 
            this.cboInstances.FormattingEnabled = true;
            this.cboInstances.Location = new System.Drawing.Point(3, 20);
            this.cboInstances.Name = "cboInstances";
            this.cboInstances.Size = new System.Drawing.Size(308, 21);
            this.cboInstances.TabIndex = 0;
            // 
            // chkUpgradeAll
            // 
            this.chkUpgradeAll.AutoSize = true;
            this.chkUpgradeAll.Location = new System.Drawing.Point(3, 109);
            this.chkUpgradeAll.Name = "chkUpgradeAll";
            this.chkUpgradeAll.Size = new System.Drawing.Size(209, 17);
            this.chkUpgradeAll.TabIndex = 1;
            this.chkUpgradeAll.Text = "Upgrade All Existing Product Instances";
            this.chkUpgradeAll.UseVisualStyleBackColor = true;
            this.chkUpgradeAll.Visible = false;
            // 
            // PageInstanceName
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkUpgradeAll);
            this.Controls.Add(this.cboInstances);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "PageInstanceName";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cboInstances;
        private System.Windows.Forms.CheckBox chkUpgradeAll;
    }
}
