using System;
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AgentProperties
{
    [DataContract(Name = "agentAuditSettings")]
    public class AgentAuditSettings
    {
        [DataMember(Order = 0, Name = "lastAgentUpdateDateTime")]
        public DateTime? LastAgentUpdateDateTime { get; set; }

        //Audit settings status
        [DataMember(Order = 1, Name = "auditSettingsUpdateEnabled")]
        public bool AuditSettingsUpdateEnabled { get; set; }//is used for update now button and for audit pending status

        [DataMember(Order = 2, Name = "agentAuditLevel")]
        public int AgentAuditLevel { get; set; }

        [DataMember(Order = 3, Name = "currentAuditLevel")]
        public int CurrentAuditLevel { get; set; }
    }
}
