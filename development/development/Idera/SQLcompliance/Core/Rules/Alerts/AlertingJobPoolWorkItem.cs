using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using BlackHen.Threading;
using Idera.SQLcompliance.Core.Cwf;
using Idera.SQLcompliance.Core.Event;
using TimeZoneInfo = Idera.SQLcompliance.Core.TimeZoneHelper.TimeZoneInfo;

namespace Idera.SQLcompliance.Core.Rules.Alerts
{
    /// <summary>
    /// Summary description for AlertingJobPoolWorkItem.
    /// </summary>
    public class AlertingJobPoolWorkItem : WorkItem
    {
        #region Member Variables

        private readonly AlertingJobInfo _jobInfo;
        private readonly ActionProcessor _actionProcessor;
        private List<PluginCommon.Alert> _dashboardAlerts;

        #endregion

        #region Properties

        #endregion

        #region Construction/Destruction

        public AlertingJobPoolWorkItem(AlertingJobInfo info)
        {
            _dashboardAlerts = new PluginCommon.Alerts();
            _jobInfo = info;
            _actionProcessor = new ActionProcessor(_jobInfo.Configuration);
        }

        #endregion

        public override void Perform()
        {
            GenerateAlerts();

            // push new alerts to CWF dashboard
            CwfHelper.Instance.PushAlertsToDashboard(_dashboardAlerts);
            _dashboardAlerts.Clear();
        }

        private void GenerateAlerts()
        {
            try
            {
                //The order here matters.  GenerateEventAlerts will update the alerting high watermark. 
                if (_jobInfo.DoBADAlertProcessing)
                    GenerateBeforeAfterAlerts();
                else
                {
                    GenerateDataAlerts();
                    GenerateEventAlerts();
                }
            }
            finally
            {
                if (_jobInfo.JobCompleteHandler != null)
                    _jobInfo.JobCompleteHandler(_jobInfo.TargetInstance);
            }
        }

        private void GenerateDataAlerts()
        {
            GenerateSensitiveColumnAlerts();
        }

