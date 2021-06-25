using System.Collections.Generic;
using System.Linq;
using Idera.SQLcompliance.Core;
using SQLcomplianceCwfAddin.Helpers;
using SQLcomplianceCwfAddin.Helpers.Agent;
using SQLcomplianceCwfAddin.Helpers.SQL;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1;
using System;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.AuditReports;

namespace SQLcomplianceCwfAddin.RestService.ServiceContracts.v1
{
    public partial class RestService
    {
        public ApplicationActivityResponse GetAuditedReportApplicationActivity(ApplicationActivityRequest request)
        {
            using (_logger.InfoCall("GetAuditReportApplicationActivity"))
            {
                var query = QueryBuilder.Instance.GetApplicationActivity();
                var result = QueryExecutor.Instance.GetApplicationActivity(GetConnection(), query, request);
                return result;
            }
        }

        public DMLActivityResponse GetAuditedReportDMLActivity(DMLActivityRequest request)
        {
            using (_logger.InfoCall("GetAuditReportDMLActivity"))
            {
                var query = QueryBuilder.Instance.GetDMLActivity();
                var result = QueryExecutor.Instance.GetDMLActivityData(GetConnection(), query, request);
                return result;
            }
        }

        public LoginCreationHistoryResponse GetAuditedReportLoginCreationHistory(LoginCreationHistoryRequest request)
        {
            using (_logger.InfoCall("GetAuditReportLoginCreationHistory"))
            {
                var query = QueryBuilder.Instance.GetLoginCreationHistory();
                var result = QueryExecutor.Instance.GetLoginCreationHistoryData(GetConnection(), query, request);
                return result;
            }
        }

        public LoginDeletionHistoryResponse GetAuditedReportLoginDeletionHistory(LoginDeletionHistoryRequest request)
        {
            using (_logger.InfoCall("GetAuditReportLogindeletionHistory"))
            {
                var query = QueryBuilder.Instance.GetLoginDeletionHistory();
                var result = QueryExecutor.Instance.GetLoginDeletionHistoryData(GetConnection(), query, request);
                return result;
            }
        }

        public ObjectActivityResponse GetAuditedReportObjectActivity(ObjectActivityRequest request)
        {
            using (_logger.InfoCall("GetAuditReportObjectDeletionHistory"))
            {
                var query = QueryBuilder.Instance.GetObjectActivity();
                var result = QueryExecutor.Instance.GetObjectActivityData(GetConnection(), query, request);
                return result;
            }
        }

        public PermissionDeniedActivityResponse GetAuditedReportPermissionDeniedActivity(PermissionDeniedActivityRequest request)
        {
            using (_logger.InfoCall("GetAuditReportPermissionDeniedActivity"))
            {
                var query = QueryBuilder.Instance.GetPermissionDeniedActivity();
                var result = QueryExecutor.Instance.GetPermissionDeniedActivityData(GetConnection(), query, request);
                return result;
            }
        }

        public UserActivityResponse GetAuditedReportUserActivity(UserActivityRequest request)
        {
            using (_logger.InfoCall("GetAuditReportUserActivity"))
            {
                var query = QueryBuilder.Instance.GetUserActivity();
                var result = QueryExecutor.Instance.GetUserActivityData(GetConnection(), query, request);
                return result;
            }
        }

        public RowCountResponse GetAuditedReportRowCount(RowCountRequest request)
        {
            using (_logger.InfoCall("GetAuditReportRowCount"))
            {
                var query = QueryBuilder.Instance.GetRowCount();
                var result = QueryExecutor.Instance.GetRowCountData(GetConnection(), query, request);
                return result;
            }
        }
        
        public RegulatoryComplianceResponse GetAuditedReportRegulatoryCompliance(RegulatoryComplianceRequest request)
        {
            using (_logger.InfoCall("GetRegulatoryCompliance"))
            {
                var query = QueryBuilder.Instance.GetRegulatoryCompliance();
                _logger.Info("RCC Stored Procedure: " + query);
                _logger.Info("RCC Parameters: Server Name -> " + request.ServerName + " Database Name -> " + request.DatabaseName
                    + " Audit Settings -> " + request.AuditSettings + " Regulation Guidelines -> " + request.RegulationGuidelines 
                    + " Values -> " + request.Values);
                var result = QueryExecutor.Instance.GetRegulatoryComplianceData(GetConnection(), query, request);
                _logger.Info("RCC result count: " + result.AuditRegulatoryCompliance.Count);
                return result;
            }
        }

        //start sqlcm 5.6-5469(configuration check report)
        public ConfigurationCheckResponse GetConfigurationCheckData(ConfigurationCheckRequest configurationCheckRequest)
        {
            using (_logger.InfoCall("GetConfigurationCheckData"))
            {
                _logger.Info("instance : "+configurationCheckRequest.Instance);
                _logger.Info("database: " + configurationCheckRequest.Database);
                _logger.Info("default: " + configurationCheckRequest.DefaultStatus);
                _logger.Info("audit setting :" + configurationCheckRequest.AuditSetting);
                var query = QueryBuilder.Instance.GetConfigurationCheck();
                var result = QueryExecutor.Instance.GetConfigurationCheckData(GetConnection(), query, configurationCheckRequest);
              
                return result;
            }
        }

        public  ConfigurationCheckSettingResponse GetConfigurationCheckSettings()
        {
            using (_logger.InfoCall("GetConfigurationCheckSettings"))
            {
                var query = QueryBuilder.Instance.GetConfigurationCheckSetting();
                var result = QueryExecutor.Instance.GetConfigurationCheckSetting(GetConnection(), query);
                return result;
            }
        }

        public ConfigurationSettingDefaultResponse GetConfigurationCheckDefaultDatabase()
        {
            using (_logger.InfoCall("GetConfigurationCheckDefaultDatabase"))
            {
                var query = String.Format("select * from {0}..{1}", CoreConstants.RepositoryDatabase, CoreConstants.RepositoryDefaultDatabaseSettings);
                var result = QueryExecutor.Instance.GetConfigurationCheckDefaultDatabase(GetConnection(), query);
                return result;
            }
        }

        public ConfigurationSettingDefaultResponse GetConfigurationCheckDefaultServer()
        {
            using (_logger.InfoCall("GetConfigurationCheckDefaultServer"))
            {
                var query = string.Format("select * from {0}..{1}", CoreConstants.RepositoryDatabase, CoreConstants.DefaultServerPropertise);
                var result = QueryExecutor.Instance.GetConfigurationCheckDefaultServer(GetConnection(), query);
                return result;
            }
        }

        //end sqlcm 5.6 -5469(configuration check report)
    }
}
