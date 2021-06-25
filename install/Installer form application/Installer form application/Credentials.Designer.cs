namespace Installer_form_application
{
    partial class Credentials
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Credentials));
            this.labelSQLBIUserName = new System.Windows.Forms.Label();
            this.labelSQLBIPassword = new System.Windows.Forms.Label();
            this.textBoxIDUserName = new System.Windows.Forms.TextBox();
            this.textBoxIDPassword = new System.Windows.Forms.TextBox();
            this.labelHeading = new System.Windows.Forms.Label();
            this.labelDesc = new System.Windows.Forms.Label();
            this.radioButtonLocal = new System.Windows.Forms.RadioButton();
            this.radioButtonRemote = new System.Windows.Forms.RadioButton();
            this.labelNote = new System.Windows.Forms.Label();
            this.labelUrlDesc = new System.Windows.Forms.Label();
            this.labelUrl = new System.Windows.Forms.Label();
            this.labelCredsDesc = new System.Windows.Forms.Label();
            this.textBoxDashboardUrl = new System.Windows.Forms.TextBox();
            this.buttonBack = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonNext = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelSQLBIUserName
            // 
            this.labelSQLBIUserName.AutoSize = true;
            this.labelSQLBIUserName.BackColor = System.Drawing.Color.Transparent;
            this.labelSQLBIUserName.Location = new System.Drawing.Point(206, 315);
            this.labelSQLBIUserName.Name = "labelSQLBIUserName";
            this.labelSQLBIUserName.Size = new System.Drawing.Size(107, 13);
            this.labelSQLBIUserName.TabIndex = 1;
            this.labelSQLBIUserName.Text = "Domain \\ UserName:";
            // 
            // labelSQLBIPassword
            // 
            this.labelSQLBIPassword.AutoSize = true;
            this.labelSQLBIPassword.BackColor = System.Drawing.Color.Transparent;
            this.labelSQLBIPassword.Location = new System.Drawing.Point(257, 341);
            this.labelSQLBIPassword.Name = "labelSQLBIPassword";
            this.labelSQLBIPassword.Size = new System.Drawing.Size(56, 13);
            this.labelSQLBIPassword.TabIndex = 2;
            this.labelSQLBIPassword.Text = "Password:";
            // 
            // textBoxIDUserName
            // 
            this.textBoxIDUserName.Enabled = false;
            this.textBoxIDUserName.Location = new System.Drawing.Point(319, 312);
            this.textBoxIDUserName.Name = "textBoxIDUserName";
            this.textBoxIDUserName.Size = new System.Drawing.Size(243, 20);
            this.textBoxIDUserName.TabIndex = 4;
            // 
            // textBoxIDPassword
            // 
            this.textBoxIDPassword.Enabled = false;
            this.textBoxIDPassword.Location = new System.Drawing.Point(319, 338);
            this.textBoxIDPassword.Name = "textBoxIDPassword";
            this.textBoxIDPassword.PasswordChar = '*';
            this.textBoxIDPassword.Size = new System.Drawing.Size(243, 20);
            this.textBoxIDPassword.TabIndex = 5;
            // 
            // labelHeading
            // 
            this.labelHeading.AutoSize = true;
            this.labelHeading.BackColor = System.Drawing.Color.Transparent;
            this.labelHeading.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.labelHeading.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelHeading.Location = new System.Drawing.Point(191, 27);
            this.labelHeading.Name = "labelHeading";
            this.labelHeading.Size = new System.Drawing.Size(287, 21);
            this.labelHeading.TabIndex = 30;
            this.labelHeading.Text = "Idera SQL Compliance Manager Setup";
            this.labelHeading.Click += new System.EventHandler(this.labelHeading_Click);
            // 
            // labelDesc
            // 
            this.labelDesc.BackColor = System.Drawing.Color.Transparent;
            this.labelDesc.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.labelDesc.Location = new System.Drawing.Point(193, 64);
            this.labelDesc.Name = "labelDesc";
            this.labelDesc.Size = new System.Drawing.Size(379, 41);
            this.labelDesc.TabIndex = 31;
            this.labelDesc.Text = "To get started, we will need to install Idera Dashboard. Please tell us the locat" +
    "ion where you like to install or upgrade the Idera Dashboard.";
            // 
            // radioButtonLocal
            // 
            this.radioButtonLocal.AutoSize = true;
            this.radioButtonLocal.BackColor = System.Drawing.Color.Transparent;
            this.radioButtonLocal.Checked = true;
            this.radioButtonLocal.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.radioButtonLocal.Location = new System.Drawing.Point(195, 108);
            this.radioButtonLocal.Name = "radioButtonLocal";
            this.radioButtonLocal.Size = new System.Drawing.Size(220, 17);
            this.radioButtonLocal.TabIndex = 1;
            this.radioButtonLocal.TabStop = true;
            this.radioButtonLocal.Text = "Install or upgrade Idera Dashboard locally";
            this.radioButtonLocal.UseVisualStyleBackColor = false;
            this.radioButtonLocal.CheckedChanged += new System.EventHandler(this.radioButtonLocal_CheckedChanged);
            // 
            // radioButtonRemote
            // 
            this.radioButtonRemote.AutoSize = true;
            this.radioButtonRemote.BackColor = System.Drawing.Color.Transparent;
            this.radioButtonRemote.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.radioButtonRemote.Location = new System.Drawing.Point(195, 126);
            this.radioButtonRemote.Name = "radioButtonRemote";
            this.radioButtonRemote.Size = new System.Drawing.Size(256, 17);
            this.radioButtonRemote.TabIndex = 2;
            this.radioButtonRemote.Text = "Register with an exisitng remote Idera Dashboard";
            this.radioButtonRemote.UseVisualStyleBackColor = false;
            this.radioButtonRemote.CheckedChanged += new System.EventHandler(this.radioButtonRemote_CheckedChanged);
            // 
            // labelNote
            // 
            this.labelNote.BackColor = System.Drawing.Color.Transparent;
            this.labelNote.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.labelNote.Location = new System.Drawing.Point(193, 155);
            this.labelNote.Name = "labelNote";
            this.labelNote.Size = new System.Drawing.Size(379, 58);
            this.labelNote.TabIndex = 35;
            this.labelNote.Text = resources.GetString("labelNote.Text");
            this.labelNote.Click += new System.EventHandler(this.labelNote_Click);
            // 
            // labelUrlDesc
            // 
            this.labelUrlDesc.BackColor = System.Drawing.Color.Transparent;
            this.labelUrlDesc.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.labelUrlDesc.Location = new System.Drawing.Point(193, 222);
            this.labelUrlDesc.Name = "labelUrlDesc";
            this.labelUrlDesc.Size = new System.Drawing.Size(369, 17);
            this.labelUrlDesc.TabIndex = 36;
            this.labelUrlDesc.Text = "Please provide Idera Dashboard URL and Admin info for existing installation.";
            // 
            // labelUrl
            // 
            this.labelUrl.AutoSize = true;
            this.labelUrl.BackColor = System.Drawing.Color.Transparent;
            this.labelUrl.Location = new System.Drawing.Point(198, 254);
            this.labelUrl.Name = "labelUrl";
            this.labelUrl.Size = new System.Drawing.Size(115, 13);
            this.labelUrl.TabIndex = 37;
            this.labelUrl.Text = "Enter Dashboard URL:";
            // 
            // labelCredsDesc
            // 
            this.labelCredsDesc.AutoSize = true;
            this.labelCredsDesc.BackColor = System.Drawing.Color.Transparent;
            this.labelCredsDesc.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.labelCredsDesc.Location = new System.Drawing.Point(193, 285);
            this.labelCredsDesc.Name = "labelCredsDesc";
            this.labelCredsDesc.Size = new System.Drawing.Size(205, 13);
            this.labelCredsDesc.TabIndex = 38;
            this.labelCredsDesc.Text = "Enter Dashboard Administrator Credentials";
            // 
            // textBoxDashboardUrl
            // 
            this.textBoxDashboardUrl.Enabled = false;
            this.textBoxDashboardUrl.Location = new System.Drawing.Point(319, 251);
            this.textBoxDashboardUrl.Name = "textBoxDashboardUrl";
            this.textBoxDashboardUrl.Size = new System.Drawing.Size(243, 20);
            this.textBoxDashboardUrl.TabIndex = 3;
            // 
            // buttonBack
            // 
            this.buttonBack.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.buttonBack.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonBack.Location = new System.Drawing.Point(330, 408);
            this.buttonBack.Name = "buttonBack";
            this.buttonBack.Size = new System.Drawing.Size(72, 22);
            this.buttonBack.TabIndex = 7;
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
            this.buttonCancel.TabIndex = 8;
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
            this.buttonNext.TabIndex = 6;
            this.buttonNext.Text = "Next";
            this.buttonNext.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.buttonNext.UseVisualStyleBackColor = true;
            this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
            // 
            // Credentials
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.BackgroundImage = global::Installer_form_application.Properties.Resources.Main_Background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(584, 442);
            this.Controls.Add(this.buttonBack);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonNext);
            this.Controls.Add(this.textBoxDashboardUrl);
            this.Controls.Add(this.labelCredsDesc);
            this.Controls.Add(this.labelUrl);
            this.Controls.Add(this.labelUrlDesc);
            this.Controls.Add(this.labelNote);
            this.Controls.Add(this.radioButtonRemote);
            this.Controls.Add(this.radioButtonLocal);
            this.Controls.Add(this.labelDesc);
            this.Controls.Add(this.labelHeading);
            this.Controls.Add(this.labelSQLBIPassword);
            this.Controls.Add(this.textBoxIDPassword);
            this.Controls.Add(this.labelSQLBIUserName);
            this.Controls.Add(this.textBoxIDUserName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Credentials";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Idera SQL Compliance Manager Setup";
            this.Load += new System.EventHandler(this.Credentials_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelSQLBIUserName;
        private System.Windows.Forms.Label labelSQLBIPassword;
        private System.Windows.Forms.TextBox textBoxIDUserName;
        private System.Windows.Forms.TextBox textBoxIDPassword;
        private System.Windows.Forms.Label labelHeading;
        private System.Windows.Forms.Label labelDesc;
        private System.Windows.Forms.RadioButton radioButtonLocal;
        private System.Windows.Forms.RadioButton radioButtonRemote;
        private System.Windows.Forms.Label labelNote;
        private System.Windows.Forms.Label labelUrlDesc;
        private System.Windows.Forms.Label labelUrl;
        private System.Windows.Forms.Label labelCredsDesc;
        private System.Windows.Forms.TextBox textBoxDashboardUrl;
        private System.Windows.Forms.Button buttonBack;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonNext;

    }
}