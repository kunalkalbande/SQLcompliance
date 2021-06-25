using System;
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AddDatabase
{
    [DataContract(Name = "auditCollectionLevel")]
    public enum AuditCollectionLevel
    {
        [EnumMember(Value = "default")]
        Default = 0,

        [EnumMember(Value = "custom")]
        Custom = 1,

        [EnumMember(Value = "regulation")]
        Regulation = 2,
    }
}
