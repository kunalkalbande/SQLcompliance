using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [DataContract]
    public class InstanceAvailableResponse
    {
        [DataMember(Order = 0, Name = "isAvailable")]
        public bool IsAvailable { get; set; }

        [DataMember(Order = 1, Name = "errorMessage")]
        public string ErrorMessage { get; set; }
    }
}
