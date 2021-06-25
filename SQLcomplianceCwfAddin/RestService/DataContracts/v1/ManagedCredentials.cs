using System.Runtime.Serialization;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.ManagedInstances;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [DataContract(Name = "managedCredentials")]
    public class ManagedCredentials : Credentials
    {
        [DataMember(Name = "accountType")]
        public SqlServerSecurityModel AccountType { get; set; }
    }
}