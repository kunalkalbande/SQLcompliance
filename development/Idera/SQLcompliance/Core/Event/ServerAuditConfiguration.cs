using System;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using System.Configuration;

namespace Idera.SQLcompliance.Core.Event
{
    /// <summary>
    /// Summary description for AuditConfiguration.
    /// </summary>
    public class ServerAuditConfiguration : AuditConfiguration
    {
        #region Private or Protected Fields

        protected Hashtable dbConfigs;
        protected string instance;
        protected string instanceAlias;
        protected bool isSQLsecure;
        protected bool isEnabled;
        protected bool isFirstTimeUpdate = false;
        protected string[] sqlsecureDBs;
        protected int[] sqlsecureDBIds;
        protected Hashtable sqlsecureDBList;
        protected AuditConfiguration userConfiguration;
        protected DateTime lastModifiedTime;
        protected string traceDirectory = "";
        public bool isServer = false;
        public int sqlVersion = 0;
        public bool CaptureDataChanges = false;
        public bool AuditSensitiveColumns = false;

        public bool enableRowCountForDatabaseAuditing = false;
        #endregion

        #region Public Properties


        public string TraceDirectory
        {
            get { return traceDirectory; }
            set { traceDirectory = value; }
        }

        public string Instance
        {
            get { return instance; }
            set { instance = value; }
        }

        public string InstanceAlias
        {
            get { return instanceAlias; }
            set { instanceAlias = value; }
        }


        public bool IsSQLsecure
        {
            get { return isSQLsecure; }
            set { isSQLsecure = value; }
        }

        public bool IsEnabled
        {
            get { return isEnabled; }
            set { isEnabled = value; }
        }

        public bool IsFirstTimeUpdate
        {
            get { return isFirstTimeUpdate; }
            set { isFirstTimeUpdate = value; }
        }

        public string[] SQLsecureDBs
        {
            get { return sqlsecureDBs; }
            set { sqlsecureDBs = value; }
        }

        public int[] SQLsecureDBIds
        {
            get { return sqlsecureDBIds; }
            set
            {
                sqlsecureDBIds = value;
                sqlsecureDBList.Clear();
                if (sqlsecureDBIds != null &&
                   sqlsecureDBIds.Length > 0)
                {
                    try
                    {
                        foreach (int id in sqlsecureDBIds)
                        {
                            sqlsecureDBList.Add(id, id);
                        }
                    }
                    catch
                    {
                        // ignore duplicates and null exceptions
                    }
                }
            }
        }

        public AuditConfiguration UserConfiguration
        {
            get { return userConfiguration; }
            set { userConfiguration = value; }
        }

        // Read only and only apply to server level audit
        public bool AuditAuditChanges
        {
            get { return auditedCategories.ContainsKey(AuditCategory.Audit); }
        }

        public bool EnableRowCountForDatabaseAuditing
        {
            get { return enableRowCountForDatabaseAuditing; }
        }
      


        //-----------------------------------------------------------
        // AuditDBList
        //-----------------------------------------------------------
        /// <summary>
        /// AuditDBList: get the list of databases in the audit setting.
        /// </summary>
        /// <returns></returns>
        public DBAuditConfiguration[] AuditDBList
        {
            get
            {
                if (dbConfigs == null || dbConfigs.Count == 0)
                    return null;

                DBAuditConfiguration[] list = new DBAuditConfiguration[dbConfigs.Count];

                IDictionaryEnumerator enumerator = dbConfigs.GetEnumerator();
                int idx = 0;
                while (enumerator.MoveNext())
                    list[idx++] = (DBAuditConfiguration)enumerator.Value;

                return list;
            }
        }

        public DateTime LastModifiedTime
        {
            get { return lastModifiedTime; }
            set { lastModifiedTime = value; }
        }

        #endregion

        #region Constructors


        public ServerAuditConfiguration(
           )
        {
            dbConfigs = new Hashtable();
            sqlsecureDBList = new Hashtable();
            // Audit Changes is always on.
            EnableAuditedCategory(AuditCategory.Audit, true);
            this.enableRowCountForDatabaseAuditing = GetEnableRowCountForDatabaseAuditingValueFromAppConfig();

        }

        #endregion

        #region Public Methods
        //-----------------------------------------------------------
        // AuditDatabase
        //-----------------------------------------------------------
        /// <summary>
        /// AddDBAuditSettings: add/remove a database for auditing.  Adding
        /// a database already in the configuration overwrites its previous
        /// setting.
        /// </summary>
        /// <param name="ddl"></param>
        /// <param name="dml"></param>
        /// <param name="select"></param>
        /// <param name="captureDetails"></param>
        /// <param name="exceptions"></param>
        /// <returns></returns>
        public bool
           AuditDatabase
           (
              int dbId,
              bool ddl,
              bool dml,
              bool security,
              bool select,
              bool admin,
              AccessCheckFilter accessCheck,
              bool captureDetails,
              bool captureTransactions,
              bool exceptions,
              bool enable,  // true means add this database for auditing, false means remove it
              bool userDDL,
              bool userDML,
              bool userSecurity,
              bool userSelect,
              bool userAdmin,
              AccessCheckFilter userAccessCheck,
              bool userCaptureSql,
              bool userCaptureTransaction,
              bool userException,
            SensitiveColumnActivity auditSensitiveColumnActivity, // SQLCM-5471 v5.6 Add Activity to Senstitive columnss
            SensitiveColumnActivity auditSensitiveColumnActivityDataset// SQLCM-5471 v5.6 Add Activity to Senstitive columnss
           )
        {
            if (dbId == 0)
                return false;

            bool exists = dbConfigs.ContainsKey(dbId);

            if (enable)
            {
                DBAuditConfiguration dbConfig = new DBAuditConfiguration(dbId,
                   ddl,
                   dml,
                   security,
                   select,
                   admin,
                   accessCheck,
                   captureDetails,
                   captureTransactions,
                   exceptions,
                   userDDL,
                   userDML,
                   userSecurity,
                   userSelect,
                   userAdmin,
                   userAccessCheck,
                   userCaptureSql,
                   userCaptureTransaction,
                   userException,
                   auditSensitiveColumnActivity,
                   auditSensitiveColumnActivityDataset);// SQLCM-5471 v5.6 Add Activity to Senstitive columns);


                if (exists)  // already in the database list, change the settings instead
                {
                    DBAuditConfiguration config = (DBAuditConfiguration)dbConfigs[dbId];
                    config.AuditCaptureDetails = captureDetails;
                    config.AuditCaptureTransactions = captureTransactions;
                    config.AuditExceptions = exceptions;
                    config.AuditDDL = ddl;
                    config.AuditDML = dml;
                    config.AuditAdmin = admin;
                    config.AuditAccessCheck = accessCheck;
                    config.AuditSecurity = security;
                    config.AuditSELECT = select;
                    config.AuditUserCaptureSql = userCaptureSql;
                    config.AuditUserCaptureTransactions = userCaptureTransaction;
                    config.AuditUserExceptions = userException;
                    config.AuditUserDDL = userDDL;
                    config.AuditUserDML = userDML;
                    config.AuditUserSecurity = userSecurity;
                    config.AuditUserSELECT = userSelect;
                    config.AuditUserAdmin = userAdmin;
                    config.AuditUserAccessCheck = userAccessCheck;
                    config.AuditSensitiveColumnActivity = auditSensitiveColumnActivity;
                    config.AuditSensitiveColumnActivityDataset = auditSensitiveColumnActivityDataset; // SQLCM-5471 v5.6 Add Activity to Senstitive columnss
                }
                else
                    dbConfigs.Add(dbId, dbConfig);
            }
            else // remove it
            {
                if (exists)
                    dbConfigs.Remove(dbId);
            }

            return true;
        }

        //-----------------------------------------------------------
        // AuditDatabase
        //-----------------------------------------------------------
        /// <summary>
        /// Add a database audit configuration to the server's audit configuration
        /// </summary>
        /// <param name="dbAuditConfig"></param>
        public void
           AuditDatabase(
              DBAuditConfiguration dbAuditConfig)
        {
            if (!dbConfigs.ContainsKey(dbAuditConfig.DBId))
                dbConfigs.Add(dbAuditConfig.DBId, dbAuditConfig);
        }


        //-----------------------------------------------------------
        // AuditObjectType
        //-----------------------------------------------------------
        /// <summary>
        /// AuditObjectType enables/disables the auditing for a particular type of object in a database.
        /// </summary>
        /// <param name="type"></param>
        public void
           AuditObjectType
           (
              int dbId,
              DBObjectType type,
              bool enable
           )
        {
            if (dbId == 0)
                throw (new ArgumentException("dbId"));
            else if (!DBObject.IsValidType(type))
                throw (new ArgumentException(String.Format("Invalid object type : {0}", type), "type"));

            if (!dbConfigs.ContainsKey(dbId))
                AuditDatabase(dbId,
                               AuditDDL,
                               AuditDML,
                               AuditSecurity,
                               AuditSELECT,
                               AuditAdmin,
                               AuditAccessCheck,
                               AuditCaptureDetails,
                               AuditCaptureTransactions,
                               AuditExceptions,
                               true,
                               AuditUserDDL,
                               AuditUserDML,
                               AuditUserSecurity,
                               AuditUserSELECT,
                               AuditUserAdmin,
                               AuditUserAccessCheck,
                               AuditUserCaptureSql,
                               AuditUserCaptureTransactions,
                               AuditUserExceptions,
                               AuditSensitiveColumnActivity,
                               AuditSensitiveColumnActivityDataset);// SQLCM-5471 v5.6 Add Activity to Senstitive columns

            DBAuditConfiguration config = (DBAuditConfiguration)dbConfigs[dbId];

            config.AuditObjectType(type, enable);

        }


        //-----------------------------------------------------------
        // AuditTable
        //-----------------------------------------------------------
        /// <summary>
        /// AuditTable: add a table to the audit setting.
        /// </summary>
        /// <returns></returns>
        public void
           AuditTable(
              int dbId,
              int tableId
           )
        {
            if (dbId == 0)
                throw (new ArgumentException("dbId"));
            if (tableId == 0)
                throw (new ArgumentException("tableId"));


            if (!dbConfigs.ContainsKey(dbId))
                AuditDatabase(dbId,
                               AuditDDL,
                               AuditDML,
                               AuditSecurity,
                               AuditSELECT,
                               AuditAdmin,
                               AuditAccessCheck,
                               AuditCaptureDetails,
                               AuditCaptureTransactions,
                               AuditExceptions,
                               true,
                               AuditUserDDL,
                               AuditUserDML,
                               AuditUserSecurity,
                               AuditUserSELECT,
                               AuditUserAdmin,
                               AuditUserAccessCheck,
                               AuditUserCaptureSql,
                               AuditUserCaptureTransactions,
                               AuditUserExceptions,
                               AuditSensitiveColumnActivity,
                               AuditSensitiveColumnActivityDataset);// SQLCM-5471 v5.6 Add Activity to Senstitive columns

            DBAuditConfiguration config = (DBAuditConfiguration)dbConfigs[dbId];

            config.AuditTable(tableId, true);

        }

        //-----------------------------------------------------------
        // AuditObject
        //-----------------------------------------------------------
        /// <summary>
        /// AuditObject: add an object to the audit setting.
        /// </summary>
        /// <returns></returns>
        public void
           AuditObject(
              int dbId,
              int objectId,
              DBObjectType type
           )
        {
            if (dbId == 0)
                throw (new ArgumentException("dbId"));
            else if (objectId == 0)
                throw (new ArgumentNullException("objectId"));
            else if (!DBObject.IsValidType(type))
                throw (new ArgumentException(String.Format("Invalid object type : {0}", (int)type), "type"));

            if (!dbConfigs.ContainsKey(dbId))
                this.AuditDatabase(dbId,
                   AuditDDL,
                   AuditDML,
                   AuditSecurity,
                   AuditSELECT,
                   AuditAdmin,
                   AuditAccessCheck,
                   AuditCaptureDetails,
                   AuditCaptureTransactions,
                   AuditExceptions,
                   true,
                   AuditUserDDL,
                   AuditUserDML,
                   AuditUserSecurity,
                   AuditUserSELECT,
                   AuditUserAdmin,
                   AuditUserAccessCheck,
                   AuditUserCaptureSql,
                   AuditUserCaptureTransactions,
                   AuditUserExceptions,
                   AuditSensitiveColumnActivity,
                   AuditSensitiveColumnActivityDataset);// SQLCM-5471 v5.6 Add Activity to Senstitive columns

            DBAuditConfiguration config = (DBAuditConfiguration)dbConfigs[dbId];

            config.AddAuditedObject(objectId);

        }



        //-----------------------------------------------------------
        // GenerateTraceConfigurations
        //-----------------------------------------------------------
        /// <summary>
        /// Generate trace configurations for the this audit configuration.
        /// </summary>
        /// <returns></returns>
        public virtual TraceConfiguration[]
           GenerateTraceConfigurations()
        {
            ArrayList configList = new ArrayList();

            configList.Add(GenerateServerOnlyTrace(false));

            TraceConfiguration[] traces = GenerateDBSecurityTraces(false);
            if (traces != null)
                foreach (TraceConfiguration trace in traces)
                    configList.Add(trace);

            // SQLCM-5850: BAD auditing not displaying data as expected
            var dataChangeList = new ArrayList();
            traces = GenerateDMLTraces(false);
            if (traces != null)
                foreach (TraceConfiguration trace in traces)
                {
                    if(trace.Category == TraceCategory.DataChange)
                    {
                        dataChangeList.Add(trace);
                        continue;
                    }
                    configList.Add(trace);
                }

            traces = GenerateSELECTTraces(false);
            if (traces != null)
                foreach (TraceConfiguration trace in traces)
                    configList.Add(trace);

            traces = GeneratePrivilegedUserTraces();
            if (traces != null)
                foreach (TraceConfiguration trace in traces)
                    configList.Add(trace);

            traces = GenerateDBPrivilegedUserTraces();
            if (traces != null)
                foreach (TraceConfiguration trace in traces)
                    configList.Add(trace);

            // SQLCM-5850: BAD auditing not displaying data as expected
            // Ensure Data Change entries comes at the end
            configList.AddRange(dataChangeList);

            return (TraceConfiguration[])configList.ToArray(typeof(TraceConfiguration));
        }
        //5.4 XE
        public virtual XeTraceConfiguration[]
          GenerateTraceConfigurationsXE()
        {
            ArrayList configList = new ArrayList();

            XeTraceConfiguration[] traces = GeneratePrivilegedUserTracesXE();
            if (traces != null)
                foreach (XeTraceConfiguration trace in traces)
                    configList.Add(trace);
            traces = GenerateDMLTracesXE(false);
            if (traces != null)
                foreach (XeTraceConfiguration trace in traces)
                    configList.Add(trace);

            traces = GenerateSELECTTracesXE(false);
            if (traces != null)
                foreach (XeTraceConfiguration trace in traces)
                    configList.Add(trace);

            traces = GenerateDBPrivilegedUserTracesXE();
            if (traces != null)
                foreach (XeTraceConfiguration trace in traces)
                    configList.Add(trace);

            return (XeTraceConfiguration[])configList.ToArray(typeof(XeTraceConfiguration));
        }
        
        //-------------------------------------------------------
        // Clear
        //-------------------------------------------------------
        /// <summary>
        /// Clear the configuration.
        /// </summary>
        public override void
           Clear()
        {
            dbConfigs.Clear();
            sqlsecureDBList.Clear();
            isSQLsecure = false;
            sqlsecureDBs = null;
            sqlsecureDBIds = null;
            if (userConfiguration != null)
                userConfiguration.Clear();
            userConfiguration = null;
            base.Clear();
        }


        #endregion

        #region Protected Methods

        #region Privileged User Trace
        //-----------------------------------------------------------
        // GeneratePrivilegedUserTraces
        //-----------------------------------------------------------
        /// <summary>
        /// Generate traces for auditing activities from users in fixed system roles.
        /// </summary>
        /// <returns></returns>
        protected virtual TraceConfiguration[]
           GeneratePrivilegedUserTraces()
        {

            if (this.UserConfiguration == null)
                return null;

            // Check if there are server roles selected
            if (!(auditServerRoleList != null ||
                   auditUserList != null ||
                   auditServerRoleList.Count > 0 ||
                   auditUserList.Count > 0))
                return null;

            ArrayList configList = new ArrayList();

            configList.Add(GenerateServerOnlyTrace(true));

            if (this.UserConfiguration.AuditAdmin ||
                this.UserConfiguration.AuditDDL ||
                this.UserConfiguration.AuditSecurity)
                configList.Add(GenerateDBSecurityTrace(this.UserConfiguration, true, 0));


            // If SQL statements are captured, separate DML and SELECT trace.
            // if( this.UserConfiguration.AuditCaptureDetails )
            if(!isAuditLogsEnabled || isServer)
            {
                if (this.UserConfiguration.AuditDML && !this.AuditCaptureSQLXE)
                {
                    configList.Add(GenerateDMLTrace(this.UserConfiguration, true, 0, null));
                }

                if (this.UserConfiguration.AuditSELECT && !this.AuditCaptureSQLXE)
                {
                    configList.Add(GenerateSELECTTrace(this.UserConfiguration, true, 0));
                }
            }
            /*
            else
            {
               if ( this.UserConfiguration.AuditDML )
                  configList.Add( GenerateDMLTrace( this.UserConfiguration, true, 0, null ) );
               if ( UserConfiguration.AuditSELECT )
                  configList.Add( GenerateSELECTTrace( this.UserConfiguration, true, 0 ) );
            }*/


            TraceConfiguration[] userTraces = MergeTraces(configList);


            foreach (TraceConfiguration config in userTraces)
            {
                config.KeepSQL = this.UserConfiguration.AuditCaptureDetails;
                config.KeepAdminSQL = this.userConfiguration.AuditAdmin;
            }

            return userTraces;

        }
        //5.4 XE
        protected virtual XeTraceConfiguration[]
          GeneratePrivilegedUserTracesXE()
        {

            if (this.UserConfiguration == null)
                return null;

            // Check if there are server roles selected
            if (!(auditServerRoleList != null ||
                   auditUserList != null ||
                   auditServerRoleList.Count > 0 ||
                   auditUserList.Count > 0) || (!(this.UserConfiguration.AuditDML || this.UserConfiguration.AuditSELECT)))
                return null;
            if (!this.AuditCaptureSQLXE)
                return null;
            ArrayList configList = new ArrayList();

            //If SQL statements are captured, separate DML and SELECT trace.
            if (this.AuditCaptureSQLXE)
            {
                configList.Add(GenerateDMLandSelectTraceXE(this.UserConfiguration, true, 0, null));
            }

            /*
            else
            {
               if ( this.UserConfiguration.AuditDML )
                  configList.Add( GenerateDMLTrace( this.UserConfiguration, true, 0, null ) );
               if ( UserConfiguration.AuditSELECT )
                  configList.Add( GenerateSELECTTrace( this.UserConfiguration, true, 0 ) );
            }*/


            XeTraceConfiguration[] userTraces = (XeTraceConfiguration[])configList.ToArray(typeof(XeTraceConfiguration));


            foreach (XeTraceConfiguration config in userTraces)
            {
                config.KeepSQLXE = this.UserConfiguration.AuditCaptureDetails;
                //config.KeepAdminSQL = this.userConfiguration.AuditAdmin;
            }

            return userTraces;

        }

        #endregion

        #region Database Privileged User Trace
        //-----------------------------------------------------------
        // GenerateDBPrivilegedUserTraces
        //-----------------------------------------------------------
        /// <summary>
        /// Generate traces for auditing activities from users in fixed system roles.
        /// </summary>
        /// <returns></returns>
        protected virtual TraceConfiguration[]
           GenerateDBPrivilegedUserTraces()
        {

            ArrayList traceSets = CreateConfigurationSets(AuditDBList, TraceCategory.DBPrivilegedUsers);
            if (traceSets == null)
                return null;

            int sequence = 0;
            int dmlSequence = 0;
            int selectSequence = 0;

            ArrayList traces = new ArrayList();
            ArrayList dbTraces = new ArrayList();
            TraceConfiguration trace;
            TraceConfiguration dmlTrace;
            TraceConfiguration selectTrace;
            foreach (DBAuditConfiguration config in traceSets)
            {
                for (int i = traces.Count - 1; i >= 0; i--)
                    traces.RemoveAt(i);
                if (!((config.PrivServerRoles != null && config.PrivServerRoles.Length > 0)
                    || (config.PrivUsers != null && config.PrivUsers.Length > 0)))
                    continue;

                trace = GenerateDBPrivilegedTrace(config, sequence);
                if (trace != null)
                {
                    sequence++;
                    traces.Add(trace);
                }

                int auditDML = Array.FindIndex(config.UserCategories, item => item == AuditCategory.DML);
                int auditSELECT = Array.FindIndex(config.UserCategories, item => item == AuditCategory.SELECT);

                if (!isAuditLogsEnabled || isServer)
                {
                    if (auditDML != -1 && !this.AuditCaptureSQLXE)
                    {
                        dmlTrace = GenerateDBPrivilegedDMLTrace(config, dmlSequence, null);
                        if (dmlTrace != null)
                        {
                            dmlSequence++;
                            traces.Add(dmlTrace);
                        }
                    }

                    if (auditSELECT != -1 && !this.AuditCaptureSQLXE)
                    {
                        selectTrace = GenerateDBPrivilegedSELECTTrace(config, selectSequence);
                        if (selectTrace != null)
                        {
                            selectSequence++;
                            traces.Add(selectTrace);
                        }
                    }
                }
                TraceConfiguration[] userTraces = MergeTraces(traces);
                if (userTraces != null)
                    foreach (TraceConfiguration dbTrace in userTraces)
                        dbTraces.Add(dbTrace);
            }


            return (TraceConfiguration[])dbTraces.ToArray(typeof(TraceConfiguration));
        }


