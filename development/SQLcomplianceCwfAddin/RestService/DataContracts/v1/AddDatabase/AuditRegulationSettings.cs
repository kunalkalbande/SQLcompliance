using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AddDatabase
{
    [DataContract(Name = "auditRegulationSettings")]
    public class AuditRegulationSettings
    {
        [DataMember(Order = 1, Name = "pci")]
        public bool PCI { get; set; }

        [DataMember(Order = 2, Name = "hipaa")]
        public bool HIPAA { get; set; }

        [DataMember(Order = 3, Name = "disa")]
        public bool DISA { get; set; }

        [DataMember(Order = 4, Name = "nerc")]
        public bool NERC { get; set; }

        [DataMember(Order = 5, Name = "cis")]
        public bool CIS { get; set; }

        [DataMember(Order = 6, Name = "sox")]
        public bool SOX { get; set; }

        [DataMember(Order = 7, Name = "ferpa")]
        public bool FERPA { get; set; }

        [DataMember(Order = 8, Name = "gdpr")]
        public bool GDPR { get; set; }
    }
}