        private void GenerateSensitiveColumnAlerts()
        {
            List<SensitiveColumnEvent> scEvents;
            List<DataAlert> alerts = null;
            int state = 0;

            try
            {
                // Get the events
                scEvents = AlertingDal.SelectSCAuditEvents(CoreConstants.alertingMaxEventsToProcess,
                                                          _jobInfo.AlertHighWatermark,
                                                          _jobInfo.HighWatermark,
                                                          _jobInfo.ServerDbName,
                                                          _jobInfo.ConnectionString);

                // No point in proceeding if there are no records to check.
                if (scEvents.Count <= 0)
                    return;

                ErrorLog.Instance.Write(ErrorLog.Level.Debug, String.Format("Alert: {0} sensitive column events from {1} are selected for processing.",
                                                                            scEvents.Count,
                                                                            _jobInfo.ServerDbName));
                state = 1;

                // Generate the alerts
                alerts = _jobInfo.DataProcessor.GenerateSensitiveColumnAlerts(_jobInfo.TargetInstance, scEvents, _jobInfo.ServerDbName, _jobInfo.ConnectionString);
                state = 2;

                if (alerts != null && alerts.Count > 0)
                    ErrorLog.Instance.Write(ErrorLog.Level.Verbose, String.Format("Alert: {0} data alerts generated for {1}.",
                                                                                   alerts.Count,
                                                                                   _jobInfo.TargetInstance));

                using (SqlConnection connection = new SqlConnection(_jobInfo.Configuration.ConnectionString))
                {
                    connection.Open();
                    // Store the alerts
                    foreach (DataAlert alert in alerts)
                    {
                        string s = ExpandMacros(alert.MessageTitle, alert);
                        alert.MessageTitle = s;
                        s = ExpandMacros(alert.MessageBody, alert);
                        alert.MessageBody = s;
                        AlertingDal.InsertAlert(alert, connection);

                        // add alert to dashboard alerts list
                        var dashboardAlert = new PluginCommon.Alert();
                        dashboardAlert.AlertCategory = alert.AlertType.ToString();
                        dashboardAlert.Database = string.Empty;
                        dashboardAlert.Instance = alert.Instance;
                        dashboardAlert.LastActiveTime = alert.Created;
                        dashboardAlert.Metric = string.Empty;
                        dashboardAlert.ProductId = -1;
                        dashboardAlert.Severity = alert.Level.ToString();
                        dashboardAlert.StartTime = alert.Created;
                        dashboardAlert.Summary = alert.MessageTitle;
                        dashboardAlert.Table = string.Empty;
                        dashboardAlert.Value = alert.RuleName;
                        _dashboardAlerts.Add(dashboardAlert);
                    }
                    if (alerts != null && alerts.Count > 0)
                        ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                                String.Format("Alert: {0} data alerts are inserted for {1}.",
                                                               alerts.Count, _jobInfo.TargetInstance));
                    state = 3;
                    // Prepare and store the actions
                    alerts.Sort(new DataAlertLevelDescending());
                    state = 4;
                    // Perform the actions
                    _actionProcessor.PerformActions(alerts);
                    ErrorLog.Instance.Write(ErrorLog.Level.Verbose, String.Format("Alert: alert processing for {0} finished.",
                                                                                   _jobInfo.TargetInstance));
                }

            }
            catch (Exception e)
            {
                string errorString = "Error";

                switch (state)
                {
                    case 0:
                        errorString = String.Format(CoreConstants.Exception_DataAlertingSCJobError_0, _jobInfo.ServerDbName);
                        break;
                    case 1:
                        errorString = String.Format(CoreConstants.Exception_DataAlertingSCJobError_1, _jobInfo.TargetInstance);
                        break;
                    case 2:
                        errorString = String.Format(CoreConstants.Exception_DataAlertingSCJobError_2, _jobInfo.ConnectionString);
                        break;
                    case 3:
                        errorString = String.Format(CoreConstants.Exception_DataAlertingSCJobError_3, _jobInfo.TargetInstance);
                        break;
                    case 4:
                        errorString = String.Format(CoreConstants.Exception_DataAlertingSCJobError_4, _jobInfo.TargetInstance);
                        break;
                }
                ErrorLog.Instance.Write(errorString, e);
            }
        }

