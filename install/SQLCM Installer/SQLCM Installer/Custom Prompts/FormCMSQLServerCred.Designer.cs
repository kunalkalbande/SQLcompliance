using System;
namespace SQLCM_Installer.Custom_Prompts
{
    partial class FormCMSQLServerCred
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCMSQLServerCred));
            this.panelHeader = new System.Windows.Forms.Panel();
            this.pictureCloseIcon = new System.Windows.Forms.PictureBox();
            this.panelSubHeader = new System.Windows.Forms.Panel();
            this.panelFooter = new System.Windows.Forms.Panel();
            this.pictureCMCredError = new System.Windows.Forms.PictureBox();
            this.labelError = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.textPassword = new SQLCM_Installer.Custom_Controls.IderaTextBox();
            this.textUserName = new SQLCM_Installer.Custom_Controls.IderaTextBox();
            this.labelPassword = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.labelUsername = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.saveButton = new SQLCM_Installer.Custom_Controls.IderaButton();
            this.cancelButton = new SQLCM_Installer.Custom_Controls.IderaButton();
            this.labelSubHeader = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.labelHeader = new SQLCM_Installer.Custom_Controls.IderaTitleLabel();
            this.panelHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureCloseIcon)).BeginInit();
            this.panelSubHeader.SuspendLayout();
            this.panelFooter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureCMCredError)).BeginInit();
            this.SuspendLayout();
            // 
            // panelHeader
            // 
            this.panelHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(183)))), ((int)(((byte)(175)))), ((int)(((byte)(167)))));
            this.panelHeader.Controls.Add(this.labelHeader);
            this.panelHeader.Controls.Add(this.pictureCloseIcon);
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(500, 55);
            this.panelHeader.TabIndex = 0;
            this.panelHeader.MouseDown += panelHeader_MouseDown;
            // 
            // pictureCloseIcon
            // 
            this.pictureCloseIcon.Image = global::SQLCM_Installer.Properties.Resources.close_icon;
            this.pictureCloseIcon.Location = new System.Drawing.Point(465, 20);
            this.pictureCloseIcon.Name = "pictureCloseIcon";
            this.pictureCloseIcon.Size = new System.Drawing.Size(17, 25);
            this.pictureCloseIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureCloseIcon.TabIndex = 1;
            this.pictureCloseIcon.TabStop = false;
            this.pictureCloseIcon.Click += new System.EventHandler(this.pictureCloseIcon_Click);
            this.pictureCloseIcon.MouseEnter += new EventHandler(this.pictureCloseIcon_MouseEnter);
            this.pictureCloseIcon.MouseLeave += new EventHandler(this.pictureCloseIcon_MouseLeave);
            // 
            // panelSubHeader
            // 
            this.panelSubHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.panelSubHeader.Controls.Add(this.labelSubHeader);
            this.panelSubHeader.Location = new System.Drawing.Point(0, 54);
            this.panelSubHeader.Name = "panelSubHeader";
            this.panelSubHeader.Size = new System.Drawing.Size(500, 58);
            this.panelSubHeader.TabIndex = 1;
            // 
            // panelFooter
            // 
            this.panelFooter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.panelFooter.Controls.Add(this.saveButton);
            this.panelFooter.Controls.Add(this.cancelButton);
            this.panelFooter.Location = new System.Drawing.Point(0, 228);
            this.panelFooter.Name = "panelFooter";
            this.panelFooter.Size = new System.Drawing.Size(500, 60);
            this.panelFooter.TabIndex = 2;
            // 
            // pictureCMCredError
            // 
            this.pictureCMCredError.Image = global::SQLCM_Installer.Properties.Resources.criticalicon;
            this.pictureCMCredError.Location = new System.Drawing.Point(111, 205);
            this.pictureCMCredError.Name = "pictureCMCredError";
            this.pictureCMCredError.Size = new System.Drawing.Size(16, 16);
            this.pictureCMCredError.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureCMCredError.TabIndex = 7;
            this.pictureCMCredError.TabStop = false;
            this.pictureCMCredError.Visible = false;
            // 
            // labelError
            // 
            this.labelError.AutoSize = true;
            this.labelError.BackColor = System.Drawing.Color.Transparent;
            this.labelError.Disabled = false;
            this.labelError.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.labelError.Location = new System.Drawing.Point(133, 204);
            this.labelError.Name = "labelError";
            this.labelError.Size = new System.Drawing.Size(0, 13);
            this.labelError.TabIndex = 8;
            this.labelError.Visible = false;
            // 
            // textPassword
            // 
            this.textPassword.Font = new System.Drawing.Font("Source Sans Pro", 10.5F);
            this.textPassword.Location = new System.Drawing.Point(100, 173);
            this.textPassword.Name = "textPassword";
            this.textPassword.Size = new System.Drawing.Size(380, 25);
            this.textPassword.TabIndex = 6;
            this.textPassword.UseSystemPasswordChar = true;
            // 
            // textUserName
            // 
            this.textUserName.Font = new System.Drawing.Font("Source Sans Pro", 10.5F);
            this.textUserName.Location = new System.Drawing.Point(100, 132);
            this.textUserName.Name = "textUserName";
            this.textUserName.Size = new System.Drawing.Size(380, 25);
            this.textUserName.TabIndex = 5;
            // 
            // labelPassword
            // 
            this.labelPassword.AutoSize = true;
            this.labelPassword.BackColor = System.Drawing.Color.Transparent;
            this.labelPassword.Disabled = false;
            this.labelPassword.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelPassword.Location = new System.Drawing.Point(20, 176);
            this.labelPassword.Name = "labelPassword";
            this.labelPassword.Size = new System.Drawing.Size(72, 18);
            this.labelPassword.TabIndex = 4;
            this.labelPassword.Text = "Password:";
            // 
            // labelUsername
            // 
            this.labelUsername.AutoSize = true;
            this.labelUsername.BackColor = System.Drawing.Color.Transparent;
            this.labelUsername.Disabled = false;
            this.labelUsername.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelUsername.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelUsername.Location = new System.Drawing.Point(20, 135);
            this.labelUsername.Name = "labelUsername";
            this.labelUsername.Size = new System.Drawing.Size(72, 18);
            this.labelUsername.TabIndex = 3;
            this.labelUsername.Text = "Username:";
            // 
            // saveButton
            // 
            this.saveButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(186)))), ((int)(((byte)(163)))));
            this.saveButton.BorderColor = System.Drawing.Color.Transparent;
            this.saveButton.BorderWidth = 2;
            this.saveButton.ButtonText = "";
            this.saveButton.Disabled = false;
            this.saveButton.EndColor = System.Drawing.Color.Yellow;
            this.saveButton.FlatAppearance.BorderSize = 0;
            this.saveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveButton.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.saveButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.saveButton.GradientAngle = 90;
            this.saveButton.Location = new System.Drawing.Point(408, 16);
            this.saveButton.Name = "saveButton";
            this.saveButton.ShowButtontext = true;
            this.saveButton.Size = new System.Drawing.Size(66, 28);
            this.saveButton.StartColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(186)))), ((int)(((byte)(163)))));
            this.saveButton.TabIndex = 1;
            this.saveButton.Text = "Save";
            this.saveButton.TextLocation_X = 100;
            this.saveButton.TextLocation_Y = 25;
            this.saveButton.UseVisualStyleBackColor = false;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(186)))), ((int)(((byte)(163)))));
            this.cancelButton.BorderColor = System.Drawing.Color.Transparent;
            this.cancelButton.BorderWidth = 2;
            this.cancelButton.ButtonText = "";
            this.cancelButton.Disabled = false;
            this.cancelButton.EndColor = System.Drawing.Color.Yellow;
            this.cancelButton.FlatAppearance.BorderSize = 0;
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cancelButton.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancelButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.cancelButton.GradientAngle = 90;
            this.cancelButton.Location = new System.Drawing.Point(332, 16);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.ShowButtontext = true;
            this.cancelButton.Size = new System.Drawing.Size(66, 28);
            this.cancelButton.StartColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(186)))), ((int)(((byte)(163)))));
            this.cancelButton.TabIndex = 0;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.TextLocation_X = 100;
            this.cancelButton.TextLocation_Y = 25;
            this.cancelButton.UseVisualStyleBackColor = false;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // labelSubHeader
            // 
            this.labelSubHeader.AutoSize = true;
            this.labelSubHeader.BackColor = System.Drawing.Color.Transparent;
            this.labelSubHeader.Disabled = false;
            this.labelSubHeader.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSubHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelSubHeader.Location = new System.Drawing.Point(15, 20);
            this.labelSubHeader.Name = "labelSubHeader";
            this.labelSubHeader.Size = new System.Drawing.Size(482, 18);
            this.labelSubHeader.TabIndex = 0;
            this.labelSubHeader.Text = "This login is for " + Constants.ProductMap[Products.Compliance] + " and " + Constants.ProductMap[Products.Dashboard] + " repositories.";
            // 
            // labelHeader
            // 
            this.labelHeader.AutoSize = true;
            this.labelHeader.ForeColor = System.Drawing.Color.White;
            this.labelHeader.Location = new System.Drawing.Point(14, 14);
            this.labelHeader.Name = "labelHeader";
            this.labelHeader.Size = new System.Drawing.Size(182, 25);
            this.labelHeader.TabIndex = 2;
            this.labelHeader.Text = "Microsoft SQL Server Authentication ";
            this.labelHeader.MouseDown += panelHeader_MouseDown;
            // 
            // FormCMSQLServerCred
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(500, 288);
            this.Controls.Add(this.labelError);
            this.Controls.Add(this.pictureCMCredError);
            this.Controls.Add(this.textPassword);
            this.Controls.Add(this.textUserName);
            this.Controls.Add(this.labelPassword);
            this.Controls.Add(this.labelUsername);
            this.Controls.Add(this.panelFooter);
            this.Controls.Add(this.panelSubHeader);
            this.Controls.Add(this.panelHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormCMSQLServerCred";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FormCMSQLServerCred";
            this.Activated += new System.EventHandler(this.FormCMSQLServerCred_Activated);
            this.Deactivate += new System.EventHandler(this.FormCMSQLServerCred_Deactivated);
            this.Load += new System.EventHandler(this.FormCMSQLServerCred_Load);
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureCloseIcon)).EndInit();
            this.panelSubHeader.ResumeLayout(false);
            this.panelSubHeader.PerformLayout();
            this.panelFooter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureCMCredError)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Panel panelSubHeader;
        private System.Windows.Forms.Panel panelFooter;
        private System.Windows.Forms.PictureBox pictureCloseIcon;
        private Custom_Controls.IderaLabel labelSubHeader;
        private Custom_Controls.IderaLabel labelUsername;
        private Custom_Controls.IderaLabel labelPassword;
        private Custom_Controls.IderaTextBox textUserName;
        private Custom_Controls.IderaTextBox textPassword;
        private Custom_Controls.IderaButton cancelButton;
        private Custom_Controls.IderaButton saveButton;
        private Custom_Controls.IderaTitleLabel labelHeader;
        private System.Windows.Forms.PictureBox pictureCMCredError;
        private Custom_Controls.IderaLabel labelError;
    }
}