using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using Idera.SQLcompliance.Core.Agent;
using Idera.SQLcompliance.Core.Rules.Alerts;
using Idera.SQLcompliance.Core.TimeZoneHelper;
using System.Web.Mail;
using Idera.SQLcompliance.Core.Event;

using System.Web.Mail;
using Idera.SQLcompliance.Core.Rules;
using System.Collections;
using System.Linq;

namespace Idera.SQLcompliance.Core.Stats
{

    public class StatsExtractor
    {
        //      private const string SELECT_sumOverRange = "SELECT SUM(count) FROM {0}..Stats WHERE category = {1} and date >= {2} and date <= {3} and lastUpdated >= {4}";
        private const string UNIONSELECT_serverOnlyRange = "SELECT date, count, {0} as srvId FROM {1}..Stats WHERE category = {2} and date >= {3} and date <= {4} and lastUpdated >= {5}";
        private const string SELECT_databaseStats = "SELECT date, count FROM {0}..Stats WHERE dbId = {1} and category = {2} and date >= {3} and date <= {4} ORDER by date";


        //
        // This function creates a new EnterpriseStatistics class and gets the data from the database.
        //
        public static EnterpriseStatistics GetEnterpriseStatistics(SqlConnection conn, StatsCategory category, List<ServerRecord> servers, DateTime startTime, DateTime endTime)
        {
            EnterpriseStatistics retVal;

            retVal = new EnterpriseStatistics(category, servers, ReportCardRecord.GetStatisticReportCardEntries(conn, (int)category));
            UpdateEnterpriseStatistics(conn, retVal, servers, startTime, endTime);
            return retVal;
        }

        //
        // This function takes an existing statistics class and updates the data in that class
        //
        public static void UpdateEnterpriseStatistics(SqlConnection conn, EnterpriseStatistics stats,
           List<ServerRecord> servers, DateTime startTime, DateTime endTime)
        {
            string query = "";
            for (int i = 0; i < servers.Count; i++)
            {
                if (i != 0)
                    query += " UNION ";
                query += String.Format(UNIONSELECT_serverOnlyRange, servers[i].SrvId,
                   SQLHelpers.CreateSafeDatabaseName(servers[i].EventDatabase), (int)stats.Category,
                   SQLHelpers.CreateSafeDateTime(startTime),
                   SQLHelpers.CreateSafeDateTime(endTime),
                   SQLHelpers.CreateSafeDateTime(stats.LastUpdated));
            }
            stats.LastUpdated = DateTime.UtcNow;
            if (query.Length > 0)
            {
                query += " ORDER by date";
                using (SqlDataReader reader = StatsDAL.ExecuteReader(conn, query))
                {
                    while (reader.Read())
                    {
                        StatsDataPoint pt;
                        int srvId;
                        pt = new StatsDataPoint();
                        pt.Time = SQLHelpers.GetDateTime(reader, 0);
                        pt.Value = SQLHelpers.GetInt32(reader, 1);
                        srvId = SQLHelpers.GetInt32(reader, 2);
                        stats.UpdateDataPoint(srvId, pt);
                    }
                }
            }
        }



        public static DatabaseStatistics GetDatabaseStatistics(SqlConnection conn, ServerRecord server,
         DatabaseRecord database, StatsCategory category, DateTime startTime, DateTime endTime)
        {
            DatabaseStatistics retVal = new DatabaseStatistics(server, database);
            string query = String.Format(SELECT_databaseStats,
               SQLHelpers.CreateSafeDatabaseName(server.EventDatabase),
               database.DbId, (int)category,
               SQLHelpers.CreateSafeDateTime(startTime),
               SQLHelpers.CreateSafeDateTime(endTime));
            using (SqlDataReader reader = StatsDAL.ExecuteReader(conn, query))
            {
                while (reader.Read())
                {
                    StatsDataPoint pt;
                    pt = new StatsDataPoint();
                    pt.Time = SQLHelpers.GetDateTime(reader, 0);
                    pt.Value = SQLHelpers.GetInt32(reader, 1);
                    retVal.Add(pt);
                }
            }

            return retVal;
        }


        //      public static int GetCurrentActivityRate(ServerRecord srv, StatsCategory category, int period)
        //      {
        //         DateTime endDate = DateTime.Now ;
        //         DateTime startDate = endDate.AddMinutes(-15 * period) ;
        //         string connString = StatsDAL.GetConnectionString();
        //         
        //         using (SqlConnection conn = new SqlConnection(connString))
        //         {
        //            string query = String.Format(SELECT_sumOverRange, srv.EventDatabase, (int)category,
        //               SQLHelpers.CreateSafeDateTime(startDate),
        //               SQLHelpers.CreateSafeDateTime(endDate)) ;
        //               
        //            conn.Open() ;
        //            using(SqlCommand cmd = new SqlCommand(query, conn))
        //            {
        //               int retVal = (int)cmd.ExecuteScalar() ;
        //               return retVal ;
        //            }
        //         }
        //      }
    }

