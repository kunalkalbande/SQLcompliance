using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using ChartFX.WinForms;
using Idera.SQLcompliance.Application.GUI.Forms;
using Idera.SQLcompliance.Application.GUI.Helper;
using Idera.SQLcompliance.Application.GUI.SQL;
using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Agent;
using Idera.SQLcompliance.Core.Rules;
using Idera.SQLcompliance.Core.Rules.Alerts;
using Idera.SQLcompliance.Core.Rules.Filters;
using Idera.SQLcompliance.Core.Stats;
using Infragistics.Win;
using Infragistics.Win.FormattedLinkLabel;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using Qios.DevSuite.Components;
using LinkClickedEventArgs = Infragistics.Win.FormattedLinkLabel.LinkClickedEventArgs;
using Resources = Idera.SQLcompliance.Application.GUI.Properties.Resources;
using TimeZoneInfo = Idera.SQLcompliance.Core.TimeZoneHelper.TimeZoneInfo;

namespace Idera.SQLcompliance.Application.GUI.Controls
{
    public partial class ServerSummary : BaseControl
    {
        private const int TAB_ALERTS = 0;
        //start sqlcm 5.6 - 5363
        private const int TAB_LOGINS = 1;
        private const int TAB_LOGOUTS = 2;
        //end sqlcm 5.6 - 5363
        private const int TAB_FAILED_LOGINS = 3;
        private const int TAB_SECURITY = 4;
        private const int TAB_DDL = 5;
        private const int TAB_PRIVILEGED_USER = 6;
        private const int TAB_TOTAL_ACTIVITY = 7;
        private const string STATUS_NOT_AUDITED_THRESHOLD = "<span style=\"font-weight:bold;\">{0}</span> is not currently audited for this server; however, a threshold (<a href=\"{1}\">{2}</a>) has been enabled.";
        private const string STATUS_NOT_AUDITED = "<span style=\"font-weight:bold;\">{0}</span> is not currently audited for this server.";
        private const string STATUS_NO_THRESHOLD = "<span style=\"font-weight:bold;\">{0}</span> currently has no <a href=\"{1}\">threshold</a> enabled.";
        private const string STATUS_OK = "<span style=\"font-weight:bold;\">{0}</span> has been within accepted limits (<a href=\"{1}\">{2}</a>) for the past {3}.";
        private const string STATUS_WARNING = "<span style=\"font-weight:bold;\">{0}</span> has exceeded expected limits (<a href=\"{1}\">{2}</a>) within the past {3}.";
        private const string STATUS_ERROR = "<span style=\"font-weight:bold;\">{0}</span> has exceeded expected limits (<a href=\"{1}\">{2}</a>) within the past {3}.";
        private const string SERVER_STATUS = "<span style=\"font-size:_x002B_2pt_x003B_\"><a href=\"Servers\">{0}</a></span>";
        private const string AUDIT_SETTINGS = "<span style=\"font-weight:bold_x003B_\">{0}</span> - {1}<br/><br/>";
        private const string AUDIT_SETTINGS_TITLE_ONLY = "<span style=\"font-weight:bold_x003B_\">{0} </span><br/><br/>";

        private Label _lblGridStatus;
        private int _daysToShow = 1;
        private int _auditedDatabaseCount;
        private ServerRecord _scope;
        private TimeZoneInfo _tzInfo;
        private Dictionary<string, int> _databaseCountCache;

        private BackgroundWorker _bgEventLoader;

        public int DaysToShow
        {
            get { return _daysToShow; }
            set { _daysToShow = value; }
        }


        public ServerSummary()
        {
            InitializeComponent();

            _bgEventLoader = new BackgroundWorker();
            _bgEventLoader.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunWorkerCompleted_bgEventLoader);
            _bgEventLoader.DoWork += new DoWorkEventHandler(DoWork_bgEventLoader);

            _lblGridStatus = new Label();
            _lblGridStatus.Text = "No audit events within the selected span.";
            _lblGridStatus.BackColor = Color.White;
            _lblGridStatus.Visible = false;
            _lblGridStatus.AutoSize = true;
            _pnlReportCard.Controls.Add(_lblGridStatus);

            SetMenuFlag(CMMenuItem.Refresh);
            SetMenuFlag(CMMenuItem.ShowHelp);

            GridHelper.ApplyDefaultSettings(_grid);

            _databaseCountCache = new Dictionary<string, int>();
            _flblServerStatus.VisitedLinksManager = new NeverVisitedLinkManager();

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

            GridHelper.ApplyRecentActivitySummarySettings(_grid);
        }

