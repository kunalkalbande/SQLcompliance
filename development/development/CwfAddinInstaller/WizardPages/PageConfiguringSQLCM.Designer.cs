namespace CwfAddinInstaller.WizardPages
{
    partial class PageConfiguringSQLCM
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
            this.SQLCMInstallationText = new System.Windows.Forms.Label();
            this.progressBarSQLCM = new System.Windows.Forms.ProgressBar();
            this.timerSQLCM = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // SQLCMInstallationText
            // 
            this.SQLCMInstallationText.AutoSize = true;
            this.SQLCMInstallationText.Location = new System.Drawing.Point(4, 54);
            this.SQLCMInstallationText.Name = "SQLCMInstallationText";
            this.SQLCMInstallationText.Size = new System.Drawing.Size(184, 13);
            this.SQLCMInstallationText.TabIndex = 1;
            this.SQLCMInstallationText.Text = "Installing SQL Compliance Manager...";
            // 
            // progressBarSQLCM
            // 
            this.progressBarSQLCM.Location = new System.Drawing.Point(7, 88);
            this.progressBarSQLCM.Name = "progressBarSQLCM";
            this.progressBarSQLCM.Size = new System.Drawing.Size(294, 23);
            this.progressBarSQLCM.TabIndex = 2;
            // 
            // timerSQLCM
            // 
            this.timerSQLCM.Tick += new System.EventHandler(this.timerSQLCM_Tick);
            // 
            // PageConfiguringSQLCM
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.progressBarSQLCM);
            this.Controls.Add(this.SQLCMInstallationText);
            this.Name = "PageConfiguringSQLCM";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label SQLCMInstallationText;
        private System.Windows.Forms.ProgressBar progressBarSQLCM;
        private System.Windows.Forms.Timer timerSQLCM;
    }
}
