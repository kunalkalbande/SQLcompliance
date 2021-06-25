namespace Idera.SQLcompliance.Application.GUI.Controls
{
    partial class PermissionsCheck
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new string[] {
            "",
            "Collection Service has rights to the repository databases.",
            "Waiting"}, "imgPass");
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem(new string[] {
            "",
            "Collection Service has rights to read registry HKLM\\Software\\Idera\\SQLCM." +
                "",
            "Waiting"}, "imgPass");
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem(new string[] {
            "",
            "Collection Service has permissions to collection trace directory.",
            "Waiting"}, "imgFail");
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem(new string[] {
            "",
            "Agent Service has permissions to agent trace directory.",
            "Failed"}, "imgInProgress");
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem(new string[] {
            "",
            "Agent Service has rights to read registry HKLM\\Software\\Idera\\SQLCM.",
            "In Progress"}, "(none)");
            System.Windows.Forms.ListViewItem listViewItem6 = new System.Windows.Forms.ListViewItem(new string[] {
            "",
            "Agent Service has rights to the instance.",
            "Waiting"}, "(none)");
            System.Windows.Forms.ListViewItem listViewItem7 = new System.Windows.Forms.ListViewItem(new string[] {
            "",
            "SQL Server has permissions to the agent trace directory.",
            "Waiting"}, "(none)");
            System.Windows.Forms.ListViewItem listViewItem8 = new System.Windows.Forms.ListViewItem(new string[] {
            "",
            "SQL Server has permissions to the collection trace directory.",
            "Waiting"}, "(none)");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PermissionsCheck));
            this.btnReCheck = new System.Windows.Forms.Button();
            this.lstChecks = new Idera.SQLcompliance.Application.GUI.Controls.OptimizedListView();
            this.colIcon = new System.Windows.Forms.ColumnHeader();
            this.colCheck = new System.Windows.Forms.ColumnHeader();
            this.colStatus = new System.Windows.Forms.ColumnHeader();
            this.imglstCheck = new System.Windows.Forms.ImageList(this.components);
            this.lblProgress = new System.Windows.Forms.Label();
            this.lnkHelp = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // btnReCheck
            // 
            this.btnReCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReCheck.Location = new System.Drawing.Point(355, 3);
            this.btnReCheck.Name = "btnReCheck";
            this.btnReCheck.Size = new System.Drawing.Size(75, 23);
            this.btnReCheck.TabIndex = 11;
            this.btnReCheck.Text = "Re-check";
            this.btnReCheck.UseVisualStyleBackColor = true;
            this.btnReCheck.Click += new System.EventHandler(this.btnReCheck_Click);
            // 
            // lstChecks
            // 
            this.lstChecks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lstChecks.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colIcon,
            this.colCheck,
            this.colStatus});
            this.lstChecks.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4,
            listViewItem5,
            listViewItem6,
            listViewItem7,
            listViewItem8});
            this.lstChecks.Location = new System.Drawing.Point(3, 32);
            this.lstChecks.MultiSelect = false;
            this.lstChecks.Name = "lstChecks";
            this.lstChecks.ShowGroups = false;
            this.lstChecks.ShowItemToolTips = true;
            this.lstChecks.Size = new System.Drawing.Size(427, 217);
            this.lstChecks.SmallImageList = this.imglstCheck;
            this.lstChecks.TabIndex = 10;
            this.lstChecks.UseCompatibleStateImageBehavior = false;
            this.lstChecks.View = System.Windows.Forms.View.Details;
            // 
            // colIcon
            // 
            this.colIcon.Text = "";
            this.colIcon.Width = 24;
            // 
            // colCheck
            // 
            this.colCheck.Text = "Check";
            this.colCheck.Width = 292;
            // 
            // colStatus
            // 
            this.colStatus.Text = "Status";
            this.colStatus.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.colStatus.Width = 68;
            // 
            // imglstCheck
            // 
            this.imglstCheck.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imglstCheck.ImageStream")));
            this.imglstCheck.TransparentColor = System.Drawing.Color.Transparent;
            this.imglstCheck.Images.SetKeyName(0, "imgPass");
            this.imglstCheck.Images.SetKeyName(1, "imgFail");
            this.imglstCheck.Images.SetKeyName(2, "imgInProgress");
            // 
            // lblProgress
            // 
            this.lblProgress.AutoSize = true;
            this.lblProgress.Location = new System.Drawing.Point(3, 8);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(300, 13);
            this.lblProgress.TabIndex = 9;
            this.lblProgress.Text = "Checking permissions, please wait. Total 0, Passed 0, Failed 0";
            // 
            // lnkHelp
            // 
            this.lnkHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lnkHelp.AutoSize = true;
            this.lnkHelp.LinkArea = new System.Windows.Forms.LinkArea(13, 4);
            this.lnkHelp.Location = new System.Drawing.Point(3, 252);
            this.lnkHelp.Name = "lnkHelp";
            this.lnkHelp.Size = new System.Drawing.Size(198, 17);
            this.lnkHelp.TabIndex = 12;
            this.lnkHelp.TabStop = true;
            this.lnkHelp.Text = "Please visit this link for additional help.";
            this.lnkHelp.UseCompatibleTextRendering = true;
            this.lnkHelp.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkHelp_LinkClicked);
            // 
            // PermissionsCheck
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lnkHelp);
            this.Controls.Add(this.btnReCheck);
            this.Controls.Add(this.lstChecks);
            this.Controls.Add(this.lblProgress);
            this.Name = "PermissionsCheck";
            this.Size = new System.Drawing.Size(433, 269);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.PermissionsCheck_HelpRequested);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnReCheck;
        private OptimizedListView lstChecks;
        private System.Windows.Forms.ColumnHeader colIcon;
        private System.Windows.Forms.ColumnHeader colCheck;
        private System.Windows.Forms.ColumnHeader colStatus;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.LinkLabel lnkHelp;
        private System.Windows.Forms.ImageList imglstCheck;
    }
}
