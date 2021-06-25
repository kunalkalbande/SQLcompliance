using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [Serializable]
    [DataContract]
    public class BeforeAfterDataEventResponse
    {
        public BeforeAfterDataEventResponse()
        {
            Tables = new List<KeyValuePair<int, string>>();
            Columns = new List<KeyValuePair<int, string>>();
            EventType = new List<EventFilterListData>();
        }

        [DataMember]
        public List<KeyValuePair<int, string>> Tables { get; set; }

        [DataMember]
        public List<KeyValuePair<int, string>> Columns { get; set; }

        [DataMember(Name = "recordCount")]
        public int RecordCount { get; set; }

        [DataMember]
        public List<EventFilterListData> EventType { get; set; }
    }
}