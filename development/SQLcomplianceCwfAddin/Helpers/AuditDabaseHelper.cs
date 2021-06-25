using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Agent;
using Idera.SQLcompliance.Core.Event;
using Idera.SQLcompliance.Core.Status;
using Idera.SQLcompliance.Core.Templates.AuditTemplates;
using SQLcomplianceCwfAddin.Helpers.Agent;
using SQLcomplianceCwfAddin.Helpers.Regulations;
using SQLcomplianceCwfAddin.Helpers.SQL;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.AddDatabase;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.DatabaseProperties;
using Idera.SQLcompliance.Core.Rules.Filters;
using Idera.SQLcompliance.Core.Rules;
using Idera.SQLcompliance.Core.Rules.Alerts;
using System.Text;
using System.Data;
using System.IO;
using Idera.SQLcompliance.Core.TraceProcessing;

namespace SQLcomplianceCwfAddin.Helpers
{
    public class AuditDabaseHelper : Singleton<AuditDabaseHelper>
    {
        #region Private Members
        private const int NO_ID = -1;
        
        private const string COMMA_CHARACTER = ",";
        private const string BAD_NoColumns = "Not configured";
        private const string Error_CantLoadTables = "The tables for this database could not be loaded. The SQLcompliance agent for the SQL Server is down or inaccessible and you do not have access to directly retrieve the list of tables from the SQL Server. You may not select specific tables for auditing until the problem is resolved.";
        private const string Warning_BAD_Table_Missing = "Audited table {0} is no longer available for auditing. It may have been removed or renamed.";
        private const string Warning_BAD_Tables_Missing = "Audited tables {0} are no longer available for auditing. They may have been removed or renamed.";
        private const string Warning_SC_Table_Missing = "Audited table {0} is no longer available for auditing. It may have been removed or renamed.";
        private const string Warning_SC_Tables_Missing = "Audited tables {0} are no longer available for auditing. They may have been removed or renamed.";
        private const string Error_ErrorSavingDatabase = "An error occurred trying to save the changes to the database. The database may be modified after the problem is resolved.";
        private const string Error_MustSelectOneAuditOption = "You must select at least one type of activity to be audited.";
        private const string Error_MustSelectOneAuditObject = "You must select at least one type of object to be audited.";
        private const string Error_NoUserTables = "At least one user table must be selected when you choose to specify which user tables to audit. Select at least one user tables or change the option to 'Don't audit user tables' or 'Audit all user tables'";
        private const string Error_DMLAuditingNotEnabled = "Before-After auditing has been configured, but DML auditing has been disabled. Enable DML auditing to continue.";
        private const string Error_BADTableNotAudited = "The table [{0}] has been selected for Before-After auditing but is not currently being audited for DML. Either add the table for DML auditing or remove it from Before-After data auditing.";
        private const string Error_UserTableAuditingNotEnabled = "Before-After auditing has been configured, but DML auditing for user tables has been disabled. Enable DML auditing for the selected user tables to continue.";
        private const string Error_MustSelectOneAuditUserOption = "You must select at least one type of activity to be audited for privileged users.";
        private const string Error_LoadingDatabaseProperties = "An error occurred trying to load the properties for the selected database.\n\nError:\n\n{0}"; 
        private const string NONE_VALUE = "None";
        private const string NO_TABLES_VALUE = "No tables";
        #endregion

        #region Private Methods

