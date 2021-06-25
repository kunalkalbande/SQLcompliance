using System.Collections.Generic;
using System.Runtime.Serialization;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.DatabaseProperties;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AddDatabase
{
    [DataContract(Name = "auditedDatabaseInfo")]
    public class AuditedDatabaseInfo
    {
        [DataMember(Order = 0, Name = "id")]
       public int Id { get; set; }

        [DataMember(Order = 1, Name = "serverId")]
        public int ServerId { get; set; }

        [DataMember(Order = 2, Name = "isEnabled")]
        public bool IsEnabled { get; set; }

        //This data is filled only for regulation flow of add database wizard
        [DataMember(Order = 3, Name = "sensitiveTableColumnData")]
        public SensitiveColumnTableData SensitiveTableColumnData { get; set; }
      
        [DataMember(Order = 4, Name = "name")]
        public string Name { get; set; }
        [DataMember(Order = 5, Name = "beforeAfterTableList")]
        public List<DatabaseObject> BeforeAfterTableList { get; set; }
    }
}
