using System;
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AddDatabase
{
    [DataContract(Name = "serverRole")]
    public class ServerRole
    {
        [DataMember(Order = 1, Name = "id")]
        public int Id;

        [DataMember(Order = 2, Name = "name")]
        public string Name;

        [DataMember(Order = 3, Name = "fullName")]
        public string FullName;
    }
}
