using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLcomplianceCwfAddin.RestService
{
    public class RestServiceConstants
    {
        public const string ProductName = "SQLCM";
        public const string ProductDescription = "Low-Impact SQL server auditing for all user activity and data changes.";
        public const string ProductShortName = "SQLCM";
        public const string ProductVersion = "5.9.0.0";
        public const string Status = "Green";
        public const string InstallerType = "Production";

        internal const string AddinProjectName = "SqlComplianceCwfAddin";
        internal const string CoreServiceUrl = "http://localhost:9292";
        internal const string RestUrl = "/sqlcm/v1";
        internal const string WebUrl = "/sqlcm";
        internal const string DefaultPage = "index";
        internal const string JarFile = "sqlcm-5.9.0.0.jar";
        internal const string ZipFile = "SqlComplianceManager.zip";
        internal const string AssemblyFile = "SqlComplianceCwfAddin.dll";
		internal const string ServicePort = "9292";
        internal const string IsRegisteredToDashboard = "True";

        internal const string Info_UpgradeComplete = "The SQLcompliance Agent for this SQL Server has been successfully upgraded.";
        internal const string Error_UpgradeFailedNoInfo = "An error occurred during verifying a version of the upgraded SQLcompliance Agent for the selected SQL Server. Please check the application event logs for more information.";

        internal const string AgentService = "SQLcomplianceAgent";
        internal const string CollectionService = "SQLcomplianceCollectionService";
    }
}
