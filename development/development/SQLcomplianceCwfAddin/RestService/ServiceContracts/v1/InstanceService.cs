using System;
using System.Collections.Generic;
using System.Linq;
using Idera.SQLcompliance.Core;
using SQLcomplianceCwfAddin.Helpers;
using SQLcomplianceCwfAddin.Helpers.Agent;
using SQLcomplianceCwfAddin.Helpers.SQL;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.ManagedInstances;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.ManagedInstances.Credentials;
using Idera.SQLcompliance.Core.Templates;
using System.Data.SqlClient;
using Idera.SQLcompliance.Core.Agent;
using System.Collections;
using System.Text;
using Microsoft.VisualBasic;
using Idera.SQLcompliance;

namespace SQLcomplianceCwfAddin.RestService.ServiceContracts.v1
{
    public partial class RestService
    {

        private readonly ServerRecord _server;
        public DatabaseRecord _db;
        private DatabaseRecord _oldDb;
        private ICollection _tableList;

        public IEnumerable<AuditedServerStatus> GetFilteredAuditedInstancesStatus(FilteredRegisteredInstancesStatusRequest statusRequest)
        {
            using (_logger.InfoCall("GetFilteredAuditedInstancesStatus"))
            {
                var result = QueryExecutor.Instance.GetFilteredInstancesStatuses(GetConnection(), statusRequest);
                return result;
            }
        }

        public AuditedServerStatus GetAuditedSqlServer(int id, int days)
        {
            using (_logger.InfoCall("GetAuditedSqlServer"))
            {
                var result = GetFilteredAuditedInstancesStatus(new FilteredRegisteredInstancesStatusRequest() { ServerId = id, Days = days, UpdateStatisticProperties = true });
                var auditedServerStatuses = result as IList<AuditedServerStatus> ?? result.ToList();
                var serverStatus = auditedServerStatuses.FirstOrDefault();
                if (serverStatus == null) throw new Exception("Server not found. It may be deleted by another user.");
                return serverStatus;
            }
        }

        public ManagedInstanceResponce GetManagedInstances(PaginationRequest request)
        {
            using (_logger.InfoCall("GetManagedInstances"))
            {
                using (var connection = GetConnection())
                {
                    var query = QueryBuilder.Instance.GetManagedInstances();
                    var result = QueryExecutor.Instance.GetManagedInstances(connection, query, request);
                    return result;
                }
            }
        }

        public ManagedInstanceForEditResponce GetManagedInstance(int id)
        {
            using (_logger.InfoCall("GetManagedInstance"))
            {
                using (var connection = GetConnection())
                {
                    var query = QueryBuilder.Instance.GetOwnersAndLocations();
                    var managedInstanceForEditResponce = QueryExecutor.Instance.GetOwnersAndLocations(connection, query);

                    query = QueryBuilder.Instance.GetManagedInstance();
                    managedInstanceForEditResponce.ManagedInstanceProperties = QueryExecutor.Instance.GetManagedInstance(connection, query, id);

                    return managedInstanceForEditResponce;
                }
            }
        }

        public void UpdateManagedInstance(ManagedInstanceProperties request)
        {
            using (_logger.InfoCall("UpdateInstance"))
            {
                if (request.DataCollectionSettings.CollectionInterval < 1 || request.DataCollectionSettings.CollectionInterval > 9999)
                    throw new ArgumentException("CollectionInterval should be in range from 1 to 9999.", "request");

                using (var connection = GetConnection())
                {
                    var query = QueryBuilder.Instance.UpdateManagedInstance();
                    QueryExecutor.Instance.UpdateManagedInstance(connection, query, request);
                }

                AgentManagerHelper.Instance.UpdateAuditConfigurationAsync(request.Id, GetConnection());
            }
        }

        public IEnumerable<CredentialValidationResult> ValidateInstanceCredentials(BatchInstancesCredentialsRequest request)
        {
            using (_logger.InfoCall("ValidateCredentials"))
            {
                using (var connection = GetConnection())
                {
                    var query = QueryBuilder.Instance.GetManagedServerInstances();
                    var serverInstances = QueryExecutor.Instance.GetManagedServerInstances(connection, query, request.InstancesIds);

                    return AuditServerHelper.Instance.ValidateCredentials(request.Credentials, serverInstances);
                }
            }
        }

        public void UpdateManagedInstancesCredentials(BatchInstancesCredentialsRequest request)
        {
            using (_logger.InfoCall("UpdateManagedInstancesCredentials"))
            {
                using (var connection = GetConnection())
                {
                    var query = QueryBuilder.Instance.UpdateManagedInstancesCredentials();
                    QueryExecutor.Instance.UpdateManagedInstancesCredentials(connection, query, request);
                }
            }
        }


        public IEnumerable<AuditedServerStatus> GetAuditedInstancesWidgetData()
        {
            using (_logger.InfoCall("GetAuditedInstancesWidgetData"))
            {
                FilteredRegisteredInstancesStatusRequest auditedInstacesRequest = new FilteredRegisteredInstancesStatusRequest();
                var result = QueryExecutor.Instance.GetFilteredInstancesStatuses(GetConnection(), auditedInstacesRequest);
                return result;
            }
        }

        //Start SQLCm-5.4 
        //Requirement - 4.1.3.1

        public SensitiveColumnInfo GetValidateSensitiveColumns(String csvData)
        {
            using (_logger.InfoCall("validateSensitiveColumns"))
            {
                var result = QueryExecutor.Instance.validateSensitiveColumns(GetConnection(), csvData);
                return result;
            }
        }

        public void GetSaveSensitiveColumnData(SensitiveColumnInfo retVal)
        {
            var updateSqlBuilder = new StringBuilder();
            using (_logger.InfoCall("saveSensitiveColumnData"))
            {
                QueryExecutor.Instance.GetSaveSensitiveColumnData(GetConnection(), retVal);
            }
        }

        // End Requirement - 4.1.3.1

        //Start - Requirement 4.1.4.1
        public AllImportSettingDetails validateAuditSettings(String xmlData)
        {
            using (_logger.InfoCall("validateAuditSettings"))
            {
                var result = QueryExecutor.Instance.ParseXml(GetConnection(), xmlData);
                return result;
            }
        }
        //End - Requirement 4.1.4.1



        ///<Export Instance Audit XML File>
        /// Rest Services Method to export the instance Audit Setting xml file.
        ///</ExportAudit>
        public string ExportServerAuditSettings(string request)
        {
            using (_logger.InfoCall("Export Server Audit Settings"))
            {
                var result = QueryExecutor.Instance.ExportServerAuditSettingsAction(GetConnection(), request);
                return result;
            }
        }

        public string GetLocalTime()
        {
            using (_logger.InfoCall("GetLocalTime"))
            {
                //Fix for SQLCM-5795, parsed local time utc format
                return DateTimeHelper.GetUTCCultureFreeDateTime(DateTime.Now);
            }
        }

        public bool IsLinqDllLoaded()
        {
            using (_logger.InfoCall("LinqAssemblyCheck"))
            {
                return QueryExecutor.Instance.IsLinqDllLoaded(GetConnection());
            }
        }

        public List<string> ExportServerRegulationAuditSettings(string request)
        {
            using (_logger.InfoCall("Export Server Regulation Audit Settings"))
            {
                var result = QueryExecutor.Instance.ExportServerRegulationAuditSettingsAction(GetConnection(), request);
                return result;
            }
        }
    }
}
