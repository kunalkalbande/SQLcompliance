using System.Collections;
using System.Windows.Forms;
using Idera.SQLcompliance.Application.GUI.Properties ;
using Idera.SQLcompliance.Core.Rules;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
	/// <summary>
	/// Summary description for Form_EventTypeTree.
	/// </summary>
	public partial class Form_EventTypeList : System.Windows.Forms.Form
	{
		private ArrayList _categories ;
		private Hashtable _allEvents ;
		private Hashtable _selectedEventTypes ;

		public Form_EventTypeList(CMEventCategory[] categories)
		{
			InitializeComponent();
         this.Icon = Resources.SQLcompliance_product_ico;
         _categories = new ArrayList(categories);
			_selectedEventTypes = new Hashtable();
			_allEvents = new Hashtable() ;

			foreach(CMEventCategory category in _categories)
			{
				foreach(CMEventType evType in category.EventTypes)
				{
					_allEvents.Add(evType.TypeId, evType) ;
				}
			}
		}

		public int[] SelectedEventIds
		{
			get 
			{
				int[] retVal = new int[_selectedEventTypes.Count];
				int i = 0 ; 
				foreach(CMEventType evType in _selectedEventTypes.Values)
					retVal[i++] = evType.TypeId ;
				return retVal ;
			}

			set
			{
				_selectedEventTypes.Clear();
				if(value == null)
					return ;
				foreach(int evId in value)
				{
					_selectedEventTypes[evId] = _allEvents[evId] ;
				}
				RebuildCheckedList();
			}
		}

		private void RebuildCheckedList()
		{
			_listBoxEventTypes.Items.Clear() ;
			foreach(CMEventCategory category in _categories)
			{
				foreach(CMEventType evType in category.EventTypes)
				{
					_listBoxEventTypes.Items.Add(evType, _selectedEventTypes.ContainsKey(evType.TypeId)) ;
				}
			}
		}

		private void Click_btnSelectAll(object sender, System.EventArgs e)
		{
			for(int i = 0 ; i < _listBoxEventTypes.Items.Count ; i++)
			{
				CMEventType item = (CMEventType)_listBoxEventTypes.Items[i] ;
				_listBoxEventTypes.SetItemChecked(i, true);
				if(!_selectedEventTypes.ContainsKey(item.TypeId))
				{
					_selectedEventTypes.Add(item.TypeId, item);
				}
			}
		}

		private void Click_btnClearAll(object sender, System.EventArgs e)
		{
			for(int i = 0 ; i < _listBoxEventTypes.Items.Count ; i++)
			{
				CMEventType item = (CMEventType)_listBoxEventTypes.Items[i] ;
				_listBoxEventTypes.SetItemChecked(i, false);
				if(_selectedEventTypes.ContainsKey(item.TypeId))
				{
					_selectedEventTypes.Remove(item.TypeId);
				}
			}
		}

		private void ItemCheck_listBoxEventTypes(object sender, System.Windows.Forms.ItemCheckEventArgs e)
		{
			CMEventType evType = (CMEventType)_listBoxEventTypes.Items[e.Index] ;
			if(e.NewValue == CheckState.Checked)
			{
				if(!_selectedEventTypes.ContainsKey(evType.TypeId))
					_selectedEventTypes.Add(evType.TypeId, evType);
			}
			else if(e.NewValue == CheckState.Unchecked)
			{
				_selectedEventTypes.Remove(evType.TypeId);
			}
		}
	}
}
