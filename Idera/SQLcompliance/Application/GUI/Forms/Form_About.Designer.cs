namespace Idera.SQLcompliance.Application.GUI.Forms
{
	partial class AboutForm
	{
      /// <summary>
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.Container components = null;

      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      protected override void Dispose(bool disposing)
      {
         if (disposing)
         {
            if (components != null)
            {
               components.Dispose();
            }
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
          System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
          this.labelSQLsecure = new System.Windows.Forms.Label();
          this.labelDesc = new System.Windows.Forms.Label();
          this.buttonClose = new System.Windows.Forms.Button();
          this.picSQLsecure = new System.Windows.Forms.PictureBox();
          this.labelBetaVersion = new System.Windows.Forms.Label();
          ((System.ComponentModel.ISupportInitialize)(this.picSQLsecure)).BeginInit();
          this.SuspendLayout();
          // 
          // labelSQLsecure
          // 
          this.labelSQLsecure.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold);
          this.labelSQLsecure.Location = new System.Drawing.Point(208, 16);
          this.labelSQLsecure.Name = "labelSQLsecure";
          this.labelSQLsecure.Size = new System.Drawing.Size(336, 16);
          this.labelSQLsecure.TabIndex = 1;
          this.labelSQLsecure.Text = "SQL Compliance Manager 1.0.300.1234";
          this.labelSQLsecure.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
          // 
          // labelDesc
          // 
          this.labelDesc.Location = new System.Drawing.Point(208, 40);
          this.labelDesc.Name = "labelDesc";
          this.labelDesc.Size = new System.Drawing.Size(344, 64);
          this.labelDesc.TabIndex = 3;
          this.labelDesc.Text = resources.GetString("labelDesc.Text");
          this.labelDesc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
          // 
          // buttonClose
          // 
          this.buttonClose.BackColor = System.Drawing.Color.White;
          this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
          this.buttonClose.FlatStyle = System.Windows.Forms.FlatStyle.System;
          this.buttonClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
          this.buttonClose.Location = new System.Drawing.Point(472, 112);
          this.buttonClose.Name = "buttonClose";
          this.buttonClose.Size = new System.Drawing.Size(72, 24);
          this.buttonClose.TabIndex = 4;
          this.buttonClose.Text = "&OK";
          this.buttonClose.UseVisualStyleBackColor = false;
          this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
          // 
          // picSQLsecure
          // 
          this.picSQLsecure.BackColor = System.Drawing.Color.White;
          this.picSQLsecure.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
          this.picSQLsecure.Image = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.About;
          this.picSQLsecure.Location = new System.Drawing.Point(16, 16);
          this.picSQLsecure.Name = "picSQLsecure";
          this.picSQLsecure.Size = new System.Drawing.Size(184, 80);
          this.picSQLsecure.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
          this.picSQLsecure.TabIndex = 0;
          this.picSQLsecure.TabStop = false;
          // 
          // labelBetaVersion
          // 
          this.labelBetaVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
          this.labelBetaVersion.BackColor = System.Drawing.Color.LightGray;
          this.labelBetaVersion.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
          this.labelBetaVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
          this.labelBetaVersion.ForeColor = System.Drawing.Color.Red;
          this.labelBetaVersion.Location = new System.Drawing.Point(16, 104);
          this.labelBetaVersion.Name = "labelBetaVersion";
          this.labelBetaVersion.Size = new System.Drawing.Size(440, 36);
          this.labelBetaVersion.TabIndex = 19;
          this.labelBetaVersion.Text = "BETA RELEASE";
          this.labelBetaVersion.TextAlign = System.Drawing.ContentAlignment.TopCenter;
          this.labelBetaVersion.Visible = false;
          // 
          // AboutForm
          // 
          this.AcceptButton = this.buttonClose;
          this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
          this.BackColor = System.Drawing.Color.White;
          this.CancelButton = this.buttonClose;
          this.ClientSize = new System.Drawing.Size(554, 144);
          this.Controls.Add(this.labelBetaVersion);
          this.Controls.Add(this.buttonClose);
          this.Controls.Add(this.labelDesc);
          this.Controls.Add(this.labelSQLsecure);
          this.Controls.Add(this.picSQLsecure);
          this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
          this.MaximizeBox = false;
          this.MinimizeBox = false;
          this.Name = "AboutForm";
          this.ShowInTaskbar = false;
          this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
          this.Text = "About SQL Compliance Manager";
          ((System.ComponentModel.ISupportInitialize)(this.picSQLsecure)).EndInit();
          this.ResumeLayout(false);

      }
      #endregion

      private System.Windows.Forms.Label labelDesc;
      private System.Windows.Forms.Button buttonClose;
      private System.Windows.Forms.Label labelSQLsecure;
      private System.Windows.Forms.PictureBox picSQLsecure;
      private System.Windows.Forms.Label labelBetaVersion;

	}
}