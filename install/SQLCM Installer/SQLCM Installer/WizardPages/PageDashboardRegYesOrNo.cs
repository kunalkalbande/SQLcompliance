using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQLCM_Installer.WizardPages
{
    [WizardPageInfo(WizardPage.DashboardDetection)]
    internal partial class PageDashboardRegYesOrNo : WizardPageBase
    {
        public PageDashboardRegYesOrNo(MainForm host)
            : base(host)
        {
            InitializeComponent();
        }

        internal override void OnNavigated(NavigationDirection direction)
        {

        }

        internal override bool DoAction(out string errorMessage, out Control invalidControl)
        {
            invalidControl = null;
            errorMessage = string.Empty;

            if (radioYes.Checked)
            {
                if (string.IsNullOrEmpty(textRemoteDashboardLocation.Text))
                {
                    errorMessage = Constants.DashboardURLEmpty;
                    return false;
                }
                if (string.IsNullOrEmpty(textRemoteDashboardAdminUserName.Text))
                {
                    errorMessage = Constants.DashboardUserInvalid;
                    return false;
                }
                if (string.IsNullOrEmpty(textRemoteDashboardAdminPassword.Text))
                {
                    errorMessage = Constants.DashboardPasswordInvalid;
                    return false;
                }

                Uri inputURL;
                if (!Uri.TryCreate(textRemoteDashboardLocation.Text, UriKind.RelativeOrAbsolute, out inputURL))
                {
                    errorMessage = Constants.DashboardUriInvalid;
                    return false;
                }
                try
                {
                Host.CwfHelper.CwfUrl = string.Format("http://{0}:{1}", inputURL.Host, inputURL.Port);
                }
                catch
                {
                    errorMessage = Constants.DashboardUriInvalid;
                    return false;
                }
                Host.CwfHelper.CwfUser = textRemoteDashboardAdminUserName.Text;
                Host.CwfHelper.CwfPassword = textRemoteDashboardAdminPassword.Text;

                bool isDashboardAvailable = Host.CwfHelper.CanConnectToDashboard();

                if (isDashboardAvailable)
                {
                    errorMessage = string.Empty;
                    Version installedDashboardVersion = Host.CwfHelper.GetCurrentVersion(out errorMessage);

                    if (installedDashboardVersion == null ||
                        !string.IsNullOrEmpty(errorMessage))
                    {
                        if (string.IsNullOrEmpty(errorMessage))
                        {
                            errorMessage = "Could not determine current version of IDERA Dashboard using provided details. Please check if the inputs are correct and the IDERA Dashboard services are running properly on given machine.";
                        }
                        else
                        {
                            errorMessage = string.Format("Couldn't get current version of IDERA Dashboard due to the following error: {0}", errorMessage);
                        }
                        return false;
                    }

                    if (installedDashboardVersion < Constants.DashboardVersion)
                    {
                        errorMessage = string.Format(Constants.DashboardVersionError, inputURL.Host);
                        return false;
                    }
                }
                else
                {
                    Host.CwfHelper.CwfUser = "";
                    Host.CwfHelper.CwfPassword = "";
                    errorMessage = Constants.InvalidRemoteDashboardValues;
                    return false;
                }

                InstallProperties.RegisterProductToDashboard = true;
            }
            else
            {
                InstallProperties.RegisterProductToDashboard = false;
            }

            errorMessage = null;
            invalidControl = null;
            return true;
        }

        private void radioYes_Click(object sender, EventArgs e)
        {
            if (radioYes.Checked)
            {
                panelDashboardLocation.Show();
                radioLocalDashboard.Checked = true;
                panelRemoteDashboardLocation.Show();

            }
        }

        private void radioNo_Click(object sender, EventArgs e)
        {
            if (radioNo.Checked)
            {
                panelDashboardLocation.Hide();
                panelRemoteDashboardLocation.Hide();
            }
        }

        private void radioLocalDashboard_Click(object sender, EventArgs e)
        {
            if (radioLocalDashboard.Checked)
            {
                labelRemoteDashboardLocation.Text = "Local " + Constants.ProductMap[Products.Dashboard] + " Location";
                textRemoteDashboardLocation.Text = string.Format("{0}://{1}:{2}/", Constants.DefaultDashboardProtocol, Environment.MachineName, Constants.DefaultDashboardPort);
                textRemoteDashboardLocation.Enabled = false;
            }
        }

        private void radioRemoteDashboard_Click(object sender, EventArgs e)
        {
            if (radioRemoteDashboard.Checked)
            {
                labelRemoteDashboardLocation.Text = "Remote " + Constants.ProductMap[Products.Dashboard] + " Location";
                textRemoteDashboardLocation.Text = "";
                textRemoteDashboardLocation.Enabled = true;
            }
        }
    }
}
