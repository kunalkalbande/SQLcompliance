using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AuditReports
{
    [DataContract]
    public class RowCountResponse
    {
        public RowCountResponse()
        {
            AuditRowCount = new List<RowCountData>();
        }

        [DataMember]
        public List<RowCountData> AuditRowCount { get; set; }

        internal void Add(RowCountData rowCountData)
        {
            AuditRowCount.Add(rowCountData);
        }
    }
}
