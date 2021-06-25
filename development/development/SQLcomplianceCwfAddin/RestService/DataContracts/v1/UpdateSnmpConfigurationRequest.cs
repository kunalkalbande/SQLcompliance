using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [Serializable]
    [DataContract(Name = "UpdateSnmpConfiguration")]
    public class UpdateSnmpConfigurationRequest
    {
        [DataMember(Order = 1, Name = "snmpServerAddress")]
        public string SnmpServerAddress { get; set; }

        [DataMember(Order = 2, Name = "port")]
        public int Port { get; set; }

        [DataMember(Order = 3, Name = "community")]
        public string Community { get; set; }
    }

    [Serializable]
    [DataContract(Name = "UpdateSNMPThresholdConfiguration")]
    public class UpdateSNMPThresholdConfiguration
    {
        [DataMember(Order = 1, Name = "senderEmail")]
        public string SenderEmail { get; set; }

        [DataMember(Order = 2, Name = "instanceName")]
        public string InstanceName { get; set; }

        [DataMember(Order = 3, Name = "snmpPermission")]
        public Boolean SnmpPermission { get; set; }

        [DataMember(Order = 4, Name = "logsPermission")]
        public Boolean LogsPermission { get; set; }

        [DataMember(Order = 5, Name = "sendMailPermission")]
        public Boolean SendMailPermission { get; set; }

        [DataMember(Order = 6, Name = "snmpServerAddress")]
        public String SnmpServerAddress { get; set; }

        [DataMember(Order = 7, Name = "port")]
        public int Port { get; set; }

        [DataMember(Order = 8, Name = "community")]
        public String Community { get; set; }

        [DataMember(Order = 9, Name = "severity")]
        public int Severity { get; set; }

        [DataMember(Order = 10, Name = "srvId")]
        public long SrvId { get; set; }

        [DataMember(Order = 11, Name = "messageData")]
        public string MessageData { get; set; }
    }


    //SQLCM_125_ _ Rohit_ Start
    [Serializable]
    [DataContract(Name = "GetSNMPThresholdConfiguration")]
    public class GetSNMPThresholdConfiguration
    {
        [DataMember(Order = 1, Name = "instanceName")]
        public string InstanceName { get; set; }
    }
    //SQLCM_125_ _ Rohit_End

    [Serializable]
    [DataContract(Name = "SNMPConfigurationData")]
    public class SNMPConfigurationData
    {
        [DataMember(Order = 1, Name = "snmpAddress")]
        public string SnmpAddress { get; set; }

        [DataMember(Order = 2, Name = "port")]
        public int Port { get; set; }

        [DataMember(Order = 3, Name = "community")]
        public string Community { get; set; }
    }
}
