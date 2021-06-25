using System;
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AgentProperties
{
    [DataContract(Name = "agentSettings")]
    public class AgentSettings
    {
        //Agent Status if true "Deployed" otherwise "Not Deployed"
        [DataMember(Order = 0, Name = "isDeployed")]
        public bool IsDeployed { get; set; }

        [DataMember(Order = 1, Name = "version")]
        public string Version { get; set; }

        [DataMember(Order = 2, Name = "port")]
        public int Port { get; set; }

        [DataMember(Order = 3, Name = "LastHeartbeatDateTime")]
        public DateTime? LastHeartbeatDateTime { get; set; }

        [DataMember(Order = 4, Name = "heartbeatInterval")]
        public int HeartbeatInterval { get; set; }

        [DataMember(Order = 5, Name = "LoggingLevel")]
        public LoggingLevel LoggingLevel { get; set; }
    }
}
