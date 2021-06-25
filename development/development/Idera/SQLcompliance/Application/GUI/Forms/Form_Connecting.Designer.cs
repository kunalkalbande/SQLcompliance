namespace Idera.SQLcompliance.Application.GUI.Forms
{
   partial class Form_Connecting
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
         this.label1 = new System.Windows.Forms.Label();
         this.labelServer = new System.Windows.Forms.Label();
         this.SuspendLayout();
         // 
         // label1
         // 
         this.label1.Location = new System.Drawing.Point(8, 12);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(424, 16);
         this.label1.TabIndex = 0;
         this.label1.Text = "Connecting to SQL Compliance Manager Repository:";
         this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
         // 
         // labelServer
         // 
         this.labelServer.Location = new System.Drawing.Point(9, 35);
         this.labelServer.Name = "labelServer";
         this.labelServer.Size = new System.Drawing.Size(424, 16);
         this.labelServer.TabIndex = 1;
         this.labelServer.Text = "server instance";
         this.labelServer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
         // 
         // Form_Connecting
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(442, 64);
         this.ControlBox = false;
         this.Controls.Add(this.labelServer);
         this.Controls.Add(this.label1);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
         this.MaximizeBox = false;
         this.Name = "Form_Connecting";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Connecting...";
         this.ResumeLayout(false);

      }
      #endregion

      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.Label labelServer;

	}
}