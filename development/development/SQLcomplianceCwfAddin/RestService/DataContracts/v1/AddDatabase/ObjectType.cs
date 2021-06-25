using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AddDatabase
{
    [DataContract(Name = "objectType")]
    public enum ObjectType : byte
    {
        [EnumMember(Value = "table")]
        Table = 0,

        [EnumMember(Value = "systemTable")]
        SystemTable = 1,

        [EnumMember(Value = "view")]
        View = 2,
    }
}
