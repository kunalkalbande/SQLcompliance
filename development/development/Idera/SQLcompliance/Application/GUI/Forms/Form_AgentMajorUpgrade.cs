//--------------------------------------------------------------------
// Form_AgentMajorUpgrade
//
// Wizard for gathering data necessary for a remote major upgrade of deployed agents
//
// (c) Copyright 2004; Idera, inc. ALL RIGHTS RESERVED
//--------------------------------------------------------------------

using System ;
using System.Windows.Forms ;
using Idera.SQLcompliance.Application.GUI.Helper ;
using Idera.SQLcompliance.Application.GUI.Properties ;
using Idera.SQLcompliance.Core ;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
   /// <summary>
   /// Summary description for Form_AgentMajorUpgrade.
   /// </summary>
   public partial class Form_AgentMajorUpgrade : Form
   {
      #region Constructor / Dispose

      public
         Form_AgentMajorUpgrade(
         string instance
         )
      {
         //
         // Required for Windows Form Designer support
         //
         InitializeComponent() ;
         this.Icon = Resources.SQLcompliance_product_ico;

         //
         // TODO: Add any constructor code after InitializeComponent call
         //
         m_instance = instance ;

         this.panelAgentService.Enabled = false ;
         this.panelSummary.Enabled = false ;

         currentPage = 0 ;
         SetCurrentPage() ;

         this.ActiveControl = textServiceAccount ;
      }

      #endregion


      #region Properties

      private string m_instance ;

      public string ServiceAccount = "" ;
      public string ServicePassword = "" ;

      #endregion

      #region Page Navigation

      //--------------------------------------------------------------------
      // btnNext_Click - Next button; move forward a page in the wizard
      //--------------------------------------------------------------------
      private void btnNext_Click(object sender, EventArgs e)
      {
         if(ValidatePage(currentPage))
         {
            ChangeWizardPage(WizardAction.Next) ;
         }
      }

      //--------------------------------------------------------------------
      // btnBack_Click - Back button; move back a page in the wizard
      //--------------------------------------------------------------------
      private void btnBack_Click(object sender, EventArgs e)
      {
         ChangeWizardPage(WizardAction.Prev) ;
      }

      //--------------------------------------------------------------------
      // ValidatePage - Simple validation done as users switches pages with
      //                back and next. More extensive validation is done
      //                after Finish is pressed.
      //--------------------------------------------------------------------
      private bool ValidatePage(int page)
      {
         if(page == 0) // agent service account
         {
            if(! ValidateAccountName())
            {
               textServiceAccount.Focus() ;
               ErrorMessage.Show(this.Text,
                                 UIConstants.Error_InvalidServiceAccountName) ;
               return false ;
            }

            if(textServicePassword.Text != textServicePasswordConfirm.Text)
            {
               textServicePassword.Focus() ;
               ErrorMessage.Show(this.Text,
                                 UIConstants.Error_MismatchedPasswords) ;
               return false ;
            }
            if(InstallUtil.VerifyPassword(textServiceAccount.Text, textServicePassword.Text) != 0)
            {
               ErrorMessage.Show(this.Text, UIConstants.Error_InvalidDomainCredentials) ;
               return false ;
            }
         }
         return true ;
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
         string domain ;
         string account ;

         string tmp = textServiceAccount.Text.Trim() ;

         int pos = tmp.IndexOf(@"\") ;
         if(pos <= 0)
         {
            return false ;
         }
         else
         {
            domain = tmp.Substring(0, pos) ;
            account = tmp.Substring(pos + 1) ;

            if((domain == "") || (account == ""))
               return false ;
         }

         return true ;
      }

      //--------------------------------
      // Wizard State Machine Constants
      //--------------------------------
      private int numPages = 3 ;
      private int currentPage = 0 ;
      private Panel currentPanel = null ;

      private enum WizardAction
      {
         Next = 0,
         Prev = 1
      } ;

      //--------------------------------------------------------------------
      // ChangeWizardPage - Move forward or backwards in the wizard
      //--------------------------------------------------------------------
      private void
         ChangeWizardPage(
         WizardAction direction
         )
      {
         // Change Page
         if(direction == WizardAction.Next)
         {
            if(currentPage < (numPages - 1))
            {
               currentPage ++ ;
            }
         }
         else // Prev
         {
            if(currentPage > 0)
            {
               currentPage -- ;
            }
         }

         SetCurrentPage() ;
      }

      //--------------------------------------------------------------------
      // SetCurrentPage - Make sure the current page is visible and buttons 
      //                  are enabled/disabled appropriately
      //--------------------------------------------------------------------
      private void SetCurrentPage()
      {
         Panel oldPanel = currentPanel ;

         if(currentPage == 0)
         {
            currentPanel = this.panelAgentService ;
            SetButtonState(false, /* back   */
                           true, /* next */
                           false) ; /* finish */
         }
         else if(currentPage == 1)
         {
            currentPanel = this.panelSummary ;

            textSummaryComputer.Text = UIUtils.GetInstanceHost(m_instance) ;
            textSummaryAccount.Text = textServiceAccount.Text ;

            SetButtonState(true, /* back   */
                           false, /* next   */
                           true) ; /* finish */
         }
         else
         {
            //internal error
         }

         if((currentPanel != null) && (currentPanel != oldPanel))
         {
            if(oldPanel != null)
               oldPanel.Enabled = false ;

            currentPanel.Enabled = true ;
            currentPanel.BringToFront() ;

            // set focus
            if(currentPage == 0)
            {
               textServiceAccount.Focus() ;
            }
            else if(currentPage == 1)
            {
               btnFinish.Focus() ;
            }
         }

         if(btnNext.Enabled)
         {
            this.AcceptButton = btnNext ;
         }
         else if(btnFinish.Enabled)
         {
            this.AcceptButton = btnFinish ;
         }
      }

      //--------------------------------------------------------------------
      // SetButtonState - Set back,next, finish enabled state
      //--------------------------------------------------------------------
      private void SetButtonState(bool back, bool next, bool finish)
      {
         btnBack.Enabled = back ;
         btnNext.Enabled = next ;
         btnFinish.Enabled = finish ;
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
         for(int i = 0; i < numPages; i++)
         {
            if(! ValidatePage(i))
            {
               currentPage = i ;
               SetCurrentPage() ;
               return ;
            }
         }

         ServiceAccount = textServiceAccount.Text ;
         ServicePassword = textServicePassword.Text ;
         this.DialogResult = DialogResult.OK ;
         this.Close() ;
      }

      #endregion

      #region Help

      //--------------------------------------------------------------------
      // Form_AgentMajorUpgrade_HelpRequested - Show Context Sensitive Help
      //--------------------------------------------------------------------
      private void Form_AgentMajorUpgrade_HelpRequested(object sender, HelpEventArgs hlpevent)
      {
      }

      #endregion
   }
}