        private List<AuditedDatabaseInfo> GetSystemDatabasesForInstace(int serverId, SqlConnection connection)
        {
            var databaseInfoList = new List<AuditedDatabaseInfo>();
            ServerRecord server = null;
            IEnumerable rawDatabaseList = null;

            try
            {
                server = SqlCmRecordReader.GetServerRecord(serverId, connection);

                if (server.IsDeployed && server.IsRunning)
                {
                    if (!server.IsHadrEnabled)
                    {
                        var agentManager = AgentManagerHelper.Instance.GetAgentManagerProxy(connection);
                        rawDatabaseList = agentManager.GetRawSystemDatabases(server.Instance);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                            String.Format("Loading database for instance '{0}'", server.Instance),
                                            ex,
                                            ErrorLog.Severity.Warning);
                rawDatabaseList = null;
            }


            // straight connection to SQL Server
            if (server != null &&
                rawDatabaseList == null)
            {
                var sqlServer = new SqlDirect();
                if (sqlServer.OpenConnection(server.Instance))
                {
                    if (!server.IsHadrEnabled)
                    {
                        rawDatabaseList = RawSQL.GetSystemDatabases(sqlServer.Connection);
                    }
                }
            }

            if ((rawDatabaseList != null))
            {

                databaseInfoList = Converter.ConvertReferenceList(rawDatabaseList,
                    delegate(RawDatabaseObject rawDatabase)
                    {
                        AuditedDatabaseInfo databaseInfo = null;
                        var database = new DatabaseRecord();
                        database.Connection = connection;

                        if (!database.Read(server.Instance, rawDatabase.name))
                        {
                            databaseInfo =
                                new AuditedDatabaseInfo
                                {
                                    Id = rawDatabase.dbid,
                                    ServerId = serverId,
                                    Name = rawDatabase.name,
                                    IsEnabled = false,
                                };
                        }

                        return databaseInfo;
                    });
            }
            return databaseInfoList;
        }

        // Method to override the custom regulation setting.

        private void UpdatedCustomRegulationAuditActivities(DatabaseRecord record, AuditActivity activity)
        {
            record.AuditDDL = activity.AuditDDL;
            record.AuditSecurity = activity.AuditSecurity;
            record.AuditAdmin = activity.AuditAdmin;
            record.AuditDML = activity.AuditDML;
            record.AuditSELECT = activity.AuditSELECT;
            record.AuditAccessCheck = activity.AuditAccessCheck;
            if (record.AuditDML || record.AuditSELECT)
            {
            record.AuditCaptureSQL = CoreConstants.AllowCaptureSql && activity.AuditCaptureSQL;
            }
            else
                record.AuditCaptureSQL = false;
            if (record.AuditDDL || record.AuditSecurity)
            {
            record.AuditCaptureDDL = CoreConstants.AllowCaptureSql && activity.AuditCaptureDDL;
            }
            else
                record.AuditCaptureDDL = false;
            if (record.AuditDML)
            {
            record.AuditCaptureTrans = activity.AuditCaptureTrans;
            }
            else
                record.AuditCaptureTrans = false;            

            record.HIPAA = record.PCI = record.DISA = record.NERC = record.CIS = record.SOX = record.FERPA = false;
        }

        private AuditActivity GetDefaultDatabaseAuditActivities()
        {
            return new AuditActivity
            {
                AuditDDL = true,
                AuditSecurity = true,
                AuditAdmin = true,
                AuditDML = false,
                AuditSELECT = false,
                AuditAccessCheck = AccessCheckFilter.SuccessOnly,
                AuditCaptureSQL = false,
                AuditCaptureTrans = false,
                AuditCaptureDDL = false,
            };
        }

        private void UpdatedDatabaseRecordWithAuditActivities(DatabaseRecord record, AuditActivity activity)
        {
            if (activity == null)
            {
                activity = GetDefaultDatabaseAuditActivities();
            }

            record.AuditDDL = activity.AuditDDL;
            record.AuditSecurity = activity.AuditSecurity;
            record.AuditAdmin = activity.AuditAdmin;
            record.AuditDML = activity.AuditDML;
            record.AuditSELECT = activity.AuditSELECT;
            record.AuditAccessCheck = activity.AuditAccessCheck;
            record.AuditCaptureSQL = CoreConstants.AllowCaptureSql && activity.AuditCaptureSQL;
            record.AuditCaptureDDL = CoreConstants.AllowCaptureSql && activity.AuditCaptureDDL;
            record.AuditCaptureTrans = activity.AuditCaptureTrans;
        }

        private void ApplyDefaultSettingsForDatabase(DatabaseRecord db, SQLcomplianceConfiguration config)
        {
            // Audit Settings	
            var defaultActivity = GetDefaultDatabaseAuditActivities();
            UpdatedDatabaseRecordWithAuditActivities(db, defaultActivity);

            db.AuditExceptions = false;
            db.AuditDmlAll = true;

            db.AuditUserTables = config.AuditUserTables;
            db.AuditSystemTables = config.AuditSystemTables;
            db.AuditStoredProcedures = config.AuditStoredProcedures;
            db.AuditDmlOther = config.AuditDmlOther;
        }

        private void ApplyRegulationSettingsForDatabase(DatabaseRecord db, AuditRegulationSettings regulationSettings, SqlConnection connection)
        {
            RegulationSettings settings;
            var regulationCategoryDictionary = RegulationSettingsHelper.Instance.GetCategoryDictionary(connection);
            DatabaseRecord tempDb = new DatabaseRecord();

            // apply PCI
            if (regulationSettings.PCI)
            {
                db.PCI = true;

                if (regulationCategoryDictionary.TryGetValue((int)Regulation.RegulationType.PCI, out settings))
                {
                    tempDb.AuditDDL = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition);
                    tempDb.AuditSecurity = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) == (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges);
                    tempDb.AuditAdmin = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) == (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity);
                    tempDb.AuditDML = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification);
                    tempDb.AuditSELECT = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Select) == (int)RegulationSettings.RegulationDatabaseCategory.Select);

                    if ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess)
                        tempDb.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess)
                        tempDb.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else
                        tempDb.AuditAccessCheck = AccessCheckFilter.NoFilter;

                    tempDb.AuditCaptureSQL = CoreConstants.AllowCaptureSql && ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SQLText) == (int)RegulationSettings.RegulationDatabaseCategory.SQLText);
                    tempDb.AuditCaptureTrans = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Transactions) == (int)RegulationSettings.RegulationDatabaseCategory.Transactions);
                    tempDb.AuditSensitiveColumns = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) == (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns);
                    tempDb.AuditDataChanges = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange) == (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange);
                }
            }
            else
                db.PCI = false;

            // apply HIPAA
            // the OR against the server settings is done because the user can select more than one template.  When more than one template is 
            // selected, the options are combined together
            if (regulationSettings.HIPAA)
            {
                db.HIPAA = true;

                if (regulationCategoryDictionary.TryGetValue((int)Regulation.RegulationType.HIPAA, out settings))
                {
                    tempDb.AuditDDL = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) || tempDb.AuditDDL;
                    tempDb.AuditSecurity = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) == (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) || tempDb.AuditSecurity;
                    tempDb.AuditAdmin = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) == (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) || tempDb.AuditAdmin;
                    tempDb.AuditDML = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) || tempDb.AuditDML;
                    tempDb.AuditSELECT = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Select) == (int)RegulationSettings.RegulationDatabaseCategory.Select) || tempDb.AuditSELECT;

                    if (((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) || (tempDb.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempDb.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if (((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) || (tempDb.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempDb.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else
                        tempDb.AuditAccessCheck = AccessCheckFilter.NoFilter;

                    tempDb.AuditCaptureSQL = (CoreConstants.AllowCaptureSql && ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SQLText) == (int)RegulationSettings.RegulationDatabaseCategory.SQLText)) || tempDb.AuditCaptureSQL;
                    tempDb.AuditCaptureTrans = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Transactions) == (int)RegulationSettings.RegulationDatabaseCategory.Transactions) || tempDb.AuditCaptureTrans;
                    tempDb.AuditSensitiveColumns = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) == (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) || tempDb.AuditSensitiveColumns;
                    tempDb.AuditDataChanges = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange) == (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange) || tempDb.AuditDataChanges;
                }
            }
            else
                db.HIPAA = false;

            if (regulationSettings.DISA)
            {
                db.DISA = true;

                if (regulationCategoryDictionary.TryGetValue((int)Regulation.RegulationType.DISA, out settings))
                {
                    tempDb.AuditDDL = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) || tempDb.AuditDDL;
                    tempDb.AuditSecurity = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) == (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) || tempDb.AuditSecurity;
                    tempDb.AuditAdmin = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) == (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) || tempDb.AuditAdmin;
                    tempDb.AuditDML = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) || tempDb.AuditDML;
                    tempDb.AuditSELECT = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Select) == (int)RegulationSettings.RegulationDatabaseCategory.Select) || tempDb.AuditSELECT;

                    if (((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) || (tempDb.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempDb.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if (((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) || (tempDb.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempDb.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else
                        tempDb.AuditAccessCheck = AccessCheckFilter.NoFilter;

                    tempDb.AuditCaptureSQL = (CoreConstants.AllowCaptureSql && ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SQLText) == (int)RegulationSettings.RegulationDatabaseCategory.SQLText)) || tempDb.AuditCaptureSQL;
                    tempDb.AuditCaptureTrans = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Transactions) == (int)RegulationSettings.RegulationDatabaseCategory.Transactions) || tempDb.AuditCaptureTrans;
                    tempDb.AuditSensitiveColumns = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) == (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) || tempDb.AuditSensitiveColumns;
                    tempDb.AuditDataChanges = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange) == (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange) || tempDb.AuditDataChanges;
                }
            }
            else
                db.DISA = false;

            if (regulationSettings.NERC)
            {
                db.NERC = true;

                if (regulationCategoryDictionary.TryGetValue((int)Regulation.RegulationType.NERC, out settings))
                {
                    tempDb.AuditDDL = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) || tempDb.AuditDDL;
                    tempDb.AuditSecurity = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) == (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) || tempDb.AuditSecurity;
                    tempDb.AuditAdmin = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) == (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) || tempDb.AuditAdmin;
                    tempDb.AuditDML = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) || tempDb.AuditDML;
                    tempDb.AuditSELECT = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Select) == (int)RegulationSettings.RegulationDatabaseCategory.Select) || tempDb.AuditSELECT;

                    if (((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) || (tempDb.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempDb.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if (((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) || (tempDb.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempDb.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else
                        tempDb.AuditAccessCheck = AccessCheckFilter.NoFilter;

                    tempDb.AuditCaptureSQL = (CoreConstants.AllowCaptureSql && ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SQLText) == (int)RegulationSettings.RegulationDatabaseCategory.SQLText)) || tempDb.AuditCaptureSQL;
                    tempDb.AuditCaptureTrans = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Transactions) == (int)RegulationSettings.RegulationDatabaseCategory.Transactions) || tempDb.AuditCaptureTrans;
                    tempDb.AuditSensitiveColumns = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) == (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) || tempDb.AuditSensitiveColumns;
                    tempDb.AuditDataChanges = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange) == (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange) || tempDb.AuditDataChanges;
                }
            }
            else
                db.NERC = false;
            if (regulationSettings.CIS)
            {
                db.CIS = true;
                if (!(regulationSettings.PCI || regulationSettings.HIPAA || regulationSettings.DISA || regulationSettings.NERC || regulationSettings.SOX || regulationSettings.FERPA))
                {
                    tempDb.AuditDDL = true;
                    tempDb.AuditSecurity = true;
                    tempDb.AuditAdmin = true;
                        tempDb.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                }
            }
            else
                db.CIS = false;
            if (regulationSettings.SOX)
            {
                db.SOX = true;
                if (regulationCategoryDictionary.TryGetValue((int)Regulation.RegulationType.SOX, out settings))
                {
                    tempDb.AuditDDL = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) || tempDb.AuditDDL;
                    tempDb.AuditSecurity = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) == (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) || tempDb.AuditSecurity;
                    tempDb.AuditAdmin = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) == (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) || tempDb.AuditAdmin;
                    tempDb.AuditDML = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) || tempDb.AuditDML;
                    tempDb.AuditSELECT = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Select) == (int)RegulationSettings.RegulationDatabaseCategory.Select) || tempDb.AuditSELECT;
                    if (((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) || (tempDb.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempDb.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if (((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) || (tempDb.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempDb.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else
                        tempDb.AuditAccessCheck = AccessCheckFilter.NoFilter;
                    tempDb.AuditCaptureSQL = (CoreConstants.AllowCaptureSql && ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SQLText) == (int)RegulationSettings.RegulationDatabaseCategory.SQLText)) || tempDb.AuditCaptureSQL;
                    tempDb.AuditCaptureTrans = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Transactions) == (int)RegulationSettings.RegulationDatabaseCategory.Transactions) || tempDb.AuditCaptureTrans;
                    tempDb.AuditSensitiveColumns = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) == (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) || tempDb.AuditSensitiveColumns;
                    tempDb.AuditDataChanges = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange) == (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange) || tempDb.AuditDataChanges;
                }
            }
            else
                db.SOX = false;
            if (regulationSettings.FERPA)
            {
                db.FERPA = true;
                if (regulationCategoryDictionary.TryGetValue((int)Regulation.RegulationType.FERPA, out settings))
                {
                    tempDb.AuditDDL = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) || tempDb.AuditDDL;
                    tempDb.AuditSecurity = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) == (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) || tempDb.AuditSecurity;
                    tempDb.AuditAdmin = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) == (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) || tempDb.AuditAdmin;
                    tempDb.AuditDML = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) || tempDb.AuditDML;
                    tempDb.AuditSELECT = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Select) == (int)RegulationSettings.RegulationDatabaseCategory.Select) || tempDb.AuditSELECT;
                    if (((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) || (tempDb.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempDb.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if (((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) || (tempDb.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempDb.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else
                        tempDb.AuditAccessCheck = AccessCheckFilter.NoFilter;
                    tempDb.AuditCaptureSQL = (CoreConstants.AllowCaptureSql && ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SQLText) == (int)RegulationSettings.RegulationDatabaseCategory.SQLText)) || tempDb.AuditCaptureSQL;
                    tempDb.AuditCaptureTrans = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Transactions) == (int)RegulationSettings.RegulationDatabaseCategory.Transactions) || tempDb.AuditCaptureTrans;
                    tempDb.AuditSensitiveColumns = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) == (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) || tempDb.AuditSensitiveColumns;
                    tempDb.AuditDataChanges = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange) == (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange) || tempDb.AuditDataChanges;
                }
            }
            else
                db.FERPA = false;
            if (regulationSettings.GDPR)
            {
                db.GDPR = true;
                if (regulationCategoryDictionary.TryGetValue((int)Regulation.RegulationType.GDPR, out settings))
                {
                    tempDb.AuditDDL = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) || tempDb.AuditDDL;
                    tempDb.AuditSecurity = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) == (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) || tempDb.AuditSecurity;
                    tempDb.AuditAdmin = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) == (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) || tempDb.AuditAdmin;
                    tempDb.AuditDML = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) || tempDb.AuditDML;
                    tempDb.AuditSELECT = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Select) == (int)RegulationSettings.RegulationDatabaseCategory.Select) || tempDb.AuditSELECT;
                    if (((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) || (tempDb.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempDb.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if (((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) || (tempDb.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempDb.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else
                        tempDb.AuditAccessCheck = AccessCheckFilter.NoFilter;
                    tempDb.AuditCaptureSQL = (CoreConstants.AllowCaptureSql && ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SQLText) == (int)RegulationSettings.RegulationDatabaseCategory.SQLText)) || tempDb.AuditCaptureSQL;
                    tempDb.AuditCaptureTrans = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Transactions) == (int)RegulationSettings.RegulationDatabaseCategory.Transactions) || tempDb.AuditCaptureTrans;
                    tempDb.AuditSensitiveColumns = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) == (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) || tempDb.AuditSensitiveColumns;
                    tempDb.AuditDataChanges = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange) == (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange) || tempDb.AuditDataChanges;
                }
            }
            else
                db.GDPR = false;

            //return the database settings
            db.AuditDDL = tempDb.AuditDDL;
            db.AuditSecurity = tempDb.AuditSecurity;
            db.AuditAdmin = tempDb.AuditAdmin;
            db.AuditDML = tempDb.AuditDML;
            db.AuditSELECT = tempDb.AuditSELECT;
            db.AuditAccessCheck = tempDb.AuditAccessCheck;
            db.AuditSensitiveColumns = tempDb.AuditSensitiveColumns;
            if (db.AuditDML || db.AuditSELECT)
            {
            db.AuditCaptureSQL = tempDb.AuditCaptureSQL;
            }
            else
            {
                db.AuditCaptureSQL = false;
            }
            if (db.AuditDDL || db.AuditSecurity)
            {
                db.AuditCaptureDDL = tempDb.AuditCaptureDDL;
            }
            else
            {
                db.AuditCaptureDDL = false;
            }
            if (db.AuditDML)
            {
                db.AuditDataChanges = tempDb.AuditDataChanges;
                db.AuditCaptureTrans = tempDb.AuditCaptureTrans;
            }
            else
                db.AuditCaptureTrans = false;
        }

        private void SetAvailAbilityGroupInfoForDatabase(DatabaseRecord db, AuditDatabaseSettings auditSeetings)
        {
            if (auditSeetings.AvailabilityGroupList != null)
            {
                foreach (var group in auditSeetings.AvailabilityGroupList)
                {
                    if (group.Name.Equals(db.Name))
                    {
                        if (db.ReplicaServers.Length == 0)
                        {
                            db.IsAlwaysOn = true;
                            db.AvailGroupName = group.Name;
                            db.ReplicaServers = group.ReplicaServerName;
                        }
                        else
                        {
                            db.ReplicaServers += COMMA_CHARACTER + group.ReplicaServerName;
                        }
                    }
                }
            }
        }

        private AuditActivity GetDefaultAuditUserActivitySettings()
        {
            return new AuditActivity
            {
                AuditAllUserActivities = false,
                AuditLogins = true,
                AuditLogouts = true,
                AuditFailedLogins = true,
                AuditSecurity = true,
                AuditAdmin = true,
                AuditDDL = true,
                AuditAccessCheck = AccessCheckFilter.SuccessOnly,
            };
        }

        private DmlSelectFilters GetDefaultDmlSelectFilters()
        {
            return new DmlSelectFilters
            {
                AuditDmlAll = false,
                AuditUserTables = AuditUserTables.All,
            };
        }

        private void FillDatabaseRecordWithAuditUserActivitySettings(DatabaseRecord database, AuditActivity userAuditedActivities)
        {
            if (userAuditedActivities == null)
            {
                userAuditedActivities = GetDefaultAuditUserActivitySettings();
            }

            database.AuditUserAll = userAuditedActivities.AuditAllUserActivities;
            database.AuditUserLogins = userAuditedActivities.AuditLogins;
            database.AuditUserLogouts = userAuditedActivities.AuditLogouts;
            database.AuditUserFailedLogins = userAuditedActivities.AuditFailedLogins;
            database.AuditUserDDL = userAuditedActivities.AuditDDL;
            database.AuditUserSecurity = userAuditedActivities.AuditSecurity;
            database.AuditUserAdmin = userAuditedActivities.AuditAdmin;
            database.AuditUserDML = userAuditedActivities.AuditDML;
            database.AuditUserSELECT = userAuditedActivities.AuditSELECT;
            database.AuditUserUDE = userAuditedActivities.AuditDefinedEvents;
            database.AuditUserAccessCheck = userAuditedActivities.AuditAccessCheck;
            database.AuditUserCaptureSQL = userAuditedActivities.AuditCaptureSQL;
            database.AuditUserCaptureTrans = userAuditedActivities.AuditCaptureTrans;
            database.AuditUserCaptureDDL = userAuditedActivities.AuditCaptureDDL;
        }

        private DatabaseRecord CreateDatabaseRecord(
            AuditedDatabaseInfo database,
            AuditDatabaseSettings auditSeetings,
            ServerRecord server,
            SQLcomplianceConfiguration config,
            SqlConnection connection)
        {
            var db = new DatabaseRecord();
            db.Connection = connection;

            // General
            db.SrvId = server == null ? -1 : server.SrvId;
            db.SrvInstance = server.Instance;
            db.Name = database.Name;
            db.SqlDatabaseId = database.Id;
            db.Description = "";
            db.IsEnabled = true;
            db.IsSqlSecureDb = false;
            db.IsAlwaysOn = false;
            db.ReplicaServers = "";

            SetAvailAbilityGroupInfoForDatabase(db, auditSeetings);

            db.AuditUsersList = AuditServerHelper.GetRolesAndUsersString(auditSeetings.TrustedRolesAndUsers);
            db.AuditPrivUsersList = AuditServerHelper.GetRolesAndUsersString(auditSeetings.PrivilegedRolesAndUsers);

            FillDatabaseRecordWithAuditUserActivitySettings(db, auditSeetings.UserAuditedActivities);

            db.AuditUserExceptions = false;

            if (auditSeetings.CollectionLevel == AuditCollectionLevel.Default)
            {
                ApplyDefaultSettingsForDatabase(db, config);
            }
            else
            {
                if (auditSeetings.CollectionLevel == AuditCollectionLevel.Regulation)
                {
                    if (auditSeetings.AuditedActivities != null)
                    {
                        UpdatedCustomRegulationAuditActivities(db, auditSeetings.AuditedActivities);
                    }
                    else
                    ApplyRegulationSettingsForDatabase(db, auditSeetings.RegulationSettings, connection);
                }
                else
                {
                    UpdatedDatabaseRecordWithAuditActivities(db, auditSeetings.AuditedActivities);
                }

                db.AuditExceptions = false;

                if (db.AuditDML || db.AuditSELECT)
                {
                    if (auditSeetings.DmlSelectFilters == null)
                    {
                        auditSeetings.DmlSelectFilters = GetDefaultDmlSelectFilters();
                    }

                    if (auditSeetings.DmlSelectFilters.AuditDmlAll)
                    {
                        db.AuditDmlAll = true;

                        // set other values to glabl defaults		         
                        db.AuditUserTables = config.AuditUserTables;
                        db.AuditSystemTables = config.AuditSystemTables;
                        db.AuditStoredProcedures = config.AuditStoredProcedures;
                        db.AuditDmlOther = config.AuditDmlOther;
                    }
                    else
                    {
                        db.AuditDmlAll = false;

                        // User Tables (not boolean; 2 means select specific tables; done in DB properties)
                        db.AuditUserTables = Convert.ToInt32(auditSeetings.DmlSelectFilters.AuditUserTables);

                        // Audit Objects
                        db.AuditSystemTables = auditSeetings.DmlSelectFilters.AuditSystemTables;
                        db.AuditStoredProcedures = auditSeetings.DmlSelectFilters.AuditStoredProcedures;
                        db.AuditDmlOther = auditSeetings.DmlSelectFilters.AuditDmlOther;
                    }
                }
                else
                {
                    // save global defaults
                    db.AuditDmlAll = config.AuditDmlAll;
                    db.AuditUserTables = config.AuditUserTables;
                    db.AuditSystemTables = config.AuditSystemTables;
                    db.AuditStoredProcedures = config.AuditStoredProcedures;
                    db.AuditDmlOther = config.AuditDmlOther;
                }
            }
            if (db.AuditDML)
            {
                db.AuditDataChanges = true;
            }

            db.AuditSensitiveColumns = true;
            return db;
        }
        private AuditActivity GetDatabaseTemplateRecord(
            AuditRegulationSettings auditSettings,
            SqlConnection connection)
        {
            AuditActivity tempDb = new AuditActivity();
            tempDb = GetRegulationSettingsForDatabase(auditSettings, connection);
            return tempDb;
        }

        private bool WriteDatabaseRecord(DatabaseRecord database, SqlConnection connection)
        {
            bool retval = false;
            int temp = 0;

            try
            {
                string strSQL = string.Format("Select * From {0} where name = '{1}' and srvInstance = '{2}'", CoreConstants.RepositoryDatabaseTable, database.Name, database.SrvInstance);
                SqlCommand checkDatbase = new SqlCommand(strSQL, connection);
                using (SqlDataReader reader = checkDatbase.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        temp = 1;
                    }
                }
                if (temp == 1)
                {
                    retval=RegulationUpdate(database, connection);
                }
                else
                {
                    retval = database.Create(null);
                }                    
            }
            
            finally
            {
                if (retval)
                {
                    ServerUpdate.RegisterChange(database.SrvId, LogType.NewDatabase, database.SrvInstance, database.Name, connection);
                }
            }
            return retval;
        }

        private bool RegulationUpdate(DatabaseRecord database, SqlConnection connection)
        {
            StringBuilder prop = new StringBuilder("");
            database.DbId = database.SqlDatabaseId;
            prop.AppendFormat("name = {0}", SQLHelpers.CreateSafeString(database.Name));

            if (prop.Length > 0) prop.Append(",");
            prop.AppendFormat("description = {0}",
                               SQLHelpers.CreateSafeString(database.Description));

            if (prop.Length > 0) prop.Append(",");
            prop.AppendFormat("isEnabled = {0}",
                               database.IsEnabled ? 1 : 0);

            //-------------------------------------------------------------------------
            // Audit Settings
            //
            // If just changed from not overridng to overriding server, force write 
            // of all audit settings        
            //-------------------------------------------------------------------------
            if (prop.Length > 0) prop.Append(",");
            prop.AppendFormat("auditDDL = {0}", database.AuditDDL ? 1 : 0);

            if (prop.Length > 0) prop.Append(",");
            prop.AppendFormat("auditSecurity = {0}", database.AuditSecurity ? 1 : 0);

            if (prop.Length > 0) prop.Append(",");
            prop.AppendFormat("auditAdmin = {0}", database.AuditAdmin ? 1 : 0);

            if (prop.Length > 0) prop.Append(",");
            prop.AppendFormat("auditBroker = {0}", database.AuditBroker ? 1 : 0);

            if (prop.Length > 0) prop.Append(",");
            prop.AppendFormat("auditDML = {0}", database.AuditDML ? 1 : 0);

            if (prop.Length > 0) prop.Append(",");
            prop.AppendFormat("auditSELECT = {0}", database.AuditSELECT ? 1 : 0);

            if (prop.Length > 0) prop.Append(",");
            prop.AppendFormat("auditFailures = {0}", (int)database.AuditAccessCheck);

            if (prop.Length > 0) prop.Append(",");
            if (!CoreConstants.AllowCaptureSql)
                prop.Append("auditCaptureSQL = 2");
            else
                prop.AppendFormat("auditCaptureSQL = {0}", database.AuditCaptureSQL ? 1 : 0);

            if (prop.Length > 0) prop.Append(",");
            prop.AppendFormat("auditExceptions = {0}", database.AuditExceptions ? 1 : 0);

            if (prop.Length > 0) prop.Append(",");
            prop.AppendFormat("auditUsersList = {0}", SQLHelpers.CreateSafeString(database.AuditUsersList));

            if (prop.Length > 0) prop.Append(",");
            prop.AppendFormat("pci = {0}", database.PCI ? 1 : 0);

            if (prop.Length > 0) prop.Append(",");
            prop.AppendFormat("hipaa = {0}", database.HIPAA ? 1 : 0);

            if (prop.Length > 0) prop.Append(",");
            prop.AppendFormat("disa = {0}", database.DISA ? 1 : 0);

            if (prop.Length > 0) prop.Append(",");
            prop.AppendFormat("nerc = {0}", database.NERC ? 1 : 0);

            if (prop.Length > 0) prop.Append(",");
            prop.AppendFormat("cis = {0}", database.CIS ? 1 : 0);

            if (prop.Length > 0) prop.Append(",");
            prop.AppendFormat("sox = {0}", database.SOX ? 1 : 0);

            if (prop.Length > 0) prop.Append(",");
            prop.AppendFormat("ferpa = {0}", database.FERPA ? 1 : 0);

            //---------------
            // DML Filtering
            //---------------

            if (prop.Length > 0) prop.Append(",");
            prop.AppendFormat("auditDmlAll = {0}", database.AuditDmlAll ? 1 : 0);

            if (prop.Length > 0) prop.Append(",");
            prop.AppendFormat("auditUserTables = {0}", database.AuditUserTables);

            if (prop.Length > 0) prop.Append(",");
            prop.AppendFormat("auditSystemTables = {0}", database.AuditSystemTables ? 1 : 0);

            if (prop.Length > 0) prop.Append(",");
            prop.AppendFormat("auditStoredProcedures = {0}", database.AuditStoredProcedures ? 1 : 0);

            if (prop.Length > 0) prop.Append(",");
            prop.AppendFormat("auditDmlOther = {0}", database.AuditDmlOther ? 1 : 0);

            if (prop.Length > 0) prop.Append(",");
            prop.AppendFormat("auditDataChanges = {0}", database.AuditDataChanges ? 1 : 0);

            if (prop.Length > 0) prop.Append(",");
            prop.AppendFormat("auditSensitiveColumns = {0}", database.AuditSensitiveColumns ? 1 : 0);

            if (prop.Length > 0) prop.Append(",");
            prop.AppendFormat("auditCaptureTrans = {0}", database.AuditCaptureTrans ? 1 : 0);


            // Users

            if (prop.Length > 0) prop.Append(",");
            prop.AppendFormat("auditPrivUsersList = {0}", SQLHelpers.CreateSafeString(database.AuditPrivUsersList));

            if (prop.Length > 0) prop.Append(",");
            prop.AppendFormat("auditUserAll = {0}", database.AuditUserAll ? 1 : 0);

            if (prop.Length > 0) prop.Append(",");
            prop.AppendFormat("auditUserLogins = {0}", database.AuditUserLogins ? 1 : 0);

            // SQLCM - 5375 Release 5.6 - 6.1.4.3 - Capture Logout Events
            if (prop.Length > 0) prop.Append(",");
            prop.AppendFormat("auditUserLogouts = {0}", database.AuditUserLogouts ? 1 : 0);

            if (prop.Length > 0) prop.Append(",");
            prop.AppendFormat("auditUserFailedLogins = {0}", database.AuditUserFailedLogins ? 1 : 0);

            if (prop.Length > 0) prop.Append(",");
            prop.AppendFormat("auditUserDDL = {0}", database.AuditUserDDL ? 1 : 0);

            if (prop.Length > 0) prop.Append(",");
            prop.AppendFormat("auditUserSecurity = {0}", database.AuditUserSecurity ? 1 : 0);

            if (prop.Length > 0) prop.Append(",");
            prop.AppendFormat("auditUserAdmin = {0}", database.AuditUserAdmin ? 1 : 0);

            if (prop.Length > 0) prop.Append(",");
            prop.AppendFormat("auditUserDML = {0}", database.AuditUserDML ? 1 : 0);

            if (prop.Length > 0) prop.Append(",");
            prop.AppendFormat("auditUserSELECT = {0}", database.AuditUserSELECT ? 1 : 0);

            if (prop.Length > 0) prop.Append(",");
            prop.AppendFormat("auditUserFailures = {0}", (int)database.AuditUserAccessCheck);

            if (CoreConstants.AllowCaptureSql)
            {
                if (prop.Length > 0) prop.Append(",");
                prop.AppendFormat("auditUserCaptureSQL = {0}", database.AuditUserCaptureSQL ? 1 : 0);
            }
            else
            {
                if (prop.Length > 0) prop.Append(",");
                prop.AppendFormat("auditUserCaptureSQL = {0}", 2);
            }

            if (CoreConstants.AllowCaptureSql)
            {
                if (prop.Length > 0) prop.Append(",");
                prop.AppendFormat("auditUserCaptureDDL = {0}", database.AuditUserCaptureDDL ? 1 : 0);
            }
            else
            {
                if (prop.Length > 0) prop.Append(",");
                prop.AppendFormat("auditUserCaptureDDL = {0}", 2);
            }

            if (CoreConstants.AllowCaptureSql)
            {
                if (prop.Length > 0) prop.Append(",");
                prop.AppendFormat("auditCaptureDDL = {0}", database.AuditCaptureDDL ? 1 : 0);
            }
            else
            {
                if (prop.Length > 0) prop.Append(",");
                prop.AppendFormat("auditCaptureDDL = {0}", 2);
            }

            if (prop.Length > 0) prop.Append(",");
            prop.AppendFormat("auditUserCaptureTrans = {0}", database.AuditUserCaptureTrans ? 1 : 0);

            if (prop.Length > 0) prop.Append(",");
            prop.AppendFormat("auditUserExceptions = {0}", database.AuditUserExceptions ? 1 : 0);

            if (prop.Length > 0) prop.Append(",");
            prop.AppendFormat("auditUserUDE = {0}", database.AuditUserUDE ? 1 : 0);


            //----------------------------------------------------
            // Finish Building SQL if any properties have changed
            //-----------------------------------------------------
            StringBuilder tmp = new StringBuilder("");
            if (prop.Length > 0)
            {
                // update last modified
                prop.Append(",timeLastModified = GETUTCDATE()");

                tmp.AppendFormat("UPDATE {0} SET ",
                                  CoreConstants.RepositoryDatabaseTable);
                tmp.Append(prop.ToString());
                tmp.AppendFormat(" WHERE dbId={0};", database.SqlDatabaseId);
            }

            string strSQL = tmp.ToString();
            SqlCommand updateDatbase = new SqlCommand(strSQL, connection);
            return (0 < updateDatbase.ExecuteNonQuery());
        }

        private bool RemoveDatabase(DatabaseRecord database, SqlConnection connection)
        {
            bool wasRemoved = false;

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        wasRemoved = database.Delete(transaction) &&
                                     DatabaseObjectRecord.DeleteUserTables(database.DbId, transaction, connection);

                        if (database.IsAlwaysOn)
                        {
                            RemoveAlwaysOnDatabases(database, connection);
                        }
                    }
                    finally
                    {
                        if (wasRemoved)
                        {
                            transaction.Commit();

                            // Register change to server and perform audit log				      
                            ServerUpdate.RegisterChange(database.SrvId, LogType.RemoveDatabase, database.SrvInstance,
                                database.Name,
                                connection);
                        }
                        else
                        {
                            transaction.Rollback();

                            ErrorLog.Instance.Write(
                                ErrorLog.Level.Verbose,
                                string.Format("Remove Database with id '{0}'. Error :  '{1}' ", database.DbId,
                                    DatabaseRecord.GetLastError()),
                                ErrorLog.Severity.Warning);
                        }
                    }
                }

            return wasRemoved;
        }

        private void RemoveAlwaysOnDatabases(DatabaseRecord database, SqlConnection connection)
        {
            IList listDatabaseAlwaysOnDetails = ServerRecord.GetDatabasesWithInAVG(connection, database.Name, database.ReplicaServers, database.AvailGroupName);

            foreach (DatabaseAlwaysOnDetails databaseAlwaysOnDetails in listDatabaseAlwaysOnDetails)
            {
                if (database.DbId == databaseAlwaysOnDetails.dbId)
                {
                    continue;
                }
                    
                var alwaysOnDatabase = new DatabaseRecord();
                alwaysOnDatabase.Connection = connection;

                if (!alwaysOnDatabase.Read(databaseAlwaysOnDetails.dbId))
                {
                    ErrorLog.Instance.Write(
                           ErrorLog.Level.Verbose,
                           String.Format("Remove Always On Database with id '{0}'. Error: {1} ", databaseAlwaysOnDetails.dbId, DatabaseRecord.GetLastError()),
                           ErrorLog.Severity.Error);
                }

                RemoveDatabase(alwaysOnDatabase, connection);
            }
        }

        private void SetaGeneralProperties(AuditDatabaseProperties databaseProperties, DatabaseRecord database)
        {
            //// General
            databaseProperties.ServerInstance = database.SrvInstance;
            databaseProperties.DatabaseName = database.Name;
            databaseProperties.Description = database.Description;

            //// status
            databaseProperties.AuditingEnableStatus = database.IsEnabled;
            databaseProperties.CreatedDateTime = DateTimeHelper.GetNullableLocalTimeOfCurrentTimeZone(database.TimeCreated).Value;
            databaseProperties.LastModifiedDateTime = DateTimeHelper.GetNullableLocalTimeOfCurrentTimeZone(database.TimeLastModified).Value;
            databaseProperties.LastChangedStatusDateTime = DateTimeHelper.GetNullableLocalTimeOfCurrentTimeZone(database.TimeEnabledModified).Value;
        }

        private void SetAuditUserTables(AuditDatabaseProperties databaseProperties, DatabaseRecord database, ServerRecord server, SqlConnection connection)
        {
            databaseProperties.DmlSelectFilters.AuditUserTables = (AuditUserTables)Enum.ToObject(typeof(AuditUserTables), database.AuditUserTables);

            switch (databaseProperties.DmlSelectFilters.AuditUserTables)
            {
                case AuditUserTables.Following:
                {
                    var userTables = DatabaseObjectRecord.GetUserTables(database.DbId, connection);

                    databaseProperties.DmlSelectFilters.UserTableList = Converter.ConvertReferenceList<DatabaseObjectRecord, DatabaseObject>(userTables,
                    databaseObject => new DatabaseObject()
                    {
                        Id = databaseObject.Id,
                        ObjectId = databaseObject.ObjectId,
                        DatabaseId = databaseObject.DbId,
                        ServerId = NO_ID,
                        ObjectType = ObjectType.Table,
                        TableName = databaseObject.TableName,
                        FullTableName = databaseObject.FullTableName,
                        SchemaName = databaseObject.SchemaName,
                    });

                    break;
                }
            }
        }

        private void SetAuditSettings(AuditDatabaseProperties databaseProperties, DatabaseRecord database, ServerRecord server)
        {
            databaseProperties.AuditedActivities = new AuditActivity();
            var auditActivities = databaseProperties.AuditedActivities;
            auditActivities.AuditDDL = database.AuditDDL;
            auditActivities.AuditSecurity = database.AuditSecurity;
            auditActivities.AuditAdmin = database.AuditAdmin;
            auditActivities.AuditDML = database.AuditDML;
            auditActivities.AuditSELECT = database.AuditSELECT;
            auditActivities.AuditCaptureSQL = database.AuditCaptureSQL;
            auditActivities.AuditCaptureDDL = database.AuditCaptureDDL;
            auditActivities.AuditCaptureTrans = database.AuditCaptureTrans;
            auditActivities.AllowCaptureSql = CoreConstants.AllowCaptureSql;
            auditActivities.IsAgentVersionSupported = ServerRecord.CompareVersions(server.AgentVersion, "3.5") >= 0;
            auditActivities.AuditAccessCheck = database.AuditAccessCheck;

            databaseProperties.DmlSelectFilters = new DmlSelectFilters();
            var dmlSelectFilters = databaseProperties.DmlSelectFilters;
            dmlSelectFilters.AuditDmlAll = database.AuditDmlAll;
            dmlSelectFilters.AuditDmlOther = database.AuditDmlOther;
            dmlSelectFilters.AuditStoredProcedures = database.AuditStoredProcedures;
            dmlSelectFilters.AuditSystemTables = database.AuditSystemTables;
        }

        private void SetPrivilegedUsers(AuditDatabaseProperties databaseProperties, DatabaseRecord database)
        {
            databaseProperties.PrivilegedRolesAndUsers = AuditServerHelper.GetServerRolesAndUsersFromString(database.AuditPrivUsersList, "Problem loading Privileged User auditing information.");
        }

        private void SetTrustedUsers(AuditDatabaseProperties databaseProperties, DatabaseRecord database, ServerRecord server)
        {
            try
            {

                if (!CheckSupportsHelper.SupportsTrustedUsers(server))
                {
                    return;
                }

                databaseProperties.TrustedRolesAndUsers = AuditServerHelper.GetServerRolesAndUsersFromString(database.AuditUsersList,
                    "Problem loading Trusted User auditing information.");

            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(
                    ErrorLog.Level.Always,
                    string.Format("Problem loading trusted users. Error: {0} ", ex),
                    ErrorLog.Severity.Error);
            }
        }

        private void SetPrivilegedUserAuditSettings(AuditDatabaseProperties databaseProperties, DatabaseRecord database, ServerRecord server)
        {
            var auditUserActivities = new AuditActivity();
            auditUserActivities.AuditDDL = database.AuditUserDDL;
            auditUserActivities.AuditSecurity = database.AuditUserSecurity;
            auditUserActivities.AuditAdmin = database.AuditUserAdmin;
            auditUserActivities.AuditDML = database.AuditUserDML;
            auditUserActivities.AuditSELECT = database.AuditUserSELECT;
            auditUserActivities.AuditCaptureSQL = database.AuditUserCaptureSQL;
            auditUserActivities.AuditCaptureDDL = database.AuditUserCaptureDDL;
            auditUserActivities.AuditCaptureTrans = database.AuditUserCaptureTrans;
            auditUserActivities.AllowCaptureSql = CoreConstants.AllowCaptureSql;
            auditUserActivities.IsAgentVersionSupported = ServerRecord.CompareVersions(server.AgentVersion, "3.5") >= 0;
            auditUserActivities.AuditAccessCheck = database.AuditUserAccessCheck;

            // Only for audting user activities
            auditUserActivities.AuditAllUserActivities = database.AuditUserAll;
            auditUserActivities.AuditLogins = database.AuditUserLogins;
            auditUserActivities.AuditLogouts = database.AuditUserLogouts;
            auditUserActivities.AuditFailedLogins = database.AuditUserFailedLogins;
            auditUserActivities.AuditDefinedEvents = database.AuditUserUDE;
            databaseProperties.AuditedPrivilegedUserActivities = auditUserActivities;
        }

        private bool SetBeforeAfterDataAvailable(AuditBeforeAfterData beforeAfterData, DatabaseRecord database, ServerRecord server, SqlConnection connection)
        {
            bool availAble = false;
            // SQL Server 2005,2008
            if (server.SqlVersion == 0)
            {
                beforeAfterData.StatusMessaage = CoreConstants.Feature_BeforeAfterNotAvailableVersionUnknown;
            }
            else if (server.SqlVersion < 9)
            {
                beforeAfterData.StatusMessaage = CoreConstants.Feature_BeforeAfterNotAvailable;
            }
            else if (AgentManagerHelper.Instance.GetCompatibilityLevel(database, connection) < 90)
            {
                beforeAfterData.StatusMessaage = CoreConstants.Feature_BeforeAfterNotAvailableCompatibility;
            }
            else if (!CheckSupportsHelper.SupportsBeforeAfter(server))
            {
                beforeAfterData.StatusMessaage = CoreConstants.Feature_BeforeAfterNotAvailableAgent;
            }
            else
            {
                availAble = true;
            }

            return availAble;
        }

        private void SetBeforeAfterTableDictionary(AuditBeforeAfterData beforeAfterData, Dictionary<string, DatabaseObject> tableDictionary, DatabaseRecord database, ServerRecord server, SqlConnection connection)
        {
            if (database.AuditDataChanges)
            {
                beforeAfterData.ColumnsSupported = CheckSupportsHelper.SupportsBeforeAfter(server);
                beforeAfterData.BeforeAfterTableColumnDictionary = new Dictionary<string, DatabaseObject>();
                var missingTables = new List<string>();
                List<DataChangeTableRecord> tables = DataChangeTableRecord.GetAuditedTables(connection, server.SrvId, database.DbId);

                foreach (DataChangeTableRecord table in tables)
                {
                    var changeDatabaseObject = new DatabaseObject
                    {
                        ServerId = table.SrvId,
                        DatabaseId = table.DbId,
                        ObjectId = table.ObjectId,
                        TableName = table.TableName,
                        FullTableName = table.FullTableName,
                        SchemaName = table.SchemaName,
                        RowLimit = table.RowLimit,
                        SelectedColumns = table.SelectedColumns,
                        ColumnList = new List<string>(table.Columns),
                    };

                    beforeAfterData.BeforeAfterTableColumnDictionary.Add(table.FullTableName, changeDatabaseObject);

                    if (!tableDictionary.ContainsKey(table.FullTableName))
                    {
                        missingTables.Add(table.FullTableName);
                    }
                }

                if (missingTables.Count == 1)
                {
                    beforeAfterData.MissingTableStatusMessage = string.Format(Warning_BAD_Table_Missing, missingTables[0]);
                }
                else if (missingTables.Count > 1)
                {
                    beforeAfterData.MissingTableStatusMessage = string.Format(Warning_BAD_Tables_Missing, string.Join(", ", missingTables.ToArray()));
                }
            }
        }

        private void SetBeforeAfterDataInfo(AuditDatabaseProperties databaseProperties,  Dictionary<string, DatabaseObject> tableDictionary, DatabaseRecord database, ServerRecord server, SqlConnection connection)
        {
            var beforeAfterData = new AuditBeforeAfterData();
            databaseProperties.AuditBeforeAfterData = beforeAfterData;

            try
            {
                
                beforeAfterData.IsAvailable = SetBeforeAfterDataAvailable(beforeAfterData, database, server, connection);

                if (!beforeAfterData.IsAvailable)
                {
                    return;
                }

                if (tableDictionary == null)
                {
                    beforeAfterData.StatusMessaage = Error_CantLoadTables;
                    return;
                }

                beforeAfterData.ClrStatus = AgentManagerHelper.Instance.GetClrStatus(server, connection);
                SetBeforeAfterTableDictionary(beforeAfterData, tableDictionary, database, server, connection);
                
            }
            catch (Exception ex)
            {
                var errorMessage = string.Format("Problem loading before after configuration. Error: {0} ", ex);
                ErrorLog.Instance.Write(
                    ErrorLog.Level.Always,
                    errorMessage,
                    ErrorLog.Severity.Error);

                beforeAfterData.IsAvailable = false;
                beforeAfterData.StatusMessaage = errorMessage;
            }
        }

        private void SetSensitiveColumns(AuditDatabaseProperties databaseProperties, Dictionary<string, DatabaseObject> tableDictionary, DatabaseRecord database, ServerRecord server, SqlConnection connection)
        {
            try
            {
                databaseProperties.SensitiveColumnTableData = new SensitiveColumnTableData();

                if (server.SqlVersion == 0)
                {
                    databaseProperties.SensitiveColumnTableData.StatusMessaage = CoreConstants.Feature_SensitiveColumnNotAvailableVersionUnknown;
                    return;
                }
                else if (!CheckSupportsHelper.SupportsSensitiveColumns(server))
                {
                    databaseProperties.SensitiveColumnTableData.StatusMessaage = CoreConstants.Feature_SensitiveColumnNotAvailableAgent;
                    return;
                }
                if (tableDictionary == null)
                {
                    databaseProperties.SensitiveColumnTableData.StatusMessaage = Error_CantLoadTables;
                    return;
                }

                if (database.AuditSensitiveColumns)
                {
                    databaseProperties.SensitiveColumnTableData.SensitiveTableColumnDictionary = new Dictionary<string, DatabaseObject>();
                    List<string> missingTables = new List<string>();
                    List<SensitiveColumnTableConfiguration> tableConfig = SensitiveColumnTableConfiguration.GetDictAuditedColumns(connection, server.SrvId, database.DbId, server.Instance);
                    bool supportsSchemas = CheckSupportsHelper.SupportsSchemas(server);

                    foreach (SensitiveColumnTableConfiguration table in tableConfig)
                    {
                        SensitiveColumnData sensitiveObject = new SensitiveColumnData();
                        sensitiveObject.Key = table.FullTableName;
                        foreach (KeyValuePair<string, List<string>> pair in table.tableColumnMap)
                        {
                        var columnTable = new DatabaseObject
                        {
                            ServerId = table.SrvId,
                            DatabaseId = table.DbId,
                                ObjectId = table.tableObjectIdMap[pair.Key],
                                TableName = pair.Key.Split('.')[pair.Key.Split('.').Length-1],
                                FullTableName = pair.Key,
                                SchemaName = pair.Key.Split('.')[0],
                                SelectedColumns = table.SelectedColumns,
                                ColumnList = pair.Value,
                                Type = table.Type,
                                ColumnId = table.ColumnId
                        };

                            sensitiveObject.Type = table.Type;

                            if (sensitiveObject.DatasetTableList == null)
                        {
                                sensitiveObject.DatasetTableList = new List<DatabaseObject>();
                        }
                            sensitiveObject.DatasetTableList.Add(columnTable);
                    }

                        if (databaseProperties.SensitiveColumnTableData.SensitiveColumnData == null)
                    {
                            databaseProperties.SensitiveColumnTableData.SensitiveColumnData = new List<SensitiveColumnData>();
                    }
                        databaseProperties.SensitiveColumnTableData.SensitiveColumnData.Add(sensitiveObject);


                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(
                        ErrorLog.Level.Always,
                        string.Format("Problem loading Sensitive Column auditing information. Error: {0} ", ex),
                        ErrorLog.Severity.Error);
            }
        }

        private List<string> GetTalbeNames(DatabaseRecord database, ServerRecord server, SqlConnection connection)
        {
            var tableNameList = new List<string>();
            var oldTables = DatabaseObjectRecord.GetUserTables(database.DbId, connection);
            foreach (DatabaseObjectRecord dbo in oldTables)
            {
                tableNameList.Add(CheckSupportsHelper.SupportsSchemas(server) ? dbo.FullTableName : dbo.TableName);
            }
            return tableNameList;
        }

        private string ValidateProperties(AuditDatabaseProperties databaseProperties, DatabaseRecord database, SqlConnection connection)
        {
            var auditedActivities = databaseProperties.AuditedActivities;

            if (auditedActivities == null)
            {
                return "Audit settings are missed.";
            }

            // audit settings - make sure something checked
            if (!auditedActivities.AuditSecurity &&
                !auditedActivities.AuditDDL &&
                !auditedActivities.AuditAdmin &&
                !auditedActivities.AuditDML &&
                !auditedActivities.AuditSELECT)
            {
                return Error_MustSelectOneAuditOption;
            }

            var dmlSelectFilters = databaseProperties.DmlSelectFilters;
            if (dmlSelectFilters == null)
            {
                return "DML or SELECT filters are missed.";
            }

            if (auditedActivities.AuditDML ||
                auditedActivities.AuditSELECT)
            {

                if (!dmlSelectFilters.AuditDmlAll)
                {

                    switch (dmlSelectFilters.AuditUserTables)
                    {
                        case AuditUserTables.None:

                            if (!dmlSelectFilters.AuditSystemTables &&
                                !dmlSelectFilters.AuditStoredProcedures &&
                                !dmlSelectFilters.AuditDmlOther)
                            {
                                return Error_MustSelectOneAuditObject;
                            }

                            break;

                        case AuditUserTables.Following:

                            if (dmlSelectFilters.UserTableList.Count == 0)
                            {
                                return Error_NoUserTables;
                            }

                            break;
                    }
                }
            }

            var beforeAfterData = databaseProperties.AuditBeforeAfterData;

            if (beforeAfterData != null &&
                beforeAfterData.BeforeAfterTableColumnDictionary != null &&
                beforeAfterData.BeforeAfterTableColumnDictionary.Count > 0)
            {
                if (auditedActivities.AuditDML)
                {
                    if (dmlSelectFilters.AuditUserTables != AuditUserTables.None)
                    {
                        var server = SqlCmRecordReader.GetServerRecord(database.SrvId, connection);
                        var oldTableNameList = GetTalbeNames(database, server, connection);

                        if (oldTableNameList != null &&
                            oldTableNameList.Count > 0)
                        {
                            foreach (var tableName in beforeAfterData.BeforeAfterTableColumnDictionary.Keys)
                            {
                                if (dmlSelectFilters.AuditUserTables == AuditUserTables.Following)
                                {
                                    // if only some tables are audited, then make sure the BAD table is being audited
                                    bool tblIsAudited = false;

                                    foreach (var oldTableName in oldTableNameList)
                                    {
                                        if (tableName == oldTableName)
                                        {
                                            tblIsAudited = true;
                                            break;
                                        }
                                    }

                                    if (!tblIsAudited)
                                    {
                                        return string.Format(Error_BADTableNotAudited, tableName);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        return Error_UserTableAuditingNotEnabled;
                    }
                }
                else
                {
                    return Error_DMLAuditingNotEnabled;
                }
            }

            // privileged users
            if (databaseProperties.PrivilegedRolesAndUsers != null &&
                (databaseProperties.PrivilegedRolesAndUsers.RoleList.Count > 0 || databaseProperties.PrivilegedRolesAndUsers.UserList.Count > 0))
            {

                var privilegedUserActivities = databaseProperties.AuditedPrivilegedUserActivities;

                if (privilegedUserActivities == null)
                {
                    return "Privileged User Activity settings are missed.";
                }

                // make sure something checked
                if (!privilegedUserActivities.AuditLogins &&
                   !privilegedUserActivities.AuditFailedLogins &&
                   !privilegedUserActivities.AuditSecurity &&
                   !privilegedUserActivities.AuditAdmin &&
                   !privilegedUserActivities.AuditDDL &&
                   !privilegedUserActivities.AuditDML &&
                   !privilegedUserActivities.AuditSELECT &&
                   privilegedUserActivities.AuditDefinedEvents)
                {
                    return Error_MustSelectOneAuditUserOption;
                }
            }

            return string.Empty;
        }

        private DatabaseRecord GetDatabaseRecordFromAuditDatabaseProperties(AuditDatabaseProperties databaseProperties, DatabaseRecord oldDatabase, SqlConnection connection)
        {
            var database = oldDatabase.Clone();
            database.Connection = connection;

            // General
            database.Description = databaseProperties.Description;

            // Audit Settings		
            database.AuditDDL = databaseProperties.AuditedActivities.AuditDDL;
            database.AuditSecurity = databaseProperties.AuditedActivities.AuditSecurity;
            database.AuditAdmin = databaseProperties.AuditedActivities.AuditAdmin;
            database.AuditDML = databaseProperties.AuditedActivities.AuditDML;
            database.AuditSELECT = databaseProperties.AuditedActivities.AuditSELECT;
            database.AuditCaptureSQL = databaseProperties.AuditedActivities.AuditCaptureSQL;
            database.AuditCaptureDDL = databaseProperties.AuditedActivities.AuditCaptureDDL;
            database.AuditCaptureTrans = databaseProperties.AuditedActivities.AuditCaptureTrans;
            database.AuditAccessCheck = databaseProperties.AuditedActivities.AuditAccessCheck;

            database.AuditExceptions = oldDatabase.AuditExceptions;

            database.AuditUsersList = AuditServerHelper.GetRolesAndUsersString(databaseProperties.TrustedRolesAndUsers);
            database.AuditPrivUsersList = AuditServerHelper.GetRolesAndUsersString(databaseProperties.PrivilegedRolesAndUsers);


            database.AuditDmlAll = databaseProperties.DmlSelectFilters.AuditDmlAll;
            database.AuditUserTables = Convert.ToInt32(databaseProperties.DmlSelectFilters.AuditUserTables);
            database.AuditSystemTables = databaseProperties.DmlSelectFilters.AuditSystemTables;
            database.AuditStoredProcedures = databaseProperties.DmlSelectFilters.AuditStoredProcedures;
            database.AuditDmlOther = databaseProperties.DmlSelectFilters.AuditDmlOther;

            //Audit Before After Data
            database.AuditDataChanges = databaseProperties.AuditedActivities.AuditDML &&
                                        databaseProperties.AuditBeforeAfterData.BeforeAfterTableColumnDictionary != null &&
                                        databaseProperties.AuditBeforeAfterData.BeforeAfterTableColumnDictionary.Count > 0;

            database.AuditSensitiveColumns = databaseProperties.SensitiveColumnTableData.SensitiveColumnData != null && databaseProperties.SensitiveColumnTableData.SensitiveColumnData.Count != 0;

            //// Privileged users
            database.AuditPrivUsersList = AuditServerHelper.GetRolesAndUsersString(databaseProperties.PrivilegedRolesAndUsers);
            FillDatabaseRecordWithAuditUserActivitySettings(database, databaseProperties.AuditedPrivilegedUserActivities);

            return database;
        }

        private ICollection GetUserTables(DatabaseRecord database, SqlConnection connection)
        {
            ICollection userTables = null;
            var oldAuditUserTables = (AuditUserTables)Enum.ToObject(typeof(AuditUserTables), database.AuditUserTables);

            // user tables
            if (oldAuditUserTables == AuditUserTables.Following)
            {
                // Load User Tables
                userTables = DatabaseObjectRecord.GetUserTables(database.DbId, connection);
            }
            else
            {
                userTables = new ArrayList();
            }

            return userTables;
        }

        private Dictionary<string, DataChangeTableRecord> GetBeforeAfterTableDictionary(DatabaseRecord database, SqlConnection connection)
        {
            var tableDictionary = new Dictionary<string, DataChangeTableRecord>();
            var tables = DataChangeTableRecord.GetAuditedTables(connection, database.SrvId, database.DbId);
            foreach (DataChangeTableRecord table in tables)
            {
                tableDictionary.Add(table.FullTableName, table);
            }
            return tableDictionary;
        }

        private Dictionary<string, SensitiveColumnTableRecord> GetSensitiveColumnTableDictionary(DatabaseRecord database, SqlConnection connection)
        {
            var tableDictionary = new Dictionary<string, SensitiveColumnTableRecord>();
            var tables = SensitiveColumnTableRecord.GetAuditedTables(connection, database.SrvId, database.DbId);
            if (tables != null)
            {
            foreach (var table in tables)
            {
                    if (!tableDictionary.ContainsKey(table.FullTableName))
                    {
                tableDictionary.Add(table.FullTableName, table);
                    }
                }
            }
            return tableDictionary;
        }

        private string GetColumnString(string[] cols)
        {
            if (cols == null || cols.Length == 0)
            {
                return BAD_NoColumns;
            }

            try
            {
                return string.Join(COMMA_CHARACTER, cols);
            }
            catch (Exception)
            {
                return BAD_NoColumns;
            }
        }

        private bool SaveBeforeAfterDataInformation(AuditDatabaseProperties databaseProperties, DatabaseRecord database, DatabaseRecord oldDatabase, SqlTransaction transaction, SqlConnection connection)
        {
            bool isDirty = false;

            if (databaseProperties.AuditBeforeAfterData == null ||
                databaseProperties.AuditBeforeAfterData.BeforeAfterTableColumnDictionary == null ||
                databaseProperties.AuditBeforeAfterData.BeforeAfterTableColumnDictionary.Count == 0)
            {
                return isDirty;
            }

            //Save Before After Data information
            var beforeAfeterTableList = Converter.GetListFromDictionary<DatabaseObject>(databaseProperties.AuditBeforeAfterData.BeforeAfterTableColumnDictionary);
            var selectedBeforeAfterTables = Converter.ConvertReferenceList<DatabaseObject, DataChangeTableRecord>(beforeAfeterTableList,
                table => new DataChangeTableRecord()
                {
                    SrvId = table.ServerId,
                    DbId = table.DatabaseId,
                    ObjectId = table.ObjectId,
                    TableName = table.TableName,
                    SchemaName = table.SchemaName,
                    RowLimit = table.RowLimit,
                    SelectedColumns = table.SelectedColumns,
                    Columns = table.ColumnList == null ? null : table.ColumnList.ToArray(),
                });


            if (oldDatabase.AuditDataChanges != database.AuditDataChanges)
            {
                // Adding new tables
                if (database.AuditDataChanges)
                {


                    DataChangeTableRecord.CreateUserTables(connection, selectedBeforeAfterTables, oldDatabase.SrvId, oldDatabase.DbId, transaction);
                }
                else
                {
                    // Removing old tables
                    DataChangeTableRecord.DeleteUserTables(connection, oldDatabase.SrvId, oldDatabase.DbId, transaction);
                }
            }
            else if (database.AuditDataChanges)
            {
                bool baDirty = false;

                var oldBeforeAfterTableDictionary = GetBeforeAfterTableDictionary(oldDatabase, connection);

                // Make sure our selected tables didn't change
                if (oldBeforeAfterTableDictionary.Count != selectedBeforeAfterTables.Count)
                {
                    baDirty = true;
                }
                else
                {
                    foreach (var table in selectedBeforeAfterTables)
                    {
                        if (oldBeforeAfterTableDictionary.ContainsKey(table.FullTableName))
                        {
                            var oldTable = oldBeforeAfterTableDictionary[table.FullTableName];
                            if (oldTable.RowLimit != table.RowLimit ||
                                oldTable.SelectedColumns != table.SelectedColumns ||
                                oldTable.Columns.Length != table.Columns.Length ||
                                GetColumnString(oldTable.Columns) != GetColumnString(table.Columns))
                            {
                                baDirty = true;
                                break;
                            }
                        }
                        else
                        {
                            baDirty = true;
                            break;
                        }

                    }
                }
                if (baDirty)
                {
                    isDirty = true;
                    DataChangeTableRecord.UpdateUserTables(connection, selectedBeforeAfterTables, oldDatabase.SrvId, oldDatabase.DbId, transaction);
                }
            }

            return isDirty;
        }

        private List<SensitiveColumnTableRecord> ConvertSensitiveColumnTableListToRecordList(List<DatabaseObject> sensitiveColumnTableList)
        {
            return Converter.ConvertReferenceList<DatabaseObject, SensitiveColumnTableRecord>(sensitiveColumnTableList,
                        table => new SensitiveColumnTableRecord()
                        {
                            SchemaName = table.SchemaName,
                            TableName = table.TableName,
                            ObjectId = table.Id,
                            SrvId = table.ServerId,
                            SelectedColumns = table.SelectedColumns,
                            Columns = table.ColumnList == null ? null : table.ColumnList.ToArray(),
                            Type = table.Type,
                            ColumnId = table.ColumnId
                        });
        }

        private List<SensitiveColumnTableRecord> GetSensitiveColumnTableRecordList(SensitiveColumnTableData sensitiveColumnTableData)
        {
            List<SensitiveColumnTableRecord> sensitiveColumnTableList = new List<SensitiveColumnTableRecord>();


            if (sensitiveColumnTableData != null &&
                sensitiveColumnTableData.SensitiveColumnData != null && sensitiveColumnTableData.SensitiveColumnData.Count != 0)
            {
                foreach (SensitiveColumnData x in sensitiveColumnTableData.SensitiveColumnData)
            {
                    List<DatabaseObject> list = x.DatasetTableList;
            
                    if (x.DatasetTableList != null)
                    {
                        foreach (DatabaseObject dor in x.DatasetTableList)
                        {
                            SensitiveColumnTableRecord sctItem = new SensitiveColumnTableRecord();
                            sctItem.SchemaName = dor.SchemaName;
                            sctItem.TableName = dor.TableName;
                            sctItem.ObjectId = dor.ObjectId;
                            sctItem.Type = dor.Type;
                            sctItem.SelectedColumns = dor.SelectedColumns;
                            if (sctItem.SelectedColumns)
                            {
                                sctItem.Columns = dor.ColumnList.ToArray();
                                sctItem.ColumnId = dor.ColumnId;
                            }
                            sensitiveColumnTableList.Add(sctItem);
                        }
                    }
                }
            }

            return sensitiveColumnTableList;
        }

        private bool ProcessSensitiveColumnTableData(ServerRecord server, DatabaseRecord database, SensitiveColumnTableData databaseInfo, SqlConnection connection)
        {
            bool success = false;
            List<SensitiveColumnData> sensitiveColumnTableList = new List<SensitiveColumnData>();
            if (databaseInfo!=null)
            {
                sensitiveColumnTableList = databaseInfo.SensitiveColumnData;
            }

            if (database.AuditSensitiveColumns && sensitiveColumnTableList!=null && databaseInfo!=null)
            {
                SensitiveColumnTableData sensitiveColumnTableData = new SensitiveColumnTableData();
                sensitiveColumnTableData.SensitiveColumnData = sensitiveColumnTableList;
                List<SensitiveColumnTableRecord> scRecords = GetSensitiveColumnTableRecordList(sensitiveColumnTableData);

                //If there are no tables, clear it out.  The point of applying a regulation is to override the current settings.
                if (scRecords.Count == 0)
                {
                    success =
                        SensitiveColumnTableRecord.DeleteUserTables(connection,
                            server.SrvId, database.DbId, null);
                }
                else
                {
                    //add the new tables (this will delete all the old tables from this DB)
                    success =
                        SensitiveColumnTableRecord.CreateUserTables(connection, scRecords,
                            server.SrvId, database.DbId, null);
                }
            }
            else
            {
                success = true;
            }

            return success;
        }

        private List<DataChangeTableRecord> ConvertDataChangeTableListToRecordList(List<DatabaseObject> dataChangeTableList)
        {
            return Converter.ConvertReferenceList<DatabaseObject, DataChangeTableRecord>(dataChangeTableList,
                        table => new DataChangeTableRecord()
                        {
                            SchemaName = table.SchemaName,
                            TableName = table.TableName,
                            ObjectId = table.ObjectId,
                            SrvId = table.ServerId,
                            RowLimit = table.RowLimit,
                            SelectedColumns = table.SelectedColumns,
                            Columns = table.ColumnList == null ? null : table.ColumnList.ToArray(),
                        });
        }
        private bool ProcessBeforeAfterTableData(ServerRecord server, DatabaseRecord database, List<DatabaseObject> dataChangeTableList, SqlConnection connection)
        {
            bool success = false;
            if (database.AuditDataChanges &&
                dataChangeTableList != null)
            {
                List<DataChangeTableRecord> badRecords = ConvertDataChangeTableListToRecordList(dataChangeTableList);
                if (badRecords.Count == 0)
                {
                    success =
                        DataChangeTableRecord.DeleteUserTables(connection,
                            server.SrvId, database.DbId, null);
                }
                else
                {
                    success =
                        DataChangeTableRecord.CreateUserTables(connection, badRecords,
                            server.SrvId, database.DbId, null);
                }
            }
            else
            {
                success = true;
            }
            return success;
        }
        private bool SelectedSensitiveColumnTables(AuditDatabaseProperties databaseProperties, DatabaseRecord database, DatabaseRecord oldDatabase, SqlTransaction transaction, SqlConnection connection, Dictionary<string, SensitiveColumnTableRecord> oldSensitiveColumnTablesDictionary)
        {

            bool isDirty = false;

            if (databaseProperties.SensitiveColumnTableData.SensitiveColumnData == null)
            {
                return isDirty;
            }

            //Save Sensitive column info
            var selectedSensitiveColumnTables = GetSensitiveColumnTableRecordList(databaseProperties.SensitiveColumnTableData);

            if (oldDatabase.AuditSensitiveColumns != database.AuditSensitiveColumns)
            {
                // Adding new tables
                if (database.AuditSensitiveColumns)
                {
                    SensitiveColumnTableRecord.CreateUserTables(connection, selectedSensitiveColumnTables, oldDatabase.SrvId, oldDatabase.DbId, transaction);
                }
                else
                {
                    // Removing old tables
                    SensitiveColumnTableRecord.DeleteUserTables(connection, oldDatabase.SrvId, oldDatabase.DbId, transaction);
                }
            }
            else if (database.AuditSensitiveColumns)
            {
                bool scDirty = false;
                // Make sure our selected tables didn't change
                if (selectedSensitiveColumnTables.Count != oldSensitiveColumnTablesDictionary.Count)
                {
                    scDirty = true;
                }
                else
                {
                    foreach (var table in selectedSensitiveColumnTables)
                    {
                        if (oldSensitiveColumnTablesDictionary.ContainsKey(table.FullTableName))
                        {
                            var oldTable = oldSensitiveColumnTablesDictionary[table.FullTableName];

                            if (oldTable.SelectedColumns != table.SelectedColumns ||
                                oldTable.Columns.Length != table.Columns.Length ||
                                GetColumnString(oldTable.Columns) != GetColumnString(table.Columns))
                            {
                                scDirty = true;
                                break;
                            }

                        }
                        else
                        {
                            scDirty = true;
                            break;
                        }

                    }
                }
                if (scDirty)
                {
                    isDirty = true;
                    SensitiveColumnTableRecord.UpdateUserTables(connection, selectedSensitiveColumnTables, oldDatabase.SrvId, oldDatabase.DbId, transaction);
                }
            }

            return isDirty;
        }

        private void SaveAlwaysOnDatabases(AuditDatabaseProperties databaseProperties, DatabaseRecord database, SqlConnection connection)
        {
            string errorMessage = string.Empty;
            var server = SqlCmRecordReader.GetServerRecord(database.DbId, connection); ;

            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                string.Format("SaveAlwaysOnDatabases: Started for SQL Server {0}", server.Instance), ErrorLog.Severity.Informational);

            IList listDatabaseAlwaysOnDetails = ServerRecord.GetDatabasesWithInAVG(connection, database.Name, database.ReplicaServers, database.AvailGroupName);
            foreach (DatabaseAlwaysOnDetails databaseAlwaysOnDetails in listDatabaseAlwaysOnDetails)
            {
                if (databaseAlwaysOnDetails.isPrimary == 1)
                {
                    continue; //If it is a primary node then it is already saved, need to save only secondary nodes
                }

                var alwaysOnServer = new ServerRecord();
                alwaysOnServer.Connection = connection;
                alwaysOnServer.Read(databaseAlwaysOnDetails.srvId);

                var alwayOnDatabase = new DatabaseRecord();
                alwayOnDatabase.Connection = connection;

                if (!alwayOnDatabase.Read(databaseAlwaysOnDetails.dbId))
                {
                    errorMessage = string.Format("{0} Server Instance {1}", Error_LoadingDatabaseProperties, server.Instance);
                    ErrorLog.Instance.Write(ErrorLog.Level.Always, errorMessage, ErrorLog.Severity.Informational);
                }

                var alwaysOnDatabaseProperties = new AuditDatabaseProperties()
                {
                    DatabaseId = alwayOnDatabase.DbId,
                    ServerInstance = alwaysOnServer.Instance,
                    DatabaseName = alwayOnDatabase.Name,
                    Description = databaseProperties.Description,
                    AuditedActivities = databaseProperties.AuditedActivities,
                    AuditedPrivilegedUserActivities = databaseProperties.AuditedPrivilegedUserActivities,
                    DmlSelectFilters = databaseProperties.DmlSelectFilters,
                    TrustedRolesAndUsers = databaseProperties.TrustedRolesAndUsers,
                    PrivilegedRolesAndUsers = databaseProperties.PrivilegedRolesAndUsers,
                    AuditBeforeAfterData = databaseProperties.AuditBeforeAfterData,
                    SensitiveColumnTableData = databaseProperties.SensitiveColumnTableData,
                };

                errorMessage = UpdateAuditDatabaseProperties(alwaysOnDatabaseProperties, connection);

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    errorMessage = string.Format("Failed to update always on database {0} on server {1}", alwayOnDatabase.Name, alwaysOnServer.Instance);
                    ErrorLog.Instance.Write(ErrorLog.Level.Always, errorMessage, ErrorLog.Severity.Informational);
                }
            }

            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                String.Format("SaveAlwaysOnDatabases: Completed for SQL Server {0}", server.Instance), ErrorLog.Severity.Informational);
        }

        public List<AuditedDatabaseInfo> GetDatabasesForRepositoryInstace(SqlConnection connection)
        {
            var databaseInfoList = new List<AuditedDatabaseInfo>();
            IEnumerable rawDatabaseList = null;

            try
            {
                var agentManager = AgentManagerHelper.Instance.GetAgentManagerProxy(connection);
                rawDatabaseList = agentManager.GetRawUserDatabases(connection.DataSource);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                            String.Format("Loading database for instance '{0}'", connection.DataSource),
                                            ex,
                                            ErrorLog.Severity.Warning);
                rawDatabaseList = null;
            }
            
            // straight connection to SQL Server
            if (rawDatabaseList == null)
            {
                rawDatabaseList = RawSQL.GetUserDatabases(connection);
            }

            if ((rawDatabaseList != null))
            {
                databaseInfoList = Converter.ConvertReferenceList(rawDatabaseList,
                    delegate(RawDatabaseObject rawDatabase)
                    {
                        AuditedDatabaseInfo databaseInfo = null;
                        var database = new DatabaseRecord();
                        database.Connection = connection;
                        if (!SQLRepository.IsSQLsecureOwnedDB(rawDatabase.name, connection))
                        {
                            databaseInfo =
                                new AuditedDatabaseInfo
                                {
                                    Id = rawDatabase.dbid,
                                    Name = rawDatabase.name,
                                    IsEnabled = false,
                                };
                        }

                        return databaseInfo;
                    });
            }

            return databaseInfoList;
        }

        private List<AuditedDatabaseInfo> GetDatabasesForInstace(int serverId, bool skipAuditedDatabases, SqlConnection connection)
        {
            var databaseInfoList = new List<AuditedDatabaseInfo>();
            ServerRecord server = null;
            IEnumerable rawDatabaseList = null;

            try
            {
                server = SqlCmRecordReader.GetServerRecord(serverId, connection);

                if (server.IsDeployed && server.IsRunning)
                {
                    var agentManager = AgentManagerHelper.Instance.GetAgentManagerProxy(connection);
                    rawDatabaseList = agentManager.GetRawUserDatabases(server.Instance);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                            String.Format("Loading database for instance '{0}'", server.Instance),
                                            ex,
                                            ErrorLog.Severity.Warning);
                rawDatabaseList = null;
            }


            // straight connection to SQL Server
            if (server != null &&
                rawDatabaseList == null)
            {
                var sqlServer = new SqlDirect();
                if (sqlServer.OpenConnection(server.Instance))
                {
                    rawDatabaseList = RawSQL.GetUserDatabases(sqlServer.Connection);
                }
            }

            if ((rawDatabaseList != null))
            {
                databaseInfoList = Converter.ConvertReferenceList(rawDatabaseList,
                    delegate(RawDatabaseObject rawDatabase)
                    {
                        AuditedDatabaseInfo databaseInfo = null;
                        var database = new DatabaseRecord();
                        database.Connection = connection;

                        bool includeDatabase = (skipAuditedDatabases && !database.Read(server.Instance, rawDatabase.name)) || !skipAuditedDatabases;

                        if (includeDatabase 
                            && !(SQLRepository.IsSQLsecureOwnedDB(rawDatabase.name, connection))
                            )
                        {
                            databaseInfo =
                                new AuditedDatabaseInfo
                                {
                                    Id = rawDatabase.dbid,
                                    ServerId = serverId,
                                    Name = rawDatabase.name,
                                    IsEnabled = false,
                                };
                        }

                        return databaseInfo;
                    });
            }

            return databaseInfoList;
        }

        private List<string> GetRegulationGuidelineList(DatabaseRecord database)
        {
            var stringList = new List<string>();

            if (database.PCI)
            {
                stringList.Add(Regulation.RegulationType.PCI.ToString());
            }

            if (database.HIPAA)
            {
                stringList.Add(Regulation.RegulationType.HIPAA.ToString());
            }

            if (database.DISA)
            {
                stringList.Add(Regulation.RegulationType.DISA.ToString());
            }

            if (database.NERC)
            {
                stringList.Add(Regulation.RegulationType.NERC.ToString());
            }

            if (database.CIS)
            {
                stringList.Add(Regulation.RegulationType.CIS.ToString());
            }

            if (database.SOX)
            {
                stringList.Add(Regulation.RegulationType.SOX.ToString());
            }

            if (database.FERPA)
            {
                stringList.Add(Regulation.RegulationType.FERPA.ToString());
            }

            if (database.GDPR)
            {
                stringList.Add(Regulation.RegulationType.GDPR.ToString());
            }

            return stringList;
        }

        private string GetStringFromList(List<string> stringList)
        {
            return stringList.Count > 0 ? string.Join(", ", stringList.ToArray()) : NONE_VALUE;
        }

        private string GetRegulationGuidelinesString(DatabaseRecord database)
        {
            var stringList = GetRegulationGuidelineList(database);
            var value = GetStringFromList(stringList);
            return value;
        }

        private string GetDatabaseAuditedActivitiesString(DatabaseRecord database)
        {
            var stringList = GetRegulationGuidelineList(database);

            if (database.AuditSecurity)
            {
                stringList.Add("Security");
            }

            if (database.AuditDDL)
            {
                stringList.Add("DDL");
            }

            if (database.AuditAdmin)
            {
                stringList.Add("Admin");
            }

            if (database.AuditDML)
            {
                if (database.AuditDmlAll)
                {
                    stringList.Add("DML");
                }
                else
                {
                    stringList.Add("DML (filtered)");
                }
            }

            if (database.AuditSELECT)
            {
                stringList.Add("Select");
            }

            if (database.AuditCaptureSQL)
            {
                stringList.Add("Capture SQL");
            }

            if (database.AuditCaptureDDL)
            {
                stringList.Add("Capture DDL");
            }

            if (database.AuditCaptureTrans)
            {
                stringList.Add("Capture Transactions");
            }

            var value = GetStringFromList(stringList);
            return value;
        }

        private string GetBeforeAfterTablesString(DatabaseRecord database, SqlConnection connection)
        {
            var value = NO_TABLES_VALUE;

            if (database.AuditDataChanges)
            {
                var dcTables = DataChangeTableRecord.GetAuditedTables(connection, database.SrvId, database.DbId);
                value = string.Format("{0} tables", dcTables.Count);
            }

            return value;
        }

        private string GetSensitiveColumnsTablesString(DatabaseRecord database, SqlConnection connection)
        {
            var value = NO_TABLES_VALUE;

            if (database.AuditSensitiveColumns)
            {
                var scTables = SensitiveColumnTableRecord.GetAuditedTables(connection, database.SrvId, database.DbId);
                if (scTables == null)
                {
                    value = string.Format("{0} tables", 0);
                }
                else
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
                    value = string.Format("{0} tables", uniqueSCTables.Count);
                }
            }

            return value;
        }

        private string GetTrustedUsersString(DatabaseRecord database, SqlConnection connection)
        {
            var value = NONE_VALUE;

            if (database.AuditUsersList != null && database.AuditUsersList.Length > 0)
            {
                UserList users = new UserList(database.AuditUsersList);
                var userCount = users.Logins.Length + users.ServerRoles.Length;

                if (userCount > 0)
                {
                    value = String.Format("({0})", userCount);
                }
            }

            return value;
        }

        private string GetEventFiltersString(DatabaseRecord database, int serverId, SqlConnection connection)
        {
            var value = NONE_VALUE;
            var server = SqlCmRecordReader.GetServerRecord(serverId, connection);
            var filters = FiltersDal.SelectEventFiltersForServer(connection, server.Instance);
            var conditions = new Dictionary<int, int>();
            var conditionNames = new List<string>();

            // Prune any that have database conditions that don't match us
            var noMatchFilters = new List<EventFilter>();
            foreach (EventFilter filter in filters)
            {
                foreach (EventCondition condition in filter.Conditions)
                {
                    if (condition.FieldId == (int)AlertableEventFields.databaseName &&
                       !condition.Matches(database.Name))
                        noMatchFilters.Add(filter);
                }
            }
            foreach (EventFilter filter in noMatchFilters)
            {
                filters.Remove(filter);
            }

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
                value = String.Format("({0}) - {1}", filters.Count, filterString);
            }

            return value;
        }

        #endregion

        #region Public Methods

        public List<AuditedDatabaseInfo> GetNotAuditedDatabasesForInstace(int serverId, SqlConnection connection)
        {
            var databaseInfoList = new List<AuditedDatabaseInfo>();
            ServerRecord server = null;
            IEnumerable rawDatabaseList = null;
            try
            {
                server = SqlCmRecordReader.GetServerRecord(serverId, connection);
               
                if (server.IsDeployed && server.IsRunning)
                {
                    if (!server.IsHadrEnabled)
                    {
                        var agentManager = AgentManagerHelper.Instance.GetAgentManagerProxy(connection);
                        rawDatabaseList = agentManager.GetRawUserDatabases(server.Instance);
                    }
                    else
                    {
                        var agentManager = AgentManagerHelper.Instance.GetAgentManagerProxy(connection);
                        rawDatabaseList = agentManager.GetRawUserDatabasesForAlwaysOn(server.Instance, null);
                    }          
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                            String.Format("Loading database for instance '{0}'", server.Instance),
                                            ex,
                                            ErrorLog.Severity.Warning);
                rawDatabaseList = null;
            }


            // straight connection to SQL Server
            if (server != null &&
                rawDatabaseList == null)
            {
                var sqlServer = new SqlDirect();
                if (sqlServer.OpenConnection(server.Instance))
                {
                    if (!server.IsHadrEnabled)
                    {
                        rawDatabaseList = RawSQL.GetUserDatabases(sqlServer.Connection);
                    }
                    else 
                    {
                        rawDatabaseList = RawSQL.GetUserDatabasesForAlwaysOn(sqlServer.Connection,server.Instance);
                    }
                    
                }
            }

            if ((rawDatabaseList != null))
            {
                databaseInfoList = Converter.ConvertReferenceList(rawDatabaseList,
                    delegate(RawDatabaseObject rawDatabase)
                    {
                        AuditedDatabaseInfo databaseInfo = null;
                        var database = new DatabaseRecord();
                        database.Connection = connection;

                        if (!database.Read(server.Instance, rawDatabase.name) &&
                           !(server.IsSqlSecureDb && SQLRepository.IsSQLsecureOwnedDB(rawDatabase.name, connection)))
                        {
                            databaseInfo =
                                new AuditedDatabaseInfo
                                {
                                    Id = rawDatabase.dbid,
                                    ServerId = serverId,
                                    Name = rawDatabase.name,
                                    IsEnabled = false,
                                };
                        }

                        return databaseInfo;
                    });
            }
            return databaseInfoList;
        }

        public List<AuditedDatabaseInfo> GetDatabasesForInstance(int serverId, SqlConnection connection)
        {
            return GetDatabasesForInstace(serverId, false, connection);
        }

        public List<AuditedDatabaseInfo> GetAllDatabasesForInstance(int serverId, SqlConnection connection)
        {
            try
            {
                var serverRecord = SqlCmRecordReader.GetServerRecord(serverId, connection);
                    var rawDatabaseList = DatabaseRecord.GetDatabases(connection, serverId);
                    var databaseInfoList = Converter.ConvertReferenceList(rawDatabaseList,
                        delegate(DatabaseRecord database)
                        {
                            return new AuditedDatabaseInfo
                            {
                                Id = database.DbId,
                                ServerId = serverId,
                                Name = database.Name,
                            IsEnabled = serverRecord.IsEnabled && database.IsEnabled,
                            };

                        });

                    return databaseInfoList;
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                            String.Format("Loading audited databases for server id '{0}'", serverId),
                                            ex,
                                            ErrorLog.Severity.Warning);
                return null;
            }
        }
                
        public List<string> GetEventsDatabasesForInstance(int serverId, SqlConnection connection)
        {
            List<string> dbList = new List<string>();
            try
            {
                var serverRecord = SqlCmRecordReader.GetServerRecord(serverId, connection);
                string cmdstr = String.Format("select distinct databaseName from [{0}]..Events", serverRecord.EventDatabase);
                using (SqlCommand cmd = new SqlCommand(cmdstr, connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if(!reader.IsDBNull(0) && reader.GetString(0) != string.Empty)
                                dbList.Add(reader.GetString(0));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                            String.Format("Loading databases from events table for server id '{0}'", serverId),
                                            ex,
                                            ErrorLog.Severity.Warning);
            }
            return dbList;
        }

        public List<AuditedDatabaseInfo> GetNotRegisteredDatabasesForInstance(int serverId, SqlConnection connection)
        {
            var allDatabaseInfoList = GetNotAuditedDatabasesForInstace(serverId, connection);
            allDatabaseInfoList.AddRange(GetSystemDatabasesForInstace(serverId, connection));
            var sortedDatabaseInfoList = allDatabaseInfoList.OrderBy(info => info.Name).ToList();
            return sortedDatabaseInfoList;
        }

        public void AddDatabases(AuditDatabaseSettings auditSeetings, SqlConnection connection)
        {
            if (auditSeetings == null)
            {
                LogAndThrowException("Failed to add databases because audit settings are not provided. it can't be null.");
            }

            if (auditSeetings.UpdateServerSettings)
            {
                AuditServerHelper.Instance.SaveServerAuditSettings(auditSeetings, connection);
            }

            if (auditSeetings.CollectionLevel == AuditCollectionLevel.Regulation && auditSeetings.IsServerType) 
            {
                AuditServerHelper.Instance.SaveServerRegulationAuditSetting(auditSeetings, connection);
            }
            if (auditSeetings.DatabaseList == null ||
                auditSeetings.DatabaseList.Count == 0)
            {
                return;
            }

            var config = SqlCmConfigurationHelper.GetConfiguration(connection);
            var server = SqlCmRecordReader.GetServerRecord(auditSeetings.DatabaseList[0].ServerId, connection);

            foreach (var databaseInfo in auditSeetings.DatabaseList)
            {
                bool wasDatabaseAdded = false;
                bool wasDatabaseBeforeAfterAdded = false;
                bool isDirty = false;
                var database = CreateDatabaseRecord(databaseInfo, auditSeetings, server, config, connection);
                try
                {
                    if (WriteDatabaseRecord(database, connection))
                    {
                        isDirty = true;
                        wasDatabaseAdded = ProcessSensitiveColumnTableData(server, database, databaseInfo.SensitiveTableColumnData, connection);
                        wasDatabaseBeforeAfterAdded = ProcessBeforeAfterTableData(server, database, databaseInfo.BeforeAfterTableList, connection);
                    }
                }
                catch (Exception ex)
                {
                    ErrorLog.Instance.Write(
                           ErrorLog.Level.Verbose,
                           String.Format("Adding database '{0}' : ", databaseInfo.Name),
                           ex,
                           ErrorLog.Severity.Warning);

                    wasDatabaseAdded = false;
                    wasDatabaseBeforeAfterAdded = false;
                }
                finally
                {
                    if (wasDatabaseAdded && wasDatabaseBeforeAfterAdded && isDirty)
                    {
                        ServerUpdate.RegisterChange(database.SrvId,
                                                    LogType.NewDatabase,
                                                    database.SrvInstance,
                                                    Snapshot.DatabaseSnapshot(connection, database.DbId, database.Name, true), 
                                                    connection);
                    }
                }
            }
        }
        public AuditActivity GetRegulationTemplatesForDatabase(AuditRegulationSettings auditSettings, SqlConnection connection)
        {
            AuditActivity databaseAuditActivity = null;
            if (auditSettings == null)
            {
                LogAndThrowException("Failed to add databases because audit settings are not provided. it can't be null.");
            }
            if (auditSettings == null)
            {
                return null;
            }
            databaseAuditActivity = GetDatabaseTemplateRecord(auditSettings, connection);
            return databaseAuditActivity;
        }

        public bool RemoveDatabase(RemoveDatabaseRequest removeDatabaseRequest, SqlConnection connection)
        {
            var database = SqlCmRecordReader.GetDatabaseRecord(removeDatabaseRequest.DatabaseId, connection);
            return RemoveDatabase(database, connection);
        }

        public void EnableAuditingForDatabases(EnableAuditForDatabases databases, SqlConnection connection)
        {
            LogType logType = databases.Enable ? LogType.EnableDatabase : LogType.DisableDatabase;
            string actionString = databases.Enable ? "enable" : "disable";

            foreach (var databaseId in databases.DatabaseIdList)
            {
                var database = SqlCmRecordReader.GetDatabaseRecord(databaseId, connection);

                if (database.Enable(databases.Enable))
                {
                    ServerUpdate.RegisterChange(database.SrvId, logType, database.SrvInstance, database.Name, connection);
                }
                else
                {

                   ErrorLog.Instance.Write(
                               ErrorLog.Level.Always,
                               string.Format("Failed to {0} database with id = {1}. Error: {2} ", actionString ,database.DbId, DatabaseRecord.GetLastError()),
                               ErrorLog.Severity.Error);
                }
            }
        }

        public AuditDatabaseProperties GetAuditDatabaseProperties(int databaseId, SqlConnection connection)
        {
            AuditDatabaseProperties databaseProperties = null;
            var database = new DatabaseRecord();
            database.Connection = connection;

            if (database.Read(databaseId))
            {
                databaseProperties = new AuditDatabaseProperties();
                databaseProperties.DatabaseId = database.DbId;
                var server = SqlCmRecordReader.GetServerRecord(database.SrvId, connection);

                /// General Properties
                SetaGeneralProperties(databaseProperties, database);

                //// Audit Settings
                SetAuditSettings(databaseProperties, database, server);
                
                // user tables
                SetAuditUserTables(databaseProperties, database, server, connection);
               
                //// Tab: Audited Users
                SetPrivilegedUsers(databaseProperties, database);

                // Tab: Privileged User Auditing
                SetPrivilegedUserAuditSettings(databaseProperties, database, server);

                // Trusted Users
                SetTrustedUsers(databaseProperties, database, server);

                var tableDictionary = AgentManagerHelper.Instance.GetTableDictionary(database, server, connection);

                // Before-After Data
                SetBeforeAfterDataInfo(databaseProperties, tableDictionary, database, server, connection);

                // Sensitive Columns
                SetSensitiveColumns(databaseProperties, tableDictionary, database, server, connection);
            }
            else
            {
                ErrorLog.Instance.Write(
                            ErrorLog.Level.Always,
                            string.Format("Failed to get properties of audited database with id = {0}. Error: {1} ", database.DbId, DatabaseRecord.GetLastError()),
                            ErrorLog.Severity.Error);
            }

            return databaseProperties;
        }

        public string UpdateAuditDatabaseProperties(AuditDatabaseProperties databaseProperties, SqlConnection connection)
        {
            var oldDatabase = SqlCmRecordReader.GetDatabaseRecord(databaseProperties.DatabaseId, connection);
            string errorMessage = ValidateProperties(databaseProperties, oldDatabase, connection);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                return errorMessage;
            }

            var database = GetDatabaseRecordFromAuditDatabaseProperties(databaseProperties, oldDatabase, connection);
            var oldTables = GetUserTables(database, connection);
            var oldTablesSnapshot = Snapshot.GetDatabaseTables(connection, oldDatabase.DbId, "\t\t");
            var oldDCTablesSnapshot = Snapshot.GetDataChangeTables(connection, oldDatabase.DbId, "\t\t");
            var oldSCTablesSnapshot = Snapshot.GetSensitiveColumnTables(connection, oldDatabase.DbId, "\t\t");
            var oldSensitiveColumnTablesDictionary = GetSensitiveColumnTableDictionary(oldDatabase, connection);

            using (var transaction = connection.BeginTransaction())
            {
                bool retval = false;
                bool isDirty = false;
                bool wasException = false;

                try
                {
                    //---------------------------------------
                    // Write Database Properties if necessary
                    //---------------------------------------
                    if (!DatabaseRecord.Match(database, oldDatabase))
                    {
                        if (!database.Write(oldDatabase, transaction))
                        {
                            errorMessage = DatabaseRecord.GetLastError();
                            throw new Exception(errorMessage);
                        }
                        else
                        {
                            isDirty = true;
                        }
                    }

                    //---------------------------
                    // Write Tables if necessary
                    //---------------------------
                    if (databaseProperties.DmlSelectFilters.AuditUserTables != AuditUserTables.Following)
                    {
                        var oldAuditUserTables = (AuditUserTables) Enum.ToObject(typeof (AuditUserTables), oldDatabase.AuditUserTables);

                        if (oldAuditUserTables == AuditUserTables.Following)
                        {
                            isDirty = true;

                            // Delete existing tables - changed from selected to all or none
                            retval = DatabaseObjectRecord.DeleteUserTables(database.DbId, transaction, connection);
                        }
                        else
                        {
                            retval = true;
                        }                        
                    }
                    else
                    {
                        bool same = true;

                        if (oldTables == null || 
                            databaseProperties.DmlSelectFilters.UserTableList.Count != oldTables.Count)
                        {
                            same = false;
                        }
                        else
                        {
                            int index = 0;
                            foreach (DatabaseObjectRecord dbo in oldTables)
                            {
                                if (databaseProperties.DmlSelectFilters.UserTableList[index].FullTableName != dbo.FullTableName)
                                {
                                    same = false;
                                    break;
                                }

                                index++;

                            }
                        }

                        if (same)
                        {
                            retval = true;
                        }
                        else
                        {
                            isDirty = true;
}
                            var tables = Converter.ConvertReferenceList<DatabaseObject, DatabaseObjectRecord>(databaseProperties.DmlSelectFilters.UserTableList,
                            databaseObject => new DatabaseObjectRecord()
                            {
                                Id = databaseObject.Id,
                                ObjectId = databaseObject.ObjectId,
                                DbId = databaseObject.DatabaseId,
                                ObjectType = Convert.ToInt32(databaseObject.ObjectType),
                                TableName = databaseObject.TableName,
                                SchemaName = databaseObject.SchemaName,
                            });

                            if (DatabaseObjectRecord.UpdateUserTables(tables, oldTables.Count, database.DbId, transaction, connection))
                            {
                                retval = true;
                            }
                        }


                    if (SaveBeforeAfterDataInformation(databaseProperties, database, oldDatabase, transaction, connection))
                    {
                        isDirty = true;
                    }

                    if (SelectedSensitiveColumnTables(databaseProperties, database, oldDatabase, transaction, connection, oldSensitiveColumnTablesDictionary))
                    {
                        isDirty = true;
                    }

                    if (!retval)
                    {
                        errorMessage = DatabaseObjectRecord.GetLastError();
                    }

                }
                catch (Exception ex)
                {
                    errorMessage = string.Format("{0} Error: {1}", Error_ErrorSavingDatabase, ex);
                    wasException = true;
                }
                finally
                {
                    //-----------------------------------------------------------
                    // Cleanup - Close transaction, update server, display error
                    //-----------------------------------------------------------
                    if (transaction != null)
                    {
                        if (retval && isDirty)
                        {
                            transaction.Commit();

                            string changeLog = Snapshot.DatabaseChangeLog(connection, oldDatabase, database, oldTablesSnapshot, oldDCTablesSnapshot, oldSCTablesSnapshot);

                            // Register change to server and perform audit log				      
                            ServerUpdate.RegisterChange(database.SrvId, LogType.ModifyDatabase, database.SrvInstance, changeLog, connection);

                            SaveAlwaysOnDatabases(databaseProperties, database, connection);
                        }
                        else
                        {
                            transaction.Rollback();
                        }
                    }
                    if (!retval && !wasException)
                    {
                        errorMessage = Error_ErrorSavingDatabase;
                    }
                }
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Always, errorMessage, ErrorLog.Severity.Error);                
            }

            return errorMessage;
        }

        /// <summary>
        /// This functionality was written based on UpdateAuditSettings() of Idera.SQLcompliance.Application.GUI.Controls.DatabaseSummary
        /// so if you do any changes here, you should do the similar changes in the UpdateAuditSettings() of Idera.SQLcompliance.Application.GUI.Controls.DatabaseSummary
        /// </summary>
        public AuditedDatabaseActivityResult GetAuditedActivityForDatabase(int databaseId, int serverId, SqlConnection connection)
        {
            var result = new AuditedDatabaseActivityResult();
            var database = SqlCmRecordReader.GetDatabaseRecord(databaseId, connection);
            result.Id = database.DbId;
            result.Name = database.Name;
            result.Instance = database.SrvInstance;
            result.IsEnabled = database.IsEnabled;
            result.RegulationGuidelinesString = GetRegulationGuidelinesString(database);
            result.DatabaseAuditedActivitiesString = GetDatabaseAuditedActivitiesString(database);
            result.beforeAfterTablesString = GetBeforeAfterTablesString(database, connection);
            result.SensitiveColumnsTablesString = GetSensitiveColumnsTablesString(database, connection);
            result.TrustedUsersString = GetTrustedUsersString(database, connection);
            result.EventFiltersString = GetEventFiltersString(database, serverId, connection);
            return result;
        }
        #endregion

        #region  Regulation Guideline template
        public AuditActivity GetRegulationSettingsForDatabase(AuditRegulationSettings regulationSettings, SqlConnection connection)
        {
            RegulationSettings settings;
            var regulationCategoryDictionary = RegulationSettingsHelper.Instance.GetCategoryDictionary(connection);
            AuditActivity tempDb = new AuditActivity();
            if (regulationSettings.PCI)
            {
                if (regulationCategoryDictionary.TryGetValue((int)Regulation.RegulationType.PCI, out settings))
                {
                    tempDb.AuditDDL = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) || tempDb.AuditDDL;
                    tempDb.AuditSecurity = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) == (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) || tempDb.AuditSecurity;
                    tempDb.AuditAdmin = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) == (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) || tempDb.AuditAdmin;
                    tempDb.AuditDML = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) || tempDb.AuditDML;
                    tempDb.AuditSELECT = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Select) == (int)RegulationSettings.RegulationDatabaseCategory.Select) || tempDb.AuditSELECT;
                    if ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess || (tempDb.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempDb.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess || (tempDb.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempDb.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else
                        tempDb.AuditAccessCheck = AccessCheckFilter.NoFilter;
                    tempDb.AuditCaptureSQL = CoreConstants.AllowCaptureSql && ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SQLText) == (int)RegulationSettings.RegulationDatabaseCategory.SQLText) || tempDb.AuditCaptureSQL; ;
                    tempDb.AuditCaptureTrans = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Transactions) == (int)RegulationSettings.RegulationDatabaseCategory.Transactions) || tempDb.AuditCaptureTrans; ;
                    tempDb.AuditSensitiveColumns = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) == (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) || tempDb.AuditSensitiveColumns;
                    tempDb.AuditBeforeAfter = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange) == (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange) || tempDb.AuditBeforeAfter;
                    tempDb.AuditPrivilegedUsers = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.PrivelegedUsers) == (int)RegulationSettings.RegulationDatabaseCategory.PrivelegedUsers) || tempDb.AuditPrivilegedUsers;
                }
            }
            if (regulationSettings.HIPAA)
            {
                if (regulationCategoryDictionary.TryGetValue((int)Regulation.RegulationType.HIPAA, out settings))
                {
                    tempDb.AuditDDL = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) || tempDb.AuditDDL;
                    tempDb.AuditSecurity = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) == (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) || tempDb.AuditSecurity;
                    tempDb.AuditAdmin = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) == (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) || tempDb.AuditAdmin;
                    tempDb.AuditDML = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) || tempDb.AuditDML;
                    tempDb.AuditSELECT = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Select) == (int)RegulationSettings.RegulationDatabaseCategory.Select) || tempDb.AuditSELECT;
                    if (((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) || (tempDb.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempDb.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if (((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) || (tempDb.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempDb.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else
                        tempDb.AuditAccessCheck = AccessCheckFilter.NoFilter;
                    tempDb.AuditCaptureSQL = (CoreConstants.AllowCaptureSql && ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SQLText) == (int)RegulationSettings.RegulationDatabaseCategory.SQLText)) || tempDb.AuditCaptureSQL;
                    tempDb.AuditCaptureTrans = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Transactions) == (int)RegulationSettings.RegulationDatabaseCategory.Transactions) || tempDb.AuditCaptureTrans;
                    tempDb.AuditSensitiveColumns = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) == (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) || tempDb.AuditSensitiveColumns;
                    tempDb.AuditBeforeAfter = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange) == (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange) || tempDb.AuditBeforeAfter;
                    tempDb.AuditPrivilegedUsers = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.PrivelegedUsers) == (int)RegulationSettings.RegulationDatabaseCategory.PrivelegedUsers) || tempDb.AuditPrivilegedUsers;
                }
            }
            if (regulationSettings.DISA)
            {
                if (regulationCategoryDictionary.TryGetValue((int)Regulation.RegulationType.DISA, out settings))
                {
                    tempDb.AuditDDL = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) || tempDb.AuditDDL;
                    tempDb.AuditSecurity = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) == (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) || tempDb.AuditSecurity;
                    tempDb.AuditAdmin = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) == (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) || tempDb.AuditAdmin;
                    tempDb.AuditDML = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) || tempDb.AuditDML;
                    tempDb.AuditSELECT = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Select) == (int)RegulationSettings.RegulationDatabaseCategory.Select) || tempDb.AuditSELECT;
                    if (((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) || (tempDb.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempDb.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if (((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) || (tempDb.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempDb.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else
                        tempDb.AuditAccessCheck = AccessCheckFilter.NoFilter;
                    tempDb.AuditCaptureSQL = (CoreConstants.AllowCaptureSql && ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SQLText) == (int)RegulationSettings.RegulationDatabaseCategory.SQLText)) || tempDb.AuditCaptureSQL;
                    tempDb.AuditCaptureTrans = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Transactions) == (int)RegulationSettings.RegulationDatabaseCategory.Transactions) || tempDb.AuditCaptureTrans;
                    tempDb.AuditSensitiveColumns = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) == (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) || tempDb.AuditSensitiveColumns;
                    tempDb.AuditBeforeAfter = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange) == (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange) || tempDb.AuditBeforeAfter;
                    tempDb.AuditPrivilegedUsers = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.PrivelegedUsers) == (int)RegulationSettings.RegulationDatabaseCategory.PrivelegedUsers) || tempDb.AuditPrivilegedUsers;
                }
            }
            if (regulationSettings.NERC)
            {
                if (regulationCategoryDictionary.TryGetValue((int)Regulation.RegulationType.NERC, out settings))
                {
                    tempDb.AuditDDL = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) || tempDb.AuditDDL;
                    tempDb.AuditSecurity = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) == (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) || tempDb.AuditSecurity;
                    tempDb.AuditAdmin = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) == (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) || tempDb.AuditAdmin;
                    tempDb.AuditDML = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) || tempDb.AuditDML;
                    tempDb.AuditSELECT = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Select) == (int)RegulationSettings.RegulationDatabaseCategory.Select) || tempDb.AuditSELECT;
                    if (((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) || (tempDb.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempDb.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if (((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) || (tempDb.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempDb.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else
                        tempDb.AuditAccessCheck = AccessCheckFilter.NoFilter;
                    tempDb.AuditCaptureSQL = (CoreConstants.AllowCaptureSql && ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SQLText) == (int)RegulationSettings.RegulationDatabaseCategory.SQLText)) || tempDb.AuditCaptureSQL;
                    tempDb.AuditCaptureTrans = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Transactions) == (int)RegulationSettings.RegulationDatabaseCategory.Transactions) || tempDb.AuditCaptureTrans;
                    tempDb.AuditSensitiveColumns = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) == (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) || tempDb.AuditSensitiveColumns;
                    tempDb.AuditBeforeAfter = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange) == (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange) || tempDb.AuditBeforeAfter;
                    tempDb.AuditPrivilegedUsers = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.PrivelegedUsers) == (int)RegulationSettings.RegulationDatabaseCategory.PrivelegedUsers) || tempDb.AuditPrivilegedUsers;
                }
            }
            if (regulationSettings.CIS)
            {
                
                if (!(regulationSettings.PCI || regulationSettings.HIPAA || regulationSettings.DISA || regulationSettings.NERC || regulationSettings.SOX || regulationSettings.FERPA))
                {
                    tempDb.AuditDDL = true;
                    tempDb.AuditSecurity = true;
                    tempDb.AuditAdmin = true;
                    tempDb.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                }
            }
            if (regulationSettings.SOX)
            {
                if (regulationCategoryDictionary.TryGetValue((int)Regulation.RegulationType.SOX, out settings))
                {
                    tempDb.AuditDDL = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) || tempDb.AuditDDL;
                    tempDb.AuditSecurity = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) == (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) || tempDb.AuditSecurity;
                    tempDb.AuditAdmin = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) == (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) || tempDb.AuditAdmin;
                    tempDb.AuditDML = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) || tempDb.AuditDML;
                    tempDb.AuditSELECT = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Select) == (int)RegulationSettings.RegulationDatabaseCategory.Select) || tempDb.AuditSELECT;
                    if (((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) || (tempDb.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempDb.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if (((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) || (tempDb.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempDb.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else
                        tempDb.AuditAccessCheck = AccessCheckFilter.NoFilter;
                    tempDb.AuditCaptureSQL = (CoreConstants.AllowCaptureSql && ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SQLText) == (int)RegulationSettings.RegulationDatabaseCategory.SQLText)) || tempDb.AuditCaptureSQL;
                    tempDb.AuditCaptureTrans = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Transactions) == (int)RegulationSettings.RegulationDatabaseCategory.Transactions) || tempDb.AuditCaptureTrans;
                    tempDb.AuditSensitiveColumns = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) == (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) || tempDb.AuditSensitiveColumns;
                    tempDb.AuditBeforeAfter = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange) == (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange) || tempDb.AuditBeforeAfter;
                    tempDb.AuditPrivilegedUsers = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.PrivelegedUsers) == (int)RegulationSettings.RegulationDatabaseCategory.PrivelegedUsers) || tempDb.AuditPrivilegedUsers;
                }
            }
            if (regulationSettings.FERPA)
            {
                if (regulationCategoryDictionary.TryGetValue((int)Regulation.RegulationType.FERPA, out settings))
                {
                    tempDb.AuditDDL = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) || tempDb.AuditDDL;
                    tempDb.AuditSecurity = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) == (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) || tempDb.AuditSecurity;
                    tempDb.AuditAdmin = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) == (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) || tempDb.AuditAdmin;
                    tempDb.AuditDML = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) || tempDb.AuditDML;
                    tempDb.AuditSELECT = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Select) == (int)RegulationSettings.RegulationDatabaseCategory.Select) || tempDb.AuditSELECT;
                    if (((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) || (tempDb.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempDb.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if (((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) || (tempDb.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempDb.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else
                        tempDb.AuditAccessCheck = AccessCheckFilter.NoFilter;
                    tempDb.AuditCaptureSQL = (CoreConstants.AllowCaptureSql && ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SQLText) == (int)RegulationSettings.RegulationDatabaseCategory.SQLText)) || tempDb.AuditCaptureSQL;
                    tempDb.AuditCaptureTrans = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Transactions) == (int)RegulationSettings.RegulationDatabaseCategory.Transactions) || tempDb.AuditCaptureTrans;
                    tempDb.AuditSensitiveColumns = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) == (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) || tempDb.AuditSensitiveColumns;
                    tempDb.AuditBeforeAfter = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange) == (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange) || tempDb.AuditBeforeAfter;
                    tempDb.AuditPrivilegedUsers = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.PrivelegedUsers) == (int)RegulationSettings.RegulationDatabaseCategory.PrivelegedUsers) || tempDb.AuditPrivilegedUsers;
                }
            }
            if (regulationSettings.GDPR)
            {
                if (regulationCategoryDictionary.TryGetValue((int)Regulation.RegulationType.GDPR, out settings))
                {
                    tempDb.AuditDDL = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) || tempDb.AuditDDL;
                    tempDb.AuditSecurity = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) == (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) || tempDb.AuditSecurity;
                    tempDb.AuditAdmin = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) == (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) || tempDb.AuditAdmin;
                    tempDb.AuditDML = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) || tempDb.AuditDML;
                    tempDb.AuditSELECT = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Select) == (int)RegulationSettings.RegulationDatabaseCategory.Select) || tempDb.AuditSELECT;
                    if (((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) || (tempDb.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempDb.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if (((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) || (tempDb.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempDb.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else
                        tempDb.AuditAccessCheck = AccessCheckFilter.NoFilter;
                    tempDb.AuditCaptureSQL = (CoreConstants.AllowCaptureSql && ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SQLText) == (int)RegulationSettings.RegulationDatabaseCategory.SQLText)) || tempDb.AuditCaptureSQL;
                    tempDb.AuditCaptureTrans = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Transactions) == (int)RegulationSettings.RegulationDatabaseCategory.Transactions) || tempDb.AuditCaptureTrans;
                    tempDb.AuditSensitiveColumns = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) == (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) || tempDb.AuditSensitiveColumns;
                    tempDb.AuditBeforeAfter = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange) == (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange) || tempDb.AuditBeforeAfter;
                    tempDb.AuditPrivilegedUsers = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.PrivelegedUsers) == (int)RegulationSettings.RegulationDatabaseCategory.PrivelegedUsers) || tempDb.AuditPrivilegedUsers;
                }
            }
            return tempDb;
        }
        #endregion
        public class SensitiveColumnTableConfiguration
        {
            #region Fields
            public Dictionary<string, List<string>> tableColumnMap = new Dictionary<string, List<string>>();
            public Dictionary<string, int> tableObjectIdMap = new Dictionary<string, int>();
            public int ColumnId { get; set; }
            public bool SelectedColumns { get; set; }
            #endregion
            #region Properties
            public int SrvId
            {
                get;
                set;
            }
            public int DbId
            {
                get;
                set;
            }
            public string Schema
            {
                get;
                set;
            }
            public string Name
            {
                get;
                set;
            }
            public string Type
            {
                get;
                set;
            }
            public string FullTableName
            {
                get
                {
                    return String.Join(",",tableObjectIdMap.Keys);
                }
            }
            #endregion
            public static List<SensitiveColumnTableConfiguration> GetDictAuditedColumns(SqlConnection connection, int instanceId, int databaseId, string instanceName)
            {
                List<SensitiveColumnTableConfiguration> columnTablesConfig = new List<SensitiveColumnTableConfiguration>();
                List<SensitiveColumnsTableRecord> colList = new List<SensitiveColumnsTableRecord>();
                string stmt = String.Format(@"SELECT 
                                             sc.srvId, db.sqlDatabaseId, st.objectId,
                                             sc.name, sc.columnId, sc.type, st.tableName,
                                             st.schemaName FROM SensitiveColumnTables st JOIN Databases db on 
                                            (db.dbId = st.dbId AND db.srvInstance = '{2}') JOIN SensitiveColumnColumns sc
                                            ON st.objectId = sc.objectId and st.dbId = sc.dbId where db.dbId = {3} union 
                                            (SELECT st.srvId,db.sqlDatabaseId,st.objectId,null,null,'Individual',st.tableName,st.schemaName 
                                            from SensitiveColumnTables as st JOIN Databases db on (db.dbId = st.dbId AND db.srvInstance = '{2}') 
                                            where selectedColumns = 0 AND db.dbId = {3}) ORDER BY objectId ASC",
                                             CoreConstants.RepositorySensitiveColumnColumnsTable,
                                             CoreConstants.RepositorySensitiveColumnTablesTable,
                                             instanceName,
                                             databaseId);
                try
                {
                    using (SqlCommand cmd = new SqlCommand(stmt, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                SensitiveColumnsTableRecord col = new SensitiveColumnsTableRecord();
                                col.Load(reader);
                                colList.Add(col);
                            }
                        }
                    }
                }
                catch
                {
                    return null;
                }
                finally
                {
                }
                var groupedList = colList.FindAll(x => x.Type != null && x.Type != CoreConstants.IndividualColumnType )
                    .GroupBy(u => u.ColumnId)
                    .Select(grp => grp.ToList())
                    .ToList();
                groupedList.AddRange(colList.FindAll(x => x.Type == null || x.Type 
                    == CoreConstants.IndividualColumnType).GroupBy(u => u.ObjectId)
                    .Select(grp => grp.ToList()).ToList());
                foreach (var group in groupedList)
                {
                    SensitiveColumnTableConfiguration colConfig = new SensitiveColumnTableConfiguration();
                    List<string> tblNameOfColumn = new List<string>();
                    List<string> columns = new List<string>();
                    foreach (var user in group)
                    {
                        if (user.Type != null)
                        {
                            colConfig.Type = user.Type;
                        }
                        else
                        {
                            colConfig.Type = "Individual";
                        }
                        colConfig.DbId = user.DbId;
                        colConfig.SrvId = user.SrvId;
                        colConfig.Schema = user.SchemaName;
                        string fullTableName = String.Format("{0}.{1}", user.SchemaName, user.TableName);
                        colConfig.tableObjectIdMap[fullTableName] = user.ObjectId;
                        //colConfig.ObjectId = user.ObjectId;
                        colConfig.ColumnId = user.ColumnId;
                        tblNameOfColumn.Add(user.TableName);
                        if (user.Name != null)
                        {
                            colConfig.SelectedColumns = true;
                            columns.Add(user.Name);
                        }
                        else
                        {
                            colConfig.SelectedColumns = false;
                        }

                        if (colConfig.tableColumnMap.ContainsKey(fullTableName))
                        {
                            colConfig.tableColumnMap[fullTableName].Add(user.Name);
                        }
                        else if (user.Name != null)
                        {
                            colConfig.tableColumnMap.Add(fullTableName, new List<string>() { user.Name });
                        }
                        else
                        {
                            colConfig.tableColumnMap.Add(fullTableName, new List<string>());
                        }
                    }
                    var unique_items = new HashSet<string>(tblNameOfColumn);
                    colConfig.Name = string.Join(",", unique_items);//builder.ToString();
                    columnTablesConfig.Add(colConfig);
                }
                return columnTablesConfig;
            }
        }
    }
}
