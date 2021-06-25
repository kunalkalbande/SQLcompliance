using System;
using System.Collections;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using Idera.SQLcompliance.Core.Agent;
using Idera.SQLcompliance.Core.Event;
using Idera.SQLcompliance.Core.Scripting;
using Idera.SQLcompliance.Core.Stats;
using Idera.SQLcompliance.Core.TimeZoneHelper;
using TimeZoneInfo = Idera.SQLcompliance.Core.TimeZoneHelper.TimeZoneInfo;
using System.Data;
using System.Collections.Generic;

namespace Idera.SQLcompliance.Core.Collector
{
    /// <summary>
    /// Summary description for ArchiveHelper.
    /// </summary>
    internal class ArchiveJob : IDisposable
    {
        internal static Object archiveLock = new Object();
        internal static bool archiveInProgress = false;

        #region Private Member Fields
        private Repository rep = null;
        private bool archiveOn = true;
        private int archiveAge = 60;
        private int archivePeriod = 3;
        private int archiveInterval = 7;

        private int archiveBias = 0;
        private int archiveStandardBias = 0;
        private DateTime archiveStandardDate = DateTime.MinValue;
        private int archiveDaylightBias = 0;
        private DateTime archiveDaylightDate = DateTime.MinValue;

        private int SqlComplianceDbSchemaVersion = CoreConstants.RepositorySqlComplianceDbSchemaVersion;
        private int EventDbSchemaVersion = CoreConstants.RepositoryEventsDbSchemaVersion;
        private DateTime archiveEndTime = DateTime.MinValue;
        private string archivePrefix = "SQLcmArchive";
        private string archiveTimeZoneName = "";

        private DateTime archiveJobCreated = DateTime.UtcNow;

        private IntegrityCheckAction archiveIntegrityCheckAction;
        private int archiveBatchSize = CoreConstants.archiveBatchSize;

        // this is here until we have impersonate in place
        public string archiveUser = "";

        public int archiveEventCount = 0;
        public int archiveEventSqlCount = 0;

        private string _archiveDatabaseFilesLocation = string.Empty;


        public int ArchiveInterval
        {
            get { return archiveInterval; }
        }

        public bool AutoArchiveOn
        {
            get { return archiveOn; }
        }

        public int ArchiveAge
        {
            get { return archiveAge; }
            set { archiveAge = value; }
        }

        public int ArchivePeriod
        {
            get { return archivePeriod; }
            set { archivePeriod = value; }
        }

        public string ArchivePrefix
        {
            get { return archivePrefix; }
            set { archivePrefix = value; }
        }

        public IntegrityCheckAction IntegrityCheckAction
        {
            get { return archiveIntegrityCheckAction; }
            set { archiveIntegrityCheckAction = value; }
        }

        public TimeZoneStruct ArchiveTimeZone
        {
            set
            {
                archiveBias = value.Bias;
                archiveStandardBias = value.StandardBias;
                archiveStandardDate = SystemTime.ToTimeZoneDateTime(value.StandardDate);
                archiveDaylightBias = value.DaylightBias;
                archiveDaylightDate = SystemTime.ToTimeZoneDateTime(value.DaylightDate);
            }
        }

        public int ArchiveBatchSize
        {
            get { return archiveBatchSize; }
            set { archiveBatchSize = value; }
        }

        #endregion

        #region Private Constructor

        internal ArchiveJob()
        {
            rep = new Repository();
            rep.OpenConnection();

            InitializeArchiveSettings(rep.connection);
        }

        public void Dispose()
        {
            if (rep != null)
            {
                rep.CloseConnection();
                rep = null;
            }
        }

        #endregion

        #region Archive Methods

        //---------------------------------------------------------------
        // Archive - archive events and logs for all instances.
        //---------------------------------------------------------------
        internal CMCommandResult Archive()
        {
            CMCommandResult retVal = new CMCommandResult();
            UpdateStatus();

            string[] instances = GetInstancesToArchive();

            if (instances == null)
            {
                retVal.ResultCode = ResultCode.Error;
                retVal.AddResultString("An error occurred getting instances to archive.");
                return retVal;
            }

            // mark archive job in progress
            lock (archiveLock)
            {
                if (archiveInProgress)
                {
                    throw new Exception(CoreConstants.Error_ArchiveInProgress);
                }
                else
                {
                    archiveInProgress = true;
                }
            }

            try
            {
                for (int i = 0; i < instances.Length; i++)
                {
                    // this case is used for scheduled job so we just want to eat the exceptions
                    // since they are already logged and then we can try the next instance            
                    try
                    {
                        Archive(instances[i], false);
                        retVal.AddResultString(String.Format("Archive of audit data completed successfully for server {0}", instances[i]));
                    }
                    catch (Exception e)
                    {
                        retVal.AddResultString(e.Message);
                    }
                }
            }
            finally
            {
                lock (archiveLock)
                {
                    archiveInProgress = false;
                }
            }
            return retVal;
        }

        //---------------------------------------------------------------
        // Archive - archive events and logs for a single instance.
        //---------------------------------------------------------------
        internal CMCommandResult Archive(string instance)
        {
            CMCommandResult retVal = new CMCommandResult();
            Archive(instance, true);
            retVal.AddResultString(String.Format("Archive of audit data completed successfully for server {0}", instance));
            return retVal;
        }

        internal void Archive(string instance, bool checkInProgress)
        {
            string state = "";
            int start = Environment.TickCount;
            bool failedIntegrityCheck = false;
            bool failed = false;
            string msg;
            string instanceDbName = "";

            if (instance == null)
                throw new ArgumentNullException("instance");


            if (checkInProgress)
            {
                lock (archiveLock)
                {
                    if (archiveInProgress)
                    {
                        throw new Exception(CoreConstants.Error_ArchiveInProgress);
                    }
                    else
                    {
                        archiveInProgress = true;
                    }
                }
            }

            try
            {
                if (!ServerRecord.ServerIsAudited(instance, rep.connection))
                {
                    throw new Exception(String.Format(CoreConstants.Exception_ServerDeleted, instance));
                }

                // mark archive attempt and set status = In Progress
                ServerRecord.UpdateLastArchiveTime(instance);

                state = "GetInstanceDbName";
                instanceDbName = GetInstanceDbName(instance);


                // check database integrity before running archive
                if (archiveIntegrityCheckAction == IntegrityCheckAction.PerformCheck)
                {
                    state = "IntegrityCheck";

                    IntegrityChecker ic = new IntegrityChecker();
                    CheckResult result = ic.CheckIntegrity(instance, false);

                    if (!result.intact)
                    {

                        failedIntegrityCheck = true;

                        // Aborted Archive Operation                                      
                        msg = String.Format("Archiving aborted for instance {0} - Integrity check failure", instance);

                        LogRecord.WriteLog(rep.connection,
                                            LogType.Archive,
                                            instance,
                                            msg,
                                            archiveUser);
                        return;  // This will force an exception in finally below
                    }
                }
                else if (archiveIntegrityCheckAction == IntegrityCheckAction.SkipCheck)
                {
                    LogRecord.WriteLog(rep.connection, LogType.ManualIntegrityCheck, instance, "Skipped integrity check before archive operation");
                }


                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                         String.Format("Archiving events for instance {0} - Start\n" +
                                                        "\tArchive end time: {1}\n" +
                                                        "\tArchive bias: {2}\n" +
                                                        "\tArchive batch size: {3}",
                                                        instance,
                                                        archiveEndTime.ToString(),
                                                        archiveBias,
                                                        archiveBatchSize));

                // Archive events table
                state = "ArchiveEventsTable";
                archiveEventCount = 0;
                Hashtable list = ArchiveEventsTable(instance, instanceDbName);

                // Archive eventSql table
                state = "ArchiveEventSQLTable";
                archiveEventSqlCount = 0;
                ArchiveEventSQLTable(instance, instanceDbName);

                // Archive SQLsecureLog table
                state = "ArchiveSQLsecureLogTable";
                ArchiveSQLsecureLogTable(instance);

                // Archive stats table
                state = "ArchiveStatsTable";
                ArchiveStatsTable(instance, instanceDbName);

                // Archive data change tables
                state = "ArchiveDataChangeTables";
                ArchiveDataChangeTables(instance, instanceDbName);

                // Archive data change tables
                state = "ArchiveSensitiveColumns";
                ArchiveSensitiveColumns(instance, instanceDbName);

                // Archive Logins tables
                state = "ArchiveLoginsTables";
                ArchiveLoginsTable(instance, instanceDbName);

                //Access to users             
                GrantAccessToArchive(instance, instanceDbName, list);



                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                        String.Format("Archiving events for instance {0} - Complete ( {1} ms )\n" +
                                                       "\tEvents records: {2}\n" +
                                                       "\tEventSQL records: {3}\n",
                                                       instance,
                                                       Environment.TickCount - start,
                                                       archiveEventCount,
                                                       archiveEventSqlCount));

