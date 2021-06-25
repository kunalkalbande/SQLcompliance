using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SQLcomplianceCwfAddin.Helpers
{

    [Serializable]
    [DataContract(Name = "SNMPConfigData")]
    public class SNMPConfigData
    {
        [DataMember(Order = 1, Name = "sender_email")]
        public string SenderEmail { get; set; }

        [DataMember(Order = 2, Name = "logs_permission")]
        public bool LogsPermission { get; set; }

        [DataMember(Order = 3, Name = "send_mail_permission")]
        public bool SendMailPermission { get; set; }

        [DataMember(Order = 4, Name = "snmp_permission")]
        public bool SnmpPermission { get; set; }

        [DataMember(Order = 5, Name = "snmpCommunity")]
        public string SnmpCommunity { get; set; }

        [DataMember(Order = 6, Name = "snmpPort")]
        public int SnmpPort { get; set; }

        [DataMember(Order = 7, Name = "snmpServerAddress")]
        public string SnmpServerAddress { get; set; }

        [DataMember(Order = 8, Name = "severity")]
        public int Severity { get; set; }

        [DataMember(Order = 9, Name = "messageData")]
        public string MessageData { get; set; }
    }
}
