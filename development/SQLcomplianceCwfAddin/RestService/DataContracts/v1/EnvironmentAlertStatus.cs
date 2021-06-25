using System;
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [Serializable]
    [DataContract(Name = "environmentAlertStatus")]
    public class EnvironmentAlertStatus
    {
        public EnvironmentAlertStatus()
        {
            AuditedInstances = new AlertStatus();
            AuditedDatabases = new AlertStatus();
        }

        [DataMember(Order = 0, Name = "auditedInstances")]
        public AlertStatus AuditedInstances { get; set; }

        [DataMember(Order = 1, Name = "auditedDatabases")]
        public AlertStatus AuditedDatabases { get; set; }
    }
}
