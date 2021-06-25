using System.Windows.Forms;
namespace Installer_form_application
{
    partial class Description
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Description));
            this.buttonBack = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonNext = new System.Windows.Forms.Button();
            this.labelHeading = new System.Windows.Forms.Label();
            this.labelVersionDescription = new System.Windows.Forms.Label();
            this.labelFeatures = new System.Windows.Forms.Label();
            this.labelPathDesc = new System.Windows.Forms.Label();
            this.textBoxSPPath = new System.Windows.Forms.TextBox();
            this.textBoxIDPath = new System.Windows.Forms.TextBox();
            this.labelPathSP = new System.Windows.Forms.Label();
            this.labelPathID = new System.Windows.Forms.Label();
            this.labelInstanceDesc = new System.Windows.Forms.Label();
            this.labelDisplayName = new System.Windows.Forms.Label();
            this.labelInstanceExpl = new System.Windows.Forms.Label();
            this.textBoxDisplayName = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // buttonBack
            // 
            this.buttonBack.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.buttonBack.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonBack.Location = new System.Drawing.Point(333, 408);
            this.buttonBack.Name = "buttonBack";
            this.buttonBack.Size = new System.Drawing.Size(72, 22);
            this.buttonBack.TabIndex = 5;
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
            this.buttonCancel.TabIndex = 6;
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
            this.buttonNext.TabIndex = 4;
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
            this.labelHeading.Location = new System.Drawing.Point(190, 20);
            this.labelHeading.Name = "labelHeading";
            this.labelHeading.Size = new System.Drawing.Size(287, 21);
            this.labelHeading.TabIndex = 46;
            this.labelHeading.Text = "Idera SQL Compliance Manager Setup";
            // 
            // labelVersionDescription
            // 
            this.labelVersionDescription.BackColor = System.Drawing.Color.Transparent;
            this.labelVersionDescription.Location = new System.Drawing.Point(191, 52);
            this.labelVersionDescription.Name = "labelVersionDescription";
            this.labelVersionDescription.Size = new System.Drawing.Size(383, 61);
            this.labelVersionDescription.TabIndex = 47;
            this.labelVersionDescription.Text = "We will be performing the following:\r\n\r\n1. Installing SQL Compliance Manager,\r\n2." +
    " Installing Idera Dashboard.";
            // 
            // labelFeatures
            // 
            this.labelFeatures.BackColor = System.Drawing.Color.Transparent;
            this.labelFeatures.Location = new System.Drawing.Point(191, 113);
            this.labelFeatures.Name = "labelFeatures";
            this.labelFeatures.Size = new System.Drawing.Size(383, 77);
            this.labelFeatures.TabIndex = 48;
            this.labelFeatures.Text = "This will provide you with new features:";
            // 
            // labelPathDesc
            // 
            this.labelPathDesc.BackColor = System.Drawing.Color.Transparent;
            this.labelPathDesc.Location = new System.Drawing.Point(191, 176);
            this.labelPathDesc.Name = "labelPathDesc";
            this.labelPathDesc.Size = new System.Drawing.Size(395, 23);
            this.labelPathDesc.TabIndex = 49;
            this.labelPathDesc.Text = "Please provide us the destination folder where you want to install each program:";
            // 
            // textBoxSPPath
            // 
            this.textBoxSPPath.Location = new System.Drawing.Point(340, 193);
            this.textBoxSPPath.Name = "textBoxSPPath";
            this.textBoxSPPath.Size = new System.Drawing.Size(234, 20);
            this.textBoxSPPath.TabIndex = 1;
            this.textBoxSPPath.Text = "C:\\Program Files\\Idera\\SQLCompliance\\";
            // 
            // textBoxIDPath
            // 
            this.textBoxIDPath.Location = new System.Drawing.Point(340, 219);
            this.textBoxIDPath.Name = "textBoxIDPath";
            this.textBoxIDPath.Size = new System.Drawing.Size(234, 20);
            this.textBoxIDPath.TabIndex = 2;
            this.textBoxIDPath.Text = "C:\\Program Files\\Idera\\Dashboard\\";
            // 
            // labelPathSP
            // 
            this.labelPathSP.AutoSize = true;
            this.labelPathSP.BackColor = System.Drawing.Color.Transparent;
            this.labelPathSP.Location = new System.Drawing.Point(191, 196);
            this.labelPathSP.Name = "labelPathSP";
            this.labelPathSP.Size = new System.Drawing.Size(134, 13);
            this.labelPathSP.TabIndex = 52;
            this.labelPathSP.Text = "SQL Compliance Manager:";
            // 
            // labelPathID
            // 
            this.labelPathID.AutoSize = true;
            this.labelPathID.BackColor = System.Drawing.Color.Transparent;
            this.labelPathID.Location = new System.Drawing.Point(191, 222);
            this.labelPathID.Name = "labelPathID";
            this.labelPathID.Size = new System.Drawing.Size(89, 13);
            this.labelPathID.TabIndex = 53;
            this.labelPathID.Text = "Idera Dashboard:";
            // 
            // labelInstanceDesc
            // 
            this.labelInstanceDesc.BackColor = System.Drawing.Color.Transparent;
            this.labelInstanceDesc.Location = new System.Drawing.Point(191, 257);
            this.labelInstanceDesc.Name = "labelInstanceDesc";
            this.labelInstanceDesc.Size = new System.Drawing.Size(394, 29);
            this.labelInstanceDesc.TabIndex = 55;
            this.labelInstanceDesc.Text = "Provide a unique name for the SQL Compliance Manager instance to be displayed on " +
    "the Idera Dashboard.";
            // 
            // labelDisplayName
            // 
            this.labelDisplayName.AutoSize = true;
            this.labelDisplayName.BackColor = System.Drawing.Color.Transparent;
            this.labelDisplayName.Location = new System.Drawing.Point(191, 295);
            this.labelDisplayName.Name = "labelDisplayName";
            this.labelDisplayName.Size = new System.Drawing.Size(75, 13);
            this.labelDisplayName.TabIndex = 56;
            this.labelDisplayName.Text = "Display Name:";
            // 
            // labelInstanceExpl
            // 
            this.labelInstanceExpl.BackColor = System.Drawing.Color.Transparent;
            this.labelInstanceExpl.Location = new System.Drawing.Point(191, 321);
            this.labelInstanceExpl.Name = "labelInstanceExpl";
            this.labelInstanceExpl.Size = new System.Drawing.Size(367, 46);
            this.labelInstanceExpl.TabIndex = 57;
            this.labelInstanceExpl.Text = "Unique display name helps you to distinguish different instance of the same produ" +
    "ct if you have multiple installs. We recommend display name using location or fu" +
    "nction. \"SqlcmWest\" or \"SqlcmProd\"";
            // 
            // textBoxDisplayName
            // 
            this.textBoxDisplayName.Location = new System.Drawing.Point(296, 289);
            this.textBoxDisplayName.Name = "textBoxDisplayName";
            this.textBoxDisplayName.Size = new System.Drawing.Size(246, 20);
            this.textBoxDisplayName.TabIndex = 3;
            // 
            // Description
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::Installer_form_application.Properties.Resources.Main_Background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(584, 442);
            this.Controls.Add(this.textBoxDisplayName);
            this.Controls.Add(this.labelInstanceExpl);
            this.Controls.Add(this.labelDisplayName);
            this.Controls.Add(this.labelInstanceDesc);
            this.Controls.Add(this.labelPathID);
            this.Controls.Add(this.labelPathSP);
            this.Controls.Add(this.textBoxIDPath);
            this.Controls.Add(this.textBoxSPPath);
            this.Controls.Add(this.labelPathDesc);
            this.Controls.Add(this.labelFeatures);
            this.Controls.Add(this.labelVersionDescription);
            this.Controls.Add(this.labelHeading);
            this.Controls.Add(this.buttonBack);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonNext);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Description";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Idera SQL Compliance Manager Setup";
            this.Load += new System.EventHandler(this.Description_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonBack;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonNext;
        private System.Windows.Forms.Label labelHeading;
        private System.Windows.Forms.Label labelVersionDescription;
        private System.Windows.Forms.Label labelFeatures;
        private System.Windows.Forms.Label labelPathDesc;
        private System.Windows.Forms.TextBox textBoxSPPath;
        private System.Windows.Forms.TextBox textBoxIDPath;
        private System.Windows.Forms.Label labelPathSP;
        private System.Windows.Forms.Label labelPathID;
        private System.Windows.Forms.Label labelInstanceDesc;
        private System.Windows.Forms.Label labelDisplayName;
        private System.Windows.Forms.Label labelInstanceExpl;
        private System.Windows.Forms.TextBox textBoxDisplayName;
    }
}