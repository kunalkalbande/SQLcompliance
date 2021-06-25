using System;
using System.Drawing;
using System.Windows.Forms;
using Idera.SQLcompliance.Application.GUI.Properties ;
using Idera.SQLcompliance.Core.Rules;
using Idera.SQLcompliance.Core.Rules.Alerts ;
using Idera.SQLcompliance.Core ;
using Idera.SQLcompliance.Core.Rules.Filters;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
   /// <summary>
   /// Summary description for Form_AlertRuleWizard.
   /// </summary>
   public partial class Form_EventFilterWizard : System.Windows.Forms.Form
   {
      private static readonly string[] _wizardTitles = new string[] { "SQL Server Event Type",
                                                                       "SQL Server Object Type",
                                                                       "SQL Server Event Source", 
                                                                       "Finish Event Filter"};
      private static readonly string[] _wizardDescriptions = new string[] { "This window allows you to select the type of events you want to filter.",
                                                                             "This window allows you to select the objects associated with filtered events.",
                                                                             "This window allows you to select the users and applications that initiated the filtered events.",
                                                                             "This window allows you to specify the general properties for this filter."} ;

      private int _pageIndex ;
      private int _pageCount ;
      private LinkString _link ;
      private Graphics _rtfGraphics ;
      private bool _isInternalUpdate ;
      private bool _isEdit ;
      private EventFilter _filter ;
      private EventCondition _eventScopeCondition ;
      private EventCondition _eventTypeCondition ;
      private EventCondition _eventCategoryCondition ;
      private FiltersConfiguration _configuration ;

      public Form_EventFilterWizard(FiltersConfiguration configuration) : this(null, configuration)
      {
      }
			
			
      public Form_EventFilterWizard(EventFilter filter, FiltersConfiguration configuration)
      {
         InitializeComponent();

         this.Icon = Resources.SQLcompliance_product_ico;

         _configuration = configuration ;

         Initialize() ;

         _pageIndex = 0 ;
         _pageCount = 4 ;

         if(filter == null)
         {
            Text = "New Event Filter" ;
            _filter = new EventFilter() ;
            _filter.Enabled = true ;
            _isEdit = false ;
         }
         else
         {
            _filter = filter ;
            foreach(EventCondition condition in _filter.Conditions)
            {
               if(condition.TargetEventField.Id == (int)AlertableEventFields.eventCategory)
               {
                  _linkCategory.Enabled = true ;
                  _eventCategoryCondition = condition ;
                  _eventScopeCondition = condition ;
               }
               else if(condition.TargetEventField.Id == (int)AlertableEventFields.eventType)
               {
                  _linkEventType.Enabled = true ;
                  _eventTypeCondition = condition ;
                  _eventScopeCondition = condition ;
               }
            }

            if(filter.Id == EventRule.NULL_ID)
            {
               // Creating a filter from a template
               Text = "New Event Filter" ;
               _isEdit = false ;
            }
            else
            {
               Text = "Edit Event Filter" ;
               _isEdit = true ;
               _btnFinish.Enabled = Globals.isAdmin ;
            }
         }
         
         _lblTitle.Text = _wizardTitles[_pageIndex] ;
         _lblDescription.Text = _wizardDescriptions[_pageIndex] ;
         UpdateData(_pageIndex) ;
         _rtfGraphics = _rtfFilterType.CreateGraphics() ;
      }

      private void Initialize()
      {
         try
         {
            EventField eventCategoryField = _configuration.LookupAlertableEventField(AlertableEventFields.eventCategory) ;
            EventField eventTypeField = _configuration.LookupAlertableEventField(AlertableEventFields.eventType) ;

            _eventCategoryCondition = new EventCondition(eventCategoryField) ;
            _eventCategoryCondition.IntegerArray = new int[] {4} ;
            CMEventCategory evtCategory = _configuration.LookupCategory(4) ;
            _linkCategory.Text = evtCategory.Name ;

            _eventTypeCondition = new EventCondition(eventTypeField) ;
            _eventTypeCondition.IntegerArray = new int[] {101} ;
            CMEventType evType = _configuration.LookupEventType(101, EventType.SqlServer) ;
            _linkEventType.Text = evType.Name ;

            CheckedListEventField field ;

            // Scope page
            field = new CheckedListEventField() ;
            field.Field = _configuration.LookupAlertableEventField(AlertableEventFields.serverName) ;
            field.ListEntry = "SQL Server" ;
            _listBoxTargetObjects.Items.Add(field) ;

            field = new CheckedListEventField() ;
            field.Field = _configuration.LookupAlertableEventField(AlertableEventFields.databaseName) ;
            field.ListEntry = "Database Name" ;
            _listBoxTargetObjects.Items.Add(field) ;

            field = new CheckedListEventField() ;
            field.Field = _configuration.LookupAlertableEventField(AlertableEventFields.objectName) ;
            field.ListEntry = "Object Name" ;
            _listBoxTargetObjects.Items.Add(field) ;

            // Filters page
            field = new CheckedListEventField() ;
            field.Field = _configuration.LookupAlertableEventField(AlertableEventFields.applicationName) ;
            field.ListEntry = "Application Name" ;
            _listBoxAdditionalFilters.Items.Add(field) ;

            field = new CheckedListEventField() ;
            field.Field = _configuration.LookupAlertableEventField(AlertableEventFields.loginName) ;
            field.ListEntry = "Login Name" ;
            _listBoxAdditionalFilters.Items.Add(field) ;

            field = new CheckedListEventField();
            field.Field = _configuration.LookupAlertableEventField(AlertableEventFields.hostName);
            field.ListEntry = "Hostname";
            _listBoxAdditionalFilters.Items.Add(field);

            field = new CheckedListEventField();
            field.Field = _configuration.LookupAlertableEventField(AlertableEventFields.privilegedUser) ;
            field.ListEntry = "Is Privileged User" ;
            _listBoxAdditionalFilters.Items.Add(field) ;

            field = new CheckedListEventField();
            field.Field = _configuration.LookupAlertableEventField(AlertableEventFields.sessionLoginName);
            field.ListEntry = "Session Login";
            _listBoxAdditionalFilters.Items.Add(field);
         }
         catch(Exception e)
         {
            ErrorLog.Instance.Write("EventFilterWizard:Initialize", e) ;
         }
      }

      public EventFilter Filter
      {
         get { return _filter ; }
      }
      public string FilterText
      {
         get
         {
            RichTextBox temp = GetActiveRtf() ;
            if(temp != null)
               return temp.Text ;
            else
               return "Filter text is currently not available." ;
         }
      }


      private void ShowPage(int pageNumber)
      {
         if(pageNumber < 0 || pageNumber >= _pageCount)
         {
            throw new Exception("Invalid page number"); 
         }
         if(pageNumber == _pageIndex)
            return ;

         if(pageNumber == 0)
         {
            _btnBack.Enabled = false ;
            _btnNext.Enabled = true ;
            _btnFinish.Enabled = _isEdit && Globals.isAdmin ;
         }
         else if(pageNumber == (_pageCount - 1))
         {
            if(_pageCount > 1)
               _btnBack.Enabled = true ;
            _btnFinish.Enabled = Globals.isAdmin;
            _btnNext.Enabled = false;
         }
         else
         {
            _btnBack.Enabled = true ;
            _btnNext.Enabled = true ;
            _btnFinish.Enabled = _isEdit && Globals.isAdmin;
         }

         _lblTitle.Text = _wizardTitles[pageNumber] ;
         _lblDescription.Text = _wizardDescriptions[pageNumber] ;

         switch(pageNumber)
         {
            case 0:
               UpdateData(0) ;
               _pnlFilterType.Show();
               break ;
            case 1:
               UpdateData(1) ;
               _pnlTargetObjects.Show() ;
               break ;
            case 2:
               UpdateData(2) ;
               _pnlAdditionalFilters.Show() ;
               break ;
            case 3:
               UpdateData(3) ;
               _pnlSummary.Show() ;
               break ;
         }

         switch(_pageIndex)
         {
            case 0:
               _pnlFilterType.Hide() ;
               break ;
            case 1:
               _pnlTargetObjects.Hide() ;
               break ;
            case 2:
               _pnlAdditionalFilters.Hide() ;
               break ;
            case 3:
               _pnlSummary.Hide() ;
               break ;
         }
         _pageIndex = pageNumber ;
         if(_pageIndex == (_pageCount - 1))
         {
            if(Globals.isAdmin)
               _btnFinish.Focus() ;
            else
               _btnCancel.Focus() ;
         }
         else
            _btnNext.Focus() ;
      }

      /// <summary>
      /// This function updates context-sensitive UI components in the wizard
      /// based upon the information availabled in the AlertRule object.
      /// </summary>
      /// <param name="page"></param>
      private void UpdateData(int page)
      {
         _isInternalUpdate = true ;
         try
         {
            switch(page)
            {
               case 0:
                  _rbAllEvents.Checked = true ;
                  foreach(EventCondition condition in _filter.Conditions)
                  {
                     if(condition.FieldId == (int)AlertableEventFields.eventCategory)
                     {
                        _rbCategory.Checked = true ;
                        CMEventCategory catType = _configuration.LookupCategory(condition.IntegerArray[0]) ;
                        _linkCategory.Text = catType.Name ;
                     }
                     else if(condition.FieldId == (int)AlertableEventFields.eventType && condition.Inclusive)
                     {
                        _rbEventType.Checked = true ;
                        CMEventType evType = _configuration.LookupEventType(condition.IntegerArray[0], EventType.SqlServer) ;
                        _linkEventType.Text = evType.Name ;
                     }
                  }
                  break ;
               case 1:
                  _listBoxTargetObjects.SetItemChecked(0, false) ;
                  _listBoxTargetObjects.SetItemChecked(1, false) ;
                  _listBoxTargetObjects.SetItemChecked(2, false) ;

                  if(_filter.TargetInstances.Length == 0 ||
                     (_filter.TargetInstances.Length >= 1 && _filter.TargetInstances[0] != "<ALL>"))
                     _listBoxTargetObjects.SetItemChecked(0, true) ;
                  foreach(EventCondition condition in _filter.Conditions)
                  {
                     switch(condition.FieldId)
                     {
                        case (int)AlertableEventFields.databaseName:
                           _listBoxTargetObjects.SetItemChecked(1, true) ;
                           break ;
                        case (int)AlertableEventFields.objectName:
                           _listBoxTargetObjects.SetItemChecked(2, true) ;
                           break ;
                     }
                  }
                  break ;
               case 2:
                  _listBoxAdditionalFilters.SetItemChecked(0, false) ;
                  _listBoxAdditionalFilters.SetItemChecked(1, false) ;
                  _listBoxAdditionalFilters.SetItemChecked(2, false) ;
                  _listBoxAdditionalFilters.SetItemChecked(3, false);

                  foreach(EventCondition condition in _filter.Conditions)
                  {
                     switch(condition.FieldId)
                     {
                        case (int)AlertableEventFields.applicationName:
                           _listBoxAdditionalFilters.SetItemChecked(0, true) ;
                           break ;
                        case (int)AlertableEventFields.loginName:
                           _listBoxAdditionalFilters.SetItemChecked(1, true) ;
                           break ;
                        case (int)AlertableEventFields.hostName:
                           _listBoxAdditionalFilters.SetItemChecked(2, true);
                           break;
                        case (int)AlertableEventFields.privilegedUser:
                           _listBoxAdditionalFilters.SetItemChecked(3, true) ;
                           break ;
                     }
                  }
                  break ;
               case 3:
                  _tbFilterName.Text = _filter.Name ;
                  _tbFilterDescription.Text = _filter.Description ;
                  _checkBoxEnableFilter.Checked = _filter.Enabled ;
                  break ;
            }
            UpdateRuleDescription(page) ;
         }
         catch(Exception)
         {
         }
         finally
         {
            _isInternalUpdate = false ;
         }
      }

      private void UpdateRuleDescription(int page)
      {
         RichTextBox rtf = GetActiveRtf(page);

         if(rtf == null)
            return ;

         try
         {
            rtf.Rtf = FilterUIHelper.GenerateFilterDescription(_filter, _configuration, out _link) ;
         }
         catch(Exception e)
         {
            ErrorLog.Instance.Write("UpdateRuleDescription", e) ;
         }
      }
		
      #region Events

      private void Click_btnBack(object sender, System.EventArgs e)
      {
         ShowPage(_pageIndex - 1) ;
      }

      private void Click_btnNext(object sender, System.EventArgs e)
      {
         ShowPage(_pageIndex + 1) ;
      }

      private void ItemCheck_listBoxAdditionalFilters(object sender, System.Windows.Forms.ItemCheckEventArgs e)
      {
         if(_isInternalUpdate)
            return ;
         CheckedListEventField info = (CheckedListEventField)_listBoxAdditionalFilters.Items[e.Index] ;

         if(e.NewValue == CheckState.Unchecked)
            _filter.RemoveCondition(info.Field.ColumnName) ;
         else if(e.NewValue == CheckState.Checked)
         {
            EventCondition newCondition = new EventCondition(info.Field) ;
            _filter.AddCondition(newCondition) ;
         }

         UpdateRuleDescription(_pageIndex) ;
      }

      private void TextChanged_tbFilterName(object sender, System.EventArgs e)
      {
         if(_isInternalUpdate)
            return ;
         _filter.Name = _tbFilterName.Text ;
         UpdateRuleDescription(_pageIndex);
      }

      private void MouseMove_rtfBox(object sender, System.Windows.Forms.MouseEventArgs e)
      {
         RichTextBox rtf = GetActiveRtf() ;

         if(rtf == null || !Globals.isAdmin)
            return ;

         LinkSegment seg = _link.LinkHitTest(e.X, e.Y, rtf, _rtfGraphics) ;
         if(seg != null && seg.Tag != null)
         {
            rtf.Cursor = Cursors.Hand ;
            Cursor.Current = Cursors.Hand ;
         }
         else
         {
            rtf.Cursor = Cursors.Default ;
            Cursor.Current = Cursors.Default ;
         }      
      }

      private void MouseDown_rtfBox(object sender, System.Windows.Forms.MouseEventArgs e)
      {
         LinkSegment link = _link.LinkHitTest(e.X, e.Y, sender as RichTextBox, _rtfGraphics) ;

         if(link == null || link.Tag == null || e.Button != MouseButtons.Left || !Globals.isAdmin)
            return ;
         else if(link.Tag is string)
         {
            string tagString = (string)link.Tag ;

            if(String.Equals(tagString, "TargetInstances"))
            {
               Form_InstanceList frmInstances = new Form_InstanceList() ;
               frmInstances.SelectedInstances = _filter.TargetInstances ;
               if(frmInstances.ShowDialog(this) == DialogResult.OK)
               {
                  _filter.TargetInstances = frmInstances.SelectedInstances ;
                  UpdateRuleDescription(_pageIndex) ;
               }
            }
         }
         else if(link.Tag is EventCondition)
         {
            EventCondition condition = (EventCondition)link.Tag ;

            switch(condition.ConditionType)
            {
               case MatchType.Bool:
                  Form_Boolean boolForm = new Form_Boolean() ;
                  boolForm.BooleanValue = condition.BooleanValue ;
                  boolForm.Directions = "&Select the desired value:" ;
                  boolForm.Title = condition.TargetEventField.DisplayName ;
                  if(boolForm.ShowDialog(this) == DialogResult.OK)
                  {
                     condition.BooleanValue = boolForm.BooleanValue ;
                     UpdateRuleDescription(_pageIndex) ;
                  }
                  break ;
               case MatchType.Integer:
                  if(condition == _eventTypeCondition)
                  {
                     SelectEventType() ;
                  }
                  else
                  {
                     SelectEventCategory() ;
                  }
                  break ;
               case MatchType.String:
                  Form_StringSearch ssForm = new Form_StringSearch(condition.FieldId, false) ;

                  ssForm.StringArray = condition.StringArray ;
                  ssForm.Inclusive = condition.Inclusive ;
                  ssForm.Blanks = condition.Blanks;
                  ssForm.Nulls = condition.Nulls;

                  if(ssForm.ShowDialog(this) == DialogResult.OK)
                  {
                     condition.StringArray = ssForm.StringArray ;
                     condition.Inclusive = ssForm.Inclusive ;
                     condition.Nulls = ssForm.Nulls;
                     condition.Blanks = ssForm.Blanks;
                     UpdateRuleDescription(_pageIndex);
                  }
                  break ;
            }
         }
      }

      private void TextChanged_tbFilterDescription(object sender, System.EventArgs e)
      {
         if(_isInternalUpdate)
            return ;
         _filter.Description = _tbFilterDescription.Text ;
         UpdateRuleDescription(_pageIndex);
      }

      private void CheckedChanged_checkBoxEnableFilter(object sender, System.EventArgs e)
      {
         if(_isInternalUpdate)
            return ;
         if(_checkBoxEnableFilter.Checked)
            _filter.Enabled = true ;
         else
            _filter.Enabled = false ;
      }

      private void LinkClicked_linkEventType(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
      {
         SelectEventType() ;
      }

      private void LinkClicked_linkCategory(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
      {
         SelectEventCategory();
      }

      private void Form_AlertRuleWizard_HelpRequested(object sender, System.Windows.Forms.HelpEventArgs hlpevent)
      {
         string helpTopic = "";
         
         if(!_isEdit)
         {
            switch(_pageIndex)
            {
               case 0:
                  helpTopic = HelpAlias.SSHELP_NewEventFilterWizard_EventType ;
                  break ;
               case 1:
                  helpTopic = HelpAlias.SSHELP_NewEventFilterWizard_ObjectType ;
                  break;
               case 2:
                  helpTopic = HelpAlias.SSHELP_NewEventFilterWizard_EventSource ;
                  break ;
               case 3:
                  helpTopic = HelpAlias.SSHELP_NewEventFilterWizard_Finish ;
                  break ;
            }
         }
         else
         {
            switch(_pageIndex)
            {
               case 0:
                  helpTopic = HelpAlias.SSHELP_EditEventFilterWizard_EventType ;
                  break ;
               case 1:
                  helpTopic = HelpAlias.SSHELP_EditEventFilterWizard_ObjectType ;
                  break;
               case 2:
                  helpTopic = HelpAlias.SSHELP_EditEventFilterWizard_EventSource ;
                  break ;
               case 3:
                  helpTopic = HelpAlias.SSHELP_EditEventFilterWizard_Finish ;
                  break ;
            }
         }
      
         if ( helpTopic != "" ) HelpAlias.ShowHelp(this,helpTopic);
         
         hlpevent.Handled = true;      
      }

      private void ItemCheck_listBoxTargetObjects(object sender, System.Windows.Forms.ItemCheckEventArgs e)
      {
         if(_isInternalUpdate)
            return ;
         CheckedListEventField info = (CheckedListEventField)_listBoxTargetObjects.Items[e.Index] ;

         if(e.NewValue == CheckState.Unchecked)
         {
            // ServerName is a special placeholder for TargetInstances of an Alert Rule
            if(info.Field.Id == (int)AlertableEventFields.serverName)
               _filter.TargetInstances = new string[] {"<ALL>"} ;
            else
               _filter.RemoveCondition(info.Field.ColumnName) ;
         }
         else if(e.NewValue == CheckState.Checked)
         {
            // ServerName is a special placeholder for TargetInstances of an Alert Rule
            if(info.Field.Id == (int)AlertableEventFields.serverName)
            {
               _filter.TargetInstances = new string[] {} ;
            }
            else
            {
               EventCondition newCondition = new EventCondition(info.Field) ;
               _filter.AddCondition(newCondition) ;
            }
         }
         UpdateRuleDescription(_pageIndex) ;
      }

      private void Click_EventTypeRadioButton(object sender, System.EventArgs e)
      {
         if(_isInternalUpdate)
            return ;

         if(_eventScopeCondition != null)
            _filter.RemoveCondition(_eventScopeCondition.TargetEventField.ColumnName) ;

         if(sender == _rbAllEvents)
         {
            _linkCategory.Enabled = false ;
            _linkEventType.Enabled = false ;
         }
         else if(sender == _rbCategory)
         {
            _linkCategory.Enabled = true ;
            _linkEventType.Enabled = false ;
            _filter.AddCondition(_eventCategoryCondition) ;
            _eventScopeCondition = _eventCategoryCondition ;
         }
         else if(sender == _rbEventType)
         {
            _linkCategory.Enabled = false ;
            _linkEventType.Enabled = true ;
            _filter.AddCondition(_eventTypeCondition) ;
            _eventScopeCondition = _eventTypeCondition ;
         }

         UpdateRuleDescription(_pageIndex) ;
      }

      #endregion // Events



      private RichTextBox GetActiveRtf()
      {
         return GetActiveRtf(_pageIndex) ;
      }

      private RichTextBox GetActiveRtf(int page)
      {
         switch(page)
         {
            case 0:
               return _rtfFilterType ;
            case 1:
               return _rtfTargetObjects ;
            case 2:
               return _rtfAdditionalFilters ;
            case 3:
               return _rtfSummary ;
         }
         return null ;
      }

      private bool SelectEventCategory()
      {
         CMEventCategory selectedCateogry = _configuration.LookupCategory(_eventCategoryCondition.IntegerArray[0]) ;
         Form_EventCategorySelector selectorForm = new Form_EventCategorySelector(_configuration.SqlServerCategories, selectedCateogry) ;
         if(selectorForm.ShowDialog(this) == DialogResult.OK)
         {
            selectedCateogry = selectorForm.SelectedCategory ;
            int[] evCategory = new int[1] ;
            evCategory[0] = selectedCateogry.CategoryId ;
            if(evCategory[0] != _eventCategoryCondition.IntegerArray[0])
               _eventCategoryCondition.IntegerArray = evCategory ;
            _linkCategory.Text = selectedCateogry.Name ;
            UpdateRuleDescription(_pageIndex) ;
            return true ;
         }
         return false ;
      }

      private bool SelectEventType()
      {
         CMEventType selectedEvent = _configuration.LookupEventType(_eventTypeCondition.IntegerArray[0], EventType.SqlServer) ;
         CMEventCategory selectedCateogry = _configuration.LookupCategory(selectedEvent.CategoryId) ;
         Form_EventSelector selectorForm = new Form_EventSelector(_configuration.SqlServerCategories, selectedCateogry, selectedEvent) ;
         if(selectorForm.ShowDialog(this) == DialogResult.OK)
         {
            selectedEvent = selectorForm.SelectedEvent ;
            int[] evEventType = new int[1] ;
            evEventType[0] = selectedEvent.TypeId ;
            if(evEventType[0] != _eventTypeCondition.IntegerArray[0])
               _eventTypeCondition.IntegerArray = evEventType ;
            _linkEventType.Text = selectedEvent.Name ;
            UpdateRuleDescription(_pageIndex) ;
            return true ;
         }
         return false; 
      }
   }
}
