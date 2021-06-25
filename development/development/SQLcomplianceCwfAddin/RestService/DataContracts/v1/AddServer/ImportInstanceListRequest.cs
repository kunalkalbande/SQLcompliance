using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AddServer
{
    [DataContract]
    public class ImportInstanceListRequest
    {
        [DataMember(Order = 0, Name = "InstanceList")]
        public List<string> InstanceList { get; set; }
    }
}
