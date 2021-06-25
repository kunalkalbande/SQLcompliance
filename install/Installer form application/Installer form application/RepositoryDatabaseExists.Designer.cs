namespace Installer_form_application
{
    partial class RepositoryDatabaseExists
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RepositoryDatabaseExists));
            this.labelInstuction = new System.Windows.Forms.Label();
            this.labelHeadDesc = new System.Windows.Forms.Label();
            this.buttonNext = new System.Windows.Forms.Button();
            this.buttonBack = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.labelHeading = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelInstuction
            // 
            this.labelInstuction.BackColor = System.Drawing.Color.Transparent;
            this.labelInstuction.Location = new System.Drawing.Point(197, 103);
            this.labelInstuction.Name = "labelInstuction";
            this.labelInstuction.Size = new System.Drawing.Size(375, 154);
            this.labelInstuction.TabIndex = 34;
            this.labelInstuction.Text = "The repository you have mentioned already exists and it will be updated to latest" +
    " version.";
            // 
            // labelHeadDesc
            // 
            this.labelHeadDesc.AutoSize = true;
            this.labelHeadDesc.BackColor = System.Drawing.Color.Transparent;
            this.labelHeadDesc.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelHeadDesc.Location = new System.Drawing.Point(197, 67);
            this.labelHeadDesc.Name = "labelHeadDesc";
            this.labelHeadDesc.Size = new System.Drawing.Size(162, 13);
            this.labelHeadDesc.TabIndex = 33;
            this.labelHeadDesc.Text = "Repository Database Exists";
            // 
            // buttonNext
            // 
            this.buttonNext.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonNext.Location = new System.Drawing.Point(408, 408);
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.Size = new System.Drawing.Size(72, 22);
            this.buttonNext.TabIndex = 30;
            this.buttonNext.Text = "Next";
            this.buttonNext.UseVisualStyleBackColor = true;
            this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
            // 
            // buttonBack
            // 
            this.buttonBack.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonBack.Location = new System.Drawing.Point(333, 408);
            this.buttonBack.Name = "buttonBack";
            this.buttonBack.Size = new System.Drawing.Size(72, 22);
            this.buttonBack.TabIndex = 31;
            this.buttonBack.Text = "Back";
            this.buttonBack.UseVisualStyleBackColor = true;
            this.buttonBack.Click += new System.EventHandler(this.buttonBack_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonCancel.Location = new System.Drawing.Point(500, 408);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(72, 22);
            this.buttonCancel.TabIndex = 32;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // labelHeading
            // 
            this.labelHeading.AutoSize = true;
            this.labelHeading.BackColor = System.Drawing.Color.Transparent;
            this.labelHeading.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.labelHeading.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelHeading.Location = new System.Drawing.Point(196, 27);
            this.labelHeading.Name = "labelHeading";
            this.labelHeading.Size = new System.Drawing.Size(287, 21);
            this.labelHeading.TabIndex = 35;
            this.labelHeading.Text = "Idera SQL Compliance Manager Setup";
            // 
            // RepositoryDatabaseExists
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::Installer_form_application.Properties.Resources.Main_Background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(584, 442);
            this.Controls.Add(this.labelHeading);
            this.Controls.Add(this.labelInstuction);
            this.Controls.Add(this.labelHeadDesc);
            this.Controls.Add(this.buttonNext);
            this.Controls.Add(this.buttonBack);
            this.Controls.Add(this.buttonCancel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "RepositoryDatabaseExists";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Idera SQL Compliance Manager Setup";
            this.Load += new System.EventHandler(this.RepositoryDatabaseExists_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelInstuction;
        private System.Windows.Forms.Label labelHeadDesc;
        private System.Windows.Forms.Button buttonNext;
        private System.Windows.Forms.Button buttonBack;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label labelHeading;
    }
}