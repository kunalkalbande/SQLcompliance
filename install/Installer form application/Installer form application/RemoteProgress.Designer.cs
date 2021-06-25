namespace Installer_form_application
{
    partial class RemoteProgress
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RemoteProgress));
            this.progressBarRemote = new System.Windows.Forms.ProgressBar();
            this.labelHeading = new System.Windows.Forms.Label();
            this.buttonBack = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonNext = new System.Windows.Forms.Button();
            this.labelStatus = new System.Windows.Forms.Label();
            this.labelProgress = new System.Windows.Forms.Label();
            this.backgroundWorkerRemote = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // progressBarRemote
            // 
            this.progressBarRemote.Location = new System.Drawing.Point(190, 175);
            this.progressBarRemote.Name = "progressBarRemote";
            this.progressBarRemote.Size = new System.Drawing.Size(382, 23);
            this.progressBarRemote.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBarRemote.TabIndex = 0;
            // 
            // labelHeading
            // 
            this.labelHeading.AutoSize = true;
            this.labelHeading.BackColor = System.Drawing.Color.Transparent;
            this.labelHeading.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.labelHeading.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelHeading.Location = new System.Drawing.Point(186, 25);
            this.labelHeading.Name = "labelHeading";
            this.labelHeading.Size = new System.Drawing.Size(287, 21);
            this.labelHeading.TabIndex = 47;
            this.labelHeading.Text = "Idera SQL Compliance Manager Setup";
            // 
            // buttonBack
            // 
            this.buttonBack.Enabled = false;
            this.buttonBack.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.buttonBack.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonBack.Location = new System.Drawing.Point(333, 408);
            this.buttonBack.Name = "buttonBack";
            this.buttonBack.Size = new System.Drawing.Size(72, 22);
            this.buttonBack.TabIndex = 50;
            this.buttonBack.Text = "Back\r\n";
            this.buttonBack.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Enabled = false;
            this.buttonCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.buttonCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonCancel.Location = new System.Drawing.Point(500, 408);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(72, 22);
            this.buttonCancel.TabIndex = 49;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonNext
            // 
            this.buttonNext.Enabled = false;
            this.buttonNext.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.buttonNext.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonNext.Location = new System.Drawing.Point(408, 408);
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.Size = new System.Drawing.Size(72, 22);
            this.buttonNext.TabIndex = 48;
            this.buttonNext.Text = "Next";
            this.buttonNext.UseVisualStyleBackColor = true;
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.BackColor = System.Drawing.Color.Transparent;
            this.labelStatus.Location = new System.Drawing.Point(187, 146);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(40, 13);
            this.labelStatus.TabIndex = 51;
            this.labelStatus.Text = "Status:";
            // 
            // labelProgress
            // 
            this.labelProgress.BackColor = System.Drawing.Color.Transparent;
            this.labelProgress.Location = new System.Drawing.Point(233, 146);
            this.labelProgress.Name = "labelProgress";
            this.labelProgress.Size = new System.Drawing.Size(300, 13);
            this.labelProgress.TabIndex = 52;
            // 
            // backgroundWorkerRemote
            // 
            this.backgroundWorkerRemote.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerRemote_DoWork);
            this.backgroundWorkerRemote.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorkerRemote_ProgressChanged);
            this.backgroundWorkerRemote.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerRemote_RunWorkerCompleted);
            // 
            // RemoteProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::Installer_form_application.Properties.Resources.Main_Background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(584, 442);
            this.Controls.Add(this.labelProgress);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.buttonBack);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonNext);
            this.Controls.Add(this.labelHeading);
            this.Controls.Add(this.progressBarRemote);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RemoteProgress";
            this.Text = "Idera SQL Compliance Manager Setup";
            this.Shown += new System.EventHandler(this.RemoteProgress_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBarRemote;
        private System.Windows.Forms.Label labelHeading;
        private System.Windows.Forms.Button buttonBack;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonNext;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Label labelProgress;
        private System.ComponentModel.BackgroundWorker backgroundWorkerRemote;
    }
}