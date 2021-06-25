namespace SQLCM_Installer.WizardPages
{
    partial class PageAgentSQLServer
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
            this.labelWindowsUserName = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.radioSQLServerAuth = new SQLCM_Installer.Custom_Controls.IderaRadioButton();
            this.radioWindowsAuth = new SQLCM_Installer.Custom_Controls.IderaRadioButton();
            this.dropdownSQLServerInstance = new SQLCM_Installer.Custom_Controls.IderaDropDown();
            this.labelSQLServerInstance = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.labelSQLServerInstanceHeaderDesc = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.panelSubHeaderPanel = new System.Windows.Forms.Panel();
            this.labelSubHeader = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.linkLabelChangeCred = new System.Windows.Forms.LinkLabel();
            this.labelSQLUserName = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.panelSubHeaderPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelWindowsUserName
            // 
            this.labelWindowsUserName.AutoSize = true;
            this.labelWindowsUserName.BackColor = System.Drawing.Color.Transparent;
            this.labelWindowsUserName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelWindowsUserName.Location = new System.Drawing.Point(220, 201);
            this.labelWindowsUserName.Name = "labelWindowsUserName";
            this.labelWindowsUserName.Size = new System.Drawing.Size(0, 13);
            this.labelWindowsUserName.TabIndex = 18;
            // 
            // radioSQLServerAuth
            // 
            this.radioSQLServerAuth.AutoSize = true;
            this.radioSQLServerAuth.Checked = false;
            this.radioSQLServerAuth.Controls.Add(this.labelSQLUserName);
            this.radioSQLServerAuth.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioSQLServerAuth.Location = new System.Drawing.Point(23, 237);
            this.radioSQLServerAuth.Name = "radioSQLServerAuth";
            this.radioSQLServerAuth.Size = new System.Drawing.Size(308, 25);
            this.radioSQLServerAuth.TabIndex = 17;
            this.radioSQLServerAuth.Text = "Use Microsoft SQL Server Authentication: ";
            this.radioSQLServerAuth.Click += new System.EventHandler(this.radioSQLServerAuth_Click);
            // 
            // radioWindowsAuth
            // 
            this.radioWindowsAuth.AutoSize = true;
            this.radioWindowsAuth.Checked = true;
            this.radioWindowsAuth.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioWindowsAuth.Location = new System.Drawing.Point(22, 199);
            this.radioWindowsAuth.Name = "radioWindowsAuth";
            this.radioWindowsAuth.Size = new System.Drawing.Size(222, 25);
            this.radioWindowsAuth.TabIndex = 16;
            this.radioWindowsAuth.Text = "Use Windows Authentication:";
            // 
            // dropdownSQLServerInstance
            // 
            this.dropdownSQLServerInstance.HighlightColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(96)))), ((int)(((byte)(137)))));
            this.dropdownSQLServerInstance.Location = new System.Drawing.Point(172, 147);
            this.dropdownSQLServerInstance.Name = "dropdownSQLServerInstance";
            this.dropdownSQLServerInstance.Size = new System.Drawing.Size(287, 32);
            this.dropdownSQLServerInstance.TabIndex = 14;
            this.dropdownSQLServerInstance.Leave += new System.EventHandler(this.dropdownSQLServerInstance_Leave);
            // 
            // labelSQLServerInstance
            // 
            this.labelSQLServerInstance.AutoSize = true;
            this.labelSQLServerInstance.BackColor = System.Drawing.Color.Transparent;
            this.labelSQLServerInstance.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSQLServerInstance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelSQLServerInstance.Location = new System.Drawing.Point(19, 149);
            this.labelSQLServerInstance.Name = "labelSQLServerInstance";
            this.labelSQLServerInstance.Size = new System.Drawing.Size(129, 18);
            this.labelSQLServerInstance.TabIndex = 13;
            this.labelSQLServerInstance.Text = "SQL Server Instance:";
            // 
            // labelSQLServerInstanceHeaderDesc
            // 
            this.labelSQLServerInstanceHeaderDesc.AutoSize = true;
            this.labelSQLServerInstanceHeaderDesc.BackColor = System.Drawing.Color.Transparent;
            this.labelSQLServerInstanceHeaderDesc.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSQLServerInstanceHeaderDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelSQLServerInstanceHeaderDesc.Location = new System.Drawing.Point(20, 78);
            this.labelSQLServerInstanceHeaderDesc.Name = "labelSQLServerInstanceHeaderDesc";
            this.labelSQLServerInstanceHeaderDesc.Size = new System.Drawing.Size(445, 54);
            this.labelSQLServerInstanceHeaderDesc.TabIndex = 12;
            this.labelSQLServerInstanceHeaderDesc.Text = "You can register a single SQL Server instance. To audit more than one SQL \r\nServe" +
    "r instance on this computer, register additional instances using the \r\nManagemen" +
    "t Console.";
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
            this.labelSubHeader.Location = new System.Drawing.Point(19, 15);
            this.labelSubHeader.Name = "labelSubHeader";
            this.labelSubHeader.Size = new System.Drawing.Size(459, 36);
            this.labelSubHeader.TabIndex = 0;
            this.labelSubHeader.Text = "What SQL Server instance do you want to audit? Registering a SQL Server \r\ninstanc" +
    "e allows you to audit SQL events";
            // 
            // linkLabelChangeCred
            // 
            this.linkLabelChangeCred.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(165)))), ((int)(((byte)(219)))));
            this.linkLabelChangeCred.AutoSize = true;
            this.linkLabelChangeCred.Font = new System.Drawing.Font("Source Sans Pro", 10.5F);
            this.linkLabelChangeCred.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.linkLabelChangeCred.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.linkLabelChangeCred.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(165)))), ((int)(((byte)(219)))));
            this.linkLabelChangeCred.Location = new System.Drawing.Point(424, 239);
            this.linkLabelChangeCred.Name = "linkLabelChangeCred";
            this.linkLabelChangeCred.Size = new System.Drawing.Size(53, 18);
            this.linkLabelChangeCred.TabIndex = 58;
            this.linkLabelChangeCred.TabStop = true;
            this.linkLabelChangeCred.Text = "Change";
            this.linkLabelChangeCred.Visible = false;
            this.linkLabelChangeCred.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelChangeCred_LinkClicked);
            // 
            // ideraLabel1
            // 
            this.labelSQLUserName.AutoSize = true;
            this.labelSQLUserName.BackColor = System.Drawing.Color.Transparent;
            this.labelSQLUserName.Disabled = false;
            this.labelSQLUserName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelSQLUserName.Location = new System.Drawing.Point(270, 2);
            this.labelSQLUserName.Name = "ideraLabel1";
            this.labelSQLUserName.Size = new System.Drawing.Size(0, 18);
            this.labelSQLUserName.TabIndex = 59;
            // 
            // PageAgentSQLServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.linkLabelChangeCred);
            this.Controls.Add(this.labelWindowsUserName);
            this.Controls.Add(this.radioSQLServerAuth);
            this.Controls.Add(this.radioWindowsAuth);
            this.Controls.Add(this.dropdownSQLServerInstance);
            this.Controls.Add(this.labelSQLServerInstance);
            this.Controls.Add(this.labelSQLServerInstanceHeaderDesc);
            this.Controls.Add(this.panelSubHeaderPanel);
            this.Name = "PageAgentSQLServer";
            this.panelSubHeaderPanel.ResumeLayout(false);
            this.panelSubHeaderPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelSubHeaderPanel;
        private Custom_Controls.IderaLabel labelSubHeader;
        private Custom_Controls.IderaLabel labelSQLServerInstanceHeaderDesc;
        private Custom_Controls.IderaLabel labelSQLServerInstance;
        private Custom_Controls.IderaDropDown dropdownSQLServerInstance;
        private Custom_Controls.IderaRadioButton radioWindowsAuth;
        private Custom_Controls.IderaRadioButton radioSQLServerAuth;
        private Custom_Controls.IderaLabel labelWindowsUserName;
        private System.Windows.Forms.LinkLabel linkLabelChangeCred;
        private Custom_Controls.IderaLabel labelSQLUserName;
    }
}
