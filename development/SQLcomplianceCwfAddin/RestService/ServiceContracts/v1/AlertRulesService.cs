using System;
using System.IO;
using System.Collections.Generic;
using SQLcomplianceCwfAddin.Helpers;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.AlertRules;
using Idera.SQLcompliance.Core.Templates;

namespace SQLcomplianceCwfAddin.RestService.ServiceContracts.v1
{
    public partial class RestService
    {
        public AlertRulesResponse GetAlertRules(AlertRulesRequest request)
        {
            Console.Out.WriteLine("inside AlertRules");
            using (_logger.InfoCall("GetAlertRules"))
            {
                var query = QueryBuilder.Instance.GetAlertRules();
                var result = QueryExecutor.Instance.GetAlertRules(GetConnection(), query, request);

                return result;
            }
        }

        public void GetEnableAlertRules(EnableAlertRulesRequest request)
        {
            using (_logger.InfoCall("GetEnableAlertRules"))
            {
                using (var connection = GetConnection())
                {
                    var query = QueryBuilder.Instance.GetEnableAlertRules();
                    QueryExecutor.Instance.GetEnableAlertRules(GetConnection(),query,request);
                }
            }
        }
        
        public void GetDeleteAlertRules(EnableAlertRulesRequest request)
        {
            using (_logger.InfoCall("GetDeleteAlertRules"))
            {
                using (var connection = GetConnection())
                {
                    var query = QueryBuilder.Instance.GetDeleteAlertRules();
                    QueryExecutor.Instance.GetDeleteAlertRules(GetConnection(), query, request);
                }
            }
        }

        public void GetUpdateSnmpConfiguration(UpdateSnmpConfigurationRequest request)
        {
            using (_logger.InfoCall("GetUpdateSnmpConfiguration"))
            {
                using (var connection = GetConnection())
                {
                    var query = QueryBuilder.Instance.GetUpdateSnmpConfiguration();
                    QueryExecutor.Instance.GetUpdateSnmpConfiguration(GetConnection(), query, request);
                }
            }
        }

        public void UpdateSNMPThresholdConfiguration(UpdateSNMPThresholdConfiguration request)
        {
            using (_logger.InfoCall("UpdateSNMPThresholdConfiguration"))
            {
                using (var connection = GetConnection())
                {
                    var query = QueryBuilder.Instance.UpdateSNMPThresholdConfiguration();
                    QueryExecutor.Instance.UpdateSNMPThresholdConfiguration(GetConnection(), query, request);
                }
            }
        }

        public void DeleteThresholdConfiguration(String request)
        {
            using (_logger.InfoCall("DeleteThresholdConfiguration"))
            {
                using (var connection = GetConnection())
                {
                    var query = QueryBuilder.Instance.DeleteThresholdConfiguration();
                    QueryExecutor.Instance.DeleteThresholdConfiguration(GetConnection(), query, request);
                }
            }
        }

        public GetSNMPConfigResponse GetSNMPThresholdConfiguration(GetSNMPThresholdConfiguration request)
        {
            using (_logger.InfoCall("GetSNMPThresholdConfiguration"))
            {
                using (var connection = GetConnection())
                {
                    var query = QueryBuilder.Instance.GetSNMPThresholdConfiguration();
                    var result = QueryExecutor.Instance.GetSNMPThresholdConfiguration(GetConnection(), query, request);
                    return result;
                }
            }
        }

        public void GetUpdateSmtpConfiguration(UpdateSmtpConfigurationRequest request)
        {
            using (_logger.InfoCall("GetUpdateSmtpConfiguration"))
            {
                using (var connection = GetConnection())
                {
                    var query = QueryBuilder.Instance.GetUpdateSmtpConfiguration();
                    QueryExecutor.Instance.GetUpdateSmtpConfiguration(GetConnection(), query, request);
                }
            }
        }

        public void GetUpdateWindowsLogEntry(UpdateWindowsLogEntryRequest request)
        {
            using (_logger.InfoCall("GetUpdateSmtpConfiguration"))
            {
                using (var connection = GetConnection())
                {
                    var query = QueryBuilder.Instance.GetUpdateWindowsLogEntry();
                    QueryExecutor.Instance.GetUpdateWindowsLogEntry(GetConnection(), query, request);
                }
            }
        }
        public void InsertStatusAlertRules(InsertStatusAlertRulesRequest request)
        {
            using (_logger.InfoCall("InsertStatusAlertRules"))
            {
                using (var connection = GetConnection())
                {
                    var query = QueryBuilder.Instance.InsertStatusAlertRules();
                    QueryExecutor.Instance.InsertStatusAlertRules(GetConnection(), query, request);
                }
            }
        }
        public AlertRulesExportResponse GetAlertRulesExport(AlertRulesExportRequest request)
        {
            using (_logger.InfoCall("GetAlertRulesExport"))
            {
                using (var connection = GetConnection())
                {
                    var query = QueryBuilder.Instance.GetDataByRuleId();
                    var result = QueryExecutor.Instance.GetAlertRulesExportList(GetConnection(), query, request);
                    return result;
                }
            }
        }

        public string ExportAlertRules(string request)
        {
            using (_logger.InfoCall("ExportAlertRules"))
            {                
                var result = QueryExecutor.Instance.ExportAlertRules(GetConnection(), request);
                return result;                
            }
        }

        public string ImportAlertRules(string request)
        {
            using (_logger.InfoCall("ImportAlertRules"))
            {
                var result = QueryExecutor.Instance.ImportAlertRules(GetConnection(), request);
                return result;
            }
        }

        public DataAlertRulesInfo GetDataAlertRulesInfo(DataAlertRulesServerId request)
        {
            using (_logger.InfoCall("GetDataAlertRulesInfo"))
            {
                using (var connection = GetConnection())
                {
                    var query = QueryBuilder.Instance.GetDataByServerId();
                    var result = QueryExecutor.Instance.GetDataAlertRulesInfo(GetConnection(), query, request);
                    return result;
                }
            }
        }

        public CategoryResponse GetCateGoryInfo(CategoryRequest request)
        {
            using (_logger.InfoCall("GetCateGoryInfo"))
            {
                using (var connection = GetConnection())
                {
                    var result = QueryExecutor.Instance.GetCateGoryInfo(GetConnection(), request);
                    return result;
                }
            }
        }

        public SNMPConfigurationData UpdateSnmpConfigData()
        {
            using (_logger.InfoCall("GetSnmpInformation"))
            {
                using (var connection = GetConnection())
                {
                    var result = QueryExecutor.Instance.UpdateSnmpConfigData(GetConnection());
                    return result;
                }
            }
        }

        public bool CheckSnmpAddress(SNMPConfigurationData request)
        {
            using (_logger.InfoCall("GetSnmpInformation"))
            {
                var result = QueryExecutor.Instance.CheckSnmpAddress(request);
                return result;
            }
        }
    }
}