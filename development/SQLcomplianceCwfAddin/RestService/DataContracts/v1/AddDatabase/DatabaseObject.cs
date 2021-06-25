using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AddDatabase
{
    [DataContract(Name = "databaseObject")]
    public class DatabaseObject
    {
        [DataMember(Order = 1, Name = "id")]
        public int Id { get; set; }

        [DataMember(Order = 2, Name = "objectId")]
        public int ObjectId { get; set; }

        [DataMember(Order = 3, Name = "databaseId")]
        public int DatabaseId { get; set; }

        [DataMember(Order = 4, Name = "serverId")]
        public int ServerId { get; set; }

        [DataMember(Order = 5, Name = "objectType")]
        public ObjectType ObjectType { get; set; }

        [DataMember(Order = 6, Name = "tableName")]
        public string TableName { get; set; }

        [DataMember(Order = 7, Name = "fullTableName")]
        public string FullTableName { get; set; }

        [DataMember(Order = 8, Name = "schemaName")]
        public string SchemaName { get; set; }

        [DataMember(Order = 9, Name = "rowLimit")]
        public int RowLimit { get; set; }

        [DataMember(Order = 10, Name = "selectedColumns")]
        public bool SelectedColumns { get; set; }

        [DataMember(Order = 11, Name = "columnList")]
        public List<string> ColumnList { get; set; }
        [DataMember(Order = 12, Name = "type")]
        public string Type { get; set; }
        [DataMember(Order = 13, Name = "columnId")]
        public int ColumnId { get; set; }
    }
}
