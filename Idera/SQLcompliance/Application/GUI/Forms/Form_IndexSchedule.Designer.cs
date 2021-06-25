namespace Idera.SQLcompliance.Application.GUI.Forms
{
   partial class Form_IndexSchedule
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
         Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
         Infragistics.Win.UltraWinEditors.DropDownEditorButton dropDownEditorButton1 = new Infragistics.Win.UltraWinEditors.DropDownEditorButton("DropDownList");
         Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
         Infragistics.Win.UltraWinEditors.DropDownEditorButton dropDownEditorButton2 = new Infragistics.Win.UltraWinEditors.DropDownEditorButton("DropDownList");
         this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
         this.groupBox1 = new System.Windows.Forms.GroupBox();
         this.label2 = new System.Windows.Forms.Label();
         this.label1 = new System.Windows.Forms.Label();
         this.IDOK = new System.Windows.Forms.Button();
         this.helpLink = new System.Windows.Forms.LinkLabel();
         this.disableScheduleCheckbox = new System.Windows.Forms.CheckBox();
         this.indexStartTimeCombo = new Idera.SQLcompliance.Application.GUI.Controls.TimeComboEditor();
         this.indexDurationCombo = new Idera.SQLcompliance.Application.GUI.Controls.TimeComboEditor();
         this.groupBox1.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.indexStartTimeCombo)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.indexDurationCombo)).BeginInit();
         this.SuspendLayout();
         // 
         // ultraLabel1
         // 
         this.ultraLabel1.Location = new System.Drawing.Point(12, 8);
         this.ultraLabel1.Name = "ultraLabel1";
         this.ultraLabel1.Size = new System.Drawing.Size(211, 49);
         this.ultraLabel1.TabIndex = 0;
         this.ultraLabel1.Text = "Specify when index maintenance should be performed on the Repository databases.";
         // 
         // groupBox1
         // 
         this.groupBox1.Controls.Add(this.label2);
         this.groupBox1.Controls.Add(this.label1);
         this.groupBox1.Controls.Add(this.indexStartTimeCombo);
         this.groupBox1.Controls.Add(this.indexDurationCombo);
         this.groupBox1.Location = new System.Drawing.Point(15, 63);
         this.groupBox1.Name = "groupBox1";
         this.groupBox1.Size = new System.Drawing.Size(192, 75);
         this.groupBox1.TabIndex = 4;
         this.groupBox1.TabStop = false;
         this.groupBox1.Text = "Specifiy Schedule";
         // 
         // label2
         // 
         this.label2.AutoSize = true;
         this.label2.Location = new System.Drawing.Point(8, 47);
         this.label2.Name = "label2";
         this.label2.Size = new System.Drawing.Size(96, 13);
         this.label2.TabIndex = 5;
         this.label2.Text = "Duration (HH:MM):";
         // 
         // label1
         // 
         this.label1.AutoSize = true;
         this.label1.Location = new System.Drawing.Point(46, 21);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(58, 13);
         this.label1.TabIndex = 4;
         this.label1.Text = "Start Time:";
         // 
         // IDOK
         // 
         this.IDOK.Location = new System.Drawing.Point(133, 148);
         this.IDOK.Name = "IDOK";
         this.IDOK.Size = new System.Drawing.Size(75, 23);
         this.IDOK.TabIndex = 5;
         this.IDOK.Text = "OK";
         this.IDOK.UseVisualStyleBackColor = true;
         this.IDOK.Click += new System.EventHandler(this.IDOK_Click);
         // 
         // helpLink
         // 
         this.helpLink.AutoSize = true;
         this.helpLink.Location = new System.Drawing.Point(68, 39);
         this.helpLink.Name = "helpLink";
         this.helpLink.Size = new System.Drawing.Size(76, 13);
         this.helpLink.TabIndex = 6;
         this.helpLink.TabStop = true;
         this.helpLink.Text = "Tell me more...";
         this.helpLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkClick_HelpLink);
         // 
         // disableScheduleCheckbox
         // 
         this.disableScheduleCheckbox.AutoSize = true;
         this.disableScheduleCheckbox.Location = new System.Drawing.Point(18, 148);
         this.disableScheduleCheckbox.Name = "disableScheduleCheckbox";
         this.disableScheduleCheckbox.Size = new System.Drawing.Size(109, 17);
         this.disableScheduleCheckbox.TabIndex = 7;
         this.disableScheduleCheckbox.Text = "Disable Schedule";
         this.disableScheduleCheckbox.UseVisualStyleBackColor = true;
         this.disableScheduleCheckbox.CheckedChanged += new System.EventHandler(this.disableScheduleCheckbox_CheckedChanged);
         // 
         // indexStartTimeCombo
         // 
         appearance1.BackColor = System.Drawing.SystemColors.Window;
         appearance1.BackColor2 = System.Drawing.SystemColors.Window;
         this.indexStartTimeCombo.Appearance = appearance1;
         dropDownEditorButton1.Key = "DropDownList";
         this.indexStartTimeCombo.ButtonsRight.Add(dropDownEditorButton1);
         this.indexStartTimeCombo.DateTime = new System.DateTime(2010, 3, 11, 1, 0, 0, 0);
         this.indexStartTimeCombo.DisplayStyle = Infragistics.Win.EmbeddableElementDisplayStyle.Standard;
         this.indexStartTimeCombo.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
         this.indexStartTimeCombo.FormatString = "hh:mm tt";
         this.indexStartTimeCombo.ListInterval = System.TimeSpan.Parse("00:30:00");
         this.indexStartTimeCombo.Location = new System.Drawing.Point(108, 17);
         this.indexStartTimeCombo.MaskInput = "hh:mm tt";
         this.indexStartTimeCombo.Name = "indexStartTimeCombo";
         this.indexStartTimeCombo.Size = new System.Drawing.Size(77, 21);
         this.indexStartTimeCombo.TabIndex = 1;
         this.indexStartTimeCombo.Time = System.TimeSpan.Parse("01:00:00");
         this.indexStartTimeCombo.Value = new System.DateTime(2010, 3, 11, 1, 0, 0, 0);
         // 
         // indexDurationCombo
         // 
         appearance2.BackColor = System.Drawing.SystemColors.Window;
         appearance2.BackColor2 = System.Drawing.SystemColors.Window;
         this.indexDurationCombo.Appearance = appearance2;
         dropDownEditorButton2.Key = "DropDownList";
         this.indexDurationCombo.ButtonsRight.Add(dropDownEditorButton2);
         this.indexDurationCombo.DateTime = new System.DateTime(2010, 3, 23, 0, 30, 0, 0);
         this.indexDurationCombo.DisplayStyle = Infragistics.Win.EmbeddableElementDisplayStyle.Standard;
         this.indexDurationCombo.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
         this.indexDurationCombo.FormatString = "HH:mm";
         this.indexDurationCombo.ListInterval = System.TimeSpan.Parse("00:30:00");
         this.indexDurationCombo.Location = new System.Drawing.Point(108, 43);
         this.indexDurationCombo.MaskInput = "hh:mm";
         this.indexDurationCombo.Name = "indexDurationCombo";
         this.indexDurationCombo.Size = new System.Drawing.Size(77, 21);
         this.indexDurationCombo.TabIndex = 2;
         this.indexDurationCombo.Time = System.TimeSpan.Parse("00:30:00");
         this.indexDurationCombo.Value = new System.DateTime(2010, 3, 23, 0, 30, 0, 0);
         this.indexDurationCombo.ValueChanged += new System.EventHandler(this.IndexDurationCombo_ValueChanged);
         // 
         // Form_IndexSchedule
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(221, 188);
         this.ControlBox = false;
         this.Controls.Add(this.helpLink);
         this.Controls.Add(this.disableScheduleCheckbox);
         this.Controls.Add(this.IDOK);
         this.Controls.Add(this.groupBox1);
         this.Controls.Add(this.ultraLabel1);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.Icon = Properties.Resources.SQLcompliance_product_ico;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "Form_IndexSchedule";
         this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Set Maintenance Schedule";
         this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.Form_IndexSchedule_HelpRequested);
         this.groupBox1.ResumeLayout(false);
         this.groupBox1.PerformLayout();
         ((System.ComponentModel.ISupportInitialize)(this.indexStartTimeCombo)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.indexDurationCombo)).EndInit();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private Infragistics.Win.Misc.UltraLabel ultraLabel1;
      private Idera.SQLcompliance.Application.GUI.Controls.TimeComboEditor indexStartTimeCombo;
      private Idera.SQLcompliance.Application.GUI.Controls.TimeComboEditor indexDurationCombo;
      private System.Windows.Forms.GroupBox groupBox1;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.Button IDOK;
      private System.Windows.Forms.LinkLabel helpLink;
      private System.Windows.Forms.CheckBox disableScheduleCheckbox;
   }
}