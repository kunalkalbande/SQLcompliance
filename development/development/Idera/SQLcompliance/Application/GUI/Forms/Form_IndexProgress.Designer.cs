namespace Idera.SQLcompliance.Application.GUI.Forms
{
   partial class Form_IndexProgress
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
         this._progressBar = new System.Windows.Forms.ProgressBar();
         this._tbMessage = new System.Windows.Forms.TextBox();
         this.SuspendLayout();
         // 
         // _progressBar
         // 
         this._progressBar.Location = new System.Drawing.Point(8, 64);
         this._progressBar.Name = "_progressBar";
         this._progressBar.Size = new System.Drawing.Size(392, 23);
         this._progressBar.TabIndex = 0;
         // 
         // _tbMessage
         // 
         this._tbMessage.BackColor = System.Drawing.SystemColors.Control;
         this._tbMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
         this._tbMessage.Location = new System.Drawing.Point(16, 16);
         this._tbMessage.Multiline = true;
         this._tbMessage.Name = "_tbMessage";
         this._tbMessage.Size = new System.Drawing.Size(384, 40);
         this._tbMessage.TabIndex = 1;
         this._tbMessage.TabStop = false;
         this._tbMessage.Text = "Grooming alerts ";
         // 
         // Form_IndexProgress
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(410, 104);
         this.ControlBox = false;
         this.Controls.Add(this._tbMessage);
         this.Controls.Add(this._progressBar);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "Form_IndexProgress";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Indexing in progress...";
         this.ResumeLayout(false);

      }
      #endregion

      private System.Windows.Forms.ProgressBar _progressBar;
      private System.Windows.Forms.TextBox _tbMessage;

	}
}