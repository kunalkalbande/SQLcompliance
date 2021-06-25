using System ;
using System.Threading ;
using System.Windows.Forms ;
using Idera.SQLcompliance.Application.GUI.Properties ;
using Idera.SQLcompliance.Core ;
using Timer=System.Windows.Forms.Timer;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
	/// <summary>
	/// Summary description for Form_GroomingProgress.
	/// </summary>
	public partial class Form_IndexProgress : Form
	{
	   #region Properties
		private Timer _poller;
      private delegate void BasicDelegate() ;
		
		private Thread    _indexingThread;
      private string _message ;
		
		private SystemDatabaseRecord[]    _records;
		
		private bool      _success = true;
		private string    _errMsg  = "";
		public bool       Success          { get { return _success; } }
		public string     ErrMsg           { get { return _errMsg; } }
		
		#endregion

      //-----------------------------------------------------------------------
      // Constructor
      //-----------------------------------------------------------------------
		public Form_IndexProgress(SystemDatabaseRecord[] records)
		{
         _records = records ;
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
         this.Icon = Resources.SQLcompliance_product_ico;

	      _tbMessage.Text     = "Staring Index Updates..." ;
			
			// Setup the progress bar timer
         _poller = new Timer();
         _poller.Tick += new EventHandler(TimerUpdateProgressBar);
         _poller.Interval = 1000;
         _poller.Enabled = true;

			// Start the installer/uninstaller
			_indexingThread = new Thread(new ThreadStart(StartIndex));
			_indexingThread.Start();
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
			_progressBar.Value = _progressBar.Value == 100 ? 0 : _progressBar.Value + 5;

			if ( ! _indexingThread.IsAlive )
			{
				Close();
		   }
     }

      //-----------------------------------------------------------------------
      // StartGroom
      //-----------------------------------------------------------------------
		private void StartIndex() 
		{
         try
         {
            foreach(SystemDatabaseRecord record in _records)
            {
               UpdateIndexMessage(record.DatabaseName) ;
               if(String.Equals(record.DatabaseType, "System"))
               {
                  // SQLcompliance Database
                  Repository.BuildIndexes(Globals.Repository.Connection) ;
                  Globals.SQLcomplianceConfig.SqlComplianceDbSchemaVersion 
                      = CoreConstants.RepositorySqlComplianceDbSchemaVersion;
               }
               else 
               {
                  EventDatabase.UpdateIndexes(Globals.Repository.Connection, record.DatabaseName);
               }
            }
            _success          = true;
			   _errMsg           = "";
         }
         catch (Exception ex)
         {
            _poller.Stop();
            
            _success = false;
            _errMsg  = ex.Message;
         }
			Close();
		}

      private void UpdateIndexMessage(string databaseName)
      {
         _message = String.Format("Updating indexes for database {0}", databaseName) ;
         UpdateMessage() ;
      }

      private void UpdateMessage()
      {
         if(InvokeRequired)
         {
            this.Invoke(new BasicDelegate(UpdateMessage)) ;
            return ;
         }
         _tbMessage.Text = _message ;
      }
   }
}
