namespace Idera.SQLcompliance.Application.GUI.Forms
{
   partial class Form_SnapshotOptions
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
         this.btnOK = new System.Windows.Forms.Button();
         this.btnCancel = new System.Windows.Forms.Button();
         this.groupBox4 = new System.Windows.Forms.GroupBox();
         this.textInterval = new Idera.SQLcompliance.Application.GUI.Controls.NumericTextBox();
         this.radioDoNotCapture = new System.Windows.Forms.RadioButton();
         this.radioSnapshotInterval = new System.Windows.Forms.RadioButton();
         this.label12 = new System.Windows.Forms.Label();
         this.label1 = new System.Windows.Forms.Label();
         this.groupBox4.SuspendLayout();
         this.SuspendLayout();
         // 
         // btnOK
         // 
         this.btnOK.Location = new System.Drawing.Point(228, 140);
         this.btnOK.Name = "btnOK";
         this.btnOK.Size = new System.Drawing.Size(75, 23);
         this.btnOK.TabIndex = 4;
         this.btnOK.Text = "&OK";
         this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
         // 
         // btnCancel
         // 
         this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.btnCancel.Location = new System.Drawing.Point(312, 140);
         this.btnCancel.Name = "btnCancel";
         this.btnCancel.Size = new System.Drawing.Size(75, 23);
         this.btnCancel.TabIndex = 5;
         this.btnCancel.Text = "&Cancel";
         this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
         // 
         // groupBox4
         // 
         this.groupBox4.Controls.Add(this.textInterval);
         this.groupBox4.Controls.Add(this.radioDoNotCapture);
         this.groupBox4.Controls.Add(this.radioSnapshotInterval);
         this.groupBox4.Controls.Add(this.label12);
         this.groupBox4.Location = new System.Drawing.Point(12, 56);
         this.groupBox4.Name = "groupBox4";
         this.groupBox4.Size = new System.Drawing.Size(376, 76);
         this.groupBox4.TabIndex = 12;
         this.groupBox4.TabStop = false;
         this.groupBox4.Text = "Audit Snapshot Schedule";
         // 
         // textInterval
         // 
         this.textInterval.Location = new System.Drawing.Point(192, 16);
         this.textInterval.MaxLength = 3;
         this.textInterval.Name = "textInterval";
         this.textInterval.Size = new System.Drawing.Size(28, 20);
         this.textInterval.TabIndex = 3;
         this.textInterval.Text = "999";
         // 
         // radioDoNotCapture
         // 
         this.radioDoNotCapture.Location = new System.Drawing.Point(12, 44);
         this.radioDoNotCapture.Name = "radioDoNotCapture";
         this.radioDoNotCapture.Size = new System.Drawing.Size(276, 20);
         this.radioDoNotCapture.TabIndex = 2;
         this.radioDoNotCapture.Text = "&Do Not Capture Audit Snapshots";
         this.radioDoNotCapture.CheckedChanged += new System.EventHandler(this.radioSnapshotInterval_CheckedChanged);
         // 
         // radioSnapshotInterval
         // 
         this.radioSnapshotInterval.Checked = true;
         this.radioSnapshotInterval.Location = new System.Drawing.Point(12, 20);
         this.radioSnapshotInterval.Name = "radioSnapshotInterval";
         this.radioSnapshotInterval.Size = new System.Drawing.Size(184, 16);
         this.radioSnapshotInterval.TabIndex = 0;
         this.radioSnapshotInterval.TabStop = true;
         this.radioSnapshotInterval.Text = "Capture &Audit Snapshots every";
         this.radioSnapshotInterval.CheckedChanged += new System.EventHandler(this.radioSnapshotInterval_CheckedChanged);
         // 
         // label12
         // 
         this.label12.Location = new System.Drawing.Point(228, 20);
         this.label12.Name = "label12";
         this.label12.Size = new System.Drawing.Size(40, 16);
         this.label12.TabIndex = 2;
         this.label12.Text = "days.";
         // 
         // label1
         // 
         this.label1.Location = new System.Drawing.Point(12, 8);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(376, 40);
         this.label1.TabIndex = 13;
         this.label1.Text = "SQL Compliance Manager will capture snapshots of the audit configuration on a reg" +
             "ular basis. These snapshots are placed in the Change Log and can be viewed in re" +
             "ports.";
         // 
         // Form_SnapshotOptions
         // 
         this.AcceptButton = this.btnOK;
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.CancelButton = this.btnCancel;
         this.ClientSize = new System.Drawing.Size(398, 172);
         this.Controls.Add(this.label1);
         this.Controls.Add(this.groupBox4);
         this.Controls.Add(this.btnCancel);
         this.Controls.Add(this.btnOK);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.HelpButton = true;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "Form_SnapshotOptions";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Audit Snapshot Preferences";
         this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.Form_Defaults_HelpRequested);
         this.groupBox4.ResumeLayout(false);
         this.groupBox4.PerformLayout();
         this.ResumeLayout(false);

      }
      #endregion

      private System.Windows.Forms.Button btnOK;
      private System.Windows.Forms.Button btnCancel;
      private System.Windows.Forms.GroupBox groupBox4;
      private System.Windows.Forms.Label label12;
      private System.Windows.Forms.RadioButton radioDoNotCapture;
      private System.Windows.Forms.RadioButton radioSnapshotInterval;
      private System.Windows.Forms.Label label1;
      private Idera.SQLcompliance.Application.GUI.Controls.NumericTextBox textInterval;
   }
}