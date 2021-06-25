using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.ServerProperties
{
    [DataContract(Name = "thresholdsData")]
    public class ThresholdsData
    {
        [DataMember(Order = 0, Name = "thresholdList")]
        public List<ReportCard> ThresholdList { get; set; }
    }
}
