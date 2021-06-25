using System.Collections.Generic;
using System.Runtime.Serialization;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.AddDatabase;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.DatabaseProperties
{
    [DataContract(Name = "auditBeforeAfterData")]
    public class AuditBeforeAfterData
    {
        [DataMember(Order = 1, Name = "isAvailable")]
        public bool IsAvailable { get; set; }

        [DataMember(Order = 2, Name = "columnsSupported")]
        public bool ColumnsSupported { get; set; }

        [DataMember(Order = 3, Name = "statusMessaage")]
        public string StatusMessaage { get; set; }

        [DataMember(Order = 4, Name = "clrStatus")]
        public ClrStatus ClrStatus { get; set; }

        [DataMember(Order = 5, Name = "beforeAfterTableColumnDictionary")]
        public Dictionary<string, DatabaseObject> BeforeAfterTableColumnDictionary { get; set; }

        [DataMember(Order = 8, Name = "missingTableStatusMessage")]
        public string MissingTableStatusMessage { get; set; }
    }
}
