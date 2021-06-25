using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.Archives
{
    [DataContract]
    public class ArchiveUpdateRequest
    {
        [DataMember(Name = "instance", Order = 0)]
        public string Instance { get; set; }
        [DataMember(Name = "databaseName", Order = 1)]
        public string DatabaseName { get; set; }
        [DataMember(Name = "displayName", Order = 2)]
        public string DisplayName { get; set; }
        [DataMember(Name = "description", Order = 3)]
        public string Description { get; set; }
        [DataMember(Name = "newDefaultAccess", Order = 4)]
        public int NewDefaultAccess { get; set; }
        [DataMember(Name = "oldDefaultAccess", Order = 5)]
        public int OldDefaultAccess { get; set; }
    }
}
