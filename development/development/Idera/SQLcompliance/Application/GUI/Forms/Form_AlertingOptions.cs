using System ;
using System.Windows.Forms ;
using Idera.SQLcompliance.Application.GUI.Properties ;
using Idera.SQLcompliance.Core.Rules.Alerts ;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
	/// <summary>
	/// Summary description for Form_AlertingOptions.
	/// </summary>
	public partial class Form_AlertingOptions : Form
	{

		private SmtpConfiguration _config ;

		public Form_AlertingOptions()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

         this.Icon = Resources.SQLcompliance_product_ico;
		}

		public SmtpConfiguration SmtpSettings
		{
			get { UpdateConfiguration() ; return _config ; }
			set { _config = value ; UpdateForm() ; }
		}

		private void UpdateConfiguration()
		{
			_config.Server = _tbServer.Text ;
			_config.Port = Int32.Parse(_tbPort.Text) ;
			_config.Authentication = _cbUseAuth.Checked ? SmtpAuthProtocol.Basic : SmtpAuthProtocol.Anonymous ;
			_config.UseSsl = _cbUseSsl.Checked ;
			if(_config.Authentication == SmtpAuthProtocol.Basic)
			{
				_config.Username = _tbUsername.Text ;
				_config.Password = _tbPassword.Text ;
			}
			else
			{
				_config.Username = "" ;
				_config.Password = "" ;
			}
			_config.SenderAddress = _tbSenderAddress.Text ;
		}

		private void UpdateForm()
		{
			_tbServer.Text = _config.Server ;
			_tbPort.Text = _config.Port.ToString() ;
			_cbUseSsl.Checked = _config.UseSsl ;
			_tbUsername.Text = _config.Username ;
			_tbPassword.Text = _config.Password ;
			if(_config.Authentication == SmtpAuthProtocol.Basic)
				_cbUseAuth.Checked = true ;
			else
				_cbUseAuth.Checked = false ;
			_tbSenderAddress.Text = _config.SenderAddress ;
		}

		private void CheckedChanged_cbUseAuth(object sender, EventArgs e)
		{
			if(_cbUseAuth.Checked)
			{
				_lblUsername.Enabled = true ;
				_lblPassword.Enabled = true ;
				_tbUsername.Enabled = true ;
				_tbPassword.Enabled = true ;
			}
			else
			{
				_lblUsername.Enabled = false ;
				_lblPassword.Enabled = false ;
				_tbUsername.Enabled = false ;
				_tbPassword.Enabled = false ;
			}
		}

		private void Click_btnTest(object sender, EventArgs e)
		{

			Form_SmtpTest form = new Form_SmtpTest() ;
			if(form.ShowDialog(this) == DialogResult.OK)
			{
            string errorString ;
				UpdateConfiguration() ;
            _btnTest.Enabled = false ;
				if(ActionProcessor.PerformSmtpTest(form.Recepient, "SQLCompliance Manager SMTP Test", "This is a test.", _config, out errorString))
				{
					MessageBox.Show(this, "The SMTP test was successful.  Please check the recepient mailbox for a test message.", "Test Successful") ;
				}
				else
				{
					MessageBox.Show(this, String.Format("The SMTP test failed for the following reason: {0}", errorString), "Test Failed", 
						MessageBoxButtons.OK, MessageBoxIcon.Error) ;
				}
            _btnTest.Enabled = true ;
			}
		}

      private void Form_AlertingOptions_HelpRequested(object sender, HelpEventArgs hlpevent)
      {
         HelpAlias.ShowHelp(this,HelpAlias.SSHELP_SMTP_Settings);      
         hlpevent.Handled = true ;
      }
	}
}
