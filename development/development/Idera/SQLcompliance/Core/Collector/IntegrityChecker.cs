using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;

using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Agent;
using Idera.SQLcompliance.Core.Event;
using Idera.SQLcompliance.Core.TraceProcessing;
using Idera.SQLcompliance.Core.Stats;
using TraceEventType = Idera.SQLcompliance.Core.Event.TraceEventType;

namespace Idera.SQLcompliance.Core.Collector
{
    public enum IntegrityCheckAction
    {
        UseDefault = -1,
        SkipCheck = 0,
        PerformCheck = 1,
        CheckAlreadyDone = 2
    }

    internal enum RecordStatus : int
    {
        Intact = 0,
        RecordDeleted = 1,
        RecordAdded = 2,
        RecordModified = 3,
        HashCodeModified = 4
    }

    [Serializable]
    public class CheckResult : ISerializable
    {
        // V 1.1 and 1.2 fields
        public bool intact;
        public int numGaps;
        public int numAdded;
        public int numModified;
        public string integrityCheckError;
        public DateTime earliestTime;
        public DateTime latestTime;

        // V 2.0 fields
        internal int classVersion = CoreConstants.SerializationVersion;

        // v 3.6 fields
        public int numSCGaps;
        public int numSCModified;
        public int numDCGaps;
        public int numDCModified;
        public int numCCModified;
        public int numCCGaps;

        public CheckResult()
        {
            Init();
        }

        // Deserialization constructor
        public CheckResult(SerializationInfo info,
                           StreamingContext context)
        {
            try
            {
                // V 1.1 and 1.2 fields
                intact = info.GetBoolean("intact");
                numGaps = info.GetInt32("numGaps");
                numAdded = info.GetInt32("numAdded");
                numModified = info.GetInt32("numModified");
                integrityCheckError = info.GetString("integrityCheckError");

                // V 2.0 fields
                try
                {
                    classVersion = info.GetInt32("classVersion");
                }
                catch
                {
                    classVersion = 0;
                }

                if (classVersion >= CoreConstants.SerializationVersion_20)
                {
                    // These DateTime fiels are UTC times
                    earliestTime = new DateTime(info.GetInt64("earliestTicks"));
                    latestTime = new DateTime(info.GetInt64("latestTicks"));
                }
                else // special handling for 1.1 and 1.2 fields
                {
                    earliestTime = info.GetDateTime("earliestTime");
                    latestTime = info.GetDateTime("latestTime");
                }

                if (classVersion >= CoreConstants.SerializationVersion_36)
                {
                    //Sensitive Column and BAD tables
                    numSCGaps = info.GetInt32("numSCGaps");
                    numSCModified = info.GetInt32("numSCModified");
                    numDCGaps = info.GetInt32("numDCGaps");
                    numDCModified = info.GetInt32("numDCModified");
                    numCCModified = info.GetInt32("numCCModified");
                    numCCGaps = info.GetInt32("numCCGaps");
                }
                else
                {
                    numSCGaps = 0;
                    numSCModified = 0;
                    numDCGaps = 0;
                    numDCModified = 0;
                    numCCModified = 0;
                    numCCGaps = 0;
                }
                Debug.WriteLine(String.Format("{0} deserializaed", GetType()));
            }
            catch (Exception e)
            {
                SerializationHelper.ThrowDeserializationException(e, GetType());
            }
        }

        // Required custom serialization method
        public void GetObjectData(SerializationInfo info,
                                  StreamingContext context)
        {
            try
            {
                // 1.1 and 1.2 fields
                info.AddValue("intact", intact);
                info.AddValue("numGaps", numGaps);
                info.AddValue("numAdded", numAdded);
                info.AddValue("numModified", numModified);
                info.AddValue("integrityCheckError", integrityCheckError);

                // 2.0 fields
                if (classVersion >= CoreConstants.SerializationVersion_20)
                {
                    info.AddValue("classVersion", classVersion);
                    // Note that event times are UTC times
                    info.AddValue("earliestTicks", earliestTime.Ticks);
                    info.AddValue("latestTicks", latestTime.Ticks);
                }
                else
                {
                    info.AddValue("earliestTime", earliestTime);
                    info.AddValue("latestTime", latestTime);
                }

                if (classVersion >= CoreConstants.SerializationVersion_36)
                {
                    info.AddValue("numSCGaps", numSCGaps);
                    info.AddValue("numSCModified", numSCModified);
                    info.AddValue("numDCGaps", numDCGaps);
                    info.AddValue("numDCModified", numDCModified);
                    info.AddValue("numCCModified", numCCModified);
                    info.AddValue("numCCGaps", numCCGaps);
                }
                Debug.WriteLine(String.Format("{0} serialized.", GetType()));
            }
            catch (Exception e)
            {
                SerializationHelper.ThrowSerializationException(e, GetType());
            }
        }

        private void Init()
        {
            intact = true;
            numGaps = 0;
            numModified = 0;
            integrityCheckError = "";
            earliestTime = DateTime.MinValue;
            latestTime = DateTime.MinValue;
            numSCGaps = 0;
            numSCModified = 0;
            numDCGaps = 0;
            numDCModified = 0;
            numCCModified = 0;
            numCCGaps = 0;
        }
    }


    /// <summary>
    /// Summary description for IntegrityChecker.
    /// </summary>
    internal class IntegrityChecker : TraceJob
    {
        #region Private Member
        private string instanceName;
        private string database;
        private int highWatermark = -2100000000;
        private int lowWatermark = -2100000000;
        private int firstId;
        private int lastId;
        private int next;
        private bool hasNextId = false;
        private bool hasLowWatermark = false;
        private bool isArchive = false;
        private bool returnEventList = true;
        private HashChain chain;
        private ArrayList badRecords = new ArrayList();
        private ArrayList badEventList = new ArrayList();
        private ArrayList badEventTypeList = new ArrayList();
        private SqlDataAdapter da = null;
        private Hashtable columnMappings = new Hashtable();
        private Hashtable resultTable = new Hashtable();
        private const int checkBatchSize = 5000;
        private const int maxBadEventsReturned = 100;

        #endregion

        #region Properties
        public int HighWatermark
        {
            get { return highWatermark; }
            set
            {
                highWatermark = value;
            }
        }

        public int LowWatermark
        {
            get { return lowWatermark; }
            set
            {
                lowWatermark = value;
            }
        }

        #endregion

        #region Integrity Check

        //----------------------------------------------------------------------
        // GetCheckResult:  returns the result for one instance.
        //----------------------------------------------------------------------
        internal CheckResult GetCheckResult(string instance)
        {
            return (CheckResult)resultTable[instance];
        }

        //----------------------------------------------------------------------
        // GetCheckResults:  returns the results for all the instances.
        //----------------------------------------------------------------------
        internal Hashtable GetCheckResults()
        {
            return resultTable;
        }

        //----------------------------------------------------------------------
        // CheckIntegrity:  for all instances
        //----------------------------------------------------------------------
        public bool CheckIntegrity(bool fixProblems)
        {
            bool intact = true;
            string[] instanceList;
            CheckResult result;

            resultTable.Clear();

            try
            {
                instanceList = GetAllInstance();
            }
            catch (Exception e)
            {
                throw e;
            }

            if (instanceList == null)
            {
                return true;
            }

            for (int i = 0; i < instanceList.Length; i++)
            {
                result = CheckIntegrity(instanceList[i], fixProblems);
                resultTable.Add(instanceList[i], result);

                if (!result.intact)
                {
                    intact = false;
                }
            }
            return intact;
        }

        //----------------------------------------------------------------------
        // CheckIntegrity: 
        //----------------------------------------------------------------------
        public CheckResult CheckIntegrity(string instance, bool fixProblems)
        {
            isArchive = false;
            return CheckIntegrity(instance,
                                    GetInstanceDatabase(instance),
                                    false,
                                    fixProblems);
        }

        //----------------------------------------------------------------------
        // CheckIntegrity
        //----------------------------------------------------------------------
        public CheckResult CheckIntegrity(string instance,
                                          string inDatabase,
                                          bool inIsArchive,
                                          bool fixProblems)
        {
            int[] badEventTypes;
            EventRecord[] eventList;

            returnEventList = false;
            return CheckIntegrity(instance,
                                   inDatabase,
                                   inIsArchive,
                                   fixProblems,
                                   out eventList,
                                   out badEventTypes);
        }

