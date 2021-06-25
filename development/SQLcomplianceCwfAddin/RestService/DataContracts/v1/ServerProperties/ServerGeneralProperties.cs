using System;
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.ServerProperties
{
    [DataContract(Name = "serverGeneralProperties")]
    public class ServerGeneralProperties
    {
        //General - Database
        [DataMember(Order = 0, Name = "instance")]
        public string Instance { get; set; }

        [DataMember(Order = 0, Name = "instanceServer")]
        public string InstanceServer { get; set; }

        [DataMember(Order = 0, Name = "instancePort")]
        public int? InstancePort { get; set; }

        [DataMember(Order = 1, Name = "instanceVersion")]
        public string InstanceVersion { get; set; }

        [DataMember(Order = 1, Name = "isClustered")]
        public bool IsClustered { get; set; }

        [DataMember(Order = 2, Name = "description")]
        public string Description { get; set; }

        [DataMember(Order = 3, Name = "statusMessage")]
        public string StatusMessage { get; set; }

        [DataMember(Order = 4, Name = "createdDateTime")]
        public DateTime? CreatedDateTime { get; set; }

        [DataMember(Order = 5, Name = "lastModifiedDateTime")]
        public DateTime? LastModifiedDateTime { get; set; }

        [DataMember(Order = 6, Name = "lastHeartbeatDateTime")]
        public DateTime? LastHeartbeatDateTime { get; set; }

        [DataMember(Order = 7, Name = "eventsReceivedDateTime")]
        public DateTime? EventsReceivedDateTime { get; set; }

        [DataMember(Order = 8, Name = "IsAuditEnabled")]
        public bool IsAuditEnabled { get; set; }//Auditing status

        [DataMember(Order = 9, Name = "IsAuditedServer")]
        public bool IsAuditedServer { get; set; }

        [DataMember(Order = 10, Name = "lastAgentUpdateDateTime")]
        public DateTime? LastAgentUpdateDateTime { get; set; }

        [DataMember(Order = 11, Name = "auditSettingsUpdateEnabled")]
        public bool AuditSettingsUpdateEnabled { get; set; }//is used for update now button and for audit pending status

        [DataMember(Order = 12, Name = "eventsDatabaseName")]
        public string EventsDatabaseName { get; set; }

        [DataMember(Order = 13, Name = "isDatabaseIntegrityOk")]
        public bool IsDatabaseIntegrityOk { get; set; }// Database integrity

        [DataMember(Order = 14, Name = "lastIntegrityCheckDateTime")]
        public DateTime? LastIntegrityCheckDateTime { get; set; }

        [DataMember(Order = 15, Name = "lastIntegrityCheckResultsStatus")]
        public IntegrityCheckStatus LastIntegrityCheckResultsStatus { get; set; }

        [DataMember(Order = 16, Name = "lastArchiveCheckDateTime")]
        public DateTime? LastArchiveCheckDateTime { get; set; }

        [DataMember(Order = 17, Name = "lastArchiveCheckResultsStatus")]
        public ArchiveCheckStatus LastArchiveCheckResultsStatus { get; set; }
    }
}
