namespace CwfAddinInstaller.WizardPages
{
    partial class PageDashboardLocation
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtHost = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtCwfUser = new System.Windows.Forms.TextBox();
            this.txtCwfPassword = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radDashboardRemote = new System.Windows.Forms.RadioButton();
            this.radDashboardLocal = new System.Windows.Forms.RadioButton();
            this.updnPort = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.updnPort)).BeginInit();
            this.SuspendLayout();
            // 
            // txtHost
            // 
            this.txtHost.Location = new System.Drawing.Point(110, 93);
            this.txtHost.Name = "txtHost";
            this.txtHost.Size = new System.Drawing.Size(201, 20);
            this.txtHost.TabIndex = 1;
            this.txtHost.Text = "\r\n";
            this.txtHost.WordWrap = false;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Location = new System.Drawing.Point(3, 146);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(2820, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Enter the IDERA Dashboard Administartor Credentials";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(3, 71);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(308, 19);
            this.label3.TabIndex = 3;
            this.label3.Text = "Enter the IDERA Dashboard Host and Port Details";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 96);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Host:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 168);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(101, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Domain\\UserName:";
            // 
            // txtCwfUser
            // 
            this.txtCwfUser.Location = new System.Drawing.Point(110, 165);
            this.txtCwfUser.Name = "txtCwfUser";
            this.txtCwfUser.Size = new System.Drawing.Size(201, 20);
            this.txtCwfUser.TabIndex = 3;
            // 
            // txtCwfPassword
            // 
            this.txtCwfPassword.Location = new System.Drawing.Point(110, 191);
            this.txtCwfPassword.Name = "txtCwfPassword";
            this.txtCwfPassword.Size = new System.Drawing.Size(201, 20);
            this.txtCwfPassword.TabIndex = 4;
            this.txtCwfPassword.UseSystemPasswordChar = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 194);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Password:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radDashboardRemote);
            this.groupBox1.Controls.Add(this.radDashboardLocal);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(308, 65);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "IDERA Dashboard Options – Install or Register with:";
            // 
            // radDashboardRemote
            // 
            this.radDashboardRemote.AutoSize = true;
            this.radDashboardRemote.Location = new System.Drawing.Point(6, 42);
            this.radDashboardRemote.Name = "radDashboardRemote";
            this.radDashboardRemote.Size = new System.Drawing.Size(153, 17);
            this.radDashboardRemote.TabIndex = 1;
            this.radDashboardRemote.Text = "Remote IDERA Dashboard";
            this.radDashboardRemote.UseVisualStyleBackColor = true;
            // 
            // radDashboardLocal
            // 
            this.radDashboardLocal.AutoSize = true;
            this.radDashboardLocal.Checked = true;
            this.radDashboardLocal.Location = new System.Drawing.Point(6, 19);
            this.radDashboardLocal.Name = "radDashboardLocal";
            this.radDashboardLocal.Size = new System.Drawing.Size(142, 17);
            this.radDashboardLocal.TabIndex = 0;
            this.radDashboardLocal.TabStop = true;
            this.radDashboardLocal.Text = "Local IDERA Dashboard";
            this.radDashboardLocal.UseVisualStyleBackColor = true;
            this.radDashboardLocal.CheckedChanged += new System.EventHandler(this.radDashboard_CheckedChanged);
            // 
            // updnPort
            // 
            this.updnPort.Location = new System.Drawing.Point(110, 119);
            this.updnPort.Maximum = new decimal(new int[] {
            49151,
            0,
            0,
            0});
            this.updnPort.Minimum = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            this.updnPort.Name = "updnPort";
            this.updnPort.Size = new System.Drawing.Size(201, 20);
            this.updnPort.TabIndex = 2;
            this.updnPort.Value = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 121);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(29, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Port:";
            // 
            // PageDashboardLocation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label6);
            this.Controls.Add(this.updnPort);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtCwfPassword);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtCwfUser);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtHost);
            this.Name = "PageDashboardLocation";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.updnPort)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtHost;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtCwfUser;
        private System.Windows.Forms.TextBox txtCwfPassword;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radDashboardLocal;
        private System.Windows.Forms.RadioButton radDashboardRemote;
        private System.Windows.Forms.NumericUpDown updnPort;
        private System.Windows.Forms.Label label6;
    }
}
