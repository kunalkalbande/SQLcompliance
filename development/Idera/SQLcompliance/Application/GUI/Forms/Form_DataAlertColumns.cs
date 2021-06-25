using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Idera.SQLcompliance.Application.GUI.Controls;
using Idera.SQLcompliance.Application.GUI.Helper;
using Idera.SQLcompliance.Application.GUI.SQL;
using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Agent;
using Idera.SQLcompliance.Core.Collector;
using Idera.SQLcompliance.Core.Remoting;
using Idera.SQLcompliance.Core.Rules.Alerts;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
   public enum Selection
   {
      Instance,
      Database,
      Table,
      Column
   }

   public partial class Form_DataAlertColumns : Form
   {
      private DataRuleType _dataRuleType;
      private Selection _selection;
      private DataAlertRule _rule;
      private string _instance;
      private string _database;
      private string _table;
      private string _column;
      private bool _BADError;
       //SQLCM -5470 v5.6
      protected ArrayList m_coll;
      protected TreeNode m_lastNode, m_firstNode;
       //SQLCM -5470 v5.6- END

      public string Instance
      {
         get { return _instance; }
      }

      public string Database
      {
         get { return _database; }
      }
      
      public string Table
      {
         get { return _table; }
      }

      public string Column
      {
         get { return _column; }
      }

       //SQLCM -5470 v5.6
      public ArrayList SelectedNodes
      {
          get
          {
              return m_coll;
          }
          set
          {
              removePaintFromNodes();
              m_coll.Clear();
              m_coll = value;
              paintSelectedNodes();
          }
      }
      //SQLCM -5470 v5.6 - END

      public Form_DataAlertColumns(Selection selection, DataRuleType dataRuleType, DataAlertRule rule)
      {
         InitializeComponent();
         _dataRuleType = dataRuleType;
         _selection = selection;
         _rule = rule;
         _BADError = false;
         m_coll = new ArrayList();  //SQLCM -5470 v5.6
         Initialize();
      }

      private void Initialize()
      {
         InitTreeView();

         if (availableColumns.Nodes[0] != null && availableColumns.Nodes[0].Nodes.Count == 0)
         {
            DisplayError(_dataRuleType, false, null, null);
         }
         else
         {
            if (!_BADError)
            {
               notConfiguredPanel.Visible = false;
               notConfiguredPanel.SendToBack();

               switch (_selection)
               {
                  case Selection.Instance:
                     this.Text = "Select An Instance";

                     if (_dataRuleType == DataRuleType.SensitiveColumn || _dataRuleType == DataRuleType.SensitiveColumnViaDataset)   //SQLCM -5470 v5.6
                        descriptionText.Text = "Choose the instance for which you want to alert on sensitive column access.";
                     else
                        descriptionText.Text = "Select an instance";
                     break;
                  case Selection.Database:
                     this.Text = "Select A Database";

                     if (_dataRuleType == DataRuleType.SensitiveColumn || _dataRuleType == DataRuleType.SensitiveColumnViaDataset)  //SQLCM -5470 v5.6
                        descriptionText.Text = "Choose the database for which you want to alert on sensitive column access..";
                     else
                        descriptionText.Text = "Select a database";
                     break;
                  case Selection.Table:
                     this.Text = "Select A Table";

                     if (_dataRuleType == DataRuleType.SensitiveColumn || _dataRuleType == DataRuleType.SensitiveColumnViaDataset)  //SQLCM -5470 v5.6
                        descriptionText.Text = "Choose the table for which you want to alert on sensitive column access.";
                     else
                        descriptionText.Text = "Select a table";
                     break;
                  case Selection.Column:
                     this.Text = "Select A Column";

                     if (_dataRuleType == DataRuleType.SensitiveColumn)  //SQLCM -5470 v5.6
                        descriptionText.Text = "Choose the column for which you want to alert on sensitive column access.";
                     else if(_dataRuleType == DataRuleType.SensitiveColumnViaDataset)
                         descriptionText.Text = "Choose the column(s) for which you want to alert on sensitive column access. (Ctrl or Shft can be used for multiple column selections, Ctrl can be used for column deselection).";
                     else
                        descriptionText.Text = "Select a column";
                     break;
               }
            }
         }
      }

      private void DisplayError(DataRuleType type, bool columnMessage, string instance, string database)
      {
         switch (_dataRuleType)
         {
            case DataRuleType.ColumnValueChangedBad:
            case DataRuleType.ColumnValueChanged:
               if (columnMessage)
                  notConfiguredText.Text = String.Format("No numeric columns have been setup for Before-After Data Auditing on: \r\n\r\nInstance - {0} \r\nDatabase - {1}.  \r\n\r\nGo to the Audited Database Properties Dialog to enable this feature.", instance, database);
               else
                  notConfiguredText.Text = "Before-After Data Auditing has not been enabled on any of the audited instances.  Go to the Audited Database Properties Dialog to enable this feature.";
               this.Text = "Before-After Data Auditing Not Enabled";
               break;
            case DataRuleType.SensitiveColumn:
            case DataRuleType.SensitiveColumnViaDataset:   //SQLCM -5470 v5.6
               notConfiguredText.Text = "Sensitive Column auditing has not been enabled on any of the audited instances.  Go to the Audited Database Properties Dialog to enable this feature.";
               this.Text = "Sensitive Column Auditing Not Enabled";
               break;
         }
         notConfiguredPanel.BringToFront();
         notConfiguredPanel.Visible = true;
         descriptionText.Visible = false;
      }

      private void InitTreeView()
      {
         bool serverAdded;
         int imgIndex;
         SQLcomplianceTreeNode root;
         SQLcomplianceTreeNode server = null;
         availableColumns.Nodes.Clear();
         availableColumns.Sorted = true;

         imgIndex = (int)AppIcons.Img16.Server;
         root = new SQLcomplianceTreeNode("Audited SQL Servers", imgIndex, imgIndex, CMNodeType.AuditByServerRoot);
         root.Tag = "All";
         root.SetMenuFlag(CMMenuItem.Refresh);
         root.SetMenuFlag(CMMenuItem.AttachArchive);
         root.SetMenuFlag(CMMenuItem.NewServer);
         root.SetMenuFlag(CMMenuItem.NewDatabase);

         availableColumns.Nodes.Add(root);
         foreach (ServerRecord serverRecord in ServerRecord.GetServers(Globals.Repository.Connection, false, true))
         {
            if (serverRecord.IsEnabled && serverRecord.IsAuditedServer)
            {
               serverAdded = false;

               foreach (DatabaseRecord dbRecord in DatabaseRecord.GetDatabases(Globals.Repository.Connection, serverRecord.SrvId))
               {
                  if (dbRecord.AuditDataChanges || dbRecord.AuditSensitiveColumns)
                  {
                     //they want Column value changed but are not auditing BAD
                      if ((_dataRuleType == DataRuleType.ColumnValueChanged || _dataRuleType == DataRuleType.ColumnValueChangedBad) && 
                          !dbRecord.AuditDataChanges)
                        continue;

                     //they want Sensitive Column accessed. but are not auditing Sensitive Columns
                      if ((_dataRuleType == DataRuleType.SensitiveColumn || _dataRuleType == DataRuleType.SensitiveColumnViaDataset)    //SQLCM -5470 v5.6
                          && !dbRecord.AuditSensitiveColumns)
                        continue;

                     if (!serverAdded)
                     {
                        server = AddServerNode(serverRecord);
                        serverAdded = true;
                     }
                     //skip adding the database.
                     if (_selection == Selection.Instance)
                        break;

                     SQLcomplianceTreeNode db = AddDatabaseNode(dbRecord);

                     //skip adding tables
                     if (_selection == Selection.Database)
                        continue;

                     AddTables(serverRecord.Instance, db, serverRecord.SrvId, dbRecord);
                  }
               }
            }
         }
         root.Expand();
         availableColumns.SelectedNode = root;
         
      }

      private void AddTables(string instance, SQLcomplianceTreeNode db, int serverId, DatabaseRecord dbRecord)
      {
         switch (_dataRuleType)
         {
            case DataRuleType.ColumnValueChanged:
            case DataRuleType.ColumnValueChangedBad:
               foreach (DataChangeTableRecord dcTable in LoadDCTables(serverId, dbRecord.DbId))
               {
                  SQLcomplianceTreeNode tblNode;

                  if (!String.IsNullOrEmpty(dcTable.SchemaName))
                  {
                     tblNode = AddTableNode(dcTable.FullTableName, db);
                  }
                  else
                  {
                     tblNode = AddTableNode(dcTable.TableName, db);
                  }

                  //skip adding the columns
                  if (_selection == Selection.Table)
                     continue;

                  List<string> columns = LoadColumns(instance, dbRecord.Name, dcTable);

                  if (columns.Count > 0)
                  {
                     foreach (string column in columns)
                     {
                        AddColumnNode(column, tblNode);
                     }
                  }
                  else
                  {
                     _BADError = true;
                     DisplayError(DataRuleType.ColumnValueChanged, true, instance, dbRecord.Name);
                  }
               }
               break;

            case DataRuleType.SensitiveColumn:
                 List<SensitiveColumnTableRecord> lstSCTableRecords = LoadSCTables(serverId, dbRecord.DbId);
                 if (lstSCTableRecords != null)
                 {                     
                     foreach (SensitiveColumnTableRecord scTable in lstSCTableRecords)
                     {
                         SQLcomplianceTreeNode tblNode;
                         if (scTable.Type != "Dataset")
                         {
                             if (!String.IsNullOrEmpty(scTable.SchemaName))
                             {
                                 tblNode = AddTableNode(scTable.FullTableName, db);
                             }
                             else
                             {
                                 tblNode = AddTableNode(scTable.TableName, db);
                             }

                             if (_selection == Selection.Table)
                                 continue;

                             foreach (string column in LoadColumns(scTable))
                             {
                                 AddColumnNode(column, tblNode);
                             }
                         }                                                  
                     }
                 }
               break;
            //SQLCM -5470 v5.6
            case DataRuleType.SensitiveColumnViaDataset:
               List<SensitiveColumnTableRecord> lstSCTableRecords1 = LoadSCTables(serverId, dbRecord.DbId);
               if (lstSCTableRecords1 != null)
               {
                   foreach (SensitiveColumnTableRecord scTable in lstSCTableRecords1)
                   {
                       SQLcomplianceTreeNode tblNode;
                       if (scTable.Type == "Dataset")
                       {
                           if (!String.IsNullOrEmpty(scTable.SchemaName))
                           {
                               foreach (var item in scTable.FullTableName.Split(','))
                               {
                                   tblNode = AddTableNode(item, db);

                                   if (_selection == Selection.Table)
                                       continue;

                                   foreach (string column in LoadColumns(scTable))
                                   {
                                       if (column.StartsWith(item + "."))
                                       {
                                           AddColumnNode(column.Replace(item + ".", ""), tblNode);
                                       }
                                   }
                               }
                           }
                           else
                           {
                               foreach (var item in scTable.TableName.Split(','))
                               {
                                   tblNode = AddTableNode(item, db);

                                   if (_selection == Selection.Table)
                                       continue;

                                   foreach (string column in LoadColumns(scTable))
                                   {
                                       if (column.StartsWith(item + "."))
                                       {
                                           AddColumnNode(column.Replace(item + ".", ""), tblNode);
                                       }
                                   }
                               }
                           }
                       }
                   }
               }
               break;
         }
      }

      private List<DataChangeTableRecord> LoadDCTables(int serverId, int dbId)
      {         
         return DataChangeTableRecord.GetAuditedTables(Globals.Repository.Connection, serverId, dbId);
      }

      private List<SensitiveColumnTableRecord> LoadSCTables(int serverId, int dbId)
      {
         return SensitiveColumnTableRecord.GetAuditedTables(Globals.Repository.Connection, serverId, dbId);
      }

      private List<string> LoadColumns(string instance, string database, DataChangeTableRecord dcTable)
      {
         List<string> columns = new List<string>();

         if (dcTable.SelectedColumns)
         {
            foreach (RawColumnObject column in LoadRawColumns(instance, database, dcTable.FullTableName))
            {
                if (_dataRuleType == DataRuleType.ColumnValueChanged && 
                    ColumnNumeric(column.Type) &&
                    ColumnBadEnabled(column.ColumnName, dcTable.Columns))
                {
                    columns.Add(column.ColumnName);
                }
                else if (_dataRuleType == DataRuleType.ColumnValueChangedBad &&
                    ColumnBadEnabled(column.ColumnName, dcTable.Columns))
                {
                    columns.Add(column.ColumnName);
                }
            }
         }
         else
         {
            columns.Add("All Columns");
         }
         return columns;
      }

      private bool ColumnNumeric(int type)
      {
         switch (type)
         {
            //these can be found in the systypes (2000) or sys.types (2005+) table
            case 48: //tinyint
            case 52: //smallint
            case 56: //int
            case 59: //real
            case 60: //money
            case 62: //float
            case 106: //decimal
            case 108: //numeric
            case 122: //smallmoney
            case 127: //bigint
               return true;
            default:
               return false;
         };
      }

      private bool ColumnBadEnabled(string columnName, string[] badColumns)
      {
         foreach (string column in badColumns)
         {
            if (String.Equals(column, columnName))
               return true;
         }
         return false;
      }


      private IList LoadRawColumns(string instance, string database, string tableName)
      {
         IList columnList = null;
         // Load list of columns for the table
         // try via connection to agent
         string url = EndPointUrlBuilder.GetUrl(typeof(AgentManager), Globals.SQLcomplianceConfig.Server, Globals.SQLcomplianceConfig.ServerPort);
         try
         {
            AgentManager manager = GUIRemoteObjectsProvider.AgentManager();

            columnList = manager.GetRawColumns(instance, database, tableName);
         }
         catch (Exception ex)
         {
            ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                    String.Format("LoadColumns: URL: {0} Instance {1} Database {2} Table {3}", url, instance, database, tableName),
                                    ex,
                                    ErrorLog.Severity.Warning);
            columnList = null;
         }

         // straight connection to SQL Server
         if (columnList == null)
         {
            SQLDirect sqlServer = new SQLDirect();
            try
            {
               if (sqlServer.OpenConnection(instance))
               {
                  columnList = RawSQL.GetColumns(sqlServer.Connection, database, tableName);
               }
            }
            finally
            {
               sqlServer.CloseConnection();
            }
         }
         return columnList;
      }

      private string[] LoadColumns(SensitiveColumnTableRecord scTable)
      {
         string[] columns;

         if (scTable.SelectedColumns)
            columns = scTable.Columns;
         else
         {
            columns = new string[1];
            columns[0] = "All Columns";
         }
         return columns;
      }

      private SQLcomplianceTreeNode AddServerNode(ServerRecord server)
      {
         SQLcomplianceTreeNode root = availableColumns.Nodes[0] as SQLcomplianceTreeNode;
         if (root == null)
            return null;
         SQLcomplianceTreeNode newNode = FindNode(server.Instance, root.Nodes);
         if (newNode != null)
         {
            RefreshServerNode(server);
         }
         else
         {
            int imgIndex;
            if (server.IsAuditedServer)
               imgIndex = server.IsEnabled ? (int)AppIcons.Img16.Server : (int)AppIcons.Img16.ServerDisabled;
            else
               imgIndex = (int)AppIcons.Img16.ReportServer;

            newNode = new SQLcomplianceTreeNode(server.Instance, imgIndex, imgIndex, CMNodeType.Server);
            newNode.Tag = server;
            //SetServerNodeMenuFlags(newNode, server);
            root.Nodes.Add(newNode);
         }
         if (server.Instance == _rule.Instance)
            newNode.Expand();
         root.Expand();
         return newNode;
      }

      private SQLcomplianceTreeNode AddDatabaseNode(DatabaseRecord database)
      {
         SQLcomplianceTreeNode root = availableColumns.Nodes[0] as SQLcomplianceTreeNode;
         if (root == null)
            return null;

         SQLcomplianceTreeNode parent = FindNode(database.SrvInstance, root.Nodes);
         if (parent == null)
            return null;
         ServerRecord server = (ServerRecord)parent.Tag;

         int imgIndex = server.IsEnabled && database.IsEnabled ?
            (int)AppIcons.Img16.Database : (int)AppIcons.Img16.DatabaseDisabled;
         SQLcomplianceTreeNode node = new SQLcomplianceTreeNode(database.Name, imgIndex, imgIndex, CMNodeType.Database);
         //SetDatabaseNodeMenuFlags(node, database);
         node.Tag = database;
         parent.Nodes.Add(node);

         if (database.Name == _rule.Database)
            node.Expand();
         return node;
      }

      private SQLcomplianceTreeNode AddTableNode(string tableName, SQLcomplianceTreeNode parent)
      {
          foreach (SQLcomplianceTreeNode tableNode in parent.Nodes)
          {
              if (tableNode.Text == tableName)
                  return tableNode;
          }
         int imgIndex = (int)AppIcons.Img16.Database;
         SQLcomplianceTreeNode node = new SQLcomplianceTreeNode(tableName, imgIndex, imgIndex, CMNodeType.Database);
         parent.Nodes.Add(node);

         if (tableName == _rule.FullTableName)
            node.Expand();
         return node;
      }

      private void AddColumnNode(string column, SQLcomplianceTreeNode parent)
      {
          int index = 0;
          while (index < parent.Nodes.Count)
          {
              if (parent.Nodes[index].Text == "All Columns"){
                  parent.Nodes.RemoveAt(index);
                  continue;
              }
              if (parent.Nodes[index].Text == column)
                  return;
              index++;
          }
         int imgIndex = (int)AppIcons.Img16.Database;
         SQLcomplianceTreeNode node = new SQLcomplianceTreeNode(column, imgIndex, imgIndex, CMNodeType.Database);
         parent.Nodes.Add(node);
      }

      private SQLcomplianceTreeNode FindNode(string name, TreeNodeCollection nodes)
      {
         foreach (TreeNode node in nodes)
         {
            if (String.Compare(node.Text, name) == 0)
               return node as SQLcomplianceTreeNode;
         }
         return null;
      }


      //------------------------------------------------------------------
      // RefreshServerNode
      //------------------------------------------------------------------
      public void RefreshServerNode(ServerRecord server)
      {
         SQLcomplianceTreeNode root = availableColumns.Nodes[0] as SQLcomplianceTreeNode;
         if (root == null)
            return;

         SQLcomplianceTreeNode node = FindNode(server.Instance, root.Nodes);
         if (node != null)
         {
            int imgIndex;
            if (server.IsAuditedServer)
               imgIndex = server.IsEnabled ? (int)AppIcons.Img16.Server : (int)AppIcons.Img16.ServerDisabled;
            else
            {
               imgIndex = (int)AppIcons.Img16.ReportServer;
               node.Nodes.Clear();
            }
            node.ImageIndex = imgIndex;
            node.SelectedImageIndex = imgIndex;
            node.Tag = server;
            foreach (TreeNode childNode in node.Nodes)
            {
               DatabaseRecord db = childNode.Tag as DatabaseRecord;
               childNode.ImageIndex = server.IsEnabled && db.IsEnabled ? (int)AppIcons.Img16.Database : (int)AppIcons.Img16.DatabaseDisabled;
               childNode.SelectedImageIndex = childNode.ImageIndex;
            }
            //SetServerNodeMenuFlags(node, server);
            if (node.IsSelected)
            {
               //_ribbonTabView.SetScope(server);
               //_ribbonTabView.RefreshView();
            }
         }
      }

      private void buttonOK_Click(object sender, EventArgs e)
      {
          if (SetSelectedInfo())
          {
              DialogResult = DialogResult.OK;
              Close();
          }
      }

      private bool SetSelectedInfo()
      {
         //walk the tree backward to get the selected information
         TreeNode node;
         bool isSelected = true;
         switch (_selection)
         {
            case Selection.Instance:
               _instance = ServerTreeNodeHelper.GetInstanceNameFromServerTreeNode(availableColumns.SelectedNode);
               break;
            case Selection.Database:
               _database = availableColumns.SelectedNode.Text;
               node = availableColumns.SelectedNode.Parent;
               _instance = ServerTreeNodeHelper.GetInstanceNameFromServerTreeNode(node);
               break;
            case Selection.Table:
               if (_dataRuleType == DataRuleType.SensitiveColumn
                 || _dataRuleType == DataRuleType.ColumnValueChanged
                 || _dataRuleType == DataRuleType.ColumnValueChangedBad)
               {
                   _table = availableColumns.SelectedNode.Text;
                   node = availableColumns.SelectedNode.Parent;
                   _database = node.Text;
                   node = node.Parent;
                   _instance = ServerTreeNodeHelper.GetInstanceNameFromServerTreeNode(node);
               }
               else
               {
                   if (m_coll.Count > 0)
                   {
                       _column = null;
                       _table = null;
                       _database = null;
                       _instance = null;
                       foreach (TreeNode tnode in m_coll)
                       {
                           if (_instance == null)
                           {
                               _instance += ServerTreeNodeHelper.GetInstanceNameFromServerTreeNode(tnode.Parent.Parent);
                           }
                           else if (!_instance.Contains(tnode.Parent.Parent.Text))
                           {
                               MessageBox.Show(this, "Please select tables from same Server instance.", "Information");
                               //_instance += ServerTreeNodeHelper.GetInstanceNameFromServerTreeNode(tnode.Parent.Parent.Parent);
                               isSelected = false;
                           }
                           if (isSelected)
                           {
                               if (_table == null)
                                   _table += tnode.Text + ",";
                               else if (!_table.Contains(tnode.Text))
                                   _table += tnode.Text + ",";

                               if (_database == null)
                                   _database += tnode.Parent.Text + ",";
                               else if (!_database.Contains(tnode.Parent.Text))
                                   _database += tnode.Parent.Text + ",";

                           }
                       }
                       if (!string.IsNullOrEmpty(_table) && _table.LastIndexOf(',') > -1)
                           _table = _table.Substring(0, _table.LastIndexOf(','));
                       if (!string.IsNullOrEmpty(_database) && _database.LastIndexOf(',') > -1)
                           _database = _database.Substring(0, _database.LastIndexOf(','));
                   }
               }
               break;
            case Selection.Column:
               //SQLCM -5470 v5.6
               if (_dataRuleType == DataRuleType.SensitiveColumn 
                   || _dataRuleType == DataRuleType.ColumnValueChanged 
                   || _dataRuleType == DataRuleType.ColumnValueChangedBad )
               {
                   _column = availableColumns.SelectedNode.Text;
                   node = availableColumns.SelectedNode.Parent;
                   _table = node.Text;
                   node = node.Parent;
                   _database = node.Text;
                   node = node.Parent;
                   _instance = ServerTreeNodeHelper.GetInstanceNameFromServerTreeNode(node);
               }
               else if (_dataRuleType == DataRuleType.SensitiveColumnViaDataset)
               {
                   if (m_coll.Count > 0)
                   {
                       _column = null;
                       _table = null;
                       _database = null;
                       _instance = null;
                       foreach (TreeNode tnode in m_coll)
                       {
                           if (_instance == null)
                           {
                               _instance += ServerTreeNodeHelper.GetInstanceNameFromServerTreeNode(tnode.Parent.Parent.Parent);
                           }
                           else if (!_instance.Contains(tnode.Parent.Parent.Parent.Text))
                           {
                               MessageBox.Show(this, "Please select tables from same Server instance.", "Information");
                               //_instance += ServerTreeNodeHelper.GetInstanceNameFromServerTreeNode(tnode.Parent.Parent.Parent);
                               isSelected = false;
                           }
                           if(isSelected)
                           {
                               _column += tnode.Text + ", ";
                               if (_table == null)
                                   _table += tnode.Parent.Text + ",";
                               else if (!_table.Contains(tnode.Parent.Text))
                                   _table += tnode.Parent.Text + ",";

                               if (_database == null)
                                   _database += tnode.Parent.Parent.Text + ",";
                               else if (!_database.Contains(tnode.Parent.Parent.Text))
                                   _database += tnode.Parent.Parent.Text + ",";

                           }
                           
                       }
                       if(!string.IsNullOrEmpty(_column) &&  _column.LastIndexOf(',') > -1)
                        _column = _column.Substring(0, _column.LastIndexOf(','));
                       if (!string.IsNullOrEmpty(_table) && _table.LastIndexOf(',') > -1)
                           _table = _table.Substring(0, _table.LastIndexOf(','));
                       if (!string.IsNullOrEmpty(_database) && _database.LastIndexOf(',') > -1)
                           _database = _database.Substring(0, _database.LastIndexOf(','));
                       if (!string.IsNullOrEmpty(_instance) && _instance.LastIndexOf(',') > -1)
                           _instance = _instance.Substring(0, _instance.LastIndexOf(','));
                     
                       
                   }
               }
                 //SQLCM -5470 v5.6 - END
              
               break;
         }
         return isSelected;
      }

      private void buttonCancel_Click(object sender, EventArgs e)
      {
         DialogResult = DialogResult.Cancel;
         Close();
      }

      //SQLCM -5470 v5.6
      protected void availableColumns_BeforeSelect(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
      {
          bool bControl = (ModifierKeys == Keys.Control);
          bool bShift = (ModifierKeys == Keys.Shift);

          // selecting twice the node while pressing CTRL ?
          if (bControl && m_coll.Contains(e.Node))
          {
              // unselect it (let framework know we don't want selection this time)
              e.Cancel = true;

              // update nodes
              removePaintFromNodes();
              m_coll.Remove(e.Node);
              paintSelectedNodes();
              return;
          }

          m_lastNode = e.Node;
          if (!bShift) m_firstNode = e.Node; // store begin of shift sequence
      }
      //SQLCM -5470 v5.6 - END
      private void availableColumns_AfterSelect(object sender, TreeViewEventArgs e)
      {
         TreeNode node = ((TreeView)sender).SelectedNode;
         //SQLCM -5470 v5.6
         if (_dataRuleType == DataRuleType.SensitiveColumnViaDataset)
         {
             if (node.Nodes.Count > 0)
             {
                 availableColumns.SelectedNode = null;
                 buttonOK.Enabled = false;
             }
             else
                 buttonOK.Enabled = true;

             bool bControl = (ModifierKeys == Keys.Control);
             bool bShift = (ModifierKeys == Keys.Shift);

             if (bControl)
             {
                 if (!m_coll.Contains(e.Node)) // new node ?
                 {
                     m_coll.Add(e.Node);
                 }
                 else  // not new, remove it from the collection
                 {
                     removePaintFromNodes();
                     m_coll.Remove(e.Node);
                 }
                 paintSelectedNodes();
             }
             else
             {
                 if (bShift)
                 {
                     Queue myQueue = new Queue();

                     TreeNode uppernode = m_firstNode;
                     TreeNode bottomnode = e.Node;
                     // case 1 : begin and end nodes are parent
                     bool bParent = isParent(m_firstNode, e.Node); // is m_firstNode parent (direct or not) of e.Node
                     if (!bParent)
                     {
                         bParent = isParent(bottomnode, uppernode);
                         if (bParent) // swap nodes
                         {
                             TreeNode t = uppernode;
                             uppernode = bottomnode;
                             bottomnode = t;
                         }
                     }
                     if (bParent)
                     {
                         TreeNode n = bottomnode;
                         while (n != uppernode.Parent)
                         {
                             if (!m_coll.Contains(n)) // new node ?
                                 myQueue.Enqueue(n);

                             n = n.Parent;
                         }
                     }
                     else
                     {
                         if ((uppernode.Parent == null && bottomnode.Parent == null) || (uppernode.Parent != null && uppernode.Parent.Nodes.Contains(bottomnode))) // are they siblings ?
                         {
                             int nIndexUpper = uppernode.Index;
                             int nIndexBottom = bottomnode.Index;
                             if (nIndexBottom < nIndexUpper) // reversed?
                             {
                                 TreeNode t = uppernode;
                                 uppernode = bottomnode;
                                 bottomnode = t;
                                 nIndexUpper = uppernode.Index;
                                 nIndexBottom = bottomnode.Index;
                             }

                             TreeNode n = uppernode;
                             while (nIndexUpper <= nIndexBottom)
                             {
                                 if (!m_coll.Contains(n)) // new node ?
                                     myQueue.Enqueue(n);

                                 n = n.NextNode;

                                 nIndexUpper++;
                             } // end while

                         }
                         else
                         {
                             if (!m_coll.Contains(uppernode)) myQueue.Enqueue(uppernode);
                             if (!m_coll.Contains(bottomnode)) myQueue.Enqueue(bottomnode);
                         }
                     }

                     m_coll.AddRange(myQueue);

                     paintSelectedNodes();
                     m_firstNode = e.Node; // let us chain several SHIFTs if we like its
                 }
                 else
                 {
                     // in the case of a simple click, just add this item
                     if (m_coll != null && m_coll.Count > 0)
                     {
                         removePaintFromNodes();
                         m_coll.Clear();
                     }
                     m_coll.Add(e.Node);
                 }
             }
         }       //SQLCM -5470 v5.6 - END
         else
         {
             //only allow the leaf node to be selected.
             if (node.Nodes.Count > 0)
             {
                 availableColumns.SelectedNode = null;
                 buttonOK.Enabled = false;
             }
             else
                 buttonOK.Enabled = true;
         }
      }
      //SQLCM -5470 v5.6
      protected void paintSelectedNodes()
      {
          foreach (TreeNode n in m_coll)
          {
              n.BackColor = SystemColors.Highlight;
              n.ForeColor = SystemColors.HighlightText;
          }
      }

      protected void removePaintFromNodes()
      {
          if (m_coll.Count == 0) return;

          TreeNode n0 = (TreeNode)m_coll[0];
          Color back = n0.TreeView.BackColor;
          Color fore = n0.TreeView.ForeColor;

          foreach (TreeNode n in m_coll)
          {
              n.BackColor = back;
              n.ForeColor = fore;
          }

      }
      protected bool isParent(TreeNode parentNode, TreeNode childNode)
      {
          if (parentNode == childNode)
              return true;

          TreeNode n = childNode;
          bool bFound = false;
          while (!bFound && n != null)
          {
              n = n.Parent;
              bFound = (n == parentNode);
          }
          return bFound;
      }
       //SQLCM -5470 v5.6 - END
   }
}