using System ;
using System.Windows.Forms ;
using Idera.SQLcompliance.Application.GUI.Properties ;
using Idera.SQLcompliance.Core.Rules ;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
	/// <summary>
	/// Summary description for Form_EventSelector.
	/// </summary>
	public partial class Form_EventCategorySelector : Form
	{
      private CMEventCategory _selectedCategory ;

		public Form_EventCategorySelector(CMEventCategory[] categories, CMEventCategory selectedCategory)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

         this.Icon = Resources.SQLcompliance_product_ico;
         _comboCategory.Items.AddRange(categories);
         _comboCategory.SelectedItem = selectedCategory ;
         _selectedCategory = selectedCategory ;
		}

      public CMEventCategory SelectedCategory
      {
         get { return _selectedCategory ; }
      }


      private void SelectedIndexChanged_comboCategory(object sender, EventArgs e)
      {
         CMEventCategory selectedCategory = (CMEventCategory)_comboCategory.SelectedItem ;
         if(selectedCategory != null)
            _selectedCategory = selectedCategory ;
      }
	}
}
