using System.Drawing;
using System.Windows.Forms;
namespace SQLCM_Installer.WizardPages
{
    partial class PageIntroduction
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
            this.labelDescription3 = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.labelDescription2 = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.labelDescription1 = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.panelSubHeaderPanel = new System.Windows.Forms.Panel();
            this.labelSubHeader = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.linkLabelInstallationRequirements = new System.Windows.Forms.LinkLabel();
            this.panelSubHeaderPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelDescription3
            // 
            this.labelDescription3.AutoSize = true;
            this.labelDescription3.BackColor = System.Drawing.Color.Transparent;
            this.labelDescription3.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDescription3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelDescription3.Location = new System.Drawing.Point(20, 362);
            this.labelDescription3.Name = "labelDescription3";
            this.labelDescription3.Size = new System.Drawing.Size(521, 36);
            this.labelDescription3.TabIndex = 11;
            this.labelDescription3.Text = "If you are auditing a cluster, your cluster configuration file will be placed in " +
    "your \r\n" + Constants.ProductMap[Products.Console] + " installation directory.";
            // 
            // labelDescription2
            // 
            this.labelDescription2.AutoSize = true;
            this.labelDescription2.BackColor = System.Drawing.Color.Transparent;
            this.labelDescription2.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDescription2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelDescription2.Location = new System.Drawing.Point(20, 148);
            this.labelDescription2.Name = "labelDescription2";
            this.labelDescription2.Size = new System.Drawing.Size(404, 18);
            this.labelDescription2.TabIndex = 10;
            this.labelDescription2.Text = "2.  Valid Service Account credentials for the IDERA Services.";
            // 
            // labelDescription1
            // 
            this.labelDescription1.AutoSize = true;
            this.labelDescription1.BackColor = System.Drawing.Color.Transparent;
            this.labelDescription1.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDescription1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelDescription1.Location = new System.Drawing.Point(20, 94);
            this.labelDescription1.Name = "labelDescription1";
            this.labelDescription1.Size = new System.Drawing.Size(486, 38);
            this.labelDescription1.TabIndex = 9;
            this.labelDescription1.Text = "1.  A SQL Server instance to host the IDERA " + Constants.ProductMap[Products.Compliance] + " and the \r\n    " +
    " " + Constants.ProductMap[Products.Dashboard] + " repository databases.";
            // 
            // panelSubHeaderPanel
            // 
            this.panelSubHeaderPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.panelSubHeaderPanel.Controls.Add(this.labelSubHeader);
            this.panelSubHeaderPanel.Location = new System.Drawing.Point(0, -1);
            this.panelSubHeaderPanel.Name = "panelSubHeaderPanel";
            this.panelSubHeaderPanel.Size = new System.Drawing.Size(550, 64);
            this.panelSubHeaderPanel.TabIndex = 8;
            // 
            // labelSubHeader
            // 
            this.labelSubHeader.AutoSize = true;
            this.labelSubHeader.BackColor = System.Drawing.Color.Transparent;
            this.labelSubHeader.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSubHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelSubHeader.Location = new System.Drawing.Point(20, 20);
            this.labelSubHeader.Name = "labelSubHeader";
            this.labelSubHeader.Size = new System.Drawing.Size(467, 18);
            this.labelSubHeader.TabIndex = 0;
            this.labelSubHeader.Text = "What you need to install or upgrade IDERA " + Constants.ProductMap[Products.Compliance];
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
            this.linkLabelInstallationRequirements.Location = new System.Drawing.Point(23, 420);
            this.linkLabelInstallationRequirements.Name = "linkLabelInstallationRequirements";
            this.linkLabelInstallationRequirements.Size = new System.Drawing.Size(416, 22);
            this.linkLabelInstallationRequirements.TabIndex = 12;
            this.linkLabelInstallationRequirements.TabStop = true;
            this.linkLabelInstallationRequirements.Text = "Need Help? Click here for detailed installation requirements.";
            this.linkLabelInstallationRequirements.UseCompatibleTextRendering = true;
            this.linkLabelInstallationRequirements.VisitedLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(165)))), ((int)(((byte)(219)))));
            this.linkLabelInstallationRequirements.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelInstallationRequirements_Click);
            // 
            // PageIntroduction
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.linkLabelInstallationRequirements);
            this.Controls.Add(this.labelDescription3);
            this.Controls.Add(this.labelDescription2);
            this.Controls.Add(this.labelDescription1);
            this.Controls.Add(this.panelSubHeaderPanel);
            this.Name = "PageIntroduction";
            this.panelSubHeaderPanel.ResumeLayout(false);
            this.panelSubHeaderPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Custom_Controls.IderaLabel labelDescription3;
        private Custom_Controls.IderaLabel labelDescription2;
        private Custom_Controls.IderaLabel labelDescription1;
        private System.Windows.Forms.Panel panelSubHeaderPanel;
        private Custom_Controls.IderaLabel labelSubHeader;
        private System.Windows.Forms.LinkLabel linkLabelInstallationRequirements;
    }
}
