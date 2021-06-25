using Idera.SQLcompliance.Application.GUI.Helper;
using Idera.SQLcompliance.Application.GUI.Properties;
using Idera.SQLcompliance.Application.GUI.SQL;
using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Agent;
using Idera.SQLcompliance.Core.Collector;
using Idera.SQLcompliance.Core.Remoting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
    public partial class Form_ConfigureSensitiveColumn : Form
    {
        private ServerRecord _server;
        private DatabaseRecord _oldDb;
        private SQLDirect _sqlServer = null;
        private ICollection _tableList = null;
        private bool _tryAgentCommunication = false;
        private Dictionary<string, DatabaseObjectRecord> _tableObjects = null;
        private string txtServer;
        private string txtName;
        private Dictionary<string, List<SensitiveColumnTableRecord>> _oldSCTables;
        private Dictionary<string, ListView> _dictOldLvSCTables;

        public ListView LvSCTables
        {
            get { return _lvSCTables; }
        }

        public Form_ConfigureSensitiveColumn(ServerRecord server, DatabaseRecord inDb, Dictionary<string, ListView> _dictLvSCTables)
        {
            InitializeComponent();
            _sqlServer = new SQLDirect();
            _server = server;
            _oldDb = inDb;
            txtServer = server.Instance;
            txtName = inDb.Name;
            _dictOldLvSCTables = _dictLvSCTables;
            _btnEditSCTable.Enabled = false;
            _btnRemoveSCTable.Enabled = false;

            if (_dictLvSCTables.Count > 0)
            {
                Dictionary<string, string> currentColumns = new Dictionary<string, string>();
                Dictionary<string, string> currentColumnType = new Dictionary<string, string>();
                ListView _lvOldSCTables = new ListView();

                if (_dictLvSCTables.TryGetValue(inDb.Name, out _lvOldSCTables))
                {
                    if (_lvOldSCTables.Items.Count != 0)
                    {
                        foreach (ListViewItem tableItem in _lvOldSCTables.Items)
                        {
                            //Type of the current existing item in the list
                            ListViewItem x = _lvSCTables.Items.Add(tableItem.Text);
                            if (tableItem.SubItems[2].Text.Equals(UIConstants.SC_Individual))
                            {
                                currentColumns[tableItem.Text] = tableItem.SubItems[1].Text;
                            }
                            else
                            {
                                IList allColumns;
                                allColumns = new ArrayList();
                                Char delimiter = ',';
                                String[] dataSetTables = tableItem.Text.Split(delimiter);
                                foreach (string dataSetTable in dataSetTables)
                                {
                                    IList columnList = null;
                                    columnList = LoadColumns(dataSetTable);
                                    foreach (var o in columnList)
                                    {
                                        allColumns.Add(dataSetTable + "." + o);
                                    }
                                }
                                string[] dataSetSelectedColumns = tableItem.SubItems[1].Text.Split(delimiter);
                                currentColumns[tableItem.Text] = (dataSetSelectedColumns.Length == allColumns.Count ? UIConstants.SC_AllColumns : tableItem.SubItems[1].Text);
                            }
                            currentColumnType[tableItem.Text] = tableItem.SubItems[2].Text;
                            x.SubItems.Add(currentColumns[tableItem.Text]);
                            x.SubItems.Add(currentColumnType[tableItem.Text]);
                            x.Tag = tableItem.Tag;
                        }
                        _lvSCTables.Focus();
                        if (_lvSCTables.Items.Count > 0)
                        {
                            //_lvSCTables.TopItem.Selected = true;
                        }
                        _lvSCTables.EndUpdate();
                    }
                }
            }

            else
            {
                try
                {
                    LoadSensitiveColumns();
                }
                catch (Exception e)
                {
                    MessageBox.Show("Problem loading Sensitive Column auditing information: " + e, "Error");
                }
            }

            try
            {
                ServerRecord sr = new ServerRecord();
                sr.Connection = Globals.Repository.Connection;
                sr.Read(txtServer);
                _tryAgentCommunication = sr.IsDeployed && sr.IsRunning;
            }
            catch { }
        }

        private void LoadSensitiveColumns()
        {
            if (_server.SqlVersion == 0)
            {
                _pnlSensitiveColumns.Visible = false;
                return;
            }
            else if (!SupportsSensitiveColumns())
            {
                _pnlSensitiveColumns.Visible = false;
                return;
            }
            if (!LoadTables())
            {
                _pnlSensitiveColumns.Visible = false;
                return;
            }

            //Load the Sensitive column info
            _oldSCTables = new Dictionary<string, List<SensitiveColumnTableRecord>>();
            if (_oldDb.AuditSensitiveColumns)
            {
                List<string> missingTables = new List<string>();
                List<SensitiveColumnTableRecord> tables = SensitiveColumnTableRecord.GetAuditedTables(Globals.Repository.Connection, _server.SrvId, _oldDb.DbId);
                string tableName;
                if (tables != null)
                {
                    foreach (SensitiveColumnTableRecord table in tables)
                    {
                        if (SupportsSchemas())
                        {
                            tableName = table.FullTableName;
                        }
                        else
                            tableName = table.TableName;

                        ListViewItem x = _lvSCTables.Items.Add(tableName);
                        if (table.Type.Equals(UIConstants.SC_Individual))
                        {
                            x.SubItems.Add(table.SelectedColumns ? Form_TableConfigure.GetColumnString(table.Columns) : UIConstants.SC_AllColumns);
                        }
                        else
                        {
                            IList allColumns;
                            allColumns = new ArrayList();
                            Char delimiter = ',';
                            String[] substrings = tableName.Split(delimiter);
                            foreach (string substring in substrings)
                            {
                                IList columnList = null;
                                columnList = LoadColumns(substring);
                                foreach (var o in columnList)
                                {
                                    allColumns.Add(substring + "." + o);
                                }
                            }
                            x.SubItems.Add(table.Columns.Length == allColumns.Count ? UIConstants.SC_AllColumns : Form_TableConfigure.GetColumnString(table.Columns));
                        }
                        x.SubItems.Add(table.Type);

                        if (tableName.Contains(","))
                        {
                            Char delimiter = ',';
                            String[] substrings = tableName.Split(delimiter);
                            List<DatabaseObjectRecord> listDataObject = new List<DatabaseObjectRecord>();
                            foreach (var substring in substrings)
                            {
                                if (_tableObjects.ContainsKey(substring))
                                {
                                    listDataObject.Add(_tableObjects[substring]);
                                }
                            }
                            x.Tag = listDataObject;
                        }
                        else if (!_tableObjects.ContainsKey(tableName))
                        {
                            missingTables.Add(table.FullTableName);
                            x.Tag = null;
                            x.ForeColor = System.Drawing.Color.LightGray;
                        }
                        else
                            x.Tag = _tableObjects[tableName];
                    }
                }
            }

            SelectedIndexChanged_lvSCTables(null, null);
            if (_lvSCTables.Items.Count == 0)
                _btnConfigureSC.Enabled = false;//SQlCM-5747 v5.6
        }

        private void SelectedIndexChanged_lvSCTables(object sender, EventArgs e)
        {
            if (_lvSCTables.SelectedItems.Count == 0)
            {
                _btnRemoveSCTable.Enabled = false;
                _btnEditSCTable.Enabled = false;
            }
            else
            {
                _btnRemoveSCTable.Enabled = true;
                _btnEditSCTable.Enabled = _lvSCTables.SelectedItems.Count == 1 && _lvSCTables.SelectedItems[0] != null ? true : false;
            }
        }

        private void _btnAddSCTable_Click(object sender, EventArgs e)
        {
            try
            {
                if (!LoadTables())
                {
                    ErrorMessage.Show(this.Text, UIConstants.Error_CantLoadTables, SQLDirect.GetLastError());
                    _btnAddSCTable.Enabled = false;
                    return;
                }
                List<string> selectedTables = new List<string>();
                List<RawTableObject> tableList = new List<RawTableObject>();

                foreach (ListViewItem tableItem in _lvSCTables.Items)
                {
                    //Type of the current existing item in the list
                    string tableType = tableItem.SubItems[2].Text;

                    if (tableType != UIConstants.SC_Dataset)
                        selectedTables.Add(tableItem.Text);
                }

                foreach (RawTableObject rto in _tableList)
                {
                    tableList.Add(rto);
                }

                Form_TableAdd frm = new Form_TableAdd(tableList, selectedTables, SupportsSchemas(), UIConstants.Table_Column_Usage.SensitiveColumns);

                if (DialogResult.OK == frm.ShowDialog())
                {
                    Dictionary<string, string> currentColumns = new Dictionary<string, string>();
                    Dictionary<string, string> currentColumnType = new Dictionary<string, string>();
                    _lvSCTables.BeginUpdate();

                    foreach (ListViewItem item in _lvSCTables.Items)
                    {
                        currentColumns[item.Text] = item.SubItems[1].Text;
                        currentColumnType[item.Text] = item.SubItems[2].Text;

                        if (item.SubItems[2].Text == UIConstants.SC_Individual)
                        {
                            item.Remove();
                        }

                    }
                    //_lvSCTables.Items.Clear();
                    if (frm.SelectedTables.Count != 0)
                    {
                        foreach (string tableName in frm.SelectedTables)
                        {
                            ListViewItem x = _lvSCTables.Items.Add(tableName);
                            string value = UIConstants.SC_Individual;
                            string found = null;
                            //if (currentColumns.ContainsKey(tableName))
                            if (currentColumnType.TryGetValue(tableName, out found) && found == value)
                            {
                                x.SubItems.Add(currentColumns[tableName]);
                                x.SubItems.Add(UIConstants.SC_Individual);
                            }
                            else
                            {
                                // This is a new table, so add with the default values
                                x.SubItems.Add(UIConstants.BAD_AllColumns);
                                x.SubItems.Add(UIConstants.SC_Individual);
                            }

                            if (_tableObjects.ContainsKey(tableName))
                            {
                                x.Tag = _tableObjects[tableName];
                            }
                            else
                                x.Tag = null;
                        }
                    }

                    _lvSCTables.Focus();

                    if (_lvSCTables.Items.Count > 0)
                    {
                        //_lvSCTables.TopItem.Selected = true;
                        _btnConfigureSC.Enabled = true;//SQlCM-5747 v5.6
                    }
                    _lvSCTables.EndUpdate();
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("Problem adding Sensitive Columns for auditing: " + ex, "Error");
            }
        }

        private void _btnAddSCDataSet_Click(object sender, EventArgs e)
        {
            try
            {
                if (!LoadTables())
                {
                    ErrorMessage.Show(this.Text, UIConstants.Error_CantLoadTables, SQLDirect.GetLastError());
                    _btnAddSCDataSet.Enabled = false;
                    return;
                }
                List<string> selectedTables = new List<string>();
                List<RawTableObject> tableList = new List<RawTableObject>();
                foreach (RawTableObject rto in _tableList)
                {
                    tableList.Add(rto);
                }

                Form_TableAdd frm = new Form_TableAdd(tableList, selectedTables, SupportsSchemas(), UIConstants.Table_Column_Usage.SensitiveColumns);

                if (DialogResult.OK == frm.ShowDialog())
                {
                    _lvSCTables.BeginUpdate();

                    if (frm.SelectedTables.Count != 0)
                    {
                        string[] datasetTables = frm.SelectedTables.ToArray();
                        string tableName = String.Join(",", datasetTables);
                        ListViewItem x = _lvSCTables.Items.Add(tableName);

                        // This is a new table, so add with the default values                    
                        IList columns;

                        // Get Column names to display
                        columns = new ArrayList();
                        Char delimiter = ',';
                        String[] substrings = tableName.Split(delimiter);

                        //Add items to the grid
                        x.SubItems.Add(UIConstants.SC_AllColumns);
                        x.SubItems.Add(UIConstants.SC_Dataset);
                        List<DatabaseObjectRecord> listDataObject = new List<DatabaseObjectRecord>();
                        foreach (var substring in substrings)
                        {
                            if (_tableObjects.ContainsKey(substring))
                            {
                                listDataObject.Add(_tableObjects[substring]);
                            }
                        }
                        if (listDataObject.Count > 1)
                        {
                            x.Tag = listDataObject;
                        }
                        else if (_tableObjects.ContainsKey(tableName))
                        {
                            x.Tag = _tableObjects[tableName];
                        }
                        else
                            x.Tag = null; // Tags creation finished
                    }

                    _lvSCTables.Focus();

                    if (_lvSCTables.Items.Count > 0)
                    {
                        //_lvSCTables.TopItem.Selected = true;
                        _btnConfigureSC.Enabled = true;//SQlCM-5747 v5.6
                    }
                    _lvSCTables.EndUpdate();
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("Problem adding Sensitive Columns Data-set for auditing: " + ex, "Error");
            }
        }

        private void Click_btnEditSCTable(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection items = _lvSCTables.SelectedItems;
            if (_tableList == null)
                LoadTables();
            if (items.Count == 0)
                return;

            string tableName = items[0].SubItems[0].Text;
            string tableType = items[0].SubItems[2].Text;
            RawTableObject tbl = null;

            bool schemas = SupportsSchemas();
            foreach (RawTableObject rto in _tableList)
            {
                if (schemas)
                {
                    if (rto.FullTableName == tableName)
                    {
                        rto.DisplayName = tableName;
                        tbl = rto;
                        break;
                    }
                    else if (
                                tableName.Contains(",")
                                ||
                                tableType == UIConstants.SC_Dataset
                        )
                    {
                        rto.DisplayName = tableName;
                        tbl = rto;
                        break;
                    }
                }
                else
                {
                    if (rto.TableName == tableName)
                    {
                        rto.DisplayName = tableName;
                        tbl = rto;
                        break;
                    }
                }
            }

            if (tbl == null)
            {
                MessageBox.Show(String.Format(UIConstants.Error_Cant_Edit_BAD_Table, tableName), UIConstants.ADDTableTilte_SC, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string[] selectedColumns = Form_TableConfigure.GetColumns(items[0].SubItems[1].Text);

            IList columns;
            Dictionary<string, IList> dataSet = new Dictionary<string, IList>();
            if (tableType == CoreConstants.DatasetColumnType)
            {
                columns = new ArrayList();
                Char delimiter = ',';
                String[] substrings = tableName.Split(delimiter);
                foreach (string substring in substrings)
                {
                    IList columnList = null;
                    columnList = LoadColumns(substring);
                    foreach (var o in columnList)
                    {
                        columns.Add(substring + "." + o);
                    }

                    //Add the column names to the dictionary which will be used for Configure Table Form
                    dataSet.Add(substring, columnList);
                }
            }
            else
            {
                columns = LoadColumns(tableName);
            }

            using (Form_TableConfigure frm = new Form_TableConfigure(tbl, 1, columns, selectedColumns, UIConstants.Table_Column_Usage.SensitiveColumns, dataSet))
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    string colList = string.Empty;
                    if (frm.AllColumns)
                    {
                        colList = UIConstants.SC_AllColumns;
                    }
                    else
                    {
                        colList = frm.SelectedColumnsString;
                    }
                    items[0].SubItems[1].Text = colList;
                    items[0].ForeColor = System.Drawing.SystemColors.WindowText;
                }
            }
        }

        private void Click_btnRemoveSCTable(object sender, EventArgs e)
        {
            if (_lvSCTables.SelectedItems.Count == 0)
                return;
            _lvSCTables.BeginUpdate();

            int ndx = _lvSCTables.SelectedItems[0].Index;


            foreach (ListViewItem table in _lvSCTables.SelectedItems)
            {
                table.Remove();
            }

            _lvSCTables.EndUpdate();

            // reset selected item
            if (_lvSCTables.Items.Count != 0)
            {
                _lvSCTables.Focus();
                if (ndx >= _lvSCTables.Items.Count)
                {
                    _lvSCTables.Items[_lvSCTables.Items.Count - 1].Selected = true;
                }
                else
                    _lvSCTables.Items[ndx].Selected = true;
            }
            if (_lvSCTables.Items.Count == 0)
                _btnConfigureSC.Enabled = false;//SQlCM-5747 v5.6
        }
        //SQlCM-5747 v5.6
        private void Click_btnConfigureSC(object sender, EventArgs e)
        {
            Form_ConfigureSCActivity configSCFrm = new Form_ConfigureSCActivity(_oldDb.SrvId, _oldDb.DbId);
            configSCFrm.ShowDialog();
        }

        private bool LoadTables()
        {
            // Attempt to load list of tables if we haven't tried already
            if (_tableList == null && _tableObjects == null)
            {
                _tableObjects = new Dictionary<string, DatabaseObjectRecord>();
                // try via connection to agent
                if (_tryAgentCommunication)
                {
                    string url = EndPointUrlBuilder.GetUrl(typeof(AgentManager), Globals.SQLcomplianceConfig.Server, Globals.SQLcomplianceConfig.ServerPort);
                    try
                    {
                        url = String.Format("tcp://{0}:{1}/{2}",
                                             Globals.SQLcomplianceConfig.Server,
                                             Globals.SQLcomplianceConfig.ServerPort,
                                             typeof(AgentManager).Name);
                        AgentManager manager = GUIRemoteObjectsProvider.AgentManager();

                        _tableList = manager.GetRawTables(txtServer, txtName);
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                                String.Format("LoadTables: URL: {0} Instance {1} Database {2}", url, txtServer, txtName),
                                                ex,
                                                ErrorLog.Severity.Warning);
                        _tableList = null;
                    }
                }

                // straight connection to SQL Server
                if (_tableList == null)
                {
                    if (_sqlServer.OpenConnection(txtServer))
                    {
                        _tableList = RawSQL.GetTables(_sqlServer.Connection, txtName);
                    }
                }

                bool supportsSchemas = SupportsSchemas();
                if (_tableList != null)
                {
                    foreach (RawTableObject rto in _tableList)
                    {
                        DatabaseObjectRecord dbo = new DatabaseObjectRecord(rto);
                        dbo.DbId = _oldDb.DbId;
                        if (supportsSchemas)
                        {
                          //  _tableObjects.Add(dbo.FullTableName, dbo);
                            //start sqlcm 5.6 - 5671
                            if (dbo.TableName.StartsWith(dbo.SchemaName))
                                _tableObjects.Add(dbo.SchemaName + "." + dbo.TableName, dbo);
                            //end sqlcm 5.6 - 5671
                            else
                                _tableObjects.Add(dbo.FullTableName, dbo);
                        }
                        else
                            _tableObjects.Add(dbo.TableName, dbo);
                    }
                }
            }
            return (_tableList != null && _tableObjects != null);
        }

        private bool SupportsSchemas()
        {
            if (_server == null || _server.SqlVersion < 9)
                return false;

            return SupportsSensitiveColumns();
        }

        private bool SupportsSensitiveColumns()
        {
            if (_server == null ||
               String.IsNullOrEmpty(_server.AgentVersion) ||
               _server.AgentVersion.StartsWith("1") ||
               _server.AgentVersion.StartsWith("2") ||
               _server.AgentVersion.StartsWith("3.0") ||
               _server.AgentVersion.StartsWith("3.1") ||
               _server.AgentVersion.StartsWith("3.2") ||
               _server.AgentVersion.StartsWith("3.3"))
                return false;
            else
                return true;
        }

        private IList LoadColumns(string tableName)
        {
            IList columnList = null;
            // Load list of columns for the table
            // try via connection to agent
            if (_tryAgentCommunication)
            {
                string url = "";
                try
                {
                    AgentManager manager = GUIRemoteObjectsProvider.AgentManager();

                    columnList = manager.GetRawColumns(txtServer, txtName, tableName);
                }
                catch (Exception ex)
                {
                    ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                            String.Format("LoadColumns: URL: {0} Instance {1} Database {2} Table {3}", url, txtServer, txtName, tableName),
                                            ex,
                                            ErrorLog.Severity.Warning);
                    columnList = null;
                }
            }

            // straight connection to SQL Server
            if (columnList == null)
            {
                if (_sqlServer.OpenConnection(txtServer))
                {
                    columnList = RawSQL.GetColumns(_sqlServer.Connection, txtName, tableName);
                }
            }

            return columnList;
        }

        private void _btnOK_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem tableItem in _lvSCTables.Items)
            {
                string tableType = tableItem.SubItems[2].Text;
                string columnName = tableItem.SubItems[1].Text;
                if (tableType == UIConstants.SC_Dataset && columnName.Equals(UIConstants.SC_AllColumns))
                {
                    var builder = new System.Text.StringBuilder();
                    string columnNames = null;
                    Char delimiter = ',';
                    string[] tableNames = tableItem.SubItems[0].Text.Split(delimiter);
                    foreach (string tableName in tableNames)
                    {
                        IList columns = LoadColumns(tableName);
                        foreach (RawColumnObject columnObject in columns)
                        {
                            builder.Append(tableName + "." + columnObject.ColumnName).Append(",");
                        }
                    }
                    builder.Remove(builder.Length - 1, 1);
                    columnNames = builder.ToString();
                    tableItem.SubItems[1].Text = columnNames;
                }
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void _btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
