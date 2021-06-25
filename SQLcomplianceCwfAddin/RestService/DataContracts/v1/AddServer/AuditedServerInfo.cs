using System.Runtime.Serialization;


namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AddServer
{
    [DataContract(Name = "auditedServerInfo")]
    public class AuditedServerInfo
    {
        [DataMember(Order = 0, Name = "serverid")]
        public int ServerId { get; set; }

        [DataMember(Order = 1, Name = "name")]
        public string Name { get; set; }

        [DataMember(Order = 2, Name = "isAuditedServer")]
        public bool IsAuditedServer { get; set; }
    }
}
