using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;

using Microsoft.SqlServer.Server;

public class TriggerCore
{
   #region constants

   private const string ChangeItemFormat = "<C: {0}><B: {1}><A: {2}><T: {3}>, ";
   private const string KeyValueFormat = "<{0} = {1}>, ";
   private const string InsertedTable = "inserted";
   private const string DeletedTable = "deleted";
   private const string DeleteAction = "D";
   private const string InsertAction = "I";
   private const string UpdateAction = "U";
   private const string EndSequenceQuery = "DELETE [SQLcompliance_Data_Change].[SQLcompliance_Changed_Data_Table] WHERE 1 = 0";

   #endregion

   #region public methods
   
   //---------------------------------------------------------------------------------------
   // GenerateTraceEvents: Common method shared by all the DML After triggers
   //
   // Note: Do not reference anything external to the SQL Server.  Otherwise, the assemblywon't be SAFE.
   //       
   //---------------------------------------------------------------------------------------
   public static void GenerateTraceEvents(string tableName, string[] pkColumns, int rowLimit)
   {
      SqlTriggerContext context = SqlContext.TriggerContext;

      try
      {
         using ( SqlConnection conn = new SqlConnection("context connection=true") )
         {
            conn.Open();
            try
            {
               StringBuilder changeList = new StringBuilder();
               string user = GetCurrentUser( conn );
                    string keyVal = String.Empty;
               int recordCount;
               int totalUpdated = 0;

               switch ( context.TriggerAction )
                  //Switch on the Action occuring on the Table
               {
                  case TriggerAction.Update:
                     recordCount = GetRecordCount(InsertedTable, conn);
                     totalUpdated = GetTotalUpdatedColumns(context);

                     ExecuteSummaryQuery(tableName, UpdateAction, user, recordCount, totalUpdated, conn);
                     if (recordCount == 0)
                        return;

                     using (DataTable inserted = GetInsertedTable(conn, rowLimit), deleted = GetDeletedTable(conn, rowLimit))
                     {
                         for (int i = 0; i < inserted.Rows.Count; i++)
                        {
                           changeList.Length = 0;
                           DataRow iRow = inserted.Rows[i];
                           DataRow dRow = deleted.Rows[i];
                            try
                            {
                           keyVal = PrimaryKeyToString(pkColumns, iRow);
							}
                                    catch (Exception)
                                    {
                                        SendMessage("Unable to get Primary key for Before - After Data change event");
                            }

                           for (int j = 0; j < inserted.Columns.Count; j++)
                           {
                              if (context.IsUpdatedColumn(j))
                              {
                                 changeList.AppendFormat(ChangeItemFormat,
                                                         CreateSafeString(inserted.Columns[j].ColumnName),
                                                         dRow[j].Equals(DBNull.Value) ? "NULL" : CreateSafeString(dRow[j].ToString()),
                                                         iRow[j].Equals(DBNull.Value) ? "NULL" : CreateSafeString(iRow[j].ToString()),
                                                         totalUpdated);
                              }
                           }
                           ExecuteDataChangeQuery(tableName,
                                                  UpdateAction,
                                                  user,
                                                  keyVal,
                                                  i + 1,
                                                  changeList.ToString(),
                                                  conn);
                           }
                     }
                     break;

                  case TriggerAction.Insert:
                     SendMessage("Getting inserted record count");
                     recordCount = GetRecordCount(InsertedTable, conn);

                     SendMessage("Getting the total number of updated columns");
                     totalUpdated = GetTotalUpdatedColumns(context);

                     SendMessage("Summarizing DML action");
                     ExecuteSummaryQuery(tableName, InsertAction, user, recordCount, totalUpdated, conn);
                     if (recordCount == 0)
                        return;

                     SendMessage("Getting inserted table");
                     using (DataTable inserted = GetInsertedTable(conn, rowLimit))
                     {
                        for (int i = 0; i < inserted.Rows.Count; i++)
                        {
                           changeList.Length = 0;
                           DataRow iRow = inserted.Rows[i];
                           keyVal = PrimaryKeyToString(pkColumns, iRow);
                           for (int j = 0; j < inserted.Columns.Count; j++)
                           {
                              changeList.AppendFormat(ChangeItemFormat,
                                                      CreateSafeString(inserted.Columns[j].ColumnName),
                                                      "<N/A>",
                                                      iRow[j].Equals(DBNull.Value) ? "NULL" : CreateSafeString(iRow[j].ToString()),
                                                      totalUpdated);
                           }
                           SendMessage("Generating DML before/after event");
                           ExecuteDataChangeQuery(tableName,
                                                  InsertAction,
                                                  user,
                                                  keyVal,
                                                  i + 1,
                                                  changeList.ToString(),
                                                  conn);
                        }
                     }
                     break;

                  case TriggerAction.Delete:

                     recordCount = GetRecordCount( DeletedTable, conn );
                     totalUpdated = GetTotalUpdatedColumns(context);

                     ExecuteSummaryQuery(tableName, DeleteAction, user, recordCount, totalUpdated, conn);
                     if ( recordCount == 0 )
                        return;

                     using (DataTable deleted = GetDeletedTable(conn, rowLimit))
                     {
                        for (int i = 0; i < deleted.Rows.Count; i++)
                        {
                           changeList.Length = 0;
                           DataRow dRow = deleted.Rows[i];
                           keyVal = PrimaryKeyToString(pkColumns, dRow);
                           for (int j = 0; j < deleted.Columns.Count; j++)
                           {
                              //Build and Audit Entry
                              changeList.AppendFormat(ChangeItemFormat,
                                                      CreateSafeString(deleted.Columns[j].ColumnName),
                                                      dRow[j].Equals(DBNull.Value) ? "NULL" : CreateSafeString(dRow[j].ToString()),
                                                      "<N/A>",
                                                      totalUpdated);
                           }
                           ExecuteDataChangeQuery(tableName,
                                                  DeleteAction,
                                                  user,
                                                  keyVal,
                                                  i + 1,
                                                  changeList.ToString(),
                                                  conn);
                        }
                     }
                     break;
                  default:
                     return;
               }
            }
            catch ( Exception innerException )
            {
               // We have a live connection and use it to generate an error event so that we can create alerts
               // somewhere.
               try
               {
                  ExecuteErrorMessageQuery( conn, tableName, innerException.Message );
               }
               catch ( Exception ie )
               {
                  SqlContext.Pipe.Send( "An error occurred sending error message query: " + ie.Message);
               }
            }
            finally
            {
               try
               {
                  SendMessage( "Generating end DML before/after event" );
                  ExecuteQuery( conn, EndSequenceQuery );
               }
               catch ( Exception e )
               {
                  SqlContext.Pipe.Send( e.Message );
               }
            } // try
         } // using
      }
      catch( Exception oe )
      {
         // Catch all exceptions so that the DML operation won't be affected.
         SqlContext.Pipe.Send(oe.Message);
      }
   }


