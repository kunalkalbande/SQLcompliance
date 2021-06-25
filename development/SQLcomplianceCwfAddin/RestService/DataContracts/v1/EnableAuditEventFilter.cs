using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [Serializable]
    [DataContract(Name = "EnableAuditEventFilter")]
    public class EnableAuditEventFilter
    {
        [DataMember(Order = 1, Name = "eventId")]
        public int EventId { get; set; }

        [DataMember(Order = 2, Name = "isEnable")]
        public bool IsEnabled { get; set; }
    }
}

