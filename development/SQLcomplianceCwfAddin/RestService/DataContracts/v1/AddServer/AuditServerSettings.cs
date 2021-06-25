using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AddServer
{
    [DataContract(Name = "auditServerSettings")]
    public class AuditServerSettings
    {
        [DataMember(Order = 0, Name = "instance")]
        public string Instance { get; set; }

        [DataMember(Order = 1, Name = "description")]
        public string Description { get; set; }

        [DataMember(Order = 2, Name = "isVirtualServer")]
        public bool IsVirtualServer { get; set; }

        [DataMember(Order = 3, Name = "existingAuditData")]
        public ExistingAuditData ExistingAuditData { get; set; }

        [DataMember(Order = 4, Name = "agentDeployStatus")]
        public AgentDeployStatus AgentDeployStatus { get; set; }

        [DataMember(Order = 5, Name = "agentDeploymentProperties")]
        public AgentDeploymentProperties AgentDeploymentProperties { get; set; }
    }
}