   //---------------------------------------------------------------------------------------
   // GenerateTraceEvents: Common method shared by all the DML After triggers
   //
   // Note: Do not reference anything external to the SQL Server.  Otherwise, the assemblywon't be SAFE.
   //       
   //---------------------------------------------------------------------------------------
   public static void GenerateTraceEvents(string tableName, string[] pkColumns, string[] auditedColumns, int rowLimit)
   {
      SqlTriggerContext context = SqlContext.TriggerContext;

      try
      {
         using (SqlConnection conn = new SqlConnection("context connection=true"))
         {
            conn.Open();
            try
            {
               StringBuilder changeList = new StringBuilder();
               string user = GetCurrentUser(conn);
               string keyVal;
               int recordCount;
               int totalUpdated = 0;
               string[] queriedColumns = MergeColumns(pkColumns, auditedColumns);

               switch (context.TriggerAction)
               //Switch on the Action occuring on the Table
               {
                  case TriggerAction.Update:
                     recordCount = GetRecordCount(InsertedTable, conn, auditedColumns[0]);
                     totalUpdated = GetTotalUpdatedColumns(context);
                     SendMessage(string.Format("{0} columns out of {1} updated", totalUpdated, context.ColumnCount));

                     ExecuteSummaryQuery(tableName, UpdateAction, user, recordCount, totalUpdated, conn);
                     if (recordCount == 0)
                        return;

                     using (DataTable inserted = GetInsertedTable(conn, queriedColumns, rowLimit),
                                      deleted = GetDeletedTable(conn, queriedColumns, rowLimit))
                     {
                        // Get the list of columns to determine the ordinal position in the table for checking if updated
                         Dictionary<string, int> columns = GetColumns(conn, tableName);

                        for (int i = 0; i < inserted.Rows.Count; i++)
                        {
                           changeList.Length = 0;
                           DataRow iRow = inserted.Rows[i];
                           DataRow dRow = deleted.Rows[i];
                           keyVal = PrimaryKeyToString(pkColumns, iRow);

                           for (int j = 0; j < auditedColumns.Length; j++)
                           {
                              int ordinal;
                              // skip the column if it is set for auditing, but no longer in the table
                              if (columns.TryGetValue(auditedColumns[j], out ordinal))
                              {
                                 SendMessage(string.Format("{0}:{1},{2}", auditedColumns[j], inserted.Columns[auditedColumns[j]].Ordinal, ordinal));

                                 if (context.IsUpdatedColumn(ordinal - 1))
                                 {
                                     changeList.AppendFormat(ChangeItemFormat,
                                                             CreateSafeString(auditedColumns[j]),
                                                             dRow[j].Equals(DBNull.Value) ? "NULL" : CreateSafeString(dRow[auditedColumns[j]].ToString()),
                                                             iRow[j].Equals(DBNull.Value) ? "NULL" : CreateSafeString(iRow[auditedColumns[j]].ToString()),
                                                             totalUpdated);
                                 }
                              }
                           }
                           ExecuteDataChangeQuery(tableName,
                                                  UpdateAction,
                                                  user,
                                                  keyVal,
                                                  i + 1,
                                                  changeList.ToString(),
                                                  conn);

                        }
                     }
                     break;

                  case TriggerAction.Insert:
                     SendMessage("Getting inserted record count");
                     recordCount = GetRecordCount(InsertedTable, conn, auditedColumns[0]);

                     SendMessage("Getting total number of updated columns");
                     totalUpdated = GetTotalUpdatedColumns(context);

                     SendMessage("Summarizing DML action");
                     ExecuteSummaryQuery(tableName, InsertAction, user, recordCount, totalUpdated, conn);
                     if (recordCount == 0)
                        return;

                     SendMessage("Getting inserted table");
                     using (DataTable inserted = GetInsertedTable(conn, queriedColumns, rowLimit))
                     {
                        for (int i = 0; i < inserted.Rows.Count; i++)
                        {
                           changeList.Length = 0;
                           DataRow iRow = inserted.Rows[i];
                           keyVal = PrimaryKeyToString(pkColumns, iRow);
                           for (int j = 0; j < auditedColumns.Length; j++)
                           {
                              changeList.AppendFormat(ChangeItemFormat,
                                                      CreateSafeString(auditedColumns[j]),
                                                      "<N/A>",
                                                      iRow[auditedColumns[j]].Equals(DBNull.Value) ? "NULL" : CreateSafeString(iRow[auditedColumns[j]].ToString()),
                                                      totalUpdated);
                           }
                           SendMessage("Generating DML before/after event");
                           ExecuteDataChangeQuery(tableName,
                                                  InsertAction,
                                                  user,
                                                  keyVal,
                                                  i + 1,
                                                  changeList.ToString(),
                                                  conn);
                        }
                     }
                     break;

                  case TriggerAction.Delete:

                     recordCount = GetRecordCount(DeletedTable, conn, auditedColumns[0]);
                     totalUpdated = GetTotalUpdatedColumns(context);

                     ExecuteSummaryQuery(tableName, DeleteAction, user, recordCount, totalUpdated, conn);
                     if (recordCount == 0)
                        return;

                     using (DataTable deleted = GetDeletedTable(conn, queriedColumns, rowLimit))
                     {
                        for (int i = 0; i < deleted.Rows.Count; i++)
                        {
                           changeList.Length = 0;
                           DataRow dRow = deleted.Rows[i];
                           keyVal = PrimaryKeyToString(pkColumns, dRow);
                           for (int j = 0; j < auditedColumns.Length; j++)
                           {
                              //Build and Audit Entry
                              changeList.AppendFormat(ChangeItemFormat,
                                                      CreateSafeString(auditedColumns[j]),
                                                      dRow[auditedColumns[j]].Equals(DBNull.Value) ? "NULL" : CreateSafeString(dRow[auditedColumns[j]].ToString()),
                                                      "<N/A>",
                                                      totalUpdated);
                           }
                           ExecuteDataChangeQuery(tableName,
                                                  DeleteAction,
                                                  user,
                                                  keyVal,
                                                  i + 1,
                                                  changeList.ToString(),
                                                  conn);
                        }
                     }
                     break;
                  default:
                     return;
               }
            }
            catch (Exception innerException)
            {
               // We have a live connection and use it to generate an error event so that we can create alerts
               // somewhere.
               try
               {
                  ExecuteErrorMessageQuery(conn, tableName, innerException.Message);
               }
               catch (Exception ie)
               {
                  SqlContext.Pipe.Send("An error occurred sending error message query: " + ie.Message);
               }
            }
            finally
            {
               try
               {
                  SendMessage("Generating end DML before/after event");
                  ExecuteQuery(conn, EndSequenceQuery);
               }
               catch (Exception e)
               {
                  SqlContext.Pipe.Send(e.Message);
               }
            } // try
         } // using
      }
      catch (Exception oe)
      {
         // Catch all exceptions so that the DML operation won't be affected.
         SqlContext.Pipe.Send(oe.Message);
      }
   }

