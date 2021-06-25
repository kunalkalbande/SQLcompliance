namespace Installer_form_application
{
    partial class CwfAddinInstall
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CwfAddinInstall));
            this.cwfAddinProgress = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cwfAddinProgress
            // 
            this.cwfAddinProgress.AutoSize = true;
            this.cwfAddinProgress.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cwfAddinProgress.Location = new System.Drawing.Point(54, 61);
            this.cwfAddinProgress.Name = "cwfAddinProgress";
            this.cwfAddinProgress.Size = new System.Drawing.Size(0, 13);
            this.cwfAddinProgress.TabIndex = 0;
            // 
            // CwfAddinInstall
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(387, 142);
            this.Controls.Add(this.cwfAddinProgress);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CwfAddinInstall";
            this.Text = "CwfAddinInstall";
            this.Load += new System.EventHandler(this.CwfAddinInstall_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label cwfAddinProgress;
    }
}