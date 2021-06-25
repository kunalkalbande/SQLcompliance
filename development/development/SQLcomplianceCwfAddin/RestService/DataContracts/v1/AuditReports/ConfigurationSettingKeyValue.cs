using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AuditReports
{
    [Serializable]
    [DataContract(Name = "KeyValue")]
    public class ConfigurationSettingKeyValue
    {
        [DataMember(Order = 0, Name = "key")]
        public string key { get; set; }

        [DataMember(Order = 1, Name = "value")]
        public string value { get; set; }
    }
}
