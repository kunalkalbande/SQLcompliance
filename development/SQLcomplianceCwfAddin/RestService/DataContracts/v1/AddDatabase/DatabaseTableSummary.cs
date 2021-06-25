using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AddDatabase
{
    [DataContract(Name = "databaseTableSummary")]
    public class DatabaseTableSummary
    {
        [DataMember(Order = 1, Name = "DatabaseName")]
        public string DatabaseName { get; set; }

        [DataMember(Order = 2, Name = "SchemaTableName")]
        public string SchemaTableName { get; set; }

        [DataMember(Order = 3, Name = "Size")]
        public decimal Size { get; set; }

        //[DataMember(Order = 4, Name = "SizeString")]
        //public string SizeString { get { return string.Format("{0:0.00}", Size); } }

        [DataMember(Order = 5, Name = "RowCount")]
        public long RowCount { get; set; }

        [DataMember(Order = 6, Name = "ColumnsIdentified")]
        public int ColumnsIdentified { get; set; }

        //[DataMember(Order = 8, Name = "SchemaName")]
        //public string SchemaName { get; set; }
    }
}
