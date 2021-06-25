using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AuditReports
{
    [DataContract]
   public  class ConfigurationCheckResponse
    {
        public ConfigurationCheckResponse()
        {
            ConfigurationCheck = new List<ConfigurationCheckJSONServer>();
        }

        [DataMember]
        public List<ConfigurationCheckJSONServer> ConfigurationCheck { get; set; }

        internal void Add(ConfigurationCheckJSONServer configurationCheckJSONServer)
        {
            ConfigurationCheck.Add(configurationCheckJSONServer);
        }
    }
}
