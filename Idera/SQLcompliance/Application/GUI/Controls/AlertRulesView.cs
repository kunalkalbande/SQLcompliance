using System ;
using System.Collections.Generic;
using System.Drawing ;
using System.Windows.Forms;
using Idera.SQLcompliance.Application.GUI.Forms ;
using Idera.SQLcompliance.Application.GUI.Helper;
using Idera.SQLcompliance.Core ;
using Idera.SQLcompliance.Core.Rules;
using Idera.SQLcompliance.Core.Rules.Alerts ;
using Infragistics.Win;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;

namespace Idera.SQLcompliance.Application.GUI.Controls
{
	/// <summary>
	/// Summary description for AlertRulesView.
	/// </summary>
	public partial class AlertRulesView : BaseControl
	{
		private AlertingConfiguration _configuration ;
		private LinkString _link ;
		private Graphics _rtfGraphics ;
      private bool _displayEventRules = true;
      private bool _displayStatusRules = true;
      private bool _displayDataRules = true;

		public AlertRulesView()
		{
         // This call is required by the Windows.Forms Form Designer.
         InitializeComponent();

         GridHelper.ApplyAdminSettings(_grid);
         _rtfGraphics = _rtfRuleDetails.CreateGraphics();

         SetMenuFlag(CMMenuItem.Refresh) ;
         SetMenuFlag(CMMenuItem.Collapse) ;
         SetMenuFlag(CMMenuItem.Expand) ;
         SetMenuFlag(CMMenuItem.GroupByColumn) ;
         SetMenuFlag(CMMenuItem.ShowHelp) ;
         SetMenuFlag(CMMenuItem.NewAlertRule, Globals.isAdmin);
         SetMenuFlag(CMMenuItem.NewStatusAlertRule, Globals.isAdmin);
         SetMenuFlag(CMMenuItem.ImportAlertRules, Globals.isAdmin);
         SetMenuFlag(CMMenuItem.NewDataAlertRule, Globals.isAdmin);
      }
      
      public AlertingConfiguration Configuration
		{
			set { _configuration = value ; }
			get { return _configuration ; }
		}

      public bool DisplayEventRules
      {
         get { return _displayEventRules; }
         set 
         {
            _displayEventRules = value;
         }
      }

      public bool DisplayStatusRules
      {
         get { return _displayStatusRules; }
         set
         { 
            _displayStatusRules = value;
         }
      }

      public bool DisplayDataRules
      {
         get { return _displayDataRules; }
         set { _displayDataRules = value; }
      }

		#region Events

		private void MouseDown_rtfRuleDetails(object sender, MouseEventArgs e)
		{
			if(_link == null || !Globals.isAdmin)
				return ;

			LinkSegment link = _link.LinkHitTest(e.X, e.Y, sender as RichTextBox, _rtfGraphics) ;
         EventRule rule = GetActiveAlertRule() ;

			if(rule == null || link == null || link.Tag == null || e.Button != MouseButtons.Left)
				return ;

         switch (rule.RuleType)
         {
            case EventType.SqlServer:
               ProcessRuleDetails(rule as AlertRule, link);
               break;
            case EventType.Status:
               ProcessRuleDetails(rule as StatusAlertRule, link);
               break;
            case EventType.Data:
               ProcessRuleDetails(rule as DataAlertRule, link);
               break;
         }
		}

