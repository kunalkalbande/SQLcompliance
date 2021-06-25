using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.ManagedInstances
{
    [DataContract(Name = "paginationRequest")]
    public class PaginationRequest
    {
        [DataMember(Name = "page")]
        public int? Page { get; set; }

        [DataMember(Name = "pageSize")]
        public int? PageSize { get; set; }

        [DataMember(Name = "sortColumn")]
        public string SortColumn { get; set; }

        [DataMember(Name = "sortDirection")]
        public int? SortDirection { get; set; }
    }
}