    public class EnterpriseStatistics : StatsDataSeries
    {
        private Dictionary<int, ServerStatistics> _serverStats;
        private DateTime _lastUpdated;


        public EnterpriseStatistics(StatsCategory category, List<ServerRecord> servers, List<ReportCardRecord> reportCards)
        {
            Category = category;
            _lastUpdated = DateTime.UtcNow.AddDays(-40);
            _serverStats = new Dictionary<int, ServerStatistics>();
            foreach (ServerRecord server in servers)
                _serverStats[server.SrvId] = new ServerStatistics(server, category);
            foreach (ReportCardRecord reportCard in reportCards)
            {
                if (_serverStats.ContainsKey(reportCard.SrvId))
                    _serverStats[reportCard.SrvId].Threshold = reportCard;
            }
        }

        public new string Name
        {
            set
            {
                base.Name = value;
                foreach (ServerStatistics stats in _serverStats.Values)
                    stats.Name = value;
            }
            get { return base.Name; }
        }

        public DateTime LastUpdated
        {
            get { return _lastUpdated; }
            set { _lastUpdated = value; }
        }

        public ServerStatistics GetServerStatistics(int srvId)
        {
            if (_serverStats.ContainsKey(srvId))
                return _serverStats[srvId];
            else
                return null;
        }

        public void UpdateDataPoint(int srvId, StatsDataPoint pt)
        {
            StatsDataPoint diff = _serverStats[srvId].UpdateDataPoint(pt);
            // These points get mutated in their different lists, so we need to clone
            Increment(diff.Clone());
        }

        public ReportCardStatus GetReportCardStatus(DateTime startTime, SqlConnection conn)
        {
            ReportCardStatus retVal = ReportCardStatus.NotAuditedNoThreshold;

            foreach (ServerStatistics stats in _serverStats.Values)
            {
                ReportCardStatus serverStatus = stats.GetReportCardStatus(stats.Server.ToString(), startTime, conn);
                if (serverStatus < retVal)
                    retVal = serverStatus;
                // At the highest spot, no need to continue
                if (retVal == ReportCardStatus.NotAuditedWithThreshold)
                    return retVal;
            }
            return retVal;
        }

        public bool ExceedsErrorThreshold(String server, DateTime startTime)
        {
            foreach (ServerStatistics s in _serverStats.Values)
            {
                if (s.ExceedsCriticalThreshold(server, startTime))
                    return true;
            }
            return false;
        }

        public bool ExceedsWarningThreshold(DateTime startTime)
        {
            foreach (ServerStatistics s in _serverStats.Values)
            {
                if (s.ExceedsWarningThreshold(s.Server.ToString(), startTime))
                    return true;
            }
            return false;
        }
    }

    public class ServerStatistics : StatsDataSeries
    {
        private readonly AlertingJobInfo _jobInfo;
        private SNMPConfiguration _snmpConfiguration;
        private string snmpServerAddress = "";
        private string snmpCommunity = "";
        private ReportCardRecord _threshold = null;
        private ServerRecord _server;
        private StatsDataSeries _rates;
        private List<AxisRange> _thresholdRanges;
        private StatsCategory Category = 0;

        public ServerStatistics(ServerRecord server, StatsCategory category)
        {
            _thresholdRanges = new List<AxisRange>();
            _server = server;
            Category = category;
            _threshold = new ReportCardRecord(_server.SrvId, category);
        }

        public ServerRecord Server
        {
            get { return _server; }
            set { _server = value; }
        }

        public ReportCardRecord Threshold
        {
            get { return _threshold; }
            set
            {
                _threshold = value;
                // threshold changes mean we might need new rate data
                //  We certainly need new violation ranges
                BuildRateData();
            }
        }

        public StatsDataSeries Rates
        {
            get
            {
                if (_rates == null)
                {
                    BuildRateData();
                }
                return _rates;
            }
        }

        public List<AxisRange> AxisRanges
        {
            get
            {
                if (_rates == null)
                {
                    BuildRateData();
                }
                return _thresholdRanges;
            }
        }

        public StatsDataPoint UpdateDataPoint(StatsDataPoint pt)
        {
            _rates = null;
            return Add(pt);
        }

