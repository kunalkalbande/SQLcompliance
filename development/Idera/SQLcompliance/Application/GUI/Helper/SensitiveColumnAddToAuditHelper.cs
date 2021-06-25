using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Idera.SQLcompliance.Application.GUI.SQL;
using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Agent;
using Idera.SQLcompliance.Core.Event;
using Idera.SQLcompliance.Core.Status;

namespace Idera.SQLcompliance.Application.GUI.Helper
{
    public class SensitiveColumnAddToAuditHelper
    {
        private readonly ServerRecord _server;
        private readonly IEnumerable<DatabaseRecord> _auditedDatabases;
        private readonly Dictionary<string, DatabaseWithAuditedTablesDto> _databasesWithAuditedTables = new Dictionary<string, DatabaseWithAuditedTablesDto>();

        public SensitiveColumnAddToAuditHelper(string instanceName, IEnumerable<string> databasesToLoad)
        {
            _server = ServerRecord.GetServer(Globals.Repository.Connection, instanceName);
            if (_server != null)
            {
                _auditedDatabases = DatabaseRecord
                    .GetDatabases(Globals.Repository.Connection, _server.SrvId);

                foreach (var databaseName in databasesToLoad)
                {
                    _databasesWithAuditedTables[databaseName] = LoadAuditedTablesFromDatabase(databaseName);
                }
            }
        }

        public bool CheckIsDatabaseAudited(string databaseName)
        {
            return FindDatabaseRecordByName(databaseName) != null;
        }

        private DatabaseWithAuditedTablesDto LoadAuditedTablesFromDatabase(string databaseName)
        {
            DatabaseWithAuditedTablesDto result = new DatabaseWithAuditedTablesDto();

            result.Database = FindDatabaseRecordByName(databaseName);

            if (result.Database == null)
            {
                return null;
            }

            result.OldTablesSnapshot = Snapshot.GetDatabaseTables(Globals.Repository.Connection, result.Database.DbId, "\t\t");
            result.OldDCTablesSnapshot = Snapshot.GetDataChangeTables(Globals.Repository.Connection, result.Database.DbId, "\t\t");
            result.OldSCTablesSnapshot = Snapshot.GetSensitiveColumnTables(Globals.Repository.Connection, result.Database.DbId, "\t\t");

            var sql = new SQLDirect();
            sql.OpenConnection(_server.Instance);

            result.AllDatabaseTables = RawSQL.GetTables(sql.Connection, databaseName).Cast<RawTableObject>();
            
            var auditedTables = SensitiveColumnTableRecord.GetAuditedTables(
                Globals.Repository.Connection,
                _server.SrvId,
                result.Database.DbId);
            
            if (auditedTables == null)
            {
                return result;
            }
            
            var auditedTableDtos = auditedTables
                .Select(tr => new SensitiveColumnTableRecordDto() { TableRecord = tr }).ToList();
            
            foreach (var tableRecord in auditedTableDtos.Where(at => !at.TableRecord.SelectedColumns))
            {
                var allColumns = new List<string>();
                foreach (RawColumnObject column in RawSQL.GetColumns(
                    sql.Connection,
                    result.Database.Name,
                    tableRecord.TableRecord.FullTableName))
                {
                    allColumns.Add(column.ColumnName);
                }

                tableRecord.AllColumns = allColumns;
            }

            result.AuditedTables = auditedTableDtos;

            return result;
        }

        public DatabaseWithAuditedTablesDto GetAllAuditedTables(string databaseName)
        {
            return _databasesWithAuditedTables.ContainsKey(databaseName) ? _databasesWithAuditedTables[databaseName] : null;
        }

