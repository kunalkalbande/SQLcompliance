namespace SQLCM_Installer.WizardPages
{
    partial class PageDashboardRegYesOrNo
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
            this.labelHeader = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.radioYes = new SQLCM_Installer.Custom_Controls.IderaRadioButton();
            this.radioNo = new SQLCM_Installer.Custom_Controls.IderaRadioButton();
            this.panelDashboardHeader = new System.Windows.Forms.Panel();
            this.labelDashboardHeader = new SQLCM_Installer.Custom_Controls.IderaHeaderLabel();
            this.radioRemoteDashboard = new SQLCM_Installer.Custom_Controls.IderaRadioButton();
            this.radioLocalDashboard = new SQLCM_Installer.Custom_Controls.IderaRadioButton();
            this.panelDashboardLocation = new System.Windows.Forms.Panel();
            this.panelRemoteDashboardLocation = new System.Windows.Forms.Panel();
            this.textRemoteDashboardAdminPassword = new SQLCM_Installer.Custom_Controls.IderaTextBox();
            this.textRemoteDashboardAdminUserName = new SQLCM_Installer.Custom_Controls.IderaTextBox();
            this.labelRemoteDashboardAdminPassword = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.labelRemoteDashboardAdminUserName = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.labelRemoteDashboardAdminCred = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.textRemoteDashboardLocation = new SQLCM_Installer.Custom_Controls.IderaTextBox();
            this.labelRemoteDashboardLocation = new SQLCM_Installer.Custom_Controls.IderaHeaderLabel();
            this.panelSubHeaderPanel.SuspendLayout();
            this.panelDashboardHeader.SuspendLayout();
            this.panelDashboardLocation.SuspendLayout();
            this.panelRemoteDashboardLocation.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelSubHeaderPanel
            // 
            this.panelSubHeaderPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.panelSubHeaderPanel.Controls.Add(this.labelHeader);
            this.panelSubHeaderPanel.Location = new System.Drawing.Point(0, 0);
            this.panelSubHeaderPanel.Name = "panelSubHeaderPanel";
            this.panelSubHeaderPanel.Size = new System.Drawing.Size(550, 64);
            this.panelSubHeaderPanel.TabIndex = 13;
            // 
            // labelHeader
            // 
            this.labelHeader.AutoSize = true;
            this.labelHeader.BackColor = System.Drawing.Color.Transparent;
            this.labelHeader.Disabled = false;
            this.labelHeader.Font = new System.Drawing.Font("Source Sans Pro", 10.5F);
            this.labelHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelHeader.Location = new System.Drawing.Point(20, 12);
            this.labelHeader.Name = "labelHeader";
            this.labelHeader.Size = new System.Drawing.Size(439, 36);
            this.labelHeader.TabIndex = 0;
            this.labelHeader.Text = "Do you want to register " + Constants.ProductMap[Products.Compliance] + " with an existing IDERA \r\nDashboard" +
    "?";
            // 
            // radioYes
            // 
            this.radioYes.AutoSize = true;
            this.radioYes.Checked = false;
            this.radioYes.Disabled = false;
            this.radioYes.Location = new System.Drawing.Point(20, 82);
            this.radioYes.Name = "radioYes";
            this.radioYes.Size = new System.Drawing.Size(46, 21);
            this.radioYes.TabIndex = 14;
            this.radioYes.Text = "Yes";
            this.radioYes.Click += new System.EventHandler(this.radioYes_Click);
            // 
            // radioNo
            // 
            this.radioNo.AutoSize = true;
            this.radioNo.Checked = true;
            this.radioNo.Disabled = false;
            this.radioNo.Location = new System.Drawing.Point(20, 107);
            this.radioNo.Name = "radioNo";
            this.radioNo.Size = new System.Drawing.Size(42, 21);
            this.radioNo.TabIndex = 15;
            this.radioNo.Text = "No";
            this.radioNo.Click += new System.EventHandler(this.radioNo_Click);
            // 
            // panelDashboardHeader
            // 
            this.panelDashboardHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(236)))), ((int)(((byte)(236)))));
            this.panelDashboardHeader.Controls.Add(this.labelDashboardHeader);
            this.panelDashboardHeader.Location = new System.Drawing.Point(0, 0);
            this.panelDashboardHeader.Name = "panelDashboardHeader";
            this.panelDashboardHeader.Size = new System.Drawing.Size(550, 38);
            this.panelDashboardHeader.TabIndex = 18;
            // 
            // labelDashboardHeader
            // 
            this.labelDashboardHeader.AutoSize = true;
            this.labelDashboardHeader.BackColor = System.Drawing.Color.Transparent;
            this.labelDashboardHeader.Font = new System.Drawing.Font("Source Sans Pro", 10.5F);
            this.labelDashboardHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelDashboardHeader.Location = new System.Drawing.Point(20, 10);
            this.labelDashboardHeader.Name = "labelDashboardHeader";
            this.labelDashboardHeader.Size = new System.Drawing.Size(168, 18);
            this.labelDashboardHeader.TabIndex = 0;
            this.labelDashboardHeader.Text = Constants.ProductMap[Products.Dashboard] + " Location";
            // 
            // radioRemoteDashboard
            // 
            this.radioRemoteDashboard.AutoSize = true;
            this.radioRemoteDashboard.Checked = false;
            this.radioRemoteDashboard.Disabled = false;
            this.radioRemoteDashboard.Font = new System.Drawing.Font("Source Sans Pro", 10.5F);
            this.radioRemoteDashboard.Location = new System.Drawing.Point(20, 85);
            this.radioRemoteDashboard.Name = "radioRemoteDashboard";
            this.radioRemoteDashboard.Size = new System.Drawing.Size(329, 25);
            this.radioRemoteDashboard.TabIndex = 20;
            this.radioRemoteDashboard.Text = "Register the " + Constants.ProductMap[Products.Dashboard] + " on a remote server. ";
            this.radioRemoteDashboard.Click += new System.EventHandler(this.radioRemoteDashboard_Click);
            // 
            // radioLocalDashboard
            // 
            this.radioLocalDashboard.AutoSize = true;
            this.radioLocalDashboard.Checked = false;
            this.radioLocalDashboard.Disabled = false;
            this.radioLocalDashboard.Font = new System.Drawing.Font("Source Sans Pro", 10.5F);
            this.radioLocalDashboard.Location = new System.Drawing.Point(20, 57);
            this.radioLocalDashboard.Name = "radioLocalDashboard";
            this.radioLocalDashboard.Size = new System.Drawing.Size(260, 25);
            this.radioLocalDashboard.TabIndex = 19;
            this.radioLocalDashboard.Text = "Register with " + Constants.ProductMap[Products.Dashboard] + " locally.";
            this.radioLocalDashboard.Click += new System.EventHandler(this.radioLocalDashboard_Click);
            // 
            // panelDashboardLocation
            // 
            this.panelDashboardLocation.BackColor = System.Drawing.Color.Transparent;
            this.panelDashboardLocation.Controls.Add(this.panelDashboardHeader);
            this.panelDashboardLocation.Controls.Add(this.radioRemoteDashboard);
            this.panelDashboardLocation.Controls.Add(this.radioLocalDashboard);
            this.panelDashboardLocation.Location = new System.Drawing.Point(0, 147);
            this.panelDashboardLocation.Name = "panelDashboardLocation";
            this.panelDashboardLocation.Size = new System.Drawing.Size(550, 124);
            this.panelDashboardLocation.TabIndex = 21;
            this.panelDashboardLocation.Visible = false;
            // 
            // panelRemoteDashboardLocation
            // 
            this.panelRemoteDashboardLocation.Controls.Add(this.textRemoteDashboardAdminPassword);
            this.panelRemoteDashboardLocation.Controls.Add(this.textRemoteDashboardAdminUserName);
            this.panelRemoteDashboardLocation.Controls.Add(this.labelRemoteDashboardAdminPassword);
            this.panelRemoteDashboardLocation.Controls.Add(this.labelRemoteDashboardAdminUserName);
            this.panelRemoteDashboardLocation.Controls.Add(this.labelRemoteDashboardAdminCred);
            this.panelRemoteDashboardLocation.Controls.Add(this.textRemoteDashboardLocation);
            this.panelRemoteDashboardLocation.Controls.Add(this.labelRemoteDashboardLocation);
            this.panelRemoteDashboardLocation.Location = new System.Drawing.Point(0, 272);
            this.panelRemoteDashboardLocation.Name = "panelRemoteDashboardLocation";
            this.panelRemoteDashboardLocation.Size = new System.Drawing.Size(550, 215);
            this.panelRemoteDashboardLocation.TabIndex = 23;
            this.panelRemoteDashboardLocation.Visible = false;
            // 
            // textRemoteDashboardAdminPassword
            // 
            this.textRemoteDashboardAdminPassword.Font = new System.Drawing.Font("Source Sans Pro", 10.5F);
            this.textRemoteDashboardAdminPassword.Location = new System.Drawing.Point(153, 147);
            this.textRemoteDashboardAdminPassword.Name = "textRemoteDashboardAdminPassword";
            this.textRemoteDashboardAdminPassword.Size = new System.Drawing.Size(287, 25);
            this.textRemoteDashboardAdminPassword.TabIndex = 6;
            // 
            // textRemoteDashboardAdminUserName
            // 
            this.textRemoteDashboardAdminUserName.Font = new System.Drawing.Font("Source Sans Pro", 10.5F);
            this.textRemoteDashboardAdminUserName.Location = new System.Drawing.Point(153, 107);
            this.textRemoteDashboardAdminUserName.Name = "textRemoteDashboardAdminUserName";
            this.textRemoteDashboardAdminUserName.Size = new System.Drawing.Size(287, 25);
            this.textRemoteDashboardAdminUserName.TabIndex = 5;
            // 
            // labelRemoteDashboardAdminPassword
            // 
            this.labelRemoteDashboardAdminPassword.AutoSize = true;
            this.labelRemoteDashboardAdminPassword.BackColor = System.Drawing.Color.Transparent;
            this.labelRemoteDashboardAdminPassword.Disabled = false;
            this.labelRemoteDashboardAdminPassword.Font = new System.Drawing.Font("Source Sans Pro", 10.5F);
            this.labelRemoteDashboardAdminPassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelRemoteDashboardAdminPassword.Location = new System.Drawing.Point(17, 150);
            this.labelRemoteDashboardAdminPassword.Name = "labelRemoteDashboardAdminPassword";
            this.labelRemoteDashboardAdminPassword.Size = new System.Drawing.Size(68, 18);
            this.labelRemoteDashboardAdminPassword.TabIndex = 4;
            this.labelRemoteDashboardAdminPassword.Text = "Password:";
            this.textRemoteDashboardAdminPassword.UseSystemPasswordChar = true;
            // 
            // labelRemoteDashboardAdminUserName
            // 
            this.labelRemoteDashboardAdminUserName.AutoSize = true;
            this.labelRemoteDashboardAdminUserName.BackColor = System.Drawing.Color.Transparent;
            this.labelRemoteDashboardAdminUserName.Disabled = false;
            this.labelRemoteDashboardAdminUserName.Font = new System.Drawing.Font("Source Sans Pro", 10.5F);
            this.labelRemoteDashboardAdminUserName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelRemoteDashboardAdminUserName.Location = new System.Drawing.Point(17, 110);
            this.labelRemoteDashboardAdminUserName.Name = "labelRemoteDashboardAdminUserName";
            this.labelRemoteDashboardAdminUserName.Size = new System.Drawing.Size(125, 18);
            this.labelRemoteDashboardAdminUserName.TabIndex = 3;
            this.labelRemoteDashboardAdminUserName.Text = "Domain\\UserName:";
            // 
            // labelRemoteDashboardAdminCred
            // 
            this.labelRemoteDashboardAdminCred.AutoSize = true;
            this.labelRemoteDashboardAdminCred.BackColor = System.Drawing.Color.Transparent;
            this.labelRemoteDashboardAdminCred.Disabled = false;
            this.labelRemoteDashboardAdminCred.Font = new System.Drawing.Font("Source Sans Pro", 10.5F);
            this.labelRemoteDashboardAdminCred.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelRemoteDashboardAdminCred.Location = new System.Drawing.Point(17, 79);
            this.labelRemoteDashboardAdminCred.Name = "labelRemoteDashboardAdminCred";
            this.labelRemoteDashboardAdminCred.Size = new System.Drawing.Size(231, 18);
            this.labelRemoteDashboardAdminCred.TabIndex = 2;
            this.labelRemoteDashboardAdminCred.Text = "Dashboard Administrator Credentials";
            // 
            // textRemoteDashboardLocation
            // 
            this.textRemoteDashboardLocation.Font = new System.Drawing.Font("Source Sans Pro", 10.5F);
            this.textRemoteDashboardLocation.Location = new System.Drawing.Point(20, 34);
            this.textRemoteDashboardLocation.Name = "textRemoteDashboardLocation";
            this.textRemoteDashboardLocation.Size = new System.Drawing.Size(479, 25);
            this.textRemoteDashboardLocation.TabIndex = 1;
            // 
            // labelRemoteDashboardLocation
            // 
            this.labelRemoteDashboardLocation.AutoSize = true;
            this.labelRemoteDashboardLocation.BackColor = System.Drawing.Color.Transparent;
            this.labelRemoteDashboardLocation.Font = new System.Drawing.Font("Source Sans Pro", 10.5F);
            this.labelRemoteDashboardLocation.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelRemoteDashboardLocation.Location = new System.Drawing.Point(17, 5);
            this.labelRemoteDashboardLocation.Name = "labelRemoteDashboardLocation";
            this.labelRemoteDashboardLocation.Size = new System.Drawing.Size(218, 18);
            this.labelRemoteDashboardLocation.TabIndex = 0;
            this.labelRemoteDashboardLocation.Text = "Remote " + Constants.ProductMap[Products.Dashboard] + " Location";
            // 
            // PageDashboardRegYesOrNo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.panelRemoteDashboardLocation);
            this.Controls.Add(this.panelDashboardLocation);
            this.Controls.Add(this.radioNo);
            this.Controls.Add(this.radioYes);
            this.Controls.Add(this.panelSubHeaderPanel);
            this.Name = "PageDashboardRegYesOrNo";
            this.panelSubHeaderPanel.ResumeLayout(false);
            this.panelSubHeaderPanel.PerformLayout();
            this.panelDashboardHeader.ResumeLayout(false);
            this.panelDashboardHeader.PerformLayout();
            this.panelDashboardLocation.ResumeLayout(false);
            this.panelDashboardLocation.PerformLayout();
            this.panelRemoteDashboardLocation.ResumeLayout(false);
            this.panelRemoteDashboardLocation.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelSubHeaderPanel;
        private Custom_Controls.IderaLabel labelHeader;
        private Custom_Controls.IderaRadioButton radioYes;
        private Custom_Controls.IderaRadioButton radioNo;
        private System.Windows.Forms.Panel panelDashboardHeader;
        private Custom_Controls.IderaHeaderLabel labelDashboardHeader;
        private Custom_Controls.IderaRadioButton radioRemoteDashboard;
        private Custom_Controls.IderaRadioButton radioLocalDashboard;
        private System.Windows.Forms.Panel panelDashboardLocation;
        private System.Windows.Forms.Panel panelRemoteDashboardLocation;
        private Custom_Controls.IderaTextBox textRemoteDashboardAdminPassword;
        private Custom_Controls.IderaTextBox textRemoteDashboardAdminUserName;
        private Custom_Controls.IderaLabel labelRemoteDashboardAdminPassword;
        private Custom_Controls.IderaLabel labelRemoteDashboardAdminUserName;
        private Custom_Controls.IderaLabel labelRemoteDashboardAdminCred;
        private Custom_Controls.IderaTextBox textRemoteDashboardLocation;
        private Custom_Controls.IderaHeaderLabel labelRemoteDashboardLocation;
    }
}
