using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AuditReports
{
    [DataContract]
    public class LoginCreationHistoryResponse
    {
        public LoginCreationHistoryResponse()
        {
            AuditLoginCreation = new List<LoginCreationHistoryData>();
        }

        [DataMember]
        public List<LoginCreationHistoryData> AuditLoginCreation { get; set; }

        internal void Add(LoginCreationHistoryData loginCreationHistoryData)
        {
            AuditLoginCreation.Add(loginCreationHistoryData);
        }
    }
}
