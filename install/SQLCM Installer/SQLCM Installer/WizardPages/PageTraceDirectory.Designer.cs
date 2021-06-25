namespace SQLCM_Installer.WizardPages
{
    partial class PageTraceDirectory
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PageTraceDirectory));
            this.panelSubHeaderPanel = new System.Windows.Forms.Panel();
            this.labelSubHeader = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.panelCollectionTrace = new System.Windows.Forms.Panel();
            this.labelCollectionTraceDesc = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.collectionBrowseButton = new SQLCM_Installer.Custom_Controls.IderaButton();
            this.txtCollectionTraceDirectory = new SQLCM_Installer.Custom_Controls.IderaTextBox();
            this.labelCollectionTraceHeader = new SQLCM_Installer.Custom_Controls.IderaHeaderLabel();
            this.panelAgentTrace = new System.Windows.Forms.Panel();
            this.ideraLabel2 = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.agentBrowseButton = new SQLCM_Installer.Custom_Controls.IderaButton();
            this.txtAgentTraceDirectory = new SQLCM_Installer.Custom_Controls.IderaTextBox();
            this.labelAgentTraceHeader = new SQLCM_Installer.Custom_Controls.IderaHeaderLabel();
            this.panelSubHeaderPanel.SuspendLayout();
            this.panelCollectionTrace.SuspendLayout();
            this.panelAgentTrace.SuspendLayout();
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
            this.labelSubHeader.Font = new System.Drawing.Font("Source Sans Pro", 10.5F);
            this.labelSubHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelSubHeader.Location = new System.Drawing.Point(20, 18);
            this.labelSubHeader.Name = "labelSubHeader";
            this.labelSubHeader.Size = new System.Drawing.Size(354, 18);
            this.labelSubHeader.TabIndex = 0;
            this.labelSubHeader.Text = "Where do you want the Collection Service Trace Directory?";
            // 
            // panelCollectionTrace
            // 
            this.panelCollectionTrace.Controls.Add(this.labelCollectionTraceDesc);
            this.panelCollectionTrace.Controls.Add(this.collectionBrowseButton);
            this.panelCollectionTrace.Controls.Add(this.txtCollectionTraceDirectory);
            this.panelCollectionTrace.Controls.Add(this.labelCollectionTraceHeader);
            this.panelCollectionTrace.Location = new System.Drawing.Point(0, 64);
            this.panelCollectionTrace.Name = "panelCollectionTrace";
            this.panelCollectionTrace.Size = new System.Drawing.Size(550, 206);
            this.panelCollectionTrace.TabIndex = 12;
            // 
            // labelCollectionTraceDesc
            // 
            this.labelCollectionTraceDesc.AutoSize = true;
            this.labelCollectionTraceDesc.BackColor = System.Drawing.Color.Transparent;
            this.labelCollectionTraceDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelCollectionTraceDesc.Location = new System.Drawing.Point(23, 86);
            this.labelCollectionTraceDesc.Name = "labelCollectionTraceDesc";
            this.labelCollectionTraceDesc.Size = new System.Drawing.Size(383, 78);
            this.labelCollectionTraceDesc.TabIndex = 17;
            this.labelCollectionTraceDesc.Text = resources.GetString("labelCollectionTraceDesc.Text");
            // 
            // collectionBrowseButton
            // 
            this.collectionBrowseButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(186)))), ((int)(((byte)(163)))));
            this.collectionBrowseButton.BorderColor = System.Drawing.Color.Transparent;
            this.collectionBrowseButton.BorderWidth = 2;
            this.collectionBrowseButton.ButtonText = "";
            this.collectionBrowseButton.Disabled = false;
            this.collectionBrowseButton.EndColor = System.Drawing.Color.Yellow;
            this.collectionBrowseButton.FlatAppearance.BorderSize = 0;
            this.collectionBrowseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.collectionBrowseButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.collectionBrowseButton.GradientAngle = 90;
            this.collectionBrowseButton.Location = new System.Drawing.Point(452, 49);
            this.collectionBrowseButton.Name = "collectionBrowseButton";
            this.collectionBrowseButton.ShowButtontext = true;
            this.collectionBrowseButton.Size = new System.Drawing.Size(78, 28);
            this.collectionBrowseButton.StartColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(186)))), ((int)(((byte)(163)))));
            this.collectionBrowseButton.TabIndex = 16;
            this.collectionBrowseButton.Text = "Browse";
            this.collectionBrowseButton.TextLocation_X = 100;
            this.collectionBrowseButton.TextLocation_Y = 25;
            this.collectionBrowseButton.UseVisualStyleBackColor = false;
            this.collectionBrowseButton.Click += new System.EventHandler(this.collectionBrowseButton_Click);
            // 
            // txtCollectionTraceDirectory
            // 
            this.txtCollectionTraceDirectory.Location = new System.Drawing.Point(23, 49);
            this.txtCollectionTraceDirectory.Name = "txtCollectionTraceDirectory";
            this.txtCollectionTraceDirectory.Size = new System.Drawing.Size(422, 20);
            this.txtCollectionTraceDirectory.TabIndex = 15;
            // 
            // labelCollectionTraceHeader
            // 
            this.labelCollectionTraceHeader.AutoSize = true;
            this.labelCollectionTraceHeader.BackColor = System.Drawing.Color.Transparent;
            this.labelCollectionTraceHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(54)))), ((int)(((byte)(54)))));
            this.labelCollectionTraceHeader.Location = new System.Drawing.Point(20, 20);
            this.labelCollectionTraceHeader.Name = "labelCollectionTraceHeader";
            this.labelCollectionTraceHeader.Size = new System.Drawing.Size(203, 13);
            this.labelCollectionTraceHeader.TabIndex = 14;
            this.labelCollectionTraceHeader.Text = "Collection Service Trace Directory Folder:";
            // 
            // panelAgentTrace
            // 
            this.panelAgentTrace.Controls.Add(this.ideraLabel2);
            this.panelAgentTrace.Controls.Add(this.agentBrowseButton);
            this.panelAgentTrace.Controls.Add(this.txtAgentTraceDirectory);
            this.panelAgentTrace.Controls.Add(this.labelAgentTraceHeader);
            this.panelAgentTrace.Location = new System.Drawing.Point(0, 270);
            this.panelAgentTrace.Name = "panelAgentTrace";
            this.panelAgentTrace.Size = new System.Drawing.Size(550, 206);
            this.panelAgentTrace.TabIndex = 11;
            // 
            // ideraLabel2
            // 
            this.ideraLabel2.AutoSize = true;
            this.ideraLabel2.BackColor = System.Drawing.Color.Transparent;
            this.ideraLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.ideraLabel2.Location = new System.Drawing.Point(23, 87);
            this.ideraLabel2.Name = "ideraLabel2";
            this.ideraLabel2.Size = new System.Drawing.Size(394, 78);
            this.ideraLabel2.TabIndex = 21;
            this.ideraLabel2.Text = resources.GetString("ideraLabel2.Text");
            // 
            // agentBrowseButton
            // 
            this.agentBrowseButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(186)))), ((int)(((byte)(163)))));
            this.agentBrowseButton.BorderColor = System.Drawing.Color.Transparent;
            this.agentBrowseButton.BorderWidth = 2;
            this.agentBrowseButton.ButtonText = "";
            this.agentBrowseButton.Disabled = false;
            this.agentBrowseButton.EndColor = System.Drawing.Color.Yellow;
            this.agentBrowseButton.FlatAppearance.BorderSize = 0;
            this.agentBrowseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.agentBrowseButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.agentBrowseButton.GradientAngle = 90;
            this.agentBrowseButton.Location = new System.Drawing.Point(452, 50);
            this.agentBrowseButton.Name = "agentBrowseButton";
            this.agentBrowseButton.ShowButtontext = true;
            this.agentBrowseButton.Size = new System.Drawing.Size(78, 28);
            this.agentBrowseButton.StartColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(186)))), ((int)(((byte)(163)))));
            this.agentBrowseButton.TabIndex = 20;
            this.agentBrowseButton.Text = "Browse";
            this.agentBrowseButton.TextLocation_X = 100;
            this.agentBrowseButton.TextLocation_Y = 25;
            this.agentBrowseButton.UseVisualStyleBackColor = false;
            this.agentBrowseButton.Click += new System.EventHandler(this.agentBrowseButton_Click);
            // 
            // txtAgentTraceDirectory
            // 
            this.txtAgentTraceDirectory.Location = new System.Drawing.Point(23, 50);
            this.txtAgentTraceDirectory.Name = "txtAgentTraceDirectory";
            this.txtAgentTraceDirectory.Size = new System.Drawing.Size(422, 20);
            this.txtAgentTraceDirectory.TabIndex = 19;
            // 
            // labelAgentTraceHeader
            // 
            this.labelAgentTraceHeader.AutoSize = true;
            this.labelAgentTraceHeader.BackColor = System.Drawing.Color.Transparent;
            this.labelAgentTraceHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(54)))), ((int)(((byte)(54)))));
            this.labelAgentTraceHeader.Location = new System.Drawing.Point(20, 21);
            this.labelAgentTraceHeader.Name = "labelAgentTraceHeader";
            this.labelAgentTraceHeader.Size = new System.Drawing.Size(273, 13);
            this.labelAgentTraceHeader.TabIndex = 18;
            this.labelAgentTraceHeader.Text = "" + Constants.ProductMap[Products.Agent] + " Trace Directory Folder:";
            // 
            // PageTraceDirectory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.panelAgentTrace);
            this.Controls.Add(this.panelCollectionTrace);
            this.Controls.Add(this.panelSubHeaderPanel);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Name = "PageTraceDirectory";
            this.panelSubHeaderPanel.ResumeLayout(false);
            this.panelSubHeaderPanel.PerformLayout();
            this.panelCollectionTrace.ResumeLayout(false);
            this.panelCollectionTrace.PerformLayout();
            this.panelAgentTrace.ResumeLayout(false);
            this.panelAgentTrace.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelSubHeaderPanel;
        private Custom_Controls.IderaLabel labelSubHeader;
        private System.Windows.Forms.Panel panelCollectionTrace;
        private Custom_Controls.IderaButton collectionBrowseButton;
        private Custom_Controls.IderaTextBox txtCollectionTraceDirectory;
        private Custom_Controls.IderaHeaderLabel labelCollectionTraceHeader;
        private Custom_Controls.IderaLabel labelCollectionTraceDesc;
        private System.Windows.Forms.Panel panelAgentTrace;
        private Custom_Controls.IderaLabel ideraLabel2;
        private Custom_Controls.IderaButton agentBrowseButton;
        private Custom_Controls.IderaTextBox txtAgentTraceDirectory;
        private Custom_Controls.IderaHeaderLabel labelAgentTraceHeader;
    }
}
