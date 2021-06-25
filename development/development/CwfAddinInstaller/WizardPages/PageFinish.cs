using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace CwfAddinInstaller.WizardPages
{
    [WizardPageInfo("Success!", "", WizardPage.Finish)]
    internal partial class PageFinish : WizardPageBase
    {
        private bool launchManagementConsole = false;
        public PageFinish(FormInstaller host)
            : base(host)
        {
            InitializeComponent();
        }

        internal override void Initialize()
        {
            IsInitialized = true;
        }

        internal override void OnNavigated(NavigationDirection direction)
        {
            if(IsManagementConsoleInstalled())
            {
                chkLaunchmainApplication.Visible = true;
            }
            DeleteManagementConsoleLaunchFromRegistry();
        }

        internal override bool DoAction(out string errorMessage, out Control invalidControl)
        {
            errorMessage = null;
            invalidControl = null;
            if (!chkLaunchmainApplication.Checked)
                return true;
            if (launchManagementConsole)
                LaunchManagementConsole();
            return true;
        }

        public void LaunchManagementConsole()
        {
            string installPath = string.Empty;
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
                    object installPathObject = cmKey.GetValue("Path");
                    if (installPathObject != null)
                    {
                        installPath = installPathObject.ToString();
                    }
                    else
                    {
                        Text = "SQL Compliance Manager (CWF Add-In Installer)";
                        MessageBox.Show(this, "Unable to find the path for SQL Compliance Manager installation.", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                if (File.Exists(Path.Combine(installPath, "SQLcompliance.exe")))
                {
                    Process.Start(Path.Combine(installPath, "SQLcompliance.exe"));
                }
                else
                {
                    Text = "SQL Compliance Manager (CWF Add-In Installer)";
                    MessageBox.Show(this, "Unable to locate SQLComplaince Management Console.", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception ex)
            {
                Text = "SQL Compliance Manager (CWF Add-In Installer)";
                MessageBox.Show(this, "Unable to find the path for SQL Compliance Manager installation.", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public bool IsManagementConsoleInstalled()
        {
            string installedType = string.Empty;
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
                    object isManagementConsoleInstalled = cmKey.GetValue("IsLaunchManagementConsole");
                    if (isManagementConsoleInstalled.ToString() == "True")
                    {
                        launchManagementConsole = true;
                        return true;
                    }
                    else
                    {
                        return false;    
                    }
                }
            }
            catch (Exception ex)
            {
                return false;    
            }
        }

        private void DeleteManagementConsoleLaunchFromRegistry()
        {
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
                using (RegistryKey cmKey = view.OpenSubKey(@"SOFTWARE\Idera\SQLCM", true))
                {
                    cmKey.DeleteValue("IsLaunchManagementConsole");
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
