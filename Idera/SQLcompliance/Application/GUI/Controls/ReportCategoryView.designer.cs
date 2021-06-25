namespace Idera.SQLcompliance.Application.GUI.Controls
{
   partial class ReportCategoryView
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
         this._tblReports = new System.Windows.Forms.FlowLayoutPanel();
         this._pnlDeployReports = new System.Windows.Forms.Panel();
         this._btnViewDeployedReports = new Idera.SQLcompliance.Application.GUI.Controls.FeatureButton();
         this._btnDeployReports = new Idera.SQLcompliance.Application.GUI.Controls.FeatureButton();
         this._headerReportingServices = new Qios.DevSuite.Components.QPanel();
         this._lblReportingServices = new System.Windows.Forms.Label();
         this._pnlBorders.SuspendLayout();
         this._pnlDeployReports.SuspendLayout();
         this._headerReportingServices.SuspendLayout();
         this.SuspendLayout();
         // 
         // _pnlBorders
         // 
         this._pnlBorders.Appearance.ShowBorderTop = false;
         this._pnlBorders.ColorScheme.PanelBorder.SetColor("Default", System.Drawing.Color.FromArgb(((int)(((byte)(101)))), ((int)(((byte)(147)))), ((int)(((byte)(207))))), false);
         this._pnlBorders.ColorScheme.PanelBorder.SetColor("LunaBlue", System.Drawing.Color.FromArgb(((int)(((byte)(101)))), ((int)(((byte)(147)))), ((int)(((byte)(207))))), false);
         this._pnlBorders.ColorScheme.PanelBorder.SetColor("LunaOlive", System.Drawing.Color.FromArgb(((int)(((byte)(101)))), ((int)(((byte)(147)))), ((int)(((byte)(207))))), false);
         this._pnlBorders.ColorScheme.PanelBorder.SetColor("LunaSilver", System.Drawing.Color.FromArgb(((int)(((byte)(101)))), ((int)(((byte)(147)))), ((int)(((byte)(207))))), false);
         this._pnlBorders.Controls.Add(this._tblReports);
         this._pnlBorders.Controls.Add(this._pnlDeployReports);
         this._pnlBorders.Dock = System.Windows.Forms.DockStyle.Fill;
         this._pnlBorders.Location = new System.Drawing.Point(0, 0);
         this._pnlBorders.Name = "_pnlBorders";
         this._pnlBorders.Size = new System.Drawing.Size(770, 670);
         this._pnlBorders.TabIndex = 1;
         this._pnlBorders.Text = "qPanel1";
         // 
         // _tblReports
         // 
         this._tblReports.AutoScroll = true;
         this._tblReports.BackColor = System.Drawing.Color.White;
         this._tblReports.Dock = System.Windows.Forms.DockStyle.Fill;
         this._tblReports.Location = new System.Drawing.Point(0, 0);
         this._tblReports.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
         this._tblReports.Name = "_tblReports";
         this._tblReports.Size = new System.Drawing.Size(768, 571);
         this._tblReports.TabIndex = 1;
         // 
         // _pnlDeployReports
         // 
         this._pnlDeployReports.Controls.Add(this._btnViewDeployedReports);
         this._pnlDeployReports.Controls.Add(this._btnDeployReports);
         this._pnlDeployReports.Controls.Add(this._headerReportingServices);
         this._pnlDeployReports.Dock = System.Windows.Forms.DockStyle.Bottom;
         this._pnlDeployReports.Location = new System.Drawing.Point(0, 571);
         this._pnlDeployReports.Name = "_pnlDeployReports";
         this._pnlDeployReports.Size = new System.Drawing.Size(768, 98);
         this._pnlDeployReports.TabIndex = 2;
         // 
         // _btnViewDeployedReports
         // 
         this._btnViewDeployedReports.DescriptionText = "View deployed reports hosted by Reporting Services 2005 or later.";
         this._btnViewDeployedReports.HeaderColor = System.Drawing.Color.Red;
         this._btnViewDeployedReports.HeaderText = "View Deployed Reports";
         this._btnViewDeployedReports.Image = null;
         this._btnViewDeployedReports.Location = new System.Drawing.Point(294, 29);
         this._btnViewDeployedReports.MinimumSize = new System.Drawing.Size(0, 40);
         this._btnViewDeployedReports.Name = "_btnViewDeployedReports";
         this._btnViewDeployedReports.Size = new System.Drawing.Size(285, 65);
         this._btnViewDeployedReports.TabIndex = 52;
         this._btnViewDeployedReports.MouseClick += new System.Windows.Forms.MouseEventHandler(this.MouseClick_btnViewDeployedReports);
         // 
         // _btnDeployReports
         // 
         this._btnDeployReports.DescriptionText = "Deploy reports and linked reports to Reporting Services 2005 or later.";
         this._btnDeployReports.HeaderColor = System.Drawing.Color.Red;
         this._btnDeployReports.HeaderText = "Deploy Reports";
         this._btnDeployReports.Image = null;
         this._btnDeployReports.Location = new System.Drawing.Point(3, 29);
         this._btnDeployReports.MinimumSize = new System.Drawing.Size(0, 40);
         this._btnDeployReports.Name = "_btnDeployReports";
         this._btnDeployReports.Size = new System.Drawing.Size(285, 65);
         this._btnDeployReports.TabIndex = 51;
         this._btnDeployReports.MouseClick += new System.Windows.Forms.MouseEventHandler(this.MouseClick_btnDeployReports);
         // 
         // _headerReportingServices
         // 
         this._headerReportingServices.Appearance.GradientAngle = 90;
         this._headerReportingServices.Appearance.ShowBorderLeft = false;
         this._headerReportingServices.Appearance.ShowBorderRight = false;
         this._headerReportingServices.ColorScheme.PanelBackground1.SetColor("Default", System.Drawing.SystemColors.ActiveCaption, false);
         this._headerReportingServices.ColorScheme.PanelBackground1.SetColor("LunaBlue", System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(254))))), false);
         this._headerReportingServices.ColorScheme.PanelBackground1.SetColor("LunaOlive", System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(247)))), ((int)(((byte)(222))))), false);
         this._headerReportingServices.ColorScheme.PanelBackground1.SetColor("LunaSilver", System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(250))))), false);
         this._headerReportingServices.ColorScheme.PanelBackground2.SetColor("Default", System.Drawing.SystemColors.ActiveCaption, false);
         this._headerReportingServices.ColorScheme.PanelBackground2.SetColor("LunaBlue", System.Drawing.Color.FromArgb(((int)(((byte)(129)))), ((int)(((byte)(169)))), ((int)(((byte)(226))))), false);
         this._headerReportingServices.ColorScheme.PanelBackground2.SetColor("LunaOlive", System.Drawing.Color.FromArgb(((int)(((byte)(183)))), ((int)(((byte)(198)))), ((int)(((byte)(145))))), false);
         this._headerReportingServices.ColorScheme.PanelBackground2.SetColor("LunaSilver", System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(151)))), ((int)(((byte)(181))))), false);
         this._headerReportingServices.ColorScheme.PanelBorder.SetColor("LunaBlue", System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(45)))), ((int)(((byte)(150))))), false);
         this._headerReportingServices.ColorScheme.PanelBorder.SetColor("LunaOlive", System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(128)))), ((int)(((byte)(88))))), false);
         this._headerReportingServices.ColorScheme.PanelBorder.SetColor("LunaSilver", System.Drawing.Color.FromArgb(((int)(((byte)(124)))), ((int)(((byte)(124)))), ((int)(((byte)(148))))), false);
         this._headerReportingServices.Controls.Add(this._lblReportingServices);
         this._headerReportingServices.Dock = System.Windows.Forms.DockStyle.Top;
         this._headerReportingServices.FontScope = Qios.DevSuite.Components.QFontScope.Local;
         this._headerReportingServices.LocalFont = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this._headerReportingServices.Location = new System.Drawing.Point(0, 0);
         this._headerReportingServices.MinimumClientSize = new System.Drawing.Size(10, 10);
         this._headerReportingServices.Name = "_headerReportingServices";
         this._headerReportingServices.Size = new System.Drawing.Size(768, 27);
         this._headerReportingServices.TabIndex = 50;
         this._headerReportingServices.Text = "qPanel4";
         // 
         // _lblReportingServices
         // 
         this._lblReportingServices.BackColor = System.Drawing.Color.Transparent;
         this._lblReportingServices.Dock = System.Windows.Forms.DockStyle.Fill;
         this._lblReportingServices.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this._lblReportingServices.Location = new System.Drawing.Point(0, 0);
         this._lblReportingServices.Name = "_lblReportingServices";
         this._lblReportingServices.Size = new System.Drawing.Size(768, 25);
         this._lblReportingServices.TabIndex = 0;
         this._lblReportingServices.Text = "Reporting Services";
         this._lblReportingServices.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // ReportCategoryView
         // 
         this.Controls.Add(this._pnlBorders);
         this.Name = "ReportCategoryView";
         this.Size = new System.Drawing.Size(770, 670);
         this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.ReportCategoryView_HelpRequested);
         this._pnlBorders.ResumeLayout(false);
         this._pnlDeployReports.ResumeLayout(false);
         this._headerReportingServices.ResumeLayout(false);
         this.ResumeLayout(false);

      }

      #endregion

      private Qios.DevSuite.Components.QPanel _pnlBorders;
      private System.Windows.Forms.FlowLayoutPanel _tblReports;
      private System.Windows.Forms.Panel _pnlDeployReports;
      private Qios.DevSuite.Components.QPanel _headerReportingServices;
      private System.Windows.Forms.Label _lblReportingServices;
      private FeatureButton _btnViewDeployedReports;
      private FeatureButton _btnDeployReports;

   }
}
