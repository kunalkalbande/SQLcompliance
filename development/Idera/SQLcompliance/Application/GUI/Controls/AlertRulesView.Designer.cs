namespace Idera.SQLcompliance.Application.GUI.Controls
{
   partial class AlertRulesView
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
          Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Alert Rules", -1);
          Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Rule");
          Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Description");
          Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Server");
          Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Level");
          Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Email");
          Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("EventLog");
          Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Icon");
          Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("RuleType");
          Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn9 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("SnmpTrap");
          Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn1 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Rule");
          Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn2 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Description");
          Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn3 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Server");
          Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn4 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Level");
          Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn5 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Email");
          Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn6 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("EventLog");
          Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn7 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Icon");
          Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn8 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("RuleType");
          Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn9 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("SnmpTrap");
          Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Infragistics.Win.UltraWinToolbars.ButtonTool("newRule");
          Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Infragistics.Win.UltraWinToolbars.ButtonTool("ruleTemplate");
          Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool3 = new Infragistics.Win.UltraWinToolbars.ButtonTool("enableRule");
          Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool4 = new Infragistics.Win.UltraWinToolbars.ButtonTool("disableRule");
          Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool5 = new Infragistics.Win.UltraWinToolbars.ButtonTool("delete");
          Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool6 = new Infragistics.Win.UltraWinToolbars.ButtonTool("refresh");
          Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool7 = new Infragistics.Win.UltraWinToolbars.ButtonTool("properties");
          Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool1 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("contextMenu");
          Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool8 = new Infragistics.Win.UltraWinToolbars.ButtonTool("newRule");
          Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool9 = new Infragistics.Win.UltraWinToolbars.ButtonTool("newStatusRule");
          Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool10 = new Infragistics.Win.UltraWinToolbars.ButtonTool("newDataRule");
          Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool11 = new Infragistics.Win.UltraWinToolbars.ButtonTool("ruleTemplate");
          Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool12 = new Infragistics.Win.UltraWinToolbars.ButtonTool("enableRule");
          Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool13 = new Infragistics.Win.UltraWinToolbars.ButtonTool("disableRule");
          Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool14 = new Infragistics.Win.UltraWinToolbars.ButtonTool("delete");
          Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool15 = new Infragistics.Win.UltraWinToolbars.ButtonTool("refresh");
          Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool16 = new Infragistics.Win.UltraWinToolbars.ButtonTool("properties");
          Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool17 = new Infragistics.Win.UltraWinToolbars.ButtonTool("newStatusRule");
          Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool18 = new Infragistics.Win.UltraWinToolbars.ButtonTool("newDataRule");
          this._gbRuleDescription = new System.Windows.Forms.GroupBox();
          this._rtfRuleDetails = new System.Windows.Forms.RichTextBox();
          this._splitter = new System.Windows.Forms.SplitContainer();
          this._grid = new Infragistics.Win.UltraWinGrid.UltraGrid();
          this._dsAlertRules = new Infragistics.Win.UltraWinDataSource.UltraDataSource(this.components);
          this._toolbarsManager = new Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(this.components);
          this._BaseControl_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
          this._BaseControl_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
          this._BaseControl_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
          this._BaseControl_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
          this._gbRuleDescription.SuspendLayout();
          this._splitter.Panel1.SuspendLayout();
          this._splitter.Panel2.SuspendLayout();
          this._splitter.SuspendLayout();
          ((System.ComponentModel.ISupportInitialize)(this._grid)).BeginInit();
          ((System.ComponentModel.ISupportInitialize)(this._dsAlertRules)).BeginInit();
          ((System.ComponentModel.ISupportInitialize)(this._toolbarsManager)).BeginInit();
          this.SuspendLayout();
          // 
          // _gbRuleDescription
          // 
          this._gbRuleDescription.Controls.Add(this._rtfRuleDetails);
          this._gbRuleDescription.Dock = System.Windows.Forms.DockStyle.Fill;
          this._gbRuleDescription.Location = new System.Drawing.Point(0, 0);
          this._gbRuleDescription.Name = "_gbRuleDescription";
          this._gbRuleDescription.Size = new System.Drawing.Size(770, 335);
          this._gbRuleDescription.TabIndex = 1;
          this._gbRuleDescription.TabStop = false;
          this._gbRuleDescription.Text = "Rule Description (click an underlined value to edit)";
          // 
          // _rtfRuleDetails
          // 
          this._rtfRuleDetails.Dock = System.Windows.Forms.DockStyle.Fill;
          this._rtfRuleDetails.Location = new System.Drawing.Point(3, 17);
          this._rtfRuleDetails.Name = "_rtfRuleDetails";
          this._rtfRuleDetails.Size = new System.Drawing.Size(764, 315);
          this._rtfRuleDetails.TabIndex = 0;
          this._rtfRuleDetails.TabStop = false;
          this._rtfRuleDetails.Text = "";
          this._rtfRuleDetails.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MouseMove_rtfRuleDetails);
          this._rtfRuleDetails.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MouseDown_rtfRuleDetails);
          // 
          // _splitter
          // 
          this._splitter.Dock = System.Windows.Forms.DockStyle.Fill;
          this._splitter.Location = new System.Drawing.Point(0, 0);
          this._splitter.Name = "_splitter";
          this._splitter.Orientation = System.Windows.Forms.Orientation.Horizontal;
          // 
          // _splitter.Panel1
          // 
          this._splitter.Panel1.Controls.Add(this._grid);
          // 
          // _splitter.Panel2
          // 
          this._splitter.Panel2.Controls.Add(this._gbRuleDescription);
          this._splitter.Size = new System.Drawing.Size(770, 670);
          this._splitter.SplitterDistance = 331;
          this._splitter.TabIndex = 2;
          // 
          // _grid
          // 
          this._toolbarsManager.SetContextMenuUltra(this._grid, "contextMenu");
          this._grid.DataSource = this._dsAlertRules;
          ultraGridColumn1.Header.VisiblePosition = 1;
          ultraGridColumn1.Width = 238;
          ultraGridColumn2.Header.VisiblePosition = 3;
          ultraGridColumn2.Hidden = true;
          ultraGridColumn3.Header.VisiblePosition = 4;
          ultraGridColumn3.Width = 226;
          ultraGridColumn4.Header.VisiblePosition = 5;
          ultraGridColumn4.Width = 88;
          ultraGridColumn5.Header.VisiblePosition = 6;
          ultraGridColumn5.Width = 40;
          ultraGridColumn6.Header.VisiblePosition = 7;
          ultraGridColumn6.Width = 64;
          ultraGridColumn7.Header.Caption = "";
          ultraGridColumn7.Header.VisiblePosition = 0;
          ultraGridColumn7.LockedWidth = true;
          ultraGridColumn7.Width = 22;
          ultraGridColumn8.Header.Caption = "Rule Type";
          ultraGridColumn8.Header.VisiblePosition = 2;
          ultraGridColumn8.Width = 68;
          ultraGridColumn9.Header.VisiblePosition = 8;
          ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4,
            ultraGridColumn5,
            ultraGridColumn6,
            ultraGridColumn7,
            ultraGridColumn8,
            ultraGridColumn9});
          this._grid.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
          this._grid.Dock = System.Windows.Forms.DockStyle.Fill;
          this._grid.Location = new System.Drawing.Point(0, 0);
          this._grid.Name = "_grid";
          this._grid.Size = new System.Drawing.Size(770, 331);
          this._grid.TabIndex = 0;
          this._grid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MouseDown_grid);
          this._grid.AfterSelectChange += new Infragistics.Win.UltraWinGrid.AfterSelectChangeEventHandler(this.AfterSelectChange_grid);
          this._grid.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyDown_grid);
          this._grid.DoubleClickRow += new Infragistics.Win.UltraWinGrid.DoubleClickRowEventHandler(this.DoubleClickRow_grid);
          // 
          // _dsAlertRules
          // 
          ultraDataColumn7.DataType = typeof(System.Drawing.Bitmap);
          this._dsAlertRules.Band.Columns.AddRange(new object[] {
            ultraDataColumn1,
            ultraDataColumn2,
            ultraDataColumn3,
            ultraDataColumn4,
            ultraDataColumn5,
            ultraDataColumn6,
            ultraDataColumn7,
            ultraDataColumn8,
            ultraDataColumn9});
          this._dsAlertRules.Band.Key = "Alert Rules";
          // 
          // _toolbarsManager
          // 
          this._toolbarsManager.DesignerFlags = 1;
          this._toolbarsManager.DockWithinContainer = this;
          this._toolbarsManager.ShowFullMenusDelay = 500;
          buttonTool1.SharedProps.Caption = "New Event Alert Rule...";
          buttonTool2.SharedProps.Caption = "Use Existing Rule as Template...";
          buttonTool3.SharedProps.Caption = "Enable Alert Rule";
          buttonTool4.SharedProps.Caption = "Disable Alert Rule";
          buttonTool5.SharedProps.Caption = "Delete";
          buttonTool6.SharedProps.Caption = "Refresh";
          buttonTool7.SharedProps.Caption = "Properties";
          popupMenuTool1.SharedProps.Caption = "contextMenu";
          buttonTool12.InstanceProps.IsFirstInGroup = true;
          buttonTool14.InstanceProps.IsFirstInGroup = true;
          buttonTool16.InstanceProps.IsFirstInGroup = true;
          popupMenuTool1.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool8,
            buttonTool9,
            buttonTool10,
            buttonTool11,
            buttonTool12,
            buttonTool13,
            buttonTool14,
            buttonTool15,
            buttonTool16});
          buttonTool17.SharedProps.Caption = "New Status Alert Rule...";
          buttonTool18.SharedProps.Caption = "New Data Alert Rule...";
          this._toolbarsManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool1,
            buttonTool2,
            buttonTool3,
            buttonTool4,
            buttonTool5,
            buttonTool6,
            buttonTool7,
            popupMenuTool1,
            buttonTool17,
            buttonTool18});
          this._toolbarsManager.ToolClick += new Infragistics.Win.UltraWinToolbars.ToolClickEventHandler(this.ToolClick_toolbarsManager);
          this._toolbarsManager.BeforeToolDropdown += new Infragistics.Win.UltraWinToolbars.BeforeToolDropdownEventHandler(this.BeforeToolDropdown_toolbarsManager);
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
          // AlertRulesView
          // 
          this.Controls.Add(this._splitter);
          this.Controls.Add(this._BaseControl_Toolbars_Dock_Area_Left);
          this.Controls.Add(this._BaseControl_Toolbars_Dock_Area_Right);
          this.Controls.Add(this._BaseControl_Toolbars_Dock_Area_Top);
          this.Controls.Add(this._BaseControl_Toolbars_Dock_Area_Bottom);
          this.Name = "AlertRulesView";
          this.Size = new System.Drawing.Size(770, 670);
          this._gbRuleDescription.ResumeLayout(false);
          this._splitter.Panel1.ResumeLayout(false);
          this._splitter.Panel2.ResumeLayout(false);
          this._splitter.ResumeLayout(false);
          ((System.ComponentModel.ISupportInitialize)(this._grid)).EndInit();
          ((System.ComponentModel.ISupportInitialize)(this._dsAlertRules)).EndInit();
          ((System.ComponentModel.ISupportInitialize)(this._toolbarsManager)).EndInit();
          this.ResumeLayout(false);

      }

      #endregion

      //private System.Windows.Forms.Panel _pnlCenter;
      private System.Windows.Forms.GroupBox _gbRuleDescription;
      private System.Windows.Forms.RichTextBox _rtfRuleDetails;
      private System.ComponentModel.IContainer components;
      private System.Windows.Forms.SplitContainer _splitter;
      private Infragistics.Win.UltraWinDataSource.UltraDataSource _dsAlertRules;
      private Infragistics.Win.UltraWinGrid.UltraGrid _grid;
      private Infragistics.Win.UltraWinToolbars.UltraToolbarsManager _toolbarsManager;
      private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseControl_Toolbars_Dock_Area_Left;
      private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseControl_Toolbars_Dock_Area_Right;
      private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseControl_Toolbars_Dock_Area_Top;
      private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseControl_Toolbars_Dock_Area_Bottom;
   }
}