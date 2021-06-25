using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AgentProperties
{
    [DataContract(Name = "loggingLevel")]
    public enum LoggingLevel
    {
        [EnumMember(Value = "silent")]
        Silent = 0,

        [EnumMember(Value = "normal")]
        Normal = 1,

        [EnumMember(Value = "verbose")]
        Verbose = 2,

        [EnumMember(Value = "debug")]
        Debug = 3,
    }
}
