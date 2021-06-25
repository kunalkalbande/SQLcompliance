using System.Collections.Generic;
using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Web;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.AddServer;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.RemoveServers;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.ServerProperties;

namespace SQLcomplianceCwfAddin.RestService.ServiceContracts.v1.Interfaces
{
    [ServiceContract]
    public interface IServerService
    {
        [OperationContract(AsyncPattern = false)]
        [WebInvoke(UriTemplate = "/GetAuditedInstances", Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get Audited (Registered) Instances.")]
        List<AuditedServerInfo> GetAuditedInstances();

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(UriTemplate = "/GetAllNotRegisteredInstanceNameList", Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get All Not Registered Instance Name List.")]
        List<string> GetAllNotRegisteredInstanceNameList();

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/AddServers", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Register SQL server instance list. Returns list of add server status.")]
        List<AddServerStatus> AddServers(List<AuditServerSettings> serverSettingsList);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/ImportServerInstances", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Register SQL server instance name list. Returns list of add server status.")]
        List<AddServerStatus> ImportServerInstances(ImportInstanceListRequest importInstanceListRequest);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/RemoveServers", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Remove servers by ids with/without deletion event database.")]
        List<RemoveServerStatus> RemoveServers(RemoveServersRequest removeServersRequest);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/EnableAuditingForServers", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Enable or disable auditing for servers.")]
        void EnableAuditingForServers(EnableAuditForServers servers);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(UriTemplate = "/GetAuditServerProperties?serverId={serverId}", Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get server properties.")]
        AuditServerProperties GetAuditServerProperties(int serverId);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(UriTemplate = "/GetAgentDeploymentPropertiesForInstance", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get deploy status of agent service for instance.")]
        AgentDeploymentProperties GetAgentDeploymentPropertiesForInstance(InstanceRequest instanceRequest);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(UriTemplate = "/IsInstanceAvailable", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Check if instance available if not also return reason.")]
        InstanceAvailableResponse IsInstanceAvailable(InstanceRequest instanceRequest);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(UriTemplate = "/UpdateAuditServerProperties", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Update audit server properties.Returns true if updating was successfully.")]
        bool UpdateAuditServerProperties(AuditServerProperties serverProperties);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(UriTemplate = "/ValidateCredentials", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Validate Credentials.")]
        bool ValidateCredentials(Credentials credentials);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(UriTemplate = "/UpdateAuditConfigurationForServer", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Update Agent Audit Settings Configuration for Server by Id.")]
        string UpdateAuditConfigurationForServer(UpdateAuditConfigurationRequest request);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(UriTemplate = "/CheckInstanceRegisteredStatus", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Check server instance registered status.")]
        InstanceRegisteredStatus CheckInstanceRegisteredStatus(CheckInstanceRegisteredRequest request);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(UriTemplate = "/GetAllAuditedInstances", Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Returns a list of all audited instances.")]
        List<string> GetAllAuditedInstances();
    }
}
