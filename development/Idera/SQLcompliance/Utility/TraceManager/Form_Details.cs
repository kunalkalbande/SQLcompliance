using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Windows.Forms;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;

namespace Idera.SQLcompliance.Utility.TraceManager
{
	/// <summary>
	/// Summary description for Form_Details.
	/// </summary>
	public class Form_Details : Form
	{
      private Label label1;
      private IContainer components;
		
		private SqlConnection _connection;
      private Label label2;
      private Button btnClose;
      private Button btnCollapse;
      private Button btnExpand;
      private Label label3;
      private ListBox listBox1;
      private Label label4;
      private ListBox listBox2;
      private UltraDataSource _dsEvents;
      private UltraDataSource _dsFilters;
      private UltraGrid _gridFilters;
      private UltraGrid _gridEvents;
		int           traceId;

		public
		   Form_Details(
		      SqlConnection inConn,
		      int           inTraceId
		   )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

         _connection    = inConn;
         traceId = inTraceId;
         
         this.Text = this.Text + " - Trace ID: " + traceId;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
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
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn1 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("eventid");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn2 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("columnid");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn3 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("columnid");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn4 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("logicaloperator");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn5 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("comparisonoperator");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn6 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("value");
         Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
         Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Filters", -1);
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("columnid");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("logicaloperator");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("comparisonoperator");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("value");
         Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
         Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
         Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
         Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand2 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Events", -1);
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("eventid");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("columnid");
         Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
         Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
         Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
         Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Details));
         this.label1 = new System.Windows.Forms.Label();
         this.label2 = new System.Windows.Forms.Label();
         this.btnClose = new System.Windows.Forms.Button();
         this.btnCollapse = new System.Windows.Forms.Button();
         this.btnExpand = new System.Windows.Forms.Button();
         this.label3 = new System.Windows.Forms.Label();
         this.listBox1 = new System.Windows.Forms.ListBox();
         this.label4 = new System.Windows.Forms.Label();
         this.listBox2 = new System.Windows.Forms.ListBox();
         this._dsEvents = new Infragistics.Win.UltraWinDataSource.UltraDataSource(this.components);
         this._dsFilters = new Infragistics.Win.UltraWinDataSource.UltraDataSource(this.components);
         this._gridFilters = new Infragistics.Win.UltraWinGrid.UltraGrid();
         this._gridEvents = new Infragistics.Win.UltraWinGrid.UltraGrid();
         ((System.ComponentModel.ISupportInitialize)(this._dsEvents)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this._dsFilters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this._gridFilters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this._gridEvents)).BeginInit();
         this.SuspendLayout();
         // 
         // label1
         // 
         this.label1.Location = new System.Drawing.Point(256, 4);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(44, 16);
         this.label1.TabIndex = 1;
         this.label1.Text = "Filters:";
         // 
         // label2
         // 
         this.label2.Location = new System.Drawing.Point(8, 4);
         this.label2.Name = "label2";
         this.label2.Size = new System.Drawing.Size(44, 16);
         this.label2.TabIndex = 2;
         this.label2.Text = "Events:";
         // 
         // btnClose
         // 
         this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.btnClose.Location = new System.Drawing.Point(652, 464);
         this.btnClose.Name = "btnClose";
         this.btnClose.Size = new System.Drawing.Size(75, 20);
         this.btnClose.TabIndex = 4;
         this.btnClose.Text = "&Close";
         this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
         // 
         // btnCollapse
         // 
         this.btnCollapse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.btnCollapse.Location = new System.Drawing.Point(52, 464);
         this.btnCollapse.Name = "btnCollapse";
         this.btnCollapse.Size = new System.Drawing.Size(75, 20);
         this.btnCollapse.TabIndex = 5;
         this.btnCollapse.Text = "Collapse";
         this.btnCollapse.Click += new System.EventHandler(this.btnCollapse_Click);
         // 
         // btnExpand
         // 
         this.btnExpand.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.btnExpand.Location = new System.Drawing.Point(132, 464);
         this.btnExpand.Name = "btnExpand";
         this.btnExpand.Size = new System.Drawing.Size(75, 20);
         this.btnExpand.TabIndex = 6;
         this.btnExpand.Text = "Expand";
         this.btnExpand.Click += new System.EventHandler(this.btnExpand_Click);
         // 
         // label3
         // 
         this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.label3.Location = new System.Drawing.Point(252, 288);
         this.label3.Name = "label3";
         this.label3.Size = new System.Drawing.Size(152, 16);
         this.label3.TabIndex = 7;
         this.label3.Text = "Column Definitions:";
         // 
         // listBox1
         // 
         this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.listBox1.HorizontalScrollbar = true;
         this.listBox1.Items.AddRange(new object[] {
            " 1  TextData",
            " 2  BinaryData",
            " 3  DatabaseID",
            " 4  TransactionID",
            " 5  Reserved",
            " 6  NTUserName",
            " 7  NTDomainName",
            " 8  ClientHostName",
            " 9  ClientProcessID",
            "10  ApplicationName",
            "11  SQLSecurityLoginName ",
            "12  SPID",
            "13  Duration",
            "14  StartTime",
            "15  EndTime               ",
            "16  Reads                 ",
            "17  Writes                ",
            "18  CPU                   ",
            "19  Permissions           ",
            "20  Severity              ",
            "21  EventSubClass         ",
            "22  ObjectID              ",
            "23  Success               ",
            "24  IndexID               ",
            "25  IntegerData           ",
            "26  ServerName            ",
            "27  EventClass            ",
            "28  ObjectType            ",
            "29  NestLevel             ",
            "30  State                 ",
            "31  Error                 ",
            "32  Mode                  ",
            "33  Handle                ",
            "34  ObjectName            ",
            "35  DatabaseName          ",
            "36  Filename              ",
            "37  ObjectOwner           ",
            "38  TargetRoleName        ",
            "39  TargetUserName        ",
            "40  DatabaseUserName      ",
            "41  LoginSID              ",
            "42  TargetLoginName       ",
            "43  TargetLoginSID        ",
            "44  ColumnPermissionsSet",
            "45  LinkedServerName",
            "46  ProviderName",
            "59  ParentName",
            "60  IsSystem",
            "64  SessionLoginName"});
         this.listBox1.Location = new System.Drawing.Point(252, 304);
         this.listBox1.Name = "listBox1";
         this.listBox1.ScrollAlwaysVisible = true;
         this.listBox1.Size = new System.Drawing.Size(476, 147);
         this.listBox1.TabIndex = 8;
         // 
         // label4
         // 
         this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.label4.Location = new System.Drawing.Point(20, 288);
         this.label4.Name = "label4";
         this.label4.Size = new System.Drawing.Size(100, 16);
         this.label4.TabIndex = 9;
         this.label4.Text = "Event Definitions";
         // 
         // listBox2
         // 
         this.listBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.listBox2.Items.AddRange(new object[] {
            "10  RpcCompleted",
            "11  RpcStarted",
            "12  BatchCompleted",
            "13  BatchStarting",
            "14  Login",
            "15  Logout",
            "16  Attention",
            "17  ExistingConnection",
            "18  ServiceControl    ",
            "19  DtcTransaction                ",
            "20  LoginFailed                   ",
            "21  EventLog                      ",
            "22  ErrorLog                      ",
            "23  LockReleased                  ",
            "24  LockAcquired                  ",
            "25  LockDeadlock                  ",
            "26  LockCancel                    ",
            "27  LockTimeout                   ",
            "28  DopEvent                      ",
            "33  Exception                     ",
            "34  SpCacheMiss                   ",
            "35  SpCacheInsert                 ",
            "36  SpCacheRemove                 ",
            "37  SpRecompile                   ",
            "38  SpCacheHit                    ",
            "39  SpExecContextHit              ",
            "40  SqlStmtStarting               ",
            "41  SqlStmtCompleted              ",
            "42  SpStarting                    ",
            "43  SpCompleted                   ",
            "46  ObjectCreated                 ",
            "47  ObjectDeleted                 ",
            "50  Transaction                   ",
            "51  ScanStarted                   ",
            "52  ScanStopped                   ",
            "53  CursorOpen                    ",
            "54  TransactionLog                ",
            "55  HashWarning                   ",
            "58  AutoUpdateStats               ",
            "59  LockDeadlockChain             ",
            "60  LockEscalation                ",
            "61  OleDbError                    ",
            "67  ExecutionWarning              ",
            "68  ExecutionPlan                 ",
            "69  SortWarning                   ",
            "70  CursorPrepare                 ",
            "71  Prepare                       ",
            "72  ExecPrepare                   ",
            "73  Unprepare                     ",
            "74  CursorExecute                 ",
            "75  CursorRecompile               ",
            "76  CursorImplicitConversion      ",
            "77  CursorUnprepare               ",
            "78  CursorClose                   ",
            "79  MissingColumn                 ",
            "80  MissingJoin                   ",
            "81  ServerMemoryChange            ",
            "92  DataFileAutoGrow              ",
            "93  LogFileAutoGrow               ",
            "94  DataFileAutoShrink            ",
            "95  LogFileAutoShrink             ",
            "96  ShowPlanText                  ",
            "97  ShowPlanAll                   ",
            "98  ShowPlanStatistics            ",
            "100 RpcOutputParameter            ",
            "102 AuditStatementGDR             ",
            "103 AuditObjectGDR                ",
            "104 AuditAddLogin                 ",
            "105 AuditLoginGDR                 ",
            "106 AuditLoginChange              ",
            "107 AuditLoginChangePassword      ",
            "108 AuditAddLoginToServer         ",
            "109 AuditAddDbUser                ",
            "110 AuditAddMember                ",
            "111 AuditAddDropRole              ",
            "112 AppRolePassChange             ",
            "113 AuditStatementPermission      ",
            "114 AuditObjectPermission         ",
            "115 AuditBackupRestore            ",
            "116 AuditDbcc                     ",
            "117 AuditChangeAudit              ",
            "118 AuditObjectDerivedPermission   ",
            "128 AuditDatabaseManagement",
            "129 AuditDatabaseObjectManagement",
            "130 AuditDatabasePrincipalManagement",
            "131 AuditSchemaObjectManagement",
            "132 AuditServerPrincipalImpersonation",
            "133 AuditDatabasePrincipalImpersonation",
            "134 AuditServerObjectTakeOwnership",
            "135 AuditDatabaseObjectTakeOwnership",
            "152 AuditChangeDatabaseOwner",
            "153 AuditSchemaObjectTakeOwnership",
            "158 AuditBrokerConversation",
            "159 AuditBrokerLogin",
            "170 AuditServerScopeGDR",
            "171 AuditServerObjectGDR",
            "172 AuditDatabaseObjectGDR",
            "173 AuditServerOperation",
            "175 AuditServerAlterTrace",
            "176 AuditServerObjectManagement",
            "177 AuditServerPrincipalManagement",
            "178 AuditDatabaseOperation",
            "180 AuditDatabaseObjectAccess"});
         this.listBox2.Location = new System.Drawing.Point(16, 304);
         this.listBox2.Name = "listBox2";
         this.listBox2.Size = new System.Drawing.Size(224, 147);
         this.listBox2.TabIndex = 10;
         // 
         // _dsEvents
         // 
         ultraDataColumn1.DataType = typeof(int);
         ultraDataColumn2.DataType = typeof(int);
         this._dsEvents.Band.Columns.AddRange(new object[] {
            ultraDataColumn1,
            ultraDataColumn2});
         this._dsEvents.Band.Key = "Events";
         // 
         // _dsFilters
         // 
         ultraDataColumn3.DataType = typeof(int);
         ultraDataColumn4.DataType = typeof(int);
         ultraDataColumn5.DataType = typeof(int);
         this._dsFilters.Band.Columns.AddRange(new object[] {
            ultraDataColumn3,
            ultraDataColumn4,
            ultraDataColumn5,
            ultraDataColumn6});
         this._dsFilters.Band.Key = "Filters";
         // 
         // _gridFilters
         // 
         this._gridFilters.DataMember = "Filters";
         this._gridFilters.DataSource = this._dsFilters;
         appearance1.BackColor = System.Drawing.SystemColors.Window;
         appearance1.ForeColor = System.Drawing.SystemColors.WindowText;
         this._gridFilters.DisplayLayout.Appearance = appearance1;
         this._gridFilters.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
         ultraGridColumn1.Header.VisiblePosition = 0;
         ultraGridColumn1.Width = 37;
         ultraGridColumn2.Header.VisiblePosition = 1;
         ultraGridColumn2.Width = 59;
         ultraGridColumn3.Header.VisiblePosition = 2;
         ultraGridColumn3.Width = 75;
         ultraGridColumn4.Header.VisiblePosition = 3;
         ultraGridColumn4.Width = 295;
         ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4});
         this._gridFilters.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
         this._gridFilters.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
         this._gridFilters.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
         this._gridFilters.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
         this._gridFilters.DisplayLayout.Override.AllowGroupBy = Infragistics.Win.DefaultableBoolean.False;
         this._gridFilters.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
         this._gridFilters.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
         appearance2.TextHAlignAsString = "Left";
         this._gridFilters.DisplayLayout.Override.HeaderAppearance = appearance2;
         this._gridFilters.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
         appearance3.BorderColor = System.Drawing.SystemColors.Control;
         appearance3.TextVAlignAsString = "Middle";
         this._gridFilters.DisplayLayout.Override.RowAppearance = appearance3;
         this._gridFilters.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
         this._gridFilters.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None;
         this._gridFilters.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
         this._gridFilters.DisplayLayout.Override.SelectTypeGroupByRow = Infragistics.Win.UltraWinGrid.SelectType.None;
         this._gridFilters.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
         this._gridFilters.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
         this._gridFilters.DisplayLayout.TabNavigation = Infragistics.Win.UltraWinGrid.TabNavigation.NextControlOnLastCell;
         this._gridFilters.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this._gridFilters.Location = new System.Drawing.Point(259, 23);
         this._gridFilters.Name = "_gridFilters";
         this._gridFilters.Size = new System.Drawing.Size(468, 262);
         this._gridFilters.TabIndex = 11;
         this._gridFilters.Text = "ultraGrid1";
         // 
         // _gridEvents
         // 
         this._gridEvents.DataMember = "Events";
         this._gridEvents.DataSource = this._dsEvents;
         appearance4.BackColor = System.Drawing.SystemColors.Window;
         appearance4.ForeColor = System.Drawing.SystemColors.WindowText;
         this._gridEvents.DisplayLayout.Appearance = appearance4;
         this._gridEvents.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
         ultraGridColumn5.Header.VisiblePosition = 0;
         ultraGridColumn5.Width = 110;
         ultraGridColumn6.Header.VisiblePosition = 1;
         ultraGridColumn6.Width = 117;
         ultraGridBand2.Columns.AddRange(new object[] {
            ultraGridColumn5,
            ultraGridColumn6});
         this._gridEvents.DisplayLayout.BandsSerializer.Add(ultraGridBand2);
         this._gridEvents.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
         appearance5.BackColor = System.Drawing.SystemColors.ButtonShadow;
         appearance5.ForeColor = System.Drawing.SystemColors.ButtonFace;
         this._gridEvents.DisplayLayout.GroupByBox.PromptAppearance = appearance5;
         this._gridEvents.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
         this._gridEvents.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
         this._gridEvents.DisplayLayout.Override.AllowGroupBy = Infragistics.Win.DefaultableBoolean.True;
         this._gridEvents.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
         this._gridEvents.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
         appearance6.BackColor = System.Drawing.SystemColors.Control;
         this._gridEvents.DisplayLayout.Override.GroupByRowAppearance = appearance6;
         appearance7.TextHAlignAsString = "Left";
         this._gridEvents.DisplayLayout.Override.HeaderAppearance = appearance7;
         this._gridEvents.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
         appearance8.BorderColor = System.Drawing.SystemColors.Control;
         appearance8.TextVAlignAsString = "Middle";
         this._gridEvents.DisplayLayout.Override.RowAppearance = appearance8;
         this._gridEvents.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
         this._gridEvents.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None;
         this._gridEvents.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
         this._gridEvents.DisplayLayout.Override.SelectTypeGroupByRow = Infragistics.Win.UltraWinGrid.SelectType.None;
         this._gridEvents.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
         this._gridEvents.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
         this._gridEvents.DisplayLayout.TabNavigation = Infragistics.Win.UltraWinGrid.TabNavigation.NextControlOnLastCell;
         this._gridEvents.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
         this._gridEvents.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this._gridEvents.Location = new System.Drawing.Point(11, 23);
         this._gridEvents.Name = "_gridEvents";
         this._gridEvents.Size = new System.Drawing.Size(229, 262);
         this._gridEvents.TabIndex = 12;
         this._gridEvents.Text = "ultraGrid2";
         // 
         // Form_Details
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(740, 490);
         this.Controls.Add(this._gridEvents);
         this.Controls.Add(this._gridFilters);
         this.Controls.Add(this.listBox2);
         this.Controls.Add(this.label4);
         this.Controls.Add(this.listBox1);
         this.Controls.Add(this.label3);
         this.Controls.Add(this.btnExpand);
         this.Controls.Add(this.btnCollapse);
         this.Controls.Add(this.btnClose);
         this.Controls.Add(this.label2);
         this.Controls.Add(this.label1);
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.Name = "Form_Details";
         this.Text = "Details";
         this.Load += new System.EventHandler(this.Details_Load);
         ((System.ComponentModel.ISupportInitialize)(this._dsEvents)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this._dsFilters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this._gridFilters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this._gridEvents)).EndInit();
         this.ResumeLayout(false);

      }
		#endregion

      private void Details_Load(object sender, EventArgs e)
      {
         try
         {
            // load filters
            string sql1 = "SELECT * from :: fn_trace_getfilterinfo("+traceId+")";
            using(SqlCommand comm = new SqlCommand(sql1, _connection))
            {
               using(SqlDataReader reader = comm.ExecuteReader())
               {
                  while(reader.Read())
                  {
                     UltraDataRow row = _dsFilters.Rows.Add() ;
                     row["columnid"] = reader.GetInt32(0);
                     row["logicaloperator"] = reader.GetInt32(1);
                     row["comparisonoperator"] = reader.GetInt32(2);
                     row["value"] = reader.GetValue(3).ToString();
                  }
               }
            }
            
            // load events
            string sql2 = "SELECT * from :: fn_trace_geteventinfo("+traceId+")";
            using (SqlCommand comm = new SqlCommand(sql2, _connection))
            {
               using (SqlDataReader reader = comm.ExecuteReader())
               {
                  while (reader.Read())
                  {
                     UltraDataRow row = _dsEvents.Rows.Add();
                     row["eventid"] = reader.GetInt32(0);
                     row["columnid"] = reader.GetInt32(1);
                  }
               }
            }
         }
         catch ( Exception ex )
         {
            MessageBox.Show (ex.Message);
         }
      
      }

      private void btnCollapse_Click(object sender, EventArgs e)
      {
         _gridEvents.Rows.CollapseAll(true) ;
      }

      private void btnExpand_Click(object sender, EventArgs e)
      {
         _gridEvents.Rows.ExpandAll(true) ;
      }

      private void btnClose_Click(object sender, EventArgs e)
      {
         this.Close();
      }
	}
}
