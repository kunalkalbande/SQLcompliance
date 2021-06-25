using System;
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [DataContract]
    public class FilteredRegisteredInstancesStatusRequest
    {
        [DataMember(Order = 1, Name = "status")]
        public string Status { get; set; }

        [DataMember(Order = 2, Name = "instance")]
        public string InstanceName { get; set; }

        [DataMember(Order = 3, Name = "auditedDbCountFrom")]
        public int? NumberOfAuditedDatabasesFrom { get; set; }

        [DataMember(Order = 4, Name = "auditedDbCountTo")]
        public int? NumberOfAuditedDatabasesTo { get; set; }

        [DataMember(Order = 5, Name = "sqlVersion")]
        public string SqlVersion { get; set; }

        [DataMember(Order = 6, Name = "isAudited")]
        public byte? IsAudited { get; set; }

        [DataMember(Order = 7, Name = "lastAgentContactFrom")]
        public DateTime? LastAgentContactFrom { get; set; }

        [DataMember(Order = 8, Name = "lastAgentContactTo")]
        public DateTime? LastAgentContactTo { get; set; }

        [DataMember(Order = 9, Name = "statusText")]
        public string StatusText { get; set; }

        [DataMember(Order = 10, Name = "isEnabled")]
        public string IsEnabled { get; set; }

        [DataMember(Order = 11, Name = "serverId")]
        public int? ServerId { get; set; }

        [DataMember(Order = 12, Name = "days")]
        public int? Days { get; set; }

        //this property is used only for internal using not for contract
        public bool UpdateStatisticProperties { get; set; } 
    }
}
