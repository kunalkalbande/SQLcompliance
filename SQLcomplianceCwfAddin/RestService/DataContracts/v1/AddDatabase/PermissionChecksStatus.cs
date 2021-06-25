using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AddDatabase
{
    [DataContract(Name = "permissionChecksStatus")]
    public class PermissionChecksStatus
    {
        [DataMember(Order = 1, Name = "permissionsCheckList")]
        public List<PermissionCheck> PermissionsCheckList { get; set; }

        [DataMember(Order = 2, Name = "totalChecks")]
        public int TotalChecks { get; set; }

        [DataMember(Order = 3, Name = "passedChecks")]
        public int PassedChecks { get; set; }

        [DataMember(Order = 4, Name = "failedChecks")]
        public int FailedChecks { get; set; }
    }
}
