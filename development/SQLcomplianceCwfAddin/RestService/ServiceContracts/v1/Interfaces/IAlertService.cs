using System.Collections.Generic;
using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Web;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.Alerts;

namespace SQLcomplianceCwfAddin.RestService.ServiceContracts.v1.Interfaces
{
    [ServiceContract]
    public interface IAlertService
    {
        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/DismissAlert", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("SQL Compliance Manager API to dismiss a single alert.")]
        void DismissAlert(int alertId);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/DismissAlerts", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("SQL Compliance Manager API to dismiss list of alerts.")]
        void DismissAlertList(IdCollection idCollection);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/DismissAlertsGroupForInstance", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("SQL Compliance Manager API to dismiss alerts group for instance.")]
        void DismissAlertsGroupForInstance(DismissAlertsGroupRequest request);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/GetFilteredAlerts", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get alerts based on filter.")]
        FilteredAlertsResponse GetFilteredAlerts(FilteredAlertRequest request);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "GET", UriTemplate = "/GetAlertsForInstance?serverId={serverId}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get data for audited SQL server instance alerts.")]
        IEnumerable<ServerAlert> GetAlertsForInstance(int serverId);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "GET", UriTemplate = "/GetAuditedInstancesAlert?alertType={alertType}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get data for audited SQL server alerts widget in audited instance view (tab).")]
        IEnumerable<ServerAlert> GetAuditedInstancesAlert(AlertType alertType);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "GET", UriTemplate = "/GetAlertsByDatabases?days={days}",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get alerts by databases.")]
        IEnumerable<ServerAlert> GetAlertsByDatabases(string days);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "GET", UriTemplate = "/GetAlertsGroups?instanceId={instanceId}",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get alerts groups.")]
        IEnumerable<AlertsGroup> GetAlertsGroups(string instanceId);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "GET", UriTemplate = "/GetAlerts?instanceId={instanceId}&alertType={alertType}&alertLevel={alertLevel}&pageSize={pageSize}&page={page}",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get alerts with paging.")]
        IEnumerable<ServerAlert> GetAlerts(string instanceId, AlertType alertType, AlertLevel alertLevel, int pageSize, int page);
    }
}
