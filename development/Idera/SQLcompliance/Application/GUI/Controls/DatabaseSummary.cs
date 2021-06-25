using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using ChartFX.WinForms.DataProviders;
using Idera.SQLcompliance.Application.GUI.Forms;
using Idera.SQLcompliance.Application.GUI.Helper;
using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Agent;
using Idera.SQLcompliance.Core.Rules;
using Idera.SQLcompliance.Core.Rules.Alerts;
using Idera.SQLcompliance.Core.Rules.Filters;
using Idera.SQLcompliance.Core.Event;
using Idera.SQLcompliance.Core.Stats;
using Infragistics.Win;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using Idera.SQLcompliance.Core.Templates.AuditTemplates;
using TimeZoneInfo=Idera.SQLcompliance.Core.TimeZoneHelper.TimeZoneInfo;
using System.ComponentModel;

namespace Idera.SQLcompliance.Application.GUI.Controls
{
   public partial class DatabaseSummary : BaseControl
   {
      private const string SELECT_EVENT_DISTRIBUTION = "SELECT category,SUM(count) FROM {0}..Stats WHERE dbId = {1} AND date >= {2} AND date <= {3} AND category IN (8,9,10,11,15) GROUP BY category" ;
      private const string AUDIT_SETTINGS = "<span style=\"font-weight:bold_x003B_\">{0}</span> - {1}<br/><br/>";
      private const string AUDIT_SETTINGS_TITLE_ONLY = "<span style=\"font-weight:bold_x003B_\">{0} </span><br/><br/>";

      private Label _lblGridStatus;
      private DatabaseRecord _scope = null;
      private ServerRecord _serverScope = null;
      private TimeZoneInfo _tzInfo;
      private int _daysToShow = 1 ;

        private BackgroundWorker _bgEventLoader;

        public int DaysToShow
      {
         get { return _daysToShow; }
         set { _daysToShow = value; } 
      }

      public DatabaseSummary()
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
         _pnlRecentActivity.Controls.Add(_lblGridStatus);

         SetMenuFlag(CMMenuItem.Refresh) ;
         SetMenuFlag(CMMenuItem.ShowHelp);
         GridHelper.ApplyRecentActivitySummarySettings(_grid);
      }

      private void RelocateStatusLabel()
      {
         UIElement elem = _grid.DisplayLayout.Bands[0].Header.GetUIElement();
         Point pt = new Point(elem.Rect.Left + 1, elem.Rect.Bottom + 1);
         pt = _grid.PointToScreen(pt);
         pt = _pnlRecentActivity.PointToClient(pt);
         _lblGridStatus.Location = pt;
      }

      protected override void OnSizeChanged(EventArgs e)
      {
         base.OnSizeChanged(e);
         if (_lblGridStatus != null && _lblGridStatus.Visible)
            RelocateStatusLabel();
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
         _lblDatabaseStatus.ForeColor = Office2007ColorTable.Colors.OutlookNavPaneCurrentGroupHeaderForecolor;
         _lblAuditedActivity.ForeColor = Office2007ColorTable.Colors.OutlookNavPaneCurrentGroupHeaderForecolor;
         _lblRecentActivity.ForeColor = Office2007ColorTable.Colors.OutlookNavPaneCurrentGroupHeaderForecolor;
         _lblEvents.ForeColor = Office2007ColorTable.Colors.OutlookNavPaneCurrentGroupHeaderForecolor;

         BackColor = Office2007ColorTable.Colors.DockAreaGradientLight;
      }


      public override void Initialize(Form_Main2 mainForm)
      {
         base.Initialize(mainForm);
         mainForm.DatabaseModified += DatabaseModified_mainForm;
      }

      void DatabaseModified_mainForm(object sender, DatabaseEventArgs e)
      {
         if (_scope != null && e.Database.DbId == _scope.DbId)
         {
            _scope = e.Database;
            UpdateAuditSettings();
         }
      }

