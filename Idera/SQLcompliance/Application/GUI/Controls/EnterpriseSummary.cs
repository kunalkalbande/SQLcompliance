using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ChartFX.WinForms;
using Idera.SQLcompliance.Application.GUI.Forms;
using Idera.SQLcompliance.Application.GUI.Helper;
using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Agent;
using Idera.SQLcompliance.Core.Rules.Alerts;
using Idera.SQLcompliance.Core.Stats;
using Infragistics.Win;
using Infragistics.Win.FormattedLinkLabel;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using Qios.DevSuite.Components;
using LinkClickedEventArgs = Infragistics.Win.FormattedLinkLabel.LinkClickedEventArgs;
using Resources = Idera.SQLcompliance.Application.GUI.Properties.Resources;

namespace Idera.SQLcompliance.Application.GUI.Controls
{
    public partial class EnterpriseSummary : BaseControl
    {
        private const int TAB_ALERTS = 0;
        private const int TAB_LOGINS = 1;
        private const int TAB_LOGOUTS = 2;
        private const int TAB_FAILED_LOGINS = 3;
        private const int TAB_SECURITY = 4;
        private const int TAB_DDL = 5;
        private const int TAB_PRIVILEGED_USER = 6;
        private const int TAB_TOTAL_ACTIVITY = 7;
        private const string STATUS_CRITICAL = "<span style=\"font-weight:bold;\">{0}</span> has exceeded expected limits on some servers within the past {1}.";
        private const string STATUS_WARNING = STATUS_CRITICAL;
        private const string STATUS_OK = "<span style=\"font-weight:bold;\">{0}</span> has been within expected limits on all servers for the past {1}.";
        private const string SYSTEM_STATUS = "<span style=\"font-size:_x002B_2pt_x003B_\"><a href=\"Servers\">{0}</a></span>";
        private const string STATUS_NOT_AUDITED_THRESHOLD = "<span style=\"font-weight:bold;\">{0}</span> is not currently audited for at least one server with an enabled threshold.";
        private const string STATUS_NOT_AUDITED = "<span style=\"font-weight:bold;\">{0}</span> is not currently audited for all servers.";
        private const string STATUS_NO_THRESHOLD = "<span style=\"font-weight:bold;\">{0}</span> currently has no threshold enabled.";

        private Label _lblGridStatus;
        private int _daysToShow = 1;
        private Dictionary<int, ReportCardRecord> _thresholds;
        private FormattedLinkEditor _linkEditorServerName;
        private FormattedLinkEditor _linkEditorThreshold;

        public int DaysToShow
        {
            get { return _daysToShow; }
            set { _daysToShow = value; }
        }

        public EnterpriseSummary()
        {
            InitializeComponent();

            _lblGridStatus = new Label();
            _lblGridStatus.Text = "No servers are currently registered";
            _lblGridStatus.BackColor = Color.White;
            _lblGridStatus.Visible = false;
            _lblGridStatus.AutoSize = true;
            _pnlReportCard.Controls.Add(_lblGridStatus);

            SetMenuFlag(CMMenuItem.Refresh);
            SetMenuFlag(CMMenuItem.ShowHelp);

            _flblSystemStatus.VisitedLinksManager = new NeverVisitedLinkManager();

            GridHelper.ApplyEnterpriseSummarySettings(_gridActivity);
            GridHelper.ApplyEnterpriseSummarySettings(_gridAlerts);
            GridHelper.ApplyEnterpriseSummarySettings(_gridDDL);
            GridHelper.ApplyEnterpriseSummarySettings(_gridFailedLogins);
            GridHelper.ApplyEnterpriseSummarySettings(_gridPrivUser);
            GridHelper.ApplyEnterpriseSummarySettings(_gridSecurity);
            //start sqlcm 5.6 - 5363
            GridHelper.ApplyEnterpriseSummarySettings(_gridLogins);
            GridHelper.ApplyEnterpriseSummarySettings(_gridLogouts);
            //end sqlcm 5.6 - 5363

            ChartHelper.InitializeChart(_chartActivity);
            ChartHelper.InitializeChart(_chartAlerts);
            ChartHelper.InitializeChart(_chartDDL);
            ChartHelper.InitializeChart(_chartFailedLogins);
            ChartHelper.InitializeChart(_chartPrivUser);
            ChartHelper.InitializeChart(_chartSecurity);
            //start sqlcm 5.6 - 5363
            ChartHelper.InitializeChart(_chartLogins);
            ChartHelper.InitializeChart(_chartLogouts);
            //end sqlcm 5.6 - 5363
        }

