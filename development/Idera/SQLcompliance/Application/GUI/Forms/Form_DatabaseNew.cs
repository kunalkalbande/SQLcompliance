using System ;
using System.Collections ;
using System.Collections.Generic ;
using System.Data.SqlClient ;
using System.Diagnostics ;
using System.Windows.Forms ;
using Idera.SQLcompliance.Application.GUI.Controls ;
using Idera.SQLcompliance.Application.GUI.Helper ;
using Idera.SQLcompliance.Application.GUI.Properties ;
using Idera.SQLcompliance.Application.GUI.SQL ;
using Idera.SQLcompliance.Core ;
using Idera.SQLcompliance.Core.Agent ;
using Idera.SQLcompliance.Core.Collector ;
using Idera.SQLcompliance.Core.Event ;
using Idera.SQLcompliance.Core.Status ;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
	/// <summary>
	/// Summary description for Form_DatabaseNew
	/// </summary>
	public partial class Form_DatabaseNew : Form
   {
      #region Members

      private ServerRecord _server = null;
      private bool _isLoaded = false;
      private List<DatabaseRecord> _databases;

      private SQLDirect _sqlServer = null;

      // Control that is invalid; focus after Validate Page
      private Control _vControl = null;

      private bool _isUserDatabaseLoaded = false;
      private bool _isSystemDatabaseLoaded = false;

      private bool _allowOverride = false;

      #endregion

      #region Constructor / Dispose

      //--------------------------------------------------------------------
      // Constructor
      //--------------------------------------------------------------------
		public Form_DatabaseNew(ServerRecord server)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

         this.Icon = Resources.SQLcompliance_product_ico;
         _server = server;
         if (server == null)
            _allowOverride = true;
		}
		
		
      #endregion		

      #region Properties		

	   public List<DatabaseRecord> DatabaseList

	   {
	      get { return _databases ; }
	      set { _databases = value ; }
	   }

	   #endregion


      #region Exit Handlers - Finish / Cancel
      
      //--------------------------------------------------------------------
      // btnCancel_Click
      //--------------------------------------------------------------------
      private void btnCancel_Click(object sender, EventArgs e)
      {
         this.Close();
         _sqlServer.CloseConnection();
      }
      
      //--------------------------------------------------------------------
      // btnFinish_Click
      //--------------------------------------------------------------------
      private void btnFinish_Click(object sender, EventArgs e)
      {
         for ( int i=0; i<numPages; i++ )
         {
            if ( ! ValidatePage() )
            {
               currentPage = i;
               SetCurrentPage();
               if ( _vControl!=null) _vControl.Focus();
               return;
            }
         }
         
         // Save
         if ( SaveDatabaseRecords() )
         {
            // Let Agent now that audit settings have changed
            // Update GUI as appropriate - or let caller do this?
            this.DialogResult = DialogResult.OK;
            this.Close();
         }
      }
      
      //--------------------------------------------------------------------
      // CreateDatabaseRecord
      //--------------------------------------------------------------------
      private DatabaseRecord CreateDatabaseRecord(string dbName, int dbId)
      {
         DatabaseRecord db = new DatabaseRecord();
         db.Connection = Globals.Repository.Connection;
         
         
         // General
     	   db.SrvId         = _server == null ? -1 : _server.SrvId ;
		   db.SrvInstance   = _server.Instance;
		   
		   db.Name          = dbName;
		   db.SqlDatabaseId = dbId;

		   db.Description   = "";
		   db.IsEnabled     = true;
		   db.IsSqlSecureDb = false;
		   
		   if ( radioTypical.Checked )
		   {
            // Audit Settings		
		      db.AuditDDL            = true;
		      db.AuditSecurity       = true;
		      db.AuditAdmin          = true;
		      db.AuditDML            = false;
		      db.AuditSELECT         = false;
		      db.AuditAccessCheck    = AccessCheckFilter.SuccessOnly ;
		      db.AuditCaptureSQL     = false;
            db.AuditCaptureTrans   = false;
		      db.AuditExceptions     = false;
            db.AuditDmlAll           = true;
            db.AuditUserTables       = Globals.SQLcomplianceConfig.AuditUserTables;
		      db.AuditSystemTables     = Globals.SQLcomplianceConfig.AuditSystemTables;
		      db.AuditStoredProcedures = Globals.SQLcomplianceConfig.AuditStoredProcedures;
		      db.AuditDmlOther         = Globals.SQLcomplianceConfig.AuditDmlOther;
		   }
		   else
		   {
            // Audit Settings		
		      db.AuditDDL            = chkAuditDDL.Checked;
		      db.AuditSecurity       = chkAuditSecurity.Checked;
		      db.AuditAdmin          = chkAuditAdmin.Checked;
		      db.AuditDML            = chkAuditDML.Checked;
		      db.AuditSELECT         = chkAuditSELECT.Checked;
            if(chkFilterOnAccess.Checked)
            {
               if(_rbAccessFailed.Checked)
                  db.AuditAccessCheck = AccessCheckFilter.FailureOnly ;
               else
                  db.AuditAccessCheck = AccessCheckFilter.SuccessOnly ;
            }
            else
            {
               db.AuditAccessCheck = AccessCheckFilter.NoFilter ;
            }
            db.AuditCaptureSQL = CoreConstants.AllowCaptureSql && chkCaptureSQL.Checked;
            db.AuditCaptureTrans = chkCaptureTrans.Checked;
            db.AuditExceptions = false;
   		   
		      if ( db.AuditDML || db.AuditSELECT )
		      {
               if ( radioAllDML.Checked )
               {
		            db.AuditDmlAll = true;

                  // set other values to glabl defaults		         
                  db.AuditUserTables       = Globals.SQLcomplianceConfig.AuditUserTables;
		            db.AuditSystemTables     = Globals.SQLcomplianceConfig.AuditSystemTables;
		            db.AuditStoredProcedures = Globals.SQLcomplianceConfig.AuditStoredProcedures;
		            db.AuditDmlOther         = Globals.SQLcomplianceConfig.AuditDmlOther;
               }
               else
               {
		            db.AuditDmlAll = false;
      		      
		            // User Tables (not boolean; 2 means select specific tables; done in DB properties)
		            if ( chkAuditUserTables.Checked )
		               db.AuditUserTables = 1;
		            else
		               db.AuditUserTables = 0;
         		
		            // Audit Objects
		            db.AuditSystemTables     = chkAuditSystemTables.Checked;
		            db.AuditStoredProcedures = chkAuditStoredProcedures.Checked;
		            db.AuditDmlOther         = chkAuditOther.Checked;
		         }
		      }
		      else
		      {
		         // save global defaults
		         db.AuditDmlAll           = Globals.SQLcomplianceConfig.AuditDmlAll;
               db.AuditUserTables       = Globals.SQLcomplianceConfig.AuditUserTables;
		         db.AuditSystemTables     = Globals.SQLcomplianceConfig.AuditSystemTables;
		         db.AuditStoredProcedures = Globals.SQLcomplianceConfig.AuditStoredProcedures;
		         db.AuditDmlOther         = Globals.SQLcomplianceConfig.AuditDmlOther;
		      }
            db.AuditUsersList = GetTrustedUserProperty();
         }
         return db;
      }


      
      //--------------------------------------------------------------------
      // SaveDatabaseRecords
      //--------------------------------------------------------------------
      private bool SaveDatabaseRecords()
      {
         string               snapshot;
         bool                 retval      = true;
         _databases = new List<DatabaseRecord>();
         for (int ndx=0; ndx<listDatabases.CheckedItems.Count; ndx++ )
         {
            RawDatabaseObject rawDB = (RawDatabaseObject)listDatabases.CheckedItems[ndx];
            DatabaseRecord db = CreateDatabaseRecord(rawDB.name, rawDB.dbid);

            if (!WriteDatabaseRecord(db))
            {
               retval = false;
               // TODO: ask user if they want to contine
            }
            else
            {
               snapshot = Snapshot.DatabaseSnapshot( Globals.Repository.Connection ,
                                                     db.DbId,
                                                     db.Name,
                                                     true );
               // log addition
               ServerUpdate.RegisterChange(db.SrvId,
                                            LogType.NewDatabase,
                                            db.SrvInstance,
                                            snapshot );
                                            
               // remove from list in case there is a error on some other db
               listDatabases.Items.Remove(listDatabases.CheckedItems[ndx]);
               ndx--;  // because we just decremented the count and changed the list!
               _databases.Add(db);
            }
         }
         
         for (int ndx=0; ndx<listSystemDatabases.CheckedItems.Count; ndx++ )
         {
            RawDatabaseObject rawDB = (RawDatabaseObject)listSystemDatabases.CheckedItems[ndx];
            DatabaseRecord db = CreateDatabaseRecord(rawDB.name, rawDB.dbid);

            if ( ! WriteDatabaseRecord( db ) )
            {
               retval = false;
               // TODO: ask user if they want to contine
            }
            else
            {
               snapshot = Snapshot.DatabaseSnapshot( Globals.Repository.Connection,
                                                     db.DbId,
                                                     db.Name,
                                                     true );
				   ServerUpdate.RegisterChange( db.SrvId,
                                            LogType.NewDatabase,
                                            db.SrvInstance,
                                            snapshot);
               // remove from list in case there is a error on some other db
               listSystemDatabases.Items.Remove(listSystemDatabases.CheckedItems[ndx]);
               ndx--;  // because we just decremented the count and changed the list!
               _databases.Add(db);
            }
           
         }
            
         return retval;
      }
      
      private bool WriteDatabaseRecord(DatabaseRecord    db)
      {
         SqlTransaction       transaction ;
         bool                 retval      = false;
         
		  using(transaction = null)
		  {
			  try
			  {
				  // Execute Update SQL in a transaction
         
				  if ( ! db.Create(null) )
				  {
					  ErrorMessage.Show( this.Text,
						  UIConstants.Error_ErrorCreatingDatabase,
						  DatabaseRecord.GetLastError() );
				  }
				  else
				  {
					  retval  = true;
				  }
			  }
			  finally
			  {
				  if ( transaction != null )
				  {
					  if ( retval )
					  {
						  transaction.Commit();
                  
						  // Register change to server and perform audit log				      
						  ServerUpdate.RegisterChange( db.SrvId,
							  LogType.NewDatabase,
							  db.SrvInstance,
							  db.Name );
					  }
					  else
					  {
						  transaction.Rollback();
					  }
				  }
			  }
		  }
            
         return retval;
      }
      
      #endregion

      #region Navigation Handlers - Back/Prev
      
      //--------------------------------------------------------------------
      // btnBack_Click
      //--------------------------------------------------------------------
      private void btnBack_Click(object sender, EventArgs e)
      {
         ChangeWizardPage ( WizardAction.Prev );
      }

      //--------------------------------------------------------------------
      // btnNext_Click
      //--------------------------------------------------------------------
      private void btnNext_Click(object sender, EventArgs e)
      {
          if ( ValidatePage() )
          {
             ChangeWizardPage ( WizardAction.Next );
          }
          else
          {
             if ( _vControl!=null) _vControl.Focus();
          }
      }

      //--------------------------------
      // Wizard State Machine Constants
      //--------------------------------
      int numPages       = 9;
      int currentPage    = pageGeneral;
      Panel currentPanel = null;
      
		enum WizardAction
		{
			Next = 0,
			Prev = 1
	   };
	   
	   // panelConstants
      private const int pageGeneral           = 0;
      private const int pageAuditMode         = 1;
      private const int pageDatabases         = 2;
      private const int pageSystemDatabases   = 3;
      private const int pageAudit             = 4;
      private const int pageDmlFilters        = 5;
      private const int pageTrustedUsers = 6;
      private const int pageSummary           = 7;
      private const int pageAllDBsAdded       = 8;
      private const int pageCantLoadDatabases = 9;
	   
	   

      //--------------------------------------------------------------------
      // ChangeWizardPage - Move forward or backwards in the wizard
      //--------------------------------------------------------------------
      private void ChangeWizardPage( WizardAction direction )
      {
         // Change Page
         if ( direction == WizardAction.Next )
         {
            if ( currentPage == pageGeneral )
            {
               bool loaded = LoadDatabases();
               if ( ! loaded )
               {
                  currentPage = pageCantLoadDatabases;
               }
               else
               {
                  loaded = LoadSystemDatabases();
                  if ( ! loaded )
                  {
                     currentPage = pageGeneral;
                  }
                  else
                  {
                     if (listDatabases.Items.Count == 0 && listSystemDatabases.Items.Count == 0 )
                     {
                        currentPage = pageAllDBsAdded;
                     }
                     else
                     {
                        currentPage = pageAuditMode;
                     }
                  }
               }
            }
            else if ( currentPage == pageDatabases )
            {
               if ( radioTypical.Checked )
                  currentPage = pageSummary;
               else
                  currentPage = pageSystemDatabases;
            }
            else if ( currentPage == pageAudit)
            {
               if ( chkAuditDML.Checked || chkAuditSELECT.Checked)
                  currentPage = pageDmlFilters;
               else
                  currentPage = pageTrustedUsers;
            }               
            else if ( currentPage < (numPages-1) )
            {
               currentPage ++;
            }
         }
         else // Prev
         {
            if ( currentPage == pageAllDBsAdded)
            {
               currentPage = pageAuditMode;
            }
            else if ( currentPage == pageCantLoadDatabases)
            {
               currentPage = pageGeneral;
            }
            else if ( currentPage == pageSummary)
            {
               if ( radioTypical.Checked )
               {
                  currentPage = pageDatabases;
               }
               else
               {
                  currentPage = pageTrustedUsers;
               }
            }
            else if (currentPage == pageTrustedUsers)
            {
               if (chkAuditDML.Checked || chkAuditSELECT.Checked)
                  currentPage = pageDmlFilters;
               else
                  currentPage = pageAudit;
            }
            else if (currentPage > 0)
            {
               currentPage--;
            }
         }
         
         SetCurrentPage();
      }
      
      //--------------------------------------------------------------------
      // SetCurrentPage - Make sure the current page is visible and buttons 
      //                  are enabled/disabled appropriately
      //--------------------------------------------------------------------
      private void SetCurrentPage()
      {
         Panel oldPanel = currentPanel;
         
         if ( currentPage == pageGeneral )
         {
            currentPanel = this.panelGeneral;
            bool flag = EnableFirstPageButtons();
            SetButtonState( false,    /* back   */
                            flag,     /* next   */
                            false );  /* finish */
         }
         else if ( currentPage == pageAuditMode )
         {
            currentPanel = this.panelAuditMode;
            SetButtonState( true,   /* back   */
                            true,   /* next   */
                            false); /* finish */
         }
         else if ( currentPage == pageDatabases )
         {
            currentPanel = this.panelDatabases;
            SetButtonState( true,   /* back   */
                            true,   /* next   */
                            false); /* finish */
         }
         else if ( currentPage == pageSystemDatabases )
         {
            currentPanel = this.panelSystemDatabases;
            SetButtonState( true,   /* back   */
                            true,   /* next   */
                            false); /* finish */
         }
         else if ( currentPage == pageAudit )
         {
            currentPanel = this.panelAudit;
            SetButtonState( true,    /* back   */
                            true,    /* next   */
                            false ); /* finish */
         }
         else if ( currentPage == pageDmlFilters )
         {
            currentPanel = this.panelDmlFilters;
            SetButtonState( true,   /* back   */
                            true,  /* next   */
                            false ); /* finish */
         }
         else if (currentPage == pageTrustedUsers)
         {
            currentPanel = this._pnlTrustedUsers;
            SetButtonState(true,   /* back   */
                            true,  /* next   */
                            false); /* finish */
         }
         else if (currentPage == pageSummary)
         {
            currentPanel = this.panelSummary;
            SetButtonState( true,   /* back   */
                            false,  /* next   */
                            true ); /* finish */
         }
         else if ( currentPage == pageAllDBsAdded )
         {
            currentPanel = this.panelAllDBsAdded;
            SetButtonState( true,   /* back   */
                            false,  /* next   */
                            false ); /* finish */
         }
         else if ( currentPage == pageCantLoadDatabases )
         {
            currentPanel = this.panelCantLoadDatabases;
            SetButtonState( comboServer.Enabled, // if they can change servers, they can go back
                            false,  /* next   */
                            false ); /* finish */
         }
         else
         {
            //internal error
         }
         
         if ( (currentPanel != null) && (currentPanel!=oldPanel) )
         {  
            if ( oldPanel != null )
               oldPanel.Enabled = false;
         
            currentPanel.Enabled = true;       
            currentPanel.BringToFront();
            
            // set focus
            if ( currentPage == pageGeneral )
            {
               if (comboServer.Enabled )
                  comboServer.Focus();
               else
               {
                  btnNext.Focus();
               }
            }
            if ( currentPage == pageAuditMode )
            {
               if (radioTypical.Checked )
                  radioTypical.Focus();
               else
                  radioCustom.Focus();
            }
            else if ( currentPage == pageDatabases )
            {
               if ( listDatabases.Items.Count > 0)
               {
                  listDatabases.Focus();
                  listDatabases.SelectedIndex = 0;
               }
               else
               {
                  btnNext.Focus();
               }
            }
            else if ( currentPage == pageSystemDatabases )
            {
               if ( listSystemDatabases.Items.Count > 0)
               {
                  listSystemDatabases.Focus();
                  listSystemDatabases.SelectedIndex = 0;
               }
               else
               {
                  btnNext.Focus();
               }
            }
            else if ( currentPage == pageAudit )
            {
               chkAuditSecurity.Focus();
            }
            else if ( currentPage == pageDmlFilters )
            {
               if(radioAllDML.Checked)
                  radioAllDML.Focus() ;
               else
                  radioSelectedDML.Focus() ;
            }
            else if (currentPage == pageTrustedUsers)
            {
               btnAddTrusted.Focus();
            }
            else if (currentPage == pageSummary)
            {
               btnFinish.Focus();
            }
         }
         
         if ( btnNext.Enabled )
         {
            this.AcceptButton = btnNext;
         }
         else if ( btnFinish.Enabled )
         {
            this.AcceptButton = btnFinish;
         }
      }
      
      //--------------------------------------------------------------------
      // SetButtonState - Set back,next, finish enabled state
      //--------------------------------------------------------------------
      private void SetButtonState( bool back, bool next, bool finish )
      {
         btnBack.Enabled   = back;
         btnNext.Enabled   = next;
         btnFinish.Enabled = finish;
         
         if ( finish )
            this.AcceptButton = btnFinish;
         else
            this.AcceptButton = btnNext;
      }
      
      #endregion

      #region Validate Pages
      
      //--------------------------------------------------------------------
      // ValidatePage - Simple validation done as users switches pages with
      //                back and next. More extensive validation is done
      //                after Finish is pressed.
      //--------------------------------------------------------------------
      private bool ValidatePage()
      {
         _vControl = null;
         
         //-------------------------------------------------
         // Page 1 - Server Name, Database and Description
         //-------------------------------------------------
         if ( currentPage == pageGeneral )
         {
            // validate server
            if ( comboServer.Enabled )
            {
               if ( comboServer.Text == "" )
               {
                  ErrorMessage.Show( this.Text,
                                     UIConstants.Error_ServerRequired );
                                     
                  _vControl = comboServer;    
                  return false;
               }
            
               int ndx = comboServer.FindString( comboServer.Text );
               if ( -1 == ndx)
               {
                  ErrorMessage.Show( this.Text,
                                     UIConstants.Error_ServerNotRegistered );

                  _vControl = comboServer;    
                  return false;
               }
               
               // get server id
               _server = ServerRecord.GetServer( Globals.Repository.Connection,
                                                          comboServer.Text );
               if (_server == null)
               {
                  ErrorMessage.Show( this.Text,
                                     UIConstants.Error_ServerRequired,
                                     Globals.Repository.GetLastError());
                  _vControl = comboServer;    
                  return false;
               }
               else
               {
                  LoadAuditedActivityDefaults();
               }
            }
         }
         //-------------------------------------------------
         // Page 2 - User Databases
         //-------------------------------------------------
         else if ( currentPage == pageDatabases )
         {
            if ( radioTypical.Checked )
            {
               if ( listDatabases.CheckedItems.Count == 0 )
               {
                  ErrorMessage.Show( this.Text,
                                       UIConstants.Error_NoDatabasesSelected,
                                       Globals.Repository.GetLastError());
                  _vControl = listDatabases;    
                  return false;
               }
            }
         
         
            if ( listDatabases.CheckedItems.Count > 100 )
            {
               ErrorMessage.Show( this.Text,
                                    UIConstants.Error_TooManyDatabasesSelected,
                                    Globals.Repository.GetLastError());
               _vControl = listDatabases;    
               return false;
            }
         }
         //-------------------------------------------------
         // Page 3 - System Databases
         //-------------------------------------------------
         else if ( currentPage == pageSystemDatabases )
         {
            if ( listDatabases.CheckedItems.Count == 0 &&
                 listSystemDatabases.CheckedItems.Count == 0 )
            {
               ErrorMessage.Show( this.Text,
                                    UIConstants.Error_NoDatabasesSelected,
                                    Globals.Repository.GetLastError());
               _vControl = listSystemDatabases;    
               return false;
            }
         }
         //-------------------------------------------------
         // Page 4 - Audit Settings
         //-------------------------------------------------
         else if ( currentPage == pageAudit )
         {
            // make sure something checked
            if ( ! chkAuditSecurity.Checked &&
                  ! chkAuditDDL.Checked &&
                  ! chkAuditAdmin.Checked &&
                  ! chkAuditDML.Checked &&
                  ! chkAuditSELECT.Checked )
            {
               ErrorMessage.Show( this.Text,
                                    UIConstants.Error_MustSelectOneAuditOption );
               _vControl = chkAuditSecurity;    
               
               return false;
            }
         }
         //-------------------------------------------------
         // Page 5 - DML Filters
         //-------------------------------------------------
         else if ( currentPage == pageDmlFilters )
         {
            if ( chkAuditDML.Checked || chkAuditSELECT.Checked )
            {
               // make sure something selected for auditing
               if (  radioSelectedDML.Checked
                        && ! chkAuditUserTables.Checked
                        && ! chkAuditSystemTables.Checked
                        && ! chkAuditStoredProcedures.Checked
                        && ! chkAuditOther.Checked )
               {
                  ErrorMessage.Show( this.Text,
                                    UIConstants.Error_MustSelectOneAuditObject );
                  _vControl = radioSelectedDML;    
                  return false;
               }
            }
         }
         
         return true;
      }
      
      #endregion
      
      #region General Page Handlers

      //-------------------------------------------------------------
      // LoadServerDropDown
      //--------------------------------------------------------------
      private void LoadServerDropDown()
      {
         Cursor = Cursors.WaitCursor;
         comboServer.BeginUpdate();
      
         comboServer.Items.Clear();
         
         if ( Globals.Repository.Connection == null )
         {
            Debug.Write( "Assertion - Failure to initialize database connection before loading view" );
            goto cleanup;
         }

         List<ServerRecord> serverList ;

         serverList = ServerRecord.GetServers( Globals.Repository.Connection, true );

			if ((serverList != null) && (serverList.Count != 0)) 
			{
				foreach (ServerRecord config in serverList) 
				{
				   comboServer.Items.Add( config.Instance );
            }
         
            // select first server
            if ( comboServer.Items.Count != 0 )
            {
               comboServer.Text = comboServer.Items[0].ToString();
            }
         }
         else
         {
            ErrorMessage.Show( this.Text,
                               UIConstants.Error_CantLoadServers,
                               Globals.Repository.GetLastError());
         }

cleanup:         
         comboServer.EndUpdate();
         Cursor = Cursors.Default;
      }
      
      
      //-------------------------------------------------------------
      // LoadDatabases
      //--------------------------------------------------------------
      private bool LoadDatabases()
      {
         bool loaded = true;
         
           
         if (_server != null) _sqlServer.CloseConnection();

         Cursor = Cursors.WaitCursor;
         
         if ( _isUserDatabaseLoaded )
         {         
            listDatabases.Items.Clear();
            _isUserDatabaseLoaded = false;
            listSystemDatabases.Items.Clear();
            _isSystemDatabaseLoaded = false;
         }
         
         if ( Globals.Repository.Connection == null )
         {
            Debug.Write( "Assertion - Failure to initialize database connection before loading view" );
            loaded = false;
            goto cleanup;
         }
         

         ICollection dbList    = null;

         // load database list via agent (if deployed)
         if ( _server.IsDeployed && _server.IsRunning )
         {
            string url = "";
            try
            {
               url = String.Format( "tcp://{0}:{1}/{2}",
                                    Globals.SQLcomplianceConfig.Server, 
                                    Globals.SQLcomplianceConfig.ServerPort,
                                    typeof(AgentManager).Name );
               AgentManager manager = (AgentManager)Activator.GetObject(
                  typeof(AgentManager), 
                  url );
                  
               dbList = manager.GetRawUserDatabases( _server.Instance );
            }
            catch (Exception ex)
            {
               ErrorLog.Instance.Write( ErrorLog.Level.Verbose,
                                        String.Format( "LoadDatabases: URL: {0} Instance {1}", url, _server.Instance ),
                                        ex,
                                        ErrorLog.Severity.Warning );
               dbList = null;
            }
         }
         
         // try straight connection to SQL Server if agent connection failed
         if ( dbList == null )
         {
            if ( _sqlServer.OpenConnection( _server.Instance ) )
            {
               dbList = RawSQL.GetUserDatabases(_sqlServer.Connection);
            }
         }

			if ((dbList != null) )
         {
            if((dbList.Count != 0)) 
			   {
				   foreach (RawDatabaseObject db in dbList) 
				   {
				      // only load if this database doesnt already exist in our other DB
				      // potentially slow but we are using selects from two sql servers
				      DatabaseRecord dbrec = new DatabaseRecord();
				      dbrec.Connection = Globals.Repository.Connection;
   				   
				      if ( ! dbrec.Read( _server.Instance, db.name ) &&
				         ! (_server.IsSqlSecureDb && SQLRepository.IsSQLsecureOwnedDB(db.name)))
				      {
				         listDatabases.Items.Add(db );
				      }
               }
            }
         }
         else
         {
            //ErrorMessage.Show( this.Text,
            //                   UIConstants.Error_CantLoadDatabase );
            loaded = false;
         }

cleanup:
         _isUserDatabaseLoaded = loaded;
         Cursor = Cursors.Default;
         
         return loaded;
      }

      //-------------------------------------------------------------
      // LoadSystemDatabases
      //--------------------------------------------------------------
      private bool LoadSystemDatabases()
      {
         bool loaded = true;
         // Only load first time server is switched
         if ( _isSystemDatabaseLoaded )
            return true;
         
         Cursor = Cursors.WaitCursor;
         
         listSystemDatabases.Items.Clear();
         
         if ( Globals.Repository.Connection == null )
         {
            Debug.Write( "Assertion - Failure to initialize database connection before loading view" );
            loaded = false;
            goto cleanup;
         }
         
         ICollection dbList = null;
         
         // load database list via agent (if deployed)
         if ( _server.IsDeployed && _server.IsRunning )
         {
            try
            {
               string url = String.Format( "tcp://{0}:{1}/{2}",
                                             Globals.SQLcomplianceConfig.Server, 
                                             Globals.SQLcomplianceConfig.ServerPort,
                                             typeof(AgentManager).Name );
               AgentManager manager = (AgentManager)Activator.GetObject(
                  typeof(AgentManager), 
                  url );
                  
               dbList = manager.GetRawSystemDatabases( _server.Instance );
            }
            catch (Exception )
            {
               dbList = null;
            }
         }
         
         // try straight connection to SQL Server if agent connection failed
         if ( dbList == null )
         {
            if ( _sqlServer.OpenConnection( _server.Instance ) )
            {
               dbList = RawSQL.GetSystemDatabases(_sqlServer.Connection);
            }
         }

			if ((dbList != null) && (dbList.Count != 0)) 
			{
				foreach (RawDatabaseObject db in dbList) 
				{
				   // only load if this database doesnt already exist in our other DB
				   // potentially slow but we are using selects from two sql servers
				   DatabaseRecord dbrec = new DatabaseRecord();
				   dbrec.Connection = Globals.Repository.Connection;
				   
				   if ( ! dbrec.Read( _server.Instance, db.name ) )
				   {
				      listSystemDatabases.Items.Add(db );
				   }
            }
         }
         else
         {
            loaded = false;
            //ErrorMessage.Show( this.Text,
            //                   UIConstants.Error_CantLoadDatabase );
         }

cleanup:         
         _isSystemDatabaseLoaded = true;
         Cursor = Cursors.Default;
         
         return loaded;
      }

      //--------------------------------------------------------------------
      // comboServerAndDatabase_TextChanged
      //--------------------------------------------------------------------
      private void comboServerAndDatabase_TextChanged(object sender, EventArgs e)
      {
         bool flag = EnableFirstPageButtons();
         btnNext.Enabled   = flag;
         btnFinish.Enabled = flag;
      }
      
      //--------------------------------------------------------------------
      // EnableFirstPageButtons
      //--------------------------------------------------------------------
      private bool EnableFirstPageButtons()
      {
         bool flag = false;
         
         if ( comboServer.Text.Trim()!="")
         {
            flag = true;
         }
         
         return flag;
      }
      
      //--------------------------------------------------------------------
      // chkCaptureAll_CheckedChanged
      //--------------------------------------------------------------------
      private void chkCaptureSQL_CheckedChanged(object sender, EventArgs e)
      {
         if ( _isLoaded && chkCaptureSQL.Checked )
         {
            ErrorMessage.Show( this.Text,
                                 UIConstants.Warning_CaptureAll,
                                 "",
                                 MessageBoxIcon.Warning );
         }
      }

      #endregion
      
      #region Audit Settings Page Handlers
      
      //--------------------------------------------------------------------
      // LoadAuditedActivityDefaults
      //--------------------------------------------------------------------
      private void
         LoadAuditedActivityDefaults()
      {
	      chkAuditDDL.Checked            = Globals.SQLcomplianceConfig.AuditDDL;
	      chkAuditSecurity.Checked       = Globals.SQLcomplianceConfig.AuditSecurity;
	      chkAuditAdmin.Checked          = Globals.SQLcomplianceConfig.AuditAdmin;
	      chkAuditSELECT.Checked         = Globals.SQLcomplianceConfig.AuditSELECT;
	      chkAuditDML.Checked            = Globals.SQLcomplianceConfig.AuditDML;
	      
         switch(Globals.SQLcomplianceConfig.AuditAccessCheck)
         {
            case AccessCheckFilter.FailureOnly:
               chkFilterOnAccess.Checked = true ;
               _rbAccessFailed.Checked = true ;
               break ;
            case AccessCheckFilter.SuccessOnly:
               chkFilterOnAccess.Checked = true ;
               _rbAccessPassed.Checked = true ;
               break ;
            case AccessCheckFilter.NoFilter:
               chkFilterOnAccess.Checked = false ;
               break ;
         }

         if (chkAuditDML.Checked && ServerRecord.CompareVersions(_server.AgentVersion, "3.5") >= 0)
         {
            chkCaptureTrans.Enabled = true;
         }
         else
         {
            chkCaptureTrans.Enabled = false;
            chkCaptureTrans.Checked = false;
         }

         if (CoreConstants.AllowCaptureSql)
         {
            chkCaptureSQL.Enabled = true;
            chkCaptureSQL.Checked = false;

            if (chkAuditDML.Checked || chkAuditSELECT.Checked)
               chkCaptureSQL.Enabled = true;
            else
               chkCaptureSQL.Enabled = false;
         }
         else
         {
            chkCaptureSQL.Enabled = false;
            chkCaptureSQL.Checked = false;
         }
	      
         _isLoaded = true;
      }
      
      #endregion

      #region Help
      
      //--------------------------------------------------------------------
      // Form_DatabaseNew_HelpRequested
      //--------------------------------------------------------------------
      private void
         Form_DatabaseNew_HelpRequested(
            object                             sender,
            HelpEventArgs hlpevent
         )
      {
         string helpTopic = "";
      
         if ( currentPage == pageGeneral ) // general
            helpTopic = HelpAlias.SSHELP_Form_DatabaseNew_General;
         else if ( currentPage == pageAuditMode ) // user databases
            helpTopic = HelpAlias.SSHELP_Form_DatabaseNew_AuditLevel;
         else if ( currentPage == pageDatabases ) // user databases
            helpTopic = HelpAlias.SSHELP_Form_DatabaseNew_User;
         else if ( currentPage == pageSystemDatabases ) // system databases
            helpTopic = HelpAlias.SSHELP_Form_DatabaseNew_System;
         else if ( currentPage == pageAudit ) // audit settings
            helpTopic = HelpAlias.SSHELP_Form_DatabaseNew_Activities;
         else if ( currentPage == pageDmlFilters ) // objects
            helpTopic = HelpAlias.SSHELP_Form_DatabaseNew_Objects;
         else if (currentPage == pageTrustedUsers) // summary
            helpTopic = HelpAlias.SSHELP_Form_DatabaseNew_TrustedUsers;
         else if (currentPage == pageSummary) // summary
            helpTopic = HelpAlias.SSHELP_Form_DatabaseNew_Summary;
      
		   if (helpTopic != "" ) HelpAlias.ShowHelp(this,helpTopic );
			hlpevent.Handled = true;
      }
      
      private void linkTypical_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
      {
         HelpAlias.ShowHelp(this, HelpAlias.SSHELP_Form_DatabaseNew_AuditLevel);
      }
      
      
      #endregion

      #region Database Panel Handlers
      
      private void btnSelectAll_Click(object sender, EventArgs e)
      {
         for ( int i=0; i<listDatabases.Items.Count; i++ )
         {
            listDatabases.SetItemChecked(i, true );
         }
      }

      private void btnUnselectAll_Click(object sender, EventArgs e)
      {
         for ( int i=0; i<listDatabases.Items.Count; i++ )
         {
            listDatabases.SetItemChecked(i, false );
         }
      }

      private void btnSystemSelectAll_Click(object sender, EventArgs e)
      {
         for ( int i=0; i<listSystemDatabases.Items.Count; i++ )
         {
            listSystemDatabases.SetItemChecked(i, true );
         }
      }

      private void btnSystemUnselectAll_Click(object sender, EventArgs e)
      {
         for ( int i=0; i<listSystemDatabases.Items.Count; i++ )
         {
            listSystemDatabases.SetItemChecked(i, false );
         }
      }
      
      #endregion

      private void chkAuditDML_CheckedChanged(object sender, EventArgs e)
      {
         if ((chkAuditDML.Checked || chkAuditSELECT.Checked) && CoreConstants.AllowCaptureSql)
            chkCaptureSQL.Enabled = true;
         else
            chkCaptureSQL.Enabled = false;

         if (chkAuditDML.Checked && ServerRecord.CompareVersions(_server.AgentVersion, "3.5") >= 0)
            chkCaptureTrans.Enabled = true;
         else
            chkCaptureTrans.Enabled = false;
      }

      private void Form_DatabaseNew_Load(object sender, EventArgs e)
      {
         this.panelGeneral.Enabled = false;
         this.panelDatabases.Enabled = false;
         this.panelSystemDatabases.Enabled = false;
         this.panelAudit.Enabled = false;
         this.panelDmlFilters.Enabled = false;
			
			// server combo box
			if ( _server != null)
			{
			   if ( _allowOverride ) LoadServerDropDown();
			   
			   comboServer.Text = _server.Instance;
			   comboServer.Enabled = _allowOverride;
			}
			else
			{
			   comboServer.Enabled = true;
			   LoadServerDropDown();
			}
			
	      currentPage = 0;
			SetCurrentPage();
			
         _sqlServer = new SQLDirect();
         
         // Audited Activity
         LoadAuditedActivityDefaults();
         
         // Audited Object Defaults
         if ( Globals.SQLcomplianceConfig.AuditDmlAll ) 
         {
            radioAllDML.Checked = true;
         }
         else
         {
            radioSelectedDML.Checked = true;
         }     
         SetFilterState();    
         
         chkAuditUserTables.Checked       = ( Globals.SQLcomplianceConfig.AuditUserTables == 1 );
         chkAuditStoredProcedures.Checked = Globals.SQLcomplianceConfig.AuditStoredProcedures;
         chkAuditSystemTables.Checked     = Globals.SQLcomplianceConfig.AuditSystemTables;
         chkAuditOther.Checked            = Globals.SQLcomplianceConfig.AuditDmlOther;
         
         _isLoaded = true;
      }

      private void radioAllDML_CheckedChanged(object sender, EventArgs e)
      {
         SetFilterState();
      }
      
      private void SetFilterState()
      {
         chkAuditUserTables.Enabled       = radioSelectedDML.Checked;
         chkAuditSystemTables.Enabled     = radioSelectedDML.Checked;
         chkAuditStoredProcedures.Enabled = radioSelectedDML.Checked;
         chkAuditOther.Enabled            = radioSelectedDML.Checked;
      }

      private void btnAddNewUserDatabase_Click(object sender, EventArgs e)
      {
         string db = textNewUserDatabase.Text.Trim();
         
         // if database already listed; select it
         for ( int i=0; i < listDatabases.Items.Count; i ++ )
         {
            RawDatabaseObject rawDB = (RawDatabaseObject)listDatabases.Items[i];
            if ( rawDB.name == db )
            {
               listDatabases.BeginUpdate();
               listDatabases.SetSelected(i,true);
               listDatabases.SetItemChecked(i,true);
               listDatabases.EndUpdate();
               
               textNewUserDatabase.Text   = "";
               btnAddNewUserDatabase.Enabled = false;
               
               return;
            }
         }
         
         // add database
         RawDatabaseObject newRawDB = new RawDatabaseObject();
         newRawDB.name = db;
         newRawDB.dbid = -1;
         int n = listDatabases.Items.Add( newRawDB, true );
         listDatabases.SetSelected(n,true);
         
         // clear text field; disable button
         textNewUserDatabase.Text   = "";
         btnAddNewUserDatabase.Enabled = false;
      }

      private void textNewUserDatabase_TextChanged(object sender, EventArgs e)
      {
         string db = textNewUserDatabase.Text.Trim();
         btnAddNewUserDatabase.Enabled = (db.Length != 0);
      }

      private void btnAddNewSystemDatabase_Click(object sender, EventArgs e)
      {
         string db = textNewSystemDatabase.Text.Trim();
         
         // if database already listed; select it
         for ( int i=0; i < listDatabases.Items.Count; i ++ )
         {
            RawDatabaseObject rawDB = (RawDatabaseObject)listSystemDatabases.Items[i];
            if ( rawDB.name == db )
            {
               listSystemDatabases.BeginUpdate();
               listSystemDatabases.SetSelected(i,true);
               listSystemDatabases.SetItemChecked(i,true);
               listSystemDatabases.EndUpdate();
               
               textNewSystemDatabase.Text   = "";
               btnAddNewSystemDatabase.Enabled = false;
               
               return;
            }
         }
         
         // add database
         RawDatabaseObject newRawDB = new RawDatabaseObject();
         newRawDB.name = db;
         newRawDB.dbid = -1;
         int n = listSystemDatabases.Items.Add( newRawDB, true );
         listSystemDatabases.SetSelected(n,true);
         
         // clear text field; disable button
         textNewSystemDatabase.Text   = "";
         btnAddNewSystemDatabase.Enabled = false;
      }

      private void textNewSystemDatabase_TextChanged(object sender, EventArgs e)
      {
         string db = textNewSystemDatabase.Text.Trim();
         btnAddNewSystemDatabase.Enabled = (db.Length != 0);
      }

      private void Click_chkFilterOnAccess(object sender, EventArgs e)
      {
         if(chkFilterOnAccess.Checked)
         {
            _rbAccessFailed.Enabled = true ;
            _rbAccessPassed.Enabled = true ;
         }
         else
         {
            _rbAccessFailed.Enabled = false ;
            _rbAccessPassed.Enabled = false ;
         }
      }

      private void btnAddTrusted_Click(object sender, EventArgs e)
      {
         Form_PrivUser frm = new Form_PrivUser(_server.Instance, false);

         frm.UseAgentEnumeration = _server.IsDeployed;

         //frm.MainForm = this.mainForm;                                                      
         if (DialogResult.OK == frm.ShowDialog())
         {
            lstTrustedUsers.BeginUpdate();

            lstTrustedUsers.SelectedItems.Clear();

            foreach (ListViewItem itm in frm.listSelected.Items)
            {
               bool found = false;
               foreach (ListViewItem s in lstTrustedUsers.Items)
               {
                  if (itm.Text == s.Text)
                  {
                     found = true;
                     s.Selected = true;
                     break;
                  }
               }

               if (!found)
               {
                  ListViewItem newItem = new ListViewItem(itm.Text);
                  newItem.Tag = itm.Tag;
                  newItem.ImageIndex = itm.ImageIndex;
                  lstTrustedUsers.Items.Add(newItem);
               }
            }

            lstTrustedUsers.EndUpdate();

            if (lstTrustedUsers.Items.Count > 0)
            {
               lstTrustedUsers.TopItem.Selected = true;
               btnRemoveTrusted.Enabled = true;
            }
         }
      }

      private void btnRemoveTrusted_Click(object sender, EventArgs e)
      {
         lstTrustedUsers.BeginUpdate();

         int ndx = lstTrustedUsers.SelectedIndices[0];

         foreach (ListViewItem priv in lstTrustedUsers.SelectedItems)
         {
            priv.Remove();
         }

         lstTrustedUsers.EndUpdate();

         // reset selected item
         if (lstTrustedUsers.Items.Count != 0)
         {
            lstTrustedUsers.Focus();
            if (ndx >= lstTrustedUsers.Items.Count)
            {
               lstTrustedUsers.Items[lstTrustedUsers.Items.Count - 1].Selected = true;
            }
            else
               lstTrustedUsers.Items[ndx].Selected = true;
         }
         else
         {
            btnRemoveTrusted.Enabled = false;
         }
      }

      private string GetTrustedUserProperty()
      {
         int count = 0;

         UserList ul = new UserList();

         foreach (ListViewItem vi in lstTrustedUsers.Items)
         {
            count++;
            if (vi.ImageIndex == (int)AppIcons.Img16.Role)
            {
               ul.AddServerRole(vi.Text, vi.Text, (int)vi.Tag);
            }
            else
            {
               ul.AddLogin(vi.Text, (byte[])vi.Tag);
            }
         }

         return (count == 0) ? "" : ul.ToString();
      }

      private void linkLblHelpBestPractices_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
      {
         HelpAlias.ShowHelp(this, HelpAlias.SSHELP_AuditingBestPractices);
      }
	}
}
