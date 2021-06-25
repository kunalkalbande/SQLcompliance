namespace Installer_form_application
{
    partial class Introduction
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Introduction));
            this.button1 = new System.Windows.Forms.Button();
            this.labelHeading = new System.Windows.Forms.Label();
            this.linkLabelHere = new System.Windows.Forms.LinkLabel();
            this.linkLabelGuide = new System.Windows.Forms.LinkLabel();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // labelHeading
            // 
            resources.ApplyResources(this.labelHeading, "labelHeading");
            this.labelHeading.BackColor = System.Drawing.Color.Transparent;
            this.labelHeading.Name = "labelHeading";
            // 
            // linkLabelHere
            // 
            this.linkLabelHere.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.linkLabelHere, "linkLabelHere");
            this.linkLabelHere.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.linkLabelHere.Name = "linkLabelHere";
            this.linkLabelHere.TabStop = true;
            this.linkLabelHere.UseCompatibleTextRendering = true;
            this.linkLabelHere.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelHere_LinkClicked);
            // 
            // linkLabelGuide
            // 
            resources.ApplyResources(this.linkLabelGuide, "linkLabelGuide");
            this.linkLabelGuide.BackColor = System.Drawing.Color.Transparent;
            this.linkLabelGuide.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.linkLabelGuide.Name = "linkLabelGuide";
            this.linkLabelGuide.TabStop = true;
            this.linkLabelGuide.UseCompatibleTextRendering = true;
            this.linkLabelGuide.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelGuide_LinkClicked);
            // 
            // button2
            // 
            resources.ApplyResources(this.button2, "button2");
            this.button2.Name = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // button3
            // 
            resources.ApplyResources(this.button3, "button3");
            this.button3.Name = "button3";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // Introduction
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::Installer_form_application.Properties.Resources.Main_Background;
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.linkLabelGuide);
            this.Controls.Add(this.linkLabelHere);
            this.Controls.Add(this.labelHeading);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Introduction";
            this.Load += new System.EventHandler(this.Introduction_Load);
            this.Shown += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label labelHeading;
        private System.Windows.Forms.LinkLabel linkLabelHere;
        private System.Windows.Forms.LinkLabel linkLabelGuide;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
    }
}