        private void GenerateBeforeAfterAlerts()
        {
            List<BeforeAfterEvent> badEvents;
            List<DataAlert> alerts = null;
            int state = 0;

            try
            {
                // Get the events
                badEvents = AlertingDal.SelectBADAuditEvents(_jobInfo.EventCountForBAD,
                                                          _jobInfo.TargetInstance,
                                                          _jobInfo.ServerDbName,
                                                          _jobInfo.ConnectionString);

                // No point in proceeding if there are no records to check.
                if (badEvents.Count <= 0)
                    return;

                ErrorLog.Instance.Write(ErrorLog.Level.Debug, String.Format("Alert: {0} Before After Data events from {1} are selected for processing.",
                                                                            badEvents.Count,
                                                                            _jobInfo.ServerDbName));
                state = 1;

                // Generate the alerts
                alerts = _jobInfo.DataProcessor.GenerateBADAlerts(_jobInfo.TargetInstance, badEvents, _jobInfo.ServerDbName, _jobInfo.ConnectionString);
                state = 2;

                if (alerts != null && alerts.Count > 0)
                    ErrorLog.Instance.Write(ErrorLog.Level.Verbose, String.Format("Alert: {0} data data alerts generated for {1}.",
                                                                                   alerts.Count,
                                                                                   _jobInfo.TargetInstance));

                using (SqlConnection connection = new SqlConnection(_jobInfo.Configuration.ConnectionString))
                {
                    connection.Open();
                    // Store the alerts
                    foreach (DataAlert alert in alerts)
                    {
                        string s = ExpandMacros(alert.MessageTitle, alert);
                        alert.MessageTitle = s;
                        s = ExpandMacros(alert.MessageBody, alert);
                        alert.MessageBody = s;
                        AlertingDal.InsertAlert(alert, connection);

                        // add alert to dashboard alerts list
                        var dashboardAlert = new PluginCommon.Alert();
                        dashboardAlert.AlertCategory = alert.AlertType.ToString();
                        dashboardAlert.Database = string.Empty;
                        dashboardAlert.Instance = alert.Instance;
                        dashboardAlert.LastActiveTime = alert.Created;
                        dashboardAlert.Metric = string.Empty;
                        dashboardAlert.ProductId = -1;
                        dashboardAlert.Severity = alert.Level.ToString();
                        dashboardAlert.StartTime = alert.Created;
                        dashboardAlert.Summary = alert.MessageTitle;
                        dashboardAlert.Table = string.Empty;
                        dashboardAlert.Value = alert.RuleName;
                        _dashboardAlerts.Add(dashboardAlert);
                    }
                    if (alerts != null && alerts.Count > 0)
                        ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                                String.Format("Alert: {0} data alerts are inserted for {1}.",
                                                               alerts.Count, _jobInfo.TargetInstance));
                    state = 3;
                    // Prepare and store the actions
                    alerts.Sort(new DataAlertLevelDescending());
                    state = 4;
                    // Perform the actions
                    _actionProcessor.PerformActions(alerts);
                    ErrorLog.Instance.Write(ErrorLog.Level.Verbose, String.Format("Alert: alert processing for {0} finished.",
                                                                                   _jobInfo.TargetInstance));
                }

            }
            catch (Exception e)
            {
                string errorString = "Error";

                switch (state)
                {
                    case 0:
                        errorString = String.Format(CoreConstants.Exception_DataAlertingBADJobError_0, _jobInfo.ServerDbName);
                        break;
                    case 1:
                        errorString = String.Format(CoreConstants.Exception_DataAlertingBADJobError_1, _jobInfo.TargetInstance);
                        break;
                    case 2:
                        errorString = String.Format(CoreConstants.Exception_DataAlertingBADJobError_2, _jobInfo.ConnectionString);
                        break;
                    case 3:
                        errorString = String.Format(CoreConstants.Exception_DataAlertingBADJobError_3, _jobInfo.TargetInstance);
                        break;
                    case 4:
                        errorString = String.Format(CoreConstants.Exception_DataAlertingBADJobError_4, _jobInfo.TargetInstance);
                        break;
                }
                ErrorLog.Instance.Write(errorString, e);
            }
        }

        private void GenerateEventAlerts()
        {
            EventRecord[] records;
            List<Alert> alerts = null;
            int state = 0;

            try
            {
                // Get the events
                records = AlertingDal.SelectAuditEvents(CoreConstants.alertingMaxEventsToProcess, _jobInfo.AlertHighWatermark, _jobInfo.HighWatermark,
                  _jobInfo.ServerDbName, _jobInfo.ConnectionString);

                // No point in proceeding if there are no records to check.
                if (records.Length <= 0)
                    return;

                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                        String.Format("Alert: {0} events from {1} are selected for processing.",
                                                       records.Length, _jobInfo.ServerDbName));
                state = 1;

                // Generate the alerts
                alerts = _jobInfo.Processor.GenerateAlerts(_jobInfo.TargetInstance, records, _jobInfo.ServerDbName, _jobInfo.ConnectionString);
                state = 2;

                if (alerts != null && alerts.Count > 0)
                    ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                            String.Format("Alert: {0} alerts generated for {1}.",
                                                           alerts.Count, _jobInfo.TargetInstance));

                using (SqlConnection connection = new SqlConnection(_jobInfo.Configuration.ConnectionString))
                {
                    connection.Open();
                    // Store the alerts
                    foreach (Alert alert in alerts)
                    {
                        string s = ExpandMacros(alert.MessageTitle, alert);
                        alert.MessageTitle = s;
                        s = ExpandMacros(alert.MessageBody, alert);
                        alert.MessageBody = s;
                        AlertingDal.InsertAlert(alert, connection);

                        // add alert to dashboard alerts list
                        var dashboardAlert = new PluginCommon.Alert();
                        dashboardAlert.AlertCategory = alert.AlertType.ToString();
                        dashboardAlert.Database = string.Empty;
                        dashboardAlert.Instance = alert.Instance;
                        dashboardAlert.LastActiveTime = alert.Created;
                        dashboardAlert.Metric = string.Empty;
                        dashboardAlert.ProductId = -1;
                        dashboardAlert.Severity = alert.Level.ToString();
                        dashboardAlert.StartTime = alert.Created;
                        dashboardAlert.Summary = alert.MessageTitle;
                        dashboardAlert.Table = string.Empty;
                        dashboardAlert.Value = alert.RuleName;
                        _dashboardAlerts.Add(dashboardAlert);
                    }
                    if (alerts != null && alerts.Count > 0)
                        ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                                String.Format("Alert: {0} alerts are inserted for {1}.",
                                                               alerts.Count, _jobInfo.TargetInstance));
                    state = 3;
                    // Prepare and store the actions
                    alerts.Sort(new AlertLevelDescending());
                    state = 4;
                    // Update alerts watermark
                    int newAlertHighWatermark = records[records.Length - 1].eventId;
                    if (newAlertHighWatermark == int.MaxValue) // If EventID is rolled over
                        newAlertHighWatermark = EventId.ReadMaxEventId(_jobInfo.TargetInstance, _jobInfo.ServerDbName, _jobInfo.HighWatermark);
                    AlertingDal.UpdateAlertHighWatermark(newAlertHighWatermark, _jobInfo.TargetInstance, connection);
                    state = 5;
                    // Perform the actions
                    _actionProcessor.PerformActions(alerts);
                    ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                           String.Format("Alert: alert processing for {0} finished.",
                                                          _jobInfo.TargetInstance));
                }

            }
            catch (Exception e)
            {
                string errorString = "Error";

                switch (state)
                {
                    case 0:
                        errorString = String.Format(CoreConstants.Exception_AlertingJobError_0, _jobInfo.ServerDbName);
                        break;
                    case 1:
                        errorString = String.Format(CoreConstants.Exception_AlertingJobError_1, _jobInfo.TargetInstance);
                        break;
                    case 2:
                        errorString = String.Format(CoreConstants.Exception_AlertingJobError_2, _jobInfo.ConnectionString);
                        break;
                    case 3:
                        errorString = String.Format(CoreConstants.Exception_AlertingJobError_3, _jobInfo.TargetInstance);
                        break;
                    case 4:
                        errorString = String.Format(CoreConstants.Exception_AlertingJobError_4, _jobInfo.TargetInstance);
                        break;
                    case 5:
                        errorString = String.Format(CoreConstants.Exception_AlertingJobError_5, _jobInfo.TargetInstance);
                        break;
                }
                ErrorLog.Instance.Write(errorString, e);
            }
            finally
            {
                // Update alerts stats
                try
                {
                    if (alerts != null && alerts.Count > 0)
                        Stats.Stats.Instance.UpdateAlertCount(_jobInfo.TargetInstance, alerts.Count);
                }
                catch
                {
                }
            }
        }

        private string ExpandMacros(string source, DataAlert alert)
        {
            StringBuilder builder = new StringBuilder(source);
            builder.Replace("$AlertType$", alert.DataRuleTypeName.ToString());
            builder.Replace("$AlertTime$", alert.Created.ToString());
            builder.Replace("$AlertLevel$", alert.Level.ToString());
            builder.Replace("$EventType$", alert.EventType.ToString());
            builder.Replace("$EventId$", ((ColumnLevelEvent)alert.EventData).eventId.ToString());
            builder.Replace("$EventTime$", ((ColumnLevelEvent)alert.EventData).startTime.ToLocalTime().ToString());

            if (alert.EventType == DataRuleType.SensitiveColumn || alert.EventType == DataRuleType.SensitiveColumnViaDataset)  //SQLCM -5470 v5.6
            {
                return ExpandMacros(builder.ToString(), (SensitiveColumnEvent)alert.EventData);
            }
            else
            {
                return ExpandMacros(builder.ToString(), (BeforeAfterEvent)alert.EventData);
            }
        }

        private string ExpandMacros(string source, SensitiveColumnEvent scEvent)
        {
            StringBuilder builder = new StringBuilder(source);

            builder.Replace("$ServerName$", scEvent.instanceName);
            builder.Replace("$DatabaseName$", scEvent.dataseName);
            builder.Replace("$TableName$", scEvent.objectName);
            builder.Replace("$ColumnName$", scEvent.columnName);
            builder.Replace("$HostName$", scEvent.hostName);
            builder.Replace("$Login$", scEvent.loginName);
            return builder.ToString();
        }

        private string ExpandMacros(string source, BeforeAfterEvent badEvent)
        {
            StringBuilder builder = new StringBuilder(source);

            builder.Replace("$AfterDataValue$", badEvent.afterValue);
            builder.Replace("$BeforeDataValue$", badEvent.beforeValue);
            builder.Replace("$ServerName$", badEvent.instanceName);
            builder.Replace("$DatabaseName$", badEvent.dataseName);
            builder.Replace("$TableName$", badEvent.objectName);
            builder.Replace("$ColumnName$", badEvent.columnName);
            builder.Replace("$HostName$", badEvent.hostName);
            builder.Replace("$Login$", badEvent.loginName);
            return builder.ToString();
        }

        private string ExpandMacros(string source, Alert alert)
        {
            StringBuilder builder = new StringBuilder(source);

            //builder.Replace("$AlertId$", alert.Id.ToString()) ;
            builder.Replace("$AlertType$", alert.AlertType.ToString());
            builder.Replace("$AlertTime$", alert.Created.ToString());
            builder.Replace("$AlertLevel$", alert.Level.ToString());

            if (alert.EventData is EventRecord)
                return ExpandMacros(builder.ToString(), alert.EventData as EventRecord);
            else
                throw new Exception("Invalid EventData object");
        }

        private string ExpandMacros(string source, EventRecord record)
        {
            StringBuilder builder = new StringBuilder(source);
            CMEventType tempEvent = _jobInfo.Configuration.LookupEventType((int)record.eventType, EventType.SqlServer);
            DateTime localAlertTime = TimeZoneInfo.ToLocalTime(TimeZoneInfo.CurrentTimeZone, record.startTime);

            builder.Replace("$EventId$", record.eventId.ToString());
            builder.Replace("$EventTime$", localAlertTime.ToString());
            builder.Replace("$EventType$", (tempEvent != null) ? tempEvent.Name : record.eventType.ToString());
            builder.Replace("$ApplicationName$", record.applicationName);
            builder.Replace("$HostName$", record.hostName);
            builder.Replace("$ServerName$", record.serverName);
            builder.Replace("$Login$", record.ResponsibleLogin);
            builder.Replace("$Success$", record.success.ToString());
            builder.Replace("$DatabaseName$", record.databaseName);
            builder.Replace("$ObjectName$", record.objectName);
            builder.Replace("$PrivilegedUser$", record.privilegedUser.ToString());
            builder.Replace("$TargetLogin$", record.targetLoginName);
            builder.Replace("$TargetUser$", record.targetUserName);
            builder.Replace("$TargetObject$", record.targetObject);

            if (builder.ToString().Contains("$SQLText$"))
            {
                builder.Replace("$SQLText$", GetSQLText(record.eventId));
            }

            return builder.ToString();
        }

        private string GetSQLText(int eventId)
        {
            using (SqlConnection conn = new SqlConnection(_jobInfo.ConnectionString))
            {
                conn.Open();
                EventSqlRecord sqlRecord = new EventSqlRecord();
                try
                {
                    if (sqlRecord.Read(conn, _jobInfo.ServerDbName, eventId))
                        return sqlRecord.sqlText;
                }
                catch
                {
                }
            }
            return "";
        }
    }
}
