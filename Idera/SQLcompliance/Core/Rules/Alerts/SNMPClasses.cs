using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Xml.Serialization;
using SnmpSharpNet;

namespace Idera.SQLcompliance.Core.Rules.Alerts
{
    public enum SNMPTrap
    {
        SqlCmTrap = 1,
        SqlCmTrapTest = 99
    }

    internal static class SNMPHelper
    {
        internal const string IderaOid = "1.3.6.1.4.1.24117";
        internal const byte ProductLine = 1; // SQL Servers
        internal const byte Product = 6; // SQL Compliance Manager

        private const int GenericTrap = 6;

        internal enum SqlCmVariable : byte
        {
            AlertType = 1,
            Instance,
            Created,
            Level,
            EventType,
            Message,
            RuleName,
            MessageTitle,
            Severity,
            ComputerName
        }

        internal static VbCollection CreateVariableBindings(List<KeyValuePair<SqlCmVariable, string>> variableBindings)
        {
            if (variableBindings == null || variableBindings.Count == 0)
                throw new ArgumentNullException("variableBindings");

            VbCollection variableBindingsCollection = new VbCollection();
            for (int index = 0; index < variableBindings.Count; index += 1)
            {
                // variable binding
                KeyValuePair<SqlCmVariable, string> variable = variableBindings[index];

                Oid oid = new Oid();
                oid.Set(FormatBindingOid(variable.Key));
                string message = string.IsNullOrEmpty(variable.Value) ? ReplaceNonPrintableCharacters("N/A") : ReplaceNonPrintableCharacters(variable.Value);

                variableBindingsCollection.Add(oid, new OctetString(message));
            }

            return variableBindingsCollection;
        }

        internal static void SendSnmpTrap(VbCollection variableBindings, SNMPConfiguration configuration, bool isTestTrap)
        {
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            if (variableBindings == null)
            {    if (isTestTrap)
                {
                    // create test SNMP trap
                    variableBindings = new VbCollection();

                    Oid oid = new Oid();
                    oid.Set(FormatBindingOid(SqlCmVariable.MessageTitle));

                    variableBindings.Add(oid, new OctetString(ReplaceNonPrintableCharacters("SQLcm SNMP Trap Test")));
                }
                else
                    throw new ArgumentNullException("variableBindings");
            }

            TrapAgent agent = new TrapAgent();
            agent.SendV1Trap(configuration.ReceiverIpAddress,
                             configuration.ReceiverPort,
                             configuration.Community,
                             configuration.SenderSysObjectId,
                             configuration.SenderAddress,
                             GenericTrap,
                             (int) (isTestTrap ? SNMPTrap.SqlCmTrapTest : SNMPTrap.SqlCmTrap),
                             0,
                             variableBindings);
        }

        internal static string FormatBindingOid(SqlCmVariable variable)
        {
            return string.Format("{0}.{1}.{2}.{3}", IderaOid, ProductLine, Product, (byte) variable);
        }

        private static string ReplaceNonPrintableCharacters(string text)
        {
            StringBuilder printableText = new StringBuilder();
            const char replaceWith = '?';

            // if character is not a printable character replace it with a '?'
            foreach (char character in text)
                printableText.Append(((character > 31 && character < 127) || 
                                      (character == 10) || 
                                      (character == 13)) ? 
                                       character : replaceWith);

            return printableText.ToString();
        }
    }

    /// <summary>
    /// This class holds configurations to send SNMP v1 trap messages.
    /// </summary>
    [Serializable]
    [XmlRoot("SnmpConfiguration")]
    public class SNMPConfiguration
    {
        #region fields

        private string _receiverAddress;
        private IpAddress _receiverIpAddress;
        private int _receiverPort;
        private string _community;

        private readonly Oid _senderSysObjectId;
        private readonly IpAddress _senderAddress;

        #endregion

        public SNMPConfiguration()
        {
            _receiverPort = 162;
            _community = string.Empty;
            _senderSysObjectId = new Oid(SNMPHelper.IderaOid);
            _senderAddress = new IpAddress(Dns.GetHostName());
        }

        #region properties

        [XmlAttribute("ReceiverAddress")]
        public string ReceiverAddress
        {
            get { return _receiverAddress; }
            set
            {
                _receiverAddress = value;
                if (!string.IsNullOrEmpty(_receiverAddress))
                    _receiverIpAddress = new IpAddress(_receiverAddress);
            }
        }

        [XmlIgnore]
        public IpAddress ReceiverIpAddress
        {
            get{ return _receiverIpAddress; }
        }

        [XmlAttribute("ReceiverPort")]
        public int ReceiverPort
        {
            get { return _receiverPort; }
            set { _receiverPort = value; }
        }

        [XmlAttribute("Community")]
        public string Community
        {
            get { return _community; }
            set { _community = value; }
        }

        [XmlIgnore]
        public Oid SenderSysObjectId
        {
            get { return _senderSysObjectId; }
        }

        [XmlIgnore]
        public IpAddress SenderAddress
        {
            get { return _senderAddress; }
        }

        #endregion

        #region public methods

        public SNMPConfiguration Clone()
        {
            SNMPConfiguration config = new SNMPConfiguration();
            config.ReceiverAddress = _receiverAddress;
            config.ReceiverPort = _receiverPort;
            config.Community = _community;

            return config;
        }

        public override int GetHashCode()
        {
            return _senderAddress.GetHashCode();
        }

        #endregion
    }
}
