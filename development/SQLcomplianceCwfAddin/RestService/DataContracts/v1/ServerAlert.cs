using System;
using System.Runtime.Serialization;
using SQLcomplianceCwfAddin.Helpers;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{    
    [Serializable]
    [DataContract(Name = "alertType")]
    public enum AlertType : byte
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
    public class ServerAlert
    {
        [DataMember(Order = 0, Name = "id")]
        public int AlertId { get; set; }

        [DataMember(Order = 1, Name = "instanceId")]
        public int InstanceId { get; set; }

        [DataMember(Order = 2, Name = "instanceName")]
        public string InstanceName { get; set; }

        [DataMember(Order = 3, Name = "time")]
        public DateTime? AlertTime { get; set; }

        [DataMember(Order = 4, Name = "alertLevel")]
        public AlertLevel Level { get; set; }

        [DataMember(Order = 5, Name = "alertType")]
        public AlertRuleType Type { get; set; }

        [DataMember(Order = 6, Name = "sourceRule")]
        public string RuleName { get; set; }

        [DataMember(Order = 7, Name = "alertEventId")]
        public int AlertEventId { get; set; }

        [DataMember(Order = 8, Name = "eventType")]
        public int EventType { get; set; }

        [DataMember(Order = 9, Name = "detail")]
        public string Detail { get; set; }

        [DataMember(Order = 10, Name = "title")]
        public string Title { get; set; }

        [DataMember(Order = 11, Name = "eventTypeName")]
        public string AlertEventTypeName { get; set; }
    }
}