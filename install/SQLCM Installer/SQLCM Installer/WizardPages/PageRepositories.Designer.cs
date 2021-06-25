namespace SQLCM_Installer.WizardPages
{
    partial class PageRepositories
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
            this.panelDashboardRepositoryHeader = new System.Windows.Forms.Panel();
            this.checkBoxSameCredAsCM = new SQLCM_Installer.Custom_Controls.IderaCheckBox();
            this.labelDashboardRepositoryHeader = new SQLCM_Installer.Custom_Controls.IderaHeaderLabel();
            this.panelDashboardRepository = new System.Windows.Forms.Panel();
            this.radioDashboardSQLServerAuth = new SQLCM_Installer.Custom_Controls.IderaRadioButton();
            this.txtDashboardUser = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.radioDashboardWindowAuth = new SQLCM_Installer.Custom_Controls.IderaRadioButton();
            this.labelDashboardDatabaseName = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.labelDashboardDatabaseNameValue = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.dropdownDashboardSQLServerInstance = new SQLCM_Installer.Custom_Controls.IderaDropDown();
            this.labelDashboardSQLServerInstance = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.panelSQLCMRepository = new System.Windows.Forms.Panel();
            this.panelSQLCMRepositoryHeader = new System.Windows.Forms.Panel();
            this.labelSQLCMRepositoryHeader = new SQLCM_Installer.Custom_Controls.IderaHeaderLabel();
            this.radioCMSQLServerAuth = new SQLCM_Installer.Custom_Controls.IderaRadioButton();
            this.txtCMUser = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.radioCMWindowAuth = new SQLCM_Installer.Custom_Controls.IderaRadioButton();
            this.labelCMDatabaseName = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.labelCMDatabaseNameValue = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.dropdownCMSQLServerInstance = new SQLCM_Installer.Custom_Controls.IderaDropDown();
            this.labelCMSQLServerInstance = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.panelNode = new System.Windows.Forms.Panel();
            this.radioActiveNode = new SQLCM_Installer.Custom_Controls.IderaRadioButton();
            this.radioPassiveNode = new SQLCM_Installer.Custom_Controls.IderaRadioButton();
            this.panelClusteredEnv = new System.Windows.Forms.Panel();
            this.checkBoxClusteredEnv = new SQLCM_Installer.Custom_Controls.IderaCheckBox();
            this.panelSubHeaderPanel = new System.Windows.Forms.Panel();
            this.labelSubHeader = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.linkLabelChangeCMCred = new System.Windows.Forms.LinkLabel();
            this.txtCMSQLServerUser = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.linkLabelChangeDashboardCred = new System.Windows.Forms.LinkLabel();
            this.txtDashboardSQLServerUser = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.panelDashboardRepositoryHeader.SuspendLayout();
            this.panelDashboardRepository.SuspendLayout();
            this.panelSQLCMRepository.SuspendLayout();
            this.panelSQLCMRepositoryHeader.SuspendLayout();
            this.panelNode.SuspendLayout();
            this.panelClusteredEnv.SuspendLayout();
            this.panelSubHeaderPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelDashboardRepositoryHeader
            // 
            this.panelDashboardRepositoryHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(236)))), ((int)(((byte)(236)))));
            this.panelDashboardRepositoryHeader.Controls.Add(this.checkBoxSameCredAsCM);
            this.panelDashboardRepositoryHeader.Controls.Add(this.labelDashboardRepositoryHeader);
            this.panelDashboardRepositoryHeader.Location = new System.Drawing.Point(0, 293);
            this.panelDashboardRepositoryHeader.Name = "panelDashboardRepositoryHeader";
            this.panelDashboardRepositoryHeader.Size = new System.Drawing.Size(550, 38);
            this.panelDashboardRepositoryHeader.TabIndex = 54;
            // 
            // checkBoxSameCredAsCM
            // 
            this.checkBoxSameCredAsCM.AutoSize = true;
            this.checkBoxSameCredAsCM.Checked = false;
            this.checkBoxSameCredAsCM.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxSameCredAsCM.Location = new System.Drawing.Point(252, 10);
            this.checkBoxSameCredAsCM.Name = "checkBoxSameCredAsCM";
            this.checkBoxSameCredAsCM.Size = new System.Drawing.Size(316, 25);
            this.checkBoxSameCredAsCM.TabIndex = 1;
            this.checkBoxSameCredAsCM.Text = "Same Instance and Authentication as above";
            this.checkBoxSameCredAsCM.Click += new System.EventHandler(this.checkBoxSameCredAsCM_Click);
            // 
            // labelDashboardRepositoryHeader
            // 
            this.labelDashboardRepositoryHeader.AutoSize = true;
            this.labelDashboardRepositoryHeader.BackColor = System.Drawing.Color.Transparent;
            this.labelDashboardRepositoryHeader.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDashboardRepositoryHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelDashboardRepositoryHeader.Location = new System.Drawing.Point(7, 10);
            this.labelDashboardRepositoryHeader.Name = "labelDashboardRepositoryHeader";
            this.labelDashboardRepositoryHeader.Size = new System.Drawing.Size(223, 18);
            this.labelDashboardRepositoryHeader.TabIndex = 0;
            this.labelDashboardRepositoryHeader.Text = "Create " + Constants.ProductMap[Products.Dashboard] + " Repository";
            // 
            // panelDashboardRepository
            // 
            this.panelDashboardRepository.Controls.Add(this.txtDashboardSQLServerUser);
            this.panelDashboardRepository.Controls.Add(this.linkLabelChangeDashboardCred);
            this.panelDashboardRepository.Controls.Add(this.radioDashboardSQLServerAuth);
            this.panelDashboardRepository.Controls.Add(this.txtDashboardUser);
            this.panelDashboardRepository.Controls.Add(this.radioDashboardWindowAuth);
            this.panelDashboardRepository.Controls.Add(this.labelDashboardDatabaseName);
            this.panelDashboardRepository.Controls.Add(this.labelDashboardDatabaseNameValue);
            this.panelDashboardRepository.Controls.Add(this.dropdownDashboardSQLServerInstance);
            this.panelDashboardRepository.Controls.Add(this.labelDashboardSQLServerInstance);
            this.panelDashboardRepository.Location = new System.Drawing.Point(3, 333);
            this.panelDashboardRepository.Name = "panelDashboardRepository";
            this.panelDashboardRepository.Size = new System.Drawing.Size(530, 138);
            this.panelDashboardRepository.TabIndex = 55;
            // 
            // radioDashboardSQLServerAuth
            // 
            this.radioDashboardSQLServerAuth.AutoSize = true;
            this.radioDashboardSQLServerAuth.Checked = false;
            this.radioDashboardSQLServerAuth.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioDashboardSQLServerAuth.Location = new System.Drawing.Point(20, 114);
            this.radioDashboardSQLServerAuth.Name = "radioDashboardSQLServerAuth";
            this.radioDashboardSQLServerAuth.TabIndex = 40;
            this.radioDashboardSQLServerAuth.Text = "Use Microsoft SQL Server Authentication: ";
            this.radioDashboardSQLServerAuth.Click += new System.EventHandler(this.radioDashboardSQLServerAuth_Click);
            // 
            // txtDashboardUser
            // 
            this.txtDashboardUser.AutoSize = true;
            this.txtDashboardUser.BackColor = System.Drawing.Color.Transparent;
            this.txtDashboardUser.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDashboardUser.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.txtDashboardUser.Location = new System.Drawing.Point(244, 88);
            this.txtDashboardUser.Name = "txtDashboardUser";
            this.txtDashboardUser.Size = new System.Drawing.Size(0, 18);
            this.txtDashboardUser.TabIndex = 39;
            // 
            // radioDashboardWindowAuth
            // 
            this.radioDashboardWindowAuth.AutoSize = true;
            this.radioDashboardWindowAuth.Checked = true;
            this.radioDashboardWindowAuth.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioDashboardWindowAuth.Location = new System.Drawing.Point(20, 85);
            this.radioDashboardWindowAuth.Name = "radioDashboardWindowAuth";
            this.radioDashboardWindowAuth.Size = new System.Drawing.Size(226, 25);
            this.radioDashboardWindowAuth.TabIndex = 38;
            this.radioDashboardWindowAuth.Text = "Use Windows Authentication: ";
            // 
            // labelDashboardDatabaseName
            // 
            this.labelDashboardDatabaseName.AutoSize = true;
            this.labelDashboardDatabaseName.BackColor = System.Drawing.Color.Transparent;
            this.labelDashboardDatabaseName.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDashboardDatabaseName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelDashboardDatabaseName.Location = new System.Drawing.Point(20, 56);
            this.labelDashboardDatabaseName.Name = "labelDashboardDatabaseName";
            this.labelDashboardDatabaseName.Size = new System.Drawing.Size(105, 18);
            this.labelDashboardDatabaseName.TabIndex = 37;
            this.labelDashboardDatabaseName.Text = "Database Name:";
            // 
            // labelDashboardDatabaseNameValue
            // 
            this.labelDashboardDatabaseNameValue.AutoSize = true;
            this.labelDashboardDatabaseNameValue.BackColor = System.Drawing.Color.Transparent;
            this.labelDashboardDatabaseNameValue.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDashboardDatabaseNameValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelDashboardDatabaseNameValue.Location = new System.Drawing.Point(174, 55);
            this.labelDashboardDatabaseNameValue.Name = "labelDashboardDatabaseNameValue";
            this.labelDashboardDatabaseNameValue.Size = new System.Drawing.Size(170, 18);
            this.labelDashboardDatabaseNameValue.TabIndex = 36;
            this.labelDashboardDatabaseNameValue.Text = "IderaDashboardRepository";
            // 
            // dropdownDashboardSQLServerInstance
            // 
            this.dropdownDashboardSQLServerInstance.HighlightColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(96)))), ((int)(((byte)(137)))));
            this.dropdownDashboardSQLServerInstance.Location = new System.Drawing.Point(177, 19);
            this.dropdownDashboardSQLServerInstance.Name = "dropdownDashboardSQLServerInstance";
            this.dropdownDashboardSQLServerInstance.Size = new System.Drawing.Size(287, 32);
            this.dropdownDashboardSQLServerInstance.TabIndex = 33;
            this.dropdownDashboardSQLServerInstance.Leave += new System.EventHandler(this.dropdownDashboardSQLServerInstance_Leave);
            // 
            // labelDashboardSQLServerInstance
            // 
            this.labelDashboardSQLServerInstance.AutoSize = true;
            this.labelDashboardSQLServerInstance.BackColor = System.Drawing.Color.Transparent;
            this.labelDashboardSQLServerInstance.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDashboardSQLServerInstance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelDashboardSQLServerInstance.Location = new System.Drawing.Point(20, 22);
            this.labelDashboardSQLServerInstance.Name = "labelDashboardSQLServerInstance";
            this.labelDashboardSQLServerInstance.Size = new System.Drawing.Size(129, 18);
            this.labelDashboardSQLServerInstance.TabIndex = 35;
            this.labelDashboardSQLServerInstance.Text = "SQL Server Instance:";
            // 
            // panelSQLCMRepository
            // 
            this.panelSQLCMRepository.Controls.Add(this.txtCMSQLServerUser);
            this.panelSQLCMRepository.Controls.Add(this.linkLabelChangeCMCred);
            this.panelSQLCMRepository.Controls.Add(this.panelSQLCMRepositoryHeader);
            this.panelSQLCMRepository.Controls.Add(this.radioCMSQLServerAuth);
            this.panelSQLCMRepository.Controls.Add(this.txtCMUser);
            this.panelSQLCMRepository.Controls.Add(this.radioCMWindowAuth);
            this.panelSQLCMRepository.Controls.Add(this.labelCMDatabaseName);
            this.panelSQLCMRepository.Controls.Add(this.labelCMDatabaseNameValue);
            this.panelSQLCMRepository.Controls.Add(this.dropdownCMSQLServerInstance);
            this.panelSQLCMRepository.Controls.Add(this.labelCMSQLServerInstance);
            this.panelSQLCMRepository.Location = new System.Drawing.Point(0, 112);
            this.panelSQLCMRepository.Name = "panelSQLCMRepository";
            this.panelSQLCMRepository.Size = new System.Drawing.Size(550, 177);
            this.panelSQLCMRepository.TabIndex = 53;
            // 
            // panelSQLCMRepositoryHeader
            // 
            this.panelSQLCMRepositoryHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(236)))), ((int)(((byte)(236)))));
            this.panelSQLCMRepositoryHeader.Controls.Add(this.labelSQLCMRepositoryHeader);
            this.panelSQLCMRepositoryHeader.Location = new System.Drawing.Point(0, 0);
            this.panelSQLCMRepositoryHeader.Name = "panelSQLCMRepositoryHeader";
            this.panelSQLCMRepositoryHeader.Size = new System.Drawing.Size(550, 38);
            this.panelSQLCMRepositoryHeader.TabIndex = 17;
            // 
            // labelSQLCMRepositoryHeader
            // 
            this.labelSQLCMRepositoryHeader.AutoSize = true;
            this.labelSQLCMRepositoryHeader.BackColor = System.Drawing.Color.Transparent;
            this.labelSQLCMRepositoryHeader.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSQLCMRepositoryHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelSQLCMRepositoryHeader.Location = new System.Drawing.Point(20, 10);
            this.labelSQLCMRepositoryHeader.Name = "labelSQLCMRepositoryHeader";
            this.labelSQLCMRepositoryHeader.Size = new System.Drawing.Size(269, 18);
            this.labelSQLCMRepositoryHeader.TabIndex = 0;
            this.labelSQLCMRepositoryHeader.Text = "Create " + Constants.ProductMap[Products.Compliance] + " Repository";
            // 
            // radioCMSQLServerAuth
            // 
            this.radioCMSQLServerAuth.AutoSize = true;
            this.radioCMSQLServerAuth.Checked = false;
            this.radioCMSQLServerAuth.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioCMSQLServerAuth.Location = new System.Drawing.Point(20, 150);
            this.radioCMSQLServerAuth.Name = "radioCMSQLServerAuth";
            this.radioCMSQLServerAuth.TabIndex = 25;
            this.radioCMSQLServerAuth.Text = "Use Microsoft SQL Server Authentication: ";
            this.radioCMSQLServerAuth.Click += new System.EventHandler(this.radioCMSQLServerAuth_Click);
            // 
            // txtCMUser
            // 
            this.txtCMUser.AutoSize = true;
            this.txtCMUser.BackColor = System.Drawing.Color.Transparent;
            this.txtCMUser.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCMUser.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.txtCMUser.Location = new System.Drawing.Point(244, 124);
            this.txtCMUser.Name = "txtCMUser";
            this.txtCMUser.Size = new System.Drawing.Size(0, 18);
            this.txtCMUser.TabIndex = 24;
            // 
            // radioCMWindowAuth
            // 
            this.radioCMWindowAuth.AutoSize = true;
            this.radioCMWindowAuth.Checked = true;
            this.radioCMWindowAuth.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioCMWindowAuth.Location = new System.Drawing.Point(20, 121);
            this.radioCMWindowAuth.Name = "radioCMWindowAuth";
            this.radioCMWindowAuth.Size = new System.Drawing.Size(226, 25);
            this.radioCMWindowAuth.TabIndex = 14;
            this.radioCMWindowAuth.Text = "Use Windows Authentication: ";
            // 
            // labelCMDatabaseName
            // 
            this.labelCMDatabaseName.AutoSize = true;
            this.labelCMDatabaseName.BackColor = System.Drawing.Color.Transparent;
            this.labelCMDatabaseName.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCMDatabaseName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelCMDatabaseName.Location = new System.Drawing.Point(20, 93);
            this.labelCMDatabaseName.Name = "labelCMDatabaseName";
            this.labelCMDatabaseName.Size = new System.Drawing.Size(105, 18);
            this.labelCMDatabaseName.TabIndex = 22;
            this.labelCMDatabaseName.Text = "Database Name:";
            // 
            // labelCMDatabaseNameValue
            // 
            this.labelCMDatabaseNameValue.AutoSize = true;
            this.labelCMDatabaseNameValue.BackColor = System.Drawing.Color.Transparent;
            this.labelCMDatabaseNameValue.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCMDatabaseNameValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelCMDatabaseNameValue.Location = new System.Drawing.Point(174, 92);
            this.labelCMDatabaseNameValue.Name = "labelCMDatabaseNameValue";
            this.labelCMDatabaseNameValue.Size = new System.Drawing.Size(100, 18);
            this.labelCMDatabaseNameValue.TabIndex = 21;
            this.labelCMDatabaseNameValue.Text = "SQLcompliance";
            // 
            // dropdownCMSQLServerInstance
            // 
            this.dropdownCMSQLServerInstance.HighlightColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(96)))), ((int)(((byte)(137)))));
            this.dropdownCMSQLServerInstance.Location = new System.Drawing.Point(177, 56);
            this.dropdownCMSQLServerInstance.Name = "dropdownCMSQLServerInstance";
            this.dropdownCMSQLServerInstance.Size = new System.Drawing.Size(287, 32);
            this.dropdownCMSQLServerInstance.TabIndex = 12;
            this.dropdownCMSQLServerInstance.Leave += new System.EventHandler(this.dropdownCMSQLServerInstance_Leave);
            this.dropdownCMSQLServerInstance.TextChanged +=dropdownCMSQLServerInstance_TextChanged;
            // 
            // labelCMSQLServerInstance
            // 
            this.labelCMSQLServerInstance.AutoSize = true;
            this.labelCMSQLServerInstance.BackColor = System.Drawing.Color.Transparent;
            this.labelCMSQLServerInstance.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCMSQLServerInstance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelCMSQLServerInstance.Location = new System.Drawing.Point(20, 57);
            this.labelCMSQLServerInstance.Name = "labelCMSQLServerInstance";
            this.labelCMSQLServerInstance.Size = new System.Drawing.Size(129, 18);
            this.labelCMSQLServerInstance.TabIndex = 18;
            this.labelCMSQLServerInstance.Text = "SQL Server Instance:";
            // 
            // panelNode
            // 
            this.panelNode.BackColor = System.Drawing.Color.White;
            this.panelNode.Controls.Add(this.radioActiveNode);
            this.panelNode.Controls.Add(this.radioPassiveNode);
            this.panelNode.Location = new System.Drawing.Point(0, 480);
            this.panelNode.Name = "panelNode";
            this.panelNode.Size = new System.Drawing.Size(533, 33);
            this.panelNode.TabIndex = 52;
            this.panelNode.Visible = false;
            // 
            // radioActiveNode
            // 
            this.radioActiveNode.AutoSize = true;
            this.radioActiveNode.Checked = true;
            this.radioActiveNode.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioActiveNode.Location = new System.Drawing.Point(53, 4);
            this.radioActiveNode.Name = "radioActiveNode";
            this.radioActiveNode.Size = new System.Drawing.Size(108, 25);
            this.radioActiveNode.TabIndex = 12;
            this.radioActiveNode.Text = "Active Node";
            // 
            // radioPassiveNode
            // 
            this.radioPassiveNode.AutoSize = true;
            this.radioPassiveNode.Checked = false;
            this.radioPassiveNode.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioPassiveNode.Location = new System.Drawing.Point(227, 4);
            this.radioPassiveNode.Name = "radioPassiveNode";
            this.radioPassiveNode.Size = new System.Drawing.Size(121, 25);
            this.radioPassiveNode.TabIndex = 13;
            this.radioPassiveNode.Text = "Passive Node";
            // 
            // panelClusteredEnv
            // 
            this.panelClusteredEnv.Controls.Add(this.checkBoxClusteredEnv);
            this.panelClusteredEnv.Location = new System.Drawing.Point(0, 64);
            this.panelClusteredEnv.Name = "panelClusteredEnv";
            this.panelClusteredEnv.Size = new System.Drawing.Size(533, 48);
            this.panelClusteredEnv.TabIndex = 51;
            // 
            // checkBoxClusteredEnv
            // 
            this.checkBoxClusteredEnv.AutoSize = true;
            this.checkBoxClusteredEnv.Checked = false;
            this.checkBoxClusteredEnv.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxClusteredEnv.Location = new System.Drawing.Point(20, 13);
            this.checkBoxClusteredEnv.Name = "checkBoxClusteredEnv";
            this.checkBoxClusteredEnv.Size = new System.Drawing.Size(229, 25);
            this.checkBoxClusteredEnv.TabIndex = 11;
            this.checkBoxClusteredEnv.Text = "Enable Clustered Environment";
            this.checkBoxClusteredEnv.Click += new System.EventHandler(this.checkBoxClusteredEnv_Click);
            // 
            // panelSubHeaderPanel
            // 
            this.panelSubHeaderPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.panelSubHeaderPanel.Controls.Add(this.labelSubHeader);
            this.panelSubHeaderPanel.Location = new System.Drawing.Point(0, 0);
            this.panelSubHeaderPanel.Name = "panelSubHeaderPanel";
            this.panelSubHeaderPanel.Size = new System.Drawing.Size(550, 64);
            this.panelSubHeaderPanel.TabIndex = 50;
            // linkLabelChangeCMCred
            // 
            this.linkLabelChangeCMCred.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(165)))), ((int)(((byte)(219)))));
            this.linkLabelChangeCMCred.AutoSize = true;
            this.linkLabelChangeCMCred.Font = new System.Drawing.Font("Source Sans Pro", 10.5F);
            this.linkLabelChangeCMCred.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.linkLabelChangeCMCred.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.linkLabelChangeCMCred.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(165)))), ((int)(((byte)(219)))));
            this.linkLabelChangeCMCred.Location = new System.Drawing.Point(424, 152);
            this.linkLabelChangeCMCred.Name = "linkLabelChangeCMCred";
            this.linkLabelChangeCMCred.Size = new System.Drawing.Size(53, 18);
            this.linkLabelChangeCMCred.TabIndex = 57;
            this.linkLabelChangeCMCred.TabStop = true;
            this.linkLabelChangeCMCred.Text = "Change";
            this.linkLabelChangeCMCred.Visible = false;
            this.linkLabelChangeCMCred.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelChangeCMCred_LinkClicked);
            // 
            // txtCMSQLServerUser
            // 
            this.txtCMSQLServerUser.AutoSize = true;
            this.txtCMSQLServerUser.BackColor = System.Drawing.Color.Transparent;
            this.txtCMSQLServerUser.Disabled = false;
            this.txtCMSQLServerUser.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.txtCMSQLServerUser.Location = new System.Drawing.Point(285, 152);
            this.txtCMSQLServerUser.Name = "txtCMSQLServerUser";
            this.txtCMSQLServerUser.Size = new System.Drawing.Size(0, 25);
            this.txtCMSQLServerUser.TabIndex = 56;
            // linkLabelChangeDashboardCred
            // 
            this.linkLabelChangeDashboardCred.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(165)))), ((int)(((byte)(219)))));
            this.linkLabelChangeDashboardCred.AutoSize = true;
            this.linkLabelChangeDashboardCred.Font = new System.Drawing.Font("Source Sans Pro", 10.5F);
            this.linkLabelChangeDashboardCred.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.linkLabelChangeDashboardCred.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.linkLabelChangeDashboardCred.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(165)))), ((int)(((byte)(219)))));
            this.linkLabelChangeDashboardCred.Location = new System.Drawing.Point(424, 117);
            this.linkLabelChangeDashboardCred.Name = "linkLabelChangeDashboardCred";
            this.linkLabelChangeDashboardCred.Size = new System.Drawing.Size(53, 18);
            this.linkLabelChangeDashboardCred.TabIndex = 57;
            this.linkLabelChangeDashboardCred.TabStop = true;
            this.linkLabelChangeDashboardCred.Text = "Change";
            this.linkLabelChangeDashboardCred.Visible = false;
            this.linkLabelChangeDashboardCred.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelChangeDashboardCred_LinkClicked);
            // 
            // txtDashboardSQLServerUser
            // 
            this.txtDashboardSQLServerUser.AutoSize = true;
            this.txtDashboardSQLServerUser.BackColor = System.Drawing.Color.Transparent;
            this.txtDashboardSQLServerUser.Disabled = false;
            this.txtDashboardSQLServerUser.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.txtDashboardSQLServerUser.Location = new System.Drawing.Point(285, 116);
            this.txtDashboardSQLServerUser.Name = "txtDashboardSQLServerUser";
            this.txtDashboardSQLServerUser.Size = new System.Drawing.Size(0, 25);
            this.txtDashboardSQLServerUser.TabIndex = 56;
            // 
            // labelSubHeader
            // 
            this.labelSubHeader.AutoSize = true;
            this.labelSubHeader.BackColor = System.Drawing.Color.Transparent;
            this.labelSubHeader.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSubHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelSubHeader.Location = new System.Drawing.Point(12, 19);
            this.labelSubHeader.Name = "labelSubHeader";
            this.labelSubHeader.Size = new System.Drawing.Size(422, 36);
            this.labelSubHeader.TabIndex = 0;
            this.labelSubHeader.Text = "Do you want to enable a Clustered Environment?  What Instance and \r\nAuthenticatio" +
    "n do you want to use to create repositories?";
            // 
            // PageRepositories
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.panelDashboardRepositoryHeader);
            this.Controls.Add(this.panelDashboardRepository);
            this.Controls.Add(this.panelSQLCMRepository);
            this.Controls.Add(this.panelNode);
            this.Controls.Add(this.panelClusteredEnv);
            this.Controls.Add(this.panelSubHeaderPanel);
            this.Name = "PageRepositories";
            this.Size = new System.Drawing.Size(550, 486);
            this.panelDashboardRepositoryHeader.ResumeLayout(false);
            this.panelDashboardRepositoryHeader.PerformLayout();
            this.panelDashboardRepository.ResumeLayout(false);
            this.panelDashboardRepository.PerformLayout();
            this.panelSQLCMRepository.ResumeLayout(false);
            this.panelSQLCMRepository.PerformLayout();
            this.panelSQLCMRepositoryHeader.ResumeLayout(false);
            this.panelSQLCMRepositoryHeader.PerformLayout();
            this.panelNode.ResumeLayout(false);
            this.panelNode.PerformLayout();
            this.radioCMSQLServerAuth.ResumeLayout(false);
            this.radioCMSQLServerAuth.PerformLayout();
            this.panelClusteredEnv.ResumeLayout(false);
            this.radioDashboardSQLServerAuth.ResumeLayout(false);
            this.radioDashboardSQLServerAuth.PerformLayout();
            this.panelClusteredEnv.PerformLayout();
            this.panelSubHeaderPanel.ResumeLayout(false);
            this.panelSubHeaderPanel.PerformLayout();
            this.ResumeLayout(false);

        }
              

        #endregion

        private System.Windows.Forms.Panel panelDashboardRepositoryHeader;
        private Custom_Controls.IderaCheckBox checkBoxSameCredAsCM;
        private Custom_Controls.IderaHeaderLabel labelDashboardRepositoryHeader;
        private System.Windows.Forms.Panel panelDashboardRepository;
        private Custom_Controls.IderaRadioButton radioDashboardSQLServerAuth;
        private Custom_Controls.IderaLabel txtDashboardUser;
        private Custom_Controls.IderaRadioButton radioDashboardWindowAuth;
        private Custom_Controls.IderaLabel labelDashboardDatabaseName;
        private Custom_Controls.IderaLabel labelDashboardDatabaseNameValue;
        private Custom_Controls.IderaDropDown dropdownDashboardSQLServerInstance;
        private Custom_Controls.IderaLabel labelDashboardSQLServerInstance;
        private System.Windows.Forms.Panel panelSQLCMRepository;
        private System.Windows.Forms.Panel panelSQLCMRepositoryHeader;
        private Custom_Controls.IderaHeaderLabel labelSQLCMRepositoryHeader;
        private Custom_Controls.IderaRadioButton radioCMSQLServerAuth;
        private Custom_Controls.IderaLabel txtCMUser;
        private Custom_Controls.IderaRadioButton radioCMWindowAuth;
        private Custom_Controls.IderaLabel labelCMDatabaseName;
        private Custom_Controls.IderaLabel labelCMDatabaseNameValue;
        private Custom_Controls.IderaDropDown dropdownCMSQLServerInstance;
        private Custom_Controls.IderaLabel labelCMSQLServerInstance;
        private System.Windows.Forms.Panel panelNode;
        private Custom_Controls.IderaRadioButton radioPassiveNode;
        private Custom_Controls.IderaRadioButton radioActiveNode;
        private System.Windows.Forms.Panel panelClusteredEnv;
        private Custom_Controls.IderaCheckBox checkBoxClusteredEnv;
        private System.Windows.Forms.Panel panelSubHeaderPanel;
        private Custom_Controls.IderaLabel labelSubHeader;
        private System.Windows.Forms.LinkLabel linkLabelChangeCMCred;
        private Custom_Controls.IderaLabel txtCMSQLServerUser;
        private System.Windows.Forms.LinkLabel linkLabelChangeDashboardCred;
        private Custom_Controls.IderaLabel txtDashboardSQLServerUser;
    }
}
