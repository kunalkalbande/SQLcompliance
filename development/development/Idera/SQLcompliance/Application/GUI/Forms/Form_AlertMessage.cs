using System.Windows.Forms ;
using Idera.SQLcompliance.Application.GUI.Properties ;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
	/// <summary>
	/// Summary description for Form_AlertMessage.
	/// </summary>
	public partial class Form_AlertMessage : Form
	{
		public Form_AlertMessage()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
         this.Icon = Resources.SQLcompliance_product_ico;
      }

      public string Title
      {
         get { return _tbTitle.Text; }
         set { _tbTitle.Text = value ; }
      }

      public string Body
      {
         get { return _tbBody.Text; }
         set { _tbBody.Text = value ; }
      }
	}
}
