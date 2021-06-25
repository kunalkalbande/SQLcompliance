namespace Installer_form_application
{
    partial class InstallScreen
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InstallScreen));
            this.labelHading = new System.Windows.Forms.Label();
            this.labelDesc = new System.Windows.Forms.Label();
            this.labelClickFinish = new System.Windows.Forms.Label();
            this.labelInstruction = new System.Windows.Forms.Label();
            this.buttonFinish = new System.Windows.Forms.Button();
            this.checkBoxLaunchApp = new System.Windows.Forms.CheckBox();
            this.dashboardLink = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // labelHading
            // 
            this.labelHading.AutoSize = true;
            this.labelHading.BackColor = System.Drawing.Color.Transparent;
            this.labelHading.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.labelHading.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelHading.Location = new System.Drawing.Point(190, 26);
            this.labelHading.Name = "labelHading";
            this.labelHading.Size = new System.Drawing.Size(287, 21);
            this.labelHading.TabIndex = 35;
            this.labelHading.Text = "Idera SQL Compliance Manager Setup";
            // 
            // labelDesc
            // 
            this.labelDesc.BackColor = System.Drawing.Color.Transparent;
            this.labelDesc.Location = new System.Drawing.Point(191, 77);
            this.labelDesc.Name = "labelDesc";
            this.labelDesc.Size = new System.Drawing.Size(373, 31);
            this.labelDesc.TabIndex = 36;
            this.labelDesc.Text = "Congratulations! You have completed the SQL Compliance Manager and Idera Dashboar" +
    "d setup process.\r\n";
            // 
            // labelClickFinish
            // 
            this.labelClickFinish.BackColor = System.Drawing.Color.Transparent;
            this.labelClickFinish.Location = new System.Drawing.Point(191, 125);
            this.labelClickFinish.Name = "labelClickFinish";
            this.labelClickFinish.Size = new System.Drawing.Size(214, 19);
            this.labelClickFinish.TabIndex = 37;
            this.labelClickFinish.Text = "Click finish button to exit the setup wizard.\r\n";
            // 
            // labelInstruction
            // 
            this.labelInstruction.BackColor = System.Drawing.Color.Transparent;
            this.labelInstruction.Location = new System.Drawing.Point(191, 159);
            this.labelInstruction.Name = "labelInstruction";
            this.labelInstruction.Size = new System.Drawing.Size(395, 149);
            this.labelInstruction.TabIndex = 38;
            this.labelInstruction.Text = "To launch Idera Dashboard:";
            // 
            // buttonFinish
            // 
            this.buttonFinish.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.buttonFinish.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonFinish.Location = new System.Drawing.Point(500, 408);
            this.buttonFinish.Name = "buttonFinish";
            this.buttonFinish.Size = new System.Drawing.Size(72, 22);
            this.buttonFinish.TabIndex = 2;
            this.buttonFinish.Text = "Finish";
            this.buttonFinish.UseVisualStyleBackColor = true;
            this.buttonFinish.Click += new System.EventHandler(this.buttonFinish_Click);
            // 
            // checkBoxLaunchApp
            // 
            this.checkBoxLaunchApp.AutoSize = true;
            this.checkBoxLaunchApp.Checked = true;
            this.checkBoxLaunchApp.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxLaunchApp.Location = new System.Drawing.Point(12, 412);
            this.checkBoxLaunchApp.Name = "checkBoxLaunchApp";
            this.checkBoxLaunchApp.Size = new System.Drawing.Size(216, 17);
            this.checkBoxLaunchApp.TabIndex = 1;
            this.checkBoxLaunchApp.Text = "Launch Idera SQL Compliance Manager";
            this.checkBoxLaunchApp.UseVisualStyleBackColor = true;
            // 
            // dashboardLink
            // 
            this.dashboardLink.AutoSize = true;
            this.dashboardLink.BackColor = System.Drawing.Color.Transparent;
            this.dashboardLink.Location = new System.Drawing.Point(191, 188);
            this.dashboardLink.Name = "dashboardLink";
            this.dashboardLink.Size = new System.Drawing.Size(0, 13);
            this.dashboardLink.TabIndex = 39;
            this.dashboardLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.dashboardLink_LinkClicked);
            // 
            // InstallScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::Installer_form_application.Properties.Resources.Main_Background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(584, 442);
            this.Controls.Add(this.dashboardLink);
            this.Controls.Add(this.checkBoxLaunchApp);
            this.Controls.Add(this.buttonFinish);
            this.Controls.Add(this.labelInstruction);
            this.Controls.Add(this.labelClickFinish);
            this.Controls.Add(this.labelDesc);
            this.Controls.Add(this.labelHading);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InstallScreen";
            this.Text = "Idera SQL Compliance Manager Setup";
            this.Load += new System.EventHandler(this.InstallScreen_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelHading;
        private System.Windows.Forms.Label labelDesc;
        private System.Windows.Forms.Label labelClickFinish;
        private System.Windows.Forms.Label labelInstruction;
        private System.Windows.Forms.Button buttonFinish;
        private System.Windows.Forms.CheckBox checkBoxLaunchApp;
        private System.Windows.Forms.LinkLabel dashboardLink;
    }
}