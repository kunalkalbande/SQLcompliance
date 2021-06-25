namespace CwfAddinInstaller.WizardPages
{
    partial class PageRepositoryDatabase
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtRepositoryDatabase = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.chkSqlAuthentication = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtSqlUser = new System.Windows.Forms.TextBox();
            this.txtSqlPassword = new System.Windows.Forms.TextBox();
            this.txtSqlServer = new System.Windows.Forms.TextBox();
            this.btnSelectServer = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(1250, 32);
            this.label1.TabIndex = 0;
            this.label1.Text = "Specify the host SQL server and name ot the Repository database.";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(109, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "SQL Server Instance:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 66);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Database Name:";
            // 
            // txtRepositoryDatabase
            // 
            this.txtRepositoryDatabase.Enabled = false;
            this.txtRepositoryDatabase.Location = new System.Drawing.Point(118, 63);
            this.txtRepositoryDatabase.Name = "txtRepositoryDatabase";
            this.txtRepositoryDatabase.Size = new System.Drawing.Size(193, 20);
            this.txtRepositoryDatabase.TabIndex = 2;
            this.txtRepositoryDatabase.Text = "SQLcompliance";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(3, 96);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(308, 31);
            this.label4.TabIndex = 5;
            this.label4.Text = "By default, credentials of you windows logon account are used to connect the Repo" +
    "sitory database.";
            // 
            // chkSqlAuthentication
            // 
            this.chkSqlAuthentication.AutoSize = true;
            this.chkSqlAuthentication.Location = new System.Drawing.Point(3, 130);
            this.chkSqlAuthentication.Name = "chkSqlAuthentication";
            this.chkSqlAuthentication.Size = new System.Drawing.Size(220, 17);
            this.chkSqlAuthentication.TabIndex = 3;
            this.chkSqlAuthentication.Text = "Use Microsoft SQL Server Authentication";
            this.chkSqlAuthentication.UseVisualStyleBackColor = true;
            this.chkSqlAuthentication.CheckedChanged += new System.EventHandler(this.chkSqlAuthentication_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 156);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(63, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "User Name:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 182);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "Password:";
            // 
            // txtSqlUser
            // 
            this.txtSqlUser.Enabled = false;
            this.txtSqlUser.Location = new System.Drawing.Point(72, 153);
            this.txtSqlUser.Name = "txtSqlUser";
            this.txtSqlUser.Size = new System.Drawing.Size(239, 20);
            this.txtSqlUser.TabIndex = 4;
            this.txtSqlUser.Text = "sa";
            // 
            // txtSqlPassword
            // 
            this.txtSqlPassword.Enabled = false;
            this.txtSqlPassword.Location = new System.Drawing.Point(72, 179);
            this.txtSqlPassword.Name = "txtSqlPassword";
            this.txtSqlPassword.Size = new System.Drawing.Size(239, 20);
            this.txtSqlPassword.TabIndex = 5;
            this.txtSqlPassword.UseSystemPasswordChar = true;
            // 
            // txtSqlServer
            // 
            this.txtSqlServer.Location = new System.Drawing.Point(118, 35);
            this.txtSqlServer.Name = "txtSqlServer";
            this.txtSqlServer.Size = new System.Drawing.Size(159, 20);
            this.txtSqlServer.TabIndex = 0;
            // 
            // btnSelectServer
            // 
            this.btnSelectServer.Location = new System.Drawing.Point(283, 34);
            this.btnSelectServer.Name = "btnSelectServer";
            this.btnSelectServer.Size = new System.Drawing.Size(28, 20);
            this.btnSelectServer.TabIndex = 1;
            this.btnSelectServer.Text = "...";
            this.btnSelectServer.UseVisualStyleBackColor = true;
            this.btnSelectServer.Click += new System.EventHandler(this.btnSelectServer_Click);
            // 
            // PageRepositoryDatabase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnSelectServer);
            this.Controls.Add(this.txtSqlServer);
            this.Controls.Add(this.txtSqlPassword);
            this.Controls.Add(this.txtSqlUser);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.chkSqlAuthentication);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtRepositoryDatabase);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "PageRepositoryDatabase";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtRepositoryDatabase;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkSqlAuthentication;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtSqlUser;
        private System.Windows.Forms.TextBox txtSqlPassword;
        private System.Windows.Forms.TextBox txtSqlServer;
        private System.Windows.Forms.Button btnSelectServer;
    }
}
