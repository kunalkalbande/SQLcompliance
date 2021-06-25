using System;
using System.Threading;
using Idera.SQLcompliance.Application.GUI.Properties ;
using Idera.SQLcompliance.Core.Rules.Alerts;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
	/// <summary>
	/// Summary description for Form_GroomingProgress.
	/// </summary>
	public partial class Form_AlertGroomProgress : System.Windows.Forms.Form
	{
	   #region Properties
		
		private Thread    m_groomingThread;
		
		private string    m_instance;
		private int       m_age;
      public int NumberRemoved ;
      private AlertingConfiguration _configuration ;
		
		private bool      m_success = true;
		private string    m_errMsg  = "";
		public bool       Success          { get { return m_success; } }
		public string     ErrMsg           { get { return m_errMsg; } }
		
		#endregion

      //-----------------------------------------------------------------------
      // Constructor
      //-----------------------------------------------------------------------
		public
		   Form_AlertGroomProgress(string instance, int age, AlertingConfiguration configuration)
		{
         _configuration = configuration ;
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
         this.Icon = Resources.SQLcompliance_product_ico;

	      tbMessage.Text     = tbMessage.Text + instance;
			
			m_instance         = instance;
			m_age              = age;

			// Setup the progress bar timer
         poller = new System.Windows.Forms.Timer();
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
            DateTime groomDate = DateTime.Now.AddDays(- m_age) ;
            // Grooming
            if ( m_instance != null )
            {
               // Groom one instance
               NumberRemoved = AlertingDal.DeleteAlerts(m_instance, groomDate, _configuration.ConnectionString) ;
            }
            else
            {
               // Groom all instance
               NumberRemoved = AlertingDal.DeleteAlerts(groomDate, _configuration.ConnectionString) ;
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
