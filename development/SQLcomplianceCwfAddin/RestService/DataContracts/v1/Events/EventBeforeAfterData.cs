using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.Events
{
    [DataContract(Name = "eventBeforeAfterData")]
    public class EventBeforeAfterData
    {
        [DataMember(Name = "isAvailable", Order = 0)]
        public bool IsAvailable { get; set; }

        [DataMember(Name = "statusMessage", Order = 1)]
        public string StatusMessage { get; set; }

        [DataMember(Name = "columnsAffectedStatusMessage", Order = 2)]
        public string ColumnsAffectedStatusMessage { get; set; }

        [DataMember(Name = "rowsAffectedStatusMessage", Order = 3)]
        public string RowsAffectedStatusMessage { get; set; }

        [DataMember(Name = "beforeAfterValueList", Order = 4)]
        public List<BeforeAfterValue> BeforeAfterValueList { get; set; }
    }
}
