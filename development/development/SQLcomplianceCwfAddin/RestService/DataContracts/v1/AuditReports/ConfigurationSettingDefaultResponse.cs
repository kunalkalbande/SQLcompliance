using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AuditReports
{

    [DataContract]
    public class ConfigurationSettingDefaultResponse
    {
      

        [DataMember]
        public List<ConfigurationSettingKeyValue> defaultProperties { get; set; }
        public ConfigurationSettingDefaultResponse()
        {
            defaultProperties = new List<ConfigurationSettingKeyValue>();
        }
        public void Add(ConfigurationSettingKeyValue value)
        {
            defaultProperties.Add(value);
        }
    }
}
