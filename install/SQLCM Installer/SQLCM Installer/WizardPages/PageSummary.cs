using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace SQLCM_Installer.WizardPages
{
    [WizardPageInfo(WizardPage.Summary)]
    internal partial class PageSummary : WizardPageBase
    {
        string CMSQLServerAuthText, DashboardSQLServerAuthText;
        public PageSummary(MainForm host)
            : base(host)
        {
            InitializeComponent();
        }

        internal override void Initialize()
        {
            IsInitialized = true;
            mainPanel.AutoScroll = true;
        }

        internal override void OnNavigated(NavigationDirection direction)
        {
            mainPanel.Focus();
            if (this.mainPanel.VerticalScroll.Visible)
            {
                this.mainPanel.AutoScrollPosition = new Point(0, 0);
                this.mainPanel.VerticalScroll.Value = 0;
            }

            if (InstallProperties.IsUpgradeRadioSelection)
            {
                this.labelSubHeader.Text = "Please review the following settings, then click Upgrade";
            }

            if (Constants.UserInstallType == InstallType.CMAndDashboard && !InstallProperties.IsUpgradeRadioSelection)
            {
                this.panelCMSummary.Visible = true;
                this.panelCMAdditionalSummary.Visible = true;
                this.panelDashboardSummary.Visible = true;
                this.panelServiceAccount.Visible = true;
                this.panelCollectionSummary.Visible = false;
                labelSQLCMHeader.Text = Constants.ProductMap[Products.Compliance];

                this.panelCMSummary.Location = new Point(0, 64);
                this.panelCMAdditionalSummary.Location = new Point(0, 144);
                this.panelDashboardSummary.Location = new Point(0, 232);
                this.panelServiceAccount.Location = new Point(0, 393);
                this.labelServiceAccount.Text = Constants.InstallCMandDashboardTitleServiceAccountScreen;

                SetCMSummaryRegion(InstallProperties.CMInstallDir);
                SetCMWindowAuth();
                SetCMAdditionalSummaryRegion(InstallProperties.CMSQLServerInstanceName, "SQLcompliance", CMSQLServerAuthText);
                SetDashboardWindowAuth();
                SetDashboardSummaryRegion(InstallProperties.DashboardInstallDir, InstallProperties.DashboardSQLServerInstanceName, "IderaDashboardRepository", DashboardSQLServerAuthText);
                SetServiceAccountRegion(InstallProperties.ServiceUserName);
            }

            else if (Constants.UserInstallType == InstallType.CMOnly && !InstallProperties.IsUpgradeRadioSelection)
            {
                this.panelCMSummary.Visible = true;
                this.panelCMAdditionalSummary.Visible = true;
                this.panelDashboardSummary.Visible = false;
                this.panelServiceAccount.Visible = true;
                this.panelCollectionSummary.Visible = false;
                labelSQLCMHeader.Text = Constants.ProductMap[Products.Compliance];

                this.panelCMSummary.Location = new Point(0, 64);
                this.panelCMAdditionalSummary.Location = new Point(0, 144);
                this.panelServiceAccount.Location = new Point(0, 242);

                SetCMSummaryRegion(InstallProperties.CMInstallDir);
                SetCMWindowAuth();
                SetCMAdditionalSummaryRegion(InstallProperties.CMSQLServerInstanceName, "SQLcompliance", CMSQLServerAuthText);
                SetServiceAccountRegion(InstallProperties.ServiceUserName);
                this.labelServiceAccount.Text = Constants.InstallCMOnlyTitleServiceAccountScreen;
            }

            else if (Constants.UserInstallType == InstallType.ConsoleOnly && !InstallProperties.IsUpgradeRadioSelection)
            {
                this.panelCMSummary.Visible = true;
                this.panelCMAdditionalSummary.Visible = false;
                this.panelDashboardSummary.Visible = false;
                this.panelServiceAccount.Visible = false;
                this.panelCollectionSummary.Visible = false;
                labelSQLCMHeader.Text = Constants.ProductMap[Products.Console];

                this.panelCMSummary.Location = new Point(0, 64);
                SetCMSummaryRegion(InstallProperties.CMInstallDir);
            }

            else if (Constants.UserInstallType == InstallType.DashboardOnly && !InstallProperties.IsUpgradeRadioSelection)
            {
                this.panelCMSummary.Visible = false;
                this.panelCMAdditionalSummary.Visible = false;
                this.panelDashboardSummary.Visible = true;
                this.panelServiceAccount.Visible = true;
                this.panelCollectionSummary.Visible = false;

                this.panelDashboardSummary.Location = new Point(0, 64);
                this.panelServiceAccount.Location = new Point(0, 224);

                SetDashboardWindowAuth();
                SetDashboardSummaryRegion(InstallProperties.DashboardInstallDir, InstallProperties.DashboardSQLServerInstanceName, "IderaDashboardRepository", DashboardSQLServerAuthText);
                SetServiceAccountRegion(InstallProperties.ServiceUserName);
                this.labelServiceAccount.Text = Constants.InstallDashboardOnlyTitleServiceAccountScreen;
            }

            else if (Constants.UserInstallType == InstallType.AgentOnly && !InstallProperties.IsUpgradeRadioSelection)
            {
                this.panelCMSummary.Visible = true;
                this.panelCMAdditionalSummary.Visible = false;
                this.panelDashboardSummary.Visible = false;
                this.panelServiceAccount.Visible = false;
                this.panelCollectionSummary.Visible = true;
                labelSQLCMHeader.Text = Constants.ProductMap[Products.Agent];

                this.panelCMSummary.Location = new Point(0, 64);
                this.panelCollectionSummary.Location = new Point(0, 154);

                SetCMSummaryRegion(InstallProperties.CMInstallDir);
                SetAgentSummaryRegion(InstallProperties.CMDisplayName, InstallProperties.AgentCollectionServer);
            }

            else if (Constants.UserInstallType == InstallType.AgentAndDashboard && !InstallProperties.IsUpgradeRadioSelection)
            {
                this.panelCMSummary.Visible = true;
                this.panelCMAdditionalSummary.Visible = false;
                this.panelDashboardSummary.Visible = true;
                this.panelServiceAccount.Visible = true;
                this.panelCollectionSummary.Visible = true;
                labelSQLCMHeader.Text = Constants.ProductMap[Products.Agent];

                this.panelCMSummary.Location = new Point(0, 64);
                this.panelCollectionSummary.Location = new Point(0, 154);
                this.panelDashboardSummary.Location = new Point(0, 242);
                this.panelServiceAccount.Location = new Point(0, 417);

                SetCMSummaryRegion(InstallProperties.CMInstallDir);
                SetAgentSummaryRegion(InstallProperties.CMDisplayName, InstallProperties.AgentCollectionServer);
                SetDashboardWindowAuth();
                SetDashboardSummaryRegion(InstallProperties.DashboardInstallDir, InstallProperties.DashboardSQLServerInstanceName, "IderaDashboardRepository", DashboardSQLServerAuthText);
                SetServiceAccountRegion(InstallProperties.ServiceUserName);
                this.labelServiceAccount.Text = Constants.InstallCMandDashboardTitleServiceAccountScreen;
            }

            else if (Constants.UserInstallType == InstallType.ConsoleAndDashboard && !InstallProperties.IsUpgradeRadioSelection)
            {
                this.panelCMSummary.Visible = true;
                this.panelCMAdditionalSummary.Visible = false;
                this.panelDashboardSummary.Visible = true;
                this.panelServiceAccount.Visible = true;
                this.panelCollectionSummary.Visible = false;
                labelSQLCMHeader.Text = Constants.ProductMap[Products.Console];

                this.panelCMSummary.Location = new Point(0, 64);
                this.panelDashboardSummary.Location = new Point(0, 153);
                this.panelServiceAccount.Location = new Point(0, 312);

                SetCMSummaryRegion(InstallProperties.CMInstallDir);
                SetDashboardWindowAuth();
                SetDashboardSummaryRegion(InstallProperties.DashboardInstallDir, InstallProperties.DashboardSQLServerInstanceName, "IderaDashboardRepository", DashboardSQLServerAuthText);
                SetServiceAccountRegion(InstallProperties.ServiceUserName);
                this.labelServiceAccount.Text = Constants.InstallDashboardOnlyTitleServiceAccountScreen;
            }

            else if (InstallProperties.IsUpgradeRadioSelection &&
                   (Constants.UserInstallType == InstallType.CMAndDashboard))
            {
                InstallProperties.CMInstallDir = this.labelCMInstallLocationValue.Text = GetCMInstallDirFromRegistry();
                this.panelCMSummary.Visible = true;
                this.panelCMAdditionalSummary.Visible = true;
                this.panelDashboardSummary.Visible = false;
                this.panelServiceAccount.Visible = true;
                this.panelCollectionSummary.Visible = false;
                labelSQLCMHeader.Text = Constants.ProductMap[Products.Compliance];

                this.panelCMSummary.Location = new Point(0, 64);
                this.panelCMAdditionalSummary.Location = new Point(0, 144);
                this.panelServiceAccount.Location = new Point(0, 242);

                SetCMWindowAuth();
                SetCMAdditionalSummaryRegion(InstallProperties.CMSQLServerInstanceName, "SQLcompliance", CMSQLServerAuthText);
                SetServiceAccountRegion(InstallProperties.ServiceUserName);
            }

            else if (InstallProperties.IsUpgradeRadioSelection &&
               (Constants.UserInstallType == InstallType.DashboardOnly))
            {
                this.panelCMSummary.Visible = false;
                this.panelCMAdditionalSummary.Visible = false;
                this.panelDashboardSummary.Visible = false;
                this.panelServiceAccount.Visible = true;
                this.panelCollectionSummary.Visible = false;

                this.panelServiceAccount.Location = new Point(0, 64);
                this.labelServiceAccount.Text = Constants.InstallDashboardOnlyTitleServiceAccountScreen;

                SetServiceAccountRegion(InstallProperties.ServiceUserName);
            }

            else if (InstallProperties.IsUpgradeRadioSelection && Constants.UserInstallType == InstallType.AgentAndDashboard)
            {
                this.labelCMInstallLocationValue.Text = GetCMInstallDirFromRegistry();
                this.panelCMSummary.Visible = true;
                this.panelCMAdditionalSummary.Visible = false;
                this.panelDashboardSummary.Visible = false;
                this.panelServiceAccount.Visible = true;
                this.panelCollectionSummary.Visible = false;
                labelSQLCMHeader.Text = Constants.ProductMap[Products.Agent];

                this.panelCMSummary.Location = new Point(0, 64);
                this.panelServiceAccount.Location = new Point(0, 154);

                SetCMWindowAuth();
                SetServiceAccountRegion(InstallProperties.ServiceUserName);
                if (InstallProperties.IsMajorUpgrade)
                {
                    this.labelServiceAccount.Text = Constants.InstallAgentandDashboardTitleServiceAccountScreen;
                }
                else
                {
                    this.labelServiceAccount.Text = Constants.InstallDashboardOnlyTitleServiceAccountScreen;
                }
            }
            else if (InstallProperties.IsUpgradeRadioSelection && Constants.UserInstallType == InstallType.ConsoleAndDashboard)
            {
                InstallProperties.CMInstallDir = this.labelCMInstallLocationValue.Text = GetCMInstallDirFromRegistry();
                this.panelCMSummary.Visible = true;
                this.panelCMAdditionalSummary.Visible = false;
                this.panelDashboardSummary.Visible = false;
                this.panelServiceAccount.Visible = true;
                this.panelCollectionSummary.Visible = false;
                labelSQLCMHeader.Text = Constants.ProductMap[Products.Console];

                this.panelCMSummary.Location = new Point(0, 64);
                this.panelServiceAccount.Location = new Point(0, 154);
                this.labelServiceAccount.Text = Constants.InstallDashboardOnlyTitleServiceAccountScreen;

                SetCMWindowAuth();
                SetServiceAccountRegion(InstallProperties.ServiceUserName);
            }
            else if (Constants.UserInstallType == InstallType.CMOnly && InstallProperties.IsUpgradeRadioSelection)
            {
                InstallProperties.CMInstallDir = this.labelCMInstallLocationValue.Text = GetCMInstallDirFromRegistry();
                this.panelCMSummary.Visible = true;
                this.panelCMAdditionalSummary.Visible = true;
                this.panelDashboardSummary.Visible = false;
                this.panelServiceAccount.Visible = InstallProperties.IsMajorUpgrade;
                this.panelCollectionSummary.Visible = false;
                labelSQLCMHeader.Text = Constants.ProductMap[Products.Compliance];

                this.panelCMSummary.Location = new Point(0, 64);
                this.panelCMAdditionalSummary.Location = new Point(0, 144);
                this.panelServiceAccount.Location = new Point(0, 242);

                SetCMWindowAuth();
                SetCMAdditionalSummaryRegion(InstallProperties.CMSQLServerInstanceName, "SQLcompliance", CMSQLServerAuthText);
                SetServiceAccountRegion(InstallProperties.ServiceUserName);
                this.labelServiceAccount.Text = Constants.InstallCMOnlyTitleServiceAccountScreen;
            }

            else if (Constants.UserInstallType == InstallType.ConsoleOnly && InstallProperties.IsUpgradeRadioSelection)
            {
                InstallProperties.CMInstallDir = this.labelCMInstallLocationValue.Text = GetCMInstallDirFromRegistry();
                this.panelCMSummary.Visible = true;
                this.panelCMAdditionalSummary.Visible = false;
                this.panelDashboardSummary.Visible = false;
                this.panelServiceAccount.Visible = false;
                this.panelCollectionSummary.Visible = false;
                labelSQLCMHeader.Text = Constants.ProductMap[Products.Console];

                this.panelCMSummary.Location = new Point(0, 64);
            }

            else if (Constants.UserInstallType == InstallType.DashboardOnly && InstallProperties.IsUpgradeRadioSelection)
            {
                this.labelDashboardInstallLocationValue.Text = GetDashboardInstallDirFromRegistry();
                this.panelCMSummary.Visible = false;
                this.panelCMAdditionalSummary.Visible = false;
                this.panelDashboardSummary.Visible = true;
                this.panelServiceAccount.Visible = true;
                this.panelCollectionSummary.Visible = false;

                this.labelDashboardSQLServerInstance.Visible = false;
                this.labelDashboardSQLServerInstanceValue.Visible = false;
                this.labelDashboardDatabaseName.Visible = false;
                this.labelDashboardDatabaseNameValue.Visible = false;
                this.labelDashboardAuthType.Visible = false;
                this.labelDashboardAuthTypeValue.Visible = false;
                this.panelDashboardSummary.Size = new Size(533, 89);
                this.panelServiceAccount.Location = new Point(0, 154);
                this.labelServiceAccount.Text = Constants.InstallDashboardOnlyTitleServiceAccountScreen;
                this.labelServiceAccountCred.Location = new Point(20, 100);
                this.labelServiceAccountCredValue.Location = new Point(190, 100);

                this.panelDashboardSummary.Location = new Point(0, 64);
                this.panelServiceAccount.Location = new Point(0, 224);

                SetDashboardWindowAuth();
                SetDashboardSummaryRegion(InstallProperties.DashboardInstallDir, InstallProperties.DashboardSQLServerInstanceName, "IderaDashboardRepository", DashboardSQLServerAuthText);
                SetServiceAccountRegion(InstallProperties.ServiceUserName);
            }

            else if (Constants.UserInstallType == InstallType.AgentOnly && InstallProperties.IsUpgradeRadioSelection)
            {
                this.labelCMInstallLocationValue.Text = GetCMInstallDirFromRegistry();
                this.panelCMSummary.Visible = true;
                this.panelCMAdditionalSummary.Visible = false;
                this.panelDashboardSummary.Visible = false;
                this.panelServiceAccount.Visible = InstallProperties.IsMajorUpgrade;
                this.panelCollectionSummary.Visible = false;
                labelSQLCMHeader.Text = Constants.ProductMap[Products.Agent];

                this.panelCMSummary.Location = new Point(0, 64);
                this.panelCollectionSummary.Location = new Point(0, 154);
                this.panelServiceAccount.Location = new Point(0, 154);
                SetServiceAccountRegion(InstallProperties.ServiceUserName);
                this.labelServiceAccount.Text = Constants.InstallAgentOnlyTitleServiceAccountScreen;
            }

            SetPanelWidth();
        }

        private void SetPanelWidth()
        {
            if (mainPanel.VerticalScroll.Visible)
            {
                panelServiceAccount.Width = 533;
                panelServiceAccountHeader.Width = 533;
                panelCollectionSummary.Width = 533;
                panelCollectionServerHeader.Width = 533;
                panelCMAdditionalSummary.Width = 533;
                panelDashboardSummary.Width = 533;
                panelDashboardHeader.Width = 533;
                panelSubHeaderPanel.Width = 533;
                panelSQLCMHeader.Width = 533;
                panelCMSummary.Width = 533;
            }
            else
            {
                panelServiceAccount.Width = 550;
                panelServiceAccountHeader.Width = 550;
                panelCollectionSummary.Width = 550;
                panelCollectionServerHeader.Width = 550;
                panelCMAdditionalSummary.Width = 550;
                panelDashboardSummary.Width = 550;
                panelDashboardHeader.Width = 550;
                panelSubHeaderPanel.Width = 550;
                panelSQLCMHeader.Width = 550;
                panelCMSummary.Width = 550;
            }
        }

        private void SetCMSummaryRegion(string _CMInstallDir)
        {
            this.labelCMInstallLocationValue.Text = _CMInstallDir;
        }

        private void SetCMAdditionalSummaryRegion(string _CMSQLServerInstanceName, string _CMdatabaseName, string _CMAuthtypeText)
        {
            this.labelCMSQLServerInstanceValue.Text = _CMSQLServerInstanceName;
            this.labelCMDatabaseNameValue.Text = _CMdatabaseName;
            this.labelCMAuthTypeValue.Text = _CMAuthtypeText;
        }

        private void SetDashboardSummaryRegion(string _DashboardInstallDir, string _DashboardSQLServerInstanceName, string _DashboarddatabaseName, string _DashboardAuthtypeText)
        {
            if (!InstallProperties.IsUpgradeRadioSelection)
            {
                this.labelDashboardInstallLocationValue.Text = _DashboardInstallDir;
            }
            this.labelDashboardSQLServerInstanceValue.Text = _DashboardSQLServerInstanceName;
            this.labelDashboardDatabaseNameValue.Text = _DashboarddatabaseName;
            this.labelDashboardAuthTypeValue.Text = _DashboardAuthtypeText;
        }

        private void SetServiceAccountRegion(string _serviceUserName)
        {
            this.labelServiceAccountCredValue.Text = _serviceUserName;
            if (InstallProperties.IsUpgradeRadioSelection)
            {
                labelServiceAccount.Text = "The " + Constants.ProductMap[Products.Dashboard] + " services will use the Windows credentials specified below.";
            }
        }

        private void SetCMWindowAuth()
        {
            if (!InstallProperties.IsCMSQLServerSQLAuth)
            {
                CMSQLServerAuthText = "Windows: " + System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            }
            else
            {
                CMSQLServerAuthText = "Microsoft SQL Server: " + InstallProperties.CMSQLServerUsername;
            }
        }

        private void SetDashboardWindowAuth()
        {
            if (!InstallProperties.IsDashboardSQLServerSQLAuth)
            {
                DashboardSQLServerAuthText = "Windows: " + System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            }
            else
            {
                DashboardSQLServerAuthText = "Microsoft SQL Server: " + InstallProperties.DashboardSQLServerUsername;
            }
        }

        private void SetAgentSummaryRegion(string _CMDisplayName, string _AgentCollectionServer)
        {
            labelCollectionServerValue.Text = _AgentCollectionServer;
        }

        private string GetCMInstallDirFromRegistry()
        {
            string path = "VALUE_NOT_FOUND";
            RegistryView registryView = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
            using (RegistryKey hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView))
            {
                RegistryKey instanceKey = null;
                if (hklm.OpenSubKey(@"SOFTWARE\Idera\SQLCM", false) != null)
                {
                    instanceKey = hklm.OpenSubKey(@"SOFTWARE\Idera\SQLCM", false);
                    if (instanceKey != null)
                    {
                        path = instanceKey.GetValue("Path", "VALUE_NOT_FOUND").ToString();
                    }
                }

                if (instanceKey == null || path == "VALUE_NOT_FOUND")
                {
                    instanceKey = hklm.OpenSubKey(@"SOFTWARE\Idera\SQLcompliance", false);
                    if (instanceKey != null)
                    {
                        path = instanceKey.GetValue("Path", "VALUE_NOT_FOUND").ToString();
                    }
                }

                if (path == "VALUE_NOT_FOUND")
                {
                    path = "C:\\Program Files\\Idera\\SQLcompliance\\";
                }
            }
            return path;
        }

        private string GetDashboardInstallDirFromRegistry()
        {
            string path = "";
            RegistryView registryView = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
            using (RegistryKey hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView))
            {
                RegistryKey instanceKey = hklm.OpenSubKey(@"SOFTWARE\Idera\CWF", false);
                if (instanceKey != null)
                {
                    path = instanceKey.GetValue("InstallDir").ToString();
                }
            }
            return path;
        }
    }
}
