using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AuditReports
{
    [Serializable]
    [DataContract(Name = "ConfigurationCheckDatabaseConfiguration")]
    public class ConfigurationCheckProcedureData
    {
        [DataMember(Order =0,Name="flag")]
        public int flag { get; set; }

        [DataMember(Order = 1, Name = "srvId")]
        public int srvId { get; set; }

        [DataMember(Order = 2, Name = "instance")]
        public string instance { get; set; }

        [DataMember(Order = 3, Name = "isDeployed")]
        public int isDeployed { get; set; }

        [DataMember(Order = 4, Name = "instanceServer")]
        public string instanceServer { get; set; }

        [DataMember(Order = 5, Name = "eventDatabase")]
        public string eventDatabase { get; set; }

        [DataMember(Order = 6, Name = "sqlDatabaseId")]
        public int sqlDatabaseId { get; set; }

        [DataMember(Order = 7, Name = "name")]
        public string name { get; set; }

        [DataMember(Order = 8, Name = "agentVersion")]
        public string agentVersion { get; set; }

        [DataMember(Order = 9, Name = "auditAdmin")]
        public int auditAdmin { get; set; }

        [DataMember(Order = 10, Name = "auditCaptureSQL")]
        public int auditCaptureSQL { get; set; }

        [DataMember(Order = 11, Name = "auditCaptureSQLXE")]
        public int auditCaptureSQLXE { get; set; }

        [DataMember(Order = 12, Name = "auditDBCC")]
        public int auditDBCC { get; set; }

        [DataMember(Order = 13, Name = "auditDDL")]
        public int auditDDL { get; set; }

        [DataMember(Order = 14, Name = "auditDML")]
        public int auditDML { get; set; }

        [DataMember(Order = 15, Name = "auditDMLAll")]
        public int auditDMLAll { get; set; }

        [DataMember(Order = 16, Name = "auditDMLOther")]
        public int auditDMLOther { get; set; }

        [DataMember(Order = 17, Name = "auditExceptions")]
        public int auditExceptions { get; set; }

        [DataMember(Order = 18, Name = "auditFailedLogins")]
        public int auditFailedLogins { get; set; }

        [DataMember(Order = 19, Name = "auditFailures")]
        public int auditFailures { get; set; }

        [DataMember(Order = 20, Name = "auditLogins")]
        public int auditLogins { get; set; }

        [DataMember(Order = 21, Name = "auditLogouts")]
        public int auditLogouts { get; set; }

        [DataMember(Order = 22, Name = "auditPrivUsersList")]
        public string auditPrivUsersList { get; set; }

        [DataMember(Order = 23, Name = "auditSecurity")]
        public int auditSecurity { get; set; }

        [DataMember(Order = 24, Name = "auditSELECT")]
        public int auditSELECT { get; set; }

        [DataMember(Order = 25, Name = "auditSystemEvents")]
        public int auditSystemEvents { get; set; }

        [DataMember(Order = 26, Name = "auditTrace")]
        public int auditTrace { get; set; }

        [DataMember(Order = 27, Name = "auditUDE")]
        public int auditUDE { get; set; }

        [DataMember(Order = 28, Name = "auditUserAdmin")]
        public int auditUserAdmin { get; set; }

        [DataMember(Order = 29, Name = "auditUserAll")]
        public int auditUserAll { get; set; }

        [DataMember(Order = 30, Name = "auditUserCaptureDDL")]
        public int auditUserCaptureDDL { get; set; }

        [DataMember(Order = 31, Name = "auditUserCaptureSQL")]
        public int auditUserCaptureSQL { get; set; }

        [DataMember(Order = 32, Name = "auditUserCaptureTrans")]
        public int auditUserCaptureTrans { get; set; }

        [DataMember(Order = 33, Name = "auditUserDDL")]
        public int auditUserDDL { get; set; }

        [DataMember(Order = 34, Name = "auditUserDML")]
        public int auditUserDML { get; set; }

        [DataMember(Order = 35, Name = "auditUserExceptions")]
        public int auditUserExceptions { get; set; }

        [DataMember(Order = 36, Name = "auditUserExtendedEvents")]
        public int auditUserExtendedEvents { get; set; }

        [DataMember(Order = 37, Name = "auditUserFailedLogins")]
        public int auditUserFailedLogins { get; set; }

        [DataMember(Order = 38, Name = "auditUserFailures")]
        public int auditUserFailures { get; set; }

        [DataMember(Order = 39, Name = "auditUserLogins")]
        public int auditUserLogins { get; set; }

        [DataMember(Order = 40, Name = "auditUserLogouts")]
        public int auditUserLogouts { get; set; }

        [DataMember(Order = 41, Name = "auditUsers")]
        public string auditUsers { get; set; }

        [DataMember(Order = 42, Name = "auditUserSecurity")]
        public int auditUserSecurity { get; set; }

        [DataMember(Order = 43, Name = "auditUserSELECT")]
        public int auditUserSELECT { get; set; }

        [DataMember(Order = 44, Name = "auditUsersList")]
        public string auditUsersList { get; set; }

        [DataMember(Order = 45, Name = "auditUserTables")]
        public int auditUserTables { get; set; }

        [DataMember(Order = 46, Name = "auditUserUDE")]
        public int auditUserUDE { get; set; }

        [DataMember(Order = 47, Name = "isAuditLogEnabled")]
        public int isAuditLogEnabled { get; set; }

        [DataMember(Order = 48, Name = "auditCaptureTrans")]
        public int auditCaptureTrans { get; set; }

        [DataMember(Order = 49, Name = "auditCaptureDDL")]
        public int auditCaptureDDL { get; set; }

        [DataMember(Order = 50, Name = "auditSystemTables")]
        public int auditSystemTables { get; set; }

        [DataMember(Order = 51, Name = "auditStoredProcedures")]
        public int auditStoredProcedures { get; set; }

        [DataMember(Order = 52, Name = "auditBroker")]
        public int auditBroker { get; set; }

        [DataMember(Order = 53, Name = "auditDataChanges")]
        public int auditDataChanges { get; set; }

        [DataMember(Order = 54, Name = "auditSensitiveColumns")]
        public int auditSensitiveColumns { get; set; }

        [DataMember(Order = 55, Name = "auditTrustedUserslist")]
        public string auditTrustedUsersList { get; set; }

       

    }
}
