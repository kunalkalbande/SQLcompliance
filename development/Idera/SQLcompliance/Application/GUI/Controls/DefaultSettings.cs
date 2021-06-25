using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using Idera.SQLcompliance.Core;
using System.Data.SqlClient;
using System.Windows.Forms;
using Idera.SQLcompliance.Application.GUI.Helper;
using Idera.SQLcompliance.Core.Agent;

namespace Idera.SQLcompliance.Application.GUI.Controls
{
    public partial class DefaultSettings : BaseControl
    {
        List<string> serverList;
        List<int> serverId;
        CheckBox[] checkBoxesServer;
        CheckBox TopLevelNode;
        CheckBox selectAllServerCheckbox;
        AdminRibbonView adminRibbonView;
        DefaultAuditSettingnsTreeView treeView;
        ListView checkedDatabases;
        ListView checkedServers;

        TreeNode[] treeNodes;
        public DefaultSettings()
        {
            InitializeComponent();
           // GridHelper.ApplyAdminSettings(_defaultGrid);
            SetMenuFlag(CMMenuItem.DefaultServerSettings);
            SetMenuFlag(CMMenuItem.DefaultDatabaseSettings);
            SetMenuFlag(CMMenuItem.EditServerSettings);
            SetMenuFlag(CMMenuItem.EditDatabaseSettings);
            Resize += resize;
            serverList = new List<string>();
            serverId = new List<int>();
            GetServers();
            InitializeServersOnUI();
            tabControl1.SelectedIndexChanged += TabChanged;

            checkedDatabases = new ListView();
            checkedServers = new ListView();
            InitializeDatabasesOnUI();
        }

        //returning checked databases
        public ListView GetCheckedDatabases()
        {
            
            checkedDatabases.Items.Clear();
            if(treeView.Nodes.Count>0)
            {
                foreach(TreeNode node in treeView.Nodes)
                {
                    if(node.Nodes.Count>0)
                    {
                        foreach(TreeNode db in node.Nodes)
                        {
                            if (db.Checked)
                            {
                                ListViewItem itm = new ListViewItem(node.Text + "," + db.Text);
                                itm.Tag = node.Tag;
                                checkedDatabases.Items.Add(itm);
                            }
                        }
                    }
                }
            }
            return checkedDatabases;
        }

        //returning checked databases
        public ListView GetCheckedServers()
        {
            checkedServers.Items.Clear();

            if (serverList.Count > 0)
            {
                for (int i = 0; i < serverList.Count; i++)
                {
                    if (checkBoxesServer[i].Checked)
                    {
                        checkedServers.Items.Add(serverList[i]+","+serverId[i].ToString());
                    }
                }
            }
            
            return checkedServers;
        }
        private void TabChanged(object sender, EventArgs e)
        {
            if(tabControl1.SelectedIndex == 0)
            {
                UncheckAllDatabases();
            }
            else
            {
                ResetCheckBox();
            }
            //SQLCM-5758
            InitializeDatabasesOnUI();
        }

        private IEnumerable<TreeNode> GetEnumerableNodes(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                yield return node;
            }
        }

        private void AfterTreeNodeCheck(object sender, TreeViewEventArgs e)
        {
            if (treeView.Tag != null)
            {
                return;
            }

            treeView.Tag = 1;
            TreeNode node = e.Node;
            foreach (TreeNode nodes in node.Nodes)
            {
                nodes.Checked = node.Checked;
            }

            if (node.Parent != null)
            {
                if (GetEnumerableNodes(node.Parent.Nodes).All(chk => chk.Checked))
                {
                    node.Parent.Checked = true;
                }
                else if (GetEnumerableNodes(node.Parent.Nodes).All(chk => !chk.Checked))
                {
                    node.Parent.Checked = false;
                }
            }

            if (GetEnumerableNodes(treeView.Nodes).All(chk => chk.Checked))
            {
                TopLevelNode.Checked = true;
            }
            else if (GetEnumerableNodes(treeView.Nodes).All(chk => !chk.Checked))
            {
                TopLevelNode.Checked = false;
            }

            if (GetEnumerableNodes(treeView.Nodes)
                .SelectMany(n => GetEnumerableNodes(n.Nodes))
                .Any(n => n.Checked))
            {
                adminRibbonView.EnableApplyDefaultDatabaseSettingsButton();
            }
            else
            {
                adminRibbonView.DisableApplyDefaultDatabaseSettingsButton();
            }

            treeView.Tag = null;
        }

        public void ResetCheckBox()
        {
            if(serverList.Count >0)
            {
                for(int i=0;i<serverList.Count;i++)
                {
                    checkBoxesServer[i].Checked = false;
                }
            }
            selectAllServerCheckbox.Checked = false;
           
        }
        public void SetAdminRibbon(AdminRibbonView adminRibbonView)
        {
            this.adminRibbonView = adminRibbonView;
        }

        //Envoked when a server checkbox's state changes
        private void CheckboxCheckChanged(object sender,EventArgs e)
        {
            if (checkBoxesServer.All(chk => chk.Checked))
            {
                selectAllServerCheckbox.Checked = true;
            } else if (checkBoxesServer.All(chk => !chk.Checked))
            {
                selectAllServerCheckbox.Checked = false;
            }

            if (checkBoxesServer.Any(chk => chk.Checked))
            {
                adminRibbonView.EnableApplyDefaultServerSettingsButton();
            }
            else
            {
                adminRibbonView.DisableApplyDefaultServerSettingsButton();
            }
        }

