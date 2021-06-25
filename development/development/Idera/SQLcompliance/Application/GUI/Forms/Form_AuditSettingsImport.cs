using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;
using Idera.SQLcompliance.Application.GUI.Helper;
using Idera.SQLcompliance.Application.GUI.Properties;
using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Agent;
using Idera.SQLcompliance.Core.Status;
using Idera.SQLcompliance.Core.Templates;
using Idera.SQLcompliance.Core.Templates.AuditSettings;
using SortOrder = System.Windows.Forms.SortOrder;
using Idera.SQLcompliance.Core.Event;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
    public partial class Form_AuditSettingsImport : Form
    {
        private static readonly string[] _wizardTitles = new string[]
           {
            "Select File to Import",
            "Import Audit Settings",
            "Target Servers",
            "Target Databases",
            "Summary"
           };

        private static readonly string[] _wizardDescriptions = new string[]
           {
            "This window allows you to select an audit settings file to import.",
            "This window allows you to select the type of audit settings to import.",
            "This window allows you to select the servers to receive the audit settings.",
            "This window allows you to select the databases to receive the audit settings.",
            "This window allows you to verify your choices."
           };

        private int _pageIndex;
        private int _pageCount;
        private InstanceTemplate _template;
        private ListViewColumnSorter _sorter;

        public Form_AuditSettingsImport()
        {
            InitializeComponent();
            this.Icon = Resources.SQLcompliance_product_ico;
            _template = null;

            _sorter = new ListViewColumnSorter();
            _lstDatabaseList.ListViewItemSorter = _sorter;

            _pageIndex = 0;
            _pageCount = 5;

            _lblTitle.Text = _wizardTitles[_pageIndex];
            _lblDescription.Text = _wizardDescriptions[_pageIndex];
            UpdateData(_pageIndex);
        }

        private void ShowPage(int pageNumber)
        {
            if (pageNumber < 0 || pageNumber >= _pageCount)
            {
                throw new Exception("Invalid page number");
            }
            if (pageNumber == _pageIndex)
                return;

            if (pageNumber == 0)
            {
                _btnBack.Enabled = false;
                _btnNext.Enabled = _tbFile.Text.Length > 0;
                _btnFinish.Enabled = false;
            }
            else if (pageNumber == (_pageCount - 1))
            {
                if (_pageCount > 1)
                    _btnBack.Enabled = true;
                _btnFinish.Enabled = true;
                _btnNext.Enabled = false;
            }
            else
            {
                _btnBack.Enabled = true;
                _btnFinish.Enabled = false;
                if (pageNumber == 1)
                    _btnNext.Enabled = (_chkDatabase.Checked || _chkDatabasePrivUser.Checked || _chkServerPrivUser.Checked || _checkServer.Checked);
                else if (pageNumber == 2)
                    _btnNext.Enabled = _lstTargetServers.SelectedIndices.Count > 0;
                else if (pageNumber == 3)
                    _btnNext.Enabled = _lstDatabaseList.SelectedIndices.Count > 0;
            }

            _lblTitle.Text = _wizardTitles[pageNumber];
            _lblDescription.Text = _wizardDescriptions[pageNumber];

            switch (pageNumber)
            {
                case 0:
                    UpdateData(0);
                    _pnlBrowse.Show();
                    break;
                case 1:
                    UpdateData(1);
                    _pnlSelectSettings.Show();
                    break;
                case 2:
                    UpdateData(2);
                    _pnlTargetServers.Show();
                    break;
                case 3:
                    UpdateData(3);
                    _pnlTargetDatabases.Show();
                    break;
                case 4:
                    UpdateData(4);
                    _pnlSummary.Show();
                    break;
            }

            switch (_pageIndex)
            {
                case 0:
                    _pnlBrowse.Hide();
                    break;
                case 1:
                    _pnlSelectSettings.Hide();
                    break;
                case 2:
                    _pnlTargetServers.Hide();
                    break;
                case 3:
                    _pnlTargetDatabases.Hide();
                    break;
                case 4:
                    _pnlSummary.Hide();
                    break;
            }
            _pageIndex = pageNumber;
            if (_pageIndex == (_pageCount - 1))
            {
                if (Globals.isAdmin)
                    _btnFinish.Focus();
                else
                    _btnCancel.Focus();
            }
            else
                _btnNext.Focus();
        }

        /// <summary>
        /// This function updates context-sensitive UI components in the wizard
        /// based upon the information availabled in the AlertRule object.
        /// </summary>
        /// <param name="page"></param>
        private void UpdateData(int page)
        {
            try
            {
                switch (page)
                {
                    case 1:
                        if (_template.AuditTemplate.ServerLevelConfig == null)
                        {
                            _checkServer.Enabled = false;
                            _checkServer.Checked = false;
                        }
                        else
                        {
                            _checkServer.Enabled = true;
                            _checkServer.Checked = true;
                        }
                        if (_template.AuditTemplate.PrivUserConfig == null)
                        {
                            _chkServerPrivUser.Enabled = false;
                            _chkServerPrivUser.Checked = false;
                        }
                        else
                        {
                            _chkServerPrivUser.Enabled = true;
                            _chkServerPrivUser.Checked = true;
                        }


                        _chkDatabasePrivUser.Enabled = false;
                        _chkDatabasePrivUser.Checked = false;

                        if (_template.AuditTemplate.DbLevelConfigs == null ||
                           _template.AuditTemplate.DbLevelConfigs.Length == 0)
                        {
                            _chkDatabase.Enabled = false;
                            _chkDatabase.Checked = false;
                            _checkMatchDbNames.Enabled = false;
                            _checkMatchDbNames.Checked = false;
                            _lstDatabaseSettings.Items.Clear();
                            _lstDatabaseSettings.Enabled = false;
                        }
                        else
                        {
                            _chkDatabase.Enabled = true;
                            _chkDatabase.Checked = true;
                            _checkMatchDbNames.Enabled = true;
                            _lstDatabaseSettings.Enabled = true;
                            _lstDatabaseSettings.Items.Clear();

                            foreach (DBAuditConfig config in _template.AuditTemplate.DbLevelConfigs)
                            {

                                if (config.PrivUserConfig != null)
                                {
                                    _chkDatabasePrivUser.Enabled = true;
                                    _chkDatabasePrivUser.Checked = true;
                                }

                                _lstDatabaseSettings.Items.Add(config.Database);
                            }
                            _lstDatabaseSettings.SelectedIndex = 0;
                        }
                        break;
                    case 2:
                        if (_lstTargetServers.Items.Count == 0)
                        {
                            // We only have to load servers once.
                            List<ServerRecord> servers = ServerRecord.GetServers(Globals.Repository.Connection, false);
                            _lstTargetServers.Items.Clear();
                            foreach (ServerRecord server in servers)
                            {
                                ListViewItem item = new ListViewItem(server.Instance);
                                item.Tag = server;
                                _lstTargetServers.Items.Add(item);
                            }
                        }
                        break;
                    case 3:
                        List<string> selectedItems = new List<string>();
                        List<string> targetDatabases = new List<string>();
                        foreach (string db in _lstDatabaseSettings.SelectedItems)
                        {
                            targetDatabases.Add(db);
                        }
                        foreach (ListViewItem item in _lstDatabaseList.SelectedItems)
                        {
                            selectedItems.Add(item.Text + item.SubItems[1].Text);
                        }
                        _lstDatabaseList.Items.Clear();
                        foreach (ListViewItem item in _lstTargetServers.SelectedItems)
                        {
                            ServerRecord server = item.Tag as ServerRecord;
                            List<DatabaseRecord> dbs = DatabaseRecord.GetDatabases(Globals.Repository.Connection, server.SrvId);
                            foreach (DatabaseRecord db in dbs)
                            {
                                if (_checkMatchDbNames.Checked && !targetDatabases.Contains(db.Name))
                                    continue;
                                ListViewItem item2 = new ListViewItem(db.Name);
                                item2.Tag = db;
                                item2.SubItems.Add(server.Instance);
                                item2.SubItems[1].Tag = server;
                                _lstDatabaseList.Items.Add(item2);
                                if (selectedItems.Contains(db.Name + db.SrvInstance))
                                    item2.Selected = true;
                            }
                        }
                        break;
                    case 4:
                        break;
                }
            }
            catch (Exception)
            {
            }
        }

        #region Events

        private void Click_btnBack(object sender, EventArgs e)
        {
            int nextPage = _pageIndex - 1;
            switch (_pageIndex)
            {
                case 4:
                    if (_chkDatabase.Checked ||
                       _chkDatabasePrivUser.Checked)
                        nextPage = 3;
                    else
                        nextPage = 2;
                    break;
            }
            ShowPage(nextPage);
        }

        private void Click_btnNext(object sender, EventArgs e)
        {
            int nextPage = _pageIndex + 1;

            switch (_pageIndex)
            {
                case 0:
                    if (_template == null || !_template.FullFilename.Equals(_tbFile.Text))
                    {
                        _template = new InstanceTemplate();
                        if (!_template.Load(_tbFile.Text))
                        {
                            MessageBox.Show(this, "Unable to parse the selected file.", "Error");
                            _template = null;
                            return;
                        }
                        else if (_template.AuditTemplate == null)
                        {
                            MessageBox.Show(this, "The selected file does not contain Audit Settings.");
                            _template = null;
                            return;
                        }
                    }
                    break;
                case 1:
                    if (!_chkDatabasePrivUser.Checked &&
                        !_chkDatabase.Checked &&
                        !_checkMatchDbNames.Checked &&
                        !_chkServerPrivUser.Checked &&
                        !_checkServer.Checked)
                    {
                        MessageBox.Show(this, "Please select the type of audit settings you want to import.", "Error");
                        return;
                    }
                    if ((_chkDatabasePrivUser.Checked || _chkDatabase.Checked) &&
                         _lstDatabaseSettings.SelectedItems.Count == 0)
                    {
                        MessageBox.Show(this, "Please select the database settings to include in the import.", "Error");
                        return;
                    }
                    break;
                case 2:
                    if (_lstTargetServers.SelectedItems.Count == 0)
                    {
                        MessageBox.Show(this, "Please select at least one target server to import audit settings.", "Error");
                        return;
                    }
                    if (_chkDatabasePrivUser.Checked ||
                        _chkDatabase.Checked)
                        nextPage = 3;
                    else
                        nextPage = 4;
                    break;
                case 3:
                    if (_lstDatabaseList.SelectedItems.Count == 0)
                    {
                        MessageBox.Show(this, "Please select at least one target database to import audit settings.", "Error");
                        return;
                    }
                    break;
            }
            ShowPage(nextPage);
        }

        #endregion // Events

        private void Click_btnBrowse(object sender, EventArgs e)
        {
            OpenFileDialog frm = new OpenFileDialog();
            frm.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
            frm.FilterIndex = 1;
            frm.RestoreDirectory = true;

            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                _tbFile.Text = frm.FileName;
            }
        }

        private void Click_clearAllServers(object sender, EventArgs e)
        {
            _lstTargetServers.SelectedItems.Clear();
        }

        private void Click_selectAllServers(object sender, EventArgs e)
        {
            foreach (ListViewItem item in _lstTargetServers.Items)
                item.Selected = true;
            _lstTargetServers.Focus();
        }

        private void ColumnClick_lstDatabaseList(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == _sorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (_sorter.Order == SortOrder.Ascending)
                {
                    _sorter.Order = SortOrder.Descending;
                }
                else
                {
                    _sorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                _sorter.SortColumn = e.Column;
                _sorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            this._lstDatabaseList.Sort();
        }

        private void Click_btnClearDatabases(object sender, EventArgs e)
        {
            _lstDatabaseList.SelectedItems.Clear();
        }

        private void Click_btnSelectAllDatabases(object sender, EventArgs e)
        {
            foreach (ListViewItem item in _lstDatabaseList.Items)
                item.Selected = true;
            _lstDatabaseList.Focus();
        }

        private void CheckChanged_checkMatchDbNames(object sender, EventArgs e)
        {
            if (_checkMatchDbNames.Checked)
                _lstDatabaseSettings.SelectionMode = SelectionMode.MultiExtended;
            else
                _lstDatabaseSettings.SelectionMode = SelectionMode.One;
            _lstDatabaseList.Items.Clear();
        }

        private void Click_btnFinish(object sender, EventArgs e)
        {
            if (_checkServer.Checked || _chkServerPrivUser.Checked)
            {
                foreach (ListViewItem item in _lstTargetServers.SelectedItems)
                {
                    ServerRecord server = (ServerRecord)item.Tag;
                    ServerRecord originalServer = server.Clone();

                    // Apply Server Settings
                    if (_checkServer.Checked)
                    {
                        _template.AuditTemplate.ApplyServerSettings(server, _rbOverwriteCurrent.Checked);
                    }

                    if (_chkServerPrivUser.Checked)
                    {
                        _template.AuditTemplate.ApplyServerUserSettings(server, _rbOverwriteCurrent.Checked);
                    }

                    // Only update the record if they don't match.
                    if (!ServerRecord.Match(originalServer, server))
                    {
                        server.Connection = Globals.Repository.Connection;
                        if (server.Write(originalServer))
                        {
                            string changeLog = Snapshot.ServerChangeLog(originalServer, server);
                            // Register change to server and perform audit log				      
                            ServerUpdate.RegisterChange(server.SrvId, LogType.ModifyServer, server.Instance, changeLog);
                            server.ConfigVersion++;

                            //SQLCM 5.7 Requirement 5.3.4.2
                            IFormatter formatter;
                            MemoryStream streamTrustedUsers = null;
                            MemoryStream streamPrivilegedUsers = null;
                            streamTrustedUsers = new MemoryStream(Convert.FromBase64String(server.AuditTrustedUsersList));
                            streamPrivilegedUsers = new MemoryStream(Convert.FromBase64String(server.AuditUsersList));
                            formatter = new BinaryFormatter();
                            RemoteUserList trustedUserList = new RemoteUserList();
                            RemoteUserList privilegedUserList = new RemoteUserList();
                            if (server.AuditTrustedUsersList != null && server.AuditTrustedUsersList != "")
                            {
                                trustedUserList = (RemoteUserList)formatter.Deserialize(streamTrustedUsers);
                            }
                            if (server.AuditUsersList != null && server.AuditUsersList != "")
                            {
                                privilegedUserList = (RemoteUserList)formatter.Deserialize(streamPrivilegedUsers);
                            }
                            var error = string.Empty;
                            try
                            {
                                if (!server.SaveServerLevelUsersFromWizard(server, trustedUserList, privilegedUserList))
                                {
                                    error = ServerRecord.GetLastError();
                                    throw (new Exception(error));
                                }
                            }
                            catch (Exception ex)
                            {
                                error = ex.Message;
                            }
                        }
                        else
                        {
                            //error
                            ErrorMessage.Show(this.Text, UIConstants.Error_ErrorSavingServer, ServerRecord.GetLastError());
                            return;
                        }
                    }
                }
            }

            if (_chkDatabase.Checked ||
               _chkDatabasePrivUser.Checked)
            {
                foreach (ListViewItem item in _lstDatabaseList.SelectedItems)
                {
                    bool dbSuccess = false;
                    DatabaseRecord database = (DatabaseRecord)item.Tag;
                    ServerRecord server = (ServerRecord)item.SubItems[1].Tag;
                    DatabaseRecord originalDb = database.Clone();
                    Dictionary<string, DBO> originalTables, tables = new Dictionary<string, DBO>();
                    Dictionary<string, DataChangeTableRecord> originalDataChangeTables, dataChangeTables;
                    List<SensitiveColumnTableRecord> originalSensitiveColumnTables, sensitiveColumnTables;

                    var databaseTemplateName = _checkMatchDbNames.Checked ? database.Name : _lstDatabaseSettings.SelectedItem.ToString();
                    bool auditUserDML = _template.AuditTemplate.CheckAuditUserDML(databaseTemplateName.ToString());

                    if (auditUserDML && !database.AuditDML)
                    {
                        database.AuditUserTables = 2;
                        database.AuditDmlAll = _template.AuditTemplate.CheckDMLSelectFilter(databaseTemplateName.ToString());
                    }
                    if (database.AuditDML && database.AuditDmlAll)
                    {
                        database.AuditUserTables = 1;
                    }

                    // We must preserve the audited tables to determine if we need to update them
                    //  after the template is applied
                    string oldTablesSnapshot = Snapshot.GetDatabaseTables(Globals.Repository.Connection, database.DbId, "\t\t");
                    if (database.AuditUserTables == 2)
                    {
                        originalTables = DBO.GetAuditedTables(Globals.Repository.Connection.ConnectionString, database.DbId);
                        foreach (string s in originalTables.Keys)
                            tables.Add(s, originalTables[s]);
                    }
                    else
                        originalTables = new Dictionary<string, DBO>();

                    // preserve the previously audited BA tables if we need to update them
                    string oldDcTablesSnapshot = Snapshot.GetDataChangeTables(Globals.Repository.Connection, database.DbId, "\t");
                    dataChangeTables = new Dictionary<string, DataChangeTableRecord>();
                    originalDataChangeTables = new Dictionary<string, DataChangeTableRecord>();
                    if (database.AuditDataChanges)
                    {
                        List<DataChangeTableRecord> tableList = DataChangeTableRecord.GetAuditedTables(Globals.Repository.Connection, database.SrvId, database.DbId);
                        foreach (DataChangeTableRecord dcTable in tableList)
                        {
                            dataChangeTables.Add(dcTable.TableName, dcTable);
                            originalDataChangeTables.Add(dcTable.TableName, dcTable);
                        }
                    }

                    // preserve the previously audited SC tables if we need to update them
                    string oldSCTablesSnapshot = Snapshot.GetSensitiveColumnTables(Globals.Repository.Connection, database.DbId, "\t");
                    sensitiveColumnTables = new List<SensitiveColumnTableRecord>();
                    originalSensitiveColumnTables = new List<SensitiveColumnTableRecord>();
                    if (database.AuditSensitiveColumns)
                    {
                        List<SensitiveColumnTableRecord> tableList = SensitiveColumnTableRecord.GetAuditedTables(Globals.Repository.Connection, database.SrvId, database.DbId);
                        if (tableList != null)
                        {
                            foreach (SensitiveColumnTableRecord scTable in tableList)
                            {
                                sensitiveColumnTables.Add(scTable);
                                originalSensitiveColumnTables.Add(scTable);
                            }
                        }
                    }

                    // apply the template

                    if (_chkDatabase.Checked)
                    {
                        _template.AuditTemplate.ApplyDatabaseSettings(databaseTemplateName,
                                                                      server,
                                                                      database,
                                                                      tables,
                                                                  dataChangeTables,
                                                                  sensitiveColumnTables,
                                                                  _rbOverwriteCurrent.Checked,
                                                                  Globals.SQLcomplianceConfig.Server,
                                                                  Globals.SQLcomplianceConfig.ServerPort);
                    }

                    if (_chkDatabasePrivUser.Checked)
                    {
                        _template.AuditTemplate.ApplyDatabasePrivilegedUserSettings(databaseTemplateName, database, _rbOverwriteCurrent.Checked);
                    }

                    database.AuditDataChanges = (dataChangeTables.Count > 0);
                    database.AuditSensitiveColumns = (sensitiveColumnTables.Count > 0);

                    // Write updates as needed
                    bool tableUpdateNeeded = false;
                    bool dcTableUpdateNeeded = false;
                    bool scTableUpdateNeeded = false;
                    bool dbChangesMade = false;
                    database.Connection = Globals.Repository.Connection;
                    using (SqlTransaction t = Globals.Repository.Connection.BeginTransaction())
                    {
                        try
                        {
                            if (!DatabaseRecord.Match(originalDb, database))
                            {
                                if (!database.Write(originalDb, t))
                                    throw new Exception(DatabaseRecord.GetLastError());
                                dbChangesMade = true;
                            }

                            if (tables.Count == originalTables.Count)
                            {
                                foreach (string s in tables.Keys)
                                    if (!originalTables.ContainsKey(s))
                                        tableUpdateNeeded = true;
                            }
                            else
                                tableUpdateNeeded = true;
                            List<DBO> newAuditTables = new List<DBO>();
                            // We need to update audited tables
                            //  Lazy way - drop then readd
                            if (tables.Count > 0 && originalTables.Count > 0)
                            {
                                List<DBO> newTables = new List<DBO>(tables.Values);
                                List<DBO> originalTable = new List<DBO>(originalTables.Values);
                                foreach (DBO s in tables.Values)
                                {
                                    if (!originalTables.ContainsKey(s.FullName))
                                    {
                                        newAuditTables.Add(s);
                                    }

                                }
                            }
                            else
                            {
                                newAuditTables = new List<DBO>(tables.Values);
                            }
                            if (tableUpdateNeeded)
                            {

                                DBO.UpdateUserTables(database.Connection,
                                                     newAuditTables, originalTables.Count, database.DbId, t, _rbOverwriteCurrent.Checked);
                            }

                            if (dataChangeTables.Count == originalDataChangeTables.Count)
                            {
                                foreach (string s in dataChangeTables.Keys)
                                    if (!originalDataChangeTables.ContainsKey(s))
                                        dcTableUpdateNeeded = true;
                            }
                            else
                                dcTableUpdateNeeded = true;
                            if (dcTableUpdateNeeded)
                            {
                                List<DataChangeTableRecord> tableList = new List<DataChangeTableRecord>();
                                tableList.AddRange(dataChangeTables.Values);
                                DataChangeTableRecord.UpdateUserTables(Globals.Repository.Connection, tableList, database.SrvId, database.DbId, t);
                            }

                            if (sensitiveColumnTables.Count == originalSensitiveColumnTables.Count)
                            {
                                foreach (SensitiveColumnTableRecord record in sensitiveColumnTables)
                                    if (!originalSensitiveColumnTables.Contains(record))
                                        scTableUpdateNeeded = true;
                            }
                            else
                                scTableUpdateNeeded = true;

                            if (scTableUpdateNeeded)
                            {
                                List<SensitiveColumnTableRecord> sensitiveColumns = new List<SensitiveColumnTableRecord>();
                                sensitiveColumns.AddRange(sensitiveColumnTables);
                                SensitiveColumnTableRecord.UpdateUserTables(Globals.Repository.Connection, sensitiveColumns, database.SrvId, database.DbId, t, true, server.Instance, database.Name);
                            }

                            t.Commit();
                            dbSuccess = true;
                        }
                        catch (Exception ex)
                        {
                            t.Rollback();
                            ErrorMessage.Show(this.Text, UIConstants.Error_ErrorSavingDatabase, ex.Message);
                        }
                        finally
                        {
                            // Only write changelog if changes were made and successfully written
                            if ((dbChangesMade || tableUpdateNeeded) && dbSuccess)
                            {
                                string changeLog = Snapshot.DatabaseChangeLog(Globals.Repository.Connection,
                                                                              originalDb,
                                                                              database,
                                                                              oldTablesSnapshot,
                                                                              oldDcTablesSnapshot,
                                                                              oldSCTablesSnapshot);

                                // Register change to server and perform audit log				      
                                ServerUpdate.RegisterChange(database.SrvId, LogType.ModifyDatabase, database.SrvInstance, changeLog);

                                //SQLCM 5.7 Requirement 5.3.4.2
                                IFormatter formatter;
                                MemoryStream streamTrustedUsers = null;
                                MemoryStream streamPrivilegedUsers = null;
                                streamTrustedUsers = new MemoryStream(Convert.FromBase64String(database.AuditUsersList));
                                streamPrivilegedUsers = new MemoryStream(Convert.FromBase64String(database.AuditPrivUsersList));
                                formatter = new BinaryFormatter();
                                RemoteUserList trustedUserList = new RemoteUserList();
                                RemoteUserList privilegedUserList = new RemoteUserList();
                                if (database.AuditUsersList != null && database.AuditUsersList != "")
                                {
                                    trustedUserList = (RemoteUserList)formatter.Deserialize(streamTrustedUsers);
                                }
                                if (database.AuditPrivUsersList != null && database.AuditPrivUsersList != "")
                                {
                                    privilegedUserList = (RemoteUserList)formatter.Deserialize(streamPrivilegedUsers);
                                }
                                var error = string.Empty;
                                try
                                {
                                    if (!database.SaveDatabaseLevelUsersFromServerSettings(database, trustedUserList, privilegedUserList))
                                    {
                                        error = DatabaseRecord.GetLastError();
                                        throw (new Exception(error));
                                    }
                                }
                                catch (Exception ex)
                                {
                                    error = ex.Message;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void SelectedIndexChanged_lstDatabaseList(object sender, EventArgs e)
        {
            if (_lstDatabaseList.SelectedIndices.Count == 0)
                _btnNext.Enabled = false;
            else
                _btnNext.Enabled = true;
        }

        private void CheckChanged_all(object sender, EventArgs e)
        {
            bool dbChecksChanged = _chkDatabasePrivUser.Checked || _chkDatabase.Checked;

            _btnNext.Enabled = dbChecksChanged || _chkServerPrivUser.Checked || _checkServer.Checked;

            _checkMatchDbNames.Enabled = dbChecksChanged;
            _lstDatabaseSettings.Enabled = dbChecksChanged;

            if (!dbChecksChanged)
            {
                _checkMatchDbNames.Checked = false;
            }
        }

        private void SelectedIndexChanged_lstTargetServers(object sender, EventArgs e)
        {
            if (_lstTargetServers.SelectedIndices.Count == 0)
                _btnNext.Enabled = false;
            else
                _btnNext.Enabled = true;
        }

        private void TextChanged_tbFile(object sender, EventArgs e)
        {
            if (_tbFile.Text.Length == 0)
                _btnNext.Enabled = false;
            else
                _btnNext.Enabled = true;
        }

        private void Form_AuditSettingsImport_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string helpTopic = "";

            switch (_pageIndex)
            {
                case 0:
                    helpTopic = HelpAlias.SSHELP_ImportAuditSettings_SelectFile;
                    break;
                case 1:
                    helpTopic = HelpAlias.SSHELP_ImportAuditSettings_SelectSettings;
                    break;
                case 2:
                    helpTopic = HelpAlias.SSHELP_ImportAuditSettings_TargetServers;
                    break;
                case 3:
                    helpTopic = HelpAlias.SSHELP_ImportAuditSettings_TargetDatabases;
                    break;
                case 4:
                    helpTopic = HelpAlias.SSHELP_ImportAuditSettings_Summary;
                    break;
            }

            if (helpTopic != "") HelpAlias.ShowHelp(this, helpTopic);

            hlpevent.Handled = true;
        }
    }

    /// <summary>
    /// This class is an implementation of the 'IComparer' interface.
    /// </summary>
    public class ListViewColumnSorter : IComparer
    {
        /// <summary>
        /// Specifies the column to be sorted
        /// </summary>
        private int ColumnToSort;

        /// <summary>
        /// Specifies the order in which to sort (i.e. 'Ascending').
        /// </summary>
        private SortOrder OrderOfSort;

        /// <summary>
        /// Case insensitive comparer object
        /// </summary>
        private CaseInsensitiveComparer ObjectCompare;

        /// <summary>
        /// Class constructor.  Initializes various elements
        /// </summary>
        public ListViewColumnSorter()
        {
            // Initialize the column to '0'
            ColumnToSort = 0;

            // Initialize the sort order to 'none'
            OrderOfSort = SortOrder.None;

            // Initialize the CaseInsensitiveComparer object
            ObjectCompare = new CaseInsensitiveComparer();
        }

        /// <summary>
        /// This method is inherited from the IComparer interface.  It compares the two objects passed using a case insensitive comparison.
        /// </summary>
        /// <param name="x">First object to be compared</param>
        /// <param name="y">Second object to be compared</param>
        /// <returns>The result of the comparison. "0" if equal, negative if 'x' is less than 'y' and positive if 'x' is greater than 'y'</returns>
        public int Compare(object x, object y)
        {
            int compareResult;
            ListViewItem listviewX, listviewY;

            // Cast the objects to be compared to ListViewItem objects
            listviewX = (ListViewItem)x;
            listviewY = (ListViewItem)y;

            // Compare the two items
            compareResult =
               ObjectCompare.Compare(listviewX.SubItems[ColumnToSort].Text, listviewY.SubItems[ColumnToSort].Text);

            // Calculate correct return value based on object comparison
            if (OrderOfSort == SortOrder.Ascending)
            {
                // Ascending sort is selected, return normal result of compare operation
                return compareResult;
            }
            else if (OrderOfSort == SortOrder.Descending)
            {
                // Descending sort is selected, return negative result of compare operation
                return (-compareResult);
            }
            else
            {
                // Return '0' to indicate they are equal
                return 0;
            }
        }

        /// <summary>
        /// Gets or sets the number of the column to which to apply the sorting operation (Defaults to '0').
        /// </summary>
        public int SortColumn
        {
            set { ColumnToSort = value; }
            get { return ColumnToSort; }
        }

        /// <summary>
        /// Gets or sets the order of sorting to apply (for example, 'Ascending' or 'Descending').
        /// </summary>
        public SortOrder Order
        {
            set { OrderOfSort = value; }
            get { return OrderOfSort; }
        }
    }
}