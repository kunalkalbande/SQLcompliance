using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [DataContract]
    public class AuditedDatabaseActivityResult
    {
        [DataMember(Order = 0, Name = "id")]
        public int Id { get; set; }

        [DataMember(Order = 1, Name = "name")]
        public string Name { get; set; }

        [DataMember(Order = 2, Name = "instance")]
        public string Instance { get; set; }

        [DataMember(Order = 3, Name = "isEnabled")]
        public bool IsEnabled { get; set; }

        [DataMember(Order = 4, Name = "regulationGuidelinesString")]
        public string RegulationGuidelinesString { get; set; }

        [DataMember(Order = 5, Name = "databaseAuditedActivitiesString")]
        public string DatabaseAuditedActivitiesString { get; set; }

        [DataMember(Order = 6, Name = "beforeAfterTables")]
        public string beforeAfterTablesString { get; set; }

        [DataMember(Order = 7, Name = "sensitiveColumnsTablesString")]
        public string SensitiveColumnsTablesString { get; set; }

        [DataMember(Order = 8, Name = "trustedUsersString")]
        public string TrustedUsersString { get; set; }

        [DataMember(Order = 9, Name = "eventFiltersString")]
        public string EventFiltersString { get; set; }
    }
}
