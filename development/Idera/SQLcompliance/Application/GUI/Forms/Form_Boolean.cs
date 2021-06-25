using System.Windows.Forms ;
using Idera.SQLcompliance.Application.GUI.Properties ;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
	/// <summary>
	/// Summary description for Form_SimpleDropDown.
	/// </summary>
	public partial class Form_Boolean : Form
	{
		public Form_Boolean()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
         this.Icon = Resources.SQLcompliance_product_ico;

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

      public bool BooleanValue
      {
         get 
         {
            if(_comboBool.SelectedItem.ToString() == "True")
               return true ;
            else
               return false ;
         }

         set 
         {
            if(value)
               _comboBool.SelectedItem = "True" ;
            else
               _comboBool.SelectedItem = "False" ;
         }
      }

      public string Title
      {
         get { return this.Text; }
         set { this.Text = value ; }
      }

      public string Directions
      {
         get { return _lblDirections.Text; }
         set { _lblDirections.Text = value ; }
      }

	}
}