        //5.4 XE
        protected virtual XeTraceConfiguration[]
          GenerateDBPrivilegedUserTracesXE()
        {
            if (!this.AuditCaptureSQLXE)
                return null;
            ArrayList traceSets = CreateConfigurationSets(AuditDBList, TraceCategory.DBPrivilegedUsers);
            if (traceSets == null)
                return null;

            int sequence = 0;

            ArrayList traces = new ArrayList();
            ArrayList dbTraces = new ArrayList();
            XeTraceConfiguration trace = null;
            foreach (DBAuditConfiguration config in traceSets)
            {
                for (int i = traces.Count - 1; i >= 0; i--)
                    traces.RemoveAt(i);
                if (!((config.PrivServerRoles != null && config.PrivServerRoles.Length > 0)
                    || (config.PrivUsers != null && config.PrivUsers.Length > 0)))
                    continue;

                int auditDML = Array.FindIndex(config.UserCategories, item => item == AuditCategory.DML);
                int auditSELECT = Array.FindIndex(config.UserCategories, item => item == AuditCategory.SELECT);
                if (auditDML != -1 || auditSELECT != -1)
                {
                    trace = new XeTraceConfiguration();
                    trace.Level = TraceLevel.Database;
                    trace.Sequence = sequence;
                    trace.Category = TraceCategory.DBPrivilegedUsers;
					//To support previous version of agent
                    if (this.isServer)
                    {
                        trace.AddEvent(TraceEventIdXE.SqlStmtStarting);
                        trace.AddEvent(TraceEventIdXE.SpStarting);
                    }
                    trace.AddEvent(TraceEventIdXE.SqlStmtCompleted);
                    trace.AddEvent(TraceEventIdXE.SpCompleted);
                    trace.AddColumn(TraceColumnIdXE.TextData);
                    trace.AddColumn(TraceColumnIdXE.ApplicationName);
                    trace.AddColumn(TraceColumnIdXE.ClientHostName);
                    trace.AddColumn(TraceColumnIdXE.DatabaseID);
                    trace.AddColumn(TraceColumnIdXE.DatabaseName);
                    trace.AddColumn(TraceColumnIdXE.EventSequence);
                    trace.AddColumn(TraceColumnIdXE.LinkedServerName);
                    trace.AddColumn(TraceColumnIdXE.StartTime);
                    trace.AddColumn(TraceColumnIdXE.IsSystem);
                    trace.AddColumn(TraceColumnIdXE.SessionLoginName);
                    trace.AddColumn(TraceColumnIdXE.SPID);
                    trace.AddColumn(TraceColumnIdXE.SQLSecurityLoginName);
                    if (config.AuditUserCaptureSql)
                    {
                        trace.KeepSQLXE = true;
                    }
                    if (auditDML != -1)
                    {
                        trace.AddFilter(new TraceFilter(TraceColumnId.Permissions,
                                                         TraceFilterComparisonOp.Equal,
                                                         2,         // Update
                                                         TraceFilterLogicalOp.OR));
                        trace.AddFilter(new TraceFilter(TraceColumnId.Permissions,
                                                         TraceFilterComparisonOp.Equal,
                                                         8,         // INSERT
                                                         TraceFilterLogicalOp.OR));
                        trace.AddFilter(new TraceFilter(TraceColumnId.Permissions,
                                                         TraceFilterComparisonOp.Equal,
                                                         16,         // DELETE
                                                         TraceFilterLogicalOp.OR));
                        trace.AddFilter(new TraceFilter(TraceColumnId.Permissions,
                                                         TraceFilterComparisonOp.Equal,
                                                         32,         // EXECUTE
                                                         TraceFilterLogicalOp.OR));
                    }
                    if (auditSELECT != -1)
                    {
                        trace.AddFilter(new TraceFilter(TraceColumnId.Permissions,
                                                 TraceFilterComparisonOp.Equal,
                                                 1,         // SELECT
                                                 TraceFilterLogicalOp.OR));
                    }

                    AddDataChangeEventFilter(trace);
                    AddSQLsecureApplicationFilters(trace);
                    AddDBPrivilegedUserFilters(config, trace);
                    AddDBIdFilters(trace, config.DBIdList);
                    trace.InstanceAlias = this.InstanceAlias;
                    trace.Version = this.Version;
                    trace.FileName = Path.Combine(traceDirectory,
                                                  TraceFileNameAttributes.ComposeTraceFileNameXE(trace.InstanceAlias,
                                                                                         (int)trace.Level,
                                                                                         (int)trace.Category,
                                                                                         trace.Version,
                                                                                         sequence));
                    trace.XESessionName = TraceFileNameAttributes.ComposeSessionNameXE(trace.InstanceAlias,
                                                                                         (int)trace.Level,
                                                                                         (int)trace.Category,
                                                                                         trace.Version,
                                                                                         sequence);
                    dbTraces.Add(trace);
                    sequence++;
                }
                else continue;
            }
            return (XeTraceConfiguration[])dbTraces.ToArray(typeof(XeTraceConfiguration));
        }


        //-----------------------------------------------------------
        // AddDBPrivilegedUserFilters
        //-----------------------------------------------------------
        private void
           AddDBPrivilegedUserFilters(
              DBAuditConfiguration config,
              TraceConfiguration trace
           )
        {

            if (config.PrivUsers.Length > 0)
            {
                trace.privUsers = config.PrivUsers;
                string[] users = config.PrivUsers;
                TraceFilter userFilter;
                for (int i = 0; i < users.Length; i++)
                {
                    userFilter = new TraceFilter(TraceColumnId.SQLSecurityLoginName,
                                                  TraceFilterComparisonOp.Like,
                                                  users[i],
                       (i == 0) ? TraceFilterLogicalOp.AND    // use AND for the last filter
                                                 : TraceFilterLogicalOp.OR);
                    trace.AddFilter(userFilter);
                }
            }
        }

        //5.4 XE
        private void
          AddDBPrivilegedUserFilters(
             DBAuditConfiguration config,
             XeTraceConfiguration trace
          )
        {

            if (config.PrivUsers.Length > 0)
            {
                trace.privUsers = config.PrivUsers;
                string[] users = config.PrivUsers;
                TraceFilter userFilter;
                for (int i = 0; i < users.Length; i++)
                {
                    userFilter = new TraceFilter(TraceColumnId.SQLSecurityLoginName,
                                                  TraceFilterComparisonOp.Like,
                                                  users[i],
                       (i == 0) ? TraceFilterLogicalOp.AND    // use AND for the last filter
                                                 : TraceFilterLogicalOp.OR);
                    trace.AddFilter(userFilter);
                }
            }
        }

        //5.4 XE
        private void
         excludeDBPrivilegedUserFilters(
            XeTraceConfiguration trace, string[] excludeUsers
         )
        {

            if (excludeUsers.Length > 0)
            {
                string[] users = excludeUsers;
                TraceFilter userFilter;
                for (int i = 0; i < users.Length; i++)
                {
                    userFilter = new TraceFilter(TraceColumnId.SQLSecurityLoginName,
                                                  TraceFilterComparisonOp.NotLike,
                                                  users[i], TraceFilterLogicalOp.AND);
                    trace.AddFilter(userFilter);
                }
            }
        }


        //-----------------------------------------------------------
        // GenerateDBPrivilegedTrace
        //-----------------------------------------------------------
        /// <summary>
        /// Generate server level security trace configuration.
        /// </summary>
        /// <param name="privilegedUsers"></param>
        /// <returns></returns>
        protected virtual TraceConfiguration
           GenerateDBPrivilegedTrace(DBAuditConfiguration config, int sequence)
        {
            TraceConfiguration trace = new TraceConfiguration();

            trace.Level = TraceLevel.Database;
            trace.Sequence = sequence;

            AddDBPrivilegedUserFilters(config, trace);

            trace.InstanceAlias = this.InstanceAlias;
            trace.Category = TraceCategory.DBPrivilegedUsers;
            trace.Version = this.Version;
            trace.FileName = Path.Combine(traceDirectory,
                                          TraceFileNameAttributes.ComposeTraceFileName(trace.InstanceAlias,
                                                                                       (int)trace.Level,
                                                                                       (int)trace.Category,
                                                                                       trace.Version,
                                                                                       sequence));

            AddCommonColumns(trace);

            trace.AddEvent(TraceEventId.Exception);
            trace.AddEvent(TraceEventId.AuditChangeAudit);

            trace.AddColumn(TraceColumnId.EventSubClass);
            trace.AddColumn(TraceColumnId.DatabaseUserName);

            int auditLogins = Array.FindIndex(config.UserCategories, item => item == AuditCategory.Logins);
            if (auditLogins != -1)
            {
                trace.AddEvent(TraceEventId.Login);
                // SQLCM-5375 Release 5.6 - 6.1.4.3 - Capture Logout Events
                // Collect Logout Events only if Login Events is enabled
                int auditLogouts = Array.FindIndex(config.UserCategories, item => item == AuditCategory.Logouts);
                if (auditLogouts != -1)
                {
                    trace.AddEvent(TraceEventId.Logout);
                }
                if (this.sqlVersion >= 9)
                    trace.AddEvent(TraceEventId.AuditDatabasePrincipalImpersonation);
            }

            int auditFailedLogins = Array.FindIndex(config.UserCategories, item => item == AuditCategory.FailedLogins);
            if (auditFailedLogins != -1)
            {
                trace.AddEvent(TraceEventId.LoginFailed);
            }

            int auditUserSecurity = Array.FindIndex(config.UserCategories, item => item == AuditCategory.Security);
            if (auditUserSecurity != -1)
            {
                if (sqlVersion >= 9)
                {
                    trace.AddEvent(TraceEventId.AuditDatabaseObjectTakeOwnership);
                    trace.AddEvent(TraceEventId.AuditDatabasePrincipalManagement);
                    trace.AddEvent(TraceEventId.AuditSchemaObjectTakeOwnership);
                    trace.AddEvent(TraceEventId.AuditDatabaseObjectGDR);
                    trace.AddEvent(TraceEventId.AuditChangeDatabaseOwner);
                }
                else
                {
                    trace.AddEvent(TraceEventId.AuditLoginGDR);
                    trace.AddEvent(TraceEventId.AuditAddLogin);
                    trace.AddEvent(TraceEventId.AuditAddDbUser);
                    trace.AddEvent(TraceEventId.AuditAddDropRole);
                }

                trace.AddEvent(TraceEventId.AuditStatementGDR); // is database scope GDR in 2005
                trace.AddEvent(TraceEventId.AuditObjectGDR);    // is schema object GDR in 2005

                trace.AddEvent(TraceEventId.AuditAddMember);
                trace.AddEvent(TraceEventId.AppRolePassChange);

                trace.AddEvent(TraceEventId.AuditAddLoginToServer);
                trace.AddEvent(TraceEventId.AuditLoginChangePassword);
                trace.AddEvent(TraceEventId.AuditLoginChange);

                trace.AddColumn(TraceColumnId.DatabaseName);
                trace.AddColumn(TraceColumnId.DatabaseID);
                trace.AddColumn(TraceColumnId.DatabaseUserName);
                trace.AddColumn(TraceColumnId.Permissions);
                trace.AddColumn(TraceColumnId.ObjectOwner);
                trace.AddColumn(TraceColumnId.TargetUserName);
                trace.AddColumn(TraceColumnId.TargetLoginName);
                trace.AddColumn(TraceColumnId.TargetRoleName);
                trace.AddColumn(TraceColumnId.EventSubClass);

                // For GDR events
                trace.AddColumn(TraceColumnId.TextData);
            }

            int auditUserAdmin = Array.FindIndex(config.UserCategories, item => item == AuditCategory.Admin);
            if (auditUserAdmin != -1)
            {
                if (sqlVersion >= 9)
                {
                    trace.AddEvent(TraceEventId.AuditDatabaseOperation);
                }
                //trace.AddEvent( TraceEventId.SqlStmtStarting );  // for DBCC commands
                trace.AddEvent(TraceEventId.AuditDbcc);
                trace.AddEvent(TraceEventId.AuditBackupRestore);
                trace.AddEvent(TraceEventId.ServiceControl);

                trace.AddColumn(TraceColumnId.TextData);
                trace.AddColumn(TraceColumnId.DatabaseName);
                trace.AddColumn(TraceColumnId.DatabaseID);
                trace.AddColumn(TraceColumnId.ObjectID);
                trace.AddColumn(TraceColumnId.DatabaseUserName);
                trace.AddColumn(TraceColumnId.TargetUserName);
                trace.AddColumn(TraceColumnId.EventSubClass);
            }

            // Capture Create/Drop Database events
            int auditUserDDL = Array.FindIndex(config.UserCategories, item => item == AuditCategory.DDL);
            if (auditUserDDL != -1)
            {
                if (sqlVersion >= 9)
                {
                    trace.AddEvent(TraceEventId.AuditDatabaseObjectManagement);
                    trace.AddEvent(TraceEventId.AuditSchemaObjectManagement);
                    trace.AddEvent(TraceEventId.AuditDatabaseManagement);

                }
                else
                {
                    trace.AddEvent(TraceEventId.AuditObjectDerivedPermission);
                    // This one contains the statement that creates the object
                    trace.AddEvent(TraceEventId.AuditStatementPermission);
                    // This one provides object name, type and ID
                    trace.AddEvent(TraceEventId.ObjectCreated);
                }

                trace.AddColumn(TraceColumnId.TextData);
                trace.AddColumn(TraceColumnId.Permissions);
                trace.AddColumn(TraceColumnId.DatabaseName);
                trace.AddColumn(TraceColumnId.DatabaseID);
                trace.AddColumn(TraceColumnId.DatabaseUserName);
                trace.AddColumn(TraceColumnId.ObjectOwner);
                trace.AddColumn(TraceColumnId.EventSubClass);
                trace.AddColumn(TraceColumnId.ObjectID);
                trace.AddColumn(TraceColumnId.Error);
            }

            //if (auditConfig.AuditUserEvents)
            {
                trace.AddEvent(TraceEventId.UserEvent0);
                trace.AddEvent(TraceEventId.UserEvent1);
                trace.AddEvent(TraceEventId.UserEvent2);
                trace.AddEvent(TraceEventId.UserEvent3);
                trace.AddEvent(TraceEventId.UserEvent4);
                trace.AddEvent(TraceEventId.UserEvent5);
                trace.AddEvent(TraceEventId.UserEvent6);
                trace.AddEvent(TraceEventId.UserEvent7);
                trace.AddEvent(TraceEventId.UserEvent8);
                trace.AddEvent(TraceEventId.UserEvent9);

                trace.AddColumn(TraceColumnId.EventSubClass);
                trace.AddColumn(TraceColumnId.DatabaseID);
                trace.AddColumn(TraceColumnId.DatabaseName);
                // Note: TextData column is included below  
            }

            // always keep SQL in server level events
            trace.AddColumn(TraceColumnId.TextData);
            if (config.AuditUserCaptureSql)
            {
                trace.KeepSQL = true;
                trace.KeepAdminSQL = true;
            }

            AddDBIdFilters(trace, config.DBIdList);

            if (this.isServer)
            {
                if (config.AuditUserAccessCheck == AccessCheckFilter.FailureOnly)
                    trace.AddFilter(new TraceFilter(TraceColumnId.Success,
                                                     TraceFilterComparisonOp.Equal,
                                                     0,
                                                     TraceFilterLogicalOp.AND));
                else if (config.AuditUserAccessCheck == AccessCheckFilter.SuccessOnly)
                    trace.AddFilter(new TraceFilter(TraceColumnId.Success,
                                                     TraceFilterComparisonOp.Equal,
                                                     1,
                                                     TraceFilterLogicalOp.AND));

            }

            return trace;

        }

        //-----------------------------------------------------------
        // GenerateDBPrivilegedDMLTrace
        //-----------------------------------------------------------
        /// <summary>
        /// Generate trace configuration for DML statement events.
        /// </summary>
        /// <returns></returns>
        protected virtual TraceConfiguration
           GenerateDBPrivilegedDMLTrace(
              DBAuditConfiguration config,
              int sequence,
              List<string> excludeUsers
           )
        {
            // SQLCM-5471 v5.6 Add Activity to Senstitive columns
            //If this is a DB config and both Select and sensitive columns are being audited, just return.
            // the sensitive columns trace will pick up the event.
            if (config.SensitiveColumns != null &&
                config.SensitiveColumns.Length > 0)
            {
                return null;
            }

            TraceConfiguration trace = new TraceConfiguration();
            trace.Level = TraceLevel.Database;
            trace.Sequence = sequence;

            AddCommonColumns(trace);

            if (config.AuditUserCaptureTransactions)
                trace.AddEvent(TraceEventId.Transaction);

            trace.AddEvent(TraceEventId.Exception);

            trace.AddEvent(TraceEventId.AuditObjectPermission);
            if (sqlVersion >= 9)
                trace.AddEvent(TraceEventId.AuditDatabaseObjectAccess);

            trace.AddColumn(TraceColumnId.Permissions);
            trace.AddColumn(TraceColumnId.EventSubClass);
            trace.AddColumn(TraceColumnId.DatabaseName);
            trace.AddColumn(TraceColumnId.DatabaseID);
            trace.AddColumn(TraceColumnId.Error);
            if (EnableRowCountForDatabaseAuditing)
            {
                trace.AddColumn(TraceColumnId.RowCounts);
            }

            int auditDML = Array.FindIndex(config.UserCategories, item => item == AuditCategory.DML);
            if (auditDML != -1 && isServer)
            {

                trace.AddFilter(new TraceFilter(TraceColumnId.Permissions,
                                                 TraceFilterComparisonOp.Equal,
                                                 2,         // Update
                                                 TraceFilterLogicalOp.OR));
                trace.AddFilter(new TraceFilter(TraceColumnId.Permissions,
                                                 TraceFilterComparisonOp.Equal,
                                                 8,         // INSERT
                                                 TraceFilterLogicalOp.OR));
                trace.AddFilter(new TraceFilter(TraceColumnId.Permissions,
                                                 TraceFilterComparisonOp.Equal,
                                                 16,         // DELETE
                                                 TraceFilterLogicalOp.OR));
                trace.AddFilter(new TraceFilter(TraceColumnId.Permissions,
                                                 TraceFilterComparisonOp.Equal,
                                                 32,         // EXECUTE
                                                 TraceFilterLogicalOp.OR));
            }
            if (this.sqlVersion > 9)
            {
                if (EnableRowCountForDatabaseAuditing)
                {
                    trace.AddEvent(TraceEventId.SqlStmtCompleted);  // Changed From SqlStmtStarting to SqlStmtCompleted for Row Counts
                    trace.AddEvent(TraceEventId.SpStmtCompleted);
                }

                if (!config.AuditCaptureDetails && !this.isServer)
                    trace.AddColumn(TraceColumnId.TextData);
            }  

            if (config.AuditUserCaptureSql)
            {
				//To support previous version of agent
                if (this.isServer || this.sqlVersion < 10)
                {
                    trace.AddEvent(TraceEventId.SqlStmtStarting);
                }              
                trace.AddColumn(TraceColumnId.TextData);
                trace.KeepSQL = true;
            }
            else
            {
                trace.AddEvent(TraceEventId.SpStarting); // for stored procedures
            }

            AddDataChangeEventFilter(trace);
            AddObjectTypeFilters(config, trace);

            if (this.isServer)
            {
                if (config.AuditUserAccessCheck == AccessCheckFilter.FailureOnly)
                    trace.AddFilter(new TraceFilter(TraceColumnId.Success,
                                                     TraceFilterComparisonOp.Equal,
                                                     0,
                                                     TraceFilterLogicalOp.AND));
                else if (config.AuditUserAccessCheck == AccessCheckFilter.SuccessOnly)
                    trace.AddFilter(new TraceFilter(TraceColumnId.Success,
                                                     TraceFilterComparisonOp.Equal,
                                                     1,
                                                     TraceFilterLogicalOp.AND));

            }


            // Filter out trusted users and SQLsecure activities

            trace.InstanceAlias = this.InstanceAlias;
            trace.Category = TraceCategory.DMLwithDetails;
            trace.Version = this.Version;
            trace.FileName = Path.Combine(traceDirectory,
                                          TraceFileNameAttributes.ComposeTraceFileName(trace.InstanceAlias,
                                                                                 (int)trace.Level,
                                                                                 (int)trace.Category,
                                                                                 trace.Version,
                                                                                 sequence));

            return trace;

        }

        //-----------------------------------------------------------
        // GenerateDBPrivilegedSELECTTrace
        //-----------------------------------------------------------
        /// <summary>
        /// Generate trace configuration for SELECT events.
        /// </summary>
        /// <returns></returns>
        protected virtual TraceConfiguration
           GenerateDBPrivilegedSELECTTrace(
              DBAuditConfiguration config,
              int sequence
           )
        {
            //If this is a DB config and both Select and sensitive columns are being audited, just return.
            // the sensitive columns trace will pick up the event.
            if (config.SensitiveColumns != null &&
                config.SensitiveColumns.Length > 0)
            {
                return null;
            }
            TraceConfiguration trace = new TraceConfiguration();
            trace.Level = TraceLevel.Database;
            trace.Sequence = sequence;


            AddCommonColumns(trace);

            trace.AddEvent(TraceEventId.Exception);

            trace.AddEvent(TraceEventId.AuditObjectPermission);
            if (sqlVersion >= 9)
                trace.AddEvent(TraceEventId.AuditDatabaseObjectAccess);

            trace.AddColumn(TraceColumnId.Permissions);
            trace.AddColumn(TraceColumnId.EventSubClass);
            trace.AddColumn(TraceColumnId.DatabaseName);
            trace.AddColumn(TraceColumnId.DatabaseID);
            trace.AddColumn(TraceColumnId.Error);
            if (EnableRowCountForDatabaseAuditing)
            {
                trace.AddColumn(TraceColumnId.RowCounts);
            }

            int auditSELECT = Array.FindIndex(config.UserCategories, item => item == AuditCategory.SELECT);
            if (auditSELECT != -1 && isServer)
            {
                trace.AddFilter(new TraceFilter(TraceColumnId.Permissions,
                                                 TraceFilterComparisonOp.Equal,
                                                 1,         // SELECT
                                                 TraceFilterLogicalOp.OR));

                trace.AddFilter(new TraceFilter(TraceColumnId.Permissions,
                                                 TraceFilterComparisonOp.Equal,
                                                 4,         // REFERENCE ALL
                                                 TraceFilterLogicalOp.OR));
            }
            
            if (this.sqlVersion > 9)
            {
                if (EnableRowCountForDatabaseAuditing)
                {
                    trace.AddEvent(TraceEventId.SqlStmtCompleted);  // Changed From SqlStmtStarting to SqlStmtCompleted for Row Counts
                    trace.AddEvent(TraceEventId.SpStmtCompleted);
                }

                if (!config.AuditCaptureDetails && !this.isServer)
                    trace.AddColumn(TraceColumnId.TextData);
            }
            if (config.AuditUserCaptureSql)
            {
				//To support previous version of agent
                if (this.isServer || this.sqlVersion < 10)
                {
                    trace.AddEvent(TraceEventId.SqlStmtStarting);
                }
                trace.AddColumn(TraceColumnId.TextData);
                trace.KeepSQL = true;
            }
            else
            {
                trace.KeepSQL = false;
            }
            if (Array.FindIndex(config.UserCategories, item => item == AuditCategory.DML) == -1)
            {
                AddDataChangeEventFilter(trace);
                AddObjectTypeFilters(config, trace);
            }

            if (this.isServer)
            {
                if (config.AuditUserAccessCheck == AccessCheckFilter.FailureOnly)
                    trace.AddFilter(new TraceFilter(TraceColumnId.Success,
                                                     TraceFilterComparisonOp.Equal,
                                                     0,
                                                     TraceFilterLogicalOp.AND));
                else if (config.AuditUserAccessCheck == AccessCheckFilter.SuccessOnly)
                    trace.AddFilter(new TraceFilter(TraceColumnId.Success,
                                                     TraceFilterComparisonOp.Equal,
                                                     1,
                                                     TraceFilterLogicalOp.AND));

            }

            trace.InstanceAlias = this.InstanceAlias;
            trace.Category = TraceCategory.SELECTwithDetails;
            trace.Version = this.Version;
            trace.FileName = Path.Combine(traceDirectory,
                                 TraceFileNameAttributes.ComposeTraceFileName(trace.InstanceAlias,
                                                                              (int)trace.Level,
                                                                              (int)trace.Category,
                                                                              trace.Version,
                                                                              sequence));
            return trace;

        }

