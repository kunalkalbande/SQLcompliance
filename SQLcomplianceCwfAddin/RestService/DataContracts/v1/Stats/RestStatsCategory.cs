using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.Stats
{
    [DataContract(Name = "statsCategory")]
    public enum RestStatsCategory : byte
    {
        [EnumMember(Value = "unknown")]
        Unknown = 0,

        [EnumMember(Value = "auditedInstance")]
        AuditedInstance = 1,

        [EnumMember(Value = "auditedDatabase")]
        AuditedDatabase = 2,
        //ProcessedEvents = 3,  //Not used.  Use EventProcessed instead.

        [EnumMember(Value = "alerts")]
        Alerts = 4,

        [EnumMember(Value = "privUserEvents")]
        PrivUserEvents = 5,

        [EnumMember(Value = "failedLogin")]
        FailedLogin = 6,

        [EnumMember(Value = "userDefinedEvents")]
        UserDefinedEvents = 7,

        [EnumMember(Value = "admin")]
        Admin = 8,

        [EnumMember(Value = "ddl")]
        Ddl = 9,

        [EnumMember(Value = "security")]
        Security = 10,

        [EnumMember(Value = "dml")]
        Dml = 11,

        [EnumMember(Value = "insert")]
        Insert = 12,

        [EnumMember(Value = "update")]
        Update = 13,

        [EnumMember(Value = "delete")]
        Delete = 14,

        [EnumMember(Value = "select")]
        Select = 15,

        [EnumMember(Value = "logins")]
        Logins = 16,

        [EnumMember(Value = "hdSpace")]
        HdSpace = 17,

        [EnumMember(Value = "integrityCheck")]
        IntegrityCheck = 18,

        [EnumMember(Value = "execute")]
        Execute = 19,

        [EnumMember(Value = "eventReceived")]
        EventReceived = 20,

        [EnumMember(Value = "eventProcessed")]
        EventProcessed = 21,

        [EnumMember(Value = "eventFiltered")]
        EventFiltered = 22,

        //start sqlcm 5.6 - 5363
        [EnumMember(Value = "logout")]
        Logout = 23,
        //end sqlcm 5.6 - 5363
        [EnumMember(Value = "maxValue")]
        MaxValue = 24,

        [EnumMember(Value = "widgetAuditInstance")]
        WidgetValue = 100
    }
}
