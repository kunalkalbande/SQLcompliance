namespace SQLCM_Installer.WizardPages
{
    partial class PageError
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
            this.panelSubHeaderPanel = new System.Windows.Forms.Panel();
            this.labelErrorHeader = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.pictureErrorImage = new System.Windows.Forms.PictureBox();
            this.labelErrorMessage = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.panelSubHeaderPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureErrorImage)).BeginInit();
            this.SuspendLayout();
            // 
            // panelSubHeaderPanel
            // 
            this.panelSubHeaderPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.panelSubHeaderPanel.Controls.Add(this.labelErrorHeader);
            this.panelSubHeaderPanel.Controls.Add(this.pictureErrorImage);
            this.panelSubHeaderPanel.Location = new System.Drawing.Point(0, 0);
            this.panelSubHeaderPanel.Name = "panelSubHeaderPanel";
            this.panelSubHeaderPanel.Size = new System.Drawing.Size(550, 64);
            this.panelSubHeaderPanel.TabIndex = 11;
            // 
            // labelErrorHeader
            // 
            this.labelErrorHeader.AutoSize = true;
            this.labelErrorHeader.BackColor = System.Drawing.Color.Transparent;
            this.labelErrorHeader.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelErrorHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(2)))), ((int)(((byte)(27)))));
            this.labelErrorHeader.Location = new System.Drawing.Point(47, 23);
            this.labelErrorHeader.Name = "labelErrorHeader";
            this.labelErrorHeader.Size = new System.Drawing.Size(122, 15);
            this.labelErrorHeader.TabIndex = 1;
            this.labelErrorHeader.Text = "Installation Failed";
            // 
            // pictureErrorImage
            // 
            this.pictureErrorImage.Image = global::SQLCM_Installer.Properties.Resources.criticalicon;
            this.pictureErrorImage.Location = new System.Drawing.Point(20, 24);
            this.pictureErrorImage.Name = "pictureErrorImage";
            this.pictureErrorImage.Size = new System.Drawing.Size(16, 16);
            this.pictureErrorImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureErrorImage.TabIndex = 0;
            this.pictureErrorImage.TabStop = false;
            // 
            // labelErrorMessage
            // 
            this.labelErrorMessage.AutoSize = true;
            this.labelErrorMessage.BackColor = System.Drawing.Color.Transparent;
            this.labelErrorMessage.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelErrorMessage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelErrorMessage.Location = new System.Drawing.Point(20, 94);
            this.labelErrorMessage.Name = "labelErrorMessage";
            this.labelErrorMessage.MaximumSize = new System.Drawing.Size(526, 0);
            this.labelErrorMessage.AutoSize = true;
            this.labelErrorMessage.TabIndex = 12;
            this.labelErrorMessage.Text = Constants.InstallationFailureMessage;
            // 
            // PageError
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.labelErrorMessage);
            this.Controls.Add(this.panelSubHeaderPanel);
            this.Name = "PageError";
            this.Size = new System.Drawing.Size(550, 486);
            this.panelSubHeaderPanel.ResumeLayout(false);
            this.panelSubHeaderPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureErrorImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelSubHeaderPanel;
        private System.Windows.Forms.PictureBox pictureErrorImage;
        private Custom_Controls.IderaLabel labelErrorHeader;
        private Custom_Controls.IderaLabel labelErrorMessage;
    }
}
