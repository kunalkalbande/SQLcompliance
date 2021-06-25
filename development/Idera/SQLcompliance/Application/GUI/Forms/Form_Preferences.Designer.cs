namespace Idera.SQLcompliance.Application.GUI.Forms
{
   partial class Form_Preferences
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
         this.btnCancel = new System.Windows.Forms.Button();
         this.btnOK = new System.Windows.Forms.Button();
         this.tabControl1 = new System.Windows.Forms.TabControl();
         this.tabViews = new System.Windows.Forms.TabPage();
         this.btnRestoreDefaults = new System.Windows.Forms.Button();
         this.groupBox1 = new System.Windows.Forms.GroupBox();
         this.textEventPageSize = new Idera.SQLcompliance.Application.GUI.Controls.NumericTextBox();
         this.label3 = new System.Windows.Forms.Label();
         this.label4 = new System.Windows.Forms.Label();
         this.grpEventTime = new System.Windows.Forms.GroupBox();
         this.rbShowLocalTime = new System.Windows.Forms.RadioButton();
         this.rbShowServerLocalTime = new System.Windows.Forms.RadioButton();
         this.tabAlertVeiws = new System.Windows.Forms.TabPage();
         this._btnRestoreAlertDefaults = new System.Windows.Forms.Button();
         this.groupBox2 = new System.Windows.Forms.GroupBox();
         this.label2 = new System.Windows.Forms.Label();
         this._tbAlertPageSize = new Idera.SQLcompliance.Application.GUI.Controls.NumericTextBox();
         this.label5 = new System.Windows.Forms.Label();
         this.tabControl1.SuspendLayout();
         this.tabViews.SuspendLayout();
         this.groupBox1.SuspendLayout();
         this.grpEventTime.SuspendLayout();
         this.tabAlertVeiws.SuspendLayout();
         this.groupBox2.SuspendLayout();
         this.SuspendLayout();
         // 
         // btnCancel
         // 
         this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.btnCancel.Location = new System.Drawing.Point(360, 220);
         this.btnCancel.Name = "btnCancel";
         this.btnCancel.Size = new System.Drawing.Size(75, 23);
         this.btnCancel.TabIndex = 24;
         this.btnCancel.Text = "&Cancel";
         this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
         // 
         // btnOK
         // 
         this.btnOK.Location = new System.Drawing.Point(280, 220);
         this.btnOK.Name = "btnOK";
         this.btnOK.Size = new System.Drawing.Size(75, 23);
         this.btnOK.TabIndex = 23;
         this.btnOK.Text = "&OK";
         this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
         // 
         // tabControl1
         // 
         this.tabControl1.Controls.Add(this.tabViews);
         this.tabControl1.Controls.Add(this.tabAlertVeiws);
         this.tabControl1.Location = new System.Drawing.Point(0, 0);
         this.tabControl1.Name = "tabControl1";
         this.tabControl1.SelectedIndex = 0;
         this.tabControl1.Size = new System.Drawing.Size(440, 216);
         this.tabControl1.TabIndex = 4;
         // 
         // tabViews
         // 
         this.tabViews.Controls.Add(this.btnRestoreDefaults);
         this.tabViews.Controls.Add(this.groupBox1);
         this.tabViews.Controls.Add(this.grpEventTime);
         this.tabViews.Location = new System.Drawing.Point(4, 22);
         this.tabViews.Name = "tabViews";
         this.tabViews.Size = new System.Drawing.Size(432, 190);
         this.tabViews.TabIndex = 0;
         this.tabViews.Text = "Event views";
         this.tabViews.UseVisualStyleBackColor = true;
         // 
         // btnRestoreDefaults
         // 
         this.btnRestoreDefaults.Location = new System.Drawing.Point(8, 164);
         this.btnRestoreDefaults.Name = "btnRestoreDefaults";
         this.btnRestoreDefaults.Size = new System.Drawing.Size(100, 23);
         this.btnRestoreDefaults.TabIndex = 22;
         this.btnRestoreDefaults.Text = "&Restore Defaults";
         this.btnRestoreDefaults.Click += new System.EventHandler(this.btnRestoreDefaults_Click);
         // 
         // groupBox1
         // 
         this.groupBox1.Controls.Add(this.textEventPageSize);
         this.groupBox1.Controls.Add(this.label3);
         this.groupBox1.Controls.Add(this.label4);
         this.groupBox1.Location = new System.Drawing.Point(8, 84);
         this.groupBox1.Name = "groupBox1";
         this.groupBox1.Size = new System.Drawing.Size(416, 76);
         this.groupBox1.TabIndex = 17;
         this.groupBox1.TabStop = false;
         this.groupBox1.Text = "Event view limits";
         // 
         // textEventPageSize
         // 
         this.textEventPageSize.Location = new System.Drawing.Point(132, 22);
         this.textEventPageSize.MaxLength = 5;
         this.textEventPageSize.Name = "textEventPageSize";
         this.textEventPageSize.Size = new System.Drawing.Size(44, 20);
         this.textEventPageSize.TabIndex = 21;
         this.textEventPageSize.Text = "5000";
         // 
         // label3
         // 
         this.label3.Location = new System.Drawing.Point(8, 52);
         this.label3.Name = "label3";
         this.label3.Size = new System.Drawing.Size(392, 16);
         this.label3.TabIndex = 17;
         this.label3.Text = "Warning: Large page sizes will cause loading of event views to take  longer.";
         // 
         // label4
         // 
         this.label4.Location = new System.Drawing.Point(8, 24);
         this.label4.Name = "label4";
         this.label4.Size = new System.Drawing.Size(116, 16);
         this.label4.TabIndex = 18;
         this.label4.Text = "&Event view page size:";
         // 
         // grpEventTime
         // 
         this.grpEventTime.Controls.Add(this.rbShowLocalTime);
         this.grpEventTime.Controls.Add(this.rbShowServerLocalTime);
         this.grpEventTime.Location = new System.Drawing.Point(8, 8);
         this.grpEventTime.Name = "grpEventTime";
         this.grpEventTime.Size = new System.Drawing.Size(416, 68);
         this.grpEventTime.TabIndex = 14;
         this.grpEventTime.TabStop = false;
         this.grpEventTime.Text = "Event time display";
         // 
         // rbShowLocalTime
         // 
         this.rbShowLocalTime.Location = new System.Drawing.Point(8, 44);
         this.rbShowLocalTime.Name = "rbShowLocalTime";
         this.rbShowLocalTime.Size = new System.Drawing.Size(240, 16);
         this.rbShowLocalTime.TabIndex = 16;
         this.rbShowLocalTime.Text = "Show event &times local to current system";
         // 
         // rbShowServerLocalTime
         // 
         this.rbShowServerLocalTime.Checked = true;
         this.rbShowServerLocalTime.Location = new System.Drawing.Point(8, 20);
         this.rbShowServerLocalTime.Name = "rbShowServerLocalTime";
         this.rbShowServerLocalTime.Size = new System.Drawing.Size(264, 16);
         this.rbShowServerLocalTime.TabIndex = 15;
         this.rbShowServerLocalTime.TabStop = true;
         this.rbShowServerLocalTime.Text = "&Show event times local to audited SQL Server";
         // 
         // tabAlertVeiws
         // 
         this.tabAlertVeiws.Controls.Add(this._btnRestoreAlertDefaults);
         this.tabAlertVeiws.Controls.Add(this.groupBox2);
         this.tabAlertVeiws.Location = new System.Drawing.Point(4, 22);
         this.tabAlertVeiws.Name = "tabAlertVeiws";
         this.tabAlertVeiws.Size = new System.Drawing.Size(432, 190);
         this.tabAlertVeiws.TabIndex = 1;
         this.tabAlertVeiws.Text = "Alert views";
         this.tabAlertVeiws.UseVisualStyleBackColor = true;
         // 
         // _btnRestoreAlertDefaults
         // 
         this._btnRestoreAlertDefaults.Location = new System.Drawing.Point(8, 164);
         this._btnRestoreAlertDefaults.Name = "_btnRestoreAlertDefaults";
         this._btnRestoreAlertDefaults.Size = new System.Drawing.Size(100, 23);
         this._btnRestoreAlertDefaults.TabIndex = 23;
         this._btnRestoreAlertDefaults.Text = "&Restore Defaults";
         this._btnRestoreAlertDefaults.Click += new System.EventHandler(this._btnRestoreAlertDefaults_Click);
         // 
         // groupBox2
         // 
         this.groupBox2.Controls.Add(this.label2);
         this.groupBox2.Controls.Add(this._tbAlertPageSize);
         this.groupBox2.Controls.Add(this.label5);
         this.groupBox2.Location = new System.Drawing.Point(8, 8);
         this.groupBox2.Name = "groupBox2";
         this.groupBox2.Size = new System.Drawing.Size(416, 80);
         this.groupBox2.TabIndex = 0;
         this.groupBox2.TabStop = false;
         this.groupBox2.Text = "Alert view limits";
         // 
         // label2
         // 
         this.label2.Location = new System.Drawing.Point(8, 56);
         this.label2.Name = "label2";
         this.label2.Size = new System.Drawing.Size(380, 16);
         this.label2.TabIndex = 22;
         this.label2.Text = "Warning: Large page sizes will cause loading of alerts view to take  longer.";
         // 
         // _tbAlertPageSize
         // 
         this._tbAlertPageSize.Location = new System.Drawing.Point(124, 20);
         this._tbAlertPageSize.MaxLength = 5;
         this._tbAlertPageSize.Name = "_tbAlertPageSize";
         this._tbAlertPageSize.Size = new System.Drawing.Size(40, 20);
         this._tbAlertPageSize.TabIndex = 24;
         this._tbAlertPageSize.Text = "1000";
         // 
         // label5
         // 
         this.label5.Location = new System.Drawing.Point(8, 24);
         this.label5.Name = "label5";
         this.label5.Size = new System.Drawing.Size(116, 16);
         this.label5.TabIndex = 23;
         this.label5.Text = "Alert view page size:";
         // 
         // Form_Preferences
         // 
         this.AcceptButton = this.btnOK;
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.CancelButton = this.btnCancel;
         this.ClientSize = new System.Drawing.Size(438, 248);
         this.Controls.Add(this.tabControl1);
         this.Controls.Add(this.btnOK);
         this.Controls.Add(this.btnCancel);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.HelpButton = true;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "Form_Preferences";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Console Preferences";
         this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.Form_Preferences_HelpRequested);
         this.tabControl1.ResumeLayout(false);
         this.tabViews.ResumeLayout(false);
         this.groupBox1.ResumeLayout(false);
         this.groupBox1.PerformLayout();
         this.grpEventTime.ResumeLayout(false);
         this.tabAlertVeiws.ResumeLayout(false);
         this.groupBox2.ResumeLayout(false);
         this.groupBox2.PerformLayout();
         this.ResumeLayout(false);

      }
      #endregion

      private System.Windows.Forms.Button btnCancel;
      private System.Windows.Forms.Button btnOK;
      private System.Windows.Forms.TabControl tabControl1;
      private System.Windows.Forms.TabPage tabViews;
      private System.Windows.Forms.GroupBox grpEventTime;
      private System.Windows.Forms.RadioButton rbShowServerLocalTime;
      private System.Windows.Forms.GroupBox groupBox1;
      private System.Windows.Forms.Label label4;
      private System.Windows.Forms.Label label3;
      private Idera.SQLcompliance.Application.GUI.Controls.NumericTextBox textEventPageSize;
   }
}