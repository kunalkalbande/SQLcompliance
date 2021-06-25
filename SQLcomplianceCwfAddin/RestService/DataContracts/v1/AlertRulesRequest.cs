using System;
using System.Runtime.Serialization;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.ManagedInstances;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [Serializable]
    [DataContract(Name = "AlertRulesRequest")]
    public class AlertRulesRequest : PaginationRequest
    {
        [DataMember(Order = 1, Name = "Levels")]
        public string Levels { get; set; }

        [DataMember(Order = 2, Name = "InstanceName")]
        public string InstanceName { get; set; }

        [DataMember(Order = 3, Name = "DateFrom")]
        public DateTime? DateFrom { get; set; }

        [DataMember(Order = 4, Name = "DateTo")]
        public DateTime? DateTo { get; set; }

        [DataMember(Order = 5, Name = "TimeFrom")]
        public DateTime? TimeFrom { get; set; }

        [DataMember(Order = 6, Name = "TimeTo")]
        public DateTime? TimeTo { get; set; }

        [DataMember(Order = 7, Name = "AlertType")]
        public AlertRuleType? AlertType { get; set; }

        [DataMember(Order = 8, Name = "Name")]
        public string Name { get; set; }

        [DataMember(Order = 9, Name = "LogMessage")]
        public string LogMessage { get; set; }

        [DataMember(Order = 10, Name = "EmailMessage")]
        public string EmailMessage { get; set; }

        [DataMember(Order = 11, Name = "SnmpTrap")]
        public string SnmpTrap { get; set; }

        [DataMember(Order = 12, Name = "Email")]
        public string Email { get; set; }

        [DataMember(Order = 13, Name = "EventLog")]
        public string EventLog { get; set; }

        [DataMember(Order = 14, Name = "Rule")]
        public string Rule { get; set; }

        [DataMember(Order = 15, Name = "RuleType")]
        public string RuleType { get; set; }

        [DataMember(Order = 16, Name = "Server")]
        public string Server { get; set; }
    }
}
