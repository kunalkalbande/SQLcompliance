using System;
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AddDatabase
{
    [DataContract(Name = "permissionCheck")]
    public class PermissionCheck
    {
        [DataMember(Order = 0, Name = "name")]
        public string Name { get; set; }

        [DataMember(Order = 1, Name = "status")]
        public CheckStatus Status { get; set; }

        [DataMember(Order = 2, Name = "resolutionSteps")]
        public string ResolutionSteps { get; set; }
    }
}
