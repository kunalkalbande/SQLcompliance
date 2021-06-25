using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    internal enum SystemAlertType
    {
        AgentWarning = 1001,
        AgentWarningResolution = 3001,
        AgentConfigurationError = 2001,
        AgentConfigurationResolution = 4001,
        TraceDirectoryError = 2002,
        TraceDirectoryResolution = 4002,
        SqlTraceError = 2003,
        SqlTraceResolution = 4003,
        ServerConnectionError = 2004,
        ServerConnectionResolution = 4004,
        CollectionServiceConnectionError = 2005,
        CollectionServiceConnectionResolution = 4005,
        ClrError = 2006,
        ClrResolution = 4006
    }

    [Serializable]
    [DataContract(Name = "serverStatusFlag")]
    public enum ServerStatusFlag : byte
    {
        [EnumMember(Value = "ok")]
        Ok,

        [EnumMember(Value = "disabled")]
        Disabled,

        [EnumMember(Value = "down")]
        Down,

        [EnumMember(Value = "error")]
        Error,

        [EnumMember(Value = "slow")]
        Slow,

        [EnumMember(Value = "up")]
        Up,

        [EnumMember(Value = "unknown")]
        Unknown
    }

    [Serializable]
    [DataContract]
    public enum ServerStatus
    {
        OK = 0,
        Warning = 1,
        Alert = 2,
        Archive = 3,
        Disabled = 4
    };

    [Serializable]
    [DataContract(Name = "alertStatusFlag")]
    public enum AlertStatusFlag : byte
    {
        [EnumMember(Value = "low")]
        Low = 1,

        [EnumMember(Value = "medium")]
        Medium = 2,

        [EnumMember(Value = "high")]
        High = 3,

        [EnumMember(Value = "severe")]
        Severe = 4,

        [EnumMember(Value = "ok")]
        Ok,

        [EnumMember(Value = "informational")]
        Informational,

        [EnumMember(Value = "warning")]
        Warning,

        [EnumMember(Value = "critical")]
        Critical
    }

    [Serializable]
    [DataContract(Name = "auditedServerStatus")]
    public class AuditedServerStatus
    {
        [DataMember(Order = 1, Name = "id")]
        public int Id { get; set; }

        [DataMember(Order = 2, Name = "instance")]
        public string Instance { get; set; }

        [DataMember(Order = 3, Name = "eventCategories")]
        public int EventCategoriesCount { get; set; }

        [DataMember(Order = 4, Name = "collectedEventCount")]
        public int CollectedEventCount { get; set; }

        [DataMember(Order = 5, Name = "agentStatus")]
        public AlertStatusFlag AgentStatus { get; set; }

        [DataMember(Order = 6, Name = "auditedDatabaseCount")]
        public int AuditedDatabaseCount { get; set; }

        [DataMember(Order = 7, Name = "statusText")]
        public string Message { get; set; }

        [DataMember(Order = 8, Name = "lastHeartbeat")]
        public DateTime? LastHeartbeat { get; set; }

        [DataMember(Order = 9, Name = "IsAudited")]
        public bool IsAudited { get; set; }

        [DataMember(Order = 10, Name = "sqlVersionString")]
        public string SqlVersionString { get; set; }

        [DataMember(Order = 11, Name = "serverStatus")]
        public bool ServerStatus { get; set; }

        [DataMember(Order = 12, Name = "lowAlerts")]
        public int LowAlerts { get; set; }

        [DataMember(Order = 13, Name = "mediumAlert")]
        public int MediumAlerts { get; set; }

        [DataMember(Order = 14, Name = "highAlerts")]
        public int HighAlerts { get; set; }

        [DataMember(Order = 15, Name = "severeAlerts")]
        public int SevereAlerts { get; set; }

        [DataMember(Order = 16, Name = "lastTimeAgentContacted")]
        public DateTime? LastAgentContactTime { get; set; }

        [DataMember(Order = 17, Name = "isEnabled")]
        public bool IsEnabled { get; set; }

        [DataMember(Order = 18, Name = "totalDatabaseCount")]
        public int TotalDatabaseCount { get; set; }

        [DataMember(Order = 19, Name = "status")]
        public ServerStatusFlag StatusFlag { get; set; }

        [DataMember(Order = 20, Name = "lastArchived")]
        public DateTime? LastArchived { get; set; }

        [DataMember(Order = 21, Name = "recentAlertCount")]
        public int RecentAlertCount { get; set; }

        [DataMember(Order = 22, Name = "eventFilters")]
        public List<string> EventFilters { get; set; }

        [DataMember(Order = 23, Name = "auditedServerActivities")]
        public List<string> AuditedServerActivities { get; set; }

        [DataMember(Order = 24, Name = "auditedPrivilegedUsersActivities")]
        public List<string> AuditedPrivilegedUsersActivities { get; set; }

        [DataMember(Order = 25, Name = "privilegedUsersCount")]
        public int PrivilegedUsersCount { get; set; }

        [DataMember(Order = 26, Name = "detailedServerStatus")]
        public ServerStatus ServerStatusDetailed { get; set; }
        
        [DataMember(Order = 27, Name = "isRunning")]
        public bool IsRunning { get; set; }

        [DataMember(Order = 28, Name = "isDeployed")]
        public bool IsDeployed { get; set; }

        [DataMember(Order = 29, Name = "auditSettingsUpdateEnabled")]
        public bool AuditSettingsUpdateEnabled { get; set; }

        [DataMember(Order = 30, Name = "isPrimary")]
        public bool IsPrimary { get; set; }
    }
}
