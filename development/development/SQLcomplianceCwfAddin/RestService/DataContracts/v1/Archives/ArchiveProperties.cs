using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.Archives
{
    [Serializable]
    public enum DatabaseIntegrityStatus
    {
        BadEvents = 0, Ok = 1
    }

    [DataContract]
    public class ArchiveProperties
    {
        [DataMember(Order = 0, Name = "instance")]
        public string Instance { get; set; }

        [DataMember(Order = 1, Name = "displayName")]
        public string DisplayName { get; set; }

        [DataMember(Order = 2, Name = "description")]
        public string Description { get; set; }

        [DataMember(Order = 3, Name = "databaseName")]
        public string DatabaseName { get; set; }

        [DataMember(Order = 4, Name = "eventTimeSpanFrom")]
        public DateTime? EventTimeSpanFrom { get; set; }

        [DataMember(Order = 5, Name = "eventTimeSpanTo")]
        public DateTime? EventTimeSpanTo { get; set; }

        [DataMember(Order = 6, Name = "databaseIntegrity")]
        public DatabaseIntegrityStatus DatabaseIntegrity { get; set; }

        [DataMember(Order = 7, Name = "lastIntegrityCheck")]
        public DateTime? LastIntegrityCheck { get; set; }

        [DataMember(Order = 8, Name = "lastIntegrityCheckResult")]
        public int LastIntegrityCheckResult { get; set; }

        [DataMember(Order = 9, Name = "defaultAccess")]
        public int DefaultAccess { get; set; }

        [DataMember(Order = 10, Name = "isCompatibleSchema")]
        public bool IsCompatibleSchema { get; set; }

        [DataMember(Order = 11, Name = "isValidArchive")]
        public bool IsValidArchive { get; set; }

        public int Schema { get; set; }
    }
}