        private void RelocateStatusLabel()
        {
            UIElement elem = _gridActivity.DisplayLayout.Bands[0].Header.GetUIElement();
            Point pt = new Point(elem.Rect.Left + 1, elem.Rect.Bottom + 1);
            pt = _gridActivity.PointToScreen(pt);
            pt = _pnlReportCard.PointToClient(pt);
            _lblGridStatus.Location = pt;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (_lblGridStatus != null && _lblGridStatus.Visible)
                RelocateStatusLabel();
        }

        public override void Initialize(Form_Main2 mainForm)
        {
            base.Initialize(mainForm);
            mainForm.ServerAdded += new EventHandler<ServerEventArgs>(ServerAltered_mainForm);
            mainForm.ServerRemoved += new EventHandler<ServerEventArgs>(ServerAltered_mainForm);
            mainForm.ServerModified += new EventHandler<ServerEventArgs>(ServerAltered_mainForm);

            mainForm.DatabaseAdded += new EventHandler<DatabaseEventArgs>(DatabaseAltered_mainForm);
            mainForm.DatabaseRemoved += new EventHandler<DatabaseEventArgs>(DatabaseAltered_mainForm);
            mainForm.DatabaseModified += new EventHandler<DatabaseEventArgs>(DatabaseAltered_mainForm);

            InitializeGrid(_gridActivity);
            InitializeGrid(_gridAlerts);
            InitializeGrid(_gridDDL);
            InitializeGrid(_gridFailedLogins);
            InitializeGrid(_gridPrivUser);
            InitializeGrid(_gridSecurity);
            //start sqlcm 5.6 - 5363
            InitializeGrid(_gridLogins);
            InitializeGrid(_gridLogouts);
            //end sqlcm 5.6 - 5363
        }

        void ServerAltered_mainForm(object sender, ServerEventArgs e)
        {
            RefreshView();
        }

        void DatabaseAltered_mainForm(object sender, DatabaseEventArgs e)
        {
            UpdateSystemStatus();
        }

        private void InitializeGrid(UltraGrid grid)
        {
            grid.DisplayLayout.AutoFitStyle = AutoFitStyle.None;
            ResizeGrid(grid);
        }

        private void ResizeGrid(UltraGrid grid)
        {
            if (grid.DisplayLayout.Bands.Count < 1 || grid.DisplayLayout.Bands[0].Columns.Count < 4)
                return;
            grid.DisplayLayout.Bands[0].Columns[0].Width = 24;
            grid.DisplayLayout.Bands[0].Columns[1].Width = Math.Max(grid.Width - 184, 150);
            grid.DisplayLayout.Bands[0].Columns[2].Width = 80;
            grid.DisplayLayout.Bands[0].Columns[3].Width = 80;
        }

        public override void RefreshView()
        {
            base.RefreshView();
            UpdateThresholds();
            UpdateSystemStatus();
            UpdateRecentAlerts();
            UpdateReportCard();
        }

        private void UpdateThresholds()
        {
            List<ReportCardRecord> items;
            _thresholds = new Dictionary<int, ReportCardRecord>();
            items = ReportCardRecord.GetReportCardEntries(Globals.Repository.Connection);
            foreach (ReportCardRecord item in items)
                _thresholds[ThresholdIndex(item.SrvId, item.StatisticId)] = item;
        }

        private static int ThresholdIndex(int srvId, int statisticId)
        {
            return statisticId * 1000 + srvId;
        }

