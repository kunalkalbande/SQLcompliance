using System;
using System.Runtime.Serialization;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.ManagedInstances;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [Serializable]
    [DataContract(Name = "AlertRequest")]
    public class FilteredAlertRequest : PaginationRequest
    {
        [DataMember(Order = 1, Name = "Levels")]
        public string Levels { get; set; }

        [DataMember(Order = 2, Name = "InstanceName")]
        public string InstanceName { get; set; }

        [DataMember(Order = 3, Name = "DateFrom")]
        public DateTime? DateFrom { get; set; }

        [DataMember(Order = 4, Name = "DateTo")]
        public DateTime? DateTo { get; set; }

        [DataMember(Order = 5, Name = "TimeFrom")]
        public DateTime? TimeFrom { get; set; }

        [DataMember(Order = 6, Name = "TimeTo")]
        public DateTime? TimeTo { get; set; }

        [DataMember(Order = 7, Name = "AlertType")]
        public AlertRuleType? AlertType { get; set; }

        [DataMember(Order = 8, Name = "SourceRule")]
        public string SourceRule { get; set; }

        [DataMember(Order = 9, Name = "Event")]
        public string Event { get; set; }

        [DataMember(Order = 10, Name = "Detail")]
        public string Detail { get; set; }
    }
}
