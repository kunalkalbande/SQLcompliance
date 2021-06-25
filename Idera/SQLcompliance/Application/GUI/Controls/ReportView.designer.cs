namespace Idera.SQLcompliance.Application.GUI.Controls
{
   partial class ReportView
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
         this._pnlBorders = new Qios.DevSuite.Components.QPanel();
         this._reportViewer = new Microsoft.Reporting.WinForms.ReportViewer();
         this._pnlColors = new Qios.DevSuite.Components.QPanel();
         this._pnlParams = new System.Windows.Forms.TableLayoutPanel();
         this._pnlBorders.SuspendLayout();
         this._pnlColors.SuspendLayout();
         this.SuspendLayout();
         // 
         // _pnlBorders
         // 
         this._pnlBorders.Appearance.ShowBorderTop = false;
         this._pnlBorders.ColorScheme.PanelBorder.SetColor("Default", System.Drawing.Color.FromArgb(((int)(((byte)(101)))), ((int)(((byte)(147)))), ((int)(((byte)(207))))), false);
         this._pnlBorders.ColorScheme.PanelBorder.SetColor("LunaBlue", System.Drawing.Color.FromArgb(((int)(((byte)(101)))), ((int)(((byte)(147)))), ((int)(((byte)(207))))), false);
         this._pnlBorders.ColorScheme.PanelBorder.SetColor("LunaOlive", System.Drawing.Color.FromArgb(((int)(((byte)(101)))), ((int)(((byte)(147)))), ((int)(((byte)(207))))), false);
         this._pnlBorders.ColorScheme.PanelBorder.SetColor("LunaSilver", System.Drawing.Color.FromArgb(((int)(((byte)(101)))), ((int)(((byte)(147)))), ((int)(((byte)(207))))), false);
         this._pnlBorders.Controls.Add(this._reportViewer);
         this._pnlBorders.Controls.Add(this._pnlColors);
         this._pnlBorders.Dock = System.Windows.Forms.DockStyle.Fill;
         this._pnlBorders.Location = new System.Drawing.Point(0, 0);
         this._pnlBorders.Name = "_pnlBorders";
         this._pnlBorders.Size = new System.Drawing.Size(616, 546);
         this._pnlBorders.TabIndex = 0;
         // 
         // _reportViewer
         // 
         this._reportViewer.BorderStyle = System.Windows.Forms.BorderStyle.None;
         this._reportViewer.Dock = System.Windows.Forms.DockStyle.Fill;
         this._reportViewer.Location = new System.Drawing.Point(0, 52);
         this._reportViewer.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
         this._reportViewer.Name = "_reportViewer";
         this._reportViewer.ShowFindControls = false;
         this._reportViewer.Size = new System.Drawing.Size(614, 493);
         this._reportViewer.TabIndex = 6;
         // 
         // _pnlColors
         // 
         this._pnlColors.Appearance.GradientAngle = 90;
         this._pnlColors.Appearance.ShowBorders = false;
         this._pnlColors.ColorScheme.PanelBackground1.SetColor("Default", System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(219)))), ((int)(((byte)(224))))), false);
         this._pnlColors.ColorScheme.PanelBackground1.SetColor("LunaBlue", System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(239)))), ((int)(((byte)(255))))), false);
         this._pnlColors.ColorScheme.PanelBackground1.SetColor("LunaOlive", System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(239)))), ((int)(((byte)(255))))), false);
         this._pnlColors.ColorScheme.PanelBackground1.SetColor("LunaSilver", System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(239)))), ((int)(((byte)(255))))), false);
         this._pnlColors.ColorScheme.PanelBackground2.SetColor("Default", System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(187)))), ((int)(((byte)(197))))), false);
         this._pnlColors.ColorScheme.PanelBackground2.SetColor("LunaBlue", System.Drawing.Color.FromArgb(((int)(((byte)(177)))), ((int)(((byte)(211)))), ((int)(((byte)(255))))), false);
         this._pnlColors.ColorScheme.PanelBackground2.SetColor("LunaOlive", System.Drawing.Color.FromArgb(((int)(((byte)(177)))), ((int)(((byte)(211)))), ((int)(((byte)(255))))), false);
         this._pnlColors.ColorScheme.PanelBackground2.SetColor("LunaSilver", System.Drawing.Color.FromArgb(((int)(((byte)(177)))), ((int)(((byte)(211)))), ((int)(((byte)(255))))), false);
         this._pnlColors.ColorScheme.PanelBorder.SetColor("Default", System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(123)))), ((int)(((byte)(123))))), false);
         this._pnlColors.Controls.Add(this._pnlParams);
         this._pnlColors.Dock = System.Windows.Forms.DockStyle.Top;
         this._pnlColors.Location = new System.Drawing.Point(0, 0);
         this._pnlColors.MinimumClientSize = new System.Drawing.Size(0, 0);
         this._pnlColors.Name = "_pnlColors";
         this._pnlColors.Size = new System.Drawing.Size(614, 52);
         this._pnlColors.TabIndex = 7;
         // 
         // _pnlParams
         // 
         this._pnlParams.AutoSize = true;
         this._pnlParams.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
         this._pnlParams.BackColor = System.Drawing.Color.Transparent;
         this._pnlParams.ColumnCount = 8;
         this._pnlParams.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
         this._pnlParams.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
         this._pnlParams.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
         this._pnlParams.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
         this._pnlParams.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
         this._pnlParams.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
         this._pnlParams.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
         this._pnlParams.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
         this._pnlParams.Dock = System.Windows.Forms.DockStyle.Fill;
         this._pnlParams.Location = new System.Drawing.Point(0, 0);
         this._pnlParams.Name = "_pnlParams";
         this._pnlParams.RowCount = 1;
         this._pnlParams.RowStyles.Add(new System.Windows.Forms.RowStyle());
         this._pnlParams.Size = new System.Drawing.Size(614, 52);
         this._pnlParams.TabIndex = 6;
         // 
         // ReportView
         // 
         this.Controls.Add(this._pnlBorders);
         this.Name = "ReportView";
         this.Size = new System.Drawing.Size(616, 546);
         this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.ReportView_HelpRequested);
         this._pnlBorders.ResumeLayout(false);
         this._pnlColors.ResumeLayout(false);
         this._pnlColors.PerformLayout();
         this.ResumeLayout(false);

      }

      #endregion

      private Qios.DevSuite.Components.QPanel _pnlBorders;
      private Microsoft.Reporting.WinForms.ReportViewer _reportViewer;
      private Qios.DevSuite.Components.QPanel _pnlColors;
      private System.Windows.Forms.TableLayoutPanel _pnlParams;

   }
}
