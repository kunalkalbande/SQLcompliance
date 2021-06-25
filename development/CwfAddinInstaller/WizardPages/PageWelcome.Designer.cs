namespace CwfAddinInstaller.WizardPages
{
    partial class PageWelcome
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PageWelcome));
            this.label1 = new System.Windows.Forms.Label();
            this.lnkIderaWiki = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(3, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(308, 174);
            this.label1.TabIndex = 0;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // lnkIderaWiki
            // 
            this.lnkIderaWiki.AutoSize = true;
            this.lnkIderaWiki.LinkArea = new System.Windows.Forms.LinkArea(0, 41);
            this.lnkIderaWiki.Location = new System.Drawing.Point(3, 166);
            this.lnkIderaWiki.Name = "lnkIderaWiki";
            this.lnkIderaWiki.Size = new System.Drawing.Size(215, 13);
            this.lnkIderaWiki.TabIndex = 1;
            this.lnkIderaWiki.TabStop = true;
            this.lnkIderaWiki.Text = "SQL Compliance Manager Installation Guide";
            this.lnkIderaWiki.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkIderaWiki_LinkClicked);
            // 
            // PageWelcome
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lnkIderaWiki);
            this.Controls.Add(this.label1);
            this.Name = "PageWelcome";
            this.Load += new System.EventHandler(this.PageWelcome_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel lnkIderaWiki;
    }
}
