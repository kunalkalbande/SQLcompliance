namespace SQLCM_Installer.WizardPages
{
    partial class PageUpgradeIntroduction
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
            this.linkLabelInstallationRequirements = new System.Windows.Forms.LinkLabel();
            this.upgradePanel = new System.Windows.Forms.Panel();
            this.EULAHyperlink = new System.Windows.Forms.LinkLabel();
            this.checkBoxLicenseAgreement = new SQLCM_Installer.Custom_Controls.IderaCheckBox();
            this.labelFreshThird = new SQLCM_Installer.Custom_Controls.IderaLabel(true);
            this.labelFreshSecond = new SQLCM_Installer.Custom_Controls.IderaLabel(true);
            this.labelFreshFirst = new SQLCM_Installer.Custom_Controls.IderaLabel(true);
            this.labelFreshText = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.radioButtonFresh = new SQLCM_Installer.Custom_Controls.IderaRadioButton(true);
            this.labelUpgradeFourth = new SQLCM_Installer.Custom_Controls.IderaLabel(true);
            this.labelUpgradeThird = new SQLCM_Installer.Custom_Controls.IderaLabel(true);
            this.labelUpgradeSecond = new SQLCM_Installer.Custom_Controls.IderaLabel(true);
            this.labelUpgradeFirst = new SQLCM_Installer.Custom_Controls.IderaLabel(true);
            this.labelUpgradeText = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.radioButtonUpgrade = new SQLCM_Installer.Custom_Controls.IderaRadioButton(true);
            this.panelSubHeaderPanel.SuspendLayout();
            this.upgradePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelSubHeaderPanel
            // 
            this.panelSubHeaderPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.panelSubHeaderPanel.Controls.Add(this.labelSubHeader);
            this.panelSubHeaderPanel.Location = new System.Drawing.Point(0, 0);
            this.panelSubHeaderPanel.Name = "panelSubHeaderPanel";
            this.panelSubHeaderPanel.Size = new System.Drawing.Size(550, 64);
            this.panelSubHeaderPanel.TabIndex = 11;
            // 
            // labelSubHeader
            // 
            this.labelSubHeader.AutoSize = true;
            this.labelSubHeader.BackColor = System.Drawing.Color.Transparent;
            this.labelSubHeader.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSubHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelSubHeader.Location = new System.Drawing.Point(20, 20);
            this.labelSubHeader.Name = "labelSubHeader";
            this.labelSubHeader.Size = new System.Drawing.Size(242, 18);
            this.labelSubHeader.TabIndex = 0;
            this.labelSubHeader.Text = "What type of installation do you want?";
            // 
            // linkLabelInstallationRequirements
            // 
            this.linkLabelInstallationRequirements.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(165)))), ((int)(((byte)(219)))));
            this.linkLabelInstallationRequirements.AutoSize = true;
            this.linkLabelInstallationRequirements.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabelInstallationRequirements.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.linkLabelInstallationRequirements.LinkArea = new System.Windows.Forms.LinkArea(10, 12);
            this.linkLabelInstallationRequirements.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.linkLabelInstallationRequirements.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(165)))), ((int)(((byte)(219)))));
            this.linkLabelInstallationRequirements.Location = new System.Drawing.Point(20, 433);
            this.linkLabelInstallationRequirements.Name = "linkLabelInstallationRequirements";
            this.linkLabelInstallationRequirements.Size = new System.Drawing.Size(391, 23);
            this.linkLabelInstallationRequirements.TabIndex = 14;
            this.linkLabelInstallationRequirements.TabStop = true;
            this.linkLabelInstallationRequirements.Text = "Need Help? Click here for detailed installation requirements.";
            this.linkLabelInstallationRequirements.UseCompatibleTextRendering = true;
            this.linkLabelInstallationRequirements.VisitedLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(165)))), ((int)(((byte)(219)))));
            this.linkLabelInstallationRequirements.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelInstallationRequirements_Click);
            // 
            // upgradePanel
            // 
            this.upgradePanel.Controls.Add(this.EULAHyperlink);
            this.upgradePanel.Controls.Add(this.checkBoxLicenseAgreement);
            this.upgradePanel.Controls.Add(this.labelFreshThird);
            this.upgradePanel.Controls.Add(this.labelFreshSecond);
            this.upgradePanel.Controls.Add(this.labelFreshFirst);
            this.upgradePanel.Controls.Add(this.labelFreshText);
            this.upgradePanel.Controls.Add(this.radioButtonFresh);
            this.upgradePanel.Controls.Add(this.labelUpgradeFourth);
            this.upgradePanel.Controls.Add(this.labelUpgradeThird);
            this.upgradePanel.Controls.Add(this.labelUpgradeSecond);
            this.upgradePanel.Controls.Add(this.labelUpgradeFirst);
            this.upgradePanel.Controls.Add(this.labelUpgradeText);
            this.upgradePanel.Controls.Add(this.radioButtonUpgrade);
            this.upgradePanel.Location = new System.Drawing.Point(20, 67);
            this.upgradePanel.Name = "upgradePanel";
            this.upgradePanel.Size = new System.Drawing.Size(510, 351);
            this.upgradePanel.TabIndex = 22;
            // 
            // EULAHyperlink
            //
            this.EULAHyperlink.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(96)))), ((int)(((byte)(137)))));
            this.EULAHyperlink.AutoSize = true;
            this.EULAHyperlink.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EULAHyperlink.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(96)))), ((int)(((byte)(137)))));
            this.EULAHyperlink.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(96)))), ((int)(((byte)(137)))));
            this.EULAHyperlink.Location = new System.Drawing.Point(295, 328);
            this.EULAHyperlink.Name = "EULAHyperlink";
            this.EULAHyperlink.Size = new System.Drawing.Size(219, 17);
            this.EULAHyperlink.TabIndex = 16;
            this.EULAHyperlink.TabStop = true;
            this.EULAHyperlink.Text = "End User License Agreement";
            this.EULAHyperlink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.EULAHyperlink_LinkClicked);
            //
            // checkBoxLicenseAgreement
            //
            this.checkBoxLicenseAgreement.AutoSize = true;
            this.checkBoxLicenseAgreement.Checked = false;
            this.checkBoxLicenseAgreement.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F);
            this.checkBoxLicenseAgreement.Location = new System.Drawing.Point(22, 327);
            this.checkBoxLicenseAgreement.Name = "checkBoxLicenseAgreement";
            this.checkBoxLicenseAgreement.Size = new System.Drawing.Size(285, 26);
            this.checkBoxLicenseAgreement.TabIndex = 15;
            this.checkBoxLicenseAgreement.Text = "I accept the Terms and Conditions of the";
            this.checkBoxLicenseAgreement.Click += new System.EventHandler(this.checkBoxLicenseAgreement_Click);
            //
            // labelFreshThird
            // 
            this.labelFreshThird.AutoSize = true;
            this.labelFreshThird.BackColor = System.Drawing.Color.Transparent;
            this.labelFreshThird.Font = new System.Drawing.Font("Source Sans Pro", 10.5F);
            this.labelFreshThird.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelFreshThird.Location = new System.Drawing.Point(65, 300);
            this.labelFreshThird.Name = "labelFreshThird";
            this.labelFreshThird.Size = new System.Drawing.Size(0, 18);
            this.labelFreshThird.TabIndex = 35;
            this.labelFreshThird.Text = "- ";
            this.labelFreshThird.Visible = false;
            // 
            // labelFreshSecond
            // 
            this.labelFreshSecond.AutoSize = true;
            this.labelFreshSecond.BackColor = System.Drawing.Color.Transparent;
            this.labelFreshSecond.Font = new System.Drawing.Font("Source Sans Pro", 10.5F);
            this.labelFreshSecond.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelFreshSecond.Location = new System.Drawing.Point(65, 280);
            this.labelFreshSecond.Name = "labelFreshSecond";
            this.labelFreshSecond.Size = new System.Drawing.Size(0, 18);
            this.labelFreshSecond.TabIndex = 34;
            this.labelFreshSecond.Text = "- ";
            this.labelFreshSecond.Visible = false;
            // 
            // labelFreshFirst
            // 
            this.labelFreshFirst.AutoSize = true;
            this.labelFreshFirst.BackColor = System.Drawing.Color.Transparent;
            this.labelFreshFirst.Font = new System.Drawing.Font("Source Sans Pro", 10.5F);
            this.labelFreshFirst.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelFreshFirst.Location = new System.Drawing.Point(65, 260);
            this.labelFreshFirst.Name = "labelFreshFirst";
            this.labelFreshFirst.Size = new System.Drawing.Size(120, 18);
            this.labelFreshFirst.TabIndex = 33;
            this.labelFreshFirst.Text = "- ";
            this.labelFreshFirst.Visible = false;
            // 
            // labelFreshText
            // 
            this.labelFreshText.AutoSize = true;
            this.labelFreshText.BackColor = System.Drawing.Color.Transparent;
            this.labelFreshText.Font = new System.Drawing.Font("Source Sans Pro", 10.5F);
            this.labelFreshText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelFreshText.Location = new System.Drawing.Point(40, 220);
            this.labelFreshText.Name = "labelFreshText";
            this.labelFreshText.Size = new System.Drawing.Size(447, 36);
            this.labelFreshText.TabIndex = 32;
            this.labelFreshText.Text = "Choose this option to install a fresh copy of the features listed below that you \r\ncur" +
    "rently do not have installed:";
            // 
            // radioButtonFresh
            // 
            this.radioButtonFresh.AutoSize = true;
            this.radioButtonFresh.Checked = false;
            this.radioButtonFresh.Font = new System.Drawing.Font("Source Sans Pro", 12F);
            this.radioButtonFresh.Location = new System.Drawing.Point(24, 190);
            this.radioButtonFresh.Name = "radioButtonFresh";
            this.radioButtonFresh.Size = new System.Drawing.Size(218, 27);
            this.radioButtonFresh.TabIndex = 31;
            this.radioButtonFresh.Text = "Fresh Installation";
            // 
            // labelUpgradeFourth
            // 
            this.labelUpgradeFourth.AutoSize = true;
            this.labelUpgradeFourth.BackColor = System.Drawing.Color.Transparent;
            this.labelUpgradeFourth.Font = new System.Drawing.Font("Source Sans Pro", 10.5F);
            this.labelUpgradeFourth.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelUpgradeFourth.Location = new System.Drawing.Point(65, 160);
            this.labelUpgradeFourth.Name = "labelUpgradeFourth";
            this.labelUpgradeFourth.Size = new System.Drawing.Size(120, 18);
            this.labelUpgradeFourth.TabIndex = 30;
            this.labelUpgradeFourth.Text = "- ";
            this.labelUpgradeFourth.Visible = false;
            // 
            // labelUpgradeThird
            // 
            this.labelUpgradeThird.AutoSize = true;
            this.labelUpgradeThird.BackColor = System.Drawing.Color.Transparent;
            this.labelUpgradeThird.Font = new System.Drawing.Font("Source Sans Pro", 10.5F);
            this.labelUpgradeThird.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelUpgradeThird.Location = new System.Drawing.Point(65, 135);
            this.labelUpgradeThird.Name = "labelUpgradeThird";
            this.labelUpgradeThird.Size = new System.Drawing.Size(285, 18);
            this.labelUpgradeThird.TabIndex = 29;
            this.labelUpgradeThird.Text = "- ";
            this.labelUpgradeThird.Visible = false;
            // 
            // labelUpgradeSecond
            // 
            this.labelUpgradeSecond.AutoSize = true;
            this.labelUpgradeSecond.BackColor = System.Drawing.Color.Transparent;
            this.labelUpgradeSecond.Font = new System.Drawing.Font("Source Sans Pro", 10.5F);
            this.labelUpgradeSecond.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelUpgradeSecond.Location = new System.Drawing.Point(65, 110);
            this.labelUpgradeSecond.Name = "labelUpgradeSecond";
            this.labelUpgradeSecond.Size = new System.Drawing.Size(299, 18);
            this.labelUpgradeSecond.TabIndex = 28;
            this.labelUpgradeSecond.Text = "- ";
            this.labelUpgradeSecond.Visible = false;
            // 
            // labelUpgradeFirst
            // 
            this.labelUpgradeFirst.AutoSize = true;
            this.labelUpgradeFirst.BackColor = System.Drawing.Color.Transparent;
            this.labelUpgradeFirst.Font = new System.Drawing.Font("Source Sans Pro", 10.5F);
            this.labelUpgradeFirst.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelUpgradeFirst.Location = new System.Drawing.Point(65, 85);
            this.labelUpgradeFirst.Name = "labelUpgradeFirst";
            this.labelUpgradeFirst.Size = new System.Drawing.Size(166, 18);
            this.labelUpgradeFirst.TabIndex = 27;
            this.labelUpgradeFirst.Text = "- ";
            this.labelUpgradeFirst.Visible = false;
            // 
            // labelUpgradeText
            // 
            this.labelUpgradeText.AutoSize = true;
            this.labelUpgradeText.BackColor = System.Drawing.Color.Transparent;
            this.labelUpgradeText.Font = new System.Drawing.Font("Source Sans Pro", 10.5F);
            this.labelUpgradeText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelUpgradeText.Location = new System.Drawing.Point(40, 45);
            this.labelUpgradeText.Name = "labelUpgradeText";
            this.labelUpgradeText.Size = new System.Drawing.Size(460, 36);
            this.labelUpgradeText.TabIndex = 26;
            this.labelUpgradeText.Text = "Choose this option to upgrade the features listed below that you currently \r\nhave installed:";
            // 
            // radioButtonUpgrade
            // 
            this.radioButtonUpgrade.AutoSize = true;
            this.radioButtonUpgrade.Checked = false;
            this.radioButtonUpgrade.Font = new System.Drawing.Font("Source Sans Pro", 12F);
            this.radioButtonUpgrade.Location = new System.Drawing.Point(22, 15);
            this.radioButtonUpgrade.Name = "radioButtonUpgrade";
            this.radioButtonUpgrade.Size = new System.Drawing.Size(242, 27);
            this.radioButtonUpgrade.TabIndex = 25;
            this.radioButtonUpgrade.Text = "Upgrade an Existing Installation";
            this.radioButtonUpgrade.Checked = true;
            // 
            // PageUpgradeIntroduction
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.Controls.Add(this.upgradePanel);
            this.Controls.Add(this.linkLabelInstallationRequirements);
            this.Controls.Add(this.panelSubHeaderPanel);
            this.Name = "PageUpgradeIntroduction";
            this.panelSubHeaderPanel.ResumeLayout(false);
            this.panelSubHeaderPanel.PerformLayout();
            this.upgradePanel.ResumeLayout(false);
            this.upgradePanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelSubHeaderPanel;
        private Custom_Controls.IderaLabel labelSubHeader;
        private System.Windows.Forms.LinkLabel linkLabelInstallationRequirements;
        private System.Windows.Forms.Panel upgradePanel;
        private Custom_Controls.IderaLabel labelUpgradeThird;
        private Custom_Controls.IderaLabel labelUpgradeSecond;
        private Custom_Controls.IderaLabel labelUpgradeFirst;
        private Custom_Controls.IderaLabel labelUpgradeText;
        private Custom_Controls.IderaRadioButton radioButtonUpgrade;
        private Custom_Controls.IderaLabel labelUpgradeFourth;
        private Custom_Controls.IderaLabel labelFreshThird;
        private Custom_Controls.IderaLabel labelFreshSecond;
        private Custom_Controls.IderaLabel labelFreshFirst;
        private Custom_Controls.IderaLabel labelFreshText;
        private Custom_Controls.IderaRadioButton radioButtonFresh;
        private Custom_Controls.IderaCheckBox checkBoxLicenseAgreement;
        private System.Windows.Forms.LinkLabel EULAHyperlink;
    }
}
