namespace SQLCM_Installer.WizardPages
{
    partial class PageInstallationDirectory
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
            this.panelDashboardHeader = new System.Windows.Forms.Panel();
            this.labelDashboardHeader = new SQLCM_Installer.Custom_Controls.IderaHeaderLabel();
            this.panelDisplayname = new System.Windows.Forms.Panel();
            this.txtDisplayName = new SQLCM_Installer.Custom_Controls.IderaTextBox();
            this.CMDisplayNameInfoPicture = new System.Windows.Forms.PictureBox();
            this.labelDisplayName = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.panelDashboardInstallDir = new System.Windows.Forms.Panel();
            this.remoteDashboardInfoPicture = new System.Windows.Forms.PictureBox();
            this.radioDashboardRemote = new SQLCM_Installer.Custom_Controls.IderaRadioButton();
            this.radioDashboardLocal = new SQLCM_Installer.Custom_Controls.IderaRadioButton();
            this.txtRemoteDashboardPassword = new SQLCM_Installer.Custom_Controls.IderaTextBox();
            this.txtRemoteDashboardUserName = new SQLCM_Installer.Custom_Controls.IderaTextBox();
            this.labelRemoteDashboardPassword = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.labelRemoteDashboardUserName = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.labelRemoteDashboardCred = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.txtRemoteDashboardUrl = new SQLCM_Installer.Custom_Controls.IderaTextBox();
            this.dashboardInstallDirBrowseButton = new SQLCM_Installer.Custom_Controls.IderaButton();
            this.txtDashboardInstallDir = new SQLCM_Installer.Custom_Controls.IderaTextBox();
            this.panelCMInstallDir = new System.Windows.Forms.Panel();
            this.cmInstallDirBrowseButton = new SQLCM_Installer.Custom_Controls.IderaButton();
            this.txtCMInstallDir = new SQLCM_Installer.Custom_Controls.IderaTextBox();
            this.labelCMInstallDir = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.panelSQLCMHeader = new System.Windows.Forms.Panel();
            this.labelSQLCMHeader = new SQLCM_Installer.Custom_Controls.IderaHeaderLabel();
            this.panelSubHeaderPanel = new System.Windows.Forms.Panel();
            this.labelSubHeader = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.toolTipDisplayName = new SQLCM_Installer.Custom_Controls.IderaToolTip();
            this.toolTipRemoteDashboard = new SQLCM_Installer.Custom_Controls.IderaToolTip();
            this.panelDashboardHeader.SuspendLayout();
            this.panelDisplayname.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CMDisplayNameInfoPicture)).BeginInit();
            this.panelDashboardInstallDir.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.remoteDashboardInfoPicture)).BeginInit();
            this.panelCMInstallDir.SuspendLayout();
            this.panelSQLCMHeader.SuspendLayout();
            this.panelSubHeaderPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelDashboardHeader
            // 
            this.panelDashboardHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(236)))), ((int)(((byte)(236)))));
            this.panelDashboardHeader.Controls.Add(this.labelDashboardHeader);
            this.panelDashboardHeader.Location = new System.Drawing.Point(0, 200);
            this.panelDashboardHeader.Name = "panelDashboardHeader";
            this.panelDashboardHeader.Size = new System.Drawing.Size(550, 38);
            this.panelDashboardHeader.TabIndex = 49;
            // 
            // labelDashboardHeader
            // 
            this.labelDashboardHeader.AutoSize = true;
            this.labelDashboardHeader.BackColor = System.Drawing.Color.Transparent;
            this.labelDashboardHeader.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDashboardHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelDashboardHeader.Location = new System.Drawing.Point(20, 10);
            this.labelDashboardHeader.Name = "labelDashboardHeader";
            this.labelDashboardHeader.Size = new System.Drawing.Size(113, 18);
            this.labelDashboardHeader.TabIndex = 0;
            this.labelDashboardHeader.Text = Constants.ProductMap[Products.Dashboard];
            // 
            // panelDisplayname
            // 
            this.panelDisplayname.Controls.Add(this.txtDisplayName);
            this.panelDisplayname.Controls.Add(this.CMDisplayNameInfoPicture);
            this.panelDisplayname.Controls.Add(this.labelDisplayName);
            this.panelDisplayname.Location = new System.Drawing.Point(0, 150);
            this.panelDisplayname.Name = "panelDisplayname";
            this.panelDisplayname.Size = new System.Drawing.Size(550, 50);
            this.panelDisplayname.TabIndex = 30;
            // 
            // txtDisplayName
            // 
            this.txtDisplayName.Font = new System.Drawing.Font("Source Sans Pro", 10.5F);
            this.txtDisplayName.Location = new System.Drawing.Point(159, 7);
            this.txtDisplayName.Name = "txtDisplayName";
            this.txtDisplayName.Size = new System.Drawing.Size(370, 25);
            this.txtDisplayName.TabIndex = 22;
            this.txtDisplayName.WatermarkText = "Enter SQL Compliance Manager Display Name";
            // 
            // CMDisplayNameInfoPicture
            // 
            this.CMDisplayNameInfoPicture.Image = global::SQLCM_Installer.Properties.Resources.infoicon;
            this.CMDisplayNameInfoPicture.Location = new System.Drawing.Point(123, 10);
            this.CMDisplayNameInfoPicture.Name = "CMDisplayNameInfoPicture";
            this.CMDisplayNameInfoPicture.Size = new System.Drawing.Size(17, 17);
            this.CMDisplayNameInfoPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.CMDisplayNameInfoPicture.TabIndex = 51;
            this.CMDisplayNameInfoPicture.TabStop = false;
            this.CMDisplayNameInfoPicture.MouseEnter += new System.EventHandler(this.CMDisplayNameInfoPicture_MouseEnter);
            this.CMDisplayNameInfoPicture.MouseLeave += new System.EventHandler(this.CMDisplayNameInfoPicture_MouseLeave);
            // 
            // toolTipDisplayName
            // 
            this.toolTipDisplayName.BackColor = System.Drawing.Color.Transparent;
            this.toolTipDisplayName.Location = new System.Drawing.Point(33, 56);
            this.toolTipDisplayName.Name = "toolTipDisplayName";
            this.toolTipDisplayName.TabIndex = 1;
            this.toolTipDisplayName.Visible = false;
            this.toolTipDisplayName.BringToFront();
            this.toolTipDisplayName.SetText("The name displayed on the IDERA dashboard for this installation");
            // 
            // toolTipRemoteDashboard
            // 
            this.toolTipRemoteDashboard.BackColor = System.Drawing.Color.Transparent;
            this.toolTipRemoteDashboard.Location = new System.Drawing.Point(212, 190);
            this.toolTipRemoteDashboard.Name = "toolTipRemoteDashboard";
            this.toolTipRemoteDashboard.TabIndex = 1;
            this.toolTipRemoteDashboard.Visible = false;
            this.toolTipRemoteDashboard.BringToFront();
            this.toolTipRemoteDashboard.SetText("Connect to a different machine to retrieve the dashboard installation");
            // 
            // labelDisplayName
            // 
            this.labelDisplayName.AutoSize = true;
            this.labelDisplayName.BackColor = System.Drawing.Color.Transparent;
            this.labelDisplayName.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDisplayName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelDisplayName.Location = new System.Drawing.Point(22, 9);
            this.labelDisplayName.Name = "labelDisplayName";
            this.labelDisplayName.Size = new System.Drawing.Size(93, 18);
            this.labelDisplayName.TabIndex = 50;
            this.labelDisplayName.Text = "Display Name:";
            // 
            // panelDashboardInstallDir
            // 
            this.panelDashboardInstallDir.Controls.Add(this.remoteDashboardInfoPicture);
            this.panelDashboardInstallDir.Controls.Add(this.radioDashboardRemote);
            this.panelDashboardInstallDir.Controls.Add(this.radioDashboardLocal);
            this.panelDashboardInstallDir.Controls.Add(this.txtRemoteDashboardPassword);
            this.panelDashboardInstallDir.Controls.Add(this.txtRemoteDashboardUserName);
            this.panelDashboardInstallDir.Controls.Add(this.labelRemoteDashboardPassword);
            this.panelDashboardInstallDir.Controls.Add(this.labelRemoteDashboardUserName);
            this.panelDashboardInstallDir.Controls.Add(this.labelRemoteDashboardCred);
            this.panelDashboardInstallDir.Controls.Add(this.txtRemoteDashboardUrl);
            this.panelDashboardInstallDir.Controls.Add(this.dashboardInstallDirBrowseButton);
            this.panelDashboardInstallDir.Controls.Add(this.txtDashboardInstallDir);
            this.panelDashboardInstallDir.Location = new System.Drawing.Point(0, 238);
            this.panelDashboardInstallDir.Name = "panelDashboardInstallDir";
            this.panelDashboardInstallDir.Size = new System.Drawing.Size(550, 245);
            this.panelDashboardInstallDir.TabIndex = 31;
            // 
            // remoteDashboardInfoPicture
            // 
            this.remoteDashboardInfoPicture.Image = global::SQLCM_Installer.Properties.Resources.infoicon;
            this.remoteDashboardInfoPicture.Location = new System.Drawing.Point(303, 59);
            this.remoteDashboardInfoPicture.Name = "remoteDashboardInfoPicture";
            this.remoteDashboardInfoPicture.Size = new System.Drawing.Size(17, 17);
            this.remoteDashboardInfoPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.remoteDashboardInfoPicture.TabIndex = 52;
            this.remoteDashboardInfoPicture.TabStop = false;
            this.remoteDashboardInfoPicture.MouseEnter += new System.EventHandler(this.remoteDashboardInfoPicture_MouseEnter);
            this.remoteDashboardInfoPicture.MouseLeave += new System.EventHandler(this.remoteDashboardInfoPicture_MouseLeave);
            // 
            // radioDashboardRemote
            // 
            this.radioDashboardRemote.AutoSize = true;
            this.radioDashboardRemote.Checked = false;
            this.radioDashboardRemote.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioDashboardRemote.Location = new System.Drawing.Point(20, 55);
            this.radioDashboardRemote.Name = "radioDashboardRemote";
            this.radioDashboardRemote.Size = new System.Drawing.Size(305, 25);
            this.radioDashboardRemote.TabIndex = 26;
            this.radioDashboardRemote.Text = "Use remote " + Constants.ProductMap[Products.Dashboard] + " installation";
            this.radioDashboardRemote.Click += new System.EventHandler(this.radioDashboardRemote_Click);
            // 
            // radioDashboardLocal
            // 
            this.radioDashboardLocal.AutoSize = true;
            this.radioDashboardLocal.Checked = true;
            this.radioDashboardLocal.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioDashboardLocal.Location = new System.Drawing.Point(20, 11);
            this.radioDashboardLocal.Name = "radioDashboardLocal";
            this.radioDashboardLocal.Size = new System.Drawing.Size(186, 26);
            this.radioDashboardLocal.TabIndex = 23;
            this.radioDashboardLocal.Text = "Install or upgrade locally";
            this.radioDashboardLocal.Click += new System.EventHandler(this.radioDashboardLocal_Click);
            // 
            // txtRemoteDashboardPassword
            // 
            this.txtRemoteDashboardPassword.Enabled = false;
            this.txtRemoteDashboardPassword.Font = new System.Drawing.Font("Source Sans Pro", 10.5F);
            this.txtRemoteDashboardPassword.Location = new System.Drawing.Point(202, 199);
            this.txtRemoteDashboardPassword.Name = "txtRemoteDashboardPassword";
            this.txtRemoteDashboardPassword.Size = new System.Drawing.Size(300, 25);
            this.txtRemoteDashboardPassword.TabIndex = 29;
            this.txtRemoteDashboardPassword.UseSystemPasswordChar = true;
            // 
            // txtRemoteDashboardUserName
            // 
            this.txtRemoteDashboardUserName.Enabled = false;
            this.txtRemoteDashboardUserName.Font = new System.Drawing.Font("Source Sans Pro", 10.5F);
            this.txtRemoteDashboardUserName.Location = new System.Drawing.Point(202, 162);
            this.txtRemoteDashboardUserName.Name = "txtRemoteDashboardUserName";
            this.txtRemoteDashboardUserName.Size = new System.Drawing.Size(300, 25);
            this.txtRemoteDashboardUserName.TabIndex = 28;
            // 
            // labelRemoteDashboardPassword
            // 
            this.labelRemoteDashboardPassword.AutoSize = true;
            this.labelRemoteDashboardPassword.BackColor = System.Drawing.Color.Transparent;
            this.labelRemoteDashboardPassword.Disabled = true;
            this.labelRemoteDashboardPassword.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelRemoteDashboardPassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelRemoteDashboardPassword.Location = new System.Drawing.Point(40, 198);
            this.labelRemoteDashboardPassword.Name = "labelRemoteDashboardPassword";
            this.labelRemoteDashboardPassword.Size = new System.Drawing.Size(68, 18);
            this.labelRemoteDashboardPassword.TabIndex = 56;
            this.labelRemoteDashboardPassword.Text = "Password:";
            // 
            // labelRemoteDashboardUserName
            // 
            this.labelRemoteDashboardUserName.AutoSize = true;
            this.labelRemoteDashboardUserName.BackColor = System.Drawing.Color.Transparent;
            this.labelRemoteDashboardUserName.Disabled = true;
            this.labelRemoteDashboardUserName.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelRemoteDashboardUserName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelRemoteDashboardUserName.Location = new System.Drawing.Point(40, 164);
            this.labelRemoteDashboardUserName.Name = "labelRemoteDashboardUserName";
            this.labelRemoteDashboardUserName.Size = new System.Drawing.Size(125, 18);
            this.labelRemoteDashboardUserName.TabIndex = 55;
            this.labelRemoteDashboardUserName.Text = "Domain\\UserName:";
            // 
            // labelRemoteDashboardCred
            // 
            this.labelRemoteDashboardCred.AutoSize = true;
            this.labelRemoteDashboardCred.BackColor = System.Drawing.Color.Transparent;
            this.labelRemoteDashboardCred.Disabled = true;
            this.labelRemoteDashboardCred.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelRemoteDashboardCred.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelRemoteDashboardCred.Location = new System.Drawing.Point(40, 137);
            this.labelRemoteDashboardCred.Name = "labelRemoteDashboardCred";
            this.labelRemoteDashboardCred.Size = new System.Drawing.Size(231, 18);
            this.labelRemoteDashboardCred.TabIndex = 54;
            this.labelRemoteDashboardCred.Text = "Dashboard Administrator Credentials";
            // 
            // txtRemoteDashboardUrl
            // 
            this.txtRemoteDashboardUrl.Enabled = false;
            this.txtRemoteDashboardUrl.Font = new System.Drawing.Font("Source Sans Pro", 10.5F);
            this.txtRemoteDashboardUrl.Location = new System.Drawing.Point(43, 92);
            this.txtRemoteDashboardUrl.Name = "txtRemoteDashboardUrl";
            this.txtRemoteDashboardUrl.Size = new System.Drawing.Size(487, 25);
            this.txtRemoteDashboardUrl.TabIndex = 27;
            this.txtRemoteDashboardUrl.WatermarkText = "Enter existing IDERA Dashboard URL";
            // 
            // dashboardInstallDirBrowseButton
            // 
            this.dashboardInstallDirBrowseButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(186)))), ((int)(((byte)(163)))));
            this.dashboardInstallDirBrowseButton.BorderColor = System.Drawing.Color.Transparent;
            this.dashboardInstallDirBrowseButton.BorderWidth = 2;
            this.dashboardInstallDirBrowseButton.ButtonText = "";
            this.dashboardInstallDirBrowseButton.Disabled = false;
            this.dashboardInstallDirBrowseButton.EndColor = System.Drawing.Color.Yellow;
            this.dashboardInstallDirBrowseButton.FlatAppearance.BorderSize = 0;
            this.dashboardInstallDirBrowseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.dashboardInstallDirBrowseButton.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dashboardInstallDirBrowseButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.dashboardInstallDirBrowseButton.GradientAngle = 90;
            this.dashboardInstallDirBrowseButton.Location = new System.Drawing.Point(453, 11);
            this.dashboardInstallDirBrowseButton.Name = "dashboardInstallDirBrowseButton";
            this.dashboardInstallDirBrowseButton.ShowButtontext = true;
            this.dashboardInstallDirBrowseButton.Size = new System.Drawing.Size(78, 28);
            this.dashboardInstallDirBrowseButton.StartColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(186)))), ((int)(((byte)(163)))));
            this.dashboardInstallDirBrowseButton.TabIndex = 25;
            this.dashboardInstallDirBrowseButton.Text = "Browse";
            this.dashboardInstallDirBrowseButton.TextLocation_X = 100;
            this.dashboardInstallDirBrowseButton.TextLocation_Y = 25;
            this.dashboardInstallDirBrowseButton.UseVisualStyleBackColor = false;
            this.dashboardInstallDirBrowseButton.Click += new System.EventHandler(this.dashboardInstallDirBrowseButton_Click);
            // 
            // txtDashboardInstallDir
            // 
            this.txtDashboardInstallDir.Font = new System.Drawing.Font("Source Sans Pro", 10.5F);
            this.txtDashboardInstallDir.Location = new System.Drawing.Point(212, 11);
            this.txtDashboardInstallDir.Name = "txtDashboardInstallDir";
            this.txtDashboardInstallDir.Size = new System.Drawing.Size(233, 25);
            this.txtDashboardInstallDir.TabIndex = 24;
            this.txtDashboardInstallDir.Text = "C:\\Program Files\\IDERA\\Dashboard";
            // 
            // panelCMInstallDir
            // 
            this.panelCMInstallDir.Controls.Add(this.cmInstallDirBrowseButton);
            this.panelCMInstallDir.Controls.Add(this.txtCMInstallDir);
            this.panelCMInstallDir.Controls.Add(this.labelCMInstallDir);
            this.panelCMInstallDir.Controls.Add(this.panelSQLCMHeader);
            this.panelCMInstallDir.Location = new System.Drawing.Point(0, 64);
            this.panelCMInstallDir.Name = "panelCMInstallDir";
            this.panelCMInstallDir.Size = new System.Drawing.Size(550, 86);
            this.panelCMInstallDir.TabIndex = 29;
            // 
            // cmInstallDirBrowseButton
            // 
            this.cmInstallDirBrowseButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(186)))), ((int)(((byte)(163)))));
            this.cmInstallDirBrowseButton.BorderColor = System.Drawing.Color.Transparent;
            this.cmInstallDirBrowseButton.BorderWidth = 2;
            this.cmInstallDirBrowseButton.ButtonText = "";
            this.cmInstallDirBrowseButton.Disabled = false;
            this.cmInstallDirBrowseButton.EndColor = System.Drawing.Color.Yellow;
            this.cmInstallDirBrowseButton.FlatAppearance.BorderSize = 0;
            this.cmInstallDirBrowseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmInstallDirBrowseButton.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmInstallDirBrowseButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.cmInstallDirBrowseButton.GradientAngle = 90;
            this.cmInstallDirBrowseButton.Location = new System.Drawing.Point(452, 52);
            this.cmInstallDirBrowseButton.Name = "cmInstallDirBrowseButton";
            this.cmInstallDirBrowseButton.ShowButtontext = true;
            this.cmInstallDirBrowseButton.Size = new System.Drawing.Size(78, 28);
            this.cmInstallDirBrowseButton.StartColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(186)))), ((int)(((byte)(163)))));
            this.cmInstallDirBrowseButton.TabIndex = 21;
            this.cmInstallDirBrowseButton.Text = "Browse";
            this.cmInstallDirBrowseButton.TextLocation_X = 100;
            this.cmInstallDirBrowseButton.TextLocation_Y = 25;
            this.cmInstallDirBrowseButton.UseVisualStyleBackColor = false;
            this.cmInstallDirBrowseButton.Click += new System.EventHandler(this.cmInstallDirBrowseButton_Click);
            // 
            // txtCMInstallDir
            // 
            this.txtCMInstallDir.Font = new System.Drawing.Font("Source Sans Pro", 10.5F);
            this.txtCMInstallDir.Location = new System.Drawing.Point(160, 52);
            this.txtCMInstallDir.Name = "txtCMInstallDir";
            this.txtCMInstallDir.Size = new System.Drawing.Size(284, 25);
            this.txtCMInstallDir.TabIndex = 20;
            this.txtCMInstallDir.Text = "C:\\Program Files\\IDERA\\SQLcompliance";
            // 
            // labelCMInstallDir
            // 
            this.labelCMInstallDir.AutoSize = true;
            this.labelCMInstallDir.BackColor = System.Drawing.Color.Transparent;
            this.labelCMInstallDir.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCMInstallDir.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelCMInstallDir.Location = new System.Drawing.Point(20, 54);
            this.labelCMInstallDir.Name = "labelCMInstallDir";
            this.labelCMInstallDir.Size = new System.Drawing.Size(122, 18);
            this.labelCMInstallDir.TabIndex = 19;
            this.labelCMInstallDir.Text = "Installation Folder:";
            // 
            // panelSQLCMHeader
            // 
            this.panelSQLCMHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(236)))), ((int)(((byte)(236)))));
            this.panelSQLCMHeader.Controls.Add(this.labelSQLCMHeader);
            this.panelSQLCMHeader.Location = new System.Drawing.Point(0, 0);
            this.panelSQLCMHeader.Name = "panelSQLCMHeader";
            this.panelSQLCMHeader.Size = new System.Drawing.Size(550, 38);
            this.panelSQLCMHeader.TabIndex = 18;
            // 
            // labelSQLCMHeader
            // 
            this.labelSQLCMHeader.AutoSize = true;
            this.labelSQLCMHeader.BackColor = System.Drawing.Color.Transparent;
            this.labelSQLCMHeader.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSQLCMHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelSQLCMHeader.Location = new System.Drawing.Point(20, 10);
            this.labelSQLCMHeader.Name = "labelSQLCMHeader";
            this.labelSQLCMHeader.Size = new System.Drawing.Size(162, 18);
            this.labelSQLCMHeader.TabIndex = 0;
            this.labelSQLCMHeader.Text = Constants.ProductMap[Products.Compliance];
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
            this.labelSubHeader.Font = new System.Drawing.Font("Source Sans Pro", 10.5F);
            this.labelSubHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelSubHeader.Location = new System.Drawing.Point(20, 20);
            this.labelSubHeader.Name = "labelSubHeader";
            this.labelSubHeader.Size = new System.Drawing.Size(473, 18);
            this.labelSubHeader.TabIndex = 0;
            this.labelSubHeader.Text = Constants.InstallCMAndDashboardInstallDirScreen;
            // 
            // PageInstallationDirectory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.panelDashboardHeader);
            this.Controls.Add(this.panelDisplayname);
            this.Controls.Add(this.panelDashboardInstallDir);
            this.Controls.Add(this.panelCMInstallDir);
            this.Controls.Add(this.panelSubHeaderPanel);
            this.Controls.Add(this.toolTipDisplayName);
            this.Controls.Add(this.toolTipRemoteDashboard);
            this.Name = "PageInstallationDirectory";
            this.panelDashboardHeader.ResumeLayout(false);
            this.panelDashboardHeader.PerformLayout();
            this.panelDisplayname.ResumeLayout(false);
            this.panelDisplayname.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CMDisplayNameInfoPicture)).EndInit();
            this.panelDashboardInstallDir.ResumeLayout(false);
            this.panelDashboardInstallDir.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.remoteDashboardInfoPicture)).EndInit();
            this.panelCMInstallDir.ResumeLayout(false);
            this.panelCMInstallDir.PerformLayout();
            this.panelSQLCMHeader.ResumeLayout(false);
            this.panelSQLCMHeader.PerformLayout();
            this.panelSubHeaderPanel.ResumeLayout(false);
            this.panelSubHeaderPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelSubHeaderPanel;
        private Custom_Controls.IderaLabel labelSubHeader;
        private System.Windows.Forms.Panel panelCMInstallDir;
        private Custom_Controls.IderaButton cmInstallDirBrowseButton;
        private Custom_Controls.IderaTextBox txtCMInstallDir;
        private Custom_Controls.IderaLabel labelCMInstallDir;
        private System.Windows.Forms.Panel panelSQLCMHeader;
        private Custom_Controls.IderaHeaderLabel labelSQLCMHeader;
        private System.Windows.Forms.Panel panelDashboardInstallDir;
        private Custom_Controls.IderaTextBox txtRemoteDashboardPassword;
        private Custom_Controls.IderaTextBox txtRemoteDashboardUserName;
        private Custom_Controls.IderaLabel labelRemoteDashboardPassword;
        private Custom_Controls.IderaLabel labelRemoteDashboardUserName;
        private Custom_Controls.IderaLabel labelRemoteDashboardCred;
        private Custom_Controls.IderaTextBox txtRemoteDashboardUrl;
        private System.Windows.Forms.PictureBox remoteDashboardInfoPicture;
        private System.Windows.Forms.Panel panelDisplayname;
        private Custom_Controls.IderaTextBox txtDisplayName;
        private System.Windows.Forms.PictureBox CMDisplayNameInfoPicture;
        private Custom_Controls.IderaLabel labelDisplayName;
        private System.Windows.Forms.Panel panelDashboardHeader;
        private Custom_Controls.IderaHeaderLabel labelDashboardHeader;
        private Custom_Controls.IderaButton dashboardInstallDirBrowseButton;
        private Custom_Controls.IderaTextBox txtDashboardInstallDir;
        private Custom_Controls.IderaRadioButton radioDashboardLocal;
        private Custom_Controls.IderaRadioButton radioDashboardRemote;
        private Custom_Controls.IderaToolTip toolTipDisplayName;
        private Custom_Controls.IderaToolTip toolTipRemoteDashboard;
    }
}
