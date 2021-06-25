namespace SQLCM_Installer.Custom_Controls
{
    partial class PageFinish
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
            this.ideraLabel1 = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.panelSubHeaderPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelSubHeaderPanel
            // 
            this.panelSubHeaderPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.panelSubHeaderPanel.Controls.Add(this.ideraLabel1);
            this.panelSubHeaderPanel.Location = new System.Drawing.Point(0, 0);
            this.panelSubHeaderPanel.Name = "panelSubHeaderPanel";
            this.panelSubHeaderPanel.Size = new System.Drawing.Size(550, 64);
            this.panelSubHeaderPanel.TabIndex = 12;
            // 
            // ideraLabel1
            // 
            this.ideraLabel1.AutoSize = true;
            this.ideraLabel1.BackColor = System.Drawing.Color.Transparent;
            this.ideraLabel1.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ideraLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.ideraLabel1.Location = new System.Drawing.Point(20, 23);
            this.ideraLabel1.Name = "ideraLabel1";
            this.ideraLabel1.Size = new System.Drawing.Size(482, 15);
            this.ideraLabel1.TabIndex = 0;
            this.ideraLabel1.Text = "IDERA " + Constants.ProductMap[Products.Compliance] + " installation has completed successfully!";
            // 
            // PageFinish
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.panelSubHeaderPanel);
            this.Name = "PageFinish";
            this.Size = new System.Drawing.Size(550, 486);
            this.panelSubHeaderPanel.ResumeLayout(false);
            this.panelSubHeaderPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelSubHeaderPanel;
        private IderaLabel ideraLabel1;
    }
}
