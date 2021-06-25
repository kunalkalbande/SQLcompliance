using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.ServerProperties
{
    [DataContract(Name = "serverStatus")]
    public enum ServerStatus
    {
        [EnumMember(Value = "ok")] 
        Ok = 0,

        [EnumMember(Value = "warning")] 
        Warning = 1,

        [EnumMember(Value = "alert")] 
        Alert = 2,

        [EnumMember(Value = "archive")] 
        Archive = 3,

        [EnumMember(Value = "disabled")] 
        Disabled = 4
    }
}
