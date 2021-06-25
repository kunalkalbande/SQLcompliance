namespace Idera.SQLcompliance.Application.GUI.Forms
{
   partial class Form_Main
   {
      /// <summary>
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
      protected override void Dispose(bool disposing)
      {
         if (disposing && (components != null))
         {
            components.Dispose();
         }
         base.Dispose(disposing);
      }

      #region Windows Form Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.components = new System.ComponentModel.Container();
         Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup ultraExplorerBarGroup1 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup();
         Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
         Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
         Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup ultraExplorerBarGroup2 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup();
         Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
         Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
         Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup ultraExplorerBarGroup3 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup();
         Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
         Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
         this._containerExploreActivity = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarContainerControl();
         this._treeServers = new System.Windows.Forms.TreeView();
         this._containerAuditReports = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarContainerControl();
         this._treeReports = new System.Windows.Forms.TreeView();
         this.ultraExplorerBarContainerControl1 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarContainerControl();
         this._treeAdmin = new System.Windows.Forms.TreeView();
         this._contextMenuAdminTree = new System.Windows.Forms.ContextMenuStrip(this.components);
         this._cmNewServer = new System.Windows.Forms.ToolStripMenuItem();
         this._cmNewDatabase = new System.Windows.Forms.ToolStripMenuItem();
         this._cmNewLogin = new System.Windows.Forms.ToolStripMenuItem();
         this._cmNewAlertRule = new System.Windows.Forms.ToolStripMenuItem();
         this._cmNewEventFilter = new System.Windows.Forms.ToolStripMenuItem();
         this._cmExportSeparator = new System.Windows.Forms.ToolStripSeparator();
         this._cmExportAlertRules = new System.Windows.Forms.ToolStripMenuItem();
         this._cmExportEventFilters = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator25 = new System.Windows.Forms.ToolStripSeparator();
         this._cmRefresh = new System.Windows.Forms.ToolStripMenuItem();
         this._toolStripContainer = new System.Windows.Forms.ToolStripContainer();
         this._splitContainer = new System.Windows.Forms.SplitContainer();
         this._explorerBar = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBar();
         this._mainTabView = new Idera.SQLcompliance.Application.GUI.Controls.MainTabView();
         this._loginsView = new Idera.SQLcompliance.Application.GUI.Controls.LoginsView();
         this._eventFiltersView = new Idera.SQLcompliance.Application.GUI.Controls.EventFiltersView();
         this._alertRulesView = new Idera.SQLcompliance.Application.GUI.Controls.AlertRulesView();
         this._registeredServerView = new Idera.SQLcompliance.Application.GUI.Controls.ServerView();
         this._changeLogView = new Idera.SQLcompliance.Application.GUI.Controls.ChangeLogViewTab();
         this._activityLogView = new Idera.SQLcompliance.Application.GUI.Controls.ActivityLogViewTab();
         this._reportCategoryView = new Idera.SQLcompliance.Application.GUI.Controls.ReportCategoryView();
         this._reportView = new Idera.SQLcompliance.Application.GUI.Controls.ReportView();
         this._menuStripMain = new System.Windows.Forms.MenuStrip();
         this._menuFile = new System.Windows.Forms.ToolStripMenuItem();
         this._miFileConnect = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
         this._miFileNewServer = new System.Windows.Forms.ToolStripMenuItem();
         this._miFileNewDatabase = new System.Windows.Forms.ToolStripMenuItem();
         this._miFileNewLogin = new System.Windows.Forms.ToolStripMenuItem();
         this._miFileNewAlertRule = new System.Windows.Forms.ToolStripMenuItem();
         this._miFileNewFilter = new System.Windows.Forms.ToolStripMenuItem();
         this._miFileAttachArchive = new System.Windows.Forms.ToolStripMenuItem();
         this._miFileImport = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
         this._miFileLicenses = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
         this._miFilePrint = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
         this._miFileServer1 = new System.Windows.Forms.ToolStripMenuItem();
         this._miFileServer2 = new System.Windows.Forms.ToolStripMenuItem();
         this._miFileServer3 = new System.Windows.Forms.ToolStripMenuItem();
         this._miFileServer4 = new System.Windows.Forms.ToolStripMenuItem();
         this._miFileServer5 = new System.Windows.Forms.ToolStripMenuItem();
         this._miFileRUSep = new System.Windows.Forms.ToolStripSeparator();
         this._miFileExit = new System.Windows.Forms.ToolStripMenuItem();
         this._menuEdit = new System.Windows.Forms.ToolStripMenuItem();
         this._miEditRemove = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
         this._miEditProperties = new System.Windows.Forms.ToolStripMenuItem();
         this._menuView = new System.Windows.Forms.ToolStripMenuItem();
         this._miViewSetFilter = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
         this._miViewConsoleTree = new System.Windows.Forms.ToolStripMenuItem();
         this._miViewToolbar = new System.Windows.Forms.ToolStripMenuItem();
         this._miViewCommonTasks = new System.Windows.Forms.ToolStripMenuItem();
         this._miViewTaskBanners = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
         this._miViewCollapse = new System.Windows.Forms.ToolStripMenuItem();
         this._miViewExpand = new System.Windows.Forms.ToolStripMenuItem();
         this._miViewGroupBy = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
         this._miViewRefresh = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
         this._miViewPreferences = new System.Windows.Forms.ToolStripMenuItem();
         this._menuAuditing = new System.Windows.Forms.ToolStripMenuItem();
         this._miAuditingEnable = new System.Windows.Forms.ToolStripMenuItem();
         this._miAuditingDisable = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
         this._miAuditingArchive = new System.Windows.Forms.ToolStripMenuItem();
         this._miAuditingArchiveNow = new System.Windows.Forms.ToolStripMenuItem();
         this._miAuditingArchiveGroomNow = new System.Windows.Forms.ToolStripMenuItem();
         this._miAuditingArchivePreferences = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
         this._miAuditingCollectNow = new System.Windows.Forms.ToolStripMenuItem();
         this._miAuditingCheckIntegrity = new System.Windows.Forms.ToolStripMenuItem();
         this._miAuditingCaptureSnapshot = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
         this._miAuditingSnapshotPreferences = new System.Windows.Forms.ToolStripMenuItem();
         this._miAuditingLoginFilters = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
         this._miAuditingCollectionServerStatus = new System.Windows.Forms.ToolStripMenuItem();
         this._miAuditingConfigureRepository = new System.Windows.Forms.ToolStripMenuItem();
         this._miAuditingConfigureReports = new System.Windows.Forms.ToolStripMenuItem();
         this._menuAlerting = new System.Windows.Forms.ToolStripMenuItem();
         this._miAlertingConfigureEmail = new System.Windows.Forms.ToolStripMenuItem();
         this._miAlertingGroom = new System.Windows.Forms.ToolStripMenuItem();
         this._menuAgent = new System.Windows.Forms.ToolStripMenuItem();
         this._miAgentDeploy = new System.Windows.Forms.ToolStripMenuItem();
         this._miAgentUpgrade = new System.Windows.Forms.ToolStripMenuItem();
         this._miAgentCheckStatus = new System.Windows.Forms.ToolStripMenuItem();
         this._miAgentUpdateNow = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
         this._miAgentTraceDirectory = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator15 = new System.Windows.Forms.ToolStripSeparator();
         this._miAgentProperties = new System.Windows.Forms.ToolStripMenuItem();
         this._menuTools = new System.Windows.Forms.ToolStripMenuItem();
         this._menuHelp = new System.Windows.Forms.ToolStripMenuItem();
         this._miHelpWindow = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator16 = new System.Windows.Forms.ToolStripSeparator();
         this._miHelpAuditing = new System.Windows.Forms.ToolStripMenuItem();
         this._miHelpSecurity = new System.Windows.Forms.ToolStripMenuItem();
         this._miHelpReports = new System.Windows.Forms.ToolStripMenuItem();
         this._miHelpAlerting = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator17 = new System.Windows.Forms.ToolStripSeparator();
         this._miHelpContents = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator18 = new System.Windows.Forms.ToolStripSeparator();
         this._miHelpSearch = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator19 = new System.Windows.Forms.ToolStripSeparator();
         this._miHelpMore = new System.Windows.Forms.ToolStripMenuItem();
         this._miHelpTechSupport = new System.Windows.Forms.ToolStripMenuItem();
         this._miHelpIderaProducts = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator20 = new System.Windows.Forms.ToolStripSeparator();
         this._miHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
         this._toolStripMain = new System.Windows.Forms.ToolStrip();
         this._tsNew = new System.Windows.Forms.ToolStripSplitButton();
         this._tsNewServer = new System.Windows.Forms.ToolStripMenuItem();
         this._tsNewDatabase = new System.Windows.Forms.ToolStripMenuItem();
         this._tsNewLogin = new System.Windows.Forms.ToolStripMenuItem();
         this._tsNewAlertRule = new System.Windows.Forms.ToolStripMenuItem();
         this._tsNewFilter = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator21 = new System.Windows.Forms.ToolStripSeparator();
         this._tsPrint = new System.Windows.Forms.ToolStripButton();
         this._tsDelete = new System.Windows.Forms.ToolStripButton();
         this.toolStripSeparator22 = new System.Windows.Forms.ToolStripSeparator();
         this._tsProperties = new System.Windows.Forms.ToolStripButton();
         this._tsRefresh = new System.Windows.Forms.ToolStripButton();
         this.toolStripSeparator23 = new System.Windows.Forms.ToolStripSeparator();
         this._tsShowTree = new System.Windows.Forms.ToolStripButton();
         this._tsCollapse = new System.Windows.Forms.ToolStripButton();
         this._tsExpand = new System.Windows.Forms.ToolStripButton();
         this._tsGroupBy = new System.Windows.Forms.ToolStripButton();
         this._tsShowBanners = new System.Windows.Forms.ToolStripButton();
         this.toolStripSeparator24 = new System.Windows.Forms.ToolStripSeparator();
         this._tsHelp = new System.Windows.Forms.ToolStripButton();
         this._contextMenuServer = new System.Windows.Forms.ContextMenuStrip(this.components);
         this._cmServerNewServer = new System.Windows.Forms.ToolStripMenuItem();
         this._cmServerNewDatabase = new System.Windows.Forms.ToolStripMenuItem();
         this._cmServerAttachArchive = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator26 = new System.Windows.Forms.ToolStripSeparator();
         this._cmServerEnableAuditing = new System.Windows.Forms.ToolStripMenuItem();
         this._cmServerDisableAuditing = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator27 = new System.Windows.Forms.ToolStripSeparator();
         this._cmServerRemove = new System.Windows.Forms.ToolStripMenuItem();
         this._cmServerRefresh = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator28 = new System.Windows.Forms.ToolStripSeparator();
         this._cmServerUpdateNow = new System.Windows.Forms.ToolStripMenuItem();
         this._cmServerCollectNow = new System.Windows.Forms.ToolStripMenuItem();
         this._cmServerAgentProperties = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator29 = new System.Windows.Forms.ToolStripSeparator();
         this._cmServerProperties = new System.Windows.Forms.ToolStripMenuItem();
         this._contextMenuRoot = new System.Windows.Forms.ContextMenuStrip(this.components);
         this._cmRootNewServer = new System.Windows.Forms.ToolStripMenuItem();
         this._cmRootAttachArchive = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator31 = new System.Windows.Forms.ToolStripSeparator();
         this._cmRootRefresh = new System.Windows.Forms.ToolStripMenuItem();
         this._contextMenuDatabase = new System.Windows.Forms.ContextMenuStrip(this.components);
         this._cmDatabaseNewDatabase = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator34 = new System.Windows.Forms.ToolStripSeparator();
         this._cmDatabaseEnableAuditing = new System.Windows.Forms.ToolStripMenuItem();
         this._cmDatabaseDisableAuditing = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator35 = new System.Windows.Forms.ToolStripSeparator();
         this._cmDatabaseRefresh = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator36 = new System.Windows.Forms.ToolStripSeparator();
         this._cmDatabaseProperties = new System.Windows.Forms.ToolStripMenuItem();
         this._containerExploreActivity.SuspendLayout();
         this._containerAuditReports.SuspendLayout();
         this.ultraExplorerBarContainerControl1.SuspendLayout();
         this._contextMenuAdminTree.SuspendLayout();
         this._toolStripContainer.ContentPanel.SuspendLayout();
         this._toolStripContainer.TopToolStripPanel.SuspendLayout();
         this._toolStripContainer.SuspendLayout();
         this._splitContainer.Panel1.SuspendLayout();
         this._splitContainer.Panel2.SuspendLayout();
         this._splitContainer.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this._explorerBar)).BeginInit();
         this._explorerBar.SuspendLayout();
         this._menuStripMain.SuspendLayout();
         this._toolStripMain.SuspendLayout();
         this._contextMenuServer.SuspendLayout();
         this._contextMenuRoot.SuspendLayout();
         this._contextMenuDatabase.SuspendLayout();
         this.SuspendLayout();
         // 
         // _containerExploreActivity
         // 
         this._containerExploreActivity.Controls.Add(this._treeServers);
         this._containerExploreActivity.Location = new System.Drawing.Point(1, 26);
         this._containerExploreActivity.Name = "_containerExploreActivity";
         this._containerExploreActivity.Size = new System.Drawing.Size(223, 464);
         this._containerExploreActivity.TabIndex = 0;
         // 
         // _treeServers
         // 
         this._treeServers.BorderStyle = System.Windows.Forms.BorderStyle.None;
         this._treeServers.Dock = System.Windows.Forms.DockStyle.Fill;
         this._treeServers.HideSelection = false;
         this._treeServers.ItemHeight = 18;
         this._treeServers.Location = new System.Drawing.Point(0, 0);
         this._treeServers.Name = "_treeServers";
         this._treeServers.Size = new System.Drawing.Size(223, 464);
         this._treeServers.TabIndex = 0;
         this._treeServers.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.AfterSelect_treeServers);
         // 
         // _containerAuditReports
         // 
         this._containerAuditReports.Controls.Add(this._treeReports);
         this._containerAuditReports.Location = new System.Drawing.Point(-10000, -10000);
         this._containerAuditReports.Name = "_containerAuditReports";
         this._containerAuditReports.Size = new System.Drawing.Size(223, 464);
         this._containerAuditReports.TabIndex = 1;
         this._containerAuditReports.Visible = false;
         // 
         // _treeReports
         // 
         this._treeReports.BorderStyle = System.Windows.Forms.BorderStyle.None;
         this._treeReports.Dock = System.Windows.Forms.DockStyle.Fill;
         this._treeReports.HideSelection = false;
         this._treeReports.Location = new System.Drawing.Point(0, 0);
         this._treeReports.Name = "_treeReports";
         this._treeReports.Size = new System.Drawing.Size(223, 464);
         this._treeReports.TabIndex = 0;
         this._treeReports.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.AfterSelect_treeReports);
         // 
         // ultraExplorerBarContainerControl1
         // 
         this.ultraExplorerBarContainerControl1.Controls.Add(this._treeAdmin);
         this.ultraExplorerBarContainerControl1.Location = new System.Drawing.Point(-10000, -10000);
         this.ultraExplorerBarContainerControl1.Name = "ultraExplorerBarContainerControl1";
         this.ultraExplorerBarContainerControl1.Size = new System.Drawing.Size(223, 464);
         this.ultraExplorerBarContainerControl1.TabIndex = 2;
         this.ultraExplorerBarContainerControl1.Visible = false;
         // 
         // _treeAdmin
         // 
         this._treeAdmin.BorderStyle = System.Windows.Forms.BorderStyle.None;
         this._treeAdmin.ContextMenuStrip = this._contextMenuAdminTree;
         this._treeAdmin.Dock = System.Windows.Forms.DockStyle.Fill;
         this._treeAdmin.HideSelection = false;
         this._treeAdmin.ItemHeight = 18;
         this._treeAdmin.Location = new System.Drawing.Point(0, 0);
         this._treeAdmin.Name = "_treeAdmin";
         this._treeAdmin.Size = new System.Drawing.Size(223, 464);
         this._treeAdmin.TabIndex = 0;
         this._treeAdmin.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.AfterSelect_treeAdmin);
         // 
         // _contextMenuAdminTree
         // 
         this._contextMenuAdminTree.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._cmNewServer,
            this._cmNewDatabase,
            this._cmNewLogin,
            this._cmNewAlertRule,
            this._cmNewEventFilter,
            this._cmExportSeparator,
            this._cmExportAlertRules,
            this._cmExportEventFilters,
            this.toolStripSeparator25,
            this._cmRefresh});
         this._contextMenuAdminTree.Name = "_contextMenuAdminTree";
         this._contextMenuAdminTree.Size = new System.Drawing.Size(231, 192);
         this._contextMenuAdminTree.Opening += new System.ComponentModel.CancelEventHandler(this.Opening_cmMenuAdminTree);
         // 
         // _cmNewServer
         // 
         this._cmNewServer.Name = "_cmNewServer";
         this._cmNewServer.Size = new System.Drawing.Size(230, 22);
         this._cmNewServer.Text = "New Registered SQL Server...";
         this._cmNewServer.Click += new System.EventHandler(this.Click_miFileNewServer);
         // 
         // _cmNewDatabase
         // 
         this._cmNewDatabase.Name = "_cmNewDatabase";
         this._cmNewDatabase.Size = new System.Drawing.Size(230, 22);
         this._cmNewDatabase.Text = "New Audited Databases...";
         this._cmNewDatabase.Click += new System.EventHandler(this.Click_miFileNewDatabase);
         // 
         // _cmNewLogin
         // 
         this._cmNewLogin.Name = "_cmNewLogin";
         this._cmNewLogin.Size = new System.Drawing.Size(230, 22);
         this._cmNewLogin.Text = "New SQL Server Login...";
         this._cmNewLogin.Click += new System.EventHandler(this.Click_miFileNewLogin);
         // 
         // _cmNewAlertRule
         // 
         this._cmNewAlertRule.Name = "_cmNewAlertRule";
         this._cmNewAlertRule.Size = new System.Drawing.Size(230, 22);
         this._cmNewAlertRule.Text = "New Alert Rule...";
         this._cmNewAlertRule.Click += new System.EventHandler(this.Click_miFileNewAlertRule);
         // 
         // _cmNewEventFilter
         // 
         this._cmNewEventFilter.Name = "_cmNewEventFilter";
         this._cmNewEventFilter.Size = new System.Drawing.Size(230, 22);
         this._cmNewEventFilter.Text = "New Event Filter...";
         this._cmNewEventFilter.Click += new System.EventHandler(this.Click_miFileNewFilter);
         // 
         // _cmExportSeparator
         // 
         this._cmExportSeparator.Name = "_cmExportSeparator";
         this._cmExportSeparator.Size = new System.Drawing.Size(227, 6);
         // 
         // _cmExportAlertRules
         // 
         this._cmExportAlertRules.Name = "_cmExportAlertRules";
         this._cmExportAlertRules.Size = new System.Drawing.Size(230, 22);
         this._cmExportAlertRules.Text = "Export Alert Rules";
         this._cmExportAlertRules.Click += new System.EventHandler(this.Click_cmExportAlertRules);
         // 
         // _cmExportEventFilters
         // 
         this._cmExportEventFilters.Name = "_cmExportEventFilters";
         this._cmExportEventFilters.Size = new System.Drawing.Size(230, 22);
         this._cmExportEventFilters.Text = "Export Event Filters";
         this._cmExportEventFilters.Click += new System.EventHandler(this.Click_emExportEventFilters);
         // 
         // toolStripSeparator25
         // 
         this.toolStripSeparator25.Name = "toolStripSeparator25";
         this.toolStripSeparator25.Size = new System.Drawing.Size(227, 6);
         // 
         // _cmRefresh
         // 
         this._cmRefresh.Name = "_cmRefresh";
         this._cmRefresh.Size = new System.Drawing.Size(230, 22);
         this._cmRefresh.Text = "Refresh";
         this._cmRefresh.Click += new System.EventHandler(this.Click_miViewRefresh);
         // 
         // _toolStripContainer
         // 
         // 
         // _toolStripContainer.ContentPanel
         // 
         this._toolStripContainer.ContentPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(218)))), ((int)(((byte)(250)))));
         this._toolStripContainer.ContentPanel.Controls.Add(this._splitContainer);
         this._toolStripContainer.ContentPanel.Margin = new System.Windows.Forms.Padding(0);
         this._toolStripContainer.ContentPanel.Padding = new System.Windows.Forms.Padding(3);
         this._toolStripContainer.ContentPanel.Size = new System.Drawing.Size(1016, 655);
         this._toolStripContainer.Dock = System.Windows.Forms.DockStyle.Fill;
         this._toolStripContainer.Location = new System.Drawing.Point(0, 0);
         this._toolStripContainer.Name = "_toolStripContainer";
         this._toolStripContainer.Size = new System.Drawing.Size(1016, 704);
         this._toolStripContainer.TabIndex = 0;
         this._toolStripContainer.Text = "toolStripContainer1";
         // 
         // _toolStripContainer.TopToolStripPanel
         // 
         this._toolStripContainer.TopToolStripPanel.Controls.Add(this._menuStripMain);
         this._toolStripContainer.TopToolStripPanel.Controls.Add(this._toolStripMain);
         // 
         // _splitContainer
         // 
         this._splitContainer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(218)))), ((int)(((byte)(250)))));
         this._splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
         this._splitContainer.Location = new System.Drawing.Point(3, 3);
         this._splitContainer.Name = "_splitContainer";
         // 
         // _splitContainer.Panel1
         // 
         this._splitContainer.Panel1.Controls.Add(this._explorerBar);
         // 
         // _splitContainer.Panel2
         // 
         this._splitContainer.Panel2.Controls.Add(this._mainTabView);
         this._splitContainer.Panel2.Controls.Add(this._loginsView);
         this._splitContainer.Panel2.Controls.Add(this._eventFiltersView);
         this._splitContainer.Panel2.Controls.Add(this._alertRulesView);
         this._splitContainer.Panel2.Controls.Add(this._registeredServerView);
         this._splitContainer.Panel2.Controls.Add(this._changeLogView);
         this._splitContainer.Panel2.Controls.Add(this._activityLogView);
         this._splitContainer.Panel2.Controls.Add(this._reportCategoryView);
         this._splitContainer.Panel2.Controls.Add(this._reportView);
         this._splitContainer.Size = new System.Drawing.Size(1010, 649);
         this._splitContainer.SplitterDistance = 225;
         this._splitContainer.SplitterWidth = 3;
         this._splitContainer.TabIndex = 0;
         // 
         // _explorerBar
         // 
         this._explorerBar.Controls.Add(this._containerExploreActivity);
         this._explorerBar.Controls.Add(this._containerAuditReports);
         this._explorerBar.Controls.Add(this.ultraExplorerBarContainerControl1);
         this._explorerBar.Dock = System.Windows.Forms.DockStyle.Fill;
         ultraExplorerBarGroup1.Container = this._containerExploreActivity;
         ultraExplorerBarGroup1.Expanded = false;
         appearance1.Image = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.ExploreActivities_48;
         ultraExplorerBarGroup1.Settings.AppearancesLarge.HeaderAppearance = appearance1;
         appearance2.Image = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.ExploreActivities_16;
         ultraExplorerBarGroup1.Settings.AppearancesSmall.NavigationOverflowButtonAppearance = appearance2;
         ultraExplorerBarGroup1.Settings.Style = Infragistics.Win.UltraWinExplorerBar.GroupStyle.ControlContainer;
         ultraExplorerBarGroup1.Text = "Explore Activity";
         ultraExplorerBarGroup2.Container = this._containerAuditReports;
         appearance3.Image = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.Reports_48;
         ultraExplorerBarGroup2.Settings.AppearancesLarge.HeaderAppearance = appearance3;
         appearance4.Image = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.Reports_16;
         ultraExplorerBarGroup2.Settings.AppearancesSmall.NavigationOverflowButtonAppearance = appearance4;
         ultraExplorerBarGroup2.Settings.Style = Infragistics.Win.UltraWinExplorerBar.GroupStyle.ControlContainer;
         ultraExplorerBarGroup2.Text = "Audit Reports";
         ultraExplorerBarGroup3.Container = this.ultraExplorerBarContainerControl1;
         appearance5.Image = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.Administration_48;
         ultraExplorerBarGroup3.Settings.AppearancesLarge.HeaderAppearance = appearance5;
         appearance6.Image = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.Administration_16;
         ultraExplorerBarGroup3.Settings.AppearancesSmall.NavigationOverflowButtonAppearance = appearance6;
         ultraExplorerBarGroup3.Settings.Style = Infragistics.Win.UltraWinExplorerBar.GroupStyle.ControlContainer;
         ultraExplorerBarGroup3.Text = "Administration";
         this._explorerBar.Groups.AddRange(new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup[] {
            ultraExplorerBarGroup1,
            ultraExplorerBarGroup2,
            ultraExplorerBarGroup3});
         this._explorerBar.GroupSettings.NavigationAllowHide = Infragistics.Win.DefaultableBoolean.False;
         this._explorerBar.GroupSettings.ShowExpansionIndicator = Infragistics.Win.DefaultableBoolean.False;
         this._explorerBar.Location = new System.Drawing.Point(0, 0);
         this._explorerBar.Margin = new System.Windows.Forms.Padding(10);
         this._explorerBar.Name = "_explorerBar";
         this._explorerBar.NavigationAllowGroupReorder = false;
         this._explorerBar.ShowDefaultContextMenu = false;
         this._explorerBar.Size = new System.Drawing.Size(225, 649);
         this._explorerBar.Style = Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarStyle.OutlookNavigationPane;
         this._explorerBar.TabIndex = 0;
         this._explorerBar.UseLargeGroupHeaderImages = Infragistics.Win.DefaultableBoolean.True;
         this._explorerBar.SelectedGroupChanged += new Infragistics.Win.UltraWinExplorerBar.SelectedGroupChangedEventHandler(this.SelectedGroupChanged_explorerBar);
         // 
         // _mainTabView
         // 
         this._mainTabView.ColorScheme.PanelBorder.SetColor("LunaBlue", System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(45)))), ((int)(((byte)(150))))), false);
         this._mainTabView.ColorScheme.PanelBorder.SetColor("LunaOlive", System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(128)))), ((int)(((byte)(88))))), false);
         this._mainTabView.ColorScheme.PanelBorder.SetColor("LunaSilver", System.Drawing.Color.FromArgb(((int)(((byte)(124)))), ((int)(((byte)(124)))), ((int)(((byte)(148))))), false);
         this._mainTabView.Dock = System.Windows.Forms.DockStyle.Fill;
         this._mainTabView.Location = new System.Drawing.Point(0, 0);
         this._mainTabView.Margin = new System.Windows.Forms.Padding(0);
         this._mainTabView.Name = "_mainTabView";
         this._mainTabView.ShowBanner = true;
         this._mainTabView.ShowGroupBy = true;
         this._mainTabView.Size = new System.Drawing.Size(782, 649);
         this._mainTabView.TabIndex = 5;
         // 
         // _loginsView
         // 
         this._loginsView.ColorScheme.PanelBorder.SetColor("LunaBlue", System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(45)))), ((int)(((byte)(150))))), false);
         this._loginsView.ColorScheme.PanelBorder.SetColor("LunaOlive", System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(128)))), ((int)(((byte)(88))))), false);
         this._loginsView.ColorScheme.PanelBorder.SetColor("LunaSilver", System.Drawing.Color.FromArgb(((int)(((byte)(124)))), ((int)(((byte)(124)))), ((int)(((byte)(148))))), false);
         this._loginsView.Dock = System.Windows.Forms.DockStyle.Fill;
         this._loginsView.Location = new System.Drawing.Point(0, 0);
         this._loginsView.Margin = new System.Windows.Forms.Padding(0);
         this._loginsView.Name = "_loginsView";
         this._loginsView.ShowBanner = true;
         this._loginsView.ShowGroupBy = true;
         this._loginsView.Size = new System.Drawing.Size(782, 649);
         this._loginsView.TabIndex = 3;
         // 
         // _eventFiltersView
         // 
         this._eventFiltersView.ColorScheme.PanelBorder.SetColor("LunaBlue", System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(45)))), ((int)(((byte)(150))))), false);
         this._eventFiltersView.ColorScheme.PanelBorder.SetColor("LunaOlive", System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(128)))), ((int)(((byte)(88))))), false);
         this._eventFiltersView.ColorScheme.PanelBorder.SetColor("LunaSilver", System.Drawing.Color.FromArgb(((int)(((byte)(124)))), ((int)(((byte)(124)))), ((int)(((byte)(148))))), false);
         this._eventFiltersView.Dock = System.Windows.Forms.DockStyle.Fill;
         this._eventFiltersView.Location = new System.Drawing.Point(0, 0);
         this._eventFiltersView.Margin = new System.Windows.Forms.Padding(0);
         this._eventFiltersView.Name = "_eventFiltersView";
         this._eventFiltersView.ShowBanner = true;
         this._eventFiltersView.ShowGroupBy = true;
         this._eventFiltersView.Size = new System.Drawing.Size(782, 649);
         this._eventFiltersView.TabIndex = 2;
         // 
         // _alertRulesView
         // 
         this._alertRulesView.ColorScheme.PanelBorder.SetColor("LunaBlue", System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(45)))), ((int)(((byte)(150))))), false);
         this._alertRulesView.ColorScheme.PanelBorder.SetColor("LunaOlive", System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(128)))), ((int)(((byte)(88))))), false);
         this._alertRulesView.ColorScheme.PanelBorder.SetColor("LunaSilver", System.Drawing.Color.FromArgb(((int)(((byte)(124)))), ((int)(((byte)(124)))), ((int)(((byte)(148))))), false);
         this._alertRulesView.Dock = System.Windows.Forms.DockStyle.Fill;
         this._alertRulesView.Location = new System.Drawing.Point(0, 0);
         this._alertRulesView.Margin = new System.Windows.Forms.Padding(0);
         this._alertRulesView.Name = "_alertRulesView";
         this._alertRulesView.ShowBanner = true;
         this._alertRulesView.ShowGroupBy = true;
         this._alertRulesView.Size = new System.Drawing.Size(782, 649);
         this._alertRulesView.TabIndex = 1;
         // 
         // _registeredServerView
         // 
         this._registeredServerView.ColorScheme.PanelBorder.SetColor("LunaBlue", System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(45)))), ((int)(((byte)(150))))), false);
         this._registeredServerView.ColorScheme.PanelBorder.SetColor("LunaOlive", System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(128)))), ((int)(((byte)(88))))), false);
         this._registeredServerView.ColorScheme.PanelBorder.SetColor("LunaSilver", System.Drawing.Color.FromArgb(((int)(((byte)(124)))), ((int)(((byte)(124)))), ((int)(((byte)(148))))), false);
         this._registeredServerView.Dock = System.Windows.Forms.DockStyle.Fill;
         this._registeredServerView.Location = new System.Drawing.Point(0, 0);
         this._registeredServerView.Margin = new System.Windows.Forms.Padding(0);
         this._registeredServerView.Name = "_registeredServerView";
         this._registeredServerView.ShowBanner = true;
         this._registeredServerView.ShowGroupBy = true;
         this._registeredServerView.Size = new System.Drawing.Size(782, 649);
         this._registeredServerView.TabIndex = 0;
         // 
         // _changeLogView
         // 
         this._changeLogView.ColorScheme.PanelBorder.SetColor("LunaBlue", System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(45)))), ((int)(((byte)(150))))), false);
         this._changeLogView.ColorScheme.PanelBorder.SetColor("LunaOlive", System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(128)))), ((int)(((byte)(88))))), false);
         this._changeLogView.ColorScheme.PanelBorder.SetColor("LunaSilver", System.Drawing.Color.FromArgb(((int)(((byte)(124)))), ((int)(((byte)(124)))), ((int)(((byte)(148))))), false);
         this._changeLogView.Dock = System.Windows.Forms.DockStyle.Fill;
         this._changeLogView.Location = new System.Drawing.Point(0, 0);
         this._changeLogView.Margin = new System.Windows.Forms.Padding(0);
         this._changeLogView.Name = "_changeLogView";
         this._changeLogView.ShowBanner = true;
         this._changeLogView.ShowGroupBy = true;
         this._changeLogView.Size = new System.Drawing.Size(782, 649);
         this._changeLogView.TabIndex = 1;
         // 
         // _activityLogView
         // 
         this._activityLogView.ColorScheme.PanelBorder.SetColor("LunaBlue", System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(45)))), ((int)(((byte)(150))))), false);
         this._activityLogView.ColorScheme.PanelBorder.SetColor("LunaOlive", System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(128)))), ((int)(((byte)(88))))), false);
         this._activityLogView.ColorScheme.PanelBorder.SetColor("LunaSilver", System.Drawing.Color.FromArgb(((int)(((byte)(124)))), ((int)(((byte)(124)))), ((int)(((byte)(148))))), false);
         this._activityLogView.Dock = System.Windows.Forms.DockStyle.Fill;
         this._activityLogView.Location = new System.Drawing.Point(0, 0);
         this._activityLogView.Margin = new System.Windows.Forms.Padding(0);
         this._activityLogView.Name = "_activityLogView";
         this._activityLogView.ShowBanner = true;
         this._activityLogView.ShowGroupBy = true;
         this._activityLogView.Size = new System.Drawing.Size(782, 649);
         this._activityLogView.TabIndex = 6;
         // 
         // _reportCategoryView
         // 
         this._reportCategoryView.CategoryInfo = null;
         this._reportCategoryView.ColorScheme.PanelBorder.SetColor("LunaBlue", System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(45)))), ((int)(((byte)(150))))), false);
         this._reportCategoryView.ColorScheme.PanelBorder.SetColor("LunaOlive", System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(128)))), ((int)(((byte)(88))))), false);
         this._reportCategoryView.ColorScheme.PanelBorder.SetColor("LunaSilver", System.Drawing.Color.FromArgb(((int)(((byte)(124)))), ((int)(((byte)(124)))), ((int)(((byte)(148))))), false);
         this._reportCategoryView.Dock = System.Windows.Forms.DockStyle.Fill;
         this._reportCategoryView.Location = new System.Drawing.Point(0, 0);
         this._reportCategoryView.Margin = new System.Windows.Forms.Padding(0);
         this._reportCategoryView.Name = "_reportCategoryView";
         this._reportCategoryView.ReportsTree = null;
         this._reportCategoryView.ShowBanner = true;
         this._reportCategoryView.ShowGroupBy = true;
         this._reportCategoryView.Size = new System.Drawing.Size(782, 649);
         this._reportCategoryView.TabIndex = 7;
         this._reportCategoryView.Text = "reportCategoryView1";
         // 
         // _reportView
         // 
         this._reportView.ColorScheme.PanelBorder.SetColor("LunaBlue", System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(45)))), ((int)(((byte)(150))))), false);
         this._reportView.ColorScheme.PanelBorder.SetColor("LunaOlive", System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(128)))), ((int)(((byte)(88))))), false);
         this._reportView.ColorScheme.PanelBorder.SetColor("LunaSilver", System.Drawing.Color.FromArgb(((int)(((byte)(124)))), ((int)(((byte)(124)))), ((int)(((byte)(148))))), false);
         this._reportView.Dock = System.Windows.Forms.DockStyle.Fill;
         this._reportView.Location = new System.Drawing.Point(0, 0);
         this._reportView.Margin = new System.Windows.Forms.Padding(0);
         this._reportView.Name = "_reportView";
         this._reportView.ReportInfo = null;
         this._reportView.ServerInstance = null;
         this._reportView.ShowBanner = true;
         this._reportView.ShowGroupBy = true;
         this._reportView.Size = new System.Drawing.Size(782, 649);
         this._reportView.TabIndex = 8;
         this._reportView.Text = "_reportView";
         // 
         // _menuStripMain
         // 
         this._menuStripMain.Dock = System.Windows.Forms.DockStyle.None;
         this._menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._menuFile,
            this._menuEdit,
            this._menuView,
            this._menuAuditing,
            this._menuAlerting,
            this._menuAgent,
            this._menuTools,
            this._menuHelp});
         this._menuStripMain.Location = new System.Drawing.Point(0, 0);
         this._menuStripMain.Name = "_menuStripMain";
         this._menuStripMain.Size = new System.Drawing.Size(1016, 24);
         this._menuStripMain.TabIndex = 0;
         this._menuStripMain.Text = "menuStrip1";
         // 
         // _menuFile
         // 
         this._menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._miFileConnect,
            this.toolStripSeparator1,
            this._miFileNewServer,
            this._miFileNewDatabase,
            this._miFileNewLogin,
            this._miFileNewAlertRule,
            this._miFileNewFilter,
            this._miFileAttachArchive,
            this._miFileImport,
            this.toolStripSeparator2,
            this._miFileLicenses,
            this.toolStripSeparator3,
            this._miFilePrint,
            this.toolStripSeparator4,
            this._miFileServer1,
            this._miFileServer2,
            this._miFileServer3,
            this._miFileServer4,
            this._miFileServer5,
            this._miFileRUSep,
            this._miFileExit});
         this._menuFile.Name = "_menuFile";
         this._menuFile.Size = new System.Drawing.Size(35, 20);
         this._menuFile.Text = "&File";
         this._menuFile.DropDownOpening += new System.EventHandler(this.DropDownOpening_MainMenu);
         // 
         // _miFileConnect
         // 
         this._miFileConnect.Name = "_miFileConnect";
         this._miFileConnect.Size = new System.Drawing.Size(230, 22);
         this._miFileConnect.Text = "&Connect...";
         this._miFileConnect.Click += new System.EventHandler(this.Click_miFileConnect);
         // 
         // toolStripSeparator1
         // 
         this.toolStripSeparator1.Name = "toolStripSeparator1";
         this.toolStripSeparator1.Size = new System.Drawing.Size(227, 6);
         // 
         // _miFileNewServer
         // 
         this._miFileNewServer.Name = "_miFileNewServer";
         this._miFileNewServer.Size = new System.Drawing.Size(230, 22);
         this._miFileNewServer.Text = "New Registered &SQL Server...";
         this._miFileNewServer.Click += new System.EventHandler(this.Click_miFileNewServer);
         // 
         // _miFileNewDatabase
         // 
         this._miFileNewDatabase.Name = "_miFileNewDatabase";
         this._miFileNewDatabase.Size = new System.Drawing.Size(230, 22);
         this._miFileNewDatabase.Text = "New Audited &Database...";
         this._miFileNewDatabase.Click += new System.EventHandler(this.Click_miFileNewDatabase);
         // 
         // _miFileNewLogin
         // 
         this._miFileNewLogin.Name = "_miFileNewLogin";
         this._miFileNewLogin.Size = new System.Drawing.Size(230, 22);
         this._miFileNewLogin.Text = "New SQL Server &Login...";
         this._miFileNewLogin.Click += new System.EventHandler(this.Click_miFileNewLogin);
         // 
         // _miFileNewAlertRule
         // 
         this._miFileNewAlertRule.Name = "_miFileNewAlertRule";
         this._miFileNewAlertRule.Size = new System.Drawing.Size(230, 22);
         this._miFileNewAlertRule.Text = "New Alert &Rule...";
         this._miFileNewAlertRule.Click += new System.EventHandler(this.Click_miFileNewAlertRule);
         // 
         // _miFileNewFilter
         // 
         this._miFileNewFilter.Name = "_miFileNewFilter";
         this._miFileNewFilter.Size = new System.Drawing.Size(230, 22);
         this._miFileNewFilter.Text = "New Event &Filter...";
         this._miFileNewFilter.Click += new System.EventHandler(this.Click_miFileNewFilter);
         // 
         // _miFileAttachArchive
         // 
         this._miFileAttachArchive.Name = "_miFileAttachArchive";
         this._miFileAttachArchive.Size = new System.Drawing.Size(230, 22);
         this._miFileAttachArchive.Text = "&Attach Archive Database...";
         this._miFileAttachArchive.Click += new System.EventHandler(this.Click_miFileAttachArchive);
         // 
         // _miFileImport
         // 
         this._miFileImport.Name = "_miFileImport";
         this._miFileImport.Size = new System.Drawing.Size(230, 22);
         this._miFileImport.Text = "Import Audit Settings...";
         this._miFileImport.Click += new System.EventHandler(this.Click_miFileImport);
         // 
         // toolStripSeparator2
         // 
         this.toolStripSeparator2.Name = "toolStripSeparator2";
         this.toolStripSeparator2.Size = new System.Drawing.Size(227, 6);
         // 
         // _miFileLicenses
         // 
         this._miFileLicenses.Name = "_miFileLicenses";
         this._miFileLicenses.Size = new System.Drawing.Size(230, 22);
         this._miFileLicenses.Text = "&Manage Licenses...";
         this._miFileLicenses.Click += new System.EventHandler(this.Click_miFileLicenses);
         // 
         // toolStripSeparator3
         // 
         this.toolStripSeparator3.Name = "toolStripSeparator3";
         this.toolStripSeparator3.Size = new System.Drawing.Size(227, 6);
         // 
         // _miFilePrint
         // 
         this._miFilePrint.Name = "_miFilePrint";
         this._miFilePrint.Size = new System.Drawing.Size(230, 22);
         this._miFilePrint.Text = "&Print...";
         this._miFilePrint.Click += new System.EventHandler(this.Click_miFilePrint);
         // 
         // toolStripSeparator4
         // 
         this.toolStripSeparator4.Name = "toolStripSeparator4";
         this.toolStripSeparator4.Size = new System.Drawing.Size(227, 6);
         // 
         // _miFileServer1
         // 
         this._miFileServer1.Name = "_miFileServer1";
         this._miFileServer1.Size = new System.Drawing.Size(230, 22);
         this._miFileServer1.Text = "Server 1";
         this._miFileServer1.Click += new System.EventHandler(this.Click_miFileRUServer);
         // 
         // _miFileServer2
         // 
         this._miFileServer2.Name = "_miFileServer2";
         this._miFileServer2.Size = new System.Drawing.Size(230, 22);
         this._miFileServer2.Text = "Server 2";
         this._miFileServer2.Click += new System.EventHandler(this.Click_miFileRUServer);
         // 
         // _miFileServer3
         // 
         this._miFileServer3.Name = "_miFileServer3";
         this._miFileServer3.Size = new System.Drawing.Size(230, 22);
         this._miFileServer3.Text = "Server 3";
         this._miFileServer3.Click += new System.EventHandler(this.Click_miFileRUServer);
         // 
         // _miFileServer4
         // 
         this._miFileServer4.Name = "_miFileServer4";
         this._miFileServer4.Size = new System.Drawing.Size(230, 22);
         this._miFileServer4.Text = "Server 4";
         this._miFileServer4.Click += new System.EventHandler(this.Click_miFileRUServer);
         // 
         // _miFileServer5
         // 
         this._miFileServer5.Name = "_miFileServer5";
         this._miFileServer5.Size = new System.Drawing.Size(230, 22);
         this._miFileServer5.Text = "Server 5";
         this._miFileServer5.Click += new System.EventHandler(this.Click_miFileRUServer);
         // 
         // _miFileRUSep
         // 
         this._miFileRUSep.Name = "_miFileRUSep";
         this._miFileRUSep.Size = new System.Drawing.Size(227, 6);
         // 
         // _miFileExit
         // 
         this._miFileExit.Name = "_miFileExit";
         this._miFileExit.Size = new System.Drawing.Size(230, 22);
         this._miFileExit.Text = "E&xit";
         this._miFileExit.Click += new System.EventHandler(this.Click_miFileExit);
         // 
         // _menuEdit
         // 
         this._menuEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._miEditRemove,
            this.toolStripSeparator5,
            this._miEditProperties});
         this._menuEdit.Name = "_menuEdit";
         this._menuEdit.Size = new System.Drawing.Size(37, 20);
         this._menuEdit.Text = "&Edit";
         this._menuEdit.DropDownOpening += new System.EventHandler(this.DropDownOpening_MainMenu);
         // 
         // _miEditRemove
         // 
         this._miEditRemove.Name = "_miEditRemove";
         this._miEditRemove.Size = new System.Drawing.Size(134, 22);
         this._miEditRemove.Text = "&Remove";
         this._miEditRemove.Click += new System.EventHandler(this.Click_miEditRemove);
         // 
         // toolStripSeparator5
         // 
         this.toolStripSeparator5.Name = "toolStripSeparator5";
         this.toolStripSeparator5.Size = new System.Drawing.Size(131, 6);
         // 
         // _miEditProperties
         // 
         this._miEditProperties.Name = "_miEditProperties";
         this._miEditProperties.Size = new System.Drawing.Size(134, 22);
         this._miEditProperties.Text = "&Properties";
         this._miEditProperties.Click += new System.EventHandler(this.Click_miEditProperties);
         // 
         // _menuView
         // 
         this._menuView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._miViewSetFilter,
            this.toolStripSeparator6,
            this._miViewConsoleTree,
            this._miViewToolbar,
            this._miViewCommonTasks,
            this._miViewTaskBanners,
            this.toolStripSeparator7,
            this._miViewCollapse,
            this._miViewExpand,
            this._miViewGroupBy,
            this.toolStripSeparator8,
            this._miViewRefresh,
            this.toolStripSeparator9,
            this._miViewPreferences});
         this._menuView.Name = "_menuView";
         this._menuView.Size = new System.Drawing.Size(41, 20);
         this._menuView.Text = "&View";
         this._menuView.DropDownOpening += new System.EventHandler(this.DropDownOpening_MainMenu);
         // 
         // _miViewSetFilter
         // 
         this._miViewSetFilter.Name = "_miViewSetFilter";
         this._miViewSetFilter.Size = new System.Drawing.Size(234, 22);
         this._miViewSetFilter.Text = "Set &Filter...";
         this._miViewSetFilter.Visible = false;
         this._miViewSetFilter.Click += new System.EventHandler(this.Click_miViewSetFilter);
         // 
         // toolStripSeparator6
         // 
         this.toolStripSeparator6.Name = "toolStripSeparator6";
         this.toolStripSeparator6.Size = new System.Drawing.Size(231, 6);
         this.toolStripSeparator6.Visible = false;
         // 
         // _miViewConsoleTree
         // 
         this._miViewConsoleTree.Checked = true;
         this._miViewConsoleTree.CheckState = System.Windows.Forms.CheckState.Checked;
         this._miViewConsoleTree.Name = "_miViewConsoleTree";
         this._miViewConsoleTree.Size = new System.Drawing.Size(234, 22);
         this._miViewConsoleTree.Text = "View Console &Tree";
         this._miViewConsoleTree.Click += new System.EventHandler(this.Click_miViewConsoleTree);
         // 
         // _miViewToolbar
         // 
         this._miViewToolbar.Name = "_miViewToolbar";
         this._miViewToolbar.Size = new System.Drawing.Size(234, 22);
         this._miViewToolbar.Text = "View &Toolbar";
         this._miViewToolbar.Click += new System.EventHandler(this.Click_miViewToolbar);
         // 
         // _miViewCommonTasks
         // 
         this._miViewCommonTasks.Name = "_miViewCommonTasks";
         this._miViewCommonTasks.Size = new System.Drawing.Size(234, 22);
         this._miViewCommonTasks.Text = "View Com&mon Tasks";
         this._miViewCommonTasks.Visible = false;
         this._miViewCommonTasks.Click += new System.EventHandler(this.Click_miViewCommonTasks);
         // 
         // _miViewTaskBanners
         // 
         this._miViewTaskBanners.Name = "_miViewTaskBanners";
         this._miViewTaskBanners.Size = new System.Drawing.Size(234, 22);
         this._miViewTaskBanners.Text = "View Task &Banners";
         this._miViewTaskBanners.Visible = false;
         this._miViewTaskBanners.Click += new System.EventHandler(this.Click_miViewTaskBanners);
         // 
         // toolStripSeparator7
         // 
         this.toolStripSeparator7.Name = "toolStripSeparator7";
         this.toolStripSeparator7.Size = new System.Drawing.Size(231, 6);
         // 
         // _miViewCollapse
         // 
         this._miViewCollapse.Name = "_miViewCollapse";
         this._miViewCollapse.Size = new System.Drawing.Size(234, 22);
         this._miViewCollapse.Text = "&Collapse All";
         this._miViewCollapse.Visible = false;
         this._miViewCollapse.Click += new System.EventHandler(this.Click_miViewCollapse);
         // 
         // _miViewExpand
         // 
         this._miViewExpand.Name = "_miViewExpand";
         this._miViewExpand.Size = new System.Drawing.Size(234, 22);
         this._miViewExpand.Text = "E&xpand All";
         this._miViewExpand.Visible = false;
         this._miViewExpand.Click += new System.EventHandler(this.Click_miViewExpand);
         // 
         // _miViewGroupBy
         // 
         this._miViewGroupBy.Name = "_miViewGroupBy";
         this._miViewGroupBy.Size = new System.Drawing.Size(234, 22);
         this._miViewGroupBy.Text = "View \"Group events by column\"";
         this._miViewGroupBy.Visible = false;
         this._miViewGroupBy.Click += new System.EventHandler(this.Click_miViewGroupBy);
         // 
         // toolStripSeparator8
         // 
         this.toolStripSeparator8.Name = "toolStripSeparator8";
         this.toolStripSeparator8.Size = new System.Drawing.Size(231, 6);
         this.toolStripSeparator8.Visible = false;
         // 
         // _miViewRefresh
         // 
         this._miViewRefresh.Name = "_miViewRefresh";
         this._miViewRefresh.ShortcutKeys = System.Windows.Forms.Keys.F5;
         this._miViewRefresh.Size = new System.Drawing.Size(234, 22);
         this._miViewRefresh.Text = "&Refresh";
         this._miViewRefresh.Click += new System.EventHandler(this.Click_miViewRefresh);
         // 
         // toolStripSeparator9
         // 
         this.toolStripSeparator9.Name = "toolStripSeparator9";
         this.toolStripSeparator9.Size = new System.Drawing.Size(231, 6);
         // 
         // _miViewPreferences
         // 
         this._miViewPreferences.Name = "_miViewPreferences";
         this._miViewPreferences.Size = new System.Drawing.Size(234, 22);
         this._miViewPreferences.Text = "&Console Preferences...";
         this._miViewPreferences.Click += new System.EventHandler(this.Click_miViewPreferences);
         // 
         // _menuAuditing
         // 
         this._menuAuditing.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._miAuditingEnable,
            this._miAuditingDisable,
            this.toolStripSeparator10,
            this._miAuditingArchive,
            this.toolStripSeparator11,
            this._miAuditingCollectNow,
            this._miAuditingCheckIntegrity,
            this._miAuditingCaptureSnapshot,
            this.toolStripSeparator12,
            this._miAuditingSnapshotPreferences,
            this._miAuditingLoginFilters,
            this.toolStripSeparator13,
            this._miAuditingCollectionServerStatus,
            this._miAuditingConfigureRepository,
            this._miAuditingConfigureReports});
         this._menuAuditing.Name = "_menuAuditing";
         this._menuAuditing.Size = new System.Drawing.Size(58, 20);
         this._menuAuditing.Text = "&Auditing";
         this._menuAuditing.DropDownOpening += new System.EventHandler(this.DropDownOpening_MainMenu);
         // 
         // _miAuditingEnable
         // 
         this._miAuditingEnable.Name = "_miAuditingEnable";
         this._miAuditingEnable.Size = new System.Drawing.Size(253, 22);
         this._miAuditingEnable.Text = "&Enable Auditing";
         this._miAuditingEnable.Click += new System.EventHandler(this.Click_miAuditingEnable);
         // 
         // _miAuditingDisable
         // 
         this._miAuditingDisable.Name = "_miAuditingDisable";
         this._miAuditingDisable.Size = new System.Drawing.Size(253, 22);
         this._miAuditingDisable.Text = "&Disable Auditing";
         this._miAuditingDisable.Click += new System.EventHandler(this.Click_miAuditingDisable);
         // 
         // toolStripSeparator10
         // 
         this.toolStripSeparator10.Name = "toolStripSeparator10";
         this.toolStripSeparator10.Size = new System.Drawing.Size(250, 6);
         // 
         // _miAuditingArchive
         // 
         this._miAuditingArchive.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._miAuditingArchiveNow,
            this._miAuditingArchiveGroomNow,
            this._miAuditingArchivePreferences});
         this._miAuditingArchive.Name = "_miAuditingArchive";
         this._miAuditingArchive.Size = new System.Drawing.Size(253, 22);
         this._miAuditingArchive.Text = "&Archive and Retention";
         // 
         // _miAuditingArchiveNow
         // 
         this._miAuditingArchiveNow.Name = "_miAuditingArchiveNow";
         this._miAuditingArchiveNow.Size = new System.Drawing.Size(211, 22);
         this._miAuditingArchiveNow.Text = "&Archive Audit Data Now...";
         this._miAuditingArchiveNow.Click += new System.EventHandler(this.Click_miAuditingArchiveNow);
         // 
         // _miAuditingArchiveGroomNow
         // 
         this._miAuditingArchiveGroomNow.Name = "_miAuditingArchiveGroomNow";
         this._miAuditingArchiveGroomNow.Size = new System.Drawing.Size(211, 22);
         this._miAuditingArchiveGroomNow.Text = "&Groom Audit Data Now...";
         this._miAuditingArchiveGroomNow.Click += new System.EventHandler(this.Click_miAuditingArchiveGroomNow);
         // 
         // _miAuditingArchivePreferences
         // 
         this._miAuditingArchivePreferences.Name = "_miAuditingArchivePreferences";
         this._miAuditingArchivePreferences.Size = new System.Drawing.Size(211, 22);
         this._miAuditingArchivePreferences.Text = "Archive &Preferences...";
         this._miAuditingArchivePreferences.Click += new System.EventHandler(this.Click_miAuditingArchivePreferences);
         // 
         // toolStripSeparator11
         // 
         this.toolStripSeparator11.Name = "toolStripSeparator11";
         this.toolStripSeparator11.Size = new System.Drawing.Size(250, 6);
         // 
         // _miAuditingCollectNow
         // 
         this._miAuditingCollectNow.Name = "_miAuditingCollectNow";
         this._miAuditingCollectNow.Size = new System.Drawing.Size(253, 22);
         this._miAuditingCollectNow.Text = "Collect Audit Data &Now";
         this._miAuditingCollectNow.Click += new System.EventHandler(this.Click_miAuditingCollectNow);
         // 
         // _miAuditingCheckIntegrity
         // 
         this._miAuditingCheckIntegrity.Name = "_miAuditingCheckIntegrity";
         this._miAuditingCheckIntegrity.Size = new System.Drawing.Size(253, 22);
         this._miAuditingCheckIntegrity.Text = "Chec&k Repository Integrity...";
         this._miAuditingCheckIntegrity.Click += new System.EventHandler(this.Click_miAuditingCheckIntegrity);
         // 
         // _miAuditingCaptureSnapshot
         // 
         this._miAuditingCaptureSnapshot.Name = "_miAuditingCaptureSnapshot";
         this._miAuditingCaptureSnapshot.Size = new System.Drawing.Size(253, 22);
         this._miAuditingCaptureSnapshot.Text = "Ca&pture Audit Snapshot...";
         this._miAuditingCaptureSnapshot.Click += new System.EventHandler(this.Click_miAuditingCaptureSnapshot);
         // 
         // toolStripSeparator12
         // 
         this.toolStripSeparator12.Name = "toolStripSeparator12";
         this.toolStripSeparator12.Size = new System.Drawing.Size(250, 6);
         // 
         // _miAuditingSnapshotPreferences
         // 
         this._miAuditingSnapshotPreferences.Name = "_miAuditingSnapshotPreferences";
         this._miAuditingSnapshotPreferences.Size = new System.Drawing.Size(253, 22);
         this._miAuditingSnapshotPreferences.Text = "Audit &Snapshot Preferences...";
         this._miAuditingSnapshotPreferences.Click += new System.EventHandler(this.Click_miAuditingSnapshotPreferences);
         // 
         // _miAuditingLoginFilters
         // 
         this._miAuditingLoginFilters.Name = "_miAuditingLoginFilters";
         this._miAuditingLoginFilters.Size = new System.Drawing.Size(253, 22);
         this._miAuditingLoginFilters.Text = "Login Filter Options...";
         this._miAuditingLoginFilters.Click += new System.EventHandler(this.Click_miAuditingLoginFilters);
         // 
         // toolStripSeparator13
         // 
         this.toolStripSeparator13.Name = "toolStripSeparator13";
         this.toolStripSeparator13.Size = new System.Drawing.Size(250, 6);
         // 
         // _miAuditingCollectionServerStatus
         // 
         this._miAuditingCollectionServerStatus.Name = "_miAuditingCollectionServerStatus";
         this._miAuditingCollectionServerStatus.Size = new System.Drawing.Size(253, 22);
         this._miAuditingCollectionServerStatus.Text = "&Collection Server Status...";
         this._miAuditingCollectionServerStatus.Click += new System.EventHandler(this.Click_miAuditingCollectionServerStatus);
         // 
         // _miAuditingConfigureRepository
         // 
         this._miAuditingConfigureRepository.Name = "_miAuditingConfigureRepository";
         this._miAuditingConfigureRepository.Size = new System.Drawing.Size(253, 22);
         this._miAuditingConfigureRepository.Text = "Configure Repository &Databases...";
         this._miAuditingConfigureRepository.Click += new System.EventHandler(this.Click_miAuditingConfigureRepository);
         // 
         // _miAuditingConfigureReports
         // 
         this._miAuditingConfigureReports.Name = "_miAuditingConfigureReports";
         this._miAuditingConfigureReports.Size = new System.Drawing.Size(253, 22);
         this._miAuditingConfigureReports.Text = "Configure &Reporting Services...";
         this._miAuditingConfigureReports.Visible = false;
         this._miAuditingConfigureReports.Click += new System.EventHandler(this.Click_miAuditingConfigureReports);
         // 
         // _menuAlerting
         // 
         this._menuAlerting.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._miAlertingConfigureEmail,
            this._miAlertingGroom});
         this._menuAlerting.Name = "_menuAlerting";
         this._menuAlerting.Size = new System.Drawing.Size(56, 20);
         this._menuAlerting.Text = "A&lerting";
         this._menuAlerting.DropDownOpening += new System.EventHandler(this.DropDownOpening_MainMenu);
         // 
         // _miAlertingConfigureEmail
         // 
         this._miAlertingConfigureEmail.Name = "_miAlertingConfigureEmail";
         this._miAlertingConfigureEmail.Size = new System.Drawing.Size(213, 22);
         this._miAlertingConfigureEmail.Text = "Configure &Email Settings...";
         this._miAlertingConfigureEmail.Click += new System.EventHandler(this.Click_miAlertingConfigureEmail);
         // 
         // _miAlertingGroom
         // 
         this._miAlertingGroom.Name = "_miAlertingGroom";
         this._miAlertingGroom.Size = new System.Drawing.Size(213, 22);
         this._miAlertingGroom.Text = "&Groom Alerts Now";
         this._miAlertingGroom.Click += new System.EventHandler(this.Click_miAlertingGroom);
         // 
         // _menuAgent
         // 
         this._menuAgent.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._miAgentDeploy,
            this._miAgentUpgrade,
            this._miAgentCheckStatus,
            this._miAgentUpdateNow,
            this.toolStripSeparator14,
            this._miAgentTraceDirectory,
            this.toolStripSeparator15,
            this._miAgentProperties});
         this._menuAgent.Name = "_menuAgent";
         this._menuAgent.Size = new System.Drawing.Size(48, 20);
         this._menuAgent.Text = "A&gent";
         this._menuAgent.DropDownOpening += new System.EventHandler(this.DropDownOpening_MainMenu);
         // 
         // _miAgentDeploy
         // 
         this._miAgentDeploy.Name = "_miAgentDeploy";
         this._miAgentDeploy.Size = new System.Drawing.Size(214, 22);
         this._miAgentDeploy.Text = "&Deploy...";
         this._miAgentDeploy.Click += new System.EventHandler(this.Click_miAgentDeploy);
         // 
         // _miAgentUpgrade
         // 
         this._miAgentUpgrade.Name = "_miAgentUpgrade";
         this._miAgentUpgrade.Size = new System.Drawing.Size(214, 22);
         this._miAgentUpgrade.Text = "Up&grade...";
         this._miAgentUpgrade.Click += new System.EventHandler(this.Click_miAgentUpgrade);
         // 
         // _miAgentCheckStatus
         // 
         this._miAgentCheckStatus.Name = "_miAgentCheckStatus";
         this._miAgentCheckStatus.Size = new System.Drawing.Size(214, 22);
         this._miAgentCheckStatus.Text = "&Check Status...";
         this._miAgentCheckStatus.Click += new System.EventHandler(this.Click_miAgentCheckStatus);
         // 
         // _miAgentUpdateNow
         // 
         this._miAgentUpdateNow.Name = "_miAgentUpdateNow";
         this._miAgentUpdateNow.Size = new System.Drawing.Size(214, 22);
         this._miAgentUpdateNow.Text = "&Update Audit Settings Now";
         this._miAgentUpdateNow.Click += new System.EventHandler(this.Click_miAgentUpdateNow);
         // 
         // toolStripSeparator14
         // 
         this.toolStripSeparator14.Name = "toolStripSeparator14";
         this.toolStripSeparator14.Size = new System.Drawing.Size(211, 6);
         // 
         // _miAgentTraceDirectory
         // 
         this._miAgentTraceDirectory.Name = "_miAgentTraceDirectory";
         this._miAgentTraceDirectory.Size = new System.Drawing.Size(214, 22);
         this._miAgentTraceDirectory.Text = "Change &Trace Directory...";
         this._miAgentTraceDirectory.Click += new System.EventHandler(this.Click_miAgentTraceDirectory);
         // 
         // toolStripSeparator15
         // 
         this.toolStripSeparator15.Name = "toolStripSeparator15";
         this.toolStripSeparator15.Size = new System.Drawing.Size(211, 6);
         // 
         // _miAgentProperties
         // 
         this._miAgentProperties.Name = "_miAgentProperties";
         this._miAgentProperties.Size = new System.Drawing.Size(214, 22);
         this._miAgentProperties.Text = "Agent Pr&operties...";
         this._miAgentProperties.Click += new System.EventHandler(this.Click_miAgentProperties);
         // 
         // _menuTools
         // 
         this._menuTools.Name = "_menuTools";
         this._menuTools.Size = new System.Drawing.Size(44, 20);
         this._menuTools.Text = "&Tools";
         // 
         // _menuHelp
         // 
         this._menuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._miHelpWindow,
            this.toolStripSeparator16,
            this._miHelpAuditing,
            this._miHelpSecurity,
            this._miHelpReports,
            this._miHelpAlerting,
            this.toolStripSeparator17,
            this._miHelpContents,
            this.toolStripSeparator18,
            this._miHelpSearch,
            this.toolStripSeparator19,
            this._miHelpMore,
            this._miHelpTechSupport,
            this._miHelpIderaProducts,
            this.toolStripSeparator20,
            this._miHelpAbout});
         this._menuHelp.Name = "_menuHelp";
         this._menuHelp.Size = new System.Drawing.Size(40, 20);
         this._menuHelp.Text = "&Help";
         this._menuHelp.DropDownOpening += new System.EventHandler(this.DropDownOpening_MainMenu);
         // 
         // _miHelpWindow
         // 
         this._miHelpWindow.Name = "_miHelpWindow";
         this._miHelpWindow.Size = new System.Drawing.Size(246, 22);
         this._miHelpWindow.Text = "Help on this &Window";
         this._miHelpWindow.Click += new System.EventHandler(this.Click_miHelpWindow);
         // 
         // toolStripSeparator16
         // 
         this.toolStripSeparator16.Name = "toolStripSeparator16";
         this.toolStripSeparator16.Size = new System.Drawing.Size(243, 6);
         // 
         // _miHelpAuditing
         // 
         this._miHelpAuditing.Name = "_miHelpAuditing";
         this._miHelpAuditing.Size = new System.Drawing.Size(246, 22);
         this._miHelpAuditing.Text = "Help on &Auditing";
         this._miHelpAuditing.Click += new System.EventHandler(this.Click_miHelpAuditing);
         // 
         // _miHelpSecurity
         // 
         this._miHelpSecurity.Name = "_miHelpSecurity";
         this._miHelpSecurity.Size = new System.Drawing.Size(246, 22);
         this._miHelpSecurity.Text = "Help on &Console Security";
         this._miHelpSecurity.Click += new System.EventHandler(this.Click_miHelpSecurity);
         // 
         // _miHelpReports
         // 
         this._miHelpReports.Name = "_miHelpReports";
         this._miHelpReports.Size = new System.Drawing.Size(246, 22);
         this._miHelpReports.Text = "Help on &Reports";
         this._miHelpReports.Click += new System.EventHandler(this.Click_miHelpReports);
         // 
         // _miHelpAlerting
         // 
         this._miHelpAlerting.Name = "_miHelpAlerting";
         this._miHelpAlerting.Size = new System.Drawing.Size(246, 22);
         this._miHelpAlerting.Text = "Help on Alerting";
         this._miHelpAlerting.Click += new System.EventHandler(this.Click_miHelpAlerting);
         // 
         // toolStripSeparator17
         // 
         this.toolStripSeparator17.Name = "toolStripSeparator17";
         this.toolStripSeparator17.Size = new System.Drawing.Size(243, 6);
         // 
         // _miHelpContents
         // 
         this._miHelpContents.Name = "_miHelpContents";
         this._miHelpContents.Size = new System.Drawing.Size(246, 22);
         this._miHelpContents.Text = "&View Help Contents";
         this._miHelpContents.Click += new System.EventHandler(this.Click_miHelpContents);
         // 
         // toolStripSeparator18
         // 
         this.toolStripSeparator18.Name = "toolStripSeparator18";
         this.toolStripSeparator18.Size = new System.Drawing.Size(243, 6);
         // 
         // _miHelpSearch
         // 
         this._miHelpSearch.Name = "_miHelpSearch";
         this._miHelpSearch.Size = new System.Drawing.Size(246, 22);
         this._miHelpSearch.Text = "Search IDERA &Knowledge Base";
         this._miHelpSearch.Click += new System.EventHandler(this.Click_miHelpSearch);
         // 
         // toolStripSeparator19
         // 
         this.toolStripSeparator19.Name = "toolStripSeparator19";
         this.toolStripSeparator19.Size = new System.Drawing.Size(243, 6);
         // 
         // _miHelpMore
         // 
         this._miHelpMore.Name = "_miHelpMore";
         this._miHelpMore.Size = new System.Drawing.Size(246, 22);
         this._miHelpMore.Text = "&More on SQL Compliance Manager";
         this._miHelpMore.Click += new System.EventHandler(this.Click_miHelpMore);
         // 
         // _miHelpTechSupport
         // 
         this._miHelpTechSupport.Name = "_miHelpTechSupport";
         this._miHelpTechSupport.Size = new System.Drawing.Size(246, 22);
         this._miHelpTechSupport.Text = "Contact &Technical Support";
         this._miHelpTechSupport.Click += new System.EventHandler(this.Click_miHelpTechSupport);
         // 
         // _miHelpIderaProducts
         // 
         this._miHelpIderaProducts.Name = "_miHelpIderaProducts";
         this._miHelpIderaProducts.Size = new System.Drawing.Size(246, 22);
         this._miHelpIderaProducts.Text = "About &IDERA products";
         this._miHelpIderaProducts.Click += new System.EventHandler(this.Click_miHelpIderaProducts);
         // 
         // toolStripSeparator20
         // 
         this.toolStripSeparator20.Name = "toolStripSeparator20";
         this.toolStripSeparator20.Size = new System.Drawing.Size(243, 6);
         // 
         // _miHelpAbout
         // 
         this._miHelpAbout.Name = "_miHelpAbout";
         this._miHelpAbout.Size = new System.Drawing.Size(246, 22);
         this._miHelpAbout.Text = "&About SQL Compliance Manager";
         this._miHelpAbout.Click += new System.EventHandler(this.Click_miHelpAbout);
         // 
         // _toolStripMain
         // 
         this._toolStripMain.Dock = System.Windows.Forms.DockStyle.None;
         this._toolStripMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
         this._toolStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsNew,
            this.toolStripSeparator21,
            this._tsPrint,
            this._tsDelete,
            this.toolStripSeparator22,
            this._tsProperties,
            this._tsRefresh,
            this.toolStripSeparator23,
            this._tsShowTree,
            this._tsCollapse,
            this._tsExpand,
            this._tsGroupBy,
            this._tsShowBanners,
            this.toolStripSeparator24,
            this._tsHelp});
         this._toolStripMain.Location = new System.Drawing.Point(3, 24);
         this._toolStripMain.Name = "_toolStripMain";
         this._toolStripMain.Size = new System.Drawing.Size(209, 25);
         this._toolStripMain.TabIndex = 1;
         // 
         // _tsNew
         // 
         this._tsNew.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsNewServer,
            this._tsNewDatabase,
            this._tsNewLogin,
            this._tsNewAlertRule,
            this._tsNewFilter});
         this._tsNew.ImageTransparentColor = System.Drawing.Color.Magenta;
         this._tsNew.Name = "_tsNew";
         this._tsNew.Size = new System.Drawing.Size(44, 22);
         this._tsNew.Text = "New";
         this._tsNew.ButtonClick += new System.EventHandler(this.ButtonClick_tsNew);
         // 
         // _tsNewServer
         // 
         this._tsNewServer.Name = "_tsNewServer";
         this._tsNewServer.Size = new System.Drawing.Size(206, 22);
         this._tsNewServer.Text = "Registered SQL Server...";
         this._tsNewServer.Click += new System.EventHandler(this.Click_miFileNewServer);
         // 
         // _tsNewDatabase
         // 
         this._tsNewDatabase.Name = "_tsNewDatabase";
         this._tsNewDatabase.Size = new System.Drawing.Size(206, 22);
         this._tsNewDatabase.Text = "Audited Database...";
         this._tsNewDatabase.Click += new System.EventHandler(this.Click_miFileNewDatabase);
         // 
         // _tsNewLogin
         // 
         this._tsNewLogin.Name = "_tsNewLogin";
         this._tsNewLogin.Size = new System.Drawing.Size(206, 22);
         this._tsNewLogin.Text = "SQL Server Login...";
         this._tsNewLogin.Click += new System.EventHandler(this.Click_miFileNewLogin);
         // 
         // _tsNewAlertRule
         // 
         this._tsNewAlertRule.Name = "_tsNewAlertRule";
         this._tsNewAlertRule.Size = new System.Drawing.Size(206, 22);
         this._tsNewAlertRule.Text = "Alert Rule...";
         this._tsNewAlertRule.Click += new System.EventHandler(this.Click_miFileNewAlertRule);
         // 
         // _tsNewFilter
         // 
         this._tsNewFilter.Name = "_tsNewFilter";
         this._tsNewFilter.Size = new System.Drawing.Size(206, 22);
         this._tsNewFilter.Text = "Event Filter...";
         this._tsNewFilter.Click += new System.EventHandler(this.Click_miFileNewFilter);
         // 
         // toolStripSeparator21
         // 
         this.toolStripSeparator21.Name = "toolStripSeparator21";
         this.toolStripSeparator21.Size = new System.Drawing.Size(6, 25);
         // 
         // _tsPrint
         // 
         this._tsPrint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
         this._tsPrint.Image = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.Print_16;
         this._tsPrint.ImageTransparentColor = System.Drawing.Color.Magenta;
         this._tsPrint.Name = "_tsPrint";
         this._tsPrint.Size = new System.Drawing.Size(23, 22);
         this._tsPrint.Text = "Print";
         this._tsPrint.Click += new System.EventHandler(this.Click_miFilePrint);
         // 
         // _tsDelete
         // 
         this._tsDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
         this._tsDelete.Image = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.Delete_16;
         this._tsDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
         this._tsDelete.Name = "_tsDelete";
         this._tsDelete.Size = new System.Drawing.Size(23, 22);
         this._tsDelete.Text = "Remove";
         this._tsDelete.Click += new System.EventHandler(this.Click_miEditRemove);
         // 
         // toolStripSeparator22
         // 
         this.toolStripSeparator22.Name = "toolStripSeparator22";
         this.toolStripSeparator22.Size = new System.Drawing.Size(6, 25);
         // 
         // _tsProperties
         // 
         this._tsProperties.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
         this._tsProperties.Image = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.Properties_16;
         this._tsProperties.ImageTransparentColor = System.Drawing.Color.Magenta;
         this._tsProperties.Name = "_tsProperties";
         this._tsProperties.Size = new System.Drawing.Size(23, 22);
         this._tsProperties.Text = "Properties";
         this._tsProperties.Click += new System.EventHandler(this.Click_miEditProperties);
         // 
         // _tsRefresh
         // 
         this._tsRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
         this._tsRefresh.Image = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.Refresh_16;
         this._tsRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
         this._tsRefresh.Name = "_tsRefresh";
         this._tsRefresh.Size = new System.Drawing.Size(23, 22);
         this._tsRefresh.Text = "Refresh";
         this._tsRefresh.Click += new System.EventHandler(this.Click_miViewRefresh);
         // 
         // toolStripSeparator23
         // 
         this.toolStripSeparator23.Name = "toolStripSeparator23";
         this.toolStripSeparator23.Size = new System.Drawing.Size(6, 25);
         // 
         // _tsShowTree
         // 
         this._tsShowTree.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
         this._tsShowTree.Image = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.ShowHide_161;
         this._tsShowTree.ImageTransparentColor = System.Drawing.Color.Magenta;
         this._tsShowTree.Name = "_tsShowTree";
         this._tsShowTree.Size = new System.Drawing.Size(23, 22);
         this._tsShowTree.Text = "View Console Tree";
         this._tsShowTree.Click += new System.EventHandler(this.Click_miViewConsoleTree);
         // 
         // _tsCollapse
         // 
         this._tsCollapse.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
         this._tsCollapse.Image = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.Bug_16;
         this._tsCollapse.ImageTransparentColor = System.Drawing.Color.Magenta;
         this._tsCollapse.Name = "_tsCollapse";
         this._tsCollapse.Size = new System.Drawing.Size(23, 22);
         this._tsCollapse.Text = "Collapse All";
         this._tsCollapse.Visible = false;
         this._tsCollapse.Click += new System.EventHandler(this.Click_miViewCollapse);
         // 
         // _tsExpand
         // 
         this._tsExpand.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
         this._tsExpand.Image = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.Bug_16;
         this._tsExpand.ImageTransparentColor = System.Drawing.Color.Magenta;
         this._tsExpand.Name = "_tsExpand";
         this._tsExpand.Size = new System.Drawing.Size(23, 22);
         this._tsExpand.Text = "Expand All";
         this._tsExpand.Visible = false;
         this._tsExpand.Click += new System.EventHandler(this.Click_miViewExpand);
         // 
         // _tsGroupBy
         // 
         this._tsGroupBy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
         this._tsGroupBy.Image = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.Bug_16;
         this._tsGroupBy.ImageTransparentColor = System.Drawing.Color.Magenta;
         this._tsGroupBy.Name = "_tsGroupBy";
         this._tsGroupBy.Size = new System.Drawing.Size(23, 22);
         this._tsGroupBy.Text = "View Group By";
         this._tsGroupBy.Visible = false;
         this._tsGroupBy.Click += new System.EventHandler(this.Click_miViewGroupBy);
         // 
         // _tsShowBanners
         // 
         this._tsShowBanners.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
         this._tsShowBanners.Image = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.Bug_16;
         this._tsShowBanners.ImageTransparentColor = System.Drawing.Color.Magenta;
         this._tsShowBanners.Name = "_tsShowBanners";
         this._tsShowBanners.Size = new System.Drawing.Size(23, 22);
         this._tsShowBanners.Text = "View Task Banners";
         this._tsShowBanners.Visible = false;
         this._tsShowBanners.Click += new System.EventHandler(this.Click_miViewTaskBanners);
         // 
         // toolStripSeparator24
         // 
         this.toolStripSeparator24.Name = "toolStripSeparator24";
         this.toolStripSeparator24.Size = new System.Drawing.Size(6, 25);
         // 
         // _tsHelp
         // 
         this._tsHelp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
         this._tsHelp.Image = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.Help_16;
         this._tsHelp.ImageTransparentColor = System.Drawing.Color.Magenta;
         this._tsHelp.Name = "_tsHelp";
         this._tsHelp.Size = new System.Drawing.Size(23, 22);
         this._tsHelp.Text = "Help";
         this._tsHelp.Click += new System.EventHandler(this.Click_miHelpWindow);
         // 
         // _contextMenuServer
         // 
         this._contextMenuServer.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._cmServerNewServer,
            this._cmServerNewDatabase,
            this._cmServerAttachArchive,
            this.toolStripSeparator26,
            this._cmServerEnableAuditing,
            this._cmServerDisableAuditing,
            this.toolStripSeparator27,
            this._cmServerRemove,
            this._cmServerRefresh,
            this.toolStripSeparator28,
            this._cmServerUpdateNow,
            this._cmServerCollectNow,
            this._cmServerAgentProperties,
            this.toolStripSeparator29,
            this._cmServerProperties});
         this._contextMenuServer.Name = "_contextMenuServer";
         this._contextMenuServer.Size = new System.Drawing.Size(231, 270);
         this._contextMenuServer.Opening += new System.ComponentModel.CancelEventHandler(this.Opening_contextMenuServer);
         // 
         // _cmServerNewServer
         // 
         this._cmServerNewServer.Name = "_cmServerNewServer";
         this._cmServerNewServer.Size = new System.Drawing.Size(230, 22);
         this._cmServerNewServer.Text = "New Registered SQL Server...";
         this._cmServerNewServer.Click += new System.EventHandler(this.Click_miFileNewServer);
         // 
         // _cmServerNewDatabase
         // 
         this._cmServerNewDatabase.Name = "_cmServerNewDatabase";
         this._cmServerNewDatabase.Size = new System.Drawing.Size(230, 22);
         this._cmServerNewDatabase.Text = "New Audited Databases...";
         this._cmServerNewDatabase.Click += new System.EventHandler(this.Click_miFileNewDatabase);
         // 
         // _cmServerAttachArchive
         // 
         this._cmServerAttachArchive.Name = "_cmServerAttachArchive";
         this._cmServerAttachArchive.Size = new System.Drawing.Size(230, 22);
         this._cmServerAttachArchive.Text = "Attach Archive Database...";
         this._cmServerAttachArchive.Click += new System.EventHandler(this.Click_miFileAttachArchive);
         // 
         // toolStripSeparator26
         // 
         this.toolStripSeparator26.Name = "toolStripSeparator26";
         this.toolStripSeparator26.Size = new System.Drawing.Size(227, 6);
         // 
         // _cmServerEnableAuditing
         // 
         this._cmServerEnableAuditing.Name = "_cmServerEnableAuditing";
         this._cmServerEnableAuditing.Size = new System.Drawing.Size(230, 22);
         this._cmServerEnableAuditing.Text = "Enable Auditing";
         this._cmServerEnableAuditing.Click += new System.EventHandler(this.Click_miAuditingEnable);
         // 
         // _cmServerDisableAuditing
         // 
         this._cmServerDisableAuditing.Name = "_cmServerDisableAuditing";
         this._cmServerDisableAuditing.Size = new System.Drawing.Size(230, 22);
         this._cmServerDisableAuditing.Text = "Disable Auditing";
         this._cmServerDisableAuditing.Click += new System.EventHandler(this.Click_miAuditingDisable);
         // 
         // toolStripSeparator27
         // 
         this.toolStripSeparator27.Name = "toolStripSeparator27";
         this.toolStripSeparator27.Size = new System.Drawing.Size(227, 6);
         // 
         // _cmServerRemove
         // 
         this._cmServerRemove.Name = "_cmServerRemove";
         this._cmServerRemove.Size = new System.Drawing.Size(230, 22);
         this._cmServerRemove.Text = "Remove";
         this._cmServerRemove.Click += new System.EventHandler(this.Click_miEditRemove);
         // 
         // _cmServerRefresh
         // 
         this._cmServerRefresh.Name = "_cmServerRefresh";
         this._cmServerRefresh.Size = new System.Drawing.Size(230, 22);
         this._cmServerRefresh.Text = "Refresh";
         this._cmServerRefresh.Click += new System.EventHandler(this.Click_miViewRefresh);
         // 
         // toolStripSeparator28
         // 
         this.toolStripSeparator28.Name = "toolStripSeparator28";
         this.toolStripSeparator28.Size = new System.Drawing.Size(227, 6);
         // 
         // _cmServerUpdateNow
         // 
         this._cmServerUpdateNow.Name = "_cmServerUpdateNow";
         this._cmServerUpdateNow.Size = new System.Drawing.Size(230, 22);
         this._cmServerUpdateNow.Text = "Update Audit Settings Now";
         this._cmServerUpdateNow.Click += new System.EventHandler(this.Click_miAgentUpdateNow);
         // 
         // _cmServerCollectNow
         // 
         this._cmServerCollectNow.Name = "_cmServerCollectNow";
         this._cmServerCollectNow.Size = new System.Drawing.Size(230, 22);
         this._cmServerCollectNow.Text = "Collect Audit Data Now";
         this._cmServerCollectNow.Click += new System.EventHandler(this.Click_miAuditingCollectNow);
         // 
         // _cmServerAgentProperties
         // 
         this._cmServerAgentProperties.Name = "_cmServerAgentProperties";
         this._cmServerAgentProperties.Size = new System.Drawing.Size(230, 22);
         this._cmServerAgentProperties.Text = "Agent Properties...";
         this._cmServerAgentProperties.Click += new System.EventHandler(this.Click_miAgentProperties);
         // 
         // toolStripSeparator29
         // 
         this.toolStripSeparator29.Name = "toolStripSeparator29";
         this.toolStripSeparator29.Size = new System.Drawing.Size(227, 6);
         // 
         // _cmServerProperties
         // 
         this._cmServerProperties.Name = "_cmServerProperties";
         this._cmServerProperties.Size = new System.Drawing.Size(230, 22);
         this._cmServerProperties.Text = "Properties";
         this._cmServerProperties.Click += new System.EventHandler(this.Click_miEditProperties);
         // 
         // _contextMenuRoot
         // 
         this._contextMenuRoot.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._cmRootNewServer,
            this._cmRootAttachArchive,
            this.toolStripSeparator31,
            this._cmRootRefresh});
         this._contextMenuRoot.Name = "_contextMenuServer";
         this._contextMenuRoot.Size = new System.Drawing.Size(231, 76);
         this._contextMenuRoot.Opening += new System.ComponentModel.CancelEventHandler(this.Opening_contextMenuRoot);
         // 
         // _cmRootNewServer
         // 
         this._cmRootNewServer.Name = "_cmRootNewServer";
         this._cmRootNewServer.Size = new System.Drawing.Size(230, 22);
         this._cmRootNewServer.Text = "New Registered SQL Server...";
         this._cmRootNewServer.Click += new System.EventHandler(this.Click_miFileNewServer);
         // 
         // _cmRootAttachArchive
         // 
         this._cmRootAttachArchive.Name = "_cmRootAttachArchive";
         this._cmRootAttachArchive.Size = new System.Drawing.Size(230, 22);
         this._cmRootAttachArchive.Text = "Attach Archive Database...";
         this._cmRootAttachArchive.Click += new System.EventHandler(this.Click_miFileAttachArchive);
         // 
         // toolStripSeparator31
         // 
         this.toolStripSeparator31.Name = "toolStripSeparator31";
         this.toolStripSeparator31.Size = new System.Drawing.Size(227, 6);
         // 
         // _cmRootRefresh
         // 
         this._cmRootRefresh.Name = "_cmRootRefresh";
         this._cmRootRefresh.Size = new System.Drawing.Size(230, 22);
         this._cmRootRefresh.Text = "Refresh";
         this._cmRootRefresh.Click += new System.EventHandler(this.Click_miViewRefresh);
         // 
         // _contextMenuDatabase
         // 
         this._contextMenuDatabase.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._cmDatabaseNewDatabase,
            this.toolStripSeparator34,
            this._cmDatabaseEnableAuditing,
            this._cmDatabaseDisableAuditing,
            this.toolStripSeparator35,
            this._cmDatabaseRefresh,
            this.toolStripSeparator36,
            this._cmDatabaseProperties});
         this._contextMenuDatabase.Name = "_contextMenuServer";
         this._contextMenuDatabase.Size = new System.Drawing.Size(213, 132);
         this._contextMenuDatabase.Opening += new System.ComponentModel.CancelEventHandler(this.Opening_contextMenuDatabase);
         // 
         // _cmDatabaseNewDatabase
         // 
         this._cmDatabaseNewDatabase.Name = "_cmDatabaseNewDatabase";
         this._cmDatabaseNewDatabase.Size = new System.Drawing.Size(212, 22);
         this._cmDatabaseNewDatabase.Text = "New Audited Databases...";
         this._cmDatabaseNewDatabase.Click += new System.EventHandler(this.Click_miFileNewDatabase);
         // 
         // toolStripSeparator34
         // 
         this.toolStripSeparator34.Name = "toolStripSeparator34";
         this.toolStripSeparator34.Size = new System.Drawing.Size(209, 6);
         // 
         // _cmDatabaseEnableAuditing
         // 
         this._cmDatabaseEnableAuditing.Name = "_cmDatabaseEnableAuditing";
         this._cmDatabaseEnableAuditing.Size = new System.Drawing.Size(212, 22);
         this._cmDatabaseEnableAuditing.Text = "Enable Auditing";
         this._cmDatabaseEnableAuditing.Click += new System.EventHandler(this.Click_miAuditingEnable);
         // 
         // _cmDatabaseDisableAuditing
         // 
         this._cmDatabaseDisableAuditing.Name = "_cmDatabaseDisableAuditing";
         this._cmDatabaseDisableAuditing.Size = new System.Drawing.Size(212, 22);
         this._cmDatabaseDisableAuditing.Text = "Disable Auditing";
         this._cmDatabaseDisableAuditing.Click += new System.EventHandler(this.Click_miAuditingDisable);
         // 
         // toolStripSeparator35
         // 
         this.toolStripSeparator35.Name = "toolStripSeparator35";
         this.toolStripSeparator35.Size = new System.Drawing.Size(209, 6);
         // 
         // _cmDatabaseRefresh
         // 
         this._cmDatabaseRefresh.Name = "_cmDatabaseRefresh";
         this._cmDatabaseRefresh.Size = new System.Drawing.Size(212, 22);
         this._cmDatabaseRefresh.Text = "Refresh";
         this._cmDatabaseRefresh.Click += new System.EventHandler(this.Click_miViewRefresh);
         // 
         // toolStripSeparator36
         // 
         this.toolStripSeparator36.Name = "toolStripSeparator36";
         this.toolStripSeparator36.Size = new System.Drawing.Size(209, 6);
         // 
         // _cmDatabaseProperties
         // 
         this._cmDatabaseProperties.Name = "_cmDatabaseProperties";
         this._cmDatabaseProperties.Size = new System.Drawing.Size(212, 22);
         this._cmDatabaseProperties.Text = "Properties";
         this._cmDatabaseProperties.Click += new System.EventHandler(this.Click_miEditProperties);
         // 
         // Form_Main
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.ClientSize = new System.Drawing.Size(1016, 704);
         this.Controls.Add(this._toolStripContainer);
         this.DataBindings.Add(new System.Windows.Forms.Binding("Location", global::Idera.SQLcompliance.Application.GUI.Settings.Default, "MainFormLocation", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
         this.Location = global::Idera.SQLcompliance.Application.GUI.Settings.Default.MainFormLocation;
         this.MainMenuStrip = this._menuStripMain;
         this.Name = "Form_Main";
         this.Text = "IDERA SQL complaince manager (MACHINE) [Evaluation Version]";
         this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormClosed_Main);
         this.Shown += new System.EventHandler(this.Shown_Form_Main);
         this._containerExploreActivity.ResumeLayout(false);
         this._containerAuditReports.ResumeLayout(false);
         this.ultraExplorerBarContainerControl1.ResumeLayout(false);
         this._contextMenuAdminTree.ResumeLayout(false);
         this._toolStripContainer.ContentPanel.ResumeLayout(false);
         this._toolStripContainer.TopToolStripPanel.ResumeLayout(false);
         this._toolStripContainer.TopToolStripPanel.PerformLayout();
         this._toolStripContainer.ResumeLayout(false);
         this._toolStripContainer.PerformLayout();
         this._splitContainer.Panel1.ResumeLayout(false);
         this._splitContainer.Panel2.ResumeLayout(false);
         this._splitContainer.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this._explorerBar)).EndInit();
         this._explorerBar.ResumeLayout(false);
         this._menuStripMain.ResumeLayout(false);
         this._menuStripMain.PerformLayout();
         this._toolStripMain.ResumeLayout(false);
         this._toolStripMain.PerformLayout();
         this._contextMenuServer.ResumeLayout(false);
         this._contextMenuRoot.ResumeLayout(false);
         this._contextMenuDatabase.ResumeLayout(false);
         this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.ToolStripContainer _toolStripContainer;
      private System.Windows.Forms.MenuStrip _menuStripMain;
      private System.Windows.Forms.ToolStripMenuItem _menuFile;
      private System.Windows.Forms.ToolStripMenuItem _menuEdit;
      private System.Windows.Forms.ToolStripMenuItem _menuView;
      private System.Windows.Forms.ToolStripMenuItem _menuAuditing;
      private System.Windows.Forms.ToolStripMenuItem _menuAlerting;
      private System.Windows.Forms.ToolStripMenuItem _menuAgent;
      private System.Windows.Forms.ToolStripMenuItem _menuTools;
      private System.Windows.Forms.ToolStripMenuItem _menuHelp;
      private System.Windows.Forms.ToolStrip _toolStripMain;
      private System.Windows.Forms.SplitContainer _splitContainer;
      private Infragistics.Win.UltraWinExplorerBar.UltraExplorerBar _explorerBar;
      private Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarContainerControl _containerExploreActivity;
      private Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarContainerControl _containerAuditReports;
      private System.Windows.Forms.TreeView _treeServers;
      private System.Windows.Forms.TreeView _treeReports;
      private Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarContainerControl ultraExplorerBarContainerControl1;
      private System.Windows.Forms.TreeView _treeAdmin;
      private Idera.SQLcompliance.Application.GUI.Controls.ServerView _registeredServerView;
      private Idera.SQLcompliance.Application.GUI.Controls.AlertRulesView _alertRulesView;
      private Idera.SQLcompliance.Application.GUI.Controls.LoginsView _loginsView;
      private Idera.SQLcompliance.Application.GUI.Controls.EventFiltersView _eventFiltersView;
      private System.Windows.Forms.ToolStripMenuItem _miFileConnect;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
      private System.Windows.Forms.ToolStripMenuItem _miFileNewServer;
      private System.Windows.Forms.ToolStripMenuItem _miFileNewDatabase;
      private System.Windows.Forms.ToolStripMenuItem _miFileNewLogin;
      private System.Windows.Forms.ToolStripMenuItem _miFileNewAlertRule;
      private System.Windows.Forms.ToolStripMenuItem _miFileNewFilter;
      private System.Windows.Forms.ToolStripMenuItem _miFileAttachArchive;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
      private System.Windows.Forms.ToolStripMenuItem _miFileLicenses;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
      private System.Windows.Forms.ToolStripMenuItem _miFilePrint;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
      private System.Windows.Forms.ToolStripMenuItem _miFileExit;
      private System.Windows.Forms.ToolStripMenuItem _miEditRemove;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
      private System.Windows.Forms.ToolStripMenuItem _miEditProperties;
      private System.Windows.Forms.ToolStripMenuItem _miViewSetFilter;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
      private System.Windows.Forms.ToolStripMenuItem _miViewConsoleTree;
      private System.Windows.Forms.ToolStripMenuItem _miViewToolbar;
      private System.Windows.Forms.ToolStripMenuItem _miViewCommonTasks;
      private System.Windows.Forms.ToolStripMenuItem _miViewTaskBanners;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
      private System.Windows.Forms.ToolStripMenuItem _miViewCollapse;
      private System.Windows.Forms.ToolStripMenuItem _miViewExpand;
      private System.Windows.Forms.ToolStripMenuItem _miViewGroupBy;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
      private System.Windows.Forms.ToolStripMenuItem _miViewRefresh;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
      private System.Windows.Forms.ToolStripMenuItem _miViewPreferences;
      private System.Windows.Forms.ToolStripMenuItem _miAuditingEnable;
      private System.Windows.Forms.ToolStripMenuItem _miAuditingDisable;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
      private System.Windows.Forms.ToolStripMenuItem _miAuditingArchive;
      private System.Windows.Forms.ToolStripMenuItem _miAuditingArchiveNow;
      private System.Windows.Forms.ToolStripMenuItem _miAuditingArchiveGroomNow;
      private System.Windows.Forms.ToolStripMenuItem _miAuditingArchivePreferences;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
      private System.Windows.Forms.ToolStripMenuItem _miAuditingCollectNow;
      private System.Windows.Forms.ToolStripMenuItem _miAuditingCheckIntegrity;
      private System.Windows.Forms.ToolStripMenuItem _miAuditingCaptureSnapshot;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator12;
      private System.Windows.Forms.ToolStripMenuItem _miAuditingSnapshotPreferences;
      private System.Windows.Forms.ToolStripMenuItem _miAuditingLoginFilters;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator13;
      private System.Windows.Forms.ToolStripMenuItem _miAuditingCollectionServerStatus;
      private System.Windows.Forms.ToolStripMenuItem _miAuditingConfigureRepository;
      private System.Windows.Forms.ToolStripMenuItem _miAuditingConfigureReports;
      private System.Windows.Forms.ToolStripMenuItem _miAlertingConfigureEmail;
      private System.Windows.Forms.ToolStripMenuItem _miAlertingGroom;
      private System.Windows.Forms.ToolStripMenuItem _miAgentDeploy;
      private System.Windows.Forms.ToolStripMenuItem _miAgentUpgrade;
      private System.Windows.Forms.ToolStripMenuItem _miAgentCheckStatus;
      private System.Windows.Forms.ToolStripMenuItem _miAgentUpdateNow;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator14;
      private System.Windows.Forms.ToolStripMenuItem _miAgentTraceDirectory;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator15;
      private System.Windows.Forms.ToolStripMenuItem _miAgentProperties;
      private System.Windows.Forms.ToolStripMenuItem _miHelpWindow;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator16;
      private System.Windows.Forms.ToolStripMenuItem _miHelpAuditing;
      private System.Windows.Forms.ToolStripMenuItem _miHelpSecurity;
      private System.Windows.Forms.ToolStripMenuItem _miHelpReports;
      private System.Windows.Forms.ToolStripMenuItem _miHelpAlerting;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator17;
      private System.Windows.Forms.ToolStripMenuItem _miHelpContents;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator18;
      private System.Windows.Forms.ToolStripMenuItem _miHelpSearch;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator19;
      private System.Windows.Forms.ToolStripMenuItem _miHelpMore;
      private System.Windows.Forms.ToolStripMenuItem _miHelpTechSupport;
      private System.Windows.Forms.ToolStripMenuItem _miHelpIderaProducts;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator20;
      private System.Windows.Forms.ToolStripMenuItem _miHelpAbout;
      private Idera.SQLcompliance.Application.GUI.Controls.MainTabView _mainTabView;
      private System.Windows.Forms.ToolStripSplitButton _tsNew;
      private System.Windows.Forms.ToolStripButton _tsPrint;
      private System.Windows.Forms.ToolStripButton _tsDelete;
      private System.Windows.Forms.ToolStripButton _tsProperties;
      private System.Windows.Forms.ToolStripButton _tsRefresh;
      private System.Windows.Forms.ToolStripButton _tsHelp;
      private System.Windows.Forms.ToolStripMenuItem _tsNewServer;
      private System.Windows.Forms.ToolStripMenuItem _tsNewDatabase;
      private System.Windows.Forms.ToolStripMenuItem _tsNewLogin;
      private System.Windows.Forms.ToolStripMenuItem _tsNewAlertRule;
      private System.Windows.Forms.ToolStripMenuItem _tsNewFilter;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator21;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator22;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator23;
      private System.Windows.Forms.ToolStripButton _tsShowTree;
      private System.Windows.Forms.ToolStripButton _tsCollapse;
      private System.Windows.Forms.ToolStripButton _tsExpand;
      private System.Windows.Forms.ToolStripButton _tsGroupBy;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator24;
      private Idera.SQLcompliance.Application.GUI.Controls.ChangeLogViewTab _changeLogView;
      private System.Windows.Forms.ToolStripMenuItem _miFileServer1;
      private System.Windows.Forms.ToolStripMenuItem _miFileServer2;
      private System.Windows.Forms.ToolStripMenuItem _miFileServer3;
      private System.Windows.Forms.ToolStripMenuItem _miFileServer4;
      private System.Windows.Forms.ToolStripMenuItem _miFileServer5;
      private System.Windows.Forms.ToolStripSeparator _miFileRUSep;
      private Idera.SQLcompliance.Application.GUI.Controls.ActivityLogViewTab _activityLogView;
      private System.Windows.Forms.ContextMenuStrip _contextMenuAdminTree;
      private System.Windows.Forms.ToolStripMenuItem _cmNewServer;
      private System.Windows.Forms.ToolStripMenuItem _cmNewDatabase;
      private System.Windows.Forms.ToolStripMenuItem _cmNewLogin;
      private System.Windows.Forms.ToolStripMenuItem _cmNewAlertRule;
      private System.Windows.Forms.ToolStripMenuItem _cmNewEventFilter;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator25;
      private System.Windows.Forms.ToolStripMenuItem _cmRefresh;
      private System.Windows.Forms.ToolStripButton _tsShowBanners;
      private System.Windows.Forms.ContextMenuStrip _contextMenuServer;
      private System.Windows.Forms.ToolStripMenuItem _cmServerNewServer;
      private System.Windows.Forms.ToolStripMenuItem _cmServerNewDatabase;
      private System.Windows.Forms.ToolStripMenuItem _cmServerAttachArchive;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator26;
      private System.Windows.Forms.ToolStripMenuItem _cmServerEnableAuditing;
      private System.Windows.Forms.ToolStripMenuItem _cmServerDisableAuditing;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator27;
      private System.Windows.Forms.ToolStripMenuItem _cmServerRemove;
      private System.Windows.Forms.ToolStripMenuItem _cmServerRefresh;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator28;
      private System.Windows.Forms.ToolStripMenuItem _cmServerUpdateNow;
      private System.Windows.Forms.ToolStripMenuItem _cmServerCollectNow;
      private System.Windows.Forms.ToolStripMenuItem _cmServerAgentProperties;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator29;
      private System.Windows.Forms.ToolStripMenuItem _cmServerProperties;
      private System.Windows.Forms.ContextMenuStrip _contextMenuRoot;
      private System.Windows.Forms.ToolStripMenuItem _cmRootNewServer;
      private System.Windows.Forms.ToolStripMenuItem _cmRootAttachArchive;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator31;
      private System.Windows.Forms.ToolStripMenuItem _cmRootRefresh;
      private System.Windows.Forms.ContextMenuStrip _contextMenuDatabase;
      private System.Windows.Forms.ToolStripMenuItem _cmDatabaseNewDatabase;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator34;
      private System.Windows.Forms.ToolStripMenuItem _cmDatabaseEnableAuditing;
      private System.Windows.Forms.ToolStripMenuItem _cmDatabaseDisableAuditing;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator35;
      private System.Windows.Forms.ToolStripMenuItem _cmDatabaseRefresh;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator36;
      private System.Windows.Forms.ToolStripMenuItem _cmDatabaseProperties;
      private System.Windows.Forms.ToolStripMenuItem _cmExportEventFilters;
      private System.Windows.Forms.ToolStripMenuItem _cmExportAlertRules;
      private System.Windows.Forms.ToolStripSeparator _cmExportSeparator;
      private System.Windows.Forms.ToolStripMenuItem _miFileImport;
      private Idera.SQLcompliance.Application.GUI.Controls.ReportCategoryView _reportCategoryView;
      private Idera.SQLcompliance.Application.GUI.Controls.ReportView _reportView;
   }
}