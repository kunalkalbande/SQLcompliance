
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.AddDatabase;
using System.Runtime.Serialization;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.AddServer;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.ServerProperties
{
    [DataContract(Name = "auditServerProperties")]
    public class AuditServerProperties
    {
        [DataMember(Order = 0, Name = "serverId")]
        public int ServerId { get; set; }

        [DataMember(Order = 1, Name = "generalProperties")]
        public ServerGeneralProperties GeneralProperties { get; set; }

        //Audit Activities
        [DataMember(Order = 2, Name = "auditedActivities")]
        public AuditActivity AuditedActivities { get; set; }

        [DataMember(Order = 3, Name = "privilegedRolesAndUsers")]
        public ServerRolesAndUsers PrivilegedRolesAndUsers { get; set; }

        //Audit User Activities
        [DataMember(Order = 4, Name = "auditedPrivilegedUserActivities")]
        public AuditActivity AuditedPrivilegedUserActivities { get; set; }

        [DataMember(Order = 5, Name = "auditThresholdsData")]
        public ThresholdsData AuditThresholdsData { get; set; }

        [DataMember(Order = 6, Name = "serverAdvancedProperties")]
        public ServerAdvancedProperties ServerAdvancedProperties { get; set; }
    }
}
