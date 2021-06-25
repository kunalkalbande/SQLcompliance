using System;
using System.Runtime.Serialization;
using SQLcomplianceCwfAddin.Helpers;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [Serializable]
    [DataContract(Name = "auditEventExport")]
    public class AuditEventExportRequest
    {
        [DataMember(Order = 0, Name = "filterId")]
        public int FilterId { get; set; }
    }
}