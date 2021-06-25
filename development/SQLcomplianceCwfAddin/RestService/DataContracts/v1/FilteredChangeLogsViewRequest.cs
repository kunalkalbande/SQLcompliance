using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [Serializable]
    [DataContract(Name = "ChangeLogsViewRequest")]
    public class FilteredChangeLogsViewRequest
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

        [DataMember(Order = 7, Name = "EventType")]
        public ChangeLogsRuleType? EventType { get; set; }

        [DataMember(Order = 8, Name = "SourceRule")]
        public string SourceRule { get; set; }

        [DataMember(Order = 9, Name = "Event")]
        public string Event { get; set; }

        [DataMember(Order = 10, Name = "User")]
        public string User { get; set; }

        [DataMember(Order = 11, Name = "Detail")]
        public string Detail { get; set; }

        [DataMember(Order = 12, Name = "page")]
        public int? Page { get; set; }

        [DataMember(Order = 13, Name = "pageSize")]
        public int? PageSize { get; set; }

        [DataMember(Order = 14, Name = "sortColumn")]
        public string SortColumn { get; set; }

        [DataMember(Order = 15, Name = "sortDirection")]
        public int? SortDirection { get; set; }

        [DataMember(Name = "eventId", Order = 16)]
        public int? EventId { get; set; }
    }
}