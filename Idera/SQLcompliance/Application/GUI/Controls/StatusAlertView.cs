using Idera.SQLcompliance.Application.GUI.Forms;
using Idera.SQLcompliance.Application.GUI.Helper;
using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Agent;
using Idera.SQLcompliance.Core.Rules;
using Idera.SQLcompliance.Core.Rules.Alerts;
using Infragistics.Win;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using TimeZoneInfo = Idera.SQLcompliance.Core.TimeZoneHelper.TimeZoneInfo;

namespace Idera.SQLcompliance.Application.GUI.Controls
{
    public delegate void VoidDelegateListStatusAlerts(List<StatusAlert> alerts);

    public partial class StatusAlertView : BaseControl
    {
        private AlertingConfiguration _configuration;

        private BackgroundWorker _bgAlertLoader;
        private object _bgLockObject;
        private int _bgCounter;
        private bool _reversePaging = false;
        private int _lastPage = -1;
        private int _matchingAlertCount = -1;
        private Stack _pageStarts;
        private bool _alertsTableNeedsUpdate = false;

        private AlertViewFilter _filter;


        public StatusAlertView()
        {
            _bgLockObject = new object();
            _bgAlertLoader = new BackgroundWorker();

            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            GridHelper.ApplyDefaultSettings(_grid);

            SetMenuFlag(CMMenuItem.Refresh);
            SetMenuFlag(CMMenuItem.Collapse);
            SetMenuFlag(CMMenuItem.Expand);
            SetMenuFlag(CMMenuItem.GroupByColumn);
            SetMenuFlag(CMMenuItem.ShowHelp);

            _pageStarts = new Stack();
            _filter = new AlertViewFilter();

            _bgAlertLoader.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunWorkerCompleted_bgAlertLoader);
            _bgAlertLoader.DoWork += new DoWorkEventHandler(DoWork_bgAlertLoader);
        }

        public AlertingConfiguration AlertConfiguration
        {
			set { _configuration = value ; }
			get { return _configuration ; }
        }

      public UltraGrid Grid
      {
         get { return _grid; }
      }

        public AlertViewFilter Filter
      {
         get { return _filter; }
         set { _filter = value; }
      }

        #region Actions

        public override void UpdateColors()
        {
            base.UpdateColors();
            _pnlNavigation.BackColor = Office2007ColorTable.Colors.DockAreaGradientLight;
            _pnlNavigation.BackColor2 = Office2007ColorTable.Colors.DockAreaGradientLight;
            _lblNavigation.ForeColor = Office2007ColorTable.Colors.OutlookNavPaneCurrentGroupHeaderForecolor;
        }

        protected override void OnShowGroupByChanged(ToggleChangedEventArgs e)
        {
            base.OnShowGroupByChanged(e);
            if (e.Enabled)
            {
                _grid.DisplayLayout.ViewStyleBand = ViewStyleBand.OutlookGroupBy;
            }
            else
            {
                _grid.DisplayLayout.ViewStyleBand = ViewStyleBand.Horizontal;
            }

            RelocateStatusLabel();
        }

        public void SetScope()
        {
            _filter.TargetServer = null;
        }

        public void SetScope(ServerRecord s)
        {
            _filter.TargetServer = s.Instance;
        }

        public void ShowAlertMessage()
        {
            StatusAlert alert = GetActiveAlert();
            if (alert != null)
            {
                Form_AlertMessage message = new Form_AlertMessage
                {
                    Title = alert.MessageTitle,
                    Body = alert.MessageBody
                };
                message.ShowDialog(this);
            }
        }

        private void LoadAlertPage()
        {
            PreLoadAlertPage();
        }

        private void PreLoadAlertPage()
        {
            _btnFirst.Enabled = false;
            _btnPrevious.Enabled = false;
            _btnNext.Enabled = false;
            _btnLast.Enabled = false;

            _lblNavigation.Value = "<b>Loading alerts...</b>";

            _dsAlerts.Rows.Clear();

            _bgAlertLoader.RunWorkerAsync();
        }

        private void StartWaiting()
        {
            int x = (Width - _pbWaiting.Width) / 2;
            int y = (Height - _pbWaiting.Height) / 2;

            _pbWaiting.Location = new Point(x, y);
            _pbWaiting.Visible = true;

            x = (Width - _lblWaiting.Width) / 2;
            y = _pbWaiting.Location.Y + _pbWaiting.Height;
            _lblWaiting.Location = new Point(x, y);
            _lblWaiting.Visible = true;
            SetGlobalWaitCursor(true);
        }

        private void StopWaiting()
        {
            _pbWaiting.Visible = false;
            _lblWaiting.Visible = false;
            SetGlobalWaitCursor(false);
        }

