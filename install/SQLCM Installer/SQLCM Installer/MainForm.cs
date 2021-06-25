using Microsoft.Win32;
using SQLCM_Installer.Custom_Controls;
using SQLCM_Installer.Custom_Prompts;
using SQLCM_Installer.Properties;
using SQLCM_Installer.WizardPages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQLCM_Installer
{
    public partial class MainForm : Form
    {
        private Dictionary<WizardPage, KeyValuePair<WizardPageInfo, WizardPageBase>> _wizardPages;
        private WizardPage _currentPage;
        private WizardPage _previousPage = WizardPage.NotSpecified;
        private List<WizardPage> _visitedPages = new List<WizardPage>();
        private string _cancelMessage;
        private bool _isPreviousEnabled;
        private bool _isNextEnabled;
        private bool _isCancelEnabled;
        internal CwfHelper CwfHelper { get; private set; }
        internal string CwfAddInZip { get; private set; }
        bool IsCMCredOk;
        bool IsDahboardCredOk;
        private bool isUpgrade = false;
        private CustomDropShadow customDropShadow;
        private CustomBorderShadow customBorderShadow;
        bool allowNavigation = true;

        #region Window Draggable
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HTCAPTION = 0x2;

        public static System.Drawing.Point location;

        [DllImport("User32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        #endregion

        public MainForm()
        {
            CwfHelper = new CwfHelper(this);
            CwfAddInZip = "SqlComplianceManager.zip";
            CwfHelper.CwfUser = "";
            CwfHelper.CwfPassword = "";
            this.FormBorderStyle = FormBorderStyle.None;
            InitializeComponent();
        }

        ~MainForm()
        {
            _wizardPages.Clear();
        }

        internal void ShowError(string message, params object[] parameters)
        {
            IderaMessageBox errorMessage = new IderaMessageBox();
            if (message.Contains("The Windows Server version you are currently running is no longer supported. By clicking OK the installation will exit"))
            {
                errorMessage.Show(message, true);
                ExitApplication();
            }
            else { 
                errorMessage.Show(message);
            }
           
        }

        private void InitializeWizardPages()
        {
            _currentPage = WizardPage.NotSpecified;

            _wizardPages = new Dictionary<WizardPage, KeyValuePair<WizardPageInfo, WizardPageBase>>();
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (!type.IsClass)
                    continue;

                var attributes = type.GetCustomAttributes(typeof(WizardPageInfo), false);
                if (attributes.Length != 1)
                    continue;

                var wizardPageInfo = (WizardPageInfo)attributes[0];
                var wizardPageEntry = new KeyValuePair<WizardPageInfo, WizardPageBase>(wizardPageInfo, (WizardPageBase)Activator.CreateInstance(type, this));
                _wizardPages.Add(wizardPageInfo.Page, wizardPageEntry);
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (!DesignMode)
            {
                customDropShadow = new CustomDropShadow(this)
                {
                    ShadowBlur = 40,
                    ShadowSpread = -30,
                    ShadowColor = Color.Black

                };
                customDropShadow.RefreshShadow();

                customBorderShadow = new CustomBorderShadow(this)
                {
                    ShadowBlur = 0,
                    ShadowSpread = 1,
                    ShadowColor = Color.FromArgb(24, 131, 215)

                };
                customBorderShadow.RefreshShadow();
            }

            string cancelMessage = "You have cancelled the installation before " + Constants.ProductMap[Products.Compliance] + " could \nbe completely installed." + "\r\n\r\n" + "Your system has not been modified. To complete the installation at another \ntime, please run setup again." + "\r\n\r\n" + "Click Resume to continue the installation.";
            _cancelMessage = string.Format(cancelMessage, Text);
            CheckFrameworkVersion();
            InitializeWizardPages();
            CheckUpgradeProducts();
            NavigateToPage(NavigationDirection.Next);
        }

        private void CheckFrameworkVersion()
        {
            bool frameworkExist35 = false;
            bool frameworkExist20 = false;
            using (RegistryKey ndpKey =
                RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, "").
                OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\"))
            {
                foreach (string versionKeyName in ndpKey.GetSubKeyNames())
                {
                    if (versionKeyName.StartsWith("v"))
                    {
                        string Framework = versionKeyName.Remove(0, 1);
                        if (Framework.StartsWith("3.5"))
                        {
                            frameworkExist35 = true;
                        }
                        if (Framework.StartsWith("2.0"))
                        {
                            frameworkExist20 = true;
                        }
                        if (Framework.StartsWith("4"))
                        {
                            frameworkExist20 = true;
                            frameworkExist35 = true;
                        }
                    }
                }

                if (!(frameworkExist20 && frameworkExist35))
                {
                    string message = "SQL Compliance Manager requires that your computer has the .NET Framework version 3.5. You can enable it from Control Panel → Windows Components or Server Manager.";
                    IderaMessageBox messageBox = new IderaMessageBox();
                    messageBox.Show(message);
                    ExitApplication();
                }
            }
        }

        private void CheckUpgradeProducts()
        {
            bool isCMInstalled = false;
            Products installedCMProduct = Products.NA;
            bool isDashboardInstalled = false;

            string uninstallKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            using (RegistryKey uninstallRoot = Registry.LocalMachine.OpenSubKey(uninstallKey))
            {
                foreach (string productKeyString in uninstallRoot.GetSubKeyNames())
                {
                    RegistryKey productKey = uninstallRoot.OpenSubKey(productKeyString);
                    {
                        try
                        {
                            var publisher = productKey.GetValue("Publisher");
                            if (publisher != null && publisher.ToString().ToUpper() == "IDERA")
                            {
                                var displayName = productKey.GetValue("DisplayName");
                                var installedVersionString = productKey.GetValue("DisplayVersion");

                                if (displayName.ToString().ToUpper() == Constants.ProductInstalledNameMap[InstalledProducts.AgentX64]
                                    || displayName.ToString().ToUpper() == Constants.ProductInstalledNameMap[InstalledProducts.AgentX86])
                                {
                                    isUpgrade = true;
                                    InstallProperties.isSilentAgentInstalled = true;
                                }
                                if (displayName.ToString().ToUpper() == Constants.ProductInstalledNameMap[InstalledProducts.ComplianceX64]
                                    || displayName.ToString().ToUpper() == Constants.ProductInstalledNameMap[InstalledProducts.ComplianceX86])
                                {
                                    isUpgrade = true;
                                    isCMInstalled = true;
                                    if (MatchVersionWithCurrentCM(installedVersionString.ToString()))
                                    {
                                        InstallProperties.isCMCurrentVersionInstalled = true;
                                    }
                                }

                                if (displayName.ToString().ToUpper() == Constants.ProductInstalledNameMap[InstalledProducts.DashboardX64]
                                    || displayName.ToString().ToUpper() == Constants.ProductInstalledNameMap[InstalledProducts.DashboardX86])
                                {
                                    isUpgrade = true;
                                    isDashboardInstalled = true;
                                    if (MatchVersionWithCurrentDashboard(installedVersionString.ToString()))
                                    {
                                        InstallProperties.isDashboardCurrentVersionInstalled = true;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            // expected exception, continue
                            continue;
                        }
                        finally
                        {
                            if (productKey != null)
                            {
                                productKey.Close();
                                productKey = null;
                            }
                        }
                    }
                }
            }

            // Check Feature
            if (isCMInstalled)
            {
                HelperFunctions helperFunction = new HelperFunctions();
                if (!helperFunction.CheckLocalInstalledProduct(out installedCMProduct))
                {
                    installedCMProduct = Products.Compliance;
                }
            }

            if (!InstallProperties.IsMajorUpgrade)
            {
                Constants.AgentUpgradeFlow.Remove(WizardPage.ServiceAccount);
            }

            if (InstallProperties.isCMCurrentVersionInstalled && InstallProperties.isDashboardCurrentVersionInstalled)
            {
                InstallProperties.isCurrentVersionInstalled = true;
            }

            if (isUpgrade)
            {
                InstallProperties.IsUpgradeRadioSelection = true;
                if (isDashboardInstalled)
                {
                    if (isCMInstalled)
                    {
                        switch (installedCMProduct)
                        {
                            case Products.Compliance:
                                Constants.UserCurrentInstallation = InstallType.CMAndDashboard;
                                break;
                            case Products.Agent:
                                Constants.UserCurrentInstallation = InstallType.AgentAndDashboard;
                                break;
                            case Products.Console:
                                Constants.UserCurrentInstallation = InstallType.ConsoleAndDashboard;
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        Constants.UserCurrentInstallation = InstallType.DashboardOnly;
                    }
                }
                else
                {
                    if (isCMInstalled)
                    {
                        switch (installedCMProduct)
                        {
                            case Products.Compliance:
                                Constants.UserCurrentInstallation = InstallType.CMOnly;
                                break;
                            case Products.Agent:
                                Constants.UserCurrentInstallation = InstallType.AgentOnly;
                                break;
                            case Products.Console:
                                Constants.UserCurrentInstallation = InstallType.ConsoleOnly;
                                break;
                            default:
                                break;
                        }
                    }
                }

                if (InstallProperties.isSilentAgentInstalled && !(Constants.UserCurrentInstallation == InstallType.ConsoleOnly
                        || Constants.UserCurrentInstallation == InstallType.ConsoleAndDashboard
                        || Constants.UserCurrentInstallation == InstallType.DashboardOnly))
                {
                    InstallProperties.BlockInstallation = true;
                }
            }
        }

        private bool MatchVersionWithCurrentCM(string installedVersionString)
        {
            Version installedVersion;
            Version currentProductVersion;
            if (Version.TryParse(installedVersionString, out installedVersion))
            {
                if (Version.TryParse(Application.ProductVersion, out currentProductVersion))
                {
                    Constants.InstalledCMVersion = installedVersion.Major + "." + installedVersion.Minor + "." + installedVersion.Build;
                    if (installedVersion.Major >= currentProductVersion.Major)
                    {
                        if (installedVersion.Minor >= currentProductVersion.Minor)
                        {
                            if (installedVersion.Build >= currentProductVersion.Build)
                            {
                                return true;
                            }
                        }
                    }
                    else if (installedVersion.Major < currentProductVersion.Major)
                    {
                        InstallProperties.IsMajorUpgrade = true;
                    }
                }
            }
            return false;
        }

        private bool MatchVersionWithCurrentDashboard(string installedVersionString)
        {
            Version installedVersion;
            if (Version.TryParse(installedVersionString, out installedVersion))
            {
                Constants.InstalledDashboardVersion = installedVersion.Major + "." + installedVersion.Minor + "." + installedVersion.Build;
                if (installedVersion.Major >= Constants.DashboardVersion.Major)
                {
                    if (installedVersion.Minor >= Constants.DashboardVersion.Minor)
                    {
                        if (installedVersion.Build >= Constants.DashboardVersion.Build)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private void MainForm_Activated(object sender, EventArgs e)
        {
            if (customBorderShadow != null && customBorderShadow.ShadowColor != Color.FromArgb(24, 131, 215))
            {
                customDropShadow.ShadowBlur = 40;
                customDropShadow.RefreshShadow();

                customBorderShadow.ShadowColor = Color.FromArgb(24, 131, 215);
                customBorderShadow.RefreshShadow();
            }
        }

        private void MainForm_Deactivated(object sender, EventArgs e)
        {
            if (customBorderShadow != null && customBorderShadow.ShadowColor != Color.Gray)
            {
                customDropShadow.ShadowBlur = 35;
                customDropShadow.RefreshShadow();

                customBorderShadow.ShadowColor = Color.Gray;
                customBorderShadow.RefreshShadow();
            }
        }

        private bool DoAction()
        {
            if (!_wizardPages.ContainsKey(_currentPage))
                return true;

            _isPreviousEnabled = backButton.Enabled;
            _isCancelEnabled = cancelButton.Enabled;
            AllowNavigation(false);

            var currentPage = _wizardPages[_currentPage];

            string errorMessage = string.Empty;
            Control invalidControl = null;
            var operationSuccessful = false;
            try
            {
                operationSuccessful = currentPage.Value.DoAction(out errorMessage, out invalidControl);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            finally
            {
                if (!operationSuccessful)
                {
                    if (!string.IsNullOrEmpty(errorMessage))
                        ShowError(errorMessage);
                    if (invalidControl != null)
                        invalidControl.Focus();
                }
            }

            AllowNavigation(true);
            return operationSuccessful;
        }

        internal void NavigateToPage(NavigationDirection direction)
        {
            if (direction == NavigationDirection.Next && !DoAction())
                return;

            if (direction == NavigationDirection.Next && nextButton.Text == "Finish")
            {
                ExitApplication();
                return;
            }

            WizardPage nextPage = WizardPage.NotSpecified;
            List<WizardPage> installationFlow = new List<WizardPage>();

            if (Constants.UserInstallType == InstallType.AgentOnly)
            {
                if (isUpgrade)
                {
                    installationFlow = Constants.AgentUpgradeFlow;
                }
                else
                {
                    installationFlow = Constants.AgentInstallationFlow;
                }
            }
            else if (Constants.UserInstallType == InstallType.AgentAndDashboard)
            {
                if (isUpgrade)
                {
                    installationFlow = Constants.AgentAndDashboardUpgradeFlow;
                }
                else
                {
                    installationFlow = Constants.AgentAndDashboardInstallationFlow;
                }
            }
            else if (Constants.UserInstallType == InstallType.ConsoleOnly)
            {
                if (isUpgrade)
                {
                    installationFlow = Constants.ConsoleUpgradeFlow;
                }
                else
                {
                    installationFlow = Constants.ConsoleInstallationFlow;
                }
            }
            else if (Constants.UserInstallType == InstallType.DashboardOnly)
            {
                if (isUpgrade)
                {
                    installationFlow = Constants.DashboardUpgradeFlow;
                }
                else
                {
                    installationFlow = Constants.DashboardInstallationFlow;
                }
            }
            else if (Constants.UserInstallType == InstallType.ConsoleAndDashboard)
            {
                if (isUpgrade)
                {
                    installationFlow = Constants.ConsoleAndDashboardUpgradeFlow;
                }
                else
                {
                    installationFlow = Constants.ConsoleAndDashboardInstallationFlow;
                }
            }
            else if (Constants.UserInstallType == InstallType.CMOnly)
            {
                if (isUpgrade)
                {
                    installationFlow = Constants.CMUpgradeFlow;
                }
                else
                {
                    installationFlow = Constants.CMInstallationFlow;
                }
            }
            else
            {
                if (isUpgrade)
                {
                    installationFlow = Constants.FullUpgradeFlow;
                }
                else
                {
                    installationFlow = Constants.FullInstallationFlow;
                }
            }

            if (direction == NavigationDirection.Next)
            {
                int currentPageIndex = installationFlow.IndexOf(_currentPage);
                if (currentPageIndex > -1 && installationFlow.Count > currentPageIndex + 1)
                {
                    nextPage = installationFlow[currentPageIndex + 1];
                }
                else
                {
                    nextPage = _currentPage;
                }

                if (isUpgrade && nextPage == WizardPage.SetupType && InstallProperties.IsUpgradeRadioSelection)
                {
                    nextPage = installationFlow[currentPageIndex + 2];
                }

                if ((InstallProperties.Clustered && !InstallProperties.IsActiveNode) && nextPage == WizardPage.TraceDirectory)
                {
                    nextPage = installationFlow[currentPageIndex + 2];
                }

                if (nextPage == WizardPage.NotSpecified)
                {
                    if (isUpgrade)
                    {
                        nextPage = WizardPage.UpgradeIntroduction;
                    }
                    else
                    {
                        nextPage = WizardPage.Introduction;
                    }
                }
            }
            else if (direction == NavigationDirection.Error)
            {
                int currentPageIndex = installationFlow.IndexOf(_currentPage);
                if (currentPageIndex > -1 && installationFlow.Count > currentPageIndex + 2)
                {
                    nextPage = installationFlow[currentPageIndex + 2];
                }
                else
                {
                    nextPage = _currentPage;
                }
            }
            else if (direction == NavigationDirection.Finish)
            {
                int currentPageIndex = installationFlow.IndexOf(_currentPage);
                if (currentPageIndex > -1 && installationFlow.Count > currentPageIndex + 1)
                {
                    nextPage = installationFlow[currentPageIndex + 1];
                }
                else
                {
                    nextPage = _currentPage;
                }
            }
            else
            {
                int currentPageIndex = installationFlow.IndexOf(_currentPage);
                if (currentPageIndex > -1 && installationFlow.Count > currentPageIndex - 1)
                {
                    nextPage = installationFlow[currentPageIndex - 1];
                }
                else
                {
                    nextPage = _currentPage;
                }

                if (isUpgrade && nextPage == WizardPage.SetupType && InstallProperties.IsUpgradeRadioSelection)
                {
                    nextPage = installationFlow[currentPageIndex - 2];
                }

                if ((InstallProperties.Clustered && !InstallProperties.IsActiveNode) && nextPage == WizardPage.TraceDirectory)
                {
                    nextPage = installationFlow[currentPageIndex - 2];
                }

                if (nextPage == WizardPage.NotSpecified)
                {
                    if (isUpgrade)
                    {
                        nextPage = WizardPage.UpgradeIntroduction;
                    }
                    else
                    {
                        nextPage = WizardPage.Introduction;
                    }
                }
            }

            if (_currentPage == WizardPage.UpgradeIntroduction && !InstallProperties.IsUpgradeRadioSelection)
            {
                isUpgrade = false;
                Constants.UserInstallType = InstallType.NotSpecified;
            }

            if (!_wizardPages.ContainsKey(nextPage))
                return;

            var wizardPage = _wizardPages[nextPage];
            if (direction == NavigationDirection.Next)
            {
                _previousPage = _currentPage;
                _visitedPages.Add(_currentPage);
            }
            _currentPage = nextPage;
            SetLeftPane(_currentPage);
            nextButton.Text = @"Next";
            nextButton.Width = 66;
            backButton.Location = new Point(512, 16);
            nextButton.Location = new Point(588, 16);

            wizardPanel.Controls.Clear();
            wizardPanel.Controls.Add(wizardPage.Value);

            if (!wizardPage.Value.IsInitialized)
                wizardPage.Value.Initialize();

            _isPreviousEnabled = backButton.Enabled;
            _isCancelEnabled = cancelButton.Enabled;
            _isNextEnabled = nextButton.Enabled;
            AllowNavigation(false);

            try
            {
                wizardPage.Value.OnNavigated(direction);
                wizardPage.Value.Focus();
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }

            AllowNavigation(true);

            switch (_currentPage)
            {
                case WizardPage.Introduction:
                    backButton.Visible = false;
                    nextButton.Visible = true;
                    nextButton.Disabled = false;
                    nextButton.Text = @"Next";
                    pictureInstallationHelp.Visible = false;
                    labelInstallationhelp.Visible = false;
                    testConnectionsButton.Visible = false;
                    break;
                case WizardPage.UpgradeIntroduction:
                    if ((InstallProperties.isCurrentVersionInstalled || InstallProperties.BlockInstallation) && !InstallProperties.IsFreshRadioSelection)
                    {
                        nextButton.Visible = false;
                        cancelButton.Text = @"Finish";
                        cancelButton.Visible = true;
                    }
                    else
                    {
                        nextButton.Visible = true;
                        nextButton.Disabled = InstallProperties.AgreeToLicense ? false : true;
                        nextButton.Text = @"Next";
                    }
                    backButton.Visible = false;
                    pictureInstallationHelp.Visible = false;
                    labelInstallationhelp.Visible = false;
                    testConnectionsButton.Visible = false;
                    break;

                case WizardPage.SetupType:
                    backButton.Visible = true;
                    pictureInstallationHelp.Visible = true;
                    labelInstallationhelp.Visible = true;
                    testConnectionsButton.Visible = false;
                    if (!InstallProperties.AgreeToLicense)
                        nextButton.Disabled = true;
                    break;

                case WizardPage.InstallationDirectory:
                    CleanupDirectories(false);
                    backButton.Visible = true;
                    testConnectionsButton.Visible = false;
                    break;

                case WizardPage.Repositories:
                    backButton.Visible = true;
                    testConnectionsButton.Visible = true;
                    break;

                case WizardPage.TraceDirectory:
                    backButton.Visible = true;
                    testConnectionsButton.Visible = false;
                    break;

                case WizardPage.ServiceAccount:
                    backButton.Visible = true;
                    testConnectionsButton.Visible = false;
                    break;

                case WizardPage.AgentSQLServer:
                    backButton.Visible = true;
                    testConnectionsButton.Visible = true;
                    break;

                case WizardPage.AgentCollectionServer:
                    backButton.Visible = true;
                    testConnectionsButton.Visible = false;
                    break;

                case WizardPage.Summary:
                    backButton.Visible = true;
                    testConnectionsButton.Visible = false;
                    if (InstallProperties.IsUpgradeRadioSelection)
                    {
                        nextButton.Text = "Upgrade";
                        nextButton.Width = 76;
                        backButton.Location = new Point(502, 16);
                        nextButton.Location = new Point(578, 16);
                    }
                    else
                    {
                        nextButton.Text = "Install";
                    }
                    break;

                case WizardPage.Install:
                    backButton.Visible = false;
                    nextButton.Visible = false;
                    testConnectionsButton.Visible = false;
                    cancelButton.Disabled = true;
                    pictureCloseIcon.Enabled = false;
                    break;

                case WizardPage.Success:
                    testConnectionsButton.Visible = false;
                    cancelButton.Visible = false;
                    backButton.Visible = false;
                    nextButton.Visible = true;
                    nextButton.Text = "Finish";
                    nextButton.Location = new Point(664, 16);
                    pictureCloseIcon.Enabled = true;
                    break;

                case WizardPage.Error:
                    testConnectionsButton.Visible = false;
                    cancelButton.Visible = false;
                    backButton.Visible = false;
                    nextButton.Visible = true;
                    nextButton.Text = "Finish";
                    nextButton.Location = new Point(664, 16);
                    pictureCloseIcon.Enabled = true;
                    break;

                default:
                    nextButton.Text = @"Next";
                    cancelButton.Enabled = true;
                    backButton.Visible = true;
                    backButton.Enabled = true;
                    break;
            }

            _isPreviousEnabled = backButton.Enabled;
            _isCancelEnabled = cancelButton.Enabled;
            _isNextEnabled = nextButton.Enabled;
        }

        public void NavigateToErrorPage(string errorMessage)
        {
            if (!string.IsNullOrEmpty(errorMessage))
            {
                ShowError(errorMessage);
            }

            NavigateToPage(NavigationDirection.Error);
        }


        private void AllowNavigation(bool isAllowed)
        {
            nextButton.Enabled = _isNextEnabled;
            backButton.Enabled = _isPreviousEnabled;
            cancelButton.Enabled = _isCancelEnabled;
            Cursor = isAllowed ? Cursors.Default : Cursors.WaitCursor;
            Application.DoEvents();
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            if (allowNavigation)
            {
                allowNavigation = false;
                if (_currentPage == WizardPage.SetupType && _visitedPages.Contains(WizardPage.UpgradeIntroduction))
                {
                    isUpgrade = true;
                }

                _visitedPages.Remove(_previousPage);
                _previousPage = _visitedPages.Last();
                if (_currentPage == WizardPage.Introduction)
                    return;

                NavigateToPage(NavigationDirection.Previous);
                allowNavigation = true;
            }
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            if (allowNavigation)
            {
                allowNavigation = false;
                NavigateToPage(NavigationDirection.Next);
                allowNavigation = true;
            }
        }

        private void SetLeftPane(WizardPage currentPage)
        {
            if (currentPage == WizardPage.Introduction)
            {
                panelMainSidePanel.Visible = false;
                panelSetupTypeSidePanel.Visible = false;
            }
            else if (currentPage == WizardPage.UpgradeIntroduction)
            {
                panelMainSidePanel.Visible = false;
                panelSetupTypeSidePanel.Visible = false;
            }
            else if (currentPage == WizardPage.SetupType)
            {
                panelMainSidePanel.Visible = false;
                panelSetupTypeSidePanel.Visible = true;
            }
            else if (_previousPage == WizardPage.SetupType || (isUpgrade && _previousPage == WizardPage.UpgradeIntroduction))
            {
                int currentPageIndex = 0;
                panelSetupTypeSidePanel.Visible = true;
                panelMainSidePanel.Visible = true;

                if (Constants.UserInstallType == InstallType.AgentAndDashboard)
                {
                    if (isUpgrade)
                    {
                        SetLeftPanelVisibility(Constants.AgentAndDashboardUpgradeFlow.Count - 4);
                        currentPageIndex = Constants.AgentAndDashboardUpgradeFlow.IndexOf(_currentPage) - 1;

                        this.labelLeftNavFirst.Text = Constants.NavigationPanelMap[WizardPage.ServiceAccount];
                        this.labelLeftNavSecond.Text = Constants.NavigationPanelMap[WizardPage.Summary];
                        this.labelLeftNavThird.Text = Constants.NavigationPanelMap[WizardPage.Install];
                    }
                    else
                    {
                        SetLeftPanelVisibility(Constants.AgentAndDashboardInstallationFlow.Count - 3);
                        currentPageIndex = Constants.AgentAndDashboardInstallationFlow.IndexOf(_currentPage);

                        this.labelLeftNavFirst.Text = Constants.NavigationPanelMap[WizardPage.SetupType];
                        this.labelLeftNavSecond.Text = Constants.NavigationPanelMap[WizardPage.InstallationDirectory];
                        this.labelLeftNavThird.Text = Constants.NavigationPanelMap[WizardPage.Repositories];
                        this.labelLeftNavFourth.Text = Constants.NavigationPanelMap[WizardPage.AgentSQLServer];
                        this.labelLeftNavFifth.Text = Constants.NavigationPanelMap[WizardPage.TraceDirectory];
                        this.labelLeftNavSixth.Text = Constants.NavigationPanelMap[WizardPage.AgentCollectionServer];
                        this.labelLeftNavSeventh.Text = Constants.NavigationPanelMap[WizardPage.ServiceAccount];
                        this.labelLeftNavEighth.Text = Constants.NavigationPanelMap[WizardPage.Summary];
                        this.labelLeftNavNineth.Text = Constants.NavigationPanelMap[WizardPage.Install];
                    }
                }
                else if (Constants.UserInstallType == InstallType.AgentOnly)
                {
                    if (isUpgrade)
                    {
                        SetLeftPanelVisibility(Constants.AgentUpgradeFlow.Count - 4);
                        currentPageIndex = Constants.AgentUpgradeFlow.IndexOf(_currentPage) - 1;

                        if (InstallProperties.IsMajorUpgrade)
                        {
                            this.labelLeftNavFirst.Text = Constants.NavigationPanelMap[WizardPage.ServiceAccount];
                            this.labelLeftNavSecond.Text = Constants.NavigationPanelMap[WizardPage.Summary];
                            this.labelLeftNavThird.Text = Constants.NavigationPanelMap[WizardPage.Install];
                        }
                        else
                        {
                            this.labelLeftNavFirst.Text = Constants.NavigationPanelMap[WizardPage.Summary];
                            this.labelLeftNavSecond.Text = Constants.NavigationPanelMap[WizardPage.Install];
                        }
                    }
                    else
                    {
                        SetLeftPanelVisibility(Constants.AgentInstallationFlow.Count - 3);
                        currentPageIndex = Constants.AgentInstallationFlow.IndexOf(_currentPage);

                        this.labelLeftNavFirst.Text = Constants.NavigationPanelMap[WizardPage.SetupType];
                        this.labelLeftNavSecond.Text = Constants.NavigationPanelMap[WizardPage.InstallationDirectory];
                        this.labelLeftNavThird.Text = Constants.NavigationPanelMap[WizardPage.AgentSQLServer];
                        this.labelLeftNavFourth.Text = Constants.NavigationPanelMap[WizardPage.TraceDirectory];
                        this.labelLeftNavFifth.Text = Constants.NavigationPanelMap[WizardPage.AgentCollectionServer];
                        this.labelLeftNavSixth.Text = Constants.NavigationPanelMap[WizardPage.ServiceAccount];
                        this.labelLeftNavSeventh.Text = Constants.NavigationPanelMap[WizardPage.Summary];
                        this.labelLeftNavEighth.Text = Constants.NavigationPanelMap[WizardPage.Install];
                    }
                }
                else if (Constants.UserInstallType == InstallType.ConsoleAndDashboard)
                {
                    if (isUpgrade)
                    {
                        SetLeftPanelVisibility(Constants.ConsoleAndDashboardUpgradeFlow.Count - 4);
                        currentPageIndex = Constants.ConsoleAndDashboardUpgradeFlow.IndexOf(_currentPage) - 1;

                        this.labelLeftNavFirst.Text = Constants.NavigationPanelMap[WizardPage.ServiceAccount];
                        this.labelLeftNavSecond.Text = Constants.NavigationPanelMap[WizardPage.Summary];
                        this.labelLeftNavThird.Text = Constants.NavigationPanelMap[WizardPage.Install];
                    }
                    else
                    {
                        SetLeftPanelVisibility(Constants.ConsoleAndDashboardInstallationFlow.Count - 3);
                        currentPageIndex = Constants.ConsoleAndDashboardInstallationFlow.IndexOf(_currentPage);

                        this.labelLeftNavFirst.Text = Constants.NavigationPanelMap[WizardPage.SetupType];
                        this.labelLeftNavSecond.Text = Constants.NavigationPanelMap[WizardPage.InstallationDirectory];
                        this.labelLeftNavThird.Text = Constants.NavigationPanelMap[WizardPage.Repositories];
                        this.labelLeftNavFourth.Text = Constants.NavigationPanelMap[WizardPage.ServiceAccount];
                        this.labelLeftNavFifth.Text = Constants.NavigationPanelMap[WizardPage.Summary];
                        this.labelLeftNavSixth.Text = Constants.NavigationPanelMap[WizardPage.Install];
                    }
                }
                else if (Constants.UserInstallType == InstallType.ConsoleOnly)
                {
                    if (isUpgrade)
                    {
                        SetLeftPanelVisibility(Constants.ConsoleUpgradeFlow.Count - 4);
                        currentPageIndex = Constants.ConsoleUpgradeFlow.IndexOf(_currentPage) - 1;

                        this.labelLeftNavFirst.Text = Constants.NavigationPanelMap[WizardPage.Summary];
                        this.labelLeftNavSecond.Text = Constants.NavigationPanelMap[WizardPage.Install];
                    }
                    else
                    {
                        SetLeftPanelVisibility(Constants.ConsoleInstallationFlow.Count - 3);
                        currentPageIndex = Constants.ConsoleInstallationFlow.IndexOf(_currentPage);

                        this.labelLeftNavFirst.Text = Constants.NavigationPanelMap[WizardPage.SetupType];
                        this.labelLeftNavSecond.Text = Constants.NavigationPanelMap[WizardPage.InstallationDirectory];
                        this.labelLeftNavThird.Text = Constants.NavigationPanelMap[WizardPage.Summary];
                        this.labelLeftNavFourth.Text = Constants.NavigationPanelMap[WizardPage.Install];
                    }
                }
                else if (Constants.UserInstallType == InstallType.DashboardOnly)
                {
                    if (isUpgrade)
                    {
                        SetLeftPanelVisibility(Constants.DashboardUpgradeFlow.Count - 4);
                        currentPageIndex = Constants.DashboardUpgradeFlow.IndexOf(_currentPage) - 1;

                        this.labelLeftNavFirst.Text = Constants.NavigationPanelMap[WizardPage.ServiceAccount];
                        this.labelLeftNavSecond.Text = Constants.NavigationPanelMap[WizardPage.Summary];
                        this.labelLeftNavThird.Text = Constants.NavigationPanelMap[WizardPage.Install];
                    }
                    else
                    {
                        SetLeftPanelVisibility(Constants.DashboardInstallationFlow.Count - 3);
                        currentPageIndex = Constants.DashboardInstallationFlow.IndexOf(_currentPage);

                        this.labelLeftNavFirst.Text = Constants.NavigationPanelMap[WizardPage.SetupType];
                        this.labelLeftNavSecond.Text = Constants.NavigationPanelMap[WizardPage.InstallationDirectory];
                        this.labelLeftNavThird.Text = Constants.NavigationPanelMap[WizardPage.Repositories];
                        this.labelLeftNavFourth.Text = Constants.NavigationPanelMap[WizardPage.ServiceAccount];
                        this.labelLeftNavFifth.Text = Constants.NavigationPanelMap[WizardPage.Summary];
                        this.labelLeftNavSixth.Text = Constants.NavigationPanelMap[WizardPage.Install];
                    }
                }
                else if (Constants.UserInstallType == InstallType.CMOnly)
                {
                    if (isUpgrade)
                    {
                        SetLeftPanelVisibility(Constants.CMUpgradeFlow.Count - 4);
                        currentPageIndex = Constants.CMUpgradeFlow.IndexOf(_currentPage) - 1;

                        if (InstallProperties.IsMajorUpgrade)
                        {
                            this.labelLeftNavFirst.Text = Constants.NavigationPanelMap[WizardPage.Repositories];
                            this.labelLeftNavSecond.Text = Constants.NavigationPanelMap[WizardPage.ServiceAccount];
                            this.labelLeftNavThird.Text = Constants.NavigationPanelMap[WizardPage.Summary];
                            this.labelLeftNavFourth.Text = Constants.NavigationPanelMap[WizardPage.Install];
                        }
                        else
                        {
                            this.labelLeftNavFirst.Text = Constants.NavigationPanelMap[WizardPage.Repositories];
                            this.labelLeftNavSecond.Text = Constants.NavigationPanelMap[WizardPage.ServiceAccount];
                            this.labelLeftNavThird.Text = Constants.NavigationPanelMap[WizardPage.Summary];
                            this.labelLeftNavFourth.Text = Constants.NavigationPanelMap[WizardPage.Install];
                        }
                    }
                    else
                    {
                        SetLeftPanelVisibility(Constants.CMInstallationFlow.Count - 3);
                        currentPageIndex = Constants.CMInstallationFlow.IndexOf(_currentPage);

                        this.labelLeftNavFirst.Text = Constants.NavigationPanelMap[WizardPage.SetupType];
                        this.labelLeftNavSecond.Text = Constants.NavigationPanelMap[WizardPage.DashboardDetection];
                        this.labelLeftNavThird.Text = Constants.NavigationPanelMap[WizardPage.InstallationDirectory];
                        this.labelLeftNavFourth.Text = Constants.NavigationPanelMap[WizardPage.Repositories];
                        this.labelLeftNavFifth.Text = Constants.NavigationPanelMap[WizardPage.TraceDirectory];
                        this.labelLeftNavSixth.Text = Constants.NavigationPanelMap[WizardPage.ServiceAccount];
                        this.labelLeftNavSeventh.Text = Constants.NavigationPanelMap[WizardPage.Summary];
                        this.labelLeftNavEighth.Text = Constants.NavigationPanelMap[WizardPage.Install];
                    }
                }
                else
                {
                    if (isUpgrade)
                    {
                        SetLeftPanelVisibility(Constants.FullUpgradeFlow.Count - 4);
                        currentPageIndex = Constants.FullUpgradeFlow.IndexOf(_currentPage) - 1;

                        this.labelLeftNavFirst.Text = Constants.NavigationPanelMap[WizardPage.Repositories];
                        this.labelLeftNavSecond.Text = Constants.NavigationPanelMap[WizardPage.ServiceAccount];
                        this.labelLeftNavThird.Text = Constants.NavigationPanelMap[WizardPage.Summary];
                        this.labelLeftNavFourth.Text = Constants.NavigationPanelMap[WizardPage.Install];
                    }
                    else
                    {
                        SetLeftPanelVisibility(Constants.FullInstallationFlow.Count - 3);
                        currentPageIndex = Constants.FullInstallationFlow.IndexOf(_currentPage);

                        this.labelLeftNavFirst.Text = Constants.NavigationPanelMap[WizardPage.SetupType];
                        this.labelLeftNavSecond.Text = Constants.NavigationPanelMap[WizardPage.InstallationDirectory];
                        this.labelLeftNavThird.Text = Constants.NavigationPanelMap[WizardPage.Repositories];
                        this.labelLeftNavFourth.Text = Constants.NavigationPanelMap[WizardPage.TraceDirectory];
                        this.labelLeftNavFifth.Text = Constants.NavigationPanelMap[WizardPage.ServiceAccount];
                        this.labelLeftNavSixth.Text = Constants.NavigationPanelMap[WizardPage.Summary];
                        this.labelLeftNavSeventh.Text = Constants.NavigationPanelMap[WizardPage.Install];
                    }
                }

                SetMainSidePanelControls(currentPageIndex);
            }
            else
            {
                int currentPageIndex = 0;

                if (Constants.UserInstallType == InstallType.AgentAndDashboard)
                {
                    if (isUpgrade)
                    {
                        currentPageIndex = Constants.AgentAndDashboardUpgradeFlow.IndexOf(_currentPage) - 1;
                    }
                    else
                    {
                        currentPageIndex = Constants.AgentAndDashboardInstallationFlow.IndexOf(_currentPage);
                    }
                }
                else if (Constants.UserInstallType == InstallType.AgentOnly)
                {
                    if (isUpgrade)
                    {
                        currentPageIndex = Constants.AgentUpgradeFlow.IndexOf(_currentPage) - 1;
                    }
                    else
                    {
                        currentPageIndex = Constants.AgentInstallationFlow.IndexOf(_currentPage);
                    }
                }
                else if (Constants.UserInstallType == InstallType.ConsoleAndDashboard)
                {
                    if (isUpgrade)
                    {
                        currentPageIndex = Constants.ConsoleAndDashboardUpgradeFlow.IndexOf(_currentPage) - 1;
                    }
                    else
                    {
                        currentPageIndex = Constants.ConsoleAndDashboardInstallationFlow.IndexOf(_currentPage);
                    }
                }
                else if (Constants.UserInstallType == InstallType.ConsoleOnly)
                {
                    if (isUpgrade)
                    {
                        currentPageIndex = Constants.ConsoleUpgradeFlow.IndexOf(_currentPage) - 1;
                    }
                    else
                    {
                        currentPageIndex = Constants.ConsoleInstallationFlow.IndexOf(_currentPage);
                    }
                }
                else if (Constants.UserInstallType == InstallType.DashboardOnly)
                {
                    if (isUpgrade)
                    {
                        currentPageIndex = Constants.DashboardUpgradeFlow.IndexOf(_currentPage) - 1;
                    }
                    else
                    {
                        currentPageIndex = Constants.DashboardInstallationFlow.IndexOf(_currentPage);
                    }
                }
                else if (Constants.UserInstallType == InstallType.CMOnly)
                {
                    if (isUpgrade)
                    {
                        currentPageIndex = Constants.CMUpgradeFlow.IndexOf(_currentPage) - 1;
                    }
                    else
                    {
                        currentPageIndex = Constants.CMInstallationFlow.IndexOf(_currentPage);
                    }
                }
                else
                {
                    if (isUpgrade)
                    {
                        currentPageIndex = Constants.FullUpgradeFlow.IndexOf(_currentPage) - 1;
                    }
                    else
                    {
                        currentPageIndex = Constants.FullInstallationFlow.IndexOf(_currentPage);
                    }
                }

                if (currentPageIndex > -1)
                {
                    SetMainSidePanelControls(currentPageIndex);
                }
            }
        }

        private void SetLeftPanelVisibility(int numberOfElements)
        {
            foreach (Control control in this.panelMainSidePanel.Controls)
            {
                if (control is PictureBox)
                {
                    int controlIndex = 0;
                    if (int.TryParse(control.Name, out controlIndex))
                    {
                        if (controlIndex > numberOfElements)
                        {
                            control.Hide();
                        }
                        else
                        {
                            control.Show();
                        }
                    }
                }
                if (control is IderaHeaderLabel)
                {
                    int controlIndex = 0;
                    if (int.TryParse(control.Name, out controlIndex))
                    {
                        if (controlIndex > numberOfElements)
                        {
                            control.Hide();
                        }
                        else
                        {
                            control.Show();
                        }
                    }
                }
            }
        }

        private void SetMainSidePanelControls(int currentPageIndex)
        {
            foreach (Control control in this.panelMainSidePanel.Controls)
            {
                if (control is PictureBox)
                {
                    int controlIndex = 0;
                    if (int.TryParse(control.Name, out controlIndex))
                    {
                        if (controlIndex < currentPageIndex)
                        {
                            ((PictureBox)control).Image = Resources.wizardimagescomplete;
                        }
                        else if (controlIndex == currentPageIndex)
                        {
                            ((PictureBox)control).Image = (Image)Resources.ResourceManager.GetObject("_" + controlIndex + "on", Properties.Resources.Culture);
                        }
                        else
                        {
                            ((PictureBox)control).Image = (Image)Resources.ResourceManager.GetObject("_" + controlIndex + "off", Properties.Resources.Culture);
                        }
                    }
                }
                if (control is IderaHeaderLabel)
                {
                    int controlIndex = 0;
                    if (int.TryParse(control.Name, out controlIndex))
                    {
                        if (controlIndex < currentPageIndex)
                        {
                            ((IderaHeaderLabel)control).ForeColor = Color.FromArgb(112, 194, 213);
                        }
                        else if (controlIndex == currentPageIndex)
                        {
                            ((IderaHeaderLabel)control).ForeColor = Color.FromArgb(0, 165, 219);
                        }
                        else
                        {
                            ((IderaHeaderLabel)control).ForeColor = Color.FromArgb(205, 205, 205);
                        }
                    }
                }
            }
        }

        internal void EnableNext(bool isAllowed)
        {
            nextButton.Enabled = isAllowed;
            nextButton.Disabled = false;
            _isNextEnabled = isAllowed;
        }

        internal void DisableNext(bool isAllowed)
        {
            nextButton.Disabled = isAllowed;
            _isNextEnabled = false;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            if (cancelButton.Text == "Finish")
            {
                ExitApplication();
            }
            else
            {
                IderaMessageBoxWithOption messageBox = new IderaMessageBoxWithOption();
                messageBox.SetButtonText("Resume", "Exit", "Exit Installer");
                messageBox.Show(_cancelMessage);
                if (messageBox.isFinishClick)
                {
                    CleanupDirectories();
                    ExitApplication();
                }
            }
        }

        internal void CleanupDirectories(bool isCancel = true)
        {
            try
            {
                if (!InstallProperties.IsUpgradeRadioSelection)
                {
                    foreach (string path in Constants.NewDirectoriesList.Values)
                    {
                        if (Directory.Exists(path))
                            Directory.Delete(path, true);
                    }
                }
            }
            catch
            {
                if (isCancel)
                {
                    string message = "An error occured while trying to cleanup installation directories.";
                    IderaMessageBox messageBox = new IderaMessageBox();
                    messageBox.Show(message);
                }
            }
        }

        internal void ExitApplication()
        {
            try
            {
                // Cleanup Installation files
                DirectoryInfo directoryInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
                foreach (FileInfo installationFile in directoryInfo.GetFiles())
                {
                    try
                    {
                        if (Constants.AllInstallationFiles.Contains(installationFile.Name))
                            installationFile.Delete();
                    }
                    catch
                    {
                        // Skip Failure
                        continue;
                    }
                }

                ProcessStartInfo Info = new ProcessStartInfo();
                Info.Arguments = "/C choice /C Y /N /D Y /T 3 & Del \"" +
                               Application.ExecutablePath + "\"";
                Info.WindowStyle = ProcessWindowStyle.Hidden;
                Info.CreateNoWindow = true;
                Info.FileName = "cmd.exe";
                Process.Start(Info);
            }
            catch
            {
                // skip, could not delete because in use.
            }

            customDropShadow.Close();
            customBorderShadow.Close();
            Application.Exit();
        }

        internal void SetCMandDashboardCredOk(bool _isCMCredOk, bool _isDahboardCredOk)
        {
            IsCMCredOk = _isCMCredOk;
            IsDahboardCredOk = _isDahboardCredOk;
        }

        private void testConnectionsButton_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            FormTestConnection formTestConnection = new FormTestConnection();
            if (Constants.UserInstallType == InstallType.AgentAndDashboard && _currentPage == WizardPage.Repositories)
            {
                formTestConnection.SetMessageVisibility(false, true);
            }
            else
            {
                formTestConnection.SetMessageVisibility(true, false);
            }

            formTestConnection.ShowDialog();
            this.Cursor = Cursors.Default;
        }

        private void pictureCloseIcon_Click(object sender, EventArgs e)
        {
            if (_currentPage == WizardPage.Success || _currentPage == WizardPage.Error)
            {
                ExitApplication();
            }
            else
            {
                IderaMessageBoxWithOption messageBox = new IderaMessageBoxWithOption();
                messageBox.SetButtonText("Resume", "Exit", "Exit Installer");
                messageBox.Show(_cancelMessage);
                if (messageBox.isFinishClick)
                {
                    CleanupDirectories();
                    ExitApplication();
                }
            }
        }
    }
}
