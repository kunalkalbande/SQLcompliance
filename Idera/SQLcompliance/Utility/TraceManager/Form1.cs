using System ;
using System.Collections.Generic;
using System.ComponentModel ;
using System.Data ;
using System.Data.SqlClient ;
using System.Text ;
using System.Windows.Forms ;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;

namespace Idera.SQLcompliance.Utility.TraceManager
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : Form
	{
      private SqlConnection _connection = null;

      private MainMenu mainMenu1;
      private MenuItem menuItem1;
      private MenuItem menuExit;
      private Label label1;
      private Button btnConnect;
      private GroupBox groupBox1;
      private Button btnStart;
      private Button btnStop;
      private Button btnClose;
      private GroupBox groupBox2;
      private MenuItem menuRefresh;
      private MenuItem menuItem4;
      private CheckBox chkAuditStoredProc;
      private CheckBox chkStartupStoredProc;
      private Button btnDrop;
      private TextBox textServer;
      private Button btnDetails;
      private MenuItem menuCopyTraces;
      private MenuItem menuItem2;
      private MenuItem menuItem333;
      private MenuItem menuItem3;
      private MenuItem menuUncompress;
      private Button btnClearAll;
      private MenuItem menuItem5;
      private MenuItem menuAbout;
      private Infragistics.Win.UltraWinDataSource.UltraDataSource _dsTraceData;
      private Panel panel1;
      private Infragistics.Win.UltraWinGrid.UltraGrid _gridTraces;
      private IContainer components;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
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
         Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("TraceData", -1);
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("traceid");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("property");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("value");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("propertyname");
         Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
         Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn1 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("traceid");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn2 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("property");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn3 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("value");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn4 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("propertyname");
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
         this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
         this.menuItem1 = new System.Windows.Forms.MenuItem();
         this.menuRefresh = new System.Windows.Forms.MenuItem();
         this.menuItem4 = new System.Windows.Forms.MenuItem();
         this.menuExit = new System.Windows.Forms.MenuItem();
         this.menuItem2 = new System.Windows.Forms.MenuItem();
         this.menuCopyTraces = new System.Windows.Forms.MenuItem();
         this.menuItem3 = new System.Windows.Forms.MenuItem();
         this.menuUncompress = new System.Windows.Forms.MenuItem();
         this.menuItem5 = new System.Windows.Forms.MenuItem();
         this.menuAbout = new System.Windows.Forms.MenuItem();
         this.label1 = new System.Windows.Forms.Label();
         this.textServer = new System.Windows.Forms.TextBox();
         this.btnConnect = new System.Windows.Forms.Button();
         this.groupBox1 = new System.Windows.Forms.GroupBox();
         this.panel1 = new System.Windows.Forms.Panel();
         this.btnClearAll = new System.Windows.Forms.Button();
         this.btnStart = new System.Windows.Forms.Button();
         this.btnStop = new System.Windows.Forms.Button();
         this.btnDetails = new System.Windows.Forms.Button();
         this.btnClose = new System.Windows.Forms.Button();
         this._gridTraces = new Infragistics.Win.UltraWinGrid.UltraGrid();
         this._dsTraceData = new Infragistics.Win.UltraWinDataSource.UltraDataSource(this.components);
         this.groupBox2 = new System.Windows.Forms.GroupBox();
         this.btnDrop = new System.Windows.Forms.Button();
         this.chkStartupStoredProc = new System.Windows.Forms.CheckBox();
         this.chkAuditStoredProc = new System.Windows.Forms.CheckBox();
         this.menuItem333 = new System.Windows.Forms.MenuItem();
         this.groupBox1.SuspendLayout();
         this.panel1.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this._gridTraces)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this._dsTraceData)).BeginInit();
         this.groupBox2.SuspendLayout();
         this.SuspendLayout();
         // 
         // mainMenu1
         // 
         this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1,
            this.menuItem2,
            this.menuItem3,
            this.menuItem5});
         // 
         // menuItem1
         // 
         this.menuItem1.Index = 0;
         this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuRefresh,
            this.menuItem4,
            this.menuExit});
         this.menuItem1.Text = "&File";
         // 
         // menuRefresh
         // 
         this.menuRefresh.Index = 0;
         this.menuRefresh.Shortcut = System.Windows.Forms.Shortcut.F5;
         this.menuRefresh.Text = "&Refresh";
         this.menuRefresh.Click += new System.EventHandler(this.menuRefresh_Click);
         // 
         // menuItem4
         // 
         this.menuItem4.Index = 1;
         this.menuItem4.Text = "-";
         // 
         // menuExit
         // 
         this.menuExit.Index = 2;
         this.menuExit.Text = "E&xit";
         this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
         // 
         // menuItem2
         // 
         this.menuItem2.Index = 1;
         this.menuItem2.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuCopyTraces});
         this.menuItem2.Text = "&Edit";
         // 
         // menuCopyTraces
         // 
         this.menuCopyTraces.Enabled = false;
         this.menuCopyTraces.Index = 0;
         this.menuCopyTraces.Shortcut = System.Windows.Forms.Shortcut.CtrlC;
         this.menuCopyTraces.Text = "&Copy trace information";
         this.menuCopyTraces.Click += new System.EventHandler(this.menuCopyTraces_Click);
         // 
         // menuItem3
         // 
         this.menuItem3.Index = 2;
         this.menuItem3.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuUncompress});
         this.menuItem3.Text = "&Action";
         // 
         // menuUncompress
         // 
         this.menuUncompress.Index = 0;
         this.menuUncompress.Text = "Uncompress Trace Files";
         this.menuUncompress.Click += new System.EventHandler(this.menuUncompress_Click);
         // 
         // menuItem5
         // 
         this.menuItem5.Index = 3;
         this.menuItem5.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuAbout});
         this.menuItem5.Text = "&Help";
         // 
         // menuAbout
         // 
         this.menuAbout.Index = 0;
         this.menuAbout.Text = "&About Trace Manager...";
         this.menuAbout.Click += new System.EventHandler(this.menuAbout_Click);
         // 
         // label1
         // 
         this.label1.Location = new System.Drawing.Point(8, 12);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(116, 16);
         this.label1.TabIndex = 0;
         this.label1.Text = "SQL Server Instance:";
         // 
         // textServer
         // 
         this.textServer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.textServer.Location = new System.Drawing.Point(128, 8);
         this.textServer.Name = "textServer";
         this.textServer.Size = new System.Drawing.Size(464, 20);
         this.textServer.TabIndex = 1;
         this.textServer.Text = "(local)";
         this.textServer.TextChanged += new System.EventHandler(this.textServer_TextChanged);
         // 
         // btnConnect
         // 
         this.btnConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.btnConnect.Location = new System.Drawing.Point(600, 8);
         this.btnConnect.Name = "btnConnect";
         this.btnConnect.Size = new System.Drawing.Size(64, 20);
         this.btnConnect.TabIndex = 2;
         this.btnConnect.Text = "Connect";
         this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
         // 
         // groupBox1
         // 
         this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                     | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.groupBox1.Controls.Add(this.panel1);
         this.groupBox1.Controls.Add(this._gridTraces);
         this.groupBox1.Location = new System.Drawing.Point(12, 40);
         this.groupBox1.Name = "groupBox1";
         this.groupBox1.Size = new System.Drawing.Size(656, 260);
         this.groupBox1.TabIndex = 3;
         this.groupBox1.TabStop = false;
         this.groupBox1.Text = "Registered Traces";
         // 
         // panel1
         // 
         this.panel1.Controls.Add(this.btnClearAll);
         this.panel1.Controls.Add(this.btnStart);
         this.panel1.Controls.Add(this.btnStop);
         this.panel1.Controls.Add(this.btnDetails);
         this.panel1.Controls.Add(this.btnClose);
         this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.panel1.Location = new System.Drawing.Point(3, 227);
         this.panel1.Name = "panel1";
         this.panel1.Size = new System.Drawing.Size(650, 30);
         this.panel1.TabIndex = 7;
         // 
         // btnClearAll
         // 
         this.btnClearAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.btnClearAll.Enabled = false;
         this.btnClearAll.Location = new System.Drawing.Point(278, 3);
         this.btnClearAll.Name = "btnClearAll";
         this.btnClearAll.Size = new System.Drawing.Size(75, 23);
         this.btnClearAll.TabIndex = 5;
         this.btnClearAll.Text = "Clear All";
         this.btnClearAll.Click += new System.EventHandler(this.btnClearAll_Click);
         // 
         // btnStart
         // 
         this.btnStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.btnStart.Enabled = false;
         this.btnStart.Location = new System.Drawing.Point(3, 3);
         this.btnStart.Name = "btnStart";
         this.btnStart.Size = new System.Drawing.Size(75, 23);
         this.btnStart.TabIndex = 0;
         this.btnStart.Text = "Start";
         this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
         // 
         // btnStop
         // 
         this.btnStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.btnStop.Enabled = false;
         this.btnStop.Location = new System.Drawing.Point(80, 3);
         this.btnStop.Name = "btnStop";
         this.btnStop.Size = new System.Drawing.Size(75, 23);
         this.btnStop.TabIndex = 1;
         this.btnStop.Text = "Stop";
         this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
         // 
         // btnDetails
         // 
         this.btnDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.btnDetails.Enabled = false;
         this.btnDetails.Location = new System.Drawing.Point(570, 3);
         this.btnDetails.Name = "btnDetails";
         this.btnDetails.Size = new System.Drawing.Size(75, 23);
         this.btnDetails.TabIndex = 4;
         this.btnDetails.Text = "Details";
         this.btnDetails.Click += new System.EventHandler(this.btnFilters_Click);
         // 
         // btnClose
         // 
         this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.btnClose.Enabled = false;
         this.btnClose.Location = new System.Drawing.Point(161, 3);
         this.btnClose.Name = "btnClose";
         this.btnClose.Size = new System.Drawing.Size(75, 23);
         this.btnClose.TabIndex = 2;
         this.btnClose.Text = "Close";
         this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
         // 
         // _gridTraces
         // 
         this._gridTraces.DataSource = this._dsTraceData;
         appearance1.BackColor = System.Drawing.SystemColors.Window;
         appearance1.ForeColor = System.Drawing.SystemColors.WindowText;
         this._gridTraces.DisplayLayout.Appearance = appearance1;
         this._gridTraces.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
         ultraGridColumn1.Header.VisiblePosition = 0;
         ultraGridColumn1.Width = 82;
         ultraGridColumn2.Header.VisiblePosition = 1;
         ultraGridColumn2.Width = 98;
         ultraGridColumn3.Header.VisiblePosition = 2;
         ultraGridColumn3.Width = 323;
         ultraGridColumn4.Header.Caption = "Property";
         ultraGridColumn4.Header.VisiblePosition = 3;
         ultraGridColumn4.Width = 145;
         ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4});
         this._gridTraces.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
         this._gridTraces.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
         this._gridTraces.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
         this._gridTraces.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
         this._gridTraces.DisplayLayout.Override.AllowGroupBy = Infragistics.Win.DefaultableBoolean.False;
         this._gridTraces.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
         this._gridTraces.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
         appearance2.TextHAlignAsString = "Left";
         this._gridTraces.DisplayLayout.Override.HeaderAppearance = appearance2;
         this._gridTraces.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
         appearance3.BorderColor = System.Drawing.SystemColors.Control;
         appearance3.TextVAlignAsString = "Middle";
         this._gridTraces.DisplayLayout.Override.RowAppearance = appearance3;
         this._gridTraces.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
         this._gridTraces.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None;
         this._gridTraces.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
         this._gridTraces.DisplayLayout.Override.SelectTypeGroupByRow = Infragistics.Win.UltraWinGrid.SelectType.None;
         this._gridTraces.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
         this._gridTraces.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
         this._gridTraces.DisplayLayout.TabNavigation = Infragistics.Win.UltraWinGrid.TabNavigation.NextControlOnLastCell;
         this._gridTraces.Dock = System.Windows.Forms.DockStyle.Fill;
         this._gridTraces.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this._gridTraces.Location = new System.Drawing.Point(3, 16);
         this._gridTraces.Name = "_gridTraces";
         this._gridTraces.Size = new System.Drawing.Size(650, 241);
         this._gridTraces.TabIndex = 6;
         this._gridTraces.Text = "ultraGrid1";
         // 
         // _dsTraceData
         // 
         ultraDataColumn1.DataType = typeof(int);
         ultraDataColumn2.DataType = typeof(int);
         this._dsTraceData.Band.Columns.AddRange(new object[] {
            ultraDataColumn1,
            ultraDataColumn2,
            ultraDataColumn3,
            ultraDataColumn4});
         this._dsTraceData.Band.Key = "TraceData";
         // 
         // groupBox2
         // 
         this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.groupBox2.Controls.Add(this.btnDrop);
         this.groupBox2.Controls.Add(this.chkStartupStoredProc);
         this.groupBox2.Controls.Add(this.chkAuditStoredProc);
         this.groupBox2.Location = new System.Drawing.Point(12, 308);
         this.groupBox2.Name = "groupBox2";
         this.groupBox2.Size = new System.Drawing.Size(656, 48);
         this.groupBox2.TabIndex = 4;
         this.groupBox2.TabStop = false;
         this.groupBox2.Text = "SQLcompliance Stored Procedures";
         // 
         // btnDrop
         // 
         this.btnDrop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.btnDrop.Enabled = false;
         this.btnDrop.Location = new System.Drawing.Point(404, 16);
         this.btnDrop.Name = "btnDrop";
         this.btnDrop.Size = new System.Drawing.Size(244, 23);
         this.btnDrop.TabIndex = 2;
         this.btnDrop.Text = "Drop SQLcompliance Stored Procedures";
         this.btnDrop.Click += new System.EventHandler(this.btnDrop_Click);
         // 
         // chkStartupStoredProc
         // 
         this.chkStartupStoredProc.Location = new System.Drawing.Point(176, 20);
         this.chkStartupStoredProc.Name = "chkStartupStoredProc";
         this.chkStartupStoredProc.Size = new System.Drawing.Size(180, 20);
         this.chkStartupStoredProc.TabIndex = 1;
         this.chkStartupStoredProc.Text = "sp_sqlcompliance_startup";
         // 
         // chkAuditStoredProc
         // 
         this.chkAuditStoredProc.Location = new System.Drawing.Point(12, 20);
         this.chkAuditStoredProc.Name = "chkAuditStoredProc";
         this.chkAuditStoredProc.Size = new System.Drawing.Size(156, 20);
         this.chkAuditStoredProc.TabIndex = 0;
         this.chkAuditStoredProc.Text = "sp_sqlcompliance_audit";
         // 
         // menuItem333
         // 
         this.menuItem333.Index = -1;
         this.menuItem333.Text = "";
         // 
         // Form1
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(676, 365);
         this.Controls.Add(this.groupBox2);
         this.Controls.Add(this.groupBox1);
         this.Controls.Add(this.btnConnect);
         this.Controls.Add(this.textServer);
         this.Controls.Add(this.label1);
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.Menu = this.mainMenu1;
         this.Name = "Form1";
         this.Text = "SQLcompliance Trace Manager";
         this.groupBox1.ResumeLayout(false);
         this.panel1.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this._gridTraces)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this._dsTraceData)).EndInit();
         this.groupBox2.ResumeLayout(false);
         this.ResumeLayout(false);
         this.PerformLayout();

      }
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
         
		   try
		   {
			   Application.Run(new Form1());
			}
			catch (Exception ex )
			{
			   MessageBox.Show( ex.Message + "\n" + ex.StackTrace );
         }
	}

      private void menuExit_Click(object sender, EventArgs e)
      {
         this.Close();
      }


      private void btnConnect_Click(object sender, EventArgs e)
      {
         try
         {
            String connStr = String.Format( "server={0};integrated security=SSPI;Application Name='Trace Manager';",
                                            textServer.Text.Trim() );
            _connection = new SqlConnection(connStr);
            _connection.Open();           
            
            btnDetails.Enabled     = true;
            menuCopyTraces.Enabled = true;
            
            LoadTraceInformation();
         }
         catch ( Exception ex )
         {
            btnDetails.Enabled   = false;
            menuCopyTraces.Enabled = true;
            
            MessageBox.Show( ex.Message );
         }
      }
      
      SqlDataAdapter adapter = null;
      DataSet ds             = new DataSet();
      
      private void LoadTraceInformation()
      {
         SqlDataReader reader = null;
         
         try
         {
            this.Cursor = Cursors.WaitCursor;
            _dsTraceData.Rows.Clear() ;
            
            // load traces
            string sql1 = "SELECT * from :: fn_trace_getinfo(default)";

            using(SqlCommand comm = new SqlCommand(sql1, _connection))
            {
               int prop ;
               reader = comm.ExecuteReader() ;
               while(reader.Read())
               {
                  UltraDataRow row = _dsTraceData.Rows.Add() ;
                  row["traceid"] = reader.GetInt32(0) ;
                  prop = reader.GetInt32(1) ;
                  row["property"] = prop ;
                  row["value"] = reader.GetValue(2).ToString() ;
                  switch(prop)
                  {
                     case 1:
                        row["propertyname"] = "Trace Option";
                        break;
                     case 2:
                        row["propertyname"] = "FileName";
                        break;
                     case 3:
                        row["propertyname"] = "MaxSize";
                        break;
                     case 4:
                        row["propertyname"] = "Stop Time";
                        break;
                     default:
                        row["propertyname"] = "Trace Status";
                        break;
                  }
               }
               reader.Close() ;
            }

            if ( _gridTraces.Rows.Count != 0 )
            {
               btnStart.Enabled = true;
               btnStop.Enabled = true;
               btnClose.Enabled = true;
               btnClearAll.Enabled = true;
            }
            else
            {
               btnStart.Enabled = false;
               btnStop.Enabled = false;
               btnClose.Enabled = false;
               btnClearAll.Enabled = false;
            }
            
            // load stored procedures
            string sql2 = "SELECT name from sysobjects where name='sp_SQLcompliance_Audit' OR name = 'sp_SQLcompliance_StartUp';";
            SqlCommand cmd2 = new SqlCommand( sql2, _connection );
            reader = cmd2.ExecuteReader();
            
            while (reader.Read() )
            {
               if ( reader.GetString(0).ToUpper() == "SP_SQLCOMPLIANCE_AUDIT" )
               {
                  chkAuditStoredProc.Checked = true;
                  btnDrop.Enabled = true;
               }
               if ( reader.GetString(0).ToUpper() == "SP_SQLCOMPLIANCE_STARTUP" )
               {
                  chkStartupStoredProc.Checked = true;
                  btnDrop.Enabled = true;
               }
            }
         }
         catch ( Exception ex )
         {
            MessageBox.Show( ex.Message );
         }
         finally
         {
            if ( reader != null ) reader.Close();
            
            this.Cursor = Cursors.Default;
         }
      }
      
      private void btnStart_Click(object sender, EventArgs e)
      {
         DoTraceCmd(GetSelectedTraceId(), 1);
         LoadTraceInformation();
      }

      private void btnStop_Click(object sender, EventArgs e)
      {
         DoTraceCmd(GetSelectedTraceId(), 0);
         LoadTraceInformation();
      }

      private void btnClose_Click(object sender, EventArgs e)
      {
         DoTraceCmd(GetSelectedTraceId(), 2);
         LoadTraceInformation();
      }

      // Returns -1 if no trace is selected
      private int GetSelectedTraceId()
      {
         if (_gridTraces.Selected.Rows.Count == 1)
         {
            UltraGridRow gridRow = _gridTraces.Selected.Rows[0];
            UltraDataRow dataRow = gridRow.ListObject as UltraDataRow;

            if (dataRow != null)
               return (int)dataRow["traceid"];
            else
               return -1;
         }
         else
            return -1;
      }
      
      private bool DoTraceCmd(int traceId, int command)
      {
         bool retval = false;

         try
         {
            // set trace status         
            string sql2;
            SqlCommand cmd;
            
            sql2 = "exec sp_trace_setstatus " + traceId + "," + command;
            cmd = new SqlCommand( sql2, _connection );
            cmd.ExecuteNonQuery();
            
            retval = true;
         }
         catch (Exception ex )
         {
            MessageBox.Show( ex.Message );
         }
         
         return retval;
      }
      

      private void btnDrop_Click(object sender, EventArgs e)
      {
         try
         {
            // load stored procedures
            string sql2;
            SqlCommand cmd;
            
            if ( chkAuditStoredProc.Checked )
            {
               sql2 = "DROP proc sp_SQLcompliance_Audit";
               cmd = new SqlCommand( sql2, _connection );
               cmd.ExecuteNonQuery();
            }
            
            if ( chkStartupStoredProc.Checked )
            {
               sql2 = "DROP proc sp_SQLcompliance_StartUp";
               cmd = new SqlCommand( sql2, _connection );
               cmd.ExecuteNonQuery();
            }

            chkAuditStoredProc.Checked   = false;
            chkStartupStoredProc.Checked = false;
         }
         catch (Exception ex )
         {
            MessageBox.Show( ex.Message );
         }
      }

      private void textServer_TextChanged(object sender, EventArgs e)
      {
         if ( textServer.Text.Trim() == "" )
            btnConnect.Enabled = false;
         else
            btnConnect.Enabled = true;
      }

      private void btnFilters_Click(object sender, EventArgs e)
      {
         ShowDetails();
      }
      
      private void ShowDetails()
      {
         int traceId = GetSelectedTraceId() ;
         if(traceId != -1)
         {
            Form_Details frm = new Form_Details(_connection, traceId);
            frm.ShowDialog();
         }
      }

      private void menuCopyTraces_Click(object sender, EventArgs e)
      {
         StringBuilder s = new StringBuilder( "", 2048 );
         
         // traces are includes in grid
         int lastId = -1;
         for (int t=0; t< _dsTraceData.Rows.Count; t++)
         {
            UltraDataRow dr = _dsTraceData.Rows[t];
            int traceId = (int)dr["traceid"];
            
            if ( traceId != lastId )
            {
               lastId = traceId;
               
               s.Append( "----------------------------------------------------\r\n" );
               s.Append( "Trace ID: " + traceId + "\r\n" );
               
               // dump trace info
               for ( int j=0;j<5;j++ )
               {
                  dr = _dsTraceData.Rows[t + j];
                  string val = dr["value"].ToString() ;         
                  string nm  = dr["propertyname"].ToString();         
                  s.Append( "\t" + nm + ":\t");
                  s.Append( val + "\r\n" );
               }

               // Get columns and filters               
               string sql;
               
               try
               {
                  
                  // load filters
                  sql = "SELECT * from :: fn_trace_getfilterinfo("+traceId+")";
                  adapter = new SqlDataAdapter( sql, _connection );
                  ds = new DataSet();
                  adapter.Fill(ds);
                  
                  s.Append( "\r\nTrace Filters:\r\n" );
                  for (int r=0; r<ds.Tables["Table"].Rows.Count; r++ )
                  {
                      s.Append( "\tColumn ID:\t ");
                         s.Append( ds.Tables["Table"].Rows[r][0].ToString() );
                         
                      s.Append( "\r\n\t\tlogicalOp:\t");
                         s.Append( ds.Tables["Table"].Rows[r][1].ToString() );
                         
                      s.Append( "\r\n\t\tcomparisonOp:\t");
                         s.Append( ds.Tables["Table"].Rows[r][2].ToString() );
                         
                      s.Append( "\r\n\t\tvalue:\t\t");
                        s.Append( ds.Tables["Table"].Rows[r][3].ToString() );
                        
                      s.Append( "\r\n" );
                  }
                  
                  // load columns
                  sql = "SELECT * from :: fn_trace_geteventinfo("+traceId+")";
                  adapter = new SqlDataAdapter( sql, _connection );
                  ds = new DataSet();
                  adapter.Fill(ds);
                  
                  s.Append( "\r\nTrace Columns:\r\n" );
                  for (int r=0; r<ds.Tables["Table"].Rows.Count; r++ )
                  {
                      s.Append( "\tEvent ID:\t" + ds.Tables["Table"].Rows[r][0] );
                      s.Append( "\tColumn ID:\t" + ds.Tables["Table"].Rows[r][1] );
                      s.Append( "\r\n" );
                  }
               }
               catch ( Exception ex )
               {
                  s.Equals(ex.Message);
                  MessageBox.Show (ex.Message);
               }
               
            }
         }
         s.Append( "----------------------------------------------------\r\n" );
         
         Clipboard.SetDataObject( s.ToString() );
         
         MessageBox.Show ("Trace detailed information copied to clipboard");
      }

      private void menuRefresh_Click(object sender, EventArgs e)
      {
         LoadTraceInformation();
      }

      private void menuUncompress_Click(object sender, EventArgs e)
      {
         Form_Uncompress frm = new Form_Uncompress();
         frm.ShowDialog();
         
      }

      private void btnClearAll_Click(object sender, EventArgs e)
      {
         List<int> traceIds = new List<int>();

         foreach(UltraDataRow row in _dsTraceData.Rows)
         {
            int traceId = (int)row["traceid"] ;
            if(!traceIds.Contains(traceId))
               traceIds.Add(traceId) ;
         }
         foreach (int traceId in traceIds)
         {
            DoTraceCmd(traceId, 0) ;
            DoTraceCmd(traceId, 2) ;
         }
         LoadTraceInformation() ;
         /*
         int lastTraceId = -1;
         while (gridTraces.DataRows.Count != 0)
         {
            DataRow dr = gridTraces.DataRows[0];
            int traceId = (int)dr.Cells[0].Value;         
            if ( lastTraceId == traceId )
               break;
            lastTraceId = traceId;
            
            gridTraces.SelectedRows.Add( gridTraces.DataRows[0] );
            DoTraceCmd(0);
            DoTraceCmd(2);
            LoadTraceInformation();
         }*/
      }

      private void menuAbout_Click(object sender, EventArgs e)
      {
         Form_About frm = new Form_About();
         frm.ShowDialog();
      }
	}
}
