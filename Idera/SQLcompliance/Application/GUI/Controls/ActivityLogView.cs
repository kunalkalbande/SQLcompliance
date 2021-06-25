using System;
using System.Collections;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Idera.SQLcompliance.Application.GUI.Forms;
using Idera.SQLcompliance.Application.GUI.Helper;
using Idera.SQLcompliance.Core;
using Infragistics.Win;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using TimeZoneInfo=Idera.SQLcompliance.Core.TimeZoneHelper.TimeZoneInfo;

namespace Idera.SQLcompliance.Application.GUI.Controls
{
   public partial class ActivityLogView : BaseControl
   {
      private Stack _pageStartsActivityLog ;
      private bool _reversePaging = false;
      private int _matchingEventCount = -1;
      private int _lastPage = -1;
      private object _bgLockObject;
      private int _bgCounter;
      private ActivityLogViewFilter _filter ;

      public ActivityLogView()
      {
         _bgLockObject = new object();
         InitializeComponent();

         GridHelper.ApplyAdminSettings(_grid) ;

         // Enable main form menus 			
         SetMenuFlag(CMMenuItem.Refresh);
         SetMenuFlag(CMMenuItem.SetFilter);
         SetMenuFlag(CMMenuItem.Collapse);
         SetMenuFlag(CMMenuItem.Expand);
         SetMenuFlag(CMMenuItem.GroupByColumn);
         SetMenuFlag(CMMenuItem.ShowHelp);

         _pageStartsActivityLog = new Stack();
         _lblStatus.Visible = false;
      }

      [Browsable(false)]
      public ActivityLogViewFilter Filter
      {
         get { return _filter; }
         set { _filter = value; }
      }



      #region BaseView Overrides

      public override void UpdateColors()
      {
         base.UpdateColors();

         _pnlNavigation.BackColor = Office2007ColorTable.Colors.DockAreaGradientLight;
         _pnlNavigation.BackColor2 = Office2007ColorTable.Colors.DockAreaGradientLight;
         _lblNavigation.ForeColor = Office2007ColorTable.Colors.OutlookNavPaneCurrentGroupHeaderForecolor;
      }

      public override void RefreshView()
      {
         Cursor = Cursors.WaitCursor;
         _reversePaging = false;
         _matchingEventCount = -1;
         _pageStartsActivityLog.Clear();
         RelocateStatusLabel();
         LoadActivityLogPage();

         Thread t = new Thread(new ThreadStart(UpdateEventCountActivityLog));
         t.IsBackground = true;
         t.Start();
         Cursor = Cursors.Default;
      }

      public ActivityLogRow PreviousRecord()
      {
         GridHelper.SelectPreviousLeafRow(_grid);
         return GetActiveEvent();
      }

      public ActivityLogRow NextRecord()
      {
         GridHelper.SelectNextLeafRow(_grid);
         return GetActiveEvent();
      }

      private ActivityLogRow GetActiveEvent()
      {
         if (_grid.Selected.Rows.Count <= 0)
            return null;
         else
         {
            UltraGridRow gridRow = _grid.Selected.Rows[0];
            UltraDataRow dataRow = gridRow.ListObject as UltraDataRow;
            return dataRow != null ? dataRow.Tag as ActivityLogRow : null;
         }
      }

      private void RelocateStatusLabel()
      {
         UIElement elem = _grid.DisplayLayout.Bands[0].Header.GetUIElement();
         _lblStatus.Location = new Point(_lblStatus.Location.X, elem.Rect.Bottom);
      }

      protected override void OnShowGroupByChanged(ToggleChangedEventArgs e)
      {
         base.OnShowGroupByChanged(e) ;

         if (e.Enabled)
            _grid.DisplayLayout.ViewStyleBand = ViewStyleBand.OutlookGroupBy;
         else
            _grid.DisplayLayout.ViewStyleBand = ViewStyleBand.Horizontal;

         SetMenuFlag(CMMenuItem.Collapse, e.Enabled);
         SetMenuFlag(CMMenuItem.Expand, e.Enabled);
         RelocateStatusLabel();
      }

      public override void ExpandAll()
      {
         _grid.Rows.ExpandAll(true);
      }

      public override void CollapseAll()
      {
         _grid.Rows.CollapseAll(true);
      }

      #endregion BaseView Overrides

