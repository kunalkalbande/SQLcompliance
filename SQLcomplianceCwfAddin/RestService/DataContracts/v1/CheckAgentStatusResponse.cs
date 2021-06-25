using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [DataContract]
    public class CheckAgentStatusResponse
    {
        [DataMember(Order = 0, Name = "isActive")]
        public bool IsActive { get; set; }

        [DataMember(Order = 1, Name = "agentServer")]
        public string AgentServer { get; set; }
    }
}
