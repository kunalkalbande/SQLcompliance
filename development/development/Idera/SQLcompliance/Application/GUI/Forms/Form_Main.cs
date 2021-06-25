using System ;
using System.Collections.Generic ;
using System.ComponentModel ;
using System.Data ;
using System.Diagnostics ;
using System.IO ;
using System.Net ;
using System.Reflection ;
using System.Windows.Forms ;
using Idera.SQLcompliance.Application.GUI.Controls ;
using Idera.SQLcompliance.Application.GUI.Helper ;
using Idera.SQLcompliance.Core ;
using Idera.SQLcompliance.Core.Agent ;
using Idera.SQLcompliance.Core.Collector ;
using Idera.SQLcompliance.Core.Reports ;
using Idera.SQLcompliance.Core.Rules ;
using Idera.SQLcompliance.Core.Rules.Alerts ;
using Idera.SQLcompliance.Core.Rules.Filters ;
using Idera.SQLcompliance.Core.Stats ;
using Idera.SQLcompliance.Core.Templates ;
using Infragistics.Win ;
using Infragistics.Win.UltraWinExplorerBar ;
using Microsoft.Win32 ;
using Qios.DevSuite.Components ;
using Resources=Idera.SQLcompliance.Application.GUI.Properties.Resources;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
   public partial class Form_Main : Form
   {
      private bool _connected ;

      private BaseControl _activeControl;
      private TreeView _activeTree;
      private AlertingConfiguration _alertingConfig ;
      private FiltersConfiguration _filteringConfig ;
      private Dictionary<StatsCategory, EnterpriseStatistics> _statistics ;

      private RecentlyUsedList _recentlyUsedList ;

      public event EventHandler<ServerEventArgs> ServerAdded;
      public event EventHandler<ServerEventArgs> ServerRemoved;
      public event EventHandler<ServerEventArgs> ServerModified;

      public event EventHandler<DatabaseEventArgs> DatabaseAdded;
      public event EventHandler<DatabaseEventArgs> DatabaseRemoved;
      public event EventHandler<DatabaseEventArgs> DatabaseModified;

      public event EventHandler<ArchiveEventArgs> ArchiveAttached;
      public event EventHandler<ArchiveEventArgs> ArchiveDetached;

      public event EventHandler<ConnectionChangedEventArgs> ConnectionChanged;

      public Form_Main()
      {
      }

      public void FireConnected()
      {
         // TODO:  Major hack, please clean this crap up
         RaiseConnectionChanged(new ConnectionChangedEventArgs(Globals.Repository));
         FetchGraphData() ;
      }

      void ColorsChanged_Global(object sender, EventArgs e)
      {
         UpdateColors() ;
      }

      #region Properties

      public bool Connected
      {
         get { return _connected ; }
         set { _connected = value ; }
      }

      public CMEventCategory[] EventCategories
      {
         get { return _alertingConfig.SqlServerCategories; }
      }


      #endregion

      private void RaiseServerAdded(ServerEventArgs args)
      {
         FetchGraphData();
         AddServerNode(args.Server);
         EventHandler<ServerEventArgs> temp = ServerAdded ;
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
         EventHandler<ConnectionChangedEventArgs> temp = ConnectionChanged;
         if (temp != null)
            temp(this, args);
      }

      #region Initialization

      private void BuildAdminTree()
      {
         _treeAdmin.Nodes.Clear();
         int imgIndex ;
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

         _treeAdmin.SelectedNode = _treeAdmin.Nodes[0] ;
      }

      // 
      // BuildServerTree()
      //
      // This function constructs the repository-specific server tree.  This should be done
      //  after a connection has been made to the repository
      //
      private void BuildServerTree()
      {
         int imgIndex ;
         SQLcomplianceTreeNode root;
         _treeServers.Nodes.Clear() ;
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
            AddServerNode(record) ;
            foreach (DatabaseRecord dbRecord in DatabaseRecord.GetDatabases(Globals.Repository.Connection, record.SrvId))
            {
               AddDatabaseNode(dbRecord);
            }
         }
         root.Expand() ;
         _treeServers.Nodes.Add(new TreeNode("Search")) ;
         _treeServers.SelectedNode = root;
      }

      private void BuildReportTree()
      {
         _treeReports.Nodes.Clear();
         int imgIndex;
         SQLcomplianceTreeNode root;
         CMReportInfo[] reports ;

         imgIndex = (int)AppIcons.Img16.ReportServer;
         root = new SQLcomplianceTreeNode("Audit Reports Categories", imgIndex, imgIndex, CMNodeType.ReportCategoryRoot);
         root.Tag = "Audit Reports";
         root.SetMenuFlag(CMMenuItem.Refresh);
         _treeReports.Nodes.Add(root);
         _treeReports.SelectedNode = root;
         try
         {
            FileInfo fInfo = new FileInfo(Assembly.GetExecutingAssembly().Location) ;
            string filePath = Path.Combine(fInfo.DirectoryName, CMReport.RelativeReportsPath);
            reports = CMReport.GetAvailableReports(Path.Combine(filePath, CMReport.RDLXmlFileName));
         }
         catch (Exception e)
         {
            ErrorLog.Instance.Write("Unable to load reports.", e);
            return;
         }

         Dictionary<ReportCategory, SQLcomplianceTreeNode> categories = new Dictionary<ReportCategory, SQLcomplianceTreeNode>();

         foreach (CMReportInfo info in reports)
         {
            SQLcomplianceTreeNode catNode;
            SQLcomplianceTreeNode rptNode;
            ReportCategoryInfo catInfo;

            if (categories.ContainsKey(info.Category))
            {
               catNode = categories[info.Category];
               catInfo = (ReportCategoryInfo)catNode.Tag;
            }
            else
            {
               imgIndex = (int)AppIcons.Img16.ReportServer;
               catNode =
                  new SQLcomplianceTreeNode(CMReport.GetCategoryName(info.Category),
                                             imgIndex,
                                             imgIndex,
                                             CMNodeType.ReportCategory);
               catInfo = new ReportCategoryInfo(info.Category);
               catNode.Tag = catInfo;
               catNode.SetMenuFlag(CMMenuItem.Refresh);

               categories.Add(info.Category, catNode);
               root.Nodes.Add(catNode);
            }

            catInfo.AddReport(info);
            rptNode = new SQLcomplianceTreeNode(info.DisplayName, imgIndex, imgIndex, CMNodeType.Report);
            rptNode.Tag = info;
            rptNode.SetMenuFlag(CMMenuItem.LoadReport);
            catNode.Nodes.Add(rptNode);
         }
         root.Expand();
      }

      #endregion

      private void InitializeUI()
      {
         //------------------------------------------------
         // Connection good - Lots of initialization to do
         //------------------------------------------------
         // Alerting Initialization
         _alertingConfig = new AlertingConfiguration() ;
         _alertingConfig.Initialize(Globals.RepositoryServer) ;
         _alertRulesView.Configuration = _alertingConfig ;
         _mainTabView.AlertConfiguration = _alertingConfig ;

         // Filtering Initialization
         _filteringConfig = new FiltersConfiguration() ;
         _filteringConfig.Initialize(Globals.RepositoryServer) ;
         _eventFiltersView.Configuration = _filteringConfig ;


         //   Adjust UI - tree; launch pad; etc
         bool evaluationVersion = true ;
         this.Text = String.Format("{0} ({1})", UIConstants.AppTitle, Globals.RepositoryServer) ;
         if(evaluationVersion)
            this.Text += " [Evaluation Version]" ;

         //   update LRU list
         LoadRUList() ;
         BuildToolsMenu();

         // Clear and Refill Tree
         BuildServerTree() ;
         BuildReportTree();
         BuildAdminTree();
         _activeTree = _treeServers;

         _treeServers.SelectedNode = _treeServers.Nodes[0] ;
         _treeServers.Focus() ;

         // Initialize report views
         _reportCategoryView.ReportsTree = _treeReports;
         _reportView.ServerInstance = Globals.RepositoryServer;

         UpdateColors();
      }

      private void FetchGraphData()
      {
         _statistics = new Dictionary<StatsCategory,EnterpriseStatistics>() ;
         List<ServerRecord> servers;
         servers = ServerRecord.GetServers(Globals.Repository.Connection, true);
         DateTime endDate = DateTime.UtcNow;
         DateTime startDate = endDate.AddDays(-31);
         EnterpriseStatistics stats ;

         stats = StatsExtractor.GetEnterpriseStatistics(Globals.Repository.Connection, StatsCategory.PrivUserEvents, servers, startDate, endDate);
         stats.Name = "Privileged User Activity";
         _statistics.Add(StatsCategory.PrivUserEvents, stats) ;

         stats = StatsExtractor.GetEnterpriseStatistics(Globals.Repository.Connection, StatsCategory.Alerts, servers, startDate, endDate);
         stats.Name = "Alert Activity";
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
         stats.Name = "Total Activity";
         _statistics.Add(StatsCategory.EventProcessed, stats);
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
         DateTime endDate = StatsDAL.CreateStatsIntervalTime(DateTime.UtcNow);
         if (endDate > stats.LastUpdated)
         {
            List<ServerRecord> servers;
            servers = ServerRecord.GetServers(Globals.Repository.Connection, false);

            // Add a minute to avoid a key collision on the endDate data point
            StatsExtractor.UpdateEnterpriseStatistics(Globals.Repository.Connection, stats, servers, 
               stats.LastUpdated.AddMinutes(1.0), endDate) ;
         }
      }

      private void UpdateColors()
      {
         AppearanceData aData = new AppearanceData();
         AppearancePropFlags aPropFlags = AppearancePropFlags.AllRender;
         _explorerBar.Groups[0].ResolveHeaderAppearance(ref aData, ref aPropFlags, false, false, true, false, true, true);

//         _mainTabView.SetAppearance(aData);
//         _alertRulesView.SetAppearance(aData);
//         _loginsView.SetAppearance(aData);
//         _eventFiltersView.SetAppearance(aData);
//         _registeredServerView.SetAppearance(aData);
//         _changeLogView.SetAppearance(aData);
//         _activityLogView.SetAppearance(aData);
//         _reportCategoryView.SetAppearance(aData);
//         _reportView.SetAppearance(aData);

         
         ProfessionalColorTable t = new ProfessionalColorTable();
         _splitContainer.BackColor = t.ToolStripContentPanelGradientEnd;
         _toolStripContainer.ContentPanel.BackColor = t.ToolStripContentPanelGradientEnd;
      }

      private bool ValidateConnection()
      {
         // check SQLcompliance DB schema
         if(!SQLcomplianceConfiguration.IsCompatibleSchema(Globals.Repository.Connection))
         {
            ErrorMessage.Show(UIConstants.Title_ConnectToServer,
                              UIConstants.Error_UnsupportedRepositoryVersion,
                              SQLcomplianceConfiguration.GetLastError()) ;
            return false ;
         }

         //--------------------------------------------------------
         // Check if user has privs by reading configuration table
         // also check for supported database schema
         //--------------------------------------------------------
         if(!Globals.ReadConfiguration())
         {
            ErrorMessage.Show(UIConstants.Title_ConnectToServer,
                              UIConstants.Error_CantReadRepository,
                              SQLcomplianceConfiguration.GetLastError()) ;
            return false ;
         }

         Globals.RepositoryServer = Globals.Repository.Instance ;
         Repository.ServerInstance = Globals.Repository.Instance ;
         _connected = true ;

         // TODO:  License code

         // Read server settings
         Globals.isSysAdmin = RawSQL.IsCurrentUserSysadmin(Globals.Repository.Connection) ;

         if(Globals.SQLcomplianceConfig.LicenseKey == "" ||
            !Globals.SQLcomplianceConfig.LicenseObject.IsValid() ||
            Globals.SQLcomplianceConfig.LicenseObject.IsExpired())
         {
            Globals.isAdmin = false ;
         }
         else
         {
            Globals.isAdmin = Globals.isSysAdmin ;
         }

         // Warn for about to expire or expired license
         int daysLeft = Globals.SQLcomplianceConfig.LicenseObject.DaysUntilExpiration() ;

         if(Globals.SQLcomplianceConfig.LicenseKey == "")
         {
            MessageBox.Show(UIConstants.Info_NoLicense,
                            UIConstants.Title_NoLicense,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error) ;
         }
         else if(Globals.SQLcomplianceConfig.LicenseObject.IsExpired())
         {
            MessageBox.Show(UIConstants.Info_LicenseExpired,
                            UIConstants.Title_LicenseExpired,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error) ;
         }
         else if(daysLeft < 7)
         {
            MessageBox.Show(String.Format(UIConstants.Info_LicenseAboutToExpire,
                                          daysLeft,
                                          Globals.SQLcomplianceConfig.LicenseObject.Product.ExpirationDate.
                                             ToShortDateString()),
                            UIConstants.Title_LicenseAboutToExpire,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning) ;
         }

         return true ;
      }

      private void BuildToolsMenu()
      {
         IList<RegisteredTool> companyTools;
         IList<RegisteredTool> internalTools;
         ToolStripMenuItem menuEntry;
         ToolStripSeparator sep = null;

         _menuTools.DropDownItems.Clear();

         companyTools = ToolFinder.GetRegisteredTools(UIConstants.RegKeyIderaProducts);
         internalTools = ToolFinder.GetRegisteredTools(UIConstants.RegKeySQLcmTools);

         foreach (RegisteredTool tool in internalTools)
         {
            if (tool.IsValid)
            {
               menuEntry = new ToolStripMenuItem(tool.Name, null, new EventHandler(tool.LaunchEvent));
               _menuTools.DropDownItems.Add(menuEntry);
            }
         }
         if (_menuTools.DropDownItems.Count > 0)
         {
            sep = new ToolStripSeparator() ;
            sep.Visible = false;
            _menuTools.DropDownItems.Add(sep);
         }
         foreach (RegisteredTool tool in companyTools)
         {
            if (tool.IsValid)
            {
               if (sep != null)
                  sep.Visible = true;
               menuEntry = new ToolStripMenuItem(tool.Name, null, new EventHandler(tool.LaunchEvent));
               _menuTools.DropDownItems.Add(menuEntry);
            }
         }
         if (_menuTools.DropDownItems.Count == 0)
            _menuTools.Visible = false;
         else
            _menuTools.Visible = true;
      }

      private bool PromptForConnection()
      {
         bool bConnected = false ;
         bool bConnecting = true ;

         while(bConnecting)
         {
            Form_Connect dlg = new Form_Connect() ;
            if(dlg.ShowDialog() == DialogResult.OK)
            {
               // try to connect
               if(ConnectToServer(dlg.Server))
               {
                  bConnected = true ;
                  bConnecting = false ;
               }
               else
               {
                  dlg.BringToFront() ;
               }
            }
            else
            {
               bConnecting = false ;

               if(_connected && Globals.Repository.Connection.State != ConnectionState.Open)
               {
                  ReconnectToCurrentServer() ;
               }
            }
         }
         if(bConnected)
            RaiseConnectionChanged(new ConnectionChangedEventArgs(Globals.Repository));
         else
            RaiseConnectionChanged(new ConnectionChangedEventArgs(null));
         return bConnected ;
      }

      private bool ReconnectToCurrentServer()
      {
         return ConnectToServer(Globals.RepositoryServer) ;
      }

      //------------------------------------------------------------
      // ConnectToServer - Routine that creates connection to 
      //                   SQLcompliance repository and server and
      //                   updates RU list of connected servers
      //------------------------------------------------------------
      private bool ConnectToServer(string serverName)
      {
         bool bConnected ;

         if(serverName == null || serverName.Length == 0)
         {
            return false ;
         }

         if(serverName == "." || serverName.ToUpper() == "(LOCAL)")
         {
            serverName = Dns.GetHostName().ToUpper() ;
         }

         // establish SQLcompliance Repository Connection
         this.Cursor = Cursors.WaitCursor ;

         Form_Connecting dlg = new Form_Connecting(serverName) ;
         DialogResult dr = dlg.ShowDialog() ;
         bConnected = (dr == DialogResult.OK) ;

         this.Cursor = Cursors.Default ;

         if(!bConnected)
         {
            ErrorMessage.Show(UIConstants.Title_ConnectToServer,
                              String.Format(UIConstants.Error_CantConnectToServer, serverName),
                              Globals.Repository.GetLastError()) ;
            return false ;
         }else
         {
            _recentlyUsedList.Insert(serverName) ;
            SaveRUList() ;
         }

         return true ;
      }

      #region Load and Save RUList

      //------------------------------------------------------------
      // LoadRUList - Load SQLcompliance Management console data
      //                   persisted to HKEY_LOCAL__MACHINE key
      //------------------------------------------------------------
      private void LoadRUList()
      {
         RegistryKey rk = null ;
         RegistryKey rks = null ;
         try
         {
            rk = Registry.CurrentUser ;
            rks = rk.CreateSubKey(UIConstants.RegKeyGUI) ;

            _recentlyUsedList.ReadFromRegistry(rks, UIConstants.RegVal_RU) ;
         }
         catch(Exception)
         {
         }
         finally
         {
            if(rks != null) rks.Close() ;
            if(rk != null) rk.Close() ;
         }

         // Update menu
         _miFileRUSep.Visible = (_recentlyUsedList.Count > 0) ;
         _miFileServer1.Visible = false ;
         _miFileServer2.Visible = false;
         _miFileServer3.Visible = false;
         _miFileServer4.Visible = false;
         _miFileServer5.Visible = false;

         // RU Items
         ToolStripMenuItem mnu = null;
         for (int i = 0; i < _recentlyUsedList.Count; i++)
         {
            switch (i)
            {
               case 0: mnu = _miFileServer1; break;
               case 1: mnu = _miFileServer2; break;
               case 2: mnu = _miFileServer3; break;
               case 3: mnu = _miFileServer4; break;
               case 4: mnu = _miFileServer5; break;
            }
            if (mnu != null)
            {
               mnu.Text = String.Format("&{0} {1}", i + 1, _recentlyUsedList.GetItem(i));
               mnu.Visible = true;
            }
         }
      }

      //------------------------------------------------------------
      // SaveRUList - Persist SQLcompliance Management console data
      //                   to HKEY_LOCAL__MACHINE key
      //------------------------------------------------------------
      private void SaveRUList()
      {
         RegistryKey rk = null ;
         RegistryKey rks = null ;
         try
         {
            rk = Registry.CurrentUser ;
            rks = rk.CreateSubKey(UIConstants.RegKeyGUI) ;

            _recentlyUsedList.WriteToRegistry(rks, UIConstants.RegVal_RU) ;
         }
         catch(Exception)
         {
         }
         finally
         {
            if(rks != null) rks.Close() ;
            if(rk != null) rk.Close() ;
         }
      }

      #endregion

      private void Shown_Form_Main(object sender, EventArgs e)
      {
         // If our cnonection failed at startup (splash), then we need to prompt to be nice.
         if(!Globals.Repository.Connected)
            PromptForConnection() ;

         // At this point, we have a connection from splash or from prompt, or nothing
         // On nothing we disable app except Connect dialog
         if(Globals.Repository.Connected)
         {
            if(ValidateConnection())
            {
               InitializeUI() ;
            }
         }
      }

      private void AfterSelect_treeAdmin(object sender, TreeViewEventArgs e)
      {
         switch(e.Node.Text)
         {
            case "Registered SQL Servers":
               _activeControl = _registeredServerView;
               break ;
            case "Alert Rules":
               _activeControl = _alertRulesView;
               break;
            case "Audit Event Filters":
               _activeControl = _eventFiltersView;
               break;
            case "Logins":
               _activeControl = _loginsView;
               break;
            case "Activity Log":
               _activeControl = _activityLogView;
               break;
            case "Change Log":
               _activeControl = _changeLogView;
               break;
         }
         _explorerBar.Groups[2].Tag = _activeControl;
         if (_treeAdmin.Focused)
         {
            _activeControl.BringToFront();
            _activeControl.RefreshView();
         }
         UpdateToolbar() ;
      }

      internal void NewEventFilter()
      {
         NewEventFilter(null) ;
      }

      internal void NewEventFilter(EventFilter template)
      {
         Form_EventFilterWizard wizard = new Form_EventFilterWizard(template, _filteringConfig) ;

         if(wizard.ShowDialog(this) == DialogResult.OK)
         {
            try
            {
               if(!FiltersDal.InsertEventFilter(wizard.Filter, _filteringConfig.ConnectionString))
               {
                  MessageBox.Show(this, "Unable to create the new event filter.", "Error") ;
                  return ;
               }
               if(!wizard.Filter.IsValid)
               {
                  if(!wizard.Filter.HasTargetInstances || wizard.Filter.HasConditions)
                     MessageBox.Show(this, CoreConstants.Exception_IncompleteEventFilter, "Invalid Event Filter",
                                     MessageBoxButtons.OK, MessageBoxIcon.Warning) ;
                  else
                     MessageBox.Show(this, CoreConstants.Exception_InvalidEventFilter, "Invalid Event Filter",
                                     MessageBoxButtons.OK, MessageBoxIcon.Warning) ;
               }
            }
            catch(Exception e)
            {
               ErrorLog.Instance.Write("Failed to create event filter", e, true) ;
               MessageBox.Show(this, String.Format("Unable to create the new event filter.\r\nMessage: {0}", e.Message),
                               "Error") ;
               return ;
            }

            UpdateEventFilters() ;
            _eventFiltersView.RefreshView() ;
            string logString = String.Format("Name:  {0}\r\nDescription:  {1}\r\n\r\nFilter:  {2}",
                                             wizard.Filter.Name, wizard.Filter.Description,
                                             wizard.FilterText.Replace("\n", "\r\n")) ;
            LogRecord.WriteLog(Globals.Repository.Connection, LogType.EventFilterAdded, logString) ;
         }
      }

      public void UpdateEventFilters()
      {
         ServerManager srvManager ;
         string url ;

         // check for collection service - cant uninstall if it is down or unreachable
         try
         {
            url = String.Format("tcp://{0}:{1}/{2}",
                                Globals.SQLcomplianceConfig.Server,
                                Globals.SQLcomplianceConfig.ServerPort,
                                typeof(ServerManager).Name) ;

            srvManager = (ServerManager)Activator.GetObject(typeof(ServerManager), url) ;
            srvManager.UpdateEventFilters() ;
         }
         catch(Exception)
         {
            // TODO:  Should we alert the user when we can't talk to the collection server?
         }
      }

      internal void NewAlertRule()
      {
         NewAlertRule(null) ;
      }

      internal void NewAlertRule(AlertRule template)
      {
         Form_AlertRuleWizard wizard = new Form_AlertRuleWizard(template, _alertingConfig) ;

         if(wizard.ShowDialog(this) == DialogResult.OK)
         {
            try
            {
               if(!AlertingDal.InsertAlertRule(wizard.Rule, _alertingConfig.ConnectionString))
               {
                  MessageBox.Show(this, "Unable to create the new alert rule.", "Error") ;
                  return ;
               }
               if(!wizard.Rule.IsValid)
               {
                  MessageBox.Show(this, CoreConstants.Exception_InvalidAlertRule, "Invalid Alert Rule",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning) ;
               }
            }
            catch(Exception e)
            {
               ErrorLog.Instance.Write("Failed to create alert rule", e, true) ;
               MessageBox.Show(this, String.Format("Unable to create the new alert rule.\r\nMessage: {0}", e.Message),
                               "Error") ;
               return ;
            }

            UpdateAlertRules() ;
            _alertRulesView.RefreshView() ;
            string logString = String.Format("Name:  {0}\r\nDescription:  {1}\r\n\r\nRule:  {2}",
                                             wizard.Rule.Name, wizard.Rule.Description,
                                             wizard.RuleText.Replace("\n", "\r\n")) ;
            LogRecord.WriteLog(Globals.Repository.Connection, LogType.AlertRuleAdded, logString) ;
         }
      }

      public void UpdateAlertRules()
      {
         ServerManager srvManager ;
         string url ;

         // check for collection service - cant uninstall if it is down or unreachable
         try
         {
            url = String.Format("tcp://{0}:{1}/{2}",
                                Globals.SQLcomplianceConfig.Server,
                                Globals.SQLcomplianceConfig.ServerPort,
                                typeof(ServerManager).Name) ;

            srvManager = (ServerManager)Activator.GetObject(typeof(ServerManager), url) ;
            srvManager.UpdateAlertRules() ;
         }
         catch(Exception)
         {
            // TODO:  Should we alert the user when we can't talk to the collection server?
         }
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

      private SQLcomplianceTreeNode AddServerNode(ServerRecord server)
      {
         SQLcomplianceTreeNode root = _treeServers.Nodes[0] as SQLcomplianceTreeNode;
         if (root == null)
            return null;
         SQLcomplianceTreeNode newNode = FindNode(server.Instance, root.Nodes);
         if (newNode != null)
         {
            RefreshServerNode(server);
         }
         else
         {
            int imgIndex ;
            if(server.IsAuditedServer)
               imgIndex = server.IsEnabled ? (int)AppIcons.Img16.Server : (int)AppIcons.Img16.ServerDisabled ;
            else
               imgIndex = (int)AppIcons.Img16.ReportServer ;

            newNode = new SQLcomplianceTreeNode(server.Instance, imgIndex, imgIndex, CMNodeType.Server) ;
            newNode.Tag = server ;
            SetServerNodeMenuFlags(newNode, server);
            root.Nodes.Add(newNode) ;
         }

         return newNode;
      }

      private void SetServerNodeMenuFlags(SQLcomplianceTreeNode node, ServerRecord server)
      {
         node.SetMenuFlag(CMMenuItem.Refresh);
         node.SetMenuFlag(CMMenuItem.Properties);

         node.SetMenuFlag(CMMenuItem.AttachArchive);
         node.SetMenuFlag(CMMenuItem.NewServer);

         if (server.IsAuditedServer)
         {
            node.SetMenuFlag(CMMenuItem.Delete);
            node.SetMenuFlag(CMMenuItem.Enable);
            node.SetMenuFlag(CMMenuItem.Disable);
            node.SetMenuFlag(CMMenuItem.UpdateAuditSettings);
            node.SetMenuFlag(CMMenuItem.ForceCollection);
            node.SetMenuFlag(CMMenuItem.AgentProperties);
            node.SetMenuFlag(CMMenuItem.NewDatabase);
         }
      }

      private SQLcomplianceTreeNode AddDatabaseNode(DatabaseRecord database)
      {
         SQLcomplianceTreeNode root = _treeServers.Nodes[0] as SQLcomplianceTreeNode;
         if(root == null)
            return null ;

         SQLcomplianceTreeNode parent = FindNode(database.SrvInstance, root.Nodes);
         if (parent == null)
            return null;
         ServerRecord server = (ServerRecord)parent.Tag;

         int imgIndex = server.IsEnabled && database.IsEnabled ? 
            (int)AppIcons.Img16.Database : (int)AppIcons.Img16.DatabaseDisabled;
         SQLcomplianceTreeNode node = new SQLcomplianceTreeNode(database.Name, imgIndex, imgIndex, CMNodeType.Database);
         node.Tag = database;
         node.SetMenuFlag(CMMenuItem.Refresh);
         node.SetMenuFlag(CMMenuItem.Properties);
         node.SetMenuFlag(CMMenuItem.Delete);
         node.SetMenuFlag(CMMenuItem.Enable);
         node.SetMenuFlag(CMMenuItem.Disable);
         node.SetMenuFlag(CMMenuItem.NewDatabase);
         parent.Nodes.Add(node);
         return node;
      }

      //------------------------------------------------------------------
      // RefreshServerNode
      //------------------------------------------------------------------
      public void RefreshServerNode(ServerRecord server)
      {
         SQLcomplianceTreeNode root = _treeServers.Nodes[0] as SQLcomplianceTreeNode;
         if (root == null)
            return ;

         SQLcomplianceTreeNode node = FindNode(server.Instance, root.Nodes);
         if (node != null)
         {
            int imgIndex;
            if (server.IsAuditedServer)
               imgIndex = server.IsEnabled ? (int)AppIcons.Img16.Server : (int)AppIcons.Img16.ServerDisabled;
            else
               imgIndex = (int)AppIcons.Img16.ReportServer;
            node.ImageIndex = imgIndex;
            node.SelectedImageIndex = imgIndex;
            node.Tag = server;
            foreach(TreeNode childNode in node.Nodes)
            {
               DatabaseRecord db = childNode.Tag as DatabaseRecord;
               childNode.ImageIndex = server.IsEnabled && db.IsEnabled ?(int)AppIcons.Img16.Database : (int)AppIcons.Img16.DatabaseDisabled;
               childNode.SelectedImageIndex = childNode.ImageIndex;
            }
            SetServerNodeMenuFlags(node, server);
            if (node.IsSelected)
            {
               _mainTabView.SetScope(server);
               _mainTabView.RefreshView() ;
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
            return ;

         SQLcomplianceTreeNode parent = FindNode(database.SrvInstance, root.Nodes);
         if (parent == null)
            return ;
         ServerRecord server = (ServerRecord)parent.Tag;

         parent = FindNode(database.Name, parent.Nodes);
         if (parent != null)
         {
            parent.ImageIndex = server.IsEnabled && database.IsEnabled ?
            (int)AppIcons.Img16.Database : (int)AppIcons.Img16.DatabaseDisabled;
            parent.SelectedImageIndex = parent.ImageIndex;
         }
      }

      public void RemoveServerNode(ServerRecord server)
      {
         SQLcomplianceTreeNode root = _treeServers.Nodes[0] as SQLcomplianceTreeNode;
         if (root == null)
            return ;

         SQLcomplianceTreeNode node = FindNode(server.Instance, root.Nodes);
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

         SQLcomplianceTreeNode parent = FindNode(database.SrvInstance, root.Nodes);
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

      public void NewLoginAction()
      {
         Form_LoginNew dlg = new Form_LoginNew();
         if (DialogResult.OK == dlg.ShowDialog())
         {
            if (_activeControl == _loginsView)
               _loginsView.RefreshView();
         }
      }

      private string GetCurrentInstance()
      {
         string instance = "";

         SQLcomplianceTreeNode currNode = (SQLcomplianceTreeNode)_treeServers.SelectedNode;
         if (currNode.Type == CMNodeType.AuditServer)
         {
            instance = currNode.Text;
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
         ServerManager srvManager;
         string url;

         // check for collection service - cant uninstall if it is down or unreachable
         try
         {
            url = String.Format("tcp://{0}:{1}/{2}",
               Globals.SQLcomplianceConfig.Server,
               Globals.SQLcomplianceConfig.ServerPort,
               typeof(ServerManager).Name);

            srvManager = (ServerManager)Activator.GetObject(typeof(ServerManager), url);
            srvManager.UpdateAlertingConfiguration();
         }
         catch (Exception)
         {
            // TODO:  Should we alert the user when we can't talk to the collection server?
         }
      }


      #region Menu Commands

      private void Click_miFileConnect(object sender, EventArgs e)
      {
         PromptForConnection();
         InitializeUI() ;
      }

      private void Click_miFileNewServer(object sender, EventArgs e)
      {
         AddServerAction();
      }

      private void Click_miFileNewDatabase(object sender, EventArgs e)
      {
         AddDatabaseAction(null);
      }

      private void Click_miFileNewLogin(object sender, EventArgs e)
      {
         NewLoginAction();
      }

      private void Click_miFileNewAlertRule(object sender, EventArgs e)
      {
         NewAlertRule();
      }

      private void Click_miFileNewFilter(object sender, EventArgs e)
      {
         NewEventFilter();
      }

      private void Click_miFileAttachArchive(object sender, EventArgs e)
      {
         AttachArchiveAction();
      }

      private void Click_miFileLicenses(object sender, EventArgs e)
      {
         Form_LicenseManagement frm = new Form_LicenseManagement();
         DialogResult choice = frm.ShowDialog();
         if (choice == DialogResult.OK)
         {
            // reset license status to handle case where we went in with an expired license
            // and came out with a valid one
         }
      }

      private void Click_miFilePrint(object sender, EventArgs e)
      {
         // TODO:  Print()
      }

      private void Click_miFileRUServer(object sender, EventArgs e)
      {
         ToolStripMenuItem mnu;

         mnu = (ToolStripMenuItem)sender;

         // attempt to connect with current server
         if (!ConnectToServer(mnu.Text.Substring(3)))
         {
            ReconnectToCurrentServer();
         }

      }


      private void Click_miFileExit(object sender, EventArgs e)
      {
         Close();
      }

      private void Click_miEditRemove(object sender, EventArgs e)
      {
         if(_treeServers.Focused)
         {
            object tag = _treeServers.SelectedNode.Tag ;
            if (tag is ServerRecord)
            {
               RemoveServerAction(tag as ServerRecord);
            }
            else if (tag is DatabaseRecord)
            {
               RemoveDatabaseAction(tag as DatabaseRecord);
            }
         }else if(_activeControl != null) 
            _activeControl.Delete();
      }

      private void Click_miEditProperties(object sender, EventArgs e)
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
            }
         }
         else if (_activeControl != null)
            _activeControl.Properties();
      }

      private void Click_miViewSetFilter(object sender, EventArgs e)
      {
         if (_activeControl != null) 
            _activeControl.SetFilter();
      }

      private void Click_miViewConsoleTree(object sender, EventArgs e)
      {
         ShowConsoleTree(!_miViewConsoleTree.Checked);
      }

      private void Click_miViewToolbar(object sender, EventArgs e)
      {
         ShowToolbar(!_miViewToolbar.Checked);
         Globals.WritePreferences();
      }

      private void Click_miViewCommonTasks(object sender, EventArgs e)
      {
         ShowCommonTasks(!_miViewCommonTasks.Checked);
         Globals.WritePreferences();
      }

      private void Click_miViewTaskBanners(object sender, EventArgs e)
      {
         ShowBanner(!_miViewTaskBanners.Checked);
         Globals.WritePreferences();
      }

      private void Click_miViewCollapse(object sender, EventArgs e)
      {
         if (_activeControl != null) 
            _activeControl.CollapseAll();
      }

      private void Click_miViewExpand(object sender, EventArgs e)
      {
         if (_activeControl != null) 
            _activeControl.ExpandAll();
      }

      private void Click_miViewGroupBy(object sender, EventArgs e)
      {
         ShowGroupByColumn(!_miViewGroupBy.Checked);
         Globals.WritePreferences();
      }

      private void Click_miViewRefresh(object sender, EventArgs e)
      {
         Cursor = Cursors.WaitCursor;

         if (_treeServers.Focused && _treeServers.SelectedNode.Tag == null)
         {
            BuildServerTree();
         }

         if (_activeControl != null)
         {
            _activeControl.RefreshView();
         }

         Cursor = Cursors.Default;
      }

      private void Click_miViewPreferences(object sender, EventArgs e)
      {
         Form_Preferences dlg = new Form_Preferences();
         if (DialogResult.OK == dlg.ShowDialog())
         {
            if (_activeControl == _mainTabView)
            {
               _activeControl.RefreshView();
            }
         }
      }

      private void Click_miAuditingEnable(object sender, EventArgs e)
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

      private void Click_miAuditingDisable(object sender, EventArgs e)
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

      private void Click_miAuditingArchiveNow(object sender, EventArgs e)
      {
         string instance = GetCurrentInstance();

         Form_Archive frm = new Form_Archive(instance);
         frm.ShowDialog();
      }

      private void Click_miAuditingArchiveGroomNow(object sender, EventArgs e)
      {
         string instance = this.GetCurrentInstance();
         _registeredServerView.Groom(instance);
      }

      private void Click_miAuditingArchivePreferences(object sender, EventArgs e)
      {
         Form_ArchiveOptions frm = new Form_ArchiveOptions();
         frm.ShowDialog();
      }

      private void Click_miAuditingCollectNow(object sender, EventArgs e)
      {
         string instance = null;

         if (_treeServers.Focused)
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
            ServerRecord tmp = _registeredServerView.GetSelectedInstance();
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

      private void Click_miAuditingCheckIntegrity(object sender, EventArgs e)
      {
         string instance = this.GetCurrentInstance();
         CheckIntegrity(instance);
      }

      private void Click_miAuditingCaptureSnapshot(object sender, EventArgs e)
      {
         string instance = "";
         if (_treeServers.Focused)
         {
            SQLcomplianceTreeNode currNode = (SQLcomplianceTreeNode)_treeServers.SelectedNode;
            if (currNode.Type == CMNodeType.AuditServer)
               instance = currNode.Text;
         }
         DoSnapshot(instance);

         if (_activeControl == _mainTabView)
         {
            _activeControl.RefreshView();
         }
      }

      private void Click_miAuditingSnapshotPreferences(object sender, EventArgs e)
      {
         Form_SnapshotOptions frm = new Form_SnapshotOptions();
         frm.ShowDialog();
      }

      private void Click_miAuditingLoginFilters(object sender, EventArgs e)
      {
         Form_LoginFilterOptions frm = new Form_LoginFilterOptions();
         frm.ShowDialog();
      }

      private void Click_miAuditingCollectionServerStatus(object sender, EventArgs e)
      {
         Form_ServerOptions frm = new Form_ServerOptions();
         frm.ShowDialog();
      }

      private void Click_miAuditingConfigureRepository(object sender, EventArgs e)
      {
         Form_RepositoryOptions frm = new Form_RepositoryOptions();
         frm.ShowDialog();
      }

      private void Click_miAuditingConfigureReports(object sender, EventArgs e)
      {
         Form_Reporting frm = new Form_Reporting();
         frm.ShowDialog();
      }

      private void Click_miAlertingConfigureEmail(object sender, EventArgs e)
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

      private void Click_miAlertingGroom(object sender, EventArgs e)
      {
         Form_GroomAlerts form = new Form_GroomAlerts(_alertingConfig);

         form.ShowDialog(this);
      }

      private void Click_miAgentDeploy(object sender, EventArgs e)
      {
         if (_activeControl != null) 
            _activeControl.DeployAgent();
      }

      private void Click_miAgentUpgrade(object sender, EventArgs e)
      {
         if (_activeControl != null) 
            _activeControl.UpgradeAgent();
      }

      private void Click_miAgentCheckStatus(object sender, EventArgs e)
      {
         if (_treeServers.Focused)
         {
            SQLcomplianceTreeNode currNode = (SQLcomplianceTreeNode)_treeServers.SelectedNode;
            if(currNode != null && currNode.Tag is ServerRecord)
               CheckAgent(currNode.Tag as ServerRecord);
         }
         else if (_activeControl != null) _activeControl.CheckAgent();
      }

      private void Click_miAgentUpdateNow(object sender, EventArgs e)
      {
         if (_treeServers.Focused)
         {
            SQLcomplianceTreeNode currNode = (SQLcomplianceTreeNode)_treeServers.SelectedNode;
            if (currNode != null && currNode.Tag is ServerRecord)
               UpdateNow(currNode.Text);
         }
         else if (_activeControl != null) _activeControl.UpdateNow();
      }

      private void Click_miAgentTraceDirectory(object sender, EventArgs e)
      {
         if (_treeServers.Focused)
         {
            SQLcomplianceTreeNode currNode = (SQLcomplianceTreeNode)_treeServers.SelectedNode;
            ChangeTraceDirectory(currNode.Tag as ServerRecord);
         }
         else if (_activeControl != null) _activeControl.AgentTraceDirectory();
      }

      private void Click_miAgentProperties(object sender, EventArgs e)
      {
         if (_treeServers.Focused)
         {
            SQLcomplianceTreeNode currNode = (SQLcomplianceTreeNode)_treeServers.SelectedNode;
            ShowAgentProperties(currNode.Tag as ServerRecord);
         }
         else if (_activeControl != null) _activeControl.AgentProperties();
      }

      private void Click_miHelpWindow(object sender, EventArgs e)
      {
         if (_activeControl != null)
            _activeControl.HelpOnThisWindow();
      }

      private void Click_miHelpAuditing(object sender, EventArgs e)
      {
         HelpAlias.ShowHelp(this, HelpAlias.SSHELP_HowAuditingWorks);
      }

      private void Click_miHelpSecurity(object sender, EventArgs e)
      {
         HelpAlias.ShowHelp(this, HelpAlias.SSHELP_HowConsoleSecurityWorks);
      }

      private void Click_miHelpReports(object sender, EventArgs e)
      {
         HelpAlias.ShowHelp(this, HelpAlias.SSHELP_ReportingOnAuditData);
      }

      private void Click_miHelpAlerting(object sender, EventArgs e)
      {
         HelpAlias.ShowHelp(this, HelpAlias.SSHELP_AlertingGeneral);
      }

      private void Click_miHelpContents(object sender, EventArgs e)
      {
         HelpAlias.ShowHelp(this, HelpAlias.SSHELP_ViewContents);
      }

      private void Click_miHelpSearch(object sender, EventArgs e)
      {
         Process.Start(UIConstants.KnowledgeBaseHomePage);
      }

      private void Click_miHelpMore(object sender, EventArgs e)
      {
         Process.Start(UIConstants.SQLcomplianceHomePage);
      }

      private void Click_miHelpTechSupport(object sender, EventArgs e)
      {
         Process.Start(UIConstants.SupportHomePage);
      }

      private void Click_miHelpIderaProducts(object sender, EventArgs e)
      {
         Process.Start(UIConstants.IderaHomePage);
      }

      private void Click_miHelpAbout(object sender, EventArgs e)
      {
         AboutForm dlg = new AboutForm();
         dlg.ShowDialog();
      }

      #endregion Menu Commands

      private void AfterSelect_treeServers(object sender, TreeViewEventArgs e)
      {
         _activeControl = _mainTabView;
         if (e.Node.Tag is ServerRecord)
         {
            ServerRecord server = (ServerRecord)e.Node.Tag ;
            _treeServers.ContextMenuStrip = _contextMenuServer ;
            _mainTabView.SetScope(server) ;
            Globals.AlertsViewFilter.TargetServer = server.Instance;
            Globals.ChangeLogViewFilter.TargetServer = server.Instance;
         }
         else if (e.Node.Tag is DatabaseRecord)
         {
            _treeServers.ContextMenuStrip = _contextMenuDatabase;
            _mainTabView.SetScope(e.Node.Tag as DatabaseRecord);
         }
         else
         {
            _treeServers.ContextMenuStrip = _contextMenuRoot;
            _mainTabView.SetScope();
            Globals.AlertsViewFilter.TargetServer = null;
            Globals.ChangeLogViewFilter.TargetServer = null ;
         }
         _mainTabView.RefreshView() ;
         UpdateToolbar() ;
      }

      private void SelectedGroupChanged_explorerBar(object sender, GroupEventArgs e)
      {
         if(e.Group == _explorerBar.Groups[0])
         {
            // Explore Activity
            SQLcomplianceTreeNode selectedNode = _treeServers.SelectedNode as SQLcomplianceTreeNode;
            if (selectedNode.Tag is ServerRecord)
            {
               // We have to reset this.  There is a chance the ChangeLog was viewed on the 
               //  admin node, where it does not have a server filter
               ServerRecord server = (ServerRecord)selectedNode.Tag;
               Globals.ChangeLogViewFilter.TargetServer = server.Instance;
            }else
               Globals.ChangeLogViewFilter.TargetServer = null;
            _activeTree = _treeServers;
            _mainTabView.BringToFront();
            _mainTabView.RefreshView();
            _activeControl = _mainTabView ;

         }else if(e.Group == _explorerBar.Groups[1])
         {
            // Audit Reports
            _activeTree = _treeReports;
            TreeNode currentNode = _treeReports.SelectedNode;
            BaseControl currentView;
            if (currentNode.Tag is string || currentNode.Tag is ReportCategoryInfo)
            {
               currentView = _reportCategoryView;
            }
            else
            {
               currentView = _reportView;
            }
            currentView.BringToFront();
            currentView.RefreshView();
            _activeTree = _treeReports;
         }else if(e.Group == _explorerBar.Groups[2])
         {
            // Administration
            Globals.ChangeLogViewFilter.TargetServer = null;
            _activeTree = _treeAdmin;
            if (e.Group.Tag is BaseControl)
            {
               ((BaseControl)e.Group.Tag).BringToFront();
               ((BaseControl)e.Group.Tag).RefreshView();
            }
            else
            {
               // Default View
               _registeredServerView.BringToFront();
               _registeredServerView.RefreshView();
               e.Group.Tag = _registeredServerView;
            }
            _activeControl = e.Group.Tag as BaseControl;
         }
         UpdateToolbar() ;
      }

      private void DropDownOpening_MainMenu(object sender, EventArgs e)
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

         if (sender == _menuFile)
         {
            if (Globals.SQLcomplianceConfig.LicenseObject.IsExpired())
            {
               _miFileNewAlertRule.Enabled = false;
               _miFileNewDatabase.Enabled = false;
               _miFileNewLogin.Enabled = false;
               _miFileNewServer.Enabled = false;
            }
            else
            {
               _miFileNewAlertRule.Enabled = Globals.isAdmin;
               _miFileNewDatabase.Enabled = Globals.isAdmin;
               _miFileNewLogin.Enabled = Globals.isAdmin;
               _miFileNewServer.Enabled = Globals.isAdmin;
            }
            _miFilePrint.Enabled = flags.GetMenuFlag(CMMenuItem.Print);
         }
         else if (sender == _menuEdit)
         {
            _miEditRemove.Enabled = Globals.isAdmin && (flags.GetMenuFlag(CMMenuItem.Delete) ||
               flags.GetMenuFlag(CMMenuItem.DetachArchive));
            if (flags.GetMenuFlag(CMMenuItem.DetachArchive))
            {
               _miEditRemove.Text = "&Detach";
            }
            else
            {
               _miEditRemove.Text = "&Remove";
            }
            _miEditProperties.Enabled = flags.GetMenuFlag(CMMenuItem.Properties);
         }
         else if (sender == _menuView)
         {
            _miViewSetFilter.Enabled = flags.GetMenuFlag(CMMenuItem.SetFilter);
            _miViewCommonTasks.Enabled = flags.GetMenuFlag(CMMenuItem.ViewTasks);
            _miViewExpand.Enabled = flags.GetMenuFlag(CMMenuItem.Expand) && Globals.ViewGroupByColumn;
            _miViewCollapse.Enabled = flags.GetMenuFlag(CMMenuItem.Collapse) && Globals.ViewGroupByColumn;
            _miViewGroupBy.Enabled = flags.GetMenuFlag(CMMenuItem.GroupByColumn);
            _miViewRefresh.Enabled = flags.GetMenuFlag(CMMenuItem.Refresh);

            _miViewGroupBy.Checked = Globals.ViewGroupByColumn;
            _miViewTaskBanners.Checked = Globals.ViewBanners;
            _miViewCommonTasks.Checked = Globals.ViewCommonTasks;
            _miViewToolbar.Checked = Globals.ViewToolbar;
         }
         else if (sender == _menuAuditing)
         {
            _miAuditingArchive.Enabled = Globals.isAdmin;
            _miAuditingCaptureSnapshot.Enabled = Globals.isAdmin;
            _miAuditingCheckIntegrity.Enabled = Globals.isAdmin;
            _miAuditingCollectNow.Enabled = Globals.isAdmin && flags.GetMenuFlag(CMMenuItem.ForceCollection);
            if (Globals.SQLcomplianceConfig.LicenseObject.IsExpired())
            {
               _miAuditingDisable.Enabled = false;
               _miAuditingEnable.Enabled = false;
            }
            else
            {
               _miAuditingDisable.Enabled = Globals.isAdmin && flags.GetMenuFlag(CMMenuItem.Disable);
               _miAuditingEnable.Enabled = Globals.isAdmin && flags.GetMenuFlag(CMMenuItem.Enable);
            }
            _miAuditingArchiveGroomNow.Enabled = Globals.isAdmin && Globals.SQLcomplianceConfig.GroomEventAllow;
         }
         else if (sender == _menuAlerting)
         {
            _miAlertingConfigureEmail.Enabled = Globals.isAdmin;
            _miAlertingGroom.Enabled = Globals.isAdmin;
         }
         else if (sender == _menuAgent)
         {
            _miAgentCheckStatus.Enabled = Globals.isAdmin && flags.GetMenuFlag(CMMenuItem.CheckAgent);
            _miAgentDeploy.Enabled = Globals.isAdmin && flags.GetMenuFlag(CMMenuItem.DeployAgent);
            _miAgentProperties.Enabled = flags.GetMenuFlag(CMMenuItem.AgentProperties);
            _miAgentTraceDirectory.Enabled = Globals.isAdmin && flags.GetMenuFlag(CMMenuItem.ChangeAgentTraceDir);
            _miAgentUpdateNow.Enabled = Globals.isAdmin && flags.GetMenuFlag(CMMenuItem.UpdateAuditSettings);
            _miAgentUpgrade.Enabled = Globals.isAdmin && flags.GetMenuFlag(CMMenuItem.UpgradeAgent);
         }
         else if (sender == _menuHelp)
         {
            _miHelpWindow.Enabled = flags.GetMenuFlag(CMMenuItem.ShowHelp);
         }
      }

      #region Toolbar events

      private void ButtonClick_tsNew(object sender, EventArgs e)
      {
         

      }

      #endregion Toolbar events

      private void Opening_contextMenuServer(object sender, CancelEventArgs e)
      {
         SQLcomplianceTreeNode currNode = (SQLcomplianceTreeNode)_treeServers.SelectedNode;
         ServerRecord server = (ServerRecord)currNode.Tag;

         _cmServerNewDatabase.Enabled = Globals.isAdmin;
         _cmServerNewServer.Enabled = Globals.isAdmin;
         if (server.IsAuditedServer)
         {
            _cmServerUpdateNow.Enabled = Globals.isAdmin;
            _cmServerCollectNow.Enabled = Globals.isAdmin;
            _cmServerRemove.Enabled = Globals.isSysAdmin;
            _cmServerDisableAuditing.Enabled = server.IsEnabled && Globals.isAdmin;
            _cmServerEnableAuditing.Enabled = !server.IsEnabled && Globals.isAdmin;
         }
         else
         {
            _cmServerUpdateNow.Enabled = false;
            _cmServerCollectNow.Enabled = false;
            _cmServerRemove.Enabled = false;
            _cmServerDisableAuditing.Enabled = false;
            _cmServerEnableAuditing.Enabled = false;
         }
      }

      private void Opening_contextMenuRoot(object sender, CancelEventArgs e)
      {
         _cmRootNewServer.Enabled = Globals.isAdmin;
      }

      private void Opening_contextMenuDatabase(object sender, CancelEventArgs e)
      {
         SQLcomplianceTreeNode currNode = (SQLcomplianceTreeNode)_treeServers.SelectedNode;
         ServerRecord server = (ServerRecord)currNode.Parent.Tag;
         DatabaseRecord database = (DatabaseRecord)currNode.Tag;

         _cmDatabaseDisableAuditing.Enabled = server.IsEnabled && database.IsEnabled && Globals.isAdmin;
         _cmDatabaseEnableAuditing.Enabled = server.IsEnabled && !database.IsEnabled && Globals.isAdmin;
         _cmDatabaseNewDatabase.Enabled = Globals.isAdmin;
      }

      private void Opening_cmMenuAdminTree(object sender, CancelEventArgs e)
      {
         if (_activeControl == _eventFiltersView)
         {
            _cmExportSeparator.Visible = true;
            _cmExportAlertRules.Visible = false;
            _cmExportEventFilters.Visible = true;
         }
         else if (_activeControl == _alertRulesView)
         {
            _cmExportSeparator.Visible = true;
            _cmExportAlertRules.Visible = true;
            _cmExportEventFilters.Visible = false;
         }
         else
         {
            _cmExportSeparator.Visible = false;
            _cmExportAlertRules.Visible = false;
            _cmExportEventFilters.Visible = false;
         }
      }

      private void Click_cmExportAlertRules(object sender, EventArgs e)
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
            InstanceTemplate tmpl = new InstanceTemplate();
            tmpl.RepositoryServer = Globals.RepositoryServer;
            tmpl.ImportAlertRules();
            tmpl.Save(dlg.FileName);
         }
      }

      private void Click_emExportEventFilters(object sender, EventArgs e)
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
            InstanceTemplate tmpl = new InstanceTemplate();
            tmpl.RepositoryServer = Globals.RepositoryServer;
            tmpl.ImportEventFilters();
            tmpl.Save(dlg.FileName);
         }
      }

      private void Click_miFileImport(object sender, EventArgs e)
      {
         Form_AuditSettingsImport frm = new Form_AuditSettingsImport();
         frm.ShowDialog(this);
      }
      
      private void AfterSelect_treeReports(object sender, TreeViewEventArgs e)
      {
         BaseControl currentView;
         if (e.Node.Tag is string) //root
         {
            //_treeServers.ContextMenuStrip = _contextMenuServer;
            currentView = _reportCategoryView;
            _reportCategoryView.CategoryInfo = null;
         }
         else if (e.Node.Tag is ReportCategoryInfo) // Category node
         {
            currentView = _reportCategoryView;
            _reportCategoryView.CategoryInfo = (ReportCategoryInfo)e.Node.Tag;
         }
         else // Report node
         {
            currentView = _reportView;
            _reportView.ReportInfo = (CMReportInfo)e.Node.Tag;
         }
         _activeControl = currentView;
         _activeControl.BringToFront();
         _activeControl.RefreshView();
         UpdateToolbar();
      }

      private void FormClosed_Main(object sender, FormClosedEventArgs e)
      {
         Settings.Default.Save();
      }
   }


}