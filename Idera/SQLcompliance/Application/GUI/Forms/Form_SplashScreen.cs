using System ;
using System.Drawing ;
using System.Threading ;
using System.Windows.Forms ;
using Idera.SQLcompliance.Application.GUI.Helper ;
using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Cwf;
using Microsoft.Win32;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
   internal delegate void StatusUpdateMethod(string s) ;
	/// <summary>
	/// Summary description for SplashScreen.
	/// </summary>
	public partial class SplashScreen : Form
	{
      private bool _isConnected ;
      private string _server ;
      private Form_Main2 _mainForm ;

		/// <summary>
		/// Constructor
		/// </summary>
		public SplashScreen()
		{
         CheckForIllegalCrossThreadCalls = false;
			InitializeComponent();

			// I am hard coding these to match EXACT image dimensions so that IDE doesn't change em.
			this.ClientSize = new Size(632, 406);
		}

      public Form_Main2 MainForm2
	   {
	      get { return _mainForm ; }
      }


	   public bool IsConnected
	   {
	      get { return _isConnected; }
	      set { _isConnected = value; }
	   }

	   private void UpdateStatus(string s)
      {
         if (InvokeRequired)
         {
            Invoke(new StatusUpdateMethod(UpdateStatus), new object[] {s});
            return;
         }
         lblStatus.Text = s;
         lblStatus.Update() ;
      }

      private void SplashScreen_Load(object sender, EventArgs e)
      {
         UpdateStatus("Initializing...");
      }

      private void DoSplashWork()
      {
         _isConnected = false;

         Globals.ImportPreferences() ;

         //------------------------------------
         // reconnect to last known connection if possible
         //------------------------------------
         if (RecentlyUsedList.Instance.Count != 0)
            _server = RecentlyUsedList.Instance.GetItem(0);
         else
            _server = ReadServerFromRegistry() ;
         while(!_isConnected)
         {
            if(_server == null)
            {
               Form_Connect connectForm = new Form_Connect();
               if (connectForm.ShowDialog(this) == DialogResult.OK)
                  _server = connectForm.Server ;
               else
               {
                  return;
               }
            }

            UpdateStatus(String.Format(UIConstants.ConnectingSplash, _server));
            _isConnected = Form_Main2.ConnectToServer(this, _server, false);
            if(!_isConnected)
               _server = null ;
         }

         // initialize CWF helper
         if (!CwfHelper.Instance.IsInitialized)
             CwfHelper.Instance.Initialize(Globals.RepositoryServer);

         LoadMainForm() ;
      }

      //
      // For new console installs alongside a collection server, this retrieves the 
      //  first repository to connect to.
      private string ReadServerFromRegistry()
      {
         RegistryKey rk = null;
         RegistryKey rks = null;
         string serverInstance = null ;

         try
         {
            rk = Registry.LocalMachine;
            rks = rk.CreateSubKey(CoreConstants.CollectionService_RegKey, RegistryKeyPermissionCheck.ReadSubTree);

            serverInstance = (string)rks.GetValue(CoreConstants.CollectionService_RegVal_ServerInstance, null);
         }
         catch (Exception)
         {
         }
         finally
         {
            if (rks != null) rks.Close();
            if (rk != null) rk.Close();
         }
         if(serverInstance != null)
            return serverInstance.ToUpper() ;
         else 
            return null ;
      }

	   private void LoadMainForm()
      {
         if(InvokeRequired)
         {
            Invoke(new ThreadStart(LoadMainForm)) ;
            return ;
         }
         UpdateStatus("Loading SQL Compliance Manager...");
         _mainForm = new Form_Main2();
      }

      private void SplashScreen_Shown(object sender, EventArgs e)
      {
         Update() ;
         DoSplashWork();
         Close() ;
      }
	}
}
