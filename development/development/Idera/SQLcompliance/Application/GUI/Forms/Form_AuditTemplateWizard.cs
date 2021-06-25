using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;
using Idera.SQLcompliance.Application.GUI.Controls;
using Idera.SQLcompliance.Application.GUI.Helper;
using Idera.SQLcompliance.Application.GUI.Properties;
using Idera.SQLcompliance.Application.GUI.SQL;
using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Agent;
using Idera.SQLcompliance.Core.Collector;
using Idera.SQLcompliance.Core.Event;
using Idera.SQLcompliance.Core.Status;
using Idera.SQLcompliance.Core.Templates.AuditTemplates;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
   public partial class Form_AuditTemplateWizard : Form
   {
      private ServerRecord _server = null;
      private DatabaseRecord _database = null;
      private TemplateSettings _settings = null;

      private bool _beforeAfterAvailable;
      private int _compatibilityLevel = -1;

      private string _oldDCTablesSnapshot = "";
      private string _oldSCTablesSnapshot = "";

      private Dictionary<string, DataChangeTableRecord> _oldBATables;
      private Dictionary<string, SensitiveColumnTableRecord> _oldSCTables;

      private ICollection _tableList = null;
      private Dictionary<string, DatabaseObjectRecord> _tableObjects = null;

      private bool _tryAgentCommunication = false;
      private SQLDirect _sqlServer = null;

      enum WizardPage
      {
         RegulationPage,
         BADPage,
         SCPage,
         Summary
      }

      private WizardPage currentPage = WizardPage.RegulationPage;

      /// <summary>
      /// 
      /// </summary>
      public Form_AuditTemplateWizard(DatabaseRecord database, ServerRecord server)
      {
         InitializeComponent();

         if (server != null)
            _server = server;
         else
         {
            _server = ServerRecord.GetServer(Globals.Repository.Connection, database.SrvId);
         }
         _tryAgentCommunication = _server.IsDeployed && _server.IsRunning;

         _database = database;
         _settings = new TemplateSettings();
         _sqlServer = new SQLDirect();

         this.Text = String.Format("Apply Audit Template for {0}..{1}", database.SrvInstance, database.Name);

         Initialize();
      }

      #region GeneralWizard

      private void Initialize()
      {
         _oldDCTablesSnapshot = Snapshot.GetDataChangeTables(Globals.Repository.Connection, _database.DbId, "\t\t");
         _oldSCTablesSnapshot = Snapshot.GetSensitiveColumnTables(Globals.Repository.Connection, _database.DbId, "\t\t");
         _compatibilityLevel = GetCompatibilityLevel();
         LoadSensitiveColumns();
         LoadBeforeAfterConfig();
         ShowPage();
      }

      /// <summary>
      /// 
      /// </summary>
      private void ShowPage()
      {
         auditTemplateWizard.Tabs[(int)currentPage].Selected = true;
         UpdateView();
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void previousButton_Click(object sender, EventArgs e)
      {
         currentPage--;
         ShowPage();
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void nextButton_Click(object sender, EventArgs e)
      {
         //increment to the next page.
         currentPage++;

         if (currentPage > WizardPage.Summary)
         {
            //if (SaveTemplate())
            //{
               this.DialogResult = DialogResult.OK;
               this.Close();
            //}
            //else
            //{
            //   currentPage--;
            //}
         }
         else
         {
            ShowPage();
         }
      }

      /// <summary>
      /// 
      /// </summary>
      /// <returns></returns>
      private bool SaveTemplate()
      {
         //this._rule.Name = this.ruleNameTextbox.Text;
         //this._rule.Description = this.ruleDescriptionTextbox.Text;
         //this._rule.Enabled = this.ruleEnabledCheckbox.Checked;
         return true;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void buttonCancel_Click(object sender, EventArgs e)
      {
         if (_sqlServer != null) 
            _sqlServer.CloseConnection();
         this.DialogResult = DialogResult.Cancel;
         this.Close();
      }

      /// <summary>
      /// 
      /// </summary>
      void UpdateView()
      {
         _pbClrStatus.Image = Resources.StatusGood_48;
         _lblClrStatus.Text = String.Format("CLR is enabled for {0}.", _database.SrvInstance);

         switch (currentPage)
         {
            case WizardPage.RegulationPage:
               {
                  this.previousButton.Enabled = false;
                  UpdateNext(WizardPage.RegulationPage);
                  this.nextButton.Text = "Next";
                  this.cancelButton.Enabled = true;
                  this.titleLabel.Text = "Select a Regulation";
                  this.descLabel.Text = "Select a regulation that you want this instance to comply with. If you select multiple regulations, the results will be and'd together.";
                  break;
               }
            case WizardPage.SCPage:
               {
                  this.previousButton.Enabled = true;
                  this.nextButton.Enabled = true;
                  this.nextButton.Text = "Next";
                  this.cancelButton.Enabled = true;
                  this.titleLabel.Text = "Sensitive Column Auditing";
                  this.descLabel.Text = "Setup Sensitive Column Auditing per section 10.2 of the regulation.";
                  break;
               }
            case WizardPage.BADPage:
               {
                  this.previousButton.Enabled = true;
                  this.nextButton.Enabled = true;
                  this.nextButton.Text = "Next";
                  this.cancelButton.Enabled = true;
                  this.titleLabel.Text = "Before/After Data Auditing";
                  this.descLabel.Text = "Setup Before/After Data Auditing per section 10.3 of the regulation.";
                  break;
               }
            case WizardPage.Summary:
               {
                  this.previousButton.Enabled = true;
                  this.nextButton.Enabled = true;
                  this.nextButton.Text = "Finish";
                  this.cancelButton.Enabled = true;
                  this.titleLabel.Text = "Audit Summary";
                  this.descLabel.Text = "This is a summary of the audit settings.";
                  InitSummary();
                  break;
               }
            default:
               {
                  this.previousButton.Enabled = false;
                  this.nextButton.Enabled = false;
                  this.cancelButton.Enabled = false;
                  break;
               }
         }
      }
      
      /// <summary>
      /// 
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>

      private void Form_AddStatusAlert_HelpRequested(object sender, System.Windows.Forms.HelpEventArgs hlpevent)
      {
         //string helpTopic = "";

         //switch (currentPage)
         //{
         //   case WizardPage.StartPage:
         //      helpTopic = HelpAlias.SSHELP_NewStatusAlertWizard_Type;
         //      break;
         //   case WizardPage.AlertActions:
         //      helpTopic = HelpAlias.SSHELP_NewStatusAlertWizard_Action;
         //      break;
         //   case WizardPage.FinishPage:
         //      helpTopic = HelpAlias.SSHELP_NewStatusAlertWizard_Finish;
         //      break;
         //}

         //if (helpTopic != "") 
         //     HelpAlias.ShowHelp(this, helpTopic);
         hlpevent.Handled = true;
      }

      #endregion

      #region Regulation

      private void pciCheck_CheckedChanged(object sender, EventArgs e)
      {
         if (pciCheck.Checked)
            _settings.regulationType |= Regulation.RegulationType.PCI;
         else
            _settings.regulationType ^= Regulation.RegulationType.PCI;
         UpdateNext(WizardPage.RegulationPage);
      }

      private void hipaaCheck_CheckedChanged(object sender, EventArgs e)
      {
         if (hipaaCheck.Checked)
            _settings.regulationType |= Regulation.RegulationType.HIPAA;
         else
            _settings.regulationType ^= Regulation.RegulationType.HIPAA;
         UpdateNext(WizardPage.RegulationPage);
      }

      private void UpdateNext(WizardPage page)
      {
         bool enabled = true;

         switch (page)
         {
            case WizardPage.RegulationPage:
               if (pciCheck.Checked == false &&
                   hipaaCheck.Checked == false)
                  enabled = false;
               break;
         }
         nextButton.Enabled = enabled;
      }

      #endregion
      
      #region SensitiveColumns

      private void LoadSensitiveColumns()
      {
         if (_server.SqlVersion == 0)
         {
            labelSCNotAvailable.Text = CoreConstants.Feature_SensitiveColumnNotAvailableVersionUnknown;
            panelSCNotAvailable.Visible = true;
            panelSCNotAvailable.BringToFront();
            return;
         }
         else if (!SupportsSensitiveColumns())
         {
            labelSCNotAvailable.Text = CoreConstants.Feature_SensitiveColumnNotAvailableAgent;
            panelSCNotAvailable.Visible = true;
            panelSCNotAvailable.BringToFront();
            return;
         }

         if (!LoadTables())
         {
            labelSCNotAvailable.Text = UIConstants.Error_CantLoadTables;
            panelSCNotAvailable.Visible = true;
            panelSCNotAvailable.BringToFront();
            return;
         }

         //load the Sensitive column info
         _oldSCTables = new Dictionary<string, SensitiveColumnTableRecord>();
         if (_database.AuditSensitiveColumns)
         {
            List<string> missingTables = new List<string>();
            List<SensitiveColumnTableRecord> tables = SensitiveColumnTableRecord.GetAuditedTables(Globals.Repository.Connection, _database.SrvId, _database.DbId);
            string tableName;

            foreach (SensitiveColumnTableRecord table in tables)
            {

               if (SupportsSchemas())
                  tableName = table.FullTableName;
               else
                  tableName = table.TableName;

               ListViewItem x = _lvSCTables.Items.Add(tableName);
               x.SubItems.Add(table.SelectedColumns ? Form_TableConfigure.GetColumnString(table.Columns) : UIConstants.SC_AllColumns);
               _oldSCTables.Add(tableName, table);

               if (!_tableObjects.ContainsKey(tableName))
               {
                  missingTables.Add(table.FullTableName);
                  x.Tag = null;
                  x.ForeColor = System.Drawing.Color.LightGray;
               }
               else
                  x.Tag = _tableObjects[tableName];
            }
            if (missingTables.Count == 1)
            {
               MessageBox.Show(String.Format(UIConstants.Warning_SC_Table_Missing, missingTables[0]), UIConstants.ADDTableTilte_SC, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (missingTables.Count > 1)
            {
               MessageBox.Show(String.Format(UIConstants.Warning_SC_Tables_Missing, String.Join(", ", missingTables.ToArray())), UIConstants.ADDTableTilte_SC, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
         }
         _lvSCTables_SelectedIndexChanged(null, null);
      }

      private bool SupportsSensitiveColumns()
      {
         if (_server == null ||
            String.IsNullOrEmpty(_server.AgentVersion) ||
            _server.AgentVersion.StartsWith("1") ||
            _server.AgentVersion.StartsWith("2") ||
            _server.AgentVersion.StartsWith("3.0") ||
            _server.AgentVersion.StartsWith("3.1") ||
            _server.AgentVersion.StartsWith("3.2") ||
            _server.AgentVersion.StartsWith("3.3"))
            return false;
         else
            return true;
      }

      private bool LoadTables()
      {
         // Attempt to load list of tables if we haven't tried already
         if (_tableList == null && _tableObjects == null)
         {
            _tableObjects = new Dictionary<string, DatabaseObjectRecord>();
            // try via connection to agent
            if (_tryAgentCommunication)
            {
               string url = "";
               try
               {
                  url = String.Format("tcp://{0}:{1}/{2}",
                                       Globals.SQLcomplianceConfig.Server,
                                       Globals.SQLcomplianceConfig.ServerPort,
                                       typeof(AgentManager).Name);
                  AgentManager manager = (AgentManager)Activator.GetObject(
                     typeof(AgentManager),
                     url);

                  _tableList = manager.GetRawTables(_server.Instance, _database.Name);
               }
               catch (Exception ex)
               {
                  ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                          String.Format("LoadTables: URL: {0} Instance {1} Database {2}", url, _server.Instance, _database.Name),
                                          ex,
                                          ErrorLog.Severity.Warning);
                  _tableList = null;
               }
            }

            // straight connection to SQL Server
            if (_tableList == null)
            {
               if (_sqlServer.OpenConnection(_server.Instance))
               {
                  _tableList = RawSQL.GetTables(_sqlServer.Connection, _database.Name);
               }
            }

            bool supportsSchemas = SupportsSchemas();
            if (_tableList != null)
            {
               foreach (RawTableObject rto in _tableList)
               {
                  DatabaseObjectRecord dbo = new DatabaseObjectRecord(rto);
                  dbo.DbId = _database.DbId;
                  if (supportsSchemas)
                     _tableObjects.Add(dbo.FullTableName, dbo);
                  else
                     _tableObjects.Add(dbo.TableName, dbo);
               }
            }
         }
         return (_tableList != null && _tableObjects != null);
      }

      private bool SupportsSchemas()
      {
         if (_server == null || _server.SqlVersion < 9)
            return false;

         return SupportsBeforeAfter();
      }

      private void _lvSCTables_SelectedIndexChanged(object sender, EventArgs e)
      {
         if (_lvSCTables.SelectedItems.Count == 0)
         {
            _btnRemoveSCTable.Enabled = false;
            _btnEditSCTable.Enabled = false;
         }
         else
         {
            _btnRemoveSCTable.Enabled = true;
            _btnEditSCTable.Enabled = _lvSCTables.SelectedItems.Count == 1 && _lvSCTables.SelectedItems[0].Tag != null ? true : false;
         }
      }

      private void _btnAddSCTable_Click(object sender, EventArgs e)
      {
         if (!LoadTables())
         {
            ErrorMessage.Show(this.Text, UIConstants.Error_CantLoadTables, SQLDirect.GetLastError());
            _btnAddSCTable.Enabled = false;
            return;
         }
         List<string> selectedTables = new List<string>();
         List<RawTableObject> tableList = new List<RawTableObject>();

         foreach (ListViewItem tableItem in _lvSCTables.Items)
            selectedTables.Add(tableItem.Text);

         foreach (RawTableObject rto in _tableList)
         {
            tableList.Add(rto);
         }

         Form_TableAdd frm = new Form_TableAdd(tableList, selectedTables, SupportsSchemas(), UIConstants.Table_Column_Usage.SensitiveColumns);

         if (DialogResult.OK == frm.ShowDialog())
         {
            Dictionary<string, string> currentColumns = new Dictionary<string, string>();
            _lvSCTables.BeginUpdate();

            foreach (ListViewItem item in _lvSCTables.Items)
            {
               currentColumns[item.Text] = item.SubItems[1].Text;
            }
            _lvSCTables.Items.Clear();

            foreach (string tableName in frm.SelectedTables)
            {
               ListViewItem x = _lvSCTables.Items.Add(tableName);

               if (currentColumns.ContainsKey(tableName))
               {
                  x.SubItems.Add(currentColumns[tableName]);
               }
               else
               {
                  // This is a new table, so add with the default values
                  x.SubItems.Add(UIConstants.BAD_AllColumns);
               }

               if (_tableObjects.ContainsKey(tableName))
               {
                  x.Tag = _tableObjects[tableName];
               }
               else
                  x.Tag = null;
            }
            _lvSCTables.Focus();

            if (_lvSCTables.Items.Count > 0)
            {
               _lvSCTables.TopItem.Selected = true;
            }
            _lvSCTables.EndUpdate();
         }
      }

      private void _btnRemoveSCTable_Click(object sender, EventArgs e)
      {
         if (_lvSCTables.SelectedItems.Count == 0)
            return;
         _lvSCTables.BeginUpdate();

         int ndx = _lvSCTables.SelectedItems[0].Index;


         foreach (ListViewItem table in _lvSCTables.SelectedItems)
         {
            table.Remove();
         }

         _lvSCTables.EndUpdate();

         // reset selected item
         if (_lvSCTables.Items.Count != 0)
         {
            _lvSCTables.Focus();
            if (ndx >= _lvSCTables.Items.Count)
            {
               _lvSCTables.Items[_lvSCTables.Items.Count - 1].Selected = true;
            }
            else
               _lvSCTables.Items[ndx].Selected = true;
         }
      }

      private void _btnEditSCTable_Click(object sender, EventArgs e)
      {
         ListView.SelectedListViewItemCollection items = _lvSCTables.SelectedItems;

         if (items.Count == 0)
            return;

         string tableName = items[0].SubItems[0].Text;
         RawTableObject tbl = null;

         bool schemas = SupportsSchemas();
         foreach (RawTableObject rto in _tableList)
         {
            if (schemas)
            {
               if (rto.FullTableName == tableName)
               {
                  tbl = rto;
                  break;
               }
            }
            else
            {
               if (rto.TableName == tableName)
               {
                  tbl = rto;
                  break;
               }
            }
         }

         if (tbl == null)
         {
            MessageBox.Show(String.Format(UIConstants.Error_Cant_Edit_BAD_Table, tableName), UIConstants.ADDTableTilte_SC, MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
         }

         string[] selectedColumns = Form_TableConfigure.GetColumns(items[0].SubItems[1].Text);

         IList columns = LoadColumns(tableName);
         using (Form_TableConfigure frm = new Form_TableConfigure(tbl, 1, columns, selectedColumns, UIConstants.Table_Column_Usage.SensitiveColumns))
         {
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
               string colList = string.Empty;
               if (frm.AllColumns)
               {
                  colList = UIConstants.BAD_AllColumns;
               }
               else
               {
                  colList = frm.SelectedColumnsString;
               }
               items[0].SubItems[1].Text = colList;
               items[0].ForeColor = System.Drawing.SystemColors.WindowText;
            }
         }
      }

      private IList LoadColumns(string tableName)
      {
         IList columnList = null;
         // Load list of columns for the table
         // try via connection to agent
         if (_tryAgentCommunication)
         {
            string url = "";
            try
            {
               url = String.Format("tcp://{0}:{1}/{2}",
                                    Globals.SQLcomplianceConfig.Server,
                                    Globals.SQLcomplianceConfig.ServerPort,
                                    typeof(AgentManager).Name);
               AgentManager manager = (AgentManager)Activator.GetObject(
                  typeof(AgentManager),
                  url);

               columnList = manager.GetRawColumns(_server.Instance, _database.Name, tableName);
            }
            catch (Exception ex)
            {
               ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                       String.Format("LoadColumns: URL: {0} Instance {1} Database {2} Table {3}", url, _server.Instance, _database.Name, tableName),
                                       ex,
                                       ErrorLog.Severity.Warning);
               columnList = null;
            }
         }

         // straight connection to SQL Server
         if (columnList == null)
         {
            if (_sqlServer.OpenConnection(_server.Instance))
            {
               columnList = RawSQL.GetColumns(_sqlServer.Connection, _database.Name, tableName);
            }
         }
         return columnList;
      }
      #endregion

      #region BAD
      // Returns true if the agent is able to support BeforeAfter data collection (3.1 and beyond)
      private bool SupportsBeforeAfter()
      {
         if (_server == null ||
            String.IsNullOrEmpty(_server.AgentVersion) ||
            _server.AgentVersion.StartsWith("1") ||
            _server.AgentVersion.StartsWith("2") ||
            _server.AgentVersion.StartsWith("3.0"))
            return false;
         else
            return true;
      }

      // Returns true if the agent is able to support BeforeAfter data collection by column (3.2 and beyond)
      private bool SupportsBeforeAfterColumns()
      {
         if (_server == null ||
            String.IsNullOrEmpty(_server.AgentVersion) ||
            _server.AgentVersion.StartsWith("1") ||
            _server.AgentVersion.StartsWith("2") ||
            _server.AgentVersion.StartsWith("3.0") ||
            _server.AgentVersion.StartsWith("3.1"))
            return false;
         else
            return true;
      }

      private void LoadBeforeAfterConfig()
      {
         _beforeAfterAvailable = true;
         // SQL Server 2005,2008
         if (_server.SqlVersion == 0)
         {
            labelSCNotAvailable.Text = CoreConstants.Feature_BeforeAfterNotAvailableVersionUnknown;
            panelSCNotAvailable.Visible = true;
            panelSCNotAvailable.BringToFront();
            _beforeAfterAvailable = false;
            return;
         }
         else if (_server.SqlVersion < 9)
         {
            labelSCNotAvailable.Text = CoreConstants.Feature_BeforeAfterNotAvailable;
            panelSCNotAvailable.Visible = true;
            panelSCNotAvailable.BringToFront();
            _beforeAfterAvailable = false;
            return;
         }
         else if (_compatibilityLevel < 90)
         {
            labelSCNotAvailable.Text = CoreConstants.Feature_BeforeAfterNotAvailableCompatibility;
            panelSCNotAvailable.Visible = true;
            panelSCNotAvailable.BringToFront();
            _beforeAfterAvailable = false;
            return;
         }
         else if (!SupportsBeforeAfter())
         {
            labelSCNotAvailable.Text = CoreConstants.Feature_BeforeAfterNotAvailableAgent;
            panelSCNotAvailable.Visible = true;
            panelSCNotAvailable.BringToFront();
            _beforeAfterAvailable = false;
            return;
         }
         if (!LoadTables())
         {
            labelSCNotAvailable.Text = UIConstants.Error_CantLoadTables;
            panelSCNotAvailable.Visible = true;
            panelSCNotAvailable.BringToFront();
            _beforeAfterAvailable = false;
            return;
         }
         else
         {
            UpdateClrStatus();
         }
         auditTemplateWizard.Tabs[(int)WizardPage.BADPage].Enabled = _database.AuditDML;

         //Load the BAD info
         _oldBATables = new Dictionary<string, DataChangeTableRecord>();
         if (_database.AuditDataChanges)
         {
            List<string> missingTables = new List<string>();
            List<DataChangeTableRecord> tables = DataChangeTableRecord.GetAuditedTables(Globals.Repository.Connection, _server.SrvId, _database.DbId);
            foreach (DataChangeTableRecord table in tables)
            {
               ListViewItem x = _lvBeforeAfterTables.Items.Add(table.FullTableName);
               x.SubItems.Add(Form_MaxRows.GetMaxRowString(table.RowLimit));
               if (SupportsBeforeAfterColumns())
               {
                  x.SubItems.Add(table.SelectedColumns ? Form_TableConfigure.GetColumnString(table.Columns) : UIConstants.BAD_AllColumns);
               }
               _oldBATables.Add(table.FullTableName, table);
               if (!_tableObjects.ContainsKey(table.FullTableName))
               {
                  missingTables.Add(table.FullTableName);
                  x.Tag = null;
                  x.ForeColor = System.Drawing.Color.LightGray;
               }
               else
                  x.Tag = _tableObjects[table.FullTableName];
            }
            if (missingTables.Count == 1)
            {
               MessageBox.Show(String.Format(UIConstants.Warning_BAD_Table_Missing, missingTables[0]), UIConstants.BAD_TABLE_Missing_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (missingTables.Count > 1)
            {
               MessageBox.Show(String.Format(UIConstants.Warning_BAD_Tables_Missing, String.Join(", ", missingTables.ToArray())), UIConstants.BAD_TABLE_Missing_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
         }

         // Update button state
         _lvBeforeAfterTables_SelectedIndexChanged(null, null);
         UpdateBeforeAfterAvailability();
      }

      private void UpdateBeforeAfterAvailability()
      {
         // If it is not enabled, the feature is unavailable for other reasones (LoadBeforeAfterConfig)
         if (!_beforeAfterAvailable)
         {
            return;
         }

         // Update Before After availability based on user-table auditing
         // Make sure user tables are not audited and that BA is available otherwise (agent version, SQL Server version)
         // Remove the column names column if they are not supported
         if (SupportsBeforeAfterColumns())
         {
            _lvBeforeAfterTables.Columns[2].Width = 169;
         }
         else
         {
            _lvBeforeAfterTables.Columns[0].Width = 268;

            //Remove the columns column if this is a 3.1 agent.
            if (_lvBeforeAfterTables.Columns.ContainsKey("_columnColumnNames"))
               _lvBeforeAfterTables.Columns.RemoveByKey("_columnColumnNames");
         }
      }
      private void _lvBeforeAfterTables_SelectedIndexChanged(object sender, EventArgs e)
      {

         if (_lvBeforeAfterTables.SelectedItems.Count == 0)
         {
            _btnRemoveBATable.Enabled = false;
            _btnEditBATable.Enabled = false;
         }
         else
         {
            _btnRemoveBATable.Enabled = true;
            _btnEditBATable.Enabled = _lvBeforeAfterTables.SelectedItems.Count == 1 && _lvBeforeAfterTables.SelectedItems[0].Tag != null ? true : false;
         }
      }

      private void _btnAddBATable_Click(object sender, EventArgs e)
      {
         if (!LoadTables())
         {
            ErrorMessage.Show(this.Text, UIConstants.Error_CantLoadTables, SQLDirect.GetLastError());
            _btnAddBATable.Enabled = false;
            return;
         }
         List<string> selectedTables = new List<string>();
         List<RawTableObject> tableList = new List<RawTableObject>();
         
         if (SupportsBeforeAfterColumns())
         {
            foreach (ListViewItem tableItem in _lvBeforeAfterTables.Items)
               selectedTables.Add(tableItem.Text);
         
            if (_database.AuditDML && _database.AuditUserTables == 1)
            {
               ICollection userTables = DatabaseObjectRecord.GetUserTables(_database.DbId);

               foreach (RawTableObject rto in _tableList)
               {
                  foreach (DatabaseObjectRecord userTable in userTables)
                  {
                     if (userTable.TableName.Equals(rto.FullTableName))
                        tableList.Add(rto);
                  }
               }
            }
            else
            {
               foreach (RawTableObject rto in _tableList)
               {
                  tableList.Add(rto);
               }
            }
         }
         else
         {
            List<string> blobTables;

            try
            {
               blobTables = GetBlobTables();
            }
            catch (Exception ex)
            {
               ErrorMessage.Show(this.Text, UIConstants.Error_CantLoadTables, ex.Message);
               _btnAddBATable.Enabled = false;
               return;
            }

            foreach (ListViewItem tableItem in _lvBeforeAfterTables.Items)
               selectedTables.Add(tableItem.Text);

            if (_database.AuditDML && _database.AuditUserTables == 1)
            {
               ICollection userTables = DatabaseObjectRecord.GetUserTables(_database.DbId);

               foreach (RawTableObject rto in _tableList)
               {
                  foreach (DatabaseObjectRecord userTable in userTables)
                  {
                     if (userTable.TableName.Equals(rto.FullTableName) &&
                                 !blobTables.Contains(rto.FullTableName))
                        tableList.Add(rto);
                  }
               }
            }
            else
            {
               foreach (RawTableObject rto in _tableList)
               {
                  if (!blobTables.Contains(rto.FullTableName))
                     tableList.Add(rto);
               }
            }
         }
         Form_TableAdd frm = new Form_TableAdd(tableList, selectedTables, SupportsSchemas(), SupportsBeforeAfterColumns() ? UIConstants.Table_Column_Usage.BADColumns : UIConstants.Table_Column_Usage.BADTables);

         if (DialogResult.OK == frm.ShowDialog())
         {
            Dictionary<string, string> currentTables = new Dictionary<string, string>();
            Dictionary<string, string> currentColumns = new Dictionary<string, string>();
            _lvBeforeAfterTables.BeginUpdate();

            foreach (ListViewItem item in _lvBeforeAfterTables.Items)
            {
               currentTables[item.Text] = item.SubItems[1].Text;
               if (SupportsBeforeAfterColumns())
               {
                  currentColumns[item.Text] = item.SubItems[2].Text;
               }
            }

            _lvBeforeAfterTables.Items.Clear();

            foreach (string tableName in frm.SelectedTables)
            {
               ListViewItem x = _lvBeforeAfterTables.Items.Add(tableName);

               if (currentTables.ContainsKey(tableName))
               {
                  x.SubItems.Add(currentTables[tableName]);

                  if (SupportsBeforeAfterColumns())
                  {
                     x.SubItems.Add(currentColumns[tableName]);

                     if (currentColumns[tableName] == UIConstants.BAD_NoColumns)
                     {
                        x.ForeColor = System.Drawing.Color.Red;
                     }
                  }
               }
               else
               {
                  // This is a new table, so add with the default values
                  x.SubItems.Add("10");
                  if (SupportsBeforeAfterColumns())
                  {
                     RawTableObject tbl = null;
                     foreach (RawTableObject rto in tableList)
                     {
                        if (rto.FullTableName == tableName)
                        {
                           tbl = rto;
                           break;
                        }
                     }

                     string cols = string.Empty;
                     if (tbl == null || !tbl.HasBlobData)
                        x.SubItems.Add(UIConstants.BAD_AllColumns);
                     else
                     {
                        x.SubItems.Add(UIConstants.BAD_NoColumns);
                        x.ForeColor = System.Drawing.Color.Red;
                     }
                  }
               }

               if (_tableObjects.ContainsKey(tableName))
               {
                  x.Tag = _tableObjects[tableName];
               }
               else
                  x.Tag = null;
            }

            _lvBeforeAfterTables.Focus();
            if (_lvBeforeAfterTables.Items.Count > 0)
            {
               _lvBeforeAfterTables.TopItem.Selected = true;
            }

            _lvBeforeAfterTables.EndUpdate();
         }
      }

      private void Click_btnRemoveBATable(object sender, EventArgs e)
      {
         if (_lvBeforeAfterTables.SelectedItems.Count == 0)
            return;
         _lvBeforeAfterTables.BeginUpdate();

         int ndx = _lvBeforeAfterTables.SelectedItems[0].Index;


         foreach (ListViewItem table in _lvBeforeAfterTables.SelectedItems)
         {
            table.Remove();
         }

         _lvBeforeAfterTables.EndUpdate();

         // reset selected item
         if (_lvBeforeAfterTables.Items.Count != 0)
         {
            _lvBeforeAfterTables.Focus();
            if (ndx >= _lvBeforeAfterTables.Items.Count)
            {
               _lvBeforeAfterTables.Items[_lvBeforeAfterTables.Items.Count - 1].Selected = true;
            }
            else
               _lvBeforeAfterTables.Items[ndx].Selected = true;
         }
      }

      private void UpdateClrStatus()
      {
         bool configured, running;

         try
         {
            GetCLRStatus(out configured, out running);
         }
         catch (Exception e)
         {
            //server not reachable
            ErrorLog.Instance.Write(String.Format("Unable to contact {0} to determine CLR Enabled status.", _server.Instance),
               e, ErrorLog.Severity.Warning);
            _pbClrStatus.Image = Resources.StatusError_48;
            _lblClrStatus.Text = String.Format("{0} cannot be reached currently.", _server.Instance);
            _btnEnableCLR.Enabled = false;
            return;
         }

         if (configured != running)
         {
            // RECONFIGURE failed it appears
            _pbClrStatus.Image = Resources.StatusWarning_48;
            if (configured)
            {
               _lblClrStatus.Text = "CLR cannot be enabled.  Verify that lightweight pooling is disabled.";
               EnableClr(false);
            }
            else
            {
               _lblClrStatus.Text = "CLR is running but not conifgured.";
            }
            _btnEnableCLR.Enabled = !configured;
            _btnAddBATable.Enabled = running;
            _btnRemoveBATable.Enabled = running;
         }
         else if (running)
         {
            _pbClrStatus.Image = Resources.StatusGood_48;
            _lblClrStatus.Text = String.Format("CLR is enabled for {0}.", _server.Instance);
            _btnEnableCLR.Enabled = false;
            _btnAddBATable.Enabled = true;
            _btnRemoveBATable.Enabled = _lvBeforeAfterTables.SelectedItems.Count > 0;
         }
         else
         {
            _pbClrStatus.Image = Resources.StatusWarning_48;
            _lblClrStatus.Text = String.Format("CLR is not enabled for {0}.", _server.Instance);
            _btnEnableCLR.Enabled = true;
            _btnAddBATable.Enabled = false;
            _btnRemoveBATable.Enabled = false;
         }
      }

      private void Click_btnEditBATable(object sender, EventArgs e)
      {
         ListView.SelectedListViewItemCollection items = _lvBeforeAfterTables.SelectedItems;

         if (items.Count == 0)
            return;

         if (SupportsBeforeAfterColumns())
         {
            string tableName = items[0].SubItems[0].Text;
            int rows = Form_TableConfigure.GetMaxRows(items[0].SubItems[1].Text);

            RawTableObject tbl = null;
            foreach (RawTableObject rto in _tableList)
            {
               if (rto.FullTableName == tableName)
               {
                  tbl = rto;
                  break;
               }
            }

            if (tbl == null)
            {
               MessageBox.Show(String.Format(UIConstants.Error_Cant_Edit_BAD_Table, tableName), UIConstants.BAD_TABLE_Missing_Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
               return;
            }

            string[] selectedColumns = Form_TableConfigure.GetColumns(items[0].SubItems[2].Text);

            IList columns = LoadColumns(tableName);
            using (Form_TableConfigure frm = new Form_TableConfigure(tbl, rows, columns, selectedColumns, UIConstants.Table_Column_Usage.BADColumns))
            {
               if (frm.ShowDialog(this) == DialogResult.OK)
               {
                  items[0].SubItems[1].Text = Form_TableConfigure.GetMaxRowString(frm.MaximumRows);
                  string colList = string.Empty;
                  if (frm.AllColumns)
                  {
                     colList = UIConstants.BAD_AllColumns;
                  }
                  else
                  {
                     colList = frm.SelectedColumnsString;
                  }
                  items[0].SubItems[2].Text = colList;
                  if (colList == UIConstants.BAD_NoColumns)
                     items[0].ForeColor = System.Drawing.Color.Red;
                  else
                     items[0].ForeColor = System.Drawing.SystemColors.WindowText;
               }
            }
         }
         else
         {
            int rows = Form_MaxRows.GetMaxRows(items[0].SubItems[1].Text);
            using (Form_MaxRows frm = new Form_MaxRows(rows))
            {
               if (frm.ShowDialog(this) == DialogResult.OK)
               {
                  foreach (ListViewItem i in items)
                  {
                     i.SubItems[1].Text = Form_MaxRows.GetMaxRowString(frm.MaximumRows);
                  }
               }
            }
         }
      }

      private void Click_btnEnableCLR(object sender, EventArgs e)
      {
         EnableClr(true);
         UpdateClrStatus();
      }

      private void EnableClr(bool enable)
      {
         // Try agent first if allowed
         if (_tryAgentCommunication)
         {
            string url = "";
            try
            {
               url = String.Format("tcp://{0}:{1}/{2}",
                                    Globals.SQLcomplianceConfig.Server,
                                    Globals.SQLcomplianceConfig.ServerPort,
                                    typeof(AgentManager).Name);
               AgentManager manager = (AgentManager)Activator.GetObject(typeof(AgentManager), url);

               manager.EnableCLR(_server.Instance, enable);
               return;
            }
            catch (Exception ex)
            {
               ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                       String.Format("EnableClr: URL: {0} Instance {1} Database {2}", url, _server.Instance, _database.Name),
                                       ex,
                                       ErrorLog.Severity.Warning);
            }
         }

         // Now we try direct connect
         if (!_sqlServer.OpenConnection(_server.Instance))
            throw new Exception("Unable to open a connection to server.");

         try
         {
            RawSQL.EnableCLR(_sqlServer.Connection, enable);
         }
         finally
         {
            _sqlServer.CloseConnection();
         }
      }

      private void GetCLRStatus(out bool configured, out bool running)
      {
         // Try agent first if allowed
         if (_tryAgentCommunication)
         {
            string url = "";
            try
            {
               url = String.Format("tcp://{0}:{1}/{2}",
                                    Globals.SQLcomplianceConfig.Server,
                                    Globals.SQLcomplianceConfig.ServerPort,
                                    typeof(AgentManager).Name);
               AgentManager manager = (AgentManager)Activator.GetObject(typeof(AgentManager), url);

               manager.GetCLRStatus(_server.Instance, out configured, out running);
               return;
            }
            catch (Exception ex)
            {
               ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                       String.Format("IsCLREnabled: URL: {0} Instance {1} Database {2}", url, _server.Instance, _database.Name),
                                       ex,
                                       ErrorLog.Severity.Warning);
            }
         }

         // Now we try direct connect
         if (!_sqlServer.OpenConnection(_server.Instance))
            throw new Exception("Unable to open a connection to server.");

         RawSQL.GetCLRStatus(_sqlServer.Connection, out configured, out running);
         _sqlServer.CloseConnection();
      }

      private int GetCompatibilityLevel()
      {
         int retVal;
         // Try agent first if allowed
         if (_tryAgentCommunication)
         {
            string url = "";
            try
            {
               url = String.Format("tcp://{0}:{1}/{2}",
                                    Globals.SQLcomplianceConfig.Server,
                                    Globals.SQLcomplianceConfig.ServerPort,
                                    typeof(AgentManager).Name);
               AgentManager manager = (AgentManager)Activator.GetObject(typeof(AgentManager), url);

               return manager.GetCompatibilityLevel(_server.Instance, _database.Name);
            }
            catch (Exception ex)
            {
               ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                       String.Format("GetCompatibilityLevel: URL: {0} Instance {1} Database {2}", url, _server.Instance, _database.Name),
                                       ex,
                                       ErrorLog.Severity.Warning);
            }
         }
         try
         {
            // Now we try direct connect
            if (!_sqlServer.OpenConnection(_server.Instance))
               throw new Exception("Unable to open a connection to server.");

            retVal = RawSQL.GetCompatibilityLevel(_sqlServer.Connection, _database.Name);
            _sqlServer.CloseConnection();
         }
         catch (Exception ex)
         {
            ErrorLog.Instance.Write(ErrorLog.Level.Default,
                                       String.Format("GetCompatibilityLevel Direct: Instance {0} Database {1}", _server.Instance, _database.Name),
                                       ex,
                                       ErrorLog.Severity.Error);
            retVal = -1;
         }
         return retVal;
      }

      private List<string> GetBlobTables()
      {
         List<string> retVal;
         // Try agent first if allowed
         if (_tryAgentCommunication)
         {
            string url = "";
            try
            {
               url = String.Format("tcp://{0}:{1}/{2}",
                                    Globals.SQLcomplianceConfig.Server,
                                    Globals.SQLcomplianceConfig.ServerPort,
                                    typeof(AgentManager).Name);
               AgentManager manager = (AgentManager)Activator.GetObject(typeof(AgentManager), url);

               return manager.GetBlobTables(_server.Instance, _database.Name);
            }
            catch (Exception ex)
            {
               ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                       String.Format("GetBlobTables: URL: {0} Instance {1} Database {2}", url, _server.Instance, _database.Name),
                                       ex,
                                       ErrorLog.Severity.Warning);
            }
         }

         // Now we try direct connect
         if (!_sqlServer.OpenConnection(_server.Instance))
            throw new Exception("Unable to open a connection to server.");

         retVal = RawSQL.GetBlobTables(_sqlServer.Connection, _database.Name);
         _sqlServer.CloseConnection();
         return retVal;
      }
   
      #endregion

      #region Summary

      private void InitSummary()
      {
         lblServerSummary.Text = _server.Instance;
         lblDatabaseSummary.Text = _database.Name;
         lblRegulationSummary.Text = GetRegs();
         lblBAD.Text = _lvBeforeAfterTables.Items.Count > 0 ? "Yes" : "No";
         lblSC.Text = _lvSCTables.Items.Count > 0 ? "Yes" : "No";
      }

      private string GetRegs()
      {
         StringBuilder regs = new StringBuilder();

         if ((_settings.RegulationType & TemplateSettings.Regulation.PCI) == TemplateSettings.Regulation.PCI)
            regs.Append(AlertUIHelper.GetEnumDescription(TemplateSettings.Regulation.PCI));

         if ((_settings.RegulationType & TemplateSettings.Regulation.HIPAA) == TemplateSettings.Regulation.HIPAA)
            regs.AppendFormat(", {0}", AlertUIHelper.GetEnumDescription(TemplateSettings.Regulation.HIPAA));

         return regs.ToString();
      }

      private void summaryLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
      {
         MessageBox.Show(this, "TODO:This is a summary of the regulation compared to SQLcm settings.", "Summary");
      }

      #endregion

   }
}