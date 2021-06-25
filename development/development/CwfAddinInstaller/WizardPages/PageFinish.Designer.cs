namespace CwfAddinInstaller.WizardPages
{
    partial class PageFinish
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
            this.chkLaunchmainApplication = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(3, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(307, 31);
            this.label1.TabIndex = 0;
            this.label1.Text = "Click Finish button to complete the IDERA SQL Compliance Manager && Dashboard Set" + "up";
            // 
            // chkLaunchmainApplication
            // 
            this.chkLaunchmainApplication.AutoSize = true;
            this.chkLaunchmainApplication.Checked = true;
            this.chkLaunchmainApplication.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkLaunchmainApplication.Location = new System.Drawing.Point(7, 66);
            this.chkLaunchmainApplication.Name = "chkLaunchmainApplication";
            this.chkLaunchmainApplication.Size = new System.Drawing.Size(208, 17);
            this.chkLaunchmainApplication.TabIndex = 0;
            this.chkLaunchmainApplication.Text = "Launch SQLCM Management Console";
            this.chkLaunchmainApplication.UseVisualStyleBackColor = true;
            this.chkLaunchmainApplication.Visible = false;
            // 
            // PageFinish
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkLaunchmainApplication);
            this.Controls.Add(this.label1);
            this.Name = "PageFinish";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkLaunchmainApplication;
    }
}
