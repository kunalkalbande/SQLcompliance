using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AuditReports
{
    [DataContract]
    public class LoginDeletionHistoryResponse
    {
        public LoginDeletionHistoryResponse()
        {
            AuditLoginDeletion = new List<LoginDeletionHistoryData>();
        }

        [DataMember]
        public List<LoginDeletionHistoryData> AuditLoginDeletion { get; set; }

        internal void Add(LoginDeletionHistoryData loginDeletionHistoryData)
        {
            AuditLoginDeletion.Add(loginDeletionHistoryData);
        }
    }
}
