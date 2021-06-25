using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AuditReports
{
    [DataContract]
    public class PermissionDeniedActivityResponse
    {
        public PermissionDeniedActivityResponse()
        {
            AuditPermissionDeniedActivity = new List<PermissionDeniedActivityData>();
        }

        [DataMember]
        public List<PermissionDeniedActivityData> AuditPermissionDeniedActivity { get; set; }

        internal void Add(PermissionDeniedActivityData permissionDeniedActivityData)
        {
            AuditPermissionDeniedActivity.Add(permissionDeniedActivityData);
        }
    }
}
