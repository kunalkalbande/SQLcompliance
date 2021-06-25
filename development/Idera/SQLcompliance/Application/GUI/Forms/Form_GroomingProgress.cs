using System ;
using System.Threading ;
using System.Windows.Forms ;
using Idera.SQLcompliance.Application.GUI.Helper;
using Idera.SQLcompliance.Application.GUI.Properties ;
using Idera.SQLcompliance.Core.Collector ;
using Timer=System.Windows.Forms.Timer;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
	/// <summary>
	/// Summary description for Form_GroomingProgress.
	/// </summary>
	public partial class Form_GroomingProgress : Form
	{
	   #region Properties
		private Timer poller;
		
		private Thread    m_groomingThread;
		
		private string    m_instance;
		private int       m_age;
      private IntegrityCheckAction _icAction ;
		
		private bool      m_success = true;
		private string    m_errMsg  = "";
		public bool       Success          { get { return m_success; } }
		public string     ErrMsg           { get { return m_errMsg; } }
		
		#endregion

      //-----------------------------------------------------------------------
      // Constructor
      //-----------------------------------------------------------------------
		public
		   Form_GroomingProgress(string instance, int age, IntegrityCheckAction icAction)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
         this.Icon = Resources.SQLcompliance_product_ico;

	      tbMessage.Text     = tbMessage.Text + instance;
			
			m_instance         = instance;
			m_age              = age;
         _icAction = icAction ;

			// Setup the progress bar timer
         poller = new Timer();
         poller.Tick += new EventHandler(TimerUpdateProgressBar);
         poller.Interval = 1000;
         poller.Enabled = true;

			// Start the installer/uninstaller
			m_groomingThread = new Thread(new ThreadStart(StartGroom));
			m_groomingThread.Start();
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

			if ( ! m_groomingThread.IsAlive )
			{
				Close();
		   }
     }

      //-----------------------------------------------------------------------
      // StartGroom
      //-----------------------------------------------------------------------
		private void StartGroom() 
		{
         try
         {
             RemoteCollector srv = GUIRemoteObjectsProvider.RemoteCollector();

            // Always skip integrity check since it is done in Form_groom
            if(m_instance == null || m_instance.Length == 0)
            {
               // Groom all instances - This never gets called
               srv.GroomAll(m_age, _icAction) ;
            }
            else
            {
               // Groom a single instance
               srv.Groom(m_instance, m_age, _icAction) ;
            }
			   
			   m_success          = true;
			   m_errMsg           = "";
         }
         catch (Exception ex)
         {
            poller.Stop();
            
            m_success = false;
            m_errMsg  = ex.Message;
         }
			Close();
		}
   }
}
