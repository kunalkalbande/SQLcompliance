namespace Idera.SQLcompliance.Application.GUI.Forms
{
    partial class Form_PermissionsCheck
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
            this.lstAuditedSqlServers = new System.Windows.Forms.CheckedListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblVerticalSeperator = new System.Windows.Forms.Label();
            this.progStatusTotal = new System.Windows.Forms.ProgressBar();
            this.btnCheckPermissions = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.progStatusIndivisual = new System.Windows.Forms.ProgressBar();
            this.lblStatsIndivisual = new System.Windows.Forms.Label();
            this.lblProgressIndivisual = new System.Windows.Forms.Label();
            this.lblProgressTotal = new System.Windows.Forms.Label();
            this.lblStatsTotal = new System.Windows.Forms.Label();
            this.permissionsCheck = new Idera.SQLcompliance.Application.GUI.Controls.PermissionsCheck();
            this.chkShowResolutionSteps = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // lstAuditedSqlServers
            // 
            this.lstAuditedSqlServers.CheckOnClick = true;
            this.lstAuditedSqlServers.FormattingEnabled = true;
            this.lstAuditedSqlServers.Location = new System.Drawing.Point(12, 25);
            this.lstAuditedSqlServers.Name = "lstAuditedSqlServers";
            this.lstAuditedSqlServers.Size = new System.Drawing.Size(232, 289);
            this.lstAuditedSqlServers.TabIndex = 0;
            this.lstAuditedSqlServers.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.lstAuditedSqlServers_ItemCheck);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Audited SQL Servers";
            // 
            // lblVerticalSeperator
            // 
            this.lblVerticalSeperator.BackColor = System.Drawing.Color.Black;
            this.lblVerticalSeperator.Location = new System.Drawing.Point(250, 0);
            this.lblVerticalSeperator.Name = "lblVerticalSeperator";
            this.lblVerticalSeperator.Size = new System.Drawing.Size(1, 395);
            this.lblVerticalSeperator.TabIndex = 2;
            // 
            // progStatusTotal
            // 
            this.progStatusTotal.Location = new System.Drawing.Point(261, 333);
            this.progStatusTotal.Name = "progStatusTotal";
            this.progStatusTotal.Size = new System.Drawing.Size(429, 13);
            this.progStatusTotal.TabIndex = 5;
            // 
            // btnCheckPermissions
            // 
            this.btnCheckPermissions.Location = new System.Drawing.Point(53, 357);
            this.btnCheckPermissions.Name = "btnCheckPermissions";
            this.btnCheckPermissions.Size = new System.Drawing.Size(110, 23);
            this.btnCheckPermissions.TabIndex = 2;
            this.btnCheckPermissions.Text = "Check Permissions";
            this.btnCheckPermissions.UseVisualStyleBackColor = true;
            this.btnCheckPermissions.Click += new System.EventHandler(this.btnCheckPermissions_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(169, 357);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // progStatusIndivisual
            // 
            this.progStatusIndivisual.Location = new System.Drawing.Point(261, 367);
            this.progStatusIndivisual.Name = "progStatusIndivisual";
            this.progStatusIndivisual.Size = new System.Drawing.Size(429, 13);
            this.progStatusIndivisual.TabIndex = 9;
            // 
            // lblStatsIndivisual
            // 
            this.lblStatsIndivisual.Location = new System.Drawing.Point(610, 349);
            this.lblStatsIndivisual.Name = "lblStatsIndivisual";
            this.lblStatsIndivisual.Size = new System.Drawing.Size(80, 20);
            this.lblStatsIndivisual.TabIndex = 11;
            this.lblStatsIndivisual.Text = "0 of 8";
            this.lblStatsIndivisual.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblProgressIndivisual
            // 
            this.lblProgressIndivisual.AutoEllipsis = true;
            this.lblProgressIndivisual.Location = new System.Drawing.Point(261, 353);
            this.lblProgressIndivisual.Name = "lblProgressIndivisual";
            this.lblProgressIndivisual.Size = new System.Drawing.Size(343, 13);
            this.lblProgressIndivisual.TabIndex = 12;
            this.lblProgressIndivisual.Text = "Checking permissions for VikasTak-PC\\SQL2014CTP2.";
            // 
            // lblProgressTotal
            // 
            this.lblProgressTotal.AutoEllipsis = true;
            this.lblProgressTotal.Location = new System.Drawing.Point(261, 319);
            this.lblProgressTotal.Name = "lblProgressTotal";
            this.lblProgressTotal.Size = new System.Drawing.Size(343, 13);
            this.lblProgressTotal.TabIndex = 13;
            this.lblProgressTotal.Text = "Total Servers: 0, Passed Servers: 0, Failed Servers: 0";
            // 
            // lblStatsTotal
            // 
            this.lblStatsTotal.Location = new System.Drawing.Point(610, 315);
            this.lblStatsTotal.Name = "lblStatsTotal";
            this.lblStatsTotal.Size = new System.Drawing.Size(80, 20);
            this.lblStatsTotal.TabIndex = 14;
            this.lblStatsTotal.Text = "0 of 8";
            this.lblStatsTotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // permissionsCheck
            // 
            this.permissionsCheck.IsReCheckAllowed = true;
            this.permissionsCheck.IsResolutionStepsShownOnFailure = true;
            this.permissionsCheck.Location = new System.Drawing.Point(261, 12);
            this.permissionsCheck.Name = "permissionsCheck";
            this.permissionsCheck.Size = new System.Drawing.Size(429, 269);
            this.permissionsCheck.TabIndex = 4;
            this.permissionsCheck.OnPermissionCheckComplete += new Idera.SQLcompliance.Application.GUI.Controls.PermissionsCheck.PermissionCheckComplete(this.permissionsCheck_OnPermissionCheckComplete);
            this.permissionsCheck.OnPermissionsCheckCompleted += new Idera.SQLcompliance.Application.GUI.Controls.PermissionsCheck.PermissionsCheckCompleted(this.permissionsCheck_OnPermissionsCheckCompleted);
            // 
            // chkShowResolutionSteps
            // 
            this.chkShowResolutionSteps.Checked = true;
            this.chkShowResolutionSteps.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkShowResolutionSteps.Location = new System.Drawing.Point(12, 320);
            this.chkShowResolutionSteps.Name = "chkShowResolutionSteps";
            this.chkShowResolutionSteps.Size = new System.Drawing.Size(232, 31);
            this.chkShowResolutionSteps.TabIndex = 1;
            this.chkShowResolutionSteps.Text = "Show resolution steps after permissions check complete.";
            this.chkShowResolutionSteps.UseVisualStyleBackColor = true;
            // 
            // Form_PermissionsCheck
            // 
            this.AcceptButton = this.btnCheckPermissions;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(702, 392);
            this.Controls.Add(this.chkShowResolutionSteps);
            this.Controls.Add(this.progStatusIndivisual);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnCheckPermissions);
            this.Controls.Add(this.progStatusTotal);
            this.Controls.Add(this.permissionsCheck);
            this.Controls.Add(this.lblVerticalSeperator);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lstAuditedSqlServers);
            this.Controls.Add(this.lblStatsTotal);
            this.Controls.Add(this.lblStatsIndivisual);
            this.Controls.Add(this.lblProgressTotal);
            this.Controls.Add(this.lblProgressIndivisual);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form_PermissionsCheck";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Permissions Check";
            this.Shown += new System.EventHandler(this.Form_PermissionsCheck_Shown);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_PermissionsCheck_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox lstAuditedSqlServers;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblVerticalSeperator;
        private Idera.SQLcompliance.Application.GUI.Controls.PermissionsCheck permissionsCheck;
        private System.Windows.Forms.ProgressBar progStatusTotal;
        private System.Windows.Forms.Button btnCheckPermissions;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ProgressBar progStatusIndivisual;
        private System.Windows.Forms.Label lblStatsIndivisual;
        private System.Windows.Forms.Label lblProgressIndivisual;
        private System.Windows.Forms.Label lblProgressTotal;
        private System.Windows.Forms.Label lblStatsTotal;
        private System.Windows.Forms.CheckBox chkShowResolutionSteps;



    }
}