                LogRecord.WriteLog(rep.connection,
                                    LogType.Archive,
                                    instance,
                                    "Archive of audit data completed successfully.",
                                    archiveUser);
            }
            catch (Exception e)
            {
                msg = String.Format("An error occurred archiving events for instance {0}. State: {1} Error message: {2}.",
                                     instance,
                                     state,
                                     e.Message);
                ErrorLog.Instance.Write(msg, ErrorLog.Severity.Warning);

                LogRecord.WriteLog(rep.connection,
                                    LogType.Archive,
                                    instance,
                                    msg,
                                    archiveUser);
                throw e;
            }
            finally
            {
                // mark archive results (dont want exception on status update to leave lock)
                try
                {
                    int resultValue = CoreConstants.Archive_Completed;
                    if (failed) resultValue = CoreConstants.Archive_FailedWithErrors;
                    if (failedIntegrityCheck) resultValue = CoreConstants.Archive_FailedIntegrity;
                    ServerRecord.UpdateLastArchiveResult(instance, resultValue);
                }
                catch { }

                if (checkInProgress)
                {
                    lock (archiveLock)
                    {
                        archiveInProgress = false;
                    }
                }

                if (failedIntegrityCheck)
                {
                    throw new Exception(String.Format(CoreConstants.Exception_ArchiveFailedIntegrity, instance));
                }
            }
        }


        //-----------------------------------------------------------------------
        // GetLocalTimeDateString
        //-----------------------------------------------------------------------
        static public string
           GetLocalTimeDateString(
              DateTime time
         )
        {
            string retStr;

            if (time == DateTime.MinValue)
            {
                retStr = "Never";
            }
            else
            {
                DateTime local = time.ToLocalTime();
                retStr = String.Format("{0} {1}",
                                        local.ToShortDateString(),
                                        local.ToShortTimeString());
            }

            return retStr;
        }

        //---------------------------------------------------------------
        // UpdateArchiveList - Makes sure all archives listed in system database 
        //                      still exist
        //---------------------------------------------------------------
        static internal void
           UpdateArchiveList()
        {
            Repository rep = new Repository();

            ArrayList databases = new ArrayList();

            try
            {
                rep.OpenConnection();

                string query = GetArchiveDatabasesStatement();
                using (SqlCommand command = new SqlCommand(query, rep.connection))
                {
                    command.CommandTimeout = CoreConstants.sqlcommandTimeout;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                string db = reader.GetString(0);
                                if (db != "") databases.Add(db);
                            }
                        }
                    }

                    if (databases != null)
                    {
                        for (int i = 0; i < databases.Count; i++)
                        {
                            string database = (string)databases[i];
                            if (!EventDatabase.DatabaseExists(database, rep.connection))
                            {
                                DeleteArchiveFromSystemDatabasesTable(database, rep.connection);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                   "An error updating the repository archive list.",
                   e,
                   true);
            }
            finally
            {
                rep.CloseConnection();
            }
        }

        //---------------------------------------------------------------
        // DeleteArchiveFromSystemDatabasesTable
        //---------------------------------------------------------------
        static private void
           DeleteArchiveFromSystemDatabasesTable(
              string databaseName,
              SqlConnection conn
           )
        {
            string sql = String.Format("DELETE FROM {0}..{1} WHERE databaseName={2}",
                                        CoreConstants.RepositoryDatabase,
                                        CoreConstants.RepositorySystemDatabaseTable,
                                        SQLHelpers.CreateSafeString(databaseName));
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write("An error remove the archive entry fro the systemdatabases table",
                                         sql,
                                         ex);
            }
        }

        //---------------------------------------------------------------
        // IsTimeToArchive
        //---------------------------------------------------------------
        internal bool
           IsTimeToArchive()
        {
            bool isTime = true;

            try
            {
                string stmt = GetLastTimeStatement();

                using (SqlCommand command = new SqlCommand(stmt, rep.connection))
                {
                    command.CommandTimeout = CoreConstants.sqlcommandTimeout;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader != null && reader.Read())
                        {
                            DateTime lastTime = SQLHelpers.GetDateTime(reader, 0);
                            DateTime lastTimeCopy = lastTime;

                            if (lastTime != DateTime.MinValue)
                            {
                                lastTime = lastTime.AddDays(ArchiveInterval + 1);

                                DateTime now = DateTime.Now;

                                if (lastTime.CompareTo(now) > 0)
                                {
                                    isTime = false;
                                }
                            }

                            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                                    String.Format("Archive job: Last time: {0}  Next time: {1}  Now: {2}  Time to run: {3}",
                                                                   lastTimeCopy.ToString(),
                                                                   lastTime.ToString(),
                                                                   DateTime.Now.ToString(),
                                                                   isTime));

                        }
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         "An error occurred updating archive times.",
                                         e,
                                         true);
                throw e;
            }

            return isTime;
        }


        #endregion

        //---------------------------------------------------------------
        // InitializeArchiveSettings
        //---------------------------------------------------------------
        private void
           InitializeArchiveSettings(
              SqlConnection dbConn
           )
        {
            try
            {
                string query = GetArchiveSettingStatement();

                using (SqlCommand command = new SqlCommand(query, dbConn))
                {
                    command.CommandTimeout = CoreConstants.sqlcommandTimeout;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader != null && reader.Read())
                        {
                            int col = 0;
                            archiveOn = (SQLHelpers.GetInt32(reader, col++) == 0) ? false : true;
                            archiveTimeZoneName = SQLHelpers.GetString(reader, col++);

                            archiveBias = SQLHelpers.GetInt32(reader, col++);
                            archiveStandardBias = SQLHelpers.GetInt32(reader, col++);
                            archiveStandardDate = SQLHelpers.GetDateTime(reader, col++);
                            archiveDaylightBias = SQLHelpers.GetInt32(reader, col++);
                            archiveDaylightDate = SQLHelpers.GetDateTime(reader, col++);

                            archiveInterval = SQLHelpers.GetInt32(reader, col++);
                            archiveAge = SQLHelpers.GetInt32(reader, col++);
                            archivePeriod = SQLHelpers.GetInt32(reader, col++);
                            archivePrefix = SQLHelpers.GetString(reader, col++);
                            archiveIntegrityCheckAction = SQLHelpers.ByteToBool(reader, col++) ?
                               IntegrityCheckAction.PerformCheck : IntegrityCheckAction.SkipCheck;
                            //archiveRebuildChain = archiveCheckIntegrity ;
                            SqlComplianceDbSchemaVersion = SQLHelpers.GetInt32(reader, col++);
                            EventDbSchemaVersion = SQLHelpers.GetInt32(reader, col++);
                            _archiveDatabaseFilesLocation = SQLHelpers.GetString(reader, col);
                        }
                        else
                        {
                            ErrorLog.Instance.Write(String.Format("No archive settings available. Query: {0}", query));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write("An error occurred reading archive settings.",
                                         e,
                                         true);
                throw e;
            }
        }

        internal void CalculateArchiveEndTime()
        {
            DateTime endTime;

            // calculate end time - convert 
            // we calculate all events up to midnght on the day in question
            // example - all events 1 day old on 12/15 would be all events
            //           up to midnight on 12/14 (in the archive timezone)
            TimeZoneInfo tz = GetTimeZoneInfo();
            endTime = archiveJobCreated;
            endTime = TimeZoneInfo.ToLocalTime(tz, endTime);
            endTime = endTime.AddDays(-archiveAge + 1);
            endTime = new DateTime(endTime.Year, endTime.Month, endTime.Day,
               0, 0, 0);
            archiveEndTime = TimeZoneInfo.ToUniversalTime(tz, endTime);

            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
               String.Format("Archive: Archive events older then {0} (UTC) = {1} (Archive Time Zone)",
               archiveEndTime.ToString(),
               endTime.ToString()));
        }


        //---------------------------------------------------------------
        // UpdateStatus
        //---------------------------------------------------------------
        private void
           UpdateStatus()
        {
            try
            {
                string stmt = GetUpdateStatusStatement();
                Execute(stmt);
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write("An error occurred updating timeLastArchive.",
                                         e,
                                         true);
                // continue anyway                                     
            }
        }

        //---------------------------------------------------------------
        // GetInstancesToArchive
        //---------------------------------------------------------------
        private string[]
           GetInstancesToArchive()
        {
            ArrayList instanceList = new ArrayList();

            try
            {
                string query = GetArchivingInstancesStatement();
                using (SqlCommand command = new SqlCommand(query, rep.connection))
                {
                    command.CommandTimeout = CoreConstants.sqlcommandTimeout;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                if (!reader.IsDBNull(0))
                                    instanceList.Add(reader.GetString(0));
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                   "An error occurred getting instances to archive.",
                   e,
                   true);
            }

            return (string[])instanceList.ToArray(typeof(string));
        }


        //---------------------------------------------------------------
        // ArchiveEventsTable
        //---------------------------------------------------------------
        private Hashtable
           ArchiveEventsTable(
              string instance,
              string instanceDbName
           )
        {
            bool eventsToArchive = true;
            DateTime earliestEventTime;
            DateTime beginTime;
            DateTime endTime;
            string archiveDbName;
            bool anyArchived = false;
            Hashtable archiveDbList = new Hashtable();

            try
            {
                while (eventsToArchive)
                {
                    earliestEventTime = GetEarliestEventTime(instanceDbName,
                                                              CoreConstants.RepositoryEventsTable);
                    if (earliestEventTime >= archiveEndTime)
                    {
                        eventsToArchive = false;

                    }
                    else
                    {
                        if (!anyArchived)
                        {
                            EventDatabase.SetDatabaseState(instanceDbName, EventsDatabaseState.Busy);
                            anyArchived = true;
                        }

                        // We must create the database name based upon the time with respect
                        //  to the timezone we are archiving against.  Otherwise we get the
                        //  wrong month on edge cases.
                        TimeZoneInfo tz = GetTimeZoneInfo();
                        DateTime archiveTimeZoneEventTime = TimeZoneInfo.ToLocalTime(tz, earliestEventTime);
                        archiveDbName = CreateArchiveDatabase(instance, archiveTimeZoneEventTime);

                        try
                        {
                            archiveDbList.Add(archiveDbName, archiveDbName);
                        }
                        catch
                        {
                            // ignore duplicate db names
                        }
                        GetArchivePeriod(earliestEventTime, out beginTime, out endTime);

                        EventDatabase.SetDatabaseState(archiveDbName, EventsDatabaseState.Busy);

                        ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                                 String.Format("Archive Period: Earliest event: {0} Archive events before: {1}\n" +
                                                                "                Begin Time:     {2} End Time:              {3}\n\n" +
                                                                "                Archive Bias:   {4}",
                                                                earliestEventTime.ToString(),
                                                                archiveEndTime.ToString(),
                                                                beginTime.ToString(),
                                                                endTime.ToString(),
                                                                archiveBias));

                        ArchiveEvents(instanceDbName, archiveDbName, beginTime, endTime);

                    }
                }

                if (anyArchived)
                {
                    // rebuild hash chain for archive databases
                    IDictionaryEnumerator enumerator = archiveDbList.GetEnumerator(); ;
                    while (enumerator.MoveNext())
                    {
                        // Only rebuild the chain if they are checking integrity
                        if (archiveIntegrityCheckAction != IntegrityCheckAction.SkipCheck)
                            RebuildHashChain(instance, (string)enumerator.Value, true);
                        else
                            EventDatabase.SetDatabaseState((string)enumerator.Value, EventsDatabaseState.NormalChainBroken);
                        UpdateArchiveEventTimeSpan((string)enumerator.Value);
                    }

                    // rebuild hash chain for 'live' events database
                    if (archiveIntegrityCheckAction != IntegrityCheckAction.SkipCheck)
                        RebuildHashChain(instance, instanceDbName, false);
                    else
                        EventDatabase.SetDatabaseState(instanceDbName, EventsDatabaseState.NormalChainBroken);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return archiveDbList;
        }

        //---------------------------------------------------------------
        // ArchiveEvents
        //---------------------------------------------------------------
        private void
           ArchiveEvents(
              string instanceDbName,
              string archiveDbName,
              DateTime beginTime,
              DateTime endTime
           )
        {
            ArchiveCopy acopy = new ArchiveCopy(instanceDbName,
                                                 archiveDbName,
                                                 beginTime,
                                                 endTime,
                                                 archiveBatchSize);

            acopy.Perform(false);
            archiveEventCount += acopy.ArchivedEventCount;

        }


        //---------------------------------------------------------------
        // ArchiveEventSQLTable
        //---------------------------------------------------------------
        private void
           ArchiveEventSQLTable(
           string instance,
           string instanceDbName
           )
        {
            bool eventsToArchive = true;
            DateTime earliestEventTime;
            DateTime beginTime;
            DateTime endTime;
            string archiveDbName;

            while (eventsToArchive)
            {
                earliestEventTime = GetEarliestEventTime(instanceDbName,
                   CoreConstants.RepositoryEventSqlTable);
                if (earliestEventTime >= archiveEndTime)
                {
                    eventsToArchive = false;
                }
                else
                {
                    // We must create the database name based upon the time with respect
                    //  to the timezone we are archiving against.  Otherwise we get the
                    //  wrong month on edge cases.
                    TimeZoneInfo tz = GetTimeZoneInfo();
                    DateTime archiveTimeZoneEventTime = TimeZoneInfo.ToLocalTime(tz, earliestEventTime);
                    archiveDbName = CreateArchiveDatabase(instance, archiveTimeZoneEventTime);
                    GetArchivePeriod(earliestEventTime, out beginTime, out endTime);
                    ArchiveEventSQL(instanceDbName, archiveDbName, beginTime, endTime);
                }

            }
        }

        //---------------------------------------------------------------
        // ArchiveEventSQL
        //---------------------------------------------------------------
        private void
           ArchiveEventSQL(
              string instanceDbName,
              string archiveDbName,
              DateTime beginTime,
              DateTime endTime
           )
        {
            ArchiveCopy acopy = new ArchiveCopy(instanceDbName,
                                                 archiveDbName,
                                                 beginTime,
                                                 endTime,
                                                 archiveBatchSize);

            acopy.Perform(true);
            archiveEventSqlCount += acopy.ArchivedEventSqlCount;
        }


        //---------------------------------------------------------------
        // ArchiveSQLsecureLogTable
        //---------------------------------------------------------------
        private void
           ArchiveSQLsecureLogTable(
              string instance
           )
        {
            bool eventsToArchive = true;
            DateTime earliestLogTime;
            DateTime beginTime;
            DateTime endTime;
            string archiveDbName;

            while (eventsToArchive)
            {
                earliestLogTime = GetEarliestLogTime(instance);

                if (earliestLogTime >= archiveEndTime)
                {
                    eventsToArchive = false;
                }
                else
                {
                    // We must create the database name based upon the time with respect
                    //  to the timezone we are archiving against.  Otherwise we get the
                    //  wrong month on edge cases.
                    TimeZoneInfo tz = GetTimeZoneInfo();
                    DateTime archiveTimeZoneEventTime = TimeZoneInfo.ToLocalTime(tz, earliestLogTime);
                    archiveDbName = CreateArchiveDatabase(instance, archiveTimeZoneEventTime);
                    GetArchivePeriod(earliestLogTime, out beginTime, out endTime);
                    ArchiveSQLsecureLog(instance, archiveDbName, beginTime, endTime);
                }
            }
        }

        //---------------------------------------------------------------
        // ArchiveSQLsecureLog
        //---------------------------------------------------------------
        private void
           ArchiveSQLsecureLog(
              string instance,
              string archiveDbName,
              DateTime beginTime,
              DateTime endTime
           )
        {
            ArchiveCopy acopy = new ArchiveCopy(instance,
                                                 CoreConstants.RepositoryDatabase,
                                                 archiveDbName,
                                                 beginTime,
                                                 endTime,
                                                 archiveBatchSize);

            while (MoreLogsToCopy(instance, beginTime, endTime) > 0)
            {
                acopy.CopyChangeLog();

                string stmt = GetDeleteSQLsecureLogStatement(instance,
                                                              beginTime,
                                                              endTime);
                Execute(stmt);
            }
        }

        //---------------------------------------------------------------
        // MoreLogsToCopy
        //---------------------------------------------------------------
        private int
           MoreLogsToCopy(
              string instance,
              DateTime beginTime,
              DateTime endTime
           )
        {
            int count;

            string stmt = GetMoreLogsStatement(instance, beginTime, endTime);
            using (SqlCommand cmd = new SqlCommand(stmt, rep.connection))
            {
                cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;

                object obj = cmd.ExecuteScalar();
                if (obj is DBNull)
                    count = 0;
                else
                    count = (int)obj;
            }

            return count;
        }

        private void ArchiveStatsTable(string instance, string instanceDbName)
        {
            bool moreToArchive = true;
            DateTime earliestStatsTime;
            DateTime beginTime;
            DateTime endTime;
            string archiveDbName;

            while (moreToArchive)
            {
                earliestStatsTime = StatsDAL.GetEarliestStatsTime(instanceDbName);

                if (earliestStatsTime >= archiveEndTime)
                {
                    moreToArchive = false;
                }
                else
                {
                    // We must create the database name based upon the time with respect
                    //  to the timezone we are archiving against.  Otherwise we get the
                    //  wrong month on edge cases.
                    TimeZoneInfo tz = GetTimeZoneInfo();
                    DateTime archiveTimeZoneEventTime = TimeZoneInfo.ToLocalTime(tz, earliestStatsTime);
                    archiveDbName = CreateArchiveDatabase(instance, archiveTimeZoneEventTime);
                    GetArchivePeriod(earliestStatsTime, out beginTime, out endTime);
                    ArchiveStats(instanceDbName, archiveDbName, beginTime, endTime);
                }
            }
        }


        void ArchiveStats(string eventDb, string archiveDb, DateTime beginTime, DateTime endTime)
        {
            string checkKeyViolationStmt = StatsDAL.CheckPrimaryKeyConflict(eventDb, archiveDb);
            int count;
            using (SqlCommand cmd = new SqlCommand(checkKeyViolationStmt, rep.connection))
            {
                cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                object obj = cmd.ExecuteScalar();
                if (obj is DBNull)
                    count = 0;
                else
                    count = (int)obj;
            }
            if (count > 0)
            {
                string updateStatsPKData = StatsDAL.UpdateStatsPrimaryKeyData(eventDb, archiveDb);
                Execute(updateStatsPKData);
            }
            string stmt = StatsDAL.CreateArchiveStatsTableStmt(eventDb, archiveDb, beginTime, endTime);

            Execute(stmt);
        }

        private void ArchiveDataChangeTables(string instance, string dbName)
        {
            ArchiveDataChangeTable(instance, dbName, CoreConstants.RepositoryColumnChangesTable);
            ArchiveDataChangeTable(instance, dbName, CoreConstants.RepositoryDataChangesTable);
        }

        private void ArchiveDataChangeTable(string instance, string instanceDbName, string table)
        {
            bool moreToArchive = true;


            while (moreToArchive)
            {
                DateTime earliestTime = GetEarliestEventTime(instanceDbName, table);

                if (earliestTime >= archiveEndTime)
                {
                    moreToArchive = false;
                }
                else
                {
                    // We must create the database name based upon the time with respect
                    //  to the timezone we are archiving against.  Otherwise we get the
                    //  wrong month on edge cases.
                    DateTime beginTime;
                    DateTime endTime;
                    TimeZoneInfo tz = GetTimeZoneInfo();
                    DateTime archiveTimeZoneEventTime =
                       TimeZoneInfo.ToLocalTime(tz, earliestTime);
                    string archiveDbName = CreateArchiveDatabase(instance, archiveTimeZoneEventTime);
                    GetArchivePeriod(earliestTime, out beginTime, out endTime);

                    // Build the archive statement
                    StringBuilder stmt = new StringBuilder();
                    string source = SQLHelpers.CreateSafeDatabaseName(instanceDbName);
                    string destination = SQLHelpers.CreateSafeDatabaseName(archiveDbName);
                    string start = SQLHelpers.CreateSafeDateTimeString(beginTime);
                    string end = SQLHelpers.CreateSafeDateTimeString(endTime);
                    string columns;
                    if (table == CoreConstants.RepositoryDataChangesTable)
                        columns = DataChangeRecord.SelectColumnList;
                    else
                        columns = ColumnChangeRecord.SelectColumnList;
                    stmt.AppendFormat("INSERT INTO {0}..{1} ( {2} ) SELECT TOP {3} {2} FROM {4}..{1} WHERE startTime >= {5} and startTime < {6}",
                                       destination,
                                       table,
                                       columns,
                                       CoreConstants.archiveBatchSize,
                                       source,
                                       start,
                                       end);
                    stmt.Append(";");
                    stmt.AppendFormat("DELETE TOP ({0}) FROM {1}..{2} WHERE startTime >= {3} and startTime < {4}",
                                       CoreConstants.archiveBatchSize,
                                       source,
                                       table,
                                       start,
                                       end);
                    Execute(stmt.ToString());

                }
            }
        }

        private void ArchiveSensitiveColumns(string instance, string instanceDbName)
        {
            bool moreToArchive = true;


            while (moreToArchive)
            {
                DateTime earliestTime = GetEarliestEventTime(instanceDbName, CoreConstants.RepositorySensitiveColumnsTable);

                if (earliestTime >= archiveEndTime)
                {
                    moreToArchive = false;
                }
                else
                {
                    // We must create the database name based upon the time with respect
                    //  to the timezone we are archiving against.  Otherwise we get the
                    //  wrong month on edge cases.
                    DateTime beginTime;
                    DateTime endTime;
                    TimeZoneInfo tz = GetTimeZoneInfo();
                    DateTime archiveTimeZoneEventTime =
                       TimeZoneInfo.ToLocalTime(tz, earliestTime);
                    string archiveDbName = CreateArchiveDatabase(instance, archiveTimeZoneEventTime);
                    GetArchivePeriod(earliestTime, out beginTime, out endTime);

                    // Build the archive statement
                    StringBuilder stmt = new StringBuilder();
                    string source = SQLHelpers.CreateSafeDatabaseName(instanceDbName);
                    string destination = SQLHelpers.CreateSafeDatabaseName(archiveDbName);
                    string start = SQLHelpers.CreateSafeDateTimeString(beginTime);
                    string end = SQLHelpers.CreateSafeDateTimeString(endTime);

                    stmt.AppendFormat("INSERT INTO {0}..{1} ( {2} ) SELECT TOP {3} {2} FROM {4}..{1} WHERE startTime >= {5} and startTime < {6}",
                                       destination,
                                       CoreConstants.RepositorySensitiveColumnsTable,
                                       SensitiveColumnRecord.SelectColumnList,
                                       CoreConstants.archiveBatchSize,
                                       source,
                                       start,
                                       end);
                    stmt.Append(";");
                    stmt.AppendFormat("DELETE TOP ({0}) FROM {1}..{2} WHERE startTime >= {3} and startTime < {4}",
                                       CoreConstants.archiveBatchSize,
                                       source,
                                       CoreConstants.RepositorySensitiveColumnsTable,
                                       start,
                                       end);
                    Execute(stmt.ToString());

                }
            }
        }

        private void ArchiveLoginsTable(string instance, string instanceDbName)
        {
            DateTime earliestTime = GetEarliestEventTime(instanceDbName, CoreConstants.RepositoryEventsTable);

            // We must create the database name based upon the time with respect
            // to the timezone we are archiving against.  Otherwise we get the
            // wrong month on edge cases.
            TimeZoneInfo tz = GetTimeZoneInfo();
            DateTime archiveTimeZoneEventTime = TimeZoneInfo.ToLocalTime(tz, earliestTime);
            string archiveDbName = CreateArchiveDatabase(instance, archiveTimeZoneEventTime);

            // Build the archive statement
            StringBuilder stmt = new StringBuilder();
            string source = SQLHelpers.CreateSafeDatabaseName(instanceDbName);
            string destination = SQLHelpers.CreateSafeDatabaseName(archiveDbName);

            stmt.AppendFormat("INSERT INTO {0}..{1} ( {2} ) (SELECT l.name,l.id FROM {3}..{1} l LEFT JOIN {0}..{1} la ON l.name = la.name AND l.id = la.id where la.name IS NULL)",
                               destination,
                               CoreConstants.RepositoryLoginsTable,
                               CoreConstants.RepositoryLoginsTablesColumn,
                               source
                              );
            stmt.Append(";");
            Execute(stmt.ToString());
        }

        //-------------------------------------------------------------------
        //Access granted
        //--------------------------------------------------------------------

        private void GrantAccessToArchive(string instance, string instanceDbName, Hashtable list)
        {
            try
            {
                if (!string.IsNullOrEmpty(instance) && !string.IsNullOrEmpty(instanceDbName))
                {
                    EventDatabase eventDatabase = new EventDatabase();

                    List<string> names = eventDatabase.GetLoginNames(rep.connection);
                    if (names.Count != 0 && list.Count != 0)
                    {
                        foreach (DictionaryEntry archiveDbName in list)
                        {
                            foreach (string name in names)
                            {
                                int defaultAccess = 0;
                                List<string> type = eventDatabase.GetAccessType(rep.connection, name, instanceDbName);
                                if (type != null && type.Count != 0)
                                {
                                    defaultAccess = 1;
                                    foreach (string tableName in type)
                                    {
                                        if (tableName == CoreConstants.RepositoryEventSqlTable)
                                        {
                                            defaultAccess = 2;
                                            break;
                                        }
                                    }
                                }
                                eventDatabase.ApplyAccess(defaultAccess, name, archiveDbName.Value.ToString(), rep.connection, instanceDbName, true);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                  ex, true);
            }
        }



        //---------------------------------------------------------------
        // GetInstanceDbName
        //---------------------------------------------------------------
        private string
           GetInstanceDbName(
              string instance
           )
        {
            string dbName = "";

            try
            {
                string query = GetInstanceDbNameQuery(instance);
                using (SqlCommand command = new SqlCommand(query, rep.connection))
                {
                    command.CommandTimeout = CoreConstants.sqlcommandTimeout;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read()) dbName = SQLHelpers.GetString(reader, 0);
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                   e,
                   true);
            }

            return dbName;
        }

        //---------------------------------------------------------------
        // GetEarliestEventTime
        //---------------------------------------------------------------
        private DateTime
           GetEarliestEventTime(
              string databaseName,
              string tableName
           )
        {
            DateTime eventTime = archiveEndTime.AddMilliseconds(1);
            try
            {
                string query = GetEarliestEventTimeQuery(databaseName, tableName);
                using (SqlCommand command = new SqlCommand(query, rep.connection))
                {
                    command.CommandTimeout = CoreConstants.sqlcommandTimeout;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader != null && reader.Read())
                        {
                            if (!reader.IsDBNull(0))
                                eventTime = reader.GetDateTime(0);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                   e,
                   true);
                throw;
            }


            return eventTime;

        }

        //---------------------------------------------------------------
        // GetEarliestLogTime
        //---------------------------------------------------------------
        private DateTime
           GetEarliestLogTime(
              string instance
           )
        {
            DateTime eventTime = DateTime.UtcNow;
            try
            {
                string query = GetEarliestLogTimeQuery(instance);
                using (SqlCommand command = new SqlCommand(query, rep.connection))
                {
                    command.CommandTimeout = CoreConstants.sqlcommandTimeout;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader != null && reader.Read())
                        {
                            if (!reader.IsDBNull(0))
                                eventTime = reader.GetDateTime(0);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                   e,
                   true);
            }

            return eventTime;
        }


        //---------------------------------------------------------------
        // GetArchivePeriod
        //---------------------------------------------------------------
        internal void
           GetArchivePeriod(
              DateTime eventTime,
              out DateTime beginTime,
              out DateTime endTime)
        {
            TimeZoneInfo tz = GetTimeZoneInfo();
            eventTime = TimeZoneInfo.ToLocalTime(tz, eventTime);

            if (archivePeriod == 12) // Monthly partition
            {
                beginTime = new DateTime(eventTime.Year, eventTime.Month, 1,
                                          0, 0, 0);
                endTime = beginTime.AddMonths(1);
            }
            else if (archivePeriod == 3) // Quarterly partition
            {
                int index = (eventTime.Month - 1) / 3;
                beginTime = new DateTime(eventTime.Year, index * 3 + 1, 1,
                                          0, 0, 0);
                endTime = beginTime.AddMonths(3);
            }
            else  // Yearly partition
            {
                beginTime = new DateTime(eventTime.Year, 1, 1,     // Jan 1, Year 12:00 am
                                          0, 0, 0);
                endTime = beginTime.AddYears(1);
            }
            // Convert to UTC time
            beginTime = TimeZoneInfo.ToUniversalTime(tz, beginTime);
            endTime = TimeZoneInfo.ToUniversalTime(tz, endTime);
            if (endTime > archiveEndTime) endTime = archiveEndTime;
        }

        //---------------------------------------------------------------
        // GetTimeZoneInfo
        //---------------------------------------------------------------
        public TimeZoneInfo
           GetTimeZoneInfo()
        {
            TimeZoneStruct tzs = new TimeZoneStruct();
            tzs.Bias = archiveBias;
            tzs.StandardBias = archiveStandardBias;
            tzs.StandardDate = SystemTime.FromTimeZoneDateTime(archiveStandardDate);
            tzs.DaylightBias = archiveDaylightBias;
            tzs.DaylightDate = SystemTime.FromTimeZoneDateTime(archiveDaylightDate);

            TimeZoneInfo outTimeZoneInfo = new TimeZoneInfo();
            outTimeZoneInfo.TimeZoneStruct = tzs;

            return outTimeZoneInfo;
        }

        //---------------------------------------------------------------
        // CreateArchiveDatabase
        //---------------------------------------------------------------
        private string
           CreateArchiveDatabase(
              string instance,
              DateTime eventTime
           )
        {
            string displayName;
            string description;
            string databaseName = CreateDatabaseName(instance,
                                                      eventTime,
                                                      out displayName,
                                                      out description);

            if (!EventDatabase.DatabaseExists(databaseName, rep.connection))
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                        String.Format("Creating archive database {0} for instance {1}", databaseName, instance));

                CreateDatabase(databaseName);
                Thread.Sleep(1000); // sometimes it seems like SQL takes a second to 
                                    // be ready after create database

                ErrorLog.Instance.Write(ErrorLog.Level.Debug, "Archive::Creating tables");

                try
                {
                    EventDatabase.SetRecoveryModel(databaseName, rep.connection);

                    EventDatabase.CreateEventsTable(databaseName, rep.connection, true);
                    EventDatabase.CreateSQLTable(databaseName, rep.connection);
                    EventDatabase.CreateIdLookUpTables(databaseName, rep.connection);
                    EventDatabase.CreateStatsTable(databaseName, rep.connection);
                    CreateSQLsecureLogTable(databaseName);
                    CreateDescriptionTable(databaseName);
                    EventDatabase.CreateDataChangesTable(databaseName, rep.connection, true);
                    EventDatabase.CreateColumnChangesTable(databaseName, rep.connection, true);
                    EventDatabase.CreateDataChangeLinkProcs(databaseName, rep.connection);
                    EventDatabase.CreateSensitiveColumnsTable(databaseName, rep.connection, true);
                    EventDatabase.BuildIndexes(databaseName, rep.connection, true);
                    EventDatabase.Build30Indexes(rep.connection, databaseName);
                    EventDatabase.Build31Indexes(rep.connection, databaseName);
                    EventDatabase.BuildPageIndexes(rep.connection, databaseName);
                    ErrorLog.Instance.Write(ErrorLog.Level.Debug, "Archive::Tables created");
                }
                catch (Exception ex)
                {
                    // exception at this point - drop the database
                    EventDatabase.DropDatabase(databaseName, rep.connection);

                    throw ex;
                }

                DateTime startTime;
                DateTime endTime;
                GetArchivePeriod(eventTime, out startTime, out endTime);
                endTime -= new TimeSpan(0, 0, 0, 0, 1);

                TimeZoneInfo tz = TimeZoneInfo.CurrentTimeZone;
                int srvBias = tz.TimeZoneStruct.Bias;
                int srvStandardBias = tz.TimeZoneStruct.StandardBias;
                DateTime srvStandardDate = SystemTime.ToTimeZoneDateTime(tz.TimeZoneStruct.StandardDate);
                int srvDaylightBias = tz.TimeZoneStruct.DaylightBias;
                DateTime srvDaylightDate = SystemTime.ToTimeZoneDateTime(tz.TimeZoneStruct.DaylightDate);

                int srvDefaultAccess = 2;

                try
                {
                    ServerRecord srv = new ServerRecord();
                    srv.Connection = rep.connection;
                    srv.Read(instance);

                    srvBias = srv.Bias;
                    srvStandardBias = srv.StandardBias;
                    srvStandardDate = srv.StandardDate;
                    srvDaylightBias = srv.DaylightBias;
                    srvDaylightDate = srv.DaylightDate;

                    srvDefaultAccess = srv.DefaultAccess;
                }
                catch (Exception e)
                {
                    ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                       "An error occurred reading Server record for " + instance + ".",
                       e,
                       true);
                }

                AddSecurity(databaseName, srvDefaultAccess);

                UpdateDescriptionTable(databaseName,
                                        instance,
                                        displayName,
                                        description,
                                        startTime,
                                        endTime,
                                        srvBias,
                                        srvStandardBias,
                                        srvStandardDate,
                                        srvDaylightBias,
                                        srvDaylightDate,
                                        srvDefaultAccess);

                EventDatabase.AddSystemDatabase(instance,
                                                 databaseName,
                                                 true,
                                                 displayName,
                                                 description,
                                                 rep.connection);
            }

            return databaseName;
        }

        //---------------------------------------------------------------
        // CreateDatabaseName
        //---------------------------------------------------------------
        private string
           CreateDatabaseName(
              string instance,
              DateTime eventTime,
              out string displayName,
              out string description
           )
        {
            string archiveSuffix;
            string name;
            string safeInstanceName;
            int nameLength;

            int index = instance.IndexOf(@"\");
            if (index > 0)
            {
                safeInstanceName = instance.Substring(0, index)
                   + "_"
                   + instance.Substring(index + 1, instance.Length - index - 1);
            }
            else
                safeInstanceName = instance;

            if (archivePeriod == 12) // Monthly
            {
                archiveSuffix = String.Format("{0}-{1:00} ({2})",
                                               eventTime.Year,
                                               eventTime.Month,
                                               eventTime.ToString("MMM", DateTimeFormatInfo.InvariantInfo));

                nameLength = archivePrefix.Length
                   + safeInstanceName.Length
                   + archiveSuffix.Length
                   + 2;

                // Truncate the instance name if the database name is too long
                if (nameLength > 128)
                    safeInstanceName = safeInstanceName.Substring(0, 128 - (nameLength - 128));

                name = String.Format("{0}_{1}_{2}",
                                      archivePrefix,
                                      safeInstanceName,
                                      archiveSuffix);

                displayName = String.Format("{0}-{1:00} ({2})",
                                             eventTime.Year,
                                             eventTime.Month,
                                             eventTime.ToString("MMM", DateTimeFormatInfo.InvariantInfo));

                description = String.Format("{0} {1} Events for SQL Server {2}",
                                              eventTime.Year,
                                              eventTime.ToString("MMMM", DateTimeFormatInfo.InvariantInfo),
                                              instance);
            }
            else if (archivePeriod == 3) // Quarterly
            {
                // calculate quater
                int quarter = (eventTime.Month + 2) / 3;
                archiveSuffix = "Q" + quarter.ToString();

                nameLength = archivePrefix.Length +
                             safeInstanceName.Length +
                             eventTime.Year.ToString().Length +
                             archiveSuffix.Length +
                             3;

                // Truncate the instance name if the database name is too long
                if (nameLength > 128)
                    safeInstanceName = safeInstanceName.Substring(0, 128 - (nameLength - 128));

                name = String.Format("{0}_{1}_{2}_{3}",
                                      archivePrefix,
                                      safeInstanceName,
                                      eventTime.Year.ToString(),
                                      archiveSuffix);

                displayName = String.Format("{0} {1}",
                                             eventTime.Year,
                                             archiveSuffix);

                description = String.Format("{0} {1} Events for SQL Server {2}",
                                             eventTime.Year,
                                             archiveSuffix,
                                             instance);
            }
            else // Yearly
            {
                nameLength = archivePrefix.Length
                   + safeInstanceName.Length
                   + eventTime.Year.ToString().Length
                   + 2;

                // Truncate the instance name if the database name is too long
                if (nameLength > 128)
                {
                    safeInstanceName = safeInstanceName.Substring(0, 128 - (nameLength - 128));
                }

                name = String.Format("{0}_{1}_{2}",
                                       archivePrefix,
                                       safeInstanceName,
                                       eventTime.Year.ToString());

                displayName = String.Format("{0}", eventTime.Year);

                description = String.Format("{0} Events for SQL Server {1}",
                                             eventTime.Year,
                                             instance);
            }

            return name;
        }

        //---------------------------------------------------------------
        // CreateDatabase
        //---------------------------------------------------------------
        private void CreateDatabase(string databaseName)
        {
            try
            {
                if (!string.IsNullOrEmpty(_archiveDatabaseFilesLocation) && Directory.Exists(_archiveDatabaseFilesLocation))
                    EventDatabase.CreateDatabase(databaseName, rep.connection, _archiveDatabaseFilesLocation);
                else
                    EventDatabase.CreateDatabase(databaseName, rep.connection);
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(String.Format("Error creating archive database: {0}", databaseName),
                                         e,
                                         true);
                throw e;
            }
        }

        //---------------------------------------------------------------
        // AddSecurity
        //---------------------------------------------------------------
        private void
           AddSecurity(
              string databaseName,
              int defaultAccess
           )
        {
            try
            {
                EventDatabase.SetDefaultSecurity(databaseName,
                                                  defaultAccess,
                                                  -1,
                                                  true,
                                                  rep.connection);
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(String.Format("Error adding default security to archive database: {0}", databaseName),
                                         e,
                                         true);
                throw e;
            }
        }

        //---------------------------------------------------------------
        // CreateDescriptionTable
        //---------------------------------------------------------------
        private void
           CreateDescriptionTable(
              string databaseName
           )
        {
            try
            {
                string stmt = GetCreateDescriptionTableStatement(databaseName);
                CreateTable(stmt);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        //---------------------------------------------------------------
        // UpdateDescriptionTable
        //---------------------------------------------------------------
        private void
           UpdateDescriptionTable(
              string databaseName,
              string instance,
              string displayName,
              string description,
              DateTime startTime,
              DateTime endTime,
              int srvBias,
              int srvStandardBias,
              DateTime srvStandardDate,
              int srvDaylightBias,
              DateTime srvDaylightDate,
              int srvDefaultAccess
           )
        {

            try
            {
                string fmt = GetInsertDescriptionRecordStmt();
                string stmt = String.Format(fmt,
                                             SQLHelpers.CreateSafeDatabaseName(databaseName),
                                             CoreConstants.RepositoryArchiveMetaTable,
                                             SQLHelpers.CreateSafeString(instance),
                                             SQLHelpers.CreateSafeString(displayName),
                                             SQLHelpers.CreateSafeString(description),
                                             SQLHelpers.CreateSafeDateTime(startTime),
                                             SQLHelpers.CreateSafeDateTime(endTime),
                                             archiveBias,
                                             archiveStandardBias,
                                             SQLHelpers.CreateSafeDateTime(archiveStandardDate),
                                             archiveDaylightBias,
                                             SQLHelpers.CreateSafeDateTime(archiveDaylightDate),
                                             SQLHelpers.CreateSafeString(archiveTimeZoneName),
                                             SqlComplianceDbSchemaVersion,
                                             EventDbSchemaVersion,
                                             srvBias,
                                             srvStandardBias,
                                             SQLHelpers.CreateSafeDateTime(srvStandardDate),
                                             srvDaylightBias,
                                             SQLHelpers.CreateSafeDateTime(srvDaylightDate),
                                             srvDefaultAccess,
                                             CoreConstants.DatabaseType_Archive);
                using (SqlCommand command = new SqlCommand(stmt, rep.connection))
                {
                    command.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                   "An error occurred updating Description record for " + databaseName + ".",
                   e,
                   true);
                throw e;
            }
        }

        private void
           UpdateArchiveEventTimeSpan(string database)
        {
            try
            {
                string stmt = GetUpdateArchiveTimeSpanStatement(database);

                using (SqlCommand cmd = new SqlCommand(stmt, rep.connection))
                {
                    cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         "An error occurred updating archived event time span for " +
                                         database + ".",
                                         e,
                                         true);
            }
        }


        //---------------------------------------------------------------
        // CreateSQLsecureLogTable
        //---------------------------------------------------------------
        private void
           CreateSQLsecureLogTable(
              string databaseName
           )
        {
            try
            {
                string stmt = GetCreateSQLsecureLogTableStatement(databaseName);
                CreateTable(stmt);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        //---------------------------------------------------------------
        // CreateTable
        //---------------------------------------------------------------
        private void
           CreateTable(
              string statement
           )

        {
            try
            {
                using (SqlCommand command = new SqlCommand(statement, rep.connection))
                {
                    command.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write("An error occurred creating a table for the archive database.",
                                         statement,
                                         e,
                                         true);
                throw e;
            }
        }

        //---------------------------------------------------------------
        // Execute - Execute a non-query SQL statement
        //---------------------------------------------------------------
        private void
           Execute(
              string stmt
           )
        {
            try
            {
                using (SqlTransaction trans = rep.connection.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    using (SqlCommand command = new SqlCommand(stmt, rep.connection, trans))
                    {
                        try
                        {
                            command.CommandTimeout = CoreConstants.sqlcommandTimeout;
                            command.ExecuteNonQuery();
                            trans.Commit();
                        }
                        catch
                        {
                            if (trans != null)
                            {
                                trans.Rollback();
                            }
                            throw;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                         String.Format("An error occurred when executing archive SQL statement.  Statement text: {0}.",
                                                        stmt),
                                         e,
                                         true);
                throw e;
            }
        }

        //---------------------------------------------------------------
        // RebuildHashChain
        //---------------------------------------------------------------
        private void
           RebuildHashChain(
              string instance,
              string dbName,
              bool isArchiveDb
           )
        {
            try
            {
                IntegrityChecker checker = new IntegrityChecker();
                checker.RebuildChain(instance, dbName, isArchiveDb);
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         String.Format("An error occurred rebuilding hash chain for {0}.",
                                                        dbName),
                                         e,
                                         true);
                throw e;
            }
        }

        #region SQL

        //---------------------------------------------------------------
        // GetArchiveSettingStatement
        //---------------------------------------------------------------
        private string
           GetArchiveSettingStatement()
        {
            return String.Format("SELECT archiveOn" +
                                    ", archiveTimeZoneName" +
                                    ", archiveBias" +
                                    ", archiveStandardBias" +
                                    ", archiveStandardDate" +
                                    ", archiveDaylightBias" +
                                    ", archiveDaylightDate" +
                                    ", archiveInterval" +
                                    ", archiveAge" +
                                    ", archivePeriod" +
                                    ", archivePrefix" +
                                    ", archiveCheckIntegrity " +
                                    ", sqlComplianceDbSchemaVersion" +
                                    ", eventsDbSchemaVersion " +
                                    ", archiveDatabaseFilesLocation " +
                                    "FROM {0}",
                                    CoreConstants.RepositoryConfigurationTable);
        }

        //---------------------------------------------------------------
        // GetInstanceDbNameQuery
        //---------------------------------------------------------------
        private string
           GetInstanceDbNameQuery(
              string instance
           )
        {
            return String.Format("SELECT eventDatabase FROM {0} WHERE instance = {1}",
                                  CoreConstants.RepositoryServerTable,
                                  SQLHelpers.CreateSafeString(instance));

        }

        //---------------------------------------------------------------
        // GetEarliestEventTimeQuery
        //---------------------------------------------------------------
        private string
           GetEarliestEventTimeQuery(
              string dbName,
              string tableName
           )
        {
            return String.Format("SELECT Min(startTime) From {0}.dbo.{1}",
                                  SQLHelpers.CreateSafeDatabaseName(dbName),
                                  tableName);
        }

        //---------------------------------------------------------------
        // GetEarliestLogTimeQuery
        //---------------------------------------------------------------
        private string
           GetEarliestLogTimeQuery(
              string instance
           )
        {
            return String.Format("SELECT Min(eventTime) From {0} WHERE logSqlServer = {1}",
                                  CoreConstants.RepositoryChangeLogEventTable,
                                  SQLHelpers.CreateSafeString(instance));
        }

        //---------------------------------------------------------------
        // GetCreateSQLsecureLogTableStatement
        //---------------------------------------------------------------
        private string
           GetCreateSQLsecureLogTableStatement(
              string dbName
           )
        {
            return String.Format("CREATE TABLE {0}.dbo.{1} ( " +
                                 "[logId] [int] NOT NULL, " +
                                 "[eventTime] [datetime] NOT NULL ," +
                                 "[logType] [int] NOT NULL ," +
                                 "[logUser] [nvarchar] (256)," +
                                 "[logSqlServer] [nvarchar](256), " +
                                 "[logInfo] [nvarchar](MAX) )",
                                 SQLHelpers.CreateSafeDatabaseName(dbName),
                                 CoreConstants.RepositoryChangeLogEventTable);
        }

        //---------------------------------------------------------------
        // GetCreateDescriptionTableStatement
        //---------------------------------------------------------------
        private string
           GetCreateDescriptionTableStatement(
              string dbName
           )
        {
            return String.Format("CREATE TABLE {0}.dbo.{1} ( " +
                                  "[instance] [nvarchar] (256)," +
                                  "[displayName] [nvarchar] (512)," +
                                  "[description] [nvarchar] (256)," +
                                  "[databaseType] [nvarchar] (16) NULL, " +
                                  "[startDate] [datetime] ," +
                                  "[endDate] [datetime] , " +
                                  "[archiveBias] [int], " +
                                  "[archiveStandardBias] [int], " +
                                  "[archiveStandardDate] [datetime] ," +
                                  "[archiveDaylightBias] [int], " +
                                  "[archiveDaylightDate] [datetime]," +
                                  "[archiveTimeZoneName] [nvarchar] (128) , " +
                                  "[sqlComplianceDbSchemaVersion] [int], " +
                                  "[eventDbSchemaVersion] [int], " +
                                  "[bias] [int], " +
                                  "[standardBias] [int], " +
                                  "[standardDate] [datetime] ," +
                                  "[daylightBias] [int], " +
                                  "[daylightDate] [datetime]," +
                                  "[defaultAccess] [int], " +
                                  "[eventReviewNeeded] [tinyint], " +
                                  "[containsBadEvents] [tinyint], " +
                                  "[state] [int] NULL, " +
                                  "[timeLastIntegrityCheck] [datetime] ," +
                                  "[lastIntegrityCheckResult] [int], " +
                                  "[highWatermark] [int], " +
                                  "[lowWatermark] [int] )",
                                  SQLHelpers.CreateSafeDatabaseName(dbName),
                                  CoreConstants.RepositoryArchiveMetaTable);
        }

        //---------------------------------------------------------------
        // GetMoreLogsStatement
        //---------------------------------------------------------------
        private string
           GetMoreLogsStatement(
              string instance,
              DateTime beginTime,
              DateTime endTime
           )
        {
            return String.Format("SELECT count(logId) " +
                                  "FROM {0}.dbo.{1} " +
                                  "WHERE eventTime >= {2} " +
                                    "AND eventTime <  {3} " +
                                    "AND logSqlServer = {4}",
                                  CoreConstants.RepositoryDatabase,
                                  CoreConstants.RepositoryChangeLogEventTable,
                                  SQLHelpers.CreateSafeDateTime(beginTime),
                                  SQLHelpers.CreateSafeDateTime(endTime),
                                  SQLHelpers.CreateSafeString(instance));
        }



        //---------------------------------------------------------------
        // GetDeleteSQLsecureLogStatement
        //---------------------------------------------------------------
        private string
           GetDeleteSQLsecureLogStatement(
              string instance,
              DateTime beginTime,
              DateTime endTime)
        {
            return String.Format("DELETE {0}.dbo.{1} " +
                                  "FROM ( SELECT TOP {2} logId from {0}.dbo.{1} " +
                                                "WHERE eventTime >= {3} " +
                                                   "AND eventTime < {4} " +
                                                   "AND logSqlServer = {5} " +
                                                 "ORDER by logId) AS e " +
                                  "WHERE e.logId={0}.dbo.{1}.logId",
                                  SQLHelpers.CreateSafeDatabaseName(CoreConstants.RepositoryDatabase),
                                  CoreConstants.RepositoryChangeLogEventTable,
                                  CoreConstants.archiveBatchSize,
                                  SQLHelpers.CreateSafeDateTime(beginTime),
                                  SQLHelpers.CreateSafeDateTime(endTime),
                                  SQLHelpers.CreateSafeString(instance));
        }

        //---------------------------------------------------------------
        // GetArchiveDatabasesStatement
        //---------------------------------------------------------------
        static internal string
           GetArchiveDatabasesStatement()
        {
            return String.Format("SELECT databaseName FROM {0}.dbo.{1} " +
                                  "WHERE databaseType='{2}'",
                                  SQLHelpers.CreateSafeDatabaseName(CoreConstants.RepositoryDatabase),
                                  CoreConstants.RepositorySystemDatabaseTable,
                                  CoreConstants.DatabaseType_Archive);

        }

        //---------------------------------------------------------------
        // GetArchivingInstancesStatement
        //---------------------------------------------------------------
        private string
           GetArchivingInstancesStatement()
        {
            return String.Format("SELECT instance FROM {0}.dbo.{1} " +
                                  "WHERE isAuditedServer = 1",
                                  SQLHelpers.CreateSafeDatabaseName(CoreConstants.RepositoryDatabase),
                                  CoreConstants.RepositoryServerTable);

        }

        //---------------------------------------------------------------
        // GetUpdateStatusStatement
        //---------------------------------------------------------------
        private string
           GetUpdateStatusStatement()
        {
            return String.Format("UPDATE {0} SET archiveLastTime = GETUTCDATE()",
                                    CoreConstants.RepositoryConfigurationTable);
        }


        //---------------------------------------------------------------
        // GetInsertDescriptionRecordStmt
        //---------------------------------------------------------------
        private string
           GetInsertDescriptionRecordStmt()
        {
            return "INSERT INTO {0}.dbo.{1} " +
                   "(instance,displayName,description,startDate,endDate," +
                       "archiveBias,archiveStandardBias,archiveStandardDate,archiveDaylightBias,archiveDaylightDate,archiveTimeZoneName," +
                       "sqlComplianceDbSchemaVersion,eventDbSchemaVersion," +
                       "bias,standardBias,standardDate,daylightBias,daylightDate," +
                       "defaultAccess, databaseType) " +
                   "VALUES( {2}, {3}, {4}, {5}, {6}, " +
                            "{7}, {8}, {9}, {10}, {11}, {12}," +
                            "{13}, {14}, " +
                            "{15}, {16}, {17}, {18}, {19}," +
                            "{20}, '{21}' )";
        }

        //---------------------------------------------------------------
        // GetLastTimeStatement
        //---------------------------------------------------------------
        private string
           GetLastTimeStatement()
        {
            return String.Format("SELECT archiveLastTime FROM {0}",
                                  CoreConstants.RepositoryConfigurationTable);
        }

        private string
           GetUpdateArchiveTimeSpanStatement(string database)
        {
            return
               String.Format(
                  "DECLARE @end DateTime; SELECT @end=MAX(startTime) From {0}.dbo.{1};" +
                  "UPDATE {0}.dbo.{2} SET endDate=@end WHERE endDate < @end",
                  SQLHelpers.CreateSafeDatabaseName(database),
                  CoreConstants.RepositoryEventsTable,
                  CoreConstants.RepositoryArchiveMetaTable);
        }

        #endregion
    }

    [Serializable]
    public class ArchiveSettings : ISerializable
    {
        private int _days;
        private IntegrityCheckAction _icAction = IntegrityCheckAction.UseDefault;
        private string _prefix;
        private int _period;
        private string _timeZoneName;
        private bool _background;
        private string _targetInstance; // "" means all instances
        private string _user;
        private int _batchSize;
        private CMCommandResult _results;

        private int _classVersion = CoreConstants.SerializationVersion;

        public ArchiveSettings()
        {
            _days = -1;
            _prefix = "";
            _period = -1;
            _timeZoneName = "";
            _background = false;
            _user = "";
            _batchSize = CoreConstants.archiveBatchSize;
        }

        public bool UseArchiveDays
        {
            get { return _days != -1; }
        }
        public int ArchiveDays
        {
            get { return _days; }
            set { _days = value; }
        }

        public bool UseIntegrityCheckAction
        {
            get { return _icAction != IntegrityCheckAction.UseDefault; }
        }

        public IntegrityCheckAction IntegrityCheckAction
        {
            get { return _icAction; }
            set { _icAction = value; }
        }

        public bool UsePrefix
        {
            get { return (_prefix != null && _prefix.Length > 0); }
        }

        public string Prefix
        {
            get { return _prefix; }
            set { _prefix = value; }
        }

        public bool UseArchivePeriod
        {
            get { return _period != -1; }
        }

        public int ArchivePeriod
        {
            get { return _period; }
            set { _period = value; }
        }

        public bool UseTimeZoneName
        {
            get { return (_timeZoneName != null && _timeZoneName.Length > 0); }
        }

        public string TimeZoneName
        {
            get { return _timeZoneName; }
            set { _timeZoneName = value; }
        }

        public bool Background
        {
            get { return _background; }
            set { _background = value; }
        }

        public string TargetInstance
        {
            get { return _targetInstance; }
            set { _targetInstance = (value == null) ? "" : value; }
        }

        public string User
        {
            get { return _user; }
            set { _user = value; }
        }

        public int BatchSize
        {
            get { return _batchSize; }
            set { _batchSize = value; }
        }

        public CMCommandResult ArchiveResults
        {
            get { return _results; }
            set { _results = value; }
        }

        #region Serialization

        public ArchiveSettings(SerializationInfo info, StreamingContext context)
        {
            try
            {
                _classVersion = info.GetInt32("classVersion");
                _icAction = (IntegrityCheckAction)info.GetInt32("icAction");
                _days = info.GetInt32("days");
                _period = info.GetInt32("period");
                _prefix = info.GetString("prefix");
                _timeZoneName = info.GetString("timeZoneName");
                _background = info.GetBoolean("background");
                _targetInstance = info.GetString("targetInstance");
                _user = info.GetString("user");
                _batchSize = info.GetInt32("batchSize");
                _results = (CMCommandResult)info.GetValue("archiveResults", typeof(CMCommandResult));
            }
            catch (Exception e)
            {
                SerializationHelper.ThrowDeserializationException(e, GetType());
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            try
            {
                info.AddValue("classVersion", _classVersion);

                // 2.1 fields
                info.AddValue("icAction", _icAction);
                info.AddValue("days", _days);
                info.AddValue("period", _period);
                info.AddValue("prefix", _prefix);
                info.AddValue("timeZoneName", _timeZoneName);
                info.AddValue("background", _background);
                info.AddValue("targetInstance", _targetInstance);
                info.AddValue("user", _user);
                info.AddValue("batchSize", _batchSize);
                info.AddValue("archiveResults", _results);
            }
            catch (Exception e)
            {
                SerializationHelper.ThrowSerializationException(e, GetType());
            }
        }

        #endregion Serialization

        private void SetTimeZone(ArchiveJob job)
        {
            TimeZoneInfo[] tzs = TimeZoneInfo.GetSystemTimeZones();

            foreach (TimeZoneInfo tz in tzs)
            {
                if (String.Equals(tz.TimeZoneStruct.StandardName, _timeZoneName))
                {
                    job.ArchiveTimeZone = tz.TimeZoneStruct;
                    return;
                }
            }
            throw new Exception(String.Format("Archive Exception:  Unable to load timezone:  {0}", _timeZoneName));
        }

        //
        // StartArchive()
        //
        // This is the threaded procedure for performing an archive
        //
        public void StartArchive()
        {
            ArchiveJob aj;

            ArchiveJob.UpdateArchiveList();

            using (aj = new ArchiveJob())
            {
                // fake impersonation
                aj.archiveUser = _user;
                if (UseArchiveDays)
                    aj.ArchiveAge = _days;
                if (UseArchivePeriod)
                    aj.ArchivePeriod = _period;
                if (UsePrefix)
                    aj.ArchivePrefix = _prefix;
                if (UseTimeZoneName)
                    SetTimeZone(aj);
                if (UseIntegrityCheckAction)
                    aj.IntegrityCheckAction = _icAction;

                aj.CalculateArchiveEndTime();
                try
                {
                    if (_targetInstance == "")
                        _results = aj.Archive(); // all instances
                    else
                        _results = aj.Archive(_targetInstance); // one instance
                }
                catch (Exception e)
                {
                    _results = new CMCommandResult(e);
                }
            }
        }
    }
}
