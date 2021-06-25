using System;
using System.IO;
using System.Security.Principal;
using System.Text;
using System.Collections;

using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;

using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Event;
using System.Collections.Generic;



namespace Idera.SQLcompliance.Core.Agent
{
    /// <summary>
    /// Data struct used to store the audit stored procedure information.
    /// </summary>
    public struct AuditSPInfo
    {
        public string AgentVersion;
        public int ConfigurationVersion;
        public int TraceCount;
        public DateTime CreationTime;
    }

    /// <summary>
    /// Summary description for SPHelper.
    /// </summary>
    public class SPHelper
    {
        #region Constructors
        private SPHelper()
        {
        }
        #endregion

        #region Static Methods

        #region Stored Procedure Validation Methods

        //----------------------------------------------------------------------
        // DoesAuditSPExist
        //----------------------------------------------------------------------
        /// <summary>
        /// Checks if the audit stored procedure exists.
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static bool
           DoesAuditSPExist(SqlConnection conn)
        {
            ValidateConnection(conn);

            using (SqlCommand cmd = new SqlCommand(
               String.Format(CoreConstants.QueryStoredProcedureExistence, CoreConstants.Agent_AuditSPName),
               conn))
            {
                bool exists = false;

                object obj = cmd.ExecuteScalar();
                if (obj is System.DBNull)
                {
                    exists = false;
                }
                else
                {
                    if ((int)obj > 0) exists = true;
                }

                return (exists);
            }
        }

        public static bool DoesAuditSPExistXE(SqlConnection conn)
        {
            ValidateConnection(conn);

            using (SqlCommand cmd = new SqlCommand(
               String.Format(CoreConstants.QueryStoredProcedureExistence, CoreConstants.Agent_AuditSPNameXE),
               conn))
            {
                bool exists = false;

                object obj = cmd.ExecuteScalar();
                if (obj is System.DBNull)
                {
                    exists = false;
                }
                else
                {
                    if ((int)obj > 0) exists = true;
                }

                return (exists);
            }
        }
        //----------------------------------------------------------------------
        // DoesStartupSPExist
        //----------------------------------------------------------------------
        /// <summary>
        /// Checks if the startup stored procedure exists.
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static bool
           DoesStartupSPExist(SqlConnection conn)
        {
            ValidateConnection(conn);

            using (SqlCommand cmd = new SqlCommand(
                    String.Format(CoreConstants.QueryStoredProcedureExistence, CoreConstants.Agent_StartUpSPName),
                    conn))
            {
                bool exists = false;

                object obj = cmd.ExecuteScalar();
                if (obj is System.DBNull)
                {
                    exists = false;
                }
                else
                {
                    if ((int)obj > 0) exists = true;
                }

                return (exists);
            }
        }

        public static bool
           DoesSPExist(SqlConnection conn, string spName)
        {
            ValidateConnection(conn);

            using (SqlCommand cmd = new SqlCommand(
                    String.Format(CoreConstants.QueryStoredProcedureExistence, spName),
                    conn))
            {
                bool exists = false;

                object obj = cmd.ExecuteScalar();
                if (obj is System.DBNull)
                {
                    exists = false;
                }
                else
                {
                    if ((int)obj > 0) exists = true;
                }

                return (exists);
            }
        }

        //----------------------------------------------------------------------
        // IsAuditSPCurrent
        //----------------------------------------------------------------------
        /// <summary>
        /// Returns true if the configuration version is current.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="configurationVersion"></param>
        /// <returns></returns>
        public static bool
           IsAuditSPCurrent(SqlConnection conn, int configurationVersion)
        {
            return (GetCurrentAuditSPVersion(conn) >= configurationVersion);
        }

