using Idera.SQLcompliance.Application.GUI.Forms;
using Idera.SQLcompliance.Application.GUI.Helper;
using Idera.SQLcompliance.Application.GUI.SQL;
using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Agent;
using Idera.SQLcompliance.Core.Rules;
using Idera.SQLcompliance.Core.Rules.Alerts;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Idera.SQLcompliance.Application.GUI.Controls
{
    public partial class RibbonView : BaseControl
    {
        public enum Tabs
        {
            EnterpriseSummary = 0,
            ServerSummary = 1,
            DatabaseSummary = 2,
            Alerts = 3,
            DataAlerts = 4,
            Events = 5,
            Archive = 6,
            StatusAlerts = 7,
        }

        private const string CREATE_INDEX = "Create index";
        private const string TRACE_STARTED = "Trace started";
        private const string TRACE_STOPPED = "Trace stopped";

        private uint _internalUpdate = 0;

        private bool _alertViewDirty;
        private CustomTime _alertsCustomTime;
        private ComboBoxTool _comboAlertViews;
        private ComboBoxTool _comboAlertTime;
        private ComboBoxTool _comboAlertLevel;
        private StateButtonTool _checkAlertDefaultView;
        private StateButtonTool _checkAlertGroupBy;
        private ButtonTool _btnAlertSave;
        private ButtonTool _btnAlertExpandAll;
        private ButtonTool _btnAlertCollapseAll;
        private ButtonTool _cmAlertMessage;
        private ButtonTool _cmAlertEventDetails;
        private int _alertTimeFilterLastIndex;

        private bool _dataAlertViewDirty;
        private CustomTime _dataAlertsCustomTime;
        private ComboBoxTool _comboDataAlertViews;
        private ComboBoxTool _comboDataAlertTime;
        private ComboBoxTool _comboDataAlertLevel;
        private StateButtonTool _checkDataAlertDefaultView;
        private StateButtonTool _checkDataAlertGroupBy;
        private ButtonTool _btnDataAlertSave;
        private ButtonTool _btnDataAlertExpandAll;
        private ButtonTool _btnDataAlertCollapseAll;
        private ButtonTool _cmDataAlertMessage;
        private ButtonTool _cmDataAlertEventDetails;
        private int _dataAlertTimeFilterLastIndex;

        private bool _statusAlertViewDirty;
        private CustomTime _statusAlertsCustomTime;
        private ComboBoxTool _comboStatusAlertViews;
        private ComboBoxTool _comboStatusAlertTime;
        private ComboBoxTool _comboStatusAlertLevel;
        private StateButtonTool _checkStatusAlertDefaultView;
        private StateButtonTool _checkStatusAlertGroupBy;
        private ButtonTool _btnStatusAlertSave;
        private ButtonTool _btnStatusAlertExpandAll;
        private ButtonTool _btnStatusAlertCollapseAll;
        private int _statusAlertTimeFilterLastIndex;

        private bool _eventViewDirty;
        private CustomTime _eventsCustomTime;
        private Dictionary<string, ValueListItem> _categories; // Name -> ValueListItem
        private ComboBoxTool _comboEventViews;
        private ComboBoxTool _comboEventTime;
        private ComboBoxTool _comboEventCategory;
        private ComboBoxTool _comboEventType;
        private ComboBoxTool _comboEventLogin;
        private ComboBoxTool _comboEventApplication;
        private ComboBoxTool _comboEventHost;
        private ComboBoxTool _comboEventTable;
        private ComboBoxTool _comboEventColumn;
        private StateButtonTool _checkEventDefaultView;
        private StateButtonTool _checkEventGroupBy;
        private ButtonTool _btnEventSave;
        private ButtonTool _btnEventExpandAll;
        private ButtonTool _btnEventCollapseAll;
        private ButtonTool _cmEventDetails;
        private StateButtonTool _checkEventFlatMode;
        private RibbonGroup _groupEventBeforeAfter;

        private int _eventTimeFilterLastIndex;

        private bool _archiveEventViewDirty;
        private CustomTime _archiveCustomTime;
        private Dictionary<string, ValueListItem> _archiveCategories; // Name -> ValueListItem
        private ComboBoxTool _comboArchiveList;
        private ButtonTool _btnArchiveDetach;
        private ComboBoxTool _comboArchiveViews;
        private ComboBoxTool _comboArchiveTime;
        private ComboBoxTool _comboArchiveCategory;
        private ComboBoxTool _comboArchiveType;
        private ComboBoxTool _comboArchiveLogin;
        private ComboBoxTool _comboArchiveApplication;
        private ComboBoxTool _comboArchiveHost;
        private ComboBoxTool _comboArchiveTable;
        private ComboBoxTool _comboArchiveColumn;
        private StateButtonTool _checkArchiveDefaultView;
        private StateButtonTool _checkArchiveGroupBy;
        private ButtonTool _btnArchiveSave;
        private ButtonTool _btnArchiveExpandAll;
        private ButtonTool _btnArchiveCollapseAll;
        private ButtonTool _cmArchiveDetails;
        private ButtonTool _btnArchiveProperties;
        private StateButtonTool _checkArchiveFlatMode;
        private RibbonGroup _groupArchiveBeforeAfter;
        private int _archiveTimeFilterLastIndex;

        private BaseControl _activeControl;

        private bool _updateEventFilterDisplays;  //  We can't up update displays until a server or db is selected

        private List<ArchiveRecord> _currentArchives;

        private BackgroundWorker _bgLoader;

        private object _scope;

        public RibbonView()
        {
            PopupMenuTool popup;
            _currentArchives = new List<ArchiveRecord>();

            InitializeComponent();

            _bgLoader = new BackgroundWorker
            {
                WorkerSupportsCancellation = true
            };
            _bgLoader.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunWorkerCompleted_bgLoader);
            _bgLoader.DoWork += new DoWorkEventHandler(DoWork_bgLoader);

            _enterpriseSummaryView.DaysToShow = Settings.Default.TimeSpan;
            _serverSummaryView.DaysToShow = Settings.Default.TimeSpan;
            _databaseSummaryView.DaysToShow = Settings.Default.TimeSpan;
            switch (Settings.Default.TimeSpan)
            {
                case 1:
                    ((StateButtonTool)_toolbarManager.Tools["summaryTime1"]).InitializeChecked(true);
                    break;
                case 7:
                    ((StateButtonTool)_toolbarManager.Tools["summaryTime7"]).InitializeChecked(true);
                    break;
                case 30:
                    ((StateButtonTool)_toolbarManager.Tools["summaryTime30"]).InitializeChecked(true);
                    break;
            }
            _statusAlertsCustomTime = new CustomTime();
            _dataAlertsCustomTime = new CustomTime();
            _alertsCustomTime = new CustomTime();
            _eventsCustomTime = new CustomTime();
            _archiveCustomTime = new CustomTime();

            _activeControl = _enterpriseSummaryView;
            _scope = null;
            _toolbarManager.Ribbon.Tabs[(int)Tabs.Events].Visible = false;
            _toolbarManager.Ribbon.Tabs[(int)Tabs.Archive].Visible = false;

            // Alert Setup
            _comboAlertViews = (ComboBoxTool)_toolbarManager.Tools["alertViewsSelector"];
            _comboAlertTime = (ComboBoxTool)_toolbarManager.Tools["alertFiltersTime"];
            _comboAlertLevel = (ComboBoxTool)_toolbarManager.Tools["alertFiltersLevel"];
            _checkAlertDefaultView = (StateButtonTool)_toolbarManager.Tools["alertViewsDefault"];
            _checkAlertGroupBy = (StateButtonTool)_toolbarManager.Tools["alertGroupsEnable"];
            _btnAlertCollapseAll = (ButtonTool)_toolbarManager.Tools["alertGroupsCollapse"];
            _btnAlertExpandAll = (ButtonTool)_toolbarManager.Tools["alertGroupsExpand"];
            _btnAlertSave = (ButtonTool)_toolbarManager.Tools["alertViewsSave"];
            _btnAlertSave.SharedProps.Enabled = false;
            popup = (PopupMenuTool)_toolbarManager.Tools["contextAlerts"];
            _cmAlertMessage = (ButtonTool)popup.Tools["alertMessage"];
            _cmAlertEventDetails = (ButtonTool)popup.Tools["viewProperties"];
            _alertView.Grid.AfterColPosChanged += new AfterColPosChangedEventHandler(AfterColPosChanged_AlertGrid);
            _alertView.Grid.AfterSortChange += new BandEventHandler(AfterSortChange_AlertGrid);

            // Data Alert Setup
            _comboDataAlertViews = (ComboBoxTool)_toolbarManager.Tools["dataAlertViewsSelector"];
            _comboDataAlertTime = (ComboBoxTool)_toolbarManager.Tools["dataAlertFiltersTime"];
            _comboDataAlertLevel = (ComboBoxTool)_toolbarManager.Tools["dataAlertFiltersLevel"];
            _checkDataAlertDefaultView = (StateButtonTool)_toolbarManager.Tools["dataAlertViewsDefault"];
            _checkDataAlertGroupBy = (StateButtonTool)_toolbarManager.Tools["dataAlertGroupsEnable"];
            _btnDataAlertCollapseAll = (ButtonTool)_toolbarManager.Tools["dataAlertGroupsCollapse"];
            _btnDataAlertExpandAll = (ButtonTool)_toolbarManager.Tools["dataAlertGroupsExpand"];
            _btnDataAlertSave = (ButtonTool)_toolbarManager.Tools["dataAlertViewsSave"];
            _btnDataAlertSave.SharedProps.Enabled = false;
            popup = (PopupMenuTool)_toolbarManager.Tools["contextDataAlerts"];
            _cmDataAlertMessage = (ButtonTool)popup.Tools["dataAlertMessage"];
            _cmDataAlertEventDetails = (ButtonTool)popup.Tools["viewProperties"];
            _dataAlertView.Grid.AfterColPosChanged += new AfterColPosChangedEventHandler(AfterColPosChanged_DataAlertGrid);
            _dataAlertView.Grid.AfterSortChange += new BandEventHandler(AfterSortChange_DataAlertGrid);

            // Status Alert Setup
            _comboStatusAlertViews = (ComboBoxTool)_toolbarManager.Tools["statusAlertViewsSelector"];
            _comboStatusAlertTime = (ComboBoxTool)_toolbarManager.Tools["statusAlertFiltersTime"];
            _comboStatusAlertLevel = (ComboBoxTool)_toolbarManager.Tools["statusAlertFiltersLevel"];
            _checkStatusAlertDefaultView = (StateButtonTool)_toolbarManager.Tools["statusAlertViewsDefault"];
            _checkStatusAlertGroupBy = (StateButtonTool)_toolbarManager.Tools["statusAlertGroupsEnable"];
            _btnStatusAlertCollapseAll = (ButtonTool)_toolbarManager.Tools["statusAlertGroupsCollapse"];
            _btnStatusAlertExpandAll = (ButtonTool)_toolbarManager.Tools["statusAlertGroupsExpand"];
            _btnStatusAlertSave = (ButtonTool)_toolbarManager.Tools["statusAlertViewsSave"];
            _btnAlertSave.SharedProps.Enabled = false;
            _statusAlertView.Grid.AfterColPosChanged += new AfterColPosChangedEventHandler(AfterColPosChanged_StatusAlertGrid);
            _statusAlertView.Grid.AfterSortChange += new BandEventHandler(AfterSortChange_StatusAlertGrid);
            InitializeAlertViewCombo();

            // Event Setup
            _comboEventViews = (ComboBoxTool)_toolbarManager.Tools["eventViewsSelector"];
            _comboEventTime = (ComboBoxTool)_toolbarManager.Tools["eventFiltersTime"];
            _comboEventCategory = (ComboBoxTool)_toolbarManager.Tools["eventFiltersCategory"];
            _comboEventType = (ComboBoxTool)_toolbarManager.Tools["eventFiltersType"];
            _comboEventLogin = (ComboBoxTool)_toolbarManager.Tools["eventFiltersLogin"];
            _comboEventApplication = (ComboBoxTool)_toolbarManager.Tools["eventFiltersApplication"];
            _comboEventHost = (ComboBoxTool)_toolbarManager.Tools["eventFiltersHost"];
            _comboEventTable = (ComboBoxTool)_toolbarManager.Tools["eventBATable"];
            _comboEventColumn = (ComboBoxTool)_toolbarManager.Tools["eventBAColumn"];
            _checkEventDefaultView = (StateButtonTool)_toolbarManager.Tools["eventViewsDefault"];
            _checkEventGroupBy = (StateButtonTool)_toolbarManager.Tools["eventGroupsEnable"];
            _btnEventCollapseAll = (ButtonTool)_toolbarManager.Tools["eventGroupsCollapse"];
            _btnEventExpandAll = (ButtonTool)_toolbarManager.Tools["eventGroupsExpand"];
            _btnEventSave = (ButtonTool)_toolbarManager.Tools["eventViewsSave"];
            _btnEventSave.SharedProps.Enabled = false;
            _checkEventFlatMode = (StateButtonTool)_toolbarManager.Tools["eventBAFlatten"];
            popup = (PopupMenuTool)_toolbarManager.Tools["contextEvents"];
            _cmEventDetails = (ButtonTool)popup.Tools["viewProperties"];
            _groupEventBeforeAfter = _toolbarManager.Ribbon.Tabs["ribbonAuditEvents"].Groups["ribbonGroupEventsBeforeAfter"];
            _eventView.GetGrid(GridViewMode.Hierarchical).AfterColPosChanged += new AfterColPosChangedEventHandler(AfterColPosChanged_EventGrid);
            _eventView.GetGrid(GridViewMode.Hierarchical).AfterSortChange += new BandEventHandler(AfterSortChange_EventGrid);
            _eventView.GetGrid(GridViewMode.Flat).AfterColPosChanged += new AfterColPosChangedEventHandler(AfterColPosChanged_EventGrid);
            _eventView.GetGrid(GridViewMode.Flat).AfterSortChange += new BandEventHandler(AfterSortChange_EventGrid);

            // Archive Setup
            _comboArchiveList = (ComboBoxTool)_toolbarManager.Tools["archiveSelector"];
            _btnArchiveDetach = (ButtonTool)_toolbarManager.Tools["archiveDetach"];
            _btnArchiveProperties = (ButtonTool)_toolbarManager.Tools["archiveProperties"];
            _comboArchiveViews = (ComboBoxTool)_toolbarManager.Tools["archiveViewSelector"];
            _comboArchiveTime = (ComboBoxTool)_toolbarManager.Tools["archiveTime"];
            _comboArchiveCategory = (ComboBoxTool)_toolbarManager.Tools["archiveCategory"];
            _comboArchiveType = (ComboBoxTool)_toolbarManager.Tools["archiveType"];
            _comboArchiveLogin = (ComboBoxTool)_toolbarManager.Tools["archiveLogin"];
            _comboArchiveApplication = (ComboBoxTool)_toolbarManager.Tools["archiveApplication"];
            _comboArchiveHost = (ComboBoxTool)_toolbarManager.Tools["archiveHost"];
            _comboArchiveTable = (ComboBoxTool)_toolbarManager.Tools["archiveBATable"];
            _comboArchiveColumn = (ComboBoxTool)_toolbarManager.Tools["archiveBAColumn"];
            _checkArchiveDefaultView = (StateButtonTool)_toolbarManager.Tools["archiveDefaultView"];
            _checkArchiveGroupBy = (StateButtonTool)_toolbarManager.Tools["archiveEnableGroups"];
            _btnArchiveCollapseAll = (ButtonTool)_toolbarManager.Tools["archiveCollapseAll"];
            _btnArchiveExpandAll = (ButtonTool)_toolbarManager.Tools["archiveExpandAll"];
            _btnArchiveSave = (ButtonTool)_toolbarManager.Tools["archiveSave"];
            _btnArchiveSave.SharedProps.Enabled = false;
            _checkArchiveFlatMode = (StateButtonTool)_toolbarManager.Tools["archiveBAFlatten"];
            popup = (PopupMenuTool)_toolbarManager.Tools["contextArchive"];
            _cmArchiveDetails = (ButtonTool)popup.Tools["archiveEventProperties"];
            _groupArchiveBeforeAfter = _toolbarManager.Ribbon.Tabs["ribbonArchiveEvents"].Groups["ribbonGroupArchiveBeforeAfter"];
            _archiveEventView.GetGrid(GridViewMode.Hierarchical).AfterColPosChanged += new AfterColPosChangedEventHandler(AfterColPosChanged_ArchiveGrid);
            _archiveEventView.GetGrid(GridViewMode.Hierarchical).AfterSortChange += new BandEventHandler(AfterSortChange_ArchiveGrid);
            _archiveEventView.GetGrid(GridViewMode.Flat).AfterColPosChanged += new AfterColPosChangedEventHandler(AfterColPosChanged_ArchiveGrid);
            _archiveEventView.GetGrid(GridViewMode.Flat).AfterSortChange += new BandEventHandler(AfterSortChange_ArchiveGrid);
            _archiveEventView.IsArchive = true;

            // This initializeds the view combos for events and archives, so we do it after
            //  intiailization for both
            InitializeEventViewCombo();
        }

        #region Properties

        public AlertingConfiguration AlertConfiguration
        {
            set
            {
                _alertView.AlertConfiguration = value;
                _statusAlertView.AlertConfiguration = value;
                _dataAlertView.AlertConfiguration = value;
            }
        }

        public override bool ShowGroupBy
        {
            get
            {
                return base.ShowGroupBy;
            }

            set
            {
                _alertView.ShowGroupBy = value;
                _statusAlertView.ShowGroupBy = value;
                _dataAlertView.ShowGroupBy = value;
                _eventView.ShowGroupBy = value;
                _archiveEventView.ShowGroupBy = value;
                base.ShowGroupBy = value;
            }
        }

        #endregion Properties


        public override void UpdateColors()
        {
            base.UpdateColors();
            _enterpriseSummaryView.UpdateColors();
            _serverSummaryView.UpdateColors();
            _databaseSummaryView.UpdateColors();
            _alertView.UpdateColors();
            _statusAlertView.UpdateColors();
            _dataAlertView.UpdateColors();
            _eventView.UpdateColors();
            _archiveEventView.UpdateColors();
        }

        public void SetScope(ServerRecord s)
        {
            RibbonTab activeTab = _toolbarManager.Ribbon.SelectedTab;
            _internalUpdate++;
            try
            {
                _scope = s;
                if (s.IsAuditedServer)
                {
                    _toolbarManager.Ribbon.Tabs[(int)Tabs.ServerSummary].Visible = true;
                    _toolbarManager.Ribbon.Tabs[(int)Tabs.Alerts].Visible = true;
                    _toolbarManager.Ribbon.Tabs[(int)Tabs.DataAlerts].Visible = true;
                    _toolbarManager.Ribbon.Tabs[(int)Tabs.Events].Visible = true;
                    _toolbarManager.Ribbon.Tabs[(int)Tabs.Archive].Visible = true;
                    _toolbarManager.Ribbon.Tabs[(int)Tabs.EnterpriseSummary].Visible = false;
                    _toolbarManager.Ribbon.Tabs[(int)Tabs.DatabaseSummary].Visible = false;
                    _toolbarManager.Ribbon.Tabs[(int)Tabs.StatusAlerts].Visible = false;
                    if (!activeTab.Visible)
                    {
                        _toolbarManager.Ribbon.SelectedTab = _toolbarManager.Ribbon.Tabs[(int)Tabs.ServerSummary];
                    }

                    _alertView.SetScope(s);
                    _statusAlertView.SetScope(s);
                    _dataAlertView.SetScope(s);
                    InitializeEventViewFilterCombos();
                }
                else
                {
                    // Archive-only server
                    _toolbarManager.Ribbon.Tabs[(int)Tabs.Archive].Visible = true;
                    _toolbarManager.Ribbon.Tabs[(int)Tabs.EnterpriseSummary].Visible = false;
                    _toolbarManager.Ribbon.Tabs[(int)Tabs.ServerSummary].Visible = false;
                    _toolbarManager.Ribbon.Tabs[(int)Tabs.DatabaseSummary].Visible = false;
                    _toolbarManager.Ribbon.Tabs[(int)Tabs.Alerts].Visible = false;
                    _toolbarManager.Ribbon.Tabs[(int)Tabs.DataAlerts].Visible = false;
                    _toolbarManager.Ribbon.Tabs[(int)Tabs.Events].Visible = false;
                    _toolbarManager.Ribbon.Tabs[(int)Tabs.StatusAlerts].Visible = false;
                    if (!activeTab.Visible)
                    {
                        _toolbarManager.Ribbon.SelectedTab = _toolbarManager.Ribbon.Tabs[(int)Tabs.Archive];
                    }
                }
                InitializeArchiveViewFilterCombos();
                LoadArchiveList();
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose, "SetScope(ServerRecord", e, ErrorLog.Severity.Warning);
            }
            _internalUpdate--;
            //if (_toolbarManager.Ribbon.SelectedTab != activeTab)
            {
                // Force an update after we've reset the ribbon tabs
                RibbonTabEventArgs args = new RibbonTabEventArgs(_toolbarManager.Ribbon.SelectedTab);
                AfterRibbonTabSelected_toolbarManager(null, args);
            }
        }

        public void SetScope(DatabaseRecord d)
        {
            RibbonTab activeTab = _toolbarManager.Ribbon.SelectedTab;
            _internalUpdate++;
            try
            {
                _scope = d;
                _toolbarManager.Ribbon.Tabs[(int)Tabs.DatabaseSummary].Visible = true;
                _toolbarManager.Ribbon.Tabs[(int)Tabs.Events].Visible = true;
                _toolbarManager.Ribbon.Tabs[(int)Tabs.Archive].Visible = true;
                _toolbarManager.Ribbon.Tabs[(int)Tabs.EnterpriseSummary].Visible = false;
                _toolbarManager.Ribbon.Tabs[(int)Tabs.ServerSummary].Visible = false;
                _toolbarManager.Ribbon.Tabs[(int)Tabs.Alerts].Visible = false;
                _toolbarManager.Ribbon.Tabs[(int)Tabs.DataAlerts].Visible = false;
                _toolbarManager.Ribbon.Tabs[(int)Tabs.StatusAlerts].Visible = false;
                if (!activeTab.Visible)
                {
                    _toolbarManager.Ribbon.SelectedTab = _toolbarManager.Ribbon.Tabs[(int)Tabs.DatabaseSummary];
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose, "SetScope(DatabaseRecord)", e, ErrorLog.Severity.Warning);
            }
            InitializeEventViewFilterCombos();
            InitializeArchiveViewFilterCombos();
            LoadArchiveList();
            _internalUpdate--;
            //if (_toolbarManager.Ribbon.SelectedTab != activeTab)
            {
                // Force an update after we've reset the ribbon tabs
                RibbonTabEventArgs args = new RibbonTabEventArgs(_toolbarManager.Ribbon.SelectedTab);
                AfterRibbonTabSelected_toolbarManager(null, args);
            }
        }

        public void SetScope()
        {
            RibbonTab activeTab = _toolbarManager.Ribbon.SelectedTab;
            _internalUpdate++;
            try
            {
                _scope = null;
                _toolbarManager.Ribbon.Tabs[(int)Tabs.EnterpriseSummary].Visible = true;
                _toolbarManager.Ribbon.Tabs[(int)Tabs.Alerts].Visible = true;
                _toolbarManager.Ribbon.Tabs[(int)Tabs.DataAlerts].Visible = true;
                _toolbarManager.Ribbon.Tabs[(int)Tabs.StatusAlerts].Visible = true;
                _toolbarManager.Ribbon.Tabs[(int)Tabs.ServerSummary].Visible = false;
                _toolbarManager.Ribbon.Tabs[(int)Tabs.DatabaseSummary].Visible = false;
                _toolbarManager.Ribbon.Tabs[(int)Tabs.Events].Visible = false;
                _toolbarManager.Ribbon.Tabs[(int)Tabs.Archive].Visible = false;
                if (!activeTab.Visible)
                {
                    _toolbarManager.Ribbon.SelectedTab = _toolbarManager.Ribbon.Tabs[(int)Tabs.EnterpriseSummary];
                }

                _alertView.SetScope();
                _statusAlertView.SetScope();
                _dataAlertView.SetScope();
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose, "SetScope", e, ErrorLog.Severity.Warning);
            }
            _internalUpdate--;
            //if (_toolbarManager.Ribbon.SelectedTab != activeTab)
            {
                // Force an update after we've reset the ribbon tabs
                RibbonTabEventArgs args = new RibbonTabEventArgs(_toolbarManager.Ribbon.SelectedTab);
                AfterRibbonTabSelected_toolbarManager(null, args);
            }
        }

        public void ShowTab(Tabs tab)
        {
            _toolbarManager.Ribbon.SelectedTab = _toolbarManager.Ribbon.Tabs[(int)tab];
        }

        public void ShowTab(Tabs tab, int reportCardIndex)
        {
            _toolbarManager.Ribbon.SelectedTab = _toolbarManager.Ribbon.Tabs[(int)tab];
            if (tab == Tabs.EnterpriseSummary)
            {
                _enterpriseSummaryView.ShowTab(reportCardIndex);
            }
            else if (tab == Tabs.ServerSummary)
            {
                _serverSummaryView.ShowTab(reportCardIndex);
            }
        }

        public Tabs GetActiveTab()
        {
            if (_activeControl == _alertView)
            {
                return Tabs.Alerts;
            }
            else if (_activeControl == _statusAlertView)
            {
                return Tabs.StatusAlerts;
            }
            else if (_activeControl == _dataAlertView)
            {
                return Tabs.DataAlerts;
            }
            else if (_activeControl == _enterpriseSummaryView)
            {
                return Tabs.EnterpriseSummary;
            }
            else if (_activeControl == _serverSummaryView)
            {
                return Tabs.ServerSummary;
            }
            else if (_activeControl == _databaseSummaryView)
            {
                return Tabs.DatabaseSummary;
            }
            else if (_activeControl == _eventView)
            {
                return Tabs.Events;
            }
            else if (_activeControl == _archiveEventView)
            {
                return Tabs.Archive;
            }

            return Tabs.EnterpriseSummary;
        }

        public override void RefreshView()
        {
            if (_bgLoader.IsBusy)
            {
                _bgLoader.CancelAsync();
                return;
            }

            try
            {
                if (_activeControl == _alertView)
                {
                    _alertView.RefreshView();
                }
                else if (_activeControl == _statusAlertView)
                {
                    _statusAlertView.RefreshView();
                }
                else if (_activeControl == _dataAlertView)
                {
                    _dataAlertView.RefreshView();
                }
                else if (_activeControl == _archiveEventView)
                {
                    // This is a startup issue and connection change issue
                    if (_updateEventFilterDisplays)
                    {
                        UpdateEventFilterDisplay();
                        UpdateArchiveFilterDisplay();
                        _updateEventFilterDisplays = false;
                    }
                    if (_scope is DatabaseRecord)
                    {
                        DatabaseRecord record = _scope as DatabaseRecord;
                        _archiveEventView.LoadArchiveDatabaseEvents(record, GetSelectedArchive());
                    }
                    else if (_scope is ServerRecord)
                    {
                        ServerRecord record = _scope as ServerRecord;
                        _archiveEventView.LoadArchiveServerEvents(GetSelectedArchive());
                    }
                    else
                    {
                    }
                }
                else if (_activeControl == _eventView)
                {
                    // This is a startup issue and connection change issue
                    if (_updateEventFilterDisplays)
                    {
                        UpdateEventFilterDisplay();
                        UpdateArchiveFilterDisplay();
                        _updateEventFilterDisplays = false;
                    }
                    if (_scope is DatabaseRecord)
                    {
                        DatabaseRecord record = _scope as DatabaseRecord;
                        if (record.AuditPrivUsersList != null && record.AuditPrivUsersList != "")
                        {
                            _eventView.Filter.DBLevelPrivUser = true;
                        }
                        _eventView.LoadDatabaseEvents(record);
                    }
                    else if (_scope is ServerRecord)
                    {
                        ServerRecord record = _scope as ServerRecord;
                        if (record.AuditUsersList != null && record.AuditUsersList != "")
                        {
                            _eventView.Filter.DBLevelPrivUser = false;

                        }
                        _eventView.LoadServerEvents(record);
                    }
                    else
                    {
                    }
                }
                else if (_activeControl == _enterpriseSummaryView)
                {
                    _enterpriseSummaryView.RefreshView();
                }
                else if (_activeControl == _serverSummaryView)
                {
                    ServerRecord record = _scope as ServerRecord;
                    _serverSummaryView.RefreshView(record);
                }
                else if (_activeControl == _databaseSummaryView)
                {
                    DatabaseRecord record = _scope as DatabaseRecord;
                    _databaseSummaryView.RefreshView(record);
                }
            }
            catch (Exception)
            {
                Cursor = Cursors.Default;
                _mainForm.UseWaitCursor = false;
                _mainForm.Cursor = Cursors.Default;
            }
        }

        private void DoWork_bgLoader(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (_activeControl == _alertView)
                {
                    _alertView.RefreshView();
                }
                else if (_activeControl == _statusAlertView)
                {
                    _statusAlertView.RefreshView();
                }
                else if (_activeControl == _dataAlertView)
                {
                    _dataAlertView.RefreshView();
                }
                else if (_activeControl == _archiveEventView)
                {
                    // This is a startup issue and connection change issue
                    if (_updateEventFilterDisplays)
                    {
                        UpdateEventFilterDisplay();
                        UpdateArchiveFilterDisplay();
                        _updateEventFilterDisplays = false;
                    }
                    if (_scope is DatabaseRecord)
                    {
                        DatabaseRecord record = _scope as DatabaseRecord;
                        _archiveEventView.LoadArchiveDatabaseEvents(record, GetSelectedArchive());
                    }
                    else if (_scope is ServerRecord)
                    {
                        ServerRecord record = _scope as ServerRecord;
                        _archiveEventView.LoadArchiveServerEvents(GetSelectedArchive());
                    }
                    else
                    {
                    }
                }
                else if (_activeControl == _eventView)
                {
                    // This is a startup issue and connection change issue
                    if (_updateEventFilterDisplays)
                    {
                        UpdateEventFilterDisplay();
                        UpdateArchiveFilterDisplay();
                        _updateEventFilterDisplays = false;
                    }
                    if (_scope is DatabaseRecord)
                    {
                        DatabaseRecord record = _scope as DatabaseRecord;
                        if (record.AuditPrivUsersList != null && record.AuditPrivUsersList != "")
                        {
                            _eventView.Filter.DBLevelPrivUser = true;
                        }
                        _eventView.LoadDatabaseEvents(record);
                    }
                    else if (_scope is ServerRecord)
                    {
                        ServerRecord record = _scope as ServerRecord;
                        if (record.AuditUsersList != null && record.AuditUsersList != "")
                        {
                            _eventView.Filter.DBLevelPrivUser = false;

                        }
                        _eventView.LoadServerEvents(record);
                    }
                    else
                    {
                    }
                }
                else if (_activeControl == _enterpriseSummaryView)
                {
                    _enterpriseSummaryView.RefreshView();
                }
                else if (_activeControl == _serverSummaryView)
                {
                    ServerRecord record = _scope as ServerRecord;
                    _serverSummaryView.RefreshView(record);
                }
                else if (_activeControl == _databaseSummaryView)
                {
                    DatabaseRecord record = _scope as DatabaseRecord;
                    _databaseSummaryView.RefreshView(record);
                }
            }
            catch (Exception)
            {
                Cursor = Cursors.Default;
                _mainForm.UseWaitCursor = false;
                _mainForm.Cursor = Cursors.Default;
            }

            if ((sender as BackgroundWorker).CancellationPending)
            {
                e.Cancel = true;
            }

            Cursor = Cursors.Default;
            _mainForm.UseWaitCursor = false;
            _mainForm.Cursor = Cursors.Default;
        }

        private void RunWorkerCompleted_bgLoader(object sender, RunWorkerCompletedEventArgs e)
        {
            Cursor = Cursors.Default;
            _mainForm.UseWaitCursor = false;
            _mainForm.Cursor = Cursors.Default;

            if (e.Cancelled)
            {
                RefreshView();
            }
        }

        private ArchiveRecord GetSelectedArchive()
        {
            ValueListItem item = (ValueListItem)_comboArchiveList.SelectedItem;
            if (item != null)
            {
                return item.DataValue as ArchiveRecord;
            }
            else
            {
                return null;
            }
        }

        public override bool GetMenuFlag(CMMenuItem item)
        {
            BaseControl ctrl = _activeControl;

            if (ctrl != null)
            {
                return ctrl.GetMenuFlag(item);
            }
            else
            {
                return base.GetMenuFlag(item);
            }
        }

        public override void Delete()
        {
            BaseControl ctrl = _activeControl;
            base.Delete();
            if (ctrl != null)
            {
                ctrl.Delete();
            }
        }

        public override void Properties()
        {
            base.Properties();
            BaseControl ctrl = _activeControl;
            if (ctrl != null)
            {
                ctrl.Properties();
            }
        }

        public override void CollapseAll()
        {
            base.CollapseAll();
            BaseControl ctrl = _activeControl;
            if (ctrl != null)
            {
                ctrl.CollapseAll();
            }
        }

        public override void ExpandAll()
        {
            base.ExpandAll();
            BaseControl ctrl = _activeControl;
            if (ctrl != null)
            {
                ctrl.ExpandAll();
            }
        }

        public override void HelpOnThisWindow()
        {
            base.HelpOnThisWindow();
            BaseControl ctrl = _activeControl;
            if (ctrl != null)
            {
                ctrl.HelpOnThisWindow();
            }
        }

        public override void Initialize(Form_Main2 mainForm)
        {
            base.Initialize(mainForm);
            _alertView.Initialize(mainForm);
            _archiveEventView.Initialize(mainForm);
            _eventView.Initialize(mainForm);
            _enterpriseSummaryView.Initialize(mainForm);
            _serverSummaryView.Initialize(mainForm);
            _databaseSummaryView.Initialize(mainForm);
            _statusAlertView.Initialize(mainForm);
            _dataAlertView.Initialize(mainForm);
            mainForm.ConnectionChanged += new EventHandler<ConnectionChangedEventArgs>(ConnectionChanged_mainForm);
            mainForm.ArchiveAttached += new EventHandler<ArchiveEventArgs>(ArchiveAttached_mainForm);
            mainForm.ArchiveDetached += new EventHandler<ArchiveEventArgs>(ArchiveDetached_mainForm);
        }

        private void ArchiveDetached_mainForm(object sender, ArchiveEventArgs e)
        {
            LoadArchiveList();
            if (_comboArchiveList.ValueList.ValueListItems.Count == 0)
            {
                RefreshView();
            }
        }

        private void ArchiveAttached_mainForm(object sender, ArchiveEventArgs e)
        {
            LoadArchiveList();
        }

        private void ConnectionChanged_mainForm(object sender, ConnectionChangedEventArgs e)
        {
            if (e.Repository != null && _comboAlertViews.SelectedItem != null)
            {
                InitializeCombos(e.Repository.GetExcludableEventCategories());

                ValueListItem item = (ValueListItem)_comboAlertViews.SelectedItem;
                ApplyAlertView(item.DataValue as AlertViewSettings);

                item = (ValueListItem)_comboStatusAlertViews.SelectedItem;
                ApplyStatusAlertView(item.DataValue as StatusAlertViewSettings);

                item = (ValueListItem)_comboDataAlertViews.SelectedItem;
                ApplyDataAlertView(item.DataValue as DataAlertViewSettings);

                item = (ValueListItem)_comboEventViews.SelectedItem;
                _updateEventFilterDisplays = true;
                ApplyEventView(item.DataValue as EventViewSettings);

                item = (ValueListItem)_comboArchiveViews.SelectedItem;
                ApplyArchiveView(item.DataValue as EventViewSettings);

                SetButtonPermissions();
            }
        }

        private void SetButtonPermissions()
        {
            // enterprise summary
            _toolbarManager.Tools["registerServer"].SharedProps.Enabled = Globals.isAdmin;

            // Server summary
            _toolbarManager.Tools["deleteServer"].SharedProps.Enabled = Globals.isAdmin;
            _toolbarManager.Tools["fileNewDatabase"].SharedProps.Enabled = Globals.isAdmin;
            _toolbarManager.Tools["auditingDisable"].SharedProps.Enabled = Globals.isAdmin;
            _toolbarManager.Tools["importAuditSettings"].SharedProps.Enabled = Globals.isAdmin;
            _toolbarManager.Tools["auditCollectNow"].SharedProps.Enabled = Globals.isAdmin;
            _toolbarManager.Tools["auditUpdateSettings"].SharedProps.Enabled = Globals.isAdmin;
            _toolbarManager.Tools["applyAuditTemplate"].SharedProps.Enabled = Globals.isAdmin;

            // Database Summary
            _toolbarManager.Tools["removeDatabase"].SharedProps.Enabled = Globals.isAdmin;
            _toolbarManager.Tools["disableAuditing"].SharedProps.Enabled = Globals.isAdmin;
            _toolbarManager.Tools["importDatabaseSettings"].SharedProps.Enabled = Globals.isAdmin;
        }

        private void MenuFlagChanged_child(object sender, MenuFlagChangedEventArgs e)
        {
            OnMenuFlagChanged(e);
        }

        private void AfterRibbonTabSelected_toolbarManager(object sender, RibbonTabEventArgs e)
        {
            if (_internalUpdate > 0)
            {
                return;
            }

            UpdateSelectedTab(e.Tab.Index);
            BaseControl b = _activeControl;
            foreach (CMMenuItem i in Enum.GetValues(typeof(CMMenuItem)))
            {
                SetMenuFlag(i, b.GetMenuFlag(i));
            }
        }

        private void UpdateSelectedTab(int tabIndex)
        {
            BaseControl previousControl = _activeControl;
            BaseControl newControl;
            switch ((Tabs)tabIndex)
            {
                case Tabs.EnterpriseSummary:
                    newControl = _enterpriseSummaryView;
                    break;
                case Tabs.ServerSummary:
                    newControl = _serverSummaryView;
                    break;
                case Tabs.DatabaseSummary:
                    newControl = _databaseSummaryView;
                    break;
                case Tabs.Alerts:
                    newControl = _alertView;
                    break;
                case Tabs.StatusAlerts:
                    newControl = _statusAlertView;
                    break;
                case Tabs.DataAlerts:
                    newControl = _dataAlertView;
                    break;
                case Tabs.Events:
                    newControl = _eventView;
                    break;
                case Tabs.Archive:
                    newControl = _archiveEventView;
                    break;
                default:
                    return;
            }
            newControl.Enabled = true;
            newControl.BringToFront();
            if (previousControl != null && previousControl != newControl)
                previousControl.Enabled = false;
            _activeControl = newControl;
            RefreshView();
        }

        private void ToolClick_toolbarManager(object sender, ToolClickEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "summaryTime1":
                    _enterpriseSummaryView.DaysToShow = 1;
                    _serverSummaryView.DaysToShow = 1;
                    _databaseSummaryView.DaysToShow = 1;
                    Settings.Default.TimeSpan = 1;
                    RefreshView();
                    break;
                case "summaryTime7":
                    _enterpriseSummaryView.DaysToShow = 7;
                    _serverSummaryView.DaysToShow = 7;
                    _databaseSummaryView.DaysToShow = 7;
                    Settings.Default.TimeSpan = 7;
                    RefreshView();
                    break;
                case "summaryTime30":
                    _enterpriseSummaryView.DaysToShow = 30;
                    _serverSummaryView.DaysToShow = 30;
                    _databaseSummaryView.DaysToShow = 30;
                    Settings.Default.TimeSpan = 30;
                    RefreshView();
                    break;

                //event Alerts
                case "alertViewsDefault":
                    Click_AlertDefaultView();
                    break;
                case "alertViewsSelectColumns":
                    _alertView.Grid.ShowColumnChooser();
                    break;
                case "alertViewsSave":
                    Click_AlertViewsSave();
                    break;
                case "alertViewsSaveAs":
                    Click_AlertViewsSaveAs();
                    break;
                case "alertViewsDelete":
                    Click_AlertViewsDelete();
                    break;
                case "alertGroupsExpand":
                    _alertView.ExpandAll();
                    break;
                case "alertGroupsCollapse":
                    _alertView.CollapseAll();
                    break;
                case "alertGroupsEnable":
                    Click_AlertEnableGroupBy();
                    break;
                case "alertMessage":
                    _alertView.ShowAlertMessage();
                    break;

                //data Alerts
                case "dataAlertViewsDefault":
                    Click_DataAlertDefaultView();
                    break;
                case "dataAlertViewsSelectColumns":
                    _dataAlertView.Grid.ShowColumnChooser();
                    break;
                case "dataAlertViewsSave":
                    Click_DataAlertViewsSave();
                    break;
                case "dataAlertViewsSaveAs":
                    Click_DataAlertViewsSaveAs();
                    break;
                case "dataAlertViewsDelete":
                    Click_DataAlertViewsDelete();
                    break;
                case "dataAlertGroupsExpand":
                    _dataAlertView.ExpandAll();
                    break;
                case "dataAlertGroupsCollapse":
                    _dataAlertView.CollapseAll();
                    break;
                case "dataAlertContextGroupsEnable":
                    _checkDataAlertGroupBy.Checked = !_checkDataAlertGroupBy.Checked;
                    Click_DataAlertEnableGroupBy();
                    break;
                case "dataAlertGroupsEnable":
                    Click_DataAlertEnableGroupBy();
                    break;
                case "dataAlertMessage":
                    _dataAlertView.ShowAlertMessage();
                    break;

                //Status alerts
                case "statusAlertViewsDefault":
                    Click_StatusAlertDefaultView();
                    break;
                case "statusAlertViewsSelectColumns":
                    _statusAlertView.Grid.ShowColumnChooser();
                    break;
                case "statusAlertViewsSave":
                    Click_StatusAlertViewsSave();
                    break;
                case "statusAlertViewsSaveAs":
                    Click_StatusAlertViewsSaveAs();
                    break;
                case "statusAlertViewsDelete":
                    Click_StatusAlertViewsDelete();
                    break;
                case "statusAlertGroupsExpand":
                    _statusAlertView.ExpandAll();
                    break;
                case "statusAlertGroupsCollapse":
                    _statusAlertView.CollapseAll();
                    break;
                case "statusAlertGroupsEnable":
                    Click_StatusAlertEnableGroupBy();
                    break;
                case "statusAlertMessage":
                    _statusAlertView.ShowAlertMessage();
                    break;
                case "viewProperties":
                    if (_activeControl == _alertView)
                    {
                        _alertView.ShowEventDetails();
                    }
                    else if (_activeControl == _dataAlertView)
                    {
                        _dataAlertView.ShowEventDetails();
                    }
                    else if (_activeControl == _eventView)
                    {
                        _eventView.ShowEventDetails();
                    }

                    break;
                case "viewRefresh":
                    RefreshView();
                    break;
                case "eventViewsSelectColumns":
                    _eventView.ActiveGrid.ShowColumnChooser();
                    break;
                case "eventViewsSave":
                    Click_EventViewsSave();
                    break;
                case "eventViewsSaveAs":
                    Click_EventViewsSaveAs();
                    break;
                case "eventViewsDelete":
                    Click_EventViewsDelete();
                    break;
                case "eventGroupsExpand":
                    _eventView.ExpandAll();
                    break;
                case "eventGroupsCollapse":
                    _eventView.CollapseAll();
                    break;
                case "eventGroupsEnable":
                    Click_EventEnableGroupBy();
                    break;
                case "eventViewsDefault":
                    Click_EventViewDefault();
                    break;
                case "eventBAFlatten":
                    if (_checkEventFlatMode.Checked)
                    {
                        SetEventGridViewMode(GridViewMode.Flat);
                    }
                    else
                    {
                        SetEventGridViewMode(GridViewMode.Hierarchical);
                    }

                    break;
                case "registerServer":
                    _mainForm.AddServerAction();
                    break;
                case "viewChangeLog":
                    _mainForm.NavigateToView(ConsoleViews.AdminChangeLog);
                    break;
                case "viewLogins":
                    _mainForm.NavigateToView(ConsoleViews.AdminLogins);
                    break;
                case "checkIntegrity":
                    _mainForm.CheckIntegrity(null);
                    break;
                case "summaryEnterpriseSearch":
                    break;
                case "viewAlertRules":
                    _mainForm.NavigateToView(ConsoleViews.AdminAlertRules);
                    break;
                case "archiveSelectColumns":
                    _archiveEventView.ActiveGrid.ShowColumnChooser();
                    break;
                case "archiveSave":
                    Click_ArchiveViewsSave();
                    break;
                case "archiveSaveAs":
                    Click_ArchiveViewsSaveAs();
                    break;
                case "archiveDeleteView":
                    Click_ArchiveViewsDelete();
                    break;
                case "archiveExpandAll":
                    _archiveEventView.ExpandAll();
                    break;
                case "archiveCollapseAll":
                    _archiveEventView.CollapseAll();
                    break;
                case "archiveEnableGroups":
                    Click_ArchiveEnableGroupBy();
                    break;
                case "archiveDefaultView":
                    Click_ArchiveViewDefault();
                    break;
                case "archiveBAFlatten":
                    if (_checkArchiveFlatMode.Checked)
                    {
                        SetArchiveGridViewMode(GridViewMode.Flat);
                    }
                    else
                    {
                        SetArchiveGridViewMode(GridViewMode.Hierarchical);
                    }

                    break;
                case "archiveEventProperties":
                    _archiveEventView.ShowEventDetails();
                    break;
                case "archiveProperties":
                    ShowArchiveProperties();
                    break;
                case "deleteServer":
                    _mainForm.RemoveServerAction((ServerRecord)_scope);
                    break;
                case "applyAuditTemplate":
                    _mainForm.ApplyAuditTemplate((DatabaseRecord)_scope, null);
                    break;
                case "applyServerAuditTemplate":
                    _mainForm.ApplyAuditTemplate(null, (ServerRecord)_scope);
                    break;
                case "fileNewDatabase":
                    _mainForm.AddDatabaseAction((ServerRecord)_scope);
                    break;
                case "auditingDisable":
                    _mainForm.DisableServerAction((ServerRecord)_scope);
                    break;
                case "editAuditSettings":
                    _mainForm.ShowServerProperties((ServerRecord)_scope, Form_ServerProperties.Context.AuditedActivities);
                    break;
                case "editPrivUsers":
                    _mainForm.ShowServerProperties((ServerRecord)_scope, Form_ServerProperties.Context.PrivilegedUser);
                    break;
                case "importAuditSettings":
                    _mainForm.ImportAuditSettingsAction();
                    break;
                case "exportAuditSettings":
                    _mainForm.ExportServerAuditSettingsAction((ServerRecord)_scope);
                    break;
                case "auditCollectNow":
                    _mainForm.CollectNowAction();
                    break;
                case "agentProperties":
                    _mainForm.ShowAgentProperties((ServerRecord)_scope);
                    break;
                case "auditUpdateSettings":
                    _mainForm.UpdateNow(((ServerRecord)_scope).Instance);
                    break;
                case "configureEventFilters":
                    _mainForm.NavigateToView(ConsoleViews.AdminEventFilters);
                    break;
                case "removeDatabase":
                    _mainForm.RemoveDatabaseAction((DatabaseRecord)_scope);
                    break;
                case "disableAuditing":
                    _mainForm.DisableDatabaseAction((DatabaseRecord)_scope);
                    break;
                case "auditDatabaseSettings":
                    _mainForm.ShowDatabaseProperties((DatabaseRecord)_scope, Form_DatabaseProperties.Context.AuditedActivities);
                    break;
                case "trustedUsers":
                    _mainForm.ShowDatabaseProperties((DatabaseRecord)_scope, Form_DatabaseProperties.Context.TrustedUsers);
                    break;
                case "importDatabaseSettings":
                    _mainForm.ImportAuditSettingsAction();
                    break;
                case "exportDatabaseSettings":
                    _mainForm.ExportDatabaseAuditSettingsAction((DatabaseRecord)_scope);
                    break;
                case "archiveAttach":
                    _mainForm.AttachArchiveAction();
                    break;
                case "archiveDetach":
                    if (_comboArchiveList.SelectedIndex >= 0)
                    {
                        _mainForm.DetachArchiveAction(GetSelectedArchive());
                    }
                    break;
            }
        }

        private void ToolValueChanged_toolbarManager(object sender, ToolEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "alertViewsSelector":
                    ValueChanged_AlertView();
                    break;
                case "alertFiltersTime":
                    ValueChanged_AlertTime();
                    break;
                case "alertFiltersLevel":
                    ValueChanged_AlertLevel();
                    break;
                case "dataAlertViewsSelector":
                    ValueChanged_DataAlertView();
                    break;
                case "dataAlertFiltersTime":
                    ValueChanged_DataAlertTime();
                    break;
                case "dataAlertFiltersLevel":
                    ValueChanged_DataAlertLevel();
                    break;
                case "statusAlertViewsSelector":
                    ValueChanged_StatusAlertView();
                    break;
                case "statusAlertFiltersTime":
                    ValueChanged_StatusAlertTime();
                    break;
                case "statusAlertFiltersLevel":
                    ValueChanged_StatusAlertLevel();
                    break;
                case "eventViewsSelector":
                    ValueChanged_EventView();
                    break;
                case "eventFiltersTime":
                    ValueChanged_EventTime();
                    break;
                case "eventFiltersCategory":
                    ValueChanged_EventCategory();
                    break;
                case "eventFiltersType":
                    ValueChanged_EventType();
                    break;
                case "eventFiltersLogin":
                    ValueChanged_EventLogin();
                    break;
                case "eventFiltersApplication":
                    ValueChanged_EventApplication();
                    break;
                case "eventFiltersHost":
                    ValueChanged_EventHost();
                    break;
                case "eventBATable":
                    ValueChanged_EventTable();
                    break;
                case "eventBAColumn":
                    ValueChanged_EventColumn();
                    break;
                case "archiveViewSelector":
                    ValueChanged_ArchiveView();
                    break;
                case "archiveTime":
                    ValueChanged_ArchiveTime();
                    break;
                case "archiveCategory":
                    ValueChanged_ArchiveCategory();
                    break;
                case "archiveType":
                    ValueChanged_ArchiveType();
                    break;
                case "archiveLogin":
                    ValueChanged_ArchiveLogin();
                    break;
                case "archiveApplication":
                    ValueChanged_ArchiveApplication();
                    break;
                case "archiveHost":
                    ValueChanged_ArchiveHost();
                    break;
                case "archiveBATable":
                    ValueChanged_ArchiveTable();
                    break;
                case "archiveBAColumn":
                    ValueChanged_ArchiveColumn();
                    break;
                case "archiveSelector":
                    ValueChanged_ArchiveList();
                    break;
            }
        }

        private void BeforeToolDropdown_toolbarManager(object sender, BeforeToolDropdownEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "contextAlerts":
                    UpdateAlertsContextMenu();
                    break;
                case "contextDataAlerts":
                    UpdateDataAlertsContextMenu();
                    break;
                case "contextEvents":
                    UpdateEventsContextMenu();
                    break;
                case "contextArchive":
                    UpdateArchiveContextMenu();
                    break;
            }
        }


        #region Alerts Ribbon

        /// <summary>
        /// The AlertMessage and EventDetails items should only be available
        /// when an alert is selected.  The Group-by options are handled directly
        /// by the ribbon (shared tools)
        /// </summary>
        private void UpdateAlertsContextMenu()
        {
            Alert alert = _alertView.GetActiveAlert();
            if (alert == null)
            {
                _cmAlertEventDetails.SharedProps.Enabled = false;
                _cmAlertMessage.SharedProps.Enabled = false;
            }
            else
            {
                _cmAlertEventDetails.SharedProps.Enabled = true;
                _cmAlertMessage.SharedProps.Enabled = !string.IsNullOrEmpty(alert.MessageBody);
            }
        }

        private void UpdateDataAlertsContextMenu()
        {
            DataAlert alert = _dataAlertView.GetActiveAlert();
            if (alert == null)
            {
                _cmDataAlertEventDetails.SharedProps.Enabled = false;
                _cmDataAlertMessage.SharedProps.Enabled = false;
            }
            else
            {
                _cmDataAlertEventDetails.SharedProps.Enabled = true;
                _cmDataAlertMessage.SharedProps.Enabled = !string.IsNullOrEmpty(alert.MessageBody);
            }
        }

        private void ExtractStatusAlertViewSettings(StatusAlertViewSettings settings)
        {
            if (settings == null)
            {
                return;
            }

            settings.Columns = GridColumnSettings.ExtractColumnSettings(_statusAlertView.Grid);
            settings.Filter = _statusAlertView.Filter.Clone();
            settings.ShowGroupBy = _checkStatusAlertGroupBy.Checked;
        }

        private void ExtractAlertViewSettings(AlertViewSettings settings)
        {
            if (settings == null)
            {
                return;
            }

            settings.Columns = GridColumnSettings.ExtractColumnSettings(_alertView.Grid);
            settings.Filter = _alertView.Filter.Clone();
            settings.ShowGroupBy = _checkAlertGroupBy.Checked;
        }

        private void ExtractDataAlertViewSettings(DataAlertViewSettings settings)
        {
            if (settings == null)
            {
                return;
            }

            settings.Columns = GridColumnSettings.ExtractColumnSettings(_dataAlertView.Grid);
            settings.Filter = _dataAlertView.Filter.Clone();
            settings.ShowGroupBy = _checkDataAlertGroupBy.Checked;
        }

        /// <summary>
        /// This function loads view from the user's settings.  If not views are present, it loads
        /// the default view.
        /// </summary>
        public void InitializeAlertViewCombo()
        {
            InitializeEventAlertsCombo();
            InitializeStatusAlertsCombo();
            InitializeDataAlertsCombo();
        }

        private void InitializeEventAlertsCombo()
        {
            List<AlertViewSettings> views = Settings.Default.AlertViews;
            if (views.Count == 0)
            {
                // At this point we pull views from the application configuration.  We must
                //  always have at least one view
                views.Add(Settings.Default.AppDefaultAlertView);
            }
            _internalUpdate++;
            _comboAlertViews.ValueList.ValueListItems.Clear();
            try
            {
                foreach (AlertViewSettings view in views)
                {
                    ValueListItem item = _comboAlertViews.ValueList.ValueListItems.Add(view, view.Name);
                    if (Settings.Default.DefaultAlertView == view.Name)
                    {
                        _comboAlertViews.SelectedItem = item;
                    }
                }
                if (_comboAlertViews.SelectedItem == null)
                {
                    _comboAlertViews.SelectedIndex = 0;
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose, "InitializeAlertViewCombo", e, ErrorLog.Severity.Warning);
            }
            _internalUpdate--;
        }

        private void InitializeDataAlertsCombo()
        {
            List<DataAlertViewSettings> views = Settings.Default.DataAlertViews;
            if (views.Count == 0)
            {
                // At this point we pull views from the application configuration.  We must
                //  always have at least one view
                views.Add(Settings.Default.AppDefaultDataAlertView);
            }
            _internalUpdate++;
            _comboDataAlertViews.ValueList.ValueListItems.Clear();
            try
            {
                foreach (DataAlertViewSettings view in views)
                {
                    ValueListItem item = _comboDataAlertViews.ValueList.ValueListItems.Add(view, view.Name);
                    if (Settings.Default.DefaultDataAlertView == view.Name)
                    {
                        _comboDataAlertViews.SelectedItem = item;
                    }
                }
                if (_comboDataAlertViews.SelectedItem == null)
                {
                    _comboDataAlertViews.SelectedIndex = 0;
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose, "InitializeDataAlertsCombo", e, ErrorLog.Severity.Warning);
            }
            _internalUpdate--;
        }

        private void InitializeStatusAlertsCombo()
        {
            List<StatusAlertViewSettings> views = Settings.Default.StatusAlertViews;
            if (views.Count == 0)
            {
                // At this point we pull views from the application configuration.  We must
                //  always have at least one view
                views.Add(Settings.Default.AppDefaultStatusAlertView);
            }
            _internalUpdate++;
            _comboStatusAlertViews.ValueList.ValueListItems.Clear();

            try
            {

                foreach (StatusAlertViewSettings view in views)
                {
                    ValueListItem item = _comboStatusAlertViews.ValueList.ValueListItems.Add(view, view.Name);
                    if (Settings.Default.DefaultStatusAlertView == view.Name)
                    {
                        _comboStatusAlertViews.SelectedItem = item;
                    }
                }
                if (_comboStatusAlertViews.SelectedItem == null)
                {
                    _comboStatusAlertViews.SelectedIndex = 0;
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose, "InitializeStatusAlertViewCombo", e, ErrorLog.Severity.Warning);
            }
            _internalUpdate--;
        }

        private void SetAlertViewDirty(bool value)
        {
            if (value == _alertViewDirty || _comboAlertViews.SelectedItem == null)
            {
                return;
            }

            _alertViewDirty = value;
            if (value)
            {
                ValueListItem oldItem = (ValueListItem)_comboAlertViews.SelectedItem;
                AlertViewSettings oldSelection = (AlertViewSettings)oldItem.DataValue;
                AlertViewSettings newSelection = oldSelection.Clone();
                newSelection.Name = "* " + newSelection.Name;
                ValueListItem newItem = _comboAlertViews.ValueList.ValueListItems.Add(newSelection, newSelection.Name);
                _internalUpdate++;
                _comboAlertViews.SelectedItem = newItem;
                _internalUpdate--;
            }
            else
            {
                if (_comboAlertViews.ValueList.ValueListItems.Count > 0)
                {
                    ValueListItem oldDirtyItem = _comboAlertViews.ValueList.ValueListItems[0];
                    AlertViewSettings oldDirty = (AlertViewSettings)oldDirtyItem.DataValue;
                    if (oldDirty.Name.StartsWith("*"))
                    {
                        _comboAlertViews.ValueList.ValueListItems.RemoveAt(0);
                    }
                }
            }
            _btnAlertSave.SharedProps.Enabled = value;
        }

        private void SetDataAlertViewDirty(bool value)
        {
            if (value == _dataAlertViewDirty || _comboDataAlertViews.SelectedItem == null)
            {
                return;
            }

            _dataAlertViewDirty = value;
            if (value)
            {
                ValueListItem oldItem = (ValueListItem)_comboDataAlertViews.SelectedItem;
                DataAlertViewSettings oldSelection = (DataAlertViewSettings)oldItem.DataValue;
                DataAlertViewSettings newSelection = oldSelection.Clone();
                newSelection.Name = "* " + newSelection.Name;
                ValueListItem newItem = _comboDataAlertViews.ValueList.ValueListItems.Add(newSelection, newSelection.Name);
                _internalUpdate++;
                _comboDataAlertViews.SelectedItem = newItem;
                _internalUpdate--;
            }
            else
            {
                if (_comboDataAlertViews.ValueList.ValueListItems.Count > 0)
                {
                    ValueListItem oldDirtyItem = _comboDataAlertViews.ValueList.ValueListItems[0];
                    DataAlertViewSettings oldDirty = (DataAlertViewSettings)oldDirtyItem.DataValue;
                    if (oldDirty.Name.StartsWith("*"))
                    {
                        _comboDataAlertViews.ValueList.ValueListItems.RemoveAt(0);
                    }
                }
            }
            _btnDataAlertSave.SharedProps.Enabled = value;
        }

        private void SetStatusAlertViewDirty(bool value)
        {
            if (value == _statusAlertViewDirty || _comboStatusAlertViews.SelectedItem == null)
            {
                return;
            }

            _statusAlertViewDirty = value;

            if (value)
            {
                ValueListItem oldItem = (ValueListItem)_comboStatusAlertViews.SelectedItem;
                StatusAlertViewSettings oldSelection = (StatusAlertViewSettings)oldItem.DataValue;
                StatusAlertViewSettings newSelection = oldSelection.Clone();
                newSelection.Name = "* " + newSelection.Name;
                ValueListItem newItem = _comboStatusAlertViews.ValueList.ValueListItems.Add(newSelection, newSelection.Name);
                _internalUpdate++;
                _comboStatusAlertViews.SelectedItem = newItem;
                _internalUpdate--;
            }
            else
            {
                if (_comboStatusAlertViews.ValueList.ValueListItems.Count > 0)
                {
                    ValueListItem oldDirtyItem = _comboStatusAlertViews.ValueList.ValueListItems[0];
                    StatusAlertViewSettings oldDirty = (StatusAlertViewSettings)oldDirtyItem.DataValue;
                    if (oldDirty.Name.StartsWith("*"))
                    {
                        _comboStatusAlertViews.ValueList.ValueListItems.RemoveAt(0);
                    }
                }
            }
            _btnStatusAlertSave.SharedProps.Enabled = value;
        }

        private void ApplyAlertView(AlertViewSettings settings)
        {
            _internalUpdate++;
            try
            {
                _alertView.Filter = settings.Filter.Clone();
                _checkAlertDefaultView.InitializeChecked(Settings.Default.DefaultAlertView == settings.Name);
                SetAlertGroupBy(settings.ShowGroupBy);
                GridColumnSettings.ApplyColumnSettings(_alertView.Grid, settings.Columns);
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose, "ApplyAlertView", e, ErrorLog.Severity.Warning);
            }
            _internalUpdate--;
            UpdateAlertFilterDisplay();
        }

        private void ApplyDataAlertView(DataAlertViewSettings settings)
        {
            _internalUpdate++;
            try
            {
                _dataAlertView.Filter = settings.Filter.Clone();
                _checkDataAlertDefaultView.InitializeChecked(Settings.Default.DefaultDataAlertView == settings.Name);
                SetDataAlertGroupBy(settings.ShowGroupBy);
                GridColumnSettings.ApplyColumnSettings(_dataAlertView.Grid, settings.Columns);
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose, "ApplyDataAlertView", e, ErrorLog.Severity.Warning);
            }
            _internalUpdate--;
            UpdateDataAlertFilterDisplay();
        }

        private void ApplyStatusAlertView(StatusAlertViewSettings settings)
        {
            _internalUpdate++;
            try
            {
                _statusAlertView.Filter = settings.Filter.Clone();
                _checkStatusAlertDefaultView.InitializeChecked(Settings.Default.DefaultStatusAlertView == settings.Name);
                SetStatusAlertGroupBy(settings.ShowGroupBy);
                GridColumnSettings.ApplyColumnSettings(_statusAlertView.Grid, settings.Columns);
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose, "ApplyStatusAlertView", e, ErrorLog.Severity.Warning);
            }
            _internalUpdate--;
            UpdateStatusAlertFilterDisplay();
        }

        private void SetAlertGroupBy(bool value)
        {
            _checkAlertGroupBy.Checked = value;
            _alertView.ShowGroupBy = value;
            _btnAlertCollapseAll.SharedProps.Enabled = value;
            _btnAlertExpandAll.SharedProps.Enabled = value;
            SetMenuFlag(CMMenuItem.Collapse, value);
            SetMenuFlag(CMMenuItem.Expand, value);
        }

        private void SetDataAlertGroupBy(bool value)
        {
            _checkDataAlertGroupBy.Checked = value;
            _dataAlertView.ShowGroupBy = value;
            _btnDataAlertCollapseAll.SharedProps.Enabled = value;
            _btnDataAlertExpandAll.SharedProps.Enabled = value;
            SetMenuFlag(CMMenuItem.Collapse, value);
            SetMenuFlag(CMMenuItem.Expand, value);
        }

        private void SetStatusAlertGroupBy(bool value)
        {
            _checkStatusAlertGroupBy.Checked = value;
            _statusAlertView.ShowGroupBy = value;
            _btnStatusAlertCollapseAll.SharedProps.Enabled = value;
            _btnStatusAlertExpandAll.SharedProps.Enabled = value;
            SetMenuFlag(CMMenuItem.Collapse, value);
            SetMenuFlag(CMMenuItem.Expand, value);
        }

        private void UpdateAlertFilterDisplay()
        {
            _internalUpdate++;

            try
            {
                if (_alertView.Filter.UseLevel)
                {
                    _comboAlertLevel.SelectedIndex = (int)_alertView.Filter.AlertLevel;
                }
                else
                {
                    _comboAlertLevel.SelectedIndex = 0;
                }

                switch (_alertView.Filter.SelectionType)
                {
                    case AlertSelectionType.SelectAll:
                        _comboAlertTime.SelectedIndex = 0;
                        break;
                    case AlertSelectionType.SelectPastDays:
                        if (_alertView.Filter.Days == 7)
                        {
                            _comboAlertTime.SelectedIndex = 2;
                        }
                        else if (_alertView.Filter.Days == 30)
                        {
                            _comboAlertTime.SelectedIndex = 3;
                        }
                        else
                        {
                            _comboAlertTime.SelectedIndex = 4;
                        }

                        break;
                    case AlertSelectionType.SelectRanged:
                        _alertsCustomTime.AddCustomRange(_comboAlertTime, _alertView.Filter.StartDate, _alertView.Filter.EndDate);
                        _comboAlertTime.SelectedIndex = 5;
                        break;
                    case AlertSelectionType.SelectToday:
                        _comboAlertTime.SelectedIndex = 1;
                        break;
                }
                _alertTimeFilterLastIndex = _comboAlertTime.SelectedIndex;
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write("UpdateFilterDisplay", e);
            }
            _internalUpdate--;
        }

        private void UpdateDataAlertFilterDisplay()
        {
            _internalUpdate++;

            try
            {
                if (_dataAlertView.Filter.UseLevel)
                {
                    _comboDataAlertLevel.SelectedIndex = (int)_dataAlertView.Filter.AlertLevel;
                }
                else
                {
                    _comboDataAlertLevel.SelectedIndex = 0;
                }

                switch (_dataAlertView.Filter.SelectionType)
                {
                    case AlertSelectionType.SelectAll:
                        _comboDataAlertTime.SelectedIndex = 0;
                        break;
                    case AlertSelectionType.SelectPastDays:
                        if (_dataAlertView.Filter.Days == 7)
                        {
                            _comboDataAlertTime.SelectedIndex = 2;
                        }
                        else if (_alertView.Filter.Days == 30)
                        {
                            _comboDataAlertTime.SelectedIndex = 3;
                        }
                        else
                        {
                            _comboDataAlertTime.SelectedIndex = 4;
                        }

                        break;
                    case AlertSelectionType.SelectRanged:
                        _dataAlertsCustomTime.AddCustomRange(_comboDataAlertTime, _dataAlertView.Filter.StartDate, _dataAlertView.Filter.EndDate);
                        _comboDataAlertTime.SelectedIndex = 5;
                        break;
                    case AlertSelectionType.SelectToday:
                        _comboDataAlertTime.SelectedIndex = 1;
                        break;
                }
                _dataAlertTimeFilterLastIndex = _comboDataAlertTime.SelectedIndex;
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write("UpdateFilterDisplay", e);
            }
            _internalUpdate--;
        }

        private void UpdateStatusAlertFilterDisplay()
        {
            _internalUpdate++;

            try
            {
                if (_statusAlertView.Filter.UseLevel)
                {
                    _comboStatusAlertLevel.SelectedIndex = (int)_statusAlertView.Filter.AlertLevel;
                }
                else
                {
                    _comboStatusAlertLevel.SelectedIndex = 0;
                }

                switch (_statusAlertView.Filter.SelectionType)
                {
                    case AlertSelectionType.SelectAll:
                        _comboStatusAlertTime.SelectedIndex = 0;
                        break;
                    case AlertSelectionType.SelectPastDays:
                        if (_statusAlertView.Filter.Days == 7)
                        {
                            _comboStatusAlertTime.SelectedIndex = 2;
                        }
                        else if (_statusAlertView.Filter.Days == 30)
                        {
                            _comboStatusAlertTime.SelectedIndex = 3;
                        }
                        else
                        {
                            _comboStatusAlertTime.SelectedIndex = 4;
                        }

                        break;
                    case AlertSelectionType.SelectRanged:
                        _statusAlertsCustomTime.AddCustomRange(_comboStatusAlertTime, _statusAlertView.Filter.StartDate, _statusAlertView.Filter.EndDate);
                        _comboStatusAlertTime.SelectedIndex = 5;
                        break;
                    case AlertSelectionType.SelectToday:
                        _comboStatusAlertTime.SelectedIndex = 1;
                        break;
                }
                _statusAlertTimeFilterLastIndex = _comboStatusAlertTime.SelectedIndex;
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write("UpdateFilterDisplay", e);
            }
            _internalUpdate--;
        }

        private void Click_AlertEnableGroupBy()
        {
            if (_internalUpdate > 0)
            {
                return;
            }

            SetAlertGroupBy(_checkAlertGroupBy.Checked);
            SetAlertViewDirty(true);
        }

        private void Click_DataAlertEnableGroupBy()
        {
            if (_internalUpdate > 0)
            {
                return;
            }

            SetDataAlertGroupBy(_checkDataAlertGroupBy.Checked);
            SetDataAlertViewDirty(true);
        }

        private void Click_StatusAlertEnableGroupBy()
        {
            if (_internalUpdate > 0)
            {
                return;
            }

            SetStatusAlertGroupBy(_checkStatusAlertGroupBy.Checked);
            SetStatusAlertViewDirty(true);
        }

        private void Click_AlertDefaultView()
        {
            if (_internalUpdate > 0)
            {
                return;
            }

            ValueListItem item = (ValueListItem)_comboAlertViews.SelectedItem;
            AlertViewSettings settings = (AlertViewSettings)item.DataValue;
            if (_checkAlertDefaultView.Checked)
            {
                Settings.Default.DefaultAlertView = settings.Name;
            }
            else
            {
                Settings.Default.DefaultAlertView = "";
            }
        }

        private void Click_DataAlertDefaultView()
        {
            if (_internalUpdate > 0)
            {
                return;
            }

            ValueListItem item = (ValueListItem)_comboDataAlertViews.SelectedItem;
            DataAlertViewSettings settings = (DataAlertViewSettings)item.DataValue;
            if (_checkDataAlertDefaultView.Checked)
            {
                Settings.Default.DefaultDataAlertView = settings.Name;
            }
            else
            {
                Settings.Default.DefaultDataAlertView = "";
            }
        }

        private void Click_StatusAlertDefaultView()
        {
            if (_internalUpdate > 0)
            {
                return;
            }

            ValueListItem item = (ValueListItem)_comboStatusAlertViews.SelectedItem;
            StatusAlertViewSettings settings = (StatusAlertViewSettings)item.DataValue;
            if (_checkStatusAlertDefaultView.Checked)
            {
                Settings.Default.DefaultStatusAlertView = settings.Name;
            }
            else
            {
                Settings.Default.DefaultStatusAlertView = "";
            }
        }

        private void Click_AlertViewsSave()
        {
            AlertViewSettings settings, originalSettings = null;
            ValueListItem originalItem = null;
            ValueListItem item = (ValueListItem)_comboAlertViews.SelectedItem;

            settings = (AlertViewSettings)item.DataValue;
            foreach (ValueListItem subItem in _comboAlertViews.ValueList.ValueListItems)
            {
                AlertViewSettings x = (AlertViewSettings)subItem.DataValue;
                if (settings != x &&
                   settings.Name.Substring(2) == x.Name)
                {
                    originalItem = subItem;
                    originalSettings = x;
                    break;
                }
            }
            if (originalSettings != null)
            {
                ExtractAlertViewSettings(originalSettings);
                _comboAlertViews.SelectedItem = originalItem;
            }
            SetAlertViewDirty(false);
        }

        private void Click_DataAlertViewsSave()
        {
            DataAlertViewSettings settings, originalSettings = null;
            ValueListItem originalItem = null;
            ValueListItem item = (ValueListItem)_comboDataAlertViews.SelectedItem;

            settings = (DataAlertViewSettings)item.DataValue;
            foreach (ValueListItem subItem in _comboDataAlertViews.ValueList.ValueListItems)
            {
                DataAlertViewSettings x = (DataAlertViewSettings)subItem.DataValue;
                if (settings != x &&
                   settings.Name.Substring(2) == x.Name)
                {
                    originalItem = subItem;
                    originalSettings = x;
                    break;
                }
            }
            if (originalSettings != null)
            {
                ExtractDataAlertViewSettings(originalSettings);
                _comboDataAlertViews.SelectedItem = originalItem;
            }
            SetDataAlertViewDirty(false);
        }

        private void Click_StatusAlertViewsSave()
        {
            StatusAlertViewSettings settings, originalSettings = null;
            ValueListItem originalItem = null;
            ValueListItem item = (ValueListItem)_comboStatusAlertViews.SelectedItem;

            settings = (StatusAlertViewSettings)item.DataValue;
            foreach (ValueListItem subItem in _comboStatusAlertViews.ValueList.ValueListItems)
            {
                StatusAlertViewSettings x = (StatusAlertViewSettings)subItem.DataValue;
                if (settings != x &&
                   settings.Name.Substring(2) == x.Name)
                {
                    originalItem = subItem;
                    originalSettings = x;
                    break;
                }
            }
            if (originalSettings != null)
            {
                ExtractStatusAlertViewSettings(originalSettings);
                _comboStatusAlertViews.SelectedItem = originalItem;
            }
            SetStatusAlertViewDirty(false);
        }

        private void Click_AlertViewsSaveAs()
        {
            string[] views = new string[Settings.Default.AlertViews.Count];

            for (int i = 0; i < views.Length; i++)
            {
                views[i] = Settings.Default.AlertViews[i].Name;
            }
            Form_ViewName frm = new Form_ViewName(views);
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                AlertViewSettings settings = new AlertViewSettings
                {
                    Name = frm.ViewName
                };
                ExtractAlertViewSettings(settings);
                Settings.Default.AlertViews.Add(settings);
                ValueListItem item = _comboAlertViews.ValueList.ValueListItems.Add(settings, settings.Name);
                _comboAlertViews.SelectedItem = item;
                SetAlertViewDirty(false);
            }
        }

        private void Click_DataAlertViewsSaveAs()
        {
            string[] views = new string[Settings.Default.DataAlertViews.Count];

            for (int i = 0; i < views.Length; i++)
            {
                views[i] = Settings.Default.DataAlertViews[i].Name;
            }
            Form_ViewName frm = new Form_ViewName(views);
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                DataAlertViewSettings settings = new DataAlertViewSettings
                {
                    Name = frm.ViewName
                };
                ExtractDataAlertViewSettings(settings);
                Settings.Default.DataAlertViews.Add(settings);
                ValueListItem item = _comboDataAlertViews.ValueList.ValueListItems.Add(settings, settings.Name);
                _comboDataAlertViews.SelectedItem = item;
                SetDataAlertViewDirty(false);
            }
        }

        private void Click_StatusAlertViewsSaveAs()
        {
            string[] views = new string[Settings.Default.StatusAlertViews.Count];

            for (int i = 0; i < views.Length; i++)
            {
                views[i] = Settings.Default.StatusAlertViews[i].Name;
            }
            Form_ViewName frm = new Form_ViewName(views);
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                StatusAlertViewSettings settings = new StatusAlertViewSettings
                {
                    Name = frm.ViewName
                };
                ExtractStatusAlertViewSettings(settings);
                Settings.Default.StatusAlertViews.Add(settings);
                ValueListItem item = _comboStatusAlertViews.ValueList.ValueListItems.Add(settings, settings.Name);
                _comboStatusAlertViews.SelectedItem = item;
                SetStatusAlertViewDirty(false);
            }
        }

        private void Click_AlertViewsDelete()
        {
            ValueListItem item = (ValueListItem)_comboAlertViews.SelectedItem;
            AlertViewSettings settings = (AlertViewSettings)item.DataValue;

            if (MessageBox.Show(this, string.Format("Do you wish to delete the selected view:  {0}?",
               settings.Name), "Confirm Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                for (int i = 0; i < Settings.Default.AlertViews.Count; i++)
                {
                    // Remove it from the Settings object
                    if (Settings.Default.AlertViews[i].Name == settings.Name)
                    {
                        Settings.Default.AlertViews.RemoveAt(i);
                        break;
                    }
                }
                _comboAlertViews.ValueList.ValueListItems.RemoveAt(_comboAlertViews.SelectedIndex);

                // Force refresh also - InitializeViewCombo does not refresh view
                InitializeAlertViewCombo();
                item = (ValueListItem)_comboAlertViews.SelectedItem;
                settings = (AlertViewSettings)item.DataValue;
                ApplyAlertView(settings);
                RefreshView();
            }
        }

        private void Click_DataAlertViewsDelete()
        {
            ValueListItem item = (ValueListItem)_comboDataAlertViews.SelectedItem;
            DataAlertViewSettings settings = (DataAlertViewSettings)item.DataValue;

            if (MessageBox.Show(this, string.Format("Do you wish to delete the selected view:  {0}?",
               settings.Name), "Confirm Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                for (int i = 0; i < Settings.Default.DataAlertViews.Count; i++)
                {
                    // Remove it from the Settings object
                    if (Settings.Default.DataAlertViews[i].Name == settings.Name)
                    {
                        Settings.Default.DataAlertViews.RemoveAt(i);
                        break;
                    }
                }
                _comboDataAlertViews.ValueList.ValueListItems.RemoveAt(_comboDataAlertViews.SelectedIndex);

                // Force refresh also - InitializeViewCombo does not refresh view
                InitializeAlertViewCombo();
                item = (ValueListItem)_comboDataAlertViews.SelectedItem;
                settings = (DataAlertViewSettings)item.DataValue;
                ApplyDataAlertView(settings);
                RefreshView();
            }
        }

        private void Click_StatusAlertViewsDelete()
        {
            ValueListItem item = (ValueListItem)_comboStatusAlertViews.SelectedItem;
            StatusAlertViewSettings settings = (StatusAlertViewSettings)item.DataValue;

            if (MessageBox.Show(this, string.Format("Do you wish to delete the selected view:  {0}?",
               settings.Name), "Confirm Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                for (int i = 0; i < Settings.Default.StatusAlertViews.Count; i++)
                {
                    // Remove it from the Settings object
                    if (Settings.Default.StatusAlertViews[i].Name == settings.Name)
                    {
                        Settings.Default.StatusAlertViews.RemoveAt(i);
                        break;
                    }
                }
                _comboStatusAlertViews.ValueList.ValueListItems.RemoveAt(_comboStatusAlertViews.SelectedIndex);

                // Force refresh also - InitializeViewCombo does not refresh view
                InitializeAlertViewCombo();
                item = (ValueListItem)_comboStatusAlertViews.SelectedItem;
                settings = (StatusAlertViewSettings)item.DataValue;
                ApplyStatusAlertView(settings);
                RefreshView();
            }
        }

        private void ValueChanged_AlertLevel()
        {
            if (_internalUpdate > 0)
            {
                return;
            }

            if (_comboAlertLevel.SelectedIndex == 0)
            {
                _alertView.Filter.UseLevel = false;
            }
            else
            {
                _alertView.Filter.UseLevel = true;
                _alertView.Filter.AlertLevel = (AlertLevel)_comboAlertLevel.SelectedIndex;
            }
            SetAlertViewDirty(true);
            RefreshView();
        }

        private void ValueChanged_DataAlertLevel()
        {
            if (_internalUpdate > 0)
            {
                return;
            }

            if (_comboDataAlertLevel.SelectedIndex == 0)
            {
                _dataAlertView.Filter.UseLevel = false;
            }
            else
            {
                _dataAlertView.Filter.UseLevel = true;
                _dataAlertView.Filter.AlertLevel = (AlertLevel)_comboDataAlertLevel.SelectedIndex;
            }
            SetDataAlertViewDirty(true);
            RefreshView();
        }


        private void ValueChanged_StatusAlertLevel()
        {
            if (_internalUpdate > 0)
            {
                return;
            }

            if (_comboStatusAlertLevel.SelectedIndex == 0)
            {
                _statusAlertView.Filter.UseLevel = false;
            }
            else
            {
                _statusAlertView.Filter.UseLevel = true;
                _statusAlertView.Filter.AlertLevel = (AlertLevel)_comboStatusAlertLevel.SelectedIndex;
            }
            SetStatusAlertViewDirty(true);
            RefreshView();
        }

        private void ValueChanged_AlertTime()
        {
            if (_internalUpdate > 0)
            {
                return;
            }

            AlertViewFilter filter = _alertView.Filter;

            switch (_comboAlertTime.SelectedIndex)
            {
                case 0:
                    filter.SelectionType = AlertSelectionType.SelectAll;
                    break;
                case 1:
                    filter.SelectionType = AlertSelectionType.SelectToday;
                    break;
                case 2:
                    filter.SelectionType = AlertSelectionType.SelectPastDays;
                    filter.Days = 7;
                    break;
                case 3:
                    filter.SelectionType = AlertSelectionType.SelectPastDays;
                    filter.Days = 30;
                    break;
                case 4:
                    _alertsCustomTime.GetCustomTime(_comboAlertTime, _alertTimeFilterLastIndex);
                    break;
                default:
                    {
                        // custom
                        int index = _comboAlertTime.SelectedIndex - 5;
                        filter.SelectionType = AlertSelectionType.SelectRanged;
                        filter.StartDate = _alertsCustomTime.StartTime[index].Value;
                        filter.EndDate = _alertsCustomTime.EndTime[index].Value;
                    }
                    break;
            }
            _alertTimeFilterLastIndex = _comboAlertTime.SelectedIndex;
            SetAlertViewDirty(true);
            RefreshView();
        }

        private void ValueChanged_DataAlertTime()
        {
            if (_internalUpdate > 0)
            {
                return;
            }

            AlertViewFilter filter = _dataAlertView.Filter;

            switch (_comboDataAlertTime.SelectedIndex)
            {
                case 0:
                    filter.SelectionType = AlertSelectionType.SelectAll;
                    break;
                case 1:
                    filter.SelectionType = AlertSelectionType.SelectToday;
                    break;
                case 2:
                    filter.SelectionType = AlertSelectionType.SelectPastDays;
                    filter.Days = 7;
                    break;
                case 3:
                    filter.SelectionType = AlertSelectionType.SelectPastDays;
                    filter.Days = 30;
                    break;
                case 4:
                    _dataAlertsCustomTime.GetCustomTime(_comboDataAlertTime, _dataAlertTimeFilterLastIndex);
                    break;
                default:
                    {
                        // custom
                        int index = _comboDataAlertTime.SelectedIndex - 5;
                        filter.SelectionType = AlertSelectionType.SelectRanged;
                        filter.StartDate = _dataAlertsCustomTime.StartTime[index].Value;
                        filter.EndDate = _dataAlertsCustomTime.EndTime[index].Value;
                    }
                    break;
            }
            _dataAlertTimeFilterLastIndex = _comboDataAlertTime.SelectedIndex;
            SetDataAlertViewDirty(true);
            RefreshView();
        }

        private void ValueChanged_StatusAlertTime()
        {
            if (_internalUpdate > 0)
            {
                return;
            }

            AlertViewFilter filter = _statusAlertView.Filter;

            switch (_comboStatusAlertTime.SelectedIndex)
            {
                case 0:
                    filter.SelectionType = AlertSelectionType.SelectAll;
                    break;
                case 1:
                    filter.SelectionType = AlertSelectionType.SelectToday;
                    break;
                case 2:
                    filter.SelectionType = AlertSelectionType.SelectPastDays;
                    filter.Days = 7;
                    break;
                case 3:
                    filter.SelectionType = AlertSelectionType.SelectPastDays;
                    filter.Days = 30;
                    break;
                case 4:
                    _statusAlertsCustomTime.GetCustomTime(_comboStatusAlertTime, _statusAlertTimeFilterLastIndex);
                    break;
                default:
                    {
                        // custom
                        int index = _comboStatusAlertTime.SelectedIndex - 5;
                        filter.SelectionType = AlertSelectionType.SelectRanged;
                        filter.StartDate = _statusAlertsCustomTime.StartTime[index].Value;
                        filter.EndDate = _statusAlertsCustomTime.EndTime[index].Value;
                    }
                    break;
            }
            _statusAlertTimeFilterLastIndex = _comboStatusAlertTime.SelectedIndex;
            SetStatusAlertViewDirty(true);
            RefreshView();
        }

        private void ValueChanged_AlertView()
        {
            if (_internalUpdate > 0)
            {
                return;
            }

            ValueListItem item = (ValueListItem)_comboAlertViews.SelectedItem;
            if (item == null)
            {
                if (_comboAlertViews.ValueList.ValueListItems.Count > 0)
                {
                    _comboAlertViews.SelectedIndex = 0;
                }

                return;
            }
            // Ignore edits
            if (_comboAlertViews.Text.StartsWith("*"))
            {
                return;
            }

            SetAlertViewDirty(false);
            AlertViewSettings settings = (AlertViewSettings)item.DataValue;
            ApplyAlertView(settings);
            RefreshView();
        }

        private void ValueChanged_DataAlertView()
        {
            if (_internalUpdate > 0)
            {
                return;
            }

            ValueListItem item = (ValueListItem)_comboDataAlertViews.SelectedItem;
            if (item == null)
            {
                if (_comboDataAlertViews.ValueList.ValueListItems.Count > 0)
                {
                    _comboDataAlertViews.SelectedIndex = 0;
                }

                return;
            }
            // Ignore edits
            if (_comboDataAlertViews.Text.StartsWith("*"))
            {
                return;
            }

            SetDataAlertViewDirty(false);
            DataAlertViewSettings settings = (DataAlertViewSettings)item.DataValue;
            ApplyDataAlertView(settings);
            RefreshView();
        }

        private void ValueChanged_StatusAlertView()
        {
            if (_internalUpdate > 0)
            {
                return;
            }

            ValueListItem item = (ValueListItem)_comboStatusAlertViews.SelectedItem;
            if (item == null)
            {
                if (_comboStatusAlertViews.ValueList.ValueListItems.Count > 0)
                {
                    _comboStatusAlertViews.SelectedIndex = 0;
                }

                return;
            }
            // Ignore edits
            if (_comboStatusAlertViews.Text.StartsWith("*"))
            {
                return;
            }

            SetStatusAlertViewDirty(false);
            StatusAlertViewSettings settings = (StatusAlertViewSettings)item.DataValue;
            ApplyStatusAlertView(settings);
            RefreshView();
        }

        private void AfterColPosChanged_AlertGrid(object sender, AfterColPosChangedEventArgs e)
        {
            if (_internalUpdate > 0)
            {
                return;
            }

            SetAlertViewDirty(true);
        }

        private void AfterSortChange_AlertGrid(object sender, BandEventArgs e)
        {
            if (_internalUpdate > 0)
            {
                return;
            }

            SetAlertViewDirty(true);
        }

        private void AfterColPosChanged_DataAlertGrid(object sender, AfterColPosChangedEventArgs e)
        {
            if (_internalUpdate > 0)
            {
                return;
            }

            SetDataAlertViewDirty(true);
        }

        private void AfterSortChange_DataAlertGrid(object sender, BandEventArgs e)
        {
            if (_internalUpdate > 0)
            {
                return;
            }

            SetDataAlertViewDirty(true);
        }

        private void AfterColPosChanged_StatusAlertGrid(object sender, AfterColPosChangedEventArgs e)
        {
            if (_internalUpdate > 0)
            {
                return;
            }

            SetStatusAlertViewDirty(true);
        }

        private void AfterSortChange_StatusAlertGrid(object sender, BandEventArgs e)
        {
            if (_internalUpdate > 0)
            {
                return;
            }

            SetStatusAlertViewDirty(true);
        }

        #endregion Alert Ribbon

        #region Event Ribbon

        private void UpdateEventsContextMenu()
        {
            _cmEventDetails.SharedProps.Enabled = _eventView.IsEventSelected();
        }

        /// <summary>
        /// This function loads view from the user's settings.  If not views are present, it loads
        /// the default view.
        /// </summary>
        public void InitializeEventViewCombo()
        {
            List<EventViewSettings> views = Settings.Default.EventViews;
            if (views.Count == 0)
            {
                // At this point we pull views from the application configuration.  We must
                //  always have at least one view
                views.Add(Settings.Default.AppDefaultEventView);
            }
            _internalUpdate++;
            _comboEventViews.ValueList.ValueListItems.Clear();
            _comboArchiveViews.ValueList.ValueListItems.Clear();
            try
            {
                foreach (EventViewSettings view in views)
                {
                    ValueListItem item = _comboEventViews.ValueList.ValueListItems.Add(view, view.Name);
                    ValueListItem archiveItem = _comboArchiveViews.ValueList.ValueListItems.Add(view, view.Name);
                    if (Settings.Default.DefaultEventView == view.Name)
                    {
                        _comboEventViews.SelectedItem = item;
                    }

                    if (Settings.Default.DefaultArchiveView == view.Name)
                    {
                        _comboArchiveViews.SelectedItem = archiveItem;
                    }
                }
                if (_comboEventViews.SelectedItem == null)
                {
                    _comboEventViews.SelectedIndex = 0;
                }

                if (_comboArchiveViews.SelectedItem == null)
                {
                    _comboArchiveViews.SelectedIndex = 0;
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose, "InitializeEventViewCombo", e, ErrorLog.Severity.Warning);
            }
            _internalUpdate--;
        }

        private void AfterColPosChanged_EventGrid(object sender, AfterColPosChangedEventArgs e)
        {
            if (_internalUpdate > 0)
            {
                return;
            }

            SetEventViewDirty(true);
        }

        private void AfterSortChange_EventGrid(object sender, BandEventArgs e)
        {
            if (_internalUpdate > 0)
            {
                return;
            }

            SetEventViewDirty(true);
        }

        private void SetEventGroupBy(bool value)
        {
            _checkEventGroupBy.Checked = value;
            _eventView.ShowGroupBy = value;
            _btnEventCollapseAll.SharedProps.Enabled = value;
            _btnEventExpandAll.SharedProps.Enabled = value;
            SetMenuFlag(CMMenuItem.Collapse, value);
            SetMenuFlag(CMMenuItem.Expand, value);
            //RelocateStatusLabel();
        }

        private void SetEventGridViewMode(GridViewMode value)
        {
            if (value == GridViewMode.Flat)
            {
                _checkEventFlatMode.InitializeChecked(true);
            }
            else
            {
                _checkEventFlatMode.InitializeChecked(false);
            }
            _eventView.ViewMode = value;
            // Dirty views modified by the user
            if (_internalUpdate == 0)
            {
                SetEventViewDirty(true);
            }
        }

        private void SetEventViewDirty(bool value)
        {
            if (value == _eventViewDirty || _comboEventViews.SelectedItem == null)
            {
                return;
            }

            _eventViewDirty = value;
            if (value)
            {
                ValueListItem oldItem, newItem;
                oldItem = (ValueListItem)_comboEventViews.SelectedItem;
                EventViewSettings oldSelection = (EventViewSettings)oldItem.DataValue;
                EventViewSettings newSelection = oldSelection.Clone();
                newSelection.Name = "* " + newSelection.Name;
                newItem = _comboEventViews.ValueList.ValueListItems.Add(newSelection, newSelection.Name);
                _internalUpdate++;
                _comboEventViews.SelectedItem = newItem;
                _internalUpdate--;
            }
            else
            {
                if (_comboEventViews.ValueList.ValueListItems.Count > 0)
                {
                    ValueListItem oldDirtyItem = _comboEventViews.ValueList.ValueListItems[0];
                    EventViewSettings oldDirty = (EventViewSettings)oldDirtyItem.DataValue;
                    if (oldDirty.Name.StartsWith("*"))
                    {
                        _comboEventViews.ValueList.ValueListItems.RemoveAt(0);
                    }
                }
            }
            _btnEventSave.SharedProps.Enabled = value;
        }

        private void Click_EventViewsSaveAs()
        {
            string[] views = new string[Settings.Default.EventViews.Count];

            for (int i = 0; i < views.Length; i++)
            {
                views[i] = Settings.Default.EventViews[i].Name;
            }
            Form_ViewName frm = new Form_ViewName(views);
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                EventViewSettings settings = new EventViewSettings
                {
                    Name = frm.ViewName
                };
                ExtractEventViewSettings(settings);
                Settings.Default.EventViews.Add(settings);
                ValueListItem item = _comboEventViews.ValueList.ValueListItems.Add(settings);
                _comboEventViews.SelectedItem = item;
                SetEventViewDirty(false);
            }
        }

        private void Click_EventViewsSave()
        {
            EventViewSettings settings, originalSettings = null;
            ValueListItem originalItem = null;
            ValueListItem item = (ValueListItem)_comboEventViews.SelectedItem;

            settings = (EventViewSettings)item.DataValue;
            foreach (ValueListItem subItem in _comboEventViews.ValueList.ValueListItems)
            {
                EventViewSettings x = (EventViewSettings)subItem.DataValue;
                if (settings != x &&
                   settings.Name.Substring(2) == x.Name)
                {
                    originalItem = subItem;
                    originalSettings = x;
                    break;
                }
            }
            if (originalSettings != null)
            {
                ExtractEventViewSettings(originalSettings);
                _comboEventViews.SelectedItem = originalItem;
            }
            SetEventViewDirty(false);
        }

        private void Click_EventEnableGroupBy()
        {
            if (_internalUpdate > 0)
            {
                return;
            }

            SetEventGroupBy(_checkEventGroupBy.Checked);
            SetEventViewDirty(true);
        }

        private void ExtractEventViewSettings(EventViewSettings settings)
        {
            if (settings == null)
            {
                return;
            }

            settings.Columns = GridColumnSettings.ExtractColumnSettings(_eventView.ActiveGrid);
            settings.Filter = _eventView.Filter.Clone();
            settings.ShowGroupBy = _checkEventGroupBy.Checked;
            settings.ViewMode = _eventView.ViewMode;
        }

        private void ApplyEventView(EventViewSettings settings)
        {
            _internalUpdate++;
            try
            {
                _eventView.Filter = settings.Filter.Clone();
                _checkEventDefaultView.InitializeChecked(Settings.Default.DefaultEventView == settings.Name);
                SetEventGroupBy(settings.ShowGroupBy);
                GridColumnSettings.ApplyColumnSettings(_eventView.GetGrid(settings.ViewMode), settings.Columns);
                SetEventGridViewMode(settings.ViewMode);
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose, "ApplyEventView", e, ErrorLog.Severity.Warning);
            }
            _internalUpdate--;
            if (_scope != null)
            {
                UpdateEventFilterDisplay();
            }
        }

        private void UpdateEventFilterDisplay()
        {
            _internalUpdate++;

            bool matchFound;

            try
            {
                EventViewFilter filter = _eventView.Filter;

                // Date Filter
                switch (filter.DateLimitType)
                {
                    case DateFilterType.Unlimited:
                        _comboEventTime.SelectedIndex = 0;
                        break;
                    case DateFilterType.NumberDays:
                        if (filter.Days == 7)
                        {
                            _comboEventTime.SelectedIndex = 2;
                        }
                        else if (filter.Days == 30)
                        {
                            _comboEventTime.SelectedIndex = 3;
                        }
                        else
                        {
                            _comboEventTime.SelectedIndex = 4;
                        }

                        break;
                    case DateFilterType.DateRange:
                        _eventsCustomTime.AddCustomRange(_comboEventTime, filter.StartDate, filter.EndDate);
                        _comboEventTime.SelectedIndex = 5;
                        break;
                    case DateFilterType.Today:
                        _comboEventTime.SelectedIndex = 1;
                        break;
                }
                _eventTimeFilterLastIndex = _comboEventTime.SelectedIndex;

                // Category and Type Filters
                if (filter.EventCategoryId == -1)
                {
                    _comboEventCategory.SelectedIndex = 0;
                    LoadTypeCombo(-1);
                }
                else
                {
                    CMEventCategory targetCategory = Globals.Repository.LookupEventCategory(filter.EventCategoryId);
                    _comboEventCategory.SelectedItem = _categories[targetCategory.Name];
                    LoadTypeCombo(filter.EventTypeId);
                }

                // Table filter
                _comboEventTable.SelectedIndex = 0;
                if (filter.TableId != null)
                {
                    matchFound = false;
                    for (int i = 0; i < _comboEventTable.ValueList.ValueListItems.Count; i++)
                    {
                        if (_comboEventTable.ValueList.ValueListItems[i].DataValue.Equals(filter.TableId))
                        {
                            _comboEventTable.SelectedIndex = i;
                            matchFound = true;
                            break;
                        }
                    }
                    if (!matchFound)
                    {
                        SetComboNotAvailable(_comboEventTable, 2, filter.TableId.GetValueOrDefault(-1));
                    }
                }

                // Column filter
                _comboEventColumn.SelectedIndex = 0;
                if (filter.ColumnId != null)
                {
                    matchFound = false;
                    for (int i = 0; i < _comboEventColumn.ValueList.ValueListItems.Count; i++)
                    {
                        if (_comboEventColumn.ValueList.ValueListItems[i].DataValue.Equals(filter.ColumnId))
                        {
                            _comboEventColumn.SelectedIndex = i;
                            matchFound = true;
                            break;
                        }
                    }
                    if (!matchFound)
                    {
                        SetComboNotAvailable(_comboEventColumn, 2, filter.ColumnId.GetValueOrDefault(-1));
                    }
                }

                // Application filter
                _comboEventApplication.SelectedIndex = 0;
                if (filter.ApplicationId != null)
                {
                    matchFound = false;
                    for (int i = 0; i < _comboEventApplication.ValueList.ValueListItems.Count; i++)
                    {
                        if (_comboEventApplication.ValueList.ValueListItems[i].DataValue.Equals(filter.ApplicationId))
                        {
                            _comboEventApplication.SelectedIndex = i;
                            matchFound = true;
                            break;
                        }
                    }
                    if (!matchFound)
                    {
                        SetComboNotAvailable(_comboEventApplication, 2, filter.ApplicationId.GetValueOrDefault(-1));
                    }
                }

                // Host Filter
                _comboEventHost.SelectedIndex = 0;
                if (filter.HostId != null)
                {
                    matchFound = false;
                    for (int i = 0; i < _comboEventHost.ValueList.ValueListItems.Count; i++)
                    {
                        if (_comboEventHost.ValueList.ValueListItems[i].DataValue.Equals(filter.HostId))
                        {
                            _comboEventHost.SelectedIndex = i;
                            matchFound = true;
                            break;
                        }
                    }
                    if (!matchFound)
                    {
                        SetComboNotAvailable(_comboEventHost, 2, filter.HostId.GetValueOrDefault(-1));
                    }
                }

                if (filter.ShowPrivUsersOnly)
                {
                    _comboEventLogin.SelectedIndex = 2;
                }
                else
                {
                    _comboEventLogin.SelectedIndex = 0;
                    if (filter.LoginId != null)
                    {
                        matchFound = false;
                        for (int i = 0; i < _comboEventLogin.ValueList.ValueListItems.Count; i++)
                        {
                            if (_comboEventLogin.ValueList.ValueListItems[i].DataValue.Equals(filter.LoginId))
                            {
                                _comboEventLogin.SelectedIndex = i;
                                matchFound = true;
                                break;
                            }
                        }
                        if (!matchFound)
                        {
                            SetComboNotAvailable(_comboEventLogin, 3, filter.LoginId.GetValueOrDefault(-1));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write("UpdateFilterDisplay", e);
            }
            _internalUpdate--;
        }

        private void Click_EventViewsDelete()
        {
            ValueListItem item = (ValueListItem)_comboEventViews.SelectedItem;
            EventViewSettings settings = (EventViewSettings)item.DataValue;

            if (MessageBox.Show(this, string.Format("Do you wish to delete the selected view:  {0}?",
               settings.Name), "Confirm Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                for (int i = 0; i < Settings.Default.EventViews.Count; i++)
                {
                    // Remove it from the Settings object
                    if (Settings.Default.EventViews[i].Name == settings.Name)
                    {
                        Settings.Default.EventViews.RemoveAt(i);
                        break;
                    }
                }
                _comboEventViews.ValueList.ValueListItems.RemoveAt(_comboEventViews.SelectedIndex);

                // Force refresh also - InitializeViewCombo does not refresh view
                InitializeEventViewCombo();
                item = (ValueListItem)_comboEventViews.SelectedItem;
                settings = (EventViewSettings)item.DataValue;
                ApplyEventView(settings);
                RefreshView();
            }
        }

        private void ValueChanged_EventView()
        {
            if (_internalUpdate > 0)
            {
                return;
            }

            if (_comboEventViews.SelectedItem == null)
            {
                if (_comboEventViews.ValueList.ValueListItems.Count > 0)
                {
                    _comboEventViews.SelectedIndex = 0;
                }

                return;
            }
            // Ignore edits
            if (_comboEventViews.Text.StartsWith("*"))
            {
                return;
            }

            SetEventViewDirty(false);
            ValueListItem item = (ValueListItem)_comboEventViews.SelectedItem;
            EventViewSettings settings = (EventViewSettings)item.DataValue;
            ApplyEventView(settings);
            RefreshView();
        }

        private void ValueChanged_EventTime()
        {
            if (_internalUpdate > 0)
            {
                return;
            }

            EventViewFilter filter = _eventView.Filter;

            switch (_comboEventTime.SelectedIndex)
            {
                case 0:
                    filter.DateLimitType = DateFilterType.Unlimited;
                    break;
                case 1:
                    filter.DateLimitType = DateFilterType.Today;
                    break;
                case 2:
                    filter.DateLimitType = DateFilterType.NumberDays;
                    filter.Days = 7;
                    break;
                case 3:
                    filter.DateLimitType = DateFilterType.NumberDays;
                    filter.Days = 30;
                    break;
                case 4:
                    _eventsCustomTime.GetCustomTime(_comboEventTime, _eventTimeFilterLastIndex);
                    break;
                default:
                    {
                        // custom
                        int index = _comboEventTime.SelectedIndex - 5;
                        filter.DateLimitType = DateFilterType.DateRange;
                        filter.StartDate = _eventsCustomTime.StartTime[index].Value;
                        filter.EndDate = _eventsCustomTime.EndTime[index].Value;
                    }
                    break;
            }
            _eventTimeFilterLastIndex = _comboEventTime.SelectedIndex;
            SetEventViewDirty(true);
            RefreshView();
        }

        private void ValueChanged_EventCategory()
        {
            if (_internalUpdate > 0)
            {
                return;
            }

            int index = _comboEventCategory.SelectedIndex;
            if (index >= 1)
            {
                ValueListItem item = (ValueListItem)_comboEventCategory.SelectedItem;
                CMEventCategory eventCategory = (CMEventCategory)item.DataValue;
                _eventView.Filter.EventCategoryId = eventCategory.CategoryId;
            }
            else
            {
                _eventView.Filter.EventCategoryId = -1;
            }
            // Always reset event id when category changes
            _eventView.Filter.EventTypeId = -1;
            EnableGroupTools(_groupEventBeforeAfter, _eventView.Filter.IsBeforeAfterEnabled());
            LoadTypeCombo(-1);
            SetEventViewDirty(true);
            RefreshView();
        }

        private void ValueChanged_EventType()
        {
            if (_internalUpdate > 0)
            {
                return;
            }

            int index = _comboEventType.SelectedIndex;
            if (index >= 1)
            {
                ValueListItem item = (ValueListItem)_comboEventType.SelectedItem;
                CMEventType eventType = (CMEventType)item.DataValue;
                _eventView.Filter.EventTypeId = eventType.TypeId;
            }
            else
            {
                _eventView.Filter.EventTypeId = -1;
            }
            EnableGroupTools(_groupEventBeforeAfter, _eventView.Filter.IsBeforeAfterEnabled());
            SetEventViewDirty(true);
            RefreshView();
        }

        private void ValueChanged_EventApplication()
        {
            if (_internalUpdate > 0)
            {
                return;
            }

            int index = _comboEventApplication.SelectedIndex;
            if (index == 0)
            {
                _eventView.Filter.ApplicationId = null;
            }
            else
            {
                ValueListItem item = (ValueListItem)_comboEventApplication.SelectedItem;
                int id = (int)item.DataValue;
                _eventView.Filter.ApplicationId = id;
            }
            SetEventViewDirty(true);
            RefreshView();
        }

        private void ValueChanged_EventHost()
        {
            if (_internalUpdate > 0)
            {
                return;
            }

            int index = _comboEventHost.SelectedIndex;
            if (index == 0)
            {
                _eventView.Filter.HostId = null;
            }
            else
            {
                ValueListItem item = (ValueListItem)_comboEventHost.SelectedItem;
                int id = (int)item.DataValue;
                _eventView.Filter.HostId = id;
            }
            SetEventViewDirty(true);
            RefreshView();

        }

        private void ValueChanged_EventLogin()
        {
            if (_internalUpdate > 0)
            {
                return;
            }

            int index = _comboEventLogin.SelectedIndex;
            switch (index)
            {
                case 0:
                    _eventView.Filter.ShowPrivUsersOnly = false;
                    _eventView.Filter.LoginId = null;
                    break;
                case 2:
                    _eventView.Filter.ShowPrivUsersOnly = true;
                    _eventView.Filter.LoginId = null;
                    break;
                default:
                    _eventView.Filter.ShowPrivUsersOnly = false;
                    ValueListItem item = (ValueListItem)_comboEventLogin.SelectedItem;
                    int id = (int)item.DataValue;
                    _eventView.Filter.LoginId = id;
                    break;
            }
            SetEventViewDirty(true);
            RefreshView();
        }

        private void ValueChanged_EventTable()
        {
            if (_internalUpdate > 0)
            {
                return;
            }

            int index = _comboEventTable.SelectedIndex;
            if (index == 0)
            {
                _eventView.Filter.TableId = null;
            }
            else
            {
                ValueListItem item = (ValueListItem)_comboEventTable.SelectedItem;
                int id = (int)item.DataValue;
                _eventView.Filter.TableId = id;
            }
            SetEventViewDirty(true);
            RefreshView();
        }

        private void ValueChanged_EventColumn()
        {
            if (_internalUpdate > 0)
            {
                return;
            }

            int index = _comboEventColumn.SelectedIndex;
            if (index == 0)
            {
                _eventView.Filter.ColumnId = null;
            }
            else
            {
                ValueListItem item = (ValueListItem)_comboEventColumn.SelectedItem;
                int id = (int)item.DataValue;
                _eventView.Filter.ColumnId = id;
            }
            SetEventViewDirty(true);
            RefreshView();
        }

        private void Click_EventViewDefault()
        {
            if (_internalUpdate > 0)
            {
                return;
            }

            ValueListItem item = (ValueListItem)_comboEventViews.SelectedItem;
            EventViewSettings settings = (EventViewSettings)item.DataValue;
            if (_checkEventDefaultView.Checked)
            {
                Settings.Default.DefaultEventView = settings.Name;
            }
            else
            {
                Settings.Default.DefaultEventView = "";
            }
        }

        private void InitializeEventViewFilterCombos()
        {
            _internalUpdate++;

            try
            {
                List<NameIdPair> pairs;
                string databaseName;
                int matchIndex = -1;
                int counter;
                bool matchFound;

                if (_scope is ServerRecord)
                {
                    databaseName = ((ServerRecord)_scope).EventDatabase;
                }
                else if (_scope is DatabaseRecord)
                {
                    ServerRecord server = ServerRecord.GetServer(Globals.Repository.Connection, ((DatabaseRecord)_scope).SrvInstance);
                    databaseName = server.EventDatabase;
                }
                else
                {
                    return;
                }

                // Applications
                matchFound = false;
                pairs = GetApplications(databaseName);
                _comboEventApplication.ValueList.ValueListItems.Clear();
                _comboEventApplication.ValueList.ValueListItems.Add("(All)");
                _comboEventApplication.ValueList.ValueListItems.Add(5381, "(Empty)");
                counter = 1;
                foreach (NameIdPair pair in pairs)
                {
                    // Skip the blank one, we already have it
                    if (pair.Id == 5381)
                    {
                        continue;
                    }

                    counter++;
                    _comboEventApplication.ValueList.ValueListItems.Add(pair.Id, pair.Name);
                    if (_eventView.Filter.ApplicationId != null && pair.Id == _eventView.Filter.ApplicationId.GetValueOrDefault(-1))
                    {
                        matchIndex = counter;
                        matchFound = true;
                    }
                }

                // We go through these in order to determine which item should be selected.
                if (_eventView.Filter.ApplicationId == null)
                {
                    _comboEventApplication.SelectedIndex = 0;
                }
                else if (_eventView.Filter.ApplicationId == 5381)
                {
                    _comboEventApplication.SelectedIndex = 1;
                }
                else if (!matchFound)
                {
                    SetComboNotAvailable(_comboEventApplication, 2, _eventView.Filter.ApplicationId.GetValueOrDefault(-1));
                }
                else
                {
                    _comboEventApplication.SelectedIndex = matchIndex;
                }

                // Hosts
                matchFound = false;
                pairs = GetHosts(databaseName);
                _comboEventHost.ValueList.ValueListItems.Clear();
                _comboEventHost.ValueList.ValueListItems.Add("(All)");
                _comboEventHost.ValueList.ValueListItems.Add(5381, "(Empty)");
                counter = 1;
                foreach (NameIdPair pair in pairs)
                {
                    // Skip the blank one, we already have it
                    if (pair.Id == 5381)
                    {
                        continue;
                    }

                    counter++;
                    _comboEventHost.ValueList.ValueListItems.Add(pair.Id, pair.Name);
                    if (_eventView.Filter.HostId != null && pair.Id == _eventView.Filter.HostId.GetValueOrDefault(-1))
                    {
                        matchIndex = counter;
                        matchFound = true;
                    }
                }
                // We go through these in order to determine which item should be selected.
                if (_eventView.Filter.HostId == null)
                {
                    _comboEventHost.SelectedIndex = 0;
                }
                else if (_eventView.Filter.HostId == 5381)
                {
                    _comboEventHost.SelectedIndex = 1;
                }
                else if (!matchFound)
                {
                    SetComboNotAvailable(_comboEventHost, 2, _eventView.Filter.HostId.GetValueOrDefault(-1));
                }
                else
                {
                    _comboEventHost.SelectedIndex = matchIndex;
                }

                // Logins
                matchFound = false;
                pairs = GetLogins(databaseName);
                _comboEventLogin.ValueList.ValueListItems.Clear();
                _comboEventLogin.ValueList.ValueListItems.Add("(All)");
                _comboEventLogin.ValueList.ValueListItems.Add(5381, "(Empty)");
                _comboEventLogin.ValueList.ValueListItems.Add("(Privileged Users)");
                counter = 2;
                foreach (NameIdPair pair in pairs)
                {
                    // Skip the blank one, we already have it
                    if (pair.Id == 5381)
                    {
                        continue;
                    }

                    counter++;
                    _comboEventLogin.ValueList.ValueListItems.Add(pair.Id, pair.Name);
                    if (_eventView.Filter.LoginId != null && pair.Id == _eventView.Filter.LoginId.GetValueOrDefault(-1))
                    {
                        matchIndex = counter;
                        matchFound = true;
                    }
                }
                // We go through these in order to determine which item should be selected.
                if (_eventView.Filter.ShowPrivUsersOnly)
                {
                    _comboEventLogin.SelectedIndex = 2;
                }
                else if (_eventView.Filter.LoginId == null)
                {
                    _comboEventLogin.SelectedIndex = 0;
                }
                else if (_eventView.Filter.LoginId == 5381)
                {
                    _comboEventLogin.SelectedIndex = 1;
                }
                else if (!matchFound)
                {
                    SetComboNotAvailable(_comboEventLogin, 3, _eventView.Filter.LoginId.GetValueOrDefault(-1));
                }
                else
                {
                    _comboEventLogin.SelectedIndex = matchIndex;
                }

                // Tables
                matchFound = false;
                pairs = GetTables(databaseName);
                _comboEventTable.ValueList.ValueListItems.Clear();
                _comboEventTable.ValueList.ValueListItems.Add("(All)");
                _comboEventTable.ValueList.ValueListItems.Add(5381, "(Empty)");
                counter = 1;
                foreach (NameIdPair pair in pairs)
                {
                    // Skip the blank one, we already have it
                    if (pair.Id == 5381)
                    {
                        continue;
                    }

                    counter++;
                    _comboEventTable.ValueList.ValueListItems.Add(pair.Id, pair.Name);
                    if (_eventView.Filter.TableId != null && pair.Id == _eventView.Filter.TableId.GetValueOrDefault(-1))
                    {
                        matchIndex = counter;
                        matchFound = true;
                    }
                }
                // We go through these in order to determine which item should be selected.
                if (_eventView.Filter.TableId == null)
                {
                    _comboEventTable.SelectedIndex = 0;
                }
                else if (_eventView.Filter.TableId == 5381)
                {
                    _comboEventTable.SelectedIndex = 1;
                }
                else if (!matchFound)
                {
                    SetComboNotAvailable(_comboEventTable, 2, _eventView.Filter.TableId.GetValueOrDefault(-1));
                }
                else
                {
                    _comboEventTable.SelectedIndex = matchIndex;
                }

                // Columns
                matchFound = false;
                pairs = GetColumns(databaseName);
                _comboEventColumn.ValueList.ValueListItems.Clear();
                _comboEventColumn.ValueList.ValueListItems.Add("(All)");
                _comboEventColumn.ValueList.ValueListItems.Add(5381, "(Empty)");
                counter = 1;
                foreach (NameIdPair pair in pairs)
                {
                    // Skip the blank one, we already have it
                    if (pair.Id == 5381)
                    {
                        continue;
                    }

                    counter++;
                    _comboEventColumn.ValueList.ValueListItems.Add(pair.Id, pair.Name);
                    if (_eventView.Filter.ColumnId != null && pair.Id == _eventView.Filter.ColumnId.GetValueOrDefault(-1))
                    {
                        matchIndex = counter;
                        matchFound = true;
                    }
                }
                // We go through these in order to determine which item should be selected.
                if (_eventView.Filter.ColumnId == null)
                {
                    _comboEventColumn.SelectedIndex = 0;
                }
                else if (_eventView.Filter.ColumnId == 5381)
                {
                    _comboEventColumn.SelectedIndex = 1;
                }
                else if (!matchFound)
                {
                    SetComboNotAvailable(_comboEventColumn, 2, _eventView.Filter.ColumnId.GetValueOrDefault(-1));
                }
                else
                {
                    _comboEventColumn.SelectedIndex = matchIndex;
                }
            }
            finally
            {
                _internalUpdate--;
            }
        }

        private void SetComboNotAvailable(ComboBoxTool combo, int position, int id)
        {
            ValueListItemsCollection items = combo.ValueList.ValueListItems;

            // In this case, we already have a not available, just set the id for it
            if (items.Count > position && items[position].DisplayText.Equals("(Not Available"))
            {
                items[position].DataValue = id;
            }
            else
            {
                ValueListItem item = new ValueListItem(id, "(Not Available)");
                items.Insert(position, item);
            }
            combo.SelectedIndex = position;
        }

        private void InitializeCombos(List<CMEventCategory> categories)
        {
            _categories = new Dictionary<string, ValueListItem>();
            _archiveCategories = new Dictionary<string, ValueListItem>();
            List<ValueListItem> items = new List<ValueListItem>();
            List<ValueListItem> archiveItems = new List<ValueListItem>();

            foreach (CMEventCategory category in categories)
            {
                ValueListItem item = new ValueListItem(category, category.Name);
                _categories[category.Name] = item;
                items.Add(item);

                // Archive combos
                item = new ValueListItem(category, category.Name);
                _archiveCategories[category.Name] = item;
                archiveItems.Add(item);
            }
            items.Sort(delegate (ValueListItem a, ValueListItem b)
               { return string.Compare(((CMEventCategory)a.DataValue).Name, ((CMEventCategory)b.DataValue).Name); });
            archiveItems.Sort(delegate (ValueListItem a, ValueListItem b)
               { return string.Compare(((CMEventCategory)a.DataValue).Name, ((CMEventCategory)b.DataValue).Name); });

            _internalUpdate++;

            try
            {
                _comboEventCategory.ValueList.ValueListItems.Clear();
                _comboEventCategory.ValueList.ValueListItems.AddRange(items.ToArray());
                _comboEventCategory.ValueList.ValueListItems.Insert(0, "All Categories", "All Categories");
                _comboEventCategory.SelectedIndex = 0;
                LoadTypeCombo(-1);

                // Archive
                _comboArchiveCategory.ValueList.ValueListItems.Clear();
                _comboArchiveCategory.ValueList.ValueListItems.AddRange(archiveItems.ToArray());
                _comboArchiveCategory.ValueList.ValueListItems.Insert(0, "All Categories", "All Categories");
                _comboArchiveCategory.SelectedIndex = 0;
                LoadArchiveTypeCombo(-1);
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write("InitializeCombos", e);
            }
            _internalUpdate--;
        }

        /// <summary>
        /// Load the eventType combo box based on the current category selection.  Select the
        /// supplied ID after loading
        /// </summary>
        /// <param name="selectedId">The eventTypeId to select after loading the combo box</param>
        private void LoadTypeCombo(int selectedId)
        {
            List<ValueListItem> items = new List<ValueListItem>();
            int index = _comboEventCategory.SelectedIndex;
            CMEventCategory category = null;
            ValueListItem selectedType = null;

            // Ignore the All selection (index 0)
            if (index >= 1)
            {
                ValueListItem item = (ValueListItem)_comboEventCategory.SelectedItem;
                category = (CMEventCategory)item.DataValue;
            }

            _internalUpdate++;
            try
            {
                _comboEventType.ValueList.ValueListItems.Clear();

                if (category != null)
                {
                    foreach (CMEventType evType in category.EventTypes)
                    {
                        ValueListItem item = new ValueListItem(evType, evType.Name);
                        items.Add(item);
                        if (evType.TypeId == selectedId)
                        {
                            selectedType = item;
                        }
                    }
                    items.Sort(
                       delegate (ValueListItem a, ValueListItem b)
                       { return string.Compare(((CMEventType)a.DataValue).Name, ((CMEventType)b.DataValue).Name); });
                }

                ValueListItem[] itemsToShow = ExcludeItems(items);
                _comboEventType.ValueList.ValueListItems.AddRange(itemsToShow);
                _comboEventType.ValueList.ValueListItems.Insert(0, "All Types", "All Types");
                if (selectedType != null)
                {
                    _comboEventType.SelectedItem = selectedType;
                    foreach (CMEventType evType in category.EventTypes)
                    {
                        if (selectedId == evType.TypeId)
                        {
                            _comboEventType.SelectedItem = evType;
                        }
                    }
                }
                else
                {
                    _comboEventType.SelectedIndex = 0;
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write("LoadTypeCombo", e);
            }
            _internalUpdate--;
        }

        private ValueListItem[] ExcludeItems(List<ValueListItem> items)
        {
            List<ValueListItem> itemsToExclude =
                items.FindAll(x => x.DisplayText == CREATE_INDEX ||
                                   x.DisplayText == TRACE_STARTED ||
                                   x.DisplayText == TRACE_STOPPED);

            foreach (ValueListItem itemToExclude in itemsToExclude)
            {
                items.Remove(itemToExclude);
            }

            return items.ToArray();
        }

        public List<NameIdPair> GetApplications(string targetDatabase)
        {
            return GetLookupItems(targetDatabase, CoreConstants.RepositoryApplicationsTable);
        }

        public List<NameIdPair> GetHosts(string targetDatabase)
        {
            return GetLookupItems(targetDatabase, CoreConstants.RepositoryHostsTable);
        }

        public List<NameIdPair> GetLogins(string targetDatabase)
        {
            return GetLookupItems(targetDatabase, CoreConstants.RepositoryLoginsTable);
        }

        public List<NameIdPair> GetTables(string targetDatabase)
        {
            return GetLookupItems(targetDatabase, CoreConstants.RepositoryTablesTable);
        }

        public List<NameIdPair> GetColumns(string targetDatabase)
        {
            return GetLookupItems(targetDatabase, CoreConstants.RepositoryColumnsTable);
        }

        private static List<NameIdPair> GetLookupItems(string databaseName, string tableName)
        {
            List<NameIdPair> retVal = new List<NameIdPair>();
            bool isTables = tableName == CoreConstants.RepositoryTablesTable ? true : false;
            string baseQuery = isTables ? "SELECT TOP (250)schemaName, name,id FROM {0}..{1} ORDER BY name ASC"
                                        : "SELECT TOP (250)name,id FROM {0}..{1} ORDER BY name ASC";
            string query;

            try
            {
                query = string.Format(baseQuery,
                                      SQLHelpers.CreateSafeDatabaseName(databaseName), tableName);
                using (SqlCommand cmd = new SqlCommand(query, Globals.Repository.Connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (isTables)
                        {
                            while (reader.Read())
                            {
                                NameIdPair item = new NameIdPair();
                                string schema = SQLHelpers.GetString(reader, 0);
                                string table = SQLHelpers.GetString(reader, 1);
                                item.Name = CoreHelpers.GetTableNameKey(schema, table);
                                item.Id = SQLHelpers.GetInt32(reader, 2);
                                retVal.Add(item);
                            }
                        }
                        else
                        {
                            while (reader.Read())
                            {
                                NameIdPair item = new NameIdPair
                                {
                                    Name = SQLHelpers.GetString(reader, 0),
                                    Id = SQLHelpers.GetInt32(reader, 1)
                                };
                                retVal.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(string.Format("Unable to lookup ids from {0}..{1}", databaseName, tableName), e);
            }
            return retVal;
        }

        #endregion Event Ribbon

        #region Archive Ribbon

        /// <summary>
        /// Load the eventType combo box based on the current category selection.  Select the
        /// supplied ID after loading
        /// </summary>
        /// <param name="selectedId">The eventTypeId to select after loading the combo box</param>
        private void LoadArchiveTypeCombo(int selectedId)
        {
            List<ValueListItem> items = new List<ValueListItem>();
            int index = _comboArchiveCategory.SelectedIndex;
            CMEventCategory category = null;
            ValueListItem selectedType = null;

            // Ignore the All selection (index 0)
            if (index >= 1)
            {
                ValueListItem item = (ValueListItem)_comboArchiveCategory.SelectedItem;
                category = (CMEventCategory)item.DataValue;
            }

            _internalUpdate++;
            try
            {
                _comboArchiveType.ValueList.ValueListItems.Clear();

                if (category != null)
                {
                    foreach (CMEventType evType in category.EventTypes)
                    {
                        ValueListItem item = new ValueListItem(evType, evType.Name);
                        items.Add(item);
                        if (evType.TypeId == selectedId)
                        {
                            selectedType = item;
                        }
                    }
                    items.Sort(
                       delegate (ValueListItem a, ValueListItem b)
                       { return string.Compare(((CMEventType)a.DataValue).Name, ((CMEventType)b.DataValue).Name); });
                }

                ValueListItem[] itemsToShow = ExcludeItems(items);
                _comboArchiveType.ValueList.ValueListItems.AddRange(itemsToShow);
                _comboArchiveType.ValueList.ValueListItems.Insert(0, "All Types", "All Types");
                if (selectedType != null)
                {
                    _comboArchiveType.SelectedItem = selectedType;
                    foreach (CMEventType evType in category.EventTypes)
                    {
                        if (selectedId == evType.TypeId)
                        {
                            _comboArchiveType.SelectedItem = evType;
                        }
                    }
                }
                else
                {
                    _comboArchiveType.SelectedIndex = 0;
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write("LoadTypeCombo", e);
            }
            _internalUpdate--;
        }

        private void AfterColPosChanged_ArchiveGrid(object sender, AfterColPosChangedEventArgs e)
        {
            if (_internalUpdate > 0)
            {
                return;
            }

            SetArchiveViewDirty(true);
        }

        private void AfterSortChange_ArchiveGrid(object sender, BandEventArgs e)
        {
            if (_internalUpdate > 0)
            {
                return;
            }

            SetArchiveViewDirty(true);
        }

        private void SetArchiveGroupBy(bool value)
        {
            _checkArchiveGroupBy.Checked = value;
            _archiveEventView.ShowGroupBy = value;
            _btnArchiveCollapseAll.SharedProps.Enabled = value;
            _btnArchiveExpandAll.SharedProps.Enabled = value;
            SetMenuFlag(CMMenuItem.Collapse, value);
            SetMenuFlag(CMMenuItem.Expand, value);
        }

        private void SetArchiveGridViewMode(GridViewMode value)
        {
            if (value == GridViewMode.Flat)
            {
                _checkArchiveFlatMode.InitializeChecked(true);
            }
            else
            {
                _checkArchiveFlatMode.InitializeChecked(false);
            }
            _archiveEventView.ViewMode = value;
            if (_internalUpdate == 0)
            {
                SetArchiveViewDirty(true);
            }
        }

        private void SetArchiveViewDirty(bool value)
        {
                //if (value == _archiveEventViewDirty || _comboArchiveViews.SelectedItem == null)
                //{
                //    return;
                //}
                if (value == _archiveEventViewDirty || _comboArchiveViews.SelectedItem != null)
                    return;

                _archiveEventViewDirty = value;
                if (value)
                {
                    ValueListItem oldItem, newItem;
                    oldItem = (ValueListItem)_comboArchiveViews.SelectedItem;
                    EventViewSettings oldSelection = (EventViewSettings)oldItem.DataValue;
                    EventViewSettings newSelection = oldSelection.Clone();
                    newSelection.Name = "* " + newSelection.Name;
                    newItem = _comboArchiveViews.ValueList.ValueListItems.Add(newSelection, newSelection.Name);
                    _internalUpdate++;
                    _comboArchiveViews.SelectedItem = newItem;
                    _internalUpdate--;
                }
                else
                {
                    if (_comboArchiveViews.ValueList.ValueListItems.Count > 0)
                    {
                        ValueListItem oldDirtyItem = _comboArchiveViews.ValueList.ValueListItems[0];
                        EventViewSettings oldDirty = (EventViewSettings)oldDirtyItem.DataValue;
                        if (oldDirty.Name.StartsWith("*"))
                        {
                            _comboArchiveViews.ValueList.ValueListItems.RemoveAt(0);
                        }
                    }
                }
                _btnArchiveSave.SharedProps.Enabled = value;
        }

        private void Click_ArchiveViewsSaveAs()
        {
            string[] views = new string[Settings.Default.EventViews.Count];

            for (int i = 0; i < views.Length; i++)
            {
                views[i] = Settings.Default.EventViews[i].Name;
            }
            Form_ViewName frm = new Form_ViewName(views);
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                EventViewSettings settings = new EventViewSettings
                {
                    Name = frm.ViewName
                };
                ExtractArchiveViewSettings(settings);
                Settings.Default.EventViews.Add(settings);
                ValueListItem item = _comboArchiveViews.ValueList.ValueListItems.Add(settings);
                _comboArchiveViews.SelectedItem = item;
                SetArchiveViewDirty(false);
            }
        }

        private void Click_ArchiveViewsSave()
        {
            EventViewSettings settings, originalSettings = null;
            ValueListItem originalItem = null;
            ValueListItem item = (ValueListItem)_comboArchiveViews.SelectedItem;

            settings = (EventViewSettings)item.DataValue;
            foreach (ValueListItem subItem in _comboArchiveViews.ValueList.ValueListItems)
            {
                EventViewSettings x = (EventViewSettings)subItem.DataValue;
                if (settings != x &&
                   settings.Name.Substring(2) == x.Name)
                {
                    originalItem = subItem;
                    originalSettings = x;
                    break;
                }
            }
            if (originalSettings != null)
            {
                ExtractArchiveViewSettings(originalSettings);
                _comboArchiveViews.SelectedItem = originalItem;
            }
            SetArchiveViewDirty(false);
        }

        private void Click_ArchiveEnableGroupBy()
        {
            if (_internalUpdate > 0)
            {
                return;
            }

            SetArchiveGroupBy(_checkArchiveGroupBy.Checked);
            SetArchiveViewDirty(true);
        }

        private void ExtractArchiveViewSettings(EventViewSettings settings)
        {
            if (settings == null)
            {
                return;
            }

            settings.Columns = GridColumnSettings.ExtractColumnSettings(_archiveEventView.ActiveGrid);
            settings.Filter = _archiveEventView.Filter.Clone();
            settings.ShowGroupBy = _checkArchiveGroupBy.Checked;
            settings.ViewMode = _archiveEventView.ViewMode;
        }

        private void ApplyArchiveView(EventViewSettings settings)
        {
            _internalUpdate++;
            try
            {
                _archiveEventView.Filter = settings.Filter.Clone();
                _checkArchiveDefaultView.InitializeChecked(Settings.Default.DefaultArchiveView == settings.Name);
                SetArchiveGroupBy(settings.ShowGroupBy);
                GridColumnSettings.ApplyColumnSettings(_archiveEventView.GetGrid(settings.ViewMode), settings.Columns);
                SetArchiveGridViewMode(settings.ViewMode);
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose, "ApplyArchiveView", e, ErrorLog.Severity.Warning);
            }
            _internalUpdate--;
            if (_scope != null)
            {
                UpdateArchiveFilterDisplay();
            }
        }

        private void UpdateArchiveFilterDisplay()
        {
            _internalUpdate++;

            bool matchFound;

            try
            {
                EventViewFilter filter = _archiveEventView.Filter;

                // Date Filter
                switch (filter.DateLimitType)
                {
                    case DateFilterType.Unlimited:
                        _comboArchiveTime.SelectedIndex = 0;
                        break;
                    case DateFilterType.NumberDays:
                        if (filter.Days == 7)
                        {
                            _comboArchiveTime.SelectedIndex = 2;
                        }
                        else if (filter.Days == 30)
                        {
                            _comboArchiveTime.SelectedIndex = 3;
                        }
                        else
                        {
                            _comboArchiveTime.SelectedIndex = 4;
                        }

                        break;
                    case DateFilterType.DateRange:
                        _archiveCustomTime.AddCustomRange(_comboArchiveTime, filter.StartDate, filter.EndDate);
                        _comboArchiveTime.SelectedIndex = 5;
                        break;
                    case DateFilterType.Today:
                        _comboArchiveTime.SelectedIndex = 1;
                        break;
                }
                _archiveTimeFilterLastIndex = _comboArchiveTime.SelectedIndex;

                // Category and Type Filters
                if (filter.EventCategoryId == -1)
                {
                    _comboArchiveCategory.SelectedIndex = 0;
                    LoadArchiveTypeCombo(-1);
                }
                else
                {
                    CMEventCategory targetCategory = Globals.Repository.LookupEventCategory(filter.EventCategoryId);
                    _comboArchiveCategory.SelectedItem = _archiveCategories[targetCategory.Name];
                    LoadArchiveTypeCombo(filter.EventTypeId);
                }

                // Table filter
                _comboArchiveTable.SelectedIndex = 0;
                if (filter.TableId != null)
                {
                    matchFound = false;
                    for (int i = 0; i < _comboArchiveTable.ValueList.ValueListItems.Count; i++)
                    {
                        if (_comboArchiveTable.ValueList.ValueListItems[i].DataValue.Equals(filter.TableId))
                        {
                            _comboArchiveTable.SelectedIndex = i;
                            matchFound = true;
                            break;
                        }
                    }
                    if (!matchFound)
                    {
                        SetComboNotAvailable(_comboArchiveTable, 2, filter.TableId.GetValueOrDefault(-1));
                    }
                }

                // Column filter
                _comboArchiveColumn.SelectedIndex = 0;
                if (filter.ColumnId != null)
                {
                    matchFound = false;
                    for (int i = 0; i < _comboArchiveColumn.ValueList.ValueListItems.Count; i++)
                    {
                        if (_comboArchiveColumn.ValueList.ValueListItems[i].DataValue.Equals(filter.ColumnId))
                        {
                            _comboArchiveColumn.SelectedIndex = i;
                            matchFound = true;
                            break;
                        }
                    }
                    if (!matchFound)
                    {
                        SetComboNotAvailable(_comboArchiveColumn, 2, filter.ColumnId.GetValueOrDefault(-1));
                    }
                }

                // Application filter
                _comboArchiveApplication.SelectedIndex = 0;
                if (filter.ApplicationId != null)
                {
                    matchFound = false;
                    for (int i = 0; i < _comboArchiveApplication.ValueList.ValueListItems.Count; i++)
                    {
                        if (_comboArchiveApplication.ValueList.ValueListItems[i].DataValue.Equals(filter.ApplicationId))
                        {
                            _comboArchiveApplication.SelectedIndex = i;
                            matchFound = true;
                            break;
                        }
                    }
                    if (!matchFound)
                    {
                        SetComboNotAvailable(_comboArchiveApplication, 2, filter.ApplicationId.GetValueOrDefault(-1));
                    }
                }

                // Host Filter
                _comboArchiveHost.SelectedIndex = 0;
                if (filter.HostId != null)
                {
                    matchFound = false;
                    for (int i = 0; i < _comboArchiveHost.ValueList.ValueListItems.Count; i++)
                    {
                        if (_comboArchiveHost.ValueList.ValueListItems[i].DataValue.Equals(filter.HostId))
                        {
                            _comboArchiveHost.SelectedIndex = i;
                            matchFound = true;
                            break;
                        }
                    }
                    if (!matchFound)
                    {
                        SetComboNotAvailable(_comboArchiveHost, 2, filter.HostId.GetValueOrDefault(-1));
                    }
                }

                if (filter.ShowPrivUsersOnly)
                {
                    _comboArchiveLogin.SelectedIndex = 2;
                }
                else
                {
                    _comboArchiveLogin.SelectedIndex = 0;
                    if (filter.LoginId != null)
                    {
                        matchFound = false;
                        for (int i = 0; i < _comboArchiveLogin.ValueList.ValueListItems.Count; i++)
                        {
                            if (_comboArchiveLogin.ValueList.ValueListItems[i].DataValue.Equals(filter.LoginId))
                            {
                                _comboArchiveLogin.SelectedIndex = i;
                                matchFound = true;
                                break;
                            }
                        }
                        if (!matchFound)
                        {
                            SetComboNotAvailable(_comboArchiveLogin, 3, filter.LoginId.GetValueOrDefault(-1));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write("UpdateFilterDisplay", e);
            }
            _internalUpdate--;
        }

        private void Click_ArchiveViewsDelete()
        {
            ValueListItem item = (ValueListItem)_comboArchiveViews.SelectedItem;
            EventViewSettings settings = (EventViewSettings)item.DataValue;

            if (MessageBox.Show(this, string.Format("Do you wish to delete the selected view:  {0}?",
               settings.Name), "Confirm Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                for (int i = 0; i < Settings.Default.EventViews.Count; i++)
                {
                    // Remove it from the Settings object
                    if (Settings.Default.EventViews[i].Name == settings.Name)
                    {
                        Settings.Default.EventViews.RemoveAt(i);
                        break;
                    }
                }
                _comboArchiveViews.ValueList.ValueListItems.RemoveAt(_comboArchiveViews.SelectedIndex);
                // Force refresh also - InitializeViewCombo does not refresh view
                InitializeEventViewCombo();
                item = (ValueListItem)_comboArchiveViews.SelectedItem;
                settings = (EventViewSettings)item.DataValue;
                ApplyArchiveView(settings);
                RefreshView();
            }
        }

        private void ValueChanged_ArchiveList()
        {
            if (_internalUpdate > 0)
            {
                return;
            }

            if (_comboArchiveList.SelectedItem == null)
            {
                if (_comboArchiveList.ValueList.ValueListItems.Count > 0)
                {
                    _comboArchiveList.SelectedIndex = 0;
                    _toolbarManager.Tools["archiveDetach"].SharedProps.Enabled = Globals.isAdmin;
                }
                else
                {
                    _toolbarManager.Tools["archiveDetach"].SharedProps.Enabled = false;
                }
                return;
            }
            RefreshView();
        }

        private void ValueChanged_ArchiveView()
        {
            if (_internalUpdate > 0)
            {
                return;
            }

            if (_comboArchiveViews.SelectedItem == null)
            {
                if (_comboArchiveViews.ValueList.ValueListItems.Count > 0)
                {
                    _comboArchiveViews.SelectedIndex = 0;
                }

                return;
            }
            // Ignore edits
            if (_comboArchiveViews.Text.StartsWith("*"))
            {
                return;
            }

            SetArchiveViewDirty(false);
            ValueListItem item = (ValueListItem)_comboArchiveViews.SelectedItem;
            EventViewSettings settings = (EventViewSettings)item.DataValue;
            ApplyArchiveView(settings);
            RefreshView();
        }

        private void ValueChanged_ArchiveTime()
        {
            if (_internalUpdate > 0)
            {
                return;
            }

            EventViewFilter filter = _archiveEventView.Filter;

            switch (_comboArchiveTime.SelectedIndex)
            {
                case 0:
                    filter.DateLimitType = DateFilterType.Unlimited;
                    break;
                case 1:
                    filter.DateLimitType = DateFilterType.Today;
                    break;
                case 2:
                    filter.DateLimitType = DateFilterType.NumberDays;
                    filter.Days = 7;
                    break;
                case 3:
                    filter.DateLimitType = DateFilterType.NumberDays;
                    filter.Days = 30;
                    break;
                case 4:
                    _archiveCustomTime.GetCustomTime(_comboArchiveTime, _archiveTimeFilterLastIndex);
                    break;
                default:
                    {
                        // custom
                        int index = _comboArchiveTime.SelectedIndex - 5;
                        filter.DateLimitType = DateFilterType.DateRange;
                        filter.StartDate = _archiveCustomTime.StartTime[index].Value;
                        filter.EndDate = _archiveCustomTime.EndTime[index].Value;
                    }
                    break;
            }
            _archiveTimeFilterLastIndex = _comboArchiveTime.SelectedIndex;
            SetArchiveViewDirty(true);
            RefreshView();
        }

        private void ValueChanged_ArchiveCategory()
        {
            if (_internalUpdate > 0)
            {
                return;
            }

            int index = _comboArchiveCategory.SelectedIndex;
            if (index >= 1)
            {
                ValueListItem item = (ValueListItem)_comboArchiveCategory.SelectedItem;
                CMEventCategory archiveCategory = (CMEventCategory)item.DataValue;
                _archiveEventView.Filter.EventCategoryId = archiveCategory.CategoryId;
            }
            else
            {
                _archiveEventView.Filter.EventCategoryId = -1;
            }
            // Always reset archive id when category changes
            _archiveEventView.Filter.EventTypeId = -1;
            EnableGroupTools(_groupArchiveBeforeAfter, _archiveEventView.Filter.IsBeforeAfterEnabled());
            LoadArchiveTypeCombo(-1);
            SetArchiveViewDirty(true);
            RefreshView();
        }

        private void ValueChanged_ArchiveType()
        {
            if (_internalUpdate > 0)
            {
                return;
            }

            int index = _comboArchiveType.SelectedIndex;
            if (index >= 1)
            {
                ValueListItem item = (ValueListItem)_comboArchiveType.SelectedItem;
                CMEventType archiveType = (CMEventType)item.DataValue;
                _archiveEventView.Filter.EventTypeId = archiveType.TypeId;
            }
            else
            {
                _archiveEventView.Filter.EventTypeId = -1;
            }
            EnableGroupTools(_groupArchiveBeforeAfter, _archiveEventView.Filter.IsBeforeAfterEnabled());
            SetArchiveViewDirty(true);
            RefreshView();
        }

        private void ValueChanged_ArchiveApplication()
        {
            if (_internalUpdate > 0)
            {
                return;
            }

            int index = _comboArchiveApplication.SelectedIndex;
            if (index == 0)
            {
                _archiveEventView.Filter.ApplicationId = null;
            }
            else
            {
                ValueListItem item = (ValueListItem)_comboArchiveApplication.SelectedItem;
                int id = (int)item.DataValue;
                _archiveEventView.Filter.ApplicationId = id;
            }
            SetArchiveViewDirty(true);
            RefreshView();
        }

        private void ValueChanged_ArchiveHost()
        {
            if (_internalUpdate > 0)
            {
                return;
            }

            int index = _comboArchiveHost.SelectedIndex;
            if (index == 0)
            {
                _archiveEventView.Filter.HostId = null;
            }
            else
            {
                ValueListItem item = (ValueListItem)_comboArchiveHost.SelectedItem;
                int id = (int)item.DataValue;
                _archiveEventView.Filter.HostId = id;
            }
            SetArchiveViewDirty(true);
            RefreshView();

        }

        private void ValueChanged_ArchiveLogin()
        {
            if (_internalUpdate > 0)
            {
                return;
            }

            int index = _comboArchiveLogin.SelectedIndex;
            switch (index)
            {
                case 0:
                    _archiveEventView.Filter.ShowPrivUsersOnly = false;
                    _archiveEventView.Filter.LoginId = null;
                    break;
                case 2:
                    _archiveEventView.Filter.ShowPrivUsersOnly = true;
                    _archiveEventView.Filter.LoginId = null;
                    break;
                default:
                    _archiveEventView.Filter.ShowPrivUsersOnly = false;
                    ValueListItem item = (ValueListItem)_comboArchiveLogin.SelectedItem;
                    int id = (int)item.DataValue;
                    _archiveEventView.Filter.LoginId = id;
                    break;
            }
            SetArchiveViewDirty(true);
            RefreshView();
        }

        private void ValueChanged_ArchiveTable()
        {
            if (_internalUpdate > 0)
            {
                return;
            }

            int index = _comboArchiveTable.SelectedIndex;
            if (index == 0)
            {
                _archiveEventView.Filter.TableId = null;
            }
            else
            {
                ValueListItem item = (ValueListItem)_comboArchiveTable.SelectedItem;
                int id = (int)item.DataValue;
                _archiveEventView.Filter.TableId = id;
            }
            SetArchiveViewDirty(true);
            RefreshView();
        }

        private void ValueChanged_ArchiveColumn()
        {
            if (_internalUpdate > 0)
            {
                return;
            }

            int index = _comboArchiveColumn.SelectedIndex;
            if (index == 0)
            {
                _archiveEventView.Filter.ColumnId = null;
            }
            else
            {
                ValueListItem item = (ValueListItem)_comboArchiveColumn.SelectedItem;
                int id = (int)item.DataValue;
                _archiveEventView.Filter.ColumnId = id;
            }
            SetArchiveViewDirty(true);
            RefreshView();

        }

        private void Click_ArchiveViewDefault()
        {
            if (_internalUpdate > 0)
            {
                return;
            }

            ValueListItem item = (ValueListItem)_comboArchiveViews.SelectedItem;
            EventViewSettings settings = (EventViewSettings)item.DataValue;
            if (_checkArchiveDefaultView.Checked)
            {
                Settings.Default.DefaultArchiveView = settings.Name;
            }
            else
            {
                Settings.Default.DefaultArchiveView = "";
            }
        }

        private string GetDatabaseNameFromServerRecord(ServerRecord serverRecord)
        {
            string databaseName = null;

            if (!string.IsNullOrEmpty(serverRecord.Instance))
            {
                if (serverRecord.IsAuditedServer)
                {
                    databaseName = EventDatabase.GetDatabaseName(serverRecord.Instance);
                }
                else
                {
                    List<ArchiveRecord> archiveRecords = SQLRepository.GetArchives(serverRecord.Instance);
                    databaseName = archiveRecords.Count == 0
                        ? ""
                        : archiveRecords[archiveRecords.Count - 1].DatabaseName;

                }
            }

            return databaseName;
        }

        private string GetDatabaseNameFromDabaseRecord(DatabaseRecord databaseRecord)
        {
            string databaseName = null;

            if (!string.IsNullOrEmpty(databaseRecord.SrvInstance))
            {
                databaseName = EventDatabase.GetDatabaseName(databaseRecord.SrvInstance);
            }

            return databaseName;
        }

        private void InitializeArchiveViewFilterCombos()
        {
            _internalUpdate++;

            try
            {
                string databaseName = null;
                List<NameIdPair> pairs;
                int matchIndex = -1;
                int counter;
                bool matchFound;

                ServerRecord serverRecord = _scope as ServerRecord;
                DatabaseRecord databaseRecord = _scope as DatabaseRecord;

                if (serverRecord != null)
                {
                    databaseName = GetDatabaseNameFromServerRecord(serverRecord);
                }
                else if (databaseRecord != null)
                {
                    databaseName = GetDatabaseNameFromDabaseRecord(databaseRecord);
                }

                // Applications
                matchFound = false;
                _comboArchiveApplication.ValueList.ValueListItems.Clear();
                _comboArchiveApplication.ValueList.ValueListItems.Add("(All)");
                _comboArchiveApplication.ValueList.ValueListItems.Add(5381, "(Empty)");
                if (databaseName != null)
                {
                    pairs = GetApplications(databaseName);
                    counter = 1;
                    foreach (NameIdPair pair in pairs)
                    {
                        // Skip the blank one, we already have it
                        if (pair.Id == 5381)
                        {
                            continue;
                        }

                        counter++;
                        _comboArchiveApplication.ValueList.ValueListItems.Add(pair.Id, pair.Name);
                        if (_archiveEventView.Filter.ApplicationId != null && pair.Id == _archiveEventView.Filter.ApplicationId.GetValueOrDefault(-1))
                        {
                            matchIndex = counter;
                            matchFound = true;
                        }
                    }
                }
                // We go through these in order to determine which item should be selected.
                if (_archiveEventView.Filter.ApplicationId == null)
                {
                    _comboArchiveApplication.SelectedIndex = 0;
                }
                else if (_archiveEventView.Filter.ApplicationId == 5381)
                {
                    _comboArchiveApplication.SelectedIndex = 1;
                }
                else if (!matchFound)
                {
                    SetComboNotAvailable(_comboArchiveApplication, 2, _archiveEventView.Filter.ApplicationId.GetValueOrDefault(-1));
                }
                else
                {
                    _comboArchiveApplication.SelectedIndex = matchIndex;
                }

                // Hosts
                matchFound = false;
                _comboArchiveHost.ValueList.ValueListItems.Clear();
                _comboArchiveHost.ValueList.ValueListItems.Add("(All)");
                _comboArchiveHost.ValueList.ValueListItems.Add(5381, "(Empty)");
                counter = 1;
                if (databaseName != null)
                {
                    pairs = GetHosts(databaseName);
                    foreach (NameIdPair pair in pairs)
                    {
                        // Skip the blank one, we already have it
                        if (pair.Id == 5381)
                        {
                            continue;
                        }

                        counter++;
                        _comboArchiveHost.ValueList.ValueListItems.Add(pair.Id, pair.Name);
                        if (_archiveEventView.Filter.HostId != null && pair.Id == _archiveEventView.Filter.HostId.GetValueOrDefault(-1))
                        {
                            matchIndex = counter;
                            matchFound = true;
                        }

                    }
                }
                // We go through these in order to determine which item should be selected.
                if (_archiveEventView.Filter.HostId == null)
                {
                    _comboArchiveHost.SelectedIndex = 0;
                }
                else if (_archiveEventView.Filter.HostId == 5381)
                {
                    _comboArchiveHost.SelectedIndex = 1;
                }
                else if (!matchFound)
                {
                    SetComboNotAvailable(_comboArchiveHost, 2, _archiveEventView.Filter.HostId.GetValueOrDefault(-1));
                }
                else
                {
                    _comboArchiveHost.SelectedIndex = matchIndex;
                }

                // Logins
                matchFound = false;
                _comboArchiveLogin.ValueList.ValueListItems.Clear();
                _comboArchiveLogin.ValueList.ValueListItems.Add("(All)");
                _comboArchiveLogin.ValueList.ValueListItems.Add(5381, "(Empty)");
                _comboArchiveLogin.ValueList.ValueListItems.Add("(Privileged Users)");
                counter = 2;
                if (databaseName != null)
                {
                    pairs = GetLogins(databaseName);
                    foreach (NameIdPair pair in pairs)
                    {
                        // Skip the blank one, we already have it
                        if (pair.Id == 5381)
                        {
                            continue;
                        }

                        counter++;
                        _comboArchiveLogin.ValueList.ValueListItems.Add(pair.Id, pair.Name);
                        if (_archiveEventView.Filter.LoginId != null && pair.Id == _archiveEventView.Filter.LoginId.GetValueOrDefault(-1))
                        {
                            matchIndex = counter;
                            matchFound = true;
                        }
                    }
                }
                // We go through these in order to determine which item should be selected.
                if (_archiveEventView.Filter.ShowPrivUsersOnly)
                {
                    _comboArchiveLogin.SelectedIndex = 2;
                }
                else if (_archiveEventView.Filter.LoginId == null)
                {
                    _comboArchiveLogin.SelectedIndex = 0;
                }
                else if (_archiveEventView.Filter.LoginId == 5381)
                {
                    _comboArchiveLogin.SelectedIndex = 1;
                }
                else if (!matchFound)
                {
                    SetComboNotAvailable(_comboArchiveLogin, 3, _archiveEventView.Filter.LoginId.GetValueOrDefault(-1));
                }
                else
                {
                    _comboArchiveLogin.SelectedIndex = matchIndex;
                }

                // Tables
                matchFound = false;
                pairs = GetTables(databaseName);
                _comboArchiveTable.ValueList.ValueListItems.Clear();
                _comboArchiveTable.ValueList.ValueListItems.Add("(All)");
                _comboArchiveTable.ValueList.ValueListItems.Add(5381, "(Empty)");
                counter = 1;
                foreach (NameIdPair pair in pairs)
                {
                    // Skip the blank one, we already have it
                    if (pair.Id == 5381)
                    {
                        continue;
                    }

                    counter++;
                    _comboArchiveTable.ValueList.ValueListItems.Add(pair.Id, pair.Name);
                    if (_archiveEventView.Filter.TableId != null && pair.Id == _archiveEventView.Filter.TableId.GetValueOrDefault(-1))
                    {
                        matchIndex = counter;
                        matchFound = true;
                    }
                }
                // We go through these in order to determine which item should be selected.
                if (_archiveEventView.Filter.TableId == null)
                {
                    _comboArchiveTable.SelectedIndex = 0;
                }
                else if (_archiveEventView.Filter.TableId == 5381)
                {
                    _comboArchiveTable.SelectedIndex = 1;
                }
                else if (!matchFound)
                {
                    SetComboNotAvailable(_comboArchiveTable, 2, _archiveEventView.Filter.TableId.GetValueOrDefault(-1));
                }
                else
                {
                    _comboArchiveTable.SelectedIndex = matchIndex;
                }

                // Columns
                matchFound = false;
                pairs = GetColumns(databaseName);
                _comboArchiveColumn.ValueList.ValueListItems.Clear();
                _comboArchiveColumn.ValueList.ValueListItems.Add("(All)");
                _comboArchiveColumn.ValueList.ValueListItems.Add(5381, "(Empty)");
                counter = 1;
                foreach (NameIdPair pair in pairs)
                {
                    // Skip the blank one, we already have it
                    if (pair.Id == 5381)
                    {
                        continue;
                    }

                    counter++;
                    _comboArchiveColumn.ValueList.ValueListItems.Add(pair.Id, pair.Name);
                    if (_archiveEventView.Filter.ColumnId != null && pair.Id == _archiveEventView.Filter.ColumnId.GetValueOrDefault(-1))
                    {
                        matchIndex = counter;
                        matchFound = true;
                    }
                }
                // We go through these in order to determine which item should be selected.
                if (_archiveEventView.Filter.ColumnId == null)
                {
                    _comboArchiveColumn.SelectedIndex = 0;
                }
                else if (_archiveEventView.Filter.ColumnId == 5381)
                {
                    _comboArchiveColumn.SelectedIndex = 1;
                }
                else if (!matchFound)
                {
                    SetComboNotAvailable(_comboArchiveColumn, 2, _archiveEventView.Filter.ColumnId.GetValueOrDefault(-1));
                }
                else
                {
                    _comboArchiveColumn.SelectedIndex = matchIndex;
                }
            }
            finally
            {
                _internalUpdate--;
            }
        }

        private void LoadArchiveList()
        {
            ArchiveRecord currentArchive = GetSelectedArchive();
            bool currentFound = false;

            string server;
            if (_scope == null)
            {
                return;
            }
            else if (_scope is DatabaseRecord)
            {
                server = ((DatabaseRecord)_scope).SrvInstance;
            }
            else if (_scope is ServerRecord)
            {
                server = ((ServerRecord)_scope).Instance;
            }
            else
            {
                return;
            }

            _internalUpdate++;
            try
            {
                _currentArchives.Clear();
                _comboArchiveList.ValueList.ValueListItems.Clear();
                _currentArchives = SQLRepository.GetArchives(server);
                foreach (ArchiveRecord r in _currentArchives)
                {
                    ValueListItem item = new ValueListItem(r, r.DisplayName);
                    _comboArchiveList.ValueList.ValueListItems.Add(item);
                    if (currentArchive != null && r.DatabaseName.Equals(currentArchive.DatabaseName))
                    {
                        currentFound = true;
                        _comboArchiveList.SelectedItem = item;
                    }
                }
            }
            finally
            {
                _internalUpdate--;
            }
            // If we didn't findt he previous selection
            //  Just highlight the first item in the list
            if (!currentFound)
            {
                if (_currentArchives.Count > 0)
                {
                    _comboArchiveList.SelectedIndex = 0;
                    _btnArchiveDetach.SharedProps.Enabled = true;
                    _btnArchiveProperties.SharedProps.Enabled = true;
                }
                else
                {
                    _comboArchiveList.SelectedItem = null;
                    _btnArchiveDetach.SharedProps.Enabled = false;
                    _btnArchiveProperties.SharedProps.Enabled = false;
                }
            }
        }

        private void ShowArchiveProperties()
        {
            ArchiveRecord r = GetSelectedArchive();
            if (r != null)
            {
                Form_ArchiveProperties frm = new Form_ArchiveProperties(r);
                frm.ShowDialog();
            }
        }

        private void UpdateArchiveContextMenu()
        {
            _cmArchiveDetails.SharedProps.Enabled = _archiveEventView.IsEventSelected();
        }

        #endregion Archive Ribbon

        private void BeforeToolbarListDropdown_toolbarsManager(object sender, BeforeToolbarListDropdownEventArgs e)
        {
            e.ShowQuickAccessToolbarAddRemoveMenuItem = false;
            e.ShowQuickAccessToolbarPositionMenuItem = false;
        }

        private void EnableGroupTools(RibbonGroup group, bool enabled)
        {
            foreach (ToolBase tool in group.Tools)
            {
                tool.SharedProps.Enabled = enabled;
            }
        }
    }

    public class NameIdPair
    {
        private string _name;
        private int _id;


        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
    }

}
