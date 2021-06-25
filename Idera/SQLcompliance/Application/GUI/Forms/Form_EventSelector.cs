using System ;
using System.Windows.Forms ;
using Idera.SQLcompliance.Application.GUI.Properties ;
using Idera.SQLcompliance.Core.Rules ;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
	/// <summary>
	/// Summary description for Form_EventSelector.
	/// </summary>
	public partial class Form_EventSelector : Form
	{
      private CMEventCategory _selectedCategory ;
      private CMEventType _selectedEvent ;

		public Form_EventSelector(CMEventCategory[] categories, CMEventCategory selectedCategory, CMEventType selectedEvent)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
         this.Icon = Resources.SQLcompliance_product_ico;

         _comboCategory.Items.AddRange(categories) ;
         _comboCategory.SelectedItem = selectedCategory ;
         _comboEventType.SelectedItem = selectedEvent ;
		}

      public CMEventType SelectedEvent
      {
         get { return _selectedEvent ; }
      }


      private void SelectedIndexChanged_comboCategory(object sender, EventArgs e)
      {
         CMEventCategory selectedCategory = (CMEventCategory)_comboCategory.SelectedItem ;
         if(selectedCategory != null)
         {
            _selectedCategory = selectedCategory ;
            _comboEventType.Items.Clear() ;
            foreach(CMEventType evType in _selectedCategory.EventTypes)
               _comboEventType.Items.Add(evType) ;
            _comboEventType.SelectedItem = _selectedCategory.EventTypes[0] ;
         }
      
      }

      private void SelectedIndexChanged_comboEventType(object sender, EventArgs e)
      {
         CMEventType selectedType = (CMEventType)_comboEventType.SelectedItem ;
         if(selectedType != null)
            _selectedEvent = selectedType ;
      
      }
	}
}