   #endregion

   #region Private methods
   //---------------------------------------------------------------
   // Merges two lists of columns
   //---------------------------------------------------------------
   private static string[] MergeColumns(string[] cols1, string[] cols2)
   {
      if (cols1 == null && cols2 == null)
         return new string[0];
      else if (cols1 != null && cols2 == null)
         return cols1;
      else if (cols2 != null && cols1 == null)
         return cols2;
      List<string> newList = new List<string>(cols1.Length + cols2.Length);
      newList.AddRange(cols1);
      foreach (string col in cols2)
         if (!newList.Contains(col))
            newList.Add(col);
      return newList.ToArray();
   }

   private static int GetTotalUpdatedColumns(SqlTriggerContext context)
   {
      int totalUpdated = 0;
      for (int j = 0; j < context.ColumnCount; j++)
      {
         totalUpdated += context.IsUpdatedColumn(j) ? 1 : 0;
      }
      return (totalUpdated);
   }


   //---------------------------------------------------------------
   //
   //---------------------------------------------------------------
   private static string GetCurrentUser(SqlConnection conn)
   {
      // SQLcommand is disposable - don't want to leak in SQL Server
      using (SqlCommand cmd = new SqlCommand("SELECT system_user", conn))
      {
         return cmd.ExecuteScalar().ToString();
      }
   }

