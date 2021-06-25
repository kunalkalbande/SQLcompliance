using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [DataContract]
    public class EventDistributionForDatabaseResult
    {
        public EventDistributionForDatabaseResult()
        {
            Security = -1;
            DDL = -1;
            Admin = -1;
            DML = -1;
            Select = -1;
        }

        [DataMember(Order = 1, Name = "Security")]
        public int Security { get; set; }

        [DataMember(Order = 2, Name = "DDL")]
        public int DDL { get; set; }

        [DataMember(Order = 3, Name = "Admin")]
        public int Admin { get; set; }

        [DataMember(Order = 4, Name = "DML")]
        public int DML { get; set; }

        [DataMember(Order = 5, Name = "Select")]
        public int Select { get; set; }
    }
}
