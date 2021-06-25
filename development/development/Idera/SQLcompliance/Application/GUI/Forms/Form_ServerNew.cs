//--------------------------------------------------------------------
// Form_ServerNew
//
// Wizard for registering a new SQLServer with SQLsecure
//
// (c) Copyright 2004; Idera, inc. ALL RIGHTS RESERVED
//--------------------------------------------------------------------

using System ;
using System.Collections ;
using System.Diagnostics;
using System.IO;
using System.Net ;
using System.Reflection;
using System.Windows.Forms ;
using Idera.SQLcompliance.Application.GUI.Controls ;
using Idera.SQLcompliance.Application.GUI.Helper ;
using Idera.SQLcompliance.Application.GUI.Properties ;
using Idera.SQLcompliance.Application.GUI.SQL ;
using Idera.SQLcompliance.Core ;
using Idera.SQLcompliance.Core.Agent ;
using Idera.SQLcompliance.Core.Collector ;
using Idera.SQLcompliance.Core.Event ;
using Idera.SQLcompliance.Core.Service ;
using Idera.SQLcompliance.Core.Status ;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
	/// <summary>
	/// Summary description for Form_ServerNew.
	/// </summary>
	public partial class Form_ServerNew : Form
	{
	   #region Window Properties

      private bool _desiredDeploymentSuccess = true ;

      public bool DesiredDeploymentSuccess
      {
         get { return _desiredDeploymentSuccess ; }
      }
		
		#endregion
		
		#region Constructor / Dispose

		public Form_ServerNew()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

         this.Icon = Resources.SQLcompliance_product_ico;
			
         this.panelServer.Enabled = false;
         this.panelAgentDeploy.Enabled = false;
         this.panelAgentService.Enabled = false;
         this.panelAgentTrace.Enabled = false;
         this.panelAudit.Enabled = false;
         this.panelAuditedUsers.Enabled = false;
         this.panelAuditedUserActivity.Enabled = false;
         this.panelSummary.Enabled = false;

         if ( LicenseAllowsMoreInstances() )
         {			
            m_licenseExceeded = false;
            
            // Audited Activity
            LoadGlobalDefaults();
            
            // Privileged User Defaults
			   rbAuditUserAll.Checked              = Globals.SQLcomplianceConfig.AuditUserAll;
			   rbAuditUserSelected.Checked         = ! Globals.SQLcomplianceConfig.AuditUserAll;
            grpAuditUserActivity.Enabled        = ! rbAuditUserAll.Checked;

            chkAuditUserLogins.Checked          = Globals.SQLcomplianceConfig.AuditUserLogins;
            chkAuditUserFailedLogins.Checked    = Globals.SQLcomplianceConfig.AuditUserFailedLogins;
            chkAuditUserDDL.Checked             = Globals.SQLcomplianceConfig.AuditUserDDL;
            chkAuditUserSecurity.Checked        = Globals.SQLcomplianceConfig.AuditUserSecurity;
            chkAuditUserAdmin.Checked           = Globals.SQLcomplianceConfig.AuditUserAdmin;
            chkAuditUserDML.Checked             = Globals.SQLcomplianceConfig.AuditUserDML;
            chkAuditUserUDE.Checked = false ;
            chkAuditUserSELECT.Checked          = Globals.SQLcomplianceConfig.AuditUserSELECT;
            switch(Globals.SQLcomplianceConfig.AuditUserAccessCheck)
            {
               case AccessCheckFilter.FailureOnly:
                  chkUserAccessCheckFilter.Checked = true ;
                  _rbUserAuditFailed.Checked = true ;
                  break ;
               case AccessCheckFilter.SuccessOnly:
                  chkUserAccessCheckFilter.Checked = true ;
                  _rbUserAuditPassed.Checked = true ;
                  break ;
               case AccessCheckFilter.NoFilter:
                  chkUserAccessCheckFilter.Checked = false ;
                  break ;
            }

			   currentPage = pageServer;
			   SetCurrentPage();
			   this.AcceptButton = btnNext;
   			
            this.ActiveControl = textSQLServer;
         }
         else
         {
            // license violated
            m_licenseExceeded = true;
			   currentPage = pageLicenseLimit;
			   SetCurrentPage();
         }
         
         
         m_isLoaded = true; 
		}

		#endregion


      #region Properties

      private ServerRecord m_srv = null;
      public  ServerRecord srv
      {
         get { return m_srv; }
      }
      
      private bool         m_isClustered = false;
      private bool         m_isLoaded = false;
      private ServerRecord m_existingServer = null;
      private ServerRecord m_existingAuditedServer = null;
      
      private bool         m_convertingNonAudited    = false;
      private bool         m_alreadyDeployed         = false;
      private bool         m_alreadyDeployedManually = false;
      private bool         m_repositoryComputer      = false;
      
      private string       m_computer = "";
      private string       m_instance = "";
      
      private bool         m_licenseExceeded = false;
      
      private string       m_lastCheckedName = "";

      
      #endregion

      #region Page Navigation
      
      //--------------------------------------------------------------------
      // btnNext_Click - Next button; move forward a page in the wizard
      //--------------------------------------------------------------------
      private void btnNext_Click(object sender, EventArgs e)
      {
          if ( ValidatePage( currentPage ) )
          {
             ChangeWizardPage ( WizardAction.Next );
          }
      }

      //--------------------------------------------------------------------
      // btnBack_Click - Back button; move back a page in the wizard
      //--------------------------------------------------------------------
      private void btnBack_Click(object sender, EventArgs e)
      {
         ChangeWizardPage( WizardAction.Prev );
      }
      

      //--------------------------------
      // Wizard State Machine Constants
      //--------------------------------
      int numPages    = 14;
      int currentPage = pageServer;
      Panel currentPanel = null;
		enum WizardAction
		{
			Next = 0,
			Prev = 1
	   };
	   
	   private const int pageServer               = 0;
	   private const int pageExistingDatabase     = 1;
	   private const int pageAgentDeploy          = 2;
	   private const int pageAgentService         = 3;
	   private const int pageAgentTrace           = 4;
	   private const int pageAudit                = 5;
      private const int pageAuditedUsers         = 6;
	   private const int pageAuditedUserActivity  = 7;
	   private const int pagePermissions          = 8;
	   private const int pageSummary              = 9;
	   private const int pageLicenseLimit         = 10;
	   private const int pageIncompatibleDatabase = 11;
	   private const int pageCluster              = 12;
	   private const int pageCantAudit2005        = 13;
      private const int pageCantConnect          = 14;
	   
	   bool m_existingDatabase = false;
	   bool m_compatibleSchema = false;

      //--------------------------------------------------------------------
      // ChangeWizardPage - Move forward or backwards in the wizard
      //--------------------------------------------------------------------
      private void ChangeWizardPage( WizardAction direction )
      {
         // Change Page
         if ( direction == WizardAction.Next )
         {
            // special navigation logic
            if ( currentPage == pageServer && ! m_alreadyDeployed)
            {
               currentPage = pageCluster;
            }
            else if ( currentPage == pageServer // && m_alreadyDeployed
                      || currentPage == pageCluster )
            {
               m_existingDatabase = DoesDatabaseExist( out m_compatibleSchema );
               
               if ( m_existingDatabase )
               {
                  if ( m_compatibleSchema )
                     currentPage = pageExistingDatabase;
                  else
                     currentPage = pageIncompatibleDatabase;
               }
               else
               {
                  currentPage = pageAgentDeploy;
               }
            }
            else if (currentPage == pageAgentDeploy)
            {
               if ( ! radioButtonDeployNow.Checked )
               {
                  currentPage = pageAudit;
               }
               else
               {
                  currentPage ++;
               }
                  
               // if trying to deploy to a SQL Server now and the collection server
               // is hosted on a 2005 SQL Server, we connect and see if the 
               // the target SQL Server is 2000 or 2005
               // SQL Server 2005,2008
               if ( Globals.repositorySqlVersion < 9 && 
                  (radioButtonDeployNow.Checked || radioButtonAlreadyDeployed.Checked))
               {
                  this.Cursor = Cursors.WaitCursor;
                  // connect
                  SQLDirect direct = new SQLDirect();
                  if ( direct.OpenConnection( this.textSQLServer.Text ) )
                  {
                     // get version
                     int targetSqlVersion = SQLHelpers.GetSqlVersion(direct.Connection);

                     // SQL Server 2005,2008
                     if (targetSqlVersion > 8)
                     {
                        currentPage = pageCantAudit2005;
                     }
                     direct.CloseConnection();
                  }
                  else
                  {
                     currentPage = pageCantConnect;
                  }
                  this.Cursor = Cursors.Default;
               }
            }
            else if ( (currentPage == pageAuditedUsers) && (lstPrivilegedUsers.Items.Count ==0 ) )
            {
               currentPage = pagePermissions;
            }
            else if ( currentPage == pageIncompatibleDatabase )
            {
               currentPage = pageAgentDeploy;
            }
            else if ( currentPage < (numPages-1) )
            {
               currentPage ++;
            }
         }
         else // Prev
         {
            // special navigation logic
            if ( currentPage == pageCluster )
            {
               currentPage = pageServer;
            }
            else if ( currentPage == pageAgentDeploy )
            {
               if ( m_existingDatabase )
               {               
                  if ( m_compatibleSchema )
                     currentPage = pageExistingDatabase;
                  else
                     currentPage = pageIncompatibleDatabase;
               }
               else
               {
                  if ( m_alreadyDeployed )
                     currentPage = pageServer;
                  else
                     currentPage = pageCluster;
               }
            }
            else if ( (currentPage == pagePermissions) && (lstPrivilegedUsers.Items.Count == 0 ) )
            {
               currentPage = pageAuditedUsers;
            }
            else if ( currentPage == pageAudit )
            {
               if ( m_alreadyDeployed || ! radioButtonDeployNow.Checked )
                  currentPage = pageAgentDeploy;
               else
                  currentPage = pageAgentTrace;
            }
            else if ( currentPage == pageIncompatibleDatabase )
            {
               if ( m_alreadyDeployed )
                  currentPage = pageServer;
               else
                  currentPage = pageCluster;
            }
            else if ( currentPage == pageCantAudit2005 )
            {
               currentPage = pageServer;
            }
            else if ( currentPage == pageCantConnect )
            {
               currentPage = pageAgentDeploy;
            }
            else if ( currentPage > 0 )
            {
               currentPage --;
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
            
         if ( currentPage == pageServer )
         {
            currentPanel = this.panelServer;
            SetButtonState( false,   /* back   */
                            ( textSQLServer.Text.Trim() != "" ),    /* next   */
                            false ); /* finish */
         }
         else if ( currentPage == pageCluster )
         {
            currentPanel = this.panelCluster;
            SetButtonState( true,   /* back   */
                            true,
                            false ); /* finish */
         }
         else if ( currentPage == pageExistingDatabase )
         {
            currentPanel = this.panelExistingDatabase;
            SetButtonState( true,   /* back   */
                            true,   /* next   */
                            false); /* finish */
         }
         else if ( currentPage == pageIncompatibleDatabase )
         {
            currentPanel = this.panelIncompatibleDatabase;
            SetButtonState( true,   /* back   */
                            ( radioIncompatibleOverwrite.Checked ),  /* next   */
                            false); /* finish */
         }
         else if ( currentPage == pageAgentDeploy )
         {
            currentPanel = this.panelAgentDeploy;
            SetButtonState( true,   /* back   */
                            true,   /* next   */
                            false); /* finish */
         }
         else if ( currentPage == pageAgentService )
         {
            currentPanel = this.panelAgentService;
            SetButtonState( true,    /* back   */
                            true,    /* next   */
                            false ); /* finish */
         }
         else if ( currentPage == pageAgentTrace )
         {
            currentPanel = this.panelAgentTrace;
            SetButtonState( true,   /* back   */
                            true,  /* next   */
                            false ); /* finish */
         }
         else if ( currentPage == pageAudit )
         {
            currentPanel = this.panelAudit;
            SetButtonState( true,   /* back   */
                            true,  /* next   */
                            false ); /* finish */
         }
         else if ( currentPage == pageAuditedUsers )
         {
            currentPanel = this.panelAuditedUsers;
            
            SetButtonState( true,    /* back   */
                            true,    /* next   */
                            false ); /* finish */
         }
         else if ( currentPage == pageAuditedUserActivity )
         {
            currentPanel = this.panelAuditedUserActivity;
            SetButtonState( true,   /* back   */
                            true,   /* next   */
                            false ); /* finish */
         }
         else if ( currentPage == pagePermissions )
         {
            currentPanel = this.panelPermissions;
            SetButtonState( true,   /* back   */
                            true,  /* next   */
                            false ); /* finish */
         }
         else if ( currentPage == pageSummary )
         {
            currentPanel = this.panelSummary;
            _flblVirtualServerInfo.Visible = m_isClustered ;
            lblInstance.Text = textSQLServer.Text;
            SetButtonState( true,   /* back   */
                            false,  /* next   */
                            true ); /* finish */
         }
         else if ( currentPage == pageLicenseLimit )
         {
            currentPanel = this.panelLicenseLimit;
            SetButtonState( false,   /* back   */
                            false,  /* next   */
                            false ); /* finish */
         }
         else if ( currentPage == pageCantAudit2005 )
         {
            currentPanel = this.panelCantAudit2005;
            SetButtonState( true,   /* back   */
               false,  /* next   */
               false ); /* finish */
         }
         else if ( currentPage == pageCantConnect )
         {
            currentPanel = this.panelCantConnect;
            SetButtonState( true,   /* back   */
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
            if ( currentPage == pageServer )
            {
               textSQLServer.Focus();
            }
            else if ( currentPage == pageCluster )
            {
               checkVirtualServer.Focus();
            }
            else if ( currentPage == pageExistingDatabase )
            {
               if ( radioMaintainDatabase.Checked )
                  radioMaintainDatabase.Focus();
               else
                  radioDeleteDatabase.Focus();
            }
            else if ( currentPage == pageIncompatibleDatabase )
            {
               if ( radioIncompatibleCancel.Checked )
                  radioIncompatibleCancel.Focus();
               else
                  radioIncompatibleOverwrite.Focus();
            }
            else if ( currentPage == pageAgentDeploy )
            {
               if ( m_alreadyDeployed )
               {
                  radioButtonAlreadyDeployed.Checked = true;
                  radioButtonAlreadyDeployed.Visible = true;
                  radioButtonDeployLater.Enabled     = false;
                  radioButtonDeployNow.Enabled       = false;
                  radioButtonDeployManually.Enabled  = false;
               }
               else if ( m_isClustered )
               {
                  radioButtonAlreadyDeployed.Visible = false;
                  radioButtonDeployLater.Enabled     = false;
                  radioButtonDeployNow.Enabled       = false;
                  radioButtonDeployManually.Enabled  = true;
                  radioButtonDeployManually.Checked  = true;
               }
               else
               {
                  radioButtonAlreadyDeployed.Visible = false;
                  radioButtonDeployLater.Enabled     = true;
                  radioButtonDeployNow.Enabled       = true;
                  radioButtonDeployManually.Enabled  = true;
               }
            
               if ( radioButtonDeployNow.Checked )
                  radioButtonDeployNow.Focus();
               else if ( radioButtonDeployLater.Checked )
                  radioButtonDeployLater.Focus();
               else if ( radioButtonDeployManually.Checked )
                  radioButtonDeployManually.Focus();
               else if ( radioButtonAlreadyDeployed.Checked && radioButtonAlreadyDeployed.Visible)
                  radioButtonAlreadyDeployed.Focus();
               else
                  btnNext.Focus();
            }
            else if ( currentPage == pageAgentService )
            {
               if ( textServiceAccount.Enabled )
               {
                  textServiceAccount.Focus();
               }
               else
               {
                  btnNext.Focus();
               }
            }
            else if ( currentPage == pageAgentTrace )
            {
               if ( radioDefaultTrace.Checked )
                  radioDefaultTrace.Focus();
               else if ( radioSpecifyTrace.Checked )
                  radioSpecifyTrace.Focus();
               else
                  btnNext.Focus();
            }
            else if ( currentPage == pageAudit )
            {
               chkAuditLogins.Focus();
            }
            else if ( currentPage == pageAuditedUsers )
            {
               if ( lstPrivilegedUsers.Items.Count > 0 )
               {
                  lstPrivilegedUsers.Focus();
                  lstPrivilegedUsers.Items[0].Selected = true;
               }
               else
               {
                  btnNext.Focus();
               }
            }
            else if ( currentPage == pageAuditedUserActivity )
            {
               if ( rbAuditUserAll.Checked )
                  rbAuditUserAll.Focus();
               else
                  rbAuditUserSelected.Focus();
            }
            else if ( currentPage == pagePermissions )
            {
               if ( radioGrantAll.Checked )
                  radioGrantAll.Focus();
               else if ( radioGrantEventsOnly.Checked )
                  radioGrantEventsOnly.Focus();
               else
                  radioDeny.Focus();
            }
            else if ( currentPage == pageSummary )
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
      }
      
      #endregion
      
      #region Validation Methods

      //--------------------------------------------------------------------
      // ValidatePage - Simple validation done as users switches pages with
      //                back and next. More extensive validation is done
      //                after Finish is pressed.
      //--------------------------------------------------------------------
      private bool
         ValidatePage(
            int               page
         )
      {
         if ( page == pageServer )  // server instance
         {
            if ( textSQLServer.Text != this.m_lastCheckedName )
            {
               if ( ! ValidateServerName() )
               {
                  ErrorMessage.Show( this.Text,
                                    UIConstants.Error_InvalidServerName );
                  return false;
               }
               
               // see if server is already registered
               // compare against existing registered instances
               string instanceServer;
               if ( m_computer == "" )
                  instanceServer = Dns.GetHostName().ToUpper();
               else
                  instanceServer = m_computer;

               m_convertingNonAudited    = false;
               m_alreadyDeployed         = false;
               m_alreadyDeployedManually = false;
               
               if ( m_repositoryComputer )
               {
                  m_alreadyDeployed = true;
               }

               ICollection serverList ;
               serverList = ServerRecord.GetServers( Globals.Repository.Connection, false );
			      if ((serverList != null) && (serverList.Count != 0)) 
			      {
				      foreach (ServerRecord config in serverList) 
				      {
				         if ( config.IsAuditedServer )
				         {
				            if (config.Instance.ToUpper() == textSQLServer.Text.ToUpper())
				            {
                           ErrorMessage.Show( this.Text,
                                             UIConstants.Error_ServerAlreadyRegistered );
                           return false;
				            }

                        // some possible states depend on state of already
                        // audited instances on same computer				      
				            if ( config.InstanceServer.ToUpper() == instanceServer.ToUpper() )
				            {
				               if (	m_existingAuditedServer == null )
				               {
				                  m_existingAuditedServer = config;
				               }
				               if ( config.IsDeployed )
				               {
				                  m_alreadyDeployed         = true;
				                  m_alreadyDeployedManually = config.IsDeployedManually;
				               }
				            }
				         }
				         else
				         {
				            if (config.Instance.ToUpper() == textSQLServer.Text.ToUpper())
				            {
				               m_existingServer       = config;
				               m_convertingNonAudited = true;
				            }
				         }
                  }
               }

               // since we are here we have a valid name dont check again until it changes
               m_lastCheckedName = textSQLServer.Text;
               
               m_isClustered                = false;
               checkVirtualServer.Checked   = false;
               radioButtonDeployNow.Checked = true;
            }
         }
         
         if ( page == pageCluster ) // cluster
         {
            m_isClustered = checkVirtualServer.Checked;
         }
         
         if ( page == pageAgentService )  // agent service account
         {
            if ( radioButtonDeployNow.Checked )
            {
               if ( ! ValidateAccountName() )
               {
                  ErrorMessage.Show( this.Text,
                                     UIConstants.Error_InvalidServiceAccountName );
                  return false;
               }
               
               if ( textServicePassword.Text != textServicePasswordConfirm.Text )
               {
                  ErrorMessage.Show( this.Text,
                                       UIConstants.Error_MismatchedPasswords );
                  return false;
               }
               
               try
               {
                  // capture exception if installutil not deployed
				      if(InstallUtil.VerifyPassword(textServiceAccount.Text, textServicePassword.Text) != 0)
				      {
					      ErrorMessage.Show(this.Text, UIConstants.Error_InvalidDomainCredentials) ;
					      return false ;
				      }
				   }
				   catch ( Exception ex )
				   {
                  ErrorMessage.Show(this.Text,
                                    String.Format( UIConstants.Error_NoInstallUtilLib,
                                                   ex.Message)) ;
                  return false ;
               }

            }
         }
         
         if ( page == pageAgentTrace ) // trace directory
         {
            if ( radioButtonDeployNow.Checked )
            {
               if ( radioSpecifyTrace.Checked )
               {
		         if ( ! UIUtils.ValidatePath( txtTraceDirectory.Text ) )
                  {
                     ErrorMessage.Show( this.Text,
                                       UIConstants.Error_InvalidTraceDirectory );
                     return false;
                  }
               }
            }
         }
         
         if ( page == pageAudit ) // audit settings
         {
            // nothing to check!
         }
         
         if ( page == pageAuditedUsers ) // privileged users
         {
            if ( lstPrivilegedUsers.Items.Count > 0 &&
               rbAuditUserSelected.Checked )
            {
               // make sure something checked
               if ( ! chkAuditUserLogins.Checked &&
                  ! chkAuditUserFailedLogins.Checked &&
                  ! chkAuditUserSecurity.Checked &&
                  ! chkAuditUserDDL.Checked &&
                  ! chkAuditUserAdmin.Checked &&
                  ! chkAuditUserDML.Checked &&
                  ! chkAuditUserSELECT.Checked &&
                  ! chkAuditUserUDE.Checked)
               {
                  ErrorMessage.Show( this.Text,
                                    UIConstants.Error_MustSelectOneAuditUserOption );
                  return false;
               }
            }
         }

         return true;
      }
      
      //--------------------------------------------------------------------
      // ValidateServerName
      //
      //     Check server form
      //     Set member variables: m_computer and m_instance
      //
      //     Cases
      //     ----------------------------------------
      //     xxxxx   - computer="xxxx",  instance = xxxx
      //     .       - computer="",  instance = local
      //     ./xxxx  - computer="",  instance = xxxx
      //     (local) - computer="",  instance = local
      //     hhh/zzz - computer=hhh, instance = zzzz
      //--------------------------------------------------------------------
      private bool ValidateServerName()
      {
         string localhost = Dns.GetHostName().ToUpper();

         m_computer           = "";
         m_instance           = "";
         
         string server      =  textSQLServer.Text.Trim().ToUpper();
         textSQLServer.Text = server;
         
         int pos = server.IndexOf(@"\");
         if ( pos == - 1)
         {
            if ( server == UIConstants.UpperLocal || server == ".")
            {
               m_instance = localhost;
               textSQLServer.Text = m_instance;
            }
            else
            {
               m_instance = server;
               if ( server != localhost )
               {
                  m_computer = server;
               }
            }
         }
         else if ( pos == 0)
         {
            return false;
         }
         else // pos > 0; we have xxx/yyy
         {
            m_computer = server.Substring(0,pos);
            m_instance = server.Substring(pos+1);
            
            if ( m_instance.Length == 0 )
            {
               return false;
            }
            else
            {
               if (m_computer == UIConstants.UpperLocal || m_computer == ".")
               {
                  m_computer = localhost;
                  textSQLServer.Text = m_computer + @"\" + m_instance;
               }
            }
         }
         
         // Is this install to the repository server??
         m_repositoryComputer = false;
         
         string repositoryHost = UIUtils.GetInstanceHost(Globals.SQLcomplianceConfig.Server.ToUpper());
         string host           = m_computer;
         if ( host == "" || host == "." ) host = localhost;
         
         if ( host == repositoryHost )
         {
            m_repositoryComputer = true;
         }
         
         // if we got here, instance name is at least a valid format          
         return true;
      }
      
      
      //--------------------------------------------------------------------
      // ValidateAccountName
      //
      // simple validation
      // -----------------
      // form domain\account
      // neither domain or account can be blank
      //--------------------------------------------------------------------
      private bool ValidateAccountName()
      {
         string domain;
         string account;
         
         string tmp = textServiceAccount.Text.Trim();
         
         int pos = tmp.IndexOf(@"\");
         if ( pos <= 0 )
         {
            return false;
         }
         else
         {
            domain  = tmp.Substring(0,pos);
            account = tmp.Substring(pos+1);
            
            if ((domain == "") || (account == "" ))
               return false;
         }
         
         return true;
      }
      
      #endregion
      
      #region Form Events

      //--------------------------------------------------------------------
      // btnBrowse_Click - Browse for SQL Servers on the network to select
      //                   one for registration
      //--------------------------------------------------------------------
      private void btnBrowse_Click(object sender, EventArgs e)
      {
         Form_SQLServerBrowse dlg = new Form_SQLServerBrowse();
         
         try
         {
            if ( dlg.LoadServers( false ) )
            {
               if ( DialogResult.OK == dlg.ShowDialog() )
               {
                  textSQLServer.Text = dlg.SelectedServer.ToString();   
               }
            }
         }
         catch (Exception ex )
         {
			   ErrorMessage.Show( UIConstants.Error_DMOLoadServers, ex.Message );
         }
      }

      //--------------------------------------------------------------------
      // textSQLServer_TextChanged - Dont let the user hit next until a
      //                             SQL Server has been specified
      //--------------------------------------------------------------------
      private void textSQLServer_TextChanged(object sender, EventArgs e)
      {
         btnNext.Enabled = textSQLServer.Text.Trim() != "";
         
         if ( btnNext.Enabled )
         {
            this.AcceptButton = btnNext;
            
            radioMaintainDatabase.Checked   = true;
            radioIncompatibleCancel.Checked = true;
         }
      }
      
      //--------------------------------------------------------------------
      // textServiceAccount_TextChanged
      //--------------------------------------------------------------------
      private void textServiceAccount_TextChanged(object sender, EventArgs e)
      {
         if ( textServiceAccount.Text.Trim() != "" )
            btnNext.Enabled = true;
         else
            btnNext.Enabled = false;
      }
      
      //--------------------------------------------------------------------
      // rbAuditUserSelected_CheckedChanged
      //--------------------------------------------------------------------
      private void rbAuditUserSelected_CheckedChanged(object sender, EventArgs e)
      {
         grpAuditUserActivity.Enabled = rbAuditUserSelected.Checked;
      }
      
      //--------------------------------------------------------------------
      // LoadGlobalDefaults
      //--------------------------------------------------------------------
      private void
         LoadGlobalDefaults()
      {
         chkAuditLogins.Checked         = Globals.SQLcomplianceConfig.AuditLogins;
         chkAuditFailedLogins.Checked   = Globals.SQLcomplianceConfig.AuditFailedLogins;
         chkAuditDDL.Checked            = Globals.SQLcomplianceConfig.AuditDDL;
         chkAuditAdmin.Checked          = Globals.SQLcomplianceConfig.AuditAdmin;
         chkAuditSecurity.Checked       = Globals.SQLcomplianceConfig.AuditSecurity;
         chkAuditUDE.Checked = false ;
         
         switch(Globals.SQLcomplianceConfig.AuditAccessCheck)
         {
            case AccessCheckFilter.FailureOnly:
               _cbAccessCheckFilter.Checked = true ;
               rbAuditFailedOnly.Checked = true ;
               break ;
            case AccessCheckFilter.SuccessOnly:
               _cbAccessCheckFilter.Checked = true ;
               rbAuditSuccessfulOnly.Checked = true ;
               break ;
            case AccessCheckFilter.NoFilter:
               _cbAccessCheckFilter.Checked = false ;
               break ;
         }
      }
      
      #endregion
      
      #region Finish and Save

      //--------------------------------------------------------------------
      // btnFinishClick - Validate input and then create the new
      //                  registered SQL Server
      //--------------------------------------------------------------------
      private void btnFinish_Click(object sender, EventArgs e)
      {
         if ( ! m_licenseExceeded )
         {
            // Validate Data
            for ( int i=0; i<numPages; i++ )
            {
               if ( ! ValidatePage(i) )
               {
                  currentPage = i;
                  SetCurrentPage();
                  return;
               }
            }
            
            // Save
            if ( CreateServer() )
            {
               // deploy agent
               if ( m_alreadyDeployed )
               {
				      if ( ! Activate( textSQLServer.Text ) )
				      {                  
					      // Activate first - if cant reach an existing agent then we cant 
					      // do anything - mark as not deployed/running.  Activate requires the
					      //  entry exist in the db to work.
					      m_srv.IsRunning  = false;
					      m_srv.IsDeployed = false;
                     _desiredDeploymentSuccess = false ;
				      }
				      else
				      {
					      m_srv.IsRunning  = true;
					      m_srv.IsDeployed = true;
				      }
                  
            	   ServerRecord.SetIsFlags( m_srv.Instance,
            	                            m_srv.IsDeployed ,
            	                            m_srv.IsDeployedManually,
            	                            m_srv.IsRunning,
            	                            m_srv.IsCrippled,
            	                            Globals.Repository.Connection );
               }
               
               if ( (! m_alreadyDeployed) && radioButtonDeployNow.Checked )
               {
				      m_srv.IsDeployed = true;
                  
				      ServerRecord.SetIsFlags( m_srv.Instance,
					      m_srv.IsDeployed ,
					      m_srv.IsDeployedManually,
					      m_srv.IsRunning,
					      m_srv.IsCrippled,
					      Globals.Repository.Connection );
				      if ( DeployAgent() )
				      {
					      // agent deployed - update all instances on that computer
					      ICollection servers = ServerRecord.GetServers( Globals.Repository.Connection,
						      true );
					      foreach ( ServerRecord srvrec in servers )
					      {
						      if ( srvrec.InstanceServer.ToUpper() == srv.InstanceServer.ToUpper() )
						      {
							      ServerRecord oldServerState = srvrec.Clone();
							      if ( oldServerState.IsDeployed )
							      {                     
								      srvrec.IsRunning            = true;
								      srvrec.IsDeployed           = true;
								      srvrec.AgentTraceDirectory  = srv.AgentTraceDirectory;
								      srvrec.AgentServiceAccount  = srv.AgentServiceAccount;
								      srvrec.Write( oldServerState );
                              
								      LogRecord.WriteLog( Globals.Repository.Connection,
									      LogType.DeployAgent,
									      srvrec.Instance,
									      "SQLcompliance Agent deployed" );
							      }
						      }
					      }
				      }
				      else
				      {
					      m_srv.IsDeployed = false;
                     _desiredDeploymentSuccess = false ;
                  
					      ServerRecord.SetIsFlags( m_srv.Instance,
						      m_srv.IsDeployed ,
						      m_srv.IsDeployedManually,
						      m_srv.IsRunning,
						      m_srv.IsCrippled,
						      Globals.Repository.Connection );
				      }
               }
               // Force a heartbeat for status
               PingAgent();
               this.DialogResult = DialogResult.OK;
               this.Close();
            }
            else
            {
               m_srv = null;
            }
         }

         this.Close();
      }
      
      //--------------------------------------------------------------------
      // Activate
      //--------------------------------------------------------------------
      private bool
         Activate(
            string            instance
         )
      {
         bool activated = false;
         
         try
         {
			 // Make sure the service is started
			 string agentServer = instance ;

			 if(agentServer.IndexOf("\\") != -1)
				 agentServer = agentServer.Substring(0, agentServer.IndexOf("\\")) ;

			 try
			 {
				 // This will fail across untrusted domains/workgroups.  However, it is
				 //  not a fatal error, so we silently catch and move along.  The true
				 //  point of failure will be in the following AgentManager.Activate().
				 AgentServiceManager serviceManager = new AgentServiceManager(null, null, agentServer, null, null) ;
				 serviceManager.Start() ;
			 }
			 catch(Exception){}

            // need to register with agent
            string url = String.Format( "tcp://{0}:{1}/{2}",
                                          Globals.SQLcomplianceConfig.Server, 
                                          Globals.SQLcomplianceConfig.ServerPort,
                                          typeof(AgentManager).Name );
            AgentManager manager = (AgentManager)Activator.GetObject( typeof(AgentManager), url );
            manager.Activate( instance );
            
            activated = true;
         }
         catch ( Exception ex )
         {
            ErrorMessage.Show( UIConstants.Title_Activate,
                               UIConstants.Error_CantActivate,
                               UIUtils.TranslateRemotingException( Globals.SQLcomplianceConfig.Server,
                                                                   UIConstants.CollectionServiceName,
                                                                   ex ) );
         }
         return activated;
      }
      
      //--------------------------------------------------------------------
      // DeployAgent
      //--------------------------------------------------------------------
      private bool DeployAgent()
      {
         bool success = false;
         
			ProgressForm progressForm = new ProgressForm(
			      "Deploying SQLcompliance Agent on " + m_srv.InstanceServer + "...",
               m_srv.InstanceServer,
               m_srv.AgentServiceAccount,
               textServicePassword.Text,
               m_srv.AgentTraceDirectory,
               m_srv.Instance,
               Globals.SQLcomplianceConfig.Server,
               DeploymentType.Install );
               
			progressForm.ShowDialog();
			
			if ( ! progressForm.IsCancelled )
			{
			   success = progressForm.IsServiceStarted;
			}
			
         return success;
      }

      private void PingAgent()
      {
         try
         {
            // ping agent
            string url = String.Format("tcp://{0}:{1}/{2}",
                                        m_srv.AgentServer,
                                        m_srv.AgentPort,
                                        typeof(AgentCommand).Name);
            AgentCommand agentCmd = (AgentCommand)Activator.GetObject(typeof(AgentCommand), url);
            agentCmd.Ping();
         }
         catch (Exception)
         {
         }
      }

	   //--------------------------------------------------------------------
      // CreateServerRecord
      //--------------------------------------------------------------------
      private void
         CreateServerRecord()
      {
         m_srv = new ServerRecord();
         
         m_srv.Connection = Globals.Repository.Connection;
         
         // General
         m_srv.Instance = textSQLServer.Text;
         
         m_srv.isClustered = m_isClustered;
         
         if ( m_computer == "" )
         {
            m_srv.InstanceServer = Dns.GetHostName().ToUpper();
         }
         else
         {
            m_srv.InstanceServer = m_computer;
         }
         m_srv.AgentServer = m_srv.InstanceServer;
         
		   m_srv.Description   = textDescription.Text;
		   
		   m_srv.IsEnabled     = true;
		   
		   if ( radioGrantAll.Checked )
		      m_srv.DefaultAccess = 2;
		   else if ( radioGrantEventsOnly.Checked )
		      m_srv.DefaultAccess = 1;
		   else
		      m_srv.DefaultAccess = 0;
		   
		   m_srv.ConfigVersion           = 1;
		   m_srv.LastKnownConfigVersion  = 0;
		   m_srv.LastConfigUpdate        = DateTime.MinValue;
		   
		   m_srv.IsAuditedServer         = true;
		   m_srv.IsOnRepositoryHost      = this.m_repositoryComputer;
		   
		   // Agent Settings
		   m_srv.AgentServiceAccount = textServiceAccount.Text;
		   if ( radioDefaultTrace.Checked )
		   {
		      m_srv.AgentTraceDirectory = ""; // install will pick default
		   }
		   else
		   {
		      m_srv.AgentTraceDirectory = txtTraceDirectory.Text;
		   }


		   
		   if ( ! m_alreadyDeployed )
		   {
            m_srv.IsDeployed          = false;
            m_srv.IsDeployedManually  = radioButtonDeployManually.Checked;   
         }
         else
         {
            m_srv.IsDeployed          = true;
            m_srv.IsDeployedManually  = m_alreadyDeployedManually;   
         }

         // Audit Settings		
		   m_srv.AuditLogins          = chkAuditLogins.Checked;
		   m_srv.AuditFailedLogins    = chkAuditFailedLogins.Checked;
		   m_srv.AuditDDL             = chkAuditDDL.Checked;
         m_srv.AuditAdmin           = chkAuditAdmin.Checked;
         m_srv.AuditSecurity        = chkAuditSecurity.Checked;
         m_srv.AuditUDE = chkAuditUDE.Checked ;

         if(_cbAccessCheckFilter.Checked)
         {
            if(rbAuditSuccessfulOnly.Checked)
               m_srv.AuditAccessCheck = AccessCheckFilter.SuccessOnly ;
            else
               m_srv.AuditAccessCheck = AccessCheckFilter.FailureOnly ;
         }
         else
         {
            m_srv.AuditAccessCheck = AccessCheckFilter.NoFilter ;
         }
		   m_srv.AuditExceptions      = false;
		   
		   m_srv.AuditUsersList        = GetPrivilegedUserProperty();
		   m_srv.AuditUserAll          = rbAuditUserAll.Checked;
		   m_srv.AuditUserLogins       = chkAuditUserLogins.Checked;
		   m_srv.AuditUserFailedLogins = chkAuditUserFailedLogins.Checked;
		   m_srv.AuditUserDDL          = chkAuditUserDDL.Checked;
		   m_srv.AuditUserSecurity     = chkAuditUserSecurity.Checked;
         m_srv.AuditUserAdmin        = chkAuditUserAdmin.Checked;
         m_srv.AuditUserDML          = chkAuditUserDML.Checked;
		   m_srv.AuditUserSELECT       = chkAuditUserSELECT.Checked;
         m_srv.AuditUserUDE = chkAuditUserUDE.Checked ;
         if(chkUserAccessCheckFilter.Checked)
         {
            if(_rbUserAuditPassed.Checked)
               m_srv.AuditUserAccessCheck = AccessCheckFilter.SuccessOnly ;
            else
               m_srv.AuditUserAccessCheck = AccessCheckFilter.FailureOnly ;
         }
         else
         {
            m_srv.AuditUserAccessCheck = AccessCheckFilter.NoFilter ;
         }
		   m_srv.AuditUserCaptureSQL   = chkAuditUserCaptureSQL.Checked;
         m_srv.AuditUserCaptureTrans = chkAuditUserCaptureTrans.Checked;
		   m_srv.AuditUserExceptions   = false;

         //DML only setting
         if (rbAuditUserSelected.Checked && chkAuditUserDML.Checked)
            chkAuditUserCaptureTrans.Enabled = true;
         else
            chkAuditUserCaptureTrans.Enabled = false;

         //DML or SELECT setting
         if (rbAuditUserSelected.Checked && (chkAuditUserDML.Checked || chkAuditUserSELECT.Checked) && CoreConstants.AllowCaptureSql)
            chkAuditUserCaptureSQL.Enabled = true;
         else
         {
            chkAuditUserCaptureSQL.Checked = false;
            chkAuditUserCaptureSQL.Enabled = false;
         }

                     
         m_srv.LowWatermark  = -2100000000;
         m_srv.HighWatermark = -2100000000;
            
         // copy agent properties from existing audited instances   
         if (	m_existingAuditedServer != null )
			{
            m_srv.IsRunning                     = m_existingAuditedServer.IsRunning;
            m_srv.IsCrippled                    = m_existingAuditedServer.IsCrippled;
            
			   m_srv.InsertAgentProperties         = true;
			   m_srv.AgentServer                   = m_existingAuditedServer.AgentServer;
			   m_srv.AgentPort                     = m_existingAuditedServer.AgentPort;
			   m_srv.AgentServiceAccount           = m_existingAuditedServer.AgentServiceAccount;
			   m_srv.AgentTraceDirectory           = m_existingAuditedServer.AgentTraceDirectory;
			   m_srv.AgentCollectionInterval       = m_existingAuditedServer.AgentCollectionInterval;
			   m_srv.AgentForceCollectionInterval  = m_existingAuditedServer.AgentForceCollectionInterval;
			   m_srv.AgentHeartbeatInterval        = m_existingAuditedServer.AgentHeartbeatInterval;
			   m_srv.AgentLogLevel                 = m_existingAuditedServer.AgentLogLevel;
			   m_srv.AgentMaxFolderSize            = m_existingAuditedServer.AgentMaxFolderSize;
			   m_srv.AgentMaxTraceSize             = m_existingAuditedServer.AgentMaxTraceSize;
			   m_srv.AgentMaxUnattendedTime        = m_existingAuditedServer.AgentMaxUnattendedTime;
			   m_srv.AgentTraceOptions             = m_existingAuditedServer.AgentTraceOptions;
			   m_srv.AgentVersion                  = m_existingAuditedServer.AgentVersion;
			   m_srv.TimeLastHeartbeat             = m_existingAuditedServer.TimeLastHeartbeat;
			}
			
         if ( m_convertingNonAudited )
         {
	         m_srv.SrvId = ServerRecord.GetServerId( Globals.Repository.Connection,
	                                                 m_srv.Instance );
         }			
      }
      
      //--------------------------------------------------------------------
      // CreateServer - creates server entry in repository dbs 
      //                and server instance database
      //--------------------------------------------------------------------
      private bool CreateServer()
      {
         Cursor = Cursors.WaitCursor;
         
         bool                 retval      = true;

         // Create events database
         string eventsDatabase;
         
         try
         {
            CreateServerRecord();
         
            eventsDatabase = EventDatabase.GetDatabaseName( srv.Instance );
            if  ( ! m_existingDatabase )
            {
               // database doesnt already exist
               EventDatabase.Create( srv.Instance,
                                     eventsDatabase,
                                     srv.DefaultAccess,
                                     Globals.Repository.Connection );
            }
            else
            {
               // Existing events database case
               if ( radioDeleteDatabase.Checked || radioIncompatibleOverwrite.Checked )
               {
                  EventDatabase.InitializeExistingEventDatabase( srv.Instance,
                                                                 eventsDatabase,
                                                                 srv.DefaultAccess,
                                                                 Globals.Repository.Connection );
                  // reset watermarks
                  srv.LowWatermark  = -2100000000;
                  srv.HighWatermark = -2100000000;
               }
               else
               {
                  // Upgrade existing database to latest version if needed
                  if(!EventDatabase.IsCompatibleSchema(eventsDatabase, Globals.Repository.Connection))
                     EventDatabase.UpgradeEventDatabase( Globals.Repository.Connection, eventsDatabase);
                  int schemaVersion = EventDatabase.GetDatabaseSchemaVersion(Globals.Repository.Connection, eventsDatabase) ;

                  if(schemaVersion != CoreConstants.RepositoryEventsDbSchemaVersion)
                  {
                     Form_CreateIndex frm = new Form_CreateIndex(true) ;
                     if(frm.ShowDialog(this) == DialogResult.OK)
                     {
                        EventDatabase.UpdateIndexes(Globals.Repository.Connection, eventsDatabase);
                     }
                     else
                     {
                        SQLcomplianceConfiguration config = new SQLcomplianceConfiguration();
                        config.Read(Globals.Repository.Connection);

                        if (config.IndexStartTime == DateTime.MinValue)
                        {
                           Form_IndexSchedule schedule = new Form_IndexSchedule();
                           schedule.ShowDialog(this);
                           config.IndexStartTime = schedule.IndexStartTime;
                           config.IndexDuration = schedule.IndexDuration;
                           config.Write(Globals.Repository.Connection);
                        }
                        SetReindexFlag(true);
                     }
                  }

                  // set watermarks to first and last record in existing database
                  int lowWatermark;
                  int highWatermark;
                  
                  EventDatabase.GetWatermarks( eventsDatabase,
                                               out lowWatermark,
                                               out highWatermark,
                                               Globals.Repository.Connection );
                                               
                  srv.LowWatermark  = lowWatermark;     
                  if ( srv.LowWatermark != -2100000000 ) srv.LowWatermark--;
                                                            
                  srv.HighWatermark = highWatermark;                                               
               }
               
               
		         // Update SystemDatabase Table
		         EventDatabase.AddSystemDatabase( srv.Instance,
		                                          eventsDatabase,
		                                          Globals.Repository.Connection );
            }
                                  
            srv.EventDatabase = eventsDatabase;                                                  
         }
         catch ( Exception ex )
         {
            if ( UIUtils.CloseIfConnectionLost() ) return false;

            Cursor = Cursors.Default;
            ErrorMessage.Show( this.Text,
                               UIConstants.Error_CantCreateEventsDatabase,
                               ex.Message );
            return false;
         }
         
         // write server record
         if ( ! WriteServerRecord() )
         {
            retval = false;
         }
         else
         {
            string snapshot = Snapshot.ServerSnapshot( Globals.Repository.Connection,
                                                       srv,
                                                       false );
         
				ServerUpdate.RegisterChange( srv.SrvId,
                                         LogType.NewServer,
                                         srv.Instance,
                                         snapshot );
            AgentStatusMsg.LogStatus( srv.AgentServer,
                                      srv.Instance,
                                      AgentStatusMsg.MsgType.Registered,
                                      Globals.Repository.Connection );
         }
            
         Cursor = Cursors.Default;
         
         return retval;
      }

      private void SetReindexFlag(bool reindex)
      {
         ServerManager srvManager;
         string url;

         // check for collection service - cant uninstall if it is down or unreachable
         try
         {
            url = String.Format("tcp://{0}:{1}/{2}",
                                Globals.SQLcomplianceConfig.Server,
                                Globals.SQLcomplianceConfig.ServerPort,
                                typeof(ServerManager).Name);

            srvManager = (ServerManager)Activator.GetObject(typeof(ServerManager), url);
            srvManager.SetReindexFlag(reindex);
         }
         catch (Exception)
         {
            // TODO:  Should we alert the user when we can't talk to the collection server?
         }
      }

      //--------------------------------------------------------------------
      // WriteServerRecord
      //--------------------------------------------------------------------
      private bool
         WriteServerRecord()
      {
         bool                 retval      = false;
         bool                 done;
         string               s;
         
         if ( m_convertingNonAudited )
         {
            done = srv.Write( m_existingServer );
            
            s = UIConstants.Error_ErrorConvertingServer;
         }
         else
         {
            done = srv.Create(null);
            s    = UIConstants.Error_ErrorCreatingServer;
         }
         
         if (! done)
            ErrorMessage.Show( this.Text, s, ServerRecord.GetLastError() );
         else
            retval  = true;
            
         return retval;
      }
      
      #endregion

      #region Audit Settings Page Handlers
      
      //--------------------------------------------------------------------
      // chkUserCaptureSQL_CheckedChanged
      //--------------------------------------------------------------------
      private void chkUserCaptureSQL_CheckedChanged(object sender, EventArgs e)
      {
         if ( m_isLoaded && chkAuditUserCaptureSQL.Checked )
         {
            ErrorMessage.Show( this.Text,
                                 UIConstants.Warning_CaptureAll,
                                 "",
                                 MessageBoxIcon.Warning );
         }
      }
      
      #endregion

      #region Help
      
      //--------------------------------------------------------------------
      // Form_ServerNew_HelpRequested - Show Context Sensitive Help
      //--------------------------------------------------------------------
      private void Form_ServerNew_HelpRequested(object sender, HelpEventArgs hlpevent)
      {
         string helpTopic = "";
         
         if ( currentPage == pageServer )
            helpTopic = HelpAlias.SSHELP_Form_ServerNew_General;
         else if ( currentPage == pageExistingDatabase )
            helpTopic = HelpAlias.SSHELP_Form_ServerNew_ExistingDatabase;
         else if ( currentPage == pageIncompatibleDatabase )
            helpTopic = HelpAlias.SSHELP_Form_ServerNew_IncompatibleDatabase;
         else if ( currentPage == pageAgentDeploy )
            helpTopic = HelpAlias.SSHELP_Form_ServerNew_Deploy;
         else if ( currentPage == pageAgentService )
            helpTopic = HelpAlias.SSHELP_Form_ServerNew_Account;
         else if ( currentPage == pageAgentTrace )
            helpTopic = HelpAlias.SSHELP_Form_ServerNew_Trace;
         else if ( currentPage == pageAudit )
            helpTopic = HelpAlias.SSHELP_Form_ServerNew_Activities;
         else if ( currentPage == pageAuditedUsers )
            helpTopic = HelpAlias.SSHELP_Form_ServerNew_PrivUsers;
         else if ( currentPage == pageAuditedUserActivity )
            helpTopic = HelpAlias.SSHELP_Form_ServerNew_PrivUserSettings;
         else if ( currentPage == pagePermissions )
            helpTopic = HelpAlias.SSHELP_Form_ServerNew_Permissions;
         else if ( currentPage == pageSummary )
            helpTopic = HelpAlias.SSHELP_Form_ServerNew_Summary;
         else if ( currentPage == pageLicenseLimit )
            helpTopic = HelpAlias.SSHELP_Form_ServerNew_Error;
         else if ( currentPage == pageCluster )
            helpTopic = HelpAlias.SSHELP_Form_ServerNew_IsCluster ;

      
         if ( helpTopic != "" ) HelpAlias.ShowHelp(this,helpTopic);
         
			hlpevent.Handled = true;
      }

      #endregion

      private void chkAuditUserDML_CheckedChanged(object sender, EventArgs e)
      {
         //DML only setting
         if (rbAuditUserSelected.Checked && chkAuditUserDML.Checked)
            chkAuditUserCaptureTrans.Enabled = true;
         else
         {
            chkAuditUserCaptureTrans.Checked = false;
            chkAuditUserCaptureTrans.Enabled = false;
         }

         //DML or SELECT Setting
         if (rbAuditUserSelected.Checked && (chkAuditUserDML.Checked || chkAuditUserSELECT.Checked) && CoreConstants.AllowCaptureSql)
            chkAuditUserCaptureSQL.Enabled = true;
         else
         {
            chkAuditUserCaptureSQL.Checked = false;
            chkAuditUserCaptureSQL.Enabled = false;
         }
      }

      #region Privileged User Handling      

      private string GetPrivilegedUserProperty()
      {
         int count = 0;
         
         UserList ul = new UserList();
         
		   foreach ( ListViewItem vi in lstPrivilegedUsers.Items )
		   {
		      count++;
            if ( vi.ImageIndex == (int)AppIcons.Img16.Role )
            {
               ul.AddServerRole( vi.Text, vi.Text, (int)vi.Tag );
            }
            else
            {
               ul.AddLogin( vi.Text, (byte[])vi.Tag );
            }
		   }
		   
		   return (count == 0 ) ? "" : ul.ToString();
      }

      private void btnAddPriv_Click(object sender, EventArgs e)
      {
         Form_PrivUser frm = new Form_PrivUser( textSQLServer.Text , true);

		  frm.UseAgentEnumeration = m_alreadyDeployed ;

         //frm.MainForm = this.mainForm;                                                      
         if ( DialogResult.OK == frm.ShowDialog() )
         {
            lstPrivilegedUsers.BeginUpdate();
            
            lstPrivilegedUsers.SelectedItems.Clear();

            foreach ( ListViewItem itm in frm.listSelected.Items )
            {
               bool found = false;
               foreach ( ListViewItem s in lstPrivilegedUsers.Items )
               {
                  if ( itm.Text == s.Text )
                  {
                     found = true;
                     s.Selected = true;
                     break;
                  }
               }
               
               if ( ! found )
               {
                  ListViewItem newItem = new ListViewItem( itm.Text );
                  newItem.Tag        = itm.Tag;
                  newItem.ImageIndex = itm.ImageIndex;
                  lstPrivilegedUsers.Items.Add( newItem );
               }
            }
            
            lstPrivilegedUsers.EndUpdate();
            
            if ( lstPrivilegedUsers.Items.Count > 0 )
            {
               lstPrivilegedUsers.TopItem.Selected = true;
               btnRemovePriv.Enabled = true;
            }
         }
      }

      private void btnRemovePriv_Click(object sender, EventArgs e)
      {
         lstPrivilegedUsers.BeginUpdate();
         
         int ndx = lstPrivilegedUsers.SelectedIndices[0];
         
         foreach (ListViewItem priv in lstPrivilegedUsers.SelectedItems)
         {
            priv.Remove();
         }
         
         lstPrivilegedUsers.EndUpdate();
         
         // reset selected item
         if (lstPrivilegedUsers.Items.Count != 0)
         {
            lstPrivilegedUsers.Focus();
            if ( ndx >= lstPrivilegedUsers.Items.Count )
            {
               lstPrivilegedUsers.Items[lstPrivilegedUsers.Items.Count-1].Selected = true;
            }
            else
               lstPrivilegedUsers.Items[ndx].Selected = true;
         }
         else
         {
            btnRemovePriv.Enabled = false;
         }
      }
      
      #endregion

      private void radioSpecifyTrace_CheckedChanged(object sender, EventArgs e)
      {
         txtTraceDirectory.Enabled = radioSpecifyTrace.Checked;
      }
      
      private bool LicenseAllowsMoreInstances()
      {
         bool moreAvailable = false;
         int  serverCount = ServerRecord.CountAuditedServers( Globals.Repository.Connection );
         
         if ( Globals.SQLcomplianceConfig.LicenseObject!=null && 
              (Globals.SQLcomplianceConfig.LicenseObject.CombinedLicense.numLicensedServers > serverCount ||
              Globals.SQLcomplianceConfig.LicenseObject.CombinedLicense.numLicensedServers == -1) )
         {
            moreAvailable = true;
         }
         
         return moreAvailable;
      }
      
      private bool
         DoesDatabaseExist(
            out bool compatibleSchema
         )
      {
         bool exists;
         compatibleSchema = false;
         
         string instance = textSQLServer.Text.Trim().ToUpper();
         string dbName = EventDatabase.GetDatabaseName( instance );
         exists = EventDatabase.DatabaseExists( dbName, Globals.Repository.Connection );
         
         if ( exists )
         {
            // check schema for compatability - equal or upgradeable
            compatibleSchema = EventDatabase.IsUpgradeableSchema(dbName,Globals.Repository.Connection) ;
            textDatabaseName.Text     = dbName;
            textExistingDatabase.Text = dbName;
         }
         return exists;
      }

      private void radioIncompatible_CheckedChanged(object sender, EventArgs e)
      {
         if ( radioIncompatibleCancel.Checked )
         {
            SetButtonState( true,    /* back   */
                            false,    /* next   */
                            false ); /* finish */
         }
         else
         {
            SetButtonState( true,    /* back   */
                            true,    /* next   */
                            false ); /* finish */
         }
      }

      private void Click_chkUserAccessCheckFilter(object sender, EventArgs e)
      {
         if(chkUserAccessCheckFilter.Checked)
         {
            _rbUserAuditFailed.Enabled = true ;
            _rbUserAuditPassed.Enabled = true ;
         }
         else
         {
            _rbUserAuditFailed.Enabled = false ;
            _rbUserAuditPassed.Enabled = false ;
         }
      
      }

      private void Click_cbAccessCheckFilter(object sender, EventArgs e)
      {
         if(_cbAccessCheckFilter.Checked)
         {
            rbAuditFailedOnly.Enabled = true ;
            rbAuditSuccessfulOnly.Enabled = true ;
         }
         else
         {
            rbAuditFailedOnly.Enabled = false ;
            rbAuditSuccessfulOnly.Enabled = false ;
         }
      }

      private void LinkClicked_flblVirtualServerInfo(object sender, Infragistics.Win.FormattedLinkLabel.LinkClickedEventArgs e)
      {
         HelpAlias.ShowHelp(this, HelpAlias.SSHELP_AuditVirtualInstance);
      }

      private void linkLblHelpBestPractices_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
      {
         HelpAlias.ShowHelp(this, HelpAlias.SSHELP_AuditingBestPractices);
      }

      private void linklblHelpBestPractices2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
      {
         HelpAlias.ShowHelp(this, HelpAlias.SSHELP_AuditingBestPractices);
      }
	}
}


