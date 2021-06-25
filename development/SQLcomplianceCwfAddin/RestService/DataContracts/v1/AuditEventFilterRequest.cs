using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [Serializable]
    [DataContract(Name = "ActivityLogsViewRequest")]
    public class AuditEventFilterRequest
    {
        [DataMember(Order = 2, Name = "Filter")]
        public string Filter { get; set; }

        [DataMember(Order = 3, Name = "Instance")]
        public string Instance { get; set; }

        [DataMember(Order = 4, Name = "isEnabled")]
        public string IsEnabled { get; set; }

        [DataMember(Order = 1, Name = "Description")]
        public string DescriptionFilter { get; set; }


        [DataMember(Order = 5, Name = "filterId")]
        public int FilterId { get; set; }

        [DataMember(Order = 6, Name = "name")]
        public string Name { get; set; }

        [DataMember(Order = 7, Name = "description")]
        public string Description { get; set; }

        [DataMember(Order = 8, Name = "eventType")]
        public int EventType { get; set; }

        [DataMember(Order = 9, Name = "targetInstances")]
        public string TargetInstances { get; set; }

        [DataMember(Order = 10, Name = "enabled")]
        public bool Enabled { get; set; }

        [DataMember(Order = 11, Name = "page")]
        public int? Page { get; set; }

        [DataMember(Order = 12, Name = "pageSize")]
        public int? PageSize { get; set; }

        [DataMember(Order = 13, Name = "sortColumn")]
        public string SortColumn { get; set; }

        [DataMember(Order = 14, Name = "sortDirection")]
        public int? SortDirection { get; set; }
    }
}
