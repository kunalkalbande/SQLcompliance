using System;
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [Serializable]
    [DataContract]
    public class RowCountRequest
    {
        [DataMember(Order = 1, Name = "instance")]
        public string instance { get; set; }

        [DataMember(Order = 2, Name = "database")]
        public string Database { get; set; }

        [DataMember(Order = 3, Name = "loginName")]
        public string LoginName { get; set; }

        [DataMember(Order = 4, Name = "objectName")]
        public string objectName { get; set; }

        [DataMember(Order = 5, Name = "columnName")]
        public string columnName { get; set; }

        [DataMember(Order = 6, Name = "to")]
        public DateTime to { get; set; }

        [DataMember(Order = 7, Name = "from")]
        public DateTime from { get; set; }

        [DataMember(Order = 8, Name = "privilegedUsers")]
        public string privilegedUsers { get; set; }

        [DataMember(Order = 9, Name = "sql")]
        public bool sql { get; set; }

        [DataMember(Order = 10, Name = "rowCountThreshold")]
        public int rowCountThreshold { get; set; }
    }
}
