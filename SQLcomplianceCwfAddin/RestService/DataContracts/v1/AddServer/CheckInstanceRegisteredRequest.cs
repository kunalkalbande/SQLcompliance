using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AddServer
{
    [DataContract]
    public class CheckInstanceRegisteredRequest
    {
        [DataMember(Order = 0, Name = "instance")]
        public string Instance { get; set; }
    }
}
