using SQLcomplianceCwfAddin.RestService.DataContracts.v1.AuditReports;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [DataContract]
    public class ObjectActivityResponse
    {
        public ObjectActivityResponse()
        {
            AuditObjectActivity = new List<ObjectActivityData>();
        }

        [DataMember]
        public List<ObjectActivityData> AuditObjectActivity { get; set; }

        internal void Add(ObjectActivityData objectActivityData)
        {
            AuditObjectActivity.Add(objectActivityData);
        }
    }
}
