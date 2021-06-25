using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SQLcomplianceCwfAddin.Helpers;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [Serializable]
    [DataContract(Name = "state")]
    public enum State : byte
    {
        [EnumMember(Value = "all")]
        All,

        [EnumMember(Value = "audited")]
        Audited,

        [EnumMember(Value = "monitered")]
        Monitered
    }

    [Serializable]
    [DataContract(Name = "auditedSqlServer")]
    public class AuditedServer
    {
        #region members

        private DateTime _lastHeartbeat;
        private DateTime _lastArchived;

        #endregion

        [DataMember(Order = 0, Name = "id")]
        public int Id { get; set; }

        [DataMember(Order = 1, Name = "name")]
        public string Instance { get; set; }

        [DataMember(Order = 2, Name = "databaseCount")]
        public int DatabaseCount { get; set; }

        [DataMember(Order = 3, Name = "auditedDatabaseCount")]
        public int AuditedDatabaseCount { get; set; }

        [DataMember(Order = 4, Name = "status")]
        public ServerStatusFlag Status { get; set; }

        [DataMember(Order = 5, Name = "statusMessage", EmitDefaultValue = false)]
        public string StatusMessage { get; set; }

        internal DateTime LastHeartbeat
        {
            get { return _lastHeartbeat; }
            set
            {
                _lastHeartbeat = value;
                LastHeartbeatString = Transformer.Instance.GetDateString(_lastHeartbeat);
            }
        }

        [DataMember(Order = 6, Name = "lastHeartbeat", EmitDefaultValue = false)]
        public string LastHeartbeatString { get; set; }

        internal DateTime LastArchived
        {
            get { return _lastArchived; }
            set
            {
                _lastArchived = value;
                LastArchivedString = Transformer.Instance.GetDateString(_lastArchived);
            }
        }

        [DataMember(Order = 7, Name = "lastArchived", EmitDefaultValue = false)]
        public string LastArchivedString { get; set; }

        [DataMember(Order = 8, Name = "collectedEventCount")]
        public long CollectedEventCount { get; set; }

        [DataMember(Order = 9, Name = "recentAlertCount")]
        public long RecentAlertCount { get; set; }

        [DataMember(Order = 10, Name = "auditedServerActivities")]
        public List<string> AuditedServerActivities { get; set; }

        [DataMember(Order = 11, Name = "auditedPrivililedgedUsersActivities")]
        public List<string> AuditedPrivililedgedUsersActivities { get; set; }

        [DataMember(Order = 12, Name = "eventFilters")]
        public List<string> EventFilters { get; set; }

        [DataMember(Order = 13, Name = "isRunning")]
        public bool IsRunning { get; set; }
    }
}