        //----------------------------------------------------------------------
        // IsAuditSPCurrent
        //----------------------------------------------------------------------
        /// <summary>
        /// Return true if both the version number and modified time are current.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="configurationVersion"></param>
        /// <param name="lastModifiedTime"></param>
        /// <returns></returns>
        public static bool
           IsAuditSPCurrent(
           SqlConnection conn,
           int configurationVersion,
           DateTime lastModifiedTime)
        {
            bool isCurrent = false;
            try
            {
                AuditSPInfo info = GetAuditSPInfo(conn);
                if (info.CreationTime >= lastModifiedTime &&
                    info.ConfigurationVersion >= configurationVersion)
                    isCurrent = true;
            }
            catch (Exception)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         "An error occurred when getting audit stored procedure information");
            }
            return isCurrent;
        }


        public static bool
         IsAuditSPCurrentXE(
         SqlConnection conn,
         int configurationVersion,
         DateTime lastModifiedTime)
        {
            bool isCurrent = false;
            try
            {
                if (DoesAuditSPExistXE(conn))
                {
                    AuditSPInfo info = GetAuditSPInfoXE(conn);
                    if (info.CreationTime >= lastModifiedTime &&
                        info.ConfigurationVersion >= configurationVersion)
                        isCurrent = true;
                }
            }
            catch (Exception)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         "An error occurred when getting audit stored procedure information");
            }
            return isCurrent;
        }

        //----------------------------------------------------------------------
        // GetCurrentAuditSPVersion
        //----------------------------------------------------------------------
        /// <summary>
        /// Get the audit stored procedure's configuration version.
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int
           GetCurrentAuditSPVersion(SqlConnection conn)
        {
            return GetAuditSPInfo(conn).ConfigurationVersion;
        }

        //----------------------------------------------------------------------
        // GetAuditSPAgentVersion
        //----------------------------------------------------------------------
        /// <summary>
        /// Get agent version who created the audit stored procedure.
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static string
           GetAuditSPAgentVersion(SqlConnection conn)
        {
            return GetAuditSPInfo(conn).AgentVersion;
        }

        //----------------------------------------------------------------------
        // GetAuditSPInfo
        //----------------------------------------------------------------------
        /// <summary>
        /// Get the audit stored procedure information.
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static AuditSPInfo
           GetAuditSPInfo(SqlConnection conn)
        {
            AuditSPInfo info = new AuditSPInfo();
            using (SqlCommand command = NewGetAuditSPInfoCommand(conn))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader != null)
                    {
                        // There is only one row returned
                        reader.Read();
                        info.CreationTime = DateTime.Parse(reader.GetString(0));
                        info.AgentVersion = reader.GetString(1);
                        info.ConfigurationVersion = reader.GetInt32(2);
                        info.TraceCount = reader.GetInt32(3);
                    }
                }
            }

            return info;
        }

        public static AuditSPInfo GetAuditSPInfoXE(SqlConnection conn)
        {
            AuditSPInfo info = new AuditSPInfo();
            using (SqlCommand command = NewGetAuditSPInfoCommandXE(conn))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader != null)
                    {
                        // There is only one row returned
                        reader.Read();
                        info.CreationTime = DateTime.Parse(reader.GetString(0));
                        info.AgentVersion = reader.GetString(1);
                        info.ConfigurationVersion = reader.GetInt32(2);
                        info.TraceCount = reader.GetInt32(3);
                    }
                }
            }

            return info;
        }

        //----------------------------------------------------------------------
        // AuditStoredProceduresHealthCheck
        //----------------------------------------------------------------------
        /// <summary>
        /// Checks if the startup and the audit stored procedures are current.  Recreate
        /// them if it is out dated or missing.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="agentVersion"></param>
        /// <param name="configVersion"></param>
        /// <param name="configList"></param>
        /// <returns></returns>
        public static bool
           AuditStoredProceduresHealthCheck(
              string instanceAlias,
              SqlConnection conn,
              string agentVersion,
              int configVersion,
              TraceConfiguration[] configList,
              DateTime lastModifiedTime,
              string traceDirectory,
              int maxTraceSize,
              int maxUnattendedTime,
              TraceOption traceOptions,
              int sqlVersion,
              XeTraceConfiguration[] xeconfigList,
              AuditLogConfiguration[] auditConfigList
           )
        {
            // Flag indicates whether we need to restart the running traces
            bool restartTrace = true;
            bool created = false;
            string sDbName = conn.Database;
            conn.ChangeDatabase("master");


            // Check if the main audit SP is healthy.
            if (DoesAuditSPExist(conn) == true || DoesAuditSPExistXE(conn) == true || DoesSPExist(conn, CoreConstants.Agent_AuditLogSPName))
            {
                // Validate and compare versions
                if (!IsAuditSPCurrent(conn, configVersion, lastModifiedTime)
                    || (xeconfigList != null && xeconfigList.Length > 0 && !IsAuditSPCurrentXE(conn, configVersion, lastModifiedTime))
                    || (auditConfigList != null && auditConfigList.Length > 0 && !IsAuditLogSPCurrent(conn, configVersion, lastModifiedTime)))
                {
                    RecreateStoredProcedures(conn,
                                               instanceAlias,
                                               agentVersion,
                                               configVersion,
                                               configList,
                                               lastModifiedTime,
                                               traceDirectory,
                                               maxTraceSize,
                                               maxUnattendedTime,
                                               traceOptions,
                                               sqlVersion,
                                               xeconfigList,
                                               auditConfigList);

                    created = true;
                }
                else
                    restartTrace = false;  // It is healthy, no need to restart the traces
            }
            else // Doesn't exist.  Create it.
            {
                CreateAuditSP(conn,
                               instanceAlias,
                               agentVersion,
                               configVersion,
                               configList,
                               lastModifiedTime,
                               traceDirectory,
                               maxTraceSize,
                               maxUnattendedTime,
                               traceOptions);
                if (xeconfigList != null && xeconfigList.Length > 0)
                {
                    CreateAuditSPXE(conn,
                                 instanceAlias,
                                 agentVersion,
                                 configVersion,
                                 lastModifiedTime,
                                 traceDirectory,
                                 maxTraceSize,
                                 maxUnattendedTime,
                                 traceOptions,
                                 xeconfigList);
                }
                if (auditConfigList != null && auditConfigList.Length > 0)
                {
                    CreateAuditLogSP(conn,
                                 instanceAlias,
                                 agentVersion,
                                 configVersion,
                                 lastModifiedTime,
                                 traceDirectory,
                                 maxTraceSize,
                                 maxUnattendedTime,
                                 traceOptions,
                                 auditConfigList);
                }
            }

            // Query SQL Server if the startup SP is there
            // Pass down exists to alter StartupSP if already exists to persist permissions
            var exists = DoesStartupSPExist(conn);
            if (!created && !exists)
            {
                // Create the startup SP
                CreateStartupSP(instanceAlias, conn, agentVersion, configVersion, traceDirectory, sqlVersion, exists);
            }
            SetSPPermissions(CoreConstants.Agent_StartUpSPName,
                              sqlVersion,
                              conn);

            var auditSpExists = DoesAuditSPExist(conn);
            if (auditSpExists)
                SetSPPermissions(CoreConstants.Agent_AuditSPName,
                                  sqlVersion,
                                  conn);

            conn.ChangeDatabase(sDbName);

            return restartTrace;
        }
        #endregion

        #region Trace Management and Status Methods

        //----------------------------------------------------------------------
        // 
        //----------------------------------------------------------------------
        /// <summary>
        /// Starts all the configured traces.
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static TraceInfo[]
           StartAllTraces(
           SqlConnection conn
           )
        {
            ArrayList traces = new ArrayList();

            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                     "Starting all configured traces.");

            using (SqlCommand command = NewStartAllTraceCommand(conn))
            {
                command.CommandTimeout = SQLcomplianceAgent.Instance.TraceStartTimeout;

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    DateTime startTime = DateTime.Now;

                    if (reader != null)
                    {
                        TraceInfo info = null;
                        while (reader.Read())
                        {
                            info = new TraceInfo();
                            GetTraceInfoFromReader(info, reader);
                            info.StartTime = startTime;
                            traces.Add(info);
                        }
                    }
                }
            }

            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                     String.Format("All {0} configured traces started.", traces.Count));

            return (TraceInfo[])traces.ToArray(typeof(TraceInfo));
        }

        public static TraceInfo[]
         StartAllTracesXE(
         SqlConnection conn
         )
        {

            //SQLCM-5514: Fix, Skip method process wheter Agent_AuditSPNameXE stored procedure is not find.
            if (!DoesAuditSPExistXE(conn))
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                          "Agent_AuditSPNameXE Stored Procedure was not found in the audited Server: " + conn.DataSource);
                return null;
            }

            ArrayList traces = new ArrayList();

            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                     "Starting all configured traces.");
            try
            {
                using (SqlCommand command = NewStartAllTraceCommandXE(conn))
                {
                    command.CommandTimeout = SQLcomplianceAgent.Instance.TraceStartTimeout;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        DateTime startTime = DateTime.Now;

                        if (reader != null)
                        {
                            TraceInfo info = null;
                            while (reader.Read())
                            {
                                info = new TraceInfo();
                                GetTraceInfoFromReader(info, reader);
                                info.StartTime = startTime;
                                traces.Add(info);
                            }
                        }
                    }
                }

                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         String.Format("All {0} configured traces started for extended events.", traces.Count));

                return (TraceInfo[])traces.ToArray(typeof(TraceInfo));
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Default,
                                    "Error running exec [sp_SQLcompliance_AuditXE]",
                                    e,
                                    true);
            }
            return null;
        }

        //----------------------------------------------------------------------
        // 
        //----------------------------------------------------------------------
        /// <summary>
        /// Starts a configured trace.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="traceNumber"></param>
        /// <returns></returns>
        public static TraceInfo
           StartTrace(
           SqlConnection conn,
           int traceNumber
           )
        {
            TraceInfo info = null;

            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                     String.Format("Starting trace number {0}", traceNumber));

            using (SqlCommand command = NewStartTraceCommand(conn, traceNumber))
            {
                command.CommandTimeout = SQLcomplianceAgent.Instance.TraceStartTimeout;

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    DateTime startTime = DateTime.Now;

                    if (reader != null)
                    {
                        if (reader.Read())
                        {
                            info = new TraceInfo();
                            info.StartTime = startTime;
                            GetTraceInfoFromReader(info, reader);
                            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                                     String.Format("Trace {0} started", traceNumber));

                        }
                    }
                }
            }

            return info;
        }

        //5.4 XE
        public static TraceInfo
         StartTraceXE(
         SqlConnection conn,
         int traceNumber
         )
        {
            TraceInfo info = null;

            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                     String.Format("Starting trace number {0}", traceNumber));

            using (SqlCommand command = NewStartTraceCommandXE(conn, traceNumber))
            {
                command.CommandTimeout = SQLcomplianceAgent.Instance.TraceStartTimeout;

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    DateTime startTime = DateTime.Now;

                    if (reader != null)
                    {
                        if (reader.Read())
                        {
                            info = new TraceInfo();
                            info.StartTime = startTime;
                            GetTraceInfoFromReader(info, reader);
                            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                                     String.Format("Trace {0} started", traceNumber));

                        }
                    }
                }
            }

            return info;
        }

        //----------------------------------------------------------------------
        // 
        //----------------------------------------------------------------------
        /// <summary>
        /// Stops a list of traces.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="traceIds"></param>
        public static void
           StopTraces(
           SqlConnection conn,
           int[] traceIds
           )
        {

            if (traceIds == null || traceIds.Length == 0)
                return;

            Hashtable ids = new Hashtable();
            StringBuilder idList = new StringBuilder();

            for (int i = 0; i < traceIds.Length; i++)
            {
                try
                {
                    ids.Add(traceIds[i], traceIds[i]);               // Eliminating duplicate trace ids
                    idList.AppendFormat("{0},", traceIds[i]);
                }
                catch { }
            }

            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                     String.Format("Stopping traces: {0}.", idList.ToString()));

            using (SqlCommand command = NewStopTracesCommand(conn, idList.ToString()))
            {
                command.ExecuteNonQuery();
            }

            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                     String.Format("Stopped traces: {0}.", idList.ToString()));
        }

        //5.4 XE
        public static void
         StopTracesXE(
         SqlConnection conn,
         int[] traceIds
         )
        {

            if (traceIds == null || traceIds.Length == 0)
                return;

            Hashtable ids = new Hashtable();
            StringBuilder idList = new StringBuilder();

            for (int i = 0; i < traceIds.Length; i++)
            {
                try
                {
                    ids.Add(traceIds[i], traceIds[i]);               // Eliminating duplicate trace ids
                    idList.AppendFormat("{0},", traceIds[i]);
                }
                catch { }
            }

            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                     String.Format("Stopping traces: {0}.", idList.ToString()));

            using (SqlCommand command = NewStopTracesCommandXE(conn, idList.ToString()))
            {
                command.ExecuteNonQuery();
            }

            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                     String.Format("Stopped traces: {0}.", idList.ToString()));
        }
        //----------------------------------------------------------------------
        // 
        //----------------------------------------------------------------------
        /// <summary>
        /// Stops a trace.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="traceId"></param>
        public static void
           StopTrace(
           SqlConnection conn,
           int traceId
           )
        {
            using (SqlCommand command = NewStopTracesCommand(conn, traceId.ToString()))
            {
                command.ExecuteNonQuery();
            }
        }

        //5.4 XE
        public static void
         StopTraceXE(
         SqlConnection conn,
         int traceId
         )
        {
            using (SqlCommand command = NewStopTracesCommandXE(conn, traceId.ToString()))
            {
                command.ExecuteNonQuery();
            }
        }

        //----------------------------------------------------------------------
        // StopAllTraces
        //----------------------------------------------------------------------
        /// <summary>
        /// Stop all SQLsecure traces.
        /// </summary>
        /// <param name="conn"></param>
        public static void
           StopAllTraces(
              SqlConnection conn
           )
        {
            using (SqlCommand command = CreateAuditSPCommand(conn, SPCmd.StopAll, null))
            {
                command.ExecuteNonQuery();
            }
        }

        public static void
         StopAllTracesXE(
            SqlConnection conn
         )
        {
            using (SqlCommand command = CreateAuditSPCommandXE(conn, SPCmd.StopAll, null))
            {
                command.ExecuteNonQuery();
            }
        }
        //----------------------------------------------------------------------
        // 
        //----------------------------------------------------------------------
        /// <summary>
        /// Get the current status for a list of traces.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="traces"></param>
        public static void
           GetTraceStatus(
           SqlConnection conn,
           TraceInfo[] traces
           )
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < traces.Length; i++)
            {
                builder.AppendFormat("{0},", traces[i].TraceId);
                // Initialize the status to unknown
                traces[i].Status = TraceStatus.Unknown;
            }

            // query SQL Server to get the current trace status
            TraceInfo[] traceStatus = GetTraceInfo(conn, builder.ToString());

            // Loop through the list and set current status
            // Note that the number of returned TraceInfo can be less than requested
            for (int i = 0, j = 0; i < traceStatus.Length && j < traces.Length; i++, j++)
            {
                // Skip if IDs are different
                while (traceStatus[i].TraceId != traces[j].TraceId &&
                   j < traces.Length)
                    j++;

                if (j < traces.Length &&
                   traceStatus[i].FileName.ToUpper() == traces[j].FileName.ToUpper())
                {
                    // Copy the relevant trace info over.  Not everything.
                    traces[j].Status = traceStatus[i].Status;
                    traces[j].MaxSize = traceStatus[i].MaxSize;
                    traces[j].Options = traceStatus[i].Options;
                    traces[j].StopTime = traceStatus[i].StopTime;
                }
            }
        }

        //----------------------------------------------------------------------
        // GetTraceStatus
        //----------------------------------------------------------------------
        /// <summary>
        /// Get the trace's current status.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="traceId"></param>
        /// <returns></returns>
        public static TraceInfo
           GetTraceStatus(
           SqlConnection conn,
           int traceId
           )
        {
            TraceInfo[] traceStatus = GetTraceInfo(conn, traceId.ToString());

            if (traceStatus != null)
                return traceStatus[0];
            else
                return null;
        }

        //----------------------------------------------------------------------
        // GetRunningTraces - returns an array of integer running trace IDs
        //----------------------------------------------------------------------
        public static int[]
           GetRunningTraces(
              SqlConnection conn
           )
        {
            ArrayList traceIds = new ArrayList();

            using (SqlCommand command = CreateAuditSPCommand(conn, SPCmd.GetRunningTraces, null))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            if (!reader.IsDBNull(0))
                                traceIds.Add(reader.GetInt32(0));
                        }
                    }
                }
            }
            return (int[])traceIds.ToArray(typeof(int));
        }

        //----------------------------------------------------------------------
        // GetRunningTraces for XE - returns an array of integer running trace IDs
        //----------------------------------------------------------------------
        public static int[]
           GetRunningTracesXE(
              SqlConnection conn
           )
        {
            ArrayList traceIds = new ArrayList();

            if (!DoesAuditSPExistXE(conn))
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                        "Agent_AuditSPNameXE Stored Procedure was not found in the audited Server: " + conn.DataSource);
                return (int[])traceIds.ToArray(typeof(int));
            }

            try
            {

                using (SqlCommand command = CreateAuditSPCommandXE(conn, SPCmd.GetRunningTraces, null))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                if (!reader.IsDBNull(0))
                                    traceIds.Add(reader.GetInt32(0));
                            }
                        }
                    }
                }

                return (int[])traceIds.ToArray(typeof(int));
            }
            catch (Exception ex) { return null; }
        }



        public static void
           GetTraceSettings(SqlConnection conn,
                             int traceId,
                             out ArrayList events,
                             out ArrayList columns,
                             out ArrayList filters,
                             int sqlVersion)
        {
            events = new ArrayList();
            columns = new ArrayList();
            filters = new ArrayList();
            Hashtable eventList = new Hashtable();
            Hashtable columnList = new Hashtable();
            int columnId;
            int eventId;

            string sql = String.Format("SELECT eventid, columnid from :: fn_trace_geteventinfo( {0} )", traceId);
            using (SqlCommand command = new SqlCommand(sql, conn))
            {
                command.CommandTimeout = CoreConstants.sqlcommandTimeout;
                // Event IDs and columns
                SqlDataReader reader = command.ExecuteReader();
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                    {
                        eventId = reader.GetInt32(0);
                        columnId = reader.GetInt32(1);
                        try
                        {
                            if (!eventList.ContainsKey(eventId))
                                eventList.Add(eventId, eventId);
                        }
                        catch
                        { }

                        try
                        {
                            if (!columnList.ContainsKey(columnId))
                                columnList.Add(columnId, columnId);
                        }
                        catch
                        { }
                    }
                    reader.Close();
                }

                // Filters
                command.CommandText = String.Format("SELECT columnid, logical_operator, comparison_operator, value from :: fn_trace_getfilterinfo( {0} )", traceId);
                reader = command.ExecuteReader();

                int compOp;
                int logicOp;
                int intVal;
                string stringVal;
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                    {
                        columnId = reader.GetInt32(0);
                        logicOp = reader.GetInt32(1);
                        compOp = reader.GetInt32(2);
                        if (compOp < 6)
                        {
                            if (columnId == 19 && sqlVersion >= 9)
                            {
                                long val = reader.GetInt64(3);
                                intVal = (int)val;
                            }
                            else
                                intVal = reader.GetInt32(3);
                            filters.Add(new TraceFilter((TraceColumnId)columnId,
                                                         (TraceFilterComparisonOp)compOp,
                                                         intVal,
                                                         (TraceFilterLogicalOp)logicOp));
                        }
                        else
                        {
                            stringVal = reader.GetString(3);
                            filters.Add(new TraceFilter((TraceColumnId)columnId,
                                                          (TraceFilterComparisonOp)compOp,
                                                         stringVal,
                                                         (TraceFilterLogicalOp)logicOp));
                        }

                    }
                    reader.Close();
                }

            }
            IDictionaryEnumerator enumerator = eventList.GetEnumerator();
            while (enumerator.MoveNext())
                events.Add((TraceEventId)enumerator.Value);
            enumerator = columnList.GetEnumerator();
            while (enumerator.MoveNext())
                columns.Add((TraceColumnId)enumerator.Value);

        }

        #endregion

        #region Create/Drop Stored procedures

        //----------------------------------------------------------------------
        // 
        //----------------------------------------------------------------------
        /// <summary>
        /// Creates a startup stored procedure on the SQL Server.
        /// </summary>
        /// <param name="server"></param>
        /// <param name="conn"></param>
        /// <param name="agentVersion"></param>
        /// <param name="configurationVersion"></param>
        public static void
           CreateStartupSP(
           string instance,
           SqlConnection conn,
           string agentVersion,
           int configurationVersion,
           string traceDirectory,
           int sqlVersion,
           bool exists
         )
        {
            SPBuilder builder = new SPBuilder(sqlVersion);
            string sDbName = conn.Database;
            var isSysadmin = RawSQL.IsCurrentUserSysadmin(conn);
            try
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         "Creating startup stored procedure for " + instance);
                ValidateConnection(conn);
                conn.ChangeDatabase("master");

                builder.AgentVersion = agentVersion;
                builder.ConfigurationVersion = configurationVersion;

                // Register Startup SP cannot be done with non sysadmin user
                using (SqlCommand command = new SqlCommand(builder.CreateStartUpSP(instance, exists), conn))
                {
                    command.ExecuteNonQuery();
                }

                // Register Permissions exists only with sysadmin user
                if (isSysadmin)
                {
                    RegisterStartupSP(conn);
                }

                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         "Startup stored procedure created and registered for " + instance);
            }
            catch (Exception e)
            {
                // No need to rethrow the exception
                // TODO: need to send an alert to the server.
                if (ErrorLog.Instance.ErrorLevel >= ErrorLog.Level.UltraDebug)
                {
                    string filename = Path.Combine(traceDirectory, "SQLsecureStartupSP.sql");

                    StreamWriter w = new StreamWriter(filename, false);
                    w.Write(builder.CreateStartUpSP(instance, exists));
                    w.Close();

                    ErrorLog.Instance.Write(ErrorLog.Level.Default,
                                             String.Format(CoreConstants.Exception_ErrorCreatingStartupStoredProcedureCopySaved,
                                                            filename),
                                              e,
                                              true);
                }
                else
                {
                    ErrorLog.Instance.Write(ErrorLog.Level.Default,
                                            CoreConstants.Exception_ErrorCreatingStartupStoredProcedure,
                                            e,
                                            true);
                }

            }

            SetSPPermissions(CoreConstants.Agent_StartUpSPName,
                             sqlVersion,
                            conn);
            conn.ChangeDatabase(sDbName);


        }

        //----------------------------------------------------------------------
        // 
        //----------------------------------------------------------------------
        /// <summary>
        /// Register the startup stored procedured with SQL Server.
        /// </summary>
        /// <param name="conn"></param>
        public static void
           RegisterStartupSP(
              SqlConnection conn
           )
        {
            ValidateConnection(conn);

            using (SqlCommand command = CreateRegisterStartupSPCommand(conn, CoreConstants.Agent_StartUpSPName, true))
            {
                command.ExecuteNonQuery();

                int returnCode = (int)command.Parameters["@ReturnCode"].Value;
                if (returnCode != 0)
                {
                    throw new CoreException(String.Format(CoreConstants.Exception_ErrorRegisterStartupStoredProcedure, returnCode));
                }
            }
        }


        //----------------------------------------------------------------------
        // 
        //----------------------------------------------------------------------
        /// <summary>
        /// Drops the registered startup stored procedure.
        /// </summary>
        /// <param name="conn"></param>
        public static void
           DropStartupSP(
           SqlConnection conn
           )
        {
            // SQLCM 5.6- 566/740/4620/5280 (Non-Admin and Non-Sysadmin role) Permissions
            // Blanks the StartupSP  to persists permissions and startup execution of the stored procedure
            BlankStoredProcedure(conn, CoreConstants.Agent_StartUpSPName);
        }

        //----------------------------------------------------------------------
        // 
        //----------------------------------------------------------------------
        /// <summary>
        /// Drops the audit stored procedure.
        /// </summary>
        /// <param name="conn"></param>
        public static void
           DropAuditSP(
           SqlConnection conn
           )
        {
            DropStoredProcedure(conn, CoreConstants.Agent_AuditSPName);
        }


        public static void
         DropAuditSPXE(
         SqlConnection conn
         )
        {
            DropStoredProcedure(conn, CoreConstants.Agent_AuditSPNameXE);
        }
        //----------------------------------------------------------------------
        // 
        //----------------------------------------------------------------------
        /// <summary>
        /// Drops the stored procedure from the SQL Server.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="storedProcedureName"></param>
        public static void
           DropStoredProcedure(
           SqlConnection conn,
           string storedProcedureName
           )
        {
            ValidateConnection(conn);
            string sDbName = conn.Database;
            conn.ChangeDatabase("master");

            try
            {
                if (DoesSPExist(conn, storedProcedureName))
                {
                    using (SqlCommand command = new SqlCommand(
                       String.Format(CoreConstants.DropStoredProcedure, storedProcedureName),
                       conn))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, sqlEx, true);
                conn.ChangeDatabase(sDbName);
                throw sqlEx;
            }
            conn.ChangeDatabase(sDbName);
        }

        //----------------------------------------------------------------------
        // 
        //----------------------------------------------------------------------
        /// <summary>
        /// Blanks the stored procedure with comments in the SQL Server.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="storedProcedureName"></param>
        /// <remarks>
        /// Blanking the stored procedure prevents losing of permissions assigned for that stored procedure
        /// </remarks>
        public static void
           BlankStoredProcedure(
           SqlConnection conn,
           string storedProcedureName
           )
        {
            ValidateConnection(conn);
            string sDbName = conn.Database;
            conn.ChangeDatabase("master");

            try
            {
                if (DoesSPExist(conn, storedProcedureName))
                {
                    using (SqlCommand command = new SqlCommand(
                       String.Format(CoreConstants.BlankStoredProcedure, storedProcedureName),
                       conn))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, sqlEx, true);
                conn.ChangeDatabase(sDbName);
                throw sqlEx;
            }
            conn.ChangeDatabase(sDbName);
        }


        //----------------------------------------------------------------------
        // 
        //----------------------------------------------------------------------
        /// <summary>
        /// Create a audit stored procedure on the SQL Server using the passed in configurations.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="agentVersion"></param>
        /// <param name="configurationVersion"></param>
        /// <param name="configurations"></param>
        public static void
           CreateAuditSP(
              SqlConnection conn,
              string instanceAlias,
              string agentVersion,
              int configurationVersion,
              TraceConfiguration[] configurations,
              DateTime lastModifiedTime,
              string traceDirectory,
              int maxTraceSize,
              int maxUnattendedTime,
              TraceOption traceOptions

           )
        {

            SPBuilder builder = null;
            int sqlVersion = 8;
            string sDbName = conn.Database;
            try
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, "Creating audit stored procedure for " + instanceAlias);
                ValidateConnection(conn);
                conn.ChangeDatabase("master");

                sqlVersion = SQLInstance.GetSqlVersion(conn.ServerVersion);
                builder = new SPBuilder(sqlVersion);

                builder.AgentVersion = agentVersion;
                builder.ConfigurationVersion = configurationVersion;
                builder.TraceConfigurations = configurations;
                builder.LastModifiedTime = lastModifiedTime;
                builder.InstanceAlias = instanceAlias;
                builder.TraceDirectory = traceDirectory;
                builder.MaxTraceSize = maxTraceSize;
                builder.Options = traceOptions;
                builder.MaxUnattendedTime = maxUnattendedTime;


                using (SqlCommand command = new SqlCommand(builder.CreateAuditSP(), conn))
                {
                    command.ExecuteNonQuery();
                }

                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                        String.Format(CoreConstants.Info_AuditSPCreated,
                                        CoreConstants.Agent_AuditSPName,
                                        instanceAlias,
                                        configurationVersion,
                                        agentVersion,
                                        lastModifiedTime));
            }
            catch (Exception e)
            {
                if (ErrorLog.Instance.ErrorLevel >= ErrorLog.Level.Debug &&
                    builder != null)
                {
                    string filename = Path.Combine(traceDirectory, "SQLsecureAudit.sql");

                    StreamWriter w = new StreamWriter(filename, false);
                    w.Write(builder.CreateAuditSP());
                    w.Close();

                    ErrorLog.Instance.Write(ErrorLog.Level.Default,
                                             String.Format(CoreConstants.Exception_ErrorCreatingAuditStoredProcedureCopySaved,
                                                            filename),
                                             e,
                                             true);

                }
                else
                {
                    ErrorLog.Instance.Write(ErrorLog.Level.Default,
                                             CoreConstants.Exception_ErrorCreatingAuditStoredProcedure,
                                             e,
                                             true);
                }

                conn.ChangeDatabase(sDbName);
                throw e;
            }

            SetSPPermissions(CoreConstants.Agent_AuditSPName,
                              sqlVersion,
                              conn);
            conn.ChangeDatabase(sDbName);

        }

        public static void
        CreateAuditSPXE(
           SqlConnection conn,
           string instanceAlias,
           string agentVersion,
           int configurationVersion,
           //      TraceConfiguration[] configurations,
           DateTime lastModifiedTime,
           string traceDirectory,
           int maxTraceSize,
           int maxUnattendedTime,
           TraceOption traceOptions,
            XeTraceConfiguration[] xeconfigurations
        )
        {

            SPBuilder builder = null;
            int sqlVersion = 8;
            string sDbName = conn.Database;
            try
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, "Creating audit stored procedure for " + instanceAlias);
                ValidateConnection(conn);
                conn.ChangeDatabase("master");

                sqlVersion = SQLInstance.GetSqlVersion(conn.ServerVersion);
                builder = new SPBuilder(sqlVersion);

                builder.AgentVersion = agentVersion;
                builder.ConfigurationVersion = configurationVersion;
                //   builder.TraceConfigurations = configurations;
                builder.XeTraceConfigurations = xeconfigurations;
                builder.LastModifiedTime = lastModifiedTime;
                builder.InstanceAlias = instanceAlias;
                builder.TraceDirectory = traceDirectory;
                builder.MaxTraceSize = maxTraceSize;
                builder.Options = traceOptions;
                builder.MaxUnattendedTime = maxUnattendedTime;


                using (SqlCommand command = new SqlCommand(builder.CreateAuditSPXE(), conn))
                {
                    command.ExecuteNonQuery();
                }

                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                        String.Format(CoreConstants.Info_AuditSPCreated,
                                        CoreConstants.Agent_AuditSPName,
                                        instanceAlias,
                                        configurationVersion,
                                        agentVersion,
                                        lastModifiedTime));
            }
            catch (Exception e)
            {
                if (ErrorLog.Instance.ErrorLevel >= ErrorLog.Level.Debug &&
                    builder != null)
                {
                    string filename = Path.Combine(traceDirectory, "SQLsecureAudit.sql");

                    StreamWriter w = new StreamWriter(filename, false);
                    w.Write(builder.CreateAuditSP());
                    w.Close();

                    ErrorLog.Instance.Write(ErrorLog.Level.Default,
                                             String.Format(CoreConstants.Exception_ErrorCreatingAuditStoredProcedureCopySaved,
                                                            filename),
                                             e,
                                             true);

                }
                else
                {
                    ErrorLog.Instance.Write(ErrorLog.Level.Default,
                                             CoreConstants.Exception_ErrorCreatingAuditStoredProcedure,
                                             e,
                                             true);
                }

                conn.ChangeDatabase(sDbName);
                throw e;
            }

            SetSPPermission(CoreConstants.Agent_AuditSPNameXE,
                              sqlVersion,
                              conn);
            conn.ChangeDatabase(sDbName);

        }

        private static void SetSPPermission(string name, int version, SqlConnection conn)
        {
            string cmdText = string.Empty;
            cmdText = GetSetGrantSPPermissionStmt(name, version);
            try
            {
                using (SqlCommand command = new SqlCommand(cmdText, conn))
                {
                    // Grant EXECUTE, ALTER, CONTROL, VIEW DEFINITION, TAKE OWNERSHIP ON sp_SQLcompliance_StartUp TO PUBLIC
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Default,
                                           String.Format(CoreConstants.Exception_ErrorSettingSPPermissions, name),
                                           e,
                                           true);
            }
        }

        private static void SetSPPermissions(string name, int version, SqlConnection conn)
        {
            string cmdText = string.Empty;
            if (CoreConstants.Agent_AuditSPName == name || CoreConstants.Agent_StartUpSPName == name)
                cmdText = GetSetExecuteGrantSPPermissionStmt(name, version);
            //{
            //    cmdText = GetSetExecuteGrantSPPermissionStmt(name, version);
            //}
            //else
            //{
            //    cmdText = GetSetGrantSPPermissionStmt(name, version);
            //}

            try
            {
                using (SqlCommand command = new SqlCommand(cmdText, conn))
                {
                    // Grant EXECUTE, ALTER, CONTROL, VIEW DEFINITION, TAKE OWNERSHIP ON sp_SQLcompliance_StartUp TO PUBLIC
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Default,
                                           String.Format(CoreConstants.Exception_ErrorSettingSPPermissions, name),
                                           e,
                                           true);
            }
        }

        private static string GetGrantSPPermissionsStmt(string name, int version)
        {
            if (version < 9)
                return String.Format("GRANT ALL ON {0} TO [{1}]", name, WindowsIdentity.GetCurrent().Name);
            else
                return String.Format("GRANT EXECUTE, ALTER, CONTROL, VIEW DEFINITION, TAKE OWNERSHIP ON {0} TO [{1}]",
                                     name,
                                     WindowsIdentity.GetCurrent().Name);
        }

        private static string GetSetSPPermissionStmt(string name, int version)
        {
            if (version < 9)
                return String.Format("DENY ALL ON {0} TO PUBLIC", name);
            else
                return String.Format("DENY EXECUTE, ALTER, CONTROL, VIEW DEFINITION, TAKE OWNERSHIP ON {0} TO PUBLIC", name);
        }

        /// <Summary>
        /// Grants permissions tofor the SP to PUBLIC
        /// </Summary>
        /// <remarks>
        /// SQLCM 5.6- 566/740/4620/5280 (Non-Admin and Non-Sysadmin role) Permissions
        /// </remarks>
        private static string GetSetGrantSPPermissionStmt(string name, int version)
        {
            if (version < 9)
                return String.Format("GRANT ALL ON {0} TO PUBLIC", name);
            else
                return String.Format("GRANT EXECUTE, ALTER, CONTROL, VIEW DEFINITION, TAKE OWNERSHIP ON {0} TO PUBLIC", name);
        }

        private static string GetSetExecuteGrantSPPermissionStmt(string name, int version)
        {
            //if (version < 9)
            //    return String.Format("GRANT ALL ON {0} TO PUBLIC", name);
            //else
            return String.Format("REVOKE EXECUTE, ALTER, CONTROL, VIEW DEFINITION, TAKE OWNERSHIP ON {0} TO PUBLIC " + Environment.NewLine + " GRANT EXECUTE ON {0} TO PUBLIC", name);
        }

        //----------------------------------------------------------------------
        // RecreateStoredProcedures - recreates the startup and audit stored
        //                            procedures.
        //----------------------------------------------------------------------
        public static void
           RecreateStoredProcedures(
              SqlConnection conn,
              string instanceAlias,
              string agentVersion,
              int configVersion,
              TraceConfiguration[] configList,
              DateTime lastModifiedTime,
              string traceDirectory,
              int maxTraceSize,
              int maxUnattendedTime,
                 TraceOption traceOptions,
                 int sqlVersion,
              XeTraceConfiguration[] xeconfigList,
              AuditLogConfiguration[] auditLogConfigList
           )
        {
            try
            {
                DropStartupSP(conn);
                DropAuditSP(conn);
                if (xeconfigList == null || xeconfigList.Length <= 0)
                {
                    if (DoesSPExist(conn, CoreConstants.Agent_AuditSPNameXE))
                        StopAllTracesXE(conn);
                }
                DropAuditSPXE(conn);
                CreateStartupSP(instanceAlias, conn, agentVersion, configVersion, traceDirectory, sqlVersion, DoesStartupSPExist(conn));
                CreateAuditSP(conn,
                               instanceAlias,
                               agentVersion,
                               configVersion,
                               configList,
                               lastModifiedTime,
                               traceDirectory,
                               maxTraceSize,
                               maxUnattendedTime,
                               traceOptions);
                if (xeconfigList != null && xeconfigList.Length > 0)
                {
                    CreateAuditSPXE(conn,
                                  instanceAlias,
                                  agentVersion,
                                  configVersion,
                                  lastModifiedTime,
                                  traceDirectory,
                                  maxTraceSize,
                                  maxUnattendedTime,
                                  traceOptions,
                                  xeconfigList);
                }
                ReCreateAuditLogSP(conn,
                                  instanceAlias,
                                  agentVersion,
                                  configVersion,
                                  lastModifiedTime,
                                  traceDirectory,
                                  maxTraceSize,
                                  maxUnattendedTime,
                                  traceOptions,
                                  auditLogConfigList);
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(CoreConstants.Exception_ErrorRecreatingStoredProcedures,
                                         e,
                                         true);
            }



        }

        #endregion

        #region Utilities
        public static int
           GetSQLVersion(
              SqlConnection conn
           )
        {
            int version = 0;
            try
            {
                using (SqlCommand command = new SqlCommand("exec master..xp_msver 'ProductVersion'", conn))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader == null)
                            return version;

                        if (reader.Read() && !reader.IsDBNull(3))
                        {
                            string versionString = reader.GetString(3);
                            string[] parts = versionString.Split(".".ToCharArray());
                            version = int.Parse(parts[0]);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         "Failed to retrieve SQL Server version number",
                                         e,
                                         true);
            }

            return version;
        }
        #endregion


        #endregion

        #region Private Methods

        //----------------------------------------------------------------------
        // 
        //----------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        private static bool
           IsValidConnection(SqlConnection conn)
        {
            if (conn != null &&
               conn.State == ConnectionState.Open)
                return true;

            return false;
        }

        //----------------------------------------------------------------------
        // 
        //----------------------------------------------------------------------
        /// <summary>
        /// Verifies the connection.
        /// </summary>
        /// <param name="conn"></param>
        private static void
           ValidateConnection(SqlConnection conn)
        {
            if (conn == null)
                throw (new CoreException(CoreConstants.Exception_InvalidConnection));
            else if (conn.State != ConnectionState.Open)
                throw (new CoreException(CoreConstants.Exception_ConnectionNotOpen));
        }


        //----------------------------------------------------------------------
        // 
        //----------------------------------------------------------------------
        /// <summary>
        /// Get current trace information and status.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="traceIdList"></param>
        /// <returns></returns>
        public static TraceInfo[]
           GetTraceInfo(
           SqlConnection conn,
           string traceIdList
           )
        {
            TraceInfo[] traces = null;

            // Connection is valided in  the Get
            using (SqlCommand command = NewGetTraceStatusCommand(conn, traceIdList))
            {

                try
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader != null)
                        {
                            ArrayList traceList = new ArrayList();
                            TraceInfo info = null;
                            while (reader.Read())
                            {
                                info = new TraceInfo();
                                GetTraceInfoFromReader(info, reader);
                                traceList.Add(info);
                            }
                            traces = (TraceInfo[])traceList.ToArray(typeof(TraceInfo));
                        }
                    }
                }
                catch (Exception e)
                {
                    ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                            "Can't get trace status from stored procedure",
                                            e);
                }
            }

            return traces;
        }

        //5.4 XE
        public static TraceInfo[]
         GetTraceInfoXE(
         SqlConnection conn,
         string traceIdList
         )
        {
            TraceInfo[] traces = null;

            // Connection is valided in  the Get
            using (SqlCommand command = NewGetTraceStatusCommandXE(conn, traceIdList))
            {

                try
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader != null)
                        {
                            ArrayList traceList = new ArrayList();
                            TraceInfo info = null;
                            while (reader.Read())
                            {
                                info = new TraceInfo();
                                GetTraceInfoFromReader(info, reader);
                                traceList.Add(info);
                            }
                            traces = (TraceInfo[])traceList.ToArray(typeof(TraceInfo));
                        }
                    }
                }
                catch (Exception e)
                {
                    return null;
                }
            }

            return traces;
        }
        //----------------------------------------------------------------------
        // 
        //----------------------------------------------------------------------
        /// <summary>
        /// Creates a command to start all the configured traces.
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        private static SqlCommand
           NewStartAllTraceCommand(
              SqlConnection conn
           )
        {
            return CreateAuditSPCommand(conn, SPCmd.Start, null);
        }

        //5.4 XE
        private static SqlCommand
        NewStartAllTraceCommandXE(
           SqlConnection conn
        )
        {
            return CreateAuditSPCommandXE(conn, SPCmd.Start, null);
        }

        //----------------------------------------------------------------------
        // 
        //----------------------------------------------------------------------
        /// <summary>
        /// Creates a command to a start configured trace.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="traceNumber"></param>
        /// <returns></returns>
        private static SqlCommand
           NewStartTraceCommand(
              SqlConnection conn,
              int traceNumber
           )
        {
            return CreateAuditSPCommand(conn, SPCmd.Start, traceNumber.ToString());
        }

        //5.4 XE
        private static SqlCommand
         NewStartTraceCommandXE(
            SqlConnection conn,
            int traceNumber
         )
        {
            return CreateAuditSPCommandXE(conn, SPCmd.Start, traceNumber.ToString());
        }
        //----------------------------------------------------------------------
        // 
        //----------------------------------------------------------------------
        /// <summary>
        /// Creates command to stop traces.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="traceIdList"></param>
        /// <returns></returns>
        private static SqlCommand
              NewStopTracesCommand(
              SqlConnection conn,
              string traceIdList
           )
        {
            return CreateAuditSPCommand(conn, SPCmd.Stop, traceIdList);
        }

        //5.4 XE
        private static SqlCommand
            NewStopTracesCommandXE(
            SqlConnection conn,
            string traceIdList
         )
        {
            return CreateAuditSPCommandXE(conn, SPCmd.Stop, traceIdList);
        }

        //----------------------------------------------------------------------
        // 
        //----------------------------------------------------------------------
        /// <summary>
        /// Creates a command to get trace information.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="traceIdList"></param>
        /// <returns></returns>
        private static SqlCommand
              NewGetTraceStatusCommand(
              SqlConnection conn,
              string traceIdList
           )
        {
            return CreateAuditSPCommand(conn, SPCmd.GetStatus, traceIdList);
        }

        //5.4 XE
        private static SqlCommand
            NewGetTraceStatusCommandXE(
            SqlConnection conn,
            string traceIdList
         )
        {
            return CreateAuditSPCommandXE(conn, SPCmd.GetStatus, traceIdList);
        }

        //----------------------------------------------------------------------
        // 
        //----------------------------------------------------------------------
        /// <summary>
        /// Creates a command to get the audit stored procedure information.
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        private static SqlCommand
           NewGetAuditSPInfoCommand(
              SqlConnection conn
           )
        {
            return CreateAuditSPCommand(conn, SPCmd.GetInfo, null);
        }

        private static SqlCommand
         NewGetAuditSPInfoCommandXE(
            SqlConnection conn
         )
        {
            return CreateAuditSPCommandXE(conn, SPCmd.GetInfo, null);
        }
        //----------------------------------------------------------------------
        // 
        //----------------------------------------------------------------------
        /// <summary>
        /// CreateAuditSPCommand: Create a SqlCommand to call the audit SP.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        private static SqlCommand
           CreateAuditSPCommand(
              SqlConnection conn,
              SPCmd command,
              string option
           )
        {
            ValidateConnection(conn);

            // Create a new SqlCommand to call the audit SP
            SqlCommand cmd = new SqlCommand(CoreConstants.Agent_Full_AuditSPName, conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(new SqlParameter("@Cmd",
               SqlDbType.Int,
               0,
               "Cmd"));

            cmd.Parameters.Add(new SqlParameter("@Option",
               SqlDbType.NVarChar,
               4000,
               "Option"));

            cmd.Parameters.Add(new SqlParameter("@ReturnCode",
               SqlDbType.Int,
               0,
               ParameterDirection.ReturnValue,
               false,
               0,
               0,
               "ReturnCode",
               DataRowVersion.Default,
               null));

            cmd.Parameters["@Cmd"].Value = (int)command;
            cmd.Parameters["@Option"].Value = option;

            return cmd;
        }

        private static SqlCommand
         CreateAuditSPCommandXE(
            SqlConnection conn,
            SPCmd command,
            string option
         )
        {
            ValidateConnection(conn);

            // Create a new SqlCommand to call the audit SP
            SqlCommand cmd = new SqlCommand(CoreConstants.Agent_Full_AuditSPNameXE, conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(new SqlParameter("@Cmd",
               SqlDbType.Int,
               0,
               "Cmd"));

            cmd.Parameters.Add(new SqlParameter("@Option",
               SqlDbType.NVarChar,
               4000,
               "Option"));

            cmd.Parameters.Add(new SqlParameter("@ReturnCode",
               SqlDbType.Int,
               0,
               ParameterDirection.ReturnValue,
               false,
               0,
               0,
               "ReturnCode",
               DataRowVersion.Default,
               null));

            cmd.Parameters["@Cmd"].Value = (int)command;
            cmd.Parameters["@Option"].Value = option;

            return cmd;
        }

        //----------------------------------------------------------------------
        // 
        //----------------------------------------------------------------------
        /// <summary>
        /// Create a SqlCommand to call sp_procoption and register a startup stored procedure.
        /// </summary>
        /// <param name="conn">Connection to the SQL Server.</param>
        /// <param name="procedure">The startup stored procedure name.</param>
        /// <param name="enable">Enable or disable the startup stored procedure.</param>
        /// <returns></returns>
        private static SqlCommand
           CreateRegisterStartupSPCommand(
              SqlConnection conn,
              string procedure,
              bool enable
           )
        {
            SqlCommand cmd = new SqlCommand(CoreConstants.Agent_ProcOptionSP, conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(new SqlParameter("@ProcName",
               SqlDbType.NVarChar,
               776,
               "ProcName"));

            cmd.Parameters.Add(new SqlParameter("@OptionName",
               SqlDbType.NVarChar,
               12,
               "OptionName"));

            cmd.Parameters.Add(new SqlParameter("@OptionValue",
               SqlDbType.NVarChar,
               245,
               "OptionValue"));

            cmd.Parameters.Add(new SqlParameter("@ReturnCode",
               SqlDbType.Int,
               0,
               ParameterDirection.ReturnValue,
               false,
               0,
               0,
               "ReturnCode",
               DataRowVersion.Default,
               null));

            cmd.Parameters["@ProcName"].Value = procedure;
            cmd.Parameters["@OptionName"].Value = CoreConstants.Agent_ProcOption_Startup;
            cmd.Parameters["@OptionValue"].Value = enable.ToString();

            return cmd;
        }



        //----------------------------------------------------------------------
        // 
        //----------------------------------------------------------------------
        /// <summary>
        /// Read a trace info record from a SqlDataReader and returns a TraceInfo. 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="reader"></param>
        private static void
           GetTraceInfoFromReader(
           TraceInfo info,
           SqlDataReader reader
           )
        {
            object value;
            info.TraceNumber = reader.GetInt32(0);
            info.TraceId = reader.GetInt32(1);
            value = reader.GetValue(2);
            if (value is DBNull)
                info.FileName = null;
            else
                info.FileName = value.ToString();
            info.Options = (TraceOption)reader.GetInt32(3);
            info.MaxSize = reader.GetInt64(4);
            value = reader.GetValue(5);
            if (value is DBNull)
                info.StopTime = DateTime.Parse("1/1/0001 0:0:0");
            else
                info.StopTime = (DateTime)value;
            info.Status = (TraceStatus)reader.GetInt32(6);
        }


        #endregion

        #region Audit Logs

        /// <summary>
        /// ReCreateAuditLogSP
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="instanceAlias"></param>
        /// <param name="agentVersion"></param>
        /// <param name="configVersion"></param>
        /// <param name="lastModifiedTime"></param>
        /// <param name="traceDirectory"></param>
        /// <param name="maxTraceSize"></param>
        /// <param name="maxUnattendedTime"></param>
        /// <param name="traceOptions"></param>
        /// <param name="auditLogConfigList"></param>
        private static void ReCreateAuditLogSP(
            SqlConnection conn,
            string instanceAlias,
            string agentVersion,
            int configVersion,
            DateTime lastModifiedTime,
            string traceDirectory,
            int maxTraceSize,
            int maxUnattendedTime,
            TraceOption traceOptions,
            AuditLogConfiguration[] auditLogConfigList)
        {
            if (auditLogConfigList == null || auditLogConfigList.Length <= 0)
            {
                if (DoesSPExist(conn, CoreConstants.Agent_AuditLogSPName))
                    StopAllAuditLogs(conn);
            }
            DropAuditLogSP(conn);
            if (auditLogConfigList != null && auditLogConfigList.Length > 0)
                CreateAuditLogSP(conn,
                                  instanceAlias,
                                  agentVersion,
                                  configVersion,
                                  lastModifiedTime,
                                  traceDirectory,
                                  maxTraceSize,
                                  maxUnattendedTime,
                                  traceOptions,
                                  auditLogConfigList);
        }

        public static void
        CreateAuditLogSP(
           SqlConnection conn,
           string instanceAlias,
           string agentVersion,
           int configurationVersion,
           DateTime lastModifiedTime,
           string traceDirectory,
           int maxTraceSize,
           int maxUnattendedTime,
           TraceOption traceOptions,
           AuditLogConfiguration[] auditLogConfigList
        )
        {
            SPBuilder builder = null;
            int sqlVersion = 8;
            string sDbName = conn.Database;
            try
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, "Creating audit stored procedure for " + instanceAlias);
                ValidateConnection(conn);


                sqlVersion = SQLInstance.GetSqlVersion(conn.ServerVersion);

                //return if SQL Server version is lower than 2017
                if (sqlVersion < 14)
                {
                    conn.ChangeDatabase(sDbName);
                    return;
                }
                builder = new SPBuilder(sqlVersion);
                builder.AgentVersion = agentVersion;
                builder.ConfigurationVersion = configurationVersion;
                builder.AuditLogConfiguration = auditLogConfigList;
                builder.LastModifiedTime = lastModifiedTime;
                builder.InstanceAlias = instanceAlias;
                builder.TraceDirectory = traceDirectory;
                builder.MaxTraceSize = maxTraceSize;
                builder.Options = traceOptions;
                builder.MaxUnattendedTime = maxUnattendedTime;

                //Adding DataChangedTable(SQLcompliance_Changed_Data_Table) ID filter
                foreach (AuditLogConfiguration auditLogConfiguration in auditLogConfigList)
                {
                    if (auditLogConfiguration.Category == TraceCategory.DataChange
                        || auditLogConfiguration.Category == TraceCategory.DataChangeWithDetails)
                    {
                        List<TraceFilter> traceFilters = new List<TraceFilter>(auditLogConfiguration.GetTraceFilters());
                        int result = traceFilters.FindIndex(r => r.AuditLogColumnId == AuditLogColumnId.ObjectID);
                        if (result > -1)
                        {
                            TraceFilter traceFilter = traceFilters.Find(r => r.AuditLogColumnId == AuditLogColumnId.DatabaseName);
                            if (traceFilter != null)
                            {
                                conn.ChangeDatabase(traceFilter.GetTextValue());
                                int dataChangedTableId = GetDataChangedTableId(conn);
                                if (dataChangedTableId != -1)
                                {
                                    auditLogConfiguration.AddFilter(new TraceFilter(AuditLogColumnId.ObjectID,
                                                        TraceFilterComparisonOpXE.Equal,
                                                        dataChangedTableId,
                                                        AuditLogFilterLogicalOp.OR));
                                }
                            }
                        }
                    }
                }

                conn.ChangeDatabase("master");

                using (SqlCommand command = new SqlCommand(builder.CreateAuditLogSP(), conn))
                {
                    command.ExecuteNonQuery();
                }

                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                        String.Format(CoreConstants.Info_AuditSPCreated,
                                        CoreConstants.Agent_AuditLogSPName,
                                        instanceAlias,
                                        configurationVersion,
                                        agentVersion,
                                        lastModifiedTime));
            }
            catch (Exception e)
            {
                if (ErrorLog.Instance.ErrorLevel >= ErrorLog.Level.Debug &&
                    builder != null)
                {
                    string filename = Path.Combine(traceDirectory, "SQLcomplianceAuditLog.sql");

                    StreamWriter w = new StreamWriter(filename, false);
                    w.Write(builder.CreateAuditLogSP());
                    w.Close();

                    ErrorLog.Instance.Write(ErrorLog.Level.Default,
                                             String.Format(CoreConstants.Exception_ErrorCreatingAuditStoredProcedureCopySaved,
                                                            filename),
                                             e,
                                             true);

                }
                else
                {
                    ErrorLog.Instance.Write(ErrorLog.Level.Default,
                                             CoreConstants.Exception_ErrorCreatingAuditStoredProcedure,
                                             e,
                                             true);
                }

                conn.ChangeDatabase(sDbName);
                throw e;
            }

            SetSPPermission(CoreConstants.Agent_AuditLogSPName,
                              sqlVersion,
                              conn);
            conn.ChangeDatabase(sDbName);

        }
        /// <summary>
        /// Stop Audit Logs
        /// </summary>
        /// <param name="conn"></param>
        public static void
          StopAllAuditLogs(
          SqlConnection conn
       )
        {
            using (SqlCommand command = CreateAuditLogSPCommand(conn, SPCmd.StopAll, null))
            {
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Drop Audit Log stored procedure
        /// </summary>
        /// <param name="conn"></param>
        public static void
         DropAuditLogSP(
         SqlConnection conn
         )
        {
            DropStoredProcedure(conn, CoreConstants.Agent_AuditLogSPName);
        }

        /// <summary>
        /// CreateAuditLogSPCommand
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="command"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        private static SqlCommand
           CreateAuditLogSPCommand(
           SqlConnection conn,
           SPCmd command,
           string option
        )
        {
            ValidateConnection(conn);

            // Create a new SqlCommand to call the audit SP
            SqlCommand cmd = new SqlCommand(CoreConstants.Agent_Full_AuditLogSPName, conn);

            // Added the missing timeout for the command
            cmd.CommandTimeout = SQLcomplianceAgent.Instance.TraceStartTimeout;

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(new SqlParameter("@Cmd",
               SqlDbType.Int,
               0,
               "Cmd"));

            cmd.Parameters.Add(new SqlParameter("@Option",
               SqlDbType.NVarChar,
               4000,
               "Option"));

            cmd.Parameters.Add(new SqlParameter("@ReturnCode",
               SqlDbType.Int,
               0,
               ParameterDirection.ReturnValue,
               false,
               0,
               0,
               "ReturnCode",
               DataRowVersion.Default,
               null));

            cmd.Parameters["@Cmd"].Value = (int)command;
            cmd.Parameters["@Option"].Value = option;

            return cmd;
        }
        //-------------------------------------------------------------------------------------------------
        // GetRunningAuditLogs for Audit Logs - returns an array of integer running Audit Logs session IDs
        //-------------------------------------------------------------------------------------------------
        public static int[]
           GetRunningAuditLogs(
              SqlConnection conn
           )
        {
            ArrayList auditLogsIds = new ArrayList();
            if (!DoesSPExist(conn, CoreConstants.Agent_AuditLogSPName))
                return null;
            try
            {

                using (SqlCommand command = CreateAuditLogSPCommand(conn, SPCmd.GetRunningTraces, null))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                if (!reader.IsDBNull(0))
                                    auditLogsIds.Add(reader.GetInt32(0));
                            }
                        }
                    }
                }

                return (int[])auditLogsIds.ToArray(typeof(int));
            }
            catch (SqlException sqlEx)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, sqlEx, true);
                throw sqlEx;
            }
        }

        public static TraceInfo[]
         StartAllAuditLogs(
        SqlConnection conn
        )
        {
            ArrayList traces = new ArrayList();

            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                     "Starting all configured audit logs.");
            if (!DoesSPExist(conn, CoreConstants.Agent_AuditLogSPName))
                return null;
            try
            {

                using (SqlCommand command = NewStartAllAuditLogsCommand(conn))
                {
                    command.CommandTimeout = SQLcomplianceAgent.Instance.TraceStartTimeout;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        DateTime startTime = DateTime.Now;

                        if (reader != null)
                        {
                            TraceInfo info = null;
                            while (reader.Read())
                            {
                                info = new TraceInfo();
                                GetTraceInfoFromReader(info, reader);
                                info.StartTime = startTime;
                                traces.Add(info);
                            }
                        }
                    }
                }

                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         String.Format("All {0} configured traces started.", traces.Count));

                return (TraceInfo[])traces.ToArray(typeof(TraceInfo));
            }

            catch (SqlException sqlEx)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, sqlEx, true);
                throw sqlEx;
            }
        }

        //5.4 XE
        public static void
         StopAuditLogs(
         SqlConnection conn,
         int[] traceIds
         )
        {

            if (traceIds == null || traceIds.Length == 0)
                return;

            Hashtable ids = new Hashtable();
            StringBuilder idList = new StringBuilder();

            for (int i = 0; i < traceIds.Length; i++)
            {
                try
                {
                    ids.Add(traceIds[i], traceIds[i]);               // Eliminating duplicate trace ids
                    idList.AppendFormat("{0},", traceIds[i]);
                }
                catch { }
            }

            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                     String.Format("Stopping traces: {0}.", idList.ToString()));

            using (SqlCommand command = NewStopAuditLogsCommand(conn, idList.ToString()))
            {
                command.ExecuteNonQuery();
            }

            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                     String.Format("Stopped traces: {0}.", idList.ToString()));
        }

        //5.5 Audit Logs
        private static SqlCommand
            NewStopAuditLogsCommand(
            SqlConnection conn,
            string traceIdList
         )
        {
            return CreateAuditLogSPCommand(conn, SPCmd.Stop, traceIdList);
        }


        /// <summary>
        /// IsAuditLogSPCurrent
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="configurationVersion"></param>
        /// <param name="lastModifiedTime"></param>
        /// <returns></returns>
        public static bool
           IsAuditLogSPCurrent(
           SqlConnection conn,
           int configurationVersion,
           DateTime lastModifiedTime)
        {
            bool isCurrent = false;
            try
            {
                if (DoesSPExist(conn, CoreConstants.Agent_AuditLogSPName))
                {
                    AuditSPInfo info = GetAuditLogSPInfo(conn);
                    if (info.CreationTime >= lastModifiedTime &&
                        info.ConfigurationVersion >= configurationVersion)
                        isCurrent = true;
                }
            }
            catch (Exception)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         "An error occurred when getting audit stored procedure information");
            }
            return isCurrent;
        }

        /// <summary>
        /// GetAuditLogSPInfo
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static AuditSPInfo GetAuditLogSPInfo(SqlConnection conn)
        {
            AuditSPInfo info = new AuditSPInfo();
            using (SqlCommand command = NewGetAuditLogSPInfoCommand(conn))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader != null)
                    {
                        // There is only one row returned
                        reader.Read();
                        info.CreationTime = DateTime.Parse(reader.GetString(0));
                        info.AgentVersion = reader.GetString(1);
                        info.ConfigurationVersion = reader.GetInt32(2);
                        info.TraceCount = reader.GetInt32(3);
                    }
                }
            }

            return info;
        }

        /// <summary>
        /// NewGetAuditLogSPInfoCommand
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        private static SqlCommand
           NewGetAuditLogSPInfoCommand(
              SqlConnection conn
           )
        {
            return CreateAuditLogSPCommand(conn, SPCmd.GetInfo, null);
        }

        /// <summary>
        /// NewStartAllAuditLogsCommand
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        private static SqlCommand
        NewStartAllAuditLogsCommand(
           SqlConnection conn
        )
        {
            return CreateAuditLogSPCommand(conn, SPCmd.Start, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="auditLogsIdList"></param>
        /// <returns></returns>
        public static TraceInfo[]
           GetAuditLogsInfo(
           SqlConnection conn,
           string auditLogsIdList
        )
        {
            TraceInfo[] auditLogs = null;

            // Connection is valided in  the Get
            using (SqlCommand command = NewGetAuditLogsStatusCommand(conn, auditLogsIdList))
            {

                try
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader != null)
                        {
                            ArrayList traceList = new ArrayList();
                            TraceInfo info = null;
                            while (reader.Read())
                            {
                                info = new TraceInfo();
                                GetTraceInfoFromReader(info, reader);
                                traceList.Add(info);
                            }
                            auditLogs = (TraceInfo[])traceList.ToArray(typeof(TraceInfo));
                        }
                    }
                }
                catch (Exception e)
                {
                    return null;
                }
            }
            return auditLogs;
        }

        //5.5 Audit Logs
        /// <summary>
        /// NewGetAuditLogsStatusCommand
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="auditLogsIdList"></param>
        /// <returns></returns>
        private static SqlCommand
            NewGetAuditLogsStatusCommand(
            SqlConnection conn,
            string auditLogsIdList
        )
        {
            return CreateAuditLogSPCommand(conn, SPCmd.GetStatus, auditLogsIdList);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connStr"></param>
        /// <returns></returns>
        public static int GetDataChangedTableId(SqlConnection conn)
        {
            int dataChangedTableId = -1;
            try
            {
                string sqlText = string.Format(@"SELECT o.object_id FROM sys.schemas s  
                                            join sys.objects o ON (s.schema_id=o.schema_id) WHERE 
                                            s.name = '{0}' AND o.name= '{1}'", CoreConstants.Agent_BeforeAfter_SchemaName, CoreConstants.Agent_BeforeAfter_TableName);
                using (SqlCommand command = new SqlCommand(sqlText, conn))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            dataChangedTableId = reader.GetInt32(0);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Default,
                                    "Error getting current configuration",
                                    e,
                                    true);
            }
            return dataChangedTableId;
        }
        #endregion
    }
}