        private void DoWork_bgAlertLoader(object sender, DoWorkEventArgs e)
        {
            string query = BuildPageQuery();
            List<StatusAlert> alerts = null;
            try
            {
                alerts = AlertingDal.SelectStatusAlerts(query, _configuration.ConnectionString);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write("LoadAlertPage", ex);
            }
            e.Result = alerts;
        }

        private void RunWorkerCompleted_bgAlertLoader(object sender, RunWorkerCompletedEventArgs e)
        {
            if (InvokeRequired)
            {
                object[] parameters = new object[1];
                parameters[0] = e.Result;
                Invoke(new VoidDelegateListStatusAlerts(PostLoadAlertPage), parameters);
            }
            else
            {
                PostLoadAlertPage(e.Result as List<StatusAlert>);
            }
        }

        private void PostLoadAlertPage(List<StatusAlert> alerts)
        {
            bool forwardNav = false;
            bool backwardNav = false;
            try
            {
                _grid.BeginUpdate();
                if (alerts == null)
                {
                    // We have an error
                    _lblStatus.Visible = true;
                    _lblNavigation.Value = "<b>Error</b>";
                }
                else if (alerts.Count == 0)
                {
                    // No data matched the criteria
                    _lblStatus.Text = UIConstants.Grid_NoStatusAlerts;
                    _lblNavigation.Value = string.Format("<b>{0}</b>", UIConstants.Grid_NoStatusAlerts);
                    _lblStatus.Visible = true;
                }
                else if (alerts.Count == Settings.Default.AlertPageSize + 1)
                {
                    // We have data with more available

                    // if reverse paging, invert the order of addition
                    //  This should be in-order; however, with no sorts
                    //  this is needed to order them by id
                    if (_reversePaging)
                    {
                        for (int i = Settings.Default.AlertPageSize - 1; i >= 0; i--)
                        {
                            UltraDataRow row = _dsAlerts.Rows.Add();
                            UpdateRowValues(row, alerts[i]);
                        }
                        //_btnFirst.Enabled = true;
                        //_btnPrevious.Enabled = true;
                        backwardNav = true;
                        if (_pageStarts.Count > 0)
                        {
                            forwardNav = true;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < Settings.Default.AlertPageSize; i++)
                        {
                            UltraDataRow row = _dsAlerts.Rows.Add();
                            UpdateRowValues(row, alerts[i]);
                        }
                        //_btnLast.Enabled = true;
                        //_btnNext.Enabled = true;
                        forwardNav = true;
                        if (_pageStarts.Count > 0)
                        {
                            backwardNav = true;
                        }
                    }
                    _pageStarts.Push(alerts[Settings.Default.AlertPageSize]);
                    UpdateAlertCountString();
                }
                else
                {
                    // We are the last page.
                    if (_reversePaging)
                    {
                        for (int i = alerts.Count - 1; i >= 0; i--)
                        {
                            UltraDataRow row = _dsAlerts.Rows.Add();
                            UpdateRowValues(row, alerts[i]);
                        }
                        //_btnFirst.Enabled = false;
                        //_btnPrevious.Enabled = false;
                        if (_pageStarts.Count != 0)
                        {
                            forwardNav = true;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < alerts.Count; i++)
                        {
                            UltraDataRow row = _dsAlerts.Rows.Add();
                            UpdateRowValues(row, alerts[i]);
                        }
                        //_btnLast.Enabled = false;
                        //_btnNext.Enabled = false;
                        if (_pageStarts.Count != 0)
                        {
                            backwardNav = true;
                        }
                    }
                    // Placeholder to allow previous to continue working
                    //  correctly
                    _pageStarts.Push(null);
                    UpdateAlertCountString();
                }
                _btnFirst.Enabled = backwardNav;
                _btnPrevious.Enabled = backwardNav;
                _btnNext.Enabled = forwardNav;
                _btnLast.Enabled = forwardNav;
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write("LoadAlertPage", e);
            }
            _grid.PerformAction(UltraGridAction.FirstRowInGrid);
            _grid.EndUpdate();
        }

