using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AgentProperties
{
    [DataContract(Name = "agentTraceOptions")]
    public class AgentTraceOptions
    {
        [DataMember(Order = 0, Name = "agentTraceDirectory")]
        public string AgentTraceDirectory { get; set; }

        [DataMember(Order = 1, Name = "traceFileRolloverSize")]
        public long TraceFileRolloverSize { get; set; }

        [DataMember(Order = 2, Name = "collectionInterval")]
        public int CollectionInterval { get; set; }

        [DataMember(Order = 3, Name = "forceCollectionInterval")]
        public int ForceCollectionInterval { get; set; }

        [DataMember(Order = 4, Name = "traceStartTimeoutEnabled")]
        public bool TraceStartTimeoutEnabled { get; set; }

        [DataMember(Order = 5, Name = "traceStartTimeout")]
        public int TraceStartTimeout { get; set; }

        [DataMember(Order = 6, Name = "temperDetectionIntervalEnabled")]
        public bool TemperDetectionIntervalEnabled { get; set; }

        [DataMember(Order = 7, Name = "temperDetectionInterval")]
        public int TemperDetectionInterval { get; set; }

        [DataMember(Order = 8, Name = "traceDirectorySizeLimit")]
        public int TraceDirectorySizeLimit { get; set; }

        [DataMember(Order = 9, Name = "unattendedTimeLimit")]
        public int UnattendedTimeLimit { get; set; }
    }
}
