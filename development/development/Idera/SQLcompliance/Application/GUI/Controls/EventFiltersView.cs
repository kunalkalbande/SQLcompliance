using System ;
using System.Collections;
using System.Drawing ;
using System.Windows.Forms;
using Idera.SQLcompliance.Application.GUI.Forms ;
using Idera.SQLcompliance.Application.GUI.Helper;
using Idera.SQLcompliance.Core ;
using Idera.SQLcompliance.Core.Rules;
using Idera.SQLcompliance.Core.Rules.Alerts;
using Idera.SQLcompliance.Core.Rules.Filters;
using Infragistics.Win;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;

namespace Idera.SQLcompliance.Application.GUI.Controls
{
	/// <summary>
	/// Summary description for ExcludeFiltersView.
	/// </summary>
	public partial class EventFiltersView : BaseControl
	{ 
      private FiltersConfiguration _configuration ;
      private LinkString _link ;
		private Graphics _rtfGraphics ;

		public EventFiltersView()
		{
			// This call is required by the Windows.Forms Form Designer.
         InitializeComponent();
         GridHelper.ApplyAdminSettings(_grid);

//         SetTitle("Event Filters");
//         SetDescription("This window lists active event filters.  Event filters allow you to filter uninte" +
//   "resting events out of the collected events.");
//         SetHelpLink("Tell me more", HelpAlias.SSHELP_EventFiltersView);

         _rtfGraphics = _rtfFilterDetails.CreateGraphics() ;

         SetMenuFlag(CMMenuItem.Refresh) ;
         SetMenuFlag(CMMenuItem.Collapse) ;
         SetMenuFlag(CMMenuItem.Expand) ;
         SetMenuFlag(CMMenuItem.GroupByColumn) ;
         SetMenuFlag(CMMenuItem.ShowHelp) ;
         SetMenuFlag(CMMenuItem.ImportEventFilters, Globals.isAdmin);
         SetMenuFlag(CMMenuItem.NewEventFilter, Globals.isAdmin);
      }

      public FiltersConfiguration Configuration
      {
         set { _configuration = value ; }
         get { return _configuration ; }
      }

		#region Events

		private void MouseDown_rtfFilterDetails(object sender, System.Windows.Forms.MouseEventArgs e)
		{
         if(_link == null || !Globals.isAdmin)
            return ;

         LinkSegment link = _link.LinkHitTest(e.X, e.Y, sender as RichTextBox, _rtfGraphics) ;
         EventFilter filter = GetActiveFilter() ;

         if(filter == null || link == null || link.Tag == null || e.Button != MouseButtons.Left)
            return ;
         else if(link.Tag is string)
         {
            string tagString = (string)link.Tag ;

            if(String.Equals(tagString, "TargetInstances"))
            {
               Form_InstanceList frmInstances = new Form_InstanceList() ;
               frmInstances.SelectedInstances = filter.TargetInstances ;
               if(frmInstances.ShowDialog(this) == DialogResult.OK)
               {
                  filter.TargetInstances = frmInstances.SelectedInstances ;
                  UpdateFilter() ;
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
                     UpdateFilter() ;
                  }
                  break ;
               case MatchType.Integer:
                  if(condition.TargetEventField.Id == (int)AlertableEventFields.eventCategory)
                  {
                     CMEventCategory selectedCateogry = _configuration.LookupCategory(condition.IntegerArray[0]) ;
                     Form_EventCategorySelector selectorForm = new Form_EventCategorySelector(_configuration.SqlServerCategories, selectedCateogry) ;
                     if(selectorForm.ShowDialog(this) == DialogResult.OK)
                     {
                        int[] newCateogry = new int[1] ;
                        newCateogry[0] = selectorForm.SelectedCategory.CategoryId ;
                        if(newCateogry[0] != condition.IntegerArray[0])
                           condition.IntegerArray = newCateogry ;
                        UpdateFilter() ;
                     }
                  }
                  else
                  {
                     CMEventType selectedEvent = _configuration.LookupEventType(condition.IntegerArray[0], EventType.SqlServer) ;
                     CMEventCategory selectedCateogry = _configuration.LookupCategory(selectedEvent.CategoryId) ;
                     Form_EventSelector selectorForm = new Form_EventSelector(_configuration.SqlServerCategories, selectedCateogry, selectedEvent) ;
                     if(selectorForm.ShowDialog(this) == DialogResult.OK)
                     {
                        int[] newEventType = new int[1] ;
                        newEventType[0] = selectorForm.SelectedEvent.TypeId ;
                        if(newEventType[0] != condition.IntegerArray[0])
                           condition.IntegerArray = newEventType ;
                        UpdateFilter() ;
                     }
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
                     condition.Blanks = ssForm.Blanks;
                     condition.Nulls = ssForm.Nulls;
                     UpdateFilter();
                  }
                  break ;
            }
         }		
      }

