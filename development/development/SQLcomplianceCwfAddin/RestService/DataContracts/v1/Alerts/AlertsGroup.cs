﻿using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.Alerts
{
    [DataContract]
    public class AlertsGroup
    {
        [DataMember(Name = "alertType", Order = 0)]
        public AlertType AlertType { get; set; }

        [DataMember(Name = "alertLevel", Order = 1)]
        public AlertLevel AlertLevel { get; set; }

        [DataMember(Name = "alertsCount", Order = 2)]
        public int AlertsCount { get; set; }
    }
}
