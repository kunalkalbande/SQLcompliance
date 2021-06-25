using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Installer_form_application
{
    public partial class InstallScreen : Form
    {
        public InstallScreen(Form backScreen)
        {
            InitializeComponent();
            MinimizeBox = false;
            MaximizeBox = false;
        }

        private void buttonFinish_Click(object sender, EventArgs e)
        {
            if (checkBoxLaunchApp.Checked)
            {
                string host = "localhost";
                if (!properties.localInstall)
                {
                    host = properties.RemoteHostname;
                }
                try
                {
                    System.Diagnostics.Process.Start(properties.SPPath + @"\SQLcompliance.exe");
                }
                catch(Exception ex)
                {
                }
            }
            Application.Exit();
        }

        private void InstallScreen_Load(object sender, EventArgs e)
        {
            string url;
            if (properties.localInstall)
            {
                url = "https://localhost:" + properties.WebAppSSLPort;
            }
            else
            {
                url = "https://" + properties.RemoteHostname + properties.WebAppSSLPort;
            }
            labelInstruction.Text += "\n\u2022 Go to Start Menu, Select Idera -> Idera Dashbaord \n\n";
            dashboardLink.Text = "\u2022 Open the url " + url + " from your browser. Be sure to \n save it as a bookmark favorite.";
            dashboardLink.LinkArea = new LinkArea(15, url.Length);
        }

        private void dashboardLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string host = "localhost";
            if (!properties.localInstall)
            {
                host = properties.RemoteHostname;
            }
            System.Diagnostics.Process.Start("https://" + host + ":" + properties.WebAppSSLPort);
        }
    }
}
