using System;
using System.Runtime.Serialization;
using Idera.SQLcompliance.Core.Rules;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AlertRules
{
    [DataContract]
    public class AlertRulesInfo
    {
        [DataMember(Name = "ruleId", Order = 0)]
        public int RuleId{ get; set; }

        [DataMember(Name = "name", Order = 1)]
        public string Name { get; set; }

        [DataMember(Name = "targetInstances", Order = 2)]
        public string TargetInstance { get; set; }

        [DataMember(Name = "alertLevel", Order = 3)]
        public AlertLevel AlertLevel { get; set; }

        [DataMember(Name = "alertType", Order = 5)]
        public int AlertType { get; set; }

        [DataMember(Name = "emailMessage", Order = 6)]
        public string EmailMessage { get; set; }

        [DataMember(Name = "logMessage", Order = 7)]
        public string LogMessage { get; set; }

        [DataMember(Name = "snmpTrap", Order = 13)]
        public int SnmpTrap{ get; set; }
    }
}
