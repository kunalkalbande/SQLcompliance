using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AddDatabase
{
    [DataContract(Name = "dmlSelectFilters")]
    public class DmlSelectFilters
    {
        //Audit all database objects
        [DataMember(Order = 0, Name = "auditDmlAll")]
        public bool AuditDmlAll { get; set; }

        //Audit the following database objects
        [DataMember(Order = 1, Name = "auditUserTables")]
        public AuditUserTables AuditUserTables { get; set; }

        [DataMember(Order = 2, Name = "userTableList")]
        public List<DatabaseObject> UserTableList { get; set; }

        [DataMember(Order = 3, Name = "auditSystemTables")]
        public bool AuditSystemTables { get; set; }

        [DataMember(Order = 4, Name = "auditStoredProcedures")]
        public bool AuditStoredProcedures { get; set; }

        //Audit all other object types (views, indexes, etc.)
        [DataMember(Order = 5, Name = "auditDmlOther")]
        public bool AuditDmlOther { get; set; }
    }
}
