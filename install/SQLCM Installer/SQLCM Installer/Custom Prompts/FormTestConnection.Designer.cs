using SQLCM_Installer.Custom_Controls;
using System;
namespace SQLCM_Installer.Custom_Prompts
{
    partial class FormTestConnection
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTestConnection));
            this.panelHeader = new System.Windows.Forms.Panel();
            this.pictureCloseIcon = new System.Windows.Forms.PictureBox();
            this.labelHeader = new SQLCM_Installer.Custom_Controls.IderaTitleLabel();
            this.panelFooter = new System.Windows.Forms.Panel();
            this.OkButton = new SQLCM_Installer.Custom_Controls.IderaButton();
            this.pictureCMCredOkorError = new System.Windows.Forms.PictureBox();
            this.pictureDashboardCredOkorError = new System.Windows.Forms.PictureBox();
            this.labelCMCredOkorError = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.labelDashboardCredOkorError = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.panelHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureCloseIcon)).BeginInit();
            this.panelFooter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureCMCredOkorError)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureDashboardCredOkorError)).BeginInit();
            this.SuspendLayout();
            // 
            // panelHeader
            // 
            this.panelHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(183)))), ((int)(((byte)(175)))), ((int)(((byte)(167)))));
            this.panelHeader.Controls.Add(this.pictureCloseIcon);
            this.panelHeader.Controls.Add(this.labelHeader);
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(500, 55);
            this.panelHeader.TabIndex = 15;
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
            // labelHeader
            // 
            this.labelHeader.AutoSize = true;
            this.labelHeader.BackColor = System.Drawing.Color.Transparent;
            this.labelHeader.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelHeader.ForeColor = System.Drawing.Color.White;
            this.labelHeader.Location = new System.Drawing.Point(14, 14);
            this.labelHeader.Name = "labelHeader";
            this.labelHeader.Size = new System.Drawing.Size(113, 25);
            this.labelHeader.TabIndex = 0;
            this.labelHeader.Text = "Test Connections";
            this.labelHeader.MouseDown += panelHeader_MouseDown;
            // 
            // panelFooter
            // 
            this.panelFooter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.panelFooter.Controls.Add(this.OkButton);
            this.panelFooter.Location = new System.Drawing.Point(0, 232);
            this.panelFooter.Name = "panelFooter";
            this.panelFooter.Size = new System.Drawing.Size(500, 60);
            this.panelFooter.TabIndex = 17;
            // 
            // OkButton
            // 
            this.OkButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(186)))), ((int)(((byte)(163)))));
            this.OkButton.BorderColor = System.Drawing.Color.Transparent;
            this.OkButton.BorderWidth = 2;
            this.OkButton.ButtonText = "";
            this.OkButton.Disabled = false;
            this.OkButton.EndColor = System.Drawing.Color.Yellow;
            this.OkButton.FlatAppearance.BorderSize = 0;
            this.OkButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.OkButton.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OkButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.OkButton.GradientAngle = 90;
            this.OkButton.Location = new System.Drawing.Point(408, 16);
            this.OkButton.Name = "OkButton";
            this.OkButton.ShowButtontext = true;
            this.OkButton.Size = new System.Drawing.Size(66, 28);
            this.OkButton.StartColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(186)))), ((int)(((byte)(163)))));
            this.OkButton.TabIndex = 1;
            this.OkButton.Text = "OK";
            this.OkButton.TextLocation_X = 100;
            this.OkButton.TextLocation_Y = 25;
            this.OkButton.UseVisualStyleBackColor = false;
            this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // pictureCMCredOkorError
            // 
            this.pictureCMCredOkorError.Location = new System.Drawing.Point(20, 77);
            this.pictureCMCredOkorError.Name = "pictureCMCredOkorError";
            this.pictureCMCredOkorError.Size = new System.Drawing.Size(16, 16);
            this.pictureCMCredOkorError.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureCMCredOkorError.TabIndex = 18;
            this.pictureCMCredOkorError.TabStop = false;
            // 
            // pictureDashboardCredOkorError
            // 
            this.pictureDashboardCredOkorError.Location = new System.Drawing.Point(20, 122);
            this.pictureDashboardCredOkorError.Name = "pictureDashboardCredOkorError";
            this.pictureDashboardCredOkorError.Size = new System.Drawing.Size(16, 16);
            this.pictureDashboardCredOkorError.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureDashboardCredOkorError.TabIndex = 19;
            this.pictureDashboardCredOkorError.TabStop = false;
            // 
            // labelCMCredOkorError
            // 
            this.labelCMCredOkorError.AutoSize = true;
            this.labelCMCredOkorError.BackColor = System.Drawing.Color.Transparent;
            this.labelCMCredOkorError.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCMCredOkorError.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelCMCredOkorError.Location = new System.Drawing.Point(47, 74);
            this.labelCMCredOkorError.Name = "labelCMCredOkorError";
            this.labelCMCredOkorError.Size = new System.Drawing.Size(80, 18);
            this.labelCMCredOkorError.TabIndex = 20;
            this.labelCMCredOkorError.Text = "ideraLabel1";
            // 
            // labelDashboardCredOkorError
            // 
            this.labelDashboardCredOkorError.AutoSize = true;
            this.labelDashboardCredOkorError.BackColor = System.Drawing.Color.Transparent;
            this.labelDashboardCredOkorError.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDashboardCredOkorError.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelDashboardCredOkorError.Location = new System.Drawing.Point(47, 119);
            this.labelDashboardCredOkorError.Name = "labelDashboardCredOkorError";
            this.labelDashboardCredOkorError.Size = new System.Drawing.Size(80, 18);
            this.labelDashboardCredOkorError.TabIndex = 21;
            this.labelDashboardCredOkorError.Text = "ideraLabel2";
            // 
            // FormTestConnection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(500, 292);
            this.Controls.Add(this.labelDashboardCredOkorError);
            this.Controls.Add(this.labelCMCredOkorError);
            this.Controls.Add(this.pictureDashboardCredOkorError);
            this.Controls.Add(this.pictureCMCredOkorError);
            this.Controls.Add(this.panelHeader);
            this.Controls.Add(this.panelFooter);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormTestConnection";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FormTestConnection";
            this.Activated += new System.EventHandler(this.FormTestConnection_Activated);
            this.Deactivate += new System.EventHandler(this.FormTestConnection_Deactivated);
            this.Load += new System.EventHandler(this.FormTestConnection_Load);
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureCloseIcon)).EndInit();
            this.panelFooter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureCMCredOkorError)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureDashboardCredOkorError)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.PictureBox pictureCloseIcon;
        private Custom_Controls.IderaTitleLabel labelHeader;
        private System.Windows.Forms.Panel panelFooter;
        private Custom_Controls.IderaButton OkButton;
        private System.Windows.Forms.PictureBox pictureCMCredOkorError;
        private System.Windows.Forms.PictureBox pictureDashboardCredOkorError;
        private Custom_Controls.IderaLabel labelCMCredOkorError;
        private Custom_Controls.IderaLabel labelDashboardCredOkorError;
    }
}