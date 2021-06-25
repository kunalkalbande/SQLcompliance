namespace Idera.SQLcompliance.Application.GUI.Forms
{
   partial class Form_TimeSpan
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
         this._btnOk = new System.Windows.Forms.Button();
         this._dtEndTime = new System.Windows.Forms.DateTimePicker();
         this._dtStartTime = new System.Windows.Forms.DateTimePicker();
         this._dtEndDate = new System.Windows.Forms.DateTimePicker();
         this._lblTo = new System.Windows.Forms.Label();
         this._dtStartDate = new System.Windows.Forms.DateTimePicker();
         this.label1 = new System.Windows.Forms.Label();
         this.SuspendLayout();
         // 
         // _btnOk
         // 
         this._btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
         this._btnOk.Location = new System.Drawing.Point(149, 70);
         this._btnOk.Name = "_btnOk";
         this._btnOk.Size = new System.Drawing.Size(75, 23);
         this._btnOk.TabIndex = 1;
         this._btnOk.Text = "OK";
         // 
         // _dtEndTime
         // 
         this._dtEndTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
         this._dtEndTime.Location = new System.Drawing.Point(136, 36);
         this._dtEndTime.Name = "_dtEndTime";
         this._dtEndTime.ShowUpDown = true;
         this._dtEndTime.Size = new System.Drawing.Size(88, 20);
         this._dtEndTime.TabIndex = 14;
         // 
         // _dtStartTime
         // 
         this._dtStartTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
         this._dtStartTime.Location = new System.Drawing.Point(136, 12);
         this._dtStartTime.Name = "_dtStartTime";
         this._dtStartTime.ShowUpDown = true;
         this._dtStartTime.Size = new System.Drawing.Size(88, 20);
         this._dtStartTime.TabIndex = 11;
         // 
         // _dtEndDate
         // 
         this._dtEndDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
         this._dtEndDate.Location = new System.Drawing.Point(48, 36);
         this._dtEndDate.MaxDate = new System.DateTime(2100, 12, 31, 0, 0, 0, 0);
         this._dtEndDate.MinDate = new System.DateTime(2004, 1, 1, 0, 0, 0, 0);
         this._dtEndDate.Name = "_dtEndDate";
         this._dtEndDate.Size = new System.Drawing.Size(84, 20);
         this._dtEndDate.TabIndex = 13;
         this._dtEndDate.Value = new System.DateTime(2005, 1, 12, 0, 0, 0, 0);
         // 
         // _lblTo
         // 
         this._lblTo.Location = new System.Drawing.Point(28, 40);
         this._lblTo.Name = "_lblTo";
         this._lblTo.Size = new System.Drawing.Size(16, 16);
         this._lblTo.TabIndex = 12;
         this._lblTo.Text = "to";
         // 
         // _dtStartDate
         // 
         this._dtStartDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
         this._dtStartDate.Location = new System.Drawing.Point(48, 12);
         this._dtStartDate.MaxDate = new System.DateTime(2100, 12, 31, 0, 0, 0, 0);
         this._dtStartDate.MinDate = new System.DateTime(2004, 1, 1, 0, 0, 0, 0);
         this._dtStartDate.Name = "_dtStartDate";
         this._dtStartDate.Size = new System.Drawing.Size(84, 20);
         this._dtStartDate.TabIndex = 10;
         this._dtStartDate.Value = new System.DateTime(2005, 1, 12, 0, 0, 0, 0);
         // 
         // label1
         // 
         this.label1.AutoSize = true;
         this.label1.Location = new System.Drawing.Point(12, 16);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(30, 13);
         this.label1.TabIndex = 15;
         this.label1.Text = "From";
         // 
         // Form_TimeSpan
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.CancelButton = this._btnOk;
         this.ClientSize = new System.Drawing.Size(236, 105);
         this.Controls.Add(this.label1);
         this.Controls.Add(this._dtEndTime);
         this.Controls.Add(this._dtStartTime);
         this.Controls.Add(this._dtEndDate);
         this.Controls.Add(this._lblTo);
         this.Controls.Add(this._dtStartDate);
         this.Controls.Add(this._btnOk);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "Form_TimeSpan";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Custom Time Range";
         this.ResumeLayout(false);
         this.PerformLayout();

      }
      #endregion

      private System.Windows.Forms.Button _btnOk;
      private System.Windows.Forms.DateTimePicker _dtEndTime;
      private System.Windows.Forms.DateTimePicker _dtStartTime;
      private System.Windows.Forms.DateTimePicker _dtEndDate;
      private System.Windows.Forms.Label _lblTo;
      private System.Windows.Forms.DateTimePicker _dtStartDate;
      private System.Windows.Forms.Label label1;
	}
}