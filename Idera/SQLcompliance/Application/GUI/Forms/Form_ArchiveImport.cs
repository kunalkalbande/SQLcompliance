using System ;
using System.Collections ;
using System.Text ;
using System.Windows.Forms ;
using Idera.SQLcompliance.Application.GUI.Helper ;
using Idera.SQLcompliance.Application.GUI.Properties ;
using Idera.SQLcompliance.Application.GUI.SQL ;
using Idera.SQLcompliance.Core ;
using Idera.SQLcompliance.Core.Agent ;
using Idera.SQLcompliance.Core.Collector;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
	/// <summary>
	/// Summary description for Form_ArchiveImport.
	/// </summary>
	public partial class Form_ArchiveImport : Form
	{
      private bool                         m_loaded = false;
      public  ArchiveRecord                m_archive = null;
      public  string                       archiveInstance = null;
      public  ServerRecord                 serverRecord = null;
      public bool serverAdded = false ;
		

		public Form_ArchiveImport()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
         this.Icon = Resources.SQLcompliance_product_ico;

			Cursor = Cursors.WaitCursor;
			
			LoadDatabases();

			Cursor = Cursors.Default;
		}


      private void btnCancel_Click(object sender, EventArgs e)
      {
         this.DialogResult = DialogResult.Cancel;
         this.Close();
      }

      private void btnOK_Click(object sender, EventArgs e)
      {
			Cursor = Cursors.WaitCursor;
			
         // Import
         try
         {
            // check for upgrade case
            if(!EventDatabase.IsCompatibleSchema(m_archive.EventDbSchemaVersion))
            {
               DialogResult choice = MessageBox.Show(this, UIConstants.Info_ArchiveUpgradeNeeded,
                  this.Text, MessageBoxButtons.YesNo);
               if ( choice == DialogResult.No)
               {
                  Cursor = Cursors.Default;
                  return;
               }
               else
               {
                  EventDatabase.UpgradeEventDatabase(Globals.Repository.Connection, comboDatabase.Text);
                  // Refresh the version now and see if we need to offer index upgrades
                  m_archive = ArchiveRecord.ReadArchiveMetaRecord(Globals.Repository.Connection,comboDatabase.Text );
               }
            }
            // check for index updates - at this point we should be compatible
            if( m_archive.EventDbSchemaVersion != CoreConstants.RepositoryEventsDbSchemaVersion )
            {
               Form_CreateIndex frm = new Form_CreateIndex(true) ;
               if (frm.ShowDialog(this) == DialogResult.OK)
               {
                  EventDatabase.UpdateIndexes(Globals.Repository.Connection, comboDatabase.Text);
                  // Refresh the version now and see if we need to offer index upgrades
                  m_archive = ArchiveRecord.ReadArchiveMetaRecord(Globals.Repository.Connection, comboDatabase.Text);
               }
               else
               {
                  SQLcomplianceConfiguration config = new SQLcomplianceConfiguration();
                  config.Read(Globals.Repository.Connection);

                  if (config.IndexStartTime == DateTime.MinValue)
                  {
                     Form_IndexSchedule schedule = new Form_IndexSchedule();
                     schedule.ShowDialog(this);
                     config.IndexStartTime = schedule.IndexStartTime;
                     config.IndexDuration = schedule.IndexDuration;
                     config.Write(Globals.Repository.Connection);
                  }
                  SetReindexFlag(true);
               }
            }
            
            // add archive record
            m_archive.WriteSystemDatabaseRecord( Globals.Repository.Connection );
            
            // add to servers table
            archiveInstance = m_archive.Instance;
            serverRecord    = ServerRecord.GetServer( Globals.Repository.Connection,
                                                      archiveInstance );
            if ( serverRecord == null )
            {
               serverRecord                 = new ServerRecord();
               serverRecord.Connection      = Globals.Repository.Connection;
               serverRecord.Instance        = archiveInstance;
               serverRecord.Description     = UIConstants.Info_ArchiveServerDescription;
               serverRecord.IsAuditedServer = false;
               
               serverRecord.Bias            = m_archive.Bias;
               serverRecord.StandardBias    = m_archive.StandardBias;
               serverRecord.StandardDate    = m_archive.StandardDate;
               serverRecord.DaylightBias    = m_archive.DaylightBias;
               serverRecord.DaylightDate    = m_archive.DaylightDate;
               
               serverRecord.Create(null);
               serverAdded = true ;
            }
            
            // Build snapshot
            StringBuilder snap = new StringBuilder (1024);
            
            snap.AppendFormat( "Attach archive database: {0}\r\n\r\n", comboDatabase.Text );
            snap.AppendFormat( "Display name: {0}\r\n", textDisplayName.Text );
            snap.AppendFormat( "Description: {0}\r\n",  textDescription.Text );
            
            string snapshot = snap.ToString();
            
            LogRecord.WriteLog( Globals.Repository.Connection,
                                LogType.AttachArchive,
                                textServer.Text,
                                snapshot );
           
			   Cursor = Cursors.Default;
            
            // Close
            this.DialogResult = DialogResult.OK;
            this.Close();
         }
         catch ( Exception ex )
         {
            ErrorMessage.Show( UIConstants.Error_CantImportArchive,
                               ex.Message );
         }
      }

      private void SetReindexFlag(bool reindex)
      {
         // check for collection service - cant uninstall if it is down or unreachable
         try
         {
             ServerManager srvManager = GUIRemoteObjectsProvider.ServerManager();
            srvManager.SetReindexFlag(reindex);
         }
         catch (Exception)
         {
            // TODO:  Should we alert the user when we can't talk to the collection server?
         }
      }

      #region Help
      //--------------------------------------------------------------------
      // Form_Defaults_HelpRequested - Show Context Sensitive Help
      //--------------------------------------------------------------------
      private void Form_Defaults_HelpRequested(object sender, HelpEventArgs hlpevent)
      {
		   HelpAlias.ShowHelp(this,HelpAlias.SSHELP_Form_ArchiveImport);
			hlpevent.Handled = true;
      }
      #endregion
      
      //-------------------------------------------------------------
      // comboDatabase_SelectedIndexChanged
      //--------------------------------------------------------------
      private void comboDatabase_SelectedIndexChanged(object sender, EventArgs e)
      {
         Cursor = Cursors.WaitCursor;
         
         // validate database
         if ( comboDatabase.Text != "" )
         {
            m_archive = ArchiveRecord.ReadArchiveMetaRecord(
                           Globals.Repository.Connection,
                           comboDatabase.Text );
                  
            string errMsg = "";
            if ( m_archive != null )
            {
               if ( ! EventDatabase.IsUpgradeableSchema( comboDatabase.Text, Globals.Repository.Connection))
               {
                  errMsg = UIConstants.Error_IncompatibleArchiveDatabase;
               }
            }
            else
            {
               errMsg = UIConstants.Error_InvalidArchiveDatabase;
            }

            if ( errMsg == "" )
            {            
               //---------------
               // Good Database
               //---------------
               btnOK.Enabled = true;
               
               // display values - enable OK
               textServer.Text      = m_archive.Instance;
               textDisplayName.Text = m_archive.DisplayName;
               textDescription.Text = m_archive.Description;
               textStartDate.Text   = UIUtils.GetLocalTimeDateString(m_archive.StartDate);
               textEndDate.Text     = UIUtils.GetLocalTimeDateString(m_archive.EndDate);
               
               if ( textStartDate.Text == UIConstants.Status_Never ) textStartDate.Text= "";
               if ( textEndDate.Text   == UIConstants.Status_Never ) textEndDate.Text= "";
            }
            else
            {
               //---------------
               // Bad Database
               //---------------
               btnOK.Enabled = false;
               
               // display error
               textDisplayName.Text = UIConstants.Title_InvalidArchiveDatabase;
               textServer.Text      = "";
               textDescription.Text = errMsg;
               textStartDate.Text   = "";
               textEndDate.Text     = "";
            }
         }
         else
         {
            btnOK.Enabled = false;
            
            // display error
            textDisplayName.Text = "";
            textServer.Text      = "";
            textDescription.Text = "";
            textStartDate.Text   = "";
            textEndDate.Text     = "";
         }
         
         Cursor = Cursors.Default;
      }
      
      //-------------------------------------------------------------
      // LoadDatabases
      //--------------------------------------------------------------
      private void LoadDatabases()
      {
         Cursor = Cursors.WaitCursor;
         
         comboDatabase.Items.Clear();
         

         SQLDirect sqlServer = new SQLDirect();
         
         sqlServer.Connection = Globals.Repository.Connection;
         
         ICollection dbList ;
         dbList = RawSQL.GetUserDatabases(sqlServer.Connection);

			if ((dbList != null) && (dbList.Count != 0)) 
			{
				foreach (RawDatabaseObject db in dbList) 
				{
				   if ( ! SQLRepository.IsSQLsecureOwnedDB( db.name ) )
				   {
				      // show all or match prefix
                  bool matchesArchivePrefix = false;
				      if (  ! checkShowAllDatabases.Checked )
				      {
   				      string dbu = db.name.ToUpper();
   				      string pu  = Globals.SQLcomplianceConfig.ArchivePrefix.ToUpper();
   				      
   				      if ( dbu.StartsWith(pu) )
   				      {
   				         matchesArchivePrefix = true;
                     }
				      }
				      
				      if ( checkShowAllDatabases.Checked || matchesArchivePrefix )
				      {
		               comboDatabase.Items.Add(db.name);
		            }
		         }
            }
         }
         Cursor = Cursors.Default;
         
         m_loaded = true;
      }

      //-----------------------------------------------------------------------
      // checkShowAllDatabases_CheckedChanged
      //-----------------------------------------------------------------------
      private void checkShowAllDatabases_CheckedChanged(object sender, EventArgs e)
      {
         m_loaded = false;
      }

      //-----------------------------------------------------------------------
      // comboDatabase_DropDown
      //-----------------------------------------------------------------------
      private void comboDatabase_DropDown(object sender, EventArgs e)
      {
         if ( ! m_loaded ) LoadDatabases();
      }
	}
}
