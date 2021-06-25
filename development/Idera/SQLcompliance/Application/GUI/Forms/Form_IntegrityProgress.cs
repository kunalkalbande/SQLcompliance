using System ;
using System.Net.Sockets ;
using System.Threading ;
using System.Windows.Forms ;
using Idera.SQLcompliance.Application.GUI.Helper;
using Idera.SQLcompliance.Application.GUI.Properties ;
using Idera.SQLcompliance.Core ;
using Idera.SQLcompliance.Core.Collector ;
using Idera.SQLcompliance.Core.Event ;
using Timer=System.Windows.Forms.Timer;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
	/// <summary>
	/// Summary description for Form_IntegrityProgress.
	/// </summary>
	public partial class Form_IntegrityProgress : Form
	{
	   #region Properties
		private Timer poller;
		
		private Thread    m_checkingThread;
		
		private string    m_instance;
		private string    m_database;
		private bool      m_isArchive;
		private bool      m_fixProblems;
		
      public  bool      callFailed = false;
		public string     errMsg1    = "";
		public string     errMsg2    = "";
		
		public  CheckResult    checkResult;
      public  EventRecord[]  badEvents     = null;
      public  int[]          badEventTypes = null;
		
		#endregion

      //-----------------------------------------------------------------------
      // Constructor
      //-----------------------------------------------------------------------
		public
		   Form_IntegrityProgress(
		      string            instance,
		      string            database,
		      bool              isArchive,
		      bool              fixProblems
         )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
         this.Icon = Resources.SQLcompliance_product_ico;
			
			m_instance    = instance;
			m_database    = database;
			m_isArchive   = isArchive;
			m_fixProblems = fixProblems;

         if ( m_fixProblems )
         {
	         tbMessage.Text = "Marking compromised events for database " + m_database;
         }
         else
         {
	         tbMessage.Text = "Checking integrity of database " + m_database;
         }

			// Setup the progress bar timer
         poller = new Timer();
         poller.Tick += new EventHandler(TimerUpdateProgressBar);
         poller.Interval = 1000;
         poller.Enabled = true;

			// Start the installer/uninstaller
			m_checkingThread = new Thread(new ThreadStart(StartIntegrityCheck));
			m_checkingThread.Start();
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

			if ( ! m_checkingThread.IsAlive )
			{
				Close();
		   }
     }

      //-----------------------------------------------------------------------
      // StartIntegrityCheck
      //-----------------------------------------------------------------------
		private void StartIntegrityCheck() 
		{
         try
         {
             RemoteCollector srv = GUIRemoteObjectsProvider.RemoteCollector();
                                        
            checkResult = srv.CheckIntegrity( m_instance,
                                              m_database,
                                              m_isArchive,
                                              m_fixProblems,
                                              out badEvents,
                                              out badEventTypes );
            if ( checkResult.integrityCheckError != "" )
            {
               errMsg1 = CoreConstants.Error_IntegrityCheckError;
               errMsg2 = checkResult.integrityCheckError;
               
               callFailed = true;
            }
            else
            {
               callFailed = false;                                         
            }
         }
         catch ( SocketException socketEx )
         {
            callFailed = true;
            if ( socketEx.ErrorCode == 10061 )
            {
                errMsg1 = String.Format( UIConstants.Error_ServerNotAvailable,
                                         UIConstants.CollectionServiceName,
                                         Globals.SQLcomplianceConfig.Server );
                errMsg2 = "";
            }
            else
            {
               errMsg1 = CoreConstants.Error_IntegrityCheckError;
               errMsg2 = socketEx.Message;
            }
         }
         catch (Exception ex)
         {
            callFailed = true;
            errMsg1 = CoreConstants.Error_IntegrityCheckError;
            errMsg2 = ex.Message;
         }
		}
   }
}
