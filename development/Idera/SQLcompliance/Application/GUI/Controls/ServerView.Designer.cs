namespace Idera.SQLcompliance.Application.GUI.Controls
{
   partial class ServerView
   {

      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      protected override void Dispose(bool disposing)
      {
         if (disposing)
         {
            if (components != null)
            {
               components.Dispose();
            }
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
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand2 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Databases", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Icon");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Name");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Status");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn9 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Description");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn6 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Icon");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn7 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Name");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn8 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Status");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn9 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Description");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn10 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("IsAlwaysOn");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn11 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Role");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Servers", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Icon");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Server");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Status");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("AuditStatus");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("LastContact");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn10 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("DeploymentMethod", 0);
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn1 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Icon");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn2 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Server");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn3 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Status");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn4 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("AuditStatus");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn5 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("LastContact");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn12 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("DeploymentMethod");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Infragistics.Win.UltraWinToolbars.ButtonTool("newServer");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Infragistics.Win.UltraWinToolbars.ButtonTool("newDatabase");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool3 = new Infragistics.Win.UltraWinToolbars.ButtonTool("enableAuditing");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool4 = new Infragistics.Win.UltraWinToolbars.ButtonTool("disableAuditing");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool5 = new Infragistics.Win.UltraWinToolbars.ButtonTool("remove");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool6 = new Infragistics.Win.UltraWinToolbars.ButtonTool("refresh");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool7 = new Infragistics.Win.UltraWinToolbars.ButtonTool("updateNow");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool8 = new Infragistics.Win.UltraWinToolbars.ButtonTool("exportAuditSettings");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool9 = new Infragistics.Win.UltraWinToolbars.ButtonTool("collectNow");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool10 = new Infragistics.Win.UltraWinToolbars.ButtonTool("agentDeploy");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool11 = new Infragistics.Win.UltraWinToolbars.ButtonTool("agentUpgrade");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool12 = new Infragistics.Win.UltraWinToolbars.ButtonTool("agentStatus");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool13 = new Infragistics.Win.UltraWinToolbars.ButtonTool("agentTraceDirectory");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool14 = new Infragistics.Win.UltraWinToolbars.ButtonTool("agentProperties");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool15 = new Infragistics.Win.UltraWinToolbars.ButtonTool("properties");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool1 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("serverContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool16 = new Infragistics.Win.UltraWinToolbars.ButtonTool("newServer");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool17 = new Infragistics.Win.UltraWinToolbars.ButtonTool("newDatabase");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool18 = new Infragistics.Win.UltraWinToolbars.ButtonTool("enableAuditing");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool19 = new Infragistics.Win.UltraWinToolbars.ButtonTool("disableAuditing");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool20 = new Infragistics.Win.UltraWinToolbars.ButtonTool("remove");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool21 = new Infragistics.Win.UltraWinToolbars.ButtonTool("refresh");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool22 = new Infragistics.Win.UltraWinToolbars.ButtonTool("updateNow");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool23 = new Infragistics.Win.UltraWinToolbars.ButtonTool("exportAuditSettings");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool24 = new Infragistics.Win.UltraWinToolbars.ButtonTool("collectNow");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool25 = new Infragistics.Win.UltraWinToolbars.ButtonTool("agentDeploy");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool26 = new Infragistics.Win.UltraWinToolbars.ButtonTool("agentUpgrade");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool27 = new Infragistics.Win.UltraWinToolbars.ButtonTool("agentStatus");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool28 = new Infragistics.Win.UltraWinToolbars.ButtonTool("agentTraceDirectory");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool29 = new Infragistics.Win.UltraWinToolbars.ButtonTool("agentProperties");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool30 = new Infragistics.Win.UltraWinToolbars.ButtonTool("properties");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool38 = new Infragistics.Win.UltraWinToolbars.ButtonTool("selectSensitiveColumnsFromCSV");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool2 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("databaseContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool31 = new Infragistics.Win.UltraWinToolbars.ButtonTool("newDatabase");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool32 = new Infragistics.Win.UltraWinToolbars.ButtonTool("enableAuditing");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool33 = new Infragistics.Win.UltraWinToolbars.ButtonTool("disableAuditing");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool34 = new Infragistics.Win.UltraWinToolbars.ButtonTool("remove");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool35 = new Infragistics.Win.UltraWinToolbars.ButtonTool("refresh");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool36 = new Infragistics.Win.UltraWinToolbars.ButtonTool("exportAuditSettings");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool37 = new Infragistics.Win.UltraWinToolbars.ButtonTool("properties");
            this._gbDatabases = new System.Windows.Forms.GroupBox();
            this._gridDatabases = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this._dsDatabases = new Infragistics.Win.UltraWinDataSource.UltraDataSource(this.components);
            this._splitContainer = new System.Windows.Forms.SplitContainer();
            this._gridServers = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this._dsServers = new Infragistics.Win.UltraWinDataSource.UltraDataSource(this.components);
            this._toolbarsManager = new Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(this.components);
            this._BaseControl_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._BaseControl_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._BaseControl_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._BaseControl_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._gbDatabases.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._gridDatabases)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._dsDatabases)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._splitContainer)).BeginInit();
            this._splitContainer.Panel1.SuspendLayout();
            this._splitContainer.Panel2.SuspendLayout();
            this._splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._gridServers)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._dsServers)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._toolbarsManager)).BeginInit();
            this.SuspendLayout();
            // 
            // _gbDatabases
            // 
            this._gbDatabases.Controls.Add(this._gridDatabases);
            this._gbDatabases.Dock = System.Windows.Forms.DockStyle.Fill;
            this._gbDatabases.Location = new System.Drawing.Point(0, 0);
            this._gbDatabases.Name = "_gbDatabases";
            this._gbDatabases.Size = new System.Drawing.Size(770, 352);
            this._gbDatabases.TabIndex = 1;
            this._gbDatabases.TabStop = false;
            this._gbDatabases.Text = "Audited Databases";
            // 
            // _gridDatabases
            // 
            this._toolbarsManager.SetContextMenuUltra(this._gridDatabases, "databaseContextMenu");
            this._gridDatabases.DataMember = "Databases";
            this._gridDatabases.DataSource = this._dsDatabases;
            ultraGridColumn6.Header.Caption = "";
            ultraGridColumn6.Header.VisiblePosition = 0;
            ultraGridColumn6.LockedWidth = true;
            ultraGridColumn6.Width = 23;
            ultraGridColumn7.Header.VisiblePosition = 1;
            ultraGridColumn7.Width = 245;
            ultraGridColumn8.Header.VisiblePosition = 2;
            ultraGridColumn8.Width = 242;
            ultraGridColumn9.Header.VisiblePosition = 3;
            ultraGridColumn9.Width = 242;
            ultraGridBand2.Columns.AddRange(new object[] {
            ultraGridColumn6,
            ultraGridColumn7,
            ultraGridColumn8,
            ultraGridColumn9});
            this._gridDatabases.DisplayLayout.BandsSerializer.Add(ultraGridBand2);
            this._gridDatabases.Dock = System.Windows.Forms.DockStyle.Fill;
            this._gridDatabases.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._gridDatabases.Location = new System.Drawing.Point(3, 17);
            this._gridDatabases.Name = "_gridDatabases";
            this._gridDatabases.Size = new System.Drawing.Size(764, 332);
            this._gridDatabases.TabIndex = 0;
            this._gridDatabases.Text = "ultraGrid2";
            this._gridDatabases.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            this._gridDatabases.AfterSelectChange += new Infragistics.Win.UltraWinGrid.AfterSelectChangeEventHandler(this.AfterSelectChange_gridDatabases);
            this._gridDatabases.DoubleClickRow += new Infragistics.Win.UltraWinGrid.DoubleClickRowEventHandler(this.DoubleClickRow_gridDatabases);
            this._gridDatabases.Enter += new System.EventHandler(this.FocusChanged_gridDatabases);
            this._gridDatabases.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyDown_gridDatabases);
            this._gridDatabases.Leave += new System.EventHandler(this.FocusChanged_gridDatabases);
            this._gridDatabases.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MouseDown_gridDatabases);
            // 
            // _dsDatabases
            // 
            ultraDataColumn6.DataType = typeof(System.Drawing.Bitmap);
            this._dsDatabases.Band.Columns.AddRange(new object[] {
            ultraDataColumn6,
            ultraDataColumn7,
            ultraDataColumn8,
            ultraDataColumn9,
            ultraDataColumn10,
            ultraDataColumn11});
            this._dsDatabases.Band.Key = "Databases";
            // 
            // _splitContainer
            // 
            this._splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this._splitContainer.Location = new System.Drawing.Point(0, 0);
            this._splitContainer.Name = "_splitContainer";
            this._splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // _splitContainer.Panel1
            // 
            this._splitContainer.Panel1.Controls.Add(this._gridServers);
            // 
            // _splitContainer.Panel2
            // 
            this._splitContainer.Panel2.Controls.Add(this._gbDatabases);
            this._splitContainer.Size = new System.Drawing.Size(770, 670);
            this._splitContainer.SplitterDistance = 314;
            this._splitContainer.TabIndex = 0;
            // 
            // _gridServers
            // 
            this._toolbarsManager.SetContextMenuUltra(this._gridServers, "serverContextMenu");
            this._gridServers.DataMember = "Servers";
            this._gridServers.DataSource = this._dsServers;
            ultraGridColumn1.Header.Caption = "";
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn1.LockedWidth = true;
            ultraGridColumn1.Width = 23;
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn2.Width = 100;
            ultraGridColumn3.Header.VisiblePosition = 2;
            ultraGridColumn3.Width = 100;
            ultraGridColumn4.Header.Caption = "Audit Status";
            ultraGridColumn4.Header.VisiblePosition = 4;
            ultraGridColumn4.Width = 179;
            ultraGridColumn5.Header.Caption = "Last Agent Contact";
            ultraGridColumn5.Header.VisiblePosition = 5;
            ultraGridColumn5.Width = 200;
            ultraGridColumn10.Header.Caption = "Deployment Method";
            ultraGridColumn10.Header.VisiblePosition = 3;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4,
            ultraGridColumn5,
            ultraGridColumn10});
            this._gridServers.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this._gridServers.Dock = System.Windows.Forms.DockStyle.Fill;
            this._gridServers.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._gridServers.Location = new System.Drawing.Point(0, 0);
            this._gridServers.Name = "_gridServers";
            this._gridServers.Size = new System.Drawing.Size(770, 314);
            this._gridServers.TabIndex = 0;
            this._gridServers.Text = "ultraGrid1";
            this._gridServers.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            this._gridServers.AfterSelectChange += new Infragistics.Win.UltraWinGrid.AfterSelectChangeEventHandler(this.AfterSelectChange_gridServers);
            this._gridServers.DoubleClickRow += new Infragistics.Win.UltraWinGrid.DoubleClickRowEventHandler(this.DoubleClickRow_gridServers);
            this._gridServers.Enter += new System.EventHandler(this.FocusChanged_gridServers);
            this._gridServers.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyDown_gridServers);
            this._gridServers.Leave += new System.EventHandler(this.FocusChanged_gridServers);
            this._gridServers.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MouseDown_gridServers);
            // 
            // _dsServers
            // 
            ultraDataColumn1.DataType = typeof(System.Drawing.Bitmap);
            this._dsServers.Band.Columns.AddRange(new object[] {
            ultraDataColumn1,
            ultraDataColumn2,
            ultraDataColumn3,
            ultraDataColumn4,
            ultraDataColumn5,
            ultraDataColumn12});
            this._dsServers.Band.Key = "Servers";
            // 
            // _toolbarsManager
            // 
            this._toolbarsManager.DesignerFlags = 1;
            this._toolbarsManager.DockWithinContainer = this;
            this._toolbarsManager.ShowFullMenusDelay = 500;
            buttonTool1.SharedPropsInternal.Caption = "New Registered SQL Server...";
            buttonTool2.SharedPropsInternal.Caption = "New Audited Database...";
            buttonTool3.SharedPropsInternal.Caption = "Enable Auditing";
            buttonTool4.SharedPropsInternal.Caption = "Disable Auditing";
            buttonTool5.SharedPropsInternal.Caption = "Remove";
            buttonTool6.SharedPropsInternal.Caption = "Refresh";
            buttonTool7.SharedPropsInternal.Caption = "Update Audit Settings Now";
            buttonTool8.SharedPropsInternal.Caption = "Export Audit Settings";
            buttonTool9.SharedPropsInternal.Caption = "Collect Audit Data Now";
            buttonTool10.SharedPropsInternal.Caption = "Deploy Agent...";
            buttonTool11.SharedPropsInternal.Caption = "Upgrade Agent...";
            buttonTool12.SharedPropsInternal.Caption = "Check Agent Status";
            buttonTool13.SharedPropsInternal.Caption = "Change Agent Trace Directory...";
            buttonTool14.SharedPropsInternal.Caption = "Agent Properties...";
            buttonTool15.SharedPropsInternal.Caption = "Properties...";
            buttonTool38.SharedPropsInternal.Caption = "Select Sensitive Columns From CSV";
            popupMenuTool1.SharedPropsInternal.Caption = "ServerContextMenu";
            buttonTool18.InstanceProps.IsFirstInGroup = true;
            buttonTool20.InstanceProps.IsFirstInGroup = true;
            buttonTool22.InstanceProps.IsFirstInGroup = true;
            buttonTool25.InstanceProps.IsFirstInGroup = true;
            buttonTool30.InstanceProps.IsFirstInGroup = true;
            popupMenuTool1.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool16,
            buttonTool17,
            buttonTool18,
            buttonTool19,
            buttonTool20,
            buttonTool21,
            buttonTool22,
            buttonTool23,
            buttonTool24,
            buttonTool25,
            buttonTool26,
            buttonTool27,
            buttonTool28,
            buttonTool29,
            buttonTool30});
            popupMenuTool2.SharedPropsInternal.Caption = "DatabaseContextMenu";
            buttonTool32.InstanceProps.IsFirstInGroup = true;
            buttonTool34.InstanceProps.IsFirstInGroup = true;
            buttonTool36.InstanceProps.IsFirstInGroup = true;
            buttonTool37.InstanceProps.IsFirstInGroup = true;
            popupMenuTool2.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool31,
            buttonTool32,
            buttonTool33,
            buttonTool34,
            buttonTool35,
            buttonTool36,
            buttonTool37});
            this._toolbarsManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool1,
            buttonTool2,
            buttonTool3,
            buttonTool4,
            buttonTool5,
            buttonTool6,
            buttonTool7,
            buttonTool8,
            buttonTool9,
            buttonTool10,
            buttonTool11,
            buttonTool12,
            buttonTool13,
            buttonTool14,
            buttonTool15,
            buttonTool38,
            popupMenuTool1,
            popupMenuTool2});
            this._toolbarsManager.BeforeToolDropdown += new Infragistics.Win.UltraWinToolbars.BeforeToolDropdownEventHandler(this.BeforeToolDropdown_toolbarsManager);
            this._toolbarsManager.ToolClick += new Infragistics.Win.UltraWinToolbars.ToolClickEventHandler(this.ToolClick_toolbarsManager);
            // 
            // _BaseControl_Toolbars_Dock_Area_Left
            // 
            this._BaseControl_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._BaseControl_Toolbars_Dock_Area_Left.BackColor = System.Drawing.SystemColors.Control;
            this._BaseControl_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
            this._BaseControl_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._BaseControl_Toolbars_Dock_Area_Left.Location = new System.Drawing.Point(0, 0);
            this._BaseControl_Toolbars_Dock_Area_Left.Name = "_BaseControl_Toolbars_Dock_Area_Left";
            this._BaseControl_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(0, 670);
            this._BaseControl_Toolbars_Dock_Area_Left.ToolbarsManager = this._toolbarsManager;
            // 
            // _BaseControl_Toolbars_Dock_Area_Right
            // 
            this._BaseControl_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._BaseControl_Toolbars_Dock_Area_Right.BackColor = System.Drawing.SystemColors.Control;
            this._BaseControl_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._BaseControl_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._BaseControl_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(770, 0);
            this._BaseControl_Toolbars_Dock_Area_Right.Name = "_BaseControl_Toolbars_Dock_Area_Right";
            this._BaseControl_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(0, 670);
            this._BaseControl_Toolbars_Dock_Area_Right.ToolbarsManager = this._toolbarsManager;
            // 
            // _BaseControl_Toolbars_Dock_Area_Top
            // 
            this._BaseControl_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._BaseControl_Toolbars_Dock_Area_Top.BackColor = System.Drawing.SystemColors.Control;
            this._BaseControl_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._BaseControl_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._BaseControl_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._BaseControl_Toolbars_Dock_Area_Top.Name = "_BaseControl_Toolbars_Dock_Area_Top";
            this._BaseControl_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(770, 0);
            this._BaseControl_Toolbars_Dock_Area_Top.ToolbarsManager = this._toolbarsManager;
            // 
            // _BaseControl_Toolbars_Dock_Area_Bottom
            // 
            this._BaseControl_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._BaseControl_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.SystemColors.Control;
            this._BaseControl_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._BaseControl_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._BaseControl_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 670);
            this._BaseControl_Toolbars_Dock_Area_Bottom.Name = "_BaseControl_Toolbars_Dock_Area_Bottom";
            this._BaseControl_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(770, 0);
            this._BaseControl_Toolbars_Dock_Area_Bottom.ToolbarsManager = this._toolbarsManager;
            // 
            // ServerView
            // 
            this.Controls.Add(this._splitContainer);
            this.Controls.Add(this._BaseControl_Toolbars_Dock_Area_Left);
            this.Controls.Add(this._BaseControl_Toolbars_Dock_Area_Right);
            this.Controls.Add(this._BaseControl_Toolbars_Dock_Area_Bottom);
            this.Controls.Add(this._BaseControl_Toolbars_Dock_Area_Top);
            this.Name = "ServerView";
            this.Size = new System.Drawing.Size(770, 670);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.HelpRequested_ServerView);
            this._gbDatabases.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._gridDatabases)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._dsDatabases)).EndInit();
            this._splitContainer.Panel1.ResumeLayout(false);
            this._splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._splitContainer)).EndInit();
            this._splitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._gridServers)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._dsServers)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._toolbarsManager)).EndInit();
            this.ResumeLayout(false);

      }
      #endregion

      private System.Windows.Forms.GroupBox _gbDatabases;
      private System.ComponentModel.IContainer components;
      private System.Windows.Forms.SplitContainer _splitContainer;
      private Infragistics.Win.UltraWinGrid.UltraGrid _gridDatabases;
      private Infragistics.Win.UltraWinGrid.UltraGrid _gridServers;
      private Infragistics.Win.UltraWinDataSource.UltraDataSource _dsServers;
      private Infragistics.Win.UltraWinDataSource.UltraDataSource _dsDatabases;
      private Infragistics.Win.UltraWinToolbars.UltraToolbarsManager _toolbarsManager;
      private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseControl_Toolbars_Dock_Area_Left;
      private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseControl_Toolbars_Dock_Area_Right;
      private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseControl_Toolbars_Dock_Area_Top;
      private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseControl_Toolbars_Dock_Area_Bottom;
	}
}