using System;
using System.Runtime.Serialization;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.ManagedInstances;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.Stats;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [DataContract]
    public class FilteredEventRequest : PaginationRequest
    {
        [DataMember]
        public int ServerId { get; set; }

        [DataMember]
        public int? DatabaseId { get; set; }

        [DataMember]
        public string DatabaseName { get; set; }

        [DataMember]
        public string Category { get; set; }

        [DataMember]
        public string EventType { get; set; }

        [DataMember]
        public string LoginName { get; set; }

        [DataMember]
        public string TargetObject { get; set; }

        [DataMember]
        public DateTime? DateFrom { get; set; }

        [DataMember]
        public DateTime? DateTo { get; set; }

        [DataMember]
        public DateTime? TimeFrom { get; set; }

        [DataMember]
        public DateTime? TimeTo { get; set; }

        [DataMember]
        public string Details { get; set; }

        [DataMember]
        public int? TableId { get; set; }

        [DataMember]
        public int? ColumnId { get; set; }

        [DataMember]
        public string Archive { get; set; }

        [DataMember]
        public RestStatsCategory? StatCategory { get; set; }

        [DataMember]
        public int? SpidFrom { get; set; }

        [DataMember]
        public int? SpidTo { get; set; }

        [DataMember]
        public string Application { get; set; }

        [DataMember]
        public string Host { get; set; }

        [DataMember]
        public string Server { get; set; }

        [DataMember]
        public string AccessCheck { get; set; }

        [DataMember]
        public string DatabaseUser { get; set; }

        [DataMember]
        public string Object { get; set; }

        [DataMember]
        public string TargetLogin { get; set; }

        [DataMember]
        public string TargetUser { get; set; }

        [DataMember]
        public string Role { get; set; }

        [DataMember]
        public string Owner { get; set; }

        [DataMember]
        public string PrivilegedUser { get; set; }

        [DataMember]
        public string SessionLogin { get; set; }

        [DataMember]
        public int? AuditedUpdatesFrom { get; set; }
        [DataMember]
        public int? AuditedUpdatesTo { get; set; }

        [DataMember]
        public string PrimaryKey { get; set; }

        [DataMember]
        public string Table { get; set; }

        [DataMember]
        public string Column { get; set; }

        [DataMember]
        public string BeforeValue { get; set; }

        [DataMember]
        public string AfterValue { get; set; }

        [DataMember]
        public int? ColumnsUpdatedFrom { get; set; }
        [DataMember]
        public int? ColumnsUpdatedTo { get; set; }

        [DataMember]
        public string Schema { get; set; }
    }
}
