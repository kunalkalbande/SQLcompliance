using System.Collections.Generic;
using System.Runtime.Serialization;
namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [DataContract]
    public class AuditEventFilterResponse
    {
        public AuditEventFilterResponse()
        {
            Events = new List<AuditEventFilter>();
        }

        [DataMember]
        public List<AuditEventFilter> Events { get; set; }

        internal void Add(AuditEventFilter events)
        {
            Events.Add(events);
        }
    }
}
