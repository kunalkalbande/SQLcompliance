//--------------------------------------------------------------------
// Form_LoginNew
//
// (c) Copyright 2004; Idera, inc. ALL RIGHTS RESERVED
//--------------------------------------------------------------------

using System ;
using System.Text ;
using System.Windows.Forms ;
using Idera.SQLcompliance.Application.GUI.Helper ;
using Idera.SQLcompliance.Application.GUI.Properties ;
using Idera.SQLcompliance.Application.GUI.SQL ;
using Idera.SQLcompliance.Core ;
using Idera.SQLcompliance.Core.Cwf;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
	/// <summary>
	/// Summary description for Form_LoginNew.
	/// </summary>
	public partial class Form_LoginNew : Form
	{
        #region Properties

        public RawLoginObject rawLogin;
        private readonly Core.Cwf.LoginAccount _loginAccount;

        #endregion

		#region Constructor / Dispose

		public
		   Form_LoginNew()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
         this.Icon = Resources.SQLcompliance_product_ico;

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
         this.panelLogin.Enabled = false;
         this.panelAccess.Enabled   = false;
         this.panelSummary.Enabled      = false;
			
			currentPage = 0;
			SetCurrentPage();
			
         this.ActiveControl = textName;
		}
		
		#endregion


      #region Properties
      
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
      
      //--------------------------------------------------------------------
      // ValidatePage - Simple validation done as users switches pages with
      //                back and next. More extensive validation is done
      //                after Finish is pressed.
      //--------------------------------------------------------------------
      private bool ValidatePage( int page )
      {
         if ( page == 0 )  // login
         {
         }
         else if ( page == 1 ) // access
         {
         }

         return true;
      }
      
      //--------------------------------
      // Wizard State Machine Constants
      //--------------------------------
      int numPages    = 3;
      int currentPage = 0;
      Panel currentPanel = null;
		enum WizardAction
		{
			Next = 0,
			Prev = 1
	   };

      //--------------------------------------------------------------------
      // ChangeWizardPage - Move forward or backwards in the wizard
      //--------------------------------------------------------------------
      private void
         ChangeWizardPage(
            WizardAction      direction
         )
      {
         // Change Page
         if ( direction == WizardAction.Next )
         {
            if ( currentPage==0 )
            {
               if ( radioDenyAccess.Checked )
               {
                  radioNo.Checked = true;
                  currentPage = 2;
               }
               else
               {
                  currentPage = 1;
               }
            }
            else if ( currentPage < (numPages-1) )
            {
               currentPage ++;
            }
         }
         else // Prev
         {
            if ( currentPage==2 )
            {
               if ( radioDenyAccess.Checked )
               {
                  currentPage = 0;
               }
               else
               {
                  currentPage = 1;
               }
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
            
         if ( currentPage == 0 )
         {
            currentPanel = this.panelLogin;
            SetButtonState( false,   /* back   */
                            (textName.Text != ""),    /* next */
                            false ); /* finish */
         }
         else if ( currentPage == 1 )
         {
            currentPanel = this.panelAccess;
            SetButtonState( true,    /* back   */
                            true,    /* next   */
                            false ); /* finish */
         }
         else if ( currentPage == 2 )
         {
            currentPanel = this.panelSummary;
            
            textSummaryLoginName.Text= textName.Text;
            
            if ( radioYes.Checked )
               textSummaryAccess.Text  = UIConstants.Yes;
            else
               textSummaryAccess.Text  = UIConstants.No;
            
            
            SetButtonState( true,   /* back   */
                            false,  /* next   */
                            true ); /* finish */
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
            if ( currentPage == 0 )
            {
               textName.Focus();
            }
            else if ( currentPage == 1 )
            {
               if ( radioYes.Checked )
                  radioYes.Focus();
               else if ( radioNo.Checked )
                  radioNo.Focus();
            }
            else if ( currentPage == 2 )
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
      
      #region Form Events

      //--------------------------------------------------------------------
      // textName_TextChanged
      //--------------------------------------------------------------------
      private void textName_TextChanged(object sender, EventArgs e)
      {
         if ( textName.Text.Trim() != "" )
            btnNext.Enabled = true;
         else
            btnNext.Enabled = false;
      }
      
      #endregion
      
      #region Finish and Save

      //--------------------------------------------------------------------
      // btnFinishClick - Validate input and then create the new
      //                  registered SQL Server
      //--------------------------------------------------------------------
      private void btnFinish_Click(object sender, EventArgs e)
      {
         // Validate Data
          EventDatabase eventDatabase = new EventDatabase();
         for ( int i=0; i<numPages; i++ )
         {
            if ( ! ValidatePage(i) )
            {
               currentPage = i;
               SetCurrentPage();
               return;
            }
         }

         StringBuilder message = new StringBuilder();
         message.AppendFormat(UIConstants.Message_Permissions, textName.Text);

         if (radioDenyAccess.Checked)
         {
            message.Append(UIConstants.Warning_NoLoginAccess);
         }
         else
         {
            message.Append(UIConstants.Warning_LoginAccess);
         }

         if (radioYes.Checked)
         {
            message.Append(UIConstants.Warning_Sysadmin);
         }
         else
         {
            message.Append(UIConstants.Warning_NoSysadmin);
         }

          if (chkWebApplicationAccess.Checked)
              message.Append(UIConstants.Warning_WebAppAccess);
          else
              message.Append(UIConstants.Warning_NoWebAppAccess);

         message.Append("\r\n");
         message.AppendFormat(UIConstants.Message_AlreadyExists, textName.Text);
         message.Append(UIConstants.Question_Continue);

         DialogResult choice = MessageBox.Show(message.ToString(),
                                              this.Text,
                                              MessageBoxButtons.YesNo,
                                              MessageBoxIcon.Warning);

         if (choice == DialogResult.No)
            return;

         // set properties
         try
         {
            string spName;
            if ( radioDenyAccess.Checked )
               spName = "sp_denylogin";
            else
               spName = "sp_grantlogin";
            
            SQLRepository.AddLogin( spName, textName.Text );
            
            string snapshot = GetSnapshot();
	         LogRecord.WriteLog( Globals.Repository.Connection,
                                LogType.NewLogin,
                                Globals.RepositoryServer,
                                snapshot );
            
         }
         catch ( Exception ex )
         {
            ErrorMessage.Show( this.Text,
                              UIConstants.Error_AddingLogin,
                              ex.Message );
            return;
         }

         // add to role
         if ( radioYes.Checked )
         {
               try
               {
                  SQLRepository.AddToRole( textName.Text, "sysadmin" );
               }
               catch ( Exception ex )
               {
                  ErrorMessage.Show( this.Text,
                                    String.Format( UIConstants.Error_AddRoleMember,
                                                   "System Administrators" ),
                                    ex.Message );
               }
         }
          // Grant access
         try
         {
             List<string> instanceNames = eventDatabase.LoadInstances(Globals.Repository.Connection);         
             
             if ( instanceNames.Count != 0)
                {
                    foreach (string instanceName in instanceNames)
                    {
                        List<string> databaseNames = eventDatabase.LoadDatabases(Globals.Repository.Connection, instanceName);

                        if (databaseNames.Count != 0)
						{
							 int defaultAccess = eventDatabase.GetDefaultAccess(Globals.Repository.Connection, instanceName);
							 foreach (string dbName in databaseNames)
							 {
								 eventDatabase.ApplyAccess(defaultAccess, textName.Text, dbName, Globals.Repository.Connection);
							 }
						}
                 }
             }
         }
         catch (Exception ex)
         {
            ErrorMessage.Show( this.Text,
                              UIConstants.Error_GrantAccessLogin,
                              ex.Message );
            return;
          }
         // try to grant web app access
         var webAppAccess = chkWebApplicationAccess.Checked;
         var loginAccount = new LoginAccount(textName.Text);
         SetWebApplicationAccess(loginAccount, webAppAccess);

         var success = CwfHelper.Instance.SynchronizeUsersWithCwf(loginAccount);
         if (!success)
         {
             SetWebApplicationAccess(loginAccount, !webAppAccess);
             MessageBox.Show(this,
                             string.Format("Failed to update Web application access permission for user {0}.", loginAccount.Name),
                             UIConstants.AppTitle,
                             MessageBoxButtons.OK,
                             MessageBoxIcon.Exclamation);
         }
         
         this.DialogResult = DialogResult.OK;
         this.Close();
      }
      
      private void SetWebApplicationAccess(LoginAccount lAccount, bool webAppAccess)
	  {
          lAccount.WebApplicationAccess = webAppAccess;
          lAccount.Set();
	  }

      private string GetSnapshot()
      {
         StringBuilder snapshot = new StringBuilder(1024);
         
         snapshot.AppendFormat( "Login: {0}\r\n\r\n", textName.Text );
         snapshot.AppendFormat( "Security Access: {0}\r\n",
                                radioDenyAccess.Checked ? "Deny Access"
                                                        : "Grant Access" );
         snapshot.AppendFormat( "SQL Compliance Manager Permissions: {0}\r\n",
                                radioYes.Checked ? "Can configure SQL Compliance Manager settings"
                                                 : "Can view and report on audit data" );
         return snapshot.ToString();                                                 
      }
      
      #endregion

      #region Help
      
      //--------------------------------------------------------------------
      // Form_LoginNew_HelpRequested - Show Context Sensitive Help
      //--------------------------------------------------------------------
      private void Form_LoginNew_HelpRequested(object sender, HelpEventArgs hlpevent)
      {
         string helpTopic = "";
      
         if ( currentPage == 0 ) // general
            helpTopic = HelpAlias.SSHELP_Form_LoginNew_General;
         else if ( currentPage == 1 ) // access
            helpTopic = HelpAlias.SSHELP_Form_LoginNew_Access;
         else if ( currentPage == 2 ) // summary
            helpTopic = HelpAlias.SSHELP_Form_LoginNew_Summary;
      
		   if (helpTopic != "" ) HelpAlias.ShowHelp(this,helpTopic );
			hlpevent.Handled = true;
      }

      #endregion

      private void btnBrowse_Click(object sender, EventArgs e)
      {
         Form_UserBrowse frm = new Form_UserBrowse();
         DialogResult choice = frm.ShowDialog();
         if ( choice == DialogResult.OK )
         {
            textName.Text = frm.userName;
         }
      }

	}
}
