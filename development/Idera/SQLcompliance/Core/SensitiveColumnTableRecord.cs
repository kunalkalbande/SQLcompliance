using Idera.SQLcompliance.Core.Agent;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Linq;

namespace Idera.SQLcompliance.Core
{
    public class SensitiveColumnTableRecord
    {
        private static string errMsg = "";

        private int _srvId;
        private int _dbId;
        private int _objectId;
        private string _schemaName = "";
        private string _tableName;
        private bool _selectedColumns;
        private List<string> _columns;
        private string _type;
        private int _columnId;

        public Dictionary<string, int> tableIdMap = new Dictionary<string, int>();
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
            get
            {
                if(_type == CoreConstants.DatasetColumnType)
                {
                    if (_columns != null && _columns.Count > 0)
                        return String.Join(",", _columns.Select(x => x.Substring(0, x.LastIndexOf('.'))).Distinct());
                    else
                        return String.Join(",",_tableName.Split(',').Select(x=>String.Format("{0}.{1}", _schemaName, x)));
                }
                else
                {
                    return String.Format("{0}.{1}", _schemaName, _tableName);
                }
            }
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

        public string[] Columns
        {
            get
            {
                if (!_selectedColumns || _columns == null)
                {
                    string[] allColumns = new string[1];
                    allColumns[0] = "All Columns";
                    return allColumns;
                }
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

        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }
        public int ColumnId
        {
            get { return _columnId; }
            set { _columnId = value; }
        }
        public SensitiveColumnTableRecord()
        {
            _srvId = -1;
            _dbId = -1;
            _objectId = -1;
            _schemaName = "";
            _tableName = "";
            _selectedColumns = false;
            _columns = null;
            _type = "";
            _columnId = -1;
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
        static public List<SensitiveColumnTableRecord> GetAuditedTables(SqlConnection conn,
                                                                        int serverId,
                                                                        int dbId)
        {
            return GetAuditedTables(conn, serverId, dbId, null);
        }

        //-----------------------------------------------------------------------------
        // GetUserTables
        //-----------------------------------------------------------------------------
        static public List<SensitiveColumnTableRecord> GetAuditedTables(SqlConnection conn,
                                                                        int dbId)
        {
            return GetAuditedTables(conn, -1, dbId, null);
        }

        //-----------------------------------------------------------------------------
        // GetUserTables
        //-----------------------------------------------------------------------------
        static public List<SensitiveColumnTableRecord> GetAuditedTables(SqlConnection conn,
                                                                        int serverId,
                                                                        int dbId,
                                                                        int[] objectList)
        {
            StringBuilder where = new StringBuilder(String.Format(" dbId={0} ", dbId));

            if (serverId != -1)
            {
                where.AppendFormat(" AND srvId={0} ", serverId);
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
        static public bool CreateUserTables(SqlConnection conn, List<SensitiveColumnTableRecord> newItems, int serverId, int dbId, SqlTransaction transaction)
        {
            if (newItems == null || newItems.Count == 0)
                return true;

            return UpdateUserTables(conn, newItems, serverId, dbId, transaction);
        }

        //-----------------------------------------------------------------------------
        // UpdateUserTables
        //-----------------------------------------------------------------------------
        static public bool UpdateUserTables(SqlConnection conn,
                                            List<SensitiveColumnTableRecord> newItems,
                                            int serverId,
                                            int dbId,
                                            SqlTransaction transaction, bool importCall = false, string instanceName = "", string databaseName = "", SqlConnection cwfConn= null)
        {
            try
            {
                StringBuilder s = new StringBuilder("");

                // Delete existing tables
                s.Append(GetDeleteAllSQL(serverId, dbId, true));
                string dbName = "";
                // Insert Tables
                int columnId = newItems.OrderBy(i => i.ColumnId).Last().ColumnId;

                SqlConnection agentConnection = new SqlConnection();

                if (importCall)
                {
                    if (cwfConn != null)
                        agentConnection = cwfConn;
                    else
                    agentConnection = SQLcomplianceAgent.GetConnection(instanceName);
                }

                foreach (SensitiveColumnTableRecord x in newItems)
                {
                    if (x.SrvId == -1) x.SrvId = serverId;
                    if (x.DbId == -1) x.DbId = dbId;
                    if (x.Type == CoreConstants.IndividualColumnType)
                    {
                        if (x.Columns[0].Equals("All Columns"))
                        {
                            s.Append(x.GetInsertSQL(true));
                        }
                        else
                        {
                            s.Append(x.GetInsertSQL(true, true));
                        }
                    }
                    else
                    {
                        string oldTableName = x.TableName;
                        string[] tableNames = x.TableName.Split(',');
                        foreach (string table in tableNames)
                        {
                            x.TableName = table;
                            if (importCall)
                            {
                                int objectId = 0;
                                GetTableDetails(agentConnection, databaseName, table, out objectId);
                                x.ObjectId = objectId;
                            }
                            s.Append(x.GetDatasetInsertSQL(true));
                        }
                        x.TableName = oldTableName;
                    }

                    if (x.ColumnId == -1)
                    {
                        columnId++;
                        x.ColumnId = columnId;
                    }

                    if (!((!x.SelectedColumns) && (x.Type == CoreConstants.IndividualColumnType)))
                    {
                        foreach (string colName in x.Columns)
                        {
                            if (!colName.Equals("All Columns"))
                            {
                                SensitiveColumnColumnsRecord col = new SensitiveColumnColumnsRecord();
                                if (importCall)
                                {
                                    int objectId = 0;
                                    List<string> columnArray;
                                    foreach (string table in x.TableName.Split(','))
                                    {
                                        col = new SensitiveColumnColumnsRecord();
                                        GetTableAndColumnDetails(agentConnection, databaseName, table, out objectId, out columnArray);
                                        if (columnArray != null && columnArray.Count > 0 && columnArray.Find(i => x.SchemaName + "." + table + "." + i == colName) != null)
                                        {
                                            col.SrvId = serverId;
                                            col.DbId = dbId;
                                            col.ObjectId = objectId;
                                            col.Name = colName;
                                            col.Type = x.Type;
                                            col.ColumnId = x.ColumnId;
                                            s.Append(col.GetInsertSQL(true));
                                        }
                                        else if (x.Type == CoreConstants.IndividualColumnType && columnArray.Contains(colName))
                                        {
                                            col.SrvId = serverId;
                                            col.DbId = dbId;
                                            col.ObjectId = objectId;
                                            col.Name = colName;
                                            col.Type = x.Type;
                                            col.ColumnId = x.ColumnId;
                                            s.Append(col.GetInsertSQL(true));
                                        }
                                    }
                                }
                                else
                                {
                                    col.SrvId = serverId;
                                    col.DbId = dbId;
                                    col.ObjectId = x.ObjectId;
                                    col.Name = colName;
                                    col.Type = x.Type;
                                    col.ColumnId = x.ColumnId;
                                    s.Append(col.GetInsertSQL(true));
                                }
                            }
                        }
                    }
                }

                if (importCall)
                {
                    agentConnection.Close();
                    agentConnection.Dispose();
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

        private static void GetTableDetails(SqlConnection connection, string databaseName, string tableName, out int objectId)
        {
            objectId = 0;
            string sqlText = string.Format("USE {0};SELECT OBJECT_ID({1});", SQLHelpers.CreateSafeDatabaseName(databaseName), SQLHelpers.CreateSafeString(tableName));

            using (SqlCommand command = new SqlCommand(sqlText, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        objectId = reader.GetInt32(0);
                    }
                }

            }
        }

        private static void GetTableAndColumnDetails(SqlConnection connection, string databaseName, string tableName, out int objectId, out List<string> columnList)
        {
            objectId = 0;
            columnList = new List<string>();
            string sqlText = string.Format("USE {0}; SELECT name, object_id FROM sys.columns WHERE object_id = OBJECT_ID({1});",
                                SQLHelpers.CreateSafeDatabaseName(databaseName),
                                SQLHelpers.CreateSafeString(tableName));

            using (SqlCommand command = new SqlCommand(sqlText, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader.FieldCount > 1)
                        {
                            columnList.Add(reader.GetString(0));
                            objectId = reader.GetInt32(1);
                        }
                    }
                }
            }
        }

        //-----------------------------------------------------------------------------
        // GetMaxColId
        //-----------------------------------------------------------------------------
        static public int? GetMaxColId(SqlConnection conn, SqlTransaction transaction)
        {
            int? maxColId = null;

            try
            {
                string stmt = String.Format("SELECT max" +
                     "(" +
                     "columnId" +
                     ")" +
                     " FROM {0}",
                      CoreConstants.RepositorySensitiveColumnColumnsTable);

                using (SqlCommand cmd = new SqlCommand(stmt, conn))
                {
                    if (transaction != null)
                    {
                        cmd.Transaction = transaction;
                    }
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (!reader.IsDBNull(0))
                            {
                                maxColId = reader.GetInt32(0);
                            }
                        }

                    }

                }

            }

            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Default,
                                         "An error occurred querying MaxColumnId in SensitiveColumnColumns table.",
                                         ex,
                                         ErrorLog.Severity.Error,
                                         true);
                errMsg = ex.Message;
            }

            return maxColId;

        }
        // GetDatabaseObjects
        //-----------------------------------------------------------------------------
        static public List<SensitiveColumnTableRecord> GetDatabaseObjects(SqlConnection conn, string whereClause)
        {
            List<SensitiveColumnTableRecord> retVal;
            List<SensitiveColumnTableRecord> tmpList;

            retVal = new List<SensitiveColumnTableRecord>();
            tmpList = new List<SensitiveColumnTableRecord>();
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
                            SensitiveColumnTableRecord item = new SensitiveColumnTableRecord();
                            item.Load(reader);

                            // Add to list               
                            retVal.Add(item);

                            // Add to the list of selected columns
                            tmpList.Add(item);
                        }
                    }
                }

                // Get selected columns for these tables
                retVal = SensitiveColumnColumnsRecord.GetAuditedColumns(conn, tmpList);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Default,
                                         "An error occurred querying SensitiveColumnTable.",
                                         ex,
                                         ErrorLog.Severity.Error,
                                         true);
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
            _selectedColumns = reader.GetByte(5) == 1 ? true : false;
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
                             ",selectedColumns" +
                             " FROM {0}" +
                             " {1} {2}" +
                             " ORDER by schemaName,tableName ;";

            return string.Format(tmp,
                                  CoreConstants.RepositorySensitiveColumnTablesTable,
                                  (strWhere != "") ? "WHERE " : "",
                                  strWhere);
        }

        //-------------------------------------------------------------
        // GetInsertSQL
        //--------------------------------------------------------------
        public string GetInsertSQL(bool withLocking, bool _selectedColumns = false)
        {
            string tmp =
                "UPDATE {0}" +
                " SET selectedColumns= {7}" +
                " WHERE objectId= {4} AND dbId= {3} AND srvId= {2}" +
                " IF @@ROWCOUNT=0" +
                " INSERT INTO {0} {1}" +
                          "(" +
                             "srvId" +
                             ",dbId" +
                             ",objectId" +
                             ",schemaName" +
                             ",tableName" +
                             ",selectedColumns" +
                          ") VALUES ({2},{3},{4},{5},{6},{7});";
            string schemaName = _schemaName;
            if (schemaName == null)
                schemaName = "";
            return string.Format(tmp,
                                  CoreConstants.RepositorySensitiveColumnTablesTable,
                                  (withLocking) ? "WITH (TABLOCKX) " : "",
                                  _srvId,
                                  _dbId,
                                  _objectId,
                                  SQLHelpers.CreateSafeString(schemaName),
                                  SQLHelpers.CreateSafeString(_tableName),
                                  _selectedColumns ? 1 : 0);
        }

        //-------------------------------------------------------------
        // GetDatasetInsertSQL
        //--------------------------------------------------------------
        public string GetDatasetInsertSQL(bool withLocking)
        {
            string tmp =
                "BEGIN" +
                " IF NOT EXISTS" +
                "(" +
                "SELECT * FROM {0}" +
                " WHERE objectId= {4} and dbId= {3}" +
                ")" +
                "BEGIN" +
                " INSERT INTO {0} {1}" +
                             "(" +
                                "srvId" +
                                ",dbId" +
                                ",objectId" +
                                ",schemaName" +
                                ",tableName" +
                                ",selectedColumns" +
                             ") VALUES ({2},{3},{4},{5},{6},{7})" +
                             " END" +
                             " END;";
            string schemaName = _schemaName;
            if (schemaName == null)
                schemaName = "";
            return string.Format(tmp,
                                  CoreConstants.RepositorySensitiveColumnTablesTable,
                                  (withLocking) ? "WITH (TABLOCKX) " : "",
                                  _srvId,
                                  _dbId,
                                  _objectId,
                                  SQLHelpers.CreateSafeString(schemaName),
                                  SQLHelpers.CreateSafeString(_tableName),
                                  _selectedColumns ? 1 : 0);
        }
        // GetDeleteAllSQL - Create UPDATE SQL to delete all tables and columns for a database
        //--------------------------------------------------------------
        public static string GetDeleteAllSQL(int serverId, int databaseId, bool withLocking)
        {
            string cmdStr = String.Format("DELETE FROM {0} {1} where srvId = {2} and dbId={3};",
                                           CoreConstants.RepositorySensitiveColumnTablesTable,
                                          (withLocking) ? "WITH (TABLOCKX) " : "",
                                           serverId, databaseId);
            return SensitiveColumnColumnsRecord.GetDeleteAllSQL(serverId, databaseId, null, withLocking) + cmdStr;
        }
    }
}
