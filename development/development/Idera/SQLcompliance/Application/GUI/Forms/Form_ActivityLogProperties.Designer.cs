namespace Idera.SQLcompliance.Application.GUI.Forms
{
   partial class Form_ActivityLogProperties
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
         this.btnClose = new System.Windows.Forms.Button();
         this.btnCopy = new System.Windows.Forms.Button();
         this.label4 = new System.Windows.Forms.Label();
         this.label7 = new System.Windows.Forms.Label();
         this.lblTime = new System.Windows.Forms.Label();
         this.txtType = new System.Windows.Forms.TextBox();
         this.txtSQLServer = new System.Windows.Forms.TextBox();
         this.btnNext = new System.Windows.Forms.Button();
         this.btnPrevious = new System.Windows.Forms.Button();
         this.txtTime = new System.Windows.Forms.TextBox();
         this.groupDetails = new System.Windows.Forms.GroupBox();
         this.textDetails = new System.Windows.Forms.TextBox();
         this.groupDetails.SuspendLayout();
         this.SuspendLayout();
         // 
         // btnClose
         // 
         this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.btnClose.Location = new System.Drawing.Point(488, 352);
         this.btnClose.Name = "btnClose";
         this.btnClose.Size = new System.Drawing.Size(56, 24);
         this.btnClose.TabIndex = 63;
         this.btnClose.Text = "&Close";
         this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
         // 
         // btnCopy
         // 
         this.btnCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.btnCopy.Image = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.copy;
         this.btnCopy.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
         this.btnCopy.Location = new System.Drawing.Point(488, 300);
         this.btnCopy.Name = "btnCopy";
         this.btnCopy.Size = new System.Drawing.Size(56, 24);
         this.btnCopy.TabIndex = 62;
         this.btnCopy.Text = "Co&py";
         this.btnCopy.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
         // 
         // label4
         // 
         this.label4.Location = new System.Drawing.Point(244, 16);
         this.label4.Name = "label4";
         this.label4.Size = new System.Drawing.Size(68, 16);
         this.label4.TabIndex = 61;
         this.label4.Tag = "1";
         this.label4.Text = "SQL Server:";
         // 
         // label7
         // 
         this.label7.Location = new System.Drawing.Point(8, 40);
         this.label7.Name = "label7";
         this.label7.Size = new System.Drawing.Size(36, 16);
         this.label7.TabIndex = 60;
         this.label7.Tag = "1";
         this.label7.Text = "Type:";
         // 
         // lblTime
         // 
         this.lblTime.Location = new System.Drawing.Point(8, 16);
         this.lblTime.Name = "lblTime";
         this.lblTime.Size = new System.Drawing.Size(36, 16);
         this.lblTime.TabIndex = 59;
         this.lblTime.Tag = "1";
         this.lblTime.Text = "Time:";
         // 
         // txtType
         // 
         this.txtType.Location = new System.Drawing.Point(48, 36);
         this.txtType.Name = "txtType";
         this.txtType.ReadOnly = true;
         this.txtType.Size = new System.Drawing.Size(176, 20);
         this.txtType.TabIndex = 49;
         this.txtType.Tag = "1";
         // 
         // txtSQLServer
         // 
         this.txtSQLServer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.txtSQLServer.Location = new System.Drawing.Point(312, 12);
         this.txtSQLServer.Name = "txtSQLServer";
         this.txtSQLServer.ReadOnly = true;
         this.txtSQLServer.Size = new System.Drawing.Size(168, 20);
         this.txtSQLServer.TabIndex = 50;
         this.txtSQLServer.Tag = "1";
         // 
         // btnNext
         // 
         this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.btnNext.Image = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.DownArrow_161;
         this.btnNext.Location = new System.Drawing.Point(504, 44);
         this.btnNext.Name = "btnNext";
         this.btnNext.Size = new System.Drawing.Size(28, 28);
         this.btnNext.TabIndex = 55;
         this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
         // 
         // btnPrevious
         // 
         this.btnPrevious.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.btnPrevious.Image = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.UpArrow_161;
         this.btnPrevious.Location = new System.Drawing.Point(504, 12);
         this.btnPrevious.Name = "btnPrevious";
         this.btnPrevious.Size = new System.Drawing.Size(28, 28);
         this.btnPrevious.TabIndex = 54;
         this.btnPrevious.Click += new System.EventHandler(this.btnPrevious_Click);
         // 
         // txtTime
         // 
         this.txtTime.Location = new System.Drawing.Point(48, 12);
         this.txtTime.Name = "txtTime";
         this.txtTime.ReadOnly = true;
         this.txtTime.Size = new System.Drawing.Size(176, 20);
         this.txtTime.TabIndex = 48;
         this.txtTime.Tag = "1";
         // 
         // groupDetails
         // 
         this.groupDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                     | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.groupDetails.Controls.Add(this.textDetails);
         this.groupDetails.Location = new System.Drawing.Point(8, 72);
         this.groupDetails.Name = "groupDetails";
         this.groupDetails.Size = new System.Drawing.Size(472, 306);
         this.groupDetails.TabIndex = 64;
         this.groupDetails.TabStop = false;
         this.groupDetails.Text = "Details";
         // 
         // textDetails
         // 
         this.textDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                     | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.textDetails.Location = new System.Drawing.Point(8, 16);
         this.textDetails.Multiline = true;
         this.textDetails.Name = "textDetails";
         this.textDetails.ReadOnly = true;
         this.textDetails.ScrollBars = System.Windows.Forms.ScrollBars.Both;
         this.textDetails.Size = new System.Drawing.Size(456, 278);
         this.textDetails.TabIndex = 0;
         // 
         // Form_ActivityLogProperties
         // 
         this.AcceptButton = this.btnClose;
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.CancelButton = this.btnClose;
         this.ClientSize = new System.Drawing.Size(552, 388);
         this.Controls.Add(this.groupDetails);
         this.Controls.Add(this.btnClose);
         this.Controls.Add(this.btnCopy);
         this.Controls.Add(this.label4);
         this.Controls.Add(this.label7);
         this.Controls.Add(this.lblTime);
         this.Controls.Add(this.txtType);
         this.Controls.Add(this.txtSQLServer);
         this.Controls.Add(this.txtTime);
         this.Controls.Add(this.btnNext);
         this.Controls.Add(this.btnPrevious);
         this.HelpButton = true;
         this.MinimumSize = new System.Drawing.Size(400, 400);
         this.Name = "Form_ActivityLogProperties";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Activity Log Properties";
         this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.Form_LogProperties_HelpRequested);
         this.groupDetails.ResumeLayout(false);
         this.groupDetails.PerformLayout();
         this.ResumeLayout(false);
         this.PerformLayout();

      }
      #endregion

      private System.Windows.Forms.Button btnClose;
      private System.Windows.Forms.Button btnCopy;
      private System.Windows.Forms.Label label4;
      private System.Windows.Forms.Label label7;
      private System.Windows.Forms.Label lblTime;
      private System.Windows.Forms.TextBox txtType;
      private System.Windows.Forms.TextBox txtSQLServer;
      private System.Windows.Forms.Button btnNext;
      private System.Windows.Forms.Button btnPrevious;
      private System.Windows.Forms.TextBox txtTime;
      private System.Windows.Forms.GroupBox groupDetails;
      private System.Windows.Forms.TextBox textDetails;
   }
}