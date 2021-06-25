namespace CwfAddinInstaller.WizardPages
{
    partial class PageConfiguringCWFDashboard
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
            this.components = new System.ComponentModel.Container();
            this.progressBarCWF = new System.Windows.Forms.ProgressBar();
            this.cwfInstallationTxt = new System.Windows.Forms.Label();
            this.timerCWF = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // progressBarCWF
            // 
            this.progressBarCWF.Location = new System.Drawing.Point(7, 88);
            this.progressBarCWF.Name = "progressBarCWF";
            this.progressBarCWF.Size = new System.Drawing.Size(294, 23);
            this.progressBarCWF.TabIndex = 5;
            // 
            // cwfInstallationTxt
            // 
            this.cwfInstallationTxt.AutoSize = true;
            this.cwfInstallationTxt.Location = new System.Drawing.Point(4, 54);
            this.cwfInstallationTxt.Name = "cwfInstallationTxt";
            this.cwfInstallationTxt.Size = new System.Drawing.Size(259, 13);
            this.cwfInstallationTxt.TabIndex = 4;
            this.cwfInstallationTxt.Text = "Configuring IDERA Dashboard Installer.  Please wait…";
            // 
            // timerCWF
            // 
            this.timerCWF.Tick += new System.EventHandler(this.timerCWF_Tick);
            // 
            // PageConfiguringCWFDashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.progressBarCWF);
            this.Controls.Add(this.cwfInstallationTxt);
            this.Name = "PageConfiguringCWFDashboard";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBarCWF;
        private System.Windows.Forms.Label cwfInstallationTxt;
        private System.Windows.Forms.Timer timerCWF;
    }
}
