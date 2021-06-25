using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AuditReports
{
    [Serializable]
    [DataContract(Name = "ConfigurationCheckSettings")]
    public class ConfigurationCheckSettingResponse
    {
        public ConfigurationCheckSettingResponse()
        {
            ConfigurationCheck = new List<String>();
        }

        [DataMember]
        public List<String> ConfigurationCheck { get; set; }

        internal void Add(String item)
        {
            ConfigurationCheck.Add(item);
        }
    }
}
