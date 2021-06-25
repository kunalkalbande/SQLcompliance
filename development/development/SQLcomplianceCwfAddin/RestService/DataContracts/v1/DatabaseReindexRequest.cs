using System;
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [DataContract]
    public class DatabaseReindexRequest
    {
        [DataMember(Order = 0, Name = "archive")]
        public string Archive { get; set; }

        [DataMember(Order = 1, Name = "indexStartTime")]
        public DateTime IndexStartTime { get; set; }

        [DataMember(Order = 2, Name = "indexDurationHours")]
        public int IndexDurationHours { get; set; }

        [DataMember(Order = 3, Name = "indexDurationMinutes")]
        public int IndexDurationMinutes { get; set; }
    }
}
