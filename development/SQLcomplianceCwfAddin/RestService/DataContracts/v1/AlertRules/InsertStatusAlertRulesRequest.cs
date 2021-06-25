using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [Serializable]
    [DataContract(Name = "InsertStatusAlertRulesRequest")]
    public class InsertStatusAlertRulesRequest
    {
        [DataMember(Order = 1, Name = "dataQuery")]
        public string DataQuery { get; set; }
    }
}
