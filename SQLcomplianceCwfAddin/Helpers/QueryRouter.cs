using System;
using System.Data.SqlClient;
using PluginCommon;
using PluginCommon.Repository.Helpers;
using PluginCommon.Repository.SqlServer;

namespace SQLcomplianceCwfAddin.Helpers
{
    internal class QueryRouter
    {
        #region members

        private static QueryRouter _instance;

        #endregion

        #region constructor \ destructor

        private QueryRouter() { }

        #endregion

        #region properties

        public static QueryRouter Instance
        {
            get { return _instance ?? (_instance = new QueryRouter()); }
        }

        #endregion

        #region methods

        internal string[] GetRepositoryInfo(ConnectionCredentials credentials)
        {
            string[] separators = { ";" };
            return credentials.Location.Split(separators, StringSplitOptions.RemoveEmptyEntries);
        }

        internal void GetSqlCmRepositoryHostAndDatabase(string[] repositoryInformation, out string repositoryHost, out string repositoryDatabase)
        {
            repositoryHost = repositoryInformation[0];
            repositoryDatabase = repositoryInformation[1];
        }

        internal SqlConnection GetRepositoryConnection(string repositoryHost, string repositoryDatabase, ConnectionCredentials credentials)
        {
            var connectionInfo = new SqlServerConnectionInfo(repositoryHost, repositoryDatabase, SqlServerSecurityModel.Integrated,credentials.ConnectionUser, credentials.ConnectionPassword);
            var connection = SqlServerHelper.OpenConnection(connectionInfo);
            return connection;
        }

        internal SqlConnection GetSqlCmRepositoryConnection(ConnectionCredentials credentials)
        {
            string repositoryHost, repositoryDatabase;

            var repositoryInformation = GetRepositoryInfo(credentials);
            GetSqlCmRepositoryHostAndDatabase(repositoryInformation, out repositoryHost, out repositoryDatabase);
            return GetRepositoryConnection(repositoryHost, repositoryDatabase, credentials);
        }

        #endregion
    }
}
