namespace Idera.SQLcompliance.Application.GUI.Controls
{
   partial class ServerSummary
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

      #region Component Designer generated code

      /// <summary> 
      /// Required method for Designer support - do not modify 
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.components = new System.ComponentModel.Container();
         System.Windows.Forms.Label @__lblLastArchived;
         System.Windows.Forms.Label @__lblLastHeartbeat;
         System.Windows.Forms.Label @__lblProcessedEvents;
         System.Windows.Forms.Label _lblRecentAlerts;
         Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
         Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
         Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
         Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
         Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
         Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
         Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
         Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            //start sqlcm 5.6 - 5363
            Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance18 = new Infragistics.Win.Appearance();
            //end sqlcm 5.6 - 5363
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand2 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Events", -1);
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn22 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("startTime");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn23 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("eventTypeString");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn24 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("spid");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn25 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("applicationName");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn26 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("hostName");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn27 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("serverName");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn28 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("loginName");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn29 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("success");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn30 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("databaseName");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn31 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("dbUserName");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn32 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("objectName");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn33 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("targetLoginName");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn34 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("targetUserName");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn35 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("roleName");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn36 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ownerName");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn37 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("targetObject");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn38 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("details");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn39 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("eventCategoryString");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn40 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("privilegedUser");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn41 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("sessionLoginName");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn42 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("sqlText");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn22 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("startTime");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn23 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("eventTypeString");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn24 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("spid");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn25 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("applicationName");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn26 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("hostName");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn27 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("serverName");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn28 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("loginName");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn29 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("success");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn30 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("databaseName");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn31 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("dbUserName");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn32 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("objectName");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn33 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("targetLoginName");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn34 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("targetUserName");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn35 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("roleName");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn36 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("ownerName");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn37 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("targetObject");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn38 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("details");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn39 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("eventCategoryString");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn40 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("privilegedUser");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn41 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("sessionLoginName");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn42 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("sqlText");
         this._tabControlReportCard = new Qios.DevSuite.Components.QTabControl();
         this._tabAlerts = new Qios.DevSuite.Components.QTabPage();
         this._chartAlerts = new ChartFX.WinForms.Chart();
         this._lblAlertStatus = new Infragistics.Win.FormattedLinkLabel.UltraFormattedLinkLabel();
         this._tabPrivUser = new Qios.DevSuite.Components.QTabPage();
         this._chartPrivUser = new ChartFX.WinForms.Chart();
         this._lblPrivUserStatus = new Infragistics.Win.FormattedLinkLabel.UltraFormattedLinkLabel();
         this._tabFailedLogins = new Qios.DevSuite.Components.QTabPage();
            //start sqlcm 5.6 - start
            this._tabLogins = new Qios.DevSuite.Components.QTabPage();
            this._tabLogouts = new Qios.DevSuite.Components.QTabPage();
            this._chartLogins = new ChartFX.WinForms.Chart();
            this._chartLogouts = new ChartFX.WinForms.Chart();
            //end sqlcm 5.6 - end
            this._chartFailedLogins = new ChartFX.WinForms.Chart();
         this._lblFailedLoginStatus = new Infragistics.Win.FormattedLinkLabel.UltraFormattedLinkLabel();
            //start sqlcm 5.6 - 5363
            this._lblLoginStatus = new Infragistics.Win.FormattedLinkLabel.UltraFormattedLinkLabel();
            this._lblLogoutStatus = new Infragistics.Win.FormattedLinkLabel.UltraFormattedLinkLabel();
            //end sqlcm 5.6 - 5363
            this._tabDDL = new Qios.DevSuite.Components.QTabPage();
         this._chartDDL = new ChartFX.WinForms.Chart();
         this._lblDDLStatus = new Infragistics.Win.FormattedLinkLabel.UltraFormattedLinkLabel();
         this._tabSecurity = new Qios.DevSuite.Components.QTabPage();
         this._chartSecurity = new ChartFX.WinForms.Chart();
         this._lblSecurityStatus = new Infragistics.Win.FormattedLinkLabel.UltraFormattedLinkLabel();
         this._tabActivity = new Qios.DevSuite.Components.QTabPage();
         this._chartActivity = new ChartFX.WinForms.Chart();
         this._lblExcessiveActivityStatus = new Infragistics.Win.FormattedLinkLabel.UltraFormattedLinkLabel();
         this.qShape1 = new Qios.DevSuite.Components.QShape();
         this._lblReportCard = new System.Windows.Forms.Label();
         this._lblLastHeartbeat = new System.Windows.Forms.Label();
         this._lblLastArchived = new System.Windows.Forms.Label();
         this._lblProcessedEvents = new System.Windows.Forms.Label();
         this._lblServerStatus = new System.Windows.Forms.Label();
         this._pbStatus = new System.Windows.Forms.PictureBox();
         this._flblServerStatus = new Infragistics.Win.FormattedLinkLabel.UltraFormattedLinkLabel();
         this._pnlStatus = new Idera.SQLcompliance.Application.GUI.Controls.GroupPanel();
         this._lblRecentAlertCount = new System.Windows.Forms.Label();
         this._headerAuditedActivity = new Qios.DevSuite.Components.QPanel();
         this._lblAuditedActivity = new System.Windows.Forms.Label();
         this._flblAuditSettings = new Infragistics.Win.FormattedLinkLabel.UltraFormattedLinkLabel();
         this._pnlReportCard = new Idera.SQLcompliance.Application.GUI.Controls.GroupPanel();
         this._grid = new Infragistics.Win.UltraWinGrid.UltraGrid();
         this._dsEvents = new Infragistics.Win.UltraWinDataSource.UltraDataSource(this.components);
         this._headerEvents = new Qios.DevSuite.Components.QPanel();
         this._lblEvents = new System.Windows.Forms.Label();
         @__lblLastArchived = new System.Windows.Forms.Label();
         @__lblLastHeartbeat = new System.Windows.Forms.Label();
         @__lblProcessedEvents = new System.Windows.Forms.Label();
         _lblRecentAlerts = new System.Windows.Forms.Label();
         ((System.ComponentModel.ISupportInitialize)(this._tabControlReportCard)).BeginInit();
         this._tabControlReportCard.SuspendLayout();
         this._tabAlerts.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this._chartAlerts)).BeginInit();
         this._tabPrivUser.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this._chartPrivUser)).BeginInit();
         this._tabFailedLogins.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this._chartFailedLogins)).BeginInit();
            //start sqlcm 5.6 - 5363
            this._tabLogins.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._chartLogins)).BeginInit();
            this._tabLogouts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._chartLogouts)).BeginInit();
            //end sqlcm 5.6 - 5363
            this._tabDDL.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this._chartDDL)).BeginInit();
         this._tabSecurity.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this._chartSecurity)).BeginInit();
         this._tabActivity.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this._chartActivity)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this._pbStatus)).BeginInit();
         this._pnlStatus.SuspendLayout();
         this._headerAuditedActivity.SuspendLayout();
         this._pnlReportCard.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this._grid)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this._dsEvents)).BeginInit();
         this._headerEvents.SuspendLayout();
         this.SuspendLayout();
         // 
         // __lblLastArchived
         // 
         @__lblLastArchived.AutoSize = true;
         @__lblLastArchived.Location = new System.Drawing.Point(6, 128);
         @__lblLastArchived.Name = "__lblLastArchived";
         @__lblLastArchived.Size = new System.Drawing.Size(79, 15);
         @__lblLastArchived.TabIndex = 24;
         @__lblLastArchived.Text = "Last Archived";
         // 
         // __lblLastHeartbeat
         // 
         @__lblLastHeartbeat.AutoSize = true;
         @__lblLastHeartbeat.Location = new System.Drawing.Point(6, 104);
         @__lblLastHeartbeat.Name = "__lblLastHeartbeat";
         @__lblLastHeartbeat.Size = new System.Drawing.Size(87, 15);
         @__lblLastHeartbeat.TabIndex = 22;
         @__lblLastHeartbeat.Text = "Last Heartbeat";
         // 
         // __lblProcessedEvents
         // 
         @__lblProcessedEvents.AutoSize = true;
         @__lblProcessedEvents.Location = new System.Drawing.Point(6, 153);
         @__lblProcessedEvents.Name = "__lblProcessedEvents";
         @__lblProcessedEvents.Size = new System.Drawing.Size(104, 15);
         @__lblProcessedEvents.TabIndex = 20;
         @__lblProcessedEvents.Text = "Processed Events";
         // 
         // _lblRecentAlerts
         // 
         _lblRecentAlerts.Location = new System.Drawing.Point(6, 178);
         _lblRecentAlerts.Name = "_lblRecentAlerts";
         _lblRecentAlerts.Size = new System.Drawing.Size(79, 16);
         _lblRecentAlerts.TabIndex = 56;
         _lblRecentAlerts.Text = "Recent Alerts";
         _lblRecentAlerts.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // _tabControlReportCard
         // 
         this._tabControlReportCard.ActiveTabPage = this._tabAlerts;
         this._tabControlReportCard.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this._tabControlReportCard.Appearance.ShowBorders = false;
         this._tabControlReportCard.ColorScheme.TabButtonActiveBackground1.SetColor("Default", System.Drawing.Color.Transparent, false);
         this._tabControlReportCard.ColorScheme.TabButtonActiveBackground1.SetColor("LunaBlue", System.Drawing.Color.Transparent, false);
         this._tabControlReportCard.ColorScheme.TabButtonActiveBackground1.SetColor("LunaOlive", System.Drawing.Color.Transparent, false);
         this._tabControlReportCard.ColorScheme.TabButtonActiveBackground1.SetColor("LunaSilver", System.Drawing.Color.Transparent, false);
         this._tabControlReportCard.ColorScheme.TabButtonActiveBackground2.SetColor("LunaBlue", System.Drawing.SystemColors.ControlLightLight, false);
         this._tabControlReportCard.ColorScheme.TabButtonActiveBackground2.SetColor("LunaOlive", System.Drawing.SystemColors.ControlLightLight, false);
         this._tabControlReportCard.ColorScheme.TabButtonActiveBackground2.SetColor("LunaSilver", System.Drawing.SystemColors.ControlLightLight, false);
         this._tabControlReportCard.ColorScheme.TabButtonActiveBorder.SetColor("Default", System.Drawing.Color.Transparent, false);
         this._tabControlReportCard.ColorScheme.TabButtonActiveBorder.SetColor("LunaBlue", System.Drawing.Color.Transparent, false);
         this._tabControlReportCard.ColorScheme.TabButtonActiveBorder.SetColor("LunaOlive", System.Drawing.Color.Transparent, false);
         this._tabControlReportCard.ColorScheme.TabButtonActiveBorder.SetColor("LunaSilver", System.Drawing.Color.Transparent, false);
         this._tabControlReportCard.ColorScheme.TabButtonBackground1.SetColor("Default", System.Drawing.SystemColors.ControlLightLight, false);
         this._tabControlReportCard.ColorScheme.TabButtonBackground1.SetColor("LunaBlue", System.Drawing.SystemColors.ControlLightLight, false);
         this._tabControlReportCard.ColorScheme.TabButtonBackground1.SetColor("LunaOlive", System.Drawing.SystemColors.ControlLightLight, false);
         this._tabControlReportCard.ColorScheme.TabButtonBackground1.SetColor("LunaSilver", System.Drawing.SystemColors.ControlLightLight, false);
         this._tabControlReportCard.ColorScheme.TabButtonBackground2.SetColor("LunaBlue", System.Drawing.SystemColors.ControlLightLight, false);
         this._tabControlReportCard.ColorScheme.TabButtonBackground2.SetColor("LunaOlive", System.Drawing.SystemColors.ControlLightLight, false);
         this._tabControlReportCard.ColorScheme.TabButtonBackground2.SetColor("LunaSilver", System.Drawing.SystemColors.ControlLightLight, false);
         this._tabControlReportCard.ColorScheme.TabButtonBorder.SetColor("Default", System.Drawing.SystemColors.ControlLightLight, false);
         this._tabControlReportCard.ColorScheme.TabButtonBorder.SetColor("LunaBlue", System.Drawing.SystemColors.ControlLightLight, false);
         this._tabControlReportCard.ColorScheme.TabButtonBorder.SetColor("LunaOlive", System.Drawing.SystemColors.ControlLightLight, false);
         this._tabControlReportCard.ColorScheme.TabButtonBorder.SetColor("LunaSilver", System.Drawing.SystemColors.ControlLightLight, false);
         this._tabControlReportCard.ColorScheme.TabButtonHotBackground1.SetColor("Default", System.Drawing.Color.Transparent, false);
         this._tabControlReportCard.ColorScheme.TabButtonHotBackground1.SetColor("LunaBlue", System.Drawing.Color.Transparent, false);
         this._tabControlReportCard.ColorScheme.TabButtonHotBackground1.SetColor("LunaOlive", System.Drawing.Color.Transparent, false);
         this._tabControlReportCard.ColorScheme.TabButtonHotBackground1.SetColor("LunaSilver", System.Drawing.Color.Transparent, false);
         this._tabControlReportCard.ColorScheme.TabButtonHotBorder.SetColor("Default", System.Drawing.Color.Transparent, false);
         this._tabControlReportCard.ColorScheme.TabButtonHotBorder.SetColor("LunaBlue", System.Drawing.Color.Transparent, false);
         this._tabControlReportCard.ColorScheme.TabButtonHotBorder.SetColor("LunaOlive", System.Drawing.Color.Transparent, false);
         this._tabControlReportCard.ColorScheme.TabButtonHotBorder.SetColor("LunaSilver", System.Drawing.Color.Transparent, false);
         this._tabControlReportCard.ColorScheme.TabControlBackground1.SetColor("LunaBlue", System.Drawing.SystemColors.ControlLightLight, false);
         this._tabControlReportCard.ColorScheme.TabControlBackground1.SetColor("LunaOlive", System.Drawing.SystemColors.ControlLightLight, false);
         this._tabControlReportCard.ColorScheme.TabControlBackground1.SetColor("LunaSilver", System.Drawing.SystemColors.ControlLightLight, false);
         this._tabControlReportCard.ColorScheme.TabControlBackground2.SetColor("LunaBlue", System.Drawing.SystemColors.ControlLightLight, false);
         this._tabControlReportCard.ColorScheme.TabControlBackground2.SetColor("LunaOlive", System.Drawing.SystemColors.ControlLightLight, false);
         this._tabControlReportCard.ColorScheme.TabControlBackground2.SetColor("LunaSilver", System.Drawing.SystemColors.ControlLightLight, false);
         this._tabControlReportCard.Controls.Add(this._tabPrivUser);
         this._tabControlReportCard.Controls.Add(this._tabAlerts);
         this._tabControlReportCard.Controls.Add(this._tabFailedLogins);
            //start sqlcm 5.6 - 5363
            this._tabControlReportCard.Controls.Add(this._tabLogins);
            this._tabControlReportCard.Controls.Add(this._tabLogouts);
            //end sqlcm 5.6 - 5363
            this._tabControlReportCard.Controls.Add(this._tabDDL);
         this._tabControlReportCard.Controls.Add(this._tabSecurity);
         this._tabControlReportCard.Controls.Add(this._tabActivity);
         this._tabControlReportCard.FocusTabButtons = false;
         this._tabControlReportCard.FontScope = Qios.DevSuite.Components.QFontScope.Local;
         this._tabControlReportCard.LocalFont = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this._tabControlReportCard.Location = new System.Drawing.Point(2, 27);
         this._tabControlReportCard.Name = "_tabControlReportCard";
         this._tabControlReportCard.PersistGuid = new System.Guid("c4933a3d-6abd-45b2-b5cc-5d5f0411618d");
         this._tabControlReportCard.Size = new System.Drawing.Size(523, 222);
         this._tabControlReportCard.TabIndex = 48;
         this._tabControlReportCard.TabStripLeftConfiguration.ButtonAreaMargin = new Qios.DevSuite.Components.QMargin(0, 0, 0, 0);
         this._tabControlReportCard.TabStripLeftConfiguration.ButtonConfiguration.Appearance.BackgroundStyle = Qios.DevSuite.Components.QColorStyle.Solid;
         this._tabControlReportCard.TabStripLeftConfiguration.ButtonConfiguration.Appearance.BorderWidth = 0;
         this._tabControlReportCard.TabStripLeftConfiguration.ButtonConfiguration.Appearance.Shape = this.qShape1;
         this._tabControlReportCard.TabStripLeftConfiguration.ButtonConfiguration.MaximumSize = new System.Drawing.Size(140, 37);
         this._tabControlReportCard.TabStripLeftConfiguration.ButtonConfiguration.MinimumSize = new System.Drawing.Size(140, 37);
         this._tabControlReportCard.TabStripLeftConfiguration.ButtonSpacing = 0;
         this._tabControlReportCard.Text = "qTabControl1";
         this._tabControlReportCard.ToolTipConfiguration.Enabled = false;
         this._tabControlReportCard.ActivePageChanged += new Qios.DevSuite.Components.QTabPageChangeEventHandler(this.ActivePageChanged_tabControlReportCard);
         this._tabControlReportCard.HotPageChanged += new Qios.DevSuite.Components.QTabPageChangeEventHandler(this._tabControlReportCard_HotPageChanged);
         // 
         // _tabAlerts
         // 
         this._tabAlerts.ButtonDockStyle = Qios.DevSuite.Components.QTabButtonDockStyle.Left;
         this._tabAlerts.ButtonOrder = 0;
         this._tabAlerts.Controls.Add(this._chartAlerts);
         this._tabAlerts.Controls.Add(this._lblAlertStatus);
         this._tabAlerts.FontScope = Qios.DevSuite.Components.QFontScope.Local;
         this._tabAlerts.LocalFont = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this._tabAlerts.Location = new System.Drawing.Point(141, 0);
         this._tabAlerts.Name = "_tabAlerts";
         this._tabAlerts.PersistGuid = new System.Guid("d689b762-8ed9-48c7-a1da-c895147b9109");
         this._tabAlerts.Size = new System.Drawing.Size(382, 222);
         this._tabAlerts.Text = "Event Alerts";
         // 
         // _chartAlerts
         // 
         this._chartAlerts.Border = new ChartFX.WinForms.Adornments.SimpleBorder(ChartFX.WinForms.Adornments.SimpleBorderType.Color, System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(125)))), ((int)(((byte)(138))))));
         this._chartAlerts.Dock = System.Windows.Forms.DockStyle.Fill;
         this._chartAlerts.Location = new System.Drawing.Point(0, 38);
         this._chartAlerts.Name = "_chartAlerts";
         this._chartAlerts.Size = new System.Drawing.Size(382, 184);
         this._chartAlerts.TabIndex = 53;
         // 
         // _lblAlertStatus
         // 
         appearance9.BackColor = System.Drawing.SystemColors.Window;
         this._lblAlertStatus.Appearance = appearance9;
         this._lblAlertStatus.AutoSize = true;
         this._lblAlertStatus.Dock = System.Windows.Forms.DockStyle.Top;
         this._lblAlertStatus.Location = new System.Drawing.Point(0, 0);
         this._lblAlertStatus.Name = "_lblAlertStatus";
         this._lblAlertStatus.Padding = new System.Drawing.Size(5, 5);
         this._lblAlertStatus.Size = new System.Drawing.Size(382, 38);
         this._lblAlertStatus.TabIndex = 52;
         this._lblAlertStatus.TabStop = true;
         this._lblAlertStatus.Value = "<span style=\"font-weight:bold;\">Event Alert activity</span> has been within accep" +
             "ted limits <a title=\"Change Threshold\" href=\"http://www.chron.com/\">(30 events p" +
             "er day</a>) for the past week.";
         this._lblAlertStatus.LinkClicked += new Infragistics.Win.FormattedLinkLabel.LinkClickedEventHandler(this.LinkClicked_Threshold);
         // 
         // _tabPrivUser
         // 
         this._tabPrivUser.ButtonDockStyle = Qios.DevSuite.Components.QTabButtonDockStyle.Left;
         this._tabPrivUser.ButtonOrder = 6;
         this._tabPrivUser.Controls.Add(this._chartPrivUser);
         this._tabPrivUser.Controls.Add(this._lblPrivUserStatus);
         this._tabPrivUser.FontScope = Qios.DevSuite.Components.QFontScope.Local;
         this._tabPrivUser.LocalFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
         this._tabPrivUser.Location = new System.Drawing.Point(141, 0);
         this._tabPrivUser.Name = "_tabPrivUser";
         this._tabPrivUser.PersistGuid = new System.Guid("f272330f-16c4-461e-a96f-b544c12b09d4");
         this._tabPrivUser.Size = new System.Drawing.Size(382, 222);
         this._tabPrivUser.Text = "Privileged User";
         // 
         // _chartPrivUser
         // 
         this._chartPrivUser.Border = new ChartFX.WinForms.Adornments.SimpleBorder(ChartFX.WinForms.Adornments.SimpleBorderType.Color, System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(125)))), ((int)(((byte)(138))))));
         this._chartPrivUser.Dock = System.Windows.Forms.DockStyle.Fill;
         this._chartPrivUser.Location = new System.Drawing.Point(0, 38);
         this._chartPrivUser.Name = "_chartPrivUser";
         this._chartPrivUser.Size = new System.Drawing.Size(382, 184);
         this._chartPrivUser.TabIndex = 47;
         // 
         // _lblPrivUserStatus
         // 
         appearance10.BackColor = System.Drawing.SystemColors.Window;
         this._lblPrivUserStatus.Appearance = appearance10;
         this._lblPrivUserStatus.AutoSize = true;
         this._lblPrivUserStatus.Dock = System.Windows.Forms.DockStyle.Top;
         this._lblPrivUserStatus.Location = new System.Drawing.Point(0, 0);
         this._lblPrivUserStatus.Name = "_lblPrivUserStatus";
         this._lblPrivUserStatus.Padding = new System.Drawing.Size(5, 5);
         this._lblPrivUserStatus.Size = new System.Drawing.Size(382, 38);
         this._lblPrivUserStatus.TabIndex = 46;
         this._lblPrivUserStatus.TabStop = true;
         this._lblPrivUserStatus.Value = "<span style=\"font-weight:bold;\">Privileged user activity</span> has been within a" +
             "ccepted limits <a title=\"Change Threshold\" href=\"http://www.chron.com/\">(30 even" +
             "ts per day</a>) for the past week.";
         this._lblPrivUserStatus.LinkClicked += new Infragistics.Win.FormattedLinkLabel.LinkClickedEventHandler(this.LinkClicked_Threshold);
         // 
         // _tabFailedLogins
         // 
         this._tabFailedLogins.ButtonDockStyle = Qios.DevSuite.Components.QTabButtonDockStyle.Left;
         this._tabFailedLogins.ButtonOrder = 2;
         this._tabFailedLogins.Controls.Add(this._chartFailedLogins);
         this._tabFailedLogins.Controls.Add(this._lblFailedLoginStatus);
         this._tabFailedLogins.FontScope = Qios.DevSuite.Components.QFontScope.Local;
         this._tabFailedLogins.LocalFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
         this._tabFailedLogins.Location = new System.Drawing.Point(141, 0);
         this._tabFailedLogins.Name = "_tabFailedLogins";
         this._tabFailedLogins.PersistGuid = new System.Guid("e7204f38-114d-41f1-9293-d76673df8c06");
         this._tabFailedLogins.Size = new System.Drawing.Size(382, 222);
         this._tabFailedLogins.Text = "Failed Logins";
         // 
         // _chartFailedLogins
         // 
         this._chartFailedLogins.Border = new ChartFX.WinForms.Adornments.SimpleBorder(ChartFX.WinForms.Adornments.SimpleBorderType.Color, System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(125)))), ((int)(((byte)(138))))));
         this._chartFailedLogins.Dock = System.Windows.Forms.DockStyle.Fill;
         this._chartFailedLogins.Location = new System.Drawing.Point(0, 38);
         this._chartFailedLogins.Name = "_chartFailedLogins";
         this._chartFailedLogins.Size = new System.Drawing.Size(382, 184);
         this._chartFailedLogins.TabIndex = 53;
         // 
         // _lblFailedLoginStatus
         // 
         appearance11.BackColor = System.Drawing.SystemColors.Window;
         this._lblFailedLoginStatus.Appearance = appearance11;
         this._lblFailedLoginStatus.AutoSize = true;
         this._lblFailedLoginStatus.Dock = System.Windows.Forms.DockStyle.Top;
         this._lblFailedLoginStatus.Location = new System.Drawing.Point(0, 0);
         this._lblFailedLoginStatus.Name = "_lblFailedLoginStatus";
         this._lblFailedLoginStatus.Padding = new System.Drawing.Size(5, 5);
         this._lblFailedLoginStatus.Size = new System.Drawing.Size(382, 38);
         this._lblFailedLoginStatus.TabIndex = 52;
         this._lblFailedLoginStatus.TabStop = true;
         this._lblFailedLoginStatus.Value = "<span style=\"font-weight:bold;\">Failed login activity</span> has been within acce" +
             "pted limits <a title=\"Change Threshold\" href=\"http://www.chron.com/\">(30 events " +
             "per day</a>) for the past week.";
         this._lblFailedLoginStatus.LinkClicked += new Infragistics.Win.FormattedLinkLabel.LinkClickedEventHandler(this.LinkClicked_Threshold);
            // 

            //start sqlcm 5.6 - 5363
            // _tabLogins
            // 
            this._tabLogins.ButtonDockStyle = Qios.DevSuite.Components.QTabButtonDockStyle.Left;
            this._tabLogins.ButtonOrder = 1;
            this._tabLogins.Controls.Add(this._chartLogins);
            this._tabLogins.Controls.Add(this._lblLoginStatus);
            this._tabLogins.FontScope = Qios.DevSuite.Components.QFontScope.Local;
            this._tabLogins.LocalFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this._tabLogins.Location = new System.Drawing.Point(141, 0);
            this._tabLogins.Name = "_tabLogins";
            this._tabLogins.PersistGuid = new System.Guid("e7204f38-114d-41f1-9293-d76673df8c06");
            this._tabLogins.Size = new System.Drawing.Size(382, 222);
            this._tabLogins.Text = "Logins";
            // 
            // _chartLogins
            // 
            this._chartLogins.Border = new ChartFX.WinForms.Adornments.SimpleBorder(ChartFX.WinForms.Adornments.SimpleBorderType.Color, System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(125)))), ((int)(((byte)(138))))));
            this._chartLogins.Dock = System.Windows.Forms.DockStyle.Fill;
            this._chartLogins.Location = new System.Drawing.Point(0, 38);
            this._chartLogins.Name = "_chartLogins";
            this._chartLogins.Size = new System.Drawing.Size(382, 184);
            this._chartLogins.TabIndex = 53;
            // 
            // _lblLoginStatus
            // 
            appearance17.BackColor = System.Drawing.SystemColors.Window;
            this._lblLoginStatus.Appearance = appearance17;
            this._lblLoginStatus.AutoSize = true;
            this._lblLoginStatus.Dock = System.Windows.Forms.DockStyle.Top;
            this._lblLoginStatus.Location = new System.Drawing.Point(0, 0);
            this._lblLoginStatus.Name = "_lblLoginStatus";
            this._lblLoginStatus.Padding = new System.Drawing.Size(5, 5);
            this._lblLoginStatus.Size = new System.Drawing.Size(382, 38);
            this._lblLoginStatus.TabIndex = 52;
            this._lblLoginStatus.TabStop = true;
            this._lblLoginStatus.Value = "<span style=\"font-weight:bold;\">Login activity</span> has been within acce" +
                "pted limits <a title=\"Change Threshold\" href=\"http://www.chron.com/\">(30 events " +
                "per day</a>) for the past week.";
            this._lblLoginStatus.LinkClicked += new Infragistics.Win.FormattedLinkLabel.LinkClickedEventHandler(this.LinkClicked_Threshold);
            // 

            // _tabLogins
            // 
            this._tabLogouts.ButtonDockStyle = Qios.DevSuite.Components.QTabButtonDockStyle.Left;
            this._tabLogouts.ButtonOrder = 2;
            this._tabLogouts.Controls.Add(this._chartLogouts);
            this._tabLogouts.Controls.Add(this._lblLogoutStatus);
            this._tabLogouts.FontScope = Qios.DevSuite.Components.QFontScope.Local;
            this._tabLogouts.LocalFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this._tabLogouts.Location = new System.Drawing.Point(141, 0);
            this._tabLogouts.Name = "_tabLogouts";
            this._tabLogouts.PersistGuid = new System.Guid("e7204f38-114d-41f1-9293-d76673df8c06");
            this._tabLogouts.Size = new System.Drawing.Size(382, 222);
            this._tabLogouts.Text = "Logouts";
            // 
            // _chartLogouts
            // 
            this._chartLogouts.Border = new ChartFX.WinForms.Adornments.SimpleBorder(ChartFX.WinForms.Adornments.SimpleBorderType.Color, System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(125)))), ((int)(((byte)(138))))));
            this._chartLogouts.Dock = System.Windows.Forms.DockStyle.Fill;
            this._chartLogouts.Location = new System.Drawing.Point(0, 38);
            this._chartLogouts.Name = "_chartLogouts";
            this._chartLogouts.Size = new System.Drawing.Size(382, 184);
            this._chartLogouts.TabIndex = 53;
            // 
            // _lblLogoutStatus
            // 
            appearance18.BackColor = System.Drawing.SystemColors.Window;
            this._lblLogoutStatus.Appearance = appearance18;
            this._lblLogoutStatus.AutoSize = true;
            this._lblLogoutStatus.Dock = System.Windows.Forms.DockStyle.Top;
            this._lblLogoutStatus.Location = new System.Drawing.Point(0, 0);
            this._lblLogoutStatus.Name = "_lblLogoutStatus";
            this._lblLogoutStatus.Padding = new System.Drawing.Size(5, 5);
            this._lblLogoutStatus.Size = new System.Drawing.Size(382, 38);
            this._lblLogoutStatus.TabIndex = 52;
            this._lblLogoutStatus.TabStop = true;
            this._lblLogoutStatus.Value = "<span style=\"font-weight:bold;\">Logout activity</span> has been within acce" +
                "pted limits <a title=\"Change Threshold\" href=\"http://www.chron.com/\">(30 events " +
                "per day</a>) for the past week.";
            this._lblLogoutStatus.LinkClicked += new Infragistics.Win.FormattedLinkLabel.LinkClickedEventHandler(this.LinkClicked_Threshold);
            //end sqlcm 5.6 -5363

            // _tabDDL
            // 
            this._tabDDL.ButtonDockStyle = Qios.DevSuite.Components.QTabButtonDockStyle.Left;
         this._tabDDL.ButtonOrder = 5;
         this._tabDDL.Controls.Add(this._chartDDL);
         this._tabDDL.Controls.Add(this._lblDDLStatus);
         this._tabDDL.FontScope = Qios.DevSuite.Components.QFontScope.Local;
         this._tabDDL.LocalFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
         this._tabDDL.Location = new System.Drawing.Point(141, 0);
         this._tabDDL.Name = "_tabDDL";
         this._tabDDL.PersistGuid = new System.Guid("793234f1-3be2-4197-afcb-9d3da36df9c5");
         this._tabDDL.Size = new System.Drawing.Size(382, 222);
         this._tabDDL.Text = "DDL";
         // 
         // _chartDDL
         // 
         this._chartDDL.Border = new ChartFX.WinForms.Adornments.SimpleBorder(ChartFX.WinForms.Adornments.SimpleBorderType.Color, System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(125)))), ((int)(((byte)(138))))));
         this._chartDDL.Dock = System.Windows.Forms.DockStyle.Fill;
         this._chartDDL.Location = new System.Drawing.Point(0, 38);
         this._chartDDL.Name = "_chartDDL";
         this._chartDDL.Size = new System.Drawing.Size(382, 184);
         this._chartDDL.TabIndex = 53;
         // 
         // _lblDDLStatus
         // 
         appearance12.BackColor = System.Drawing.SystemColors.Window;
         this._lblDDLStatus.Appearance = appearance12;
         this._lblDDLStatus.AutoSize = true;
         this._lblDDLStatus.Dock = System.Windows.Forms.DockStyle.Top;
         this._lblDDLStatus.Location = new System.Drawing.Point(0, 0);
         this._lblDDLStatus.Name = "_lblDDLStatus";
         this._lblDDLStatus.Padding = new System.Drawing.Size(5, 5);
         this._lblDDLStatus.Size = new System.Drawing.Size(382, 38);
         this._lblDDLStatus.TabIndex = 52;
         this._lblDDLStatus.TabStop = true;
         this._lblDDLStatus.Value = "<span style=\"font-weight:bold;\">DDL activity</span> has been within accepted limi" +
             "ts <a title=\"Change Threshold\" href=\"http://www.chron.com/\">(30 events per day</" +
             "a>) for the past week.";
         this._lblDDLStatus.LinkClicked += new Infragistics.Win.FormattedLinkLabel.LinkClickedEventHandler(this.LinkClicked_Threshold);
         // 
         // _tabSecurity
         // 
         this._tabSecurity.ButtonDockStyle = Qios.DevSuite.Components.QTabButtonDockStyle.Left;
         this._tabSecurity.ButtonOrder = 4;
         this._tabSecurity.Controls.Add(this._chartSecurity);
         this._tabSecurity.Controls.Add(this._lblSecurityStatus);
         this._tabSecurity.FontScope = Qios.DevSuite.Components.QFontScope.Local;
         this._tabSecurity.LocalFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
         this._tabSecurity.Location = new System.Drawing.Point(141, 0);
         this._tabSecurity.Name = "_tabSecurity";
         this._tabSecurity.PersistGuid = new System.Guid("e49055a1-bcdb-43aa-8828-32f554b5feaa");
         this._tabSecurity.Size = new System.Drawing.Size(382, 222);
         this._tabSecurity.Text = "Security";
         // 
         // _chartSecurity
         // 
         this._chartSecurity.Border = new ChartFX.WinForms.Adornments.SimpleBorder(ChartFX.WinForms.Adornments.SimpleBorderType.Color, System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(125)))), ((int)(((byte)(138))))));
         this._chartSecurity.Dock = System.Windows.Forms.DockStyle.Fill;
         this._chartSecurity.Location = new System.Drawing.Point(0, 38);
         this._chartSecurity.Name = "_chartSecurity";
         this._chartSecurity.Size = new System.Drawing.Size(382, 184);
         this._chartSecurity.TabIndex = 53;
         // 
         // _lblSecurityStatus
         // 
         appearance13.BackColor = System.Drawing.SystemColors.Window;
         this._lblSecurityStatus.Appearance = appearance13;
         this._lblSecurityStatus.AutoSize = true;
         this._lblSecurityStatus.Dock = System.Windows.Forms.DockStyle.Top;
         this._lblSecurityStatus.Location = new System.Drawing.Point(0, 0);
         this._lblSecurityStatus.Name = "_lblSecurityStatus";
         this._lblSecurityStatus.Padding = new System.Drawing.Size(5, 5);
         this._lblSecurityStatus.Size = new System.Drawing.Size(382, 38);
         this._lblSecurityStatus.TabIndex = 52;
         this._lblSecurityStatus.TabStop = true;
         this._lblSecurityStatus.Value = "<span style=\"font-weight:bold;\">Security activity</span> has been within accepted" +
             " limits <a title=\"Change Threshold\" href=\"http://www.chron.com/\">(30 events per " +
             "day</a>) for the past week.";
         this._lblSecurityStatus.LinkClicked += new Infragistics.Win.FormattedLinkLabel.LinkClickedEventHandler(this.LinkClicked_Threshold);
         // 
         // _tabActivity
         // 
         this._tabActivity.ButtonDockStyle = Qios.DevSuite.Components.QTabButtonDockStyle.Left;
         this._tabActivity.ButtonOrder = 7;
         this._tabActivity.Controls.Add(this._chartActivity);
         this._tabActivity.Controls.Add(this._lblExcessiveActivityStatus);
         this._tabActivity.FontScope = Qios.DevSuite.Components.QFontScope.Local;
         this._tabActivity.LocalFont = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this._tabActivity.Location = new System.Drawing.Point(141, 0);
         this._tabActivity.Name = "_tabActivity";
         this._tabActivity.PersistGuid = new System.Guid("b4345277-6c20-4aac-bae9-7b2bc7589458");
         this._tabActivity.Size = new System.Drawing.Size(382, 222);
         this._tabActivity.Text = "Overall Activity";
         // 
         // _chartActivity
         // 
         this._chartActivity.Border = new ChartFX.WinForms.Adornments.SimpleBorder(ChartFX.WinForms.Adornments.SimpleBorderType.Color, System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(125)))), ((int)(((byte)(138))))));
         this._chartActivity.Dock = System.Windows.Forms.DockStyle.Fill;
         this._chartActivity.Location = new System.Drawing.Point(0, 38);
         this._chartActivity.Name = "_chartActivity";
         this._chartActivity.Size = new System.Drawing.Size(382, 184);
         this._chartActivity.TabIndex = 53;
         // 
         // _lblExcessiveActivityStatus
         // 
         appearance14.BackColor = System.Drawing.SystemColors.Window;
         this._lblExcessiveActivityStatus.Appearance = appearance14;
         this._lblExcessiveActivityStatus.AutoSize = true;
         this._lblExcessiveActivityStatus.Dock = System.Windows.Forms.DockStyle.Top;
         this._lblExcessiveActivityStatus.Location = new System.Drawing.Point(0, 0);
         this._lblExcessiveActivityStatus.Name = "_lblExcessiveActivityStatus";
         this._lblExcessiveActivityStatus.Padding = new System.Drawing.Size(5, 5);
         this._lblExcessiveActivityStatus.Size = new System.Drawing.Size(382, 38);
         this._lblExcessiveActivityStatus.TabIndex = 52;
         this._lblExcessiveActivityStatus.TabStop = true;
         this._lblExcessiveActivityStatus.Value = "<span style=\"font-weight:bold;\">Overall activity</span> has been within accepted " +
             "limits <a title=\"Change Threshold\" href=\"http://www.chron.com/\">(30 events per d" +
             "ay</a>) for the past week.";
         this._lblExcessiveActivityStatus.LinkClicked += new Infragistics.Win.FormattedLinkLabel.LinkClickedEventHandler(this.LinkClicked_Threshold);
         // 
         // qShape1
         // 
         this.qShape1.ClonedBaseShapeType = Qios.DevSuite.Components.QBaseShapeType.SquareTab;
         // 
         // _lblReportCard
         // 
         this._lblReportCard.BackColor = System.Drawing.Color.Transparent;
         this._lblReportCard.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this._lblReportCard.Location = new System.Drawing.Point(1, 0);
         this._lblReportCard.Name = "_lblReportCard";
         this._lblReportCard.Size = new System.Drawing.Size(279, 27);
         this._lblReportCard.TabIndex = 0;
         this._lblReportCard.Text = "Server Activity Report Card";
         this._lblReportCard.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // _lblLastHeartbeat
         // 
         this._lblLastHeartbeat.Location = new System.Drawing.Point(95, 104);
         this._lblLastHeartbeat.Name = "_lblLastHeartbeat";
         this._lblLastHeartbeat.Size = new System.Drawing.Size(123, 15);
         this._lblLastHeartbeat.TabIndex = 23;
         this._lblLastHeartbeat.Text = "01/04/2007 12:15:46 AM";
         this._lblLastHeartbeat.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
         // 
         // _lblLastArchived
         // 
         this._lblLastArchived.Location = new System.Drawing.Point(98, 128);
         this._lblLastArchived.Name = "_lblLastArchived";
         this._lblLastArchived.Size = new System.Drawing.Size(120, 15);
         this._lblLastArchived.TabIndex = 25;
         this._lblLastArchived.Text = "08/08/2006";
         this._lblLastArchived.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
         // 
         // _lblProcessedEvents
         // 
         this._lblProcessedEvents.Location = new System.Drawing.Point(125, 153);
         this._lblProcessedEvents.Name = "_lblProcessedEvents";
         this._lblProcessedEvents.Size = new System.Drawing.Size(93, 15);
         this._lblProcessedEvents.TabIndex = 21;
         this._lblProcessedEvents.Text = "85,865";
         this._lblProcessedEvents.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
         // 
         // _lblServerStatus
         // 
         this._lblServerStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this._lblServerStatus.BackColor = System.Drawing.Color.Transparent;
         this._lblServerStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this._lblServerStatus.Location = new System.Drawing.Point(1, 0);
         this._lblServerStatus.Name = "_lblServerStatus";
         this._lblServerStatus.Size = new System.Drawing.Size(235, 27);
         this._lblServerStatus.TabIndex = 0;
         this._lblServerStatus.Text = "Server Status";
         this._lblServerStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // _pbStatus
         // 
         this._pbStatus.BackColor = System.Drawing.Color.Transparent;
         this._pbStatus.Image = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.StatusGood_48;
         this._pbStatus.Location = new System.Drawing.Point(6, 33);
         this._pbStatus.Name = "_pbStatus";
         this._pbStatus.Size = new System.Drawing.Size(48, 48);
         this._pbStatus.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
         this._pbStatus.TabIndex = 7;
         this._pbStatus.TabStop = false;
         // 
         // _flblServerStatus
         // 
         appearance15.BackColor = System.Drawing.Color.Transparent;
         appearance15.BackColor2 = System.Drawing.Color.Transparent;
         appearance15.BorderColor = System.Drawing.Color.Transparent;
         appearance15.BorderColor2 = System.Drawing.Color.Transparent;
         appearance15.TextVAlignAsString = "Middle";
         this._flblServerStatus.Appearance = appearance15;
         this._flblServerStatus.Location = new System.Drawing.Point(60, 30);
         this._flblServerStatus.Name = "_flblServerStatus";
         this._flblServerStatus.Size = new System.Drawing.Size(172, 55);
         this._flblServerStatus.TabIndex = 8;
         this._flblServerStatus.TabStop = true;
         this._flblServerStatus.Value = "All features are functioning normally";
         this._flblServerStatus.LinkClicked += new Infragistics.Win.FormattedLinkLabel.LinkClickedEventHandler(this.LinkClicked_flblServerStatus);
         // 
         // _pnlStatus
         // 
         this._pnlStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                     | System.Windows.Forms.AnchorStyles.Left)));
         this._pnlStatus.BackColor = System.Drawing.Color.Transparent;
         this._pnlStatus.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(144)))), ((int)(((byte)(144)))), ((int)(((byte)(144)))));
         this._pnlStatus.Controls.Add(_lblRecentAlerts);
         this._pnlStatus.Controls.Add(this._lblRecentAlertCount);
         this._pnlStatus.Controls.Add(this._headerAuditedActivity);
         this._pnlStatus.Controls.Add(this._lblServerStatus);
         this._pnlStatus.Controls.Add(this._pbStatus);
         this._pnlStatus.Controls.Add(@__lblLastArchived);
         this._pnlStatus.Controls.Add(this._lblLastHeartbeat);
         this._pnlStatus.Controls.Add(this._flblServerStatus);
         this._pnlStatus.Controls.Add(this._lblLastArchived);
         this._pnlStatus.Controls.Add(@__lblProcessedEvents);
         this._pnlStatus.Controls.Add(@__lblLastHeartbeat);
         this._pnlStatus.Controls.Add(this._lblProcessedEvents);
         this._pnlStatus.Controls.Add(this._flblAuditSettings);
         this._pnlStatus.GroupBoxBackColor = System.Drawing.Color.White;
         this._pnlStatus.HeaderBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(219)))), ((int)(((byte)(224)))));
         this._pnlStatus.HeaderBackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(187)))), ((int)(((byte)(197)))));
         this._pnlStatus.Location = new System.Drawing.Point(3, 3);
         this._pnlStatus.Name = "_pnlStatus";
         this._pnlStatus.RoundedBottoms = false;
         this._pnlStatus.Size = new System.Drawing.Size(238, 451);
         this._pnlStatus.TabIndex = 17;
         // 
         // _lblRecentAlertCount
         // 
         this._lblRecentAlertCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this._lblRecentAlertCount.Location = new System.Drawing.Point(128, 178);
         this._lblRecentAlertCount.Name = "_lblRecentAlertCount";
         this._lblRecentAlertCount.Size = new System.Drawing.Size(90, 16);
         this._lblRecentAlertCount.TabIndex = 55;
         this._lblRecentAlertCount.Text = "2";
         this._lblRecentAlertCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
         // 
         // _headerAuditedActivity
         // 
         this._headerAuditedActivity.Appearance.GradientAngle = 90;
         this._headerAuditedActivity.Appearance.ShowBorderLeft = false;
         this._headerAuditedActivity.Appearance.ShowBorderRight = false;
         this._headerAuditedActivity.ColorScheme.PanelBackground1.SetColor("Default", System.Drawing.SystemColors.ActiveCaption, false);
         this._headerAuditedActivity.ColorScheme.PanelBackground1.SetColor("LunaBlue", System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254))))), false);
         this._headerAuditedActivity.ColorScheme.PanelBackground1.SetColor("LunaOlive", System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(247)))), ((int)(((byte)(222))))), false);
         this._headerAuditedActivity.ColorScheme.PanelBackground1.SetColor("LunaSilver", System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(250))))), false);
         this._headerAuditedActivity.ColorScheme.PanelBackground2.SetColor("Default", System.Drawing.SystemColors.ActiveCaption, false);
         this._headerAuditedActivity.ColorScheme.PanelBackground2.SetColor("LunaBlue", System.Drawing.Color.FromArgb(((int)(((byte)(129)))), ((int)(((byte)(169)))), ((int)(((byte)(226))))), false);
         this._headerAuditedActivity.ColorScheme.PanelBackground2.SetColor("LunaOlive", System.Drawing.Color.FromArgb(((int)(((byte)(183)))), ((int)(((byte)(198)))), ((int)(((byte)(145))))), false);
         this._headerAuditedActivity.ColorScheme.PanelBackground2.SetColor("LunaSilver", System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(151)))), ((int)(((byte)(181))))), false);
         this._headerAuditedActivity.ColorScheme.PanelBorder.SetColor("LunaBlue", System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(45)))), ((int)(((byte)(150))))), false);
         this._headerAuditedActivity.ColorScheme.PanelBorder.SetColor("LunaOlive", System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(128)))), ((int)(((byte)(88))))), false);
         this._headerAuditedActivity.ColorScheme.PanelBorder.SetColor("LunaSilver", System.Drawing.Color.FromArgb(((int)(((byte)(124)))), ((int)(((byte)(124)))), ((int)(((byte)(148))))), false);
         this._headerAuditedActivity.Controls.Add(this._lblAuditedActivity);
         this._headerAuditedActivity.FontScope = Qios.DevSuite.Components.QFontScope.Local;
         this._headerAuditedActivity.LocalFont = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this._headerAuditedActivity.Location = new System.Drawing.Point(1, 207);
         this._headerAuditedActivity.MinimumClientSize = new System.Drawing.Size(10, 10);
         this._headerAuditedActivity.Name = "_headerAuditedActivity";
         this._headerAuditedActivity.Size = new System.Drawing.Size(235, 27);
         this._headerAuditedActivity.TabIndex = 10;
         this._headerAuditedActivity.Text = "qPanel4";
         // 
         // _lblAuditedActivity
         // 
         this._lblAuditedActivity.BackColor = System.Drawing.Color.Transparent;
         this._lblAuditedActivity.Dock = System.Windows.Forms.DockStyle.Fill;
         this._lblAuditedActivity.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this._lblAuditedActivity.Location = new System.Drawing.Point(0, 0);
         this._lblAuditedActivity.Name = "_lblAuditedActivity";
         this._lblAuditedActivity.Size = new System.Drawing.Size(235, 25);
         this._lblAuditedActivity.TabIndex = 0;
         this._lblAuditedActivity.Text = "Audit Configuration";
         this._lblAuditedActivity.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // _flblAuditSettings
         // 
         this._flblAuditSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                     | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         appearance16.BorderColor = System.Drawing.Color.Transparent;
         appearance16.BorderColor2 = System.Drawing.Color.Transparent;
         this._flblAuditSettings.Appearance = appearance16;
         this._flblAuditSettings.Location = new System.Drawing.Point(9, 250);
         this._flblAuditSettings.Name = "_flblAuditSettings";
         this._flblAuditSettings.Size = new System.Drawing.Size(209, 191);
         this._flblAuditSettings.TabIndex = 57;
         this._flblAuditSettings.TabStop = true;
         this._flblAuditSettings.Value = "<span style=\"font-weight:bold_x003B_\">Event Filters (4)</span> - Event Type, Even" +
             "t Category, Database Name, Object Name, Application Name, Is Privileged User";
         // 
         // _pnlReportCard
         // 
         this._pnlReportCard.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                     | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this._pnlReportCard.AutoScroll = true;
         this._pnlReportCard.AutoScrollMinSize = new System.Drawing.Size(481, 359);
         this._pnlReportCard.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(144)))), ((int)(((byte)(144)))), ((int)(((byte)(144)))));
         this._pnlReportCard.Controls.Add(this._grid);
         this._pnlReportCard.Controls.Add(this._headerEvents);
         this._pnlReportCard.Controls.Add(this._lblReportCard);
         this._pnlReportCard.Controls.Add(this._tabControlReportCard);
         this._pnlReportCard.GroupBoxBackColor = System.Drawing.Color.White;
         this._pnlReportCard.HeaderBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(219)))), ((int)(((byte)(224)))));
         this._pnlReportCard.HeaderBackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(187)))), ((int)(((byte)(197)))));
         this._pnlReportCard.Location = new System.Drawing.Point(244, 3);
         this._pnlReportCard.Name = "_pnlReportCard";
         this._pnlReportCard.RoundedBottoms = false;
         this._pnlReportCard.Size = new System.Drawing.Size(528, 451);
         this._pnlReportCard.TabIndex = 18;
         // 
         // _grid
         // 
         this._grid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                     | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this._grid.DataSource = this._dsEvents;
         ultraGridColumn22.Format = "G";
         ultraGridColumn22.Header.Caption = "Time";
         ultraGridColumn22.Header.VisiblePosition = 2;
         ultraGridColumn22.Width = 127;
         ultraGridColumn23.Header.Caption = "Event";
         ultraGridColumn23.Header.VisiblePosition = 1;
         ultraGridColumn23.Width = 112;
         ultraGridColumn24.Header.Caption = "Spid";
         ultraGridColumn24.Header.VisiblePosition = 7;
         ultraGridColumn24.Hidden = true;
         ultraGridColumn25.Header.Caption = "Application";
         ultraGridColumn25.Header.VisiblePosition = 8;
         ultraGridColumn25.Hidden = true;
         ultraGridColumn26.Header.Caption = "Host";
         ultraGridColumn26.Header.VisiblePosition = 9;
         ultraGridColumn26.Hidden = true;
         ultraGridColumn27.Header.Caption = "Server";
         ultraGridColumn27.Header.VisiblePosition = 10;
         ultraGridColumn27.Hidden = true;
         ultraGridColumn28.Header.Caption = "Login";
         ultraGridColumn28.Header.VisiblePosition = 3;
         ultraGridColumn28.Hidden = true;
         ultraGridColumn28.Width = 117;
         ultraGridColumn29.Header.Caption = "Success";
         ultraGridColumn29.Header.VisiblePosition = 11;
         ultraGridColumn29.Hidden = true;
         ultraGridColumn30.Header.Caption = "Database";
         ultraGridColumn30.Header.VisiblePosition = 4;
         ultraGridColumn30.Hidden = true;
         ultraGridColumn30.Width = 105;
         ultraGridColumn31.Header.Caption = "Database User";
         ultraGridColumn31.Header.VisiblePosition = 12;
         ultraGridColumn31.Hidden = true;
         ultraGridColumn32.Header.Caption = "Object";
         ultraGridColumn32.Header.VisiblePosition = 13;
         ultraGridColumn32.Hidden = true;
         ultraGridColumn33.Header.Caption = "Target Login";
         ultraGridColumn33.Header.VisiblePosition = 14;
         ultraGridColumn33.Hidden = true;
         ultraGridColumn34.Header.Caption = "Target User";
         ultraGridColumn34.Header.VisiblePosition = 15;
         ultraGridColumn34.Hidden = true;
         ultraGridColumn35.Header.Caption = "Role";
         ultraGridColumn35.Header.VisiblePosition = 16;
         ultraGridColumn35.Hidden = true;
         ultraGridColumn36.Header.Caption = "Owner";
         ultraGridColumn36.Header.VisiblePosition = 17;
         ultraGridColumn36.Hidden = true;
         ultraGridColumn37.Header.Caption = "Target Object";
         ultraGridColumn37.Header.VisiblePosition = 5;
         ultraGridColumn37.Hidden = true;
         ultraGridColumn37.Width = 121;
         ultraGridColumn38.Header.Caption = "Details";
         ultraGridColumn38.Header.VisiblePosition = 6;
         ultraGridColumn38.Width = 169;
         ultraGridColumn39.Header.Caption = "Category";
         ultraGridColumn39.Header.VisiblePosition = 0;
         ultraGridColumn39.Width = 117;
         ultraGridColumn40.Header.Caption = "Privileged User";
         ultraGridColumn40.Header.VisiblePosition = 18;
         ultraGridColumn40.Hidden = true;
         ultraGridColumn41.Header.Caption = "Session Login";
         ultraGridColumn41.Header.VisiblePosition = 19;
         ultraGridColumn41.Hidden = true;
         ultraGridColumn42.Header.VisiblePosition = 20;
         ultraGridColumn42.Hidden = true;
         ultraGridBand2.Columns.AddRange(new object[] {
            ultraGridColumn22,
            ultraGridColumn23,
            ultraGridColumn24,
            ultraGridColumn25,
            ultraGridColumn26,
            ultraGridColumn27,
            ultraGridColumn28,
            ultraGridColumn29,
            ultraGridColumn30,
            ultraGridColumn31,
            ultraGridColumn32,
            ultraGridColumn33,
            ultraGridColumn34,
            ultraGridColumn35,
            ultraGridColumn36,
            ultraGridColumn37,
            ultraGridColumn38,
            ultraGridColumn39,
            ultraGridColumn40,
            ultraGridColumn41,
            ultraGridColumn42});
         this._grid.DisplayLayout.BandsSerializer.Add(ultraGridBand2);
         this._grid.Location = new System.Drawing.Point(1, 276);
         this._grid.Name = "_grid";
         this._grid.Size = new System.Drawing.Size(525, 173);
         this._grid.TabIndex = 50;
         this._grid.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyDown_grid);
         this._grid.DoubleClickRow += new Infragistics.Win.UltraWinGrid.DoubleClickRowEventHandler(this.DoubleClickRow_grid);
         // 
         // _dsEvents
         // 
         ultraDataColumn22.DataType = typeof(System.DateTime);
         ultraDataColumn24.DataType = typeof(int);
         ultraDataColumn29.DataType = typeof(bool);
         ultraDataColumn40.DataType = typeof(bool);
         this._dsEvents.Band.Columns.AddRange(new object[] {
            ultraDataColumn22,
            ultraDataColumn23,
            ultraDataColumn24,
            ultraDataColumn25,
            ultraDataColumn26,
            ultraDataColumn27,
            ultraDataColumn28,
            ultraDataColumn29,
            ultraDataColumn30,
            ultraDataColumn31,
            ultraDataColumn32,
            ultraDataColumn33,
            ultraDataColumn34,
            ultraDataColumn35,
            ultraDataColumn36,
            ultraDataColumn37,
            ultraDataColumn38,
            ultraDataColumn39,
            ultraDataColumn40,
            ultraDataColumn41,
            ultraDataColumn42});
         this._dsEvents.Band.Key = "Events";
         // 
         // _headerEvents
         // 
         this._headerEvents.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this._headerEvents.Appearance.GradientAngle = 90;
         this._headerEvents.Appearance.ShowBorderLeft = false;
         this._headerEvents.Appearance.ShowBorderRight = false;
         this._headerEvents.ColorScheme.PanelBackground1.SetColor("Default", System.Drawing.SystemColors.ActiveCaption, false);
         this._headerEvents.ColorScheme.PanelBackground1.SetColor("LunaBlue", System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254))))), false);
         this._headerEvents.ColorScheme.PanelBackground1.SetColor("LunaOlive", System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(247)))), ((int)(((byte)(222))))), false);
         this._headerEvents.ColorScheme.PanelBackground1.SetColor("LunaSilver", System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(250))))), false);
         this._headerEvents.ColorScheme.PanelBackground2.SetColor("Default", System.Drawing.SystemColors.ActiveCaption, false);
         this._headerEvents.ColorScheme.PanelBackground2.SetColor("LunaBlue", System.Drawing.Color.FromArgb(((int)(((byte)(129)))), ((int)(((byte)(169)))), ((int)(((byte)(226))))), false);
         this._headerEvents.ColorScheme.PanelBackground2.SetColor("LunaOlive", System.Drawing.Color.FromArgb(((int)(((byte)(183)))), ((int)(((byte)(198)))), ((int)(((byte)(145))))), false);
         this._headerEvents.ColorScheme.PanelBackground2.SetColor("LunaSilver", System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(151)))), ((int)(((byte)(181))))), false);
         this._headerEvents.ColorScheme.PanelBorder.SetColor("LunaBlue", System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(45)))), ((int)(((byte)(150))))), false);
         this._headerEvents.ColorScheme.PanelBorder.SetColor("LunaOlive", System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(128)))), ((int)(((byte)(88))))), false);
         this._headerEvents.ColorScheme.PanelBorder.SetColor("LunaSilver", System.Drawing.Color.FromArgb(((int)(((byte)(124)))), ((int)(((byte)(124)))), ((int)(((byte)(148))))), false);
         this._headerEvents.Controls.Add(this._lblEvents);
         this._headerEvents.FontScope = Qios.DevSuite.Components.QFontScope.Local;
         this._headerEvents.LocalFont = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this._headerEvents.Location = new System.Drawing.Point(1, 249);
         this._headerEvents.MinimumClientSize = new System.Drawing.Size(10, 10);
         this._headerEvents.Name = "_headerEvents";
         this._headerEvents.Size = new System.Drawing.Size(525, 27);
         this._headerEvents.TabIndex = 49;
         this._headerEvents.Text = "qPanel4";
         // 
         // _lblEvents
         // 
         this._lblEvents.BackColor = System.Drawing.Color.Transparent;
         this._lblEvents.Dock = System.Windows.Forms.DockStyle.Fill;
         this._lblEvents.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this._lblEvents.Location = new System.Drawing.Point(0, 0);
         this._lblEvents.Name = "_lblEvents";
         this._lblEvents.Size = new System.Drawing.Size(525, 25);
         this._lblEvents.TabIndex = 0;
         this._lblEvents.Text = "Recent Audit Events";
         this._lblEvents.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // ServerSummary
         // 
         this.Controls.Add(this._pnlReportCard);
         this.Controls.Add(this._pnlStatus);
         this.Name = "ServerSummary";
         this.Size = new System.Drawing.Size(775, 456);
         this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.ServerSummary_HelpRequested);
         ((System.ComponentModel.ISupportInitialize)(this._tabControlReportCard)).EndInit();
         this._tabControlReportCard.ResumeLayout(false);
         this._tabAlerts.ResumeLayout(false);
         this._tabAlerts.PerformLayout();
         ((System.ComponentModel.ISupportInitialize)(this._chartAlerts)).EndInit();
         this._tabPrivUser.ResumeLayout(false);
         this._tabPrivUser.PerformLayout();
         ((System.ComponentModel.ISupportInitialize)(this._chartPrivUser)).EndInit();
         this._tabFailedLogins.ResumeLayout(false);
         this._tabFailedLogins.PerformLayout();
         ((System.ComponentModel.ISupportInitialize)(this._chartFailedLogins)).EndInit();
            //start sqlcm 5.6 - 5363
            this._tabLogins.ResumeLayout(false);
            this._tabLogins.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._chartLogins)).EndInit();
            this._tabLogouts.ResumeLayout(false);
            this._tabLogouts.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._chartLogouts)).EndInit();
            //end sqlcm 5.6 - 5363
            this._tabDDL.ResumeLayout(false);
         this._tabDDL.PerformLayout();
         ((System.ComponentModel.ISupportInitialize)(this._chartDDL)).EndInit();
         this._tabSecurity.ResumeLayout(false);
         this._tabSecurity.PerformLayout();
         ((System.ComponentModel.ISupportInitialize)(this._chartSecurity)).EndInit();
         this._tabActivity.ResumeLayout(false);
         this._tabActivity.PerformLayout();
         ((System.ComponentModel.ISupportInitialize)(this._chartActivity)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this._pbStatus)).EndInit();
         this._pnlStatus.ResumeLayout(false);
         this._pnlStatus.PerformLayout();
         this._headerAuditedActivity.ResumeLayout(false);
         this._pnlReportCard.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this._grid)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this._dsEvents)).EndInit();
         this._headerEvents.ResumeLayout(false);
         this.ResumeLayout(false);

      }

      #endregion

      private Qios.DevSuite.Components.QTabControl _tabControlReportCard;
      private Qios.DevSuite.Components.QTabPage _tabPrivUser;
      private Infragistics.Win.FormattedLinkLabel.UltraFormattedLinkLabel _lblPrivUserStatus;
      private Qios.DevSuite.Components.QTabPage _tabAlerts;
      private Qios.DevSuite.Components.QTabPage _tabFailedLogins;
        //start sqlcm 5.6-5363
        private Qios.DevSuite.Components.QTabPage _tabLogins;
        private Qios.DevSuite.Components.QTabPage _tabLogouts;
        //end sqlcm 5.6 - 5363
        private Qios.DevSuite.Components.QTabPage _tabDDL;
      private Qios.DevSuite.Components.QTabPage _tabSecurity;
      private Qios.DevSuite.Components.QTabPage _tabActivity;
      private System.Windows.Forms.Label _lblReportCard;
      private System.Windows.Forms.Label _lblLastArchived;
      private System.Windows.Forms.Label _lblLastHeartbeat;
      private System.Windows.Forms.Label _lblProcessedEvents;
      private System.Windows.Forms.Label _lblServerStatus;
      private System.Windows.Forms.PictureBox _pbStatus;
      private Infragistics.Win.FormattedLinkLabel.UltraFormattedLinkLabel _flblServerStatus;
      private Infragistics.Win.FormattedLinkLabel.UltraFormattedLinkLabel _lblAlertStatus;
      private Infragistics.Win.FormattedLinkLabel.UltraFormattedLinkLabel _lblFailedLoginStatus;
        //start sqlcm 5.6 - 5363
        private Infragistics.Win.FormattedLinkLabel.UltraFormattedLinkLabel _lblLoginStatus;
        private Infragistics.Win.FormattedLinkLabel.UltraFormattedLinkLabel _lblLogoutStatus;
        //end sqlcm 5.6 - 5363
        private Infragistics.Win.FormattedLinkLabel.UltraFormattedLinkLabel _lblDDLStatus;
      private Infragistics.Win.FormattedLinkLabel.UltraFormattedLinkLabel _lblSecurityStatus;
      private Infragistics.Win.FormattedLinkLabel.UltraFormattedLinkLabel _lblExcessiveActivityStatus;
      private ChartFX.WinForms.Chart _chartAlerts;
        //start sqlcm 5.6- 5363
        private ChartFX.WinForms.Chart _chartLogins;
        private ChartFX.WinForms.Chart _chartLogouts;
        //end sqlcm 5.6 - 5363

        private ChartFX.WinForms.Chart _chartFailedLogins;
      private ChartFX.WinForms.Chart _chartSecurity;
      private ChartFX.WinForms.Chart _chartPrivUser;
      private ChartFX.WinForms.Chart _chartDDL;
      private ChartFX.WinForms.Chart _chartActivity;
      private GroupPanel _pnlStatus;
      private GroupPanel _pnlReportCard;
      private Qios.DevSuite.Components.QShape qShape1;
      private Qios.DevSuite.Components.QPanel _headerEvents;
      private System.Windows.Forms.Label _lblEvents;
      private Qios.DevSuite.Components.QPanel _headerAuditedActivity;
      private System.Windows.Forms.Label _lblAuditedActivity;
      private Infragistics.Win.UltraWinDataSource.UltraDataSource _dsEvents;
      private Infragistics.Win.UltraWinGrid.UltraGrid _grid;
      private System.Windows.Forms.Label _lblRecentAlertCount;
      private Infragistics.Win.FormattedLinkLabel.UltraFormattedLinkLabel _flblAuditSettings;
   }
}
