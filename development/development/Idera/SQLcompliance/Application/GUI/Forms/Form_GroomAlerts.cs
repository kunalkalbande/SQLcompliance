using System ;
using System.Collections ;
using System.Windows.Forms ;
using Idera.SQLcompliance.Application.GUI.Controls ;
using Idera.SQLcompliance.Application.GUI.Helper ;
using Idera.SQLcompliance.Application.GUI.Properties ;
using Idera.SQLcompliance.Application.GUI.SQL ;
using Idera.SQLcompliance.Core ;
using Idera.SQLcompliance.Core.Agent ;
using Idera.SQLcompliance.Core.Rules.Alerts ;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
   /// <summary>
   /// Summary description for Form_Groom.
   /// </summary>
   public partial class Form_GroomAlerts : Form
   {
      private AlertingConfiguration _configuration ;

      public int _age = -1;
      private GroupBox groupBox3;
      private ComboBox comboServer;
      private RadioButton radioServer;
      private RadioButton radioAll;
      private NumericTextBox textAge;


      public Form_GroomAlerts(AlertingConfiguration config)
      {
         _configuration = config ;
         //
         // Required for Windows Form Designer support
         //
         InitializeComponent();
         this.Icon = Resources.SQLcompliance_product_ico;

         Cursor = Cursors.WaitCursor;
			
         // load server combo
         LoadServerDropDown();
			
         textAge.Text = Globals.SQLcomplianceConfig.GroomAlertAge.ToString();

         Cursor = Cursors.Default;
      }


      private void btnCancel_Click(object sender, EventArgs e)
      {
         this.DialogResult = DialogResult.Cancel;
         this.Close();
      }

      private void btnOK_Click(object sender, EventArgs e)
      {
         this._age = UIUtils.TextToInt(textAge.Text);
         if ( this._age < 1 || this._age > 999 )
         {
            ErrorMessage.Show( this.Text, UIConstants.Error_IllegalAlertGroomingAge );
            return;
         }

         Form_AlertGroomProgress groomProgress ;
         string msg ;

         if(radioServer.Checked)
            groomProgress = new Form_AlertGroomProgress(comboServer.Text, _age, _configuration) ;
         else
            groomProgress = new Form_AlertGroomProgress(null, _age, _configuration) ;
			
         groomProgress.ShowDialog(this) ;

         if ( groomProgress.Success )
         {
            if(radioServer.Checked)
            {
               msg = String.Format( "Alerts older than {0} days groomed for SQL Server instance {1}.  {2} alerts were groomed.", _age, comboServer.Text, groomProgress.NumberRemoved );
               LogRecord.WriteLog( Globals.Repository.Connection, LogType.GroomAlerts, comboServer.Text, msg ); 
            }
            else
            {
               msg = String.Format( "Alerts older than {0} days groomed for all instances.  {1} alerts were groomed.", _age, groomProgress.NumberRemoved);
               LogRecord.WriteLog( Globals.Repository.Connection, LogType.GroomAlerts, msg );                                   
            }
            MessageBox.Show(msg, "Groom Alerts"); 
         }
         else
         {
            msg = String.Format( UIConstants.Error_GroomFailedAlerts, groomProgress.ErrMsg );
                                        
            LogRecord.WriteLog( Globals.Repository.Connection, LogType.GroomAlerts, msg );

            MessageBox.Show(msg, "Groom Alerts", MessageBoxButtons.OK, MessageBoxIcon.Error) ;                    
         }
         this.DialogResult = DialogResult.OK;
         this.Close();
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
               if ( comboServer.Text == "" )
               {
                  comboServer.Text = comboServer.Items[0].ToString();
                  radioServer.Checked = true;
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

      private void radioServer_CheckedChanged(object sender, EventArgs e)
      {
         comboServer.Enabled = radioServer.Checked;
      }
      

      #region Help
      //--------------------------------------------------------------------
      // Form_Defaults_HelpRequested - Show Context Sensitive Help
      //--------------------------------------------------------------------
      private void Form_Defaults_HelpRequested(object sender, HelpEventArgs hlpevent)
      {
         HelpAlias.ShowHelp(this,HelpAlias.SSHELP_Alerts_Groom);
         hlpevent.Handled = true;
      }
      #endregion
   }
}
