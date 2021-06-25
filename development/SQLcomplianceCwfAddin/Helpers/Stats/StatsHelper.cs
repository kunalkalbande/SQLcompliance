using Idera.SQLcompliance.Core.Agent;
using Idera.SQLcompliance.Core.Stats;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace SQLcomplianceCwfAddin.Helpers.Stats
{
    public class StatsHelper
    {
        public static int CountTotalSince(int srvId, StatsCategory category, int days, SqlConnection connection)
        {
            ServerStatistics stats = GetServerStatistics(srvId, category, connection);
            var count = stats.TotalSince(DateTime.UtcNow.AddDays(-days));
            return count;
        }

        public static ServerStatistics GetServerStatistics(int srvId, StatsCategory category, SqlConnection connection)
        {
            EnterpriseStatistics ent = GetEnterpriseStatistics(category, connection);
            return ent.GetServerStatistics(srvId);
        }

        private static EnterpriseStatistics GetEnterpriseStatistics(StatsCategory category, SqlConnection connection)
        {
            var servers = ServerRecord.GetServers(connection, true);
            DateTime endDate = DateTime.UtcNow;
            DateTime startDate = endDate.AddDays(-31);

            EnterpriseStatistics retVal = StatsExtractor.GetEnterpriseStatistics(connection, category, servers, startDate, endDate);
            UpdateStatistics(retVal, connection);
            return retVal;
        }

        private static void UpdateStatistics(EnterpriseStatistics stats, SqlConnection connection)
        {
            DateTime endDate = DateTime.UtcNow;
            DateTime startDate = endDate.AddDays(-31);

            List<ServerRecord> servers;
            servers = ServerRecord.GetServers(connection, true);

            // Add a minute to avoid a key collision on the endDate data point
            StatsExtractor.UpdateEnterpriseStatistics(connection, stats, servers, startDate, endDate);
        }
    }
}