		private void MouseMove_rtfFilterDetails(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if(_link != null && Globals.isAdmin)
			{
				LinkSegment seg = _link.LinkHitTest(e.X, e.Y, _rtfFilterDetails, _rtfGraphics) ;
				if(seg != null && seg.Tag != null)
				{
					_rtfFilterDetails.Cursor = Cursors.Hand ;
					Cursor.Current = Cursors.Hand ;
				}
				else
				{
					_rtfFilterDetails.Cursor = Cursors.Default ;
					Cursor.Current = Cursors.Default ;
				}
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

		#endregion //Events

      #region Actions

      private void UpdateFilter()
      {
         EventFilter filter = GetActiveFilter() ;

         try
         {
            if(FiltersDal.UpdateEventFilter(filter, _configuration.ConnectionString) != 1)
            {
               MessageBox.Show(this, "Unable to update the event filter.", "Error") ;
               return ;
            }
            if(!filter.IsValid)
            {
               if(!filter.HasTargetInstances || filter.HasConditions)
                  MessageBox.Show(this, CoreConstants.Exception_IncompleteEventFilter, "Invalid Event Filter", MessageBoxButtons.OK, MessageBoxIcon.Warning) ;
               else
                  MessageBox.Show(this, CoreConstants.Exception_InvalidEventFilter, "Invalid Event Filter", MessageBoxButtons.OK, MessageBoxIcon.Warning) ;
            }
         }
         catch(Exception e)
         {
            ErrorLog.Instance.Write("Unable to update event filter", e, true) ;
            MessageBox.Show(this, 
               String.Format("Unable to update the event filter.\r\nMessage: {0}", e.Message), "Error") ;
            return ;
         }

         string oldFilterText = _rtfFilterDetails.Text.Replace("\n", "\r\n") ;
         UpdateFilterText(filter) ;
         string logString = String.Format("Name:  {0}\r\nDescription:  {1}\r\n\r\nPrevious Filter:  {2}\r\n\r\nNew Filter:  {3}",
            filter.Name, filter.Description, oldFilterText, _rtfFilterDetails.Text.Replace("\n", "\r\n")) ;
         LogRecord.WriteLog(Globals.Repository.Connection, LogType.EventFilterModified, logString) ;

         UltraGridRow row = _grid.Selected.Rows[0];
         if (row != null)
         {
            UltraDataRow dataRow = row.ListObject as UltraDataRow;
            UpdateRowValues(dataRow, filter);
         }
         _mainForm.UpdateEventFilters();
      }

      private void UpdateFilterText(EventFilter filter)
      {
         _rtfFilterDetails.Rtf = FilterUIHelper.GenerateFilterDescription(filter, _configuration, out _link) ;
      }

      private EventFilter GetActiveFilter()
      {
         if (_grid.Selected.Rows.Count <= 0)
            return null;
         else
         {
            UltraGridRow gridRow = _grid.Selected.Rows[0];
            UltraDataRow dataRow = gridRow.ListObject as UltraDataRow;
            return dataRow != null ? dataRow.Tag as EventFilter : null;
         }
      }

      public void NewEventFilter()
      {
         NewEventFilter(null) ;
      }

      public void NewEventFilterFromTemplate()
      {
         EventFilter original = GetActiveFilter();

         if (original != null)
         {
            EventFilter newRule = original.Clone();
            newRule.Id = EventRule.NULL_ID;
            NewEventFilter(newRule);
         }
      }

	   private void NewEventFilter(EventFilter template)
      {
         _mainForm.NewEventFilter(template) ;
      }

      public override void Delete()
      {
         DeleteSelectedFilter() ;
      }

      public override void Enable(bool flag)
      {
         base.Enable(flag);
         UltraGridRow row = _grid.Selected.Rows[0];
         if (row == null)
            return;
         UltraDataRow dataRow = row.ListObject as UltraDataRow;
         EventFilter filter = dataRow.Tag as EventFilter;

         filter.Enabled = flag;
         int result;

         result = FiltersDal.UpdateEventFilterEnabled(filter, _configuration.ConnectionString);
         if (result == 1)
         {
            UpdateRowValues(dataRow, filter);

            string logString = String.Format("Name:  {0}\r\nDescription:  {1}\r\n\r\nFilter:  {2}",
               filter.Name, filter.Description, _rtfFilterDetails.Text.Replace("\n", "\r\n"));
            if (filter.Enabled)
               LogRecord.WriteLog(Globals.Repository.Connection, LogType.EventFilterEnabled, logString);
            else
               LogRecord.WriteLog(Globals.Repository.Connection, LogType.EventFilterDisabled, logString);
            UpdateMenuFlags();
            _mainForm.UpdateEventFilters();
         }
         else
         {
            MessageBox.Show(this, "Failed to modify event filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
         }
      }

      private void DeleteSelectedFilter()
      {
         UltraGridRow row = _grid.Selected.Rows[0];
         if (row == null)
            return;
         UltraDataRow dataRow = row.ListObject as UltraDataRow;
         EventFilter filter = dataRow.Tag as EventFilter;

         if(MessageBox.Show(this, String.Format("Are you sure you want to delete '{0}' filter?", filter.Name),
            "Delete Filter Filter", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
         {
            int result ;

            result = FiltersDal.DeleteEventFilter(filter, _configuration.ConnectionString) ;

            if(result == 1)
            {
               string logString = String.Format("Name:  {0}\r\nDescription:  {1}\r\n\r\nFilter:  {2}",
                  filter.Name, filter.Description, _rtfFilterDetails.Text.Replace("\n", "\r\n")) ;
               LogRecord.WriteLog(Globals.Repository.Connection, LogType.EventFilterRemoved, logString) ;
               _dataSource.Rows.Remove(dataRow);
               if (_dataSource.Rows.Count > 0)
                  _grid.Rows[0].Selected = true;
               else
                  _grid.Selected.Rows.Clear();
               _mainForm.UpdateEventFilters();
            }
            else
            {
               MessageBox.Show(this, "Failed to delete event filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) ;
            }
         }
      }

      private EventFilter EditEventFilter(EventFilter filter)
      {
         EventFilter clonedFilter = filter.Clone() ;
         Form_EventFilterWizard wizard = new Form_EventFilterWizard(clonedFilter, _configuration);

         if(wizard.ShowDialog(this) == DialogResult.OK)
         {
            try
            {
               if(FiltersDal.UpdateEventFilter(clonedFilter, _configuration.ConnectionString) != 1)
               {
                  MessageBox.Show(this, "Unable to update the event filter.", "Error") ;
                  return filter ;
               }
               if(!wizard.Filter.IsValid)
               {
                  if(!wizard.Filter.HasTargetInstances || wizard.Filter.HasConditions)
                     MessageBox.Show(this, CoreConstants.Exception_IncompleteEventFilter, "Invalid Event Filter", MessageBoxButtons.OK, MessageBoxIcon.Warning) ;
                  else
                     MessageBox.Show(this, CoreConstants.Exception_InvalidEventFilter, "Invalid Event Filter", MessageBoxButtons.OK, MessageBoxIcon.Warning) ;
               }
            }
            catch(Exception e)
            {
               ErrorLog.Instance.Write("Unable to update event filter", e, true) ;
               MessageBox.Show(this, String.Format("Unable to update the event filter.\r\nMessage: {0}", e.Message), "Error") ;
               return filter ;
            }

            clonedFilter.Dirty = false ;
            string oldFilterText = _rtfFilterDetails.Text.Replace("\n", "\r\n") ;
            UpdateFilterText(clonedFilter) ;
            string logString = String.Format("Name:  {0}\r\nDescription:  {1}\r\n\r\nPrevious Filter:  {2}\r\n\r\nNew Filter:  {3}",
               clonedFilter.Name, clonedFilter.Description, oldFilterText, _rtfFilterDetails.Text.Replace("\n", "\r\n")) ;
            LogRecord.WriteLog(Globals.Repository.Connection, LogType.EventFilterModified, logString) ;
            _mainForm.UpdateEventFilters() ;
            return clonedFilter ;
         }
         return filter ;
      }

      public override void RefreshView()
      {
         ArrayList filters = FiltersDal.SelectEventFilters(_configuration.ConnectionString) ;
         _grid.BeginUpdate() ;
         _dataSource.Rows.Clear();
         foreach (EventFilter filter in filters)
         {
            UltraDataRow row = _dataSource.Rows.Add();
            UpdateRowValues(row, filter);
         }
         _grid.EndUpdate();
         if (_dataSource.Rows.Count > 0)
            _grid.Rows[0].Selected = true;
         UpdateMenuFlags();
      }

      private void UpdateRowValues(UltraDataRow row, EventFilter filter)
      {
         Image img ;
         if(filter.IsValid)
         {
            if(filter.Enabled)
               img = GUI.Properties.Resources.EventFilter_16;
            else
               img = GUI.Properties.Resources.EventFilterDisabled_16;
         }
         else
            img = GUI.Properties.Resources.EventFilterInvalid_16;
         row["Icon"] = img ;
         if(filter.IsValid)
            row["Name"] = filter.Name ;
         else
            row["Name"] = filter.Name + " (Invalid Filter)";
         row["Server"] = filter.TargetInstanceList;
         row["Description"] = filter.Description;

         row.Tag = filter ;
      }

		#endregion  // Actions

      //-------------------------------------------------------------
      // UpdateMenuFlags
      //--------------------------------------------------------------
      private void UpdateMenuFlags()
      {
         EventFilter filter = GetActiveFilter();

         if (filter != null)
         {

            SetMenuFlag(CMMenuItem.UseRuleAsTemplate, Globals.isAdmin);
            SetMenuFlag(CMMenuItem.EnableRule, Globals.isAdmin && filter.IsValid && !filter.Enabled);
            SetMenuFlag(CMMenuItem.DisableRule, Globals.isAdmin && filter.IsValid && filter.Enabled);
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
         if (_dataSource.Rows.Count > 0)
            SetMenuFlag(CMMenuItem.ExportEventFilters);
         else
            SetMenuFlag(CMMenuItem.ExportEventFilters, false);
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
         HelpAlias.ShowHelp(this,HelpAlias.SSHELP_EventFiltersView);
      }

      public override void Properties()
      {
         EditSelectedFilter() ;
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

      private void EditSelectedFilter()
      {
         UltraGridRow row = _grid.Selected.Rows[0];
         if (row == null)
            return;
         UltraDataRow dataRow = row.ListObject as UltraDataRow;
         EventFilter filter = dataRow.Tag as EventFilter ;
         EventFilter editedFilter = EditEventFilter(filter);
         row.Tag = editedFilter ;
         UpdateRowValues(dataRow, editedFilter) ;
         UpdateMenuFlags();
      }

      private void KeyDown_grid(object sender, KeyEventArgs e)
      {
         if (_grid.Selected.Rows.Count > 0)
         {
            if (e.KeyCode == Keys.Delete)
               DeleteSelectedFilter();
            else if (e.KeyCode == Keys.Enter)
               EditSelectedFilter();
         }
      }

      private void DoubleClickRow_grid(object sender, DoubleClickRowEventArgs e)
      {
         if (_grid.Selected.Rows.Count > 0)
         {
            EditSelectedFilter();
         }
      }

      private void AfterSelectChange_grid(object sender, AfterSelectChangeEventArgs e)
      {
         EventFilter filter = GetActiveFilter();
         if (filter == null)
         {
            _rtfFilterDetails.Rtf = "";
         }
         else
         {
            _rtfFilterDetails.Rtf = FilterUIHelper.GenerateFilterDescription(filter, _configuration, out _link);
         }
         UpdateMenuFlags();
      }

      private void ToolClick_toolbarsManager(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
      {
         switch (e.Tool.Key)
         {
            case "newFilter":
               NewEventFilter();
               break;
            case "filterTemplate":
               NewEventFilterFromTemplate() ;
               break;
            case "enableFilter":
               Enable(true) ;
               break;
            case "disableFilter":
               Enable(false) ;
               break;
            case "delete":
               DeleteSelectedFilter();
               break;
            case "refresh":
               RefreshView();
               break;
            case "properties":
               EditSelectedFilter();
               break;
         }
      }

      private void BeforeToolDropdown_toolbarsManager(object sender, Infragistics.Win.UltraWinToolbars.BeforeToolDropdownEventArgs e)
      {
         EventFilter filter = GetActiveFilter();
         if (filter != null && Globals.isAdmin)
         {
            _toolbarsManager.Tools["enableFilter"].SharedProps.Enabled = filter.IsValid && !filter.Enabled;
            _toolbarsManager.Tools["disableFilter"].SharedProps.Enabled = filter.IsValid && filter.Enabled;
         }
         else
         {
            _toolbarsManager.Tools["enableFilter"].SharedProps.Enabled = false;
            _toolbarsManager.Tools["disableFilter"].SharedProps.Enabled = false;
         }
         _toolbarsManager.Tools["newFilter"].SharedProps.Enabled = Globals.isAdmin;
         _toolbarsManager.Tools["filterTemplate"].SharedProps.Enabled = (filter != null) && Globals.isAdmin;
         _toolbarsManager.Tools["delete"].SharedProps.Enabled = (filter != null) && Globals.isAdmin;
         _toolbarsManager.Tools["properties"].SharedProps.Enabled = (filter != null);
      }
   }
}
