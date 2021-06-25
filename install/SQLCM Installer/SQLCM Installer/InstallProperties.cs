using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLCM_Installer
{
    public class InstallProperties
    {
        public static bool AgreeToLicense = false;
        public static string CMInstallDir;
        public static string DashboardInstallDir;
        public static string CMDisplayName;
        public static bool IsActiveNode;
        public static string CMSQLServerInstanceName;
        public static bool IsCMSQLServerSQLAuth;
        public static string CMSQLServerUsername = "";
        public static string CMSQLServerPassword = "";
        public static string DashboardSQLServerInstanceName;
        public static bool IsDashboardSQLServerSQLAuth;
        public static string DashboardSQLServerUsername = "";
        public static string DashboardSQLServerPassword = "";
        public static string ServiceUserName;
        public static string ServicePassword;
        public static string AgentCollectionServer;
        public static bool RegisterProductToDashboard = false;
        public static bool IsUpgradeRadioSelection = false;
        public static bool IsFreshRadioSelection = false;
        public static bool isCurrentVersionInstalled = false;
        public static bool isDashboardCurrentVersionInstalled = false;
        public static bool isCMCurrentVersionInstalled = false;
        public static bool isSilentAgentInstalled = false;
        public static bool BlockInstallation = false;
        public static string AgentTraceDirectory;
        public static string CollectionTraceDirectory;
        public static bool Clustered;
        public static string SQLServerUserName;
        public static bool UpgradeSchema = false;
        public static bool IsMajorUpgrade = false;
    }
}
