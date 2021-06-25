using System.Data;
using PluginCommon.Repository.Helpers;
using PluginCommon.Repository.SqlServer;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1;
using SqlServerSecurityModel = SQLcomplianceCwfAddin.RestService.DataContracts.v1.ManagedInstances.SqlServerSecurityModel;

namespace SQLcomplianceCwfAddin.Helpers
{
    public static class CredentialsValidationHelper
    {
        private const int ConnectionTimeout = 10;

        public static SqlServerConnectionInfo GetConnectionInfoForValidation(this ManagedServerInstance instance, ManagedCredentials credentials)
        {
            var securityModel = GetSecurityModel(credentials);

            var authToken = new SqlServerAuthToken(credentials.Account, credentials.Password, securityModel);
            var connectioInfo = new SqlServerConnectionInfo(instance.Instance, authToken)
            {
                ConnectionTimeout = ConnectionTimeout
            };

            return connectioInfo;
        }

        public static bool IsCredentialsValid(this ManagedServerInstance instance, ManagedCredentials credentials)
        {
            using (var conn = SqlServerHelper.OpenConnection(instance.GetConnectionInfoForValidation(credentials)))
            {
                return conn.State == ConnectionState.Open;
            }
        }

        private static PluginCommon.Repository.SqlServer.SqlServerSecurityModel GetSecurityModel(ManagedCredentials credentials)
        {
            var securityModel = credentials.AccountType == SqlServerSecurityModel.IntegratedWindowsImpersonation
                ? PluginCommon.Repository.SqlServer.SqlServerSecurityModel.Integrated
                : PluginCommon.Repository.SqlServer.SqlServerSecurityModel.User;
            return securityModel;
        }
    }
}
