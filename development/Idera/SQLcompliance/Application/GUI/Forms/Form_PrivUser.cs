using System ;
using System.Collections ;
using System.Linq;
using System.Windows.Forms ;
using Idera.SQLcompliance.Application.GUI.Controls ;
using Idera.SQLcompliance.Application.GUI.Helper ;
using Idera.SQLcompliance.Application.GUI.Properties ;
using Idera.SQLcompliance.Application.GUI.SQL ;
using Idera.SQLcompliance.Core ;
using Idera.SQLcompliance.Core.Agent ;
using Idera.SQLcompliance.Core.Collector ;
using Idera.SQLcompliance.Core.Remoting;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
	/// <summary>
	/// Summary description for Form_PrivUser.
	/// </summary>
	public partial class Form_PrivUser : Form
	{
      
      string instance;
        ServerRecord serverRecord;
        bool isDatabaseLvelPrivUserCall;

	   //-----------------------------------------------------------------------------
	   // Form_PrivUser (Constructor)
	   //-----------------------------------------------------------------------------
		public Form_PrivUser(string inInstance, bool privUser, bool isDatabaseLevelCall = false)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
         if (!privUser)
         {
            // Trusted users
            this.label3.Text = "Add to Trusted User List:";
            this.label4.Text = "Note: Specifying large numbers of trusted users may have a performance impact " +
                "on the audited SQL Server.";
            this.Text = "Add Trusted Users";
         }
		   this.Icon = Resources.SQLcompliance_product_ico;
         listAvailable.SmallImageList = AppIcons.AppImageList16();
         listSelected.SmallImageList = AppIcons.AppImageList16();
			
			instance = inInstance;
            serverRecord = new ServerRecord();
            isDatabaseLvelPrivUserCall = isDatabaseLevelCall;

        try
         {
            ServerRecord sr = new ServerRecord();
            sr.Connection = Globals.Repository.Connection;
            sr.Read(instance);
            serverRecord = sr;
            m_useAgentEnum = sr.IsDeployed && sr.IsRunning;
         }
         catch
         {
            m_useAgentEnum = false;
         }
		}

        #region OK / CANCEL Logic

        //-----------------------------------------------------------------------------
        // btnCancel_Click
        //-----------------------------------------------------------------------------
        private void btnCancel_Click(object sender, EventArgs e)
      {
         DialogResult = DialogResult.Cancel;
         this.Close();
      }

	   //-----------------------------------------------------------------------------
	   // btnOK_Click
	   //-----------------------------------------------------------------------------
      private void btnOK_Click(object sender, EventArgs e)
      {
         if ( listSelected.Items.Count != 0 )
         {
            DialogResult = DialogResult.OK;
         }
         else
         {
            DialogResult = DialogResult.Cancel;
         }
         this.Close();
      }
      
      #endregion
      
      #region Add & Remove Logic

	   //-----------------------------------------------------------------------------
	   // btnAdd_Click
	   //-----------------------------------------------------------------------------
      private void btnAdd_Click(object sender, EventArgs e)
      {
         AddSelected();
      }
      
	   //-----------------------------------------------------------------------------
	   // listAvailable_DoubleClick
	   //-----------------------------------------------------------------------------
      private void listAvailable_DoubleClick(object sender, EventArgs e)
      {
         if ( btnAdd.Enabled )
            AddSelected();
      }
      
	   //-----------------------------------------------------------------------------
	   // AddSelected
	   //-----------------------------------------------------------------------------
      private void AddSelected()
      {
         if ( listAvailable.SelectedItems.Count == 0 ) return;
      
         listSelected.BeginUpdate();
         listAvailable.BeginUpdate();
         
         int ndx = listAvailable.SelectedItems[0].Index;
         
         listSelected.SelectedItems.Clear();
         
         ListViewItem tmp;
         
         foreach ( ListViewItem itm in listAvailable.SelectedItems )
         {
            bool found = false;
            foreach ( ListViewItem s in listSelected.Items )
            {
               if ( itm.Text == s.Text )
               {
                  found = true;
                  s.Selected = true;
                  break;
               }
            }
            
            if ( ! found )
            {
               ListViewItem newItem = new ListViewItem( itm.Text );
               newItem.Tag        = itm.Tag;
               newItem.ImageIndex = itm.ImageIndex;

               tmp = listSelected.Items.Add( newItem );
               tmp.Selected = true;
            }
         }
         
         if (listAvailable.Items.Count != 0)
         {
            if ( ndx >= listAvailable.Items.Count )
            {
               listAvailable.Items[listAvailable.Items.Count-1].Selected = true;
            }
            else
            {
               listAvailable.Items[ndx].Selected = true;
            }
               
            listAvailable.Focus();
         }
         
         btnRemove.Enabled = ( listSelected.SelectedItems.Count != 0 );
         
         listAvailable.EndUpdate();
         listSelected.EndUpdate();
      }

	   //-----------------------------------------------------------------------------
	   // btnRemove_Click
	   //-----------------------------------------------------------------------------
      private void btnRemove_Click(object sender, EventArgs e)
      {
         RemoveSelected();
      }
      
	   //-----------------------------------------------------------------------------
	   // RemoveSelected
	   //-----------------------------------------------------------------------------
      private void RemoveSelected()
      {
         if ( listSelected.SelectedItems.Count == 0 ) return;
         
         listSelected.BeginUpdate();
         
         int ndx = listSelected.SelectedItems[0].Index;
         
         foreach ( ListViewItem itm in listSelected.SelectedItems )
         {
            itm.Remove();
         }
         
         if (listSelected.Items.Count != 0)
         {
            if ( ndx >= listSelected.Items.Count )
            {
               listSelected.Items[listSelected.Items.Count-1].Selected = true;
            }
            else
            {
               listSelected.Items[ndx].Selected = true;
            }
               
            listSelected.Focus();
            btnRemove.Enabled = true;
         }
         else
         {
            btnRemove.Enabled = false;
         }
         
         listSelected.EndUpdate();
      }

	   //-----------------------------------------------------------------------------
	   // listAvailable_SelectedIndexChanged
	   //-----------------------------------------------------------------------------
      private void listAvailable_SelectedIndexChanged(object sender, EventArgs e)
      {
         if ( listAvailable.SelectedItems.Count == 0 )
         {
            btnAdd.Enabled = false;
         }
         else
         {
            btnAdd.Enabled = true;
         }
      }

	   //-----------------------------------------------------------------------------
	   // listSelected_SelectedIndexChanged
	   //-----------------------------------------------------------------------------
      private void listSelected_SelectedIndexChanged(object sender, EventArgs e)
      {
         if ( listSelected.SelectedItems.Count == 0 )
         {
            btnRemove.Enabled   = false;
         }
         else
         {
            btnRemove.Enabled   = true;
         }
      }

      #endregion

      private void Form_PrivUser_Load(object sender, EventArgs e)
      {
         bool success ;
         
			// select combo box entry 1
         Cursor = Cursors.WaitCursor;
			comboDatabases.Sorted = true;

         success = true;
			
         Cursor = Cursors.Default;
         
			if ( success )
			{
			   comboDatabases.Sorted = false;
			   comboDatabases.Items.Insert(0, "Server Roles" );
			   comboDatabases.Items.Insert(1, "Server Logins" );
			   comboDatabases.SelectedIndex = 0;
			}
			else
			{
			   Close();
			}
      }
      
      private bool      m_useAgentEnum = true;
		public bool UseAgentEnumeration
		{
         get { return m_useAgentEnum; }
			set { m_useAgentEnum = value ; }
		}

      private void comboDatabases_SelectedIndexChanged(object sender, EventArgs e)
      {

            UserList serverPrivilegedUsers = new UserList(serverRecord.AuditUsersList);


            ICollection roleList  = null;
         ICollection loginList = null;
         SQLDirect sqlServer   = null;
         
         listAvailable.Items.Clear();
         btnAdd.Enabled = false;
      
         if ( m_useAgentEnum )
         {
             string url = EndPointUrlBuilder.GetUrl(typeof(AgentManager), Globals.SQLcomplianceConfig.Server, Globals.SQLcomplianceConfig.ServerPort);
            try
            {
                AgentManager manager = GUIRemoteObjectsProvider.AgentManager();
                  
               if ( comboDatabases.SelectedIndex == 0 )
                  roleList  = manager.GetRawServerRoles( instance );
               else if ( comboDatabases.SelectedIndex == 1 )
                  loginList = manager.GetRawServerLogins( instance );
               else
                  loginList = manager.GetRawLogins( instance,
                                                    comboDatabases.Text.Substring( 10 ) );
            }
            catch (Exception ex)
            {
               ErrorLog.Instance.Write( ErrorLog.Level.Verbose,
                                        String.Format( "LoadRoles or Logins: URL: {0} Instance {1}", url, instance ),
                                        ex,
                                        ErrorLog.Severity.Warning );
               roleList  = null;
               loginList = null;
               m_useAgentEnum = false;
            }
         }

         if ( roleList == null && loginList == null)
         {
             sqlServer = new SQLDirect();
             if ( sqlServer.OpenConnection( instance ) )
             {
                 if ( comboDatabases.SelectedIndex == 0 )
                     roleList  = RawSQL.GetServerRoles(sqlServer.Connection) ;
                 else if ( comboDatabases.SelectedIndex == 1 )
                     loginList = RawSQL.GetServerLogins(sqlServer.Connection);
                 else
                     loginList = RawSQL.GetLogins(sqlServer.Connection, comboDatabases.Text.Substring( 10 ) );
             }
         }   

 		 if ( roleList == null && loginList == null)
         {
            ErrorMessage.Show( this.Text,
                              UIConstants.Error_CantLoadRoles,
                              SQLDirect.GetLastError() );
            btnAdd.Enabled = false;
            Cursor = Cursors.Default;
            return;
         }
         
         if ( comboDatabases.SelectedIndex == 0 )
         {
            // server roles
   			if ((roleList != null) && (roleList.Count != 0)) 
	   		{
		   		foreach (RawRoleObject role in roleList) 
			   	{
                        if (isDatabaseLvelPrivUserCall)
                        {
                            if (!serverPrivilegedUsers.ServerRoles.Any(serverSr => serverSr.CompareName(role)))
                            {
                                ListViewItem x = listAvailable.Items.Add(role.fullName);
                                x.Tag = role.roleid;
                                x.ImageIndex = (int)AppIcons.Img16.Role;
                            }
                        }
                        else
                        {
                            ListViewItem x = listAvailable.Items.Add(role.fullName);
                            x.Tag = role.roleid;
                            x.ImageIndex = (int)AppIcons.Img16.Role;
                        }
				}
            }
         }
         else if ( comboDatabases.SelectedIndex == 1 )
         {
            // server logins
			   if ((loginList != null) && (loginList.Count != 0)) 
			   {
				   foreach (RawLoginObject login in loginList) 
				   {
                        if (isDatabaseLvelPrivUserCall)
                        {
                            if (!serverPrivilegedUsers.Logins.Any(serverLogin => serverLogin.Name == login.name))
                            {
                                ListViewItem x = listAvailable.Items.Add(login.name);
                                x.Tag = login.sid;
                                x.ImageIndex = (int)AppIcons.Img16.WindowsUser;
                            }
                        }
                        else
                        {
                            ListViewItem x = listAvailable.Items.Add(login.name);
                            x.Tag = login.sid;
                            x.ImageIndex = (int)AppIcons.Img16.WindowsUser;
                        }
               }
            }
         }
         
         if ( sqlServer != null )
         {
            sqlServer.CloseConnection();
         }
         
         if ( listAvailable.Items.Count != 0 )
         {
            listAvailable.Items[0].Selected = true;
            btnAdd.Enabled = true;
         }
         else
         {
            btnAdd.Enabled = false;
         }
         
         
         Cursor = Cursors.Default;
      }
      
      #region Help
      //--------------------------------------------------------------------
      // Form_PrivUser_HelpRequested - Show Context Sensitive Help
      //--------------------------------------------------------------------
      private void Form_PrivUser_HelpRequested(object sender, HelpEventArgs hlpevent)
      {
		   HelpAlias.ShowHelp(this,HelpAlias.SSHELP_Form_PrivUser);
			hlpevent.Handled = true;
      }
      #endregion
	}
}