        private void UpdateRowValues(UltraDataRow row, StatusAlert alert)
        {
            switch (alert.Level)
            {
                case AlertLevel.Severe:
                    row["icon"] = GUI.Properties.Resources.AlertSevere_16;
                    break;
                case AlertLevel.High:
                    row["icon"] = GUI.Properties.Resources.AlertHigh_16;
                    break;
                case AlertLevel.Medium:
                    row["icon"] = GUI.Properties.Resources.AlertMedium_16;
                    break;
                case AlertLevel.Low:
                    row["icon"] = GUI.Properties.Resources.AlertLow_16;
                    break;
            }
            row["Date"] = TimeZoneInfo.ToLocalTime(TimeZoneInfo.CurrentTimeZone, alert.Created);
            row["Time"] = TimeZoneInfo.ToLocalTime(TimeZoneInfo.CurrentTimeZone, alert.Created);
            row["alertLevelString"] = alert.Level.ToString();
            row["ruleName"] = alert.RuleName;
            row["instance"] = alert.Instance;

            if (alert.ComputerName != null)
            {
                row["computerName"] = alert.ComputerName;
            }
            else
            {
                row["computerName"] = "N/A";
            }

            row["StatusRuleName"] = alert.StatusRuleTypeName;
            row.Tag = alert;
        }

        public override void RefreshView()
        {
            if (_bgAlertLoader.IsBusy)
            {
                return;
            }

            CheckVersion();
            _reversePaging = false;
            _pageStarts.Clear();
            _matchingAlertCount = -1;
            _lastPage = -1;
            _lblStatus.Visible = false;
            _pnlWarning.Visible = _alertsTableNeedsUpdate;
            RelocateStatusLabel();
            LoadAlertPage();

            Thread t = new Thread(new ThreadStart(UpdateAlertCount))
            {
                IsBackground = true
            };
            t.Start();
        }

        public StatusAlert GetActiveAlert()
        {
            if (_grid.Selected.Rows.Count <= 0)
            {
                return null;
            }
            else
            {
                UltraGridRow gridRow = _grid.Selected.Rows[0];
                UltraDataRow dataRow = gridRow.ListObject as UltraDataRow;
                return dataRow != null ? dataRow.Tag as StatusAlert : null;
            }
        }

        #endregion // Actions

        private string BuildOrderByClause()
        {
            if (!_reversePaging)
            {
                return " ORDER BY alertId DESC ";
            }
            else
            {
                return " ORDER BY alertId ASC ";
            }
        }

        private string BuildPageQuery()
        {
            string whereClause;
            string orderByClause = BuildOrderByClause();

            whereClause = _filter.GetWhereClause(EventType.Status);

            // Make sure we aren't the first page being loaded
            //  
            if (_pageStarts.Count > 0)
            {
                StatusAlert pageStart = (StatusAlert)_pageStarts.Peek();
                // Need to add paging conditions
                if (whereClause.Length > 0)
                {
                    whereClause = string.Format("alertId {0} {1} AND {2}",
                  _reversePaging ? ">=" : "<=", pageStart.Id, whereClause);
                }
                else
                {
                    whereClause = string.Format("alertId {0} {1}", _reversePaging ? ">=" : "<=", pageStart.Id);
                }
            }
            else
            {
                // No paging. fake numbers
                if (whereClause.Length > 0)
                {
                    whereClause = string.Format("alertId {0} AND {1}",
                  _reversePaging ? ">= -2100000000" : "<= 2100000000", whereClause);
                }
                else
                {
                    whereClause = string.Format("alertId {0}", _reversePaging ? ">= -2100000000" : "<= 2100000000");
                }
            }
            whereClause = string.Format("{0} AND alertType = 2", whereClause);

            return string.Format("SELECT TOP {0} a.alertId,a.alertRuleId,a.created," +
                                 "a.alertLevel,a.emailStatus,a.logStatus,a.message, " +
                                 "a.instance,a.computerName,a.eventType,t.RuleName, " +
                                 "a.ruleName as 'StatusRuleName'" +
                                 " FROM {1}..{2} a " +
                                 "JOIN {1}..StatusRuleTypes t on a.eventType = t.StatusRuleId " +
                                 "WHERE {3} {4}",
                                 Settings.Default.AlertPageSize + 1,
                                 CoreConstants.RepositoryDatabase,
                                 CoreConstants.RepositoryAlertsTable,
                                 whereClause, orderByClause);
        }

