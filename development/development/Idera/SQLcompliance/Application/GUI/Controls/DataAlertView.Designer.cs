namespace Idera.SQLcompliance.Application.GUI.Controls
{
   partial class DataAlertView
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
         Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
         Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
         Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Alerts", -1);
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("instance");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("eventTypeString");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Date", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Descending, false);
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("alertLevelString");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("messageSubject");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("messageBody");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ruleName");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("icon");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn9 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Time");
         Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn1 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("instance");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn2 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("eventTypeString");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn3 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Date");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn4 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("alertLevelString");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn5 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("messageSubject");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn6 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("messageBody");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn7 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("ruleName");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn8 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("icon");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn9 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Time");
         Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
         Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
         Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
         Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
         this._lnkWarning = new System.Windows.Forms.LinkLabel();
         this._lblStatus = new Infragistics.Win.Misc.UltraLabel();
         this._btnFirst = new Infragistics.Win.Misc.UltraButton();
         this._grid = new Infragistics.Win.UltraWinGrid.UltraGrid();
         this._dsAlerts = new Infragistics.Win.UltraWinDataSource.UltraDataSource(this.components);
         this._btnPrevious = new Infragistics.Win.Misc.UltraButton();
         this._btnNext = new Infragistics.Win.Misc.UltraButton();
         this._pnlWarning = new System.Windows.Forms.Panel();
         this._pbWarning = new System.Windows.Forms.PictureBox();
         this._lblWaiting = new System.Windows.Forms.Label();
         this._pnlNavigation = new Qios.DevSuite.Components.QPanel();
         this._lblNavigation = new Infragistics.Win.FormattedLinkLabel.UltraFormattedLinkLabel();
         this._btnLast = new Infragistics.Win.Misc.UltraButton();
         this._pbWaiting = new System.Windows.Forms.PictureBox();
         ((System.ComponentModel.ISupportInitialize)(this._grid)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this._dsAlerts)).BeginInit();
         this._pnlWarning.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this._pbWarning)).BeginInit();
         this._pnlNavigation.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this._pbWaiting)).BeginInit();
         this.SuspendLayout();
         // 
         // _lnkWarning
         // 
         this._lnkWarning.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
         this._lnkWarning.Location = new System.Drawing.Point(46, 13);
         this._lnkWarning.Name = "_lnkWarning";
         this._lnkWarning.Size = new System.Drawing.Size(560, 16);
         this._lnkWarning.TabIndex = 4;
         this._lnkWarning.TabStop = true;
         this._lnkWarning.Text = "The SQLcompliance database needs an index update.";
         this._lnkWarning.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkClicked_lnkWarning);
         // 
         // _lblStatus
         // 
         appearance1.BackColor = System.Drawing.SystemColors.Window;
         this._lblStatus.Appearance = appearance1;
         this._lblStatus.AutoSize = true;
         this._lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this._lblStatus.Location = new System.Drawing.Point(3, 260);
         this._lblStatus.Name = "_lblStatus";
         this._lblStatus.Size = new System.Drawing.Size(76, 17);
         this._lblStatus.TabIndex = 12;
         this._lblStatus.Text = "";
         // 
         // _btnFirst
         // 
         this._btnFirst.AcceptsFocus = false;
         this._btnFirst.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         appearance2.Image = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.NavigateBeginning_16;
         this._btnFirst.Appearance = appearance2;
         this._btnFirst.AutoSize = true;
         this._btnFirst.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Office2003ToolbarButton;
         this._btnFirst.Location = new System.Drawing.Point(635, 2);
         this._btnFirst.Name = "_btnFirst";
         this._btnFirst.Size = new System.Drawing.Size(22, 22);
         this._btnFirst.TabIndex = 3;
         this._btnFirst.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
         this._btnFirst.Click += new System.EventHandler(this.Click_btnFirst);
         // 
         // _grid
         // 
         this._grid.DataMember = "Alerts";
         this._grid.DataSource = this._dsAlerts;
         ultraGridColumn1.Header.Caption = "SQL Server";
         ultraGridColumn1.Header.VisiblePosition = 6;
         ultraGridColumn1.Width = 158;
         ultraGridColumn2.Header.Caption = "Event";
         ultraGridColumn2.Header.VisiblePosition = 5;
         ultraGridColumn2.Width = 140;
         ultraGridColumn3.Format = "";
         ultraGridColumn3.Header.VisiblePosition = 1;
         ultraGridColumn3.Width = 88;
         ultraGridColumn4.Header.Caption = "Level";
         ultraGridColumn4.Header.VisiblePosition = 3;
         ultraGridColumn4.Width = 55;
         ultraGridColumn5.Header.Caption = "Subject";
         ultraGridColumn5.Header.VisiblePosition = 7;
         ultraGridColumn5.Hidden = true;
         ultraGridColumn6.Header.Caption = "Details";
         ultraGridColumn6.Header.VisiblePosition = 8;
         ultraGridColumn6.Hidden = true;
         ultraGridColumn7.Header.Caption = "Source Rule";
         ultraGridColumn7.Header.VisiblePosition = 4;
         ultraGridColumn7.Width = 157;
         ultraGridColumn8.ColumnChooserCaption = "Icon";
         ultraGridColumn8.Header.Caption = "";
         ultraGridColumn8.Header.VisiblePosition = 0;
         ultraGridColumn8.LockedWidth = true;
         ultraGridColumn8.Width = 23;
         ultraGridColumn9.Header.VisiblePosition = 2;
         ultraGridColumn9.Width = 96;
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
         appearance3.ForeColor = System.Drawing.Color.White;
         this._grid.DisplayLayout.GroupByBox.Appearance = appearance3;
         this._grid.Dock = System.Windows.Forms.DockStyle.Fill;
         this._grid.Location = new System.Drawing.Point(0, 42);
         this._grid.Name = "_grid";
         this._grid.Size = new System.Drawing.Size(760, 492);
         this._grid.TabIndex = 11;
         this._grid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MouseDown_grid);
         this._grid.AfterSelectChange += new Infragistics.Win.UltraWinGrid.AfterSelectChangeEventHandler(this.AfterSelectChange_grid);
         this._grid.Enter += new System.EventHandler(this.FocusChanged_grid);
         this._grid.Leave += new System.EventHandler(this.FocusChanged_grid);
         this._grid.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyDown_grid);
         this._grid.DoubleClickRow += new Infragistics.Win.UltraWinGrid.DoubleClickRowEventHandler(this.DoubleClickRow_grid);
         // 
         // _dsAlerts
         // 
         ultraDataColumn3.DataType = typeof(System.DateTime);
         ultraDataColumn8.DataType = typeof(System.Drawing.Bitmap);
         ultraDataColumn9.DataType = typeof(System.DateTime);
         this._dsAlerts.Band.Columns.AddRange(new object[] {
            ultraDataColumn1,
            ultraDataColumn2,
            ultraDataColumn3,
            ultraDataColumn4,
            ultraDataColumn5,
            ultraDataColumn6,
            ultraDataColumn7,
            ultraDataColumn8,
            ultraDataColumn9});
         this._dsAlerts.Band.Key = "Alerts";
         // 
         // _btnPrevious
         // 
         this._btnPrevious.AcceptsFocus = false;
         this._btnPrevious.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         appearance4.Image = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.NavigateLeft_16;
         this._btnPrevious.Appearance = appearance4;
         this._btnPrevious.AutoSize = true;
         this._btnPrevious.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Office2003ToolbarButton;
         this._btnPrevious.Location = new System.Drawing.Point(667, 2);
         this._btnPrevious.Name = "_btnPrevious";
         this._btnPrevious.Size = new System.Drawing.Size(22, 22);
         this._btnPrevious.TabIndex = 2;
         this._btnPrevious.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
         this._btnPrevious.Click += new System.EventHandler(this.Click_btnPrevious);
         // 
         // _btnNext
         // 
         this._btnNext.AcceptsFocus = false;
         this._btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         appearance5.Image = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.NavigateRight_16;
         this._btnNext.Appearance = appearance5;
         this._btnNext.AutoSize = true;
         this._btnNext.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Office2003ToolbarButton;
         this._btnNext.Location = new System.Drawing.Point(699, 2);
         this._btnNext.Name = "_btnNext";
         this._btnNext.Size = new System.Drawing.Size(22, 22);
         this._btnNext.TabIndex = 1;
         this._btnNext.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
         this._btnNext.Click += new System.EventHandler(this.Click_btnShowNextAlerts);
         // 
         // _pnlWarning
         // 
         this._pnlWarning.BackColor = System.Drawing.Color.White;
         this._pnlWarning.Controls.Add(this._lnkWarning);
         this._pnlWarning.Controls.Add(this._pbWarning);
         this._pnlWarning.Dock = System.Windows.Forms.DockStyle.Top;
         this._pnlWarning.Location = new System.Drawing.Point(0, 0);
         this._pnlWarning.Margin = new System.Windows.Forms.Padding(0);
         this._pnlWarning.Name = "_pnlWarning";
         this._pnlWarning.Size = new System.Drawing.Size(760, 42);
         this._pnlWarning.TabIndex = 10;
         this._pnlWarning.Visible = false;
         // 
         // _pbWarning
         // 
         this._pbWarning.Image = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.StatusWarning_32;
         this._pbWarning.Location = new System.Drawing.Point(8, 5);
         this._pbWarning.Name = "_pbWarning";
         this._pbWarning.Size = new System.Drawing.Size(32, 32);
         this._pbWarning.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
         this._pbWarning.TabIndex = 3;
         this._pbWarning.TabStop = false;
         // 
         // _lblWaiting
         // 
         this._lblWaiting.AutoSize = true;
         this._lblWaiting.BackColor = System.Drawing.SystemColors.Window;
         this._lblWaiting.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this._lblWaiting.Location = new System.Drawing.Point(264, 310);
         this._lblWaiting.Name = "_lblWaiting";
         this._lblWaiting.Size = new System.Drawing.Size(140, 20);
         this._lblWaiting.TabIndex = 14;
         this._lblWaiting.Text = "Loading Alerts...";
         this._lblWaiting.Visible = false;
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
         this._pnlNavigation.TabIndex = 16;
         this._pnlNavigation.Text = "qPanel1";
         // 
         // _lblNavigation
         // 
         this._lblNavigation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         appearance6.BackColor = System.Drawing.Color.Transparent;
         appearance6.BackColor2 = System.Drawing.Color.Transparent;
         appearance6.ForeColor = System.Drawing.Color.White;
         appearance6.TextVAlignAsString = "Middle";
         this._lblNavigation.Appearance = appearance6;
         this._lblNavigation.Location = new System.Drawing.Point(3, 2);
         this._lblNavigation.Name = "_lblNavigation";
         this._lblNavigation.Size = new System.Drawing.Size(541, 23);
         this._lblNavigation.TabIndex = 4;
         this._lblNavigation.TabStop = true;
         this._lblNavigation.Value = "ultraFormattedLinkLabel1";
         // 
         // _btnLast
         // 
         this._btnLast.AcceptsFocus = false;
         this._btnLast.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         appearance7.Image = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.NavigateEnd_16;
         this._btnLast.Appearance = appearance7;
         this._btnLast.AutoSize = true;
         this._btnLast.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Office2003ToolbarButton;
         this._btnLast.Location = new System.Drawing.Point(731, 2);
         this._btnLast.Name = "_btnLast";
         this._btnLast.Size = new System.Drawing.Size(22, 22);
         this._btnLast.TabIndex = 0;
         this._btnLast.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
         this._btnLast.Click += new System.EventHandler(this.Click_btnLast);
         // 
         // _pbWaiting
         // 
         this._pbWaiting.BackColor = System.Drawing.SystemColors.Window;
         this._pbWaiting.Image = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.Refresh_48;
         this._pbWaiting.Location = new System.Drawing.Point(305, 259);
         this._pbWaiting.Name = "_pbWaiting";
         this._pbWaiting.Size = new System.Drawing.Size(48, 48);
         this._pbWaiting.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
         this._pbWaiting.TabIndex = 13;
         this._pbWaiting.TabStop = false;
         this._pbWaiting.Visible = false;
         // 
         // DataAlertView
         // 
         this.Controls.Add(this._lblStatus);
         this.Controls.Add(this._pbWaiting);
         this.Controls.Add(this._lblWaiting);
         this.Controls.Add(this._grid);
         this.Controls.Add(this._pnlNavigation);
         this.Controls.Add(this._pnlWarning);
         this.Name = "DataAlertView";
         this.Size = new System.Drawing.Size(760, 560);
         ((System.ComponentModel.ISupportInitialize)(this._grid)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this._dsAlerts)).EndInit();
         this._pnlWarning.ResumeLayout(false);
         this._pnlWarning.PerformLayout();
         ((System.ComponentModel.ISupportInitialize)(this._pbWarning)).EndInit();
         this._pnlNavigation.ResumeLayout(false);
         this._pnlNavigation.PerformLayout();
         ((System.ComponentModel.ISupportInitialize)(this._pbWaiting)).EndInit();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.LinkLabel _lnkWarning;
      private Infragistics.Win.Misc.UltraLabel _lblStatus;
      private Infragistics.Win.Misc.UltraButton _btnFirst;
      private Infragistics.Win.UltraWinGrid.UltraGrid _grid;
      private System.Windows.Forms.PictureBox _pbWarning;
      private Infragistics.Win.Misc.UltraButton _btnPrevious;
      private Infragistics.Win.Misc.UltraButton _btnNext;
      private Infragistics.Win.Misc.UltraButton _btnLast;
      private System.Windows.Forms.Panel _pnlWarning;
      private Infragistics.Win.UltraWinDataSource.UltraDataSource _dsAlerts;
      private System.Windows.Forms.PictureBox _pbWaiting;
      private System.Windows.Forms.Label _lblWaiting;
      private Qios.DevSuite.Components.QPanel _pnlNavigation;
      private Infragistics.Win.FormattedLinkLabel.UltraFormattedLinkLabel _lblNavigation;
   }
}
