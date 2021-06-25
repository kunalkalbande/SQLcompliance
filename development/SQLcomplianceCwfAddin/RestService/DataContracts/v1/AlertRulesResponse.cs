using System.Collections.Generic;
using System.Runtime.Serialization;
namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [DataContract]
    public class AlertRulesResponse
    {
        public AlertRulesResponse()
        {
            AlertRules = new List<ServerAlertRule>();
            EventType = new List<EventFilterListData>();
        }

        [DataMember]
        public List<ServerAlertRule> AlertRules { get; set; }

        [DataMember]
        public List<EventFilterListData> EventType { get; set; }

        [DataMember]
        public int TotalSevereAlerts { get; set; }

        [DataMember]
        public int TotalHighAlerts { get; set; }

        [DataMember]
        public int TotalMediumAlerts { get; set; }

        [DataMember]
        public int TotalLowAlerts { get; set; }

        [DataMember]
        public int TotalEventAlertRules { get; set; }

        [DataMember]
        public int TotalDataAlertRules { get; set; }

        [DataMember]
        public int TotalStatusAlertRules { get; set; }

        [DataMember]
        public int CountOfInstancesWithEventAlerts { get; set; }

        [DataMember]
        public int CountOfInstancesWithDataAlerts { get; set; }

        [DataMember]
        public int CountOfInstancesWithStatusAlerts { get; set; }

        internal void Add(ServerAlertRule alertRules)
        {
            AlertRules.Add(alertRules);
        }

        [DataMember(Name = "recordCount")]
        public int RecordCount { get; set; }
    }
}
