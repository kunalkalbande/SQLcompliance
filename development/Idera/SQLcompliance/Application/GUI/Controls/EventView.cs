using Idera.SQLcompliance.Application.GUI.Forms;
using Idera.SQLcompliance.Application.GUI.Helper;
using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Agent;
using Idera.SQLcompliance.Core.Event;
using Idera.SQLcompliance.Core.Rules;
using Infragistics.Win;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using TimeZoneInfo = Idera.SQLcompliance.Core.TimeZoneHelper.TimeZoneInfo;

namespace Idera.SQLcompliance.Application.GUI.Controls
{
    public delegate void VoidDelegateListEvents(List<EventRow> events);
    public delegate void VoidDelegateListFlatEvents(List<FlatEventRow> events);

    public partial class EventView : BaseControl
    {
        private bool _eventDatabaseNeedsUpdate;
        private string _eventDatabaseName;
        private string _databaseName;
        private TimeZoneInfo _timeZoneInfo;
        private bool _reversePaging = false;
        private int _lastPage = -1;
        private int _matchingEventCount = -1;
        private object _bgLockObject;
        private int _bgCounter;
        private ServerRecord _currentServer;
        private DateTime?[] _startDateLru;
        private DateTime?[] _endDateLru;
        private bool _isArchive;

        private GridViewMode _viewMode = GridViewMode.Hierarchical;
        private UltraDataSource _dsActive;
        private UltraGrid _gridActive;

        private uint _internalUpdate = 0;
        private EventViewFilter _filter;

        // this stack stores the page starts for previously viewed pages and
        //  the page currently being viewed.  It is used to support backward
        //  browsing through events.
        private Stack<EventViewPageBookmark> _pageStarts;

        private BackgroundWorker _bgEventLoader;

        public EventView()
        {
            _bgLockObject = new object();

            _bgEventLoader = new BackgroundWorker();
            _bgEventLoader.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunWorkerCompleted_bgEventLoader);
            _bgEventLoader.DoWork += new DoWorkEventHandler(DoWork_bgEventLoader);

            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            ViewMode = (GridViewMode.Flat);
            //         _gridActive = _gridFlat;
            //         _dsActive = _dsFlatEvents;

            GridHelper.ApplyDefaultSettings(_gridFlat);
            GridHelper.ApplyDefaultSettings(_gridHierarchical);
            _gridHierarchical.DisplayLayout.Override.ExpansionIndicator = ShowExpansionIndicator.CheckOnDisplay;
            _gridHierarchical.DisplayLayout.ViewStyle = ViewStyle.MultiBand;
            _gridHierarchical.DisplayLayout.ViewStyleBand = ViewStyleBand.Vertical;
            _gridHierarchical.DisplayLayout.Override.AllowColSizing = AllowColSizing.Free;

            SetMenuFlag(CMMenuItem.Refresh);
            SetMenuFlag(CMMenuItem.SetFilter);
            SetMenuFlag(CMMenuItem.Collapse);
            SetMenuFlag(CMMenuItem.Expand);
            SetMenuFlag(CMMenuItem.GroupByColumn);
            SetMenuFlag(CMMenuItem.ShowHelp);

            _startDateLru = new DateTime?[3];
            _startDateLru[0] = null;
            _startDateLru[1] = null;
            _startDateLru[2] = null;
            _endDateLru = new DateTime?[3];
            _endDateLru[0] = null;
            _endDateLru[1] = null;
            _endDateLru[2] = null;

            _pageStarts = new Stack<EventViewPageBookmark>();

            _filter = new EventViewFilter();
            _isArchive = false;
        }

        public bool IsArchive
        {
            get { return _isArchive; }
            set { _isArchive = value; }
        }

        public EventViewFilter Filter
        {
            get { return _filter; }
            set { _filter = value; }
        }

        public UltraGrid ActiveGrid
        {
            get { return _gridActive; }
        }

        public GridViewMode ViewMode
        {
            get { return _viewMode; }
            set
            {
                if (_viewMode == value)
                {
                    return;
                }

                _viewMode = value;
                if (value == GridViewMode.Hierarchical)
                {
                    _gridHierarchical.Visible = true;
                    _gridHierarchical.BringToFront();
                    _gridFlat.Visible = false;
                    _dsFlatEvents.Rows.Clear();
                    _gridActive = _gridHierarchical;
                    _dsActive = _dsHierarchicalEvents;
                }
                else
                {
                    _gridFlat.Visible = true;
                    _gridFlat.BringToFront();
                    _gridHierarchical.Visible = false;
                    _dsHierarchicalEvents.Rows.Clear();
                    _gridActive = _gridFlat;
                    _dsActive = _dsFlatEvents;
                }
                RefreshView();
            }
        }

        public UltraGrid GetGrid(GridViewMode mode)
        {
            if (mode == GridViewMode.Flat)
            {
                return _gridFlat;
            }
            else
            {
                return _gridHierarchical;
            }
        }

        public override void UpdateColors()
        {
            base.UpdateColors();

            _pnlNavigation.BackColor = Office2007ColorTable.Colors.DockAreaGradientLight;
            _pnlNavigation.BackColor2 = Office2007ColorTable.Colors.DockAreaGradientLight;
            _lblNavigation.ForeColor = Office2007ColorTable.Colors.OutlookNavPaneCurrentGroupHeaderForecolor;
        }

        #region Actions

