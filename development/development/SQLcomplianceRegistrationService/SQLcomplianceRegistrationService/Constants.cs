using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLcomplianceRegistrationService
{
    public static class Constants
    {
        public static string CoreServicesUrl = "http://localhost:9292";
        public static string UserDomainName = Environment.UserDomainName;
        public static string UserName = Environment.UserName;
        public static string ServiceAdminUser = UserDomainName + @"\" + UserName;
        //We will get these values from Registry.
        public static string ServiceAdminPassword = "pass@123";
        public static string Instance = @"SQLCM-AJ\SQL-AJ2014";
        public static string SQLcmSQLServerName = @"SQLCM-AJ\SQL-AJ2014";
        public static int ProductId = 0;
        public static bool IsRegistered = false;
        public static string SQLcmRepository = "SQLcompliance";
        public static string ProductName = "SQLCM";
        public static string ShortName = "SQLCM";
        public static string ProductVersion = "5.9.0.0";
        internal const string JarFile = "sqlcm-widgets-5.9.0.0.jar";
        internal const string WarFileName = "sqlcm-war-5.9.0.0.war";
        public static string RestUrl = "/sqlcm/v1";
        public static string WebUrl = "/sqlcm";
        public static string DefaultPage = "index";
        public static string AssemblyFile = "SqlComplianceCwfAddin.dll";
        public static string Description = "SQLcomplianceRegistrationService for Register a Product";
        public static string Status = "Green";
        internal const Boolean IsWarEnabled = true;
        public static bool IsDeveloperMode = true;

        public static string ZipFile = "SqlComplianceManager.zip";

        public static int IsTaggable = 0;

        public static string DashboardHost { get; set; }

        public static string DashboardPort { get; set; }

        public static object ServicePort { get; set; }
    }
}
