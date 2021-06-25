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
using System.Reflection;
using Microsoft.Win32;
using SQLCM_Installer.Custom_Controls;
using System.ServiceProcess;
using System.Diagnostics;
using System.Threading;
using System.Security.Principal;
using System.Security.AccessControl;

namespace SQLCM_Installer.WizardPages
{
    [WizardPageInfo(WizardPage.Install)]
    internal partial class PageInstall : WizardPageBase
    {
        string[] agentInstanceList;
        private int installerReturnCode = 0;
        private readonly Version m_installerVersion = new Version(Constants.ProductVersion);
        private int currentProgress = 0;
        private int currentProgressLimit = 0;
        private int progressSpeed = 1500;
        private bool installationFailure = false;
        private bool IsNativeClientInstalled = true;

        public PageInstall(MainForm host)
            : base(host)
        {
            InitializeComponent();
        }

        internal override void OnNavigated(NavigationDirection direction)
        {
            SetPageHeader();
            backgroundWorker.RunWorkerAsync();
        }

        public void setProgressStatus(string status)
        {
            if (labelInstallationText.InvokeRequired)
            {
                labelInstallationText.Invoke(new Action<String>(setProgressStatus), new object[] { status });
            }
            else
            {
                labelInstallationText.Text = status;
            }
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string fileName = "";
            if (Environment.Is64BitOperatingSystem)
            {
                fileName = "SQLcompliance-x64.exe";
            }
            else
            {
                fileName = "SQLcompliance.exe";
            }

            if (Constants.isDeleteAction)
            {
                HelperFunctions helperFunction = new HelperFunctions();
                try
                {
                    helperFunction.IsSQLComplianceDBRemove(InstallProperties.IsCMSQLServerSQLAuth, InstallProperties.CMSQLServerInstanceName, InstallProperties.CMSQLServerUsername, InstallProperties.CMSQLServerPassword);
                 }
                catch (Exception th)
                {
                }
            }

            switch (Constants.UserInstallType)
            {
                #region DashboardOnly
                case InstallType.DashboardOnly:
                    {
                        LogHelper.SetLogger(InstallProperties.DashboardInstallDir + Constants.SQLDashboardRevisionLogFile);

                        if (!InstallProperties.IsUpgradeRadioSelection)
                        {
                            currentProgress = 0;
                            currentProgressLimit = 99;
                            progressSpeed = 1000;
                            backgroundWorkerProgress.RunWorkerAsync();
                            setProgressStatus("Installing IDERA Dashboard...");
                            StopRegistrationService();
                            if (!InstallDashboard())
                            {
                                StartRegistrationService();
                                installationFailure = true;
                                Constants.FailedProduct = Products.Dashboard;
                                return;
                            }
                            else
                            {
                                //dashboard installation was success
                                UpdateRevisionLogs(Constants.ProductMap[Products.Dashboard], Constants.InstalledCMVersion, Application.ProductVersion);

                                if (backgroundWorkerProgress.IsBusy)
                                {
                                    backgroundWorkerProgress.CancelAsync();
                                    Thread.Sleep(5000);
                                }
                                StartRegistrationService();
                            }
                        }
                        else
                        {
                            currentProgress = 0;
                            currentProgressLimit = 80;
                            progressSpeed = 1000;
                            backgroundWorkerProgress.RunWorkerAsync();
                            setProgressStatus("Upgrading IDERA Dashboard...");
                            StopRegistrationService();
                            if (!InstallDashboard())
                            {
                                StartRegistrationService();
                                installationFailure = true;
                                Constants.FailedProduct = Products.Dashboard;
                                return;
                            }
                            else
                            {
                                //dashboard upgrade was success
                                UpdateRevisionLogs(Constants.ProductMap[Products.Dashboard], Constants.InstalledCMVersion, Application.ProductVersion);

                                StartRegistrationService();
                                if (InstallProperties.isCMCurrentVersionInstalled)
                                {
                                    if (backgroundWorkerProgress.IsBusy)
                                    {
                                        backgroundWorkerProgress.CancelAsync();
                                        Thread.Sleep(5000);
                                    }
                                    setProgressStatus("Starting CWF Dashboard Services, Please wait...");
                                    Thread.Sleep(30000);
                                    currentProgressLimit = 99;
                                    backgroundWorkerProgress.RunWorkerAsync();
                                    setProgressStatus("Gathering required data for registration...");
                                    if (!GetUpgradeData())
                                    {
                                        installationFailure = true;
                                        Constants.FailedProduct = Products.Registration;
                                        return;
                                    }
                                    else
                                    {
                                        if (InstallProperties.RegisterProductToDashboard)
                                        {
                                            setProgressStatus("Registering SQL Compliance Manager with IDERA Dashboard...");
                                            if (!Host.CwfHelper.RegisterProduct())
                                            {
                                                installationFailure = true;
                                                Constants.FailedProduct = Products.Registration;
                                                return;
                                            }
                                            if (backgroundWorkerProgress.IsBusy)
                                            {
                                                backgroundWorkerProgress.CancelAsync();
                                                Thread.Sleep(5000);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (backgroundWorkerProgress.IsBusy)
                                    {
                                        backgroundWorkerProgress.CancelAsync();
                                        Thread.Sleep(5000);
                                    }
                                }
                            }
                        }
                        break;
                    }
                #endregion

                #region CMOnly
                case InstallType.CMOnly:
                    {
                        if (!InstallNativeClient())
                        {                           
                            IsNativeClientInstalled = false;
                            installationFailure = true;
                            Constants.FailedProduct = Products.Compliance;
                            return;
                        }

                        LogHelper.SetLogger(InstallProperties.CMInstallDir + Constants.SQLCMRevisionLogFile);
                        if (!InstallProperties.IsUpgradeRadioSelection)
                        {
                            if (!InstallProperties.Clustered)
                            {
                                currentProgress = 0;
                                currentProgressLimit = 80;
                                progressSpeed = 2000;
                                backgroundWorkerProgress.RunWorkerAsync();
                                setProgressStatus("Installing SQL Compliance Manager...");
                                if (!InstallSQLCM(fileName))
                                {
                                    installationFailure = true;
                                    Constants.FailedProduct = Products.Compliance;
                                    return;
                                }
                                else
                                {
                                    //CM installation was success
                                    UpdateRevisionLogs(Constants.ProductMap[Products.Compliance], Constants.InstalledCMVersion, Application.ProductVersion);

                                    StartRegistrationService();
                                    CopyClusterExeToInstallDir();

                                    if (InstallProperties.RegisterProductToDashboard)
                                    {
                                        if (backgroundWorkerProgress.IsBusy)
                                        {
                                            backgroundWorkerProgress.CancelAsync();
                                            Thread.Sleep(5000);
                                        }
                                        currentProgressLimit = 99;
                                        backgroundWorkerProgress.RunWorkerAsync();
                                        setProgressStatus("Registering SQL Compliance Manager with IDERA Dashboard...");
                                        if (!Host.CwfHelper.RegisterProduct())
                                        {
                                            installationFailure = true;
                                            Constants.FailedProduct = Products.Registration;
                                            return;
                                        }
                                    }
                                    if (backgroundWorkerProgress.IsBusy)
                                    {
                                        backgroundWorkerProgress.CancelAsync();
                                        Thread.Sleep(5000);
                                    }
                                }
                            }
                            else if (InstallProperties.IsActiveNode)
                            {
                                currentProgress = 0;
                                currentProgressLimit = 80;
                                progressSpeed = 2000;
                                backgroundWorkerProgress.RunWorkerAsync();
                                setProgressStatus("Installing SQL Compliance Manager...");
                                if (!InstallSQLCMActiveNode(fileName))
                                {
                                    installationFailure = true;
                                    Constants.FailedProduct = Products.Compliance;
                                    return;
                                }
                                else
                                {
                                    //CM installation was success
                                    UpdateRevisionLogs(Constants.ProductMap[Products.Compliance], Constants.InstalledCMVersion, Application.ProductVersion);

                                    StartRegistrationService();
                                    CopyClusterExeToInstallDir();
                                    if (InstallProperties.RegisterProductToDashboard)
                                    {
                                        if (backgroundWorkerProgress.IsBusy)
                                        {
                                            backgroundWorkerProgress.CancelAsync();
                                            Thread.Sleep(5000);
                                        }
                                        currentProgressLimit = 99;
                                        backgroundWorkerProgress.RunWorkerAsync();
                                        setProgressStatus("Registering SQL Compliance Manager with IDERA Dashboard...");
                                        if (!Host.CwfHelper.RegisterProduct())
                                        {
                                            installationFailure = true;
                                            Constants.FailedProduct = Products.Registration;
                                            return;
                                        }
                                    }
                                    if (backgroundWorkerProgress.IsBusy)
                                    {
                                        backgroundWorkerProgress.CancelAsync();
                                        Thread.Sleep(5000);
                                    }
                                }
                            }
                            else
                            {
                                currentProgress = 0;
                                currentProgressLimit = 80;
                                progressSpeed = 2000;
                                backgroundWorkerProgress.RunWorkerAsync();
                                setProgressStatus("Installing SQL Compliance Manager...");
                                if (!InstallSQLCMPassiveNode(fileName))
                                {
                                    installationFailure = true;
                                    Constants.FailedProduct = Products.Compliance;
                                    return;
                                }
                                else
                                {
                                    //CM installation was success
                                    UpdateRevisionLogs(Constants.ProductMap[Products.Compliance], Constants.InstalledCMVersion, Application.ProductVersion);

                                    StartRegistrationService();
                                    CopyClusterExeToInstallDir();
                                    if (InstallProperties.RegisterProductToDashboard)
                                    {
                                        if (backgroundWorkerProgress.IsBusy)
                                        {
                                            backgroundWorkerProgress.CancelAsync();
                                            Thread.Sleep(5000);
                                        }
                                        currentProgressLimit = 99;
                                        backgroundWorkerProgress.RunWorkerAsync();
                                        setProgressStatus("Registering SQL Compliance Manager with IDERA Dashboard...");
                                        if (!Host.CwfHelper.RegisterProduct())
                                        {
                                            installationFailure = true;
                                            Constants.FailedProduct = Products.Registration;
                                            return;
                                        }
                                    }
                                    if (backgroundWorkerProgress.IsBusy)
                                    {
                                        backgroundWorkerProgress.CancelAsync();
                                        Thread.Sleep(5000);
                                    }
                                }
                            }
                        }
                        else
                        {
                            currentProgress = 0;
                            currentProgressLimit = 70;
                            progressSpeed = 2000;
                            backgroundWorkerProgress.RunWorkerAsync();
                            setProgressStatus("Upgrading SQL Compliance Manager...");
                            if (!UpgradeSQLCM(fileName))
                            {
                                installationFailure = true;
                                Constants.FailedProduct = Products.Compliance;
                                return;
                            }
                            else
                            {
                                //CM upgrade was success
                                UpdateRevisionLogs(Constants.ProductMap[Products.Compliance], Constants.InstalledCMVersion, Application.ProductVersion);
                                StartAlertsService();

                                string currentCMVersion = "";
                                HelperFunctions helperFunctions = new HelperFunctions();
                                if (helperFunctions.ReadDataFromRegistry(RegistryHive.LocalMachine, Constants.CMRegistryPath, "Version", out currentCMVersion))
                                {
                                    if (currentCMVersion.Equals(Application.ProductVersion))
                                    {
                                        if (backgroundWorkerProgress.IsBusy)
                                        {
                                            backgroundWorkerProgress.CancelAsync();
                                            Thread.Sleep(5000);
                                        }
                                        currentProgressLimit = 99;
                                        backgroundWorkerProgress.RunWorkerAsync();
                                        StartRegistrationService();
                                        CopyClusterExeToInstallDir();
                                        setProgressStatus("Gathering required data for registration...");
                                        if (!GetUpgradeData())
                                        {
                                            installationFailure = true;
                                            Constants.FailedProduct = Products.Registration;
                                            return;
                                        }
                                        else
                                        {
                                            if (InstallProperties.RegisterProductToDashboard)
                                            {
                                                setProgressStatus("Registering SQL Compliance Manager with IDERA Dashboard...");
                                                if (!Host.CwfHelper.RegisterProduct())
                                                {
                                                    installationFailure = true;
                                                    Constants.FailedProduct = Products.Registration;
                                                    return;
                                                }
                                            }
                                            if (backgroundWorkerProgress.IsBusy)
                                            {
                                                backgroundWorkerProgress.CancelAsync();
                                                Thread.Sleep(5000);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        installationFailure = true;
                                        Constants.FailedProduct = Products.Compliance;
                                        return;
                                    }
                                }
                                else
                                {
                                    installationFailure = true;
                                    Constants.FailedProduct = Products.Compliance;
                                    return;
                                }
                            }
                        }
                        break;
                    }
                #endregion

                #region ConsoleOnly
                case InstallType.ConsoleOnly:
                    {
                        if (!InstallNativeClient())
                        {                           
                            IsNativeClientInstalled = false;
                            installationFailure = true;
                            Constants.FailedProduct = Products.Console;
                            return;
                        }

                        if (!InstallProperties.IsUpgradeRadioSelection)
                        {
                            currentProgress = 0;
                            currentProgressLimit = 99;
                            progressSpeed = 450;
                            backgroundWorkerProgress.RunWorkerAsync();
                            setProgressStatus("Installing SQL Compliance Manager Console...");
                            if (!InstallSQLCMConsole(fileName))
                            {
                                installationFailure = true;
                                Constants.FailedProduct = Products.Console;
                                return;
                            }
                            else
                            {
                                if (backgroundWorkerProgress.IsBusy)
                                {
                                    backgroundWorkerProgress.CancelAsync();
                                    Thread.Sleep(5000);
                                }
                            }
                        }
                        else
                        {
                            currentProgress = 0;
                            currentProgressLimit = 99;
                            progressSpeed = 450;
                            backgroundWorkerProgress.RunWorkerAsync();
                            setProgressStatus("Upgrading SQL Compliance Manager Console...");
                            if (!UpgradeCMAgentOrConsole(fileName))
                            {
                                installationFailure = true;
                                Constants.FailedProduct = Products.Console;
                                return;
                            }
                            else
                            {
                                if (backgroundWorkerProgress.IsBusy)
                                {
                                    backgroundWorkerProgress.CancelAsync();
                                    Thread.Sleep(5000);
                                }
                            }
                        }
                        break;
                    }
                #endregion

                #region AgentOnly

                case InstallType.AgentOnly:
                    {
                        LogHelper.SetLogger(InstallProperties.CMInstallDir + Constants.SQLAgenRevisionLogFile);
                        if (!InstallProperties.IsUpgradeRadioSelection)
                        {
                            currentProgress = 0;
                            currentProgressLimit = 99;
                            progressSpeed = 450;
                            backgroundWorkerProgress.RunWorkerAsync();
                            setProgressStatus("Installing SQL Compliance Manager Agent...");
                            if (!InstallSQLCMAgent(fileName))
                            {
                                installationFailure = true;
                                Constants.FailedProduct = Products.Agent;
                                return;
                            }
                            else
                            {
                                //Agent installation was success
                                UpdateRevisionLogs(Constants.ProductMap[Products.Agent], Constants.InstalledCMVersion, Application.ProductVersion);

                                if (backgroundWorkerProgress.IsBusy)
                                {
                                    backgroundWorkerProgress.CancelAsync();
                                    Thread.Sleep(5000);
                                }
                            }
                        }
                        else
                        {
                            currentProgress = 0;
                            currentProgressLimit = 99;
                            progressSpeed = 450;
                            backgroundWorkerProgress.RunWorkerAsync();
                            setProgressStatus("Upgrading SQL Compliance Manager Agent...");
                            if (!UpgradeCMAgentOrConsole(fileName))
                            {
                                installationFailure = true;
                                Constants.FailedProduct = Products.Agent;
                                return;
                            }
                            else
                            {
                                //Agent upgrade was success
                                UpdateRevisionLogs(Constants.ProductMap[Products.Agent], Constants.InstalledCMVersion, Application.ProductVersion);

                                if (backgroundWorkerProgress.IsBusy)
                                {
                                    backgroundWorkerProgress.CancelAsync();
                                    Thread.Sleep(5000);
                                }
                            }
                        }
                        break;
                    }
                #endregion

                #region CMAndDashboard

                case InstallType.CMAndDashboard:
                    {
                        if (!InstallNativeClient())
                        {                           
                            IsNativeClientInstalled = false;
                            installationFailure = true;
                            Constants.FailedProduct = Products.Compliance;
                            return;
                        }

                        LogHelper.SetLogger(InstallProperties.CMInstallDir + Constants.SQLCMRevisionLogFile);
                        if (!InstallProperties.IsUpgradeRadioSelection)
                        {
                            if (!InstallProperties.Clustered)
                            {
                                progressSpeed = 2000;
                                currentProgressLimit = 40;
                                setProgressStatus("Installing SQL Compliance Manager...");
                                backgroundWorkerProgress.RunWorkerAsync();
                                if (!InstallSQLCM(fileName))
                                {
                                    installationFailure = true;
                                    Constants.FailedProduct = Products.Compliance;
                                    return;
                                }
                                else
                                {
                                    //CM installation was success
                                    UpdateRevisionLogs(Constants.ProductMap[Products.Compliance], Constants.InstalledCMVersion, Application.ProductVersion);

                                    CopyClusterExeToInstallDir();
                                    if (backgroundWorkerProgress.IsBusy)
                                    {
                                        backgroundWorkerProgress.CancelAsync();
                                        Thread.Sleep(5000);
                                    }
                                    setProgressStatus("Installing IDERA Dashboard...");
                                    currentProgressLimit = 80;
                                    backgroundWorkerProgress.RunWorkerAsync();
                                    StopRegistrationService();
                                    if (!InstallDashboard())
                                    {
                                        StartRegistrationService();
                                        installationFailure = true;
                                        Constants.FailedProduct = Products.Dashboard;
                                        return;
                                    }
                                    else
                                    {
                                        //dashboard installation was success
                                        LogHelper.SetLogger(InstallProperties.DashboardInstallDir + Constants.SQLDashboardRevisionLogFile);
                                        UpdateRevisionLogs(Constants.ProductMap[Products.Dashboard], Constants.InstalledCMVersion, Application.ProductVersion);

                                        if (backgroundWorkerProgress.IsBusy)
                                        {
                                            backgroundWorkerProgress.CancelAsync();
                                            Thread.Sleep(5000);
                                        }
                                        StartRegistrationService();
                                        setProgressStatus("Starting CWF Dashboard Services, Please wait...");
                                        Thread.Sleep(30000);
                                        setProgressStatus("Registering SQL Compliance Manager with IDERA Dashboard...");
                                        currentProgressLimit = 99;
                                        backgroundWorkerProgress.RunWorkerAsync();
                                        if (!Host.CwfHelper.RegisterProduct())
                                        {
                                            installationFailure = true;
                                            Constants.FailedProduct = Products.Registration;
                                            return;
                                        }
                                        if (backgroundWorkerProgress.IsBusy)
                                        {
                                            backgroundWorkerProgress.CancelAsync();
                                            Thread.Sleep(5000);
                                        }

                                    }
                                }
                            }
                            else if (InstallProperties.IsActiveNode)
                            {
                                progressSpeed = 2000;
                                currentProgress = 0;
                                currentProgressLimit = 40;
                                setProgressStatus("Installing SQL Compliance Manager...");
                                backgroundWorkerProgress.RunWorkerAsync();
                                if (!InstallSQLCMActiveNode(fileName))
                                {
                                    installationFailure = true;
                                    Constants.FailedProduct = Products.Compliance;
                                    return;
                                }
                                else
                                {
                                    //cm installation was success
                                    UpdateRevisionLogs(Constants.ProductMap[Products.Compliance], Constants.InstalledCMVersion, Application.ProductVersion);

                                    CopyClusterExeToInstallDir();
                                    if (backgroundWorkerProgress.IsBusy)
                                    {
                                        backgroundWorkerProgress.CancelAsync();
                                        Thread.Sleep(5000);
                                    }
                                    setProgressStatus("Installing IDERA Dashboard...");
                                    currentProgressLimit = 80;
                                    backgroundWorkerProgress.RunWorkerAsync();
                                    StopRegistrationService();
                                    if (!InstallDashboard())
                                    {
                                        StartRegistrationService();
                                        installationFailure = true;
                                        Constants.FailedProduct = Products.Dashboard;
                                        return;
                                    }
                                    else
                                    {
                                        //dashboard installation was success
                                        LogHelper.SetLogger(InstallProperties.DashboardInstallDir + Constants.SQLDashboardRevisionLogFile);
                                        UpdateRevisionLogs(Constants.ProductMap[Products.Dashboard], Constants.InstalledCMVersion, Application.ProductVersion);

                                        StartRegistrationService();
                                        if (backgroundWorkerProgress.IsBusy)
                                        {
                                            backgroundWorkerProgress.CancelAsync();
                                            Thread.Sleep(5000);
                                        }
                                        setProgressStatus("Starting CWF Dashboard Services, Please wait...");
                                        Thread.Sleep(30000);
                                        setProgressStatus("Registering SQL Compliance Manager with IDERA Dashboard...");
                                        currentProgressLimit = 99;
                                        backgroundWorkerProgress.RunWorkerAsync();
                                        if (!Host.CwfHelper.RegisterProduct())
                                        {
                                            installationFailure = true;
                                            Constants.FailedProduct = Products.Registration;
                                            return;
                                        }
                                        if (backgroundWorkerProgress.IsBusy)
                                        {
                                            backgroundWorkerProgress.CancelAsync();
                                            Thread.Sleep(5000);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                progressSpeed = 2000;
                                currentProgress = 0;
                                currentProgressLimit = 40;
                                setProgressStatus("Installing SQL Compliance Manager...");
                                backgroundWorkerProgress.RunWorkerAsync();
                                if (!InstallSQLCMPassiveNode(fileName))
                                {
                                    installationFailure = true;
                                    Constants.FailedProduct = Products.Compliance;
                                    return;
                                }
                                else
                                {
                                    //cm installation was success
                                    UpdateRevisionLogs(Constants.ProductMap[Products.Compliance], Constants.InstalledCMVersion, Application.ProductVersion);

                                    CopyClusterExeToInstallDir();
                                    if (backgroundWorkerProgress.IsBusy)
                                    {
                                        backgroundWorkerProgress.CancelAsync();
                                        Thread.Sleep(5000);
                                    }
                                    setProgressStatus("Installing IDERA Dashboard...");
                                    currentProgressLimit = 80;
                                    backgroundWorkerProgress.RunWorkerAsync();
                                    StopRegistrationService();
                                    if (!InstallDashboard())
                                    {
                                        StartRegistrationService();
                                        installationFailure = true;
                                        Constants.FailedProduct = Products.Dashboard;
                                        return;
                                    }
                                    else
                                    {
                                        //dashboard installation was success
                                        LogHelper.SetLogger(InstallProperties.DashboardInstallDir + Constants.SQLDashboardRevisionLogFile);
                                        UpdateRevisionLogs(Constants.ProductMap[Products.Dashboard], Constants.InstalledCMVersion, Application.ProductVersion);

                                        StartRegistrationService();
                                        if (backgroundWorkerProgress.IsBusy)
                                        {
                                            backgroundWorkerProgress.CancelAsync();
                                            Thread.Sleep(5000);
                                        }
                                        setProgressStatus("Starting CWF Dashboard Services, Please wait...");
                                        Thread.Sleep(30000);
                                        setProgressStatus("Registering SQL Compliance Manager with IDERA Dashboard...");
                                        currentProgressLimit = 99;
                                        backgroundWorkerProgress.RunWorkerAsync();
                                        if (!Host.CwfHelper.RegisterProduct())
                                        {
                                            installationFailure = true;
                                            Constants.FailedProduct = Products.Registration;
                                            return;
                                        }
                                        if (backgroundWorkerProgress.IsBusy)
                                        {
                                            backgroundWorkerProgress.CancelAsync();
                                            Thread.Sleep(5000);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            progressSpeed = 2000;
                            currentProgress = 0;
                            currentProgressLimit = 40;
                            backgroundWorkerProgress.RunWorkerAsync();
                            setProgressStatus("Upgrading SQL Compliance Manager...");
                            if (!UpgradeSQLCM(fileName))
                            {
                                installationFailure = true;
                                Constants.FailedProduct = Products.Compliance;
                                return;
                            }
                            else
                            {
                                //cm upgrade was success
                                UpdateRevisionLogs(Constants.ProductMap[Products.Compliance], Constants.InstalledCMVersion, Application.ProductVersion);
                                StartAlertsService();

                                CopyClusterExeToInstallDir();
                                string currentCMVersion = "";
                                HelperFunctions helperFunctions = new HelperFunctions();
                                if (helperFunctions.ReadDataFromRegistry(RegistryHive.LocalMachine, Constants.CMRegistryPath, "Version", out currentCMVersion))
                                {
                                    if (currentCMVersion.Equals(Application.ProductVersion))
                                    {
                                        if (backgroundWorkerProgress.IsBusy)
                                        {
                                            backgroundWorkerProgress.CancelAsync();
                                            Thread.Sleep(5000);
                                        }
                                        currentProgressLimit = 80;
                                        backgroundWorkerProgress.RunWorkerAsync();
                                        setProgressStatus("Upgrading IDERA Dashboard...");
                                        StopRegistrationService();
                                        if (!InstallDashboard())
                                        {
                                            StartRegistrationService();
                                            installationFailure = true;
                                            Constants.FailedProduct = Products.Dashboard;
                                            return;
                                        }
                                        else
                                        {
                                            //dashboard installation was success
                                            LogHelper.SetLogger(InstallProperties.DashboardInstallDir + Constants.SQLDashboardRevisionLogFile);
                                            UpdateRevisionLogs(Constants.ProductMap[Products.Dashboard], Constants.InstalledCMVersion, Application.ProductVersion);

                                            StartRegistrationService();
                                            if (backgroundWorkerProgress.IsBusy)
                                            {
                                                backgroundWorkerProgress.CancelAsync();
                                                Thread.Sleep(5000);
                                            }
                                            setProgressStatus("Starting CWF Dashboard Services, Please wait...");
                                            Thread.Sleep(30000);
                                            currentProgressLimit = 99;
                                            backgroundWorkerProgress.RunWorkerAsync();
                                            setProgressStatus("Gathering required data for registration...");
                                            if (!GetUpgradeData())
                                            {
                                                installationFailure = true;
                                                Constants.FailedProduct = Products.Registration;
                                                return;
                                            }
                                            else
                                            {
                                                if (InstallProperties.RegisterProductToDashboard)
                                                {
                                                    setProgressStatus("Registering SQL Compliance Manager with IDERA Dashboard...");
                                                    if (!Host.CwfHelper.RegisterProduct())
                                                    {
                                                        installationFailure = true;
                                                        Constants.FailedProduct = Products.Registration;
                                                        return;
                                                    }
                                                    if (backgroundWorkerProgress.IsBusy)
                                                    {
                                                        backgroundWorkerProgress.CancelAsync();
                                                        Thread.Sleep(5000);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        installationFailure = true;
                                        Constants.FailedProduct = Products.Compliance;
                                        return;
                                    }
                                }
                                else
                                {
                                    installationFailure = true;
                                    Constants.FailedProduct = Products.Compliance;
                                    return;
                                }
                            }
                        }
                        break;
                    }

                #endregion

                #region ConsoleAndDashboard

                case InstallType.ConsoleAndDashboard:
                    {
                        if (!InstallNativeClient())
                        {
                            IsNativeClientInstalled = false;
                            installationFailure = true;
                            Constants.FailedProduct = Products.Console;
                            return;
                        }

                        if (!InstallProperties.IsUpgradeRadioSelection)
                        {
                            currentProgress = 0;
                            currentProgressLimit = 40;
                            progressSpeed = 450;
                            backgroundWorkerProgress.RunWorkerAsync();
                            setProgressStatus("Installing SQL Compliance Manager Console...");
                            if (!InstallSQLCMConsole(fileName))
                            {
                                installationFailure = true;
                                Constants.FailedProduct = Products.Console;
                                return;
                            }
                            else
                            {
                                if (backgroundWorkerProgress.IsBusy)
                                {
                                    backgroundWorkerProgress.CancelAsync();
                                    Thread.Sleep(5000);
                                }
                                currentProgressLimit = 99;
                                progressSpeed = 1000;
                                backgroundWorkerProgress.RunWorkerAsync();
                                setProgressStatus("Installing IDERA Dashboard...");
                                if (!InstallDashboard())
                                {
                                    installationFailure = true;
                                    Constants.FailedProduct = Products.Dashboard;
                                    return;
                                }
                                else
                                {
                                    if (backgroundWorkerProgress.IsBusy)
                                    {
                                        backgroundWorkerProgress.CancelAsync();
                                        Thread.Sleep(5000);
                                    }
                                }
                            }
                        }
                        else
                        {
                            currentProgress = 0;
                            currentProgressLimit = 40;
                            progressSpeed = 450;
                            backgroundWorkerProgress.RunWorkerAsync();
                            setProgressStatus("Upgrading SQL Compliance Manager Console...");
                            if (!UpgradeCMAgentOrConsole(fileName))
                            {
                                installationFailure = true;
                                Constants.FailedProduct = Products.Console;
                                return;
                            }
                            else
                            {
                                string currentCMVersion = "";
                                HelperFunctions helperFunctions = new HelperFunctions();
                                if (helperFunctions.ReadDataFromRegistry(RegistryHive.LocalMachine, Constants.CMRegistryPath, "Version", out currentCMVersion))
                                {
                                    if (currentCMVersion.Equals(Application.ProductVersion))
                                    {
                                        if (backgroundWorkerProgress.IsBusy)
                                        {
                                            backgroundWorkerProgress.CancelAsync();
                                            Thread.Sleep(5000);
                                        }
                                        currentProgressLimit = 99;
                                        progressSpeed = 1000;
                                        backgroundWorkerProgress.RunWorkerAsync();
                                        setProgressStatus("Upgrading IDERA Dashboard...");
                                        if (!InstallDashboard())
                                        {
                                            installationFailure = true;
                                            Constants.FailedProduct = Products.Dashboard;
                                            return;
                                        }
                                        else
                                        {
                                            if (backgroundWorkerProgress.IsBusy)
                                            {
                                                backgroundWorkerProgress.CancelAsync();
                                                Thread.Sleep(5000);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        installationFailure = true;
                                        Constants.FailedProduct = Products.Compliance;
                                        return;
                                    }
                                }
                                else
                                {
                                    installationFailure = true;
                                    Constants.FailedProduct = Products.Compliance;
                                    return;
                                }
                            }
                        }
                        break;
                    }

                #endregion

                #region AgentAndDashboard
                case InstallType.AgentAndDashboard:
                    {
                        LogHelper.SetLogger(InstallProperties.CMInstallDir + Constants.SQLAgenRevisionLogFile);
                        if (!InstallProperties.IsUpgradeRadioSelection)
                        {
                            currentProgress = 0;
                            currentProgressLimit = 40;
                            progressSpeed = 450;
                            backgroundWorkerProgress.RunWorkerAsync();
                            setProgressStatus("Installing SQL Compliance Manager Agent...");
                            if (!InstallSQLCMAgent(fileName))
                            {
                                installationFailure = true;
                                Constants.FailedProduct = Products.Agent;
                                return;
                            }
                            else
                            {
                                //Agent installation was success
                                UpdateRevisionLogs(Constants.ProductMap[Products.Agent], Constants.InstalledCMVersion, Application.ProductVersion);

                                string currentCMVersion = "";
                                HelperFunctions helperFunctions = new HelperFunctions();
                                if (helperFunctions.ReadDataFromRegistry(RegistryHive.LocalMachine, Constants.CMRegistryPath, "Version", out currentCMVersion))
                                {
                                    if (currentCMVersion.Equals(Application.ProductVersion))
                                    {
                                        if (backgroundWorkerProgress.IsBusy)
                                        {
                                            backgroundWorkerProgress.CancelAsync();
                                            Thread.Sleep(5000);
                                        }
                                        currentProgressLimit = 99;
                                        progressSpeed = 1000;
                                        backgroundWorkerProgress.RunWorkerAsync();
                                        setProgressStatus("Installing IDERA Dashboard...");
                                        if (!InstallDashboard())
                                        {
                                            installationFailure = true;
                                            Constants.FailedProduct = Products.Dashboard;
                                            return;
                                        }
                                        else
                                        {
                                            //dashboard installation was success
                                            LogHelper.SetLogger(InstallProperties.DashboardInstallDir + Constants.SQLDashboardRevisionLogFile);
                                            UpdateRevisionLogs(Constants.ProductMap[Products.Dashboard], Constants.InstalledCMVersion, Application.ProductVersion);

                                            if (backgroundWorkerProgress.IsBusy)
                                            {
                                                backgroundWorkerProgress.CancelAsync();
                                                Thread.Sleep(5000);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        installationFailure = true;
                                        Constants.FailedProduct = Products.Compliance;
                                        return;
                                    }
                                }
                                else
                                {
                                    installationFailure = true;
                                    Constants.FailedProduct = Products.Compliance;
                                    return;
                                }
                            }
                        }
                        else
                        {
                            currentProgress = 0;
                            currentProgressLimit = 40;
                            progressSpeed = 450;
                            backgroundWorkerProgress.RunWorkerAsync();
                            setProgressStatus("Upgrading SQL Compliance Manager Agent...");
                            if (!UpgradeCMAgentOrConsole(fileName))
                            {
                                installationFailure = true;
                                Constants.FailedProduct = Products.Agent;
                                return;
                            }
                            else
                            {
                                //Agent upgrade was success
                                UpdateRevisionLogs(Constants.ProductMap[Products.Agent], Constants.InstalledCMVersion, Application.ProductVersion);

                                if (backgroundWorkerProgress.IsBusy)
                                {
                                    backgroundWorkerProgress.CancelAsync();
                                    Thread.Sleep(5000);
                                }
                                currentProgressLimit = 99;
                                progressSpeed = 1000;
                                backgroundWorkerProgress.RunWorkerAsync();
                                setProgressStatus("Upgrading IDERA Dashboard...");
                                if (!InstallDashboard())
                                {
                                    installationFailure = true;
                                    Constants.FailedProduct = Products.Dashboard;
                                    return;
                                }
                                else
                                {
                                    //dashboard upgrade was success
                                    LogHelper.SetLogger(InstallProperties.DashboardInstallDir + Constants.SQLDashboardRevisionLogFile);
                                    UpdateRevisionLogs(Constants.ProductMap[Products.Dashboard], Constants.InstalledCMVersion, Application.ProductVersion);

                                    if (backgroundWorkerProgress.IsBusy)
                                    {
                                        backgroundWorkerProgress.CancelAsync();
                                        Thread.Sleep(5000);
                                    }
                                }
                            }
                        }
                        break;
                    }
                    #endregion
            }
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBarAll.MarqueeAnimationSpeed = 30;
            progressBarAll.Value = e.ProgressPercentage;

            //Progress bar progress in percentage stopped
            //progressTracker.Text = e.ProgressPercentage + "%";
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string failureMessage = string.Empty;

            if (backgroundWorkerProgress.IsBusy)
            {
                backgroundWorkerProgress.CancelAsync();
                Thread.Sleep(1000);
            }

            if (e != null && e.Error != null)
            {
                Constants.FailedProduct = Products.Compliance;
                installationFailure = true;
                failureMessage = e.Error.Message;
            }
            if (Constants.UserInstallType != InstallType.DashboardOnly && Constants.FailedProduct == Products.NA)
            {
                HelperFunctions helperFunction = new HelperFunctions();
                string currentVersion = string.Empty;
                if (!(helperFunction.ReadDataFromRegistry(RegistryHive.LocalMachine, Constants.CMRegistryPath, "Version", out currentVersion) && currentVersion == Constants.ProductVersion))
                {
                    Constants.FailedProduct = Products.Compliance;
                    installationFailure = true;
                }
            }

            if (installationFailure)
            {
                if (!IsNativeClientInstalled)
                {
                     failureMessage = "The Windows Server version you are currently running is no longer supported. By clicking OK the installation will exit";
                }
                if (string.IsNullOrEmpty(failureMessage))
                {
                    if (Constants.FailedProduct == Products.Registration)
                    {
                        failureMessage = Constants.RegistrationFailureMessage;
                    }
                    else
                    {
                        if (installerReturnCode != 0 && !Constants.ReturnCodes.TryGetValue(installerReturnCode, out failureMessage))
                        {
                            failureMessage = "An error occurred during installation.";
                        }
                    }
                }
                Host.Invoke(new MethodInvoker(delegate
                {
                    Host.NavigateToErrorPage(failureMessage);
                }));
            }
            else
            {
                GenerateSQLCMFirewallConfigurationInformation();
                Host.Invoke(new MethodInvoker(delegate
                {
                    Host.NavigateToPage(NavigationDirection.Finish);
                }));
            }
        }

        private void backgroundWorkerProgress_DoWork(object sender, DoWorkEventArgs e)
        {
            while (currentProgress < currentProgressLimit)
            {
                if (backgroundWorkerProgress.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
                backgroundWorker.ReportProgress(currentProgress);
                currentProgress++;
                Thread.Sleep(progressSpeed);
            }
        }

        private void backgroundWorkerProgress_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBarAll.MarqueeAnimationSpeed = 30;
            progressBarAll.Value = e.ProgressPercentage;
            progressTracker.Text = e.ProgressPercentage + "%";
        }

        private void backgroundWorkerProgress_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            currentProgress = currentProgressLimit;
        }

        private bool InstallSQLCM(string _fileName)
        {
            string curFile = @"installLogs_SQLCM.log";
            if (File.Exists(curFile))
                File.Delete(curFile);

            string productArgs = " /v\"/l*v \\";
            productArgs += "\"installLogs_SQLCM.log\\\" ";

            SQLCMArguments arg = new SQLCMArguments();

            arg.AgreeToLicense = "yes";
            arg.InstallDir = "\"\"" + InstallProperties.CMInstallDir + "\"\"";
            arg.TraceDirCollect = "\"\"" + InstallProperties.CollectionTraceDirectory + "\"\"";
            // SQLCM-5412 User name and password are displaying as plain text in the command
            arg.Encrypted = "1";
            arg.ServiceUsername = "\"\"" + HelperFunctions.EncryptString(InstallProperties.ServiceUserName) + "\"\"";
            arg.ServicePassword = "\"\"" + HelperFunctions.EncryptString(InstallProperties.ServicePassword) + "\"\"";
            arg.Repository = "\"\"" + InstallProperties.CMSQLServerInstanceName + "\"\"";
            arg.TraceDirAgent = "\"\"" + InstallProperties.AgentTraceDirectory + "\"\"";
            arg.CwfUrl = "\"\"" + Host.CwfHelper.CwfUrl + "\"\"";
            arg.CwfToken = "\"\"" + Host.CwfHelper.CwfToken + "\"\"";
            arg.SetupType = "ALL";

            if (InstallProperties.IsCMSQLServerSQLAuth)
            {
                arg.SQLServerAuthentication = "1";
                arg.SQLServerUsername = "\"\"" + InstallProperties.CMSQLServerUsername + "\"\"";
                arg.SQLServerPassword = "\"\"" + InstallProperties.CMSQLServerPassword + "\"\"";
            }

            if (InstallProperties.UpgradeSchema)
            {
                arg.UseExistingDatabase = "1";
                arg.UpgradeSchema = "1";
                arg.DatabaseExists = "TRUE";
            }
            productArgs = productArgs + arg.ToString() + " /quiet\"";
            string filename = _fileName;
            HelperFunctions helperFunctions = new HelperFunctions();
            if (executeExe(productArgs, filename))
            {
                string errorMessage = "";
                helperFunctions.SetFolderAccess(false, InstallProperties.AgentTraceDirectory, Constants.AgentTrace, out errorMessage);
                helperFunctions.SetFolderAccess(false, InstallProperties.CollectionTraceDirectory, Constants.CollectionTrace, out errorMessage);
                helperFunctions.SetFolderAccess(false, InstallProperties.CMInstallDir, Constants.InstallationDir, out errorMessage);

                if (errorMessage != string.Empty)
                {
                    errorMessage = Constants.FolderAccessErrorMessage + errorMessage;
                    IderaMessageBox messageBox = new IderaMessageBox();
                    messageBox.Show(errorMessage);
                    
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool InstallSQLCMConsole(string _fileName)
        {
            SetPRQRegistry();
            string curFile = @"installLogs_SQLCM_Console.log";
            if (File.Exists(curFile))
                File.Delete(curFile);

            string productArgs = " /v\"/l*v \\";
            productArgs += "\"installLogs_SQLCM_Console.log\\\" ";

            SQLCMArguments arg = new SQLCMArguments();

            arg.AgreeToLicense = "yes";
            arg.InstallDir = "\"\"" + InstallProperties.CMInstallDir + "\"\"";
            arg.SetupType = "CONSOLE";

            productArgs = productArgs + arg.ToString() + " /quiet\"";
            string filename = _fileName;
            HelperFunctions helperFunctions = new HelperFunctions();
            try
            {
                if (executeExe(productArgs, filename))
                {
                    string errorMessage = "";
                    helperFunctions.SetFolderAccess(false, InstallProperties.CMInstallDir, Constants.InstallationDir, out errorMessage);

                    if (errorMessage != string.Empty)
                    {
                        errorMessage = Constants.FolderAccessErrorMessage + errorMessage;
                        IderaMessageBox messageBox = new IderaMessageBox();
                        messageBox.Show(errorMessage);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
            finally
            {
                RemovePRQRegistry();
            }
        }

        private bool InstallSQLCMAgent(string _fileName)
        {
            SetPRQRegistry();
            string curFile = @"installLogs_SQLCM_Agent.log";
            if (File.Exists(curFile))
                File.Delete(curFile);

            string productArgs = " /v\"/l*v \\";
            productArgs += "\"installLogs_SQLCM_Agent.log\\\" ";

            SQLCMArguments arg = new SQLCMArguments();

            arg.AgreeToLicense = "yes";
            arg.InstallDir = "\"\"" + InstallProperties.CMInstallDir + "\"\"";
            arg.TraceDirAgent = "\"\"" + InstallProperties.AgentTraceDirectory + "\"\"";
            // SQLCM-5412 User name and password are displaying as plain text in the command
            arg.Encrypted = "1";
            arg.ServiceUsername = "\"\"" + HelperFunctions.EncryptString(InstallProperties.ServiceUserName) + "\"\"";
            arg.ServicePassword = "\"\"" + HelperFunctions.EncryptString(InstallProperties.ServicePassword) + "\"\"";
            arg.CollectionServer = "\"\"" + InstallProperties.AgentCollectionServer + "\"\"";
            arg.Instance = "\"\"" + InstallProperties.CMSQLServerInstanceName + "\"\"";
            arg.SetupType = "AGENT";

            if (InstallProperties.IsCMSQLServerSQLAuth)
            {
                arg.SQLServerAuthentication = "1";
                arg.SQLServerUsername = "\"\"" + InstallProperties.CMSQLServerUsername + "\"\"";
                arg.SQLServerPassword = "\"\"" + InstallProperties.CMSQLServerPassword + "\"\"";
            }

            productArgs = productArgs + arg.ToString() + " /quiet\"";
            string filename = _fileName;
            HelperFunctions helperFunctions = new HelperFunctions();
            try
            {
                if (executeExe(productArgs, filename))
                {
                    string errorMessage = "";
                    helperFunctions.SetFolderAccess(false, InstallProperties.CMInstallDir, Constants.InstallationDir, out errorMessage);
                    helperFunctions.SetFolderAccess(false, InstallProperties.AgentTraceDirectory, Constants.AgentTrace, out errorMessage);

                    if (errorMessage != string.Empty)
                    {
                        errorMessage = Constants.FolderAccessErrorMessage + errorMessage;
                        IderaMessageBox messageBox = new IderaMessageBox();
                        messageBox.Show(errorMessage);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
            finally
            {
                RemovePRQRegistry();
            }
        }

        private bool InstallSQLCMActiveNode(string _fileName)
        {
            string curFile = @"installLogs_SQLCM_Agent.log";
            if (File.Exists(curFile))
                File.Delete(curFile);

            string productArgs = " /v\"/l*v \\";
            productArgs += "\"installLogs_SQLCM.log\\\" ";

            SQLCMArguments arg = new SQLCMArguments();

            arg.AgreeToLicense = "yes";
            arg.InstallDir = "\"\"" + InstallProperties.CMInstallDir + "\"\"";
            arg.TraceDirCollect = "\"\"" + InstallProperties.CollectionTraceDirectory + "\"\"";
            // SQLCM-5412 User name and password are displaying as plain text in the command
            arg.Encrypted = "1";
            arg.ServiceUsername = "\"\"" + HelperFunctions.EncryptString(InstallProperties.ServiceUserName) + "\"\"";
            arg.ServicePassword = "\"\"" + HelperFunctions.EncryptString(InstallProperties.ServicePassword) + "\"\"";
            arg.Repository = "\"\"" + InstallProperties.CMSQLServerInstanceName + "\"\"";
            arg.Clustered = "1";
            arg.Node = "1";
            arg.SetupType = "CLUSTERED";

            if (InstallProperties.IsCMSQLServerSQLAuth)
            {
                arg.SQLServerAuthentication = "1";
                arg.SQLServerUsername = "\"\"" + InstallProperties.CMSQLServerUsername + "\"\"";
                arg.SQLServerPassword = "\"\"" + InstallProperties.CMSQLServerPassword + "\"\"";
            }

            if (InstallProperties.UpgradeSchema)
            {
                arg.UseExistingDatabase = "1";
                arg.UpgradeSchema = "1";
                arg.DatabaseExists = "TRUE";
            }
            productArgs = productArgs + arg.ToString() + " /quiet\"";
            string filename = _fileName;
            HelperFunctions helperFunctions = new HelperFunctions();
            if (executeExe(productArgs, filename))
            {
                string errorMessage = "";
                helperFunctions.SetFolderAccess(false, InstallProperties.CollectionTraceDirectory, Constants.CollectionTrace, out errorMessage);
                helperFunctions.SetFolderAccess(false, InstallProperties.CMInstallDir, Constants.InstallationDir, out errorMessage);

                if (errorMessage != string.Empty)
                {
                    errorMessage = Constants.FolderAccessErrorMessage + errorMessage;
                    IderaMessageBox messageBox = new IderaMessageBox();
                    messageBox.Show(errorMessage);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool InstallSQLCMPassiveNode(string _fileName)
        {
            string curFile = @"installLogs_SQLCM_Agent.log";
            if (File.Exists(curFile))
                File.Delete(curFile);

            string productArgs = " /v\"/l*v \\";
            productArgs += "\"installLogs_SQLCM.log\\\" ";

            SQLCMArguments arg = new SQLCMArguments();

            arg.AgreeToLicense = "yes";
            arg.InstallDir = "\"\"" + InstallProperties.CMInstallDir + "\"\"";
            arg.TraceDirCollect = "\"\"" + InstallProperties.CMInstallDir + "\\" + "CollectionServerTraceFiles" + "\"\"";
            // SQLCM-5412 User name and password are displaying as plain text in the command
            arg.Encrypted = "1";
            arg.ServiceUsername = "\"\"" + HelperFunctions.EncryptString(InstallProperties.ServiceUserName) + "\"\"";
            arg.ServicePassword = "\"\"" + HelperFunctions.EncryptString(InstallProperties.ServicePassword) + "\"\"";
            arg.Repository = "\"\"" + InstallProperties.CMSQLServerInstanceName + "\"\"";
            arg.Clustered = "1";
            arg.Node = "2";
            arg.SetupType = "CLUSTERED";

            if (InstallProperties.IsCMSQLServerSQLAuth)
            {
                arg.SQLServerAuthentication = "1";
                arg.SQLServerUsername = "\"\"" + InstallProperties.CMSQLServerUsername + "\"\"";
                arg.SQLServerPassword = "\"\"" + InstallProperties.CMSQLServerPassword + "\"\"";
            }

            productArgs = productArgs + arg.ToString() + " /quiet\"";
            string filename = _fileName;
            HelperFunctions helperFunctions = new HelperFunctions();
            if (executeExe(productArgs, filename))
            {
                string errorMessage = "";
                InstallProperties.CollectionTraceDirectory = InstallProperties.CMInstallDir + "\\" + "CollectionServerTraceFiles";
                helperFunctions.SetFolderAccess(false, InstallProperties.CollectionTraceDirectory, Constants.CollectionTrace, out errorMessage);
                helperFunctions.SetFolderAccess(false, InstallProperties.CMInstallDir, Constants.InstallationDir, out errorMessage);

                if (errorMessage != string.Empty)
                {
                    errorMessage = Constants.FolderAccessErrorMessage + errorMessage;
                    IderaMessageBox messageBox = new IderaMessageBox();
                    messageBox.Show(errorMessage);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool UpgradeCMAgentOrConsole(string _fileName)
        {
            SetPRQRegistry();
            string curFile = @"installLogs_SQLCM_Agent.log";
            if (File.Exists(curFile))
                File.Delete(curFile);

            string productArgs = " /v\"/l*v \\";
            productArgs += "\"installLogs_SQLCM.log\\\" ";

            SQLCMArguments arg = new SQLCMArguments();
            if (InstallProperties.IsMajorUpgrade)
            {
                arg.IsMajorUpgrade = "Yes";
                if (Constants.UserCurrentInstallation == InstallType.AgentOnly || Constants.UserCurrentInstallation == InstallType.AgentAndDashboard)
                {
                    // SQLCM-5412 User name and password are displaying as plain text in the command
                    arg.Encrypted = "1";
                    arg.ServiceUsername = "\"\"" + HelperFunctions.EncryptString(InstallProperties.ServiceUserName) + "\"\"";
                    arg.ServicePassword = "\"\"" + HelperFunctions.EncryptString(InstallProperties.ServicePassword) + "\"\"";
                }
            }
            else
            {
                arg.ReInstall = "ALL";
                arg.ReInstallMode = "vomus";
            }

            productArgs = productArgs + arg.ToString() + " /quiet\"";
            string filename = _fileName;

            if (Constants.UserInstallType == InstallType.AgentOnly || Constants.UserInstallType == InstallType.AgentAndDashboard)
            {
                GetAgentInstancesFromCMRegistry();
            }

            try
            {
                if (executeExe(productArgs, filename))
                {
                    if (Constants.UserInstallType == InstallType.AgentOnly || Constants.UserInstallType == InstallType.AgentAndDashboard)
                    {
                        SetAgentInstancesInCMRegistry();
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
            finally
            {
                RemovePRQRegistry();
            }
        }

        private bool UpgradeSQLCM(string _fileName)
        {
            string curFile = @"installLogs_SQLCM_Agent.log";
            if (File.Exists(curFile))
                File.Delete(curFile);

            string productArgs = " /v\"/l*v \\";
            productArgs += "\"installLogs_SQLCM.log\\\" ";

            SQLCMArguments arg = new SQLCMArguments();
            arg.UseExistingDatabase = "1";
            arg.UpgradeSchema = "1";

            if (InstallProperties.IsMajorUpgrade)
            {
                arg.IsMajorUpgrade = "Yes";
                arg.IsUpgrade = "1";
                arg.SchemaValidation = "2";
                // SQLCM-5412 User name and password are displaying as plain text in the command
                arg.Encrypted = "1";
                arg.ServiceUsername = "\"\"" + HelperFunctions.EncryptString(InstallProperties.ServiceUserName) + "\"\"";
                arg.ServicePassword = "\"\"" + HelperFunctions.EncryptString(InstallProperties.ServicePassword) + "\"\"";
            }
            else
            {
                arg.ReInstall = "ALL";
                arg.ReInstallMode = "vomus";
                arg.Encrypted = "1";
                arg.ServiceUsername = "\"\"" + HelperFunctions.EncryptString(InstallProperties.ServiceUserName) + "\"\"";
                arg.ServicePassword = "\"\"" + HelperFunctions.EncryptString(InstallProperties.ServicePassword) + "\"\"";
            }

            if (InstallProperties.IsCMSQLServerSQLAuth)
            {
                arg.SQLServerAuthentication = "1";
                arg.SQLServerUsername = "\"\"" + InstallProperties.CMSQLServerUsername + "\"\"";
                arg.SQLServerPassword = "\"\"" + InstallProperties.CMSQLServerPassword + "\"\"";
            }

            productArgs = productArgs + arg.ToString() + " /quiet\"";
            string filename = _fileName;
            if (executeExe(productArgs, filename))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool InstallDashboard()
        {
            string curFile = @"installLogs_Dashboard.log";
            if (File.Exists(curFile))
                File.Delete(curFile);

            string productArgs = " /l*V installLogs_Dashboard.log /quiet ";
            DashboardArguments arg = new DashboardArguments();

            arg.InstallDir = InstallProperties.DashboardInstallDir;
            arg.AgreeToLicence = "yes";
            arg.ServiceAccount = InstallProperties.ServiceUserName;
            arg.ServicePassword = InstallProperties.ServicePassword;

            if (!InstallProperties.IsUpgradeRadioSelection)
            {
                arg.RepositoryCoreDatabase = "IderaDashboardRepository";
                arg.RepositoryInstance = InstallProperties.DashboardSQLServerInstanceName;
                arg.WebAppPort = "9290";
                arg.WebAppMonitorPort = "9094";
                arg.WebAppSSLPort = "9291";
                arg.CollectionPort = "9292";

                if (InstallProperties.IsDashboardSQLServerSQLAuth)
                {
                    arg.RepositorySQLAUTH = "1";
                    arg.RepositorySQLUSERNAME = InstallProperties.DashboardSQLServerUsername;
                    arg.RepositorySQLPASSWORD = InstallProperties.DashboardSQLServerPassword;

                }
            }

            productArgs = " /i \"IderaDashboard.msi\"" + productArgs + arg.ToString();
            if (installNewMSI(productArgs))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool GetUpgradeData()
        {
            // Get CWF Details from registry
            HelperFunctions helperFunction = new HelperFunctions();
            string registryValue = string.Empty;

            string cwfURL = string.Empty;
            string cwfToken = string.Empty;
            Version installedCMVersion;

            Version.TryParse(Constants.InstalledCMVersion, out installedCMVersion);

            try
            {
                if (installedCMVersion > Constants.CWFRegistrySupportedVersion)
                {
                    if (!helperFunction.ReadDataFromRegistry(RegistryHive.LocalMachine, Constants.CMRegistryPath, Constants.CWFURLRegKey, out cwfURL))
                    {
                        InstallProperties.RegisterProductToDashboard = false;
                        return true;
                    }
                    Host.CwfHelper.CwfUrl = cwfURL;

                    if (!helperFunction.ReadDataFromRegistry(RegistryHive.LocalMachine, Constants.CMRegistryPath, Constants.CWFTokenRegKey, out cwfToken))
                    {
                        InstallProperties.RegisterProductToDashboard = false;
                        return true;
                    }
                    // ToDo - Set Token
                    Host.CwfHelper.CwfToken = cwfToken;
                }
                else
                {
                    // Get CWF Details from Database
                    if (!helperFunction.GetCWFDetailsFromDatabase(InstallProperties.IsCMSQLServerSQLAuth, InstallProperties.CMSQLServerInstanceName, InstallProperties.CMSQLServerUsername, InstallProperties.CMSQLServerPassword, out cwfURL, out cwfToken, out registryValue))
                    {
                        InstallProperties.RegisterProductToDashboard = false;
                        return true;
                    }
                    Host.CwfHelper.CwfUrl = cwfURL;
                    // ToDo - Set Token
                    Host.CwfHelper.CwfToken = cwfToken;
                }

                helperFunction.ReadDataFromRegistry(RegistryHive.LocalMachine, Constants.CMRegistryPath, Constants.CWFDisplayNameRegKey, out registryValue);
                InstallProperties.CMDisplayName = registryValue;
                Host.CwfHelper.CwfProductInstance = InstallProperties.CMDisplayName;

                string repositoryInstance = string.Empty;
                // Make service call 
                var allRegistredSqlCmProducts = Host.CwfHelper.GetRegisteredSqlCmProducts();
                if (Constants.UserInstallType != InstallType.DashboardOnly && !InstallProperties.isCMCurrentVersionInstalled)
                {
                    repositoryInstance = InstallProperties.CMSQLServerInstanceName.ToUpper();
                }
                else
                {
                    helperFunction.ReadDataFromRegistry(RegistryHive.LocalMachine, Constants.CollectionRegistryPath, "ServerInstance", out repositoryInstance);
                    InstallProperties.CMSQLServerInstanceName = repositoryInstance;
                }
                repositoryInstance = repositoryInstance.Replace(".", Environment.MachineName).Replace("(LOCAL)", Environment.MachineName);

                bool foundInstance = false;
                foreach (var product in allRegistredSqlCmProducts)
                {
                    string location = product.Location.Split(';')[0].ToUpper();
                    location = location.Replace(".", Environment.MachineName).Replace("(LOCAL)", Environment.MachineName);
                    if (product.InstanceName.ToUpper().Equals(InstallProperties.CMDisplayName.ToUpper()))
                    {
                        var productVersion = new Version(product.Version);
                        if (location.StartsWith(repositoryInstance) && m_installerVersion > productVersion)
                        {
                            Host.CwfHelper.ProductsToBeUpgraded = new CMProducts();
                            Host.CwfHelper.ProductsToBeUpgraded.Add(product);
                            InstallProperties.RegisterProductToDashboard = true;
                            foundInstance = true;
                            break;
                        }
                    }
                }
                if (foundInstance)
                {
                    return true;
                }
                else
                {
                    foreach (var product in allRegistredSqlCmProducts)
                    {
                        InstallProperties.RegisterProductToDashboard = false;
                        string location = product.Location.Split(';')[0].ToUpper();
                        location = location.Replace(".", Environment.MachineName).Replace("(LOCAL)", Environment.MachineName);
                        var productVersion = new Version(product.Version);
                        if (location.StartsWith(repositoryInstance) && m_installerVersion > productVersion)
                        {
                            Host.CwfHelper.ProductsToBeUpgraded = new CMProducts();
                            Host.CwfHelper.ProductsToBeUpgraded.Add(product);
                            InstallProperties.RegisterProductToDashboard = true;
                            InstallProperties.CMDisplayName = product.InstanceName;
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // log exception here
                return false;
            }
            return false;
        }

        private bool installNewMSI(string productArgs)
        {
            try
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\";
                process.StartInfo.FileName = @"msiexec";
                process.StartInfo.Arguments = productArgs;
                process.Start();
                process.WaitForExit();
                installerReturnCode = process.ExitCode;
                process.Close();
                if (installerReturnCode == 0 || installerReturnCode == 3010)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                // log error here
                return false;
            }
            return false;
        }

        private bool executeExe(string productArgs, string filename)
        {
            try
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\";
                process.StartInfo.FileName = filename;
                process.StartInfo.Arguments = productArgs;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                process.WaitForExit();
                installerReturnCode = process.ExitCode;
                process.Close();
                if (installerReturnCode == 0 || installerReturnCode == 3010)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                // log error here
                return false;
            }
            return false;
        }

        private bool StartRegistrationService()
        {
            try
            {
                ServiceController RegistrationService = new ServiceController(Constants.RegistrationServiceName);
                RegistrationService.Start();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private bool StartAlertsService()
        {
            try
            {
                ServiceController AlertsService = new ServiceController(Constants.AlertsServiceName);
                AlertsService.Start();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private bool StopRegistrationService()
        {
            try
            {
                ServiceController RegistrationService = new ServiceController(Constants.RegistrationServiceName);
                if (RegistrationService.Status != ServiceControllerStatus.Stopped)
                {
                    RegistrationService.Stop();
                }
                RegistrationService.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private bool CopyClusterExeToInstallDir()
        {
            string clusterExeFileName = "";
            if (Environment.Is64BitOperatingSystem)
            {
                clusterExeFileName = "SQLcomplianceClusterSetup-x64.exe";
            }
            else
            {
                clusterExeFileName = "SQLcomplianceClusterSetup.exe";
            }

            try
            {
                var CMInstallPath = Path.Combine(InstallProperties.CMInstallDir, clusterExeFileName);
                if (File.Exists(CMInstallPath))
                {
                    File.Delete(CMInstallPath);
                }
                File.Copy(clusterExeFileName, CMInstallPath);
                return true;
            }
            catch (Exception ex)
            {
                // log error message
                return false;
            }
        }

        private void GenerateSQLCMFirewallConfigurationInformation()
        {
            if (Constants.UserInstallType == InstallType.CMAndDashboard)
            {
                try
                {
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"SQLCM_FirewallConfigurationInformation.txt"))
                    {
                        string line = "---------- IDERA SQL Compliance Manager PORTS ----------";
                        line += "\r\nTo ensure the SQLcompliance Agent and Collection Server can successfully audit instances in your environment,\r\n open the following ports.";
                        line += "\r\n\r\n";
                        line += "\r\n+------------------------------------------------+--------------------+--------------------------+";
                        line += "\r\n|               Service                          |  Trusted Domains   |  Non-trusted Domains     |";
                        line += "\r\n+------------------------------------------------+--------------------+--------------------------+";
                        line += "\r\n| SQL Compliance Manager Collection Service      |       5201         |           5201           |";
                        line += "\r\n+------------------------------------------------+--------------------+--------------------------+";
                        line += "\r\n| SQL Compliance Manager Agent Service           |       5200         |           5200           |";
                        line += "\r\n+------------------------------------------------+--------------------+--------------------------+";
                        line += "\r\n";
                        line += "\r\n----------------- IDERA Dashboard Ports --------------------";
                        line += "\r\nThe IDERA Dashboard services use specific ports for communication.";
                        line += "\r\nEnsure the following ports are available:";
                        line += "\r\n";
                        line += "\r\n+-------------------------------------------------------+-----------+";
                        line += "\r\n| Service                                               |    Port   |";
                        line += "\r\n+-------------------------------------------------------+-----------+";
                        line += "\r\n| IDERA Dashboard Core Service                          |    " + "9292" + "   |";
                        line += "\r\n+-------------------------------------------------------+-----------+";
                        line += "\r\n| IDERA Dashboard Web Application Service (HTTP)        |    " + "9290" + "   |";
                        line += "\r\n+-------------------------------------------------------+-----------+";
                        line += "\r\n| IDERA Dashboard Web Application Service (HTTPS)       |    " + "9291" + "   |";
                        line += "\r\n+-------------------------------------------------------+-----------+";
                        line += "\r\n| IDERA Dashboard Web Application Service (Monitor)     |    " + "9094" + "   |";
                        line += "\r\n+-------------------------------------------------------+-----------+";
                        file.WriteLine(line);
                    }
                }
                catch (Exception ex)
                { }
            }

            if (Constants.UserInstallType == InstallType.AgentAndDashboard)
            {
                try
                {
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"SQLCM_FirewallConfigurationInformation.txt"))
                    {
                        string line = "---------- IDERA SQL Compliance Manager PORTS ----------";
                        line += "\r\nTo ensure the SQLcompliance Agent and Collection Server can successfully audit instances in your environment,\r\n open the following ports.";
                        line += "\r\n\r\n";
                        line += "\r\n+------------------------------------------------+--------------------+--------------------------+";
                        line += "\r\n|               Service                          |  Trusted Domains   |  Non-trusted Domains     |";
                        line += "\r\n+------------------------------------------------+--------------------+--------------------------+";
                        line += "\r\n| SQL Compliance Manager Agent Service           |       5200         |           5200           |";
                        line += "\r\n+------------------------------------------------+--------------------+--------------------------+";
                        line += "\r\n";
                        line += "\r\n----------------- IDERA Dashboard Ports --------------------";
                        line += "\r\nThe IDERA Dashboard services use specific ports for communication.";
                        line += "\r\nEnsure the following ports are available:";
                        line += "\r\n";
                        line += "\r\n+-------------------------------------------------------+-----------+";
                        line += "\r\n| Service                                               |    Port   |";
                        line += "\r\n+-------------------------------------------------------+-----------+";
                        line += "\r\n| IDERA Dashboard Core Service                          |    " + "9292" + "   |";
                        line += "\r\n+-------------------------------------------------------+-----------+";
                        line += "\r\n| IDERA Dashboard Web Application Service (HTTP)        |    " + "9290" + "   |";
                        line += "\r\n+-------------------------------------------------------+-----------+";
                        line += "\r\n| IDERA Dashboard Web Application Service (HTTPS)       |    " + "9291" + "   |";
                        line += "\r\n+-------------------------------------------------------+-----------+";
                        line += "\r\n| IDERA Dashboard Web Application Service (Monitor)     |    " + "9094" + "   |";
                        line += "\r\n+-------------------------------------------------------+-----------+";
                        file.WriteLine(line);
                    }
                }
                catch (Exception ex)
                { }
            }

            else if (Constants.UserInstallType == InstallType.CMOnly)
            {
                try
                {
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"SQLCM_FirewallConfigurationInformation.txt"))
                    {
                        string line = "---------- IDERA SQL Compliance Manager PORTS ----------";
                        line += "\r\nTo ensure the SQLcompliance Agent and Collection Server can successfully audit instances in your environment,\r\n open the following ports.";
                        line += "\r\n\r\n";
                        line += "\r\n+------------------------------------------------+--------------------+--------------------------+";
                        line += "\r\n|               Service                          |  Trusted Domains   |  Non-trusted Domains     |";
                        line += "\r\n+------------------------------------------------+--------------------+--------------------------+";
                        line += "\r\n| SQL Compliance Manager Collection Service      |       5201         |           5201           |";
                        line += "\r\n+------------------------------------------------+--------------------+--------------------------+";
                        line += "\r\n| SQL Compliance Manager Agent Service           |       5200         |           5200           |";
                        line += "\r\n+------------------------------------------------+--------------------+--------------------------+";
                        line += "\r\n";
                        file.WriteLine(line);
                    }
                }
                catch (Exception ex)
                { }
            }

            else if (Constants.UserInstallType == InstallType.AgentOnly)
            {
                try
                {
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"SQLCM_FirewallConfigurationInformation.txt"))
                    {
                        string line = "---------- IDERA SQL Compliance Manager PORTS ----------";
                        line += "\r\nTo ensure the SQLcompliance Agent and Collection Server can successfully audit instances in your environment,\r\n open the following ports.";
                        line += "\r\n\r\n";
                        line += "\r\n+------------------------------------------------+--------------------+--------------------------+";
                        line += "\r\n|               Service                          |  Trusted Domains   |  Non-trusted Domains     |";
                        line += "\r\n+------------------------------------------------+--------------------+--------------------------+";
                        line += "\r\n| SQL Compliance Manager Agent Service           |       5200         |           5200           |";
                        line += "\r\n+------------------------------------------------+--------------------+--------------------------+";
                        line += "\r\n";
                        file.WriteLine(line);
                    }
                }
                catch (Exception ex)
                { }
            }


            else if (Constants.UserInstallType == InstallType.DashboardOnly ||
                     Constants.UserInstallType == InstallType.ConsoleAndDashboard)
            {
                try
                {
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"SQLCM_FirewallConfigurationInformation.txt"))
                    {
                        string line = "\r\n----------------- IDERA Dashboard Ports --------------------";
                        line += "\r\nThe IDERA Dashboard services use specific ports for communication.";
                        line += "\r\nEnsure the following ports are available:";
                        line += "\r\n";
                        line += "\r\n+-------------------------------------------------------+-----------+";
                        line += "\r\n| Service                                               |    Port   |";
                        line += "\r\n+-------------------------------------------------------+-----------+";
                        line += "\r\n| IDERA Dashboard Core Service                          |    " + "9292" + "   |";
                        line += "\r\n+-------------------------------------------------------+-----------+";
                        line += "\r\n| IDERA Dashboard Web Application Service (HTTP)        |    " + "9290" + "   |";
                        line += "\r\n+-------------------------------------------------------+-----------+";
                        line += "\r\n| IDERA Dashboard Web Application Service (HTTPS)       |    " + "9291" + "   |";
                        line += "\r\n+-------------------------------------------------------+-----------+";
                        line += "\r\n| IDERA Dashboard Web Application Service (Monitor)     |    " + "9094" + "   |";
                        line += "\r\n+-------------------------------------------------------+-----------+";
                        file.WriteLine(line);
                    }
                }
                catch (Exception ex)
                { }
            }
        }

        private void GetAgentInstancesFromCMRegistry()
        {
            RegistryView registryView = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
            using (RegistryKey hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView))
            {
                RegistryKey instanceKey;
                if (hklm.OpenSubKey(@"SOFTWARE\Idera\SQLCM\SQLcomplianceAgent", false) != null)
                {
                    instanceKey = hklm.OpenSubKey(@"SOFTWARE\Idera\SQLCM\SQLcomplianceAgent", false);
                }
                else
                {
                    instanceKey = hklm.OpenSubKey(@"SOFTWARE\Idera\SQLcompliance\SQLcomplianceAgent", false);
                }

                if (instanceKey != null)
                {
                    agentInstanceList = (string[])instanceKey.GetValue("Instances");
                }
            }
        }

        private void SetAgentInstancesInCMRegistry()
        {
            RegistryView registryView = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
            using (RegistryKey hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView))
            {
                if (agentInstanceList != null && agentInstanceList.Length != 0)
                {
                    RegistryKey instanceKey = hklm.OpenSubKey(@"SOFTWARE\Idera\SQLCM\SQLcomplianceAgent", true);
                    instanceKey.SetValue("Instances", agentInstanceList, RegistryValueKind.MultiString);
                }
            }
        }

        private void SetPageHeader()
        {
            if (!InstallProperties.IsUpgradeRadioSelection)
            {
                if (Constants.UserInstallType == InstallType.CMOnly)
                {
                    labelHeader.Text = "Installing " + Constants.ProductMap[Products.Compliance];
                }
                else if (Constants.UserInstallType == InstallType.ConsoleOnly)
                {
                    labelHeader.Text = "Installing " + Constants.ProductMap[Products.Console];
                }
                else if (Constants.UserInstallType == InstallType.AgentOnly)
                {
                    labelHeader.Text = "Installing " + Constants.ProductMap[Products.Agent];
                }
                else if (Constants.UserInstallType == InstallType.CMAndDashboard)
                {
                    labelHeader.Text = "Installing " + Constants.ProductMap[Products.Compliance] + " and " + Constants.ProductMap[Products.Dashboard];
                }
                else if (Constants.UserInstallType == InstallType.ConsoleAndDashboard)
                {
                    labelHeader.Text = "Installing " + Constants.ProductMap[Products.Console] + " and " + Constants.ProductMap[Products.Dashboard];
                }
                else if (Constants.UserInstallType == InstallType.AgentAndDashboard)
                {
                    labelHeader.Text = "Installing " + Constants.ProductMap[Products.Agent] + " and " + Constants.ProductMap[Products.Dashboard];
                }
                else if (Constants.UserInstallType == InstallType.DashboardOnly)
                {
                    labelHeader.Text = "Installing " + Constants.ProductMap[Products.Dashboard];
                }
            }

            else
            {
                if (Constants.UserInstallType == InstallType.CMOnly)
                {
                    labelHeader.Text = "Upgrading " + Constants.ProductMap[Products.Compliance];
                }
                else if (Constants.UserInstallType == InstallType.ConsoleOnly)
                {
                    labelHeader.Text = "Upgrading " + Constants.ProductMap[Products.Console];
                }
                else if (Constants.UserInstallType == InstallType.AgentOnly)
                {
                    labelHeader.Text = "Upgrading " + Constants.ProductMap[Products.Agent];
                }
                else if (Constants.UserInstallType == InstallType.CMAndDashboard)
                {
                    labelHeader.Text = "Upgrading " + Constants.ProductMap[Products.Compliance] + " and " + Constants.ProductMap[Products.Dashboard];
                }
                else if (Constants.UserInstallType == InstallType.ConsoleAndDashboard)
                {
                    labelHeader.Text = "Upgrading " + Constants.ProductMap[Products.Console] + " and " + Constants.ProductMap[Products.Dashboard];
                }
                else if (Constants.UserInstallType == InstallType.AgentAndDashboard)
                {
                    labelHeader.Text = "Upgrading " + Constants.ProductMap[Products.Agent] + " and " + Constants.ProductMap[Products.Dashboard];
                }
                else if (Constants.UserInstallType == InstallType.DashboardOnly)
                {
                    labelHeader.Text = "Upgrading " + Constants.ProductMap[Products.Dashboard];
                }
            }
        }

        private bool SetPRQRegistry()
        {
            RegistryView registryView = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
            using (RegistryKey hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView))
            {
                RegistryKey sqlcmKey;
                if (hklm.OpenSubKey(@"SOFTWARE\Idera\SQLCM", false) != null)
                {
                    sqlcmKey = hklm.OpenSubKey(@"SOFTWARE\Idera\SQLCM", true);
                }
                else
                {
                    sqlcmKey = hklm.CreateSubKey(@"SOFTWARE\Idera\SQLCM");
                }

                if (sqlcmKey != null)
                {
                    sqlcmKey.SetValue("InstallPRQ", "FALSE");
                    sqlcmKey.Flush();
                    sqlcmKey.Close();
                }
            }
            return true;
        }

        private bool RemovePRQRegistry()
        {
            RegistryView registryView = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
            using (RegistryKey hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView))
            {
                RegistryKey sqlcmKey;
                if (hklm.OpenSubKey(@"SOFTWARE\Idera\SQLCM", false) != null)
                {
                    sqlcmKey = hklm.OpenSubKey(@"SOFTWARE\Idera\SQLCM", true);
                    sqlcmKey.DeleteValue("InstallPRQ", false);
                    sqlcmKey.Flush();
                    sqlcmKey.Close();
                }
            }
            return true;
        }

        private bool InstallNativeClient()
        {
            if (!ShouldInstallNativeClient())
            {
                // This means native client is already present on the system, so return true;
                return true;
            }

            try
            {
                string fileName = "";

                if (Environment.Is64BitOperatingSystem)
                {
                    fileName = "sqlncli-x64.msi";
                }
                else
                {
                    fileName = "sqlncli.msi";
                }

                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\";
                process.StartInfo.FileName = fileName;
                process.StartInfo.Arguments = "/qn IACCEPTSQLNCLILICENSETERMS=YES REBOOT=ReallySuppress";
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                process.WaitForExit();
                installerReturnCode = process.ExitCode;
                process.Close();


                if (installerReturnCode == 0 || installerReturnCode == 3010)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return false;
        }

        private bool ShouldInstallNativeClient()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\SQLNCLI11.DLL";
            if (!System.IO.File.Exists(path))
            {
                // If the file is NOT FOUND on the target system.
                return true;
            }

            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(path);
            string currentNativeClientDLLVersion = versionInfo.FileMajorPart.ToString() + "." + versionInfo.FileMinorPart.ToString() + "." +
                                                    versionInfo.FileBuildPart.ToString() + "." + versionInfo.FilePrivatePart.ToString();
            string nativeClientDLLVersion = "2011.110.7001.0";
            if (String.Compare(currentNativeClientDLLVersion, nativeClientDLLVersion) < 0)
            {
                // If the found SQLNCLI11.dll file version is less than 2011.110.7001.0, we install native client.
                // which is same as, product version less than 11.0.7001.0.
                return true;
            }

            return false;
        }

        private void UpdateRevisionLogs(string productName, string currentVersion, string upgradeVersion, string upgradeType = "Installer")
        {
            var date = DateTime.UtcNow.ToString();
            var logMsg = string.Empty;

            if (string.IsNullOrEmpty(currentVersion))//new installation
                logMsg = string.Format("\nDateTime [UTC]: {0}, Install Type: Fresh Install, Product: {1}, Version: {2}, Upgrade Type: {3}", date, productName, upgradeVersion, upgradeType);
            else
                logMsg = string.Format("\nDateTime [UTC]: {0}, Install Type: Upgrade, Product: {1}, Current Version: {2}, Upgrade Version: {3}, Upgrade Type: {4}", date, productName, currentVersion, upgradeVersion, upgradeType);

            LogHelper.Log(logMsg);
        }
    }
}