        //----------------------------------------------------------------------
        // CheckIntegrity
        //----------------------------------------------------------------------
        public CheckResult CheckIntegrity(string instance,
                                          string inDatabase,
                                          bool inIsArchive,
                                          bool fixProblems,
                                          out EventRecord[] eventList,
                                          out int[] eventTypeList
           )
        {
            Repository rep = new Repository();
            try
            {
                rep.OpenConnection();

                if (!inIsArchive)
                {
                    // if this isnt an archive database, make sure server is still audited
                    if (!ServerRecord.ServerIsAudited(instance, rep.connection))
                    {
                        throw new Exception(String.Format(CoreConstants.Exception_ServerDeleted, instance));
                    }
                }

                // check schema version of file we are suppsoed to check
                if (!EventDatabase.IsCompatibleSchema(inDatabase, rep.connection))
                {
                    throw new Exception(String.Format(CoreConstants.Error_IntegrityCheckSchemaError, inDatabase));
                }
            }
            finally
            {
                rep.CloseConnection();
            }

            DataTable table = null;
            bool checkLowWatermark = true;
            bool badRecord;
            int totalRows = 0;
            int rowCount;
            int newLowWatermark = int.MaxValue;
            CheckResult result = new CheckResult();

            result.intact = true;
            result.numGaps = 0;
            result.numAdded = 0;
            result.numModified = 0;
            result.integrityCheckError = "";
            result.earliestTime = DateTime.MaxValue;
            result.latestTime = DateTime.MinValue;
            badEventList.Clear();
            badEventTypeList.Clear();

            try
            {
                instanceName = instance;
                database = inDatabase;
                isArchive = inIsArchive;

                // check if this database is in need of a hash Chain rebuild - this could happen if
                // an archive or groom was aborted without rebuilding (tempdb full or the like)
                EventsDatabaseState state = EventDatabase.GetDatabaseState(inDatabase);
                if (state == EventsDatabaseState.Busy || state == EventsDatabaseState.NormalChainBroken)
                {
                    RebuildChain(instance, database, isArchive);
                }

                // mark integrity attempt and set status = In Progress
                if (inIsArchive)
                    UpdateLastIntegrityCheckTimeArchive(database);
                else
                    UpdateLastIntegrityCheckTime(instance);

                GetWatermarks();
                // Special case: lowWatermark is null
                if (lowWatermark == -2100000000)
                    newLowWatermark = lowWatermark;
                firstId = GetFirstIdToCheck();
                chain = new HashChain(instanceName);

                do
                {
                    lastId = GetLastIdToCheck(highWatermark);
                    table = GetEventsTable();

                    if (table == null)
                    {
                        // GetEventsTable() should not return nulls.  
                        // It throws an exception when something is wrong.  Otherwise,
                        // it returns a valid table.
                        result.intact = false;
                        result.integrityCheckError = "Error retrieving records from database";
                        return result;
                    }
                    else if (table.Rows.Count == 0)
                    {
                        rowCount = 0;
                        continue;
                    }

                    DataRow row;
                    DataRow prevRow = null;
                    DataRow newRow;
                    EventRecord er = null;

                    badRecords.Clear();
                    rowCount = table.Rows.Count;
                    int newId;
                    bool needUpdate = false;

                    for (int i = 0; i < rowCount; i++)
                    {
                        row = table.Rows[i];
                        er = CreateEventRecord(row);
                        badRecord = false;
                        if (er.startTime < result.earliestTime)
                            result.earliestTime = er.startTime;
                        if (er.startTime > result.latestTime)
                            result.latestTime = er.startTime;

                        // Special case for the first row
                        if (checkLowWatermark)
                        {
                            if (er.eventId > lowWatermark + 1)
                            {
                                // missing events from the head
                                result.numGaps++;
                                result.intact = false;
                                checkLowWatermark = false;
                                string msg = String.Format("Integrity Check: Records missing from the head of the chain."
                                                           + "First event in the chain: {0}.\n"
                                                           + "Low watermark: {1}.",
                                                           er.eventId,
                                                           lowWatermark);
                                ErrorLog.Instance.Write(ErrorLog.Level.Debug, msg);

                                if (fixProblems)
                                {
                                    newRow = table.NewRow();
                                    InsertBadRecord(newRow, er.eventId, er.startTime);
                                    newLowWatermark = er.eventId - 2;
                                    prevRow = newRow;
                                    needUpdate = true;
                                }

                                if (returnEventList)
                                {
                                    AddToReturnList(CoreConstants.BadEventType_EventGap, CreateBadRecord(er.eventId, er.startTime));
                                }
                            }
                            else if (er.eventId == lowWatermark + 1)
                            {
                                checkLowWatermark = false;
                            }
                            else
                            {
                                // extra event at the head
                                result.numAdded++;
                                result.intact = false;
                                badRecord = true;
                                GetNextId(table, i, er.eventId);

                                string msg = String.Format("Integrity Check: Records add before the head of the chain."
                                                           + "First event in the chain: {0}.\n"
                                                           + "Low watermark: {1}.",
                                                           er.eventId,
                                                           lowWatermark);
                                ErrorLog.Instance.Write(ErrorLog.Level.Debug, msg);

                                if (returnEventList)
                                {
                                    AddToReturnList(CoreConstants.BadEventType_AddedEvent, er);
                                }

                                if (fixProblems)
                                {
                                    if (prevRow != null)
                                    {
                                        FixHashCode(prevRow, er.eventId);
                                    }
                                    else
                                    {
                                        newLowWatermark = er.eventId - 1;
                                    }
                                    er.startTime = FixRecordTime(er, prevRow, table, i);
                                    MarkRowBad(row, er, next, RecordStatus.RecordAdded);
                                    needUpdate = true;
                                }
                            }
                        }

                        // Check for broken chain
                        if (hasNextId)
                        {
                            if ((er.eventId == int.MaxValue) ? er.eventId + 1 > next + 1 : er.eventId > next)
                            {
                                // Events got deleted
                                string msg = String.Format("IntegrityCheck: Missing record. Id = {0}.  Rebuild chain with record {1}.", next, er.eventId);
                                ErrorLog.Instance.Write(ErrorLog.Level.Debug, msg);
                                result.numGaps++; //Math.Abs(next - er.eventId); // add number of missing events
                                result.intact = false;
                                hasNextId = false;

                                if (fixProblems)
                                {
                                    newRow = table.NewRow();
                                    newId = InsertBadRecord(newRow, er.eventId, er.startTime);
                                    if (prevRow != null)
                                    {
                                        FixHashCode(prevRow, newId);
                                    }
                                    needUpdate = true;
                                }
                                if (returnEventList)
                                {
                                    AddToReturnList(CoreConstants.BadEventType_EventGap, CreateBadRecord(er.eventId, er.startTime));
                                }
                            }
                            else if ((er.eventId == int.MaxValue) ? er.eventId + 1 < next + 1 : er.eventId < next)
                            {
                                // Inserted events. Reconnect chain and mark it bad.
                                string msg = String.Format("IntegrityCheck: Inserted bad record found. Id = {0}.  The original chained record Id = {1}.",
                                                            er.eventId,
                                                            next);
                                ErrorLog.Instance.Write(ErrorLog.Level.Debug, msg);
                                result.numAdded++;
                                result.intact = false;
                                badRecord = true;
                                GetNextId(table, i, er.eventId);

                                if (returnEventList)
                                {
                                    AddToReturnList(CoreConstants.BadEventType_AddedEvent, er);
                                }

                                if (fixProblems)
                                {
                                    if (prevRow != null)
                                    {
                                        FixHashCode(prevRow, er.eventId);
                                    }
                                    er.startTime = FixRecordTime(er, prevRow, table, i);
                                    MarkRowBad(row, er, next, RecordStatus.RecordAdded);
                                    needUpdate = true;
                                }
                            }
                        }

                        // Check checksum
                        if (!badRecord && er.checksum != er.GetHashCode())
                        {
                            // Modified
                            string msg = String.Format("IntegrityCheck: Record {0} is modified.", er.eventId);
                            ErrorLog.Instance.Write(ErrorLog.Level.Debug, msg);
                            result.numModified++;
                            result.intact = false;
                            badRecord = true;
                            GetNextId(table, i, er.eventId);

                            if (returnEventList)
                            {
                                AddToReturnList(CoreConstants.BadEventType_ModifiedEvent, er);
                            }

                            if (fixProblems)
                            {
                                er.startTime = FixRecordTime(er, prevRow, table, i);
                                MarkRowBad(row, er, next, RecordStatus.RecordModified);
                                needUpdate = true;
                            }
                        }

                        if (!badRecord)
                        {
                            next = chain.GetNextId(er.hash, er.checksum);

                            // Check hash code
                            if (er.hash != chain.GetHashCode(next, er.checksum))
                            {
                                string msg = String.Format("IntegrityCheck: Hashcode for record {0} is modified.", er.eventId);
                                ErrorLog.Instance.Write(ErrorLog.Level.Debug, msg);
                                result.numModified++;
                                GetNextId(table, i, er.eventId);
                                result.intact = false;

                                if (returnEventList)
                                {
                                    AddToReturnList(CoreConstants.BadEventType_ModifiedEvent, er);
                                }

                                if (fixProblems)
                                {
                                    MarkRowBad(row, er, next, RecordStatus.HashCodeModified);
                                    needUpdate = true;
                                }
                            }
                        }

                        // Special case for the last row
                        if (i == (rowCount - 1) && er.eventId == lastId)
                        {
                            if (er.eventId < highWatermark)
                            {
                                // events deleted from the tail
                                string msg = String.Format("Integrity Check: Records deleted at the tail of the chain."
                                                           + "Last event in the chain: {0}.\n"
                                                           + "High watermark: {1}.",
                                                           er.eventId,
                                                           highWatermark);
                                ErrorLog.Instance.Write(ErrorLog.Level.Debug, msg);
                                result.numGaps++;
                                result.intact = false;
                                if (fixProblems)
                                {
                                    newRow = table.NewRow();
                                    InsertBadRecord(newRow, highWatermark + 1, er.startTime);
                                    needUpdate = true;
                                }
                                if (returnEventList)
                                {
                                    AddToReturnList(CoreConstants.BadEventType_EventGap, CreateBadRecord(highWatermark + 1, er.startTime));
                                }
                            }
                        }
                        prevRow = row;
                        hasNextId = true;
                    }

                    if (fixProblems && needUpdate)
                    {
                        UpdateTable(table);
                    }
                    firstId = er.eventId + 1;
                    totalRows += rowCount;
                }
                while (rowCount > 0);

                //Reset first and last and Check DataChange and Sensitive Column tables
                firstId = GetFirstIdToCheck();
                lastId = GetLastIdToCheck(highWatermark);
                CheckDataChangeTables(fixProblems, ref result);
                CheckSensitiveColumnTable(fixProblems, ref result);

                // special case for empty table
                if (totalRows == 0 &&
                    table != null &&
                    highWatermark != -2100000000 &&
                    highWatermark != lowWatermark /*MARCUS + 1*/ )
                {
                    result.intact = false;
                    result.numGaps++;

                    string msg = String.Format("Integrity Check: The table is empty and there are events deleted."
                                                + "Low watermark: {0}.\n"
                                                + "High watermark: {1}.",
                                                lowWatermark,
                                                highWatermark);
                    ErrorLog.Instance.Write(ErrorLog.Level.Debug, msg);
                    next = highWatermark + 1;
                    DateTime eTime = GetCurrentTime();

                    if (fixProblems)
                    {
                        DataRow newRow = table.NewRow();
                        InsertBadRecord(newRow, next, eTime);
                        UpdateTable(table);
                        newLowWatermark = highWatermark - 1;
                    }
                    highWatermark = next + 1;

                    if (returnEventList)
                    {
                        AddToReturnList(CoreConstants.BadEventType_EventGap, CreateBadRecord(next, eTime));
                    }
                }
                else if (highWatermark == -2100000000 && totalRows > 0)
                {
                    // fix archive high watermark.  Note that we don't update repository database high watermark
                    // since it is update automatically.
                    if (isArchive)
                        UpdateArchiveHighWatermark(lastId);
                }

                if (!isArchive)
                    CheckEventsAddedAtTail(result, fixProblems);

                if (!result.intact &&
                    fixProblems &&
                    newLowWatermark != int.MaxValue)
                {
                    if (isArchive)
                        UpdateArchiveLowWatermark(newLowWatermark);
                    else
                        UpdateLowWatermark(newLowWatermark);
                    int count = result.numAdded + result.numGaps + result.numModified;
                    Stats.Stats.Instance.UpdateIntegrityCheckCount(inDatabase, count);
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write("An error occurred checking " + database + " integrity .",
                                         e,
                                         ErrorLog.Severity.Warning,
                                         true);
                result.integrityCheckError = e.Message;
                result.intact = false;
            }
            finally
            {
                da = null;
                if (table != null)
                    table.Dispose();
                chain = null;
                returnEventList = true;

                eventList = (EventRecord[])badEventList.ToArray(typeof(EventRecord));
                eventTypeList = (int[])badEventTypeList.ToArray(typeof(int));

                StringBuilder msg = new StringBuilder();
                msg.AppendFormat("\r\nIntegrity Check: {0}\r\n\tDatabase: {2}\r\n{1} Rows checked .\r\n",
                                  result.intact ? "Passed" : "Failed",
                                  totalRows,
                                  database);
                msg.AppendFormat("\tEarliest event startTime: {0}\r\n", result.earliestTime);
                msg.AppendFormat("\tLatest event startTime: {0}\r\n", result.latestTime);
                if (!result.intact)
                {
                    if (result.integrityCheckError != "")
                    {
                        msg.AppendFormat("\tError during processing:\r\n\t{0}\r\n", result.integrityCheckError);
                        RaiseAlert(msg.ToString());
                    }
                    else
                    {
                        msg.AppendFormat("\t{0} inserted bad records\r\n", result.numAdded);
                        msg.AppendFormat("\t{0} modified records\r\n", result.numModified);
                        msg.AppendFormat("\t{0} deleted record gaps\r\n", result.numGaps);
                        if (fixProblems)
                        {
                            msg.AppendFormat("{0}", CoreConstants.Info_IntegrityRepaired);
                            ErrorLog.Instance.Write(msg.ToString());
                            ResetBadIntegrityFlags();
                        }
                        else
                        {
                            SetBadIntegrityFlags();
                            RaiseAlert(msg.ToString());
                        }
                    }
                }
                else
                    ErrorLog.Instance.Write(ErrorLog.Level.Debug, msg.ToString());

                // mark integrity results
                int resultValue = CoreConstants.IntegrityCheck_Passed;
                if (!result.intact)
                {
                    if (fixProblems)
                        resultValue = CoreConstants.IntegrityCheck_FailedAndRepaired;
                    else
                        resultValue = CoreConstants.IntegrityCheck_Failed;
                }

                if (inIsArchive)
                    UpdateLastIntegrityCheckResultArchive(database, resultValue);
                else
                    UpdateLastIntegrityCheckResult(instance, resultValue);

                // Now we do our ChangeLog entries
                string snapshot;
                rep = new Repository();
                rep.OpenConnection();

                if (!result.intact)
                {
                    // Did we get an error?
                    if (result.integrityCheckError != null && result.integrityCheckError.Length > 0)
                    {
                        // an error occurred checking integrity
                        // Were we repairing or just checking?
                        if (fixProblems)
                        {
                            // We were attempting a repair
                            snapshot = String.Format(CoreConstants.Error_IntegrityCheckRepairErrorLog, instance, result.integrityCheckError);
                        }
                        else
                        {
                            // We were simply checking the integrity
                            snapshot = String.Format(CoreConstants.Error_IntegrityCheckErrorLog, instance, result.integrityCheckError);
                        }
                    }
                    else
                    {
                        snapshot = String.Format(CoreConstants.Info_IntegrityCheckFailedLog,
                                                   instance,
                                                   inDatabase,
                                                   result.earliestTime,
                                                   result.latestTime,
                                                   result.numGaps,
                                                   result.numModified,
                                                   result.numAdded);
                        if (fixProblems)
                            snapshot += CoreConstants.Info_IntegrityCheckFailedLog_Fixed;
                        else
                            snapshot += CoreConstants.Info_IntegrityCheckFailedLog_NotFixed;
                    }

                    // Integrity Failure
                    LogRecord.WriteLog(rep.connection, LogType.IntegrityBroken, instance, snapshot);
                }
                else
                {
                    snapshot = String.Format(CoreConstants.Info_IntegrityCheckPassedLog, instance, database);

                    if (result.earliestTime != DateTime.MaxValue)
                    {
                        snapshot += String.Format(CoreConstants.Info_IntegrityCheckPassedPeriodLog, result.earliestTime, result.latestTime);
                    }
                    else
                    {
                        snapshot += CoreConstants.Info_IntegrityCheckPassedNoEventsLog;
                    }
                    LogRecord.WriteLog(rep.connection, LogType.ManualIntegrityCheck, instance, snapshot);
                }
                rep.CloseConnection();
            }
            return result;
        }

        private void CheckDataChangeTables(bool fixProblems, ref CheckResult result)
        {
            CheckDataChanges(fixProblems, ref result);
            CheckColumnChanges(fixProblems, ref result);
        }

        private void CheckDataChanges(bool fixProblems, ref CheckResult result)
        {
            DataTable dcTable;
            DataRow row;
            DataChangeRecord dcr = null;
            List<DataChangeRecord> badDCRecords = new List<DataChangeRecord>();
            bool badRecord;
            bool needUpdate = false;
            long prevdcId = -9223372036854775807 - 1;
            long currentdcId = -9223372036854775807;
            int rowCount;

            try
            {
                dcTable = GetDataChangeTable();

                if (dcTable == null)
                {
                    // GetDataChangeTable() should not return nulls.  
                    // It throws an exception when something is wrong.  Otherwise,
                    // it returns a valid table.
                    result.intact = false;
                    result.integrityCheckError = "Error retrieving records from DataChanges table";
                }
                else if (dcTable.Rows.Count == 0)
                {
                    return;
                }
                rowCount = dcTable.Rows.Count;

                for (int i = 0; i < rowCount; i++)
                {
                    row = dcTable.Rows[i];
                    dcr = CreateDataChangeRecord(row);
                    badRecord = false;

                    if (i == 0)
                    {
                        //this is the first record.
                        prevdcId = dcr.dcId;
                        currentdcId = dcr.dcId;
                    }
                    else
                    {
                        prevdcId = currentdcId;
                        currentdcId = dcr.dcId;
                    }

                    //check for missing record after the first one.
                    if (i > 0 && prevdcId + 1 != currentdcId)
                    {
                        //Some of the eventIDs might be null;
                        ArrayList missingIds = GetMissingDCIds(prevdcId + 1, (currentdcId - (prevdcId + 1)));

                        if (missingIds.Count > 0)
                        {
                            //missing record!
                            badRecord = true;
                            string msg = String.Format("IntegrityCheck: Missing {0} DataChange Id{1}.", missingIds.Count, missingIds.Count == 1 ? "" : "s");
                            ErrorLog.Instance.Write(ErrorLog.Level.Debug, msg);
                            result.numDCGaps += missingIds.Count;
                            result.intact = false;

                            if (fixProblems)
                            {
                                badDCRecords.AddRange(CreateBadDataChangeRecord(missingIds, dcr.startTime));
                                needUpdate = true;
                            }
                        }
                    }

                    //check for modified record
                    var hash = dcr.GetHashCode(); //NativeMethods.GetHashCode(dcr.ToString()); changed By Hemant

                    if (!badRecord && hash != dcr.hashcode)
                    {
                        // modified record!
                        badRecord = true;
                        string msg = String.Format("IntegrityCheck: Modified DataChange record. Id = {0}.", dcr.dcId);
                        ErrorLog.Instance.Write(ErrorLog.Level.Debug, msg);
                        result.numDCModified++;
                        result.intact = false;

                        if (fixProblems)
                        {
                            //Fix the hashcode
                            row["hashcode"] = hash; //NativeMethods.GetHashCode(dcr.ToString()); changed By Hemant
                            needUpdate = true;
                        }

                        if (returnEventList)
                        {
                            AddToReturnList(CoreConstants.BadEventType_ModifiedEvent, GetEventRecord(dcr.dcId));
                        }
                    }
                }

                if (fixProblems && needUpdate)
                    UpdateDCTable(dcTable, badDCRecords);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(String.Format("An error occurred checking DataChange table in {0} integrity .", database),
                                         ex,
                                         ErrorLog.Severity.Warning,
                                         true);
                throw ex;
            }
        }

        private EventRecord GetEventRecord(long dcId)
        {
            string stmt = String.Format("SELECT eventId from DataChanges where dcId = {0}", dcId);
            int eventId = 0;
            bool success = false;
            object obj = null;
            EventRecord record = null;

            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                SqlCommand command = new SqlCommand(stmt, conn);
                command.CommandTimeout = CoreConstants.sqlcommandTimeout;
                obj = command.ExecuteScalar();

                if (obj != null && !(obj is System.DBNull))
                {
                    eventId = (int)obj;

                    record = GetEvent(conn, eventId);
                }
                conn.Close();
            }
            return record;
        }

        private EventRecord GetSCEventRecord(int eventId)
        {
            EventRecord record = null;

            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                record = GetEvent(conn, eventId);
                conn.Close();
            }
            return record;
        }