        public bool IsCategoryAudited(SqlConnection conn)
        {
            UserList users = new UserList(_server.AuditUsersList);
            bool privUser = (users.Logins.Length > 0 || users.ServerRoles.Length > 0);
            if (!_server.IsEnabled || !_server.IsDeployed ||
               _server.TimeLastAgentContact == DateTime.MinValue)
                return false;
            else if (privUser && _server.AuditUserAll)
                return true;

            switch (Category)
            {
                case StatsCategory.Alerts:
                    List<AlertRule> alertRules = AlertingDal.SelectAlertRulesForServer(conn, _server.Instance);
                    if (alertRules.Count > 0)
                        return true;
                    else
                        return false;
                case StatsCategory.PrivUserEvents:
                    return privUser;
                case StatsCategory.FailedLogin:
                    return _server.AuditFailedLogins || (privUser && _server.AuditUserFailedLogins);
                //start sqlcm 5.6-5363
                case StatsCategory.Logins:
                    return _server.AuditLogins || (privUser && _server.AuditUserLogins);
                case StatsCategory.Logout:
                    return _server.AuditLogouts || (privUser && _server.AuditUserLogouts);
                //end sqlcm 5.6 -5363
                case StatsCategory.DDL:
                    return _server.AuditDDL || (privUser && _server.AuditUserDDL);
                case StatsCategory.Security:
                    return _server.AuditSecurity || (privUser && _server.AuditUserSecurity);
                case StatsCategory.EventProcessed:
                    return true;
            }
            return false;
        }

        public ReportCardStatus GetReportCardStatus(String server, DateTime startTime, SqlConnection conn)
        {
            if (!IsCategoryAudited(conn))
            {
                if (HasThresholds())
                    return ReportCardStatus.NotAuditedWithThreshold;
                else
                    return ReportCardStatus.NotAuditedNoThreshold;
            }
            else if (!HasThresholds())
            {
                return ReportCardStatus.AuditedNoThreshold;
            }
            else if (ExceedsCriticalThreshold(server, startTime))
            {
                return ReportCardStatus.AuditedExceedsCritical;
            }
            else if (ExceedsWarningThreshold(server, startTime))
            {
                return ReportCardStatus.AuditedExceedsWarning;
            }
            else
            {
                return ReportCardStatus.AuditedOk;
            }
        }


        private void BuildRateData()
        {
            _rates = ConvertSeriesToRate(_threshold.Period);
            BuildRanges();
            //DumpRateData(String.Format("c:\\temp\\RateData_{0}_{1}.csv", _server.Instance.Replace("\\", ""), Category));
        }

        private void DumpRateData(string filename)
        {

            // DEBUG code
            StreamWriter writer = new StreamWriter(filename);
            writer.WriteLine("Raw Data");
            writer.WriteLine("Time,Value");
            foreach (StatsDataPoint pt in Points)
            {
                writer.WriteLine("{0},{1}", pt.Time, pt.Value);
            }

            writer.WriteLine();
            writer.WriteLine();
            writer.WriteLine("Rates");
            writer.WriteLine("Time,Value");
            foreach (StatsDataPoint pt in _rates.Points)
            {
                writer.WriteLine("{0},{1}", pt.Time, pt.Value);
            }

            writer.WriteLine();
            writer.WriteLine();
            writer.WriteLine("Threshold Ranges");
            writer.WriteLine("Start, Stop, Range");
            foreach (AxisRange range in _thresholdRanges)
            {
                writer.WriteLine("{0},{1},{2}", range.StartTime, range.EndTime, range.Level);
            }
            writer.Close();
        }

        private void BuildRanges()
        {
            _thresholdRanges.Clear();
            if (!_threshold.Enabled)
                return;

            DateTime startTime = DateTime.MinValue;
            int level = 0;

            foreach (StatsDataPoint pt in _rates.Points)
            {
                int ptLevel = 0;

                if (pt.Value > _threshold.CriticalThreshold)
                    ptLevel = 2;
                else if (pt.Value > _threshold.WarningThreshold)
                    ptLevel = 1;

                if (startTime == DateTime.MinValue && ptLevel > 0)
                {
                    startTime = pt.Time;
                    level = ptLevel;
                }
                else if (ptLevel != level)
                {
                    // Need to possibly close current range and open another
                    // Close current range
                    if (level != 0)
                        _thresholdRanges.Add(new AxisRange(startTime, pt.Time, level));

                    if (ptLevel > 0)
                    {
                        startTime = pt.Time;
                        level = ptLevel;
                    }
                    else
                    {
                        startTime = DateTime.MinValue;
                        level = 0;
                    }
                }
            }
            // Finally, close the last section if we are in one.
            if (level != 0)
            {
                DateTime lastTime = _rates.Points[_rates.Points.Count - 1].Time;
                _thresholdRanges.Add(new AxisRange(startTime, lastTime, level));
            }
        }

        public bool HasThresholds()
        {
            return _threshold.Enabled;
        }

        public bool ExceedsCriticalThreshold(String server, DateTime startTime)
        {
            String thresholdType = "Critical";
            if (!_threshold.Enabled)
                return false;
            foreach (StatsDataPoint pt in Rates.Points)
            {
                if (pt.Time >= startTime && pt.Value > _threshold.CriticalThreshold)
                {
                    performThresholdConfiguration(server, thresholdType);
                    return true;
                }
            }
            return false;
        }

