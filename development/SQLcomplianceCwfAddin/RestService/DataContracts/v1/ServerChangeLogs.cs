using System;
using System.Runtime.Serialization;
using SQLcomplianceCwfAddin.Helpers;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [Serializable]
    [DataContract(Name = "alertType")]
    public enum ChangeLogsType : byte
    {
        [EnumMember(Value = "event")]
        Event = 1,

        [EnumMember(Value = "status")]
        Status,

        [EnumMember(Value = "data")]
        Data,

        [EnumMember(Value = "all")]
        All
    }

    [Serializable]
    [DataContract(Name = "alertTypeFlag")]
    public enum ChangeLogsRuleType : byte
    {
        [EnumMember(Value = "event")]
        Event = 1,

        [EnumMember(Value = "status")]
        Status,

        [EnumMember(Value = "data")]
        Data
    }

    [Serializable]
    [DataContract(Name = "serverAlert")]
    public class ServerChangeLogs
    {
        [DataMember(Order = 0, Name = "logId")]
        public int LogId{ get; set; }


        [DataMember(Order = 1, Name = "eventId")]
        public int EventId { get; set; }

        [DataMember(Order = 2, Name = "eventTime")]
        public DateTime? EventTime { get; set; }

        [DataMember(Order = 3, Name = "logType")]
        public String LogType { get; set; }

        [DataMember(Order = 4, Name = "logUser")]
        public String LogUser { get; set; }
        
        [DataMember(Order = 5, Name = "logSqlServer")]
        public String LogSQLServer { get; set; }

        [DataMember(Order = 5, Name = "instanceId")]
        public int instanceId { get; set; }

        [DataMember(Order = 6, Name = "logInfo")]
        public string LogInfo { get; set; }
    }
}