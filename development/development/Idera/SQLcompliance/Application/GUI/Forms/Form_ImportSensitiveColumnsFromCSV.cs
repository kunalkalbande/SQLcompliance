using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Idera.SQLcompliance.Application.GUI.Helper;
using Idera.SQLcompliance.Application.GUI.Properties;
using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Agent;
using Idera.SQLcompliance.Core.Collector;
using Idera.SQLcompliance.Core.Remoting;
using StringComparer = System.StringComparer;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
    public partial class Form_ImportSensitiveColumnsFromCSV : Form
    {
        private readonly ServerRecord _server;
        public DatabaseRecord _db;
        private DatabaseRecord _oldDb;
        private ICollection _tableList;
        private Dictionary<string, DatabaseObjectRecord> _tableObjects = null;
        private bool _updatingTreeView;

        public Form_ImportSensitiveColumnsFromCSV(ServerRecord server)
        {
            InitializeComponent();

            _server = server;
            Icon = Resources.SQLcompliance_product_ico;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            treeSensitiveColumns.Nodes.Clear();

            var dlgbrowseCsv = new OpenFileDialog();
            dlgbrowseCsv.Title = @"Select CSV file to import.";
            dlgbrowseCsv.Filter = @"CSV files|*.CSV";
            dlgbrowseCsv.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (dlgbrowseCsv.ShowDialog() != DialogResult.OK)
                return;

            txtCSVFile.Text = dlgbrowseCsv.FileName;

            btnOK.Enabled = false;
            treeSensitiveColumns.SuspendLayout();
            treeSensitiveColumns.Nodes.Clear();

            var sensitiveColumnLines = File.ReadAllLines(txtCSVFile.Text, Encoding.ASCII);
            if (sensitiveColumnLines.Length == 0)
                return;

            Array.Sort(sensitiveColumnLines, StringComparer.InvariantCulture); //TODO: optimize using dictionary
            for (var index = 0; index < sensitiveColumnLines.Length; index += 1)
            {
                var sensitiveColumnDetail = sensitiveColumnLines[index].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (sensitiveColumnDetail.Length < 2)
                    continue;

                var databaseName = sensitiveColumnDetail[0].Trim();
                var scTableName = sensitiveColumnDetail[1].Trim();
                var allColumns = sensitiveColumnDetail.Length == 2;

                // load database list via agent (if deployed)
                if (_server.IsDeployed && _server.IsRunning)
                {
                    var url = EndPointUrlBuilder.GetUrl(typeof(AgentManager), Globals.SQLcomplianceConfig.Server, Globals.SQLcomplianceConfig.ServerPort);
                    try
                    {
                        var manager = GUIRemoteObjectsProvider.AgentManager();

                        // get list of all databses on server, leaving out 'tempdb'
                        var dbList = manager.GetRawUserDatabases(_server.Instance);
                        var sysList = manager.GetRawSystemDatabases(_server.Instance);
                        if (dbList != null && sysList != null)
                        {
                            foreach (object o in sysList)
                                if (o.ToString() != "tempdb")
                                    dbList.Add(o);
                        }

                        foreach (RawDatabaseObject rdo in dbList)
                        {
                            if (!rdo.name.Equals(databaseName, StringComparison.InvariantCultureIgnoreCase))
                                continue;

                            var databaseNode = treeSensitiveColumns.Nodes.ContainsKey(databaseName)
                                ? treeSensitiveColumns.Nodes[databaseName]
                                : treeSensitiveColumns.Nodes.Add(databaseName, databaseName);
                            databaseNode.Checked = true;

                            _tableList = manager.GetRawTables(_server.Instance, rdo.name);
                            foreach (RawTableObject table in _tableList)
                            {
                                if (!table.FullTableName.Equals(scTableName, StringComparison.InvariantCultureIgnoreCase) &&
                                    !table.FullTableName.Equals(String.Format("DBO.{0}", scTableName), StringComparison.InvariantCultureIgnoreCase))
                                    continue;

                                var tableNode = databaseNode.Nodes.ContainsKey(table.FullTableName)
                                    ? databaseNode.Nodes[table.FullTableName]
                                    : databaseNode.Nodes.Add(table.FullTableName, table.FullTableName);

                                var dbo = new DatabaseObjectRecord(table);
                                tableNode.Tag = dbo;
                                tableNode.Checked = true;

                                ICollection columnList = manager.GetRawColumns(_server.Instance, databaseName, table.FullTableName);

                                if (!allColumns)
                                {
                                    for (var columnIndex = 2;
                                        columnIndex < sensitiveColumnDetail.Length;
                                        columnIndex += 1)
                                    {
                                        var columnName = sensitiveColumnDetail[columnIndex].Trim();
                                        foreach (RawColumnObject rco in columnList)
                                        {
                                            if (!rco.ColumnName.Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                                                continue;

                                            if (!tableNode.Nodes.ContainsKey(columnName))
                                            {
                                                var columnNode = tableNode.Nodes.Add(rco.ColumnName, rco.ColumnName);
                                                columnNode.Checked = true;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (RawColumnObject rco in columnList)
                                    {
                                        if (!tableNode.Nodes.ContainsKey(rco.ColumnName))
                                        {
                                            var columnNode = tableNode.Nodes.Add(rco.ColumnName, rco.ColumnName);
                                            columnNode.Checked = true;
                                        }
                                    }
                                }

                                if (tableNode.Nodes.Count == 0)
                                    databaseNode.Nodes.Remove(tableNode);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.Instance.Write(ErrorLog.Level.Verbose, String.Format("LoadDatabases: URL: {0} Instance {1}", url, _server.Instance), ex, ErrorLog.Severity.Warning);
                    }
                }
            }
            btnOK.Enabled = treeSensitiveColumns.Nodes.Count > 0;
            treeSensitiveColumns.ResumeLayout();
        }

        private void sscTreeViewFromCSV_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (_updatingTreeView)
                return;
            if (e.Node.Level < 2)
            {
                if (e.Node.Nodes.Count <= 0) return;
                foreach (TreeNode node in e.Node.Nodes)
                {
                    node.Checked = e.Node.Checked;
                    if (node.Nodes.Count <= 0) continue;
                    foreach (TreeNode cNode in node.Nodes)
                    {
                        cNode.Checked = e.Node.Checked;
                        if (cNode.Nodes.Count > 0)
                        {
                            cNode.Checked = e.Node.Checked;
                        }
                    }
                }
            }
            else
            {
                if (_updatingTreeView) return;
                _updatingTreeView = true;
                SelectParents(e.Node, e.Node.Checked);
                _updatingTreeView = false;
            }
        }

        private void SelectParents(TreeNode node, Boolean isChecked)
        {
            var parent = node.Parent;

            if (parent == null)
                return;

            if (!isChecked && HasCheckedNode(parent))
                return;

            parent.Checked = isChecked;
            SelectParents(parent, isChecked);
        }

        private static bool HasCheckedNode(TreeNode node)
        {
            return node.Nodes.Cast<TreeNode>().Any(n => n.Checked);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            var currentTitle = Text;

            Text = @"Importing sensitive columns from CSV, please wait...";
            Cursor = Cursors.WaitCursor;
            txtCSVFile.Enabled = false;
            btnBrowse.Enabled = false;
            treeSensitiveColumns.Enabled = false;
            btnOK.Enabled = false;
            btnCancel.Enabled = false;

            var transaction = Globals.Repository.Connection.BeginTransaction();

            var conn = new SqlConnection();
            conn.ConnectionString = _server.Connection.ConnectionString;
            conn.Open();

            try
            {
                var updateSqlBuilder = new StringBuilder();
                foreach (TreeNode dNode in treeSensitiveColumns.Nodes)
                {
                    if (!dNode.Checked)
                        continue;

                    var dbId = DatabaseRecord.GetDatabaseId(conn, _server.SrvId, dNode.Name);
                    var oldSCLists = SensitiveColumnTableRecord.GetAuditedTables(conn, _server.SrvId, dbId);
                    List<SensitiveColumnTableRecord> tableRecords = null;
                    if (oldSCLists != null)
                    {
                        tableRecords = new List<SensitiveColumnTableRecord>();
                        for (int i=0;i<oldSCLists.Count;i++)
                        {
                            if (oldSCLists[i].Type != CoreConstants.IndividualColumnType)
                            {
                                foreach (string table in oldSCLists[i].FullTableName.Split(','))
                                {
                                    SensitiveColumnTableRecord newRecord = new SensitiveColumnTableRecord();
                                    newRecord.TableName = table.Split('.')[1];
                                    if (oldSCLists[i].tableIdMap.ContainsKey(table))
                                        newRecord.ObjectId = oldSCLists[i].tableIdMap[table];
                                    else
                                        newRecord.ObjectId = oldSCLists[i].ObjectId;
                                    newRecord.SchemaName = table.Split('.')[0];
                                    newRecord.SelectedColumns = oldSCLists[i].SelectedColumns;
                                    newRecord.SrvId = oldSCLists[i].SrvId;
                                    newRecord.Columns = Array.FindAll(oldSCLists[i].Columns, x => x.Contains(table + "."));
                                    newRecord.DbId = oldSCLists[i].DbId;
                                    newRecord.ColumnId = oldSCLists[i].ColumnId;
                                    newRecord.Type = oldSCLists[i].Type;
                                    tableRecords.Add(newRecord);
                                }
                                oldSCLists.Remove(oldSCLists[i]);
                                i--;
                            }
                        }
                    }

                    if (tableRecords != null && tableRecords.Count > 0)
                    {
                        oldSCLists.AddRange(tableRecords);
                    }
                    var retVal = new List<SensitiveColumnTableRecord>();
                    foreach (TreeNode tNode in dNode.Nodes)
                    {
                        if (!tNode.Checked)
                            continue;

                        // Ignore our tables
                        if (tNode.Tag == null || tNode.Nodes.Count == 0 || CoreConstants.Agent_BeforeAfter_TableName.Equals(tNode.Name))
                            continue;

                        var dor = (DatabaseObjectRecord)tNode.Tag;

                        var sctItem = new SensitiveColumnTableRecord();
                        sctItem.SchemaName = dor.SchemaName;
                        sctItem.TableName = dor.TableName;
                        sctItem.ObjectId = dor.Id;
                        sctItem.SelectedColumns = true;
                        sctItem.Type = CoreConstants.IndividualColumnType;

                        foreach (TreeNode col in tNode.Nodes)
                            if (col.Checked)
                                sctItem.AddColumn(col.Name);

                        retVal.Add(sctItem);

                    }

                    var isFound = false;
                    if (oldSCLists != null)
                    {
                        foreach (var oldSCList in oldSCLists)
                        {
                            foreach (var newSCList in retVal)
                            {
                                if (oldSCList.Type == CoreConstants.IndividualColumnType && oldSCList.FullTableName.Equals(newSCList.FullTableName, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    isFound = true;
                                    break;
                                }
                            }

                            if (!isFound)
                                retVal.Add(oldSCList);

                            isFound = false;
                        }
                    }

                    SensitiveColumnTableRecord.CreateUserTables(Globals.Repository.Connection, retVal, _server.SrvId, dbId, transaction);

                    updateSqlBuilder.AppendFormat("UPDATE {0} SET auditSensitiveColumns = 1 WHERE dbId={1} AND srvId={2} AND name='{3}';{4}", CoreConstants.RepositoryDatabaseTable, dbId, _server.SrvId, dNode.Name, Environment.NewLine);
                }

                var command = new SqlCommand(updateSqlBuilder.ToString(), Globals.Repository.Connection, transaction);
                command.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                transaction.Rollback();
                var errorMsg = ex.Message;

                MessageBox.Show(errorMsg, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                transaction.Commit();
                conn.Close();
                Text = currentTitle;
                Cursor = Cursors.Default;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}