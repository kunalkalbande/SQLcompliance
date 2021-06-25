using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.ServerProperties
{
    [DataContract(Name = "serverAdvancedProperties")]
    public class ServerAdvancedProperties
    {
        [DataMember(Order = 0, Name = "defaultDatabasePermissions")]
        public DatabaseReadAccessLevel DefaultDatabasePermissions { get; set; }

        [DataMember(Order = 1, Name = "sqlStatementLimit")]
        public int SQLStatementLimit { get; set; }
    }
}
