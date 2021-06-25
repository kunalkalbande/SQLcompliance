using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [DataContract]
    public class CheckAgentStatusRequest
    {
        [DataMember(Order = 0, Name = "instance")]
        public string Instance { get; set; }
    }
}