        public bool ExceedsWarningThreshold(String server, DateTime startTime)
        {
            String thresholdType = "Warning";
            if (!_threshold.Enabled)
                return false;
            foreach (StatsDataPoint pt in Rates.Points)
            {
                if (pt.Time >= startTime && pt.Value > _threshold.WarningThreshold)
                {
                    performThresholdConfiguration(server, thresholdType);
                    return true;
                }
            }
            return false;
        }

        //
        public static void PerformThresholdMailActions(String thresholdType, String msgBody, String Subject, SQLcomplianceConfiguration sqlcomplianceConfig)
        {
            MailMessage msg;
            msg = new MailMessage();

            msg.BodyFormat = MailFormat.Text;
            msg.From = sqlcomplianceConfig.smtpSenderAddress;
            msg.Subject = Subject != null && Subject != "" ? Subject : thresholdType + " Threshold";
            msg.Body = msgBody;
            msg.To = String.Join(";", sqlcomplianceConfig.smtpRecieverAddress);

            msg.Fields["http://schemas.microsoft.com/cdo/configuration/smtsperver"] = sqlcomplianceConfig.smtpServer;
            msg.Fields["http://schemas.microsoft.com/cdo/configuration/smtpserverport"] = sqlcomplianceConfig.smtpPort;
            msg.Fields["http://schemas.microsoft.com/cdo/configuration/sendusing"] = 2;
            msg.Fields["http://schemas.microsoft.com/cdo/configuration/smtpusessl"] = sqlcomplianceConfig.smtpSsl.ToString();
            msg.Fields["http://schemas.microsoft.com/cdo/configuration/smtpauthenticate"] = sqlcomplianceConfig.smtpAuthType;
            // if (smtpAuthType == SmtpAuthProtocol.Basic)
            {
                msg.Fields["http://schemas.microsoft.com/cdo/configuration/sendusername"] = sqlcomplianceConfig.smtpUsername;
                msg.Fields["http://schemas.microsoft.com/cdo/configuration/sendpassword"] = sqlcomplianceConfig.smtpPassword;
            }

            try
            {
                SmtpMail.SmtpServer = sqlcomplianceConfig.smtpServer;
                SmtpMail.Send(msg);

                Logger.Initialize(msgBody, 5, 1000000);
                if (msgBody != null)
                {
                    Logger.Info(msgBody);
                }
            }
            catch (Exception e)
            {
                string errorString;
                while (e != null)
                {
                    errorString = e.Message;
                    e = e.InnerException;
                    Logger.Initialize(sqlcomplianceConfig.Server, 5, 1000000);
                    Logger.Info("Email notification failed for :" + msg.Subject);
                }
            }
        }

        public void PerformThresholdMail(String server, String thresholdType, String message, SQLcomplianceConfiguration sqlcomplianceConfig)
        {
            String statsCategory = String.Empty, msgBody = String.Empty, msgTitle = String.Empty; ;
            switch (Category)
            {
                case StatsCategory.Alerts:
                    statsCategory = "Alerts";
                    break;
                case StatsCategory.PrivUserEvents:
                    statsCategory = "Privileged User Events";
                    break;
                case StatsCategory.FailedLogin:
                    statsCategory = "Failed Logins";
                    break;
                case StatsCategory.DDL:
                    statsCategory = "DDL";
                    break;
                case StatsCategory.Security:
                    statsCategory = "Security";
                    break;
                case StatsCategory.EventProcessed:
                    statsCategory = "Event Processed";
                    break;
                //start sqlcm 5.6 - 5363
                case StatsCategory.Logins:
                    statsCategory = "Logins";
                    break;
                case StatsCategory.Logout:
                    statsCategory = "Logouts";
                    break;
                    //end sqlcm 5.6 - 5363
            }

            Repository rep = null;
            rep = new Repository();
            rep.OpenConnection();
            try
            {
                Hashtable map = KeyValueParser.ParseString(message);
                msgTitle = (string)map["title"];
                msgBody = (string)map["body"];
            }
            catch
            {
            }
            if (msgBody == null || msgBody == "")
            {
                msgBody = statsCategory + " threshold event occurred on " + server + " instance at " + this.MaxTime;
            }
            else
            {
                msgBody = msgBody.Replace("$AlertTypeName$", statsCategory);
                msgBody = msgBody.Replace("$ServerName$", server);
                msgBody = msgBody.Replace("$AlertTime$", this.MaxTime.ToString());
                msgBody = msgBody.Replace("$AlertLevel$", thresholdType);
            }
            if (msgTitle != null && msgTitle != "")
            {
                msgTitle = msgTitle.Replace("$AlertTypeName$", statsCategory);
                msgTitle = msgTitle.Replace("$ServerName$", server);
                msgTitle = msgTitle.Replace("$AlertTime$", this.MaxTime.ToString());
                msgTitle = msgTitle.Replace("$AlertLevel$", thresholdType);
            }

            String query = "Select smtpServer,smtpPort,smtpAuthType,smtpSsl,smtpUsername,smtpPassword,smtpSenderAddress from Configuration";
            using (SqlCommand command = new SqlCommand(query, rep.connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        sqlcomplianceConfig.smtpServer = SQLHelpers.GetString(reader, 0);
                        sqlcomplianceConfig.smtpPort = SQLHelpers.GetInt32(reader, 1);
                        sqlcomplianceConfig.smtpAuthType = SQLHelpers.GetInt32(reader, 2);
                        sqlcomplianceConfig.smtpSsl = SQLHelpers.ByteToBool(reader, 3);
                        sqlcomplianceConfig.smtpUsername = SQLHelpers.GetString(reader, 4);
                        sqlcomplianceConfig.smtpPassword = SQLHelpers.GetString(reader, 5);
                        sqlcomplianceConfig.smtpSenderAddress = SQLHelpers.GetString(reader, 6);
                    }
                }
            }
            AlertingConfiguration alertingConfig = new AlertingConfiguration();
            PerformThresholdMailActions(thresholdType, msgBody, msgTitle, sqlcomplianceConfig);

        }

