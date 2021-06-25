using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AddDatabase
{
    [DataContract(Name = "regulationCustomDetail")]
    public class RegulationCustomDetail
    {
        [DataMember(Order = 0, Name = "auditedServerActivities")]
        public AuditActivity AuditedServerActivities { get; set; }

        [DataMember(Order = 1, Name = "auditedDatabaseActivities")]
        public AuditActivity AuditedDatabaseActivities { get; set; }
    }
}
