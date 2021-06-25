using System;
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [Serializable]
    [DataContract(Name = "User")]
    public class User
    {
        [DataMember(Order = 1, Name = "UserType")]
        public string UserType { get; set; }

        [DataMember(Order = 2, Name = "Account")]
        public string Account { get; set; }

        [DataMember(Order = 3, Name = "Sid")]
        public string Sid { get; set; }

        [DataMember(Order = 4, Name = "IsEnabled")]
        public bool IsEnabled { get; set; }
    }
}
