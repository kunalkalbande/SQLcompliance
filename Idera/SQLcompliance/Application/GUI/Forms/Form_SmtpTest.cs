using System.Windows.Forms ;
using Idera.SQLcompliance.Application.GUI.Properties ;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
	/// <summary>
	/// Summary description for SmtpTest.
	/// </summary>
	public partial class Form_SmtpTest : Form
	{
		public Form_SmtpTest()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
         this.Icon = Resources.SQLcompliance_product_ico;
		}

		public string Recepient
		{
			get { return _tbRecepient.Text ; }
			set { _tbRecepient.Text = value ; }
		}

	}
}
