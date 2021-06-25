using System;
using System.Drawing;
using System.Windows.Forms;
using Idera.SQLcompliance.Application.GUI.Properties ;
using Idera.SQLcompliance.Core.Rules;
using Idera.SQLcompliance.Core.Rules.Alerts ;
using Idera.SQLcompliance.Core ;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
    /// <summary>
    /// Summary description for Form_AlertRuleWizard.
    /// </summary>
    public partial class Form_AlertRuleWizard : System.Windows.Forms.Form
    {
        private static readonly string[] _wizardTitles = new string[] { "SQL Server Event Type",
                                                                       "SQL Server Object Type",
                                                                       "Additional Event Filters",
                                                                       "Alert Rule Timeframe",
                                                                       "Alert Actions",
                                                                       "Finish Alert Rule"};
        private static readonly string[] _wizardDescriptions = new string[] { "Select the SQL Server event you want to monitor.",
                                                                             "Select the SQL Server, database, object, or host you want to monitor.",
                                                                             "Specify when the selected event should trigger this alert.",
                                                                             "Specify the timeframes for applying the event rule. When not specified, the rule will always be active",
                                                                             "Select the action to be taken for events that match this rule.",
                                                                             "Specify a name for this rule and select the categorization level for the alert.  Also, select whether to enable this rule now."};

        private int _pageIndex;
        private int _pageCount;
        private AlertRule _rule;
        private LinkString _link;
        private Graphics _rtfGraphics;
        private AlertingConfiguration _configuration;
        private bool _isInternalUpdate;
        private EventCondition _verbCondition;
        private EventCondition _verbConditionSecurity;
        private EventCondition _verbConditionAdministrative;
        private EventCondition _verbConditionLogin;
        private EventCondition _verbConditionDDL;
        private EventCondition _verbConditionDML;
        private EventCondition _verbConditionUserDefined;
        private EventCondition _verbConditionSpecificEvent;
        private CheckedListEventField _swingField;
        private bool _isEdit;

        public Form_AlertRuleWizard(AlertingConfiguration configuration) : this(null, configuration)
        {
        }


        public Form_AlertRuleWizard(AlertRule rule, AlertingConfiguration configuration)
        {
            InitializeComponent();

            this.Icon = Resources.SQLcompliance_product_ico;
            _configuration = configuration;

            Initialize();

            _pageIndex = 0;
            _pageCount = 6;

            if (rule == null)
            {
                Text = "New Event Alert Rule";
                _rule = new AlertRule();
                _rule.Enabled = true;
                _rule.AddCondition(_verbConditionSecurity);
                _isEdit = false;
                _rule.AlertRuleTimeFrameStartTime = "00:00:00";
                _rule.AlertRuleTimeFrameEndTime = "23:59:59";
                _rule.AlertRuleTimeFrameDaysOfWeek = "";
                _rule.SummaryEmailFrequency = 30;
            }
            else
            {
                _rule = rule;
                foreach (EventCondition condition in _rule.Conditions)
                {
                    if (condition.TargetEventField.Id == (int)AlertableEventFields.eventType)
                    {
                        _linkSpecificEvent.Enabled = true;
                        _verbConditionSpecificEvent = condition;
                    }
                }
                if (rule.Id == AlertingConfiguration.NULL_ID)
                {
                    // Creating a rule from a template
                    Text = "New Event Alert Rule";
                    _isEdit = false;
                }
                else
                {
                    Text = "Edit Event Alert Rule";
                    _isEdit = true;
                    _btnFinish.Enabled = Globals.isAdmin;
                }
            }

            //_verbCondition = _verbConditionSecurity ;
            _lblTitle.Text = _wizardTitles[_pageIndex];
            _lblDescription.Text = _wizardDescriptions[_pageIndex];
            UpdateData(_pageIndex);
            // Now that we are initialized and updated, do we need the swing field?
            if (!_rbSpecificEvent.Checked)
                _listBoxAdditionalFilters.Items.Add(_swingField);

            _rtfGraphics = _rtfVerb.CreateGraphics();
        }

        private void Initialize()
        {
            try
            {
                EventField eventCategoryField = _configuration.LookupAlertableEventField(AlertableEventFields.eventCategory);
                EventField eventTypeField = _configuration.LookupAlertableEventField(AlertableEventFields.eventType);

                _verbConditionSecurity = new EventCondition(eventCategoryField);
                _verbConditionSecurity.IntegerArray = new int[] { 3 };

                _verbConditionAdministrative = new EventCondition(eventCategoryField);
                _verbConditionAdministrative.IntegerArray = new int[] { 6 };

                _verbConditionLogin = new EventCondition(eventCategoryField);
                _verbConditionLogin.IntegerArray = new int[] { 1 };

                _verbConditionDML = new EventCondition(eventCategoryField);
                _verbConditionDML.IntegerArray = new int[] { 4 };

                _verbConditionDDL = new EventCondition(eventCategoryField);
                _verbConditionDDL.IntegerArray = new int[] { 2 };

                _verbConditionUserDefined = new EventCondition(eventCategoryField);
                _verbConditionUserDefined.IntegerArray = new int[] { 9 };

                _verbConditionSpecificEvent = new EventCondition(eventTypeField);
                _verbConditionSpecificEvent.IntegerArray = new int[] { 101 };
                CMEventType evType = _configuration.LookupEventType(101, EventType.SqlServer);
                _linkSpecificEvent.Text = evType.Name;

                CheckedListEventField field;

                // Scope page
                field = new CheckedListEventField();
                field.Field = _configuration.LookupAlertableEventField(AlertableEventFields.serverName);
                field.ListEntry = "SQL Server";
                _listBoxTargetObjects.Items.Add(field);

                field = new CheckedListEventField();
                field.Field = _configuration.LookupAlertableEventField(AlertableEventFields.databaseName);
                field.ListEntry = "Database Name";
                _listBoxTargetObjects.Items.Add(field);

                field = new CheckedListEventField();
                field.Field = _configuration.LookupAlertableEventField(AlertableEventFields.objectName);
                field.ListEntry = "Object Name (including Table)";
                _listBoxTargetObjects.Items.Add(field);

                field = new CheckedListEventField();
                field.Field = _configuration.LookupAlertableEventField(AlertableEventFields.hostName);
                field.ListEntry = "Host Name";
                _listBoxTargetObjects.Items.Add(field);

                // Filters page
                field = new CheckedListEventField();
                field.Field = _configuration.LookupAlertableEventField(AlertableEventFields.applicationName);
                field.ListEntry = "Application Name";
                _listBoxAdditionalFilters.Items.Add(field);

                field = new CheckedListEventField();
                field.Field = _configuration.LookupAlertableEventField(AlertableEventFields.loginName);
                field.ListEntry = "Login Name";
                _listBoxAdditionalFilters.Items.Add(field);

                field = new CheckedListEventField();
                field.Field = _configuration.LookupAlertableEventField(AlertableEventFields.success);
                field.ListEntry = "Access Check Passed";
                _listBoxAdditionalFilters.Items.Add(field);

                field = new CheckedListEventField();
                field.Field = _configuration.LookupAlertableEventField(AlertableEventFields.privilegedUser);
                field.ListEntry = "Is Privileged User";
                _listBoxAdditionalFilters.Items.Add(field);

                _swingField = new CheckedListEventField();
                _swingField.Field = _configuration.LookupAlertableEventField(AlertableEventFields.eventType);
                _swingField.ListEntry = "Exclude certain event types";

                field = new CheckedListEventField();
                field.Field = _configuration.LookupAlertableEventField(AlertableEventFields.privilegedUsers);
                field.ListEntry = "Privileged Users";
                _listBoxAdditionalFilters.Items.Add(field);

                field = new CheckedListEventField();
                field.Field = _configuration.LookupAlertableEventField(AlertableEventFields.rowCount);
                field.ListEntry = "Row Count (with Time Interval)";
                _listBoxAdditionalFilters.Items.Add(field);

                _listBoxP3AlertActions.Items.Add("Email Notification");
                _listBoxP3AlertActions.Items.Add("Email Summary Notification");
                _listBoxP3AlertActions.Items.Add("Event Log Entry");
                _listBoxP3AlertActions.Items.Add("SNMP Trap");
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write("AlertWizard:Initialize", e);
            }
        }

        public AlertRule Rule
        {
            get { return _rule; }
        }

        public string RuleText
        {
            get
            {
                RichTextBox temp = GetActiveRtf();
                if (temp != null)
                    return temp.Text;
                else
                    return "Rule text is currently not available.";
            }
        }

        private void ShowPage(int pageNumber)
        {
            if (pageNumber < 0 || pageNumber >= _pageCount)
            {
                throw new Exception("Invalid page number");
            }
            if (pageNumber == _pageIndex)
                return;

            if (pageNumber == 0)
            {
                _btnBack.Enabled = false;
                _btnNext.Enabled = true;
                _btnFinish.Enabled = _isEdit && Globals.isAdmin;
            }
            else if (pageNumber == (_pageCount - 1))
            {
                if (_pageCount > 1)
                    _btnBack.Enabled = true;
                _btnFinish.Enabled = Globals.isAdmin;
                _btnNext.Enabled = false;
            }
            else
            {
                _btnBack.Enabled = true;
                _btnNext.Enabled = true;
                _btnFinish.Enabled = _isEdit && Globals.isAdmin;
            }

            switch (pageNumber)
            {
                case 0:
                    UpdateData(0);
                    _pnlVerb.Show();
                    break;
                case 1:
                    UpdateData(1);
                    _pnlTargetObjects.Show();
                    break;
                case 2:
                    UpdateData(2);
                    _pnlAdditionalFilters.Show();
                    break;
                case 3:
                    UpdateData(3);
                    _pnlAlertRuleTimeFrame.Show();
                    break;
                case 4:
                    if (_rbAlertRulesActiveSpecifiedTimeframe.Checked
                            &&
                        !_chkAlertRuleActiveOnMon.Checked
                            &&
                        !_chkAlertRuleActiveOnTue.Checked
                            &&
                        !_chkAlertRuleActiveOnWed.Checked
                            &&
                        !_chkAlertRuleActiveOnThu.Checked
                            &&
                        !_chkAlertRuleActiveOnFri.Checked
                            &&
                        !_chkAlertRuleActiveOnSat.Checked
                            &&
                        !_chkAlertRuleActiveOnSun.Checked)
                    {
                        MessageBox.Show("Please select at least one day before continuing", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    UpdateData(4);
                    _pnlActions.Show();
                    break;
                case 5:
                    UpdateData(5);
                    _pnlSummary.Show();
                    break;
            }

            _lblTitle.Text = _wizardTitles[pageNumber];
            _lblDescription.Text = _wizardDescriptions[pageNumber];

            switch (_pageIndex)
            {
                case 0:
                    _pnlVerb.Hide();
                    break;
                case 1:
                    _pnlTargetObjects.Hide();
                    break;
                case 2:
                    _pnlAdditionalFilters.Hide();
                    break;
                case 3:
                    _pnlAlertRuleTimeFrame.Hide();
                    break;
                case 4:
                    _pnlActions.Hide();
                    break;
                case 5:
                    _pnlSummary.Hide();
                    break;
            }
            _pageIndex = pageNumber;
            if (_pageIndex == (_pageCount - 1))
            {
                if (Globals.isAdmin)
                    _btnFinish.Focus();
                else
                    _btnCancel.Focus();
            }
            else
                _btnNext.Focus();
        }

        /// <summary>
        /// This function updates context-sensitive UI components in the wizard
        /// based upon the information availabled in the AlertRule object.
        /// </summary>
        /// <param name="page"></param>
        private void UpdateData(int page)
        {
            _isInternalUpdate = true;
            try
            {
                switch (page)
                {
                    case 0:
                        foreach (EventCondition condition in _rule.Conditions)
                        {
                            if (condition.FieldId == (int)AlertableEventFields.eventCategory)
                            {
                                int[] values = condition.IntegerArray;
                                if (values != null && values.Length > 0)
                                {
                                    switch (values[0])
                                    {
                                        case 2:
                                            _verbCondition = _verbConditionDDL;
                                            _rbDDL.Checked = true;
                                            break;
                                        case 3:
                                            _verbCondition = _verbConditionSecurity;
                                            _rbSecurity.Checked = true;
                                            break;
                                        case 4:
                                            _verbCondition = _verbConditionDML;
                                            _rbDML.Checked = true;
                                            break;
                                        case 6:
                                            _verbCondition = _verbConditionAdministrative;
                                            _rbAdministrative.Checked = true;
                                            break;
                                        case 9:
                                            _verbCondition = _verbConditionUserDefined;
                                            _rbUserDefined.Checked = true;
                                            break;
                                    }
                                }
                            }
                            else if (condition.FieldId == (int)AlertableEventFields.eventType && condition.Inclusive)
                            {
                                int[] values = condition.IntegerArray;
                                if (values != null && values.Length > 0)
                                {
                                    _rbSpecificEvent.Checked = true;
                                    CMEventType evType = _configuration.LookupEventType(condition.IntegerArray[0], EventType.SqlServer);
                                    _linkSpecificEvent.Text = evType.Name;
                                    _verbCondition = _verbConditionSpecificEvent;
                                }
                            }
                        }
                        break;
                    case 1:
                        _listBoxTargetObjects.SetItemChecked(0, false);
                        _listBoxTargetObjects.SetItemChecked(1, false);
                        _listBoxTargetObjects.SetItemChecked(2, false);
                        _listBoxTargetObjects.SetItemChecked(3, false);

                        if (_rule.TargetInstances.Length == 0 ||
                           (_rule.TargetInstances.Length >= 1 && _rule.TargetInstances[0] != "<ALL>"))
                            _listBoxTargetObjects.SetItemChecked(0, true);
                        foreach (EventCondition condition in _rule.Conditions)
                        {
                            switch (condition.FieldId)
                            {
                                case (int)AlertableEventFields.databaseName:
                                    _listBoxTargetObjects.SetItemChecked(1, true);
                                    break;
                                case (int)AlertableEventFields.objectName:
                                    _listBoxTargetObjects.SetItemChecked(2, true);
                                    break;
                                case (int)AlertableEventFields.hostName:
                                    _listBoxTargetObjects.SetItemChecked(3, true);
                                    break;
                            }
                        }
                        break;
                    case 2:
                        _listBoxAdditionalFilters.SetItemChecked(0, false);
                        _listBoxAdditionalFilters.SetItemChecked(1, false);
                        _listBoxAdditionalFilters.SetItemChecked(2, false);
                        _listBoxAdditionalFilters.SetItemChecked(3, false);
                        _listBoxAdditionalFilters.SetItemChecked(4, false);
                        _listBoxAdditionalFilters.SetItemChecked(5, false);
                        if (_listBoxAdditionalFilters.Items.Count == 7)
                            _listBoxAdditionalFilters.SetItemChecked(6, false);




                        foreach (EventCondition condition in _rule.Conditions)
                        {
                            switch (condition.FieldId)
                            {
                                case (int)AlertableEventFields.applicationName:
                                    _listBoxAdditionalFilters.SetItemChecked(0, true);
                                    break;
                                case (int)AlertableEventFields.loginName:
                                    _listBoxAdditionalFilters.SetItemChecked(1, true);
                                    break;
                                case (int)AlertableEventFields.success:
                                    _listBoxAdditionalFilters.SetItemChecked(2, true);
                                    break;
                                case (int)AlertableEventFields.privilegedUser:
                                    _listBoxAdditionalFilters.SetItemChecked(3, true);
                                    break;
                                case (int)AlertableEventFields.privilegedUsers:
                                    _listBoxAdditionalFilters.SetItemChecked(4, true);
                                    break;
                                case (int)AlertableEventFields.rowCount:
                                    _listBoxAdditionalFilters.SetItemChecked(5, true);
                                    break;
                                case (int)AlertableEventFields.eventType:
                                    if (_listBoxAdditionalFilters.Items.Count == 7)
                                        _listBoxAdditionalFilters.SetItemChecked(6, true);
                                    break;


                            }
                        }
                        break;
                    case 3:
                        if (_rule.IsAlertRuleTimeFrameEnabled)
                        {
                            groupBox3.Enabled = true;
                            _rbAlertRulesActiveSpecifiedTimeframe.Checked = true;
                            _alertRuleActiveStartTime.Value = Convert.ToDateTime(_rule.AlertRuleTimeFrameStartTime);
                            _alertRuleActiveEndTime.Value = Convert.ToDateTime(_rule.AlertRuleTimeFrameEndTime);
                            foreach(char ch in _rule.AlertRuleTimeFrameDaysOfWeek)
                            {
                                switch (ch)
                                {
                                    case '1':
                                        _chkAlertRuleActiveOnSun.Checked = true;
                                        break;
                                    case '2':
                                        _chkAlertRuleActiveOnMon.Checked = true;
                                        break;
                                    case '3':
                                        _chkAlertRuleActiveOnTue.Checked = true;
                                        break;
                                    case '4':
                                        _chkAlertRuleActiveOnWed.Checked = true;
                                        break;
                                    case '5':
                                        _chkAlertRuleActiveOnThu.Checked = true;
                                        break;
                                    case '6':
                                        _chkAlertRuleActiveOnFri.Checked = true;
                                        break;
                                    case '7':
                                        _chkAlertRuleActiveOnSat.Checked = true;
                                        break;
                                }
                            }
                        }
                        else
                        {
                            _rbAlertRuleActiveAllTimes.Checked = true;
                        }
                        break;
                    case 4:
                        int summaryEmailFrequency = _rule.SummaryEmailFrequency;
                        _listBoxP3AlertActions.SetItemChecked(0, _rule.HasEmailAction);
                        _listBoxP3AlertActions.SetItemChecked(1, _rule.HasEmailSummaryAction);
                        _listBoxP3AlertActions.SetItemChecked(2, _rule.HasLogAction);
                        _listBoxP3AlertActions.SetItemChecked(3, _rule.HasSnmpTrapAction);
                        if (summaryEmailFrequency == 0)
                        {
                            _txtEmailSummaryIntervalMinutes.Value = 30;
                            _txtEmailSummaryIntervalHours.Value = 0;
                            _rule.SummaryEmailFrequency += (Convert.ToInt32(Math.Round(_txtEmailSummaryIntervalHours.Value, 0)) * 60) + Convert.ToInt32(Math.Round(_txtEmailSummaryIntervalMinutes.Value, 0));
                            _rule.LastEmailSummarySendTime = "UpdateOldRule";
                        }
                        else
                        {
                            _txtEmailSummaryIntervalMinutes.Value = summaryEmailFrequency % 60;
                            _txtEmailSummaryIntervalHours.Value = summaryEmailFrequency / 60;
                        }
                        if (_rule.HasEmailSummaryAction)
                            groupBox4.Enabled = true;
                        break;
                    case 5:
                        _tbP4RuleName.Text = _rule.Name;
                        _tbP4RuleDescription.Text = _rule.Description;
                        _comboP4AlertLevel.SelectedIndex = ((int)_rule.Level) - 1;
                        _checkBoxP4EnableRule.Checked = _rule.Enabled;
                        break;
                }
                UpdateRuleDescription(page);
            }
            finally
            {
                _isInternalUpdate = false;
            }
        }

        private void UpdateRuleDescription(int page)
        {
            RichTextBox rtf = GetActiveRtf(page);

            if (rtf == null)
                return;

            try
            {
                rtf.Rtf = AlertUIHelper.GenerateRuleDescription(_rule, _configuration, out _link);
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write("UpdateRuleDescription", e);
            }
        }

        #region Events

        private void Click_btnBack(object sender, System.EventArgs e)
        {
            ShowPage(_pageIndex - 1);
        }

        private void Click_btnFinish(object sender, System.EventArgs e)
        {
            if (!_rule.HasEmailAction && !_rule.HasEmailSummaryAction)
                _rule.Recipients = new string[0];
        }

        private void Click_btnCancel(object sender, System.EventArgs e)
        {
        }

        private void Click_btnNext(object sender, System.EventArgs e)
        {
            ShowPage(_pageIndex + 1);
        }

        private void ItemCheck_listBoxAdditionalFilters(object sender, System.Windows.Forms.ItemCheckEventArgs e)
        {
            if (_isInternalUpdate)
                return;
            CheckedListEventField info = (CheckedListEventField)_listBoxAdditionalFilters.Items[e.Index];

            if (e.NewValue == CheckState.Unchecked)
                _rule.RemoveCondition(info.Field.ColumnName);
            else if (e.NewValue == CheckState.Checked)
            {
                EventCondition newCondition = new EventCondition(info.Field);
                if (info.Field.Id == (int)AlertableEventFields.eventType)
                    newCondition.Inclusive = false;
                if (info.Field.Id == (int)AlertableEventFields.success)
                    newCondition.BooleanValue = true;
                _rule.AddCondition(newCondition);
            }
            UpdateRuleDescription(_pageIndex);
        }

        private void ItemCheck_listBoxP3AlertActions(object sender, System.Windows.Forms.ItemCheckEventArgs e)
        {
            if (_isInternalUpdate)
                return;

            if (e.Index == 0)
            {
                _rule.HasEmailAction = e.NewValue == CheckState.Checked;
                _listBoxP3AlertActions.SetItemChecked(1, false);
                groupBox4.Enabled = false;
                _txtEmailSummaryIntervalHours.Value = 0;
                _txtEmailSummaryIntervalMinutes.Value = 30;
            }
            if (e.Index == 1)
            {
                _rule.HasEmailSummaryAction = e.NewValue == CheckState.Checked;
                if (e.NewValue == CheckState.Checked)
                {
                    _listBoxP3AlertActions.SetItemChecked(0, false);
                    groupBox4.Enabled = true;
                }
                else
                {
                    _txtEmailSummaryIntervalHours.Value = 0;
                    _txtEmailSummaryIntervalMinutes.Value = 30;
                    groupBox4.Enabled = false;
                }
            }
            else if (e.Index == 2)
                _rule.HasLogAction = e.NewValue == CheckState.Checked;
            else if (e.Index == 3)
                _rule.HasSnmpTrapAction = e.NewValue == CheckState.Checked;

            UpdateRuleDescription(4);
        }

        private void SelectedIndexChanged_comboP4AlertLevel(object sender, System.EventArgs e)
        {
            if (_isInternalUpdate)
                return;
            _rule.Level = (AlertLevel)(_comboP4AlertLevel.SelectedIndex + 1);
            UpdateRuleDescription(4);
        }

        private void TextChanged_tbP4RuleName(object sender, System.EventArgs e)
        {
            if (_isInternalUpdate)
                return;
            _rule.Name = _tbP4RuleName.Text;
            UpdateRuleDescription(_pageIndex);
        }

        private void MouseMove_rtfBox(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            RichTextBox rtf = GetActiveRtf();

            if (rtf == null || !Globals.isAdmin)
                return;

            LinkSegment seg = _link.LinkHitTest(e.X, e.Y, rtf, _rtfGraphics);
            if (seg != null && seg.Tag != null)
            {
                rtf.Cursor = Cursors.Hand;
                Cursor.Current = Cursors.Hand;
            }
            else
            {
                rtf.Cursor = Cursors.Default;
                Cursor.Current = Cursors.Default;
            }
        }

        private void MouseDown_rtfBox(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            LinkSegment link = _link.LinkHitTest(e.X, e.Y, sender as RichTextBox, _rtfGraphics);

            if (link == null || link.Tag == null || e.Button != MouseButtons.Left || !Globals.isAdmin)
                return;
            else if (link.Tag is string)
            {
                string tagString = (string)link.Tag;

                if (String.Equals(tagString, "TargetInstances"))
                {
                    Form_InstanceList frmInstances = new Form_InstanceList();
                    frmInstances.SelectedInstances = _rule.TargetInstances;
                    if (frmInstances.ShowDialog(this) == DialogResult.OK)
                    {
                        _rule.TargetInstances = frmInstances.SelectedInstances;
                        UpdateRuleDescription(_pageIndex);
                    }
                }
                else if (String.Equals(tagString, "AlertLevel"))
                {
                    Form_AlertLevel frmLevel = new Form_AlertLevel();
                    frmLevel.Level = _rule.Level;
                    if (frmLevel.ShowDialog(this) == DialogResult.OK)
                    {
                        _rule.Level = frmLevel.Level;
                        UpdateRuleDescription(_pageIndex);
                    }
                }
                else if (String.Equals(tagString, "AlertMessage"))
                {
                    Form_EmailTemplate options;

                    options = new Form_EmailTemplate(_configuration.SqlServerMacros);
                    options.Subject = _rule.MessageTitle;
                    options.Message = _rule.MessageBody;
                    if (options.ShowDialog(this) == DialogResult.OK)
                    {
                        _rule.MessageTitle = options.Subject;
                        _rule.MessageBody = options.Message;
                    }
                }
                else if (String.Equals(tagString, "Recipients"))
                {
                    Form_EmailList emailForm = new Form_EmailList();
                    emailForm.EmailArray = _rule.Recipients;
                    if (emailForm.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                    {
                        _rule.Recipients = emailForm.EmailArray;
                        UpdateRuleDescription(_pageIndex);
                    }
                }
                else if (String.Equals(tagString, "LogEntryType"))
                {
                    Form_EventLogType logTypeForm = new Form_EventLogType();

                    logTypeForm.EntryType = _rule.LogEntryType;
                    if (logTypeForm.ShowDialog(this) == DialogResult.OK)
                    {
                        _rule.LogEntryType = logTypeForm.EntryType;
                        UpdateRuleDescription(_pageIndex);
                    }
                }
                else if (String.Equals(tagString, "SnmpConfigurations"))
                {
                    // set default SNMP configurations if missing
                    if (_rule.SnmpConfiguration == null)
                        _rule.SnmpConfiguration = AlertingDal.SelectSnmpConfiguration(Globals.Repository.Connection.ConnectionString);

                    Form_AlertingOptions_Snmp frmSnmpConfigurations = new Form_AlertingOptions_Snmp(_rule.SnmpConfiguration);
                    if (frmSnmpConfigurations.ShowDialog(this) == DialogResult.OK)
                    {
                        _rule.SnmpConfiguration = frmSnmpConfigurations.SnmpConfiguration;
                        UpdateRuleDescription(_pageIndex);
                    }
                }

            }
            else if (link.Tag is EventCondition)
            {
                EventCondition condition = (EventCondition)link.Tag;

                switch (condition.ConditionType)
                {
                    case MatchType.Bool:
                        Form_Boolean boolForm = new Form_Boolean();
                        boolForm.BooleanValue = condition.BooleanValue;
                        boolForm.Directions = "&Select the desired value:";
                        boolForm.Title = condition.TargetEventField.DisplayName;
                        if (boolForm.ShowDialog(this) == DialogResult.OK)
                        {
                            condition.BooleanValue = boolForm.BooleanValue;
                            UpdateRuleDescription(_pageIndex);
                        }
                        break;
                    /*
                 case MatchType.Date:
                    Form_TimeSpan tsForm = new Form_TimeSpan() ;

                    tsForm.StartTime = condition.StartTime ;
                    tsForm.Duration = condition.Duration ;
                    tsForm.DaysOfWeek = condition.DaysOfWeek ;
                    tsForm.TimeZoneName = condition.TimeZoneName ;

                    if(tsForm.ShowDialog(this) == DialogResult.OK)
                    {
                       condition.StartTime = tsForm.StartTime ;
                       condition.Duration = tsForm.Duration ;
                       condition.DaysOfWeek = tsForm.DaysOfWeek ;
                       condition.TimeZoneName = tsForm.TimeZoneName ;
                       UpdateRuleDescription(_currentPage) ;
                    }
                    break ;
                    */
                    case MatchType.Integer:
                        if (condition.Inclusive)
                        {
                            SelectEventType();
                        }
                        else
                        {
                            Form_EventTypeList evForm;
                            CMEventCategory[] evCategories;

                            if (_verbCondition == null)
                            {
                                evCategories = _configuration.SqlServerCategories;
                            }
                            else
                            {
                                evCategories = new CMEventCategory[_verbCondition.IntegerArray.Length];

                                for (int i = 0; i < _verbCondition.IntegerArray.Length; i++)
                                    evCategories[i] = _configuration.LookupCategory(_verbCondition.IntegerArray[i]);
                            }

                            evForm = new Form_EventTypeList(evCategories);
                            evForm.SelectedEventIds = condition.IntegerArray;
                            if (evForm.ShowDialog(this) == DialogResult.OK)
                            {
                                condition.IntegerArray = evForm.SelectedEventIds;
                                UpdateRuleDescription(_pageIndex);
                            }
                        }
                        break;
                    case MatchType.String:


                        if (condition.FieldId == (int)AlertableEventFields.rowCount)
                        {
                            Form_RowCounts rcForm = new Form_RowCounts(condition.FieldId, true);
                            rcForm.IntegerRowcount = condition.IntegerRowcount;
                            rcForm.CbOprtr = condition.CbOprtr;
                            rcForm.IntegerTimeFrame = condition.IntegerTimeFrame;
                            if (rcForm.ShowDialog(this) == DialogResult.OK)
                            {
                                condition.CbOprtr = rcForm.CbOprtr;
                                condition.IntegerRowcount = rcForm.IntegerRowcount;
                                condition.IntegerTimeFrame = rcForm.IntegerTimeFrame;
                                UpdateRuleDescription(_pageIndex);
                            }

                            break;
                        }

                        Form_StringSearch ssForm = new Form_StringSearch(condition.FieldId, true);
                        ssForm.StringArray = condition.StringArray;
                        ssForm.Inclusive = condition.Inclusive;
                        if (ssForm.ShowDialog(this) == DialogResult.OK)
                        {
                            condition.StringArray = ssForm.StringArray;
                            condition.Inclusive = ssForm.Inclusive;
                            UpdateRuleDescription(_pageIndex);
                        }
                        break;
                }
            }
        }

        private void TextChanged_tbP4RuleDescription(object sender, System.EventArgs e)
        {
            if (_isInternalUpdate)
                return;
            _rule.Description = _tbP4RuleDescription.Text;
            UpdateRuleDescription(_pageIndex);
        }

        private void CheckedChanged_cbP4EnableRule(object sender, System.EventArgs e)
        {
            if (_isInternalUpdate)
                return;
            if (_checkBoxP4EnableRule.Checked)
                _rule.Enabled = true;
            else
                _rule.Enabled = false;
            //UpdateRuleDescription() ;
        }



        #endregion // Events

        private RichTextBox GetActiveRtf()
        {
            return GetActiveRtf(_pageIndex);
        }

        private void LinkClicked_linkSpecificEvent(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            SelectEventType();
        }

        private bool SelectEventType()
        {
            CMEventType selectedEvent = _configuration.LookupEventType(_verbConditionSpecificEvent.IntegerArray[0], EventType.SqlServer);
            CMEventCategory selectedCateogry = _configuration.LookupCategory(selectedEvent.CategoryId);
            Form_EventSelector selectorForm = new Form_EventSelector(_configuration.SqlServerCategories, selectedCateogry, selectedEvent);
            if (selectorForm.ShowDialog(this) == DialogResult.OK)
            {
                selectedEvent = selectorForm.SelectedEvent;
                int[] evEventType = new int[1];
                evEventType[0] = selectedEvent.TypeId;
                if (evEventType[0] != _verbConditionSpecificEvent.IntegerArray[0])
                    _verbConditionSpecificEvent.IntegerArray = evEventType;
                _linkSpecificEvent.Text = selectedEvent.Name;
                UpdateRuleDescription(_pageIndex);
                return true;
            }
            return false;
        }


        private void Form_AlertRuleWizard_HelpRequested(object sender, System.Windows.Forms.HelpEventArgs hlpevent)
        {
            string helpTopic = "";

            if (!_isEdit)
            {
                switch (_pageIndex)
                {
                    case 0:
                        helpTopic = HelpAlias.SSHELP_NewAlertWizard_EventType;
                        break;
                    case 1:
                        helpTopic = HelpAlias.SSHELP_NewAlertWizard_ObjectType;
                        break;
                    case 2:
                        helpTopic = HelpAlias.SSHELP_NewAlertWizard_Filters;
                        break;
                    case 3:
                        helpTopic = HelpAlias.SSHELP_NewAlertWizard_AlertRuleTimeFrame;
                        break;
                    case 4:
                        helpTopic = HelpAlias.SSHELP_NewAlertWizard_Actions;
                        break;
                    case 5:
                        helpTopic = HelpAlias.SSHELP_NewAlertWizard_Finish;
                        break;
                }
            }
            else
            {
                switch (_pageIndex)
                {
                    case 0:
                        helpTopic = HelpAlias.SSHELP_EditAlertWizard_EventType;
                        break;
                    case 1:
                        helpTopic = HelpAlias.SSHELP_EditAlertWizard_ObjectType;
                        break;
                    case 2:
                        helpTopic = HelpAlias.SSHELP_EditAlertWizard_Filters;
                        break;
                    case 3:
                        helpTopic = HelpAlias.SSHELP_NewAlertWizard_AlertRuleTimeFrame;
                        break;
                    case 4:
                        helpTopic = HelpAlias.SSHELP_EditAlertWizard_Actions;
                        break;
                    case 5:
                        helpTopic = HelpAlias.SSHELP_EditAlertWizard_Finish;
                        break;
                }
            }

            if (helpTopic != "") HelpAlias.ShowHelp(this, helpTopic);

            hlpevent.Handled = true;
        }

        private void ItemCheck_listBoxTargetObjects(object sender, System.Windows.Forms.ItemCheckEventArgs e)
        {
            if (_isInternalUpdate)
                return;
            CheckedListEventField info = (CheckedListEventField)_listBoxTargetObjects.Items[e.Index];

            if (e.NewValue == CheckState.Unchecked)
            {
                // ServerName is a special placeholder for TargetInstances of an Alert Rule
                if (info.Field.Id == (int)AlertableEventFields.serverName)
                    _rule.TargetInstances = new string[] { "<ALL>" };
                else
                    _rule.RemoveCondition(info.Field.ColumnName);
            }
            else if (e.NewValue == CheckState.Checked)
            {
                // ServerName is a special placeholder for TargetInstances of an Alert Rule
                if (info.Field.Id == (int)AlertableEventFields.serverName)
                {
                    _rule.TargetInstances = new string[] { };
                }
                else
                {
                    EventCondition newCondition = new EventCondition(info.Field);
                    _rule.AddCondition(newCondition);
                }
            }
            UpdateRuleDescription(_pageIndex);
        }

        private void Click_VerbRadioButton(object sender, System.EventArgs e)
        {
            if (_isInternalUpdate)
                return;
            bool stripSwing = false;

            if (_verbCondition != null)
                _rule.RemoveCondition(_verbCondition.TargetEventField.ColumnName);

            if (_rbSecurity.Checked)
            {
                _verbCondition = _verbConditionSecurity;
            }
            else if (_rbAdministrative.Checked)
            {
                _verbCondition = _verbConditionAdministrative;
            }
            else if (_rbLogins.Checked)
            {
                _verbCondition = _verbConditionLogin;
            }
            else if (_rbDDL.Checked)
            {
                _verbCondition = _verbConditionDDL;
            }
            else if (_rbDML.Checked)
            {
                _verbCondition = _verbConditionDML;
            }
            else if (_rbUserDefined.Checked)
            {
                _verbCondition = _verbConditionUserDefined;
            }
            if (_rbSpecificEvent.Checked)
            {
                _verbCondition = _verbConditionSpecificEvent;
                stripSwing = true;
                _linkSpecificEvent.Enabled = true;
            }
            else
            {
                _linkSpecificEvent.Enabled = false;
            }

            //  We only include the swing field for category-level verbs (not specific events)
            if (stripSwing)
            {
                if (_listBoxAdditionalFilters.Items.Contains(_swingField))
                    _listBoxAdditionalFilters.Items.Remove(_swingField);
                // Remove the swing from the rule if present
                foreach (EventCondition condition in _rule.Conditions)
                {
                    if (condition.FieldId == (int)AlertableEventFields.eventType && !condition.Inclusive)
                    {
                        _rule.RemoveCondition(condition.TargetEventField.ColumnName);
                    }
                }
            }
            else
            {
                if (!_listBoxAdditionalFilters.Items.Contains(_swingField))
                    _listBoxAdditionalFilters.Items.Add(_swingField);
                // However, we must always clear the swing on change
                foreach (EventCondition condition in _rule.Conditions)
                {
                    if (condition.FieldId == (int)AlertableEventFields.eventType)
                    {
                        condition.IntegerArray = new int[0];
                    }
                }
            }
            if (_verbCondition != null)
                _rule.AddCondition(_verbCondition);

            UpdateRuleDescription(_pageIndex);
        }

        private RichTextBox GetActiveRtf(int page)
        {
            switch (page)
            {
                case 0:
                    return _rtfVerb;
                case 1:
                    return _rtfTargetObjects;
                case 2:
                    return _rtfAdditionalFilters;
                case 3:
                    return _rtfPNewRuleDetails;
                case 4:
                    return _rtfP3RuleDetails;
                case 5:
                    return _rtfP4RuleDetails;
            }
            return null;
        }

        private void Click_AlertRulesActiveButton(object sender, System.EventArgs e)
        {
            if (_rbAlertRuleActiveAllTimes.Checked)
            {
                groupBox3.Enabled = false;
                _rule.IsAlertRuleTimeFrameEnabled = false;
                _alertRuleActiveStartTime.Value = DateTime.ParseExact("00:00:00", "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                _alertRuleActiveEndTime.Value = DateTime.ParseExact("23:59:59", "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                _chkAlertRuleActiveOnSun.Checked = false;
                _chkAlertRuleActiveOnMon.Checked = false;
                _chkAlertRuleActiveOnTue.Checked = false;
                _chkAlertRuleActiveOnWed.Checked = false;
                _chkAlertRuleActiveOnThu.Checked = false;
                _chkAlertRuleActiveOnFri.Checked = false;
                _chkAlertRuleActiveOnSat.Checked = false;
            }
            else if (_rbAlertRulesActiveSpecifiedTimeframe.Checked)
            {
                groupBox3.Enabled = true;
                _rule.IsAlertRuleTimeFrameEnabled = true;
                if(_rule.AlertRuleTimeFrameStartTime == "")
                    _rule.AlertRuleTimeFrameStartTime = "00:00:00";
                if (_rule.AlertRuleTimeFrameEndTime == "")
                    _rule.AlertRuleTimeFrameEndTime = "23:59:59";

                _alertRuleActiveStartTime.Value = Convert.ToDateTime(_rule.AlertRuleTimeFrameStartTime);
                _alertRuleActiveEndTime.Value = Convert.ToDateTime(_rule.AlertRuleTimeFrameEndTime);
                if(_rule.AlertRuleTimeFrameDaysOfWeek == "")
                {
                    _chkAlertRuleActiveOnMon.Checked = true;
                    _chkAlertRuleActiveOnTue.Checked = true;
                    _chkAlertRuleActiveOnWed.Checked = true;
                    _chkAlertRuleActiveOnThu.Checked = true;
                    _chkAlertRuleActiveOnFri.Checked = true;
                }
                foreach (char ch in _rule.AlertRuleTimeFrameDaysOfWeek)
                {
                    switch (ch)
                    {
                        case '1':
                            _chkAlertRuleActiveOnSun.Checked = true;
                            break;
                        case '2':
                            _chkAlertRuleActiveOnMon.Checked = true;
                            break;
                        case '3':
                            _chkAlertRuleActiveOnTue.Checked = true;
                            break;
                        case '4':
                            _chkAlertRuleActiveOnWed.Checked = true;
                            break;
                        case '5':
                            _chkAlertRuleActiveOnThu.Checked = true;
                            break;
                        case '6':
                            _chkAlertRuleActiveOnFri.Checked = true;
                            break;
                        case '7':
                            _chkAlertRuleActiveOnSat.Checked = true;
                            break;
                    }
                }
            }
        }

        private void ValueChanged_AlertRulesActiveStartTime(object sender, System.EventArgs e)
        {
            _rule.AlertRuleTimeFrameStartTime = _alertRuleActiveStartTime.Value.ToString("HH:mm:ss");
        }

        private void ValueChanged_AlertRulesActiveEndTime(object sender, System.EventArgs e)
        {
            _rule.AlertRuleTimeFrameEndTime = _alertRuleActiveEndTime.Value.ToString("HH:mm:ss");
        }

        private void CheckedChanged_AlertRuleActiveDaysOfTheWeek(object sender, System.EventArgs e)
        {
            _rule.AlertRuleTimeFrameDaysOfWeek = String.Empty;
            if (_chkAlertRuleActiveOnSun.Checked)
            {
                _rule.AlertRuleTimeFrameDaysOfWeek += "1";
            }
            if (_chkAlertRuleActiveOnMon.Checked)
            {
                _rule.AlertRuleTimeFrameDaysOfWeek += "2";
            }
            if (_chkAlertRuleActiveOnTue.Checked)
            {
                _rule.AlertRuleTimeFrameDaysOfWeek += "3";
            }
            if (_chkAlertRuleActiveOnWed.Checked)
            {
                _rule.AlertRuleTimeFrameDaysOfWeek += "4";
            }
            if (_chkAlertRuleActiveOnThu.Checked)
            {
                _rule.AlertRuleTimeFrameDaysOfWeek += "5";
            }
            if (_chkAlertRuleActiveOnFri.Checked)
            {
                _rule.AlertRuleTimeFrameDaysOfWeek += "6";
            }
            if (_chkAlertRuleActiveOnSat.Checked)
            {
                _rule.AlertRuleTimeFrameDaysOfWeek += "7";
            }
        }

        private void ValueChanged_EmailSummaryInterval(object sender, System.EventArgs e)
        {
            if(_txtEmailSummaryIntervalHours.Value > 0)
            {
                this._txtEmailSummaryIntervalMinutes.Minimum = 00;
            }
            else
            {
                this._txtEmailSummaryIntervalMinutes.Minimum = 30;
            }
            _rule.SummaryEmailFrequency = 0;
            _rule.SummaryEmailFrequency += (Convert.ToInt32(Math.Round(_txtEmailSummaryIntervalHours.Value, 0)) * 60) + Convert.ToInt32(Math.Round(_txtEmailSummaryIntervalMinutes.Value, 0));
        }
    }

   public class CheckedListEventField
   {
      private EventField _field ;
      private string _listEntry ;

      public EventField Field
      {
         get { return _field ; }
         set { _field = value ; }
      }

      public string ListEntry
      {
         get { return _listEntry ; }
         set { _listEntry = value ; }
      }

      public override string ToString()
      {
         return _listEntry ;
      }

      public override int GetHashCode()
      {
         if(_listEntry != null)
            return _listEntry.GetHashCode() ;
         else
            return 0 ;
      }
   }
}
