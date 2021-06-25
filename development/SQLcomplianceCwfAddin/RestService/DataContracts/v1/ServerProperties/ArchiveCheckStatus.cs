
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.ServerProperties
{
    [DataContract(Name = "archiveCheckStatus")]
    public enum ArchiveCheckStatus
    {
        [EnumMember(Value = "none")]
        None = -1,

        [EnumMember(Value = "completed")]
        Completed = 0,

        [EnumMember(Value = "inProgress")]
        InProgress = 1,

        [EnumMember(Value = "failedIntegrity")]
        FailedIntegrity = 2,

        [EnumMember(Value = "failedWithErrors")]
        FailedWithErrors = 3,

        [EnumMember(Value = "incomplete")] 
        Incomplete = 4,
    }
}
