using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.UpgradeAgent
{
    [DataContract(Name = "upgradeAgentType")]
    public enum UpgradeAgentType
    {
        [EnumMember(Value = "minorUpgrade")]
        MinorUpgrade,

        [EnumMember(Value = "majorUpgrade")]
        MajorUpgrade 
    }
}
