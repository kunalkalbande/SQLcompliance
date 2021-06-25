using SQLcomplianceCwfAddin.RestService;
using System;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Threading;
using System.Windows.Forms;

namespace CwfAddinInstaller.WizardPages
{
    [WizardPageInfo("Installing IDERA Dashboard", "", WizardPage.ConfiguringCWFDashboard)]
    internal partial class PageConfiguringCWFDashboard : WizardPageBase
    {
        public PageConfiguringCWFDashboard(FormInstaller host)
            : base(host)
        {
            InitializeComponent();
        }

        internal override void Initialize()
        {
            Host.Log("{0} - Initialized.", WizardPage.ConfiguringCWFDashboard);
            IsInitialized = true;
        }

        internal override bool DoAction(out string errorMessage, out Control invalidControl)
        {
            errorMessage = null;
            invalidControl = null;

            Host.Log("Yes we are ready to configure.");
            return true;
        }

        internal override void OnNavigated(NavigationDirection direction)
        {
            Host.lblPageTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            RunProgressBar();
        }

        private void RunProgressBar()
        {
            this.progressBarCWF.Value = 0;
            this.timerCWF.Interval = 100;
            this.timerCWF.Enabled = true;

        }

        private void timerCWF_Tick(object sender, EventArgs e)
        {
            if (this.progressBarCWF.Value < 100)
            {
                this.progressBarCWF.Value += 10;
            }
            else
            {
                timerCWF.Dispose();
                if (InstallDashboard())
                {
                    StartRegistrationService();
                    Host.NavigateToPage(NavigationDirection.Next);
                }
                else
                {
                    Host.NavigateToPage(NavigationDirection.Error);
                }
            }
        }

        private bool InstallDashboard()
        {
            string errorMessage = string.Empty;
            // try to install dashboard locally
            if (PageDashboardLocation.installNewLocalDashboard || PageDashboardLocation.upgradeLocalDashboard)
            {
                if (InstallLocalDashboard())
                {
                    errorMessage = Host.CwfHelper.LoadRegisteredProducts();
                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        Host.Log(errorMessage);
                        return false;
                    }

                    Host.Log("CWF Dashboard URL: {0}", Host.CwfHelper.CwfUrl);
                    Host.Log("CWF Dashboard Administrator: {0}", Host.CwfHelper.CwfUser);
                    Host.Log("CWF Dashboard Administartor Password: {0}", FormInstaller.NoPassowrdlogging);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        private bool InstallLocalDashboard()
        {
            if (!File.Exists(Host.CwfDashboardInstaller))
            {
                MessageBox.Show(string.Format("SQL Compliance Manager dashboard installer file '{0}' is missing.", Host.CwfDashboardInstaller));
                return false;
            }

            try
            {
                var p = new Process { StartInfo = { FileName = @"msiexec", Arguments = " /i \"" + Host.CwfDashboardInstaller + "\"" } };
                p.Start();
                p.WaitForExit();

                if (p.ExitCode == 0 || p.ExitCode == 3010)
                {
                    cwfInstallationTxt.Text = "Starting CWF Dashboard Services, Please wait...";
                    this.Refresh();

                    // sleep for half minute to give time to dashboard services to initialize
                    Thread.Sleep(30000);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("An error occured while launching IDERA dashboard installer. Error: {0}", ex.Message));
                return false;
            }
        }

        private bool StartRegistrationService()
        {
            try
            {
                ServiceController RegistrationService = new ServiceController(RestServiceConstants.RegistrationService);
                TimeSpan AgentTimeout = TimeSpan.FromMilliseconds(2000);
                RegistrationService.Start();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
