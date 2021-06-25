using System ;
using System.Text ;
using System.Windows.Forms ;
using Idera.SQLcompliance.Application.GUI.Helper ;
using Idera.SQLcompliance.Application.GUI.Properties ;
using Idera.SQLcompliance.Core ;
using Idera.SQLcompliance.Core.Status ;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
	/// <summary>
	/// Summary description for Form_ArchiveProperties.
	/// </summary>
	public partial class Form_ArchiveProperties : Form
	{
      #region Window Properties	

		
		#endregion
		
		#region Properties

      
      private string          databaseName;

      
      public  ArchiveRecord   archiveRecord;
		
		#endregion

      #region Constructor / Dispose

		public
		   Form_ArchiveProperties(
		      ArchiveRecord         inArchive
         )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
         this.Icon = Resources.SQLcompliance_product_ico;

			databaseName = inArchive.DatabaseName;
			
         // reorder the tabs since it's a CF bug
         // by setting the index to "1" it sets it to the top
         // and the existing tabs get "pushed" down
         // this means the tab order on-screen will be in the reverse
         // order that we set here.              |
         tabControl1.Controls.SetChildIndex(this.tabPagePermissions, 1);
         tabControl1.Controls.SetChildIndex(this.tabPageGeneral, 1);
      }
      
      protected override void OnLoad(EventArgs e)
      {
			ArchiveRecord arc = ArchiveRecord.ReadArchiveMetaRecord( Globals.Repository.Connection,
			                                                         databaseName );
         if ( arc == null )
         {
            MessageBox.Show( String.Format( UIConstants.Error_ArchiveDeleted, databaseName), this.Text );
            DialogResult = DialogResult.Cancel;
            Close();
         }
         else
         {
            archiveRecord = arc;
            
			   textServer.Text      = archiveRecord.Instance;
			   textDisplayName.Text = archiveRecord.DisplayName;
			   textDescription.Text = archiveRecord.Description;
   			
            textStartDate.Text   = UIUtils.GetLocalTimeDateString(archiveRecord.StartDate);
            textEndDate.Text     = UIUtils.GetLocalTimeDateString(archiveRecord.EndDate);
            if ( textStartDate.Text == UIConstants.Status_Never ) textStartDate.Text= "";
            if ( textEndDate.Text   == UIConstants.Status_Never ) textEndDate.Text= "";
            
		      if ( archiveRecord.DefaultAccess == 2 )
		         radioGrantAll.Checked  = true;
		      else if ( archiveRecord.DefaultAccess == 1 )
		         radioGrantEventsOnly.Checked = true;
		      else
		         radioDeny.Checked = true;
            
			   textDatabase.Text    = archiveRecord.DatabaseName;
            if ( arc.ContainsBadEvents == 1 )   
               textIntegrityStatus.Text = UIConstants.Info_DatabaseHasErrors;
            else
               textIntegrityStatus.Text = UIConstants.Info_DatabaseIsClean;
               
            if ( archiveRecord.TimeLastIntegrityCheck ==  DateTime.MinValue )
            {
               textLastIntegrityCheck.Text        = UIConstants.Status_Never;
               textLastIntegrityCheckResults.Text = "";
            }
            else
            {
               textLastIntegrityCheck.Text        = UIUtils.GetLocalTimeDateString(archiveRecord.TimeLastIntegrityCheck);
               textLastIntegrityCheckResults.Text = UIUtils.GetIntegrityCheckResult(archiveRecord.LastIntegrityCheckResult);
            }
                        
         }
         
         //------------------------------------------------------
         // Make controls read only unless user has admin access
         //------------------------------------------------------
         if ( ! Globals.isAdmin)
         {
            // other tabs
            for ( int i=0; i<tabControl1.TabPages.Count; i++ )            
            {
               foreach ( Control ctrl in tabControl1.TabPages[i].Controls )
               {
                  ctrl.Enabled = false;
               }
            }
            
            // change buttons
            btnOK.Visible = false;
            btnCancel.Text = "Close";
            btnCancel.Enabled = true;
            this.AcceptButton = btnCancel;
         }
         
         
         base.OnLoad (e);
		}
		
		#endregion

      
      #region OK/Apply/Cancel

      //--------------------------------------------------------------------
      // btnCancel_Click - Close without saving
      //--------------------------------------------------------------------
      private void btnCancel_Click(object sender, EventArgs e)
      {
         this.DialogResult = DialogResult.Cancel;
         this.Close();
      }
      
      //--------------------------------------------------------------------
      // btnOK_Click - Save and close
      //--------------------------------------------------------------------
      private void btnOK_Click(object sender, EventArgs e)
      {
         try
         {
            if ( Changed() )
            {
               int newDefaultAccess;
		         if ( radioGrantAll.Checked )
		            newDefaultAccess = 2;
		         else if ( radioGrantEventsOnly.Checked )
		            newDefaultAccess = 1;
		         else
		            newDefaultAccess = 0;
            
               ArchiveRecord.UpdateArchiveProperties(
                  Globals.Repository.Connection,
                  textServer.Text,
                  databaseName,
                  textDisplayName.Text,
                  textDescription.Text,
                  newDefaultAccess,
                  archiveRecord.DefaultAccess );
                  
               archiveRecord.DisplayName   = textDisplayName.Text;               
               archiveRecord.Description   = textDescription.Text;
               archiveRecord.DefaultAccess = newDefaultAccess;   
               
               LogRecord.WriteLog( Globals.Repository.Connection,
                                   LogType.ArchiveChanged,
                                   snapshot );
            }
               
            this.DialogResult = DialogResult.OK;
            this.Close();
         }
         catch ( Exception ex )
         {
            ErrorMessage.Show( UIConstants.Error_CantUpdateArchive,
                               ex.Message );
         }
      }
      
      private string snapshot;
      
      private bool Changed()
      {
         StringBuilder snap = new StringBuilder(1024);
         bool dirty = false;
         snapshot = "";
      
		   if ( textDisplayName.Text != archiveRecord.DisplayName )
		   {
		      dirty = true;
		      snap.AppendFormat ( "Display Name Changed:\r\n\tOld: {0}\r\n\tNew: {1}\r\n\r\n",
		                          archiveRecord.DisplayName, 
		                          textDisplayName.Text );
         }		                          
         
		   if ( textDescription.Text != archiveRecord.Description )
		   {
		      dirty = true;
		      snap.AppendFormat ( "Description Changed:\r\n\tOld: {0}\r\n\tNew: {1}\r\n\r\n",
		                          archiveRecord.Description, 
		                          textDescription.Text );
         }		                          
         
         int newDefaultAccess;
		   if ( radioGrantAll.Checked )
		      newDefaultAccess = 2;
		   else if ( radioGrantEventsOnly.Checked )
		      newDefaultAccess = 1;
		   else
		      newDefaultAccess = 0;
		      
		   if ( newDefaultAccess != archiveRecord.DefaultAccess )
		   {
		      dirty = true;
		      
		      snap.AppendFormat ( "Default Permissions Changed:\r\n\tOld: {0}\r\n\tNew: {1}\r\n\r\n",
		                          Snapshot.GetDefaultAccess( archiveRecord.DefaultAccess ), 
		                          Snapshot.GetDefaultAccess( newDefaultAccess ) );
         }		                          
		         
         if ( dirty )
         {
            snapshot = String.Format( "{0}\r\n\r\nArchive Database Properties Modified\r\n\r\n{1}",
                                      archiveRecord.DatabaseName,
                                      snap.ToString() );
         }
         
         return dirty;		         
      }
      
      
      
      #endregion      

      #region Help
      //--------------------------------------------------------------------
      // Form_ArchiveProperties_HelpRequested - Show Context Sensitive Help
      //--------------------------------------------------------------------
      private void Form_ArchiveProperties_HelpRequested(object sender, HelpEventArgs hlpevent)
      {
         string helpTopic;

         if ( tabControl1.SelectedTab == tabPagePermissions )
            helpTopic = HelpAlias.SSHELP_Form_ArchiveProperties_Permissions;
         else
            helpTopic = HelpAlias.SSHELP_Form_ArchiveProperties_General;

		   HelpAlias.ShowHelp(this,helpTopic);
			hlpevent.Handled = true;
      }
      #endregion

      //--------------------------------------------------------------------
      // textDisplayName_TextChanged
      //--------------------------------------------------------------------
      private void textDisplayName_TextChanged(object sender, EventArgs e)
      {
         if (textDisplayName.Text.Trim() == "" )
         {
            btnOK.Enabled = false;
         }
         else
         {
            btnOK.Enabled = true;
         }
      }
	}
}
