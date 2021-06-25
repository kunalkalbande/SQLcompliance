using SQLcomplianceCwfAddin.RestService.DataContracts.v1.AddDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [DataContract]
    public class DatabaseTableDetailsFilter
    {            
            
            [DataMember(Order = 0, Name = "databaseName")]
            public string DatabaseName { get; set; }

            [DataMember(Order = 1, Name = "tableName")]
            public string TableName { get; set; }

            [DataMember(Order = 2, Name = "serverId")]
            public int ServerId { get; set; }

            [DataMember(Order = 3, Name = "databaseList")]
            public List<AuditedDatabaseInfo> DatabaseList { get; set; }
       
    }
}
