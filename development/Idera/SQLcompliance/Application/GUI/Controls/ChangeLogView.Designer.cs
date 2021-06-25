namespace Idera.SQLcompliance.Application.GUI.Controls
{
   partial class ChangeLogView
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
         Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("ChangeLog", -1);
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Time");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Event");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("User");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("SQL Server");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Description");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Date");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn1 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Time");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn2 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Event");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn3 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("User");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn4 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("SQL Server");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn5 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Description");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn6 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Date");
         Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
         Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
         Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
         Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
         Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
         Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
         Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Infragistics.Win.UltraWinToolbars.ButtonTool("collapseAll");
         Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Infragistics.Win.UltraWinToolbars.ButtonTool("expandAll");
         Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool1 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("showGroupBy", "");
         Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool3 = new Infragistics.Win.UltraWinToolbars.ButtonTool("properties");
         Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool4 = new Infragistics.Win.UltraWinToolbars.ButtonTool("refresh");
         Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool1 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("Context Menu");
         Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool5 = new Infragistics.Win.UltraWinToolbars.ButtonTool("collapseAll");
         Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool6 = new Infragistics.Win.UltraWinToolbars.ButtonTool("expandAll");
         Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool2 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("showGroupBy", "");
         Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool7 = new Infragistics.Win.UltraWinToolbars.ButtonTool("refresh");
         Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool8 = new Infragistics.Win.UltraWinToolbars.ButtonTool("properties");
         this._grid = new Infragistics.Win.UltraWinGrid.UltraGrid();
         this._gridDataSource = new Infragistics.Win.UltraWinDataSource.UltraDataSource(this.components);
         this._lblStatus = new Infragistics.Win.Misc.UltraLabel();
         this._pnlNavigation = new Qios.DevSuite.Components.QPanel();
         this._lblNavigation = new Infragistics.Win.FormattedLinkLabel.UltraFormattedLinkLabel();
         this._btnFirst = new Infragistics.Win.Misc.UltraButton();
         this._btnNext = new Infragistics.Win.Misc.UltraButton();
         this._btnPrevious = new Infragistics.Win.Misc.UltraButton();
         this._btnLast = new Infragistics.Win.Misc.UltraButton();
         this._toolbarsManager = new Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(this.components);
         this._BaseControl_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
         this._BaseControl_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
         this._BaseControl_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
         this._BaseControl_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
         ((System.ComponentModel.ISupportInitialize)(this._grid)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this._gridDataSource)).BeginInit();
         this._pnlNavigation.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this._toolbarsManager)).BeginInit();
         this.SuspendLayout();
         // 
         // _grid
         // 
         this._toolbarsManager.SetContextMenuUltra(this._grid, "Context Menu");
         this._grid.DataSource = this._gridDataSource;
         ultraGridColumn1.Format = "";
         ultraGridColumn1.Header.VisiblePosition = 1;
         ultraGridColumn1.Width = 132;
         ultraGridColumn2.Header.VisiblePosition = 2;
         ultraGridColumn2.Width = 161;
         ultraGridColumn3.Header.VisiblePosition = 3;
         ultraGridColumn3.Width = 131;
         ultraGridColumn4.Header.VisiblePosition = 4;
         ultraGridColumn4.Width = 170;
         ultraGridColumn5.Header.VisiblePosition = 5;
         ultraGridColumn5.Width = 169;
         ultraGridColumn6.Header.VisiblePosition = 0;
         ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4,
            ultraGridColumn5,
            ultraGridColumn6});
         this._grid.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
         this._grid.Dock = System.Windows.Forms.DockStyle.Fill;
         this._grid.Location = new System.Drawing.Point(0, 0);
         this._grid.Name = "_grid";
         this._grid.Size = new System.Drawing.Size(760, 534);
         this._grid.TabIndex = 5;
         this._grid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MouseDown_grid);
         this._grid.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyDown_grid);
         this._grid.DoubleClickRow += new Infragistics.Win.UltraWinGrid.DoubleClickRowEventHandler(this.DoubleClickRow_grid);
         // 
         // _gridDataSource
         // 
         ultraDataColumn1.DataType = typeof(System.DateTime);
         ultraDataColumn6.DataType = typeof(System.DateTime);
         this._gridDataSource.Band.Columns.AddRange(new object[] {
            ultraDataColumn1,
            ultraDataColumn2,
            ultraDataColumn3,
            ultraDataColumn4,
            ultraDataColumn5,
            ultraDataColumn6});
         this._gridDataSource.Band.Key = "ChangeLog";
         // 
         // _lblStatus
         // 
         appearance1.BackColor = System.Drawing.SystemColors.Window;
         this._lblStatus.Appearance = appearance1;
         this._lblStatus.AutoSize = true;
         this._lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this._lblStatus.Location = new System.Drawing.Point(3, 80);
         this._lblStatus.Name = "_lblStatus";
         this._lblStatus.Size = new System.Drawing.Size(76, 17);
         this._lblStatus.TabIndex = 6;
         this._lblStatus.Text = "";
         // 
         // _pnlNavigation
         // 
         this._pnlNavigation.Appearance.GradientAngle = 90;
         this._pnlNavigation.Appearance.ShowBorders = false;
         this._pnlNavigation.ColorScheme.PanelBackground1.SetColor("Default", System.Drawing.SystemColors.Window, false);
         this._pnlNavigation.ColorScheme.PanelBackground1.SetColor("LunaBlue", System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254))))), false);
         this._pnlNavigation.ColorScheme.PanelBackground1.SetColor("LunaOlive", System.Drawing.SystemColors.Window, false);
         this._pnlNavigation.ColorScheme.PanelBackground1.SetColor("LunaSilver", System.Drawing.SystemColors.Window, false);
         this._pnlNavigation.ColorScheme.PanelBackground2.SetColor("Default", System.Drawing.SystemColors.Window, false);
         this._pnlNavigation.ColorScheme.PanelBackground2.SetColor("LunaBlue", System.Drawing.Color.FromArgb(((int)(((byte)(129)))), ((int)(((byte)(169)))), ((int)(((byte)(226))))), false);
         this._pnlNavigation.ColorScheme.PanelBackground2.SetColor("LunaOlive", System.Drawing.SystemColors.Window, false);
         this._pnlNavigation.ColorScheme.PanelBackground2.SetColor("LunaSilver", System.Drawing.SystemColors.Window, false);
         this._pnlNavigation.Controls.Add(this._lblNavigation);
         this._pnlNavigation.Controls.Add(this._btnFirst);
         this._pnlNavigation.Controls.Add(this._btnNext);
         this._pnlNavigation.Controls.Add(this._btnPrevious);
         this._pnlNavigation.Controls.Add(this._btnLast);
         this._pnlNavigation.Dock = System.Windows.Forms.DockStyle.Bottom;
         this._pnlNavigation.FontScope = Qios.DevSuite.Components.QFontScope.Local;
         this._pnlNavigation.LocalFont = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this._pnlNavigation.Location = new System.Drawing.Point(0, 534);
         this._pnlNavigation.MinimumClientSize = new System.Drawing.Size(0, 0);
         this._pnlNavigation.Name = "_pnlNavigation";
         this._pnlNavigation.Size = new System.Drawing.Size(760, 26);
         this._pnlNavigation.TabIndex = 17;
         this._pnlNavigation.Text = "qPanel1";
         // 
         // _lblNavigation
         // 
         this._lblNavigation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         appearance2.BackColor = System.Drawing.Color.Transparent;
         appearance2.BackColor2 = System.Drawing.Color.Transparent;
         appearance2.ForeColor = System.Drawing.Color.White;
         appearance2.TextVAlignAsString = "Middle";
         this._lblNavigation.Appearance = appearance2;
         this._lblNavigation.Location = new System.Drawing.Point(3, 2);
         this._lblNavigation.Name = "_lblNavigation";
         this._lblNavigation.Size = new System.Drawing.Size(541, 23);
         this._lblNavigation.TabIndex = 4;
         this._lblNavigation.TabStop = true;
         this._lblNavigation.Value = "ultraFormattedLinkLabel1";
         // 
         // _btnFirst
         // 
         this._btnFirst.AcceptsFocus = false;
         this._btnFirst.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         appearance3.Image = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.NavigateBeginning_16;
         this._btnFirst.Appearance = appearance3;
         this._btnFirst.AutoSize = true;
         this._btnFirst.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Office2003ToolbarButton;
         this._btnFirst.Location = new System.Drawing.Point(635, 2);
         this._btnFirst.Name = "_btnFirst";
         this._btnFirst.Size = new System.Drawing.Size(22, 22);
         this._btnFirst.TabIndex = 3;
         this._btnFirst.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
         this._btnFirst.Click += new System.EventHandler(this.Click_btnFirst);
         // 
         // _btnNext
         // 
         this._btnNext.AcceptsFocus = false;
         this._btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         appearance4.Image = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.NavigateRight_16;
         this._btnNext.Appearance = appearance4;
         this._btnNext.AutoSize = true;
         this._btnNext.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Office2003ToolbarButton;
         this._btnNext.Location = new System.Drawing.Point(699, 2);
         this._btnNext.Name = "_btnNext";
         this._btnNext.Size = new System.Drawing.Size(22, 22);
         this._btnNext.TabIndex = 1;
         this._btnNext.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
         this._btnNext.Click += new System.EventHandler(this.Click_btnNext);
         // 
         // _btnPrevious
         // 
         this._btnPrevious.AcceptsFocus = false;
         this._btnPrevious.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         appearance5.Image = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.NavigateLeft_16;
         this._btnPrevious.Appearance = appearance5;
         this._btnPrevious.AutoSize = true;
         this._btnPrevious.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Office2003ToolbarButton;
         this._btnPrevious.Location = new System.Drawing.Point(667, 2);
         this._btnPrevious.Name = "_btnPrevious";
         this._btnPrevious.Size = new System.Drawing.Size(22, 22);
         this._btnPrevious.TabIndex = 2;
         this._btnPrevious.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
         this._btnPrevious.Click += new System.EventHandler(this.Click_btnPrevious);
         // 
         // _btnLast
         // 
         this._btnLast.AcceptsFocus = false;
         this._btnLast.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         appearance6.Image = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.NavigateEnd_16;
         this._btnLast.Appearance = appearance6;
         this._btnLast.AutoSize = true;
         this._btnLast.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Office2003ToolbarButton;
         this._btnLast.Location = new System.Drawing.Point(731, 2);
         this._btnLast.Name = "_btnLast";
         this._btnLast.Size = new System.Drawing.Size(22, 22);
         this._btnLast.TabIndex = 0;
         this._btnLast.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
         this._btnLast.Click += new System.EventHandler(this.Click_btnLast);
         // 
         // _toolbarsManager
         // 
         this._toolbarsManager.DesignerFlags = 1;
         this._toolbarsManager.DockWithinContainer = this;
         this._toolbarsManager.ShowFullMenusDelay = 500;
         buttonTool1.SharedProps.Caption = "Collapse All";
         buttonTool2.SharedProps.Caption = "Expand All";
         stateButtonTool1.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
         stateButtonTool1.SharedProps.Caption = "Show Group By";
         buttonTool3.SharedProps.Caption = "Properties";
         buttonTool4.SharedProps.Caption = "Refresh";
         popupMenuTool1.SharedProps.Caption = "Context Menu";
         stateButtonTool2.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
         buttonTool7.InstanceProps.IsFirstInGroup = true;
         buttonTool8.InstanceProps.IsFirstInGroup = true;
         popupMenuTool1.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool5,
            buttonTool6,
            stateButtonTool2,
            buttonTool7,
            buttonTool8});
         this._toolbarsManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool1,
            buttonTool2,
            stateButtonTool1,
            buttonTool3,
            buttonTool4,
            popupMenuTool1});
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
         this._BaseControl_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(0, 560);
         this._BaseControl_Toolbars_Dock_Area_Left.ToolbarsManager = this._toolbarsManager;
         // 
         // _BaseControl_Toolbars_Dock_Area_Right
         // 
         this._BaseControl_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
         this._BaseControl_Toolbars_Dock_Area_Right.BackColor = System.Drawing.SystemColors.Control;
         this._BaseControl_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
         this._BaseControl_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
         this._BaseControl_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(760, 0);
         this._BaseControl_Toolbars_Dock_Area_Right.Name = "_BaseControl_Toolbars_Dock_Area_Right";
         this._BaseControl_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(0, 560);
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
         this._BaseControl_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(760, 0);
         this._BaseControl_Toolbars_Dock_Area_Top.ToolbarsManager = this._toolbarsManager;
         // 
         // _BaseControl_Toolbars_Dock_Area_Bottom
         // 
         this._BaseControl_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
         this._BaseControl_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.SystemColors.Control;
         this._BaseControl_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
         this._BaseControl_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
         this._BaseControl_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 560);
         this._BaseControl_Toolbars_Dock_Area_Bottom.Name = "_BaseControl_Toolbars_Dock_Area_Bottom";
         this._BaseControl_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(760, 0);
         this._BaseControl_Toolbars_Dock_Area_Bottom.ToolbarsManager = this._toolbarsManager;
         // 
         // ChangeLogView
         // 
         this.Controls.Add(this._lblStatus);
         this.Controls.Add(this._grid);
         this.Controls.Add(this._pnlNavigation);
         this.Controls.Add(this._BaseControl_Toolbars_Dock_Area_Left);
         this.Controls.Add(this._BaseControl_Toolbars_Dock_Area_Right);
         this.Controls.Add(this._BaseControl_Toolbars_Dock_Area_Top);
         this.Controls.Add(this._BaseControl_Toolbars_Dock_Area_Bottom);
         this.Name = "ChangeLogView";
         this.Size = new System.Drawing.Size(760, 560);
         this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.ChangeLogView_HelpRequested);
         ((System.ComponentModel.ISupportInitialize)(this._grid)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this._gridDataSource)).EndInit();
         this._pnlNavigation.ResumeLayout(false);
         this._pnlNavigation.PerformLayout();
         ((System.ComponentModel.ISupportInitialize)(this._toolbarsManager)).EndInit();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private Infragistics.Win.UltraWinGrid.UltraGrid _grid;
      private Infragistics.Win.UltraWinDataSource.UltraDataSource _gridDataSource;
      private Infragistics.Win.Misc.UltraLabel _lblStatus;
      private Qios.DevSuite.Components.QPanel _pnlNavigation;
      private Infragistics.Win.FormattedLinkLabel.UltraFormattedLinkLabel _lblNavigation;
      private Infragistics.Win.Misc.UltraButton _btnFirst;
      private Infragistics.Win.Misc.UltraButton _btnNext;
      private Infragistics.Win.Misc.UltraButton _btnPrevious;
      private Infragistics.Win.Misc.UltraButton _btnLast;
      private Infragistics.Win.UltraWinToolbars.UltraToolbarsManager _toolbarsManager;
      private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseControl_Toolbars_Dock_Area_Left;
      private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseControl_Toolbars_Dock_Area_Right;
      private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseControl_Toolbars_Dock_Area_Top;
      private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseControl_Toolbars_Dock_Area_Bottom;
   }
}
