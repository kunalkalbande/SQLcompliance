using System.Collections.Generic;
using System.Data;
using System.Linq;
using Idera.SQLcompliance.Application.GUI.SQL;
using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Agent;

namespace Idera.SQLcompliance.Application.GUI.Helper
{
    public class SensitiveColumnSearchHelper
    {
        public List<string> GetSQLServerInstances(bool getOnlyAuditedInstances = false)
        {
            var result = new List<string>();
            if (!getOnlyAuditedInstances)
            {
                var dataSources = System.Data.Sql.SqlDataSourceEnumerator.Instance.GetDataSources();
                foreach (DataRow dataSourceRow in dataSources.Rows)
                {
                    result.Add(dataSourceRow["ServerName"] + @"\" + dataSourceRow["InstanceName"]);
                }
            }

            return result
                .Union(ServerRecord.GetServers(Globals.Repository.Connection, false, true)
                    .Select(s => s.Instance))
                .OrderBy(s => s).ToList();
        }

        public List<string> GetDatabases(string instanceName)
        {
            var result = new List<string>();

            var sql = new SQLDirect();
            if (sql.OpenConnection(instanceName))
            {
                var databases = RawSQL.GetUserDatabases(sql.Connection);
                foreach (RawDatabaseObject database in databases)
                {
                    result.Add(database.name);
                }

                sql.CloseConnection();
            }

            return result;
        }

        public List<string> GetTables(string instanceName, string databaseName)
        {
            var result = new List<string>();

            var sql = new SQLDirect();
            if (sql.OpenConnection(instanceName))
            {
                var tables = RawSQL.GetTables(sql.Connection, databaseName);
                foreach (RawTableObject table in tables)
                {
                    result.Add(table.FullTableName);
                }

                sql.CloseConnection();
            }

            return result;
        }

        public List<RawTableDetails> SearchTables(string instanceName, string databaseName, IEnumerable<string> tableNames, string profile)
        {
            var result = new List<RawTableDetails>();

            var sql = new SQLDirect();
            if (sql.OpenConnection(instanceName))
            {
                foreach (RawTableDetails tableDetails in RawSQL.GetTableDetails(Globals.Repository.Connection, sql.Connection, databaseName, tableNames, profile))
                {
                    result.Add(tableDetails);
                }

                sql.CloseConnection();
            }

            return result;
        }

        public List<RawColumnDetails> SearchColumns(string instanceName, string databaseName, IEnumerable<string> tableNames, string profileName)
        {
            var result = new List<RawColumnDetails>();
            var sql = new SQLDirect();
            if (sql.OpenConnection(instanceName))
            {
                result.AddRange(RawSQL.GetColumnDetails(Globals.Repository.Connection, sql.Connection, databaseName, tableNames, profileName));
                sql.CloseConnection();
            }

            return result;
        }
    }
}