      private void LoadActivityLogPage()
      {
         Cursor = Cursors.WaitCursor;

         _lblStatus.Visible = false;
         if (_pageStartsActivityLog.Count == 0)
         {
            if (!_reversePaging)
            {
               _btnPrevious.Enabled = false;
               _btnFirst.Enabled = false;
            }
            else
            {
               _btnLast.Enabled = false;
               _btnNext.Enabled = false;
            }
         }
         else
         {
            _btnFirst.Enabled = true;
            _btnPrevious.Enabled = true;
            _btnLast.Enabled = true;
            _btnNext.Enabled = true;
         }

         try
         {
            _grid.BeginUpdate();
            _dataSource.Rows.Clear();

            string query = BuildPageQueryActivityLog();
            ActivityLogRow[] rows = GetActivityLogRows(query);

            if (rows == null)
            {
               // We have an error
               _lblStatus.Visible = true;
               _lblNavigation.Value = "<b>Error</b>"; 
            }
            else if (rows.Length == 0)
            {
               // No data matchedt he criteria
               _lblStatus.Text = UIConstants.Grid_NoEvents;
               _lblNavigation.Value = String.Format("<b>{0}</b>", UIConstants.Grid_NoEvents); 
               _lblStatus.Visible = true;
            }
            else if (rows.Length == Settings.Default.EventPageSize + 1)
            {
               // We have data with more available

               // if reverse paging, invert the order of addition
               //  This should be in-order; however, with no sorts
               //  this is needed to order them by id
               if (_reversePaging)
               {
                  for (int i = Settings.Default.EventPageSize - 1; i >= 0; i--)
                  {
                     UltraDataRow row = _dataSource.Rows.Add();
                     UpdateRowValuesActivityLog(row, rows[i]);
                  }
                  _btnFirst.Enabled = true;
                  _btnPrevious.Enabled = true;
               }
               else
               {
                  for (int i = 0; i < Settings.Default.EventPageSize; i++)
                  {
                     UltraDataRow row = _dataSource.Rows.Add();
                     UpdateRowValuesActivityLog(row, rows[i]);
                  }
                  _btnLast.Enabled = true;
                  _btnNext.Enabled = true;
               }
               _pageStartsActivityLog.Push(rows[Settings.Default.EventPageSize]);
               UpdateEventCountString();
            }
            else
            {
               // We are the last page.
               if (_reversePaging)
               {
                  for (int i = rows.Length - 1; i >= 0; i--)
                  {
                     UltraDataRow row = _dataSource.Rows.Add();
                     UpdateRowValuesActivityLog(row, rows[i]);
                  }
                  _btnFirst.Enabled = false;
                  _btnPrevious.Enabled = false;
               }
               else
               {
                  for (int i = 0; i < rows.Length; i++)
                  {
                     UltraDataRow row = _dataSource.Rows.Add();
                     UpdateRowValuesActivityLog(row, rows[i]);
                  }
                  _btnLast.Enabled = false;
                  _btnNext.Enabled = false;
               }
               // Placeholder to allow previous to continue working
               //  correctly
               _pageStartsActivityLog.Push(null);
               UpdateEventCountString();
            }
            _grid.EndUpdate() ;
         }
         catch (Exception e)
         {
            ErrorLog.Instance.Write("LoadEventPage", e);
         }
         Cursor = Cursors.Default;
      }

      private void UpdateEventCountActivityLog()
      {
         int myCounter;
         int count;

         lock (_bgLockObject)
         {
            myCounter = ++_bgCounter;
         }

         string whereClause;
         whereClause = _filter.GetWhereClause(TimeZoneInfo.CurrentTimeZone);

         string selectQuery = String.Format("SELECT count(eventId) FROM {0}..{1} AS e {2}{3}",
            CoreConstants.RepositoryDatabase,
            CoreConstants.RepositoryAgentEventTable,
            (whereClause != "") ? " WHERE " : "",
            whereClause);

         try
         {
            string strConn = String.Format("server={0};" +
               "integrated security=SSPI;" +
               "Connect Timeout=30;" +
               "Application Name='{1}';",
               Repository.ServerInstance,
               CoreConstants.DefaultSqlApplicationName);
            using (SqlConnection conn = new SqlConnection(strConn))
            {
               conn.Open();
               using (SqlCommand command = new SqlCommand(selectQuery, conn))
               {
                  object obj = command.ExecuteScalar();
                  if (obj is DBNull)
                     count = 0;
                  else
                     count = (int)obj;

               }
            }
         }
         catch (Exception e)
         {
            ErrorLog.Instance.Write("Unable to load AgentLog EventCount.", e, true);
            count = -1;
         }
         lock (_bgLockObject)
         {
            if (myCounter == _bgCounter)
            {
               _matchingEventCount = count;
               _lastPage = _matchingEventCount / Settings.Default.EventPageSize - (_pageStartsActivityLog.Count - 2);
               if (_matchingEventCount % Settings.Default.EventPageSize == 0) _lastPage--;
               UpdateEventCountString();
            }
         }
      }

