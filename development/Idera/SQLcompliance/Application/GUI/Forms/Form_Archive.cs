using System ;
using System.Collections ;
using System.Net.Sockets ;
using System.Security.Principal ;
using System.Windows.Forms ;
using Idera.SQLcompliance.Application.GUI.Helper ;
using Idera.SQLcompliance.Application.GUI.Properties ;
using Idera.SQLcompliance.Application.GUI.SQL ;
using Idera.SQLcompliance.Core ;
using Idera.SQLcompliance.Core.Agent ;
using Idera.SQLcompliance.Core.Collector ;
using Idera.SQLcompliance.Core.Scripting ;
using Idera.SQLcompliance.Core.TimeZoneHelper ;
using TimeZoneInfo = Idera.SQLcompliance.Core.TimeZoneHelper.TimeZoneInfo;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
	/// <summary>
	/// Summary description for Form_Archive.
	/// </summary>
	public partial class Form_Archive : Form
	{

      private  string m_instance = "";

		public
		   Form_Archive(
		      string instance
		   )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
         this.Icon = Resources.SQLcompliance_product_ico;
			
			Cursor = Cursors.WaitCursor;
			
			// load server combo
			m_instance = instance;
			LoadServerDropDown();
			
			// load archive options
			_lblCurrentRight.Text = String.Format( "Move events older than {0} days to an archive database.",
			                                   Globals.SQLcomplianceConfig.ArchiveAge );
         if(Globals.SQLcomplianceConfig.ArchiveCheckIntegrity)
            _lblIntegrityRight.Text = "No" ;
         else
            _lblIntegrityRight.Text = "Yes" ;

			Cursor = Cursors.Default;
		}

      //-----------------------------------------------------------------------
      // btnCancel_Click
      //-----------------------------------------------------------------------
      private void btnCancel_Click(object sender, EventArgs e)
      {
         this.DialogResult = DialogResult.Cancel;
         this.Close();
      }

      //-----------------------------------------------------------------------
      // btnOK_Click
      //-----------------------------------------------------------------------
      private void btnOK_Click(object sender, EventArgs e)
      {
         // Validate
         if ( radioServer.Checked && comboServer.Text == "" )
         {
            MessageBox.Show( UIConstants.Error_ServerRequired,
                             this.Text );
            return;
         }

          if (ArchiveScheduler.IsScheduledArchiveRunning)
          {
              MessageBox.Show(UIConstants.Warning_ScheduledArchiveRunning, Text);
              return;
          }

         
         // Request Archive
         Cursor = Cursors.WaitCursor;
         
         RequestArchive();
         
         Cursor = Cursors.Default;

         // Time to go      
         this.DialogResult = DialogResult.OK;
         this.Close();
      }
      
      private   string  errMsg1 = "";
      private   string  errMsg2 = "";
      
      
      //-----------------------------------------------------------------------
      // RequestArchive
      //
      // Cases
      //  (1) Single instance  - Background (Integrity, Launch job)
      //  (2) Single instance  - non Background  (Integrity, Launch job)
      //  (3) All instances    - Launch job
      //-----------------------------------------------------------------------
      private void
         RequestArchive()
      {
         bool    worked  = false;
         
         try
         {
            bool   background = true;
            string instance = "";
            
            if ( radioServer.Checked ) instance = comboServer.Text;

            if ( instance != "" )
            {
               //-----------------------------------------            
               // Check integrity in single instance case
               //-----------------------------------------            
               if (Globals.SQLcomplianceConfig.ArchiveCheckIntegrity)
               {
                  bool hasIntegrity ;
                  
                  bool succeeded = IntegrityCheck.CheckAndRepair( "Archive Audit Data",
                                                                  "Archiving",
                                                                  instance,
                                                                  out hasIntegrity );
                  if ( succeeded )                                                   
                  {
                     if ( ! hasIntegrity )
                     {
                        MessageBox.Show( String.Format( UIConstants.Error_ArchiveAbortedAfterIntegrityCheck,
                                                      comboServer.Text ),
                                    this.Text );
                        return;
                     }
                  }
                  else
                  {
                     // error displayed in CheckAndRepair
                     return;
                  }
               }

               //------------------------------------------------------------
               // Ask if they want to run this archive job in the background
               //------------------------------------------------------------
               if ( instance != "" )
               {
                  DialogResult choice = MessageBox.Show( "Archive operations can take a significant amount " +
                                                         "of time depending on the number of events to be moved " +
                                                         "to archive databases. If you run the operation as a background process, " +
                                                         "you can continue to use the Management Console during the archive operation but the " +
                                                         "archived events will not be available for viewing until the operation is complete and you " +
                                                         "refresh the Archives node for this SQL Server. " +
                                                         "\n\n" +
                                                         "Do you want to run the archive operation as a background process?",
                                                         this.Text,
                                                         MessageBoxButtons.YesNoCancel );
                  if ( choice == DialogResult.Cancel ) return;
                  if ( choice == DialogResult.Yes )
                     background = true;
                  else
                     background = false;
               }
               else
               {
                  // always run multiple instance case in background
                  // integrity check done during archive operation
                  background = true;
               }

            }
            
            //-----------------------------------------            
            // Run archive
            //-----------------------------------------            
            if ( background )
            {
               worked = RequestBackgroundArchive( instance );
            }
            else
            {
               Form_ArchiveProgress archiveProgressFrm
                  = new Form_ArchiveProgress( instance );
                  
               archiveProgressFrm.ShowDialog();

               worked = archiveProgressFrm.Success;
               if ( ! worked )
               {
                  errMsg1 = archiveProgressFrm.errMsg1;
                  errMsg2 = archiveProgressFrm.errMsg2;
               }
               else
               {
                  string msg = String.Format( "Audit data older than {0} days archived for SQL Server instance {1}.",
                                              Globals.SQLcomplianceConfig.ArchiveAge,
                                              instance );
                  
			         MessageBox.Show(msg, this.Text); 
               }
            }
         }
         catch ( SocketException socketEx )
         {
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
            errMsg1 = UIConstants.Error_ArchiveNowFailed;
            errMsg2 = ex.Message;
         }

         if ( ! worked )
         {
            ErrorMessage.Show( this.Text,
                               errMsg1,
                               errMsg2,
                               MessageBoxIcon.Error );
         }
      }
      
      //-----------------------------------------------------------------------
      // RequestBackgroundArchive
      //-----------------------------------------------------------------------
      private bool
         RequestBackgroundArchive(
            string   instance
         )
      {
         bool worked ;
         
         try
         {
            RemoteCollector srv = GUIRemoteObjectsProvider.RemoteCollector();
            // fake impersonation
            WindowsIdentity id = WindowsIdentity.GetCurrent();
            
            ArchiveSettings settings = new ArchiveSettings() ;
            settings.TargetInstance = instance ;
            settings.User = id.Name ;
            settings.Background = true ;

            // request archive by server
            //    for multiple instance - integrity check request depends on global flag
            //    for single instance - integrity check already run
            if(instance != "")
            {
               if(Globals.SQLcomplianceConfig.ArchiveCheckIntegrity)
                  settings.IntegrityCheckAction = IntegrityCheckAction.CheckAlreadyDone ;
               else
                  settings.IntegrityCheckAction = IntegrityCheckAction.SkipCheck ;
            }

            srv.Archive(settings) ;
            
            string msg ;
            if ( radioServer.Checked )
            {
               // Background is a GUI-specific operation, so we leave the ChangeLog entries in the GUI
               LogRecord.WriteLog( Globals.Repository.Connection,
                                   LogType.Archive,
                                   instance,
                                   String.Format( "Background archive started for SQL Server {0}.", instance ) );
            
               // single instance - archive op ran in synchronous mode
                                    
   	         // archive started as background process
               msg = String.Format( "An archive operation has been started at the Collection Server. This will " +
                                    "move all audit events older than {0} days to archive databases. The archive " +
                                    "database will be automatically attached to the Repository when it is completed.",
   	                              Globals.SQLcomplianceConfig.ArchiveAge );
               MessageBox.Show( msg, this.Text );
            }
            else
            {
               // Background is a GUI-specific operation, so we leave the ChangeLog entries in the GUI
               msg = "Background archive started for all registered SQL Servers.";
               
               if ( !Globals.SQLcomplianceConfig.ArchiveCheckIntegrity )
               {
                  msg += " User opted to skip the integrity check operation run prior to the archive oepration";
               }
               
               LogRecord.WriteLog( Globals.Repository.Connection,
                                   LogType.Archive,
                                   instance,
                                   msg );
                                   
               // multiple instances - archive op ran as background thread
   	         // archive started as background process
               msg = String.Format( "Archive operations have been started for all audited SQL Servers and are running at the Collection Server. This will " +
                                    "move all audit events older than {0} days to archive databases. New archive " +
                                    "databases will be automatically attached to the Repository.",
   	                              Globals.SQLcomplianceConfig.ArchiveAge );
               MessageBox.Show( msg, this.Text );
            }
            worked = true;
         }
         catch ( SocketException socketEx )
         {
            worked = false;
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
            worked = false;
            errMsg1 = UIConstants.Error_ArchiveNowFailed;
            errMsg2 = ex.Message;
         }
         return worked;
      }

      #region Help
      //--------------------------------------------------------------------
      // Form_Defaults_HelpRequested - Show Context Sensitive Help
      //--------------------------------------------------------------------
      private void Form_Defaults_HelpRequested(object sender, HelpEventArgs hlpevent)
      {
		   HelpAlias.ShowHelp(this,HelpAlias.SSHELP_Form_Archive);
			hlpevent.Handled = true;
      }
      #endregion

      //-----------------------------------------------------------------------
      // radioServer_CheckedChanged
      //-----------------------------------------------------------------------
      private void radioServer_CheckedChanged(object sender, EventArgs e)
      {
         comboServer.Enabled = radioServer.Checked;
      }
      
      //-------------------------------------------------------------
      // LoadServerDropDown
      //--------------------------------------------------------------
      private void LoadServerDropDown()
      {
         Cursor = Cursors.WaitCursor;
         comboServer.BeginUpdate();
      
         comboServer.Items.Clear();
         
         ICollection serverList ;
         serverList = ServerRecord.GetServers( Globals.Repository.Connection, false );

			if ((serverList != null) && (serverList.Count != 0)) 
			{
				foreach (ServerRecord config in serverList) 
				{
				   comboServer.Items.Add( config.Instance );
            }
         
            // select first server
            if ( comboServer.Items.Count != 0 )
            {
               comboServer.Text = comboServer.Items[0].ToString();
               if (String.IsNullOrEmpty(m_instance))
                  radioAll.Checked = true ;
               else
               {
                  if ( comboServer.FindString(m_instance) != -1 )
                  {
                     comboServer.Text = m_instance;
                     radioServer.Checked = true;
                  }
                  if (comboServer.Text == "")
                  {
                     comboServer.Text = comboServer.Items[0].ToString();
                     radioServer.Checked = true;
                  }
               }
            }
         }
         else
         {
            ErrorMessage.Show( this.Text,
                               UIConstants.Error_CantLoadServers,
                               Globals.Repository.GetLastError());
         }

         comboServer.EndUpdate();
         Cursor = Cursors.Default;
      }

      //-------------------------------------------------------------
      // btnShowAutoArchivePreferences_Click
      //--------------------------------------------------------------
      private void btnShowAutoArchivePreferences_Click(object sender, EventArgs e)
      {
         Form_ArchiveOptions frm = new Form_ArchiveOptions();
         frm.ShowDialog();
         
         _lblCurrentRight.Text = String.Format( "Move events older than {0} days to an archive database.",
            Globals.SQLcomplianceConfig.ArchiveAge );
         if(Globals.SQLcomplianceConfig.ArchiveCheckIntegrity)
            _lblIntegrityRight.Text = "No" ;
         else
            _lblIntegrityRight.Text = "Yes" ;
      }

      private void Click_btnScript(object sender, EventArgs e)
      {
         Form_Script frm = new Form_Script() ;
         ArchiveSettings settings = new ArchiveSettings() ;

         // Set to "" for the -all option
         if(radioAll.Checked)
            settings.TargetInstance = "" ;
         else
            settings.TargetInstance = comboServer.Text ;
         settings.ArchiveDays = Globals.SQLcomplianceConfig.ArchiveAge ;
         if(Globals.SQLcomplianceConfig.ArchiveCheckIntegrity)
            settings.IntegrityCheckAction = IntegrityCheckAction.PerformCheck ;
         else
            settings.IntegrityCheckAction = IntegrityCheckAction.SkipCheck ;
         settings.Prefix = Globals.SQLcomplianceConfig.ArchivePrefix ;
         settings.ArchivePeriod = Globals.SQLcomplianceConfig.ArchivePeriod ;
         foreach(TimeZoneInfo info in TimeZoneInfo.GetSystemTimeZones())
         {
            if(String.Equals(info.ToString(), Globals.SQLcomplianceConfig.ArchiveTimeZoneName))
            {
               settings.TimeZoneName = info.TimeZoneStruct.StandardName ;
               break ;
            }
         }
         frm.Script = ScriptGenerator.GenerateArchive(ScriptGenerator.GenerateGlobals(Globals.SQLcomplianceConfig), settings) ;
         frm.ShowDialog(this) ;
      }
	}
}
