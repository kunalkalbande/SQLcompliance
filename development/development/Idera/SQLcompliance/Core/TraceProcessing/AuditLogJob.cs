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
using System.Net;

namespace Idera.SQLcompliance.Core.TraceProcessing
{
    ///<summary>
    ///Summary description for AuditLogJob.
    ///</summary>
    internal class AuditLogJob : TraceRow
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
        internal const int state_Finalize = 5; // write the current stateRecord
        internal const int state_Skip = 6; // dont write the current stateRecord


        // Mapping table for SQL 2005 event types to SQLcompliance event type offsets


        // There could be gaps in tracing, we wont correlate events 
        // who dont fall within a minute of each other
        internal const int max_CorrelateTime = 2;

        internal int[,] traceEventCount = new int[200, 4];
        private static object lookupLock = new object();
        private static Hashtable typeOffsetTable = new Hashtable();
        private static List<DataColumn> eventsCols = new List<DataColumn>();
        private static List<DataColumn> eventSqlCols = new List<DataColumn>();
        private static List<DataColumn> dataChangeCols = new List<DataColumn>();
        private static List<DataColumn> colChangeCols = new List<DataColumn>();
        private static List<DataColumn> sensitiveColumnCols = new List<DataColumn>();
        internal static Dictionary<string, string> HostIPDictionary = new Dictionary<string, string>();

        private int rowsWritten = 0;
        private int insertBatchSize = 5000;

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
        private static Object auditLogFileReadLock = new Object();

        private HashChain chain;

        private string eventsTable;
        private int currentId;

        DataSet tempEventsDataSet;
        DataTable tmpTable; // Used by data change trace

        // input parameters
        internal TraceJobInfo jobInfo;

        bool processCurrent = false;

        LoginFilter loginCache = null;

        TimeZoneInfo timeZoneInfo = new TimeZoneInfo();

        #endregion

        #region Constructor

        protected
            AuditLogJob()
        {
            jobInfo = null;

            eventsTable = null;
        }

        public
            AuditLogJob(
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
                                         "Start AuditLogJob:Start Phase 3");

