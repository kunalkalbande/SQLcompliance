namespace Idera.SQLcompliance.Application.GUI.Forms
{
    partial class Form_ConfirmDefaultAuditSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_ConfirmDefaultAuditSettings));
            this.lblWhenDefaultAuditSettingsApplied = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // lblWhenDefaultAuditSettingsApplied
            // 
            this.lblWhenDefaultAuditSettingsApplied.AutoSize = true;
            this.lblWhenDefaultAuditSettingsApplied.Location = new System.Drawing.Point(23, 46);
            this.lblWhenDefaultAuditSettingsApplied.Name = "lblWhenDefaultAuditSettingsApplied";
            this.lblWhenDefaultAuditSettingsApplied.Size = new System.Drawing.Size(443, 52);
            this.lblWhenDefaultAuditSettingsApplied.TabIndex = 0;
            this.lblWhenDefaultAuditSettingsApplied.Text = resources.GetString("lblWhenDefaultAuditSettingsApplied.Text");
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(31, 188);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(158, 17);
            this.checkBox1.TabIndex = 1;
            this.checkBox1.Text = "Don\'t show this dialog again";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(354, 188);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "OK"; //SQLCM-5654
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.GrayText;
            this.panel1.Location = new System.Drawing.Point(1, 162);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(476, 1);
            this.panel1.TabIndex = 3;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.GrayText;
            this.panel2.Location = new System.Drawing.Point(-2, -9);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(478, 10);
            this.panel2.TabIndex = 4;
            // 
            // Form_ConfirmDefaultAuditSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(478, 217);
            this.ControlBox = false;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.lblWhenDefaultAuditSettingsApplied);
            this.Name = "Form_ConfirmDefaultAuditSettings";
            this.Text = "When Will This Default Audit Settings be Applied ?";
            this.Load += new System.EventHandler(this.Form_ConfirmDefaultAuditSettings_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblWhenDefaultAuditSettingsApplied;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
    }
}