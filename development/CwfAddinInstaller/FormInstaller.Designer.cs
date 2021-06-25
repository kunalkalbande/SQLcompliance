namespace CwfAddinInstaller
{
    partial class FormInstaller
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormInstaller));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnPrevious = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pnlPageHost = new System.Windows.Forms.Panel();
            this.lblPageDescription = new System.Windows.Forms.Label();
            this.lblPageTitle = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(416, 311);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(335, 311);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(75, 23);
            this.btnNext.TabIndex = 5;
            this.btnNext.Text = "Next";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnPrevious
            // 
            this.btnPrevious.Location = new System.Drawing.Point(254, 311);
            this.btnPrevious.Name = "btnPrevious";
            this.btnPrevious.Size = new System.Drawing.Size(75, 23);
            this.btnPrevious.TabIndex = 4;
            this.btnPrevious.Text = "Previous";
            this.btnPrevious.UseVisualStyleBackColor = true;
            this.btnPrevious.Click += new System.EventHandler(this.btnPrevious_Click);
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::CwfAddinInstaller.Properties.Resources.Image;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel1.Controls.Add(this.pnlPageHost);
            this.panel1.Controls.Add(this.lblPageDescription);
            this.panel1.Controls.Add(this.lblPageTitle);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(503, 305);
            this.panel1.TabIndex = 7;
            // 
            // pnlPageHost
            // 
            this.pnlPageHost.BackColor = System.Drawing.Color.Transparent;
            this.pnlPageHost.Location = new System.Drawing.Point(176, 81);
            this.pnlPageHost.Margin = new System.Windows.Forms.Padding(0);
            this.pnlPageHost.Name = "pnlPageHost";
            this.pnlPageHost.Size = new System.Drawing.Size(315, 215);
            this.pnlPageHost.TabIndex = 0;
            // 
            // lblPageDescription
            // 
            this.lblPageDescription.BackColor = System.Drawing.Color.Transparent;
            this.lblPageDescription.Location = new System.Drawing.Point(173, 26);
            this.lblPageDescription.Name = "lblPageDescription";
            this.lblPageDescription.Size = new System.Drawing.Size(318, 55);
            this.lblPageDescription.TabIndex = 2;
            this.lblPageDescription.Text = resources.GetString("lblPageDescription.Text");
            // 
            // lblPageTitle
            // 
            this.lblPageTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblPageTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPageTitle.Location = new System.Drawing.Point(173, 9);
            this.lblPageTitle.Name = "lblPageTitle";
            this.lblPageTitle.Size = new System.Drawing.Size(318, 48);
            this.lblPageTitle.TabIndex = 1;
            this.lblPageTitle.Text = "Title Goes Here";
            // 
            // FormInstaller
            // 
            this.AcceptButton = this.btnNext;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(503, 346);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.btnPrevious);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FormInstaller";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "IDERA SQL Compliance Manager and Dashboard Setup";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormInstaller_FormClosing);
            this.Load += new System.EventHandler(this.FormInstaller_Load);
            this.Shown += new System.EventHandler(this.FormInstaller_Shown);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnPrevious;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel pnlPageHost;
        public System.Windows.Forms.Label lblPageTitle;
        public System.Windows.Forms.Label lblPageDescription;
    }
}