      public override void RefreshView()
      {
         base.RefreshView();
         //UpdateDatabaseStatus() ;
         _scope.Connection = Globals.Repository.Connection;
         _scope.Read(_scope.DbId);

         UpdateAuditSettings() ;
         UpdateEventDistribution() ;
         UpdateRecentActivity() ;

            _bgEventLoader.RunWorkerAsync();
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
                ErrorLog.Instance.Write("LoadDatabaseSummaryPage", ex);
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

        public void RefreshView(DatabaseRecord record)
      {
         _scope = record ;
         _serverScope = ServerRecord.GetServer(Globals.Repository.Connection, _scope.SrvId);
         if (Settings.Default.ShowLocalTime)
            _tzInfo = TimeZoneInfo.CurrentTimeZone;
         else
            _tzInfo = _serverScope.GetTimeZoneInfo();
         RefreshView() ;
      }

//      private void UpdateDatabaseStatus()
//      {
//         _scope.Connection = Globals.Repository.Connection;
//         _scope.Read(_scope.DbId);
//         if (_scope.TimeEnabledModified > DateTime.MinValue)
//         {
//            DateTime modTime = _tzInfo.ToLocalTime(_scope.TimeEnabledModified);
//            _lblAuditedSince.Text = String.Format("{0} {1}", modTime.ToShortDateString(),
//               modTime.ToShortTimeString());
//         }
//         else
//            _lblAuditedSince.Text = "Never";
//         if (_scope.TimeLastModified > DateTime.MinValue)
//         {
//            DateTime modTime = _tzInfo.ToLocalTime(_scope.TimeLastModified);
//            _lblLastModified.Text = String.Format("{0} {1}", modTime.ToShortDateString(),
//               modTime.ToShortTimeString());
//         }
//         else
//            _lblLastModified.Text = "Never";
//      }

      private void UpdateAuditSettings()
      {
         if (_scope == null)
            return;

         List<string> stringList = new List<string>();
         string[] stringArray;

         if (_scope.PCI)
            stringList.Add(Regulation.RegulationType.PCI.ToString());

         if (_scope.HIPAA)
            stringList.Add(Regulation.RegulationType.HIPAA.ToString());

         if (_scope.DISA)
             stringList.Add(Regulation.RegulationType.DISA.ToString());

         if (_scope.NERC)
             stringList.Add(Regulation.RegulationType.NERC.ToString());

         if (_scope.CIS)
             stringList.Add(Regulation.RegulationType.CIS.ToString());

         if (_scope.SOX)
             stringList.Add(Regulation.RegulationType.SOX.ToString());

         if (_scope.FERPA)
             stringList.Add(Regulation.RegulationType.FERPA.ToString());

          //GDPR - SQLCM-5372
         if (_scope.GDPR)
             stringList.Add(Regulation.RegulationType.GDPR.ToString());
         
         if (stringList.Count > 0)
            _flblAuditedActivity.Value = String.Format(AUDIT_SETTINGS, "Regulation Guideline(s)", String.Join(", ", stringList.ToArray()));
         else
            _flblAuditedActivity.Value = String.Format(AUDIT_SETTINGS, "Regulation Guideline(s)", "None");

         if (_scope.AuditSecurity)
            stringList.Add("Security") ;

         if (_scope.AuditDDL)
            stringList.Add("DDL");

         if (_scope.AuditAdmin)
            stringList.Add("Admin");

         if (_scope.AuditDML)
         {
            if(_scope.AuditDmlAll)
               stringList.Add("DML");
            else
               stringList.Add("DML (filtered)") ;
         }

         if (_scope.AuditSELECT)
            stringList.Add("Select");

         if (_scope.AuditCaptureSQL)
            stringList.Add("Capture SQL");

         if (_scope.AuditCaptureDDL)
             stringList.Add("Capture DDL");

         if (_scope.AuditCaptureTrans)
            stringList.Add("Capture Transactions");

         stringArray = stringList.ToArray();
         if (stringArray.Length > 0)
            _flblAuditedActivity.Value += String.Format(AUDIT_SETTINGS, "Database", String.Join(", ", stringArray));
         else
            _flblAuditedActivity.Value += String.Format(AUDIT_SETTINGS, "Database", "None");

         // Before-After Data
         if (_scope.AuditDataChanges)
         {
            List<DataChangeTableRecord> dcTables = DataChangeTableRecord.GetAuditedTables(Globals.Repository.Connection,
               _scope.SrvId, _scope.DbId);
            _flblAuditedActivity.Value += String.Format(AUDIT_SETTINGS, "Before-After", String.Format("{0} tables", dcTables.Count));
         }
         else
         {
            _flblAuditedActivity.Value += String.Format(AUDIT_SETTINGS, "Before-After", "No tables");
         }

         if (_scope.AuditSensitiveColumns)
         {
             int scTableCount = 0;
            List<SensitiveColumnTableRecord> scTables = SensitiveColumnTableRecord.GetAuditedTables(Globals.Repository.Connection,
                                                                                                    _scope.SrvId, _scope.DbId);
            if (scTables != null && scTables.Count > 0)
            {
                HashSet<string> uniqueSCTables = new HashSet<string>();
                foreach (var item in scTables)
                {
                    if (item.Type.Equals("Individual"))
                    {
                        uniqueSCTables.Add(item.FullTableName);
                    }
                    else
                    {
                        string[] fullTablesName = item.FullTableName.Split(',');
                        foreach (var tableName in fullTablesName)
                        {
                            uniqueSCTables.Add(tableName);
                        }
                    }
                }
                scTableCount = uniqueSCTables.Count;
            }
            _flblAuditedActivity.Value += String.Format(AUDIT_SETTINGS, "Sensitive Columns", String.Format("{0} tables", scTableCount));
         }
         else
         {
            _flblAuditedActivity.Value += String.Format(AUDIT_SETTINGS, "Sensitive Columns", "No tables");
         }

         // Trusted Users
         stringList.Clear();
         int userCount = 0 ;
         if (_scope.AuditUsersList != null && _scope.AuditUsersList.Length > 0)
         {
            UserList users = new UserList(_scope.AuditUsersList);
            userCount = users.Logins.Length + users.ServerRoles.Length;
         }
         if(userCount > 0)
         {
            _flblAuditedActivity.Value += String.Format(AUDIT_SETTINGS_TITLE_ONLY, String.Format("Trusted Users ({0})", userCount));
         }
         else
         {
            _flblAuditedActivity.Value += String.Format(AUDIT_SETTINGS, "Trusted Users", "None");
         }

         // Event Filters
         List<EventFilter> filters = FiltersDal.SelectEventFiltersForServer(Globals.Repository.Connection, _serverScope.Instance);
         Dictionary<int, int> conditions = new Dictionary<int, int>();
         List<string> conditionNames = new List<string>();

         // Prune any that have database conditions that don't match us
         List<EventFilter> noMatchFilters = new List<EventFilter>();
         foreach (EventFilter filter in filters)
         {
            foreach (EventCondition condition in filter.Conditions)
            {
               if (condition.FieldId == (int)AlertableEventFields.databaseName &&
                  !condition.Matches(_scope.Name))
                  noMatchFilters.Add(filter);
            }
         }
         foreach (EventFilter filter in noMatchFilters)
            filters.Remove(filter);

         if (filters.Count > 0)
         {
            foreach (EventFilter filter in filters)
            {
               foreach (EventCondition condition in filter.Conditions)
               {
                  if (!conditions.ContainsKey(condition.FieldId) &&
                      condition.FieldId != (int)AlertableEventFields.serverName &&
                      condition.FieldId != (int)AlertableEventFields.databaseName)
                  {
                     conditions.Add(condition.FieldId, condition.FieldId);
                     conditionNames.Add(condition.TargetEventField.DisplayName);
                  }
               }
            }
            conditionNames.Sort();
            string filterString = String.Join(", ", conditionNames.ToArray());
            string title = String.Format("Event Filters ({0})", filters.Count);
            _flblAuditedActivity.Value += String.Format(AUDIT_SETTINGS, title, filterString);
         }
         else
         {
            _flblAuditedActivity.Value += String.Format(AUDIT_SETTINGS, "Event Filters", "None");
         }

      }

      private void UpdateEventDistribution()
      {
         bool hasData = false ;
         DateTime endDate = DateTime.UtcNow;
         DateTime startDate = endDate.AddDays(-_daysToShow);
         List<PieValue> pieList = new List<PieValue>();
         string query = String.Format(SELECT_EVENT_DISTRIBUTION, 
            SQLHelpers.CreateSafeDatabaseName(_serverScope.EventDatabase), 
            _scope.DbId, SQLHelpers.CreateSafeDateTime(startDate),
            SQLHelpers.CreateSafeDateTime(endDate));
         using(SqlCommand cmd = new SqlCommand(query, Globals.Repository.Connection))
         {
            using(SqlDataReader reader = cmd.ExecuteReader())
            {
               PieValue pieAdmin = new PieValue("Admin") ;
               PieValue pieDDL = new PieValue("DDL") ;
               PieValue pieDML = new PieValue("DML");
               PieValue pieSecurity = new PieValue("Security");
               PieValue pieSelect = new PieValue("Select");

               while (reader.Read())
               {
                  switch(SQLHelpers.GetInt32(reader, 0))
                  {
                     case (int)StatsCategory.Security:
                        pieSecurity.Count = SQLHelpers.GetInt32(reader, 1) ;
                        hasData = true ;
                        break ;
                     case (int)StatsCategory.DDL:
                        pieDDL.Count = SQLHelpers.GetInt32(reader, 1);
                        hasData = true;
                        break;
                     case (int)StatsCategory.Admin:
                        pieAdmin.Count = SQLHelpers.GetInt32(reader, 1);
                        hasData = true;
                        break;
                     case (int)StatsCategory.DML:
                        pieDML.Count = SQLHelpers.GetInt32(reader, 1);
                        hasData = true;
                        break;
                     case (int)StatsCategory.Select:
                        pieSelect.Count = SQLHelpers.GetInt32(reader, 1);
                        hasData = true;
                        break;
                  }
               }
               pieList.Add(pieAdmin) ;
               pieList.Add(pieDDL);
               pieList.Add(pieDML);
               pieList.Add(pieSecurity);
               pieList.Add(pieSelect);
            }
         }
         if(hasData)
            _chartEventDistribution.DataSource = new ListProvider(pieList) ;
         else
            _chartEventDistribution.Data.Clear() ;
//         if (_dsEventDistribution.Rows.Count == 0)
//         {
//            UltraDataRow row = _dsEventDistribution.Rows.Add();
//            row[0] = "No Events";
//            row[1] = 0;
//         }
      }

      private void UpdateRecentActivity()
      {
         DateTime utcEndDate = DateTime.UtcNow;
         DateTime utcStartDate = utcEndDate.AddDays(-_daysToShow);
         // ChartFx compacts on date/time boundaires from 12:00.  Because of this,
         //  we ensure that our start time is on an hour marker for the daily chart
         DateTime startDate = TimeZoneInfo.CurrentTimeZone.ToLocalTime(utcStartDate);
         if(_daysToShow == 1)
         {
            startDate = startDate.AddMinutes(-startDate.Minute);
            utcStartDate = TimeZoneInfo.CurrentTimeZone.ToUniversalTime(startDate);
         }
         else if (_daysToShow == 7)
         {
            startDate = startDate.AddMinutes(-startDate.Minute); // dump the minutes
            startDate = startDate.AddHours(-startDate.Hour % 6); // put hours at 0,6,12, or 18
            utcStartDate = TimeZoneInfo.CurrentTimeZone.ToUniversalTime(startDate);
         }
         else
         {
            startDate = startDate.Date; // we only want the day
            utcStartDate = TimeZoneInfo.CurrentTimeZone.ToUniversalTime(startDate);
         }

         DatabaseStatistics statsDDL = StatsExtractor.GetDatabaseStatistics(Globals.Repository.Connection,
            _serverScope, _scope, StatsCategory.DDL, utcStartDate, utcEndDate);
         DatabaseStatistics statsDML = StatsExtractor.GetDatabaseStatistics(Globals.Repository.Connection,
            _serverScope, _scope, StatsCategory.DML, utcStartDate, utcEndDate);
         DatabaseStatistics statsAdmin = StatsExtractor.GetDatabaseStatistics(Globals.Repository.Connection,
            _serverScope, _scope, StatsCategory.Admin, utcStartDate, utcEndDate);
         DatabaseStatistics statsSecurity = StatsExtractor.GetDatabaseStatistics(Globals.Repository.Connection,
            _serverScope, _scope, StatsCategory.Security, utcStartDate, utcEndDate);
         DatabaseStatistics statsSelect = StatsExtractor.GetDatabaseStatistics(Globals.Repository.Connection,
            _serverScope, _scope, StatsCategory.Select, utcStartDate, utcEndDate);
         StatsDataSeries seriesDDL = statsDDL.ExtractRange(utcStartDate, utcEndDate, _tzInfo);
         StatsDataSeries seriesDML = statsDML.ExtractRange(utcStartDate, utcEndDate, _tzInfo);
         StatsDataSeries seriesAdmin = statsAdmin.ExtractRange(utcStartDate, utcEndDate, _tzInfo);
         StatsDataSeries seriesSecurity = statsSecurity.ExtractRange(utcStartDate, utcEndDate, _tzInfo);
         StatsDataSeries seriesSelect = statsSelect.ExtractRange(utcStartDate, utcEndDate, _tzInfo);

         int dataPointCount = seriesDDL.Points.Count;
         ChartValue[] chartData = new ChartValue[dataPointCount];
         for(int i = 0 ; i < dataPointCount ; i++)
         {
            ChartValue c = new ChartValue() ;
            c.Date = seriesDDL.Points[i].Time;
            c.DDL = seriesDDL.Points[i].Value;
            c.DML = seriesDML.Points[i].Value;
            c.Admin = seriesAdmin.Points[i].Value;
            c.Security = seriesSecurity.Points[i].Value;
            c.Select = seriesSelect.Points[i].Value;

            chartData[i] = c ;
         }
         if(_daysToShow == 1)
            _chart1.Data.Y.CompactFormula = ChartHelper.DayCompactor;
         else if(_daysToShow == 7)
            _chart1.Data.Y.CompactFormula = ChartHelper.WeekCompactor;
         else
            _chart1.Data.Y.CompactFormula = ChartHelper.MonthCompactor;

         _chart1.DataSource = new ListProvider(chartData);
         switch(_daysToShow)
         {
            case 1:
               _chart1.Data.Compact(1.0 / 24.0);
               _chart1.AxisX.LabelsFormat.CustomFormat = "M/dd\nhh:mm tt";
               _chart1.RecalculateScale();
               _chart1.AxisX.Step = 6.0 / 24.0;
               _chart1.AxisX.MinorStep = 1.0 / 24.0;
               break;
            case 7:
               _chart1.Data.Compact(6.0 / 24.0);
               _chart1.AxisX.LabelsFormat.CustomFormat = "M/dd\nhh:mm tt";
               _chart1.RecalculateScale();
               _chart1.AxisX.Step = 42.0 / 24.0;
               _chart1.AxisX.MinorStep = 6.0 / 24.0;
               break;
            case 30:
               _chart1.Data.Compact(1);
               _chart1.AxisX.LabelsFormat.CustomFormat = "M/dd";
               _chart1.AxisX.Step = 5;
               _chart1.AxisX.MinorStep = 1;
               break;
         }
         _chart1.RecalculateScale();
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

      private string BuildRecentEventQuery()
      {
         string whereClause;
         string orderByClause = " ORDER BY startTime DESC, eventId DESC ";
         DateTime theDate = DateTime.UtcNow.AddDays(-_daysToShow);

         whereClause = String.Format(" startTime >= {0} AND databaseId = {1}", 
            SQLHelpers.CreateSafeDateTime(theDate), _scope.SqlDatabaseId);

         return String.Format("SELECT TOP {0} e.eventCategory,eventType,startTime,loginName," +
            "databaseName,targetObject,details,e.eventId,spid,applicationName,hostName," +
            "serverName,success,dbUserName,objectName,targetLoginName,targetUserName,roleName," +
            "ownerName,privilegedUser,sessionLoginName," +
            "hasSensitiveColumns = case when e.eventCategory = {1} " +
            "then (select count(sc.columnName) from {2}..{3} sc where e.eventId = sc.eventId) else (select 0) END" +
            " FROM {2}..{4} e WHERE {5} {6}",
            100,
            (int)TraceEventCategory.SELECT,
            SQLHelpers.CreateSafeDatabaseName(_serverScope.EventDatabase),
            CoreConstants.RepositorySensitiveColumnsTable,
            CoreConstants.RepositoryEventsTable,
            whereClause, 
            orderByClause);
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
                                row.HasSensitiveColumns = ((int)SQLHelpers.GetInt32(reader, i++) > 0);
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
            ErrorLog.Instance.Write("GetEventRows", ex, ErrorLog.Severity.Error);
            //_lblStatus.Text = ex.Message;
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

      private void DoubleClickRow_grid(object sender, DoubleClickRowEventArgs e)
      {
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
            Form_EventProperties evProps = new Form_EventProperties(this, _serverScope, theRecord.EventId, _tzInfo);
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

      private void DatabaseSummary_HelpRequested(object sender, HelpEventArgs hlpevent)
      {
         HelpOnThisWindow();
         hlpevent.Handled = true;
      }

      public override void HelpOnThisWindow()
      {
         HelpAlias.ShowHelp(this, HelpAlias.SSHELP_DatabaseSummary);
      }
      
//      private void LinkClicked_lnkEditAuditSettings(object sender, LinkClickedEventArgs e)
//      {
//         e.OpenLink = false;
//         _mainForm.ShowDatabaseProperties(_scope, Form_DatabaseProperties.Context.AuditedActivities);
//      }
  }

   public class PieValue
   {
      private int _count = 0 ;
      private string _name ;

      public PieValue(string name)
      {
         _name = name ;
      }

      public int Count
      {
         get { return _count; }
         set { _count = value; }
      }

      public string Name
      {
         get { return _name; }
         set { _name = value; }
      }
   }

   public class ChartValue
   {
      private double _ddl ;
      private double _dml;
      private double _security;
      private double _admin;
      private double _select;
      private DateTime _date ;


      public DateTime Date
      {
         get { return _date; }
         set { _date = value; }
      }

      public double Admin
      {
         get { return _admin; }
         set { _admin = value; }
      }

      public double DDL
      {
         get { return _ddl; }
         set { _ddl = value; }
      }

      public double DML
      {
         get { return _dml; }
         set { _dml = value; }
      }

      public double Security
      {
         get { return _security; }
         set { _security = value; }
      }

      public double Select
      {
         get { return _select; }
         set { _select = value; }
      }
   }
}