   //---------------------------------------------------------------
   //
   //---------------------------------------------------------------
   private static DataTable GetInsertedTable(SqlConnection conn, int rowLimit)
   {
      string query = GetTableQuery(InsertedTable, rowLimit);
      return GetDataTable(query, conn);
   }

   //---------------------------------------------------------------
   //
   //---------------------------------------------------------------
   private static DataTable GetDeletedTable(SqlConnection conn, int rowLimit)
   {
      string query = GetTableQuery(DeletedTable, rowLimit);
      return GetDataTable(query, conn);
   }

   //---------------------------------------------------------------
   //
   //---------------------------------------------------------------
   private static string GetTableQuery(string table, int rowLimit)
   {
      if (rowLimit <= 0)
         return String.Format("SELECT * FROM {0}", table);
      else
         return String.Format("SELECT TOP ({0}) * FROM {1} OPTION (FAST {0} )", rowLimit, table);
   }

   //---------------------------------------------------------------
   //
   //---------------------------------------------------------------
   private static string GetTableQuery(string table, string[] cols, int rowLimit)
   {
      if (rowLimit <= 0)
      {
         return String.Format("SELECT {0} FROM {1}", CreateColumnList(cols), table);
      }
      else
      {
         return String.Format("SELECT TOP ({0}) {1} FROM {2} OPTION (FAST {0} )", rowLimit, CreateColumnList(cols), table);
      }
   }

