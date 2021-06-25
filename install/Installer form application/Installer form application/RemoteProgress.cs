using CWFInstallerService;
using Microsoft.Win32;
using SQLcomplianceCwfAddin.RestService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.ServiceProcess;

namespace Installer_form_application
{
    public partial class RemoteProgress : Form
    {
        bool isCMInstalled = false;
        Form backScreen;
        public RemoteProgress(Form backScreenObj)
        {
            InitializeComponent();
            backScreen = backScreenObj;
        }

        private void RemoteProgress_Shown(Object sender, EventArgs e)
        {
            if (properties.localInstall)
            {
                this.Hide();
                installProduct(false);
            }
            else
            {
                backgroundWorkerRemote.RunWorkerAsync();
            }
        }

        public void setProgressStatus(string status)
        {
            if (!properties.localInstall)
            {
                labelProgress.Text = status;
            }
        }

        private void installProduct(bool isRemoteInstall)
        {
            string currentPath = Directory.GetCurrentDirectory();
            string dashboardMsiLocation = currentPath + @"\" + Constants.dashboardMsiLocation;
            string dashboardx86MsiLocation = currentPath + @"\" + Constants.dashboardx86MsiLocation;
            string SQLCMMsiLocation = currentPath + @"\" + Constants.SQLCMMsiLocation;
            string SQLCMx86MsiLocation = currentPath + @"\" + Constants.SQLCMx86MsiLocation;

            if (Environment.Is64BitOperatingSystem)
            {
                if (System.IO.File.Exists(dashboardMsiLocation) && System.IO.File.Exists(SQLCMMsiLocation))
                {
                    if (properties.installDashboard)
                    {
                        if (!installDashboard())
                        {
                            Constants.installationFailureErrorMessage = "Idera Dashboard Wizard ended prematurely because of an error. To install this program at a later time run Setup again.";
                            InstallationFailure nextScreen = new InstallationFailure();
                            nextScreen.Show();
                            return;
                        }
                    }
                    else
                    {
                        this.Hide();
                    }


                    CwfAddinInstall cwfAddinInstall = new CwfAddinInstall();
                    cwfAddinInstall.Show();
                    cwfAddinInstall.RefreshLabel();
                    CwfAddinInstaller cwfAddinInstaller = new CwfAddinInstaller();
                    bool addinInstallationStatus = false;
                    
                    // Install addin only if required
                    if((properties.oldSQLCMVersion == string.Empty && properties.installedSQLCMVersion == null)
                        || (properties.oldSQLCMVersion != string.Empty && properties.installedSQLCMVersion == null)
                        || (properties.oldSQLCMVersion != string.Empty && properties.installedSQLCMVersion != null && (new Version(properties.oldSQLCMVersion) == properties.installedSQLCMVersion)))
                    {
                        addinInstallationStatus = cwfAddinInstaller.RegisterProduct(currentPath);
                    }
                    else
                    {
                        addinInstallationStatus = true;
                    }

                    if (addinInstallationStatus)
                    {
                        cwfAddinInstall.Hide();
                        if (!properties.isSQLCMUpgrade)
                        {
                            if (installSQLCM(cwfAddinInstaller.CwfUrl, cwfAddinInstaller.CwfToken))
                            {
                                if (isRemoteInstall == false)
                                {
                                    InstallScreen nextScreen = new InstallScreen(this);
                                    nextScreen.Show();
                                }
                            }
                            else
                            {
                                try
                                {
                                    RegistryKey versionKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Idera\\SQLCM", true);
                                    if (versionKey != null)
                                    {
                                        versionKey.SetValue("Version", "", RegistryValueKind.String);
                                        versionKey.Close();
                                    }
                                }
                                catch
                                {

                                }
                                if (isRemoteInstall == false)
                                {
                                    Constants.installationFailureErrorMessage = "Idera SQL Compliance Manager Wizard ended prematurely because of an error. To install this program at a later time run Setup again.";
                                    InstallationFailure nextScreen = new InstallationFailure();
                                    nextScreen.Show();
                                }
                            }
                        }
                        else
                        {
                            if (UpgradeSQLCM(cwfAddinInstaller.CwfUrl, cwfAddinInstaller.CwfToken))
                            {
                                if (isRemoteInstall == false)
                                {
                                    InstallScreen nextScreen = new InstallScreen(this);
                                    nextScreen.Show();
                                }
                            }
                            else
                            {
                                try
                                {
                                    RegistryKey versionKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Idera\\SQLCM", true);
                                    if (versionKey != null)
                                    {
                                        versionKey.SetValue("Version", properties.oldSQLCMVersion, RegistryValueKind.String);
                                        versionKey.Close();
                                    }
                                }
                                catch
                                {
                                }
                                if (isRemoteInstall == false)
                                {
                                    Constants.installationFailureErrorMessage = "Idera SQL Compliance Manager Wizard ended prematurely because of an error. To install this program at a later time run Setup again.";
                                    InstallationFailure nextScreen = new InstallationFailure();
                                    nextScreen.Show();
                                }
                            }
                        }
                    }
                    else
                    {
                        Constants.installationFailureErrorMessage = "CwfAddin installation ended prematurely because of an error. To install this program at a later time run Setup again.";
                        cwfAddinInstall.Hide();
                        InstallationFailure nextScreen = new InstallationFailure();
                        nextScreen.Show();
                    }
                }
                else
                {
                    MessageBox.Show("Files Missing Please Download the correct installer.", "Files Missing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
            }
            else
            {
                if (System.IO.File.Exists(dashboardx86MsiLocation) && System.IO.File.Exists(SQLCMx86MsiLocation))
                {
                    if (properties.installDashboard)
                    {
                        if (!installDashboard())
                        {
                            Constants.installationFailureErrorMessage = "Idera Dashboard Wizard ended prematurely because of an error. To install this program at a later time run Setup again.";
                            InstallationFailure nextScreen = new InstallationFailure();
                            nextScreen.Show();
                            return;
                        }
                    }
                    else
                    {
                        this.Hide();
                    }

                    CwfAddinInstall cwfAddinInstall = new CwfAddinInstall();
                    cwfAddinInstall.Show();
                    Thread.Sleep(5000);
                    CwfAddinInstaller cwfAddinInstaller = new CwfAddinInstaller();
                    bool addinInstallationStatus = false;

                    // Install addin only if required
                    if ((properties.oldSQLCMVersion == string.Empty && properties.installedSQLCMVersion == null)
                        || (properties.oldSQLCMVersion != string.Empty && properties.installedSQLCMVersion == null)
                        || (properties.oldSQLCMVersion != string.Empty && properties.installedSQLCMVersion != null && (new Version(properties.oldSQLCMVersion) == properties.installedSQLCMVersion)))
                    {
                        addinInstallationStatus = cwfAddinInstaller.RegisterProduct(currentPath);
                    }
                    else
                    {
                        addinInstallationStatus = true;
                    }

                    if (addinInstallationStatus)
                    {
                        cwfAddinInstall.Hide();
                        if (!properties.isSQLCMUpgrade)
                        {
                            if (installSQLCM(cwfAddinInstaller.CwfUrl, cwfAddinInstaller.CwfToken))
                            {
                                if (isRemoteInstall == false)
                                {
                                    InstallScreen nextScreen = new InstallScreen(this);
                                    nextScreen.Show();
                                }
                            }
                            else
                            {
                                try
                                {
                                    RegistryKey versionKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Idera\\SQLCM", true);
                                    if (versionKey != null)
                                    {
                                        versionKey.SetValue("Version", "", RegistryValueKind.String);
                                        versionKey.Close();
                                    }
                                }
                                catch
                                {
                                }
                                if (isRemoteInstall == false)
                                {
                                    Constants.installationFailureErrorMessage = "Idera SQL Compliance Manager Wizard ended prematurely because of an error. To install this program at a later time run Setup again.";
                                    InstallationFailure nextScreen = new InstallationFailure();
                                    nextScreen.Show();
                                }
                            }
                        }
                        else
                        {
                            if (UpgradeSQLCM(cwfAddinInstaller.CwfUrl, cwfAddinInstaller.CwfToken))
                            {
                                if (isRemoteInstall == false)
                                {
                                    InstallScreen nextScreen = new InstallScreen(this);
                                    nextScreen.Show();
                                }
                            }
                            else
                            {
                                try
                                {
                                    RegistryKey versionKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Idera\\SQLCM", true);
                                    if (versionKey != null)
                                    {
                                        versionKey.SetValue("Version", properties.oldSQLCMVersion, RegistryValueKind.String);
                                        versionKey.Close();
                                    }
                                }
                                catch
                                {
                                }
                                if (isRemoteInstall == false)
                                {
                                    Constants.installationFailureErrorMessage = "Idera SQL Compliance Manager Wizard ended prematurely because of an error. To install this program at a later time run Setup again.";
                                    InstallationFailure nextScreen = new InstallationFailure();
                                    nextScreen.Show();
                                }
                            }
                        }
                    }
                    else
                    {
                        Constants.installationFailureErrorMessage = "CwfAddin installation ended prematurely because of an error. To install this program at a later time run Setup again.";
                        cwfAddinInstall.Hide();
                        InstallationFailure nextScreen = new InstallationFailure();
                        nextScreen.Show();
                    }
                }
                else
                {
                    MessageBox.Show("Files Missing Please Download the correct installer.", "Files Missing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
            }

        }

        private bool installDashboard()
        {
            bool retVal = false;
            setProgressStatus("Configuring Idera Dashboard Install");
            RegistryKey cwfkey = null;
            RegistryKey localMachineX64View = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            try
            {
                cwfkey = localMachineX64View.OpenSubKey(@"SOFTWARE\Idera\CWF");
            }
            catch (System.Security.SecurityException secex)
            {
                MessageBox.Show("Please run this installer as Admin user" + " Security Exception: " + secex.ToString());
                Application.Exit();
            }

            string productArgs = " /l*V InstallDashboardLog.log /passive AGREETOLICENSE=yes SERVICE_ACCOUNT=" + properties.IDSUsername;
            productArgs += " SERVICE_PASSWORD=" + properties.IDSPassword;
            productArgs += " REPOSITORY_CORE_DATABASE=" + properties.IDDBName + " REPOSITORY_INSTANCE=" + properties.IDInstance;
            productArgs += " WEBAPP_PORT=" + properties.WebAppServicePort + " WEBAPP_MONITOR_PORT=" + properties.WebAppMonitorPort;
            productArgs += " WEBAPP_SSL_PORT=" + properties.WebAppSSLPort + " COLLECTION_PORT=" + properties.CoreServicesPort;

            if (!properties.upgrading)
            {
                productArgs += " INSTALLDIR=\"" + properties.IDPath + "\"";
            }

            if (properties.SQLAUTHID)
            {
                productArgs += " REPOSITORY_SQLAUTH=1 REPOSITORY_SQLUSERNAME=" + properties.SQLUsernameID + " REPOSITORY_SQLPASSWORD=" + properties.SQLPasswordID;
            }
            if (properties.localInstall)
            {
                if (Environment.Is64BitOperatingSystem)
                    productArgs = " /i \"" + Constants.dashboardMsiLocation + "\"" + productArgs;
                else
                    productArgs = " /i \"" + Constants.dashboardx86MsiLocation + "\"" + productArgs;
                if (InstallNewMSI(productArgs))
                {
                    retVal = true;
                }
                else
                {
                    retVal = false;
                }
            }
            else
            {
                setProgressStatus("Copying MSI File on Remote PC");
                bool status = InstallationHelper.copyMsiToRemote(properties.RemoteUsername, properties.RemotePassword, properties.RemoteHostname, "IderaDashboard.msi");
                if (!status)
                {
                    MessageBox.Show("Could Not connect to remote machine.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
                productArgs = " /i C:\\temp\\IderaDashboard.msi" + productArgs;
                setProgressStatus("Installing Idera Dashboard on remote PC");
                status = InstallationHelper.install_msi_on_remote(properties.RemoteUsername, properties.RemotePassword, properties.RemoteHostname, productArgs);
                if (!status)
                {
                    retVal = false;
                    MessageBox.Show("Could Not connect to remote machine.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
            }
            return retVal;
        }

        private bool installSQLCM(string cwfURL, string cwfToken)
        {
            bool retVal = false;
            setProgressStatus("Configuring Idera SQL Compliance Manager");

            string productArgs = "/l*V InstallSQLCMLog.log AGREETOLICENSE=yes INSTALLDIR=\"" + properties.SPPath + "\"";
            productArgs += " TRACEDIR_COLLECT=\"" + properties.SPPath + "\\CollectionServerTraceFiles\"";
            productArgs += " CWF_URL=\"" + cwfURL.Replace("\\", "\\\\") + "\"";
            productArgs += " CWF_TOKEN=\"" + cwfToken + "\"";
            productArgs += " SERVICEUSERNAME=\"" + properties.SPSUsername + "\"";
            productArgs += " SERVICEPASSWORD=\"" + properties.SPSPassword + "\"";
            productArgs += " REPOSITORY=\"" + properties.JMInstance + "\"";
            //productArgs += " REPOSITORY=\"" + properties.SPSUsername.Substring(0, properties.SPSUsername.IndexOf('\\')) + "\"";
            productArgs += " TARGETDIR=\"" + @"C:\" + "\"";
            productArgs += " TRACEDIR_AGENT=\"" + properties.SPPath + "\\AgentTraceFiles" + "\"";
            productArgs += " /qb+!";

            if (properties.isTaggableSampleProduct)
            {
                productArgs += " TAGGING_ENABLED=1";
            }
            if (properties.SQLAUTH2)
            {
                productArgs += " REPOSITORY_SQLAUTH=1 REPOSITORY_SQLUSERNAME=" + properties.SQLUsername2 + " REPOSITORY_SQLPASSWORD=" + properties.SQLPassword2;
            }
            if (properties.isTaggableSampleProduct)
            {
                productArgs += " TAGGING_ENABLED=1";
            }
            if (Environment.Is64BitOperatingSystem)
                productArgs = " /i \"" + Constants.SQLCMMsiLocation + "\" " + productArgs;
            else
                productArgs = " /i \"" + Constants.SQLCMx86MsiLocation + "\" " + productArgs;

            if (InstallNewMSI(productArgs))
            {
                retVal = true;
                isCMInstalled = true;
                SetRegistryforAddin();
            }
            return retVal;
        }

        private void SetRegistryforAddin()
        {
            try
            {
                RegistryKey versionKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Idera\\SQLCM", true);
                if (versionKey != null)
                {
                    versionKey.SetValue("Version", RestServiceConstants.ProductVersion, RegistryValueKind.String);
                    versionKey.Close();
                }
            }
            catch
            {
            }
        }

        private bool UpgradeSQLCM(string cwfURL, string cwfToken)
        {
            bool retVal = false;
            setProgressStatus("Configuring Idera SQL Compliance Manager");

            string productArgs = "/l*V InstallSQLCMLog.log AGREETOLICENSE=yes INSTALLDIR=\"" + properties.SPPath + "\"";
            productArgs += " TRACEDIR_COLLECT=\"" + properties.SPPath + "\\CollectionServerTraceFiles\"";
            productArgs += " CWF_URL=\"" + cwfURL.Replace("\\", "\\\\") + "\"";
            productArgs += " CWF_TOKEN=\"" + cwfToken + "\"";
            productArgs += " SERVICEUSERNAME=\"" + properties.SPSUsername + "\"";
            productArgs += " SERVICEPASSWORD=\"" + properties.SPSPassword + "\"";
            productArgs += " REPOSITORY=\"" + properties.JMInstance + "\"";
            //productArgs += " REPOSITORY=\"" + properties.SPSUsername.Substring(0, properties.SPSUsername.IndexOf('\\')) + "\"";
            productArgs += " TARGETDIR=\"" + @"C:\" + "\"";
            productArgs += " TRACEDIR_AGENT=\"" + properties.SPPath + "\\AgentTraceFiles" + "\"";
            productArgs += " REINSTALL=\"" + "ALL" + "\"";
            productArgs += " REINSTALLMODE=\"" + "vomus" + "\"";
            productArgs += " IS_MINOR_UPGRADE=\"" + "1" + "\"";
            productArgs += " USE_EXISTING_DATABASE=\"" + "1" + "\"";
            productArgs += " UPGRADE_SCHEMA=\"" + "1" + "\"";
            productArgs += " /qb+!";

            if (properties.isTaggableSampleProduct)
            {
                productArgs += " TAGGING_ENABLED=1";
            }
            if (properties.SQLAUTH2)
            {
                productArgs += " REPOSITORY_SQLAUTH=1 REPOSITORY_SQLUSERNAME=" + properties.SQLUsername2 + " REPOSITORY_SQLPASSWORD=" + properties.SQLPassword2;
            }
            if (properties.isTaggableSampleProduct)
            {
                productArgs += " TAGGING_ENABLED=1";
            }

            if (Environment.Is64BitOperatingSystem)
                productArgs = " /i \"" + Constants.SQLCMMsiLocation + "\" " + productArgs;
            else
                productArgs = " /i \"" + Constants.SQLCMx86MsiLocation + "\" " + productArgs;
            if (InstallNewMSI(productArgs))
            {
                retVal = true;
                isCMInstalled = true;
                SetRegistryforAddin();
            }
            return retVal;
        }

        private bool InstallNewMSI(string productArgs)
        {
            try
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo.FileName = @"msiexec";
                process.StartInfo.Arguments = productArgs;
                process.Start();
                process.WaitForExit();
                int retVal = process.ExitCode;
                process.Close();
                if (retVal != 0 && retVal != 3010)
                {
                    return false;
                }
                return true;
            }
            catch
            {
                // process exception
                return false;
            }
        }

        private bool RunServices()
        {
            try
            {
                ServiceController AgentService = new ServiceController(RestServiceConstants.AgentService);
                TimeSpan AgentTimeout = TimeSpan.FromMilliseconds(2000);
                AgentService.Start();
                AgentService.WaitForStatus(ServiceControllerStatus.Running, AgentTimeout);
                ServiceController CollectionService = new ServiceController(RestServiceConstants.CollectionService);
                TimeSpan CollectionTimeout = TimeSpan.FromMilliseconds(2000);
                CollectionService.Start();
                CollectionService.WaitForStatus(ServiceControllerStatus.Running, CollectionTimeout);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void backgroundWorkerRemote_DoWork(object sender, DoWorkEventArgs e)
        {
            installProduct(true);
        }

        private void backgroundWorkerRemote_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBarRemote.MarqueeAnimationSpeed = 30;
        }

        private void backgroundWorkerRemote_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Hide();
            if (isCMInstalled)
            {
                InstallScreen nextScreen = new InstallScreen(this);
                nextScreen.Show();
            }
            else
            {
                Constants.installationFailureErrorMessage = "Idera SQL Compliance Manager Wizard ended prematurely because of an error. To install this program at a later time run Setup again.";
                InstallationFailure nextScreen = new InstallationFailure();
                nextScreen.Show();
            }
        }
    }
}
