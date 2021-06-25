namespace Idera.SQLcompliance.Application.GUI.Forms
{
    partial class Form_AlertPermissionsCheckFailed
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_AlertPermissionsCheckFailed));
            this.lblMessage = new System.Windows.Forms.Label();
            this.btnIgnore = new System.Windows.Forms.Button();
            this.btnStay = new System.Windows.Forms.Button();
            this.txtResolutionSteps = new System.Windows.Forms.RichTextBox();
            this.lnkHelp = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // lblMessage
            // 
            this.lblMessage.Location = new System.Drawing.Point(9, 9);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(413, 52);
            this.lblMessage.TabIndex = 0;
            this.lblMessage.Text = resources.GetString("lblMessage.Text");
            // 
            // btnIgnore
            // 
            this.btnIgnore.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnIgnore.Location = new System.Drawing.Point(172, 247);
            this.btnIgnore.Name = "btnIgnore";
            this.btnIgnore.Size = new System.Drawing.Size(122, 23);
            this.btnIgnore.TabIndex = 2;
            this.btnIgnore.Text = "Ignore and Continue";
            this.btnIgnore.UseVisualStyleBackColor = true;
            this.btnIgnore.Click += new System.EventHandler(this.btnIgnore_Click);
            // 
            // btnStay
            // 
            this.btnStay.Location = new System.Drawing.Point(300, 247);
            this.btnStay.Name = "btnStay";
            this.btnStay.Size = new System.Drawing.Size(122, 23);
            this.btnStay.TabIndex = 1;
            this.btnStay.Text = "Stay and Re-check";
            this.btnStay.UseVisualStyleBackColor = true;
            this.btnStay.Click += new System.EventHandler(this.btnStay_Click);
            // 
            // txtResolutionSteps
            // 
            this.txtResolutionSteps.BackColor = System.Drawing.Color.White;
            this.txtResolutionSteps.Location = new System.Drawing.Point(12, 64);
            this.txtResolutionSteps.MaxLength = 0;
            this.txtResolutionSteps.Name = "txtResolutionSteps";
            this.txtResolutionSteps.ReadOnly = true;
            this.txtResolutionSteps.Size = new System.Drawing.Size(410, 150);
            this.txtResolutionSteps.TabIndex = 3;
            this.txtResolutionSteps.Text = "";
            this.txtResolutionSteps.WordWrap = false;
            this.txtResolutionSteps.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.txtResolutionSteps_LinkClicked);
            // 
            // lnkHelp
            // 
            this.lnkHelp.LinkArea = new System.Windows.Forms.LinkArea(111, 4);
            this.lnkHelp.Location = new System.Drawing.Point(9, 217);
            this.lnkHelp.Name = "lnkHelp";
            this.lnkHelp.Size = new System.Drawing.Size(413, 27);
            this.lnkHelp.TabIndex = 4;
            this.lnkHelp.TabStop = true;
            this.lnkHelp.Text = "You can still ignore the checks and continue or stay on the wizard page and re-ch" +
                "eck permissions. Please visit this link for additional help.";
            this.lnkHelp.UseCompatibleTextRendering = true;
            this.lnkHelp.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkHelp_LinkClicked);
            // 
            // Form_AlertPermissionsCheckFailed
            // 
            this.AcceptButton = this.btnStay;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnIgnore;
            this.ClientSize = new System.Drawing.Size(434, 282);
            this.ControlBox = false;
            this.Controls.Add(this.lnkHelp);
            this.Controls.Add(this.txtResolutionSteps);
            this.Controls.Add(this.btnStay);
            this.Controls.Add(this.btnIgnore);
            this.Controls.Add(this.lblMessage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "Form_AlertPermissionsCheckFailed";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Permissions Check Failed";
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.Form_AlertPermissionsCheckFailed_HelpRequested);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Button btnIgnore;
        private System.Windows.Forms.Button btnStay;
        private System.Windows.Forms.RichTextBox txtResolutionSteps;
        private System.Windows.Forms.LinkLabel lnkHelp;
    }
}