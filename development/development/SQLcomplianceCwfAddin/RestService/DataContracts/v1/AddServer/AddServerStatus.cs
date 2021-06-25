
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AddServer
{
    [DataContract(Name = "addServerStatus")]
    public class AddServerStatus
    {
        [DataMember(Order = 0, Name = "serverId")]
        public int ServerId { get; set; }

        [DataMember(Order = 0, Name = "instance")]
        public string Instance { get; set; }

        [DataMember(Order = 1, Name = "wasSuccessfullyAdded")]
        public bool WasSuccessfullyAdded { get; set; }

        [DataMember(Order = 1, Name = "wasAgentDeployedAutomatically")]
        public bool WasAgentDeployedAutomatically { get; set; }

        [DataMember(Order = 2, Name = "errorMessage")]
        public string ErrorMessage { get; set; }

        [DataMember(Order = 3, Name = "shouldIndexesToBeUpdated")]
        public bool ShouldIndexesToBeUpdated { get; set; }
    }
}
