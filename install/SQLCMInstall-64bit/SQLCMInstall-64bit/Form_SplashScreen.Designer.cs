using SQLCMInstall_64bit.Properties;
namespace SQLCMInstall_64bit
{
   partial class SplashScreen
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SplashScreen));
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblCopyright = new System.Windows.Forms.Label();
            this.lblIderaText = new System.Windows.Forms.Label();
            this.lblIncText = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblStatus
            // 
            this.lblStatus.BackColor = System.Drawing.Color.Transparent;
            this.lblStatus.ForeColor = System.Drawing.Color.Transparent;
            this.lblStatus.Location = new System.Drawing.Point(13, 350);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(228, 20);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "Initializing...";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblStatus.Visible = false;
            // 
            // lblCopyright
            // 
            this.lblCopyright.BackColor = System.Drawing.Color.Transparent;
            this.lblCopyright.ForeColor = System.Drawing.Color.Transparent;
            this.lblCopyright.Location = new System.Drawing.Point(14, 371);
            this.lblCopyright.Name = "lblCopyright";
            this.lblCopyright.Size = new System.Drawing.Size(121, 17);
            this.lblCopyright.TabIndex = 1;
            this.lblCopyright.Text = "Copyright © 2004-2021";
            // 
            // lblIderaText
            // 
            this.lblIderaText.BackColor = System.Drawing.Color.Transparent;
            this.lblIderaText.Font = new System.Drawing.Font("Segoe UI Black", 9F);
            this.lblIderaText.ForeColor = System.Drawing.Color.Transparent;
            this.lblIderaText.Location = new System.Drawing.Point(132, 369);
            this.lblIderaText.Name = "lblIderaText";
            this.lblIderaText.Size = new System.Drawing.Size(47, 27);
            this.lblIderaText.TabIndex = 2;
            this.lblIderaText.Text = "IDERA,";
            // 
            // lblIncText
            // 
            this.lblIncText.BackColor = System.Drawing.Color.Transparent;
            this.lblIncText.ForeColor = System.Drawing.Color.Transparent;
            this.lblIncText.Location = new System.Drawing.Point(176, 371);
            this.lblIncText.Name = "lblIncText";
            this.lblIncText.Size = new System.Drawing.Size(100, 23);
            this.lblIncText.TabIndex = 3;
            this.lblIncText.Text = "Inc.";
            // 
            // SplashScreen
            // 
            this.BackColor = System.Drawing.Color.LightGray;
            this.BackgroundImage = global::SQLCMInstall_64bit.Properties.Resources.SplashScreen;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(632, 406);
            this.ControlBox = false;
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblCopyright);
            this.Controls.Add(this.lblIderaText);
            this.Controls.Add(this.lblIncText);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SplashScreen";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SplashScreen";
            this.ResumeLayout(false);

      }
      #endregion

      private System.Windows.Forms.Label lblStatus;
      private System.Windows.Forms.Label lblCopyright;
      private System.Windows.Forms.Label lblIderaText;
      private System.Windows.Forms.Label lblIncText;
    }
}