        //Selecting/Deselecting all check boxes 
        private void CheckboxSelectAllChanged(object sender,EventArgs e)
        {
            if (serverList.Count > 0)
            {
                if (selectAllServerCheckbox.Checked)
                {

                    for (int i = 0; i < serverList.Count; i++)
                    {
                        checkBoxesServer[i].Checked = true;
                    }

                }
                else
                {
                    for (int i = 0; i < serverList.Count; i++)
                    {
                        checkBoxesServer[i].Checked = false;
                    }
                }
            }
        }

        //Initialize server checkboxes on Tab
        public void InitializeServersOnUI()
        {
            tabPage1.Controls.Clear();

            selectAllServerCheckbox = new CheckBox();
            selectAllServerCheckbox.BackColor = Color.LightGray;
            selectAllServerCheckbox.Text = "                       " + "Server";
            selectAllServerCheckbox.Width = Width;
            selectAllServerCheckbox.Location = new Point(0, 0);
            selectAllServerCheckbox.CheckedChanged += CheckboxSelectAllChanged;
            tabPage1.Controls.Add(selectAllServerCheckbox);
            if (serverList.Count > 0)
            {
                int  y = 20;
                checkBoxesServer = new CheckBox[serverList.Count];
                for(int i=0;i<serverList.Count;i++)
                {
                    checkBoxesServer[i] = new CheckBox();
                    checkBoxesServer[i].Text = "                       "+serverList[i];
                    checkBoxesServer[i].Width = Width;
                    checkBoxesServer[i].Location = new Point(0, y);
                    checkBoxesServer[i].CheckedChanged += CheckboxCheckChanged;
                    tabPage1.Controls.Add(checkBoxesServer[i]);
                    y = y + 20;
                }
            }
        }
        public void TopLevelNodeChecked(object sender, EventArgs e)
        {
            if(treeNodes!=null)
            {
                foreach(TreeNode node in treeNodes)
                {
                    node.Checked = TopLevelNode.Checked;
                }
            }
        }
        public void UncheckAllDatabases()
        {
            if(treeView!=null)
            {
                foreach(TreeNode node in treeView.Nodes)
                {
                    node.Checked = false;
                }
            }
            TopLevelNode.Checked = false;
        }
        public void InitializeDatabasesOnUI()
        {
            tabPage2.Controls.Clear();
            treeView = new DefaultAuditSettingnsTreeView();
            treeView.ShowPlusMinus = true;
            treeView.CheckBoxes = true;
            treeView.AfterCheck += AfterTreeNodeCheck;
            treeView.Height = tabControl1.Height*8/10;
            treeView.Width = Width;

            TopLevelNode = new CheckBox();
            TopLevelNode.Text = "Server/Database";
            TopLevelNode.Padding = new Padding(23, 0, 0, 0);
            TopLevelNode.Width = Width;
            TopLevelNode.BackColor = Color.LightGray;
            TopLevelNode.CheckedChanged += TopLevelNodeChecked;
            tabPage2.Controls.Add(TopLevelNode);
            treeView.Location = new Point(0, 30);
            
            string query = "";
            treeView.Width = Width;
            TreeNode treeNode;
            if(serverList.Count > 0)
            {
                
               treeNodes = new TreeNode[serverList.Count];
                
                for(int i=0;i<serverList.Count;i++)
                {
                    treeNodes[i] = new TreeNode(serverList[i]);
                    treeNodes[i].Tag = serverId[i];
                   query = string.Format("select name from {0}..{1} where srvId = {2}", CoreConstants.RepositoryDatabase, CoreConstants.RepositoryDatabaseTable,serverId[i]);
                    try
                    {
                        using (SqlCommand sqlCommand = new SqlCommand(query, Globals.Repository.Connection))
                        {
                            using (SqlDataReader reader = sqlCommand.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    treeNode = new TreeNode(reader[0].ToString());
                                    treeNodes[i].Nodes.Add(treeNode);
                                }
                            }

                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("Error reading database names from Databases table");
                    }
                    treeView.Nodes.Add(treeNodes[i]);
                }
                tabPage2.Controls.Add(treeView);
            }
        }
        public void resize(Object sender,EventArgs e)
        {
            tabControl1.Size = this.Size;
            treeView.Width = Width;
            treeView.Height = Height * 8/10;
      
        }
        private void tabPage1_Click(object sender, EventArgs e)
        {

        }
        public void GetServers()
        {
            serverList.Clear();
            serverId.Clear();
            string query = string.Format("select srvId,instance from {0}..{1}",CoreConstants.RepositoryDatabase,CoreConstants.RepositoryServerTable);
            try
            {
                using (SqlCommand sqlCommand = new SqlCommand(query, Globals.Repository.Connection))
                {
                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            serverId.Add(Convert.ToInt32 (  reader[0].ToString()));
                            serverList.Add(reader[1].ToString());
                        }
                    }

                }
            }
            catch(Exception e)
            {
                MessageBox.Show("Error reading server names from Servers table");
            }
        }

        //Don't remove this class. It fixes the inconsistent behaviour of the treeview.
        public class DefaultAuditSettingnsTreeView : TreeView
        {
            protected override void WndProc(ref Message m)
            {
                // Suppress WM_LBUTTONDBLCLK
                if (m.Msg == 0x203) { m.Result = IntPtr.Zero; }
                else base.WndProc(ref m);
            }
        }
    }
}
