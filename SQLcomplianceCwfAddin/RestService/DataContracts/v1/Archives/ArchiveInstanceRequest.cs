using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.Archives
{
    [DataContract]
    public class ArchiveInstanceRequest
    {
        [DataMember(Order = 0, Name = "instance")]
        public string Instance { get; set; }
    }
}