        private void UpdateSystemStatus()
        {
            // Server Status
            string overallStatusMsg;
            int serverCount, auditedServerCount, dbCount, eventCount;

            int overallStatusIndex = Globals.Repository.GetOverallStatus(out overallStatusMsg,
               out serverCount,
               out auditedServerCount,
               out dbCount,
               out eventCount);
            switch (overallStatusIndex)
            {
                case UIConstants.ServerImage_OK:
                    _pbSystemStatus.Image = Resources.StatusGood_48;
                    break;
                case UIConstants.ServerImage_Warning:
                    _pbSystemStatus.Image = Resources.StatusWarning_48;
                    break;
                case UIConstants.ServerImage_Alert:
                    _pbSystemStatus.Image = Resources.StatusError_48;
                    break;
            }
            _flblSystemStatus.Value = String.Format(SYSTEM_STATUS, overallStatusMsg);
            _lblRegisteredServers.Text = GetCountString(serverCount);
            _lblAuditedServers.Text = GetCountString(auditedServerCount);
            _lblAuditedDatabases.Text = GetCountString(dbCount);
            //_lblProcessedEvents.Text = GetCountString(eventCount);
            EnterpriseStatistics stats = _mainForm.GetEnterpriseStatistics(StatsCategory.EventProcessed);
            _lblProcessedEvents.Text = GetCountString(stats.TotalSince(DateTime.UtcNow.AddDays(-_daysToShow)));            
        }

        private void UpdateRecentAlerts()
        {
            Dictionary<AlertLevel, int> alertCounts;
            string whereClause = String.Format("created >= {0}",
               SQLHelpers.CreateSafeDateTime(DateTime.UtcNow.AddDays(-_daysToShow)));
            alertCounts = AlertingDal.SelectAlertLevelCount(whereClause, Globals.Repository.Connection);
            if (alertCounts != null)
            {
                if (alertCounts.ContainsKey(AlertLevel.Severe))
                    _lblSevereAlertCount.Text = GetCountString(alertCounts[AlertLevel.Severe]);
                else
                    _lblSevereAlertCount.Text = "0";
                if (alertCounts.ContainsKey(AlertLevel.High))
                    _lblHighAlertCount.Text = GetCountString(alertCounts[AlertLevel.High]);
                else
                    _lblHighAlertCount.Text = "0";
                if (alertCounts.ContainsKey(AlertLevel.Medium))
                    _lblMediumAlertCount.Text = GetCountString(alertCounts[AlertLevel.Medium]);
                else
                    _lblMediumAlertCount.Text = "0";
                if (alertCounts.ContainsKey(AlertLevel.Low))
                    _lblLowAlertCount.Text = GetCountString(alertCounts[AlertLevel.Low]);
                else
                    _lblLowAlertCount.Text = "0";
            }
            else
            {
                _lblSevereAlertCount.Text = "0";
                _lblHighAlertCount.Text = "0";
                _lblMediumAlertCount.Text = "0";
                _lblLowAlertCount.Text = "0";
            }
        }

        public static string GetCountString(int count)
        {
            if (count != 0)
                return count.ToString("#,#");
            else
                return "0";
        }

        private int GetActiveTabIndex()
        {
            QTabButton activeButton = _tabControlReportCard.TabStripLeft.ActiveButton;
            if (activeButton == _tabControlReportCard.TabStripLeft.TabButtons[TAB_PRIVILEGED_USER])
                return TAB_PRIVILEGED_USER;
            else if (activeButton == _tabControlReportCard.TabStripLeft.TabButtons[TAB_ALERTS])
                return TAB_ALERTS;
            else if (activeButton == _tabControlReportCard.TabStripLeft.TabButtons[TAB_FAILED_LOGINS])
                return TAB_FAILED_LOGINS;
            else if (activeButton == _tabControlReportCard.TabStripLeft.TabButtons[TAB_DDL])
                return TAB_DDL;
            else if (activeButton == _tabControlReportCard.TabStripLeft.TabButtons[TAB_SECURITY])
                return TAB_SECURITY;
            else if (activeButton == _tabControlReportCard.TabStripLeft.TabButtons[TAB_TOTAL_ACTIVITY])
                return TAB_TOTAL_ACTIVITY;
            //start sqlcm 5.6 - 5363
            else if (activeButton == _tabControlReportCard.TabStripLeft.TabButtons[TAB_LOGINS])
                return TAB_LOGINS;
            else if (activeButton == _tabControlReportCard.TabStripLeft.TabButtons[TAB_LOGOUTS])
                return TAB_LOGOUTS;
            //end sqlcm 5.6 - 5363
            else
                return -1;
        }

