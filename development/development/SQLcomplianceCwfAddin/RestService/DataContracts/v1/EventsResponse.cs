using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [Serializable]
    [DataContract]
    public class EventsResponse : BeforeAfterDataEventResponse
    {
        public EventsResponse()
        {
            Events = new List<AuditEvent>();
        }

        [DataMember]
        public List<AuditEvent> Events { get; set; } 
    }
}