      private void UpdateEventCountString()
      {
         if (InvokeRequired)
         {
            this.Invoke(new MethodInvoker(UpdateEventCountString));
            return;
         }
         int pageCount;
         pageCount = _pageStartsActivityLog.Count;
         if (_matchingEventCount != -1)
         {
            int currentPage;
            if (_matchingEventCount == 0)
            {
               _lblNavigation.Value = String.Format("<b>{0}</b>", UIConstants.Grid_NoEvents);
            }
            else
            {
               if (_reversePaging)
               {
                  currentPage = _matchingEventCount / Settings.Default.EventPageSize - (pageCount - 2);
                  if (_matchingEventCount % Settings.Default.EventPageSize == 0) currentPage--;
               }
               else
                  currentPage = pageCount;
               _lblNavigation.Value = String.Format("<b>Page {0} of {1} : {2} matching events</b>", currentPage, _lastPage, _matchingEventCount);
            }
         }
         else
            _lblNavigation.Value = String.Format("<b>Page {0} of ... : ... matching events</b>", _reversePaging ? "..." : _pageStartsActivityLog.Count.ToString());
      }

      private void DoubleClickRow_grid(object sender, DoubleClickRowEventArgs e)
      {
         Properties();
      }

      private void KeyDown_grid(object sender, KeyEventArgs e)
      {
         if (e.KeyCode == Keys.Enter)
            Properties();
      }


      public override void Properties()
      {
         ActivityLogRow theRecord = GetActiveEvent();
         if (theRecord == null)
            return;
         Form_ActivityLogProperties frm = new Form_ActivityLogProperties(this, theRecord);
         frm.ShowDialog(this);
      }


      private string BuildPageQueryActivityLog()
      {
         string whereClause;
         string orderByClause;

         if (!_reversePaging)
            orderByClause = " ORDER BY eventId DESC ";
         else
            orderByClause = " ORDER BY eventId ASC ";

         whereClause = _filter.GetWhereClause(TimeZoneInfo.CurrentTimeZone);

         // Make sure we aren't the first page being loaded
         //  
         if (_pageStartsActivityLog.Count > 0)
         {
            ActivityLogRow pageStart = (ActivityLogRow)_pageStartsActivityLog.Peek();
            // Need to add paging conditions
            if (whereClause.Length > 0)
               whereClause = String.Format("eventId {0} {1} AND {2}",
                  _reversePaging ? ">=" : "<=", pageStart.EventId, whereClause);
            else
               whereClause = String.Format("eventId {0} {1}", _reversePaging ? ">=" : "<=", pageStart.EventId);
         }
         else
         {
            // No paging. fake numbers
            if (whereClause.Length > 0)
               whereClause = String.Format("eventId {0} AND {1}",
                  _reversePaging ? ">= -2100000000" : "<= 2100000000", whereClause);
            else
               whereClause = String.Format("eventId {0}", _reversePaging ? ">= -2100000000" : "<= 2100000000");
         }

         return String.Format("SELECT TOP {0} eventId,eventTime,instance,eventType,details" +
            " FROM {1}..{2} WHERE {3} {4}",
            Settings.Default.EventPageSize + 1,
            CoreConstants.RepositoryDatabase,
            CoreConstants.RepositoryAgentEventTable,
            whereClause, orderByClause);
      }

