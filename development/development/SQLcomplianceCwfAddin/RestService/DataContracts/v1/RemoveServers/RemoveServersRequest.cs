using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.RemoveServers
{
    [DataContract]
    public class RemoveServersRequest
    {
        [DataMember(Order = 0, Name = "serverIdList")]
        public List<int> ServerIdList { get; set; }

        [DataMember(Order = 1, Name = "deleteEventsDatabase")]
        public bool DeleteEventsDatabase { get; set; }
    }
}
