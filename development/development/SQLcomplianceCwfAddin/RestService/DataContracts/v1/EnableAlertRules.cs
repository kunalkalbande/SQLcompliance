using System.Collections.Generic;
using System.Runtime.Serialization;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.AddDatabase;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [DataContract]
    public class EnableAlertRules
    {
        [DataMember(Order = 0, Name = "ruleId")]
        public int RuleId { get; set; }

        [DataMember(Order = 1, Name = "enable")]
        public bool Enable { get; set; }
    }
}