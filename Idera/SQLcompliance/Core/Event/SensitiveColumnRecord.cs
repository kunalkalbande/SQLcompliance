using System;
using System.Text;
using System.Collections.Generic;
using System.Data.SqlClient;
using Idera.SQLcompliance.Core.TraceProcessing;

namespace Idera.SQLcompliance.Core.Event
{
   public class SensitiveColumnRecord
   {
      public DateTime startTime;
      public int eventId;
      public string columnName;
      public int hashcode;
      public int tableId;
      public int columnId;
      public int scId;

      internal static readonly string InsertColumnList = "startTime, eventId, columnName, hashcode, tableId, columnId";
      internal static readonly string SelectColumnList = "startTime, eventId, columnName, hashcode, tableId, columnId, scId";

      public static List<SensitiveColumnRecord> GetSensitiveColumnRecords(SqlConnection conn, string dbName, EventRecord ev)
      {
         return GetSensitiveColumnRecords(conn, dbName, ev.eventId);
      }

      public static List<SensitiveColumnRecord> GetSensitiveColumnRecords(SqlConnection conn, string dbName, int eventId)
      {
         List<SensitiveColumnRecord> retVal = new List<SensitiveColumnRecord>();

         try
         {
            using (SqlCommand cmd = new SqlCommand(GetSelectSQL(dbName, eventId), conn))
            {
               using (SqlDataReader reader = cmd.ExecuteReader())
               {
                  while (reader.Read())
                  {
                     SensitiveColumnRecord record = new SensitiveColumnRecord();
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
      public bool InsertWithId(SqlConnection connection, string dbName)
      {
         return InsertWithId(connection, dbName, null);
      }

      //-----------------------------------------------------------------------------
      // InsertWithId - Insert record into events table
      //-----------------------------------------------------------------------------
      public bool InsertWithId(SqlConnection connection, string dbName, SqlTransaction trans)
      {
         bool inserted;
         string sqlCmd = GetInsertSQL(dbName, true);

         using (SqlCommand cmd = new SqlCommand(sqlCmd, connection))
         {
            cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;

            if (trans != null)
               cmd.Transaction = trans;

            cmd.ExecuteNonQuery();
            inserted = true;
         }
         return inserted;
      }

      //-----------------------------------------------------------------------------
      // Insert - Insert record into events table
      //-----------------------------------------------------------------------------
      public bool Insert(SqlConnection connection, string dbName)
      {
         return Insert(connection, dbName, null);
      }

      //-----------------------------------------------------------------------------
      // Insert - Insert record into events table
      //-----------------------------------------------------------------------------
      public bool Insert(SqlConnection connection, string dbName, SqlTransaction trans)
      {
         bool inserted;
         string sqlCmd = GetInsertSQL(dbName, false);

         using (SqlCommand cmd = new SqlCommand(sqlCmd, connection))
         {
            cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;

            if (trans != null)
               cmd.Transaction = trans;

            cmd.ExecuteNonQuery();
            inserted = true;
         }
         return inserted;
      }

      private static string GetSelectSQL(string serverDBName, int eventId)
      {
         return String.Format("SELECT {0} FROM {1}..{2} WHERE eventId = {3} ORDER BY columnName ASC",
                              SelectColumnList,
                              SQLHelpers.CreateSafeDatabaseName(serverDBName),
                              CoreConstants.RepositorySensitiveColumnsTable,
                              eventId);
      }

      //-------------------------------------------------------------
      // GetInsertSQL
      //--------------------------------------------------------------
      private string GetInsertSQL(string serverDBName, bool includeId)
      {
         StringBuilder query = new StringBuilder();
         if (includeId)
         {
            query.AppendFormat("SET IDENTITY_INSERT {0}..{1} ON; ",  SQLHelpers.CreateSafeDatabaseName(serverDBName),
                                                                     CoreConstants.RepositorySensitiveColumnsTable);
            query.AppendFormat("INSERT INTO {0}..{1} ({2}) VALUES ({3},{4},{5},{6},{7},{8},{9});",
                               SQLHelpers.CreateSafeDatabaseName(serverDBName),
                               CoreConstants.RepositorySensitiveColumnsTable,
                               SelectColumnList,
                               SQLHelpers.CreateSafeDateTime(startTime),
                               eventId,
                               SQLHelpers.CreateSafeString(columnName),
                               hashcode,
                               tableId,
                               columnId,
                               scId);
            query.AppendFormat(" SET IDENTITY_INSERT {0}..{1} OFF;", SQLHelpers.CreateSafeDatabaseName(serverDBName),
                                                                     CoreConstants.RepositorySensitiveColumnsTable);
         }
         else
         {
            query.AppendFormat("INSERT INTO {0}..{1} ({2}) VALUES ({3},{4},{5},{6},{7},{8});",
                                  SQLHelpers.CreateSafeDatabaseName(serverDBName),
                                  CoreConstants.RepositorySensitiveColumnsTable,
                                  InsertColumnList,
                                  SQLHelpers.CreateSafeDateTime(startTime),
                                  eventId,
                                  SQLHelpers.CreateSafeString(columnName),
                                  hashcode,
                                  tableId,
                                  columnId);
         }
         return query.ToString();
      }

      public void Load(SqlDataReader reader)
      {
         int col = 0;

         startTime = SQLHelpers.GetDateTime(reader, col++);
         eventId = SQLHelpers.GetInt32(reader, col++);
         columnName = SQLHelpers.GetString( reader, col++ ); 
         hashcode = SQLHelpers.GetInt32(reader, col++);
         tableId = SQLHelpers.GetInt32(reader, col++);
         columnId = SQLHelpers.GetInt32(reader, col++);
         scId = SQLHelpers.GetInt32(reader, col++);
      }

      new public int GetHashCode()
      {
         int x = NativeMethods.GetHashCode(startTime.ToString());
         x += eventId << 2;
         x ^= NativeMethods.GetHashCode(columnName.ToString());
         return x;
      }
   }
}
