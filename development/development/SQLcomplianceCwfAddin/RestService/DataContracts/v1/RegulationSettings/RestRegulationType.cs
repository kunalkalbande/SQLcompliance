using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.RegulationSettings
{
    [DataContract(Name = "regulationType")]
    public enum RestRegulationType
    {
        [EnumMember(Value = "noRegulation")]
        NoRegulation,

        [EnumMember(Value = "pci")]
        PCI,

        [EnumMember(Value = "hipaa")]
        HIPAA,

        [EnumMember(Value = "disa")]
        DISA,
		
		[EnumMember(Value = "nerc")]
        NERC,

        [EnumMember(Value = "cis")]
        CIS,

         [EnumMember(Value = "sox")]
        SOX,

        [EnumMember(Value = "ferpa")]
        FERPA

       
    }
}
