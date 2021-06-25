using Idera.SQLcompliance.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SQLcomplianceCwfAddin.RestService.ServiceContracts.v1
{
    public sealed class RestServiceConfiguration
    {
        //TODO:  Add code to loop until a repository connection is established.  
        //TODO:  Retry interval needs to be configurable (config file only).
        //TODO:  What happens when the repository is not available.
        //private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("RestServiceConfiguration");

        public delegate void ConfigValueChangedDelegate<T>(T oldValue, T newValue);
        public static event ConfigValueChangedDelegate<int> OnServicePortChanged;
        public static event ConfigValueChangedDelegate<SqlConnectionInfo> OnRepositoryConnectInfoChanged;

        private static ReaderWriterLock syncRoot;
        private static string instanceName;
        private static int servicePort;
        private static SqlConnectionInfo connectInfo;

        private static bool enableEvents;
        private static bool configDirty;
        private static bool needRefresh;

        static RestServiceConfiguration()
        {
            //syncRoot = new ReaderWriterLock();
            //connectInfo = new SqlConnectionInfo();
            //connectInfo.ApplicationName = Constants.RestServiceConnectionStringApplicationName;

            //// set the database connection factory for the object cache
            //CachedObjectRepositoryConnectionFactory.ConnectionFactory = GetRepositoryConnection;
        }

        public static void LogConfiguration()
        {
            //StringBuilder buffer = new StringBuilder();
            //buffer.AppendFormat("Rest Service Instance Name: {0}\n", InstanceName);
            //buffer.AppendFormat("Rest Service Remoting Port: {0}\n", ServicePort);
            //buffer.AppendFormat("Repository Connection String: {0}\n", ConnectionString);
            //buffer.AppendFormat("Repository Instance: {0}\n", RepositoryHost);
            //buffer.AppendFormat("Repository Database: {0}\n", RepositoryDatabase);
            //buffer.AppendFormat("Use Windows Authentication: {0}", RepositoryUseSSPI);
            //if (!RepositoryUseSSPI)
            //{
            //    buffer.AppendFormat("\nRepository User: {0}", RepositoryUser);
            //    buffer.AppendFormat("\nRepository Password: {0}", String.IsNullOrEmpty(RepositoryPassword) ? "Not Set!" : "********");
            //}

            //LOG.Info(buffer.ToString());
        }

        public static void SetRepositoryConnectInfo(SqlConnectionInfo sqlConnectInfo)
        {
            SqlConnectionInfo oldInfo = connectInfo;

            //syncRoot.AcquireWriterLock(-1);
            try
            {
                //if (RefreshNeeded)
                //    internal_refresh();

                connectInfo = sqlConnectInfo;
                configDirty = true;
            }
            finally
            {
               // syncRoot.ReleaseWriterLock();
            }

            if (OnRepositoryConnectInfoChanged != null)
                OnRepositoryConnectInfoChanged(oldInfo, sqlConnectInfo);
        }

        public static SqlConnectionInfo SQLConnectInfo
        {
            get
            {
                //if (RefreshNeeded)
                //    internal_refresh();

               // syncRoot.AcquireReaderLock(-1);
                SqlConnectionInfo result = connectInfo;
                //syncRoot.ReleaseReaderLock();
                return result;
            }
        }
    }
}
