
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.RegulationSettings
{
    [DataContract(Name = "regulationSection")]
    public class RestRegulationSection
    {
        [DataMember(Order = 0, Name = "name")]
        public string Name { get; set; }

        [DataMember(Order = 1, Name = "serverEvents")]
        public string ServerEvents { get; set; }

        [DataMember(Order = 2, Name = "databaseEvents")]
        public string DatabaseEvents { get; set; }
    }
}
