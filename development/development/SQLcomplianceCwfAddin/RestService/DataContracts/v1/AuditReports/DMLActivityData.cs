using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AuditReports
{
    [Serializable]
    [DataContract(Name = "DMLActivityData")]
    public class DMLActivityData
    {
        [DataMember(Order = 0, Name = "eventType")]
        public string EventType { get; set; }

        [DataMember(Order = 1, Name = "startTime")]
        public string StartTime { get; set; }

        [DataMember(Order = 2, Name = "loginName")]
        public string LoginName { get; set; }

        [DataMember(Order = 3, Name = "databaseName")]
        public string DatabaseName { get; set; }

        [DataMember(Order = 4, Name = "table")]
        public string Table { get; set; }

        [DataMember(Order = 5, Name = "columnName")]
        public string ColumnName { get; set; }

        [DataMember(Order = 6, Name = "beforeValue")]
        public string BeforeValue { get; set; }

        [DataMember(Order = 7, Name = "afterValue")]
        public string AfterValue { get; set; }

        [DataMember(Order = 8, Name = "primaryKeys")]
        public string Keys { get; set; }
    }

}
