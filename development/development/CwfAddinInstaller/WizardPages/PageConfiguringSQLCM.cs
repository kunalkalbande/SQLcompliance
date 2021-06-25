using Microsoft.Win32;
using SQLcomplianceCwfAddin.RestService;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace CwfAddinInstaller.WizardPages
{
    [WizardPageInfo("Installing SQL Compliance Manager", "", WizardPage.ConfiguringSQLCM)]
    internal partial class PageConfiguringSQLCM : WizardPageBase
    {
        //public static string SQLCMSQLServerInstanceName = string.Empty;

        public PageConfiguringSQLCM(FormInstaller host)
            : base(host)
        {
            InitializeComponent();
        }

        internal override void Initialize()
        {
            Host.Log("{0} - Initialized.", WizardPage.ConfiguringSQLCM);
            IsInitialized = true;
        }

        internal override bool DoAction(out string errorMessage, out Control invalidControl)
        {
            errorMessage = null;
            invalidControl = null;

            /*SQLCMSQLServerInstanceName = GetSQLCMSQLServerNameFromRegistry();
            Host.CwfHelper.RepositoryInstance = SQLCMSQLServerInstanceName;
            if (SQLCMSQLServerInstanceName == "")
            {
                Host.NavigateToPage(NavigationDirection.Finish);
            }*/

            Host.Log("Yes we are ready to configure.");
            return true;
        }

        internal override void OnNavigated(NavigationDirection direction)
        {
            string installedSQLCMVersion = string.Empty;
            installedSQLCMVersion = GetInstalledVersionFromRegistry();
            
            if (installedSQLCMVersion == RestServiceConstants.ProductVersion)
            {
                Text = "SQL Compliance Manager (CWF Add-In Installer)";
                MessageBox.Show(this, "Skipping SQLCM Product Installation. Current Version is already installed on the system", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                Host.NavigateToPage(NavigationDirection.Next);
            }
            else
            {
                RunProgressBar();
            }
        }

        private void RunProgressBar()
        {
            this.progressBarSQLCM.Value = 0;
            this.timerSQLCM.Interval = 100;
            this.timerSQLCM.Enabled = true;
        }

        private void timerSQLCM_Tick(object sender, EventArgs e)
        {
            if (this.progressBarSQLCM.Value < 100)
            {
                this.progressBarSQLCM.Value += 10;
            }
            else
            {
                timerSQLCM.Dispose();
                if (InstallSQLCM())
                {
                    Host.NavigateToPage(NavigationDirection.Next);
                }
                else
                {
                    Host.NavigateToPage(NavigationDirection.Error);
                }
            }
        }

        private bool InstallSQLCM()
        {
            if (!File.Exists(Host.MainApplicationInstaller))
            {
                Host.Log("Can't launch the main SQLCM appplication installer '{0}' as it's missing.", Host.MainApplicationInstaller);
                Host.ShowError("Can't launch the main SQLCM appplication installer '{0}' as it's missing.", Host.MainApplicationInstaller);
                return false;
            }

            try
            {
                var startInfo = new ProcessStartInfo();
                startInfo.FileName = Host.MainApplicationInstaller;
                //startInfo.Arguments = string.Format("/v\"CWF_URL=\"{0}\" CWF_TOKEN=\"{1}\"",
                                                    //Host.CwfHelper.CwfUrl, Host.CwfHelper.CwfToken);
                //Process.Start(startInfo);

                var p = new Process { StartInfo = { FileName = Host.MainApplicationInstaller } };
                p.Start();
                p.WaitForExit();

                if (p.ExitCode == 0 || p.ExitCode == 3010)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Host.Log("An error occured while launching main SQLCM application installer.{0}{0}Error: {1}{0}{0}Stack Trace: {2}", Environment.NewLine, ex.Message, ex.StackTrace);
                Host.ShowError("An error occured while launching main SQLCM application installer '{1}'.{0}Error: {2}", Environment.NewLine, Host.MainApplicationInstaller, ex.Message);
                return false;
            }
        }

        private string GetInstalledVersionFromRegistry()
        {
            string currentVersion = string.Empty;
            RegistryKey view;
            if (Environment.Is64BitOperatingSystem)
            {
                view = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            }
            else
            {
                view = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
            }
            try
            {
                using (RegistryKey cmKey = view.OpenSubKey(@"SOFTWARE\Idera\SQLCM", false))
                {
                    object currentInstalledVersion = cmKey.GetValue("Version");
                    if (currentInstalledVersion != null)
                    {
                        currentVersion = currentInstalledVersion.ToString();
                        return currentVersion;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        /*private string GetSQLCMSQLServerNameFromRegistry()
        {
            string CollectionServerName = string.Empty;
            RegistryKey view;
            if (Environment.Is64BitOperatingSystem)
            {
                view = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            }
            else
            {
                view = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
            }
            try
            {
                using (RegistryKey cmCollectionKey = view.OpenSubKey(@"SOFTWARE\Idera\SQLCM\CollectionService", false))
                {
                    object SQLServerName = cmCollectionKey.GetValue("ServerInstance");
                    if (SQLServerName != null)
                    {
                        CollectionServerName = SQLServerName.ToString();
                    }
                    else
                    {
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return CollectionServerName;
        }*/
    }
}
