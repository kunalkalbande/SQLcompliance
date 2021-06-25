using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLCM_Installer
{
    public class SQLCMArguments : SilentInstallationArguments
    {
        public string AgreeToLicense
        {
            get
            {
                return GetValue("AGREETOLICENSE");
            }
            set
            {
                SetValue("AGREETOLICENSE", value);
            }
        }

        public string Encrypted
        {
            get
            {
                return GetValue("ENCRYPTED");
            }
            set
            {
                SetValue("ENCRYPTED", value);
            }
        }

        public string InstallDir
        {
            get
            {
                return GetValue("INSTALLDIR");
            }
            set
            {
                SetValue("INSTALLDIR", value);
            }
        }

        public string TraceDirCollect
        {
            get
            {
                return GetValue("TRACEDIR_COLLECT");
            }
            set
            {
                SetValue("TRACEDIR_COLLECT", value);
            }
        }

        public string ServiceUsername
        {
            get
            {
                return GetValue("SERVICEUSERNAME");
            }
            set
            {
                SetValue("SERVICEUSERNAME", value);
            }
        }

        public string ServicePassword
        {
            get
            {
                return GetValue("SERVICEPASSWORD");
            }
            set
            {
                SetValue("SERVICEPASSWORD", value);
            }
        }

        public string Repository
        {
            get
            {
                return GetValue("REPOSITORY");
            }
            set
            {
                SetValue("REPOSITORY", value);
            }
        }

        public string TraceDirAgent
        {
            get
            {
                return GetValue("TRACEDIR_AGENT");
            }
            set
            {
                SetValue("TRACEDIR_AGENT", value);
            }
        }

        public string CwfUrl
        {
            get
            {
                return GetValue("CWF_URL");
            }
            set
            {
                SetValue("CWF_URL", value);
            }
        }

        public string CwfToken
        {
            get
            {
                return GetValue("CWF_TOKEN");
            }
            set
            {
                SetValue("CWF_TOKEN", value);
            }
        }
        
        public string SQLServerAuthentication
        {
            get
            {
                return GetValue("IS_SQLSERVER_AUTHENTICATION");
            }
            set
            {
                SetValue("IS_SQLSERVER_AUTHENTICATION", value);
            }
        }

        public string SQLServerUsername
        {
            get
            {
                return GetValue("IS_SQLSERVER_USERNAME");
            }
            set
            {
                SetValue("IS_SQLSERVER_USERNAME", value);
            }
        }

        public string SQLServerPassword
        {
            get
            {
                return GetValue("IS_SQLSERVER_PASSWORD");
            }
            set
            {
                SetValue("IS_SQLSERVER_PASSWORD", value);
            }
        }

        public string SetupType
        {
            get
            {
                return GetValue("SETUPTYPE");
            }
            set
            {
                SetValue("SETUPTYPE", value);
            }
        }

        public string Clustered
        {
            get
            {
                return GetValue("CLUSTERED");
            }
            set
            {
                SetValue("CLUSTERED", value);
            }
        }
        
        public string Node
        {
            get
            {
                return GetValue("NODE");
            }
            set
            {
                SetValue("NODE", value);
            }
        }

        public string CollectionServer
        {
            get
            {
                return GetValue("COLLECTION_SERVER");
            }
            set
            {
                SetValue("COLLECTION_SERVER", value);
            }
        }
        
        public string Instance
        {
            get
            {
                return GetValue("INSTANCE");
            }
            set
            {
                SetValue("INSTANCE", value);
            }
        }
        
        public string ReInstall
        {
            get
            {
                return GetValue("REINSTALL");
            }
            set
            {
                SetValue("REINSTALL", value);
            }
        }

        public string ReInstallMode
        {
            get
            {
                return GetValue("REINSTALLMODE");
            }
            set
            {
                SetValue("REINSTALLMODE", value);
            }
        }

        public string UseExistingDatabase
        {
            get
            {
                return GetValue("USE_EXISTING_DATABASE");
            }
            set
            {
                SetValue("USE_EXISTING_DATABASE", value);
            }
        }

        public string UpgradeSchema
        {
            get
            {
                return GetValue("UPGRADE_SCHEMA");
            }
            set
            {
                SetValue("UPGRADE_SCHEMA", value);
            }
        }
        public string DatabaseExists
        {
            get
            {
                return GetValue("DATABASE_EXISTS");
            }
            set
            {
                SetValue("DATABASE_EXISTS", value);
            }
        }

        public string IsMajorUpgrade
        {
            get
            {
                return GetValue("IS_MAJOR_UPGRADE");
            }
            set
            {
                SetValue("IS_MAJOR_UPGRADE", value);
            }
        }

        public string IsUpgrade
        {
            get
            {
                return GetValue("IS_UPGRADE");
            }
            set
            {
                SetValue("IS_UPGRADE", value);
            }
        }

        public string SchemaValidation
        {
            get
            {
                return GetValue("SCHEMA_VALIDATION");
            }
            set
            {
                SetValue("SCHEMA_VALIDATION", value);
            }
        }
    }

    public class DashboardArguments : SilentInstallationArguments
    {
        public string InstallDir
        {
            get
            {
                return GetValue("INSTALLDIR");
            }
            set
            {
                SetValue("INSTALLDIR", value);
            }
        }

        public string AgreeToLicence
        {
            get
            {
                return GetValue("AGREETOLICENSE");
            }
            set
            {
                SetValue("AGREETOLICENSE", value);
            }
        }

        public string ServiceAccount
        {
            get
            {
                return GetValue("SERVICE_ACCOUNT");
            }
            set
            {
                SetValue("SERVICE_ACCOUNT", value);
            }
        }
        public string ServicePassword
        {
            get
            {
                return GetValue("SERVICE_PASSWORD");
            }
            set
            {
                SetValue("SERVICE_PASSWORD", value);
            }
        }
        public string RepositoryCoreDatabase
        {
            get
            {
                return GetValue("REPOSITORY_CORE_DATABASE");
            }
            set
            {
                SetValue("REPOSITORY_CORE_DATABASE", value);
            }
        }
        public string RepositoryInstance
        {
            get
            {
                return GetValue("REPOSITORY_INSTANCE");
            }
            set
            {
                SetValue("REPOSITORY_INSTANCE", value);
            }
        }
        public string WebAppPort
        {
            get
            {
                return GetValue("WEBAPP_PORT");
            }
            set
            {
                SetValue("WEBAPP_PORT", value);
            }
        }
        public string WebAppMonitorPort
        {
            get
            {
                return GetValue("WEBAPP_MONITOR_PORT");
            }
            set
            {
                SetValue("WEBAPP_MONITOR_PORT", value);
            }
        }
        public string WebAppSSLPort
        {
            get
            {
                return GetValue("WEBAPP_SSL_PORT");
            }
            set
            {
                SetValue("WEBAPP_SSL_PORT", value);
            }
        }
        public string CollectionPort
        {
            get
            {
                return GetValue("COLLECTION_PORT");
            }
            set
            {
                SetValue("COLLECTION_PORT", value);
            }
        }
        public string RepositorySQLAUTH
        {
            get
            {
                return GetValue("REPOSITORY_SQLAUTH");
            }
            set
            {
                SetValue("REPOSITORY_SQLAUTH", value);
            }
        }
        public string RepositorySQLUSERNAME
        {
            get
            {
                return GetValue("REPOSITORY_SQLUSERNAME");
            }
            set
            {
                SetValue("REPOSITORY_SQLUSERNAME", value);
            }
        }
        public string RepositorySQLPASSWORD
        {
            get
            {
                return GetValue("REPOSITORY_SQLPASSWORD");
            }
            set
            {
                SetValue("REPOSITORY_SQLPASSWORD", value);
            }
        }
    }

    public class SilentInstallationArguments : Dictionary<string, string>
    {
        public override string ToString()
        {
            StringBuilder command = new StringBuilder();
            foreach (KeyValuePair<string, string> argument in this)
            {
                if (!string.IsNullOrWhiteSpace(argument.Value))
                {
                    command.Append(argument.Key);
                    command.Append("=");
                    command.Append(@"""");
                        command.Append(argument.Value);
                    command.Append(@"""");
                    command.Append(" ");
                }
            }
            return command.ToString();
        }

        protected string GetValue(string key)
        {
            string val = string.Empty;
            this.TryGetValue(key, out val);
            return val;
        }

        protected void SetValue(string key, string val)
        {
            if (this.ContainsKey(key))
                this[key] = val;
            else
                this.Add(key, val);
        }
    }
}
