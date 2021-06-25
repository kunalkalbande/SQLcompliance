using System.Collections.Generic;
using System.Runtime.Serialization;
namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [DataContract]
    public class FilteredChangeLogsViewResponce
    {
        public FilteredChangeLogsViewResponce()
        {
            Alerts = new List<ServerChangeLogs>();
                EventType = new List<EventFilterListData>();
        }

        [DataMember]
        public List<ServerChangeLogs> Alerts { get; set; }

        [DataMember]
        public List<EventFilterListData> EventType { get; set; }

        internal void Add(ServerChangeLogs alert)
        {
            Alerts.Add(alert);
        }

        [DataMember(Name = "recordCount")]
        public int RecordCount { get; set; }
    }
}
