using System.Drawing;
namespace SQLCM_Installer.WizardPages
{
    partial class PageSetupType
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
            this.toolTipAgent = new SQLCM_Installer.Custom_Controls.IderaToolTip();
            this.sqlcmManagementConsoleInfoPicture = new System.Windows.Forms.PictureBox();
            this.toolTipDashboard = new SQLCM_Installer.Custom_Controls.IderaToolTip();
            this.toolTipSQLCMManagamanetConsole = new SQLCM_Installer.Custom_Controls.IderaToolTip();
            this.checkBoxManagementConsole = new SQLCM_Installer.Custom_Controls.IderaCheckBox(true);
            this.toolTipSQLCM = new SQLCM_Installer.Custom_Controls.IderaToolTip();
            this.sqlcmAgentInfoPicture = new System.Windows.Forms.PictureBox();
            this.dashboardInfoPicture = new System.Windows.Forms.PictureBox();
            this.sqlcmInfoPicture = new System.Windows.Forms.PictureBox();
            this.pictureSetupType = new System.Windows.Forms.PictureBox();
            this.EULAHyperlink = new System.Windows.Forms.LinkLabel();
            this.checkBoxLicenseAgreement = new SQLCM_Installer.Custom_Controls.IderaCheckBox();
            this.panelStraightLine = new System.Windows.Forms.Panel();
            this.checkBoxDashboard = new SQLCM_Installer.Custom_Controls.IderaCheckBox(true);
            this.checkBoxAgent = new SQLCM_Installer.Custom_Controls.IderaCheckBox(true);
            this.checkBoxSQLCM = new SQLCM_Installer.Custom_Controls.IderaCheckBox(true);
            this.panelSubHeaderPanel = new System.Windows.Forms.Panel();
            this.labelSubHeader = new SQLCM_Installer.Custom_Controls.IderaLabel();
            ((System.ComponentModel.ISupportInitialize)(this.sqlcmManagementConsoleInfoPicture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sqlcmAgentInfoPicture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dashboardInfoPicture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sqlcmInfoPicture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureSetupType)).BeginInit();
            this.panelSubHeaderPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolTipAgent
            // 
            this.toolTipAgent.BackColor = System.Drawing.Color.Transparent;
            this.toolTipAgent.Location = new System.Drawing.Point(240, 250);
            this.toolTipAgent.Name = "toolTipAgent";
            this.toolTipAgent.TabIndex = 24;
            this.toolTipAgent.Visible = false;
            this.toolTipAgent.SetText("The SQL Compliance Manager Agent service and its required components");
            // 
            // sqlcmManagementConsoleInfoPicture
            // 
            this.sqlcmManagementConsoleInfoPicture.Image = global::SQLCM_Installer.Properties.Resources.infoicon;
            this.sqlcmManagementConsoleInfoPicture.Location = new System.Drawing.Point(348, 324);
            this.sqlcmManagementConsoleInfoPicture.Name = "sqlcmManagementConsoleInfoPicture";
            this.sqlcmManagementConsoleInfoPicture.Size = new System.Drawing.Size(17, 17);
            this.sqlcmManagementConsoleInfoPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.sqlcmManagementConsoleInfoPicture.TabIndex = 21;
            this.sqlcmManagementConsoleInfoPicture.TabStop = false;
            this.sqlcmManagementConsoleInfoPicture.MouseEnter += new System.EventHandler(this.sqlcmManagementConsoleInfoPicture_MouseEnter);
            this.sqlcmManagementConsoleInfoPicture.MouseLeave += new System.EventHandler(this.sqlcmManagementConsoleInfoPicture_MouseLeave);
            // 
            // toolTipDashboard
            // 
            this.toolTipDashboard.BackColor = System.Drawing.Color.Transparent;
            this.toolTipDashboard.Location = new System.Drawing.Point(65, 280);
            this.toolTipDashboard.Name = "toolTipDashboard";
            this.toolTipDashboard.TabIndex = 2;
            this.toolTipDashboard.Visible = false;
            this.toolTipDashboard.SetText("A web console to access SQL Compliance Manager");
            // 
            // toolTipSQLCMManagamanetConsole
            // 
            this.toolTipSQLCMManagamanetConsole.BackColor = System.Drawing.Color.Transparent;
            this.toolTipSQLCMManagamanetConsole.Location = new System.Drawing.Point(256, 220);
            this.toolTipSQLCMManagamanetConsole.Name = "toolTipSQLCMManagamanetConsole";
            this.toolTipSQLCMManagamanetConsole.TabIndex = 23;
            this.toolTipSQLCMManagamanetConsole.Visible = false;
            this.toolTipSQLCMManagamanetConsole.SetText("A Windows application to access SQL Compliance Manager");
            // 
            // checkBoxManagementConsole
            // 
            this.checkBoxManagementConsole.AutoSize = true;
            this.checkBoxManagementConsole.Checked = true;
            this.checkBoxManagementConsole.Disabled = true;
            this.checkBoxManagementConsole.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxManagementConsole.Location = new System.Drawing.Point(20, 320);
            this.checkBoxManagementConsole.Name = "checkBoxManagementConsole";
            this.checkBoxManagementConsole.Size = new System.Drawing.Size(321, 26);
            this.checkBoxManagementConsole.TabIndex = 11;
            this.checkBoxManagementConsole.Text = Constants.ProductMap[Products.Console];
            this.checkBoxManagementConsole.Click += new System.EventHandler(this.checkBoxManagementConsole_Click);
            // 
            // toolTipSQLCM
            // 
            this.toolTipSQLCM.BackColor = System.Drawing.Color.Transparent;
            this.toolTipSQLCM.Location = new System.Drawing.Point(117, 190);
            this.toolTipSQLCM.Name = "toolTipSQLCM";
            this.toolTipSQLCM.TabIndex = 22;
            this.toolTipSQLCM.Visible = false;
            this.toolTipSQLCM.SetText("This will install SQL Collection Service, SQL Compliance Manager Management Console (Windows) and Agent Components");
            // 
            // sqlcmAgentInfoPicture
            // 
            this.sqlcmAgentInfoPicture.Image = global::SQLCM_Installer.Properties.Resources.infoicon;
            this.sqlcmAgentInfoPicture.Location = new System.Drawing.Point(330, 354);
            this.sqlcmAgentInfoPicture.Name = "sqlcmAgentInfoPicture";
            this.sqlcmAgentInfoPicture.Size = new System.Drawing.Size(17, 17);
            this.sqlcmAgentInfoPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.sqlcmAgentInfoPicture.TabIndex = 20;
            this.sqlcmAgentInfoPicture.TabStop = false;
            this.sqlcmAgentInfoPicture.MouseEnter += new System.EventHandler(this.sqlcmAgentInfoPicture_MouseEnter);
            this.sqlcmAgentInfoPicture.MouseLeave += new System.EventHandler(this.sqlcmAgentInfoPicture_MouseLeave);
            // 
            // dashboardInfoPicture
            // 
            this.dashboardInfoPicture.Image = global::SQLCM_Installer.Properties.Resources.infoicon;
            this.dashboardInfoPicture.Location = new System.Drawing.Point(157, 384);
            this.dashboardInfoPicture.Name = "dashboardInfoPicture";
            this.dashboardInfoPicture.Size = new System.Drawing.Size(17, 17);
            this.dashboardInfoPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.dashboardInfoPicture.TabIndex = 19;
            this.dashboardInfoPicture.TabStop = false;
            this.dashboardInfoPicture.MouseEnter += new System.EventHandler(this.dashboardInfoPicture_MouseEnter);
            this.dashboardInfoPicture.MouseLeave += new System.EventHandler(this.dashboardInfoPicture_MouseLeave);
            // 
            // sqlcmInfoPicture
            // 
            this.sqlcmInfoPicture.Image = global::SQLCM_Installer.Properties.Resources.infoicon;
            this.sqlcmInfoPicture.Location = new System.Drawing.Point(206, 293);
            this.sqlcmInfoPicture.Name = "sqlcmInfoPicture";
            this.sqlcmInfoPicture.Size = new System.Drawing.Size(17, 17);
            this.sqlcmInfoPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.sqlcmInfoPicture.TabIndex = 18;
            this.sqlcmInfoPicture.TabStop = false;
            this.sqlcmInfoPicture.MouseEnter += new System.EventHandler(this.sqlcmInfoPicture_MouseEnter);
            this.sqlcmInfoPicture.MouseLeave += new System.EventHandler(this.sqlcmInfoPicture_MouseLeave);
            // 
            // pictureSetupType
            // 
            this.pictureSetupType.Image = global::SQLCM_Installer.Properties.Resources.diagramAllComponents;
            this.pictureSetupType.Location = new System.Drawing.Point(20, 84);
            this.pictureSetupType.Name = "pictureSetupType";
            this.pictureSetupType.Size = new System.Drawing.Size(510, 180);
            this.pictureSetupType.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureSetupType.TabIndex = 17;
            this.pictureSetupType.TabStop = false;
            // 
            // EULAHyperlink
            // 
            this.EULAHyperlink.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(96)))), ((int)(((byte)(137)))));
            this.EULAHyperlink.AutoSize = true;
            this.EULAHyperlink.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EULAHyperlink.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(96)))), ((int)(((byte)(137)))));
            this.EULAHyperlink.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(96)))), ((int)(((byte)(137)))));
            this.EULAHyperlink.Location = new System.Drawing.Point(280, 451);
            this.EULAHyperlink.Name = "EULAHyperlink";
            this.EULAHyperlink.Size = new System.Drawing.Size(182, 18);
            this.EULAHyperlink.TabIndex = 16;
            this.EULAHyperlink.TabStop = true;
            this.EULAHyperlink.Text = "End User License Agreement";
            this.EULAHyperlink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.EULAHyperlink_LinkClicked);
            // 
            // checkBoxLicenseAgreement
            // 
            this.checkBoxLicenseAgreement.AutoSize = true;
            this.checkBoxLicenseAgreement.Checked = false;
            this.checkBoxLicenseAgreement.Font = new System.Drawing.Font("Source Sans Pro", 10.5F);
            this.checkBoxLicenseAgreement.Location = new System.Drawing.Point(20, 450);
            this.checkBoxLicenseAgreement.Name = "checkBoxLicenseAgreement";
            this.checkBoxLicenseAgreement.Size = new System.Drawing.Size(270, 26);
            this.checkBoxLicenseAgreement.TabIndex = 15;
            this.checkBoxLicenseAgreement.Text = "I accept the Terms and Conditions of the";
            this.checkBoxLicenseAgreement.Click += new System.EventHandler(this.checkBoxLicenseAgreement_Click);
            // 
            // panelStraightLine
            // 
            this.panelStraightLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(205)))), ((int)(((byte)(205)))));
            this.panelStraightLine.Location = new System.Drawing.Point(20, 437);
            this.panelStraightLine.Name = "panelStraightLine";
            this.panelStraightLine.Size = new System.Drawing.Size(510, 1);
            this.panelStraightLine.TabIndex = 14;
            // 
            // checkBoxDashboard
            // 
            this.checkBoxDashboard.AutoSize = true;
            this.checkBoxDashboard.Checked = true;
            this.checkBoxDashboard.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxDashboard.Location = new System.Drawing.Point(20, 381);
            this.checkBoxDashboard.Name = "checkBoxDashboard";
            this.checkBoxDashboard.Size = new System.Drawing.Size(138, 26);
            this.checkBoxDashboard.TabIndex = 13;
            this.checkBoxDashboard.Text = Constants.ProductMap[Products.Dashboard];
            this.checkBoxDashboard.Click += new System.EventHandler(this.checkBoxDashboard_Click);
            // 
            // checkBoxAgent
            // 
            this.checkBoxAgent.AutoSize = true;
            this.checkBoxAgent.Checked = true;
            this.checkBoxAgent.Disabled = true;
            this.checkBoxAgent.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxAgent.Location = new System.Drawing.Point(20, 351);
            this.checkBoxAgent.Name = "checkBoxAgent";
            this.checkBoxAgent.Size = new System.Drawing.Size(306, 26);
            this.checkBoxAgent.TabIndex = 12;
            this.checkBoxAgent.Text = "SQL Compliance Manager Agent Components";
            this.checkBoxAgent.Click += new System.EventHandler(this.checkBoxAgent_Click);
            // 
            // checkBoxSQLCM
            // 
            this.checkBoxSQLCM.AutoSize = true;
            this.checkBoxSQLCM.Checked = true;
            this.checkBoxSQLCM.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxSQLCM.Location = new System.Drawing.Point(20, 290);
            this.checkBoxSQLCM.Name = "checkBoxSQLCM";
            this.checkBoxSQLCM.Size = new System.Drawing.Size(193, 26);
            this.checkBoxSQLCM.TabIndex = 10;
            this.checkBoxSQLCM.Text = Constants.ProductMap[Products.Compliance];
            this.checkBoxSQLCM.Click += new System.EventHandler(this.checkBoxSQLCM_Click);
            // 
            // panelSubHeaderPanel
            // 
            this.panelSubHeaderPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.panelSubHeaderPanel.Controls.Add(this.labelSubHeader);
            this.panelSubHeaderPanel.Location = new System.Drawing.Point(0, 0);
            this.panelSubHeaderPanel.Name = "panelSubHeaderPanel";
            this.panelSubHeaderPanel.Size = new System.Drawing.Size(550, 64);
            this.panelSubHeaderPanel.TabIndex = 9;
            // 
            // labelSubHeader
            // 
            this.labelSubHeader.AutoSize = true;
            this.labelSubHeader.BackColor = System.Drawing.Color.Transparent;
            this.labelSubHeader.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSubHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelSubHeader.Location = new System.Drawing.Point(20, 20);
            this.labelSubHeader.Name = "labelSubHeader";
            this.labelSubHeader.Size = new System.Drawing.Size(231, 18);
            this.labelSubHeader.TabIndex = 0;
            this.labelSubHeader.Text = "What features do you want to install?";
            // 
            // PageSetupType
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.toolTipAgent);
            this.Controls.Add(this.sqlcmManagementConsoleInfoPicture);
            this.Controls.Add(this.toolTipDashboard);
            this.Controls.Add(this.toolTipSQLCMManagamanetConsole);
            this.Controls.Add(this.checkBoxManagementConsole);
            this.Controls.Add(this.toolTipSQLCM);
            this.Controls.Add(this.sqlcmAgentInfoPicture);
            this.Controls.Add(this.dashboardInfoPicture);
            this.Controls.Add(this.sqlcmInfoPicture);
            this.Controls.Add(this.pictureSetupType);
            this.Controls.Add(this.EULAHyperlink);
            this.Controls.Add(this.checkBoxLicenseAgreement);
            this.Controls.Add(this.panelStraightLine);
            this.Controls.Add(this.checkBoxDashboard);
            this.Controls.Add(this.checkBoxAgent);
            this.Controls.Add(this.checkBoxSQLCM);
            this.Controls.Add(this.panelSubHeaderPanel);
            this.Name = "PageSetupType";
            ((System.ComponentModel.ISupportInitialize)(this.sqlcmManagementConsoleInfoPicture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sqlcmAgentInfoPicture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dashboardInfoPicture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sqlcmInfoPicture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureSetupType)).EndInit();
            this.panelSubHeaderPanel.ResumeLayout(false);
            this.panelSubHeaderPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelSubHeaderPanel;
        private Custom_Controls.IderaLabel labelSubHeader;
        private Custom_Controls.IderaCheckBox checkBoxSQLCM;
        private Custom_Controls.IderaCheckBox checkBoxManagementConsole;
        private Custom_Controls.IderaCheckBox checkBoxAgent;
        private Custom_Controls.IderaCheckBox checkBoxDashboard;
        private System.Windows.Forms.Panel panelStraightLine;
        private Custom_Controls.IderaCheckBox checkBoxLicenseAgreement;
        private System.Windows.Forms.LinkLabel EULAHyperlink;
        private System.Windows.Forms.PictureBox pictureSetupType;
        private System.Windows.Forms.PictureBox sqlcmInfoPicture;
        private System.Windows.Forms.PictureBox dashboardInfoPicture;
        private System.Windows.Forms.PictureBox sqlcmAgentInfoPicture;
        private System.Windows.Forms.PictureBox sqlcmManagementConsoleInfoPicture;
        private Custom_Controls.IderaToolTip toolTipSQLCM;
        private Custom_Controls.IderaToolTip toolTipSQLCMManagamanetConsole;
        private Custom_Controls.IderaToolTip toolTipAgent;
        private Custom_Controls.IderaToolTip toolTipDashboard;
    }
}
