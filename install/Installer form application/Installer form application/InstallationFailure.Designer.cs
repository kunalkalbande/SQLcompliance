namespace Installer_form_application
{
    partial class InstallationFailure
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InstallationFailure));
            this.labelHeadDescription = new System.Windows.Forms.Label();
            this.labelHeading = new System.Windows.Forms.Label();
            this.buttonFinish = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelHeadDescription
            // 
            this.labelHeadDescription.BackColor = System.Drawing.Color.Transparent;
            this.labelHeadDescription.Location = new System.Drawing.Point(201, 70);
            this.labelHeadDescription.Name = "labelHeadDescription";
            this.labelHeadDescription.Size = new System.Drawing.Size(371, 47);
            this.labelHeadDescription.TabIndex = 43;
            // 
            // labelHeading
            // 
            this.labelHeading.BackColor = System.Drawing.Color.Transparent;
            this.labelHeading.Font = new System.Drawing.Font("Segoe UI Semibold", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.labelHeading.Location = new System.Drawing.Point(200, 30);
            this.labelHeading.Name = "labelHeading";
            this.labelHeading.Size = new System.Drawing.Size(327, 23);
            this.labelHeading.TabIndex = 42;
            this.labelHeading.Text = "Idera SQL Compliance Manager Setup";
            // 
            // buttonFinish
            // 
            this.buttonFinish.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonFinish.Location = new System.Drawing.Point(500, 408);
            this.buttonFinish.Name = "buttonFinish";
            this.buttonFinish.Size = new System.Drawing.Size(72, 22);
            this.buttonFinish.TabIndex = 1;
            this.buttonFinish.Text = "Finish";
            this.buttonFinish.UseVisualStyleBackColor = true;
            this.buttonFinish.Click += new System.EventHandler(this.buttonFinish_Click);
            // 
            // InstallationFailure
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::Installer_form_application.Properties.Resources.Main_Background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(584, 442);
            this.Controls.Add(this.labelHeadDescription);
            this.Controls.Add(this.labelHeading);
            this.Controls.Add(this.buttonFinish);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InstallationFailure";
            this.Text = "Idera SQL Compliance Manager Setup";
            this.Load += new System.EventHandler(this.InstallationFailure_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelHeadDescription;
        private System.Windows.Forms.Label labelHeading;
        private System.Windows.Forms.Button buttonFinish;
    }
}