      private void ProcessRuleDetails(AlertRule rule, LinkSegment link)
      {
         if (link.Tag is string)
         {
            string tagString = (string)link.Tag;

            if (String.Equals(tagString, "TargetInstances"))
            {
               Form_InstanceList frmInstances = new Form_InstanceList();
               frmInstances.SelectedInstances = rule.TargetInstances;

               if (frmInstances.ShowDialog(this) == DialogResult.OK)
               {
                  rule.TargetInstances = frmInstances.SelectedInstances;
                  UpdateRule();
               }
            }
            else if (String.Equals(tagString, "AlertLevel"))
            {
               Form_AlertLevel frmLevel = new Form_AlertLevel();
               frmLevel.Level = rule.Level;

               if (frmLevel.ShowDialog(this) == DialogResult.OK)
               {
                  rule.Level = frmLevel.Level;
                  UpdateRule();
               }
            }
            else if (String.Equals(tagString, "AlertMessage"))
            {
               Form_EmailTemplate options;
               options = new Form_EmailTemplate(_configuration.SqlServerMacros);
               options.Subject = rule.MessageTitle;
               options.Message = rule.MessageBody;

               if (options.ShowDialog(this) == DialogResult.OK)
               {
                  rule.MessageTitle = options.Subject;
                  rule.MessageBody = options.Message;
                  UpdateRule();
               }
            }
            else if (String.Equals(tagString, "Recipients"))
            {
               Form_EmailList emailForm = new Form_EmailList();
               emailForm.EmailArray = rule.Recipients;

               if (emailForm.ShowDialog(this) == DialogResult.OK)
               {
                  rule.Recipients = emailForm.EmailArray;
                  UpdateRule();
               }
            }
            else if (String.Equals(tagString, "LogEntryType"))
            {
               Form_EventLogType logTypeForm = new Form_EventLogType();
               logTypeForm.EntryType = rule.LogEntryType;

               if (logTypeForm.ShowDialog(this) == DialogResult.OK)
               {
                  rule.LogEntryType = logTypeForm.EntryType;
                  UpdateRule();
               }
            }
            else if (String.Equals(tagString, "SnmpConfigurations"))
            {
                // set default SNMP configurations if missing
                if (rule.SnmpConfiguration == null)
                    rule.SnmpConfiguration = AlertingDal.SelectSnmpConfiguration(Globals.Repository.Connection.ConnectionString);

                Form_AlertingOptions_Snmp frmSnmpConfigurations = new Form_AlertingOptions_Snmp(rule.SnmpConfiguration);
                if (frmSnmpConfigurations.ShowDialog(this) == DialogResult.OK)
                {
                    rule.SnmpConfiguration = frmSnmpConfigurations.SnmpConfiguration;
                    UpdateRule();
                }
            }
         }
         else if (link.Tag is EventCondition)
         {
            EventCondition condition = (EventCondition)link.Tag;

            switch (condition.ConditionType)
            {
               case MatchType.Bool:
                  {
                     Form_Boolean boolForm = new Form_Boolean();
                     boolForm.BooleanValue = condition.BooleanValue;
                     boolForm.Directions = "&Select the desired value:";
                     boolForm.Title = condition.TargetEventField.DisplayName;
                   
                     if (boolForm.ShowDialog(this) == DialogResult.OK)
                     {
                        condition.BooleanValue = boolForm.BooleanValue;
                        UpdateRule();
                     }
                     break;
                  }
                case MatchType.Integer:
                  {
                     EventCondition verb = AlertUIHelper.ExtractVerbCondition(rule);

                     if (condition.Inclusive)
                     {
                        CMEventType selectedEvent = _configuration.LookupEventType(verb.IntegerArray[0], EventType.SqlServer);
                        CMEventCategory selectedCateogry = _configuration.LookupCategory(selectedEvent.CategoryId);
                        Form_EventSelector selectorForm = new Form_EventSelector(_configuration.SqlServerCategories, selectedCateogry, selectedEvent);

                        if (selectorForm.ShowDialog(this) == DialogResult.OK)
                        {
                           selectedEvent = selectorForm.SelectedEvent;
                           int[] evEventType = new int[1];
                           evEventType[0] = selectedEvent.TypeId;

                           if (evEventType[0] != verb.IntegerArray[0])
                           {
                              verb.IntegerArray = evEventType;
                              UpdateRule();
                           }
                        }
                     }
                     else
                     {
                        Form_EventTypeList evForm;
                        CMEventCategory[] evCategories;

                        if (verb == null)
                        {
                           evCategories = _configuration.SqlServerCategories;
                        }
                        else
                        {
                           evCategories = new CMEventCategory[verb.IntegerArray.Length];

                           for (int i = 0; i < verb.IntegerArray.Length; i++)
                           {
                              evCategories[i] = _configuration.LookupCategory(verb.IntegerArray[i]);
                           }
                        }
                        evForm = new Form_EventTypeList(evCategories);
                        evForm.SelectedEventIds = condition.IntegerArray;

                        if (evForm.ShowDialog(this) == DialogResult.OK)
                        {
                           condition.IntegerArray = evForm.SelectedEventIds;
                           UpdateRule();
                        }
                     }
                     break;
                  }
               case MatchType.String:
                  {
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
                              UpdateRule();
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
                        UpdateRule();
                     }
                     break;
                  }
            }
         }
      }

