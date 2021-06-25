using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Reflection;
using System.Windows.Forms;
using Idera.SQLcompliance.Application.GUI.Forms;
using Idera.SQLcompliance.Application.GUI.Helper;
using Idera.SQLcompliance.Application.GUI.SQL;
using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Agent;
using Idera.SQLcompliance.Core.Collector;
using Idera.SQLcompliance.Core.Service;
using Infragistics.Win;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;

namespace Idera.SQLcompliance.Application.GUI.Controls
{
    /// <summary>
    /// Summary description for DatabaseView.
    /// </summary>
    public partial class ServerView : BaseControl
    {
        #region Members

        private bool isLoaded = false;

        #endregion

        #region Constructor / Dispose

        public ServerView()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            GridHelper.ApplyAdminSettings(_gridServers);
            GridHelper.ApplyAdminSettings(_gridDatabases);
            _gridServers.DisplayLayout.Override.SelectTypeRow = SelectType.Extended;
            _gridDatabases.DisplayLayout.Override.SelectTypeRow = SelectType.Extended;

            // Enable main form menus 			
            SetMenuFlag(CMMenuItem.Refresh);
            SetMenuFlag(CMMenuItem.ShowHelp);
            SetMenuFlag(CMMenuItem.ImportAuditSettings, Globals.isAdmin);
        }

        public override void Initialize(Form_Main2 frm)
        {
            base.Initialize(frm);

            frm.ServerAdded += new EventHandler<ServerEventArgs>(ServerAdded_mainForm);
            frm.ServerRemoved += new EventHandler<ServerEventArgs>(ServerRemoved_mainForm);
            frm.ServerModified += new EventHandler<ServerEventArgs>(ServerModified_mainForm);

            frm.DatabaseAdded += new EventHandler<DatabaseEventArgs>(DatabasesAltered_mainForm);
            frm.DatabaseRemoved += new EventHandler<DatabaseEventArgs>(DatabasesAltered_mainForm);
            frm.DatabaseModified += new EventHandler<DatabaseEventArgs>(DatabasesAltered_mainForm);
        }


        void DatabasesAltered_mainForm(object sender, DatabaseEventArgs e)
        {
            ServerRecord srv = GetSelectedServer();
            if (srv != null && srv.Instance == e.Database.SrvInstance)
                LoadDatabases();
        }

        void ServerModified_mainForm(object sender, ServerEventArgs e)
        {
            UpdateServerListItem(e.Server);
        }

        void ServerRemoved_mainForm(object sender, ServerEventArgs e)
        {
            RefreshView();
        }

        void ServerAdded_mainForm(object sender, ServerEventArgs e)
        {
            // We don't list archive servers in this view
            if (!e.Server.IsAuditedServer)
                return;
            UltraDataRow row = _dsServers.Rows.Add();
            UpdateServerRowValues(row, e.Server);
        }

        #endregion

        private List<ServerRecord> GetSelectedServers()
        {
            if (_gridServers.Selected.Rows.Count == 0)
                return null;
            List<ServerRecord> retVal = new List<ServerRecord>();
            foreach (UltraGridRow gridRow in _gridServers.Selected.Rows)
            {
                UltraDataRow row = (UltraDataRow)gridRow.ListObject;
                retVal.Add((ServerRecord)row.Tag);
            }
            return retVal;
        }

        private List<DatabaseRecord> GetSelectedDatabases()
        {
            if (_gridDatabases.Selected.Rows.Count == 0)
                return null;
            List<DatabaseRecord> retVal = new List<DatabaseRecord>();
            foreach (UltraGridRow gridRow in _gridDatabases.Selected.Rows)
            {
                UltraDataRow row = (UltraDataRow)gridRow.ListObject;
                retVal.Add((DatabaseRecord)row.Tag);
            }
            return retVal;
        }

        public ServerRecord GetSelectedServer()
        {
            if (_gridServers.Selected.Rows.Count == 0)
                return null;
            UltraDataRow row = (UltraDataRow)_gridServers.Selected.Rows[0].ListObject;
            return (ServerRecord)row.Tag;
        }

        private DatabaseRecord GetSelectedDatabase()
        {
            if (_gridDatabases.Selected.Rows.Count == 0)
                return null;
            UltraDataRow row = (UltraDataRow)_gridDatabases.Selected.Rows[0].ListObject;
            return (DatabaseRecord)row.Tag;
        }

        private void UpdateServerRowValues(UltraDataRow row, ServerRecord record)
        {
            ServerStatus tmpStatus;
            string opStatus;
            string auditStatusString;
            string deploymentMethod = "";
            if (record.IsDeployedManually && record.DeployedByCommand == 2)
            {
                deploymentMethod = "Silent Installer Script";
            }
            else if (record.IsDeployedManually && record.DeployedByCommand != 2)
            {
                deploymentMethod = "Manually deployed";
            }
            else
            {
                deploymentMethod = "Console deployed";
            }

            tmpStatus = SQLRepository.GetStatus(record, out opStatus, out auditStatusString);
            row["Icon"] = GetImage(tmpStatus);
            row["Server"] = record.Instance;
            row["Status"] = opStatus;
            row["DeploymentMethod"] = deploymentMethod;
            row["AuditStatus"] = auditStatusString;
            row["LastContact"] = SQLRepository.GetLastAgentContactTime(record);
            row.Tag = record;
        }

        private void UpdateServerListItem(ServerRecord record)
        {
            UltraDataRow row = FindServer(record);
            if (row != null)
                UpdateServerRowValues(row, record);
        }

        private static Image GetImage(ServerStatus s)
        {
            switch (s)
            {
                case ServerStatus.OK:
                    return AppIcons.AppImg16(AppIcons.Img16.OkServer);
                case ServerStatus.Warning:
                    return AppIcons.AppImg16(AppIcons.Img16.WarningServer);
                case ServerStatus.Alert:
                    return AppIcons.AppImg16(AppIcons.Img16.ErrorServer);
                case ServerStatus.Archive:
                    return AppIcons.AppImg16(AppIcons.Img16.ReportServer);
                case ServerStatus.Disabled:
                    return AppIcons.AppImg16(AppIcons.Img16.DisabledServer);
            }
            return null;
        }

