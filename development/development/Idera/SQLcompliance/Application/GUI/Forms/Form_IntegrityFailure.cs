using System ;
using System.Data.SqlClient ;
using System.Windows.Forms ;
using Idera.SQLcompliance.Application.GUI.Helper ;
using Idera.SQLcompliance.Core ;
using Idera.SQLcompliance.Core.Collector ;
using Idera.SQLcompliance.Core.Event ;
using Infragistics.Win.UltraWinDataSource;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
	/// <summary>
	/// Summary description for Form_IntegrityFailure.
	/// </summary>
	public partial class Form_IntegrityFailure : Form
	{
	
		public
		   Form_IntegrityFailure(
		      CheckResult    result,
		      EventRecord[]  badRecords,
		      int[]          badRecordTypes,
		      string         operation,
		      string         caption
		   )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
         GridHelper.ApplyDefaultSettings(_gridEvents);

         this.Icon = Properties.Resources.SQLcompliance_product_ico;

			lblEventGaps.Text      = result.numGaps.ToString();
			lblAddedEvents.Text    = result.numAdded.ToString();
			lblModifiedEvents.Text = result.numModified.ToString();
         lblModifiedColumnChanges.Text = result.numCCModified.ToString();
         lblModifiedDataChanges.Text = result.numDCModified.ToString();
         lblModifiedSensitiveColumns.Text = result.numSCModified.ToString();
         lblDataChangeGaps.Text = result.numCCGaps.ToString();
         lblColumnChangeGaps.Text = result.numDCGaps.ToString();
         lblSensitiveColumnGaps.Text = result.numSCGaps.ToString();
			
			this.Text = this.Text + caption;
			
			// display grid
			for( int i=0; i<badRecords.Length; i++ )
			{
			   LoadRow( badRecordTypes[i], badRecords[i] );
			}
			
			if ( operation != "" )
			{
            groupDeletions.Visible = false;
			   groupOperationCantContinue.Visible = true;
			   labelOperation.Text = operation + labelOperation.Text;
			}
			else
			{
            groupDeletions.Visible = true;
            groupOperationCantContinue.Visible = false;
			}
		}
		


      //--------------------------------------------------------
      // LoadRow
      //--------------------------------------------------------
		private void
		   LoadRow(
		      int         badEventType,
		      EventRecord ev
		   )
		{
         string eventCategoryString ;
         string eventTypeString     ;
         
         GetStrings( (int)ev.eventType,
                     (int)ev.eventCategory,
                     out eventCategoryString,
                     out eventTypeString );

         UltraDataRow row = _dsEvents.Rows.Add();

         row["Problem"] = GetBadEventType( badEventType ) ;
         row["Category"] =  eventCategoryString;
         row["Event"] =  eventTypeString;
         row["Time"] =  UIUtils.GetLocalTimeDateString(ev.startTime);
         row["Login"] =  ev.loginName;
         row["Database"] =  ev.databaseName;
         row["TargetObject"] =  ev.targetObject;
         row["Details"] =  ev.details;
		}

      string integrityCategory = "";
      string missingEvent       = "";
      string insertedEvent      = "";
      string modifiedEvent      = "";
      
      //--------------------------------------------------------
      // GetStrings
      //--------------------------------------------------------
      private void
         GetStrings(
            int   eventType,
            int   eventCategory,
            out string eventCategoryString,
            out string eventTypeString
         )
      {
         eventCategoryString = "";
         eventTypeString     = "";
         
         if ( eventType == (int)TraceEventType.MissingEvents )
            eventTypeString = missingEvent;
         else if ( eventType == (int)TraceEventType.InsertedEvent )
            eventTypeString = insertedEvent;
         else if ( eventType == (int)TraceEventType.ModifiedEvent )
            eventTypeString = modifiedEvent;
         
         if ( eventTypeString != "" )
         {
            eventCategoryString = integrityCategory;
            return;
         }
         
         try
         {
            string sql = String.Format( "SELECT name,category from {0}..{1} WHERE evtypeid={2}",
                                        CoreConstants.RepositoryDatabase,
                                        CoreConstants.RepositoryEventTypesTable,
                                        eventType );
            using ( SqlCommand cmd = new SqlCommand( sql, Globals.Repository.Connection ) )
            {
               using ( SqlDataReader reader = cmd.ExecuteReader() )
               {
                  if ( reader.Read() )
                  {
                     eventTypeString     = SQLHelpers.GetString( reader, 0 );
                     eventCategoryString = SQLHelpers.GetString( reader, 1 );
                  }
                  else
                  {
                     eventTypeString     = eventType.ToString();
                     eventCategoryString = eventCategory.ToString();
                  }
               }
            }
         }
         catch {}
      }
      
      //--------------------------------------------------------
      // GetBadEventType
      //--------------------------------------------------------
      private string
         GetBadEventType(
            int   badEventType
         )
      {
         if ( badEventType == CoreConstants.BadEventType_AddedEvent )
            return "Inserted";
         else if ( badEventType == CoreConstants.BadEventType_ModifiedEvent )
            return "Modified";
         else
            return "Deleted";
      }

      private void btnRepair_Click(object sender, EventArgs e)
      {
         DialogResult = DialogResult.Yes;
         Close();
      }

      private void btnDontRepair_Click(object sender, EventArgs e)
      {
         DialogResult = DialogResult.No;
         Close();
      }

      #region Help
      //--------------------------------------------------------------------
      // Form_Defaults_HelpRequested - Show Context Sensitive Help
      //--------------------------------------------------------------------
      private void Form_Defaults_HelpRequested(object sender, HelpEventArgs hlpevent)
      {
		   HelpAlias.ShowHelp(this,HelpAlias.SSHELP_Form_IntegrityCheckResults);
			hlpevent.Handled = true;
      }

      private void linkMoreHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
      {
		   HelpAlias.ShowHelp(this,HelpAlias.SSHELP_Form_IntegrityCheckResults);
      }
      #endregion

	}
}
