using System.Collections.Generic;
using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Web;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.ManagedInstances;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.ManagedInstances.Credentials;
using System;

namespace SQLcomplianceCwfAddin.RestService.ServiceContracts.v1.Interfaces
{
    [ServiceContract]
    public interface IInstanceService
    {
        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "GET", UriTemplate = "/GetAuditedSqlServer?id={id}&days={days}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get audited SQL server details. To be used in instance view widegt, server status widget and audit configuration widget.")]
        AuditedServerStatus GetAuditedSqlServer(int id, int days);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "GET", UriTemplate = "/GetAuditedInstancesWidgetData", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get data for audited SQL servers widget in dashboard page.")]
        IEnumerable<AuditedServerStatus> GetAuditedInstancesWidgetData();

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/GetFilteredAuditedInstancesStatus", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get data for audited SQL servers widget in dashboard page.")]
        IEnumerable<AuditedServerStatus> GetFilteredAuditedInstancesStatus(FilteredRegisteredInstancesStatusRequest statusRequest);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/GetManagedInstances", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get managed instances. To be used in Manage SQL Server Instances (Administration page).")]
        ManagedInstanceResponce GetManagedInstances(PaginationRequest request);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "GET", UriTemplate = "/GetManagedInstance?id={id}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get managed instance by id. To be used in Manage Edit Instance Properties (Administration page).")]
        ManagedInstanceForEditResponce GetManagedInstance(int id);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateManagedInstance", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Update managed instance. To be used in Manage SQL Server Instances (Administration page).")]
        void UpdateManagedInstance(ManagedInstanceProperties request);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/ValidateInstanceCredentials", RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        [Description("Validate credentials. To be used in Manage SQL Server Instances (Administration page).")]
        IEnumerable<CredentialValidationResult> ValidateInstanceCredentials(BatchInstancesCredentialsRequest request);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateManagedInstancesCredentials", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Update credentials. To be used in Manage SQL Server Instances (Administration page).")]
        void UpdateManagedInstancesCredentials(BatchInstancesCredentialsRequest request);

        //Start SQLCm-5.4 
        //Requirement - 4.1.3.1

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/GetValidateSensitiveColumns", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Import Sensitive Column")]
        SensitiveColumnInfo GetValidateSensitiveColumns(string request);

        //End Requirement - 4.1.3.1

        //Requirement - 4.1.3.1

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/GetSaveSensitiveColumnData", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Save sensitive column data for audit Instance details")]
        void GetSaveSensitiveColumnData(SensitiveColumnInfo retVal);

        //End Requirement - 4.1.3.1

        //Start - Requirement - 4.1.4.1

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/validateAuditSettings", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Import Audit Settings")]
        AllImportSettingDetails validateAuditSettings(string request);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/ExportServerAuditSettings", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Export Server Audit Settings")]
        string ExportServerAuditSettings(string request);
		
		[OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/GetLocalTime", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get Local Time")]
        string GetLocalTime();

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/IsLinqDllLoaded", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Linq assembly check")]
        bool IsLinqDllLoaded();

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/ExportServerRegulationAuditSettings", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Export Server Regualtion Audit Settings")]
        List<string> ExportServerRegulationAuditSettings(string request);
    }
}