        #endregion

        #region Server Only Trace
        //-----------------------------------------------------------
        // GenerateServerOnlyTrace
        //-----------------------------------------------------------
        /// <summary>
        /// Generate server level security trace configuration.
        /// </summary>
        /// <param name="privilegedUsers"></param>
        /// <returns></returns>
        protected virtual TraceConfiguration
           GenerateServerOnlyTrace(
              bool privilegedUsers
           )
        {
            AuditConfiguration auditConfig = this;

            TraceConfiguration trace = new TraceConfiguration();
            //v5.6 SQLCM-5373
            if (!privilegedUsers && auditConfig.AuditedServerTrustedUsers.Length > 0)
            {
                AddTrustedUserServerLevelFilters(trace, auditConfig.AuditedServerTrustedUsers);
            }

            if (privilegedUsers)
            {
                trace.Level = TraceLevel.User;
                auditConfig = this.UserConfiguration;
                AddUserFilters(trace);
                AddDataChangeEventFilter(trace);
            }
            else
                trace.Level = TraceLevel.Server;
            trace.Sequence = 0;

            trace.InstanceAlias = this.InstanceAlias;
            trace.Category = TraceCategory.ServerTrace;
            trace.Version = this.Version;
            trace.FileName = Path.Combine(traceDirectory,
                                          TraceFileNameAttributes.ComposeTraceFileName(trace.InstanceAlias,
                                                                                       (int)trace.Level,
                                                                                       (int)trace.Category,
                                                                                       trace.Version,
                                                                                       0));

            AddCommonColumns(trace);

            //if ( auditConfig.AuditExceptions )
            //{
            //   trace.AddEvent( TraceEventId.Exception );
            //   trace.AddEvent( TraceEventId.SqlStmtCompleted );
            //}

            trace.AddEvent(TraceEventId.AuditChangeAudit);

            trace.AddColumn(TraceColumnId.EventSubClass);
            trace.AddColumn(TraceColumnId.DatabaseUserName);

            if (auditConfig.AuditLogins)
            {
                trace.AddEvent(TraceEventId.Login);
                // SQLCM-5375 Release 5.6 - 6.1.4.3 - Capture Logout Events
                if (auditConfig.AuditLogouts)
                {
                    trace.AddEvent(TraceEventId.Logout);
                }
                if (this.sqlVersion >= 9)
                    trace.AddEvent(TraceEventId.AuditServerPrincipalImpersonation);
            }

            if (auditConfig.AuditFailedLogins)
            {
                trace.AddEvent(TraceEventId.LoginFailed);
            }

            if (auditConfig.AuditSecurity)
            {
                if (sqlVersion >= 9)
                {
                    trace.AddEvent(TraceEventId.AuditServerObjectTakeOwnership);
                    trace.AddEvent(TraceEventId.AuditServerPrincipalManagement);
                    trace.AddEvent(TraceEventId.AuditServerObjectGDR);
                    trace.AddEvent(TraceEventId.AuditServerScopeGDR);
                    trace.AddEvent(TraceEventId.AuditChangeDatabaseOwner);
                }
                else
                {
                    trace.AddEvent(TraceEventId.AuditLoginGDR);
                    trace.AddEvent(TraceEventId.AuditAddLogin);
                }

                trace.AddEvent(TraceEventId.AuditAddLoginToServer);
                trace.AddEvent(TraceEventId.AuditLoginChangePassword);
                trace.AddEvent(TraceEventId.AuditLoginChange);

                trace.AddColumn(TraceColumnId.TargetLoginName);
                trace.AddColumn(TraceColumnId.ObjectID);
                trace.AddColumn(TraceColumnId.TargetRoleName);
                trace.AddColumn(TraceColumnId.EventSubClass);
            }

            if (auditConfig.AuditAdmin)
            {
                if (sqlVersion >= 9)
                {
                    trace.AddEvent(TraceEventId.AuditServerOperation);
                    trace.AddEvent(TraceEventId.AuditServerAlterTrace);
                }
                trace.AddEvent(TraceEventId.AuditDbcc);
                trace.AddColumn(TraceColumnId.DatabaseID);
                trace.AddColumn(TraceColumnId.DatabaseName);
                trace.AddColumn(TraceColumnId.DatabaseUserName);
                trace.AddColumn(TraceColumnId.ObjectOwner);
                trace.AddColumn(TraceColumnId.DatabaseUserName);
                trace.AddColumn(TraceColumnId.TargetUserName);

            }


            // Capture Create/Drop Database events
            if (auditConfig.AuditDDL)
            {
                trace.AddColumn(TraceColumnId.DatabaseID);
                trace.AddColumn(TraceColumnId.DatabaseName);
                trace.AddColumn(TraceColumnId.Permissions);
                trace.AddColumn(TraceColumnId.ObjectOwner);
                trace.AddColumn(TraceColumnId.DatabaseUserName);
                trace.AddColumn(TraceColumnId.TargetUserName);

                if (sqlVersion >= 9) // SQL 2005
                {
                    // Create, Drop and Alter databases
                    trace.AddEvent(TraceEventId.AuditDatabaseManagement);
                    trace.AddEvent(TraceEventId.AuditServerObjectManagement);

                    trace.AddColumn(TraceColumnId.ObjectName);
                    trace.AddColumn(TraceColumnId.ObjectType);
                    trace.AddColumn(TraceColumnId.ObjectID);
                }
                else // SQL 2000
                {
                    // For create database events
                    trace.AddEvent(TraceEventId.AuditStatementPermission);
                    trace.AddEvent(TraceEventId.ObjectCreated);

                    // For drop database events
                    trace.AddEvent(TraceEventId.AuditObjectDerivedPermission);
                    trace.AddEvent(TraceEventId.ObjectDeleted);
                }

            }

            if (auditConfig.AuditUserEvents)
            {
                trace.AddEvent(TraceEventId.UserEvent0);
                trace.AddEvent(TraceEventId.UserEvent1);
                trace.AddEvent(TraceEventId.UserEvent2);
                trace.AddEvent(TraceEventId.UserEvent3);
                trace.AddEvent(TraceEventId.UserEvent4);
                trace.AddEvent(TraceEventId.UserEvent5);
                trace.AddEvent(TraceEventId.UserEvent6);
                trace.AddEvent(TraceEventId.UserEvent7);
                trace.AddEvent(TraceEventId.UserEvent8);
                trace.AddEvent(TraceEventId.UserEvent9);

                trace.AddColumn(TraceColumnId.EventSubClass);
                trace.AddColumn(TraceColumnId.DatabaseID);
                trace.AddColumn(TraceColumnId.DatabaseName);
                // Note: TextData column is included below  
            }

            /*

            if( sqlVersion >= 9 &&
                !auditConfig.AuditSystemEvents )
                  trace.AddFilter( new TraceFilter( TraceColumnId.IsSystem, 
                                                 TraceFilterComparisonOp.NotEqual,
                                                 1,
                                                 TraceFilterLogicalOp.AND ) );
                                                 */

            // always keep SQL in server level events
            trace.AddColumn(TraceColumnId.TextData);
            if ((trace.Level == TraceLevel.Server) || auditConfig.AuditCaptureDetails)
                trace.KeepSQL = true;

            // Filter out SQLsecure activities
            AddSQLsecureApplicationFilters(trace);

            if (this.isServer)
            {
                if (auditConfig.AuditAccessCheck == AccessCheckFilter.FailureOnly)
                    trace.AddFilter(new TraceFilter(TraceColumnId.Success,
                                                     TraceFilterComparisonOp.Equal,
                                                     0,
                                                     TraceFilterLogicalOp.AND));
                else if (auditConfig.AuditAccessCheck == AccessCheckFilter.SuccessOnly)
                    trace.AddFilter(new TraceFilter(TraceColumnId.Success,
                                                     TraceFilterComparisonOp.Equal,
                                                     1,
                                                     TraceFilterLogicalOp.AND));

            }

            return trace;

        }

        #endregion

        #region Database Security Trace
        //-----------------------------------------------------------
        // GenerateDBSecurityTraces
        //-----------------------------------------------------------
        /// <summary>
        /// Generate database level security traces.
        /// </summary>
        /// <returns></returns>
        protected virtual TraceConfiguration[]
           GenerateDBSecurityTraces(
              bool privilegedUsers
           )
        {
            ArrayList traceSets = CreateConfigurationSets(AuditDBList, TraceCategory.DBSecurity);
            if (traceSets == null)
                return null;

            int sequence = 0;

            ArrayList traces = new ArrayList();
            TraceConfiguration trace;
            foreach (AuditConfiguration config in traceSets)
            {
                trace = GenerateDBSecurityTrace(config, privilegedUsers, sequence);
                if (trace != null)
                {
                    sequence++;
                    traces.Add(trace);
                }
            }

            return (TraceConfiguration[])traces.ToArray(typeof(TraceConfiguration));
        }

        //-----------------------------------------------------------
        // GenerateDBSecurityTrace
        //-----------------------------------------------------------
        /// <summary>
        /// Generate database level security trace configuration.
        /// </summary>
        /// <returns></returns>
        protected virtual TraceConfiguration
           GenerateDBSecurityTrace(
              AuditConfiguration config,
              bool privilegedUsers,
              int sequence
           )
        {
            if (!config.AuditSecurity &&
                !config.AuditDDL &&
                !config.AuditAdmin)
                return null;

            TraceConfiguration trace = new TraceConfiguration();
            if (privilegedUsers)
            {
                trace.Level = TraceLevel.User;
            }
            else
                trace.Level = TraceLevel.Database;
            trace.Sequence = sequence;

            AddCommonColumns(trace);

            //if ( config.AuditExceptions )
            //{
            trace.AddEvent(TraceEventId.Exception);
            //   trace.AddEvent( TraceEventId.SqlStmtCompleted );
            //}

            if (config.AuditSecurity)
            {
                if (sqlVersion >= 9)
                {
                    trace.AddEvent(TraceEventId.AuditDatabaseObjectTakeOwnership);
                    trace.AddEvent(TraceEventId.AuditDatabasePrincipalManagement);
                    trace.AddEvent(TraceEventId.AuditSchemaObjectTakeOwnership);
                    trace.AddEvent(TraceEventId.AuditDatabaseObjectGDR);
                }
                else
                {
                    trace.AddEvent(TraceEventId.AuditAddDbUser);
                    trace.AddEvent(TraceEventId.AuditAddDropRole);
                }

                trace.AddEvent(TraceEventId.AuditStatementGDR); // is database scope GDR in 2005
                trace.AddEvent(TraceEventId.AuditObjectGDR);    // is schema object GDR in 2005

                trace.AddEvent(TraceEventId.AuditAddMember);
                trace.AddEvent(TraceEventId.AppRolePassChange);

                trace.AddColumn(TraceColumnId.DatabaseName);
                trace.AddColumn(TraceColumnId.DatabaseID);
                trace.AddColumn(TraceColumnId.DatabaseUserName);
                trace.AddColumn(TraceColumnId.Permissions);
                trace.AddColumn(TraceColumnId.ObjectOwner);
                trace.AddColumn(TraceColumnId.TargetUserName);
                trace.AddColumn(TraceColumnId.TargetLoginName);
                trace.AddColumn(TraceColumnId.TargetRoleName);
                trace.AddColumn(TraceColumnId.EventSubClass);

                // For GDR events
                trace.AddColumn(TraceColumnId.TextData);

            }

            if (config.AuditDDL)
            {
                if (sqlVersion >= 9)
                {
                    trace.AddEvent(TraceEventId.AuditDatabaseObjectManagement);
                    trace.AddEvent(TraceEventId.AuditSchemaObjectManagement);
                    trace.AddEvent(TraceEventId.AuditDatabaseManagement);

                }
                else
                {
                    trace.AddEvent(TraceEventId.AuditObjectDerivedPermission);

                    // This one contains the statement that creates the object
                    trace.AddEvent(TraceEventId.AuditStatementPermission);
                    // This one provides object name, type and ID
                    trace.AddEvent(TraceEventId.ObjectCreated);
                }

                trace.AddColumn(TraceColumnId.TextData);
                trace.AddColumn(TraceColumnId.Permissions);
                trace.AddColumn(TraceColumnId.DatabaseName);
                trace.AddColumn(TraceColumnId.DatabaseID);
                trace.AddColumn(TraceColumnId.DatabaseUserName);
                trace.AddColumn(TraceColumnId.ObjectOwner);
                trace.AddColumn(TraceColumnId.EventSubClass);
                trace.AddColumn(TraceColumnId.ObjectID);
                trace.AddColumn(TraceColumnId.Error);
            }

            if (config.AuditAdmin)
            {
                if (sqlVersion >= 9)
                {
                    trace.AddEvent(TraceEventId.AuditDatabaseOperation);
                }
                //trace.AddEvent( TraceEventId.SqlStmtStarting );  // for DBCC commands
                trace.AddEvent(TraceEventId.AuditDbcc);
                trace.AddEvent(TraceEventId.AuditBackupRestore);
                trace.AddEvent(TraceEventId.ServiceControl);

                trace.AddColumn(TraceColumnId.TextData);
                trace.AddColumn(TraceColumnId.DatabaseName);
                trace.AddColumn(TraceColumnId.DatabaseID);
                trace.AddColumn(TraceColumnId.ObjectID);
                trace.AddColumn(TraceColumnId.DatabaseUserName);
                trace.AddColumn(TraceColumnId.TargetUserName);
                trace.AddColumn(TraceColumnId.EventSubClass);
            }

            if (sqlVersion >= 9)
            {
                /* Disable for now
                if( config.AuditLogins )
                {
                   trace.AddEvent( TraceEventId.AuditDatabasePrincipalImpersonation );

                   trace.AddColumn( TraceColumnId.TextData );
                   trace.AddColumn( TraceColumnId.DatabaseName );
                   trace.AddColumn( TraceColumnId.DatabaseID );
                   trace.AddColumn( TraceColumnId.DatabaseUserName );
                   trace.AddColumn( TraceColumnId.ObjectName );
                   trace.AddColumn( TraceColumnId.ObjectType );
                   trace.AddColumn( TraceColumnId.SQLSecurityLoginName );  // impersonate as this login
                   trace.AddColumn( TraceColumnId.NTUserName );
                }*/


                /* Removed from 2.0 features
                if( config.AuditBroker )
                {
                   trace.AddEvent( TraceEventId.AuditBrokerConversation );
                   trace.AddEvent( TraceEventId.AuditBrokerLogin );
               
                   trace.AddColumn( TraceColumnId.ClientHostName );
                   trace.AddColumn( TraceColumnId.EventSubClass );
                   trace.AddColumn( TraceColumnId.TextData );
                   trace.AddColumn( TraceColumnId.TargetRoleName ); // target or initiator
                   trace.AddColumn( TraceColumnId.ObjectID );  // the target service's id.

                   // for the login event
                   trace.AddColumn( TraceColumnId.ObjectName );  // connection string
                   trace.AddColumn( TraceColumnId.ProviderName ); // authentication method
                   trace.AddColumn( TraceColumnId.TargetLoginName ); // login state

                }

                if( !config.AuditSystemEvents &&
                    !privilegedUsers )
                   trace.AddFilter( new TraceFilter( TraceColumnId.IsSystem, 
                                                  TraceFilterComparisonOp.NotEqual,
                                                  1,
                                                  TraceFilterLogicalOp.AND ) );
                */
            }

            // always keep SQL in security traces
            trace.AddColumn(TraceColumnId.TextData);
            if ((trace.Level == TraceLevel.Database) || config.AuditCaptureDetails)
                trace.KeepSQL = true;

            if ( /*this.isServer && */ config is DBAuditConfiguration)
            {
                // dbid filtering done on server side only
                AddDBIdFilters(trace, ((DBAuditConfiguration)config).DBIdList);
            }

            if (!isServer && config is DBAuditConfiguration && config.AuditedUsers.Length > 0)
            {
                AddTrustedUserFilters(trace, config.AuditedUsers);
            }


            // Filter out SQLsecure activities
            if (!privilegedUsers)
                AddSQLsecureApplicationFilters(trace);

            if (this.isServer)
            {
                if (config.AuditAccessCheck == AccessCheckFilter.FailureOnly)
                    trace.AddFilter(new TraceFilter(TraceColumnId.Success,
                                                     TraceFilterComparisonOp.Equal,
                                                     0,
                                                     TraceFilterLogicalOp.AND));
                else if (config.AuditAccessCheck == AccessCheckFilter.SuccessOnly)
                    trace.AddFilter(new TraceFilter(TraceColumnId.Success,
                                                     TraceFilterComparisonOp.Equal,
                                                     1,
                                                     TraceFilterLogicalOp.AND));

            }

            trace.InstanceAlias = this.InstanceAlias;
            trace.Category = TraceCategory.DBSecurity;
            trace.Version = this.Version;
            trace.FileName = Path.Combine(traceDirectory,
                                           TraceFileNameAttributes.ComposeTraceFileName(trace.InstanceAlias,
                                                                                         (int)trace.Level,
                                                                                         (int)trace.Category,
                                                                                         trace.Version,
                                                                                         sequence));


            return trace;
        }

        #endregion

        #region DML and SELECT traces
        //-----------------------------------------------------------
        // GenerateDMLTraces
        //-----------------------------------------------------------
        /// <summary>
        /// Generate database level DML traces.
        /// </summary>
        /// <returns></returns>
        protected virtual TraceConfiguration[]
           GenerateDMLTraces(
              bool privilegedUsers
           )
        {
            if (isAuditLogsEnabled && !isServer)
                return null;
            ArrayList traces = new ArrayList();
            int sequence = 0;
            int dcSequence = 0;

            ArrayList traceSets = CreateConfigurationSets(AuditDBList, TraceCategory.DML);
            if (traceSets == null)
                return null;

            TraceConfiguration trace;
            List<string> pUsers = privilegedUsers ? null : ConfigurationHelper.GetPrivUsers(this);
            foreach (AuditConfiguration config in traceSets)
            {
                if (config == null)
                    continue;
                trace = GenerateDMLTrace(config, privilegedUsers, sequence, pUsers);
                if (trace != null)
                {
                    sequence++;
                    traces.Add(trace);
                }

                if (!privilegedUsers && !this.AuditCaptureSQLXE)
                {
                    if (pUsers == null)
                        pUsers = ConfigurationHelper.GetPrivUsers(this);
                    DBAuditConfiguration dbConfig = (DBAuditConfiguration)config;
                    if (dbConfig.DataChangeTables != null &&
                         dbConfig.DataChangeTables.Length > 0)
                    {
                        //Single file is used for DML and Data change in case of Audit Log auditing
                        if (isAuditLogsEnabled)
                        {
                            if (trace.Category == TraceCategory.DMLwithDetails)
                                trace.Category = TraceCategory.DataChangeWithDetails;
                            else
                                trace.Category = TraceCategory.DataChange;
                            continue;
                        }
                        dcSequence++;
                        trace = GenerateDataChangeTrace(dbConfig, dcSequence, pUsers);
                        traces.Add(trace);
                    }
                }
            }

            return (TraceConfiguration[])traces.ToArray(typeof(TraceConfiguration));
        }


        //5.4 XE
        protected virtual XeTraceConfiguration[]
         GenerateDMLTracesXE(
            bool privilegedUsers
         )
        {
            ArrayList traces = new ArrayList();
            int sequence = 0;
            int dcSequence = 0;
            if (!this.AuditCaptureSQLXE)
                return null;
            ArrayList traceSets = CreateConfigurationSets(AuditDBList, TraceCategory.DML);
            if (traceSets == null)
                return null;

            XeTraceConfiguration trace;
            List<string> pUsers = privilegedUsers ? null : ConfigurationHelper.GetPrivUsers(this);
            foreach (AuditConfiguration config in traceSets)
            {
                if (config == null)
                    continue;
                trace = GenerateDMLTraceXE(config, privilegedUsers, sequence, pUsers);
                if (trace != null)
                {
                    sequence++;
                    traces.Add(trace);
                }
                if (!privilegedUsers)
                {
                    if (pUsers == null)
                        pUsers = ConfigurationHelper.GetPrivUsers(this);
                    DBAuditConfiguration dbConfig = (DBAuditConfiguration)config;
                    if (dbConfig.DataChangeTables != null &&
                         dbConfig.DataChangeTables.Length > 0)
                    {
                        dcSequence++;
                        trace = GenerateDataChangeTraceXE(dbConfig, dcSequence, pUsers);
                        traces.Add(trace);
                    }
                }
            }

            return (XeTraceConfiguration[])traces.ToArray(typeof(XeTraceConfiguration));
        }

