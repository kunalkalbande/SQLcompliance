namespace Installer_form_application
{
    partial class InstallReady
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InstallReady));
            this.buttonBack = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonNext = new System.Windows.Forms.Button();
            this.labelHeading = new System.Windows.Forms.Label();
            this.labelDesc = new System.Windows.Forms.Label();
            this.labeSPSAccount = new System.Windows.Forms.Label();
            this.labelIDSAccount = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonBack
            // 
            this.buttonBack.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.buttonBack.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonBack.Location = new System.Drawing.Point(333, 408);
            this.buttonBack.Name = "buttonBack";
            this.buttonBack.Size = new System.Drawing.Size(72, 22);
            this.buttonBack.TabIndex = 2;
            this.buttonBack.Text = "Back\r\n";
            this.buttonBack.UseVisualStyleBackColor = true;
            this.buttonBack.Click += new System.EventHandler(this.buttonBack_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.buttonCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonCancel.Location = new System.Drawing.Point(500, 408);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(72, 22);
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonNext
            // 
            this.buttonNext.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.buttonNext.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonNext.Location = new System.Drawing.Point(408, 408);
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.Size = new System.Drawing.Size(72, 22);
            this.buttonNext.TabIndex = 1;
            this.buttonNext.Text = "Install";
            this.buttonNext.UseVisualStyleBackColor = true;
            this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
            // 
            // labelHeading
            // 
            this.labelHeading.AutoSize = true;
            this.labelHeading.BackColor = System.Drawing.Color.Transparent;
            this.labelHeading.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.labelHeading.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelHeading.Location = new System.Drawing.Point(187, 27);
            this.labelHeading.Name = "labelHeading";
            this.labelHeading.Size = new System.Drawing.Size(287, 21);
            this.labelHeading.TabIndex = 71;
            this.labelHeading.Text = "Idera SQL Compliance Manager Setup";
            // 
            // labelDesc
            // 
            this.labelDesc.BackColor = System.Drawing.Color.Transparent;
            this.labelDesc.Location = new System.Drawing.Point(188, 67);
            this.labelDesc.Name = "labelDesc";
            this.labelDesc.Size = new System.Drawing.Size(352, 18);
            this.labelDesc.TabIndex = 72;
            this.labelDesc.Text = "Ready to install SQL Compliance Manager and Idera Dashboard.";
            // 
            // labeSPSAccount
            // 
            this.labeSPSAccount.AutoSize = true;
            this.labeSPSAccount.BackColor = System.Drawing.Color.Transparent;
            this.labeSPSAccount.Location = new System.Drawing.Point(188, 159);
            this.labeSPSAccount.Name = "labeSPSAccount";
            this.labeSPSAccount.Size = new System.Drawing.Size(216, 13);
            this.labeSPSAccount.TabIndex = 74;
            this.labeSPSAccount.Text = "SQL Compliance Manager Service Account:";
            // 
            // labelIDSAccount
            // 
            this.labelIDSAccount.AutoSize = true;
            this.labelIDSAccount.BackColor = System.Drawing.Color.Transparent;
            this.labelIDSAccount.Location = new System.Drawing.Point(188, 119);
            this.labelIDSAccount.Name = "labelIDSAccount";
            this.labelIDSAccount.Size = new System.Drawing.Size(171, 13);
            this.labelIDSAccount.TabIndex = 73;
            this.labelIDSAccount.Text = "Idera Dashboard Service Account:";
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(188, 85);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(395, 34);
            this.label1.TabIndex = 75;
            this.label1.Text = "Setup will grant these accounts access to Idera dashboard and SQL Compliance Mana" +
    "ger.";
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(188, 308);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(395, 35);
            this.label2.TabIndex = 76;
            this.label2.Text = "Use this account to log into the Idera Dashboard and grant other users access fro" +
    "m the Idera Dashboard Administration tab.\r\n";
            // 
            // InstallReady
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::Installer_form_application.Properties.Resources.Main_Background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(584, 442);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labeSPSAccount);
            this.Controls.Add(this.labelIDSAccount);
            this.Controls.Add(this.labelDesc);
            this.Controls.Add(this.labelHeading);
            this.Controls.Add(this.buttonBack);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonNext);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InstallReady";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Idera SQL Compliance Manager Setup";
            this.Load += new System.EventHandler(this.InstallReady_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonBack;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonNext;
        private System.Windows.Forms.Label labelHeading;
        private System.Windows.Forms.Label labelDesc;
        private System.Windows.Forms.Label labeSPSAccount;
        private System.Windows.Forms.Label labelIDSAccount;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}