using System ;
using System.Threading ;
using System.Windows.Forms ;
using Idera.SQLcompliance.Application.GUI.Helper ;
using Idera.SQLcompliance.Application.GUI.Properties ;
using Idera.SQLcompliance.Core ;
using Idera.SQLcompliance.Core.Security ;
using Timer=System.Windows.Forms.Timer;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
   public enum DeploymentType
   {
      Install,
      MinorUpgrade,
      MajorUpgrade
   } ;

	/// <summary>
	/// Summary description for ProgressForm.
	/// </summary>
	public partial class ProgressForm : Form
	{
	   #region Properties
	
		private Thread    m_startServiceThread;

		private bool      m_isServiceStarted = false;
		private bool      m_cancelled = false;
		
		private string    m_localAdmin    = null;
		private string    m_localPassword = null;
		
		private DeploymentType      m_deploymentType;
		private string    m_remoteServer;

		private string m_remoteAdmin = null ;
		private string m_remotePassword = null ;
		
		private string    m_serviceAccount;
		private string    m_servicePassword;
		private string    m_traceDirectory;
		private string    m_instance;
		private string    m_collectionServer;
		
		#endregion

      //-----------------------------------------------------------------------
      // Constructor
      //-----------------------------------------------------------------------
		public
		   ProgressForm(
		      string            message,
		      string            remoteServer,     // machine on which install will be run
		      string            serviceAccount,
		      string            servicePassword,
		      string            traceDirectory,
		      string            instance,         // instance to be audited on remote machine
		      string            collectionServer, // home - computer hostng collection service
		      DeploymentType    deploymentType
         )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
         this.Icon = Resources.SQLcompliance_product_ico;

			tbMessage.Text     = message;
		   m_remoteServer     = remoteServer;
		   m_serviceAccount   = serviceAccount;
		   m_servicePassword  = servicePassword;
		   m_traceDirectory   = traceDirectory;
		   m_instance         = instance;
		   m_collectionServer = collectionServer;
		   m_deploymentType          = deploymentType ;

			// Setup the progress bar timer
         poller = new Timer();
         poller.Tick += new EventHandler(TimerUpdateProgressBar);
         poller.Interval = 1000;
         poller.Enabled = true;

			// Start the installer/uninstaller
			m_startServiceThread = new Thread(new ThreadStart(StartService));
			m_startServiceThread.Start();
		}

      //-----------------------------------------------------------------------
      // IsServiceStarted
      //-----------------------------------------------------------------------
		public bool IsServiceStarted
		{
			get { return m_isServiceStarted; }
			set { m_isServiceStarted = value; }
		}

      //-----------------------------------------------------------------------
      // IsCancelled
      //-----------------------------------------------------------------------
		public bool IsCancelled
		{
			get { return m_cancelled; }
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

			if ( ! m_startServiceThread.IsAlive )
			{
				Close();
		   }
     }

      //-----------------------------------------------------------------------
      // StartService
      //-----------------------------------------------------------------------
		private void StartService() 
		{
			UidPwdForm uidpwdForm = new UidPwdForm();

			bool keepTrying = true;

			while (keepTrying) 
			{
				try 
				{
					// Tell user that we are trying to install/start Backup Agent
					Cursor.Current = Cursors.WaitCursor;
					
               switch(m_deploymentType)
               {
                  case DeploymentType.Install:
                     AgentControl.RemoteInstallAgentService( m_localAdmin,
                        m_localPassword,
                        m_remoteServer,
                        m_remoteAdmin,
                        m_remotePassword,
                        m_serviceAccount,
                        m_servicePassword,
                        m_instance,
                        m_traceDirectory,
                        m_collectionServer );
                     break ;
                  case DeploymentType.MinorUpgrade:
                     // Upgrade the remote backup service
                     AgentControl.RemoteMinorUpgradeAgentService( m_localAdmin,
                        m_localPassword,
                        m_remoteServer,
                        m_remoteAdmin, 
                        m_remotePassword,
                        m_collectionServer);
                     break ;
                  case DeploymentType.MajorUpgrade:
                     // Upgrade the remote backup service
                     AgentControl.RemoteMajorUpgradeAgentService( m_localAdmin,
                        m_localPassword,
                        m_remoteServer,
                        m_remoteAdmin, 
                        m_remotePassword,
                        m_serviceAccount,
                        m_servicePassword) ;
                     break ;
               }

               Cursor.Current = this.Cursor;

					// If we got here without exception, then we've been successful
					m_isServiceStarted = true;
					keepTrying = false;
				} 
				catch (LocalSecurityException se) 
				{			
					// User does not have local admin access
					// User does not have remote admin access
					ErrorLog.Instance.Write(se, true);

					uidpwdForm.MessageForUser = UIConstants.PleaseEnterAdministrativeCredentials;

					// if RemoteAdmin variables have values, populate them into the form
					if ( (m_localAdmin != null) && (!m_localAdmin.Trim().Equals("")) ) 
					{
						uidpwdForm.Login    = m_localAdmin;
						uidpwdForm.Password = m_localPassword;
					}
					Cursor.Current = this.Cursor;
					if ( uidpwdForm.ShowDialog(this) == DialogResult.OK ) 
					{
						if ( (uidpwdForm.Login == null) || (uidpwdForm.Login.Trim().Equals("")) ) 
						{
							m_localAdmin    = null;
						   m_localPassword = null;
						}
						else 
						{
						   m_localAdmin    = uidpwdForm.Login;
							m_localPassword = uidpwdForm.Password;
						}
					}
					else 
					{
						MessageBox.Show(this, "SQLcompliance Agent could not be installed.", "SQLcompliance Agent Not Installed");
						Close();
						keepTrying = false;
					}
				} 
				catch (RemoteSecurityException se) 
				{
					// User does not have remote admin access
					ErrorLog.Instance.Write(se, true);

					/*
					uidpwdForm.MessageForUser = UIConstants.PleaseEnterAdministrativeCredentialsFor + m_remoteServer+".";

					// if RemoteAdmin variables have values, populate them into the form
					if ( (m_remoteAdmin != null) && (!m_remoteAdmin.Trim().Equals("")) ) 
					{
						uidpwdForm.Login    = m_remoteAdmin;
						uidpwdForm.Password = m_remotePassword;
					}
					Cursor.Current = this.Cursor;
					if ( uidpwdForm.ShowDialog(this) == DialogResult.OK ) 
					{
						if ( (uidpwdForm.Login == null) || (uidpwdForm.Login.Trim().Equals("")) ) 
						{
							m_remoteAdmin  = null;
							m_remotePassword = null;
						}
						else 
						{
							m_remoteAdmin  = uidpwdForm.Login;
							m_remotePassword = uidpwdForm.Password;
						}
					}
					else */
					{
						MessageBox.Show(this, "SQLcompliance Agent could not be installed.  Access is denied.", "SQLcompliance Agent Not Installed");
						Close();
						keepTrying = false;
					}
				}
				catch (Exception ex) 
				{
					keepTrying = false;
					Cursor.Current = this.Cursor;

					// If the deployment hasn't been cancelled, the failure is most likely could not find msi file
					if (!m_cancelled) 
					{
						MessageBox.Show(this,
						                String.Format( "The SQLcompliance Agent on {0} could not be {1} for the following reason:\n\nError: {2}",
						                               m_remoteServer,
                                                (this.m_deploymentType == DeploymentType.Install) ? "started or installed" : "upgraded" ,
						                               ex.Message ),
                                  "SQLcompliance Agent Error" );
						Close();
						ErrorLog.Instance.Write(ex,true);
					}
				}
			}
		}

      //-----------------------------------------------------------------------
      // buttonClose_Click
      //-----------------------------------------------------------------------
		private void buttonClose_Click(object sender, EventArgs e)
		{
			m_cancelled = true;

			if (m_startServiceThread.IsAlive)
			{
				m_startServiceThread.Abort();
	      }
		}
   }
}
