using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.UpgradeAgent
{
    [DataContract(Name = "upgradeAgentResponse")]
    public class UpgradeAgentResponse
    {
        [DataMember(Name = "errorMessage", Order = 0)]
        public string ErrorMessage { get; set; }

        [DataMember(Name = "success", Order = 1)]
        public bool Success { get; set; }

        [DataMember(Name = "upgradeStatusMessage", Order = 2)]
        public string UpgradeStatusMessage { get; set; }
    }
}
