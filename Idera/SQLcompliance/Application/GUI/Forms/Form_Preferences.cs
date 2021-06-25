using System ;
using System.Windows.Forms ;
using Idera.SQLcompliance.Application.GUI.Controls ;
using Idera.SQLcompliance.Application.GUI.Helper ;
using Idera.SQLcompliance.Application.GUI.Properties ;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
	/// <summary>
	/// Summary description for Form_Preferences.
	/// </summary>
	public partial class Form_Preferences : Form
	{
		#region Properties
		
      private Button btnRestoreDefaults;
      private RadioButton rbShowLocalTime;
      private TabPage tabAlertVeiws;
      private GroupBox groupBox2;
      private Label label2;
      private NumericTextBox _tbAlertPageSize;
      private Label label5;
      private Button _btnRestoreAlertDefaults;
		
		#endregion

		public Form_Preferences()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
         this.Icon = Resources.SQLcompliance_product_ico;

			textEventPageSize.Text = Settings.Default.EventPageSize.ToString();
         _tbAlertPageSize.Text = Settings.Default.AlertPageSize.ToString() ;
			
			if ( Settings.Default.ShowLocalTime )
			   rbShowLocalTime.Checked = true;
			else
			   rbShowServerLocalTime.Checked = true;
		}


      private void btnCancel_Click(object sender, EventArgs e)
      {
         this.DialogResult = DialogResult.Cancel;
         this.Close();
      }

      private void btnOK_Click(object sender, EventArgs e)
      {
         // validate
         int ps = UIUtils.TextToInt( textEventPageSize.Text ); 
         if ( ps <= 0 || ps > 99999 )
         {
            ErrorMessage.Show( this.Text, UIConstants.Error_IllegalValue_PageSize );
            return;
         }

         int pageAlerts = UIUtils.TextToInt(_tbAlertPageSize.Text) ;
         if(pageAlerts <= 0 || pageAlerts > 99999)
         {
            ErrorMessage.Show( this.Text, UIConstants.Error_IllegalValue_AlertPageSize );
            return;
         }
         
         // write
         Settings.Default.EventPageSize = ps;
         Settings.Default.AlertPageSize = pageAlerts;
         Settings.Default.ShowLocalTime = rbShowLocalTime.Checked;

         // leave         
         this.DialogResult = DialogResult.OK;
         this.Close();
      }

      #region Help
      //--------------------------------------------------------------------
      // Form_Preferences_HelpRequested - Show Context Sensitive Help
      //--------------------------------------------------------------------
      private void Form_Preferences_HelpRequested(object sender, HelpEventArgs hlpevent)
      {
         if(tabControl1.SelectedTab == tabAlertVeiws)
            HelpAlias.ShowHelp(this,HelpAlias.SSHELP_Alerts_Console_Preferences);
         else
            HelpAlias.ShowHelp(this,HelpAlias.SSHELP_Form_Preferences);
			hlpevent.Handled = true;
      }
      #endregion

      private void btnRestoreDefaults_Click(object sender, EventArgs e)
      {
			textEventPageSize.Text = Globals.EventPageSizeDefault.ToString();
	      rbShowLocalTime.Checked = true;
      }

      private void _btnRestoreAlertDefaults_Click(object sender, EventArgs e)
      {
         _tbAlertPageSize.Text = Globals.AlertPageSizeDefault.ToString() ;
      }
	}
}