        private void SetStatsUnavailable()
        {
            _tabAlerts.Icon = _tabDDL.Icon = _tabFailedLogins.Icon =
               _tabOverallActivity.Icon = _tabPrivUser.Icon = _tabSecurity.Icon = Resources.StatusError_16_ico;
            _lblAlertsStatus.Text = _lblDDLStatus.Text = _lblFailedLoginStatus.Text =
               _lblActivityStatus.Text = _lblPrivUserStatus.Text = _lblSecurityStatus.Text =
               "Error:  Statistics are not currently available.";

            _chartActivity.DataSource = null;
            _chartAlerts.DataSource = null;
            _chartDDL.DataSource = null;
            _chartFailedLogins.DataSource = null;
            _chartPrivUser.DataSource = null;
            _chartSecurity.DataSource = null;
            //start sqlcm 5.6 - 5363
            _chartLogins.DataSource = null;
            _chartLogouts.DataSource = null;
            //end sqlcm 5.6 - 5363
            _dsActivity.Rows.Clear();
            _dsAlerts.Rows.Clear();
            _dsDDL.Rows.Clear();
            _dsFailedLogins.Rows.Clear();
            _dsPrivUsers.Rows.Clear();
            _dsSecurity.Rows.Clear();
            //start sqlcm 5.6 -5363
            _dsLogins.Rows.Clear();
            _dsLogouts.Rows.Clear();
            //end sqlcm 5.6 -5363
        }

        private void UpdateReportCard()
        {
            EnterpriseStatistics stats;
            Chart chart = null;
            UltraDataSource ds = null;
            int activeButtonIndex = GetActiveTabIndex();

            // If the stats are not available, we just blank things out and show errors
            if (!_mainForm.StatsAvailable())
            {
                SetStatsUnavailable();
                return;
            }

            stats = _mainForm.GetEnterpriseStatistics(StatsCategory.PrivUserEvents);
            UpdateTabStatus(stats, DateTime.UtcNow.AddDays(-_daysToShow));
            stats = _mainForm.GetEnterpriseStatistics(StatsCategory.Alerts);
            UpdateTabStatus(stats, DateTime.UtcNow.AddDays(-_daysToShow));
            stats = _mainForm.GetEnterpriseStatistics(StatsCategory.FailedLogin);
            UpdateTabStatus(stats, DateTime.UtcNow.AddDays(-_daysToShow));
            stats = _mainForm.GetEnterpriseStatistics(StatsCategory.DDL);
            UpdateTabStatus(stats, DateTime.UtcNow.AddDays(-_daysToShow));
            stats = _mainForm.GetEnterpriseStatistics(StatsCategory.Security);
            UpdateTabStatus(stats, DateTime.UtcNow.AddDays(-_daysToShow));
            stats = _mainForm.GetEnterpriseStatistics(StatsCategory.EventProcessed);
            UpdateTabStatus(stats, DateTime.UtcNow.AddDays(-_daysToShow));
            //start sqlcm 5.6 - 5363
            stats = _mainForm.GetEnterpriseStatistics(StatsCategory.Logins);
            UpdateTabStatus(stats, DateTime.UtcNow.AddDays(-_daysToShow));
            stats = _mainForm.GetEnterpriseStatistics(StatsCategory.Logout);
            UpdateTabStatus(stats, DateTime.UtcNow.AddDays(-_daysToShow));
            //end sqlcm 5.6 - 5363

            switch (activeButtonIndex)
            {
                case TAB_PRIVILEGED_USER:
                    stats = _mainForm.GetEnterpriseStatistics(StatsCategory.PrivUserEvents);
                    chart = _chartPrivUser;
                    ds = _dsPrivUsers;
                    break;
                case TAB_ALERTS:
                    stats = _mainForm.GetEnterpriseStatistics(StatsCategory.Alerts);
                    chart = _chartAlerts;
                    ds = _dsAlerts;
                    break;
                case TAB_FAILED_LOGINS:
                    stats = _mainForm.GetEnterpriseStatistics(StatsCategory.FailedLogin);
                    chart = _chartFailedLogins;
                    ds = _dsFailedLogins;
                    break;
                case TAB_DDL:
                    stats = _mainForm.GetEnterpriseStatistics(StatsCategory.DDL);
                    chart = _chartDDL;
                    ds = _dsDDL;
                    break;
                case TAB_SECURITY:
                    stats = _mainForm.GetEnterpriseStatistics(StatsCategory.Security);
                    chart = _chartSecurity;
                    ds = _dsSecurity;
                    break;
                case TAB_TOTAL_ACTIVITY:
                    stats = _mainForm.GetEnterpriseStatistics(StatsCategory.EventProcessed);
                    chart = _chartActivity;
                    ds = _dsActivity;
                    break;
                //start sqlcm 5.6 - 5363
                case TAB_LOGINS:
                    stats = _mainForm.GetEnterpriseStatistics(StatsCategory.Logins);
                    chart = _chartLogins;
                    ds = _dsLogins;
                    break;
                case TAB_LOGOUTS:
                    stats = _mainForm.GetEnterpriseStatistics(StatsCategory.Logout);
                    chart = _chartLogouts;
                    ds = _dsLogouts;
                    break;
                    //end sqlcm 5.6 - 5363
            }

            if (stats.GetReportCardStatus(DateTime.UtcNow.AddDays(-_daysToShow), Globals.Repository.Connection) == ReportCardStatus.NotAuditedNoThreshold)
            {
                ChartHelper.ShowEmptyChart(chart);
                UpdateGrid(stats, ds);
                return;
            }

            switch (_daysToShow)
            {
                case 1:
                    ShowDayChart(stats, chart, ds);
                    break;
                case 7:
                    ShowWeekChart(stats, chart, ds);
                    break;
                case 30:
                    ShowMonthChart(stats, chart, ds);
                    break;
            }
        }

