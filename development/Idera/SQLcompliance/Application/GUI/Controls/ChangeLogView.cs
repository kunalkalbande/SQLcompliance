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
   public partial class ChangeLogView : BaseControl
   {
      private Stack _pageStartsChangeLog;
      private bool _reversePaging = false;
      private int _matchingEventCount = -1;
      private int _lastPage = -1;
      private object _bgLockObject;
      private int _bgCounter;
      private ChangeLogViewFilter _filter ;

      #region Initialization

      public ChangeLogView()
      {
         _bgLockObject = new object();
         InitializeComponent();

         GridHelper.ApplyAdminSettings(_grid);

         _filter = new ChangeLogViewFilter() ;

         // Enable main form menus 			
         SetMenuFlag(CMMenuItem.Refresh);
         SetMenuFlag(CMMenuItem.SetFilter);
         SetMenuFlag(CMMenuItem.Collapse);
         SetMenuFlag(CMMenuItem.Expand);
         SetMenuFlag(CMMenuItem.GroupByColumn);
         SetMenuFlag(CMMenuItem.ShowHelp);

         _pageStartsChangeLog = new Stack();
         _lblStatus.Visible = false ;
      }

      #endregion Initialization

      [Browsable(false)]
      public ChangeLogViewFilter Filter
      {
         get { return _filter; }
         set { _filter = value; }
      }

      #region BaseControl Overrides

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
         RelocateStatusLabel();
         _pageStartsChangeLog.Clear();
         LoadChangeLogPage();

         Thread t = new Thread(new ThreadStart(UpdateEventCountChangeLog));
         t.IsBackground = true;
         t.Start();
         Cursor = Cursors.Default;
      }

      private void RelocateStatusLabel()
      {
         UIElement elem = _grid.DisplayLayout.Bands[0].Header.GetUIElement();
         _lblStatus.Location = new Point(_lblStatus.Location.X, elem.Rect.Bottom);
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
         RelocateStatusLabel();
      }

      public override void Properties()
      {
         ChangeLogRow theRecord = GetActiveEvent();
         if (theRecord == null)
            return;
         Form_LogProperties frm = new Form_LogProperties(this, theRecord.LogId);
         frm.ShowDialog(this);
      }

      public override void ExpandAll()
      {
         _grid.Rows.ExpandAll(true);
      }

      public override void CollapseAll()
      {
         _grid.Rows.CollapseAll(true);
      }

      public override int Previous()
      {
         GridHelper.SelectPreviousLeafRow(_grid) ;
         ChangeLogRow activeRow = GetActiveEvent();
         if (activeRow != null)
            return activeRow.LogId;
         else
            return -1;
      }

      public override int Next()
      {
         GridHelper.SelectNextLeafRow(_grid) ;
         ChangeLogRow activeRow = GetActiveEvent();
         if (activeRow != null)
            return activeRow.LogId;
         else
            return -1;
      }

      #endregion BaseControl Overrides

      private void LoadChangeLogPage()
      {
         Cursor = Cursors.WaitCursor;

         if (_pageStartsChangeLog.Count == 0)
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
            //_grid.BeginInit();
            _lblStatus.Visible = false ;
            _gridDataSource.Rows.Clear() ;

            string query = BuildPageQueryChangeLog();
            ChangeLogRow[] rows = GetChangeLogRows(query);

            if (rows == null)
            {
               // We have an error
               //_statusRowChangeLog.Visible = true;
               _lblNavigation.Value = "<b>Error</b>";
            }
            else if (rows.Length == 0)
            {
               // No data matchedt he criteria
               _lblStatus.Text = UIConstants.Grid_NoEvents;
               _lblNavigation.Text = String.Format("<b>{0}</b>", UIConstants.Grid_NoEvents);
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
                     UltraDataRow row = _gridDataSource.Rows.Add() ;
                     UpdateRowValuesChangeLog(row, rows[i]);
                  }
                  _btnFirst.Enabled = true;
                  _btnPrevious.Enabled = true;
               }
               else
               {
                  for (int i = 0; i < Settings.Default.EventPageSize; i++)
                  {
                     UltraDataRow row = _gridDataSource.Rows.Add();
                     UpdateRowValuesChangeLog(row, rows[i]);
                  }
                  _btnLast.Enabled = true;
                  _btnNext.Enabled = true;
               }
               _pageStartsChangeLog.Push(rows[Settings.Default.EventPageSize]);
               UpdateEventCountString();
            }
            else
            {
               // We are the last page.
               if (_reversePaging)
               {
                  for (int i = rows.Length - 1; i >= 0; i--)
                  {
                     UltraDataRow row = _gridDataSource.Rows.Add();
                     UpdateRowValuesChangeLog(row, rows[i]);
                  }
                  _btnFirst.Enabled = false;
                  _btnPrevious.Enabled = false;
               }
               else
               {
                  for (int i = 0; i < rows.Length; i++)
                  {
                     UltraDataRow row = _gridDataSource.Rows.Add();
                     UpdateRowValuesChangeLog(row, rows[i]);
                  }
                  _btnLast.Enabled = false;
                  _btnNext.Enabled = false;
               }
               // Placeholder to allow previous to continue working
               //  correctly
               _pageStartsChangeLog.Push(null);
               UpdateEventCountString();
            }
            //_lvChangeLog.EndInit();
         }
         catch (Exception e)
         {
            ErrorLog.Instance.Write("LoadEventPage", e);
         }
         Cursor = Cursors.Default;
      }

      private void UpdateEventCountChangeLog()
      {
         int myCounter;
         int count;

         lock (_bgLockObject)
         {
            myCounter = ++_bgCounter;
         }

         string whereClause;
         whereClause = _filter.GetWhereClause(TimeZoneInfo.CurrentTimeZone);

         string selectQuery = String.Format("SELECT count(logId) FROM {0}..{1} AS e {2}{3}",
            CoreConstants.RepositoryDatabase,
            CoreConstants.RepositoryChangeLogEventTable,
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
            ErrorLog.Instance.Write("Unable to load ChangeLog EventCount.", e, true);
            count = -1;
         }
         lock (_bgLockObject)
         {
            if (myCounter == _bgCounter)
            {
               _matchingEventCount = count;
               _lastPage = _matchingEventCount / Settings.Default.EventPageSize - (_pageStartsChangeLog.Count - 2);
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
         pageCount = _pageStartsChangeLog.Count;
         if (_matchingEventCount != -1)
         {
            if (_matchingEventCount == 0)
            {
               _lblNavigation.Value = String.Format("<b>{0}</b>", UIConstants.Grid_NoEvents);
            }
            else
            {
               int currentPage;
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
            _lblNavigation.Value = String.Format("<b>Page {0} of ... : ... matching events</b>", _reversePaging ? "..." : _pageStartsChangeLog.Count.ToString());
      }

      private string BuildPageQueryChangeLog()
      {
         string whereClause;
         string orderByClause;

         if (!_reversePaging)
            orderByClause = " ORDER BY logId DESC ";
         else
            orderByClause = " ORDER BY logId ASC ";

         whereClause = _filter.GetWhereClause(TimeZoneInfo.CurrentTimeZone) ;

         // Make sure we aren't the first page being loaded
         //  
         if (_pageStartsChangeLog.Count > 0)
         {
            ChangeLogRow pageStart = (ChangeLogRow)_pageStartsChangeLog.Peek();
            // Need to add paging conditions
            if (whereClause.Length > 0)
               whereClause = String.Format("logId {0} {1} AND {2}",
                  _reversePaging ? ">=" : "<=", pageStart.LogId, whereClause);
            else
               whereClause = String.Format("logId {0} {1}", _reversePaging ? ">=" : "<=", pageStart.LogId);
         }
         else
         {
            // No paging. fake numbers
            if (whereClause.Length > 0)
               whereClause = String.Format("logId {0} AND {1}",
                  _reversePaging ? ">= -2100000000" : "<= 2100000000", whereClause);
            else
               whereClause = String.Format("logId {0}", _reversePaging ? ">= -2100000000" : "<= 2100000000");
         }
         return String.Format("SELECT TOP {0} logId, eventTime, logType, logUser, logSqlServer, logInfo " +
            "FROM {1}..{2} WHERE {3} {4}",
            Settings.Default.EventPageSize + 1,
            CoreConstants.RepositoryDatabase,
            CoreConstants.RepositoryChangeLogEventTable,
            whereClause, orderByClause);
      }

      private ChangeLogRow[] GetChangeLogRows(string query)
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
                     ChangeLogRow row = new ChangeLogRow();
                     row.LogId = SQLHelpers.GetInt32(reader, i++);
                     row.StartTime = SQLHelpers.GetDateTime(reader, i++);
                     row.LogEventTypeId = SQLHelpers.GetInt32(reader, i++);
                     row.UserName = SQLHelpers.GetString(reader, i++);
                     row.Server = SQLHelpers.GetString(reader, i++);
                     row.Description = SQLHelpers.GetString(reader, i++);
                     list.Add(row);
                  }
               }
            }
            ChangeLogRow[] retVal = new ChangeLogRow[list.Count];
            for (int i = 0; i < retVal.Length; i++)
               retVal[i] = (ChangeLogRow)list[i];

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

      private static void UpdateRowValuesChangeLog(UltraDataRow row, ChangeLogRow record)
      {
         string sEventType = Globals.Repository.LookupChangeLogEventType(record.LogEventTypeId);

         row["Date"] = record.StartTime.ToLocalTime();
         row["Time"] = record.StartTime.ToLocalTime();
         row["Event"] = sEventType == null ? record.LogEventTypeId.ToString() : sEventType;
         row["User"] = record.UserName;
         row["SQL Server"] = record.Server;
         row["Description"] = record.Description;
         row.Tag = record;
      }

      private ChangeLogRow GetActiveEvent()
      {
         if (_grid.Selected.Rows.Count <= 0)
            return null;
         else
         {
            UltraGridRow gridRow = _grid.Selected.Rows[0] ;
            UltraDataRow dataRow = gridRow.ListObject as UltraDataRow ;
            return dataRow != null ? dataRow.Tag as ChangeLogRow : null ;
         }
      }

      #region Event Handlers

      private void Click_btnNext(object sender, EventArgs e)
      {
         if (_reversePaging)
         {
            _pageStartsChangeLog.Pop();
            _pageStartsChangeLog.Pop();
         }
         LoadChangeLogPage();
         UpdateEventCountString();
      }

      private void Click_btnPrevious(object sender, EventArgs e)
      {
         if (!_reversePaging)
         {
            _pageStartsChangeLog.Pop();
            _pageStartsChangeLog.Pop();
         }
         LoadChangeLogPage();
         UpdateEventCountString();
      }

      private void Click_btnFirst(object sender, EventArgs e)
      {
         _reversePaging = false;
         _pageStartsChangeLog.Clear();
         LoadChangeLogPage();
         UpdateEventCountString();
      }

      private void Click_btnLast(object sender, EventArgs e)
      {
         _reversePaging = true;
         _pageStartsChangeLog.Clear();
         LoadChangeLogPage();
         UpdateEventCountString();
      }

      private void DoubleClickRow_grid(object sender, DoubleClickRowEventArgs e)
      {
         Properties() ;
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

      private void KeyDown_grid(object sender, KeyEventArgs e)
      {
         if(e.KeyCode == Keys.Enter)
            Properties() ;
      }

      #endregion Event Handlers

      private void ToolClick_toolbarsManager(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
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
            case "properties":
               Properties() ;
               break ;
         }
      }

      private void BeforeToolDropdown_toolbarsManager(object sender, Infragistics.Win.UltraWinToolbars.BeforeToolDropdownEventArgs e)
      {
         ChangeLogRow record = GetActiveEvent();

         if (record != null)
         {
            _toolbarsManager.Tools["properties"].SharedProps.Enabled = true;
         }
         else
         {
            _toolbarsManager.Tools["properties"].SharedProps.Enabled = false;
         }
         ((StateButtonTool) ((PopupMenuTool) e.Tool).Tools["showGroupBy"]).InitializeChecked(ShowGroupBy);
         _toolbarsManager.Tools["expandAll"].SharedProps.Enabled = ShowGroupBy;
         _toolbarsManager.Tools["collapseAll"].SharedProps.Enabled = ShowGroupBy;
      }

      private void ChangeLogView_HelpRequested(object sender, HelpEventArgs hlpevent)
      {
         HelpOnThisWindow();
         hlpevent.Handled = true;
      }

      public override void HelpOnThisWindow()
      {
         HelpAlias.ShowHelp(this, HelpAlias.SSHELP_ChangeLogView);
      }
   }

   internal class ChangeLogRow
   {
      private int logId;
      private int _eventTypeId;
      private DateTime _startTime;
      private string _userName;
      private string _server;
      private string _description;

      public int LogId
      {
         get { return logId; }
         set { logId = value; }
      }

      public int LogEventTypeId
      {
         get { return _eventTypeId; }
         set { _eventTypeId = value; }
      }

      public DateTime StartTime
      {
         get { return _startTime; }
         set { _startTime = value; }
      }

      public string UserName
      {
         get { return _userName; }
         set { _userName = value; }
      }

      public string Server
      {
         get { return _server; }
         set { _server = value; }
      }

      public string Description
      {
         get { return _description; }
         set { _description = value; }
      }
   }}

