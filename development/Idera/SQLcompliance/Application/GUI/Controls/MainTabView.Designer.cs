namespace Idera.SQLcompliance.Application.GUI.Controls
{
   partial class MainTabView
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
         this._alertViewTab = new Idera.SQLcompliance.Application.GUI.Controls.AlertViewTab();
         this._eventViewTab = new Idera.SQLcompliance.Application.GUI.Controls.EventViewTab();
         this._archiveViewTab = new Idera.SQLcompliance.Application.GUI.Controls.EventViewTab();
         this._changeLogViewTab = new Idera.SQLcompliance.Application.GUI.Controls.ChangeLogViewTab();
         this._activityLogViewTab = new Idera.SQLcompliance.Application.GUI.Controls.ActivityLogViewTab();
         this._tabControl = new Qios.DevSuite.Components.QTabControl();
         this._tabSummary = new Qios.DevSuite.Components.QTabPage();
         this._pnlSummaryTabHeader = new Qios.DevSuite.Components.QPanel();
         this._serverSummaryTab = new Idera.SQLcompliance.Application.GUI.Controls.ServerSummary();
         this._databaseSummaryTab = new Idera.SQLcompliance.Application.GUI.Controls.DatabaseSummary();
         this._enterpriseSummaryTab = new Idera.SQLcompliance.Application.GUI.Controls.EnterpriseSummary();
         this._tabAlerts = new Qios.DevSuite.Components.QTabPage();
         this._pnlAlertTabHeader = new Qios.DevSuite.Components.QPanel();
         this._tabEvents = new Qios.DevSuite.Components.QTabPage();
         this._pnlEventsTabHeader = new Qios.DevSuite.Components.QPanel();
         this._tabArchive = new Qios.DevSuite.Components.QTabPage();
         this._tabChangeLog = new Qios.DevSuite.Components.QTabPage();
         this._pnlChangeLogTabHeader = new Qios.DevSuite.Components.QPanel();
         this._tabActivityLog = new Qios.DevSuite.Components.QTabPage();
         ((System.ComponentModel.ISupportInitialize)(this._tabControl)).BeginInit();
         this._tabControl.SuspendLayout();
         this._tabSummary.SuspendLayout();
         this._tabAlerts.SuspendLayout();
         this._tabEvents.SuspendLayout();
         this._tabArchive.SuspendLayout();
         this._tabChangeLog.SuspendLayout();
         this._tabActivityLog.SuspendLayout();
         this.SuspendLayout();
         // 
         // _alertViewTab
         // 
         this._alertViewTab.AlertConfiguration = null;
         this._alertViewTab.Appearance.ShowBorders = false;
         this._alertViewTab.Dock = System.Windows.Forms.DockStyle.Fill;
         this._alertViewTab.Location = new System.Drawing.Point(0, 4);
         this._alertViewTab.Name = "_alertViewTab";
         this._alertViewTab.ShowBanner = true;
         this._alertViewTab.ShowGroupBy = true;
         this._alertViewTab.Size = new System.Drawing.Size(768, 610);
         this._alertViewTab.TabIndex = 0;
         this._alertViewTab.MenuFlagChanged += new Idera.SQLcompliance.Application.GUI.Controls.MenuFlagChangedEventHandler(this.MenuFlagChanged_child);
         // 
         // _eventViewTab
         // 
         this._eventViewTab.Appearance.ShowBorders = false;
         this._eventViewTab.Dock = System.Windows.Forms.DockStyle.Fill;
         this._eventViewTab.Location = new System.Drawing.Point(0, 4);
         this._eventViewTab.Name = "_eventViewTab";
         this._eventViewTab.ShowBanner = true;
         this._eventViewTab.ShowGroupBy = true;
         this._eventViewTab.Size = new System.Drawing.Size(768, 610);
         this._eventViewTab.TabIndex = 0;
         this._eventViewTab.MenuFlagChanged += new Idera.SQLcompliance.Application.GUI.Controls.MenuFlagChangedEventHandler(this.MenuFlagChanged_child);
         // 
         // _archiveViewTab
         // 
         this._archiveViewTab.Appearance.ShowBorders = false;
         this._archiveViewTab.Dock = System.Windows.Forms.DockStyle.Fill;
         this._archiveViewTab.Location = new System.Drawing.Point(0, 0);
         this._archiveViewTab.Name = "_archiveViewTab";
         this._archiveViewTab.ShowBanner = true;
         this._archiveViewTab.ShowGroupBy = true;
         this._archiveViewTab.Size = new System.Drawing.Size(768, 614);
         this._archiveViewTab.TabIndex = 0;
         this._archiveViewTab.MenuFlagChanged += new Idera.SQLcompliance.Application.GUI.Controls.MenuFlagChangedEventHandler(this.MenuFlagChanged_child);
         // 
         // _changeLogViewTab
         // 
         this._changeLogViewTab.Appearance.ShowBorders = false;
         this._changeLogViewTab.Dock = System.Windows.Forms.DockStyle.Fill;
         this._changeLogViewTab.Location = new System.Drawing.Point(0, 4);
         this._changeLogViewTab.Name = "_changeLogViewTab";
         this._changeLogViewTab.ShowBanner = true;
         this._changeLogViewTab.ShowGroupBy = true;
         this._changeLogViewTab.Size = new System.Drawing.Size(768, 610);
         this._changeLogViewTab.TabIndex = 0;
         this._changeLogViewTab.MenuFlagChanged += new Idera.SQLcompliance.Application.GUI.Controls.MenuFlagChangedEventHandler(this.MenuFlagChanged_child);
         // 
         // _activityLogViewTab
         // 
         this._activityLogViewTab.Appearance.ShowBorders = false;
         this._activityLogViewTab.Dock = System.Windows.Forms.DockStyle.Fill;
         this._activityLogViewTab.Location = new System.Drawing.Point(0, 0);
         this._activityLogViewTab.Name = "_activityLogViewTab";
         this._activityLogViewTab.ShowBanner = true;
         this._activityLogViewTab.ShowGroupBy = true;
         this._activityLogViewTab.Size = new System.Drawing.Size(768, 614);
         this._activityLogViewTab.TabIndex = 0;
         this._activityLogViewTab.MenuFlagChanged += new Idera.SQLcompliance.Application.GUI.Controls.MenuFlagChangedEventHandler(this.MenuFlagChanged_child);
         // 
         // _tabControl
         // 
         this._tabControl.ActiveTabPage = this._tabSummary;
         this._tabControl.Appearance.ShowBorders = false;
         this._tabControl.ColorScheme.TabButtonActiveBackground1.SetColor("LunaBlue", System.Drawing.Color.FromArgb(((int)(((byte)(186)))), ((int)(((byte)(211)))), ((int)(((byte)(245))))), false);
         this._tabControl.ColorScheme.TabButtonActiveBackground1.SetColor("LunaOlive", System.Drawing.Color.FromArgb(((int)(((byte)(218)))), ((int)(((byte)(227)))), ((int)(((byte)(187))))), false);
         this._tabControl.ColorScheme.TabButtonActiveBackground1.SetColor("LunaSilver", System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(212)))), ((int)(((byte)(226))))), false);
         this._tabControl.Controls.Add(this._tabSummary);
         this._tabControl.Controls.Add(this._tabAlerts);
         this._tabControl.Controls.Add(this._tabEvents);
         this._tabControl.Controls.Add(this._tabArchive);
         this._tabControl.Controls.Add(this._tabChangeLog);
         this._tabControl.Controls.Add(this._tabActivityLog);
         this._tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this._tabControl.FocusTabButtons = false;
         this._tabControl.FontScope = Qios.DevSuite.Components.QFontScope.Local;
         this._tabControl.LocalFont = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this._tabControl.Location = new System.Drawing.Point(0, 0);
         this._tabControl.Name = "_tabControl";
         this._tabControl.PersistGuid = new System.Guid("b81472c2-f523-4a0c-812a-a7cf1014a466");
         this._tabControl.Size = new System.Drawing.Size(768, 643);
         this._tabControl.TabIndex = 1;
         this._tabControl.TabStripTopConfiguration.ButtonConfiguration.Appearance.BackgroundStyle = Qios.DevSuite.Components.QColorStyle.Solid;
         this._tabControl.TabStripTopConfiguration.ButtonSpacing = -10;
         this._tabControl.ActivePageChanged += new Qios.DevSuite.Components.QTabPageChangeEventHandler(this.ActivePageChanged_tabControl);
         // 
         // _tabSummary
         // 
         this._tabSummary.ButtonOrder = 0;
         this._tabSummary.Controls.Add(this._pnlSummaryTabHeader);
         this._tabSummary.Controls.Add(this._serverSummaryTab);
         this._tabSummary.Controls.Add(this._databaseSummaryTab);
         this._tabSummary.Controls.Add(this._enterpriseSummaryTab);
         this._tabSummary.Location = new System.Drawing.Point(0, 29);
         this._tabSummary.Name = "_tabSummary";
         this._tabSummary.PersistGuid = new System.Guid("bdcde701-848d-4dc1-90d0-c175f82be2e6");
         this._tabSummary.Size = new System.Drawing.Size(768, 614);
         this._tabSummary.Text = "Summary";
         // 
         // _pnlSummaryTabHeader
         // 
         this._pnlSummaryTabHeader.Appearance.BackgroundStyle = Qios.DevSuite.Components.QColorStyle.Solid;
         this._pnlSummaryTabHeader.Appearance.ShowBorderLeft = false;
         this._pnlSummaryTabHeader.Appearance.ShowBorderRight = false;
         this._pnlSummaryTabHeader.Appearance.ShowBorderTop = false;
         this._pnlSummaryTabHeader.ColorScheme.PanelBackground1.SetColor("Default", System.Drawing.SystemColors.Control, false);
         this._pnlSummaryTabHeader.ColorScheme.PanelBackground1.SetColor("LunaBlue", System.Drawing.Color.FromArgb(((int)(((byte)(186)))), ((int)(((byte)(211)))), ((int)(((byte)(245))))), false);
         this._pnlSummaryTabHeader.ColorScheme.PanelBackground1.SetColor("LunaOlive", System.Drawing.Color.FromArgb(((int)(((byte)(218)))), ((int)(((byte)(227)))), ((int)(((byte)(187))))), false);
         this._pnlSummaryTabHeader.ColorScheme.PanelBackground1.SetColor("LunaSilver", System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(212)))), ((int)(((byte)(226))))), false);
         this._pnlSummaryTabHeader.ColorScheme.PanelBorder.SetColor("LunaBlue", System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(45)))), ((int)(((byte)(150))))), false);
         this._pnlSummaryTabHeader.Dock = System.Windows.Forms.DockStyle.Top;
         this._pnlSummaryTabHeader.Location = new System.Drawing.Point(0, 0);
         this._pnlSummaryTabHeader.MinimumClientSize = new System.Drawing.Size(0, 0);
         this._pnlSummaryTabHeader.Name = "_pnlSummaryTabHeader";
         this._pnlSummaryTabHeader.Size = new System.Drawing.Size(768, 4);
         this._pnlSummaryTabHeader.TabIndex = 8;
         this._pnlSummaryTabHeader.Text = "qPanel1";
         // 
         // _serverSummaryTab
         // 
         this._serverSummaryTab.Appearance.ShowBorders = false;
         this._serverSummaryTab.Dock = System.Windows.Forms.DockStyle.Fill;
         this._serverSummaryTab.Location = new System.Drawing.Point(0, 0);
         this._serverSummaryTab.Name = "_serverSummaryTab";
         this._serverSummaryTab.ShowBanner = true;
         this._serverSummaryTab.ShowGroupBy = true;
         this._serverSummaryTab.Size = new System.Drawing.Size(768, 614);
         this._serverSummaryTab.TabIndex = 10;
         this._serverSummaryTab.Text = "serverSummary1";
         this._serverSummaryTab.Visible = false;
         // 
         // _databaseSummaryTab
         // 
         this._databaseSummaryTab.Appearance.ShowBorders = false;
         this._databaseSummaryTab.Dock = System.Windows.Forms.DockStyle.Fill;
         this._databaseSummaryTab.Location = new System.Drawing.Point(0, 0);
         this._databaseSummaryTab.Name = "_databaseSummaryTab";
         this._databaseSummaryTab.ShowBanner = true;
         this._databaseSummaryTab.ShowGroupBy = true;
         this._databaseSummaryTab.Size = new System.Drawing.Size(768, 614);
         this._databaseSummaryTab.TabIndex = 11;
         this._databaseSummaryTab.Text = "databaseSummary1";
         // 
         // _enterpriseSummaryTab
         // 
         this._enterpriseSummaryTab.Appearance.ShowBorders = false;
         this._enterpriseSummaryTab.Dock = System.Windows.Forms.DockStyle.Fill;
         this._enterpriseSummaryTab.Location = new System.Drawing.Point(0, 0);
         this._enterpriseSummaryTab.Name = "_enterpriseSummaryTab";
         this._enterpriseSummaryTab.ShowBanner = true;
         this._enterpriseSummaryTab.ShowGroupBy = true;
         this._enterpriseSummaryTab.Size = new System.Drawing.Size(768, 614);
         this._enterpriseSummaryTab.TabIndex = 9;
         this._enterpriseSummaryTab.Text = "enterpriseSummary1";
         // 
         // _tabAlerts
         // 
         this._tabAlerts.ButtonOrder = 1;
         this._tabAlerts.Controls.Add(this._alertViewTab);
         this._tabAlerts.Controls.Add(this._pnlAlertTabHeader);
         this._tabAlerts.Location = new System.Drawing.Point(0, 29);
         this._tabAlerts.Name = "_tabAlerts";
         this._tabAlerts.PersistGuid = new System.Guid("7aca6b0c-6065-4e52-a847-d4df929e7581");
         this._tabAlerts.Size = new System.Drawing.Size(768, 614);
         this._tabAlerts.Text = "Alerts";
         // 
         // _pnlAlertTabHeader
         // 
         this._pnlAlertTabHeader.Appearance.BackgroundStyle = Qios.DevSuite.Components.QColorStyle.Solid;
         this._pnlAlertTabHeader.Appearance.ShowBorderLeft = false;
         this._pnlAlertTabHeader.Appearance.ShowBorderRight = false;
         this._pnlAlertTabHeader.Appearance.ShowBorderTop = false;
         this._pnlAlertTabHeader.ColorScheme.PanelBackground1.SetColor("Default", System.Drawing.SystemColors.Control, false);
         this._pnlAlertTabHeader.ColorScheme.PanelBackground1.SetColor("LunaBlue", System.Drawing.Color.FromArgb(((int)(((byte)(186)))), ((int)(((byte)(211)))), ((int)(((byte)(245))))), false);
         this._pnlAlertTabHeader.ColorScheme.PanelBackground1.SetColor("LunaOlive", System.Drawing.Color.FromArgb(((int)(((byte)(218)))), ((int)(((byte)(227)))), ((int)(((byte)(187))))), false);
         this._pnlAlertTabHeader.ColorScheme.PanelBackground1.SetColor("LunaSilver", System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(212)))), ((int)(((byte)(226))))), false);
         this._pnlAlertTabHeader.ColorScheme.PanelBorder.SetColor("LunaBlue", System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(45)))), ((int)(((byte)(150))))), false);
         this._pnlAlertTabHeader.Dock = System.Windows.Forms.DockStyle.Top;
         this._pnlAlertTabHeader.Location = new System.Drawing.Point(0, 0);
         this._pnlAlertTabHeader.MinimumClientSize = new System.Drawing.Size(0, 0);
         this._pnlAlertTabHeader.Name = "_pnlAlertTabHeader";
         this._pnlAlertTabHeader.Size = new System.Drawing.Size(768, 4);
         this._pnlAlertTabHeader.TabIndex = 7;
         this._pnlAlertTabHeader.Text = "qPanel1";
         this._pnlAlertTabHeader.Visible = false;
         // 
         // _tabEvents
         // 
         this._tabEvents.ButtonOrder = 2;
         this._tabEvents.Controls.Add(this._eventViewTab);
         this._tabEvents.Controls.Add(this._pnlEventsTabHeader);
         this._tabEvents.Location = new System.Drawing.Point(0, 29);
         this._tabEvents.Name = "_tabEvents";
         this._tabEvents.PersistGuid = new System.Guid("7decb6ba-4cc1-4c3f-a335-17d9c272ed1b");
         this._tabEvents.Size = new System.Drawing.Size(768, 614);
         this._tabEvents.Text = "Audit Events";
         // 
         // _pnlEventsTabHeader
         // 
         this._pnlEventsTabHeader.Appearance.BackgroundStyle = Qios.DevSuite.Components.QColorStyle.Solid;
         this._pnlEventsTabHeader.Appearance.ShowBorderLeft = false;
         this._pnlEventsTabHeader.Appearance.ShowBorderRight = false;
         this._pnlEventsTabHeader.Appearance.ShowBorderTop = false;
         this._pnlEventsTabHeader.ColorScheme.PanelBackground1.SetColor("Default", System.Drawing.SystemColors.Control, false);
         this._pnlEventsTabHeader.ColorScheme.PanelBackground1.SetColor("LunaBlue", System.Drawing.Color.FromArgb(((int)(((byte)(186)))), ((int)(((byte)(211)))), ((int)(((byte)(245))))), false);
         this._pnlEventsTabHeader.ColorScheme.PanelBackground1.SetColor("LunaOlive", System.Drawing.Color.FromArgb(((int)(((byte)(218)))), ((int)(((byte)(227)))), ((int)(((byte)(187))))), false);
         this._pnlEventsTabHeader.ColorScheme.PanelBackground1.SetColor("LunaSilver", System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(212)))), ((int)(((byte)(226))))), false);
         this._pnlEventsTabHeader.ColorScheme.PanelBorder.SetColor("LunaBlue", System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(45)))), ((int)(((byte)(150))))), false);
         this._pnlEventsTabHeader.Dock = System.Windows.Forms.DockStyle.Top;
         this._pnlEventsTabHeader.Location = new System.Drawing.Point(0, 0);
         this._pnlEventsTabHeader.MinimumClientSize = new System.Drawing.Size(0, 0);
         this._pnlEventsTabHeader.Name = "_pnlEventsTabHeader";
         this._pnlEventsTabHeader.Size = new System.Drawing.Size(768, 4);
         this._pnlEventsTabHeader.TabIndex = 8;
         this._pnlEventsTabHeader.Text = "qPanel1";
         this._pnlEventsTabHeader.Visible = false;
         // 
         // _tabArchive
         // 
         this._tabArchive.ButtonOrder = 3;
         this._tabArchive.Controls.Add(this._archiveViewTab);
         this._tabArchive.Location = new System.Drawing.Point(0, 29);
         this._tabArchive.Name = "_tabArchive";
         this._tabArchive.PersistGuid = new System.Guid("d8753db9-a6fc-4e88-8bd3-4262dd9e40ad");
         this._tabArchive.Size = new System.Drawing.Size(768, 614);
         this._tabArchive.Text = "Archived Events";
         // 
         // _tabChangeLog
         // 
         this._tabChangeLog.ButtonOrder = 4;
         this._tabChangeLog.Controls.Add(this._changeLogViewTab);
         this._tabChangeLog.Controls.Add(this._pnlChangeLogTabHeader);
         this._tabChangeLog.Location = new System.Drawing.Point(0, 29);
         this._tabChangeLog.Name = "_tabChangeLog";
         this._tabChangeLog.PersistGuid = new System.Guid("445d8249-5d97-4644-a9b4-90d508a10e62");
         this._tabChangeLog.Size = new System.Drawing.Size(768, 614);
         this._tabChangeLog.Text = "Change Log";
         // 
         // _pnlChangeLogTabHeader
         // 
         this._pnlChangeLogTabHeader.Appearance.BackgroundStyle = Qios.DevSuite.Components.QColorStyle.Solid;
         this._pnlChangeLogTabHeader.Appearance.ShowBorderLeft = false;
         this._pnlChangeLogTabHeader.Appearance.ShowBorderRight = false;
         this._pnlChangeLogTabHeader.Appearance.ShowBorderTop = false;
         this._pnlChangeLogTabHeader.ColorScheme.PanelBackground1.SetColor("Default", System.Drawing.SystemColors.Control, false);
         this._pnlChangeLogTabHeader.ColorScheme.PanelBackground1.SetColor("LunaBlue", System.Drawing.Color.FromArgb(((int)(((byte)(186)))), ((int)(((byte)(211)))), ((int)(((byte)(245))))), false);
         this._pnlChangeLogTabHeader.ColorScheme.PanelBackground1.SetColor("LunaOlive", System.Drawing.Color.FromArgb(((int)(((byte)(218)))), ((int)(((byte)(227)))), ((int)(((byte)(187))))), false);
         this._pnlChangeLogTabHeader.ColorScheme.PanelBackground1.SetColor("LunaSilver", System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(212)))), ((int)(((byte)(226))))), false);
         this._pnlChangeLogTabHeader.ColorScheme.PanelBorder.SetColor("LunaBlue", System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(45)))), ((int)(((byte)(150))))), false);
         this._pnlChangeLogTabHeader.Dock = System.Windows.Forms.DockStyle.Top;
         this._pnlChangeLogTabHeader.Location = new System.Drawing.Point(0, 0);
         this._pnlChangeLogTabHeader.MinimumClientSize = new System.Drawing.Size(0, 0);
         this._pnlChangeLogTabHeader.Name = "_pnlChangeLogTabHeader";
         this._pnlChangeLogTabHeader.Size = new System.Drawing.Size(768, 4);
         this._pnlChangeLogTabHeader.TabIndex = 8;
         this._pnlChangeLogTabHeader.Text = "qPanel1";
         this._pnlChangeLogTabHeader.Visible = false;
         // 
         // _tabActivityLog
         // 
         this._tabActivityLog.ButtonOrder = 5;
         this._tabActivityLog.Controls.Add(this._activityLogViewTab);
         this._tabActivityLog.Location = new System.Drawing.Point(0, 29);
         this._tabActivityLog.Name = "_tabActivityLog";
         this._tabActivityLog.PersistGuid = new System.Guid("a6d685a5-e6e8-4cf1-aab8-bd14978de39e");
         this._tabActivityLog.Size = new System.Drawing.Size(768, 614);
         this._tabActivityLog.Text = "Activity Log";
         // 
         // MainTabView
         // 
         this.ColorScheme.PanelBorder.SetColor("LunaBlue", System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(45)))), ((int)(((byte)(150))))), false);
         this.ColorScheme.PanelBorder.SetColor("LunaOlive", System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(128)))), ((int)(((byte)(88))))), false);
         this.ColorScheme.PanelBorder.SetColor("LunaSilver", System.Drawing.Color.FromArgb(((int)(((byte)(124)))), ((int)(((byte)(124)))), ((int)(((byte)(148))))), false);
         this.Name = "MainTabView";
         this.ShowBanner = false;
         this.ShowGroupBy = false;
         this.Size = new System.Drawing.Size(770, 670);
         this.Controls.Add(this._tabControl);
         ((System.ComponentModel.ISupportInitialize)(this._tabControl)).EndInit();
         this._tabControl.ResumeLayout(false);
         this._tabSummary.ResumeLayout(false);
         this._tabAlerts.ResumeLayout(false);
         this._tabEvents.ResumeLayout(false);
         this._tabArchive.ResumeLayout(false);
         this._tabChangeLog.ResumeLayout(false);
         this._tabActivityLog.ResumeLayout(false);
         this.ResumeLayout(false);

      }

      #endregion

      private AlertViewTab _alertViewTab;
      private EventViewTab _eventViewTab;
      private ChangeLogViewTab _changeLogViewTab;
      private ActivityLogViewTab _activityLogViewTab;
      private EventViewTab _archiveViewTab;
      private Qios.DevSuite.Components.QTabControl _tabControl;
      private Qios.DevSuite.Components.QTabPage _tabSummary;
      private Qios.DevSuite.Components.QTabPage _tabAlerts;
      private Qios.DevSuite.Components.QTabPage _tabEvents;
      private Qios.DevSuite.Components.QTabPage _tabArchive;
      private Qios.DevSuite.Components.QTabPage _tabChangeLog;
      private Qios.DevSuite.Components.QTabPage _tabActivityLog;
      private Qios.DevSuite.Components.QPanel _pnlAlertTabHeader;
      private Qios.DevSuite.Components.QPanel _pnlSummaryTabHeader;
      private Qios.DevSuite.Components.QPanel _pnlEventsTabHeader;
      private Qios.DevSuite.Components.QPanel _pnlChangeLogTabHeader;
      private EnterpriseSummary _enterpriseSummaryTab;
      private ServerSummary _serverSummaryTab;
      private DatabaseSummary _databaseSummaryTab;
   }
}
