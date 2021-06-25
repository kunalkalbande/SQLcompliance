using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CWFInstallerService;

namespace Installer_form_application
{
    public partial class PortForm : Form
    {
        Form screenObject;
        public PortForm(Form screenObj)
        {
            InitializeComponent();
            MinimizeBox = false;
            MaximizeBox = false;
            screenObject = screenObj;
        }

       
        private void buttonBack_Click(object sender, EventArgs e)
        {
            properties.CoreServicesPort = textBoxCoreServicesPort.Text;
            properties.WebAppMonitorPort = textBoxWebAppMonitorPort.Text;
            properties.WebAppServicePort = textBoxWebAppServicePort.Text;
            properties.WebAppSSLPort = textBoxWebAppSSLPort.Text;
            this.Hide();
            screenObject.Show();
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            try
            {
                List<int> ports = new List<int>();
                try
                {
                    ports.Add(Int32.Parse(textBoxCoreServicesPort.Text));
                    ports.Add(Int32.Parse(textBoxWebAppMonitorPort.Text));
                    ports.Add(Int32.Parse(textBoxWebAppServicePort.Text));
                    ports.Add(Int32.Parse(textBoxWebAppSSLPort.Text));
                }
                catch
                {
                    MessageBox.Show("You have entered an invalid port. Please enter a port in the range of 1 to 65535.", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Show();
                    return;
                }
                if (properties.localInstall)
                {
                    Validator.ValidatePorts(ports);
                }
                else
                {
                    Validator.ValidateRemotePort(properties.RemoteHostname, ports);
                }
                properties.CoreServicesPort = textBoxCoreServicesPort.Text;
                properties.WebAppMonitorPort = textBoxWebAppMonitorPort.Text;
                properties.WebAppServicePort = textBoxWebAppServicePort.Text;
                properties.WebAppSSLPort = textBoxWebAppSSLPort.Text;
                this.Hide();
                RepositoryDetails nextScreen = new RepositoryDetails(this);
                nextScreen.Show();
            }
            catch (CWFBaseException ex)
            {
                MessageBox.Show(ex.ErrorCode + " - " + ex.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            

        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("Do you really want to exit?", "Exit", MessageBoxButtons.YesNo))
            {
                Application.Exit();
            }
        }

        private void PortForm_Load(object sender, EventArgs e)
        {
            textBoxCoreServicesPort.Text = properties.CoreServicesPort;
            textBoxWebAppMonitorPort.Text = properties.WebAppMonitorPort;
            textBoxWebAppServicePort.Text = properties.WebAppServicePort;
            textBoxWebAppSSLPort.Text = properties.WebAppSSLPort;
        }
    }
}
