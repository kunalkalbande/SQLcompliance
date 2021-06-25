using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.UpgradeAgent
{
    [DataContract]
    public class MajorUpgradeAgentData
    {
        [DataMember(Order = 0, Name = "serverId")]
        public int ServerId { get; set; }

        [DataMember(Order = 1, Name = "account")]
        public string Account { get; set; }

        [DataMember(Order = 2, Name = "password")]
        public string Password { get; set; }
    }
}
