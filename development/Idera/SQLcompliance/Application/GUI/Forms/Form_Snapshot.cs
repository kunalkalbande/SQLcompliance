using System ;
using System.Collections ;
using System.Windows.Forms ;
using Idera.SQLcompliance.Application.GUI.Helper ;
using Idera.SQLcompliance.Application.GUI.Properties ;
using Idera.SQLcompliance.Application.GUI.SQL ;
using Idera.SQLcompliance.Core ;
using Idera.SQLcompliance.Core.Agent ;
using Idera.SQLcompliance.Core.Status ;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
	/// <summary>
	/// Summary description for Form_Snapshot.
	/// </summary>
	public partial class Form_Snapshot : Form
	{
      private string m_instance = "";      

		public Form_Snapshot( string instance )
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

			Cursor = Cursors.Default;
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
         // Validate
         if ( radioServer.Checked && comboServer.Text == "" )
         {
            MessageBox.Show( UIConstants.Error_ServerRequired,
                             this.Text );
            return;
         }


         // Do snapshots         
         if  ( radioAll.Checked )
         {
            Snapshot.DumpAllServers(Globals.Repository.Connection);
         }
         else
         {
            ServerRecord srv = new ServerRecord();
            srv.Connection = Globals.Repository.Connection;
            if ( srv.Read( comboServer.Text ) )
            {
               string snapshot = Snapshot.ServerSnapshot( Globals.Repository.Connection,
                                                          srv,
                                                          true );
               LogRecord.WriteLog( Globals.Repository.Connection,
                                   LogType.Snapshot,
                                   comboServer.Text,
                                   snapshot );                                   
            }
         }
         
         // Time to go      
         this.DialogResult = DialogResult.OK;
         this.Close();
      }
      
      #region Help
      //--------------------------------------------------------------------
      // Form_Snapshot_HelpRequested - Show Context Sensitive Help
      //--------------------------------------------------------------------
      private void Form_Snapshot_HelpRequested(object sender, HelpEventArgs hlpevent)
      {
		   HelpAlias.ShowHelp(this,HelpAlias.SSHELP_Form_Snapshot);
			hlpevent.Handled = true;
      }
      #endregion

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
         
         ICollection serverList;
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
               if ( m_instance != "" )
               {
                  if ( comboServer.FindString(m_instance) != -1 )
                  {
                     comboServer.Text = m_instance;
                     radioServer.Checked = true;
                  }
               }
               if ( comboServer.Text == "" )
               {
                  comboServer.Text = comboServer.Items[0].ToString();
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
	}
}