        //-------------------------------------------------------------
        // GetCount - Get total number or rows that could be returned
        //--------------------------------------------------------------
        private void UpdateAlertCount()
        {
            int myCounter;
            int count;

            lock (_bgLockObject)
            {
                myCounter = ++_bgCounter;
            }

            string whereClause = _filter.GetWhereClause(EventType.Status);

            string selectQuery = string.Format("SELECT count(e.alertId) FROM {0}..{1} AS e {2}{3}",
               CoreConstants.RepositoryDatabase,
               CoreConstants.RepositoryAlertsTable,
               (whereClause != "") ? " WHERE " : "",
               whereClause);

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
                ErrorLog.Instance.Write("Unable to load AlertCount.", e, true);
                count = -1;
            }
            lock (_bgLockObject)
            {
                if (myCounter == _bgCounter)
                {
                    _matchingAlertCount = count;
                    _lastPage = _matchingAlertCount / Settings.Default.AlertPageSize;
                    if (_matchingAlertCount % Settings.Default.AlertPageSize > 0)
                    {
                        _lastPage++;
                    }

                    if (!_bgAlertLoader.IsBusy)
                    {
                        UpdateAlertCountString();
                    }
                }
            }
        }

        private void UpdateAlertCountString()
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(UpdateAlertCountString));
                return;
            }
            if (_matchingAlertCount == 0)
            {
                _lblNavigation.Value = string.Format("<b>{0}</b>", UIConstants.Grid_NoStatusAlerts);
            }
            else if (_matchingAlertCount == -1)
            {
                _lblNavigation.Value = string.Format("<b>Page {0} of ... : ... matching alerts</b>", _reversePaging ? "..." : _pageStarts.Count.ToString());
            }
            else
            {
                _lblNavigation.Value = string.Format("<b>Page {0} of {1} : {2} matching alerts</b>", GetCurrentPage(), _lastPage, _matchingAlertCount);
            }
        }

        private int GetCurrentPage()
        {
            if (_matchingAlertCount != -1)
            {
                if (_matchingAlertCount == 0)
                {
                    return -1;
                }
                else
                {
                    int currentPage;
                    if (_reversePaging)
                    {
                        currentPage = _matchingAlertCount / Settings.Default.AlertPageSize - (_pageStarts.Count - 2);
                        if (_matchingAlertCount % Settings.Default.AlertPageSize == 0)
                        {
                            currentPage--;
                        }
                    }
                    else
                    {
                        currentPage = _pageStarts.Count;
                    }

                    return currentPage;
                }
            }
            else
            {
                if (_reversePaging)
                {
                    return -1;
                }
                else
                {
                    return _pageStarts.Count;
                }
            }
        }

        private void CheckVersion()
        {
            try
            {
                if (Globals.SQLcomplianceConfig.SqlComplianceDbSchemaVersion < CoreConstants.RepositorySqlComplianceDbSchemaVersion)
                {
                    _alertsTableNeedsUpdate = true;
                }
                else
                {
                    _alertsTableNeedsUpdate = false;
                }
            }
            catch (Exception)
            {
                // Gobble this if database is not available
                _alertsTableNeedsUpdate = false;
            }
        }
        #region Events

        private void Click_btnShowNextAlerts(object sender, EventArgs e)
        {
            if (_reversePaging)
            {
                _pageStarts.Pop();
                _pageStarts.Pop();
            }
            LoadAlertPage();
        }

        private void Click_btnPrevious(object sender, EventArgs e)
        {
            if (!_reversePaging)
            {
                _pageStarts.Pop();
                _pageStarts.Pop();
            }
            LoadAlertPage();
        }

        private void Click_btnFirst(object sender, EventArgs e)
        {
            _pageStarts.Clear();
            _reversePaging = false;
            LoadAlertPage();
        }

        private void Click_btnLast(object sender, EventArgs e)
        {
            _pageStarts.Clear();
            _reversePaging = true;
            LoadAlertPage();
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

        #endregion // Events

        public override void CollapseAll()
        {
            _grid.Rows.CollapseAll(true);
        }

        public override void ExpandAll()
        {
            _grid.Rows.ExpandAll(true);
        }

        public override void HelpOnThisWindow()
        {
            HelpAlias.ShowHelp(this, HelpAlias.SSHELP_StatusAlertView);
        }

        private void UpdateMenuFlags()
        {
            bool legalRow = _grid.Selected.Rows.Count == 1;
            //SetMenuFlag(CMMenuItem.Properties, legalRow && Globals.isAdmin && _grid.Focused);
        }

        private void FocusChanged_grid(object sender, EventArgs e)
        {
            UpdateMenuFlags();
        }

        private void LinkClicked_lnkWarning(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form_RepositoryOptions frm = new Form_RepositoryOptions(1);
            frm.ShowDialog();
            RefreshView();
        }

        private void AfterSelectChange_grid(object sender, AfterSelectChangeEventArgs e)
        {
            UpdateMenuFlags();
        }

        private void RelocateStatusLabel()
        {
            UIElement elem = _grid.DisplayLayout.Bands[0].Header.GetUIElement();
            int yOffset = elem == null ? 0 : elem.Rect.Bottom;
            if (_alertsTableNeedsUpdate)
            {
                _lblStatus.Location = new Point(_lblStatus.Location.X, yOffset + _pnlWarning.Height);
            }
            else
            {
                _lblStatus.Location = new Point(_lblStatus.Location.X, yOffset);
            }
        }

        private void DoubleClickRow_grid(object sender, DoubleClickRowEventArgs e)
        {
            ShowAlertMessage();
        }
    }
}

