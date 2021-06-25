using System.Runtime.Serialization;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.ServerProperties;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AddDatabase
{
    [DataContract(Name = "serverSettingsData")]
    public class ServerSettingsData
    {
        [DataMember(Order = 0, Name = "serverId")]
        public int ServerId { get; set; }

        [DataMember(Order = 1, Name = "serverAuditedActivities")]
        public AuditActivity ServerAuditedActivities { get; set; }

        [DataMember(Order = 2, Name = "databasePermissions")]
        public DatabaseReadAccessLevel DatabasePermissions { get; set; }
    }
}
