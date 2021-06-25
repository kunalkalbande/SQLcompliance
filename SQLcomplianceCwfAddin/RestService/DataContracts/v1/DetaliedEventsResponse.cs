using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [Serializable]
    [DataContract]
    public class DetaliedEventsResponse : BeforeAfterDataEventResponse
    {
        public DetaliedEventsResponse()
        {
            Events = new List<DetaliedAuditEvent>();
        }

        [DataMember]
        public List<DetaliedAuditEvent> Events { get; set; }
    }
}