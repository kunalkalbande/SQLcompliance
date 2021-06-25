using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.Archives
{
    [DataContract]
    public class ArchiveRecord
    {
        [DataMember(Order = 0, Name = "instance")]
        public string Instance { get; set; }

        [DataMember(Order = 1, Name = "displayName")]
        public string DisplayName { get; set; }

        [DataMember(Order = 2, Name = "description")]
        public string Description { get; set; }

        [DataMember(Order = 3, Name = "databaseName")]
        public string DatabaseName { get; set; }
    }
}
