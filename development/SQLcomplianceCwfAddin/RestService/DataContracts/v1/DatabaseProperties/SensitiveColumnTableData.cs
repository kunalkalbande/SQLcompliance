using System.Collections.Generic;
using System.Runtime.Serialization;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.AddDatabase;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.DatabaseProperties
{
    [DataContract(Name = "sensitiveColumnTableData")]
    public class SensitiveColumnTableData
    {
        [DataMember(Order = 0, Name = "statusMessaage")]
        public string StatusMessaage { get; set; }

        [DataMember(Order = 1, Name = "missingTableStatusMessage")]
        public string MissingTableStatusMessage { get; set; }

        [DataMember(Order = 2, Name = "columnsSupported")]
        public bool ColumnsSupported { get; set; }

        [DataMember(Order = 3, Name = "sensitiveTableColumnDictionary")]
        public List<SensitiveColumnData> SensitiveColumnData { get; set; }
        public Dictionary<string, DatabaseObject> SensitiveTableColumnDictionary { get; set; }
    }
    [DataContract(Name = "sensitiveColumnTableData")]
    public class SensitiveColumnData
    {
        [DataMember(Order = 1, Name = "Key")]
        public string Key { get; set; }
        [DataMember(Order = 2, Name = "Type")]
        public string Type { get; set; }
        [DataMember(Order = 3, Name = "datasetTableList")]
        public List<DatabaseObject> DatasetTableList { get; set; }
    }
}
