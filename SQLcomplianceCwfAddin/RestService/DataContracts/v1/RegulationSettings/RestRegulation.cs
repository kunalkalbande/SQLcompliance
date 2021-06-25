using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.RegulationSettings
{
    [DataContract(Name = "regulation")]
    public class RestRegulation
    {
        [DataMember(Order = 0, Name = "name")]
        public string Name { get; set; }

        [DataMember(Order = 1, Name = "description")]
        public string Description { get; set; }

        [DataMember(Order = 2, Name = "type")]
        public RestRegulationType Type { get; set; }
    }
}
