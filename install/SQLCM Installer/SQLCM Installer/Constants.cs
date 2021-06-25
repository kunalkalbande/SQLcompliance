using SQLCM_Installer.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SQLCM_Installer
{
    public static class Constants
    {
        public static Dictionary<Products, string> ProductMap = new Dictionary<Products, string>
        {
            {Products.Agent, "SQL Compliance Manager Agent"},
            {Products.Console, "SQL Compliance Manager Management Console"},
            {Products.Compliance, "SQL Compliance Manager"},
            {Products.Dashboard, "IDERA Dashboard"},
            {Products.NA, "N/A"}
        };

        public static Dictionary<string, string> NewDirectoriesList = new Dictionary<string, string>();
        #region Installation

        public static int RepositoryDBSchemaVersion = 2402;
        public static string DefaultDashboardProtocol = "http";
        public static string DefaultDashboardPort = "9292";
        public static Version DashboardVersion = Version.Parse("4.2.0.17");
        public static string InstalledDashboardVersion = string.Empty;
        public static string InstalledCMVersion = string.Empty;
        public static Version CWFRegistrySupportedVersion = Version.Parse("5.4.1.0");
        public static string CMRegistryPath = @"SOFTWARE\Idera\SQLCM";
        public static string CollectionRegistryPath = @"SOFTWARE\Idera\SQLCM\CollectionService";
        public static string CWFURLRegKey = "CwfUrl";
        public static string CWFTokenRegKey = "CwfToken";
        public static string CWFDisplayNameRegKey = "DisplayName";
        public static string CWFIsRegisteredRegKey = "IsRegisteredToDashboard";
        public static string CWFTableFullName = "[SQLcompliance].[dbo].[Cwf]";

        public static string CollectionServiceName = "SQLcomplianceCollectionService";
        public static string AgentServiceName = "SQLcomplianceAgent";
        public static string RegistrationServiceName = "SQLcomplianceRegistrationService";
        public static string AlertsServiceName = "SQLComplianceAlertsService";


        public const string ProductName = "SQLCM";
        public const string ProductDescription = "Low-Impact SQL server auditing for all user activity and data changes.";
        public const string ProductShortName = "SQLCM";
        public const string ProductVersion = "5.9.0.0";
        public const string Status = "Green";
        public const string InstallerType = "Production";

        internal const string CoreServiceUrl = "http://localhost:9292";
        internal const string RestUrl = "/sqlcm/v1";
        internal const string WebUrl = "/sqlcm";
        internal const string DefaultPage = "index";
        internal const string JarFile = "sqlcm-widgets-5.9.0.0.jar";
        internal const string WarFileName = "sqlcm-war-5.9.0.0.war";
        internal const string ZipFile = "SqlComplianceManager.zip";
        internal const string AssemblyFile = "SqlComplianceCwfAddin.dll";
        internal const string ServicePort = "9292";
        internal const string IsRegisteredToDashboard = "True";
        internal const Boolean IsWarEnabled = true;

        #endregion

        #region Messages
        public static string AgentTrace = "Agent trace directory";
        public static string CollectionTrace = "Collection trace directory";
        public static string InstallationDir = "Installation directory";
        public static string InstallCMOnlyInstallDirScreen = "Where do you want " + Constants.ProductMap[Products.Compliance] + " installed?";
        public static string InstallConsoleOnlyInstallDirScreen = "Where do you want SQL Compliance Manager Management Console installed?";
        public static string InstallAgentOnlyInstallDirScreen = "Where do you want " + Constants.ProductMap[Products.Agent] + " installed?";
        public static string InstallDashboardOnlyInstallDirScreen = "Where do you want " + Constants.ProductMap[Products.Dashboard] + " installed?";
        public static string InstallCMAndDashboardInstallDirScreen = "Where do you want " + Constants.ProductMap[Products.Compliance] + " and " + Constants.ProductMap[Products.Dashboard] + " installed?";
        public static string InstallConsoleAndDashboardInstallDirScreen = "Where do you want " + Constants.ProductMap[Products.Console] + " and IDERA \r\nDashboard installed?";
        public static string InstallAgentAndDashboardInstallDirScreen = "Where do you want " + Constants.ProductMap[Products.Agent] + " and " + Constants.ProductMap[Products.Dashboard] + " installed?";
        public static string InstallDashboardOnlyRepositoryScreen = "What Instance and Authentication do you want to use to create  the IDERA \r\nDashboard repository?";
        public static string InstallCMAndDashboardRespositoryScreen = "Do you want to enable a Clustered Environment?  What Instance and \r\nAuthentication do you want to use to create repositories?";
        public static string InstallCMOnlyTitleServiceAccountScreen = "The " + Constants.ProductMap[Products.Compliance] + " services will use the Windows credentials\r\n specified below.";
        public static string InstallDashboardOnlyTitleServiceAccountScreen = "The " + Constants.ProductMap[Products.Dashboard] + " services will use the Windows credentials specified below.";
        public static string InstallCMandDashboardTitleServiceAccountScreen = "The " + Constants.ProductMap[Products.Compliance] + " and " + Constants.ProductMap[Products.Dashboard] + " services will use the \r\nWindows credentials specified below.";
        public static string InstallAgentandDashboardTitleServiceAccountScreen = "The " + Constants.ProductMap[Products.Agent] + " and " + Constants.ProductMap[Products.Dashboard] + " services will use the \r\nWindows credentials specified below.";
        public static string InstallAgentOnlyTitleServiceAccountScreen = "The " + Constants.ProductMap[Products.Agent] + " services will use the Windows credentials\r\n specified below.";
        public static string CMInstallDirEmptyError = "The " + Constants.ProductMap[Products.Compliance] + " installation directory field cannot be empty.";
        public static string DashboardInstallDirEmptyError = "The " + Constants.ProductMap[Products.Dashboard] + " installation directory field cannot be empty.";
        public static string CMInstallDirError = "The entered " + Constants.ProductMap[Products.Compliance] + " installation directory cannot be created on this machine due to insufficient permissions. Please specify a different directory path.";
        public static string DashboardInstallDirError = "The entered " + Constants.ProductMap[Products.Dashboard] + " installation directory cannot be created on this machine due to insufficient permissions. Please specify a different directory path.";
        public static string InvalidDisplayNameError = "Display name for product installation is either empty or having\r\ninvalid characters. It can only contain letters, numbers or hyphen(-) character.";
        public static string InvalidRemoteDashboardValues = "The IDERA Dashboard you are trying to connect is either not accessible or the values entered for connecting to remote Dashboard are not valid.";
        public static string CMSQLServerConnectionOK = "Test Connection successful for " + Constants.ProductMap[Products.Compliance] + " Repository.";
        public static string CMSQLServerConnectionError = "Test Connection failed for " + Constants.ProductMap[Products.Compliance] + " Repository. \r\nPlease check Server Instance and Authentication.";
        public static string DashboardSQLServerConnectionOK = "Test Connection successful for " + Constants.ProductMap[Products.Dashboard] + " Repository.";
        public static string DashboardSQLServerConnectionError = "Test Connection failed for " + Constants.ProductMap[Products.Dashboard] + " Repository. \r\nPlease check Server Instance and Authentication.";
        public static string ServiceAccountCredError = "The service credentials are not valid. Please provide correct credentials.";
        public static string ServiceAccountCredNotEntered = "Please enter Username and Password";
        public static string CollectionTraceDirError = "The entered Collection Service Trace directory cannot be created on this machine due to insufficient permissions. Please specify a different directory path.";
        public static string AgentTraceDirError = "The entered Agent Trace directory cannot be created on this machine due to insufficient permissions. Please specify a different directory path.";
        public static string CollectionServiceError = "The setup program found the Server \"%MachinePlaceholder%\" but could not connect to the Collection Service. Ensure the Collection Service is running on the computer you specified. \r\n\r\nDo you want to specify a different Collection Service computer?";
        public static string CollectionServiceMachineError = "The Server \"%MachinePlaceholder%\" was not found.\r\n"
                                                                + "This could be due to a number of factors:\r\n"
                                                                + "      1. The logon account does not have access to connect to server remotely.\r\n"
                                                                + "      2. The Collection Service computer is in another domain.\r\n"
                                                                + "      3. The name you specified for the Collection Service computer is invalid.\r\n"
                                                                + "      4. The Collection Service computer is unavailable at this time.\r\n"
                                                                + "\r\nDo you want to specify a different Collection Service computer?\r\n";
        public static string SilentAgentInstalled = "The " + Constants.ProductMap[Products.Agent] + " was installed on this system by SQLcomplianceAgent-x64.msi and can not be upgraded by the current installer. Please refer to this link"
                                                    + " for the steps necessary to upgrade the agent.";
        public static string SilentAgentInstalledx86 = "The " + Constants.ProductMap[Products.Agent] + " was installed on this system by SQLcomplianceAgent.msi and can not be upgraded by the current installer. Please refer to this link"
                                                    + " for the steps necessary to upgrade the agent.";
        public static string DashboardURLEmpty = "The " + Constants.ProductMap[Products.Dashboard] + " URL cannot be null or empty.";
        public static string DashboardURLInvalid = "The " + Constants.ProductMap[Products.Dashboard] + " URL is in invalid format.";
        public static string DashboardUserInvalid = "The " + Constants.ProductMap[Products.Dashboard] + " User cannot be null or empty.";
        public static string DashboardPasswordInvalid = "The " + Constants.ProductMap[Products.Dashboard] + " Password cannot be null or empty.";
        public static string DashboardUriInvalid = "The " + Constants.ProductMap[Products.Dashboard] + " url is either empty or is in invalid format. Please provide URL in below format:\r\nhttp://<hostname>:<port>/";
        public static string DashboardVersionError = "The Dashboard on remote machine: {0} is not compatible with this version of SQL CM. Either select \"no\" to register the Dashboard later, point to a compatible version of the Dashboard, or exit this installation and upgrade the Dashboard on {0} before continuing.";
        public static string RegistrationFailureMessage = "An error occurred while registering SQL Compliance Manager to the IDERA Dashboard.";
        public static string InstallationFailureMessage = "The installation was interrupted before {0} could be completely installed.\r\n\r\nTo complete the installation at another time, please run setup again.\r\n\r\nClick Finish to exit the installer.";
        public static string FolderAccessErrorMessage = "The folder access could not be provided to the following folders. The installation will continue however, you will have to manually provide access to these folders after installation.\r\n";
        public static string RemoteDashboardWarningMessage = "You have selected to install Dashboard on " + Environment.MachineName + " and opted to register SQL Compliance Manager with the Remote Dashboard.\r\nIf you continue with this selection, Dashboard will be installed on this machine but SQL Compliance Manager will get registered with Remote Dashboard.\r\n\r\nIf you do not want to install Dashboard, go back to 'Install Features' screen and un-check IDERA Dashboard.";
        public static string DirectoryPathError = "Ensure the specified path contains a drive letter (C:\\).";

        #endregion

        #region Install Sequence

        public static InstallType UserInstallType = InstallType.NotSpecified;
        public static InstallType UserCurrentInstallation = InstallType.NotSpecified;
        public static Products FailedProduct = Products.NA;

        public static List<WizardPage> CMInstallationFlow = new List<WizardPage> { WizardPage.Introduction, 
            WizardPage.SetupType, 
            WizardPage.DashboardDetection,
            WizardPage.InstallationDirectory,
            WizardPage.Repositories,
            WizardPage.TraceDirectory,
            WizardPage.ServiceAccount, 
            WizardPage.Summary,
            WizardPage.Install,
            WizardPage.Success,
            WizardPage.Error
        };

        public static List<WizardPage> FullInstallationFlow = new List<WizardPage> { WizardPage.Introduction, 
            WizardPage.SetupType, 
            WizardPage.InstallationDirectory,
            WizardPage.Repositories,
            WizardPage.TraceDirectory,
            WizardPage.ServiceAccount, 
            WizardPage.Summary,
            WizardPage.Install,
            WizardPage.Success,
            WizardPage.Error
        };

        public static List<WizardPage> DashboardInstallationFlow = new List<WizardPage> { WizardPage.Introduction, 
            WizardPage.SetupType,
            WizardPage.InstallationDirectory,
            WizardPage.Repositories,
            WizardPage.ServiceAccount,
            WizardPage.Summary,
            WizardPage.Install,
            WizardPage.Success,
            WizardPage.Error
        };

        public static List<WizardPage> AgentInstallationFlow = new List<WizardPage> { WizardPage.Introduction, 
            WizardPage.SetupType, 
            WizardPage.InstallationDirectory,
            WizardPage.AgentSQLServer,
            WizardPage.TraceDirectory,
            WizardPage.AgentCollectionServer,
            WizardPage.ServiceAccount,
            WizardPage.Summary,
            WizardPage.Install,
            WizardPage.Success,
            WizardPage.Error
        };

        public static List<WizardPage> ConsoleInstallationFlow = new List<WizardPage> { WizardPage.Introduction, 
            WizardPage.SetupType, 
            WizardPage.InstallationDirectory, 
            WizardPage.Summary,
            WizardPage.Install,
            WizardPage.Success,
            WizardPage.Error
        };

        public static List<WizardPage> ConsoleAndDashboardInstallationFlow = new List<WizardPage> { WizardPage.Introduction, 
            WizardPage.SetupType, 
            WizardPage.InstallationDirectory,
            WizardPage.Repositories,
            WizardPage.ServiceAccount,
            WizardPage.Summary,
            WizardPage.Install,
            WizardPage.Success,
            WizardPage.Error
        };

        public static List<WizardPage> AgentAndDashboardInstallationFlow = new List<WizardPage> { WizardPage.Introduction, 
            WizardPage.SetupType, 
            WizardPage.InstallationDirectory,
            WizardPage.Repositories,
            WizardPage.AgentSQLServer,
            WizardPage.TraceDirectory,
            WizardPage.AgentCollectionServer,
            WizardPage.ServiceAccount,
            WizardPage.Summary,
            WizardPage.Install,
            WizardPage.Success,
            WizardPage.Error
        };

        public static List<WizardPage> AgentUpgradeFlow = new List<WizardPage> { WizardPage.UpgradeIntroduction, 
            WizardPage.SetupType,
            WizardPage.ServiceAccount,
            WizardPage.Summary,
            WizardPage.Install,
            WizardPage.Success,
            WizardPage.Error
        };

        public static List<WizardPage> ConsoleUpgradeFlow = new List<WizardPage> { WizardPage.UpgradeIntroduction, 
            WizardPage.SetupType,
            WizardPage.Summary,
            WizardPage.Install,
            WizardPage.Success,
            WizardPage.Error
        };

        public static List<WizardPage> CMUpgradeFlow = new List<WizardPage> { WizardPage.UpgradeIntroduction, 
            WizardPage.SetupType,
            WizardPage.Repositories,
            WizardPage.ServiceAccount,
            WizardPage.Summary,
            WizardPage.Install,
            WizardPage.Success,
            WizardPage.Error
        };

        public static List<WizardPage> DashboardUpgradeFlow = new List<WizardPage> { WizardPage.UpgradeIntroduction, 
            WizardPage.SetupType,
            WizardPage.ServiceAccount,
            WizardPage.Summary,
            WizardPage.Install,
            WizardPage.Success,
            WizardPage.Error
        };

        public static List<WizardPage> AgentAndDashboardUpgradeFlow = new List<WizardPage> { WizardPage.UpgradeIntroduction, 
            WizardPage.SetupType,
            WizardPage.ServiceAccount,
            WizardPage.Summary,
            WizardPage.Install,
            WizardPage.Success,
            WizardPage.Error
        };

        public static List<WizardPage> ConsoleAndDashboardUpgradeFlow = new List<WizardPage> { WizardPage.UpgradeIntroduction, 
            WizardPage.SetupType,
            WizardPage.ServiceAccount,
            WizardPage.Summary,
            WizardPage.Install,
            WizardPage.Success,
            WizardPage.Error
        };

        public static List<WizardPage> FullUpgradeFlow = new List<WizardPage> { WizardPage.UpgradeIntroduction, 
            WizardPage.SetupType,
            WizardPage.Repositories,
            WizardPage.ServiceAccount,
            WizardPage.Summary,
            WizardPage.Install,
            WizardPage.Success,
            WizardPage.Error
        };

        public static Dictionary<WizardPage, string> NavigationPanelMap = new Dictionary<WizardPage, string>
        {
            {WizardPage.SetupType, "Install Features"},
            {WizardPage.InstallationDirectory, "Installation Directory"},
            {WizardPage.Repositories, "Repositories"},
            {WizardPage.TraceDirectory, "Trace Directory"},
            {WizardPage.ServiceAccount,  "Service Account"},
            {WizardPage.AgentSQLServer, "Register SQL Server"},
            {WizardPage.AgentCollectionServer, "Collection Service \r\nLocation"},
            {WizardPage.Summary, "Summary"},
            {WizardPage.Install, "Install"},
            {WizardPage.Success, "Success"},
            {WizardPage.Error, "Error"},
            {WizardPage.DashboardDetection, "Dashboard Registration"}
        };

        public static Dictionary<InstalledProducts, string> ProductInstalledNameMap = new Dictionary<InstalledProducts, string>
        {
            {InstalledProducts.AgentX64, "IDERA SQLCOMPLIANCE AGENT (X64)"},
            {InstalledProducts.AgentX86, "IDERA SQLCOMPLIANCE AGENT"},
            {InstalledProducts.ComplianceX64, "IDERA SQL COMPLIANCE MANAGER (X64)"},
            {InstalledProducts.ComplianceX86, "IDERA SQL COMPLIANCE MANAGER"},
            {InstalledProducts.DashboardX64, "IDERA DASHBOARD"},
            {InstalledProducts.DashboardX86, "IDERA DASHBOARD"}
        };

        #endregion

        #region Fonts
        public static PrivateFontCollection fontCollection;
        public static FontFamily SourceSansProRegular;
        public static FontFamily SourceSansProBold;
        public static FontFamily SourceSansProSemiBold;

        public static void InitializePrivateFonts()
        {
            fontCollection = new PrivateFontCollection();
            fontCollection.AddFontFile("SourceSansPro-Regular.ttf");
            SourceSansProRegular = fontCollection.Families[0];

            fontCollection = null;
            fontCollection = new PrivateFontCollection();
            fontCollection.AddFontFile("SourceSansPro-SemiBold.ttf");
            SourceSansProSemiBold = fontCollection.Families[0];

            fontCollection = null;
            fontCollection = new PrivateFontCollection();
            fontCollection.AddFontFile("SourceSansPro-Bold.ttf");
            SourceSansProBold = fontCollection.Families[0];
        }
        #endregion

        #region Return Codes

        public static Dictionary<int, string> ReturnCodes = new Dictionary<int, string>() 
        {
            {0, "The action completed successfully."},
            {13, "The data is invalid."},
            {87, "One of the parameters was invalid."},
            {120	,"This value is returned when a custom action attempts to call a function that cannot be called from custom actions. The function returns the value ERROR_CALL_NOT_IMPLEMENTED. Available beginning with Windows Installer version 3.0."},
            {1259, "If Windows Installer determines a product may be incompatible with the current operating system, it displays a dialog box informing the user and asking whether to try to install anyway. This error code is returned if the user chooses not to try the installation."},
            {1601, "The Windows Installer service could not be accessed. Contact your support personnel to verify that the Windows Installer service is properly registered."},
            {1602, "The user cancels installation."},
            {1603, "A fatal error occurred during installation."},
            {1604, "Installation suspended, incomplete."},
            {1605, "This action is only valid for products that are currently installed."},
            {1606, "The feature identifier is not registered."},
            {1607, "The component identifier is not registered."},
            {1608, "This is an unknown property."},
            {1609, "The handle is in an invalid state."},
            {1610, "The configuration data for this product is corrupt. Contact your support personnel."},
            {1611, "The component qualifier not present."},
            {1612, "The installation source for this product is not available. Verify that the source exists and that you can access it."},
            {1613, "This installation package cannot be installed by the Windows Installer service. You must install a Windows service pack that contains a newer version of the Windows Installer service."},
            {1614, "The product is uninstalled."},
            {1615, "The SQL query syntax is invalid or unsupported."},
            {1616, "The record field does not exist."},
            {1618, "Another installation is already in progress. Complete that installation before proceeding with this install."},
            {1619, "This installation package could not be opened. Verify that the package exists and is accessible, or contact the application vendor to verify that this is a valid Windows Installer package."},
            {1620, "This installation package could not be opened. Contact the application vendor to verify that this is a valid Windows Installer package."},
            {1621, "There was an error starting the Windows Installer service user interface. Contact your support personnel."},
            {1622, "There was an error opening installation log file. Verify that the specified log file location exists and is writable."},
            {1623, "This language of this installation package is not supported by your system."},
            {1624, "There was an error applying transforms. Verify that the specified transform paths are valid."},
            {1625, "This installation is forbidden by system policy. Contact your system administrator."},
            {1626, "The function could not be executed."},
            {1627, "The function failed during execution."},
            {1628, "An invalid or unknown table was specified."},
            {1629, "The data supplied is the wrong type."},
            {1630, "Data of this type is not supported."},
            {1631, "The Windows Installer service failed to start. Contact your support personnel."},
            {1632, "The Temp folder is either full or inaccessible. Verify that the Temp folder exists and that you can write to it."},
            {1633, "This installation package is not supported on this platform. Contact your application vendor."},
            {1634, "Component is not used on this machine."},
            {1635, "This patch package could not be opened. Verify that the patch package exists and is accessible, or contact the application vendor to verify that this is a valid Windows Installer patch package."},
            {1636, "This patch package could not be opened. Contact the application vendor to verify that this is a valid Windows Installer patch package."},
            {1637, "This patch package cannot be processed by the Windows Installer service. You must install a Windows service pack that contains a newer version of the Windows Installer service."},
            {1638, "Another version of this product is already installed. Installation of this version cannot continue. To configure or remove the existing version of this product, use Add/Remove Programs in Control Panel."},
            {1639, "Invalid command line argument. Consult the Windows Installer SDK for detailed command-line help."},
            {1640, "The current user is not permitted to perform installations from a client session of a server running the Terminal Server role service."},
            {1641, "The installer has initiated a restart. This message is indicative of a success."},
            {1642, "The installer cannot install the upgrade patch because the program being upgraded may be missing or the upgrade patch updates a different version of the program. Verify that the program to be upgraded exists on your computer and that you have the correct upgrade patch."},
            {1643, "The patch package is not permitted by system policy."},
            {1644, "One or more customizations are not permitted by system policy."},
            {1645, "Windows Installer does not permit installation from a Remote Desktop Connection."},
            {1646, "The patch package is not a removable patch package. Available beginning with Windows Installer version 3.0."},
            {1647, "The patch is not applied to this product. Available beginning with Windows Installer version 3.0."},
            {1648, "No valid sequence could be found for the set of patches. Available beginning with Windows Installer version 3.0."},
            {1649, "Patch removal was disallowed by policy. Available beginning with Windows Installer version 3.0."},
            {1650, "The XML patch data is invalid. Available beginning with Windows Installer version 3.0."},
            {1651, "Administrative user failed to apply patch for a per-user managed or a per-machine application that is in advertise state. Available beginning with Windows Installer version 3.0."},
            {1652, "Windows Installer is not accessible when the computer is in Safe Mode. Exit Safe Mode and try again or try using System Restore to return your computer to a previous state. Available beginning with Windows Installer version 4.0."},
            {1653, "Could not perform a multiple-package transaction because rollback has been disabled. Multiple-Package Installations cannot run if rollback is disabled. Available beginning with Windows Installer version 4.5."},
            {1654, "The app that you are trying to run is not supported on this version of Windows. A Windows Installer package, patch, or transform that has not been signed by Microsoft cannot be installed on an ARM computer."},
            {3010, "A restart is required to complete the install. This message is indicative of a success. This does not include installs where the ForceReboot action is run."}
        };

        #endregion

        #region Installation Files

        public static string[] AllInstallationFiles = { "IderaDashboard.msi", 
                                                        "SQLcomplianceAgent-x64.msi", 
                                                        "SQLcomplianceClusterSetup-x64.exe",
                                                        "SqlCompliancemanager.zip",
                                                        "SQLcompliance-x64.exe",
                                                        "SQLCMInstall.exe",
                                                        "CWFInstallerService.dll",
                                                        "SQLcompliance.exe",
                                                        "SQLcomplianceAgent.msi",
                                                        "SQLcomplianceClusterSetup.exe",
                                                        "SourceSansPro-Regular.ttf",
                                                        "SourceSansPro-SemiBold.ttf",
                                                        "SourceSansPro-Bold.ttf",
                                                        "sqlncli.msi",
                                                        "sqlncli-x64.msi" };

        #endregion

        public static void SetDirectoryList(string name, string path){
            if(string.IsNullOrEmpty(path))
                return;
            if (NewDirectoriesList.ContainsKey(name))
            {
                try
                {
                    if (Directory.Exists(NewDirectoriesList[name]))
                    {
                        Directory.Delete(NewDirectoriesList[name],true);
                    }
                }
                catch 
                { 
                    //Ignore error
                }
            }
            string tempPath = "";
            string parent =  path;
            while (true)
            {
                try
                {
                    if (Directory.Exists(parent) && !string.IsNullOrEmpty(tempPath))
                    {
                        NewDirectoriesList[name] = tempPath;
                        return;
                    }
                    if (!Directory.Exists(parent))
                        tempPath = parent;
                    if (Directory.GetParent(parent) != null)
                        parent = Directory.GetParent(parent).FullName;
                    else
                        break;
                }
                catch
                {
                    //ignore error
                }
            }
        }

        #region logfiles
        public const string SQLCMRevisionLogFile = "\\CMRevisionLog.log";
        public const string SQLAgenRevisionLogFile = "\\AgentRevisionLog.log";
        public const string SQLDashboardRevisionLogFile = "\\DashboardRevisionLog.log";
        #endregion

        public static bool isDeleteAction = false;
    }
}
