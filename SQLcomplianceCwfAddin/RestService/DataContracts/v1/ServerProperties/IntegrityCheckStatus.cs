using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.ServerProperties
{
    [DataContract(Name = "integrityCheckStatus")]
    public enum IntegrityCheckStatus
    {
        [EnumMember(Value = "none")]
        None = -1,

        [EnumMember(Value = "passed")] 
        Passed = 0,

        [EnumMember(Value = "inProgress")]
        InProgress = 1,

        [EnumMember(Value = "failed")] 
        Failed = 2,

        [EnumMember(Value = "failedAndRepaired")] 
        FailedAndRepaired = 3,

        [EnumMember(Value = "incomplete")] 
        Incomplete = 4,
    }
}
