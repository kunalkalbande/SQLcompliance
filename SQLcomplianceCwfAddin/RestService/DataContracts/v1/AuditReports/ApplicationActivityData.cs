using System;
using System.Runtime.Serialization;
using SQLcomplianceCwfAddin.Helpers;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AuditReports
{
   
    [Serializable]
    [DataContract(Name = "ApplicationActivityData")]
    public class ApplicationActivityData
    {
        [DataMember(Order = 0, Name = "applicationName")]
        public string ApplicationName { get; set; }

        [DataMember(Order = 1, Name = "details")]
        public string Details { get; set; }

        [DataMember(Order = 2, Name = "databaseName")]
        public string DatabaseName { get; set; }

        [DataMember(Order = 3, Name = "eventType")]
        public string EventType { get; set; }

        [DataMember(Order = 4, Name = "sqlText")]
        public string SqlText { get; set; }

        [DataMember(Order = 5, Name = "hostName")]
        public string HostName { get; set; }

        [DataMember(Order = 6, Name = "loginName")]
        public string LoginName { get; set; }

        [DataMember(Order = 7, Name = "targetObject")]
        public string TargetObject { get; set; }

        [DataMember(Order = 8, Name = "startTime")]
        public string StartTime { get; set; }
    }
}
