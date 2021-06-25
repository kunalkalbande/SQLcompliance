using System.Collections.Generic;
using System.Runtime.Serialization;
namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [DataContract]
    public class AuditEventExportResponse
    {

        public AuditEventExportResponse()
        {
            Events = new List<AuditEventExportData>();
            ConditionEvents = new List<AuditEventExportConditionData>();
            EventType = new List<AuditEventExportEventType>();
        }


        [DataMember]
        public List<AuditEventExportData> Events { get; set; }

        internal void Add(AuditEventExportData events)
        {
            Events.Add(events);
        }

        [DataMember]
        public List<AuditEventExportConditionData> ConditionEvents { get; set; }

        internal void Add(AuditEventExportConditionData conditionEvents)
        {
            ConditionEvents.Add(conditionEvents);
        }

        [DataMember]
        public List<AuditEventExportEventType> EventType { get; set; }

        internal void Add(AuditEventExportEventType eventType)
        {
            EventType.Add(eventType);
        }
    }
}




