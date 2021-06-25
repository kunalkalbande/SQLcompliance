using System ;
using System.Collections ;
using System.Windows.Forms ;
using Idera.SQLcompliance.Application.GUI.Helper ;
using Idera.SQLcompliance.Application.GUI.Properties ;
using Idera.SQLcompliance.Core ;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
	/// <summary>
	/// Summary description for Form_RepositoryOptions.
	/// </summary>
	public partial class Form_RepositoryOptions : Form
	{
		int oldRecoveryModel;

      public Form_RepositoryOptions() :this(0) {}

      public Form_RepositoryOptions(int tabToShow)
      {
         //
         // Required for Windows Form Designer support
         //
         InitializeComponent();
         this.Icon = Resources.SQLcompliance_product_ico;
         switch (tabToShow)
         {
            case 0:
               _tabControl.SelectedTab = _tabRecoveryModel ;
               break ;
            case 1:
               _tabControl.SelectedTab = _tabIndexes ;
               break; 
         }
      }
      
      protected override void OnLoad(EventArgs e)
      {
			Cursor = Cursors.WaitCursor;

			// Reload recover model
			try
			{
			   oldRecoveryModel = SQLcomplianceConfiguration.GetRecoveryModel();
			   
			   Globals.SQLcomplianceConfig.RecoveryModel = oldRecoveryModel;
			   
			}
			catch ( Exception ex )
			{
			   ErrorMessage.Show( this.Text,
			                      "Error reading Repository configuration." ,
			                      ex.Message );
            throw ex;			                      
			}
			finally
			{
   			Cursor = Cursors.Default;
			}
			
         // Archive Preferences
         if ( Globals.SQLcomplianceConfig.RecoveryModel == 0 )
         {
            radioSimple.Checked = true;
         }
         else
         {
            radioModel.Checked = true;
         }

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

         LoadDatabases() ;
         
         base.OnLoad (e);
		}


      private void btnCancel_Click(object sender, EventArgs e)
      {
         this.DialogResult = DialogResult.Cancel;
         this.Close();
      }

      private void btnOK_Click(object sender, EventArgs e)
      {
         int newRecoveryModel;
         
         if ( radioSimple.Checked )
            newRecoveryModel = 0;
         else
            newRecoveryModel = 1;
            
         if ( oldRecoveryModel != newRecoveryModel )
         {
            SQLcomplianceConfiguration.SetRecoveryModel( newRecoveryModel );
            
            string snapshot = String.Format( "Recovery model for new databases created in Repository changed.\r\n\tOld: '{0}'\r\n\tNew: '{1}'\r\n\r\nCollection Server: {2}",
                                             (oldRecoveryModel==0) ? "Use simple recovery model" : "Use the recovery model set for the model database",
                                             (newRecoveryModel==0) ? "Use simple recovery model" : "Use the recovery model set for the model database",
                                             Globals.RepositoryServer );

            LogRecord.WriteLog( Globals.Repository.Connection,
                                LogType.ConfigureRepository,
                                snapshot );
         }            
         
         this.DialogResult = DialogResult.OK;
         this.Close();
      }

      #region Help
      //--------------------------------------------------------------------
      // Form_Defaults_HelpRequested - Show Context Sensitive Help
      //--------------------------------------------------------------------
      private void Form_Defaults_HelpRequested(object sender, HelpEventArgs hlpevent)
      {
         if(_tabControl.SelectedTab == _tabRecoveryModel)
            HelpAlias.ShowHelp(this,HelpAlias.SSHELP_Form_RepositoryOptions_RecoveryModel);
         else
            HelpAlias.ShowHelp(this,HelpAlias.SSHELP_Form_RepositoryOptions_Databases);
			hlpevent.Handled = true;
      }
      #endregion

      private void LoadDatabases()
      {
         _btnUpdateIndexes.Enabled = false ;
         _listDatabases.Items.Clear() ;
         try
         {
            SystemDatabaseRecord[] records ;

            records = SystemDatabaseRecord.Read(Globals.Repository.Connection) ;

            foreach(SystemDatabaseRecord record in records)
            {
               ListViewItem item = new ListViewItem() ;
               string status ;
               if(!String.Equals(record.DatabaseType, "System"))
               {
                  // Event Database
                  try
                  {
                     if(EventDatabase.GetDatabaseSchemaVersion(Globals.Repository.Connection, record.DatabaseName)
                        == CoreConstants.RepositoryEventsDbSchemaVersion)
                     {
                        status = "OK" ;
                        item.ImageIndex = 0 ;
                     }
                     else
                     {
                        status = "Needs Index Update" ;
                        item.ImageIndex = 1 ; 
                     }
                  }
                  catch(Exception)
                  {
                     status = "Unavailable" ;
                     item.ImageIndex = 2 ;
                  }
               }
               else
               {
                  int dbSchemaVersion, eventSchemaVersion ;

                  // System Database
                  if(String.Equals(record.DatabaseName, CoreConstants.RepositoryTempDatabase))
                     continue ;
                  SQLcomplianceConfiguration.ReadSchemaVersions(Globals.Repository.Connection,
                     out dbSchemaVersion, out eventSchemaVersion) ;
                  if(dbSchemaVersion != CoreConstants.RepositorySqlComplianceDbSchemaVersion)
                  {
                     status = "Needs Index Update" ;
                     item.ImageIndex = 1 ; 
                  }
                  else
                  {
                     status = "OK" ;
                     item.ImageIndex = 0 ;
                  }
               }
               item.Text = record.DatabaseName ;
               item.Tag = record ;
               item.SubItems.Add(record.DatabaseType) ;
               item.SubItems.Add(status) ;
               _listDatabases.Items.Add(item) ;
            }
         }
         catch (Exception ex )
         {
            ErrorLog.Instance.Write(ex, true) ;
         }
      }

      private void SelectedIndexChanged_listDatabases(object sender, EventArgs e)
      {
         bool enabled = false ;
         foreach(ListViewItem item in _listDatabases.SelectedItems)
         {
            if(item.ImageIndex == 1)
               enabled = true ;
         }
         _btnUpdateIndexes.Enabled = enabled ;
      }

      private void Click_btnUpdateIndexes(object sender, EventArgs e)
      {
         Cursor.Current = Cursors.WaitCursor ;
         try
         {
            Form_CreateIndex frmAsk = new Form_CreateIndex(false) ;
            if(frmAsk.ShowDialog() == DialogResult.OK)
            {
               ArrayList list = new ArrayList() ;
               foreach(ListViewItem item in _listDatabases.SelectedItems)
                  if(item.ImageIndex == 1)
                     list.Add(item.Tag) ;

               SystemDatabaseRecord[] records = new SystemDatabaseRecord[list.Count];
               for(int i = 0 ; i < list.Count ; i++)
                  records[i] = (SystemDatabaseRecord)list[i] ;

               Form_IndexProgress frm = new Form_IndexProgress(records) ;
               frm.ShowDialog(this) ;
               if(!frm.Success)
                  ErrorMessage.Show("Index update failed",frm.ErrMsg);

            }
         }
         catch(Exception ex)
         {
            ErrorMessage.Show("Index update failed", ex.Message) ;
            ErrorLog.Instance.Write(ex, true) ;
         }
         finally
         {
            Cursor.Current = Cursors.Default ;
            LoadDatabases() ;
         }
      }

      private void Click_btnUpdateSchedule(object sender, EventArgs e)
      {
         SQLcomplianceConfiguration config = new SQLcomplianceConfiguration();
         config.Read(Globals.Repository.Connection);

         Form_IndexSchedule schedule = new Form_IndexSchedule();
         schedule.IndexStartTime = config.IndexStartTime;
         schedule.IndexDuration = config.IndexDuration;
         schedule.ShowDialog(this);
         config.IndexStartTime = schedule.IndexStartTime;
         config.IndexDuration = schedule.IndexDuration;
         config.Write(Globals.Repository.Connection);
      }
	}
}
