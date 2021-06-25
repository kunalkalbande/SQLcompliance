using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.ManagedInstances
{
    [DataContract(Name = "dataCollectionSettings")]
    public class DataCollectionSettings
    {
        [DataMember(Order = 1, Name = "collectionInterval")]
        public int CollectionInterval { get; set; }

        [DataMember(Order = 2, Name = "keepDataFor")]
        public int KeepDataFor { get; set; }
    }
}