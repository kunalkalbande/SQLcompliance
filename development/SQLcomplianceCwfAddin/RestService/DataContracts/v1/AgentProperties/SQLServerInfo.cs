using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AgentProperties
{
    [DataContract(Name = "sqlServerInfo")]
    public class SQLServerInfo
    {
        [DataMember(Order = 0, Name = "instance")]
        public string Instance { get; set; }

        [DataMember(Order = 1, Name = "description")]
        public string Description { get; set; }
    }
}
