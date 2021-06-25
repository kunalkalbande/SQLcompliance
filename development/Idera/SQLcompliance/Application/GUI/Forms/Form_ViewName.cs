using System ;
using System.Windows.Forms ;
using Idera.SQLcompliance.Application.GUI.Properties ;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
	/// <summary>
	/// Summary description for SmtpTest.
	/// </summary>
	public partial class Form_ViewName : Form
	{
      string[] _existingNames;
      public Form_ViewName(string[] existingNames)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
         _existingNames = existingNames;
         this.Icon = Resources.SQLcompliance_product_ico;
		}

		public string ViewName
		{
			get { return _tbViewName.Text.Trim() ; }
			set { _tbViewName.Text = value ; }
		}

      private void Click_btnOk(object sender, System.EventArgs e)
      {
         string strValue = _tbViewName.Text.Trim();
         if (String.IsNullOrEmpty(strValue))
         {
            MessageBox.Show(this, "Blank view names are not allowed.  Please specify a name containing alphanumeric characters.", "Invalid Name", MessageBoxButtons.OK);
            DialogResult = DialogResult.None ;
            return;
         }
         foreach(string s in _existingNames)
            if (s == strValue)
            {
               MessageBox.Show(this, String.Format("A view named {0} already exists.  Please specify a different name.", strValue), "View Already Exists", MessageBoxButtons.OK);
               DialogResult = DialogResult.None;
               return;
            }
         DialogResult = DialogResult.OK;
         Close();
      }

	}
}