        private UltraDataRow FindServer(ServerRecord record)
        {
            foreach (UltraDataRow row in _dsServers.Rows)
            {
                if (String.Compare(row["Server"].ToString(), record.Instance) == 0)
                    return row;
            }
            return null;
        }

        #region Public Methods

        //------------------------------------------------
        // LoadServers - load view contents from database
        //------------------------------------------------
        public void LoadServers()
        {
            // if we are already loaded; we dont reload - refresh or reconnection reset will clear the flag
            if (isLoaded) return;

            Cursor = Cursors.WaitCursor;

            _dsServers.Rows.Clear();
            _dsDatabases.Rows.Clear();

            List<ServerRecord> serverList;
            serverList = ServerRecord.GetServers(Globals.Repository.Connection, true, true);

            if ((serverList != null) && (serverList.Count != 0))
            {
                foreach (ServerRecord config in serverList)
                {
                    UltraDataRow row = _dsServers.Rows.Add();
                    UpdateServerRowValues(row, config);
                }
            }

            isLoaded = true;
            Cursor = Cursors.Default;

            if (_dsServers.Rows.Count == 0)
            {
                SetMenuFlag(CMMenuItem.NewDatabase, false);
            }
            else
                _gridServers.Rows[0].Selected = true;
        }

        #endregion

        #region Server - Deploy and Upgrade Agent

