using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Windows.Forms;
using Idera.SQLcompliance.Application.GUI.Helper;
using Idera.SQLcompliance.Core;
using Infragistics.Win.UltraMessageBox;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
    public partial class Form_SensitiveColumnsAddToAudit : Form
    {
        private readonly SensitiveColumnAddToAuditHelper _helper;
        private readonly string _instanceName;
        private readonly string _selectedDatabase;
        private readonly string _selectedTable;
        private readonly IEnumerable<string> _databases;
        private readonly IEnumerable<RawColumnDetails> _sensitiveColumns;
        private readonly HashSet<string> _userActions = new HashSet<string>();
        public Form_SensitiveColumnsAddToAudit()
        {
            InitializeComponent();
        }

        public Form_SensitiveColumnsAddToAudit(
            string instanceName,
            IEnumerable<RawColumnDetails> sensitiveColumns,
            string selectedDatabase = null,
            string selectedTable = null
        ) : this()
        {
            _instanceName = instanceName;
            _sensitiveColumns = sensitiveColumns;
            _selectedDatabase = selectedDatabase;
            _selectedTable = selectedTable;
            _databases = _sensitiveColumns.Select(sc => sc.DatabaseName).Distinct().OrderBy(d => d).ToList();
            _helper = new SensitiveColumnAddToAuditHelper(_instanceName, _databases);
        }

        private void FillGrids()
        {
            var auditedColumns = FillAuditedGrid();

            foreach (var rawColumnDetails in _sensitiveColumns.OrderBy(sc => sc.DatabaseName).ThenBy(sc => sc.TableName)
                .ThenBy(sc => sc.FieldName))
            {
                if (!auditedColumns.Contains(Tuple.Create(rawColumnDetails.DatabaseName, rawColumnDetails.TableName,
                    rawColumnDetails.FieldName)))
                {
                    var isAuditedDatabase = _helper.CheckIsDatabaseAudited(rawColumnDetails.DatabaseName);

                    var shouldBeCheckedByDefault =
                        isAuditedDatabase
                        && string.Equals(_selectedDatabase, rawColumnDetails.DatabaseName,
                            StringComparison.InvariantCultureIgnoreCase)
                        && string.Equals(_selectedTable, rawColumnDetails.TableName,
                            StringComparison.InvariantCultureIgnoreCase);

                    ultraDataSourceAvailable.Rows.Add(true, new object[]
                    {
                        shouldBeCheckedByDefault,
                        rawColumnDetails.DatabaseName,
                        rawColumnDetails.TableName,
                        rawColumnDetails.FieldName
                    });

                    if (!isAuditedDatabase)
                    {
                        var gridRow = ultraGridAvailable.Rows.Last();
                        gridRow.Activation = Activation.Disabled;
                    }
                }
            }

            UpdateAddButtonState();
        }

        private HashSet<Tuple<string, string, string>> FillAuditedGrid()
        {
            var auditedColumns = new HashSet<Tuple<string, string, string>>();

            foreach (var databaseName in _databases.Where(db => _helper.CheckIsDatabaseAudited(db)).OrderBy(db => db))
            {
                var dto = _helper.GetAllAuditedTables(databaseName);
                if (dto.AuditedTables != null)
                {
                    foreach (var auditedTable in dto.AuditedTables
                        .Where(t => t.TableRecord.Type == CoreConstants.IndividualColumnType)
                        .OrderBy(t => t.TableRecord.FullTableName))
                    {
                        var tableName = auditedTable.TableRecord.FullTableName;
                        foreach (var columnName in (auditedTable.TableRecord.SelectedColumns
                            ? auditedTable.TableRecord.Columns
                            : auditedTable.AllColumns).OrderBy(c => c))
                        {
                            auditedColumns.Add(Tuple.Create(databaseName, tableName,
                                columnName));
                            ultraDataSourceAudited.Rows.Add(true, new object[]
                            {
                                false,
                                databaseName,
                                tableName,
                                columnName
                            });
                        }
                    }
                }
            }

            return auditedColumns;
        }

        private void Form_SensitiveColumnsAddToAudit_Load(object sender, EventArgs e)
        {
            try
            {
                FillGrids();
            }
            catch (Exception ex)
            {
                UltraMessageBoxManager.Show(ex.Message,
                    "SQL Compliance Manager",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private IEnumerable<UltraDataRow> GetCheckedRows(UltraDataSource dataSource)
        {
            return
                    dataSource
                        .Rows
                        .Cast<UltraDataRow>()
                        .Where(dr => (bool)dr.GetCellValue("IsChecked"));
        }

        private void MoveSelectedRows(UltraDataSource from, UltraDataSource to)
        {
            var checkedRows = GetCheckedRows(from).ToList();
            if (checkedRows.Any())
            {
                foreach (var row in checkedRows)
                {
                    RegisterUserAction(
                        (string)row.GetCellValue(1),
                        (string)row.GetCellValue(2),
                        (string)row.GetCellValue(3));

                    to.Rows.Add(true, new object[]
                    {
                        false,
                        row.GetCellValue(1),
                        row.GetCellValue(2),
                        row.GetCellValue(3)
                    });
                    from.Rows.Remove(row);
                }

                UpdateSaveButtonState();
            }
        }

        private void UpdateSaveButtonState()
        {
            ubtnSave.Enabled = _userActions.Any();
        }

        private void RegisterUserAction(string databaseName, string tableName, string columnName)
        {
            var fullColumnName = databaseName + "," +
                                 tableName + "," +
                                 columnName;
            if (_userActions.Contains(fullColumnName))
            {
                _userActions.Remove(fullColumnName);
            }
            else
            {
                _userActions.Add(fullColumnName);
            }
        }

        private void ubtnAdd_Click(object sender, EventArgs e)
        {
            MoveSelectedRows(ultraDataSourceAvailable, ultraDataSourceAudited);
            ubtnAdd.Enabled = false;
        }

        private void ubtnRemove_Click(object sender, EventArgs e)
        {
            MoveSelectedRows(ultraDataSourceAudited, ultraDataSourceAvailable);
            ubtnRemove.Enabled = false;
        }

        private void ubtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                _helper.SaveAll(GetDataToSave());
                UltraMessageBoxManager.Show(this,
                    string.Format("Selected columns have been successfully added to {0}.", _instanceName),
                    "Columns Added to Audit",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                Close();
            }
            catch (Exception ex)
            {
                UltraMessageBoxManager.Show(ex.Message,
                    "SQL Compliance Manager",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private List<DatabaseWithAuditedTablesDto> GetDataToSave()
        {
            var dataToSave = new List<DatabaseWithAuditedTablesDto>();

            var columnsToAuditByDatabase = ultraDataSourceAudited.Rows.Cast<UltraDataRow>()
                .ToLookup(dr => (string)dr.GetCellValue(1));

            foreach (var databaseName in _databases.Where(db => _helper.CheckIsDatabaseAudited(db)))
            {
                var databaseGroup = columnsToAuditByDatabase[databaseName];

                DatabaseWithAuditedTablesDto auditedDatabaseWithAuditedTables = _helper.GetAllAuditedTables(databaseName);
                if (auditedDatabaseWithAuditedTables == null)
                {
                    throw new InstanceNotFoundException();
                }

                var tablesToSave = new List<SensitiveColumnTableRecord>();

                if (auditedDatabaseWithAuditedTables.AuditedTables != null)
                {
                    tablesToSave.AddRange(auditedDatabaseWithAuditedTables.AuditedTables
                        .Where(t => t.TableRecord.Type != CoreConstants.IndividualColumnType)
                        .Select(t => t.TableRecord));
                }

                var columnsByTable =
                    databaseGroup.GroupBy(dr => (string)dr.GetCellValue(2), dr => (string)dr.GetCellValue(3));

                foreach (var tableGroup in columnsByTable)
                {
                    var tableName = tableGroup.Key;

                    var auditedTable = auditedDatabaseWithAuditedTables.AuditedTables != null
                        ? auditedDatabaseWithAuditedTables.AuditedTables.FirstOrDefault(t =>
                            t.TableRecord.Type == CoreConstants.IndividualColumnType &&
                            t.TableRecord.FullTableName == tableName)
                        : null;

                    if (
                        auditedTable != null
                        && !auditedTable.TableRecord.SelectedColumns
                        && !auditedTable.AllColumns.Except(tableGroup).Any())
                    {
                        tablesToSave.Add(auditedTable.TableRecord);
                        continue;
                    }

                    var rawTableObject =
                        auditedDatabaseWithAuditedTables.AllDatabaseTables.Single(t => t.FullTableName == tableName);

                    var tableRecord = new SensitiveColumnTableRecord()
                    {
                        SelectedColumns = true,
                        Type = CoreConstants.IndividualColumnType,
                        SrvId = -1,
                        DbId = -1,
                        ColumnId = -1,
                        SchemaName = rawTableObject.SchemaName,
                        TableName = rawTableObject.TableName,
                        ObjectId = rawTableObject.ObjectID,
                        Columns = tableGroup.ToArray()
                    };

                    tablesToSave.Add(tableRecord);
                }

                auditedDatabaseWithAuditedTables.NewAuditedTables =
                    tablesToSave.Select(t => new SensitiveColumnTableRecordDto() {TableRecord = t});

                dataToSave.Add(auditedDatabaseWithAuditedTables);
            }

            return dataToSave;
        }

        private void ubtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ultraGridAvailable_AfterCellUpdate(object sender, CellEventArgs e)
        {
            UpdateAddButtonState();
        }

        private void UpdateAddButtonState()
        {
            ubtnAdd.Enabled = GetCheckedRows(ultraDataSourceAvailable).Any();
        }

        private void ultraGridAudited_AfterCellUpdate(object sender, CellEventArgs e)
        {
            UpdateRemoveButtonState();
        }

        private void UpdateRemoveButtonState()
        {
            ubtnRemove.Enabled = GetCheckedRows(ultraDataSourceAudited).Any();
        }

        private void ultraGrid_CellChange(object sender, CellEventArgs e)
        {
            if (e.Cell.Column.DataType == typeof(bool))
            {
                e.Cell.Row.Update();
            }
        }
    }
}
