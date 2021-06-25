namespace CwfAddinInstaller.WizardPages
{
    partial class PageLicenseAgreement
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
            this.txtAgreement = new System.Windows.Forms.RichTextBox();
            this.chkIAccept = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // txtAgreement
            // 
            this.txtAgreement.BackColor = System.Drawing.Color.White;
            this.txtAgreement.Location = new System.Drawing.Point(3, 3);
            this.txtAgreement.Name = "txtAgreement";
            this.txtAgreement.ReadOnly = true;
            this.txtAgreement.Size = new System.Drawing.Size(308, 185);
            this.txtAgreement.TabIndex = 1;
            this.txtAgreement.TabStop = false;
            this.txtAgreement.Text = "";
            // 
            // chkIAccept
            // 
            this.chkIAccept.AutoSize = true;
            this.chkIAccept.Location = new System.Drawing.Point(3, 194);
            this.chkIAccept.Name = "chkIAccept";
            this.chkIAccept.Size = new System.Drawing.Size(234, 17);
            this.chkIAccept.TabIndex = 0;
            this.chkIAccept.Text = "I accept the terms in the License Agreement";
            this.chkIAccept.UseVisualStyleBackColor = true;
            this.chkIAccept.CheckedChanged += new System.EventHandler(this.chkIAccept_CheckedChanged);
            // 
            // PageLicenseAgreement
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkIAccept);
            this.Controls.Add(this.txtAgreement);
            this.Name = "PageLicenseAgreement";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox txtAgreement;
        private System.Windows.Forms.CheckBox chkIAccept;
    }
}