        private void RelocateStatusLabel()
        {
            UIElement elem = _grid.DisplayLayout.Bands[0].Header.GetUIElement();
            Point pt = new Point(elem.Rect.Left + 1, elem.Rect.Bottom + 1);
            pt = _grid.PointToScreen(pt);
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
            _mainForm.DatabaseAdded += DatabaseAltered_mainForm;
            _mainForm.DatabaseRemoved += DatabaseAltered_mainForm;
            _mainForm.DatabaseModified += DatabaseAltered_mainForm;
            _mainForm.ServerModified += ServerModified_mainForm;
        }

        public override void UpdateColors()
        {
            base.UpdateColors();
            _headerAuditedActivity.BackColor = Office2007ColorTable.Colors.OutlookNavPaneCurrentGroupHeaderGradientLight;
            _headerAuditedActivity.BackColor2 = Office2007ColorTable.Colors.OutlookNavPaneCurrentGroupHeaderGradientDark;
            _headerAuditedActivity.BorderColor = Office2007ColorTable.Colors.OutlookNavPaneBorder;
            _headerEvents.BackColor = Office2007ColorTable.Colors.OutlookNavPaneCurrentGroupHeaderGradientLight;
            _headerEvents.BackColor2 = Office2007ColorTable.Colors.OutlookNavPaneCurrentGroupHeaderGradientDark;
            _headerEvents.BorderColor = Office2007ColorTable.Colors.OutlookNavPaneBorder;
            _lblReportCard.ForeColor = Office2007ColorTable.Colors.OutlookNavPaneCurrentGroupHeaderForecolor;
            _lblServerStatus.ForeColor = Office2007ColorTable.Colors.OutlookNavPaneCurrentGroupHeaderForecolor;
            _lblEvents.ForeColor = Office2007ColorTable.Colors.OutlookNavPaneCurrentGroupHeaderForecolor;
            _lblAuditedActivity.ForeColor = Office2007ColorTable.Colors.OutlookNavPaneCurrentGroupHeaderForecolor;

            BackColor = Office2007ColorTable.Colors.DockAreaGradientLight;
        }

        void ServerModified_mainForm(object sender, ServerEventArgs e)
        {
            if (_scope != null && e.Server.SrvId == _scope.SrvId)
            {
                _scope = e.Server;
                UpdateReportCard();
                UpdateAuditSettings();
            }
        }

        void DatabaseAltered_mainForm(object sender, DatabaseEventArgs e)
        {
            if (_scope != null && e.Database.SrvId == _scope.SrvId)
                UpdateAuditSettings();
        }

