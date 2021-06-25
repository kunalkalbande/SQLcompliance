namespace Installer_form_application
{
    partial class ServiceAccount
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServiceAccount));
            this.buttonBack = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonNext = new System.Windows.Forms.Button();
            this.labelHeading = new System.Windows.Forms.Label();
            this.labelDesc = new System.Windows.Forms.Label();
            this.labelSACredentials = new System.Windows.Forms.Label();
            this.labelSQLBIPassword = new System.Windows.Forms.Label();
            this.textBoxSPSPassword = new System.Windows.Forms.TextBox();
            this.labelSQLBIUserName = new System.Windows.Forms.Label();
            this.textBoxSPSUserName = new System.Windows.Forms.TextBox();
            this.checkBoxSameCreds = new System.Windows.Forms.CheckBox();
            this.labelIDSUsername = new System.Windows.Forms.Label();
            this.textBoxIDSUsername = new System.Windows.Forms.TextBox();
            this.labelIDSPassword = new System.Windows.Forms.Label();
            this.textBoxIDSPassword = new System.Windows.Forms.TextBox();
            this.labelSASPCredentials = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonBack
            // 
            this.buttonBack.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.buttonBack.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonBack.Location = new System.Drawing.Point(333, 408);
            this.buttonBack.Name = "buttonBack";
            this.buttonBack.Size = new System.Drawing.Size(72, 22);
            this.buttonBack.TabIndex = 6;
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
            this.buttonCancel.TabIndex = 7;
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
            this.buttonNext.TabIndex = 5;
            this.buttonNext.Text = "Next";
            this.buttonNext.UseVisualStyleBackColor = true;
            this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
            // 
            // labelHeading
            // 
            this.labelHeading.AutoSize = true;
            this.labelHeading.BackColor = System.Drawing.Color.Transparent;
            this.labelHeading.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.labelHeading.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelHeading.Location = new System.Drawing.Point(191, 28);
            this.labelHeading.Name = "labelHeading";
            this.labelHeading.Size = new System.Drawing.Size(287, 21);
            this.labelHeading.TabIndex = 49;
            this.labelHeading.Text = "Idera SQL Compliance Manager Setup";
            // 
            // labelDesc
            // 
            this.labelDesc.BackColor = System.Drawing.Color.Transparent;
            this.labelDesc.Location = new System.Drawing.Point(192, 60);
            this.labelDesc.Name = "labelDesc";
            this.labelDesc.Size = new System.Drawing.Size(380, 72);
            this.labelDesc.TabIndex = 50;
            this.labelDesc.Text = resources.GetString("labelDesc.Text");
            // 
            // labelSACredentials
            // 
            this.labelSACredentials.AutoSize = true;
            this.labelSACredentials.BackColor = System.Drawing.Color.Transparent;
            this.labelSACredentials.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.labelSACredentials.Location = new System.Drawing.Point(192, 242);
            this.labelSACredentials.Name = "labelSACredentials";
            this.labelSACredentials.Size = new System.Drawing.Size(183, 13);
            this.labelSACredentials.TabIndex = 55;
            this.labelSACredentials.Text = "Service Account for Idera Dashboard\r\n";
            this.labelSACredentials.Visible = false;
            // 
            // labelSQLBIPassword
            // 
            this.labelSQLBIPassword.AutoSize = true;
            this.labelSQLBIPassword.BackColor = System.Drawing.Color.Transparent;
            this.labelSQLBIPassword.Location = new System.Drawing.Point(243, 203);
            this.labelSQLBIPassword.Name = "labelSQLBIPassword";
            this.labelSQLBIPassword.Size = new System.Drawing.Size(56, 13);
            this.labelSQLBIPassword.TabIndex = 52;
            this.labelSQLBIPassword.Text = "Password:";
            // 
            // textBoxSPSPassword
            // 
            this.textBoxSPSPassword.Location = new System.Drawing.Point(305, 200);
            this.textBoxSPSPassword.Name = "textBoxSPSPassword";
            this.textBoxSPSPassword.PasswordChar = '*';
            this.textBoxSPSPassword.Size = new System.Drawing.Size(255, 20);
            this.textBoxSPSPassword.TabIndex = 2;
            // 
            // labelSQLBIUserName
            // 
            this.labelSQLBIUserName.AutoSize = true;
            this.labelSQLBIUserName.BackColor = System.Drawing.Color.Transparent;
            this.labelSQLBIUserName.Location = new System.Drawing.Point(192, 177);
            this.labelSQLBIUserName.Name = "labelSQLBIUserName";
            this.labelSQLBIUserName.Size = new System.Drawing.Size(107, 13);
            this.labelSQLBIUserName.TabIndex = 51;
            this.labelSQLBIUserName.Text = "Domain \\ UserName:";
            // 
            // textBoxSPSUserName
            // 
            this.textBoxSPSUserName.Location = new System.Drawing.Point(305, 174);
            this.textBoxSPSUserName.Name = "textBoxSPSUserName";
            this.textBoxSPSUserName.Size = new System.Drawing.Size(255, 20);
            this.textBoxSPSUserName.TabIndex = 1;
            // 
            // checkBoxSameCreds
            // 
            this.checkBoxSameCreds.AutoSize = true;
            this.checkBoxSameCreds.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxSameCreds.Checked = true;
            this.checkBoxSameCreds.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxSameCreds.Location = new System.Drawing.Point(195, 124);
            this.checkBoxSameCreds.Name = "checkBoxSameCreds";
            this.checkBoxSameCreds.Size = new System.Drawing.Size(378, 17);
            this.checkBoxSameCreds.TabIndex = 8;
            this.checkBoxSameCreds.Text = "Use the same account for SQL Compliance Manager and Idera Dashboard";
            this.checkBoxSameCreds.UseVisualStyleBackColor = false;
            this.checkBoxSameCreds.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // labelIDSUsername
            // 
            this.labelIDSUsername.AutoSize = true;
            this.labelIDSUsername.BackColor = System.Drawing.Color.Transparent;
            this.labelIDSUsername.Location = new System.Drawing.Point(192, 267);
            this.labelIDSUsername.Name = "labelIDSUsername";
            this.labelIDSUsername.Size = new System.Drawing.Size(107, 13);
            this.labelIDSUsername.TabIndex = 58;
            this.labelIDSUsername.Text = "Domain \\ UserName:";
            this.labelIDSUsername.Visible = false;
            // 
            // textBoxIDSUsername
            // 
            this.textBoxIDSUsername.Location = new System.Drawing.Point(305, 264);
            this.textBoxIDSUsername.Name = "textBoxIDSUsername";
            this.textBoxIDSUsername.Size = new System.Drawing.Size(255, 20);
            this.textBoxIDSUsername.TabIndex = 3;
            this.textBoxIDSUsername.Visible = false;
            // 
            // labelIDSPassword
            // 
            this.labelIDSPassword.AutoSize = true;
            this.labelIDSPassword.BackColor = System.Drawing.Color.Transparent;
            this.labelIDSPassword.Location = new System.Drawing.Point(243, 293);
            this.labelIDSPassword.Name = "labelIDSPassword";
            this.labelIDSPassword.Size = new System.Drawing.Size(56, 13);
            this.labelIDSPassword.TabIndex = 60;
            this.labelIDSPassword.Text = "Password:";
            this.labelIDSPassword.Visible = false;
            // 
            // textBoxIDSPassword
            // 
            this.textBoxIDSPassword.Location = new System.Drawing.Point(305, 290);
            this.textBoxIDSPassword.Name = "textBoxIDSPassword";
            this.textBoxIDSPassword.PasswordChar = '*';
            this.textBoxIDSPassword.Size = new System.Drawing.Size(255, 20);
            this.textBoxIDSPassword.TabIndex = 4;
            this.textBoxIDSPassword.Visible = false;
            // 
            // labelSASPCredentials
            // 
            this.labelSASPCredentials.AutoSize = true;
            this.labelSASPCredentials.BackColor = System.Drawing.Color.Transparent;
            this.labelSASPCredentials.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.labelSASPCredentials.Location = new System.Drawing.Point(192, 154);
            this.labelSASPCredentials.Name = "labelSASPCredentials";
            this.labelSASPCredentials.Size = new System.Drawing.Size(228, 13);
            this.labelSASPCredentials.TabIndex = 61;
            this.labelSASPCredentials.Text = "Service Account for SQL Compliance Manager";
            this.labelSASPCredentials.Visible = false;
            // 
            // ServiceAccount
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::Installer_form_application.Properties.Resources.Main_Background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(584, 442);
            this.Controls.Add(this.labelSASPCredentials);
            this.Controls.Add(this.textBoxIDSPassword);
            this.Controls.Add(this.labelIDSPassword);
            this.Controls.Add(this.textBoxIDSUsername);
            this.Controls.Add(this.labelIDSUsername);
            this.Controls.Add(this.checkBoxSameCreds);
            this.Controls.Add(this.labelSACredentials);
            this.Controls.Add(this.labelSQLBIPassword);
            this.Controls.Add(this.textBoxSPSPassword);
            this.Controls.Add(this.labelSQLBIUserName);
            this.Controls.Add(this.textBoxSPSUserName);
            this.Controls.Add(this.labelDesc);
            this.Controls.Add(this.labelHeading);
            this.Controls.Add(this.buttonBack);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonNext);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ServiceAccount";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Idera SQL Compliance Manager Setup";
            this.Load += new System.EventHandler(this.ServiceAccount_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonBack;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonNext;
        private System.Windows.Forms.Label labelHeading;
        private System.Windows.Forms.Label labelDesc;
        private System.Windows.Forms.Label labelSACredentials;
        private System.Windows.Forms.Label labelSQLBIPassword;
        private System.Windows.Forms.TextBox textBoxSPSPassword;
        private System.Windows.Forms.Label labelSQLBIUserName;
        private System.Windows.Forms.TextBox textBoxSPSUserName;
        private System.Windows.Forms.CheckBox checkBoxSameCreds;
        private System.Windows.Forms.Label labelIDSUsername;
        private System.Windows.Forms.TextBox textBoxIDSUsername;
        private System.Windows.Forms.Label labelIDSPassword;
        private System.Windows.Forms.TextBox textBoxIDSPassword;
        private System.Windows.Forms.Label labelSASPCredentials;
    }
}