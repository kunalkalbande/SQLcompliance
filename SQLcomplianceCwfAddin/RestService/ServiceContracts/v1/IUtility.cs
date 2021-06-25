using System.Collections.Generic;
using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Web;
using PluginCommon;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1;
using CWFContracts = Idera.SQLcompliance.Core.CWFDataContracts;

namespace SQLcomplianceCwfAddin.RestService.ServiceContracts.v1
{
    [ServiceContract]
    public interface IUtility
    {
        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "GET", UriTemplate = "/GetAuditedDatabase?id={id}&serverId={serverId}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get monitered SQL server database details. To be used in database view widget and database audited activity widget.")]
        AuditedDatabase GetAuditedDatabase(int id, int serverId);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/PushAlertsToCwfDashboard", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Push latest alerts to CWF dashboard.")]
        bool PushAlertsToCwfDashboard(List<Alert> alerts);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/PushInstancesToCwfDashboard", RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        [Description("Push latest registered instances to CWF dashboard.")]
        void PushInstancesToCwfDashboard();

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/SyncUsers", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Synchronize the list of users with CWF.")]
        bool SyncUsersWithCwf(List<CWFContracts.User> users);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "GET", UriTemplate = "/GetDatabaseByServerName?serverName={serverName}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Returns a list of all databases of a server.")]
        List<string> GetDatabaseByServerName(string serverName);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "GET", UriTemplate = "/GetAllAuditSettings", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Returns a list of all audit settings.")]
        List<string> GetAllAuditSettings();

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "GET", UriTemplate = "/GetAllRegulationGuidelines", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Returns a list of all regulation settings.")]
        List<string> GetAllRegulationGuidelines();

    }
}