        //-----------------------------------------------------------
        // GenerateDMLTrace
        //-----------------------------------------------------------
        /// <summary>
        /// Generate trace configuration for DML statement events.
        /// </summary>
        /// <returns></returns>
        protected virtual TraceConfiguration
           GenerateDMLTrace(
              AuditConfiguration config,
              bool privilegedUsers,
              int sequence,
              List<string> excludeUsers
           )
        {
            /* Before 3.1 we run DMLwithSELECT traces without capturing the TextColumns.
             * Since 3.1 adds extra filters for SELECT statements only, we separate each DMLwithSELECT
             * trace into a DML and a SELECT trace
            if( (!config.AuditDML && !config.AuditSELECT) ||
                (!config.AuditDML && config.AuditSELECT && config.AuditCaptureDetails) ) 
               return null;
            */
            if (!config.AuditDML || (this.AuditCaptureSQLXE && !privilegedUsers))
                return null;



            TraceConfiguration trace = new TraceConfiguration();
            if (privilegedUsers)
            {
                trace.Level = TraceLevel.User;
            }
            else
                trace.Level = TraceLevel.Database;
            trace.Sequence = sequence;

            AddCommonColumns(trace);

            if (config.AuditCaptureTransactions)
                trace.AddEvent(TraceEventId.Transaction);

            //if ( config.AuditExceptions )
            //{
            trace.AddEvent(TraceEventId.Exception);
            //   trace.AddEvent( TraceEventId.SqlStmtCompleted );
            //}

            trace.AddEvent(TraceEventId.AuditObjectPermission);
            if (sqlVersion >= 9)
                trace.AddEvent(TraceEventId.AuditDatabaseObjectAccess);

            trace.AddColumn(TraceColumnId.Permissions);
            trace.AddColumn(TraceColumnId.EventSubClass);
            trace.AddColumn(TraceColumnId.DatabaseName);
            trace.AddColumn(TraceColumnId.DatabaseID);
            trace.AddColumn(TraceColumnId.Error);
            if (EnableRowCountForDatabaseAuditing)
            {
                trace.AddColumn(TraceColumnId.RowCounts); // Added to get Row Counts
            }

            if (config.AuditDML &&
                (!privilegedUsers || isServer))
            {

                trace.AddFilter(new TraceFilter(TraceColumnId.Permissions,
                                                 TraceFilterComparisonOp.Equal,
                                                 2,         // Update
                                                 TraceFilterLogicalOp.OR));
                trace.AddFilter(new TraceFilter(TraceColumnId.Permissions,
                                                 TraceFilterComparisonOp.Equal,
                                                 8,         // INSERT
                                                 TraceFilterLogicalOp.OR));
                trace.AddFilter(new TraceFilter(TraceColumnId.Permissions,
                                                 TraceFilterComparisonOp.Equal,
                                                 16,         // DELETE
                                                 TraceFilterLogicalOp.OR));
                trace.AddFilter(new TraceFilter(TraceColumnId.Permissions,
                                                 TraceFilterComparisonOp.Equal,
                                                 32,         // EXECUTE
                                                 TraceFilterLogicalOp.OR));
            }
            if (this.sqlVersion > 9)
            {   
                if (EnableRowCountForDatabaseAuditing)
                {
                    trace.AddEvent(TraceEventId.SqlStmtCompleted);  // Changed From SqlStmtStarting to SqlStmtCompleted for Row Counts
                    trace.AddEvent(TraceEventId.SpStmtCompleted);
                }

                if (config.AuditCaptureDetails && !this.isServer)
                    trace.AddColumn(TraceColumnId.TextData);
                if (!privilegedUsers && !isServer)
                {
                    trace.AddFilter(new TraceFilter(TraceColumnId.TextData,
                                                     TraceFilterComparisonOp.NotLike,
                                                     "S%",         
                                                     TraceFilterLogicalOp.AND));
                    trace.AddFilter(new TraceFilter(TraceColumnId.TextData,
                                                     TraceFilterComparisonOp.NotLike,
                                                     "s%",         
                                                     TraceFilterLogicalOp.AND));
                    trace.AddFilter(new TraceFilter(TraceColumnId.TextData,
                                                     TraceFilterComparisonOp.NotLike,
                                                     "R%",         
                                                     TraceFilterLogicalOp.AND));
                    trace.AddFilter(new TraceFilter(TraceColumnId.TextData,
                                                     TraceFilterComparisonOp.NotLike,
                                                     "r%",         
                                                     TraceFilterLogicalOp.AND));
                }
            }
            if (config.AuditCaptureDetails)
            {
				//To support previous version of agent
                if (this.isServer || this.sqlVersion < 10)
                {
                    trace.AddEvent(TraceEventId.SqlStmtStarting);
                }
                trace.Category = TraceCategory.DMLwithDetails;
                trace.AddColumn(TraceColumnId.TextData);
                trace.KeepSQL = true;
            }
            else
            {
				//To support previous version of agent
                if (this.isServer || this.sqlVersion < 10)
                {
                    trace.AddEvent(TraceEventId.SpStarting); // for stored procedures
                }
                /* Remove SELECT sicne 3.1
                if( config.AuditSELECT )
                {
                   // AuditSELECT is splitted into another trace when CaptureDetails is true
                   trace.AddEvent( TraceEventId.AuditObjectPermission );
                   if( sqlVersion >= 9 )
                      trace.AddEvent( TraceEventId.AuditDatabaseObjectAccess );
                   trace.AddColumn( TraceColumnId.Permissions );

                   if( !privilegedUsers || isServer )
                   {
                      trace.AddFilter( new TraceFilter( TraceColumnId.Permissions,
                                                       TraceFilterComparisonOp.Equal,
                                                       1,         // SELECT
                                                       TraceFilterLogicalOp.OR ) );

                      trace.AddFilter( new TraceFilter( TraceColumnId.Permissions,
                                                       TraceFilterComparisonOp.Equal,
                                                       4,         // REFERENCE ALL
                                                       TraceFilterLogicalOp.OR ) );
                   }

                   trace.Category = TraceCategory.DMLwithSELECT;

                }
                else
                {
                   trace.Category = TraceCategory.DML;
                }
                */
                trace.Category = TraceCategory.DML;
            }

            // Add Database ID filters if any
            if (config is DBAuditConfiguration)
            {
                AddObjectTypeFilters((DBAuditConfiguration)config, trace);

                // CM-5.4 version SQLCM-3638 
                //if( !config.AuditCaptureDetails || isServer)
                //AddDBIdFilters( trace, ((DBAuditConfiguration)config).DBIdList );
                // CM-5.4 version SQLCM-3638 
                if (isServer)
                    AddObjectFilters(trace, config.AuditObjectList);

                // CM-5.4 version SQLCM-3638 Add dbId filter for all the DML events
                AddDBIdFilters(trace, ((DBAuditConfiguration)config).DBIdList);
                // CM-5.4 version SQLCM-3638 
            }

            if (this.isServer)
            {
                if (config.AuditAccessCheck == AccessCheckFilter.FailureOnly)
                    trace.AddFilter(new TraceFilter(TraceColumnId.Success,
                                                     TraceFilterComparisonOp.Equal,
                                                     0,
                                                     TraceFilterLogicalOp.AND));
                else if (config.AuditAccessCheck == AccessCheckFilter.SuccessOnly)
                    trace.AddFilter(new TraceFilter(TraceColumnId.Success,
                                                     TraceFilterComparisonOp.Equal,
                                                     1,
                                                     TraceFilterLogicalOp.AND));

            }

            // Filter out trusted users and SQLsecure activities
            if (!privilegedUsers)
            {
                if (!isServer && config is DBAuditConfiguration && config.AuditedUsers.Length > 0)
                {
                    AddTrustedUserFiltersEx(trace, config.AuditedUsers, excludeUsers);
                }
            }

            AddSQLsecureApplicationFilters(trace);


            trace.InstanceAlias = this.InstanceAlias;
            trace.Version = this.Version;
            trace.FileName = Path.Combine(traceDirectory,
                                          TraceFileNameAttributes.ComposeTraceFileName(trace.InstanceAlias,
                                                                                 (int)trace.Level,
                                                                                 (int)trace.Category,
                                                                                 trace.Version,
                                                                                 sequence));

            return trace;

        }

        //5.4 XE

        protected virtual XeTraceConfiguration
          GenerateDMLandSelectTraceXE(
             AuditConfiguration config,
             bool privilegedUsers,
             int sequence,
             List<string> excludeUsers
          )
        {
            /* Before 3.1 we run DMLwithSELECT traces without capturing the TextColumns.
             * Since 3.1 adds extra filters for SELECT statements only, we separate each DMLwithSELECT
             * trace into a DML and a SELECT trace
            if( (!config.AuditDML && !config.AuditSELECT) ||
                (!config.AuditDML && config.AuditSELECT && config.AuditCaptureDetails) ) 
               return null;
            */
            if (!privilegedUsers)
                return null;

            XeTraceConfiguration trace = new XeTraceConfiguration();
            if (privilegedUsers)
            {
                trace.Level = TraceLevel.User;
                AddUserFiltersXE(trace);
            }
            else
                trace.Level = TraceLevel.Database;
            trace.Sequence = sequence;

            //Need to add XEvent Column
            //AddCommonColumns(trace);


            //To support previous version of agent
            if (this.isServer)
            {
                trace.AddEvent(TraceEventIdXE.SqlStmtStarting);
                trace.AddEvent(TraceEventIdXE.SpStarting);
            }
            trace.AddEvent(TraceEventIdXE.SqlStmtCompleted);
            trace.AddEvent(TraceEventIdXE.SpCompleted);
            trace.Category = TraceCategory.ServerTrace;
            trace.AddColumn(TraceColumnIdXE.TextData);
            trace.AddColumn(TraceColumnIdXE.ApplicationName);
            trace.AddColumn(TraceColumnIdXE.ClientHostName);
            trace.AddColumn(TraceColumnIdXE.DatabaseID);
            trace.AddColumn(TraceColumnIdXE.DatabaseName);
            trace.AddColumn(TraceColumnIdXE.EventSequence);
            trace.AddColumn(TraceColumnIdXE.LinkedServerName);
            trace.AddColumn(TraceColumnIdXE.StartTime);
            trace.AddColumn(TraceColumnIdXE.IsSystem);
            trace.AddColumn(TraceColumnIdXE.SessionLoginName);
            trace.AddColumn(TraceColumnIdXE.SPID);
            trace.AddColumn(TraceColumnIdXE.SQLSecurityLoginName);


            if (config.AuditDML)
            {
                trace.AddFilter(new TraceFilter(TraceColumnId.Permissions,
                                                   TraceFilterComparisonOp.Equal,
                                                   2,         // Update
                                                   TraceFilterLogicalOp.OR));
                trace.AddFilter(new TraceFilter(TraceColumnId.Permissions,
                                                 TraceFilterComparisonOp.Equal,
                                                 8,         // INSERT
                                                 TraceFilterLogicalOp.OR));
                trace.AddFilter(new TraceFilter(TraceColumnId.Permissions,
                                                 TraceFilterComparisonOp.Equal,
                                                 16,         // DELETE
                                                 TraceFilterLogicalOp.OR));
                trace.AddFilter(new TraceFilter(TraceColumnId.Permissions,
                                                 TraceFilterComparisonOp.Equal,
                                                 32,         // EXECUTE
                                                 TraceFilterLogicalOp.OR));
            }

            if (config.AuditSELECT)
            {
                trace.AddFilter(new TraceFilter(TraceColumnId.Permissions,
                                               TraceFilterComparisonOp.Equal,
                                               1,         // SELECT
                                               TraceFilterLogicalOp.OR));

                trace.AddFilter(new TraceFilter(TraceColumnId.Permissions,
                                                 TraceFilterComparisonOp.Equal,
                                                 4,         // REFERENCE ALL
                                                 TraceFilterLogicalOp.OR));
            }
            if (config.AuditCaptureDetails)
                trace.KeepSQLXE = true;
            AddDataChangeEventFilter(trace);

            if (this.isServer)
            {
                if (config.AuditAccessCheck == AccessCheckFilter.FailureOnly)
                    trace.AddFilter(new TraceFilter(TraceColumnId.Success,
                                                     TraceFilterComparisonOp.Equal,
                                                     0,
                                                     TraceFilterLogicalOp.AND));
                else if (config.AuditAccessCheck == AccessCheckFilter.SuccessOnly)
                    trace.AddFilter(new TraceFilter(TraceColumnId.Success,
                                                     TraceFilterComparisonOp.Equal,
                                                     1,
                                                     TraceFilterLogicalOp.AND));

            }
            AddSQLsecureApplicationFilters(trace);



            trace.InstanceAlias = this.InstanceAlias;
            trace.Version = this.Version;
            trace.FileName = Path.Combine(traceDirectory,
                                          TraceFileNameAttributes.ComposeTraceFileNameXE(trace.InstanceAlias,
                                                                                 (int)trace.Level,
                                                                                 (int)trace.Category,
                                                                                 trace.Version,
                                                                                 sequence));
            trace.XESessionName = TraceFileNameAttributes.ComposeSessionNameXE(trace.InstanceAlias,
                                                                                 (int)trace.Level,
                                                                                 (int)trace.Category,
                                                                                 trace.Version,
                                                                                 sequence);

            return trace;

        }

        //5.4 XE
        protected virtual XeTraceConfiguration
          GenerateDMLTraceXE(
             AuditConfiguration config,
             bool privilegedUsers,
             int sequence,
             List<string> excludeUsers
          )
        {
            if (!this.AuditCaptureSQLXE || !config.AuditDML)
                return null;

            XeTraceConfiguration trace = new XeTraceConfiguration();
            trace.Level = TraceLevel.Database;
            trace.Category = TraceCategory.DML;
            trace.Sequence = sequence;
			//To support previous version of agent
            if (this.isServer)
            {
                trace.AddEvent(TraceEventIdXE.SqlStmtStarting);
                trace.AddEvent(TraceEventIdXE.SpStarting);
            }
            trace.AddEvent(TraceEventIdXE.SqlStmtCompleted);
            trace.AddEvent(TraceEventIdXE.SpCompleted);
            trace.AddColumn(TraceColumnIdXE.TextData);
            trace.AddColumn(TraceColumnIdXE.ApplicationName);
            trace.AddColumn(TraceColumnIdXE.ClientHostName);
            trace.AddColumn(TraceColumnIdXE.DatabaseID);
            trace.AddColumn(TraceColumnIdXE.DatabaseName);
            trace.AddColumn(TraceColumnIdXE.EventSequence);
            trace.AddColumn(TraceColumnIdXE.LinkedServerName);
            trace.AddColumn(TraceColumnIdXE.StartTime);
            trace.AddColumn(TraceColumnIdXE.IsSystem);
            trace.AddColumn(TraceColumnIdXE.SessionLoginName);
            trace.AddColumn(TraceColumnIdXE.SPID);
            trace.AddColumn(TraceColumnIdXE.SQLSecurityLoginName);

            if (config.AuditDML &&
              (!privilegedUsers || isServer))
            {

                trace.AddFilter(new TraceFilter(TraceColumnId.Permissions,
                                                    TraceFilterComparisonOp.Equal,
                                                    2,         // Update
                                                    TraceFilterLogicalOp.OR));
                trace.AddFilter(new TraceFilter(TraceColumnId.Permissions,
                                                 TraceFilterComparisonOp.Equal,
                                                 8,         // INSERT
                                                 TraceFilterLogicalOp.OR));
                trace.AddFilter(new TraceFilter(TraceColumnId.Permissions,
                                                 TraceFilterComparisonOp.Equal,
                                                 16,         // DELETE
                                                 TraceFilterLogicalOp.OR));
                trace.AddFilter(new TraceFilter(TraceColumnId.Permissions,
                                                 TraceFilterComparisonOp.Equal,
                                                 32,         // EXECUTE
                                                 TraceFilterLogicalOp.OR));
            }

            if (config.AuditCaptureDetails)
            {
                trace.Category = TraceCategory.DMLwithDetails;
                trace.KeepSQLXE = true;
            }

            if (config is DBAuditConfiguration)
            {
                AddDBIdFilters(trace, ((DBAuditConfiguration)config).DBIdList);
            }

            AddDataChangeEventFilter(trace);
            // Filter out trusted users and SQLsecure activities
            if (!privilegedUsers)
            {


                if (!isServer && config is DBAuditConfiguration && config.AuditedUsers.Length > 0)
                {
                    if (this.UserConfiguration == null || !this.UserConfiguration.AuditDML)
                        excludeUsers = null;
                    AddTrustedUserFiltersEx(trace, config.AuditedUsers, excludeUsers);
                }
                AddSQLsecureApplicationFilters(trace);
            }

            if (this.isServer)
            {
                if (config.AuditAccessCheck == AccessCheckFilter.FailureOnly)
                    trace.AddFilter(new TraceFilter(TraceColumnId.Success,
                                                     TraceFilterComparisonOp.Equal,
                                                     0,
                                                     TraceFilterLogicalOp.AND));
                else if (config.AuditAccessCheck == AccessCheckFilter.SuccessOnly)
                    trace.AddFilter(new TraceFilter(TraceColumnId.Success,
                                                     TraceFilterComparisonOp.Equal,
                                                     1,
                                                     TraceFilterLogicalOp.AND));

            }

            trace.InstanceAlias = this.InstanceAlias;
            trace.Version = this.Version;
            trace.FileName = Path.Combine(traceDirectory,
                                          TraceFileNameAttributes.ComposeTraceFileNameXE(trace.InstanceAlias,
                                                                                 (int)trace.Level,
                                                                                 (int)trace.Category,
                                                                                 trace.Version,
                                                                                 sequence));
            trace.XESessionName = TraceFileNameAttributes.ComposeSessionNameXE(trace.InstanceAlias,
                                                                                 (int)trace.Level,
                                                                                 (int)trace.Category,
                                                                                 trace.Version,
                                                                                 sequence);

            return trace;

        }

        /////<5.3.1>
        ///// Date : 7/15/2016 SQLCM-3799 Trace files are getting generated for trusted user
        /////</5.3.1>
        //public class TrustedUserFilterDBid
        //{
        //    public string dbName;
        //    public List<int> dbId = new List<int>();
        //    public List<string> trustedUser = new List<string>();
        //}

        /////<5.3.1>
        ///// Date : 7/15/2016 SQLCM-3799 Trace files are getting generated for trusted user
        /////</5.3.1>

        //this is the database level transaction auditing.  The privileged user transaction auditing will be in GenerateDMLTrace
        protected virtual TraceConfiguration GenerateTransactionTrace(DBAuditConfiguration config,
                                                                          int sequence,
                                                                          List<string> trustedUsers)
        {
            if (!config.AuditDML)
                return null;

            TraceConfiguration trace = new TraceConfiguration();
            trace.Level = TraceLevel.Database;
            trace.Sequence = sequence;
            trace.AddEvent(TraceEventId.Transaction);
            AddCommonColumns(trace);
            trace.AddColumn(TraceColumnId.EventSubClass);
            trace.AddColumn(TraceColumnId.DatabaseName);
            trace.AddColumn(TraceColumnId.DatabaseID);
            trace.AddColumn(TraceColumnId.Error);
            trace.Category = TraceCategory.Transactions;

            if (config.AuditCaptureDetails)
            {
                trace.AddColumn(TraceColumnId.TextData);
                trace.KeepSQL = true;
            }
            AddObjectTypeFilters(config, trace);
            AddDBIdFilters(trace, config.DBIdList);

            if (this.isServer)
            {
                if (config.AuditAccessCheck == AccessCheckFilter.FailureOnly)
                    trace.AddFilter(new TraceFilter(TraceColumnId.Success,
                                                     TraceFilterComparisonOp.Equal,
                                                     0,
                                                     TraceFilterLogicalOp.AND));
                else if (config.AuditAccessCheck == AccessCheckFilter.SuccessOnly)
                    trace.AddFilter(new TraceFilter(TraceColumnId.Success,
                                                     TraceFilterComparisonOp.Equal,
                                                     1,
                                                     TraceFilterLogicalOp.AND));

            }

            //This has to be before the ObjectName filters because it is a parentName filter, then an object name
            //The common filters have to be applied on after another in the trace.
            AddDataChangeEventFilter(trace);

            if (config.AuditedUsers.Length > 0)
            {
                AddTrustedUserFiltersEx(trace, config.AuditedUsers, trustedUsers);
            }
            AddSQLsecureApplicationFilters(trace);
            trace.InstanceAlias = this.InstanceAlias;
            trace.Version = this.Version;
            trace.FileName = Path.Combine(traceDirectory,
                                          TraceFileNameAttributes.ComposeTraceFileName(trace.InstanceAlias,
                                                                                 (int)trace.Level,
                                                                                 (int)trace.Category,
                                                                                 trace.Version,
                                                                                 sequence));
            return trace;
        }

        //-----------------------------------------------------------
        // GenerateSELECTTraces
        //-----------------------------------------------------------
        /// <summary>
        /// Generate database level DML traces.
        /// </summary>
        /// <returns></returns>
        protected virtual TraceConfiguration[]
           GenerateSELECTTraces(
              bool privilegedUsers
           )
        {
            if (isAuditLogsEnabled && !isServer)
                return null;
            if (this.AuditCaptureSQLXE && !privilegedUsers)
                return null;
            ArrayList traces = new ArrayList();
            int sequence = 0;
            int scSequence = 0;
            ArrayList traceSets = CreateConfigurationSets(AuditDBList, TraceCategory.SELECT);
            if (traceSets == null)
                return null;

            TraceConfiguration trace;
            // DE41609 To avoid creating multiple trace files for one instance when sensitive columns configured for multiple databases
            //bool isSensitiveColumnTracedGenerated = false;     
            foreach (AuditConfiguration config in traceSets)
            {
                if (config == null)
                    continue;

                trace = GenerateSELECTTrace(config, privilegedUsers, sequence);
                if (trace != null)
                {
                    sequence++;
                    traces.Add(trace);
                }

                if (!privilegedUsers)
                {
                    List<string> pUsers = privilegedUsers ? null : ConfigurationHelper.GetPrivUsers(this);
                    DBAuditConfiguration dbConfig = (DBAuditConfiguration)config;
                    if (dbConfig.SensitiveColumns != null &&
                         dbConfig.SensitiveColumns.Length > 0)
                    {
                        trace = GenerateSensitiveColumnTrace(dbConfig, scSequence, pUsers);
                        if (trace != null)
                        {
                            scSequence++;
                            traces.Add(trace);
                        }
                    }
                }
            }
            return (TraceConfiguration[])traces.ToArray(typeof(TraceConfiguration));

        }


