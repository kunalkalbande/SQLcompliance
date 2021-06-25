using System ;
using System.Windows.Forms ;
using Idera.SQLcompliance.Application.GUI.Properties ;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
	/// <summary>
	/// Summary description for Form_StringSearch.
	/// </summary>
	public partial class Form_EmailList : Form
	{
		public Form_EmailList()
		{
			InitializeComponent();
         this.Icon = Resources.SQLcompliance_product_ico;
      }

		public string[] EmailArray
		{
			get 
			{ 
				string[] retVal = new string[_listBoxStringList.Items.Count] ;
				for(int i = 0 ; i < _listBoxStringList.Items.Count ; i++)
				{
					retVal[i] = (string)_listBoxStringList.Items[i] ;
				}
				return retVal ; 
			}
			set 
			{ 
				_listBoxStringList.Items.Clear() ;
				if(value == null)
					return ;
				foreach(string s in value)
				{
					_listBoxStringList.Items.Add(s) ;
				}
			}
		}


      #region Actions

      private void AddAddress()
      {
         if(!ValidEmailAddress(_tbStringEntry.Text))
         {
            MessageBox.Show(this, "Invalid email address format.", "Warning") ;
            return ;
         }
         foreach(string s in _listBoxStringList.Items)
         {
            if(String.Compare(s, _tbStringEntry.Text, true) == 0)
            {
               MessageBox.Show(this, "The list already contains " + _tbStringEntry.Text + ".", "Warning") ;
               return ;
            }
         }
         _listBoxStringList.Items.Add(_tbStringEntry.Text) ;
         _tbStringEntry.Text = "" ;
         _tbStringEntry.Focus() ;
      }

      private void RemoveAddress()
      {
         int oldIndex = _listBoxStringList.SelectedIndex ;
         if(oldIndex == -1)
            return ;

         _listBoxStringList.Items.RemoveAt(_listBoxStringList.SelectedIndex);
         if(_listBoxStringList.Items.Count > 0)
         {
            if(oldIndex >= _listBoxStringList.Items.Count)
               _listBoxStringList.SelectedIndex = oldIndex - 1 ;
            else
               _listBoxStringList.SelectedIndex = oldIndex ;
         }
      }

      // TODO:  make sure this is correct RFC822
      private bool ValidEmailAddress(string s)
      {
         int nAt, nDot ;

         if(s.Length < 5)
            return false ;
         nAt = s.IndexOf('@') ;
         if(nAt == -1)
            return false ;
         nDot = s.IndexOf('.',nAt) ;
         if(nDot == -1)
            return false; 

         return true ;
      }

      #endregion // Actions

      #region Event Handlers

		private void Click_btnAdd(object sender, EventArgs e)
		{
         AddAddress() ;
		}

		private void Click_btnRemove(object sender, EventArgs e)
		{
         RemoveAddress() ;
		}

		private void Click_btnOK(object sender, EventArgs e)
		{
		
		}

		private void Click_btnCancel(object sender, EventArgs e)
		{
		
		}

		private void TextChanged_tbStringEntry(object sender, EventArgs e)
		{
			if(_tbStringEntry.Text.Length > 0)
				_btnAdd.Enabled = true ;
			else
				_btnAdd.Enabled = false ;
		}

		private void SelectedIndexChanged_listBoxStringList(object sender, EventArgs e)
		{
			if(_listBoxStringList.SelectedIndex >= 0)
				_btnRemove.Enabled = true ;
			else
				_btnRemove.Enabled = false ;
		}

      private void KeyDown_Form_EmailList(object sender, KeyEventArgs e)
      {
         if(e.KeyCode == Keys.Enter)
         {
            if(_tbStringEntry.Focused && _tbStringEntry.Text.Length > 0)
               AddAddress() ;
            else
               _btnOk.PerformClick() ;
         }
         if(e.KeyCode == Keys.Delete)
         {
            if(_listBoxStringList.Focused && _listBoxStringList.SelectedIndex >= 0)
               RemoveAddress() ;
         }
      }

      #endregion // Event Handlers

      private void Form_EmailList_HelpRequested(object sender, HelpEventArgs hlpevent)
      {
         HelpAlias.ShowHelp(this,HelpAlias.SSHELP_Alerts_Distribution_List);
         hlpevent.Handled = true;
      }
	}
}
