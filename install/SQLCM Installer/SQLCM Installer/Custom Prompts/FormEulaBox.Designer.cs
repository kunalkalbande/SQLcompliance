using SQLCM_Installer.Custom_Controls;
using System;
namespace SQLCM_Installer.Custom_Prompts
{
    partial class FormEulaBox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEulaBox));
            this.panelEULAHeader = new System.Windows.Forms.Panel();
            this.panelEULA = new System.Windows.Forms.Panel();
            this.closeButton = new System.Windows.Forms.PictureBox();
            this.labelEULAHeader = new SQLCM_Installer.Custom_Controls.IderaTitleLabel();
            this.textEULA = new System.Windows.Forms.RichTextBox();
            this.panelEULAHeader.SuspendLayout();
            this.panelEULA.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.closeButton)).BeginInit();
            this.SuspendLayout();
            // 
            // panelEULAHeader
            // 
            this.panelEULAHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(183)))), ((int)(((byte)(175)))), ((int)(((byte)(167)))));
            this.panelEULAHeader.Controls.Add(this.closeButton);
            this.panelEULAHeader.Controls.Add(this.labelEULAHeader);
            this.panelEULAHeader.Location = new System.Drawing.Point(0, 0);
            this.panelEULAHeader.Name = "panelEULAHeader";
            this.panelEULAHeader.Size = new System.Drawing.Size(500, 55);
            this.panelEULAHeader.TabIndex = 1;
            // 
            // closeButton
            // 
            this.closeButton.Image = global::SQLCM_Installer.Properties.Resources.close_icon;
            this.closeButton.Location = new System.Drawing.Point(465, 20);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(17, 25);
            this.closeButton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.closeButton.TabIndex = 1;
            this.closeButton.TabStop = false;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            this.closeButton.MouseEnter += new System.EventHandler(this.closeButton_MouseEnter);
            // 
            // labelEULAHeader
            // 
            this.labelEULAHeader.AutoSize = true;
            this.labelEULAHeader.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelEULAHeader.ForeColor = System.Drawing.Color.White;
            this.labelEULAHeader.Location = new System.Drawing.Point(14, 14);
            this.labelEULAHeader.Name = "labelEULAHeader";
            this.labelEULAHeader.Size = new System.Drawing.Size(182, 25);
            this.labelEULAHeader.TabIndex = 0;
            this.labelEULAHeader.Text = "End User License Agreement";
            // 
            this.panelEULA.Controls.Add(this.textEULA);
            this.panelEULA.BackColor = System.Drawing.Color.White;
            this.panelEULA.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelEULA.Location = new System.Drawing.Point(10, 65);
            this.panelEULA.Name = "panelEULA";
            this.panelEULA.Size = new System.Drawing.Size(480, 275);
            this.panelEULA.TabIndex = 3;
            // textEULA
            // 
            this.textEULA.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textEULA.Location = new System.Drawing.Point(10, 10);
            this.textEULA.Name = "textEULA";
            this.textEULA.ReadOnly = true;
            this.textEULA.Size = new System.Drawing.Size(468, 255);
            this.textEULA.TabIndex = 2;
            this.textEULA.BackColor = System.Drawing.Color.White;
            this.textEULA.Text = "";
            this.textEULA.Rtf = Properties.Resources.Idera___Software_License_Agreement;
            // 
            // FormEulaBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 350);
            this.Controls.Add(this.panelEULAHeader);
            this.Controls.Add(this.panelEULA);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormEulaBox";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FormEulaBox";
            this.Load += new System.EventHandler(this.FormEulaBox_Load);
            this.Activated += new EventHandler(this.FormEulaBox_Activated);
            this.Deactivate += new EventHandler(this.FormEulaBox_Deactivated);
            this.panelEULAHeader.ResumeLayout(false);
            this.panelEULAHeader.PerformLayout();
            this.panelEULA.ResumeLayout(false);
            this.panelEULA.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.closeButton)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelEULAHeader;
        private System.Windows.Forms.Panel panelEULA;
        private System.Windows.Forms.PictureBox closeButton;
        private IderaTitleLabel labelEULAHeader;
        private System.Windows.Forms.RichTextBox textEULA;
    }
}