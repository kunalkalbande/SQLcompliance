using CWFInstallerService;
using Microsoft.Win32;
using SQLcomplianceCwfAddin.RestService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Installer_form_application
{

    public partial class Description : Form
    {
        Object reader = null;
        String InstanceName = String.Empty;
        Form screenObject;
        public Description(Form screenObj)
        {
            InitializeComponent();
            MinimizeBox = false;
            MaximizeBox = false;
            screenObject = screenObj;
        }

        private void Description_Load(object sender, EventArgs e)
        {
            if (properties.localInstall)
            {
                bool errorScenario = CheckCMUpgrade();
                if ((properties.upgrading && properties.isSQLCMUpgrade) || errorScenario)
                {
                    if (!errorScenario)
                    {
                        properties.JMInstance = properties.IDInstance;
                    }
                    string localInstanceName = GetInstalledProductName(false);

                    properties.DisplayName = localInstanceName;
                }
                textBoxIDPath.Text = properties.IDPath;
                if (properties.IDVersion != string.Empty)
                {
                    labelVersionDescription.Text = "Looks like you have Idera Dashboard v" + properties.IDVersion + ". We will be performing the following:";
                    string dashboardText = string.Empty;
                    if (properties.installDashboard && properties.upgrading)
                    {
                        dashboardText = "2. Upgrading Idera Dashboard to v" + CWFInstallerService.Constants.version;
                    }
                    else if (properties.installDashboard && !properties.upgrading)
                    {
                        dashboardText = "2. Installing Idera Dashboard v" + CWFInstallerService.Constants.version;
                    }
                    if (properties.isSQLCMUpgrade)
                    {
                        labelVersionDescription.Text += "\n   1. Upgrading SQL Compliance Manager \n   " + dashboardText;
                    }
                    else
                    {
                        labelVersionDescription.Text += "\n   1. Installing SQL Compliance Manager \n   " + dashboardText;
                    }
                }
                labelFeatures.Text = "We have implemented a number of new features in the upgraded Idera Dashboard experience like common administration tasks," +
                    "global tags, improved UI, and much more.";
                textBoxSPPath.Text = properties.SPPath;
                textBoxDisplayName.Text = properties.DisplayName;
                if (properties.upgrading && properties.installedSQLCMVersion != null)
                {
                    textBoxIDPath.Enabled = false;
                    textBoxDisplayName.Enabled = false;
                    if (!errorScenario)
                    {
                        textBoxSPPath.Enabled = false;
                    }
                }
            }
            else
            {
                textBoxSPPath.Text = properties.SPPath;
                textBoxDisplayName.Text = properties.DisplayName;
                textBoxIDPath.Hide();
                labelPathID.Hide();

                CheckCMUpgrade();

                if (properties.isSQLCMUpgrade)
                {
                    labelVersionDescription.Text = "We will be performing the following:\n\n 1. Upgrading SQL Compliance Manager.";
                }
                else
                {
                    labelVersionDescription.Text = "We will be performing the following:\n\n 1. Installing SQL Compliance Manager.";
                }

                string remoteInstanceName = GetInstalledProductName(true);
                if (!remoteInstanceName.Equals("") && properties.installedSQLCMVersion != null)
                {
                    textBoxDisplayName.Text = remoteInstanceName;
                    textBoxDisplayName.Enabled = false;
                }

                if (properties.isSQLCMUpgrade)
                {
                    textBoxSPPath.Enabled = false;
                }
            }
        }

        private bool CheckCMUpgrade()
        {
            bool returnVal = false;
            RegistryKey view;
            if (Environment.Is64BitOperatingSystem)
            {
                view = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            }
            else
            {
                view = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
            }
            using (RegistryKey cmKey = view.OpenSubKey(@"SOFTWARE\Idera", false))
            {
                if (cmKey != null)
                {
                    foreach (string subKeyName in cmKey.GetSubKeyNames())
                    {
                        if (subKeyName.ToUpper().Equals("SQLCM") || subKeyName.Equals("SQLcompliance"))
                        {
                            try
                            {
                                string installPath = string.Empty;
                                string oldVersion = cmKey.OpenSubKey(subKeyName).GetValue("Version").ToString();
                                properties.oldSQLCMVersion = oldVersion;
                                object installPathObject = cmKey.OpenSubKey(subKeyName).GetValue("Path");
                                if (installPathObject != null)
                                {
                                    installPath = installPathObject.ToString();
                                }

                                if ((oldVersion == null || oldVersion == "") && (installPath == null || installPath == string.Empty))
                                {
                                    return true;
                                }

                                RegistryKey collectionServiceKey = cmKey.OpenSubKey(subKeyName).OpenSubKey("CollectionService");
                                string sqlServerInstance = collectionServiceKey.GetValue("ServerInstance").ToString();
                                if (installPath != null && installPath != string.Empty)
                                {
                                    if (sqlServerInstance != null && sqlServerInstance != String.Empty)
                                    {
                                        properties.JMInstance = sqlServerInstance;
                                    }
                                    properties.SPPath = installPath;
                                    properties.isSQLCMUpgrade = true;
                                    returnVal = false;
                                }
                            }
                            catch
                            {
                                returnVal = true;
                            }
                        }
                    }
                }
            }
            return returnVal;
        }

        private string GetInstalledProductName(bool remoteDashboard)
        {
            string dashboardUrl = string.Empty;
            string CwfTokenRemote = string.Empty;
            if (remoteDashboard)
            {
                dashboardUrl = string.Format("http://{0}:{1}", properties.RemoteHostname, properties.CoreServicesPort);
                CwfTokenRemote = Convert.ToBase64String(Encoding.Default.GetBytes(Regex.Replace(properties.RemoteUsername + ":" + properties.RemotePassword, "\\\\", "\\")));
            }
            else
            {
                dashboardUrl = properties.dashboardUrl;
                CwfTokenRemote = Convert.ToBase64String(Encoding.Default.GetBytes(Regex.Replace(properties.serviceUsername + ":" + properties.servicePassword, "\\\\", "\\")));
            }


            var url = String.Format("{0}/IderaCoreServices/v1/Products?shortname={1}",
                                    dashboardUrl,
                                    RestServiceConstants.ProductShortName);
            string errorMessage;
            CwfAddinInstaller installer = new CwfAddinInstaller();
            var response = installer.GetRequest(url, out errorMessage, CwfTokenRemote);

            Products m_registeredProducts = NewJsonHelper.FromJson<Products>(response);

            foreach (Product upgradeProduct in m_registeredProducts)
            {
                string location = upgradeProduct.Location.Replace(".", Environment.MachineName).ToUpper().Replace("(LOCAL)", Environment.MachineName);
                if (location.StartsWith(Environment.MachineName))
                {
                    try
                    {
                        properties.installedSQLCMVersion = new Version(upgradeProduct.Version);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Invalid Product version in RegisteredProduct table");
                    }
                    return upgradeProduct.InstanceName;
                }
            }

            return string.Empty;
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            properties.IDPath = textBoxIDPath.Text;
            properties.SPPath = textBoxSPPath.Text;
            properties.DisplayName = textBoxDisplayName.Text;
            this.Hide();
            screenObject.Show();
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            this.Hide();
            try
            {
                if (properties.upgrading && properties.oldSQLCMVersion != "" && (new Version(properties.oldSQLCMVersion)) == properties.installedSQLCMVersion)
                {
                    bool instanceNameExists = SampleProductInstallationHelper.checkIfProductInstanceExists(properties.dashboardUrl, textBoxDisplayName.Text, properties.serviceUsername, properties.servicePassword);
                    bool isPreviousVersion = SampleProductInstallationHelper.checkPreviousProductVersion(properties.dashboardUrl, properties.serviceUsername, properties.servicePassword);
                    if (isPreviousVersion) throw new VersionMismatchException();
                    if (instanceNameExists) throw new InstanceExistsException();
                }
                else if (properties.upgrading && properties.oldSQLCMVersion != "" && (new Version(properties.oldSQLCMVersion)) != properties.installedSQLCMVersion)
                {
                    // Error scenario when CM upgrade failed
                }
                else
                {
                    if (properties.localInstall)
                    {
                        Validator.validatePath(textBoxIDPath.Text);
                        string driveName = textBoxIDPath.Text.Split('\\')[0];
                        Validator.ValidateDiskSpaceForDashboard(driveName);
                        if (properties.oldSQLCMVersion == "" && properties.installedSQLCMVersion == null)
                        {
                            Validator.validateInstanceName(textBoxDisplayName.Text);
                        }
                        else
                        {
                            // Error scenario when CM frash failed
                        }
                    }
                }
            }
            catch (CWFBaseException ex)
            {
                MessageBox.Show(ex.ErrorCode + " - " + ex.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Show();
                return;
            }
            properties.DisplayName = textBoxDisplayName.Text;
            properties.IDPath = textBoxIDPath.Text;
            properties.SPPath = textBoxSPPath.Text;

            if (!properties.localInstall && textBoxDisplayName.Enabled != false && properties.installedSQLCMVersion == null)
            {
                try
                {
                    string dashboardUrl = string.Format("http://{0}:{1}", properties.RemoteHostname, properties.CoreServicesPort);
                    string CwfTokenRemote = Convert.ToBase64String(Encoding.Default.GetBytes(Regex.Replace(properties.RemoteUsername + ":" + properties.RemotePassword, "\\\\", "\\")));

                    var url = String.Format("{0}/IderaCoreServices/v1/Products?shortname={1}",
                                            dashboardUrl,
                                            RestServiceConstants.ProductShortName);
                    string errorMessage;
                    CwfAddinInstaller installer = new CwfAddinInstaller();
                    var response = installer.GetRequest(url, out errorMessage, CwfTokenRemote);

                    Products m_registeredProducts = NewJsonHelper.FromJson<Products>(response);
                    if (m_registeredProducts != null)
                    {
                        foreach (Product upgradeProduct in m_registeredProducts)
                        {
                            if (upgradeProduct.InstanceName.Equals(textBoxDisplayName.Text))
                            {
                                MessageBox.Show(String.Format("Another product is already registered with '{0}' display name. Please specify another display name.", textBoxDisplayName.Text), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                this.Show();
                                return;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }

            ServiceAccount nextScreen = new ServiceAccount(this);
            nextScreen.Show();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("Do you really want to exit?", "Exit", MessageBoxButtons.YesNo))
            {
                Application.Exit();
            }
        }
    }
}
