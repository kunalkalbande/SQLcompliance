using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AddDatabase
{
    [DataContract(Name = "databaseSummary")]
    public class DatabaseSummary
    {
        [DataMember(Order = 0, Name = "DbName")]
        public string DbName { get; set; }

        [DataMember(Order = 1, Name = "DatabaseId")]
        public int DatabaseId { get; set; }

        [DataMember(Order = 2, Name = "ServerId")]
        public int ServerId { get; set; }
        
    }
}
