using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace CwfAddinInstaller.WizardPages
{
    [WizardPageInfo("Now we are ready to install IDERA Dashboard", "The IDERA Dashboard hosts the SQL Compliance Manager Web console and provides an integrated experience across the IDERA SQL suite of products.  Choose to install or register with a local or remote IDERA Dashboard.", WizardPage.DashboardLocation)]
    internal partial class PageDashboardLocation : WizardPageBase
    {
        #region members

        private const int DefaultDashboardPort = 9292;

        private int _localDashboardPort;
        private string _localUserName;
        private string _localPassword;

        private string _remoteDashboardHost;
        private int _remoteDashboardPort;
        private string _remoteUserName;
        private string _remotePassword;
        public static bool installNewLocalDashboard;
        public static bool upgradeLocalDashboard;
        public static bool upgradeRemoteDashboard;
        public static bool isDashboardAvailable;
        public static string remoteDashboardMachineName;

        #endregion

        public PageDashboardLocation(FormInstaller host)
            : base(host)
        {
            _localDashboardPort = DefaultDashboardPort;
            _remoteDashboardPort = DefaultDashboardPort;

            InitializeComponent();

            updnPort.Value = DefaultDashboardPort;
            txtHost.Clear();
            radDashboard_CheckedChanged(this, EventArgs.Empty);
        }

        internal override void Initialize()
        {
            Host.lblPageTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.00F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            Host.lblPageDescription.BringToFront();
            Host.Log("{0} - Initialized.", WizardPage.DashboardLocation);
            IsInitialized = true;
        }

        internal override bool DoAction(out string errorMessage, out Control invalidControl)
        {
            invalidControl = null;
            if (!Host.IsInputValid(txtHost))
            {
                errorMessage = "IDERA dashboard host name not entered.";
                invalidControl = txtHost;
                return false;
            }

            if (!Host.IsInputValid(txtCwfUser))
            {
                errorMessage = "IDERA dashboard administrator user name not entered.";
                invalidControl = txtCwfUser;
                return false;
            }

            if (!Host.IsInputValid(txtCwfPassword))
            {
                errorMessage = "IDERA dashboard administrator password not entered.";
                invalidControl = txtCwfPassword;
                return false;
            }

            Host.CwfHelper.CwfUrl = string.Format("http://{0}:{1}", txtHost.Text, updnPort.Value);
            Host.CwfHelper.CwfUser = txtCwfUser.Text.Trim();
            Host.CwfHelper.CwfPassword = txtCwfPassword.Text;

            bool coreUpgradeNeaded = false;

            isDashboardAvailable = Host.CwfHelper.CanConnectToDashboard();

            if (isDashboardAvailable)
            {
                Version installedDashboardVersion = Host.CwfHelper.GetCurrentVersion(out errorMessage);

                if (installedDashboardVersion == null &&
                    !string.IsNullOrEmpty(errorMessage))
                {
                    errorMessage = string.Format("Couldn't get current version of dashboard due to the following error: {0}", errorMessage);
                    return false;
                }

                coreUpgradeNeaded = Host.CwfHelper.CoreUpgradeNeaded(installedDashboardVersion);
            }

            installNewLocalDashboard = radDashboardLocal.Checked && !isDashboardAvailable;
            upgradeLocalDashboard = radDashboardLocal.Checked && isDashboardAvailable && coreUpgradeNeaded;
            upgradeRemoteDashboard = radDashboardRemote.Checked && isDashboardAvailable && coreUpgradeNeaded;

            remoteDashboardMachineName = txtHost.Text;

            if (PageDashboardLocation.installNewLocalDashboard || PageDashboardLocation.upgradeLocalDashboard)
            {
                errorMessage = string.Empty;
                return true;
            }
            else if (PageDashboardLocation.upgradeRemoteDashboard)
            {
                errorMessage = string.Format("This product requires newer version of the Dashboard. You have to manually upgrade the Dashboard on remote machine: {0} to continue with product installation.", PageDashboardLocation.remoteDashboardMachineName);
                MessageBox.Show(this, errorMessage, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            Host.CwfHelper.IsDashboardOnRemoteHost = !radDashboardLocal.Checked;

            errorMessage = "None";
            return true;
        }

        private void radDashboard_CheckedChanged(object sender, EventArgs e)
        {
            SaveSettings();
            LoadSettings();
        }

        private void SaveSettings()
        {
            if (radDashboardRemote.Checked)
            {
                _localDashboardPort = (int)updnPort.Value;
                _localUserName = txtCwfUser.Text;
                _localPassword = txtCwfPassword.Text;
            }
            else
            {
                _remoteDashboardHost = txtHost.Text;
                _remoteDashboardPort = (int)updnPort.Value;
                _remoteUserName = txtCwfUser.Text;
                _remotePassword = txtCwfPassword.Text;
            }
        }

        private void LoadSettings()
        {
            if (radDashboardLocal.Checked)
            {
                txtHost.Text = Dns.GetHostName();
                updnPort.Value = _localDashboardPort;
                txtCwfUser.Text = _localUserName;
                txtCwfPassword.Text = _localPassword;

                txtHost.Enabled = false;
                updnPort.Focus();
            }
            else
            {
                txtHost.Text = _remoteDashboardHost;
                updnPort.Value = _remoteDashboardPort;
                txtCwfUser.Text = _remoteUserName;
                txtCwfPassword.Text = _remotePassword;

                txtHost.Enabled = true;
                txtHost.Focus();
            }
        }
    }
}
