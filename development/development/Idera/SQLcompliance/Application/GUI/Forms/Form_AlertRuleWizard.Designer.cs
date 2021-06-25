namespace Idera.SQLcompliance.Application.GUI.Forms
{
   partial class Form_AlertRuleWizard
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
          this._pnlSummary = new System.Windows.Forms.Panel();
          this._tbP4RuleDescription = new System.Windows.Forms.TextBox();
          this._lblP4RuleDescription = new System.Windows.Forms.Label();
          this._rtfP4RuleDetails = new System.Windows.Forms.RichTextBox();
          this._checkBoxP4EnableRule = new System.Windows.Forms.CheckBox();
          this._comboP4AlertLevel = new System.Windows.Forms.ComboBox();
          this._lblP4S2RuleOptions = new System.Windows.Forms.Label();
          this._tbP4RuleName = new System.Windows.Forms.TextBox();
          this._lblP4S3RuleDetails = new System.Windows.Forms.Label();
          this._lblP4S1NameRule = new System.Windows.Forms.Label();
            this._lblAlertRuleActiveStartTime = new System.Windows.Forms.Label();
            this._lblAlertRuleActiveEndTime = new System.Windows.Forms.Label();
            this._lblAlertRuleActiveOn = new System.Windows.Forms.Label();
            this._alertRuleActiveStartTime = new System.Windows.Forms.DateTimePicker();
            this._alertRuleActiveEndTime = new System.Windows.Forms.DateTimePicker();
            this._chkAlertRuleActiveOnMon = new System.Windows.Forms.CheckBox();
            this._chkAlertRuleActiveOnTue = new System.Windows.Forms.CheckBox();
            this._chkAlertRuleActiveOnWed = new System.Windows.Forms.CheckBox();
            this._chkAlertRuleActiveOnThu = new System.Windows.Forms.CheckBox();
            this._chkAlertRuleActiveOnFri = new System.Windows.Forms.CheckBox();
            this._chkAlertRuleActiveOnSat = new System.Windows.Forms.CheckBox();
            this._chkAlertRuleActiveOnSun = new System.Windows.Forms.CheckBox();
            this._pnlActions = new System.Windows.Forms.Panel();
            this._lblEmailSummaryNotification = new System.Windows.Forms.Label();
            this._txtEmailSummaryIntervalHours = new System.Windows.Forms.NumericUpDown();
            this._lblEmailSummaryIntervalHours = new System.Windows.Forms.Label();
            this._txtEmailSummaryIntervalMinutes = new System.Windows.Forms.NumericUpDown();
            this._lblEmailSummaryIntervalMinutes = new System.Windows.Forms.Label();
            this._rtfPNewRuleDetails = new System.Windows.Forms.RichTextBox();
          this._rtfP3RuleDetails = new System.Windows.Forms.RichTextBox();
          this._listBoxP3AlertActions = new System.Windows.Forms.CheckedListBox();
          this._lblP3S2RuleDetails = new System.Windows.Forms.Label();
          this._lblP3S1AlertActions = new System.Windows.Forms.Label();
          this._pnlAdditionalFilters = new System.Windows.Forms.Panel();
          this._pnlAlertRuleTimeFrame = new System.Windows.Forms.Panel();
          this._rtfAdditionalFilters = new System.Windows.Forms.RichTextBox();
          this._listBoxAdditionalFilters = new System.Windows.Forms.CheckedListBox();
          this._lblP2S2RuleDetails = new System.Windows.Forms.Label();
          this._lblP2S1AlertConditions = new System.Windows.Forms.Label();
          this._pnlTargetObjects = new System.Windows.Forms.Panel();
          this._rtfTargetObjects = new System.Windows.Forms.RichTextBox();
          this._listBoxTargetObjects = new System.Windows.Forms.CheckedListBox();
          this.label2 = new System.Windows.Forms.Label();
          this.label3 = new System.Windows.Forms.Label();
          this._pnlVerb = new System.Windows.Forms.Panel();
          this._rtfVerb = new System.Windows.Forms.RichTextBox();
          this.label1 = new System.Windows.Forms.Label();
          this.groupBox1 = new System.Windows.Forms.GroupBox();
          this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this._linkSpecificEvent = new System.Windows.Forms.LinkLabel();
          this._rbSpecificEvent = new System.Windows.Forms.RadioButton();
          this._rbUserDefined = new System.Windows.Forms.RadioButton();
          this._rbAdministrative = new System.Windows.Forms.RadioButton();
          this._rbDDL = new System.Windows.Forms.RadioButton();
          this._rbSecurity = new System.Windows.Forms.RadioButton();
          this._rbDML = new System.Windows.Forms.RadioButton();
          this._rbLogins = new System.Windows.Forms.RadioButton();
            this._rbAlertRuleActiveAllTimes = new System.Windows.Forms.RadioButton();
            this._rbAlertRulesActiveSpecifiedTimeframe = new System.Windows.Forms.RadioButton();
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
          this._pnlSummary.SuspendLayout();
          this._pnlActions.SuspendLayout();
          this._pnlAdditionalFilters.SuspendLayout();
            this._pnlAlertRuleTimeFrame.SuspendLayout();
            this._pnlTargetObjects.SuspendLayout();
          this._pnlVerb.SuspendLayout();
          this.groupBox1.SuspendLayout();
          this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
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
          this._pictureBox.Image = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.Wizard_AlertRules;
          this._pictureBox.Location = new System.Drawing.Point(0, 0);
          this._pictureBox.Name = "_pictureBox";
          this._pictureBox.Size = new System.Drawing.Size(110, 335);
          this._pictureBox.TabIndex = 0;
          this._pictureBox.TabStop = false;
          // 
          // _pnlCenter
          // 
          this._pnlCenter.Controls.Add(this._pnlSummary);
          this._pnlCenter.Controls.Add(this._pnlActions);
          this._pnlCenter.Controls.Add(this._pnlAdditionalFilters);
            this._pnlCenter.Controls.Add(this._pnlAlertRuleTimeFrame);
            this._pnlCenter.Controls.Add(this._pnlTargetObjects);
          this._pnlCenter.Controls.Add(this._pnlVerb);
          this._pnlCenter.Location = new System.Drawing.Point(110, 61);
          this._pnlCenter.Name = "_pnlCenter";
          this._pnlCenter.Size = new System.Drawing.Size(446, 274);
          this._pnlCenter.TabIndex = 1;
          // 
          // _pnlSummary
          // 
          this._pnlSummary.Controls.Add(this._tbP4RuleDescription);
          this._pnlSummary.Controls.Add(this._lblP4RuleDescription);
          this._pnlSummary.Controls.Add(this._rtfP4RuleDetails);
          this._pnlSummary.Controls.Add(this._checkBoxP4EnableRule);
          this._pnlSummary.Controls.Add(this._comboP4AlertLevel);
          this._pnlSummary.Controls.Add(this._lblP4S2RuleOptions);
          this._pnlSummary.Controls.Add(this._tbP4RuleName);
          this._pnlSummary.Controls.Add(this._lblP4S3RuleDetails);
          this._pnlSummary.Controls.Add(this._lblP4S1NameRule);
          this._pnlSummary.Dock = System.Windows.Forms.DockStyle.Fill;
          this._pnlSummary.Location = new System.Drawing.Point(0, 0);
          this._pnlSummary.Name = "_pnlSummary";
          this._pnlSummary.Size = new System.Drawing.Size(446, 274);
          this._pnlSummary.TabIndex = 2;
          this._pnlSummary.Visible = false;
          // 
          // _tbP4RuleDescription
          // 
          this._tbP4RuleDescription.Location = new System.Drawing.Point(16, 72);
          this._tbP4RuleDescription.Multiline = true;
          this._tbP4RuleDescription.Name = "_tbP4RuleDescription";
          this._tbP4RuleDescription.Size = new System.Drawing.Size(408, 40);
          this._tbP4RuleDescription.TabIndex = 22;
          this._tbP4RuleDescription.TextChanged += new System.EventHandler(this.TextChanged_tbP4RuleDescription);
          // 
          // _lblP4RuleDescription
          // 
          this._lblP4RuleDescription.Location = new System.Drawing.Point(16, 56);
          this._lblP4RuleDescription.Name = "_lblP4RuleDescription";
          this._lblP4RuleDescription.Size = new System.Drawing.Size(168, 16);
          this._lblP4RuleDescription.TabIndex = 21;
          this._lblP4RuleDescription.Text = "Specify a description for this rule";
          this._lblP4RuleDescription.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
          // 
          // _rtfP4RuleDetails
          // 
          this._rtfP4RuleDetails.Location = new System.Drawing.Point(16, 154);
          this._rtfP4RuleDetails.Name = "_rtfP4RuleDetails";
          this._rtfP4RuleDetails.ReadOnly = true;
          this._rtfP4RuleDetails.Size = new System.Drawing.Size(408, 113);
          this._rtfP4RuleDetails.TabIndex = 20;
          this._rtfP4RuleDetails.Text = "richTextBox1";
          this._rtfP4RuleDetails.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MouseMove_rtfBox);
          this._rtfP4RuleDetails.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MouseDown_rtfBox);
          // 
          // _checkBoxP4EnableRule
          // 
          this._checkBoxP4EnableRule.Location = new System.Drawing.Point(308, 136);
          this._checkBoxP4EnableRule.Name = "_checkBoxP4EnableRule";
          this._checkBoxP4EnableRule.Size = new System.Drawing.Size(122, 16);
          this._checkBoxP4EnableRule.TabIndex = 18;
          this._checkBoxP4EnableRule.Text = "Enable this rule now";
          this._checkBoxP4EnableRule.CheckedChanged += new System.EventHandler(this.CheckedChanged_cbP4EnableRule);
          // 
          // _comboP4AlertLevel
          // 
          this._comboP4AlertLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
          this._comboP4AlertLevel.Items.AddRange(new object[] {
            "1 - Low",
            "2 - Medium",
            "3 - High",
            "4 - Severe"});
          this._comboP4AlertLevel.Location = new System.Drawing.Point(248, 23);
          this._comboP4AlertLevel.Name = "_comboP4AlertLevel";
          this._comboP4AlertLevel.Size = new System.Drawing.Size(170, 21);
          this._comboP4AlertLevel.TabIndex = 17;
          this._comboP4AlertLevel.SelectedIndexChanged += new System.EventHandler(this.SelectedIndexChanged_comboP4AlertLevel);
          // 
          // _lblP4S2RuleOptions
          // 
          this._lblP4S2RuleOptions.Location = new System.Drawing.Point(246, 8);
          this._lblP4S2RuleOptions.Name = "_lblP4S2RuleOptions";
          this._lblP4S2RuleOptions.Size = new System.Drawing.Size(96, 16);
          this._lblP4S2RuleOptions.TabIndex = 15;
          this._lblP4S2RuleOptions.Text = "Specify alert level:";
          this._lblP4S2RuleOptions.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
          // 
          // _tbP4RuleName
          // 
          this._tbP4RuleName.Location = new System.Drawing.Point(16, 24);
          this._tbP4RuleName.Name = "_tbP4RuleName";
          this._tbP4RuleName.Size = new System.Drawing.Size(216, 20);
          this._tbP4RuleName.TabIndex = 13;
          this._tbP4RuleName.Text = "New Alert Rule";
          this._tbP4RuleName.TextChanged += new System.EventHandler(this.TextChanged_tbP4RuleName);
          // 
          // _lblP4S3RuleDetails
          // 
          this._lblP4S3RuleDetails.Location = new System.Drawing.Point(16, 135);
          this._lblP4S3RuleDetails.Name = "_lblP4S3RuleDetails";
          this._lblP4S3RuleDetails.Size = new System.Drawing.Size(368, 16);
          this._lblP4S3RuleDetails.TabIndex = 12;
          this._lblP4S3RuleDetails.Text = "Review rule details (click on an underlined value to edit)";
          this._lblP4S3RuleDetails.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
          // 
          // _lblP4S1NameRule
          // 
          this._lblP4S1NameRule.Location = new System.Drawing.Point(16, 8);
          this._lblP4S1NameRule.Name = "_lblP4S1NameRule";
          this._lblP4S1NameRule.Size = new System.Drawing.Size(160, 16);
          this._lblP4S1NameRule.TabIndex = 10;
          this._lblP4S1NameRule.Text = "Specify a name for this rule";
          this._lblP4S1NameRule.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
          // 
          // _pnlActions
          // 
          this._pnlActions.Controls.Add(this._rtfP3RuleDetails);
          this._pnlActions.Controls.Add(this._listBoxP3AlertActions);
          this._pnlActions.Controls.Add(this._lblP3S2RuleDetails);
          this._pnlActions.Controls.Add(this._lblP3S1AlertActions);
            this._pnlActions.Controls.Add(this.groupBox4);
            this._pnlActions.Dock = System.Windows.Forms.DockStyle.Fill;
          this._pnlActions.Location = new System.Drawing.Point(0, 0);
          this._pnlActions.Name = "_pnlActions";
          this._pnlActions.Size = new System.Drawing.Size(446, 274);
          this._pnlActions.TabIndex = 1;
          this._pnlActions.Visible = false;
          // 
          // _rtfP3RuleDetails
          // 
          this._rtfP3RuleDetails.Location = new System.Drawing.Point(16, 154);
          this._rtfP3RuleDetails.Name = "_rtfP3RuleDetails";
          this._rtfP3RuleDetails.ReadOnly = true;
          this._rtfP3RuleDetails.Size = new System.Drawing.Size(408, 113);
          this._rtfP3RuleDetails.TabIndex = 18;
          this._rtfP3RuleDetails.Text = "richTextBox1";
          this._rtfP3RuleDetails.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MouseMove_rtfBox);
          this._rtfP3RuleDetails.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MouseDown_rtfBox);
          // 
          // _listBoxP3AlertActions
          // 
          this._listBoxP3AlertActions.CheckOnClick = true;
          this._listBoxP3AlertActions.Location = new System.Drawing.Point(16, 33);
          this._listBoxP3AlertActions.Name = "_listBoxP3AlertActions";
          this._listBoxP3AlertActions.Size = new System.Drawing.Size(408, 74);
          this._listBoxP3AlertActions.TabIndex = 17;
          this._listBoxP3AlertActions.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.ItemCheck_listBoxP3AlertActions);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this._lblEmailSummaryNotification);
            this.groupBox4.Controls.Add(this._txtEmailSummaryIntervalHours);
            this.groupBox4.Controls.Add(this._lblEmailSummaryIntervalHours);
            this.groupBox4.Controls.Add(this._txtEmailSummaryIntervalMinutes);
            this.groupBox4.Controls.Add(this._lblEmailSummaryIntervalMinutes);
            this.groupBox4.Location = new System.Drawing.Point(16, 100);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(408, 35);
            this.groupBox4.TabIndex = 10;
            this.groupBox4.TabStop = false;
            this.groupBox4.Enabled = false;
            // 
            // _lblEmailSummaryNotification
            // 
            this._lblEmailSummaryNotification.Location = new System.Drawing.Point(4, 10);
            this._lblEmailSummaryNotification.Name = "_lblEmailSummaryNotification";
            this._lblEmailSummaryNotification.Size = new System.Drawing.Size(200, 21);
            this._lblEmailSummaryNotification.TabIndex = 16;
            this._lblEmailSummaryNotification.Text = "Send email summary notification every: ";
            this._lblEmailSummaryNotification.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _txtEmailSummaryIntervalHours
            // 
            this._txtEmailSummaryIntervalHours.Location = new System.Drawing.Point(208, 10);
            this._txtEmailSummaryIntervalHours.Name = "_txtEmailSummaryIntervalHours";
            this._txtEmailSummaryIntervalHours.Size = new System.Drawing.Size(35, 16);
            this._txtEmailSummaryIntervalHours.Minimum = 00;
            this._txtEmailSummaryIntervalHours.Maximum = 23;
            this._txtEmailSummaryIntervalHours.ValueChanged += new System.EventHandler(this.ValueChanged_EmailSummaryInterval);
            // 
            // _lblEmailSummaryIntervalHours
            // 
            this._lblEmailSummaryIntervalHours.Location = new System.Drawing.Point(250, 10);
            this._lblEmailSummaryIntervalHours.Name = "_lblEmailSummaryIntervalHours";
            this._lblEmailSummaryIntervalHours.Size = new System.Drawing.Size(50, 21);
            this._lblEmailSummaryIntervalHours.TabIndex = 16;
            this._lblEmailSummaryIntervalHours.Text = "hour(s)";
            this._lblEmailSummaryIntervalHours.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _txtEmailSummaryIntervalMinutes
            // 
            this._txtEmailSummaryIntervalMinutes.Location = new System.Drawing.Point(308, 10);
            this._txtEmailSummaryIntervalMinutes.Name = "_txtEmailSummaryIntervalMinutes";
            this._txtEmailSummaryIntervalMinutes.Size = new System.Drawing.Size(35, 16);
            this._txtEmailSummaryIntervalMinutes.Minimum = 30;
            this._txtEmailSummaryIntervalMinutes.Maximum = 59;
            this._txtEmailSummaryIntervalMinutes.ValueChanged += new System.EventHandler(this.ValueChanged_EmailSummaryInterval);
            // 
            // _lblEmailSummaryIntervalMinutes
            // 
            this._lblEmailSummaryIntervalMinutes.Location = new System.Drawing.Point(350, 10);
            this._lblEmailSummaryIntervalMinutes.Name = "_lblEmailSummaryIntervalMinutes";
            this._lblEmailSummaryIntervalMinutes.Size = new System.Drawing.Size(50, 21);
            this._lblEmailSummaryIntervalMinutes.TabIndex = 17;
            this._lblEmailSummaryIntervalMinutes.Text = "minute(s)";
            this._lblEmailSummaryIntervalMinutes.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _lblP3S2RuleDetails
            // 
            this._lblP3S2RuleDetails.Location = new System.Drawing.Point(16, 134);
          this._lblP3S2RuleDetails.Name = "_lblP3S2RuleDetails";
          this._lblP3S2RuleDetails.Size = new System.Drawing.Size(360, 21);
          this._lblP3S2RuleDetails.TabIndex = 16;
          this._lblP3S2RuleDetails.Text = "Edit rule details (click on an underlined value)";
          this._lblP3S2RuleDetails.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
          // 
          // _lblP3S1AlertActions
          // 
          this._lblP3S1AlertActions.Location = new System.Drawing.Point(16, 8);
          this._lblP3S1AlertActions.Name = "_lblP3S1AlertActions";
          this._lblP3S1AlertActions.Size = new System.Drawing.Size(224, 21);
          this._lblP3S1AlertActions.TabIndex = 14;
          this._lblP3S1AlertActions.Text = "Select alert actions";
          this._lblP3S1AlertActions.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
          // 
          // _pnlAdditionalFilters
          // 
          this._pnlAdditionalFilters.Controls.Add(this._rtfAdditionalFilters);
          this._pnlAdditionalFilters.Controls.Add(this._listBoxAdditionalFilters);
          this._pnlAdditionalFilters.Controls.Add(this._lblP2S2RuleDetails);
          this._pnlAdditionalFilters.Controls.Add(this._lblP2S1AlertConditions);
          this._pnlAdditionalFilters.Dock = System.Windows.Forms.DockStyle.Fill;
          this._pnlAdditionalFilters.Location = new System.Drawing.Point(0, 0);
          this._pnlAdditionalFilters.Name = "_pnlAdditionalFilters";
          this._pnlAdditionalFilters.Size = new System.Drawing.Size(446, 274);
          this._pnlAdditionalFilters.TabIndex = 1;
          this._pnlAdditionalFilters.Visible = false;
            // 
            // _pnlAlertRuleTimeframe
            // 
            this._pnlAlertRuleTimeFrame.Controls.Add(this.groupBox2);
            this._pnlAlertRuleTimeFrame.Controls.Add(this._rtfPNewRuleDetails);
            this._pnlAlertRuleTimeFrame.Dock = System.Windows.Forms.DockStyle.Fill;
            this._pnlAlertRuleTimeFrame.Location = new System.Drawing.Point(0, 0);
            this._pnlAlertRuleTimeFrame.Name = "_pnlAlertRuleTimeFrame";
            this._pnlAlertRuleTimeFrame.Size = new System.Drawing.Size(446, 274);
            this._pnlAlertRuleTimeFrame.TabIndex = 1;
            this._pnlAlertRuleTimeFrame.Visible = false;
            // 
            // _rtfAdditionalFilters
            // 
            this._rtfAdditionalFilters.Location = new System.Drawing.Point(16, 154);
          this._rtfAdditionalFilters.Name = "_rtfAdditionalFilters";
          this._rtfAdditionalFilters.ReadOnly = true;
          this._rtfAdditionalFilters.Size = new System.Drawing.Size(408, 113);
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
          // _lblP2S2RuleDetails
          // 
          this._lblP2S2RuleDetails.Location = new System.Drawing.Point(16, 134);
          this._lblP2S2RuleDetails.Name = "_lblP2S2RuleDetails";
          this._lblP2S2RuleDetails.Size = new System.Drawing.Size(360, 21);
          this._lblP2S2RuleDetails.TabIndex = 12;
          this._lblP2S2RuleDetails.Text = "Edit rule details (click on an underlined value)";
          this._lblP2S2RuleDetails.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
          // 
          // _lblP2S1AlertConditions
          // 
          this._lblP2S1AlertConditions.Location = new System.Drawing.Point(16, 8);
          this._lblP2S1AlertConditions.Name = "_lblP2S1AlertConditions";
          this._lblP2S1AlertConditions.Size = new System.Drawing.Size(224, 21);
          this._lblP2S1AlertConditions.TabIndex = 10;
          this._lblP2S1AlertConditions.Text = "Select additional event filters";
          this._lblP2S1AlertConditions.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
          // 
          // _pnlTargetObjects
          // 
          this._pnlTargetObjects.Controls.Add(this._rtfTargetObjects);
          this._pnlTargetObjects.Controls.Add(this._listBoxTargetObjects);
          this._pnlTargetObjects.Controls.Add(this.label2);
          this._pnlTargetObjects.Controls.Add(this.label3);
          this._pnlTargetObjects.Dock = System.Windows.Forms.DockStyle.Fill;
          this._pnlTargetObjects.Location = new System.Drawing.Point(0, 0);
          this._pnlTargetObjects.Name = "_pnlTargetObjects";
          this._pnlTargetObjects.Size = new System.Drawing.Size(446, 274);
          this._pnlTargetObjects.TabIndex = 17;
          this._pnlTargetObjects.Visible = false;
          // 
          // _rtfTargetObjects
          // 
          this._rtfTargetObjects.Location = new System.Drawing.Point(16, 154);
          this._rtfTargetObjects.Name = "_rtfTargetObjects";
          this._rtfTargetObjects.ReadOnly = true;
          this._rtfTargetObjects.Size = new System.Drawing.Size(408, 113);
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
          // label2
          // 
          this.label2.Location = new System.Drawing.Point(16, 134);
          this.label2.Name = "label2";
          this.label2.Size = new System.Drawing.Size(360, 21);
          this.label2.TabIndex = 12;
          this.label2.Text = "Edit rule details (click on an underlined value)";
          this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
          // 
          // label3
          // 
          this.label3.Location = new System.Drawing.Point(16, 8);
          this.label3.Name = "label3";
          this.label3.Size = new System.Drawing.Size(320, 21);
          this.label3.TabIndex = 10;
          this.label3.Text = "Select the SQL Server objects:";
          this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
          // 
          // _pnlVerb
          // 
          this._pnlVerb.Controls.Add(this._rtfVerb);
          this._pnlVerb.Controls.Add(this.label1);
          this._pnlVerb.Controls.Add(this.groupBox1);
          this._pnlVerb.Dock = System.Windows.Forms.DockStyle.Fill;
          this._pnlVerb.Location = new System.Drawing.Point(0, 0);
          this._pnlVerb.Name = "_pnlVerb";
          this._pnlVerb.Size = new System.Drawing.Size(446, 274);
          this._pnlVerb.TabIndex = 16;
          // 
          // _rtfVerb
          // 
          this._rtfVerb.Location = new System.Drawing.Point(16, 154);
          this._rtfVerb.Name = "_rtfVerb";
          this._rtfVerb.ReadOnly = true;
          this._rtfVerb.Size = new System.Drawing.Size(408, 113);
          this._rtfVerb.TabIndex = 14;
          this._rtfVerb.Text = "";
          this._rtfVerb.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MouseMove_rtfBox);
          this._rtfVerb.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MouseDown_rtfBox);
          // 
          // label1
          // 
          this.label1.Location = new System.Drawing.Point(16, 134);
          this.label1.Name = "label1";
          this.label1.Size = new System.Drawing.Size(360, 21);
          this.label1.TabIndex = 12;
          this.label1.Text = "Edit rule details (click on an underlined value)";
          this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
          // 
          // groupBox1
          // 
          this.groupBox1.Controls.Add(this._linkSpecificEvent);
          this.groupBox1.Controls.Add(this._rbSpecificEvent);
          this.groupBox1.Controls.Add(this._rbUserDefined);
          this.groupBox1.Controls.Add(this._rbAdministrative);
          this.groupBox1.Controls.Add(this._rbDDL);
          this.groupBox1.Controls.Add(this._rbSecurity);
          this.groupBox1.Controls.Add(this._rbDML);
          this.groupBox1.Controls.Add(this._rbLogins);
          this.groupBox1.Location = new System.Drawing.Point(16, 8);
          this.groupBox1.Name = "groupBox1";
          this.groupBox1.Size = new System.Drawing.Size(408, 123);
          this.groupBox1.TabIndex = 15;
          this.groupBox1.TabStop = false;
          this.groupBox1.Text = "Select the event type";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this._rbAlertRuleActiveAllTimes);
            this.groupBox2.Controls.Add(this._rbAlertRulesActiveSpecifiedTimeframe);
            this.groupBox2.Controls.Add(this.groupBox3);
            this.groupBox2.Location = new System.Drawing.Point(16, 8);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(408, 123);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Select the alert rule timeframe";
            // 
            // groupBox3
            //
            this.groupBox3.Controls.Add(this._lblAlertRuleActiveStartTime);
            this.groupBox3.Controls.Add(this._lblAlertRuleActiveEndTime);
            this.groupBox3.Controls.Add(this._alertRuleActiveStartTime);
            this.groupBox3.Controls.Add(this._alertRuleActiveEndTime);
            this.groupBox3.Controls.Add(this._lblAlertRuleActiveOn);
            this.groupBox3.Controls.Add(this._chkAlertRuleActiveOnMon);
            this.groupBox3.Controls.Add(this._chkAlertRuleActiveOnTue);
            this.groupBox3.Controls.Add(this._chkAlertRuleActiveOnWed);
            this.groupBox3.Controls.Add(this._chkAlertRuleActiveOnThu);
            this.groupBox3.Controls.Add(this._chkAlertRuleActiveOnFri);
            this.groupBox3.Controls.Add(this._chkAlertRuleActiveOnSat);
            this.groupBox3.Controls.Add(this._chkAlertRuleActiveOnSun);
            this.groupBox3.Location = new System.Drawing.Point(16, 54);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(388, 64);
            this.groupBox3.TabIndex = 10;
            this.groupBox3.TabStop = false;
            this.groupBox3.Enabled = false;
            // 
            // _linkSpecificEvent
            // 
            this._linkSpecificEvent.Enabled = false;
          this._linkSpecificEvent.Location = new System.Drawing.Point(112, 96);
          this._linkSpecificEvent.Name = "_linkSpecificEvent";
          this._linkSpecificEvent.Size = new System.Drawing.Size(176, 16);
          this._linkSpecificEvent.TabIndex = 8;
          this._linkSpecificEvent.TabStop = true;
          this._linkSpecificEvent.Text = "CREATE INDEX";
          this._linkSpecificEvent.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkClicked_linkSpecificEvent);
          // 
          // _rbSpecificEvent
          // 
          this._rbSpecificEvent.Location = new System.Drawing.Point(16, 96);
          this._rbSpecificEvent.Name = "_rbSpecificEvent";
          this._rbSpecificEvent.Size = new System.Drawing.Size(104, 17);
          this._rbSpecificEvent.TabIndex = 7;
          this._rbSpecificEvent.Text = "Specific Event:";
          this._rbSpecificEvent.Click += new System.EventHandler(this.Click_VerbRadioButton);
          // 
          // _rbUserDefined
          // 
          this._rbUserDefined.Location = new System.Drawing.Point(232, 72);
          this._rbUserDefined.Name = "_rbUserDefined";
          this._rbUserDefined.Size = new System.Drawing.Size(136, 17);
          this._rbUserDefined.TabIndex = 6;
          this._rbUserDefined.Text = "User Defined Events";
          this._rbUserDefined.Click += new System.EventHandler(this.Click_VerbRadioButton);
          // 
          // _rbAdministrative
          // 
          this._rbAdministrative.Location = new System.Drawing.Point(16, 48);
          this._rbAdministrative.Name = "_rbAdministrative";
          this._rbAdministrative.Size = new System.Drawing.Size(136, 17);
          this._rbAdministrative.TabIndex = 5;
          this._rbAdministrative.Text = "Administrative Activity";
          this._rbAdministrative.Click += new System.EventHandler(this.Click_VerbRadioButton);
          // 
          // _rbDDL
          // 
          this._rbDDL.Location = new System.Drawing.Point(232, 24);
          this._rbDDL.Name = "_rbDDL";
          this._rbDDL.Size = new System.Drawing.Size(136, 17);
          this._rbDDL.TabIndex = 4;
          this._rbDDL.Text = "Data Definition (DDL)";
          this._rbDDL.Click += new System.EventHandler(this.Click_VerbRadioButton);
          // 
          // _rbSecurity
          // 
          this._rbSecurity.Checked = true;
          this._rbSecurity.Location = new System.Drawing.Point(16, 24);
          this._rbSecurity.Name = "_rbSecurity";
          this._rbSecurity.Size = new System.Drawing.Size(136, 17);
          this._rbSecurity.TabIndex = 3;
          this._rbSecurity.TabStop = true;
          this._rbSecurity.Text = "Security Changes";
          this._rbSecurity.Click += new System.EventHandler(this.Click_VerbRadioButton);
          // 
          // _rbDML
          // 
          this._rbDML.Location = new System.Drawing.Point(232, 48);
          this._rbDML.Name = "_rbDML";
          this._rbDML.Size = new System.Drawing.Size(152, 17);
          this._rbDML.TabIndex = 2;
          this._rbDML.Text = "Data Manipulation (DML)";
          this._rbDML.Click += new System.EventHandler(this.Click_VerbRadioButton);
          // 
          // _rbLogins
          // 
          this._rbLogins.Location = new System.Drawing.Point(16, 72);
          this._rbLogins.Name = "_rbLogins";
          this._rbLogins.Size = new System.Drawing.Size(136, 17);
          this._rbLogins.TabIndex = 0;
          this._rbLogins.Text = "Login Activity";
          this._rbLogins.Click += new System.EventHandler(this.Click_VerbRadioButton);
            // 
            // _rbAlertRulesActiveAllTimes
            // 
            this._rbAlertRuleActiveAllTimes.Checked = true;
            this._rbAlertRuleActiveAllTimes.Location = new System.Drawing.Point(16, 16);
            this._rbAlertRuleActiveAllTimes.Name = "_rbAlertRuleActiveAllTimes";
            this._rbAlertRuleActiveAllTimes.Size = new System.Drawing.Size(300, 17);
            this._rbAlertRuleActiveAllTimes.TabIndex = 6;
            this._rbAlertRuleActiveAllTimes.Text = "Keep the rule active at all times";
            this._rbAlertRuleActiveAllTimes.Click += new System.EventHandler(this.Click_AlertRulesActiveButton);
            // 
            // _rbAlertRulesActiveSpecifiedTimeframe
            // 
            this._rbAlertRulesActiveSpecifiedTimeframe.Location = new System.Drawing.Point(16, 40);
            this._rbAlertRulesActiveSpecifiedTimeframe.Name = "_rbAlertRulesActiveSpecifiedTimeframe";
            this._rbAlertRulesActiveSpecifiedTimeframe.Size = new System.Drawing.Size(300, 17);
            this._rbAlertRulesActiveSpecifiedTimeframe.TabIndex = 6;
            this._rbAlertRulesActiveSpecifiedTimeframe.Text = "Keep the rule active only within a specified timeframe";
            this._rbAlertRulesActiveSpecifiedTimeframe.Click += new System.EventHandler(this.Click_AlertRulesActiveButton);
            // 
            // _lblAlertRuleActiveStartTime
            // 
            this._lblAlertRuleActiveStartTime.Location = new System.Drawing.Point(4, 16);
            this._lblAlertRuleActiveStartTime.Name = "_lblAlertRuleActiveStartTime";
            this._lblAlertRuleActiveStartTime.Size = new System.Drawing.Size(48, 16);
            this._lblAlertRuleActiveStartTime.TabIndex = 10;
            this._lblAlertRuleActiveStartTime.Text = "Start:";
            this._lblAlertRuleActiveStartTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // _alertRuleActiveStartTime
            //
            this._alertRuleActiveStartTime.Location = new System.Drawing.Point(64, 16);
            this._alertRuleActiveStartTime.Name = "_alertRuleActiveStartTime";
            this._alertRuleActiveStartTime.Size = new System.Drawing.Size(96, 16);
            this._alertRuleActiveStartTime.TabIndex = 15;
            this._alertRuleActiveStartTime.CustomFormat = "hh:mm:ss tt";
            this._alertRuleActiveStartTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this._alertRuleActiveStartTime.ShowUpDown = true;
            this._alertRuleActiveStartTime.Value = System.DateTime.ParseExact("00:00:00", "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            this._alertRuleActiveStartTime.ValueChanged += new System.EventHandler(this.ValueChanged_AlertRulesActiveStartTime);
            // 
            // _lblAlertRuleActiveEndTime
            // 
            this._lblAlertRuleActiveEndTime.Location = new System.Drawing.Point(208, 16);
            this._lblAlertRuleActiveEndTime.Name = "_lblAlertRuleActiveEndTime";
            this._lblAlertRuleActiveEndTime.Size = new System.Drawing.Size(48, 16);
            this._lblAlertRuleActiveEndTime.TabIndex = 15;
            this._lblAlertRuleActiveEndTime.Text = "End:";
            this._lblAlertRuleActiveEndTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // _alertRuleActiveEndTime
            //
            this._alertRuleActiveEndTime.Location = new System.Drawing.Point(264, 16);
            this._alertRuleActiveEndTime.Name = "_alertRuleActiveEndTime";
            this._alertRuleActiveEndTime.Size = new System.Drawing.Size(96, 16);
            this._alertRuleActiveEndTime.TabIndex = 15;
            this._alertRuleActiveEndTime.CustomFormat = "hh:mm:ss tt";
            this._alertRuleActiveEndTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this._alertRuleActiveEndTime.ShowUpDown = true;
            this._alertRuleActiveEndTime.Value = System.DateTime.ParseExact("23:59:59", "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            this._alertRuleActiveEndTime.ValueChanged += new System.EventHandler(this.ValueChanged_AlertRulesActiveEndTime);
            // 
            // _lblAlertRuleActiveOn
            // 
            this._lblAlertRuleActiveOn.Location = new System.Drawing.Point(4, 44);
            this._lblAlertRuleActiveOn.Name = "_lblAlertRuleActiveOn";
            this._lblAlertRuleActiveOn.Size = new System.Drawing.Size(24, 16);
            this._lblAlertRuleActiveOn.TabIndex = 15;
            this._lblAlertRuleActiveOn.Text = "On";
            this._lblAlertRuleActiveOn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _chkAlertRuleActiveOnMon
            // 
            this._chkAlertRuleActiveOnMon.Location = new System.Drawing.Point(30, 44);
            this._chkAlertRuleActiveOnMon.Name = "_chkAlertRuleActiveOnMon";
            this._chkAlertRuleActiveOnMon.Size = new System.Drawing.Size(48, 16);
            this._chkAlertRuleActiveOnMon.Text = "Mon";
            this._chkAlertRuleActiveOnMon.CheckedChanged += new System.EventHandler(this.CheckedChanged_AlertRuleActiveDaysOfTheWeek);
            // 
            // _chkAlertRuleActiveOnTue
            // 
            this._chkAlertRuleActiveOnTue.Location = new System.Drawing.Point(83, 44);
            this._chkAlertRuleActiveOnTue.Name = "_chkAlertRuleActiveOnTue";
            this._chkAlertRuleActiveOnTue.Size = new System.Drawing.Size(48, 16);
            this._chkAlertRuleActiveOnTue.Text = "Tue";
            this._chkAlertRuleActiveOnTue.CheckedChanged += new System.EventHandler(this.CheckedChanged_AlertRuleActiveDaysOfTheWeek);
            // 
            // _chkAlertRuleActiveOnWed
            // 
            this._chkAlertRuleActiveOnWed.Location = new System.Drawing.Point(136, 44);
            this._chkAlertRuleActiveOnWed.Name = "_chkAlertRuleActiveOnWed";
            this._chkAlertRuleActiveOnWed.Size = new System.Drawing.Size(53, 16);
            this._chkAlertRuleActiveOnWed.Text = "Wed";
            this._chkAlertRuleActiveOnWed.CheckedChanged += new System.EventHandler(this.CheckedChanged_AlertRuleActiveDaysOfTheWeek);
            // 
            // _chkAlertRuleActiveOnThu
            // 
            this._chkAlertRuleActiveOnThu.Location = new System.Drawing.Point(194, 44);
            this._chkAlertRuleActiveOnThu.Name = "_chkAlertRuleActiveOnThu";
            this._chkAlertRuleActiveOnThu.Size = new System.Drawing.Size(48, 16);
            this._chkAlertRuleActiveOnThu.Text = "Thu";
            this._chkAlertRuleActiveOnThu.CheckedChanged += new System.EventHandler(this.CheckedChanged_AlertRuleActiveDaysOfTheWeek);
            // 
            // _chkAlertRuleActiveOnFri
            // 
            this._chkAlertRuleActiveOnFri.Location = new System.Drawing.Point(247, 44);
            this._chkAlertRuleActiveOnFri.Name = "_chkAlertRuleActiveOnFri";
            this._chkAlertRuleActiveOnFri.Size = new System.Drawing.Size(40, 16);
            this._chkAlertRuleActiveOnFri.Text = "Fri";
            this._chkAlertRuleActiveOnFri.CheckedChanged += new System.EventHandler(this.CheckedChanged_AlertRuleActiveDaysOfTheWeek);
            // 
            // _chkAlertRuleActiveOnSat
            // 
            this._chkAlertRuleActiveOnSat.Location = new System.Drawing.Point(288, 44);
            this._chkAlertRuleActiveOnSat.Name = "_chkAlertRuleActiveOnSat";
            this._chkAlertRuleActiveOnSat.Size = new System.Drawing.Size(48, 16);
            this._chkAlertRuleActiveOnSat.Text = "Sat";
            this._chkAlertRuleActiveOnSat.CheckedChanged += new System.EventHandler(this.CheckedChanged_AlertRuleActiveDaysOfTheWeek);
            // 
            // _chkAlertRuleActiveOnSun
            // 
            this._chkAlertRuleActiveOnSun.Location = new System.Drawing.Point(338, 44);
            this._chkAlertRuleActiveOnSun.Name = "_chkAlertRuleActiveOnSun";
            this._chkAlertRuleActiveOnSun.Size = new System.Drawing.Size(48, 16);
            this._chkAlertRuleActiveOnSun.Text = "Sun";
            this._chkAlertRuleActiveOnSun.CheckedChanged += new System.EventHandler(this.CheckedChanged_AlertRuleActiveDaysOfTheWeek);
            // 
            // _rtfPNewRuleDetails
            // 
            this._rtfPNewRuleDetails.Location = new System.Drawing.Point(16, 154);
            this._rtfPNewRuleDetails.Name = "_rtfPNewRuleDetails";
            this._rtfPNewRuleDetails.ReadOnly = true;
            this._rtfPNewRuleDetails.Size = new System.Drawing.Size(408, 113);
            this._rtfPNewRuleDetails.TabIndex = 18;
            this._rtfPNewRuleDetails.Text = "richTextBox1";
            this._rtfPNewRuleDetails.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MouseMove_rtfBox);
            this._rtfPNewRuleDetails.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MouseDown_rtfBox);
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
          this._btnCancel.Click += new System.EventHandler(this.Click_btnCancel);
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
          this._btnFinish.Click += new System.EventHandler(this.Click_btnFinish);
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
          // Form_AlertRuleWizard
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
          this.Name = "Form_AlertRuleWizard";
          this.ShowInTaskbar = false;
          this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
          this.Text = "New Event Alert Rule";
          this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.Form_AlertRuleWizard_HelpRequested);
          this._pnlLeft.ResumeLayout(false);
          ((System.ComponentModel.ISupportInitialize)(this._pictureBox)).EndInit();
          this._pnlCenter.ResumeLayout(false);
          this._pnlSummary.ResumeLayout(false);
          this._pnlSummary.PerformLayout();
          this._pnlActions.ResumeLayout(false);
          this._pnlAdditionalFilters.ResumeLayout(false);
            this._pnlAlertRuleTimeFrame.ResumeLayout(false);
          this._pnlTargetObjects.ResumeLayout(false);
          this._pnlVerb.ResumeLayout(false);
          this.groupBox1.ResumeLayout(false);
          this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
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
      private System.Windows.Forms.Label _lblP2S2RuleDetails;
      private System.Windows.Forms.Label _lblP2S1AlertConditions;
      private System.Windows.Forms.CheckedListBox _listBoxAdditionalFilters;
      private System.Windows.Forms.Label _lblBorder1;
      private System.Windows.Forms.CheckedListBox _listBoxP3AlertActions;
      private System.Windows.Forms.Label _lblP3S2RuleDetails;
      private System.Windows.Forms.Label _lblP3S1AlertActions;
      private System.Windows.Forms.Label _lblP4S3RuleDetails;
      private System.Windows.Forms.Label _lblP4S1NameRule;
        private System.Windows.Forms.Label _lblAlertRuleActiveStartTime;
        private System.Windows.Forms.Label _lblAlertRuleActiveEndTime;
        private System.Windows.Forms.Label _lblAlertRuleActiveOn;
        private System.Windows.Forms.TextBox _tbP4RuleName;
      private System.Windows.Forms.Label _lblP4S2RuleOptions;
      private System.Windows.Forms.ComboBox _comboP4AlertLevel;
      private System.Windows.Forms.CheckBox _checkBoxP4EnableRule;
      private System.Windows.Forms.RichTextBox _rtfP4RuleDetails;
      private System.Windows.Forms.RichTextBox _rtfP3RuleDetails;
      private System.Windows.Forms.Label _lblP4RuleDescription;
      private System.Windows.Forms.TextBox _tbP4RuleDescription;
      private System.Windows.Forms.Panel _pnlVerb;
      private System.Windows.Forms.GroupBox groupBox1;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.Panel _pnlTargetObjects;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.Panel _pnlActions;
      private System.Windows.Forms.RadioButton _rbAdministrative;
      private System.Windows.Forms.RadioButton _rbDDL;
      private System.Windows.Forms.RadioButton _rbSecurity;
      private System.Windows.Forms.RadioButton _rbDML;
      private System.Windows.Forms.RadioButton _rbLogins;
      private System.Windows.Forms.RichTextBox _rtfVerb;
      private System.Windows.Forms.Panel _pnlAdditionalFilters;
      private System.Windows.Forms.RichTextBox _rtfAdditionalFilters;
      private System.Windows.Forms.RadioButton _rbUserDefined;
      private System.Windows.Forms.RadioButton _rbSpecificEvent;
      private System.Windows.Forms.LinkLabel _linkSpecificEvent;
      private System.Windows.Forms.RichTextBox _rtfTargetObjects;
      private System.Windows.Forms.CheckedListBox _listBoxTargetObjects;
      private System.Windows.Forms.Panel _pnlSummary;
        private System.Windows.Forms.Panel _pnlAlertRuleTimeFrame;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton _rbAlertRuleActiveAllTimes;
        private System.Windows.Forms.RadioButton _rbAlertRulesActiveSpecifiedTimeframe;
        private System.Windows.Forms.DateTimePicker _alertRuleActiveStartTime;
        private System.Windows.Forms.DateTimePicker _alertRuleActiveEndTime;
        private System.Windows.Forms.CheckBox _chkAlertRuleActiveOnMon;
        private System.Windows.Forms.CheckBox _chkAlertRuleActiveOnTue;
        private System.Windows.Forms.CheckBox _chkAlertRuleActiveOnWed;
        private System.Windows.Forms.CheckBox _chkAlertRuleActiveOnThu;
        private System.Windows.Forms.CheckBox _chkAlertRuleActiveOnFri;
        private System.Windows.Forms.CheckBox _chkAlertRuleActiveOnSat;
        private System.Windows.Forms.CheckBox _chkAlertRuleActiveOnSun;
        private System.Windows.Forms.Label _lblEmailSummaryNotification;
        private System.Windows.Forms.NumericUpDown _txtEmailSummaryIntervalHours;
        private System.Windows.Forms.Label _lblEmailSummaryIntervalHours;
        private System.Windows.Forms.NumericUpDown _txtEmailSummaryIntervalMinutes;
        private System.Windows.Forms.Label _lblEmailSummaryIntervalMinutes;
        private System.Windows.Forms.RichTextBox _rtfPNewRuleDetails;
    }
}