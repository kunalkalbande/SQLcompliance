using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.ManagedInstances
{
    [DataContract(Name = "managedInstancePropertis")]
    public class ManagedInstanceProperties : ManagedInstance
    {
        public ManagedInstanceProperties()
        {
            DataCollectionSettings = new DataCollectionSettings();
        }

        [DataMember(Name = "owner")]
        public string Owner { get; set; }

        [DataMember(Name = "location")]
        public string Location { get; set; }

        [DataMember(Name = "dataCollectionSettings")]
        public DataCollectionSettings DataCollectionSettings { get; set; }

        [DataMember(Name = "comments")]
        public string Comments { get; set; }
    }
}