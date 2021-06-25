using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AuditReports
{

    [Serializable]
    [DataContract(Name = "DMLActivityData")]
    public class LoginCreationHistoryData
    {
        [DataMember(Order = 0, Name = "targetLoginName")]
        public string TargetLoginName { get; set; }

        [DataMember(Order = 1, Name = "loginName")]
        public string LoginName { get; set; }

        [DataMember(Order = 2, Name = "hostName")]
        public string HostName { get; set; }

        [DataMember(Order = 3, Name = "applicationName")]
        public string ApplicationName { get; set; }

        [DataMember(Order = 4, Name = "startTime")]
        public string StartTime { get; set; }
    }
}

