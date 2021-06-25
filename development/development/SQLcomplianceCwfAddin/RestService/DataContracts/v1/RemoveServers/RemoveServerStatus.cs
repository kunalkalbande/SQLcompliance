using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.RemoveServers
{
    [DataContract(Name = "removeServerStatus")]
    public class RemoveServerStatus
    {
        [DataMember(Order = 0, Name = "serverId")]
        public int ServerId { get; set; }

        [DataMember(Order = 1, Name = "wasRemoved")]
        public bool WasRemoved { get; set; }

        [DataMember(Order = 2, Name = "wasAgentDeactivated")]
        public bool WasAgentDeactivated { get; set; }

        [DataMember(Order = 3, Name = "errorMessage")]
        public string ErrorMessage { get; set; }
    }
}
