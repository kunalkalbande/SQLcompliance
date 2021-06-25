
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [DataContract]
    public class RemoveDatabaseRequest
    {
        [DataMember(Order = 0, Name = "databaseId")]
        public int DatabaseId { get; set; }
    }
}
