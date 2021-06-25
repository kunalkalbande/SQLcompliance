using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AddDatabase
{
    [DataContract(Name = "availabilityGroup")]
    public class AvailabilityGroup
    {
        [DataMember(Order = 2, Name = "name")]
        public string Name;

        [DataMember(Order = 1, Name = "databaseName")]
        public string DatabaseName;

        [DataMember(Order = 3, Name = "replicaServerName")]
        public string ReplicaServerName;

        [DataMember(Order = 4, Name = "replicaCount")]
        public int ReplicaCount;
    }
}
