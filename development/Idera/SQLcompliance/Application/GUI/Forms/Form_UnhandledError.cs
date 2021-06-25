using System ;
using System.Windows.Forms ;
using Idera.SQLcompliance.Application.GUI.Properties ;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
	/// <summary>
	/// Summary description for Form_UnhandledError.
	/// </summary>
	public partial class Form_UnhandledError : Form
	{
		public Form_UnhandledError(string errorString)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
         this.Icon = Resources.SQLcompliance_product_ico;
         _tbError.Text = errorString; 
         _tbError.SelectionLength = 0 ;
		}

   
      private void _btnCopyToClipboard_Click(object sender, EventArgs e)
      {
         Clipboard.SetDataObject(_tbError.Text, true) ;
      }
   }
}
