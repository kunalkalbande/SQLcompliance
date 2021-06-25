namespace Idera.SQLcompliance.Application.GUI.Forms
{
    partial class Linq_ErrorBox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Linq_ErrorBox));
            this.errorLbl = new System.Windows.Forms.Label();
            this.okBtn = new System.Windows.Forms.Button();
            this.linkLbl = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // errorLbl
            // 
            this.errorLbl.Location = new System.Drawing.Point(13, 13);
            this.errorLbl.Name = "errorLbl";
            this.errorLbl.Size = new System.Drawing.Size(384, 59);
            this.errorLbl.TabIndex = 0;
            this.errorLbl.Text = resources.GetString("errorLbl.Text");
            // 
            // okBtn
            // 
            this.okBtn.AutoSize = true;
            this.okBtn.Location = new System.Drawing.Point(322, 103);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(75, 23);
            this.okBtn.TabIndex = 1;
            this.okBtn.Text = "&OK";
            this.okBtn.UseVisualStyleBackColor = true;
            this.okBtn.Click += new System.EventHandler(this.okBtn_Click);
            // 
            // linkLbl
            // 
            this.linkLbl.AutoSize = true;
            this.linkLbl.Location = new System.Drawing.Point(16, 69);
            this.linkLbl.Name = "linkLbl";
            this.linkLbl.Size = new System.Drawing.Size(151, 13);
            this.linkLbl.TabIndex = 2;
            this.linkLbl.TabStop = true;
            this.linkLbl.Text = "http://wiki.idera.com/x/NoBfB";
            this.linkLbl.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLbl_LinkClicked);
            // 
            // Linq_ErrorBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(409, 138);
            this.Controls.Add(this.linkLbl);
            this.Controls.Add(this.okBtn);
            this.Controls.Add(this.errorLbl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Linq_ErrorBox";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Error";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label errorLbl;
        private System.Windows.Forms.Button okBtn;
        private System.Windows.Forms.LinkLabel linkLbl;
    }
}