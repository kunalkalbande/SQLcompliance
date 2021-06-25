using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AddDatabase
{
    [DataContract(Name = "serverLogin")]
    public class ServerLogin
    {
        [DataMember(Order = 1, Name = "sid")]
        public string Sid { get; set; }

        [DataMember(Order = 2, Name = "name")]
        public string Name { get; set; }
    }
}
