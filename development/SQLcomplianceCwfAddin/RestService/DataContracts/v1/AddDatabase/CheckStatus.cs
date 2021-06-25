using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AddDatabase
{
    [DataContract(Name = "checkStatus")]
    public enum CheckStatus : byte
    {
        [EnumMember(Value = "passed")]
        Passesd = 0,

        [EnumMember(Value = "failed")]
        Failed = 1,
    }
}
