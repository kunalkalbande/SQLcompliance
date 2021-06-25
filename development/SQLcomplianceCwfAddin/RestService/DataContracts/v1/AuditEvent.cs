using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SQLcomplianceCwfAddin.Helpers;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [DataContract(Name = "auditEvent")]
    public class AuditEvent
    { 
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "category")]
        public string Category { get; set; }

        [DataMember(Name = "event")]
        public string EventType { get; set; }

        [DataMember(Name = "time")]
        public DateTime Time { get; set; }

        [DataMember(Name = "details")]
        public string Details { get; set; }

        [DataMember(Name = "databaseName")]
        public string DatabaseName { get; set; }

        [DataMember(Name = "categoryId")]
        public int CategoryId { get; set; }

        [DataMember(Name = "eventTypeId")]
        public int EventTypeId { get; set; }

        [DataMember(Name = "loginName")]
        public string LoginName { get; set; }

        [DataMember(Name = "targetObject")]
        public string TargetObject { get; set; }

        [DataMember(Name = "databaseId")]
        public int DatabaseId { get; set; }

        [DataMember(Name = "eventDatabase")]
        public string EventDatabase { get; set; }
    }
}
