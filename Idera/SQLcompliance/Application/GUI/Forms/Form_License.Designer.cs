namespace Idera.SQLcompliance.Application.GUI.Forms
{
    partial class Form_License
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button_Delete = new System.Windows.Forms.Button();
            this.button_Add = new System.Windows.Forms.Button();
            this.listview_Licenses = new System.Windows.Forms.ListView();
            this.columnHeader_Key = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader_NumServers = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader_Expiration = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textBox_LicenseType = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_DaysToExpire = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_LicenseExpiration = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox_LicensedServers = new System.Windows.Forms.TextBox();
            this.textBox_LicensedFor = new System.Windows.Forms.TextBox();
            this.button_OK = new System.Windows.Forms.Button();
            this._btn_Help = new System.Windows.Forms.Button();
            this.lnkCustomerPortal = new System.Windows.Forms.LinkLabel();
            this.button_LicenseManager = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.button_LicenseManager);
            this.groupBox1.Controls.Add(this.button_Delete);
            this.groupBox1.Controls.Add(this.button_Add);
            this.groupBox1.Controls.Add(this.listview_Licenses);
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Location = new System.Drawing.Point(11, 11);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(479, 426);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Registered Licenses ";
            // 
            // button_Delete
            // 
            this.button_Delete.Location = new System.Drawing.Point(87, 364);
            this.button_Delete.Name = "button_Delete";
            this.button_Delete.Size = new System.Drawing.Size(75, 23);
            this.button_Delete.TabIndex = 2;
            this.button_Delete.Text = "&Delete";
            this.button_Delete.UseVisualStyleBackColor = true;
            this.button_Delete.Click += new System.EventHandler(this.button_Delete_Click);
            // 
            // button_Add
            // 
            this.button_Add.Location = new System.Drawing.Point(6, 364);
            this.button_Add.Name = "button_Add";
            this.button_Add.Size = new System.Drawing.Size(75, 23);
            this.button_Add.TabIndex = 1;
            this.button_Add.Text = "&Add";
            this.button_Add.UseVisualStyleBackColor = true;
            this.button_Add.Click += new System.EventHandler(this.button_Add_Click);
            // 
            // listview_Licenses
            // 
            this.listview_Licenses.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listview_Licenses.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader_Key,
            this.columnHeader_NumServers,
            this.columnHeader_Expiration});
            this.listview_Licenses.FullRowSelect = true;
            this.listview_Licenses.HideSelection = false;
            this.listview_Licenses.LabelWrap = false;
            this.listview_Licenses.Location = new System.Drawing.Point(6, 19);
            this.listview_Licenses.MultiSelect = false;
            this.listview_Licenses.Name = "listview_Licenses";
            this.listview_Licenses.Size = new System.Drawing.Size(464, 142);
            this.listview_Licenses.TabIndex = 0;
            this.listview_Licenses.UseCompatibleStateImageBehavior = false;
            this.listview_Licenses.View = System.Windows.Forms.View.Details;
            this.listview_Licenses.SelectedIndexChanged += new System.EventHandler(this.listview_Licenses_SelectedIndexChanged);
            // 
            // columnHeader_Key
            // 
            this.columnHeader_Key.Text = "License String";
            this.columnHeader_Key.Width = 208;
            // 
            // columnHeader_NumServers
            // 
            this.columnHeader_NumServers.Text = "Servers";
            this.columnHeader_NumServers.Width = 111;
            // 
            // columnHeader_Expiration
            // 
            this.columnHeader_Expiration.Text = "Days to Expiration";
            this.columnHeader_Expiration.Width = 130;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.textBox_LicenseType);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.textBox_DaysToExpire);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.textBox_LicenseExpiration);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.textBox_LicensedServers);
            this.groupBox3.Controls.Add(this.textBox_LicensedFor);
            this.groupBox3.Location = new System.Drawing.Point(6, 167);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(464, 156);
            this.groupBox3.TabIndex = 14;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "License Details";
            // 
            // textBox_LicenseType
            // 
            this.textBox_LicenseType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_LicenseType.Location = new System.Drawing.Point(106, 19);
            this.textBox_LicenseType.Name = "textBox_LicenseType";
            this.textBox_LicenseType.ReadOnly = true;
            this.textBox_LicenseType.Size = new System.Drawing.Size(352, 20);
            this.textBox_LicenseType.TabIndex = 6;
            this.textBox_LicenseType.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Type:";
            // 
            // textBox_DaysToExpire
            // 
            this.textBox_DaysToExpire.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_DaysToExpire.Location = new System.Drawing.Point(106, 97);
            this.textBox_DaysToExpire.Name = "textBox_DaysToExpire";
            this.textBox_DaysToExpire.ReadOnly = true;
            this.textBox_DaysToExpire.Size = new System.Drawing.Size(352, 20);
            this.textBox_DaysToExpire.TabIndex = 11;
            this.textBox_DaysToExpire.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 126);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Licensed For:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 100);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(95, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Days to Expiration:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 48);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Servers:";
            // 
            // textBox_LicenseExpiration
            // 
            this.textBox_LicenseExpiration.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_LicenseExpiration.Location = new System.Drawing.Point(106, 71);
            this.textBox_LicenseExpiration.Name = "textBox_LicenseExpiration";
            this.textBox_LicenseExpiration.ReadOnly = true;
            this.textBox_LicenseExpiration.Size = new System.Drawing.Size(352, 20);
            this.textBox_LicenseExpiration.TabIndex = 9;
            this.textBox_LicenseExpiration.TabStop = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 74);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(61, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Expires On:";
            // 
            // textBox_LicensedServers
            // 
            this.textBox_LicensedServers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_LicensedServers.Location = new System.Drawing.Point(106, 45);
            this.textBox_LicensedServers.Name = "textBox_LicensedServers";
            this.textBox_LicensedServers.ReadOnly = true;
            this.textBox_LicensedServers.Size = new System.Drawing.Size(352, 20);
            this.textBox_LicensedServers.TabIndex = 8;
            this.textBox_LicensedServers.TabStop = false;
            // 
            // textBox_LicensedFor
            // 
            this.textBox_LicensedFor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_LicensedFor.Location = new System.Drawing.Point(106, 123);
            this.textBox_LicensedFor.Name = "textBox_LicensedFor";
            this.textBox_LicensedFor.ReadOnly = true;
            this.textBox_LicensedFor.Size = new System.Drawing.Size(352, 20);
            this.textBox_LicensedFor.TabIndex = 7;
            this.textBox_LicensedFor.TabStop = false;
            // 
            // button_OK
            // 
            this.button_OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_OK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_OK.Location = new System.Drawing.Point(334, 443);
            this.button_OK.Name = "button_OK";
            this.button_OK.Size = new System.Drawing.Size(75, 23);
            this.button_OK.TabIndex = 1;
            this.button_OK.Text = "&OK";
            this.button_OK.UseVisualStyleBackColor = true;
            this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
            // 
            // _btn_Help
            // 
            this._btn_Help.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._btn_Help.Location = new System.Drawing.Point(415, 443);
            this._btn_Help.Name = "_btn_Help";
            this._btn_Help.Size = new System.Drawing.Size(75, 23);
            this._btn_Help.TabIndex = 2;
            this._btn_Help.Text = "&Help";
            this._btn_Help.UseVisualStyleBackColor = true;
            this._btn_Help.Click += new System.EventHandler(this._btn_Help_Click);
            // 
            // lnkCustomerPortal
            // 
            this.lnkCustomerPortal.AutoSize = true;
            this.lnkCustomerPortal.Location = new System.Drawing.Point(11, 408);
            this.lnkCustomerPortal.Name = "lnkCustomerPortal";
            this.lnkCustomerPortal.Size = new System.Drawing.Size(279, 13);
            this.lnkCustomerPortal.TabIndex = 3;
            this.lnkCustomerPortal.TabStop = true;
            this.lnkCustomerPortal.Text = "Access the customer portal to generate new license keys.";
            this.lnkCustomerPortal.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkCustomerPortal_LinkClicked);
            // 
            // button_LicenseManager
            // 
            this.button_LicenseManager.Location = new System.Drawing.Point(8, 329);
            this.button_LicenseManager.Name = "button_LicenseManager";
            this.button_LicenseManager.Size = new System.Drawing.Size(116, 23);
            this.button_LicenseManager.TabIndex = 15;
            this.button_LicenseManager.Text = "&License Manager";
            this.button_LicenseManager.UseVisualStyleBackColor = true;
            this.button_LicenseManager.Click += new System.EventHandler(this.button_LicenseManager_Click);
            // 
            // Form_License
            // 
            this.AcceptButton = this.button_OK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button_OK;
            this.ClientSize = new System.Drawing.Size(501, 477);
            this.Controls.Add(this.lnkCustomerPortal);
            this.Controls.Add(this._btn_Help);
            this.Controls.Add(this.button_OK);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form_License";
            this.Padding = new System.Windows.Forms.Padding(8);
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Manage Licenses";
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.Form_License_HelpRequested);
            this.groupBox1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBox_LicenseType;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_LicenseExpiration;
        private System.Windows.Forms.TextBox textBox_LicensedServers;
        private System.Windows.Forms.TextBox textBox_LicensedFor;
        private System.Windows.Forms.TextBox textBox_DaysToExpire;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button button_OK;
        private System.Windows.Forms.Button button_Delete;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ListView listview_Licenses;
        private System.Windows.Forms.ColumnHeader columnHeader_Key;
        private System.Windows.Forms.ColumnHeader columnHeader_NumServers;
        private System.Windows.Forms.Button button_Add;
        private System.Windows.Forms.ColumnHeader columnHeader_Expiration;
        private System.Windows.Forms.Button _btn_Help;
        private System.Windows.Forms.LinkLabel lnkCustomerPortal;
        private System.Windows.Forms.Button button_LicenseManager;


    }
}