using System;
namespace SQLCM_Installer.Custom_Prompts
{
    partial class FormDashboardSQLServerCred
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDashboardSQLServerCred));
            this.panelHeader = new System.Windows.Forms.Panel();
            this.pictureCloseIcon = new System.Windows.Forms.PictureBox();
            this.panelSubHeader = new System.Windows.Forms.Panel();
            this.panelFooter = new System.Windows.Forms.Panel();
            this.pictureDashboardCredError = new System.Windows.Forms.PictureBox();
            this.labelError = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.labelHeader = new SQLCM_Installer.Custom_Controls.IderaTitleLabel();
            this.labelSubHeader = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.saveButton = new SQLCM_Installer.Custom_Controls.IderaButton();
            this.cancelButton = new SQLCM_Installer.Custom_Controls.IderaButton();
            this.textPassword = new SQLCM_Installer.Custom_Controls.IderaTextBox();
            this.textUserName = new SQLCM_Installer.Custom_Controls.IderaTextBox();
            this.labelPassword = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.labelUsername = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.panelHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureCloseIcon)).BeginInit();
            this.panelSubHeader.SuspendLayout();
            this.panelFooter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureDashboardCredError)).BeginInit();
            this.SuspendLayout();
            // 
            // panelHeader
            // 
            this.panelHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(183)))), ((int)(((byte)(175)))), ((int)(((byte)(167)))));
            this.panelHeader.Controls.Add(this.pictureCloseIcon);
            this.panelHeader.Controls.Add(this.labelHeader);
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(500, 54);
            this.panelHeader.TabIndex = 7;
            this.panelHeader.MouseDown += panelHeader_MouseDown;
            // 
            // pictureCloseIcon
            // 
            this.pictureCloseIcon.Image = global::SQLCM_Installer.Properties.Resources.close_icon;
            this.pictureCloseIcon.Location = new System.Drawing.Point(469, 14);
            this.pictureCloseIcon.Name = "pictureCloseIcon";
            this.pictureCloseIcon.Size = new System.Drawing.Size(17, 16);
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
            this.panelSubHeader.TabIndex = 8;
            // 
            // panelFooter
            // 
            this.panelFooter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.panelFooter.Controls.Add(this.saveButton);
            this.panelFooter.Controls.Add(this.cancelButton);
            this.panelFooter.Location = new System.Drawing.Point(0, 228);
            this.panelFooter.Name = "panelFooter";
            this.panelFooter.Size = new System.Drawing.Size(500, 60);
            this.panelFooter.TabIndex = 9;
            // 
            // pictureDashboardCredError
            // 
            this.pictureDashboardCredError.Image = global::SQLCM_Installer.Properties.Resources.criticalicon;
            this.pictureDashboardCredError.Location = new System.Drawing.Point(112, 205);
            this.pictureDashboardCredError.Name = "pictureDashboardCredError";
            this.pictureDashboardCredError.Size = new System.Drawing.Size(16, 16);
            this.pictureDashboardCredError.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureDashboardCredError.TabIndex = 14;
            this.pictureDashboardCredError.TabStop = false;
            this.pictureDashboardCredError.Visible = false;
            // 
            // labelError
            // 
            this.labelError.AutoSize = true;
            this.labelError.BackColor = System.Drawing.Color.Transparent;
            this.labelError.Disabled = false;
            this.labelError.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelError.Location = new System.Drawing.Point(135, 203);
            this.labelError.Name = "labelError";
            this.labelError.Size = new System.Drawing.Size(0, 13);
            this.labelError.TabIndex = 15;
            this.labelError.Visible = false;
            // 
            // labelHeader
            // 
            this.labelHeader.AutoSize = true;
            this.labelHeader.BackColor = System.Drawing.Color.Transparent;
            this.labelHeader.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelHeader.ForeColor = System.Drawing.Color.White;
            this.labelHeader.Location = new System.Drawing.Point(20, 17);
            this.labelHeader.Name = "labelHeader";
            this.labelHeader.Size = new System.Drawing.Size(238, 18);
            this.labelHeader.TabIndex = 0;
            this.labelHeader.Text = "Microsoft SQL Server Authentication ";
            this.labelHeader.MouseDown += panelHeader_MouseDown;
            // 
            // labelSubHeader
            // 
            this.labelSubHeader.AutoSize = true;
            this.labelSubHeader.BackColor = System.Drawing.Color.Transparent;
            this.labelSubHeader.Disabled = false;
            this.labelSubHeader.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSubHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelSubHeader.Location = new System.Drawing.Point(20, 20);
            this.labelSubHeader.Name = "labelSubHeader";
            this.labelSubHeader.Size = new System.Drawing.Size(482, 18);
            this.labelSubHeader.TabIndex = 0;
            this.labelSubHeader.Text = "This login is for " + Constants.ProductMap[Products.Compliance] + " and " + Constants.ProductMap[Products.Dashboard] + " repositories.";
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
            // textPassword
            // 
            this.textPassword.Font = new System.Drawing.Font("Source Sans Pro", 10.5F);
            this.textPassword.Location = new System.Drawing.Point(102, 173);
            this.textPassword.Name = "textPassword";
            this.textPassword.Size = new System.Drawing.Size(388, 25);
            this.textPassword.TabIndex = 13;
            this.textPassword.UseSystemPasswordChar = true;
            // 
            // textUserName
            // 
            this.textUserName.Font = new System.Drawing.Font("Source Sans Pro", 10.5F);
            this.textUserName.Location = new System.Drawing.Point(102, 132);
            this.textUserName.Name = "textUserName";
            this.textUserName.Size = new System.Drawing.Size(388, 25);
            this.textUserName.TabIndex = 12;
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
            this.labelPassword.TabIndex = 11;
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
            this.labelUsername.Size = new System.Drawing.Size(74, 18);
            this.labelUsername.TabIndex = 10;
            this.labelUsername.Text = "Username:";
            // 
            // FormDashboardSQLServerCred
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(500, 288);
            this.Controls.Add(this.labelError);
            this.Controls.Add(this.pictureDashboardCredError);
            this.Controls.Add(this.panelHeader);
            this.Controls.Add(this.panelSubHeader);
            this.Controls.Add(this.panelFooter);
            this.Controls.Add(this.textPassword);
            this.Controls.Add(this.textUserName);
            this.Controls.Add(this.labelPassword);
            this.Controls.Add(this.labelUsername);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormDashboardSQLServerCred";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FormDashboardSQLServerCred";
            this.Activated += new System.EventHandler(this.FormDashboardSQLServerCred_Activated);
            this.Deactivate += new System.EventHandler(this.FormDashboardSQLServerCred_Deactivated);
            this.Load += new System.EventHandler(this.FormDashboardSQLServerCred_Load);
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureCloseIcon)).EndInit();
            this.panelSubHeader.ResumeLayout(false);
            this.panelSubHeader.PerformLayout();
            this.panelFooter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureDashboardCredError)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.PictureBox pictureCloseIcon;
        private Custom_Controls.IderaTitleLabel labelHeader;
        private System.Windows.Forms.Panel panelSubHeader;
        private Custom_Controls.IderaLabel labelSubHeader;
        private System.Windows.Forms.Panel panelFooter;
        private Custom_Controls.IderaButton saveButton;
        private Custom_Controls.IderaButton cancelButton;
        private Custom_Controls.IderaTextBox textPassword;
        private Custom_Controls.IderaTextBox textUserName;
        private Custom_Controls.IderaLabel labelPassword;
        private Custom_Controls.IderaLabel labelUsername;
        private System.Windows.Forms.PictureBox pictureDashboardCredError;
        private Custom_Controls.IderaLabel labelError;


    }
}