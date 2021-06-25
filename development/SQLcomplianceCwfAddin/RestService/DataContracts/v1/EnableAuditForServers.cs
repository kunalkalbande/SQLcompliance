using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [DataContract]
    public class EnableAuditForServers
    {
        [DataMember(Order = 0, Name = "serverIdList")]
        public List<int> ServerIdList { get; set; }

        [DataMember(Order = 1, Name = "enable")]
        public bool Enable { get; set; }
    }
}
