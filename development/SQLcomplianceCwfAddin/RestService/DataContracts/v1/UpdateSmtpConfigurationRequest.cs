using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [Serializable]
    [DataContract(Name = "UpdateSmtpConfigurationRequest")]
    public class UpdateSmtpConfigurationRequest
    {
        [DataMember(Order = 1, Name = "smtpServer")]
        public string SmtpServer { get; set; }

        [DataMember(Order = 2, Name = "smtpPort")]
        public int SmtpPort { get; set; }

        [DataMember(Order = 3, Name = "smtpAuthType")]
        public int SmtpAuthType { get; set; }

        [DataMember(Order = 4, Name = "smtpSsl")]
        public string SmtpSsl { get; set; }

        [DataMember(Order = 5, Name = "smtpUsername")]
        public string SmtpUsername { get; set; }

        [DataMember(Order = 6, Name = "smtpPassword")]
        public string SmtpPassword { get; set; }

        [DataMember(Order = 7, Name = "smtpSenderAddress")]
        public string SmtpSenderAddress { get; set; }

        [DataMember(Order = 8, Name = "smtpSenderName")]
        public string SmtpSenderName { get; set; }
    }
}

    