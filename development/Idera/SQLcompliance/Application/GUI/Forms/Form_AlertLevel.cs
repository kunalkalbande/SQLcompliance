using System.Windows.Forms ;
using Idera.SQLcompliance.Application.GUI.Properties ;
using Idera.SQLcompliance.Core.Rules.Alerts ;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
	/// <summary>
	/// Summary description for Form_AlertLevel.
	/// </summary>
	public partial class Form_AlertLevel : Form
	{
		public Form_AlertLevel()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
         this.Icon = Resources.SQLcompliance_product_ico;

         _comboAlertLevel.Items.Add(AlertLevel.Severe) ;
         _comboAlertLevel.Items.Add(AlertLevel.High) ;
         _comboAlertLevel.Items.Add(AlertLevel.Medium) ;
         _comboAlertLevel.Items.Add(AlertLevel.Low) ;
      }

	   public AlertLevel Level
	   {
	      get 
         {
            return (AlertLevel)_comboAlertLevel.SelectedItem ;
         }
	      set 
         { 
            _comboAlertLevel.SelectedItem = value ;
         }
	   }
	}
}
