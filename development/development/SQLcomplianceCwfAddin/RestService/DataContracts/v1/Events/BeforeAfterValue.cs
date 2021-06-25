using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.Events
{
    [DataContract(Name = "beforeAfterValue")]
    public class BeforeAfterValue
    {
        [DataMember(Name = "rowNumber" , Order = 0)]
        public int RowNumber { get; set; }

        [DataMember(Name = "primaryKey", Order = 1)]
        public string PrimaryKey { get; set; }

        [DataMember(Name = "column", Order = 2)]
        public string Column { get; set; }

        [DataMember(Name = "beforeValue", Order = 3)]
        public string BeforeValue { get; set; }

        [DataMember(Name = "afterValue", Order = 4)]
        public string AfterValue { get; set; }
    }
}
