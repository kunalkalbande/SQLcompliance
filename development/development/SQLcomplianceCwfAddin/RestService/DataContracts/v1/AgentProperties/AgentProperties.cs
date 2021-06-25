using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AgentProperties
{
    [DataContract(Name = "agentProperties")]
    public class AgentProperties
    {
        [DataMember(Order = 0, Name = "serverId")]
        public int ServerId { get; set; }

        [DataMember(Order = 1, Name = "generalProperties")]
        public AgentGeneralProperties GeneralProperties { get; set; }

        [DataMember(Order = 2, Name = "deployment")]
        public AgentDeployment Deployment { get; set; }

        [DataMember(Order = 3, Name = "sqlServerList")]
        public List<SQLServerInfo> SqlServerList { get; set; }

        [DataMember(Order = 4, Name = "traceOptions")]
        public AgentTraceOptions TraceOptions { get; set; }
    }
}
