using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.Archives
{
    [DataContract]
    public class ArchivePropertiesRequest
    {
        [DataMember(Name = "archive", Order = 0)]
        public string Archive { get; set; }
    }
}
