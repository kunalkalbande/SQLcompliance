using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.ManagedInstances
{
    [DataContract]
    public class ManagedInstanceResponce
    {
        [DataMember(Name = "totalCount")]
        public int TotalCount { get; set; }

        [DataMember(Name = "managedInstances")]
        public IList<ManagedInstance> ManagedInstances { get; set; }
    }
}