        public override void RefreshView()
        {
            base.RefreshView();

            try
            {
                UpdateGeneralStatus();
                UpdateAuditSettings();
                UpdateReportCard();

                _bgEventLoader.RunWorkerAsync();
            }
            catch (NotSynchronizedException e)
            {
                if (_scope == null || !_scope.Read(_scope.SrvId))
                {
                    MessageBox.Show(this, "Some servers was modified outside the application. Server information will be refreshed.", "Server Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    _mainForm.RefreshServerRecords();
                    return;
                }
            }
        }

        private void DoWork_bgEventLoader(object sender, DoWorkEventArgs e)
        {
            try
            {
                string query = BuildRecentEventQuery();
                List<EventRow> rows = GetEventRows(query);
                e.Result = rows;
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write("LoadServerSummaryPage", ex);
            }
        }

        private void RunWorkerCompleted_bgEventLoader(object sender, RunWorkerCompletedEventArgs e)
        {
            if (InvokeRequired)
            {
                object[] parameters = new object[1];
                parameters[0] = e.Result;
                Invoke(new VoidDelegateListEvents(UpdateRecentEvents), parameters);
            }
            else
            {
                UpdateRecentEvents(e.Result as List<EventRow>);
            }
        }

        public void RefreshView(ServerRecord server)
        {
            _scope = server;
            if (Settings.Default.ShowLocalTime)
                _tzInfo = TimeZoneInfo.CurrentTimeZone;
            else
                _tzInfo = _scope.GetTimeZoneInfo();
            RefreshView();
        }

        private void UpdateGeneralStatus()
        {
            // Server Status
            string opStatus, auditStatus;

            _scope.Connection = Globals.Repository.Connection;
            if (!_scope.Read(_scope.SrvId))
            {
                throw new NotSynchronizedException();
            }
            ServerStatus resultStatus = SQLRepository.GetStatus(_scope, out opStatus, out auditStatus);
            switch (resultStatus)
            {
                case ServerStatus.OK:
                    _pbStatus.Image = Resources.StatusGood_48;
                    break;
                case ServerStatus.Warning:
                    _pbStatus.Image = Resources.StatusWarning_48;
                    break;
                case ServerStatus.Alert:
                    _pbStatus.Image = Resources.StatusError_48;
                    break;
                case ServerStatus.Archive:
                    _pbStatus.Image = Resources.StatusGood_48;
                    break;
                case ServerStatus.Disabled:
                    opStatus = auditStatus;
                    _pbStatus.Image = Resources.StatusWarning_48;
                    break;
            }
            _flblServerStatus.Value = String.Format(SERVER_STATUS, opStatus);

            _scope = ServerRecord.GetServer(Globals.Repository.Connection, _scope.SrvId);

            if (_scope.TimeLastArchive > DateTime.MinValue)
            {
                DateTime modTime = _tzInfo.ToLocalTime(_scope.TimeLastArchive);
                _lblLastArchived.Text = String.Format("{0} {1}", modTime.ToShortDateString(),
                   modTime.ToShortTimeString());
            }
            else
                _lblLastArchived.Text = "Never";
            if (_scope.TimeLastHeartbeat > DateTime.MinValue)
            {
                DateTime modTime = _tzInfo.ToLocalTime(_scope.TimeLastHeartbeat);
                _lblLastHeartbeat.Text = String.Format("{0} {1}", modTime.ToShortDateString(),
                   modTime.ToShortTimeString());
            }
            else
                _lblLastHeartbeat.Text = "Never";

            ServerStatistics stats = _mainForm.GetServerStatistics(_scope.SrvId, StatsCategory.EventProcessed);
            _lblProcessedEvents.Text = EnterpriseSummary.GetCountString(stats.TotalSince(DateTime.UtcNow.AddDays(-_daysToShow)));            
            stats = _mainForm.GetServerStatistics(_scope.SrvId, StatsCategory.Alerts);
            _lblRecentAlertCount.Text = EnterpriseSummary.GetCountString(stats.TotalSince(DateTime.UtcNow.AddDays(-_daysToShow)));
        }

        private void UpdateAuditSettings()
        {
            if (_scope == null)
                return;
            List<string> stringList = new List<string>();
            string[] stringArray;
            string title;

            // Server Settings
            title = "Server";
            if (_scope.AuditLogins)
                stringList.Add("Logins");

            // SQLCM-5375 - Capture Logout Events at Server level
            if (_scope.AuditLogouts)
                stringList.Add("Logouts");

            if (_scope.AuditFailedLogins)
                stringList.Add("Failed Logins");

            if (_scope.AuditSecurity)
                stringList.Add("Security");

            if (_scope.AuditDDL)
                stringList.Add("DDL");

            if (_scope.AuditAdmin)
                stringList.Add("Admin");

            if (_scope.AuditUDE)
                stringList.Add("UDE");

            stringArray = stringList.ToArray();
            if (stringArray.Length > 0)
                _flblAuditSettings.Value = String.Format(AUDIT_SETTINGS, title, String.Join(", ", stringArray));
            else
                _flblAuditSettings.Value = String.Format(AUDIT_SETTINGS, title, "None");

            // Privileged User Settings
            stringList.Clear();
            if (_scope.AuditUsersList != null && _scope.AuditUsersList.Length > 0)
            {
                UserList users = new UserList(_scope.AuditUsersList);
                int userCount = users.Logins.Length + users.ServerRoles.Length;
                title = String.Format("Privileged Users ({0})", userCount);
                if (_scope.AuditUserLogins || _scope.AuditUserAll)
                    stringList.Add("Logins");

                // SQLCM-5375-6.1.4.3-Capture Logout Events at Server level
                if (_scope.AuditUserLogouts || _scope.AuditUserAll)
                    stringList.Add("Logouts");

                if (_scope.AuditUserFailedLogins || _scope.AuditUserAll)
                    stringList.Add("Failed Logins");

                if (_scope.AuditUserSecurity || _scope.AuditUserAll)
                    stringList.Add("Security");

                if (_scope.AuditUserDDL || _scope.AuditUserAll)
                    stringList.Add("DDL");

                if (_scope.AuditUserAdmin || _scope.AuditUserAll)
                    stringList.Add("Admin");

                if (_scope.AuditUserUDE || _scope.AuditUserAll)
                    stringList.Add("UDE");

                if (_scope.AuditUserDML || _scope.AuditUserAll)
                    stringList.Add("DML");

                if (_scope.AuditUserSELECT || _scope.AuditUserAll)
                    stringList.Add("Select");

                if (((_scope.AuditUserDML || _scope.AuditUserSELECT) && _scope.AuditUserCaptureSQL) || _scope.AuditUserAll)
                    stringList.Add("Capture SQL");

                if (_scope.AuditUserCaptureDDL)
                    stringList.Add("Capture DDL");

                if ((_scope.AuditUserDML && _scope.AuditUserCaptureTrans) || _scope.AuditUserAll)
                    stringList.Add("Capture Transactions");
            }
            else
            {
                title = "Privileged Users";
            }
            stringArray = stringList.ToArray();
            if (stringArray.Length > 0)
                _flblAuditSettings.Value += String.Format(AUDIT_SETTINGS, title, String.Join(", ", stringArray));
            else
                _flblAuditSettings.Value += String.Format(AUDIT_SETTINGS, title, "None");

            //Trusted Users - SQLSM-5373
            stringList.Clear();

            int trustedUserCount = 0;
            if (_scope.AuditTrustedUsersList != null && _scope.AuditTrustedUsersList.Length > 0)
            {
                UserList users = new UserList(_scope.AuditTrustedUsersList);
                trustedUserCount = users.Logins.Length + users.ServerRoles.Length;
                title = String.Format("Trusted Users ({0})", trustedUserCount);
            }
            else
            {
                title = "Trusted Users";
            }
            if (trustedUserCount > 0)
                _flblAuditSettings.Value += String.Format(AUDIT_SETTINGS_TITLE_ONLY, String.Format("Trusted Users ({0})", trustedUserCount));
            else
                _flblAuditSettings.Value += String.Format(AUDIT_SETTINGS, title, "None");
            //END Trusted users- SQLSM-5373
            // Database Count
            _auditedDatabaseCount = DatabaseRecord.GetAuditedDatabaseCount(Globals.Repository.Connection, _scope.SrvId);
            if (!_databaseCountCache.ContainsKey(_scope.Instance))
            {
                Thread t = new Thread(ThreadedDatabaseCounter);
                t.IsBackground = true;
                t.Start(_scope);
            }
            if (!_databaseCountCache.ContainsKey(_scope.Instance))
            {
                _flblAuditSettings.Value += String.Format(AUDIT_SETTINGS, "Databases", String.Format("{0} of N/A", _auditedDatabaseCount));
            }
            else
            {
                _flblAuditSettings.Value += String.Format(AUDIT_SETTINGS, "Databases", String.Format("{0} of {1}", _auditedDatabaseCount, _databaseCountCache[_scope.Instance]));
            }
            //         UpdateDatabaseCount();

            // Event Filters
            List<EventFilter> filters = FiltersDal.SelectEventFiltersForServer(Globals.Repository.Connection, _scope.Instance);
            Dictionary<int, int> conditions = new Dictionary<int, int>();
            List<string> conditionNames = new List<string>();
            if (filters.Count > 0)
            {
                foreach (EventFilter filter in filters)
                {
                    foreach (EventCondition condition in filter.Conditions)
                    {
                        if (!conditions.ContainsKey(condition.FieldId) &&
                            condition.FieldId != (int)AlertableEventFields.serverName)
                        {
                            conditions.Add(condition.FieldId, condition.FieldId);
                            conditionNames.Add(condition.TargetEventField.DisplayName);
                        }
                    }
                }
                conditionNames.Sort();
                string filterString = String.Join(", ", conditionNames.ToArray());
                title = String.Format("Event Filters ({0})", filters.Count);
                _flblAuditSettings.Value += String.Format(AUDIT_SETTINGS, title, filterString);
            }
            else
            {
                _flblAuditSettings.Value += String.Format(AUDIT_SETTINGS, "Event Filters", "None");
            }
        }

        private void UpdateRecentEvents(List<EventRow> rows)
        {
            _grid.BeginUpdate();
            _dsEvents.Rows.Clear();
            foreach (EventRow row in rows)
            {
                UltraDataRow ultraRow = _dsEvents.Rows.Add();
                UpdateRowValues(ultraRow, row);
            }
            _grid.EndUpdate();
            if (_dsEvents.Rows.Count > 0)
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

        private void SetStatsUnavailable()
        {
            _tabAlerts.Icon = _tabDDL.Icon = _tabFailedLogins.Icon =
               _tabActivity.Icon = _tabPrivUser.Icon = _tabSecurity.Icon = Resources.StatusError_16_ico;
            _lblAlertStatus.Text = _lblDDLStatus.Text = _lblFailedLoginStatus.Text =
               _lblExcessiveActivityStatus.Text = _lblPrivUserStatus.Text = _lblSecurityStatus.Text =
               "Error:  Statistics are not currently available.";

            ChartHelper.ShowEmptyChart(_chartActivity);
            ChartHelper.ShowEmptyChart(_chartAlerts);
            ChartHelper.ShowEmptyChart(_chartDDL);
            ChartHelper.ShowEmptyChart(_chartFailedLogins);
            ChartHelper.ShowEmptyChart(_chartPrivUser);
            ChartHelper.ShowEmptyChart(_chartSecurity);
            //start sqlcm 5.6 - 5363
            ChartHelper.ShowEmptyChart(_chartLogins);
            ChartHelper.ShowEmptyChart(_chartLogouts);
            //end sqlcm 5.6 - 5363
        }


        private void UpdateReportCard()
        {
            ServerStatistics stats;
            Chart chart = null;
            QTabButton activeButton = _tabControlReportCard.TabStripLeft.ActiveButton;

            // If the stats are not available, we just blank things out and show errors
            if (!_mainForm.StatsAvailable())
            {
                SetStatsUnavailable();
                return;
            }

            stats = _mainForm.GetServerStatistics(_scope.SrvId, StatsCategory.PrivUserEvents);
            stats.Server = _scope;
            UpdateTabStatus(stats, DateTime.UtcNow.AddDays(-_daysToShow), StatsCategory.PrivUserEvents);
            stats = _mainForm.GetServerStatistics(_scope.SrvId, StatsCategory.Alerts);
            stats.Server = _scope;
            UpdateTabStatus(stats, DateTime.UtcNow.AddDays(-_daysToShow), StatsCategory.Alerts);
            stats = _mainForm.GetServerStatistics(_scope.SrvId, StatsCategory.FailedLogin);
            stats.Server = _scope;
            UpdateTabStatus(stats, DateTime.UtcNow.AddDays(-_daysToShow), StatsCategory.FailedLogin);
            stats = _mainForm.GetServerStatistics(_scope.SrvId, StatsCategory.DDL);
            stats.Server = _scope;
            UpdateTabStatus(stats, DateTime.UtcNow.AddDays(-_daysToShow), StatsCategory.DDL);
            stats = _mainForm.GetServerStatistics(_scope.SrvId, StatsCategory.Security);
            stats.Server = _scope;
            UpdateTabStatus(stats, DateTime.UtcNow.AddDays(-_daysToShow), StatsCategory.Security);
            stats = _mainForm.GetServerStatistics(_scope.SrvId, StatsCategory.EventProcessed);
            stats.Server = _scope;
            UpdateTabStatus(stats, DateTime.UtcNow.AddDays(-_daysToShow), StatsCategory.EventProcessed);

            //start sqlcm 5.6 - 5363
            stats = _mainForm.GetServerStatistics(_scope.SrvId, StatsCategory.Logins);
            stats.Server = _scope;
            UpdateTabStatus(stats, DateTime.UtcNow.AddDays(-_daysToShow), StatsCategory.Logins);

            stats = _mainForm.GetServerStatistics(_scope.SrvId, StatsCategory.Logout);
            stats.Server = _scope;
            UpdateTabStatus(stats, DateTime.UtcNow.AddDays(-_daysToShow), StatsCategory.Logout);
            //end sqlcm 5.6 - 5363

            if (activeButton == _tabControlReportCard.TabStripLeft.TabButtons[TAB_PRIVILEGED_USER])
            {
                stats = _mainForm.GetServerStatistics(_scope.SrvId, StatsCategory.PrivUserEvents);
                chart = _chartPrivUser;
            }
            else if (activeButton == _tabControlReportCard.TabStripLeft.TabButtons[TAB_ALERTS])
            {
                stats = _mainForm.GetServerStatistics(_scope.SrvId, StatsCategory.Alerts);
                chart = _chartAlerts;
            }
            else if (activeButton == _tabControlReportCard.TabStripLeft.TabButtons[TAB_FAILED_LOGINS])
            {
                stats = _mainForm.GetServerStatistics(_scope.SrvId, StatsCategory.FailedLogin);
                chart = _chartFailedLogins;
            }
            else if (activeButton == _tabControlReportCard.TabStripLeft.TabButtons[TAB_DDL])
            {
                stats = _mainForm.GetServerStatistics(_scope.SrvId, StatsCategory.DDL);
                chart = _chartDDL;
            }
            else if (activeButton == _tabControlReportCard.TabStripLeft.TabButtons[TAB_SECURITY])
            {
                stats = _mainForm.GetServerStatistics(_scope.SrvId, StatsCategory.Security);
                chart = _chartSecurity;
            }
            else if (activeButton == _tabControlReportCard.TabStripLeft.TabButtons[TAB_TOTAL_ACTIVITY])
            {
                stats = _mainForm.GetServerStatistics(_scope.SrvId, StatsCategory.EventProcessed);
                chart = _chartActivity;
            }
            //start sqlcm 5.6- 5363
            else if (activeButton == _tabControlReportCard.TabStripLeft.TabButtons[TAB_LOGINS])
            {
                stats = _mainForm.GetServerStatistics(_scope.SrvId, StatsCategory.Logins);
                chart = _chartLogins;
            }
            else if (activeButton == _tabControlReportCard.TabStripLeft.TabButtons[TAB_LOGOUTS])
            {
                stats = _mainForm.GetServerStatistics(_scope.SrvId, StatsCategory.Logout);
                chart = _chartLogouts;
            }
            //end sqlcm 5.6 - 5363

            if (!stats.IsCategoryAudited(Globals.Repository.Connection))
            {
                ChartHelper.ShowEmptyChart(chart);
                return;
            }
            switch (_daysToShow)
            {
                case 1:
                    ShowDayChart(stats, chart);
                    break;
                case 7:
                    ShowWeekChart(stats, chart);
                    break;
                case 30:
                    ShowMonthChart(stats, chart);
                    break;
            }
        }

        private void UpdateTabStatus(ServerStatistics stats, DateTime startTime, StatsCategory statsCategory)
        {
            QTabPage page;
            UltraFormattedLinkLabel statusLabel;
            switch (statsCategory)
            {
                case StatsCategory.Alerts:
                    statusLabel = _lblAlertStatus;
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
                    statusLabel = _lblExcessiveActivityStatus;
                    page = _tabActivity;
                    break;
                //start sqlcm 5.6-5363
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

            // We have several states here:
            //  IsAudited   HasThreshold   Result
            //     Y              Y          OK, Warning, Error (based on threshold violation)
            //     Y              N          Grey Checkmark - not interesting
            //     N              Y          Error - thresholds set for unaudited properties
            //     N              N          Grey Disabled - no big deal
            //
            switch (stats.GetReportCardStatus(stats.Server.ToString(), startTime, Globals.Repository.Connection))
            {
                case ReportCardStatus.AuditedOk:
                    page.Icon = Resources.StatusGood_16_ico;
                    statusLabel.Value = String.Format(STATUS_OK, stats.Name, (int)stats.Category, stats.Threshold.GetLowestThresholdString(), span);
                    break;
                case ReportCardStatus.AuditedExceedsWarning:
                    page.Icon = Resources.StatusWarning_16_ico;
                    statusLabel.Value = String.Format(STATUS_WARNING, stats.Name, (int)stats.Category, stats.Threshold.GetWarningThresholdString(), span);
                    break;
                case ReportCardStatus.AuditedExceedsCritical:
                    page.Icon = Resources.StatusError_16_ico;
                    statusLabel.Value = String.Format(STATUS_ERROR, stats.Name, (int)stats.Category, stats.Threshold.GetCriticalThresholdString(), span);
                    break;
                case ReportCardStatus.AuditedNoThreshold:
                    page.Icon = Resources.ThresholdNotSpecified_16_ico;
                    statusLabel.Value = String.Format(STATUS_NO_THRESHOLD, stats.Name, (int)stats.Category);
                    break;
                case ReportCardStatus.NotAuditedNoThreshold:
                    page.Icon = Resources.Disabled_16;
                    statusLabel.Value = String.Format(STATUS_NOT_AUDITED, stats.Name);
                    break;
                case ReportCardStatus.NotAuditedWithThreshold:
                    page.Icon = Resources.StatusError_16_ico;
                    statusLabel.Value = String.Format(STATUS_NOT_AUDITED_THRESHOLD, stats.Name, (int)stats.Category, stats.Threshold.GetLowestThresholdString());
                    break;
            }
        }

        private void ShowMonthChart(ServerStatistics stats, Chart chart)
        {
            ChartHelper.ShowMonthChart(stats, chart);
            ChartHelper.ShowAxisSections(chart, stats.AxisRanges);
        }

        private void ShowWeekChart(ServerStatistics stats, Chart chart)
        {
            ChartHelper.ShowWeekChart(stats, chart);
            ChartHelper.ShowAxisSections(chart, stats.AxisRanges);
        }

        private void ShowDayChart(ServerStatistics stats, Chart chart)
        {
            ChartHelper.ShowDayChart(stats, chart);
            ChartHelper.ShowAxisSections(chart, stats.AxisRanges);
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

        private void LinkClicked_Threshold(object sender, LinkClickedEventArgs e)
        {
            e.OpenLink = false;
            _mainForm.ShowServerProperties(_scope, Form_ServerProperties.Context.Thresholds);
        }

        private void _tabControlReportCard_HotPageChanged(object sender, QTabPageChangeEventArgs e)
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

        private void LinkClicked_flblServerStatus(object sender, LinkClickedEventArgs e)
        {
            e.OpenLink = false;
            _mainForm.NavigateToView(ConsoleViews.AdminRegisteredServers);
        }

        private string BuildRecentEventQuery()
        {
            string whereClause;
            string orderByClause = " ORDER BY startTime DESC, eventId DESC ";
            DateTime theDate = DateTime.UtcNow.AddDays(-_daysToShow);

            whereClause = String.Format(" startTime >= {0} ", SQLHelpers.CreateSafeDateTime(theDate));

            return String.Format("SELECT TOP {0} eventCategory,eventType,startTime,loginName," +
               "databaseName,targetObject,details,eventId,spid,applicationName,hostName," +
               "serverName,success,dbUserName,objectName,targetLoginName,targetUserName,roleName," +
               "ownerName,privilegedUser,sessionLoginName" +
               " FROM {1}..{2} WHERE {3} {4}",
               100,
               SQLHelpers.CreateSafeDatabaseName(_scope.EventDatabase),
               CoreConstants.RepositoryEventsTable,
               whereClause, orderByClause);
        }

        private List<EventRow> GetEventRows(string query)
        {
            ArrayList list = new ArrayList();
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
                                int i = 0;
                                EventRow row = new EventRow();
                                row.CategoryId = SQLHelpers.GetInt32(reader, i++);
                                row.EventTypeId = SQLHelpers.GetInt32(reader, i++);
                                row.StartTime = SQLHelpers.GetDateTime(reader, i++);
                                row.LoginName = SQLHelpers.GetString(reader, i++);
                                row.DatabaseName = SQLHelpers.GetString(reader, i++);
                                row.TargetObjectName = SQLHelpers.GetString(reader, i++);
                                row.Details = SQLHelpers.GetString(reader, i++);
                                row.EventId = SQLHelpers.GetInt32(reader, i++);
                                row.Spid = SQLHelpers.GetInt32(reader, i++);
                                row.ApplicationName = SQLHelpers.GetString(reader, i++);
                                row.HostName = SQLHelpers.GetString(reader, i++);
                                row.ServerName = SQLHelpers.GetString(reader, i++);
                                row.Success = SQLHelpers.GetInt32(reader, i++) == 1 ? true : false;
                                row.DbUserName = SQLHelpers.GetString(reader, i++);
                                row.ObjectName = SQLHelpers.GetString(reader, i++);
                                row.TargetLoginName = SQLHelpers.GetString(reader, i++);
                                row.TargetUserName = SQLHelpers.GetString(reader, i++);
                                row.RoleName = SQLHelpers.GetString(reader, i++);
                                row.OwnerName = SQLHelpers.GetString(reader, i++);
                                row.PrivilegedUser = SQLHelpers.GetInt32(reader, i++) == 1 ? true : false;
                                row.SessionLoginName = SQLHelpers.GetString(reader, i++);
                                list.Add(row);
                            }
                        }
                    }
                }

                List<EventRow> retVal = new List<EventRow>();
                for (int i = 0; i < list.Count; i++)
                    retVal.Add((EventRow)list[i]);

                return retVal;
            }