        //5.4 XE
        protected virtual XeTraceConfiguration[]
           GenerateSELECTTracesXE(
              bool privilegedUsers
           )
        {
            if (!this.AuditCaptureSQLXE)
                return null;
            ArrayList traces = new ArrayList();
            int sequence = 0;
            int scSequence = 0;
            ArrayList traceSets = CreateConfigurationSets(AuditDBList, TraceCategory.SELECT);
            if (traceSets == null)
                return null;

            XeTraceConfiguration trace;
            // DE41609 To avoid creating multiple trace files for one instance when sensitive columns configured for multiple databases
            //bool isSensitiveColumnTracedGenerated = false;     
            foreach (AuditConfiguration config in traceSets)
            {
                if (config == null)
                    continue;

                trace = GenerateSELECTTraceXE(config, privilegedUsers, sequence);
                if (trace != null)
                {
                    sequence++;
                    traces.Add(trace);
                }

                if (!privilegedUsers)
                {
                    List<string> pUsers = privilegedUsers ? null : ConfigurationHelper.GetPrivUsers(this);
                    DBAuditConfiguration dbConfig = (DBAuditConfiguration)config;
                    if (dbConfig.SensitiveColumns != null &&
                         dbConfig.SensitiveColumns.Length > 0)
                    {
                        scSequence++;
                        trace = GenerateSensitiveColumnTraceXE(dbConfig, scSequence, pUsers);
                        if (trace != null)
                        {
                            traces.Add(trace);
                        }                        
                    }
                }
            }
            return (XeTraceConfiguration[])traces.ToArray(typeof(XeTraceConfiguration));

        }

        ////-----------------------------------------------------------
        //// 5.4 XE
        ////-----------------------------------------------------------
        ///// <summary>
        ///// GenerateSELECTTraces
        ///// </summary>
        ///// <returns></returns>
        //protected virtual XeTraceConfiguration[]
        //   GenerateSELECTTracesXE(
        //      bool privilegedUsers
        //   )
        //{
        //   ArrayList traces   = new ArrayList();
        //   int       sequence = 0;
        //   int       scSequence = 0;
        //   ArrayList traceSets = CreateConfigurationSets( AuditDBList, TraceCategory.SELECT );
        //   if( traceSets == null )
        //      return null;

        //   XeTraceConfiguration trace;
        //   // DE41609 To avoid creating multiple trace files for one instance when sensitive columns configured for multiple databases
        //   //bool isSensitiveColumnTracedGenerated = false;     
        //   foreach( AuditConfiguration config in traceSets )
        //   {
        //      if( config == null )
        //         continue;

        //      trace = GenerateSELECTTraceXE( config, privilegedUsers, sequence );
        //      if( trace != null )
        //      {
        //         sequence++;
        //         traces.Add( trace );
        //      }


        //      if (!privilegedUsers)
        //      {
        //          List<string> pUsers = privilegedUsers ? null : ConfigurationHelper.GetPrivUsers(this);
        //          DBAuditConfiguration dbConfig = (DBAuditConfiguration)config;
        //            if (dbConfig.SensitiveColumns != null &&
        //                 dbConfig.SensitiveColumns.Length > 0)
        //            {
        //                trace = GenerateSensitiveColumnTraceXE(dbConfig, scSequence,pUsers);
        //                traces.Add(trace);
        //                scSequence++;
        //            }
        //    }
        //    return (XeTraceConfiguration[])traces.ToArray(typeof(XeTraceConfiguration));

        //}

        //-----------------------------------------------------------
        // GenerateSELECTTrace
        //-----------------------------------------------------------
        /// <summary>
        /// Generate trace configuration for SELECT events.
        /// </summary>
        /// <returns></returns>
        protected virtual TraceConfiguration
           GenerateSELECTTrace(
              AuditConfiguration config,
              bool privilegedUsers,
              int sequence
           )
        {
            if (!config.AuditSELECT)
                return null;

            TraceConfiguration trace = new TraceConfiguration();
            if (privilegedUsers)
            {
                trace.Level = TraceLevel.User;
            }
            else
                trace.Level = TraceLevel.Database;
            trace.Sequence = sequence;


            AddCommonColumns(trace);

            //if ( config.AuditExceptions )
            //{
            trace.AddEvent(TraceEventId.Exception);
            //   trace.AddEvent( TraceEventId.SqlStmtCompleted );
            //}

            trace.AddEvent(TraceEventId.AuditObjectPermission);
            if (sqlVersion >= 9)
                trace.AddEvent(TraceEventId.AuditDatabaseObjectAccess);

            trace.AddColumn(TraceColumnId.Permissions);
            trace.AddColumn(TraceColumnId.EventSubClass);
            trace.AddColumn(TraceColumnId.DatabaseName);
            trace.AddColumn(TraceColumnId.DatabaseID);
            trace.AddColumn(TraceColumnId.Error);
            if (EnableRowCountForDatabaseAuditing)
            {
                trace.AddColumn(TraceColumnId.RowCounts); // Added to get Row Counts
            }

            if (!privilegedUsers || isServer)
            {
                trace.AddFilter(new TraceFilter(TraceColumnId.Permissions,
                                                 TraceFilterComparisonOp.Equal,
                                                 1,         // SELECT
                                                 TraceFilterLogicalOp.OR));

                trace.AddFilter(new TraceFilter(TraceColumnId.Permissions,
                                                 TraceFilterComparisonOp.Equal,
                                                 4,         // REFERENCE ALL
                                                 TraceFilterLogicalOp.OR));
            }
            
            if (this.sqlVersion > 9)
            {
                if (EnableRowCountForDatabaseAuditing)
                {
                    trace.AddEvent(TraceEventId.SqlStmtCompleted);  // Changed From SqlStmtStarting to SqlStmtCompleted for Row Counts
                    trace.AddEvent(TraceEventId.SpStmtCompleted);
                }

                if (config.AuditCaptureDetails && !this.isServer)
                    trace.AddColumn(TraceColumnId.TextData);
                if (!privilegedUsers && !isServer)
                {
                    trace.AddFilter(new TraceFilter(TraceColumnId.TextData,
                                                     TraceFilterComparisonOp.Like,
                                                     "S%",       
                                                     TraceFilterLogicalOp.AND));
                    trace.AddFilter(new TraceFilter(TraceColumnId.TextData,
                                                     TraceFilterComparisonOp.Like,
                                                     "s%",         
                                                     TraceFilterLogicalOp.OR));
                    trace.AddFilter(new TraceFilter(TraceColumnId.TextData,
                                                     TraceFilterComparisonOp.Like,
                                                     "R%",        
                                                     TraceFilterLogicalOp.OR));
                    trace.AddFilter(new TraceFilter(TraceColumnId.TextData,
                                                     TraceFilterComparisonOp.Like,
                                                     "r%",         
                                                     TraceFilterLogicalOp.OR));
                }
            }
            if (config.AuditCaptureDetails)
            {
				//To support previous version of agent
                if (this.isServer || this.sqlVersion < 10)
                {
                    trace.AddEvent(TraceEventId.SqlStmtStarting);
                }
                trace.AddColumn(TraceColumnId.TextData);
                trace.Category = TraceCategory.SELECTwithDetails;
                trace.KeepSQL = true;
            }
            else
            {
                trace.Category = TraceCategory.SELECT;
                trace.KeepSQL = false;
            }

            if (config is DBAuditConfiguration)
            {
                AddObjectTypeFilters((DBAuditConfiguration)config, trace);
                if (isServer)
                    AddObjectFilters(trace, config.AuditObjectList);
                AddDBIdFilters(trace, ((DBAuditConfiguration)config).DBIdList);
            }

            if (!privilegedUsers)
            {
                if (!isServer && config is DBAuditConfiguration && config.AuditedUsers.Length > 0)
                {
                    AddTrustedUserFilters(trace, config.AuditedUsers);
                }
                AddSQLsecureApplicationFilters(trace);
                AddDataChangeEventFilter(trace);
            }

            if (this.isServer)
            {
                if (config.AuditAccessCheck == AccessCheckFilter.FailureOnly)
                    trace.AddFilter(new TraceFilter(TraceColumnId.Success,
                                                     TraceFilterComparisonOp.Equal,
                                                     0,
                                                     TraceFilterLogicalOp.AND));
                else if (config.AuditAccessCheck == AccessCheckFilter.SuccessOnly)
                    trace.AddFilter(new TraceFilter(TraceColumnId.Success,
                                                     TraceFilterComparisonOp.Equal,
                                                     1,
                                                     TraceFilterLogicalOp.AND));

            }

            trace.InstanceAlias = this.InstanceAlias;
            trace.Category = TraceCategory.SELECTwithDetails;
            trace.Version = this.Version;
            trace.FileName = Path.Combine(traceDirectory,
                                 TraceFileNameAttributes.ComposeTraceFileName(trace.InstanceAlias,
                                                                              (int)trace.Level,
                                                                              (int)trace.Category,
                                                                              trace.Version,
                                                                              sequence));
            return trace;

        }


        //5.4 XE 1st
        protected virtual XeTraceConfiguration
           GenerateSELECTTraceXE(
              AuditConfiguration config,
              bool privilegedUsers,
              int sequence
           )
        {
            if (!config.AuditSELECT)
                return null;

            XeTraceConfiguration trace = new XeTraceConfiguration();
            if (privilegedUsers)
            {
                trace.Level = TraceLevel.User;
            }
            else
                trace.Level = TraceLevel.Database;
            trace.Sequence = sequence;
			//To support previous version of agent
            if (this.isServer)
            {
                trace.AddEvent(TraceEventIdXE.SqlStmtStarting);
                trace.AddEvent(TraceEventIdXE.SpStarting);
            }
            trace.AddEvent(TraceEventIdXE.SqlStmtCompleted);
            trace.AddEvent(TraceEventIdXE.SpCompleted);
            trace.AddColumn(TraceColumnIdXE.TextData);
            trace.AddColumn(TraceColumnIdXE.ApplicationName);
            trace.AddColumn(TraceColumnIdXE.ClientHostName);
            trace.AddColumn(TraceColumnIdXE.DatabaseID);
            trace.AddColumn(TraceColumnIdXE.DatabaseName);
            trace.AddColumn(TraceColumnIdXE.EventSequence);
            trace.AddColumn(TraceColumnIdXE.LinkedServerName);
            trace.AddColumn(TraceColumnIdXE.StartTime);
            trace.AddColumn(TraceColumnIdXE.IsSystem);
            trace.AddColumn(TraceColumnIdXE.SessionLoginName);
            trace.AddColumn(TraceColumnIdXE.SPID);
            trace.AddColumn(TraceColumnIdXE.SQLSecurityLoginName);

            if (!privilegedUsers || isServer)
            {
                trace.AddFilter(new TraceFilter(TraceColumnId.Permissions,
                                                 TraceFilterComparisonOp.Equal,
                                                 1,         // SELECT
                                                 TraceFilterLogicalOp.OR));

                trace.AddFilter(new TraceFilter(TraceColumnId.Permissions,
                                                 TraceFilterComparisonOp.Equal,
                                                 4,         // REFERENCE ALL
                                                 TraceFilterLogicalOp.OR));
            }

            if (config.AuditCaptureDetails)
            {
                trace.Category = TraceCategory.SELECTwithDetails;
                trace.KeepSQLXE = true;
            }
            else
            {
                trace.Category = TraceCategory.SELECT;
                trace.KeepSQLXE = false;
            }

            if (config is DBAuditConfiguration)
            {
                AddDBIdFilters(trace, ((DBAuditConfiguration)config).DBIdList);
            }

            if (!privilegedUsers)
            {
                List<string> excludeUsers = ConfigurationHelper.GetPrivUsers(this);
                if (!isServer && config is DBAuditConfiguration && config.AuditedUsers.Length > 0)
                {
                    if (this.userConfiguration == null || !this.userConfiguration.AuditSELECT)
                    {
                        excludeUsers = null;
                    }
                    AddTrustedUserFiltersEx(trace, config.AuditedUsers, excludeUsers);
                }
                if (!this.isServer)
                {
                    int auditSelect = Array.FindIndex(((DBAuditConfiguration)config).UserCategories, item => item == AuditCategory.SELECT);
                    if (auditSelect != -1)
                    {

                        List<string> users = new List<string>();
                        string[] privUsers = ((DBAuditConfiguration)config).PrivUsers;
                        List<string> trustUsers = new List<string>(config.AuditedUsers);
                        if (privUsers != null && privUsers.Length > 0)
                        {
                            foreach (string user in privUsers)
                            {
                                if (!trustUsers.Contains(user))
                                {
                                    users.Add(user);
                                }
                            }
                        }
                        if (excludeUsers != null && excludeUsers.Count > 0
                            && this.userConfiguration != null && this.userConfiguration.AuditSELECT)
                            foreach (string user in excludeUsers)
                            {
                                if (!users.Contains(user))
                                {
                                    users.Add(user);
                                }
                            }
                        excludeDBPrivilegedUserFilters(trace, users.ToArray());
                    }
                }

                AddDataChangeEventFilter(trace);
                AddSQLsecureApplicationFilters(trace);
                /*
                if( sqlVersion >= 9 &&
                    !config.AuditSystemEvents )
                   trace.AddFilter( new TraceFilter( TraceColumnId.IsSystem, 
                                                  TraceFilterComparisonOp.NotEqual,
                                                  1,
                                                  TraceFilterLogicalOp.AND ) );
                                                  */

            }

            if (this.isServer)
            {
                if (config.AuditAccessCheck == AccessCheckFilter.FailureOnly)
                    trace.AddFilter(new TraceFilter(TraceColumnId.Success,
                                                     TraceFilterComparisonOp.Equal,
                                                     0,
                                                     TraceFilterLogicalOp.AND));
                else if (config.AuditAccessCheck == AccessCheckFilter.SuccessOnly)
                    trace.AddFilter(new TraceFilter(TraceColumnId.Success,
                                                     TraceFilterComparisonOp.Equal,
                                                     1,
                                                     TraceFilterLogicalOp.AND));

            }

            trace.InstanceAlias = this.InstanceAlias;
            trace.Category = TraceCategory.SELECTwithDetails;
            trace.Version = this.Version;
            trace.FileName = Path.Combine(traceDirectory,
                                 TraceFileNameAttributes.ComposeTraceFileNameXE(trace.InstanceAlias,
                                                                              (int)trace.Level,
                                                                              (int)trace.Category,
                                                                              trace.Version,
                                                                              sequence));

            trace.XESessionName = TraceFileNameAttributes.ComposeSessionNameXE(trace.InstanceAlias,
                                                                                (int)trace.Level,
                                                                                (int)trace.Category,
                                                                                trace.Version,
                                                                                sequence);
            return trace;

        }


        #endregion

        #region Data Change Trace

        protected void
           AddDataChangeEventFilter(TraceConfiguration trace)
        {
            if (sqlVersion < 9)  // Data change features are not available before SQL 2005
                return;
            // filter out DML trigger generated events
            trace.AddFilter(new TraceFilter(TraceColumnId.ParentName,
                                              TraceFilterComparisonOp.NotLike,
                                              CoreConstants.Agent_BeforeAfter_SchemaName,
                                              TraceFilterLogicalOp.AND));

            trace.AddFilter(new TraceFilter(TraceColumnId.ObjectName,
                                              TraceFilterComparisonOp.NotLike,
                                              CoreConstants.Agent_BeforeAfter_TableName,
                                              TraceFilterLogicalOp.AND));
        }

        //5.4 XE
        protected void
          AddDataChangeEventFilter(XeTraceConfiguration trace)
        {
            if (sqlVersion < 9)  // Data change features are not available before SQL 2005
                return;
            // filter out DML trigger generated events
            trace.AddFilter(new TraceFilter(TraceColumnId.ParentName,
                                              TraceFilterComparisonOp.NotLike,
                                              CoreConstants.Agent_BeforeAfter_SchemaName,
                                              TraceFilterLogicalOp.AND));

            trace.AddFilter(new TraceFilter(TraceColumnId.ObjectName,
                                              TraceFilterComparisonOp.NotLike,
                                              CoreConstants.Agent_BeforeAfter_TableName,
                                              TraceFilterLogicalOp.AND));
        }

        //-----------------------------------------------------------
        // GenerateDataChangeTrace
        //-----------------------------------------------------------
        /// <summary>
        /// Generate trace configuration for SELECT events generated by before/after triggers.
        /// Note that there is one trace for each instance.
        /// </summary>
        /// <returns></returns>
        protected virtual TraceConfiguration
           GenerateDataChangeTrace(
              DBAuditConfiguration config,
              int sequence,
              List<string> pUsers
  )
        {
            TraceConfiguration trace = new TraceConfiguration();
            trace.Level = TraceLevel.Table;
            trace.Sequence = sequence;

            // Event: Schema Object Access Event
            trace.AddEvent(TraceEventId.AuditObjectPermission);

            // The columns needed to capture DML before/after data
            // trace.AddColumn( TraceColumnId.EventClass ); default column
            trace.AddColumn(TraceColumnId.StartTime);
            trace.AddColumn(TraceColumnId.SPID);
            trace.AddColumn(TraceColumnId.EventSequence);
            trace.AddColumn(TraceColumnId.DatabaseID);
            trace.AddColumn(TraceColumnId.DatabaseName);
            trace.AddColumn(TraceColumnId.TextData);

            AddDBIdFilters(trace, config.DBIdList);


            // Filters
            trace.KeepSQL = true;
            trace.AddFilter(new TraceFilter(TraceColumnId.Permissions,
                                              TraceFilterComparisonOp.Equal,
                                              1,
                                              TraceFilterLogicalOp.AND));

            trace.AddFilter(new TraceFilter(TraceColumnId.ParentName,
                                              TraceFilterComparisonOp.Like,
                                              CoreConstants.Agent_BeforeAfter_SchemaName,
                                              TraceFilterLogicalOp.AND));

            trace.AddFilter(new TraceFilter(TraceColumnId.ObjectName,
                                                TraceFilterComparisonOp.Like,
                                                CoreConstants.Agent_BeforeAfter_TableName,
                                                TraceFilterLogicalOp.AND));

            if (!isServer && config.AuditedUsers.Length > 0)
            {
                AddTrustedUserFiltersEx(trace, config.AuditedUsers, pUsers);
            }

            AddSQLsecureApplicationFilters(trace);

            trace.InstanceAlias = this.InstanceAlias;
            trace.Category = TraceCategory.DataChange;
            trace.Version = this.Version;
            trace.FileName = Path.Combine(traceDirectory,
                                 TraceFileNameAttributes.ComposeTraceFileName(trace.InstanceAlias,
                                                                              (int)trace.Level,
                                                                              (int)trace.Category,
                                                                              trace.Version,
                                                                              sequence));
            return trace;

        }

        //5.4 XE
        protected virtual XeTraceConfiguration
         GenerateDataChangeTraceXE(
            DBAuditConfiguration config,
            int sequence,
            List<string> pUsers
  )
        {
            XeTraceConfiguration trace = new XeTraceConfiguration();
            trace.Level = TraceLevel.Table;
            trace.Sequence = sequence;

            // The columns needed to capture DML before/after data
            // trace.AddColumn( TraceColumnId.EventClass ); default column
			//To support previous version of agent
            if (this.isServer)
            {
                trace.AddEvent(TraceEventIdXE.SqlStmtStarting);
                trace.AddEvent(TraceEventIdXE.SpStarting);
            }
            trace.AddEvent(TraceEventIdXE.SqlStmtCompleted);
            trace.AddEvent(TraceEventIdXE.SpCompleted);
            trace.AddColumn(TraceColumnIdXE.StartTime);
            trace.AddColumn(TraceColumnIdXE.SPID);
            trace.AddColumn(TraceColumnIdXE.EventSequence);
            trace.AddColumn(TraceColumnIdXE.DatabaseID);
            trace.AddColumn(TraceColumnIdXE.DatabaseName);
            trace.AddColumn(TraceColumnIdXE.TextData);
            trace.KeepSQLXE = true;

            AddDBIdFilters(trace, config.DBIdList);


            trace.AddFilter(new TraceFilter(TraceColumnId.Permissions,
                                              TraceFilterComparisonOp.Equal,
                                              1,
                                              TraceFilterLogicalOp.AND));

            trace.AddFilter(new TraceFilter(TraceColumnId.ParentName,
                                              TraceFilterComparisonOp.Like,
                                              CoreConstants.Agent_BeforeAfter_SchemaName,
                                              TraceFilterLogicalOp.AND));

            trace.AddFilter(new TraceFilter(TraceColumnId.ObjectName,
                                                TraceFilterComparisonOp.Like,
                                                CoreConstants.Agent_BeforeAfter_TableName,
                                                TraceFilterLogicalOp.AND));

            if (!isServer && config.AuditedUsers.Length > 0)
            {
                AddTrustedUserFiltersEx(trace, config.AuditedUsers, null);
            }

            AddSQLsecureApplicationFilters(trace);

            trace.InstanceAlias = this.InstanceAlias;
            trace.Category = TraceCategory.DataChange;
            trace.Version = this.Version;
            trace.FileName = Path.Combine(traceDirectory, TraceFileNameAttributes.ComposeTraceFileNameXE(trace.InstanceAlias,
                                                                                                      (int)trace.Level,
                                                                                                      (int)trace.Category,
                                                                                                      trace.Version,
                                                                                                      sequence));
            trace.XESessionName = TraceFileNameAttributes.ComposeSessionNameXE(trace.InstanceAlias,
                                                                                (int)trace.Level,
                                                                                (int)trace.Category,
                                                                                trace.Version,
                                                                                sequence);
            return trace;

        }
       

