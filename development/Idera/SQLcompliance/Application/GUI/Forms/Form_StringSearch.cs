using System ;
using System.Drawing;
using System.Windows.Forms ;
using Idera.SQLcompliance.Application.GUI.Properties ;
using Idera.SQLcompliance.Core.Rules.Alerts ;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
	/// <summary>
	/// Summary description for Form_StringSearch.
	/// </summary>
	public partial class Form_StringSearch : Form
	{
      private bool _forAlerts ;
      private int _fieldId;

		public Form_StringSearch(int fieldId, bool forAlerts)
		{
			InitializeComponent();
         this.Icon = Resources.SQLcompliance_product_ico;
         //_lblDirections.Text = String.Format("Specify a word or phrase to search for in {0}:", displayName) ;
         _rbInclude.Text = "&listed" ;
         _rbExclude.Text = "&except those listed" ;
         _forAlerts = forAlerts;
         _fieldId = fieldId;

         switch (fieldId)
         {
            case (int)AlertableEventFields.databaseName:
               this.Text = "Specify Databases" ;
               _lblDirections.Text = "&Specify database names to match (wildcards allowed):" ;
               _lblListDescription.Text = "&Database names to match:" ;
               _gbIncludeExclude.Text = "Match all database names" ;
               break ;

            case (int)AlertableEventFields.privilegedUsers:
               this.Text = "Specify Privileged Users";
               _lblDirections.Text = "&Specify Privileged Users names to match (wildcards allowed):";
               _lblListDescription.Text = "&Privileged Users to match:";
               _gbIncludeExclude.Text = "Match all Privileged Users";
               _cbNulls.Text = "Match Null Privileged Users";
               _cbBlanks.Text = "Match Empty or Blank Privileged Users";
               break;

            case (int)AlertableEventFields.objectName:
               this.Text = "Specify Database Objects" ;
               _lblDirections.Text = "&Specify database object names to match (wildcards allowed):" ;
               _lblListDescription.Text = "O&bject names to match:" ;
               _gbIncludeExclude.Text = "Match all database object names" ;
               break ;
            case (int)AlertableEventFields.applicationName:
            case (int)AlertableEventFields.dataRuleApplicationName:
               this.Text = "Specify Application Names" ;
               _lblDirections.Text = "&Specify application names to match (wildcards allowed):" ;
               _lblListDescription.Text = "&Application names to match:" ;
               _gbIncludeExclude.Text = "Match all application names" ;
               _cbNulls.Text = "Match Null Application Names";
               _cbBlanks.Text = "Match Empty or Blank Application Names";

               if (_forAlerts == false)
               {
                  _cbBlanks.Visible = true;
                  _cbNulls.Visible = true;
               }
               break ;
            case (int)AlertableEventFields.loginName:
            case (int)AlertableEventFields.dataRuleLoginName:
               this.Text = "Specify Login Names" ;
               _lblDirections.Text = "&Specify login names to match (wildcards allowed):" ;
               _lblListDescription.Text = "&Login names to match:" ;
               _gbIncludeExclude.Text = "Match all login names" ;
               break ;
            case (int)AlertableEventFields.hostName:
               this.Text = "Specify Hostnames";
               _lblDirections.Text = "&Specify hostnames to match (wildcards allowed):";
               _lblListDescription.Text = "&Hostnames to match:";
               _gbIncludeExclude.Text = "Match all hostnames";
               _cbNulls.Text = "Match Null hostnames";
               _cbBlanks.Text = "Match Empty or Blank hostnames";

               if (_forAlerts == false)
               {
                  _cbBlanks.Visible = true;
                  _cbNulls.Visible = true;
               }
               break;
         }

         //the include/exclude checkboxes are hidden for every string filter
         //except for the event filter, application name.
         if (forAlerts || (fieldId != (int)AlertableEventFields.applicationName && fieldId != (int)AlertableEventFields.hostName))
         {
            _btnOk.Location = new Point(_btnOk.Location.X, _btnOk.Location.Y - 35);
            _btnCancel.Location = new Point(_btnCancel.Location.X, _btnCancel.Location.Y - 35);
            this.Size = new Size(this.Size.Width, this.Size.Height - 35);
         }
	  }

		public string[] StringArray
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

		public bool Inclusive
		{
			get { return _rbInclude.Checked ; }
			set 
			{
				if(value)
				{
					_rbInclude.Checked = true ;
				}
				else
				{
					_rbExclude.Checked = true ;
            }
			}
		}

      public bool Blanks
      {
         get { return _cbBlanks.Checked; }
         set { _cbBlanks.Checked = value; }
      }

      public bool Nulls
      {
         get { return _cbNulls.Checked; }
         set { _cbNulls.Checked = value; }
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
      
      public string IncludeString
      {
         get { return _rbInclude.Text; }
         set { _rbInclude.Text = value ; }
      }

      public string ExcludeString
      {
         get { return _rbExclude.Text; }
         set { _rbExclude.Text = value ; }
      }


      #region Actions

      private void AddString()
      {
         if(!_listBoxStringList.Items.Contains(_tbStringEntry.Text))
         {
            _listBoxStringList.Items.Add(_tbStringEntry.Text) ;
            _tbStringEntry.Text = "" ;
         }
         else
         {
            MessageBox.Show(this, "The list already contains " + _tbStringEntry.Text + ".", "Warning") ;
         }
         _tbStringEntry.Focus() ;
      }

      private void RemoveSelectedString()
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

      #endregion // Actions

		private void Click_btnAdd(object sender, EventArgs e)
		{
         AddString() ;
		}

		private void Click_btnRemove(object sender, EventArgs e)
		{
         RemoveSelectedString() ;
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

      private void KeyDown_Form_StringSearch(object sender, KeyEventArgs e)
      {
         if(e.KeyCode == Keys.Enter)
         {
            if(_tbStringEntry.Focused && _tbStringEntry.Text.Length > 0)
               AddString() ;
            else
               _btnOk.PerformClick() ;
         }
         if(e.KeyCode == Keys.Delete)
         {
            if(_listBoxStringList.Focused && _listBoxStringList.SelectedIndex >= 0)
               RemoveSelectedString() ;
         }
      }

      private void Form_StringSearch_HelpRequested(object sender, HelpEventArgs hlpevent)
      {
         if(_forAlerts)
            HelpAlias.ShowHelp(this,HelpAlias.SSHELP_AlertWizard_StringSearch);      
         else
            HelpAlias.ShowHelp(this,HelpAlias.SSHELP_EventFilterWizard_StringSearch);      
         hlpevent.Handled = true ;
      }
	}
}
