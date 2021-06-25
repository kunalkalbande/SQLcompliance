using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.Alerts
{
    [DataContract]
    public class DismissAlertsGroupRequest
    {
        [DataMember(Name = "instanceId", Order = 0)]
        public int InstanceId { get; set; }

        [DataMember(Name = "alertType", Order = 1)]
        public AlertType AlertType { get; set; }

        [DataMember(Name = "alertLevel", Order = 2)]
        public AlertLevel AlertLevel { get; set; }
    }
}
