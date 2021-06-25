namespace SQLCM_Installer.WizardPages
{
    partial class PageServiceAccount
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
            this.labelSubHeader = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.labelTitle = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.labelServiceUserName = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.labelServicePassword = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.txtServiceUserName = new SQLCM_Installer.Custom_Controls.IderaTextBox();
            this.txtServicePassword = new SQLCM_Installer.Custom_Controls.IderaTextBox();
            this.pictureCredentialError = new System.Windows.Forms.PictureBox();
            this.labelCredentialError = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.panelSubHeaderPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureCredentialError)).BeginInit();
            this.SuspendLayout();
            // 
            // panelSubHeaderPanel
            // 
            this.panelSubHeaderPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.panelSubHeaderPanel.Controls.Add(this.labelSubHeader);
            this.panelSubHeaderPanel.Location = new System.Drawing.Point(0, 0);
            this.panelSubHeaderPanel.Name = "panelSubHeaderPanel";
            this.panelSubHeaderPanel.Size = new System.Drawing.Size(550, 64);
            this.panelSubHeaderPanel.TabIndex = 10;
            // 
            // labelSubHeader
            // 
            this.labelSubHeader.AutoSize = true;
            this.labelSubHeader.BackColor = System.Drawing.Color.Transparent;
            this.labelSubHeader.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSubHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelSubHeader.Location = new System.Drawing.Point(20, 20);
            this.labelSubHeader.Name = "labelSubHeader";
            this.labelSubHeader.Size = new System.Drawing.Size(329, 18);
            this.labelSubHeader.TabIndex = 0;
            this.labelSubHeader.Text = "Please specify the account to be used by IDERA services. ";
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.BackColor = System.Drawing.Color.Transparent;
            this.labelTitle.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelTitle.Location = new System.Drawing.Point(20, 84);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(442, 36);
            this.labelTitle.TabIndex = 11;
            this.labelTitle.Text = "The " + Constants.ProductMap[Products.Compliance] + " and " + Constants.ProductMap[Products.Dashboard] + " services will use the \r\nWindows cr" +
    "edentials specified below.";
            // 
            // labelServiceUserName
            // 
            this.labelServiceUserName.AutoSize = true;
            this.labelServiceUserName.BackColor = System.Drawing.Color.Transparent;
            this.labelServiceUserName.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelServiceUserName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelServiceUserName.Location = new System.Drawing.Point(26, 145);
            this.labelServiceUserName.Name = "labelServiceUserName";
            this.labelServiceUserName.Size = new System.Drawing.Size(124, 18);
            this.labelServiceUserName.TabIndex = 12;
            this.labelServiceUserName.Text = "Domain\\Username:";
            // 
            // labelServicePassword
            // 
            this.labelServicePassword.AutoSize = true;
            this.labelServicePassword.BackColor = System.Drawing.Color.Transparent;
            this.labelServicePassword.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelServicePassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelServicePassword.Location = new System.Drawing.Point(26, 184);
            this.labelServicePassword.Name = "labelServicePassword";
            this.labelServicePassword.Size = new System.Drawing.Size(68, 18);
            this.labelServicePassword.TabIndex = 13;
            this.labelServicePassword.Text = "Password:";
            // 
            // txtServiceUserName
            // 
            this.txtServiceUserName.Font = new System.Drawing.Font("Source Sans Pro", 10.5F);
            this.txtServiceUserName.Location = new System.Drawing.Point(176, 141);
            this.txtServiceUserName.Name = "txtServiceUserName";
            this.txtServiceUserName.Size = new System.Drawing.Size(287, 25);
            this.txtServiceUserName.TabIndex = 14;
            // 
            // txtServicePassword
            // 
            this.txtServicePassword.Font = new System.Drawing.Font("Source Sans Pro", 10.5F);
            this.txtServicePassword.Location = new System.Drawing.Point(176, 181);
            this.txtServicePassword.Name = "txtServicePassword";
            this.txtServicePassword.Size = new System.Drawing.Size(287, 25);
            this.txtServicePassword.TabIndex = 15;
            this.txtServicePassword.UseSystemPasswordChar = true;
            // 
            // pictureCredentialError
            // 
            this.pictureCredentialError.Image = global::SQLCM_Installer.Properties.Resources.criticalicon;
            this.pictureCredentialError.Location = new System.Drawing.Point(176, 222);
            this.pictureCredentialError.Name = "pictureCredentialError";
            this.pictureCredentialError.Size = new System.Drawing.Size(16, 16);
            this.pictureCredentialError.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureCredentialError.TabIndex = 16;
            this.pictureCredentialError.TabStop = false;
            this.pictureCredentialError.Visible = false;
            // 
            // labelCredentialError
            // 
            this.labelCredentialError.AutoSize = true;
            this.labelCredentialError.BackColor = System.Drawing.Color.Transparent;
            this.labelCredentialError.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.labelCredentialError.Location = new System.Drawing.Point(199, 222);
            this.labelCredentialError.Name = "labelCredentialError";
            this.labelCredentialError.Size = new System.Drawing.Size(0, 13);
            this.labelCredentialError.TabIndex = 17;
            this.labelCredentialError.Visible = false;
            // 
            // PageServiceAccount
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.labelCredentialError);
            this.Controls.Add(this.pictureCredentialError);
            this.Controls.Add(this.txtServicePassword);
            this.Controls.Add(this.txtServiceUserName);
            this.Controls.Add(this.labelServicePassword);
            this.Controls.Add(this.labelServiceUserName);
            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.panelSubHeaderPanel);
            this.Name = "PageServiceAccount";
            this.panelSubHeaderPanel.ResumeLayout(false);
            this.panelSubHeaderPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureCredentialError)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelSubHeaderPanel;
        private Custom_Controls.IderaLabel labelSubHeader;
        private Custom_Controls.IderaLabel labelTitle;
        private Custom_Controls.IderaLabel labelServiceUserName;
        private Custom_Controls.IderaLabel labelServicePassword;
        private Custom_Controls.IderaTextBox txtServiceUserName;
        private Custom_Controls.IderaTextBox txtServicePassword;
        private System.Windows.Forms.PictureBox pictureCredentialError;
        private Custom_Controls.IderaLabel labelCredentialError;
    }
}
