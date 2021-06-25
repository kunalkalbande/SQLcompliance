using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.ServerProperties
{
    [DataContract(Name = "databaseReadAccessLevel")]
    public enum DatabaseReadAccessLevel
    {
        [EnumMember(Value = "deny")]
        Deny = 0,

        [EnumMember(Value = "eventsOnly")]
        EventsOnly = 1,

        [EnumMember(Value = "all")]
        All = 2,
    }
}