                goodEvents = ProcessRows();
                stage2 = Environment.TickCount;
                if (jobInfo.GetAborting())
                {
                    aborting = true;
                    return;
                }

                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                         "Start AuditLogJob:Start Phase 4");


                if (goodEvents != 0)
                {
                    lock (AcquireInstanceLock(jobInfo.instance))
                    {
                        WriteFinalEventTable(goodEvents,
                                              tempEventsDataSet.Tables["Events"]);
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
                                            "Audit Log: {0}\n  Events: {1}\n  Uncompress time: {2} ms\n  Read time: {3} ms\n  Process time: {4} ms\n  Write time: {5} ms",
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
                ErrorLog.Instance.Write("AuditLogJob::Start",
                                         String.Format(
                                            CoreConstants.Exception_ErrorProcessingAuditLogFile,
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
                try
                {
                    // Try Populating Events DataSet
                    PopulateEventsDataSet(rep, sqlText);
                }
                catch (Exception innerException)
                {
                    // Existing behaviour in 5.6: Modified the SQL to have null application and host name fields
                    sqlText = GetLoadSQLModified(jobInfo.traceFile);
                    PopulateEventsDataSet(rep, sqlText);
                }
                //-------------------------------------------------------            
                // Add columns to loaded DataRow for processing purposes
                //-------------------------------------------------------            
                DataTable table = tempEventsDataSet.Tables["Events"];

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
                                            CoreConstants.Exception_CantLoadAuditLogFile,
                                            jobInfo.instance),
                                         ex);
                throw;
            }
            finally
            {
                rep.CloseConnection();
            }
        }

        /// <summary>
        /// Populate Events DataSet for AuditLog events based on the input SQL Text
        /// </summary>
        private void PopulateEventsDataSet(Repository rep, string sqlText)
        {
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
                    tempEventsDataSet = new DataSet("AuditLog");

                    try
                    {
                        //lock (auditLogFileReadLock)
                        {
                            dataAdapter.Fill(tempEventsDataSet);
                        }
                    }
                    catch (SqlException sqlEx)
                    {
                        if (sqlEx.Number == 567)
                        {
                            // cant read file or it dosnt exist - usually caused by SQl Server not having permission to read files in that directory
                            string msg =
                               String.Format(CoreConstants.Exception_CantReadAuditLogFile,
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

                //Get database list with their Ids
                Dictionary<string, int> agentDbListWithId = GetDatabaseListWithId();

                //---------------      
                // Walk the Rows
                //---------------      
                int rowIndex = -1;
                int dataChangeRowIndex = -1;
                foreach (DataRow row in tempEventsDataSet.Tables[0].Rows)
                {
                    rowIndex++;
                    if (row == null) continue;

                    //row count feature is not supporeted for Audit Logs
                    row[ndxRowCounts] = DBNull.Value;

                    //extracting file name from full path because size of file name column in table is 128
                    row[ndxFileName] = Path.GetFileName(GetRowString(row, ndxFileName));

                    string hostIP = GetRowString(row, ndxHostName);


                    //Audit log does not provide host name so retrieving hostname from Host IP
                    if (HostIPDictionary.ContainsKey(hostIP))
                    {
                        row[ndxHostName] = HostIPDictionary[hostIP];
                    }
                    else
                    {
                        if (hostIP == "local machine")
                        {
                            HostIPDictionary[hostIP] = Environment.MachineName;
                            row[ndxHostName] = HostIPDictionary[hostIP];
                        }
                        else
                        {
                            try
                            {
                                IPHostEntry hostEntry = Dns.GetHostEntry(hostIP);
                                if (hostEntry != null)
                                {
                                    HostIPDictionary[hostIP] = hostEntry.HostName.Split('.')[0];
                                    row[ndxHostName] = HostIPDictionary[hostIP];
                                }
                            }
                            catch
                            {
                                //Used IP instead of Host name in case of any error
                            }
                        }
                    }

                    string databaseName = GetRowString(row, ndxDatabaseName);
                    if (agentDbListWithId.ContainsKey(databaseName))
                        row[ndxDatabaseId] = agentDbListWithId[databaseName];

                    // skip new rows added during state processing 
                    if (1 == GetRowInt32(row, ndxProcessed)) continue;

                    // mark all rows that get here deleted until we know better 
                    row[ndxDeleteme] = 1;

                    if (IsDataChangeEndEvent(row) && dataChangeRowIndex > -1)
                    {
                        tempEventsDataSet.Tables[0].Rows[dataChangeRowIndex][ndxStartSequence] = tempEventsDataSet.Tables[0].Rows[dataChangeRowIndex][ndxEventSequence];
                        tempEventsDataSet.Tables[0].Rows[dataChangeRowIndex][ndxEndSequence] = 0;
                    }

                    if (GetRowString(row, ndxObjectName) == CoreConstants.Agent_BeforeAfter_TableName
                        && GetRowString(row, ndxParentName) == CoreConstants.Agent_BeforeAfter_SchemaName)
                        continue;

                    if (IsDataChangeEvent(row))
                        dataChangeRowIndex = rowIndex;
                    else
                        dataChangeRowIndex = -1;

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

                    if (processCurrent)
                    {
                        if (FinalizeRow(row)) goodRows++;
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
                ErrorLog.Instance.Write("AuditLogJob::ProcessRows",
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

            // check filter?
            if (TraceFilter.RowMatchesEventFilter(new TraceJob(jobInfo), row))
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

        #endregion


        #region Row Validity Check

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
                ErrorLog.Instance.Write("AuditLogJob::RowMatchesExcludeFilter",
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

            if (jobInfo.traceCategory == (int)TraceCategory.DataChange
                || jobInfo.traceCategory == (int)TraceCategory.DataChangeWithDetails)
            {
                tmpTable = new DataTable();
                InitializeDataChangeColumns(tmpTable);
            }

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
                        int processedRow = 0;
                        foreach (DataRow row in table.Rows)
                        {
                            processedRow++;
                            if (row == null) continue;

                            if (GetRowString(row, ndxObjectName) == CoreConstants.Agent_BeforeAfter_TableName
                                && GetRowString(row, ndxParentName) == CoreConstants.Agent_BeforeAfter_SchemaName
                                && GetRowInt32(row, ndxPermissions) == 1)
                            {
                                row[ndxEventSequence] = processedRow;
                                CreateDataChangeRow(tmpTable, row, eventId);
                            }

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

                            EventRecord er = AddEventRow(eventsTbl, row);

                            jobInfo.UpdateIdCache(er);
                            eventId = er.eventId;
                            AddStats(er);
                            currentId++;
                            if (jobInfo.keepingSql || (GetRowInt32(row, ndxKeepSql) == ON))
                            {
                                AddSqlTableRow(row, eventId, eventSQLTbl);
                            }
                        }

                        AddEventReceivedStats(table.Rows.Count);
                        CommitChanges(writeRepository.connection,
                                                    writeTrans,
                                                    eventsTbl,
                                                    eventSQLTbl);
                        if (tmpTable != null && tmpTable.Rows.Count > 0)
                        {
                            ProcessDataChangeTrace(writeTrans, writeRepository.connection);
                            UpdateDataChangeIds(writeRepository.connection);
                        }
                        else
                        {
                            writeTrans.Commit();
                        }
                        writeTrans = null;
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

        private void InitializeDataChangeColumns(DataTable table)
        {
            table.Columns.Add("EventClass", Type.GetType("System.Int32"));
            table.Columns.Add("StartTime", Type.GetType("System.DateTime"));
            table.Columns.Add("SPID", Type.GetType("System.Int32"));
            table.Columns.Add("EventSequence", Type.GetType("System.Int64"));
            table.Columns.Add("DatabaseID", Type.GetType("System.Int32"));
            table.Columns.Add("DatabaseName", Type.GetType("System.String"));
            table.Columns.Add("TextData", Type.GetType("System.String"));
            table.Columns.Add("EventID", Type.GetType("System.Int32"));
        }

        private void CreateDataChangeRow(DataTable table, DataRow row, int eventId)
        {
            DataRow dataRow = tmpTable.NewRow();
            dataRow["EventClass"] = row[ndxEventClass];
            dataRow["StartTime"] = row[ndxStartTime];
            dataRow["SPID"] = row[ndxSpid];
            dataRow["EventSequence"] = row[ndxEventSequence];
            dataRow["DatabaseID"] = row[ndxDatabaseId];
            dataRow["DatabaseName"] = row[ndxDatabaseName];
            dataRow["TextData"] = row[ndxTextData];
            dataRow["EventID"] = eventId;
            table.Rows.Add(dataRow);
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

            foreach (KeyValuePair<int, DBAuditConfiguration> kvp in jobInfo.dbconfigs)
            {
                if (dbname == kvp.Value.Name)
                {
                    auditDBSettings = kvp.Value;
                    break;
                }
            }
            return auditDBSettings;
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

        #endregion


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
                // DML and Select   
                case (int)AuditLogEventID.SCHEMA_OBJECT_ACCESS_GROUP:
                    retval = AuditObjectPermissionEventHandler(row);
                    break;
                case (int)AuditLogEventID.TRANSACTION_GROUP:
                    retval = SQLTransaction(row);
                    break;
                default:
                    retval = false;
                    break;
            }
            if (retval)
            {
                if (jobInfo.privilegedUserTrace ||
                   (!row.IsNull(ndxPrivilegedUser) && Convert.ToInt32(row[ndxPrivilegedUser]) == 1))
                    row[ndxPrivilegedUser] = 1;
                else
                    row[ndxPrivilegedUser] = 0;
                row[ndxNewRow] = 0;
            }
            else
            {
                if (ErrorLog.Instance.ErrorLevel >= ErrorLog.Level.Debug)
                {
                    // if this is not a login event then it was an invalid event
                    // if it is a login, we just skipped because it was not unique
                    if (eventClass != (int)TraceEventId.Login || eventClass != (int)TraceEventId.Logout)
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
            if (eventCls == (int)TraceEventId.AuditObjectPermission || jobInfo.traceFile.ToLower().EndsWith(".xel"))  // DML or SELECt events
            {
                if (jobInfo.traceCategory == (int)TraceCategory.DBPrivilegedUsers)
                {
                    return IsValidDbPrivEvents(row);  // Check Validity of  DML and Select Events for DB Priviliged User
                }
                else
                {
                    return IsValidDbEvents(row); // Check Validity of DML and Select Events for DB Only
                }
            }
            else
            {
                return !jobInfo.privEvents.Contains(eventCls);
            }
        }

        internal bool
            IsServerPrivEvent(
            DataRow row
            )
        {
            if (!IsPrivUser(row)) return false;  // Not a priv user

            int eventCls = GetRowInt32(row, ndxEventClass);
            if (eventCls == (int)TraceEventId.AuditObjectPermission || jobInfo.traceFile.ToLower().EndsWith(".xel"))  // DML or SELECt events
            {
                return IsValidServerPrivEvents(row);  // Check Validity of DML and Select Events for Server Priviliged User
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
        /// <summary>
        /// Check File Preferences for DB Level 
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        internal bool IsValidDbEvents(DataRow row)
        {
            bool isDataChangeEvent = false;
            int dbId = GetRowInt32(row, ndxDatabaseId);
            if (jobInfo.dbconfigs == null && !jobInfo.dbconfigs.ContainsKey(dbId))
            {
                return false;
            }
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

        private string GetLoadSQL(string auditLogFile)
        {
            //Mapping fn_get_audit_file and trace_subclass_values to get object type
            string sqlStmt = @"SELECT 
                               CASE WHEN [action_id] in ('TXBG', 'TXCM', 'TXRB')
	                                THEN 50 
	                                WHEN [action_id] in ('SL','IN','UP','DL','RF','EX','RC')
	                                THEN 114 
	                                ELSE NULL END as EventClass,
                               CASE WHEN [action_id] = 'TXBG' THEN 0 
	                                WHEN [action_id] = 'TXCM' THEN 1 
	                                WHEN [action_id] = 'TXCM' THEN 2 
	                                ELSE NULL END as EventSubClass,
                               CONVERT(Datetime, [event_time]) as StartTime,
                               [session_id] as SPID,
                               [application_name] as ApplicationName, 
                               CONVERT(nvarchar(100),[client_ip]) as HostName, 
                               [server_instance_name] as ServerName,
                               [server_principal_name] as LoginName,
                               CONVERT(int,[succeeded]) as Success,
                               [database_name] as DatabaseName,
                               -1 as DatabaseID,
                               [database_principal_name] as DBUserName,
                               [subclass_value] as ObjectType,
                               [object_name] as ObjectName,
                               [object_id] as ObjectID,
                               CONVERT(int,[permission_bitmask]) as Permissions,
                               [is_column_permission] as ColumnPermissions,
                               [target_server_principal_name] as TargetLoginName,
                               NULL as TargetUserName,
                               NULL as RoleName,
                               NULL as OwnerName,
                               [statement] as TextData,
                               0 as NestLevel,
                               [file_name] as FileName,
                               NULL as LinkedServerName,
                               [schema_name] as ParentName,
                               0 as IsSystem,
                               [session_server_principal_name] as SessionLoginName,
                               NULL as ProviderName,
                               [sequence_number] as eventSequence,
                               [affected_rows] as RowCounts
                               from sys.fn_get_audit_file('{0}',default,default) auditLog join 
                               (SELECT DISTINCT subclass_name,subclass_value FROM sys.trace_subclass_values WHERE trace_column_id = 28) trace 
                               ON auditLog.class_type = trace.subclass_name";
            return String.Format(sqlStmt, auditLogFile);
        }

        /// <summary>
        /// Load Modified SQL String for reading AuditLogs with null application and host name
        /// </summary>
        /// <param name="auditLogFile"></param>
        /// <returns></returns>
        private string GetLoadSQLModified(string auditLogFile)
        {
            //Mapping fn_get_audit_file and trace_subclass_values to get object type
            string sqlStmt = @"SELECT 
                               CASE WHEN [action_id] in ('TXBG', 'TXCM', 'TXRB')
	                                THEN 50 
	                                WHEN [action_id] in ('SL','IN','UP','DL','RF','EX','RC')
	                                THEN 114 
	                                ELSE NULL END as EventClass,
                               CASE WHEN [action_id] = 'TXBG' THEN 0 
	                                WHEN [action_id] = 'TXCM' THEN 1 
	                                WHEN [action_id] = 'TXCM' THEN 2 
	                                ELSE NULL END as EventSubClass,
                               CONVERT(Datetime, [event_time]) as StartTime,
                               [session_id] as SPID,
                               NULL as ApplicationName, 
                               CONVERT(nvarchar(100),NULL) as HostName, 
                               [server_instance_name] as ServerName,
                               [server_principal_name] as LoginName,
                               CONVERT(int,[succeeded]) as Success,
                               [database_name] as DatabaseName,
                               -1 as DatabaseID,
                               [database_principal_name] as DBUserName,
                               [subclass_value] as ObjectType,
                               [object_name] as ObjectName,
                               [object_id] as ObjectID,
                               CONVERT(int,[permission_bitmask]) as Permissions,
                               [is_column_permission] as ColumnPermissions,
                               [target_server_principal_name] as TargetLoginName,
                               NULL as TargetUserName,
                               NULL as RoleName,
                               NULL as OwnerName,
                               [statement] as TextData,
                               0 as NestLevel,
                               [file_name] as FileName,
                               NULL as LinkedServerName,
                               [schema_name] as ParentName,
                               0 as IsSystem,
                               [session_server_principal_name] as SessionLoginName,
                               NULL as ProviderName,
                               [sequence_number] as eventSequence,
                               NULL as RowCounts
                               from sys.fn_get_audit_file('{0}',default,default) auditLog join 
                               (SELECT DISTINCT subclass_name,subclass_value FROM sys.trace_subclass_values WHERE trace_column_id = 28) trace 
                               ON auditLog.class_type = trace.subclass_name";
            return String.Format(sqlStmt, auditLogFile);
        }

        #endregion


        #region Data Change Trace (Before/after data)

        // Return true if records were inserted
        private bool ProcessDataChangeTrace(SqlTransaction writeTrans, SqlConnection connection)
        {
            // Only the first table in the dataset is being used
            if (tmpTable == null || tmpTable.Rows.Count == 0)
                return false;

            DataTable table = tmpTable;
            //Repository writeRepository = new Repository();
            Repository readRepository = new Repository();

            long start = Environment.TickCount;
            int currentBatch = 0;
            int totalRows = 0;
            bool getFirstEventTime = true;
            try
            {
                using (DataTable dataChangeTable = new DataTable())
                {
                    using (DataTable columnChangeTable = new DataTable())
                    {
                        //writeRepository.OpenConnection(jobInfo.eventDatabase);
                        readRepository.OpenConnection(jobInfo.eventDatabase);
                        InitDataChangesTableColumns(dataChangeTable, readRepository.connection);
                        InitColumnChangesTableColumns(columnChangeTable, readRepository.connection);

                        int i = table.Rows.Count;
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

                            //Some times dml events captured with SELECT(1) permission so skip the event
                            if (!SQLHelpers.GetRowString(row, "TextData").ToUpper().StartsWith("SELECT"))
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
                                writeTrans = InsertDataChangeRecords(
                                                            connection,
                                                            writeTrans,
                                                            dataChangeTable,
                                                            columnChangeTable);

                                totalRows += currentBatch;
                                currentBatch = 0;

                                // check aborting flag	                  
                                if (jobInfo.GetAborting())
                                    return false;
                            }

                            ProcessDataChangeRow(dataChangeTable, columnChangeTable, row);

                        }

                        writeTrans = InsertDataChangeRecords(
                                                    connection,
                                                    writeTrans,
                                                    dataChangeTable,
                                                    columnChangeTable);

                        totalRows += currentBatch;
                    } // End of using
                } // End of using


                if (jobInfo.eventsInserted > 0)
                {
                    AlertingJobPool.DoBADAlertProcessing = true;
                    AlertingJobPool.SetBADAlertDetails(jobInfo.instance, jobInfo.eventDatabase, jobInfo.eventsInserted);
                }
                return totalRows > 0;
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                         String.Format("Audit Log processing: {0} rows inserted and {1} rows skipped in {2}ms",
                                                        totalRows,
                                                        rowsWritten - totalRows,
                                                        Environment.TickCount - start));
                readRepository.CloseConnection();
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
            dcr.eventSequence = SQLHelpers.GetRowLong(row, "eventSequence");
            dr["eventSequence"] = dcr.eventSequence;
            dr["spid"] = dcr.spid = SQLHelpers.GetRowInt32(row, "SPID");
            dcr.databaseId = SQLHelpers.GetRowInt32(row, "DatabaseID");

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
            dr["hashcode"] = dcr.GetHashCode();
            dr["eventId"] = SQLHelpers.GetRowInt32(row, "EventID");
            dr["totalChanges"] = dcr.totalChanges;
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

        private static void InitDataChangesTableColumns(DataTable table, SqlConnection conn)
        {
            InitTableColumns(table, CoreConstants.RepositoryDataChangesTable, dataChangeCols, conn);
        }

        private static void InitColumnChangesTableColumns(DataTable table, SqlConnection conn)
        {
            InitTableColumns(table, CoreConstants.RepositoryColumnChangesTable, colChangeCols, conn);
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
                    ErrorLog.Instance.Write(ErrorLog.Level.Verbose, "Start AuditLogJob:ProcessSensitiveColumnTrace Phase 4");


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
                    ErrorLog.Instance.Write("AuditLogJob::ProcessSensitiveColumnTrace",
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


        private int ProcessSCRows(DataRowCollection rows, DataTable scTable)
        {
            if (rows.Count == 0)
                return 0;

            int rowCount = 0;
            bool preoverlap = true;
            bool overlapped = false;
            bool getFirstEventTime = true;
            TimesRecord lastTrace;

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

            //Get database list with their Ids
            Dictionary<string, int> agentDbListWithId = GetDatabaseListWithId();
            columnTablesConfig = GetDictAuditedColumns(jobInfo.instance);
            foreach (DataRow row in rows)
            {
                if (row == null)
                    continue;

                //row count feature is not supporeted for Audit Logs
                row[ndxRowCounts] = DBNull.Value;

                //extracting file name from full path because size of file name column in table is 128
                row[ndxFileName] = Path.GetFileName(GetRowString(row, ndxFileName));

                string hostIP = GetRowString(row, ndxHostName);


                //Audit log does not provide host name so retrieving hostname from Host IP
                if (HostIPDictionary.ContainsKey(hostIP))
                {
                    row[ndxHostName] = HostIPDictionary[hostIP];
                }
                else
                {
                    if (hostIP == "local machine")
                    {
                        HostIPDictionary[hostIP] = Environment.MachineName;
                        row[ndxHostName] = HostIPDictionary[hostIP];
                    }
                    else
                    {
                        try
                        {
                            IPHostEntry hostEntry = Dns.GetHostEntry(hostIP);
                            if (hostEntry != null)
                            {
                                HostIPDictionary[hostIP] = hostEntry.HostName.Split('.')[0];
                                row[ndxHostName] = HostIPDictionary[hostIP];
                            }
                        }
                        catch
                        {
                            //Used IP instead of Host name in case of any error
                        }
                    }
                }

                string databaseName = GetRowString(row, ndxDatabaseName);
                if (agentDbListWithId.ContainsKey(databaseName))
                    row[ndxDatabaseId] = agentDbListWithId[databaseName];


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

                if (SQLHelpers.GetRowInt32(row, "EventClass") == (int)TraceEventId.AuditObjectPermission)
                {
                    if (ProcessSensitiveColumnRow(row, rowCount, scTable))
                    {
                        rowCount++;
                        row[ndxDeleteme] = 0;
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
                                        FindAll(x => x.Name.Split(',').Contains(GetRowString(row, ndxParentName) + "." + GetRowString(row, ndxObjectName)));

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
                    if (TraceFilter.RowMatchesEventFilter(new TraceJob(jobInfo), row))
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

            //Parse the query and see if there is any sensitive column info.
            IList<ParseError> parseErrors;
            TSqlScript parsedScript = ColumnParser.ParseColumns(textData, out parseErrors);
            ColumnParser parser = new ColumnParser();
            int categeroy = jobInfo.traceCategory;// SQLCM-5471 v5.6 Add Activity to Senstitive columns
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

        private bool GetSensitiveColumns(DataRow row, out TableConfiguration tableConfig)
        {
            TableConfiguration config = new TableConfiguration();
            int dbId = GetRowInt32(row, ndxDatabaseId);

            if (jobInfo.scTableLists.ContainsKey(dbId))
            {
                Dictionary<string, TableConfiguration> tables = jobInfo.scTableLists[dbId];
                string table = GetRowString(row, ndxObjectName);
                if (!row.IsNull(ndxParentName))
                    table = GetRowString(row, ndxParentName) + "." + table;


                if (tables.TryGetValue(table, out config))
                {
                    tableConfig = config;
                    return true;
                }
            }
            tableConfig = config;
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
                            try
                            {
                                if ((int)row[ndxEventType] == 1
                                    && (int)row[ndxEventCategory] == 5
                                    && (txtData.StartsWith("UPDATE") || txtData.StartsWith("INSERT") || txtData.StartsWith("DELETE")))
                                    continue;
                            }
                            catch (Exception)
                            {
                            }
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
                            AddSqlTableRow(row, eventId, eventSQLTable);

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

        /// <summary>
        /// GetDatabaseListWithId
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, int> GetDatabaseListWithId()
        {
            Dictionary<string, int> dbListDictionary = new Dictionary<string, int>();
            Repository repo = null;
            try
            {
                repo = new Repository();
                repo.OpenConnection(jobInfo.eventDatabase);
                string sqlText = string.Format("SELECT databaseName,dbId FROM {0}",
                                CoreConstants.RepositoryDatabaseTable);
                using (SqlCommand command = new SqlCommand(sqlText, repo.connection))
                {
                    command.CommandTimeout = 5;
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        dbListDictionary.Add(reader.GetString(0), reader.GetInt16(1));
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
        /// UpdateDataChangeIds: Map dcId in ColumnChanges and DataChanges tables
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        private void UpdateDataChangeIds(SqlConnection conn)
        {
            try
            {
                string query = "UPDATE t1 set t1.dcId=t2.dcId FROM ColumnChanges t1 INNER JOIN DataChanges t2 " +
                               "ON (t1.startTime = t2.startTime AND t1.eventSequence = t2.eventSequence AND t1.spid = t2.spid)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    cmd.ExecuteNonQuery();
                }
                return;
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write("UpdateDataChangeIds failed.", e);
            }
        }
    }
}
