using System;
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [Serializable]
    [DataContract]
    public class LoginDeletionHistoryRequest
    {
        [DataMember(Order = 1, Name = "instance")]
        public string Instance { get; set; }

        [DataMember(Order = 2, Name = "login")]
        public string Login { get; set; }

        [DataMember(Order = 3, Name = "to")]
        public DateTime To { get; set; }

        [DataMember(Order = 4, Name = "from")]
        public DateTime From { get; set; }

        [DataMember(Order = 5, Name = "sortColumn")]
        public string SortColumn { get; set; }

        [DataMember(Order = 6, Name = "rowCount")]
        public int RowCount { get; set; }

    }
}
