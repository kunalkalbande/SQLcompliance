using System;
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{   [Serializable]
    [DataContract]
    public class DMLActivityRequest
    {
        [DataMember(Order = 1, Name = "instance")]
        public string Instance { get; set; }

        [DataMember(Order = 2, Name = "database")]
        public string Database { get; set; }

        [DataMember(Order = 3, Name = "loginName")]
        public string LoginName { get; set; }

        [DataMember(Order = 4, Name = "objectName")]
        public string ObjectName { get; set; }

        [DataMember(Order = 5, Name = "schemaName")]
        public string SchemaName { get; set; }

        [DataMember(Order = 6, Name = "to")]
        public DateTime To { get; set; }

        [DataMember(Order = 7, Name = "from")]
        public DateTime From { get; set; }

        [DataMember(Order = 8, Name = "user")]
        public string User { get; set; }

        [DataMember(Order = 9, Name = "key")]
        public string Key { get; set; }

        [DataMember(Order = 10, Name = "sortColumn")]
        public string SortColumn { get; set; }

        [DataMember(Order = 11, Name = "rowCount")]
        public int RowCount { get; set; }

    }
}
