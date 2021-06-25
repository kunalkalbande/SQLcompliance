using System.Runtime.Serialization;
using Idera.SQLcompliance.Core.Event;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AddDatabase
{
    [DataContract(Name = "auditActivity")]
    public class AuditActivity
    {
        [DataMember(Order = 0, Name = "auditDDL")]
        public bool AuditDDL { get; set; }

        [DataMember(Order = 1, Name = "auditSecurity")]
        public bool AuditSecurity { get; set; }

        [DataMember(Order = 2, Name = "auditAdmin")]
        public bool AuditAdmin { get; set; }

        [DataMember(Order = 3, Name = "auditDML")]
        public bool AuditDML { get; set; }

        [DataMember(Order = 4, Name = "auditSELECT")]
        public bool AuditSELECT { get; set; }

        [DataMember(Order = 5, Name = "auditCaptureSQL")]
        public bool AuditCaptureSQL { get; set; }

        [DataMember(Order = 6, Name = "auditCaptureTrans")]
        public bool AuditCaptureTrans { get; set; }

        [DataMember(Order = 7, Name = "allowCaptureSql")]
        public bool AllowCaptureSql { get; set; }

        [DataMember(Order = 8, Name = "isAgentVersionSupported")]
        public bool IsAgentVersionSupported { get; set; }

        [DataMember(Order = 9, Name = "auditAccessCheck")]
        public AccessCheckFilter AuditAccessCheck { get; set; }

        // The following properties are used only  for audting user activities
        [DataMember(Order = 10, Name = "auditAllUserActivities")]
        public bool AuditAllUserActivities { get; set; }

        [DataMember(Order = 11, Name = "auditLogins")]
        public bool AuditLogins { get; set; }

        [DataMember(Order = 12, Name = "auditFailedLogins")]
        public bool AuditFailedLogins { get; set; }

        [DataMember(Order = 13, Name = "auditDefinedEvents")]
        public bool AuditDefinedEvents { get; set; }

        [DataMember(Order = 14, Name = "auditUserCaptureDDL")]
        public bool AuditCaptureDDL { get; set; }
		// SQLCm 5.4_4.1.1_Extended Events
        [DataMember(Order = 15, Name = "auditUserExtendedEvents")]
        public bool AuditUserExtendedEvents { get; set; }
        // SQLCm 5.4_4.1.1_Extended Events B
        [DataMember(Order = 16, Name = "auditCaptureSQLXE")]
        public bool AuditCaptureSQLXE { get; set; }

        [DataMember(Order = 17, Name = "isAuditLogEnabled")]
        public bool IsAuditLogEnabled { get; set; }
        [DataMember(Order = 18, Name = "auditSensitiveColumns")]
        public bool AuditSensitiveColumns { get; set; }
        [DataMember(Order = 19, Name = "auditBeforeAfter")]
        public bool AuditBeforeAfter { get; set; }
        [DataMember(Order = 20, Name = "auditPrivilegedUsers")]
        public bool AuditPrivilegedUsers { get; set; }
        [DataMember(Order=21,Name="customEnabled")]
        public  bool CustomEnabled { get; set; }
        /// <summary>
        /// SQLCM-5375 Release 5.6 - 6.1.4.3 - Capture Logout Events
        /// </summary>
        [DataMember(Order = 22, Name = "auditLogouts")]
        public bool AuditLogouts { get; set; }

    }
}
