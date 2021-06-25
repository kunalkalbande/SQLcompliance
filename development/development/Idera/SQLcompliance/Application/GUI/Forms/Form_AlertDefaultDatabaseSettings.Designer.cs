namespace Idera.SQLcompliance.Application.GUI.Forms
{
    partial class Form_AlertDefaultDatabaseSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_AlertDefaultDatabaseSettings));
            this.lblWhenDefaultAuditSettingsApplied = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblWhenDefaultAuditSettingsApplied
            // 
            this.lblWhenDefaultAuditSettingsApplied.AutoSize = true;
            this.lblWhenDefaultAuditSettingsApplied.Location = new System.Drawing.Point(28, 19);
            this.lblWhenDefaultAuditSettingsApplied.Name = "lblWhenDefaultAuditSettingsApplied";
            this.lblWhenDefaultAuditSettingsApplied.Size = new System.Drawing.Size(0, 13);
            this.lblWhenDefaultAuditSettingsApplied.TabIndex = 0;
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
            this.button1.Text = "OK"; //SQLCM-5654 text changed
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
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(449, 117);
            this.label1.TabIndex = 5;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // Form_AlertDefaultDatabaseSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(478, 231);
            this.ControlBox = false;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.lblWhenDefaultAuditSettingsApplied);
            this.Name = "Form_AlertDefaultDatabaseSettings";
            this.Text = "When Are Default Audit Settings  Applied ?";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblWhenDefaultAuditSettingsApplied;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
    }
}