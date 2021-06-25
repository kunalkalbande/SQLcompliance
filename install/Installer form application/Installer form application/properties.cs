using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Installer_form_application
{
    public static class properties
    {
        public static string dashboardUrl = "http://localhost:9292";
        public static string serviceUsername = string.Empty;
        public static string servicePassword = string.Empty;
        public static bool localInstall = true;
        public static bool upgrading = false;
        public static string IDPath = @"C:\Program Files\Idera\Dashboard";
        public static string IDAccount = string.Empty;
        public static string IDServicePort = string.Empty;
        public static string IDVersion = string.Empty;
        public static bool installDashboard = true;
        public static string SPPath = @"C:\Program Files\Idera\SQLCompliance";
        public static string DisplayName = "FirstInstance";
        public static string IDSUsername = string.Empty;
        public static string IDSPassword = string.Empty;
        public static string SPSPassword = string.Empty;
        public static string SPSUsername = string.Empty;
        public static string RemoteUsername = string.Empty;
        public static string RemotePassword = string.Empty;
        public static string RemoteHostname = string.Empty;
        public static string CoreServicesPort = "9292";
        public static string WebAppMonitorPort = "9094";
        public static string WebAppServicePort = "9290";
        public static string WebAppSSLPort = "9291";
        public static string IDDBName = "IderaDashboardRepository";
        public static string IDInstance = "(local)";
        public static string SQLUsernameID = string.Empty;
        public static string SQLPasswordID = string.Empty;
        public static bool SQLAUTHID = false;
        public static bool isTaggableSampleProduct = false;
        public static string CMInstance = "(local)";
        public static string DMDBName = "DiagnosticManagerRepository";
        public static string JMInstance = "(local)";
        public static string CMDBName = "SQLcompliance";
        public static bool SQLAUTH2 = false;
        public static string SQLUsername2 = string.Empty;
        public static string SQLPassword2 = string.Empty;
        public static bool AGREETOLICENSE = false;
        public static bool dbExists = false;
        public static bool isSQLCMUpgrade = false;
        public static string oldSQLCMVersion = string.Empty;
        public static Version installedSQLCMVersion = null;
    }
}