        //-----------------------------------------------------------
        // GenerateSensitiveColumnTrace
        //-----------------------------------------------------------
        /// <summary>
        /// Generate trace configuration for SELECT events for Sensitive column access.
        /// Note that there is one trace for each instance.
        /// </summary>
        /// <returns></returns>
        protected virtual TraceConfiguration GenerateSensitiveColumnTrace(DBAuditConfiguration config,
                                                                          int sequence,
                                                                          List<string> pUsers)
        {

            TraceConfiguration trace = new TraceConfiguration();
            trace.Level = TraceLevel.Table;
            trace.Sequence = sequence;
            trace.KeepSQL = true;


            //Event - 114 - Audit Schema Object Access
            trace.AddEvent(TraceEventId.AuditObjectPermission);
            if (config.AuditSELECT || config.AuditDDL || config.AuditDML)
            {
                //Event - 114 - Audit Schema Object Access
                trace.AddEvent(TraceEventId.AuditObjectPermission);
                // SQLCM-5471 v5.6 Add Activity to Senstitive columns
                if (sqlVersion >= 9)
                    trace.AddEvent(TraceEventId.AuditDatabaseObjectAccess);
            }
            //We use this event to get the sql text that was issued to SQL Server.  Not the SQL that was executed.
            if (this.isServer)
            {
                trace.AddEvent(TraceEventId.SqlStmtStarting);
            }

            //Trace Columns
            AddCommonColumns(trace);
            trace.AddColumn(TraceColumnId.TextData);
            trace.AddColumn(TraceColumnId.Permissions);
            trace.AddColumn(TraceColumnId.EventSubClass);
            trace.AddColumn(TraceColumnId.DatabaseName);
            trace.AddColumn(TraceColumnId.DatabaseID);
            trace.AddColumn(TraceColumnId.Error);
            if (EnableRowCountForDatabaseAuditing)
            {
                trace.AddEvent(TraceEventId.SqlStmtCompleted);  // Changed From SqlStmtStarting to SqlStmtCompleted for Row Counts
                trace.AddEvent(TraceEventId.SpStmtCompleted);
                trace.AddColumn(TraceColumnId.RowCounts);
            }

            //Trace Filters
            if (config.AuditSELECT)
                //Add the select filters
                AddObjectTypeFilters((DBAuditConfiguration)config, trace);

            //we can no longer filter on database Id.
            //AddDBIdFilters(trace, config.DBIdList);
            trace.AddFilter(new TraceFilter(TraceColumnId.Permissions,
                                              TraceFilterComparisonOp.Equal,
                                              (int)TraceEventType.SELECT,
                                              TraceFilterLogicalOp.AND));
            // SQLCM-5471 v5.6 Add Activity to Senstitive columns
            if (config.AuditDML && !isServer)
            {
                trace.AddFilter(new TraceFilter(TraceColumnId.Permissions,
                                                TraceFilterComparisonOp.Equal,
                                                8,         // INSERT
                                                TraceFilterLogicalOp.OR));
                trace.AddFilter(new TraceFilter(TraceColumnId.Permissions,
                                                 TraceFilterComparisonOp.Equal,
                                                 16,         // DELETE
                                                 TraceFilterLogicalOp.OR));
            }
            if (config.AuditDDL || config.AuditDML)
            {
                if (sqlVersion >= 9)
                {
                    trace.AddEvent(TraceEventId.AuditDatabaseObjectManagement);
                    trace.AddEvent(TraceEventId.AuditSchemaObjectManagement);
                    trace.AddEvent(TraceEventId.AuditDatabaseManagement);
                }
                else
                {
                    trace.AddEvent(TraceEventId.AuditObjectDerivedPermission);
                    // This one contains the statement that creates the object
                    trace.AddEvent(TraceEventId.AuditStatementPermission);
                    // This one provides object name, type and ID
                    trace.AddEvent(TraceEventId.ObjectCreated);
                }
            }
            //SQLCM-5471 v5.6
            if (config.AuditAdmin)
            {
                if(sqlVersion >=9)
                    trace.AddEvent(TraceEventId.AuditDatabaseOperation);
                trace.AddEvent(TraceEventId.AuditDbcc);
            }
            // SQLCM-5471 v5.6 Add Activity to Senstitive columns - END

            trace.AddFilter(new TraceFilter(TraceColumnId.DatabaseID,
                                           TraceFilterComparisonOp.Equal,
                                           config.DBId,
                                           TraceFilterLogicalOp.AND));

            //This has to be before the ObjectName filters because it is a parentName filter, then an object name
            //The common filters have to be applied on after another in the trace.
            AddDataChangeEventFilter(trace);
            int i = 0;
            foreach (TableConfiguration tableConfig in config.SensitiveColumns)
            {
                trace.AddFilter(new TraceFilter(TraceColumnId.ObjectName,
                                                    TraceFilterComparisonOp.Like,
                                                    tableConfig.Name,
                                                    (i == 0) ? TraceFilterLogicalOp.AND : TraceFilterLogicalOp.OR));
                i++;
            }

            if (!isServer && config.AuditedUsers.Length > 0)
            {
                AddTrustedUserFiltersEx(trace, config.AuditedUsers, pUsers);
            }
            AddSQLsecureApplicationFilters(trace);
            // SQLCM-5471 v5.6 Add Activity to Senstitive columns
            if (this.isServer)
            {
                if (config.AuditAccessCheck == AccessCheckFilter.FailureOnly)
                    trace.AddFilter(new TraceFilter(TraceColumnId.Success,
                                                     TraceFilterComparisonOp.Equal,
                                                     0,
                                                     TraceFilterLogicalOp.AND));
                else if (config.AuditAccessCheck == AccessCheckFilter.SuccessOnly)
                    trace.AddFilter(new TraceFilter(TraceColumnId.Success,
                                                     TraceFilterComparisonOp.Equal,
                                                     1,
                                                     TraceFilterLogicalOp.AND));
            }
            // SQLCM-5471 v5.6 Add Activity to Senstitive columns - END
            trace.InstanceAlias = this.InstanceAlias;

            if (config.AuditSELECT)
                trace.Category = TraceCategory.SensitiveColumnwithSelect;
            else
                trace.Category = TraceCategory.SensitiveColumn;
            trace.Version = this.Version;
            trace.FileName = Path.Combine(traceDirectory, TraceFileNameAttributes.ComposeTraceFileName(trace.InstanceAlias,
                                                                                                      (int)trace.Level,
                                                                                                      (int)trace.Category,
                                                                                                      trace.Version,
                                                                                                      sequence));
            return trace;
        }

        //5.4 XE
        protected virtual XeTraceConfiguration GenerateSensitiveColumnTraceXE(DBAuditConfiguration config,
                                                                         int sequence,
                                                                         List<string> pUsers)
        {

            //if (!config.AuditSELECT && !config.AuditDDL && !config.AuditDML)
            //    return null;

            XeTraceConfiguration trace = new XeTraceConfiguration();
            trace.Level = TraceLevel.Table;
            trace.Sequence = sequence;
            if (this.isServer)
            {
                trace.AddEvent(TraceEventIdXE.SqlStmtStarting);
                trace.AddEvent(TraceEventIdXE.SpStarting);
            }
            trace.AddEvent(TraceEventIdXE.SqlStmtCompleted);
            trace.AddEvent(TraceEventIdXE.SpCompleted);
            trace.AddColumn(TraceColumnIdXE.TextData);
            trace.AddColumn(TraceColumnIdXE.ApplicationName);
            trace.AddColumn(TraceColumnIdXE.ClientHostName);
            trace.AddColumn(TraceColumnIdXE.DatabaseID);
            trace.AddColumn(TraceColumnIdXE.DatabaseName);
            trace.AddColumn(TraceColumnIdXE.EventSequence);
            trace.AddColumn(TraceColumnIdXE.LinkedServerName);
            trace.AddColumn(TraceColumnIdXE.StartTime);
            trace.AddColumn(TraceColumnIdXE.IsSystem);
            trace.AddColumn(TraceColumnIdXE.SessionLoginName);
            trace.AddColumn(TraceColumnIdXE.SPID);
            trace.AddColumn(TraceColumnIdXE.SQLSecurityLoginName);
            trace.KeepSQLXE = true;

            //AddDBIdFilters(trace, config.DBIdList);

            trace.AddFilter(new TraceFilter(TraceColumnId.Permissions,
                                                  TraceFilterComparisonOp.Equal,
                                                  (int)TraceEventType.SELECT,
                                                  TraceFilterLogicalOp.AND));

            if (config.AuditSensitiveColumnActivity == SensitiveColumnActivity.SelectAndDML ||
                config.AuditSensitiveColumnActivityDataset == SensitiveColumnActivity.SelectAndDML
                )
            {
                trace.AddFilter(new TraceFilter(TraceColumnId.Permissions,
                                              TraceFilterComparisonOp.Equal,
                                              (int)TraceEventType.UPDATE,
                                              TraceFilterLogicalOp.OR));

                trace.AddFilter(new TraceFilter(TraceColumnId.Permissions,
                                              TraceFilterComparisonOp.Equal,
                                              (int)TraceEventType.INSERT,
                                              TraceFilterLogicalOp.OR));

                trace.AddFilter(new TraceFilter(TraceColumnId.Permissions,
                                              TraceFilterComparisonOp.Equal,
                                              (int)TraceEventType.DELETE,
                                              TraceFilterLogicalOp.OR));

                trace.AddFilter(new TraceFilter(TraceColumnId.Permissions,
                                              TraceFilterComparisonOp.Equal,
                                              (int)TraceEventType.EXECUTE,
                                              TraceFilterLogicalOp.OR));
            }
                

            // SQLCM-5471 v5.6 Add Activity to Senstitive columns
            if (config.AuditDML)
            {
                trace.AddFilter(new TraceFilter(TraceColumnId.Permissions,
                                                   TraceFilterComparisonOp.Equal,
                                                   2,         // Update
                                                   TraceFilterLogicalOp.OR));
                trace.AddFilter(new TraceFilter(TraceColumnId.Permissions,
                                                 TraceFilterComparisonOp.Equal,
                                                 8,         // INSERT
                                                 TraceFilterLogicalOp.OR));
                trace.AddFilter(new TraceFilter(TraceColumnId.Permissions,
                                                 TraceFilterComparisonOp.Equal,
                                                 16,         // DELETE
                                                 TraceFilterLogicalOp.OR));
                trace.AddFilter(new TraceFilter(TraceColumnId.Permissions,
                                                 TraceFilterComparisonOp.Equal,
                                                 32,         // EXECUTE
                                                 TraceFilterLogicalOp.OR));
            }
            if (config.AuditDDL)
            {
                trace.AddFilter(new TraceFilter(TraceColumnId.Permissions,
                                                 TraceFilterComparisonOp.Equal,
                                                 217,         // Alter user Tables
                                                 TraceFilterLogicalOp.OR));
            }
            // SQLCM-5471 v5.6 Add Activity to Senstitive columns - END

            trace.AddFilter(new TraceFilter(TraceColumnId.DatabaseID,
                                           TraceFilterComparisonOp.Equal,
                                           config.DBId,
                                           TraceFilterLogicalOp.AND));

            int i = 0;
            foreach (TableConfiguration tableConfig in config.SensitiveColumns)
            {
                trace.AddFilter(new TraceFilter(TraceColumnId.ObjectName,
                                                    TraceFilterComparisonOp.Like,
                                                    tableConfig.Name,
                                                    (i == 0) ? TraceFilterLogicalOp.AND : TraceFilterLogicalOp.OR));
                i++;
            }

            AddDataChangeEventFilter(trace);

            if (!isServer && config.AuditedUsers.Length > 0)
            {
                AddTrustedUserFiltersEx(trace, config.AuditedUsers, null);
            }

            AddSQLsecureApplicationFilters(trace);
            trace.InstanceAlias = this.InstanceAlias;

            if (config.AuditSELECT)
                trace.Category = TraceCategory.SensitiveColumnwithSelect;
            else
                trace.Category = TraceCategory.SensitiveColumn;
            trace.Version = this.Version;
            trace.FileName = Path.Combine(traceDirectory, TraceFileNameAttributes.ComposeTraceFileNameXE(trace.InstanceAlias,
                                                                                                      (int)trace.Level,
                                                                                                      (int)trace.Category,
                                                                                                      trace.Version,
                                                                                                      sequence));
            trace.XESessionName = TraceFileNameAttributes.ComposeSessionNameXE(trace.InstanceAlias,
                                                                                (int)trace.Level,
                                                                                (int)trace.Category,
                                                                                trace.Version,
                                                                                sequence);
            return trace;

        }

        #endregion

        #region Utilities
        //-----------------------------------------------------------
        // RemoveSQLsecureDBs
        //-----------------------------------------------------------
        /// <summary>
        /// Remove SQLsecure database IDs from the audited database list.
        /// </summary>
        protected void
           RemoveSQLsecureDBs()
        {
            if (sqlsecureDBList == null ||
               sqlsecureDBList.Count == 0)
                return;

            IDictionaryEnumerator enumerator = sqlsecureDBList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (dbConfigs.ContainsKey((int)enumerator.Key))
                    dbConfigs.Remove((int)enumerator.Key);
            }
        }

       private bool GetEnableRowCountForDatabaseAuditingValueFromAppConfig()
       {
           bool isEnabledRowCountEnabled = false;
           try
           {
              isEnabledRowCountEnabled = bool.Parse(ConfigurationManager.AppSettings[CoreConstants.Agent_EnableRowcount_configName]);
           }
           catch (Exception ex)
           {
               ErrorLog.Instance.Write(ErrorLog.Level.Debug,
               "An error ocurred while getting EnableRowCountForDatabaseAuditing date from config file", ex);
           }
          return isEnabledRowCountEnabled;
       }


        //-----------------------------------------------------------
        // AddCommonColumns
        //-----------------------------------------------------------
        /// <summary>
        /// Add columns common to all traces.
        /// </summary>
        /// <param name="trace"></param>
        protected void AddCommonColumns(TraceConfiguration trace)
        {
            foreach (TraceColumnId id in EventHelper.TraceCommonColumns)
                trace.AddColumn(id);

            if (this.sqlVersion >= 9)
            {
                foreach (TraceColumnId id in EventHelper.TraceCommon2005Columns)
                    trace.AddColumn(id);
            }
        }

        //-----------------------------------------------------------
        // AddDBIdFilters
        //-----------------------------------------------------------
        /// <summary>
        /// Add database ID filters
        /// </summary>
        /// <param name="trace"></param>
        /// <param name="dbIdList"></param>
        protected void AddDBIdFilters(
           TraceConfiguration trace,
           int[] dbIdList
           )
        {

            if (dbIdList != null)
            {
                for (int i = 0; i < dbIdList.Length; i++)
                {
                    trace.AddFilter(new TraceFilter(TraceColumnId.DatabaseID,
                                                      TraceFilterComparisonOp.Equal,
                                                      dbIdList[i],
                                                      (i == 0) ? TraceFilterLogicalOp.AND
                                                                                  : TraceFilterLogicalOp.OR));
                }
            }
        }

        //5.4 XE
        protected void AddDBIdFilters(
         XeTraceConfiguration trace,
         int[] dbIdList
         )
        {

            if (dbIdList != null)
            {
                for (int i = 0; i < dbIdList.Length; i++)
                {
                    trace.AddFilter(new TraceFilter(TraceColumnId.DatabaseID,
                                                      TraceFilterComparisonOp.Equal,
                                                      dbIdList[i],
                                                      (i == 0) ? TraceFilterLogicalOp.AND
                                                                                  : TraceFilterLogicalOp.OR));
                }
            }
        }

        //-----------------------------------------------------------
        // AddObjectFilters
        //-----------------------------------------------------------
        /// <summary>
        /// Add object filters
        /// </summary>
        /// <param name="trace"></param>
        /// <param name="objects"></param>
        protected void AddObjectFilters(
           TraceConfiguration trace,
           int[] objects)
        {
            if (objects != null)
            {
                for (int i = 0; i < objects.Length; i++)
                {
                    trace.AddFilter(new TraceFilter(TraceColumnId.ObjectID,
                                                      TraceFilterComparisonOp.Equal,
                                                      objects[i],
                       (i == 0) ? TraceFilterLogicalOp.AND
                                               : TraceFilterLogicalOp.OR));
                }
            }
        }




        //-----------------------------------------------------------
        // CreateConfigurationSets
        //-----------------------------------------------------------
        /// <summary>
        /// Group similar database audit configuations into sets to
        /// reduce the number of traces.
        /// </summary>
        /// <param name="dbList"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        protected ArrayList
           CreateConfigurationSets(
              AuditConfiguration[] dbList,
              TraceCategory category
           )
        {

            AuditConfiguration matchedConfig = null;
            ArrayList uniqueList = new ArrayList();

            if (dbList == null ||
               dbList.Length == 0)
            {
                return null;
            }

            foreach (DBAuditConfiguration db in dbList)
            {
                foreach (AuditConfiguration processedConfig in uniqueList)
                {
                    if (HasSameConfiguration(processedConfig, db, category))
                    {
                        matchedConfig = processedConfig;
                        break;
                    }
                }

                if (matchedConfig != null)
                {
                    DBAuditConfiguration dbConfig = ((DBAuditConfiguration)matchedConfig);
                    dbConfig.AddDBId(db.DBId);
                    dbConfig.AddDBName(db.Name);
                    dbConfig.AddAuditedObjects(db.AuditObjectList);
                    matchedConfig = null;
                }
                else
                {
                    uniqueList.Add(db);
                    db.ClearDBIdList();
                    db.ClearDBNameList();
                    db.AddDBId(db.DBId);
                    db.AddDBName(db.Name);
                }
            }

            return uniqueList;
        }

        //-----------------------------------------------------------
        // MergeTraces - merge a set of traces into one
        // Note: there is no comparison of the two trace configurations
        //-----------------------------------------------------------
        protected TraceConfiguration[]
           MergeTraces(
              ArrayList traces
           )
        {
            if (traces == null)
                return null;

            if (traces.Count == 0)
                return (TraceConfiguration[])traces.ToArray(typeof(TraceConfiguration));

            TraceConfiguration tmpTrace;
            TraceConfiguration newTrace = (TraceConfiguration)traces[0];
            TraceColumnId[] columns;
            TraceEventId[] events;
            TraceFilter[] filters;

            for (int i = 1; i < traces.Count; i++)
            {
                tmpTrace = (TraceConfiguration)traces[i];

                events = tmpTrace.GetTraceEvents();
                for (int j = 0; j < events.Length; j++)
                    newTrace.AddEvent(events[j]);

                columns = tmpTrace.GetTraceColumns();
                for (int j = 0; j < columns.Length; j++)
                    newTrace.AddColumn(columns[j]);

                filters = tmpTrace.GetTraceFilters();
                for (int j = 0; j < filters.Length; j++)
                    newTrace.AddFilter(filters[j]);

            }

            int lastTrace = traces.Count - 1;
            for (int i = lastTrace; i > 0; i--)
                traces.RemoveAt(i);


            return (TraceConfiguration[])traces.ToArray(typeof(TraceConfiguration));

        }

