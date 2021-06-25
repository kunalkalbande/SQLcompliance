using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Idera.SQLcompliance.Core
{
   public class DataChangeTableRecord
   {
      private static string errMsg = "";

      private int _srvId;
      private int _dbId;
      private int _objectId;
      private string _schemaName = "";
      private string _tableName;
      private int _rowLimit;
      private bool _selectedColumns;
      private List<string> _columns;


      public int SrvId
      {
         get { return _srvId; }
         set { _srvId = value; }
      }

      public int DbId
      {
         get { return _dbId; }
         set { _dbId = value; }
      }

      public int ObjectId
      {
         get { return _objectId; }
         set { _objectId = value; }
      }
      
      public string SchemaName
      {
         get
         {
            return _schemaName;
         }
         set
         {
            _schemaName = value;
         }
      }

      public string TableName
      {
         get { return _tableName; }
         set { _tableName = value; }
      }

      public string FullTableName
      {
         get { return String.Format("{0}.{1}", _schemaName, _tableName); }
      }

      public int RowLimit
      {
         get { return _rowLimit; }
         set { _rowLimit = value; }
      }

      public bool SelectedColumns
      {
         get { return _selectedColumns; }
         set
         {
            _selectedColumns = value;
            if (!_selectedColumns)
               _columns = null;
         }
      }

      public string [] Columns
      {
         get
         {
            if (!_selectedColumns || _columns == null)
               return new String[0];
            else
               return _columns.ToArray();
         }
          set
          {
              string[] stringArray = value;

              if (stringArray != null)
              {
                  foreach (var column in value)
                  {
                      AddColumn(column);
                  }
              }
          }
      }
      public string ColumnNames
      {
         get
         {
            if (_selectedColumns)
               return string.Format("Columns:{0}", string.Join(", ", Columns));
            else
               return "All Columns";
         }
      }

      public DataChangeTableRecord()
      {
         _srvId = -1;
         _dbId = -1;
         _objectId = -1;
         _schemaName = "";
         _tableName = "";
         _rowLimit = 10;
         _selectedColumns = false;
         _columns = null;
      }

      public void AddColumn(string name)
      {
         if (_selectedColumns == false)
            _selectedColumns = true;
         if (_columns == null)
            _columns = new List<string>();
         _columns.Add(name);
      }

      static public string GetLastError() 
      { 
         return errMsg; 
      }

      //-----------------------------------------------------------------------------
      // GetUserTables
      //-----------------------------------------------------------------------------
      static public List<DataChangeTableRecord> 
         GetAuditedTables( 
            SqlConnection conn, 
            int serverId, 
            int dbId )
      {
         return GetAuditedTables( conn, serverId, dbId, null );
      }

      //-----------------------------------------------------------------------------
      // GetUserTables
      //-----------------------------------------------------------------------------
      static public List<DataChangeTableRecord>
         GetAuditedTables(
            SqlConnection conn,
            int dbId)
      {
         return GetAuditedTables(conn, -1, dbId, null);
      }

      //-----------------------------------------------------------------------------
      // GetUserTables
      //-----------------------------------------------------------------------------
      static public List<DataChangeTableRecord> 
         GetAuditedTables( 
            SqlConnection conn, 
            int serverId, 
            int dbId,
            int [] objectList )
      {
         StringBuilder where = new StringBuilder(String.Format(" dbId={0} ", dbId));

         if (serverId != -1)
         {
            where.AppendFormat(" AND srvId={0} ", serverId);
         }

         if( objectList != null && objectList.Length > 0 )
         {
            where.Append( " AND objectId in (" );
            for ( int i = 0; i < objectList.Length - 1; i++ )
               where.AppendFormat( "{0}, ", objectList[i] );
            where.AppendFormat( "{0} )", objectList[objectList.Length - 1] );
         }
         return GetDatabaseObjects(conn, where.ToString());
      }

      //-----------------------------------------------------------------------------
      // DeleteUserTables
      //-----------------------------------------------------------------------------
      static public bool DeleteUserTables(SqlConnection conn, int serverId, int dbId, SqlTransaction transaction)
      {
         bool retval = false;

         try
         {
            string sqlStr = GetDeleteAllSQL(serverId, dbId, true);

            SqlCommand cmd = new SqlCommand(sqlStr, conn);
            if (transaction != null)
            {
               cmd.Transaction = transaction;
            }

            cmd.ExecuteNonQuery();

            retval = true;
         }
         catch (Exception ex)
         {
            errMsg = ex.Message;
         }

         return retval;
      }

      //-----------------------------------------------------------------------------
      // CreateUserTables
      //-----------------------------------------------------------------------------
      static public bool CreateUserTables(SqlConnection conn, List<DataChangeTableRecord> newItems, int serverId, int dbId, SqlTransaction transaction)
      {
         if (newItems == null || newItems.Count == 0)
            return true;

         return UpdateUserTables(conn, newItems, serverId, dbId, transaction);
      }

      //-----------------------------------------------------------------------------
      // UpdateUserTables
      //-----------------------------------------------------------------------------
      static public bool UpdateUserTables(SqlConnection conn, List<DataChangeTableRecord> newItems, int serverId, int dbId, SqlTransaction transaction)
      {
         try
         {
            StringBuilder s = new StringBuilder("");

            // Delete existing tables
            s.Append(GetDeleteAllSQL(serverId, dbId, true));

            // Insert Tables
            foreach (DataChangeTableRecord x in newItems)
            {
               if (x.SrvId == -1) x.SrvId = serverId;
               if (x.DbId == -1) x.DbId = dbId;
               s.Append(x.GetInsertSQL(true));
               if (x.SelectedColumns && x.Columns.Length > 0)
               {
                  foreach (string colName in x.Columns)
                  {
                     DataChangeColumnsRecord col = new DataChangeColumnsRecord();
                     col.SrvId = serverId;
                     col.DbId = dbId;
                     col.ObjectId = x.ObjectId;
                     col.Name = colName;
                     s.Append(col.GetInsertSQL(true));
                  }
               }
            }

            //---------
            // Execute 
            //---------
            SqlCommand cmd = new SqlCommand(s.ToString(), conn);
            if (transaction != null)
            {
               cmd.Transaction = transaction;
            }

            cmd.ExecuteNonQuery();
            return true;
         }
         catch (Exception ex)
         {
            errMsg = ex.Message;
            return false;
         }
      }

      //-----------------------------------------------------------------------------
      // GetDatabaseObjects
      //-----------------------------------------------------------------------------
      static public List<DataChangeTableRecord> GetDatabaseObjects(SqlConnection conn, string whereClause)
      {
         List<DataChangeTableRecord> retVal;
         List<DataChangeTableRecord> tmpList;

         retVal = new List<DataChangeTableRecord>();
         tmpList = new List<DataChangeTableRecord>();
         // Load Database Objects
         try
         {
            string cmdstr = GetSelectSQL(whereClause);

            using (SqlCommand cmd = new SqlCommand(cmdstr, conn))
            {
               using (SqlDataReader reader = cmd.ExecuteReader())
               {

                  while (reader.Read())
                  {
                     DataChangeTableRecord item = new DataChangeTableRecord();
                     item.Load(reader);

                     // Add to list               
                     retVal.Add(item);

                     // Add to the list of selected columns are audited
                     if (item.SelectedColumns) 
                        tmpList.Add(item);
                  }
               }
            }

            // Get selected columns for these tables
            DataChangeColumnsRecord.GetAuditedColumns( conn, tmpList);
         }
         catch (Exception ex)
         {
            ErrorLog.Instance.Write( ErrorLog.Level.Default,
                                     "An error occurred querying DataChangeTable.",
                                     ex,
                                     ErrorLog.Severity.Error,
                                     true );
            errMsg = ex.Message;
         }

         return retVal;
      }


      //-------------------------------------------------------------
      // Load - Loads DatabaseRecord from SELECT result set
      //--------------------------------------------------------------
      private void Load(SqlDataReader reader)
      {
         _srvId = reader.GetInt32(0);
         _dbId = reader.GetInt32(1);
         _objectId = reader.GetInt32(2);
         _schemaName = reader.GetString(3);
         _tableName = SQLHelpers.GetString(reader, 4);
         _rowLimit = reader.GetInt32(5);
         _selectedColumns = reader.GetByte(6) == 1 ? true : false;
      }

      //-------------------------------------------------------------
      // GetSelectSQL
      //--------------------------------------------------------------
      private static string GetSelectSQL(string strWhere)
      {
         string tmp = "SELECT srvId" +
                          ",dbId" +
                          ",objectId" +
                          ",schemaName" +
                          ",tableName" +
                          ",rowLimit" +
                          ",selectedColumns" +
                          " FROM {0}" +
                          " {1} {2}" +
                          " ORDER by schemaName,tableName ;";

         return string.Format(tmp,
                               CoreConstants.RepositoryDataChangeTablesTable,
                               (strWhere != "") ? "WHERE " : "",
                               strWhere);
      }

      //-------------------------------------------------------------
      // GetInsertSQL
      //--------------------------------------------------------------
      public string GetInsertSQL(bool withLocking)
      {
         string tmp = "INSERT INTO {0} {1}" +
                          "(" +
                             "srvId" +
                             ",dbId" +
                             ",objectId" +
                             ",schemaName" +
                             ",tableName" +
                             ",rowLimit" +
                             ",selectedColumns" +
                          ") VALUES ({2},{3},{4},{5},{6},{7}, {8});";
         string schemaName = _schemaName;
         if( schemaName == null )
            schemaName = "";
         return string.Format(tmp,
                               CoreConstants.RepositoryDataChangeTablesTable,
                               (withLocking) ? "WITH (TABLOCKX) " : "",
                               _srvId,
                               _dbId,
                               _objectId,
                               SQLHelpers.CreateSafeString(schemaName ),
                               SQLHelpers.CreateSafeString(_tableName),
                               _rowLimit,
                               _selectedColumns ? 1 : 0);
      }

      //-------------------------------------------------------------
      // GetDeleteAllSQL - Create UPDATE SQL to delete all tables and columns for a database
      //--------------------------------------------------------------
      public static string GetDeleteAllSQL(int serverId, int databaseId, bool withLocking)
      {
         string cmdStr = String.Format("DELETE FROM {0} {1} where srvId = {2} and dbId={3};",
                                        CoreConstants.RepositoryDataChangeTablesTable,
                                       (withLocking) ? "WITH (TABLOCKX) " : "",
                                        serverId, databaseId);
         return DataChangeColumnsRecord.GetDeleteAllSQL(serverId, databaseId, null, withLocking) + cmdStr;
      }
   }
}
