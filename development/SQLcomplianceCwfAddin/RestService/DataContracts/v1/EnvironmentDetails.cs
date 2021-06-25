using System;
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [DataContract]
    [Serializable]
    public enum EnvironmentHealth
    {
        Ok = 0, Warning = 1, Error = 2, NoInstances = 3
    }

    [Serializable]
    [DataContract]
    public class EnvironmentDetails
    {
        [DataMember(Order = 1, Name = "registeredSqlServerCount")]
        public int RegisteredSqlServerCount { get; set; }

        [DataMember(Order = 2, Name = "auditedSqlServerCount")]
        public int AuditedSqlServerCount { get; set; }

        [DataMember(Order = 3, Name = "auditedDatabaseCount")]
        public int AuditedDatabaseCount { get; set; }

        [DataMember(Order = 4, Name = "processedEventCount")]
        public long ProcessedEventCount { get; set; }

        [DataMember(Order = 5, Name = "environmentHealth")]
        public EnvironmentHealth EnvironmentHealth { get; set; }
    }
}
