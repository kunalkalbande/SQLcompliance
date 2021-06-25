
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AddServer
{
    [DataContract(Name = "instanceRegisteredStatus")]
    public class InstanceRegisteredStatus
    {
        [DataMember(Order = 0, Name = "registeredStatus")]
        public RegisteredStatus RegisteredStatus { get; set; }

        [DataMember(Order = 1, Name = "eventDatabaseName")]
        public string EventDatabaseName { get; set; }
    }
}
