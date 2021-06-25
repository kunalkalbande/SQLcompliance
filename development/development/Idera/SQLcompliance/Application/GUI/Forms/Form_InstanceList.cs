using System.Windows.Forms ;
using Idera.SQLcompliance.Application.GUI.Properties ;
using Idera.SQLcompliance.Core ;
using Idera.SQLcompliance.Core.Rules ;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
	/// <summary>
	/// Summary description for Form_InstanceList.
	/// </summary>
	public partial class Form_InstanceList : Form
	{
		public Form_InstanceList()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
         this.Icon = Resources.SQLcompliance_product_ico;

		   Repository rep = new Repository() ;
         rep.OpenConnection(false) ;
         string[] availableInstances = RulesDal.SelectRegisteredInstances(rep.connection) ;
         rep.CloseConnection() ;

		   _listBoxServers.Items.AddRange(availableInstances) ;

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

      public string[] SelectedInstances
      {
         get 
         {
            string[] retVal = new string[_listBoxServers.SelectedItems.Count] ;
            int i = 0 ;
            foreach(string s in _listBoxServers.SelectedItems)
               retVal[i++] = s ;
            return retVal ;
         }

         set 
         {
            if(value == null || value.Length == 0)
               _listBoxServers.ClearSelected() ;
            else
            {
               foreach(string s in value)
               {
                  if(_listBoxServers.Items.Contains(s))
                     _listBoxServers.SelectedIndex = _listBoxServers.Items.IndexOf(s) ;
               }
            }
         }
      }


	}
}
