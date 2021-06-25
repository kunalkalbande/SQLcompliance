using System.Collections.Generic;
using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Web;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.Alerts;
using SQLcomplianceCwfAddin.Helpers;

namespace SQLcomplianceCwfAddin.RestService.ServiceContracts.v1.Interfaces
{
    [ServiceContract]
    public interface IAlertRules
    {
        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/GetAlertRules", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get alerts based on filter.")]
        AlertRulesResponse GetAlertRules(AlertRulesRequest request);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/GetEnableAlertRules", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Enable or disable alert Rules.")]
        void GetEnableAlertRules(EnableAlertRulesRequest request);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/DeleteAlertRules", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Enable or disable alert Rules.")]
        void GetDeleteAlertRules(EnableAlertRulesRequest request);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/GetUpdateSnmpConfiguration", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Enable or disable alert Rules.")]
        void GetUpdateSnmpConfiguration(UpdateSnmpConfigurationRequest request);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateSNMPThresholdConfiguration", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Enable or disable alert Rules.")]
        void UpdateSNMPThresholdConfiguration(UpdateSNMPThresholdConfiguration request);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/DeleteThresholdConfiguration", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Delete Threshold Configuration")]
        void DeleteThresholdConfiguration(string request);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/GetSNMPThresholdConfiguration", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("retrieves value of threshold configuration")]
        GetSNMPConfigResponse GetSNMPThresholdConfiguration(GetSNMPThresholdConfiguration request);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/GetUpdateSmtpConfiguration", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Enable or disable alert Rules.")]
        void GetUpdateSmtpConfiguration(UpdateSmtpConfigurationRequest request);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/GetUpdateWindowsLogEntry", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Enable or disable alert Rules.")]
        void GetUpdateWindowsLogEntry(UpdateWindowsLogEntryRequest request);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/InsertStatusAlertRules", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Enable or disable alert Rules.")]
        void InsertStatusAlertRules(InsertStatusAlertRulesRequest request);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/GetAlertRulesExport", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get Audit Event Filter Data to Export")]
        AlertRulesExportResponse GetAlertRulesExport(AlertRulesExportRequest request);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/ExportAlertRules", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Export Alert Rules by Id")]
        string ExportAlertRules(string request);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/ImportAlertRules", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Import Alert Rules")]
        string ImportAlertRules(string request);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/GetDataAlertRulesInfo", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get Audit Event Filter Data to Export")]
        DataAlertRulesInfo GetDataAlertRulesInfo(DataAlertRulesServerId request);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/GetCateGoryInfo", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get Category info for Alert Rules and Events")]
        CategoryResponse GetCateGoryInfo(CategoryRequest request);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateSnmpConfigData", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get SNMP Configuration")]
        SNMPConfigurationData UpdateSnmpConfigData();

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/CheckSnmpAddress", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Check SNMP Configuration")]
        bool CheckSnmpAddress(SNMPConfigurationData request);
    }
}