        public void SaveAll(IEnumerable<DatabaseWithAuditedTablesDto> databases)
        {
            foreach (var database in databases)
            {
                var oldDatabase = database.Database.Clone();

                using (var transaction = Globals.Repository.Connection.BeginTransaction())
                {
                    SaveAuditedTablesInDatabase(database, transaction);

                    database.Database.Connection = Globals.Repository.Connection;
                    database.Database.Write(oldDatabase, transaction);

                    transaction.Commit();
                    string changeLog = Snapshot.DatabaseChangeLog(
                        Globals.Repository.Connection,
                        oldDatabase,
                        database.Database,
                        database.OldTablesSnapshot,
                        database.OldDCTablesSnapshot,
                        database.OldSCTablesSnapshot);

                    // Register change to server and perform audit log
                    ServerUpdate.RegisterChange(database.Database.SrvId,
                        LogType.ModifyDatabase,
                        database.Database.SrvInstance,
                        changeLog);
                }
            }
        }

        private void SaveAuditedTablesInDatabase(DatabaseWithAuditedTablesDto database, SqlTransaction transaction)
        {
            if (database == null)
            {
                return;
            }

            var tableDtosToAudit = database.NewAuditedTables;

            var isEmpty = tableDtosToAudit == null || !tableDtosToAudit.Any();
            var needDelete = isEmpty && GetAllAuditedTables(database.Database.Name) != null &
                             GetAllAuditedTables(database.Database.Name).AuditedTables.Any();

            if (isEmpty && !needDelete)
            {
                return;
            }

            if (needDelete)
            {
                database.Database.AuditSensitiveColumns = false;

                SensitiveColumnTableRecord.DeleteUserTables(
                    Globals.Repository.Connection,
                    _server.SrvId,
                    database.Database.DbId,
                    transaction);
            }
            else
            {
                database.Database.AuditSensitiveColumns = true;

                var maxColumnId = GetMaxColumnId(Globals.Repository.Connection, transaction);
                var tablesToAudit = tableDtosToAudit.Select(dto => dto.TableRecord).ToList();
                foreach (var table in tablesToAudit.Where(t => t.ColumnId == -1))
                {
                    table.ColumnId = ++maxColumnId;
                }

                SensitiveColumnTableRecord.UpdateUserTables(
                    Globals.Repository.Connection,
                    tablesToAudit,
                    _server.SrvId,
                    database.Database.DbId,
                    transaction
                );
            }
        }

        private int GetMaxColumnId(SqlConnection conn, SqlTransaction transaction)
        {
            
            using (var cmd = conn.CreateCommand())
            {
                cmd.Transaction = transaction;

                cmd.CommandText = string.Format("SELECT isnull(max(columnId), 0) FROM {0}",
                    CoreConstants.RepositorySensitiveColumnColumnsTable);
                return (int)cmd.ExecuteScalar();
            }
        }

        private DatabaseRecord FindDatabaseRecordByName(string tableName)
        {
            return _auditedDatabases != null
                ? _auditedDatabases
                    .FirstOrDefault(dt => dt.Name.Equals(tableName, StringComparison.InvariantCultureIgnoreCase))
                : null;
        }
    }

    public class DatabaseWithAuditedTablesDto
    {
        public DatabaseWithAuditedTablesDto()
        {
            AllDatabaseTables = Enumerable.Empty<RawTableObject>();
            AuditedTables = Enumerable.Empty<SensitiveColumnTableRecordDto>();
            NewAuditedTables = Enumerable.Empty<SensitiveColumnTableRecordDto>();
        }

        public string OldTablesSnapshot { get; set; }
        public string OldDCTablesSnapshot { get; set; }
        public string OldSCTablesSnapshot { get; set; }

        public DatabaseRecord Database { get; set; }
        public IEnumerable<RawTableObject> AllDatabaseTables { get; set; }
        public IEnumerable<SensitiveColumnTableRecordDto> AuditedTables { get; set; }
        public IEnumerable<SensitiveColumnTableRecordDto> NewAuditedTables { get; set; }
    }

    public class SensitiveColumnTableRecordDto
    {
        public SensitiveColumnTableRecord TableRecord { get; set; }
        public IEnumerable<string> AllColumns { get; set; }
    }
}
