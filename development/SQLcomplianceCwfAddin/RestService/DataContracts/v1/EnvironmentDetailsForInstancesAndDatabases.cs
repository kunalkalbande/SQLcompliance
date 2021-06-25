using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [DataContract]
    public class EnvironmentDetailsForInstancesAndDatabases
    {
        [DataMember(Order = 0, Name = "alertsStatus")]
        public EnvironmentAlertStatus AlertStatus { get; set; }

        [DataMember(Order = 1, Name = "auditedInstanceCount")]
        public int AuditedInstanceCount { get; set; }

        [DataMember(Order = 2, Name = "auditedDatabaseCount")]
        public int AuditedDatabaseCount { get; set; }
    }
}
