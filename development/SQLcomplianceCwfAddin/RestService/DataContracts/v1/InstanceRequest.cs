using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [DataContract]
    public class InstanceRequest
    {
        [DataMember(Order = 0, Name = "instance")]
        public string Instance { get; set; }
    }
}
