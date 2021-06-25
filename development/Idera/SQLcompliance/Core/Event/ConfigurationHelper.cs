using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Data.SqlClient;

using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Agent;
using Idera.SQLcompliance.Core.Remoting;
using Idera.SQLcompliance.Core.Triggers;

namespace Idera.SQLcompliance.Core.Event
{
    /// <summary>
    /// Summary description for ConfigurationHelper.
    /// </summary>
    public class ConfigurationHelper
    {
        #region Private fields
        //private const int MaxAuditedUserCount     =  500;

        private static string server;
        private static int serverPort;
        private static Hashtable configurationUpdated = new Hashtable();
        private static Dictionary<string, UserCache> auditedUsersCache = new Dictionary<string, UserCache>();
        //v5.6 SQLCM-5373
        private static Dictionary<string, UserCache> trustedUsersCache = new Dictionary<string, UserCache>();
        private static Hashtable enumerationTimeTable = new Hashtable();
        private static TimeSpan enumerationInterval = new TimeSpan(0, 5, 0); //( 6, 0, 0 );

        #endregion

        #region Properties

        public static string Server
        {
            get { return server; }
            set { server = value; }
        }

        public static int ServerPort
        {
            get { return serverPort; }
            set { serverPort = value; }
        }

        public static TimeSpan EnumerationInterval
        {
            get { return enumerationInterval; }
            set { enumerationInterval = value; }
        }

        #endregion

        #region Constructors

        static ConfigurationHelper()
        {
        }

        #endregion

        #region Audit Configuration Serialization Methods
        public static bool
           LoadAuditConfigurationFromFile(
              ServerAuditConfiguration config)
        {
            string filename = null;

            try
            {

                filename = Path.Combine(config.TraceDirectory,
                                         config.InstanceAlias + ".bin");

            }
            catch (Exception e)
            {
                string msg = String.Format("Error creating configuration filename.\n TraceDirectory: {0}\nInstanceAlias: {1}",
                                             config.TraceDirectory,
                                             config.InstanceAlias);
                ErrorLog.Instance.Write(ErrorLog.Level.Default, msg, e, true);
                return false;

            }

            return LoadAuditConfigurationFromFile(config, filename);

        }

        //-------------------------------------------------------------------
        // LoadAuditConfigurationFromFile
        //-------------------------------------------------------------------
        public static bool
           LoadAuditConfigurationFromFile(
              ServerAuditConfiguration config,
              string fileName
           )
        {
            IFormatter formatter;
            Stream stream;
            bool loaded = false;

            try
            {
                formatter = new BinaryFormatter();
                using (stream = new FileStream(fileName,
                                                FileMode.Open,
                                                FileAccess.Read,
                                                FileShare.Read))
                {
                    RemoteAuditConfiguration remoteConfig = (RemoteAuditConfiguration)formatter.Deserialize(stream);

                    CopyRemoteConfigToServerAuditConfig(config, remoteConfig);
                }
                loaded = true;

            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         CoreConstants.Exception_ErrorReadingAuditConfiguratioin,
                                         e,
                                         true);

            }
            return loaded;

        }

