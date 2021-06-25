﻿using System.Collections.Generic;
using System.Runtime.Serialization;
namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [DataContract]
    public class FilteredActivityLogsViewResponce
    {
        public FilteredActivityLogsViewResponce()
        {
            Alerts = new List<ServerActivityLogs>();
            EventType = new List<EventFilterListData>();
        }

        [DataMember]
        public List<ServerActivityLogs> Alerts { get; set; }

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
        public int CountOfInstancesWithLowAlerts { get; set;}

        [DataMember]
        public int CountOfInstancesWithMediumAlerts { get; set; }

        [DataMember]
        public int CountOfInstancesWithHighAlerts { get; set; }

        [DataMember]
        public int CountOfInstancesWithSevereAlerts { get; set; }

        internal void Add(ServerActivityLogs alert)
        {
            Alerts.Add(alert);
        }

        [DataMember(Name = "recordCount")]
        public int RecordCount { get; set; }
     }
}
