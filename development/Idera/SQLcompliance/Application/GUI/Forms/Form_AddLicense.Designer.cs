namespace Idera.SQLcompliance.Application.GUI.Forms
{
    partial class Form_AddLicense
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_AddLicense));
            this.button_OK = new System.Windows.Forms.Button();
            this.button_Cancel = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBox_NewKey = new System.Windows.Forms.TextBox();
            this.lnkCustomerPortal = new System.Windows.Forms.LinkLabel();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_OK
            // 
            this.button_OK.Location = new System.Drawing.Point(287, 104);
            this.button_OK.Name = "button_OK";
            this.button_OK.Size = new System.Drawing.Size(75, 23);
            this.button_OK.TabIndex = 2;
            this.button_OK.Text = "&OK";
            this.button_OK.UseVisualStyleBackColor = true;
            this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
            // 
            // button_Cancel
            // 
            this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_Cancel.Location = new System.Drawing.Point(379, 104);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.Size = new System.Drawing.Size(75, 23);
            this.button_Cancel.TabIndex = 3;
            this.button_Cancel.Text = "&Cancel";
            this.button_Cancel.UseVisualStyleBackColor = true;
            this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBox_NewKey);
            this.groupBox2.Location = new System.Drawing.Point(12, 10);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(442, 72);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Enter License Key";
            // 
            // textBox_NewKey
            // 
            this.textBox_NewKey.Location = new System.Drawing.Point(11, 27);
            this.textBox_NewKey.Name = "textBox_NewKey";
            this.textBox_NewKey.Size = new System.Drawing.Size(425, 20);
            this.textBox_NewKey.TabIndex = 0;
            // 
            // lnkCustomerPortal
            // 
            this.lnkCustomerPortal.AutoSize = true;
            this.lnkCustomerPortal.Location = new System.Drawing.Point(9, 109);
            this.lnkCustomerPortal.Name = "lnkCustomerPortal";
            this.lnkCustomerPortal.Size = new System.Drawing.Size(279, 13);
            this.lnkCustomerPortal.TabIndex = 4;
            this.lnkCustomerPortal.TabStop = true;
            this.lnkCustomerPortal.Text = "Access the customer portal to generate new license keys.";
            this.lnkCustomerPortal.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkCustomerPortal_LinkClicked);
            // 
            // Form_AddLicense
            // 
            this.AcceptButton = this.button_OK;
            this.CancelButton = this.button_Cancel;
            this.ClientSize = new System.Drawing.Size(466, 140);
            this.Controls.Add(this.button_OK);
            this.Controls.Add(this.lnkCustomerPortal);
            this.Controls.Add(this.button_Cancel);
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form_AddLicense";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add New License Key";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_OK;
        private System.Windows.Forms.Button button_Cancel;
       private System.Windows.Forms.GroupBox groupBox2;
       private System.Windows.Forms.TextBox textBox_NewKey;
       private System.Windows.Forms.LinkLabel lnkCustomerPortal;
    }
}