        //-------------------------------------------------------------------
        // SaveAuditConfigurationToFile
        //-------------------------------------------------------------------
        public static void
           SaveAuditConfigurationToFile(
              ServerAuditConfiguration config,
              RemoteAuditConfiguration remoteConfig
           )
        {
            if (config == null)
                throw new ArgumentException("config");

            IFormatter formatter = null;
            Stream stream = null;

            try
            {
                string filename = Path.Combine(config.TraceDirectory, config.InstanceAlias + ".bin");

                remoteConfig.StructVersion = CoreConstants.SerializationVersion;

                formatter = new BinaryFormatter();
                using (stream = new FileStream(filename,
                                              FileMode.Create,
                                              FileAccess.Write,
                                              FileShare.None))
                {
                    formatter.Serialize(stream, remoteConfig);
                }

            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         CoreConstants.Exception_ErrorPersistingAuditConfiguration,
                                         e,
                                         true);
            }
            finally
            {
                formatter = null;
            }


        }
        /*
        //-------------------------------------------------------------------
        // GetTraceConfigurationsFromFile
        //-------------------------------------------------------------------
        /// <summary>
        /// Retrieve the trace configurations from an audit configuration file.
        /// </summary>
        /// <param name="xmlFileName"></param>
        /// <returns></returns>
        public static TraceConfiguration []
           GetTraceConfigurationsFromFile(
              string filename
           )
        {
           return GetTraceConfigurationsFromFile( filename, false );
        }
        */

        //-------------------------------------------------------------------
        // GetTraceConfigurationsFromFile
        //-------------------------------------------------------------------
        /// <summary>
        /// Retrieve the trace configurations from an audit configuration file.
        /// </summary>
        /// <returns></returns>
        public static TraceConfiguration[]
           GetTraceConfigurationsFromFile(
              string filename,
              string instance,
              bool isServer,
              out TraceEventId[] privEvents,
              out string[] privUsers,
              out bool select,
              out bool dml,
              out int[] dbIds,
              int sqlVersion,
              out Dictionary<int, DBAuditConfiguration> dbConfigs,
              out AuditConfiguration userConfigs
            )
        {
            Dictionary<int, List<string>> dcTableLists;
            Dictionary<int, Dictionary<string, TableConfiguration>> scTableLists;
            return GetTraceConfigurationsFromFile(filename,
                                                   instance,
                                                   isServer,
                                                   out privEvents,
                                                   out privUsers,
                                                   out select,
                                                   out dml,
                                                   out dbIds,
                                                   sqlVersion,
                                                   out dcTableLists,
                                                   out scTableLists,
                                                   out dbConfigs,
                                                   out userConfigs);
        }

        //-------------------------------------------------------------------
        // GetTraceConfigurationsFromFile
        //-------------------------------------------------------------------
        /// <summary>
        /// Retrieve the trace configurations from an audit configuration file.
        /// </summary>
        /// <returns></returns>
        public static TraceConfiguration[]
           GetTraceConfigurationsFromFile(
              string filename,
              string instance,
              bool isServer,
              out TraceEventId[] privEvents,
              out string[] privUsers,
              out bool select,
              out bool dml,
              out int[] dbIds,
              int sqlVersion,
              out Dictionary<int, List<string>> dcTableLists, // data change table lists
              out Dictionary<int, Dictionary<string, TableConfiguration>> scTableLists,
              out Dictionary<int, DBAuditConfiguration> dbConfigs,
              out AuditConfiguration userConfigs
           )
        {
            ArrayList dbIdList = new ArrayList();
            TraceConfiguration[] traces = null;
            privEvents = null;
            privUsers = null;
            select = false;
            dbIds = null;
            dml = false;
            dcTableLists = new Dictionary<int, List<string>>();
            scTableLists = new Dictionary<int, Dictionary<string, TableConfiguration>>();
            dbConfigs = new Dictionary<int, DBAuditConfiguration>();
            userConfigs = null;

            try
            {
                ServerAuditConfiguration auditConfig = new ServerAuditConfiguration();
                auditConfig.Instance = instance;
                auditConfig.isServer = isServer;
                auditConfig.sqlVersion = sqlVersion;
                if (!LoadAuditConfigurationFromFile(auditConfig, filename))
                    return null;
                traces = auditConfig.GenerateTraceConfigurations();
                userConfigs = auditConfig.UserConfiguration;
                DBAuditConfiguration[] dbConfigList = auditConfig.AuditDBList;
                if (dbConfigList != null && dbConfigList.Length > 0)
                {
                    for (int i = 0; i < dbConfigList.Length; i++)
                    {
                        dbConfigs.Add(dbConfigList[i].DBId, dbConfigList[i]);
                    }
                }
                if (isServer && dbConfigList != null)
                {
                    foreach (DBAuditConfiguration dbconfig in dbConfigList)
                    {
                        dbIdList.Add(dbconfig.DBId);
                        if (dbconfig.DataChangeTables != null &&
                            dbconfig.DataChangeTables.Length > 0)
                        {
                            List<string> list = new List<string>(dbconfig.DataChangeTables.Length);
                            foreach (TableConfiguration tbl in dbconfig.DataChangeTables)
                                list.Add(tbl.GetFullName());  // 2 part table name if schema is not default
                            dcTableLists.Add(dbconfig.DBId, list);
                            auditConfig.CaptureDataChanges = true;
                        }

                        if (dbconfig.SensitiveColumns != null &&
                            dbconfig.SensitiveColumns.Length > 0)
                        {
                            Dictionary<string, TableConfiguration> tables = new Dictionary<string, TableConfiguration>();
                            foreach (TableConfiguration tbl in dbconfig.SensitiveColumns)
                            {
                                //Release:5.4 Defect:SQLCM-2171 Start
                                if (!tables.ContainsKey(GetFullTableName(tbl.Schema, tbl.Name)))
                                {
                                    tables.Add(GetFullTableName(tbl.Schema, tbl.Name), tbl);
                                }
                                //Release:5.4 Defect:SQLCM-2171 end
                            }
                            scTableLists.Add(dbconfig.DBId, tables);
                            auditConfig.AuditSensitiveColumns = true;
                        }

                    }
                }
                dbIds = (int[])dbIdList.ToArray(typeof(int));

                if (!isServer)
                    return traces;

                bool found = false;
                for (int i = 0; i < traces.Length && !found; i++)
                {
                    if (traces[i].Level == TraceLevel.User)
                    {
                        privEvents = traces[i].GetTraceEvents();
                        found = true;
                    }
                }

                List<string> pUsers = GetPrivUsers(auditConfig);

                if (pUsers.Count > 0)
                {
                    select = auditConfig.UserConfiguration.AuditSELECT;
                    dml = auditConfig.UserConfiguration.AuditDML;
                }
                privUsers = pUsers.ToArray();
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose, e, true);
                throw;

            }
            return traces;
        }
        private static string GetFullTableName(string schemaName, string tableName)
        {
            if (schemaName == null || schemaName.Length == 0)
                return tableName;
            if (tableName.Contains(","))
            {
                string tempTableName = tableName.Replace(",", "," + schemaName + ".");
                return String.Format("{0}.{1}", schemaName, tempTableName);
            }
            else
            {
                return String.Format("{0}.{1}", schemaName, tableName);
            }
        }

        //5.4 XE
        public static XeTraceConfiguration[]
        GetTraceConfigurationsFromFileXE(
           string filename,
           string instance,
           bool isServer,
           out TraceEventId[] privEvents,
           out string[] privUsers,
           out bool select,
           out bool dml,
           out int[] dbIds,
           int sqlVersion,
           out Dictionary<int, List<string>> dcTableLists, // data change table lists
           out Dictionary<int, Dictionary<string, TableConfiguration>> scTableLists,
           out Dictionary<int, DBAuditConfiguration> dbConfigs,
           out AuditConfiguration userConfigs
        )
        {
            ArrayList dbIdList = new ArrayList();
            XeTraceConfiguration[] traces = null;
            privEvents = null;
            privUsers = null;
            select = false;
            dbIds = null;
            dml = false;
            dcTableLists = new Dictionary<int, List<string>>();
            scTableLists = new Dictionary<int, Dictionary<string, TableConfiguration>>();
            dbConfigs = new Dictionary<int, DBAuditConfiguration>();
            userConfigs = null;
            try
            {
                ServerAuditConfiguration auditConfig = new ServerAuditConfiguration();
                auditConfig.Instance = instance;
                auditConfig.isServer = isServer;
                auditConfig.sqlVersion = sqlVersion;
                if (!LoadAuditConfigurationFromFile(auditConfig, filename))
                    return null;
                traces = auditConfig.GenerateTraceConfigurationsXE();
                DBAuditConfiguration[] dbConfigList = auditConfig.AuditDBList;
                userConfigs = auditConfig.UserConfiguration;
                if (dbConfigList != null && dbConfigList.Length > 0)
                {
                    for (int i = 0; i < dbConfigList.Length; i++)
                    {
                        dbConfigs.Add(dbConfigList[i].DBId, dbConfigList[i]);
                    }
                }
                if (isServer && dbConfigList != null)
                {
                    foreach (DBAuditConfiguration dbconfig in dbConfigList)
                    {
                        dbIdList.Add(dbconfig.DBId);
                        if (dbconfig.DataChangeTables != null &&
                            dbconfig.DataChangeTables.Length > 0)
                        {
                            List<string> list = new List<string>(dbconfig.DataChangeTables.Length);
                            foreach (TableConfiguration tbl in dbconfig.DataChangeTables)
                                list.Add(tbl.GetFullName());  // 2 part table name if schema is not default
                            dcTableLists.Add(dbconfig.DBId, list);
                            auditConfig.CaptureDataChanges = true;
                        }

                        if (dbconfig.SensitiveColumns != null &&
                            dbconfig.SensitiveColumns.Length > 0)
                        {
                            Dictionary<string, TableConfiguration> tables = new Dictionary<string, TableConfiguration>();
                            foreach (TableConfiguration tbl in dbconfig.SensitiveColumns)
                            {
                                //Release:5.4 Defect:SQLCM-2171 Start
                                tables.Add(tbl.GetFullName(), tbl);
                                //Release:5.4 Defect:SQLCM-2171 end
                            }
                            scTableLists.Add(dbconfig.DBId, tables);
                            auditConfig.AuditSensitiveColumns = true;
                        }
                    }
                }
                dbIds = (int[])dbIdList.ToArray(typeof(int));

                if (!isServer)
                    return traces;

                bool found = false;
                for (int i = 0; i < traces.Length && !found; i++)
                {
                    if (traces[i].Level == TraceLevel.User)
                    {
                        privEvents = new TraceEventId[2];//traces[i].GetTraceEventsXE();
                        privEvents[0] = TraceEventId.SpStarting;
                        privEvents[1] = TraceEventId.SqlStmtStarting;
                        found = true;
                    }
                }

                List<string> pUsers = GetPrivUsers(auditConfig);

                if (pUsers.Count > 0)
                {
                    select = auditConfig.UserConfiguration.AuditSELECT;
                    dml = auditConfig.UserConfiguration.AuditDML;
                }
                privUsers = pUsers.ToArray();

            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose, e, true);
                throw;

            }
            return traces;

        }


        //-------------------------------------------------------------------
        // GetTraceConfiguration
        //-------------------------------------------------------------------
        public static TraceConfiguration
           GetTraceConfiguration(
              string binFile,
              string traceFile,
              bool isServer,
              string instance,
              int sqlVersion
           )
        {
            Dictionary<int, List<string>> dcTableLists;
            Dictionary<int, Dictionary<string, TableConfiguration>> scTableLists;
            Dictionary<int, DBAuditConfiguration> dbConfigs;
            AuditConfiguration userConfigs;

            return
               GetTraceConfiguration(binFile,
                                      traceFile,
                                      isServer,
                                      instance,
                                      sqlVersion,
                                      out dcTableLists,
                                      out scTableLists,
                                      out dbConfigs,
                                      out userConfigs);
        }

        //-------------------------------------------------------------------
        // GetTraceConfiguration
        //-------------------------------------------------------------------
        public static TraceConfiguration
           GetTraceConfiguration(
              string binFile,
              string traceFile,
              bool isServer,
              string instance,
              int sqlVersion,
              out Dictionary<int, List<string>> dcTableLists,
              out Dictionary<int, Dictionary<string, TableConfiguration>> scTableLists,
              out Dictionary<int, DBAuditConfiguration> dbConfig,
              out AuditConfiguration userConfigs
           )
        {
            FileInfo fileInfo = new FileInfo(traceFile);
            TraceFileNameAttributes attrs = TraceFileNameAttributes.GetNameAttributes(fileInfo);
            TraceEventId[] privEvents = null;
            string[] privUsers = null;
            bool select = false;
            bool dml = false;
            int[] dbIds = null;
            Dictionary<int, List<string>> TmpDcTableLists;
            Dictionary<int, Dictionary<string, TableConfiguration>> TmpScTableLists;

            dcTableLists = new Dictionary<int, List<string>>();
            scTableLists = new Dictionary<int, Dictionary<string, TableConfiguration>>();

            TraceConfiguration[] traceConfigs = GetTraceConfigurationsFromFile(binFile,
                                                                                 instance,
                                                                                 isServer,
                                                                                 out privEvents,
                                                                                 out privUsers,
                                                                                 out select,
                                                                                 out dml,
                                                                                 out dbIds,
                                                                                 sqlVersion,
                                                                                 out TmpDcTableLists,
                                                                                 out TmpScTableLists,
                                                                                 out dbConfig,
                                                                                 out userConfigs);

            if (traceConfigs == null ||
               traceConfigs.Length == 0)
                return null;

            TraceConfiguration config = null;
            for (int i = 0; i < traceConfigs.Length && config == null; i++)
            {
                //if ( (int) traceConfigs[i].Level == attrs.AuditLevel &&
                //     traceConfigs[i].Sequence == attrs.AuditSequence )
                if ((int)traceConfigs[i].Level == attrs.AuditLevel)
                {
                    if ((int)traceConfigs[i].Category == attrs.AuditCategory)
                    {
                        //config = traceConfigs[i];
                        bool confMatches = traceConfigs[i].Sequence == attrs.AuditSequence;
                        string [] traceFileS = traceFile.Split('\\');
                        
                        //Ensures that SQL Server audit files (AL) are not took in account for the trace config vaidation
                        //Ensures that db validation is executed only in Collection side
                        //In collection side confs are obtained from .bin file
                        if (traceFile.Contains(".bin") && !traceFileS[traceFileS.Length-1].StartsWith("AL"))
                        {
                            bool hasDBFilter = false;
                            bool confBelongsTraceFile = false;
                            ValidateTraceConfig(traceFile, traceConfigs[i], instance, out hasDBFilter, out confBelongsTraceFile);
                            confMatches = confBelongsTraceFile || (confMatches && !hasDBFilter);
                        }

                        if (confMatches)
                        {
                            config = traceConfigs[i];
                        }
                    }
                    else if (traceConfigs[i].Category == TraceCategory.DML &&
                              attrs.AuditCategory == (int)TraceCategory.DMLwithSELECT)
                    {  // DMLwithSELECT is no longer exist in 3.1 but old agents still
                       // generate these traces.
                        traceConfigs[i].AddFilter(new TraceFilter(TraceColumnId.Permissions,
                                                                    TraceFilterComparisonOp.
                                                                       Equal,
                                                                    1,
                                                                    // SELECT
                                                                    TraceFilterLogicalOp.OR));

                        traceConfigs[i].AddFilter(new TraceFilter(TraceColumnId.Permissions,
                                                                    TraceFilterComparisonOp.
                                                                       Equal,
                                                                    4,
                                                                    // REFERENCE ALL
                                                                    TraceFilterLogicalOp.OR));

                        config = traceConfigs[i];
                    }
                }
            }

            if (config != null)
            {
                if (privEvents != null)
                {
                    ArrayList tmpArray = new ArrayList();
                    for (int i = 0; i < privEvents.Length; i++)
                        tmpArray.Add((int)privEvents[i]);
                    config.privEvents = (int[])tmpArray.ToArray(typeof(int));
                }

                if (privUsers != null)
                {
                    config.privUsers = privUsers;
                }

                config.privSELECT = select;
                config.privDML = dml;
                if (isServer)  // Server side configurations
                {
                    if (config.Level == TraceLevel.Database)
                    {
                        if (((config.Category == TraceCategory.DBSecurity &&
                                 config.Sequence == 0) ||
                               (config.Category == TraceCategory.DML ||
                                 config.Category == TraceCategory.DMLwithDetails ||
                                 config.Category == TraceCategory.DMLwithSELECT ||
                                 config.Category == TraceCategory.SELECTwithDetails ||
                                 config.Category == TraceCategory.SELECT)))
                        {
                            config.Databases = GetDatabaseNames(instance, dbIds);
                        }

                        if (config.Category == TraceCategory.DML
                            || config.Category == TraceCategory.DMLwithDetails
                            || config.Category == TraceCategory.DataChangeWithDetails
                            || config.Category == TraceCategory.DataChange)
                        {
                            foreach (int id in dbIds)
                            {
                                if (TmpDcTableLists.ContainsKey(id))
                                    dcTableLists.Add(id, TmpDcTableLists[id]);
                            }
                        }
                    }
                    else if (config.Level == TraceLevel.Table)
                    {
                        //add the sensitive columns settings
                        foreach (int id in dbIds)
                        {
                            if (TmpScTableLists.ContainsKey(id))
                                scTableLists.Add(id, TmpScTableLists[id]);
                        }
                    }
                    else if (config.Level == TraceLevel.User)
                    {
                        // priv user trace needs to filter out data change and sensitive column events
                        // using these lists.
                        dcTableLists = TmpDcTableLists;
                        scTableLists = TmpScTableLists;
                    }
                }
            }

            return config;

        }
        private static void ValidateTraceConfig(String traceFile, TraceConfiguration trcConfig, String instanceName,
         out bool hasDBFilter, out bool confBelongsTraceFile)
        {
            hasDBFilter = false;
            confBelongsTraceFile = false;

            try
            {
                traceFile = traceFile.Replace(".bin", ".trc");
                List<TraceFilter> traceFilters = new List<TraceFilter>(trcConfig.GetTraceFilters());
                foreach (TraceFilter filter in traceFilters)
                {
                    if (filter.ColumnId.ToString().Equals("DatabaseID"))
                    {
                        hasDBFilter = true;
                        confBelongsTraceFile = isDBIdInTraceFile(traceFile, instanceName, filter.GetIntValue());
                        if (confBelongsTraceFile)
                        {
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write("Exception while validating trace configuration", e);
            }

        }

        private static bool isDBIdInTraceFile(String traceFile, String instanceName, int dbId)
        {
            instanceName = instanceName.ToUpper();
            Repository repository = new Repository();
            try
            {   
                repository.OpenConnection();
                
                string queryEmptyValidation = String.Format("SELECT * FROM ::fn_trace_gettable('{0}', 1) where DatabaseId is not null", traceFile);

                using (SqlCommand cmd = new SqlCommand(queryEmptyValidation, repository.connection))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (!reader.HasRows)
                    {
                        return true;
                    }
                    reader.Close();
                }

                string query = String.Format("SELECT * FROM ::fn_trace_gettable('{0}', 1) where DatabaseId = {1}", traceFile, dbId);

                using (SqlCommand cmd = new SqlCommand(query, repository.connection))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        return true;
                    }
                    reader.Close();
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write("Exception while getting trace data", e);
                return false;
            }
            finally
            {
                repository.CloseConnection();
            }

            return false;
        }
        //5.4 XE
        public static XeTraceConfiguration
         GetTraceConfigurationXE(
            string binFile,
            string traceFile,
            bool isServer,
            string instance,
            int sqlVersion,
            out Dictionary<int, List<string>> dcTableLists,
            out Dictionary<int, Dictionary<string, TableConfiguration>> scTableLists,
            out Dictionary<int, DBAuditConfiguration> dbConfig,
            out AuditConfiguration userConfigs
         )
        {
            FileInfo fileInfo = new FileInfo(traceFile);
            TraceFileNameAttributes attrs = TraceFileNameAttributes.GetNameAttributes(fileInfo);
            TraceEventId[] privEvents = null;
            string[] privUsers = null;
            bool select = false;
            bool dml = false;
            int[] dbIds = null;
            Dictionary<int, List<string>> TmpDcTableLists;
            Dictionary<int, Dictionary<string, TableConfiguration>> TmpScTableLists;

            dcTableLists = new Dictionary<int, List<string>>();
            scTableLists = new Dictionary<int, Dictionary<string, TableConfiguration>>();

            XeTraceConfiguration[] traceConfigs = GetTraceConfigurationsFromFileXE(binFile,
                                                                                 instance,
                                                                                 isServer,
                                                                                 out privEvents,
                                                                                 out privUsers,
                                                                                 out select,
                                                                                 out dml,
                                                                                 out dbIds,
                                                                                 sqlVersion,
                                                                                 out TmpDcTableLists,
                                                                                 out TmpScTableLists,
                                                                                 out dbConfig,
                                                                                 out userConfigs);

            if (traceConfigs == null ||
               traceConfigs.Length == 0)
                return null;

            XeTraceConfiguration config = null;
            for (int i = 0; i < traceConfigs.Length && config == null; i++)
            {
                if ((int)traceConfigs[i].Level == attrs.AuditLevel &&
                     traceConfigs[i].Sequence == attrs.AuditSequence)
                {
                    if ((int)traceConfigs[i].Category == attrs.AuditCategory)
                    {
                        config = traceConfigs[i];
                    }
                    else if (traceConfigs[i].Category == TraceCategory.DML &&
                              attrs.AuditCategory == (int)TraceCategory.DMLwithSELECT)
                    {  // DMLwithSELECT is no longer exist in 3.1 but old agents still
                       // generate these traces.
                        traceConfigs[i].AddFilter(new TraceFilter(TraceColumnId.Permissions,
                                                                    TraceFilterComparisonOp.
                                                                       Equal,
                                                                    1,
                                                                    // SELECT
                                                                    TraceFilterLogicalOp.OR));

                        traceConfigs[i].AddFilter(new TraceFilter(TraceColumnId.Permissions,
                                                                    TraceFilterComparisonOp.
                                                                       Equal,
                                                                    4,
                                                                    // REFERENCE ALL
                                                                    TraceFilterLogicalOp.OR));

                        config = traceConfigs[i];
                    }
                }
            }

            if (config != null)
            {
                if (privEvents != null)
                {
                    ArrayList tmpArray = new ArrayList();
                    for (int i = 0; i < privEvents.Length; i++)
                        tmpArray.Add((int)privEvents[i]);
                    config.privEvents = (int[])tmpArray.ToArray(typeof(int));
                }

                if (privUsers != null)
                {
                    config.privUsers = privUsers;
                }

                config.privSELECT = select;
                config.privDML = dml;
                if (isServer)  // Server side configurations
                {
                    if (config.Level == TraceLevel.Database)
                    {
                        if (((config.Category == TraceCategory.DBSecurity &&
                                 config.Sequence == 0) ||
                               (config.Category == TraceCategory.DML ||
                                 config.Category == TraceCategory.DMLwithDetails ||
                                 config.Category == TraceCategory.DMLwithSELECT ||
                                 config.Category == TraceCategory.SELECTwithDetails ||
                                 config.Category == TraceCategory.SELECT)))
                        {
                            config.Databases = GetDatabaseNames(instance, dbIds);
                        }

                        if (config.Category == TraceCategory.DML ||
                             config.Category == TraceCategory.DMLwithDetails)
                        {
                            foreach (int id in dbIds)
                            {
                                if (TmpDcTableLists.ContainsKey(id))
                                    dcTableLists.Add(id, TmpDcTableLists[id]);
                            }
                        }
                    }
                    else if (config.Level == TraceLevel.Table)
                    {
                        //add the sensitive columns settings
                        foreach (int id in dbIds)
                        {
                            if (TmpScTableLists.ContainsKey(id))
                                scTableLists.Add(id, TmpScTableLists[id]);
                        }
                    }
                    else if (config.Level == TraceLevel.User)
                    {
                        // priv user trace needs to filter out data change and sensitive column events
                        // using these lists.
                        dcTableLists = TmpDcTableLists;
                        scTableLists = TmpScTableLists;
                    }
                }
            }

            return config;

        }

        //-------------------------------------------------------------------
        // GetTraceConfigurationFromFile
        //-------------------------------------------------------------------
        public static TraceConfiguration
           GetTraceConfiguration(
              string filename,
              bool isServer,
              string instance,
              int sqlVersion
           )
        {
            return GetTraceConfiguration(
               filename,
               filename,
               isServer,
               instance,
               sqlVersion
               );
        }



        #endregion

        #region Methods for Loading Configuration from Server

        //-------------------------------------------------------------------
        // LoadNewConfigurationFromServer
        //-------------------------------------------------------------------

        public static void
            LoadNewConfigurationFromServerAuditUsers(ServerAuditConfiguration config)
        {
            ErrorLog.Instance.Write(ErrorLog.Level.Debug, "Retrieving audit configurations from server.");
            RemoteAuditConfiguration newConfig = GetLatestAuditConfigurationFromServer(config.Instance);
            ErrorLog.Instance.Write(ErrorLog.Level.Debug, "Audit configurations received from server.");

            newConfig = SetAuditUsersConfiguration(config, newConfig, false);

            if ((newConfig.ServerRoles != null && newConfig.ServerRoles.Length > 0) ||
                (newConfig.Users != null && newConfig.Users.Length > 0))
            {
                config.Clear();
                CopyAuditUsersConfig(config, newConfig);
            }

            ErrorLog.Instance.Write(ErrorLog.Level.Debug, "Saving audit configurations to file.");
            SaveAuditConfigurationToFile(config, newConfig);
            ErrorLog.Instance.Write(ErrorLog.Level.Debug, "Audit configuration file saved.");
        }

        /// <summary>
        /// Get RemoteAuditConfiguration, save it into ServerAuditConfiguration and persist to an XML file.
        /// </summary>
        /// <param name="config"></param>
        public static void
           LoadNewConfigurationFromServer(ServerAuditConfiguration config)
        {
            ErrorLog.Instance.Write(ErrorLog.Level.Debug, "Retrieving audit configurations from server.");
            RemoteAuditConfiguration newConfig = GetLatestAuditConfigurationFromServer(config.Instance);
            ErrorLog.Instance.Write(ErrorLog.Level.Debug, "Audit configurations received from server.");

            SetConfigurationUpdatedFlag(config.Instance);

            newConfig = SetAuditUsersConfiguration(config, newConfig, true);

            CopyRemoteConfigToServerAuditConfig(config, newConfig);
            ErrorLog.Instance.Write(ErrorLog.Level.Debug, "Saving audit configurations to file.");
            SaveAuditConfigurationToFile(config, newConfig);
            ErrorLog.Instance.Write(ErrorLog.Level.Debug, "Audit configuration file saved.");
            SQLcomplianceAgent.Instance.UpdateisEnabledValue(config.Instance, config.IsEnabled);
        }

        private static RemoteAuditConfiguration SetAuditUsersConfiguration(ServerAuditConfiguration config,
            RemoteAuditConfiguration newConfig, bool isFullUpdate)
        {
            //5.6 - Trusted users at server level
            newConfig.ServerTrustedUsers = GetTrustedUsers(config.Instance, newConfig.ServerTrustedUsers,
                newConfig.ServerTrustedUsersServerRoles, isFullUpdate);

            newConfig.AuditedUsers = GetAuditedPrivilegedUsers(config.Instance,
                                                                newConfig.Users,
                   newConfig.ServerRoles, isFullUpdate);

            if (newConfig.DBConfigs != null)
            {
                for (int i = 0; i < newConfig.DBConfigs.Length; i++)
                {
                    if ((newConfig.DBConfigs[i].ServerRoles != null && newConfig.DBConfigs[i].ServerRoles.Length != 0) ||
                       (newConfig.DBConfigs[i].Users != null && newConfig.DBConfigs[i].Users.Length != 0))
                    {
                        newConfig.DBConfigs[i].TrustedUsers =
                           GetAuditedPrivilegedUsers(config.Instance,
                                                    newConfig.DBConfigs[i].Users,
                                                    newConfig.DBConfigs[i].ServerRoles);
                    }

                    if ((newConfig.DBConfigs[i].PrivServerRoles != null && newConfig.DBConfigs[i].PrivServerRoles.Length != 0) ||
                        (newConfig.DBConfigs[i].PrivUsers != null && newConfig.DBConfigs[i].PrivUsers.Length != 0))
                    {
                        newConfig.DBConfigs[i].PrivUsers =
                            GetAuditedPrivilegedUsers(config.Instance,
                                                       newConfig.DBConfigs[i].PrivUsers,
                                   newConfig.DBConfigs[i].PrivServerRoles, isFullUpdate);
                    }
                }
            }
            return newConfig;
        }


        //-------------------------------------------------------------------
        // CopyRemoteConfigToServerAuditConfig
        //-------------------------------------------------------------------
        /// <summary>
        /// Copy the content of a RemoteAuditConfiguration into ServerAuditConfiguration.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="newConfig"></param>
        public static void CopyRemoteConfigToServerAuditConfig(ServerAuditConfiguration config, RemoteAuditConfiguration newConfig)
        {

            config.Clear();
            if (newConfig.BaseConfig.Categories != null)
                foreach (AuditCategory category in newConfig.BaseConfig.Categories)
                    config.AddAuditedCategory(category);

            //v5.6 SQLCM-5373
            if (newConfig.ServerTrustedUsersServerRoles != null) // this method will add the server level trusted userss
                foreach (int serverRole in newConfig.ServerTrustedUsersServerRoles)
                    config.AddAuditedServerTrustedUserRole(serverRole);

            if (newConfig.ServerTrustedUsers != null)
            {
                foreach (string userName in newConfig.ServerTrustedUsers)
                {
                    config.AddAuditedServerTrustedUser(userName);
                }
            }

            if ((newConfig.ServerRoles != null && newConfig.ServerRoles.Length > 0) ||
                (newConfig.Users != null && newConfig.Users.Length > 0))
            {
                CopyAuditUsersConfig(config, newConfig);

                if (config.UserConfiguration == null)
                    config.UserConfiguration = new AuditConfiguration();
                foreach (AuditCategory category in newConfig.UserConfig.Categories)
                    config.UserConfiguration.AddAuditedCategory(category);
                config.UserConfiguration.AuditCaptureDetails = newConfig.UserConfig.CaptureDetails;

                config.UserConfiguration.AuditCaptureDetailsXE = newConfig.UserConfig.CaptureDetailsXE;
                config.UserConfiguration.AuditCaptureTransactions = newConfig.UserConfig.CaptureTransactions;
                config.UserConfiguration.AuditCaptureDDL = newConfig.UserConfig.CaptureDDL;
                config.UserConfiguration.AuditExceptions = newConfig.UserConfig.Exceptions;
                config.UserConfiguration.AuditAccessCheck = (AccessCheckFilter)newConfig.UserConfig.AccessCheck;
            }


            if (newConfig.AuditObjects != null)
                foreach (int obj in newConfig.AuditObjects)
                    config.AddAuditedObject(obj);

            // Copy the database audit configuations for all the audited databases
            if (newConfig.DBConfigs != null &&
                newConfig.DBConfigs.Length > 0)
            {
                if (config.isServer)
                {
                    foreach (DBRemoteAuditConfiguration remoteConfig in newConfig.DBConfigs)
                    {
                        config.AuditDatabase(GetDBAuditConfiguration(remoteConfig));
                        if (remoteConfig.DataChangeTables != null &&
                            remoteConfig.DataChangeTables.Length > 0)
                        {
                            config.CaptureDataChanges = true;
                        }

                        if (remoteConfig.SensitiveColumns != null &&
                            remoteConfig.SensitiveColumns.Length > 0)
                        {
                            config.AuditSensitiveColumns = true;
                        }
                        // SQLCM-5471 v5.6 Add Activity to Senstitive columns
                        config.AuditSensitiveColumnActivity = (SensitiveColumnActivity)remoteConfig.AuditSensitiveColumnActivity;
                        config.AuditSensitiveColumnActivityDataset = (SensitiveColumnActivity)remoteConfig.AuditSensitiveColumnActivityDataset;
                    }
                }
                else
                {
                    Hashtable dbInfo = GetDbIds(config.Instance, newConfig.DBConfigs);

                    for (int i = 0; i < newConfig.DBConfigs.Length; i++)
                    {
                        try
                        {
                            if (dbInfo.ContainsKey(newConfig.DBConfigs[i].dbName))
                            {
                                newConfig.DBConfigs[i].DbId = (int)dbInfo[newConfig.DBConfigs[i].dbName];
                                if (newConfig.DBConfigs[i].DataChangeTables != null &&
                                    newConfig.DBConfigs[i].DataChangeTables.Length > 0)
                                {
                                    config.CaptureDataChanges = true;
                                }

                                if (newConfig.DBConfigs[i].SensitiveColumns != null &&
                                    newConfig.DBConfigs[i].SensitiveColumns.Length > 0)
                                {
                                    config.AuditSensitiveColumns = true;
                                }
                                config.AuditDatabase(GetDBAuditConfiguration(newConfig.DBConfigs[i]));
                            }
                        }
                        catch (Exception e)
                        {
                            ErrorLog.Instance.Write(ErrorLog.Level.Debug, e, true);
                        }
                    }
                }
            }

            config.AuditAccessCheck = (AccessCheckFilter)newConfig.AccessCheck;
            config.AuditCaptureDetails = newConfig.CaptureDetails;
            config.AuditCaptureTransactions = newConfig.CaptureTransactions;
            config.AuditExceptions = newConfig.Exceptions;
            config.Version = newConfig.Version;
            config.IsSQLsecure = newConfig.IsSQLsecure;
            config.IsEnabled = newConfig.IsEnabled;
            config.SQLsecureDBIds = newConfig.sqlsecureDBIds;
            config.LastModifiedTime = newConfig.LastModifiedTime;
            config.AuditCaptureSQLXE = newConfig.AuditCaptureSQLXE;
            //5.5 Audit Logs
            config.IsAuditLogsEnabled = newConfig.IsAuditLogEnabled;
        }

        private static void CopyAuditUsersConfig(ServerAuditConfiguration config, RemoteAuditConfiguration newConfig)
        {
            if (newConfig.ServerRoles != null)
                foreach (int serverRole in newConfig.ServerRoles)
                    config.AddAuditedServerRole(serverRole);

            if (newConfig.Users != null)
                foreach (string group in newConfig.Users)
                    config.AddAuditedUserGroup(@group);

            if (newConfig.AuditedUsers != null)
            {
                foreach (string userName in newConfig.AuditedUsers)
                {
                    config.AddAuditedUser(userName);
                }
            }

            if (newConfig.PrivUsers != null)
            {
                foreach (string privUser in newConfig.PrivUsers)
                {
                    config.AddPrivUser(privUser);
                }
            }
            if (newConfig.ServerTrustedUsers != null)
            {
                foreach (string trustedUser in newConfig.ServerTrustedUsers)
                {
                    config.AddAuditedServerTrustedUser(trustedUser);
                }
            }
        }

        public static Hashtable GetDbIds(string instance, DBRemoteAuditConfiguration[] configs)
        {
            Hashtable dbIdList = new Hashtable();

            if (SQLcomplianceAgent.Instance == null || configs.Length == 0)
                return dbIdList;

            StringBuilder query = new StringBuilder();

            try
            {
                if (configs[0].dbName == CoreConstants.SQL2005SystemDatabase)
                    dbIdList.Add(CoreConstants.SQL2005SystemDatabase,
                                  CoreConstants.SQL2005SystemDatabaseId);

                query.AppendFormat(
                   "SELECT name, dbid from master.dbo.sysdatabases where name in ( {0}",
                   SQLHelpers.CreateSafeString(configs[0].dbName));
                for (int i = 1; i < configs.Length; i++)
                {
                    if (configs[i].dbName == CoreConstants.SQL2005SystemDatabase)
                        dbIdList.Add(CoreConstants.SQL2005SystemDatabase,
                                      CoreConstants.SQL2005SystemDatabaseId);
                    else
                        query.AppendFormat(", {0}",
                                            SQLHelpers.CreateSafeString(configs[i].dbName));
                }
                query.Append(")");
                GetIds(instance, query.ToString(), ref dbIdList);
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(String.Format("An error occurred retrieving IDs. Stmt: {0}",
                                                        query.ToString()),
                                         e,
                                         true);
            }
            return dbIdList;
        }

        public static Hashtable GetDbIds(string instance, DBAuditConfiguration[] configs)
        {
            Hashtable dbIdList = new Hashtable();

            if (SQLcomplianceAgent.Instance == null || configs.Length == 0)
                return dbIdList;

            StringBuilder query = new StringBuilder();

            try
            {
                if (configs[0].Name == CoreConstants.SQL2005SystemDatabase)
                    dbIdList.Add(CoreConstants.SQL2005SystemDatabase, CoreConstants.SQL2005SystemDatabaseId);

                query.AppendFormat("SELECT name, dbid from master.dbo.sysdatabases where name in ( {0}", SQLHelpers.CreateSafeString(configs[0].Name));

                for (int i = 1; i < configs.Length; i++)
                {
                    if (configs[i].Name == CoreConstants.SQL2005SystemDatabase)
                        dbIdList.Add(CoreConstants.SQL2005SystemDatabase, CoreConstants.SQL2005SystemDatabaseId);
                    else
                        query.AppendFormat(", {0}", SQLHelpers.CreateSafeString(configs[i].Name));
                }
                query.Append(")");
                GetIds(instance, query.ToString(), ref dbIdList);
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(String.Format("An error occurred retrieving IDs. Stmt: {0}", query.ToString()), e, true);
            }
            return dbIdList;
        }

        private static void GetIds(string instance, string cmd, ref Hashtable dbIdList)
        {
            using (SqlConnection conn = SQLcomplianceAgent.GetConnection(instance))
            {
                using (SqlCommand command = new SqlCommand(cmd, conn))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                dbIdList.Add((string)reader.GetSqlString(0), (int)reader.GetSqlInt16(1));
                            }
                            reader.Close();
                        }
                    }
                }
            }
        }

        //-------------------------------------------------------------------
        // GetDBAuditConfiguration
        //-------------------------------------------------------------------
        /// <summary>
        /// Copy the content of a DBRemoteAuditConfiguration struct to a DBAuditConfiguration
        /// object.
        /// </summary>
        /// <param name="dbRemoteConfig"></param>
        /// <returns></returns>
        public static DBAuditConfiguration GetDBAuditConfiguration(DBRemoteAuditConfiguration dbRemoteConfig)
        {
            DBAuditConfiguration auditConfig = new DBAuditConfiguration(dbRemoteConfig.DbId);

            auditConfig.Name = dbRemoteConfig.dbName;

            foreach (AuditCategory category in dbRemoteConfig.Categories)
                auditConfig.AddAuditedCategory(category);

            auditConfig.AuditCaptureDetails = dbRemoteConfig.CaptureDetails;
            auditConfig.AuditCaptureTransactions = dbRemoteConfig.CaptureTransactions;
            auditConfig.AuditCaptureDDL = dbRemoteConfig.CaptureDDL;
            auditConfig.AuditAccessCheck = (AccessCheckFilter)dbRemoteConfig.AccessCheck;
            auditConfig.AuditExceptions = dbRemoteConfig.Exceptions;

            if (dbRemoteConfig.ObjectTypes != null)
                foreach (DBObjectType type in dbRemoteConfig.ObjectTypes)
                    auditConfig.AuditObjectType(type, true);

            if (dbRemoteConfig.AuditObjects != null)
            {
                foreach (int dbObject in dbRemoteConfig.AuditObjects)
                    auditConfig.AddAuditedObject(dbObject);
            }

            if (dbRemoteConfig.ServerRoles != null)
            {
                foreach (int serverRole in dbRemoteConfig.ServerRoles)
                    auditConfig.AddAuditedServerRole(serverRole);
            }

            if (dbRemoteConfig.Users != null)
            {
                foreach (string user in dbRemoteConfig.Users)
                    auditConfig.AddAuditedUserGroup(user);
            }

            if (dbRemoteConfig.TrustedUsers != null)
            {
                auditConfig.AuditedUsers = dbRemoteConfig.TrustedUsers;
            }

            if (dbRemoteConfig.DataChangeTables != null)
            {
                auditConfig.DataChangeTables = dbRemoteConfig.DataChangeTables;
            }

            if (dbRemoteConfig.SensitiveColumns != null)
            {
                auditConfig.SensitiveColumns = dbRemoteConfig.SensitiveColumns;
            }
            auditConfig.Version = dbRemoteConfig.Version;

            if (dbRemoteConfig.PrivServerRoles != null)
            {
                foreach (int serverRole in dbRemoteConfig.PrivServerRoles)
                    auditConfig.AddAuditedDBRole(serverRole);
            }

            if (dbRemoteConfig.PrivUsers != null)
            {
                foreach (string user in dbRemoteConfig.PrivUsers)
                    auditConfig.AddPrivUser(user);   //AddAuditedUserGroup( user );
            }

            if (dbRemoteConfig.UserCategories != null)
            {
                foreach (AuditCategory category in dbRemoteConfig.UserCategories)
                {
                    auditConfig.AddAuditUserCategory(category);
                }
            }

            auditConfig.UserCategories = auditConfig.AuditUserCategories;
            auditConfig.AuditUserCaptureSql = dbRemoteConfig.UserCaptureSql;
            auditConfig.AuditUserCaptureTransactions = dbRemoteConfig.UserCaptureTran;
            auditConfig.AuditUserCaptureDDL = dbRemoteConfig.UserCaptureDDL;
            auditConfig.AuditUserAccessCheck = (AccessCheckFilter)dbRemoteConfig.UserAccessCheck;
            auditConfig.AuditUserExceptions = dbRemoteConfig.UserExceptions;
            auditConfig.UserCaptureSql = auditConfig.AuditUserCaptureSql;
            auditConfig.UserCaptureTran = auditConfig.AuditUserCaptureTransactions;
            auditConfig.UserExceptions = dbRemoteConfig.UserExceptions;
            auditConfig.UserAccessCheck = dbRemoteConfig.UserAccessCheck;
            auditConfig.PrivServerRoles = dbRemoteConfig.PrivServerRoles;
            auditConfig.PrivUsers = dbRemoteConfig.PrivUsers;
            //v5.6 SQLCM-5471
            auditConfig.AuditSensitiveColumnActivity = (SensitiveColumnActivity)dbRemoteConfig.AuditSensitiveColumnActivity;
            auditConfig.AuditSensitiveColumnActivityDataset = (SensitiveColumnActivity)dbRemoteConfig.AuditSensitiveColumnActivityDataset;
            return auditConfig;
        }

        //-------------------------------------------------------------------
        // GetDatabaseNames: this function can only be called on the server
        //                   side.  Do not use it on the agent side.
        //-------------------------------------------------------------------
        private static string[] GetDatabaseNames(string instance, int[] dbIds)
        {
            if (dbIds == null ||
                dbIds.Length == 0)
                return null;

            ArrayList dbNames = new ArrayList();
            Repository rep = new Repository();
            SqlCommand command = null;
            SqlDataReader reader = null;

            try
            {
                rep.OpenConnection();
                StringBuilder builder = new StringBuilder(500);
                builder.AppendFormat("SELECT name from {0} WHERE srvInstance = {1} "
                                    + " and isEnabled = 1  "
                                    + " and sqlDatabaseId in ( ",
                                    CoreConstants.RepositoryDatabaseTable,
                                    SQLHelpers.CreateSafeString(instance));
                builder.AppendFormat(" {0}", dbIds[0]);
                for (int i = 1; i < dbIds.Length; i++)
                    builder.AppendFormat(", {0}", dbIds[i]);
                builder.Append(" )");

                command = new SqlCommand(builder.ToString(), rep.connection);
                reader = command.ExecuteReader();

                if (reader != null)
                {
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(0))
                            dbNames.Add(reader.GetString(0).ToUpper());
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write("An error occurred retrieving database names for trace processing.", e, true);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                reader = null;

                if (command != null)
                    command = null;

                rep.CloseConnection();
                rep = null;
            }

            return (string[])dbNames.ToArray(typeof(string));

        }
        #endregion

        #region Methods for Getting Audit Versions

        //-------------------------------------------------------------------
        // GetCurrentAuditVerion
        //-------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static int GetCurrentAuditVerion(ServerAuditConfiguration config)
        {
            int version = 0;
            IFormatter formatter = null;
            Stream stream = null;

            try
            {
                string filename = Path.Combine(config.TraceDirectory, config.InstanceAlias + ".bin");
                formatter = new BinaryFormatter();
                using (stream = new FileStream(filename,
                                               FileMode.Open,
                                               FileAccess.Read,
                                               FileShare.Read))
                {
                    RemoteAuditConfiguration remoteConfig = (RemoteAuditConfiguration)formatter.Deserialize(stream);

                    version = remoteConfig.Version;
                }

            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         CoreConstants.Exception_ErrorReadingAuditConfiguratioin,
                                         e,
                                         true);

            }
            finally
            {
                formatter = null;
            }
            return version;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static int
           GetLatestAuditVerion(
           string instance
           )
        {
            return GetRemoteAuditManager().GetLatestAuditVersion(instance);
        }

        #endregion

        #region Methods for Getting Audit Configurations Remotely
        //-------------------------------------------------------------------
        // GetLatestAuditConfigurationFromServer
        //-------------------------------------------------------------------
        /// <summary>
        /// Get remote audit manager proxy and make the call to get the configuration.
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static RemoteAuditConfiguration
           GetLatestAuditConfigurationFromServer(
              string instance
           )
        {
            return GetRemoteAuditManager().GetCurrentAuditConfiguration(instance);
        }


        //-------------------------------------------------------------------
        // GetRemoteAuditManager
        //-------------------------------------------------------------------
        /// <summary>
        /// Gets the RemoteAuditManager proxy
        /// </summary>
        /// <returns></returns>
        public static RemoteAuditManager
           GetRemoteAuditManager()
        {
            string remotingURL = EndPointUrlBuilder.GetUrl(typeof(RemoteAuditManager), server, serverPort);

            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                     String.Format("Getting RemoteAuditManager URL: {0}", remotingURL),
                                     ErrorLog.Severity.Informational);

            return CoreRemoteObjectsProvider.RemoteAuditManager(server, serverPort);

        }

        //
        // Get audited table names for a database
        //
        static public string[]
           GetAuditedTableNames(string instance,
              string dbName, string connString)
        {
            List<string> tables = new List<string>();


            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    // Note that we don't have valid object types in this table.  Treat everything as a table for now.
                    string query = String.Format("SELECT o.name,o.schemaName from {0} o INNER JOIN {1} d ON  o.dbId = d.dbId where d.srvInstance = {2} and d.name = '{3}'",
                                                  CoreConstants.RepositoryDatabaseObjectsTable,
                                                  CoreConstants.RepositoryDatabaseTable,
                                                  SQLHelpers.CreateSafeString(instance),
                                                  dbName);
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader == null)
                                return null;


                            while (reader.Read())
                            {
                                if (!reader.IsDBNull(0))
                                {

                                    tables.Add(String.Format("{0}.{1}", reader.GetString(1), reader.GetString(0)));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                string msg = String.Format(CoreConstants.Exception_ErrorReadingAuditedUserTables,
                                            CoreConstants.RepositoryDatabaseObjectsTable,
                                            dbName);
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         msg,
                                         e,
                                         true);
                throw;
            }

            return tables.ToArray();

        }

        #endregion

        #region Privileged User List

        public static string[]
           GetAuditedServerRoles(string instance, SqlConnection conn)
        {
            string userList = null;
            List<string> roles = new List<string>();
            try
            {
                string commandText = String.Format("SELECT auditUsersList FROM {0} WHERE instance = {1}",
                                                    CoreConstants.RepositoryServerTable,
                                                    SQLHelpers.CreateSafeString(instance));

                using (SqlCommand command = new SqlCommand(commandText,
                                                             conn))
                {
                    object obj = command.ExecuteScalar();
                    if (obj == null || obj is DBNull)
                        userList = "";
                    else userList = (string)obj;
                }

                UserList list = new UserList(userList);
                if (list.ServerRoles != null && list.ServerRoles.Length > 0)
                {
                    foreach (ServerRole role in list.ServerRoles)
                        roles.Add(role.Name);
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         "An error occurred retrieving audited privileged user list.",
                                         e,
                                         true);
            }

            return roles.ToArray();
        }

        // Retrieve the UserList from a server record.
        public static UserList
           GetAuditedUserList(string instance, SqlConnection conn)
        {
            UserList userList = null;
            try
            {
                string commandText = String.Format("SELECT auditUsersList FROM {0} WHERE instance = {1}",
                                                    CoreConstants.RepositoryServerTable,
                                                    SQLHelpers.CreateSafeString(instance));

                using (SqlCommand command = new SqlCommand(commandText,
                                                             conn))
                {
                    object obj = command.ExecuteScalar();
                    if (obj != null && !(obj is DBNull))
                        userList = new UserList((string)obj);
                }

            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         "An error occurred retrieving audited privileged user list.",
                                         e,
                                         true);
            }

            return userList;
        }


        // Retrieve the UserList from a database record.
        public static UserList GetDatabaseAuditedPrevelegedUserList(string srvInstance, int sqlDatabaseId, SqlConnection conn)
        {
            UserList userList = null;
            try
            {
                string commandText = String.Format("SELECT auditPrivUsersList FROM {0} WHERE sqlDatabaseId = {1} and srvInstance = {2}",
                                                    CoreConstants.RepositoryDatabaseTable,
                                                    sqlDatabaseId.ToString(),
                                                    SQLHelpers.CreateSafeString(srvInstance));

                using (SqlCommand command = new SqlCommand(commandText,
                                                             conn))
                {
                    object obj = command.ExecuteScalar();
                    if (obj != null && !(obj is DBNull))
                        userList = new UserList((string)obj);
                }

            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         "An error occurred retrieving audited privileged user list.",
                                         e,
                                         true);
            }

            return userList;
        }


        // Retrieve the trusted UserList from a database record.
        public static UserList
           GetTrustedUserList(string instance, string database, string connStr)
        {
            UserList userList = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    string commandText =
                       String.Format(
                          "SELECT auditUsersList FROM {0} WHERE srvInstance = {1} and name = {2}",
                          CoreConstants.RepositoryDatabaseTable,
                          SQLHelpers.CreateSafeString(instance),
                          SQLHelpers.CreateSafeString(database));

                    using (SqlCommand command = new SqlCommand(commandText,
                                                                 conn))
                    {
                        object obj = command.ExecuteScalar();
                        if (obj != null && !(obj is DBNull))
                            userList = new UserList((string)obj);
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         "An error occurred retrieving trusted user list.",
                                         e,
                                         true);
            }

            return userList;
        }

        //-------------------------------------------------------------------
        // GetTrustesUsers for the Server Instance //v5.6 SQLCM-5373
        //-------------------------------------------------------------------
        public static string[]
         GetTrustedUsers(
            string instance,
            string[] users,
            int[] trustedUserServerRoles,
          bool isFullUpdate = true
         )
        {
            bool replace = true;
            string instanceUpper = instance.ToUpper();
            UserCache cache;

            if (trustedUsersCache.ContainsKey(instanceUpper))
            {
                if (isFullUpdate)
                {
                    if (!TimeToReenumerateUsers(instanceUpper))
                        return trustedUsersCache[instanceUpper].GetTrustedUsers(trustedUserServerRoles, users);
                }

                cache = trustedUsersCache[instanceUpper];
            }
            else
            {
                replace = false;
                cache = new UserCache(instanceUpper);
                trustedUsersCache.Add(instanceUpper, cache);
            }

            cache.RefreshTrustedUsers(trustedUserServerRoles, users);

            UpdateStatus(instanceUpper, replace);

            return cache.GetTrustedUsers(trustedUserServerRoles, users);

        }


        //-------------------------------------------------------------------
        // GetAuditedPrivilegedUsers
        //-------------------------------------------------------------------
        public static string[]
           GetAuditedPrivilegedUsers(
              string instance,
              string[] users,
              int[] serverRoles,
            bool isFullUpdate = true
           )
        {
            bool replace = true;
            string instanceUpper = instance.ToUpper();
            UserCache cache;

            if (auditedUsersCache.ContainsKey(instanceUpper))
            {
                if (isFullUpdate)
                {
                    if (!TimeToReenumerateUsers(instanceUpper))
                        return auditedUsersCache[instanceUpper].GetAuditedUsers(serverRoles, users);
                }

                cache = auditedUsersCache[instanceUpper];
            }
            else
            {
                replace = false;
                cache = new UserCache(instanceUpper);
                auditedUsersCache.Add(instanceUpper, cache);
            }

            cache.Refresh(serverRoles, users);

            UpdateStatus(instanceUpper, replace);

            return cache.GetAuditedUsers(serverRoles, users);

            /*
            ArrayList auditedUsers = new ArrayList();
            try
            {
               auditedUsers.AddRange( UserList.ExplodeGroupsToUserList( instance, users ) );
            }
            catch( ArgumentNullException )
            {}  // ignore null exception

            try
            {
               auditedUsers.AddRange( UserList.GetAuditedServerRoleUsers( instance,
                                                                          serverRoles ) );
            }
            catch( ArgumentNullException )
            {}

            // eliminate duplicate names
            Hashtable au = new Hashtable();
            for( int i = 0; i < auditedUsers.Count; i++ )
            {
               try
               {
                   if(!au.Contains(auditedUsers[i]))
                       au.Add( auditedUsers[i], auditedUsers[i] );
               }
               catch{}
            }

            string [] tmpusers = new string[ au.Count];
            int idx = 0;
            IDictionaryEnumerator enumerator = au.GetEnumerator();
            while( enumerator.MoveNext() )
            {
               tmpusers[idx++] = (string)enumerator.Value;
            }

            UpdateStatus( instanceUpper, replace, tmpusers );

            return tmpusers;
            */

        }

        //-------------------------------------------------------------------
        // TimeToReenumerateUsers
        //-------------------------------------------------------------------
        protected static bool
           TimeToReenumerateUsers(
              string instance
           )
        {
            if ((bool)configurationUpdated[instance])
                return true;
            else if (!enumerationTimeTable.Contains(instance))
            {
                try
                {
                    Hashtable table = Hashtable.Synchronized(enumerationTimeTable);
                    table.Add(instance, DateTime.Now);
                    table = null;
                }
                catch { }

                return true;
            }
            else if (DateTime.Now >= (DateTime)enumerationTimeTable[instance])
                return true;
            return false;
        }

        //-------------------------------------------------------------------
        // UpdateStatus
        //-------------------------------------------------------------------
        protected static void
           UpdateStatus(
              string instance,
              bool replace
           )
        {
            try
            {
                if (replace)
                {
                    configurationUpdated[instance] = false;
                }
                else
                {
                    bool updated = false;
                    configurationUpdated.Add(instance, updated);
                }
            }
            catch { }

            enumerationTimeTable[instance] = DateTime.Now + enumerationInterval;

        }

        //-------------------------------------------------------------------
        // UpdateStatus
        //-------------------------------------------------------------------
        internal static void
           SetConfigurationUpdatedFlag(
              string instance
           )
        {
            try
            {
                configurationUpdated[instance.ToUpper()] = true;
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(String.Format("An error occurred setting configuration updated flag for {0}.",
                                                        instance),
                                         e,
                                         true);
            }

        }

        internal static List<string> GetPrivUsers(ServerAuditConfiguration auditConfig)
        {
            List<string> pUsers = new List<string>();
            string[] tmpUser = auditConfig.AuditedUsers;
            if (tmpUser != null)
            {
                for (int i = 0; i < tmpUser.Length; i++)
                {
                    if (!pUsers.Contains(tmpUser[i]))
                        pUsers.Add(tmpUser[i]);
                }
            }

            tmpUser = auditConfig.AuditedPrivUsers;
            if (tmpUser != null)
            {
                for (int i = 0; i < tmpUser.Length; i++)
                {
                    if (!pUsers.Contains(tmpUser[i]))
                        pUsers.Add(tmpUser[i]);
                }
            }


            return pUsers;
        }

        #endregion

        #region Data Change Tables

        #endregion
    }
}
