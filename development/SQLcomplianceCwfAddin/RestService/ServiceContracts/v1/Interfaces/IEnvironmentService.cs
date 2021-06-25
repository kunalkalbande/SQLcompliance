using System.Collections.Generic;
using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Web;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.Stats;

namespace SQLcomplianceCwfAddin.RestService.ServiceContracts.v1.Interfaces
{
    [ServiceContract]
    public interface IEnvironmentService
    {
        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "GET", UriTemplate = "/GetStatsData?instanceId={serverId}&days={days}&category={category}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get data for server activity report card in indivisual instance view.")]
        List<KeyValuePair<RestStatsCategory, List<RestStatsData>>> GetStatsData(string serverId, int days, RestStatsCategory category);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "GET", UriTemplate = "/GetEnvironmentObjects?objectId={objectId}&type={type}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get SQL Compliance Manager environment object hierarchy widget data.")]
        IEnumerable<EnvironmentObject> GetEnvironmentObjects(string objectId, EnvironmentObjectType type);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "GET", UriTemplate = "/GetEnvironmentDetails?days={days}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get SQL Compliance Manager environment details widget data.")]
        EnvironmentDetails GetEnvironmentDetails(string days);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "GET", UriTemplate = "/GetEnvironmentDetailsForInstancesAndDatabases?days={days}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get SQL Compliance Manager environment details widget data.")]
        EnvironmentDetailsForInstancesAndDatabases GetEnvironmentDetailsForInstancesAndDatabases(string days);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "GET", UriTemplate = "/GetEnvironmentAlertStatus?days={days}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get data for SQL compliance Environment Alert Status widget in CWF dashboard.")]
        EnvironmentAlertStatus GetEnvironmentAlertStatus(string days);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "GET", UriTemplate = "/GetDatabaseStatsData?instanceId={serverId}&databaseId={databaseId}&days={days}&category={category}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get data for database activity report card in indivisual database view.")]
        List<KeyValuePair<RestStatsCategory, List<RestStatsData>>> GetDatabaseStatsData(int serverId, int databaseId, int days, RestStatsCategory category);
    }
}
