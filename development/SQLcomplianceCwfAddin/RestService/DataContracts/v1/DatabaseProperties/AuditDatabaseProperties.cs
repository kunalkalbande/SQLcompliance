using System;
using System.Runtime.Serialization;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.AddDatabase;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.DatabaseProperties
{
    [DataContract(Name = "auditDatabaseProperties")]
    public class AuditDatabaseProperties
    {
        [DataMember(Order = 0, Name = "databaseId")]
        public int DatabaseId { get; set; }

        //General - Database
        [DataMember(Order = 1, Name = "serverInstance")]
        public string ServerInstance { get; set; }

        [DataMember(Order = 2, Name = "databaseName")]
        public string DatabaseName { get; set; }

        [DataMember(Order = 3, Name = "description")]
        public string Description { get; set; }

        //General - Status
        [DataMember(Order = 4, Name = "auditingEnableStatus")]
        public bool AuditingEnableStatus { get; set; }

        [DataMember(Order = 5, Name = "createdDateTime")]
        public DateTime CreatedDateTime { get; set; }

        [DataMember(Order = 6, Name = "lastModifiedDateTime")]
        public DateTime LastModifiedDateTime { get; set; }

        [DataMember(Order = 7, Name = "lastChangedStatusDateTime")]
        public DateTime LastChangedStatusDateTime { get; set; }

        //Audit Activities
        [DataMember(Order = 8, Name = "auditedActivities")]
        public AuditActivity AuditedActivities { get; set; }

        //Audit User Activities
        [DataMember(Order = 9, Name = "auditedPrivilegedUserActivities")]
        public AuditActivity AuditedPrivilegedUserActivities { get; set; }

        //DML / SELECT Filters
        [DataMember(Order = 10, Name = "dmlSelectFilters")]
        public DmlSelectFilters DmlSelectFilters { get; set; }

        [DataMember(Order = 11, Name = "trustedRolesAndUsers")]
        public ServerRolesAndUsers TrustedRolesAndUsers { get; set; }

        [DataMember(Order = 12, Name = "privilegedRolesAndUsers")]
        public ServerRolesAndUsers PrivilegedRolesAndUsers { get; set; }

        [DataMember(Order = 13, Name = "auditBeforeAfterData")]
        public AuditBeforeAfterData AuditBeforeAfterData { get; set; }

        [DataMember(Order = 14, Name = "sensitiveColumnTableData")]
        public SensitiveColumnTableData SensitiveColumnTableData { get; set; }
    }
}