        private void UpdateTabStatus(EnterpriseStatistics stats, DateTime startTime)
        {
            QTabPage page;
            UltraFormattedLinkLabel statusLabel;

            switch (stats.Category)
            {
                case StatsCategory.Alerts:
                    statusLabel = _lblAlertsStatus;
                    page = _tabAlerts;
                    break;
                case StatsCategory.PrivUserEvents:
                    statusLabel = _lblPrivUserStatus;
                    page = _tabPrivUser;
                    break;
                case StatsCategory.FailedLogin:
                    statusLabel = _lblFailedLoginStatus;
                    page = _tabFailedLogins;
                    break;
                case StatsCategory.DDL:
                    statusLabel = _lblDDLStatus;
                    page = _tabDDL;
                    break;
                case StatsCategory.Security:
                    statusLabel = _lblSecurityStatus;
                    page = _tabSecurity;
                    break;
                case StatsCategory.EventProcessed:
                    statusLabel = _lblActivityStatus;
                    page = _tabOverallActivity;
                    break;
                //start sqlcm 5.6 - 5363
                case StatsCategory.Logins:
                    statusLabel = _lblLoginStatus;
                    page = _tabLogins;
                    break;

                case StatsCategory.Logout:
                    statusLabel = _lblLogoutStatus;
                    page = _tabLogouts;
                    break;
                //end sqlcm 5.6 - 5363
                default:
                    return;
            }
            string span;

            if (_daysToShow == 1)
                span = "24 hours";
            else
                span = String.Format("{0} days", _daysToShow);

            switch (stats.GetReportCardStatus(startTime, Globals.Repository.Connection))
            {
                case ReportCardStatus.AuditedOk:
                    page.Icon = Resources.StatusGood_16_ico;
                    statusLabel.Value = String.Format(STATUS_OK, stats.Name, span);
                    break;
                case ReportCardStatus.AuditedExceedsWarning:
                    page.Icon = Resources.StatusWarning_16_ico;
                    statusLabel.Value = String.Format(STATUS_WARNING, stats.Name, span);
                    break;
                case ReportCardStatus.AuditedExceedsCritical:
                    page.Icon = Resources.StatusError_16_ico;
                    statusLabel.Value = String.Format(STATUS_CRITICAL, stats.Name, span);
                    break;
                case ReportCardStatus.AuditedNoThreshold:
                    page.Icon = Resources.ThresholdNotSpecified_16_ico;
                    statusLabel.Value = String.Format(STATUS_NO_THRESHOLD, stats.Name);
                    break;
                case ReportCardStatus.NotAuditedNoThreshold:
                    page.Icon = Resources.Disabled_16;
                    statusLabel.Value = String.Format(STATUS_NOT_AUDITED, stats.Name);
                    break;
                case ReportCardStatus.NotAuditedWithThreshold:
                    page.Icon = Resources.StatusError_16_ico;
                    statusLabel.Value = String.Format(STATUS_NOT_AUDITED_THRESHOLD, stats.Name);
                    break;
            }
        }

