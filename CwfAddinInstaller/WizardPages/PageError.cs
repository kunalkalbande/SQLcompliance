using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace CwfAddinInstaller.WizardPages
{
    [WizardPageInfo("Failed IDERA SQL Compliance Manager &&\r\nDashboard Setup", "", WizardPage.Error)]
    internal partial class PageError : WizardPageBase
    {
        public PageError(FormInstaller host)
            : base(host)
        {
            InitializeComponent();
        }

        internal override void Initialize()
        {
            Host.Log("{0} - Initialized.", WizardPage.Error);
            IsInitialized = true;
        }

        internal override void OnNavigated(NavigationDirection direction)
        {
            Host.lblPageTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            Host.lblPageDescription.Location = new System.Drawing.Point(173, 45);
            Host.lblPageDescription.BringToFront();
        }

        internal override bool DoAction(out string errorMessage, out Control invalidControl)
        {
            errorMessage = null;
            invalidControl = null;
            DeleteManagementConsoleLaunchFromRegistry();
            Host.CloseInstaller();
            return true;
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
