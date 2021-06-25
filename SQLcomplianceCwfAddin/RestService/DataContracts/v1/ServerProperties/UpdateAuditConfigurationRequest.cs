using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.ServerProperties
{
    [DataContract]
    public class UpdateAuditConfigurationRequest
    {
        [DataMember(Order = 0, Name = "serverId")]
        public int ServerId { get; set; }
    }
}
