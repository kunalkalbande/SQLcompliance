using System;
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [Serializable]
    [DataContract(Name = "AuditEventExportData")]
    public class AuditEventExportData
    {
        [DataMember(Name = "filterId")]
        public int FilterId { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "eventType")]
        public int EventType { get; set; }

        [DataMember(Name = "targetInstance")]
        public string TargetInstance { get; set; }

        [DataMember(Name = "enabled")]
        public bool Enabled { get; set; }
    }

    [Serializable]
    [DataContract(Name = "AuditEventExportConditionData")]
    public class AuditEventExportConditionData
    {
        [DataMember(Name = "conditionId")]
        public int ConditionId { get; set; }

        [DataMember(Name = "fieldId")]
        public int FieldId { get; set; }

        [DataMember(Name = "matchString")]
        public string MatchString { get; set; }
    }

    [Serializable]
    [DataContract(Name = "AuditEventExportEventType")]
    public class AuditEventExportEventType
    {

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "category")]
        public string Category { get; set; }
    }
   
}