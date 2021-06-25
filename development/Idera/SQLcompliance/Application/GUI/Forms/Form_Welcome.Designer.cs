namespace Idera.SQLcompliance.Application.GUI.Forms
{
    partial class Form_Welcome
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
           System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Welcome));
           this.label1 = new System.Windows.Forms.Label();
           this.label2 = new System.Windows.Forms.Label();
           this.button_Yes = new System.Windows.Forms.Button();
           this.button_No = new System.Windows.Forms.Button();
           this.pictureBox_Tagline = new System.Windows.Forms.PictureBox();
           this.panel1 = new System.Windows.Forms.Panel();
           this.pictureBox1 = new System.Windows.Forms.PictureBox();
           this.label3 = new System.Windows.Forms.Label();
           this.label_ProductsPageText = new System.Windows.Forms.Label();
           this.helpAudit = new System.Windows.Forms.LinkLabel();
           this.label4 = new System.Windows.Forms.Label();
           this.helpOptimize = new System.Windows.Forms.LinkLabel();
           ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Tagline)).BeginInit();
           this.panel1.SuspendLayout();
           ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
           this.SuspendLayout();
           // 
           // label1
           // 
           this.label1.AutoSize = true;
           this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
           this.label1.Location = new System.Drawing.Point(110, 61);
           this.label1.Name = "label1";
           this.label1.Size = new System.Drawing.Size(336, 24);
           this.label1.TabIndex = 2;
           this.label1.Text = "Welcome to SQL Compliance Manager";
           // 
           // label2
           // 
           this.label2.AutoSize = true;
           this.label2.Location = new System.Drawing.Point(110, 225);
           this.label2.Name = "label2";
           this.label2.Size = new System.Drawing.Size(327, 13);
           this.label2.TabIndex = 3;
           this.label2.Text = "To begin capturing user activity, first register a SQL Server instance.";
           // 
           // button_Yes
           // 
           this.button_Yes.Location = new System.Drawing.Point(201, 277);
           this.button_Yes.Name = "button_Yes";
           this.button_Yes.Size = new System.Drawing.Size(75, 23);
           this.button_Yes.TabIndex = 4;
           this.button_Yes.Text = "Yes";
           this.button_Yes.UseVisualStyleBackColor = true;
           this.button_Yes.Click += new System.EventHandler(this.button_Yes_Click);
           // 
           // button_No
           // 
           this.button_No.Location = new System.Drawing.Point(297, 277);
           this.button_No.Name = "button_No";
           this.button_No.Size = new System.Drawing.Size(75, 23);
           this.button_No.TabIndex = 5;
           this.button_No.Text = "No";
           this.button_No.UseVisualStyleBackColor = true;
           this.button_No.Click += new System.EventHandler(this.button_No_Click);
           // 
           // pictureBox_Tagline
           // 
           this.pictureBox_Tagline.BackColor = System.Drawing.Color.Transparent;
           this.pictureBox_Tagline.BackgroundImage = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.SQLcmHeader;
           this.pictureBox_Tagline.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
           this.pictureBox_Tagline.Location = new System.Drawing.Point(0, 0);
           this.pictureBox_Tagline.Name = "pictureBox_Tagline";
           this.pictureBox_Tagline.Size = new System.Drawing.Size(429, 50);
           this.pictureBox_Tagline.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
           this.pictureBox_Tagline.TabIndex = 6;
           this.pictureBox_Tagline.TabStop = false;
           // 
           // panel1
           // 
           this.panel1.Controls.Add(this.pictureBox_Tagline);
           this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
           this.panel1.Location = new System.Drawing.Point(0, 0);
           this.panel1.Name = "panel1";
           this.panel1.Size = new System.Drawing.Size(573, 50);
           this.panel1.TabIndex = 7;
           // 
           // pictureBox1
           // 
           this.pictureBox1.Image = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.WelcomeSideBar;
           this.pictureBox1.Location = new System.Drawing.Point(0, 51);
           this.pictureBox1.Name = "pictureBox1";
           this.pictureBox1.Size = new System.Drawing.Size(104, 261);
           this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
           this.pictureBox1.TabIndex = 8;
           this.pictureBox1.TabStop = false;
           // 
           // label3
           // 
           this.label3.AutoSize = true;
           this.label3.Location = new System.Drawing.Point(110, 247);
           this.label3.Name = "label3";
           this.label3.Size = new System.Drawing.Size(265, 13);
           this.label3.TabIndex = 9;
           this.label3.Text = "Would you like to register a SQL Server instance now?";
           // 
           // label_ProductsPageText
           // 
           this.label_ProductsPageText.Location = new System.Drawing.Point(110, 93);
           this.label_ProductsPageText.Name = "label_ProductsPageText";
           this.label_ProductsPageText.Size = new System.Drawing.Size(451, 55);
           this.label_ProductsPageText.TabIndex = 10;
           this.label_ProductsPageText.Text = resources.GetString("label_ProductsPageText.Text");
           // 
           // helpAudit
           // 
           this.helpAudit.AutoSize = true;
           this.helpAudit.Location = new System.Drawing.Point(130, 166);
           this.helpAudit.Name = "helpAudit";
           this.helpAudit.Size = new System.Drawing.Size(125, 13);
           this.helpAudit.TabIndex = 11;
           this.helpAudit.TabStop = true;
           this.helpAudit.Text = "Audit SQL Server Events";
           this.helpAudit.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.helpAudit_LinkClicked);
           // 
           // label4
           // 
           this.label4.AutoSize = true;
           this.label4.Location = new System.Drawing.Point(110, 145);
           this.label4.Name = "label4";
           this.label4.Size = new System.Drawing.Size(255, 13);
           this.label4.TabIndex = 12;
           this.label4.Text = "Get started by reviewing the following best practices.";
           // 
           // helpOptimize
           // 
           this.helpOptimize.AutoSize = true;
           this.helpOptimize.Location = new System.Drawing.Point(130, 185);
           this.helpOptimize.Name = "helpOptimize";
           this.helpOptimize.Size = new System.Drawing.Size(110, 13);
           this.helpOptimize.TabIndex = 14;
           this.helpOptimize.TabStop = true;
           this.helpOptimize.Text = "Optimize Performance";
           this.helpOptimize.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.helpOptimize_LinkClicked);
           // 
           // Form_Welcome
           // 
           this.BackColor = System.Drawing.Color.White;
           this.ClientSize = new System.Drawing.Size(573, 312);
           this.Controls.Add(this.helpOptimize);
           this.Controls.Add(this.label4);
           this.Controls.Add(this.helpAudit);
           this.Controls.Add(this.label_ProductsPageText);
           this.Controls.Add(this.label3);
           this.Controls.Add(this.pictureBox1);
           this.Controls.Add(this.panel1);
           this.Controls.Add(this.button_No);
           this.Controls.Add(this.button_Yes);
           this.Controls.Add(this.label2);
           this.Controls.Add(this.label1);
           this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
           this.Name = "Form_Welcome";
           this.ShowIcon = false;
           this.ShowInTaskbar = false;
           this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
           this.Text = "Welcome to SQL Compliance Manager";
           ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Tagline)).EndInit();
           this.panel1.ResumeLayout(false);
           this.panel1.PerformLayout();
           ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
           this.ResumeLayout(false);
           this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button_Yes;
        private System.Windows.Forms.Button button_No;
        private System.Windows.Forms.PictureBox pictureBox_Tagline;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label_ProductsPageText;
       private System.Windows.Forms.LinkLabel helpAudit;
       private System.Windows.Forms.Label label4;
       private System.Windows.Forms.LinkLabel helpOptimize;
    }
}
