namespace SQLCM_Installer.WizardPages
{
    partial class PageAgentCollectionServer
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
            this.labelAgentServerName = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.txtAgentServerName = new SQLCM_Installer.Custom_Controls.IderaTextBox();
            this.panelSubHeaderPanel.SuspendLayout();
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
            this.labelSubHeader.Location = new System.Drawing.Point(20, 17);
            this.labelSubHeader.Name = "labelSubHeader";
            this.labelSubHeader.Size = new System.Drawing.Size(448, 36);
            this.labelSubHeader.TabIndex = 0;
            this.labelSubHeader.Text = "Specify the name of the server that is hosting " + Constants.ProductMap[Products.Compliance] + " \r\nCollectio" + "n Service";
            // 
            // labelAgentServerName
            // 
            this.labelAgentServerName.AutoSize = true;
            this.labelAgentServerName.BackColor = System.Drawing.Color.Transparent;
            this.labelAgentServerName.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelAgentServerName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelAgentServerName.Location = new System.Drawing.Point(20, 96);
            this.labelAgentServerName.Name = "labelAgentServerName";
            this.labelAgentServerName.Size = new System.Drawing.Size(87, 18);
            this.labelAgentServerName.TabIndex = 12;
            this.labelAgentServerName.Text = "Server Name:";
            // 
            // txtAgentServerName
            // 
            this.txtAgentServerName.Font = new System.Drawing.Font("Source Sans Pro", 10.5F);
            this.txtAgentServerName.Location = new System.Drawing.Point(127, 95);
            this.txtAgentServerName.Name = "txtAgentServerName";
            this.txtAgentServerName.Size = new System.Drawing.Size(287, 25);
            this.txtAgentServerName.TabIndex = 13;
            // 
            // PageAgentCollectionServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.txtAgentServerName);
            this.Controls.Add(this.labelAgentServerName);
            this.Controls.Add(this.panelSubHeaderPanel);
            this.Name = "PageAgentCollectionServer";
            this.panelSubHeaderPanel.ResumeLayout(false);
            this.panelSubHeaderPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelSubHeaderPanel;
        private Custom_Controls.IderaLabel labelSubHeader;
        private Custom_Controls.IderaLabel labelAgentServerName;
        private Custom_Controls.IderaTextBox txtAgentServerName;
    }
}
