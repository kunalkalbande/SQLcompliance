namespace Installer_form_application
{
    partial class PortForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PortForm));
            this.textBoxCoreServicesPort = new System.Windows.Forms.TextBox();
            this.labelCoreServicePort = new System.Windows.Forms.Label();
            this.labelWebAppServicePort = new System.Windows.Forms.Label();
            this.textBoxWebAppServicePort = new System.Windows.Forms.TextBox();
            this.labelWebAppMonitorPort = new System.Windows.Forms.Label();
            this.textBoxWebAppMonitorPort = new System.Windows.Forms.TextBox();
            this.textBoxWebAppSSLPort = new System.Windows.Forms.TextBox();
            this.labelWebAppSSLPort = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonBack = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonNext = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxCoreServicesPort
            // 
            this.textBoxCoreServicesPort.Location = new System.Drawing.Point(514, 128);
            this.textBoxCoreServicesPort.Name = "textBoxCoreServicesPort";
            this.textBoxCoreServicesPort.Size = new System.Drawing.Size(49, 20);
            this.textBoxCoreServicesPort.TabIndex = 1;
            this.textBoxCoreServicesPort.Text = "9292";
            this.textBoxCoreServicesPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // labelCoreServicePort
            // 
            this.labelCoreServicePort.AutoSize = true;
            this.labelCoreServicePort.BackColor = System.Drawing.Color.Transparent;
            this.labelCoreServicePort.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.labelCoreServicePort.Location = new System.Drawing.Point(189, 131);
            this.labelCoreServicePort.Name = "labelCoreServicePort";
            this.labelCoreServicePort.Size = new System.Drawing.Size(177, 13);
            this.labelCoreServicePort.TabIndex = 1;
            this.labelCoreServicePort.Text = "Idera Dashboard Core Services Port";
            // 
            // labelWebAppServicePort
            // 
            this.labelWebAppServicePort.AutoSize = true;
            this.labelWebAppServicePort.BackColor = System.Drawing.Color.Transparent;
            this.labelWebAppServicePort.Location = new System.Drawing.Point(189, 172);
            this.labelWebAppServicePort.Name = "labelWebAppServicePort";
            this.labelWebAppServicePort.Size = new System.Drawing.Size(228, 13);
            this.labelWebAppServicePort.TabIndex = 2;
            this.labelWebAppServicePort.Text = "Idera Dashboard Web Application Service Port";
            // 
            // textBoxWebAppServicePort
            // 
            this.textBoxWebAppServicePort.Location = new System.Drawing.Point(514, 169);
            this.textBoxWebAppServicePort.Name = "textBoxWebAppServicePort";
            this.textBoxWebAppServicePort.Size = new System.Drawing.Size(49, 20);
            this.textBoxWebAppServicePort.TabIndex = 2;
            this.textBoxWebAppServicePort.Text = "9290";
            this.textBoxWebAppServicePort.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // labelWebAppMonitorPort
            // 
            this.labelWebAppMonitorPort.AutoSize = true;
            this.labelWebAppMonitorPort.BackColor = System.Drawing.Color.Transparent;
            this.labelWebAppMonitorPort.Location = new System.Drawing.Point(189, 213);
            this.labelWebAppMonitorPort.Name = "labelWebAppMonitorPort";
            this.labelWebAppMonitorPort.Size = new System.Drawing.Size(227, 13);
            this.labelWebAppMonitorPort.TabIndex = 4;
            this.labelWebAppMonitorPort.Text = "Idera Dashboard Web Application Monitor Port";
            // 
            // textBoxWebAppMonitorPort
            // 
            this.textBoxWebAppMonitorPort.Location = new System.Drawing.Point(514, 210);
            this.textBoxWebAppMonitorPort.Name = "textBoxWebAppMonitorPort";
            this.textBoxWebAppMonitorPort.Size = new System.Drawing.Size(49, 20);
            this.textBoxWebAppMonitorPort.TabIndex = 3;
            this.textBoxWebAppMonitorPort.Text = "9094";
            this.textBoxWebAppMonitorPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBoxWebAppSSLPort
            // 
            this.textBoxWebAppSSLPort.Location = new System.Drawing.Point(514, 251);
            this.textBoxWebAppSSLPort.Name = "textBoxWebAppSSLPort";
            this.textBoxWebAppSSLPort.Size = new System.Drawing.Size(49, 20);
            this.textBoxWebAppSSLPort.TabIndex = 4;
            this.textBoxWebAppSSLPort.Text = "9291";
            this.textBoxWebAppSSLPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // labelWebAppSSLPort
            // 
            this.labelWebAppSSLPort.AutoSize = true;
            this.labelWebAppSSLPort.BackColor = System.Drawing.Color.Transparent;
            this.labelWebAppSSLPort.Location = new System.Drawing.Point(189, 254);
            this.labelWebAppSSLPort.Name = "labelWebAppSSLPort";
            this.labelWebAppSSLPort.Size = new System.Drawing.Size(212, 13);
            this.labelWebAppSSLPort.TabIndex = 6;
            this.labelWebAppSSLPort.Text = "Idera Dashboard Web Application SSL Port";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(189, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(215, 21);
            this.label3.TabIndex = 50;
            this.label3.Text = "Idera SQL Compliance Manager Setup\r\n";
            // 
            // buttonBack
            // 
            this.buttonBack.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.buttonBack.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonBack.Location = new System.Drawing.Point(332, 408);
            this.buttonBack.Name = "buttonBack";
            this.buttonBack.Size = new System.Drawing.Size(72, 22);
            this.buttonBack.TabIndex = 6;
            this.buttonBack.Text = "Back\r\n";
            this.buttonBack.UseVisualStyleBackColor = true;
            this.buttonBack.Click += new System.EventHandler(this.buttonBack_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.buttonCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonCancel.Location = new System.Drawing.Point(499, 408);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(72, 22);
            this.buttonCancel.TabIndex = 7;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonNext
            // 
            this.buttonNext.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.buttonNext.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonNext.Location = new System.Drawing.Point(407, 408);
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.Size = new System.Drawing.Size(72, 22);
            this.buttonNext.TabIndex = 5;
            this.buttonNext.Text = "Next";
            this.buttonNext.UseVisualStyleBackColor = true;
            this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
            // 
            // PortForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::Installer_form_application.Properties.Resources.Main_Background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(584, 442);
            this.Controls.Add(this.buttonBack);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonNext);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxWebAppSSLPort);
            this.Controls.Add(this.labelWebAppSSLPort);
            this.Controls.Add(this.textBoxWebAppMonitorPort);
            this.Controls.Add(this.labelWebAppMonitorPort);
            this.Controls.Add(this.textBoxWebAppServicePort);
            this.Controls.Add(this.labelWebAppServicePort);
            this.Controls.Add(this.labelCoreServicePort);
            this.Controls.Add(this.textBoxCoreServicesPort);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PortForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Idera SQL Compliance Manager Setup";
            this.Load += new System.EventHandler(this.PortForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxCoreServicesPort;
        private System.Windows.Forms.Label labelCoreServicePort;
        private System.Windows.Forms.Label labelWebAppServicePort;
        private System.Windows.Forms.TextBox textBoxWebAppServicePort;
        private System.Windows.Forms.Label labelWebAppMonitorPort;
        private System.Windows.Forms.TextBox textBoxWebAppMonitorPort;
        private System.Windows.Forms.TextBox textBoxWebAppSSLPort;
        private System.Windows.Forms.Label labelWebAppSSLPort;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonBack;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonNext;
    }
}