using System;
using System.Text;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Idera.SQLcompliance.Core.Event
{
   public class DataChangeRecord
   {
      public DateTime startTime;
      public long eventSequence;
      public int spid;
      public int databaseId;
      public int actionType;
      public string schemaName = "";
      public string tableName;
      public int recordNumber;
      public string user;
      public int changedColumns;
      public int totalChanges;
      public string primaryKey;
      public int hashcode;
      public int tableId;
      public int eventId;
      public long dcId;
      public string guid;

      internal static readonly string InsertColumnList = "startTime,eventSequence,spid,databaseId,actionType" +
                                                   ",schemaName,tableName,recordNumber,userName,changedColumns" +
                                                   ",primaryKey,hashcode,tableId,totalChanges";

      internal static readonly string columns = "startTime,eventSequence,spid,databaseId,actionType" +
                                                ",schemaName,tableName,recordNumber,userName,changedColumns" +
                                                ",primaryKey,hashcode,tableId,dcId,eventId," +
                                                "totalChanges = (SELECT Top 1 ISNULL(dc2.totalChanges, 0) from {0}..{1} dc2 where dc2.recordNumber = 0 and dc2.eventId = {3}),guid";

      internal static readonly string SelectColumnList = "startTime,eventSequence,spid,databaseId,actionType" +
                                                         ",schemaName,tableName,recordNumber,userName,changedColumns" +
                                                         ",primaryKey,hashcode,tableId,dcId,eventId,totalChanges";


      public static List<DataChangeRecord> GetDataChangeRecords(SqlConnection conn, string dbName, EventRecord ev)
      {
         return GetDataChangeRecords(conn, dbName, ev.eventId);
      }

      public static List<DataChangeRecord> GetDataChangeRecords(SqlConnection conn, string dbName, int eventId)
      {
         string whereClause ;
         List<DataChangeRecord> retVal = new List<DataChangeRecord>();

         try
         {
            whereClause = String.Format("recordNumber<>0 AND eventId={0} ", eventId);
            using (SqlCommand cmd = new SqlCommand(GetSelectSQL(dbName, whereClause, eventId), conn))
            {
               using (SqlDataReader reader = cmd.ExecuteReader())
               {
                  while (reader.Read())
                  {
                     DataChangeRecord record = new DataChangeRecord();
                     record.Load(reader);
                     retVal.Add(record);
                  }
               }
            }
         }
         catch
         {
            Console.WriteLine("");
         }

         return retVal;
      }

      public static List<DataChangeRecord> GetDataChangeRecords(SqlConnection conn, string dbName,
         int eventId, int tableId)
      {
         string whereClause;
         List<DataChangeRecord> retVal = new List<DataChangeRecord>();

         try
         {
            whereClause =
               String.Format("recordNumber<>0 AND eventId={0} AND tableId={1}",
                             eventId,tableId);
            using (SqlCommand cmd = new SqlCommand(GetSelectSQL(dbName, whereClause, eventId), conn))
            {
               using (SqlDataReader reader = cmd.ExecuteReader())
               {
                  while (reader.Read())
                  {
                     DataChangeRecord record = new DataChangeRecord();
                     record.Load(reader);
                     retVal.Add(record);
                  }
               }
            }
         }
         catch
         {
            Console.WriteLine("");
         }

         return retVal;
      }


      //-----------------------------------------------------------------------------
      // InsertWithId - Insert record into events table
      //-----------------------------------------------------------------------------
      public bool InsertWithId(SqlConnection inConnection, string inDatabaseName)
      {
         return InsertWithId(inConnection, inDatabaseName, null);
      }

      //-----------------------------------------------------------------------------
      // InsertWithId - Insert record into events table
      //-----------------------------------------------------------------------------
      public bool InsertWithId(SqlConnection inConnection, string inDatabaseName, SqlTransaction inTransaction)
      {
         bool inserted;
         string sqlCmd = GetInsertSQL(inDatabaseName, true);

         using (SqlCommand cmd = new SqlCommand(sqlCmd,
                                                  inConnection))
         {
            cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;

            if (inTransaction != null)
               cmd.Transaction = inTransaction;

            cmd.ExecuteNonQuery();
            inserted = true;
         }
         return inserted;
      }

      //-----------------------------------------------------------------------------
      // Insert - Insert record into events table
      //-----------------------------------------------------------------------------
      public bool Insert(SqlConnection inConnection, string inDatabaseName)
      {
         return Insert(inConnection, inDatabaseName, null);
      }

      //-----------------------------------------------------------------------------
      // Insert - Insert record into events table
      //-----------------------------------------------------------------------------
      public bool Insert(SqlConnection inConnection,string inDatabaseName,SqlTransaction inTransaction)
      {
         bool inserted;
         string sqlCmd = GetInsertSQL(inDatabaseName, false);

         using (SqlCommand cmd = new SqlCommand(sqlCmd,
                                                  inConnection))
         {
            cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;

            if (inTransaction != null)
               cmd.Transaction = inTransaction;

            cmd.ExecuteNonQuery();
            inserted = true;
         }
         return inserted;
      }

      private static string GetSelectSQL(string serverDBName, string whereClause, int eventId)
      {
         return String.Format("SELECT " + columns + 
            " FROM {0}..{1} {2} ORDER BY eventSequence ASC", 
            SQLHelpers.CreateSafeDatabaseName(serverDBName),
            CoreConstants.RepositoryDataChangesTable,
            String.IsNullOrEmpty(whereClause) ? "" : " WHERE " + whereClause,
            eventId) ;
      }

      //-------------------------------------------------------------
      // GetInsertSQL
      //--------------------------------------------------------------
      private string GetInsertSQL(string serverDBName, bool includeId)
      {
         StringBuilder query = new StringBuilder();
         
         if (includeId)
         {
            query.AppendFormat("SET IDENTITY_INSERT {0}..{1} ON; ", SQLHelpers.CreateSafeDatabaseName(serverDBName),
                                                                     CoreConstants.RepositoryDataChangesTable);
            query.AppendFormat("INSERT INTO {0}..{1} ({2}) VALUES ({3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18});",
                               SQLHelpers.CreateSafeDatabaseName(serverDBName),
                               CoreConstants.RepositoryDataChangesTable,
                               SelectColumnList,
                               SQLHelpers.CreateSafeDateTime(startTime),
                               eventSequence,
                               spid,
                               databaseId,
                               actionType,
                               SQLHelpers.CreateSafeString(schemaName),
                               SQLHelpers.CreateSafeString(tableName),
                               recordNumber,
                               SQLHelpers.CreateSafeString(user),
                               changedColumns,
                               SQLHelpers.CreateSafeString(primaryKey),
                               hashcode,
                               tableId,
                               dcId,
                               eventId,
                               totalChanges);
            query.AppendFormat(" SET IDENTITY_INSERT {0}..{1} OFF;", SQLHelpers.CreateSafeDatabaseName(serverDBName),
                                                                     CoreConstants.RepositoryDataChangesTable);
         }
         else
         {
            query.AppendFormat("INSERT INTO {0}..{1} ({2}) VALUES ({3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16});",
                               SQLHelpers.CreateSafeDatabaseName(serverDBName),
                               CoreConstants.RepositoryDataChangesTable,
                               InsertColumnList,
                               SQLHelpers.CreateSafeDateTime(startTime),
                               eventSequence,
                               spid,
                               databaseId,
                               actionType,
                               SQLHelpers.CreateSafeString(schemaName),
                               SQLHelpers.CreateSafeString(tableName),
                               recordNumber,
                               SQLHelpers.CreateSafeString(user),
                               changedColumns,
                               SQLHelpers.CreateSafeString(primaryKey),
                               hashcode,
                               tableId,
                               totalChanges);
         }
         return query.ToString();
      }

      private string GetUpdateSQL(string serverDBName)
      {
         string tmp = "UPDATE {0}..{1} SET " +
                           ",databaseId = {2}" +
                           ",actionType = {3}" +
                           ",schemaName = {4}" +
                           ",tableName = {5}" +
                           ",recordNumber = (6)" +
                           ",userName = {7}" +
                           ",changedColumns = {8}" +
                           ",primaryKey = {9}" +
                           ",hashcode = {10}" +
                           ",tableId = {11}" +
                           ",totalChanges = {12}" +
                           " WHERE startTime = {13} AND spid = {14} AND eventSequence = {15};";
         return string.Format(tmp,
                               SQLHelpers.CreateSafeDatabaseName(serverDBName),
                               CoreConstants.RepositoryDataChangesTable,
                               databaseId,
                               actionType,
                               SQLHelpers.CreateSafeString(schemaName),
                               SQLHelpers.CreateSafeString(tableName),
                               recordNumber,
                               SQLHelpers.CreateSafeString(user),
                               changedColumns,
                               SQLHelpers.CreateSafeString(primaryKey),
                               hashcode,
                               tableId,
                               totalChanges,
                               SQLHelpers.CreateSafeDateTime(startTime),
                               spid,
                               eventSequence) ;
      }

      public void Load(SqlDataReader reader)
      {
         int col = 0;

         startTime = SQLHelpers.GetDateTime(reader, col++);
         eventSequence = SQLHelpers.GetLong(reader, col++);
         spid = SQLHelpers.GetInt32(reader, col++);
         databaseId = SQLHelpers.GetInt32(reader, col++);
         actionType = SQLHelpers.GetInt32(reader, col++);
         schemaName = SQLHelpers.GetString( reader, col++ ); 
         tableName = SQLHelpers.GetString(reader, col++);
         recordNumber = SQLHelpers.GetInt32(reader, col++);
         user = SQLHelpers.GetString(reader, col++);
         changedColumns = SQLHelpers.GetInt32(reader, col++);
         primaryKey = SQLHelpers.GetString(reader, col++);
         hashcode = SQLHelpers.GetInt32(reader, col++);
         tableId = SQLHelpers.GetInt32(reader, col++);
         dcId = SQLHelpers.GetLong(reader, col++);
         eventId = SQLHelpers.GetInt32(reader, col++);

         if (reader.IsDBNull(col))
         {
            totalChanges = changedColumns;
            col++;
         }
         else
         {
            totalChanges = SQLHelpers.GetInt32(reader, col++);

            //totalChanges should always have a value greater than zero. If it does not, it is from 3.1 agent, so set it to the changed Columns value.
            if (totalChanges == 0)
               totalChanges = changedColumns;
         }
      }
      
      new public int GetHashCode()
      {
         int x = NativeMethods.GetHashCode(startTime.ToString());

         x += ((int)eventSequence )<< 2;
         x += spid << 4;
         x += databaseId << 6;
         x += actionType << 8;
         if( recordNumber != 0 ) x += actionType << 10;
         if( changedColumns != 0 ) x += changedColumns << 12;

         if (schemaName != null && schemaName.Length > 0)
            x ^= NativeMethods.GetHashCode(schemaName);
         
         if (tableName != null && tableName.Length > 0)
            x ^= NativeMethods.GetHashCode(tableName);
         if (user != null && user.Length > 0)
            x ^= NativeMethods.GetHashCode(user);
         if (primaryKey != null && primaryKey.Length > 0)
            x ^= NativeMethods.GetHashCode( primaryKey );
         return x;
      }
   }
}
