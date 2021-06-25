using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using SQLcomplianceCwfAddin.Helpers;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
        [Serializable]
        [DataContract(Name = "alertLevel")]
        public enum AlertLevel : byte
        {
            [EnumMember(Value = "severe")]
            Severe = 4,

            [EnumMember(Value = "high")]
            High = 3,

            [EnumMember(Value = "medium")]
            Medium = 2,

            [EnumMember(Value = "low")]
            Low = 1
       }
    
        [Serializable]
        [DataContract(Name = "alertRuleTypeFlag")]
        public enum AlertRuleType : byte
        {
            [EnumMember(Value = "event")]
            Event = 1,

            [EnumMember(Value = "status")]
            Status,

            [EnumMember(Value = "data")]
            Data
        }

        [Serializable]
        [DataContract(Name = "serverAlert")]
        public class ServerAlertRule
        {
            [DataMember(Order = 0, Name = "ruleId")]
            public int RuleId { get; set; }

            [DataMember(Order = 1, Name = "name")]
            public string Name { get; set; }

            [DataMember(Order = 2, Name = "targetInstances")]
            public string TargetInstances { get; set; }

            [DataMember(Order = 3, Name = "alertLevel")]
            public AlertLevel Level { get; set; }

            [DataMember(Order = 4, Name = "alertType")]
            public AlertRuleType Type { get; set; }

            [DataMember(Order = 5, Name = "emailMessage")]
            public string EmailMessage { get; set; }

            [DataMember(Order = 7, Name = "logMessage")]
            public string LogMessage { get; set; }

            [DataMember(Order = 8, Name = "snmpTrap")]
            public string SnmpTrap{ get; set; }

            [DataMember(Order = 9, Name = "enabled")]
            public bool Enabled { get; set; }

            [DataMember(Order = 10, Name = "instanceId")]
            public int InstanceId { get; set; }

            [DataMember(Order = 11, Name = "ruleValidation")]
            public int RuleValidation { get; set; }
        }
    }