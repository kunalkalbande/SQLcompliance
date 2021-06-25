using System;
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [Serializable]
    [DataContract]
    public class ObjectActivityRequest
    {
        [DataMember(Order = 1, Name = "instance")]
        public string instance { get; set; }

        [DataMember(Order = 1, Name = "database")]
        public string Database { get; set; }

        [DataMember(Order = 1, Name = "loginName")]
        public string LoginName { get; set; }

        [DataMember(Order = 2, Name = "objectName")]
        public string ObjectName { get; set; }

        [DataMember(Order = 3, Name = "category")]
        public int category { get; set; }

        [DataMember(Order = 4, Name = "to")]
        public DateTime to { get; set; }

        [DataMember(Order = 5, Name = "from")]
        public DateTime from { get; set; }

        [DataMember(Order = 6, Name = "user")]
        public bool user { get; set; }

        [DataMember(Order = 7, Name = "sql")]
        public bool sql { get; set; }

        [DataMember(Order = 5, Name = "sortColumn")]
        public string SortColumn { get; set; }

        [DataMember(Order = 6, Name = "rowCount")]
        public int RowCount { get; set; }

    }
}
