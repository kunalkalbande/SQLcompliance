namespace SQLCM_Installer.WizardPages
{
    partial class PageSuccess
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
            this.panelSQLCompliance = new System.Windows.Forms.Panel();
            this.labelSQLCMDesc = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.labelSQLCMHeader = new SQLCM_Installer.Custom_Controls.IderaLabel(true);
            this.pictureSQLCompliance = new System.Windows.Forms.PictureBox();
            this.panelIderaDashboard = new System.Windows.Forms.Panel();
            this.linkLabelDashboardLaunchDesc = new System.Windows.Forms.LinkLabel();
            this.labelDashboardHeader = new SQLCM_Installer.Custom_Controls.IderaLabel(true);
            this.pictureIderaDashboard = new System.Windows.Forms.PictureBox();
            this.labelFirewallConfigurationHeader = new SQLCM_Installer.Custom_Controls.IderaLabel(true);
            this.checkBoxLaunchManagementConsole = new SQLCM_Installer.Custom_Controls.IderaCheckBox();
            this.linkLabelPortDesc = new System.Windows.Forms.LinkLabel();
            this.panelStraightLine = new System.Windows.Forms.Panel();
            this.panelFirewallConfiguration = new System.Windows.Forms.Panel();
            this.panelSubHeaderPanel.SuspendLayout();
            this.panelSQLCompliance.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureSQLCompliance)).BeginInit();
            this.panelIderaDashboard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureIderaDashboard)).BeginInit();
            this.panelFirewallConfiguration.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelSubHeaderPanel
            // 
            this.panelSubHeaderPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.panelSubHeaderPanel.Controls.Add(this.labelHeader);
            this.panelSubHeaderPanel.Location = new System.Drawing.Point(0, 0);
            this.panelSubHeaderPanel.Name = "panelSubHeaderPanel";
            this.panelSubHeaderPanel.Size = new System.Drawing.Size(550, 64);
            this.panelSubHeaderPanel.TabIndex = 12;
            // 
            // labelHeader
            // 
            this.labelHeader.AutoSize = true;
            this.labelHeader.BackColor = System.Drawing.Color.Transparent;
            this.labelHeader.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelHeader.Location = new System.Drawing.Point(20, 23);
            this.labelHeader.Name = "labelHeader";
            this.labelHeader.Size = new System.Drawing.Size(458, 18);
            this.labelHeader.TabIndex = 0;
            this.labelHeader.Text = "IDERA " + Constants.ProductMap[Products.Compliance] + " installation has completed successfully!";
            // 
            // panelSQLCompliance
            // 
            this.panelSQLCompliance.Controls.Add(this.labelSQLCMDesc);
            this.panelSQLCompliance.Controls.Add(this.labelSQLCMHeader);
            this.panelSQLCompliance.Controls.Add(this.pictureSQLCompliance);
            this.panelSQLCompliance.Location = new System.Drawing.Point(0, 87);
            this.panelSQLCompliance.Name = "panelSQLCompliance";
            this.panelSQLCompliance.Size = new System.Drawing.Size(550, 76);
            this.panelSQLCompliance.TabIndex = 13;
            // 
            // labelSQLCMDesc
            // 
            this.labelSQLCMDesc.AutoSize = true;
            this.labelSQLCMDesc.BackColor = System.Drawing.Color.Transparent;
            this.labelSQLCMDesc.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSQLCMDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelSQLCMDesc.Location = new System.Drawing.Point(67, 33);
            this.labelSQLCMDesc.Name = "labelSQLCMDesc";
            this.labelSQLCMDesc.MaximumSize = new System.Drawing.Size(450, 0);
            this.labelSQLCMDesc.AutoSize = true;
            this.labelSQLCMDesc.TabIndex = 2;
            this.labelSQLCMDesc.Text = "Go to the Start Menu: Select IDERA > " + Constants.ProductMap[Products.Console] + ".";
            // 
            // labelSQLCMHeader
            // 
            this.labelSQLCMHeader.AutoSize = true;
            this.labelSQLCMHeader.BackColor = System.Drawing.Color.Transparent;
            this.labelSQLCMHeader.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSQLCMHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelSQLCMHeader.Location = new System.Drawing.Point(67, 14);
            this.labelSQLCMHeader.Name = "labelSQLCMHeader";
            this.labelSQLCMHeader.Size = new System.Drawing.Size(165, 18);
            this.labelSQLCMHeader.TabIndex = 1;
            this.labelSQLCMHeader.Text = Constants.ProductMap[Products.Compliance];
            // 
            // pictureSQLCompliance
            // 
            this.pictureSQLCompliance.Image = global::SQLCM_Installer.Properties.Resources.windowsicon;
            this.pictureSQLCompliance.Location = new System.Drawing.Point(23, 15);
            this.pictureSQLCompliance.Name = "pictureSQLCompliance";
            this.pictureSQLCompliance.Size = new System.Drawing.Size(27, 30);
            this.pictureSQLCompliance.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureSQLCompliance.TabIndex = 0;
            this.pictureSQLCompliance.TabStop = false;
            // 
            // panelIderaDashboard
            // 
            this.panelIderaDashboard.Controls.Add(this.linkLabelDashboardLaunchDesc);
            this.panelIderaDashboard.Controls.Add(this.labelDashboardHeader);
            this.panelIderaDashboard.Controls.Add(this.pictureIderaDashboard);
            this.panelIderaDashboard.Location = new System.Drawing.Point(0, 163);
            this.panelIderaDashboard.Name = "panelIderaDashboard";
            this.panelIderaDashboard.Size = new System.Drawing.Size(550, 76);
            this.panelIderaDashboard.TabIndex = 14;
            // 
            // linkLabelDashboardLaunchDesc
            // 
            this.linkLabelDashboardLaunchDesc.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(165)))), ((int)(((byte)(219)))));
            this.linkLabelDashboardLaunchDesc.AutoSize = true;
            this.linkLabelDashboardLaunchDesc.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabelDashboardLaunchDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.linkLabelDashboardLaunchDesc.LinkArea = new System.Windows.Forms.LinkArea(13, 22);
            this.linkLabelDashboardLaunchDesc.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.linkLabelDashboardLaunchDesc.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(165)))), ((int)(((byte)(219)))));
            this.linkLabelDashboardLaunchDesc.Location = new System.Drawing.Point(73, 30);
            this.linkLabelDashboardLaunchDesc.Name = "linkLabelDashboardLaunchDesc";
            this.linkLabelDashboardLaunchDesc.MaximumSize = new System.Drawing.Size(450, 0);
            this.linkLabelDashboardLaunchDesc.AutoSize = true;
            this.linkLabelDashboardLaunchDesc.TabIndex = 4;
            this.linkLabelDashboardLaunchDesc.TabStop = true;
            this.linkLabelDashboardLaunchDesc.Text = "Open the URL https://localhost:9291 from a web browser. Be sure to save the URL as a favorite.";
            this.linkLabelDashboardLaunchDesc.UseCompatibleTextRendering = true;
            this.linkLabelDashboardLaunchDesc.VisitedLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(165)))), ((int)(((byte)(219)))));
            this.linkLabelDashboardLaunchDesc.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelDashboardLaunchDesc_LinkClicked);
            // 
            // labelDashboardHeader
            // 
            this.labelDashboardHeader.AutoSize = true;
            this.labelDashboardHeader.BackColor = System.Drawing.Color.Transparent;
            this.labelDashboardHeader.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDashboardHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelDashboardHeader.Location = new System.Drawing.Point(70, 13);
            this.labelDashboardHeader.Name = "labelDashboardHeader";
            this.labelDashboardHeader.Size = new System.Drawing.Size(116, 18);
            this.labelDashboardHeader.TabIndex = 3;
            this.labelDashboardHeader.Text = Constants.ProductMap[Products.Dashboard];
            // 
            // pictureIderaDashboard
            // 
            this.pictureIderaDashboard.Image = global::SQLCM_Installer.Properties.Resources.webicon;
            this.pictureIderaDashboard.Location = new System.Drawing.Point(23, 23);
            this.pictureIderaDashboard.Name = "pictureIderaDashboard";
            this.pictureIderaDashboard.Size = new System.Drawing.Size(30, 30);
            this.pictureIderaDashboard.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureIderaDashboard.TabIndex = 0;
            this.pictureIderaDashboard.TabStop = false;
            // 
            // labelFirewallConfigurationHeader
            // 
            this.labelFirewallConfigurationHeader.AutoSize = true;
            this.labelFirewallConfigurationHeader.BackColor = System.Drawing.Color.Transparent;
            this.labelFirewallConfigurationHeader.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelFirewallConfigurationHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelFirewallConfigurationHeader.Location = new System.Drawing.Point(16, 30);
            this.labelFirewallConfigurationHeader.Name = "labelFirewallConfigurationHeader";
            this.labelFirewallConfigurationHeader.Size = new System.Drawing.Size(147, 18);
            this.labelFirewallConfigurationHeader.TabIndex = 15;
            this.labelFirewallConfigurationHeader.Text = "Firewall Configuration";
            // 
            // checkBoxLaunchManagementConsole
            // 
            this.checkBoxLaunchManagementConsole.AutoSize = true;
            this.checkBoxLaunchManagementConsole.Checked = false;
            this.checkBoxLaunchManagementConsole.Location = new System.Drawing.Point(26, 432);
            this.checkBoxLaunchManagementConsole.Name = "checkBoxLaunchManagementConsole";
            this.checkBoxLaunchManagementConsole.Size = new System.Drawing.Size(393, 22);
            this.checkBoxLaunchManagementConsole.TabIndex = 18;
            this.checkBoxLaunchManagementConsole.Text = "Launch the " + Constants.ProductMap[Products.Compliance] + " Windows Console.";
            // 
            // linkLabelPortDesc
            // 
            this.linkLabelPortDesc.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(165)))), ((int)(((byte)(219)))));
            this.linkLabelPortDesc.AutoSize = true;
            this.linkLabelPortDesc.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabelPortDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.linkLabelPortDesc.LinkArea = new System.Windows.Forms.LinkArea(0, 10);
            this.linkLabelPortDesc.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.linkLabelPortDesc.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(165)))), ((int)(((byte)(219)))));
            this.linkLabelPortDesc.Location = new System.Drawing.Point(20, 53);
            this.linkLabelPortDesc.Name = "linkLabelPortDesc";
            this.linkLabelPortDesc.Size = new System.Drawing.Size(297, 23);
            this.linkLabelPortDesc.TabIndex = 19;
            this.linkLabelPortDesc.TabStop = true;
            this.linkLabelPortDesc.Text = "Click here  for a list of ports used by this product.";
            this.linkLabelPortDesc.UseCompatibleTextRendering = true;
            this.linkLabelPortDesc.VisitedLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(165)))), ((int)(((byte)(219)))));
            this.linkLabelPortDesc.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelPortDesc_LinkClicked);
            // 
            // panelStraightLine
            // 
            this.panelStraightLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(205)))), ((int)(((byte)(205)))));
            this.panelStraightLine.Location = new System.Drawing.Point(16, 15);
            this.panelStraightLine.Name = "panelStraightLine";
            this.panelStraightLine.Size = new System.Drawing.Size(510, 1);
            this.panelStraightLine.TabIndex = 20;
            // 
            // panelFirewallConfiguration
            // 
            this.panelFirewallConfiguration.BackColor = System.Drawing.Color.Transparent;
            this.panelFirewallConfiguration.Controls.Add(this.panelStraightLine);
            this.panelFirewallConfiguration.Controls.Add(this.labelFirewallConfigurationHeader);
            this.panelFirewallConfiguration.Controls.Add(this.linkLabelPortDesc);
            this.panelFirewallConfiguration.Location = new System.Drawing.Point(0, 265);
            this.panelFirewallConfiguration.Name = "panelFirewallConfiguration";
            this.panelFirewallConfiguration.Size = new System.Drawing.Size(550, 79);
            this.panelFirewallConfiguration.TabIndex = 21;
            // 
            // PageSuccess
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.panelFirewallConfiguration);
            this.Controls.Add(this.checkBoxLaunchManagementConsole);
            this.Controls.Add(this.panelIderaDashboard);
            this.Controls.Add(this.panelSQLCompliance);
            this.Controls.Add(this.panelSubHeaderPanel);
            this.Name = "PageSuccess";
            this.panelSubHeaderPanel.ResumeLayout(false);
            this.panelSubHeaderPanel.PerformLayout();
            this.panelSQLCompliance.ResumeLayout(false);
            this.panelSQLCompliance.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureSQLCompliance)).EndInit();
            this.panelIderaDashboard.ResumeLayout(false);
            this.panelIderaDashboard.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureIderaDashboard)).EndInit();
            this.panelFirewallConfiguration.ResumeLayout(false);
            this.panelFirewallConfiguration.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelSubHeaderPanel;
        private Custom_Controls.IderaLabel labelHeader;
        private System.Windows.Forms.Panel panelSQLCompliance;
        private System.Windows.Forms.PictureBox pictureSQLCompliance;
        private System.Windows.Forms.Panel panelIderaDashboard;
        private System.Windows.Forms.PictureBox pictureIderaDashboard;
        private Custom_Controls.IderaLabel labelSQLCMDesc;
        private Custom_Controls.IderaLabel labelSQLCMHeader;
        private Custom_Controls.IderaLabel labelDashboardHeader;
        private Custom_Controls.IderaLabel labelFirewallConfigurationHeader;
        private Custom_Controls.IderaCheckBox checkBoxLaunchManagementConsole;
        private System.Windows.Forms.LinkLabel linkLabelDashboardLaunchDesc;
        private System.Windows.Forms.LinkLabel linkLabelPortDesc;
        private System.Windows.Forms.Panel panelStraightLine;
        private System.Windows.Forms.Panel panelFirewallConfiguration;
    }
}
