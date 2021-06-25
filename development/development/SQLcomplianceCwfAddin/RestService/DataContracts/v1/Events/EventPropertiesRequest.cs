
using System.Collections.Generic;
using System.Runtime.Serialization;
namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.Events
{
    [DataContract]
    public class EventPropertiesRequest
    {
        [DataMember(Name = "eventId", Order = 0)]
        public int EventId { get; set; }

        [DataMember(Name = "serverId", Order = 1)]
        public int ServerId { get; set; }

        [DataMember(Name = "eventDatabase", Order = 2)]
        public string EventDatabase { get; set; }
    }
}
