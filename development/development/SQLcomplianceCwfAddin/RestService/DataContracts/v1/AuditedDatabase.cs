using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [DataContract(Name = "auditedDatabase")]
    public class AuditedDatabase
    {
        [DataMember(Order = 0, Name = "id")]
        public int Id { get; set; }

        [DataMember(Order = 1, Name = "name")]
        public string Name { get; set; }

        [DataMember(Order = 2, Name = "instance")]
        public string Instance { get; set; }

        [DataMember(Order = 3, Name = "auditedActivities")]
        public List<string> AuditedActivities { get; set; }

        [DataMember(Order = 4, Name = "regulationGuidelines")]
        public List<string> RegulationGuidelines { get; set; }

        [DataMember(Order = 5, Name = "beforeAfterTableCount")]
        public int BeforeAfterTableCount { get; set; }

        [DataMember(Order = 6, Name = "sensitiveColumnsTableCount")]
        public int SensitiveColumnsTableCount { get; set; }

        [DataMember(Order = 7, Name = "trustedUserCount")]
        public int TrustedUserCount { get; set; }

        [DataMember(Order = 8, Name = "isEnabled")]
        public bool IsEnabled { get; set; }
    }
}
