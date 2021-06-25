using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AddServer
{
    [DataContract(Name = "agentDeployStatus")]
    public enum AgentDeployStatus
    {
        [EnumMember(Value = "now")]
        Now,

        [EnumMember(Value = "later")]
        Later,

        [EnumMember(Value = "manually")]
        Manually,

        [EnumMember(Value = "alreadyDeployed")]
        AlreadyDeployed,
    }
}
