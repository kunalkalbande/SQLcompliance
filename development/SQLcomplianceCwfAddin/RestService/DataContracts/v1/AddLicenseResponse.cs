using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [DataContract]
    public class AddLicenseResponse
    {
        [DataMember(Name = "errorMessage", Order = 0)]
        public string ErrorMessage { get; set; }

        [DataMember(Name = "success", Order = 1)]
        public bool Success { get; set; }
    }
}