        private void ShowMonthChart(EnterpriseStatistics stats, Chart chart, UltraDataSource ds)
        {
            ChartHelper.ShowMonthChart(stats, chart);
            UpdateGrid(stats, ds);
        }

        private void ShowWeekChart(EnterpriseStatistics stats, Chart chart, UltraDataSource ds)
        {
            ChartHelper.ShowWeekChart(stats, chart);
            UpdateGrid(stats, ds);
        }

        private void ShowDayChart(EnterpriseStatistics stats, Chart chart, UltraDataSource ds)
        {
            ChartHelper.ShowDayChart(stats, chart);
            //         AxisSection section = new AxisSection(DateTime.UtcNow.AddHours(-7).ToOADate(), DateTime.UtcNow.AddHours(-6).ToOADate(), Color.Red);
            //         chart.AxisX.Sections.Add(section) ;
            //         section.Visible = true ;
            //         AxisSection section2 = new AxisSection(DateTime.UtcNow.AddHours(-18).ToOADate(), DateTime.UtcNow.AddHours(-16).ToOADate(), Color.Yellow);
            //         chart.AxisX.Sections.Add(section2);
            //         section2.Visible = true;
            UpdateGrid(stats, ds);
        }

        private void UpdateGrid(EnterpriseStatistics stats, UltraDataSource ds)
        {
            List<ServerRecord> servers;
            servers = ServerRecord.GetServers(Globals.Repository.Connection, true);
            List<object[]> noThreshold = new List<object[]>();
            List<object[]> good = new List<object[]>();
            List<object[]> warning = new List<object[]>();
            List<object[]> error = new List<object[]>();
            List<object[]> notAudited = new List<object[]>();
            DateTime startTime = DateTime.UtcNow.AddDays(-_daysToShow);

            // Servers are returned in sorted order.  We use the three lists to organize,
            //  in order by server name based on status.  This allows us to put the
            //  results in the grid sorted first by status, then server name
            ds.Rows.Clear();
            foreach (ServerRecord srv in servers)
            {
                object[] row = new object[4];

                int thresholdKey = ThresholdIndex(srv.SrvId, (int)stats.Category);
                double maxRate;
                row[1] = srv.Instance;
                ServerStatistics serverStats = stats.GetServerStatistics(srv.SrvId);
                maxRate = serverStats.MaxRate(DateTime.UtcNow.AddDays(-_daysToShow));
                if (_thresholds.ContainsKey(thresholdKey))
                {
                    ReportCardRecord record = _thresholds[thresholdKey];
                    if (record.Period == 4)
                    {
                        row[2] = String.Format("{0:F0}/{1}", maxRate, "hour");
                    }
                    else
                    {
                        row[2] = String.Format("{0:F0}/{1}", maxRate, "day");
                    }

                    row[3] = record.GetLowestThresholdString();
                }
                else
                {
                    row[2] = String.Format("{0:F0}/{1}", maxRate, "day");
                    row[3] = "None";
                }
                switch (serverStats.GetReportCardStatus(serverStats.Server.ToString(), startTime, Globals.Repository.Connection))
                {
                    case ReportCardStatus.NotAuditedWithThreshold:
                        row[0] = Resources.StatusError_16;
                        error.Add(row);
                        break;
                    case ReportCardStatus.AuditedExceedsCritical:
                        row[0] = Resources.StatusError_16;
                        error.Add(row);
                        break;
                    case ReportCardStatus.AuditedExceedsWarning:
                        row[0] = Resources.StatusWarning_16;
                        warning.Add(row);
                        break;
                    case ReportCardStatus.AuditedOk:
                        row[0] = Resources.StatusGood_16;
                        good.Add(row);
                        break;
                    case ReportCardStatus.AuditedNoThreshold:
                        row[0] = Resources.ThresholdNotSpecified_16;
                        noThreshold.Add(row);
                        break;
                    case ReportCardStatus.NotAuditedNoThreshold:
                        row[0] = Resources.DisabledGray_16;
                        notAudited.Add(row);
                        break;
                }
            }
            foreach (object[] data in error)
                ds.Rows.Add(data);
            foreach (object[] data in warning)
                ds.Rows.Add(data);
            foreach (object[] data in good)
                ds.Rows.Add(data);
            foreach (object[] data in noThreshold)
                ds.Rows.Add(data);
            foreach (object[] data in notAudited)
                ds.Rows.Add(data);

            if (ds.Rows.Count > 0)
            {
                _lblGridStatus.Visible = false;
            }
            else
            {
                RelocateStatusLabel();
                _lblGridStatus.Visible = true;
                _lblGridStatus.BringToFront();

            }
        }

