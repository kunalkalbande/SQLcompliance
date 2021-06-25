using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.AuditReports;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [DataContract]
    public class DMLActivityResponse
    {
        public DMLActivityResponse()
        {
            AuditDML = new List<DMLActivityData>();
        }

        [DataMember]
        public List<DMLActivityData> AuditDML { get; set; }

        internal void Add(DMLActivityData dmlActivityData)
        {
            AuditDML.Add(dmlActivityData);
        }
    }
}
