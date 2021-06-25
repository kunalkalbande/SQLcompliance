using System;
using System.Runtime.Serialization;
using SQLcomplianceCwfAddin.Helpers;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AuditReports
{
    [Serializable]
    [DataContract(Name = "RegulatoryComplianceRequest")]
    public class RegulatoryComplianceRequest
    {
        [DataMember(Order = 1, Name = "serverName")]
        public string ServerName { get; set; }

        [DataMember(Order = 2, Name = "databaseName")]
        public string DatabaseName { get; set; }

        [DataMember(Order = 3, Name = "auditSettings")]
        public int AuditSettings { get; set; }

        [DataMember(Order = 4, Name = "regulationGuidelines")]
        public int RegulationGuidelines { get; set; }

        [DataMember(Order = 5, Name = "values")]
        public int Values { get; set; }

    }
}
