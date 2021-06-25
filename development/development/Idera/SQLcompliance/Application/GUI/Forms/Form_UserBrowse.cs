using System ;
using System.Collections ;
using System.DirectoryServices ;
using System.Windows.Forms ;
using Idera.SQLcompliance.Application.GUI.Properties ;
using Idera.SQLcompliance.Application.GUI.SQL ;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
	/// <summary>
	/// Summary description for Form_UserBrowse.
	/// </summary>
	public partial class Form_UserBrowse : Form
	{
      public string userName = "";

		public Form_UserBrowse()
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

      protected override void OnLoad(EventArgs e)
      {
         Cursor = Cursors.WaitCursor;

         ICollection domains = Globals.Repository.GetDomains();
         foreach (string d in domains )
         {
            comboDomains.Items.Add(d);
         }
         
         Cursor = Cursors.Default;
         
         base.OnLoad (e);
      }

      private void comboDomains_SelectedIndexChanged(object sender, EventArgs e)
      {
         Cursor = Cursors.WaitCursor;
         listUsers.BeginUpdate();
         
         listUsers.Items.Clear();
         listUsers.Items.Add( String.Format( "User:{0}", comboDomains.Text) );
         LoadUsers();
         
         listUsers.EndUpdate();
         Cursor = Cursors.Default;
      }

      private void btnOK_Click(object sender, EventArgs e)
      {
         if ( listUsers.SelectedItems.Count != 0 )
         {
            userName = String.Format( @"{0}\{1}",
                                    comboDomains.Text,
                                    listUsers.SelectedItems[0].Text );
            DialogResult = DialogResult.OK;
            Close();
         }
      }

      private void btnCancel_Click(object sender, EventArgs e)
      {
         DialogResult = DialogResult.Cancel;
         Close();
      }

      private void listUsers_SelectedIndexChanged(object sender, EventArgs e)
      {
         if ( listUsers.SelectedItems.Count == 0 )
         {
            btnOK.Enabled = false;
         }
         else
         {
            btnOK.Enabled = true;
         }
      }
      
		private void
		   LoadUsers()
		{
			string strPath = String.Format( "LDAP://dc={0}", comboDomains.Text );

			DirectoryEntry entry ;

			try
			{
			   entry = new DirectoryEntry(strPath);

			   DirectorySearcher mySearcher = new DirectorySearcher(entry);
    
			   string strCat = "(objectCategory=users)";
            mySearcher.Filter = strCat;

			   string strName;
			   
				foreach(SearchResult result in 	mySearcher.FindAll()) 
				{
					strName = result.GetDirectoryEntry().Name;
					strName = strName.Remove(0, 3); //delete "CN="
					listUsers.Items.Add(strName);
				}		
			}
			catch (Exception ex)
			{
				MessageBox.Show( String.Format( "An error occurred enumerating the users in domain {0}.\n\nError:\n\n{1}",
				                                comboDomains.Text,
				                                ex.Message ));
			}
		}
      
	}
}
