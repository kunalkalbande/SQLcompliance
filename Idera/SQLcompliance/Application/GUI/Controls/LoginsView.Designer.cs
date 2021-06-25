namespace Idera.SQLcompliance.Application.GUI.Controls
{
   partial class LoginsView
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
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn1 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Icon");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn2 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Name");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn3 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Type");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn4 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("ServerAccess");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn5 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("WebApplicationAccess");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn6 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("CMAccess");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Logins", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Icon");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Name");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Type");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ServerAccess");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("WebApplicationAccess");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CMAccess");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Infragistics.Win.UltraWinToolbars.ButtonTool("newLogin");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Infragistics.Win.UltraWinToolbars.ButtonTool("delete");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool3 = new Infragistics.Win.UltraWinToolbars.ButtonTool("refresh");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool4 = new Infragistics.Win.UltraWinToolbars.ButtonTool("properties");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool1 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("contextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool5 = new Infragistics.Win.UltraWinToolbars.ButtonTool("newLogin");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool6 = new Infragistics.Win.UltraWinToolbars.ButtonTool("delete");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool7 = new Infragistics.Win.UltraWinToolbars.ButtonTool("refresh");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool8 = new Infragistics.Win.UltraWinToolbars.ButtonTool("properties");
            this._dsLogins = new Infragistics.Win.UltraWinDataSource.UltraDataSource(this.components);
            this._grid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this._toolbarsManager = new Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(this.components);
            this._BaseControl_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._BaseControl_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._BaseControl_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._BaseControl_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            ((System.ComponentModel.ISupportInitialize)(this._dsLogins)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._grid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._toolbarsManager)).BeginInit();
            this.SuspendLayout();
            // 
            // _dsLogins
            // 
            ultraDataColumn1.DataType = typeof(System.Drawing.Bitmap);
            ultraDataColumn5.DataType = typeof(bool);
            ultraDataColumn5.ReadOnly = Infragistics.Win.DefaultableBoolean.False;
            this._dsLogins.Band.Columns.AddRange(new object[] {
            ultraDataColumn1,
            ultraDataColumn2,
            ultraDataColumn3,
            ultraDataColumn4,
            ultraDataColumn5,
            ultraDataColumn6});
            this._dsLogins.Band.Key = "Logins";
            // 
            // _grid
            // 
            this._toolbarsManager.SetContextMenuUltra(this._grid, "contextMenu");
            this._grid.DataMember = "Logins";
            this._grid.DataSource = this._dsLogins;
            ultraGridColumn1.Header.Caption = "";
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn1.LockedWidth = true;
            ultraGridColumn1.Width = 22;
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn2.Width = 186;
            ultraGridColumn3.Header.VisiblePosition = 2;
            ultraGridColumn3.Width = 123;
            ultraGridColumn4.Header.Caption = "Server Access";
            ultraGridColumn4.Header.VisiblePosition = 3;
            ultraGridColumn4.Width = 127;
            ultraGridColumn6.Header.Caption = "Web Application Access";
            ultraGridColumn6.Header.VisiblePosition = 4;
            ultraGridColumn6.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
            ultraGridColumn5.Header.Caption = "Permissions in SQL Compliance Manager";
            ultraGridColumn5.Header.VisiblePosition = 5;
            ultraGridColumn5.Width = 179;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4,
            ultraGridColumn6,
            ultraGridColumn5});
            this._grid.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this._grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._grid.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._grid.Location = new System.Drawing.Point(0, 0);
            this._grid.Name = "_grid";
            this._grid.Size = new System.Drawing.Size(770, 489);
            this._grid.TabIndex = 1;
            this._grid.Text = "ultraGrid1";
            this._grid.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            this._grid.AfterSelectChange += new Infragistics.Win.UltraWinGrid.AfterSelectChangeEventHandler(this.AfterSelectChange_grid);
            this._grid.ClickCell += new Infragistics.Win.UltraWinGrid.ClickCellEventHandler(this._grid_ClickCell);
            this._grid.DoubleClickRow += new Infragistics.Win.UltraWinGrid.DoubleClickRowEventHandler(this.DoubleClickRow_grid);
            this._grid.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyDown_grid);
            this._grid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MouseDown_grid);
            // 
            // _toolbarsManager
            // 
            this._toolbarsManager.DesignerFlags = 1;
            this._toolbarsManager.DockWithinContainer = this;
            this._toolbarsManager.ShowFullMenusDelay = 500;
            buttonTool1.SharedPropsInternal.Caption = "New SQL Server Login...";
            buttonTool2.SharedPropsInternal.Caption = "Delete";
            buttonTool3.SharedPropsInternal.Caption = "Refresh";
            buttonTool4.SharedPropsInternal.Caption = "Properties";
            popupMenuTool1.SharedPropsInternal.Caption = "contextMenu";
            buttonTool6.InstanceProps.IsFirstInGroup = true;
            buttonTool8.InstanceProps.IsFirstInGroup = true;
            popupMenuTool1.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool5,
            buttonTool6,
            buttonTool7,
            buttonTool8});
            this._toolbarsManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool1,
            buttonTool2,
            buttonTool3,
            buttonTool4,
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
            this._BaseControl_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(0, 489);
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
            this._BaseControl_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(0, 489);
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
            this._BaseControl_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 489);
            this._BaseControl_Toolbars_Dock_Area_Bottom.Name = "_BaseControl_Toolbars_Dock_Area_Bottom";
            this._BaseControl_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(770, 0);
            this._BaseControl_Toolbars_Dock_Area_Bottom.ToolbarsManager = this._toolbarsManager;
            // 
            // LoginsView
            // 
            this.Controls.Add(this._grid);
            this.Controls.Add(this._BaseControl_Toolbars_Dock_Area_Left);
            this.Controls.Add(this._BaseControl_Toolbars_Dock_Area_Right);
            this.Controls.Add(this._BaseControl_Toolbars_Dock_Area_Bottom);
            this.Controls.Add(this._BaseControl_Toolbars_Dock_Area_Top);
            this.Name = "LoginsView";
            this.Size = new System.Drawing.Size(770, 489);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.LoginsView_HelpRequested);
            ((System.ComponentModel.ISupportInitialize)(this._dsLogins)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._grid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._toolbarsManager)).EndInit();
            this.ResumeLayout(false);

      }
      #endregion

      private System.ComponentModel.IContainer components;
      private Infragistics.Win.UltraWinDataSource.UltraDataSource _dsLogins;
      private Infragistics.Win.UltraWinGrid.UltraGrid _grid;
      private Infragistics.Win.UltraWinToolbars.UltraToolbarsManager _toolbarsManager;
      private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseControl_Toolbars_Dock_Area_Left;
      private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseControl_Toolbars_Dock_Area_Right;
      private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseControl_Toolbars_Dock_Area_Top;
      private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseControl_Toolbars_Dock_Area_Bottom;
   }
}