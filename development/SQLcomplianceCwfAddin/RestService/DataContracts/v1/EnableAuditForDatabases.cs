using System.Collections.Generic;
using System.Runtime.Serialization;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.AddDatabase;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [DataContract]
    public class EnableAuditForDatabases
    {
        [DataMember(Order = 0, Name = "databaseIdList")]
        public List<int> DatabaseIdList { get; set; }

        [DataMember(Order = 1, Name = "enable")]
        public bool Enable { get; set; }
    }
}
