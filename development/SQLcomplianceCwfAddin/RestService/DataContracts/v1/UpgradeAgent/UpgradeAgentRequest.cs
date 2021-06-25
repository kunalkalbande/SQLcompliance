using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.UpgradeAgent
{
    [DataContract]
    public class UpgradeAgentRequest
    {
        [DataMember(Order = 0, Name = "serverId")]
        public int ServerId { get; set; }

        [DataMember(Order = 1, Name = "account")]
        public string Account { get; set; }

        [DataMember(Order = 2, Name = "password")]
        public string Password { get; set; }
    }
}