        //-------------------------------------------------------------
        // GetCount - Get total number or rows that could be returned
        //--------------------------------------------------------------
        private void UpdateEventCount()
        {
            int myCounter;
            int count;

            lock (_bgLockObject)
            {
                myCounter = ++_bgCounter;
            }

            string whereClause;
            whereClause = Filter.GetWhereClause(_databaseName, _timeZoneInfo);
            string selectQuery;

            if (_viewMode == GridViewMode.Flat)
            {
                string beforeAfterClause = string.Format(" LEFT OUTER JOIN {0}..{1} as d ON (e.eventId = d.eventId) " +
                   " LEFT OUTER JOIN {0}..{2} as c ON (d.dcId = c.dcId)",
                   SQLHelpers.CreateSafeDatabaseName(_eventDatabaseName),
                   CoreConstants.RepositoryDataChangesTable,
                   CoreConstants.RepositoryColumnChangesTable);

                //selectQuery = String.Format("SELECT count(*) FROM {0}..{1} AS e {2} {3}{4}",
                //                                   SQLHelpers.CreateSafeDatabaseName(_eventDatabaseName),
                //                                   CoreConstants.RepositoryEventsTable,
                //                                   beforeAfterClause,
                //                                   (whereClause != "") ? " WHERE " : "",
                //                                   whereClause);

                selectQuery = string.Format("SELECT count(r) FROM (select (case when e.guid is null then ROW_NUMBER() over" +
                                            "(partition by e.eventType, e.startSequence,e.startTime order by e.endSequence) else ROW_NUMBER() over" +
                                            "(partition by e.eventType, e.guid,e.startTime order by e.eventCategory) end) r FROM {0}..{1} AS e {2} {3}{4} ) as row where r=1",
                                                  SQLHelpers.CreateSafeDatabaseName(_eventDatabaseName),
                                                  CoreConstants.RepositoryEventsTable,
                                                  beforeAfterClause,
                                                  (whereClause != "") ? " WHERE " : "",
                                                  whereClause);
            }
            else
            {
                string joinClause = "";

                // We do not need a join if no filtering on Column/TableId
                if (Filter.IsBeforeAfterEnabled() && Filter.ColumnId != null)
                {
                    // If columnId, we need both joins
                    joinClause = string.Format(" JOIN {0}..{1} as d ON (e.eventId = d.eventId) " +
                                               " JOIN {0}..{2} as c ON (d.dcId = c.dcId) ",
                                               SQLHelpers.CreateSafeDatabaseName(_eventDatabaseName),
                                               CoreConstants.RepositoryDataChangesTable,
                                               CoreConstants.RepositoryColumnChangesTable);
                }
                else if (Filter.IsBeforeAfterEnabled() && Filter.TableId != null)
                {
                    // If only tableId, only need join on DataChanges
                    joinClause = string.Format(" JOIN {0}..{1} as d ON (e.eventId = d.eventId) ",
                                               SQLHelpers.CreateSafeDatabaseName(_eventDatabaseName),
                                               CoreConstants.RepositoryDataChangesTable);
                }

                //selectQuery = String.Format("SELECT count(distinct e.eventId) FROM {0}..{1} AS e {2} {3}{4}",
                //                                   SQLHelpers.CreateSafeDatabaseName(_eventDatabaseName),
                //                                   CoreConstants.RepositoryEventsTable,
                //                                   joinClause,
                //                                   (whereClause != "") ? " WHERE " : "",
                //                                   whereClause);

                selectQuery = string.Format("SELECT count(r) FROM (select (case when e.guid is null then ROW_NUMBER() over" +
                                            "(partition by e.eventType, e.startSequence,e.startTime order by e.endSequence) else ROW_NUMBER() over" +
                                            "(partition by e.eventType, e.guid,e.startTime order by e.eventCategory) end)  r,e.eventId FROM {0}..{1} AS e {2} {3}{4} ) as row where r=1",
                                                 SQLHelpers.CreateSafeDatabaseName(_eventDatabaseName),
                                                 CoreConstants.RepositoryEventsTable,
                                                 joinClause,
                                                 (whereClause != "") ? " WHERE " : "",
                                                 whereClause);
            }

            try
            {
                string strConn = string.Format("server={0};" +
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
                        {
                            count = 0;
                        }
                        else
                        {
                            count = (int)obj;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write("Unable to load EventCount.", e, true);
                count = -1;
            }
            lock (_bgLockObject)
            {
                if (myCounter == _bgCounter)
                {
                    _matchingEventCount = count;
                    _lastPage = _matchingEventCount / Settings.Default.EventPageSize - (_pageStarts.Count - 2);
                    if (_matchingEventCount % Settings.Default.EventPageSize == 0)
                    {
                        _lastPage--;
                    }

                    UpdateEventCountString();
                }
            }
        }

        private void UpdateEventCountString()
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(UpdateEventCountString));
                return;
            }
            if (_matchingEventCount != -1)
            {
                if (_matchingEventCount == 0)
                {
                    _lblNavigation.Value = string.Format("<b>{0}</b>", UIConstants.Grid_NoEvents);
                }
                else
                {
                    int currentPage;
                    if (_reversePaging)
                    {
                        currentPage = _matchingEventCount / Settings.Default.EventPageSize - (_pageStarts.Count - 2);
                        if (_matchingEventCount % Settings.Default.EventPageSize == 0)
                        {
                            currentPage--;
                        }

                        _lblNavigation.Value = string.Format("<b>Page {0} of {1} : {2} matching events</b>", currentPage, _lastPage, _matchingEventCount);
                        //if (currentPage == 1)
                        //{
                        //    _btnFirst.Enabled = false;
                        //    _btnPrevious.Enabled = false;
                        //}
                    }
                    else
                    {
                        currentPage = _pageStarts.Count;
                        //if (currentPage >= _lastPage)
                        //{
                        //    _lblNavigation.Value = String.Format("<b>Page {0} of {1} : {2} matching events</b>", _lastPage, _lastPage, _matchingEventCount);
                        //    _btnNext.Enabled = false;
                        //    _btnLast.Enabled = false;
                        //}
                        //else
                        //{
                        _lblNavigation.Value = string.Format("<b>Page {0} of {1} : {2} matching events</b>", currentPage, _lastPage, _matchingEventCount);
                        //    if (_lastPage > 1)
                        //    {
                        //        _btnNext.Enabled = true;
                        //        _btnLast.Enabled = true;
                        //    }
                        //}
                    }
                }
            }
            else
            {
                _lblNavigation.Value = string.Format("<b>Page {0} of ... : ... matching events</b>", _reversePaging ? "..." : _pageStarts.Count.ToString());
            }
        }

        private void LoadEventPage()
        {
            if (_pageStarts.Count == 0)
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
            
            _dsActive.Rows.Clear();
            _gridActive.Visible = false;
            _lblNavigation.Value = UIConstants.Grid_LoadingEvents;
            _lblStatus.Text = UIConstants.Grid_LoadingEvents;
            _lblStatus.Visible = true;
            _lblStatus.BringToFront();

            Cursor = Cursors.WaitCursor;
            _mainForm.UseWaitCursor = true;
            _mainForm.Cursor = Cursors.WaitCursor;

            _bgEventLoader.RunWorkerAsync();
        }

        private void DoWork_bgEventLoader(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (_viewMode == GridViewMode.Hierarchical)
                {
                    List<EventRow> eventRows = GetEventRowsEx();
                    e.Result = eventRows;
                }
                else
                {
                    List<FlatEventRow> eventRows = GetFlatEventRowsEx();
                    e.Result = eventRows;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write("LoadEventPage", ex);
            }
        }

        private void RunWorkerCompleted_bgEventLoader(object sender, RunWorkerCompletedEventArgs e)
        {
            Cursor = Cursors.Default;
            _mainForm.UseWaitCursor = false;
            _mainForm.Cursor = Cursors.Default;

            if (InvokeRequired)
            {
                object[] parameters = new object[1];
                parameters[0] = e.Result;
                if (_viewMode == GridViewMode.Hierarchical)
                {
                    Invoke(new VoidDelegateListEvents(LoadHierarchicalEventPage), parameters);
                }
                else
                {
                    Invoke(new VoidDelegateListFlatEvents(LoadFlatEventPage), parameters);
                }
            }
            else
            {
                if (_viewMode == GridViewMode.Hierarchical)
                {
                    LoadHierarchicalEventPage(e.Result as List<EventRow>);
                }
                else
                {
                    LoadFlatEventPage(e.Result as List<FlatEventRow>);
                }
            }
        }

        private void LoadFlatEventPage(List<FlatEventRow> eventRows)
        {
            try
            {
                _gridActive.BeginUpdate();

                _lblStatus.Visible = false;
                _lblStatus.SendToBack();

                int eventId = int.MinValue;
                int rowCount = 0;

                if (eventRows == null)
                {
                    // We have an error
                    _lblStatus.Visible = true;
                    _lblNavigation.Value = "<b>Error</b>";
                }
                else if (eventRows.Count == 0)
                {
                    // No data matchedt he criteria
                    _lblStatus.Text = UIConstants.Grid_NoEvents;
                    _lblNavigation.Value = string.Format("<b>{0}</b>", UIConstants.Grid_NoEvents);
                    _lblStatus.Visible = true;
                }
                else if (eventRows.Count == Settings.Default.EventPageSize + 1)
                {
                    // We have data with more available

                    // if reverse paging, invert the order of addition
                    //  This should be in-order; however, with no sorts
                    //  this is needed to order them by id
                    if (_reversePaging)
                    {
                        for (int i = Settings.Default.EventPageSize - 1; i >= 0; i--)
                        {
                            UltraDataRow row = _dsFlatEvents.Rows.Add();
                            UpdateRowValues(row, eventRows[i]);

                            // we are going in reverse, so handle it differently
                            if (eventId == int.MinValue)
                            {
                                eventId = eventRows[i].EventData.EventId;
                            }
                            else if (eventId == eventRows[i].EventData.EventId)
                            {
                                rowCount++;
                            }
                        }
                        _btnFirst.Enabled = true;
                        _btnPrevious.Enabled = true;
                    }
                    else
                    {
                        for (int i = 0; i < Settings.Default.EventPageSize; i++)
                        {
                            UltraDataRow row = _dsFlatEvents.Rows.Add();
                            UpdateRowValues(row, eventRows[i]);

                            // used to help paging on flat data
                            if (eventId == eventRows[i].EventData.EventId)
                            {
                                rowCount++;
                            }
                            else
                            {
                                eventId = eventRows[i].EventData.EventId;
                                rowCount = 0;
                            }
                        }
                        _btnLast.Enabled = true;
                        _btnNext.Enabled = true;
                    }
                    EventViewPageBookmark bookmark = new EventViewPageBookmark(eventRows[Settings.Default.EventPageSize]);
                    if (eventId == eventRows[Settings.Default.EventPageSize].EventData.EventId)
                    {
                        rowCount++;
                    }
                    else
                    {
                        rowCount = 0;
                    }

                    bookmark.JoinedRowCount = rowCount;
                    PushPageStart(bookmark);
                    UpdateEventCountString();
                }
                else
                {
                    // We are the last page.
                    if (_reversePaging)
                    {
                        for (int i = eventRows.Count - 1; i >= 0; i--)
                        {
                            UltraDataRow row = _dsFlatEvents.Rows.Add();
                            UpdateRowValues(row, eventRows[i]);
                        }
                        //_btnFirst.Enabled = true;
                        //_btnPrevious.Enabled = true;
                        _btnFirst.Enabled = false;
                        _btnPrevious.Enabled = false;
                    }
                    else
                    {
                        for (int i = 0; i < eventRows.Count; i++)
                        {
                            UltraDataRow row = _dsFlatEvents.Rows.Add();
                            UpdateRowValues(row, eventRows[i]);
                        }
                        //_btnLast.Enabled = true;
                        //_btnNext.Enabled = true;
                        _btnLast.Enabled = false;
                        _btnNext.Enabled = false;
                    }
                    // Placeholder to allow previous to continue working
                    //  correctly
                    PushPageStart(null);
                    UpdateEventCountString();
                }

                _gridActive.Visible = true;
                _gridActive.PerformAction(UltraGridAction.FirstRowInGrid);
                _gridActive.EndUpdate();
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write("LoadEventPage", e);
            }
        }

        private void PushPageStart(EventViewPageBookmark bookmark)
        {
            if (_pageStarts.Count == 0 || bookmark == null)
            {
                _pageStarts.Push(bookmark);
                return;
            }
            EventViewPageBookmark previous = _pageStarts.Peek();
            //if (previous != null)
            //{
            if (previous.EventId == bookmark.EventId)
            {
                bookmark.JoinedRowCount += previous.JoinedRowCount;
            }

            _pageStarts.Push(bookmark);
            //}
            //else if (_pageStarts.Count < _lastPage)
            //{
            //    _pageStarts.Push(bookmark);
            //}

        }

        private void LoadHierarchicalEventPage(List<EventRow> eventRows)
        {
            try
            {
                _gridActive.BeginUpdate();

                _lblStatus.Visible = false;
                _lblStatus.SendToBack();

                if (eventRows == null)
                {
                    // We have an error
                    _lblStatus.Visible = true;
                    _lblNavigation.Value = "<b>Error</b>";
                }
                else if (eventRows.Count == 0)
                {
                    // No data matchedt he criteria
                    _lblStatus.Text = UIConstants.Grid_NoEvents;
                    _lblNavigation.Value = string.Format("<b>{0}</b>", UIConstants.Grid_NoEvents);
                    _lblStatus.Visible = true;

                    _btnNext.Enabled = false;
                    _btnLast.Enabled = false;
                }
                else if (eventRows.Count == Settings.Default.EventPageSize + 1)
                {
                    // We have data with more available

                    // if reverse paging, invert the order of addition
                    //  This should be in-order; however, with no sorts
                    //  this is needed to order them by id
                    if (_reversePaging)
                    {
                        for (int i = Settings.Default.EventPageSize - 1; i >= 0; i--)
                        {
                            UltraDataRow row = _dsHierarchicalEvents.Rows.Add();
                            UpdateRowValues(row, eventRows[i]);
                        }
                        _btnFirst.Enabled = true;
                        _btnPrevious.Enabled = true;
                    }
                    else
                    {
                        for (int i = 0; i < Settings.Default.EventPageSize; i++)
                        {
                            UltraDataRow row = _dsHierarchicalEvents.Rows.Add();
                            UpdateRowValues(row, eventRows[i]);
                        }
                        _btnLast.Enabled = true;
                        _btnNext.Enabled = true;
                    }
                    PushPageStart(new EventViewPageBookmark(eventRows[Settings.Default.EventPageSize]));
                    UpdateEventCountString();
                }
                else
                {
                    //if (_matchingEventCount < Settings.Default.EventPageSize)
                    //{
                    //    for (int i = 0; i < eventRows.Count; i++)
                    //    {
                    //        UltraDataRow row = _dsHierarchicalEvents.Rows.Add();
                    //        UpdateRowValues(row, eventRows[i]);
                    //    }
                    //    _btnLast.Enabled = false;
                    //    _btnNext.Enabled = false;
                    //    if(eventRows.Count > 1)
                    //    PushPageStart(new EventViewPageBookmark(eventRows[eventRows.Count - 1]));
                    //}
                    //else
                    //{
                    // We are the last page.
                    if (_reversePaging)
                    {
                        for (int i = eventRows.Count - 1; i >= 0; i--)
                        {
                            UltraDataRow row = _dsHierarchicalEvents.Rows.Add();
                            UpdateRowValues(row, eventRows[i]);
                        }
                        _btnFirst.Enabled = false;
                        _btnPrevious.Enabled = false;
                        //_btnFirst.Enabled = true;
                        //_btnPrevious.Enabled = true;
                    }
                    else
                    {
                        for (int i = 0; i < eventRows.Count; i++)
                        {
                            UltraDataRow row = _dsHierarchicalEvents.Rows.Add();
                            UpdateRowValues(row, eventRows[i]);
                        }
                        //_btnLast.Enabled = true;
                        //_btnNext.Enabled = true;
                        _btnLast.Enabled = false;
                        _btnNext.Enabled = false;
                    }
                    //if (eventRows.Count > 1)
                    //    PushPageStart(new EventViewPageBookmark(eventRows[eventRows.Count - 1]));
                    // }
                    // Placeholder to allow previous to continue working
                    //  correctly
                    PushPageStart(null);
                    UpdateEventCountString();
                }

                _gridActive.Visible = true;
                _gridActive.PerformAction(UltraGridAction.FirstRowInGrid);
                _gridActive.EndUpdate();
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write("LoadEventPage", e);
            }
        }

        private void UpdateRowValues(UltraDataRow row, FlatEventRow record)
        {
            UpdateRowValues(row, record.EventData);
            UpdateRowValues(row, record.RowData);
            UpdateRowValues(row, record.ColumnData);
        }

        private void UpdateRowValues(UltraDataRow row, DataChangeRow record)
        {
            row["allchangedColumns"] = record.TotalChanges;
            row["changedColumns"] = record.ChangedColumns;
            row["primaryKey"] = record.PrimaryKey;
            row["tableName"] = record.TableName;

            // these columns duplicate existing data in the EventData rows.  As such,
            //  they are not included in Flat mode
            if (_viewMode == GridViewMode.Hierarchical)
            {
                CMEventType evType = Globals.Repository.LookupEventType(record.ActionType);
                if (Settings.Default.ShowLocalTime)
                {
                    row["Time"] = record.StartTime.ToLocalTime();
                    row["Date"] = record.StartTime.ToLocalTime();
                }
                else
                {
                    row["Time"] = TimeZoneInfo.ToLocalTime(_timeZoneInfo, record.StartTime);
                    row["Date"] = TimeZoneInfo.ToLocalTime(_timeZoneInfo, record.StartTime);
                }
                row["actionType"] = evType == null ? record.ActionType.ToString() : evType.Name;
                row["userName"] = record.UserName;
            }
        }

        private void UpdateRowValues(UltraDataRow row, ChangeColumnRow record)
        {
            row["columnName"] = record.ColumnName;
            row["beforeData"] = record.BeforeValue;
            row["afterData"] = record.AfterValue;
        }

        private void UpdateRowValues(UltraDataRow row, EventRow record)
        {
            CMEventCategory evCategory = Globals.Repository.LookupEventCategory(record.CategoryId);
            CMEventType evType = Globals.Repository.LookupEventType(record.EventTypeId);

            row["eventCategoryString"] = evCategory == null ? record.CategoryId.ToString() : evCategory.Name;
            row["eventTypeString"] = evType == null ? record.EventTypeId.ToString() : evType.Name;
            if (Settings.Default.ShowLocalTime)
            {
                row["Time"] = record.StartTime.ToLocalTime();
                row["Date"] = record.StartTime.ToLocalTime();
            }
            else
            {
                row["Time"] = TimeZoneInfo.ToLocalTime(_timeZoneInfo, record.StartTime);
                row["Date"] = TimeZoneInfo.ToLocalTime(_timeZoneInfo, record.StartTime);
            }
            row["loginName"] = record.LoginName;
            row["databaseName"] = record.DatabaseName;
            row["targetObject"] = record.TargetObjectName;
            row["details"] = record.Details;
            row["spid"] = record.Spid;
            row["applicationName"] = record.ApplicationName;
            row["hostName"] = record.HostName;
            row["serverName"] = record.ServerName;
            row["success"] = record.Success ? "Passed" : "Failed";
            row["dbUserName"] = record.DbUserName;
            row["objectName"] = record.ObjectName;
            row["targetLoginName"] = record.TargetLoginName;
            row["targetUserName"] = record.TargetUserName;
            row["roleName"] = record.RoleName;
            row["ownerName"] = record.OwnerName;
            row["privilegedUser"] = record.PrivilegedUser;
            row["sessionLoginName"] = record.SessionLoginName;
            row["parentName"] = record.ParentName;
            switch (record.CategoryId)
            {
                case 1:
                    row["icon"] = GUI.Properties.Resources.Event_Login_16;
                    break;
                case 2:
                    row["icon"] = GUI.Properties.Resources.Event_DDL_16;
                    break;
                case 3:
                    row["icon"] = GUI.Properties.Resources.Event_Security_16;
                    break;
                case 4:
                    row["icon"] = GUI.Properties.Resources.Event_DML_16;
                    break;
                case 5:
                    row["icon"] = GUI.Properties.Resources.Event_Select_16;
                    break;
                case 6:
                    row["icon"] = GUI.Properties.Resources.Event_Admin_16;
                    break;
                case 9:
                    row["icon"] = GUI.Properties.Resources.Event_UserDefined_16;
                    break;
            }

            if (_viewMode == GridViewMode.Hierarchical)
            {
                if (record.HasBeforeAfterData)
                {
                    UltraDataRow child = row.GetChildRows("Changed Rows").Add();
                    child["actionType"] = "Placeholder";
                }
                else if (record.HasSensitiveColumns)
                {
                    UltraDataRow sensitiveRow = row.GetChildRows("Sensitive Rows").Add();
                    sensitiveRow["actiontype"] = "Placeholder";
                }
            }

            row.Tag = record;
        }

        private List<EventRow> GetEventRowsEx()
        {
            List<EventRow> retVal;
            string query;
            int expectedRowCount;

            query = BuildStarterPageQuery(out expectedRowCount);
            retVal = GetEventRows(query);
            if (retVal == null)
            {
                return null;
            }

            // We got everything we wanted already
            if (retVal.Count == expectedRowCount)
            {
                return retVal;
            }

            // Are we a middle page?
            if (_pageStarts.Count > 0)
            {
                // In this case, we need to get more data.
                int rowsToGet = Settings.Default.EventPageSize - retVal.Count;
                query = BuildMiddlePageQuery(rowsToGet);
                List<EventRow> tmp = GetEventRows(query);
                if (tmp == null)
                {
                    return null;
                }

                retVal.AddRange(tmp);
            }

            return retVal;
        }

        private List<FlatEventRow> GetFlatEventRowsEx()
        {
            List<FlatEventRow> retVal;
            string query;
            int trimCount;

            query = BuildStarterPageQuery(out trimCount);
            retVal = GetFlatEventRows(query);
            if (retVal == null)
            {
                return null;
            }

            // Remove any addition rows fetched because of flat-mode
            retVal.RemoveRange(0, trimCount);

            // We got everything we wanted already
            if (retVal.Count == Settings.Default.EventPageSize + 1)
            {
                return retVal;
            }

            // Are we a middle page?
            if (_pageStarts.Count > 0)
            {
                // In this case, we need to get more data.
                int rowsToGet = Settings.Default.EventPageSize - retVal.Count;
                query = BuildMiddlePageQuery(rowsToGet);
                List<FlatEventRow> tmp = GetFlatEventRows(query);
                if (tmp == null)
                {
                    return null;
                }

                retVal.AddRange(tmp);
            }

            return retVal;
        }



        private List<EventRow> GetEventRows(string query)
        {
            List<EventRow> list = new List<EventRow>();
            try
            {
                using (SqlConnection conn = Globals.Repository.GetPooledConnection())
                {
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.CommandTimeout = CoreConstants.sqlcommandTimeout;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                EventRow row = EventRow.ReadRow(reader);
                                list.Add(row);
                                //if (list.Where(x => x.Guid == row.Guid && (x.CategoryId == row.CategoryId)).Count() == 0)
                                //{
                                //    list.Add(row);
                                //}
                                //else if ((row.Guid == string.Empty || row.Guid == null) &&
                                //       list.Where(x => x.StartSequence == row.StartSequence && (x.CategoryId == row.CategoryId)).Count() == 0)// Via Trace Events
                                //{
                                //    list.Add(row);
                                //}
                                //else if (row.CategoryId != 4)
                                //{
                                //    list.Add(row);
                                //}
                            }
                        }
                    }
                }
                return list;
            }
            catch (SqlException sqlEx)
            {
                if (sqlEx.Number == 208)
                {
                    // database gone
                    _lblStatus.Text = UIConstants.Grid_EventsDatabaseMissing;
                }
                else if (sqlEx.Number == 229)
                {
                    // database gone
                    _lblStatus.Text = UIConstants.Grid_UserDoesntHavePermission;
                }
                else
                {
                    _lblStatus.Text = string.Format("Class: {0}  Number: {1}  Message: {2}",
                       sqlEx.Class,
                       sqlEx.Number,
                       sqlEx.Message);
                }
            }
            catch (Exception ex)
            {
                _lblStatus.Text = ex.Message;
            }
            return null;
        }

        private List<FlatEventRow> GetFlatEventRows(string query)
        {
            List<FlatEventRow> list = new List<FlatEventRow>();
            try
            {
                using (SqlConnection conn = Globals.Repository.GetPooledConnection())
                {
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.CommandTimeout = CoreConstants.sqlcommandTimeout;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                FlatEventRow row = FlatEventRow.ReadRow(reader);
                                list.Add(row);
                            }
                        }
                    }
                }
                return list;
            }
            catch (SqlException sqlEx)
            {
                if (sqlEx.Number == 208)
                {
                    // database gone
                    _lblStatus.Text = UIConstants.Grid_EventsDatabaseMissing;
                }
                else if (sqlEx.Number == 229)
                {
                    // database gone
                    _lblStatus.Text = UIConstants.Grid_UserDoesntHavePermission;
                }
                else
                {
                    _lblStatus.Text = string.Format("Class: {0}  Number: {1}  Message: {2}",
                       sqlEx.Class,
                       sqlEx.Number,
                       sqlEx.Message);
                }
            }
            catch (Exception ex)
            {
                _lblStatus.Text = ex.Message;
            }
            _lblStatus.Visible = true;
            _lblStatus.BringToFront();
            return null;
        }

        //
        // This query is responsible for getting the first set of data when you are
        //  between the first and last pages.
        private string BuildStarterPageQuery(out int trimCount)
        {
            string whereClause;
            string orderByClause = BuildOrderByClause();
            string orderByPartiotionClause = BuildOrderByPartitionClause();
            int expectedRowCount;

            trimCount = 0;
            whereClause = _filter.GetWhereClause(_databaseName, _timeZoneInfo);

            // Make sure we aren't the first page being loaded
            //  
            if (_pageStarts.Count > 0)
            {
                EventViewPageBookmark pageStart = _pageStarts.Peek();
                //if (pageStart != null)
                //{
                trimCount = pageStart.JoinedRowCount;
                expectedRowCount = Settings.Default.EventPageSize + 1 + pageStart.JoinedRowCount;
                // Need to add paging conditions
                if (whereClause.Length > 0)
                {
                    whereClause = string.Format("e.startTime = {0} AND e.eventId {1} {2} AND {3}",
                       SQLHelpers.CreateSafeDateTime(pageStart.StartTime),
                       _reversePaging ? ">=" : "<=",
                       pageStart.EventId,
                       whereClause);
                }
                else
                {
                    whereClause = string.Format("e.startTime = {0} AND e.eventId {1} {2}",
                       SQLHelpers.CreateSafeDateTime(pageStart.StartTime),
                       _reversePaging ? ">=" : "<=",
                       pageStart.EventId);
                }
                //}
                //else
                //{
                //    expectedRowCount = Settings.Default.EventPageSize + 1;
                //    if (whereClause.Length == 0)
                //        whereClause = "1=1";
                //}
            }
            else
            {
                expectedRowCount = Settings.Default.EventPageSize + 1;
                if (whereClause.Length == 0)
                {
                    whereClause = "1=1";
                }
            }

            if (_viewMode == GridViewMode.Hierarchical)
            {
                string joinClause = "";

                // We do not need a join if no filtering on Column/TableId
                if (Filter.IsBeforeAfterEnabled() && Filter.ColumnId != null)
                {
                    // If columnId, we need both joins
                    joinClause = string.Format(" JOIN {0}..{1} as d ON (e.eventId = d.eventId) " +
                                               " JOIN {0}..{2} as c ON (d.dcId = c.dcId) ",
                                               SQLHelpers.CreateSafeDatabaseName(_eventDatabaseName),
                                               CoreConstants.RepositoryDataChangesTable,
                                               CoreConstants.RepositoryColumnChangesTable);
                }
                else if (Filter.IsBeforeAfterEnabled() && Filter.TableId != null)
                {
                    // If only tableId, only need join on DataChanges
                    joinClause = string.Format(" JOIN {0}..{1} as d ON (e.eventId = d.eventId) ",
                                               SQLHelpers.CreateSafeDatabaseName(_eventDatabaseName),
                                               CoreConstants.RepositoryDataChangesTable);
                }
                return string.Format("SELECT TOP {0} * from (select e.eventCategory,e.eventType,e.startTime,e.loginName," +
                   "e.databaseName,e.targetObject,e.details,e.eventId,e.spid,e.applicationName,e.hostName," +
                   "e.serverName,e.success,e.dbUserName,e.objectName,e.targetLoginName,e.targetUserName,e.roleName," +
                   "e.ownerName,e.privilegedUser,e.sessionLoginName,e.startSequence,e.endSequence,e.endTime,e.parentName," +
                   "hasSensitiveColumns = case when e.eventCategory = {1} " +
                   "then (select count(sc.columnName) from {2}..{3} sc where e.eventId = sc.eventId) else (select 0) END" +
                   ",e.guid,case when e.guid is null then ROW_NUMBER() over " +
                   "(partition by e.eventType, e.startSequence,e.startTime {7}) else ROW_NUMBER() over " +
                   "(partition by e.eventType, e.guid,e.startTime {7}) end as r" +
                   " FROM {2}..{4} as e {5} WHERE {6}) as row where r=1 {8}",
                   expectedRowCount,
                   (int)TraceEventCategory.SELECT,
                   SQLHelpers.CreateSafeDatabaseName(_eventDatabaseName),
                   CoreConstants.RepositorySensitiveColumnsTable,
                   CoreConstants.RepositoryEventsTable,
                   joinClause,
                   whereClause,
                   orderByPartiotionClause,
                   orderByClause.Replace("e.", "row."));
            }
            else
            {
                string beforeAfterClause = string.Format(" {0} JOIN {1}..{2} as d ON (e.eventId = d.eventId) " +
                   "LEFT OUTER JOIN {1}..{3} as c ON (d.dcId = c.dcId) ",
                   Filter.IsBeforeAfterEnabled() && (Filter.TableId != null || Filter.ColumnId != null) ? "" : "LEFT OUTER",
                   SQLHelpers.CreateSafeDatabaseName(_eventDatabaseName),
                   CoreConstants.RepositoryDataChangesTable,
                   CoreConstants.RepositoryColumnChangesTable);
                string totalChanges = string.Format("totalChanges = (SELECT Top 1 ISNULL(dc2.totalChanges, 0) from {0}..{1} dc2 where dc2.recordNumber = 0 and dc2.eventId = d.eventId and dc2.eventId IS NOT NULL),",
                                                    SQLHelpers.CreateSafeDatabaseName(_eventDatabaseName),
                                                    CoreConstants.RepositoryDataChangesTable);

                string beforeAfterColumns = ",d.startTime as startTime0,d.eventSequence as eventSequence0,d.spid as spid0,d.databaseId,d.actionType,d.tableName," +
                   "d.recordNumber,d.userName,d.changedColumns,d.primaryKey,d.hashcode as hashcode0," +
                   totalChanges +
                   "c.startTime as startTime1,c.eventSequence as eventSequence1,c.spid as spid1,c.columnName,c.beforeValue,c.afterValue,c.hashcode";

                // Ignore metadata records
                whereClause += " AND (d.recordNumber IS NULL OR d.recordNumber <> 0) ";

                return string.Format("SELECT TOP {0} * from (select e.eventCategory,e.eventType,e.startTime,e.loginName," +
                   "e.databaseName,e.targetObject,e.details,e.eventId,e.spid,e.applicationName,e.hostName," +
                   "e.serverName,e.success,e.dbUserName,e.objectName,e.targetLoginName,e.targetUserName,e.roleName," +
                   "e.ownerName,e.privilegedUser,e.sessionLoginName,e.startSequence,e.endSequence,e.endTime,e.parentName," +
                   "hasSensitiveColumns = case when e.eventCategory = {8} " +
                   "then (select count(sc.columnName) from {2}..{7} sc where e.eventId = sc.eventId) else (select 0) END" +
                   "{1}" +
                    ",e.guid,case when e.guid is null then ROW_NUMBER() over " +
                   "(partition by e.eventType, e.startSequence,e.startTime order by e.eventCategory) else ROW_NUMBER() over " +
                   "(partition by e.eventType, e.guid,e.startTime order by e.eventCategory) end as r" +
                   " FROM {2}..{3} as e {4} WHERE {5}) as row where r=1 {6}",
                   expectedRowCount,
                   beforeAfterColumns,
                   SQLHelpers.CreateSafeDatabaseName(_eventDatabaseName),
                   CoreConstants.RepositoryEventsTable,
                   beforeAfterClause,
                   whereClause, orderByClause.Replace("e.", "row."),
                   CoreConstants.RepositorySensitiveColumnsTable,
                   (int)TraceEventCategory.SELECT);


            }
        }

        private string BuildMiddlePageQuery(int rowCountToGet)
        {
            string whereClause;
            string orderByClause = BuildOrderByClause();
            string orderByPartiotionClause = BuildOrderByPartitionClause();

            whereClause = _filter.GetWhereClause(_databaseName, _timeZoneInfo);

            // Make sure we aren't the first page being loaded
            //  
            if (_pageStarts.Count > 0)
            {
                EventViewPageBookmark pageStart = _pageStarts.Peek();
                //if (pageStart != null)
                //{
                // Need to add paging conditions
                if (whereClause.Length > 0)
                {
                    whereClause = string.Format("e.startTime {0} {1} AND {2}",
                           _reversePaging ? ">" : "<",
                           SQLHelpers.CreateSafeDateTime(pageStart.StartTime),
                           whereClause);
                }
                else
                {
                    whereClause = string.Format("e.startTime {0} {1}",
                           _reversePaging ? ">" : "<",
                           SQLHelpers.CreateSafeDateTime(pageStart.StartTime));
                }
                //}
                //else
                //{
                //    if (whereClause.Length == 0)
                //        whereClause = "1=1";
                //}

            }
            else
            {
                // In this case, we should not be called
                return null;
            }

            if (_viewMode == GridViewMode.Hierarchical)
            {
                string joinClause = "";

                // We do not need a join if no filtering on Column/TableId
                if (Filter.IsBeforeAfterEnabled() && Filter.ColumnId != null)
                {
                    // If columnId, we need both joins
                    joinClause = string.Format(" JOIN {0}..{1} as d ON (e.eventId = d.eventId) " +
                                               " JOIN {0}..{2} as c ON (d.dcId = c.dcId) ",
                                               SQLHelpers.CreateSafeDatabaseName(_eventDatabaseName),
                                               CoreConstants.RepositoryDataChangesTable,
                                               CoreConstants.RepositoryColumnChangesTable);
                }
                else if (Filter.IsBeforeAfterEnabled() && Filter.TableId != null)
                {
                    // If only tableId, only need join on DataChanges
                    joinClause = string.Format(" JOIN {0}..{1} as d ON (e.eventId = d.eventId) ",
                                               SQLHelpers.CreateSafeDatabaseName(_eventDatabaseName),
                                               CoreConstants.RepositoryDataChangesTable);
                }

                return string.Format("SELECT TOP {0} * from (select e.eventCategory,e.eventType,e.startTime,e.loginName," +
                   "e.databaseName,e.targetObject,e.details,e.eventId,e.spid,e.applicationName,e.hostName," +
                   "e.serverName,e.success,e.dbUserName,e.objectName,e.targetLoginName,e.targetUserName,e.roleName," +
                   "e.ownerName,e.privilegedUser,e.sessionLoginName,e.startSequence,e.endSequence,e.endTime,e.parentName," +
                   "hasSensitiveColumns = case when e.eventCategory = {1} " +
                   "then (select count(sc.columnName) from {2}..{3} sc where e.eventId = sc.eventId) else (select 0) END" +
                   ",e.guid,case when e.guid is null then ROW_NUMBER() over (partition by e.eventType, e.startSequence,e.startTime {7}) " +
                   "else ROW_NUMBER() over(partition by e.eventType, e.guid,e.startTime {7}) end as r" +
                   " FROM {2}..{4} as e {5} WHERE {6} ) as row where r=1 {8}",
                   rowCountToGet + 1,
                   (int)TraceEventCategory.SELECT,
                   SQLHelpers.CreateSafeDatabaseName(_eventDatabaseName),
                   CoreConstants.RepositorySensitiveColumnsTable,
                   CoreConstants.RepositoryEventsTable,
                   joinClause,
                   whereClause,
                   orderByPartiotionClause,
                   orderByClause.Replace("e.", "row."));
            }
            else
            {
                string beforeAfterClause = string.Format(" {0} JOIN {1}..{2} as d ON (e.eventId = d.eventId) " +
                   "LEFT OUTER JOIN {1}..{3} as c ON (d.dcId = c.dcId) ",
                   Filter.IsBeforeAfterEnabled() && (Filter.TableId != null || Filter.ColumnId != null) ? "" : "LEFT OUTER",
                   SQLHelpers.CreateSafeDatabaseName(_eventDatabaseName),
                   CoreConstants.RepositoryDataChangesTable,
                   CoreConstants.RepositoryColumnChangesTable);

                string totalChanges = string.Format("totalChanges = (SELECT Top 1 ISNULL(dc2.totalChanges, 0) from {0}..{1} dc2 where dc2.recordNumber = 0 and dc2.eventId = d.eventId and dc2.eventId IS NOT NULL),",
                                                    SQLHelpers.CreateSafeDatabaseName(_eventDatabaseName),
                                                    CoreConstants.RepositoryDataChangesTable);

                string beforeAfterColumns = ",d.startTime as startTime0,d.eventSequence as eventSequence0,d.spid as spid0,d.databaseId,d.actionType,d.tableName," +
                   "d.recordNumber,d.userName,d.changedColumns,d.primaryKey,d.hashcode as hashcode0," +
                   totalChanges +
                   "c.startTime as startTime1,c.eventSequence as eventSequence1,c.spid as spid1,c.columnName,c.beforeValue,c.afterValue,c.hashcode as hashcode1";

                // Ignore metadata records
                whereClause += " AND (d.recordNumber IS NULL OR d.recordNumber <> 0) ";

                return string.Format("SELECT TOP {0} * from (select e.eventCategory,e.eventType,e.startTime,e.loginName," +
                                     "e.databaseName,e.targetObject,e.details,e.eventId,e.spid,e.applicationName,e.hostName," +
                                     "e.serverName,e.success,e.dbUserName,e.objectName,e.targetLoginName,e.targetUserName,e.roleName," +
                                     "e.ownerName,e.privilegedUser,e.sessionLoginName,e.startSequence,e.endSequence,e.endTime,e.parentName," +
                                     "hasSensitiveColumns = case when e.eventCategory = {8} " +
                                     "then (select count(sc.columnName) from {2}..{7} sc where e.eventId = sc.eventId) else (select 0) END" +
                                     "{1}" +
                                     ",e.guid,case when e.guid is null then ROW_NUMBER() over " +
                                     "(partition by e.eventType, e.startSequence,e.startTime order by e.eventCategory) else ROW_NUMBER() over " +
                                     "(partition by e.eventType, e.guid,e.startTime order by e.eventCategory) end as r" +
                                     " FROM {2}..{3} as e {4} WHERE {5}) as row where r=1 {6}",
                                     rowCountToGet + 1,
                                     beforeAfterColumns,
                                     SQLHelpers.CreateSafeDatabaseName(_eventDatabaseName),
                                     CoreConstants.RepositoryEventsTable,
                                     beforeAfterClause,
                                     whereClause, orderByClause.Replace("e.", "row."),
                                     CoreConstants.RepositorySensitiveColumnsTable,
                                     (int)TraceEventCategory.SELECT);
            }
        }

        public override void RefreshView()
        {
            if (_eventDatabaseName == null || _eventDatabaseName.Length == 0)
            {
                _dsActive.Rows.Clear();
                _pnlWarning.Visible = false;
                _lblStatus.Visible = true;
                _lblStatus.Text = UIConstants.List_NoArchives;
                RelocateStatusLabel();
                _lblNavigation.Value = "";
                return;
            }

            if (_bgEventLoader.IsBusy)
                return;

            _reversePaging = false;
            _pageStarts.Clear();
            _matchingEventCount = -1;
            _lblStatus.Visible = false;
            _pnlWarning.Visible = _eventDatabaseNeedsUpdate;
            RelocateStatusLabel();
            LoadEventPage();

            Thread t = new Thread(UpdateEventCount)
            {
                IsBackground = true
            };
            t.Start();
        }

        public override int Next()
        {
            GridHelper.SelectNextLeafRow(_gridActive, GetActiveEventListIndex());

            EventRow activeRow = GetActiveEvent();
            if (activeRow != null)
            {
                return activeRow.EventId;
            }
            else
            {
                return -1;
            }
        }

        public override int Previous()
        {
            GridHelper.SelectPreviousLeafRow(_gridActive, GetActiveEventListIndex());

            EventRow activeRow = GetActiveEvent();
            if (activeRow != null)
            {
                return activeRow.EventId;
            }
            else
            {
                return -1;
            }
        }

        public void ShowEventDetails()
        {
            EventRow theRecord = GetActiveEvent();
            if (theRecord == null)
            {
                return;
            }

            try
            {
                Cursor.Current = Cursors.WaitCursor;
                Form_EventProperties evProps = new Form_EventProperties(this, _eventDatabaseName, _currentServer, theRecord.EventId, _timeZoneInfo);
                Cursor.Current = Cursors.Default;
                evProps.ShowDialog(this);
            }
            catch (Exception)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show(this, string.Format("Event {0} is not available in the database.", theRecord.EventId), "Event Warning", MessageBoxButtons.OK,
                   MessageBoxIcon.Warning);
            }
        }

        public bool IsEventSelected()
        {
            return _gridActive.Selected.Rows.Count > 0;
        }

        private int GetActiveEventListIndex()
        {
            if (_gridActive.Selected.Rows.Count <= 0)
            {
                return -1;
            }
            else
            {
                UltraGridRow gridRow = _gridActive.Selected.Rows[0];
                UltraDataRow dataRow = gridRow.ListObject as UltraDataRow;
                if (dataRow != null)
                {
                    // If we have a child row selected in hierarchical mode, we need the parent.
                    while (dataRow.ParentRow != null)
                    {
                        dataRow = dataRow.ParentRow;
                    }
                }
                return dataRow != null ? dataRow.Index : -1;
            }
        }

        private EventRow GetActiveEvent()
        {
            if (_gridActive.Selected.Rows.Count <= 0)
            {
                return null;
            }
            else
            {
                UltraGridRow gridRow = _gridActive.Selected.Rows[0];
                UltraDataRow dataRow = gridRow.ListObject as UltraDataRow;
                if (dataRow != null)
                {
                    // If we have a child row selected in hierarchical mode, we need the parent.
                    while (dataRow.ParentRow != null)
                    {
                        dataRow = dataRow.ParentRow;
                    }
                }
                return dataRow != null ? dataRow.Tag as EventRow : null;
            }
        }


        #endregion // Actions

        #region Events

        private void DoubleClickRow_grid(object sender, DoubleClickRowEventArgs e)
        {
            ShowEventDetails();
        }

        private void Click_btnNext(object sender, EventArgs e)
        {
            if (_reversePaging)
            {
                _pageStarts.Pop();
                _pageStarts.Pop();
            }
            LoadEventPage();
            UpdateEventCountString();
        }

        private void Click_btnPrevious(object sender, EventArgs e)
        {
            if (!_reversePaging)
            {
                _pageStarts.Pop();
                _pageStarts.Pop();
            }
            LoadEventPage();
            UpdateEventCountString();
        }

        private void Click_btnFirst(object sender, EventArgs e)
        {
            _pageStarts.Clear();
            _reversePaging = false;
            LoadEventPage();
            UpdateEventCountString();
        }

        private void Click_btnLast(object sender, EventArgs e)
        {
            _pageStarts.Clear();
            _reversePaging = true;
            LoadEventPage();
            UpdateEventCountString();
        }

        private void MouseDown_grid(object sender, MouseEventArgs e)
        {
            UIElement elementMain;
            UIElement elementUnderMouse;

            elementMain = _gridActive.DisplayLayout.UIElement;
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
                            _gridActive.Selected.Rows.Clear();
                            cell.Row.Selected = true;
                            _gridActive.ActiveRow = cell.Row;
                        }
                    }
                }
                else
                {
                    HeaderUIElement he = elementUnderMouse.GetAncestor(typeof(HeaderUIElement)) as HeaderUIElement;
                    if (he == null)
                    {
                        _gridActive.Selected.Rows.Clear();
                        _gridActive.ActiveRow = null;
                    }
                }
            }
        }

        #endregion // Events

        public override void CollapseAll()
        {
            _gridActive.Rows.CollapseAll(true);
        }

        public override void ExpandAll()
        {
            _internalUpdate++;
            try
            {
                _gridActive.Rows.ExpandAll(true);
            }
            finally
            {
                _internalUpdate--;
            }
        }

        public override void HelpOnThisWindow()
        {
            if (_isArchive)
            {
                HelpAlias.ShowHelp(this, HelpAlias.SSHELP_ArchiveView);
            }
            else
            {
                HelpAlias.ShowHelp(this, HelpAlias.SSHELP_EventView);
            }
        }

        private void RelocateStatusLabel()
        {
            UIElement elem = _gridActive.DisplayLayout.Bands[0].Header.GetUIElement();
            int yOffset = 0;
            if (_pnlWarning.Visible)
            {
                yOffset += _pnlWarning.Height;
            }

            if (elem != null)
            {
                yOffset += elem.Rect.Bottom;
            }

            _lblStatus.Location = new Point(_lblStatus.Location.X, yOffset);
        }

        protected override void OnShowGroupByChanged(ToggleChangedEventArgs e)
        {
            base.OnShowGroupByChanged(e);
            if (e.Enabled)
            {
                _gridFlat.DisplayLayout.ViewStyleBand = ViewStyleBand.OutlookGroupBy;
                _gridHierarchical.DisplayLayout.ViewStyleBand = ViewStyleBand.OutlookGroupBy;
            }
            else
            {
                _gridFlat.DisplayLayout.ViewStyleBand = ViewStyleBand.Horizontal;
                _gridHierarchical.DisplayLayout.ViewStyleBand = ViewStyleBand.Vertical;
            }
            RelocateStatusLabel();
        }

        public override void Properties()
        {
            ShowEventDetails();
        }

        private void UpdateMenuFlags()
        {
            bool legalRow = _gridActive.Selected.Rows.Count == 1;
            SetMenuFlag(CMMenuItem.Properties, legalRow && Globals.isAdmin && _gridActive.Focused);
        }

        private void KeyDown_grid(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ShowEventDetails();
            }
        }

        private void FocusChanged_grid(object sender, EventArgs e)
        {
            UpdateMenuFlags();
        }

        private string BuildOrderByClause()
        {
            //string orderClause = string.Empty;
            //if (!_reversePaging)
            //{
            //    if (Filter.DBLevelPrivUser)
            //        orderClause = " ORDER BY e.startTime DESC, e.startSequence, e.eventId DESC ";
            //    else if (!Filter.DBLevelPrivUser)
            //        orderClause = " ORDER BY e.startTime DESC, e.startSequence DESC, e.eventId DESC ";
            //}
            //else
            //    orderClause = " ORDER BY e.startTime ASC, e.startSequence ASC, e.eventId ASC ";

            if (!_reversePaging)
            {
                return " ORDER BY e.startTime DESC, e.startSequence DESC, e.eventId DESC ";
            }
            else
            {
                return " ORDER BY e.startTime ASC, e.startSequence ASC, e.eventId ASC ";
            }

            //return orderClause;
        }

        private string BuildOrderByPartitionClause()
        {
            if (!_reversePaging)
            {
                return " ORDER BY e.endSequence ASC,e.privilegedUser ASC,e.startTime DESC, e.startSequence DESC, e.eventId DESC ";
            }
            else
            {
                return " ORDER BY e.endSequence ASC,e.privilegedUser ASC,e.startTime ASC, e.startSequence ASC, e.eventId ASC ";
            }
        }

        //-------------------------------------------------------------
        // GetServerInfo
        //--------------------------------------------------------------
        private void GetServerInfo(string instance)
        {
            _currentServer = ServerRecord.GetServer(Globals.Repository.Connection, instance);
            if (_currentServer != null)
            {
                if (Settings.Default.ShowLocalTime)
                {
                    // get local adjustment
                    _timeZoneInfo = TimeZoneInfo.CurrentTimeZone;
                }
                else
                {
                    // server time
                    _timeZoneInfo = _currentServer.GetTimeZoneInfo();
                }

                _eventDatabaseName = _currentServer.EventDatabase;
            }
            if (_eventDatabaseName != null && _eventDatabaseName.Length > 0)
            {
                CheckVersion();
            }
        }

        private void GetServerInfo(ArchiveRecord archive)
        {
            if (archive != null)
            {
                _currentServer = ServerRecord.GetServer(Globals.Repository.Connection, archive.Instance);
            }

            if (_currentServer != null)
            {
                if (Settings.Default.ShowLocalTime)
                {
                    // get local adjustment
                    _timeZoneInfo = TimeZoneInfo.CurrentTimeZone;
                }
                else
                {
                    // server time
                    _timeZoneInfo = _currentServer.GetTimeZoneInfo();
                }
            }
            if (archive != null)
            {
                _eventDatabaseName = archive.DatabaseName;
            }
            else
            {
                _eventDatabaseName = null;
            }

            if (_eventDatabaseName != null && _eventDatabaseName.Length > 0)
            {
                CheckVersion();
            }
        }

        private void CheckVersion()
        {
            try
            {
                int dbVersion = EventDatabase.GetDatabaseSchemaVersion(Globals.Repository.Connection, _eventDatabaseName);
                if (dbVersion < Globals.SQLcomplianceConfig.EventsDbSchemaVersion)
                {
                    _eventDatabaseNeedsUpdate = true;
                }
                else
                {
                    _eventDatabaseNeedsUpdate = false;
                }
            }
            catch (Exception)
            {
                // Gobble this if database is not available
                _eventDatabaseNeedsUpdate = false;
            }
        }

        public void LoadServerEvents(ServerRecord record)
        {
            _databaseName = "";
            GetServerInfo(record.Instance);
            RefreshView();
        }

        public void LoadArchiveServerEvents(ArchiveRecord archive)
        {
            _databaseName = "";
            GetServerInfo(archive);
            RefreshView();
        }

        public void LoadArchiveDatabaseEvents(DatabaseRecord record, ArchiveRecord archive)
        {
            _databaseName = record.Name;
            GetServerInfo(archive);
            RefreshView();
        }

        public void LoadDatabaseEvents(DatabaseRecord record)
        {
            _databaseName = record.Name;
            GetServerInfo(record.SrvInstance);
            RefreshView();
        }

        private void LinkClicked_lnkWarning(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form_RepositoryOptions frm = new Form_RepositoryOptions(1);
            frm.ShowDialog();
            CheckVersion();
            RefreshView();
        }

        private void AfterSelectChange_grid(object sender, AfterSelectChangeEventArgs e)
        {
            UpdateMenuFlags();
        }

        private void BeforeRowExpanded_gridHierarchical(object sender, CancelableRowEventArgs e)
        {
            UltraDataRow dataRow = e.Row.ListObject as UltraDataRow;

            if (dataRow == null)
            {
                return;
            }

            if (!(dataRow.Tag is EventRow))
            {
                return;
            }

            if (_internalUpdate > 0)
            {
                e.Cancel = true;
                return;
            }

            UltraDataRowsCollection childRows = dataRow.GetChildRows("Changed Rows");
            if (childRows.Count == 1 && Equals(childRows[0]["actionType"], "Placeholder"))
            {
                // We have placeholder data - load new data.
                EventRow rowData = (EventRow)dataRow.Tag;
                List<DataChangeRecord> dcRecords;

                if (Filter.TableId == null)
                {
                    dcRecords = DataChangeRecord.GetDataChangeRecords(Globals.Repository.Connection, _eventDatabaseName, rowData.EventId);
                }
                else
                {
                    dcRecords = DataChangeRecord.GetDataChangeRecords(Globals.Repository.Connection, _eventDatabaseName, rowData.EventId,
                       Filter.TableId.GetValueOrDefault());
                }
                childRows.Clear();
                foreach (DataChangeRecord dcRecord in dcRecords)
                {
                    List<ColumnChangeRecord> ccRecords;
                    bool ignoreRow = false;
                    if (Filter.ColumnId == null)
                    {
                        ccRecords = ColumnChangeRecord.GetColumnChangeRecords(Globals.Repository.Connection, _eventDatabaseName, dcRecord);
                    }
                    else
                    {
                        ccRecords = ColumnChangeRecord.GetColumnChangeRecords(Globals.Repository.Connection, _eventDatabaseName, dcRecord,
                           Filter.ColumnId.GetValueOrDefault());
                        if (ccRecords.Count == 0)
                        {
                            ignoreRow = true;
                        }
                    }
                    // We ignore DataChange rows that do not having a matching child columnId row (when ColumnID filter is in use)
                    if (ignoreRow)
                    {
                        continue;
                    }

                    DataChangeRow dcRow = new DataChangeRow(dcRecord);
                    UltraDataRow dcDataRow = childRows.Add();
                    UpdateRowValues(dcDataRow, dcRow);

                    long? rowCount = null;
                    //Add the list of columns, read them from the columns table.
                    EventRecord ev = new EventRecord(Globals.Repository.Connection, _eventDatabaseName);
                    if (ev.Read(rowData.EventId))
                    {
                        rowCount = ev.rowCounts;
                    }
                    foreach (ColumnChangeRecord ccRecord in ccRecords)
                    {
                        ChangeColumnRow ccRow = new ChangeColumnRow(ccRecord);
                        UltraDataRow childRow = dcDataRow.GetChildRows("Changed Columns").Add();
                        UpdateRowValues(childRow, ccRow);
                        childRow["rowCounts"] = rowCount.HasValue ? "1" : string.Empty;
                    }
                }
            }
            else
            {
                UltraDataRowsCollection sensitiveRows = dataRow.GetChildRows("Sensitive Rows");

                //load the Sensitive row data if we have it.
                if (sensitiveRows.Count == 1 && Equals(sensitiveRows[0]["actionType"], "Placeholder"))
                {
                    EventRow rowData = (EventRow)dataRow.Tag;
                    UltraDataRow sensitiveRow = sensitiveRows[0];

                    // these columns duplicate existing data in the EventData rows.  As such,
                    //  they are not included in Flat mode
                    CMEventType evType = Globals.Repository.LookupEventType(rowData.EventTypeId);
                    if (Settings.Default.ShowLocalTime)
                    {
                        sensitiveRow["Time"] = rowData.StartTime.ToLocalTime();
                        sensitiveRow["Date"] = rowData.StartTime.ToLocalTime();
                    }
                    else
                    {
                        sensitiveRow["Time"] = TimeZoneInfo.ToLocalTime(_timeZoneInfo, rowData.StartTime);
                        sensitiveRow["Date"] = TimeZoneInfo.ToLocalTime(_timeZoneInfo, rowData.StartTime);
                    }
                    sensitiveRow["actionType"] = evType == null ? rowData.CategoryId.ToString() : evType.Name;
                    sensitiveRow["applicationName"] = rowData.ApplicationName;
                    sensitiveRow["hostname"] = rowData.HostName;
                    sensitiveRow["databaseName"] = rowData.DatabaseName;
                    sensitiveRow["loginName"] = rowData.LoginName;

                    List<SensitiveColumnRecord> sensitiveColumns = SensitiveColumnRecord.GetSensitiveColumnRecords(Globals.Repository.Connection, _eventDatabaseName, rowData.EventId);
                    UltraDataRow columnRow;
                    long? rowCount = null;

                    //Add the list of columns, read them from the columns table.
                    EventRecord ev = new EventRecord(Globals.Repository.Connection, _eventDatabaseName);
                    if (ev.Read(rowData.EventId))
                    {
                        rowCount = ev.rowCounts;
                    }
                    //Add the list of columns, read them from the columns table.
                    foreach (SensitiveColumnRecord record in sensitiveColumns)
                    {
                        columnRow = sensitiveRow.GetChildRows("Sensitive Columns").Add();
                        columnRow["columnName"] = record.columnName;
                        columnRow["rowcounts"] = rowCount;
                    }
                }
            }
        }
    }

    internal class EventViewPageBookmark
    {
        private int _eventId;
        private DateTime _startTime;
        private int _joinedRowCount = 0;

        public EventViewPageBookmark(EventRow row)
        {
            _eventId = row.EventId;
            _startTime = row.StartTime;
        }

        public EventViewPageBookmark(FlatEventRow row) : this(row.EventData)
        {
        }

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

        public int JoinedRowCount
        {
            get { return _joinedRowCount; }
            set { _joinedRowCount = value; }
        }
    }
}
