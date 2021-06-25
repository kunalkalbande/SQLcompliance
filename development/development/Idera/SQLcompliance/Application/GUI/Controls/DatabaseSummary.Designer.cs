namespace Idera.SQLcompliance.Application.GUI.Controls
{
   partial class DatabaseSummary
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
         System.Windows.Forms.Label @__lblLastModified;
         System.Windows.Forms.Label @__lblAuditedSince;
         ChartFX.WinForms.Adornments.SolidBackground solidBackground1 = new ChartFX.WinForms.Adornments.SolidBackground();
         ChartFX.WinForms.Adornments.SolidBackground solidBackground2 = new ChartFX.WinForms.Adornments.SolidBackground();
         Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
         Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Events", -1);
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("startTime");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("eventTypeString");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("spid");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("applicationName");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("hostName");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("serverName");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("loginName");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("success");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn9 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("databaseName");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn10 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("dbUserName");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn11 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("objectName");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn12 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("targetLoginName");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn13 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("targetUserName");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn14 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("roleName");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn15 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ownerName");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn16 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("targetObject");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn17 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("details");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn18 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("eventCategoryString");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn19 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("privilegedUser");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn20 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("sessionLoginName");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn21 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("sqlText");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn1 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("startTime");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn2 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("eventTypeString");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn3 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("spid");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn4 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("applicationName");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn5 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("hostName");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn6 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("serverName");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn7 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("loginName");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn8 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("success");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn9 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("databaseName");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn10 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("dbUserName");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn11 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("objectName");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn12 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("targetLoginName");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn13 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("targetUserName");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn14 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("roleName");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn15 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("ownerName");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn16 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("targetObject");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn17 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("details");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn18 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("eventCategoryString");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn19 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("privilegedUser");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn20 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("sessionLoginName");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn21 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("sqlText");
         this._lblAuditedActivity = new System.Windows.Forms.Label();
         this._lblDatabaseStatus = new System.Windows.Forms.Label();
         this._lblAuditedSince = new System.Windows.Forms.Label();
         this._lblLastModified = new System.Windows.Forms.Label();
         this._chart1 = new ChartFX.WinForms.Chart();
         this._lblRecentActivity = new System.Windows.Forms.Label();
         this._chartEventDistribution = new ChartFX.WinForms.Chart();
         this._headerAuditedActivity = new Qios.DevSuite.Components.QPanel();
         this._pnlStatus = new Idera.SQLcompliance.Application.GUI.Controls.GroupPanel();
         this._flblAuditedActivity = new Infragistics.Win.FormattedLinkLabel.UltraFormattedLinkLabel();
         this._pnlRecentActivity = new Idera.SQLcompliance.Application.GUI.Controls.GroupPanel();
         this._grid = new Infragistics.Win.UltraWinGrid.UltraGrid();
         this._dsEvents = new Infragistics.Win.UltraWinDataSource.UltraDataSource(this.components);
         this._headerEvents = new Qios.DevSuite.Components.QPanel();
         this._lblEvents = new System.Windows.Forms.Label();
         @__lblLastModified = new System.Windows.Forms.Label();
         @__lblAuditedSince = new System.Windows.Forms.Label();
         ((System.ComponentModel.ISupportInitialize)(this._chart1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this._chartEventDistribution)).BeginInit();
         this._headerAuditedActivity.SuspendLayout();
         this._pnlStatus.SuspendLayout();
         this._pnlRecentActivity.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this._grid)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this._dsEvents)).BeginInit();
         this._headerEvents.SuspendLayout();
         this.SuspendLayout();
         // 
         // __lblLastModified
         // 
         @__lblLastModified.AutoSize = true;
         @__lblLastModified.Location = new System.Drawing.Point(6, 61);
         @__lblLastModified.Name = "__lblLastModified";
         @__lblLastModified.Size = new System.Drawing.Size(81, 15);
         @__lblLastModified.TabIndex = 20;
         @__lblLastModified.Text = "Last Modified";
         @__lblLastModified.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         @__lblLastModified.Visible = false;
         // 
         // __lblAuditedSince
         // 
         @__lblAuditedSince.AutoSize = true;
         @__lblAuditedSince.Location = new System.Drawing.Point(6, 36);
         @__lblAuditedSince.Name = "__lblAuditedSince";
         @__lblAuditedSince.Size = new System.Drawing.Size(82, 15);
         @__lblAuditedSince.TabIndex = 18;
         @__lblAuditedSince.Text = "Audited Since";
         @__lblAuditedSince.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         @__lblAuditedSince.Visible = false;
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
         this._lblAuditedActivity.Text = "Audited Activity";
         this._lblAuditedActivity.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // _lblDatabaseStatus
         // 
         this._lblDatabaseStatus.BackColor = System.Drawing.Color.Transparent;
         this._lblDatabaseStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this._lblDatabaseStatus.Location = new System.Drawing.Point(3, 0);
         this._lblDatabaseStatus.Name = "_lblDatabaseStatus";
         this._lblDatabaseStatus.Size = new System.Drawing.Size(160, 27);
         this._lblDatabaseStatus.TabIndex = 0;
         this._lblDatabaseStatus.Text = "Event Distribution";
         this._lblDatabaseStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // _lblAuditedSince
         // 
         this._lblAuditedSince.Location = new System.Drawing.Point(92, 36);
         this._lblAuditedSince.Name = "_lblAuditedSince";
         this._lblAuditedSince.Size = new System.Drawing.Size(126, 15);
         this._lblAuditedSince.TabIndex = 19;
         this._lblAuditedSince.Text = "06/08/2006 by abullwinkel";
         this._lblAuditedSince.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
         this._lblAuditedSince.Visible = false;
         // 
         // _lblLastModified
         // 
         this._lblLastModified.Location = new System.Drawing.Point(95, 61);
         this._lblLastModified.Name = "_lblLastModified";
         this._lblLastModified.Size = new System.Drawing.Size(123, 15);
         this._lblLastModified.TabIndex = 21;
         this._lblLastModified.Text = "85,865";
         this._lblLastModified.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
         this._lblLastModified.Visible = false;
         // 
         // _chart1
         // 
         this._chart1.AllSeries.MarkerSize = ((short)(1));
         this._chart1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         solidBackground1.AssemblyName = "ChartFX.WinForms.Adornments";
         this._chart1.Background = solidBackground1;
         this._chart1.Border = new ChartFX.WinForms.Adornments.SimpleBorder(ChartFX.WinForms.Adornments.SimpleBorderType.Color, System.Drawing.Color.Transparent);
         this._chart1.ContextMenus = false;
         this._chart1.LegendBox.PlotAreaOnly = false;
         this._chart1.Location = new System.Drawing.Point(1, 26);
         this._chart1.Name = "_chart1";
         this._chart1.Size = new System.Drawing.Size(525, 224);
         this._chart1.TabIndex = 48;
         // 
         // _lblRecentActivity
         // 
         this._lblRecentActivity.BackColor = System.Drawing.Color.Transparent;
         this._lblRecentActivity.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this._lblRecentActivity.Location = new System.Drawing.Point(3, 0);
         this._lblRecentActivity.Name = "_lblRecentActivity";
         this._lblRecentActivity.Size = new System.Drawing.Size(279, 27);
         this._lblRecentActivity.TabIndex = 0;
         this._lblRecentActivity.Text = "Recent Database Activity";
         this._lblRecentActivity.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // _chartEventDistribution
         // 
         this._chartEventDistribution.AllSeries.Gallery = ChartFX.WinForms.Gallery.Pie;
         solidBackground2.AssemblyName = "ChartFX.WinForms.Adornments";
         this._chartEventDistribution.Background = solidBackground2;
         this._chartEventDistribution.Border = new ChartFX.WinForms.Adornments.SimpleBorder(ChartFX.WinForms.Adornments.SimpleBorderType.Color, System.Drawing.Color.Transparent);
         this._chartEventDistribution.ContextMenus = false;
         this._chartEventDistribution.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         ((ChartFX.WinForms.Galleries.Pie)(this._chartEventDistribution.GalleryAttributes)).Shadows = true;
         this._chartEventDistribution.LegendBox.ContentLayout = ChartFX.WinForms.ContentLayout.Spread;
         this._chartEventDistribution.LegendBox.Dock = ChartFX.WinForms.DockArea.Bottom;
         this._chartEventDistribution.Location = new System.Drawing.Point(6, 36);
         this._chartEventDistribution.Name = "_chartEventDistribution";
         this._chartEventDistribution.PlotAreaMargin.Bottom = 1;
         this._chartEventDistribution.PlotAreaMargin.Left = 1;
         this._chartEventDistribution.PlotAreaMargin.Right = 1;
         this._chartEventDistribution.PlotAreaMargin.Top = 1;
         this._chartEventDistribution.RandomData.Series = 2;
         this._chartEventDistribution.Size = new System.Drawing.Size(224, 233);
         this._chartEventDistribution.TabIndex = 31;
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
         this._headerAuditedActivity.ColorScheme.PanelBackground2.SetColor("LunaBlue", System.Drawing.Color.FromArgb(((int)(((byte)(129)))), ((int)(((byte)(169)))), ((int)(((byte)(226))))), false);
         this._headerAuditedActivity.ColorScheme.PanelBackground2.SetColor("LunaOlive", System.Drawing.Color.FromArgb(((int)(((byte)(183)))), ((int)(((byte)(198)))), ((int)(((byte)(145))))), false);
         this._headerAuditedActivity.ColorScheme.PanelBackground2.SetColor("LunaSilver", System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(151)))), ((int)(((byte)(181))))), false);
         this._headerAuditedActivity.ColorScheme.PanelBorder.SetColor("LunaBlue", System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(45)))), ((int)(((byte)(150))))), false);
         this._headerAuditedActivity.ColorScheme.PanelBorder.SetColor("LunaOlive", System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(128)))), ((int)(((byte)(88))))), false);
         this._headerAuditedActivity.ColorScheme.PanelBorder.SetColor("LunaSilver", System.Drawing.Color.FromArgb(((int)(((byte)(124)))), ((int)(((byte)(124)))), ((int)(((byte)(148))))), false);
         this._headerAuditedActivity.Controls.Add(this._lblAuditedActivity);
         this._headerAuditedActivity.Location = new System.Drawing.Point(1, 275);
         this._headerAuditedActivity.MinimumClientSize = new System.Drawing.Size(10, 10);
         this._headerAuditedActivity.Name = "_headerAuditedActivity";
         this._headerAuditedActivity.Size = new System.Drawing.Size(235, 27);
         this._headerAuditedActivity.TabIndex = 9;
         this._headerAuditedActivity.Text = "qPanel4";
         // 
         // _pnlStatus
         // 
         this._pnlStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                     | System.Windows.Forms.AnchorStyles.Left)));
         this._pnlStatus.BackColor = System.Drawing.Color.Transparent;
         this._pnlStatus.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(144)))), ((int)(((byte)(144)))), ((int)(((byte)(144)))));
         this._pnlStatus.Controls.Add(this._flblAuditedActivity);
         this._pnlStatus.Controls.Add(this._chartEventDistribution);
         this._pnlStatus.Controls.Add(this._lblDatabaseStatus);
         this._pnlStatus.Controls.Add(@__lblAuditedSince);
         this._pnlStatus.Controls.Add(this._lblLastModified);
         this._pnlStatus.Controls.Add(this._lblAuditedSince);
         this._pnlStatus.Controls.Add(@__lblLastModified);
         this._pnlStatus.Controls.Add(this._headerAuditedActivity);
         this._pnlStatus.GroupBoxBackColor = System.Drawing.Color.White;
         this._pnlStatus.HeaderBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(219)))), ((int)(((byte)(224)))));
         this._pnlStatus.HeaderBackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(187)))), ((int)(((byte)(197)))));
         this._pnlStatus.Location = new System.Drawing.Point(3, 3);
         this._pnlStatus.Name = "_pnlStatus";
         this._pnlStatus.RoundedBottoms = false;
         this._pnlStatus.Size = new System.Drawing.Size(238, 451);
         this._pnlStatus.TabIndex = 18;
         // 
         // _flblAuditedActivity
         // 
         this._flblAuditedActivity.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                     | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         appearance1.BorderColor = System.Drawing.Color.Transparent;
         appearance1.BorderColor2 = System.Drawing.Color.Transparent;
         this._flblAuditedActivity.Appearance = appearance1;
         this._flblAuditedActivity.Location = new System.Drawing.Point(6, 308);
         this._flblAuditedActivity.Name = "_flblAuditedActivity";
         this._flblAuditedActivity.Size = new System.Drawing.Size(212, 133);
         this._flblAuditedActivity.TabIndex = 52;
         this._flblAuditedActivity.TabStop = true;
         this._flblAuditedActivity.Value = "<span style=\"font-weight:bold_x003B_\">Database </span>- DDL, Security, Admin, DML" +
             " (filtered), Select (filtered), Capture SQL";
         // 
         // _pnlRecentActivity
         // 
         this._pnlRecentActivity.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                     | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this._pnlRecentActivity.BackColor = System.Drawing.Color.Transparent;
         this._pnlRecentActivity.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(144)))), ((int)(((byte)(144)))), ((int)(((byte)(144)))));
         this._pnlRecentActivity.Controls.Add(this._grid);
         this._pnlRecentActivity.Controls.Add(this._headerEvents);
         this._pnlRecentActivity.Controls.Add(this._lblRecentActivity);
         this._pnlRecentActivity.Controls.Add(this._chart1);
         this._pnlRecentActivity.GroupBoxBackColor = System.Drawing.Color.White;
         this._pnlRecentActivity.HeaderBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(219)))), ((int)(((byte)(224)))));
         this._pnlRecentActivity.HeaderBackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(187)))), ((int)(((byte)(197)))));
         this._pnlRecentActivity.Location = new System.Drawing.Point(244, 3);
         this._pnlRecentActivity.Name = "_pnlRecentActivity";
         this._pnlRecentActivity.RoundedBottoms = false;
         this._pnlRecentActivity.Size = new System.Drawing.Size(528, 451);
         this._pnlRecentActivity.TabIndex = 19;
         // 
         // _grid
         // 
         this._grid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                     | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this._grid.DataSource = this._dsEvents;
         ultraGridColumn1.Format = "G";
         ultraGridColumn1.Header.Caption = "Time";
         ultraGridColumn1.Header.VisiblePosition = 2;
         ultraGridColumn1.Width = 126;
         ultraGridColumn2.Header.Caption = "Event";
         ultraGridColumn2.Header.VisiblePosition = 1;
         ultraGridColumn2.Width = 113;
         ultraGridColumn3.Header.Caption = "Spid";
         ultraGridColumn3.Header.VisiblePosition = 7;
         ultraGridColumn3.Hidden = true;
         ultraGridColumn4.Header.Caption = "Application";
         ultraGridColumn4.Header.VisiblePosition = 8;
         ultraGridColumn4.Hidden = true;
         ultraGridColumn5.Header.Caption = "Host";
         ultraGridColumn5.Header.VisiblePosition = 9;
         ultraGridColumn5.Hidden = true;
         ultraGridColumn6.Header.Caption = "Server";
         ultraGridColumn6.Header.VisiblePosition = 10;
         ultraGridColumn6.Hidden = true;
         ultraGridColumn7.Header.Caption = "Login";
         ultraGridColumn7.Header.VisiblePosition = 3;
         ultraGridColumn7.Hidden = true;
         ultraGridColumn7.Width = 117;
         ultraGridColumn8.Header.Caption = "Success";
         ultraGridColumn8.Header.VisiblePosition = 11;
         ultraGridColumn8.Hidden = true;
         ultraGridColumn9.Header.Caption = "Database";
         ultraGridColumn9.Header.VisiblePosition = 4;
         ultraGridColumn9.Hidden = true;
         ultraGridColumn9.Width = 105;
         ultraGridColumn10.Header.Caption = "Database User";
         ultraGridColumn10.Header.VisiblePosition = 12;
         ultraGridColumn10.Hidden = true;
         ultraGridColumn11.Header.Caption = "Object";
         ultraGridColumn11.Header.VisiblePosition = 13;
         ultraGridColumn11.Hidden = true;
         ultraGridColumn12.Header.Caption = "Target Login";
         ultraGridColumn12.Header.VisiblePosition = 14;
         ultraGridColumn12.Hidden = true;
         ultraGridColumn13.Header.Caption = "Target User";
         ultraGridColumn13.Header.VisiblePosition = 15;
         ultraGridColumn13.Hidden = true;
         ultraGridColumn14.Header.Caption = "Role";
         ultraGridColumn14.Header.VisiblePosition = 16;
         ultraGridColumn14.Hidden = true;
         ultraGridColumn15.Header.Caption = "Owner";
         ultraGridColumn15.Header.VisiblePosition = 17;
         ultraGridColumn15.Hidden = true;
         ultraGridColumn16.Header.Caption = "Target Object";
         ultraGridColumn16.Header.VisiblePosition = 5;
         ultraGridColumn16.Hidden = true;
         ultraGridColumn16.Width = 121;
         ultraGridColumn17.Header.Caption = "Details";
         ultraGridColumn17.Header.VisiblePosition = 6;
         ultraGridColumn17.Width = 169;
         ultraGridColumn18.Header.Caption = "Category";
         ultraGridColumn18.Header.VisiblePosition = 0;
         ultraGridColumn18.Width = 117;
         ultraGridColumn19.Header.Caption = "Privileged User";
         ultraGridColumn19.Header.VisiblePosition = 18;
         ultraGridColumn19.Hidden = true;
         ultraGridColumn20.Header.Caption = "Session Login";
         ultraGridColumn20.Header.VisiblePosition = 19;
         ultraGridColumn20.Hidden = true;
         ultraGridColumn21.Header.VisiblePosition = 20;
         ultraGridColumn21.Hidden = true;
         ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4,
            ultraGridColumn5,
            ultraGridColumn6,
            ultraGridColumn7,
            ultraGridColumn8,
            ultraGridColumn9,
            ultraGridColumn10,
            ultraGridColumn11,
            ultraGridColumn12,
            ultraGridColumn13,
            ultraGridColumn14,
            ultraGridColumn15,
            ultraGridColumn16,
            ultraGridColumn17,
            ultraGridColumn18,
            ultraGridColumn19,
            ultraGridColumn20,
            ultraGridColumn21});
         this._grid.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
         this._grid.Location = new System.Drawing.Point(1, 276);
         this._grid.Name = "_grid";
         this._grid.Size = new System.Drawing.Size(525, 173);
         this._grid.TabIndex = 52;
         this._grid.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyDown_grid);
         this._grid.DoubleClickRow += new Infragistics.Win.UltraWinGrid.DoubleClickRowEventHandler(this.DoubleClickRow_grid);
         // 
         // _dsEvents
         // 
         ultraDataColumn1.DataType = typeof(System.DateTime);
         ultraDataColumn3.DataType = typeof(int);
         ultraDataColumn8.DataType = typeof(bool);
         ultraDataColumn19.DataType = typeof(bool);
         this._dsEvents.Band.Columns.AddRange(new object[] {
            ultraDataColumn1,
            ultraDataColumn2,
            ultraDataColumn3,
            ultraDataColumn4,
            ultraDataColumn5,
            ultraDataColumn6,
            ultraDataColumn7,
            ultraDataColumn8,
            ultraDataColumn9,
            ultraDataColumn10,
            ultraDataColumn11,
            ultraDataColumn12,
            ultraDataColumn13,
            ultraDataColumn14,
            ultraDataColumn15,
            ultraDataColumn16,
            ultraDataColumn17,
            ultraDataColumn18,
            ultraDataColumn19,
            ultraDataColumn20,
            ultraDataColumn21});
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
         this._headerEvents.TabIndex = 51;
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
         // DatabaseSummary
         // 
         this.AutoScroll = true;
         this.AutoScrollMinSize = new System.Drawing.Size(481, 359);
         this.Controls.Add(this._pnlRecentActivity);
         this.Controls.Add(this._pnlStatus);
         this.Name = "DatabaseSummary";
         this.Size = new System.Drawing.Size(775, 456);
         this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.DatabaseSummary_HelpRequested);
         ((System.ComponentModel.ISupportInitialize)(this._chart1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this._chartEventDistribution)).EndInit();
         this._headerAuditedActivity.ResumeLayout(false);
         this._pnlStatus.ResumeLayout(false);
         this._pnlStatus.PerformLayout();
         this._pnlRecentActivity.ResumeLayout(false);
         this._pnlRecentActivity.PerformLayout();
         ((System.ComponentModel.ISupportInitialize)(this._grid)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this._dsEvents)).EndInit();
         this._headerEvents.ResumeLayout(false);
         this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.Label _lblDatabaseStatus;
      private System.Windows.Forms.Label _lblAuditedSince;
      private System.Windows.Forms.Label _lblLastModified;
      private System.Windows.Forms.Label _lblRecentActivity;
      private Qios.DevSuite.Components.QPanel _headerAuditedActivity;
      private ChartFX.WinForms.Chart _chart1;
      private ChartFX.WinForms.Chart _chartEventDistribution;
      private GroupPanel _pnlStatus;
      private GroupPanel _pnlRecentActivity;
      private Infragistics.Win.FormattedLinkLabel.UltraFormattedLinkLabel _flblAuditedActivity;
      private System.Windows.Forms.Label _lblAuditedActivity;
      private Infragistics.Win.UltraWinDataSource.UltraDataSource _dsEvents;
      private Infragistics.Win.UltraWinGrid.UltraGrid _grid;
      private Qios.DevSuite.Components.QPanel _headerEvents;
      private System.Windows.Forms.Label _lblEvents;
   }
}