        public void performThresholdConfiguration(String server, String thresholdType)
        {

            String instanceName = String.Empty, senderEmail = String.Empty, snmpAddress = String.Empty, snmpCommunity = String.Empty;
            String message = String.Empty;
            bool send_mail_permission = false, snmp_permission = false, logs_permission = false;
            int srvId, snmpPort, severity = -1;
            SQLcomplianceConfiguration sqlcomplianceConfig = new SQLcomplianceConfiguration();

            Repository rep = null;
            rep = new Repository();
            rep.OpenConnection();
            sqlcomplianceConfig.Read(rep.connection);

            String query = String.Format("Select * from ThresholdConfiguration where instance_name = {0}", SQLHelpers.CreateSafeString(server));
            using (SqlCommand command = new SqlCommand(query, rep.connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int col = 0;
                        instanceName = SQLHelpers.GetString(reader, col++);
                        sqlcomplianceConfig.smtpRecieverAddress = SQLHelpers.GetString(reader, col++);
                        send_mail_permission = SQLHelpers.ByteToBool(reader, col++);
                        snmp_permission = SQLHelpers.ByteToBool(reader, col++);
                        logs_permission = SQLHelpers.ByteToBool(reader, col++);
                        srvId = SQLHelpers.GetInt32(reader, col++);
                        snmpAddress = SQLHelpers.GetString(reader, col++);
                        snmpPort = SQLHelpers.GetInt32(reader, col++);
                        snmpCommunity = SQLHelpers.GetString(reader, col++);
                        severity = SQLHelpers.GetInt32(reader, col++);
                        message = SQLHelpers.GetString(reader, col);

                    }
                }
            }
            if (send_mail_permission)
            {
                if ((severity == 0 && thresholdType.Equals("Warning")) || (severity == 1 && thresholdType.Equals("Critical") || (severity == 2)))
                {
                    PerformThresholdMail(server, thresholdType, message, sqlcomplianceConfig);
                }
            }
            if (logs_permission)
            {
                Logger.Initialize(instanceName, 5, 1000000);
                Logger.Info(mailMsgBody());
            }
            if (snmp_permission)
            {
                performSNMPThresholdAction();

            }
        }

        public void performSNMPThresholdAction()
        {
            SQLcomplianceConfiguration sqlLcomplianceConfig = new SQLcomplianceConfiguration();
            Repository rep = null;
            rep = new Repository();
            rep.OpenConnection();
            sqlLcomplianceConfig.Read(rep.connection);
            int snmpPort = 0;
            String query = "Select snmpServerAddress,snmpPort,snmpCommunity from Configuration";
            using (SqlCommand command = new SqlCommand(query, rep.connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int col = 0;
                        snmpServerAddress = SQLHelpers.GetString(reader, col++);
                        snmpPort = SQLHelpers.GetInt32(reader, col++);
                        snmpCommunity = SQLHelpers.GetString(reader, col++);
                    }
                }
            }
            _snmpConfiguration = new SNMPConfiguration();
            _snmpConfiguration.ReceiverAddress = snmpServerAddress;
            _snmpConfiguration.ReceiverPort = snmpPort;
            _snmpConfiguration.Community = snmpCommunity;

            String error = "";
            bool result = ActionProcessor.PerformSnmpTrapTest(_snmpConfiguration, out error);
            if (result)
            {
                SNMPHelper.SendSnmpTrap(null, _snmpConfiguration, true);
            }
        }

        public string mailMsgBody()
        {
            String statsCategory = "";
            switch (Category)
            {
                case StatsCategory.Alerts:
                    statsCategory = "Alerts";
                    break;
                case StatsCategory.PrivUserEvents:
                    statsCategory = "Privileged User Events";
                    break;
                case StatsCategory.FailedLogin:
                    statsCategory = "Failed Logins";
                    break;
                case StatsCategory.DDL:
                    statsCategory = "DDL";
                    break;
                case StatsCategory.Security:
                    statsCategory = "Security";
                    break;
                case StatsCategory.EventProcessed:
                    statsCategory = "Event Processed";
                    break;
                //start sqlcm 5.6 - 5363
                case StatsCategory.Logins:
                    statsCategory = "Logins";
                    break;
                case StatsCategory.Logout:
                    statsCategory = "Logouts";
                    break;
                    //end sqlcm 5.6 - 5363
            }

            SQLcomplianceConfiguration sqlcomplianceConfig = new SQLcomplianceConfiguration();

            Repository rep = null;
            rep = new Repository();
            rep.OpenConnection();
            sqlcomplianceConfig.Read(rep.connection);
            String msgBody = statsCategory + " threshold event occurred on " + sqlcomplianceConfig.Server + " instance at " + this.MaxTime;

            return msgBody;
        }


        public double CurrentRate()
        {
            if (Rates.Points.Count == 0)
                return 0.0;
            else
                return Rates.Points[Rates.Points.Count - 1].Value;
        }

        public double MaxRate(DateTime lowerBound)
        {
            double max = 0;
            foreach (StatsDataPoint p in Rates.Points)
            {
                if (p.Time > lowerBound && p.Value > max)
                    max = p.Value;
            }
            return max;
        }
    }

    public class DatabaseStatistics : StatsDataSeries
    {
        private ServerRecord _server;
        private DatabaseRecord _database;

        public DatabaseStatistics(ServerRecord server, DatabaseRecord database)
        {
            _server = server;
            _database = database;
        }

        public ServerRecord Server
        {
            get { return _server; }
            set { _server = value; }
        }

        public DatabaseRecord Database
        {
            get { return _database; }
            set { _database = value; }
        }
    }

    public class StatsDataSeries
    {
        private SortedList<DateTime, StatsDataPoint> _points;
        private double _minValue;
        private double _maxValue;
        private string _name;
        private StatsCategory _category;

        public StatsDataSeries()
        {
            _points = new SortedList<DateTime, StatsDataPoint>();
            _minValue = Double.MaxValue;
            _maxValue = Double.MinValue;
        }

        public IList<StatsDataPoint> Points
        {
            get { return _points.Values; }
        }

        public List<StatsDataPoint> PointsEx
        {
            get
            {
                List<StatsDataPoint> retVal = new List<StatsDataPoint>();
                retVal.AddRange(_points.Values);
                return retVal;
            }
        }

        public double MinValue
        {
            get { return _minValue; }
        }

        public double MaxValue
        {
            get { return _maxValue; }
        }

        public DateTime MinTime
        {
            get
            {
                if (_points.Count == 0)
                    return DateTime.MinValue;
                else
                    return _points.Values[0].Time;
            }
        }

        public DateTime MaxTime
        {
            get
            {
                if (_points.Count == 0)
                    return DateTime.MaxValue;
                else
                    return _points.Values[_points.Count - 1].Time;
            }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public StatsCategory Category
        {
            get { return _category; }
            set { _category = value; }
        }

        public int TotalSince(DateTime t)
        {
            int retVal = 0;
            foreach (StatsDataPoint pt in _points.Values)
            {
                if (pt.Time > t)
                    retVal += (int)pt.Value;
            }

            return retVal;
        }

        //
        // Add the incoming data point to the stats series.
        //  If the point already exists in the series, we return
        //  a data-point that represents the difference between the original
        //  data point and the new data point.
        //
        public StatsDataPoint Add(StatsDataPoint pt)
        {
            StatsDataPoint retVal = pt;

            if (pt.Value > _maxValue)
                _maxValue = pt.Value;
            if (pt.Value < _minValue)
                _minValue = pt.Value;

            if (_points.ContainsKey(pt.Time))
            {
                StatsDataPoint existingPoint = _points[pt.Time];
                retVal = new StatsDataPoint();
                retVal.Time = pt.Time;
                retVal.Value = pt.Value - existingPoint.Value;
                existingPoint.Value = pt.Value;
            }
            else
                _points.Add(pt.Time, pt);

            return retVal;
        }

        public void Increment(StatsDataPoint pt)
        {
            if (_points.ContainsKey(pt.Time))
            {
                StatsDataPoint pt2 = _points[pt.Time];
                pt2.Value += pt.Value;
                if (pt2.Value > _maxValue)
                    _maxValue = pt2.Value;
                if (pt2.Value < _minValue)
                    _minValue = pt2.Value;

            }
            else
            {
                Add(pt);
            }
        }

        public void Increment(DateTime time, double value)
        {
            StatsDataPoint pt = new StatsDataPoint();
            pt.Time = time;
            pt.Value = value;
            Increment(pt);
        }

        public StatsDataSeries ExtractRange(DateTime startDate, DateTime endDate, TimeZoneHelper.TimeZoneInfo tzInfo)
        {
            StatsDataSeries retVal = new StatsDataSeries();
            startDate = StatsDAL.CreateStatsIntervalTime(startDate);
            endDate = StatsDAL.CreateStatsIntervalTime(endDate);
            DateTime entry = startDate.AddMinutes(15);

            // We need a point for each spot on the graph, regardless of data being present
            //  so we preload.
            while (entry <= endDate)
            {
                StatsDataPoint pt = new StatsDataPoint();
                pt.Time = tzInfo.ToLocalTime(entry);
                retVal.Add(pt);
                entry = entry.AddMinutes(15);
            }

            foreach (StatsDataPoint pt in Points)
            {
                if (pt.Time > startDate && pt.Time <= endDate)
                {
                    //               TimeSpan diff = pt.Time - startDate;
                    //               int periodOffset = (int)Math.Ceiling(diff.TotalMinutes / (periodTime * 15.0));
                    //               DateTime key = startDate.AddMinutes(periodOffset * periodTime * 15);
                    retVal.Increment(tzInfo.ToLocalTime(pt.Time), pt.Value);
                }
            }
            return retVal;
        }

        /// <summary>
        /// This function compresses a set of data points using the period to determine the distance in time
        /// between each point rendered.  The compression is accomplished by adding all values within a
        /// gap.  If data is absent within the desired ranges, zeroes will be generated.
        /// </summary>
        /// <param name="startDate">start of the range of generated data</param>
        /// <param name="endDate">end of the range of generated data</param>
        /// <param name="periodTime">the width between points in the generated data in 15-minute increments</param>
        /// <returns>The resulting compressed series</returns>
        public StatsDataSeries CompressSeriesWithSum(DateTime startDate, DateTime endDate, int periodTime)
        {
            StatsDataSeries retVal = new StatsDataSeries();
            startDate = StatsDAL.CreateStatsIntervalTime(startDate);
            endDate = StatsDAL.CreateStatsIntervalTime(endDate);
            DateTime entry = startDate.AddMinutes(15 * periodTime);

            // We need a point for each spot on the graph, regardless of data being present
            //  so we preload.
            while (entry <= endDate)
            {
                StatsDataPoint pt = new StatsDataPoint();
                pt.Time = entry;
                retVal.Add(pt);
                entry = entry.AddMinutes(15 * periodTime);
            }

            foreach (StatsDataPoint pt in Points)
            {
                if (pt.Time > startDate && pt.Time <= endDate)
                {
                    TimeSpan diff = pt.Time - startDate;
                    int periodOffset = (int)Math.Ceiling(diff.TotalMinutes / (periodTime * 15.0));
                    DateTime key = startDate.AddMinutes(periodOffset * periodTime * 15);
                    retVal.Increment(key, pt.Value);
                }
            }
            return retVal;
        }

        /// <summary>
        /// This function compresses a set of data points using the period to determine the distance in time
        /// between each point rendered.  The compression is accomplished by averaging all values within a
        /// gap.  If data is absent within the desired ranges, zeroes will be generated.
        /// </summary>
        /// <param name="startDate">start of the range of generated data</param>
        /// <param name="endDate">end of the range of generated data</param>
        /// <param name="periodTime">the width between points in the generated data in 15-minute increments</param>
        /// <returns>The resulting compressed series</returns>
        public StatsDataSeries CompressSeriesWithAverage(DateTime startDate, DateTime endDate, int periodTime)
        {
            StatsDataSeries retVal = new StatsDataSeries();
            Dictionary<DateTime, int> divisors = new Dictionary<DateTime, int>();
            startDate = StatsDAL.CreateStatsIntervalTime(startDate);
            endDate = StatsDAL.CreateStatsIntervalTime(endDate);
            DateTime entry = startDate.AddMinutes(15 * periodTime);

            // We need a point for each spot on the graph, regardless of data being present
            //  so we preload.
            while (entry <= endDate)
            {
                StatsDataPoint pt = new StatsDataPoint();
                pt.Time = entry;
                retVal.Add(pt);
                divisors.Add(pt.Time, 0);
                entry = entry.AddMinutes(15 * periodTime);
            }

            // First we add them up in the proper slots
            foreach (StatsDataPoint pt in Points)
            {
                if (pt.Time > startDate && pt.Time <= endDate)
                {
                    TimeSpan diff = pt.Time - startDate;
                    int periodOffset = (int)Math.Ceiling(diff.TotalMinutes / (periodTime * 15.0));
                    DateTime key = startDate.AddMinutes(periodOffset * periodTime * 15);
                    retVal.Increment(key, pt.Value);
                    divisors[key]++;
                }
            }

            // Now we divide them to get our average
            foreach (StatsDataPoint pt in retVal.Points)
            {
                if (divisors[pt.Time] > 1)
                    pt.Value /= divisors[pt.Time];
            }
            // We modified points directly, update min/max
            retVal.UpdateMinMax();
            return retVal;
        }

        public StatsDataSeries ConvertSeriesToRate(int periodTime)
        {
            if (_points.Count == 0)
                return new StatsDataSeries();
            else
                return ConvertSeriesToRate(MinTime.AddMinutes(-periodTime * 15), DateTime.UtcNow, periodTime);
        }

        public StatsDataSeries ConvertSeriesToRate(DateTime startDate, DateTime endDate, int periodTime)
        {
            StatsDataSeries tmpSeries = new StatsDataSeries();
            startDate = StatsDAL.CreateStatsIntervalTime(startDate).AddMinutes(-15 * (periodTime - 1)); // Subtract period so the range of rates is correct
            endDate = StatsDAL.CreateStatsIntervalTime(endDate);
            DateTime entry = startDate;
            Queue<StatsDataPoint> currentValues = new Queue<StatsDataPoint>();
            double sum = 0;

            // We need a point for each spot on the graph, regardless of data being present
            //  so we preload.
            while (entry <= endDate)
            {
                StatsDataPoint pt = new StatsDataPoint();
                pt.Time = entry;
                tmpSeries.Add(pt);
                entry = entry.AddMinutes(15);
            }

            foreach (StatsDataPoint p in Points)
            {
                if (p.Time >= startDate && p.Time <= endDate)
                    tmpSeries.Increment(p);
            }

            StatsDataSeries retVal = new StatsDataSeries();
            foreach (StatsDataPoint p in tmpSeries.Points)
            {
                currentValues.Enqueue(p);
                sum += p.Value;
                if (currentValues.Count == periodTime)
                {
                    // Queue is full, drop a rate
                    StatsDataPoint pRate = p.Clone();
                    pRate.Value = sum;
                    retVal.Add(pRate);
                    StatsDataPoint pRemove = currentValues.Dequeue();
                    sum -= pRemove.Value;
                }
            }

            return retVal;
        }

        private void UpdateMinMax()
        {
            _minValue = Double.MaxValue;
            _maxValue = Double.MinValue;
            foreach (StatsDataPoint p in _points.Values)
            {
                if (p.Value > _maxValue)
                    _maxValue = p.Value;
                if (p.Value < _minValue)
                    _minValue = p.Value;
            }
        }
    }

    public class StatsDataPoint
    {
        private DateTime _time;
        private double _value = 0;

        public double Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public DateTime Time
        {
            get { return _time; }
            set { _time = value; }
        }

        public StatsDataPoint Clone()
        {
            return MemberwiseClone() as StatsDataPoint;
        }
    }

    public class AxisRange : IComparable
    {
        private DateTime _startTime;
        private DateTime _endTime;
        private int _level;

        public AxisRange(DateTime start, DateTime end) : this(start, end, 0)
        {
        }

        public AxisRange(DateTime start, DateTime end, int level)
        {
            _startTime = start;
            _endTime = end;
            _level = level;
        }


        public DateTime StartTime
        {
            get { return _startTime; }
            set { _startTime = value; }
        }

        public DateTime EndTime
        {
            get { return _endTime; }
            set { _endTime = value; }
        }

        public int Level
        {
            get { return _level; }
            set { _level = value; }
        }

        public int CompareTo(object obj)
        {
            if (obj is AxisRange)
            {
                return _level.CompareTo(((AxisRange)obj)._level);
            }
            else
                throw new Exception("AxisRange can only compare to AxisRange objects");
        }
    }

    public class AxisRangePoint : StatsDataPoint
    {
        private List<AxisRange> _ranges;

        public AxisRangePoint(DateTime time)
        {
            Time = time;
            _ranges = new List<AxisRange>();
        }

        public void AddRange(AxisRange range)
        {
            if (_ranges.Contains(range))
                return;
            if (Time >= range.StartTime && Time <= range.EndTime)
            {
                _ranges.Add(range);
                UpdateValue();
            }
        }

        private void UpdateValue()
        {
            _ranges.Sort();
            if (_ranges.Count > 0)
                Value = _ranges[0].Level;
            else
                Value = 0;
        }

        public void RemoveRange(AxisRange range)
        {
            if (_ranges.Remove(range))
            {
                UpdateValue();
            }
        }
    }

    // These are defined in the order they take precedence on the report card.
    public enum ReportCardStatus
    {
        NotAuditedWithThreshold,
        AuditedExceedsCritical,
        AuditedExceedsWarning,
        AuditedOk,
        AuditedNoThreshold,
        NotAuditedNoThreshold
    };
}
