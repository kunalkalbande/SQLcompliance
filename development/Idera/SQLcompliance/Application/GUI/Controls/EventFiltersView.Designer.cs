namespace Idera.SQLcompliance.Application.GUI.Controls
{
   partial class EventFiltersView
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
         Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("EventFilters", -1);
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Name");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Description");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Server");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Icon");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn1 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Name");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn2 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Description");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn3 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Server");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn4 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Icon");
         Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Infragistics.Win.UltraWinToolbars.ButtonTool("newFilter");
         Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Infragistics.Win.UltraWinToolbars.ButtonTool("filterTemplate");
         Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool3 = new Infragistics.Win.UltraWinToolbars.ButtonTool("enableFilter");
         Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool4 = new Infragistics.Win.UltraWinToolbars.ButtonTool("disableFilter");
         Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool5 = new Infragistics.Win.UltraWinToolbars.ButtonTool("delete");
         Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool6 = new Infragistics.Win.UltraWinToolbars.ButtonTool("refresh");
         Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool7 = new Infragistics.Win.UltraWinToolbars.ButtonTool("properties");
         Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool1 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("contextMenu");
         Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool8 = new Infragistics.Win.UltraWinToolbars.ButtonTool("newFilter");
         Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool9 = new Infragistics.Win.UltraWinToolbars.ButtonTool("filterTemplate");
         Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool10 = new Infragistics.Win.UltraWinToolbars.ButtonTool("enableFilter");
         Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool11 = new Infragistics.Win.UltraWinToolbars.ButtonTool("disableFilter");
         Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool12 = new Infragistics.Win.UltraWinToolbars.ButtonTool("delete");
         Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool13 = new Infragistics.Win.UltraWinToolbars.ButtonTool("refresh");
         Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool14 = new Infragistics.Win.UltraWinToolbars.ButtonTool("properties");
         this._gbFilterDescription = new System.Windows.Forms.GroupBox();
         this._rtfFilterDetails = new System.Windows.Forms.RichTextBox();
         this._splitter = new System.Windows.Forms.SplitContainer();
         this._grid = new Infragistics.Win.UltraWinGrid.UltraGrid();
         this._dataSource = new Infragistics.Win.UltraWinDataSource.UltraDataSource(this.components);
         this._toolbarsManager = new Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(this.components);
         this._BaseControl_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
         this._BaseControl_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
         this._BaseControl_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
         this._BaseControl_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
         this._gbFilterDescription.SuspendLayout();
         this._splitter.Panel1.SuspendLayout();
         this._splitter.Panel2.SuspendLayout();
         this._splitter.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this._grid)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this._dataSource)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this._toolbarsManager)).BeginInit();
         this.SuspendLayout();
         // 
         // _gbFilterDescription
         // 
         this._gbFilterDescription.Controls.Add(this._rtfFilterDetails);
         this._gbFilterDescription.Dock = System.Windows.Forms.DockStyle.Fill;
         this._gbFilterDescription.Location = new System.Drawing.Point(0, 0);
         this._gbFilterDescription.Name = "_gbFilterDescription";
         this._gbFilterDescription.Size = new System.Drawing.Size(770, 332);
         this._gbFilterDescription.TabIndex = 1;
         this._gbFilterDescription.TabStop = false;
         this._gbFilterDescription.Text = "Filter Description (click an underlined value to edit)";
         // 
         // _rtfFilterDetails
         // 
         this._rtfFilterDetails.Dock = System.Windows.Forms.DockStyle.Fill;
         this._rtfFilterDetails.Location = new System.Drawing.Point(3, 19);
         this._rtfFilterDetails.Name = "_rtfFilterDetails";
         this._rtfFilterDetails.Size = new System.Drawing.Size(764, 310);
         this._rtfFilterDetails.TabIndex = 0;
         this._rtfFilterDetails.Text = "";
         this._rtfFilterDetails.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MouseMove_rtfFilterDetails);
         this._rtfFilterDetails.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MouseDown_rtfFilterDetails);
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
         this._splitter.Panel2.Controls.Add(this._gbFilterDescription);
         this._splitter.Size = new System.Drawing.Size(770, 670);
         this._splitter.SplitterDistance = 334;
         this._splitter.TabIndex = 2;
         // 
         // _grid
         // 
         this._toolbarsManager.SetContextMenuUltra(this._grid, "contextMenu");
         this._grid.DataMember = "EventFilters";
         this._grid.DataSource = this._dataSource;
         ultraGridColumn1.Header.Caption = "Filter";
         ultraGridColumn1.Header.VisiblePosition = 1;
         ultraGridColumn1.Width = 255;
         ultraGridColumn2.Header.VisiblePosition = 3;
         ultraGridColumn2.Width = 279;
         ultraGridColumn3.Header.Caption = "SQL Server";
         ultraGridColumn3.Header.VisiblePosition = 2;
         ultraGridColumn3.Width = 209;
         ultraGridColumn4.Header.Caption = "";
         ultraGridColumn4.Header.VisiblePosition = 0;
         ultraGridColumn4.LockedWidth = true;
         ultraGridColumn4.Width = 19;
         ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4});
         this._grid.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
         this._grid.Dock = System.Windows.Forms.DockStyle.Fill;
         this._grid.Location = new System.Drawing.Point(0, 0);
         this._grid.Name = "_grid";
         this._grid.Size = new System.Drawing.Size(770, 334);
         this._grid.TabIndex = 0;
         this._grid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MouseDown_grid);
         this._grid.AfterSelectChange += new Infragistics.Win.UltraWinGrid.AfterSelectChangeEventHandler(this.AfterSelectChange_grid);
         this._grid.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyDown_grid);
         this._grid.DoubleClickRow += new Infragistics.Win.UltraWinGrid.DoubleClickRowEventHandler(this.DoubleClickRow_grid);
         // 
         // _dataSource
         // 
         ultraDataColumn4.DataType = typeof(System.Drawing.Bitmap);
         this._dataSource.Band.Columns.AddRange(new object[] {
            ultraDataColumn1,
            ultraDataColumn2,
            ultraDataColumn3,
            ultraDataColumn4});
         this._dataSource.Band.Key = "EventFilters";
         // 
         // _toolbarsManager
         // 
         this._toolbarsManager.DesignerFlags = 1;
         this._toolbarsManager.DockWithinContainer = this;
         this._toolbarsManager.ShowFullMenusDelay = 500;
         buttonTool1.SharedProps.Caption = "New Event Filter...";
         buttonTool2.SharedProps.Caption = "Use as Filter Template...";
         buttonTool3.SharedProps.Caption = "Enable Event Filter";
         buttonTool4.SharedProps.Caption = "Disable Event Filter";
         buttonTool5.SharedProps.Caption = "Delete";
         buttonTool6.SharedProps.Caption = "Refresh";
         buttonTool7.SharedProps.Caption = "Properties...";
         popupMenuTool1.SharedProps.Caption = "contextMenu";
         buttonTool10.InstanceProps.IsFirstInGroup = true;
         buttonTool12.InstanceProps.IsFirstInGroup = true;
         buttonTool14.InstanceProps.IsFirstInGroup = true;
         popupMenuTool1.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool8,
            buttonTool9,
            buttonTool10,
            buttonTool11,
            buttonTool12,
            buttonTool13,
            buttonTool14});
         this._toolbarsManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool1,
            buttonTool2,
            buttonTool3,
            buttonTool4,
            buttonTool5,
            buttonTool6,
            buttonTool7,
            popupMenuTool1});
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
         // EventFiltersView
         // 
         this.Controls.Add(this._splitter);
         this.Controls.Add(this._BaseControl_Toolbars_Dock_Area_Left);
         this.Controls.Add(this._BaseControl_Toolbars_Dock_Area_Right);
         this.Controls.Add(this._BaseControl_Toolbars_Dock_Area_Top);
         this.Controls.Add(this._BaseControl_Toolbars_Dock_Area_Bottom);
         this.Name = "EventFiltersView";
         this.Size = new System.Drawing.Size(770, 670);
         this._gbFilterDescription.ResumeLayout(false);
         this._splitter.Panel1.ResumeLayout(false);
         this._splitter.Panel2.ResumeLayout(false);
         this._splitter.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this._grid)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this._dataSource)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this._toolbarsManager)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.GroupBox _gbFilterDescription;
      private System.Windows.Forms.RichTextBox _rtfFilterDetails;
      private System.ComponentModel.IContainer components;
      private System.Windows.Forms.SplitContainer _splitter;
      private Infragistics.Win.UltraWinGrid.UltraGrid _grid;
      private Infragistics.Win.UltraWinDataSource.UltraDataSource _dataSource;
      private Infragistics.Win.UltraWinToolbars.UltraToolbarsManager _toolbarsManager;
      private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseControl_Toolbars_Dock_Area_Left;
      private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseControl_Toolbars_Dock_Area_Right;
      private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseControl_Toolbars_Dock_Area_Top;
      private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseControl_Toolbars_Dock_Area_Bottom;
   }
}