        private void LinkClicked_flblSystemStatus(object sender, LinkClickedEventArgs e)
        {
            e.OpenLink = false;
            _mainForm.NavigateToView(ConsoleViews.AdminRegisteredServers);
        }

        public override void UpdateColors()
        {
            base.UpdateColors();
            _headerAlerts.BackColor = Office2007ColorTable.Colors.OutlookNavPaneCurrentGroupHeaderGradientLight;
            _headerAlerts.BackColor2 = Office2007ColorTable.Colors.OutlookNavPaneCurrentGroupHeaderGradientDark;
            _headerAlerts.BorderColor = Office2007ColorTable.Colors.OutlookNavPaneBorder;
            _lblReportCard.ForeColor = Office2007ColorTable.Colors.OutlookNavPaneCurrentGroupHeaderForecolor;
            _lblSystemStatus.ForeColor = Office2007ColorTable.Colors.OutlookNavPaneCurrentGroupHeaderForecolor;
            _lblRecentAlerts.ForeColor = Office2007ColorTable.Colors.OutlookNavPaneCurrentGroupHeaderForecolor;

            BackColor = Office2007ColorTable.Colors.DockAreaGradientLight;
            _pbHeaderGraphic.BackColor = Office2007ColorTable.Colors.DockAreaGradientDark;
        }

        private void ActivePageChanged_tabControlReportCard(object sender, QTabPageChangeEventArgs e)
        {
            if (e.FromPage != null)
                e.FromPage.ButtonBackgroundImage = null;
            if (e.ToPage != null)
            {
                e.ToPage.ButtonConfiguration.BackgroundImageAlign = QImageAlign.Stretched;
                e.ToPage.ButtonBackgroundImage = Resources.RowHotTracked;
            }
            if (_mainForm != null)
                UpdateReportCard();
        }

        private void HotPageChanged_tabControlReportCard(object sender, QTabPageChangeEventArgs e)
        {
            QTabPage active = _tabControlReportCard.ActiveTabPage;
            if (e.FromPage != null && e.FromPage != active)
                e.FromPage.ButtonBackgroundImage = null;
            if (e.ToPage != null && e.ToPage != active)
            {
                e.ToPage.ButtonConfiguration.BackgroundImageAlign = QImageAlign.Stretched;
                e.ToPage.ButtonBackgroundImage = Resources.RowSelected;
            }
        }

