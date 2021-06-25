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
    public partial class Form_BeforeAfterRegulationGuidelines : Form
    {

        private ServerRecord _server;
        private DatabaseRecord _oldDb;
        private SQLDirect _sqlServer = null;
        private ICollection _tableList = null;
        private bool _tryAgentCommunication = false;
        private Dictionary<string, DatabaseObjectRecord> _tableObjects = null;
        private Dictionary<string, DataChangeTableRecord> _oldBATables;
        private bool _beforeAfterAvailable;
        private string txtServer;
        private string txtName;
        private int _compatibilityLevel = -1;
        private bool auditDMLCheck = false;

        public ListView LvBeforeAfterTables
        {
            get { return _lvBeforeAfterTables; }
        }

        public Form_BeforeAfterRegulationGuidelines(ServerRecord server, DatabaseRecord inDb, bool chkAuditDML, Dictionary<string, ListView> _badListViewItems)
        {
            InitializeComponent();
            _sqlServer = new SQLDirect();
            _server = server;
            _oldDb = inDb;
            txtServer = server.Instance;
            txtName = inDb.Name;
            auditDMLCheck = chkAuditDML;
            try
            {
                ServerRecord sr = new ServerRecord();
                sr.Connection = Globals.Repository.Connection;
                sr.Read(txtServer);
                _tryAgentCommunication = sr.IsDeployed && sr.IsRunning;
            }
            catch { }
            _compatibilityLevel = GetCompatibilityLevel();
            LoadBeforeAfterConfig(_badListViewItems);
        }

        private void _btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void LoadBeforeAfterConfig(Dictionary<string, ListView> _badListViewItems)
        {
            _beforeAfterAvailable = true;
            // SQL Server 2005,2008
            if (_server.SqlVersion == 0)
            {
                _lblBeforeAfterStatus.Text = CoreConstants.Feature_BeforeAfterNotAvailableVersionUnknown;
                _pnlBeforeAfter.Visible = false;
                _beforeAfterAvailable = false;
                return;
            }
            else if (_server.SqlVersion < 9)
            {
                _lblBeforeAfterStatus.Text = CoreConstants.Feature_BeforeAfterNotAvailable;
                _pnlBeforeAfter.Visible = false;
                _beforeAfterAvailable = false;
                return;
            }
            else if (_compatibilityLevel < 90)
            {
                _lblBeforeAfterStatus.Text = CoreConstants.Feature_BeforeAfterNotAvailableCompatibility;
                _pnlBeforeAfter.Visible = false;
                _beforeAfterAvailable = false;
                return;
            }
            else if (!SupportsBeforeAfter())
            {
                _lblBeforeAfterStatus.Text = CoreConstants.Feature_BeforeAfterNotAvailableAgent;
                _pnlBeforeAfter.Visible = false;
                _beforeAfterAvailable = false;
                return;
            }
            if (!LoadTables())
            {
                _lblBeforeAfterStatus.Text = UIConstants.Error_CantLoadTables;
                _pnlBeforeAfter.Visible = false;
                _beforeAfterAvailable = false;
                return;
            }
            else
            {
                UpdateClrStatus();
            }
            _pnlBeforeAfter.Enabled = auditDMLCheck;

            //Load the BAD info
            _oldBATables = new Dictionary<string, DataChangeTableRecord>();
            if (_oldDb.AuditDataChanges || (auditDMLCheck && _badListViewItems != null && _badListViewItems.Count > 0))
            {
                List<string> missingTables = new List<string>();
                List<DataChangeTableRecord> tables = DataChangeTableRecord.GetAuditedTables(Globals.Repository.Connection, _server.SrvId, _oldDb.DbId);
                if (_badListViewItems != null && _badListViewItems.Count > 0)
                {
                    ListView _lvOldBADTables = new ListView();
                    if (_badListViewItems.TryGetValue(_oldDb.Name, out _lvOldBADTables))
                    {
                        if (_lvOldBADTables.Items.Count != 0)
                        {
                            tables.Clear();
                            foreach (ListViewItem tableItem in _lvOldBADTables.Items)
                            {
                                DatabaseObjectRecord dor = (DatabaseObjectRecord)tableItem.Tag;
                                DataChangeTableRecord dctItem = new DataChangeTableRecord();
                                dctItem.SchemaName = dor.SchemaName;
                                dctItem.TableName = dor.TableName;
                                dctItem.ObjectId = dor.Id;

                                if (!SupportsBeforeAfterColumns())
                                {
                                    dctItem.RowLimit = Form_MaxRows.GetMaxRows(tableItem.SubItems[1].Text);
                                    dctItem.SelectedColumns = false;
                                }
                                else if (tableItem.SubItems[2].Text == UIConstants.BAD_AllColumns)
                                {
                                    dctItem.RowLimit = Form_TableConfigure.GetMaxRows(tableItem.SubItems[1].Text);
                                    dctItem.SelectedColumns = false;
                                }
                                else
                                {
                                    dctItem.RowLimit = Form_TableConfigure.GetMaxRows(tableItem.SubItems[1].Text);
                                    dctItem.SelectedColumns = true;
                                    foreach (string col in Form_TableConfigure.GetColumns(tableItem.SubItems[2].Text))
                                    {
                                        dctItem.AddColumn(col);
                                    }
                                }
                                tables.Add(dctItem);
                            }
                        }
                    }
                }
                foreach (DataChangeTableRecord table in tables)
                {
                    ListViewItem x = _lvBeforeAfterTables.Items.Add(table.FullTableName);
                    x.SubItems.Add(Form_MaxRows.GetMaxRowString(table.RowLimit));
                    if (SupportsBeforeAfterColumns())
                    {
                        x.SubItems.Add(table.SelectedColumns ? Form_TableConfigure.GetColumnString(table.Columns) : UIConstants.BAD_AllColumns);
                    }
                    _oldBATables.Add(table.FullTableName, table);
                    if (!_tableObjects.ContainsKey(table.FullTableName))
                    {
                        missingTables.Add(table.FullTableName);
                        x.Tag = null;
                        x.ForeColor = System.Drawing.Color.LightGray;
                    }
                    else
                        x.Tag = _tableObjects[table.FullTableName];
                }
                if (missingTables.Count == 1)
                {
                    MessageBox.Show(String.Format(UIConstants.Warning_BAD_Table_Missing, missingTables[0]), UIConstants.BAD_TABLE_Missing_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (missingTables.Count > 1)
                {
                    MessageBox.Show(String.Format(UIConstants.Warning_BAD_Tables_Missing, String.Join(", ", missingTables.ToArray())), UIConstants.BAD_TABLE_Missing_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            // Update button state
            _lvBeforeAfterTables_SelectedIndexChanged(null, null);
            UpdateBeforeAfterAvailability();
        }
        private void _lvBeforeAfterTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_lvBeforeAfterTables.SelectedItems.Count == 0)
            {
                _btnRemoveBATable.Enabled = false;
                _btnEditBATable.Enabled = false;
            }
            else
            {
                _btnRemoveBATable.Enabled = true;
                _btnEditBATable.Enabled = _lvBeforeAfterTables.SelectedItems.Count == 1 && _lvBeforeAfterTables.SelectedItems[0].Tag != null ? true : false;
            }
        }

        private void UpdateBeforeAfterAvailability()
        {
            // If it is not enabled, the feature is unavailable for other reasones (LoadBeforeAfterConfig)
            if (!_beforeAfterAvailable)
            {
                return;
            }
            _pnlBeforeAfter.Visible = true;
            // Remove the column names column if they are not supported
            if (SupportsBeforeAfterColumns())
            {
                _lvBeforeAfterTables.Columns[2].Width = 169;
            }
            else
            {
                _lvBeforeAfterTables.Columns[0].Width = 268;

                //Remove the columns column if this is a 3.1 agent.
                if (_lvBeforeAfterTables.Columns.ContainsKey("_columnColumnNames"))
                    _lvBeforeAfterTables.Columns.RemoveByKey("_columnColumnNames");
            }
        }
        private void _btnAddBATable_Click(object sender, EventArgs e)
        {
            if (!LoadTables())
            {
                ErrorMessage.Show(this.Text,
                                  UIConstants.Error_CantLoadTables,
                                  SQLDirect.GetLastError());
                _btnAddBATable.Enabled = false;
                return;
            }
            List<string> selectedTables = new List<string>();
            List<RawTableObject> tableList = new List<RawTableObject>();
            if (SupportsBeforeAfterColumns())
            {
                foreach (ListViewItem tableItem in _lvBeforeAfterTables.Items)
                    selectedTables.Add(tableItem.Text);
                foreach (RawTableObject rto in _tableList)
                {
                    tableList.Add(rto);
                }
            }
            else
            {
                List<string> blobTables;

                try
                {
                    blobTables = GetBlobTables();
                }
                catch (Exception ex)
                {
                    ErrorMessage.Show(this.Text,
                                      UIConstants.Error_CantLoadTables,
                                      ex.Message);
                    _btnAddBATable.Enabled = false;
                    return;
                }

                foreach (ListViewItem tableItem in _lvBeforeAfterTables.Items)
                    selectedTables.Add(tableItem.Text);
                foreach (RawTableObject rto in _tableList)
                {
                    if (!blobTables.Contains(rto.FullTableName))
                        tableList.Add(rto);
                }
            }

            Form_TableAdd frm = new Form_TableAdd(tableList, selectedTables, SupportsSchemas(), SupportsBeforeAfterColumns() ? UIConstants.Table_Column_Usage.BADColumns : UIConstants.Table_Column_Usage.BADTables);
            if (DialogResult.OK == frm.ShowDialog())
            {
                Dictionary<string, string> currentTables = new Dictionary<string, string>();
                Dictionary<string, string> currentColumns = new Dictionary<string, string>();
                _lvBeforeAfterTables.BeginUpdate();

                foreach (ListViewItem item in _lvBeforeAfterTables.Items)
                {
                    currentTables[item.Text] = item.SubItems[1].Text;
                    if (SupportsBeforeAfterColumns())
                    {
                        currentColumns[item.Text] = item.SubItems[2].Text;
                    }
                }

                _lvBeforeAfterTables.Items.Clear();

                foreach (string tableName in frm.SelectedTables)
                {
                    ListViewItem x = _lvBeforeAfterTables.Items.Add(tableName);

                    if (currentTables.ContainsKey(tableName))
                    {
                        x.SubItems.Add(currentTables[tableName]);
                        if (SupportsBeforeAfterColumns())
                        {
                            x.SubItems.Add(currentColumns[tableName]);
                            if (currentColumns[tableName] == UIConstants.BAD_NoColumns)
                            {
                                x.ForeColor = System.Drawing.Color.Red;
                            }
                        }
                    }
                    else
                    {
                        // This is a new table, so add with the default values
                        x.SubItems.Add("10");
                        if (SupportsBeforeAfterColumns())
                        {
                            RawTableObject tbl = null;
                            foreach (RawTableObject rto in tableList)
                            {
                                if (rto.FullTableName == tableName)
                                {
                                    tbl = rto;
                                    break;
                                }
                            }

                            string cols = string.Empty;
                            if (tbl == null || !tbl.HasBlobData)
                                x.SubItems.Add(UIConstants.BAD_AllColumns);
                            else
                            {
                                x.SubItems.Add(UIConstants.BAD_NoColumns);
                                x.ForeColor = System.Drawing.Color.Red;
                            }
                        }
                    }

                    if (_tableObjects.ContainsKey(tableName))
                    {
                        x.Tag = _tableObjects[tableName];
                    }
                    else
                        x.Tag = null;
                }

                _lvBeforeAfterTables.Focus();
                if (_lvBeforeAfterTables.Items.Count > 0)
                {
                    _lvBeforeAfterTables.TopItem.Selected = true;
                }

                _lvBeforeAfterTables.EndUpdate();
            }
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
                            // _tableObjects.Add(dbo.FullTableName, dbo);
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

        // Returns true if the agent is able to support BeforeAfter data collection (3.1 and beyond)
        private bool SupportsBeforeAfter()
        {
            if (_server == null ||
               String.IsNullOrEmpty(_server.AgentVersion) ||
               _server.AgentVersion.StartsWith("1") ||
               _server.AgentVersion.StartsWith("2") ||
               _server.AgentVersion.StartsWith("3.0"))
                return false;
            else
                return true;
        }

        // Returns true if the agent is able to support BeforeAfter data collection by column (3.2 and beyond)
        private bool SupportsBeforeAfterColumns()
        {
            if (_server == null ||
               String.IsNullOrEmpty(_server.AgentVersion) ||
               _server.AgentVersion.StartsWith("1") ||
               _server.AgentVersion.StartsWith("2") ||
               _server.AgentVersion.StartsWith("3.0") ||
               _server.AgentVersion.StartsWith("3.1"))
                return false;
            else
                return true;
        }
        private bool SupportsSchemas()
        {
            if (_server == null || _server.SqlVersion < 9)
                return false;

            return SupportsBeforeAfter();
        }

        private List<string> GetBlobTables()
        {
            List<string> retVal;
            // Try agent first if allowed
            if (_tryAgentCommunication)
            {
                string url = EndPointUrlBuilder.GetUrl(typeof(AgentManager), Globals.SQLcomplianceConfig.Server, Globals.SQLcomplianceConfig.ServerPort);
                try
                {
                    AgentManager manager = GUIRemoteObjectsProvider.AgentManager();

                    return manager.GetBlobTables(_server.Instance, _oldDb.Name);
                }
                catch (Exception ex)
                {
                    ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                            String.Format("GetBlobTables: URL: {0} Instance {1} Database {2}", url, txtServer, txtName),
                                            ex,
                                            ErrorLog.Severity.Warning);
                }
            }

            // Now we try direct connect
            if (!_sqlServer.OpenConnection(_server.Instance))
                throw new Exception("Unable to open a connection to server.");

            retVal = RawSQL.GetBlobTables(_sqlServer.Connection, _oldDb.Name);
            _sqlServer.CloseConnection();
            return retVal;
        }

        private int GetCompatibilityLevel()
        {
            int retVal;
            // Try agent first if allowed
            if (_tryAgentCommunication)
            {
                string url = EndPointUrlBuilder.GetUrl(typeof(AgentManager), Globals.SQLcomplianceConfig.Server, Globals.SQLcomplianceConfig.ServerPort);
                try
                {
                    AgentManager manager = GUIRemoteObjectsProvider.AgentManager();
                    return manager.GetCompatibilityLevel(_server.Instance, _oldDb.Name);
                }
                catch (Exception ex)
                {
                    ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                            String.Format("GetCompatibilityLevel: URL: {0} Instance {1} Database {2}", url, txtServer, txtName),
                                            ex,
                                            ErrorLog.Severity.Warning);
                }
            }
            try
            {
                // Now we try direct connect
                if (!_sqlServer.OpenConnection(_server.Instance))
                    throw new Exception("Unable to open a connection to server.");

                retVal = RawSQL.GetCompatibilityLevel(_sqlServer.Connection, _oldDb.Name);
                _sqlServer.CloseConnection();
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Default,
                                           String.Format("GetCompatibilityLevel Direct: Instance {0} Database {1}", txtServer, txtName),
                                           ex,
                                           ErrorLog.Severity.Error);
                retVal = -1;
            }
            return retVal;
        }

        private void UpdateClrStatus()
        {
            bool configured, running;

            try
            {
                GetCLRStatus(out configured, out running);
            }
            catch (Exception e)
            {
                //server not reachable
                ErrorLog.Instance.Write(String.Format("Unable to contact {0} to determine CLR Enabled status.", txtServer),
                   e, ErrorLog.Severity.Warning);
                _pbClrStatus.Image = Resources.StatusError_48;
                _lblClrStatus.Text = String.Format("{0} cannot be reached currently.", _server.Instance);
                _btnEnableCLR.Enabled = false;
                return;
            }

            if (configured != running)
            {
                // RECONFIGURE failed it appears
                _pbClrStatus.Image = Resources.StatusWarning_48;
                if (configured)
                {
                    _lblClrStatus.Text = "CLR cannot be enabled.  Verify that lightweight pooling is disabled.";
                    EnableClr(false);
                }
                else
                {
                    _lblClrStatus.Text = "CLR is running but not conifgured.";
                }
                _btnEnableCLR.Enabled = !configured;
                _btnAddBATable.Enabled = running;
                _btnRemoveBATable.Enabled = running;
            }
            else if (running)
            {
                _pbClrStatus.Image = Resources.StatusGood_48;
                _lblClrStatus.Text = String.Format("CLR is enabled for {0}.", _server.Instance);
                _btnEnableCLR.Enabled = false;
                _btnAddBATable.Enabled = true;
                _btnRemoveBATable.Enabled = _lvBeforeAfterTables.SelectedItems.Count > 0;
            }
            else
            {
                _pbClrStatus.Image = Resources.StatusWarning_48;
                _lblClrStatus.Text = String.Format("CLR is not enabled for {0}.", _server.Instance);
                _btnEnableCLR.Enabled = true;
                _btnAddBATable.Enabled = false;
                _btnRemoveBATable.Enabled = false;
            }
        }
        private void GetCLRStatus(out bool configured, out bool running)
        {
            // Try agent first if allowed
            if (_tryAgentCommunication)
            {
                string url = EndPointUrlBuilder.GetUrl(typeof(AgentManager), Globals.SQLcomplianceConfig.Server, Globals.SQLcomplianceConfig.ServerPort);
                try
                {
                    AgentManager manager = GUIRemoteObjectsProvider.AgentManager();

                    manager.GetCLRStatus(_server.Instance, out configured, out running);
                    return;
                }
                catch (Exception ex)
                {
                    ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                            String.Format("IsCLREnabled: URL: {0} Instance {1} Database {2}", url, txtServer, txtName),
                                            ex,
                                            ErrorLog.Severity.Warning);
                }
            }

            // Now we try direct connect
            if (!_sqlServer.OpenConnection(_server.Instance))
                throw new Exception("Unable to open a connection to server.");

            RawSQL.GetCLRStatus(_sqlServer.Connection, out configured, out running);
            _sqlServer.CloseConnection();
        }

        private void EnableClr(bool enable)
        {
            // Try agent first if allowed
            if (_tryAgentCommunication)
            {
                string url = EndPointUrlBuilder.GetUrl(typeof(AgentManager), Globals.SQLcomplianceConfig.Server, Globals.SQLcomplianceConfig.ServerPort);
                try
                {
                    AgentManager manager = GUIRemoteObjectsProvider.AgentManager(); //TODO log url

                    manager.EnableCLR(_server.Instance, enable);
                    return;
                }
                catch (Exception ex)
                {
                    ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                            String.Format("EnableClr: URL: {0} Instance {1} Database {2}", url, txtServer, txtName),
                                            ex,
                                            ErrorLog.Severity.Warning);
                }
            }

            // Now we try direct connect
            if (!_sqlServer.OpenConnection(_server.Instance))
                throw new Exception("Unable to open a connection to server.");

            try
            {
                RawSQL.EnableCLR(_sqlServer.Connection, enable);
            }
            finally
            {
                _sqlServer.CloseConnection();
            }
        }

        private void _btnRemoveBATable_Click(object sender, EventArgs e)
        {
            if (_lvBeforeAfterTables.SelectedItems.Count == 0)
                return;
            _lvBeforeAfterTables.BeginUpdate();

            int ndx = _lvBeforeAfterTables.SelectedItems[0].Index;


            foreach (ListViewItem table in _lvBeforeAfterTables.SelectedItems)
            {
                table.Remove();
            }

            _lvBeforeAfterTables.EndUpdate();

            // reset selected item
            if (_lvBeforeAfterTables.Items.Count != 0)
            {
                _lvBeforeAfterTables.Focus();
                if (ndx >= _lvBeforeAfterTables.Items.Count)
                {
                    _lvBeforeAfterTables.Items[_lvBeforeAfterTables.Items.Count - 1].Selected = true;
                }
                else
                    _lvBeforeAfterTables.Items[ndx].Selected = true;
            }
        }

        private void _btnEditBATable_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection items = _lvBeforeAfterTables.SelectedItems;

            if (items.Count == 0)
                return;

            if (SupportsBeforeAfterColumns())
            {
                string tableName = items[0].SubItems[0].Text;
                int rows = Form_TableConfigure.GetMaxRows(items[0].SubItems[1].Text);

                RawTableObject tbl = null;
                foreach (RawTableObject rto in _tableList)
                {
                    if (rto.FullTableName == tableName)
                    {
                        tbl = rto;
                        break;
                    }
                }

                if (tbl == null)
                {
                    MessageBox.Show(String.Format(UIConstants.Error_Cant_Edit_BAD_Table, tableName), UIConstants.BAD_TABLE_Missing_Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string[] selectedColumns = Form_TableConfigure.GetColumns(items[0].SubItems[2].Text);

                IList columns = LoadColumns(tableName);
                using (Form_TableConfigure frm = new Form_TableConfigure(tbl, rows, columns, selectedColumns, UIConstants.Table_Column_Usage.BADColumns))
                {
                    if (frm.ShowDialog(this) == DialogResult.OK)
                    {
                        items[0].SubItems[1].Text = Form_TableConfigure.GetMaxRowString(frm.MaximumRows);
                        string colList = string.Empty;
                        if (frm.AllColumns)
                        {
                            colList = UIConstants.BAD_AllColumns;
                        }
                        else
                        {
                            colList = frm.SelectedColumnsString;
                        }
                        items[0].SubItems[2].Text = colList;
                        if (colList == UIConstants.BAD_NoColumns)
                            items[0].ForeColor = System.Drawing.Color.Red;
                        else
                            items[0].ForeColor = System.Drawing.SystemColors.WindowText;
                    }
                }
            }
            else
            {
                int rows = Form_MaxRows.GetMaxRows(items[0].SubItems[1].Text);
                using (Form_MaxRows frm = new Form_MaxRows(rows))
                {
                    if (frm.ShowDialog(this) == DialogResult.OK)
                    {
                        foreach (ListViewItem i in items)
                        {
                            i.SubItems[1].Text = Form_MaxRows.GetMaxRowString(frm.MaximumRows);
                        }
                    }
                }
            }
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
            // BAD tables
            if (_lvBeforeAfterTables.Items.Count > 0)
            {
                if (auditDMLCheck)
                {
                    bool notConfigured = false;
                    foreach (ListViewItem lvi in _lvBeforeAfterTables.Items)
                    {
                        if (SupportsBeforeAfterColumns())
                        {
                            if (lvi.SubItems[2].Text == UIConstants.BAD_NoColumns)
                            {
                                notConfigured = true;
                                _lvBeforeAfterTables.SelectedItems.Clear();
                                lvi.Selected = true;
                                break;
                            }
                        }
                    }
                    if (notConfigured)
                    {
                        ErrorMessage.Show(this.Text,
                                          UIConstants.Error_BlobTablesNotConfigured);

                        _lvBeforeAfterTables.Focus();
                        return;
                    }
                }
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void _btnEnableCLR_Click(object sender, EventArgs e)
        {
            EnableClr(true);
            UpdateClrStatus();
        }
    }
}
