using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AddDatabase
{
    [DataContract(Name = "columnDetails")]
    public class ColumnDetails
    {
        [DataMember(Order = 1, Name = "DatabaseName")]
        public string DatabaseName { get; set; }

        [DataMember(Order = 2, Name = "SchemaTableName")]
        public string SchemaTableName { get; set; }

        [DataMember(Order = 3, Name = "MatchedColmuns")]
        public string MatchedColmuns { get; set; }

        [DataMember(Order = 4, Name = "DataType")]
        public string DataType { get; set; }

        [DataMember(Order = 5, Name = "MaxLength")]
        public int MaxLength { get; set; }

        [DataMember(Order = 6, Name = "StringMatched")]
        public string StringMatched { get; set; }
    }
}