            catch (Exception ex)
            {
                //_lblStatus.Text = ex.Message;
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose, "GetEventRows", ex, ErrorLog.Severity.Warning);
            }
            return null;
        }

        private void UpdateRowValues(UltraDataRow row, EventRow record)
        {
            CMEventCategory evCategory = Globals.Repository.LookupEventCategory(record.CategoryId);
            CMEventType evType = Globals.Repository.LookupEventType(record.EventTypeId);

            row["eventCategoryString"] = evCategory == null ? record.CategoryId.ToString() : evCategory.Name;
            row["eventTypeString"] = evType == null ? record.EventTypeId.ToString() : evType.Name;
            if (Settings.Default.ShowLocalTime)
                row["startTime"] = record.StartTime.ToLocalTime();
            else
                row["startTime"] = TimeZoneInfo.ToLocalTime(_tzInfo, record.StartTime);
            row["loginName"] = record.LoginName;
            row["databaseName"] = record.DatabaseName;
            row["targetObject"] = record.TargetObjectName;
            row["details"] = record.Details;
            row["spid"] = record.Spid;
            row["applicationName"] = record.ApplicationName;
            row["hostName"] = record.HostName;
            row["serverName"] = record.ServerName;
            row["success"] = record.Success;
            row["dbUserName"] = record.DbUserName;
            row["objectName"] = record.ObjectName;
            row["targetLoginName"] = record.TargetLoginName;
            row["targetUserName"] = record.TargetUserName;
            row["roleName"] = record.RoleName;
            row["ownerName"] = record.OwnerName;
            row["privilegedUser"] = record.PrivilegedUser;
            row["sessionLoginName"] = record.SessionLoginName;

            row.Tag = record;
        }

        private void ThreadedDatabaseCounter(object arg)
        {
            ServerRecord server = arg as ServerRecord;

            if (server == null)
                return;
            IList dbList = AgentHelper.LoadAllDatabases(Globals.SQLcomplianceConfig, server);
            if (dbList == null)
                return;
            _databaseCountCache[server.Instance] = dbList.Count;
            UpdateDatabaseCount();
        }

        private void UpdateDatabaseCount()
        {
            if (InvokeRequired)
            {
                Invoke(new ThreadStart(UpdateDatabaseCount));
                return;
            }
            if (_databaseCountCache.ContainsKey(_scope.Instance))
            {
                string tmp = _flblAuditSettings.Value.ToString();
                _flblAuditSettings.Value = tmp.Replace("N/A", _databaseCountCache[_scope.Instance].ToString());
                //ultraFormattedLinkLabel2.Value = tmp ;
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
                    _tabControlReportCard.ActiveTabPage = _tabActivity;
                    break;
                //start sqlcm 5.6 - 5363
                case TAB_LOGINS:
                    _tabControlReportCard.ActiveTabPage = _tabLogins;
                    break;
                case TAB_LOGOUTS:
                    _tabControlReportCard.ActiveTabPage = _tabLogouts;
                    break;
                    //end sqlcm 5.6 - 5363
            }
        }

        public override int Next()
        {
            int selectedIndex = _grid.Selected.Rows[0].Index;

            if (selectedIndex < _grid.Rows.Count - 1)
            {
                _grid.Rows[selectedIndex].Selected = false;
                _grid.Rows[selectedIndex + 1].Selected = true;
                _grid.Rows[selectedIndex + 1].Activate();
            }

            EventRow activeRow = GetActiveEvent();
            if (activeRow != null)
                return activeRow.EventId;
            else
                return -1;
        }

        public override int Previous()
        {
            int selectedIndex = _grid.Selected.Rows[0].Index;

            if (selectedIndex > 0)
            {
                _grid.Rows[selectedIndex].Selected = false;
                _grid.Rows[selectedIndex - 1].Selected = true;
                _grid.Rows[selectedIndex - 1].Activate();
            }

            EventRow activeRow = GetActiveEvent();
            if (activeRow != null)
                return activeRow.EventId;
            else
                return -1;
        }

        private EventRow GetActiveEvent()
        {
            if (_grid.Selected.Rows.Count <= 0)
                return null;
            else
            {
                UltraGridRow gridRow = _grid.Selected.Rows[0];
                UltraDataRow dataRow = gridRow.ListObject as UltraDataRow;
                return dataRow != null ? dataRow.Tag as EventRow : null;
            }
        }

        private void KeyDown_grid(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                ShowEventDetails();

        }

        public void ShowEventDetails()
        {
            EventRow theRecord = GetActiveEvent();
            if (theRecord == null)
                return;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                Form_EventProperties evProps = new Form_EventProperties(this, _scope, theRecord.EventId, _tzInfo);
                Cursor.Current = Cursors.Default;
                evProps.ShowDialog(this);
            }
            catch (Exception)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show(this, String.Format("Event {0} is not available in the database.", theRecord.EventId), "Event Warning", MessageBoxButtons.OK,
                   MessageBoxIcon.Warning);
            }
        }

        private void DoubleClickRow_grid(object sender, DoubleClickRowEventArgs e)
        {
            ShowEventDetails();
        }

        private void ServerSummary_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            HelpOnThisWindow();
            hlpevent.Handled = true;
        }

        public override void HelpOnThisWindow()
        {
            HelpAlias.ShowHelp(this, HelpAlias.SSHELP_InstanceSummary);
        }
    }
}
