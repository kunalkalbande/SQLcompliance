using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AddServer
{
    [DataContract(Name = "registeredStatus")]
    public enum RegisteredStatus
    {
        [EnumMember(Value = "notRegistered")]
        NotRegistered,

        [EnumMember(Value = "isRegistered")]
        IsRegistered,

        [EnumMember(Value = "wasRegistered")]
        //was removed but has event database audit data
        WasRegistered,
    }
}
