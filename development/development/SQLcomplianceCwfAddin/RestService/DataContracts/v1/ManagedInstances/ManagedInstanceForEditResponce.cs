using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.ManagedInstances
{
    [DataContract]
    public class ManagedInstanceForEditResponce
    {
        [DataMember(Order = 1, Name = "managedInstanceProperties")]
        public ManagedInstanceProperties ManagedInstanceProperties { get; set; }

        [DataMember(Order = 2, Name = "owners")]
        public List<string> Owners { get; set; }

        [DataMember(Order = 3, Name = "locations")]
        public List<string> Locations { get; set; }
    }
}
