using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AgentProperties
{
    [DataContract(Name = "agentGeneralProperties")]
    public class AgentGeneralProperties
    {
        [DataMember(Order = 0, Name = "agentComputer")]
        public string AgentComputer { get; set; }

        [DataMember(Order = 1, Name = "agentSettings")]
        public AgentSettings AgentSettings { get; set; }

        [DataMember(Order = 2, Name = "auditSettings")]
        public AgentAuditSettings AuditSettings { get; set; }
    }
}
