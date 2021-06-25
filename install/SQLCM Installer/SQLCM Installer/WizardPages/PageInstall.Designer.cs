namespace SQLCM_Installer.WizardPages
{
    partial class PageInstall
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
            this.progressBarAll = new System.Windows.Forms.ProgressBar();
            this.labelInstallationText = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.labelHeader = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorkerProgress = new System.ComponentModel.BackgroundWorker();
            this.progressTracker = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // progressBarAll
            // 
            this.progressBarAll.Location = new System.Drawing.Point(20, 96);
            this.progressBarAll.Name = "progressBarAll";
            this.progressBarAll.Size = new System.Drawing.Size(410, 23);
            this.progressBarAll.TabIndex = 2;
            // 
            // labelInstallationText
            // 
            this.labelInstallationText.AutoSize = true;
            this.labelInstallationText.BackColor = System.Drawing.Color.Transparent;
            this.labelInstallationText.Disabled = false;
            this.labelInstallationText.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelInstallationText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelInstallationText.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelInstallationText.Location = new System.Drawing.Point(20, 133);
            this.labelInstallationText.Name = "labelInstallationText";
            this.labelInstallationText.Size = new System.Drawing.Size(245, 23);
            this.labelInstallationText.TabIndex = 1;
            this.labelInstallationText.Text = "Installing SQL Compliance Manager…";
            this.labelInstallationText.UseCompatibleTextRendering = true;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.panel1.Controls.Add(this.labelHeader);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(550, 64);
            this.panel1.TabIndex = 0;
            // 
            // labelHeader
            // 
            this.labelHeader.AutoSize = true;
            this.labelHeader.BackColor = System.Drawing.Color.Transparent;
            this.labelHeader.Disabled = false;
            this.labelHeader.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelHeader.Location = new System.Drawing.Point(20, 23);
            this.labelHeader.Name = "labelHeader";
            this.labelHeader.Size = new System.Drawing.Size(254, 18);
            this.labelHeader.TabIndex = 0;
            this.labelHeader.Text = "Installing SQL Compliance Manager and IDERA Dashboard";
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.WorkerReportsProgress = true;
            this.backgroundWorker.WorkerSupportsCancellation = true;
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
            this.backgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker_ProgressChanged);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
            // 
            // backgroundWorkerProgress
            // 
            this.backgroundWorkerProgress.WorkerReportsProgress = true;
            this.backgroundWorkerProgress.WorkerSupportsCancellation = true;
            this.backgroundWorkerProgress.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerProgress_DoWork);
            this.backgroundWorkerProgress.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorkerProgress_ProgressChanged);
            this.backgroundWorkerProgress.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerProgress_RunWorkerCompleted);
            // 
            // label1
            // 
            this.progressTracker.AutoSize = true;
            this.progressTracker.Location = new System.Drawing.Point(438, 98);
            this.progressTracker.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.progressTracker.Name = "progressTracker";
            this.progressTracker.Size = new System.Drawing.Size(50, 13);
            this.progressTracker.TabIndex = 3;
            this.progressTracker.Text = "";
            // 
            // PageInstall
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.progressTracker);
            this.Controls.Add(this.progressBarAll);
            this.Controls.Add(this.labelInstallationText);
            this.Controls.Add(this.panel1);
            this.Name = "PageInstall";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private Custom_Controls.IderaLabel labelHeader;
        private Custom_Controls.IderaLabel labelInstallationText;
        private System.Windows.Forms.ProgressBar progressBarAll;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.ComponentModel.BackgroundWorker backgroundWorkerProgress;
        private System.Windows.Forms.Label progressTracker;
    }
}
