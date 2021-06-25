using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AgentProperties
{
    [DataContract(Name = "agentDeployment")]
    public class AgentDeployment
    {
        [DataMember(Order = 0, Name = "serviceAccount")]
        public string ServiceAccount { get; set; }

        [DataMember(Order = 1, Name = "wasManuallyDeployed")]
        public bool WasManuallyDeployed { get; set; }
    }
}
