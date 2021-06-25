using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [DataContract(Name = "credentials")]
    public class Credentials
    {
        [DataMember(Name = "account")]
        public string Account { get; set; }

        [DataMember(Name = "password")]
        public string Password { get; set; }
    }
}
