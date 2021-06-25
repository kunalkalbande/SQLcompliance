using SQLcomplianceCwfAddin.RestService.DataContracts.v1.AuditReports;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AuditReports
{
    [DataContract]
    public class RegulatoryComplianceResponse
    {
        public RegulatoryComplianceResponse()
        {
            AuditRegulatoryCompliance = new List<RegulatoryComplianceData>();
        }

        [DataMember]
        public List<RegulatoryComplianceData> AuditRegulatoryCompliance{ get; set; }

        internal void Add(RegulatoryComplianceData regulatoryComplianceData)
        {
            AuditRegulatoryCompliance.Add(regulatoryComplianceData);
        }
    }
}
