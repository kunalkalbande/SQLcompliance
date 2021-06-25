using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AuditReports
{

    [Serializable]
    [DataContract(Name = "UserActivityData")]
    public class UserActivityData
    {
        [DataMember(Order = 0, Name = "applicationName")]
        public string ApplicationName { get; set; }

        [DataMember(Order = 1, Name = "databaseName")]
        public string DatabaseName { get; set; }

        [DataMember(Order = 2, Name = "eventType")]
        public string EventType { get; set; }

        [DataMember(Order = 3, Name = "hostName")]
        public string HostName { get; set; }

        [DataMember(Order = 4, Name = "details")]
        public string Details { get; set; }

        [DataMember(Order = 5, Name = "loginName")]
        public string LoginName { get; set; }

        [DataMember(Order = 6, Name = "targetObject")]
        public string TargetObject { get; set; }

        [DataMember(Order = 7, Name = "startTime")]
        public string StartTime { get; set; }

        [DataMember(Order = 8, Name = "sqlText")]
        public string SqlText { get; set; }
    }
}

