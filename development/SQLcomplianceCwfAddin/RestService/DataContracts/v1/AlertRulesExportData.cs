using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [Serializable]
    [DataContract(Name = "AlertRulesExportData")]
    public class AlertRulesExportData
    {
        [DataMember(Name = "ruleId")]
        public int RuleId { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "alertLevel")]
        public int AlertLevel { get; set; }

        [DataMember(Name = "alertType")]
        public int AlertType { get; set; }

        [DataMember(Name = "targetInstances")]
        public string TargetInstances { get; set; }

        [DataMember(Name = "enabled")]
        public bool Enabled { get; set; }

        [DataMember(Name = "message")]
        public string Message { get; set; }

        [DataMember(Name = "logMessage")]
        public int LogMessage { get; set; }

        [DataMember(Name = "emailMessage")]
        public int EmailMessage { get; set; }

        [DataMember(Name = "snmpTrap")]
        public int SnmpTrap { get; set; }

        [DataMember(Name = "snmpServerAddress")]
        public string SnmpServerAddress { get; set; }

        [DataMember(Name = "snmpPort")]
        public int SnmpPort { get; set; }

        [DataMember(Name = "snmpCommunity")]
        public string SnmpCommunity { get; set; }
    }

    [Serializable]
    [DataContract(Name = "AlertRulesExportConditionData")]
    public class AlertRulesExportConditionData
    {
        [DataMember(Name = "conditionId")]
        public int ConditionId { get; set; }

        [DataMember(Name = "fieldId")]
        public int FieldId { get; set; }

        [DataMember(Name = "matchString")]
        public string MatchString { get; set; }
    }

    [Serializable]
    [DataContract(Name = "AlertRulesExportCategoryData")]
    public class AlertRulesExportCategoryData
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "category")]
        public string Category { get; set; }
    }
}