using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.UpgradeAgent
{
    [DataContract]
    public class MinorUpgradeAgentData
    {
        [DataMember(Order = 0, Name = "serverId")]
        public int ServerId { get; set; }
    }
}
