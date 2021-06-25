using SQLcomplianceCwfAddin.RestService.DataContracts.v1.Stats;
using System;
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [DataContract(Name = "statsData")]
    public class RestStatsData
    {
        [DataMember(Order = 0, Name = "databaseId")]
        public int DatabaseId { get; set; }

        [DataMember(Order = 1, Name = "date")]
        public DateTime? Date { get; set; }

        [DataMember(Order = 2, Name = "lastUpdated")]
        public DateTime? LastUpdated { get; set; }

        [DataMember(Order = 3, Name = "category")]
        public RestStatsCategory Category { get; set; }

        [DataMember(Order = 4, Name = "categoryName")]
        public string CategoryName { get; set; }

        [DataMember(Order = 5, Name = "count")]
        public int Count { get; set; }

        [DataMember(Order = 6, Name = "serverId")]
        public int ServerId { get; set; }
        
		//SCM-4 Start
        [DataMember(Order = 7, Name = "criticalThreshold")]
        public int CriticalThreshold { get; set; }

        [DataMember(Order = 8, Name = "warningThreshold")]
        public int WarningThreshold { get; set; }
        //SCM-4 End
    }
}
