namespace Idera.SQLcompliance.Application.GUI.Forms
{
    partial class Form_AlertPermissionsCheck
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblAgentServicePermissionsCheck = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.lblCollectionServicePermissionsCheck = new System.Windows.Forms.Label();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnYes = new System.Windows.Forms.Button();
            this.btnNo = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(449, 31);
            this.label1.TabIndex = 0;
            this.label1.Text = "Please make sure that following checks are performed before adding new SQL server" +
                " for auditing:";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(41, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(421, 32);
            this.label2.TabIndex = 1;
            this.label2.Text = "Service account under which SQL server service is running has full control permis" +
                "sions for processing agent trace directory.";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.StatusGood_16;
            this.pictureBox1.Location = new System.Drawing.Point(16, 44);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(19, 18);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // lblAgentServicePermissionsCheck
            // 
            this.lblAgentServicePermissionsCheck.Location = new System.Drawing.Point(41, 76);
            this.lblAgentServicePermissionsCheck.Name = "lblAgentServicePermissionsCheck";
            this.lblAgentServicePermissionsCheck.Size = new System.Drawing.Size(421, 32);
            this.lblAgentServicePermissionsCheck.TabIndex = 3;
            this.lblAgentServicePermissionsCheck.Text = "Service account ({0}) under which SQL compliance agent service is running has ful" +
                "l control permissions for processing agent trace directory.";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.StatusGood_16;
            this.pictureBox2.Location = new System.Drawing.Point(16, 76);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(19, 18);
            this.pictureBox2.TabIndex = 4;
            this.pictureBox2.TabStop = false;
            // 
            // lblCollectionServicePermissionsCheck
            // 
            this.lblCollectionServicePermissionsCheck.Location = new System.Drawing.Point(41, 108);
            this.lblCollectionServicePermissionsCheck.Name = "lblCollectionServicePermissionsCheck";
            this.lblCollectionServicePermissionsCheck.Size = new System.Drawing.Size(421, 32);
            this.lblCollectionServicePermissionsCheck.TabIndex = 5;
            this.lblCollectionServicePermissionsCheck.Text = "Service account ({0}) under which SQL compliance collection service is running ha" +
                "s full control permissions for processing collection server trace files director" +
                "y.";
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.StatusGood_16;
            this.pictureBox3.Location = new System.Drawing.Point(16, 108);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(19, 18);
            this.pictureBox3.TabIndex = 6;
            this.pictureBox3.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 161);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(301, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Are you sure you want to register new SQL server for auditing?";
            // 
            // btnYes
            // 
            this.btnYes.Location = new System.Drawing.Point(306, 177);
            this.btnYes.Name = "btnYes";
            this.btnYes.Size = new System.Drawing.Size(75, 23);
            this.btnYes.TabIndex = 0;
            this.btnYes.Text = "Yes";
            this.btnYes.UseVisualStyleBackColor = true;
            this.btnYes.Click += new System.EventHandler(this.btnYes_Click);
            // 
            // btnNo
            // 
            this.btnNo.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnNo.Location = new System.Drawing.Point(387, 177);
            this.btnNo.Name = "btnNo";
            this.btnNo.Size = new System.Drawing.Size(75, 23);
            this.btnNo.TabIndex = 1;
            this.btnNo.Text = "No";
            this.btnNo.UseVisualStyleBackColor = true;
            this.btnNo.Click += new System.EventHandler(this.btnNo_Click);
            // 
            // Form_AlertPermissionsCheck
            // 
            this.AcceptButton = this.btnYes;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnNo;
            this.ClientSize = new System.Drawing.Size(474, 212);
            this.ControlBox = false;
            this.Controls.Add(this.btnNo);
            this.Controls.Add(this.btnYes);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.lblCollectionServicePermissionsCheck);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.lblAgentServicePermissionsCheck);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "Form_AlertPermissionsCheck";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Register New SQL Server";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblAgentServicePermissionsCheck;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label lblCollectionServicePermissionsCheck;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnYes;
        private System.Windows.Forms.Button btnNo;
    }
}