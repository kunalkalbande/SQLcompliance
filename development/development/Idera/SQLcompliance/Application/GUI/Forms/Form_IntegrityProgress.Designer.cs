namespace Idera.SQLcompliance.Application.GUI.Forms
{
   partial class Form_IntegrityProgress
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
         this.progressBar1 = new System.Windows.Forms.ProgressBar();
         this.tbMessage = new System.Windows.Forms.TextBox();
         this.SuspendLayout();
         // 
         // progressBar1
         // 
         this.progressBar1.Location = new System.Drawing.Point(8, 48);
         this.progressBar1.Name = "progressBar1";
         this.progressBar1.Size = new System.Drawing.Size(392, 23);
         this.progressBar1.TabIndex = 0;
         // 
         // tbMessage
         // 
         this.tbMessage.BackColor = System.Drawing.SystemColors.Control;
         this.tbMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
         this.tbMessage.Location = new System.Drawing.Point(16, 16);
         this.tbMessage.Multiline = true;
         this.tbMessage.Name = "tbMessage";
         this.tbMessage.Size = new System.Drawing.Size(384, 24);
         this.tbMessage.TabIndex = 1;
         this.tbMessage.TabStop = false;
         this.tbMessage.Text = "Checking integrity of database ";
         // 
         // Form_IntegrityProgress
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(410, 88);
         this.ControlBox = false;
         this.Controls.Add(this.tbMessage);
         this.Controls.Add(this.progressBar1);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "Form_IntegrityProgress";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Integrity Check In Progress...";
         this.ResumeLayout(false);

      }
      #endregion

      private System.Windows.Forms.ProgressBar progressBar1;
      private System.Windows.Forms.TextBox tbMessage;
	}
}