        //-------------------------------------------------------------
        // InternalDeployAgent - Deploy agent for registered server
        //--------------------------------------------------------------
        private void InternalDeployAgent()
        {
            bool bActivated = false;

            ServerRecord oldServerState;

            if (_gridServers.Selected.Rows.Count == 0) return;
            UltraDataRow serverRow = (UltraDataRow)_gridServers.Selected.Rows[0].ListObject;
            ServerRecord config = (ServerRecord)serverRow.Tag;

            Cursor = Cursors.WaitCursor;

            config.Connection = Globals.Repository.Connection;

            // check for already deployed - check other instances - not a call to the agent!
            bool alreadyDeployed = config.IsAgentDeployed(Globals.Repository.Connection);
            if (alreadyDeployed)
            {
                try
                {
                    // need to register with agent
                    AgentManager manager = GUIRemoteObjectsProvider.AgentManager();

                    manager.Activate(config.Instance);
                    oldServerState = config.Clone();
                    config.IsDeployed = true;
                    config.IsRunning = true;
                    config.Write(oldServerState);

                    MessageBox.Show(UIConstants.Info_DeployComplete,
                                    UIConstants.Title_DeployAgent,
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }
                catch (Exception e)
                {
                    ErrorMessage.Show(UIConstants.Title_DeployAgent,
                                      UIConstants.Error_DeployFailed,
                                      e.Message,
                                      MessageBoxIcon.Error);
                }
                UpdateServerRowValues(serverRow, config);
                SetServerContextMenuStatus();
                return;
            }

            // Agent is not deployed on the machine yet
            //   Non-Manual case     - Attempt install
            //   Manual Install case - indicate that this requires manual install;
            //                         ask if they want to override this and attempt
            //                         local install
            if (config.IsDeployedManually)
            {
                DialogResult choice = MessageBox.Show(UIConstants.Info_DeployManualInstallOverride,
                                                      UIConstants.Title_DeployAgent,
                                                      MessageBoxButtons.YesNo,
                                                      MessageBoxIcon.Question);
                if (choice != DialogResult.Yes)
                    return;
            }

            bool success = false;

            Form_ServerDeploy frm = new Form_ServerDeploy(config.Instance);
            if (DialogResult.OK == frm.ShowDialog())
            {
                try
                {
                    config.IsDeployed = true;
                    ServerRecord.SetIsFlags(config.Instance,
                                            config.IsDeployed,
                                            config.IsDeployedManually,
                                            config.IsRunning,
                                            config.IsCrippled,
                                            Globals.Repository.Connection);

                    ProgressForm progressForm = new ProgressForm(
                       "Deploying SQLcompliance Agent on " + config.InstanceServer + "...",
                       config.InstanceServer,
                       frm.ServiceAccount,
                       frm.ServicePassword,
                       frm.TraceDirectory,
                       config.Instance,
                       Globals.SQLcomplianceConfig.Server,
                       DeploymentType.Install);
                    progressForm.ShowDialog();

                    if (!progressForm.IsCancelled)
                    {
                        success = progressForm.IsServiceStarted;
                    }
                    if (progressForm.IsCancelled || !success)
                    {
                        config.IsDeployed = false;
                        ServerRecord.SetIsFlags(config.Instance,
                                                config.IsDeployed,
                                                config.IsDeployedManually,
                                                config.IsRunning,
                                                config.IsCrippled,
                                                Globals.Repository.Connection);
                    }
                }
                catch (Exception)
                {
                    config.IsDeployed = false;
                    ServerRecord.SetIsFlags(config.Instance,
                                            config.IsDeployed,
                                            config.IsDeployedManually,
                                            config.IsRunning,
                                            config.IsCrippled,
                                            Globals.Repository.Connection);
                }
            }
            else
            {
                return;
            }

            if (success)
            {
                // agent deployed - update all instances on that computer
                ICollection servers = ServerRecord.GetServers(Globals.Repository.Connection,
                                                              true);
                foreach (ServerRecord srvrec in servers)
                {
                    if (srvrec.InstanceServer.ToUpper() == config.InstanceServer.ToUpper())
                    {
                        oldServerState = srvrec.Clone();
                        if (oldServerState.IsDeployed)
                        {
                            srvrec.IsRunning = true;
                            srvrec.IsDeployed = true;
                            srvrec.AgentTraceDirectory = frm.TraceDirectory;
                            srvrec.AgentServiceAccount = frm.ServiceAccount;
                            srvrec.Write(oldServerState);

                            LogRecord.WriteLog(Globals.Repository.Connection,
                                               LogType.DeployAgent,
                                               srvrec.Instance,
                                               "SQLcompliance Agent deployed");
                        }
                    }
                }

                // set flags on all instances sharing this computer
                foreach (UltraDataRow row in _dsServers.Rows)
                {
                    ServerRecord tempConfig = (ServerRecord)row.Tag;
                    if (tempConfig.InstanceServer == config.InstanceServer)
                    {
                        tempConfig.IsDeployed = true;
                        tempConfig.IsDeployedManually = false;
                        UpdateServerRowValues(row, tempConfig);
                    }
                }
                bActivated = true;
            }

            Cursor = Cursors.Default;

            if (bActivated)
            {
                MessageBox.Show(UIConstants.Info_DeployComplete,
                                UIConstants.Title_DeployAgent,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                SetServerContextMenuStatus();
            }
        }

        //-------------------------------------------------------------
        // InternalUpgradeAgent - Upgrade agent at registered server
        //--------------------------------------------------------------
        private void InternalUpgradeAgent()
        {
            // Assume minor upgrade.  If it is major, we must act differently
            bool minorUpgrade = true;

            if (_gridServers.Selected.Rows.Count == 0) return;
            UltraDataRow serverRow = (UltraDataRow)_gridServers.Selected.Rows[0].ListObject;
            ServerRecord config = (ServerRecord)serverRow.Tag;

            try
            {
                Cursor = Cursors.WaitCursor;

                // If trying to upgrade local agent, tell them they need to use full install
                if (config.InstanceServer.ToUpper() == Dns.GetHostName().ToUpper())
                {
                    MessageBox.Show(UIConstants.Info_CantUpgradeLocal,
                                    UIConstants.Title_UpgradeAgent,
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }

                // If trying to upgrade a manually deployed agent, tell them they need to use full install
                if (config.IsDeployedManually)
                {
                    MessageBox.Show(UIConstants.Info_CantUpgradeManual,
                                    UIConstants.Title_UpgradeAgent,
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }

                // validate that GUI version is greater then agent version
                try
                {
                    ServerRecord srv = new ServerRecord();
                    srv.Connection = Globals.Repository.Connection;
                    if (srv.Read(config.Instance))
                    {
                        if (srv.AgentVersion != null && srv.AgentVersion != "")
                        {
                            string guiVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

                            if (ServerRecord.CompareVersions(srv.AgentVersion, guiVersion) > 0)
                            {
                                MessageBox.Show(UIConstants.Info_CantUpgradeAgentNewer,
                                                UIConstants.Title_UpgradeAgent,
                                                MessageBoxButtons.OK,
                                                MessageBoxIcon.Information);
                                return;
                            }

                            if (ServerRecord.CompareVersions(srv.AgentVersion, guiVersion) == 0)
                            {
                                MessageBox.Show(UIConstants.Info_AlreadyUpgraded,
                                                UIConstants.Title_UpgradeAgent,
                                                MessageBoxButtons.OK,
                                                MessageBoxIcon.Information);
                                return;
                            }
                            // if the version is the same, then always do a minor upgrade
                            if (ServerRecord.CompareReleaseVersions(srv.AgentVersion, guiVersion) == 0)
                            {
                                //if (AgentServiceManager.GetCPUFamily(srv.AgentServer) == AgentServiceManager.CpuFamily.x64)
                                //   minorUpgrade = false;
                                //else
                                minorUpgrade = true;
                            }
                            else
                                minorUpgrade = false;
                        }
                    }
                }
                catch
                {
                }

                // Perform the Upgrade
                config.Connection = Globals.Repository.Connection;

                bool upgradeSuccessful = false;
                ProgressForm progressForm;
                if (minorUpgrade)
                {
                    progressForm = new ProgressForm(
                       "Upgrading SQLcompliance Agent on " + config.InstanceServer + "...",
                       config.InstanceServer,
                       "",
                       "",
                       "",
                       config.Instance,
                       Globals.SQLcomplianceConfig.Server,
                       DeploymentType.MinorUpgrade);
                    progressForm.ShowDialog();
                }
                else
                {
                    Form_AgentMajorUpgrade upgradeForm = new Form_AgentMajorUpgrade(config.InstanceServer);

                    if (upgradeForm.ShowDialog() == DialogResult.OK)
                    {
                        progressForm = new ProgressForm(
                           "Upgrading SQLcompliance Agent on " + config.InstanceServer + "...",
                           config.InstanceServer,
                           upgradeForm.ServiceAccount,
                           upgradeForm.ServicePassword,
                           "",
                           config.Instance,
                           Globals.SQLcomplianceConfig.Server,
                           DeploymentType.MajorUpgrade);
                        progressForm.ShowDialog();
                    }
                    else
                        return;
                }

                // Verify that an upgrade actually happened
                try
                {
                    ServerRecord srv = new ServerRecord();
                    srv.Connection = Globals.Repository.Connection;
                    if (srv.Read(config.Instance))
                    {
                        if (srv.AgentVersion != null && srv.AgentVersion != "")
                        {
                            string guiVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

                            if (ServerRecord.CompareVersions(srv.AgentVersion, guiVersion) == 0)
                            {
                                upgradeSuccessful = true;
                            }
                        }
                    }
                }
                catch
                {
                }

                if (!progressForm.IsCancelled && upgradeSuccessful)
                {
                    MessageBox.Show(UIConstants.Info_UpgradeComplete,
                                    UIConstants.Title_UpgradeAgent,
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }
                if (!upgradeSuccessful)
                {
                    MessageBox.Show(UIConstants.Error_UpgradeFailedNoInfo,
                                    UIConstants.Title_UpgradeAgent,
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage.Show(UIConstants.Title_UpgradeAgent,
                                  UIConstants.Error_UpgradeFailed,
                                  ex.Message,
                                  MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        #endregion

        #region Internal (CheckAgent,StartAgent,StopAgent)

        //-------------------------------------------------------------
        // InternalCheckAgent
        //--------------------------------------------------------------
        private void InternalCheckAgent()
        {
            ServerRecord config = GetSelectedServer();
            if (config == null) return;

            Cursor = Cursors.WaitCursor;
            _mainForm.CheckAgent(config);
            Cursor = Cursors.Default;
        }

        #endregion

        #region Server Logic

        //-------------------------------------------------------------
        // ShowServerProperties
        //--------------------------------------------------------------
        private void ShowServerProperties()
        {
            ServerRecord config = GetSelectedServer();
            if (config == null) return;

            Cursor = Cursors.WaitCursor;
            _mainForm.ShowServerProperties(config);
            Cursor = Cursors.Default;
        }

        public void UserProperties()
        {
            if (_gridServers.Focused)
            {
                ServerRecord config = GetSelectedServer();
                if (config == null) return;

                Cursor = Cursors.WaitCursor;
                _mainForm.ShowServerProperties(config, Form_ServerProperties.Context.PrivilegedUser);
                Cursor = Cursors.Default;

            }
            else if (_gridDatabases.Focused)
            {
                DatabaseRecord db = GetSelectedDatabase();
                if (db == null) return;

                Cursor = Cursors.WaitCursor;
                _mainForm.ShowDatabaseProperties(db, Form_DatabaseProperties.Context.TrustedUsers);
                Cursor = Cursors.Default;
            }
        }


        //-------------------------------------------------------------
        // EnableServers
        //--------------------------------------------------------------
        private void EnableServers(bool flag)
        {
            List<ServerRecord> servers = GetSelectedServers();
            if (servers == null || servers.Count == 0) return;

            if (flag)
                _mainForm.EnableServersAction(servers);
            else
                _mainForm.DisableServersAction(servers);

            SetServerContextMenuStatus();
        }

        #endregion

        #region Window Events



        #endregion

        #region Database Logic

        private static void UpdateDatabaseRowValue(UltraDataRow row, ServerRecord server, DatabaseRecord db)
        {
            string statusStr;
            Image statusImage;

            if (db.IsEnabled && server.IsEnabled)
            {
                statusStr = UIConstants.Status_Enabled;
                statusImage = AppIcons.AppImg16(AppIcons.Img16.Database);
            }
            else
            {
                statusStr = UIConstants.Status_Disabled;
                statusImage = AppIcons.AppImg16(AppIcons.Img16.DatabaseDisabled);
            }
            row["Icon"] = statusImage;
            row["Name"] = db.Name;
            row["Status"] = statusStr;
            row["Description"] = db.Description;
            if (server.SqlVersion >= 11)
            {
                row["IsAlwaysOn"] = db.IsAlwaysOn ? "Yes" : "No";
                if (db.IsAlwaysOn)
                    row["Role"] = db.IsPrimary ? "Primary" : "Secondary";
                else
                    row["Role"] = "NA";
            }
            else
            {
                row["IsAlwaysOn"] = "NA";
                row["Role"] = "NA";
            }
            row.Tag = db;
        }

        //-------------------------------------------------------------
        // LoadDatabases
        //--------------------------------------------------------------
        private void LoadDatabases()
        {
            _dsDatabases.Rows.Clear();
            ServerRecord config = GetSelectedServer();
            if (config == null)
                return;
            if (config.IsHadrEnabled)
            {
                //Update db record, if the Role of the Nodes is changed from Primary to Secondary or Vice versa
                UpdateDatabaseRole(config);
            }

            Cursor = Cursors.WaitCursor;


            List<DatabaseRecord> dbList = null;
            try
            {
                dbList = DatabaseRecord.GetDatabases(Globals.Repository.Connection, config.SrvId);
            }
            catch (Exception)
            {
            }

            if ((dbList != null) && (dbList.Count != 0))
            {
                foreach (DatabaseRecord db in dbList)
                {
                    UltraDataRow row = _dsDatabases.Rows.Add();
                    UpdateDatabaseRowValue(row, config, db);
                }
            }
            Cursor = Cursors.Default;
        }

        //------------------------------------------------------------------
        // UpdateDatabaseRole
        //------------------------------------------------------------------
        private void UpdateDatabaseRole(ServerRecord server)
        {
            try
            {
                AgentManager manager = GUIRemoteObjectsProvider.AgentManager();
                var readOnlySecondaryReplicaList = manager.GetAllReplicaNodeInfoList(server.Instance);

                foreach (var replicaInfo in readOnlySecondaryReplicaList)
                {
                    if (replicaInfo.ReplicaServerName.ToLower() == server.Instance.ToLower())
                    {
                        bool isRolePrimary = replicaInfo.IsPrimary;
                        server.Connection = Globals.Repository.Connection;
                        bool dbRoleUpdated = server.UpdateDbRecord(server, isRolePrimary);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(
                    ErrorLog.Level.Always,
                    new Exception("Failed to update Database Roles", ex),
                    ErrorLog.Severity.Warning);
            }
        }

        //-------------------------------------------------------------
        // ShowDatabaseProperties
        //--------------------------------------------------------------
        private void ShowDatabaseProperties()
        {
            DatabaseRecord db = GetSelectedDatabase();
            if (db == null) return;

            Cursor = Cursors.WaitCursor;
            _mainForm.ShowDatabaseProperties(db);
            Cursor = Cursors.Default;
        }

        //-------------------------------------------------------------
        // RemoveDatabases
        //--------------------------------------------------------------
        private void RemoveDatabases()
        {
            List<DatabaseRecord> dbs = GetSelectedDatabases();
            if (dbs == null || dbs.Count == 0) return;

            _mainForm.RemoveDatabasesAction(dbs);
        }

        //-------------------------------------------------------------
        // EnableDatabases
        //--------------------------------------------------------------
        private void EnableDatabases(bool flag)
        {
            List<DatabaseRecord> dbs = GetSelectedDatabases();
            if (dbs == null || dbs.Count == 0) return;

            if (flag)
                _mainForm.EnableDatabasesAction(dbs);
            else
                _mainForm.DisableDatabasesAction(dbs);
            SetServerContextMenuStatus();
            SetDatabaseContextMenuStatus();
        }

        //-------------------------------------------------------------
        // SetContextMenuStatus
        //--------------------------------------------------------------
        private void SetDatabaseContextMenuStatus()
        {
            if (_gridDatabases.Focused)
            {
                SetMenuFlag(CMMenuItem.ServerAuditSettings, false);
                SetMenuFlag(CMMenuItem.PrivilegedUserSettings, false);

                ServerRecord server = GetSelectedServer();
                bool serverEnabled = server == null ? false : server.IsEnabled;

                SetMenuFlag(CMMenuItem.DeployAgent, false);
                SetMenuFlag(CMMenuItem.AgentProperties, false);
                SetMenuFlag(CMMenuItem.CheckAgent, false);

                if (_gridDatabases.Selected.Rows.Count == 0)
                {
                    SetMenuFlag(CMMenuItem.Delete, false);
                    SetMenuFlag(CMMenuItem.EnableAuditing, false);
                    SetMenuFlag(CMMenuItem.DisableAuditing, false);
                    SetMenuFlag(CMMenuItem.Properties, false);
                    SetMenuFlag(CMMenuItem.DatabaseAuditSettings, false);
                    SetMenuFlag(CMMenuItem.TrustedUserSettings, false);
                    SetMenuFlag(CMMenuItem.ExportAuditSettings, false);
                }
                else
                {
                    if (_gridDatabases.Selected.Rows.Count == 1)
                    {
                        SetMenuFlag(CMMenuItem.Properties, true);
                        SetMenuFlag(CMMenuItem.DatabaseAuditSettings, true);
                        SetMenuFlag(CMMenuItem.TrustedUserSettings, true);
                        SetMenuFlag(CMMenuItem.ExportAuditSettings, true);

                        bool isSqlSecure;
                        // Database Object
                        DatabaseRecord dbRecord = GetSelectedDatabase();
                        isSqlSecure = dbRecord.IsSqlSecureDb;

                        if (isSqlSecure)
                        {
                            SetMenuFlag(CMMenuItem.Delete, false);
                            SetMenuFlag(CMMenuItem.EnableAuditing, false);
                            SetMenuFlag(CMMenuItem.DisableAuditing, false);
                        }
                        else
                        {
                            SetMenuFlag(CMMenuItem.Delete, Globals.isSysAdmin);

                            // check state of selected item for enable/disable
                            if (serverEnabled)
                            {
                                if (server.IsEnabled && dbRecord.IsEnabled)
                                    SetMenuFlag(CMMenuItem.EnableAuditing, false);
                                else
                                    SetMenuFlag(CMMenuItem.EnableAuditing, Globals.isAdmin);

                                if (!Globals.isAdmin)
                                    SetMenuFlag(CMMenuItem.DisableAuditing, false);
                                else
                                    SetMenuFlag(CMMenuItem.DisableAuditing, !GetMenuFlag(CMMenuItem.EnableAuditing));
                            }
                            else
                            {
                                SetMenuFlag(CMMenuItem.EnableAuditing, false);
                                SetMenuFlag(CMMenuItem.DisableAuditing, false);
                            }
                        }
                        SetMenuFlag(CMMenuItem.ExportAuditSettings, true);
                    }
                    else // multi-select
                    {
                        SetMenuFlag(CMMenuItem.Properties, false);
                        SetMenuFlag(CMMenuItem.DatabaseAuditSettings, false);
                        SetMenuFlag(CMMenuItem.TrustedUserSettings, false);
                        SetMenuFlag(CMMenuItem.EnableAuditing, Globals.isAdmin && serverEnabled);
                        SetMenuFlag(CMMenuItem.DisableAuditing, Globals.isAdmin && serverEnabled);
                        SetMenuFlag(CMMenuItem.ExportAuditSettings, false);
                    }
                }

                // disable certain menu items if license is expired            
                if (!Globals.SQLcomplianceConfig.LicenseObject.IsProductLicensed())
                {
                    SetMenuFlag(CMMenuItem.EnableAuditing, false);
                    SetMenuFlag(CMMenuItem.DisableAuditing, false);
                    SetMenuFlag(CMMenuItem.NewDatabase, false);
                }
                else
                {
                    SetMenuFlag(CMMenuItem.NewDatabase, Globals.isAdmin);
                }
            }
        }

        #endregion

        #region Base Control Overrides

        public override void AgentProperties()
        {
            if (_gridServers.Focused)
                ShowAgentProperties();
        }


        public override void AgentTraceDirectory()
        {
            if (_gridServers.Focused)
                ChangeAgentTraceDirectory();
        }

        //-------------------------------------------------------------
        // Properties - public function called from here and mainForm menus
        //--------------------------------------------------------------
        override public void Properties()
        {
            if (_gridDatabases.Focused)
                ShowDatabaseProperties();
            else
                ShowServerProperties();
        }

        //-------------------------------------------------------------
        // Enable - public function called from here and mainForm menus
        //--------------------------------------------------------------
        override public void Enable(bool flag)
        {
            if (_gridDatabases.Focused)
                EnableDatabases(flag);
            else if (_gridServers.Focused)
                EnableServers(flag);
        }

        //-----------------------------------------------------
        // RefreshView - Called from mainForm refresh handlers
        //-----------------------------------------------------
        override public void RefreshView()
        {
            isLoaded = false;

            SetMenuFlag(CMMenuItem.NewServer, Globals.isAdmin);
            SetMenuFlag(CMMenuItem.NewDatabase, Globals.isAdmin);

            if (_gridServers.Focused || _gridServers.Selected.Rows.Count != 1)
            {
                isLoaded = false;
                LoadServers();
            }
            else
            {
                LoadDatabases();
            }
        }

        //-------------------------------------------------------------
        // UpdateNow
        //--------------------------------------------------------------
        override public void UpdateNow()
        {
            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                    String.Format("Processing ServerView::UpdateNow().  Selected item count = {0}.",
                                                  _gridServers.Selected.Rows.Count));

            if (_gridServers.Selected.Rows.Count == 0) return;
            UltraDataRow row = (UltraDataRow)_gridServers.Selected.Rows[0].ListObject;
            ServerRecord config = (ServerRecord)row.Tag;
            if (config == null) return;

            config.Connection = Globals.Repository.Connection;
            this.Cursor = Cursors.WaitCursor;
            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                    String.Format("Sending UpdateNow command to {0}.",
                                                  config.Instance));
            try
            {
                AgentManager manager = GUIRemoteObjectsProvider.AgentManager();

                manager.UpdateAuditConfiguration(config.Instance);
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                        String.Format("UpdateNow command sent to {0}.",
                                                      config.Instance));

                ServerRecord oldServerState = config.Clone();
                config.ConfigUpdateRequested = true;
                config.Write(oldServerState);
                UpdateServerRowValues(row, config);
            }
            catch (Exception ex)
            {
                ErrorMessage.Show(this.Text,
                                  UIConstants.Error_UpdateNowFailed,
                                  UIUtils.TranslateRemotingException(Globals.SQLcomplianceConfig.Server,
                                                                     UIConstants.CollectionServiceName,
                                                                     ex),
                                  MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        //-------------------------------------------------------------
        // DeployAgent
        //--------------------------------------------------------------
        override public void DeployAgent()
        {
            if (_gridDatabases.Focused != true)
            {
                InternalDeployAgent();
            }
        }

        //-------------------------------------------------------------
        // UpgradeAgent
        //--------------------------------------------------------------
        override public void UpgradeAgent()
        {
            if (_gridDatabases.Focused != true)
            {
                InternalUpgradeAgent();
            }
        }

        //-------------------------------------------------------------
        // CheckAgent
        //--------------------------------------------------------------
        override public void CheckAgent()
        {
            if (_gridDatabases.Focused != true)
            {
                InternalCheckAgent();
                RefreshView();
            }
        }

        //-------------------------------------------------------------
        // Delete - public function called from here and mainForm menus
        //--------------------------------------------------------------
        override public void Delete()
        {
            if (_gridDatabases.Focused)
            {
                RemoveDatabases();
            }
            else
            {
                ServerRecord server = GetSelectedServer();
                if (server != null)
                    _mainForm.RemoveServerAction(server);
            }
        }

        //-------------------------------------------------------------
        // DoSnapshot
        //--------------------------------------------------------------
        override public void DoSnapshot(string myInstance)
        {
            string snapshotInstance = myInstance;

            if (myInstance == "")
            {
                if (_gridServers.Focused && (_gridServers.Selected.Rows.Count == 1))
                {
                    ServerRecord record = GetSelectedServer();
                    snapshotInstance = record.Instance;
                }
            }
            _mainForm.DoSnapshot(snapshotInstance);
        }

        #endregion


        #region Help

        //--------------------------------------------------------------------
        // HelpRequested_ServerView - Show Context Sensitive Help
        //--------------------------------------------------------------------
        private void HelpRequested_ServerView(object sender, HelpEventArgs hlpevent)
        {
            HelpOnThisWindow();
            hlpevent.Handled = true;
        }

        public override void HelpOnThisWindow()
        {
            HelpAlias.ShowHelp(this, HelpAlias.SSHELP_ServerView);
        }

        #endregion

        //-------------------------------------------------------------
        // ShowAgentProperties
        //--------------------------------------------------------------
        private void ShowAgentProperties()
        {
            ServerRecord config = GetSelectedServer();
            if (config != null)
                _mainForm.ShowAgentProperties(config);
        }

        //-------------------------------------------------------------
        // ChangeAgentTraceDirectory
        //--------------------------------------------------------------
        private void ChangeAgentTraceDirectory()
        {
            ServerRecord config = GetSelectedServer();
            if (config != null)
                _mainForm.ChangeAgentTraceDirectory(config);
        }

        //-------------------------------------------------------------
        // SetServerContextMenuStatus
        //--------------------------------------------------------------
        private void SetServerContextMenuStatus()
        {
            if (_gridServers.Focused)
            {
                SetMenuFlag(CMMenuItem.DatabaseAuditSettings, false);
                SetMenuFlag(CMMenuItem.TrustedUserSettings, false);

                // Enable and disable context menu based on number of selections
                if (_gridServers.Selected.Rows.Count == 0)
                {
                    SetMenuFlag(CMMenuItem.Delete, false);
                    SetMenuFlag(CMMenuItem.Properties, false);
                    SetMenuFlag(CMMenuItem.EnableAuditing, false);
                    SetMenuFlag(CMMenuItem.DisableAuditing, false);
                    SetMenuFlag(CMMenuItem.DeployAgent, false);
                    SetMenuFlag(CMMenuItem.UpgradeAgent, false);
                    SetMenuFlag(CMMenuItem.CheckAgent, false);
                    SetMenuFlag(CMMenuItem.ExportAuditSettings, false);
                    SetMenuFlag(CMMenuItem.AgentProperties, false);
                    SetMenuFlag(CMMenuItem.ChangeAgentTraceDir, false);
                    SetMenuFlag(CMMenuItem.UpdateAuditSettings, false);
                    SetMenuFlag(CMMenuItem.ForceCollection, false);
                    SetMenuFlag(CMMenuItem.Groom, false);
                    SetMenuFlag(CMMenuItem.ServerAuditSettings, false);
                    SetMenuFlag(CMMenuItem.PrivilegedUserSettings, false);
                }
                else if (_gridServers.Selected.Rows.Count == 1)
                {
                    ServerRecord config = GetSelectedServer();

                    SetMenuFlag(CMMenuItem.Delete, Globals.isSysAdmin);
                    SetMenuFlag(CMMenuItem.Properties, true);
                    SetMenuFlag(CMMenuItem.NewDatabase, config.IsAuditedServer);

                    SetMenuFlag(CMMenuItem.DeployAgent, Globals.isAdmin && !config.IsDeployed);
                    //SetMenuFlag(CMMenuItem.UpgradeAgent, Globals.isAdmin && config.IsDeployed
                    //                                     && !config.isClustered);
                    SetMenuFlag(CMMenuItem.UpgradeAgent, CanUpgradeAgent(config));
                    SetMenuFlag(CMMenuItem.CheckAgent, Globals.isAdmin);
                    SetMenuFlag(CMMenuItem.AgentProperties, true);
                    SetMenuFlag(CMMenuItem.ChangeAgentTraceDir, Globals.isAdmin && config.IsDeployed);
                    SetMenuFlag(CMMenuItem.ExportAuditSettings, true);
                    SetMenuFlag(CMMenuItem.ServerAuditSettings, true);
                    SetMenuFlag(CMMenuItem.PrivilegedUserSettings, true);

                    if (!Globals.isAdmin)
                    {
                        SetMenuFlag(CMMenuItem.EnableAuditing, false);
                        SetMenuFlag(CMMenuItem.DisableAuditing, false);
                        SetMenuFlag(CMMenuItem.UpdateAuditSettings, false);
                        SetMenuFlag(CMMenuItem.ForceCollection, false);
                        SetMenuFlag(CMMenuItem.Groom, false);
                    }
                    else
                    {
                        SetMenuFlag(CMMenuItem.EnableAuditing, !config.IsEnabled);
                        SetMenuFlag(CMMenuItem.DisableAuditing, config.IsEnabled);

                        if (config.ConfigVersion != config.LastKnownConfigVersion)
                            SetMenuFlag(CMMenuItem.UpdateAuditSettings, true);
                        else
                            SetMenuFlag(CMMenuItem.UpdateAuditSettings, false);

                        SetMenuFlag(CMMenuItem.ForceCollection, true);

                        SetMenuFlag(CMMenuItem.Groom,
                                    Globals.isAdmin && Globals.SQLcomplianceConfig.GroomEventAllow);
                    }
                }
                else // multi-select
                {
                    SetMenuFlag(CMMenuItem.Delete, false);
                    SetMenuFlag(CMMenuItem.Properties, false);
                    SetMenuFlag(CMMenuItem.EnableAuditing, Globals.isAdmin);
                    SetMenuFlag(CMMenuItem.DisableAuditing, Globals.isAdmin);
                    SetMenuFlag(CMMenuItem.DeployAgent, false);
                    SetMenuFlag(CMMenuItem.UpgradeAgent, false);
                    SetMenuFlag(CMMenuItem.CheckAgent, false);
                    SetMenuFlag(CMMenuItem.ExportAuditSettings, false);
                    SetMenuFlag(CMMenuItem.AgentProperties, false);
                    SetMenuFlag(CMMenuItem.ChangeAgentTraceDir, false);
                    SetMenuFlag(CMMenuItem.UpdateAuditSettings, false);
                    SetMenuFlag(CMMenuItem.ForceCollection, false);
                    SetMenuFlag(CMMenuItem.Groom, false);
                    SetMenuFlag(CMMenuItem.ServerAuditSettings, false);
                    SetMenuFlag(CMMenuItem.PrivilegedUserSettings, false);
                }

                // disable certain menu items if license is expired            
                if (!Globals.SQLcomplianceConfig.LicenseObject.IsProductLicensed())
                {
                    SetMenuFlag(CMMenuItem.EnableAuditing, false);
                    SetMenuFlag(CMMenuItem.DisableAuditing, false);
                    SetMenuFlag(CMMenuItem.NewServer, false);
                    SetMenuFlag(CMMenuItem.NewDatabase, false);
                    SetMenuFlag(CMMenuItem.UpdateAuditSettings, false);
                    SetMenuFlag(CMMenuItem.ForceCollection, false);
                    SetMenuFlag(CMMenuItem.DeployAgent, false);
                    SetMenuFlag(CMMenuItem.UpgradeAgent, false);
                    SetMenuFlag(CMMenuItem.CheckAgent, false);
                    SetMenuFlag(CMMenuItem.ChangeAgentTraceDir, false);
                }
                else
                {
                    SetMenuFlag(CMMenuItem.NewServer, Globals.isAdmin);
                    SetMenuFlag(CMMenuItem.NewDatabase, Globals.isAdmin);
                }
            }
        }

        public void ExportAuditSettings()
        {
            if (_gridDatabases.Focused)
            {
                DatabaseRecord db = GetSelectedDatabase();
                _mainForm.ExportDatabaseAuditSettingsAction(db);
            }
            else if (_gridServers.Focused)
            {
                ServerRecord server = GetSelectedServer();
                _mainForm.ExportServerAuditSettingsAction(server);
            }
        }

        private void DoubleClickRow_gridServers(object sender, DoubleClickRowEventArgs e)
        {
            ShowServerProperties();
        }

        private void FocusChanged_gridServers(object sender, EventArgs e)
        {
            SetServerContextMenuStatus();
        }

        private void KeyDown_gridServers(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ShowServerProperties();
            }
        }

        private void AfterSelectChange_gridServers(object sender, AfterSelectChangeEventArgs e)
        {
            SetServerContextMenuStatus();

            //----------------------------------------------------------
            // Load and Enable Database stuff based on server selection
            //----------------------------------------------------------
            if (_gridServers.Selected.Rows.Count != 1)
            {
                _dsDatabases.Rows.Clear();
                //            _gridDatabases.Enabled = false;
            }
            else
            {
                //            _gridDatabases.Enabled = true;
                LoadDatabases();
            }
        }

        private void MouseDown_gridServers(object sender, MouseEventArgs e)
        {
            UIElement elementMain;
            UIElement elementUnderMouse;
            _gridServers.Focus();
            elementMain = _gridServers.DisplayLayout.UIElement;
            elementUnderMouse = elementMain.ElementFromPoint(e.Location);
            if (elementUnderMouse != null)
            {
                UltraGridCell cell = elementUnderMouse.GetContext(typeof(UltraGridCell)) as UltraGridCell;
                if (cell != null)
                {
                    if (!cell.Row.Selected)
                    {
                        if (e.Button == MouseButtons.Right)
                        {
                            _gridServers.Selected.Rows.Clear();
                            cell.Row.Selected = true;
                            cell.Row.Activate();
                            //_gridServers.ActiveRow = cell.Row;
                        }
                    }
                }
                else
                {
                    HeaderUIElement he = elementUnderMouse.GetAncestor(typeof(HeaderUIElement)) as HeaderUIElement;
                    if (he == null)
                    {
                        _gridServers.Selected.Rows.Clear();
                        _gridServers.ActiveRow = null;
                    }
                }
            }
        }

        private void DoubleClickRow_gridDatabases(object sender, DoubleClickRowEventArgs e)
        {
            ShowDatabaseProperties();
        }

        private void FocusChanged_gridDatabases(object sender, EventArgs e)
        {
            SetDatabaseContextMenuStatus();
        }

        private void KeyDown_gridDatabases(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ShowDatabaseProperties();
            }
        }

        private void MouseDown_gridDatabases(object sender, MouseEventArgs e)
        {
            UIElement elementMain;
            UIElement elementUnderMouse;
            _gridDatabases.Focus();
            elementMain = _gridDatabases.DisplayLayout.UIElement;
            elementUnderMouse = elementMain.ElementFromPoint(e.Location);
            if (elementUnderMouse != null)
            {
                UltraGridCell cell = elementUnderMouse.GetContext(typeof(UltraGridCell)) as UltraGridCell;
                if (cell != null)
                {
                    if (!cell.Row.Selected)
                    {
                        if (e.Button == MouseButtons.Right)
                        {
                            _gridDatabases.Selected.Rows.Clear();
                            cell.Row.Selected = true;
                            cell.Row.Activate();
                            //_gridDatabases.ActiveRow = cell.Row;
                        }
                    }
                }
                else
                {
                    HeaderUIElement he = elementUnderMouse.GetAncestor(typeof(HeaderUIElement)) as HeaderUIElement;
                    if (he == null)
                    {
                        _gridDatabases.Selected.Rows.Clear();
                        _gridDatabases.ActiveRow = null;
                    }
                }
            }
        }

        private void AfterSelectChange_gridDatabases(object sender, AfterSelectChangeEventArgs e)
        {
            SetDatabaseContextMenuStatus();
        }

        private void ToolClick_toolbarsManager(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "newServer":
                    _mainForm.AddServerAction();
                    break;
                case "newDatabase":
                    _mainForm.AddDatabaseAction(GetSelectedServer());
                    break;
                case "enableAuditing":
                    Enable(true);
                    break;
                case "disableAuditing":
                    Enable(false);
                    break;
                case "remove":
                    Delete();
                    break;
                case "refresh":
                    isLoaded = false;
                    LoadServers();
                    LoadDatabases();

                    break;
                case "updateNow":
                    ErrorLog.Instance.Write(ErrorLog.Level.UltraDebug,
                                            "menuUpdateNow clicked.");
                    UpdateNow();
                    ErrorLog.Instance.Write(ErrorLog.Level.UltraDebug,
                                            "Click_menuUpdateNow processed.");
                    break;
                case "exportAuditSettings":
                    ExportAuditSettings();
                    break;
                case "collectNow":
                    ServerRecord config = GetSelectedServer();
                    if (config == null) return;

                    config.Connection = Globals.Repository.Connection;
                    Cursor = Cursors.WaitCursor;
                    Server.ForceCollection(config.Instance);
                    Cursor = Cursors.Default; break;
                case "agentDeploy":
                    DeployAgent();
                    break;
                case "agentUpgrade":
                    UpgradeAgent();
                    break;
                case "agentStatus":
                    CheckAgent();
                    break;
                case "agentTraceDirectory":
                    ChangeAgentTraceDirectory();
                    break;
                case "agentProperties":
                    ShowAgentProperties();
                    break;
                case "properties":
                    Properties();
                    break;
                case "importCSV":
                    ErrorLog.Instance.Write(ErrorLog.Level.UltraDebug, "Import Sensitive Columns from CSV...");
                    break;
            }
        }


        private void BeforeToolDropdown_toolbarsManager(object sender, Infragistics.Win.UltraWinToolbars.BeforeToolDropdownEventArgs e)
        {
            _toolbarsManager.Tools["newServer"].SharedProps.Enabled = GetMenuFlag(CMMenuItem.NewServer);
            _toolbarsManager.Tools["newDatabase"].SharedProps.Enabled = GetMenuFlag(CMMenuItem.NewDatabase);
            _toolbarsManager.Tools["disableAuditing"].SharedProps.Enabled = GetMenuFlag(CMMenuItem.DisableAuditing);
            _toolbarsManager.Tools["enableAuditing"].SharedProps.Enabled = GetMenuFlag(CMMenuItem.EnableAuditing);
            _toolbarsManager.Tools["updateNow"].SharedProps.Enabled = GetMenuFlag(CMMenuItem.UpdateAuditSettings);
            _toolbarsManager.Tools["exportAuditSettings"].SharedProps.Enabled = GetMenuFlag(CMMenuItem.ExportAuditSettings);
            _toolbarsManager.Tools["collectNow"].SharedProps.Enabled = GetMenuFlag(CMMenuItem.ForceCollection);
            _toolbarsManager.Tools["agentDeploy"].SharedProps.Enabled = GetMenuFlag(CMMenuItem.DeployAgent);
            _toolbarsManager.Tools["agentUpgrade"].SharedProps.Enabled = GetMenuFlag(CMMenuItem.UpgradeAgent);
            _toolbarsManager.Tools["agentStatus"].SharedProps.Enabled = GetMenuFlag(CMMenuItem.CheckAgent);
            _toolbarsManager.Tools["agentTraceDirectory"].SharedProps.Enabled = GetMenuFlag(CMMenuItem.ChangeAgentTraceDir);
            _toolbarsManager.Tools["agentProperties"].SharedProps.Enabled = GetMenuFlag(CMMenuItem.AgentProperties);
            _toolbarsManager.Tools["remove"].SharedProps.Enabled = GetMenuFlag(CMMenuItem.Delete);
            _toolbarsManager.Tools["properties"].SharedProps.Enabled = GetMenuFlag(CMMenuItem.Properties);
        }

        private bool CanUpgradeAgent(ServerRecord config)
        {
            bool response = false;
            if (Globals.isAdmin && !config.isClustered && config.IsDeployed)
            {
                response = !(String.Compare(config.AgentVersion, Globals.SQLcomplianceConfig.ServerCoreVersion) >= 0);
            }
            return response;
        }
    }
}
