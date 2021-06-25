using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.DatabaseProperties
{
    [DataContract(Name = "clrStatus")]
    public class ClrStatus
    {
        [DataMember(Order = 0, Name = "isConfigured")]
        public bool IsConfigured { get; set; }

        [DataMember(Order = 1, Name = "isRunning")]
        public bool IsRunning { get; set; }

        [DataMember(Order = 2, Name = "statusMessage")]
        public string StatusMessage { get; set; }

        //The properties below are used only for enable/disable CLR for server
        [DataMember(Order = 3, Name = "enable")]
        public bool Enable { get; set; }

        [DataMember(Order = 4, Name = "serverId")]
        public int ServerId { get; set; }
    }
}
