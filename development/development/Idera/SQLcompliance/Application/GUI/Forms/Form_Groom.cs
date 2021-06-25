using System ;
using System.Collections ;
using System.Windows.Forms ;
using Idera.SQLcompliance.Application.GUI.Helper ;
using Idera.SQLcompliance.Application.GUI.Properties ;
using Idera.SQLcompliance.Application.GUI.SQL ;
using Idera.SQLcompliance.Core ;
using Idera.SQLcompliance.Core.Agent ;
using Idera.SQLcompliance.Core.Collector ;
using Idera.SQLcompliance.Core.Scripting ;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
	/// <summary>
	/// Summary description for Form_Groom.
	/// </summary>
	public partial class Form_Groom : Form
	{
      public int Age = -1;
      private string m_instance = "";      


		public Form_Groom( string instance )
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
			

			if ( ! Globals.SQLcomplianceConfig.GroomEventAllow )
			{
			   btnOK.Enabled = false;
			}
			
			textAge.Text = Globals.SQLcomplianceConfig.GroomEventAge.ToString();

			Cursor = Cursors.Default;
		}


      private void btnCancel_Click(object sender, EventArgs e)
      {
         this.DialogResult = DialogResult.Cancel;
         this.Close();
      }

      private void btnOK_Click(object sender, EventArgs e)
      {
			this.Age = UIUtils.TextToInt(textAge.Text);
			if ( this.Age < 1 || this.Age > 999 )
			{
            ErrorMessage.Show( this.Text, UIConstants.Error_IllegalGroomingAge );
			   return;
			}
			
			// Grooming
         if ( radioServer.Checked )
         {
            // Groom one instance
            bool hasIntegrity ;
            bool succeeded;
            
            if ( _cbSkipIntegrityCheck.Checked )
            {
               succeeded    = true;
               hasIntegrity = true;
            }
            else
            {
               succeeded = IntegrityCheck.CheckAndRepair( "Groom Audit Data",
                                                          "Grooming",
                                                          comboServer.Text,
                                                          out hasIntegrity );
            }
            
            if ( succeeded )
            {                                                            
               if ( hasIntegrity )
               {
                  GroomInstance( comboServer.Text, this.Age, true );
               }
               else
               {
                  MessageBox.Show( String.Format( UIConstants.Error_GroomAbortedAfterIntegrityCheck,
                                                comboServer.Text ),
                                 this.Text );
               }
            }
         }
         else
         {
            // Groom all instance
            for ( int i=0; i<comboServer.Items.Count; i++ )
            {
               bool hasIntegrity ;
               bool succeeded;
               
               if ( _cbSkipIntegrityCheck.Checked )
               {
                  succeeded    = true;
                  hasIntegrity = true;
               }
               else
               {
                  succeeded = IntegrityCheck.CheckAndRepair( "Groom Audit Data",
                                                             "Grooming",
                                                             comboServer.Items[i].ToString(),
                                                             out hasIntegrity );
               }
               
               if ( succeeded )
               {
                  if ( hasIntegrity)
                  {                                                          
                     bool continueWithNext = GroomInstance( comboServer.Items[i].ToString(),
                                                            this.Age,
                                                            (i==comboServer.Items.Count-1) );
                     if ( ! continueWithNext ) break;                                        
                  }                 
                  else
                  {
                     MessageBox.Show( String.Format( UIConstants.Error_GroomAbortedAfterIntegrityCheck,
                                                     comboServer.Text ),
                                    this.Text );
                  }
               }
               else
               {
                  break;
               }
            }
         }
      
         this.DialogResult = DialogResult.OK;
         this.Close();
      }
      
      private bool
         GroomInstance(
            string instance,
            int    age,
            bool   lastInstance
         )
      {
         string msg;
         bool continueWithNext = true;
         
         Form_GroomingProgress frm = new Form_GroomingProgress( instance, age,
            _cbSkipIntegrityCheck.Checked ? IntegrityCheckAction.SkipCheck : IntegrityCheckAction.CheckAlreadyDone);
         frm.ShowDialog();
         
         if ( frm.Success )
         {
            msg = String.Format( CoreConstants.Info_GroomSuccessLog, age, instance);
			   MessageBox.Show(msg, "Groom Audit Data"); 
			}
			else
			{
            msg = String.Format( UIConstants.Error_GroomFailed, instance, frm.ErrMsg );            
            
            MessageBoxButtons btns = MessageBoxButtons.OK;
            if ( ! lastInstance )
            {
               msg  = msg + UIConstants.Error_GroomFailedContinue;
               btns = MessageBoxButtons.YesNo;
            }
            
            DialogResult choice = MessageBox.Show( msg,
                                                   "Groom Audit Data",
                                                   btns,
                                                   MessageBoxIcon.Error );
            if ( ! lastInstance )
            {
			      continueWithNext = (choice == DialogResult.Yes);
            }
			}
         
         return continueWithNext;
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
            if (comboServer.Items.Count != 0)
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
		   HelpAlias.ShowHelp(this,HelpAlias.SSHELP_Form_Groom);
			hlpevent.Handled = true;
      }
      #endregion

      private void Click_btnScript(object sender, EventArgs e)
      {
         GenerateScript() ;
      }

      private void GenerateScript()
      {
         string script = ScriptGenerator.GenerateGroom(ScriptGenerator.GenerateGlobals(Globals.SQLcomplianceConfig),
            radioAll.Checked ? "-all" : comboServer.Text, UIUtils.TextToInt(textAge.Text), _cbSkipIntegrityCheck.Checked) ;

         Form_Script frm = new Form_Script() ;

         frm.Script = script ;
         frm.ShowDialog(this) ;


      }
	}
}
