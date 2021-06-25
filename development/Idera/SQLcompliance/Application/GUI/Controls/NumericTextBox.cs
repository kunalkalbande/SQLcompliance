using System ;
using System.Windows.Forms ;

namespace Idera.SQLcompliance.Application.GUI.Controls
{
	/// <summary>
	/// Summary description for NumericTextBox.
	/// </summary>
	public partial class NumericTextBox : TextBox
	{
		public NumericTextBox()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call

		}

      protected override void OnKeyPress(KeyPressEventArgs e)
      {
         if (!Char.IsDigit(e.KeyChar) && e.KeyChar !=(char)8)
         {
            e.Handled=true; // input is not passed on to the control(TextBox) 
         }
         base.OnKeyPress (e);
      }

	}
}
