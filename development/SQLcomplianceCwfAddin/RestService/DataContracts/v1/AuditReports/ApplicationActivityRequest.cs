using System;
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [Serializable]
    [DataContract]
    public class ApplicationActivityRequest
    {
        [DataMember(Order = 1, Name = "instance")]
        public string Instance { get; set; }

        [DataMember(Order = 2, Name = "database")]
        public string Database { get; set; }

        [DataMember(Order = 3, Name = "application")]
        public string Application { get; set; }

        [DataMember(Order = 4, Name = "from")]
        public DateTime From { get; set; }

        [DataMember(Order = 5, Name = "to")]
        public DateTime To { get; set; }

        [DataMember(Order = 6, Name = "sql")]
        public bool Sql { get; set; }

        [DataMember(Order = 7, Name = "user")]
        public bool User { get; set; }

        [DataMember(Order = 8, Name = "category")]
        public int Category { get; set; }

        [DataMember(Order = 9, Name = "sortColumn")]
        public string SortColumn { get; set; }

        [DataMember(Order = 10, Name = "rowCount")]
        public int RowCount { get; set; }
    }
}
