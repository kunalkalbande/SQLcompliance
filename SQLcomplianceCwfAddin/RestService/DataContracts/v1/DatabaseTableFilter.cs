using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [DataContract]
    public class DatabaseTableFilter
    {
        [DataMember(Order = 0, Name = "serverId")]
        public int ServerId { get; set; }

        [DataMember(Order = 1, Name = "databaseName")]
        public string DatabaseName { get; set; }

        [DataMember(Order = 2, Name = "tableNameSearchText")]
        public string TableNameSearchText { get; set; }
    }
}
