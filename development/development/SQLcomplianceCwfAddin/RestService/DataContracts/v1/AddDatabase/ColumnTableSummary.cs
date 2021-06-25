using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AddDatabase
{
    [DataContract(Name = "columnTableSummary")]
    public class ColumnTableSummary
    {
            [DataMember(Order = 0, Name = "DatabaseName")]
            public string DatabaseName { get; set; }
        
            [DataMember(Order = 1, Name = "TableName")]
            public string TableName { get; set; }

            [DataMember(Order = 2, Name = "FieldName")]
            public string FieldName { get; set; }

            [DataMember(Order = 3, Name = "DataType")]
            public string DataType { get; set; }

            [DataMember(Order = 4, Name = "MatchStr")]
            public string MatchStr { get; set; }

          
       
    }
}
