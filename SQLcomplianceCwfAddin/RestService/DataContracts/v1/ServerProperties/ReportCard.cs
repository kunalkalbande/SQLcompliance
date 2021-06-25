using SQLcomplianceCwfAddin.RestService.DataContracts.v1.Stats;
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.ServerProperties
{
    [DataContract(Name = "reportCard")]
    public class ReportCard
    {
        [DataMember(Order = 0, Name = "serverId")]
        public int ServerId { get; set; }

        [DataMember(Order = 1, Name = "statisticCategory")]
        public RestStatsCategory StatisticCategory { get; set; }

        [DataMember(Order = 2, Name = "warningThreshold")]
        public int WarningThreshold { get; set; }

        [DataMember(Order = 3, Name = "criticalThreshold")]
        public int CriticalThreshold { get; set; }

        [DataMember(Order = 4, Name = "period")]
        public int Period { get; set; }

        [DataMember(Order = 5, Name = "enabled")]
        public bool Enabled { get; set; }
    }
}
