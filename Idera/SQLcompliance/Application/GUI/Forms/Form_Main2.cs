using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using Qios.DevSuite.Components;
using Infragistics.Win;
using Infragistics.Win.AppStyling;
using Infragistics.Win.FormattedLinkLabel;
using Infragistics.Win.UltraWinExplorerBar;
using Infragistics.Win.UltraWinToolbars;
using Idera.SQLcompliance.Core.Cwf;
using Idera.SQLcompliance.Application.GUI.Controls;
using Idera.SQLcompliance.Application.GUI.Helper;
using Idera.SQLcompliance.Application.GUI.SQL;
using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Agent;
using Idera.SQLcompliance.Core.Collector;
using Idera.SQLcompliance.Core.Licensing;
using Idera.SQLcompliance.Core.Reports;
using Idera.SQLcompliance.Core.Rules;
using Idera.SQLcompliance.Core.Rules.Alerts;
using Idera.SQLcompliance.Core.Rules.Filters;
using Idera.SQLcompliance.Core.Stats;
using Idera.SQLcompliance.Core.Templates;
using Resources = Idera.SQLcompliance.Application.GUI.Properties.Resources;
using Idera.SQLcompliance.Core.Settings;
using System.Security.Principal;
using System.ComponentModel;
using System.Linq;
using Microsoft.Win32;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
    public partial class Form_Main2 : Form
    {
        private uint _internalUpdate = 0;
        private BaseControl _activeControl;
        private TreeView _activeTree;
        private AlertingConfiguration _alertingConfig;
        private FiltersConfiguration _filteringConfig;
        private Dictionary<StatsCategory, EnterpriseStatistics> _statistics;

        private PopupMenuTool _toolsMenu;
        private List<ButtonTool> _toolsItems;
        private List<EventHandler> _toolHandlers;
        //start sqlcm 5.6 - 5365
        ButtonTool defaultSettings;
        //end sqlcm 5.6 - 5365
        private ButtonTool[] _recentServerMenus;
        private string m_currentExportingDirectory = string.Empty;

        public event EventHandler<ServerEventArgs> ServerAdded;
        public event EventHandler<ServerEventArgs> ServerRemoved;
        public event EventHandler<ServerEventArgs> ServerModified;

        public event EventHandler<DatabaseEventArgs> DatabaseAdded;
        public event EventHandler<DatabaseEventArgs> DatabaseRemoved;
        public event EventHandler<DatabaseEventArgs> DatabaseModified;

        public event EventHandler<ArchiveEventArgs> ArchiveAttached;
        public event EventHandler<ArchiveEventArgs> ArchiveDetached;

        public event EventHandler<ConnectionChangedEventArgs> ConnectionChanged;
        private BackgroundWorker xeDefaultUpdateWorker = new BackgroundWorker();
        public Form_Main2()
        {
            InitializeComponent();
            this.Icon = Resources.SQLcompliance_product_ico;
            _treeAdmin.ImageList = AppIcons.AppImageList16();
            _treeServers.ImageList = AppIcons.AppImageList16();

            if (Settings.Default.CallUpgrade)
            {
                try
                {
                    // CallUpgrade will always be true after a new install or upgrade
                    //  Get the previous version to verify that we actually need to do an upgrade.
                    object o = Settings.Default.GetPreviousVersion("CallUpgrade");
                    if (o != null)
                    {
                        Settings.Default.Upgrade();
                        Settings.Default.Save();
                        _ribbonTabView.InitializeEventViewCombo();
                        _ribbonTabView.InitializeAlertViewCombo();
                    }
                }
                catch
                {
                }
                Settings.Default.CallUpgrade = false;
            }

            if (Settings.Default.FirstRun)
                LoadDefaultViews();

            _ribbonTabView.Initialize(this);
            _adminRibbonView.Initialize(this);
            _reportView.Initialize(this);

            _toolsMenu = (PopupMenuTool)_toolbarManager.Tools["mainTools"];
            //start sqlcm 5.6 - 5365
            defaultSettings = new ButtonTool("DefaultSettings");
            defaultSettings.SharedProps.Caption = "Default Settings";

            _toolbarManager.Tools.Add(defaultSettings);
            _toolsMenu.Tools.AddTool("DefaultSettings");
            //end sqlcm 5.6 - 5365
            _toolsItems = new List<ButtonTool>();

            foreach (ButtonTool b in _toolsMenu.Tools)
                _toolsItems.Add(b);
            _toolHandlers = new List<EventHandler>();

            _ribbonTabView.MenuFlagChanged += MenuFlagChanged_View;
            _adminRibbonView.MenuFlagChanged += MenuFlagChanged_View;

            _recentServerMenus = new ButtonTool[5];
            _recentServerMenus[0] = (ButtonTool)_toolbarManager.Tools["server1"];
            _recentServerMenus[1] = (ButtonTool)_toolbarManager.Tools["server2"];
            _recentServerMenus[2] = (ButtonTool)_toolbarManager.Tools["server3"];
            _recentServerMenus[3] = (ButtonTool)_toolbarManager.Tools["server4"];
            _recentServerMenus[4] = (ButtonTool)_toolbarManager.Tools["server5"];

            ShowToolbar(Settings.Default.ViewToolbar);
            ((StateButtonTool)_toolbarManager.Tools["viewTree"]).InitializeChecked(true);

            QGlobalColorScheme.Global.ColorsChanged += ColorsChanged_Global;            
        }

        private void UpdateColors()
        {
            UIElement headerElem;

            headerElem = _explorerBar.UIElement.GetDescendant(typeof(UltraExplorerBarGroupHeaderUIElement));
            while (headerElem == null)
            {
                System.Windows.Forms.Application.DoEvents();
                headerElem = _explorerBar.UIElement.GetDescendant(typeof(UltraExplorerBarGroupHeaderUIElement));
            }

            AppearanceData aData = new AppearanceData();
            AppearancePropFlags aPropFlags = AppearancePropFlags.AllRender;
            _explorerBar.Groups[0].ResolveHeaderAppearance(ref aData, ref aPropFlags, false, false, true, false, true, true);
            CopyAppearance(_lblTitle.Appearance, aData);

            aData = new AppearanceData();
            aPropFlags = AppearancePropFlags.AllRender;
            headerElem = _explorerBar.UIElement.GetDescendant(typeof(UltraExplorerBarGroupHeaderUIElement));
            _explorerBar.ResolveAppearance(ref aData, ref aPropFlags);
            _pnlHeader.BorderColor = aData.BorderColor;
            _pnlHeader.Height = headerElem.Rect.Height + _pnlHeader.Appearance.BorderWidth * 2;
            ;
            _pnlHeader.Visible = true;
        }

        private void CopyAppearance(AppearanceBase a, AppearanceData b)
        {
            a.AlphaLevel = b.AlphaLevel;
            a.BackColor = b.BackColor;
            a.BackColor2 = b.BackColor2;
            a.BackColorAlpha = b.BackColorAlpha;
            a.BackColorDisabled = b.BackColorDisabled;
            a.BackColorDisabled2 = b.BackColorDisabled2;
            a.BackGradientAlignment = b.BackGradientAlignment;
            a.BackGradientStyle = b.BackGradientStyle;
            a.BackHatchStyle = b.BackHatchStyle;
            a.BorderAlpha = b.BorderAlpha;
            a.BorderColor = b.BorderColor;
            a.BorderColor3DBase = b.BorderColor3DBase;
            a.Cursor = b.Cursor;
            a.FontData.Bold = b.FontData.Bold;
            a.FontData.Italic = b.FontData.Italic;
            a.FontData.Name = b.FontData.Name;
            a.FontData.SizeInPoints = b.FontData.SizeInPoints;
            a.FontData.Strikeout = b.FontData.Strikeout;
            a.FontData.Underline = b.FontData.Underline;
            a.ForeColor = b.ForeColor;
            a.ForeColorDisabled = b.ForeColorDisabled;
            a.ForegroundAlpha = b.ForegroundAlpha;
            a.Image = b.Image;
            a.ImageAlpha = b.ImageAlpha;
            a.ImageBackground = b.ImageBackground;
            a.ImageBackgroundAlpha = b.ImageBackgroundAlpha;
            a.ImageBackgroundDisabled = b.ImageBackgroundDisabled;
            a.ImageBackgroundOrigin = b.ImageBackgroundOrigin;
            a.ImageBackgroundStretchMargins = b.ImageBackgroundStretchMargins;
            a.ImageBackgroundStyle = b.ImageBackgroundStyle;
            a.ImageHAlign = b.ImageHAlign;
            a.ImageVAlign = b.ImageVAlign;
            a.TextHAlign = b.TextHAlign;
            a.TextTrimming = b.TextTrimming;
            a.TextVAlign = b.TextVAlign;
            a.ThemedElementAlpha = b.ThemedElementAlpha;
        }


        void ColorsChanged_Global(object sender, EventArgs e)
        {
            UpdateColors();
        }

        private void Form_Main2_Load(object sender, EventArgs e)
        {
            StyleManager.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream("Idera.SQLcompliance.Application.GUI.Styles.Office2007Black.isl"));
            _ribbonTabView.UpdateColors();
            _adminRibbonView.UpdateColors();
            _reportView.UpdateColors();
            _reportCategoryView.UpdateColors();
        }

        private void MenuFlagChanged_View(Object sender, MenuFlagChangedEventArgs e)
        {
            UpdateToolbar();
        }

        private void UpdateToolbar()
        {
            IMenuFlags flag = _activeControl;

            if (_activeTree != null)
            {
                if (_activeTree.Focused)
                {
                    flag = (SQLcomplianceTreeNode)_activeTree.SelectedNode;
                }
            }
            _toolbarManager.Tools["fileNew"].SharedProps.Enabled = Globals.isAdmin;
            if (_activeControl == null)
                return;
            _toolbarManager.Tools["viewRefresh"].SharedProps.Enabled = _activeControl.GetMenuFlag(CMMenuItem.Refresh);
            _toolbarManager.Tools["editRemove"].SharedProps.Enabled = _activeControl.GetMenuFlag(CMMenuItem.Delete) && Globals.isAdmin;
            _toolbarManager.Tools["editProperties"].SharedProps.Enabled = flag.GetMenuFlag(CMMenuItem.Properties);
            _toolbarManager.Tools["filePrint"].SharedProps.Enabled = _activeControl.GetMenuFlag(CMMenuItem.Print);
            _toolbarManager.Tools["helpWindow"].SharedProps.Enabled = _activeControl.GetMenuFlag(CMMenuItem.ShowHelp);
        }

        public void ShowConsoleTree(bool on)
        {
            _splitContainer.Panel1Collapsed = !on;
        }

        public void ShowToolbar(bool on)
        {
            _toolbarManager.Toolbars["_mainToolbar"].Visible = on;
            Settings.Default.ViewToolbar = on;
        }

        public void ShowAgentProperties(ServerRecord server)
        {
            // read current server record
            server.Connection = Globals.Repository.Connection;
            if (!server.Read(server.Instance))
            {
                MessageBox.Show(String.Format(UIConstants.Error_LoadingServerProperties,
                                              ServerRecord.GetLastError()),
                                Text,
                                MessageBoxButtons.OKCancel,
                                MessageBoxIcon.Error);
                return;
            }

            Form_AgentProperties frm = new Form_AgentProperties(server);
            if (frm.ShowDialog() == DialogResult.OK)
            {
            }
        }

        public void ShowServerProperties(ServerRecord server)
        {
            ShowServerProperties(server, Form_ServerProperties.Context.General);
        }

        public void ShowServerProperties(ServerRecord server, Form_ServerProperties.Context context)
        {
            // read current server record
            server.Connection = Globals.Repository.Connection;
            if (!server.Read(server.Instance))
            {
                MessageBox.Show(String.Format(UIConstants.Error_LoadingServerProperties,
                                              ServerRecord.GetLastError()),
                                Text,
                                MessageBoxButtons.OKCancel,
                                MessageBoxIcon.Error);
            }
            else
            {
                Form_ServerProperties frm = new Form_ServerProperties(server);
                frm.SetContext(context);
                DialogResult dr = frm.ShowDialog(this);
                // These are different because they are stored in different tables
                //  and have different ramifications.  Thresholds are not used anywhere
                //  but the GUI currently, so are not considered part of an audit snapshot
                if (frm.ThresholdsDirty)
                {
                    List<ReportCardRecord> reportCards = ReportCardRecord.GetReportCardEntries(Globals.Repository.Connection);
                    foreach (ReportCardRecord reportCard in reportCards)
                    {
                        if (_statistics.ContainsKey((StatsCategory)reportCard.StatisticId))
                        {
                            EnterpriseStatistics entStats = _statistics[(StatsCategory)reportCard.StatisticId];
                            ServerStatistics serverStats = entStats.GetServerStatistics(reportCard.SrvId);
                            if (serverStats != null)
                            {
                                serverStats.Threshold = reportCard;
                            }
                        }
                    }
                }
                if (dr == DialogResult.OK || frm.ThresholdsDirty)
                {
                    RaiseServerModified(new ServerEventArgs(frm.newServer));
                }
            }
        }

        public void ShowDatabaseProperties(DatabaseRecord database)
        {
            ShowDatabaseProperties(database, Form_DatabaseProperties.Context.General);
        }

        public void ShowDatabaseProperties(DatabaseRecord database, Form_DatabaseProperties.Context context)
        {
            // read current database record
            ServerRecord server = new ServerRecord();
            server.Connection = Globals.Repository.Connection;
            server.Read(database.SrvId);
            database.Connection = Globals.Repository.Connection;
            if (!database.Read(database.DbId))
            {
                var error = DatabaseRecord.GetLastError();
                if (string.IsNullOrEmpty(error))
                {
                    MessageBox.Show(this, "Some servers was modified outside the application. Server information will be refreshed.", "Server Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.RefreshServerRecords();
                }
                else
                {
                    MessageBox.Show(String.Format(UIConstants.Error_LoadingDatabaseProperties,
                                                  DatabaseRecord.GetLastError()),
                                    Text,
                                    MessageBoxButtons.OKCancel,
                                    MessageBoxIcon.Error);
                }
            }
            else
            {
                Form_DatabaseProperties frm = new Form_DatabaseProperties(server, database);
                frm.SetContext(context);
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    RaiseDatabaseModified(new DatabaseEventArgs(frm._db));
                    foreach (DatabaseRecord dbRec in frm.AlwaysOnDBList)
                    {
                        RaiseDatabaseModified(new DatabaseEventArgs(dbRec));
                    }
                }
            }
        }

        public void CheckAgent(ServerRecord server)
        {
            Cursor = Cursors.WaitCursor;

            try
            {
                // ping agent
                AgentCommand agentCmd = GUIRemoteObjectsProvider.AgentCommand(server.AgentServer, server.AgentPort);
                agentCmd.Ping();

                ServerRecord.UpdateLastAgentContact(server.Instance);

                MessageBox.Show("A SQLcompliance Agent is active on computer '" +
                                 server.AgentServer +
                                 "'.",
                                 "Check SQLcompliance Agent");
            }
            catch (Exception)
            {
                if (server.isClustered)
                {
                    MessageBox.Show("The SQLcompliance Agent on computer '" +
                                    server.AgentServer +
                                    "' failed to respond. The SQLcompliance Agent service may not be running or may be inaccessible.");
                }
                else
                {
                    DialogResult choice = MessageBox.Show("The SQLcompliance Agent on computer '" +
                                                          server.AgentServer +
                                                          "' failed to respond. The SQLcompliance Agent service may not be running.\n\nDo you want to attempt to start the SQLcompliance Agent service?",
                                                          "Check SQLcompliance Agent",
                                                          MessageBoxButtons.YesNo);
                    if (choice == DialogResult.Yes)
                    {
                        StartAgentService(server.AgentServer);
                    }
                }
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        public void StartAgentService(string agentServer)
        {
            Cursor = Cursors.WaitCursor;

            Form_ServiceProgress frm = new Form_ServiceProgress(agentServer, true);
            frm.ShowDialog();

            bool started = frm.Success;

            if (started)
            {
                MessageBox.Show(String.Format(UIConstants.Info_AgentStarted,
                                              agentServer),
                                UIConstants.Title_StartAgent);
            }
            else
            {
                MessageBox.Show(String.Format(UIConstants.Error_CantStartAgent,
                                              agentServer,
                                              frm.ErrorMessage),
                                UIConstants.Title_StartAgent);
            }
            Cursor = Cursors.Default;
        }

        public void ChangeAgentTraceDirectory(ServerRecord server)
        {
            // read current server record
            server.Connection = Globals.Repository.Connection;
            if (!server.Read(server.Instance))
            {
                MessageBox.Show(String.Format(UIConstants.Error_LoadingServerProperties,
                                              ServerRecord.GetLastError()),
                                Text,
                                MessageBoxButtons.OKCancel,
                                MessageBoxIcon.Error);
                return;
            }

            Form_AgentTraceDirectory frm = new Form_AgentTraceDirectory(server);
            if (frm.ShowDialog() == DialogResult.OK)
            {
            }
        }

        public void DoSnapshot(string instance)
        {
            Form_Snapshot frm = new Form_Snapshot(instance);
            frm.ShowDialog();
        }

        public void UpdateNow(string instance, string agentVersion = null)
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                AgentManager manager = GUIRemoteObjectsProvider.AgentManager();

                manager.UpdateAuditConfiguration(instance, agentVersion);
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

        // Added method to update audit setting to make XE default auditing after upgrade 5.8 - SQLCM-6206
        public void XEDefaultUpdateNow(object sender, DoWorkEventArgs e)
        {
            List<string> serverList = new List<string>();
            serverList = ServerRecord.GetServersForDefaultXE(Globals.Repository.Connection);
            List<ServerRecord> serverRecords = new List<ServerRecord>();
            serverRecords = ServerRecord.GetServers(Globals.Repository.Connection, true, false);

            foreach (var item in serverRecords)
            {
                if (serverList.Count > 0 && serverList.Contains(item.Instance) && item.AuditCaptureSQLXE)
                {
                    try
                    {
                        AgentManager manager = GUIRemoteObjectsProvider.AgentManager();
                        manager.UpdateAuditConfiguration(item.Instance);
                        ServerRecord.ResetServersForDefaultXE(Globals.Repository.Connection, item.Instance);
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                        "XEDefaultUpdateNow",
                                        ex);
                    }
                }
            }
        }
        public void ChangeTraceDirectory(ServerRecord record)
        {
            Form_AgentTraceDirectory frm = new Form_AgentTraceDirectory(record);
            frm.ShowDialog();
        }

        public void CheckIntegrity(string targetInstance)
        {
            Form_IntegrityCheck frm = new Form_IntegrityCheck(targetInstance);
            frm.ShowDialog();
        }

        public void NavigateToServerSummary(string serverName, int reportCardTab)
        {
            _explorerBar.SelectedGroup = _explorerBar.Groups[0];
            SQLcomplianceTreeNode node = ServerTreeNodeHelper.FindServerNode(serverName, _treeServers.Nodes[0].Nodes);
            if (node != null)
                _treeServers.SelectedNode = node;
            _ribbonTabView.ShowTab(RibbonView.Tabs.ServerSummary, reportCardTab);
        }

        public void NavigateToAdminNode(AdminRibbonView.Tabs tab, bool notifyRibbon)
        {
            if (!notifyRibbon)
                _internalUpdate++;
            try
            {
                switch (tab)
                {
                    case AdminRibbonView.Tabs.RegisteredServers:
                        _treeAdmin.SelectedNode = _treeAdmin.Nodes[0];
                        break;
                    case AdminRibbonView.Tabs.AlertRules:
                        _treeAdmin.SelectedNode = _treeAdmin.Nodes[1];
                        break;
                    case AdminRibbonView.Tabs.EventFilters:
                        _treeAdmin.SelectedNode = _treeAdmin.Nodes[2];
                        break;
                    case AdminRibbonView.Tabs.SqlLogins:
                        _treeAdmin.SelectedNode = _treeAdmin.Nodes[3];
                        break;
                    case AdminRibbonView.Tabs.ActivityLog:
                        _treeAdmin.SelectedNode = _treeAdmin.Nodes[4];
                        break;
                    case AdminRibbonView.Tabs.ChangeLog:
                        _treeAdmin.SelectedNode = _treeAdmin.Nodes[5];
                        break;
                    //start sqlcm 5.6 - 5467
                    case AdminRibbonView.Tabs.DefaultSettings:
                        _treeAdmin.SelectedNode = _treeAdmin.Nodes[6];
                        break;
                        //end sqlcm 5.6 - 5467
                }
            }
            finally
            {
                if (!notifyRibbon)
                    _internalUpdate--;
            }
        }

        public void NavigateToView(ConsoleViews view)
        {
            switch (view)
            {
                case ConsoleViews.EnterpriseSummary:
                    _explorerBar.SelectedGroup = _explorerBar.Groups[0];
                    break;
                case ConsoleViews.EnterpriseAlerts:
                    _explorerBar.SelectedGroup = _explorerBar.Groups[0];
                    break;
                case ConsoleViews.ServerSummary:
                    _explorerBar.SelectedGroup = _explorerBar.Groups[0];
                    break;
                case ConsoleViews.ServerAlerts:
                    _explorerBar.SelectedGroup = _explorerBar.Groups[0];
                    break;
                case ConsoleViews.ServerEvents:
                    _explorerBar.SelectedGroup = _explorerBar.Groups[0];
                    break;
                case ConsoleViews.ServerArchive:
                    _explorerBar.SelectedGroup = _explorerBar.Groups[0];
                    break;
                case ConsoleViews.ServerChangeLog:
                    _explorerBar.SelectedGroup = _explorerBar.Groups[0];
                    break;
                case ConsoleViews.DatabaseSummary:
                    _explorerBar.SelectedGroup = _explorerBar.Groups[0];
                    break;
                case ConsoleViews.DatabaseEvents:
                    _explorerBar.SelectedGroup = _explorerBar.Groups[0];
                    break;
                case ConsoleViews.DatabaseArchive:
                    _explorerBar.SelectedGroup = _explorerBar.Groups[0];
                    break;
                case ConsoleViews.AdminRegisteredServers:
                    _explorerBar.SelectedGroup = _explorerBar.Groups[2];
                    _treeAdmin.SelectedNode = _treeAdmin.Nodes[0];
                    break;
                case ConsoleViews.AdminAlertRules:
                    _explorerBar.SelectedGroup = _explorerBar.Groups[2];
                    _treeAdmin.SelectedNode = _treeAdmin.Nodes[1];
                    break;
                case ConsoleViews.AdminEventFilters:
                    _explorerBar.SelectedGroup = _explorerBar.Groups[2];
                    _treeAdmin.SelectedNode = _treeAdmin.Nodes[2];
                    break;
                case ConsoleViews.AdminLogins:
                    _explorerBar.SelectedGroup = _explorerBar.Groups[2];
                    _treeAdmin.SelectedNode = _treeAdmin.Nodes[3];
                    break;
                case ConsoleViews.AdminActivityLog:
                    _explorerBar.SelectedGroup = _explorerBar.Groups[2];
                    _treeAdmin.SelectedNode = _treeAdmin.Nodes[4];
                    break;
                case ConsoleViews.AdminChangeLog:
                    _explorerBar.SelectedGroup = _explorerBar.Groups[2];
                    _treeAdmin.SelectedNode = _treeAdmin.Nodes[5];
                    break;
                //start sqlcm 5.6 - 5467
                case ConsoleViews.AdminDefaultSettings:
                    _explorerBar.SelectedGroup = _explorerBar.Groups[2];
                    _treeAdmin.SelectedNode = _treeAdmin.Nodes[6];
                    break;
                    //end sqlcm 5.6 - 5467
            }
        }

        #region Server Actions

        internal bool RemoveServerAction(ServerRecord srv)
        {
            if (Server.RemoveServer(srv))
            {
                RaiseServerRemoved(new ServerEventArgs(srv));
                //start sqlcm 5.6 - 5467
                _adminRibbonView.RefreshDefaultAuditServersDatabases();
                //end sqlcm 5.6 - 5467
                return true;
            }
            return false;
        }

        internal bool ApplyAuditTemplate(DatabaseRecord database, ServerRecord server)
        {
            List<DatabaseRecord> dbs;


            if (server == null)
            {
                server = new ServerRecord();
                server.Connection = Globals.Repository.Connection;
                server.Read(database.SrvId);
            }
            RefreshAction();
            Form_RegisterWizard registerWizard = new Form_RegisterWizard(Form_RegisterWizard.StartPage.RegulationGuidelineInfo, server);

            if (database != null)
            {
                dbs = new List<DatabaseRecord>();
                dbs.Add(database);
                registerWizard.DatabaseList = dbs;
            }
            else if (server != null)
            {
                dbs = DatabaseRecord.GetDatabases(Globals.Repository.Connection, server.SrvId);
                registerWizard.DatabaseList = dbs;
            }

            //Fix for 5241
            CoreHelpers.currentServerId = server.SrvId;
            if (database != null)
            {
                CoreHelpers.currentDatabaseId = database.DbId;
            }

            if (registerWizard.ShowDialog(this) == DialogResult.OK)
            {
                server.Connection = Globals.Repository.Connection;
                server.Read(server.Instance); //re-read the server from the repository
                RefreshAction();
            }
            return true;
        }

        public void AddServerAction()
        {
            // not required in SQLcm 4.5
            //Form_AlertPermissionsCheck frmPermissionsCheck = new Form_AlertPermissionsCheck();
            //if (frmPermissionsCheck.ShowDialog(this) == DialogResult.No)
            //    return;

            Form_RegisterWizard dlg = new Form_RegisterWizard(Form_RegisterWizard.StartPage.Server, null);
            if (DialogResult.OK == dlg.ShowDialog(this))
            {
                RaiseServerAdded(new ServerEventArgs(dlg.Server));
                if (dlg.ReplicaServerList != null)
                {
                    foreach (ServerRecord srvRec in dlg.ReplicaServerList)
                        RaiseServerAdded(new ServerEventArgs(srvRec));
                }

                foreach (DatabaseRecord db in dlg.DatabaseList)
                    RaiseDatabaseAdded(new DatabaseEventArgs(db));

                foreach (DatabaseRecord db in dlg.ReplicaDatabasesList)
                    RaiseDatabaseAdded(new DatabaseEventArgs(db));
                //start sqlcm 5.6 - 5467
                _adminRibbonView.RefreshDefaultAuditServersDatabases();
                //end sqlcm 5.6 - 5467
            }
        }

        public void EnableServersAction(List<ServerRecord> servers)
        {
            foreach (ServerRecord server in servers)
                EnableServerAction(server);
        }

        //-------------------------------------------------------------------------
        // EnableServerAction
        //-------------------------------------------------------------------------
        public void EnableServerAction(ServerRecord srv)
        {
            srv.Connection = Globals.Repository.Connection;
            if (srv.EnableServer(true))
            {
                // Register change to server and perform audit log				      
                ServerUpdate.RegisterChange(srv.SrvId, LogType.EnableServer, srv.Instance);
                RaiseServerModified(new ServerEventArgs(srv));
            }
            else
            {
                ErrorMessage.Show(this.Text,
                                   UIConstants.Error_CantChangeAuditingStatus,
                                   ServerRecord.GetLastError());
            }
        }

        public void DisableServersAction(List<ServerRecord> servers)
        {
            if (DialogResult.OK != MessageBox.Show(UIConstants.Warning_DisableAuditedServers,
                                                   UIConstants.Title_DisableAuditedServers,
                                                   MessageBoxButtons.OKCancel,
                                                   MessageBoxIcon.Warning))
                return;

            foreach (ServerRecord srv in servers)
                DisableServerAction(srv, false);
        }

        public void DisableServerAction(ServerRecord srv)
        {
            DisableServerAction(srv, true);
        }

        private void DisableServerAction(ServerRecord srv, bool promptUser)
        {
            if (promptUser)
            {
                if (DialogResult.OK != MessageBox.Show(UIConstants.Warning_DisableAuditedServers,
                                                       UIConstants.Title_DisableAuditedServers,
                                                       MessageBoxButtons.OKCancel,
                                                       MessageBoxIcon.Warning))
                    return;
            }
            srv.Connection = Globals.Repository.Connection;
            if (srv.EnableServer(false))
            {
                // Register change to server and perform audit log				      
                ServerUpdate.RegisterChange(srv.SrvId, LogType.DisableServer, srv.Instance);
                RaiseServerModified(new ServerEventArgs(srv));
            }
            else
            {
                ErrorMessage.Show(this.Text,
                                   UIConstants.Error_CantChangeAuditingStatus,
                                   ServerRecord.GetLastError());
            }
        }

        #endregion Server Actions

        #region Database Actions

        public void AddDatabaseAction(ServerRecord server)
        {
            Form_RegisterWizard frm = new Form_RegisterWizard(Form_RegisterWizard.StartPage.Database, server);

            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                foreach (DatabaseRecord db in frm.DatabaseList)
                    RaiseDatabaseAdded(new DatabaseEventArgs(db));

                //For AlwaysOn additions
                if (frm.ReplicaServerList != null)
                {
                    foreach (ServerRecord srvRec in frm.ReplicaServerList)
                        RaiseServerAdded(new ServerEventArgs(srvRec));
                }
                foreach (DatabaseRecord db in frm.ReplicaDatabasesList)
                    RaiseDatabaseAdded(new DatabaseEventArgs(db));
                //For AlwaysOn additions

                //SQLcm 5.6 (Aakash Prakash) - Fix for 4967
                //Include these lines if the audit DML or audit Select is enabled for a database by default
                System.Data.SqlClient.SqlConnection conn = Globals.Repository.Connection;
                ServerRecord.UpdateCountDatabasesAuditingAllObjects(conn, server.SrvId,
                    ServerRecord.GetCountAuditingDMLEnabled(conn, server.SrvId));

                server.Connection = Globals.Repository.Connection;
                server.Read(server.Instance); //re-read the server from the repository
                //start sqlcm 5.6 - 5467
                _adminRibbonView.RefreshDefaultAuditServersDatabases();
                //end sqlcm 5.6 - 5467
            }
        }

        internal bool RemoveDatabasesAction(List<DatabaseRecord> databases)
        {
            //SQLCM-5.6 SQLCM-3773 Removing databases from the Administration Pane.

            //Checking if databases is empty 

            if (databases == null)
                return false;

            foreach (DatabaseRecord record in databases)
            {
                // try catch added so that user can be shown exception if any problem occurs in removing database
                try
                {
                    RemoveDatabaseAction(record, false);
                }
                catch (Exception e)
                {
                    ErrorLog.Instance.Write("Unable to remove database..", e);
                    return false;
                }
            }

            return true;
        }

        internal bool RemoveDatabaseAction(DatabaseRecord dbItem)
        {
            return RemoveDatabaseAction(dbItem, true);
        }

        private bool RemoveDatabaseAction(DatabaseRecord dbItem, bool promptUser)
        {
            // warning
            if (promptUser)
            {
                if (DialogResult.OK != MessageBox.Show(UIConstants.Warning_RemoveAuditedDatabases,
                                                         UIConstants.Title_RemoveAuditedDatabases,
                                                         MessageBoxButtons.OKCancel,
                                                         MessageBoxIcon.Warning))
                    return false;
            }

            this.Cursor = Cursors.WaitCursor;

            try
            {
                dbItem.Connection = Globals.Repository.Connection;
                DatabaseRecord.UpdateCountDatabasesAuditingAllObjects(dbItem.Connection, dbItem.SrvId, dbItem.DbId, false); //SQLcm 5.6 (Aakash Prakash) - Fix for 4967
                if (Server.RemoveDatabase(dbItem))
                {
                    DeleteUserForDatabase(dbItem);
                    RaiseDatabaseRemoved(new DatabaseEventArgs(dbItem));
                    //start sqlcm 5.6 - 5467
                    _adminRibbonView.RefreshDefaultAuditServersDatabases();
                    //end sqlcm 5.6 - 5467
                    return true;
                }
                else
                {
                    DatabaseRecord.UpdateCountDatabasesAuditingAllObjects(dbItem.Connection, dbItem.SrvId, dbItem.DbId, true); //SQLcm 5.6 (Aakash Prakash) - Fix for 4967
                }
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
            return false;
        }
        private void DeleteUserForDatabase(DatabaseRecord db)
        {
            string querySQL = db.GetDeleteSQLForUser(db.DbId);
            string errorMsg;
            try
            {
                if (!db.DeleteUser(querySQL))
                {
                    errorMsg = DatabaseRecord.GetLastError();
                    throw (new Exception(errorMsg));
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }
        }

        public void EnableDatabasesAction(List<DatabaseRecord> databases)
        {
            foreach (DatabaseRecord database in databases)
                EnableDatabaseAction(database);
        }

        public void EnableDatabaseAction(DatabaseRecord database)
        {
            database.Connection = Globals.Repository.Connection;
            if (database.Enable(true))
            {
                // Register change to server and perform audit log				      
                ServerUpdate.RegisterChange(database.SrvId, LogType.EnableDatabase, database.SrvInstance, database.Name);
                RaiseDatabaseModified(new DatabaseEventArgs(database));
            }
            else
            {
                ErrorMessage.Show(this.Text,
                                  UIConstants.Error_CantChangeAuditingStatus,
                                  DatabaseRecord.GetLastError());
            }
        }

        public void DisableDatabasesAction(List<DatabaseRecord> databases)
        {
            if (DialogResult.OK != MessageBox.Show(UIConstants.Warning_DisableAuditedDatabases,
                                               UIConstants.Title_DisableAuditedDatabases,
                                               MessageBoxButtons.OKCancel,
                                               MessageBoxIcon.Warning))
                return;

            foreach (DatabaseRecord database in databases)
                DisableDatabaseAction(database, false);
        }

        public void DisableDatabaseAction(DatabaseRecord database)
        {
            DisableDatabaseAction(database, true);
        }

        private void DisableDatabaseAction(DatabaseRecord database, bool promptUser)
        {
            if (promptUser)
            {
                if (DialogResult.OK != MessageBox.Show(UIConstants.Warning_DisableAuditedDatabases,
                                                   UIConstants.Title_DisableAuditedDatabases,
                                                   MessageBoxButtons.OKCancel,
                                                   MessageBoxIcon.Warning))
                    return;
            }
            database.Connection = Globals.Repository.Connection;
            if (database.Enable(false))
            {
                // Register change to server and perform audit log				      
                ServerUpdate.RegisterChange(database.SrvId, LogType.DisableDatabase, database.SrvInstance, database.Name);
                RaiseDatabaseModified(new DatabaseEventArgs(database));
            }
            else
            {
                ErrorMessage.Show(this.Text,
                                  UIConstants.Error_CantChangeAuditingStatus,
                                  DatabaseRecord.GetLastError());
            }
        }

        #endregion Database Actions

        //-------------------------------------------------------------------------
        // RemoveArchiveAction
        //-------------------------------------------------------------------------
        public void DetachArchiveAction(ArchiveRecord archive)
        {
            // warning
            DialogResult choice;

            choice = MessageBox.Show(UIConstants.Warning_RemoveArchive,
                                      UIConstants.Title_RemoveArchive,
                                      MessageBoxButtons.YesNo,
                                      MessageBoxIcon.Warning);
            if (choice != DialogResult.Yes)
            {
                return;
            }

            if (SQLRepository.RemoveArchiveDatabase(archive.DatabaseName))
            {
                // remove or refresh instance node as appropriate
                ServerRecord server = new ServerRecord();
                server.Connection = Globals.Repository.Connection;
                server.Read(archive.Instance);
                int archiveCount = server.CountLoadedArchives(Globals.Repository.Connection);

                if ((!server.IsAuditedServer) && (0 == archiveCount))
                {
                    // kill server record
                    ServerRecord.DeleteServerRecord(Globals.Repository.Connection, server.Instance);
                    RaiseServerRemoved(new ServerEventArgs(server));
                }

                // Build snapshot
                string snapshot = String.Format("Detach archive database: {0}\r\n\r\n", archive.DatabaseName);

                LogRecord.WriteLog(Globals.Repository.Connection,
                                    LogType.DetachArchive,
                                    archive.Instance,
                                    snapshot);
                RaiseArchiveDetached(new ArchiveEventArgs(archive));
            }
        }



        public void AttachArchiveAction()
        {
            Form_ArchiveImport frm = new Form_ArchiveImport();
            if (DialogResult.OK == frm.ShowDialog())
            {
                if (frm.serverAdded)
                    RaiseServerAdded(new ServerEventArgs(frm.serverRecord));
                RaiseArchiveAttached(new ArchiveEventArgs(frm.m_archive));
            }
        }

        private void RaiseServerAdded(ServerEventArgs args)
        {
            FetchGraphData();
            AddServerNode(args.Server);
            EventHandler<ServerEventArgs> temp = ServerAdded;
            if (temp != null)
                temp(this, args);
        }

        private void RaiseServerRemoved(ServerEventArgs args)
        {
            FetchGraphData();
            ServerRecord server = new ServerRecord();
            server.Connection = Globals.Repository.Connection;
            // If it exists, that means we have archives remaining
            //  This happens when adding/removing a server with archives.
            if (server.Read(args.Server.SrvId))
                RefreshServerNode(args.Server);
            else
                RemoveServerNode(args.Server);
            EventHandler<ServerEventArgs> temp = ServerRemoved;
            if (temp != null)
                temp(this, args);
        }

        private void RaiseServerModified(ServerEventArgs args)
        {
            RefreshServerNode(args.Server);
            EventHandler<ServerEventArgs> temp = ServerModified;
            if (temp != null)
                temp(this, args);
        }

        private void RaiseArchiveAttached(ArchiveEventArgs args)
        {
            EventHandler<ArchiveEventArgs> temp = ArchiveAttached;
            if (temp != null)
                temp(this, args);
        }

        private void RaiseArchiveDetached(ArchiveEventArgs args)
        {
            EventHandler<ArchiveEventArgs> temp = ArchiveDetached;
            if (temp != null)
                temp(this, args);
        }

        private void RaiseDatabaseAdded(DatabaseEventArgs args)
        {
            AddDatabaseNode(args.Database);
            EventHandler<DatabaseEventArgs> temp = DatabaseAdded;
            if (temp != null)
                temp(this, args);
        }

        private void RaiseDatabaseRemoved(DatabaseEventArgs args)
        {
            RemoveDatabaseNode(args.Database);
            EventHandler<DatabaseEventArgs> temp = DatabaseRemoved;
            if (temp != null)
                temp(this, args);
        }

        private void RaiseDatabaseModified(DatabaseEventArgs args)
        {
            RefreshDatabaseNode(args.Database);
            EventHandler<DatabaseEventArgs> temp = DatabaseModified;
            if (temp != null)
                temp(this, args);
        }

        private void RaiseConnectionChanged(ConnectionChangedEventArgs args)
        {
            FetchGraphData();
            EventHandler<ConnectionChangedEventArgs> temp = ConnectionChanged;
            if (temp != null)
                temp(this, args);
        }

        private void FetchGraphData()
        {
            _statistics = new Dictionary<StatsCategory, EnterpriseStatistics>();
            List<ServerRecord> servers;
            servers = ServerRecord.GetServers(Globals.Repository.Connection, true);
            DateTime endDate = DateTime.UtcNow;
            DateTime startDate = endDate.AddDays(-31);
            EnterpriseStatistics stats;

            try
            {
                stats = StatsExtractor.GetEnterpriseStatistics(Globals.Repository.Connection, StatsCategory.PrivUserEvents, servers, startDate, endDate);
                stats.Name = "Privileged User Activity";
                _statistics.Add(StatsCategory.PrivUserEvents, stats);

                stats = StatsExtractor.GetEnterpriseStatistics(Globals.Repository.Connection, StatsCategory.Alerts, servers, startDate, endDate);
                stats.Name = "Event Alert Activity";
                _statistics.Add(StatsCategory.Alerts, stats);

                stats = StatsExtractor.GetEnterpriseStatistics(Globals.Repository.Connection, StatsCategory.FailedLogin, servers, startDate, endDate);
                stats.Name = "Failed Login Activity";
                _statistics.Add(StatsCategory.FailedLogin, stats);

                stats = StatsExtractor.GetEnterpriseStatistics(Globals.Repository.Connection, StatsCategory.DDL, servers, startDate, endDate);
                stats.Name = "DDL Activity";
                _statistics.Add(StatsCategory.DDL, stats);

                stats = StatsExtractor.GetEnterpriseStatistics(Globals.Repository.Connection, StatsCategory.Security, servers, startDate, endDate);
                stats.Name = "Security Activity";
                _statistics.Add(StatsCategory.Security, stats);

                stats = StatsExtractor.GetEnterpriseStatistics(Globals.Repository.Connection, StatsCategory.EventProcessed, servers, startDate, endDate);
                stats.Name = "Overall Activity";
                _statistics.Add(StatsCategory.EventProcessed, stats);

                //start sqlcm 5.6 - 5363
                stats = StatsExtractor.GetEnterpriseStatistics(Globals.Repository.Connection, StatsCategory.Logins, servers, startDate, endDate);
                stats.Name = "Logins";
                _statistics.Add(StatsCategory.Logins, stats);

                stats = StatsExtractor.GetEnterpriseStatistics(Globals.Repository.Connection, StatsCategory.Logout, servers, startDate, endDate);
                stats.Name = "Logouts";
                _statistics.Add(StatsCategory.Logout, stats);
                //end sqlcm 5.6 - 5363
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write("Unable to fetch statistics data.", e);
            }
        }

        internal bool StatsAvailable()
        {
            //sqlcm 5.6 -5363 (changed 6 to 8)
            return (_statistics.Count == 8);
        }

        internal EnterpriseStatistics GetEnterpriseStatistics(StatsCategory category)
        {
            EnterpriseStatistics retVal = _statistics[category];

            UpdateStatistics(retVal);

            return retVal;
        }

        internal ServerStatistics GetServerStatistics(int srvId, StatsCategory category)
        {
            EnterpriseStatistics ent = GetEnterpriseStatistics(category);
            return ent.GetServerStatistics(srvId);
        }

        internal void UpdateStatistics(EnterpriseStatistics stats)
        {
            DateTime endDate = DateTime.UtcNow;
            DateTime startDate = endDate.AddDays(-31);

            List<ServerRecord> servers;
            servers = ServerRecord.GetServers(Globals.Repository.Connection, true);

            // Add a minute to avoid a key collision on the endDate data point
            StatsExtractor.UpdateEnterpriseStatistics(Globals.Repository.Connection, stats, servers, startDate, endDate);
        }


        private SQLcomplianceTreeNode FindNode(string name, TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                if (String.Compare(node.Text, name) == 0)
                    return node as SQLcomplianceTreeNode;
            }
            return null;
        }

        private string GetServerNodeName(ServerRecord server)
        {
            string serverName = server.Instance;

            try
            {
                AgentManager manager = GUIRemoteObjectsProvider.AgentManager();
                var readOnlySecondaryReplicaList = manager.GetAllReplicaNodeInfoList(server.Instance);

                foreach (var replicaInfo in readOnlySecondaryReplicaList)
                {
                    if (replicaInfo.ReplicaServerName.ToLower() == server.Instance.ToLower() &&
                        replicaInfo.IsPrimary)
                    {
                        return string.Format("{0} (Primary)", serverName);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(
                    ErrorLog.Level.Always,
                    new Exception("Failed to get server node name (for Primary role in always on group). This is required only for servers who has always on databases", ex),
                    ErrorLog.Severity.Warning);
            }

            return serverName;
        }

        private SQLcomplianceTreeNode AddServerNode(ServerRecord server)
        {
            SQLcomplianceTreeNode root = _treeServers.Nodes[0] as SQLcomplianceTreeNode;
            if (root == null)
                return null;

            SQLcomplianceTreeNode newNode = ServerTreeNodeHelper.FindServerNode(server.Instance, root.Nodes);
            if (newNode != null)
            {
                RefreshServerNode(server);
            }
            else
            {
                int imgIndex;
                if (server.IsAuditedServer)
                    imgIndex = server.IsEnabled ? (int)AppIcons.Img16.Server : (int)AppIcons.Img16.ServerDisabled;
                else
                    imgIndex = (int)AppIcons.Img16.ReportServer;

                /*Commented as per Robert's suggestion*/
                //var serverName = GetServerNodeName(server);

                newNode = new SQLcomplianceTreeNode(server.Instance, imgIndex, imgIndex, CMNodeType.Server);
                newNode.Tag = server;
                SetServerNodeMenuFlags(newNode, server);
                root.Nodes.Add(newNode);
            }
            root.Expand();

            return newNode;
        }

        private void SetServerNodeMenuFlags(SQLcomplianceTreeNode node, ServerRecord server)
        {
            node.SetMenuFlag(CMMenuItem.Refresh);
            node.SetMenuFlag(CMMenuItem.Properties);

            node.SetMenuFlag(CMMenuItem.AttachArchive, Globals.isAdmin);
            node.SetMenuFlag(CMMenuItem.NewServer, Globals.isAdmin);

            node.SetMenuFlag(CMMenuItem.Delete, Globals.isAdmin);
            node.SetMenuFlag(CMMenuItem.AgentProperties, Globals.isAdmin);
            node.SetMenuFlag(CMMenuItem.NewDatabase, Globals.isAdmin);

            node.SetMenuFlag(CMMenuItem.DisableAuditing, server.IsEnabled && Globals.isAdmin);
            node.SetMenuFlag(CMMenuItem.EnableAuditing, !server.IsEnabled && Globals.isAdmin);
            node.SetMenuFlag(CMMenuItem.UpdateAuditSettings, server.IsEnabled && Globals.isAdmin);
            node.SetMenuFlag(CMMenuItem.ForceCollection, server.IsEnabled && Globals.isAdmin);
        }

        private SQLcomplianceTreeNode AddDatabaseNode(DatabaseRecord database)
        {
            SQLcomplianceTreeNode root = _treeServers.Nodes[0] as SQLcomplianceTreeNode;
            if (root == null)
                return null;

            SQLcomplianceTreeNode parent = ServerTreeNodeHelper.FindServerNode(database.SrvInstance, root.Nodes);
            if (parent == null)
                return null;
            ServerRecord server = (ServerRecord)parent.Tag;

            int imgIndex = server.IsEnabled && database.IsEnabled ?
               (int)AppIcons.Img16.Database : (int)AppIcons.Img16.DatabaseDisabled;
            SQLcomplianceTreeNode node = new SQLcomplianceTreeNode(database.Name, imgIndex, imgIndex, CMNodeType.Database);
            SetDatabaseNodeMenuFlags(node, database);
            node.Tag = database;
            parent.Nodes.Add(node);
            return node;
        }

        private void SetDatabaseNodeMenuFlags(SQLcomplianceTreeNode node, DatabaseRecord database)
        {
            node.SetMenuFlag(CMMenuItem.Refresh);
            node.SetMenuFlag(CMMenuItem.Properties);
            node.SetMenuFlag(CMMenuItem.Delete);
            node.SetMenuFlag(CMMenuItem.EnableAuditing, !database.IsEnabled && Globals.isAdmin);
            node.SetMenuFlag(CMMenuItem.DisableAuditing, database.IsEnabled && Globals.isAdmin);
            node.SetMenuFlag(CMMenuItem.NewDatabase, Globals.isAdmin);
        }


        //------------------------------------------------------------------
        // RefreshServerNode
        //------------------------------------------------------------------
        public void RefreshServerNode(ServerRecord server)
        {
            SQLcomplianceTreeNode root = _treeServers.Nodes[0] as SQLcomplianceTreeNode;
            if (root == null)
                return;

            SQLcomplianceTreeNode node = ServerTreeNodeHelper.FindServerNode(server.Instance, root.Nodes);
            if (node != null)
            {
                int imgIndex;
                if (server.IsAuditedServer)
                    imgIndex = server.IsEnabled ? (int)AppIcons.Img16.Server : (int)AppIcons.Img16.ServerDisabled;
                else
                {
                    imgIndex = (int)AppIcons.Img16.ReportServer;
                    node.Nodes.Clear();
                }
                node.ImageIndex = imgIndex;
                node.SelectedImageIndex = imgIndex;
                node.Tag = server;
                foreach (TreeNode childNode in node.Nodes)
                {
                    DatabaseRecord db = childNode.Tag as DatabaseRecord;
                    childNode.ImageIndex = server.IsEnabled && db.IsEnabled ? (int)AppIcons.Img16.Database : (int)AppIcons.Img16.DatabaseDisabled;
                    childNode.SelectedImageIndex = childNode.ImageIndex;
                }
                SetServerNodeMenuFlags(node, server);
                if (node.IsSelected)
                {
                    _ribbonTabView.SetScope(server);
                    _ribbonTabView.RefreshView();
                }
            }
        }

        //------------------------------------------------------------------
        // RefreshDatabaseNode
        //------------------------------------------------------------------
        public void RefreshDatabaseNode(DatabaseRecord database)
        {
            SQLcomplianceTreeNode root = _treeServers.Nodes[0] as SQLcomplianceTreeNode;
            if (root == null)
                return;

            SQLcomplianceTreeNode parent = ServerTreeNodeHelper.FindServerNode(database.SrvInstance, root.Nodes);
            if (parent == null)
                return;
            ServerRecord server = (ServerRecord)parent.Tag;

            parent = FindNode(database.Name, parent.Nodes);
            if (parent != null)
            {
                parent.ImageIndex = server.IsEnabled && database.IsEnabled ?
                (int)AppIcons.Img16.Database : (int)AppIcons.Img16.DatabaseDisabled;
                parent.Tag = database;
                parent.SelectedImageIndex = parent.ImageIndex;
            }
            SetDatabaseNodeMenuFlags(parent, database);
        }

        public void RemoveServerNode(ServerRecord server)
        {
            SQLcomplianceTreeNode root = _treeServers.Nodes[0] as SQLcomplianceTreeNode;
            if (root == null)
                return;

            SQLcomplianceTreeNode node = ServerTreeNodeHelper.FindServerNode(server.Instance, root.Nodes);
            if (node != null)
            {
                if (node.IsSelected)
                    _treeServers.SelectedNode = root;
                node.Remove();
            }
        }

        public void RemoveDatabaseNode(DatabaseRecord database)
        {
            SQLcomplianceTreeNode root = _treeServers.Nodes[0] as SQLcomplianceTreeNode;
            if (root == null)
                return;

            SQLcomplianceTreeNode parent = ServerTreeNodeHelper.FindServerNode(database.SrvInstance, root.Nodes);
            if (parent == null)
                return;

            parent = FindNode(database.Name, parent.Nodes);
            if (parent != null)
            {
                if (parent.IsSelected)
                    _treeServers.SelectedNode = root;
                parent.Remove();
            }
        }

        internal void NewEventFilter()
        {
            NewEventFilter(null);
        }

        internal void NewEventFilter(EventFilter template)
        {
            Form_EventFilterWizard wizard = new Form_EventFilterWizard(template, _filteringConfig);

            if (wizard.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    if (!FiltersDal.InsertEventFilter(wizard.Filter, _filteringConfig.ConnectionString))
                    {
                        MessageBox.Show(this, "Unable to create the new event filter.", "Error");
                        return;
                    }
                    if (!wizard.Filter.IsValid)
                    {
                        if (!wizard.Filter.HasTargetInstances || wizard.Filter.HasConditions)
                            MessageBox.Show(this, CoreConstants.Exception_IncompleteEventFilter, "Invalid Event Filter",
                                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        else
                            MessageBox.Show(this, CoreConstants.Exception_InvalidEventFilter, "Invalid Event Filter",
                                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception e)
                {
                    ErrorLog.Instance.Write("Failed to create event filter", e, true);
                    MessageBox.Show(this, String.Format("Unable to create the new event filter.\r\nMessage: {0}", e.Message),
                                    "Error");
                    return;
                }

                if (_activeControl == _adminRibbonView &&
                   _adminRibbonView.GetActiveTab() == AdminRibbonView.Tabs.EventFilters)
                    _activeControl.RefreshView();

                UpdateEventFilters();
                string logString = String.Format("Name:  {0}\r\nDescription:  {1}\r\n\r\nFilter:  {2}",
                                                 wizard.Filter.Name, wizard.Filter.Description,
                                                 wizard.FilterText.Replace("\n", "\r\n"));
                LogRecord.WriteLog(Globals.Repository.Connection, LogType.EventFilterAdded, logString);
            }
        }

        public void UpdateEventFilters()
        {
            // check for collection service - cant uninstall if it is down or unreachable
            try
            {
                ServerManager srvManager = GUIRemoteObjectsProvider.ServerManager();
                srvManager.UpdateEventFilters();
            }
            catch (Exception)
            {
                // TODO:  Should we alert the user when we can't talk to the collection server?
            }
        }

        internal void NewAlertRule()
        {
            AlertRule rule = null;
            NewAlertRule(rule);
        }

        internal void NewAlertRule(AlertRule template)
        {
            Form_AlertRuleWizard wizard = new Form_AlertRuleWizard(template, _alertingConfig);

            if (wizard.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    if (!AlertingDal.InsertAlertRule(wizard.Rule, _alertingConfig.ConnectionString))
                    {
                        MessageBox.Show(this, "Unable to create the new alert rule.", "Error");
                        return;
                    }
                    if (!wizard.Rule.IsValid)
                    {
                        MessageBox.Show(this, CoreConstants.Exception_InvalidAlertRule, "Invalid Alert Rule",
                                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception e)
                {
                    ErrorLog.Instance.Write("Failed to create alert rule", e, true);
                    MessageBox.Show(this, String.Format("Unable to create the new alert rule.\r\nMessage: {0}", e.Message),
                                    "Error");
                    return;
                }

                if (_activeControl == _adminRibbonView &&
                   _adminRibbonView.GetActiveTab() == AdminRibbonView.Tabs.AlertRules)
                    _activeControl.RefreshView();

                UpdateAlertRules();
                //_alertRulesView.RefreshView();
                string logString = String.Format("Name:  {0}\r\nDescription:  {1}\r\n\r\nRule:  {2}",
                                                 wizard.Rule.Name, wizard.Rule.Description,
                                                 wizard.RuleText.Replace("\n", "\r\n"));
                LogRecord.WriteLog(Globals.Repository.Connection, LogType.AlertRuleAdded, logString);
            }
        }

        internal void NewAlertRule(StatusAlertRule rule)
        {
            Form_AddStatusAlert statusAlertWizard = new Form_AddStatusAlert(rule, AlertingDal.SelectStatusRuleNames(_alertingConfig.ConnectionString), !(rule == null));

            if (statusAlertWizard.ShowDialog(this) == DialogResult.OK)
            {
                //Add the alert rule to the repository
                try
                {
                    if (AlertingDal.InsertAlertRule(statusAlertWizard.StatusRule, _alertingConfig.ConnectionString) == false)
                    {
                        MessageBox.Show(this, "Unable to create the new status alert rule.", "Error");
                        return;
                    }
                }
                catch (Exception e)
                {
                    ErrorLog.Instance.Write("Failed to create alert rule", e, true);
                    MessageBox.Show(this, String.Format("Unable to create the new status alert rule.\r\nMessage: {0}", e.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                //Update the Alert Rules View
                if (_activeControl == _adminRibbonView &&
                   _adminRibbonView.GetActiveTab() == AdminRibbonView.Tabs.AlertRules)
                    _activeControl.RefreshView();

                UpdateAlertRules();
                string logString = String.Format("Name:  {0}\r\nDescription:  {1}\r\n\r\nRule:  {2}", statusAlertWizard.StatusRule.Name,
                                                                                                       statusAlertWizard.StatusRule.Description,
                                                                                                       statusAlertWizard.RuleText.Replace("\n", "\r\n"));
                LogRecord.WriteLog(Globals.Repository.Connection, LogType.AlertRuleAdded, logString);
            }
        }

        internal void NewAlertRule(DataAlertRule rule)
        {
            Form_AddDataAlert dataAlertWizard = new Form_AddDataAlert(rule, AlertingDal.SelectDataRuleNames(_alertingConfig.ConnectionString), !(rule == null));

            if (dataAlertWizard.ShowDialog(this) == DialogResult.OK)
            {
                //Add the alert rule to the repository
                try
                {
                    if (AlertingDal.InsertAlertRule(dataAlertWizard.DataRule, _alertingConfig.ConnectionString) == false)
                    {
                        MessageBox.Show(this, "Unable to create the new data alert rule.", "Error");
                        return;
                    }

                    if (!dataAlertWizard.DataRule.IsRuleValid)
                    {
                        MessageBox.Show(this, CoreConstants.Exception_InvalidAlertRule, "Invalid Data Alert Rule",
                                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception e)
                {
                    ErrorLog.Instance.Write("Failed to create alert rule", e, true);
                    MessageBox.Show(this, String.Format("Unable to create the new data alert rule.\r\nMessage: {0}", e.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                //Update the Alert Rules View
                if (_activeControl == _adminRibbonView &&
                   _adminRibbonView.GetActiveTab() == AdminRibbonView.Tabs.AlertRules)
                    _activeControl.RefreshView();

                UpdateAlertRules();
                string logString = String.Format("Name:  {0}\r\nDescription:  {1}\r\n\r\nRule:  {2}", dataAlertWizard.DataRule.Name,
                                                                                                      dataAlertWizard.DataRule.Description,
                                                                                                      dataAlertWizard.RuleText.Replace("\n", "\r\n"));
                LogRecord.WriteLog(Globals.Repository.Connection, LogType.AlertRuleAdded, logString);
            }
        }

        public void UpdateAlertRules()
        {
            // check for collection service - cant uninstall if it is down or unreachable
            try
            {
                ServerManager srvManager = GUIRemoteObjectsProvider.ServerManager();
                srvManager.UpdateAlertRules();
            }
            catch (Exception)
            {
                // TODO:  Should we alert the user when we can't talk to the collection server?
            }
        }

        public void FireConnected()
        {
            // TODO:  Major hack, please clean this crap up
            RaiseConnectionChanged(new ConnectionChangedEventArgs(Globals.Repository));
        }

        #region Properties

        public CMEventCategory[] EventCategories
        {
            get { return _alertingConfig.SqlServerCategories; }
        }


        #endregion

        #region Initialization

        private void BuildAdminTree()
        {
            _treeAdmin.Nodes.Clear();
            int imgIndex;
            SQLcomplianceTreeNode node;

            imgIndex = (int)AppIcons.Img16.Server;
            node = new SQLcomplianceTreeNode("Registered SQL Servers", imgIndex, imgIndex, CMNodeType.ServerRoot);
            node.SetMenuFlag(CMMenuItem.Refresh);
            node.SetMenuFlag(CMMenuItem.AttachArchive);
            node.SetMenuFlag(CMMenuItem.NewServer);
            node.SetMenuFlag(CMMenuItem.NewDatabase);
            _treeAdmin.Nodes.Add(node);

            imgIndex = (int)AppIcons.Img16.AlertRules;
            node = new SQLcomplianceTreeNode("Alert Rules", imgIndex, imgIndex, CMNodeType.AlertRulesRoot);
            node.SetMenuFlag(CMMenuItem.Refresh);
            node.SetMenuFlag(CMMenuItem.NewAlertRule);
            node.SetMenuFlag(CMMenuItem.NewStatusAlertRule);
            node.SetMenuFlag(CMMenuItem.GroupByColumn);
            node.SetMenuFlag(CMMenuItem.Expand);
            node.SetMenuFlag(CMMenuItem.Collapse);
            _treeAdmin.Nodes.Add(node);

            imgIndex = (int)AppIcons.Img16.EventFilters;
            node = new SQLcomplianceTreeNode("Audit Event Filters", imgIndex, imgIndex, CMNodeType.EventFilters);
            node.SetMenuFlag(CMMenuItem.Refresh);
            node.SetMenuFlag(CMMenuItem.NewEventFilter);
            node.SetMenuFlag(CMMenuItem.GroupByColumn);
            node.SetMenuFlag(CMMenuItem.Expand);
            node.SetMenuFlag(CMMenuItem.Collapse);
            _treeAdmin.Nodes.Add(node);

            imgIndex = (int)AppIcons.Img16.Role;
            node = new SQLcomplianceTreeNode("Logins", imgIndex, imgIndex, CMNodeType.LoginsRoot);
            node.SetMenuFlag(CMMenuItem.Refresh);
            node.SetMenuFlag(CMMenuItem.NewLogin);
            _treeAdmin.Nodes.Add(node);

            imgIndex = (int)AppIcons.Img16.ActivityLog;
            node = new SQLcomplianceTreeNode("Activity Log", imgIndex, imgIndex, CMNodeType.ActivityLog);
            node.SetMenuFlag(CMMenuItem.Refresh);
            node.SetMenuFlag(CMMenuItem.GroupByColumn);
            node.SetMenuFlag(CMMenuItem.Expand);
            node.SetMenuFlag(CMMenuItem.Collapse);
            _treeAdmin.Nodes.Add(node);

            imgIndex = (int)AppIcons.Img16.ChangeLog;
            node = new SQLcomplianceTreeNode("Change Log", imgIndex, imgIndex, CMNodeType.ChangeLog);
            node.SetMenuFlag(CMMenuItem.Refresh);
            node.SetMenuFlag(CMMenuItem.GroupByColumn);
            node.SetMenuFlag(CMMenuItem.Expand);
            node.SetMenuFlag(CMMenuItem.Collapse);
            _treeAdmin.Nodes.Add(node);
            //start sqlcm 5.6 - 5467
            imgIndex = (int)AppIcons.Img16.DefaultAuditSettings;
            node = new SQLcomplianceTreeNode("Default Audit Settings", imgIndex, imgIndex, CMNodeType.DefaultSettings);
            node.SetMenuFlag(CMMenuItem.Refresh);
            node.SetMenuFlag(CMMenuItem.GroupByColumn);
            node.SetMenuFlag(CMMenuItem.Expand);
            node.SetMenuFlag(CMMenuItem.Collapse);
            _treeAdmin.Nodes.Add(node);
            //end sqlcm 5.6 - 5467

            _treeAdmin.SelectedNode = _treeAdmin.Nodes[0];
        }

        // 
        // BuildServerTree()
        //
        // This function constructs the repository-specific server tree.  This should be done
        //  after a connection has been made to the repository
        //
        private void BuildServerTree(bool resetSelectedNode = true)
        {
            int imgIndex;
            SQLcomplianceTreeNode root;
            _treeServers.Nodes.Clear();
            _treeServers.Sorted = true;

            imgIndex = (int)AppIcons.Img16.Server;
            root = new SQLcomplianceTreeNode("Audited SQL Servers", imgIndex, imgIndex, CMNodeType.AuditByServerRoot);
            root.Tag = "All";
            root.SetMenuFlag(CMMenuItem.Refresh);
            root.SetMenuFlag(CMMenuItem.AttachArchive);
            root.SetMenuFlag(CMMenuItem.NewServer);
            root.SetMenuFlag(CMMenuItem.NewDatabase);

            _treeServers.Nodes.Add(root);
            foreach (ServerRecord record in ServerRecord.GetServers(Globals.Repository.Connection, false, true))
            {
                AddServerNode(record);
                foreach (DatabaseRecord dbRecord in DatabaseRecord.GetDatabases(Globals.Repository.Connection, record.SrvId))
                {
                    AddDatabaseNode(dbRecord);
                }
            }
            root.Expand();

            if (resetSelectedNode)
            {
                _treeServers.SelectedNode = root;
            }

            UpdateDatabasePrivUsersAfterUpgrade();
        }

        public void UpdateDatabasePrivUsersAfterUpgrade()
        {
            bool isDatabaseUpdated = ReadIsDatabaseUpdatedFromRegistry();
            if (!isDatabaseUpdated)
            {
                foreach (ServerRecord record in ServerRecord.GetServers(Globals.Repository.Connection, false, true))
                {
                    UserList serverPrivilegedUsers = new UserList(record.AuditUsersList);
                    foreach (DatabaseRecord dbRecord in DatabaseRecord.GetDatabases(Globals.Repository.Connection, record.SrvId))
                    {
                        DatabaseRecord oldDBRecord = dbRecord.Clone();
                        UserList databasePrivilegedUsers = new UserList(dbRecord.AuditPrivUsersList);
                        foreach (Login l in databasePrivilegedUsers.Logins)
                        {
                            if (serverPrivilegedUsers.Logins.Any(serverLogin => serverLogin.Name == l.Name))
                            {
                                databasePrivilegedUsers.RemoveLogin(l.Name);
                            }
                        }

                        foreach (ServerRole r in databasePrivilegedUsers.ServerRoles)
                        {
                            if (serverPrivilegedUsers.ServerRoles.Any(serverSr => serverSr.CompareName(r)))
                            {
                                databasePrivilegedUsers.RemoveServerRole(r.Name);
                            }
                        }
                        dbRecord.AuditPrivUsersList = databasePrivilegedUsers.ToString();
                        DatabaseRecord.Match(dbRecord, oldDBRecord);
                        dbRecord.UpdatePrivUsers(oldDBRecord, Globals.Repository.Connection);
                    }
                }
                WriteIsDatabaseUpdatedToRegistry();
            }
        }

        private bool ReadIsDatabaseUpdatedFromRegistry()
        {
            RegistryKey rk = null;
            RegistryKey rks = null;
            bool isDatabaseUpdated = false;

            try
            {
                rk = Registry.LocalMachine;
                rks = rk.CreateSubKey(CoreConstants.SQLcompliance_RegKey, RegistryKeyPermissionCheck.ReadSubTree);

                isDatabaseUpdated = (string)rks.GetValue("IsDatabasePrivUsersUpdated", null) == "False" ? false : true;
            }
            catch (Exception)
            {
            }
            finally
            {
                if (rks != null) rks.Close();
                if (rk != null) rk.Close();
            }
            return isDatabaseUpdated;
        }

        private void WriteIsDatabaseUpdatedToRegistry()
        {
            RegistryKey rk = null;
            RegistryKey rks = null;

            try
            {
                rk = Registry.LocalMachine;
                rks = rk.CreateSubKey(CoreConstants.SQLcompliance_RegKey, RegistryKeyPermissionCheck.ReadWriteSubTree);

                rks.SetValue("IsDatabasePrivUsersUpdated", "True");
            }
            catch (Exception)
            {
            }
            finally
            {
                if (rks != null) rks.Close();
                if (rk != null) rk.Close();
            }
        }

        public void RefreshServerRecords()
        {
            TreeNode _selectedNode = _treeServers.SelectedNode;

            FetchGraphData();
            BuildServerTree(false);

            var selectedNode = FindNodeInTree(_selectedNode, _treeServers.Nodes);

            if (selectedNode != null)
            {
                _treeServers.SelectedNode = selectedNode;
            }
            else
            {
                _treeServers.SelectedNode = _treeServers.Nodes[0];
                _treeServers.Focus();
                NavigateToView(ConsoleViews.EnterpriseSummary);
            }
        }

        private TreeNode FindNodeInTree(TreeNode selectedNode, TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Text.Equals(selectedNode.Text) && node.Parent != null && node.Parent.Text.Equals(selectedNode.Parent.Text))
                    return node;
                if (node.Nodes.Count > 0)
                {
                    var innerNode = FindNodeInTree(selectedNode, node.Nodes);
                    if (innerNode != null)
                        return innerNode;
                }
            }
            return null;
        }

        private void BuildReportTree()
        {
            _treeReports.Nodes.Clear();
            int imgIndex;
            SQLcomplianceTreeNode rootAlertsOrHistory;
            SQLcomplianceTreeNode rootConfiguration;
            List<CMReport> reports;

            imgIndex = (int)AppIcons.Img16.ReportServer;
            rootAlertsOrHistory = new SQLcomplianceTreeNode("Alerts /  History", imgIndex, imgIndex, CMNodeType.ReportCategoryRoot);
            rootConfiguration = new SQLcomplianceTreeNode("Configuration", imgIndex, imgIndex, CMNodeType.ReportCategoryRoot);
            rootAlertsOrHistory.Tag = "Alerts /  History";
            rootConfiguration.Tag = "Configuration";
            rootAlertsOrHistory.SetMenuFlag(CMMenuItem.Refresh);
            rootConfiguration.SetMenuFlag(CMMenuItem.Refresh);
            _treeReports.Nodes.Add(rootAlertsOrHistory);
            _treeReports.Nodes.Add(rootConfiguration);
            _treeReports.SelectedNode = rootAlertsOrHistory;
            try
            {
                FileInfo fInfo = new FileInfo(Assembly.GetExecutingAssembly().Location);
                string filePath = Path.Combine(fInfo.DirectoryName, CMReport.RelativeReportsPath);
                reports = ReportXmlHelper.LoadReportListFromXmlFile(Path.Combine(filePath, CMReport.RDLXmlFileName));
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write("Unable to load reports.", e);
                return;
            }

            foreach (CMReport info in reports)
            {
                SQLcomplianceTreeNode rptNode;

                rptNode = new SQLcomplianceTreeNode(info.Name, imgIndex, imgIndex, CMNodeType.Report);
                rptNode.Tag = info;
                rptNode.SetMenuFlag(CMMenuItem.LoadReport);
                if (info.RootHead == "Configuration")
                {
                    rootConfiguration.Nodes.Add(rptNode);
                }
                else
                {
                    rootAlertsOrHistory.Nodes.Add(rptNode);
                }
            }
            rootAlertsOrHistory.Expand();
            rootConfiguration.Expand();
            _reportCategoryView.Reports = reports;
        }

        #endregion

        private void UpdateTitleBar()
        {
            //   Adjust UI - tree; launch pad; etc
            bool evaluationVersion = Globals.SQLcomplianceConfig.LicenseObject.CombinedLicense.isTrial;
            this.Text = String.Format("{0} ({1})", UIConstants.AppTitle, Globals.RepositoryServer);
            if (evaluationVersion)
                this.Text += " [Evaluation Version]";
        }

        private void InitializeUI()
        {
            //------------------------------------------------
            // Connection good - Lots of initialization to do
            //------------------------------------------------
            // Alerting Initialization
            _alertingConfig = new AlertingConfiguration();
            _alertingConfig.Initialize(Globals.RepositoryServer);
            _adminRibbonView.SetAlertingConfiguration(_alertingConfig);
            _ribbonTabView.AlertConfiguration = _alertingConfig;

            // Filtering Initialization
            _filteringConfig = new FiltersConfiguration();
            _filteringConfig.Initialize(Globals.RepositoryServer);
            _adminRibbonView.SetFiltersConfiguration(_filteringConfig);

            UpdateTitleBar();

            //   update LRU list
            UpdateRUList();
            BuildToolsMenu();

            // Clear and Refill Tree
            BuildServerTree();
            BuildReportTree();
            BuildAdminTree();
            _activeTree = _treeServers;

            _treeServers.SelectedNode = _treeServers.Nodes[0];
            _treeServers.Focus();
            NavigateToView(ConsoleViews.EnterpriseSummary);

            // Initialize report views
            _reportCategoryView.ReportsTree = _treeReports;
            _reportView.ServerInstance = Globals.RepositoryServer;

            UpdateColors();

            VerifySchedule();

            // We only show the welcome screen to admins
            if (Globals.isAdmin)
            {
                List<ServerRecord> servers = ServerRecord.GetServers(Globals.Repository.Connection, true);
                if (servers.Count == 0)
                {
                    Settings.Default.FirstRun = false;
                    Form_Welcome frm = new Form_Welcome();
                    if (frm.ShowDialog(this) == DialogResult.Yes)
                        AddServerAction();
                }
            }
        }

        private void VerifySchedule()
        {
            SQLcomplianceConfiguration config = new SQLcomplianceConfiguration();
            config.Read(Globals.Repository.Connection);

            if (config.IndexStartTime == DateTime.MinValue)
            {
                List<ServerRecord> servers = ServerRecord.GetServers(Globals.Repository.Connection, true);

                if (servers.Count > 0)
                {
                    Form_IndexSchedule schedule = new Form_IndexSchedule();
                    schedule.ShowDialog(this);
                    config.IndexStartTime = schedule.IndexStartTime;
                    config.IndexDuration = schedule.IndexDuration;
                    config.Write(Globals.Repository.Connection);
                }
            }
        }

        private void LoadDefaultViews()
        {
            try
            {
                List<EventViewSettings> eventViews;
                List<string> currentViews = new List<string>();
                XmlSerializer ser = new XmlSerializer(typeof(List<EventViewSettings>));
                StringReader reader = new StringReader(Resources.EventViews);
                eventViews = (List<EventViewSettings>)ser.Deserialize(reader);
                foreach (EventViewSettings s in Settings.Default.EventViews)
                {
                    currentViews.Add(s.Name);
                }
                foreach (EventViewSettings s in eventViews)
                {
                    if (!currentViews.Contains(s.Name))
                        Settings.Default.EventViews.Add(s);
                }
                _ribbonTabView.InitializeEventViewCombo();
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write("Unable to load default views.", e);
            }
        }

        private static bool ValidateConnection()
        {
            // check SQLcompliance DB schema
            if (!SQLcomplianceConfiguration.IsCompatibleSchema(Globals.Repository.Connection))
            {
                ErrorMessage.Show(UIConstants.Title_ConnectToServer,
                                  UIConstants.Error_UnsupportedRepositoryVersion,
                                  SQLcomplianceConfiguration.GetLastError());
                return false;
            }

            //--------------------------------------------------------
            // Check if user has privs by reading configuration table
            // also check for supported database schema
            //--------------------------------------------------------
            if (!Globals.ReadConfiguration())
            {
                ErrorMessage.Show(UIConstants.Title_ConnectToServer,
                                  UIConstants.Error_CantReadRepository,
                                  SQLcomplianceConfiguration.GetLastError());
                return false;
            }

            Globals.RepositoryServer = Globals.Repository.Instance;
            Repository.ServerInstance = Globals.Repository.Instance;
            //_connected = true;

            // Read server settings
            Globals.isSysAdmin = RawSQL.IsCurrentUserSysadmin(Globals.Repository.Connection);

            CheckProductLicense();
            UpdateLicenseState();

            return true;
        }

        private static void CheckProductLicense()
        {
            if (!Globals.SQLcomplianceConfig.IsLicenseOk() && Globals.isSysAdmin)
            {
                Form_AddLicense form = new Form_AddLicense(Globals.SQLcomplianceConfig.LicenseObject);
                form.ShowDialog();
                if (!Globals.SQLcomplianceConfig.IsLicenseOk())
                {
                    MessageBox.Show(CoreConstants.LicenseNoValidLicense, CoreConstants.LicenseCaption);
                    System.Windows.Forms.Application.Exit();
                    return;
                }
            }

            WarnForExpiringLicenses();
            WarnForTooManyRegisteredServers();
            WarnForConvertedTrial();
        }

        private static void UpdateLicenseState()
        {
            if (!Globals.SQLcomplianceConfig.LicenseObject.IsProductLicensed())
            {
                Globals.isAdmin = false;
            }
            else
            {
                Globals.isAdmin = Globals.isSysAdmin;
            }
        }

        private static void WarnForConvertedTrial()
        {
            try
            {
                IderaLicense oldLicense = new IderaLicense(Globals.SQLcomplianceConfig.OldLicenseKey);

                if (Globals.SQLcomplianceConfig.LicenseObject.CombinedLicense.isTrial &&
                   oldLicense.IsValid() && !oldLicense.IsTrialLicense())
                {
                    string message = String.Format(CoreConstants.LicenseConvertExpiring,
                       Globals.SQLcomplianceConfig.LicenseObject.CombinedLicense.key,
                       Globals.SQLcomplianceConfig.LicenseObject.CombinedLicense.daysToExpire);
                    MessageBox.Show(message, CoreConstants.LicenseCaption);
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose, "WarnForConvertedTrial", e, ErrorLog.Severity.Warning);
            }
        }

        private static void WarnForTooManyRegisteredServers()
        {
            List<ServerRecord> servers = ServerRecord.GetServers(Globals.Repository.Connection, true);
            if (!Globals.SQLcomplianceConfig.LicenseObject.IsLicneseGoodForServerCount(servers.Count))
            {
                string message = CoreConstants.LicenseTooManyRegisteredServers;
                MessageBox.Show(message, CoreConstants.LicenseCaption);
            }
        }

        private static void WarnForExpiringLicenses()
        {
            List<BBSProductLicense.LicenseData> licenses = Globals.SQLcomplianceConfig.LicenseObject.Licenses;
            StringBuilder message = new StringBuilder();
            foreach (BBSProductLicense.LicenseData licData in licenses)
            {
                if (licData.isAboutToExpire)
                {
                    if (message.Length > 0)
                    {
                        message.AppendFormat("\n");
                    }
                    if (licData.daysToExpire == 0)
                        message.AppendFormat(CoreConstants.LicenseExpiringToday, licData.key);
                    else if (licData.daysToExpire == 1)
                        message.AppendFormat(CoreConstants.LicenseExpiringDay, licData.key);
                    else
                        message.AppendFormat(CoreConstants.LicenseExpiringDays, licData.key, licData.daysToExpireStr);
                }
            }
            if (message.Length > 0)
            {
                MessageBox.Show(message.ToString(), CoreConstants.LicenseCaption);
            }
        }

        private void BuildToolsMenu()
        {
            try
            {
                IList<RegisteredTool> companyTools;
                IList<RegisteredTool> internalTools;
                ButtonTool activeTool;
                int toolCounter = 0;

                foreach (ButtonTool menuItem in _toolsItems)
                {
                    menuItem.InstanceProps.IsFirstInGroup = false;
                    menuItem.SharedProps.Visible = false;
                }
                _toolHandlers.Clear();

                companyTools = ToolFinder.GetRegisteredTools(UIConstants.RegKeyIderaProducts);
                internalTools = ToolFinder.GetRegisteredTools(UIConstants.RegKeySQLcmTools);

                // Internal tools first
                foreach (RegisteredTool tool in internalTools)
                {
                    if (tool.IsValid)
                    {
                        activeTool = _toolsItems[toolCounter];
                        activeTool.SharedProps.Visible = true;
                        activeTool.SharedProps.Caption = tool.Name;
                        _toolHandlers.Add(tool.LaunchEvent);
                        toolCounter++;
                    }
                }
                // If we have internal tools, we want a seperator for the first external tool
                bool addSeperator = false;
                if (_toolHandlers.Count > 0)
                    addSeperator = true;

                foreach (RegisteredTool tool in companyTools)
                {
                    if (tool.IsValid)
                    {
                        activeTool = _toolsItems[toolCounter];
                        activeTool.SharedProps.Visible = true;
                        activeTool.SharedProps.Caption = tool.Name;
                        _toolHandlers.Add(tool.LaunchEvent);
                        toolCounter++;
                        if (addSeperator)
                        {
                            activeTool.InstanceProps.IsFirstInGroup = true;
                            addSeperator = false;
                        }
                    }
                }

                //start sqlcm 5.6 - 5365
                activeTool = _toolsItems[toolCounter];
                activeTool.SharedProps.Visible = true;
                activeTool.SharedProps.Caption = "Default Settings";
                activeTool.InstanceProps.IsFirstInGroup = true;
                defaultSettings.ToolClick += DefaultSettingsClick;
                _toolHandlers.Add(DefaultSettingsClick);

                //start sqlcm 5.6 - 5538
                _toolsMenu.Tools["sensitiveColumnSearch"].SharedProps.Enabled = true;
                _toolsMenu.Tools["sensitiveColumnSearch"].SharedProps.Visible = true;
                _toolsMenu.Tools["sensitiveColumnSearch"].InstanceProps.IsFirstInGroup = true;
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write("Error building tools menu.", e);
                _toolsMenu.SharedProps.Visible = false;
            }
        }
        //start sqlcm 5.6 - 5365
        public void DefaultSettingsClick(Object sender, EventArgs e)
        {
            BaseControl previousControl = _activeControl;
            _activeTree = _treeAdmin;
            TreeNode currentNode = _treeAdmin.Nodes[6];
            _lblTitle.Text = currentNode.Text;
            _activeControl = _adminRibbonView;
            _adminRibbonView.ResetCheckBox();
            _activeControl.RefreshView();
            _adminRibbonView.ShowTab(AdminRibbonView.Tabs.DefaultSettings);
            _activeControl.Enabled = true;
            _activeControl.BringToFront();

            if (previousControl != null && _activeControl != previousControl)
                previousControl.Enabled = false;

            _activeControl.RefreshView();

            UpdateToolbar();
            _explorerBar.SelectedGroup = _explorerBar.Groups[2];
        }
        //end sqlcm 5.6 - 5365


        //start sqlcm 5.6 - 5538
        private void SensitiveColumnSearchAction()
        {
            string serverName = null;
            string databaseName = null;
            if (_treeServers.Focused && _treeServers.SelectedNode != null)
            {
                var node = _treeServers.SelectedNode as SQLcomplianceTreeNode;
                if (node != null)
                {
                    switch (node.Type)
                    {
                        case CMNodeType.Server:
                            serverName = ((ServerRecord)node.Tag).Instance;
                            break;
                        case CMNodeType.Database:
                            serverName = ((DatabaseRecord)node.Tag).SrvInstance;
                            databaseName = ((DatabaseRecord)node.Tag).Name;
                            break;
                    }
                }
            }

            using (var form = new Form_SensitiveColumnsSearch(serverName, databaseName))
            {
                form.ShowDialog(this);
            }
        }
        //end sqlcm 5.6 - 5538

        private void ConnectAction()
        {
            if (PromptForConnection())
                InitializeUI();
        }

        private void ConnectAction(string serverName)
        {
            string server = null;

            if (!String.IsNullOrEmpty(Globals.RepositoryServer))
                server = Globals.RepositoryServer;

            // try to connect
            if (ConnectToServer(this, serverName, true)
               || (server != null && ConnectToServer(this, server, true)))
            {
                RaiseConnectionChanged(new ConnectionChangedEventArgs(Globals.Repository));
                InitializeUI();
            }
            else
            {
                // At this point, we have failed to connect to the supplied server and have
                //  failed to reconnect to the previous server.  Simply prompt the user.
                Globals.RepositoryServer = null;
                ConnectAction();
            }
        }

        //
        // This routine prompts the user for a connection.  If the connection
        //  attempt fails, it continues to prompt the user.  If the user cancels 
        //  the connection attempt, it attempts to connect to the previous repository
        //  again.  If this also fails, the user has the option to simply close compliance.
        //
        private bool PromptForConnection()
        {
            bool isConnected = false;
            string server = null;

            if (!String.IsNullOrEmpty(Globals.RepositoryServer))
                server = Globals.RepositoryServer;

            while (!isConnected)
            {
                using (Form_Connect dlg = new Form_Connect())
                {
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        // try to connect
                        if (ConnectToServer(this, dlg.Server, true))
                            isConnected = true;
                    }
                    else
                    {
                        // In this case the user has canceled - we reconnect to previous server
                        if (server != null && ConnectToServer(this, server, true))
                        {
                            isConnected = true;
                        }
                        else
                        {
                            if (MessageBox.Show(String.Format("Unable to connect to repository {0}.  Do you wish to close SQL Compliance Manager?", server),
                               "Error Connecting", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                // We are done.
                                Close();
                                return false;
                            }
                        }
                    }
                }
            }
            RaiseConnectionChanged(new ConnectionChangedEventArgs(Globals.Repository));
            return true;
        }

        //      private bool ReconnectToCurrentServer()
        //      {
        //         return ConnectToServer(this, Globals.RepositoryServer, true);
        //      }

        //------------------------------------------------------------
        // ConnectToServer - Routine that creates connection to 
        //                   SQLcompliance repository and server and
        //                   updates RU list of connected servers
        //------------------------------------------------------------
        public static bool ConnectToServer(Form owner, string serverName, bool showConnectingDialog)
        {
            if (owner == null)
                return false;

            if (serverName == null || serverName.Length == 0)
            {
                return false;
            }

            if (serverName == "." || serverName.ToUpper() == "(LOCAL)")
            {
                serverName = Dns.GetHostName().ToUpper();
            }

            // establish SQLcompliance Repository Connection
            owner.Cursor = Cursors.WaitCursor;
            bool connected;

            if (showConnectingDialog)
            {
                Form_Connecting dlg = new Form_Connecting(serverName);
                connected = (dlg.ShowDialog(owner) == DialogResult.OK);
            }
            else
            {
                connected = Globals.Repository.OpenConnection(serverName);
            }
            if (!connected)
            {
                ErrorMessage.Show(UIConstants.Title_ConnectToServer,
                                  String.Format(UIConstants.Error_CantConnectToServer, serverName),
                                  Globals.Repository.GetLastError());
                return false;
            }
            else
            {
                RecentlyUsedList.Instance.Insert(serverName);
                return ValidateConnection();
            }
        }

        #region Load and Save RUList

        //------------------------------------------------------------
        // UpdateRUList - Load SQLcompliance Management console data
        //                   persisted to HKEY_LOCAL__MACHINE key
        //------------------------------------------------------------
        private void UpdateRUList()
        {
            // Update menu
            _recentServerMenus[0].SharedProps.Visible = false;
            _recentServerMenus[1].SharedProps.Visible = false;
            _recentServerMenus[2].SharedProps.Visible = false;
            _recentServerMenus[3].SharedProps.Visible = false;
            _recentServerMenus[4].SharedProps.Visible = false;

            // RU Items
            for (int i = 0; i < RecentlyUsedList.Instance.Count; i++)
            {
                _recentServerMenus[i].SharedProps.Caption = String.Format("&{0} {1}", i + 1, RecentlyUsedList.Instance.GetItem(i));
                _recentServerMenus[i].SharedProps.Visible = true;
                _recentServerMenus[i].SharedProps.Tag = RecentlyUsedList.Instance.GetItem(i);
            }
        }

        #endregion

        private void Shown_Form_Main(object sender, EventArgs e)
        {
            // If our cnonection failed at startup (splash), then we need to prompt to be nice.
            if (!Globals.Repository.Connected && !PromptForConnection())
                return;


            // At this point, we have a connection from splash or from prompt, or nothing
            // On nothing we disable app except Connect dialog
            if (Globals.Repository.Connected)
            {
                InitializeUI();
                string path = AppDomain.CurrentDomain.BaseDirectory;
                path = path + @"SQLcomplianceIndexUpgrade.exe";
                ProcessStartInfo startInfo = new ProcessStartInfo(path);
                startInfo.WindowStyle = ProcessWindowStyle.Normal;
                startInfo.CreateNoWindow = false;
                startInfo.Arguments = " \"" + Globals.Repository.Connection.ConnectionString.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";
                Process.Start(startInfo);
            }
            else
                System.Windows.Forms.Application.Exit();

            // to make XE default auditing after upgrade 5.8 - SQLCM-6206
            if (xeDefaultUpdateWorker.IsBusy == false)
            {
                xeDefaultUpdateWorker.DoWork += new DoWorkEventHandler(XEDefaultUpdateNow);
                xeDefaultUpdateWorker.RunWorkerAsync();
            }
        }

        private void AfterSelect_treeAdmin(object sender, TreeViewEventArgs e)
        {
            _lblTitle.Text = e.Node.Text;
            if (_internalUpdate == 0)
            {
                switch (e.Node.Text)
                {
                    case "Registered SQL Servers":
                        _adminRibbonView.ShowTab(AdminRibbonView.Tabs.RegisteredServers);
                        break;
                    case "Alert Rules":
                        _adminRibbonView.ShowTab(AdminRibbonView.Tabs.AlertRules);
                        break;
                    case "Audit Event Filters":
                        _adminRibbonView.ShowTab(AdminRibbonView.Tabs.EventFilters);
                        break;
                    case "Logins":
                        _adminRibbonView.ShowTab(AdminRibbonView.Tabs.SqlLogins);
                        break;
                    case "Activity Log":
                        _adminRibbonView.ShowTab(AdminRibbonView.Tabs.ActivityLog);
                        break;
                    case "Change Log":
                        _adminRibbonView.ShowTab(AdminRibbonView.Tabs.ChangeLog);
                        break;
                    //start sqlcm 5.6 - 5467
                    case "Default Audit Settings":
                        _adminRibbonView.ShowTab(AdminRibbonView.Tabs.DefaultSettings);
                        break;
                        //end sqlcm 5.6 - 5467
                }
                _explorerBar.Groups[2].Tag = _activeControl;
                if (_treeAdmin.Focused)
                {
                    _activeControl.BringToFront();
                    _activeControl.RefreshView();
                }
            }
            UpdateToolbar();
        }

        public void NewLoginAction()
        {
            Form_LoginNew dlg = new Form_LoginNew();
            if (DialogResult.OK == dlg.ShowDialog())
            {
                if (_activeControl == _adminRibbonView &&
                   _adminRibbonView.GetActiveTab() == AdminRibbonView.Tabs.SqlLogins)
                    _activeControl.RefreshView();
            }
        }

        private string GetCurrentInstance()
        {
            string instance = "";

            SQLcomplianceTreeNode currNode = (SQLcomplianceTreeNode)_treeServers.SelectedNode;
            if (currNode.Type == CMNodeType.Server)
            {
                instance = ServerTreeNodeHelper.GetInstanceNameFromServerTreeNode(currNode);
            }
            else if (currNode.Type == CMNodeType.Database)
            {
                SQLcomplianceTreeNode prnt = (SQLcomplianceTreeNode)currNode.Parent;
                instance = prnt.Text;
            }

            return instance;
        }

        public void UpdateAlertingConfiguration()
        {
            // check for collection service - cant uninstall if it is down or unreachable
            try
            {
                ServerManager srvManager = GUIRemoteObjectsProvider.ServerManager();
                srvManager.UpdateAlertingConfiguration();
            }
            catch (Exception)
            {
                // TODO:  Should we alert the user when we can't talk to the collection server?
            }
        }

        private void AfterSelect_treeServers(object sender, TreeViewEventArgs e)
        {
            _activeControl = _ribbonTabView;
            if (e.Node.Tag is ServerRecord)
            {
                ServerRecord server = (ServerRecord)e.Node.Tag;
                _lblTitle.Text = server.Instance;
                _toolbarManager.SetContextMenuUltra(_treeServers, "contextServer");
                _ribbonTabView.SetScope(server);
                Globals.isServerNodeSelected = true;
            }
            else if (e.Node.Tag is DatabaseRecord)
            {
                _toolbarManager.SetContextMenuUltra(_treeServers, "contextDatabase");
                DatabaseRecord db = (DatabaseRecord)e.Node.Tag;
                _lblTitle.Text = String.Format("{0}::{1}", db.SrvInstance, db.Name);
                _ribbonTabView.SetScope(db);
                Globals.isServerNodeSelected = false;
            }
            else
            {
                _toolbarManager.SetContextMenuUltra(_treeServers, "contextRoot");
                _ribbonTabView.SetScope();
                _lblTitle.Text = "Audited SQL Servers";
            }


            UpdateToolbar();
        }

        private void SelectedGroupChanged_explorerBar(object sender, GroupEventArgs e)
        {
            BaseControl previousControl = _activeControl;

            if (e.Group == _explorerBar.Groups[0])
            {
                // Explore Activity
                SQLcomplianceTreeNode selectedNode = _treeServers.SelectedNode as SQLcomplianceTreeNode;
                if (selectedNode.Tag is ServerRecord)
                {
                    // We have to reset this.  There is a chance the ChangeLog was viewed on the 
                    //  administrator node, where it does not have a server filter
                    _lblTitle.Text = ServerTreeNodeHelper.GetInstanceNameFromServerTreeNode(selectedNode);
                }
                else if (selectedNode.Tag is DatabaseRecord)
                {
                    DatabaseRecord db = (DatabaseRecord)selectedNode.Tag;
                    _lblTitle.Text = String.Format("{0}::{1}", db.SrvInstance, db.Name);
                }
                else
                {
                    _lblTitle.Text = "Audited SQL Servers";
                }
                _activeTree = _treeServers;
                _activeControl = _ribbonTabView;
            }
            else if (e.Group == _explorerBar.Groups[1])
            {
                // Audit Reports

                _activeTree = _treeReports;
                TreeNode currentNode = _treeReports.SelectedNode;
                _lblTitle.Text = currentNode.Text;
                if (currentNode.Tag is string)
                {
                    _activeControl = _reportCategoryView;
                }
                else
                {
                    _activeControl = _reportView;
                }
                _activeTree = _treeReports;
            }
            else if (e.Group == _explorerBar.Groups[2])
            {
                // Administration
                _activeTree = _treeAdmin;
                TreeNode currentNode = _treeAdmin.SelectedNode;
                _lblTitle.Text = currentNode.Text;
                _activeControl = _adminRibbonView;
                //start sqlcm 5.6 - 5467
                _adminRibbonView.ResetCheckBox();
                //end sqlcm 5.6 - 5467

            }
            _activeControl.Enabled = true;
            _activeControl.BringToFront();
            if (previousControl != null && _activeControl != previousControl)
                previousControl.Enabled = false;

            _activeControl.RefreshView();
            UpdateToolbar();
        }

        #region Toolbar events

        #endregion Toolbar events

        //      private void Opening_contextMenuServer(object sender, CancelEventArgs e)
        //      {
        //         SQLcomplianceTreeNode currNode = (SQLcomplianceTreeNode)_treeServers.SelectedNode;
        //         ServerRecord server = (ServerRecord)currNode.Tag;
        //
        //         _toolbarManager.Tools["fileNewDatabase"].SharedProps.Enabled = Globals.isAdmin;
        //         _toolbarManager.Tools["fileNewServer"].SharedProps.Enabled = Globals.isAdmin;
        //         if (server.IsAuditedServer)
        //         {
        //            _toolbarManager.Tools["agentUpdateAuditSettings"].SharedProps.Enabled = Globals.isAdmin;
        //            _toolbarManager.Tools["auditingCollectNow"].SharedProps.Enabled = Globals.isAdmin;
        //            _toolbarManager.Tools["editRemove"].SharedProps.Enabled = Globals.isSysAdmin;
        //            _toolbarManager.Tools["auditingDisableAuditing"].SharedProps.Enabled = server.IsEnabled && Globals.isAdmin;
        //            _toolbarManager.Tools["auditingEnableAuditing"].SharedProps.Enabled = !server.IsEnabled && Globals.isAdmin;
        //         }
        //         else
        //         {
        //            _toolbarManager.Tools["agentUpdateAuditSettings"].SharedProps.Enabled = false;
        //            _toolbarManager.Tools["auditingCollectNow"].SharedProps.Enabled = false;
        //            _toolbarManager.Tools["editRemove"].SharedProps.Enabled = false;
        //            _toolbarManager.Tools["auditingDisableAuditing"].SharedProps.Enabled = false ;
        //            _toolbarManager.Tools["auditingEnableAuditing"].SharedProps.Enabled = false;
        //         }
        //      }
        //
        //      private void Opening_contextMenuRoot(object sender, CancelEventArgs e)
        //      {
        //         _toolbarManager.Tools["fileNewServer"].SharedProps.Enabled = Globals.isAdmin;
        //      }
        //
        //      private void Opening_contextMenuDatabase(object sender, CancelEventArgs e)
        //      {
        //         SQLcomplianceTreeNode currNode = (SQLcomplianceTreeNode)_treeServers.SelectedNode;
        //         ServerRecord server = (ServerRecord)currNode.Parent.Tag;
        //         DatabaseRecord database = (DatabaseRecord)currNode.Tag;
        //
        //         _toolbarManager.Tools["auditingDisableAuditing"].SharedProps.Enabled = server.IsEnabled && database.IsEnabled && Globals.isAdmin;
        //         _toolbarManager.Tools["auditingEnableAuditing"].SharedProps.Enabled = server.IsEnabled && !database.IsEnabled && Globals.isAdmin;
        //         _toolbarManager.Tools["fileNewDatabase"].SharedProps.Enabled = Globals.isAdmin;
        //      }
        //
        //      private void Opening_cmMenuAdminTree(object sender, CancelEventArgs e)
        //      {
        ////         if (_activeControl == _eventFiltersView)
        ////         {
        ////            _toolbarManager.Tools["exportAlertRules"].SharedProps.Visible = false ;
        ////            _toolbarManager.Tools["exportEventFilters"].SharedProps.Visible = true ;
        ////         }
        ////         else if (_activeControl == _alertRulesView)
        ////         {
        ////            _toolbarManager.Tools["exportAlertRules"].SharedProps.Visible = true ;
        ////            _toolbarManager.Tools["exportEventFilters"].SharedProps.Visible = false ;
        ////         }
        ////         else
        ////         {
        ////            _toolbarManager.Tools["exportAlertRules"].SharedProps.Visible = false ;
        ////            _toolbarManager.Tools["exportEventFilters"].SharedProps.Visible = false ;
        ////         }
        //      }

        public void ExportAlertRulesAction()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
            dlg.FilterIndex = 1;
            dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            dlg.FileName = "AlertRules.xml";
            dlg.Title = "Export Alert Rules";

            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                //Dump the data
                AlertRuleTemplate tmpl = new AlertRuleTemplate();
                tmpl.RepositoryServer = Globals.RepositoryServer;
                tmpl.Import(null);
                tmpl.Save(dlg.FileName);
            }
        }

        public void ImportAlertRulesAction()
        {
            OpenFileDialog frm = new OpenFileDialog();
            frm.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
            frm.FilterIndex = 1;
            frm.RestoreDirectory = true;
            frm.Title = "Select Alert Rules to Import";

            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                AlertRuleTemplate ruleTemplate = new AlertRuleTemplate();
                if (!ruleTemplate.Load(frm.FileName))
                {
                    MessageBox.Show("Unable to read alert rules for import.", "Error");
                    return;
                }
                else
                {
                    foreach (AlertRule rule in ruleTemplate.AlertRules)
                    {
                        rule.Enabled = false;
                        AlertingDal.InsertAlertRule(rule, Globals.Repository.Connection);
                    }

                    foreach (StatusAlertRule statusRule in ruleTemplate.StatusAlertRules)
                    {
                        statusRule.Enabled = false;
                        AlertingDal.InsertAlertRule(statusRule, Globals.Repository.Connection);
                    }

                    foreach (DataAlertRule dataRule in ruleTemplate.DataAlertRules)
                    {
                        dataRule.Enabled = false;
                        AlertingDal.InsertAlertRule(dataRule, Globals.Repository.Connection);
                    }
                }
                // Refresh View if alert rules are visible
                if (_activeControl == _adminRibbonView &&
                   _adminRibbonView.GetActiveTab() == AdminRibbonView.Tabs.AlertRules)
                    _activeControl.RefreshView();
            }
        }

        public void ExportEventFiltersAction()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
            dlg.FilterIndex = 1;
            dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            dlg.FileName = "EventFilters.xml";
            dlg.Title = "Export Event Filters";

            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                //Dump the data
                FilterTemplate tmpl = new FilterTemplate();
                tmpl.RepositoryServer = Globals.RepositoryServer;
                tmpl.Import(null);
                tmpl.Save(dlg.FileName);
            }
        }

        public void ImportEventFiltersAction()
        {
            OpenFileDialog frm = new OpenFileDialog();
            frm.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
            frm.FilterIndex = 1;
            frm.RestoreDirectory = true;
            frm.Title = "Select Event Filters to Import";

            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                FilterTemplate filterTemplate = new FilterTemplate();
                if (!filterTemplate.Load(frm.FileName))
                {
                    MessageBox.Show("Unable to read event filters for import.", "Error");
                    return;
                }
                else
                {
                    foreach (EventFilter filter in filterTemplate.EventFilters)
                    {
                        filter.Enabled = false;
                        FiltersDal.InsertEventFilter(filter, Globals.Repository.Connection);
                    }
                }
                // Refresh View if event filters are visible
                if (_activeControl == _adminRibbonView &&
                   _adminRibbonView.GetActiveTab() == AdminRibbonView.Tabs.EventFilters)
                    _activeControl.RefreshView();
            }
        }

        public void ImportAuditSettingsAction()
        {
            Form_AuditSettingsImport frm = new Form_AuditSettingsImport();
            frm.ShowDialog(this);
        }

        private void AfterSelect_treeReports(object sender, TreeViewEventArgs e)
        {

            BaseControl previousControl = _activeControl;

            _lblTitle.Text = e.Node.Text;
            if (e.Node.Tag is string) //root
            {
                //_treeServers.ContextMenuStrip = _contextMenuServer;
                _activeControl = _reportCategoryView;
            }
            else // Report node
            {
                string username = WindowsIdentity.GetCurrent().Name;
                if (RawSQL.IsReportAccessible(username, e.Node.Text, Globals.Repository.Connection))
                {
                    MessageBox.Show(this, "You do not have access to this report.", "Unauthorized Report Access", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    BuildReportTree();
                }
                else
                {
                    _activeControl = _reportView;
                    _reportView.Report = (CMReport)e.Node.Tag;
                }
            }
            _activeControl.Enabled = true;
            _activeControl.BringToFront();
            if (previousControl != null && _activeControl != previousControl)
                previousControl.Enabled = false;

            _activeControl.RefreshView();
            UpdateToolbar();
        }

        private void FormClosed_Main(object sender, FormClosedEventArgs e)
        {
            Settings.Default.Save();
            DefaultAuditSettings.RestoreDefaultAuditFlags(Globals.Repository.Connection);
        }

        private void ToolClick_toolbarManager(object sender, ToolClickEventArgs e)
        {
            object o;

            switch (e.Tool.Key)
            {
                case "LaunchWebConsole":
                    if (CwfHelper.Instance.IsInitialized)
                    {
                        var launchWebConsole = new Form_LaunchWebConsole();
                        launchWebConsole.ShowDialog(this);
                    }
                    else
                        Process.Start("https://localhost:9291/");
                    break;

                case "fileConnect":    // ButtonTool
                    ConnectAction();
                    break;

                case "fileNewServer":    // ButtonTool
                    AddServerAction();
                    break;

                case "fileNewDatabase":    // ButtonTool
                    o = _activeTree.SelectedNode.Tag;
                    if (o is ServerRecord)
                        AddDatabaseAction(o as ServerRecord);
                    else if (o is DatabaseRecord)
                    {
                        if (_activeTree.SelectedNode.Parent.Tag is ServerRecord)
                            AddDatabaseAction(_activeTree.SelectedNode.Parent.Tag as ServerRecord);
                        else
                            //we should never get here,
                            AddDatabaseAction(null);
                    }
                    else
                        // or here but they are being included for completeness.
                        AddDatabaseAction(null);
                    break;

                case "fileNewLogin":    // ButtonTool
                    NewLoginAction();
                    break;

                case "fileNewAlertRule":    // ButtonTool
                    NewAlertRule();
                    break;

                case "fileNewEventFilter":    // ButtonTool
                    NewEventFilter();
                    break;

                case "fileAttachArchive":    // ButtonTool
                    AttachArchiveAction();
                    break;

                case "fileImportAuditSettings":    // ButtonTool
                    ImportAuditSettingsAction();
                    break;
                case "fileExportAuditSettings":    // ButtonTool
                    if (_activeTree == _treeServers)
                    {
                        o = _activeTree.SelectedNode.Tag;
                        if (o is DatabaseRecord)
                            ExportDatabaseAuditSettingsAction(o as DatabaseRecord);
                        else if (o is ServerRecord)
                            ExportServerAuditSettingsAction(o as ServerRecord);
                    }
                    else
                    {
                        ServerRecord server = _adminRibbonView.GetSelectedServer();
                        if (server != null)
                            ExportServerAuditSettingsAction(server);
                    }
                    break;
                case "applyAuditTemplate":

                    if (_activeTree.SelectedNode.Tag is DatabaseRecord)
                        ApplyAuditTemplate(_activeTree.SelectedNode.Tag as DatabaseRecord, _activeTree.SelectedNode.Parent.Tag as ServerRecord);
                    else if (_activeTree.SelectedNode.Tag is ServerRecord)
                        ApplyAuditTemplate(null, _activeTree.SelectedNode.Tag as ServerRecord);
                    break;
                case "fileImportAlertRules":    // ButtonTool
                    ImportAlertRulesAction();
                    break;
                case "fileImportEventFilters":    // ButtonTool
                    ImportEventFiltersAction();
                    break;

                case "fileManageLicense":    // ButtonTool
                    ManageLicenseAction();
                    break;

                case "filePrint":    // ButtonTool
                                     // Place code here
                    break;
                case "server1":
                    ConnectAction(_recentServerMenus[0].SharedProps.Tag as string);
                    break;
                case "server2":
                    ConnectAction(_recentServerMenus[1].SharedProps.Tag as string);
                    break;
                case "server3":
                    ConnectAction(_recentServerMenus[2].SharedProps.Tag as string);
                    break;
                case "server4":
                    ConnectAction(_recentServerMenus[3].SharedProps.Tag as string);
                    break;
                case "server5":
                    ConnectAction(_recentServerMenus[4].SharedProps.Tag as string);
                    break;
                case "fileExit":    // ButtonTool
                    Close();
                    break;

                case "editRemove":    // ButtonTool
                    RemoveAction();
                    break;

                case "editProperties":    // ButtonTool
                    PropertiesAction();
                    break;

                case "viewTree":    // StateButtonTool
                    ShowConsoleTree(((StateButtonTool)_toolbarManager.Tools["viewTree"]).Checked);
                    break;

                case "viewToolbar":    // StateButtonTool
                    ShowToolbar(((StateButtonTool)_toolbarManager.Tools["viewToolbar"]).Checked);
                    break;

                case "viewRefresh":    // ButtonTool
                    RefreshAction();
                    break;

                case "viewPreferences":    // ButtonTool
                    PreferencesAction();
                    break;

                case "auditingEnableAuditing":    // ButtonTool
                    EnableAction();
                    break;

                case "auditingDisableAuditing":    // ButtonTool
                    DisableAction();
                    break;

                case "auditingCollectNow":    // ButtonTool
                    CollectNowAction();
                    break;

                case "auditingPermissionsCheck": // ButtonTool
                    PermissionsCheckAction();
                    break;

                case "auditingCheckIntegrity":    // ButtonTool
                    CheckIntegrityAction();
                    break;

                case "auditingCaptureSnapshot":    // ButtonTool
                    CaptureSnapshotAction();
                    break;

                case "auditingSnapshotPreferences":    // ButtonTool
                    SnapshotPreferencesAction();
                    break;

                case "auditingLoginFilter":    // ButtonTool
                    LoginFiltersAction();
                    break;

                case "auditingCollectionServerStatus":    // ButtonTool
                    CollectionServerStatusAction();
                    break;

                case "auditingConfigureRepository":    // ButtonTool
                    ConfigureRepositoryAction();
                    break;

                case "alertingConfigureEmail":    // ButtonTool
                    ConfigureEmailAction();
                    break;

                case "alertingConfigureSnmp": // ButtonTool
                    ConfigureSnmpAction();
                    break;

                case "alertingGroom":    // ButtonTool
                    GroomAlertsAction();
                    break;

                case "agentDeploy":    // ButtonTool
                    DeployAgentAction();
                    break;

                case "agentUpgrade":    // ButtonTool
                    UpgradeAgentAction();
                    break;

                case "agentStatus":    // ButtonTool
                    CheckAgentStatusAction();
                    break;

                case "agentUpdateAuditSettings":    // ButtonTool
                    UpdateNowAction();
                    break;

                case "auditingImportCSV":      // ButtonTool
                    ImportCsvAction();
                    break;

                case "sensitiveColumnSearch":      // ButtonTool
                    SensitiveColumnSearchAction();
                    break;

                case "agentChangeTraceDirectory":    // ButtonTool
                    AgentTraceDirectoryAction();
                    break;

                case "agentProperties":    // ButtonTool
                    AgentPropertiesAction();
                    break;

                case "helpWindow":    // ButtonTool
                    HelpOnThisWindowAction();
                    break;

                case "helpAuditing":    // ButtonTool
                    HelpAlias.ShowHelp(this, HelpAlias.SSHELP_HowAuditingWorks);
                    break;

                case "helpSecurity":    // ButtonTool
                    HelpAlias.ShowHelp(this, HelpAlias.SSHELP_HowConsoleSecurityWorks);
                    break;

                case "helpReports":    // ButtonTool
                    HelpAlias.ShowHelp(this, HelpAlias.SSHELP_ReportingOnAuditData);
                    break;

                case "helpAlerting":    // ButtonTool
                    HelpAlias.ShowHelp(this, HelpAlias.SSHELP_AlertingGeneral);
                    break;

                case "helpBestPractices":    // ButtonTool
                    HelpAlias.ShowHelp(this, HelpAlias.SSHELP_AuditingBestPractices);
                    break;

                case "helpMigrate":    // ButtonTool
                    HelpAlias.ShowHelp(this, HelpAlias.SSHELP_MigrateCollectionServer);
                    break;

                case "helpContents":    // ButtonTool
                    HelpAlias.ShowHelp(this, HelpAlias.SSHELP_ViewContents);
                    break;

                case "helpSearch":    // ButtonTool
                    HelpAlias.LaunchWebBrowser(UIConstants.KnowledgeBaseHomePage);
                    break;

                case "helpMore":    // ButtonTool
                    HelpAlias.LaunchWebBrowser(UIConstants.SQLcomplianceHomePage);
                    break;

                case "helpContactSupport":    // ButtonTool
                    HelpAlias.LaunchWebBrowser(UIConstants.SupportHomePage);
                    break;

                case "helpIderaProducts":    // ButtonTool
                    HelpAlias.LaunchWebBrowser(UIConstants.IderaHomePage);
                    break;

                case "helpCheckForUpdates":
                    HelpAlias.LaunchWebBrowser(UIConstants.SQLcomplianceUpdatePage);
                    break;

                case "helpAbout":    // ButtonTool
                    AboutAction();
                    break;

                case "auditingArchiveNow":    // ButtonTool
                    ArchiveNowAction();
                    break;

                case "auditingGroomNow":    // ButtonTool
                    GroomNowAction();
                    break;

                case "auditingArchivePreferences":    // ButtonTool
                    ArchivePreferencesAction();
                    break;
                case "fileExportAlertRules":    // ButtonTool
                    ExportAlertRulesAction();
                    break;
                case "fileExportEventFilters":    // ButtonTool
                    ExportEventFiltersAction();
                    break;
                case "tool0":
                    LaunchTool(0);
                    break;
                case "tool1":
                    LaunchTool(1);
                    break;
                case "tool2":
                    LaunchTool(2);
                    break;
                case "tool3":
                    LaunchTool(3);
                    break;
                case "tool4":
                    LaunchTool(4);
                    break;
                case "tool5":
                    LaunchTool(5);
                    break;
                case "tool6":
                    LaunchTool(6);
                    break;
                case "tool7":
                    LaunchTool(7);
                    break;
                case "tool8":
                    LaunchTool(8);
                    break;
                case "tool9":
                    LaunchTool(9);
                    break;
            }
        }

        private void LaunchTool(int index)
        {
            if (_toolHandlers.Count > index)
            {
                _toolHandlers[index].Invoke(this, new EventArgs());
            }
        }

        private void AboutAction()
        {
            AboutForm dlg = new AboutForm();
            dlg.ShowDialog();
        }

        internal void CollectNowAction()
        {
            string instance = null;

            if (_activeTree == _treeServers)
            {
                SQLcomplianceTreeNode node = _treeServers.SelectedNode as SQLcomplianceTreeNode;
                if (node == null)
                    return;
                if (node.Tag is ServerRecord)
                {
                    instance = ((ServerRecord)node.Tag).Instance;
                }
            }
            else
            {
                ServerRecord tmp = _adminRibbonView.GetSelectedServer();
                if (tmp != null)
                    instance = tmp.Instance;
            }

            if (instance != null && instance.Length > 0)
            {
                Cursor = Cursors.WaitCursor;
                Server.ForceCollection(instance);
                Cursor = Cursors.Default;
            }
        }

        private void PermissionsCheckAction()
        {
            Form_PermissionsCheck permissionsCheck = new Form_PermissionsCheck();
            permissionsCheck.ShowDialog(this);
        }

        private void CheckIntegrityAction()
        {
            string instance = this.GetCurrentInstance();
            CheckIntegrity(instance);
        }

        private void CaptureSnapshotAction()
        {
            string instance = "";
            if (_activeTree == _treeServers)
            {
                SQLcomplianceTreeNode currNode = (SQLcomplianceTreeNode)_treeServers.SelectedNode;
                if (currNode.Type == CMNodeType.AuditServer)
                    instance = ServerTreeNodeHelper.GetInstanceNameFromServerTreeNode(currNode);
            }
            DoSnapshot(instance);

            if (_activeControl == _ribbonTabView)
            {
                _activeControl.RefreshView();
            }
        }

        private void SnapshotPreferencesAction()
        {
            Form_SnapshotOptions frm = new Form_SnapshotOptions();
            frm.ShowDialog();
        }

        private void LoginFiltersAction()
        {
            Form_LoginFilterOptions frm = new Form_LoginFilterOptions();
            frm.ShowDialog();
        }

        private void CollectionServerStatusAction()
        {
            Form_ServerOptions frm = new Form_ServerOptions();
            frm.ShowDialog();
        }

        private void ConfigureRepositoryAction()
        {
            Form_RepositoryOptions frm = new Form_RepositoryOptions();
            frm.ShowDialog();
        }

        private void ConfigureEmailAction()
        {
            Form_AlertingOptions options = new Form_AlertingOptions();

            options.SmtpSettings = _alertingConfig.SmtpSettings.Clone();

            if (options.ShowDialog(this) == DialogResult.OK)
            {
                if (!options.SmtpSettings.Equals(_alertingConfig.SmtpSettings))
                {
                    _alertingConfig.SmtpSettings = options.SmtpSettings;
                    AlertingDal.UpdateSmtpConfiguration(_alertingConfig.SmtpSettings, _alertingConfig.ConnectionString);
                    UpdateAlertingConfiguration();
                }
            }
        }

        private void ConfigureSnmpAction()
        {
            Form_AlertingOptions_Snmp options = new Form_AlertingOptions_Snmp(_alertingConfig.SnmpConfiguration.Clone());

            if (options.ShowDialog(this) == DialogResult.OK &&
               !options.SnmpConfiguration.Equals(_alertingConfig.SnmpConfiguration))
            {
                _alertingConfig.SnmpConfiguration = options.SnmpConfiguration;
                AlertingDal.UpdateSnmpConfiguration(_alertingConfig.SnmpConfiguration, _alertingConfig.ConnectionString);
                UpdateAlertingConfiguration();
            }
        }

        private void GroomAlertsAction()
        {
            Form_GroomAlerts form = new Form_GroomAlerts(_alertingConfig);

            form.ShowDialog(this);
            // If the alerts view is active, refresh it so we can show the results of the groom
            if (_activeControl == _ribbonTabView &&
               _ribbonTabView.GetActiveTab() == RibbonView.Tabs.Alerts)
                _activeControl.RefreshView();
        }

        private void DeployAgentAction()
        {
            if (_activeControl != null)
                _activeControl.DeployAgent();
        }

        private void UpgradeAgentAction()
        {
            if (_activeControl != null)
                _activeControl.UpgradeAgent();
        }

        private void CheckAgentStatusAction()
        {
            if (_treeServers.Focused)
            {
                SQLcomplianceTreeNode currNode = (SQLcomplianceTreeNode)_treeServers.SelectedNode;
                if (currNode != null && currNode.Tag is ServerRecord)
                    CheckAgent(currNode.Tag as ServerRecord);
            }
            else if (_activeControl != null)
                _activeControl.CheckAgent();
        }

        private void UpdateNowAction()
        {
            if (_treeServers.Focused)
            {
                SQLcomplianceTreeNode currNode = (SQLcomplianceTreeNode)_treeServers.SelectedNode;
                if (currNode != null && currNode.Tag is ServerRecord)
                {
                    var instanceName = ServerTreeNodeHelper.GetInstanceNameFromServerTreeNode(currNode);
                    ServerRecord server = ServerTreeNodeHelper.GetServerRecordFromServerTreeNode(currNode);

                    UpdateNow(instanceName, server.AgentVersion);
                }
            }
            else if (_activeControl != null)
                _activeControl.UpdateNow();
        }

        private void AgentTraceDirectoryAction()
        {
            if (_treeServers.Focused)
            {
                SQLcomplianceTreeNode currNode = (SQLcomplianceTreeNode)_treeServers.SelectedNode;
                ChangeTraceDirectory(currNode.Tag as ServerRecord);
            }
            else if (_activeControl != null)
                _activeControl.AgentTraceDirectory();
        }

        private void AgentPropertiesAction()
        {
            if (_treeServers.Focused)
            {
                SQLcomplianceTreeNode currNode = (SQLcomplianceTreeNode)_treeServers.SelectedNode;
                ShowAgentProperties(currNode.Tag as ServerRecord);
            }
            else if (_activeControl != null)
                _activeControl.AgentProperties();
        }

        private void HelpOnThisWindowAction()
        {
            if (_activeControl != null)
                _activeControl.HelpOnThisWindow();
        }

        private void ArchiveNowAction()
        {
            string instance = GetCurrentInstance();

            Form_Archive frm = new Form_Archive(instance);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                // We assume an archive was attached in case one was.
                RaiseArchiveAttached(null);
                RefreshAction();
            }
        }

        private void GroomNowAction()
        {
            string instance = this.GetCurrentInstance();
            Form_Groom frm = new Form_Groom(instance);
            frm.ShowDialog();
        }

        private void ArchivePreferencesAction()
        {
            Form_ArchiveOptions frm = new Form_ArchiveOptions();
            frm.ShowDialog();
        }

        private void EnableAction()
        {
            if (_treeServers.Focused)
            {
                SQLcomplianceTreeNode node = _treeServers.SelectedNode as SQLcomplianceTreeNode;
                if (node == null)
                    return;
                if (node.Type == CMNodeType.Server)
                {
                    EnableServerAction(node.Tag as ServerRecord);
                }
                else if (node.Type == CMNodeType.Database)
                {
                    EnableDatabaseAction(node.Tag as DatabaseRecord);
                }
            }
            else if (_activeControl != null)
                _activeControl.Enable(true);
        }

        private void DisableAction()
        {
            if (_treeServers.Focused)
            {
                SQLcomplianceTreeNode node = _treeServers.SelectedNode as SQLcomplianceTreeNode;
                if (node == null)
                    return;
                if (node.Type == CMNodeType.Server)
                {
                    DisableServerAction(node.Tag as ServerRecord);
                }
                else if (node.Type == CMNodeType.Database)
                {
                    DisableDatabaseAction(node.Tag as DatabaseRecord);
                }
            }
            else if (_activeControl != null)
                _activeControl.Enable(false);
        }

        private void PreferencesAction()
        {
            Form_Preferences dlg = new Form_Preferences();
            if (DialogResult.OK == dlg.ShowDialog())
            {
                if (_activeControl == _ribbonTabView)
                {
                    _activeControl.RefreshView();
                }
            }
        }

        private void RefreshAction()
        {
            Cursor = Cursors.WaitCursor;

            if (_treeServers.Focused && _treeServers.SelectedNode.Tag == null)
            {
                BuildServerTree();
            }
            else if (_treeServers.Focused)
            {
                RefreshServerRecords();
            }

            if (_activeControl != null)
            {
                _activeControl.RefreshView();
            }

            Cursor = Cursors.Default;
        }

        private void PropertiesAction()
        {
            if (_treeServers.Focused)
            {
                SQLcomplianceTreeNode node = _treeServers.SelectedNode as SQLcomplianceTreeNode;
                if (node == null)
                    return;
                if (node.Type == CMNodeType.Server)
                {
                    ShowServerProperties(node.Tag as ServerRecord);
                }
                else if (node.Type == CMNodeType.Database)
                {
                    ShowDatabaseProperties(node.Tag as DatabaseRecord);
                    _ribbonTabView.SetScope(node.Tag as DatabaseRecord);
                }
            }
            else if (_activeControl != null)
                _activeControl.Properties();
        }

        private void ImportCsvAction()
        {
            if (!_treeServers.Focused)
                return;

            var node = _treeServers.SelectedNode as SQLcomplianceTreeNode;
            if (node == null)
                return;

            if (node.Type != CMNodeType.Server)
                return;

            var formImportFromCsv = new Form_ImportSensitiveColumnsFromCSV(node.Tag as ServerRecord);
            formImportFromCsv.ShowDialog(this);
        }

        private void RemoveAction()
        {
            if (_treeServers.Focused)
            {
                object tag = _treeServers.SelectedNode.Tag;
                if (tag is ServerRecord)
                {
                    RemoveServerAction(tag as ServerRecord);
                }
                else if (tag is DatabaseRecord)
                {
                    RemoveDatabaseAction(tag as DatabaseRecord);
                }
            }
            else if (_activeControl != null)
                _activeControl.Delete();
        }

        private void ManageLicenseAction()
        {
            Form_License frm = new Form_License();

            DialogResult choice = frm.ShowDialog();
            if (choice == DialogResult.OK)
            {
                UpdateTitleBar();
                UpdateLicenseState();
            }
        }

        public void ExportServerAuditSettingsAction(ServerRecord server)
        {
            if (server != null)
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
                dlg.FilterIndex = 1;
                dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                dlg.FileName = String.Format("{0}_AuditSettings.xml", server.Instance).Replace('\\', '_');
                dlg.Title = "Export Server Audit Settings";

                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    //Dump the data
                    InstanceTemplate tmpl = new InstanceTemplate();
                    tmpl.RepositoryServer = Globals.RepositoryServer;
                    tmpl.ImportAuditSettings(server.Instance);
                    tmpl.Save(dlg.FileName);
                }
            }
        }

        public void ExportDatabaseAuditSettingsAction(DatabaseRecord db)
        {
            if (db != null)
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
                dlg.FilterIndex = 1;
                dlg.InitialDirectory = string.IsNullOrEmpty(m_currentExportingDirectory)
                     ? Environment.GetFolderPath(Environment.SpecialFolder.Personal) : m_currentExportingDirectory;
                dlg.FileName = String.Format("{0}_{1}_AuditSettings.xml", db.SrvInstance, db.Name).Replace('\\', '_');
                dlg.Title = "Export Database Audit Settings";

                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    m_currentExportingDirectory = Path.GetDirectoryName(dlg.FileName);
                    //Dump the data
                    InstanceTemplate tmpl = new InstanceTemplate();
                    tmpl.RepositoryServer = Globals.RepositoryServer;
                    tmpl.ExportDatabaseAuditSettings(db);
                    tmpl.Save(dlg.FileName);
                }
            }
        }

        private void BeforeToolDropdown_toolbarManager(object sender, BeforeToolDropdownEventArgs e)
        {
            IMenuFlags flags;
            if (_activeTree.Focused)
            {
                flags = (SQLcomplianceTreeNode)_activeTree.SelectedNode;
            }
            else
            {
                flags = _activeControl;
            }

            if (e.Tool.Key == "mainFile")
            {
                if (!Globals.SQLcomplianceConfig.LicenseObject.IsProductLicensed())
                {
                    _toolbarManager.Tools["fileNew"].SharedProps.Enabled = false;
                    _toolbarManager.Tools["fileImport"].SharedProps.Enabled = false;
                    _toolbarManager.Tools["fileExport"].SharedProps.Enabled = false;
                }
                else
                {
                    _toolbarManager.Tools["fileNew"].SharedProps.Enabled = Globals.isAdmin;
                    _toolbarManager.Tools["fileImport"].SharedProps.Enabled = Globals.isAdmin;
                    _toolbarManager.Tools["fileExport"].SharedProps.Enabled = Globals.isAdmin;

                    if ((_activeTree == _treeServers && _activeTree.SelectedNode != null &&
                       _activeTree.SelectedNode.Tag != null && _activeTree.SelectedNode.Tag.ToString() != "All")
                       || (_activeControl == _adminRibbonView && _adminRibbonView.GetActiveTab() == AdminRibbonView.Tabs.RegisteredServers))
                    {
                        _toolbarManager.Tools["fileExportAuditSettings"].SharedProps.Enabled = true;
                    }
                    else
                    {
                        _toolbarManager.Tools["fileExportAuditSettings"].SharedProps.Enabled = false;
                    }
                }
                _toolbarManager.Tools["filePrint"].SharedProps.Visible = false;
            }
            else if (e.Tool.Key == "mainEdit")
            {
                _toolbarManager.Tools["editRemove"].SharedProps.Enabled =
                   Globals.isAdmin && flags.GetMenuFlag(CMMenuItem.NewDatabase) && (flags.GetMenuFlag(CMMenuItem.Delete) ||
                   flags.GetMenuFlag(CMMenuItem.DetachArchive));

                if (flags.GetMenuFlag(CMMenuItem.DetachArchive))
                {
                    _toolbarManager.Tools["editRemove"].InstanceProps.Caption = "&Detach";
                }
                else
                {
                    _toolbarManager.Tools["editRemove"].SharedProps.Caption = "&Remove";
                }
                _toolbarManager.Tools["editProperties"].SharedProps.Enabled = flags.GetMenuFlag(CMMenuItem.Properties);
                //SQLCM-4963
                if (_treeServers.SelectedNode != null)
                {
                    SQLcomplianceTreeNode currNode = (SQLcomplianceTreeNode)_treeServers.SelectedNode;
                    if (currNode.Tag.GetType() != typeof(String))
                    {
                        object tag = currNode.Tag;
                        if (tag is ServerRecord)
                        {
                            ServerRecord server = (ServerRecord)currNode.Tag;
                            if (!server.IsAuditedServer)
                            {
                                _toolbarManager.Tools["editRemove"].SharedProps.Enabled = false;
                                _toolbarManager.Tools["editProperties"].SharedProps.Enabled = false;
                            }
                        }
                    }
                }
            }
            else if (e.Tool.Key == "mainView")
            {
                _toolbarManager.Tools["viewRefresh"].SharedProps.Enabled = flags.GetMenuFlag(CMMenuItem.Refresh);
                ((StateButtonTool)_toolbarManager.Tools["viewToolbar"]).InitializeChecked(Settings.Default.ViewToolbar);
            }
            else if (e.Tool.Key == "mainAuditing")
            {
                _toolbarManager.Tools["auditingArchive"].SharedProps.Enabled = Globals.isAdmin;
                _toolbarManager.Tools["auditingCaptureSnapshot"].SharedProps.Enabled = Globals.isAdmin;
                _toolbarManager.Tools["auditingCheckIntegrity"].SharedProps.Enabled = Globals.isAdmin;
                _toolbarManager.Tools["auditingCollectNow"].SharedProps.Enabled = Globals.isAdmin && flags.GetMenuFlag(CMMenuItem.ForceCollection);
                _toolbarManager.Tools["auditingImportCSV"].SharedProps.Enabled = Globals.isAdmin && flags.GetMenuFlag(CMMenuItem.ForceCollection);
                if (!Globals.SQLcomplianceConfig.LicenseObject.IsProductLicensed())
                {
                    _toolbarManager.Tools["auditingEnableAuditing"].SharedProps.Enabled = false;
                    _toolbarManager.Tools["auditingDisableAuditing"].SharedProps.Enabled = false;
                }
                else
                {
                    _toolbarManager.Tools["auditingDisableAuditing"].SharedProps.Enabled = Globals.isAdmin && flags.GetMenuFlag(CMMenuItem.DisableAuditing);
                    _toolbarManager.Tools["auditingEnableAuditing"].SharedProps.Enabled = Globals.isAdmin && flags.GetMenuFlag(CMMenuItem.EnableAuditing);
                }
                _toolbarManager.Tools["auditingGroomNow"].SharedProps.Enabled = Globals.isAdmin && Globals.SQLcomplianceConfig.GroomEventAllow;
                //SQLCM-4963
                if (_treeServers.SelectedNode != null)
                {
                    SQLcomplianceTreeNode currNode = (SQLcomplianceTreeNode)_treeServers.SelectedNode;
                    if (currNode.Tag.GetType() != typeof(String))
                    {
                        object tag = currNode.Tag;
                        if (tag is ServerRecord)
                        {
                            ServerRecord server = (ServerRecord)currNode.Tag;
                            if (!server.IsAuditedServer)
                            {
                                _toolbarManager.Tools["auditingEnableAuditing"].SharedProps.Enabled = false;
                                _toolbarManager.Tools["auditingDisableAuditing"].SharedProps.Enabled = false;
                                _toolbarManager.Tools["auditingArchive"].SharedProps.Enabled = false;
                                _toolbarManager.Tools["auditingCaptureSnapshot"].SharedProps.Enabled = false;
                                _toolbarManager.Tools["auditingCollectNow"].SharedProps.Enabled = false;
                                _toolbarManager.Tools["auditingImportCSV"].SharedProps.Enabled = false;
                                _toolbarManager.Tools["auditingPermissionsCheck"].SharedProps.Enabled = false;
                                _toolbarManager.Tools["auditingSnapshotPreferences"].SharedProps.Enabled = false;
                                _toolbarManager.Tools["auditingConfigureRepository"].SharedProps.Enabled = false;
                                _toolbarManager.Tools["auditingLoginFilter"].SharedProps.Enabled = false;
                                _toolbarManager.Tools["auditingCollectionServerStatus"].SharedProps.Enabled = false;
                            }
                        }
                    }
                }
            }
            else if (e.Tool.Key == "mainAlerting")
            {
                _toolbarManager.Tools["alertingConfigureEmail"].SharedProps.Enabled = Globals.isAdmin;
                _toolbarManager.Tools["alertingGroom"].SharedProps.Enabled = Globals.isAdmin;
            }
            else if (e.Tool.Key == "mainAgent")
            {
                _toolbarManager.Tools["agentStatus"].SharedProps.Enabled = Globals.isAdmin && flags.GetMenuFlag(CMMenuItem.CheckAgent);
                _toolbarManager.Tools["agentDeploy"].SharedProps.Enabled = Globals.isAdmin && flags.GetMenuFlag(CMMenuItem.DeployAgent);
                _toolbarManager.Tools["agentProperties"].SharedProps.Enabled = flags.GetMenuFlag(CMMenuItem.AgentProperties);
                _toolbarManager.Tools["agentChangeTraceDirectory"].SharedProps.Enabled = Globals.isAdmin && flags.GetMenuFlag(CMMenuItem.ChangeAgentTraceDir);
                _toolbarManager.Tools["agentUpdateAuditsettings"].SharedProps.Enabled = Globals.isAdmin && flags.GetMenuFlag(CMMenuItem.UpdateAuditSettings);
                _toolbarManager.Tools["agentUpgrade"].SharedProps.Enabled = Globals.isAdmin && flags.GetMenuFlag(CMMenuItem.UpgradeAgent);
            }
            else if (e.Tool.Key == "mainHelp")
            {
                _toolbarManager.Tools["helpWindow"].SharedProps.Enabled = (_activeControl != null);
            }
            else if (e.Tool.Key == "contextRoot")
            {
                _toolbarManager.Tools["fileNewServer"].SharedProps.Enabled = Globals.isAdmin;
                _toolbarManager.Tools["viewRefresh"].SharedProps.Enabled = true;
                _toolbarManager.Tools["fileExportAuditSettings"].SharedProps.Enabled = false;
            }
            else if (e.Tool.Key == "contextServer")
            {
                SQLcomplianceTreeNode currNode = (SQLcomplianceTreeNode)_treeServers.SelectedNode;
                ServerRecord server = (ServerRecord)currNode.Tag;

                _toolbarManager.Tools["fileNewDatabase"].SharedProps.Enabled = Globals.isAdmin;
                _toolbarManager.Tools["fileNewServer"].SharedProps.Enabled = Globals.isAdmin;
                _toolbarManager.Tools["editRemove"].SharedProps.Enabled = Globals.isSysAdmin;
                _toolbarManager.Tools["editProperties"].SharedProps.Enabled = true;
                _toolbarManager.Tools["viewRefresh"].SharedProps.Enabled = true;
                _toolbarManager.Tools["sensitiveColumnSearch"].SharedProps.Enabled = true;

                if (server.IsAuditedServer)
                {
                    _toolbarManager.Tools["agentUpdateAuditSettings"].SharedProps.Enabled = Globals.isAdmin;
                    _toolbarManager.Tools["auditingImportCSV"].SharedProps.Enabled = Globals.isAdmin;
                    _toolbarManager.Tools["auditingCollectNow"].SharedProps.Enabled = Globals.isAdmin;
                    _toolbarManager.Tools["auditingDisableAuditing"].SharedProps.Enabled = server.IsEnabled && Globals.isAdmin;
                    _toolbarManager.Tools["auditingEnableAuditing"].SharedProps.Enabled = !server.IsEnabled && Globals.isAdmin;
                    _toolbarManager.Tools["auditingImportCSV"].SharedProps.Enabled = Globals.isAdmin;

                    //only enable it if there are databases.
                    if (currNode.Nodes.Count > 0)
                        _toolbarManager.Tools["applyAuditTemplate"].SharedProps.Enabled = Globals.isAdmin;
                    else
                        _toolbarManager.Tools["applyAuditTemplate"].SharedProps.Enabled = false;
                    //SQLCM-4963
                    _toolbarManager.Tools["fileAttachArchive"].SharedProps.Enabled = Globals.isAdmin;
                    _toolbarManager.Tools["auditingPermissionsCheck"].SharedProps.Enabled = Globals.isAdmin;
                    _toolbarManager.Tools["agentProperties"].SharedProps.Enabled = Globals.isAdmin;
                }
                else
                {
                    _toolbarManager.Tools["agentUpdateAuditSettings"].SharedProps.Enabled = false;
                    _toolbarManager.Tools["auditingImportCSV"].SharedProps.Enabled = false;
                    _toolbarManager.Tools["auditingCollectNow"].SharedProps.Enabled = false;
                    _toolbarManager.Tools["auditingDisableAuditing"].SharedProps.Enabled = false;
                    _toolbarManager.Tools["auditingEnableAuditing"].SharedProps.Enabled = false;
                    _toolbarManager.Tools["applyAuditTemplate"].SharedProps.Enabled = false;
                    _toolbarManager.Tools["auditingImportCSV"].SharedProps.Enabled = false;
                    //SQLCM-4963
                    _toolbarManager.Tools["fileNewDatabase"].SharedProps.Enabled = false;
                    _toolbarManager.Tools["viewPreferences"].SharedProps.Enabled = false;
                    _toolbarManager.Tools["editProperties"].SharedProps.Enabled = false;
                    _toolbarManager.Tools["agentProperties"].SharedProps.Enabled = false;
                    _toolbarManager.Tools["auditingPermissionsCheck"].SharedProps.Enabled = false;
                    _toolbarManager.Tools["editRemove"].SharedProps.Enabled = false;
                    _toolbarManager.Tools["fileAttachArchive"].SharedProps.Enabled = false;
                }
            }
            else if (e.Tool.Key == "contextDatabase")
            {
                SQLcomplianceTreeNode currNode = (SQLcomplianceTreeNode)_treeServers.SelectedNode;
                ServerRecord server = (ServerRecord)currNode.Parent.Tag;
                DatabaseRecord database = (DatabaseRecord)currNode.Tag;

                _toolbarManager.Tools["editProperties"].SharedProps.Enabled = true;
                _toolbarManager.Tools["viewRefresh"].SharedProps.Enabled = true;
                _toolbarManager.Tools["auditingDisableAuditing"].SharedProps.Enabled = server.IsEnabled && database.IsEnabled && Globals.isAdmin;
                _toolbarManager.Tools["auditingEnableAuditing"].SharedProps.Enabled = server.IsEnabled && !database.IsEnabled && Globals.isAdmin;
                _toolbarManager.Tools["fileNewDatabase"].SharedProps.Enabled = Globals.isAdmin;
                _toolbarManager.Tools["editRemove"].SharedProps.Enabled = Globals.isSysAdmin;
                _toolbarManager.Tools["applyAuditTemplate"].SharedProps.Enabled = Globals.isAdmin;
                _toolbarManager.Tools["sensitiveColumnSearch"].SharedProps.Enabled = true;
            }
            else if (e.Tool.Key == "contextAdmin")
            {
                if (_activeControl == _adminRibbonView &&
                   _adminRibbonView.GetActiveTab() == AdminRibbonView.Tabs.RegisteredServers)
                {
                    _toolbarManager.Tools["fileExportAuditSettings"].SharedProps.Enabled = true;
                }
                else
                {
                    _toolbarManager.Tools["fileExportAuditSettings"].SharedProps.Enabled = false;
                }
            }
        }

        private void NodeMouseClick_treeServers(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                _activeTree.SelectedNode = e.Node;
            }
        }

        private void _toolbarManager_BeforeToolbarListDropdown(object sender, BeforeToolbarListDropdownEventArgs e)
        {
            e.ShowQuickAccessToolbarAddRemoveMenuItem = false;
            e.ShowQuickAccessToolbarPositionMenuItem = false;
            e.ShowMinimizeRibbonMenuItem = false;
        }
    }

    public class NeverVisitedLinkManager : IVisitedLinksManager
    {
        public void AddToVisitedList(string linkRef, string linkText, object context)
        {
        }

        public bool IsLinkVisited(string linkRef, string linkText, object context)
        {
            return false;
        }
    }

    public enum ConsoleViews
    {
        EnterpriseSummary,
        EnterpriseAlerts,
        ServerSummary,
        ServerAlerts,
        ServerEvents,
        ServerArchive,
        ServerChangeLog,
        DatabaseSummary,
        DatabaseEvents,
        DatabaseArchive,
        AdminRegisteredServers,
        AdminAlertRules,
        AdminEventFilters,
        AdminLogins,
        AdminActivityLog,
        AdminChangeLog,
        //start sqlcm 5.6- 5467
        AdminDefaultSettings
        //end sqlcm 5.6- 5467
    }
}
