using System;
using System.Runtime.Serialization;
using SQLcomplianceCwfAddin.Helpers;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [Serializable]
    [DataContract(Name = "alertLevel")]
    public enum ActivityLogsLevel : byte
    {
        [EnumMember(Value = "severe")]
        Severe = 4,

        [EnumMember(Value = "high")]
        High = 3,

        [EnumMember(Value = "medium")]
        Medium = 2,

        [EnumMember(Value = "low")]
        Low = 1
    }

    [Serializable]
    [DataContract(Name = "alertType")]
    public enum ActivityLogsType : byte
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
    [DataContract(Name = "serverAlert")]
    public class ServerActivityLogs
    {
        [DataMember(Order = 0, Name = "logId")]
        public int LogId { get; set; }

        [DataMember(Order = 1, Name = "eventId")]
        public int EventId { get; set; }

        [DataMember(Order = 2, Name = "instanceName")]
        public string InstanceName { get; set; }

        [DataMember(Order = 2, Name = "instanceId")]
        public int instanceId { get; set; }

        [DataMember(Order = 3, Name = "eventTime")]
        public DateTime? EventTime { get; set; }

        [DataMember(Order = 4, Name = "eventType")]
        public string EventType { get; set; }

        [DataMember(Order = 5, Name = "detail")]
        public string Detail { get; set; }
    
    }
}