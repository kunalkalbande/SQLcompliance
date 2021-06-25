using System ;
using System.Windows.Forms ;
using Idera.SQLcompliance.Application.GUI.Properties ;
using Idera.SQLcompliance.Core.Rules.Alerts ;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
	/// <summary>
	/// Summary description for Form_SmtpOptions.
	/// </summary>
	public partial class Form_EmailTemplate : Form
	{

		private TextBox _lastFocusBox ;

		public Form_EmailTemplate(MacroDefinition[] macros)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

         this.Icon = Resources.SQLcompliance_product_ico;
         _lastFocusBox = null;
			_listBoxMacros.Items.AddRange(macros) ;
		}


		public string Subject
		{
			get { return _tbSubject.Text ; }
			set { _tbSubject.Text = value ; }
		}

		public string Message
		{
			get { return _tbMessage.Text ; }
			set { _tbMessage.Text = value ; }
		}

		private void DoubleClick_listBoxMacros(object sender, EventArgs e)
		{
			if(_lastFocusBox != null)
			{
				MacroDefinition def = (MacroDefinition)_listBoxMacros.SelectedItem ;

				int i = _lastFocusBox.SelectionStart ;
				_lastFocusBox.Focus() ;
				String s = _lastFocusBox.Text ;
				s = s.Insert(_lastFocusBox.SelectionStart, def.Value) ;
				_lastFocusBox.Text = s ;
				_lastFocusBox.SelectionStart = i + def.Value.Length ;
			}
		}

		private void Focus_Enter(object sender, EventArgs e)
		{
			if(sender is TextBox && _lastFocusBox != sender)
				_lastFocusBox = sender as TextBox ;
		}

      private void KeyDown_Form_EmailTemplate(object sender, KeyEventArgs e)
      {
         if(e.KeyCode == Keys.Enter)
         {
            if(!_tbMessage.Focused)
               _btnOk.PerformClick() ;
         }
      }

      private void Form_EmailTemplate_HelpRequested(object sender, HelpEventArgs hlpevent)
      {
         HelpAlias.ShowHelp(this,HelpAlias.SSHELP_Alert_Message_Template);
         hlpevent.Handled = true;
      }
	}
}
