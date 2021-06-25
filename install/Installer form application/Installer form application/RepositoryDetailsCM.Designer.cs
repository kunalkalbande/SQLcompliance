namespace Installer_form_application
{
    partial class RepositoryDetailsCM
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RepositoryDetailsCM));
            this.buttonChange = new System.Windows.Forms.Button();
            this.checkBoxUseAuth = new System.Windows.Forms.CheckBox();
            this.labelNote = new System.Windows.Forms.Label();
            this.textBoxCMDBName = new System.Windows.Forms.TextBox();
            this.textBoxCMInstance = new System.Windows.Forms.TextBox();
            this.labelJMDBName = new System.Windows.Forms.Label();
            this.labelJMInstance = new System.Windows.Forms.Label();
            this.labelCM = new System.Windows.Forms.Label();
            this.labelDesc = new System.Windows.Forms.Label();
            this.buttonBack = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonNext = new System.Windows.Forms.Button();
            this.labelHeading = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonChange
            // 
            this.buttonChange.Enabled = false;
            this.buttonChange.Location = new System.Drawing.Point(425, 326);
            this.buttonChange.Name = "buttonChange";
            this.buttonChange.Size = new System.Drawing.Size(75, 23);
            this.buttonChange.TabIndex = 6;
            this.buttonChange.Text = "Change";
            this.buttonChange.UseVisualStyleBackColor = true;
            this.buttonChange.Click += new System.EventHandler(this.buttonChange_Click);
            // 
            // checkBoxUseAuth
            // 
            this.checkBoxUseAuth.AutoSize = true;
            this.checkBoxUseAuth.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxUseAuth.Location = new System.Drawing.Point(195, 330);
            this.checkBoxUseAuth.Name = "checkBoxUseAuth";
            this.checkBoxUseAuth.Size = new System.Drawing.Size(220, 17);
            this.checkBoxUseAuth.TabIndex = 5;
            this.checkBoxUseAuth.Text = "Use Microsoft SQL Server Authentication";
            this.checkBoxUseAuth.UseVisualStyleBackColor = false;
            this.checkBoxUseAuth.CheckedChanged += new System.EventHandler(this.checkBoxUseAuth_CheckedChanged);
            // 
            // labelNote
            // 
            this.labelNote.BackColor = System.Drawing.Color.Transparent;
            this.labelNote.Location = new System.Drawing.Point(192, 197);
            this.labelNote.Name = "labelNote";
            this.labelNote.Size = new System.Drawing.Size(368, 39);
            this.labelNote.TabIndex = 79;
            this.labelNote.Text = "Connection Credentials: By default, we will use Windows credentials that you prov" +
    "ided to create the repository. If you like to use different credentials, please " +
    "change it below.";
            // 
            // textBoxCMDBName
            // 
            this.textBoxCMDBName.Location = new System.Drawing.Point(307, 158);
            this.textBoxCMDBName.Name = "textBoxCMDBName";
            this.textBoxCMDBName.Size = new System.Drawing.Size(254, 20);
            this.textBoxCMDBName.TabIndex = 2;
            this.textBoxCMDBName.Text = "SQLcompliance";
            // 
            // textBoxCMInstance
            // 
            this.textBoxCMInstance.Location = new System.Drawing.Point(307, 134);
            this.textBoxCMInstance.Name = "textBoxCMInstance";
            this.textBoxCMInstance.Size = new System.Drawing.Size(254, 20);
            this.textBoxCMInstance.TabIndex = 1;
            this.textBoxCMInstance.Text = "(local)";
            // 
            // labelJMDBName
            // 
            this.labelJMDBName.AutoSize = true;
            this.labelJMDBName.BackColor = System.Drawing.Color.Transparent;
            this.labelJMDBName.Location = new System.Drawing.Point(214, 161);
            this.labelJMDBName.Name = "labelJMDBName";
            this.labelJMDBName.Size = new System.Drawing.Size(87, 13);
            this.labelJMDBName.TabIndex = 71;
            this.labelJMDBName.Text = "Database Name:";
            // 
            // labelJMInstance
            // 
            this.labelJMInstance.AutoSize = true;
            this.labelJMInstance.BackColor = System.Drawing.Color.Transparent;
            this.labelJMInstance.Location = new System.Drawing.Point(192, 137);
            this.labelJMInstance.Name = "labelJMInstance";
            this.labelJMInstance.Size = new System.Drawing.Size(109, 13);
            this.labelJMInstance.TabIndex = 70;
            this.labelJMInstance.Text = "SQL Server Instance:";
            // 
            // labelCM
            // 
            this.labelCM.AutoSize = true;
            this.labelCM.BackColor = System.Drawing.Color.Transparent;
            this.labelCM.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCM.Location = new System.Drawing.Point(192, 107);
            this.labelCM.Name = "labelCM";
            this.labelCM.Size = new System.Drawing.Size(153, 13);
            this.labelCM.TabIndex = 69;
            this.labelCM.Text = "SQL Compliance Manager";
            // 
            // labelDesc
            // 
            this.labelDesc.BackColor = System.Drawing.Color.Transparent;
            this.labelDesc.Location = new System.Drawing.Point(192, 54);
            this.labelDesc.Name = "labelDesc";
            this.labelDesc.Size = new System.Drawing.Size(380, 42);
            this.labelDesc.TabIndex = 68;
            this.labelDesc.Text = "Almost there - we need to get the repository information for SQL Compliance Manag" +
    "er.\nPlease provide the SQL Server Instance and Database Names.";
            // 
            // buttonBack
            // 
            this.buttonBack.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.buttonBack.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonBack.Location = new System.Drawing.Point(333, 408);
            this.buttonBack.Name = "buttonBack";
            this.buttonBack.Size = new System.Drawing.Size(72, 22);
            this.buttonBack.TabIndex = 8;
            this.buttonBack.Text = "Back\r\n";
            this.buttonBack.UseVisualStyleBackColor = true;
            this.buttonBack.Click += new System.EventHandler(this.buttonBack_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.buttonCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonCancel.Location = new System.Drawing.Point(500, 408);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(72, 22);
            this.buttonCancel.TabIndex = 9;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonNext
            // 
            this.buttonNext.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.buttonNext.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonNext.Location = new System.Drawing.Point(408, 408);
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.Size = new System.Drawing.Size(72, 22);
            this.buttonNext.TabIndex = 7;
            this.buttonNext.Text = "Next";
            this.buttonNext.UseVisualStyleBackColor = true;
            this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
            // 
            // labelHeading
            // 
            this.labelHeading.AutoSize = true;
            this.labelHeading.BackColor = System.Drawing.Color.Transparent;
            this.labelHeading.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.labelHeading.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelHeading.Location = new System.Drawing.Point(191, 24);
            this.labelHeading.Name = "labelHeading";
            this.labelHeading.Size = new System.Drawing.Size(287, 21);
            this.labelHeading.TabIndex = 64;
            this.labelHeading.Text = "Idera SQL Compliance Manager Setup";
            // 
            // RepositoryDetailsCM
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::Installer_form_application.Properties.Resources.Main_Background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(584, 442);
            this.Controls.Add(this.buttonChange);
            this.Controls.Add(this.checkBoxUseAuth);
            this.Controls.Add(this.labelNote);
            this.Controls.Add(this.textBoxCMDBName);
            this.Controls.Add(this.textBoxCMInstance);
            this.Controls.Add(this.labelJMDBName);
            this.Controls.Add(this.labelJMInstance);
            this.Controls.Add(this.labelCM);
            this.Controls.Add(this.labelDesc);
            this.Controls.Add(this.buttonBack);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonNext);
            this.Controls.Add(this.labelHeading);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RepositoryDetailsCM";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Idera SQL Compliance Manager Setup";
            this.Load += new System.EventHandler(this.RepositoryDetailsDM_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonChange;
        private System.Windows.Forms.CheckBox checkBoxUseAuth;
        private System.Windows.Forms.Label labelNote;
        private System.Windows.Forms.TextBox textBoxCMDBName;
        private System.Windows.Forms.TextBox textBoxCMInstance;
        private System.Windows.Forms.Label labelJMDBName;
        private System.Windows.Forms.Label labelJMInstance;
        private System.Windows.Forms.Label labelCM;
        private System.Windows.Forms.Label labelDesc;
        private System.Windows.Forms.Button buttonBack;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonNext;
        private System.Windows.Forms.Label labelHeading;
    }
}