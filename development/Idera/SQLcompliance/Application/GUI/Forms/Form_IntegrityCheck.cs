using System ;
using System.Data.SqlClient ;
using System.Windows.Forms ;
using Idera.SQLcompliance.Application.GUI.Helper ;
using Idera.SQLcompliance.Application.GUI.Properties ;
using Idera.SQLcompliance.Core ;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
	/// <summary>
	/// Summary description for Form_IntegrityCheck.
	/// </summary>
	public partial class Form_IntegrityCheck : Form
	{
      string m_instance;
      bool   m_loaded = false;
      
		public
		   Form_IntegrityCheck(
		      string instance
		   )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
         this.Icon = Resources.SQLcompliance_product_ico;

			m_instance = instance;
      }
      
      protected override void OnLoad(EventArgs e)
      {
			Cursor = Cursors.WaitCursor;
			LoadDatabases();
			Cursor = Cursors.Default;
         
         base.OnLoad (e);
      }
      
      private void LoadDatabases()
      {
         m_loaded = false;
         
			// load server combo
			GetRepositoryDatabases();
			
			if ( listDatabases.Items.Count > 0)
			{
			   btnOK.Enabled = true;
			   
			   bool found = false;
			   
			   if ( m_instance != "" )
			   {
			      // find first occurence of instance in list and select
			      foreach ( ListViewItem lvi in listDatabases.Items )
			      {
			         if ( m_instance == lvi.Text )
			         {
			            found = true;
			            lvi.Selected = true;
			            listDatabases.EnsureVisible(lvi.Index);
			            break;
			         }
			      }
			   }
			   
			   if ( ! found )
			   {
			      listDatabases.Items[0].Selected = true;
			   }
			   listDatabases.Focus();
			}
			
			m_loaded = true;
			
		}

	   //-----------------------------------------------------------------------------
	   // GetRepositoryDatabases
	   //-----------------------------------------------------------------------------
	   private void
	      GetRepositoryDatabases()
	   {
         Cursor = Cursors.WaitCursor;
         listDatabases.Items.Clear();
         
         string instance;
         string dbType;
         string dbName;

         
         // Load databases
			try
			{
			   string cmdstr = String.Format( "SELECT instance,databaseType,databaseName " +
		                                       "FROM {0} " +
		                                       "WHERE {1} databaseType='Event' " +
		                                       "ORDER by instance ASC, databaseType DESC",
			                                  CoreConstants.RepositorySystemDatabaseTable,
			                                  (checkShowArchives.Checked ) ? "databaseType='Archive' OR " : "");

			   using (  SqlCommand    cmd    = new SqlCommand( cmdstr,
			                                                   Globals.Repository.Connection ) )
            {
               using ( SqlDataReader reader = cmd.ExecuteReader() )
               {
			         while ( reader.Read() )
                  {
                     instance = SQLHelpers.GetString( reader, 0);
                     dbType   = SQLHelpers.GetString( reader, 1);
                     dbName   = SQLHelpers.GetString( reader, 2);

                     // Add to list               
                     ListViewItem lvi = new ListViewItem( instance ); 
                     lvi.SubItems.Add( dbType );
                     lvi.SubItems.Add( dbName );
                     
                     listDatabases.Items.Add(lvi);
                  }
               }
            }
			}
			catch ( Exception ex )
			{
            ErrorMessage.Show( this.Text,
                               "An error occurred loading the list for integrity checking",
                               ex.Message,
                               MessageBoxIcon.Error );
			}
			
         Cursor = Cursors.Default;
	   }

      private void btnCancel_Click(object sender, EventArgs e)
      {
         this.DialogResult = DialogResult.Cancel;
         this.Close();
      }

      private void btnOK_Click(object sender, EventArgs e)
      {
         // Request Archive
         Cursor = Cursors.WaitCursor;
         
         ListViewItem lvi = listDatabases.SelectedItems[0];
         
         bool hasIntegrity;
         
         
         IntegrityCheck.CheckAndRepair( this.Text,
                                        "",
                                        lvi.Text,
                                        lvi.SubItems[2].Text,
                                        (lvi.SubItems[1].Text == "Archive"),
                                        out hasIntegrity );
         
         Cursor = Cursors.Default;

         // Leave open so they can check more then one db
      }
      
      #region Help
      //--------------------------------------------------------------------
      // Form_Defaults_HelpRequested - Show Context Sensitive Help
      //--------------------------------------------------------------------
      private void Form_Defaults_HelpRequested(object sender, HelpEventArgs hlpevent)
      {
		   HelpAlias.ShowHelp(this,HelpAlias.SSHELP_Form_IntegrityCheck);
			hlpevent.Handled = true;
      }
      
      #endregion

      private void listDatabases_SelectedIndexChanged(object sender, EventArgs e)
      {
         if ( m_loaded )
         {
            if ( listDatabases.SelectedItems.Count == 0 )
               btnOK.Enabled = false;
            else
               btnOK.Enabled = true;
         }
      }

      private void checkShowArchives_CheckedChanged(object sender, EventArgs e)
      {
         LoadDatabases();
      
      }
	}
}