      private ActivityLogRow[] GetActivityLogRows(string query)
      {
         ArrayList list = new ArrayList();
         try
         {
            using (SqlCommand command = new SqlCommand(query, Globals.Repository.Connection))
            {
               command.CommandTimeout = CoreConstants.sqlcommandTimeout;
               using (SqlDataReader reader = command.ExecuteReader())
               {
                  while (reader.Read())
                  {
                     int i = 0;
                     ActivityLogRow row = new ActivityLogRow();
                     row.EventId = SQLHelpers.GetInt32(reader, i++);
                     row.StartTime = SQLHelpers.GetDateTime(reader, i++);
                     row.Instance = SQLHelpers.GetString(reader, i++);
                     row.EventTypeId = SQLHelpers.GetInt16(reader, i++);
                     row.Details = SQLHelpers.GetString(reader, i++);
                     list.Add(row);
                  }
               }
            }
            ActivityLogRow[] retVal = new ActivityLogRow[list.Count];
            for (int i = 0; i < retVal.Length; i++)
               retVal[i] = (ActivityLogRow)list[i];

            return retVal;
         }
         catch (SqlException sqlEx)
         {
            _lblStatus.Text = String.Format("Class: {0}  Number: {1}  Message: {2}",
               sqlEx.Class,
               sqlEx.Number,
               sqlEx.Message);
         }
         catch (Exception ex)
         {
            _lblStatus.Text = ex.Message;
         }
         return null;
      }


      private static void UpdateRowValuesActivityLog(UltraDataRow row, ActivityLogRow record)
      {
         string sEventType = Globals.Repository.LookupAgentLogEventType(record.EventTypeId);

         if(record.EventTypeId > 3000)
            row["Icon"] = GUI.Properties.Resources.StatusGood_16;
         else if(record.EventTypeId > 2000)
            row["Icon"] = GUI.Properties.Resources.StatusError_16;
         else if (record.EventTypeId > 1000)
            row["Icon"] = GUI.Properties.Resources.StatusWarning_16;
         else
            row["Icon"] = GUI.Properties.Resources.information;

         row["Date"] = record.StartTime.ToLocalTime();
         row["Time"] = record.StartTime.ToLocalTime();
         row["Event"] = sEventType == null ? record.EventTypeId.ToString() : sEventType;
         row["Details"] = record.Details;
         row["SQLServer"] = record.Instance;
         row.Tag = record;
      }

      #region Event Handlers

      private void Click_btnNext(object sender, EventArgs e)
      {
         if (_reversePaging)
         {
            _pageStartsActivityLog.Pop();
            _pageStartsActivityLog.Pop();
         }
         LoadActivityLogPage();
         UpdateEventCountString();
      }

      private void Click_btnPrevious(object sender, EventArgs e)
      {
         if (!_reversePaging)
         {
            _pageStartsActivityLog.Pop();
            _pageStartsActivityLog.Pop();
         }
         LoadActivityLogPage();
         UpdateEventCountString();
      }

      private void Click_btnFirst(object sender, EventArgs e)
      {
         _reversePaging = false;
         _pageStartsActivityLog.Clear();
         LoadActivityLogPage();
         UpdateEventCountString();
      }

