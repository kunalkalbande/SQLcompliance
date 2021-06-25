namespace Idera.SQLcompliance.Application.GUI.Forms
{
   partial class Form_EventFilterWizard
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
            if (_rtfGraphics != null)
            {
               _rtfGraphics.Dispose();
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
          this._pnlLeft = new System.Windows.Forms.Panel();
          this._pictureBox = new System.Windows.Forms.PictureBox();
          this._pnlCenter = new System.Windows.Forms.Panel();
          this._pnlAdditionalFilters = new System.Windows.Forms.Panel();
          this._rtfAdditionalFilters = new System.Windows.Forms.RichTextBox();
          this._listBoxAdditionalFilters = new System.Windows.Forms.CheckedListBox();
          this._lblRtfAdditionalFilters = new System.Windows.Forms.Label();
          this._lblAdditionalFilters = new System.Windows.Forms.Label();
          this._pnlSummary = new System.Windows.Forms.Panel();
          this._tbFilterDescription = new System.Windows.Forms.TextBox();
          this._lblFilterDescription = new System.Windows.Forms.Label();
          this._rtfSummary = new System.Windows.Forms.RichTextBox();
          this._checkBoxEnableFilter = new System.Windows.Forms.CheckBox();
          this._tbFilterName = new System.Windows.Forms.TextBox();
          this._lblRtfSummary = new System.Windows.Forms.Label();
          this._lblFilterName = new System.Windows.Forms.Label();
          this._pnlFilterType = new System.Windows.Forms.Panel();
          this._rtfFilterType = new System.Windows.Forms.RichTextBox();
          this._lblRtfFilterType = new System.Windows.Forms.Label();
          this._groupBoxFilterType = new System.Windows.Forms.GroupBox();
          this._linkEventType = new System.Windows.Forms.LinkLabel();
          this._linkCategory = new System.Windows.Forms.LinkLabel();
          this._rbCategory = new System.Windows.Forms.RadioButton();
          this._rbAllEvents = new System.Windows.Forms.RadioButton();
          this._rbEventType = new System.Windows.Forms.RadioButton();
          this._pnlTargetObjects = new System.Windows.Forms.Panel();
          this._rtfTargetObjects = new System.Windows.Forms.RichTextBox();
          this._listBoxTargetObjects = new System.Windows.Forms.CheckedListBox();
          this._lblRtfTargetObjects = new System.Windows.Forms.Label();
          this._lblTargetObjects = new System.Windows.Forms.Label();
          this._pnlBottom = new System.Windows.Forms.Panel();
          this._btnCancel = new System.Windows.Forms.Button();
          this._btnNext = new System.Windows.Forms.Button();
          this._btnBack = new System.Windows.Forms.Button();
          this._btnFinish = new System.Windows.Forms.Button();
          this._pnlTop = new System.Windows.Forms.Panel();
          this._lblDescription = new System.Windows.Forms.Label();
          this._lblTitle = new System.Windows.Forms.Label();
          this._lblBorder1 = new System.Windows.Forms.Label();
          this._lblBorder2 = new System.Windows.Forms.Label();
          this._pnlLeft.SuspendLayout();
          ((System.ComponentModel.ISupportInitialize)(this._pictureBox)).BeginInit();
          this._pnlCenter.SuspendLayout();
          this._pnlAdditionalFilters.SuspendLayout();
          this._pnlSummary.SuspendLayout();
          this._pnlFilterType.SuspendLayout();
          this._groupBoxFilterType.SuspendLayout();
          this._pnlTargetObjects.SuspendLayout();
          this._pnlBottom.SuspendLayout();
          this._pnlTop.SuspendLayout();
          this.SuspendLayout();
          // 
          // _pnlLeft
          // 
          this._pnlLeft.Controls.Add(this._pictureBox);
          this._pnlLeft.Location = new System.Drawing.Point(0, 0);
          this._pnlLeft.Name = "_pnlLeft";
          this._pnlLeft.Size = new System.Drawing.Size(110, 335);
          this._pnlLeft.TabIndex = 3;
          // 
          // _pictureBox
          // 
          this._pictureBox.Image = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.Wizard_EventFilter;
          this._pictureBox.Location = new System.Drawing.Point(0, 0);
          this._pictureBox.Name = "_pictureBox";
          this._pictureBox.Size = new System.Drawing.Size(110, 335);
          this._pictureBox.TabIndex = 0;
          this._pictureBox.TabStop = false;
          // 
          // _pnlCenter
          // 
          this._pnlCenter.Controls.Add(this._pnlAdditionalFilters);
          this._pnlCenter.Controls.Add(this._pnlSummary);
          this._pnlCenter.Controls.Add(this._pnlFilterType);
          this._pnlCenter.Controls.Add(this._pnlTargetObjects);
          this._pnlCenter.Location = new System.Drawing.Point(110, 61);
          this._pnlCenter.Name = "_pnlCenter";
          this._pnlCenter.Size = new System.Drawing.Size(446, 274);
          this._pnlCenter.TabIndex = 1;
          // 
          // _pnlAdditionalFilters
          // 
          this._pnlAdditionalFilters.Controls.Add(this._rtfAdditionalFilters);
          this._pnlAdditionalFilters.Controls.Add(this._listBoxAdditionalFilters);
          this._pnlAdditionalFilters.Controls.Add(this._lblRtfAdditionalFilters);
          this._pnlAdditionalFilters.Controls.Add(this._lblAdditionalFilters);
          this._pnlAdditionalFilters.Dock = System.Windows.Forms.DockStyle.Fill;
          this._pnlAdditionalFilters.Location = new System.Drawing.Point(0, 0);
          this._pnlAdditionalFilters.Name = "_pnlAdditionalFilters";
          this._pnlAdditionalFilters.Size = new System.Drawing.Size(446, 274);
          this._pnlAdditionalFilters.TabIndex = 1;
          this._pnlAdditionalFilters.Visible = false;
          // 
          // _rtfAdditionalFilters
          // 
          this._rtfAdditionalFilters.Location = new System.Drawing.Point(16, 169);
          this._rtfAdditionalFilters.Name = "_rtfAdditionalFilters";
          this._rtfAdditionalFilters.ReadOnly = true;
          this._rtfAdditionalFilters.Size = new System.Drawing.Size(408, 98);
          this._rtfAdditionalFilters.TabIndex = 14;
          this._rtfAdditionalFilters.Text = "";
          this._rtfAdditionalFilters.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MouseMove_rtfBox);
          this._rtfAdditionalFilters.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MouseDown_rtfBox);
          // 
          // _listBoxAdditionalFilters
          // 
          this._listBoxAdditionalFilters.CheckOnClick = true;
          this._listBoxAdditionalFilters.Location = new System.Drawing.Point(16, 33);
          this._listBoxAdditionalFilters.Name = "_listBoxAdditionalFilters";
          this._listBoxAdditionalFilters.Size = new System.Drawing.Size(408, 94);
          this._listBoxAdditionalFilters.TabIndex = 13;
          this._listBoxAdditionalFilters.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.ItemCheck_listBoxAdditionalFilters);
          // 
          // _lblRtfAdditionalFilters
          // 
          this._lblRtfAdditionalFilters.Location = new System.Drawing.Point(16, 144);
          this._lblRtfAdditionalFilters.Name = "_lblRtfAdditionalFilters";
          this._lblRtfAdditionalFilters.Size = new System.Drawing.Size(360, 21);
          this._lblRtfAdditionalFilters.TabIndex = 12;
          this._lblRtfAdditionalFilters.Text = "Edit filter details (click on an underlined value)";
          this._lblRtfAdditionalFilters.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
          // 
          // _lblAdditionalFilters
          // 
          this._lblAdditionalFilters.Location = new System.Drawing.Point(16, 8);
          this._lblAdditionalFilters.Name = "_lblAdditionalFilters";
          this._lblAdditionalFilters.Size = new System.Drawing.Size(224, 21);
          this._lblAdditionalFilters.TabIndex = 10;
          this._lblAdditionalFilters.Text = "Filter events generated by:";
          this._lblAdditionalFilters.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
          // 
          // _pnlSummary
          // 
          this._pnlSummary.Controls.Add(this._tbFilterDescription);
          this._pnlSummary.Controls.Add(this._lblFilterDescription);
          this._pnlSummary.Controls.Add(this._rtfSummary);
          this._pnlSummary.Controls.Add(this._checkBoxEnableFilter);
          this._pnlSummary.Controls.Add(this._tbFilterName);
          this._pnlSummary.Controls.Add(this._lblRtfSummary);
          this._pnlSummary.Controls.Add(this._lblFilterName);
          this._pnlSummary.Dock = System.Windows.Forms.DockStyle.Fill;
          this._pnlSummary.Location = new System.Drawing.Point(0, 0);
          this._pnlSummary.Name = "_pnlSummary";
          this._pnlSummary.Size = new System.Drawing.Size(446, 274);
          this._pnlSummary.TabIndex = 2;
          this._pnlSummary.Visible = false;
          // 
          // _tbFilterDescription
          // 
          this._tbFilterDescription.Location = new System.Drawing.Point(16, 72);
          this._tbFilterDescription.Multiline = true;
          this._tbFilterDescription.Name = "_tbFilterDescription";
          this._tbFilterDescription.Size = new System.Drawing.Size(408, 40);
          this._tbFilterDescription.TabIndex = 22;
          this._tbFilterDescription.TextChanged += new System.EventHandler(this.TextChanged_tbFilterDescription);
          // 
          // _lblFilterDescription
          // 
          this._lblFilterDescription.Location = new System.Drawing.Point(16, 56);
          this._lblFilterDescription.Name = "_lblFilterDescription";
          this._lblFilterDescription.Size = new System.Drawing.Size(216, 16);
          this._lblFilterDescription.TabIndex = 21;
          this._lblFilterDescription.Text = "Specify a description for this filter";
          this._lblFilterDescription.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
          // 
          // _rtfSummary
          // 
          this._rtfSummary.Location = new System.Drawing.Point(16, 169);
          this._rtfSummary.Name = "_rtfSummary";
          this._rtfSummary.ReadOnly = true;
          this._rtfSummary.Size = new System.Drawing.Size(408, 98);
          this._rtfSummary.TabIndex = 20;
          this._rtfSummary.Text = "richTextBox1";
          this._rtfSummary.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MouseMove_rtfBox);
          this._rtfSummary.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MouseDown_rtfBox);
          // 
          // _checkBoxEnableFilter
          // 
          this._checkBoxEnableFilter.Location = new System.Drawing.Point(16, 128);
          this._checkBoxEnableFilter.Name = "_checkBoxEnableFilter";
          this._checkBoxEnableFilter.Size = new System.Drawing.Size(128, 16);
          this._checkBoxEnableFilter.TabIndex = 18;
          this._checkBoxEnableFilter.Text = "Enable this filter now";
          this._checkBoxEnableFilter.CheckedChanged += new System.EventHandler(this.CheckedChanged_checkBoxEnableFilter);
          // 
          // _tbFilterName
          // 
          this._tbFilterName.Location = new System.Drawing.Point(16, 24);
          this._tbFilterName.Name = "_tbFilterName";
          this._tbFilterName.Size = new System.Drawing.Size(160, 20);
          this._tbFilterName.TabIndex = 13;
          this._tbFilterName.Text = "New Alert Rule";
          this._tbFilterName.TextChanged += new System.EventHandler(this.TextChanged_tbFilterName);
          // 
          // _lblRtfSummary
          // 
          this._lblRtfSummary.Location = new System.Drawing.Point(16, 152);
          this._lblRtfSummary.Name = "_lblRtfSummary";
          this._lblRtfSummary.Size = new System.Drawing.Size(368, 16);
          this._lblRtfSummary.TabIndex = 12;
          this._lblRtfSummary.Text = "Review filter details (click on an underlined value to edit)";
          this._lblRtfSummary.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
          // 
          // _lblFilterName
          // 
          this._lblFilterName.Location = new System.Drawing.Point(16, 8);
          this._lblFilterName.Name = "_lblFilterName";
          this._lblFilterName.Size = new System.Drawing.Size(160, 16);
          this._lblFilterName.TabIndex = 10;
          this._lblFilterName.Text = "Specify a name for this filter";
          this._lblFilterName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
          // 
          // _pnlFilterType
          // 
          this._pnlFilterType.Controls.Add(this._rtfFilterType);
          this._pnlFilterType.Controls.Add(this._lblRtfFilterType);
          this._pnlFilterType.Controls.Add(this._groupBoxFilterType);
          this._pnlFilterType.Dock = System.Windows.Forms.DockStyle.Fill;
          this._pnlFilterType.Location = new System.Drawing.Point(0, 0);
          this._pnlFilterType.Name = "_pnlFilterType";
          this._pnlFilterType.Size = new System.Drawing.Size(446, 274);
          this._pnlFilterType.TabIndex = 16;
          // 
          // _rtfFilterType
          // 
          this._rtfFilterType.Location = new System.Drawing.Point(16, 169);
          this._rtfFilterType.Name = "_rtfFilterType";
          this._rtfFilterType.ReadOnly = true;
          this._rtfFilterType.Size = new System.Drawing.Size(408, 98);
          this._rtfFilterType.TabIndex = 14;
          this._rtfFilterType.Text = "";
          this._rtfFilterType.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MouseMove_rtfBox);
          this._rtfFilterType.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MouseDown_rtfBox);
          // 
          // _lblRtfFilterType
          // 
          this._lblRtfFilterType.Location = new System.Drawing.Point(16, 144);
          this._lblRtfFilterType.Name = "_lblRtfFilterType";
          this._lblRtfFilterType.Size = new System.Drawing.Size(360, 21);
          this._lblRtfFilterType.TabIndex = 12;
          this._lblRtfFilterType.Text = "Edit filter details (click on an underlined value)";
          this._lblRtfFilterType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
          // 
          // _groupBoxFilterType
          // 
          this._groupBoxFilterType.Controls.Add(this._linkEventType);
          this._groupBoxFilterType.Controls.Add(this._linkCategory);
          this._groupBoxFilterType.Controls.Add(this._rbCategory);
          this._groupBoxFilterType.Controls.Add(this._rbAllEvents);
          this._groupBoxFilterType.Controls.Add(this._rbEventType);
          this._groupBoxFilterType.Location = new System.Drawing.Point(16, 8);
          this._groupBoxFilterType.Name = "_groupBoxFilterType";
          this._groupBoxFilterType.Size = new System.Drawing.Size(408, 128);
          this._groupBoxFilterType.TabIndex = 15;
          this._groupBoxFilterType.TabStop = false;
          this._groupBoxFilterType.Text = "Filter these events";
          // 
          // _linkEventType
          // 
          this._linkEventType.Enabled = false;
          this._linkEventType.Location = new System.Drawing.Point(104, 99); //SQLCM-5733
          this._linkEventType.Name = "_linkEventType";
          this._linkEventType.Size = new System.Drawing.Size(232, 16);
          this._linkEventType.TabIndex = 7;
          this._linkEventType.TabStop = true;
          this._linkEventType.Text = "CREATE INDEX";
          this._linkEventType.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkClicked_linkEventType);
          // 
          // _linkCategory
          // 
          this._linkCategory.Enabled = false;
          this._linkCategory.Location = new System.Drawing.Point(104, 63); //SQLCM-5733
          this._linkCategory.Name = "_linkCategory";
          this._linkCategory.Size = new System.Drawing.Size(232, 16);
          this._linkCategory.TabIndex = 6;
          this._linkCategory.TabStop = true;
          this._linkCategory.Text = "Database Modification (DML)";
          this._linkCategory.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkClicked_linkCategory);
          // 
          // _rbCategory
          // 
          this._rbCategory.Location = new System.Drawing.Point(16, 60);
          this._rbCategory.Name = "_rbCategory";
          this._rbCategory.Size = new System.Drawing.Size(72, 17); //SQLCM-5733
          this._rbCategory.TabIndex = 5;
          this._rbCategory.Text = "Category:";
          this._rbCategory.Click += new System.EventHandler(this.Click_EventTypeRadioButton);
          // 
          // _rbAllEvents
          // 
          this._rbAllEvents.Checked = true;
          this._rbAllEvents.Location = new System.Drawing.Point(16, 24);
          this._rbAllEvents.Name = "_rbAllEvents";
          this._rbAllEvents.Size = new System.Drawing.Size(80, 16); 
          this._rbAllEvents.TabIndex = 3;
          this._rbAllEvents.TabStop = true;
          this._rbAllEvents.Text = "All Events";
          this._rbAllEvents.Click += new System.EventHandler(this.Click_EventTypeRadioButton);
          // 
          // _rbEventType
          // 
          this._rbEventType.Location = new System.Drawing.Point(16, 96);
          this._rbEventType.Name = "_rbEventType";
          this._rbEventType.Size = new System.Drawing.Size(79, 17); //SQLCM-5733
          this._rbEventType.TabIndex = 0;
          this._rbEventType.Text = "Event type:";
          this._rbEventType.Click += new System.EventHandler(this.Click_EventTypeRadioButton);
          // 
          // _pnlTargetObjects
          // 
          this._pnlTargetObjects.Controls.Add(this._rtfTargetObjects);
          this._pnlTargetObjects.Controls.Add(this._listBoxTargetObjects);
          this._pnlTargetObjects.Controls.Add(this._lblRtfTargetObjects);
          this._pnlTargetObjects.Controls.Add(this._lblTargetObjects);
          this._pnlTargetObjects.Dock = System.Windows.Forms.DockStyle.Fill;
          this._pnlTargetObjects.Location = new System.Drawing.Point(0, 0);
          this._pnlTargetObjects.Name = "_pnlTargetObjects";
          this._pnlTargetObjects.Size = new System.Drawing.Size(446, 274);
          this._pnlTargetObjects.TabIndex = 17;
          this._pnlTargetObjects.Visible = false;
          // 
          // _rtfTargetObjects
          // 
          this._rtfTargetObjects.Location = new System.Drawing.Point(16, 169);
          this._rtfTargetObjects.Name = "_rtfTargetObjects";
          this._rtfTargetObjects.ReadOnly = true;
          this._rtfTargetObjects.Size = new System.Drawing.Size(408, 98);
          this._rtfTargetObjects.TabIndex = 14;
          this._rtfTargetObjects.Text = "";
          this._rtfTargetObjects.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MouseMove_rtfBox);
          this._rtfTargetObjects.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MouseDown_rtfBox);
          // 
          // _listBoxTargetObjects
          // 
          this._listBoxTargetObjects.CheckOnClick = true;
          this._listBoxTargetObjects.Location = new System.Drawing.Point(16, 33);
          this._listBoxTargetObjects.Name = "_listBoxTargetObjects";
          this._listBoxTargetObjects.Size = new System.Drawing.Size(408, 94);
          this._listBoxTargetObjects.TabIndex = 13;
          this._listBoxTargetObjects.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.ItemCheck_listBoxTargetObjects);
          // 
          // _lblRtfTargetObjects
          // 
          this._lblRtfTargetObjects.Location = new System.Drawing.Point(16, 144);
          this._lblRtfTargetObjects.Name = "_lblRtfTargetObjects";
          this._lblRtfTargetObjects.Size = new System.Drawing.Size(360, 21);
          this._lblRtfTargetObjects.TabIndex = 12;
          this._lblRtfTargetObjects.Text = "Edit filter details (click on an underlined value)";
          this._lblRtfTargetObjects.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
          // 
          // _lblTargetObjects
          // 
          this._lblTargetObjects.Location = new System.Drawing.Point(16, 8);
          this._lblTargetObjects.Name = "_lblTargetObjects";
          this._lblTargetObjects.Size = new System.Drawing.Size(320, 21);
          this._lblTargetObjects.TabIndex = 10;
          this._lblTargetObjects.Text = "Filter events on these objects:";
          this._lblTargetObjects.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
          // 
          // _pnlBottom
          // 
          this._pnlBottom.Controls.Add(this._btnCancel);
          this._pnlBottom.Controls.Add(this._btnNext);
          this._pnlBottom.Controls.Add(this._btnBack);
          this._pnlBottom.Controls.Add(this._btnFinish);
          this._pnlBottom.Location = new System.Drawing.Point(0, 336);
          this._pnlBottom.Name = "_pnlBottom";
          this._pnlBottom.Size = new System.Drawing.Size(556, 38);
          this._pnlBottom.TabIndex = 0;
          // 
          // _btnCancel
          // 
          this._btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
          this._btnCancel.Location = new System.Drawing.Point(490, 10);
          this._btnCancel.Name = "_btnCancel";
          this._btnCancel.Size = new System.Drawing.Size(62, 20);
          this._btnCancel.TabIndex = 3;
          this._btnCancel.Text = "Cancel";
          // 
          // _btnNext
          // 
          this._btnNext.Location = new System.Drawing.Point(350, 10);
          this._btnNext.Name = "_btnNext";
          this._btnNext.Size = new System.Drawing.Size(62, 20);
          this._btnNext.TabIndex = 0;
          this._btnNext.Text = "Next >";
          this._btnNext.Click += new System.EventHandler(this.Click_btnNext);
          // 
          // _btnBack
          // 
          this._btnBack.Enabled = false;
          this._btnBack.Location = new System.Drawing.Point(280, 10);
          this._btnBack.Name = "_btnBack";
          this._btnBack.Size = new System.Drawing.Size(62, 20);
          this._btnBack.TabIndex = 0;
          this._btnBack.Text = "< Back";
          this._btnBack.Click += new System.EventHandler(this.Click_btnBack);
          // 
          // _btnFinish
          // 
          this._btnFinish.DialogResult = System.Windows.Forms.DialogResult.OK;
          this._btnFinish.Enabled = false;
          this._btnFinish.Location = new System.Drawing.Point(420, 10);
          this._btnFinish.Name = "_btnFinish";
          this._btnFinish.Size = new System.Drawing.Size(62, 20);
          this._btnFinish.TabIndex = 2;
          this._btnFinish.Text = "Finish";
          // 
          // _pnlTop
          // 
          this._pnlTop.BackColor = System.Drawing.Color.White;
          this._pnlTop.Controls.Add(this._lblDescription);
          this._pnlTop.Controls.Add(this._lblTitle);
          this._pnlTop.Location = new System.Drawing.Point(110, 0);
          this._pnlTop.Name = "_pnlTop";
          this._pnlTop.Size = new System.Drawing.Size(446, 60);
          this._pnlTop.TabIndex = 2;
          // 
          // _lblDescription
          // 
          this._lblDescription.Location = new System.Drawing.Point(14, 24);
          this._lblDescription.Name = "_lblDescription";
          this._lblDescription.Size = new System.Drawing.Size(416, 28);
          this._lblDescription.TabIndex = 1;
          this._lblDescription.Text = "Description";
          // 
          // _lblTitle
          // 
          this._lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
          this._lblTitle.Location = new System.Drawing.Point(14, 8);
          this._lblTitle.Name = "_lblTitle";
          this._lblTitle.Size = new System.Drawing.Size(281, 16);
          this._lblTitle.TabIndex = 0;
          this._lblTitle.Text = "Title";
          this._lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
          // 
          // _lblBorder1
          // 
          this._lblBorder1.BackColor = System.Drawing.Color.Black;
          this._lblBorder1.Location = new System.Drawing.Point(110, 60);
          this._lblBorder1.Name = "_lblBorder1";
          this._lblBorder1.Size = new System.Drawing.Size(446, 1);
          this._lblBorder1.TabIndex = 3;
          this._lblBorder1.Text = "label1";
          // 
          // _lblBorder2
          // 
          this._lblBorder2.BackColor = System.Drawing.Color.Black;
          this._lblBorder2.Location = new System.Drawing.Point(0, 335);
          this._lblBorder2.Name = "_lblBorder2";
          this._lblBorder2.Size = new System.Drawing.Size(556, 1);
          this._lblBorder2.TabIndex = 4;
          this._lblBorder2.Text = "label2";
          // 
          // Form_EventFilterWizard
          // 
          this.AcceptButton = this._btnFinish;
          this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
          this.CancelButton = this._btnCancel;
          this.ClientSize = new System.Drawing.Size(556, 374);
          this.Controls.Add(this._lblBorder2);
          this.Controls.Add(this._lblBorder1);
          this.Controls.Add(this._pnlLeft);
          this.Controls.Add(this._pnlBottom);
          this.Controls.Add(this._pnlCenter);
          this.Controls.Add(this._pnlTop);
          this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
          this.HelpButton = true;
          this.MaximizeBox = false;
          this.MinimizeBox = false;
          this.Name = "Form_EventFilterWizard";
          this.ShowInTaskbar = false;
          this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
          this.Text = "New Alert Rule";
          this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.Form_AlertRuleWizard_HelpRequested);
          this._pnlLeft.ResumeLayout(false);
          ((System.ComponentModel.ISupportInitialize)(this._pictureBox)).EndInit();
          this._pnlCenter.ResumeLayout(false);
          this._pnlAdditionalFilters.ResumeLayout(false);
          this._pnlSummary.ResumeLayout(false);
          this._pnlSummary.PerformLayout();
          this._pnlFilterType.ResumeLayout(false);
          this._groupBoxFilterType.ResumeLayout(false);
          this._pnlTargetObjects.ResumeLayout(false);
          this._pnlBottom.ResumeLayout(false);
          this._pnlTop.ResumeLayout(false);
          this.ResumeLayout(false);

      }
      #endregion

      private System.Windows.Forms.Panel _pnlLeft;
      private System.Windows.Forms.Panel _pnlCenter;
      private System.Windows.Forms.Panel _pnlBottom;
      private System.Windows.Forms.Button _btnBack;
      private System.Windows.Forms.Button _btnNext;
      private System.Windows.Forms.Button _btnFinish;
      private System.Windows.Forms.Button _btnCancel;
      private System.Windows.Forms.PictureBox _pictureBox;
      private System.Windows.Forms.Panel _pnlTop;
      private System.Windows.Forms.Label _lblTitle;
      private System.Windows.Forms.Label _lblDescription;
      private System.Windows.Forms.Label _lblBorder2;
      private System.Windows.Forms.Label _lblRtfAdditionalFilters;
      private System.Windows.Forms.Label _lblAdditionalFilters;
      private System.Windows.Forms.CheckedListBox _listBoxAdditionalFilters;
      private System.Windows.Forms.Label _lblBorder1;
      private System.Windows.Forms.Label _lblRtfSummary;
      private System.Windows.Forms.Label _lblFilterName;
      private System.Windows.Forms.TextBox _tbFilterName;
      private System.Windows.Forms.CheckBox _checkBoxEnableFilter;
      private System.Windows.Forms.RichTextBox _rtfSummary;
      private System.Windows.Forms.Label _lblFilterDescription;
      private System.Windows.Forms.TextBox _tbFilterDescription;
      private System.Windows.Forms.Panel _pnlFilterType;
      private System.Windows.Forms.GroupBox _groupBoxFilterType;
      private System.Windows.Forms.Label _lblRtfFilterType;
      private System.Windows.Forms.Panel _pnlTargetObjects;
      private System.Windows.Forms.Label _lblRtfTargetObjects;
      private System.Windows.Forms.Label _lblTargetObjects;
      private System.Windows.Forms.RichTextBox _rtfFilterType;
      private System.Windows.Forms.RichTextBox _rtfTargetObjects;
      private System.Windows.Forms.CheckedListBox _listBoxTargetObjects;
      private System.Windows.Forms.Panel _pnlSummary;
      private System.Windows.Forms.Panel _pnlAdditionalFilters;
      private System.Windows.Forms.RichTextBox _rtfAdditionalFilters;
      private System.Windows.Forms.RadioButton _rbCategory;
      private System.Windows.Forms.RadioButton _rbAllEvents;
      private System.Windows.Forms.RadioButton _rbEventType;
      private System.Windows.Forms.LinkLabel _linkCategory;
      private System.Windows.Forms.LinkLabel _linkEventType;
	}
}