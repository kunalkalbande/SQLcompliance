namespace Installer_form_application
{
    partial class RepositoryDetails
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RepositoryDetails));
            this.labelHeading = new System.Windows.Forms.Label();
            this.buttonBack = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonNext = new System.Windows.Forms.Button();
            this.labelDesc1 = new System.Windows.Forms.Label();
            this.labelID = new System.Windows.Forms.Label();
            this.buttonChangeID = new System.Windows.Forms.Button();
            this.checkBoxUseAuthID = new System.Windows.Forms.CheckBox();
            this.labelDesc2 = new System.Windows.Forms.Label();
            this.textBoxIDDBName = new System.Windows.Forms.TextBox();
            this.textBoxIDInstance = new System.Windows.Forms.TextBox();
            this.labelDBName = new System.Windows.Forms.Label();
            this.labelIDInstance = new System.Windows.Forms.Label();
            this.labelTagging = new System.Windows.Forms.Label();
            this.checkBoxTagging = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // labelHeading
            // 
            this.labelHeading.AutoSize = true;
            this.labelHeading.BackColor = System.Drawing.Color.Transparent;
            this.labelHeading.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.labelHeading.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelHeading.Location = new System.Drawing.Point(189, 25);
            this.labelHeading.Name = "labelHeading";
            this.labelHeading.Size = new System.Drawing.Size(287, 21);
            this.labelHeading.TabIndex = 34;
            this.labelHeading.Text = "Idera SQL Compliance Manager Setup";
            // 
            // buttonBack
            // 
            this.buttonBack.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.buttonBack.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonBack.Location = new System.Drawing.Point(333, 408);
            this.buttonBack.Name = "buttonBack";
            this.buttonBack.Size = new System.Drawing.Size(72, 22);
            this.buttonBack.TabIndex = 6;
            this.buttonBack.Text = "Back\r\n";
            this.buttonBack.UseVisualStyleBackColor = true;
            this.buttonBack.Click += new System.EventHandler(this.buttonBack_Click_1);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.buttonCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonCancel.Location = new System.Drawing.Point(500, 408);
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
            this.buttonNext.Location = new System.Drawing.Point(408, 408);
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.Size = new System.Drawing.Size(72, 22);
            this.buttonNext.TabIndex = 5;
            this.buttonNext.Text = "Next";
            this.buttonNext.UseVisualStyleBackColor = true;
            this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
            // 
            // labelDesc1
            // 
            this.labelDesc1.BackColor = System.Drawing.Color.Transparent;
            this.labelDesc1.Location = new System.Drawing.Point(190, 55);
            this.labelDesc1.Name = "labelDesc1";
            this.labelDesc1.Size = new System.Drawing.Size(369, 42);
            this.labelDesc1.TabIndex = 46;
            this.labelDesc1.Text = "Next we need to create the Idera Dashboard Repository.  Please provide the instan" +
    "ce and database names below.";
            // 
            // labelID
            // 
            this.labelID.AutoSize = true;
            this.labelID.BackColor = System.Drawing.Color.Transparent;
            this.labelID.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelID.Location = new System.Drawing.Point(190, 113);
            this.labelID.Name = "labelID";
            this.labelID.Size = new System.Drawing.Size(165, 13);
            this.labelID.TabIndex = 56;
            this.labelID.Text = "Idera Dashboard Repository\r\n";
            // 
            // buttonChangeID
            // 
            this.buttonChangeID.Enabled = false;
            this.buttonChangeID.Location = new System.Drawing.Point(421, 237);
            this.buttonChangeID.Name = "buttonChangeID";
            this.buttonChangeID.Size = new System.Drawing.Size(75, 23);
            this.buttonChangeID.TabIndex = 4;
            this.buttonChangeID.Text = "Change";
            this.buttonChangeID.UseVisualStyleBackColor = true;
            this.buttonChangeID.Click += new System.EventHandler(this.button2_Click);
            // 
            // checkBoxUseAuthID
            // 
            this.checkBoxUseAuthID.AutoSize = true;
            this.checkBoxUseAuthID.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxUseAuthID.Location = new System.Drawing.Point(193, 241);
            this.checkBoxUseAuthID.Name = "checkBoxUseAuthID";
            this.checkBoxUseAuthID.Size = new System.Drawing.Size(220, 17);
            this.checkBoxUseAuthID.TabIndex = 3;
            this.checkBoxUseAuthID.Text = "Use Microsoft SQL Server Authentication";
            this.checkBoxUseAuthID.UseVisualStyleBackColor = false;
            this.checkBoxUseAuthID.CheckedChanged += new System.EventHandler(this.checkBoxUseAuthID_CheckedChanged);
            // 
            // labelDesc2
            // 
            this.labelDesc2.BackColor = System.Drawing.Color.Transparent;
            this.labelDesc2.Location = new System.Drawing.Point(190, 187);
            this.labelDesc2.Name = "labelDesc2";
            this.labelDesc2.Size = new System.Drawing.Size(375, 40);
            this.labelDesc2.TabIndex = 61;
            this.labelDesc2.Text = "Connection Credentials: By default, we will use Windows credentials that you prov" +
    "ided to create the repository. If you like to use different credentials, please " +
    "change it below.";
            // 
            // textBoxIDDBName
            // 
            this.textBoxIDDBName.Location = new System.Drawing.Point(305, 156);
            this.textBoxIDDBName.Name = "textBoxIDDBName";
            this.textBoxIDDBName.Size = new System.Drawing.Size(254, 20);
            this.textBoxIDDBName.TabIndex = 2;
            this.textBoxIDDBName.Text = "IderaDashboardRepository";
            // 
            // textBoxIDInstance
            // 
            this.textBoxIDInstance.Location = new System.Drawing.Point(305, 132);
            this.textBoxIDInstance.Name = "textBoxIDInstance";
            this.textBoxIDInstance.Size = new System.Drawing.Size(254, 20);
            this.textBoxIDInstance.TabIndex = 1;
            this.textBoxIDInstance.Text = "(local)";
            // 
            // labelDBName
            // 
            this.labelDBName.AutoSize = true;
            this.labelDBName.BackColor = System.Drawing.Color.Transparent;
            this.labelDBName.Location = new System.Drawing.Point(212, 159);
            this.labelDBName.Name = "labelDBName";
            this.labelDBName.Size = new System.Drawing.Size(87, 13);
            this.labelDBName.TabIndex = 58;
            this.labelDBName.Text = "Database Name:";
            // 
            // labelIDInstance
            // 
            this.labelIDInstance.AutoSize = true;
            this.labelIDInstance.BackColor = System.Drawing.Color.Transparent;
            this.labelIDInstance.Location = new System.Drawing.Point(190, 135);
            this.labelIDInstance.Name = "labelIDInstance";
            this.labelIDInstance.Size = new System.Drawing.Size(109, 13);
            this.labelIDInstance.TabIndex = 57;
            this.labelIDInstance.Text = "SQL Server Instance:";
            // 
            // labelTagging
            // 
            this.labelTagging.AutoSize = true;
            this.labelTagging.BackColor = System.Drawing.Color.Transparent;
            this.labelTagging.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTagging.Location = new System.Drawing.Point(177, 274);
            this.labelTagging.Name = "labelTagging";
            this.labelTagging.Size = new System.Drawing.Size(53, 13);
            this.labelTagging.TabIndex = 62;
            this.labelTagging.Text = "Tagging";
            this.labelTagging.Visible = false;
            // 
            // checkBoxTagging
            // 
            this.checkBoxTagging.AutoSize = true;
            this.checkBoxTagging.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxTagging.Location = new System.Drawing.Point(193, 299);
            this.checkBoxTagging.Name = "checkBoxTagging";
            this.checkBoxTagging.Size = new System.Drawing.Size(266, 17);
            this.checkBoxTagging.TabIndex = 63;
            this.checkBoxTagging.Text = "Enable Tagging in Idera SQL Compliance Manager";
            this.checkBoxTagging.UseVisualStyleBackColor = false;
            this.checkBoxTagging.Visible = false;
            // 
            // RepositoryDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::Installer_form_application.Properties.Resources.Main_Background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(584, 442);
            this.Controls.Add(this.checkBoxTagging);
            this.Controls.Add(this.labelTagging);
            this.Controls.Add(this.buttonChangeID);
            this.Controls.Add(this.checkBoxUseAuthID);
            this.Controls.Add(this.labelDesc2);
            this.Controls.Add(this.textBoxIDDBName);
            this.Controls.Add(this.textBoxIDInstance);
            this.Controls.Add(this.labelDBName);
            this.Controls.Add(this.labelIDInstance);
            this.Controls.Add(this.labelID);
            this.Controls.Add(this.labelDesc1);
            this.Controls.Add(this.buttonBack);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonNext);
            this.Controls.Add(this.labelHeading);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RepositoryDetails";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Idera SQL Compliance Manager Setup";
            this.Load += new System.EventHandler(this.RepositoryDetails_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelHeading;
        private System.Windows.Forms.Button buttonBack;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonNext;
        private System.Windows.Forms.Label labelDesc1;
        private System.Windows.Forms.Label labelID;
        private System.Windows.Forms.Button buttonChangeID;
        private System.Windows.Forms.CheckBox checkBoxUseAuthID;
        private System.Windows.Forms.Label labelDesc2;
        private System.Windows.Forms.TextBox textBoxIDDBName;
        private System.Windows.Forms.TextBox textBoxIDInstance;
        private System.Windows.Forms.Label labelDBName;
        private System.Windows.Forms.Label labelIDInstance;
        private System.Windows.Forms.Label labelTagging;
        private System.Windows.Forms.CheckBox checkBoxTagging;
    }
}