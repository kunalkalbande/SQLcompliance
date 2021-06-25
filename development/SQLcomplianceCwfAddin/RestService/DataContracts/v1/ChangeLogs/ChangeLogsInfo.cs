using System;
using System.Runtime.Serialization;
using Idera.SQLcompliance.Core.Rules;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.ChangeLogs
{
    [DataContract]
    public class ChangeLogsInfo
    {
        [DataMember(Name = "alertId", Order = 0)]
        public int AlertId { get; set; }

        [DataMember(Name = "alertType", Order = 1)]
        public ActivityLogsType AlertType { get; set; }

        [DataMember(Name = "instance", Order = 2)]
        public string Instance { get; set; }

        [DataMember(Name = "created", Order = 3)]
        public DateTime? Created { get; set; }

        [DataMember(Name = "alertLevel", Order = 4)]
        public ActivityLogsLevel AlertLevel { get; set; }

        [DataMember(Name = "alertEventId", Order = 5)]
        public int AlertEventId { get; set; }

        [DataMember(Name = "ruleName", Order = 6)]
        public string RuleName { get; set; }

        [DataMember(Name = "eventType", Order = 7)]
        public EventType EventType { get; set; }

        [DataMember(Name = "alertRuleId", Order = 8)]
        public int AlertRuleId { get; set; }

        [DataMember(Name = "emailStatus", Order = 9)]
        public int EmailStatus { get; set; }

        [DataMember(Name = "logStatus", Order = 10)]
        public int LogStatus { get; set; }

        [DataMember(Name = "message", Order = 11)]
        public string Message { get; set; }

        [DataMember(Name = "computerName", Order = 12)]
        public string ComputerName { get; set; }

        [DataMember(Name = "snmpTrapStatus", Order = 13)]
        public int SnmpTrapStatus { get; set; }
    }
}