      private void Click_btnLast(object sender, EventArgs e)
      {
         _reversePaging = true;
         _pageStartsActivityLog.Clear();
         LoadActivityLogPage();
         UpdateEventCountString();
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

      private void ToolClick_toolbarsManager(object sender, ToolClickEventArgs e)
      {
         switch(e.Tool.Key)
         {
            case "collapseAll":
               CollapseAll() ;
               break ;
            case "expandAll":
               ExpandAll() ;
               break ;
            case "showGroupBy":
               ShowGroupBy = !ShowGroupBy ;
               break ;
            case "refresh":
               RefreshView() ;
               break ;
         }
      }

      private void BeforeToolDropdown_toolbarsManager(object sender, BeforeToolDropdownEventArgs e)
      {
         ((StateButtonTool)((PopupMenuTool)e.Tool).Tools["showGroupBy"]).InitializeChecked(ShowGroupBy);
         _toolbarsManager.Tools["expandAll"].SharedProps.Enabled = ShowGroupBy;
         _toolbarsManager.Tools["collapseAll"].SharedProps.Enabled = ShowGroupBy;
      }

      private void ActivityLogView_HelpRequested(object sender, HelpEventArgs hlpevent)
      {
         HelpOnThisWindow();
         hlpevent.Handled = true;
      }

      public override void HelpOnThisWindow()
      {
         HelpAlias.ShowHelp(this, HelpAlias.SSHELP_ActivityLogView);
      }

//      private void UpdateFilterDisplay()
//      {
//         _comboTime.SelectionChanged -= new EventHandler(SelectionChanged_comboTime);
//
//         EventViewFilter filter = Globals.ActivityLogViewFilter;
//
//         switch (filter.DateLimitType)
//         {
//            case EventViewFilter.LimitType.Unlimited:
//               _comboTime.SelectedIndex = 0;
//               break;
//            case EventViewFilter.LimitType.NumberDays:
//               if (filter.Days == 7)
//                  _comboTime.SelectedIndex = 2;
//               else if (filter.Days == 30)
//                  _comboTime.SelectedIndex = 3;
//               else
//                  _comboTime.SelectedIndex = 4;
//               break;
//            case EventViewFilter.LimitType.DateRange:
//               AddCustomRange(filter.StartDate, filter.EndDate);
//               _comboTime.SelectedIndex = 5;
//               break;
//            case EventViewFilter.LimitType.Today:
//               _comboTime.SelectedIndex = 1;
//               break;
//         }
//         _comboTime.SelectionChanged += new EventHandler(SelectionChanged_comboTime);
//      }
//
//      private void CheckChanged_cbEnableGroupBy(object sender, EventArgs e)
//      {
//         if (ShowGroupBy == _cbEnableGroupBy.Checked)
//            return;
//         ShowGroupBy = _cbEnableGroupBy.Checked;
//      }
//
//      private void Click_btnExpandAll(object sender, EventArgs e)
//      {
//         ExpandAll();
//      }
//
//      private void Click_btnCollapseAll(object sender, EventArgs e)
//      {
//         CollapseAll();
//      }
//
//      private void SelectionChanged_comboTime(object sender, EventArgs e)
//      {
//         EventViewFilter filter = Globals.ActivityLogViewFilter;
//
//         switch (_comboTime.SelectedIndex)
//         {
//            case 0:
//               filter.DateLimitType = EventViewFilter.LimitType.Unlimited;
//               break;
//            case 1:
//               filter.DateLimitType = EventViewFilter.LimitType.Today;
//               break;
//            case 2:
//               filter.DateLimitType = EventViewFilter.LimitType.NumberDays;
//               filter.Days = 7;
//               break;
//            case 3:
//               filter.DateLimitType = EventViewFilter.LimitType.NumberDays;
//               filter.Days = 30;
//               break;
//            case 4:
//               {
//                  _comboTime.CloseUp();
//                  Form_TimeSpan frm = new Form_TimeSpan();
//                  frm.StartDate = filter.StartDate;
//                  frm.EndDate = filter.EndDate;
//                  frm.ShowDialog(this);
//                  AddCustomRange(frm.StartDate, frm.EndDate);
//                  _comboTime.SelectedIndex = 5;
//               }
//               break;
//            default:
//               {
//                  // custom
//                  int index = _comboTime.SelectedIndex - 5;
//                  filter.DateLimitType = EventViewFilter.LimitType.DateRange;
//                  filter.StartDate = _startDateLru[index].Value;
//                  filter.EndDate = _endDateLru[index].Value;
//               }
//               break;
//         }
//         RefreshView();
//      }
//
//      private void AddCustomRange(DateTime start, DateTime end)
//      {
//         _startDateLru[2] = _startDateLru[1];
//         _startDateLru[1] = _startDateLru[0];
//         _startDateLru[0] = start;
//         _endDateLru[2] = _endDateLru[1];
//         _endDateLru[1] = _endDateLru[0];
//         _endDateLru[0] = end;
//
//         int itemsToRemove = _comboTime.Items.Count - 5;
//         ValueListItem item;
//         for (int i = 0; i < 3; i++)
//         {
//            if (_startDateLru[i] != null)
//            {
//               item = new ValueListItem("", String.Format("{0} - {1}", _startDateLru[i].Value.ToShortDateString(), _endDateLru[i].Value.ToShortDateString()));
//               _comboTime.Items.Add(item);
//            }
//         }
//         while (itemsToRemove > 0)
//         {
//            _comboTime.Items.RemoveAt(5);
//            itemsToRemove--;
//         }
//      }

      #endregion Event Handlers
   }

   public class ActivityLogRow
   {
      private int _eventId;
      private DateTime _startTime;
      private string _details;
      private string _instance;
      private int _eventTypeId;

      public int EventId
      {
         get { return _eventId; }
         set { _eventId = value; }
      }

      public DateTime StartTime
      {
         get { return _startTime; }
         set { _startTime = value; }
      }

      public string Details
      {
         get { return _details; }
         set { _details = value; }
      }

      public string Instance
      {
         get { return _instance; }
         set { _instance = value; }
      }

      public int EventTypeId
      {
         get { return _eventTypeId; }
         set { _eventTypeId = value; }
      }
   }}

