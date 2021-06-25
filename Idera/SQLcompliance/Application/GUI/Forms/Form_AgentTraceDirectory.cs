using System ;
using System.Text ;
using System.Windows.Forms ;
using Idera.SQLcompliance.Application.GUI.Helper ;
using Idera.SQLcompliance.Application.GUI.Properties ;
using Idera.SQLcompliance.Core ;
using Idera.SQLcompliance.Core.Agent ;
using Idera.SQLcompliance.Core.Collector ;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
	/// <summary>
	/// Summary description for Form_AgentTraceDirectory.
	/// </summary>
	public partial class Form_AgentTraceDirectory : Form
	{

      /*
      private Form_MainForm mainForm = null;
      public Form_MainForm MainForm
      {
         get { return this.mainForm;  }
         set { this.mainForm = value; }
      }*/

		
		ServerRecord         oldServer = null;
		public ServerRecord  newServer = null;

		public Form_AgentTraceDirectory(
		      ServerRecord          server
   		)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
         this.Icon = Resources.SQLcompliance_product_ico;

         oldServer = server;

			textAgentTraceDirectory.Text = server.AgentTraceDirectory;
		}


      #region Help
      //--------------------------------------------------------------------
      // Form_AgentTraceDirectory_HelpRequested - Show Context Sensitive Help
      //--------------------------------------------------------------------
      private void Form_AgentTraceDirectory_HelpRequested(object sender, HelpEventArgs hlpevent)
      {
		   HelpAlias.ShowHelp(this,HelpAlias.SSHELP_Form_AgentTraceDirectory);
			hlpevent.Handled = true;
      }
      #endregion

      private void btnCancel_Click(object sender, EventArgs e)
      {
         Close();
      }

      private void btnOK_Click(object sender, EventArgs e)
      {
         if ( oldServer.AgentTraceDirectory == textAgentTraceDirectory.Text )
         {
            // no change
            DialogResult = DialogResult.Cancel;
            Close();
         }
         else
         {
            if ( ChangeTraceDirectory() )
            {
               // update the agent information for all instances
               newServer = oldServer.Clone();
               newServer.Connection = Globals.Repository.Connection;
               newServer.AgentTraceDirectory = textAgentTraceDirectory.Text;
               newServer.Write();
               newServer.CopyTraceDirectoryToAll(oldServer);
               
               // Change Log Entry
               StringBuilder log = new StringBuilder(1024);
               log.AppendFormat( "Agent Properties Change: Server {0}\r\n",
                                 newServer.AgentServer );
               log.Append( "New Settings\r\n" );
               log.AppendFormat( "\tTrace Directory: {0}\r\n", newServer.AgentTraceDirectory );
               log.Append( "Old Settings\r\n" );
               log.AppendFormat( "\tTrace Directory: {0}\r\n", oldServer.AgentTraceDirectory );
               LogRecord.WriteLog( Globals.Repository.Connection,
                                 LogType.ChangeAgentProperties,
                                 newServer.AgentServer,
                                 log.ToString() );                                   
               
               DialogResult = DialogResult.OK;
               Close();
            }
         }
      }
      
      private bool ChangeTraceDirectory()
      {
         bool success = false;
         
         // simple validation before trying to change trace directory
		   if ( ! UIUtils.ValidatePath( textAgentTraceDirectory.Text ) )
         {
            ErrorMessage.Show( this.Text, UIConstants.Error_InvalidTraceDirectory );
            return false;
         }

         try
         {
            this.Cursor = Cursors.WaitCursor;
			 string newTraceDirectory = textAgentTraceDirectory.Text.Trim() ;
            
             AgentManager manager = GUIRemoteObjectsProvider.AgentManager();
			 if(!manager.DirectoryExists(oldServer.Instance, newTraceDirectory))
			 {
				 if(MessageBox.Show(this, String.Format("{0} does not exist.  Do you want to create it?", 
					 newTraceDirectory), "Create New Directory", MessageBoxButtons.YesNo) == DialogResult.No)
				 {
					 return false ;
				 }
			 }
            manager.SetTraceDirectory( oldServer.Instance, newTraceDirectory) ;

            success = true;
         }
         catch (Exception ex )
         {
            ErrorMessage.Show( this.Text,
                               UIConstants.Error_UpdateTraceDirectoryFailed,
                               UIUtils.TranslateRemotingException( Globals.SQLcomplianceConfig.Server,
                                                                   UIConstants.CollectionServiceName,
                                                                   ex ),
                               MessageBoxIcon.Error );
         }
         finally
         {
            this.Cursor = Cursors.Default;
         }
         
         return success;
      }

      private void textAgentTraceDirectory_TextChanged(object sender, EventArgs e)
      {
         if ( textAgentTraceDirectory.Text.Trim() != "" )
            btnOK.Enabled = true; 
         else
            btnOK.Enabled = true; 
      }
	}
}
