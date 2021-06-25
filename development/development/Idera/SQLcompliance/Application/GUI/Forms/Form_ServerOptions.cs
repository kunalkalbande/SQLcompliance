using System ;
using System.ServiceProcess ;
using System.Windows.Forms ;
using Idera.SQLcompliance.Application.GUI.Helper ;
using Idera.SQLcompliance.Application.GUI.Properties ;
using Idera.SQLcompliance.Core ;
using Idera.SQLcompliance.Core.Collector ;
using Idera.SQLcompliance.Core.Service ;
using Microsoft.Win32;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
	/// <summary>
	/// Summary description for Form_ServerOptions.
	/// </summary>
	public partial class Form_ServerOptions : Form
	{
      //--------------------------------------------------------------------
      // Constructor
      //--------------------------------------------------------------------
		public Form_ServerOptions()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
         this.Icon = Resources.SQLcompliance_product_ico;

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
	   }
	   
      //--------------------------------------------------------------------
      // OnLoad
      //--------------------------------------------------------------------
      protected override void OnLoad(EventArgs e)
      {
         RefreshValues();
         base.OnLoad (e);
      }
      
      //--------------------------------------------------------------------
      // RefreshValues
      //--------------------------------------------------------------------
      private void RefreshValues()
      {
			Cursor = Cursors.WaitCursor;
			LoadConfiguration();
			GetServerStatus();
         ReadLogLevel();
			Cursor = Cursors.Default;
      }
      
      //--------------------------------------------------------------------
      // LoadConfiguration
      //--------------------------------------------------------------------
      private void LoadConfiguration()
      {
			// Reload globals from repository
			Globals.SQLcomplianceConfig.Read( Globals.Repository.Connection );
			
			// fill dialog values
         textServerPort.Text    = Globals.SQLcomplianceConfig.ServerPort.ToString();			                                
         textServerMachine.Text = Globals.SQLcomplianceConfig.Server;
         textServerLastHeartbeatTime.Text = UIUtils.GetLocalTimeDateString(Globals.SQLcomplianceConfig.ServerLastHeartbeatTime);
         heartbeatIntervalText.Text = Globals.SQLcomplianceConfig.CollectionServerHeartbeatInterval.ToString();
			
         // Versions         
			textServerVersion.Text  = ( Globals.SQLcomplianceConfig.ServerVersion == "" )
			                              ? UIConstants.ServerStatus_Unknown
			                              : Globals.SQLcomplianceConfig.ServerVersion;
			textServerCoreVersion.Text = ( Globals.SQLcomplianceConfig.ServerCoreVersion == "" )
			                              ? UIConstants.ServerStatus_Unknown
			                              : Globals.SQLcomplianceConfig.ServerCoreVersion;
		}

      /// <summary>
      /// 
      /// </summary>
      private void ReadLogLevel()
      {
         RegistryKey rk = null;
         RegistryKey rks = null;

         try
         {
            rk = Registry.LocalMachine;
            rks = rk.CreateSubKey(CoreConstants.CollectionService_RegKey);

            // log level
            int lvl = (int)rks.GetValue(CoreConstants.CollectionService_RegVal_LogLevel, 1);

            if (lvl < 0)
            {
               lvl = 1;
            }

            if (lvl > 4)
            {
               lvl = 4;
            }
            comboLogLevel.SelectedIndex = lvl - 1;
         }
         catch
         {
         }
         finally
         {
            if (rks != null) rks.Close();
            if (rk != null) rk.Close();
         }
      }

      //--------------------------------------------------------------------
      // GetServerStatus
      //--------------------------------------------------------------------
      private void GetServerStatus()
      {
         ServiceControllerStatus status ;
         bool serviceDown = false;
         
         // first see if service is event installed and running
         try
         {
			   NonWmiServiceManager serviceManager = new NonWmiServiceManager( Globals.RepositoryServer, CoreConstants.CollectionServiceName );
				status = serviceManager.GetServiceStatus();
				
				if ( status != ServiceControllerStatus.StartPending  && 
				     status != ServiceControllerStatus.Running )
				{
				   serviceDown = true;
				}
			
	      }
	      catch
	      {
	         // if we fail with a permissions error, then we still might be able to talk
	         // via .NET remoting to server - so pretend things are ok but service is down
            status = ServiceControllerStatus.Stopped;
  	      }

         if ( !serviceDown )
         {
            try
            {
                RemoteCollector srv = GUIRemoteObjectsProvider.RemoteCollector();
                                                                              
               string traceDirectory;
               int    logLevel;
               int    activityLogLevel;
               
               srv.GetStatus( out traceDirectory,
                              out logLevel,
                              out activityLogLevel );
                              
               // if we are here the servers is up - lets see if it is heartbeating
               if ( UIUtils.IsHeartbeatStale( Globals.SQLcomplianceConfig.ServerLastHeartbeatTime ) )
               {
                  //server is up but hasnt reported in for awhile
                  textServerStatus.Text   = UIConstants.ServerStatus_Unknown;
                  imgStatus.Image         = Resources.StatusWarning_48;
                  lblBigStatus.Text       = UIConstants.ServerStatus_UnknownVerbose;
               }
               else
               {
                  // all is well in the kingdom
                  textServerStatus.Text   = UIConstants.ServerStatus_OK;
                  imgStatus.Image = Resources.StatusGood_48;
                  lblBigStatus.Text    = UIConstants.ServerStatus_OKVerbose;
               }
               textTraceDirectory.Text   = traceDirectory;
               comboLogLevel.Text        = UIUtils.GetLogLevelString(logLevel);
               textActivityLogLevel.Text = UIUtils.GetAgentLogLevelString(activityLogLevel);
               
               // service control buttons
               btnStart.Enabled   = false;
               btnStop.Enabled    = true;
               serviceDown = false;
            }
            catch (Exception )
            {
               if ( status == ServiceControllerStatus.StartPending || status == ServiceControllerStatus.Running )
               {
                  lblBigStatus.Text     = UIConstants.ServerStatus_StartingVerbose;
                  textServerStatus.Text = UIConstants.ServerStatus_Initializing;
                  imgStatus.Image = Resources.StatusError_48;

                  // clear fields            
                  textTraceDirectory.Text   = "";
                  comboLogLevel.Text = "";
                  textActivityLogLevel.Text = "";
                  
                  // service control buttons
                  btnStart.Enabled   = false;
                  btnStop.Enabled    = false;
               }
               else
               {
	               serviceDown = true;
	            }
	         }
	      }
	      
	      if ( serviceDown )
	      {
            lblBigStatus.Text     = UIConstants.ServerStatus_DownVerbose;
            textServerStatus.Text = UIConstants.ServerStatus_Down;
            imgStatus.Image = Resources.StatusError_48;

            // clear fields            
            textTraceDirectory.Text = "";
            comboLogLevel.Text = "";
            textActivityLogLevel.Text = "";
            
            // service control buttons
            btnStart.Enabled   = true;
            btnStop.Enabled    = false;
         }
      }
		
      //--------------------------------------------------------------------
      // btnCancel_Click
      //--------------------------------------------------------------------
      private void btnCancel_Click(object sender, EventArgs e)
      {
         this.DialogResult = DialogResult.Cancel;
         this.Close();
      }

      //--------------------------------------------------------------------
      // btnOK_Click
      //--------------------------------------------------------------------
      private void btnOK_Click(object sender, EventArgs e)
      {
         bool success;

         if (ValidateProperties())
         {
            //save the log level too.
            Globals.SQLcomplianceConfig.CollectionServerHeartbeatInterval = Convert.ToInt32(this.heartbeatIntervalText.Text);
            success = Globals.SQLcomplianceConfig.Write(Globals.Repository.Connection);

            if (success)
            {
               //store the log level in the registry.
               try
               {
                  // check for collection service - cant uninstall if it is down or unreachable
                   ServerManager srvManager = GUIRemoteObjectsProvider.ServerManager();
                  srvManager.SetCollectionServerLogLevel(comboLogLevel.SelectedIndex + 1);
                  success = true;
               }
               catch (Exception exception)
               {
                  ErrorLog.Instance.Write("Unable to update collection server logging level", exception, true);
                  MessageBox.Show(this, String.Format("Unable to update the Collection Server Logging Level.\r\nMessage: {0}", exception.ToString()), "Error");
                  success = false;
               }

               if (success)
               {
                  this.DialogResult = DialogResult.OK;
                  this.Close();
               }
            }
            else
            {
               ErrorLog.Instance.Write(String.Format("Unable to update the Collection server heartbeat interval. Error: {0}", SQLcomplianceConfiguration.GetLastError()));
               MessageBox.Show(this, String.Format("Unable to update the Collection server heartbeat interval. \r\n Error: {0}", SQLcomplianceConfiguration.GetLastError()));
            }
         }
      }

      private bool ValidateProperties()
      {
         try
         {
            ValidateRange(heartbeatIntervalText.Text, 2, 9999, heartbeatIntervalText, UIConstants.Error_BadHeartbeatFrequency);
         }
         catch
         {
            return false;
         }
         return true;
      }

      private void ValidateRange(string strVal, int minValue, 
                                 int maxValue, Control cntrl,
                                 string errMsg)
      {
         int val = UIUtils.TextToInt(strVal);

         if (val < minValue || val > maxValue)
         {
            ErrorMessage.Show(this.Text, errMsg);
            cntrl.Focus();
            throw new Exception("out of range");
         }
      }

      //--------------------------------------------------------------------
      // btnRefreshStatus_Click
      //--------------------------------------------------------------------
      private void btnRefreshStatus_Click(object sender, EventArgs e)
      {
         RefreshValues();
      }
      
      #region Help
      //--------------------------------------------------------------------
      // Form_Defaults_HelpRequested - Show Context Sensitive Help
      //--------------------------------------------------------------------
      private void Form_Defaults_HelpRequested(object sender, HelpEventArgs hlpevent)
      {
		   HelpAlias.ShowHelp(this,HelpAlias.SSHELP_Form_ServerOptions);
			hlpevent.Handled = true;
      }
      #endregion

      #region Service Control

      //--------------------------------------------------------------------
      // btnStart_Click - try to start collection server service
      //--------------------------------------------------------------------
      private void btnStart_Click(object sender, EventArgs e)
      {
         Cursor = Cursors.WaitCursor;
         if ( InternalStartService() )
         {
            textServerStatus.Text   = UIConstants.ServerStatus_Initializing;
            imgStatus.Image = Resources.StatusGood_48;
            lblBigStatus.Text       = UIConstants.ServerStatus_StartingVerbose;
            
            // clear fields            
            textTraceDirectory.Text   = "";
            comboLogLevel.Text = "";
            textActivityLogLevel.Text = "";
            
            // service control buttons
            btnStart.Enabled   = false;
            btnStop.Enabled    = false;
         }
         Cursor = Cursors.Default;
      }

      //--------------------------------------------------------------------
      // btnStop_Click - try to stop collection server service
      //--------------------------------------------------------------------
      private void btnStop_Click(object sender, EventArgs  e)
      {
         Cursor = Cursors.WaitCursor;

         if ( InternalStopService() )
         {
            lblBigStatus.Text     = UIConstants.ServerStatus_DownVerbose;
            textServerStatus.Text = UIConstants.ServerStatus_Down;
            imgStatus.Image       = Resources.StatusError_48;
            
            // clear fields            
            textTraceDirectory.Text = "";
            comboLogLevel.Text = "";
            textActivityLogLevel.Text = "";
            
            // service control buttons
            btnStart.Enabled = true;
            btnStop.Enabled  = false;
         }
         Cursor = Cursors.Default;
      }

      #endregion
      
      #region Internal Service Control routines
         
      //-------------------------------------------------------------
      // InternalStartService
      //--------------------------------------------------------------
      private bool InternalStartService()
      {
         Form_ServiceProgress frm = new Form_ServiceProgress( Globals.RepositoryServer, false );
         frm.ShowDialog();
         bool started = frm.Success;
         
         if ( started )
         {
			   MessageBox.Show( this,
			                    String.Format( UIConstants.Info_ServerStarted, Globals.RepositoryServer ),
				                  UIConstants.Title_StartServer);
         }
         else
         {
			   MessageBox.Show( this,
			                    String.Format( UIConstants.Error_CantStartServer,
			                                   Globals.RepositoryServer,
			                                   frm.ErrorMessage ),
				                 UIConstants.Title_StartServer,
				                 MessageBoxButtons.OK,
				                 MessageBoxIcon.Error );
         }
         return started;
      }
      
      //-------------------------------------------------------------
      // InternalStopService
      //--------------------------------------------------------------
      private bool
         InternalStopService()
      {
         bool stopped = false;
         
         // warn before stopping         
         DialogResult choice = MessageBox.Show( this,
                                                UIConstants.Warning_StopServer,
                                                UIConstants.Title_StopServer,
                                                MessageBoxButtons.YesNo,
                                                MessageBoxIcon.Question );
         if ( choice == DialogResult.No )
         {
            return false;
         }
         
         try
         {
			   NonWmiServiceManager serviceManager = new NonWmiServiceManager( Globals.RepositoryServer, CoreConstants.CollectionServiceName );
				stopped = serviceManager.Stop();
				
            if ( ! stopped )
            {
				   MessageBox.Show( this,
				                    String.Format( UIConstants.Error_CantStopServer,
                                               Globals.RepositoryServer,
                                               UIConstants.Error_ServerNotInstalled ),
				                    UIConstants.Title_StopServer);
            }                                                                  
            else
            {
			      MessageBox.Show( this,
			                       String.Format( UIConstants.Info_ServerStopped,
			                                      Globals.RepositoryServer ),
				                    UIConstants.Title_StopServer);
            }
         }
         catch( Exception ex )
         {
			   MessageBox.Show( this,
			                    String.Format( UIConstants.Error_CantStopServer,
			                                   Globals.RepositoryServer,
			                                   ex.Message ),
				                 UIConstants.Title_StopServer,
				                 MessageBoxButtons.OK,
				                 MessageBoxIcon.Error );
         }

         return stopped;
      }
      
      #endregion


	}
}
