using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.ManagedInstances.Credentials
{
    [DataContract(Name = "credentialsValidationRequest")]
    public class BatchInstancesCredentialsRequest
    {
        [DataMember(Name = "instancesIds")]
        public IEnumerable<int> InstancesIds { get; set; }

        [DataMember(Name = "credentials")]
        public ManagedCredentials Credentials { get; set; }
    }
}
