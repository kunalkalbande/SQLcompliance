using System.Windows.Forms ;
using Idera.SQLcompliance.Application.GUI.Properties ;
using Idera.SQLcompliance.Core ;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
	/// <summary>
	/// Summary description for Form_EventLogType.
	/// </summary>
	public partial class Form_EventLogType : Form
	{
		public Form_EventLogType()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
         this.Icon = Resources.SQLcompliance_product_ico;
         _comboEntryTypes.SelectedItem = "Information";
		}

      public ErrorLog.Severity EntryType
      {
         get 
         {
            switch(_comboEntryTypes.SelectedItem.ToString())
            {
               case "Information":
                  return ErrorLog.Severity.Informational ;
               case "Error":
                  return ErrorLog.Severity.Error ;
               case "Warning":
                  return ErrorLog.Severity.Warning ;
               default:
                  return ErrorLog.Severity.Informational ;
            }
         }

         set
         {
            if(value == ErrorLog.Severity.Informational)
               _comboEntryTypes.SelectedItem = "Information" ;
            else
               _comboEntryTypes.SelectedItem = value.ToString() ;
         }
      }

	}
}
