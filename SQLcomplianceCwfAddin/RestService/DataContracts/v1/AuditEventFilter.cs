using System;
using System.Runtime.Serialization;
using SQLcomplianceCwfAddin.Helpers;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [Serializable]
    [DataContract(Name="auditEvent")]
    public class AuditEventFilter
    {
        [DataMember(Order = 0, Name = "filterid")]
        public int Filterid { get; set; }

        [DataMember(Order = 1, Name = "name")]
        public string Name { get; set; }

        [DataMember(Order = 2, Name = "description")]
        public string Description { get; set; }

        [DataMember(Order = 3, Name = "eventType")]
        public int EventType { get; set; }

        [DataMember(Order = 4, Name = "targetInstances")]
        public string TargetInstances { get; set; }

        [DataMember(Order = 5, Name = "enabled")]
        public bool Enabled { get; set; }

        [DataMember(Order = 6, Name = "instanceId")]
        public int InstanceId { get; set; }

        [DataMember(Order = 7, Name = "validFilter")]
        public int ValidFilter { get; set; }
    }
}
