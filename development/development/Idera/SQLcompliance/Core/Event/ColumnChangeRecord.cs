using System;
using System.Text;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Idera.SQLcompliance.Core.Event
{
   public class ColumnChangeRecord
   {
      public DateTime startTime;
      public long eventSequence;
      public int spid;
      public string columnName;
      public string beforeValue;
      public string afterValue;
      public int hashcode;
      public int columnId;
      public long dcId;
      public long ccId;

      internal static readonly string InsertColumnList = "startTime,eventSequence,spid,columnName,beforeValue,afterValue,hashcode,columnId,dcId";
      internal static readonly string SelectColumnList = "startTime,eventSequence,spid,columnName,beforeValue,afterValue,hashcode,columnId,dcId,ccId";
      internal static readonly string AliasedSelectColumnList = "{0}.startTime,{0}.eventSequence,{0}.spid,{0}.columnName,{0}.beforeValue,{0}.afterValue,{0}.hashcode,{0}.columnId,{0}.dcId,{0}.ccId";


      public static List<ColumnChangeRecord> GetColumnChangeRecords(SqlConnection conn, string dbName, DataChangeRecord dcRecord, int columnId)
      {
         string whereClause;
         List<ColumnChangeRecord> retVal = new List<ColumnChangeRecord>();

         try
         {
            whereClause =
               String.Format("spid={0} AND eventSequence={1} AND startTime={2} AND columnId={3}",
                             dcRecord.spid, dcRecord.eventSequence,
                             SQLHelpers.CreateSafeDateTime(dcRecord.startTime),
                             columnId);
            using (SqlCommand cmd = new SqlCommand(GetSelectSQL(dbName, whereClause), conn))
            {
               using (SqlDataReader reader = cmd.ExecuteReader())
               {
                  while (reader.Read())
                  {
                     ColumnChangeRecord record = new ColumnChangeRecord();
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
      
      public static List<ColumnChangeRecord> GetColumnChangeRecords(SqlConnection conn, string dbName, DataChangeRecord dcRecord)
      {
         string whereClause;
         List<ColumnChangeRecord> retVal = new List<ColumnChangeRecord>();

         try
         {
            whereClause =
               String.Format("spid={0} AND eventSequence={1} AND startTime={2}",
                             dcRecord.spid, dcRecord.eventSequence,
                             SQLHelpers.CreateSafeDateTime(dcRecord.startTime));
            using (SqlCommand cmd = new SqlCommand(GetSelectSQL(dbName, whereClause), conn))
            {
               using (SqlDataReader reader = cmd.ExecuteReader())
               {
                  while (reader.Read())
                  {
                     ColumnChangeRecord record = new ColumnChangeRecord();
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

      private static string GetSelectSQL(string serverDBName, string whereClause)
      {
         return String.Format("SELECT {0} FROM {1}..{2} {3} ORDER BY eventSequence ASC",
            SelectColumnList,
            SQLHelpers.CreateSafeDatabaseName(serverDBName),
            CoreConstants.RepositoryColumnChangesTable,
            String.IsNullOrEmpty(whereClause) ? "" : " WHERE " + whereClause);
      }

      //-----------------------------------------------------------------------------
      // InsertWithId - Insert record into events table
      //-----------------------------------------------------------------------------
      public bool InsertWithId(SqlConnection inConnection, string inDatabaseName)
      {
         return InsertWithId(inConnection, inDatabaseName, null);
      }

      //-----------------------------------------------------------------------------
      // Insert - Insert record into events table
      //-----------------------------------------------------------------------------
      public bool InsertWithId(SqlConnection inConnection, string inDatabaseName, SqlTransaction inTransaction)
      {
         bool inserted;
         string sqlCmd = GetInsertSQL(inDatabaseName, true);

         using (SqlCommand cmd = new SqlCommand(sqlCmd, inConnection))
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
      public bool Insert(SqlConnection inConnection, string inDatabaseName, SqlTransaction inTransaction)
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

      //-------------------------------------------------------------
      // GetInsertSQL
      //--------------------------------------------------------------
      private string GetInsertSQL(string serverDBName, bool includeId)
      {
         StringBuilder query = new StringBuilder();
         if (includeId)
         {
            query.AppendFormat("SET IDENTITY_INSERT {0}..{1} ON; ", SQLHelpers.CreateSafeDatabaseName(serverDBName),
                                                                     CoreConstants.RepositoryColumnChangesTable);
            query.AppendFormat("INSERT INTO {0}..{1} ( {2} ) VALUES ({3},{4},{5},{6},{7},{8},{9},{10},{11},{12});",
                               SQLHelpers.CreateSafeDatabaseName(serverDBName),
                               CoreConstants.RepositoryColumnChangesTable,
                               SelectColumnList,
                               SQLHelpers.CreateSafeDateTime(startTime),
                               eventSequence,
                               spid,
                               SQLHelpers.CreateSafeString(columnName),
                               SQLHelpers.CreateSafeString(beforeValue),
                               SQLHelpers.CreateSafeString(afterValue),
                               hashcode,
                               columnId,
                               "NULL",
                               ccId);
            query.AppendFormat(" SET IDENTITY_INSERT {0}..{1} OFF;", SQLHelpers.CreateSafeDatabaseName(serverDBName),
                                                                     CoreConstants.RepositoryColumnChangesTable);
         }
         else
         {
            query.AppendFormat("INSERT INTO {0}..{1} ( {2} ) VALUES ({3},{4},{5},{6},{7},{8},{9},{10},{11});",
                               SQLHelpers.CreateSafeDatabaseName(serverDBName),
                               CoreConstants.RepositoryColumnChangesTable,
                               InsertColumnList,
                               SQLHelpers.CreateSafeDateTime(startTime),
                               eventSequence,
                               spid,
                               SQLHelpers.CreateSafeString(columnName),
                               SQLHelpers.CreateSafeString(beforeValue),
                               SQLHelpers.CreateSafeString(afterValue),
                               hashcode,
                               columnId,
                               "NULL");
         }
         return query.ToString();
      }

      private string GetUpdateSQL(string serverDBName)
      {
         string tmp = "UPDATE {0}..{1} SET " +
                           ",columnName = {2}" +
                           ",beforeValue = {3}" +
                           ",afterValue = {4}" +
                           ",hashcode = (5)" +
                           ",columnId = {6}" +
                           " WHERE startTime = {7} AND spid = {8} AND eventSequence = {9};";
         return string.Format(tmp,
                               SQLHelpers.CreateSafeDatabaseName(serverDBName),
                               CoreConstants.RepositoryDataChangesTable,
                               SQLHelpers.CreateSafeString(columnName),
                               SQLHelpers.CreateSafeString(beforeValue),
                               SQLHelpers.CreateSafeString(afterValue),
                               hashcode,
                               columnId,
                               SQLHelpers.CreateSafeDateTime(startTime),
                               spid,
                               eventSequence);
      }

      public void Load(SqlDataReader reader)
      {
         int col = 0;

         startTime = SQLHelpers.GetDateTime(reader, col++);
         eventSequence = SQLHelpers.GetLong(reader, col++);
         spid = SQLHelpers.GetInt32(reader, col++);
         columnName = SQLHelpers.GetString(reader, col++);
         beforeValue = SQLHelpers.GetString(reader, col++);
         afterValue = SQLHelpers.GetString(reader, col++);
         hashcode = SQLHelpers.GetInt32(reader, col++);
         columnId = SQLHelpers.GetInt32(reader, col++);
         dcId = SQLHelpers.GetLong(reader, col++);
         ccId = SQLHelpers.GetLong(reader, col++);
      }

      new public int GetHashCode()
      {
         int x = NativeMethods.GetHashCode( startTime.ToString());

         x += ((int)eventSequence )<< 2;
         x += spid << 4;
         x ^= NativeMethods.GetHashCode( columnName );
         if ( beforeValue != null && beforeValue.Length > 0 )
            x ^= NativeMethods.GetHashCode( beforeValue );
         if ( afterValue != null && afterValue.Length > 0 )
            x ^= ( NativeMethods.GetHashCode( afterValue ) << 2 );
         return x;
      }
   }
}
