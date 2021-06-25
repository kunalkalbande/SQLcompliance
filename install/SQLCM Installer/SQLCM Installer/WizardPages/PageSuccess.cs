using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace SQLCM_Installer.WizardPages
{
    [WizardPageInfo(WizardPage.Success)]
    internal partial class PageSuccess : WizardPageBase
    {
        public PageSuccess(MainForm host)
            : base(host)
        {
            InitializeComponent();
        }

        internal override void OnNavigated(NavigationDirection direction)
        {
            if (Constants.UserInstallType == InstallType.CMAndDashboard ||
                Constants.UserInstallType == InstallType.CMOnly ||
                Constants.UserInstallType == InstallType.ConsoleAndDashboard ||
                Constants.UserInstallType == InstallType.ConsoleOnly)
            {
                checkBoxLaunchManagementConsole.Visible = true;
                checkBoxLaunchManagementConsole.Checked = true;
            }
            else
            {
                checkBoxLaunchManagementConsole.Visible = false;
            }

            if (Constants.UserInstallType == InstallType.AgentAndDashboard)
            {
                labelHeader.Text = "IDERA " + Constants.ProductMap[Products.Agent] + " and " + Constants.ProductMap[Products.Dashboard] + " installation has\r\ncompleted successfully!";
                labelSQLCMHeader.Text = Constants.ProductMap[Products.Agent];
                labelSQLCMDesc.Visible = false;

            }

            else if (Constants.UserInstallType == InstallType.CMOnly)
            {
                labelHeader.Text = "IDERA " + Constants.ProductMap[Products.Compliance] + " installation has completed successfully!";
                labelSQLCMHeader.Text = Constants.ProductMap[Products.Compliance];

                panelIderaDashboard.Visible = false;
                panelFirewallConfiguration.Location = new Point(0, 163);
            }

            else if (Constants.UserInstallType == InstallType.ConsoleOnly)
            {
                labelHeader.Text = "IDERA " + Constants.ProductMap[Products.Console] + " installation has completed\r\nsuccessfully!";
                labelSQLCMHeader.Text = Constants.ProductMap[Products.Console];

                panelIderaDashboard.Visible = false;
                panelFirewallConfiguration.Visible = false;
            }

            else if (Constants.UserInstallType == InstallType.AgentOnly)
            {
                labelHeader.Text = "IDERA " + Constants.ProductMap[Products.Agent] + " installation has completed successfully!";
                labelSQLCMHeader.Text = Constants.ProductMap[Products.Agent];

                panelIderaDashboard.Visible = false;
                panelFirewallConfiguration.Location = new Point(0, 163);
                labelSQLCMDesc.Visible = false;
            }

            else if (Constants.UserInstallType == InstallType.DashboardOnly)
            {
                labelHeader.Text = Constants.ProductMap[Products.Dashboard] + " installation has completed successfully!";
                labelSQLCMHeader.Text = Constants.ProductMap[Products.Dashboard];
                panelSQLCompliance.Visible = false;

                panelIderaDashboard.Location = new Point(0, 87);
                panelFirewallConfiguration.Location = new Point(0, 163);
            }

            else if (Constants.UserInstallType == InstallType.CMAndDashboard)
            {
                labelHeader.Text = "IDERA " + Constants.ProductMap[Products.Compliance] + " and " + Constants.ProductMap[Products.Dashboard] + " installation has completed\r\nsuccessfully!";
                labelSQLCMHeader.Text = Constants.ProductMap[Products.Compliance];
            }

            else if (Constants.UserInstallType == InstallType.ConsoleAndDashboard)
            {
                labelHeader.Text = "IDERA " + Constants.ProductMap[Products.Console] + " and " + Constants.ProductMap[Products.Dashboard] + "\r\ninstallation has completed successfully!";
                labelSQLCMHeader.Text = Constants.ProductMap[Products.Console];
            }
        }

        internal override bool DoAction(out string errorMessage, out Control invalidControl)
        {
            invalidControl = null;
            errorMessage = string.Empty;

            if (this.checkBoxLaunchManagementConsole.Checked)
            {
                if (File.Exists(Path.Combine(InstallProperties.CMInstallDir, "SQLcompliance.exe")))
                {
                    Process.Start(Path.Combine(InstallProperties.CMInstallDir, "SQLcompliance.exe"));
                }
            }
            return true;
        }

        private void linkLabelPortDesc_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(@"SQLCM_FirewallConfigurationInformation.txt");
        }

        private void linkLabelDashboardLaunchDesc_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(@"https://localhost:9291");
        }
    }
}