   private static string CreateColumnList(string[] cols)
   {
      StringBuilder list = new StringBuilder(String.Format("[{0}]", cols[0]));
      for (int i = 1; i < cols.Length; i++)
         list.AppendFormat(", [{0}] ", cols[i]);
      return list.ToString();
   }

   //---------------------------------------------------------------
   //
   //---------------------------------------------------------------
   private static DataTable GetInsertedTable(SqlConnection conn, string[] cols, int rowLimit)
   {
      string query = GetTableQuery(InsertedTable, cols, rowLimit);
      return GetDataTable(query, conn);
   }

   //---------------------------------------------------------------
   //
   //---------------------------------------------------------------
   private static DataTable GetDeletedTable(SqlConnection conn, string[] cols, int rowLimit)
   {
      string query = GetTableQuery(DeletedTable, cols, rowLimit);
      return GetDataTable(query, conn);
   }

   //---------------------------------------------------------------
   //
   //---------------------------------------------------------------
   private static Dictionary<string, int> GetColumns(SqlConnection conn, string fullTableName) 
   {
      string schema = GetSchemaNameFromTable(fullTableName);
      string tableName = GetTableNameWithoutSchema(fullTableName);
        
      string query = string.Format(@"SELECT COLUMN_NAME, ORDINAL_POSITION FROM [{0}].INFORMATION_SCHEMA.COLUMNS 
                                   WHERE TABLE_NAME = '{1}' and TABLE_SCHEMA = '{2}'", conn.Database, tableName, schema);

      Dictionary<string, int> columns = new Dictionary<string, int>();

      using (DataTable columnTable = GetDataTable(query, conn))
      {
         foreach (DataRow row in columnTable.Rows)
         {
            columns.Add(row[0].ToString(), (int)row[1]);
         }
      }

      return columns;
   }

   private static string GetSchemaNameFromTable(string fullTableName)
   {
       string[] values = fullTableName.Split('.');

       if (values.Length > 1)
       {
           return values[0];
       }

       return "dbo";
   }

   private static string GetTableNameWithoutSchema(string fullTableName)
   {
       string[] values = fullTableName.Split('.');

       if (values.Length > 1)
       {
           return values[1];
       }

       return fullTableName;
   }

   //---------------------------------------------------------------
   //
   //---------------------------------------------------------------
   private static DataTable GetDataTable(string query, SqlConnection conn)
   {
      using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
      {
         DataTable table = new DataTable();  
         da.Fill(table);

         return table;
      }
   }

   //---------------------------------------------------------------
   //
   //---------------------------------------------------------------
   private static string PrimaryKeyToString(IEnumerable<string> pkCols, DataRow row)
   {
      StringBuilder keyVal = new StringBuilder();

      foreach (string col in pkCols)
      {
         keyVal.AppendFormat(KeyValueFormat, CreateSafeString(col), CreateSafeString(row[col].ToString()));
      }

      return keyVal.ToString();
   }

   //---------------------------------------------------------------
   //
   //---------------------------------------------------------------
   private static int GetRecordCount(string table, SqlConnection conn)
   {
      string query = String.Format("SELECT COUNT(*) FROM {0}", table);
      return ExecuteInt(conn, query);
   }

   //---------------------------------------------------------------
   //
   //---------------------------------------------------------------
   private static int GetRecordCount(string table, SqlConnection conn, string col)
   {
      string query = String.Format("SELECT COUNT('{1}') FROM {0}", table, col);
      return ExecuteInt(conn, query);
   }

   //---------------------------------------------------------------
   //
   //---------------------------------------------------------------
   private static void ExecuteSummaryQuery(string table,
                                            string action,
                                            string user,
                                            int auditedUpdates,
                                            int totalUpdated,
                                            SqlConnection conn)
   {
      String query =
         String.Format(
            "SELECT 'Metadata - Table: {0}, Action: {1}, By: {2}, Count: {3}, TotalCount: {4}' SUMMARY FROM [SQLcompliance_Data_Change].[SQLcompliance_Changed_Data_Table] WHERE 1 = 0",
            CreateSafeString(table),
            action,
            user,
            auditedUpdates,
            totalUpdated);
      ExecuteQuery(conn, query);
   }

   private static void ExecuteErrorMessageQuery(SqlConnection conn,
                                                 string table,
                                                 string error)
   {
      string query =
         String.Format(
            "SELECT 'Error.  Table: {0}, Error: {1}' Message FROM [SQLcompliance_Data_Change].[SQLcompliance_Changed_Data_Table] WHERE 1 = 0",
             CreateSafeString(table),
             CreateSafeString(error));
             
      SqlContext.Pipe.Send( "ErrorQuery: " + query );
      ExecuteQuery(conn, query);
   }


   //---------------------------------------------------------------
   //
   //---------------------------------------------------------------
   private static void ExecuteDataChangeQuery(string table,
                                                 string action,
                                                 string user,
                                                 string keyVal,
                                                 int recordNo,
                                                 string changeList,
                                                 SqlConnection conn)
   {
      string query = BuildQueryString(action,
                                       table,
                                       keyVal,
                                       recordNo,
                                       changeList,
                                       user);
      ExecuteQuery(conn, query);
   }

   //
   // Put the audit result values into a string
   //
   private static string BuildQueryString(string actionType,
                                           string tableName,
                                           string pk,
                                           int recordNo,
                                           string changeList,
                                           string user)
   {
      StringBuilder resultString = new StringBuilder("SELECT '");

      resultString.AppendFormat("Table: {0}, ", tableName);
      resultString.AppendFormat("Action: {0}, ", actionType);
      resultString.AppendFormat("By: {0}, ", user);
      resultString.AppendFormat("PK: {0}, ", pk);
      resultString.AppendFormat("No: {0}, ", recordNo ); 
      resultString.Append(changeList);
      resultString.AppendFormat("' CHANGES FROM [SQLcompliance_Data_Change].[SQLcompliance_Changed_Data_Table] WHERE 1 = 0");

      return resultString.ToString();
   }

   //
   // Executes a query
   //
   private static void ExecuteQuery(SqlConnection conn,
                                     string queryStmt)
   {
      using (SqlCommand cmd = new SqlCommand(queryStmt, conn))
      {
         cmd.ExecuteScalar();
      }
   }


   private static int ExecuteInt(SqlConnection conn,
                                     string queryStmt)
   {
      int val;
      using (SqlCommand cmd = new SqlCommand(queryStmt, conn))
      {
         val = (int)cmd.ExecuteScalar();
      }
      return val;
   }

   private static bool IsAuditedType(int type)
   {
      switch (type)
      {
         case 127:
         case 173:
         case 104:
         case 175:
         case 61:
         case 106:
         case 62:
         case 56:
         case 60:
         case 239:
         case 108:
         case 231:
         case 59:
         case 58:
         case 52:
         case 98:
         case 189:
         case 48:
         case 36:
         case 165:
         case 167:
         case 241:
            return true;
         default:
            return false;
      }
   }
   
   private static void SendMessage( string message )
   {
      //SqlContext.Pipe.Send( message );
   }

   #region Create Escaped Strings

   //-----------------------------------------------------------------------
   // CreateSafeString - creates safe string parameter includes
   //                    single quotes; used to create sql parameters
   //-----------------------------------------------------------------------
   static private string CreateSafeString(string propName)
   {
      return CreateSafeString(propName, int.MaxValue);
   }

   //-----------------------------------------------------------------------
   // CreateSafeString - creates safe string parameter includes
   //                    single quotes; used to create sql parameters with
   //                    length limit
   //-----------------------------------------------------------------------
   static private string CreateSafeString(string propName, int limit)
   {
      StringBuilder newName;
      string tmpValue;

      if (propName == null)
      {
         newName = new StringBuilder("null");
      }
      else
      {
         newName = new StringBuilder("");
         tmpValue = propName.Replace("'", "''");
         if (tmpValue.Length > limit)
         {
            if (tmpValue[limit - 1] == '\'')
            {
               limit--;
               if (tmpValue[limit - 1] == '\'')
                  limit--;
            }
            tmpValue = tmpValue.Remove(limit, tmpValue.Length - limit);
         }
         newName.Append(tmpValue);
         //newName.Append("");
      }

      return newName.ToString();
   }
   #endregion
   
   #endregion
}
