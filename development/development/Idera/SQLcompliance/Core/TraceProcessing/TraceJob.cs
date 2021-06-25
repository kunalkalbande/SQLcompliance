using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Idera.SQLcompliance.Core.Event;
using Idera.SQLcompliance.Core.Rules;
using Idera.SQLcompliance.Core.Rules.Alerts;
using Idera.SQLcompliance.Core.Stats;
using Idera.SQLcompliance.Core.TSqlParsing;
using Microsoft.Data.Schema.ScriptDom;
using Microsoft.Data.Schema.ScriptDom.Sql;
using PluginCommon;
using IsolationLevel = System.Data.IsolationLevel;
using TimeZoneInfo = Idera.SQLcompliance.Core.TimeZoneHelper.TimeZoneInfo;
using Idera.SQLcompliance.Core.Agent;
using Idera.SQLcompliance.Core.Settings;
using System.Text.RegularExpressions;
using Idera.SQLcompliance.Core.Remoting;
using System.Globalization;

namespace Idera.SQLcompliance.Core.TraceProcessing
{
    /// <summary>
    /// Summary description for TraceJob.
    /// </summary>
    internal class TraceJob : TraceRow
    {
        #region Enums / Constants

        internal const int ndxEventClass = 0;
        internal const int ndxEventSubclass = 1;
        internal const int ndxStartTime = 2;
        internal const int ndxSpid = 3;
        internal const int ndxApplicationName = 4;
        internal const int ndxHostName = 5;
        internal const int ndxServerName = 6;
        internal const int ndxLoginName = 7;
        internal const int ndxSuccess = 8;
        internal const int ndxDatabaseName = 9;
        internal const int ndxDatabaseId = 10;
        internal const int ndxDbUserName = 11;
        internal const int ndxObjectType = 12;
        internal const int ndxObjectName = 13;
        internal const int ndxObjectId = 14;
        internal const int ndxPermissions = 15;
        internal const int ndxColumnPermissions = 16;
        internal const int ndxTargetLoginName = 17;
        internal const int ndxTargetUserName = 18;
        internal const int ndxRoleName = 19;
        internal const int ndxOwnerName = 20;
        internal const int ndxTextData = 21;
        internal const int ndxNestLevel = 22;

        // new SQL Server 2005 columns
        internal const int ndxFileName = 23;
        internal const int ndxLinkedServerName = 24;
        internal const int ndxParentName = 25;
        internal const int ndxIsSystem = 26;
        internal const int ndxSessionLoginName = 27;
        internal const int ndxProviderName = 28;

        // Added since 3.1 (SQL 2005 only)
        internal const int ndxEventSequence = 29;

        //Added since 5.5 (SQL Server 2008 or later version)
        internal const int ndxRowCounts = 30;

        // marks the end of trace columns
        internal const int ndxCalculatedStart = ndxRowCounts + 1;

        // calculated columns      
        internal const int ndxTargetObject = ndxCalculatedStart;
        internal const int ndxDetails = ndxCalculatedStart + 1;
        internal const int ndxEventType = ndxCalculatedStart + 2;
        internal const int ndxEventCategory = ndxCalculatedStart + 3;
        internal const int ndxHash = ndxCalculatedStart + 4;
        internal const int ndxDeleteme = ndxCalculatedStart + 5;
        internal const int ndxAlertLevel = ndxCalculatedStart + 6;
        internal const int ndxChecksum = ndxCalculatedStart + 7;
        internal const int ndxPrivilegedUser = ndxCalculatedStart + 8;
        internal const int ndxNewRow = ndxCalculatedStart + 9;
        internal const int ndxKeepSql = ndxCalculatedStart + 10;
        internal const int ndxProcessed = ndxCalculatedStart + 11;
        internal const int ndxStartSequence = ndxCalculatedStart + 12;
        internal const int ndxEndSequence = ndxCalculatedStart + 13;
        internal const int ndxEndTime = ndxCalculatedStart + 14;
        internal const int ndxGUId = ndxCalculatedStart + 15;

        // event correlation states
        internal const int state_None = 0; // start state
        internal const int state_InCreate = 1; // DDL - seen AuditStatementPermission (Create)
        internal const int state_InDrop = 2; // DDL - seen AuditStatementPermission (Drop)
        internal const int state_InDmlSql = 3; // DML/SELECT - seen SQLStmt:Starting
        internal const int state_InDml = 4; // Seen AuditObjectPermission (no SqlStmt:Starting)
        internal const int state_Finalize = 5; // write the current stateRecord
        internal const int state_Skip = 6; // dont write the current stateRecord
        internal const int state_InSp = 7; // See SP:Starting
        internal const int state_WriteWithinSql = 8; // Found a AuditObjectPerm while in InDmlSQl; write but stay inDmlSql
        internal const int state_InDmlDataChange = 9; // Seen AuditObjectPermission and table is monitored for data changes


        // Mapping table for SQL 2005 event types to SQLcompliance event type offsets


        // There could be gaps in tracing, we wont correlate events 
        // who dont fall within a minute of each other
        internal const int max_CorrelateTime = 2;

        internal int[,] traceEventCount = new int[200, 4];
        private static object lookupLock = new object();
        private static Hashtable typeOffsetTable = new Hashtable();
        private static bool offsetTableInitialized = false;
        private static List<DataColumn> eventsCols = new List<DataColumn>();
        private static List<DataColumn> eventSqlCols = new List<DataColumn>();
        private static List<DataColumn> dataChangeCols = new List<DataColumn>();
        private static List<DataColumn> colChangeCols = new List<DataColumn>();
        private static List<DataColumn> sensitiveColumnCols = new List<DataColumn>();

        private int rowsWritten = 0;
        private int insertBatchSize = 5000;
        private int temp_m_state = 0;

        private void IncrementTraceEventCount(int ndx, int ndx2)
        {
            if (ndx < (int)TraceEventId.EventIdMin || ndx > (int)TraceEventId.EventIdMax)
                traceEventCount[199, ndx2]++;
            else
                traceEventCount[ndx, ndx2]++;
        }

        private void DumpTraceEventCount()
        {
            if (ErrorLog.Instance.ErrorLevel < ErrorLog.Level.Verbose) return;

            bool atLeastOne = false;

            StringBuilder s = new StringBuilder(1024);

            s.AppendFormat("Counts per Trace Event ID - Trace File: {0}", jobInfo.traceFile);
            for (int i = 0; i < 200; i++)
            {
                if (traceEventCount[i, 0] != 0)
                {
                    if (!atLeastOne)
                    {
                        s.Append("\nID\tINS\tDEL\tFIL");
                        atLeastOne = true;
                    }

                    s.AppendFormat("\n{0}\t{1}\t{2}\t{3}\t{4}",
                                   i,
                                   traceEventCount[i, 0],
                                   traceEventCount[i, 1],
                                   traceEventCount[i, 2],
                                   traceEventCount[i, 3]);
                }
            }

            if (!atLeastOne)
            {
                s.AppendFormat("\n   No events to process in this trace file");
            }
            ErrorLog.Instance.Write(ErrorLog.Level.Verbose, s.ToString());
        }

        private const string BEFORE_DATA_COLUMN_NAME = "beforeValue";
        private const string AFTER_DATA_COLUMN_NAME = "afterValue";
        #endregion

        #region Properties
        //------------------------------------------------------------------------
        // Properties
        //------------------------------------------------------------------------

        //private static Object traceWriteLock = new Object();
        private static Object traceFileReadLock = new Object();

        private HashChain chain;

        private string eventsTable;
        private int currentId;

        DataSet tempEventsDataSet;
        DataTable tmpTable; // Used by data change trace

        // input parameters
        internal TraceJobInfo jobInfo;

        bool processCurrent = false;
        DataRow workingRow = null;

        LoginFilter loginCache = null;

        TimeZoneInfo timeZoneInfo = new TimeZoneInfo();

        #endregion

        #region Constructor

        protected
            TraceJob()
        {
            jobInfo = null;

            eventsTable = null;
        }

        public
            TraceJob(
            TraceJobInfo inJobInfo
            )
        {
            jobInfo = inJobInfo;

            // mark job as server start stop trace job
            if (jobInfo.traceFile.Contains("_1_998_"))
                jobInfo.traceCategory = (int)TraceCategory.ServerStartStop;

            eventsTable = jobInfo.tempTablePrefix + CoreConstants.Repository_EventsSuffix;

            chain = new HashChain(jobInfo.instance);
        }

        #endregion

        #region Job Entrypoint

        //-----------------------------------------------------------------------
        // Start - Entry point for trace file processing job
        //                     Processes temporary trace file and loads
        //                     events, sqlText
        //-----------------------------------------------------------------------
        public void
            Start()
        {
            bool aborting = false;
            bool failed = false;
            try
            {
                int readStart = Environment.TickCount;
                int stage1;
                int stage2;
                int goodEvents;

                if (jobInfo.GetAborting())
                {
                    aborting = true;
                    return;
                }

                // v5.0 added processing of 
                if (jobInfo.traceLevel == (int)TraceLevel.Server &&
                    jobInfo.traceCategory == (int)TraceCategory.ServerStartStop)
                {
                    ProcessDefaultTrace();
                    return;
                }

                // V3.1 added a new data change trace with different columns
                //
                if (jobInfo.traceCategory == (int)TraceCategory.DataChange &&
                    jobInfo.traceLevel == (int)TraceLevel.Table)
                {
                    if (ProcessDataChangeTrace())
                    {
                        if (AlertingJobPool.DoBADAlertProcessing)
                        {
                            AlertingJobPool.SignalNewEventsAvailable();
                        }
                    }
                    return;
                }

                // V3.5 - Process the Sensitive Column Trace
                if ((jobInfo.traceCategory == (int)TraceCategory.SensitiveColumn ||
                    jobInfo.traceCategory == (int)TraceCategory.SensitiveColumnwithSelect) &&
                    jobInfo.traceLevel == (int)TraceLevel.Table)
                {
                    ProcessSensitiveColumnTrace();
                    return;
                }


                LoadDataSet();
                stage1 = Environment.TickCount;

                if (jobInfo.GetAborting())
                {
                    aborting = true;
                    return;
                }

                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                         "Start TraceJob:Start Phase 3");

                goodEvents = ProcessRows();
                stage2 = Environment.TickCount;
                if (jobInfo.GetAborting())
                {
                    aborting = true;
                    return;
                }

                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                         "Start TraceJob:Start Phase 4");


                if (goodEvents != 0)
                {
                    lock (AcquireInstanceLock(jobInfo.instance))
                    {
                        WriteFinalEventTable(goodEvents,
                                              tempEventsDataSet.Tables["Events"]);
                    }

                    if (jobInfo.traceCategory == (int)TraceCategory.DML ||
                       jobInfo.traceCategory == (int)TraceCategory.DMLwithDetails)
                    {
                        UpdateDataChangeIds(jobInfo.firstEventTime, jobInfo.lastEventTime);
                    }
                    AlertingJobPool.SignalNewEventsAvailable();
                }

