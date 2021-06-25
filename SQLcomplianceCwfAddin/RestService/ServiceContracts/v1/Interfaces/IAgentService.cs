using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Web;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.AgentProperties;

using SQLcomplianceCwfAddin.RestService.DataContracts.v1.UpgradeAgent;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1;

namespace SQLcomplianceCwfAddin.RestService.ServiceContracts.v1.Interfaces
{
    [ServiceContract]
    public interface IAgentService
    {
        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/CheckAgentStatus", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("SQL Compliance Manager check agent instance.")]
        CheckAgentStatusResponse CheckAgentStatus(CheckAgentStatusRequest instance);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(UriTemplate = "/GetAgentProperties?serverId={serverId}", Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get agent properties by server Id.")]
        AgentProperties GetAgentProperties(int serverId);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(UriTemplate = "/UpdateAgentProperties", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Update agent properties.")]
        bool UpdateAgentProperties(AgentProperties agentProperties);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/UpgradeAgent", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Upgrade Agent")]
        UpgradeAgentResponse UpgradeAgent(UpgradeAgentRequest upgradeAgentRequest);
    }
}
