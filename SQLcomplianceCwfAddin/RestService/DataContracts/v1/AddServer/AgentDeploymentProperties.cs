using System.Runtime.Serialization;
using PluginCommon;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AddServer
{
    [DataContract(Name = "agentDeploymentProperties")]
    public class AgentDeploymentProperties
    {
        [DataMember(Order = 0, Name = "isDeployed")]
        public bool IsDeployed { get; set; }

        [DataMember(Order = 1, Name = "isDeployedManually")]
        public bool IsDeployedManually { get; set; }

        [DataMember(Order = 2, Name = "agentServiceAccount")]
        public Credentials AgentServiceCredentials { get; set; }

        [DataMember(Order = 3, Name = "agentTraceDirectory")]
        public string AgentTraceDirectory { get; set; }
    }
}
