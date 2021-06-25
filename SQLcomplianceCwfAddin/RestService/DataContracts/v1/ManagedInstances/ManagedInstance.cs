using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.ManagedInstances
{
    [DataContract(Name = "managedInstance")]
    public class ManagedInstance
    {
        public ManagedInstance()
        {
            Credentials = new ManagedCredentials();
        }

        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "instance")]
        public string InstanceName { get; set; }

        [DataMember(Name = "credentials")]
        public ManagedCredentials Credentials { get; set; }
    }
}