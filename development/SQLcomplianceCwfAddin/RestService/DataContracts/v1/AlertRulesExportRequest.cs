using System;
using System.Runtime.Serialization;
using SQLcomplianceCwfAddin.Helpers;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{

    [Serializable]
    [DataContract(Name = "alertRulesExportRequest")]
    public class AlertRulesExportRequest
    {
         [DataMember(Order = 0, Name = "ruleId")]
         public int RuleId { get; set; }
    }
}