                if (jobInfo.GetAborting())
                {
                    aborting = true;
                    return;
                }


                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         String.Format(
                                            "Trace: {0}\n  Events: {1}\n  Uncompress time: {2} ms\n  Read time: {3} ms\n  Process time: {4} ms\n  Write time: {5} ms",
                                            jobInfo.traceFile,
                                            goodEvents,
                                            jobInfo.uncompressTime,
                                            stage1 - readStart,
                                            stage2 - stage1,
                                            Environment.TickCount - stage2));
            }
            catch (Exception ex)
            {
                failed = true;
                ErrorLog.Instance.Write("TraceJob::Start",
                                         String.Format(
                                            CoreConstants.Exception_ErrorProcessingTraceFile,
                                            jobInfo.traceFile),
                                         ex,
                                         true);
                throw;
            }
            finally
            {
                if (!aborting && !failed &&
                     ErrorLog.Instance.ErrorLevel >= ErrorLog.Level.Debug)
                {
                    ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                             String.Format(CoreConstants.Debug_EventCounts,
                                                            jobInfo.compressedTraceFile,
                                                            jobInfo.eventsTotal,
                                                            jobInfo.eventsUpdated,
                                                            jobInfo.eventsInserted,
                                                            jobInfo.eventsSQLcm,
                                                            jobInfo.eventsOverlap,
                                                            jobInfo.eventsFilteredOut,
                                                            jobInfo.eventsDeleted));
                    DumpTraceEventCount();
                }

                if (tempEventsDataSet != null)
                {
                    foreach (DataTable t in tempEventsDataSet.Tables)
                    {
                        foreach (DataColumn d in t.Columns)
                            d.Dispose();
                        t.Dispose();
                    }
                    tempEventsDataSet.Dispose();
                }
            }
        }

        //-----------------------------------------------------------------------
        // AcquireInstanceLock
        //-----------------------------------------------------------------------

        private static object
         AcquireInstanceLock(
            string instance
         )
        {
            string upperInstance = instance.ToUpper();

            lock (CoreConstants.AcquireInstanceLock)
            {
                if (CoreConstants.InstanceLocks.Contains(upperInstance))
                {
                    return CoreConstants.InstanceLocks[upperInstance];
                }
                else
                {
                    object syncObj = new object();
                    try
                    {
                        CoreConstants.InstanceLocks.Add(upperInstance, syncObj);
                    }
                    catch (ArgumentException)
                    {
                        // ignore duplicate key error
                        syncObj = CoreConstants.InstanceLocks[upperInstance];
                    }

                    return syncObj;
                }
            }
        }

        public void
            StartXE()
        {
            bool aborting = false;
            bool failed = false;
            try
            {
                int readStart = Environment.TickCount;
                int stage1;
                int stage2;
                int goodEvents;

                if (jobInfo.GetAborting())
                {
                    aborting = true;
                    return;
                }

                if (jobInfo.traceCategory == (int)TraceCategory.DataChange &&
                   jobInfo.traceLevel == (int)TraceLevel.Table)
                {
                    if (ProcessDataChangeTraceXE())
                    {
                        if (AlertingJobPool.DoBADAlertProcessing)
                            AlertingJobPool.SignalNewEventsAvailable();
                    }
                    return;
                }

                if ((jobInfo.traceCategory == (int)TraceCategory.SensitiveColumn ||
                   jobInfo.traceCategory == (int)TraceCategory.SensitiveColumnwithSelect) &&
                   jobInfo.traceLevel == (int)TraceLevel.Table)
                {
                    ProcessSensitiveColumnTraceXE();
                    return;
                }

                if (jobInfo.GetAborting())
                {
                    aborting = true;
                    return;
                }



                stage1 = Environment.TickCount;
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                         "Start TraceJob:Start Phase 3");


                DataTable table = new DataTable();
                AddTableColumnXE(table);
                goodEvents = ProcessRowsXE(table);
                stage2 = Environment.TickCount;
                if (jobInfo.GetAborting())
                {
                    aborting = true;
                    return;
                }

                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                         "Start TraceJob:Start Phase 4");

                if (goodEvents != 0)
                {
                    lock (AcquireInstanceLock(jobInfo.instance))
                    {
                        WriteFinalEventTable(goodEvents, table);
                    }

                    if (jobInfo.traceCategory == (int)TraceCategory.DML ||
                        jobInfo.traceCategory == (int)TraceCategory.DMLwithDetails
                        || jobInfo.traceCategory == (int)TraceCategory.DBPrivilegedUsers
                        || (jobInfo.traceCategory == (int)TraceCategory.ServerTrace && jobInfo.privDML))
                    {
                        UpdateDataChangeIds(jobInfo.firstEventTime, jobInfo.lastEventTime);
                    }
                    
                    AlertingJobPool.SignalNewEventsAvailable();
                }
            }
            catch (Exception ex)
            {
                failed = true;
                ErrorLog.Instance.Write("TraceJob::Start",
                                         String.Format(
                                            CoreConstants.Exception_ErrorProcessingTraceFile,
                                            jobInfo.traceFile),
                                         ex,
                                         true);
                throw;
            }


            finally
            {
                if (!aborting && !failed &&
                     ErrorLog.Instance.ErrorLevel >= ErrorLog.Level.Debug)
                {
                    ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                             String.Format(CoreConstants.Debug_EventCounts,
                                                            jobInfo.compressedTraceFile,
                                                            jobInfo.eventsTotal,
                                                            jobInfo.eventsUpdated,
                                                            jobInfo.eventsInserted,
                                                            jobInfo.eventsSQLcm,
                                                            jobInfo.eventsOverlap,
                                                            jobInfo.eventsFilteredOut,
                                                            jobInfo.eventsDeleted));
                    DumpTraceEventCount();
                }
            }
        }



        //-------------------------------------------------------------------------
        // LoadDataSet - Load data from temporary table into in memory DataSet
        //               for processing
        //-------------------------------------------------------------------------
        private void LoadDataSet()
        {
            Repository rep = new Repository();

            try
            {
                rep.OpenConnection();

                // select cmd
                //sqlText = GetSelectSQL( eventsTable );
                string sqlText = GetLoadSQL(jobInfo.traceFile);
                using (SqlCommand selectCmd = new SqlCommand(sqlText, rep.connection))
                {
                    selectCmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    selectCmd.CommandType = CommandType.Text;

                    // SqlDataAdapter
                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter())
                    {
                        dataAdapter.TableMappings.Add("Table", "Events");

                        dataAdapter.SelectCommand = selectCmd;

                        // Load DataSet  
                        tempEventsDataSet = new DataSet("Trace");

                        try
                        {
                            // there is some anecdotal evidence on the web to indicate that 
                            // fn_trace_gettable is not threadsafe so we will just put a lock
                            // around it
                            //lock (traceFileReadLock)
                            {
                                dataAdapter.Fill(tempEventsDataSet);
                            }
                        }
                        catch (SqlException sqlEx)
                        {
                            // workaround - for some rollover files, SQL Server 2000 creates files
                            //              that fn_trace_gettable throws error 568 on - what we do
                            //              is just gnore this error - it seems to be a problem
                            //              at the end of the file and not with the real data


                            if (sqlEx.Number == 567)
                            {
                                // cant read file or it dosnt exist - usually caused by SQl Server not having permission to read files in that directory
                                string msg =
                                   String.Format(CoreConstants.Exception_CantReadTraceFile,
                                                  jobInfo.traceFile);

                                ErrorLog.Instance.Write(msg);
                                throw new Exception(msg);
                            }
                            else if (sqlEx.Number != 568)
                            {
                                ErrorLog.Instance.Write(
                                   String.Format(
                                      "LoadDataSet: Number: Error: {0} Level : {1} State : {2}",
                                      sqlEx.Number,
                                      sqlEx.Class,
                                      sqlEx.State),
                                   sqlEx);
                                throw;
                            }
                        }
                    }
                }

                //-------------------------------------------------------            
                // Add columns to loaded DataRow for processing purposes
                //-------------------------------------------------------            
                DataTable table = tempEventsDataSet.Tables["Events"];

                // add dummy columns for SQL Server 2005 columns that dont exist in 2000
                if (!jobInfo.isSqlServer2005)
                {
                    table.Columns.Add("fileName", Type.GetType("System.String"));
                    table.Columns.Add("linkedServerName", Type.GetType("System.String"));
                    table.Columns.Add("parentName", Type.GetType("System.String"));
                    table.Columns.Add("isSystem", Type.GetType("System.Int32"));
                    table.Columns.Add("sessionLoginName", Type.GetType("System.String"));
                    table.Columns.Add("providerName", Type.GetType("System.String"));
                    table.Columns.Add("eventSequence", Type.GetType("System.Int64"));
                }
                else if (!jobInfo.SupportsBeforeAfter())
                {
                    // traces from agents before 3.1 and instance is 2005 or later
                    table.Columns.Add("eventSequence", Type.GetType("System.Int64"));
                }

                if (!table.Columns.Contains("RowCounts"))
                    table.Columns.Add("RowCounts", Type.GetType("System.Int64"));
                // add calculated columns
                table.Columns.Add("targetObject", Type.GetType("System.String"));
                table.Columns.Add("details", Type.GetType("System.String"));
                table.Columns.Add("eventType", Type.GetType("System.Int32"));
                table.Columns.Add("eventCategory", Type.GetType("System.Int32"));
                table.Columns.Add("hash", Type.GetType("System.Int32"));
                table.Columns.Add("deleteMe", Type.GetType("System.Int32"));
                table.Columns.Add("alertLevel", Type.GetType("System.Int32"));
                table.Columns.Add("checksum", Type.GetType("System.Int32"));
                table.Columns.Add("privilegedUser", Type.GetType("System.Int32"));
                table.Columns.Add("newRow", Type.GetType("System.Int32"));
                table.Columns.Add("keepSql", Type.GetType("System.Int32"));
                table.Columns.Add("processed", Type.GetType("System.Int32"));
                table.Columns.Add("startSequence", Type.GetType("System.Int64"));
                table.Columns.Add("endSequence", Type.GetType("System.Int64"));
                table.Columns.Add("endTime", Type.GetType("System.DateTime"));
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write("LoadDataSet",
                                         String.Format(
                                            CoreConstants.Exception_CantLoadTraceFile,
                                            jobInfo.instance),
                                         ex);
                throw;
            }
            finally
            {
                rep.CloseConnection();
            }
        }

        #endregion

        #region Constants

        private const int ON = 1;

        #endregion

        #region ProcessRows

        //-------------------------------------------------------------------------
        // ProcessRows - Walk rows in loaded trace file table and process each row
        //-------------------------------------------------------------------------
        private int
            ProcessRows()
        {
            int goodRows = 0;
            bool preoverlap = true;
            bool overlapped = false;
            TimesRecord lastTrace = null;
            DataTable tableNewRows = null;

            try
            {
                bool emptyTrace = (tempEventsDataSet.Tables[0].Rows.Count == 0);
                bool dumpStateTable = false;

                StateTable stateTable;
                stateTable = StateTable.Read(jobInfo.instance,
                                              jobInfo.traceType);
                if (emptyTrace)
                {
                    dumpStateTable = true;
                }
                else
                {
                    tableNewRows = tempEventsDataSet.Tables[0].Clone();

                    // Get first and last times in current trace file
                    int i = 0;
                    while (jobInfo.firstEventTime == DateTime.MinValue && i < tempEventsDataSet.Tables[0].Rows.Count)
                    {
                        jobInfo.firstEventTime =
                           GetRowDateTime(tempEventsDataSet.Tables[0].Rows[i],
                                          ndxStartTime);
                        i++;
                    }
                    i = tempEventsDataSet.Tables[0].Rows.Count - 1;
                    while (jobInfo.lastEventTime == DateTime.MinValue && i >= 0)
                    {
                        jobInfo.lastEventTime =
                           GetRowDateTime(tempEventsDataSet.Tables[0].Rows[i],
                                          ndxStartTime);
                        i--;
                    }

                    // get last trace processed times
                    lastTrace = TimesRecord.Read(jobInfo.instance,
                                                  jobInfo.traceType);


                    // check if current trace file is out of sequence with last trace file
                    if (OutOfSequence(lastTrace.StartTime,
                                        lastTrace.EndTime,
                                        jobInfo.firstEventTime,
                                        jobInfo.lastEventTime))
                    {
                        dumpStateTable = true;
                    }
                    else
                    {
                        overlapped = Overlapped(lastTrace.StartTime,
                                                 lastTrace.EndTime,
                                                 jobInfo.firstEventTime,
                                                 jobInfo.lastEventTime);
                    }
                }

                if (dumpStateTable)
                {
                    for (int i = 0; i < stateTable.m_records.Count; i++)
                    {
                        StateRecord sr = (StateRecord)stateTable.m_records[i];

                        if (sr.m_flushed == 0)
                        {
                            // Add new row to dataset
                            DataRow newRow = tempEventsDataSet.Tables[0].NewRow();
                            PopulateRow(newRow, sr);
                            tempEventsDataSet.Tables[0].Rows.Add(newRow);
                            if (FinalizeRow(newRow))
                                goodRows++;
                        }

                        ((StateRecord)stateTable.m_records[i]).m_state = state_None;
                    }
                }

                // Login events are only recorded in server or priv user
                // read login cache if we are processing one of these
                if (jobInfo.traceLevel == (int)TraceLevel.Server
                     || jobInfo.traceLevel == (int)TraceLevel.User)
                {
                    bool loginCollapse;
                    int loginTimespan;
                    int loginCacheSize;

                    // get login filtering options
                    SQLcomplianceConfiguration.GetLoginFilterOptions(out loginCollapse,
                                                                      out loginTimespan,
                                                                      out loginCacheSize);
                    // initialize login cache
                    loginCache = new LoginFilter(jobInfo.instance,
                                                  loginCollapse,
                                                  loginTimespan,
                                                  loginCacheSize);
                }
                else
                {
                    loginCache = null;
                }
                //SQLCM 3789
                Dictionary<int, string> agentDbList = null;
                //---------------      
                // Walk the Rows
                //---------------      
                int rowIndex = -1;
                int currentObjEventIndex = -1;
                List<DataRow> objEventList = new List<DataRow>();
                foreach (DataRow row in tempEventsDataSet.Tables[0].Rows)
                {
                    rowIndex++;

                    // skip new rows added during state processing 
                    if (1 == GetRowInt32(row, ndxProcessed)) continue;

                    if (row == null) continue;

                    if ((row[ndxDatabaseName] == null || row[ndxDatabaseName].ToString() == string.Empty)
                        && (row[ndxDatabaseId] != null && row[ndxDatabaseId].ToString() != string.Empty))
                    {
                        if (agentDbList != null)
                        {
                            row[ndxDatabaseName] = (agentDbList.ContainsKey((int)row[ndxDatabaseId]) ? agentDbList[(int)row[ndxDatabaseId]] : "");
                        }
                        else
                        {
                            agentDbList = GetDatabaseListWithId();
                            row[ndxDatabaseName] = (agentDbList.ContainsKey((int)row[ndxDatabaseId]) ? agentDbList[(int)row[ndxDatabaseId]] : "");
                        }
                    }

                    // mark all rows that get here deleted until we know better 
                    row[ndxDeleteme] = 1;
                    row[ndxKeepSql] = jobInfo.keepingSql;

                    // Row Validation
                    if (!IsValidRow(row)) continue;

                    jobInfo.eventsTotal++;

                    // skip SQLcompliance internal processes early - we just eat all
                    // these events - other server side filtering done later
                    if (IsSQLcomplianceAction(row))
                    {
                        AddEventFilteredStats(GetRowDateTime(row, ndxStartTime));
                        jobInfo.eventsSQLcm++;
                        continue;
                    }

                    // some event preprocessing

                    // eat comments from sql - we no longer eat comments - users
                    //  use this to tag data (see Hangar Orthopedic case study)
                    string sqlText = GetRowString(row, ndxTextData);
                    row[ndxTextData] = sqlText;
                    string sqlTxt = sqlText.ToUpper();
                    long permissions = GetRowInt64(row, ndxPermissions);
                    //Skipping unwanted DML events.
                    if (permissions == 1
                        && (sqlTxt.StartsWith("UPDATE")
                            || sqlTxt.StartsWith("INSERT")
                            || sqlTxt.StartsWith("DELETE")
                            || (sqlTxt.StartsWith("EXEC") && !sqlTxt.StartsWith("EXECUTE AS")))
                        )
                        continue;

                    //Skipping unwanted SELECT events.
                    if ((permissions == 2 || permissions == 8
                            || permissions == 16 || permissions == 32)
                        && (sqlTxt.StartsWith("SELECT"))
                        )
                        continue;

                    // change success column - some events return NULL in this 
                    // column - this is equivalent to success=1 since it means
                    // we are doing a real event instead of permission event
                    if (row.IsNull(ndxSuccess))
                    {
                        row[ndxSuccess] = 1;
                    }

                    // skip overlap regions
                    if (preoverlap && overlapped)
                    {
                        if (
                           lastTrace.EndTime.CompareTo(GetRowDateTime(row, ndxStartTime)) >
                           0)
                        {
                            if ((TraceEventId)GetRowInt32(row, ndxEventClass) == TraceEventId.SqlStmtCompleted
                            || (TraceEventId)GetRowInt32(row, ndxEventClass) == TraceEventId.SpStmtCompleted)
                            {
                                UpdateRowCounts(row);
                            }
                            jobInfo.eventsOverlap++;
                            continue;
                        }
                        else
                        {
                            preoverlap = false;
                        }
                    }
                    bool isDCEndEvent = IsDataChangeEndEvent(row);
                    if (isDCEndEvent && currentObjEventIndex > -1 && IsDataChangeEvent(tempEventsDataSet.Tables[0].Rows[currentObjEventIndex]))
                    {
                        row[ndxStartSequence] = tempEventsDataSet.Tables[0].Rows[currentObjEventIndex][ndxStartSequence];
                        if (objEventList.Contains(tempEventsDataSet.Tables[0].Rows[currentObjEventIndex]))
                            objEventList.Remove(tempEventsDataSet.Tables[0].Rows[currentObjEventIndex]);
                        objEventList.Add(row);
                        currentObjEventIndex = -1;
                    }
                    else if((TraceEventId)GetRowInt32(row, ndxEventClass) == TraceEventId.AuditObjectPermission && !isDCEndEvent){
                        currentObjEventIndex = rowIndex;
                        row[ndxStartSequence] = row[ndxEventSequence];
                        objEventList.Add(row);
                    }


                    if ((TraceEventId)GetRowInt32(row, ndxEventClass) == TraceEventId.SqlStmtCompleted
                        || (TraceEventId)GetRowInt32(row, ndxEventClass) == TraceEventId.SpStmtCompleted)
                    {
                        bool rowMatched = false;
                        for(int i = 0;i < objEventList.Count;)
                        {
                            if (RowsMatched(objEventList[i], row))
                            {
                                objEventList[i][ndxTextData] = row[ndxTextData];
                                objEventList[i][ndxRowCounts] = row[ndxRowCounts];
                                objEventList.RemoveAt(i);                                
                                rowMatched = true;
                            }
                            else
                            {
                                i++;
                            }
                        }
                        if(!rowMatched)
                            UpdateRowCounts(row);
                    }

                    // if we have reached this point, then we know we have an event
                    // we need to pay some attention to so we load state for this spid
                    StateRecord stateRecord =
                       stateTable.GetRecord(GetRowInt32(row, ndxSpid));

                    IncrementTraceEventCount(GetRowInt32(row, ndxEventClass), 0);
                    temp_m_state = stateRecord.m_state;
                    //-----------------------------
                    // State Machine
                    //-----------------------------
                    if (stateRecord.m_state == state_None)
                    {
                        processCurrent = true;
                    }
                    else
                    {
                        // Intermediate State Handling
                        if (stateRecord.m_state == state_InCreate)
                        {
                            InCreateHandler(row, stateRecord);
                        }
                        else if (stateRecord.m_state == state_InDrop)
                        {
                            InDropHandler(row, stateRecord);
                        }
                        else if (stateRecord.m_state == state_InDmlSql)
                        {
                            InDmlSqlHandler(row, stateRecord);
                        }
                        else if (stateRecord.m_state == state_InDml)
                        {
                            InDmlHandler(row, stateRecord);
                        }
                        else if (stateRecord.m_state == state_InDmlDataChange)
                        {
                            InDmlDataChangeHandler(row, stateRecord);
                        }
                        else if (stateRecord.m_state == state_InSp)
                        {
                            InSpHandler(row, stateRecord);
                        }

                        // Post Intermediate State Handling
                        if (stateRecord.m_state == state_None)
                        {
                            int saveSpid = stateRecord.spid;
                            stateRecord.Clear();
                            stateRecord.spid = saveSpid;

                            processCurrent = true;
                        }
                        else if (stateRecord.m_state == state_WriteWithinSql)
                        {
                            // we found something worth writing while in state = InSql
                            // but we dont want to reset this state just yet
                            // use current row
                            stateRecord.CopyToRow(row);
                            workingRow = row;

                            if (FinalizeRow(workingRow)) goodRows++;

                            stateRecord.m_state = state_InDmlSql;
                        }
                        else if (stateRecord.m_state == state_Finalize)
                        {
                            if (processCurrent)
                            {
                                // Add new row to dataset
                                if (stateRecord.m_row != -1)
                                {
                                    // row exists in dataset
                                    stateRecord.CopyToRow(
                                       tempEventsDataSet.Tables[0].Rows[stateRecord.m_row]);
                                    workingRow =
                                       tempEventsDataSet.Tables[0].Rows[stateRecord.m_row];
                                }
                                else
                                {
                                    // need new row 
                                    if (tableNewRows == null)
                                        throw new Exception(
                                           "Trace processing error: tableNewRows is null.",
                                           new NullReferenceException("ProcessRows::tableNewRows"));
                                    DataRow newRow = tableNewRows.NewRow();
                                    PopulateRow(newRow, stateRecord);
                                    tableNewRows.Rows.Add(newRow);
                                    workingRow = newRow;
                                }
                            }
                            else
                            {
                                // use current row
                                stateRecord.CopyToRow(row);
                                workingRow = row;
                            }

                            if (FinalizeRow(workingRow)) goodRows++;

                            stateRecord.m_state = state_None;
                        }

                        if (stateRecord.m_state == state_Skip)
                        {
                            stateRecord.m_state = state_None;
                        }
                    }

                    if (processCurrent)
                    {
                        // in the NONE case we will either see
                        //   (1) Single row (just finalize)
                        //   (2) First row in multi-row event set (finalize later)
                        bool singleRowEvent = InNoneHandler(row, rowIndex, stateRecord);
                        if (singleRowEvent)
                        {
                            if (FinalizeRow(row)) goodRows++;
                        }
                    }

                    // Update state table            
                    stateTable.PutRecord(stateRecord);
                }

                //------------------------------------------------------------------------
                // Done processing rows - persist all the state information for next time
                //------------------------------------------------------------------------
                if (!emptyTrace)
                {
                    // dump extra rows into main dataset
                    foreach (DataRow srcRow in tableNewRows.Rows)
                    {
                        tempEventsDataSet.Tables[0].ImportRow(srcRow);
                    }

                    lastTrace.StartTime = jobInfo.firstEventTime;
                    lastTrace.EndTime = jobInfo.lastEventTime;
                    lastTrace.Write();
                }

                stateTable.Write();

                // Write and clear login cache
                if (loginCache != null)
                {
                    loginCache.WriteCache();
                    loginCache = null;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write("TraceJob::ProcessRows",
                                         ex,
                                         true);
                throw;
            }
            finally
            {
                if (tableNewRows != null)
                {
                    foreach (DataColumn d in tableNewRows.Columns)
                        d.Dispose();
                    tableNewRows.Dispose();
                }
            }

            return goodRows;
        }
        
        //5.4 XE
        private int
             ProcessRowsXE(DataTable table)
        {
            XEventData eventList = new XEventData(jobInfo.traceFile);
            int goodRows = 0;
            bool preoverlap = true;
            bool overlapped = false;
            TimesRecord lastTrace = null;
            DataTable tableNewRows = null;

            try
            {
                bool emptyTrace = (eventList.EventData.Count == 0);
                bool dumpStateTable = false;

                StateTable stateTable;
                stateTable = StateTable.Read(jobInfo.instance,
                                              jobInfo.traceType);
                if (emptyTrace)
                {
                    dumpStateTable = true;
                }
                else
                {
                    tableNewRows = table.Clone();

                    // Get first and last times in current trace file
                    int i = 0;
                    while (jobInfo.firstEventTime == DateTime.MinValue && i < eventList.EventData.Count)
                    {
                        jobInfo.firstEventTime =
                          timeZoneInfo.Convert(jobInfo.timeZoneInfo, eventList.EventData[i].EventTime);
                        i++;
                    }
                    i = eventList.EventData.Count - 1;
                    while (jobInfo.lastEventTime == DateTime.MinValue && i >= 0)
                    {
                        jobInfo.lastEventTime = timeZoneInfo.Convert(jobInfo.timeZoneInfo, eventList.EventData[i].EventTime).AddMilliseconds(100);
                        i--;
                    }

                    // get last trace processed times
                    lastTrace = TimesRecord.Read(jobInfo.instance,
                                                  jobInfo.traceType);


                    // check if current trace file is out of sequence with last trace file
                    if (OutOfSequence(lastTrace.StartTime,
                                        lastTrace.EndTime,
                                        jobInfo.firstEventTime,
                                        jobInfo.lastEventTime))
                    {
                        dumpStateTable = true;
                    }
                    else
                    {
                        overlapped = Overlapped(lastTrace.StartTime,
                                                 lastTrace.EndTime,
                                                 jobInfo.firstEventTime,
                                                 jobInfo.lastEventTime);
                    }
                }

                if (dumpStateTable)
                {
                    for (int i = 0; i < stateTable.m_records.Count; i++)
                    {
                        StateRecord sr = (StateRecord)stateTable.m_records[i];
                        if (sr.m_flushed == 0)
                        {
                            // Add new row to dataset
                            DataRow newRow = table.NewRow();
                            PopulateRow(newRow, sr);
                            if (IsDataChangeEvent(newRow))
                            {
                                newRow[ndxEndSequence] = 0;
                            }
                            table.Rows.Add(newRow);
                            if (FinalizeRow(newRow))
                                goodRows++;
                        }

                        ((StateRecord)stateTable.m_records[i]).m_state = state_None;
                    }
                }

                // Login events are only recorded in server or priv user
                // read login cache if we are processing one of these
                if (jobInfo.traceLevel == (int)TraceLevel.Server
                     || jobInfo.traceLevel == (int)TraceLevel.User)
                {
                    bool loginCollapse;
                    int loginTimespan;
                    int loginCacheSize;

                    // get login filtering options
                    SQLcomplianceConfiguration.GetLoginFilterOptions(out loginCollapse,
                                                                      out loginTimespan,
                                                                      out loginCacheSize);
                    // initialize login cache
                    loginCache = new LoginFilter(jobInfo.instance,
                                                  loginCollapse,
                                                  loginTimespan,
                                                  loginCacheSize);
                }
                else
                {
                    loginCache = null;
                }

                //---------------      
                // Walk the Rows
                //---------------      
                int rowIndex = -1;
                int rowCount = 0;
                DateTime flag;
                foreach (XEventSingleEvent eventRow in eventList.EventData)
                {
                    if (eventRow == null) continue;

                    if (!eventRow.EventData.ContainsKey("sql_text")) continue;
                    //this needs to be set first.
                    DataRow row = table.NewRow();
                    GetRowDataXE(eventRow, row);
                    row[ndxDeleteme] = 1;
                    if (!IsDMLorSelectXE(row)) continue;
                    row[ndxStartTime] = timeZoneInfo.Convert(jobInfo.timeZoneInfo, (DateTime)row[ndxStartTime]);

                    table.Rows.Add(row);
                    rowCount++;
                }
                rowCount = -1;
                foreach (DataRow row in table.Rows)
                {
                    if (row == null) continue;
                    rowIndex++;
                    rowCount++;

                    // skip new rows added during state processing 
                    if (1 == GetRowInt32(row, ndxProcessed))
                    {
                        continue;
                    }

                    if (row == null) continue;

                    // mark all rows that get here deleted until we know better 
                    row[ndxDeleteme] = 1;
                    row[ndxKeepSql] = jobInfo.keepingSqlXE;

                    // Row Validation
                    if (!IsValidRow(row)) continue;

                    jobInfo.eventsTotal++;

                    // skip SQLcompliance internal processes early - we just eat all
                    // these events - other server side filtering done later
                    if (IsSQLcomplianceAction(row))
                    {
                        AddEventFilteredStats(GetRowDateTime(row, ndxStartTime));
                        jobInfo.eventsSQLcm++;
                        continue;
                    }

                    // some event preprocessing

                    // eat comments from sql - we no longer eat comments - users
                    //  use this to tag data (see Hangar Orthopedic case study)
                    string sqlText = GetRowString(row, ndxTextData);
                    row[ndxTextData] = sqlText;

                    // change success column - some events return NULL in this 
                    // column - this is equivalent to success=1 since it means
                    // we are doing a real event instead of permission event
                    if (row.IsNull(ndxSuccess))
                    {
                        row[ndxSuccess] = 1;
                    }

                    // skip overlap regions
                    if (preoverlap && overlapped)
                    {
                        if (
                           lastTrace.EndTime.CompareTo(GetRowDateTime(row, ndxStartTime)) >
                           0)
                        {
                            jobInfo.eventsOverlap++;
                            continue;
                        }
                        else
                        {
                            preoverlap = false;
                        }
                    }

                    // if we have reached this point, then we know we have an event
                    // we need to pay some attention to so we load state for this spid
                    StateRecord stateRecord =
                       stateTable.GetRecord(GetRowInt32(row, ndxSpid));

                    IncrementTraceEventCount(GetRowInt32(row, ndxEventClass), 0);

                    //-----------------------------
                    // State Machine
                    //-----------------------------
                    if (stateRecord.m_state == state_None)
                    {
                        processCurrent = true;
                    }
                    else
                    {
                        // Intermediate State Handling
                        if (stateRecord.m_state == state_InCreate)
                        {
                            InCreateHandler(row, stateRecord);
                        }
                        else if (stateRecord.m_state == state_InDrop)
                        {
                            InDropHandler(row, stateRecord);
                        }
                        else if (stateRecord.m_state == state_InDmlSql)
                        {
                            InDmlSqlHandler(row, stateRecord);
                        }
                        else if (stateRecord.m_state == state_InDml)
                        {
                            InDmlHandler(row, stateRecord);
                        }
                        else if (stateRecord.m_state == state_InDmlDataChange)
                        {
                            InDmlDataChangeHandler(row, stateRecord);
                        }
                        else if (stateRecord.m_state == state_InSp)
                        {
                            InSpHandler(row, stateRecord);
                        }

                        // Post Intermediate State Handling
                        if (stateRecord.m_state == state_None)
                        {
                            int saveSpid = stateRecord.spid;
                            stateRecord.Clear();
                            stateRecord.spid = saveSpid;

                            processCurrent = true;
                        }
                        else if (stateRecord.m_state == state_WriteWithinSql)
                        {
                            // we found something worth writing while in state = InSql
                            // but we dont want to reset this state just yet
                            // use current row
                            stateRecord.CopyToRow(row);
                            workingRow = row;

                            if (FinalizeRow(workingRow)) goodRows++;

                            stateRecord.m_state = state_InDmlSql;
                        }
                        else if (stateRecord.m_state == state_Finalize)
                        {
                            if (processCurrent)
                            {
                                // Add new row to dataset
                                if (stateRecord.m_row != -1)
                                {
                                    // row exists in dataset
                                    stateRecord.CopyToRow(
                                       table.Rows[stateRecord.m_row]);
                                    workingRow =
                                       table.Rows[stateRecord.m_row];
                                }
                                else
                                {
                                    // need new row 
                                    if (tableNewRows == null)
                                        throw new Exception(
                                           "Trace processing error: tableNewRows is null.",
                                           new NullReferenceException("ProcessRows::tableNewRows"));
                                    DataRow newRow = tableNewRows.NewRow();
                                    PopulateRow(newRow, stateRecord);
                                    tableNewRows.Rows.Add(newRow);
                                    workingRow = newRow;
                                }
                            }
                            else
                            {
                                // use current row
                                stateRecord.CopyToRow(row);
                                workingRow = row;
                            }

                            if (FinalizeRow(workingRow)) goodRows++;

                            stateRecord.m_state = state_None;
                        }

                        if (stateRecord.m_state == state_Skip)
                        {
                            stateRecord.m_state = state_None;
                        }
                    }

                    if (processCurrent)
                    {
                        // in the NONE case we will either see
                        //   (1) Single row (just finalize)
                        //   (2) First row in multi-row event set (finalize later)
                        bool singleRowEvent = InNoneHandler(row, rowIndex, stateRecord);
                        if (singleRowEvent)
                        {
                            if (FinalizeRow(row)) goodRows++;
                        }
                    }
                    // Update state table            
                    stateTable.PutRecord(stateRecord);
                }

                //------------------------------------------------------------------------
                // Done processing rows - persist all the state information for next time
                //------------------------------------------------------------------------
                if (!emptyTrace)
                {
                    // dump extra rows into main dataset
                    foreach (DataRow srcRow in tableNewRows.Rows)
                    {
                        table.ImportRow(srcRow);
                    }

                    lastTrace.StartTime = jobInfo.firstEventTime;
                    lastTrace.EndTime = jobInfo.lastEventTime;
                    lastTrace.Write();
                }

                stateTable.Write();

                // Write and clear login cache
                if (loginCache != null)
                {
                    loginCache.WriteCache();
                    loginCache = null;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write("TraceJob::ProcessRows",
                                         ex,
                                         true);
                throw;
            }
            finally
            {
                if (tableNewRows != null)
                {
                    foreach (DataColumn d in tableNewRows.Columns)
                        d.Dispose();
                    tableNewRows.Dispose();
                }
            }

            return goodRows;
        }

        //----------------------------------------------------------------------------
        // FinalizeRow - server side filtering, extra column calculation
        //               get ready to write
        //----------------------------------------------------------------------------
        private bool
            FinalizeRow(
            DataRow row
            )
        {
            bool retval = false;

            // load row into current       
            LoadRow(row);

            // SQLCM-5471 v5.6 Add Activity to Senstitive columns
            if (!isSenstiveColumnData(row))
            {
                return false;
            }

            // check filter?
            if (TraceFilter.RowMatchesEventFilter(this, row))
            {
                if (CreateCalculatedColumns(row) && !RowMatchesExcludeFilter(row))
                {
                    IncrementTraceEventCount(GetRowInt32(row, ndxEventClass), 1);

                    // if we are here, then this must be a valid event; unmark deletion flag
                    row[ndxDeleteme] = 0;
                    retval = true;
                }
                else // event is deleted by the exclude filter
                {
                    IncrementTraceEventCount(GetRowInt32(row, ndxEventClass), 2);
                    jobInfo.eventsDeleted++;
                }
            }
            else // event is filtered out
            {
                IncrementTraceEventCount(GetRowInt32(row, ndxEventClass), 3);
                jobInfo.eventsFilteredOut++;
            }

            if (!retval)
                AddEventFilteredStats(GetRowDateTime(row, ndxStartTime));

            return retval;
        }

        //-----------------------------------------------------------------------
        // DeleteTempTables - routine used by job harness to cleanup
        //
        // Note: If this fails, life goes on, We leave the temp tables and
        //       clean up in some regular grooming job
        //-----------------------------------------------------------------------
        public void
            DeleteTempTables(
            )
        {
            Repository rep = new Repository();

            try
            {
                rep.OpenConnection(CoreConstants.RepositoryTempDatabase);

                try
                {
                    string sqlText;
                    SqlCommand cmd;

                    sqlText = GetDropSQL(eventsTable);
                    using (cmd = new SqlCommand(sqlText, rep.connection))
                    {
                        cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception)
                { }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write("TraceJob::DeleteTempTables",
                                         CoreConstants.Exception_RepositoryNotAvailable,
                                         ex);
            }
            finally
            {
                rep.CloseConnection();
            }
        }

        #endregion

        #region State Handlers

        //---------------------------------------------------------------------
        // InNoneHandler
        //---------------------------------------------------------------------
        private bool
            InNoneHandler(
            DataRow row,
            int rowIndex,
            StateRecord stateRecord
            )
        {
            TraceEventId evtClass = (TraceEventId)GetRowInt32(row, ndxEventClass);
            if (jobInfo.traceFile.Contains("xel"))
                evtClass = TraceEventId.AuditObjectPermission;
            TraceSubclass evtsubClass =
               (TraceSubclass)GetRowInt32(row, ndxEventSubclass);
            int objType = GetRowInt32(row, ndxObjectType);

            TracePermissions perm = (TracePermissions)GetRowPermissions(row, ndxPermissions);


            int tmpSuccess = GetRowInt32(row, ndxSuccess);

            if (evtClass == TraceEventId.AuditStatementPermission)
            {
                if (tmpSuccess == 1)
                {
                    if (perm != TracePermissions.CreateRule
                         && perm != TracePermissions.CreateDefault
                         && perm != TracePermissions.BackupDatabase
                         && perm != TracePermissions.BackupTable
                         && perm != TracePermissions.BackupLog)
                    {
                        // Start of Create Event - AuditStatement Permissions
                        //    waiting for Object:Create or Exception
                        stateRecord.LoadRow(row);
                        stateRecord.m_state = state_InCreate;
                    }
                    else if (perm == TracePermissions.CreateRule)
                    {
                        row[ndxObjectType] = (int)DBObjectType.Rule;
                    }
                    else if (perm == TracePermissions.CreateDefault)
                    {
                        row[ndxObjectType] = (int)DBObjectType.DefaultConstraint;
                    }
                }
                else if (perm == TracePermissions.CreateDatabase)
                {
                    // failed access create database doesnt set objectId
                    row[ndxObjectType] = (int)DBObjectType.Database;
                }
            }
            else if (evtClass == TraceEventId.AuditDbcc)
            {
                // need to parse SQL to get database before filtering because SQL Server has bug
                // where it doesnt set databaseName correctly for DBCC commands
                GetDbccDatabaseInfo(row);
            }
            else if (evtClass == TraceEventId.AuditObjectDerivedPermission)
            {
                if (tmpSuccess == 1)
                {
                    if (evtsubClass == TraceSubclass.Drop &&
                         (objType == (int)DBObjectType.Database ||
                           objType == (int)DBObjectType.Database_2005))
                    {
                        // Start of Drop Database
                        //    waiting for Object:Deleted or Exception
                        stateRecord.LoadRow(row);
                        stateRecord.databaseName = GetRowString(row, ndxObjectName);
                        stateRecord.databaseId = -1; // have to wait for obejct:deleted
                        stateRecord.objectName = "";

                        stateRecord.m_state = state_InDrop;
                    }
                }
                else if (perm == TracePermissions.CreateDatabase)
                {
                    row[ndxObjectType] = (int)DBObjectType.Database;
                }
            }
            else if (evtClass == TraceEventId.Transaction)
            {
                if (evtsubClass != TraceSubclass.BeginTran)
                {
                    if (stateRecord.startTime != DateTime.MaxValue && stateRecord.startTime != DateTime.MinValue)
                        //row[ndxStartTime] = TimeZoneInfo.ToUniversalTime(jobInfo.timeZoneInfo, stateRecord.startTime);
                        row[ndxStartTime] = stateRecord.startTime;
                }
            }
            else if (evtClass == TraceEventId.SpStarting || evtClass == TraceEventId.SpCompleted) // Added SpCompleted for Row Counts
            {
                stateRecord.LoadRow(row);
                stateRecord.m_state = state_InSp;
            }

            else if (evtClass == TraceEventId.AuditServerPrincipalManagement ||
                      evtClass == TraceEventId.AuditDatabasePrincipalManagement)
            {
                // These two events returns success if the premission check passed no
                // matter the end result is success or failed.  Validate the login
                // name to reset the success column.
                if (GetRowInt32(row, ndxSuccess) == 1)
                {
                    if (!IsValidLoginName(GetRowString(row, ndxObjectName)))
                    {
                        row[ndxObjectName] = "";
                        row[ndxSuccess] = 0;
                    }
                }
            }
            else if (jobInfo.sqlVersion >= 9 && (
                     evtClass == TraceEventId.AuditObjectPermission) &&
                     (perm == TracePermissions.Insert ||
                       perm == TracePermissions.UpdateAll ||
                       perm == TracePermissions.Delete) &&
                     IsDataChangeEvent(row)) // Check DML data changes
            {
                // Check if object is monitored for data change
                if (jobInfo.traceFile.Contains("xel"))
                {
                    row[ndxStartSequence] = row[ndxEventSequence];
                    row[ndxEndSequence] = 0;
                }
                stateRecord.LoadRow(row);
                stateRecord.m_state = state_InDmlDataChange;
            }
            else if (jobInfo.keepingSql)
            {
                // Start of DML Event - SQLStmtStarting
                //    waiting for Audit Object Permissions and Exception
                if (evtClass == TraceEventId.SqlStmtStarting)   // Changed From SqlStmtStarting to SqlStmtCompleted for Row Counts
                {
                    stateRecord.LoadRow(row);
                    stateRecord.m_state = state_InDmlSql;
                }
            }

            if (stateRecord.m_state == state_None)
            {
                if (evtClass == TraceEventId.Transaction
                    && temp_m_state == state_InDmlSql
                    && (jobInfo.traceCategory == (int)TraceCategory.DBPrivilegedUsers
                    || jobInfo.traceCategory == (int)TraceCategory.DMLwithDetails
                    || jobInfo.traceCategory == (int)TraceCategory.ServerTrace))
                {
                    stateRecord.m_state = state_InDmlSql;
                    temp_m_state = 0;
                }
                // single row event
                return true;
            }
            else
            {
                // multi-row event
                stateRecord.m_flushed = 0;
                stateRecord.m_updated = DateTime.UtcNow;
                stateRecord.m_row = rowIndex;

                return false;
            }
        }

        //---------------------------------------------------------------------
        // InCreateHandler
        //---------------------------------------------------------------------
        private void
            InCreateHandler(
            DataRow row,
            StateRecord stateRecord
            )
        {
            TraceEventId evtClass = (TraceEventId)GetRowInt32(row, ndxEventClass);
            int objType = GetRowInt32(row, ndxObjectType);

            if (!Matches(row, stateRecord))
            {
                processCurrent = true;
                stateRecord.m_state = state_Finalize;
            }
            else
            {
                if (evtClass == TraceEventId.ObjectCreated) // 46
                {
                    processCurrent = false;

                    // make sure we are processing the permission matching objectType
                    if (ObjectTypeMatchesPermission(objType,
                                                    (TracePermissions)stateRecord.permissions))
                    {
                        stateRecord.m_state = state_Finalize;

                        if (objType == (int)DBObjectType.Database || objType == (int)DBObjectType.Database_2005)
                        {
                            // database create - get database information
                            stateRecord.databaseName = GetRowString(row, ndxDatabaseName);
                            stateRecord.databaseId = GetRowInt32(row, ndxDatabaseId);
                            stateRecord.objectType = (int)DBObjectType.Database;
                            stateRecord.objectName = "";
                            stateRecord.objectId = 0;
                        }
                        else
                        {
                            // non database create - get object information
                            stateRecord.objectType = GetRowInt32(row, ndxObjectType);
                            stateRecord.objectName = GetRowString(row, ndxObjectName);
                            stateRecord.objectId = GetRowInt32(row, ndxObjectId);
                        }
                    }
                    //else
                    //{
                    // eat other object created - someday we could create events for these
                    // just leave state alone and processcurrent = false
                    //}
                }
                else if (evtClass == TraceEventId.Exception) //33
                {
                    processCurrent = false;
                    stateRecord.m_state = state_Skip;
                }
                else
                {
                    processCurrent = true;
                    stateRecord.m_state = state_Finalize;
                }
            }
        }

        //---------------------------------------------------------------------
        // ObjectTypeMatchesPermission
        //---------------------------------------------------------------------
        private bool
            ObjectTypeMatchesPermission(
            int objType,
            TracePermissions perm
            )
        {
            if (jobInfo.isSqlServer2005)
            {
                if (perm == TracePermissions.CreateDatabase && objType == (int)DBObjectType.Database_2005) return true;
                if (perm == TracePermissions.CreateTable && objType == (int)DBObjectType.UserTable_2005) return true;
                if (perm == TracePermissions.CreateProcedure && objType == (int)DBObjectType.StoredProcedure_2005) return true;
                if (perm == TracePermissions.CreateView && objType == (int)DBObjectType.View_2005) return true;
                if (perm == TracePermissions.CreateRule && objType == (int)DBObjectType.Rule_2005) return true;
                if (perm == TracePermissions.CreateDefault && objType == (int)DBObjectType.DefaultConstraint_2005) return true;
                if (perm == TracePermissions.CreateFunction && (objType == (int)DBObjectType.CLRAggregateFunction_2005 ||
                         objType == (int)DBObjectType.InlineTableValuedSQLFunction_2005 ||
                         objType == (int)DBObjectType.PartitionFunction_2005 ||
                         objType == (int)DBObjectType.TableValuedSQLFunction_2005 ||
                         objType == (int)DBObjectType.ScalarSQLFunction_2005 ||
                         objType == (int)DBObjectType.CLRScalarFunction_2005 ||
                         objType == (int)DBObjectType.InlineScalarSQLFunction_2005 ||
                         objType == (int)DBObjectType.CLRTableValuedFunction_2005)) return true;
            }
            else
            {
                if (perm == TracePermissions.CreateDatabase && objType == (int)DBObjectType.Database) return true;
                if (perm == TracePermissions.CreateTable && objType == (int)DBObjectType.UserTable) return true;
                if (perm == TracePermissions.CreateProcedure && objType == (int)DBObjectType.StoredProcedure) return true;
                if (perm == TracePermissions.CreateView && objType == (int)DBObjectType.View) return true;
                if (perm == TracePermissions.CreateRule && objType == (int)DBObjectType.Rule) return true;
                if (perm == TracePermissions.CreateDefault && objType == (int)DBObjectType.DefaultConstraint) return true;
                if (perm == TracePermissions.CreateFunction && (objType == (int)DBObjectType.UDF ||
                                                                objType == (int)DBObjectType.InlineFunction ||
               objType == (int)DBObjectType.TableValuedUDF)) return true;
            }
            return false;
        }

        //---------------------------------------------------------------------
        // InDropHandler
        //---------------------------------------------------------------------
        private void
            InDropHandler(
            DataRow row,
            StateRecord stateRecord
            )
        {
            TraceEventId evtClass = (TraceEventId)GetRowInt32(row, ndxEventClass);

            if (!Matches(row, stateRecord))
            {
                processCurrent = true;
                stateRecord.m_state = state_Finalize;
            }
            else
            {
                if (evtClass == TraceEventId.ObjectDeleted)
                {
                    processCurrent = false;
                    stateRecord.m_state = state_Finalize;

                    stateRecord.databaseId = GetRowInt32(row, ndxDatabaseId);
                }
                else if (evtClass == TraceEventId.Exception) //33
                {
                    processCurrent = false;
                    stateRecord.m_state = state_Skip;
                }
                else
                {
                    processCurrent = true;
                    stateRecord.m_state = state_Finalize;
                }
            }
        }

        //---------------------------------------------------------------------
        // InDmlSqlHandler
        //--------------------------------------------------------------------
        private void
            InDmlSqlHandler(
            DataRow row,
            StateRecord stateRecord
            )
        {
            if (!Matches(row, stateRecord)) // Another DML event with the same spid starting?
            {
                processCurrent = true;
                stateRecord.m_state = state_Finalize;
            }
            else
            {
                TraceEventId evtClass = (TraceEventId)GetRowInt32(row, ndxEventClass);
                if (jobInfo.traceFile.Contains("xel"))
                {
                    evtClass = TraceEventId.AuditObjectPermission;
                }

                switch (evtClass)
                {
                    case TraceEventId.Exception:  // error when executing the DML statement
                        processCurrent = false;
                        stateRecord.m_state = state_Skip;
                        return;

                    case TraceEventId.AuditObjectPermission: //The matching event?

                        TracePermissions perm = (TracePermissions)GetRowPermissions(row, ndxPermissions);

                        if (perm != TracePermissions.Execute) // Insert, update and delete
                        {
                            // The matching event found.
                            processCurrent = false;
                            if (stateRecord.eventClass == (int)TraceEventId.SqlStmtStarting
                                || stateRecord.eventClass == (int)TraceEventId.SpStarting)
                            {
                                string sql = stateRecord.textData;
                                stateRecord.LoadRow(row);
                                stateRecord.textData = sql;  // copy the sql text
                            }

                            if (jobInfo.sqlVersion >= 9 &&
                                 IsDataChangeEvent(row) && (jobInfo.traceCategory == 4 || jobInfo.traceCategory == 5))
                            {
                                // the three states for a data change DML event without SQL
                                // state_IndmlSql-->state_IndmlDataChange-->state_Finalize
                                stateRecord.m_state = state_InDmlDataChange;
                            }
                            else
                                stateRecord.m_state = state_Finalize;
                        }
                        else // execute
                        {
                            processCurrent = true;
                            stateRecord.m_state = state_Skip;
                        }
                        return;
                    default:
                        processCurrent = true;
                        stateRecord.m_state = state_Skip;
                        return;
                }
            }
        }

        //---------------------------------------------------------------------
        // InDmlHandler
        //---------------------------------------------------------------------
        private void
            InDmlHandler(
            DataRow row,
            StateRecord stateRecord
            )
        {
            TraceEventId evtClass = (TraceEventId)GetRowInt32(row, ndxEventClass);

            if (IsDataChangeEndEvent(row))
            {
                // found
                stateRecord.m_state = state_Finalize;
                processCurrent = false;
                return;
            }

            if (!Matches(row, stateRecord))
            {
                processCurrent = true;
                stateRecord.m_state = state_Finalize;
            }
            else
            {
                if (evtClass == TraceEventId.Exception) // 33
                {
                    processCurrent = false;
                    stateRecord.m_state = state_Skip;
                }
                else
                {
                    processCurrent = true;
                    stateRecord.m_state = state_Finalize;
                }
            }
        }

        private void InServerStartStopHandler(DataRow row, StateRecord stateRecord)
        {
            TraceEventId eClass = (TraceEventId)row[ndxEventClass];
            if (eClass == TraceEventId.ServiceControl)
            {
                stateRecord.m_state = state_Finalize;
                processCurrent = true;
            }
        }


        //---------------------------------------------------------------------
        // InDmlDataChangeHandler
        //---------------------------------------------------------------------
        private void
            InDmlDataChangeHandler(
            DataRow row,
            StateRecord stateRecord
            )
        {
            if (IsDataChangeEndEvent(row))
            {
                // found

                stateRecord.m_state = state_Finalize;
                stateRecord.startSequence = stateRecord.eventSequence;
                stateRecord.endTime = GetRowDateTime(row, ndxStartTime);
                stateRecord.endSequence = GetRowInt64(row, ndxEventSequence);
                processCurrent = false;
            }
            else
            {
                TraceEventId eClass = (TraceEventId)row[ndxEventClass];
                // Added SpCompleted for Row Counts
                if ((eClass == TraceEventId.SpStarting
                    || eClass == TraceEventId.SqlStmtStarting
                    || eClass == TraceEventId.SpCompleted
                    || eClass == TraceEventId.SqlStmtCompleted
                    || eClass == TraceEventId.SpStmtCompleted)
                    && !jobInfo.traceFile.Contains("xel"))  // Added SqlStmtCompleted for Row Counts
                {
                    stateRecord.m_state = state_InDmlDataChange;
                    processCurrent = false;
                }
                else // errors, DML events are treated as the end of the previous DML event
                {
                    stateRecord.m_state = state_Finalize;
                    stateRecord.startSequence = stateRecord.eventSequence;
                    stateRecord.endTime = GetRowDateTime(row, ndxStartTime);
                    stateRecord.endSequence = GetRowInt64(row, ndxEventSequence) - 1;
                    processCurrent = true;
                }
            }

            return;
        }

        //---------------------------------------------------------------------
        // InSpHandler
        //--------------------------------------------------------------------
        private void
            InSpHandler(
            DataRow row,
            StateRecord stateRecord
            )
        {
            TraceEventId evtClass = (TraceEventId)GetRowInt32(row, ndxEventClass);
            TracePermissions perm = (TracePermissions)GetRowPermissions(row, ndxPermissions);

            if (!Matches(row, stateRecord))
            {
                processCurrent = true;
                stateRecord.m_state = state_Finalize;
            }
            else
            {
                if (evtClass == TraceEventId.AuditObjectPermission && //114
                    perm == TracePermissions.Execute)
                {
                    processCurrent = false;
                    stateRecord.m_state = state_Finalize;

                    string sql = stateRecord.textData;
                    stateRecord.LoadRow(row);
                    stateRecord.textData = sql;
                }
                else
                {
                    processCurrent = true;
                    stateRecord.m_state = state_Skip;
                }
            }
        }

        #endregion

        #region Event Correlation Utility Functions

        //-----------------------------------------------------------------------
        // IsValidRow - Simple validation of record - make sure that
        //              basic properties exist and are within expected range
        //-----------------------------------------------------------------------
        internal bool
            IsValidRow(
            DataRow row
            )
        {
            bool retval = false;
            string errMsg = "";


            int evClass = GetRowInt32(row, ndxEventClass);

            if (evClass <= (int)TraceEventId.EventIdMin ||
                evClass > (int)TraceEventId.EventIdMax)
            {
                errMsg = "Invalid event class";
                goto leave_routine;
            }

            if (GetRowDateTime(row, ndxStartTime) == DateTime.MinValue)
            {
                errMsg = "Invalid StartTime";
                goto leave_routine;
            }

            if (GetRowInt32(row, ndxSpid) == 0)
            {
                errMsg = "Invalid SPID";
                goto leave_routine;
            }

            retval = true;

        leave_routine:
            if (!retval)
            {
                if (ErrorLog.Instance.ErrorLevel >= ErrorLog.Level.Debug)
                    ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                            CoreConstants.Msg_EventJob,
                                            String.Format(CoreConstants.Debug_InvalidTraceRecord,
                                                          evClass,
                                                      startTime,
                                                          errMsg),
                                            ErrorLog.Severity.Warning);
            }

            return retval;
        }


        //----------------------------------------------------------------------------
        // Matches - basic check that two events shoudl be correlated
        //           (1) few basic properties
        //           (2) events occurred close enough together
        //----------------------------------------------------------------------------
        private bool
            Matches(
            DataRow row,
            StateRecord stateRecord

            )
        {
            // No nest level on objectcreate and delete events so skip check
            TraceEventId evtClass = (TraceEventId)GetRowInt32(row, ndxEventClass);
            if (evtClass != TraceEventId.ObjectCreated
                && evtClass != TraceEventId.ObjectDeleted)
            {
                if (GetRowInt32(row, ndxNestLevel) != stateRecord.nestLevel) return false;
            }

            if (GetRowString(row, ndxHostName) != stateRecord.hostName) return false;
            if (GetRowString(row, ndxServerName) != stateRecord.serverName) return false;
            if (GetRowString(row, ndxApplicationName) != stateRecord.applicationName) return false;

            // events must fall within a couple minutes of each other or we assume there is a gap
            DateTime t = stateRecord.startTime;
            t = t.AddMinutes(max_CorrelateTime);
            if (t.CompareTo(GetRowDateTime(row, ndxStartTime)) < 0)
            {
                return false;
            }

            return true;
        }

        //----------------------------------------------------------------------------
        // PopulateRow
        //----------------------------------------------------------------------------
        private void
            PopulateRow(
            DataRow row,
            StateRecord stateRecord
            )
        {
            stateRecord.CopyToRow(row);

            // zero out calculated fields
            row[ndxTargetObject] = "";
            row[ndxDetails] = "";
            row[ndxEventType] = 0;
            row[ndxEventCategory] = 0;
            row[ndxHash] = 0;
            row[ndxAlertLevel] = 0;
            row[ndxChecksum] = 0;
            if (row.IsNull(ndxPrivilegedUser))
                row[ndxPrivilegedUser] = jobInfo.privilegedUserTrace;
            row[ndxKeepSql] = jobInfo.keepingSql;

            row[ndxNewRow] = 0;
            row[ndxDeleteme] = 1;
            row[ndxProcessed] = 1;
        }


        // Check if the event is a special event generated by the before/after triggers that
        // marks the end of a sequence of SELECT.
        private bool IsDataChangeEndEvent(DataRow row)
        {
            TraceEventId evtClass = (TraceEventId)GetRowInt32(row, ndxEventClass);
            if (jobInfo.traceFile.Contains("xel"))
                evtClass = TraceEventId.AuditObjectPermission;
            if (evtClass != TraceEventId.AuditObjectPermission)
                return false;
            int perm = GetRowInt32(row, ndxPermissions);

            if (perm != 16) // Delete
                return false;
            string parent = GetRowString(row, ndxParentName);
            if (parent != "SQLcompliance_Data_Change")
                return false;

            string objName = GetRowString(row, ndxObjectName);
            if (objName != "SQLcompliance_Changed_Data_Table")
                return false;
            return true;
        }

        // Check if the object in the event is being monitored for data changes
        private bool IsDataChangeEvent(DataRow row)
        {
            int dbId = GetRowInt32(row, ndxDatabaseId);
            if (jobInfo.dcTableLists.ContainsKey(dbId))
            {
                List<string> tables = jobInfo.dcTableLists[dbId];
                string table = GetRowString(row, ndxObjectName);
                string schema = GetRowString(row, ndxParentName);
                if (tables.Contains(CoreHelpers.GetTableNameKey(schema, table)))
                {
                    DBObjectType type = (DBObjectType)GetRowInt32(row, ndxObjectType);
                    if (type == DBObjectType.UserTable_2005 ||
                         type == DBObjectType.SystemTable_2005)
                        return true;
                }
            }
            return false;
        }

        #endregion

        #region State Machine Time handling methods

        /*
      //---------------------------------------------------------------------
      // WithinTimeContraint
      //---------------------------------------------------------------------
      private bool
         WithinTimeConstraint(
            DateTime firstEvent,
            DateTime secondEvent
         )
      {
         if ( secondEvent.CompareTo(firstEvent) < 0 ) 
            return false;
            
         DateTime t = firstEvent;
         t = t.AddMinutes( max_CorrelateTime);
         if ( t.CompareTo( secondEvent ) < 0 )
         {   
            return false;
         }
            
         return true;                  
      }
      */

        //---------------------------------------------------------------------
        // OutOfSequence - Check for times where we have gaps of missing
        //                 events or are processing trace files out of order
        //                 for some reason - this will cause us to flush 
        //                 evrything we have and start fresh
        //---------------------------------------------------------------------
        private bool
            OutOfSequence(
            DateTime prevStart,
            DateTime prevEnd,
            DateTime thisStart,
            DateTime thisEnd
            )
        {
            if (prevStart == DateTime.MinValue) return true; // no previous events
            if (thisEnd.CompareTo(prevEnd) < 0) return true; // out of sequence
            if (thisStart.CompareTo(prevStart) < 0) return true; // out of sequence

            // look for too much of a gap between trace files
            DateTime t = prevEnd;
            t = t.AddMinutes(max_CorrelateTime);
            if (t.CompareTo(thisStart) < 0)
            {
                return true;
            }

            return false;
        }


        //---------------------------------------------------------------------
        // Overlapped - Does new file overlap in time (forwards) old file
        //              if so, we will ignore events up to the time the old
        //              one ends
        //---------------------------------------------------------------------
        private bool
            Overlapped(
            DateTime prevStart,
            DateTime prevEnd,
            DateTime thisStart,
            DateTime thisEnd
            )
        {
            if (thisStart.CompareTo(prevEnd) > 0) return false;
            if (thisEnd.CompareTo(prevStart) < 0) return false;
            return true;
        }

        #endregion

        #region Event Filter Matching

        //----------------------------------------------------------------------------
        // IsSQLcomplianceAction - used to provide server side filtering of SQLcompliance
        //                     actions
        //----------------------------------------------------------------------------
        private bool
            IsSQLcomplianceAction(
            DataRow row
            )
        {
            if (!jobInfo.sqlSecureTrace)
            {
                // Simple Filter: Ignore events from agent, GUI and server
                TraceEventId evClass = (TraceEventId)GetRowInt32(row, ndxEventClass);
                if (evClass == TraceEventId.Logout
                    || evClass == TraceEventId.Login
                    || evClass == TraceEventId.AuditChangeAudit
                    || evClass == TraceEventId.AuditStatementPermission
                    || evClass == TraceEventId.AuditObjectPermission
                    || evClass == TraceEventId.AuditObjectDerivedPermission
                    || evClass == TraceEventId.AuditDbcc
                    || evClass == TraceEventId.SpStarting
                    || evClass == TraceEventId.SpCompleted  // Added SpCompleted for Row Counts
                    || evClass == TraceEventId.ObjectCreated
                    || evClass == TraceEventId.ObjectDeleted
                    || evClass == TraceEventId.Exception
                    || evClass == TraceEventId.SqlStmtStarting
                    || evClass == TraceEventId.SqlStmtCompleted
                    )
                {
                    string appName = GetRowString(row, ndxApplicationName);

                    if (CoreConstants.filterServerEvents
                        && ((appName == CoreConstants.DefaultSqlApplicationName)
                            || (appName == CoreConstants.CollectionServiceName)))
                    {
                        return true;
                    }

                    if (CoreConstants.filterAgentEvents
                        && ((appName == CoreConstants.DefaultSqlApplicationName)
                            || (appName == CoreConstants.CollectionServiceName)))
                    {
                        return true;
                    }

                    if (CoreConstants.filterGUIEvents && (appName == CoreConstants.ManagementConsoleName))
                    {
                        return true;
                    }
                }
            }
            else
            {
                return false;
            }
            return false;
        }

        private bool RowMatchesExcludeFilter(DataRow row)
        {
            // Check event filtering here
            bool bFiltered = false;
            try
            {

                if (jobInfo.eventFilterProcessor.ContainsRules)
                {
                    EventRecord record = RuleProcessor.RowToRecord(row);
                    bFiltered = jobInfo.eventFilterProcessor.IsFiltered(record);
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write("TraceJob::RowMatchesExcludeFilter",
                                        "Error in event filtering.",
                                        e,
                                        true);
            }
            return bFiltered;
        }

        #endregion

        #region Write Final Events Table

        //-----------------------------------------------------------------------
        // WriteFinalEventTable
        //
        //
        // Note: If the new event is for a privileged user we dont want to 
        //       drop the event. We need to update the privilegedUser
        //       column in the matching event so that it is flagged.
        //-----------------------------------------------------------------------
        private void WriteFinalEventTable(int eventsToBeWritten, DataTable table)
        {
            Repository writeRepository = new Repository();
            Repository readRepository = new Repository();

            int eventId = 0;
            long start = Environment.TickCount;
            int newWatermark;
            int currentBatch = 0;
            int totalRows = 0;
            SqlTransaction writeTrans = null;

            //Before proceding check if the Max(eventId), highWatermark,lowWatermark and alertHighWatermark are in sync if not sync it
            EventId.SyncWatermarks(jobInfo.instance, jobInfo.eventDatabase);

            // allocate a block of event Ids
            currentId = EventId.GetNextIdBlock(jobInfo.instance,
                                               eventsToBeWritten);
            newWatermark = currentId + eventsToBeWritten - 1;

            try
            {
                using (DataTable eventsTbl = new DataTable())
                {
                    using (DataTable eventSQLTbl = new DataTable())
                    {
                        writeRepository.OpenConnection(jobInfo.eventDatabase);
                        readRepository.OpenConnection(jobInfo.eventDatabase);
                        InitEventsTableColumns(eventsTbl, readRepository.connection);
                        InitEventSqlTableColumns(eventSQLTbl, readRepository.connection);
                        writeTrans =
                           writeRepository.connection.BeginTransaction(IsolationLevel.ReadUncommitted);

                        bool auditUserCaptureDDL = false;
                        bool auditUserCaptureSecurity = false;
                        if (jobInfo.userConfigs != null)
                        {
                            auditUserCaptureDDL = jobInfo.userConfigs.AuditCaptureDDL;
                            auditUserCaptureSecurity = jobInfo.userConfigs.AuditUserSecurity;
                        }

                        foreach (DataRow row in table.Rows)
                        {
                            if (row == null) continue;
                            var textData = row[ndxTextData].ToString().ToUpper();

                            if ((int)row[ndxDeleteme] == 1) continue;

                            rowsWritten++;
                            // We can skip these rows - they have already been written
                            if (rowsWritten <= jobInfo.lastFinalEventProcessed)
                            {
                                continue;
                            }

                            currentBatch++;
                            if (currentBatch % insertBatchSize == 0)
                            {
                                writeTrans = CommitChanges(writeRepository.connection,
                                                            writeTrans,
                                                            eventsTbl,
                                                            eventSQLTbl);

                                totalRows += currentBatch;
                                currentBatch = 0;

                                // check aborting flag	                  
                                if (jobInfo.GetAborting())
                                    return;

                                writeTrans =
                                   writeRepository.connection.BeginTransaction(IsolationLevel.ReadUncommitted);
                            }

                            // SQLCM-3040 Create and drop index events are recorded as 'Alter User Table' event type
                            var eventType = (TraceEventType)GetRowInt32(row, ndxEventType);

                            // 217 denotes Alter Table Function, 101 denotes Create Index and 301 denotes Drop Index
                            if (eventType == TraceEventType.AlterUserTable)
                            {
                                // Create Index
                                if (textData.StartsWith("CREATE INDEX "))
                                {
                                    row[ndxEventType] = TraceEventType.CreateIndex;
                                }
                                // Drop Index
                                else if (textData.StartsWith("DROP INDEX "))
                                {
                                    row[ndxEventType] = TraceEventType.DropIndex;
                                }
                            }

                            EventRecord er = AddEventRow(eventsTbl, row);

                            jobInfo.UpdateIdCache(er);
                            eventId = er.eventId;
                            AddStats(er);
                            currentId++; // bump current event Id

                            // Write SQL Statement - only write on new rows
                            //     old ones being updated already have SQL

                            var txtData = row[ndxTextData].ToString().ToUpper();
                            if (IsDDLSqlStatement(txtData))
                            {
                                if (jobInfo.traceLevel == (int)TraceLevel.User)
                                {
                                    if (auditUserCaptureDDL && IsCurrentUserIsPrivileged((string)row[ndxLoginName]))
                                        AddSqlTableRow(row, eventId, eventSQLTbl);
                                }
                                else
                                {
                                    
                                    string dbname = row[ndxDatabaseName].ToString();


                                    //SQLCM-5.6 SQLCM-5713 Create database DDL event SQL text is not captured.
                                    if (checkDDLType(txtData))
                                    {
                                        AddSqlTableRow(row, eventId, eventSQLTbl);
                                        continue;
                                    }
                                    //    
                                        //unfortunately we have to get this for every row since each row in the trace could be for a different database.                                    
                                    DBAuditConfiguration auditDBSettings = GetDBAuditSettings(dbname);
                                    if (auditDBSettings == null)
                                    {
                                        // Ensure that the SQL text for the DDL events is captured
                                        AddSqlTableRow(row, eventId, eventSQLTbl);
                                        continue;
                                    }
                                    bool auditCaptureDBDDL = auditDBSettings.AuditCaptureDDL;

                                    //SQLCM-5.6 SQLCM-5713  Storing auditDDL
                                    bool auditDDL = auditDBSettings.AuditDDL;
                                    //
                                    bool auditCaptureDBUserDDL = auditDBSettings.AuditUserCaptureDDL;


                                    
                                    if (jobInfo.traceCategory != (int)TraceCategory.DBPrivilegedUsers)
                                    {
                                        //SQLCM-5.6 SQLCM-5713 Checking auditDDL before adding row to table.
                                        if (auditCaptureDBDDL)
                                            AddSqlTableRow(row, eventId, eventSQLTbl);
                                    }
                                    else
                                    {
                                        if (auditCaptureDBUserDDL && IsDbPrivUser(row))
                                        {
                                            AddSqlTableRow(row, eventId, eventSQLTbl);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //By Hemant
                                //Added and used a new variable to check ShowSQL for AuditAdmin (DBCC), thus differentiating it from PrivilegedUser ShowSQL for Select and DML.
                                if ((int)row[ndxEventClass] == 116 && jobInfo.keepingAdminSql) //checking DBCC
                                {
                                    AddSqlTableRow(row, eventId, eventSQLTbl);
                                }
                                else if (jobInfo.keepingSql || (GetRowInt32(row, ndxKeepSql) == ON))
                                {
                                    AddSqlTableRow(row, eventId, eventSQLTbl);
                                }
                                else if((int)row[ndxEventCategory] == (int)TraceEventCategory.Security)
                                {
                                    if (auditUserCaptureSecurity && IsCurrentUserIsPrivileged((string)row[ndxLoginName]))
                                    {
                                        // Ensure that the SQL text for the DDL events is captured
                                        AddSqlTableRow(row, eventId, eventSQLTbl);
                                    }
                                    else
                                    {
                                        string dbname = (string)row[ndxDatabaseName];
                                        DBAuditConfiguration auditDBSettings = GetDBAuditSettings(dbname);
                                        if ((auditDBSettings.AuditSecurity && auditDBSettings.AuditCaptureDDL) ||
                                            (IsDbPrivUser(row) && auditDBSettings.AuditUserSecurity && auditDBSettings.AuditUserCaptureDDL))
                                        {
                                            // Ensure that the SQL text is captured
                                            AddSqlTableRow(row, eventId, eventSQLTbl);
                                        }

                                    }
                                }
                            }
                        }

                        AddEventReceivedStats(table.Rows.Count);
                        writeTrans = CommitChanges(writeRepository.connection,
                                                    writeTrans,
                                                    eventsTbl,
                                                    eventSQLTbl);

                        totalRows += currentBatch;
                        jobInfo.UpdateIdTables(writeRepository.connection);
                    } // End of using eventSQLtbl
                } // End of using eventsTbl


                // summary of what we did
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                        String.Format(CoreConstants.Debug_NewEventsCommitted,
                                                      jobInfo.instance,
                                                      jobInfo.traceFile,
                                                      eventsToBeWritten,
                                                      jobInfo.eventsInserted,
                                                      eventId,
                                                      newWatermark));
            }
            finally
            {
                if (writeTrans != null)
                {
                    rowsWritten -= currentBatch;
                    ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                             String.Format("Transaction rollback: at rowsWritten: {0}, totalRows: {1}, currentBatch: {2}",
                                                             rowsWritten,
                                                             totalRows,
                                                             currentBatch));
                    writeTrans.Rollback();
                }
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                         String.Format("Trace processing: {0} rows inserted and {1} rows skipped in {2}ms",
                                                        totalRows,
                                                        rowsWritten - totalRows,
                                                        Environment.TickCount - start));

                writeRepository.CloseConnection();
                readRepository.CloseConnection();
            }

            EventId.UpdateStatus(jobInfo.instance,
                                 currentId - 1);
        }

        private bool IsCurrentUserIsPrivileged(string privUserCheck)
        {
            if (jobInfo.privUsers != null)
            {
                return jobInfo.privUsers.Contains(privUserCheck.ToUpper());
            }
            return false;
        }

        //Get the DB configuration for the database from the jobInfo for this trace
        private DBAuditConfiguration GetDBAuditSettings(string dbname)
        {
            DBAuditConfiguration auditDBSettings = null;

            if (jobInfo.dbconfigs != null)
            {
                foreach (KeyValuePair<int, DBAuditConfiguration> kvp in jobInfo.dbconfigs)
                {
                    if (dbname == kvp.Value.Name)
                    {
                        auditDBSettings = kvp.Value;
                        break;
                    }
                }
            }
            return auditDBSettings;
        }

        //SQLCM-5.6 SQLCM-5713 Create database DDL event SQL text is not captured.
        private bool checkDDLType(string txtData)
        {
            if (txtData.StartsWith("ALTER DATABASE") ||
                txtData.StartsWith("CREATE DATABASE") ||
                txtData.StartsWith("DROP DATABASE") )
            {
                return true;
            }

            return false;
        }
        private bool IsDDLSqlStatement(string txtData)
        {
            if (txtData.StartsWith("ALTER") ||
                txtData.StartsWith("CREATE") ||
                txtData.StartsWith("DISABLE TRIGGER") ||
                txtData.StartsWith("DROP") ||
                txtData.StartsWith("ENABLE TRIGGER") ||
                txtData.StartsWith("TRUNCATE TABLE") ||
                txtData.StartsWith("UPDATE STATISTICS") ||
                Regex.IsMatch(txtData, @"(EXEC(\s*)%%Owner(\s*)\((\s*)Name(\s*)=(\s*)@name_in_db(\s*)\)(\s*)\.(\s*)DropSchema(\s*)\((\s*)OwnerType(\s*)=(\s*)\d(\s*)\))+", RegexOptions.IgnoreCase))
            {
                return true;
            }

            return false;
        }

        //SQLCM-8 SQLCM-2163-Enabling Capture Transaction Status removes values from SQL text 5.4 Start
        private bool IsDMLSqlStatement(string txtData)
        {
            if (txtData.StartsWith("DELETE") ||
                txtData.StartsWith("INSERT") ||
                txtData.StartsWith("UPDATE") ||
                txtData.StartsWith("UPDATE STATISTICS"))
            {
                return true;
            }

            return false;
        } /// SQLCM-2163-Enabling Capture Transaction Status removes values from SQL text End
        /// 

        //5.4 XE
        private bool IsDMLorSelectXE(DataRow row)
        {
            string txtData = row[ndxTextData].ToString();
            if (txtData != null && !txtData.Equals(""))
            {

                txtData = txtData.ToUpper();
                try
                {
                    if (txtData.StartsWith("SELECT"))
                    {
                        row[ndxEventCategory] = TraceEventCategory.SELECT;
                        row[ndxEventType] = TraceEventType.SELECT;
                        row[ndxPermissions] = 1;
                        GetObjectDataXE(row);
                        return true;
                    }
                    if (IsDMLSqlStatement(txtData) || (txtData.StartsWith("EXEC") && !txtData.StartsWith("EXECUTE AS")))
                    {
                        row[ndxEventCategory] = TraceEventCategory.DML;
                        if (txtData.StartsWith("DELETE"))
                        {
                            row[ndxEventType] = TraceEventType.DELETE;
                            row[ndxPermissions] = 16;
                        }
                        else if (txtData.StartsWith("INSERT"))
                        {
                            row[ndxEventType] = TraceEventType.INSERT;
                            row[ndxPermissions] = 8;
                        }
                        else if (txtData.StartsWith("UPDATE"))
                        {
                            row[ndxEventType] = TraceEventType.UPDATE;
                            row[ndxPermissions] = 2;
                        }
                        else if (txtData.StartsWith("EXEC") && !txtData.StartsWith("EXECUTE AS"))
                        {
                            row[ndxEventType] = TraceEventType.EXECUTE;
                            row[ndxPermissions] = 32;
                        }
                        GetObjectDataXE(row);
                        return true;
                    }
                }
                catch (Exception e)
                { return false; }
            }
            return false;
        }

        //5.4 XE
        //public void GetRowDataXE()

        private int GetAuditUserCaptureDDL(string instance, SqlConnection conn)
        {
            int auditUserCaptureDDL = 0;
            string sqlText = String.Format("SELECT [auditUserCaptureDDL] FROM [SQLcompliance].[dbo].[Servers] where instance = '{0}'", instance);

            using (SqlCommand selectCmd = new SqlCommand(sqlText, conn))
            {
                selectCmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                selectCmd.CommandType = CommandType.Text;
                SqlDataReader reader = selectCmd.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        int.TryParse(reader.GetValue(0).ToString(), out auditUserCaptureDDL);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
            return auditUserCaptureDDL;
        }

        private string GetUsersList(string sqlText, SqlConnection conn)
        {
            string auditUsersList = string.Empty;

            using (SqlCommand selectCmd = new SqlCommand(sqlText, conn))
            {
                selectCmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                selectCmd.CommandType = CommandType.Text;

                using (SqlDataReader reader = selectCmd.ExecuteReader())
                {
                    try
                    {
                        while (reader.Read())
                        {
                            auditUsersList = reader.GetValue(0).ToString();
                        }
                    }
                    finally
                    {
                        reader.Close();
                    }
                }
            }

            return auditUsersList;
        }

        //----------------------------------------------------------------------
        // CommitChanges(): insert the processed records, update the stats table 
        // and commit the transaction.
        // Note: The transaction object returned is always a null.
        //---------------------------------------------------------------------
        private SqlTransaction
           CommitChanges(
              SqlConnection conn,
              SqlTransaction trans,
              DataTable events,
              DataTable eventSql
           )
        {
            if (trans != null)
            {
                try
                {
                    jobInfo.eventsInserted += events.Rows.Count;
                    ErrorLog.Instance.Write(ErrorLog.Level.Debug, String.Format("Transaction commit: at rowsWritten: {0}", rowsWritten));
                    UpdateStats(conn, trans);

                    BulkInsert(conn, trans, CoreConstants.RepositoryEventsTable, events);
                    BulkInsert(conn, trans, CoreConstants.RepositoryEventSqlTable, eventSql);

                    jobInfo.UpdateRowsProcessed(rowsWritten, conn, trans);
                    trans.Commit();
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 2601)
                    {
                        Repository repo = new Repository();
                        try
                        {
                            repo.OpenConnection(CoreConstants.RepositoryDatabase);
                            LogRecord.WriteLog(repo.connection, LogType.EventDatabaseFull, jobInfo.instance,
                                CoreConstants.Info_EventDatabaseLimitExceeded + "\r\n\r\n\r\n" +
                                "Error: " + ex.Message);
                        }
                        catch
                        {
                        }
                        finally
                        {
                            repo.CloseConnection();
                        }
                    }
                    throw;
                }
            }

            return null;
        }


        protected virtual EventRecord
            CreateEventRecord(
            SqlConnection conn,
            DataRow row
            )
        {
            EventRecord er = new EventRecord(conn, jobInfo.eventDatabase);

            er.eventClass = GetRowInt32(row, ndxEventClass);
            er.eventSubclass = GetRowInt32(row, ndxEventSubclass);
            er.startTime = GetRowDateTime(row, ndxStartTime);
            er.spid = GetRowInt32(row, ndxSpid);
            er.applicationName = GetRowString(row, ndxApplicationName);
            er.hostName = GetRowString(row, ndxHostName);
            er.serverName = GetRowString(row, ndxServerName);
            er.loginName = GetRowString(row, ndxLoginName);
            er.success = GetRowInt32(row, ndxSuccess);
            er.databaseName = GetRowString(row, ndxDatabaseName);
            er.databaseId = GetRowInt32(row, ndxDatabaseId);
            er.dbUserName = GetRowString(row, ndxDbUserName);
            er.objectType = GetRowInt32(row, ndxObjectType);
            er.objectName = GetRowString(row, ndxObjectName);
            er.objectId = GetRowInt32(row, ndxObjectId);

            er.permissions = GetRowPermissions(row, ndxPermissions);

            er.columnPermissions = GetRowInt32(row, ndxColumnPermissions);
            er.targetLoginName = GetRowString(row, ndxTargetLoginName);
            er.targetUserName = GetRowString(row, ndxTargetUserName);
            er.roleName = GetRowString(row, ndxRoleName);
            er.ownerName = GetRowString(row, ndxOwnerName);

            er.targetObject = GetRowString(row, ndxTargetObject);
            er.details = GetRowString(row, ndxDetails);
            er.eventType = (TraceEventType)GetRowInt32(row, ndxEventType);
            er.eventCategory = (TraceEventCategory)GetRowInt32(row, ndxEventCategory);
            er.hash = GetRowInt32(row, ndxHash);
            er.privilegedUser = GetRowInt32(row, ndxPrivilegedUser);

            er.fileName = GetRowString(row, ndxFileName);
            er.linkedServerName = GetRowString(row, ndxLinkedServerName);
            er.parentName = GetRowString(row, ndxParentName);
            er.isSystem = GetRowInt32(row, ndxIsSystem);
            er.sessionLoginName = GetRowString(row, ndxSessionLoginName);
            er.providerName = GetRowString(row, ndxProviderName);
            er.appNameId = NativeMethods.GetHashCode(er.applicationName);
            er.hostId = NativeMethods.GetHashCode(er.hostName);
            er.loginId = NativeMethods.GetHashCode(er.loginName);
            er.startSequence = GetRowInt64(row, ndxStartSequence, -1);
            er.endSequence = GetRowInt64(row, ndxEndSequence, -1);
            er.endTime = GetRowDateTime(row, ndxEndTime);
            er.rowCounts = GetRowCounts(row, ndxRowCounts);

            er.checksum = er.GetHashCode();
            er.eventId = currentId;
            er.hash = chain.GetHashCode(currentId + 1, er.checksum);


            row[ndxHash] = er.hash;
            row[ndxChecksum] = er.checksum;

            return er;
        }

        protected EventRecord
           AddEventRow(
              DataTable table,
              DataRow row
           )
        {
            DataRow dr = table.NewRow();
            EventRecord er = CreateEventRecord(null, row);

            dr["eventClass"] = er.eventClass;
            dr["eventSubclass"] = er.eventSubclass;
            dr["startTime"] = er.startTime;
            dr["spid"] = er.spid;
            dr["applicationName"] = er.applicationName;
            dr["hostName"] = er.hostName;
            dr["serverName"] = er.serverName;
            dr["loginName"] = er.loginName;
            dr["success"] = er.success;
            dr["databaseName"] = er.databaseName;
            dr["databaseId"] = er.databaseId;
            dr["dbUserName"] = er.dbUserName;
            dr["objectType"] = er.objectType;
            dr["objectName"] = er.objectName;
            dr["objectId"] = er.objectId;

            dr["permissions"] = er.permissions;

            dr["columnPermissions"] = er.columnPermissions;
            dr["targetLoginName"] = er.targetLoginName;
            dr["targetUserName"] = er.targetUserName;
            dr["roleName"] = er.roleName;
            dr["ownerName"] = er.ownerName;

            dr["targetObject"] = er.targetObject;
            dr["details"] = er.details;
            dr["eventType"] = er.eventType;
            dr["eventCategory"] = er.eventCategory;
            dr["privilegedUser"] = er.privilegedUser;

            dr["checksum"] = er.checksum;
            dr["eventId"] = er.eventId;
            dr["hash"] = er.hash;
            dr["alertLevel"] = er.alertLevel;

            dr["fileName"] = er.fileName;
            dr["linkedServerName"] = er.linkedServerName;
            dr["parentName"] = er.parentName;
            dr["isSystem"] = er.isSystem;
            dr["sessionLoginName"] = er.sessionLoginName;
            dr["providerName"] = er.providerName;
            dr["appNameId"] = er.appNameId;
            dr["hostId"] = er.hostId;
            dr["loginId"] = NativeMethods.GetHashCode(er.loginName);

            dr["startSequence"] = er.startSequence;
            dr["endSequence"] = er.endSequence;
            if (er.rowCounts == null)
            {
                dr["rowCounts"] = DBNull.Value;
            }
            else
            {
                dr["rowCounts"] = er.rowCounts;
            }
            if (er.endTime != DateTime.MaxValue && er.endTime != DateTime.MinValue)
                dr["endTime"] = er.endTime;
            if (row.Table.Columns.Count > ndxGUId)
                dr["guid"] = row[ndxGUId];
            table.Rows.Add(dr);

            return er;


        }

        #endregion

        #region Bulk Insert

        private static int _bulkInsertTimeout = -1;

        /// <summary>
        /// This method reads bulk insert timeout from settings file.
        /// Settings file must be stored at assembly's location.
        /// Timeout settings are stored in first line.
        /// </summary>
        /// <returns>Bulk insert timeout.</returns>
        private static int GetBulkInsertTimeout()
        {
            // already read timeout from settings file, return it
            if (_bulkInsertTimeout != -1)
                return _bulkInsertTimeout;

            bool settingsRead = false;

            string assemblyPath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            if (!string.IsNullOrEmpty(assemblyPath))
            {
                // try to read settings file for bulk insert timeout
                // we are storing it in first line of file
                string configurationFilePath = Path.Combine(assemblyPath, "settings.conf");
                if (File.Exists(configurationFilePath))
                {
                    string[] settings = File.ReadAllLines(configurationFilePath);
                    if (settings.Length > 0 && int.TryParse(settings[0], out _bulkInsertTimeout))
                        settingsRead = true;
                }
            }

            if (!settingsRead)
                _bulkInsertTimeout = CoreConstants.sqlcommandTimeout;

            return _bulkInsertTimeout;
        }

        /// <summary>
        /// This method will perform bulk insertion operation using SqlBulkCopy class.
        /// This method tries bulk insertion three times before failing and will throw exception on failture.
        /// It will return connection and transaction which it creates so they can be controlled by caller.
        /// Returned transaction is uncomitted and is to be committed by caller.
        /// </summary>
        /// <param name="destinationTableName">Destination table to which insert has to be performed.</param>
        /// <param name="sourceTable">Data to insert in destination table.</param>
        /// <param name="connection">An open SQL connection to use for bulk insertion.</param>
        /// <param name="transaction">Transaction to carry out bulk insert operation.</param>
        private static void BulkInsert(SqlConnection connection, SqlTransaction transaction, string destinationTableName, DataTable sourceTable)
        {
            if (connection == null || transaction == null || sourceTable.Rows.Count == 0)
                return;

            // we will try to do bulk copy maximum three times
            const int maxRetries = 3;
            byte noOfRetries = 0;

            bool bulkCopySuccessed;
            Exception lastKnownException = null;
            do
            {
                string savePoint = Guid.NewGuid().ToString("N");
                try
                {
                    noOfRetries += 1;

                    // create transaction save point
                    transaction.Save(savePoint);

                    // log bulk insert operation for more than one attempts
                    if (noOfRetries > 1)
                    {
                        StringBuilder errorMessageBuilder = new StringBuilder();
                        errorMessageBuilder.AppendFormat("Bulk Insert:{0}", Environment.NewLine);
                        errorMessageBuilder.AppendFormat("\tAttempt Number: {1}{0}", Environment.NewLine, noOfRetries);
                        errorMessageBuilder.AppendFormat("\tTable: {1}{0}", Environment.NewLine, destinationTableName);
                        errorMessageBuilder.AppendFormat("\tNumber of Rows: {1}{0}", Environment.NewLine, sourceTable.Rows.Count);
                        errorMessageBuilder.AppendFormat("\tError: {1}{0}", Environment.NewLine, lastKnownException != null ? lastKnownException.Message : "N/A");

                        ErrorLog.Instance.Write(ErrorLog.Level.Debug, errorMessageBuilder.ToString());
                    }

                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
                    {
                        bulkCopy.DestinationTableName = destinationTableName;
                        bulkCopy.BulkCopyTimeout = GetBulkInsertTimeout();
                        bulkCopy.WriteToServer(sourceTable);

                        bulkCopySuccessed = true;
                    }
                }
                catch (Exception ex)
                {
                    lastKnownException = ex;
                    bulkCopySuccessed = false;

                    // rollback to savepoint before retrying
                    transaction.Rollback(savePoint);
                }
            } while (bulkCopySuccessed == false && noOfRetries < maxRetries);

            sourceTable.Rows.Clear();

            if (!bulkCopySuccessed)
                throw lastKnownException;
        }

        #endregion

        #region Write SQL Table Row

        /*
        EventSqlRecord esr = new EventSqlRecord();
*/

        //-----------------------------------------------------------------------
        // WriteSqlTableRow
        //-----------------------------------------------------------------------
        /*
                private void
                    WriteSqlTableRow(
                    DataRow row,
                    int eventId,
                    SqlConnection conn,
                    SqlTransaction transaction
                    )
                {
         
                    esr.eventId = eventId;
                    esr.startTime = GetRowDateTime(row, ndxStartTime);
                    esr.hash = GetRowInt32(row, ndxHash);

                    esr.sqlText = GetRowString(row, ndxTextData);
                    if (esr.sqlText != "")
                    {
                        if ((jobInfo.maxSql > 0) && (esr.sqlText.Length > jobInfo.maxSql))
                        {
                            esr.sqlText = esr.sqlText.Substring(0, jobInfo.maxSql);
                            esr.sqlText += " ...";
                        }

                        esr.Insert(conn,
                                   jobInfo.eventDatabase,
                                   transaction);
                    }
                }
        */


        //
        // Add a new row to the eventSqlTable for insertion.
        //
        private void
           AddSqlTableRow(DataRow row,
                           int eventId,
                           DataTable table)
        {
            DataRow newRow = table.NewRow();
            newRow["eventId"] = eventId;
            newRow["startTime"] = GetRowDateTime(row, ndxStartTime);
            newRow["hash"] = GetRowInt32(row, ndxHash);
            string sqlText = GetRowString(row, ndxTextData);

            if (sqlText != "")
            {
                if ((jobInfo.maxSql > 0) && (sqlText.Length > jobInfo.maxSql))
                {
                    sqlText = sqlText.Substring(0, jobInfo.maxSql);
                    sqlText += " ...";
                }
            }
            newRow["sqlText"] = sqlText;
            table.Rows.Add(newRow);
        }
        // SQLCM-5471 v5.6 Add Activity to Senstitive columns
        private void
           AddSqlTableRowForSensitiveColumns(DataRow row,
                           int eventId,
                           DataTable table)
        {
            DataRow newRow = table.NewRow();
            newRow["eventId"] = eventId;
            newRow["startTime"] = GetRowDateTime(row, ndxStartTime);
            newRow["hash"] = GetRowInt32(row, ndxHash);
            string sqlText = GetRowString(row, ndxTextData);

            if (sqlText != "")
            {
                if ((jobInfo.maxSql > 0) && (sqlText.Length > jobInfo.maxSql))
                {
                    sqlText = sqlText.Substring(0, jobInfo.maxSql);
                    sqlText += " ...";
                }
            }
            
            if (isSensitiveDataQuery(sqlText.ToUpper()))
            {
                sqlText = string.Empty;
            }
            
            newRow["sqlText"] = sqlText;
            table.Rows.Add(newRow);
        }
        // SQLCM-5471 v5.6 Add Activity to Senstitive columns - END
        #endregion

        #region DBCC SQL Parsing Logic

        //------------------------------------------------------------------------------
        // GetDbccDatabaseInfo
        //------------------------------------------------------------------------------
        private void
            GetDbccDatabaseInfo(
            DataRow row
            )
        {
            if (row.IsNull("textData")) return;

            // Parse DBCC command to set DBCC_read and DBCC_write
            // The following commands are SQLServer 200 read commands - all
            // others are treated as write commands
            string cmd = GetToken(GetRowString(row, ndxTextData), "(", 1);
            cmd = cmd.ToUpper();

            // get database info - many commands are of format
            // DBCC COMMAND ( 'databasename' )
            string dbName;
            int dbId;

            GetDbccDatabaseInfo(cmd,
                                GetRowString(row, ndxTextData),
                                GetRowString(row, ndxDatabaseName),
                                GetRowInt32(row, ndxDatabaseId),
                                out dbName,
                                out dbId);

            // special format databasename.owner.table
            //if ( cmd.StartsWith("DBREINDEX") )

            row[ndxDatabaseId] = dbId;
            row[ndxDatabaseName] = dbName;
        }

        #endregion

        #region Column Calculation

        private static void InitEventsTableColumns(DataTable table, SqlConnection conn)
        {
            InitTableColumns(table, CoreConstants.RepositoryEventsTable, eventsCols, conn);
        }

        private static void InitEventSqlTableColumns(DataTable table, SqlConnection conn)
        {
            InitTableColumns(table, CoreConstants.RepositoryEventSqlTable, eventSqlCols, conn);
        }

        //
        // Add source table's columns to a DataTable
        //
        private static void InitTableColumns(DataTable table, string sourceTable, List<DataColumn> colList, SqlConnection conn)
        {
            table.Columns.Clear();
            lock (lookupLock)
            {
                if (colList.Count == 0)
                {
                    using (DataTable tmpTable = new DataTable())
                    {
                        string query =
                           String.Format("SELECT * FROM {0} WHERE 1 = 0",
                                         sourceTable);
                        using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
                        {
                            da.Fill(tmpTable);
                        }
                        foreach (DataColumn column in tmpTable.Columns)
                            colList.Add(column);
                    }
                }
            }

            foreach (DataColumn column in colList)
            {
                table.Columns.Add(new DataColumn(column.ColumnName, column.DataType));
            }
        }


        //---------------------------------------------------------------------------------
        // GetTargetObject
        //
        // builds more complete string then objectName (adds owner to front of object name
        // if it doesnt exist alread
        //---------------------------------------------------------------------------------
        private string
            GetTargetObject()
        {
            StringBuilder temp = new StringBuilder("");

            // special case - if name contains a 0
            // we have only seen this with CREATE VIEW where we got a dbo\0name
            string obj;
            int pos = objectName.IndexOf("\0");
            if (pos != -1 && pos < (objectName.Length - 1))
            {
                obj = objectName.Substring(pos + 1);
            }
            else
            {
                obj = objectName;
            }

            temp.Append(obj);

            return temp.ToString();
        }

        // Initialize the type offset mapping hashtable
        private static void InitOffsetTable()
        {
            // We are already in a lock, so this function has protected access to the statics

            offsetTableInitialized = true;
            int[,] objectTypeMap =
                {

                    // type  offset

                    {8259, 4}, // CheckConstraint_2005               

                    {8260, 5}, // DefaultConstraint_2005              

                    {8262, 6}, // ForeignKeyConstraint_2005          

                    {8272, 8}, // StoredProcedure_2005               

                    {8274, 10}, // Rule_2005                          

                    {8275, 12}, // SystemTable_2005                    

                    {8276, 25}, // TriggerOnServer_2005               

                    {8277, 17}, // UserTable_2005                     

                    {8278, 18}, // View_2005                          

                    {8280, 19}, // ExtendedStoredProcedure_2005        

                    {16724, 26}, // CLRTrigger_2005                    

                    {16964, 2}, // Database_2005                      

                    {16975, 3}, // Object_2005                        

                    {17222, 27}, // FullTextCatalog_2005               

                    {17232, 28}, // CLRStoredProcedure_2005            

                    {17235, 29}, // Schema_2005                        

                    {17475, 30}, // Credential_2005                    

                    {17491, 31}, // DDLEvent_2005                      

                    {17741, 32}, // ManagementEvent_2005               

                    {17747, 33}, // SecurityEvent_2005                 

                    {17749, 34}, // UserEvent_2005                     

                    {17985, 35}, // CLRAggregateFunction_2005          

                    {17993, 15}, // InlineTableValuedSQLFunction_2005  

                    {18000, 36}, // PartitionFunction_2005             

                    {18002, 11}, // ReplicationFilterProcedure_2005    

                    {18004, 37}, // TableValuedSQLFunction_2005        

                    {18259, 38}, // ServerRole_2005                    

                    {18263, 39}, // MicrosoftWindowsGroup_2005         

                    {19265, 40}, // AsymmetricKey_2005                 

                    {19277, 41}, // MasterKey_2005                     

                    {19280, 7}, // PrimaryKey_2005                    

                    {19283, 42}, // ObfusKey_2005                      

                    {19521, 43}, // AsymmetricKeyLogin_2005            

                    {19523, 44}, // CertificateLogin1_2005             

                    {19538, 45}, // Role_2005                          

                    {19539, 46}, // SQLLogin_2005                      

                    {19543, 47}, // WindowsLogin_2005                  

                    {20034, 48}, // RemoteServiceBinding_2005          

                    {20036, 49}, // EventNotificationOnDatabase_2005   

                    {20037, 50}, // EventNotification_2005             

                    {20038, 51}, // ScalarSQLFunction_2005             

                    {20047, 52}, // EventNotificationOnObject_2005     

                    {20051, 53}, // Synonym_2005                       

                    {20549, 54}, // EndPoint_2005                      

                    {20801, 20}, // CachedAdhocQueries_2005            

                    {20816, 21}, // CachedAdhocQueries2_2005           

                    {20819, 55}, // ServiceBrokerServiceQueue_2005     

                    {20821, 16}, // UniqueConstraint_2005              

                    {21057, 56}, // ApplicationRole_2005               

                    {21059, 57}, // Certificate_2005                   

                    {21075, 58}, // Server_2005                        

                    {21076, 59}, // TransactSQLTrigger_2005            

                    {21313, 60}, // Assembly_2005                      

                    {21318, 61}, // CLRScalarFunction_2005             

                    {21321, 14}, // InlineScalarSQLFunction_2005       

                    {21328, 62}, // PartitionScheme_2005               

                    {21333, 63}, // User1_2005                         

                    {21571, 64}, // ServiceBrokerServiceContract_2005  

                    {21572, 13}, // TriggerOnDatabase_2005             

                    {21574, 65}, // CLRTableValuedFunction_2005        

                    {21577, 66}, // InternalTable_2005                 

                    {21581, 67}, // ServiceBrokerMessageType_2005      

                    {21586, 68}, // ServiceBrokerRoute_2005            

                    {21587, 22}, // Statistics_2005                    

                    {21825, 69}, // User2_2005                         

                    {21827, 69}, // User3_2005                         

                    {21831, 69}, // User4_2005                         

                    {21843, 69}, // User5_2005                         

                    {21847, 69}, // User6_2005                         

                    {22099, 70}, // ServiceBrokerService_2005          

                    {22601, 1}, // Index_2005

                    {22604, 71}, // CertificateLogin2_2005

                    {22611, 72}, // XMLSchema_2005

                    {22868, 73} // Type_2005

                };

            int numElements = objectTypeMap.Length / 2;

            for (int i = 0; i < numElements; i++)
            {
                typeOffsetTable.Add(objectTypeMap[i, 0], objectTypeMap[i, 1]);
            }
            return;
        }

        //--------------------------------------------------------------------
        // GetEventTypeOffset: returns event type offset based on object type
        //--------------------------------------------------------------------
        private int
            GetEventTypeOffset()
        {
            lock (lookupLock)
            {
                if (!offsetTableInitialized)
                    InitOffsetTable();
            }

            try
            {
                if (objectType <= 22)
                    return objectType;
                else // SQL Server 2005 Object Types Mapping
                    return (int)typeOffsetTable[objectType];
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                           String.Format("Invalid object type: {0}.  Object type mapping failed.", objectType),
                                        e,
                                        ErrorLog.Severity.Warning);

            }

            return -1;
        }





        #region Main routine for calculating columns

        //------------------------------------------------------------------------------
        // CreateCalculatedColumns
        //------------------------------------------------------------------------------
        private bool
            CreateCalculatedColumns(
            DataRow row
            )
        {
            bool retval;

            // Load Generic
            switch (eventClass)
            {
                // Login Events
                case (int)TraceEventId.Login:
                    retval = LoginEventHandler(row);
                    break;
                case (int)TraceEventId.Logout:
                    retval = LogoutEventHandler(row);
                    break;
                case (int)TraceEventId.LoginFailed:
                    retval = LoginFailedEventHandler(row);
                    break;

                // Login Events (SQL Server 2005)	            
                case (int)TraceEventId.AuditServerPrincipalImpersonation:
                    retval = AuditServerPrincipalImpersonationEventHandler(row);
                    break;
                case (int)TraceEventId.AuditDatabasePrincipalImpersonation:
                    retval = AuditDatabasePrincipalImpersonationEventHandler(row);
                    break;

                // DDL: Schema Object 
                case (int)TraceEventId.AuditObjectDerivedPermission:
                    retval = AuditObjectDerivedPermissionEventHandler(row);
                    break;
                case (int)TraceEventId.AuditStatementPermission:
                    retval = AuditStatementPermissionEventHandler(row);
                    break;

                // DDL (SQL Server 2005)	            
                case (int)TraceEventId.AuditDatabaseManagement:
                    retval = AuditDatabaseManagementEventHandler(row);
                    break;
                case (int)TraceEventId.AuditDatabaseObjectManagement:
                    retval = AuditDatabaseObjectManagementEventHandler(row);
                    break;
                case (int)TraceEventId.AuditSchemaObjectManagement:
                    retval = AuditSchemaObjectManagementEventHandler(row);
                    break;
                case (int)TraceEventId.AuditServerObjectManagement:
                    retval = AuditServerObjectManagementEventHandler(row);
                    break;

                // Security Events
                case (int)TraceEventId.AuditObjectGDR:
                    retval = AuditObjectGdrEventHandler(row);
                    break;
                case (int)TraceEventId.AuditStatementGDR:
                    retval = AuditStatementGdrEventHandler(row);
                    break;
                case (int)TraceEventId.AuditLoginGDR:
                    retval = AuditLoginGdrEventHandler(row);
                    break;
                case (int)TraceEventId.AuditLoginChange:
                    retval = AuditLoginChangeEventHandler(row);
                    break;
                case (int)TraceEventId.AuditLoginChangePassword:
                    retval = AuditLoginChangePasswordEventHandler(row);
                    break;
                case (int)TraceEventId.AuditAddLogin:
                    retval = AuditAddLoginEventHandler(row);
                    break;
                case (int)TraceEventId.AuditAddLoginToServer:
                    retval = AuditAddLoginToServerEventHandler(row);
                    break;
                case (int)TraceEventId.AuditAddDbUser:
                    retval = AuditAddDbUserEventHandler(row);
                    break;
                case (int)TraceEventId.AuditAddMember:
                    retval = AuditAddMemberEventHandler(row);
                    break;
                case (int)TraceEventId.AuditAddDropRole:
                    retval = AuditAddRoleEventHandler(row);
                    break;
                case (int)TraceEventId.AppRolePassChange:
                    retval = AppRolePassChangeEventHandler(row);
                    break;

                // Security Events (SQL Server 2005)
                case (int)TraceEventId.AuditDatabasePrincipalManagement:
                    retval = AuditDatabasePrincipalManagementEventHandler(row);
                    break;
                case (int)TraceEventId.AuditServerObjectTakeOwnership:
                    retval = AuditServerObjectTakeOwnershipEventHandler(row);
                    break;
                case (int)TraceEventId.AuditDatabaseObjectTakeOwnership:
                    retval = AuditDatabaseObjectTakeOwnershipEventHandler(row);
                    break;
                case (int)TraceEventId.AuditChangeDatabaseOwner:
                    retval = AuditChangeDatabaseOwnerEventHandler(row);
                    break;
                case (int)TraceEventId.AuditSchemaObjectTakeOwnership:
                    retval = AuditSchemaObjectTakeOwnershipEventHandler(row);
                    break;
                case (int)TraceEventId.AuditServerScopeGDR:
                    retval = AuditServerScopeGDREventHandler(row);
                    break;
                case (int)TraceEventId.AuditServerObjectGDR:
                    retval = AuditServerObjectGDREventHandler(row);
                    break;
                case (int)TraceEventId.AuditDatabaseObjectGDR:
                    retval = AuditDatabaseObjectGDREventHandler(row);
                    break;
                case (int)TraceEventId.AuditServerPrincipalManagement:
                    retval = AuditServerPrincipalManagementEventHandler(row);
                    break;

                // DML    
                case (int)TraceEventId.AuditObjectPermission:
                    retval = AuditObjectPermissionEventHandler(row);
                    break;

                case (int)TraceEventId.Transaction:
                    retval = SQLTransaction(row);
                    break;

                // Admin   
                case (int)TraceEventId.AuditBackupRestore:
                    retval = AuditBackupRestoreEventHandler(row);
                    break;
                case (int)TraceEventId.AuditChangeAudit:
                    retval = AuditChangeAuditEventHandler(/*row*/);
                    break;

                // Admin (SQL Server 2005)    
                case (int)TraceEventId.AuditServerOperation:
                    retval = AuditServerOperationEventHandler(row);
                    break;
                case (int)TraceEventId.AuditServerAlterTrace:
                    retval = AuditServerAlterTraceEventHandler(row);
                    break;
                case (int)TraceEventId.AuditDatabaseOperation:
                    retval = AuditDatabaseOperationEventHandler(row);
                    break;

                // Admin (DBCC)
                case (int)TraceEventId.AuditDbcc:
                    retval = AuditDbccEventHandler(row);
                    break;

                // Access (SQL Server 2005)
                case (int)TraceEventId.AuditDatabaseObjectAccess:
                    retval = AuditDatabaseObjectAccessEventHandler(row);
                    break;

                case (int)TraceEventId.UserEvent0:
                case (int)TraceEventId.UserEvent1:
                case (int)TraceEventId.UserEvent2:
                case (int)TraceEventId.UserEvent3:
                case (int)TraceEventId.UserEvent4:
                case (int)TraceEventId.UserEvent5:
                case (int)TraceEventId.UserEvent6:
                case (int)TraceEventId.UserEvent7:
                case (int)TraceEventId.UserEvent8:
                case (int)TraceEventId.UserEvent9:
                    retval = AuditUserDefinedEventsHandler(row);
                    break;

                default:
                    retval = false;
                    break;
            }

            if (jobInfo.traceFile.Contains("xel"))
            {
                retval = AuditObjectPermissionEventHandler(row);
            }

            if (retval)
            {
                if (jobInfo.privilegedUserTrace ||
                   (!row.IsNull(ndxPrivilegedUser) && Convert.ToInt32(row[ndxPrivilegedUser]) == 1))
                    row[ndxPrivilegedUser] = 1;
                else
                    row[ndxPrivilegedUser] = 0;
                row[ndxNewRow] = 0;

                //only set the time for commit, save and rollback transaction events.  Yes, the right side of the OR is probably redundant.
                //if (eventClass != (int)TraceEventId.Transaction || (eventClass == (int)TraceEventId.Transaction && eventSubclass == (int)TraceSubclass.BeginTran))
                //{
                // convert startTime to UTC
                row[ndxStartTime] = TimeZoneInfo.ToUniversalTime(jobInfo.timeZoneInfo, startTime);

                if (endTime != DateTime.MaxValue && endTime != DateTime.MinValue)
                {
                    endTime = TimeZoneInfo.ToUniversalTime(jobInfo.timeZoneInfo, endTime);
                    row[ndxEndTime] = endTime;
                }
                //}
                //else
                //{
                //if the start time for the transaction has not been set, go ahead and set it.
                //   if (GetRowDateTime(row, ndxStartTime) != startTime)
                //   {
                //      row[ndxStartTime] = startTime;
                //   }
                //   else
                //   {
                //      //they are the same.  Now to determine if it needs to be converted to UTC
                //      if (GetRowDateTime(row, ndxStartTime) != TimeZoneInfo.ToUniversalTime(jobInfo.timeZoneInfo, startTime))
                //      {
                //         row[ndxStartTime] = TimeZoneInfo.ToUniversalTime(jobInfo.timeZoneInfo, startTime);
                //      }
                //   }
                //}
            }
            else
            {
                if (ErrorLog.Instance.ErrorLevel >= ErrorLog.Level.Debug)
                {
                    // if this is not a login event then it was an invalid event
                    // if it is a login, we just skipped because it was not unique
                    if (eventClass != (int)TraceEventId.Login)
                    {
                        ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                                String.Format(CoreConstants.Debug_InvalidEventClass,
                                                              jobInfo.traceFile,
                                                          startTime,
                                                              eventClass));
                    }
                }
            }

            return retval;
        }

        #endregion

        #region Login Event Handlers
        //------------------------------------------------------------------------------
        // LoginEventHandler (14)
        //------------------------------------------------------------------------------
        private bool
            LoginEventHandler(
            DataRow row
            )
        {
            // check for login filtering
            if (loginCache != null)
            {
                if (!loginCache.IsLoginUnique(loginName,
                                               applicationName,
                                               hostName,
                                               startTime))
                {
                    // not unique - ignore this login
                    return false;
                }
            }

            // process this login         
            row[ndxEventCategory] = (int)TraceEventCategory.Login;
            row[ndxEventType] = (int)TraceEventType.Login;

            row[ndxTextData] = ""; // login SQL is useless; just eat

            return true;
        }


        //------------------------------------------------------------------------------
        // LogoutFailedEventHandler (20)
        //------------------------------------------------------------------------------
        private bool
            LogoutEventHandler(
            DataRow row
            )
        {
            row[ndxEventCategory] = (int)TraceEventCategory.Login;
            row[ndxEventType] = (int)TraceEventType.Logout;

            return true;
        }

        //------------------------------------------------------------------------------
        // LoginFailedEventHandler (20)
        //------------------------------------------------------------------------------
        private bool
            LoginFailedEventHandler(
            DataRow row
            )
        {
            row[ndxEventCategory] = (int)TraceEventCategory.Login;
            row[ndxEventType] = (int)TraceEventType.LoginFailed;

            return true;
        }

        //------------------------------------------------------------------------------
        // AuditServerPrincipalImpersonationEventHandler (132) (SQL Server 2005)
        //------------------------------------------------------------------------------
        private bool
            AuditServerPrincipalImpersonationEventHandler(
            DataRow row
            )
        {
            row[ndxEventCategory] = (int)TraceEventCategory.Login;
            row[ndxEventType] = (int)TraceEventType.ServerImpersonation;

            row[ndxTargetObject] = GetTargetObject();
            row[ndxDetails] = String.Format("{0} executes as {1}", sessionLoginName, loginName);

            return true;
        }

        //------------------------------------------------------------------------------
        // AuditDatabasePrincipalImpersonationEventHandler (133) (SQL Server 2005)
        //------------------------------------------------------------------------------
        private bool
            AuditDatabasePrincipalImpersonationEventHandler(
            DataRow row
            )
        {
            row[ndxEventCategory] = (int)TraceEventCategory.Login;
            row[ndxEventType] = (int)TraceEventType.DatabaseImersonation;

            row[ndxTargetObject] = objectName;
            row[ndxDetails] = String.Format("{0} executes as {1}", sessionLoginName, loginName);

            return true;
        }

        #endregion

        #region DDL Event Handlers
        //------------------------------------------------------------------------------
        // AuditObjectDerivedPermissionEventHandler (118)
        // --  It is renamed to SchemaObjectManagement Event.
        //------------------------------------------------------------------------------
        private bool
            AuditObjectDerivedPermissionEventHandler(
            DataRow row
            )
        {
            // calculate operation type = base + objectType
            TraceEventType opType;
            if (eventSubclass == (int)TraceSubclass.Create)
                opType = TraceEventType.CreateBase;
            else if (eventSubclass == (int)TraceSubclass.Alter)
                opType = TraceEventType.AlterBase;
            else if (eventSubclass == (int)TraceSubclass.Drop)
                opType = TraceEventType.DropBase;
            else if (eventSubclass == (int)TraceSubclass.Dump)
                opType = TraceEventType.DumpBase;
            else if (eventSubclass == (int)TraceSubclass.Open)
                opType = TraceEventType.OpenBase;
            else if (eventSubclass == (int)TraceSubclass.Load)
                opType = TraceEventType.LoadBase;
            else if (eventSubclass == (int)TraceSubclass.Access)
                opType = TraceEventType.AccessBase;
            else
                return false;


            row[ndxEventCategory] = (int)TraceEventCategory.DDL;

            int offset = GetEventTypeOffset();
            if (offset < 0)
                return false;
            opType += offset;
            row[ndxEventType] = (int)opType;

            if (objectType == (int)DBObjectType.Database ||
                objectType == (int)DBObjectType.Database_2005)
            {
                // special code for DROP DATABASE (failure case) - set database field not object field
                if (eventSubclass == (int)TraceSubclass.Drop && success == 0)
                {
                    string db = GetToken(GetRowString(row, ndxTextData), 2);
                    row[ndxDatabaseName] = db;
                    row[ndxDatabaseId] = -1;
                }
            }
            else
            {
                row[ndxTargetObject] = GetTargetObject();
            }

            return true;
        }

        //------------------------------------------------------------------------------
        // HandleAuditStatementPermissionEventHandler (113)
        //------------------------------------------------------------------------------
        private bool
            AuditStatementPermissionEventHandler(
            DataRow row
            )
        {
            TraceEventType opType;

            switch (permissions)
            {
                case (int)TracePermissions.CreateDatabase:
                    opType = TraceEventType.CreateDatabase;
                    break;
                case (int)TracePermissions.CreateTable:
                    opType = TraceEventType.CreateUserTable;
                    break;
                case (int)TracePermissions.CreateProcedure:
                    opType = TraceEventType.CreateStoredProcedure;
                    break;
                case (int)TracePermissions.CreateView:
                    opType = TraceEventType.CreateView;
                    break;
                case (int)TracePermissions.CreateRule:
                    opType = TraceEventType.CreateRule;
                    row[ndxObjectType] = DBObjectType.Rule;
                    break;
                case (int)TracePermissions.CreateDefault:
                    opType = TraceEventType.CreateDEFAULT;
                    row[ndxObjectType] = DBObjectType.DefaultConstraint;
                    break;
                case (int)TracePermissions.BackupDatabase:
                    opType = TraceEventType.BackupDatabase;
                    break;
                case (int)TracePermissions.BackupTable:
                    opType = TraceEventType.BackupTable;
                    break;
                case (int)TracePermissions.BackupLog:
                    opType = TraceEventType.BackupLog;
                    break;
                case (int)TracePermissions.CreateFunction:
                    opType = TraceEventType.CreateUDF;
                    break;
                default:
                    return false;
            }

            row[ndxEventCategory] = (int)TraceEventCategory.DDL;
            row[ndxEventType] = (int)opType;

            if (permissions == (int)TracePermissions.CreateDatabase && success == 0)
            {
                // special case for database drop/create with access failure to 
                // force sql parsing - object will be filled in with master at this point and since access check
                // failed we didnt have a ObjectCreate or ObjectDrop
                string dbName = GetToken(GetRowString(row, ndxTextData), 2);
                if (dbName != "")
                {
                    row[ndxDatabaseName] = StripBrackets(dbName);
                }

                row[ndxDatabaseId] = -1;
            }
            else if (permissions != (int)TracePermissions.CreateDatabase)
            {
                if (permissions != (int)TracePermissions.CreateRule &&
                    permissions != (int)TracePermissions.CreateDefault &&
                    permissions != (int)TracePermissions.CreateFunction)
                {
                    row[ndxTargetObject] = GetTargetObject();
                }

                string target = GetRowString(row, ndxObjectName);

                if (target == "" ||
                    permissions == (int)TracePermissions.CreateRule ||
                    permissions == (int)TracePermissions.CreateDefault ||
                    permissions == (int)TracePermissions.CreateFunction)
                {
                    // need to parse rule name for CREATE RULE <name> and CREATE DEFAULT - not anywhere but in SQL
                    string ruleName = GetToken(GetRowString(row, ndxTextData), 2);
                    if (ruleName != "" && ruleName.Substring(0, 1) != "@")
                    {
                        row[ndxTargetObject] = StripBrackets(ruleName);
                    }
                }
            }

            return true;
        }

        //------------------------------------------------------------------------------
        // AuditDatabaseManagementEventHandler (128) (SQL Server 2005)
        //------------------------------------------------------------------------------
        private bool
            AuditDatabaseManagementEventHandler(
            DataRow row
            )
        {
            TraceEventType opType;

            if (eventSubclass == (int)TraceSubclass.Create)
                opType = TraceEventType.CreateBase;
            else if (eventSubclass == (int)TraceSubclass.Alter)
                opType = TraceEventType.AlterBase;
            else if (eventSubclass == (int)TraceSubclass.Drop)
                opType = TraceEventType.DropBase;
            else if (eventSubclass == (int)TraceSubclass.Dump)
                opType = TraceEventType.DumpBase;
            else if (eventSubclass == (int)TraceSubclass.Load)
                opType = TraceEventType.LoadBase;
            else
                return false;

            row[ndxEventCategory] = (int)TraceEventCategory.DDL;

            row[ndxEventType] = opType + (int)DBObjectType.Database;

            // Override database name and ID when necessary
            if (databaseName != objectName)
            {
                row[ndxDatabaseName] = objectName;
                row[ndxDatabaseId] = -1;
            }
            row[ndxTargetObject] = objectName;

            return true;
        }

        //------------------------------------------------------------------------------
        // AuditDatabaseObjectManagementEventHandler (129) (SQL Server 2005)
        //------------------------------------------------------------------------------
        private bool
            AuditDatabaseObjectManagementEventHandler(
            DataRow row
            )
        {
            // calculate operation type = base + objectType
            TraceEventType opType;
            if (eventSubclass == (int)TraceSubclass.Create)
                opType = TraceEventType.CreateBase;
            else if (eventSubclass == (int)TraceSubclass.Alter)
                opType = TraceEventType.AlterBase;
            else if (eventSubclass == (int)TraceSubclass.Drop)
                opType = TraceEventType.DropBase;
            else if (eventSubclass == (int)TraceSubclass.Dump)
                opType = TraceEventType.DumpBase;
            else if (eventSubclass == (int)TraceSubclass.Load)
                opType = TraceEventType.LoadBase;
            else
                return false;

            row[ndxEventCategory] = (int)TraceEventCategory.DDL;
            int offset = GetEventTypeOffset();
            if (offset == -1)
                return false;
            row[ndxEventType] = (int)opType + offset;

            row[ndxTargetObject] = objectName;

            return true;
        }

        //------------------------------------------------------------------------------
        // AuditSchemaObjectManagementEventHandler (131) (SQL Server 2005)
        //------------------------------------------------------------------------------
        private bool
            AuditSchemaObjectManagementEventHandler(
            DataRow row
            )
        {
            TraceEventType opType;
            if (eventSubclass == (int)TraceSubclass.Create)
                opType = TraceEventType.CreateBase;
            else if (eventSubclass == (int)TraceSubclass.Alter)
            {
                opType = TraceEventType.AlterBase;

                if (CoreConstants.ParseForUpdateStats
                    && objectType == (int)DBObjectType.UserTable_2005
                    && !row.IsNull("textData"))
                {
                    string[] tokens = GetTokens(GetRowString(row, ndxTextData), 2);
                    if (tokens.Length >= 2 &&
                         tokens[0].ToUpper().Equals("UPDATE") &&
                         tokens[1].ToUpper().Equals("STATISTICS"))
                        objectType = (int)DBObjectType.Statistics_2005;
                }
            }
            else if (eventSubclass == (int)TraceSubclass.Drop)
                opType = TraceEventType.DropBase;
            else if (eventSubclass == (int)TraceSubclass.Dump)
                opType = TraceEventType.DumpBase;
            else if (eventSubclass == (int)TraceSubclass.Load)
                opType = TraceEventType.LoadBase;
            else if (eventSubclass == (int)TraceSubclass.Transfer)
                opType = TraceEventType.TransferBase;
            else
                return false;
            
            int offset = GetEventTypeOffset();
            if (offset == -1)
                return false;
            string txtData = row[ndxTextData].ToString().ToUpper();
            if (!txtData.StartsWith("TRUNCATE"))
            {
                row[ndxEventCategory] = (int)TraceEventCategory.DDL;
                row[ndxEventType] = (int)opType + offset;
            }
            else
            {
            	// SQLCM-5921 DML: 'Integrity Check - Invalid' type event generated when 'Truncate' operation is done on a table
            	row[ndxEventCategory] = (int)TraceEventCategory.DML;
                row[ndxEventType] = (int)TraceEventType.DELETE;
            }

            row[ndxTargetObject] = GetTargetObject();

            return true;
        }

        //------------------------------------------------------------------------------
        // AuditServerObjectManagementEventHandler (176) (SQL Server 2005)
        //------------------------------------------------------------------------------
        private bool
            AuditServerObjectManagementEventHandler(
            DataRow row
            )
        {
            TraceEventType opType;
            int offset = GetEventTypeOffset();
            if (offset == -1)
                return false;

            if (eventSubclass == (int)TraceSubclass.Create)
                opType = TraceEventType.CreateBase + offset;
            else if (eventSubclass == (int)TraceSubclass.Alter)
                opType = TraceEventType.AlterBase + offset;
            else if (eventSubclass == (int)TraceSubclass.Drop)
                opType = TraceEventType.DropBase + offset;
            else if (eventSubclass == (int)TraceSubclass.Dump)
                opType = TraceEventType.DumpBase + offset;
            else if (eventSubclass == (int)TraceSubclass.CredentialMappedToLogin) // Login objects only
                opType = TraceEventType.CredentialMapped;
            else if (eventSubclass == (int)TraceSubclass.CredentialMapDropped) // Login objects only
                opType = TraceEventType.CredentialMapDropped;
            else if (eventSubclass == (int)TraceSubclass.Load)
                opType = TraceEventType.LoadBase + offset;
            else
                return false;

            // event category mappings
            row[ndxEventCategory] = (int)TraceEventCategory.DDL;
            row[ndxEventType] = (int)opType;

            row[ndxTargetObject] = GetTargetObject();

            return true;
        }

        #endregion

        #region Security Event Handlers

        //------------------------------------------------------------------------------
        // AuditObjectGdrEventHandler (103)
        // -- Renamed to Schema Object GDR event in SQL 2005
        //------------------------------------------------------------------------------
        private bool
            AuditObjectGdrEventHandler(
            DataRow row
            )
        {
            // calculate operation type = base + objectType
            TraceEventType opType;
            if (eventSubclass == (int)TraceSubclass.Grant)
                opType = TraceEventType.GrantObjectGdrBase;
            else if (eventSubclass == (int)TraceSubclass.Revoke)
                opType = TraceEventType.RevokeObjectGdrBase;
            else if (eventSubclass == (int)TraceSubclass.Deny)
                opType = TraceEventType.DenyObjectGdrBase;
            else
                return false;

            /*
         int offset = GetEventTypeOffset();
         if( offset == -1 )
            return false;
         opType += offset;
         */

            StringBuilder stmt = new StringBuilder("");

            AddGDRString(stmt, 1, "SELECT");
            AddGDRString(stmt, 2, "UPDATE");
            AddGDRString(stmt, 4, "REFERENCES");
            AddGDRString(stmt, 8, "INSERT");
            AddGDRString(stmt, 16, "DELETE");
            AddGDRString(stmt, 32, "EXECUTE");
            AddGDRString(stmt, 4096, "SELECT ANY");
            AddGDRString(stmt, 8192, "UPDATE ANY");
            AddGDRString(stmt, 16384, "REFERENCES ANY");

            row[ndxEventCategory] = (int)TraceEventCategory.Security;

            row[ndxEventType] = (int)opType;
            row[ndxTargetObject] = objectName;
            row[ndxDetails] = stmt.ToString();
            row[ndxDetails] = TruncateString((string)row[ndxDetails], 512);

            string toList = GetGDRTrailingTokenString(GetRowString(row, ndxTextData));
            if (toList != "")
            {
                row[ndxTargetLoginName] = toList;
                row[ndxTargetLoginName] = TruncateString((string)row[ndxTargetLoginName], 128);
                row[ndxDetails] += " TO " + toList;
                row[ndxDetails] = TruncateString((string)row[ndxDetails], 512);
            }

            return true;
        }

        //------------------------------------------------------------------------------
        // AuditStatementGdrEventHandler (102)
        // Rename to Audit Database Scope GDR in 2005
        //------------------------------------------------------------------------------
        private bool
            AuditStatementGdrEventHandler(
            DataRow row
            )
        {
            TraceEventType opType;
            if (eventSubclass == (int)TraceSubclass.Grant)
                opType = TraceEventType.GrantStmtBase;
            else if (eventSubclass == (int)TraceSubclass.Revoke)
                opType = TraceEventType.RevokeStmtBase;
            else if (eventSubclass == (int)TraceSubclass.Deny)
                opType = TraceEventType.DenyStmtBase;
            else
                return false;

            StringBuilder stmt = new StringBuilder("");

            var permission = GetRowPermissions(row, ndxPermissions);
            switch (permission)
            {
                case -999:
                    AddGDRString(stmt, permission, "ALTER ANY SCHEMA");
                    break;

                case -998:
                    AddGDRString(stmt, permission, "ALTER ANY USER");
                    break;

                default:
                    AddGDRString(stmt, 1, "CREATE DATABASE");
                    AddGDRString(stmt, 2, "CREATE TABLE");
                    AddGDRString(stmt, 4, "CREATE PROCEDURE");
                    AddGDRString(stmt, 8, "CREATE VIEW");
                    AddGDRString(stmt, 16, "CREATE RULE");
                    AddGDRString(stmt, 32, "CREATE DEFAULT");
                    AddGDRString(stmt, 64, "BACKUP DATABASE");
                    AddGDRString(stmt, 128, "BACKUP LOG");
                    AddGDRString(stmt, 512, "CREATE FUNCTION");
                    break;
            }

            row[ndxEventCategory] = (int)TraceEventCategory.Security;
            row[ndxEventType] = (int)opType;
            row[ndxDetails] = stmt.ToString();
            row[ndxDetails] = TruncateString((string)row[ndxDetails], 512);

            string toList = GetGDRTrailingTokenString(GetRowString(row, ndxTextData));
            if (toList != "")
            {
                row[ndxTargetObject] = toList;
                row[ndxTargetObject] = TruncateString((string)row[ndxTargetObject], 512);
                row[ndxTargetLoginName] = toList;
                row[ndxTargetLoginName] = TruncateString((string)row[ndxTargetLoginName], 128);
            }

            return true;
        }

        private void
            AddGDRString(
            StringBuilder sb,
            int flag,
            string stmt
            )
        {
            if ((permissions & flag) != 0)
            {
                if (sb.Length != 0) sb.Append(", ");
                sb.Append(stmt);
            }
        }

        private void
            AddGDRPermissionStrings(StringBuilder stmt)
        {
            switch (objectType)
            {
                case (int)DBObjectType.SQLLogin_2005:
                case (int)DBObjectType.WindowsLogin_2005:
                case (int)DBObjectType.CertificateLogin1_2005:
                case (int)DBObjectType.CertificateLogin2_2005:
                case (int)DBObjectType.MicrosoftWindowsGroup_2005:
                    AddGDRString(stmt, 1, "IMPERSONATE");
                    AddGDRString(stmt, 2, "VIEW DEFINITION");
                    AddGDRString(stmt, 8, "ALTER");
                    AddGDRString(stmt, 64, "CONTROL");
                    break;

                case (int)DBObjectType.EndPoint_2005:
                    AddGDRString(stmt, 1, "CONNECT");
                    AddGDRString(stmt, 2, "VIEW DEFINITION");
                    AddGDRString(stmt, 8, "ALTER");
                    AddGDRString(stmt, 16, "TAKE OWNERSHIP");
                    AddGDRString(stmt, 32, "Permission 32");
                    AddGDRString(stmt, 64, "CONTROL");
                    break;
                default:
                    stmt.AppendFormat("Permissioins {0}", permissions);
                    break;
            }
        }

        //------------------------------------------------------------------------------
        // AuditLoginGdrEventHandler (105)
        //------------------------------------------------------------------------------
        private bool
            AuditLoginGdrEventHandler(
            DataRow row
            )
        {
            TraceEventType opType;
            if (eventSubclass == (int)TraceSubclass.Grant)
                opType = TraceEventType.GrantLogin;
            else if (eventSubclass == (int)TraceSubclass.Revoke)
                opType = TraceEventType.RevokeLogin;
            else if (eventSubclass == (int)TraceSubclass.Deny)
                opType = TraceEventType.DenyLogin;
            else
                return false;

            row[ndxEventCategory] = (int)TraceEventCategory.Security;
            row[ndxEventType] = (int)opType;
            row[ndxTargetObject] = targetLoginName;

            return true;
        }

        //------------------------------------------------------------------------------
        // AuditLoginChangeEventHandler (106)
        //------------------------------------------------------------------------------
        private bool
            AuditLoginChangeEventHandler(
            DataRow row
            )
        {
            if (eventSubclass != (int)TraceSubclass.DefaultDatabase &&
                eventSubclass != (int)TraceSubclass.DefaultLanguage)
            {
                return false;
            }

            row[ndxEventCategory] = (int)TraceEventCategory.Security;
            row[ndxEventType] = (eventSubclass == (int)TraceSubclass.DefaultDatabase)
                                    ? (int)TraceEventType.LoginChangePropertyDB
                                    : (int)TraceEventType.LoginChangePropertyLanguage;
            row[ndxTargetObject] = targetLoginName;

            return true;
        }

        //------------------------------------------------------------------------------
        // AuditLoginChangePasswordEventHandler (107)
        //------------------------------------------------------------------------------
        private bool
            AuditLoginChangePasswordEventHandler(
            DataRow row
            )
        {

            row[ndxEventCategory] = (int)TraceEventCategory.Security;
            switch ((TraceSubclass)eventSubclass)
            {
                case TraceSubclass.ChangedSelf:
                case TraceSubclass.ResetSelf:
                    row[ndxEventType] = (int)TraceEventType.PasswordChangeSelf;
                    break;
                case TraceSubclass.ResetOther:
                case TraceSubclass.ChangedOther:
                case TraceSubclass.PasswordMustChange:
                case TraceSubclass.PasswordUnlocked:
                default:
                    row[ndxEventType] = (int)TraceEventType.PasswordChange;
                    break;
            }

            row[ndxTargetObject] = targetLoginName;

            return true;
        }

        //------------------------------------------------------------------------------
        // AuditAddLoginEventHandler (104)
        //------------------------------------------------------------------------------
        private bool
            AuditAddLoginEventHandler(
            DataRow row
            )
        {
            if (eventSubclass != (int)TraceSubclass.AddLogin &&
                eventSubclass != (int)TraceSubclass.DropLogin)
            {
                return false;
            }

            row[ndxEventCategory] = (int)TraceEventCategory.Security;
            row[ndxEventType] = (eventSubclass == (int)TraceSubclass.AddLogin)
                                    ? (int)TraceEventType.AddLogin
                                    : (int)TraceEventType.DropLogin;
            row[ndxTargetObject] = targetLoginName;

            return true;
        }

        //------------------------------------------------------------------------------
        // AuditAddLoginToServerEventHandler (108)
        //------------------------------------------------------------------------------
        private bool
            AuditAddLoginToServerEventHandler(
            DataRow row
            )
        {
            if (eventSubclass != (int)TraceSubclass.AddLogin &&
                eventSubclass != (int)TraceSubclass.DropLogin)
            {
                return false;
            }

            row[ndxEventCategory] = (int)TraceEventCategory.Security;
            row[ndxEventType] = (eventSubclass == (int)TraceSubclass.AddLogin)
                                    ? (int)TraceEventType.AddLogintoServerRole
                                    : (int)TraceEventType.DropLoginfromServerRole;
            row[ndxTargetObject] = roleName;
            row[ndxDetails] = targetLoginName;

            return true;
        }

        //------------------------------------------------------------------------------
        // AuditAddDbUserEventHandler (109)
        //------------------------------------------------------------------------------
        private bool
            AuditAddDbUserEventHandler(
            DataRow row
            )
        {
            row[ndxEventCategory] = (int)TraceEventCategory.Security;

            switch (eventSubclass)
            {
                case (int)TraceSubclass.AddDatabaseUser:
                    row[ndxEventType] = (int)TraceEventType.AddDatabaseUser;
                    break;
                case (int)TraceSubclass.DropDatabaseUser:
                    row[ndxEventType] = (int)TraceEventType.DropDatabaseUser;
                    break;
                case (int)TraceSubclass.GrantDatabaseAccess:
                    row[ndxEventType] = (int)TraceEventType.GrantDatabaseAccess;
                    break;
                case (int)TraceSubclass.RevokeDatabaseAccess:
                    row[ndxEventType] = (int)TraceEventType.RevokeDatabaseAccess;
                    break;
                default:
                    return false;
            }

            if (targetLoginName != "")
                row[ndxTargetObject] = targetLoginName;
            else
                row[ndxTargetObject] = targetUserName;

            // details
            StringBuilder prop = new StringBuilder("");
            if (roleName != "") prop.AppendFormat("Role name: {0} ", roleName);
            if (targetLoginName != "" && targetUserName != "")
            {
                prop.AppendFormat("User name: {0} ", targetUserName);
            }
            row[ndxDetails] = prop.ToString();

            return true;
        }

        //------------------------------------------------------------------------------
        // AuditAddMemberEventHandler (110)
        //------------------------------------------------------------------------------
        private bool
            AuditAddMemberEventHandler(
            DataRow row
            )
        {
            if (eventSubclass != (int)TraceSubclass.AddLogin &&
                eventSubclass != (int)TraceSubclass.DropLogin)
            {
                return false;
            }

            row[ndxEventCategory] = (int)TraceEventCategory.Security;
            row[ndxEventType] = (eventSubclass == (int)TraceSubclass.AddLogin)
                                    ? (int)TraceEventType.AddMembertoDatabaseRole
                                    : (int)TraceEventType.DropMembertoDatabaseRole;
            row[ndxTargetObject] = roleName;

            StringBuilder prop = new StringBuilder("");
            if (targetUserName != "") prop.AppendFormat("Login name: {0} ", targetLoginName);
            if (targetUserName != "") prop.AppendFormat("User name: {0} ", targetUserName);
            row[ndxDetails] = prop.ToString();

            row[ndxDetails] = targetUserName;

            return true;
        }

        //------------------------------------------------------------------------------
        // AuditAddRoleEventHandler (111)
        //------------------------------------------------------------------------------
        private bool
            AuditAddRoleEventHandler(
            DataRow row
            )
        {
            if (eventSubclass != (int)TraceSubclass.AddLogin &&
                eventSubclass != (int)TraceSubclass.DropLogin)
            {
                return false;
            }

            row[ndxEventCategory] = (int)TraceEventCategory.Security;
            row[ndxEventType] = (eventSubclass == (int)TraceSubclass.AddLogin)
                                    ? (int)TraceEventType.AddDatabaseRole
                                    : (int)TraceEventType.DropDatabaseRole;
            row[ndxTargetObject] = roleName;

            return true;
        }

        //------------------------------------------------------------------------------
        // AppRolePassChangeEventHandler (112)
        //------------------------------------------------------------------------------
        private bool
            AppRolePassChangeEventHandler(
            DataRow row
            )
        {
            row[ndxEventCategory] = (int)TraceEventCategory.Security;
            row[ndxEventType] = (int)TraceEventType.AppRoleChangePassword;
            row[ndxTargetObject] = roleName;

            return true;
        }

        //------------------------------------------------------------------------------
        // AuditDatabasePrincipalManagementEventHandler (130) (SQL Server 2005)
        //------------------------------------------------------------------------------
        private bool
            AuditDatabasePrincipalManagementEventHandler(
            DataRow row
            )
        {
            TraceEventType opType;
            switch (eventSubclass)
            {
                case (int)TraceSubclass.Create:
                    opType = TraceEventType.CreateBase;
                    break;
                case (int)TraceSubclass.Drop:
                    opType = TraceEventType.DropBase;
                    break;
                case (int)TraceSubclass.Alter:
                    opType = TraceEventType.AlterBase;
                    break;
                case (int)TraceSubclass.Dump:
                    opType = TraceEventType.DumpBase;
                    break;
                case (int)TraceSubclass.Load:
                    opType = TraceEventType.LoadBase;
                    break;
                default:
                    return false;
            }
            row[ndxEventCategory] = (int)TraceEventCategory.Security;
            opType += GetEventTypeOffset();
            row[ndxEventType] = (int)opType;

            row[ndxTargetObject] = GetTargetObject();

            return true;
        }

        //------------------------------------------------------------------------------
        // AuditServerObjectTakeOwnershipEventHandler (134) (SQL Server 2005)
        //------------------------------------------------------------------------------
        private bool
            AuditServerObjectTakeOwnershipEventHandler(
            DataRow row
            )
        {
            row[ndxEventCategory] = (int)TraceEventCategory.Security;
            row[ndxEventType] = TraceEventType.ServerObjectChangeOwner;

            if (targetLoginName != "")
                row[ndxDetails] = string.Format("New owner: {0}", targetLoginName);
            else
                row[ndxDetails] = string.Format("New owner: {0}", targetUserName);

            row[ndxTargetObject] = GetTargetObject();

            return true;
        }

        //------------------------------------------------------------------------------
        // AuditDatabaseObjectTakeOwnershipEventHandler (135) (SQL Server 2005)
        //------------------------------------------------------------------------------
        private bool
            AuditDatabaseObjectTakeOwnershipEventHandler(
            DataRow row
            )
        {
            row[ndxEventCategory] = (int)TraceEventCategory.Security;
            row[ndxEventType] = TraceEventType.DatabaseObjectChangeOwner;

            if (targetLoginName != "")
                row[ndxDetails] = string.Format("New owner: {0}", targetLoginName);
            else
                row[ndxDetails] = string.Format("New owner: {0}", targetUserName);

            row[ndxTargetObject] = objectName;


            return true;
        }

        //------------------------------------------------------------------------------
        // AuditChangeDatabaseOwnerEventHandler (152) (SQL Server 2005)
        //------------------------------------------------------------------------------
        private bool
            AuditChangeDatabaseOwnerEventHandler(
            DataRow row
            )
        {
            row[ndxEventCategory] = (int)TraceEventCategory.Security;
            row[ndxEventType] = TraceEventType.ChangeDatabaseOwner;

            row[ndxTargetObject] = objectName;
            if (targetLoginName != "")
                row[ndxDetails] = string.Format("New owner: {0}", targetLoginName);
            else
                row[ndxDetails] = string.Format("New owner: {0}", targetUserName);


            return true;
        }

        //------------------------------------------------------------------------------
        // AuditSchemaObjectTakeOwnershipEventHandler (153) (SQL Server 2005)
        //------------------------------------------------------------------------------
        private bool
            AuditSchemaObjectTakeOwnershipEventHandler(
            DataRow row
            )
        {
            row[ndxEventCategory] = (int)TraceEventCategory.Security;
            row[ndxEventType] = TraceEventType.SchemaObjectChangeOwner;

            row[ndxTargetObject] = GetTargetObject();
            if (targetLoginName != "")
                row[ndxDetails] = string.Format("New owner: {0}", targetLoginName);
            else
                row[ndxDetails] = string.Format("New owner: {0}", targetUserName);


            return true;
        }

        //------------------------------------------------------------------------------
        // AuditServerScopeGDREventHandler (170) (SQL Server 2005)
        //------------------------------------------------------------------------------
        private bool
            AuditServerScopeGDREventHandler(
            DataRow row
            )
        {
            TraceEventType opType;

            if (eventSubclass == (int)TraceSubclass.Grant)
                opType = TraceEventType.GrantObjectGdrBase;
            else if (eventSubclass == (int)TraceSubclass.Revoke)
                opType = TraceEventType.RevokeObjectGdrBase;
            else if (eventSubclass == (int)TraceSubclass.Deny)
                opType = TraceEventType.DenyObjectGdrBase;
            else
                return false;

            /*
         int offset = GetEventTypeOffset();
         if( offset == -1 )
            return false;
         opType += offset;
         */
            row[ndxEventCategory] = (int)TraceEventCategory.Security;
            row[ndxEventType] = opType;


            row[ndxTargetObject] = GetTargetObject();
            StringBuilder stmt = new StringBuilder("");
            AddGDRPermissionStrings(stmt);

            stmt.AppendFormat(" TO {0}", targetLoginName != "" ? targetLoginName : targetUserName);
            row[ndxDetails] = stmt.ToString();
            row[ndxDetails] = TruncateString((string)row[ndxDetails], 512);

            return true;
        }

        private string TruncateString(string input, int maxSize)
        {
            string temp;

            if (input.Length < maxSize)
                return input;

            temp = input.Substring(0, maxSize - 3);
            temp += "...";
            return temp;
        }

        //------------------------------------------------------------------------------
        // AuditServerObjectGDREventHandler (171) (SQL Server 2005)
        //------------------------------------------------------------------------------
        private bool
            AuditServerObjectGDREventHandler(
            DataRow row
            )
        {
            TraceEventType opType;
            if (eventSubclass == (int)TraceSubclass.Grant)
                opType = TraceEventType.GrantObjectGdrBase;
            else if (eventSubclass == (int)TraceSubclass.Revoke)
                opType = TraceEventType.RevokeObjectGdrBase;
            else if (eventSubclass == (int)TraceSubclass.Deny)
                opType = TraceEventType.DenyObjectGdrBase;
            else
                return false;

            /*
         int offset = GetEventTypeOffset();
         if( offset == -1 )
            return false;
         opType += offset;
         */
            row[ndxEventCategory] = (int)TraceEventCategory.Security;
            row[ndxEventType] = opType;


            row[ndxTargetObject] = GetTargetObject();
            StringBuilder stmt = new StringBuilder("");
            AddGDRPermissionStrings(stmt);

            stmt.AppendFormat(" TO {0}", targetLoginName != "" ? targetLoginName : targetUserName);

            row[ndxDetails] = stmt.ToString();
            row[ndxDetails] = TruncateString((string)row[ndxDetails], 512);
            return true;
        }

        //------------------------------------------------------------------------------
        // AuditDatabaseObjectGDREventHandler (172) (SQL Server 2005)
        //------------------------------------------------------------------------------
        private bool
            AuditDatabaseObjectGDREventHandler(
            DataRow row
            )
        {
            TraceEventType opType;
            if (eventSubclass == (int)TraceSubclass.Grant)
                opType = TraceEventType.GrantObjectGdrBase;
            else if (eventSubclass == (int)TraceSubclass.Revoke)
                opType = TraceEventType.RevokeObjectGdrBase;
            else if (eventSubclass == (int)TraceSubclass.Deny)
                opType = TraceEventType.DenyObjectGdrBase;
            else
                return false;

            /*
         int offset = GetEventTypeOffset();
         if( offset == -1 )
            return false;
         opType += offset;
         */

            row[ndxEventCategory] = (int)TraceEventCategory.Security;
            row[ndxEventType] = (int)opType;
            row[ndxTargetObject] = objectName;
            StringBuilder stmt = new StringBuilder("");

            AddGDRString(stmt, 1, "SELECT");
            AddGDRString(stmt, 2, "UPDATE");
            AddGDRString(stmt, 4, "REFERENCES");
            AddGDRString(stmt, 8, "INSERT");
            AddGDRString(stmt, 16, "DELETE");
            AddGDRString(stmt, 32, "EXECUTE");
            AddGDRString(stmt, 4096, "SELECT ANY");
            AddGDRString(stmt, 8192, "UPDATE ANY");
            AddGDRString(stmt, 16384, "REFERENCES ANY");

            stmt.AppendFormat(" TO {0}", targetLoginName != "" ? targetLoginName : targetUserName);

            row[ndxDetails] = stmt.ToString();
            row[ndxDetails] = TruncateString((string)row[ndxDetails], 512);
            return true;
        }

        //------------------------------------------------------------------------------
        // AuditServerPrincipalManagementEventHandler (177) (SQL Server 2005)
        //------------------------------------------------------------------------------
        private bool
            AuditServerPrincipalManagementEventHandler(
            DataRow row
            )
        {
            TraceEventType opType;
            bool getTypeOffset = true;
            if (eventSubclass == (int)TraceSubclass.Create)
                opType = TraceEventType.CreateBase;
            else if (eventSubclass == (int)TraceSubclass.Alter)
                opType = TraceEventType.AlterBase;
            else if (eventSubclass == (int)TraceSubclass.Drop)
                opType = TraceEventType.DropBase;
            else if (eventSubclass == (int)TraceSubclass.Dump)
                opType = TraceEventType.DumpBase;
            else if (eventSubclass == (int)TraceSubclass.Disable)
            {
                opType = TraceEventType.DisableLogin;
                getTypeOffset = false;
            }
            else if (eventSubclass == (int)TraceSubclass.Enable)
            {
                opType = TraceEventType.EnableLogin;
                getTypeOffset = false;
            }
            else if (eventSubclass == (int)TraceSubclass.Load)
                opType = TraceEventType.LoadBase;
            else
                return false;

            row[ndxEventCategory] = (int)TraceEventCategory.Security;
            if (getTypeOffset)
            {
                int offset = GetEventTypeOffset();
                if (offset == -1)
                    return false;
                opType += offset;
            }
            row[ndxEventType] = (int)opType;

            // Special processing for failed Server Principal Management event.
            // Exception event for ServerPrincipalManagement event comes before it when there is an error.
            // The success column for this event can be 1.  The only indication of this action failed is the
            // invalid login name.
            row[ndxTargetObject] = GetTargetObject();

            return true;
        }

        #endregion

        #region DML Event Handlers
        //------------------------------------------------------------------------------
        // AuditObjectPermissionEventHandler (114)
        //------------------------------------------------------------------------------
        private bool
            AuditObjectPermissionEventHandler(
            DataRow row
            )
        {
            if (permissions == (int)TracePermissions.SelectAll ||
                permissions == (int)TracePermissions.ReferencesAll)
            {
                row[ndxEventCategory] = (int)TraceEventCategory.SELECT;
            }
            else if (permissions == (int)TracePermissions.UpdateAll ||
                     permissions == (int)TracePermissions.Insert ||
                     permissions == (int)TracePermissions.Delete ||
                     permissions == (int)TracePermissions.Execute)
            {
                row[ndxEventCategory] = (int)TraceEventCategory.DML;
                row[ndxStartSequence] = GetRowInt64(row, ndxEventSequence);
            }
            // SQLCM-5471 v5.6 Add Activity to Senstitive columns
            else if (permissions == (int)TracePermissions.AlterUserTable)
            {
                row[ndxEventCategory] = (int)TraceEventCategory.DDL;
                row[ndxStartSequence] = GetRowInt64(row, ndxEventSequence);
            }
            // SQLCM-5471 v5.6 Add Activity to Senstitive columns- END
            else
            {
                return false;
            }

            row[ndxEventType] = permissions;
            objectName = GetRowString(row, ndxObjectName);

            // target object
            row[ndxTargetObject] = GetTargetObject();

            return true;
        }

        //------------------------------------------------------------------------------
        // SQLTransaction (50)
        //------------------------------------------------------------------------------
        private bool SQLTransaction(DataRow row)
        {
            switch (eventSubclass)
            {
                case (int)TraceSubclass.BeginTran:
                    row[ndxEventType] = (int)TraceEventType.BeginTran;
                    break;
                case (int)TraceSubclass.CommitTran:
                    row[ndxEventType] = (int)TraceEventType.CommitTran;
                    break;
                case (int)TraceSubclass.RollbackTran:
                    row[ndxEventType] = (int)TraceEventType.RollbackTran;
                    break;
                case (int)TraceSubclass.SaveTran:
                    row[ndxEventType] = (int)TraceEventType.SaveTran;
                    break;
                default:
                    return false;
            }

            //We only care about user transactions.  They start at 51
            if (spid < 51)
                return false;

            row[ndxEventCategory] = (int)TraceEventCategory.DML;
            row[ndxStartSequence] = GetRowInt64(row, ndxEventSequence);
            return true;
        }

        #endregion

        #region Broker Event Handlers

        //------------------------------------------------------------------------------
        // AuditBrokerConversationEventHandler (158) (SQL Server 2005)
        //------------------------------------------------------------------------------
        /*
                private bool
                    AuditBrokerConversationEventHandler(
                    DataRow row
                    )
                {
                    row[ndxEventCategory] = (int) TraceEventCategory.Broker;
                    row[ndxEventType] = (int) TraceEventType.BrokerConversation;

                    StringBuilder details = new StringBuilder("Error: ");
                    if (eventSubclass == (int) TraceSubclass.NoSecurityHeader)
                        details.Append("No security header.");
                    else if (eventSubclass == (int) TraceSubclass.NoCertificate)
                        details.Append("No certificate.");
                    else if (eventSubclass == (int) TraceSubclass.InvalidSignature)
                        details.Append("Invalid signature.");
                    else if (eventSubclass == (int) TraceSubclass.RunAsTargetFailure)
                        details.Append("Run as target failure.");
                    else
                        details.AppendFormat("Unknown error (subclass={0}).", eventSubclass);

                    if (textData != "")
                        details.AppendFormat("Message: {0}", textData);

                    row[ndxDetails] = details.ToString();

                    return true;
                }
        */

        //------------------------------------------------------------------------------
        // AuditBrokerLoginEventHandler (159) (SQL Server 2005)
        //------------------------------------------------------------------------------
        /*
                private bool
                    AuditBrokerLoginEventHandler(
                    DataRow row
                    )
                {
                    row[ndxEventCategory] = (int) TraceEventCategory.Broker;
                    row[ndxEventType] = (int) TraceEventType.BrokerLogin;

                    StringBuilder details = new StringBuilder("");
                    details.AppendFormat("Authenticatioin method: {0}. ", providerName);
                    details.AppendFormat("Login state: {0}. ", targetUserName);
                    details.AppendFormat("Local supported methods: {0}. ", ownerName);
                    details.AppendFormat("Remote supported methods: {0}. ", fileName);
                    row[ndxDetails] = details.ToString();

                    return true;
                }
        */

        #endregion

        #region Admin Type Things

        //------------------------------------------------------------------------------
        // AuditBackupRestoreEventHandler
        //------------------------------------------------------------------------------
        private bool
            AuditBackupRestoreEventHandler(
            DataRow row
            )
        {
            if (eventSubclass != (int)TraceSubclass.Backup &&
                eventSubclass != (int)TraceSubclass.Restore &&
                eventSubclass != (int)TraceSubclass.BackupLog)
            {
                return false;
            }

            // For 2000, the subclass is 1 for all backups, and 2 for restores
            //  For 2005, the subclass is 1 for backup, 2 for restore, and 3 for log backup
            row[ndxEventCategory] = (int)TraceEventCategory.Admin;
            int eventType = 0;
            switch (eventSubclass)
            {
                case 1:
                    eventType = (int)TraceEventType.Backup;
                    break;
                case 2:
                    eventType = (int)TraceEventType.Restore;
                    break;
                case 3:
                    eventType = (int)TraceEventType.BackupLog;
                    break;
            }
            row[ndxEventType] = eventType;
            return true;
        }

        //------------------------------------------------------------------------------
        // AuditChangeAuditEventHandler (117)
        //------------------------------------------------------------------------------
        private bool
            AuditChangeAuditEventHandler(
            //DataRow           row
            )
        {
            // these dont provide any useful information - we already capture the event in the exec
            // we will just eat for now
            return false;

            /*         
                  if ( eventSubclass != 1 && eventSubclass!= 2) return false;
         
                  row[ndxEventCategory] = (int)TraceEventCategory.Admin;
                  row[ndxEventType]     = (eventSubclass == 1 ) ? (int)TraceEventType.AuditStarted
                                                               : (int)TraceEventType.AuditStopped;
         
                  return true;
         */
        }

        //------------------------------------------------------------------------------
        // AuditServerOperationEventHandler (173) (SQL Server 2005)
        //------------------------------------------------------------------------------
        private bool
            AuditServerOperationEventHandler(
            DataRow row
            )
        {
            row[ndxEventCategory] = (int)TraceEventCategory.Admin;
            row[ndxEventType] = (int)TraceEventType.ServerOperation;
            if (eventSubclass == (int)TraceSubclass.AdminBulkOperations)
                row[ndxDetails] = "Administrator bulk Operations";
            else if (eventSubclass == (int)TraceSubclass.AlterSettings)
                row[ndxDetails] = "Alter settings";
            else if (eventSubclass == (int)TraceSubclass.AlterResource)
                row[ndxDetails] = "Alter resources";
            else if (eventSubclass == (int)TraceSubclass.Authenticate)
                row[ndxDetails] = "Authenticate";
            else if (eventSubclass == (int)TraceSubclass.AlterServerState)
                row[ndxDetails] = "Alter server state";
            else if (eventSubclass == (int)TraceSubclass.ExternalAccess)
                row[ndxDetails] = "External access";
            else
                return false;

            row[ndxTargetObject] = objectName;

            return true;
        }

        //------------------------------------------------------------------------------
        // AuditServerAlterTraceEventHandler (175) (SQL Server 2005)
        //------------------------------------------------------------------------------
        private bool
            AuditServerAlterTraceEventHandler(
            DataRow row
            )
        {
            row[ndxEventCategory] = (int)TraceEventCategory.Admin;
            row[ndxEventType] = (int)TraceEventType.ServerAlterTrace;
            row[ndxDetails] = sessionLoginName;
            return true;
        }

        //------------------------------------------------------------------------------
        // AuditDatabaseOperationEventHandler (178) (SQL Server 2005)
        //------------------------------------------------------------------------------
        private bool
            AuditDatabaseOperationEventHandler(
            DataRow row
            )
        {
            row[ndxEventCategory] = (int)TraceEventCategory.Admin;
            row[ndxEventType] = (int)TraceEventType.DatabaseOperation;

            if (eventSubclass == (int)TraceSubclass.CheckPoint)
                row[ndxDetails] = "Checkpoint";
            else if (eventSubclass == (int)TraceSubclass.SubscribeToQueryNotification)
                row[ndxDetails] = "Subscribe to query notification";
            else
                return false;

            return true;
        }

        #endregion

        #region Access Handler (SQL Server 2005)

        //------------------------------------------------------------------------------
        // AuditDatabaseObjectAccessEventHandler (180) (SQL Server 2005)
        //------------------------------------------------------------------------------
        private bool
            AuditDatabaseObjectAccessEventHandler(
            DataRow row
            )
        {
            if (permissions == (int)TracePermissions.SelectAll ||
                permissions == (int)TracePermissions.ReferencesAll)
            {
                row[ndxEventCategory] = (int)TraceEventCategory.SELECT;
            }
            else if (permissions == (int)TracePermissions.UpdateAll ||
                     permissions == (int)TracePermissions.Insert ||
                     permissions == (int)TracePermissions.Delete ||
                     permissions == (int)TracePermissions.Execute)
            {
                row[ndxEventCategory] = (int)TraceEventCategory.DML;
            }
            else
            {
                return false;
            }

            row[ndxEventType] = permissions;

            // target object
            row[ndxTargetObject] = objectName;

            return true;
        }

        #endregion

        #region DBCC Event Handler

        static string[] dbccReadCommands = new string[]
            {
                "CHECKALLOC",
                "CHECKCATALOG",
                "CHECKCONSTRAINTS",
                "CHECKDB",
                "CHECKFILEGROUP",
                "CHECKIDENT",
                "CHECKTABLE",
                "CONCURRENCYVIOLATION",
                "HELP",
                "INPUTBUFFER",
                "MEMUSAGE",
                "NEWALLOC",
                "OPENTRAN",
                "OUTPUTBUFFER",
                "PERFMON",
                "SQLPERF",
                "PROCCACHE",
                "SHOW_STATISTICS",
                "SHOWCONTIG",
                "TRACESTATUS",
            
                // undocumented read commands
                // from http://www.sql-server-performance.com/ac_sql_server_2000_undocumented_dbcc.asp
                "BUFFER",
                "BYTES",
                "DBINFO",
                "DBTABLE",
                "DES",
                "HELP",
                "IND",
                "LOG",
                "PAGE",
                "PROCBUF",
                "PRTIPAGE",
                "PSS",
                "RESOURCE",
                "TAB",
            
                // some new SQL Server 2005 commands
                "SHOWFILESTATS",
                "USEROPTIONS"
            
            
            };

        static string[] dbccDatabaseCommands = new string[]
            {
                "CHECKALLOC",
                "CHECKCATALOG",
                "CHECKCONSTRAINTS",
                "CHECKDB",
                "CHECKIDENT",
                "CHECKTABLE",
                "CLEANTABLE", // 'name' or id
                "DBREINDEX", // db.owner.table
                "INDEXDEFRAG", // 'name' or id, dsasadsa
                "OPENTRAN", // 'name' or id
                "PINTABLE", // id
                "SHOWCONTIG",
                "SHOW_STATISTICS",
                "SHRINKDATABASE", // 'name', ddds
                "UNPINTABLE", // id
                "UPDATEUSAGE", // 'database' | 0 , sdsdfds
            
                // undocumented dbcc commands
                // from http://www.sql-server-performance.com/ac_sql_server_2000_undocumented_dbcc.asp
                "BUFFER", // [dbid|dbname] [,objid|objname] [,nbufs], [printopt])
                "DBINFO", // [(dbname)]
                "DBTABLE", // ({dbid|dbname})
                "DES", // [([dbid|dbname] [,objid|objname])]
                "IND", // (dbid|dbname, objid|objname,  printopt = {-2|-1|0|1|2|3})
                "LOG", // ({dbid|dbname}, [, type={-1|0|1|2|3|4}])
                "PAGE", // ({dbid|dbname}, pagenum [,print option] [,cache] [,logical])
                "PROCBUF", // ([dbid|dbname], [objid|objname], [nbufs], [printopt = {0|1}])
            
                // SQL Server 2005
                "CALLFULLTEXT",
                "FLUSHPROCINDB"
            };

        static string[] dbccTableCommands = new string[]
            {
                "CHECKCONSTRAINTS",
                "CHECKIDENT",
                "CHECKTABLE",
                "DBREINDEX",
                "SHOWCONTIG",
                "SHOW_STATISTICS",
            };


        static string[] dbccIdCommands = new string[]
            {
                "CLEANTABLE",
                "INDEXDEFRAG",
                "OPENTRANS",
                "UPDATEUSAGE",
                "CALLFULLTEXT",
                "FLUSHPROCINDB"
            };

        //------------------------------------------------------------------------------
        // AuditDbccEventHandler
        //------------------------------------------------------------------------------
        private bool
            AuditDbccEventHandler(
            DataRow row
            )
        {
            row[ndxEventCategory] = (int)TraceEventCategory.Admin;

            if (row.IsNull("textData"))
            {
                // NO SQL - just log DBCC command
                row[ndxEventType] = (int)TraceEventType.DBCC;
            }
            else
            {
                row[ndxEventType] = (int)TraceEventType.DBCC_write;

                // Parse DBCC command to set DBCC_read and DBCC_write
                // The following commands are SQLServer 200 read commands - all
                // others are treated as write commands
                string cmd = GetToken(GetRowString(row, ndxTextData), "(", 1);
                cmd = cmd.ToUpper();

                row[ndxDetails] = cmd;

                foreach (string d in dbccReadCommands)
                {
                    if (cmd.StartsWith(d))
                    {
                        row[ndxEventType] = (int)TraceEventType.DBCC_read;
                        break;
                    }
                }
            }
            return true;
        }

        //------------------------------------------------------------------------------
        // IsDatabaseCmd
        //------------------------------------------------------------------------------
        private bool
            IsDatabaseCmd(
            string sql
            )
        {
            // see if this is one of those commands
            bool found = false;
            foreach (string d in dbccDatabaseCommands)
            {
                if (sql.StartsWith(d))
                {
                    found = true;
                    break;
                }
            }
            return found;
        }

        //------------------------------------------------------------------------------
        // IsTableCmd
        //------------------------------------------------------------------------------
        private bool
            IsTableCmd(
            string sql
            )
        {
            // see if this is one of those commands
            bool found = false;
            foreach (string d in dbccTableCommands)
            {
                if (sql.StartsWith(d))
                {
                    found = true;
                    break;
                }
            }
            return found;
        }

        //------------------------------------------------------------------------------
        // IsIdCmd
        //------------------------------------------------------------------------------
        private bool
            IsIdCmd(
            string sql
            )
        {
            // see if this is one of those commands
            bool found = false;
            foreach (string d in dbccIdCommands)
            {
                if (sql.StartsWith(d))
                {
                    found = true;
                    break;
                }
            }
            return found;
        }

        //------------------------------------------------------------------------------
        // GetDbccDatabaseInfo
        //------------------------------------------------------------------------------
        private void
            GetDbccDatabaseInfo(
            string dbccCommand,
            string sql,
            string inDbName,
            int inDbId,
            out string outDbName,
            out int outDbId
            )
        {
            // see if this is one of those commands
            if (!IsDatabaseCmd(dbccCommand) &&
                 !IsTableCmd(dbccCommand))
            {
                outDbName = "";
                // blank out so there is no database context since there is none for the command anyway
                outDbId = 0;
                // this stops them from being filtered out at the database level
                return;
            }

            // get database token         
            string token = GetToken(sql, "(", 2, true);
            if (token == null || token == "")
            {
                outDbName = "";
                outDbId = 0;
                return;
            }

            if (token[0] != '(')
            {
                outDbName = "";
                outDbId = 0;
                return;
            }
            else
            {
                // eat leading ( and whitespace
                token = token.Substring(1);
                token = token.Trim();
            }

            // database format could be 'name', name or ID
            // name format could be database.owner.table,owner.table or table

            //------------
            // Case 1: ID
            //------------
            if (IsIdCmd(dbccCommand) && Char.IsDigit(token[0]))
            {
                outDbId = 0;

                try
                {
                    outDbId = Convert.ToInt32(token);
                }
                catch
                { }

                if (outDbId == 0)
                {
                    outDbName = inDbName;
                    outDbId = inDbId;
                }
                else
                {
                    outDbName = String.Format("databaseId={0}", outDbId);
                }
            }
            else
            {
                string db = "";

                if (token[0] == '\'')
                {
                    //-----------------------------
                    // Case 2: Single Quoted Token
                    //-----------------------------
                    // database name - escape sequence == two single quotes
                    token = token.Substring(1);

                    // look for trailing singlequote
                    bool inString = true;

                    while (inString)
                    {
                        int pos;
                        pos = token.IndexOf("'");
                        if (pos == -1)
                        {
                            // string ended before finding closing single quote - error
                            outDbName = "";
                            outDbId = 0;
                            return;
                        }

                        if (token.Substring(pos).StartsWith("''"))
                        {
                            db += token.Substring(0, pos + 1);
                            token = token.Substring(pos + 2);
                        }
                        else
                        {
                            db += token.Substring(0, pos);
                            inString = false;
                        }
                    }
                }
                else
                {
                    //-------------------------
                    // Case 3: Non-quoted Name
                    //-------------------------
                    string searchString = ",)";
                    char[] anyOf = searchString.ToCharArray();

                    int pos = token.IndexOfAny(anyOf);
                    if (pos != -1)
                    {
                        db = token.Substring(0, pos);
                    }
                }

                outDbName = "";
                outDbId = 0;
                if (db.Length > 0 && db[0] != '@')
                // eat normalized DBCC commands so they fall through and show up in auditing
                {
                    if (IsTableCmd(dbccCommand))
                    {
                        db = GetDatabaseFromTable(db, inDbName);
                        if (db.ToUpper() == inDbName.ToUpper())
                        {
                            db = inDbName;
                            outDbId = inDbId;
                        }
                    }
                    outDbName = db;
                }
            }
        }

        //------------------------------------------------------------------------------
        // GetDatabaseFromTable
        //------------------------------------------------------------------------------
        private string
            GetDatabaseFromTable(
            string fullName,
            string defaultDbName
            )
        {
            string name = fullName.Trim();
            // possible strings
            //    dbname.owner.table
            //    dbname..table
            //    owner.table
            //    table
            // any token can be in []s
            // double closing bracket gets ignored
            int count = 0;
            string[] token = new string[3];

            while (name.Length > 0 && count < 3)
            {
                if (name[0] == '.')
                {
                    name = name.Substring(1);
                }
                // token either from 0 to . or [ to ]
                int pos = 0;
                if (name[0] == '[')
                {
                    // token in brackets - find closing bracket
                    bool looking = true;
                    while (looking)
                    {
                        pos = name.IndexOf("]", pos);
                        if (pos == -1)
                        {
                            // no closing bracket - bad syntax
                            return "";
                        }
                        else
                        {
                            // ] found, check for double bracket escape sequence
                            if ((pos + 1 == name.Length) || (name[pos + 1] != ']'))
                            {
                                // have the name - save without brackets
                                token[count++] = name.Substring(1, pos - 1);
                                if (pos + 1 == name.Length)
                                    name = "";
                                else
                                    name = name.Substring(pos + 1);
                                looking = false;
                            }
                            else
                            {
                                // skip square brackets
                                pos = pos + 2;
                                if (pos >= name.Length)
                                {
                                    // no closing bracket - bad syntax
                                    return "";
                                }
                            }
                        }
                    }
                }
                else
                {
                    // token not in brackets - find next period
                    pos = name.IndexOf(".");
                    if (pos == -1)
                    {
                        token[count++] = name;
                        name = "";
                    }
                    else
                    {
                        token[count++] = name.Substring(0, pos);
                        if (pos == 0)
                        {
                            if (name.Length > 1)
                            {
                                name = name.Substring(pos + 1);
                            }
                            else
                            {
                                name = "";
                            }
                        }
                        else
                        {
                            name = name.Substring(pos);
                        }
                    }
                }
            }

            if (count < 3)
            {
                return defaultDbName;
            }
            else
            {
                return token[0];
            }
        }

        #endregion

        #region User Defined Events Handler

        private bool AuditUserDefinedEventsHandler(DataRow row)
        {
            bool retval = true;
            TraceEventType opType = TraceEventType.UserDefinedEvent0;

            switch (eventClass)
            {
                case (int)TraceEventId.UserEvent0:
                    opType = TraceEventType.UserDefinedEvent0;
                    break;
                case (int)TraceEventId.UserEvent1:
                    opType = TraceEventType.UserDefinedEvent1;
                    break;
                case (int)TraceEventId.UserEvent2:
                    opType = TraceEventType.UserDefinedEvent2;
                    break;
                case (int)TraceEventId.UserEvent3:
                    opType = TraceEventType.UserDefinedEvent3;
                    break;
                case (int)TraceEventId.UserEvent4:
                    opType = TraceEventType.UserDefinedEvent4;
                    break;
                case (int)TraceEventId.UserEvent5:
                    opType = TraceEventType.UserDefinedEvent5;
                    break;
                case (int)TraceEventId.UserEvent6:
                    opType = TraceEventType.UserDefinedEvent6;
                    break;
                case (int)TraceEventId.UserEvent7:
                    opType = TraceEventType.UserDefinedEvent7;
                    break;
                case (int)TraceEventId.UserEvent8:
                    opType = TraceEventType.UserDefinedEvent8;
                    break;
                case (int)TraceEventId.UserEvent9:
                    opType = TraceEventType.UserDefinedEvent9;
                    break;
                default:
                    retval = false;
                    break;
            }

            if (retval)
            {
                row[ndxEventCategory] = (int)TraceEventCategory.UserDefined;
                row[ndxEventType] = (int)opType;
                row[ndxDetails] = textData;
                row[ndxDetails] = TruncateString((string)row[ndxDetails], 512);
            }
            return retval;
        }

        #endregion



        #region Token Parsing
        //------------------------------------------------------------------------------
        // GetToken
        //------------------------------------------------------------------------------
        private string
            GetToken(
            string cmd,
            int tokenNumber // zero based
            )
        {
            return GetToken(cmd, "", tokenNumber, false);
        }

        private string
            GetToken(
            string cmd,
            string extraChars,
            int tokenNumber // zero based
            )
        {
            return GetToken(cmd, extraChars, tokenNumber, false);
        }

        /*
      private string
         GetToken(
            string  cmd,
            int     tokenNumber, // zero based
            bool    returnWholeString
         )
      {
         return GetToken( cmd, "", tokenNumber, returnWholeString );
      }
      */



        private string
            GetToken(
            string cmd,
            string extraChars,
            int tokenNumber, // zero based
            bool returnWholeString
            )
        {
            string token = "";

            cmd = EatComments(cmd);

            string[] tokens = cmd.Split();

            char[] anyOf = extraChars.ToCharArray();

            int count = 0;
            for (int i = 0; i < tokens.Length; i++)
            {
                if (tokens[i].Length == 0) continue;

                // look for extra chars
                if ((extraChars != "") && (count <= tokenNumber))
                {
                    int pos = tokens[i].IndexOfAny(anyOf);
                    if (pos != -1)
                    {
                        if (count == tokenNumber)
                        {
                            if (pos == 0)
                            {
                                tokens[i] = tokens[i].Substring(pos);
                            }
                            else
                            {
                                tokens[i] = tokens[i].Substring(0, pos);
                            }
                        }
                        else
                        {
                            count++;
                            tokens[i] = tokens[i].Substring(pos);
                        }
                    }
                }

                if (count == tokenNumber)
                {
                    StringBuilder tokenBuilder = new StringBuilder(1024);

                    tokenBuilder.Append(tokens[i]);
                    if (returnWholeString)
                    {
                        // add on rest of tokens
                        for (int j = i + 1; j < tokens.Length; j++)
                        {
                            tokenBuilder.Append(" " + tokens[j]);
                        }
                    }

                    token = tokenBuilder.ToString();

                    break;
                }
                else
                {
                    count++;
                }
            }

            if (token.StartsWith("@"))
            {
                // parameter marker not real value
                token = "Unknown";
            }

            if (token.StartsWith("[") && token.EndsWith("]"))
            {
                // eat brackets around database name
                token = token.Substring(1, token.Length - 2);
            }

            return token;
        }

        private string[] GetTokens(string sqlText, int nTokens)
        {
            if (nTokens <= 0 || sqlText == null)
                return new string[0];

            sqlText = EatComments(sqlText);
            return sqlText.Split(null, nTokens + 1);
        }

        //------------------------------------------------------------------------------
        // EatComments
        //------------------------------------------------------------------------------
        private string
            EatComments(
            string inString
            )
        {
            // input validation
            if (inString == null) return inString;
            if (inString.Length == 0) return inString;

            StringBuilder outString = new StringBuilder(2048);
            bool inSingleQuote = false;
            bool inDoubleQuote = false;
            bool inLineComment = false;
            bool inSlashStarComment = false;

            for (int i = 0; i < inString.Length; i++)
            {
                char c = inString[i];

                if (i == inString.Length - 1)
                {
                    // special last character treatment
                    if (!inLineComment && !inSlashStarComment)
                    {
                        outString.Append(c);
                    }
                }
                else
                {
                    if (inSingleQuote)
                    {
                        if (c == '\'')
                        {
                            inSingleQuote = false;
                        }
                        outString.Append(c);
                    }
                    else if (inDoubleQuote)
                    {
                        if (c == '"')
                        {
                            inSingleQuote = false;
                        }
                        outString.Append(c);
                    }
                    else if (inLineComment)
                    {
                        // eating chars until linefeed
                        if (c == '\n')
                        {
                            inLineComment = false;
                        }
                    }
                    else if (inSlashStarComment)
                    {
                        // eating chars until ending */
                        if (c == '*' && inString[i + 1] == '/')
                        {
                            i++;
                            inSlashStarComment = false;
                        }
                    }
                    else
                    {
                        if (c == '-' && inString[i + 1] == '-')
                        {
                            inLineComment = true;
                        }
                        else if (c == '/' && inString[i + 1] == '*')
                        {
                            inSlashStarComment = true;
                        }
                        else
                        {
                            outString.Append(c);

                            if (c == '\'')
                                inSingleQuote = true;
                            else if (c == '"')
                                inDoubleQuote = true;
                        }
                    }
                }
            }

            return outString.ToString();
        }

        //------------------------------------------------------------------------------
        // StripBrackets
        //------------------------------------------------------------------------------
        private string
            StripBrackets(
            string inToken
            )
        {
            string outToken = inToken;

            if (inToken.Length > 2)
            {
                if (inToken[0] == '[' && inToken[inToken.Length - 1] == ']')
                {
                    outToken = inToken.Substring(1, inToken.Length - 2);
                }
            }

            return outToken;
        }

        //------------------------------------------------------------------------------
        // GetTralingTokenString
        //------------------------------------------------------------------------------
        private string
            GetTrailingTokenString(
            string sqlStmt,
            List<string> endingTokenList
            )
        {
            bool toFound = false;
            StringBuilder targetBuilder = new StringBuilder(1024);
            string target = "";

            string[] tokens = sqlStmt.Split();
            for (int i = 0; i < endingTokenList.Count; i++)
                endingTokenList[i] = endingTokenList[i].ToUpper();


            for (int i = tokens.Length - 1; i >= 0; i--)
            {
                if (endingTokenList.Contains(tokens[i].ToUpper()))
                {
                    toFound = true;
                    break;
                }

                if (tokens[i] != "")
                {
                    targetBuilder.Insert(0, tokens[i]);
                }
            }

            if (toFound)
            {
                target = targetBuilder.ToString();
            }

            return target;
        }

        // returns the target name of a GDR statement
        private string GetGDRTrailingTokenString(string sqlStmt)
        {
            List<string> tokenList = new List<string>(2);
            tokenList.Add("To");
            tokenList.Add("From");
            return GetTrailingTokenString(sqlStmt, tokenList);
        }

        //------------------------------------------------------------------------------
        // Validate login name
        //------------------------------------------------------------------------------
        private bool
            IsValidLoginName(
            string name)
        {
            if (name.Length == 0) // Note that some Windows Group name may contain blanks
                return false;
            return true;
        }

        #endregion

        #endregion

        #region Stats Functions

        void UpdateStats(SqlConnection conn, SqlTransaction trans)
        {
            try
            {
                jobInfo.iStats.Update(conn, trans, jobInfo.newStats);
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write("An error occurred updating new stats.",
                                         e,
                                         ErrorLog.Severity.Warning,
                                         true);
            }
        }

        void AddStats(EventRecord er)
        {
            try
            {
                jobInfo.newStats.Add(jobInfo.traceLevel,
                                      er.eventType,
                                      er.databaseName,
                                      er.startTime,
                                      1);
                // We build the DML category for convenience
                if (er.eventType == TraceEventType.INSERT ||
                   er.eventType == TraceEventType.UPDATE ||
                   er.eventType == TraceEventType.DELETE ||
                   er.eventType == TraceEventType.EXECUTE)
                    jobInfo.newStats.Add(er.databaseName, StatsCategory.DML, er.startTime, 1);

                // We want to record EventProcessed at the eventTime
                jobInfo.newStats.Add(StatsCategory.EventProcessed,
                                      er.startTime,
                                      1);
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         "An error occurred adding new stats data.",
                                         e,
                                         ErrorLog.Severity.Warning,
                                         true);
            }
        }


        void AddEventFilteredStats(DateTime time)
        {
            try
            {
                jobInfo.newStats.Add(StatsCategory.EventFiltered,
                                      time,
                                      1);
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         "An error occurred adding new EventFiltered stats data.",
                                         e,
                                         ErrorLog.Severity.Warning,
                                         true);
            }
        }

        void AddEventReceivedStats(int count)
        {
            try
            {
                jobInfo.newStats.Add(StatsCategory.EventReceived,
                                      DateTime.UtcNow,
                                      count);
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         "An error occurred adding new EventReceived stats data.",
                                         e,
                                         ErrorLog.Severity.Warning,
                                         true);
            }
        }

        #endregion


        #region Privileged User Functions

        //------------------------------------------------------------------------
        // IsPrivUser - decide if current row is done by someone in the set
        //              of privileged users      
        //------------------------------------------------------------------------
        internal bool
            IsPrivUser(
            DataRow row
            )
        {
            string user = GetRowString(row, ndxLoginName);

            return jobInfo.privUsers.Contains(user.ToUpper());
        }
        //------------------------------------------------------------------------
        // IsDbPrivUser - decide if current row is done by someone in the set
        //              of privileged users of database level  
        //------------------------------------------------------------------------
        internal bool
            IsDbPrivUser(
            DataRow row
            )
        {
            string user = GetRowString(row, ndxLoginName);
            int dbId = GetRowInt32(row, ndxDatabaseId);
            return !string.IsNullOrEmpty(user)
                && jobInfo.dbconfigs.ContainsKey(dbId)
                && jobInfo.dbconfigs[dbId].PrivUsers != null
                && jobInfo.dbconfigs[dbId].PrivUsers.Contains(user, StringComparer.OrdinalIgnoreCase);
        }
        //------------------------------------------------------------------------
        // IsDbTrustedUser - decide if current row is done by someone in the set
        //              of Trusted users of database level  
        //------------------------------------------------------------------------
        internal bool
            IsDbTrustedUser(
            DataRow row
            )
        {
            string user = GetRowString(row, ndxLoginName);
            int dbId = GetRowInt32(row, ndxDatabaseId);
            return !string.IsNullOrEmpty(user)
                && jobInfo.dbconfigs.ContainsKey(dbId)
                && jobInfo.dbconfigs[dbId].AuditedUsers != null
                && jobInfo.dbconfigs[dbId].AuditedUsers.Contains(user, StringComparer.OrdinalIgnoreCase);
        }
        //------------------------------------------------------------------------
        // IsDataBasePrivEvent - Is this an event that matches the set of events being
        //               collected by priv user auditing?
        //------------------------------------------------------------------------
        internal bool
            IsDatabaseEvents(
            DataRow row
            )
        {
            int eventCls = GetRowInt32(row, ndxEventClass);
            // SQLCM-5850: BAD auditing not displaying data as expected
            // SQLCM-5957: Privileged User: 'SELECT' queries are not getting captured
            if(IsDataChangeEvent(row) && eventCls == (int)TraceEventId.AuditObjectPermission &&
                jobInfo.traceLevel == (int)TraceLevel.Database && (jobInfo.traceCategory == (int)TraceCategory.DML ||
                jobInfo.traceCategory == (int)TraceCategory.DMLwithDetails || jobInfo.traceCategory == (int)TraceCategory.DMLwithSELECT))
            {
            	// SQLCM-5850: BAD auditing not displaying data as expected
                if (CanCaptureBadSql(row))
                {
                    row[ndxKeepSql] = true;
                }
                return true;
            }
            // Ensure that the Privilege User events are not duplicated
            if (IsPrivUser(row) && jobInfo.privEvents.Contains(eventCls))
            {
                return false;
            } else if (eventCls == (int)TraceEventId.AuditObjectPermission || jobInfo.traceFile.ToLower().EndsWith(".xel"))  // DML or SELECt events
            {
                if (jobInfo.traceCategory == (int)TraceCategory.DBPrivilegedUsers)
                {
                    row[ndxPrivilegedUser] = 1;
                    return IsValidDbPrivEvents(row);  // Check Validity of  DML and Select Events for DB Priviliged User
                }
                else
                {
                    //return IsValidDbEvents(row); // Check Validity of DML and Select Events for DB Only
                    try
                    {
                        return IsValidDbEvents(row); // Check Validity of DML and Select Events for DB Only
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.Instance.Write(ErrorLog.Level.Always,
                                      String.Format("SQLCM-6205: exception captured from IsValidDbEvents message: {0} n/ Source: {1} n/ StackTrace: " +
                                      "{2} n/ Target site: {3} n/ Inner exception: {4}", ex.Message, ex.Source, ex.StackTrace, ex.TargetSite, ex.InnerException));
                        return false;
                    }
                }
            }
            else if (eventCls == (int)TraceEventId.AuditSchemaObjectManagement || eventCls == (int)TraceEventId.AuditDatabaseObjectManagement) // DDL events
            {
                if (jobInfo.traceCategory == (int)TraceCategory.DBPrivilegedUsers)
                {
                    row[ndxPrivilegedUser] = 1;
                    return IsValidDbDDLPrivEvents(row);  // Check Validity of  DDL Events for DB Priviliged User
                }

                else
                {
                    return IsValidDbDDLEvents(row); // Check Validity of DDL Events for DB Only
                }

            }
            else
            {
                // Handle duplicate Db and DbPriv events
                // handling of Select and DML related events are tricky because of no collection for Sensitive Columns
                if (eventCls == (int)TraceEventId.AuditDatabaseObjectGDR ||
                eventCls == (int)TraceEventId.AppRolePassChange || eventCls == (int)TraceEventId.AuditAddMember ||
                eventCls == (int)TraceEventId.AuditBackupRestore || eventCls == (int)TraceEventId.AuditChangeAudit ||
                eventCls == (int)TraceEventId.AuditDatabaseManagement ||
                eventCls == (int)TraceEventId.AuditDatabaseObjectAccess ||
                eventCls == (int)TraceEventId.AuditDatabaseObjectGDR ||
                eventCls == (int)TraceEventId.AuditDatabaseObjectManagement ||
                eventCls == (int)TraceEventId.AuditDatabaseObjectTakeOwnership ||
                eventCls == (int)TraceEventId.AuditDatabaseOperation ||
                eventCls == (int)TraceEventId.AuditDatabasePrincipalManagement || 
                eventCls == (int)TraceEventId.AuditDbcc || eventCls == (int)TraceEventId.AuditObjectGDR ||
                eventCls == (int)TraceEventId.AuditSchemaObjectManagement ||
                eventCls == (int)TraceEventId.AuditSchemaObjectTakeOwnership ||
                eventCls == (int)TraceEventId.AuditStatementGDR || eventCls == (int)TraceEventId.ServiceControl)
                {
                    // Handle for monitored database
                    if (jobInfo.dbconfigs.ContainsKey(GetRowInt32(row, ndxDatabaseId)))
                    {
                        // If user is defined in database privileged user, the event will be handled there
                        if (IsDbPrivUser(row) && jobInfo.traceCategory != (int)TraceCategory.DBPrivilegedUsers)
                        {
                            return false;
                        }
                        row[ndxPrivilegedUser] = 1;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// Need to write a logic considering all the levels for BAD data
        /// </summary>
        /// <param name="row">input data</param>
        /// <returns>returns true if BAD is enabled</returns>
        /// <remarks>
        /// SQLCM-5850: BAD auditing not displaying data as expected
        /// </remarks>
        private bool CanCaptureBadSql(DataRow row)
        {
            // Checks at job level, collected data level and PU settings
            bool isSqlCaptureEnabled = jobInfo.keepingSql || GetRowInt32(row, ndxKeepSql) == ON ||
                                        (IsPrivUser(row) && jobInfo.userConfigs != null &&
                                            (jobInfo.userConfigs.AuditDML || jobInfo.userConfigs.AuditUserDML) &&
                                                jobInfo.userConfigs.AuditCaptureDetails);
            if (!isSqlCaptureEnabled)
            {
                // Check at database level
                var dbId = GetRowInt32(row, ndxDatabaseId);
                if (jobInfo.dbconfigs != null && jobInfo.dbconfigs.ContainsKey(dbId))
                {
                    // Handle database DML Capture
                    var dbConfig = jobInfo.dbconfigs[dbId];
                    isSqlCaptureEnabled = dbConfig.AuditCaptureDetails && dbConfig.AuditDML;

                    // Handle database PU - DML Capture
                    if (!isSqlCaptureEnabled && IsDbPrivUser(row))
                    {
                        isSqlCaptureEnabled = dbConfig.AuditUserCaptureSql && dbConfig.AuditUserDML;
                    }
                }
            }
            return isSqlCaptureEnabled;
        }

        internal bool
            IsServerPrivEvent(
            DataRow row
            )
        {
            row[ndxPrivilegedUser] = 1;
            if (!IsPrivUser(row)) return false;  // Not a priv user

            int eventCls = GetRowInt32(row, ndxEventClass);

            if (eventCls == (int)TraceEventId.AuditObjectPermission || jobInfo.traceFile.ToLower().EndsWith(".xel"))  // DML or SELECt events
            {
                return IsValidServerPrivEvents(row);  // Check Validity of DML and Select Events for Server Priviliged User
            }

            else if (eventCls == (int)TraceEventId.AuditSchemaObjectManagement || eventCls == (int)TraceEventId.AuditDatabaseObjectManagement) // DDL events
            {
                return IsValidServerDDLPrivEvents(row);  // Check Validity of DDL Events for Server Priviliged User
            }
            else
            {
                return jobInfo.privEvents.Contains(eventCls);
            }
        }
        /// <summary>
        /// Check File Preferences for Server Level Priv Users
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        internal bool IsValidServerPrivEvents(DataRow row)
        {

            int dbId = GetRowInt32(row, ndxDatabaseId);
            if (jobInfo.dbconfigs == null || !jobInfo.dbconfigs.ContainsKey(dbId) || jobInfo.userConfigs.AuditCaptureDetails)
            {
                return true;
            }
            AccessCheckFilter accessCheckType = (int)row[ndxSuccess] != 1 ? AccessCheckFilter.FailureOnly : AccessCheckFilter.SuccessOnly;
            AccessCheckFilter serverAccessCheck, dbAccessCheck, dbUserAccessCheck;
            serverAccessCheck = jobInfo.userConfigs.AuditAccessCheck;
            dbAccessCheck = jobInfo.dbconfigs[dbId].AuditAccessCheck;
            if (serverAccessCheck != AccessCheckFilter.NoFilter && accessCheckType != serverAccessCheck && !jobInfo.traceFile.ToLower().EndsWith(".xel"))
                return false;
            AuditCategory eventCategory = GetAuditCategory(row);
            if (IsDbPrivUser(row) && jobInfo.dbconfigs[dbId].UserCategories != null && jobInfo.dbconfigs[dbId].UserCategories.Contains(eventCategory)
                && (eventCategory == AuditCategory.DML || eventCategory == AuditCategory.SELECT))
            {
                dbUserAccessCheck = (AccessCheckFilter)jobInfo.dbconfigs[dbId].UserAccessCheck;
                if (dbUserAccessCheck == AccessCheckFilter.NoFilter || accessCheckType == dbUserAccessCheck || jobInfo.traceFile.ToLower().EndsWith(".xel"))
                {
                    if (jobInfo.dbconfigs[dbId].UserCaptureSql)
                    {
                        return false;
                    }
                }
            }
            if ((jobInfo.dbconfigs[dbId].AuditDML && eventCategory == AuditCategory.DML)
                || (jobInfo.dbconfigs[dbId].AuditSELECT && eventCategory == AuditCategory.SELECT))
            {
                if (IsDbTrustedUser(row))
                {
                    return true;
                }
                else
                {
                    if (dbAccessCheck == AccessCheckFilter.NoFilter || accessCheckType == dbAccessCheck || jobInfo.traceFile.ToLower().EndsWith(".xel"))
                    {
                        if (jobInfo.dbconfigs[dbId].AuditCaptureDetails)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// Check File Preferences for Server Level Priv Users DDL events
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        internal bool IsValidServerDDLPrivEvents(DataRow row)
        {

            int dbId = GetRowInt32(row, ndxDatabaseId);
            if (jobInfo.dbconfigs == null || !jobInfo.dbconfigs.ContainsKey(dbId) || jobInfo.userConfigs.AuditCaptureDDL)
            {
                return true;
            }
            AccessCheckFilter accessCheckType = (int)row[ndxSuccess] != 1 ? AccessCheckFilter.FailureOnly : AccessCheckFilter.SuccessOnly;
            AccessCheckFilter serverAccessCheck, dbAccessCheck, dbUserAccessCheck;
            serverAccessCheck = jobInfo.userConfigs.AuditAccessCheck;
            dbAccessCheck = jobInfo.dbconfigs[dbId].AuditAccessCheck;
            if (serverAccessCheck != AccessCheckFilter.NoFilter && accessCheckType != serverAccessCheck)
                return false;
            string txtData = GetRowString(row, ndxTextData).ToUpper();
            if (IsDbPrivUser(row))
            {

                if (IsDDLSqlStatement(txtData))
                {
                    dbUserAccessCheck = (AccessCheckFilter)jobInfo.dbconfigs[dbId].UserAccessCheck;
                    if (dbUserAccessCheck == AccessCheckFilter.NoFilter || accessCheckType == dbUserAccessCheck)
                    {
                        if (jobInfo.dbconfigs[dbId].AuditUserCaptureDDL)
                        {
                            return false;
                        }
                    }
                }
            }
            if (IsDDLSqlStatement(txtData))
            {
                if (IsDbTrustedUser(row))
                {
                    return true;
                }
                else
                {
                    if (dbAccessCheck == AccessCheckFilter.NoFilter || accessCheckType == dbAccessCheck)
                    {
                        if (jobInfo.dbconfigs[dbId].AuditCaptureDDL)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        //end

        /// <summary>
        /// Check File Preferences for Server Level Priv Users
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        internal bool IsValidDbPrivEvents(DataRow row)
        {
            int dbId = GetRowInt32(row, ndxDatabaseId);
            if (jobInfo.dbconfigs == null || !jobInfo.dbconfigs.ContainsKey(dbId))
            {
                return false;
            }

            // SQLCM-5850: BAD auditing not displaying data as expected
            int eventCls = GetRowInt32(row, ndxEventClass);

            AccessCheckFilter accessCheckType = (int)row[ndxSuccess] != 1 ? AccessCheckFilter.FailureOnly : AccessCheckFilter.SuccessOnly;
            AccessCheckFilter serverAccessCheck, dbAccessCheck, dbUserAccessCheck;
            dbAccessCheck = jobInfo.dbconfigs[dbId].AuditAccessCheck;
            dbUserAccessCheck = (AccessCheckFilter)jobInfo.dbconfigs[dbId].UserAccessCheck;
            if (dbUserAccessCheck != AccessCheckFilter.NoFilter && accessCheckType != dbUserAccessCheck && !jobInfo.traceFile.ToLower().EndsWith(".xel"))
                return false;
            AuditCategory eventCategory = GetAuditCategory(row);
            if (IsPrivUser(row))
            {
                serverAccessCheck = jobInfo.userConfigs.AuditAccessCheck;
                if ((jobInfo.privDML && eventCategory == AuditCategory.DML)
                    || (jobInfo.privSELECT && eventCategory == AuditCategory.SELECT))
                {
                    if (serverAccessCheck == AccessCheckFilter.NoFilter || accessCheckType == serverAccessCheck || jobInfo.traceFile.ToLower().EndsWith(".xel"))
                    {
                        if (jobInfo.userConfigs.AuditCaptureDetails || !jobInfo.dbconfigs[dbId].UserCaptureSql)
                        {
                            return false;
                        }
                    }
                }
            }
            if (jobInfo.dbconfigs[dbId].UserCaptureSql)  // Check SQL capture at DB Priv level;
                return true;
            if ((jobInfo.dbconfigs[dbId].AuditDML && eventCategory == AuditCategory.DML)
                || (jobInfo.dbconfigs[dbId].AuditSELECT && eventCategory == AuditCategory.SELECT))
            {
                if (IsDbTrustedUser(row))
                {
                    return true;
                }
                if (dbAccessCheck == AccessCheckFilter.NoFilter || accessCheckType == dbAccessCheck || jobInfo.traceFile.ToLower().EndsWith(".xel"))
                {
                    if (jobInfo.dbconfigs[dbId].AuditCaptureDetails)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        // Check Validity of  DDL Events for DB Priviliged User
        internal bool IsValidDbDDLPrivEvents(DataRow row)
        {
            int dbId = GetRowInt32(row, ndxDatabaseId);
            if (!jobInfo.dbconfigs.ContainsKey(dbId))
            {
                return false;
            }
            AccessCheckFilter accessCheckType = (int)row[ndxSuccess] != 1 ? AccessCheckFilter.FailureOnly : AccessCheckFilter.SuccessOnly;
            AccessCheckFilter serverAccessCheck, dbAccessCheck, dbUserAccessCheck;
            dbAccessCheck = jobInfo.dbconfigs[dbId].AuditAccessCheck;
            dbUserAccessCheck = (AccessCheckFilter)jobInfo.dbconfigs[dbId].UserAccessCheck;
            if (dbUserAccessCheck != AccessCheckFilter.NoFilter && accessCheckType != dbUserAccessCheck)
                return false;

            string txtData = GetRowString(row, ndxTextData).ToUpper();

            if (IsPrivUser(row))
            {
                serverAccessCheck = jobInfo.userConfigs.AuditAccessCheck;

                if (jobInfo.userConfigs.AuditDDL && IsDDLSqlStatement(txtData))
                {
                    if (serverAccessCheck == AccessCheckFilter.NoFilter || accessCheckType == serverAccessCheck)
                    {
                        if (jobInfo.userConfigs.AuditCaptureDDL || !jobInfo.dbconfigs[dbId].AuditUserCaptureDDL)
                        {
                            return false;
                        }
                    }
                }
            }
            if (jobInfo.dbconfigs[dbId].AuditUserCaptureDDL)  // Check SQL capture at DB Priv level;
                return true;
            if (jobInfo.dbconfigs[dbId].AuditDDL && IsDDLSqlStatement(txtData))
            {
                if (IsDbTrustedUser(row))
                {
                    return true;
                }
                if (dbAccessCheck == AccessCheckFilter.NoFilter || accessCheckType == dbAccessCheck)
                {
                    if (jobInfo.dbconfigs[dbId].AuditCaptureDDL)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        // SQLCM-5471 v5.6 Add Activity to Senstitive columns
        internal bool isSenstiveColumnData(DataRow row)
        {
            int dbId = GetRowInt32(row, ndxDatabaseId);
            if (jobInfo.dbconfigs == null)
                return false;
            string txtData = GetRowString(row, ndxTextData).ToUpper();
            string queryTableName = GetRowString(row, ndxObjectName).ToUpper();

            bool foundInSensitiveTable = false;
            bool foundInBAD = false;
            if (jobInfo.dbconfigs.ContainsKey(dbId))
            {
                foreach (TableConfiguration sensitiveTable in jobInfo.dbconfigs[dbId].SensitiveColumns)
                {
                    if (sensitiveTable.Name.ToUpper() == queryTableName.ToUpper())
                    {
                        foundInSensitiveTable = true;
                        break;
                    }
                    //else if (txtData.ToUpper().Contains(sensitiveTable.Name.ToUpper()))
                    else 
                    {
                        string pattern = @"\b" + Regex.Escape(sensitiveTable.Name) + @"\b";
                        if (Regex.Match(txtData, pattern, RegexOptions.IgnoreCase).Success)
                        {
                            foundInSensitiveTable = true;
                            break;
                        }
                    }
                }
                foreach (TableConfiguration dataChangeTable in jobInfo.dbconfigs[dbId].DataChangeTables)
                {
                    if (dataChangeTable.Name.ToUpper() == queryTableName.ToUpper())
                    {
                        foundInBAD = true;
                        break;
                    }
                }
                if (foundInBAD && foundInSensitiveTable)
                    return true;
                else if (foundInBAD && !foundInSensitiveTable)
                    return true;
                else if (!foundInBAD && foundInSensitiveTable)
                {
                    if (jobInfo.dbconfigs[dbId].SensitiveColumns != null &&
                        jobInfo.dbconfigs[dbId].SensitiveColumns.Length > 0 && foundInSensitiveTable &&
                        (jobInfo.dbconfigs[dbId].AuditSensitiveColumnActivity == SensitiveColumnActivity.SelectOnly ||
                        jobInfo.dbconfigs[dbId].AuditSensitiveColumnActivity == SensitiveColumnActivity.SelectAndDML ||
                        jobInfo.dbconfigs[dbId].AuditSensitiveColumnActivity == SensitiveColumnActivity.AllActivity ||
                        jobInfo.dbconfigs[dbId].AuditSensitiveColumnActivityDataset == SensitiveColumnActivity.SelectOnly ||
                        jobInfo.dbconfigs[dbId].AuditSensitiveColumnActivityDataset == SensitiveColumnActivity.SelectAndDML ||
                        jobInfo.dbconfigs[dbId].AuditSensitiveColumnActivityDataset == SensitiveColumnActivity.AllActivity))
                    {
                        if (isSensitiveDataQuery(txtData))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        // SQLCM-5471 v5.6 Add Activity to Senstitive columns

        internal bool isSensitiveDataQuery(string txtData)
        {
            if (txtData.StartsWith("UPDATE") || 
                txtData.StartsWith("INSERT") || 
                txtData.StartsWith("DELETE") ||
                txtData.StartsWith("ALTER") ||
                txtData.StartsWith("CREATE") || 
                txtData.StartsWith("DROP") || 
                txtData.StartsWith("DBCC") ||
                (txtData.StartsWith("EXEC") && !txtData.StartsWith("EXECUTE AS")) ||
                txtData.StartsWith("DISABLE TRIGGER") ||
                txtData.StartsWith("ENABLE TRIGGER") ||
                txtData.StartsWith("TRUNCATE TABLE") ||
                txtData.StartsWith("UPDATE STATISTICS"))
                return true;
            else
                return false;
        }
        // SQLCM-5471 v5.6 Add Activity to Senstitive columns
        /// <summary>
        /// Check File Preferences for DB Level 
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        internal bool IsValidDbEvents(DataRow row)
        {
            int dbId = GetRowInt32(row, ndxDatabaseId);
            if (jobInfo.dbconfigs == null && !jobInfo.dbconfigs.ContainsKey(dbId))
            {
                return false;
            }
            bool isDataChangeEvent = false;
            if (IsDbTrustedUser(row))
            {
                return false;
            }
            else if (IsDataChangeEvent(row))
            {
                isDataChangeEvent = true;
            }
            AccessCheckFilter accessCheckType = (int)row[ndxSuccess] != 1 ? AccessCheckFilter.FailureOnly : AccessCheckFilter.SuccessOnly;
            AccessCheckFilter serverAccessCheck, dbAccessCheck, dbUserAccessCheck;
            dbAccessCheck = jobInfo.dbconfigs[dbId].AuditAccessCheck;
            if (dbAccessCheck != AccessCheckFilter.NoFilter && accessCheckType != dbAccessCheck && !jobInfo.traceFile.ToLower().EndsWith(".xel"))
                return false;
            AuditCategory eventCategory = GetAuditCategory(row);
            if (IsPrivUser(row))
            {
                serverAccessCheck = jobInfo.userConfigs.AuditAccessCheck;
                if ((jobInfo.privDML && eventCategory == AuditCategory.DML)
                    || (jobInfo.privSELECT && eventCategory == AuditCategory.SELECT))
                {
                    if (serverAccessCheck == AccessCheckFilter.NoFilter || accessCheckType == serverAccessCheck || jobInfo.traceFile.ToLower().EndsWith(".xel"))
                    {
                        row[ndxPrivilegedUser] = 1;
                        if (isDataChangeEvent)
                            return true;
                        if (jobInfo.userConfigs.AuditCaptureDetails || !jobInfo.dbconfigs[dbId].AuditCaptureDetails)
                        {
                            return false;
                        }
                    }
                }
            }
            if (IsDbPrivUser(row) && jobInfo.dbconfigs[dbId].UserCategories != null && jobInfo.dbconfigs[dbId].UserCategories.Contains(eventCategory)
                && (eventCategory == AuditCategory.DML || eventCategory == AuditCategory.SELECT))
            {
                dbUserAccessCheck = (AccessCheckFilter)jobInfo.dbconfigs[dbId].UserAccessCheck;
                if (dbUserAccessCheck == AccessCheckFilter.NoFilter || accessCheckType == dbUserAccessCheck || jobInfo.traceFile.ToLower().EndsWith(".xel"))
                {
                    row[ndxPrivilegedUser] = 1;
                    if (isDataChangeEvent)
                        return true;
                    if (jobInfo.dbconfigs[dbId].UserCaptureSql || !jobInfo.dbconfigs[dbId].AuditCaptureDetails)
                    {
                        return false;
                    }
                }

            }

            return true;
        }

        // Check Validity of DDL Events for DB Only
        internal bool IsValidDbDDLEvents(DataRow row)
        {
            int dbId = GetRowInt32(row, ndxDatabaseId);
            if (jobInfo.dbconfigs == null || !jobInfo.dbconfigs.ContainsKey(dbId))
            {
                return false;
            }
            if (IsDbTrustedUser(row))
            {
                return false;
            }
            AccessCheckFilter accessCheckType = (int)row[ndxSuccess] != 1 ? AccessCheckFilter.FailureOnly : AccessCheckFilter.SuccessOnly;
            AccessCheckFilter serverAccessCheck, dbAccessCheck, dbUserAccessCheck;
            dbAccessCheck = jobInfo.dbconfigs[dbId].AuditAccessCheck;
            if (dbAccessCheck != AccessCheckFilter.NoFilter && accessCheckType != dbAccessCheck)
                return false;

            if (IsPrivUser(row))
            {
                serverAccessCheck = jobInfo.userConfigs.AuditAccessCheck;
                string txtData = GetRowString(row, ndxTextData).ToUpper();
                if (jobInfo.userConfigs.AuditDDL && IsDDLSqlStatement(txtData))
                {
                    if (serverAccessCheck == AccessCheckFilter.NoFilter || accessCheckType == serverAccessCheck)
                    {
                        row[ndxPrivilegedUser] = 1;
                        if ((jobInfo.userConfigs.AuditCaptureDDL) || !jobInfo.dbconfigs[dbId].AuditCaptureDDL)
                        {
                            return false;
                        }
                    }
                }
            }

            if (IsDbPrivUser(row))
            {
                string txtData = GetRowString(row, ndxTextData).ToUpper();
                if (jobInfo.dbconfigs[dbId].AuditDDL && IsDDLSqlStatement(txtData))
                {
                    dbUserAccessCheck = (AccessCheckFilter)jobInfo.dbconfigs[dbId].UserAccessCheck;
                    if (dbUserAccessCheck == AccessCheckFilter.NoFilter || accessCheckType == dbUserAccessCheck)
                    {
                        row[ndxPrivilegedUser] = 1;
                        if (!jobInfo.dbconfigs[dbId].AuditCaptureDDL || jobInfo.dbconfigs[dbId].AuditUserCaptureDDL)
                        {
                            return false;
                        }
                    }
                }

            }

            return true;
        }


        /// <summary>
        /// Check and Return audit Category
        /// </summary>
        /// <param name="traceFile"></param>
        /// <returns></returns>
        internal AuditCategory GetAuditCategory(DataRow row)
        {
            int prms = GetRowPermissions(row, ndxPermissions);
            if (prms == (int)TracePermissions.SelectAll
                || prms == (int)TracePermissions.ReferencesAll)// SELECT events
            {
                return AuditCategory.SELECT;
            }
            else  // DML events
            {
                return AuditCategory.DML;
            }
        }
        #endregion

        #region SQL - Events Table

        private string GetLoadSQL(string traceFile)
        {
            return String.Format("SELECT {0}  FROM ::fn_trace_gettable('{1}', 1)", GetColumnsSQL(jobInfo.isSqlServer2005, jobInfo.SupportsBeforeAfter()), traceFile);
        }


        #endregion

        #region SQL - Generic

        //------------------------------------------------------------------
        // GetDropSQL - SQL to drop temporary table after we are through
        //------------------------------------------------------------------
        private string
            GetDropSQL(
            string tableName
            )
        {
            string sqlTemp = "DROP TABLE [{0}]";

            return String.Format(sqlTemp, tableName);
        }

        #endregion

        #region Data Change Trace (Before/after data)

        // Return true if records were inserted
        private bool ProcessDataChangeTrace()
        {
            LoadDataChangeTable();

            // Only the first table in the dataset is being used
            if (tmpTable.Rows.Count == 0)
                return false;

            DataTable table = tmpTable;
            Repository writeRepository = new Repository();
            Repository readRepository = new Repository();

            long start = Environment.TickCount;
            int currentBatch = 0;
            int totalRows = 0;
            SqlTransaction writeTrans = null;
            bool getFirstEventTime = true;
            try
            {
                using (DataTable dataChangeTable = new DataTable())
                {
                    using (DataTable columnChangeTable = new DataTable())
                    {
                        writeRepository.OpenConnection(jobInfo.eventDatabase);
                        readRepository.OpenConnection(jobInfo.eventDatabase);
                        InitDataChangesTableColumns(dataChangeTable, readRepository.connection);
                        InitColumnChangesTableColumns(columnChangeTable, readRepository.connection);
                        writeTrans =
                        writeRepository.connection.BeginTransaction(IsolationLevel.ReadUncommitted);

                        // The first event is usually an invalid event with null as the StartTime
                        int i = table.Rows.Count - 1;
                        while (jobInfo.lastEventTime == DateTime.MinValue && i >= 0)
                        {
                            jobInfo.lastEventTime = GetRowDateTime(table.Rows[i], "StartTime");
                            i--;
                        }
                        foreach (DataRow row in table.Rows)
                        {
                            if (row == null) continue;

                            if (getFirstEventTime && !row.IsNull("StartTime"))
                            {
                                jobInfo.firstEventTime = GetRowDateTime(row, "StartTime");
                                getFirstEventTime = false;
                            }

                            if (SQLHelpers.GetRowInt32(row, "EventClass") != (int)TraceEventId.AuditObjectPermission)
                                continue;

                            rowsWritten++;
                            // We can skip these rows - they have already been written
                            if (rowsWritten <= jobInfo.lastFinalEventProcessed)
                            {
                                continue;
                            }

                            currentBatch++;
                            if (currentBatch % insertBatchSize == 0)
                            {
                                lock (AcquireInstanceLock(jobInfo.instance))
                                {
                                    writeTrans = InsertDataChangeRecords(
                                                            writeRepository.connection,
                                                            writeTrans,
                                                            dataChangeTable,
                                                            columnChangeTable);
                                }
                                totalRows += currentBatch;
                                currentBatch = 0;

                                // check aborting flag	                  
                                if (jobInfo.GetAborting())
                                    return false;

                                writeTrans =
                                   writeRepository.connection.BeginTransaction(IsolationLevel.ReadUncommitted);
                            }

                            ProcessDataChangeRow(dataChangeTable, columnChangeTable, row);

                        }

                        lock (AcquireInstanceLock(jobInfo.instance))
                        {
                            writeTrans = InsertDataChangeRecords(
                                                    writeRepository.connection,
                                                    writeTrans,
                                                    dataChangeTable,
                                                    columnChangeTable);
                        }
                        totalRows += currentBatch;
                    } // End of using
                } // End of using


                if (jobInfo.eventsInserted > 0)
                {
                    AlertingJobPool.DoBADAlertProcessing = true;
                    AlertingJobPool.SetBADAlertDetails(jobInfo.instance, jobInfo.eventDatabase, jobInfo.eventsInserted);
                }

                if (totalRows > 0)
                {
                    if (UpdateDataChangeIds(jobInfo.firstEventTime, jobInfo.lastEventTime))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         String.Format(
                                            "An error occurred processing {0} data change trace file {1}.  Error: {2}.",
                                            jobInfo.instance,
                                            jobInfo.traceFile,
                                            e.Message));
                throw;
            }
            finally
            {
                if (writeTrans != null)
                {
                    rowsWritten -= currentBatch;
                    ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                             String.Format("Transaction rollback: at rowsWritten: {0}, totalRows: {1}, currentBatch: {2}",
                                                             rowsWritten,
                                                             totalRows,
                                                             currentBatch));
                    try
                    {
                        writeTrans.Rollback();
                    }
                    catch(Exception ex)
                    {
                        ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                             String.Format("Rollback Exception Type: {0}, message : {1}", ex.GetType(), ex.Message));
                    }
                }
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                         String.Format("Trace processing: {0} rows inserted and {1} rows skipped in {2}ms",
                                                        totalRows,
                                                        rowsWritten - totalRows,
                                                        Environment.TickCount - start));

                writeRepository.CloseConnection();
                readRepository.CloseConnection();
            }
        }


        //5.4 XE
        // Return true if records were inserted
        private bool ProcessDataChangeTraceXE()
        {
            XEventData eventList = new XEventData(jobInfo.traceFile);
            // Only the first table in the dataset is being used
            if (eventList.EventData.Count == 0)
            {
                return false;
            }

            DataTable table = new DataTable();
            AddDataChangeTableColumnXE(table);
            Repository writeRepository = new Repository();
            Repository readRepository = new Repository();

            long start = Environment.TickCount;
            int currentBatch = 0;
            int totalRows = 0;
            SqlTransaction writeTrans = null;
            bool getFirstEventTime = true;



            try
            {
                using (DataTable dataChangeTable = new DataTable())
                {
                    using (DataTable columnChangeTable = new DataTable())
                    {
                        writeRepository.OpenConnection(jobInfo.eventDatabase);
                        readRepository.OpenConnection(jobInfo.eventDatabase);
                        InitDataChangesTableColumns(dataChangeTable, readRepository.connection);
                        InitColumnChangesTableColumns(columnChangeTable, readRepository.connection);
                        writeTrans =
                        writeRepository.connection.BeginTransaction(IsolationLevel.ReadUncommitted);

                        // The first event is usually an invalid event with null as the StartTime
                        int i = eventList.EventData.Count - 1;
                        while (jobInfo.lastEventTime == DateTime.MinValue && i >= 0)
                        {
                            jobInfo.lastEventTime = timeZoneInfo.Convert(jobInfo.timeZoneInfo, eventList.EventData[i].EventTime).AddMilliseconds(100);
                            i--;
                        }
                        foreach (XEventSingleEvent eventRow in eventList.EventData)
                        {
                            if (eventRow == null) continue;

                            if (!eventRow.EventData.ContainsKey("sql_text")) continue;
                            //this needs to be set first.

                            DataRow row = table.NewRow();
                            GetDataChangeRowDataXE(eventRow, row);
                            row["StartTime"] = timeZoneInfo.Convert(jobInfo.timeZoneInfo, (DateTime)row["StartTime"]);

                            if (getFirstEventTime && !row.IsNull("StartTime"))
                            {
                                jobInfo.firstEventTime = GetRowDateTime(row, "StartTime");
                                getFirstEventTime = false;
                            }

                            //if (SQLHelpers.GetRowInt32(row, "EventClass") != (int)TraceEventId.AuditObjectPermission)
                            //    continue;

                            rowsWritten++;
                            // We can skip these rows - they have already been written
                            if (rowsWritten <= jobInfo.lastFinalEventProcessed)
                            {
                                continue;
                            }

                            currentBatch++;
                            if (currentBatch % insertBatchSize == 0)
                            {
                                lock (AcquireInstanceLock(jobInfo.instance))
                                {
                                    writeTrans = InsertDataChangeRecords(
                                                            writeRepository.connection,
                                                            writeTrans,
                                                            dataChangeTable,
                                                            columnChangeTable);
                                }
                                totalRows += currentBatch;
                                currentBatch = 0;

                                // check aborting flag	                  
                                if (jobInfo.GetAborting())
                                    return false;

                                writeTrans =
                                   writeRepository.connection.BeginTransaction(IsolationLevel.ReadUncommitted);
                            }

                            ProcessDataChangeRow(dataChangeTable, columnChangeTable, row);

                        }
                        lock (AcquireInstanceLock(jobInfo.instance))
                        {
                            writeTrans = InsertDataChangeRecords(
                                                    writeRepository.connection,
                                                    writeTrans,
                                                    dataChangeTable,
                                                    columnChangeTable);
                        }
                        totalRows += currentBatch;
                    } // End of using
                } // End of using


                if (jobInfo.eventsInserted > 0)
                {
                    AlertingJobPool.DoBADAlertProcessing = true;
                    AlertingJobPool.SetBADAlertDetails(jobInfo.instance, jobInfo.eventDatabase, jobInfo.eventsInserted);
                }
                if (totalRows > 0)
                {
                    if (UpdateDataChangeIds(jobInfo.firstEventTime, jobInfo.lastEventTime))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         String.Format(
                                            "An error occurred processing {0} data change trace file {1}.  Error: {2}.",
                                            jobInfo.instance,
                                            jobInfo.traceFile,
                                            e.Message));
                throw;
            }
            finally
            {
                if (writeTrans != null)
                {
                    rowsWritten -= currentBatch;
                    ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                             String.Format("Transaction rollback: at rowsWritten: {0}, totalRows: {1}, currentBatch: {2}",
                                                             rowsWritten,
                                                             totalRows,
                                                             currentBatch));
                    writeTrans.Rollback();
                }
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                         String.Format("Trace processing: {0} rows inserted and {1} rows skipped in {2}ms",
                                                        totalRows,
                                                        rowsWritten - totalRows,
                                                        Environment.TickCount - start));

                writeRepository.CloseConnection();
                readRepository.CloseConnection();
            }
        }

        //-------------------------------------------------------------------------
        // LoadDataChangeTable - Load data from tempoary table into in memory DataSet
        //               for processing
        //-------------------------------------------------------------------------
        private void
            LoadDataChangeTable()
        {
            Repository rep = new Repository();

            try
            {
                rep.OpenConnection();

                string sqlText = String.Format("SELECT EventClass, StartTime, SPID, EventSequence, DatabaseID, DatabaseName, TextData FROM ::fn_trace_gettable('{0}', 1)", jobInfo.traceFile);

                using (SqlCommand selectCmd = new SqlCommand(sqlText, rep.connection))
                {
                    selectCmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    selectCmd.CommandType = CommandType.Text;

                    // SqlDataAdapter
                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter())
                    {
                        dataAdapter.TableMappings.Add("Table", "Events");

                        dataAdapter.SelectCommand = selectCmd;

                        // Load DataSet  
                        tmpTable = new DataTable();

                        try
                        {
                            // there is some anecdotal evidence on the web to indicate that 
                            // fn_trace_gettable is not threadsafe so we will just put a lock
                            // around it
                            //lock (traceFileReadLock)
                            {
                                dataAdapter.Fill(tmpTable);
                            }
                        }
                        catch (SqlException sqlEx)
                        {
                            // workaround - for some rollover files, SQL Server 2000 creates files
                            //              that fn_trace_gettable throws error 568 on - what we do
                            //              is just gnore this error - it seems to be a problem
                            //              at the end of the file and not with the real data


                            if (sqlEx.Number == 567)
                            {
                                // cant read file or it dosnt exist - usually caused by SQl Server not having permission to read files in that directory
                                string msg =
                                   String.Format(CoreConstants.Exception_CantReadTraceFile,
                                                  jobInfo.traceFile);

                                ErrorLog.Instance.Write(msg);
                                throw new Exception(msg);
                            }
                            else if (sqlEx.Number != 568)
                            {
                                ErrorLog.Instance.Write(
                                   String.Format(
                                      "LoadDataChangeTable: Number: Error: {0} Level : {1} State : {2}",
                                      sqlEx.Number,
                                      sqlEx.Class,
                                      sqlEx.State),
                                   sqlEx);
                                throw;
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write("LoadDataChangeTable",
                                         String.Format(
                                            CoreConstants.Exception_CantLoadTraceFile,
                                            jobInfo.instance),
                                         ex);
                throw;
            }
            finally
            {
                rep.CloseConnection();
            }
        }


        protected bool
           ProcessDataChangeRow(
              DataTable dataChangeTable,
              DataTable columnChangeTable,
              DataRow row
           )
        {
            DataRow dr = dataChangeTable.NewRow();
            DataChangeRecord dcr = new DataChangeRecord();
            dcr.startTime = SQLHelpers.GetRowDateTime(row, "StartTime");
            dr["startTime"] = dcr.startTime = TimeZoneInfo.ToUniversalTime(jobInfo.timeZoneInfo, dcr.startTime);
            if (row["eventSequence"].GetType() == Type.GetType("System.Int64"))
            {
                dcr.eventSequence = SQLHelpers.GetRowLong(row, "eventSequence");
            }
            else
            {
                dcr.eventSequence = (uint)SQLHelpers.GetRowInt32(row, "eventSequence");  // before SQL2005 SP2 the column is singed 32-bit integer
            }
            dr["eventSequence"] = dcr.eventSequence;
            dr["spid"] = dcr.spid = SQLHelpers.GetRowInt32(row, "SPID");
            dcr.databaseId = SQLHelpers.GetRowInt32(row, "DatabaseID");
            //string dbName = SQLHelpers.GetRowString( row, "DatabaseName" );

            string colList = ParseDataChangeString(SQLHelpers.GetRowString(row, "TextData"), dcr);
            if (colList == null)
                return false;
            dr["databaseId"] = dcr.databaseId;
            dr["actionType"] = dcr.actionType;
            dr["schemaName"] = dcr.schemaName;
            dr["tableName"] = dcr.tableName;
            dr["tableId"] = dcr.tableId;
            jobInfo.UpdateTableIdCache(dcr.schemaName, dcr.tableName, dcr.tableId);
            dr["recordNumber"] = dcr.recordNumber;
            dr["userName"] = dcr.user;
            dr["primaryKey"] = dcr.primaryKey;

            if (colList.Length > 0)
                ParseColumnChanges(dcr, colList, columnChangeTable);

            dr["changedColumns"] = dcr.changedColumns;
            dr["hashcode"] = dcr.GetHashCode();  //NativeMethods.GetHashCode(dcr.ToString()); Changed By Hemant
            dr["totalChanges"] = dcr.totalChanges;
            if (row.Table.Columns.Contains("guid"))
            {
                dcr.guid = SQLHelpers.GetRowString(row, "guid");
            }
            else
            {
                dcr.guid = "";
            }
            dr["guid"] = dcr.guid;

            dataChangeTable.Rows.Add(dr);

            return true;
        }

        private string ParseDataChangeString(string changes, DataChangeRecord dcr)
        {
            int start;
            int end;
            bool summary = false;

            if (changes.StartsWith("SELECT 'Metadata - Table:"))
                summary = true;
            else if (changes.StartsWith("-- Encrypted text"))
            {
                dcr.actionType = (int)TraceEventType.EncryptedDML;
                dcr.schemaName = "";
                dcr.tableName = "<n/a>";
                dcr.recordNumber = -1;
                dcr.databaseId = -1;
                dcr.user = "<n/a>";
                dcr.primaryKey = "<n/a>";
                return "";
            }

            // Table name
            start = changes.IndexOf("Table: ") + 7;
            end = changes.IndexOf(", Action: ");
            if (end <= start)  // This happens when the operation is encrypted.
            {
                dcr.actionType = -1;  // Invalid type
                dcr.schemaName = "";
                dcr.tableName = "<n/a>";
                dcr.recordNumber = -1;
                dcr.databaseId = -1;
                dcr.user = "<n/a>";
                dcr.primaryKey = "<n/a>";
                return "";
            }

            string table = changes.Substring(start, end - start);

            string[] parts = table.Split(".".ToCharArray());
            if (parts.Length == 1)
            {
                dcr.schemaName = "";
                dcr.tableName = table;
            }
            else
            {
                dcr.schemaName = parts[0];
                dcr.tableName = parts[1];
            }
            dcr.tableId = NativeMethods.GetHashCode(table);

            // Action:
            start = end + 10;
            switch (changes[start])
            {
                case 'I': //insert
                    dcr.actionType = (int)TraceEventType.INSERT;
                    break;

                case 'U': //update
                    dcr.actionType = (int)TraceEventType.UPDATE;
                    break;

                case 'D': // delete
                    dcr.actionType = (int)TraceEventType.DELETE;
                    break;
            }
            changes = changes.Remove(0, start + 1);

            // User: 
            start = changes.IndexOf(", By: ") + 6;
            int skip;
            if (summary)
            {
                end = changes.IndexOf(", Count: ");
                skip = 9;
            }
            else
            {
                end = changes.IndexOf(", PK: ");
                skip = 6;
            }
            dcr.user = changes.Substring(start, end - start);

            start = end + skip;
            if (summary)
            {
                end = changes.IndexOf(", TotalCount: ");

                if (end == -1)
                {
                    // Affected record count
                    end = changes.IndexOf("' SUMMARY FROM");
                    dcr.changedColumns = Int32.Parse(changes.Substring(start, end - start));
                }
                else
                {
                    // Affected record count
                    dcr.changedColumns = Int32.Parse(changes.Substring(start, end - start));
                    start = end + 14;
                    // Total Columns changed
                    end = changes.IndexOf("' SUMMARY FROM");
                    dcr.totalChanges = Int32.Parse(changes.Substring(start, end - start));
                }
                dcr.recordNumber = 0;
                changes = "";
            }
            else
            {
                // Primary key
                end = changes.IndexOf(", No: ");
                dcr.primaryKey = changes.Substring(start, end - start);

                start = end + 6;
                end = changes.IndexOf(", <C: ");
                if (end > 0)
                {
                    dcr.recordNumber = Int32.Parse(changes.Substring(start, end - start));
                    changes = changes.Remove(0, end + 2);
                }
                else
                {
                    end = changes.IndexOf(", ' CHANGES ");
                    dcr.recordNumber = Int32.Parse(changes.Substring(start, end - start));
                    changes = changes.Remove(0, end + 1);
                }
            }
            return changes;
        }

        private void ParseColumnChanges(DataChangeRecord dcr, string list, DataTable columnChangeTable)
        {
            if (list == null || list.Length == 0)
                return;

            bool more = true;

            // format "<C: {0}><B: {1}><A: {2}><T: {3}>, ";

            int start;
            int end;
            int last = list.IndexOf("' CHANGES FROM");
            if (last < 2)
                return;

            int count = 0;
            int totalUpdated = 0;

            do
            {
                DataRow row = columnChangeTable.NewRow();
                ColumnChangeRecord rec = new ColumnChangeRecord();
                row["startTime"] = rec.startTime = dcr.startTime;
                row["eventSequence"] = rec.eventSequence = dcr.eventSequence;
                row["spid"] = rec.spid = dcr.spid;

                // column name
                start = list.IndexOf("<C: ") + 4;
                end = list.IndexOf("><B: ");
                row["columnName"] = rec.columnName = list.Substring(start, end - start);
                row["columnId"] = rec.columnId = NativeMethods.GetHashCode(rec.columnName);
                jobInfo.UpdateColumnIdCache(rec.columnName, rec.columnId);
                end += 5;

                // before
                list = list.Remove(0, end);
                last -= end;
                end = list.IndexOf("><A: ");
                row["beforeValue"] = rec.beforeValue = list.Substring(0, end).Trim();
                end += 5;
                last -= end;
                list = list.Remove(0, end);

                end = list.IndexOf("><T: ");

                if (end == -1)
                {
                    // after
                    end = list.IndexOf(">, ");
                    row["afterValue"] = rec.afterValue = list.Substring(0, end).Trim();
                    end += 3;
                    last -= end;
                    list = list.Remove(0, end);
                }
                else
                {
                    // after
                    row["afterValue"] = rec.afterValue = list.Substring(0, end).Trim();
                    end += 5;
                    last -= end;
                    list = list.Remove(0, end);

                    //total columns updated
                    end = list.IndexOf(">, ");
                    totalUpdated = Int32.Parse(list.Substring(0, end));
                    end += 3;
                    last -= end;
                    list = list.Remove(0, end);
                }

                // hashcode
                row["hashcode"] = rec.GetHashCode();

                columnChangeTable.Rows.Add(row);
                count++;
                if (last <= 0)
                    more = false;
            } while (more);
            dcr.changedColumns = count;
            dcr.totalChanges = totalUpdated;
        }

        //----------------------------------------------------------------------
        // CommitChanges(): insert the processed records, update the stats table 
        // and commit the transaction.
        // Note: The transaction object returned is always a null.
        //---------------------------------------------------------------------
        private SqlTransaction
           InsertDataChangeRecords(
              SqlConnection conn,
              SqlTransaction trans,
              DataTable dataChangeTable,
              DataTable columnChangeTable
           )
        {
            if (trans != null)
            {
                //TrimSpacesBeforeAfterDataValues(columnChangeTable);
                jobInfo.eventsInserted += columnChangeTable.Rows.Count;
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, String.Format("Transaction commit: dataChangeRecords: {0}", rowsWritten));

                BulkInsert(conn, trans, CoreConstants.RepositoryDataChangesTable, dataChangeTable);
                BulkInsert(conn, trans, CoreConstants.RepositoryColumnChangesTable, columnChangeTable);

                jobInfo.UpdateRowsProcessed(rowsWritten, conn, trans);
                trans.Commit();
                jobInfo.UpdateTableAndColumnIdTables(conn); // Keep this outside of the transaction
            }

            return null;
        }

        //[SQLCM-3775] Now this method is not required because Column changed values are being trimmed when they are parsed from Query string 

        //private void TrimSpacesBeforeAfterDataValues(DataTable columnChangeTable)
        //{
        //    foreach (DataRow row in columnChangeTable.Rows)
        //    {
        //        row[BEFORE_DATA_COLUMN_NAME] = row[BEFORE_DATA_COLUMN_NAME].ToString().Trim();
        //        row[AFTER_DATA_COLUMN_NAME] = row[AFTER_DATA_COLUMN_NAME].ToString().Trim();
        //    }
        //}

        private static void InitDataChangesTableColumns(DataTable table, SqlConnection conn)
        {
            InitTableColumns(table, CoreConstants.RepositoryDataChangesTable, dataChangeCols, conn);
        }

        private static void InitColumnChangesTableColumns(DataTable table, SqlConnection conn)
        {
            InitTableColumns(table, CoreConstants.RepositoryColumnChangesTable, colChangeCols, conn);
        }


        private bool UpdateDataChangeIds(DateTime startTime, DateTime endTime)
        {
            bool isEventIdNull = true;
            DateTime utcStartTime, utcEndTime;

            if (!CoreConstants.LinkDataChangeRecords)
                return false;

            if (startTime != DateTime.MinValue)
                utcStartTime = TimeZoneInfo.ToUniversalTime(jobInfo.timeZoneInfo, startTime);
            else
            {
                utcStartTime = SqlDateTime.MinValue.Value;
            }
            if (endTime != DateTime.MinValue)
                utcEndTime = TimeZoneInfo.ToUniversalTime(jobInfo.timeZoneInfo, endTime);
            else
            {
                utcEndTime = SqlDateTime.MinValue.Value;
            }

            Repository rep = new Repository();
            try
            {
                rep.OpenConnection(jobInfo.eventDatabase);
                string procName = "p_LinkDataChangeRecords";
                if (jobInfo.traceFile.Contains("xel"))
                {
                    string mapCondition = "t1.startTime >= t2.startTime AND t1.startTime <= t2.endTime AND t1.spid = t2.spid AND " +
                                          "t1.eventSequence >= t2.startSequence AND t1.eventSequence <= t2.endSequence";
                    if (jobInfo.SupportsRowCount())
                    {
                        mapCondition = "t1.guid = t2.guid AND t1.guid IS NOT NULL AND t1.guid <> '' AND t1.eventSequence< startSequence AND " +
                                       "t1.eventSequence > " +
                                       "(SELECT COALESCE((SELECT MAX(startSequence) FROM Events WHERE startSequence < " +
                                       "t2.startSequence AND guid = t2.guid " +
                                       "AND eventCategory=4 AND (eventType = 2 OR eventType = 8 OR eventType = 16)),0))";
                    }
                    string query = "UPDATE t1 SET t1.eventId=t2.eventId FROM DataChanges t1 INNER JOIN " +
                                     "(SELECT eventType, eventCategory, eventId, guid, startSequence FROM Events WHERE eventCategory=4 AND (eventType = 2 OR eventType = 8 OR eventType = 16)) t2 " +
                                     "ON (" + mapCondition + ") " +
                                     "WHERE t1.eventId IS NULL " +
                                     "UPDATE t1 set t1.dcId=t2.dcId FROM ColumnChanges t1 INNER JOIN DataChanges t2 " +
                                     "ON (t1.startTime = t2.startTime AND t1.eventSequence = t2.eventSequence AND t1.spid = t2.spid) " +
                                     "WHERE t1.dcId IS NULL AND t1.startTime >= " + SQLHelpers.CreateSafeDateTime(utcStartTime) + " AND t1.startTime <= " + SQLHelpers.CreateSafeDateTime(utcEndTime);
                    using (SqlCommand cmd = new SqlCommand(query, rep.connection))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                        lock(AcquireInstanceLock(jobInfo.instance))
                            cmd.ExecuteNonQuery();
                    }
                    return true;
                }
                using (SqlCommand cmd = new SqlCommand(procName, rep.connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    cmd.Parameters.Add("@startTime", SqlDbType.DateTime).Value = utcStartTime;
                    cmd.Parameters.Add("@endTime", SqlDbType.DateTime).Value = utcEndTime;
                    cmd.Parameters.Add("@isEventIdNull", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                    lock(AcquireInstanceLock(jobInfo.instance))
                        cmd.ExecuteNonQuery();
                    isEventIdNull = (int)cmd.Parameters["@isEventIdNull"].Value == 1 ? true : false;
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write("UpdateDataChangeIds failed.", e);
            }
            finally
            {
                rep.CloseConnection();
            }

            return isEventIdNull;
        }

        //5.4 XE
        public void AddTableColumnXE(DataTable table)
        {
            table.Columns.Add("EventClass", Type.GetType("System.Int32"));
            table.Columns.Add("EventSubClass", Type.GetType("System.Int32"));
            table.Columns.Add("StartTime", Type.GetType("System.DateTime"));
            table.Columns.Add("SPID", Type.GetType("System.Int32"));
            table.Columns.Add("ApplicationName", Type.GetType("System.String"));
            table.Columns.Add("HostName", Type.GetType("System.String"));
            table.Columns.Add("ServerName", Type.GetType("System.String"));
            table.Columns.Add("LoginName", Type.GetType("System.String"));
            table.Columns.Add("Success", Type.GetType("System.Int32"));
            table.Columns.Add("DatabaseName", Type.GetType("System.String"));
            table.Columns.Add("DatabaseID", Type.GetType("System.Int64"));
            table.Columns.Add("DBUserName", Type.GetType("System.String"));
            table.Columns.Add("ObjectType", Type.GetType("System.Int32"));
            table.Columns.Add("ObjectName", Type.GetType("System.String"));
            table.Columns.Add("ObjectID", Type.GetType("System.Int64"));
            table.Columns.Add("Permissions", Type.GetType("System.Int64"));
            table.Columns.Add("ColumnPermissions", Type.GetType("System.Int64"));
            table.Columns.Add("TargetLoginName", Type.GetType("System.String"));
            table.Columns.Add("TargetUserName", Type.GetType("System.String"));
            table.Columns.Add("RoleName", Type.GetType("System.String"));
            table.Columns.Add("OwnerName", Type.GetType("System.String"));
            table.Columns.Add("TextData", Type.GetType("System.String"));
            table.Columns.Add("NestLevel", Type.GetType("System.String"));
            table.Columns.Add("fileName", Type.GetType("System.String"));
            table.Columns.Add("linkedServerName", Type.GetType("System.String"));
            table.Columns.Add("parentName", Type.GetType("System.String"));
            table.Columns.Add("isSystem", Type.GetType("System.Int32"));
            table.Columns.Add("sessionLoginName", Type.GetType("System.String"));
            table.Columns.Add("providerName", Type.GetType("System.String"));
            table.Columns.Add("eventSequence", Type.GetType("System.Int64"));
            table.Columns.Add("rowCounts", Type.GetType("System.Int64"));
            table.Columns.Add("targetObject", Type.GetType("System.String"));
            table.Columns.Add("details", Type.GetType("System.String"));
            table.Columns.Add("eventType", Type.GetType("System.Int32"));
            table.Columns.Add("eventCategory", Type.GetType("System.Int32"));
            table.Columns.Add("hash", Type.GetType("System.Int32"));
            table.Columns.Add("deleteMe", Type.GetType("System.Int32"));
            table.Columns.Add("alertLevel", Type.GetType("System.Int32"));
            table.Columns.Add("checksum", Type.GetType("System.Int32"));
            table.Columns.Add("privilegedUser", Type.GetType("System.Int32"));
            table.Columns.Add("newRow", Type.GetType("System.Int32"));
            table.Columns.Add("keepSql", Type.GetType("System.Int32"));
            table.Columns.Add("processed", Type.GetType("System.Int32"));
            table.Columns.Add("startSequence", Type.GetType("System.Int64"));
            table.Columns.Add("endSequence", Type.GetType("System.Int64"));
            table.Columns.Add("endTime", Type.GetType("System.DateTime"));
            table.Columns.Add("guid", Type.GetType("System.String"));
        }

        //5.4 XE
        //Columns added for DataChange trace
        public void AddDataChangeTableColumnXE(DataTable table)
        {
            table.Columns.Add("EventClass", Type.GetType("System.Int32"));
            table.Columns.Add("StartTime", Type.GetType("System.DateTime"));
            table.Columns.Add("SPID", Type.GetType("System.Int32"));
            table.Columns.Add("EventSequence", Type.GetType("System.Int64"));
            table.Columns.Add("DatabaseID", Type.GetType("System.Int32"));
            table.Columns.Add("DatabaseName", Type.GetType("System.String"));
            table.Columns.Add("TextData", Type.GetType("System.String"));
            table.Columns.Add("guid", Type.GetType("System.String"));
        }

        //5.4 XE
        public void GetRowDataXE(XEventSingleEvent eventRow, DataRow row)
        {
            int eventClassId = 0;
            if (eventRow.EventName.Equals(TraceEventIdXE.SqlStmtStarting))
                eventClassId = (int)TraceEventId.SqlStmtStarting;
            else if (eventRow.EventName.Equals(TraceEventIdXE.SpStarting))
                eventClassId = (int)TraceEventId.SpStarting;
            else if (eventRow.EventName.Equals(TraceEventIdXE.RpcCompleted))
                eventClassId = (int)TraceEventId.RpcCompleted;
            else if (eventRow.EventName.Equals(TraceEventIdXE.SpCompleted))
                eventClassId = (int)TraceEventId.SpCompleted;
            else if (eventRow.EventName.Equals(TraceEventIdXE.SqlStmtCompleted))
                eventClassId = (int)TraceEventId.SqlStmtCompleted;
            row[ndxEventClass] = eventClassId;
            row[ndxEventSubclass] = 0;
            row[ndxStartTime] = eventRow.EventTime;
            row[ndxSpid] = Convert.ToInt32(eventRow.EventData["session_id"]);
            row[ndxApplicationName] = eventRow.EventData["client_app_name"].ToString();
            row[ndxHostName] = eventRow.EventData["client_hostname"].ToString();
            row[ndxServerName] = eventRow.EventData["server_instance_name"].ToString();
            row[ndxLoginName] = eventRow.EventData["server_principal_name"].ToString();
            row[ndxSuccess] = 1;
            row[ndxDatabaseName] = eventRow.EventData["database_name"].ToString();
            row[ndxDatabaseId] = Convert.ToInt32(eventRow.EventData["database_id"]);
            row[ndxDbUserName] = "";
            row[ndxObjectType] = 0;
            row[ndxObjectName] = "";
            row[ndxObjectId] = 0;
            row[ndxPermissions] = 0;
            row[ndxColumnPermissions] = 0;
            row[ndxTargetLoginName] = "";
            row[ndxTargetUserName] = "";
            row[ndxRoleName] = "";
            row[ndxOwnerName] = "";
            row[ndxTextData] = eventRow.EventData["statement"].ToString();
            row[ndxNestLevel] = (eventRow.EventData.ContainsKey("nest_level")) ? Convert.ToInt32(eventRow.EventData["nest_level"]) : 0;
            row[ndxFileName] = "";
            row[ndxLinkedServerName] = "";
            row[ndxParentName] = "";
            row[ndxIsSystem] = (eventRow.EventData["is_system"].ToString().Equals("false")) ? 0 : 1;
            row[ndxSessionLoginName] = eventRow.EventData["session_server_principal_name"].ToString();
            row[ndxProviderName] = "";            
            if (eventClassId == (int)TraceEventId.SpCompleted
                || eventClassId == (int)TraceEventId.SqlStmtCompleted)
            {
                if (IsRowCountEnabled(jobInfo.instance))
                {
                    row[ndxRowCounts] = Convert.ToInt64(eventRow.EventData["row_count"]);
                }     
                else
                    row[ndxRowCounts] = DBNull.Value;
            }
            try
            {
                object activityIDObj = eventRow.EventData["attach_activity_id"];
                Type activityIDType = activityIDObj.GetType();
                PropertyInfo activityInfo = activityIDType.GetProperty("Sequence");
                object activityID = activityInfo.GetValue(activityIDObj, null);
                if (activityID != null)
                {
                    row[ndxEventSequence] = Convert.ToInt64(activityID);
                }
                else
                {
                    row[ndxEventSequence] = Convert.ToInt64(eventRow.EventData["event_sequence"]);
                }

                PropertyInfo activityInfoId = activityIDType.GetProperty("Id");
                object activityGUID = activityInfoId.GetValue(activityIDObj, null);
                if (activityGUID != null)
                {
                    row[ndxGUId] = activityGUID.ToString();
                }
            }
            catch
            {
                // In case of exception, use the event sequence
                row[ndxEventSequence] = Convert.ToInt64(eventRow.EventData["event_sequence"]);
            }

        }

        //5.4 XE
        public void GetDataChangeRowDataXE(XEventSingleEvent eventRow, DataRow row)
        {
            int eventClassId = 0;
            if (eventRow.EventName.Equals(TraceEventIdXE.SqlStmtStarting))
                eventClassId = (int)TraceEventId.SqlStmtStarting;
            else if (eventRow.EventName.Equals(TraceEventIdXE.SpStarting))
                eventClassId = (int)TraceEventId.SpStarting;
            else if (eventRow.EventName.Equals(TraceEventIdXE.RpcCompleted))
                eventClassId = (int)TraceEventId.RpcCompleted;

            try
            {
                object activityIDObj = eventRow.EventData["attach_activity_id"];
                Type activityIDType = activityIDObj.GetType();
                PropertyInfo activityInfo = activityIDType.GetProperty("Sequence");
                object activityID = activityInfo.GetValue(activityIDObj, null);
                if (activityID != null)
                {
                    row[3] = Convert.ToInt64(activityID);
                }
                else
                {
                    row[3] = Convert.ToInt64(eventRow.EventData["event_sequence"]);
                }
                PropertyInfo activityInfoId = activityIDType.GetProperty("Id");
                object activityGUID = activityInfoId.GetValue(activityIDObj, null);
                if (activityGUID != null)
                {
                    row[7] = activityGUID.ToString();
                }
            }
            catch
            {
                // In case of exception, use the event sequence
                row[3] = Convert.ToInt64(eventRow.EventData["event_sequence"]);
            }

            row[0] = eventClassId;
            row[1] = eventRow.EventTime;
            row[2] = Convert.ToInt32(eventRow.EventData["session_id"]);
            row[4] = Convert.ToInt32(eventRow.EventData["database_id"]);
            row[5] = eventRow.EventData["database_name"].ToString();
            row[6] = eventRow.EventData["statement"].ToString();
        }

        public void GetObjectDataXE(DataRow row)
        {
            string temp = row[ndxTextData].ToString();

            var parser = new TSql100Parser(true);
            IList<ParseError> errors = new List<ParseError>();
            string table = "";
            string schema = "";
            int flag = -1;
            string dataChangeQuery = "DELETE [SQLCOMPLIANCE_DATA_CHANGE].[SQLCOMPLIANCE_CHANGED_DATA_TABLE] WHERE 1 = 0";
            if (dataChangeQuery.Equals(temp.ToUpper()))
            {
                row[ndxObjectName] = "SQLcompliance_Changed_Data_Table";
                row[ndxTargetObject] = "SQLcompliance_Data_Change";
                row[ndxParentName] = "SQLcompliance_Data_Change";
            }
            else
            {
                using (TextReader r = new StringReader(temp))
                {
                    var result = parser.GetTokenStream(r, errors);

                    if ((int)row[ndxEventType] == (int)TraceEventType.SELECT
                        || (int)row[ndxEventType] == (int)TraceEventType.DELETE
                        || (int)row[ndxEventType] == (int)TraceEventType.INSERT)
                    {
                        var tables = result
                            .Select((i, index) => (i.TokenType == TSqlTokenType.From || i.TokenType == TSqlTokenType.Into) ? ((result[index + 3].TokenType == TSqlTokenType.EndOfFile || result[index + 3].TokenType == TSqlTokenType.WhiteSpace
                                || result[index + 3].TokenType == TSqlTokenType.LeftParenthesis) ?
                                result[index + 2].Text : ((result[index + 5].TokenType == TSqlTokenType.EndOfFile || result[index + 5].TokenType == TSqlTokenType.WhiteSpace
                                || result[index + 3].TokenType == TSqlTokenType.LeftParenthesis) ?
                                result[index + 4].Text : ((result[index + 4].TokenType == TSqlTokenType.Dot) ?
                                result[index + 5].Text : result[index + 6].Text))) : null)
                            .Where(i => i != null)
                            .ToArray();

                        if (tables.Length == 0) {                            
                            //The result is obteined through a group of ternary operators into a lambda operation that are executed if the token type is equals to Insert, 
                            //if it is not the case, the return result will be null
                            //Those conditionals determines the position of the data in the tokens array that we are looking for
                            tables = result
                            .Select((i, index) => (i.TokenType == TSqlTokenType.Insert) ? ((result[index + 3].TokenType == TSqlTokenType.EndOfFile || result[index + 3].TokenType == TSqlTokenType.WhiteSpace
                                || result[index + 3].TokenType == TSqlTokenType.LeftParenthesis) ?
                                result[index + 2].Text : ((result[index + 5].TokenType == TSqlTokenType.EndOfFile || result[index + 5].TokenType == TSqlTokenType.WhiteSpace
                                || result[index + 3].TokenType == TSqlTokenType.LeftParenthesis) ?
                                result[index + 4].Text : ((result[index + 4].TokenType == TSqlTokenType.Dot) ?
                                result[index + 5].Text : result[index + 6].Text))) : null)
                            .Where(i => i != null)
                            .ToArray();                           
                        }

                        var schemas = result
                            .Select((i, index) => (i.TokenType == TSqlTokenType.From || i.TokenType == TSqlTokenType.Into) ? ((result[index + 3].TokenType == TSqlTokenType.EndOfFile
                                || result[index + 3].TokenType == TSqlTokenType.WhiteSpace
                                || result[index + 3].TokenType == TSqlTokenType.LeftParenthesis) ?
                                "dbo" : ((result[index + 4].TokenType == TSqlTokenType.Dot) ?
                                "dbo" : ((result[index + 5].TokenType == TSqlTokenType.Dot) ?
                                result[index + 4].Text : result[index + 2].Text))) : null)
                            .Where(i => i != null)
                            .ToArray();
                        table = (tables != null && tables.Length > 0) ? tables[0] : "";
                        schema = (schemas != null && schemas.Length > 0) ? schemas[0] : "";
                    }
                    else if ((int)row[ndxEventType] == (int)TraceEventType.UPDATE)
                    {
                        for (int index = 0; index < result.Count; index++)
                        {
                            if (result[index].TokenType == TSqlTokenType.Update)
                            {
                                if (result[index + 2].TokenType != TSqlTokenType.Top)
                                    flag = index;
                                else
                                {
                                    for (int i = index; i < result.Count; i++)
                                    {
                                        if (result[i].TokenType == TSqlTokenType.RightParenthesis)
                                        {
                                            if (result[i + 1].TokenType != TSqlTokenType.WhiteSpace)
                                                i = i - 1;
                                            flag = i;
                                            break;
                                        }
                                    }
                                }
                                if (flag != -1)
                                {
                                    table = (result[flag + 3].TokenType == TSqlTokenType.WhiteSpace) ?
                                                        result[flag + 2].Text : (result[flag + 5].TokenType == TSqlTokenType.EndOfFile || (result[flag + 5].TokenType == TSqlTokenType.WhiteSpace) ?
                                                        result[flag + 4].Text : ((result[flag + 4].Text).Equals(".") ?
                                                        result[flag + 5].Text : result[flag + 6].Text));

                                    schema = (result[flag + 3].TokenType == TSqlTokenType.EndOfFile || (result[flag + 3].TokenType == TSqlTokenType.WhiteSpace) ?
                                        "dbo" : ((result[flag + 4].TokenType == TSqlTokenType.Dot) ?
                                        "dbo" : ((result[flag + 5].TokenType == TSqlTokenType.Dot) ?
                                                result[flag + 4].Text : result[flag + 2].Text)));
                                }
                                break;
                            }
                        }
                    }
                    if (table != null && table.Length > 0)
                    {
                        row[ndxObjectName] = table.Replace("[", "").Replace("]", "");
                        row[ndxTargetObject] = table.Replace("[", "").Replace("]", "");
                        row[ndxObjectId] = 0;
                        row[ndxObjectType] = "8277";
                    }
                    if (schema != null && schema.Length > 0)
                    {
                        row[ndxParentName] = schema.Replace("[", "").Replace("]", "");
                    }

                }
            }
        }

        #endregion

        #region Sensitive Column trace

        private void ProcessSensitiveColumnTrace()
        {
            LoadDataSet();

            DataTable table = tempEventsDataSet.Tables["Events"];
            int totalRows = 0;

            using (DataTable sensitiveColumnsTable = new DataTable())
            {
                Repository repo = new Repository();
                repo.OpenConnection(jobInfo.eventDatabase);
                InitSensitiveColumnTableColumns(sensitiveColumnsTable, repo.connection);
                repo.CloseConnection();

                try
                {
                    totalRows = ProcessSCRows(table.Rows, sensitiveColumnsTable);

                    if (jobInfo.GetAborting())
                    {
                        return;
                    }
                    ErrorLog.Instance.Write(ErrorLog.Level.Verbose, "Start TraceJob:ProcessSensitiveColumnTrace Phase 4");


                    if (totalRows > 0)
                    {
                        lock (AcquireInstanceLock(jobInfo.instance))
                        {
                            WriteSensitiveColumEvents(totalRows, tempEventsDataSet.Tables["Events"], sensitiveColumnsTable);
                        }
                        AlertingJobPool.SignalNewEventsAvailable();
                    }

                    if (jobInfo.GetAborting())
                    {
                        return;
                    }
                }
                catch (Exception ex)
                {
                    ErrorLog.Instance.Write("TraceJob::ProcessSensitiveColumnTrace",
                                            String.Format(CoreConstants.Exception_ErrorProcessingTraceFile,
                                                          jobInfo.traceFile),
                                             ex,
                                             true);
                    throw;
                }
                finally
                {
                    if (tempEventsDataSet != null)
                    {
                        foreach (DataTable t in tempEventsDataSet.Tables)
                        {
                            foreach (DataColumn d in t.Columns)
                                d.Dispose();
                            t.Dispose();
                        }
                        tempEventsDataSet.Dispose();
                    }
                }
            }
        }

        // 5.4 XE
        private void ProcessSensitiveColumnTraceXE()
        {
            XEventData eventList = new XEventData(jobInfo.traceFile);
            int totalRows = 0;

            using (DataTable sensitiveColumnsTable = new DataTable())
            {
                Repository repo = new Repository();
                repo.OpenConnection(jobInfo.eventDatabase);
                InitSensitiveColumnTableColumns(sensitiveColumnsTable, repo.connection);
                repo.CloseConnection();

                DataTable table = new DataTable();
                AddTableColumnXE(table);
                try
                {
                    columnTablesConfig = GetDictAuditedColumns(jobInfo.instance);
                    totalRows = ProcessSCRowsXE(eventList, table, sensitiveColumnsTable);

                    if (jobInfo.GetAborting())
                    {
                        return;
                    }
                    ErrorLog.Instance.Write(ErrorLog.Level.Verbose, "Start TraceJob:ProcessSensitiveColumnTrace Phase 4");


                    if (totalRows > 0)
                    {
                        lock (AcquireInstanceLock(jobInfo.instance))
                        {
                            WriteSensitiveColumEvents(table.Rows.Count, table, sensitiveColumnsTable);
                        }
                        AlertingJobPool.SignalNewEventsAvailable();
                    }

                    if (jobInfo.GetAborting())
                    {
                        return;
                    }
                }
                catch (Exception ex)
                {
                    ErrorLog.Instance.Write("TraceJob::ProcessSensitiveColumnTrace",
                                            String.Format(CoreConstants.Exception_ErrorProcessingTraceFile,
                                                          jobInfo.traceFile),
                                             ex,
                                             true);
                    throw;
                }
            }
        }

        private int ProcessSCRows(DataRowCollection rows, DataTable scTable)
        {
            if (rows.Count == 0)
                return 0;

            int rowCount = 0;
            bool preoverlap = true;
            bool overlapped = false;
            bool getFirstEventTime = true;
            TimesRecord lastTrace;
            StateRecord state = new StateRecord();

            // Find the most current (last) event Time
            // The first event is usually an invalid event with null as the StartTime
            int i = rows.Count - 1;

            while (jobInfo.lastEventTime == DateTime.MinValue && i >= 0)
            {
                jobInfo.lastEventTime = GetRowDateTime(rows[i], "StartTime");
                i--;
            }
            // get last trace processed times
            lastTrace = TimesRecord.Read(jobInfo.instance,
                                          jobInfo.traceType);

            overlapped = Overlapped(lastTrace.StartTime,
                                     lastTrace.EndTime,
                                     jobInfo.firstEventTime,
                                     jobInfo.lastEventTime);
            int rowIndex = -1;
            List<DataRow> objectEventRows = new List<DataRow>();
            columnTablesConfig = GetDictAuditedColumns(jobInfo.instance);
            foreach (DataRow row in rows)
            {
                rowIndex++;
                if (row == null)
                    continue;

                //this needs to be set first.
                row[ndxDeleteme] = 1;

                if (!IsValidRow(row))
                    continue;

                // change success column - some events return NULL in this 
                // column - this is equivalent to success=1 since it means
                // we are doing a real event instead of permission event
                if (row.IsNull(ndxSuccess))
                {
                    row[ndxSuccess] = 1;
                }
                row[ndxKeepSql] = true;
                row[ndxEventCategory] = (int)TraceEventCategory.SELECT;
                row[ndxEventType] = (int)TraceEventType.SELECT;
                //SQLCM-5471 - v5.6
                var txtData = row[ndxTextData].ToString().ToUpper();
                if (txtData.StartsWith("SELECT"))
                {
                    row[ndxEventCategory] = (int)TraceEventCategory.SELECT;
                    row[ndxEventType] = (int)TraceEventType.SELECT;
                    row[ndxPermissions] = 1;
                }
                else if (IsDMLSqlStatement(txtData) || (txtData.StartsWith("EXEC") && !txtData.StartsWith("EXECUTE AS")))
                {
                    row[ndxEventCategory] = TraceEventCategory.DML;
                    if (txtData.StartsWith("UPDATE"))
                    {
                        row[ndxEventType] = TraceEventType.UPDATE;
                        row[ndxPermissions] = 2;
                    }
                    if (txtData.StartsWith("DELETE"))
                    {
                        row[ndxEventType] = TraceEventType.DELETE;
                        row[ndxPermissions] = 16;
                    }
                    if (txtData.StartsWith("INSERT"))
                    {
                        row[ndxEventType] = TraceEventType.INSERT;
                        row[ndxPermissions] = 8;
                    }
                    if (txtData.StartsWith("EXEC") && !txtData.StartsWith("EXECUTE AS"))
                    {
                        row[ndxEventType] = TraceEventType.EXECUTE;
                        row[ndxPermissions] = 32;
                    }
                }
                else if (txtData.ToUpper().StartsWith("ALTER") || txtData.ToUpper().StartsWith("DROP"))
                {
                    row[ndxEventCategory] = (int)TraceEventCategory.DDL;
                    row[ndxEventType] = (int)TraceEventType.AlterUserTable;
                    row[ndxPermissions] = 217;
                }
                else if (txtData.ToUpper().StartsWith("TRUNCATE"))
                {
                    row[ndxEventCategory] = (int)TraceEventCategory.DML;
                    row[ndxEventType] = (int)TraceEventType.DELETE;
                    row[ndxPermissions] = 16;
                }
                else if (txtData.ToUpper().StartsWith("DBCC"))
                {
                    row[ndxEventCategory] = (int)TraceEventCategory.Admin;
                    row[ndxEventType] = (int)TraceEventType.DBCC;
                    row[ndxPermissions] = 80;
                }
                //SQLCM-5471 - v5.6 - END

                if (getFirstEventTime && !row.IsNull("StartTime"))
                {
                    jobInfo.firstEventTime = GetRowDateTime(row, "StartTime");
                    getFirstEventTime = false;
                }

                // skip overlap regions
                if (preoverlap && overlapped)
                {
                    if (lastTrace.EndTime.CompareTo(GetRowDateTime(row, ndxStartTime)) > 0)
                    {
                        if ((TraceEventId)GetRowInt32(row, ndxEventClass) == TraceEventId.SqlStmtCompleted
                            || (TraceEventId)GetRowInt32(row, ndxEventClass) == TraceEventId.SpStmtCompleted)
                        {
                            UpdateRowCounts(row);
                        }
                        jobInfo.eventsOverlap++;
                        continue;
                    }
                    else
                    {
                        preoverlap = false;
                    }
                }
                if ((TraceEventId)GetRowInt32(row, ndxEventClass) == TraceEventId.AuditObjectPermission)
                {
                    row[ndxStartSequence] = row[ndxEventSequence];
                    objectEventRows.Add(row);
                }
                // Change for RowCount
                if ((TraceEventId)GetRowInt32(row, ndxEventClass) == TraceEventId.SqlStmtCompleted
                    || (TraceEventId)GetRowInt32(row, ndxEventClass) == TraceEventId.SpStmtCompleted)
                {
                    bool rowMatched = false;
                    if (objectEventRows.Count == 0)
                    {
                        row[ndxStartSequence] = row[ndxEventSequence];
                        objectEventRows.Add(row);
                        rowMatched = true;
                    }
                    else
                    {
                        for (int count = 0; count < objectEventRows.Count; )
                        {
                            //SQLCM-5677 - commented this method as the logic is not working for Views, SPs, Triggers for sensitive columns
                            //if (RowsMatched(objectEventRows[count], row, true))
                            //{
                                objectEventRows[count][ndxTextData] = row[ndxTextData];
                                objectEventRows[count][ndxRowCounts] = row[ndxRowCounts];
                                objectEventRows.RemoveAt(count);
                                rowMatched = true;
                            //}
                            //else
                            //{
                            //    count++;
                            //}
                        }
                    }
                    if (!rowMatched)
                        UpdateRowCounts(row);
                    continue;
                }

                if (SQLHelpers.GetRowInt32(row, "EventClass") == (int)TraceEventId.SqlStmtStarting)
                {
                    state.LoadRow(row);
                    continue;
                }

                    if (SQLHelpers.GetRowInt32(row, "EventClass") == (int)TraceEventId.AuditObjectPermission
                        || SQLHelpers.GetRowInt32(row, "EventClass") == (int)TraceEventId.AuditSchemaObjectManagement  //SQLCM-5471 - v5.6
                        || SQLHelpers.GetRowInt32(row, "EventClass") == (int)TraceEventId.SqlStmtCompleted)
                {
                    if (ProcessSensitiveColumnRow(row, rowCount, scTable))
                    {
                        rowCount++;
                        row[ndxDeleteme] = 0;
                        //Get the sql text from sqlstmtStarting if it matches the select event (114), otherwise use the sqlText from the 114 event
                        state.startTime = TimeZoneInfo.ToUniversalTime(jobInfo.timeZoneInfo, state.startTime);  //at this point the time for the sc event has been converted to UTC.

                        if (Matches(row, state))
                        {
                            string sqlText = state.textData;
                            row[ndxTextData] = sqlText;
                        }

                        //we are keeping the row, see if it is from a priveleged user
                        IsValidDbEvents(row);
                    }
                }
            }
            return rowCount;
        }

        //5.4 XE
        private int ProcessSCRowsXE(XEventData events, DataTable table, DataTable scTable)
        {
            if (events.EventData.Count == 0)
            {
                return 0;
            }

            int rowCount = 0;
            bool preoverlap = true;
            bool overlapped = false;
            bool getFirstEventTime = true;
            TimesRecord lastTrace;
            StateRecord state = new StateRecord();

            // Find the most current (last) event Time
            // The first event is usually an invalid event with null as the StartTime
            int i = events.EventData.Count - 1;
            while (jobInfo.lastEventTime == DateTime.MinValue && i >= 0)
            {
                jobInfo.lastEventTime = timeZoneInfo.Convert(jobInfo.timeZoneInfo, events.EventData[i].EventTime).AddMilliseconds(100);
                i--;
            }
            // get last trace processed times
            lastTrace = TimesRecord.Read(jobInfo.instance,
                                          jobInfo.traceType);

            overlapped = Overlapped(lastTrace.StartTime,
                                     lastTrace.EndTime,
                                     jobInfo.firstEventTime,
                                     jobInfo.lastEventTime);

            foreach (XEventSingleEvent eventRow in events.EventData)
            {
                if (eventRow == null) continue;

                if (!eventRow.EventData.ContainsKey("sql_text")) continue;
                string sqlText = eventRow.EventData["statement"].ToString();
                //if (!sqlText.ToUpper().StartsWith("SELECT")) continue;
                IList<ParseError> parseErrors;
                TSqlScript parsedScript = ObjectParser.ParseObjects(sqlText, out parseErrors);
                ObjectParser parser = new ObjectParser();
                parser.SQLText = sqlText; //SQLCM-5471 - v5.6
                parser.DatabaseName = eventRow.EventData["database_name"].ToString();//SQLCM-5471 - v5.6
                parser.ServerInstance = eventRow.EventData["server_instance_name"].ToString();
                parser.GetObjects(parsedScript);
                //this needs to be set first.
                foreach (string tableName in parser.TableListWithSchema)
                {
                    DataRow row = table.NewRow();
                    GetRowDataXE(eventRow, row);
                    row[ndxObjectName] = tableName.Split('.')[1];
                    row[ndxParentName] = tableName.Split('.')[0];
                    row[ndxPermissions] = 1;
                    table.Rows.Add(row);
                }
            }

            foreach (DataRow row in table.Rows)
            {
                if (!IsValidRow(row))
                    continue;

                row[ndxStartTime] = timeZoneInfo.Convert(jobInfo.timeZoneInfo, (DateTime)row[ndxStartTime]);
                row[ndxDeleteme] = 1;

                // change success column - some events return NULL in this 
                // column - this is equivalent to success=1 since it means
                // we are doing a real event instead of permission event
                if (row.IsNull(ndxSuccess))
                {
                    row[ndxSuccess] = 1;
                }
                row[ndxKeepSql] = true;
                row[ndxEventCategory] = (int)TraceEventCategory.SELECT;
                row[ndxEventType] = (int)TraceEventType.SELECT;
                //SQLCM-5471 - v5.6
                var txtData = row[ndxTextData].ToString().ToUpper();
                if (txtData.StartsWith("SELECT"))
                {
                    row[ndxEventCategory] = (int)TraceEventCategory.SELECT;
                    row[ndxEventType] = (int)TraceEventType.SELECT;
                    row[ndxPermissions] = 1;
                }
                else if (IsDMLSqlStatement(txtData) || (txtData.StartsWith("EXEC") && !txtData.StartsWith("EXECUTE AS")))
                {
                    row[ndxEventCategory] = TraceEventCategory.DML;
                    if (txtData.StartsWith("UPDATE"))
                    {
                        row[ndxEventType] = TraceEventType.UPDATE;
                        row[ndxPermissions] = 2;
                    }
                    if (txtData.StartsWith("DELETE"))
                    {
                        row[ndxEventType] = TraceEventType.DELETE;
                        row[ndxPermissions] = 16;
                    }
                    if (txtData.StartsWith("INSERT"))
                    {
                        row[ndxEventType] = TraceEventType.INSERT;
                        row[ndxPermissions] = 8;
                    }
                    if (txtData.StartsWith("EXEC") && !txtData.StartsWith("EXECUTE AS"))
                    {
                        row[ndxEventType] = TraceEventType.EXECUTE;
                        row[ndxPermissions] = 32;
                    }
                }
                else if (txtData.ToUpper().StartsWith("ALTER") || txtData.ToUpper().StartsWith("DROP"))
                {
                    row[ndxEventCategory] = (int)TraceEventCategory.DDL;
                    row[ndxEventType] = TraceEventType.AlterUserTable;
                    row[ndxPermissions] = 217;
                }
                else if (txtData.ToUpper().StartsWith("TRUNCATE"))
                {
                    row[ndxEventCategory] = (int)TraceEventCategory.DML;
                    row[ndxEventType] = (int)TraceEventType.DELETE;
                    row[ndxPermissions] = 16;
                }
                else if (txtData.ToUpper().StartsWith("DBCC"))
                {
                    row[ndxEventCategory] = (int)TraceEventCategory.Admin;
                    row[ndxEventType] = (int)TraceEventType.DBCC;
                    row[ndxPermissions] = 80;
                }
                //SQLCM-5471 - v5.6

                if (getFirstEventTime && !row.IsNull("StartTime"))
                {
                    jobInfo.firstEventTime = GetRowDateTime(row, "StartTime");
                    getFirstEventTime = false;
                }

                // skip overlap regions
                if (preoverlap && overlapped)
                {
                    if (lastTrace.EndTime.CompareTo(GetRowDateTime(row, ndxStartTime)) > 0)
                    {
                        jobInfo.eventsOverlap++;
                        continue;
                    }
                    else
                    {
                        preoverlap = false;
                    }
                }

                if (SQLHelpers.GetRowInt32(row, "EventClass") == (int)TraceEventId.SqlStmtCompleted
                    || SQLHelpers.GetRowInt32(row, "EventClass") == (int)TraceEventId.SpCompleted
                    || SQLHelpers.GetRowInt32(row, "EventClass") == (int)TraceEventId.SqlStmtStarting
                    || SQLHelpers.GetRowInt32(row, "EventClass") == (int)TraceEventId.SpStarting)
                {
                    if (ProcessSensitiveColumnRow(row, rowCount, scTable))
                    {
                        rowCount++;
                        row[ndxDeleteme] = 0;

                        //Get the sql text from sqlstmtStarting if it matches the select event (114), otherwise use the sqlText from the 114 event
                        state.startTime = TimeZoneInfo.ToUniversalTime(jobInfo.timeZoneInfo, state.startTime);  //at this point the time for the sc event has been converted to UTC.

                        if (Matches(row, state))
                        {
                            string sqlText = state.textData;
                            row[ndxTextData] = sqlText;
                        }

                        //we are keeping the row, see if it is from a priveleged user
                        IsValidDbEvents(row);
                    }
                }
            }
            return rowCount;
        }

        protected bool ProcessSensitiveColumnRow(DataRow row, int rowNumber, DataTable sensitiveColumnTable)
        {
            bool parseError = false;
            bool retval = false;

            List<ColumnTableConfiguration> tableConfigs = null;
            if (columnTablesConfig.ContainsKey(GetRowInt32(row, ndxDatabaseId)))
                tableConfigs = columnTablesConfig[GetRowInt32(row, ndxDatabaseId)].
                                        FindAll(x => x.Name.ToLower().Split(',').Contains(GetRowString(row, ndxParentName).ToLower() + "." + GetRowString(row, ndxObjectName).ToLower()));

            
            //Get the Sensitive columns for the table in this event, if they exist.  If they don't exist, this table
            // was not being audited for sensitive columns
            if (tableConfigs == null)
            {
                //If it is a sensitive column only trace, quit.  Otherwise, it is 
                // sensitive column and select so there is no parsing to do.  Just 
                //check the event filter and quit
                if (jobInfo.traceCategory == (int)TraceCategory.SensitiveColumn)
                    return false;
                else
                {
                    // process the select event
                    // check filter?
                    if (TraceFilter.RowMatchesEventFilter(this, row))
                    {
                        if (CreateCalculatedColumns(row) && !RowMatchesExcludeFilter(row))
                        {
                            IncrementTraceEventCount(GetRowInt32(row, ndxEventClass), 1);

                            // if we are here, then this must be a valid event; unmark deletion flag
                            row[ndxDeleteme] = 0;
                            retval = true;
                        }
                        else // event is deleted by the exclude filter
                        {
                            IncrementTraceEventCount(GetRowInt32(row, ndxEventClass), 2);
                            jobInfo.eventsDeleted++;
                        }
                    }
                    else // event is filtered out
                    {
                        IncrementTraceEventCount(GetRowInt32(row, ndxEventClass), 3);
                        jobInfo.eventsFilteredOut++;
                    }

                    if (!retval)
                        AddEventFilteredStats(GetRowDateTime(row, ndxStartTime));

                    return retval;
                }
            }
            LoadRow(row);

            int indexIndividual = tableConfigs.FindIndex(x => x.Type.ToUpper() == "INDIVIDUAL"
                    && x.Name.ToLower().Equals(String.Format("{0}.{1}", GetRowString(row, TraceJob.ndxParentName).ToLower(), GetRowString(row, TraceJob.ndxObjectName).ToLower())));
            
            //SQLCM-5471 - v5.6
            if (indexIndividual >= 0)
            {
                if (jobInfo.dbconfigs[databaseId].AuditSensitiveColumnActivity == SensitiveColumnActivity.SelectOnly
                    && isSensitiveDataQuery(textData.ToUpper()))
                    return false;
                if (jobInfo.dbconfigs[databaseId].AuditSensitiveColumnActivity == SensitiveColumnActivity.SelectAndDML
                    && ((textData.ToUpper().StartsWith("EXEC") && !textData.ToUpper().StartsWith("EXECUTE AS")) ||
                    textData.ToUpper().StartsWith("ALTER") || textData.ToUpper().StartsWith("DBCC") || textData.ToUpper().StartsWith("DROP") ||
                    textData.StartsWith("DISABLE TRIGGER") || textData.StartsWith("ENABLE TRIGGER") ||
                    textData.StartsWith("TRUNCATE TABLE") || textData.StartsWith("UPDATE STATISTICS")))
                    return false;
            }
            else
            {
                if (jobInfo.dbconfigs[databaseId].AuditSensitiveColumnActivityDataset == SensitiveColumnActivity.SelectOnly
                    && isSensitiveDataQuery(textData.ToUpper()))
                    return false;
                if (jobInfo.dbconfigs[databaseId].AuditSensitiveColumnActivityDataset == SensitiveColumnActivity.SelectAndDML
                    && ((textData.ToUpper().StartsWith("EXEC") && !textData.ToUpper().StartsWith("EXECUTE AS")) ||
                    textData.ToUpper().StartsWith("ALTER") || textData.ToUpper().StartsWith("DBCC") || textData.ToUpper().StartsWith("DROP") ||
                    textData.StartsWith("DISABLE TRIGGER") || textData.StartsWith("ENABLE TRIGGER") ||
                    textData.StartsWith("TRUNCATE TABLE") || textData.StartsWith("UPDATE STATISTICS")))
                    return false;
            }
            //SQLCM-5471 - v5.6
            //Parse the query and see if there is any sensitive column info.
            IList<ParseError> parseErrors;
            TSqlScript parsedScript = ColumnParser.ParseColumns(textData, out parseErrors);
            ColumnParser parser = new ColumnParser();
            int categeroy = jobInfo.traceCategory;//SQLCM-5471 - v5.6
            parser.SQLText = textData;//SQLCM-5471 - v5.6
            parser.DatabaseName = GetRowString(row, ndxDatabaseName);//SQLCM-5471 - v5.6
            parser.ServerInstance = GetRowString(row, ndxServerName);
            parser.GetColumns(parsedScript);

            //log any errors parsing the script
            if (textData == null || parseErrors.Count > 0)
            {
                if (parseErrors.Count > 0)
                {
                    if (CoreConstants.LogSQLParsingErrors)
                    {
                        StringBuilder msg = new StringBuilder();
                        msg.Append("There was an eror parsing the T-Sql. The event will still be stored in the SQL complaince manager Repository.  Error parsing T-Sql:");

                        foreach (ParseError error in parseErrors)
                        {
                            msg.AppendFormat(" {0}, ", error.Message);
                        }
                        ErrorLog.Instance.Write(String.Format("{0} \r\n {1}", msg.ToString(), textData), ErrorLog.Severity.Warning);
                    }
                }
                else
                {
                    ErrorLog.Instance.Write("The SQL Text for this event was null.", ErrorLog.Severity.Warning);
                }
                //if we can't process the sql text.  log it but store the event.
                parseError = true;
            }
            List<string> accessedColumns = new List<string>();

            //if there was an error, make that error be the column list but still store the event.
            if (parseError)
            {
                accessedColumns.Add("Unable to parse the query.");
                AddAccessedColumns(accessedColumns, rowNumber, sensitiveColumnTable, GetTableId());
            }
            else
            {
                if (!ProcessSensitiveColumns(row, tableConfigs, parser, out accessedColumns))
                {
                    if (jobInfo.traceCategory == (int)TraceCategory.SensitiveColumn)
                        return false;
                    else
                        return CreateCalculatedColumns(row);
                }
                //If none of the sensitive columns were accessed and we are not auditing for selects, we don't want this row.
                if (jobInfo.traceCategory == (int)TraceCategory.SensitiveColumn && accessedColumns.Count <= 0)
                    return false;
                //create the table Id
                int tableId = GetTableId();
                jobInfo.UpdateTableIdCache(parentName, objectName, tableId);
                AddAccessedColumns(accessedColumns, rowNumber, sensitiveColumnTable, tableId);
            }
            return CreateCalculatedColumns(row);
        }

        private int GetTableId()
        {
            string fullTable;
            if (String.IsNullOrEmpty(parentName))
                fullTable = objectName;
            else
                fullTable = String.Format("{0}.{1}", parentName, objectName);
            return (NativeMethods.GetHashCode(fullTable));
        }

        private bool GetSensitiveColumns(DataRow row, out Dictionary<string, TableConfiguration> tableConfig)
        {
            tableConfig = new Dictionary<string, TableConfiguration>();
            TableConfiguration config = new TableConfiguration();
            int dbId = GetRowInt32(row, ndxDatabaseId);
            bool foundConfig = false;

            if (jobInfo.scTableLists.ContainsKey(dbId))
            {
                Dictionary<string, TableConfiguration> tables = jobInfo.scTableLists[dbId];

                foreach (string tableName in tables.Keys)
                {
                    string table = GetRowString(row, ndxObjectName);
                    if (!row.IsNull(ndxParentName))
                        table = GetRowString(row, ndxParentName) + "." + table;

                    if (tableName.ToUpper() == table.ToUpper())
                    {
                        tableConfig.Add(tableName, tables[tableName]);
                        foundConfig = true;
                        continue;
                    }
                    if (CheckDataSetTableCondition(tables[tableName], tableName, table, out config))
                    {
                        tableConfig.Add(tableName, config);
                        foundConfig = true;
                        continue;
                    }
                }
            }
            if (foundConfig)
            {
                return true;
            }
            else
            {
                //This object should not be empty or null, that is why adding a dummy string
                tableConfig.Add("DummyTable", config);
                return false;
            }
        }

        private bool CheckDataSetTableCondition(TableConfiguration tables, string tableName, string table, out TableConfiguration config)
        {
            if (tableName.Contains(","))
            {
                List<string> tableList = tableName.Split(',').ToList();
                if (tableList.Contains(table))
                {
                    config = tables;
                    return true;
                }
            }
            config = new TableConfiguration();
            return false;
        }

        private void AddAccessedColumns(List<string> accessedColumns, int rowNumber, DataTable sensitiveColumnTable, int tableId)
        {
            DataRow dr;

            foreach (string column in accessedColumns)
            {
                dr = sensitiveColumnTable.NewRow();
                dr["startTime"] = startTime;
                dr["eventId"] = rowNumber;
                dr["columnName"] = column;
                dr["hashcode"] = 0;  //we can't create the hashcode here because the eventId is still unknown
                dr["tableId"] = tableId;
                dr["columnId"] = NativeMethods.GetHashCode(column);
                sensitiveColumnTable.Rows.Add(dr);
                jobInfo.UpdateColumnIdCache(column, NativeMethods.GetHashCode(column));
            }
        }

        private void WriteSensitiveColumEvents(int eventsToBeWritten, DataTable eventsToBeInsertedTable, DataTable sensitiveColumnsTable)
        {
            Repository writeRepository = new Repository();
            Repository readRepository = new Repository();

            int columnRowIndex = 0;
            int currentBatch = 0;
            int totalRows = 0;
            int eventId = 0;
            int newWatermark;
            SqlTransaction writeTrans = null;

            //Before proceding check if the Max(eventId), highWatermark,lowWatermark and alertHighWatermark are in sync if not sync it
            EventId.SyncWatermarks(jobInfo.instance, jobInfo.eventDatabase);

            currentId = EventId.GetNextIdBlock(jobInfo.instance, eventsToBeWritten);
            newWatermark = currentId + eventsToBeWritten - 1;

            try
            {
                using (DataTable eventsTable = new DataTable())
                {
                    using (DataTable eventSQLTable = new DataTable())
                    {
                        writeRepository.OpenConnection(jobInfo.eventDatabase);
                        readRepository.OpenConnection(jobInfo.eventDatabase);
                        InitEventsTableColumns(eventsTable, readRepository.connection);
                        InitEventSqlTableColumns(eventSQLTable, readRepository.connection);
                        writeTrans = writeRepository.connection.BeginTransaction(IsolationLevel.ReadUncommitted);

                        foreach (DataRow row in eventsToBeInsertedTable.Rows)
                        {
                            if (row == null)
                                continue;
                            if ((int)row[ndxDeleteme] == 1)
                                continue;

                            //Checking if SQL contains DML but has Event Type and Category of SELECT, then ignore it. //By Hemant
                            var txtData = row[ndxTextData].ToString().ToUpper();
                            ///Commented out for SQLCM-5471 //SQLCM-5471 - v5.6
                            //try
                            //{
                            //    if ((int)row[ndxEventType] == 1
                            //        && (int)row[ndxEventCategory] == 5
                            //        && (txtData.StartsWith("UPDATE") || txtData.StartsWith("INSERT") || txtData.StartsWith("DELETE")))
                            //        continue;
                            //}
                            //catch (Exception)
                            //{
                            //}
                            ////////////

                            rowsWritten++;

                            if (rowsWritten <= jobInfo.lastFinalEventProcessed)
                                continue;
                            currentBatch++;

                            if (currentBatch % insertBatchSize == 0)
                            {
                                writeTrans = InsertSensitiveColumnChanges(writeRepository.connection,
                                                                          writeTrans,
                                                                          eventsTable,
                                                                          eventSQLTable,
                                                                          sensitiveColumnsTable);

                                totalRows += currentBatch;
                                currentBatch = 0;

                                if (jobInfo.GetAborting())
                                    return;

                                writeTrans = writeRepository.connection.BeginTransaction(IsolationLevel.ReadUncommitted);
                            }
                            EventRecord er = AddEventRow(eventsTable, row);
                            jobInfo.UpdateIdCache(er);
                            eventId = er.eventId;
                            AddStats(er);
                            currentId++; //bump current event Id

                            //Add the SQL Text
                            //SQLCM-5471 - v5.6
                            AddSqlTableRowForSensitiveColumns(row, eventId, eventSQLTable);

                            //Add the sensitive Columns for the current event, there will always be at least one
                            UpdateSensitiveColumns(columnRowIndex++, eventId, sensitiveColumnsTable);
                        }
                        AddEventReceivedStats(eventsToBeInsertedTable.Rows.Count);
                        writeTrans = InsertSensitiveColumnChanges(writeRepository.connection,
                                                                  writeTrans,
                                                                  eventsTable,
                                                                  eventSQLTable,
                                                                  sensitiveColumnsTable);

                        totalRows += currentBatch;
                        jobInfo.UpdateIdTables(writeRepository.connection);

                    }//End of using eventSQLTbl
                }// End of using eventsTbl

                // summary of what we did
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, String.Format(CoreConstants.Debug_NewEventsCommitted,
                                                                            jobInfo.instance,
                                                                            jobInfo.traceFile,
                                                                            eventsToBeWritten,
                                                                            jobInfo.eventsInserted,
                                                                            eventId,
                                                                            newWatermark));
            }
            finally
            {
                if (writeTrans != null)
                {
                    rowsWritten -= currentBatch;
                    ErrorLog.Instance.Write(ErrorLog.Level.Verbose, String.Format("Transaction rollback: at rowsWritten: {0}, totalRows: {1}, currentBatch: {2}",
                                                                                  rowsWritten,
                                                                                  totalRows,
                                                                                  currentBatch));
                    writeTrans.Rollback();
                }
                writeRepository.CloseConnection();
                readRepository.CloseConnection();
            }
            EventId.UpdateStatus(jobInfo.instance, currentId - 1);
        }

        //
        // Add a new row to the eventSqlTable for insertion.
        //
        private void UpdateSensitiveColumns(int rowIndex, int eventId, DataTable table)
        {
            foreach (DataRow row in table.Rows)
            {
                if ((int)row["eventId"] != rowIndex)
                    continue;

                row["eventId"] = eventId;
                row["hashcode"] = CreateSensitiveColumnRecord(row).GetHashCode();
            }
        }

        protected SensitiveColumnRecord CreateSensitiveColumnRecord(DataRow row)
        {
            SensitiveColumnRecord scr = new SensitiveColumnRecord();

            //only set the columns that are used to create the hash
            scr.startTime = GetRowDateTime(row, "startTime");
            scr.eventId = GetRowInt32(row, "eventId");
            scr.columnName = GetRowString(row, "columnName");
            return scr;
        }

        //----------------------------------------------------------------------
        // CommitChanges(): insert the processed records, update the stats table 
        // and commit the transaction.
        // Note: The transaction object returned is always a null.
        //---------------------------------------------------------------------
        private SqlTransaction InsertSensitiveColumnChanges(SqlConnection conn,
                                                            SqlTransaction trans,
                                                            DataTable events,
                                                            DataTable eventSql,
                                                            DataTable sensitiveColumns)
        {
            if (trans != null)
            {
                jobInfo.eventsInserted += events.Rows.Count;
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, String.Format("Transaction commit: at rowsWritten: {0}", rowsWritten));
                UpdateStats(conn, trans);

                BulkInsert(conn, trans, CoreConstants.RepositoryEventsTable, events);
                BulkInsert(conn, trans, CoreConstants.RepositoryEventSqlTable, eventSql);
                BulkInsert(conn, trans, CoreConstants.RepositorySensitiveColumnsTable, sensitiveColumns);

                jobInfo.UpdateRowsProcessed(rowsWritten, conn, trans);
                trans.Commit();
                jobInfo.UpdateTableAndColumnIdTables(conn); // Keep this outside of the transaction
            }
            return null;
        }

        private static void InitSensitiveColumnTableColumns(DataTable table, SqlConnection conn)
        {
            InitTableColumns(table, CoreConstants.RepositorySensitiveColumnsTable, sensitiveColumnCols, conn);
        }

        #endregion

        #region server default trace

        private T ReadData<T>(DataRow reader, string columnName)
        {
            if (DBNull.Value.Equals(reader[columnName]))
                return default(T);

            if (typeof(T) == typeof(DateTime))
                return (T)reader[columnName];

            return (T)Convert.ChangeType(reader[columnName], typeof(T));
        }

        private void ProcessDefaultTrace()
        {
            LoadDataSet();

            var table = tempEventsDataSet.Tables["Events"];
            var repository = new Repository();
            SqlTransaction writeTrans = null;

            try
            {
                repository.OpenConnection(jobInfo.eventDatabase);
                writeTrans = repository.connection.BeginTransaction(IsolationLevel.ReadUncommitted);

                // The first event is usually an invalid event with null as the StartTime
                var i = table.Rows.Count - 1;
                while (jobInfo.lastEventTime == DateTime.MinValue && i >= 0)
                {
                    jobInfo.lastEventTime = GetRowDateTime(table.Rows[i], "StartTime");
                    i--;
                }

                var getFirstEventTime = true;
                foreach (DataRow row in table.Rows)
                {
                    if (row == null)
                        continue;

                    if (getFirstEventTime && !row.IsNull("StartTime"))
                    {
                        jobInfo.firstEventTime = GetRowDateTime(row, "StartTime");
                        getFirstEventTime = false;
                    }

                    if (ReadData<int>(row, "EventClass") != (int)TraceEventId.ServiceControl)
                        continue;

                    if (jobInfo.GetAborting())
                        return;

                    row["eventType"] = ReadData<int>(row, "eventSubclass") == 1 ? (int)TraceEventType.ServerStop : (int)TraceEventType.ServerStart;
                    row["eventCategory"] = (int)TraceEventCategory.Server;
                    row["StartTime"] = TimeZoneInfo.ToUniversalTime(jobInfo.timeZoneInfo, GetRowDateTime(row, "StartTime"));

                    currentId = EventId.GetNextId(jobInfo.instance);
                    var eventRecord = CreateEventRecord(repository.connection, row);
                    eventRecord.Insert(repository.connection, jobInfo.eventDatabase, writeTrans);
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         String.Format(
                                            "An error occurred processing {0} server's default trace file {1}.  Error: {2}.",
                                            jobInfo.instance,
                                            jobInfo.traceFile,
                                            e.Message));
                if (writeTrans != null)
                    writeTrans.Rollback();

                throw;
            }
            finally
            {
                if (writeTrans != null)
                    writeTrans.Commit();

                repository.CloseConnection();
            }
        }

        #endregion

        #region Match rows
        //----------------------------------------------------------------------------
        // Match rows - basic check that two events shoudl be correlated
        //----------------------------------------------------------------------------
        private bool
            RowsMatched(
            DataRow prevRow,
            DataRow currentRow)
        {
            string prevSqlText = GetRowString(prevRow, ndxTextData).Trim().ToUpper();
            string currentSQLText = GetRowString(currentRow, ndxTextData).Trim().ToUpper();
            //Comparing that previous and current events' SQL Text starting with same text
            //Checking length for the string is 4 because in EXEC, Select, Update, Insert, and Delete events, EXEC has minimum number of characters which is 4.
            if (!(prevSqlText.Length == 0 && currentSQLText.Length == 0))
            {
                if (!(currentSQLText.Length >= 4 && prevSqlText.Length >= 4
                    && currentSQLText.Substring(0, 4) == prevSqlText.Substring(0, 4)
                    && currentSQLText.Contains(GetRowString(prevRow, ndxObjectName).ToUpper()))
                    && DateTime.Compare(GetRowDateTime(currentRow, ndxStartTime), GetRowDateTime(prevRow, ndxStartTime)) > 0
                    && GetRowInt64(currentRow, ndxEndSequence) < GetRowInt64(prevRow, ndxEventSequence))
                {
                    return false;
                }
            }
            if (GetRowInt32(prevRow, ndxNestLevel) != GetRowInt32(currentRow, ndxNestLevel)
                || GetRowInt32(prevRow, ndxSpid) != GetRowInt32(currentRow, ndxSpid)
                || GetRowString(prevRow, ndxHostName) != GetRowString(currentRow, ndxHostName)
                || GetRowString(prevRow, ndxServerName) != GetRowString(currentRow, ndxServerName)
                || GetRowString(prevRow, ndxApplicationName) != GetRowString(currentRow, ndxApplicationName))
            {
                return false;
            }

            return true;
        }
        #endregion

        #region Get Databases List

        /// <summary>
        /// GetDatabaseListWithId
        /// </summary>
        /// <returns></returns>
        private Dictionary<int, string> GetDatabaseListWithId()
        {
            Dictionary<int, string> dbListDictionary = new Dictionary<int, string>();
            Repository repo = null;
            try
            {
                repo = new Repository();
                repo.OpenConnection(jobInfo.eventDatabase);
                string sqlText = string.Format("SELECT dbId,databaseName FROM {0}",
                                CoreConstants.RepositoryDatabaseTable);
                using (SqlCommand command = new SqlCommand(sqlText, repo.connection))
                {
                    command.CommandTimeout = 5;
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        dbListDictionary.Add(reader.GetInt16(0), reader.GetString(1));
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                            String.Format(
                                            "An error occurred while retrieving database details  Error: {0}.",
                                            e.Message));
            }
            finally
            {
                if (repo != null)
                    repo.CloseConnection();
            }
            return dbListDictionary;
        }

        /// <summary>
        /// Update rowcounts details in Events table
        /// </summary>
        private void UpdateRowCounts(DataRow row)
        {
            // skipping this method for SQLCM-5491 for SQL CM - 5.5.1 version. This is a temporary fix.
            return;
            if (!IsDMLorSelect(row))
                return;
            Repository repo = null;
            DateTime startTime = TimeZoneInfo.ToUniversalTime(jobInfo.timeZoneInfo, GetRowDateTime(row, ndxStartTime));   
            try
            {
                repo = new Repository();
                repo.OpenConnection(jobInfo.eventDatabase);
                string whereCluase = String.Format(" e.rowCounts IS NULL and e.eventType = {0} and e.eventCategory = {1} and e.spid = {2} and e.databaseId = {3} \n" +
                                                    "and e.applicationName = ''{4}'' and e.hostName = ''{5}'' and e.serverName = ''{6}'' and e.objectName <> '''' \n"+
                                                    "and e.startTime >= {7} and e.startSequence < {8}", GetRowInt32(row, ndxEventType) 
                                                                            ,GetRowInt32(row, ndxEventCategory)
                                                                            ,GetRowInt32(row, ndxSpid)
                                                                            ,GetRowInt32(row, ndxDatabaseId)
                                                                            ,GetRowString(row, ndxApplicationName)
                                                                            ,GetRowString(row, ndxHostName)
                                                                            ,GetRowString(row, ndxServerName)
                                                                            , String.Format("CONVERT(DATETIME, ''{0}'',121)",
                                                                            startTime.ToString("yyyy-M-d HH:mm:ss.fff",
                                                                            CultureInfo.InvariantCulture))
                                                                            ,GetRowInt64(row,ndxEventSequence));
                string query = EventRecord.GetSelectSQL(jobInfo.eventDatabase,
                                         CoreConstants.RepositoryEventsTable,
                                         whereCluase,
                                         "ORDER BY e.startTime ASC",
                                         true);
                query = query.Replace("'SELECT", "'SELECT TOP 1");
                using (SqlDataAdapter da = new SqlDataAdapter(query, repo.connection))
                {
                    da.SelectCommand.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    using (DataTable table = new DataTable())
                    {
                        da.Fill(table);
                        if (table.Rows.Count > 0)
                        {
                            EventRecord er = CreateEventRecord(table.Rows[0]);
                            er.rowCounts = GetRowInt64(row, ndxRowCounts);
                            er.checksum = er.GetHashCode();
                            er.hash = chain.GetHashCode(er.eventId + 1, er.checksum);

                            string updateQuery = "UPDATE Events SET hash = @hash,checksum = @checksum,rowCounts = @rowCount where eventId = @eventId;";
                            updateQuery += "UPDATE EventSQL SET hash = @hash, sqlText = @sqlText where eventId = @eventId;";
                            using (SqlCommand cmd = new SqlCommand(updateQuery, repo.connection))
                            {
                                cmd.Parameters.Add("@hash",SqlDbType.Int);
                                cmd.Parameters["@hash"].Value = er.hash;
                                cmd.Parameters.Add("@checksum", SqlDbType.Int);
                                cmd.Parameters["@checksum"].Value = er.checksum;
                                cmd.Parameters.Add("@rowCount", SqlDbType.BigInt);
                                cmd.Parameters["@rowCount"].Value = er.rowCounts;
                                cmd.Parameters.Add("@sqlText", SqlDbType.NVarChar);
                                cmd.Parameters["@sqlText"].Value = row[ndxTextData];
                                cmd.Parameters.Add("@eventId", SqlDbType.Int);
                                cmd.Parameters["@eventId"].Value = er.eventId;
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                            String.Format(
                                            "An error occurred while updating the Events table.  Error: {0}.",
                                            ex.Message));
            }
            finally
            {
                if (repo != null)
                    repo.CloseConnection();
            }
            
        }

        private bool IsDMLorSelect(DataRow row)
        {
            string txtData = row[ndxTextData].ToString();
            if (txtData != null && !txtData.Equals(""))
            {

                txtData = txtData.ToUpper();
                try
                {
                    if (txtData.StartsWith("SELECT"))
                    {
                        row[ndxEventCategory] = TraceEventCategory.SELECT;
                        row[ndxEventType] = TraceEventType.SELECT;
                        row[ndxPermissions] = 1;
                        return true;
                    }
                    if (IsDMLSqlStatement(txtData) || (txtData.StartsWith("EXEC") && !txtData.StartsWith("EXECUTE AS")))
                    {
                        row[ndxEventCategory] = TraceEventCategory.DML;
                        if (txtData.StartsWith("DELETE"))
                        {
                            row[ndxEventType] = TraceEventType.DELETE;
                            row[ndxPermissions] = 16;
                        }
                        else if (txtData.StartsWith("INSERT"))
                        {
                            row[ndxEventType] = TraceEventType.INSERT;
                            row[ndxPermissions] = 8;
                        }
                        else if (txtData.StartsWith("UPDATE"))
                        {
                            row[ndxEventType] = TraceEventType.UPDATE;
                            row[ndxPermissions] = 2;
                        }
                        else if (txtData.StartsWith("EXEC") && !txtData.StartsWith("EXECUTE AS"))
                        {
                            row[ndxEventType] = TraceEventType.EXECUTE;
                            row[ndxPermissions] = 32;
                        }
                        return true;
                    }
                }
                catch 
                { return false; }
            }
            return false;
        }

        protected EventRecord CreateEventRecord(DataRow row)
        {
            EventRecord er = new EventRecord();

            er.eventId = GetRowInt32(row, "eventId");
            er.eventType = (TraceEventType)GetRowInt32(row, "eventType");
            er.eventCategory = (TraceEventCategory)GetRowInt32(row, "eventCategory");
            er.targetObject = GetRowString(row, "targetObject");
            er.details = GetRowString(row, "details");
            er.hash = GetRowInt32(row, "hash");
            er.eventClass = GetRowInt32(row, "eventClass");
            er.eventSubclass = GetRowInt32(row, "eventSubclass");
            er.startTime = GetRowDateTime(row, "startTime");
            er.spid = GetRowInt32(row, "spid");
            er.applicationName = GetRowString(row, "applicationName");
            er.hostName = GetRowString(row, "hostName");
            er.serverName = GetRowString(row, "serverName").TrimEnd(null);
            er.loginName = GetRowString(row, "loginName");
            er.success = GetRowInt32(row, "success");
            er.databaseName = GetRowString(row, "databaseName");
            er.databaseId = GetRowInt32(row, "databaseId");
            er.dbUserName = GetRowString(row, "dbUserName");
            er.objectType = GetRowInt32(row, "objectType");
            er.objectName = GetRowString(row, "objectName");
            er.objectId = GetRowInt32(row, "objectId");
            er.permissions = GetRowInt32(row, "permissions");
            er.columnPermissions = GetRowInt32(row, "columnPermissions");
            er.targetLoginName = GetRowString(row, "targetLoginName");
            er.targetUserName = GetRowString(row, "targetUserName");
            er.roleName = GetRowString(row, "roleName");
            er.ownerName = GetRowString(row, "ownerName");
            er.alertLevel = GetRowInt32(row, "alertLevel");
            er.checksum = GetRowInt32(row, "checksum");
            er.privilegedUser = GetRowInt32(row, "privilegedUser");
            er.fileName = GetRowString(row, "fileName");
            er.linkedServerName = GetRowString(row, "linkedServerName");
            er.parentName = GetRowString(row, "parentName");
            er.isSystem = GetRowInt32(row, "isSystem");
            er.sessionLoginName = GetRowString(row, "sessionLoginName");
            er.providerName = GetRowString(row, "providerName");
            er.appNameId = GetRowInt32(row, "appNameId");
            er.hostId = GetRowInt32(row, "hostId"); 
            er.loginId = GetRowInt32(row, "loginId");
            er.endTime = GetRowDateTime(row, "endTime");
            er.startSequence = GetRowInt64(row, "startSequence");
            er.endSequence = GetRowInt64(row, "endSequence");
            return er;
        }

        #endregion

        //-----------------------------------------------------------------------
        // DeleteTempTables - routine used by job harness to cleanup
        //
        // Note: If this fails, life goes on, We leave the temp tables and
        //       clean up in some regular grooming job
        //-----------------------------------------------------------------------
        public bool
            IsRowCountEnabled(
            string instance
            )
        {
            Repository rep = new Repository();
            bool isRowCountEnabled = false;
            try
            {
                rep.OpenConnection(CoreConstants.RepositoryDatabase);

                try
                {
                    string sqlText;
                    SqlCommand cmd;

                    sqlText = GetIsRowCountFlag(instance);
                    using (cmd = new SqlCommand(sqlText, rep.connection))
                    {
                        cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                        isRowCountEnabled =  Convert.ToBoolean(cmd.ExecuteScalar());
                    }
                }
                catch (Exception ex)
                { }
                
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write("SQLcompliance::RowCountEnable",
                                         CoreConstants.Exception_RepositoryNotAvailable,
                                         ex);
            }
            finally
            {
                rep.CloseConnection();                
            }
            return isRowCountEnabled;
        }

        //------------------------------------------------------------------
        // GetDropSQL - SQL to drop temporary table after we are through
        //------------------------------------------------------------------
        private string
            GetIsRowCountFlag(
            string instance
            )
        {
            string sqlTemp = "SELECT isRowCountEnabled FROM [Servers] WHERE instance = {0}";

            return String.Format(sqlTemp, SQLHelpers.CreateSafeString(instance));
        }
    }
}
