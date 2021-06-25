using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Web;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.AgentProperties;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.AuditReports;

namespace SQLcomplianceCwfAddin.RestService.ServiceContracts.v1.Interfaces
{
    [ServiceContract]
    public interface IAuditReportService
    {        
        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/GetAuditedReportApplicationActivity", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("List all Application Activities.")]
        ApplicationActivityResponse GetAuditedReportApplicationActivity(ApplicationActivityRequest request);


        [OperationContract(AsyncPattern = false)]
        [WebInvoke(UriTemplate = "/GetAuditedReportDMLActivity", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("List DML event for which before and after data are available.")]
        DMLActivityResponse GetAuditedReportDMLActivity(DMLActivityRequest request);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(UriTemplate = "/GetAuditedReportLoginCreationHistory", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("List all login creation activity.")]
        LoginCreationHistoryResponse GetAuditedReportLoginCreationHistory(LoginCreationHistoryRequest request);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(UriTemplate = "/GetAuditedReportLoginDeletionHistory", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("List all login deletion activity.")]
        LoginDeletionHistoryResponse GetAuditedReportLoginDeletionHistory(LoginDeletionHistoryRequest request);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(UriTemplate = "/GetAuditedReportObjectActivity", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("List all activity for specified object.")]
        ObjectActivityResponse GetAuditedReportObjectActivity(ObjectActivityRequest request);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(UriTemplate = "/GetAuditedReportPermissionDeniedActivity", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("List all activity for which permission was denied.")]
        PermissionDeniedActivityResponse GetAuditedReportPermissionDeniedActivity(PermissionDeniedActivityRequest request);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(UriTemplate = "/GetAuditedReportUserActivity", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("List all activity for specified user.")]
        UserActivityResponse GetAuditedReportUserActivity(UserActivityRequest request);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(UriTemplate = "/GetAuditedReportRowCount", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("List all row count activities.")]
        RowCountResponse GetAuditedReportRowCount(RowCountRequest request);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(UriTemplate = "/GetAuditedReportRegulatoryCompliance", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Lists all server level audit settings.")]
        RegulatoryComplianceResponse GetAuditedReportRegulatoryCompliance(RegulatoryComplianceRequest request);

        //start sqlcm 5.6 -5469(Configuration Check Report)
        [OperationContract(AsyncPattern = false)]
        [WebInvoke(UriTemplate = "/GetConfigurationCheckData", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("List Configuration of Database and Servers")]
        ConfigurationCheckResponse GetConfigurationCheckData(ConfigurationCheckRequest request);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(UriTemplate = "/GetConfigurationCheckSettings", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("List all audit settings")]
        ConfigurationCheckSettingResponse GetConfigurationCheckSettings();

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(UriTemplate = "/GetConfigurationCheckDefaultDatabase", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("List all database audit settings")]
        ConfigurationSettingDefaultResponse GetConfigurationCheckDefaultDatabase();

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(UriTemplate = "/GetConfigurationCheckDefaultServer", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("List all server audit settings")]
        ConfigurationSettingDefaultResponse GetConfigurationCheckDefaultServer();

        //end sqlcm 5.6 -5469(Configuration Check Report)
    }
}
