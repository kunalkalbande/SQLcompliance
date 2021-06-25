using System;
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [Serializable]
    [DataContract(Name = "detaliedAuditEvent")]
    public class DetaliedAuditEvent : AuditEvent
    {
        [DataMember(Name = "spid")]
        public int Spid { get; set; }
        
        [DataMember(Name = "application")]
        public string Application { get; set; }
        
        [DataMember(Name = "host")]
        public string Host { get; set; }
        
        [DataMember(Name = "server")]
        public string Server { get; set; }
        
        [DataMember(Name = "accessCheck")]
        public int AccessCheck { get; set; }
        
        [DataMember(Name = "databaseUser")]
        public string DatabaseUser { get; set; }
        
        [DataMember(Name = "object")]
        public string Object { get; set; }
        
        [DataMember(Name = "targetLogin")]
        public string TargetLogin { get; set; }
        
        [DataMember(Name = "targetUser")]
        public string TargetUser { get; set; }
        
        [DataMember(Name = "role")]
        public string Role { get; set; }
        
        [DataMember(Name = "owner")]
        public string Owner { get; set; }
        
        [DataMember(Name = "privilegedUser")]
        public int PrivilegedUser { get; set; }
        
        [DataMember(Name = "sessionLogin")]
        public string SessionLogin { get; set; }
        
        [DataMember(Name = "auditedUpdates")]
        public int AuditedUpdates { get; set; }
        
        [DataMember(Name = "primaryKey")]
        public string PrimaryKey { get; set; }
        
        [DataMember(Name = "table")]
        public string Table { get; set; }
        
        [DataMember(Name = "column")]
        public string Column { get; set; }
        
        [DataMember(Name = "beforeValue")]
        public string BeforeValue { get; set; }
        
        [DataMember(Name = "afterValue")]
        public string AfterValue { get; set; }
        
        [DataMember(Name = "columnsUpdated")]
        public int ColumnsUpdated { get; set; }

        [DataMember(Name = "schema")]
        public string Schema { get; set; }
    }

    [Serializable]
    [DataContract(Name = "eventTypeFilter")]
    public class EventFilterListData
    {
       [DataMember(Name = "eventType")]
       public string EventType { get; set; }
    }
}