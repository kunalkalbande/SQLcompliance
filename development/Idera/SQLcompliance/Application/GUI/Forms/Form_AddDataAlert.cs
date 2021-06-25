using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Idera.SQLcompliance.Core.Rules.Alerts;
using Infragistics.Win;
using Idera.SQLcompliance.Core.Rules;
using System.Data.SqlClient;
using Idera.SQLcompliance.Core;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
    public partial class Form_AddDataAlert : Form
    {
        private const string SensitiveColumnDesc = "This alert will be generated when a column has been read that has been setup for Sensitive Column Auditing.";
        private const string ColumnValueChangedDesc = "The alert will be generated when a numeric column that has been setup for Before-After Data Auditing has been changed.";
        private const string ColumnValueChangedBadDesc = "The alert will be generated when a column that has been setup for Before-After Data Auditing has been changed.";
        private const string SensitiveColumnDatasetDesc = "This alert will be generated when a Dataset has been read that has been setup for Sensitive Column Auditing.";
        private readonly AlertingConfiguration _configuration = new AlertingConfiguration();
        private readonly bool _editing;
        private readonly bool _wizardInit;
        private LinkString _link;
        private Graphics _rtfGraphics;
        private List<string> _ruleNames = new List<string>();
        private WizardPage currentPage = WizardPage.StartPage;
        private Dictionary<string, CheckedListEventField> checkBoxList;

        public Form_AddDataAlert(DataAlertRule rule, List<string> ruleNames, bool fromTemplate)
        {
            InitializeComponent();
            CreateEventConditions();
            if (rule == null)
            {
                Text = @"New Data Alert Rule";
                DataRule = new DataAlertRule();
                DataRule.Enabled = true;
                DataRule.DataType = DataRuleType.SensitiveColumn;
                //DataRule.DataRuleTypeName = ruleNames[1];
                DataRule.Level = AlertLevel.Medium;
                DataRule.Instance = "<ALL>";
                DataRule.Database = "<ALL>";
                DataRule.FullTableName = "<ALL>";
                DataRule.Column = "<ALL>";
                _editing = false;
            }
            else
            {
                _wizardInit = true;
                _editing = true;
                DataRule = rule;

                if (fromTemplate)
                    Text = "New Data Alert Rule";
                else
                    Text = "Edit Data Alert Rule";
            }
            _ruleNames = ruleNames;
            Initialize();
            _wizardInit = false;
        }

        private void CreateEventConditions()
        {
            _configuration.Initialize(Globals.RepositoryServer);
            checkBoxList = new Dictionary<string, CheckedListEventField>();
            CheckedListEventField field = new CheckedListEventField();
            field.Field = _configuration.LookupAlertableEventField(AlertableEventFields.dataRuleApplicationName);
            field.ListEntry = "Application Name";
            checkBoxList[Applicationcheck.Name] = field;

            field = new CheckedListEventField();
            field.Field = _configuration.LookupAlertableEventField(AlertableEventFields.dataRuleLoginName);
            field.ListEntry = "Login Name";
            checkBoxList[Logincheck.Name] = field;

            field = new CheckedListEventField();
            field.Field = _configuration.LookupAlertableEventField(AlertableEventFields.rowCount);
            field.ListEntry = "Row Count (with Time Interval)";
            checkBoxList[Rowcountcheck.Name] = field;
        }

        #region properties

        public DataAlertRule DataRule { get; private set; }

        public string RuleText
        {
            get { return alertRuleRtfTextbox.Text; }
        }

        public List<string> RuleNames
        {
            set { _ruleNames = value; }
        }

        #endregion

        private void Initialize()
        {
            _rtfGraphics = alertRuleRtfTextbox.CreateGraphics();

            ruleDescriptionTextbox.Text = DataRule.Description;
            ruleNameTextbox.Text = DataRule.Name;
            alertLevelCombo.SelectedIndex = ((int) DataRule.Level) - 1;
            eventLogCheckbox.Checked = DataRule.HasLogAction;
            emailCheckbox.Checked = DataRule.HasEmailAction;
            snmpTrapCheckbox.Checked = DataRule.HasSnmpTrapAction;
            ruleEnabledCheckbox.Checked = DataRule.Enabled;
            foreach (EventCondition condition in DataRule.Conditions)
            {
                switch ((AlertableEventFields)condition.FieldId)
                {
                    case AlertableEventFields.dataRuleApplicationName:
                        Applicationcheck.Checked = true;
                        break;
                    case AlertableEventFields.dataRuleLoginName:
                        Logincheck.Checked = true;
                        break;
                    case AlertableEventFields.rowCount:
                        Rowcountcheck.Checked = true;
                        break;
                }
            }

            InitRuleCombo();
            //txtRuleType.Text = _ruleNames[((int)DataRuleType.SensitiveColumn)- 1];
            InitObjectCheckboxes();
            SetSelectedRuleType();
            SetDescriptionText();
            ShowPage();
        }

        private void InitRuleCombo()
        {
            ValueListItem item = null;
            var index = 0;

            foreach (var name in _ruleNames)
            {
                index += 1;

                // ColumnValueChanged rule skipped as it was disabled earlier
                if (index == 2)
                    continue;

                item = new ValueListItem((DataRuleType) index, name);
                ruleCombo.Items.Add(item);
            }
            ruleCombo.Sorted = true;
        }

        private void SetSelectedRuleType()
        {
            foreach (ValueListItem item in ruleCombo.Items)
            {
                if ((DataRuleType) item.DataValue == DataRule.DataType)
                {
                    ruleCombo.SelectedItem = item;
                    break;
                }
            }
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            //increment to the next page.
            currentPage++;

            if (currentPage > WizardPage.FinishPage)
            {
                if (SaveRule())
                {
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    currentPage--;
                }
            }
            else
            {
                ShowPage();
            }
        }

        private void previousButton_Click(object sender, EventArgs e)
        {
            currentPage--;
            ShowPage();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// </summary>
        private void ShowPage()
        {
            addRuleWizard.Tabs[(int) currentPage].Selected = true;
            UpdateView();
        }

        private bool SaveRule()
        {
            DataRule.Name = ruleNameTextbox.Text;
            DataRule.Description = ruleDescriptionTextbox.Text;
            DataRule.Enabled = ruleEnabledCheckbox.Checked;
            return true;
        }

        private void UpdateView()
        {
            switch (currentPage)
            {
                case WizardPage.StartPage:
                {
                    previousButton.Enabled = false;
                    nextButton.Enabled = true;
                    cancelButton.Enabled = true;
                    titleLabel.Text = "Data Alert Type";
                    descLabel.Text = @"Define the rule that will alert on Sensitive Column access or Before After Data change.  Click Next to continue.";
                    UpdateRuleText();
                    break;
                }
                case WizardPage.ObjectTypes:
                {
                    previousButton.Enabled = true;
                    nextButton.Enabled = true;
                    nextButton.Text = "Next";
                    cancelButton.Enabled = true;
                    titleLabel.Text = "Data Alert Type";
                    descLabel.Text = "Select the SQL Server, database, table, and column that you want to monitor.";
                    UpdateRuleText();
                    break;
                }
                case WizardPage.AdditionalEventFilter:
                {
                    if (DataRule.TargetInstanceList == String.Empty)
                    {
                        MessageBox.Show(this, CoreConstants.Exception_InvalidInstance, "Invalid Data Alert Rule",
                                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        currentPage = currentPage - 1;
                        addRuleWizard.Tabs[(int)currentPage].Selected = true;
                        break;
                    }

                    previousButton.Enabled = true;
                    nextButton.Enabled = true;
                    nextButton.Text = "Next";
                    cancelButton.Enabled = true;
                    titleLabel.Text = "Data Alert Type";
                    descLabel.Text = "Specify when the selected event should trigger this alert.";
                    UpdateRuleText();
                    Rowcountcheck.Enabled = VerifyRowcountStatusByInstance();
                    break;
                }
                case WizardPage.AlertRulesTimeFrame:
                {
                    previousButton.Enabled = true;
                    nextButton.Enabled = true;
                    nextButton.Text = "Next";
                    cancelButton.Enabled = true;
                    titleLabel.Text = "Alert Rule Timeframe";
                    descLabel.Text = "Specify the timeframes for applying the event rule. When not specified, the rule will always be active";
                    UpdateRuleText();
                    break;
                }
                case WizardPage.AlertActions:
                {
                    previousButton.Enabled = true;
                    nextButton.Enabled = true;
                    nextButton.Text = "Next";
                    cancelButton.Enabled = true;
                    titleLabel.Text = "Alert Actions";
                    descLabel.Text = "Select the action to be taken when SQL Server data matches this rule.";
                    UpdateRuleText();
                    break;
                }
                case WizardPage.FinishPage:
                {
                    previousButton.Enabled = true;
                    nextButton.Enabled = true;
                    nextButton.Text = "Finish";
                    cancelButton.Enabled = true;
                    titleLabel.Text = "Finish Data Alert Rule";
                    descLabel.Text =
                        "Specify the name for this rule and select the categorization level for the alert. Also, \r\nselect whether to enable this rule now.";
                    UpdateRuleText();
                    break;
                }
                default:
                {
                    previousButton.Enabled = false;
                    nextButton.Enabled = false;
                    cancelButton.Enabled = false;
                    break;
                }
            }
        }

        private bool VerifyRowcountStatusByInstance()
        {
            bool rowCountEnabled = false;
            foreach (string[] instanceData in GetInstanceDataFromRepository())
            {
                var agentServiceRequest = Helper.GUIRemoteObjectsProvider.AgentManagementConsoleRequest(instanceData[0], int.Parse(instanceData[1]));
                rowCountEnabled = agentServiceRequest.isRowCountEnabled();
                if (rowCountEnabled)
                    break;
            }
            return rowCountEnabled;
        }

        private List<string[]> GetInstanceDataFromRepository()
        {
            List<string[]> instanceData = new List<string[]>();
            string cmdstr;
            try
            {
                if (DataRule.TargetInstanceList.Contains("ALL"))
                {
                    cmdstr = "SELECT [agentServer], [agentPort] FROM [SQLcompliance].[dbo].[Servers] where isEnabled = 1";

                }
                else
                {
                    cmdstr = string.Format("SELECT [agentServer], [agentPort] FROM[SQLcompliance].[dbo].[Servers] where isEnabled = 1 " +
                        "and [instance] = '{0}'", DataRule.TargetInstanceList);
                }

                using (SqlCommand cmd = new SqlCommand(cmdstr,
                                                  Globals.Repository.Connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())                    {
                        
                        while (reader.Read())
                        {
                            string[] instanceDataResult = new string[2];
                            instanceDataResult[0] = SQLHelpers.GetString(reader, 0);
                            instanceDataResult[1] = SQLHelpers.GetInt32(reader, 1).ToString();
                            instanceData.Add(instanceDataResult);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("A problem occurred while getting data from repository: {0}"), ex.Message);               
            }
            return instanceData;
        }

        private void RuleCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedItem = (ValueListItem) ((ComboBox) sender).SelectedItem;

            if (selectedItem.DataValue == null)
                return;

            //if this is the case, the combo is just being initialized.
            if ((DataRuleType) selectedItem.DataValue == DataRule.DataType)
                return;

            if (selectedItem != null)
            {
                DataRule.DataType = (DataRuleType) selectedItem.DataValue;
                SetDescriptionText();
                DataRule.DataRuleTypeName = selectedItem.DisplayText;
                DataRule.Instance = "<ALL>";
                DataRule.Database = "<ALL>";
                DataRule.FullTableName = "<ALL>";
                DataRule.Column = "<ALL>";
                UpdateRuleText();
            }
        }

        private void SetDescriptionText()
        {
            //this.ruleDescription.Text = this._sensitiveColumnDesc;

            switch (DataRule.DataType)
            {
                case DataRuleType.SensitiveColumn:
                    ruleDescription.Text = SensitiveColumnDesc;
                    break;
                case DataRuleType.ColumnValueChanged:
                    ruleDescription.Text = ColumnValueChangedDesc;
                    break;
                case DataRuleType.ColumnValueChangedBad:
                    ruleDescription.Text = ColumnValueChangedBadDesc;
                    break;
                case DataRuleType.SensitiveColumnViaDataset:
                    ruleDescription.Text = SensitiveColumnDatasetDesc;
                    break;
            }
        }

        private void AlertRuleRtf_MouseMove(object sender, MouseEventArgs e)
        {
            if (!Globals.isAdmin)
                return;

            var seg = _link.LinkHitTest(e.X, e.Y, alertRuleRtfTextbox, _rtfGraphics);

            if (seg != null && seg.Tag != null)
            {
                alertRuleRtfTextbox.Cursor = Cursors.Hand;
                Cursor.Current = Cursors.Hand;
            }
            else
            {
                alertRuleRtfTextbox.Cursor = Cursors.Default;
                Cursor.Current = Cursors.Default;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AlertRuleRtf_MouseDown(object sender, MouseEventArgs e)
        {
            var link = _link.LinkHitTest(e.X, e.Y, sender as RichTextBox, _rtfGraphics);
            Form_DataAlertColumns columns;

            if (link == null || link.Tag == null || e.Button != MouseButtons.Left || !Globals.isAdmin)
                return;
            if (link.Tag is string)
            {
                var tagString = (string) link.Tag;

                if (String.Equals(tagString, "AlertLevel"))
                {
                    var frmLevel = new Form_AlertLevel();
                    frmLevel.Level = DataRule.Level;
                    if (frmLevel.ShowDialog(this) == DialogResult.OK)
                    {
                        DataRule.Level = frmLevel.Level;
                        alertLevelCombo.SelectedIndex = ((int) DataRule.Level) - 1;
                    }
                }
                else if (String.Equals(tagString, "Comparison"))
                {
                    var frmComparison = new Form_ChangeComparison(DataRule);

                    if (frmComparison.ShowDialog(this) == DialogResult.OK)
                    {
                        DataRule.Comparison.Operator = frmComparison.Operator;
                        DataRule.Comparison.Value = frmComparison.Value;
                    }
                }
                else if (String.Equals(tagString, "Column"))
                {
                    columns = new Form_DataAlertColumns(Selection.Column, DataRule.DataType, DataRule);

                    if (columns.ShowDialog(this) == DialogResult.OK)
                    {
                        DataRule.Instance = columns.Instance;
                        DataRule.Database = columns.Database;
                        DataRule.FullTableName = columns.Table;

                        if (String.Equals("All Columns", columns.Column))
                        {
                            DataRule.Column = "<ALL>";
                            columnCheck.Checked = false;
                        }
                        else
                        {
                            DataRule.Column = columns.Column;
                        }
                    }
                }
                else if (String.Equals(tagString, "Table"))
                {
                    columns = new Form_DataAlertColumns(Selection.Table, DataRule.DataType, DataRule);

                    if (columns.ShowDialog(this) == DialogResult.OK)
                    {
                        DataRule.Instance = columns.Instance;
                        DataRule.Database = columns.Database;

                        //only clear it if the table changed
                        if (!String.Equals(DataRule.FullTableName, columns.Table))
                        {
                            if (!String.IsNullOrEmpty(DataRule.FullTableName))
                            {
                                DataRule.Column = "<ALL>";
                                columnCheck.Checked = false;
                            }
                            DataRule.FullTableName = columns.Table;
                        }
                    }
                }
                else if (String.Equals(tagString, "Database"))
                {
                    columns = new Form_DataAlertColumns(Selection.Database, DataRule.DataType, DataRule);

                    if (columns.ShowDialog(this) == DialogResult.OK)
                    {
                        DataRule.Instance = columns.Instance;

                        //only clear it if the database changed
                        if (!String.Equals(DataRule.Database, columns.Database))
                        {
                            if (!String.IsNullOrEmpty(DataRule.Database))
                            {
                                DataRule.FullTableName = "<ALL>";
                                tableCheck.Checked = false;
                                DataRule.Column = "<ALL>";
                                columnCheck.Checked = false;
                            }
                            DataRule.Database = columns.Database;
                        }
                    }
                }
                else if (String.Equals(tagString, "Instance"))
                {
                    columns = new Form_DataAlertColumns(Selection.Instance, DataRule.DataType, DataRule);
                    if (columns.ShowDialog(this) == DialogResult.OK)
                    {
                        //only clear it if the instance changed
                        if (!String.Equals(DataRule.Instance, columns.Instance))
                        {
                            if (!String.IsNullOrEmpty(DataRule.Instance))
                            {
                                DataRule.Database = "<ALL>";
                                databaseCheck.Checked = false;
                                DataRule.FullTableName = "<ALL>";
                                tableCheck.Checked = false;
                                DataRule.Column = "<ALL>";
                                columnCheck.Checked = false;
                            }
                            DataRule.Instance = columns.Instance;
                        }
                    }
                }
                else if (String.Equals(tagString, "AlertMessage"))
                {
                    Form_EmailTemplate options;
                    List<MacroDefinition> dataMacros = new List<MacroDefinition>();
                    foreach (MacroDefinition dataMacro in _configuration.DataMacros)
                    {
                        if (DataRule.DataType == DataRuleType.SensitiveColumn
                            && (dataMacro.Name == "After Data Value" || dataMacro.Name == "Before Data Value"))
                            continue;
                        dataMacros.Add(dataMacro);
                    }
                    options = new Form_EmailTemplate(dataMacros.ToArray());
                    options.Subject = DataRule.MessageTitle;
                    options.Message = DataRule.MessageBody;
                    if (options.ShowDialog(this) == DialogResult.OK)
                    {
                        DataRule.MessageTitle = options.Subject;
                        DataRule.MessageBody = options.Message;
                    }
                }
                else if (String.Equals(tagString, "Recipients"))
                {
                    var emailForm = new Form_EmailList();
                    emailForm.EmailArray = DataRule.Recipients;
                    if (emailForm.ShowDialog(this) == DialogResult.OK)
                    {
                        DataRule.Recipients = emailForm.EmailArray;
                    }
                }
                else if (String.Equals(tagString, "LogEntryType"))
                {
                    var logTypeForm = new Form_EventLogType();

                    logTypeForm.EntryType = DataRule.LogEntryType;
                    if (logTypeForm.ShowDialog(this) == DialogResult.OK)
                    {
                        DataRule.LogEntryType = logTypeForm.EntryType;
                    }
                }
                else if (String.Equals(tagString, "SnmpConfigurations"))
                {
                    // set default SNMP configurations if missing
                    if (DataRule.SnmpConfiguration == null)
                        DataRule.SnmpConfiguration = AlertingDal.SelectSnmpConfiguration(Globals.Repository.Connection.ConnectionString);

                    var frmSnmpConfigurations = new Form_AlertingOptions_Snmp(DataRule.SnmpConfiguration);
                    if (frmSnmpConfigurations.ShowDialog(this) == DialogResult.OK)
                        DataRule.SnmpConfiguration = frmSnmpConfigurations.SnmpConfiguration;
                }
            }

            else if (link.Tag is EventCondition)
            {
                EventCondition condition = (EventCondition)link.Tag;

                switch (condition.ConditionType)
                {
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
                        }

                        break;
                }
            }
            UpdateRuleText();
        }

        private void EventLogCheckbox_CheckChanged(object sender, EventArgs e)
        {
            DataRule.HasLogAction = eventLogCheckbox.Checked;
            UpdateRuleText();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EmailCheckbox_CheckChanged(object sender, EventArgs e)
        {
            DataRule.HasEmailAction = emailCheckbox.Checked;
            UpdateRuleText();
        }

        private void SnmpTrapCheckbox_CheckChanged(object sender, EventArgs e)
        {
            DataRule.HasSnmpTrapAction = snmpTrapCheckbox.Checked;
            UpdateRuleText();
        }

        private void UpdateRuleText()
        {
            alertRuleRtfTextbox.Rtf = AlertUIHelper.GenerateRuleDescription(DataRule, out _link);
        }

        private void AlertLevelCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataRule.Level = (AlertLevel) (alertLevelCombo.SelectedIndex + 1);
            UpdateRuleText();
        }

        private void Form_AddDataAlert_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            var helpTopic = "";

            switch (currentPage)
            {
                case WizardPage.StartPage:
                    if (_editing)
                        helpTopic = HelpAlias.SSHELP_EditDataAlertWizard_Type;
                    else
                        helpTopic = HelpAlias.SSHELP_NewDataAlertWizard_Type;
                    break;
                case WizardPage.ObjectTypes:
                    if (_editing)
                        helpTopic = HelpAlias.SSHELP_EditDataAlertWizard_Object;
                    else
                        helpTopic = HelpAlias.SSHELP_NewDataAlertWizard_Object;
                    break;
                case WizardPage.AlertActions:
                    if (_editing)
                        helpTopic = HelpAlias.SSHELP_EditDataAlertWizard_Action;
                    else
                        helpTopic = HelpAlias.SSHELP_NewDataAlertWizard_Action;
                    break;
                case WizardPage.FinishPage:
                    if (_editing)
                        helpTopic = HelpAlias.SSHELP_EditDataAlertWizard_Finish;
                    else
                        helpTopic = HelpAlias.SSHELP_NewDataAlertWizard_Finish;
                    break;
            }

            if (helpTopic != "")
                HelpAlias.ShowHelp(this, helpTopic);
            hlpevent.Handled = true;
        }

        private void InitObjectCheckboxes()
        {
            if (!String.Equals(DataRule.Instance, "<ALL>"))
            {
                instanceCheck.Checked = true;

                if (!String.Equals(DataRule.Database, "<ALL>"))
                {
                    databaseCheck.Checked = true;

                    if (!String.Equals(DataRule.Table, "<ALL>"))
                    {
                        tableCheck.Checked = true;

                        if (!String.Equals(DataRule.Column, "<ALL>"))
                        {
                            columnCheck.Checked = true;
                        }
                    }
                }
            }
        }

        private void instanceCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (!_wizardInit)
            {
                if (instanceCheck.Checked)
                    DataRule.Instance = String.Empty;
                else
                    DataRule.Instance = "<ALL>";
            }
            UpdateCheckboxes(ObjectType.Instance, instanceCheck);
        }

        private void databaseCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (!_wizardInit)
            {
                if (databaseCheck.Checked)
                    DataRule.Database = String.Empty;
                else
                    DataRule.Database = "<ALL>";
            }
            UpdateCheckboxes(ObjectType.Database, databaseCheck);
        }

        private void tableCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (!_wizardInit)
            {
                if (tableCheck.Checked)
                    DataRule.FullTableName = String.Empty;
                else
                    DataRule.FullTableName = "<ALL>";
            }
            UpdateCheckboxes(ObjectType.Table, tableCheck);
        }

        private void columnCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (!_wizardInit)
            {
                if (columnCheck.Checked)
                    DataRule.Column = String.Empty;
                else
                    DataRule.Column = "<ALL>";
            }
            UpdateCheckboxes(ObjectType.Column, columnCheck);
        }

        private void UpdateCheckboxes(ObjectType type, CheckBox checkBox)
        {
            if (checkBox.Checked)
            {
                switch (type)
                {
                    case ObjectType.Column:
                        instanceCheck.Checked = true;
                        databaseCheck.Checked = true;
                        tableCheck.Checked = true;
                        break;
                    case ObjectType.Table:
                        instanceCheck.Checked = true;
                        databaseCheck.Checked = true;
                        break;
                    case ObjectType.Database:
                        instanceCheck.Checked = true;
                        break;
                }
                ;
            }
            else
            {
                switch (type)
                {
                    case ObjectType.Instance:
                        databaseCheck.Checked = false;
                        tableCheck.Checked = false;
                        columnCheck.Checked = false;
                        break;
                    case ObjectType.Database:
                        tableCheck.Checked = false;
                        columnCheck.Checked = false;
                        break;
                    case ObjectType.Table:
                        columnCheck.Checked = false;
                        break;
                }
                ;
            }
            UpdateRuleText();
        }

        private enum WizardPage
        {
            StartPage,
            ObjectTypes,
            AdditionalEventFilter,
            AlertRulesTimeFrame,
            AlertActions,
            FinishPage
        }

        private enum ObjectType
        {
            Instance,
            Database,
            Table,
            Column
        }

        private void addRuleWizard_SelectedTabChanged(object sender, Infragistics.Win.UltraWinTabControl.SelectedTabChangedEventArgs e)
        {

        }

        private void EventFilterCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            DataRule.ApplicationName =   Applicationcheck.Checked;
            DataRule.LoginName =         Logincheck.Checked;
            DataRule.RowCount =          Rowcountcheck.Checked;
            CheckedListEventField info = checkBoxList[((CheckBox)sender).Name];
            if (((CheckBox)sender).CheckState == CheckState.Unchecked)
                DataRule.RemoveCondition(info.Field.ColumnName);
             else if (((CheckBox)sender).CheckState == CheckState.Checked)
            {
                if (new List<EventCondition>(DataRule.Conditions).Exists(x => x.FieldId == info.Field.Id))
                    return;
                EventCondition newCondition = new EventCondition(info.Field);
                DataRule.AddCondition(newCondition);
            }
            UpdateRuleText();
        }
    }
}