        public void ShowTab(int reportCardIndex)
        {
            switch (reportCardIndex)
            {
                case TAB_PRIVILEGED_USER:
                    _tabControlReportCard.ActiveTabPage = _tabPrivUser;
                    break;
                case TAB_ALERTS:
                    _tabControlReportCard.ActiveTabPage = _tabAlerts;
                    break;
                case TAB_FAILED_LOGINS:
                    _tabControlReportCard.ActiveTabPage = _tabFailedLogins;
                    break;
                case TAB_DDL:
                    _tabControlReportCard.ActiveTabPage = _tabDDL;
                    break;
                case TAB_SECURITY:
                    _tabControlReportCard.ActiveTabPage = _tabSecurity;
                    break;
                case TAB_TOTAL_ACTIVITY:
                    _tabControlReportCard.ActiveTabPage = _tabOverallActivity;
                    break;
                //start sqlcm 5.6 -5363
                case TAB_LOGINS:
                    _tabControlReportCard.ActiveTabPage = _tabLogins;
                    break;
                case TAB_LOGOUTS:
                    _tabControlReportCard.ActiveTabPage = _tabLogouts;
                    break;
                    //end sqlcm 5.6 - 5363
            }
        }

        private void InitializeLayout_grids(object sender, InitializeLayoutEventArgs e)
        {
            if (_linkEditorServerName == null)
            {
                _linkEditorServerName = new FormattedLinkEditor(false);
                _linkEditorServerName.TreatValueAs = TreatValueAs.URL;
                _linkEditorServerName.LinkClicked += LinkClicked_linkEditorServerName;
            }
            e.Layout.Bands[0].Columns["ServerName"].Editor = _linkEditorServerName;

            if (_linkEditorThreshold == null)
            {
                _linkEditorThreshold = new FormattedLinkEditor(false);
                _linkEditorThreshold.TreatValueAs = TreatValueAs.URL;
                _linkEditorThreshold.LinkClicked += LinkClicked_linkEditorThreshold;
            }
            e.Layout.Bands[0].Columns["Threshold"].Editor = _linkEditorThreshold;
        }

        void LinkClicked_linkEditorServerName(object sender, LinkClickedEventArgs e)
        {
            e.OpenLink = false;
            int activeButtonIndex = GetActiveTabIndex();

            _mainForm.NavigateToServerSummary(e.LinkText, activeButtonIndex);
        }

        void LinkClicked_linkEditorThreshold(object sender, LinkClickedEventArgs e)
        {
            e.OpenLink = false;
            int activeButtonIndex = GetActiveTabIndex();
            UltraGridRow row = null;
            switch (activeButtonIndex)
            {
                case TAB_ALERTS:
                    row = _gridAlerts.ActiveRow;
                    break;
                case TAB_DDL:
                    row = _gridDDL.ActiveRow;
                    break;
                case TAB_FAILED_LOGINS:
                    row = _gridFailedLogins.ActiveRow;
                    break;
                case TAB_PRIVILEGED_USER:
                    row = _gridPrivUser.ActiveRow;
                    break;
                case TAB_SECURITY:
                    row = _gridSecurity.ActiveRow;
                    break;
                case TAB_TOTAL_ACTIVITY:
                    row = _gridActivity.ActiveRow;
                    break;
                //start sqlcm 5.6 - 5363
                case TAB_LOGINS:
                    row = _gridLogins.ActiveRow;
                    break;
                case TAB_LOGOUTS:
                    row = _gridLogouts.ActiveRow;
                    break;
                    //end sqlcm 5.6 - 5363
            }
            if (row != null)
            {
                string serverName = row.Cells[1].Text;
                ServerRecord server = ServerRecord.GetServer(Globals.Repository.Connection, serverName);
                if (server != null)
                    _mainForm.ShowServerProperties(server, Form_ServerProperties.Context.Thresholds);
            }
        }

        private void SizeChanged_grid(object sender, EventArgs e)
        {
            if (sender is UltraGrid)
                ResizeGrid((UltraGrid)sender);
        }

        private void EnterpriseSummary_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            HelpOnThisWindow();
            hlpevent.Handled = true;
        }

        public override void HelpOnThisWindow()
        {
            HelpAlias.ShowHelp(this, HelpAlias.SSHELP_EnterpriseSummary);
        }
    }
}