        private ArrayList GetMissingSCIds(int startingId, int count)
        {
            ArrayList missingIds = new ArrayList();
            ArrayList foundIds = new ArrayList();
            SqlConnection conn = null;

            try
            {
                conn = new SqlConnection(GetConnectionString(database));
                conn.Open();

                string query = String.Format("SELECT scId from {0} where scId >= {1} and scId <= {2} ORDER BY scId", CoreConstants.RepositorySensitiveColumnsTable,
                                                                                                                      startingId,
                                                                                                                      startingId + (count - 1));

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                if (!reader.IsDBNull(0))
                                    foundIds.Add(reader.GetInt32(0));
                            }
                        }
                    }
                }

                for (int i = 0; i < count; i++)
                {
                    if (foundIds.IndexOf(startingId + i) == -1)
                        missingIds.Add(startingId + i);
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write("An error occurred checking for missing SensitiveColumn Ids.",
                                         e,
                                         ErrorLog.Severity.Warning,
                                         true);
                throw e;
            }
            finally
            {
                conn.Close();
            }
            return missingIds;
        }

        private ArrayList GetMissingCCIds(long startingId, long count)
        {
            ArrayList missingIds = new ArrayList();
            ArrayList foundIds = new ArrayList();
            SqlConnection conn = null;

            try
            {
                conn = new SqlConnection(GetConnectionString(database));
                conn.Open();

                string query = String.Format("SELECT ccId from {0} where ccId >= {1} and ccId <= {2} ORDER BY ccId", CoreConstants.RepositoryColumnChangesTable,
                                                                                                       startingId, startingId + (count - 1));

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                if (!reader.IsDBNull(0))
                                    foundIds.Add(reader.GetInt32(0));
                            }
                        }
                    }
                }

                for (int i = 0; i < count; i++)
                {
                    if (foundIds.IndexOf(startingId + i) == -1)
                        missingIds.Add(startingId + i);
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write("An error occurred checking for missing ColumnChange Ids.",
                                         e,
                                         ErrorLog.Severity.Warning,
                                         true);
                throw e;
            }
            finally
            {
                conn.Close();
            }
            return missingIds;
        }

        private ArrayList GetMissingDCIds(long startingId, long count)
        {
            ArrayList missingIds = new ArrayList();
            ArrayList foundIds = new ArrayList();
            SqlConnection conn = null;

            try
            {
                conn = new SqlConnection(GetConnectionString(database));
                conn.Open();

                string query = String.Format("SELECT dcId from {0} where dcId >= {1} and dcId <= {2} ORDER BY dcId", CoreConstants.RepositoryDataChangesTable,
                                                                                                       startingId, startingId + (count - 1));

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                if (!reader.IsDBNull(0))
                                    foundIds.Add(reader.GetInt32(0));
                            }
                        }
                    }
                }

                for (int i = 0; i < count; i++)
                {
                    if (foundIds.IndexOf(startingId + i) == -1)
                        missingIds.Add(startingId + i);
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write("An error occurred checking for missing DataChange Ids.",
                                         e,
                                         ErrorLog.Severity.Warning,
                                         true);
                throw e;
            }
            finally
            {
                conn.Close();
            }
            return missingIds;
        }

        private EventRecord GetEvent(SqlConnection conn, int eventId)
        {
            EventRecord record = new EventRecord(conn, database);

            if (record.Read(eventId))
                return record;
            return null;
        }

        private void CheckColumnChanges(bool fixProblems, ref CheckResult result)
        {
            DataTable ccTable;
            DataRow row;
            ColumnChangeRecord ccr = null;
            List<ColumnChangeRecord> badCCRecords = new List<ColumnChangeRecord>();
            bool badRecord;
            bool needUpdate = false;
            long prevccId = -9223372036854775807 - 1;
            long currentccId = -9223372036854775807;
            int rowCount;

            try
            {
                ccTable = GetColumnChangeTable();

                if (ccTable == null)
                {
                    // GetColumnChangeTable() should not return nulls.  
                    // It throws an exception when something is wrong.  Otherwise,
                    // it returns a valid table.
                    result.intact = false;
                    result.integrityCheckError = "Error retrieving records from ColumnChanges table";
                }
                else if (ccTable.Rows.Count == 0)
                {
                    return;
                }
                rowCount = ccTable.Rows.Count;

                for (int i = 0; i < rowCount; i++)
                {
                    row = ccTable.Rows[i];
                    ccr = CreateColumnChangeRecord(row);
                    badRecord = false;

                    if (i == 0)
                    {
                        //this is the first record.
                        prevccId = ccr.ccId;
                        currentccId = ccr.ccId;
                    }
                    else
                    {
                        prevccId = currentccId;
                        currentccId = ccr.ccId;
                    }

                    //check for missing record
                    if (i > 0 && prevccId + 1 != currentccId)
                    {
                        ArrayList missingIds = GetMissingCCIds(prevccId + 1, (currentccId - (prevccId + 1)));

                        if (missingIds.Count > 0)
                        {
                            //missing record!
                            badRecord = true;
                            string msg = String.Format("IntegrityCheck: Missing {0} ColumnChange Id{1}.", missingIds.Count, missingIds.Count == 1 ? "" : "s");
                            ErrorLog.Instance.Write(ErrorLog.Level.Debug, msg);
                            result.numCCGaps += missingIds.Count;
                            result.intact = false;

                            if (fixProblems)
                            {
                                badCCRecords.AddRange(CreateBadColumnChangeRecord(missingIds, ccr.startTime));
                                needUpdate = true;
                            }
                        }
                    }

                    //check for modified record
                    var hash = ccr.GetHashCode();

                    if (!badRecord && hash != ccr.hashcode)
                    {
                        // modified record!
                        badRecord = true;
                        string msg = String.Format("IntegrityCheck: Modified ColumnChange record. Id = {0}.", ccr.ccId);
                        ErrorLog.Instance.Write(ErrorLog.Level.Debug, msg);
                        result.numCCModified++;
                        result.intact = false;

                        if (fixProblems)
                        {
                            //Fix the hashcode
                            row["hashcode"] = hash; //ccr.GetHashCode(); changed by Hemant
                            needUpdate = true;
                        }

                        if (returnEventList)
                        {
                            AddToReturnList(CoreConstants.BadEventType_ModifiedEvent, GetEventRecord(ccr.dcId));
                        }
                    }
                }

                if (fixProblems && needUpdate)
                    UpdateCCTable(ccTable, badCCRecords);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(String.Format("An error occurred checking ColumnChange table in {0} integrity .", database),
                                         ex,
                                         ErrorLog.Severity.Warning,
                                         true);
                throw ex;
            }
        }

        private DataTable GetDataChangeTable()
        {
            SqlDataAdapter da;
            DataTable table;
            string stmt = "";

            try
            {
                stmt = GetDataChangeRecordStmt();
                da = new SqlDataAdapter(stmt, GetConnectionString());
                da.SelectCommand.CommandTimeout = CoreConstants.sqlcommandTimeout;
                table = new DataTable();
                da.Fill(table);
                columnMappings.Clear();

                foreach (DataColumn column in table.Columns)
                {
                    try
                    {
                        columnMappings.Add(column.ColumnName, column.Ordinal);
                    }
                    catch { }
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(String.Format("An error occurred retrieving Data Change records for integrity check. Stmt: {0}", stmt),
                                         e,
                                         ErrorLog.Severity.Warning);
                throw e;
            }
            return table;
        }

        private DataTable GetSensitiveColumnTable()
        {
            SqlDataAdapter da;
            DataTable table;
            string stmt = "";

            try
            {
                stmt = GetSensitiveColumnRecordStmt();
                da = new SqlDataAdapter(stmt, GetConnectionString());
                da.SelectCommand.CommandTimeout = CoreConstants.sqlcommandTimeout;
                table = new DataTable();
                da.Fill(table);
                columnMappings.Clear();

                foreach (DataColumn column in table.Columns)
                {
                    try
                    {
                        columnMappings.Add(column.ColumnName, column.Ordinal);
                    }
                    catch { }
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(String.Format("An error occurred retrieving Sensitive Column records for integrity check. Stmt: {0}", stmt),
                                         e,
                                         ErrorLog.Severity.Warning);
                throw e;
            }
            return table;
        }

        private DataTable GetColumnChangeTable()
        {
            SqlDataAdapter da;
            DataTable table;
            string stmt = "";

            try
            {
                stmt = GetColumnChangeRecordStmt();
                da = new SqlDataAdapter(stmt, GetConnectionString());
                da.SelectCommand.CommandTimeout = CoreConstants.sqlcommandTimeout;
                table = new DataTable();
                da.Fill(table);
                columnMappings.Clear();

                foreach (DataColumn column in table.Columns)
                {
                    try
                    {
                        columnMappings.Add(column.ColumnName, column.Ordinal);
                    }
                    catch { }
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(String.Format("An error occurred retrieving ColumnChange records for integrity check. Stmt: {0}", stmt),
                                         e,
                                         ErrorLog.Severity.Warning);
                throw e;
            }
            return table;
        }

        private void CheckSensitiveColumnTable(bool fixProblems, ref CheckResult result)
        {
            DataTable scTable;
            DataRow row;
            SensitiveColumnRecord scr = null;
            List<SensitiveColumnRecord> badSCRecords = new List<SensitiveColumnRecord>();
            bool badRecord;
            bool needUpdate = false;
            int prevscId = -2100000000 - 1;
            int currentscId = -2100000000;
            int rowCount;

            try
            {
                scTable = GetSensitiveColumnTable();

                if (scTable == null)
                {
                    // GetSensitiveColumnTable() should not return nulls.  
                    // It throws an exception when something is wrong.  Otherwise,
                    // it returns a valid table.
                    result.intact = false;
                    result.integrityCheckError = "Error retrieving records from SensitiveColumn table";
                }
                else if (scTable.Rows.Count == 0)
                {
                    return;
                }
                rowCount = scTable.Rows.Count;

                for (int i = 0; i < rowCount; i++)
                {
                    row = scTable.Rows[i];
                    scr = CreateSensitiveColumnRecord(row);
                    badRecord = false;

                    if (i == 0)
                    {
                        //this is the first record.
                        prevscId = scr.scId;
                        currentscId = scr.scId;
                    }
                    else
                    {
                        prevscId = currentscId;
                        currentscId = scr.scId;
                    }

                    //check for missing record
                    if (i > 0 && prevscId + 1 != currentscId)
                    {
                        ArrayList missingIds = GetMissingSCIds(prevscId + 1, (currentscId - (prevscId + 1)));

                        if (missingIds.Count > 0)
                        {
                            //missing record!
                            badRecord = true;
                            string msg = String.Format("IntegrityCheck: Missing {0} Sensitive Column Id{1}.", missingIds.Count, missingIds.Count == 1 ? "" : "s");
                            ErrorLog.Instance.Write(ErrorLog.Level.Debug, msg);
                            result.numSCGaps += missingIds.Count;
                            result.intact = false;

                            if (fixProblems)
                            {
                                badSCRecords.AddRange(CreateBadSensitiveColumnRecord(missingIds, scr.startTime));
                                needUpdate = true;
                            }
                        }
                    }

                    //check for modified record
                    int hash = scr.GetHashCode();

                    if (!badRecord && hash != scr.hashcode)
                    {
                        // modified record!
                        badRecord = true;
                        string msg = String.Format("IntegrityCheck: Modified SensitiveColumn record. Id = {0}.", scr.scId);
                        ErrorLog.Instance.Write(ErrorLog.Level.Debug, msg);
                        result.numSCModified++;
                        result.intact = false;

                        if (fixProblems)
                        {
                            //Fix the hashcode
                            row["hashcode"] = scr.GetHashCode();
                            needUpdate = true;
                        }

                        if (returnEventList)
                        {
                            AddToReturnList(CoreConstants.BadEventType_ModifiedEvent, GetSCEventRecord(scr.eventId));
                        }
                    }
                }

                if (fixProblems && needUpdate)
                    UpdateSCTable(scTable, badSCRecords);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(String.Format("An error occurred checking SensitiveColumn table in {0} integrity .", database),
                                         ex,
                                         ErrorLog.Severity.Warning,
                                         true);
                throw ex;
            }
        }

        private string GetDataChangeRecordStmt()
        {
            return String.Format("SELECT {0} from {1} dc where dc.eventId >= {2} and dc.eventId <= {3} ORDER BY dc.dcId", DataChangeRecord.SelectColumnList,
                                                                                                                           CoreConstants.RepositoryDataChangesTable,
                                                                                                                           firstId,
                                                                                                                           lastId);
        }

        private string GetColumnChangeRecordStmt()
        {
            string columnList = String.Format(ColumnChangeRecord.AliasedSelectColumnList, "cc");
            return String.Format("SELECT {0} from {1} cc join DataChanges dc on cc.dcId = dc.dcId where dc.eventId >= {2} and dc.eventId <= {3} ORDER BY cc.ccId", columnList,
                                                                                                                                                                     CoreConstants.RepositoryColumnChangesTable,
                                                                                                                                                                     firstId,
                                                                                                                                                                     lastId);
        }

        private string GetSensitiveColumnRecordStmt()
        {
            return String.Format("SELECT {0} from {1} sc where sc.eventId >= {2} and sc.eventId <= {3} ORDER BY sc.scId", SensitiveColumnRecord.SelectColumnList,
                                                                                                                           CoreConstants.RepositorySensitiveColumnsTable,
                                                                                                                           firstId,
                                                                                                                           lastId);
        }

        //----------------------------------------------------------------------
        // CheckEventsAddedAtTail: check if there are added events after high
        //                         watermark.
        //----------------------------------------------------------------------
        private void CheckEventsAddedAtTail(CheckResult result, bool fixProblems)
        {
            int numAdded = 0;
            int nextId = 0;
            int rowCount = 0;
            DataTable table = null;
            DateTime startTime;

            lock (EventId.AcquireInstanceLock(instanceName))
            {
                highWatermark = EventId.ReadHighWatermark(instanceName);
                numAdded = GetAddedEventsAtTailCount(highWatermark);

                if (numAdded > 0 && fixProblems)
                    nextId = EventId.GetNextIdBlock(instanceName, highWatermark, numAdded);
            }

            if (numAdded > 0)
            {
                EventRecord er = null;
                DataRow row;
                bool loop;

                result.intact = false;
                result.numAdded += numAdded;

                if (!returnEventList && !fixProblems)
                    return;

                string msg = String.Format("Integrity Check: Records add after the tail of the chain. High watermark: {0}.", highWatermark);
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, msg);
                firstId = highWatermark;

                if (result.latestTime == DateTime.MinValue)
                    result.latestTime = GetCurrentTime();
                startTime = result.latestTime;

                do
                {
                    table = GetAddedEventsAtTail(firstId);
                    rowCount = table.Rows.Count;

                    for (int i = 0; i < rowCount; i++)
                    {
                        row = table.Rows[i];
                        er = CreateEventRecord(row);
                        if (fixProblems)
                        {
                            er.startTime = startTime;
                            nextId++;
                            MarkAddedRow(er, row, nextId);
                        }

                        if (returnEventList)
                        {
                            AddToReturnList(CoreConstants.BadEventType_AddedEvent, er);
                        }
                    }

                    if (fixProblems && rowCount > 0)
                    {
                        UpdateAddedTable(table);
                    }
                    firstId = er.eventId;
                    loop = (rowCount > 0) && (fixProblems || returnEventList);
                }
                while (loop);
            }
        }

        #endregion

        #region Single Record Check
        //----------------------------------------------------------------------
        // CheckRow: check the integrity of a DataRow.
        //----------------------------------------------------------------------
        private RecordStatus CheckRow(DataRow row)
        {
            return CheckRecord(CreateEventRecord(row));
        }

        //----------------------------------------------------------------------
        // CheckRecord: check the integrity of an EventRecord.
        //----------------------------------------------------------------------
        private RecordStatus CheckRecord(EventRecord er)
        {
            if (hasNextId)
            {
                if (er.eventId > next)
                {
                    // Events got deleted
                    hasNextId = false;
                    return RecordStatus.RecordDeleted;
                }
                else if (er.eventId < next)
                {
                    // Inserted events.  next is still valid.
                    return RecordStatus.RecordAdded;
                }
            }
            hasNextId = false;

            if (er.checksum != er.GetHashCode())
            {
                // Modified
                return RecordStatus.RecordModified;
            }
            next = chain.GetNextId(er.hash, er.checksum);

            if (er.hash != chain.GetHashCode(next, er.checksum))
            {
                hasNextId = false;
                // HashCode modified
                return RecordStatus.HashCodeModified;
            }
            hasNextId = true;
            return RecordStatus.Intact;
        }

        private RecordStatus CheckRecord(DataChangeRecord dc)
        {
            if (hasNextId)
            {
                if (dc.eventId > next)
                {
                    // Events got deleted
                    hasNextId = false;
                    return RecordStatus.RecordDeleted;
                }
                else if (dc.eventId < next)
                {
                    // Inserted events.  next is still valid.
                    return RecordStatus.RecordAdded;
                }
            }

            hasNextId = false;

            if (dc.hashcode != dc.GetHashCode())
            {
                // Modified
                return RecordStatus.RecordModified;
            }
            //next = chain.GetNextId(dc.hash, dc.hashcode);

            //if (dc.hash != chain.GetHashCode(next, dc.hashcode))
            //{
            //   hasNextId = false;
            //   // HashCode modified
            //   return RecordStatus.HashCodeModified;
            //}

            hasNextId = true;
            return RecordStatus.Intact;
        }

        private RecordStatus CheckRecord(ColumnChangeRecord cc, int eventId)
        {
            if (hasNextId)
            {
                if (eventId > next)
                {
                    // Events got deleted
                    hasNextId = false;
                    return RecordStatus.RecordDeleted;
                }
                else if (eventId < next)
                {
                    // Inserted events.  next is still valid.
                    return RecordStatus.RecordAdded;
                }
            }

            hasNextId = false;

            if (cc.hashcode != cc.GetHashCode())
            {
                // Modified
                return RecordStatus.RecordModified;
            }
            //next = chain.GetNextId(cc.hash, cc.hashcode);

            //if (cc.hash != chain.GetHashCode(next, cc.hashcode))
            //{
            //   hasNextId = false;
            //   // HashCode modified
            //   return RecordStatus.HashCodeModified;
            //}

            hasNextId = true;
            return RecordStatus.Intact;
        }

        private RecordStatus CheckRecord(SensitiveColumnRecord sc)
        {
            if (hasNextId)
            {
                if (sc.eventId > next)
                {
                    // Events got deleted
                    hasNextId = false;
                    return RecordStatus.RecordDeleted;
                }
                else if (sc.eventId < next)
                {
                    // Inserted events.  next is still valid.
                    return RecordStatus.RecordAdded;
                }
            }

            hasNextId = false;

            if (sc.hashcode != sc.GetHashCode())
            {
                // Modified
                return RecordStatus.RecordModified;
            }
            //next = chain.GetNextId(sc.hash, sc.hashcode);

            //if (scr.hash != chain.GetHashCode(next, sc.hashcode))
            //{
            //   hasNextId = false;
            //   // HashCode modified
            //   return RecordStatus.HashCodeModified;
            //}

            hasNextId = true;
            return RecordStatus.Intact;
        }

        #endregion

        #region Review and Rebuild Chain
        //----------------------------------------------------------------------
        // RebuildChain: rebuilds the chain after archiving.
        //----------------------------------------------------------------------
        public bool RebuildChain(string instance, string inDatabase, bool inIsArchive)
        {
            DataTable table = null;
            bool success = true;
            int totalRows = 0;

            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                     String.Format("Rebuilding hash chain for database {0}.", inDatabase));
            try
            {
                instanceName = instance;
                database = inDatabase;
                isArchive = inIsArchive;

                DataRow row;
                DataRow prevRow;
                EventRecord er;
                bool needUpdate;
                int high = GetHighWatermark();
                int checksum = 0;

                firstId = GetFirstIdToCheck();
                if (firstId == int.MaxValue)
                    lowWatermark = high - 1;
                else
                    lowWatermark = firstId - 1;

                lastId = GetLastIdToCheck();
                if (lastId == int.MinValue)
                    highWatermark = high;
                else
                    highWatermark = lastId;

                chain = new HashChain(instanceName);

                do
                {
                    needUpdate = false;
                    table = GetEventsTable();

                    if (table == null)
                    {
                        // Error
                        return false;
                    }
                    else if (table.Rows.Count == 0)
                    {
                        continue;
                    }
                    row = table.Rows[0];
                    er = CreateEventRecord(row);
                    next = chain.GetNextId(er.hash, er.checksum);
                    prevRow = row;

                    for (int i = 1; i < table.Rows.Count; i++)
                    {
                        row = table.Rows[i];
                        er = CreateEventRecord(row);

                        if (er.eventId != next)
                        {
                            prevRow[GetIndex("hash")] = chain.GetHashCode(er.eventId, (int)prevRow[GetIndex("checksum")]);
                            needUpdate = true;
                        }
                        //GetNextId( table, i, er.eventId );
                        next = chain.GetNextId(er.hash, er.checksum);
                        prevRow = row;
                    }
                    checksum = er.checksum;
                    int hash = chain.GetHashCode(next, checksum);

                    if (hash != er.hash)
                    {
                        row[GetIndex("hash")] = hash;
                        needUpdate = true;
                    }

                    if (needUpdate)
                    {
                        UpdateTableHash(table);
                    }
                    firstId = er.eventId; // include the last record from the previous result set
                    totalRows += (table.Rows.Count - 1);
                }
                while (table.Rows.Count == checkBatchSize);

                if (success)
                {
                    if (totalRows == 0)
                        lowWatermark = highWatermark;

                    if (isArchive)
                        success = UpdateArchiveWatermarks(database, lowWatermark, highWatermark);
                    else
                    {
                        ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                                 String.Format("Rebuild Hashchain: Updating watermarks.  Last Id = {0}, old highWatermark = {1}, total rows = {2}",
                                                                lastId, high, totalRows));
                        if (lastId < high && totalRows > 0) // The record with the high watermark is archived.  Fix it.
                        {
                            FixMissingHighWatermarkRecord(instance, database, lastId, checksum, high);
                        }
                        EventId.ResetWatermarks(instance, database);
                    }

                    EventDatabase.SetDatabaseState(database, EventsDatabaseState.NormalChainIntact);
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write("An error occurred rebuilding the hash chain.", e, ErrorLog.Severity.Warning, true);
                success = false;

            }
            finally
            {
                if (table != null)
                {
                    table.Dispose();
                }

                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         String.Format("Rebuild hash chain: {0} records chained for database {1}.",
                                                        totalRows,
                                                        database));
            }
            return success;
        }

        //----------------------------------------------------------------------
        // AddToReturnList
        //----------------------------------------------------------------------
        private void AddToReturnList(int badEventType, EventRecord er)
        {
            if (er != null)
            {
                badEventList.Add(er);
                badEventTypeList.Add(badEventType);

                if (badEventList.Count >= maxBadEventsReturned)
                    returnEventList = false;
            }
        }

        //----------------------------------------------------------------------
        // InsertBadRecord
        //----------------------------------------------------------------------
        private int InsertBadRecord(DataRow row, int id, DateTime startTime)
        {
            EventRecord er = CreateBadRecord(id, startTime);
            CopyToRow(er, row);
            badRecords.Add(row);
            return er.eventId;
        }

        //----------------------------------------------------------------------
        // CreateBadRecord
        //----------------------------------------------------------------------
        private EventRecord CreateBadRecord(int id, DateTime startTime)
        {
            EventRecord er = new EventRecord();

            er.startTime = startTime;
            er.eventId = id - 1;
            er.eventType = TraceEventType.MissingEvents;
            er.eventCategory = TraceEventCategory.Corrupted;
            er.applicationName = CoreConstants.CollectionServerName;
            er.databaseName = database;
            er.alertLevel = 1;  // TODO: make it a real alert level when defined.
            er.checksum = er.GetHashCode();
            er.hash = chain.GetHashCode(id, er.checksum);
            return er;
        }

        private List<DataChangeRecord> CreateBadDataChangeRecord(ArrayList missingIds, DateTime startTime)
        {
            List<DataChangeRecord> dcRecords = new List<DataChangeRecord>();
            DataChangeRecord dcRecord;

            foreach (int id in missingIds)
            {
                dcRecord = new DataChangeRecord();

                dcRecord.startTime = startTime;
                dcRecord.eventSequence = -1;
                dcRecord.spid = -1;
                dcRecord.databaseId = -1;
                dcRecord.actionType = -1;
                dcRecord.schemaName = "";
                dcRecord.tableName = "";
                dcRecord.recordNumber = 0;
                dcRecord.user = "";
                dcRecord.changedColumns = 0;
                dcRecord.totalChanges = 0;
                dcRecord.primaryKey = "";
                dcRecord.tableId = -1;
                dcRecord.dcId = id;
                dcRecord.hashcode = dcRecord.GetHashCode();
                dcRecords.Add(dcRecord);
            }
            return dcRecords;
        }

        private List<SensitiveColumnRecord> CreateBadSensitiveColumnRecord(ArrayList missingIds, DateTime startTime)
        {
            List<SensitiveColumnRecord> scRecords = new List<SensitiveColumnRecord>();
            SensitiveColumnRecord scRecord;

            foreach (int id in missingIds)
            {
                scRecord = new SensitiveColumnRecord();

                scRecord.startTime = startTime;
                scRecord.eventId = -2100000000;
                scRecord.columnName = "";
                scRecord.scId = id;
                scRecord.hashcode = scRecord.GetHashCode();
                scRecords.Add(scRecord);
            }
            return scRecords;
        }

        private List<ColumnChangeRecord> CreateBadColumnChangeRecord(ArrayList missingIds, DateTime startTime)
        {
            List<ColumnChangeRecord> ccRecords = new List<ColumnChangeRecord>();
            ColumnChangeRecord ccRecord;

            foreach (int id in missingIds)
            {
                ccRecord = new ColumnChangeRecord();
                ccRecord.startTime = startTime;
                ccRecord.eventSequence = -1;
                ccRecord.spid = -1;
                ccRecord.columnName = "";
                ccRecord.beforeValue = "";
                ccRecord.afterValue = "";
                ccRecord.columnId = -1;
                ccRecord.dcId = -1;
                ccRecord.ccId = id;
                ccRecord.hashcode = ccRecord.GetHashCode();
                ccRecords.Add(ccRecord);
            }
            return ccRecords;
        }

        //----------------------------------------------------------------------
        // FixCheckSum
        //----------------------------------------------------------------------
        private void FixChecksum(DataRow row, EventRecord er)
        {
            row[GetIndex("checksum")] = er.GetHashCode();
        }

        //----------------------------------------------------------------------
        // FixHashCode
        //----------------------------------------------------------------------
        private void FixHashCode(DataRow row, int id)
        {
            row[GetIndex("hash")] = chain.GetHashCode(id, (int)row[GetIndex("checksum")]);
        }

        //----------------------------------------------------------------------
        // FixRecordTime
        //----------------------------------------------------------------------
        private DateTime FixRecordTime(EventRecord er,
                                       DataRow prevRow,
                                       DataTable table,
                                       int rowNumber)
        {
            DateTime prevTime;
            DateTime nextTime;

            // if there was a previous row, use its startTime
            //     else just use what we've got
            if (prevRow != null)
                prevTime = (DateTime)prevRow[GetIndex("startTime")];
            else
                prevTime = er.startTime; // DateTime.MaxValue;

            // if there is a next row, gets its time
            // else use current time
            DataRow nextRow = GetNextRow(table, rowNumber);
            if (nextRow != null)
                nextTime = (DateTime)nextRow[GetIndex("startTime")];
            else
                nextTime = GetCurrentTime();

            // use what weve got to work with
            if (prevRow == null)
            {
                if (nextRow == null)
                {
                    // no prev or next; use startTime unless it is in the future
                    DateTime currTime = GetCurrentTime();

                    if (er.startTime.CompareTo(currTime) < 0)
                    {
                        return er.startTime;
                    }
                    else
                    {
                        return currTime;
                    }
                }
                else
                {
                    //no prev but we have a next; tie it together with this one
                    return nextTime;
                }
            }
            else
            {
                // we have a previous record; use its time
                return prevTime;
            }
        }

        //----------------------------------------------------------------------
        // MarkAddedRow
        //----------------------------------------------------------------------
        private void MarkAddedRow(EventRecord er, DataRow row, int id)
        {
            MarkRecordBad(er, RecordStatus.RecordAdded);
            row[GetIndex("eventType")] = (int)er.eventType;
            row[GetIndex("eventCategory")] = (int)er.eventCategory;
            row[GetIndex("startTime")] = er.startTime;
            row[GetIndex("oldId")] = er.eventId;
            row[GetIndex("eventId")] = id - 1;
            FixChecksum(row, er);
            FixHashCode(row, id);
        }

        //----------------------------------------------------------------------
        // MarkRowBad
        //----------------------------------------------------------------------
        private void MarkRowBad(DataRow row,
                                EventRecord er,
                                int id,
                                RecordStatus status)
        {
            MarkRecordBad(er, status);
            row[GetIndex("eventType")] = (int)er.eventType;
            row[GetIndex("eventCategory")] = (int)er.eventCategory;
            row[GetIndex("startTime")] = er.startTime;
            FixChecksum(row, er);
            FixHashCode(row, id);
        }

        //----------------------------------------------------------------------
        // MarkRecordBad
        //----------------------------------------------------------------------
        private EventRecord MarkRecordBad(EventRecord er, RecordStatus status)
        {
            switch (status)
            {
                case RecordStatus.RecordModified:
                    er.eventType = TraceEventType.ModifiedEvent;
                    //er.details = CoreConstants.Integrity_ModifiedRecordsFound;
                    break;
                case RecordStatus.HashCodeModified:
                    er.eventType = TraceEventType.ModifiedEvent;
                    //er.details = CoreConstants.Integrity_ModifiedRecordsFound;
                    break;
                case RecordStatus.RecordDeleted:
                    er.eventType = TraceEventType.MissingEvents;
                    //er.details = CoreConstants.Integrity_MissingRecords;
                    break;
                case RecordStatus.RecordAdded:
                    er.eventType = TraceEventType.InsertedEvent;
                    //er.details = CoreConstants.Integrity_NewRecordsFound;
                    break;
            }
            er.eventCategory = TraceEventCategory.Corrupted;
            return er;
        }

        //----------------------------------------------------------------------
        // GetNextId
        //----------------------------------------------------------------------
        private void GetNextId(DataTable table, int rowNumber, int currentId)
        {
            if (rowNumber < table.Rows.Count - 1)
            {
                next = (int)table.Rows[rowNumber + 1]["eventId"];
            }
            else
            {
                next = currentId + 1;
            }
        }

        //----------------------------------------------------------------------
        // GetNextId
        //----------------------------------------------------------------------
        private DataRow GetNextRow(DataTable table, int rowNumber)
        {
            if (rowNumber < table.Rows.Count - 1)
            {
                return table.Rows[rowNumber + 1];
            }
            else
            {
                return null;
            }
        }

        //----------------------------------------------------------------------
        // GetBadRecord: rebuilds the chain after archiving.
        //----------------------------------------------------------------------
        public EventRecord GetBadRecord(string instance,
                                        string inDatabase,
                                        out RecordStatus status,
                                        bool inIsArchive)
        {
            EventRecord badRecord = null;
            DataTable table;

            status = RecordStatus.Intact;

            try
            {
                hasNextId = true;
                instanceName = instance;
                database = inDatabase;
                isArchive = inIsArchive;
                table = GetEventsTable();

                if (table == null)
                {
                    return null;
                }

                chain = new HashChain(instanceName);

                if (hasLowWatermark)
                    next = lowWatermark;
                else
                    hasNextId = false;

                foreach (DataRow row in table.Rows)
                {
                    status = CheckRow(row);
                    if (status != RecordStatus.Intact)
                    {
                        badRecord = CreateEventRecord(row);
                        break;
                    }
                }

                if (badRecord != null)
                    LowWatermark = badRecord.eventId + 1;

            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write("An error occurred retrieving bad records for " + database + " integrity .",
                                         e,
                                         ErrorLog.Severity.Warning,
                                         true);
                throw e;

            }
            finally
            {
                da = null;
            }

            if (table != null)
                table.Dispose();
            chain = null;
            return badRecord;

        }

        //--------------------------------------------------------------------------------
        // FixMissingHighWatermarkRecord - This function is called when the record with
        // the high watermark is archived.  
        //--------------------------------------------------------------------------------
        private void FixMissingHighWatermarkRecord(string instance,
                                                   string database,
                                                   int lastId,
                                                   int checksum,
                                                   int highWatermark)
        {
            lock (EventId.AcquireInstanceLock(instance))
            {
                int nextId = GetNextRecordIdFromDatabase(instance, database, lastId);

                // new events inserted or being inserted during chain rebuild
                if (EventId.IsInsertingBlock(instance) ||
                    nextId >= (highWatermark + 1))
                {
                    if (nextId <= highWatermark)
                        nextId = highWatermark + 1;
                    HashChain chain = new HashChain(instance);
                    using (SqlConnection conn = GetConnection())
                    {
                        conn.Open();
                        string sql = String.Format("UPDATE {0}..{1} SET hash = {2} where eventId = {3}",
                                                   SQLHelpers.CreateSafeDatabaseName(database),
                                                   CoreConstants.RepositoryEventsTable,
                                                   chain.GetHashCode(nextId, checksum),
                                                   lastId);

                        using (SqlCommand cmd = new SqlCommand(sql, conn))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                else
                {
                    HashChain chain = new HashChain(instance);
                    using (SqlConnection conn = GetConnection())
                    {
                        conn.Open();
                        nextId = highWatermark + 1;
                        string sql = String.Format("UPDATE {0}..{1} SET hash = {2} where eventId = {3}",
                                             SQLHelpers.CreateSafeDatabaseName(database),
                                             CoreConstants.RepositoryEventsTable,
                                             chain.GetHashCode(nextId, checksum),
                                             lastId);

                        using (SqlCommand cmd = new SqlCommand(sql, conn))
                        {
                            cmd.ExecuteNonQuery();
                        }

                        // Insert a dummy record to hold the high watermark
                        EventRecord nr = new EventRecord();
                        nr.startTime = GetCurrentTime();
                        nr.serverName = instance.ToUpper();
                        nr.eventId = nextId;
                        nr.applicationName = CoreConstants.CollectionServerName;
                        nr.eventType = TraceEventType.DummyEvent;
                        nr.eventCategory = TraceEventCategory.Corrupted;
                        nr.details = CoreConstants.Integrity_DummyRecord;
                        nr.checksum = nr.GetHashCode();
                        nr.hash = chain.GetHashCode(nextId + 1, nr.checksum);
                        nr.Insert(conn, database);
                    }
                    EventId.WriteHighWatermark(instance, nextId);
                }
            }
        }

        //--------------------------------------------------------------------------------
        // GetNextRecordIdFromDatabase 
        //--------------------------------------------------------------------------------
        private int GetNextRecordIdFromDatabase(string instance, string database, int currentId)
        {
            int nextId = currentId;

            string stmt = String.Format("SELECT TOP 1 eventId FROM {0}..{1} WHERE eventId > {2} ORDER BY eventId",
                                       SQLHelpers.CreateSafeDatabaseName(database),
                                       CoreConstants.RepositoryEventsTable,
                                       currentId);

            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                SqlCommand command = new SqlCommand(stmt, conn);
                command.CommandTimeout = CoreConstants.sqlcommandTimeout;
                object obj = command.ExecuteScalar();
                if (obj != null && !(obj is System.DBNull))
                    nextId = (int)obj;
                conn.Close();
            }
            return nextId;
        }


        //----------------------------------------------------------------------
        // FixRecord
        //----------------------------------------------------------------------
        public bool FixRecord(EventRecord er, RecordStatus status)
        {
            int nextId;
            bool success = true;

            chain = new HashChain(instanceName);

            try
            {
                switch (status)
                {
                    case RecordStatus.HashCodeModified:
                        nextId = GetNextId(er.eventId);
                        er.hash = chain.GetHashCode(nextId, er.checksum);
                        er.eventType = TraceEventType.ModifiedEvent;
                        er.eventCategory = TraceEventCategory.Corrupted;
                        er.details = CoreConstants.Integrity_ModifiedRecordsFound;
                        er.Update();
                        break;

                    case RecordStatus.RecordAdded:
                        UpdatePreviousRecord(er.eventId);
                        nextId = GetNextId(er.eventId);
                        er.checksum = er.GetHashCode();
                        er.hash = chain.GetHashCode(nextId, er.checksum);
                        er.eventType = TraceEventType.InsertedEvent;
                        er.eventCategory = TraceEventCategory.Corrupted;
                        er.details = CoreConstants.Integrity_NewRecordsFound;
                        er.Update();
                        break;

                    case RecordStatus.RecordDeleted:
                        EventRecord nr = new EventRecord();
                        nr.startTime = er.startTime;
                        nr.serverName = er.serverName;
                        nr.eventId = er.eventId - 1;
                        nr.applicationName = CoreConstants.CollectionServerName;
                        nr.eventType = TraceEventType.MissingEvents;
                        nr.eventCategory = TraceEventCategory.Corrupted;
                        er.details = CoreConstants.Integrity_MissingRecords;
                        nr.checksum = nr.GetHashCode();
                        nr.Insert(GetConnection(), database);
                        UpdatePreviousRecord(nr.eventId);
                        break;

                    case RecordStatus.RecordModified:
                        nextId = GetNextId(er.eventId);
                        er.hash = chain.GetHashCode(nextId, er.checksum);
                        er.eventType = TraceEventType.ModifiedEvent;
                        er.eventCategory = TraceEventCategory.Corrupted;
                        er.details = CoreConstants.Integrity_ModifiedRecordsFound;
                        er.checksum = er.GetHashCode();
                        er.Update();
                        break;
                }
                SetContainsBadEventsFlag();
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write("An error occurred setting bad record content.", e, ErrorLog.Severity.Warning, true);
                success = false;
            }
            return success;
        }

        #endregion

        #region Populate and Update Table
        //----------------------------------------------------------------------
        // GetEventsTable
        //----------------------------------------------------------------------
        private DataTable GetEventsTable()
        {
            DataTable table;
            string stmt = "";
            try
            {
                stmt = GetRetrieveRecordStmt();
                da = new SqlDataAdapter(stmt, GetConnectionString());
                da.SelectCommand.CommandTimeout = CoreConstants.sqlcommandTimeout;
                table = new DataTable();
                da.Fill(table);
                columnMappings.Clear();

                foreach (DataColumn column in table.Columns)
                {
                    try
                    {
                        columnMappings.Add(column.ColumnName, column.Ordinal);
                    }
                    catch { }
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(String.Format("An error occurred retrieving records for integrity check. Stmt: {0}", stmt),
                                         e,
                                         ErrorLog.Severity.Warning);
                throw e;
            }
            return table;
        }

        //----------------------------------------------------------------------
        // GetAddedEventsAtTail
        //----------------------------------------------------------------------
        private DataTable GetAddedEventsAtTail(int id)
        {
            DataTable table;
            string stmt = "";
            try
            {
                stmt = GetRetrieveAddedRecordStmt(id);
                da = new SqlDataAdapter(stmt, GetConnectionString());
                da.SelectCommand.CommandTimeout = CoreConstants.sqlcommandTimeout;
                table = new DataTable();
                da.Fill(table);
                table.Columns.Add("oldId", typeof(int));
                columnMappings.Clear();

                foreach (DataColumn column in table.Columns)
                {
                    try
                    {
                        columnMappings.Add(column.ColumnName, column.Ordinal);
                    }
                    catch { }
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(String.Format("GetAddedEventsAtTail():An error occurred retrieving records for integrity check. Stmt: {0}", stmt),
                                         e,
                                         ErrorLog.Severity.Warning);
                throw e;
            }
            return table;
        }

        //----------------------------------------------------------------------
        // GetAddedEventsCount
        //----------------------------------------------------------------------
        private int GetAddedEventsAtTailCount(int id)
        {
            string stmt = "";
            int count;
            try
            {
                stmt = GetRetrieveAddedRecordCountStmt(id);

                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    SqlCommand command = new SqlCommand(stmt, conn);
                    command.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    count = (int)command.ExecuteScalar();
                    conn.Close();
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(String.Format("GetAddedEventsCount():An error occurred retrieving record count for integrity check. Stmt: {0}", stmt),
                                         e,
                                         ErrorLog.Severity.Warning);
                throw e;
            }
            return count;
        }

        //----------------------------------------------------------------------
        // UpdateTableHash: update the hash code after the chain is rebuilt.
        //----------------------------------------------------------------------
        private void UpdateTableHash(DataTable table)
        {
            try
            {
                using (SqlCommand cmd = CreateUpdateHashCommand())
                {
                    da.UpdateCommand = cmd;
                    da.Update(table);
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, "An error occurred updating the table after chain is rebuilt.", e, true);
                throw e;
            }
        }

        private void UpdateSCTable(DataTable scTable, List<SensitiveColumnRecord> badSCRecords)
        {
            SqlDataAdapter da;
            Repository rep = new Repository();
            int numRows = 0;
            int inserted = 0;

            try
            {
                da = new SqlDataAdapter("", GetConnectionString());
                using (SqlCommand cmd = CreateSCUpdateCommand())
                {
                    da.UpdateCommand = cmd;
                    numRows = da.Update(scTable);
                }

                if (badSCRecords.Count > 0)
                {
                    rep.OpenConnection();

                    foreach (SensitiveColumnRecord scRecord in badSCRecords)
                    {
                        try
                        {
                            scRecord.InsertWithId(rep.connection, database);
                            inserted++;
                        }
                        catch (Exception ex)
                        {
                            string msg = String.Format("An error occurred inserting missing sensitive column records.\n" +
                                                        "Record number: {0}\n" +
                                                        "StartTime: {1}\n" +
                                                        "Hash: {2}",
                                                        scRecord.scId,
                                                        scRecord.startTime,
                                                        scRecord.hashcode);

                            ErrorLog.Instance.Write(ErrorLog.Level.Debug, msg, ex, true);
                            throw ex;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                        "An error occurred updating the sensitive column table.",
                                        e,
                                        true);
                throw e;
            }
            finally
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                        String.Format("Integrity check: {0} sensitive column records are updated.  {1} sensitive column records are inserted.",
                                        numRows,
                                        inserted));
                rep.CloseConnection();
            }
        }

        private void UpdateDCTable(DataTable dcTable, List<DataChangeRecord> badDCRecords)
        {
            SqlDataAdapter da;
            Repository rep = new Repository();
            int numRows = 0;
            int inserted = 0;

            try
            {
                da = new SqlDataAdapter("", GetConnectionString());
                using (SqlCommand cmd = CreateDCUpdateCommand())
                {
                    da.UpdateCommand = cmd;
                    numRows = da.Update(dcTable);
                }

                if (badDCRecords.Count > 0)
                {
                    rep.OpenConnection();

                    foreach (DataChangeRecord dcRecord in badDCRecords)
                    {
                        try
                        {
                            dcRecord.InsertWithId(rep.connection, database);
                            inserted++;
                        }
                        catch (Exception ex)
                        {
                            string msg = String.Format("An error occurred inserting missing data change records.\n" +
                                                        "Record number: {0}\n" +
                                                        "StartTime: {1}\n" +
                                                        "Hash: {2}",
                                                        dcRecord.dcId,
                                                        dcRecord.startTime,
                                                        dcRecord.hashcode);

                            ErrorLog.Instance.Write(ErrorLog.Level.Debug, msg, ex, true);
                            throw ex;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                        "An error occurred updating the data changes table.",
                                        e,
                                        true);
                throw e;
            }
            finally
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                        String.Format("Integrity check: {0} data change records are updated.  {1} data change records are inserted.",
                                        numRows,
                                        inserted));
                rep.CloseConnection();
            }
        }

        private void UpdateCCTable(DataTable ccTable, List<ColumnChangeRecord> badCCRecords)
        {
            SqlDataAdapter da;
            Repository rep = new Repository();
            int numRows = 0;
            int inserted = 0;

            try
            {
                da = new SqlDataAdapter("", GetConnectionString());
                using (SqlCommand cmd = CreateCCUpdateCommand())
                {
                    da.UpdateCommand = cmd;
                    numRows = da.Update(ccTable);
                }

                if (badCCRecords.Count > 0)
                {
                    rep.OpenConnection();

                    foreach (ColumnChangeRecord ccRecord in badCCRecords)
                    {
                        try
                        {
                            ccRecord.InsertWithId(rep.connection, database);
                            inserted++;
                        }
                        catch (Exception ex)
                        {
                            string msg = String.Format("An error occurred inserting missing column change records.\n" +
                                                        "Record number: {0}\n" +
                                                        "StartTime: {1}\n" +
                                                        "Hash: {2}",
                                                        ccRecord.dcId,
                                                        ccRecord.startTime,
                                                        ccRecord.hashcode);

                            ErrorLog.Instance.Write(ErrorLog.Level.Debug, msg, ex, true);
                            throw ex;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                        "An error occurred updating the column changes table.",
                                        e,
                                        true);
                throw e;
            }
            finally
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                        String.Format("Integrity check: {0} column changes records are updated.  {1} column changes records are inserted.",
                                        numRows,
                                        inserted));
                rep.CloseConnection();
            }
        }

        //----------------------------------------------------------------------
        // UpdateTable: update the table after the chain is rebuilt.
        //----------------------------------------------------------------------
        private void UpdateTable(DataTable table)
        {
            Repository rep = new Repository();
            int numRows = 0;
            int inserted = 0;

            try
            {
                using (SqlCommand cmd = CreateUpdateCommand())
                {
                    da.UpdateCommand = cmd;
                    numRows = da.Update(table);
                }

                if (badRecords.Count > 0)
                {
                    EventRecord er = null;
                    rep.OpenConnection();

                    foreach (DataRow row in badRecords)
                    {
                        try
                        {
                            er = CreateEventRecord(row);
                            er.Insert(rep.connection, database);
                            inserted++;
                        }
                        catch (Exception ex)
                        {
                            string msg = String.Format("An error occurred inserting missing record event.\n" +
                                                        "Record number: {0}\n" +
                                                        "StartTime: {1}\n" +
                                                        "Checksum: {2}",
                                                        er.eventId,
                                                        er.startTime,
                                                        er.checksum);

                            ErrorLog.Instance.Write(ErrorLog.Level.Debug, msg, ex, true);
                            throw ex;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                        "An error occurred updating the table after chain is rebuilt.",
                                        e,
                                        true);
                throw e;
            }
            finally
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                        String.Format("Integrity check: {0} records are updated.  {1} records are inserted.",
                                        numRows,
                                        inserted));
                rep.CloseConnection();
            }
        }

        //----------------------------------------------------------------------
        // UpdateTable: update the table after the chain is rebuilt.
        //----------------------------------------------------------------------
        private void UpdateAddedTable(DataTable table)
        {
            Repository rep = new Repository();
            int numRows = 0;

            try
            {

                using (SqlCommand cmd = CreateUpdateAddedCommand())
                {
                    da.UpdateCommand = cmd;
                    numRows = da.Update(table);
                }

            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         "An error occurred updating the added record table.",
                                         e,
                                         true);
                throw e;
            }
            finally
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                        String.Format("Integrity check: {0} added records are updated.",
                                        numRows));
                rep.CloseConnection();
            }
        }
        #endregion

        #region Instance and Instance Database
        //----------------------------------------------------------------------
        // GetAllInstance
        //----------------------------------------------------------------------
        private string[] GetAllInstance()
        {
            ArrayList instanceList = new ArrayList();
            Repository rep = null;
            try
            {
                rep = new Repository();

                rep.OpenConnection();
                string query = GetAllInstanceQuery();

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
                ErrorLog.Instance.Write("An error occurred retrieving instances for integrity check.",
                                         e,
                                         ErrorLog.Severity.Warning,
                                         true);
                throw e;
            }
            finally
            {
                rep.CloseConnection();
            }
            return (string[])instanceList.ToArray(typeof(string));
        }

        //----------------------------------------------------------------------
        // GetInstanceDatabase
        //----------------------------------------------------------------------
        private string GetInstanceDatabase(string instance)
        {
            string databaseName = null;
            Repository rep = null;
            try
            {
                rep = new Repository();

                rep.OpenConnection();
                string query = GetInstanceDatabaseQuery(instance);

                using (SqlCommand command = new SqlCommand(query, rep.connection))
                {
                    command.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader != null)
                        {
                            if (reader.Read())
                            {
                                if (!reader.IsDBNull(0))
                                    databaseName = reader.GetString(0);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write("An error occurred retrieving instance database for integrity check.",
                                         e,
                                         ErrorLog.Severity.Warning,
                                         true);
                throw e;
            }
            finally
            {
                if (rep != null)
                    rep.CloseConnection();
            }
            return databaseName;
        }

        #endregion

        #region ID and Watermarks
        //----------------------------------------------------------------------
        // GetNextId
        //----------------------------------------------------------------------
        private int GetNextId(int currentId)
        {
            string stmt = GetNextRecordIdQuery(currentId);
            int nextId = currentId + 1;
            Repository rep = null;

            try
            {
                rep = new Repository();
                rep.OpenConnection();

                using (SqlCommand command = new SqlCommand(stmt, rep.connection))
                {
                    command.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    object obj = command.ExecuteScalar();
                    if (obj is System.DBNull)
                    {
                        UpdateHighwWatermark(currentId);
                    }
                    else
                    {
                        nextId = (int)obj;
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(String.Format("An error occurred retrieving next record ID. Stmt: {0}", stmt),
                                         e,
                                         ErrorLog.Severity.Warning,
                                         true);
            }
            finally
            {
                if (rep != null) rep.CloseConnection();
            }

            return nextId;
        }

        //----------------------------------------------------------------------
        // GetHighWatermark: retrieve current high watermark from the database.
        //----------------------------------------------------------------------
        private int GetHighWatermark()
        {
            string query;
            SqlConnection conn;
            Repository rep;
            int high = -2100000000;

            try
            {

                if (isArchive)
                {
                    conn = new SqlConnection(GetConnectionString(database));
                    conn.Open();
                    query = GetArchiveHighWatermarksStmt();
                }
                else
                {
                    rep = new Repository();
                    rep.OpenConnection();
                    conn = rep.connection;
                    query = GetHighWatermarksStmt();
                }

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    object obj = command.ExecuteScalar();

                    if (!(obj is System.DBNull))
                    {
                        high = (int)obj;
                    }
                    conn.Close();
                }
            }
            catch (Exception e)
            {
                string msg = String.Format("An error occurred retrieving high watermarks for database {0}.", database);
                ErrorLog.Instance.Write(msg, e, true);
                throw e;
            }
            return high;
        }

        //----------------------------------------------------------------------
        // GetWatermarks: retrieve current watermarks from the database.
        //----------------------------------------------------------------------
        private bool GetWatermarks()
        {
            bool success = true;
            string query;
            SqlConnection conn;
            Repository rep;

            lowWatermark = -2100000000;
            highWatermark = -2100000000;

            try
            {

                if (isArchive)
                {
                    conn = new SqlConnection(GetConnectionString(database));
                    conn.Open();
                    query = GetArchiveWatermarksStmt();
                }
                else
                {
                    rep = new Repository();
                    rep.OpenConnection();
                    conn = rep.connection;
                    query = GetLowWatermarkStmt();
                }

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader != null)
                        {
                            if (reader.Read())
                            {
                                if (reader.IsDBNull(0))
                                {
                                    lowWatermark = -2100000000;
                                }
                                else
                                {
                                    lowWatermark = reader.GetInt32(0);
                                }

                                if (isArchive)
                                {
                                    if (reader.IsDBNull(1))
                                    {
                                        highWatermark = -2100000000;
                                    }
                                    else
                                    {
                                        highWatermark = reader.GetInt32(1);
                                    }
                                }
                            }
                            else
                            {
                                lowWatermark = -2100000000;
                                highWatermark = -2100000000;
                            }
                        }
                    }
                    conn.Close();
                }

                if (!isArchive)
                    highWatermark = EventId.GetHighWatermark(instanceName);
            }
            catch (Exception e)
            {
                string msg = String.Format("An error occurred retrieving watermarks for database {0}.", database);
                ErrorLog.Instance.Write(msg, e, true);
                throw e;
            }
            return success;
        }


        //----------------------------------------------------------------------
        // UpdateLowWatermark
        //----------------------------------------------------------------------
        private bool UpdateLowWatermark(int id)
        {
            bool success = true;

            try
            {
                Execute(GetUpdateLowWatermarkStmt(id));
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write("An error occurred updating low watermark.", e, ErrorLog.Severity.Warning, true);
                success = false;
            }
            return success;
        }

        //----------------------------------------------------------------------
        // UpdateHighWatermark
        //----------------------------------------------------------------------
        private void UpdateHighwWatermark(int id)
        {
            try
            {
                Execute(GetUpdateHighWatermarkStmt(id));
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write("An error occurred updating high watermark.", e, ErrorLog.Severity.Warning, true);
                throw e;
            }
        }

        //----------------------------------------------------------------------
        // UpdateArchiveLowWatermark : updates low watermark for an
        //       archive database after an integrity check or the hashchain is rebuilt.
        //----------------------------------------------------------------------
        private bool UpdateArchiveLowWatermark(int low)
        {
            bool success = true;

            try
            {
                using (SqlConnection conn = new SqlConnection(GetConnectionString(database)))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(GetUpdateArchiveLowWatermarkStmt(low), conn))
                    {
                        command.CommandTimeout = CoreConstants.sqlcommandTimeout;
                        command.ExecuteNonQuery();
                    }
                    conn.Close();
                }
            }
            catch (Exception e)
            {
                string msg = String.Format("An error occurred updating low watermark for archive database {0}.", database);
                ErrorLog.Instance.Write(msg, e, true);
                success = false;
            }
            return success;
        }

        //----------------------------------------------------------------------
        // UpdateArchiveLowWatermark : updates low watermark for an
        //       archive database after an integrity check or the hashchain is rebuilt.
        //----------------------------------------------------------------------
        private bool UpdateArchiveHighWatermark(int id)
        {
            bool succeeded = true;

            try
            {
                using (SqlConnection conn = new SqlConnection(GetConnectionString(database)))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(GetUpdateArchiveHighWatermarkStmt(id), conn))
                    {
                        command.CommandTimeout = CoreConstants.sqlcommandTimeout;
                        command.ExecuteNonQuery();
                    }
                    conn.Close();
                }
            }
            catch (Exception e)
            {
                string msg = String.Format("An error occurred updating high watermark for archive database {0}.", database);
                ErrorLog.Instance.Write(msg, e, true);
                succeeded = false;
            }
            return succeeded;
        }



        //----------------------------------------------------------------------
        // UpdateArchiveWatermarks : updates low and high watermarks for an
        //       archive database after the hashchain is rebuilt.
        //----------------------------------------------------------------------
        private bool UpdateArchiveWatermarks(string database, int low, int high)
        {
            bool success = true;

            try
            {
                using (SqlConnection conn = new SqlConnection(GetConnectionString(database)))
                {
                    conn.Open();

                    using (SqlCommand command = new SqlCommand(GetUpdateArchiveWatermarksStmt(low, high), conn))
                    {
                        command.CommandTimeout = CoreConstants.sqlcommandTimeout;
                        command.ExecuteNonQuery();
                    }
                    conn.Close();
                }
            }
            catch (Exception e)
            {
                string msg = String.Format("An error occurred updating watermarks for archive database {0}.", database);
                ErrorLog.Instance.Write(msg, e, true);
                success = false;
            }
            return success;
        }

        #endregion

        #region Alerting and Set Integrity Flags

        //----------------------------------------------------------------------
        // RaiseAlert
        //----------------------------------------------------------------------
        private void RaiseAlert(string msg)
        {
            InternalAlert.Raise(this.instanceName, msg);
        }

        //----------------------------------------------------------------------
        // SetBadIntegrityFlags
        //----------------------------------------------------------------------
        private void SetBadIntegrityFlags()
        {
            try
            {
                Execute(GetSetBadIntegrityFlagsSQL());
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         "An error occurred updating integrity check flags for " + database,
                                         e,
                                         true);
            }
        }

        //----------------------------------------------------------------------
        // SetBadIntegrityFlags
        //----------------------------------------------------------------------
        private void ResetBadIntegrityFlags()
        {
            try
            {
                Execute(GetResetBadIntegrityFlagsSQL());
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         "An error occurred reseting integrity check flags for " + database,
                                         e,
                                         true);
            }
        }

        //----------------------------------------------------------------------
        // SetContainsBadEventsFlag
        //----------------------------------------------------------------------
        private void SetContainsBadEventsFlag()
        {
            try
            {
                Execute(GetSetContainsBadEventsSQL());
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         "An error occurred updating containsBadEvents flag for " + database,
                                         e,
                                         true);
            }
        }

        #endregion

        #region Connection Related
        //----------------------------------------------------------------------
        // GetConnectionString
        //----------------------------------------------------------------------
        private string GetConnectionString()
        {
            return GetConnectionString(database);
        }

        //----------------------------------------------------------------------
        // GetConnectionString
        //----------------------------------------------------------------------
        private string GetConnectionString(string dbName)
        {

            return String.Format("server={0};" +
                                   "database = {1};" +
                                   "integrated security=SSPI;" +
                                   "Connect Timeout=30;" +
                                   "Application Name='{2}';",
                                   Repository.ServerInstance,
                                   SQLHelpers.CreateSafeDatabaseNameForConnectionString(dbName),
                                   CoreConstants.DefaultSqlApplicationName);
        }

        //----------------------------------------------------------------------
        // GetConnectionString
        //----------------------------------------------------------------------
        private SqlConnection GetConnection()
        {
            return new SqlConnection(GetConnectionString());
        }

        #endregion

        #region SqlCommand and Helpers
        //----------------------------------------------------------------------
        // CreateUpdateHashCommand: for rebuilding the chain after archiving.
        //----------------------------------------------------------------------
        private SqlCommand CreateUpdateHashCommand()
        {
            string stmt = GetUpdateHashStmt();

            SqlCommand command = new SqlCommand(stmt, GetConnection());
            command.CommandTimeout = CoreConstants.sqlcommandTimeout;
            command.Parameters.Add("@hashcode", SqlDbType.Int, 0, "hash");
            command.Parameters.Add("@eid", SqlDbType.Int, 0, "eventId");
            return command;
        }

        //----------------------------------------------------------------------
        // CreateUpdateCommand: for rebuilding the chain.
        //----------------------------------------------------------------------
        private SqlCommand CreateUpdateCommand()
        {
            string stmt = GetUpdateStmt();
            SqlCommand command = new SqlCommand(stmt, GetConnection());

            command.CommandTimeout = CoreConstants.sqlcommandTimeout;
            command.Parameters.Add("@neweid", SqlDbType.Int, 0, "eventId");
            command.Parameters.Add("@hashcode", SqlDbType.Int, 0, "hash");
            command.Parameters.Add("@checksum", SqlDbType.Int, 0, "checksum");
            command.Parameters.Add("@eventType", SqlDbType.Int, 0, "eventType");
            command.Parameters.Add("@eventCategory", SqlDbType.Int, 0, "eventCategory");
            command.Parameters.Add("@details", SqlDbType.NVarChar, 512, "details");
            command.Parameters.Add("@startTime", SqlDbType.DateTime, 0, "startTime");
            command.Parameters.Add("@eid", SqlDbType.Int, 0, "eventId");
            return command;
        }

        private SqlCommand CreateDCUpdateCommand()
        {
            string stmt = GetDCUpdateStmt();
            SqlCommand command = new SqlCommand(stmt, GetConnection());

            command.CommandTimeout = CoreConstants.sqlcommandTimeout;
            command.Parameters.Add("@hashcode", SqlDbType.Int, 0, "hashcode");
            command.Parameters.Add("@dcId", SqlDbType.BigInt, 0, "dcId");
            return command;
        }

        private SqlCommand CreateCCUpdateCommand()
        {
            string stmt = GetCCUpdateStmt();
            SqlCommand command = new SqlCommand(stmt, GetConnection());

            command.CommandTimeout = CoreConstants.sqlcommandTimeout;
            command.Parameters.Add("@hashcode", SqlDbType.Int, 0, "hashcode");
            command.Parameters.Add("@ccId", SqlDbType.BigInt, 0, "ccId");
            return command;
        }

        private SqlCommand CreateSCUpdateCommand()
        {
            string stmt = GetSCUpdateStmt();
            SqlCommand command = new SqlCommand(stmt, GetConnection());

            command.CommandTimeout = CoreConstants.sqlcommandTimeout;
            command.Parameters.Add("@hashcode", SqlDbType.Int, 0, "hashcode");
            command.Parameters.Add("@scId", SqlDbType.Int, 0, "scId");
            return command;
        }

        //----------------------------------------------------------------------
        // CreateUpdateCommand: for rebuilding the chain.
        //----------------------------------------------------------------------
        private SqlCommand CreateUpdateAddedCommand()
        {
            string stmt = GetUpdateAddedStmt();
            SqlCommand command = new SqlCommand(stmt, GetConnection());

            command.CommandTimeout = CoreConstants.sqlcommandTimeout;
            command.Parameters.Add("@hashcode", SqlDbType.Int, 0, "hash");
            command.Parameters.Add("@checksum", SqlDbType.Int, 0, "checksum");
            command.Parameters.Add("@eventType", SqlDbType.Int, 0, "eventType");
            command.Parameters.Add("@eventCategory", SqlDbType.Int, 0, "eventCategory");
            command.Parameters.Add("@startTime", SqlDbType.DateTime, 0, "startTime");
            command.Parameters.Add("@eid", SqlDbType.Int, 0, "eventId");
            command.Parameters.Add("@oldId", SqlDbType.Int, 0, "oldId");
            return command;
        }

        //----------------------------------------------------------------------
        // Execute: executes an action SQL statement without result sets.
        //----------------------------------------------------------------------
        private void Execute(string stmt)
        {
            Repository rep = null;

            try
            {
                rep = new Repository();
                rep.OpenConnection();

                using (SqlCommand command = new SqlCommand(stmt, rep.connection))
                {
                    command.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                string msg = String.Format("An error occurred executing the SQL statement.  Statement: {0}.", stmt);
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, msg, e, true);
                throw e;
            }
            finally
            {
                if (rep != null)
                    rep.CloseConnection();
            }
        }

        #endregion

        #region Event Record
        //----------------------------------------------------------------------
        // UpdatePreviousRecord
        //----------------------------------------------------------------------
        private void UpdatePreviousRecord(int eventId)
        {
            EventRecord pr = GetPreviousRecord(eventId);

            if (pr != null)
            {
                pr.hash = chain.GetHashCode(eventId, pr.checksum);
                pr.Update();
            }
            else
            {
                UpdateLowWatermark(eventId);
            }
        }

        //----------------------------------------------------------------------
        // GetPreviousRecord
        //----------------------------------------------------------------------
        private EventRecord GetPreviousRecord(int eventId)
        {
            int id;
            Repository rep = new Repository();

            rep.OpenConnection();
            string query = GetPreviousRecordStmt(eventId);

            using (SqlCommand command = new SqlCommand(query, rep.connection))
            {
                command.CommandTimeout = CoreConstants.sqlcommandTimeout;
                object obj = command.ExecuteScalar();
                if (obj is System.DBNull)
                {
                    return null;
                }
                else
                {
                    id = (int)obj;
                }
            }
            EventRecord record = new EventRecord(rep.connection, database);
            record.Read(id);
            return record;
        }

        //----------------------------------------------------------------------
        // GetFirstIdToCheck
        //----------------------------------------------------------------------
        private int GetFirstIdToCheck()
        {
            int id;

            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                string sql = "DECLARE @minEventId int\n" +
                               "SET @minEventId = (SELECT min(eventId) FROM  {0}..{1} where eventId >= {2})\n" +
                               "if(@minEventId IS NULL)\n" +
                               "	SET @minEventId = (SELECT min(eventId) FROM  {0}..{1})\n" +
                               "SELECT @minEventId";
                string query = String.Format(sql,
                                             SQLHelpers.CreateSafeDatabaseName(database),
                                             CoreConstants.RepositoryEventsTable,
                                             lowWatermark);
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.CommandTimeout = CoreConstants.sqlcommandTimeout;

                    object obj = command.ExecuteScalar();
                    if (obj is System.DBNull)
                    {
                        id = int.MaxValue;
                    }
                    else
                        id = (int)obj;
                }
                conn.Close();
            }
            return id;
        }

        //----------------------------------------------------------------------
        // GetLastIdToCheck
        //----------------------------------------------------------------------
        private int GetLastIdToCheck(int high)
        {
            int id;

            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                string query = String.Format("SELECT MAX( eventId ) FROM {0}..{1} where eventId <= {2}",
                                     SQLHelpers.CreateSafeDatabaseName(database),
                                     CoreConstants.RepositoryEventsTable,
                                     high <= lowWatermark && lowWatermark <= firstId ? int.MaxValue : high);
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.CommandTimeout = CoreConstants.sqlcommandTimeout;

                    object obj = command.ExecuteScalar();
                    if (obj is System.DBNull)
                        id = int.MinValue;
                    else
                        id = (int)obj;
                }
                conn.Close();
            }
            return id;
        }

        //----------------------------------------------------------------------
        // GetFirstIdToCheck
        //----------------------------------------------------------------------
        private int GetLastIdToCheck()
        {
            int id;

            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                string query = String.Format("SELECT MAX( eventId ) FROM {0}..{1}",
                                     SQLHelpers.CreateSafeDatabaseName(database),
                                     CoreConstants.RepositoryEventsTable);
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.CommandTimeout = CoreConstants.sqlcommandTimeout;

                    object obj = command.ExecuteScalar();
                    if (obj is System.DBNull)
                        id = int.MinValue;
                    else
                        id = (int)obj;
                }

                conn.Close();
            }
            return id;
        }
        #endregion

        #region Column Index
        //----------------------------------------------------------------------
        // GetIndex: get column index from column name
        //----------------------------------------------------------------------
        private int GetIndex(string columnName)
        {
            return (int)columnMappings[columnName];
        }

        #endregion

        #region EventRecord and DataRow Copy/Create
        //----------------------------------------------------------------------
        // CreateEventRecord: populates values from a DataRow to an EventRecord.
        //----------------------------------------------------------------------
        protected void CopyToRow(EventRecord er, DataRow row)
        {
            row[GetIndex("eventType")] = (int)er.eventType;
            row[GetIndex("eventCategory")] = (int)er.eventCategory;
            row[GetIndex("targetObject")] = er.targetObject;
            row[GetIndex("details")] = er.details;
            row[GetIndex("hash")] = er.hash;
            row[GetIndex("eventClass")] = er.eventClass;
            row[GetIndex("eventSubclass")] = er.eventSubclass;
            row[GetIndex("startTime")] = er.startTime;
            row[GetIndex("spid")] = er.spid;
            row[GetIndex("applicationName")] = er.applicationName;
            row[GetIndex("hostName")] = er.hostName;
            row[GetIndex("serverName")] = er.serverName;
            row[GetIndex("loginName")] = er.loginName;
            row[GetIndex("success")] = er.success;
            row[GetIndex("databaseName")] = er.databaseName;
            row[GetIndex("databaseId")] = er.databaseId;
            row[GetIndex("dbUserName")] = er.dbUserName;
            row[GetIndex("objectType")] = er.objectType;
            row[GetIndex("objectName")] = er.objectName;
            row[GetIndex("objectId")] = er.objectId;
            row[GetIndex("permissions")] = er.permissions;
            row[GetIndex("columnPermissions")] = er.columnPermissions;
            row[GetIndex("targetLoginName")] = er.targetLoginName;
            row[GetIndex("targetUserName")] = er.targetUserName;
            row[GetIndex("roleName")] = er.roleName;
            row[GetIndex("ownerName")] = er.ownerName;
            row[GetIndex("alertLevel")] = er.alertLevel;
            row[GetIndex("checksum")] = er.checksum;
            row[GetIndex("privilegedUser")] = er.privilegedUser;
            row[GetIndex("eventId")] = er.eventId;
            row[GetIndex("fileName")] = er.fileName;
            row[GetIndex("linkedServerName")] = er.linkedServerName;
            row[GetIndex("parentName")] = er.parentName;
            row[GetIndex("isSystem")] = er.isSystem;
            row[GetIndex("sessionLoginName")] = er.sessionLoginName;
            row[GetIndex("providerName")] = er.providerName;
            row[GetIndex("appNameId")] = er.appNameId;
            row[GetIndex("hostId")] = er.hostId;
            row[GetIndex("loginId")] = er.loginId;
            row[GetIndex("endTime")] = er.endTime;
            row[GetIndex("startSequence")] = er.startSequence;
            row[GetIndex("endSequence")] = er.endSequence;
        }

        //----------------------------------------------------------------------
        // CreateEventRecord: populates values from a DataRow to an EventRecord.
        //----------------------------------------------------------------------
        protected EventRecord CreateEventRecord(DataRow row)
        {
            EventRecord er = new EventRecord();

            er.eventId = GetRowInt32(row, GetIndex("eventId"));
            er.eventType = (TraceEventType)GetRowInt32(row, GetIndex("eventType"));
            er.eventCategory = (TraceEventCategory)GetRowInt32(row, GetIndex("eventCategory"));
            er.targetObject = GetRowString(row, GetIndex("targetObject"));
            er.details = GetRowString(row, GetIndex("details"));
            er.hash = GetRowInt32(row, GetIndex("hash"));
            er.eventClass = GetRowInt32(row, GetIndex("eventClass"));
            er.eventSubclass = GetRowInt32(row, GetIndex("eventSubclass"));
            er.startTime = GetRowDateTime(row, GetIndex("startTime"));
            er.spid = GetRowInt32(row, GetIndex("spid"));
            er.applicationName = GetRowString(row, GetIndex("applicationName"));
            er.hostName = GetRowString(row, GetIndex("hostName"));
            er.serverName = GetRowString(row, GetIndex("serverName")).TrimEnd(null);
            er.loginName = GetRowString(row, GetIndex("loginName"));
            er.success = GetRowInt32(row, GetIndex("success"));
            er.databaseName = GetRowString(row, GetIndex("databaseName"));
            er.databaseId = GetRowInt32(row, GetIndex("databaseId"));
            er.dbUserName = GetRowString(row, GetIndex("dbUserName"));
            er.objectType = GetRowInt32(row, GetIndex("objectType"));
            er.objectName = GetRowString(row, GetIndex("objectName"));
            er.objectId = GetRowInt32(row, GetIndex("objectId"));
            er.permissions = GetRowInt32(row, GetIndex("permissions"));
            er.columnPermissions = GetRowInt32(row, GetIndex("columnPermissions"));
            er.targetLoginName = GetRowString(row, GetIndex("targetLoginName"));
            er.targetUserName = GetRowString(row, GetIndex("targetUserName"));
            er.roleName = GetRowString(row, GetIndex("roleName"));
            er.ownerName = GetRowString(row, GetIndex("ownerName"));
            er.alertLevel = GetRowInt32(row, GetIndex("alertLevel"));
            er.checksum = GetRowInt32(row, GetIndex("checksum"));
            er.privilegedUser = GetRowInt32(row, GetIndex("privilegedUser"));
            er.fileName = GetRowString(row, GetIndex("fileName"));
            er.linkedServerName = GetRowString(row, GetIndex("linkedServerName"));
            er.parentName = GetRowString(row, GetIndex("parentName"));
            er.isSystem = GetRowInt32(row, GetIndex("isSystem"));
            er.sessionLoginName = GetRowString(row, GetIndex("sessionLoginName"));
            er.providerName = GetRowString(row, GetIndex("providerName"));
            er.appNameId = GetRowInt32(row, GetIndex("appNameId"));
            er.hostId = GetRowInt32(row, GetIndex("hostId")); ;
            er.loginId = GetRowInt32(row, GetIndex("loginId"));
            er.endTime = GetRowDateTime(row, GetIndex("endTime"));
            er.startSequence = GetRowInt64(row, GetIndex("startSequence"));
            er.endSequence = GetRowInt64(row, GetIndex("endSequence"));
            if (row["rowCounts"] != DBNull.Value)
                er.rowCounts = GetRowInt64(row, GetIndex("rowCounts"));
            return er;
        }

        protected DataChangeRecord CreateDataChangeRecord(DataRow row)
        {
            DataChangeRecord dcr = new DataChangeRecord();

            dcr.startTime = GetRowDateTime(row, GetIndex("startTime"));
            dcr.eventSequence = GetRowInt64(row, GetIndex("eventSequence"));
            dcr.spid = GetRowInt32(row, GetIndex("spid"));
            dcr.databaseId = GetRowInt32(row, GetIndex("databaseId"));
            dcr.actionType = GetRowInt32(row, GetIndex("actionType"));
            dcr.schemaName = GetRowString(row, GetIndex("schemaName"));
            dcr.tableName = GetRowString(row, GetIndex("tableName"));
            dcr.recordNumber = GetRowInt32(row, GetIndex("recordNumber"));
            dcr.user = GetRowString(row, GetIndex("userName"));
            dcr.changedColumns = GetRowInt32(row, GetIndex("changedColumns"));
            dcr.primaryKey = GetRowString(row, GetIndex("primaryKey"));
            dcr.hashcode = GetRowInt32(row, GetIndex("hashcode"));
            dcr.tableId = GetRowInt32(row, GetIndex("tableId"));
            dcr.dcId = GetRowInt64(row, GetIndex("dcId"));
            dcr.eventId = GetRowInt32(row, GetIndex("eventId"));
            dcr.totalChanges = GetRowInt32(row, GetIndex("totalChanges"));
            return dcr;
        }

        protected SensitiveColumnRecord CreateSensitiveColumnRecord(DataRow row)
        {
            SensitiveColumnRecord scr = new SensitiveColumnRecord();

            scr.startTime = GetRowDateTime(row, GetIndex("startTime"));
            scr.eventId = GetRowInt32(row, GetIndex("eventId"));
            scr.columnName = GetRowString(row, GetIndex("columnName"));
            scr.hashcode = GetRowInt32(row, GetIndex("hashcode"));
            scr.tableId = GetRowInt32(row, GetIndex("tableId"));
            scr.columnId = GetRowInt32(row, GetIndex("columnId"));
            scr.scId = GetRowInt32(row, GetIndex("scId"));

            return scr;
        }

        protected ColumnChangeRecord CreateColumnChangeRecord(DataRow row)
        {
            ColumnChangeRecord ccr = new ColumnChangeRecord();

            ccr.startTime = GetRowDateTime(row, GetIndex("startTime"));
            ccr.eventSequence = GetRowInt64(row, GetIndex("eventSequence"));
            ccr.spid = GetRowInt32(row, GetIndex("spid"));
            ccr.columnName = GetRowString(row, GetIndex("columnName")); ;
            ccr.beforeValue = GetRowString(row, GetIndex("beforeValue")); ;
            ccr.afterValue = GetRowString(row, GetIndex("afterValue")); ;
            ccr.hashcode = GetRowInt32(row, GetIndex("hashcode")); ;
            ccr.columnId = GetRowInt32(row, GetIndex("columnId")); ;
            ccr.dcId = GetRowInt64(row, GetIndex("dcId")); ;
            ccr.ccId = GetRowInt64(row, GetIndex("ccId")); ;
            return ccr;
        }
        #endregion

        #region SQL
        //----------------------------------------------------------------------
        // GetRetrieveRecordStmt
        //----------------------------------------------------------------------
        private string GetRetrieveRecordStmt()
        {
            string whereCluase;
            string query;

            whereCluase = String.Format(" e.eventId >= {0} and e.eventId <= {1}", firstId, lastId);
            query = EventRecord.GetSelectSQL(database,
                                                CoreConstants.RepositoryEventsTable,
                                                whereCluase,
                                                "ORDER BY e.eventId",
                                                true);
            return query.Replace("'SELECT", String.Format("'SELECT TOP {0}", checkBatchSize));
        }

        //----------------------------------------------------------------------
        // GetRetrieveAddedRecordStmt
        //----------------------------------------------------------------------
        private string GetRetrieveAddedRecordStmt(int id)
        {
            string whereCluase;
            string query;

            whereCluase = String.Format(" (eventId > {0} AND {0} > {1}) OR  (eventId > {0} AND {0} < {1} AND eventId < {1}) ", id, lowWatermark);
            query = EventRecord.GetSelectSQL(database,
                                                CoreConstants.RepositoryEventsTable,
                                                whereCluase,
                                                "ORDER BY e.eventId",
                                                true);
            return query.Replace("'SELECT", String.Format("'SELECT TOP {0}", checkBatchSize));
        }

        //----------------------------------------------------------------------
        // GetRetrieveAddedRecordCountStmt
        //----------------------------------------------------------------------
        private string GetRetrieveAddedRecordCountStmt(int id)
        {
            return String.Format("SELECT count(*) from {0}..{1} where (eventId > {2} AND {2} > {3}) OR  (eventId > {2} AND {2} < {3} AND eventId < {3})",
                                 SQLHelpers.CreateSafeDatabaseName(database),
                                 CoreConstants.RepositoryEventsTable,
                                 id,
                                 lowWatermark);
        }
        //----------------------------------------------------------------------
        // GetAllInstanceQuery
        //----------------------------------------------------------------------
        private string GetPreviousRecordStmt(int currentId)
        {
            string whereCluase = String.Format(" e.eventId < {0} ", currentId);
            return EventRecord.GetSelectSQL(database,
                                             CoreConstants.RepositoryEventsTable,
                                             whereCluase,
                                             "ORDER BY e.eventId",
                                             true);
        }

        //----------------------------------------------------------------------
        // GetInstanceDatabaseQuery
        //----------------------------------------------------------------------
        private string GetAllInstanceQuery()
        {
            return String.Format("SELECT instance FROM {0} WHERE isEnabled = 1 AND isDeployed = 1",
                                  CoreConstants.RepositoryServerTable);
        }

        //----------------------------------------------------------------------
        // GetAllInstanceQuery
        //----------------------------------------------------------------------
        private string GetInstanceDatabaseQuery(string instance)
        {
            return String.Format("SELECT eventDatabase FROM {0} WHERE instance = {1}",
                                  CoreConstants.RepositoryServerTable,
                                  SQLHelpers.CreateSafeString(instance));
        }

        //----------------------------------------------------------------------
        // GetNextRecordIdQuery
        //----------------------------------------------------------------------
        private string GetNextRecordIdQuery(int currentId)
        {
            return String.Format("SELECT MIN( eventId ) FROM {0}..{1} WHERE eventId > {2}",
                                  SQLHelpers.CreateSafeDatabaseName(database),
                                  CoreConstants.RepositoryEventsTable,
                                  currentId);
        }

        //----------------------------------------------------------------------
        // GetUpdateHighWatermarkStmt
        //----------------------------------------------------------------------
        private string GetUpdateHighWatermarkStmt(int high)
        {
            return String.Format("UPDATE {0} SET highWatermark = {1} WHERE instance = {2}",
                                  CoreConstants.RepositoryServerTable,
                                  high,
                                  SQLHelpers.CreateSafeString(instanceName));
        }

        //----------------------------------------------------------------------
        // GetUpdateLowWatermarkStmt
        //----------------------------------------------------------------------
        private string GetUpdateLowWatermarkStmt(int low)
        {
            return String.Format("UPDATE {0} SET lowWatermark = {1} WHERE instance = {2}",
                                  CoreConstants.RepositoryServerTable,
                                  low,
                                  SQLHelpers.CreateSafeString(instanceName));

        }

        //----------------------------------------------------------------------
        // GetUpdateHashStmt
        //----------------------------------------------------------------------
        private string GetUpdateHashStmt()
        {
            return String.Format("UPDATE {0} SET hash = @hashcode where eventId = @eid", CoreConstants.RepositoryEventsTable);
        }

        //----------------------------------------------------------------------
        // GetUpdateStmt
        //----------------------------------------------------------------------
        private string GetUpdateStmt()
        {
            return String.Format("UPDATE {0} SET hash = @hashcode" +
                                    ", checksum = @checksum" +
                                    ", eventType = @eventType" +
                                    ", eventCategory = @eventCategory" +
                                    ", details = @details" +
                                    ", startTime = @startTime" +
                                    " where eventId = @eid",
                                   CoreConstants.RepositoryEventsTable);
        }

        private string GetDCUpdateStmt()
        {
            return String.Format("UPDATE {0} SET hashcode = @hashcode where dcId = @dcId",
                                 CoreConstants.RepositoryDataChangesTable);
        }

        private string GetCCUpdateStmt()
        {
            return String.Format("UPDATE {0} SET hashcode = @hashcode where ccId = @ccId",
                                 CoreConstants.RepositoryColumnChangesTable);
        }

        private string GetSCUpdateStmt()
        {
            return String.Format("UPDATE {0} SET hashcode = @hashcode where scId = @scId",
                                 CoreConstants.RepositorySensitiveColumnsTable);
        }

        //----------------------------------------------------------------------
        // GetUpdateStmt
        //----------------------------------------------------------------------
        private string GetUpdateAddedStmt()
        {
            return String.Format("UPDATE {0} SET hash = @hashcode" +
                                    ", checksum = @checksum" +
                                    ", eventType = @eventType" +
                                    ", eventCategory = @eventCategory" +
                                    ", startTime = @startTime" +
                                    ", eventId = @eid" +
                                    " where eventId = @oldId",
                                   CoreConstants.RepositoryEventsTable);
        }

        //----------------------------------------------------------------------
        // GetSetContainsBadEventsSQL
        //----------------------------------------------------------------------
        private string GetSetContainsBadEventsSQL()
        {
            if (isArchive)
                return String.Format("UPDATE {0}..{1} SET containsBadEvents = 1",
                                        SQLHelpers.CreateSafeDatabaseName(database),
                                        CoreConstants.RepositoryArchiveMetaTable);
            else
                return String.Format("UPDATE {0}..{1} SET containsBadEvents = 1 WHERE instance = {2}",
                                      CoreConstants.RepositoryDatabase,
                                      CoreConstants.RepositoryServerTable,
                                      SQLHelpers.CreateSafeString(instanceName));
        }

        //----------------------------------------------------------------------
        // GetSetBadIntegrityFlagsSQL
        //----------------------------------------------------------------------
        private string GetSetBadIntegrityFlagsSQL()
        {
            return GetSetIntegrityFlagsSQL(1);  // has bad events
        }

        //----------------------------------------------------------------------
        // GetSetBadIntegrityFlagsSQL
        //----------------------------------------------------------------------
        private string GetResetBadIntegrityFlagsSQL()
        {
            return GetSetIntegrityFlagsSQL(0);  // no bad events
        }

        //----------------------------------------------------------------------
        // GetSetIntegrityFlagsSQL
        //----------------------------------------------------------------------
        private string GetSetIntegrityFlagsSQL(int flag)
        {
            if (isArchive)
                return String.Format("UPDATE {0}..{1} SET containsBadEvents = {2}, eventReviewNeeded = {2}",
                                        SQLHelpers.CreateSafeDatabaseName(database),
                                        CoreConstants.RepositoryArchiveMetaTable,
                                        flag);
            else
                return String.Format("UPDATE {0}..{1} SET containsBadEvents = {3}, eventReviewNeeded = {3} WHERE instance = {2}",
                                      CoreConstants.RepositoryDatabase,
                                      CoreConstants.RepositoryServerTable,
                                      SQLHelpers.CreateSafeString(instanceName),
                                      flag);
        }

        //----------------------------------------------------------------------
        // GetWatermarksStmt
        //----------------------------------------------------------------------
        internal string GetWatermarksStmt()
        {
            return String.Format("SELECT lowWatermark, highWatermark FROM {0} WHERE instance = {1}",
                                  CoreConstants.RepositoryServerTable,
                                  SQLHelpers.CreateSafeString(instanceName));
        }

        //----------------------------------------------------------------------
        // GetLowWatermarkStmt
        //----------------------------------------------------------------------
        internal string GetLowWatermarkStmt()
        {
            return String.Format("SELECT lowWatermark FROM {0} WHERE instance = {1}",
                                  CoreConstants.RepositoryServerTable,
                                  SQLHelpers.CreateSafeString(instanceName));
        }

        //----------------------------------------------------------------------
        // GetHighWatermarksStmt
        //----------------------------------------------------------------------
        internal string GetHighWatermarksStmt()
        {
            return String.Format("SELECT highWatermark FROM {0} WHERE instance = {1}",
                                  CoreConstants.RepositoryServerTable,
                                  SQLHelpers.CreateSafeString(instanceName));
        }

        //----------------------------------------------------------------------
        // GetArchiveHighWatermarksStmt
        //----------------------------------------------------------------------
        internal string GetArchiveHighWatermarksStmt()
        {
            return String.Format("SELECT highWatermark FROM {0}..{1}",
                                  SQLHelpers.CreateSafeDatabaseName(database),
                                  CoreConstants.RepositoryArchiveMetaTable);
        }

        //----------------------------------------------------------------------
        // GetArchiveWatermarksStmt
        //----------------------------------------------------------------------
        internal string GetArchiveWatermarksStmt()
        {
            return String.Format("SELECT lowWatermark, highWatermark FROM {0}..{1}",
                                  SQLHelpers.CreateSafeDatabaseName(database),
                                  CoreConstants.RepositoryArchiveMetaTable);
        }


        //----------------------------------------------------------------------
        // UpdateCurrentHighLowWatermarksSQL
        //----------------------------------------------------------------------
        internal static string SetNewHighLowWatermarksSQL(string inDatabase, bool isArchiveDb)
        {
            return String.Format("SELECT MIN(eventId)-1, MAX(eventId) FROM {0}..{1}",
                                  SQLHelpers.CreateSafeDatabaseName(inDatabase),
                                  CoreConstants.RepositoryEventsTable);
        }

        //----------------------------------------------------------------------
        // GetUpdateArchiveWatermarksStmt
        //----------------------------------------------------------------------
        internal static string GetUpdateArchiveWatermarksStmt(int low, int high)
        {
            return String.Format("UPDATE {0} SET lowWatermark = {1}, highWatermark = {2}",
                                  CoreConstants.RepositoryArchiveMetaTable,
                                  low,
                                  high);
        }


        //----------------------------------------------------------------------
        // GetUpdateArchiveLowWatermarkStmt
        //----------------------------------------------------------------------
        internal static string GetUpdateArchiveLowWatermarkStmt(int low)
        {
            return String.Format("UPDATE {0} SET lowWatermark = {1}",
                                  CoreConstants.RepositoryArchiveMetaTable,
                                  low);
        }

        //----------------------------------------------------------------------
        // GetUpdateArchiveHighWatermarkStmt
        //----------------------------------------------------------------------
        internal static string GetUpdateArchiveHighWatermarkStmt(int high)
        {
            return String.Format("UPDATE {0} SET highWatermark = {1}",
                                  CoreConstants.RepositoryArchiveMetaTable,
                                  high);
        }

        #endregion

        #region Integrity Check status in DB

        //----------------------------------------------------------------
        // UpdateLastIntegrityCheckTimeArchive
        //----------------------------------------------------------------
        static public void UpdateLastIntegrityCheckTimeArchive(string archiveDatabase)
        {
            Repository rep = null;
            string sql;

            try
            {
                rep = new Repository();
                rep.OpenConnection();

                sql = String.Format("UPDATE {0}..{1} SET timeLastIntegrityCheck=GETUTCDATE(),lastIntegrityCheckResult=1",
                                     SQLHelpers.CreateSafeDatabaseName(archiveDatabase),
                                     CoreConstants.RepositoryArchiveMetaTable);
                using (SqlCommand cmd = new SqlCommand(sql, rep.connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                // Log the error and let the caller handle alerting, messaging etc
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, e, true);
                throw e;
            }
            finally
            {
                if (rep != null) rep.CloseConnection();
            }
        }

        //----------------------------------------------------------------
        // UpdateLastIntegrityCheckResultArchive
        //----------------------------------------------------------------
        static public void UpdateLastIntegrityCheckResultArchive(string archiveDatabase,
                                                                 int integrityCheckResult)
        {
            Repository rep = null;
            string sql;

            try
            {
                rep = new Repository();
                rep.OpenConnection();
                sql = String.Format("UPDATE {0}..{1} SET lastIntegrityCheckResult={2}",
                                     SQLHelpers.CreateSafeDatabaseName(archiveDatabase),
                                     CoreConstants.RepositoryArchiveMetaTable,
                                     integrityCheckResult);
                using (SqlCommand cmd = new SqlCommand(sql, rep.connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                // Log the error and let the caller handle alerting, messaging etc
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, e, true);
                throw e;
            }
            finally
            {
                if (rep != null) rep.CloseConnection();
            }
        }

        //----------------------------------------------------------------
        // UpdateLastIntegrityCheckTime
        //----------------------------------------------------------------
        static public void UpdateLastIntegrityCheckTime(string instance)
        {
            Repository rep = null;
            string sql;

            try
            {
                rep = new Repository();
                rep.OpenConnection();

                sql = String.Format("UPDATE {0}..{1} SET timeLastIntegrityCheck=GETUTCDATE(),lastIntegrityCheckResult=1 WHERE instance={2}",
                                     CoreConstants.RepositoryDatabase,
                                     CoreConstants.RepositoryServerTable,
                                     SQLHelpers.CreateSafeString(instance));
                using (SqlCommand cmd = new SqlCommand(sql, rep.connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                // Log the error and let the caller handle alerting, messaging etc
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, e, true);
                throw e;
            }
            finally
            {
                if (rep != null) rep.CloseConnection();
            }
        }

        //----------------------------------------------------------------
        // UpdateLastIntegrityCheckResult
        //----------------------------------------------------------------
        static public void UpdateLastIntegrityCheckResult(string instance,
                                                         int integrityCheckResult)
        {
            Repository rep = null;
            string sql;

            try
            {
                rep = new Repository();
                rep.OpenConnection();

                sql = String.Format("UPDATE {0}..{1} SET lastIntegrityCheckResult={2} WHERE instance={3}",
                                     CoreConstants.RepositoryDatabase,
                                     CoreConstants.RepositoryServerTable,
                                     integrityCheckResult,
                                     SQLHelpers.CreateSafeString(instance));
                using (SqlCommand cmd = new SqlCommand(sql, rep.connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                // Log the error and let the caller handle alerting, messaging etc
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, e, true);
                throw e;
            }
            finally
            {
                if (rep != null) rep.CloseConnection();
            }
        }

        #endregion

        #region Utilities

        //----------------------------------------------------------------
        // GetCurrentTime: this function is create to solve a data
        //                 conversion problem with DateTime.  DateTime's
        //                 hashcode is generated using Ticks.  DateTime
        //                 value in the database doesn't have this info
        //                 so the number of ticks changed after a value
        //                 is inserted and then retrieved and the checksum
        //                 check fails because of this.
        //----------------------------------------------------------------
        private DateTime GetCurrentTime()
        {
            DateTime time = DateTime.UtcNow;
            // Data conversion problem
            long ticks = time.Ticks / 10000000 * 10000000;
            time = time.AddTicks(ticks - time.Ticks);
            return time;
        }

        #endregion
    }
}
