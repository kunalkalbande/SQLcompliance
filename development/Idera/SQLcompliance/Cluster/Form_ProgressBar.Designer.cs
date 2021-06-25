namespace Idera.SQLcompliance.Cluster
{
   partial class Form_ProgressBar
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
         this.buttonCancel = new System.Windows.Forms.Button();
         this.progressBar = new System.Windows.Forms.ProgressBar();
         this.progressMessage = new System.Windows.Forms.Label();
         this.SuspendLayout();
         // 
         // buttonCancel
         // 
         this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.buttonCancel.Location = new System.Drawing.Point(312, 80);
         this.buttonCancel.Name = "buttonCancel";
         this.buttonCancel.Size = new System.Drawing.Size(88, 23);
         this.buttonCancel.TabIndex = 0;
         this.buttonCancel.Text = "Cancel";
         this.buttonCancel.UseVisualStyleBackColor = true;
         this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
         // 
         // progressBar
         // 
         this.progressBar.Location = new System.Drawing.Point(8, 48);
         this.progressBar.Name = "progressBar";
         this.progressBar.Size = new System.Drawing.Size(392, 23);
         this.progressBar.TabIndex = 1;
         // 
         // progressMessage
         // 
         this.progressMessage.Location = new System.Drawing.Point(16, 16);
         this.progressMessage.Name = "progressMessage";
         this.progressMessage.Size = new System.Drawing.Size(384, 24);
         this.progressMessage.TabIndex = 2;
         this.progressMessage.Text = "Updating the Trigger Assembly Information...";
         // 
         // Form_ProgressBar
         // 
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
         this.CancelButton = this.buttonCancel;
         this.ClientSize = new System.Drawing.Size(408, 110);
         this.Controls.Add(this.progressMessage);
         this.Controls.Add(this.progressBar);
         this.Controls.Add(this.buttonCancel);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "Form_ProgressBar";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Updating SQLcompliance Agent Settings...";
         this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.Timer progressTimer;
      private System.Windows.Forms.Button buttonCancel;
      private System.Windows.Forms.ProgressBar progressBar;
      private System.Windows.Forms.Label progressMessage;
   }
}