using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AuditReports
{
    [Serializable]
    [DataContract]
    public class ConfigurationCheckRequest
    {
        [DataMember(Order = 1, Name = "instance")]
        public string Instance { get; set; }

        [DataMember(Order = 2, Name = "database")]
        public string Database { get; set; }

        [DataMember(Order = 3, Name = "default")]
        public int  DefaultStatus { get; set; }

        [DataMember(Order = 4, Name = "setting")]
        public int AuditSetting { get; set; }

    }
}
