using System ;
using System.Net.Sockets ;
using System.Security.Principal ;
using System.Threading ;
using System.Windows.Forms ;
using Idera.SQLcompliance.Application.GUI.Helper;
using Idera.SQLcompliance.Application.GUI.Properties ;
using Idera.SQLcompliance.Core.Collector ;
using Timer=System.Windows.Forms.Timer;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
	/// <summary>
	/// Summary description for Form_ArchiveProgress.
	/// </summary>
	public partial class Form_ArchiveProgress : Form
	{
	   #region Properties
		
		private Thread    m_archiveThread;
		
		private string    m_instance;
		
		private bool      m_success = false;
		public bool       Success { get{ return m_success; } }
		public string     errMsg1 = "";
		public string     errMsg2 = "";
		
		#endregion

      //-----------------------------------------------------------------------
      // Constructor
      //-----------------------------------------------------------------------
		public
		   Form_ArchiveProgress(
		      string            instance
         )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
         this.Icon = Resources.SQLcompliance_product_ico;

	      tbMessage.Text     = tbMessage.Text + instance;
			
			m_instance         = instance;

			// Setup the progress bar timer
         poller = new Timer();
         poller.Tick += new EventHandler(TimerUpdateProgressBar);
         poller.Interval = 1000;
         poller.Enabled = true;

			// Start the installer/uninstaller
			m_archiveThread = new Thread(new ThreadStart(StartArchive));
			m_archiveThread.Start();
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

			if ( ! m_archiveThread.IsAlive )
			{
				Close();
		   }
     }
     
    //-----------------------------------------------------------------------
    // StartArchive
    //-----------------------------------------------------------------------
     private void StartArchive()
     {
         try
         {
            RemoteCollector srv = GUIRemoteObjectsProvider.RemoteCollector();
            // fake impersonation
            WindowsIdentity id = WindowsIdentity.GetCurrent();
            ArchiveSettings settings = new ArchiveSettings() ;
            settings.TargetInstance = m_instance ;
            settings.Background = false ;
            settings.User = id.Name ;
            if(Globals.SQLcomplianceConfig.ArchiveCheckIntegrity)
               settings.IntegrityCheckAction = IntegrityCheckAction.CheckAlreadyDone ;
            else
               settings.IntegrityCheckAction = IntegrityCheckAction.SkipCheck ;
            srv.Archive(settings) ;
            
            m_success = true;
            errMsg1   = "";
            errMsg2   = "";
         }
         catch ( SocketException socketEx )
         {
            m_success = false;
            if ( socketEx.ErrorCode == 10061 )
            {
                errMsg1 = String.Format( UIConstants.Error_ServerNotAvailable,
                                         UIConstants.CollectionServiceName,
                                         Globals.SQLcomplianceConfig.Server );
                errMsg2 = "";
            }
            else
            {
               errMsg1 = UIConstants.Error_ArchiveNowFailed;
               errMsg2 = socketEx.Message;
            }
         }
         catch (Exception ex)
         {
            m_success = false;
            errMsg1 = UIConstants.Error_ArchiveNowFailed;
            errMsg2 = ex.Message;
         }
			Close();
     }
   }
}
