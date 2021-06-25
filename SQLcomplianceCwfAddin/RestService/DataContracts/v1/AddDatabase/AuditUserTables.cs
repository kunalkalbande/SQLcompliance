using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AddDatabase
{
    [DataContract(Name = "auditUserTables")]
    public enum AuditUserTables: byte
    {
        [EnumMember(Value = "none")]
        None = 0,

        [EnumMember(Value = "all")]
        All = 1,

        [EnumMember(Value = "following")]
        Following = 2,
    }
}