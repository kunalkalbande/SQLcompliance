using System.Data.SqlClient;
using Idera.SQLcompliance.Core.Agent;

namespace Idera.SQLcompliance.Core.Licensing
{
    public static class LicenseHelper
    {
        private const int UNLIMITED_SERVERS = -1;

        public static bool LicenseAllowsMoreInstances(SqlConnection connection, SQLcomplianceConfiguration config)
        {
            bool moreAvailable = false;
            int serverCount = ServerRecord.CountAuditedServers(connection);

            //var tuple = ServerRecord.CountAlwaysOnReplicaServersForVirtualServers(connection);
            //var alwaysOnVirtualServerCount = tuple.Item1;
            //var uniqueReplicaServerCount = tuple.Item2;

            //if (uniqueReplicaServerCount > alwaysOnVirtualServerCount)
            //{
            //    serverCount = serverCount + uniqueReplicaServerCount - alwaysOnVirtualServerCount;
            //}

            if (config.LicenseObject != null &&
                 (config.LicenseObject.CombinedLicense.numLicensedServers > serverCount ||
                 config.LicenseObject.CombinedLicense.numLicensedServers == -1))
            {
                moreAvailable = true;
            }
            return moreAvailable;
        }

        public static bool LicenseAllowsMoreInstances(SqlConnection connection)
        {
            var config = new SQLcomplianceConfiguration();
            config.Read(connection);
            return LicenseAllowsMoreInstances(connection, config);
        }
    }
}