        private bool CompareUsers(string[] users1, string[] users2)
        {
            if (users1 != null ||
                users2 != null)
            {

                if ((users1 == null && users2 != null) ||
                     (users2 == null && users1 != null) ||
                      users1.Length != users2.Length)
                {
                    return false;
                }


                for (int index = 0; index < users1.Length; index++)
                {
                    if (users1[index] != users2[index])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool CompareRoles(int[] roles1, int[] roles2)
        {
            if (roles1 != null ||
                roles2 != null)
            {

                if ((roles1 == null && roles2 != null) ||
                     (roles2 == null && roles1 != null) ||
                      roles1.Length != roles2.Length)
                {
                    return false;
                }


                for (int index = 0; index < roles1.Length; index++)
                {
                    if (roles1[index] != roles2[index])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        //-----------------------------------------------------------
        // HasSameConfiguration
        //-----------------------------------------------------------
        /// <summary>
        /// Compares two audit configurations for a specified category.
        /// </summary>
        /// <param name="config1"></param>
        /// <param name="config2"></param>
        /// <param name="category"></param>
        protected bool
           HasSameConfiguration(
              AuditConfiguration config1,
              AuditConfiguration config2,
              TraceCategory category
           )
        {
            if (config1.AuditCaptureDetails != config2.AuditCaptureDetails)
                return false;

            if (config1.AuditCaptureTransactions != config2.AuditCaptureTransactions)
                return false;

            if ((config1 is DBAuditConfiguration &&
                 !(config2 is DBAuditConfiguration)) ||
                (!(config1 is DBAuditConfiguration) &&
                  config2 is DBAuditConfiguration))
                return false;

            bool returnValue = false;

            switch (category)
            {
                case TraceCategory.DBSecurity:
                    if (config1.AuditSecurity == config2.AuditSecurity &&
                       config1.AuditDDL == config2.AuditDDL &&
                       config1.AuditAdmin == config2.AuditAdmin)
                        returnValue = true;
                    break;

                case TraceCategory.ServerTrace:
                    if (config1.AuditLogins == config2.AuditLogins &&
                        config1.AuditLogouts == config2.AuditLogouts &&
                       config1.AuditFailedLogins == config2.AuditFailedLogins &&
                       config1.AuditSecurity == config2.AuditSecurity)
                        returnValue = true;
                    break;

                case TraceCategory.DMLwithDetails:
                case TraceCategory.DML:
                    if (config1.AuditObjectList.Length != 0 ||
                        config2.AuditObjectList.Length != 0 ||
                        config1.AuditCaptureDetails != config2.AuditCaptureDetails ||
                        config1.AuditCaptureTransactions != config2.AuditCaptureTransactions)
                        return false;
                    else
                    {
                        DBAuditConfiguration dbc1 = (DBAuditConfiguration)config1;
                        DBAuditConfiguration dbc2 = (DBAuditConfiguration)config2;
                        if (dbc1.AuditDML != dbc2.AuditDML)  // DML settings must match
                            return false;

                        //They are only considerd the same if neither is doing BAD auditing.
                        if ((dbc1.DataChangeTables != null && dbc1.DataChangeTables.Length > 0) ||
                            (dbc2.DataChangeTables != null && dbc2.DataChangeTables.Length > 0))
                            return false;
                        returnValue = true;
                    }
                    break;
                case TraceCategory.DMLwithSELECT:
                    if (config1.AuditDML == config2.AuditDML)
                    {
                        if (config1.AuditObjectList.Length != 0 ||
                            config2.AuditObjectList.Length != 0)
                            return false;
                        else if (!config1.AuditCaptureDetails && !config2.AuditCaptureDetails)
                            returnValue = (config1.AuditSELECT == config2.AuditSELECT);
                        else if (config1.AuditCaptureDetails != config2.AuditCaptureDetails)
                            returnValue = false;
                        else if (config1.AuditCaptureTransactions != config2.AuditCaptureTransactions)
                            returnValue = false;
                        else
                        {
                            DBAuditConfiguration dbc1 = (DBAuditConfiguration)config1;
                            DBAuditConfiguration dbc2 = (DBAuditConfiguration)config2;
                            if ((dbc1.DataChangeTables != null && dbc1.DataChangeTables.Length > 0) ||
                                (dbc2.DataChangeTables != null && dbc2.DataChangeTables.Length > 0))
                                return false;
                            returnValue = true;
                        }
                    }
                    break;

                case TraceCategory.SELECTwithDetails:
                case TraceCategory.SELECT:
                    if (config1.AuditObjectList.Length != 0 ||
                        config2.AuditObjectList.Length != 0)
                        return false;
                    else if (config1.AuditSELECT == config2.AuditSELECT &&
                       config1.AuditCaptureDetails == config2.AuditCaptureDetails)
                        returnValue = true;
                    else
                        returnValue = false;
                    break;

                case TraceCategory.DBPrivilegedUsers:
                    DBAuditConfiguration dbConf1 = (DBAuditConfiguration)config1;
                    DBAuditConfiguration dbConf2 = (DBAuditConfiguration)config2;
                    if (dbConf1.AuditUserAccessCheck != dbConf2.AuditUserAccessCheck)
                        return false;

                    if (dbConf1.AuditUserCaptureSql != dbConf2.AuditUserCaptureSql)
                        return false;

                    if (dbConf1.AuditUserCaptureTransactions != dbConf2.AuditUserCaptureTransactions)
                        return false;

                    //They are only considered the same if neither is doing Sensitive Column auditing.
                    if ((dbConf1.SensitiveColumns != null && dbConf1.SensitiveColumns.Length > 0) ||
                        (dbConf2.SensitiveColumns != null && dbConf2.SensitiveColumns.Length > 0))
                        return false;

                    if (!CompareUsers(dbConf1.PrivUsers, dbConf2.PrivUsers))
                    {
                        return false;
                    }

                    if (!CompareRoles(dbConf1.PrivServerRoles, dbConf2.PrivServerRoles))
                    {
                        return false;
                    }

                    returnValue = true;
                    break;
                default:
                    returnValue = false;
                    break;
            }

            if (returnValue && config1 is DBAuditConfiguration)
                returnValue = HasSameDBLevelConfiguration((DBAuditConfiguration)config1,
                                                           (DBAuditConfiguration)config2);

            return returnValue;
        }


        //-----------------------------------------------------------
        // HasSameDBLevelConfiguration
        //-----------------------------------------------------------
        private bool
           HasSameDBLevelConfiguration(
              DBAuditConfiguration config1,
              DBAuditConfiguration config2)
        {
            if (config1.AuditAccessCheck != config2.AuditAccessCheck)
                return false;

            if (config1.AuditCaptureDetails != config2.AuditCaptureDetails)
                return false;

            if (config1.AuditCaptureTransactions != config2.AuditCaptureTransactions)
                return false;

            //They are only considerd the same if neither is doing Sensitive Column auditing.
            if ((config1.SensitiveColumns != null && config1.SensitiveColumns.Length > 0) ||
                (config2.SensitiveColumns != null && config2.SensitiveColumns.Length > 0))
                return false;


            // Compare audited object types
            DBObjectType[] type1 = config1.AuditObjectTypeList;
            DBObjectType[] type2 = config2.AuditObjectTypeList;

            if (type1.Length != type2.Length)
                return false;

            for (int i = 0; i < type1.Length; i++)
                if (type1[i] != type2[i])
                    return false;

            // Configurations with no audited user tables should not
            // merge with the ones with selected user tables.
            if ((config1.AuditObjectList.Length == 0 &&
                  config2.AuditObjectList.Length > 0) ||
                (config1.AuditObjectList.Length > 0 &&
                  config2.AuditObjectList.Length == 0))
                return false;

            if (!CompareUsers(config1.AuditedUsers, config2.AuditedUsers))
            {
                return false;
            }

            if (!CompareRoles(config1.AuditedServerRoles, config2.AuditedServerRoles))
            {
                return false;
            }

            return true;
        }

        /* Not being used anymore.
        //-----------------------------------------------------------
        // AddSQLsecureDBFilters
        //-----------------------------------------------------------
        /// <summary>
        /// Add SQLsecure database filters to a trace.
        /// </summary>
        /// <param name="trace"></param>
        private void 
           AddSQLsecureDBFilters (
              TraceConfiguration   trace
           )
        {
           if( trace == null )
              return;

           TraceFilter filter;

           try
           {
              foreach( int dbId in sqlsecureDBIds )
              {
                 filter = new TraceFilter( TraceColumnId.DatabaseID,
                    TraceFilterComparisonOp.NotEqual,
                    dbId,
                    TraceFilterLogicalOp.AND );
                 trace.AddFilter( filter );
              }
           }
           catch( Exception e )
           {
              ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                 "Error adding SQLsecure database filters",
                 e );
           }
        }
        */


        //-----------------------------------------------------------
        // AddSQLsecureApplicationFilters
        //-----------------------------------------------------------
        /// <summary>
        /// Add SQLsecure application name filters to a trace.
        /// </summary>
        /// <param name="trace"></param>
        private void
           AddSQLsecureApplicationFilters(
              TraceConfiguration trace
           )
        {
            if (trace == null)
                return;

            TraceFilter filter;

            try
            {
                // Filter for agent and collection service connections
                filter = new TraceFilter(TraceColumnId.ApplicationName,
                                           TraceFilterComparisonOp.NotLike,
                                           CoreConstants.DefaultSqlApplicationName,
                                           TraceFilterLogicalOp.AND);
                trace.AddFilter(filter);
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                   "Error adding SQL Compliance Manager application filters",
                   e);
            }
        }

        //5.4 XE
        private void
         AddSQLsecureApplicationFilters(
            XeTraceConfiguration trace
         )
        {
            if (trace == null)
                return;

            TraceFilter filter;

            try
            {
                // Filter for agent and collection service connections
                filter = new TraceFilter(TraceColumnId.ApplicationName,
                                           TraceFilterComparisonOp.NotLike,
                                           CoreConstants.DefaultSqlApplicationName,
                                           TraceFilterLogicalOp.AND);
                trace.AddFilter(filter);
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                   "Error adding SQL Compliance Manager application filters",
                   e);
            }
        }

        //-----------------------------------------------------------
        // AddObjectTypeFilters
        //-----------------------------------------------------------
        /// <summary>
        /// Add object type filters to a trace.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="trace"></param>
        private void
           AddObjectTypeFilters(
              DBAuditConfiguration config,
              TraceConfiguration trace)
        {
            if (config.AuditObjectTypeList == null ||
                config.AuditObjectTypeList.Length == 0)
                return;

            //foreach( DBObjectType type in config.AuditObjectTypeList )
            DBObjectType[] list = config.AuditObjectTypeList;
            for (int i = 0; i < list.Length; i++)
            {
                trace.AddFilter(new TraceFilter(TraceColumnId.ObjectType,
                                                  TraceFilterComparisonOp.Equal,
                                                  (int)list[i],
                                                  (i == 0) ? TraceFilterLogicalOp.AND // use AND for the last filter
                                                                         : TraceFilterLogicalOp.OR));
            }

            // have 0 so we get SQLSTmt:Starting where this col is null (Marcus)
            /*
            trace.AddFilter( new TraceFilter( TraceColumnId.ObjectType,
                                                TraceFilterComparisonOp.Equal,
                                                0 , TraceFilterLogicalOp.OR ));
                                                */

        }

        //-----------------------------------------------------------
        // AddUserFilters
        //-----------------------------------------------------------
        private void
           AddUserFilters(
              TraceConfiguration trace
           )
        {

            if (auditUserList.Count > 0)
            {
                trace.privUsers = AuditedUsers;
                string[] users = AuditedUsers;
                TraceFilter userFilter;
                for (int i = 0; i < users.Length; i++)
                {
                    userFilter = new TraceFilter(TraceColumnId.SQLSecurityLoginName,
                                                  TraceFilterComparisonOp.Like,
                                                  users[i],
                       (i == 0) ? TraceFilterLogicalOp.AND    // use AND for the last filter
                                                 : TraceFilterLogicalOp.OR);
                    trace.AddFilter(userFilter);
                }
            }
        }

        private void
          AddUserFiltersXE(
             XeTraceConfiguration trace
          )
        {

            if (auditUserList.Count > 0)
            {
                trace.privUsers = AuditedUsers;
                string[] users = AuditedUsers;
                TraceFilter userFilter;
                for (int i = 0; i < users.Length; i++)
                {
                    userFilter = new TraceFilter(TraceColumnId.SQLSecurityLoginName,
                                                  TraceFilterComparisonOp.Like,
                                                  users[i],
                       (i == 0) ? TraceFilterLogicalOp.AND    // use AND for the last filter
                                                 : TraceFilterLogicalOp.OR);
                    trace.AddFilter(userFilter);
                }
            }
        }

        //-----------------------------------------------------------
        // AddTrustedUserServerLevelFilters //v5.6 SQLCM-5373
        //-----------------------------------------------------------
        private void
           AddTrustedUserServerLevelFilters(
              TraceConfiguration trace,
              string[] users
           )
        {

            if (users != null && users.Length > 0)
            {
                //trace.privUsers = users;
                TraceFilter userFilter;
                for (int i = 0; i < users.Length; i++)
                {
                    userFilter = new TraceFilter(TraceColumnId.SQLSecurityLoginName,
                                                  TraceFilterComparisonOp.NotLike,
                                                  users[i],
                                                  TraceFilterLogicalOp.AND);
                    trace.AddFilter(userFilter);
                }

                if (sqlVersion > 8)
                {
                    for (int i = 0; i < users.Length; i++)
                    {
                        userFilter = new TraceFilter(TraceColumnId.SessionLoginName,
                                                      TraceFilterComparisonOp.NotLike,
                                                      users[i],
                                                      TraceFilterLogicalOp.AND);
                        trace.AddFilter(userFilter);
                    }
                }
            }
        }

        //-----------------------------------------------------------
        // AddTrustedUserFilters
        //-----------------------------------------------------------
        private void
           AddTrustedUserFilters(
              TraceConfiguration trace,
              string[] users
           )
        {

            if (users != null && users.Length > 0)
            {
                trace.privUsers = users;
                TraceFilter userFilter;
                for (int i = 0; i < users.Length; i++)
                {
                    userFilter = new TraceFilter(TraceColumnId.SQLSecurityLoginName,
                                                  TraceFilterComparisonOp.NotLike,
                                                  users[i],
                                                  TraceFilterLogicalOp.AND);
                    trace.AddFilter(userFilter);
                }

                if (sqlVersion > 8)
                {
                    for (int i = 0; i < users.Length; i++)
                    {
                        userFilter = new TraceFilter(TraceColumnId.SessionLoginName,
                                                      TraceFilterComparisonOp.NotLike,
                                                      users[i],
                                                      TraceFilterLogicalOp.AND);
                        trace.AddFilter(userFilter);
                    }
                }
            }
        }

        //-----------------------------------------------------------
        // AddTrustedUserFilters
        //-----------------------------------------------------------
        private void
           AddTrustedUserFiltersEx(
              TraceConfiguration trace,
              string[] users,
              List<string> excludeUsers
           )
        {

            if (excludeUsers == null)
                excludeUsers = new List<string>();
            if (users != null && users.Length > 0)
            {
                trace.privUsers = users;
                TraceFilter userFilter;
                for (int i = 0; i < users.Length; i++)
                {
                    if (!excludeUsers.Contains(users[i]))
                    {
                        userFilter = new TraceFilter(TraceColumnId.SQLSecurityLoginName,
                                                      TraceFilterComparisonOp.NotLike,
                                                      users[i],
                                                      TraceFilterLogicalOp.AND);
                        trace.AddFilter(userFilter);
                    }
                }

                if (sqlVersion > 8)
                {
                    for (int i = 0; i < users.Length; i++)
                    {
                        if (!excludeUsers.Contains(users[i]))
                        {
                            userFilter = new TraceFilter(TraceColumnId.SessionLoginName,
                                                          TraceFilterComparisonOp.NotLike,
                                                          users[i],
                                                          TraceFilterLogicalOp.AND);
                            trace.AddFilter(userFilter);
                        }
                    }
                }
            }
        }

        //5.4 XE
        private void
           AddTrustedUserFiltersEx(
              XeTraceConfiguration trace,
              string[] users,
              List<string> excludeUsers
           )
        {

            if (excludeUsers == null)
                excludeUsers = new List<string>();
            if (users != null && users.Length > 0)
            {
                trace.privUsers = users;
                TraceFilter userFilter;
                for (int i = 0; i < users.Length; i++)
                {
                    if (!excludeUsers.Contains(users[i]))
                    {
                        userFilter = new TraceFilter(TraceColumnId.SQLSecurityLoginName,
                                                      TraceFilterComparisonOp.NotLike,
                                                      users[i],
                                                      TraceFilterLogicalOp.AND);
                        trace.AddFilter(userFilter);
                    }
                }

                if (sqlVersion > 8)
                {
                    for (int i = 0; i < users.Length; i++)
                    {
                        if (!excludeUsers.Contains(users[i]))
                        {
                            userFilter = new TraceFilter(TraceColumnId.SessionLoginName,
                                                          TraceFilterComparisonOp.NotLike,
                                                          users[i],
                                                          TraceFilterLogicalOp.AND);
                            trace.AddFilter(userFilter);
                        }
                    }
                }
            }
        }

        #endregion		

        #endregion
		
        #region Audit Logs
        //-----------------------------------------------------------
        // GenerateAuditLogConfigurations
        //-----------------------------------------------------------
        /// <summary>
        /// Generate Audit Logs configurations for the this audit configuration.
        /// </summary>
        /// <returns></returns>
        public virtual AuditLogConfiguration[] GenerateAuditLogConfigurations()
        {
            ArrayList configList = new ArrayList();

            AuditLogConfiguration[] auditLogs = GenerateDMLAuditLogs(false);
            if (auditLogs != null)
                foreach (AuditLogConfiguration trace in auditLogs)
                    configList.Add(trace);

            auditLogs = GenerateSELECTAuditLogs(false);
            if (auditLogs != null)
                foreach (AuditLogConfiguration trace in auditLogs)
                    configList.Add(trace);

            auditLogs = GeneratePrivilegedUserAuditLogs();
            if (auditLogs != null)
                foreach (AuditLogConfiguration trace in auditLogs)
                    configList.Add(trace);

            auditLogs = GenerateDBPrivilegedUserAuditLogs();
            if (auditLogs != null)
                foreach (AuditLogConfiguration trace in auditLogs)
                    configList.Add(trace);

            return (AuditLogConfiguration[])configList.ToArray(typeof(AuditLogConfiguration));
        }

        //-----------------------------------------------------------
        // GenerateDMLAuditLogs
        //-----------------------------------------------------------
        /// <summary>
        /// Generate database level DML traces.
        /// </summary>
        /// <returns></returns>
        protected virtual AuditLogConfiguration[]
           GenerateDMLAuditLogs(
              bool privilegedUsers
           )
        {
            if (!this.IsAuditLogsEnabled || this.AuditCaptureSQLXE)
                return null;
            ArrayList auditLogs = new ArrayList();
            int sequence = 0;

            ArrayList auditLogSets = CreateConfigurationSets(AuditDBList, TraceCategory.DML);
            if (auditLogSets == null)
                return null;

            AuditLogConfiguration auditLog;
            List<string> pUsers = privilegedUsers ? null : ConfigurationHelper.GetPrivUsers(this);
            foreach (AuditConfiguration config in auditLogSets)
            {
                if (config == null)
                    continue;
                auditLog = GenerateDMLAuditLog(config, privilegedUsers, sequence, pUsers);
                if (auditLog != null)
                {
                    sequence++;
                    auditLogs.Add(auditLog);
                } 
            }
            return (AuditLogConfiguration[])auditLogs.ToArray(typeof(AuditLogConfiguration));
        }

        //-----------------------------------------------------------
        // GenerateDMLAuditLog
        //-----------------------------------------------------------
        /// <summary>
        /// Generate Audit Log configuration for DML statement events.
        /// </summary>
        /// <returns></returns>
        protected virtual AuditLogConfiguration
           GenerateDMLAuditLog(
              AuditConfiguration config,
              bool privilegedUsers,
              int sequence,
              List<string> excludeUsers
           )
        {
            if (!config.AuditDML)
                return null;

            AuditLogConfiguration auditLog = new AuditLogConfiguration();
            if (privilegedUsers)
            {
                auditLog.Level = TraceLevel.User;
            }
            else
                auditLog.Level = TraceLevel.Database;
            auditLog.Sequence = sequence;

            auditLog.AddEvent(AuditLogEventID.SCHEMA_OBJECT_ACCESS_GROUP);
            if (config.AuditUserCaptureTransactions || config.AuditCaptureTransactions)
            {
                auditLog.AddEvent(AuditLogEventID.TRANSACTION_GROUP);
                auditLog.AddFilter(new TraceFilter(AuditLogColumnId.ActionID,
                                                    TraceFilterComparisonOpXE.Equal,
                                                    AuditLogActionID.TransactionBegin,         
                                                    AuditLogFilterLogicalOp.OR));
                auditLog.AddFilter(new TraceFilter(AuditLogColumnId.ActionID,
                                                   TraceFilterComparisonOpXE.Equal,
                                                   AuditLogActionID.TransactionCommit,
                                                   AuditLogFilterLogicalOp.OR));
                auditLog.AddFilter(new TraceFilter(AuditLogColumnId.ActionID,
                                                   TraceFilterComparisonOpXE.Equal,
                                                   AuditLogActionID.TransactionRollback,
                                                   AuditLogFilterLogicalOp.OR));
            }

            auditLog.AddFilter(new TraceFilter(AuditLogColumnId.ActionID,
                                                TraceFilterComparisonOpXE.Equal,
                                                AuditLogActionID.Update,         // Update
                                                AuditLogFilterLogicalOp.OR));
            auditLog.AddFilter(new TraceFilter(AuditLogColumnId.ActionID,
                                                TraceFilterComparisonOpXE.Equal,
                                                AuditLogActionID.Insert,         // INSERT
                                                AuditLogFilterLogicalOp.OR));
            auditLog.AddFilter(new TraceFilter(AuditLogColumnId.ActionID,
                                                TraceFilterComparisonOpXE.Equal,
                                                AuditLogActionID.Delete,         // DELETE
                                                AuditLogFilterLogicalOp.OR));
            auditLog.AddFilter(new TraceFilter(AuditLogColumnId.ActionID,
                                                TraceFilterComparisonOpXE.Equal,
                                                AuditLogActionID.Execute,         // EXECUTE
                                                AuditLogFilterLogicalOp.OR));
            if (config.AuditCaptureDetails)
            {
                auditLog.KeepSQL = true;                
                auditLog.Category = TraceCategory.DMLwithDetails;
            }
            else
            {
                auditLog.Category = TraceCategory.DML;
            }

            if(privilegedUsers)
                auditLog.Category = TraceCategory.ServerTrace;

            if (config is DBAuditConfiguration && !privilegedUsers)
            {
                AddObjectTypeFiltersAuditLog((DBAuditConfiguration)config, auditLog);
                AddObjectFilters(auditLog, config.AuditObjectList);
                //Datachangefilter
                DBAuditConfiguration dbConfig = (DBAuditConfiguration)config;
                if (dbConfig.DataChangeTables != null &&
                     dbConfig.DataChangeTables.Length > 0)
                {
                    if (config.AuditCaptureDetails)
                        auditLog.Category = TraceCategory.DataChangeWithDetails;
                    else
                        auditLog.Category = TraceCategory.DataChange;

                    auditLog.AddFilter(new TraceFilter(AuditLogColumnId.ParentName,
                                                      TraceFilterComparisonOpXE.Equal,
                                                      CoreConstants.Agent_BeforeAfter_SchemaName,
                                                      AuditLogFilterLogicalOp.AND));

                    auditLog.AddFilter(new TraceFilter(AuditLogColumnId.ObjectName,
                                                        TraceFilterComparisonOpXE.Equal,
                                                        CoreConstants.Agent_BeforeAfter_TableName,
                                                        AuditLogFilterLogicalOp.AND));
                }
                else
                {
                    AddDataChangeEventFilter(auditLog);
                }
                // Add Database ID filters if any
                AddDBNameFilters(auditLog, ((DBAuditConfiguration)config).DBNameList);
            }
            else
            {
                AddDataChangeEventFilter(auditLog);
            }

            // Filter out trusted users and SQLCM activities
            if (!privilegedUsers)
            {
                if (!isServer && config is DBAuditConfiguration && config.AuditedUsers.Length > 0)
                {
                    AddTrustedUserFiltersEx(auditLog, config.AuditedUsers, excludeUsers);
                }
            }

            AddSQLCMApplicationFilters(auditLog);
            auditLog.InstanceAlias = this.InstanceAlias;
            auditLog.Version = this.Version;
            auditLog.FileName = Path.Combine(traceDirectory,
                                          TraceFileNameAttributes.ComposeAuditLogFileName(auditLog.InstanceAlias,
                                                                                 (int)auditLog.Level,
                                                                                 (int)auditLog.Category,
                                                                                 auditLog.Version,
                                                                                 sequence));

            return auditLog;

        }        

        //-----------------------------------------------------------
        // GenerateSELECTAuditLogs
        //-----------------------------------------------------------
        /// <summary>
        /// Generate database level DML Audit Logs.
        /// </summary>
        /// <returns></returns>
        protected virtual AuditLogConfiguration[]
           GenerateSELECTAuditLogs(
              bool privilegedUsers
           )
        {

            if (this.AuditCaptureSQLXE || !this.IsAuditLogsEnabled)
                return null;
            ArrayList auditLogs = new ArrayList();
            int sequence = 0;
            int scSequence = 0;
            ArrayList auditLogSets = CreateConfigurationSets(AuditDBList, TraceCategory.SELECT);
            if (auditLogSets == null)
                return null;

            AuditLogConfiguration auditLog;
            foreach (AuditConfiguration config in auditLogSets)
            {
                if (config == null)
                    continue;

                auditLog = GenerateSELECTAuditLog(config, privilegedUsers, sequence);
                if (auditLog != null)
                {
                    sequence++;
                    auditLogs.Add(auditLog);
                }

                if (!privilegedUsers)
                {
                    List<string> pUsers = privilegedUsers ? null : ConfigurationHelper.GetPrivUsers(this);
                    DBAuditConfiguration dbConfig = (DBAuditConfiguration)config;
                    if (dbConfig.SensitiveColumns != null &&
                         dbConfig.SensitiveColumns.Length > 0)
                    {
                        scSequence++;
                        auditLog = GenerateSensitiveColumnAuditLog(dbConfig, scSequence, pUsers);
                        auditLogs.Add(auditLog);
                    }
                }
            }
            return (AuditLogConfiguration[])auditLogs.ToArray(typeof(AuditLogConfiguration));
        }


        //-----------------------------------------------------------
        // GenerateSELECTAuditLog
        //-----------------------------------------------------------
        /// <summary>
        /// Generate trace configuration for SELECT events.
        /// </summary>
        /// <returns></returns>
        protected virtual AuditLogConfiguration
           GenerateSELECTAuditLog(
              AuditConfiguration config,
              bool privilegedUsers,
              int sequence
           )
        {
            if (!config.AuditSELECT)
                return null;

            AuditLogConfiguration auditLog = new AuditLogConfiguration();
            if (privilegedUsers)
            {
                auditLog.Level = TraceLevel.User;
            }
            else
                auditLog.Level = TraceLevel.Database;
            auditLog.Sequence = sequence;

            auditLog.AddEvent(AuditLogEventID.SCHEMA_OBJECT_ACCESS_GROUP);

            auditLog.AddFilter(new TraceFilter(AuditLogColumnId.ActionID,
                                            TraceFilterComparisonOpXE.Equal,
                                            AuditLogActionID.Select,         // SELECT
                                            AuditLogFilterLogicalOp.OR));

            auditLog.AddFilter(new TraceFilter(AuditLogColumnId.ActionID,
                                             TraceFilterComparisonOpXE.Equal,
                                             AuditLogActionID.References,         // REFERENCE ALL
                                             AuditLogFilterLogicalOp.OR));

            if (config.AuditCaptureDetails)
            {
                auditLog.KeepSQL = true;
            }
            else
            {
                auditLog.Category = TraceCategory.SELECT;
                auditLog.KeepSQL = false;
            }

            if (config is DBAuditConfiguration)
            {
                AddObjectTypeFiltersAuditLog((DBAuditConfiguration)config, auditLog);
                //if (isServer)
                AddObjectFilters(auditLog, config.AuditObjectList);
                AddDBNameFilters(auditLog, ((DBAuditConfiguration)config).DBNameList);
            }

            if (!privilegedUsers)
            {
                if (!isServer && config is DBAuditConfiguration && config.AuditedUsers.Length > 0)
                {
                    AddTrustedUserFilters(auditLog, config.AuditedUsers);
                }

                AddDataChangeEventFilter(auditLog);

            }

            AddSQLCMApplicationFilters(auditLog);
            auditLog.InstanceAlias = this.InstanceAlias;
            auditLog.Category = TraceCategory.SELECTwithDetails;
            auditLog.Version = this.Version;
            auditLog.FileName = Path.Combine(traceDirectory,
                                 TraceFileNameAttributes.ComposeAuditLogFileName(auditLog.InstanceAlias,
                                                                              (int)auditLog.Level,
                                                                              (int)auditLog.Category,
                                                                              auditLog.Version,
                                                                              sequence));
            return auditLog;

        }

        //-----------------------------------------------------------
        // GeneratePrivilegedUserAuditLogs
        //-----------------------------------------------------------
        /// <summary>
        /// Generate Audit Logs for auditing activities from users in fixed system roles.
        /// </summary>
        /// <returns></returns>
        protected virtual AuditLogConfiguration[]
           GeneratePrivilegedUserAuditLogs()
        {
            if (!this.isAuditLogsEnabled
                || this.AuditCaptureSQLXE
                || this.UserConfiguration == null)
                return null;

            // Check if there are server roles selected
            if (!(auditServerRoleList != null ||
                   auditUserList != null ||
                   auditServerRoleList.Count > 0 ||
                   auditUserList.Count > 0))
                return null;

            ArrayList configList = new ArrayList();

            if (this.UserConfiguration.AuditDML)
            {
                configList.Add(GenerateDMLAuditLog(this.UserConfiguration, true, 0, null));
            }

            if (this.UserConfiguration.AuditSELECT)
            {
                configList.Add(GenerateSELECTAuditLog(this.UserConfiguration, true, 0));
            }


            AuditLogConfiguration[] userAuditLogs = MergeAuditLogs(configList);


            foreach (AuditLogConfiguration config in userAuditLogs)
            {
                config.KeepSQL = this.UserConfiguration.AuditCaptureDetails;
                config.KeepAdminSQL = this.userConfiguration.AuditAdmin;
            }

            return userAuditLogs;

        }

        //-----------------------------------------------------------
        // GenerateDBPrivilegedUserAuditLogs
        //-----------------------------------------------------------
        /// <summary>
        /// Generate traces for auditing activities from users in fixed system roles.
        /// </summary>
        /// <returns></returns>
        protected virtual AuditLogConfiguration[]
           GenerateDBPrivilegedUserAuditLogs()
        {
            if (!this.isAuditLogsEnabled
                || this.AuditCaptureSQLXE)
                return null;
            ArrayList auditLogSets = CreateConfigurationSets(AuditDBList, TraceCategory.DBPrivilegedUsers);
            if (auditLogSets == null)
                return null;
            int dmlSequence = 0;
            int selectSequence = 0;

            ArrayList auditLogs = new ArrayList();
            ArrayList dbAuditLogs = new ArrayList();
            AuditLogConfiguration dmlAuditLog;
            AuditLogConfiguration selectAuditLog;
            foreach (DBAuditConfiguration config in auditLogSets)
            {
                for (int i = auditLogs.Count - 1; i >= 0; i--)
                    auditLogs.RemoveAt(i);
                if (!((config.PrivServerRoles != null && config.PrivServerRoles.Length > 0)
                    || (config.PrivUsers != null && config.PrivUsers.Length > 0)))
                    continue;
                int auditDML = Array.FindIndex(config.UserCategories, item => item == AuditCategory.DML);
                int auditSELECT = Array.FindIndex(config.UserCategories, item => item == AuditCategory.SELECT);

                if (auditDML != -1)
                {
                    dmlAuditLog = GenerateDBPrivilegedDMLAuditLog(config, dmlSequence, null);
                    if (dmlAuditLog != null)
                    {
                        dmlSequence++;
                        auditLogs.Add(dmlAuditLog);
                    }
                }

                if (auditSELECT != -1)
                {
                    selectAuditLog = GenerateDBPrivilegedSELECTAuditLog(config, selectSequence);
                    if (selectAuditLog != null)
                    {
                        selectSequence++;
                        auditLogs.Add(selectAuditLog);
                    }
                }
                AuditLogConfiguration[] userTraces = MergeAuditLogs(auditLogs);
                if (userTraces != null)
                    foreach (AuditLogConfiguration dbTrace in userTraces)
                        dbAuditLogs.Add(dbTrace);
            }
            return (AuditLogConfiguration[])dbAuditLogs.ToArray(typeof(AuditLogConfiguration));
        }

        //-----------------------------------------------------------
        // GenerateDBPrivilegedDMLAuditLog
        //-----------------------------------------------------------
        /// <summary>
        /// Generate Audit Log configuration for DML statement events.
        /// </summary>
        /// <returns></returns>
        protected virtual AuditLogConfiguration
           GenerateDBPrivilegedDMLAuditLog(
              DBAuditConfiguration config,
              int sequence,
              List<string> excludeUsers
           )
        {
            // SQLCM-5471 v5.6 Add Activity to Senstitive columns
            //If this is a DB config and both Select and sensitive columns are being audited, just return.
            // the sensitive columns trace will pick up the event.
            if (config.SensitiveColumns != null &&
                config.SensitiveColumns.Length > 0)
            {
                return null;
            }
            AuditLogConfiguration auditLog = new AuditLogConfiguration();
            auditLog.Level = TraceLevel.Database;
            auditLog.Sequence = sequence;

            
            auditLog.AddEvent(AuditLogEventID.SCHEMA_OBJECT_ACCESS_GROUP);
            if (config.UserCaptureTran)
            {
                auditLog.AddEvent(AuditLogEventID.TRANSACTION_GROUP);
                auditLog.AddFilter(new TraceFilter(AuditLogColumnId.ActionID,
                                                    TraceFilterComparisonOpXE.Equal,
                                                    AuditLogActionID.TransactionBegin,
                                                    AuditLogFilterLogicalOp.OR));
                auditLog.AddFilter(new TraceFilter(AuditLogColumnId.ActionID,
                                                   TraceFilterComparisonOpXE.Equal,
                                                   AuditLogActionID.TransactionCommit,
                                                   AuditLogFilterLogicalOp.OR));
                auditLog.AddFilter(new TraceFilter(AuditLogColumnId.ActionID,
                                                   TraceFilterComparisonOpXE.Equal,
                                                   AuditLogActionID.TransactionRollback,
                                                   AuditLogFilterLogicalOp.OR));
            }
            auditLog.AddFilter(new TraceFilter(AuditLogColumnId.ActionID,
                                            TraceFilterComparisonOpXE.Equal,
                                            AuditLogActionID.Update,         // Update
                                            AuditLogFilterLogicalOp.OR));
            auditLog.AddFilter(new TraceFilter(AuditLogColumnId.ActionID,
                                                TraceFilterComparisonOpXE.Equal,
                                                AuditLogActionID.Insert,         // INSERT
                                                AuditLogFilterLogicalOp.OR));
            auditLog.AddFilter(new TraceFilter(AuditLogColumnId.ActionID,
                                                TraceFilterComparisonOpXE.Equal,
                                                AuditLogActionID.Delete,         // DELETE
                                                AuditLogFilterLogicalOp.OR));
            auditLog.AddFilter(new TraceFilter(AuditLogColumnId.ActionID,
                                                TraceFilterComparisonOpXE.Equal,
                                                    AuditLogActionID.Execute,         // EXECUTE
                                                    AuditLogFilterLogicalOp.OR));
            if (config.AuditUserCaptureSql)
            {
                auditLog.KeepSQL = true;
            }

            AddDBNameFilters(auditLog, config.DBNameList);
            AddSQLCMApplicationFilters(auditLog);

            auditLog.InstanceAlias = this.InstanceAlias;
            auditLog.Category = TraceCategory.DBPrivilegedUsers;
            auditLog.Version = this.Version;
            auditLog.FileName = Path.Combine(traceDirectory,
                                          TraceFileNameAttributes.ComposeAuditLogFileName(auditLog.InstanceAlias,
                                                                                 (int)auditLog.Level,
                                                                                 (int)auditLog.Category,
                                                                                 auditLog.Version,
                                                                                 sequence));

            return auditLog;
        }

        //-----------------------------------------------------------
        // GenerateDBPrivilegedSELECTAuditLog
        //-----------------------------------------------------------
        /// <summary>
        /// Generate trace configuration for SELECT events.
        /// </summary>
        /// <returns></returns>
        protected virtual AuditLogConfiguration
           GenerateDBPrivilegedSELECTAuditLog(
              DBAuditConfiguration config,
              int sequence
           )
        {
            //If this is a DB config and both Select and sensitive columns are being audited, just return.
            // the sensitive columns trace will pick up the event.
            if (config.SensitiveColumns != null &&
                config.SensitiveColumns.Length > 0)
            {
                return null;
            }
            AuditLogConfiguration auditLog = new AuditLogConfiguration();
            auditLog.Level = TraceLevel.Database;
            auditLog.Sequence = sequence;

            auditLog.AddEvent(AuditLogEventID.SCHEMA_OBJECT_ACCESS_GROUP);
            auditLog.AddFilter(new TraceFilter(AuditLogColumnId.ActionID,
                                            TraceFilterComparisonOpXE.Equal,
                                            AuditLogActionID.Select,         // SELECT
                                            AuditLogFilterLogicalOp.OR));

            auditLog.AddFilter(new TraceFilter(AuditLogColumnId.ActionID,
                                                TraceFilterComparisonOpXE.Equal,
                                                AuditLogActionID.References,         // REFERENCE ALL
                                                AuditLogFilterLogicalOp.OR));
            if (config.AuditUserCaptureSql)
            {
                auditLog.KeepSQL = true;
            }
            else
            {
                auditLog.KeepSQL = false;
            }
            AddDBNameFilters(auditLog, config.DBNameList);
            AddDataChangeEventFilter(auditLog);
            AddSQLCMApplicationFilters(auditLog);

            auditLog.InstanceAlias = this.InstanceAlias;
            auditLog.Category = TraceCategory.DBPrivilegedUsers;
            auditLog.Version = this.Version;
            auditLog.FileName = Path.Combine(traceDirectory,
                                 TraceFileNameAttributes.ComposeAuditLogFileName(auditLog.InstanceAlias,
                                                                              (int)auditLog.Level,
                                                                              (int)auditLog.Category,
                                                                              auditLog.Version,
                                                                              sequence));
            return auditLog;
        }


        //-----------------------------------------------------------
        // GenerateSensitiveColumnAuditLog
        //-----------------------------------------------------------
        /// <summary>
        /// Generate trace configuration for SELECT events for Sensitive column access.
        /// Note that there is one trace for each instance.
        /// </summary>
        /// <returns></returns>
        protected virtual AuditLogConfiguration GenerateSensitiveColumnAuditLog(DBAuditConfiguration config,
                                                                          int sequence,
                                                                          List<string> pUsers)
        {
            AuditLogConfiguration auditLog = new AuditLogConfiguration();
            auditLog.Level = TraceLevel.Table;
            auditLog.Sequence = sequence;
            auditLog.KeepSQL = true;

            auditLog.AddEvent(AuditLogEventID.SCHEMA_OBJECT_ACCESS_GROUP);


            if (config.AuditSELECT)
                AddObjectTypeFiltersAuditLog((DBAuditConfiguration)config, auditLog);

            if (isServer)
            {
                auditLog.AddFilter(new TraceFilter(AuditLogColumnId.Permissions,
                                                    TraceFilterComparisonOpXE.Equal,
                                                    1,         // SELECT
                                                    AuditLogFilterLogicalOp.OR));
            }
            else
            {
                auditLog.AddFilter(new TraceFilter(AuditLogColumnId.ActionID,
                                                  TraceFilterComparisonOpXE.Equal,
                                                  AuditLogActionID.Select,
                                                  AuditLogFilterLogicalOp.AND));
            }

            auditLog.AddFilter(new TraceFilter(AuditLogColumnId.DatabaseName,
                                           TraceFilterComparisonOpXE.Equal,
                                           config.Name,
                                           AuditLogFilterLogicalOp.AND));


            int i = 0;
            foreach (TableConfiguration tableConfig in config.SensitiveColumns)
            {
                auditLog.AddFilter(new TraceFilter(AuditLogColumnId.ObjectName,
                                                        TraceFilterComparisonOpXE.Like,
                                                        tableConfig.Name,
                                                        (i == 0) ? AuditLogFilterLogicalOp.AND : AuditLogFilterLogicalOp.OR));
                i++;
            }

            if (!isServer && config.AuditedUsers.Length > 0)
            {
                AddTrustedUserFiltersEx(auditLog, config.AuditedUsers, pUsers);
            }
            AddSQLCMApplicationFilters(auditLog);
            auditLog.InstanceAlias = this.InstanceAlias;

            if (config.AuditSELECT)
                auditLog.Category = TraceCategory.SensitiveColumnwithSelect;
            else
                auditLog.Category = TraceCategory.SensitiveColumn;
            auditLog.Version = this.Version;
            auditLog.FileName = Path.Combine(traceDirectory, TraceFileNameAttributes.ComposeAuditLogFileName(auditLog.InstanceAlias,
                                                                                                      (int)auditLog.Level,
                                                                                                      (int)auditLog.Category,
                                                                                                      auditLog.Version,
                                                                                                      sequence));
            return auditLog;
        }

        //-----------------------------------------------------------
        // AddObjectTypeFiltersAuditLog
        //-----------------------------------------------------------
        /// <summary>
        /// Add object type filters to a AuditLog.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="auditLog"></param>
        private void
           AddObjectTypeFiltersAuditLog(
              DBAuditConfiguration config,
              AuditLogConfiguration auditLog)
        {
            if (config.AuditObjectTypeList == null ||
                config.AuditObjectTypeList.Length == 0)
                return;
            DBObjectType[] list = config.AuditObjectTypeList;
            for (int i = 0; i < list.Length; i++)
            {
                auditLog.AddFilter(new TraceFilter(AuditLogColumnId.ObjectType,
                                                  TraceFilterComparisonOpXE.Equal,
                                                  (int)list[i],
                                                  (i == 0) ? AuditLogFilterLogicalOp.AND // use AND for the last filter
                                                                         : AuditLogFilterLogicalOp.OR));
            }
        }

        //-----------------------------------------------------------
        // AddDBNameFilters
        //-----------------------------------------------------------
        /// <summary>
        /// Add database ID filters
        /// </summary>
        /// <param name="auditLog"></param>
        /// <param name="dbList"></param>
        protected void AddDBNameFilters(
           AuditLogConfiguration auditLog,
           string[] dbList
           )
        {

            if (dbList != null)
            {
                for (int i = 0; i < dbList.Length; i++)
                {
                    auditLog.AddFilter(new TraceFilter(AuditLogColumnId.DatabaseName,
                                                      TraceFilterComparisonOpXE.Like,
                                                      dbList[i],
                                                      (i == 0) ? AuditLogFilterLogicalOp.AND
                                                                                  : AuditLogFilterLogicalOp.OR));
                }
            }
        }

        //-----------------------------------------------------------
        // AddSQLCMApplicationFilters
        //-----------------------------------------------------------
        /// <summary>
        /// Add SQLCM application name filters to a trace.
        /// </summary>
        /// <param name="auditLog"></param>
        private void
           AddSQLCMApplicationFilters(
              AuditLogConfiguration auditLog
           )
        {
            if (auditLog == null)
                return;

            TraceFilter filter;

            try
            {
                // Filter for agent and collection service connections
                filter = new TraceFilter(AuditLogColumnId.ApplicationName,
                                           TraceFilterComparisonOpXE.NotLike,
                                           CoreConstants.DefaultSqlApplicationName,
                                           AuditLogFilterLogicalOp.AND);
                auditLog.AddFilter(filter);
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                   "Error adding SQL compliance manager application filters",
                   e);
            }
        }

        //-----------------------------------------------------------
        // AddTrustedUserFilters
        //-----------------------------------------------------------
        private void
           AddTrustedUserFiltersEx(
              AuditLogConfiguration auditLog,
              string[] users,
              List<string> excludeUsers
           )
        {

            if (excludeUsers == null)
                excludeUsers = new List<string>();
            if (users != null && users.Length > 0)
            {
                auditLog.privUsers = users;
                TraceFilter userFilter;
                for (int i = 0; i < users.Length; i++)
                {
                    if (!excludeUsers.Contains(users[i]))
                    {
                        userFilter = new TraceFilter(AuditLogColumnId.SQLSecurityLoginName,
                                                      TraceFilterComparisonOpXE.NotLike,
                                                      users[i],
                                                      AuditLogFilterLogicalOp.AND);
                        auditLog.AddFilter(userFilter);
                    }
                }

                if (sqlVersion > 8)
                {
                    for (int i = 0; i < users.Length; i++)
                    {
                        if (!excludeUsers.Contains(users[i]))
                        {
                            userFilter = new TraceFilter(AuditLogColumnId.SessionLoginName,
                                                          TraceFilterComparisonOpXE.NotLike,
                                                          users[i],
                                                          AuditLogFilterLogicalOp.AND);
                            auditLog.AddFilter(userFilter);
                        }
                    }
                }
            }
        }

        //-----------------------------------------------------------
        // AddObjectFilters
        //-----------------------------------------------------------
        /// <summary>
        /// Add object filters
        /// </summary>
        /// <param name="aduitLog"></param>
        /// <param name="objects"></param>
        protected void AddObjectFilters(
           AuditLogConfiguration aduitLog,
           int[] objects)
        {
            if (objects != null)
            {
                for (int i = 0; i < objects.Length; i++)
                {
                    aduitLog.AddFilter(new TraceFilter(AuditLogColumnId.ObjectID,
                                                      TraceFilterComparisonOpXE.Equal,
                                                      objects[i],
                       (i == 0) ? AuditLogFilterLogicalOp.AND
                                               : AuditLogFilterLogicalOp.OR));
                }
            }
        }

        protected void
           AddDataChangeEventFilter(AuditLogConfiguration auditLog)
        {
            // filter out DML trigger generated events
            auditLog.AddFilter(new TraceFilter(AuditLogColumnId.ParentName,
                                              TraceFilterComparisonOpXE.NotEqual,
                                              CoreConstants.Agent_BeforeAfter_SchemaName,
                                              AuditLogFilterLogicalOp.AND));

            auditLog.AddFilter(new TraceFilter(AuditLogColumnId.ObjectName,
                                              TraceFilterComparisonOpXE.NotEqual,
                                              CoreConstants.Agent_BeforeAfter_TableName,
                                              AuditLogFilterLogicalOp.AND));
        }

        //-----------------------------------------------------------
        // AddTrustedUserFilters
        //-----------------------------------------------------------
        private void
           AddTrustedUserFilters(
              AuditLogConfiguration trace,
              string[] users
           )
        {

            if (users != null && users.Length > 0)
            {
                trace.privUsers = users;
                TraceFilter userFilter;
                for (int i = 0; i < users.Length; i++)
                {
                    userFilter = new TraceFilter(AuditLogColumnId.SQLSecurityLoginName,
                                                  TraceFilterComparisonOpXE.NotLike,
                                                  users[i],
                                                  AuditLogFilterLogicalOp.AND);
                    trace.AddFilter(userFilter);
                }

                if (sqlVersion > 8)
                {
                    for (int i = 0; i < users.Length; i++)
                    {
                        userFilter = new TraceFilter(AuditLogColumnId.SessionLoginName,
                                                      TraceFilterComparisonOpXE.NotLike,
                                                      users[i],
                                                      AuditLogFilterLogicalOp.AND);
                        trace.AddFilter(userFilter);
                    }
                }
            }
        }


        //-----------------------------------------------------------
        // MergeAuditLogs - merge a set of Audit Logs into one
        // Note: there is no comparison of the two Audit Logs configurations
        //-----------------------------------------------------------
        protected AuditLogConfiguration[]
           MergeAuditLogs(
              ArrayList auditLogs
           )
        {
            if (auditLogs == null)
                return null;

            if (auditLogs.Count == 0)
                return (AuditLogConfiguration[])auditLogs.ToArray(typeof(AuditLogConfiguration));

            AuditLogConfiguration tmpAuditLog;
            AuditLogConfiguration newAuditLog = (AuditLogConfiguration)auditLogs[0];
            AuditLogEventID[] events;
            TraceFilter[] filters;

            for (int i = 1; i < auditLogs.Count; i++)
            {
                tmpAuditLog = (AuditLogConfiguration)auditLogs[i];

                events = tmpAuditLog.GetAuditLogEvents();
                for (int j = 0; j < events.Length; j++)
                    newAuditLog.AddEvent(events[j]);

                filters = tmpAuditLog.GetTraceFilters();
                for (int j = 0; j < filters.Length; j++)
                    newAuditLog.AddFilter(filters[j]);

            }

            int lastTrace = auditLogs.Count - 1;
            for (int i = lastTrace; i > 0; i--)
                auditLogs.RemoveAt(i);


            return (AuditLogConfiguration[])auditLogs.ToArray(typeof(AuditLogConfiguration));

        }

        #endregion
    }
}
