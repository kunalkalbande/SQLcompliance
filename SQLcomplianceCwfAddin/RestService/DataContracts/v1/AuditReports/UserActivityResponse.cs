using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AuditReports
{
    [DataContract]
    public class UserActivityResponse
    {
        public UserActivityResponse()
        {
            AuditUserActivity = new List<UserActivityData>();
        }

        [DataMember]
        public List<UserActivityData> AuditUserActivity { get; set; }

        internal void Add(UserActivityData userActivityData)
        {
            AuditUserActivity.Add(userActivityData);
        }
    }
}
