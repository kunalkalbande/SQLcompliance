using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [DataContract]
    public class DetachArchiveRequest
    {
        [DataMember(Name = "instance", Order = 0)]
        public string Instance { get; set; }

        [DataMember(Name = "databaseName", Order = 1)]
        public string DatabaseName { get; set; }
    }
}
