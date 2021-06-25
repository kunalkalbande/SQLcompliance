using System ;
using System.Threading ;
using System.Windows.Forms ;
using Idera.SQLcompliance.Application.GUI.Properties ;
using Idera.SQLcompliance.Core ;
using Idera.SQLcompliance.Core.Service ;
using Timer=System.Windows.Forms.Timer;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
	/// <summary>
	/// Summary description for Form_ServiceProgress.
	/// </summary>
	public partial class Form_ServiceProgress : Form
	{
	   #region Properties
		private Timer poller;
		
		private Thread    m_Thread;
		
		private string    m_computer;
		
		private bool      m_success = false;
		private string    m_errMsg  = "";
		
		public bool       Success { get{ return m_success; } }
		public string     ErrorMessage { get{ return m_errMsg; } }
		
		#endregion

      //-----------------------------------------------------------------------
      // Constructor
      //-----------------------------------------------------------------------
		public
		   Form_ServiceProgress(
		      string            computer,
		      bool              agentService
         )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
         this.Icon = Resources.SQLcompliance_product_ico;

			// Setup the progress bar timer
         poller = new Timer();
         poller.Tick += new EventHandler(TimerUpdateProgressBar);
         poller.Interval = 1000;
         poller.Enabled = true;
         
         m_computer = computer;
         
			// Start the installer/uninstaller
			if ( agentService )
			{
			   tbMessage.Text = String.Format( "Starting SQLcompliance Agent on {0}", computer );
			   m_Thread = new Thread(new ThreadStart(StartAgent));
			}
			else
			{
			   tbMessage.Text = String.Format( "Starting Collection Server on {0}",computer );
			   m_Thread = new Thread(new ThreadStart(StartServer));
			}
			m_Thread.Start();
		}



      //-----------------------------------------------------------------------
      // TimerUpdateProgressBar
      //-----------------------------------------------------------------------
      private void
         TimerUpdateProgressBar(
            Object            myObject,
            EventArgs         myEventArgs
         ) 
		{
			progressBar1.Value = progressBar1.Value == 100 ? 0 : progressBar1.Value + 5;

			if ( ! m_Thread.IsAlive )
			{
				Close();
		   }
     }
     
    //-----------------------------------------------------------------------
    // StartAgent
    //-----------------------------------------------------------------------
     private void StartAgent()
     {
         bool started = false;
         
         try
         {
			   NonWmiServiceManager serviceManager
			      = new NonWmiServiceManager( m_computer,
                                           CoreConstants.AgentServiceName );
		                                     
				// Attempt to start the SQLsecure service
				started = serviceManager.Start();
            if ( ! started )
            {
               m_errMsg = UIConstants.Error_AgentNotInstalled;
            }                                                                  
         }
         catch( Exception ex )
         {
            m_errMsg = ex.Message;
         }
         
         m_success = started;

			Close();
     }
     
    //-----------------------------------------------------------------------
    // StartServer
    //-----------------------------------------------------------------------
     private void StartServer()
     {
         bool started = false;
         
         try
         {
			   NonWmiServiceManager serviceManager
			      = new NonWmiServiceManager( Globals.RepositoryServer,
                                           CoreConstants.CollectionServiceName );
		                                     
				// Attempt to start the SQLsecure service
				started = serviceManager.Start();
				
            if ( ! started )
            {
               m_errMsg = UIConstants.Error_ServerNotInstalled;
            }                                                                  
         }
         catch( Exception ex )
         {
            m_errMsg = ex.Message;
         }

         m_success = started;
     
			Close();
      }
   }
}
