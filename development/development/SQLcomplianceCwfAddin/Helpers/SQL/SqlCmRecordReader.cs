using System.Collections.Generic;
using System.Data.SqlClient;
using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Agent;
using Idera.SQLcompliance.Core.Event;

namespace SQLcomplianceCwfAddin.Helpers.SQL
{
    public class SqlCmRecordReader
    {
        public static ServerRecord GetServerRecord(int serverId, SqlConnection connection)
        {
            var server = new ServerRecord();
            server.Connection = connection;
            server.Read(serverId);
            return server;
        }

        public static DatabaseRecord GetDatabaseRecord(int databaseId, SqlConnection connection)
        {
            var database = new DatabaseRecord();
            database.Connection = connection;
            database.Read(databaseId);
            return database;
        }

        public static DatabaseRecord GetDatabaseRecordByName(int srvId, string databaseName, SqlConnection connection)
        {
            var database = new DatabaseRecord();
            database.Connection = connection;
            database.Read(srvId, databaseName);
            return database;
        }

        public static ServerRecord FindServerRecord(string sqlInstance, SqlConnection connection)
        {
            string instance = sqlInstance.ToUpper();
            var serverList = ServerRecord.GetServers(connection, false);

            if (serverList == null ||
                serverList.Count == 0)
            {
                return null;
            }

            foreach (ServerRecord foundServer in serverList)
            {
                if (foundServer.Instance.ToUpper() == instance)
                {
                    return foundServer;
                }
            }

            return null;
        }

        public static ServerRecord FindServerRecord(string sqlInstance, List<ServerRecord> serverList)
        {
            string instance = sqlInstance.ToUpper();

            if (serverList == null ||
                serverList.Count == 0)
            {
                return null;
            }

            foreach (ServerRecord foundServer in serverList)
            {
                if (foundServer.Instance.ToUpper() == instance)
                {
                    return foundServer;
                }
            }

            return null;
        }

        public static ServerRecord FindRegisteredAudedtedServerRecord(string sqlInstance, SqlConnection connection)
        {
            var registeredRecord = FindServerRecord(sqlInstance, connection);

            if (registeredRecord != null &&
                registeredRecord.IsAuditedServer)
            {
                return registeredRecord;
            }

            return null;
        }

        public static ServerRecord CloneRecord(ServerRecord recordToBeCloned, SqlConnection connection)
        {
            var newRecord = recordToBeCloned.Clone();
            newRecord.Connection = connection;
            return newRecord;
        }

        public static EventRecord GetEventRecord(int eventId, string eventDatabase, SqlConnection connection)
        {
            EventRecord eventRecord = new EventRecord(connection, eventDatabase);
            if (eventRecord.Read(eventId))
            {
                return eventRecord;
            }
            else
            {
               return null;
            }
        }
    }
}