      private void ProcessRuleDetails(StatusAlertRule rule, LinkSegment link)
      {
         if (link.Tag is string)
         {
            string tagString = (string)link.Tag;

            if (String.Equals(tagString, "AlertLevel"))
            {
               Form_AlertLevel frmLevel = new Form_AlertLevel();
               frmLevel.Level = rule.Level;

               if (frmLevel.ShowDialog(this) == DialogResult.OK)
               {
                  rule.Level = frmLevel.Level;
                  UpdateRule();
               }
            }
            else if (String.Equals(tagString, "Threshold"))
            {
               Form_ChangeThreshold frmThreshold = new Form_ChangeThreshold(rule);

               if (frmThreshold.ShowDialog(this) == DialogResult.OK)
               {
                  rule.Threshold.Value = frmThreshold.Threshold;
                  UpdateRule();
               }
            }
            else if (String.Equals(tagString, "AlertMessage"))
            {
               Form_EmailTemplate options;
               options = new Form_EmailTemplate(_configuration.DataMacros);
               options.Subject = rule.MessageTitle;
               options.Message = rule.MessageBody;

               if (options.ShowDialog(this) == DialogResult.OK)
               {
                  rule.MessageTitle = options.Subject;
                  rule.MessageBody = options.Message;
                  UpdateRule();
               }
            }
            else if (String.Equals(tagString, "Recipients"))
            {
               Form_EmailList emailForm = new Form_EmailList();
               emailForm.EmailArray = rule.Recipients;

               if (emailForm.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
               {
                  rule.Recipients = emailForm.EmailArray;
                  UpdateRule();
               }
            }
            else if (String.Equals(tagString, "LogEntryType"))
            {
               Form_EventLogType logTypeForm = new Form_EventLogType();
               logTypeForm.EntryType = rule.LogEntryType;

               if (logTypeForm.ShowDialog(this) == DialogResult.OK)
               {
                  rule.LogEntryType = logTypeForm.EntryType;
                  UpdateRule();
               }
            }
            else if (String.Equals(tagString, "SnmpConfigurations"))
            {
                // set default SNMP configurations if missing
                if (rule.SnmpConfiguration == null)
                    rule.SnmpConfiguration = AlertingDal.SelectSnmpConfiguration(Globals.Repository.Connection.ConnectionString);

                Form_AlertingOptions_Snmp frmSnmpConfigurations = new Form_AlertingOptions_Snmp(rule.SnmpConfiguration);
                if (frmSnmpConfigurations.ShowDialog(this) == DialogResult.OK)
                {
                    rule.SnmpConfiguration = frmSnmpConfigurations.SnmpConfiguration;
                    UpdateRule();
                }
            }
         }
      }

      private void ProcessRuleDetails(DataAlertRule rule, LinkSegment link)
      {
         Form_DataAlertColumns columns;

         if (link.Tag is string)
         {
            string tagString = (string)link.Tag;

            if (String.Equals(tagString, "AlertLevel"))
            {
               Form_AlertLevel frmLevel = new Form_AlertLevel();
               frmLevel.Level = rule.Level;
               if (frmLevel.ShowDialog(this) == DialogResult.OK)
               {
                  rule.Level = frmLevel.Level;
               }
            }
            else if (String.Equals(tagString, "Comparison"))
            {
               Form_ChangeComparison frmComparison = new Form_ChangeComparison(rule);

               if (frmComparison.ShowDialog(this) == DialogResult.OK)
               {
                  rule.Comparison.Operator = frmComparison.Operator;
                  rule.Comparison.Value = frmComparison.Value;
               }
            }
            else if (String.Equals(tagString, "Column"))
            {
               columns = new Form_DataAlertColumns(Selection.Column, rule.DataType, rule);

               if (columns.ShowDialog(this) == DialogResult.OK)
               {
                  rule.Instance = columns.Instance;
                  rule.Database = columns.Database;
                  rule.FullTableName = columns.Table;

                  if (String.Equals("All Columns", columns.Column))
                  {
                     rule.Column = "<ALL>";
                  }
                  else
                  {
                     rule.Column = columns.Column;
                  }
               }
            }
            else if (String.Equals(tagString, "Table"))
            {
               columns = new Form_DataAlertColumns(Selection.Table, rule.DataType, rule);

               if (columns.ShowDialog(this) == DialogResult.OK)
               {
                  rule.Instance = columns.Instance;
                  rule.Database = columns.Database;

                  //only clear it if the table changed
                  if (!String.Equals(rule.FullTableName, columns.Table))
                  {
                     if (!String.IsNullOrEmpty(rule.FullTableName))
                     {
                        rule.Column = "<ALL>";
                     }
                     rule.FullTableName = columns.Table;
                  }
               }
            }
            else if (String.Equals(tagString, "Database"))
            {
               columns = new Form_DataAlertColumns(Selection.Database, rule.DataType, rule);

               if (columns.ShowDialog(this) == DialogResult.OK)
               {
                  rule.Instance = columns.Instance;

                  //only clear it if the database changed
                  if (!String.Equals(rule.Database, columns.Database))
                  {
                     if (!String.IsNullOrEmpty(rule.Database))
                     {
                        rule.FullTableName = "<ALL>";
                        rule.Column = "<ALL>";
                     }
                     rule.Database = columns.Database;
                  }
               }
            }
            else if (String.Equals(tagString, "Instance"))
            {
               columns = new Form_DataAlertColumns(Selection.Instance, rule.DataType, rule);
               if (columns.ShowDialog(this) == DialogResult.OK)
               {
                  //only clear it if the instance changed
                  if (!String.Equals(rule.Instance, columns.Instance))
                  {
                     if (!String.IsNullOrEmpty(rule.Instance))
                     {
                        rule.Database = "<ALL>";
                        rule.FullTableName = "<ALL>";
                        rule.Column = "<ALL>";
                     }
                     rule.Instance = columns.Instance;
                  }
               }
            }
            else if (String.Equals(tagString, "AlertMessage"))
            {
               Form_EmailTemplate options;
               List<MacroDefinition> dataMacros = new List<MacroDefinition>();
               foreach (MacroDefinition dataMacro in _configuration.DataMacros)
               {
                   if (rule.DataType == DataRuleType.SensitiveColumn
                       && (dataMacro.Name == "After Data Value" || dataMacro.Name == "Before Data Value"))
                       continue;
                   dataMacros.Add(dataMacro);
               }
               options = new Form_EmailTemplate(dataMacros.ToArray());
               options.Subject = rule.MessageTitle;
               options.Message = rule.MessageBody;
               if (options.ShowDialog(this) == DialogResult.OK)
               {
                  rule.MessageTitle = options.Subject;
                  rule.MessageBody = options.Message;
               }
            }
            else if (String.Equals(tagString, "Recipients"))
            {
               Form_EmailList emailForm = new Form_EmailList();
               emailForm.EmailArray = rule.Recipients;
               if (emailForm.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
               {
                  rule.Recipients = emailForm.EmailArray;
               }
            }
            else if (String.Equals(tagString, "LogEntryType"))
            {
               Form_EventLogType logTypeForm = new Form_EventLogType();

               logTypeForm.EntryType = rule.LogEntryType;
               if (logTypeForm.ShowDialog(this) == DialogResult.OK)
               {
                  rule.LogEntryType = logTypeForm.EntryType;
               }
            }
            else if (String.Equals(tagString, "SnmpConfigurations"))
            {
                // set default SNMP configurations if missing
                if (rule.SnmpConfiguration == null)
                    rule.SnmpConfiguration = AlertingDal.SelectSnmpConfiguration(Globals.Repository.Connection.ConnectionString);

                Form_AlertingOptions_Snmp frmSnmpConfigurations = new Form_AlertingOptions_Snmp(rule.SnmpConfiguration);
                if (frmSnmpConfigurations.ShowDialog(this) == DialogResult.OK)
                    rule.SnmpConfiguration = frmSnmpConfigurations.SnmpConfiguration;
            }
            UpdateRule();
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
             UpdateRule();
         }
      }  

		private void MouseMove_rtfRuleDetails(object sender, MouseEventArgs e)
		{
			if(_link != null && Globals.isAdmin)
			{
				LinkSegment seg = _link.LinkHitTest(e.X, e.Y, _rtfRuleDetails, _rtfGraphics) ;
				if(seg != null && seg.Tag != null)
				{
					_rtfRuleDetails.Cursor = Cursors.Hand ;
					Cursor.Current = Cursors.Hand ;
				}
				else
				{
					_rtfRuleDetails.Cursor = Cursors.Default ;
					Cursor.Current = Cursors.Default ;
				}
			}
		}
		
			
		#endregion //Events

		#region Actions

      private EventRule GetActiveAlertRule()
      {
         if (_grid.Selected.Rows.Count <= 0)
            return null;
         else
         {
            UltraGridRow gridRow = _grid.Selected.Rows[0];
            UltraDataRow dataRow = gridRow.ListObject as UltraDataRow;
            return dataRow != null ? dataRow.Tag as EventRule : null;
         }
      }
      
      private void UpdateRule()
		{
			EventRule rule = GetActiveAlertRule();

         switch (rule.RuleType)
         {
            case EventType.SqlServer:
               try
               {
                  if (AlertingDal.UpdateAlertRule(rule as AlertRule, _configuration.ConnectionString) != 1)
                  {
                     MessageBox.Show(this, "Unable to update the alert rule.", "Error");
                     return;
                  }
                  if (!rule.IsValid)
                  {
                     MessageBox.Show(this, CoreConstants.Exception_InvalidAlertRule, "Invalid Alert Rule", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                  }
               }
               catch (Exception e)
               {
                  ErrorLog.Instance.Write("Unable to update alert rule", e, true);
                  MessageBox.Show(this,
                     String.Format("Unable to update the alert rule.\r\nMessage: {0}", e.Message), "Error");
                  return;
               } 
               break;
            case EventType.Status:
               try
               {
                  if (AlertingDal.UpdateAlertRule(rule as StatusAlertRule, _configuration.ConnectionString) != 1)
                  {
                     MessageBox.Show(this, "Unable to update the alert rule.", "Error");
                     return;
                  }
               }
               catch (Exception e)
               {
                  ErrorLog.Instance.Write("Unable to update alert rule", e, true);
                  MessageBox.Show(this,
                     String.Format("Unable to update the alert rule.\r\nMessage: {0}", e.Message), "Error");
                  return;
               }
               break;
            case EventType.Data:
               try
               {
                  if (AlertingDal.UpdateAlertRule(rule as DataAlertRule, _configuration.ConnectionString) != 1)
                  {
                     MessageBox.Show(this, "Unable to update the alert rule.", "Error");
                     return;
                  }
               }
               catch (Exception e)
               {
                  ErrorLog.Instance.Write("Unable to update alert rule", e, true);
                  MessageBox.Show(this,
                     String.Format("Unable to update the alert rule.\r\nMessage: {0}", e.Message), "Error");
                  return;
               }
               break;
         }
         string oldRuleText = _rtfRuleDetails.Text.Replace("\n", "\r\n") ;

         switch (rule.RuleType)
         {
            case EventType.SqlServer:
               UpdateRuleText(rule as AlertRule);
               break;
            case EventType.Status:
               UpdateRuleText(rule as StatusAlertRule);
               break;
            case EventType.Data:
               UpdateRuleText(rule as DataAlertRule);
               break;
         }
         string logString = String.Format("Name:  {0}\r\nDescription:  {1}\r\n\r\nPrevious Rule:  {2}\r\n\r\nNew Rule:  {3}",
            rule.Name, rule.Description, oldRuleText, _rtfRuleDetails.Text.Replace("\n", "\r\n")) ;
         LogRecord.WriteLog(Globals.Repository.Connection, LogType.AlertRuleModified, logString) ;
         
         UltraGridRow row = _grid.Selected.Rows[0] ;
         if(row != null)
         {
            UltraDataRow dataRow = row.ListObject as UltraDataRow ;
            
            switch (rule.RuleType)
            {
               case EventType.SqlServer:
                  UpdateRowValues(dataRow, rule as AlertRule);
                  break;
               case EventType.Status:
                  UpdateRowValues(dataRow, rule as StatusAlertRule);
                  break;
               case EventType.Data:
                  UpdateRowValues(dataRow, rule as DataAlertRule);
                  break;
            }
         }
         _mainForm.UpdateAlertRules() ;
		}

      private void UpdateRuleText(AlertRule rule)
      {
         _rtfRuleDetails.Rtf = AlertUIHelper.GenerateRuleDescription(rule, _configuration, out _link) ;
      }

      private void UpdateRuleText(StatusAlertRule rule)
      {
         _rtfRuleDetails.Rtf = AlertUIHelper.GenerateRuleDescription(rule, out _link);
      }

      private void UpdateRuleText(DataAlertRule rule)
      {
         _rtfRuleDetails.Rtf = AlertUIHelper.GenerateRuleDescription(rule, out _link);
      }

      public void NewAlertRule()
      {
         NewAlertRule(null);
      }

      public void NewAlertRuleFromTemplate()
      {
         EventRule original = GetActiveAlertRule();

         if (original != null)
         {
            switch (original.RuleType)
            {
               case EventType.SqlServer:
                  AlertRule newRule = ((AlertRule)original).Clone();
                  newRule.Id = AlertingConfiguration.NULL_ID;
                  NewAlertRule(newRule);
                  break;
               case EventType.Status:
                  StatusAlertRule newStatusRule = ((StatusAlertRule)original).Clone();
                  newStatusRule.Id = AlertingConfiguration.NULL_ID;
                  NewStatusAlertRule(newStatusRule);
                  break;
               case EventType.Data:
                  DataAlertRule newDataRule = ((DataAlertRule)original).Clone();
                  newDataRule.Id = AlertingConfiguration.NULL_ID;
                  NewDataAlertRule(newDataRule);
                  break;
            }
         }
      }

	   private void NewAlertRule(AlertRule template)
		{
         _mainForm.NewAlertRule(template);
		}

      public void NewStatusAlertRule()
      {
         NewStatusAlertRule(null);
      }

      public void NewStatusAlertRule(StatusAlertRule rule)
      {
         _mainForm.NewAlertRule(rule);
      }

      public void NewDataAlertRule()
      {
         NewDataAlertRule(null);
      }

      public void NewDataAlertRule(DataAlertRule rule)
      {
         _mainForm.NewAlertRule(rule);
      }

      public override void Delete()
      {
         DeleteSelectedRule() ;
      }

		private void DeleteSelectedRule()
		{
         UltraGridRow row = _grid.Selected.Rows[0];
         if (row == null)
            return;
         UltraDataRow dataRow = row.ListObject as UltraDataRow;
         EventRule rule = dataRow.Tag as EventRule;

			if(MessageBox.Show(this, String.Format("Are you sure you want to delete '{0}' rule?", rule.Name),
				"Delete Alert Rule", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
			{
            int result ;

            result = AlertingDal.DeleteAlertRule(rule, _configuration.ConnectionString);

				if (result == 1)
				{
               string logString = String.Format("Name:  {0}\r\nDescription:  {1}\r\n\r\nRule:  {2}",  rule.Name, 
                                                                                                      rule.Description, 
                                                                                                      _rtfRuleDetails.Text.Replace("\n", "\r\n")) ;
               LogRecord.WriteLog(Globals.Repository.Connection, LogType.AlertRuleRemoved, logString) ;

               // This remove does not trigger a selected row change event
               _dsAlertRules.Rows.Remove(dataRow);

               if (_dsAlertRules.Rows.Count > 0)
               {
                  _grid.Rows[0].Selected = true;
               }
               else
               {
                  _grid.Selected.Rows.Clear();
               }
               _mainForm.UpdateAlertRules() ;
            }
				else
				{
					MessageBox.Show(this, "Failed to delete alert rule.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) ;
				}
			}
		}

		private AlertRule EditAlertRule(AlertRule rule)
		{
			AlertRule clonedRule = rule.Clone() ;
			Form_AlertRuleWizard wizard = new Form_AlertRuleWizard(clonedRule, _configuration);

            if (wizard.ShowDialog(this) == DialogResult.OK)
			{
            try
            {
               if (AlertingDal.UpdateAlertRule(clonedRule, _configuration.ConnectionString) != 1)
               {
                  MessageBox.Show(this, "Unable to update the alert rule.", "Error") ;
                  return rule ;
               }
               if (!wizard.Rule.IsValid)
               {
                  MessageBox.Show(this, CoreConstants.Exception_InvalidAlertRule, "Invalid Alert Rule", MessageBoxButtons.OK, MessageBoxIcon.Warning) ;
               }
            }
            catch (Exception e)
            {
               ErrorLog.Instance.Write("Unable to update alert rule", e, true) ;
               MessageBox.Show(this, String.Format("Unable to update the alert rule.\r\nMessage: {0}", e.Message), "Error") ;
               return rule ;
            }
            clonedRule.Dirty = false ;
            string oldRuleText = _rtfRuleDetails.Text.Replace("\n", "\r\n") ;
            UpdateRuleText(clonedRule) ;
            string logString = String.Format("Name:  {0}\r\nDescription:  {1}\r\n\r\nPrevious Rule:  {2}\r\n\r\nNew Rule:  {3}",
               clonedRule.Name, clonedRule.Description, oldRuleText, _rtfRuleDetails.Text.Replace("\n", "\r\n")) ;
            LogRecord.WriteLog(Globals.Repository.Connection, LogType.AlertRuleModified, logString) ;
            _mainForm.UpdateAlertRules() ;
            return clonedRule ;
			}
			return rule ;
		}

		private StatusAlertRule EditAlertRule(StatusAlertRule rule)
		{
         StatusAlertRule clonedRule = rule.Clone();
         Form_AddStatusAlert wizard = new Form_AddStatusAlert(clonedRule, AlertingDal.SelectStatusRuleNames(_configuration.ConnectionString), false);

			if (wizard.ShowDialog(this) == DialogResult.OK)
			{
            try
            {
               if (AlertingDal.UpdateAlertRule(clonedRule, _configuration.ConnectionString) != 1)
               {
                  MessageBox.Show(this, "Unable to update the alert rule.", "Error") ;
                  return rule ;
               }
            }
            catch (Exception e)
            {
               ErrorLog.Instance.Write("Unable to update alert rule", e, true) ;
               MessageBox.Show(this, String.Format("Unable to update the alert rule.\r\nMessage: {0}", e.Message), "Error") ;
               return rule ;
            }
            clonedRule.Dirty = false ;
            string oldRuleText = _rtfRuleDetails.Text.Replace("\n", "\r\n") ;
            UpdateRuleText(clonedRule) ;
            string logString = String.Format("Name:  {0}\r\nDescription:  {1}\r\n\r\nPrevious Rule:  {2}\r\n\r\nNew Rule:  {3}",
               clonedRule.Name, clonedRule.Description, oldRuleText, _rtfRuleDetails.Text.Replace("\n", "\r\n")) ;
            LogRecord.WriteLog(Globals.Repository.Connection, LogType.AlertRuleModified, logString) ;
            _mainForm.UpdateAlertRules() ;
            return clonedRule ;
			}
			return rule ;
		}

      private DataAlertRule EditAlertRule(DataAlertRule rule)
      {
         DataAlertRule clonedRule = rule.Clone();
         Form_AddDataAlert wizard = new Form_AddDataAlert(clonedRule, AlertingDal.SelectDataRuleNames(_configuration.ConnectionString), false);

         if (wizard.ShowDialog(this) == DialogResult.OK)
         {
            try
            {
                if (AlertingDal.UpdateAlertRule(clonedRule, _configuration.ConnectionString) != 1)
                {
                    MessageBox.Show(this, "Unable to update the alert rule.", "Error");
                    return rule;
                }
                else
                {
                    clonedRule.ClearAllRemovedCondiions();
                }
               if (!wizard.DataRule.IsRuleValid)
               {
                  MessageBox.Show(this, CoreConstants.Exception_InvalidAlertRule, "Invalid Alert Rule", MessageBoxButtons.OK, MessageBoxIcon.Warning);
               }
            }
            catch (Exception e)
            {
               ErrorLog.Instance.Write("Unable to update data alert rule", e, true);
               MessageBox.Show(this, String.Format("Unable to update the data alert rule.\r\nMessage: {0}", e.Message), "Error");
               return rule;
            }
            clonedRule.Dirty = false;
            string oldRuleText = _rtfRuleDetails.Text.Replace("\n", "\r\n");
            UpdateRuleText(clonedRule);
            string logString = String.Format("Name:  {0}\r\nDescription:  {1}\r\n\r\nPrevious Rule:  {2}\r\n\r\nNew Rule:  {3}",
               clonedRule.Name, clonedRule.Description, oldRuleText, _rtfRuleDetails.Text.Replace("\n", "\r\n"));
            LogRecord.WriteLog(Globals.Repository.Connection, LogType.AlertRuleModified, logString);
            _mainForm.UpdateAlertRules();
            return clonedRule;
         }
         return rule;
      }
      
      public override void RefreshView()
		{
         _grid.BeginUpdate();
         _dsAlertRules.Rows.Clear();

         if (_displayEventRules)
         {
            List<AlertRule> rules = AlertingDal.SelectAlertRules(_configuration.ConnectionString);

            foreach (AlertRule rule in rules)
            {
               UltraDataRow row = _dsAlertRules.Rows.Add();
               UpdateRowValues(row, rule);
            }
         }

         if (_displayStatusRules)
         {
            List<StatusAlertRule> statusRules = AlertingDal.SelectStatusAlertRules(_configuration.ConnectionString);

            foreach (StatusAlertRule rule in statusRules)
            {
               UltraDataRow row = _dsAlertRules.Rows.Add();
               UpdateRowValues(row, rule);
            }
         }

         if (_displayDataRules)
         {
            List<DataAlertRule> dataRules = AlertingDal.SelectDataAlertRules(_configuration.ConnectionString);

            foreach (DataAlertRule rule in dataRules)
            {
               UltraDataRow row = _dsAlertRules.Rows.Add();
               UpdateRowValues(row, rule);
            }
         }
         _grid.EndUpdate();

         if (_dsAlertRules.Rows.Count > 0)
            _grid.Rows[0].Selected = true;
         UpdateMenuFlags();
      }

      private void UpdateRowValues(UltraDataRow row, AlertRule rule)
		{
			Image img ;
			if (rule.IsValid)
			{
				if(rule.Enabled)
					img = GUI.Properties.Resources.AlertRule_16 ;
				else
               img = GUI.Properties.Resources.AlertRulesDisabled_16;
			}
			else
            img = GUI.Properties.Resources.AlertRuleIgnored_16;
			row["Icon"] = img ;
         if(rule.IsValid)
			   row["Rule"] = rule.Name ;
         else
            row["Rule"] = rule.Name + " (Invalid Rule)";
         row["Description"] = rule.Description ;
			row["Server"] = rule.TargetInstanceList ;
			row["Level"] = rule.Level.ToString() ;
			row["Email"] = rule.HasEmailAction ? "Yes" : "No" ;
         row["EventLog"] = rule.HasLogAction ? "Yes" : "No" ;
         row["RuleType"] = "Event";
          row["SnmpTrap"] = rule.HasSnmpTrapAction ? "Yes" : "No";
			row.Tag = rule ;
		}

      private void UpdateRowValues(UltraDataRow row, StatusAlertRule rule)
      {
         Image img;

         if (rule.Enabled)
         {
            img = GUI.Properties.Resources.AlertRule_16;
         }
         else
         {
            img = GUI.Properties.Resources.AlertRulesDisabled_16;
         }
         row["Icon"] = img;
         row["Rule"] = rule.Name;
         row["Description"] = rule.Description;
         row["Server"] = rule.TargetInstanceList;
         row["Level"] = rule.Level.ToString();
         row["Email"] = rule.HasEmailAction ? "Yes" : "No";
         row["EventLog"] = rule.HasLogAction ? "Yes" : "No";
         row["RuleType"] = "Status";
         row["SnmpTrap"] = rule.HasSnmpTrapAction ? "Yes" : "No";
         row.Tag = rule;
      }

      private void UpdateRowValues(UltraDataRow row, DataAlertRule rule)
      {
         Image img;

         if (rule.IsRuleValid)
         {
            if (rule.Enabled)
               img = GUI.Properties.Resources.AlertRule_16;
            else
               img = GUI.Properties.Resources.AlertRulesDisabled_16;
         }
         else
            img = GUI.Properties.Resources.AlertRuleIgnored_16;
         row["Icon"] = img;

         if (rule.IsRuleValid)
            row["Rule"] = rule.Name;
         else
            row["Rule"] = rule.Name + " (Invalid Rule)";
         row["Description"] = rule.Description;
         row["Server"] = rule.TargetInstanceList;
         row["Level"] = rule.Level.ToString();
         row["Email"] = rule.HasEmailAction ? "Yes" : "No";
         row["EventLog"] = rule.HasLogAction ? "Yes" : "No";
         row["RuleType"] = "Data";
         row["SnmpTrap"] = rule.HasSnmpTrapAction ? "Yes" : "No";
         row.Tag = rule;
      }

		#endregion  // Actions

      //-------------------------------------------------------------
      // UpdateMenuFlags
      //--------------------------------------------------------------
      private void UpdateMenuFlags()
      {
         EventRule rule = GetActiveAlertRule() ;
         
         if (rule != null)
         {
            SetMenuFlag(CMMenuItem.UseRuleAsTemplate, Globals.isAdmin);

            if (rule.RuleType == EventType.SqlServer)
            {
               SetMenuFlag(CMMenuItem.EnableRule, Globals.isAdmin && rule.IsValid && !rule.Enabled);
               SetMenuFlag(CMMenuItem.DisableRule, Globals.isAdmin && rule.IsValid && rule.Enabled);
            }
            else
            {
               SetMenuFlag(CMMenuItem.EnableRule, Globals.isAdmin && !rule.Enabled);
               SetMenuFlag(CMMenuItem.DisableRule, Globals.isAdmin && rule.Enabled);
            }
            SetMenuFlag(CMMenuItem.Delete, Globals.isAdmin);
            SetMenuFlag(CMMenuItem.Properties, true);
         }
         else
         {
            SetMenuFlag(CMMenuItem.UseRuleAsTemplate, false);
            SetMenuFlag(CMMenuItem.EnableRule, false);
            SetMenuFlag(CMMenuItem.DisableRule, false);
            SetMenuFlag(CMMenuItem.Delete, false);
            SetMenuFlag(CMMenuItem.Properties, false);
         }

         if (_dsAlertRules.Rows.Count > 0)
            SetMenuFlag(CMMenuItem.ExportAlertRules);
         else
            SetMenuFlag(CMMenuItem.ExportAlertRules, false);
      }

      public override void Enable(bool flag)
      {
         base.Enable(flag);
         UltraGridRow row = _grid.Selected.Rows[0];
         if (row == null)
            return;
         UltraDataRow dataRow = row.ListObject as UltraDataRow;
         EventRule rule = dataRow.Tag as EventRule;
         rule.Enabled = flag;
         int result = AlertingDal.UpdateAlertRuleEnabled(rule, _configuration.ConnectionString);

         if (result == 1)
         {
            switch (rule.RuleType)
            {
               case EventType.SqlServer:
                  UpdateRowValues(dataRow, rule as AlertRule);
                  break;
               case EventType.Status:
                  UpdateRowValues(dataRow, rule as StatusAlertRule);
                  break;
               case EventType.Data:
                  UpdateRowValues(dataRow, rule as DataAlertRule);
                  break;
            }

            string logString = String.Format("Name:  {0}\r\nDescription:  {1}\r\n\r\nRule:  {2}",
               rule.Name, rule.Description, _rtfRuleDetails.Text.Replace("\n", "\r\n"));
            if (rule.Enabled)
               LogRecord.WriteLog(Globals.Repository.Connection, LogType.AlertRuleEnabled, logString);
            else
               LogRecord.WriteLog(Globals.Repository.Connection, LogType.AlertRuleDisabled, logString);
            UpdateMenuFlags();
            _mainForm.UpdateAlertRules();
         }
         else
         {
            MessageBox.Show(this, "Failed to modify alert rule.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
         }
      }

      public override void CollapseAll()
      {
         _grid.Rows.CollapseAll(true) ;
      }
      
      public override void ExpandAll()
      {
         _grid.Rows.ExpandAll(true) ;
      }
      
      public override void HelpOnThisWindow()
      {
         HelpAlias.ShowHelp(this,HelpAlias.SSHELP_AlertRulesView);
      }

      public override void Properties()
      {
         EditSelectedRule() ;
      }

      protected override void OnShowGroupByChanged(ToggleChangedEventArgs e)
      {
         base.OnShowGroupByChanged(e);

         if (e.Enabled)
            _grid.DisplayLayout.ViewStyleBand = ViewStyleBand.OutlookGroupBy;
         else
            _grid.DisplayLayout.ViewStyleBand = ViewStyleBand.Horizontal;
         SetMenuFlag(CMMenuItem.Collapse, e.Enabled);
         SetMenuFlag(CMMenuItem.Expand, e.Enabled);
      }

      private void EditSelectedRule()
      {
         UltraGridRow row = _grid.Selected.Rows[0];
         if (row == null)
            return;
         UltraDataRow dataRow = row.ListObject as UltraDataRow;
         EventRule rule = dataRow.Tag as EventRule;

         switch (rule.RuleType)
         {
            case EventType.SqlServer:
               AlertRule editedRule = EditAlertRule(dataRow.Tag as AlertRule);
               row.Tag = editedRule;
               UpdateRowValues(dataRow, editedRule);
               break;
            case EventType.Status:
               StatusAlertRule editedStatusRule = EditAlertRule(dataRow.Tag as StatusAlertRule);
               row.Tag = editedStatusRule;
               UpdateRowValues(dataRow, editedStatusRule);
               break;
            case EventType.Data:
               DataAlertRule editedDataRule = EditAlertRule(dataRow.Tag as DataAlertRule);
               row.Tag = editedDataRule;
               UpdateRowValues(dataRow, editedDataRule);
               break;
         }
         UpdateMenuFlags();
      }

      private void KeyDown_grid(object sender, KeyEventArgs e)
      {
         if (_grid.Selected.Rows.Count > 0)
         {
            if (e.KeyCode == Keys.Delete)
               DeleteSelectedRule();
            else if (e.KeyCode == Keys.Enter)
               EditSelectedRule();
         }
      }

      private void DoubleClickRow_grid(object sender, DoubleClickRowEventArgs e)
      {
         if (_grid.Selected.Rows.Count > 0)
         {
            EditSelectedRule();
         }
      }

      private void MouseDown_grid(object sender, MouseEventArgs e)
      {
         UIElement elementMain;
         UIElement elementUnderMouse;

         elementMain = _grid.DisplayLayout.UIElement;
         elementUnderMouse = elementMain.ElementFromPoint(e.Location);
         if (elementUnderMouse != null)
         {
            UltraGridCell cell = elementUnderMouse.GetContext(typeof(UltraGridCell)) as UltraGridCell;
            if (cell != null)
            {
               if (!cell.Row.Selected)
               {
                  if (e.Button == MouseButtons.Right)
                  {
                     _grid.Selected.Rows.Clear();
                     cell.Row.Selected = true;
                     _grid.ActiveRow = cell.Row;
                  }
               }
            }
            else
            {
               HeaderUIElement he = elementUnderMouse.GetAncestor(typeof(HeaderUIElement)) as HeaderUIElement;
               if (he == null)
               {
                  _grid.Selected.Rows.Clear();
                  _grid.ActiveRow = null;
               }
            }
         }
      }

      private void AfterSelectChange_grid(object sender, AfterSelectChangeEventArgs e)
      {
         EventRule rule = GetActiveAlertRule();

         if (rule == null)
         {
            _rtfRuleDetails.Rtf = "";
         }
         else
         {
            switch (rule.RuleType)
            {
               case EventType.SqlServer:
                  _rtfRuleDetails.Rtf = AlertUIHelper.GenerateRuleDescription(rule as AlertRule, _configuration, out _link);
                  break;
               case EventType.Status:
                  _rtfRuleDetails.Rtf = AlertUIHelper.GenerateRuleDescription(rule as StatusAlertRule, out _link);
                  break;

               case EventType.Data:
                  _rtfRuleDetails.Rtf = AlertUIHelper.GenerateRuleDescription(rule as DataAlertRule, out _link);
                  break;
            }
         }
         UpdateMenuFlags();
      }

      private void BeforeToolDropdown_toolbarsManager(object sender, Infragistics.Win.UltraWinToolbars.BeforeToolDropdownEventArgs e)
      {

         EventRule rule = GetActiveAlertRule();

         if (rule != null && Globals.isAdmin)
         {
            if (rule.RuleType == EventType.SqlServer)
            {
               _toolbarsManager.Tools["enableRule"].SharedProps.Enabled = rule.IsValid && !rule.Enabled;
               _toolbarsManager.Tools["disableRule"].SharedProps.Enabled = rule.IsValid && rule.Enabled;
            }
            else
            {
               _toolbarsManager.Tools["enableRule"].SharedProps.Enabled = !rule.Enabled;
               _toolbarsManager.Tools["disableRule"].SharedProps.Enabled = rule.Enabled;
            }
         }
         else
         {
            _toolbarsManager.Tools["enableRule"].SharedProps.Enabled = false ;
            _toolbarsManager.Tools["disableRule"].SharedProps.Enabled = false ;
         }
         _toolbarsManager.Tools["newRule"].SharedProps.Enabled = Globals.isAdmin;
         _toolbarsManager.Tools["newStatusRule"].SharedProps.Enabled = Globals.isAdmin;
         _toolbarsManager.Tools["newDataRule"].SharedProps.Enabled = Globals.isAdmin;
         _toolbarsManager.Tools["ruleTemplate"].SharedProps.Enabled = (rule != null) && Globals.isAdmin;
         _toolbarsManager.Tools["delete"].SharedProps.Enabled = (rule != null) && Globals.isAdmin;
         _toolbarsManager.Tools["properties"].SharedProps.Enabled = (rule != null);
      }

      private void ToolClick_toolbarsManager(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
      {
         switch(e.Tool.Key)
         {
            case "newRule":
               NewAlertRule();
               break;
            case "newStatusRule":
               NewStatusAlertRule();
               break;
            case "newDataRule":
               NewDataAlertRule();
               break;
            case "ruleTemplate":
               NewAlertRuleFromTemplate() ;
               break;
            case "enableRule":
               Enable(true);
               break;
            case "disableRule":
               Enable(false);
               break;
            case "delete":
               DeleteSelectedRule();
               break;
            case "refresh":
               RefreshView();
               break;
            case "properties":
               EditSelectedRule();
               break;
         }
      }
   }
}
