using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AuditReports
{
    [Serializable]
    [DataContract(Name = "ConfigurationCheckDataServer")]
    public class ConfigurationCheckJSONServer
    {
        public ConfigurationCheckJSONServer()
        {
            databasesConfigurationList = new List<ConfigurationCheckProcedureData>();
        }
        [DataMember(Order = 0, Name = "srvId")]
        public int srvId { get; set; }

        [DataMember(Order = 1, Name = "instance")]
        public string instance { get; set; }


        [DataMember(Order = 2, Name = "isDeployed")]
        public int isDeployed { get; set; }


        [DataMember(Order = 3, Name = "eventDatabase")]
        public string eventDatabase { get; set; }

        

        [DataMember(Order = 4, Name = "agentVersion")]
        public string agentVersion { get; set; }

        [DataMember(Order = 5, Name = "auditAdmin")]
        public int auditAdmin { get; set; }

        [DataMember(Order = 6, Name = "auditCaptureSql")]
        public int auditCaptureSql { get; set; }

        [DataMember(Order = 7, Name = "auditCaptureSqlXE")]
        public int auditCaptureSqlXE { get; set; }

        [DataMember(Order = 8, Name = "auditDBCC")]
        public int auditDBCC { get; set; }

        [DataMember(Order = 9, Name = "auditDDL")]
        public int auditDDL { get; set; }

        [DataMember(Order = 10, Name = "auditDML")]
        public int auditDML { get; set; }

        [DataMember(Order = 11, Name = "auditExceptions")]
        public int auditExceptions { get; set; }

        [DataMember(Order = 12, Name = "auditFailedLogins")]
        public int auditFailedLogins { get; set; }

        [DataMember(Order = 13, Name = "auditFailures")]
        public int auditFailures { get; set; }

        [DataMember(Order = 14, Name = "auditLogins")]
        public int auditLogins { get; set; }

        [DataMember(Order = 15, Name = "auditLogouts")]
        public int auditLogouts { get; set; }


        [DataMember(Order = 16, Name = "auditSecurity")]
        public int auditSecurity { get; set; }

        [DataMember(Order = 17, Name = "auditSELECT")]
        public int auditSELECT { get; set; }

        [DataMember(Order = 18, Name = "auditSystemEvents")]
        public int auditSystemEvents { get; set; }

        [DataMember(Order = 19, Name = "auditTrace")]
        public int auditTrace { get; set; }

        [DataMember(Order = 20, Name = "auditUDE")]
        public int auditUDE { get; set; }

        [DataMember(Order = 21, Name = "auditUserAdmin")]
        public int auditUserAdmin { get; set; }

        [DataMember(Order = 22, Name = "auditUserAll")]
        public int auditUserAll { get; set; }

        [DataMember(Order = 23, Name = "auditUserCaptureDDL")]
        public int auditUserCaptureDDL { get; set; }

        [DataMember(Order = 24, Name = "auditUserCaptureSQL")]
        public int auditUserCaptureSQL { get; set; }

        [DataMember(Order = 25, Name = "auditUserCaptureTrans")]
        public int auditUserCaptureTrans { get; set; }

        [DataMember(Order = 26, Name = "auditUserDDL")]
        public int auditUserDDL { get; set; }

        [DataMember(Order = 27, Name = "auditUserDML")]
        public int auditUserDML { get; set; }

        [DataMember(Order = 28, Name = "auditUserExceptions")]
        public int auditUserExceptions { get; set; }

        [DataMember(Order = 29, Name = "auditUserExtendedEvents")]
        public int auditUserExtendedEvents { get; set; }

        [DataMember(Order = 30, Name = "auditUserFailedLogins")]
        public int auditUserFailedLogins { get; set; }

        [DataMember(Order = 31, Name = "auditUserFailures")]
        public int auditUserFailures { get; set; }

        [DataMember(Order = 32, Name = "auditUserLogins")]
        public int auditUserLogins { get; set; }

        [DataMember(Order = 33, Name = "auditUserLogouts")]
        public int auditUserLogouts { get; set; }

        [DataMember(Order = 34, Name = "auditUsers")]
        public string auditUsers { get; set; }

        [DataMember(Order = 35, Name = "auditUserSecurity")]
        public int auditUserSecurity { get; set; }

        [DataMember(Order = 36, Name = "auditUserSELECT")]
        public int auditUserSELECT { get; set; }


        [DataMember(Order = 37, Name = "auditUsersList")]
        public string auditUsersList { get; set; }

    

        [DataMember(Order = 38, Name = "auditUserUDE")]
        public int auditUserUDE { get; set; }

        [DataMember(Order = 39, Name = "isAuditLogEnabled")]
        public int isAuditLogEnabled { get; set; }


        [DataMember(Order = 40, Name = "auditTrustedUsersList")]
        public string auditTrustedUsersList { get; set; }

        [DataMember(Order = 41, Name = "DatabasesList")]
        public List<ConfigurationCheckProcedureData> databasesConfigurationList;

    }
}
