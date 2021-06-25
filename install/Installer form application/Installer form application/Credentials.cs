using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CWFInstallerService;
using System.Text.RegularExpressions;

namespace Installer_form_application
{
    public partial class Credentials : Form
    {
        Form screenObject;
        bool installExists = false;
        string CwfUrl;
        string CwfTokenRemote;   
        private static readonly Version MinimumVersionOfDashboard = new Version("3.0.3");

        public Credentials(Form screenObj)
        {
            InitializeComponent();
            MinimizeBox = false;
            MaximizeBox = false;
            screenObject = screenObj;
        }

        private void radioButtonLocal_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonLocal.Checked)
            {
                checkLocalInstalls();
                properties.localInstall = true;
            }
            else
            {
                toggleCredentialSection(true);
                properties.localInstall = false;
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("Do you really want to exit?", "Exit", MessageBoxButtons.YesNo))
            {
                Application.Exit();
            }
        }

        private void toggleCredentialSection(Boolean status)
        {
            labelUrlDesc.Visible = status;
            labelUrl.Visible = status;
            textBoxDashboardUrl.Visible = status;
            textBoxIDUserName.Visible = status;
            textBoxIDPassword.Visible = status;
            labelSQLBIUserName.Visible = status;
            labelSQLBIPassword.Visible = status;
            labelCredsDesc.Visible = status;
        }

        private void FetchUpdatedRemoteDetails()
        {
            properties.RemoteUsername = textBoxIDUserName.Text;
            properties.RemotePassword = textBoxIDPassword.Text;
            string hostname;
            string servicePort;
            try
            {
                string dashboardURL = textBoxDashboardUrl.Text;
                if (dashboardURL != string.Empty)
                { 
                    string[] temp = dashboardURL.Split(':');
                    hostname = temp[1].TrimStart('/');
                    servicePort = temp[2].TrimEnd('/');
                    properties.RemoteHostname = hostname;
                    properties.CoreServicesPort = servicePort;
                }
            }
            catch { }
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            
            try
            {
                if (radioButtonRemote.Checked)
                {
                    properties.localInstall = false;
                    FetchUpdatedRemoteDetails();
                    if (!IsValidDashbaordVersion())
                    {
                        return;
                    }
                    properties.installDashboard = false;
                }
                else
                {
                    if (installExists)
                    {
                        Validator.validateDashboardUrl(textBoxDashboardUrl.Text);
                        Validator.ValidateServiceCredentials(textBoxIDUserName.Text, textBoxIDPassword.Text);

                        bool isAdmin = SampleProductInstallationHelper.checkIfDashboardAdministrator(textBoxDashboardUrl.Text, textBoxIDUserName.Text, textBoxIDPassword.Text);

                        if (!isAdmin)
                        {
                            throw new InvalidCredentialsException();
                        }

                        properties.dashboardUrl = textBoxDashboardUrl.Text;
                        properties.serviceUsername = textBoxIDUserName.Text;
                        properties.servicePassword = textBoxIDPassword.Text;

                        properties.upgrading = installExists;
                    }
                }

                this.Hide();
                Description nextScreen = new Description(this);
                nextScreen.Show();
            }
            catch (CWFBaseException ex)
            {
                MessageBox.Show(ex.ErrorCode + " - " + ex.ErrorMessage, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private bool IsValidDashbaordVersion()
        {
            string errorMessage;
            SetData(properties.RemoteUsername, properties.RemotePassword, properties.RemoteHostname);
            try
            {
                Version installedDashboardVersion = GetCurrentVersion(out errorMessage);
                if (installedDashboardVersion < MinimumVersionOfDashboard)
                {
                    MessageBox.Show(string.Format("This product requires newer version of the Dashboard. You have to manually upgrade the Dashboard on remote machine: {0} to continue with product installation.", properties.RemoteHostname), "Idera SQL Compliance Manager Setup");
                    Application.Exit();
                    return false;
                }
                else
                {
                    installExists = true;
                    return true;
                }
            }
            catch
            {
                MessageBox.Show("Cannot connect to Remote Dashboard", "Idera SQL Compliance Manager Setup");
                return false;
            }
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            if (radioButtonLocal.Checked)
            {
                properties.localInstall = true;
            }
            else
            {
                properties.localInstall = false;
            }
            properties.dashboardUrl = textBoxDashboardUrl.Text;
            properties.serviceUsername = textBoxIDUserName.Text;
            properties.servicePassword = textBoxIDPassword.Text;
            this.Hide();
            screenObject.Show();
        }

        private void radioButtonRemote_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonRemote.Checked)
            {
                properties.localInstall = false;
                RemoteCredentials credDialog = new RemoteCredentials(this);
                credDialog.ShowDialog();
            }
        }

        public void SetData(string username, string password, string hostname)
        {
            CwfUrl = string.Format("http://{0}:{1}", properties.RemoteHostname, properties.CoreServicesPort);
            CwfTokenRemote = Convert.ToBase64String(Encoding.Default.GetBytes(Regex.Replace(username + ":" + password, "\\\\", "\\")));
        }

        public Version GetCurrentVersion(out string errorMessage)
        {
            var url = string.Format("{0}/IderaCoreServices/v1/Version", CwfUrl);
            CwfAddinInstaller cwfAddinInstaller = new CwfAddinInstaller();
            var response = cwfAddinInstaller.GetRequest(url, out errorMessage, CwfTokenRemote);

            if (string.IsNullOrEmpty(response)) return null;
            if (!string.IsNullOrEmpty(errorMessage)) return null;

            var versionString = response.Trim('\"');

            Version version;

            return Version.TryParse(versionString, out version) ? version : null;
        }

        private void SetTextBoxesInUI(string username, string password, string hostname)
        {
            string url = GetDahboardURL(hostname);
            textBoxDashboardUrl.Text = url;
            textBoxIDUserName.Text = username;
            textBoxIDPassword.Text = password;
            textBoxDashboardUrl.Enabled = true;
            textBoxIDUserName.Enabled = true;
            textBoxIDPassword.Enabled = true;
        }

        private string GetDahboardURL(string hostname)
        {
            var url = string.Format("https://{0}:{1}/", hostname, properties.CoreServicesPort); 
            return url;
        }

        public void getRemoteCredentials(string username, string password, string hostname)
        {
            if (username == null && password == null)
            {
                radioButtonLocal.Checked = true;
            }
            else
            {
                SetTextBoxesInUI(username, password, hostname);
                properties.RemoteUsername = username;
                properties.RemotePassword = password;
                properties.RemoteHostname = hostname;
            }
        }

        private void Credentials_Load(object sender, EventArgs e)
        {
            checkLocalInstalls();
        }

        private void checkLocalInstalls()
        {
            toggleCredentialSection(false);
            Dictionary<string, string> installDetails = InstallationHelper.checkIfDashboardIsAlreadyInstalled();
            properties.installDashboard = true;
            if (Convert.ToBoolean(installDetails["isInstalled"]))
            {
                toggleCredentialSection(true);
                getValuesFromDictionary(installDetails);
                textBoxDashboardUrl.Enabled = true;
                textBoxIDUserName.Enabled = true;
                textBoxIDPassword.Enabled = true;
                installExists = true;
                textBoxIDUserName.Text = properties.IDAccount;
                textBoxDashboardUrl.Text = "http://localhost:" + properties.IDServicePort;
                textBoxIDPassword.Text = properties.servicePassword;
                textBoxIDUserName.Enabled = false;
                textBoxDashboardUrl.Enabled = false;
            }
            if (properties.localInstall)
            {
                radioButtonLocal.Checked = true;
            }
            else
            {
                radioButtonRemote.Checked = true;
                properties.installDashboard = false;
            }
        }

        private void getValuesFromDictionary(Dictionary<string, string> installDetails)
        {
            if (properties.IDAccount == string.Empty)
                properties.IDAccount = installDetails.ContainsKey("Account") ? installDetails["Account"] : Environment.MachineName;
            properties.IDServicePort = installDetails.ContainsKey("CorePort") ? installDetails["CorePort"] : "9292";
            if (properties.IDPath != string.Empty && installDetails.ContainsKey("InstallDir"))
            {
                properties.IDPath = installDetails["InstallDir"];
                Dictionary<string, string> configValues = InstallationHelper.getOldConfigValuesFromLocal(properties.IDPath);
                properties.IDInstance = configValues["RepositoryHost"];
                properties.IDDBName = configValues["RepositoryDatabase"];
                properties.IDServicePort = configValues["ServicePort"];
                properties.WebAppMonitorPort = configValues["WebAppMonitorPort"];
            }
            if (installDetails.ContainsKey("ProductVersion"))
            {
                properties.IDVersion = installDetails["ProductVersion"];
                Version versionCurrent = new Version(InstallationHelper.getCurrentVersion());
                Version versionExists = new Version(installDetails["ProductVersion"]);
                if (versionCurrent <= versionExists)
                {
                    properties.installDashboard = false;
                }
            }
        }

        private void labelHeading_Click(object sender, EventArgs e)
        {

        }

        private void labelNote_Click(object sender, EventArgs e)
        {

        }
    }
}
