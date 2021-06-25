using System.Windows.Forms;
using Idera.SQLcompliance.Application.GUI.Properties;
using Idera.SQLcompliance.Core.Rules.Alerts;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
    public partial class Form_AlertingOptions_Snmp : Form
    {
        private SNMPConfiguration _snmpConfiguration;

        public Form_AlertingOptions_Snmp(SNMPConfiguration configuration)
        {
            _snmpConfiguration = configuration;

            InitializeComponent();
            Icon = Resources.SQLcompliance_product_ico;

            if (_snmpConfiguration != null)
            {
                txtAddress.Text = _snmpConfiguration.ReceiverAddress;
                updnPort.Value = _snmpConfiguration.ReceiverPort;
                txtCommunity.Text = _snmpConfiguration.Community;
            }
        }

        public SNMPConfiguration SnmpConfiguration
        {
            get { return _snmpConfiguration; } 
        }

        private void UpdateSnmpConfiguraton()
        {
            if (_snmpConfiguration == null)
                _snmpConfiguration = new SNMPConfiguration();

            _snmpConfiguration.ReceiverAddress = txtAddress.Text;
            _snmpConfiguration.ReceiverPort = (int)updnPort.Value;
            _snmpConfiguration.Community = txtCommunity.Text;
        }

        private void Form_AlertingOptions_Snmp_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult != DialogResult.OK)
                return;

            UpdateSnmpConfiguraton();
        }

        private void btnTest_Click(object sender, System.EventArgs e)
        {
            UpdateSnmpConfiguraton();

            string errorString;
            string title = Text;

            Cursor = Cursors.WaitCursor;
            Text = @"Performing SNMP Test...";
            btnTest.Enabled = false;

            System.Windows.Forms.Application.DoEvents();

            if (ActionProcessor.PerformSnmpTrapTest(_snmpConfiguration, out errorString))
                MessageBox.Show(this, string.Format("The SNMP test was successful. SNMP Trap message sent to {0} on port {1}.", _snmpConfiguration.ReceiverAddress, _snmpConfiguration.ReceiverPort), @"Test Successful");
            else
                MessageBox.Show(this, string.Format("The SNMP test failed for the following reason: {0}", errorString), @"Test Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);

            Cursor = Cursors.Default;
            Text = title;
            btnTest.Enabled = true;
        }
    }
}