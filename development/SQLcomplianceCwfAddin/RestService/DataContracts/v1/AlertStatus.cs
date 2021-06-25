using System;
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [Serializable]
    [DataContract(Name = "alertStatus")]
    public class AlertStatus
    {
        [DataMember(Order = 1, Name = "total")]
        public int Total { get; set; }

        [DataMember(Order = 2, Name = "severe")]
        public int Severe { get; set; }

        [DataMember(Order = 3, Name = "high")]
        public int High { get; set; }

        [DataMember(Order = 4, Name = "medium")]
        public int Medium { get; set; }

        [DataMember(Order = 5, Name = "low")]
        public int Low { get; set; }
    }
}
