using SQLcomplianceCwfAddin.RestService.DataContracts.v1.AuditReports;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [DataContract]
    public class ApplicationActivityResponse
    {
        public ApplicationActivityResponse()
        {
           AuditApplication  = new List<ApplicationActivityData>();
        }

        [DataMember]
        public List<ApplicationActivityData> AuditApplication { get; set; }

        internal void Add(ApplicationActivityData applicationActivityData)
        {
            AuditApplication.Add(applicationActivityData);
        }
    }
}
