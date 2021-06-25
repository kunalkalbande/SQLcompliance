using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SQLcomplianceCwfAddin.Helpers
{

    [DataContract]
    public class GetSNMPConfigResponse
    {
        public GetSNMPConfigResponse()
        {
            SNMPConfig = new List<SNMPConfigData>();
        }

        [DataMember]
        public List<SNMPConfigData> SNMPConfig { get; set; }

        internal void Add(SNMPConfigData SNMPConfigData)
        {
            SNMPConfig.Add(SNMPConfigData);
        }
    }
}
