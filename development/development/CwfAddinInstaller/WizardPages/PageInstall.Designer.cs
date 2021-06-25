namespace CwfAddinInstaller.WizardPages
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
            this.cboInstances = new System.Windows.Forms.ComboBox();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.lblDisplayName = new System.Windows.Forms.Label();
            this.progInstall = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // cboInstances
            // 
            this.cboInstances.FormattingEnabled = true;
            this.cboInstances.Location = new System.Drawing.Point(140, 21);
            this.cboInstances.Name = "cboInstances";
            this.cboInstances.Size = new System.Drawing.Size(171, 21);
            this.cboInstances.TabIndex = 2;
            // 
            // txtLog
            // 
            this.txtLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txtLog.Location = new System.Drawing.Point(3, 48);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLog.Size = new System.Drawing.Size(308, 163);
            this.txtLog.TabIndex = 0;
            this.txtLog.WordWrap = false;
            // 
            // lblDisplayName
            // 
            this.lblDisplayName.Location = new System.Drawing.Point(3, 25);
            this.lblDisplayName.Name = "lblDisplayName";
            this.lblDisplayName.Size = new System.Drawing.Size(144, 14);
            this.lblDisplayName.TabIndex = 1;
            this.lblDisplayName.Text = "Display Name for SQLCM :";
            // 
            // progInstall
            // 
            this.progInstall.Location = new System.Drawing.Point(3, 26);
            this.progInstall.Name = "progInstall";
            this.progInstall.Size = new System.Drawing.Size(308, 16);
            this.progInstall.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progInstall.TabIndex = 0;
            // 
            // PageInstall
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cboInstances);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.lblDisplayName);
            this.Controls.Add(this.progInstall);
            this.Name = "PageInstall";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progInstall;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Label lblDisplayName;
        private System.Windows.Forms.ComboBox cboInstances;
    }
}
