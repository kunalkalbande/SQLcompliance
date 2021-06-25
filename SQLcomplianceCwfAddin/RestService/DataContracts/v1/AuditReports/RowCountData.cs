using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AuditReports
{

    [Serializable]
    [DataContract(Name = "RowCountData")]
    public class RowCountData
    {
        [DataMember(Order = 0, Name = "serverName")]
        public string serverName { get; set; }

        [DataMember(Order = 1, Name = "applicationName")]
        public string ApplicationName { get; set; }

        [DataMember(Order = 2, Name = "databaseName")]
        public string DatabaseName { get; set; }

        [DataMember(Order = 3, Name = "eventType")]
        public string EventType { get; set; }

        [DataMember(Order = 4, Name = "loginName")]
        public string LoginName { get; set; }

        [DataMember(Order = 5, Name = "targetObject")]
        public string TargetObject { get; set; }

        [DataMember(Order = 6, Name = "startTime")]
        public string StartTime { get; set; }

        [DataMember(Order = 7, Name = "sqlText")]
        public string SqlText { get; set; }

        [DataMember(Order = 8, Name = "roleName")]
        public string RoleName { get; set; }

        [DataMember(Order = 9, Name = "spid")]
        public string Spid { get; set; }

        [DataMember(Order = 10, Name = "rowCounts")]
        public string RowCounts { get; set; }

        [DataMember(Order = 11, Name = "columnName")]
        public string ColumnName { get; set; }
    }
}

