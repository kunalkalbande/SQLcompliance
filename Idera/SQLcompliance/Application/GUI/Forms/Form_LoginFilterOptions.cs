using System ;
using System.Text ;
using System.Windows.Forms ;
using Idera.SQLcompliance.Application.GUI.Properties ;
using Idera.SQLcompliance.Core ;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
	/// <summary>
	/// Summary description for Form_LoginFilterOptions.
	/// </summary>
	public partial class Form_LoginFilterOptions : Form
	{
      bool   m_loginCollapse     = true; 
      int    m_loginTimespan     = 60; 
      string m_strLoginTimespan  = "1 hour";
      int    m_loginCacheSize    = 500;
      
      bool   m_loaded            = false;
      
		public Form_LoginFilterOptions()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
         this.Icon = Resources.SQLcompliance_product_ico;

			Cursor = Cursors.WaitCursor;
			
         //------------------------------------------------------
         // Make controls read only unless user has admin access
         //------------------------------------------------------
         if ( ! Globals.isAdmin  )
         {
            foreach ( Control ctrl in this.Controls )
            {
               ctrl.Enabled = false;
            }

            // change buttons
            btnOK.Visible = false;
            btnCancel.Text = "Close";
            btnCancel.Enabled = true;
            this.AcceptButton = btnCancel;
         }

			Cursor = Cursors.Default;
		}

      protected override void OnLoad(EventArgs e)
      {
         SQLcomplianceConfiguration.GetLoginFilterOptions( out m_loginCollapse,
                                                           out m_loginTimespan,
                                                           out m_loginCacheSize ); 

         checkFilterLogins.Checked = m_loginCollapse;
         
         switch ( m_loginTimespan )
         {
            case 15:
               comboTime.Text = "15 minutes";
               break;
            case 30:
               comboTime.Text = "30 minutes";
               break;
            case 60:
               comboTime.Text = "1 hour";
               break;
            case 120:
               comboTime.Text = "2 hours";
               break;
            case 240:
               comboTime.Text = "4 hours";
               break;
            case 480:
               comboTime.Text = "8 hours";
               break;
            case 720:
               comboTime.Text = "12 hours";
               break;
            case 1440:
               comboTime.Text = "1 day";
               break;
            default:
            {
               string tmp = m_loginTimespan.ToString() + " minutes";
               comboTime.Items.Add( tmp );
               comboTime.Text = tmp;
            }
            break;
         } 
         m_strLoginTimespan = comboTime.Text;
         
         SetTimeStatus();
         
         m_loaded = true;
                                                           
         base.OnLoad (e);
      }



      private void btnCancel_Click(object sender, EventArgs e)
      {
         this.DialogResult = DialogResult.Cancel;
         this.Close();
      }

      private void btnOK_Click(object sender, EventArgs e)
      {
         int timeSpan = m_loginTimespan;
         
         if ( checkFilterLogins.Checked )
            timeSpan = ConvertComboTime();
         
         // see if anything changed
         if ( checkFilterLogins.Checked != m_loginCollapse  ||
              ( checkFilterLogins.Checked
                   && (comboTime.Text != m_strLoginTimespan) )
            )
         {
            // settings were changes
            SQLcomplianceConfiguration.SetLoginFilterOptions( checkFilterLogins.Checked,
                                                              timeSpan,
                                                              m_loginCacheSize ); 
            // change log snapshot
			   StringBuilder log = new StringBuilder(1024);
   			
			   log.Append("Login Filtering Options Changed\r\n\r\n");
			   
			   log.Append("Old Settings:\r\n");
			   log.AppendFormat( "\tStatus: {0}",
			                     m_loginCollapse ? "Enabled" : "Disabled");
			   if ( m_loginCollapse )
			   {
               log.AppendFormat( "\r\n\tTime Period: {0}",
                                 m_strLoginTimespan );
            }
            
            log.Append("\r\n");
            log.Append("New Settings:\r\n");
            log.AppendFormat( "\tStatus: {0}",
                              checkFilterLogins.Checked ? "Enabled" : "Disabled");
            if ( checkFilterLogins.Checked )
            {
               log.AppendFormat( "\r\n\tTime Period: {0}",
                                 comboTime.Text );
            }
            
            LogRecord.WriteLog( Globals.Repository.Connection,
                                LogType.LoginFilteringChanged,
                                log.ToString() );
         }
         this.DialogResult = DialogResult.OK;
         this.Close();
      }
      
      private int
         ConvertComboTime()
      {
         int retval ;
         
         switch ( comboTime.Text )
         {
            case "15 minutes":
               retval = 15;
               break;
            case "30 minutes":
               retval = 30;
               break;
            case "1 hour":
               retval = 60;
               break;
            case "2 hours":
               retval = 120;
               break;
            case "4 hours":
               retval = 240;
               break;
            case "8 hours":
               retval = 480;
               break;
            case "12 hours":
               retval = 720;
               break;
            case "1 day":
               retval = 1440;
               break;
            default:
               // the only way this can happen is if we got passed in a number not in the list
               retval = m_loginTimespan;
               break;
         } 
         return retval;
      }

      #region Help
      //--------------------------------------------------------------------
      // Form_Defaults_HelpRequested - Show Context Sensitive Help
      //--------------------------------------------------------------------
      private void Form_Defaults_HelpRequested(object sender, HelpEventArgs hlpevent)
      {
		   HelpAlias.ShowHelp(this,HelpAlias.SSHELP_Form_LoginFiltering);
			hlpevent.Handled = true;
      }
      #endregion

      private void checkFilterLogins_CheckedChanged(object sender, EventArgs e)
      {
         if ( !m_loaded ) return;
         SetTimeStatus();
      }
      
      private void SetTimeStatus()
      {
         comboTime.Enabled   = checkFilterLogins.Checked;
      }
	}
}
